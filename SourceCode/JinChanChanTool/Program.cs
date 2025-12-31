using JinChanChanTool.Forms;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Services.LineupCrawling;
using JinChanChanTool.Services.LineupCrawling.Interface;
using JinChanChanTool.Services.RecommendedEquipment;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using System.Diagnostics;
namespace JinChanChanTool
{
    internal static class Program
    {       
        [STAThread]
        static void Main()
        {
            // 设置高DPI模式
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ApplicationConfiguration.Initialize();

            // 最大选择英雄数量
            const int MaxCountOfHero = 10;

            //创建并加载用户应用设置服务
            IManualSettingsService _iManualSettingsService = new ManualSettingsService();
            _iManualSettingsService.Load();

            //创建并加载自动应用设置服务
            IAutomaticSettingsService _iAutomaticSettingsService = new AutomaticSettingsService();
            _iAutomaticSettingsService.Load();

            // 展示输出窗口          
            OutputForm.Instance.InitializeObject(_iAutomaticSettingsService);
            OutputForm.Instance.Show();
            if (!_iManualSettingsService.CurrentConfig.IsUseOutputForm)
            {
                OutputForm.Instance.Visible = false;
            }

           

            //创建并加载英雄数据服务
            IHeroDataService _iheroDataService = new HeroDataService();
            _iheroDataService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason);        
            _iheroDataService.Load();

            //创建并加载装备数据服务
            IEquipmentService _iEquipmentService = new EquipmentService();
            _iEquipmentService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason);
            _iEquipmentService.Load();

            //创建OCR结果纠正服务
            ICorrectionService _iCorrectionService = new CorrectionService();
            _iCorrectionService.Load();
            _iCorrectionService.SetCharDictionary(_iheroDataService.GetCharDictionary());

            //创建并加载阵容数据服务
            ILineUpService _iLineUpService = new LineUpService(_iheroDataService, MaxCountOfHero,_iAutomaticSettingsService.CurrentConfig.SelectedLineUpIndex);
            _iLineUpService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason);
            _iLineUpService.Load();        

            // 创建并加载英雄装备推荐数据服务
            IHeroEquipmentDataService _iHeroEquipmentDataService = new HeroEquipmentDataService();
            _iHeroEquipmentDataService.Load();

            // 创建并配置推荐阵容数据服务
            IRecommendedLineUpService _iRecommendedLineUpService = new RecommendedLineUpService();
            _iRecommendedLineUpService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason);
            _iRecommendedLineUpService.Load();
            
            // 运行主窗体并传入应用设置服务
            Application.Run(new MainForm(_iManualSettingsService,_iAutomaticSettingsService, _iheroDataService, _iEquipmentService,  _iCorrectionService, _iLineUpService, _iHeroEquipmentDataService, _iRecommendedLineUpService));
        }
    }
}