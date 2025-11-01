using JinChanChanTool.DataClass;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.RecommendedEquipment
{
    /// <summary>
    /// 重构后的核心爬取服务。
    /// 该服务现在依赖于 IDynamicGameDataService 来获取基础数据，并实现了全新的、
    /// 基于 metatft.com API 的英雄装备数据抓取和分析逻辑。
    /// </summary>
    public class CrawlingService : ICrawlingService
    {
        // 共享的HttpClient实例
        private static readonly HttpClient _httpClient = new HttpClient();

        // 从外部注入的、提供动态游戏数据的服务。
        private readonly IDynamicGameDataService _gameDataService;

        /// <summary>
        /// 构造函数，用于实现依赖注入。
        /// </summary>
        /// <param name="gameDataService">一个实现了 IDynamicGameDataService 接口的对象。</param>
        public CrawlingService(IDynamicGameDataService gameDataService)
        {
            _gameDataService = gameDataService; // 依赖注入新的服务
        }

        /// <summary>
        /// (核心方法) 异步执行完整的网络爬取流程。
        /// </summary>
        public async Task<List<HeroEquipment>> GetEquipmentsAsync(IProgress<Tuple<int, string>> progress)
        {
            // 步骤 1: 从新服务获取基础数据
            var heroKeys = _gameDataService.CurrentSeasonHeroKeys;
            var heroTranslations = _gameDataService.HeroTranslations;
            var itemTranslations = _gameDataService.ItemTranslations;

            if (heroKeys == null || heroKeys.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("CrawlingService: 英雄列表为空，请确保 DynamicGameDataService 已成功初始化。");
                progress?.Report(Tuple.Create(100, "错误：未能获取英雄列表！"));
                return new List<HeroEquipment>();
            }

            // 步骤 2: 并行请求英雄装备详情
            System.Diagnostics.Debug.WriteLine($"CrawlingService: 开始为 {heroKeys.Count} 位英雄并行请求装备详情...");
            var finalHeroEquipments = new ConcurrentBag<HeroEquipment>();

            // 限制并发数量，防止因请求过于频繁而被目标服务器拒绝服务。
            const int MAX_CONCURRENT_TASKS = 3;
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
                        System.Diagnostics.Debug.WriteLine($"处理英雄 {heroKey} 时发生未知错误: {ex.Message}");
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

            progress?.Report(Tuple.Create(100, "所有数据处理完毕！"));
            return finalHeroEquipments.ToList();
        }

        /// <summary>
        /// (辅助方法) 异步获取并处理单个英雄的数据。
        /// </summary>
        private async Task<HeroEquipment> FetchAndProcessHeroDataAsync(string heroKey, Dictionary<string, string> heroTranslations, Dictionary<string, string> itemTranslations)
        {
            string apiUrl = $"https://api-hc.metatft.com/tft-stat-api/unit_detail?queue=1100&patch=current&days=1&rank=CHALLENGER,DIAMOND,GRANDMASTER,MASTER&permit_filter_adjustment=true&unit={heroKey}";

            try
            {
                string jsonResponse = await _httpClient.GetStringAsync(apiUrl);
                var unitDetail = JsonSerializer.Deserialize<UnitDetailResponse>(jsonResponse);

                if (unitDetail?.Builds == null || unitDetail.Builds.Count == 0)
                {
                    return null; // 仍然处理API没有返回任何数据的情况
                }

                // 寻找两种最佳出装
                Build bestBuild_HighQuality = null; // 1. 满足场次要求(>=100)的最佳出装
                double maxScore_HighQuality = -1.0;

                Build bestBuild_Any = null;         // 2. 不考虑场次要求，所有出装中的最佳出装 (作为备选)
                double maxScore_Any = -1.0;

                foreach (var build in unitDetail.Builds)
                {
                    // 基本过滤条件
                    if (build.BuildNames.Split('|').Length != 3) continue;
                    if (build.Places == null || build.Places.Count != 8) continue;
                    if (build.Total == 0) continue;

                    // 计算平均名次和综合评分 
                    double weightedSum = 0;
                    for (int i = 0; i < build.Places.Count; i++)
                    {
                        weightedSum += build.Places[i] * (i + 1);
                    }
                    double avgPlacement = weightedSum / build.Total;
                    double score = (double)build.Total / avgPlacement;

                    // 无论如何，都更新“备选”的最佳出装
                    if (score > maxScore_Any)
                    {
                        maxScore_Any = score;
                        bestBuild_Any = build;
                    }

                    // 仅当场次满足要求时，才更新最佳出装
                    if (build.Total >= 100)
                    {
                        if (score > maxScore_HighQuality)
                        {
                            maxScore_HighQuality = score;
                            bestBuild_HighQuality = build;
                        }
                    }
                }

                // 优先返回高质量的出装，如果不存在，则返回备选的最佳出装。
                // '??' 是C#的空合并运算符，如果左边为null，则返回右边的值。
                var finalBestBuild = bestBuild_HighQuality ?? bestBuild_Any;

                // ========== 核心逻辑修改结束 ==========


                if (finalBestBuild == null)
                {
                    // 如果两种都找不到（例如所有出装都不是三件套），则仍然放弃该英雄
                    return null;
                }

                var equipmentKeys = finalBestBuild.BuildNames.Split('|');
                var equipmentNames = equipmentKeys
                    .Select(key => itemTranslations.GetValueOrDefault(key, $"【翻译失败:{key}】"))
                    .ToList();

                return new HeroEquipment
                {
                    HeroName = heroTranslations.GetValueOrDefault(heroKey, heroKey),
                    Equipments = equipmentNames
                };
            }
            catch (HttpRequestException)
            {
                // 网络错误时，仍然静默失败，不影响其他英雄
                return null;
            }
        }
    }
}