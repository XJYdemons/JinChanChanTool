using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Services.LineupCrawling;
using JinChanChanTool.Services.RecommendedEquipment;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using System.Diagnostics;

namespace JinChanChanTool.Services.DataServices
{
    /// <summary>
    /// 自动更新服务实现类
    /// 负责在应用启动时检查并后台更新推荐装备和阵容数据
    /// </summary>
    public class AutoUpdateService : IAutoUpdateService
    {
        private readonly IManualSettingsService _manualSettingsService;
        private readonly IAutomaticSettingsService _automaticSettingsService;
        private readonly IHeroEquipmentDataService _heroEquipmentDataService;
        private readonly IRecommendedLineUpService _recommendedLineUpService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AutoUpdateService(
            IManualSettingsService manualSettingsService,
            IAutomaticSettingsService automaticSettingsService,
            IHeroEquipmentDataService heroEquipmentDataService,
            IRecommendedLineUpService recommendedLineUpService)
        {
            _manualSettingsService = manualSettingsService;
            _automaticSettingsService = automaticSettingsService;
            _heroEquipmentDataService = heroEquipmentDataService;
            _recommendedLineUpService = recommendedLineUpService;
        }

        /// <summary>
        /// 检查并在后台更新数据（如果需要）
        /// </summary>
        public async Task CheckAndUpdateAsync()
        {
            string selectedSeason = _automaticSettingsService.CurrentConfig.SelectedSeason;
            string mainSeason = _automaticSettingsService.CurrentConfig.MainSeason;
            bool isMainSeason = string.Equals(
                selectedSeason,
                mainSeason,
                StringComparison.OrdinalIgnoreCase);
            if (!isMainSeason)
            {
                OutputForm.Instance.WriteLineOutputMessage(
                    $"当前选择的是非主赛季“{selectedSeason}”，仅使用本地推荐数据，不执行网络更新。");
                return;
            }

            // 检查是否需要更新装备数据
            bool needsEquipmentUpdate = _manualSettingsService.CurrentConfig.IsAutomaticUpdateEquipment &&
                                        _heroEquipmentDataService.NeedsUpdate(_manualSettingsService.CurrentConfig.UpdateEquipmentInterval);

            // 检查是否需要更新阵容数据
            bool needsLineupUpdate = _manualSettingsService.CurrentConfig.IsAutomaticUpdateLineup &&
                                     _recommendedLineUpService.NeedsUpdate(_manualSettingsService.CurrentConfig.UpdateLineupInterval);
            // 如果都不需要更新，直接返回
            if (!needsEquipmentUpdate && !needsLineupUpdate)
            {
                OutputForm.Instance.WriteLineOutputMessage("数据检查完成，均为最新版本。");
                return;
            }

            // 在后台异步更新
            _ = Task.Run(async () =>
            {
                try
                {
                    if (needsEquipmentUpdate)
                    {
                        await UpdateEquipmentDataAsync(mainSeason);
                    }

                    if (needsLineupUpdate)
                    {
                        await UpdateLineupDataAsync(mainSeason);
                    }
                }
                catch (Exception ex)
                {
                    // 静默失败，仅记录日志
                    OutputForm.Instance.WriteLineOutputMessage($"后台更新失败: {ex.Message}");
                }
            });

            // 立即返回，不阻塞启动流程
            OutputForm.Instance.WriteLineOutputMessage("正在后台更新推荐数据...");
        }

        /// <summary>
        /// 更新装备数据
        /// </summary>
        private async Task UpdateEquipmentDataAsync(string targetSeason)
        {
            try
            {
                OutputForm.Instance.WriteLineOutputMessage("开始更新推荐装备数据...");

                // 创建动态数据服务和爬虫服务
                DynamicGameDataService dynamicGameDataService = new DynamicGameDataService();
                await dynamicGameDataService.InitializeAsync();

                CrawlingService crawlingService = new CrawlingService(dynamicGameDataService);

                // 创建进度报告器
                Progress<Tuple<int, string>> progress = new Progress<Tuple<int, string>>(tuple =>
                {
                    OutputForm.Instance.WriteLineOutputMessage($"[装备更新] {tuple.Item2} ({tuple.Item1}%)");
                });

                // 执行爬取
                List<DataClass.RecommendedEquipment> crawledData = await crawlingService.GetEquipmentsAsync(progress);

                if (crawledData != null && crawledData.Count > 0)
                {
                    _heroEquipmentDataService.UpdateDataFromCrawling(crawledData, targetSeason);

                    // 重新加载数据使其立即生效
                    if (string.Equals(
                            _automaticSettingsService.CurrentConfig.SelectedSeason,
                            targetSeason,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        _heroEquipmentDataService.ReLoad();
                    }

                    OutputForm.Instance.WriteLineOutputMessage($"推荐装备数据更新成功，共 {crawledData.Count} 位英雄。");
                }
                else
                {
                    OutputForm.Instance.WriteLineOutputMessage("装备数据更新失败。");
                }
            }
            catch (Exception ex)
            {
                OutputForm.Instance.WriteLineOutputMessage($"装备数据更新失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新阵容数据
        /// </summary>
        private async Task UpdateLineupDataAsync(string targetSeason)
        {
            try
            {
                OutputForm.Instance.WriteLineOutputMessage("开始更新推荐阵容数据...");

                // 创建动态数据服务和爬虫服务
                DynamicGameDataService dynamicGameDataService = new DynamicGameDataService();
                await dynamicGameDataService.InitializeAsync();

                LineupCrawlingService lineupCrawlingService = new LineupCrawlingService(dynamicGameDataService);

                // 创建进度报告器
                Progress<Tuple<int, string>> progress = new Progress<Tuple<int, string>>(tuple =>
                {
                    OutputForm.Instance.WriteLineOutputMessage($"[阵容更新] {tuple.Item2} ({tuple.Item1}%)");
                });

                // 执行爬取
                List<RecommendedLineUp> crawledLineups = await lineupCrawlingService.GetRecommendedLineUpsAsync(progress);

                if (crawledLineups != null && crawledLineups.Count > 0)
                {
                    int addedCount = _recommendedLineUpService.UpdateDataFromCrawling(crawledLineups, targetSeason);

                    // 重新加载数据使其立即生效
                    if (string.Equals(
                            _automaticSettingsService.CurrentConfig.SelectedSeason,
                            targetSeason,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        _recommendedLineUpService.ReLoad();
                    }

                    OutputForm.Instance.WriteLineOutputMessage($"推荐阵容数据更新成功，共 {addedCount} 个阵容。");
                }
                else
                {
                    OutputForm.Instance.WriteLineOutputMessage("阵容数据更新失败。");
                }
            }
            catch (Exception ex)
            {
                OutputForm.Instance.WriteLineOutputMessage($"阵容数据更新失败: {ex.Message}");
            }
        }
    }
}
