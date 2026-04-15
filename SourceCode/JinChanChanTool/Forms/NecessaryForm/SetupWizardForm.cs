using JinChanChanTool.DataClass;
using JinChanChanTool.DataClass.GPUEnvironments;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms;
using JinChanChanTool.Forms.DisplayUIForm;
using JinChanChanTool.Services;
using JinChanChanTool.Services.AutoSetCoordinates;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Services.GPUEnvironments;
using JinChanChanTool.Services.LineupCrawling;
using JinChanChanTool.Services.Localization;
using JinChanChanTool.Services.ManuallySetCoordinates;
using JinChanChanTool.Services.RecommendedEquipment;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using JinChanChanTool.Tools;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.LineUpCodeTools;
using JinChanChanTool.Tools.MouseTools;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static JinChanChanTool.DataClass.LineUp;
using System.Text.RegularExpressions;
using Microsoft.Win32;
namespace JinChanChanTool
{
    public partial class SetupWizardForm : Form
    {
        #region 初始化相关

        private readonly IManualSettingsService _manualSettingsService;
        private readonly ILocalizationService _iLocalizationService;

        // GPU环境配置相关服务
        private readonly GpuDetectionService _gpuDetectionService;
        private readonly CudaDetectionService _cudaDetectionService;
        private readonly CudaInstallerService _cudaInstallerService;
        private readonly CudnnInstallerService _cudnnInstallerService;
        private readonly RuntimeDeployService _runtimeDeployService;

        // GPU环境检测结果
        private GpuInfo? _gpuInfo;
        private CudaInfo? _cudaInfo;

        // GPU环境配置取消令牌
        private CancellationTokenSource? _gpuConfigCancellationTokenSource;
        private bool _isGpuConfiguring;

        public SetupWizardForm(IManualSettingsService manualSettingsService, ILocalizationService iLocalizationService)
        {
            InitializeComponent();
            _manualSettingsService = manualSettingsService;
            _iLocalizationService = iLocalizationService;
            //添加拖动
            DragHelper.EnableDragForChildren(panel_标题栏);
            //应用本地化
            ApplyLocalization();
            updateUI();
            Pages = new List<Panel>()
            {
                panel_1欢迎页,
                panel_2坐标设置模式,
                panel_3自动设置坐标,
                panel_4手动设置坐标,
                Panel_5选择拿牌方式,
                panel_6选择刷新商店方式,
                panel_7选择OCR推理设备,
                panel_8GPU环境配置,
                panel_9配置完成
            };
            UpdatePage();
            screens = Screen.AllScreens;
            LoadDisplays();

            // 初始化拿牌方式开关状态
            初始化拿牌方式();
            // 初始化刷新方式开关状态
            初始化刷新方式();
            // 初始化推理方式开关状态
            初始化推理方式();

            // 初始化GPU环境配置服务
            _gpuDetectionService = new GpuDetectionService();
            _cudaDetectionService = new CudaDetectionService();
            _cudaInstallerService = new CudaInstallerService();
            _cudnnInstallerService = new CudnnInstallerService();
            _runtimeDeployService = new RuntimeDeployService();

            // 订阅安装进度事件
            _cudaInstallerService.ProgressChanged += OnCudaInstallerProgress;
            _cudnnInstallerService.ProgressChanged += OnCudnnInstallerProgress;
            _runtimeDeployService.ProgressChanged += OnRuntimeDeployProgress;
        }
        private async void Form1_Load(object sender, EventArgs e)
        {

        }

        #region 显示器相关逻辑       
        private Screen targetScreen;//目标显示器
        private Screen[] screens;//显示器数组
        /// <summary>
        /// 加载所有显示器并填充到 ComboBox 中
        /// </summary>
        private void LoadDisplays()
        {
            // 清空显示器下拉框            
            comboBox_选择显示器.Items.Clear();
            // 查询每个显示器的设备名称
            for (int i = 0; i < screens.Length; i++)
            {
                // 将显示器的序号和设备名称添加到显示器下拉框
                comboBox_选择显示器.Items.Add($"{i + 1} - {screens[i].DeviceName}");
            }
            if (comboBox_选择显示器.Items.Count > 0)
            {
                comboBox_选择显示器.SelectedIndex = 0;
            }
        }
        #endregion

        #region 通用文本框事件
        private void Update_AllComponents()
        {

        }

        /// <summary>
        /// 通用的文本框进入事件 ——> 进入时清空文本框内容并禁用快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Enter(object sender, EventArgs e)
        {
            //禁用全局热键，防止冲突
            GlobalHotkeyTool.Enabled = false;
            // 当用户进入文本框时，清空现有内容
            (sender as TextBox).Text = "";
        }

