using JinChanChanTool.DataClass.StaticData;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Services.LineupCrawling;
using JinChanChanTool.Services.Localization;
using JinChanChanTool.Services.RecommendedEquipment;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using JinChanChanTool.Services.Update;
using JinChanChanTool.Tools.LineUpCodeTools;
using JinChanChanTool.Tools.KeyboardMouseTools;
using System.Diagnostics;
namespace JinChanChanTool
{
    internal static class Program
    {       
        [STAThread]
        static void Main(string[] args)
        {
            // 更新应用流程必须最先处理：此时主程序正在退出，进程内只允许执行文件替换。
            if (TryRunUpdateApplier(args, out int exitCode))
            {
                Environment.Exit(exitCode);
                return;
            }

            // 设置高DPI模式
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ApplicationConfiguration.Initialize();

            // 上次更新留下的默认值快照 + 当前版本默认值 → 只给用户配置补齐新增项（不覆盖已有设置）
            RunConfigurationMigration();

            //创建并加载用户应用设置服务
            IManualSettingsService _iManualSettingsService = new ManualSettingsService();
            _iManualSettingsService.Load();

            // 最大选择英雄数量（从配置读取）
            int maxCountOfHero = _iManualSettingsService.CurrentConfig.LineUpCapacity;

            //创建并加载自动应用设置服务
            IAutomaticSettingsService _iAutomaticSettingsService = new AutomaticSettingsService();
            _iAutomaticSettingsService.Load();

            //创建并加载本地化服务
            ILocalizationService _iLocalizationService = new LocalizationService();
            _iLocalizationService.Load(_iManualSettingsService.CurrentConfig.Language);

            // 检查是否是首次启动
            if (_iAutomaticSettingsService.CurrentConfig.IsFirstStart)
            {
                // 显示配置向导
                using (SetupWizardForm testForm = new SetupWizardForm(_iManualSettingsService, _iLocalizationService))
                {
                    DialogResult result = testForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        // 用户完成了配置向导，标记为非首次启动
                        _iAutomaticSettingsService.CurrentConfig.IsFirstStart = false;
                        _iAutomaticSettingsService.Save();

                        // 重启应用程序以使配置生效
                        Application.Restart();
                        Environment.Exit(0);
                        return; // 确保不继续执行后续代码
                    }
                    else
                    {
                        // 用户跳过了向导，仍然标记为非首次启动（使用默认配置）
                        _iAutomaticSettingsService.CurrentConfig.IsFirstStart = false;
                        _iAutomaticSettingsService.Save();
                    }
                }
            }

            // 展示输出窗口
            OutputForm.Instance.InitializeLocalization(_iLocalizationService);
            OutputForm.Instance.InitializeObject(_iAutomaticSettingsService);
            OutputForm.Instance.TopMost = _iManualSettingsService.CurrentConfig.IsAllWindowsTopMost;
            OutputForm.Instance.Show();
            if (!_iManualSettingsService.CurrentConfig.IsUseOutputForm)
            {
                OutputForm.Instance.Visible = false;
            }

           

            //创建并加载英雄数据服务
            IHeroDataService _iheroDataService = new HeroDataService();
            string selectedSeason;
            try
            {
                selectedSeason = ResolveSelectedSeason(
                    _iheroDataService.GetFilePaths(),
                    _iAutomaticSettingsService.CurrentConfig.SelectedSeason,
                    _iAutomaticSettingsService.CurrentConfig.MainSeason);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "赛季配置错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!string.Equals(selectedSeason,
                               _iAutomaticSettingsService.CurrentConfig.SelectedSeason,
                               StringComparison.OrdinalIgnoreCase))
            {
                OutputForm.Instance.WriteLineOutputMessage(
                    $"配置的赛季不存在，已切换到可用赛季：{selectedSeason}");
                _iAutomaticSettingsService.CurrentConfig.SelectedSeason = selectedSeason;
                _iAutomaticSettingsService.Save();
            }

            // 优先加载所选赛季随版本分发的阵容码字典，缺失时由后台更新服务补齐。
            ILineUpCodeDictionaryService _iLineUpCodeDictionaryService = new LineUpCodeDictionaryService();
            _iLineUpCodeDictionaryService.LoadSeasonDictionary(selectedSeason);
            ILineUpParser _iLineUpParser = new LineUpParser(_iLineUpCodeDictionaryService);

            _iheroDataService.SetFilePathsIndex(selectedSeason);
            _iheroDataService.Load();

            //创建并加载装备数据服务
            IEquipmentService _iEquipmentService = new EquipmentService();
            _iEquipmentService.SetFilePathsIndex(selectedSeason);
            _iEquipmentService.Load();

            //创建OCR结果纠正服务
            ICorrectionService _iCorrectionService = new CorrectionService(_iManualSettingsService);
            _iCorrectionService.Load();
            _iCorrectionService.SetCharDictionary(_iheroDataService.GetCharDictionary());

            //创建并加载阵容数据服务
            ILineUpService _iLineUpService = new LineUpService(_iheroDataService, _iManualSettingsService, _iLocalizationService, maxCountOfHero,_iAutomaticSettingsService.CurrentConfig.SelectedLineUpIndex);
            _iLineUpService.SetFilePathsIndex(selectedSeason);
            _iLineUpService.Load();        

            // 创建并加载英雄装备推荐数据服务
            IHeroEquipmentDataService _iHeroEquipmentDataService = new HeroEquipmentDataService();
            _iHeroEquipmentDataService.SetFilePathsIndex(selectedSeason);
            _iHeroEquipmentDataService.Load();

