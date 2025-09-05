using JinChanChanTool.Services.DataServices;
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
            IAppConfigService _iappConfigService = new AppConfigService();
            _iappConfigService.Load();

            //创建OCR结果纠正服务
            ICorrectionService _iCorrectionService = new CorrectionService();
            _iCorrectionService.Load();

            // 创建并加载英雄数据服务
            IHeroDataService _iheroDataService = new HeroDataService();
            _iheroDataService.Load();

            //创建并加载阵容数据服务
            ILineUpService _ilineUpService = new LineUpService(_iheroDataService.HeroDatas.Count, _iappConfigService.CurrentConfig.CountOfLine);
            _ilineUpService.Load();

            // 运行主窗体并传入应用设置服务
            Application.Run(new Form1(_iappConfigService, _iheroDataService, _ilineUpService, _iCorrectionService));

        }
    }
}