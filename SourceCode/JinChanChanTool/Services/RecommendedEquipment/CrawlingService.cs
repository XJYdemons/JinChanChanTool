using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.Network;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace JinChanChanTool.Services.RecommendedEquipment
{
    /// <summary>
    /// 接入全局 HttpProvider 以共享连接池，并优化了高并发下的异常处理。
    /// </summary>
    public class CrawlingService
    {
        // 删除了本地 static readonly HttpClient 实例，改用 HttpProvider.Client

        private readonly DynamicGameDataService _gameDataService;

        /// <summary>
        /// 构造函数，通过依赖注入获取动态数据服务。
        /// </summary>
        public CrawlingService(DynamicGameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }

        /// <summary>
        /// 异步执行完整的网络爬取流程。
        /// </summary>
        public async Task<List<DataClass.RecommendedEquipment>> GetEquipmentsAsync(IProgress<Tuple<int, string>> progress)
        {
            // 1. 获取基础翻译数据
            var heroKeys = _gameDataService.CurrentSeasonHeroKeys;
            var heroTranslations = _gameDataService.HeroTranslations;
            var itemTranslations = _gameDataService.ItemTranslations;

            if (heroKeys == null || heroKeys.Count == 0)
            {
                LogAndReportError("英雄列表为空，请确保数据服务已初始化。", progress);
                return new List<DataClass.RecommendedEquipment>();
            }

            Debug.WriteLine($"CrawlingService: 开始为 {heroKeys.Count} 位英雄并行请求数据...");
            LogTool.Log($"CrawlingService: 开始为 {heroKeys.Count} 位英雄并行请求数据...");
            OutputForm.Instance.WriteLineOutputMessage($"CrawlingService: 开始并行请求 {heroKeys.Count} 位英雄的出装详情...");

            var finalHeroEquipments = new ConcurrentBag<DataClass.RecommendedEquipment>();
            const int MAX_CONCURRENT_TASKS = 10; // 保持限制并发数量
            var semaphore = new SemaphoreSlim(MAX_CONCURRENT_TASKS);
            var tasks = new List<Task>();
            var processedCount = 0;
            int totalHeroes = heroKeys.Count;

            foreach (var heroKey in heroKeys)
            {
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var heroEquipment = await FetchAndProcessHeroDataAsync(heroKey, heroTranslations, itemTranslations);
                        if (heroEquipment != null)
                        {
                            finalHeroEquipments.Add(heroEquipment);
                        }
                    }
                    catch (Exception ex)
                    {
                        string heroName = heroTranslations.GetValueOrDefault(heroKey, heroKey);
                        Debug.WriteLine($"处理英雄 {heroName} 时发生未知错误: {ex.Message}");
                    }
                    finally
                    {
                        int currentCount = Interlocked.Increment(ref processedCount);
                        int percentage = (int)((double)currentCount / totalHeroes * 100);
                        string heroName = heroTranslations.GetValueOrDefault(heroKey, heroKey);
                        progress?.Report(Tuple.Create(percentage, $"({currentCount}/{totalHeroes}) 已处理: {heroName}"));

                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            progress?.Report(Tuple.Create(100, "所有英雄装备数据处理完毕！"));
            return finalHeroEquipments.ToList();
        }

        /// <summary>
        /// (辅助方法) 异步获取并处理单个英雄的数据。
        /// </summary>
        private async Task<DataClass.RecommendedEquipment> FetchAndProcessHeroDataAsync(string heroKey, Dictionary<string, string> heroTranslations, Dictionary<string, string> itemTranslations)
        {
            string apiUrl = $"https://api-hc.metatft.com/tft-stat-api/unit_detail?queue=1100&patch=current&days=3&rank=CHALLENGER,DIAMOND,GRANDMASTER,MASTER&permit_filter_adjustment=true&unit={heroKey}";

            const int MaxRetries = 2; // 总计最多尝试 3 次
            int retryCount = 0;

            while (true)
            {
                try
                {
                    // 使用全局 HttpProvider.Client 发起请求
                    using (var response = await HttpProvider.Client.GetAsync(apiUrl, HttpCompletionOption.ResponseContentRead))
                    {
                        // 检查状态码
                        if (!response.IsSuccessStatusCode) return null;

                        // 先读取为字节数组再转字符串，确保数据流被完整排空，减少 IOException 概率
                        byte[] contentBytes = await response.Content.ReadAsByteArrayAsync();
                        string jsonResponse = System.Text.Encoding.UTF8.GetString(contentBytes);

                        var unitDetail = JsonSerializer.Deserialize<UnitDetailResponse>(jsonResponse);
                        if (unitDetail?.Builds == null || unitDetail.Builds.Count == 0) return null;

                        // 计算英雄数据的逻辑（虽然 ExtractBestBuild 里已经不再强依赖这两个参数，但保留着也没问题，以防万一后续用得到）
                        long heroTotalGames = 0;
                        double unitWeightedSum = 0;

                        // 注意：当 days=3 时，Dates 数组会有多天数据。
                        // 但这里主要用 Builds 列表（API已经聚合好了3天的 Build 数据），
                        // 所以 Dates 这里的平均值计算即使只取 Dates[0]（最近一天）作为参考也无伤大雅，
                        // 因为核心算法 ExtractBestBuild 已经进化为自适应比较了。
                        if (unitDetail.Dates != null && unitDetail.Dates.Any())
                        {
                            var p = unitDetail.Dates[0].Places;
                            for (int i = 0; i < p.Count; i++)
                            {
                                heroTotalGames += p[i];
                                unitWeightedSum += p[i] * (i + 1);
                            }
                        }

                        if (heroTotalGames <= 0) heroTotalGames = 100000;
                        double unitGlobalAvgRank = unitWeightedSum > 0 ? unitWeightedSum / (double)heroTotalGames : 4.0;

                        // 调用公差带算法
                        Build bestBuild = ExtractBestBuild(unitDetail.Builds, heroTotalGames, unitGlobalAvgRank);
                        if (bestBuild == null) return null;

                        var equipmentKeys = bestBuild.BuildNames.Split('|');
                        var equipmentNames = equipmentKeys
                            .Select(key => itemTranslations.GetValueOrDefault(key, $"【翻译失败:{key}】"))
                            .ToList();

                        string rawName = heroTranslations.GetValueOrDefault(heroKey, heroKey);
                        string cleanName = rawName.Replace("·", "").Trim();

                        return new DataClass.RecommendedEquipment
                        {
                            HeroName = cleanName,
                            Equipments = equipmentNames
                        };
                    }
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is IOException || ex is TaskCanceledException)
                {
                    // 如果达到最大重试次数，则记录并退出
                    if (retryCount >= MaxRetries)
                    {
                        Debug.WriteLine($"[网络错误] 英雄 {heroKey} 在重试 {MaxRetries} 次后仍然失败: {ex.Message}");
                        return null;
                    }

                    retryCount++;
                    int delay = retryCount * 2000;
                    Debug.WriteLine($"[重试提示] 英雄 {heroKey} 请求失败 (SSL/IO抖动)，正在进行第 {retryCount} 次重试...");
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    // 严重的逻辑异常（如解析失败）不进行重试
                    Debug.WriteLine($"[逻辑异常] 英雄 {heroKey}: {ex.Message}");
                    return null;
                }
            }
        }

        private Build ExtractBestBuild(List<Build> builds, long heroTotalGames, double unitAvg)
        {
            // 1. 数据清洗 (Artifacts/Radiant Filter)
            var rawList = builds
                .Where(b => !string.IsNullOrEmpty(b.BuildNames))
                .Where(b =>
                {
                    var items = b.BuildNames.Split('|');
                    if (items.Length != 3) return false;
                    foreach (var item in items)
                    {
                        if (item.Contains("Artifact", StringComparison.OrdinalIgnoreCase) ||
                            item.Contains("Radiant", StringComparison.OrdinalIgnoreCase))
                            return false;
                    }
                    return true;
                })
                .Where(b => b.Places != null && b.Places.Count == 8)
                .Select(b =>
                {
                    double weightedSum = 0;
                    for (int i = 0; i < b.Places.Count; i++) weightedSum += b.Places[i] * (i + 1);
                    double avgRank = b.Total > 0 ? weightedSum / b.Total : 8.0;
                    return new { Origin = b, AvgRank = avgRank, Total = b.Total };
                })
                .ToList();

            if (!rawList.Any()) return null;

            long maxSampleInList = rawList.Max(x => x.Total);
            bool isWeakHero = unitAvg > 3.5;

            // 分支一：小样本模式 (Max < 2000)
            // 适用：路边工具人、非主c，主坦但需要携带装备等英雄
            if (maxSampleInList < 2000)
            {
                var candidates = rawList.Where(x => x.Total >= 100).ToList();
                if (!candidates.Any()) candidates = rawList;

                double bestRank = candidates.Min(x => x.AvgRank);
                double tolerance = (bestRank < 3.5) ? 0.35 : 0.15;
                double threshold = bestRank + tolerance;

                var finalCandidates = candidates.Where(x => x.AvgRank <= threshold).ToList();

                if (finalCandidates.Any())
                {
                    return finalCandidates.OrderByDescending(x => x.Total).First().Origin;
                }
                return candidates.OrderByDescending(x => x.Total).First().Origin;
            }

            // 分支二：大样本模式 (Max >= 2000)
            // 适用：主c、主坦等核心英雄
            else
            {
                // 1. 基础筛选
                var candidates = rawList.Where(x =>
                {
                    if (x.Total < 200) return false;
                    if (x.Total > 2000) return true;

                    // Rank <= 3.75 视为不错，门槛放宽到 10%
                    double requiredRatio = (x.AvgRank <= 3.75) ? 0.10 : 0.34;
                    return ((double)x.Total / maxSampleInList) >= requiredRatio;
                }).ToList();

                if (!candidates.Any())
                {
                    candidates = rawList.Where(x => x.Total >= 200).ToList();
                    if (!candidates.Any()) candidates = rawList;
                }

                // 2. 弱势英雄的大样本安全锁
                bool hasMegaSamples = candidates.Any(x => x.Total > 2000);
                if (isWeakHero && hasMegaSamples)
                {
                    var safeBuilds = candidates.Where(x =>
                        x.Total > 2000 ||
                        (x.Total > 1000 && x.AvgRank <= 3.75)
                    ).ToList();

                    if (safeBuilds.Any()) candidates = safeBuilds;
                }

                // 3. 评分决策
                var bestBuild = candidates
                    .Select(x =>
                    {
                        double cap = isWeakHero ? 200000 : 5000;
                        double weight = isWeakHero ? 0.15 : 0.40;

                        double cappedTotal = Math.Min(x.Total, cap);
                        double score = x.AvgRank - (Math.Log10(cappedTotal) * weight);

                        return new { Origin = x.Origin, Score = score };
                    })
                    .OrderBy(x => x.Score)
                    .First();

                return bestBuild.Origin;
            }
        }

        private void LogAndReportError(string message, IProgress<Tuple<int, string>> progress)
        {
            Debug.WriteLine($"CrawlingService: {message}");
            LogTool.Log($"CrawlingService: {message}");
            OutputForm.Instance.WriteLineOutputMessage($"错误：{message}");
            progress?.Report(Tuple.Create(100, $"错误：{message}"));
        }
    }
}