            // 创建并配置推荐阵容数据服务
            IRecommendedLineUpService _iRecommendedLineUpService = new RecommendedLineUpService();
            _iRecommendedLineUpService.SetFilePathsIndex(selectedSeason);
            _iRecommendedLineUpService.Load();

            // 创建自动更新服务并启动后台检查
            IAutoUpdateService _iAutoUpdateService = new AutoUpdateService(_iManualSettingsService, _iAutomaticSettingsService, _iHeroEquipmentDataService, _iRecommendedLineUpService, _iLineUpCodeDictionaryService);

            // 创建程序本体更新服务（检查 GitHub 发布、下载安装包、退出后由独立进程完成替换）
            ProgramUpdateService _programUpdateService = new ProgramUpdateService();

            // 清理上一次更新残留的解压目录与过期备份，避免长期占用磁盘
            _programUpdateService.CleanupExpiredArtifacts(UpdateConstants.KeepBackupCount);

            // 根据设置创建统一的键鼠操作设备。旧配置中的未知枚举值会回退到 WinAPI；
            // 已知但尚未实现的设备类型由工厂明确报告，避免误把输入发到本机。
            IKeyboardMouseDevice _iKeyboardMouseDevice;
            try
            {
                _iKeyboardMouseDevice = KeyboardMouseDeviceFactory.CreateOrFallback(
                    _iManualSettingsService.CurrentConfig.KeyboardMouseDevice,
                    _iManualSettingsService.CurrentConfig.MakcuPortName,
                    _iManualSettingsService.CurrentConfig.MakcuBaudRate,
                    _iManualSettingsService.CurrentConfig.KmBoxIp,
                    _iManualSettingsService.CurrentConfig.KmBoxPort,
                    _iManualSettingsService.CurrentConfig.KmBoxMac);
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show(
                    $"键鼠设备初始化失败：{ex.Message}",
                    "键鼠设备错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // 与参考实现一致，在应用退出时兜底释放共享设备实例。
            Application.ApplicationExit += (_, _) => _iKeyboardMouseDevice.Dispose();

            _ = _iAutoUpdateService.CheckAndUpdateAsync();

            // 运行主窗体并传入应用设置服务
            Application.Run(new MainForm(_iManualSettingsService,_iAutomaticSettingsService, _iLocalizationService, _iheroDataService, _iEquipmentService,  _iCorrectionService, _iLineUpService, _iHeroEquipmentDataService, _iRecommendedLineUpService, _iLineUpParser, _iAutoUpdateService, _programUpdateService, _iKeyboardMouseDevice));
        }

        /// <summary>
        /// 判断当前进程是否由更新流程启动；是则执行文件替换并返回退出码。
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <param name="exitCode">更新结果对应的退出码</param>
        /// <returns>是否为更新应用模式</returns>
        private static bool TryRunUpdateApplier(string[] args, out int exitCode)
        {
            exitCode = UpdateExitCode.NotApplyMode;

            if (args == null || args.Length == 0)
            {
                return false;
            }

            int argumentIndex = Array.FindIndex(
                args,
                argument => string.Equals(argument, UpdateConstants.ApplyUpdateArgument, StringComparison.OrdinalIgnoreCase));

            if (argumentIndex < 0)
            {
                return false;
            }

            // 计划文件路径紧随开关参数；缺失时退回到默认位置
            string planPath = args.Length > argumentIndex + 1
                ? args[argumentIndex + 1]
                : new ProgramUpdateService().GetPlanFilePath();

            bool success = new UpdateApplier().Apply(planPath);
            exitCode = success ? UpdateExitCode.Success : UpdateExitCode.Failed;
            return true;
        }

        /// <summary>
        /// 执行配置增量合并：只把新版本新增的设置项/数据条目补进用户配置，已有内容保持不变。
        /// </summary>
        private static void RunConfigurationMigration()
        {
            try
            {
                ProgramUpdateService programUpdateService = new ProgramUpdateService();
                string? snapshotDirectory = programUpdateService.GetLatestDefaultsSnapshotDirectory();

                ConfigurationMigrationService migrationService = new ConfigurationMigrationService(
                    AppDomain.CurrentDomain.BaseDirectory);

                ConfigurationMigrationResult result = migrationService.Apply(
                    snapshotDirectory,
                    ProgramVersion.VersionText);

                if (result.HasChanges)
                {
                    OutputForm.Instance.WriteLineOutputMessage(
                        $"配置已增量更新，新增 {result.AddedMembers.Count} 项：{string.Join("、", result.AddedMembers)}");
                }
            }
            catch (Exception ex)
            {
                // 配置合并失败不应阻断程序启动
                LogTool.Log($"[Program] 配置增量合并失败：{ex.Message}");
                Debug.WriteLine($"[Program] 配置增量合并失败：{ex.Message}");
            }
        }

        private static string ResolveSelectedSeason(
            string[] seasonPaths,
            string configuredSeason,
            string mainSeason)
        {
            string[] availableSeasons = seasonPaths
                .Select(Path.GetFileName)
                .Where(season => !string.IsNullOrWhiteSpace(season))
                .Select(season => season!)
                .ToArray();

            if (availableSeasons.Length == 0)
            {
                throw new InvalidOperationException("没有找到任何可用的赛季数据目录。");
            }

            return availableSeasons.FirstOrDefault(season =>
                       string.Equals(season, configuredSeason, StringComparison.OrdinalIgnoreCase))
                   ?? availableSeasons.FirstOrDefault(season =>
                       string.Equals(season, mainSeason, StringComparison.OrdinalIgnoreCase))
                   ?? availableSeasons[0];
        }
    }
}
