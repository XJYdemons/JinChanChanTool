using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices; 
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace JinChanChanTool.Services
{
    /// <summary>
    /// 这个类封装了所有与网络API交互的逻辑，是执行数据获取的“工人”。
    /// </summary>
    public class CrawlingService : ICrawlingService
    {
        // 在整个应用程序中共享一个HttpClient实例
        // static readonly 确保它只被创建一次，性能更高
        private static readonly HttpClient _httpClient = new HttpClient();

        // 用于存放从外部注入的、提供本地配置数据的服务。
        private readonly IApiRequestPayloadDataService _payloadDataService;

        /// <summary>
        /// 构造函数，用于实现依赖注入。
        /// </summary>
        /// <param name="payloadDataService">一个实现了 IApiRequestPayloadDataService 接口的对象。</param>
        public CrawlingService(IApiRequestPayloadDataService payloadDataService)
        {
            // 将外部传入的服务实例保存到私有字段中，供后续方法使用。
            _payloadDataService = payloadDataService;
        }

        /// <summary>
        /// (核心方法) 异步执行完整的网络爬取流程。
        /// </summary>
        public async Task<List<HeroEquipment>> GetEquipmentsAsync(IProgress<Tuple<int, string>> progress)
        {
            System.Diagnostics.Debug.WriteLine("CrawlingService: 开始请求英雄列表API...");

            ListApiResponse listResponse;
            try
            {
                string listApiUrl = "https://api.datatft.com/data/explore/list";

                var listPayload = new ListApiRequestPayload();
                string jsonListPayload = System.Text.Json.JsonSerializer.Serialize(listPayload);
                var payload = new StringContent(jsonListPayload, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(listApiUrl, payload);
                response.EnsureSuccessStatusCode(); // 确保HTTP响应状态码为 2xx

                string jsonResponse = await response.Content.ReadAsStringAsync();
                listResponse = System.Text.Json.JsonSerializer.Deserialize<ListApiResponse>(jsonResponse);

                if (listResponse?.Data?.Units == null || listResponse.Data.Units.Count == 0)
                {
                    // 在服务层，我们不直接弹出MessageBox，而是记录日志并返回空结果。
                    // UI层可以根据返回的空结果来决定是否提示用户。
                    System.Diagnostics.Debug.WriteLine("错误: 未能从英雄列表API获取到任何英雄数据。");
                    return new List<HeroEquipment>(); // 返回空列表表示失败
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"严重错误: 请求英雄列表API时发生异常: {ex.Message}");
                // 同样，返回空列表
                return new List<HeroEquipment>();
            }


            // 整合数据，准备并行抓取
            System.Diagnostics.Debug.WriteLine("CrawlingService: 开始整合API数据与本地映射...");

            // 从注入的 _payloadDataService 中获取已加载的映射字典
            var heroKeyToNameMap = _payloadDataService.HeroKeyToNameMap;
            var heroIdToKeyMap = _payloadDataService.HeroIdToKeyMap;

            // 反转 HeroIdToKeyMap 以便通过Key快速查找ID
            var keyToIdMap = heroIdToKeyMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            var heroBlacklist = new HashSet<string> { "璐璐" };

            // 使用 LINQ 从API获取的英雄列表中，筛选出我们本地有完整映射信息的英雄
            var heroesToScrape = listResponse.Data.Units
                .Select(unit => new {
                    Key = unit.Key,
                    Name = heroKeyToNameMap.GetValueOrDefault(unit.Key),
                    Id = keyToIdMap.GetValueOrDefault(unit.Key)
                })
                .Where(hero =>
                    !string.IsNullOrEmpty(hero.Name) &&
                    !string.IsNullOrEmpty(hero.Id) &&
                    !heroBlacklist.Contains(hero.Name))
                .ToList();

            int totalHeroes = heroesToScrape.Count;
            System.Diagnostics.Debug.WriteLine($"API返回 {listResponse.Data.Units.Count} 位英雄，本地成功匹配到 {totalHeroes} 位准备抓取。");

            if (totalHeroes == 0)
            {
                System.Diagnostics.Debug.WriteLine("警告: API返回了英雄数据，但本地映射文件未能匹配到任何英雄，请检查映射文件是否过时。");
                return new List<HeroEquipment>(); // 没有可处理的英雄，直接返回
            }


            // 并行请求 /detail API 获取装备详情
            System.Diagnostics.Debug.WriteLine("CrawlingService: 开始并行请求装备详情API...");

            // 从注入的服务获取装备翻译字典
            var equipmentApiNameMap = _payloadDataService.EquipmentApiNameMap;
            var finalHeroEquipments = new System.Collections.Concurrent.ConcurrentBag<HeroEquipment>();

            const int MAX_CONCURRENT_TASKS = 1;//限制并发数量，防止被拉黑ip，最高测试3也不拉黑，但是1够快了，所以默认为1
            var semaphore = new System.Threading.SemaphoreSlim(MAX_CONCURRENT_TASKS);
            var tasks = new List<Task>();
            var processedCount = 0;//线程安全的计数器
            foreach (var hero in heroesToScrape)
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        int currentCount = System.Threading.Interlocked.Increment(ref processedCount);
                        int percentage = (int)((double)currentCount / totalHeroes * 100);
                        string message = $"({currentCount}/{totalHeroes}) 正在请求: {hero.Name}";
                        progress?.Report(Tuple.Create(percentage, message)); // ?.确保progress不为null

                        var items = await FetchEquipmentFromApiAsync(hero.Id, hero.Key, equipmentApiNameMap);

                        if (items.Any())
                        {
                            finalHeroEquipments.Add(new HeroEquipment
                            {
                                HeroName = hero.Name,
                                Equipments = items
                            });
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }
            await Task.WhenAll(tasks);

            progress?.Report(Tuple.Create(100, "所有数据处理完毕！"));
            // 将最终结果从 ConcurrentBag 转换为 List 并返回
            return finalHeroEquipments.ToList();
        }

        /// <summary>
        /// (辅助方法) 异步获取单个英雄的推荐装备。
        /// </summary>
        private async Task<List<string>> FetchEquipmentFromApiAsync(string heroId, string heroKey, Dictionary<string, string> equipmentMap)
        {
            try
            {
                const string apiUrl = "https://api.datatft.com/data/explore/detail";
                var payload = new ApiRequestPayload { Id = heroId, Key = heroKey };
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(jsonResponse);

                var equipBuild = apiResponse?.Data?.EquipBuilds?.FirstOrDefault(build => build.Key != null && build.Key.Count == 3);

                if (equipBuild != null)
                {
                    return equipBuild.Key
                        .Select(itemApiKey => equipmentMap.GetValueOrDefault(itemApiKey, $"【翻译失败:{itemApiKey}】"))
                        .ToList();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" -> 错误: 处理英雄 {heroKey} 时发生异常: {ex.Message}");
            }

            return new List<string>(); // 任何失败情况都返回空列表
        }

        // 用于构建发送到API的POST请求体
        public class ApiRequestPayload
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("key")]
            public string Key { get; set; }

            [JsonPropertyName("filterTargetType")]
            public string FilterTargetType { get; set; } = "hero";

            [JsonPropertyName("version")]
            public string Version { get; set; } = "15.3"; // 注意：此版本号需要更新,随版本更新

            [JsonPropertyName("tier")]
            public string Tier { get; set; } = "diamond";
        }

        // 用于接收API响应的顶层结构
        public class ApiResponse
        {
            [JsonPropertyName("data")]
            public ApiData Data { get; set; }
        }

        // 用于接收API响应中 "data" 字段的内容
        public class ApiData
        {
            [JsonPropertyName("equipBuilds")]
            public List<EquipBuild> EquipBuilds { get; set; }
        }

        // 用于接收 "equipBuilds" 数组中的每个装备组合
        public class EquipBuild
        {
            [JsonPropertyName("key")]
            public List<string> Key { get; set; }
        }

        // 用于构建发送到 /list API 的POST请求体
        public class ListApiRequestPayload
        {
            [JsonPropertyName("version")]
            public string Version { get; set; } = "15.3"; // 这个版本号需要更新

            [JsonPropertyName("tier")]
            public string Tier { get; set; } = "diamond";

            [JsonPropertyName("filterTargetType")]
            public string FilterTargetType { get; set; } = "hero";
        }

        // 用于接收 /list API 响应的顶层结构
        public class ListApiResponse
        {
            [JsonPropertyName("data")]
            public ListApiData Data { get; set; }
        }

        // 用于接收 /list API 响应中 "data" 字段的内容
        public class ListApiData
        {
            [JsonPropertyName("units")]
            public List<ApiUnit> Units { get; set; }
        }

        // 用于接收 "units" 数组中的每个英雄key
        public class ApiUnit
        {
            [JsonPropertyName("key")]
            public string Key { get; set; }
        }
    }
}