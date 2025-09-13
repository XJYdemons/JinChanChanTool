using JinChanChanTool.Services.DataServices;
using System.Diagnostics;
namespace JinChanChanTool
{
    internal static class Program
    {       
        [STAThread]
        static void Main()
        {
            // ���ø�DPIģʽ
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ApplicationConfiguration.Initialize();
        
            //����������Ӧ�����÷���
            IAppConfigService _iappConfigService = new AppConfigService();
            _iappConfigService.Load();

            //����OCR�����������
            ICorrectionService _iCorrectionService = new CorrectionService();
            _iCorrectionService.Load();

            // ����������Ӣ�����ݷ���
            IHeroDataService _iheroDataService = new HeroDataService();
            _iheroDataService.Load();

            //�����������������ݷ���
            ILineUpService _ilineUpService = new LineUpService(_iheroDataService, _iappConfigService.CurrentConfig.CountOfLine,_iappConfigService.CurrentConfig.MaxOfChoices);
            _ilineUpService.Load();

            // ����������Ӣ��װ���Ƽ����ݷ���
            IHeroEquipmentDataService _iheroEquipmentDataService = new HeroEquipmentDataService();
            _iheroEquipmentDataService.Load();

            // ���������岢����Ӧ�����÷���
            Application.Run(new Form1(_iappConfigService, _iheroDataService, _ilineUpService, _iCorrectionService, _iheroEquipmentDataService));

        }
    }
}