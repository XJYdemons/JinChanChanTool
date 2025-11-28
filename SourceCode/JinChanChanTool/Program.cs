using JinChanChanTool.Forms;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
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
        
            //创建并加载应用设置服务
            IManualSettingsService _iappConfigService = new ManualSettingsService();
            _iappConfigService.Load();

            #region 错误信息输出窗口
            OutputForm.Instance.Show();
            if (!_iappConfigService.CurrentConfig.IsUseOutputForm)
            {
                OutputForm.Instance.Visible = false;
            }
            #endregion

            //创建并加载应用设置服务
            IAutomaticSettingsService _iAutoConfigService = new AutomaticSettingsService();
            _iAutoConfigService.Load();
            
            //创建并加载英雄数据服务
            IHeroDataService _iheroDataService = new HeroDataService();
            _iheroDataService.SetFilePathsIndex(_iAutoConfigService.CurrentConfig.SelectedSeason);        
            _iheroDataService.Load();

            //创建OCR结果纠正服务
            ICorrectionService _iCorrectionService = new CorrectionService();
            _iCorrectionService.Load();
            _iCorrectionService.SetCharDictionary(_iheroDataService.GetCharDictionary());
            
            //创建并加载阵容数据服务
            ILineUpService _ilineUpService = new LineUpService(_iheroDataService, _iappConfigService.CurrentConfig.MaxLineUpCount,_iappConfigService.CurrentConfig.MaxHerosCount,_iAutoConfigService.CurrentConfig.SelectedLineUpIndex);
            _ilineUpService.SetFilePathsIndex(_iAutoConfigService.CurrentConfig.SelectedSeason);
            _ilineUpService.Load();

            // 创建并加载英雄装备推荐数据服务
            IHeroEquipmentDataService _iheroEquipmentDataService = new HeroEquipmentDataService();
            _iheroEquipmentDataService.Load();

            // 运行主窗体并传入应用设置服务
            Application.Run(new MainForm(_iappConfigService,_iAutoConfigService, _iheroDataService, _ilineUpService, _iCorrectionService, _iheroEquipmentDataService));

        }
    }
}