        /// <summary>
        /// 通用的文本框离开事件 ——> 离开时若文本框内容为空则从应用设置服务类读取显示，否则不做任何操作，并启用快捷键。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                Update_AllComponents();
            }

        }

        /// <summary>
        /// 通用的文本框按键按下事件 ——> 若用户键入回车，则使该组件失焦，并启用快捷键。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion
        #endregion

        private List<Panel> Pages;
        private int pageIndex = 0;

        /// <summary>
        /// 更新页面显示
        /// </summary>
        private void UpdatePage()
        {
            if (pageIndex < Pages.Count && pageIndex >= 0)
            {
                // 更新按钮显示状态
                if (pageIndex == 0)
                {
                    button_上一步.Visible = false;
                    button_下一步.Visible = true;
                    button_完成.Visible = false;
                }
                else if (pageIndex == Pages.Count - 1)
                {
                    button_上一步.Visible = true;
                    button_下一步.Visible = false;
                    button_完成.Visible = true;

                    // 在最后一页显示配置摘要
                    richTextBox_配置概览.Text = GenerateConfigSummary();
                }
                else
                {
                    button_上一步.Visible = true;
                    button_下一步.Visible = true;
                    button_完成.Visible = false;
                }

                // 更新页面显示
                for (int i = 0; i < Pages.Count; i++)
                {
                    if (i == pageIndex)
                    {
                        Pages[i].Visible = true;
                    }
                    else
                    {
                        Pages[i].Visible = false;
                    }
                }

                // 页面8：GPU环境配置 - 进入时自动检测
                if (pageIndex == 7 && _gpuInfo == null)
                {
                    DetectGpuEnvironmentAsync();
                }
            }
        }
        private void updateUI()
        {
            radioButton_手动设置坐标.Checked = IsUseFixedCoordinates;
            radioButton_自动设置坐标.Checked = IsUseDynamicCoordinates;
        }

        #region 本地化
        /// <summary>
        /// 应用本地化文本到所有控件
        /// </summary>
        private void ApplyLocalization()
        {
            // 标题栏
            label_标题.Text = _iLocalizationService.Get("SetupWizard.标题");

            // 按钮区
            button_下一步.Text = _iLocalizationService.Get("SetupWizard.Button.下一步");
            button_上一步.Text = _iLocalizationService.Get("SetupWizard.Button.上一步");
            button_跳过向导.Text = _iLocalizationService.Get("SetupWizard.Button.跳过向导");
            button_完成.Text = _iLocalizationService.Get("SetupWizard.Button.完成");

            // 页1：欢迎页
            label_欢迎使用.Text = _iLocalizationService.Get("SetupWizard.Page1.欢迎使用");
            label_配置向导描述.Text = _iLocalizationService.Get("SetupWizard.Page1.配置向导描述");
            label_配置向导页描述.Text = _iLocalizationService.Get("SetupWizard.Page1.配置向导页描述");
            label_点击下一步继续.Text = _iLocalizationService.Get("SetupWizard.Page1.点击下一步继续");

            // 页2：坐标设置模式
            label_选择坐标设置模式.Text = _iLocalizationService.Get("SetupWizard.Page2.选择坐标设置模式");
            label_坐标描述.Text = _iLocalizationService.Get("SetupWizard.Page2.坐标描述");
            radioButton_自动设置坐标.Text = _iLocalizationService.Get("SetupWizard.Page2.自动设置坐标");
            label_自动坐标模式描述.Text = _iLocalizationService.Get("SetupWizard.Page2.自动坐标模式描述");
            radioButton_手动设置坐标.Text = _iLocalizationService.Get("SetupWizard.Page2.手动设置坐标");
            label_手动坐标模式描述.Text = _iLocalizationService.Get("SetupWizard.Page2.手动坐标模式描述");

            // 页3：自动设置坐标
            label_自动设置坐标.Text = _iLocalizationService.Get("SetupWizard.Page3.自动设置坐标");
            label_自动设置坐标提示.Text = _iLocalizationService.Get("SetupWizard.Page3.自动设置坐标提示");
            label_选择游戏进程.Text = _iLocalizationService.Get("SetupWizard.Page3.选择游戏进程");
            选择游戏窗口进程.Text = _iLocalizationService.Get("SetupWizard.Page3.选择游戏窗口进程");
            label_进程状态.Text = _iLocalizationService.Get("SetupWizard.Page3.未选择进程");

            // 页4：手动设置坐标
            label_手动设置坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.手动设置坐标");
            label_手动设置坐标提示.Text = _iLocalizationService.Get("SetupWizard.Page4.手动设置坐标提示");
            label_选择显示器.Text = _iLocalizationService.Get("SetupWizard.Page4.选择显示器");
            roundedButton_设置英雄名称坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.设置英雄名称坐标");
            roundedButton_设置刷新按钮坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.设置刷新按钮坐标");
            roundedButton_设置高亮提示坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.设置高亮提示坐标");
            label_设置英雄名称坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.未设置");
            label_设置刷新按钮坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.未设置");
            label_设置高亮提示坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.未设置");

            // 页5：选择拿牌方式
            label_选择拿牌方式.Text = _iLocalizationService.Get("SetupWizard.Page5.选择拿牌方式");
            label_购买英雄按键描述.Text = _iLocalizationService.Get("SetupWizard.Page5.购买英雄按键描述");
            label_鼠标拿牌.Text = _iLocalizationService.Get("SetupWizard.Page5.模拟鼠标拿牌");
            label_按键拿牌.Text = _iLocalizationService.Get("SetupWizard.Page5.模拟按键拿牌");
            label_拿牌按键1.Text = _iLocalizationService.Get("SetupWizard.Page5.拿牌按键", 1);
            label_拿牌按键2.Text = _iLocalizationService.Get("SetupWizard.Page5.拿牌按键", 2);
            label_拿牌按键3.Text = _iLocalizationService.Get("SetupWizard.Page5.拿牌按键", 3);
            label_拿牌按键4.Text = _iLocalizationService.Get("SetupWizard.Page5.拿牌按键", 4);
            label_拿牌按键5.Text = _iLocalizationService.Get("SetupWizard.Page5.拿牌按键", 5);

            // 页6：选择刷新商店方式
            label_选择刷新商店方式.Text = _iLocalizationService.Get("SetupWizard.Page6.选择刷新商店方式");
            label_选择刷新商店方式描述.Text = _iLocalizationService.Get("SetupWizard.Page6.选择刷新商店方式描述");
            label_鼠标刷新.Text = _iLocalizationService.Get("SetupWizard.Page6.鼠标模拟刷新商店");
            label_按键刷新.Text = _iLocalizationService.Get("SetupWizard.Page6.按键模拟刷新商店");
            label_刷新按键.Text = _iLocalizationService.Get("SetupWizard.Page6.刷新商店按键");

            // 页7：选择OCR推理设备
            label_选择OCR推理设备.Text = _iLocalizationService.Get("SetupWizard.Page7.选择OCR推理设备");
            label_OCR设备描述.Text = _iLocalizationService.Get("SetupWizard.Page7.OCR设备描述");
            label_CPU推理.Text = _iLocalizationService.Get("SetupWizard.Page7.CPU推理");
            label_CPU说明.Text = _iLocalizationService.Get("SetupWizard.Page7.CPU说明");
            label_GPU推理.Text = _iLocalizationService.Get("SetupWizard.Page7.GPU推理");
            label_GPU说明.Text = _iLocalizationService.Get("SetupWizard.Page7.GPU说明");

            // 页8：GPU环境配置
            label_GPU环境配置.Text = _iLocalizationService.Get("SetupWizard.Page8.GPU环境配置");
            label_GPU环境配置页描述.Text = _iLocalizationService.Get("SetupWizard.Page8.GPU环境配置页描述");
            groupBox_环境检测.Text = _iLocalizationService.Get("SetupWizard.Page8.环境检测");
            groupBox_安装配置.Text = _iLocalizationService.Get("SetupWizard.Page8.安装配置");
            label_GPU状态.Text = _iLocalizationService.Get("SetupWizard.Page8.显卡未检测");
            label_CUDA状态.Text = _iLocalizationService.Get("SetupWizard.Page8.CUDA未检测");
            label_cuDNN状态.Text = _iLocalizationService.Get("SetupWizard.Page8.cuDNN未检测");
            button_检测GPU环境.Text = _iLocalizationService.Get("SetupWizard.Page8.检测GPU环境");
            button_一键配置.Text = _iLocalizationService.Get("SetupWizard.Page8.一键配置");
            label_CUDA版本.Text = _iLocalizationService.Get("SetupWizard.Page8.CUDA版本");

            // 页9：配置完成
            label_配置完成.Text = _iLocalizationService.Get("SetupWizard.Page9.配置完成");
            label_完成说明.Text = _iLocalizationService.Get("SetupWizard.Page9.完成说明");
        }
        #endregion

        #region 页2-选择坐标设置模式        
        private bool IsUseFixedCoordinates = false;
        private bool IsUseDynamicCoordinates = true;
        private void radioButton_自动设置坐标_CheckedChanged(object sender, EventArgs e)
        {
            IsUseDynamicCoordinates = radioButton_自动设置坐标.Checked;
            IsUseFixedCoordinates = radioButton_手动设置坐标.Checked;
        }

        private void radioButton_手动设置坐标_CheckedChanged(object sender, EventArgs e)
        {
            IsUseDynamicCoordinates = radioButton_自动设置坐标.Checked;
            IsUseFixedCoordinates = radioButton_手动设置坐标.Checked;
        }
        #endregion

        #region 页3-自动设置坐标
        private string TargetProcessName = "";
        private int TargetProcessId = 0;

        private void 选择游戏窗口进程_Click(object sender, EventArgs e)
        {
            // 1. 实时创建进程发现服务
            var discoveryService = new ProcessDiscoveryService();

            // 2. 创建并显示进程选择窗体
            using (var processForm = new ProcessSelectorForm(discoveryService, _iLocalizationService))
            {
                if (processForm.ShowDialog(this) == DialogResult.OK)
                {
                    var selectedProcess = processForm.SelectedProcess;
                    if (selectedProcess != null)
                    {
                        // 同时保存 Name 和 ID
                        TargetProcessName = selectedProcess.ProcessName;
                        TargetProcessId = selectedProcess.Id;
                        // 给用户反馈
                        string displayName = $"{selectedProcess.ProcessName} (ID: {selectedProcess.Id})";

                        label_进程状态.Text = _iLocalizationService.Get("SetupWizard.Page3.已选择进程", displayName);
                        label_进程状态.ForeColor = Color.FromArgb(0, 150, 0);
                    }
                }
            }
        }
        #endregion

        #region 页4-手动设置坐标

        public Rectangle HeroNameScreenshotRectangle_1;

        public Rectangle HeroNameScreenshotRectangle_2;

        public Rectangle HeroNameScreenshotRectangle_3;

        public Rectangle HeroNameScreenshotRectangle_4;

        public Rectangle HeroNameScreenshotRectangle_5;

        public Rectangle RefreshStoreButtonRectangle;

        public Rectangle HighLightRectangle_1;

        public Rectangle HighLightRectangle_2;

        public Rectangle HighLightRectangle_3;

        public Rectangle HighLightRectangle_4;

        public Rectangle HighLightRectangle_5;

        /// <summary>
        /// 选择显示器下拉框选择项改变时触发 ——> targetScreen值更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_选择显示器_SelectedIndexChanged(object sender, EventArgs e)
        {
            targetScreen = screens[comboBox_选择显示器.SelectedIndex];
        }

        private async void roundedButton_设置英雄名称坐标_Click(object sender, EventArgs e)
        {
            using (var setter = new FastSettingPositionService(targetScreen, _iLocalizationService))
            {
                try
                {
                    // 第一张卡片
                    var rect1 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.弈子坐标设置提示词1"));
                    HeroNameScreenshotRectangle_1 = rect1;

                    // 第二张卡片
                    var rect2 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.弈子坐标设置提示词2"));
                    HeroNameScreenshotRectangle_2 = rect2;

                    // 第三张卡片
                    var rect3 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.弈子坐标设置提示词3"));
                    HeroNameScreenshotRectangle_3 = rect3;

                    // 第四张卡片
                    var rect4 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.弈子坐标设置提示词4"));
                    HeroNameScreenshotRectangle_4 = rect4;

                    // 第五张卡片
                    var rect5 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.弈子坐标设置提示词5"));
                    HeroNameScreenshotRectangle_5 = rect5;
                    label_设置英雄名称坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.已设置完成");
                    label_设置英雄名称坐标.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_iLocalizationService.Get("SetupWizard.Msg.出现错误", ex.Message));
                }
            }
        }

        private async void roundedButton_设置刷新按钮坐标_Click(object sender, EventArgs e)
        {
            using (var setter = new FastSettingPositionService(targetScreen, _iLocalizationService))
            {
                try
                {
                    Rectangle rectangle = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.商店刷新按钮坐标设置提示词"));
                    RefreshStoreButtonRectangle = rectangle;
                    label_设置刷新按钮坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.已设置完成");
                    label_设置刷新按钮坐标.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_iLocalizationService.Get("SetupWizard.Msg.出现错误", ex.Message));
                }
            }
        }

        private async void roundedButton_设置高亮提示坐标_Click(object sender, EventArgs e)
        {
            using (var setter = new FastSettingPositionService(targetScreen, _iLocalizationService))
            {
                try
                {
                    // 第一张卡片
                    var rect1 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.高亮提示框坐标设置提示词1"));
                    HighLightRectangle_1 = rect1;

                    // 第二张卡片
                    var rect2 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.高亮提示框坐标设置提示词2"));
                    HighLightRectangle_2 = rect2;

                    // 第三张卡片
                    var rect3 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.高亮提示框坐标设置提示词3"));
                    HighLightRectangle_3 = rect3;

                    // 第四张卡片
                    var rect4 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.高亮提示框坐标设置提示词4"));
                    HighLightRectangle_4 = rect4;

                    // 第五张卡片
                    var rect5 = await setter.WaitForDrawAsync(
                        _iLocalizationService.Get("SetupWizard.Page4.高亮提示框坐标设置提示词5"));
                    HighLightRectangle_5 = rect5;
                    label_设置高亮提示坐标.Text = _iLocalizationService.Get("SetupWizard.Page4.已设置完成");
                    label_设置高亮提示坐标.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_iLocalizationService.Get("SetupWizard.Msg.出现错误", ex.Message));
                }
            }
        }
        #endregion

        #region 页5-选择拿牌方式

        private bool isMouseHeroPurchase = true;

        private bool isKeyboardHeroPurchase = false;

        /// <summary>
        /// 拿牌按键输入框更新标志（防止循环触发）
        /// </summary>
        private bool isUpdatingSwitch_鼠标拿牌 = false;
        private bool isUpdatingSwitch_按键拿牌 = false;

        public string HeroPurchaseKey1 = "Q";

        public string HeroPurchaseKey2 = "W";

        public string HeroPurchaseKey3 = "E";

        public string HeroPurchaseKey4 = "R";

        public string HeroPurchaseKey5 = "T";

        /// <summary>
        /// 鼠标拿牌开关状态改变事件
        /// </summary>
        private void capsuleSwitch_鼠标拿牌_IsOnChanged(object sender, EventArgs e)
        {
            isMouseHeroPurchase = capsuleSwitch_鼠标拿牌.IsOn;
            if (isUpdatingSwitch_鼠标拿牌) return;
            拿牌方式变更_鼠标拿牌();
        }

        /// <summary>
        /// 按键拿牌开关状态改变事件
        /// </summary>
        private void capsuleSwitch_按键拿牌_IsOnChanged(object sender, EventArgs e)
        {
            isKeyboardHeroPurchase = capsuleSwitch_按键拿牌.IsOn;
            if (isUpdatingSwitch_按键拿牌) return;
            拿牌方式变更_按键拿牌();
        }

        /// <summary>
        /// 拿牌方式变更 - 鼠标拿牌
        /// </summary>
        private void 拿牌方式变更_鼠标拿牌()
        {
            if (isMouseHeroPurchase)
            {
                // 鼠标拿牌开启时，禁用按键文本框，关闭按键拿牌开关
                textBox_拿牌按键1.Enabled = false;
                textBox_拿牌按键2.Enabled = false;
                textBox_拿牌按键3.Enabled = false;
                textBox_拿牌按键4.Enabled = false;
                textBox_拿牌按键5.Enabled = false;
                isUpdatingSwitch_按键拿牌 = true;
                capsuleSwitch_按键拿牌.IsOn = false;
                isUpdatingSwitch_按键拿牌 = false;
            }
            else
            {
                // 鼠标拿牌关闭时，启用按键文本框，开启按键拿牌开关
                textBox_拿牌按键1.Enabled = true;
                textBox_拿牌按键2.Enabled = true;
                textBox_拿牌按键3.Enabled = true;
                textBox_拿牌按键4.Enabled = true;
                textBox_拿牌按键5.Enabled = true;
                isUpdatingSwitch_按键拿牌 = true;
                capsuleSwitch_按键拿牌.IsOn = true;
                isUpdatingSwitch_按键拿牌 = false;
            }
        }

        /// <summary>
        /// 拿牌方式变更 - 按键拿牌
        /// </summary>
        private void 拿牌方式变更_按键拿牌()
        {
            if (isKeyboardHeroPurchase)
            {
                // 按键拿牌开启时，启用按键文本框，关闭鼠标拿牌开关
                textBox_拿牌按键1.Enabled = true;
                textBox_拿牌按键2.Enabled = true;
                textBox_拿牌按键3.Enabled = true;
                textBox_拿牌按键4.Enabled = true;
                textBox_拿牌按键5.Enabled = true;
                isUpdatingSwitch_鼠标拿牌 = true;
                capsuleSwitch_鼠标拿牌.IsOn = false;
                isUpdatingSwitch_鼠标拿牌 = false;
            }
            else
            {
                // 按键拿牌关闭时，禁用按键文本框，开启鼠标拿牌开关
                textBox_拿牌按键1.Enabled = false;
                textBox_拿牌按键2.Enabled = false;
                textBox_拿牌按键3.Enabled = false;
                textBox_拿牌按键4.Enabled = false;
                textBox_拿牌按键5.Enabled = false;
                isUpdatingSwitch_鼠标拿牌 = true;
                capsuleSwitch_鼠标拿牌.IsOn = true;
                isUpdatingSwitch_鼠标拿牌 = false;
            }
        }

        /// <summary>
        /// 初始化拿牌方式开关和文本框状态
        /// </summary>
        private void 初始化拿牌方式()
        {
            // 设置开关初始状态
            capsuleSwitch_鼠标拿牌.IsOn = isMouseHeroPurchase;
            capsuleSwitch_按键拿牌.IsOn = isKeyboardHeroPurchase;

            // 填充默认按键值到文本框
            textBox_拿牌按键1.Text = HeroPurchaseKey1;
            textBox_拿牌按键2.Text = HeroPurchaseKey2;
            textBox_拿牌按键3.Text = HeroPurchaseKey3;
            textBox_拿牌按键4.Text = HeroPurchaseKey4;
            textBox_拿牌按键5.Text = HeroPurchaseKey5;

            // 根据默认状态设置文本框启用状态
            if (isMouseHeroPurchase)
            {
                textBox_拿牌按键1.Enabled = false;
                textBox_拿牌按键2.Enabled = false;
                textBox_拿牌按键3.Enabled = false;
                textBox_拿牌按键4.Enabled = false;
                textBox_拿牌按键5.Enabled = false;
            }
            else
            {
                textBox_拿牌按键1.Enabled = true;
                textBox_拿牌按键2.Enabled = true;
                textBox_拿牌按键3.Enabled = true;
                textBox_拿牌按键4.Enabled = true;
                textBox_拿牌按键5.Enabled = true;
            }
        }

        private void textBox_拿牌按键1_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (KeyboardControlTool.IsRightKey(key))
            {
                HeroPurchaseKey1 = key.ToString();
                textBox_拿牌按键1.Text = key.ToString();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        private void textBox_拿牌按键2_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (KeyboardControlTool.IsRightKey(key))
            {
                HeroPurchaseKey2 = key.ToString();
                textBox_拿牌按键2.Text = key.ToString();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        private void textBox_拿牌按键3_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (KeyboardControlTool.IsRightKey(key))
            {
                HeroPurchaseKey3 = key.ToString();
                textBox_拿牌按键3.Text = key.ToString();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        private void textBox_拿牌按键4_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (KeyboardControlTool.IsRightKey(key))
            {
                HeroPurchaseKey4 = key.ToString();
                textBox_拿牌按键4.Text = key.ToString();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        private void textBox_拿牌按键5_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (KeyboardControlTool.IsRightKey(key))
            {
                HeroPurchaseKey5 = key.ToString();
                textBox_拿牌按键5.Text = key.ToString();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion

        #region 页6-选择刷新商店方式

        /// <summary>
        /// 是否使用鼠标刷新
        /// </summary>
        private bool isMouseRefreshStore = true;

        /// <summary>
        /// 是否使用按键刷新
        /// </summary>
        private bool isKeyboardRefreshStore = false;

        /// <summary>
        /// 刷新按键输入框更新标志（防止循环触发）
        /// </summary>
        private bool isUpdatingSwitch_鼠标刷新 = false;
        private bool isUpdatingSwitch_按键刷新 = false;

        /// <summary>
        /// 刷新按键默认值
        /// </summary>
        public string RefreshStoreKey = "D";

        /// <summary>
        /// 鼠标刷新开关状态改变事件
        /// </summary>
        private void capsuleSwitch_鼠标刷新_IsOnChanged(object sender, EventArgs e)
        {
            isMouseRefreshStore = capsuleSwitch_鼠标刷新.IsOn;
            if (isUpdatingSwitch_鼠标刷新) return;
            刷新方式变更_鼠标刷新();
        }

        /// <summary>
        /// 按键刷新开关状态改变事件
        /// </summary>
        private void capsuleSwitch_按键刷新_IsOnChanged(object sender, EventArgs e)
        {
            isKeyboardRefreshStore = capsuleSwitch_按键刷新.IsOn;
            if (isUpdatingSwitch_按键刷新) return;
            刷新方式变更_按键刷新();
        }

        /// <summary>
        /// 刷新方式变更 - 鼠标刷新
        /// </summary>
        private void 刷新方式变更_鼠标刷新()
        {
            if (isMouseRefreshStore)
            {
                // 鼠标刷新开启时，禁用按键文本框，关闭按键刷新开关
                textBox_刷新按键.Enabled = false;
                isUpdatingSwitch_按键刷新 = true;
                capsuleSwitch_按键刷新.IsOn = false;
                isUpdatingSwitch_按键刷新 = false;
            }
            else
            {
                // 鼠标刷新关闭时，启用按键文本框，开启按键刷新开关
                textBox_刷新按键.Enabled = true;
                isUpdatingSwitch_按键刷新 = true;
                capsuleSwitch_按键刷新.IsOn = true;
                isUpdatingSwitch_按键刷新 = false;
            }
        }

        /// <summary>
        /// 刷新方式变更 - 按键刷新
        /// </summary>
        private void 刷新方式变更_按键刷新()
        {
            if (isKeyboardRefreshStore)
            {
                // 按键刷新开启时，启用按键文本框，关闭鼠标刷新开关
                textBox_刷新按键.Enabled = true;
                isUpdatingSwitch_鼠标刷新 = true;
                capsuleSwitch_鼠标刷新.IsOn = false;
                isUpdatingSwitch_鼠标刷新 = false;
            }
            else
            {
                // 按键刷新关闭时，禁用按键文本框，开启鼠标刷新开关
                textBox_刷新按键.Enabled = false;
                isUpdatingSwitch_鼠标刷新 = true;
                capsuleSwitch_鼠标刷新.IsOn = true;
                isUpdatingSwitch_鼠标刷新 = false;
            }
        }

        /// <summary>
        /// 初始化刷新方式开关和文本框状态
        /// </summary>
        private void 初始化刷新方式()
        {
            // 设置开关初始状态
            capsuleSwitch_鼠标刷新.IsOn = isMouseRefreshStore;
            capsuleSwitch_按键刷新.IsOn = isKeyboardRefreshStore;

            // 填充默认按键值到文本框
            textBox_刷新按键.Text = RefreshStoreKey;

            // 根据默认状态设置文本框启用状态
            if (isMouseRefreshStore)
            {
                textBox_刷新按键.Enabled = false;
            }
            else
            {
                textBox_刷新按键.Enabled = true;
            }
        }

        /// <summary>
        /// 刷新按键文本框按键按下事件
        /// </summary>
        private void textBox_刷新按键_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;
                return;
            }
            if (KeyboardControlTool.IsRightKey(key))
            {
                RefreshStoreKey = key.ToString();
                textBox_刷新按键.Text = key.ToString();
                e.SuppressKeyPress = true;
            }
            GlobalHotkeyTool.Enabled = true;
        }
        #endregion

        #region 页7-选择OCR推理设备

        /// <summary>
        /// 是否使用CPU推理
        /// </summary>
        private bool isUseCPUForInference = true;

        /// <summary>
        /// 是否使用GPU推理
        /// </summary>
        private bool isUseGPUForInference = false;

        /// <summary>
        /// 推理模式更新标志（防止循环触发）
        /// </summary>
        private bool isUpdatingSwitch_CPU推理 = false;
        private bool isUpdatingSwitch_GPU推理 = false;

        /// <summary>
        /// CPU推理开关状态改变事件
        /// </summary>
        private void capsuleSwitch_CPU推理_IsOnChanged(object sender, EventArgs e)
        {
            isUseCPUForInference = capsuleSwitch_CPU推理.IsOn;
            if (isUpdatingSwitch_CPU推理) return;
            推理方式变更_CPU();
        }

        /// <summary>
        /// GPU推理开关状态改变事件
        /// </summary>
        private void capsuleSwitch_GPU推理_IsOnChanged(object sender, EventArgs e)
        {
            isUseGPUForInference = capsuleSwitch_GPU推理.IsOn;
            if (isUpdatingSwitch_GPU推理) return;
            推理方式变更_GPU();
        }

        /// <summary>
        /// 推理方式变更 - CPU
        /// </summary>
        private void 推理方式变更_CPU()
        {
            if (isUseCPUForInference)
            {
                // CPU推理开启时，关闭GPU推理开关
                isUpdatingSwitch_GPU推理 = true;
                capsuleSwitch_GPU推理.IsOn = false;
                isUpdatingSwitch_GPU推理 = false;
            }
            else
            {
                // CPU推理关闭时，开启GPU推理开关
                isUpdatingSwitch_GPU推理 = true;
                capsuleSwitch_GPU推理.IsOn = true;
                isUpdatingSwitch_GPU推理 = false;
            }
        }

        /// <summary>
        /// 推理方式变更 - GPU
        /// </summary>
        private void 推理方式变更_GPU()
        {
            if (isUseGPUForInference)
            {
                // GPU推理开启时，关闭CPU推理开关
                isUpdatingSwitch_CPU推理 = true;
                capsuleSwitch_CPU推理.IsOn = false;
                isUpdatingSwitch_CPU推理 = false;
            }
            else
            {
                // GPU推理关闭时，开启CPU推理开关
                isUpdatingSwitch_CPU推理 = true;
                capsuleSwitch_CPU推理.IsOn = true;
                isUpdatingSwitch_CPU推理 = false;
            }
        }

        /// <summary>
        /// 初始化推理方式开关状态
        /// </summary>
        private void 初始化推理方式()
        {
            // 设置开关初始状态
            capsuleSwitch_CPU推理.IsOn = isUseCPUForInference;
            capsuleSwitch_GPU推理.IsOn = isUseGPUForInference;
        }
        #endregion

        #region 页8-GPU环境配置
        /// <summary>
        /// GPU环境检测状态
        /// </summary>
        private string gpuStatusText = "";
        private string cudaStatusText = "";
        private string cudnnStatusText = "";

        /// <summary>
        /// 检测GPU环境按钮点击事件
        /// </summary>
        private async void button_检测GPU环境_Click(object sender, EventArgs e)
        {
            await DetectGpuEnvironmentAsync();
        }

        /// <summary>
        /// 异步检测GPU环境
        /// </summary>
        private async Task DetectGpuEnvironmentAsync()
        {
            button_检测GPU环境.Enabled = false;
           
            button_一键配置.Enabled = false;

            label_GPU状态.Text = _iLocalizationService.Get("SetupWizard.Page8.显卡检测中");
            label_CUDA状态.Text = _iLocalizationService.Get("SetupWizard.Page8.CUDA检测中");
            label_cuDNN状态.Text = _iLocalizationService.Get("SetupWizard.Page8.cuDNN检测中");

            await Task.Run(() =>
            {
                // 检测GPU
                _gpuInfo = _gpuDetectionService.DetectGpu();

                // 检测CUDA环境
                _cudaInfo = _cudaDetectionService.DetectCudaEnvironment();
            });

            // 更新UI
            UpdateGpuEnvironmentStatus();

            // 更新CUDA版本选项
            UpdateCudaVersionOptions();

            button_检测GPU环境.Enabled = true;
           
            button_一键配置.Enabled = _gpuInfo?.IsSupportedForInference == true;
        }

        /// <summary>
        /// 更新GPU环境状态显示
        /// </summary>
        private void UpdateGpuEnvironmentStatus()
        {
            // 更新GPU状态
            if (_gpuInfo == null || !_gpuInfo.IsNvidiaGpuDetected)
            {
                gpuStatusText = _iLocalizationService.Get("SetupWizard.Page8.未检测到NVIDIA显卡");
                label_GPU状态.Text = _iLocalizationService.Get("SetupWizard.Page8.显卡状态", gpuStatusText);
                label_GPU状态.ForeColor = Color.Red;
            }
            else if (!_gpuInfo.IsSupportedForInference)
            {
                gpuStatusText = _iLocalizationService.Get("SetupWizard.Page8.GPU不支持推理", _gpuInfo.GpuName);
                label_GPU状态.Text = _iLocalizationService.Get("SetupWizard.Page8.显卡状态", gpuStatusText);
                label_GPU状态.ForeColor = Color.Orange;
            }
            else
            {
                // 显示支持的最高CUDA版本
                string maxCuda = _gpuInfo.MaxSupportedCudaVersion;
                string cudaInfo = !string.IsNullOrEmpty(maxCuda)
                    ? _iLocalizationService.Get("SetupWizard.Page8.最高支持CUDA", maxCuda)
                    : "";
                gpuStatusText = _iLocalizationService.Get("SetupWizard.Page8.GPU支持信息",
                    _gpuInfo.GpuName, _gpuInfo.SmVersionString, cudaInfo);
                label_GPU状态.Text = _iLocalizationService.Get("SetupWizard.Page8.显卡状态", gpuStatusText);
                label_GPU状态.ForeColor = Color.Green;
            }

            // 更新CUDA状态
            if (_cudaInfo == null || !_cudaInfo.IsCudaInstalled)
            {
                cudaStatusText = _iLocalizationService.Get("SetupWizard.Page8.CUDA未安装");
                label_CUDA状态.Text = _iLocalizationService.Get("SetupWizard.Page8.CUDA状态", cudaStatusText);
                label_CUDA状态.ForeColor = Color.Orange;
            }
            else if (!_cudaInfo.IsSupportedCudaVersion)
            {
                cudaStatusText = _iLocalizationService.Get("SetupWizard.Page8.CUDA版本不兼容", _cudaInfo.CudaVersion);
                label_CUDA状态.Text = _iLocalizationService.Get("SetupWizard.Page8.CUDA状态", cudaStatusText);
                label_CUDA状态.ForeColor = Color.Orange;
            }
            else
            {
                cudaStatusText = $"{_cudaInfo.CudaVersion}";
                label_CUDA状态.Text = _iLocalizationService.Get("SetupWizard.Page8.CUDA状态", cudaStatusText);
                label_CUDA状态.ForeColor = Color.Green;
            }

            // 更新cuDNN状态
            if (_cudaInfo == null || !_cudaInfo.IsCudnnInstalled)
            {
                cudnnStatusText = _iLocalizationService.Get("SetupWizard.Page8.cuDNN未安装");
                label_cuDNN状态.Text = _iLocalizationService.Get("SetupWizard.Page8.cuDNN状态", cudnnStatusText);
                label_cuDNN状态.ForeColor = Color.Orange;
            }
            else
            {
                cudnnStatusText = $"{_cudaInfo.CudnnVersion}";
                label_cuDNN状态.Text = _iLocalizationService.Get("SetupWizard.Page8.cuDNN状态", cudnnStatusText);
                label_cuDNN状态.ForeColor = Color.Green;
            }
        }

        /// <summary>
        /// 更新CUDA版本选项
        /// </summary>
        private void UpdateCudaVersionOptions()
        {
            comboBox_选择CUDA版本.Items.Clear();

            if (_gpuInfo == null || !_gpuInfo.IsSupportedForInference)
            {
                return;
            }

            List<CudaVersionOption> options = RuntimeConfig.GetSupportedCudaVersions(_gpuInfo);

            foreach (CudaVersionOption option in options)
            {
                comboBox_选择CUDA版本.Items.Add(option);
            }

            // 默认选择推荐版本
            if (comboBox_选择CUDA版本.Items.Count > 0)
            {
                comboBox_选择CUDA版本.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 刷新检测按钮点击事件
        /// </summary>
        private void buttonRefreshDetection_Click(object sender, EventArgs e)
        {
            DetectGpuEnvironmentAsync();
        }

        /// <summary>
        /// 一键配置按钮点击事件
        /// </summary>
        private async void button_一键配置_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (comboBox_选择CUDA版本.SelectedItem == null)
            {
                MessageBox.Show(
                    _iLocalizationService.Get("SetupWizard.Page8.Msg.请先检测GPU"),
                    _iLocalizationService.Get("SetupWizard.MsgTitle.提示"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 获取主程序目录（当前程序所在目录）
            string targetDir = AppDomain.CurrentDomain.BaseDirectory;

            // 获取选择的CUDA版本
            CudaVersionOption selectedVersion = (CudaVersionOption)comboBox_选择CUDA版本.SelectedItem;

            // 检查驱动兼容性
            if (!selectedVersion.IsSupported)
            {
                string maxCuda = _gpuInfo?.MaxSupportedCudaVersion ?? "N/A";

                DialogResult driverWarning = MessageBox.Show(
                    _iLocalizationService.Get("SetupWizard.Page8.Msg.驱动兼容性警告",
                        maxCuda, selectedVersion.DisplayName),
                    _iLocalizationService.Get("SetupWizard.Page8.MsgTitle.驱动兼容性警告"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (driverWarning != DialogResult.Yes)
                {
                    return;
                }
            }

            string cudaTag = selectedVersion.CudaTag;

            // 确认安装
            string confirmMessage = _iLocalizationService.Get("SetupWizard.Page8.Msg.确认安装");

            // 检查是否需要安装CUDA
            bool needInstallCuda = !_cudaInfo?.IsCudaInstalled == true ||
                                   _cudaInfo?.RecommendedCudaTag != cudaTag;

            // 检查是否需要安装cuDNN
            bool needInstallCudnn = !_cudaInfo?.IsCudnnInstalled == true || needInstallCuda;

            if (needInstallCuda)
            {
                confirmMessage += _iLocalizationService.Get("SetupWizard.Page8.Msg.安装CUDA",
                    selectedVersion.DisplayName);
            }

            if (needInstallCudnn)
            {
                RuntimeConfig config = RuntimeConfig.CreateFromGpuInfo(_gpuInfo!, cudaTag);
                confirmMessage += _iLocalizationService.Get("SetupWizard.Page8.Msg.安装cuDNN",
                    config.CudnnDisplayName);
            }

            confirmMessage += _iLocalizationService.Get("SetupWizard.Page8.Msg.部署运行时", targetDir);
            confirmMessage += _iLocalizationService.Get("SetupWizard.Page8.Msg.是否继续");

            DialogResult result = MessageBox.Show(confirmMessage,
                _iLocalizationService.Get("SetupWizard.Page8.MsgTitle.确认安装"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            // 开始安装
            await StartGpuConfigurationAsync(cudaTag, targetDir, needInstallCuda, needInstallCudnn);
        }

        /// <summary>
        /// 开始GPU环境配置流程
        /// </summary>
        private async Task StartGpuConfigurationAsync(string cudaTag, string targetDir,
            bool needInstallCuda, bool needInstallCudnn)
        {
            _isGpuConfiguring = true;
            _gpuConfigCancellationTokenSource = new CancellationTokenSource();

            // 禁用界面
            SetGpuConfigUIEnabled(false);
            richTextBox_GPU环境配置输出.Clear();

            try
            {
                RuntimeConfig config = RuntimeConfig.CreateFromGpuInfo(_gpuInfo!, cudaTag);

                // 步骤1: 安装CUDA
                if (needInstallCuda)
                {
                    AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.开始安装CUDA",
                        config.CudaDisplayName), Color.Cyan);

                    bool cudaSuccess = await _cudaInstallerService.InstallCudaAsync(
                        cudaTag, _gpuConfigCancellationTokenSource.Token);

                    if (!cudaSuccess)
                    {
                        AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.CUDA安装失败"), Color.Red);
                        return;
                    }

                    AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.CUDA安装完成"), Color.Green);

                    // 重新检测CUDA环境
                    _cudaInfo = _cudaDetectionService.DetectCudaEnvironment();
                }
                else
                {
                    AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.CUDA已安装跳过"), Color.Gray);
                }

                // 步骤2: 安装cuDNN
                if (needInstallCudnn)
                {
                    AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.开始安装cuDNN",
                        config.CudnnDisplayName), Color.Cyan);

                    string cudaPath = _cudaInfo?.CudaPath ?? string.Empty;
                    if (string.IsNullOrEmpty(cudaPath))
                    {
                        // 尝试获取默认路径
                        cudaPath = _cudaDetectionService.GetCudaPathByVersion(
                            cudaTag switch
                            {
                                "cu118" => "11.8",
                                "cu126" => "12.6",
                                "cu129" => "12.9",
                                _ => "12.9"
                            }) ?? string.Empty;
                    }

                    if (string.IsNullOrEmpty(cudaPath))
                    {
                        AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.无法找到CUDA路径"), Color.Red);
                        return;
                    }

                    bool cudnnSuccess = await _cudnnInstallerService.InstallCudnnAsync(
                        config.CudnnTag, cudaPath, _gpuConfigCancellationTokenSource.Token);

                    if (!cudnnSuccess)
                    {
                        AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.cuDNN安装失败"), Color.Red);
                        return;
                    }

                    AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.cuDNN安装完成"), Color.Green);
                }
                else
                {
                    AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.cuDNN已安装跳过"), Color.Gray);
                }

                // 步骤3: 部署运行时
                AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.开始部署运行时"), Color.Cyan);

                bool runtimeSuccess = await _runtimeDeployService.DeployRuntimeAsync(
                    config, targetDir, _gpuConfigCancellationTokenSource.Token);

                if (!runtimeSuccess)
                {
                    AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.运行时部署失败"), Color.Red);
                    return;
                }

                AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.运行时部署完成"), Color.Green);

                // 全部完成
                AppendGpuLog("\n========================================", Color.Yellow);
                AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.GPU环境配置完成"), Color.Yellow);
                AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.请在设置中开启GPU推理"), Color.Yellow);
                AppendGpuLog("", Color.Yellow);
                AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.nvcc提示"), Color.Cyan);
                AppendGpuLog("========================================", Color.Yellow);

                MessageBox.Show(
                    _iLocalizationService.Get("SetupWizard.Page8.Msg.GPU配置完成"),
                    _iLocalizationService.Get("SetupWizard.Page8.MsgTitle.配置完成"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.配置已取消"), Color.Orange);
            }
            catch (Exception ex)
            {
                AppendGpuLog(_iLocalizationService.Get("SetupWizard.Page8.Log.配置出错", ex.Message), Color.Red);
            }
            finally
            {
                _isGpuConfiguring = false;
                SetGpuConfigUIEnabled(true);

                // 重新检测环境
                await DetectGpuEnvironmentAsync();
            }
        }

        /// <summary>
        /// 上一次日志消息（用于避免重复日志）
        /// </summary>
        private string _lastGpuLogMessage = string.Empty;

        /// <summary>
        /// 上一次日志时间
        /// </summary>
        private DateTime _lastGpuLogTime = DateTime.MinValue;

        /// <summary>
        /// CUDA安装进度回调
        /// </summary>
        private void OnCudaInstallerProgress(object? sender, CudaInstallerService.InstallProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnCudaInstallerProgress(sender, e));
                return;
            }

            LogGpuProgressIfNeeded(e.Message, e.ProgressPercentage, "CUDA");
        }

        /// <summary>
        /// cuDNN安装进度回调
        /// </summary>
        private void OnCudnnInstallerProgress(object? sender, CudnnInstallerService.InstallProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnCudnnInstallerProgress(sender, e));
                return;
            }

            LogGpuProgressIfNeeded(e.Message, e.ProgressPercentage, "cuDNN");
        }

        /// <summary>
        /// 运行时部署进度回调
        /// </summary>
        private void OnRuntimeDeployProgress(object? sender, RuntimeDeployService.DeployProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnRuntimeDeployProgress(sender, e));
                return;
            }

            LogGpuProgressIfNeeded(e.Message, e.ProgressPercentage, "Runtime");
        }

        /// <summary>
        /// 记录进度到日志（避免过于频繁）
        /// </summary>
        private void LogGpuProgressIfNeeded(string message, int progress, string stage)
        {
            // 避免重复日志和过于频繁的日志
            DateTime now = DateTime.Now;
            bool shouldLog = false;

            // 下载进度：每10%或每2秒记录一次
            if (message.Contains("下载") || message.Contains("Downloading"))
            {
                if ((now - _lastGpuLogTime).TotalSeconds >= 2 ||
                    progress % 10 == 0 && !_lastGpuLogMessage.Contains($"{progress}%"))
                {
                    shouldLog = true;
                }
            }
            // 安装/解压/复制等操作：消息变化时记录
            else if (message != _lastGpuLogMessage)
            {
                shouldLog = true;
            }

            if (shouldLog)
            {
                // 根据进度选择颜色
                Color logColor = progress < 50 ? Color.White :
                                progress < 90 ? Color.LightBlue : Color.LightGreen;

                AppendGpuLog($"[{stage}] {message}", logColor);
                _lastGpuLogMessage = message;
                _lastGpuLogTime = now;

                // 强制刷新UI
                Application.DoEvents();
            }
        }

        /// <summary>
        /// 添加GPU配置日志
        /// </summary>
        private void AppendGpuLog(string message, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(() => AppendGpuLog(message, color));
                return;
            }

            richTextBox_GPU环境配置输出.SelectionStart = richTextBox_GPU环境配置输出.TextLength;
            richTextBox_GPU环境配置输出.SelectionLength = 0;
            richTextBox_GPU环境配置输出.SelectionColor = color;
            richTextBox_GPU环境配置输出.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            richTextBox_GPU环境配置输出.ScrollToCaret();
        }

        /// <summary>
        /// 设置GPU配置UI启用状态
        /// </summary>
        private void SetGpuConfigUIEnabled(bool enabled)
        {
            button_一键配置.Enabled = enabled && _gpuInfo?.IsSupportedForInference == true;
            button_检测GPU环境.Enabled = enabled;
            
            comboBox_选择CUDA版本.Enabled = enabled;
        }

        #endregion

        #region 页9-配置完成-配置摘要

        /// <summary>
        /// 生成配置摘要文本
        /// </summary>
        private string GenerateConfigSummary()
        {
            System.Text.StringBuilder summary = new System.Text.StringBuilder();
            summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.配置摘要"));

            // 坐标设置模式
            summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.坐标设置"));
            if (IsUseDynamicCoordinates)
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.模式自动"));
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.目标进程",
                    TargetProcessName, TargetProcessId));
            }
            else if (IsUseFixedCoordinates)
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.模式手动"));
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.显示器",
                    comboBox_选择显示器.SelectedIndex + 1));
            }
            summary.AppendLine();

            // 拿牌方式
            summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.拿牌方式"));
            if (isMouseHeroPurchase)
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.模拟鼠标拿牌"));
            }
            else if (isKeyboardHeroPurchase)
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.模拟按键拿牌"));
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.按键列表",
                    HeroPurchaseKey1, HeroPurchaseKey2, HeroPurchaseKey3, HeroPurchaseKey4, HeroPurchaseKey5));
            }
            summary.AppendLine();

            // 刷新方式
            summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.刷新商店方式"));
            if (isMouseRefreshStore)
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.鼠标模拟刷新"));
            }
            else if (isKeyboardRefreshStore)
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.按键模拟刷新"));
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.刷新按键", RefreshStoreKey));
            }
            summary.AppendLine();

            // OCR推理设备
            summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.OCR推理设备"));
            if (isUseCPUForInference)
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.CPU推理"));
            }
            else if (isUseGPUForInference)
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.GPU推理"));
            }
            summary.AppendLine();

            // GPU环境配置
            string notDetected = _iLocalizationService.Get("SetupWizard.Summary.未检测");
            summary.AppendLine(_iLocalizationService.Get("SetupWizard.Summary.GPU环境配置"));
            if (!string.IsNullOrEmpty(gpuStatusText) && gpuStatusText != notDetected)
            {
                summary.AppendLine($"  {gpuStatusText}");
            }
            else
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Page8.显卡未检测"));
            }

            if (!string.IsNullOrEmpty(cudaStatusText) && cudaStatusText != notDetected)
            {
                summary.AppendLine($"  {cudaStatusText}");
            }
            else
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Page8.CUDA未检测"));
            }

            if (!string.IsNullOrEmpty(cudnnStatusText) && cudnnStatusText != notDetected)
            {
                summary.AppendLine($"  {cudnnStatusText}");
            }
            else
            {
                summary.AppendLine(_iLocalizationService.Get("SetupWizard.Page8.cuDNN未检测"));
            }

            return summary.ToString();
        }

        /// <summary>
        /// 同步数据到ManualSettingsService
        /// </summary>
        private void SyncDataToService()
        {
            ManualSettings config = _manualSettingsService.CurrentConfig;

            // 坐标设置模式
            config.IsUseFixedCoordinates = IsUseFixedCoordinates;
            config.IsUseDynamicCoordinates = IsUseDynamicCoordinates;

            // 自动设置坐标相关
            if (IsUseDynamicCoordinates)
            {
                config.TargetProcessName = TargetProcessName;
                config.TargetProcessId = TargetProcessId;
            }

            // 手动设置坐标相关
            if (IsUseFixedCoordinates)
            {
                config.SelectedScreenIndex = comboBox_选择显示器.SelectedIndex;
                config.HeroNameScreenshotRectangle_1 = HeroNameScreenshotRectangle_1;
                config.HeroNameScreenshotRectangle_2 = HeroNameScreenshotRectangle_2;
                config.HeroNameScreenshotRectangle_3 = HeroNameScreenshotRectangle_3;
                config.HeroNameScreenshotRectangle_4 = HeroNameScreenshotRectangle_4;
                config.HeroNameScreenshotRectangle_5 = HeroNameScreenshotRectangle_5;
                config.RefreshStoreButtonRectangle = RefreshStoreButtonRectangle;
                config.HighLightRectangle_1 = HighLightRectangle_1;
                config.HighLightRectangle_2 = HighLightRectangle_2;
                config.HighLightRectangle_3 = HighLightRectangle_3;
                config.HighLightRectangle_4 = HighLightRectangle_4;
                config.HighLightRectangle_5 = HighLightRectangle_5;
            }

            // 拿牌方式
            config.IsMouseHeroPurchase = isMouseHeroPurchase;
            config.IsKeyboardHeroPurchase = isKeyboardHeroPurchase;
            config.HeroPurchaseKey1 = HeroPurchaseKey1;
            config.HeroPurchaseKey2 = HeroPurchaseKey2;
            config.HeroPurchaseKey3 = HeroPurchaseKey3;
            config.HeroPurchaseKey4 = HeroPurchaseKey4;
            config.HeroPurchaseKey5 = HeroPurchaseKey5;

            // 刷新商店方式
            config.IsMouseRefreshStore = isMouseRefreshStore;
            config.IsKeyboardRefreshStore = isKeyboardRefreshStore;
            config.RefreshStoreKey = RefreshStoreKey;

            // OCR推理设备
            config.IsUseCPUForInference = isUseCPUForInference;
            config.IsUseGPUForInference = isUseGPUForInference;

            // 保存配置到文件
            _manualSettingsService.Save(true);
        }

        #endregion

        #region 按钮面板交互

        /// <summary>
        /// 下一步按钮点击事件
        /// </summary>
        private void button_下一步_Click(object sender, EventArgs e)
        {
            if (pageIndex < Pages.Count - 1)
            {
                // 验证当前页面是否完成必要配置
                if (!ValidateCurrentPage())
                {
                    return;
                }

                // 根据当前页面决定下一页
                int nextPageIndex = GetNextPageIndex();
                if (nextPageIndex != -1)
                {
                    pageIndex = nextPageIndex;
                }
                else
                {
                    pageIndex++;
                }

                UpdatePage();
            }
        }

        /// <summary>
        /// 上一步按钮点击事件
        /// </summary>
        private void button_上一步_Click(object sender, EventArgs e)
        {
            if (pageIndex > 0)
            {
                // 根据当前页面决定上一页
                int previousPageIndex = GetPreviousPageIndex();
                if (previousPageIndex != -1)
                {
                    pageIndex = previousPageIndex;
                }
                else
                {
                    pageIndex--;
                }
            }
            UpdatePage();
        }

        /// <summary>
        /// 跳过向导按钮点击事件
        /// </summary>
        private void button_跳过向导_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                _iLocalizationService.Get("SetupWizard.Msg.跳过向导"),
                _iLocalizationService.Get("SetupWizard.MsgTitle.跳过向导"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        /// <summary>
        /// 完成按钮点击事件
        /// </summary>
        private void button_完成_Click(object sender, EventArgs e)
        {
            // 同步所有数据到ManualSettingsService
            SyncDataToService();

            // 显示完成消息
            MessageBox.Show(
                _iLocalizationService.Get("SetupWizard.Msg.配置已完成"),
                _iLocalizationService.Get("SetupWizard.MsgTitle.配置完成"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // 设置DialogResult为OK，让Program.cs知道配置已完成
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 验证当前页面是否完成必要配置
        /// </summary>
        private bool ValidateCurrentPage()
        {
            // 页面3：自动设置坐标 - 需要选择进程
            if (pageIndex == 2 && IsUseDynamicCoordinates)
            {
                if (string.IsNullOrEmpty(TargetProcessName) || TargetProcessId == 0)
                {
                    MessageBox.Show(
                        _iLocalizationService.Get("SetupWizard.Msg.请先选择进程"),
                        _iLocalizationService.Get("SetupWizard.MsgTitle.提示"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }
            }

            // 页面4：手动设置坐标 - 需要设置所有坐标
            if (pageIndex == 3 && IsUseFixedCoordinates)
            {
                if (HeroNameScreenshotRectangle_1.Width <= 10 ||
                    RefreshStoreButtonRectangle.Width <= 10 ||
                    HighLightRectangle_1.Width <= 10)
                {
                    MessageBox.Show(
                        _iLocalizationService.Get("SetupWizard.Msg.请完成坐标设置"),
                        _iLocalizationService.Get("SetupWizard.MsgTitle.提示"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取下一页索引（根据用户选择跳过某些页面）
        /// </summary>
        private int GetNextPageIndex()
        {
            // 页面2：坐标设置模式
            if (pageIndex == 1)
            {
                if (IsUseDynamicCoordinates)
                {
                    return 2; // 跳转到页面3：自动设置坐标
                }
                else if (IsUseFixedCoordinates)
                {
                    return 3; // 跳转到页面4：手动设置坐标
                }
            }

            // 页面3：自动设置坐标 - 跳过页面4
            if (pageIndex == 2)
            {
                return 4; // 跳转到页面5：选择拿牌方式
            }

            // 页面4：手动设置坐标 - 跳转到页面5
            if (pageIndex == 3)
            {
                return 4; // 跳转到页面5：选择拿牌方式
            }

            // 页面7：选择OCR推理设备
            if (pageIndex == 6)
            {
                if (isUseCPUForInference)
                {
                    return 8; // 跳过GPU环境配置，直接到配置完成
                }
                else if (isUseGPUForInference)
                {
                    return 7; // 跳转到GPU环境配置
                }
            }

            return -1; // 使用默认的下一页
        }

        /// <summary>
        /// 获取上一页索引（根据用户选择跳过某些页面）
        /// </summary>
        private int GetPreviousPageIndex()
        {
            // 页面3：自动设置坐标
            if (pageIndex == 2)
            {
                return 1; // 返回到页面2：坐标设置模式
            }

            // 页面4：手动设置坐标
            if (pageIndex == 3)
            {
                return 1; // 返回到页面2：坐标设置模式
            }

            // 页面5：选择拿牌方式
            if (pageIndex == 4)
            {
                if (IsUseDynamicCoordinates)
                {
                    return 2; // 返回到页面3：自动设置坐标
                }
                else if (IsUseFixedCoordinates)
                {
                    return 3; // 返回到页面4：手动设置坐标
                }
            }

            // 页面8：GPU环境配置
            if (pageIndex == 7)
            {
                return 6; // 返回到页面7：选择OCR推理设备
            }

            // 页面9：配置完成
            if (pageIndex == 8)
            {
                if (isUseCPUForInference)
                {
                    return 6; // 返回到页面7：选择OCR推理设备
                }
                else if (isUseGPUForInference)
                {
                    return 7; // 返回到页面8：GPU环境配置
                }
            }

            return -1; // 使用默认的上一页
        }

        #endregion

        #region 圆角实现
        // GDI32 API - 用于创建圆角效果
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        // 圆角半径
        private const int CORNER_RADIUS = 16;

        /// <summary>
        /// 在窗口句柄创建后应用圆角效果
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // 应用 GDI Region 圆角效果（支持 Windows 10 和 Windows 11）
            ApplyRoundedCorners();
        }

        /// <summary>
        /// 应用 GDI Region 圆角效果
        /// </summary>
        private void ApplyRoundedCorners()
        {
            try
            {
                // 创建圆角矩形区域
                IntPtr region = CreateRoundRectRgn(0, 0, Width, Height, CORNER_RADIUS, CORNER_RADIUS);

                if (region != IntPtr.Zero)
                {
                    SetWindowRgn(Handle, region, true);
                    // 注意：SetWindowRgn 会接管 region 的所有权，不需要手动删除

                }
                else
                {

                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 窗口大小改变时重新应用圆角
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 调整大小时重新创建圆角区域
            if (Handle != IntPtr.Zero)
            {
                ApplyRoundedCorners();
            }
        }
        #endregion


       
    }
}
