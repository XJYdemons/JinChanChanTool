using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using JinChanChanTool.Services.Network; // 引入全局网络管理
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace JinChanChanTool.Services.RecommendedEquipment
{
    /// <summary>
    /// 接入全局 HttpProvider 以共享连接池，并优化了高并发下的异常处理。
    /// </summary>
    public class CrawlingService : ICrawlingService
    {
        // 删除了本地 static readonly HttpClient 实例，改用 HttpProvider.Client

        private readonly IDynamicGameDataService _gameDataService;

        /// <summary>
        /// 构造函数，通过依赖注入获取动态数据服务。
        /// </summary>
        public CrawlingService(IDynamicGameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }

        /// <summary>
        /// 异步执行完整的网络爬取流程。
        /// </summary>
        public async Task<List<HeroEquipment>> GetEquipmentsAsync(IProgress<Tuple<int, string>> progress)
        {
            // 1. 获取基础翻译数据
            var heroKeys = _gameDataService.CurrentSeasonHeroKeys;
            var heroTranslations = _gameDataService.HeroTranslations;
            var itemTranslations = _gameDataService.ItemTranslations;

            if (heroKeys == null || heroKeys.Count == 0)
            {
                LogAndReportError("英雄列表为空，请确保数据服务已初始化。", progress);
                return new List<HeroEquipment>();
            }

            Debug.WriteLine($"CrawlingService: 开始为 {heroKeys.Count} 位英雄并行请求数据...");
            LogTool.Log($"CrawlingService: 开始为 {heroKeys.Count} 位英雄并行请求数据...");
            OutputForm.Instance.WriteLineOutputMessage($"CrawlingService: 开始并行请求 {heroKeys.Count} 位英雄的出装详情...");

            var finalHeroEquipments = new ConcurrentBag<HeroEquipment>();
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
        private async Task<HeroEquipment> FetchAndProcessHeroDataAsync(string heroKey, Dictionary<string, string> heroTranslations, Dictionary<string, string> itemTranslations)
        {
            string apiUrl = $"https://api-hc.metatft.com/tft-stat-api/unit_detail?queue=1100&patch=current&days=1&rank=CHALLENGER,DIAMOND,GRANDMASTER,MASTER&permit_filter_adjustment=true&unit={heroKey}";

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

                        // 后续解析逻辑 (ExtractBestBuild 是类内部的辅助方法)
                        Build bestBuild = ExtractBestBuild(unitDetail.Builds);
                        if (bestBuild == null) return null;

                        var equipmentKeys = bestBuild.BuildNames.Split('|');
                        var equipmentNames = equipmentKeys
                            .Select(key => itemTranslations.GetValueOrDefault(key, $"【翻译失败:{key}】"))
                            .ToList();

                        return new HeroEquipment
                        {
                            HeroName = heroTranslations.GetValueOrDefault(heroKey, heroKey),
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

        /// <summary>
        /// 内部优化：从所有出装中提取最优解
        /// </summary>
        private Build ExtractBestBuild(List<Build> builds)
        {
            Build bestBuild_HighQuality = null;
            double maxScore_HighQuality = -1.0;

            Build bestBuild_Any = null;
            double maxScore_Any = -1.0;

            foreach (var build in builds)
            {
                if (string.IsNullOrEmpty(build.BuildNames) || build.BuildNames.Split('|').Length != 3) continue;
                if (build.Places == null || build.Places.Count != 8 || build.Total == 0) continue;

                // 计算加权平均排名评分
                double weightedSum = 0;
                for (int i = 0; i < build.Places.Count; i++)
                {
                    weightedSum += build.Places[i] * (i + 1);
                }
                double avgPlacement = weightedSum / build.Total;
                double score = (double)build.Total / avgPlacement;

                if (score > maxScore_Any)
                {
                    maxScore_Any = score;
                    bestBuild_Any = build;
                }

                // 高质量标准：场次 >= 100
                if (build.Total >= 100 && score > maxScore_HighQuality)
                {
                    maxScore_HighQuality = score;
                    bestBuild_HighQuality = build;
                }
            }

            return bestBuild_HighQuality ?? bestBuild_Any;
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