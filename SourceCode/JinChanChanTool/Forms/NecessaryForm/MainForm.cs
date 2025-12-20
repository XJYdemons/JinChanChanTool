using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms;
using JinChanChanTool.Forms.DisplayUIForm;
using JinChanChanTool.Services;
using JinChanChanTool.Services.AutoSetCoordinates;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Services.LineupCrawling.Interface;
using JinChanChanTool.Services.RecommendedEquipment;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.LineUpCodeTools;
using JinChanChanTool.Tools.MouseTools;
using System.Diagnostics;
using System.Linq;
using static JinChanChanTool.DataClass.LineUp;
namespace JinChanChanTool
{
    public partial class MainForm : Form
    {
        #region 初始化相关
        /// <summary>
        /// 用户应用设置服务实例
        /// </summary>
        private readonly IManualSettingsService _iManualSettingsService;

        /// <summary>
        /// 自动应用设置服务实例
        /// </summary>
        private readonly IAutomaticSettingsService _iAutomaticSettingsService;

        /// <summary>
        /// 英雄数据服务实例
        /// </summary>
        private readonly IHeroDataService _iheroDataService;

        /// <summary>
        /// 装备数据服务实例
        /// </summary>
        private readonly IEquipmentService _iEquipmentService;

        /// <summary>
        /// OCR结果纠正服务实例
        /// </summary>
        private readonly ICorrectionService _iCorrectionService;

        /// <summary>
        /// 阵容数据服务实例
        /// </summary>
        private readonly ILineUpService _iLineUpService;

        /// <summary>
        /// 英雄装备推荐数据服务实例
        /// </summary>
        private readonly IHeroEquipmentDataService _iHeroEquipmentDataService;

        /// <summary>
        /// 推荐阵容数据服务实例
        /// </summary>
        private readonly IRecommendedLineUpService _iRecommendedLineUpService;

        /// <summary>
        /// UI构建服务实例
        /// </summary>
        private readonly UIBuilderService _uiBuilderService;

        /// <summary>
        /// 动态游戏数据服务实例
        /// </summary>
        private readonly IDynamicGameDataService _iDynamicGameDataService;

        /// <summary>
        /// 网络爬虫服务实例
        /// </summary>
        private readonly ICrawlingService _iCrawlingService;

        /// <summary>
        /// 阵容推荐爬虫服务实例
        /// </summary>
        private readonly ILineupCrawlingService _iLineupCrawlingService;

        /// <summary>
        /// 自动拿牌服务
        /// </summary>
        private CardService _cardService;

        /// <summary>
        /// 装备选择面板专用的自定义提示框（主窗口用）
        /// </summary>
        private EquipmentInformationToolTip _equipmentToolTip;

        /// <summary>
        /// 装备选择面板专用的自定义提示框（LineUpForm窗口用）
        /// </summary>
        private EquipmentInformationToolTip _lineUpFormEquipmentToolTip;

        // 这个字段将作为开关，记录了哪个赛季文件夹的名字才允许显示装备推荐       
        private string _seasonForEquipmentTooltip = "英雄联盟传奇";

        public MainForm(IManualSettingsService iManualSettingsService, IAutomaticSettingsService iAutomaticSettingsService, IHeroDataService iheroDataService, IEquipmentService iEquipmentService, ICorrectionService iCorrectionService, ILineUpService iLineUpService, IHeroEquipmentDataService iHeroEquipmentDataService, IRecommendedLineUpService iRecommendedLineUpService, IDynamicGameDataService iDynamicGameDataService, ICrawlingService iCrawlingService, ILineupCrawlingService iLineupCrawlingService)
        {
            InitializeComponent();
            #region 自定义标题栏
            // 自定义标题栏,带图标、带标题、最小化与关闭按钮。
            CustomTitleBar titleBar = new CustomTitleBar(this, this.LogicalToDeviceUnits(32), Image.FromFile(Path.Combine(Application.StartupPath, "Resources", "icon.ico")), "JinChanChanTool", CustomTitleBar.ButtonOptions.Close | CustomTitleBar.ButtonOptions.Minimize);
            this.Controls.Add(titleBar);
            #endregion

            #region 用户应用设置服务实例化并绑定事件
            _iManualSettingsService = iManualSettingsService;
            _iManualSettingsService.OnConfigSaved += OnConfigSaved;//绑定设置保存事件

            #endregion

            #region 自动应用设置服务实例化
            _iAutomaticSettingsService = iAutomaticSettingsService;
            #endregion

            #region 英雄数据服务实例化
            _iheroDataService = iheroDataService;
            #endregion

            #region 装备数据服务实例化
            _iEquipmentService = iEquipmentService;
            #endregion

            #region OCR结果纠正服务实例化
            _iCorrectionService = iCorrectionService;
            #endregion

            #region 阵容数据服务实例化
            _iLineUpService = iLineUpService;
            _iLineUpService.LineUpChanged += LineUpChanged;
            _iLineUpService.LineUpNameChanged += LineUpNameChanged;
            #endregion

            #region 英雄装备推荐数据服务实例化
            _iHeroEquipmentDataService = iHeroEquipmentDataService;
            #endregion

            #region 推荐阵容数据服务实例化
            _iRecommendedLineUpService = iRecommendedLineUpService;
            #endregion

            #region UI构建服务实例化并构建UI并绑定事件           
            _uiBuilderService = new UIBuilderService(_iheroDataService, _iManualSettingsService, this, tabControl_HeroSelector, flowLayoutPanel_SubLineUp, LineUpForm.Instance.flowLayoutPanel1, _iLineUpService.GetMaxSelect());
            _uiBuilderService.FirstBuilding();
            FirstBinding();
            #endregion

            #region 游戏窗口捕获服务对象实例化并绑定事件
            _windowInteractionService = new WindowInteractionService();
            _coordService = new CoordinateCalculationService(_windowInteractionService);
            _automationService = new AutomationService(_windowInteractionService, _coordService);
            #endregion

            #region 动态游戏数据服务实例化
            _iDynamicGameDataService = iDynamicGameDataService;
            #endregion

            #region 网络爬虫服务实例化
            _iCrawlingService = iCrawlingService;
            #endregion

            #region 阵容爬虫服务实例化
            _iLineupCrawlingService = iLineupCrawlingService;
            #endregion
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化赛季选择下拉框
            comboBox_Season.Items.Clear();// 清空赛季选择下拉框

            // 填充赛季选择下拉框
            comboBox_Season.Items.AddRange(_iheroDataService.GetFilePaths()
                                            .Select(p => Path.GetFileName(p))
                                            .ToArray());

            int selectedIndex = 0;

            if (!string.IsNullOrEmpty(_iAutomaticSettingsService.CurrentConfig.SelectedSeason))
            {
                for (int i = 0; i < comboBox_Season.Items.Count; i++)
                {
                    if (comboBox_Season.Items[i].ToString().Equals(_iAutomaticSettingsService.CurrentConfig.SelectedSeason, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }

            if (comboBox_Season.Items.Count > 0)
            {
                comboBox_Season.SelectedIndex = Math.Min(selectedIndex, comboBox_Season.Items.Count - 1);
            }

            comboBox_Season.SelectedIndexChanged += comboBox_HeroPool_SelectedIndexChanged;
            #endregion

            #region 初始化阵容选择下拉框

            LoadLineUpsToComboBox();

            #endregion

            #region 初始化阵容选择窗口
            LineUpForm.Instance.InitializeObject(_iLineUpService, _iAutomaticSettingsService, _iRecommendedLineUpService, _iheroDataService, _iEquipmentService);
            if (_iManualSettingsService.CurrentConfig.IsUseLineUpForm)
            {
                LineUpForm.Instance.Show();
            }
            #endregion

            #region 初始化英雄选择窗口
            SelectForm.Instance.InitializeObject(_iAutomaticSettingsService);
            if (_iManualSettingsService.CurrentConfig.IsUseSelectForm)
            {
                SelectForm.Instance.Show();
            }
            #endregion

            #region 加载阵容到UI            
            LoadLineUpToUI();//分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容    
            #endregion

            #region 自动拿牌服务对象实例化
            _cardService = new CardService(button_GetCard, button_Refresh, _iManualSettingsService, _iAutomaticSettingsService, _iCorrectionService, _iheroDataService, _iLineUpService);
            _cardService.isGetCardStatusChanged += OnIsGetCardChanged;
            _cardService.isRefreshStoreStatusChanged += OnAutoRefreshStatusChanged;
            #endregion

            #region 初始化状态显示窗口
            StatusOverlayForm.Instance.InitializeObject(_iAutomaticSettingsService, _cardService);
            if (_iManualSettingsService.CurrentConfig.IsUseStatusOverlayForm)
            {
                StatusOverlayForm.Instance.Show();
                UpdateOverlayStatus();
            }
            #endregion

            #region 初始化热键管理器并注册快捷键
            GlobalHotkeyTool.Initialize(this);
            RegisterHotKeys();//注册快捷键
            #endregion

            #region 初始化鼠标钩子并绑定事件
            MouseHookTool.Initialize();
            MouseHookTool.MouseLeftButtonDown += MouseHook_MouseLeftButtonDown;
            MouseHookTool.MouseLeftButtonUp += MouseHook_MouseLeftButtonUp;
            #endregion

            #region 更新英雄推荐装备
            // 检查是否启用自动更新推荐装备数据
            if (_iManualSettingsService.CurrentConfig.IsAutomaticUpdateEquipment)
            {
                await UpdateEquipmentsAsync();
            }
            #endregion
        }

        /// <summary>
        /// 当程序关闭时执行——>注销所有快捷键与鼠标钩子
        /// </summary>
        /// <param name="e"></param>      
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            GlobalHotkeyTool.Dispose();
            MouseHookTool.Dispose();
            base.OnFormClosing(e);
        }

        #endregion

        #region 用户应用设置数据对象服务保存通知事件
        /// <summary>
        /// 当设置被保存时触发,根据修改项提示用户操作。
        /// </summary>
        private void OnConfigSaved(object sender, ConfigChangedEventArgs e)
        {
            // 确保在UI线程执行
            if (InvokeRequired)
            {
                Invoke(new Action<object, ConfigChangedEventArgs>(OnConfigSaved), sender, e);
                return;
            }

            if (e.IsManualChange)
            {
                if (e.ChangedFields.Count == 0)
                {
                    MessageBox.Show("保存成功！本次保存无设置项发生修改。", "无设置项发生修改", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    MessageBox.Show("设置保存成功！", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            //输出被修改的项
            //Debug.WriteLine("-----------------------");
            //foreach (var f in e.ChangedFields)
            //{
            //    Debug.WriteLine(f.ToString());
            //}

            //如果变更的是快捷键，则重新注册快捷键
            if (e.ChangedFields.Contains("HotKey1") ||
                e.ChangedFields.Contains("HotKey2") ||
                e.ChangedFields.Contains("HotKey3") ||
                e.ChangedFields.Contains("HotKey4"))
            {
                RegisterHotKeys();
                UpdateOverlayStatus();
            }

            #region 如果变更的是窗口显示相关设置，则更新对应窗口的显示状态           
            if (e.ChangedFields.Contains("IsUseSelectForm"))
            {
                if (_iManualSettingsService.CurrentConfig.IsUseSelectForm)
                {
                    SelectForm.Instance.TopMost = false;
                    SelectForm.Instance.TopMost = true;
                    SelectForm.Instance.Show();
                }
                else
                {
                    SelectForm.Instance.Visible = false;
                }
            }

            if (e.ChangedFields.Contains("IsUseLineUpForm"))
            {
                if (_iManualSettingsService.CurrentConfig.IsUseLineUpForm)
                {
                    LineUpForm.Instance.TopMost = false;
                    LineUpForm.Instance.TopMost = true;
                    LineUpForm.Instance.Show();
                }
                else
                {
                    LineUpForm.Instance.Visible = false;
                }
            }

            if (e.ChangedFields.Contains("IsUseStatusOverlayForm"))
            {
                if (_iManualSettingsService.CurrentConfig.IsUseStatusOverlayForm)
                {
                    StatusOverlayForm.Instance.TopMost = false;
                    StatusOverlayForm.Instance.TopMost = true;
                    StatusOverlayForm.Instance.Show();
                    UpdateOverlayStatus();
                }
                else
                {
                    StatusOverlayForm.Instance.Visible = false;
                }
            }

            if (e.ChangedFields.Contains("IsUseOutputForm"))
            {
                if (_iManualSettingsService.CurrentConfig.IsUseOutputForm)
                {
                    OutputForm.Instance.TopMost = false;
                    OutputForm.Instance.TopMost = true;
                    OutputForm.Instance.Show();
                }
                else
                {
                    OutputForm.Instance.Visible = false;
                }
            }
            #endregion

            if (e.ChangedFields.Contains("TransparentHeroPictureBoxSize") ||
               e.ChangedFields.Contains("TransparentHeroPictureBoxHorizontalSpacing") ||
               e.ChangedFields.Contains("TransparentHeroPanelsVerticalSpacing"))
            {
                _uiBuilderService.ReBuild();
                UIReBinding();
            }

            //如果变更的是需要重启才能生效的设置，则询问用户是否重启应用
            if (e.ChangedFields.Contains("MaxLineUpCount") ||
                e.ChangedFields.Contains("IsUseCPUForInference") ||
                e.ChangedFields.Contains("IsUseGPUForInference"))
            {
                // 配置已保存，询问用户是否重启应用
                var result = MessageBox.Show(
                    "需要重启应用程序才能生效。是否立即重启？",
                    "重启确认",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // 重启应用程序
                    Application.Restart();
                    // 确保当前进程退出
                    Environment.Exit(0);
                }
            }
        }
        #endregion

        #region 快捷键注册
        /// <summary>
        /// 注册快捷键
        /// </summary>
        private void RegisterHotKeys()
        {
            GlobalHotkeyTool.UnregisterAll();//先注销所有热键
            string hotKey1 = _iManualSettingsService.CurrentConfig.HotKey1;
            string hotKey2 = _iManualSettingsService.CurrentConfig.HotKey2;
            string hotKey3 = _iManualSettingsService.CurrentConfig.HotKey3;
            string hotKey4 = _iManualSettingsService.CurrentConfig.HotKey4;
            if (GlobalHotkeyTool.IsKeyAvailable(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey1)))
            {
                GlobalHotkeyTool.Register(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey1), () => button_GetCard_Click(this, EventArgs.Empty));
            }
            if (GlobalHotkeyTool.IsKeyAvailable(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey2)))
            {
                GlobalHotkeyTool.Register(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey2), () => button_Refresh_Click(this, EventArgs.Empty));
            }
            if (GlobalHotkeyTool.IsKeyAvailable(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey3)))
            {
                GlobalHotkeyTool.Register(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey3), () => ShowMainForm());
            }
            if (GlobalHotkeyTool.IsKeyAvailable(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey4)))
            {
                GlobalHotkeyTool.Register(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey4), () =>
                {
                    _cardService.StartLoop();
                    _cardService.AutoRefreshOn();
                });
            }
            GlobalHotkeyTool.RegisterKeyUp(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey4), () =>
            {
                _cardService.StopLoop();
                _cardService.AutoRefreshOff();
            });
        }
        #endregion

        #region UI构建       
        /// <summary>
        /// 首次启动绑定UI事件
        /// </summary>
        private void FirstBinding()
        {
            #region 主窗口UI事件绑定
            //为主窗口每个奕子单选框绑定事件——选择状态改变时触发
            for (int i = 0; i < _uiBuilderService.MainForm_CheckBoxes.Count; i++)
            {
                _uiBuilderService.MainForm_CheckBoxes[i].CheckedChanged += CheckBoxCheckedChanged;
            }

            //为主窗口英雄头像框绑定交互事件
            for (int i = 0; i < _uiBuilderService.MainForm_HeroPictureBoxes.Count; i++)
            {

                _uiBuilderService.MainForm_HeroPictureBoxes[i].MouseEnter += HeroPictureBox_MouseEnter;
                _uiBuilderService.MainForm_HeroPictureBoxes[i].MouseLeave += HeroPictureBox_MouseLeave;
                _uiBuilderService.MainForm_HeroPictureBoxes[i].MouseDown += HeroPictureBox_MouseDown;
                _uiBuilderService.MainForm_HeroPictureBoxes[i].MouseUp += HeroPictureBox_MouseUp;
            }

            //为主窗口按职业选择英雄按钮添加点击事件
            for (int i = 0; i < _uiBuilderService.MainForm_ProfessionButtons.Count; i++)
            {
                _uiBuilderService.MainForm_ProfessionButtons[i].Click += ProfessionButtonClick;
            }

            //为主窗口按特质选择英雄按钮添加点击事件
            for (int i = 0; i < _uiBuilderService.MainForm_PeculiarityButtons.Count; i++)
            {
                _uiBuilderService.MainForm_PeculiarityButtons[i].Click += PeculiarityButtonClick;
            }


            // 初始化装备ToolTip（主窗口用）
            _equipmentToolTip = new EquipmentInformationToolTip(_iEquipmentService, this)
            {
                AutoPopDelay = 5000,
                InitialDelay = 300,
                ReshowDelay = 100,
                ShowAlways = true
            };

            // 初始化装备ToolTip（LineUpForm窗口用）
            _lineUpFormEquipmentToolTip = new EquipmentInformationToolTip(_iEquipmentService, this)
            {
                AutoPopDelay = 5000,
                InitialDelay = 300,
                ReshowDelay = 100,
                ShowAlways = true
            };

            for (int i = 0; i < _uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes.Count; i++)
            {
                _uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes[i].heroPictureBox.MouseUp += HeroAndEquipmentPictureBox_Hero_MouseUp;
                _uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox1.MouseUp += HeroAndEquipmentPictureBox_Equipment_MouseUp;
                _uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox2.MouseUp += HeroAndEquipmentPictureBox_Equipment_MouseUp;
                _uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox3.MouseUp += HeroAndEquipmentPictureBox_Equipment_MouseUp;

                // 为主窗口装备图片框设置ToolTip
                _equipmentToolTip.SetEquipment(_uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox1);
                _equipmentToolTip.SetEquipment(_uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox2);
                _equipmentToolTip.SetEquipment(_uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox3);
            }

            #endregion

            #region 半透明英雄选择窗口UI事件
            //为英雄选择窗口英雄头像框绑定交互事件
            for (int i = 0; i < _uiBuilderService.SelectForm_HeroPictureBoxes.Count; i++)
            {
                _uiBuilderService.SelectForm_HeroPictureBoxes[i].MouseUp += SelectFormPictureBoxes_Click;
                SelectForm.Instance.绑定拖动(_uiBuilderService.SelectForm_HeroPictureBoxes[i]);

            }


            #endregion

            #region 阵容窗口UI事件
            for (int i = 0; i < _uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes.Count; i++)
            {
                _uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].heroPictureBox.MouseUp += HeroAndEquipmentPictureBox_Hero_MouseUp;
                _uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox1.MouseUp += LineUpFormEquipmentPictureBox_MouseUp;
                _uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox2.MouseUp += LineUpFormEquipmentPictureBox_MouseUp;
                _uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox3.MouseUp += LineUpFormEquipmentPictureBox_MouseUp;
                LineUpForm.Instance.绑定拖动(_uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i]);
                LineUpForm.Instance.绑定拖动(_uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].heroPictureBox);
                LineUpForm.Instance.绑定拖动(_uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox1);
                LineUpForm.Instance.绑定拖动(_uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox2);
                LineUpForm.Instance.绑定拖动(_uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox3);
                // 为LineUpForm窗口装备图片框设置独立的ToolTip
                _lineUpFormEquipmentToolTip.SetEquipment(_uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox1);
                _lineUpFormEquipmentToolTip.SetEquipment(_uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox2);
                _lineUpFormEquipmentToolTip.SetEquipment(_uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].equipmentPictureBox3);
            }






            //为阵容下拉框绑定事件
            LineUpForm.Instance.GetLineUpSelectedComboBox().DropDownClosed += comboBox_LineUps_DropDownClosed;
            LineUpForm.Instance.GetLineUpSelectedComboBox().Leave += comboBox_LineUps_Leave;
            LineUpForm.Instance.GetLineUpSelectedComboBox().KeyDown += comboBox_LineUps_KeyDown;
            #endregion
        }

        /// <summary>
        /// 再次绑定UI事件(切换赛季或修改UI布局时调用)
        /// </summary>
        private void UIReBinding()
        {
            #region 主窗口UI事件绑定
            //为主窗口每个奕子单选框绑定事件——选择状态改变时触发
            for (int i = 0; i < _uiBuilderService.MainForm_CheckBoxes.Count; i++)
            {
                _uiBuilderService.MainForm_CheckBoxes[i].CheckedChanged += CheckBoxCheckedChanged;
            }

            //为主窗口英雄头像框绑定交互事件
            for (int i = 0; i < _uiBuilderService.MainForm_HeroPictureBoxes.Count; i++)
            {

                _uiBuilderService.MainForm_HeroPictureBoxes[i].MouseEnter += HeroPictureBox_MouseEnter;
                _uiBuilderService.MainForm_HeroPictureBoxes[i].MouseLeave += HeroPictureBox_MouseLeave;
                _uiBuilderService.MainForm_HeroPictureBoxes[i].MouseDown += HeroPictureBox_MouseDown;
                _uiBuilderService.MainForm_HeroPictureBoxes[i].MouseUp += HeroPictureBox_MouseUp;
            }

            //为主窗口按职业选择英雄按钮添加点击事件
            for (int i = 0; i < _uiBuilderService.MainForm_ProfessionButtons.Count; i++)
            {
                _uiBuilderService.MainForm_ProfessionButtons[i].Click += ProfessionButtonClick;
            }

            //为主窗口按特质选择英雄按钮添加点击事件
            for (int i = 0; i < _uiBuilderService.MainForm_PeculiarityButtons.Count; i++)
            {
                _uiBuilderService.MainForm_PeculiarityButtons[i].Click += PeculiarityButtonClick;
            }
            #endregion

            #region 半透明英雄选择窗口UI事件
            //为英雄选择窗口英雄头像框绑定交互事件
            for (int i = 0; i < _uiBuilderService.SelectForm_HeroPictureBoxes.Count; i++)
            {
                _uiBuilderService.SelectForm_HeroPictureBoxes[i].MouseUp += SelectFormPictureBoxes_Click;
                SelectForm.Instance.绑定拖动(_uiBuilderService.SelectForm_HeroPictureBoxes[i]);
            }
            #endregion          
        }
        #endregion

        #region 窗口、菜单项相关
        private SettingForm _settingFormInstance = null; // 保存窗口实例的字段
        private AboutForm _aboutFormInstance = null;

        /// <summary>
        /// 菜单项“设置”被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettingForm();
        }

        /// <summary>
        /// 打开新的设置窗口
        /// </summary>
        private void ShowSettingForm()
        {
            // 检查窗口是否已存在且未被释放
            if (_settingFormInstance == null || _settingFormInstance.IsDisposed)
            {
                _settingFormInstance = new SettingForm(_iManualSettingsService);
                _settingFormInstance.FormClosed += (s, args) => _settingFormInstance = null; // 窗口关闭时重置实例
                _settingFormInstance.TopMost = true;
                _settingFormInstance.Show();
            }
            else
            {
                // 如果窗口最小化则恢复正常状态
                if (_settingFormInstance.WindowState == FormWindowState.Minimized)
                {
                    _settingFormInstance.WindowState = FormWindowState.Normal;
                }

                // 激活窗口并置顶
                _settingFormInstance.BringToFront();
                _settingFormInstance.Activate();
            }
        }

        /// <summary>
        /// 菜单项“关于”被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutForm();
        }

        /// <summary>
        /// 打开新的关于窗口
        /// </summary>
        private void ShowAboutForm()
        {
            // 检查窗口是否已存在且未被释放
            if (_aboutFormInstance == null || _aboutFormInstance.IsDisposed)
            {
                _aboutFormInstance = new AboutForm();
                _aboutFormInstance.FormClosed += (s, args) => _aboutFormInstance = null; // 窗口关闭时重置实例
                _aboutFormInstance.TopMost = true;
                _aboutFormInstance.Show();
            }
            else
            {
                // 如果窗口最小化则恢复正常状态
                if (_aboutFormInstance.WindowState == FormWindowState.Minimized)
                {
                    _aboutFormInstance.WindowState = FormWindowState.Normal;
                }

                // 激活窗口并置顶
                _aboutFormInstance.BringToFront();
                _aboutFormInstance.Activate();
            }
        }

        /// <summary>
        /// 帮助-日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 运行日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!LogTool.OpenLogFile())
            {
                MessageBox.Show("日志文件不存在或无法打开！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 主窗口展示/隐藏 方法
        /// </summary>
        private void ShowMainForm()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // 如果窗口最小化，则还原
                this.WindowState = FormWindowState.Normal;
                this.TopMost = false;
                this.TopMost = true;
                this.Show();
            }
            else
            {
                // 如果窗口未最小化，则最小化
                this.WindowState = FormWindowState.Minimized;
            }
        }

        /// <summary>
        /// 更新状态显示窗口状态
        /// </summary>
        public void UpdateOverlayStatus()
        {
            StatusOverlayForm.Instance.UpdateStatus(_iManualSettingsService.CurrentConfig.HotKey1, _iManualSettingsService.CurrentConfig.HotKey2, _iManualSettingsService.CurrentConfig.HotKey3, _iManualSettingsService.CurrentConfig.HotKey4);
        }

        #endregion

        #region 拿牌相关
        /// <summary>
        /// 自动拿牌按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void button_GetCard_Click(object sender, EventArgs e)
        {
            _cardService.ToggleLoop();
        }

        /// <summary>
        /// 自动刷新商店按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button_Refresh_Click(object sender, EventArgs e)
        {
            _cardService.ToggleRefreshStore();
        }

        /// <summary>
        /// 自动拿牌服务对象自动拿牌功能变更通知事件
        /// </summary>
        /// <param name="isRunning"></param>
        private void OnIsGetCardChanged(bool isRunning)
        {
            // 确保在UI线程上更新
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(OnIsGetCardChanged), isRunning);
                return;
            }
            button_GetCard.Text = isRunning ? "停止" : "开启";
            comboBox_Season.Enabled = !isRunning;
        }

        /// <summary>
        /// 自动拿牌服务对象自动刷新商店功能变更通知事件
        /// </summary>
        /// <param name="isRunning"></param>
        private void OnAutoRefreshStatusChanged(bool isRunning)
        {
            // 确保在UI线程上更新
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(OnAutoRefreshStatusChanged), isRunning);
                return;
            }
            button_Refresh.Text = isRunning ? "停止" : "开启";
        }

        /// <summary>
        /// 鼠标左键按下事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseHook_MouseLeftButtonDown(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { MouseHook_MouseLeftButtonDown(sender, e); });
                return;
            }
            if (_iManualSettingsService.CurrentConfig.IsHighUserPriority)
            {
                _cardService.MouseLeftButtonDown();
            }
            else
            {
                _cardService.MouseLeftButtonUp();
            }
        }

        /// <summary>
        /// 鼠标左键抬起事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseHook_MouseLeftButtonUp(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { MouseHook_MouseLeftButtonUp(sender, e); });
                return;
            }
            _cardService.MouseLeftButtonUp();
        }
        #endregion

        #region 英雄选择与阵容相关           
        private bool waitForLoad = false;

        /// <summary>
        /// “清空”按钮_单击——>执行取消选择所有奕子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Clear_Click(object sender, EventArgs e)
        {
            _iLineUpService.ClearCurrentSubLineUp();
        }

        /// <summary>
        /// “保存阵容”按钮_单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Save_Click(object sender, EventArgs e)
        {
            if (_iLineUpService.Save())
            {
                MessageBox.Show("阵容已保存", "阵容已保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region 职业与特质按钮交互
        /// <summary>
        /// 按职业选择英雄按钮单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfessionButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Profession profession = button.Tag as Profession;
            SelectHerosFromProfession(profession);
        }

        /// <summary>
        /// 根据指定的Profession选择英雄
        /// </summary>
        /// <param name="profession"></param>
        private void SelectHerosFromProfession(Profession profession)
        {
            foreach (Hero hero in profession.Heros)
            {
                _iLineUpService.AddHero(hero.HeroName);
            }

        }
        /// <summary>
        /// 按特质选择英雄按钮单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeculiarityButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Peculiarity peculiarity = button.Tag as Peculiarity;
            SelectHerosFromPeculiarity(peculiarity);
        }

        /// <summary>
        /// 根据指定的Peculiarity选择英雄
        /// </summary>
        /// <param name="peculiarity"></param>
        private void SelectHerosFromPeculiarity(Peculiarity peculiarity)
        {
            List<LineUpUnit> units = new List<LineUpUnit>();
            foreach (Hero hero in peculiarity.Heros)
            {
                _iLineUpService.AddHero(hero.HeroName);
            }

        }
        #endregion

        #region 奕子复选框交互
        /// <summary>
        /// 当有奕子单选框选择状态发生改变时触发——>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (waitForLoad)
            {
                return;
            }
            CheckBox _checkBox = sender as CheckBox;
            string name = _checkBox.Tag as string;
            if (_checkBox.Checked)
            {
                if (!_iLineUpService.AddHero(name, new string[] { "", "", "" }))
                {
                    waitForLoad = true;
                    _checkBox.Checked = false;
                    waitForLoad = false;
                }
            }
            else
            {
                _iLineUpService.DeleteHero(name);
            }
        }
        #endregion

        #region 赛季选择下拉框交互
        /// <summary>
        /// 赛季下拉框选择项被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_HeroPool_SelectedIndexChanged(object sender, EventArgs e)
        {
            _iAutomaticSettingsService.CurrentConfig.SelectedSeason = comboBox_Season.Items[comboBox_Season.SelectedIndex].ToString();
            _iAutomaticSettingsService.Save();
            _iheroDataService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason);
            _iheroDataService.ReLoad();
            _iEquipmentService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason);
            _iEquipmentService.ReLoad();
            _iRecommendedLineUpService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason);
            _iRecommendedLineUpService.ReLoad();
            _iCorrectionService.SetCharDictionary(_iheroDataService.GetCharDictionary());
            if (!_iLineUpService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason))
            {
                Debug.WriteLine($"严重错误：阵容数据服务对象所读取的阵容中，未包含赛季名为“{_iAutomaticSettingsService.CurrentConfig.SelectedSeason}”的赛季！");
                LogTool.Log($"严重错误：阵容数据服务对象所读取的阵容中，未包含赛季名为“{_iAutomaticSettingsService.CurrentConfig.SelectedSeason}”的赛季！");
                OutputForm.Instance.WriteLineOutputMessage($"严重错误：阵容数据服务对象所读取的阵容中，未包含赛季名为“{_iAutomaticSettingsService.CurrentConfig.SelectedSeason}”的赛季！");
                MessageBox.Show($"严重错误：阵容数据服务对象所读取的阵容中，未包含赛季名为“{_iAutomaticSettingsService.CurrentConfig.SelectedSeason}”的赛季！", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _iLineUpService.ReLoad(_iheroDataService);
            _iAutomaticSettingsService.CurrentConfig.SelectedLineUpIndex = _iLineUpService.GetLineUpIndex();
            _iAutomaticSettingsService.Save();

            _uiBuilderService.ReBuild();
            UIReBinding();

            LoadLineUpsToComboBox();
            LoadLineUpToUI();
        }
        #endregion

        #region 阵容下拉框交互事件
        /// <summary>
        /// 当下拉框被关闭（即选择了新的或没选）时触发——>记录当前选择的下拉框，并从中读取阵容组合到单选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_LineUps_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex != -1)
            {
                _iLineUpService.SetLineUpIndex(comboBox.SelectedIndex);
                _iAutomaticSettingsService.CurrentConfig.SelectedLineUpIndex = _iLineUpService.GetLineUpIndex();
                _iAutomaticSettingsService.Save();
            }

            //从本地阵容文件读取数据到_lineupManager
            _iLineUpService.Load();
            //读取阵容名称到阵容下拉框，并将阵容下拉框当前选中项同步程序记录的值
            LoadLineUpsToComboBox();
            //分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容            
            LoadLineUpToUI();
        }

        /// <summary>
        /// 离开下拉框时触发——>保存阵容名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_LineUps_Leave(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.Items.Count > _iLineUpService.GetLineUpIndex())
            {
                comboBox.Items[_iLineUpService.GetLineUpIndex()] = comboBox.Text;
            }
            UpdataNameFromComboBoxToLineUps(comboBox);
        }

        /// <summary>
        /// 当下拉框按下任意键时触发——>若该键是回车键则 保存阵容名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_LineUps_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                if (comboBox.Items.Count > _iLineUpService.GetLineUpIndex())
                {
                    comboBox.Items[_iLineUpService.GetLineUpIndex()] = comboBox.Text;
                }
                UpdataNameFromComboBoxToLineUps(comboBox);
                this.ActiveControl = null;  // 将活动控件设置为null，下拉框失去焦点
            }

        }

        /// <summary>
        /// 从阵容下拉框更新阵容数据服务对象中的阵容名称
        /// </summary>
        private void UpdataNameFromComboBoxToLineUps(ComboBox comboBox)
        {
            for (int i = 0; i < _iLineUpService.GetMaxLineUpCount(); i++)
            {
                _iLineUpService.SetLineUpName(i, comboBox.Items[i].ToString());
            }
        }

        /// <summary>
        /// 从阵容数据服务对象读取阵容名称到下拉框
        /// </summary>
        private void LoadLineUpsToComboBox()
        {
            // 清空主窗口阵容选择下拉框
            comboBox_SelectedLineUp.Items.Clear();

            // 清空阵容窗口阵容选择下拉框
            LineUpForm.Instance.GetLineUpSelectedComboBox().Items.Clear();

            // 填充主窗口与阵容窗口的阵容选择下拉框
            List<LineUp> _lineUps = _iLineUpService.GetLineUps();
            foreach (LineUp lineUp in _lineUps)
            {
                comboBox_SelectedLineUp.Items.Add(lineUp.LineUpName);
                LineUpForm.Instance.GetLineUpSelectedComboBox().Items.Add(lineUp.LineUpName);
            }

            string lineUpName = _iLineUpService.GetCurrentLineUp().LineUpName;

            int mainFormLineUpSelectedComboBoxIndex = 0;
            if (!string.IsNullOrEmpty(lineUpName))
            {
                for (int i = 0; i < comboBox_SelectedLineUp.Items.Count; i++)
                {
                    if (comboBox_SelectedLineUp.Items[i].ToString().Equals(lineUpName, StringComparison.OrdinalIgnoreCase))
                    {
                        mainFormLineUpSelectedComboBoxIndex = i;
                        break;
                    }
                }
            }
            if (comboBox_SelectedLineUp.Items.Count > 0)
            {
                comboBox_SelectedLineUp.SelectedIndex = Math.Min(mainFormLineUpSelectedComboBoxIndex, comboBox_SelectedLineUp.Items.Count - 1);
            }

            int lineUpFormLineUpSelectedComboBoxIndex = 0;
            if (!string.IsNullOrEmpty(lineUpName))
            {
                for (int i = 0; i < LineUpForm.Instance.GetLineUpSelectedComboBox().Items.Count; i++)
                {
                    if (LineUpForm.Instance.GetLineUpSelectedComboBox().Items[i].ToString().Equals(lineUpName, StringComparison.OrdinalIgnoreCase))
                    {
                        lineUpFormLineUpSelectedComboBoxIndex = i;
                        break;
                    }
                }
            }
            if (LineUpForm.Instance.GetLineUpSelectedComboBox().Items.Count > 0)
            {
                LineUpForm.Instance.GetLineUpSelectedComboBox().SelectedIndex = Math.Min(lineUpFormLineUpSelectedComboBoxIndex, LineUpForm.Instance.GetLineUpSelectedComboBox().Items.Count - 1);
            }

        }
        #endregion

        #region 主窗口HeroAndEquipmentPictureBox交互      
        /// <summary>
        /// 从阵容中移除该英雄头像框所对应的英雄。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeroAndEquipmentPictureBox_Hero_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                HeroPictureBox pictureBox_ = sender as HeroPictureBox;
                Image image = pictureBox_.Image;
                if (image != null)
                {
                    string name = (pictureBox_.Tag as Hero).HeroName;
                    _iLineUpService.DeleteHero(name);
                }
            }

        }

        /// <summary>
        /// 召出装备设置对话框窗口(主窗口)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeroAndEquipmentPictureBox_Equipment_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sender is not HeroPictureBox pictureBox)
                    return;

                // 确定装备槽位索引，并通过父容器获取英雄信息
                int equipmentIndex = -1;
                Hero hero = null;

                foreach (var haep in _uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes)
                {
                    if (pictureBox == haep.equipmentPictureBox1)
                    {

                        equipmentIndex = 0;
                        hero = haep.heroPictureBox.Tag as Hero;
                        break;
                    }
                    else if (pictureBox == haep.equipmentPictureBox2)
                    {

                        equipmentIndex = 1;
                        hero = haep.heroPictureBox.Tag as Hero;
                        break;
                    }
                    else if (pictureBox == haep.equipmentPictureBox3)
                    {

                        equipmentIndex = 2;
                        hero = haep.heroPictureBox.Tag as Hero;
                        break;
                    }
                }

                if (equipmentIndex < 0 || hero == null)
                    return;

                // 打开装备选择窗口
                using (EquipmentForm equipmentForm = new EquipmentForm(_iEquipmentService, hero.HeroName, equipmentIndex))
                {
                    if (equipmentForm.ShowDialog(this) == DialogResult.OK)
                    {
                        // 用户选择了装备，更新阵容数据
                        string selectedEquipment = equipmentForm.SelectedEquipmentName;
                        _iLineUpService.SetHeroEquipment(hero.HeroName, equipmentIndex, selectedEquipment);
                    }
                }
            }

        }

        /// <summary>
        /// 召出装备设置对话框窗口(LineUpForm窗口)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineUpFormEquipmentPictureBox_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sender is not HeroPictureBox pictureBox)
                    return;

                // 确定装备槽位索引，并通过父容器获取英雄信息
                int equipmentIndex = -1;
                Hero hero = null;

                foreach (var haep in _uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes)
                {
                    if (pictureBox == haep.equipmentPictureBox1)
                    {

                        equipmentIndex = 0;
                        hero = haep.heroPictureBox.Tag as Hero;
                        break;
                    }
                    else if (pictureBox == haep.equipmentPictureBox2)
                    {

                        equipmentIndex = 1;
                        hero = haep.heroPictureBox.Tag as Hero;
                        break;
                    }
                    else if (pictureBox == haep.equipmentPictureBox3)
                    {

                        equipmentIndex = 2;
                        hero = haep.heroPictureBox.Tag as Hero;
                        break;
                    }
                }

                if (equipmentIndex < 0 || hero == null)
                    return;

                // 打开装备选择窗口
                using (EquipmentForm equipmentForm = new EquipmentForm(_iEquipmentService, hero.HeroName, equipmentIndex))
                {
                    if (equipmentForm.ShowDialog(LineUpForm.Instance) == DialogResult.OK)
                    {
                        // 用户选择了装备，更新阵容数据
                        string selectedEquipment = equipmentForm.SelectedEquipmentName;
                        _iLineUpService.SetHeroEquipment(hero.HeroName, equipmentIndex, selectedEquipment);
                    }
                }
            }

        }
        #endregion

        #region 英雄头像框交互事件
        /// <summary>
        /// 英雄头像框鼠标进入——>尺寸变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeroPictureBox_MouseEnter(object sender, EventArgs e)
        {
            HeroPictureBox clickedBox = sender as HeroPictureBox;
            Size size = new Size(_uiBuilderService.GetHeroPictureBoxSize().Width + 1, _uiBuilderService.GetHeroPictureBoxSize().Height + 1);
            clickedBox.Size = this.LogicalToDeviceUnits(size);

            // 计时器逻辑
            toolTipTimer.Stop(); // 停止上一个计时
            if (_activeToolTip != null) // 立刻销毁上一个残留的ToolTip
            {
                _activeToolTip.Dispose();
                _activeToolTip = null;
            }
            _hoveredHeroPictureBox = clickedBox; // 记录当前悬停的PictureBox
            toolTipTimer.Start(); // 启动新的计时            
        }

        /// <summary>
        /// 英雄头像框鼠标离开——>尺寸还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeroPictureBox_MouseLeave(object sender, EventArgs e)
        {
            HeroPictureBox clickedBox = sender as HeroPictureBox;
            Size size = new Size(_uiBuilderService.GetHeroPictureBoxSize().Width, _uiBuilderService.GetHeroPictureBoxSize().Height);
            clickedBox.Size = this.LogicalToDeviceUnits(size);

            // 停止计时并销毁ToolTip
            toolTipTimer.Stop();
            _hoveredHeroPictureBox = null;

            if (_activeToolTip != null)
            {
                _activeToolTip.Dispose();
                _activeToolTip = null;
            }
        }

        /// <summary>
        /// 英雄头像框鼠标释放——>尺寸还原，通过图片溯源到hero对象，取消选中该对象。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeroPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                HeroPictureBox clickedBox = sender as HeroPictureBox;
                Size size = new Size(_uiBuilderService.GetHeroPictureBoxSize().Width + 1, _uiBuilderService.GetHeroPictureBoxSize().Height + 1);
                clickedBox.Size = this.LogicalToDeviceUnits(size);
                string name = clickedBox.Tag as string;
                _iLineUpService.AddAndDeleteHero(name, new string[] { "", "", "" });
            }
        }

        /// <summary>
        /// 英雄头像框鼠标按下——>尺寸变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeroPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                HeroPictureBox clickedBox = sender as HeroPictureBox;
                Size size = new Size(_uiBuilderService.GetHeroPictureBoxSize().Width + 2, _uiBuilderService.GetHeroPictureBoxSize().Height + 2);
                clickedBox.Size = this.LogicalToDeviceUnits(size);
            }
        }
        #endregion

        #region 英雄选择面板英雄头像框交互事件
        /// <summary>
        /// 透明面板英雄头像框单击——>通过图片溯源到hero对象，添加或删除该对象。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFormPictureBoxes_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                HeroPictureBox clickedBox = sender as HeroPictureBox;
                string name = clickedBox.Tag as string;
                _iLineUpService.AddAndDeleteHero(name, new string[] { "", "", "" });
            }
        }
        #endregion

        #region 变阵按钮交互
        /// <summary>
        /// 变阵按钮1按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_变阵1_Click(object sender, EventArgs e)
        {

            _iLineUpService.SetSubLineUpIndex(0);
        }

        /// <summary>
        /// 变阵按钮1鼠标右键抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_变阵1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RenameSubLineUp(0);
            }
        }

        /// <summary>
        /// 变阵按钮2按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_变阵2_Click(object sender, EventArgs e)
        {
            _iLineUpService.SetSubLineUpIndex(1);
        }

        /// <summary>
        /// 变阵按钮2鼠标右键抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_变阵2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RenameSubLineUp(1);
            }

        }

        /// <summary>
        /// 变阵按钮3按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_变阵3_Click(object sender, EventArgs e)
        {
            _iLineUpService.SetSubLineUpIndex(2);
        }

        /// <summary>
        /// 变阵按钮3鼠标右键抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_变阵3_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RenameSubLineUp(2);
            }

        }

        /// <summary>
        /// 重命名当前阵容变阵名
        /// </summary>
        /// <param name="index"></param>
        private void RenameSubLineUp(int index)
        {
            LineUp currentLineUp = _iLineUpService.GetCurrentLineUp();
            if (index < 0 || index >= currentLineUp.SubLineUps.Length)
            {
                return;
            }

            string? newName = PromptForSubLineUpName("修改变阵名称", currentLineUp.SubLineUps[index].SubLineUpName);
            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }
            if (newName.Length > 4)
            {
                MessageBox.Show("变阵名称不能超过4个字符!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            _iLineUpService.SetSubLineUpName(index, newName.Trim());
        }

        /// <summary>
        /// 生成对话框窗口，提供变阵重命名的UI。
        /// </summary>
        /// <param name="title"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private string? PromptForSubLineUpName(string title, string defaultValue)
        {
            using Form prompt = new Form();
            prompt.TopMost = true;
            prompt.Text = title;
            prompt.StartPosition = FormStartPosition.CenterParent;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.MinimizeBox = false;
            prompt.MaximizeBox = false;
            prompt.ClientSize = new Size(prompt.LogicalToDeviceUnits(280), prompt.LogicalToDeviceUnits(120));
            prompt.FormBorderStyle = FormBorderStyle.None;
            prompt.AutoSize = true;
            prompt.AutoScaleMode = AutoScaleMode.Dpi;
            prompt.BackColor = Color.White;
            // 自定义标题栏,带图标、带标题、最小化与关闭按钮。
            CustomTitleBar titleBar = new CustomTitleBar(prompt, prompt.LogicalToDeviceUnits(32), null, title, CustomTitleBar.ButtonOptions.Close);
            prompt.Controls.Add(titleBar);

            Label textLabel = new Label() { Left = prompt.LogicalToDeviceUnits(10), Top = prompt.LogicalToDeviceUnits(42), Text = "变阵名称：", AutoSize = true };
            TextBox inputBox = new TextBox() { Left = prompt.LogicalToDeviceUnits(10), Top = prompt.LogicalToDeviceUnits(67), Width = prompt.LogicalToDeviceUnits(260), Text = defaultValue };
            Button confirmation = new Button() { Text = "确定", Left = prompt.LogicalToDeviceUnits(110), Width = prompt.LogicalToDeviceUnits(75), Height = prompt.LogicalToDeviceUnits(25), Top = prompt.LogicalToDeviceUnits(102), DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat };
            confirmation.FlatAppearance.BorderColor = Color.Gray;
            Button cancel = new Button() { Text = "取消", Left = prompt.LogicalToDeviceUnits(195), Width = prompt.LogicalToDeviceUnits(75), Height = prompt.LogicalToDeviceUnits(25), Top = prompt.LogicalToDeviceUnits(102), DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat };
            cancel.FlatAppearance.BorderColor = Color.Gray;
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog(this) == DialogResult.OK ? inputBox.Text : null;
        }
        #endregion

        #region UI更新逻辑
        /// <summary>
        /// 加载当前选中变阵到UI
        /// CheckBox、主窗口HeroAndEquipmentPictureBox、LineUpForm的HeroAndEquipmentPictureBox、SelectForm的HeroPictureBox、变阵按钮
        /// </summary>
        public void LoadLineUpToUI()
        {
            waitForLoad = true;

            // 获取数据
            SubLineUp currentSubLineUp = _iLineUpService.GetCurrentSubLineUp();
            List<LineUpUnit> units = currentSubLineUp.LineUpUnits;
            LineUp currentLineUp = _iLineUpService.GetCurrentLineUp();
            int subLineUpIndex = _iLineUpService.GetSubLineUpIndex();

            // 获取所有英雄和装备数据
            var heroDataList = units.Select(unit => new
            {
                Hero = _iheroDataService.GetHeroFromName(unit.HeroName),
                Equipment0 = _iEquipmentService.GetEquipmentFromName(unit.EquipmentNames[0]),
                Equipment1 = _iEquipmentService.GetEquipmentFromName(unit.EquipmentNames[1]),
                Equipment2 = _iEquipmentService.GetEquipmentFromName(unit.EquipmentNames[2])
            }).ToList();

            // 清空UI
            ClearCheckBox();
            ClearHeroAndEquipmentPictureBoxes();

            // 更新UI
            UpdateCheckBoxes(units);
            UpdateHeroAndEquipmentPictureBoxes(heroDataList);
            UpdateSelectFormHeroPictureBoxes(currentSubLineUp);
            UpdateSubLineUpButtons(subLineUpIndex, currentLineUp);

            waitForLoad = false;

            // 调试输出
            Debug.WriteLine("当前阵容：" + string.Join("   ", heroDataList.Where(h => h.Hero != null).Select(h => h.Hero.HeroName)));
        }

        /// <summary>
        /// 使奕子复选框全部取消勾选
        /// </summary>
        private void ClearCheckBox()
        {
            for (int i = 0; i < _uiBuilderService.MainForm_CheckBoxes.Count; i++)
            {
                _uiBuilderService.MainForm_CheckBoxes[i].Checked = false;
            }
        }

        /// <summary>
        /// 清空主窗口和LineUpForm的HeroAndEquipmentPictureBox
        /// </summary>
        private void ClearHeroAndEquipmentPictureBoxes()
        {
            foreach (var haep in _uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes)
            {
                haep.Clear();
            }
            foreach (var haep in _uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes)
            {
                haep.Clear();
            }
        }

        /// <summary>
        /// 根据阵容数据更新CheckBox选中状态
        /// </summary>
        private void UpdateCheckBoxes(List<LineUpUnit> units)
        {
            foreach (LineUpUnit unit in units)
            {
                CheckBox checkBox = _uiBuilderService.GetCheckBoxFromName(unit.HeroName);
                if (checkBox != null)
                {
                    checkBox.Checked = true;
                }
            }
        }

        /// <summary>
        /// 更新主窗口和LineUpForm的HeroAndEquipmentPictureBox
        /// </summary>
        private void UpdateHeroAndEquipmentPictureBoxes(dynamic heroDataList)
        {
            for (int i = 0; i < heroDataList.Count; i++)
            {
                var data = heroDataList[i];
                if (data.Hero != null)
                {
                    _uiBuilderService.MainForm_HeroAndEquipmentPictureBoxes[i].SetHero(
                        data.Hero, _uiBuilderService, data.Equipment0, data.Equipment1, data.Equipment2);
                    _uiBuilderService.LineUpForm_HeroAndEquipmentPictureBoxes[i].SetHero(
                        data.Hero, _uiBuilderService, data.Equipment0, data.Equipment1, data.Equipment2);
                }
            }
        }

        /// <summary>
        /// 更新SelectForm的HeroPictureBox选中状态
        /// </summary>
        private void UpdateSelectFormHeroPictureBoxes(SubLineUp currentSubLineUp)
        {
            foreach (HeroPictureBox heroPictureBox in _uiBuilderService.SelectForm_HeroPictureBoxes)
            {
                heroPictureBox.IsSelected = currentSubLineUp.Contains(heroPictureBox.Tag as string);
            }
        }

        /// <summary>
        /// 更新变阵按钮的选中状态和名称
        /// </summary>
        private void UpdateSubLineUpButtons(int selectedIndex, LineUp currentLineUp)
        {
            Color selectedColor = Color.FromArgb(130, 189, 39);
            Color normalColor = Color.White;

            button_变阵1.BackColor = selectedIndex == 0 ? selectedColor : normalColor;
            button_变阵2.BackColor = selectedIndex == 1 ? selectedColor : normalColor;
            button_变阵3.BackColor = selectedIndex == 2 ? selectedColor : normalColor;

            button_变阵1.Text = currentLineUp.SubLineUps[0].SubLineUpName;
            button_变阵2.Text = currentLineUp.SubLineUps[1].SubLineUpName;
            button_变阵3.Text = currentLineUp.SubLineUps[2].SubLineUpName;

            LineUpForm.Instance.UpdateSubLineUpButtons(selectedIndex);
        }

        #endregion

        /// <summary>
        /// 当iLineUpService中的当前子阵容被修改时触发——>重新加载当前子阵容到UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineUpChanged(object sender, EventArgs e)
        {
            LoadLineUpToUI();
        }

        /// <summary>
        /// 当iLineUpService中的阵容被保存时触发——>重新从本地阵容文件读取数据到_lineupManager，并刷新阵容下拉框和UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineUpNameChanged(object sender, EventArgs e)
        {
            //从本地阵容文件读取数据到_lineupManager
            _iLineUpService.Load();
            //读取阵容名称到阵容下拉框，并将阵容下拉框当前选中项同步程序记录的值
            LoadLineUpsToComboBox();
            //分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容
            LoadLineUpToUI();
        }
        #endregion

        #region 解析阵容码
        /// <summary>
        /// 阵容解析按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ParseLineUp_Click(object sender, EventArgs e)
        {
            string lineupCode = textBox_LineUpCode.Text.Trim();
            if (string.IsNullOrEmpty(lineupCode))
            {
                MessageBox.Show("请输入阵容代码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                List<string> heroNames;
                heroNames = LineUpParser.ParseCode(lineupCode);
                // 统一处理结果
                if (heroNames != null && heroNames.Count > 0)
                {
                    _iLineUpService.ClearCurrentSubLineUp();
                    List<LineUpUnit> units = new List<LineUpUnit>();
                    foreach (string heroName in heroNames)
                    {
                        _iLineUpService.AddHero(heroName);
                    }

                    MessageBox.Show($"成功解析出 {heroNames.Count} 位英雄并已自动勾选！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("未能解析出任何英雄。请检查阵容码是否正确或完整。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // 捕获并显示任何在解析过程中可能发生的错误提示
                MessageBox.Show($"解析失败！请确保阵容码正确无误。\n\n详细错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 阵容码文本框进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_LineUpCode_Enter(object sender, EventArgs e)
        {
            textBox_LineUpCode.Text = "";
        }

        /// <summary>
        /// 阵容码文本块离开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_LineUpCode_Leave(object sender, EventArgs e)
        {
            if (textBox_LineUpCode.Text == "")
            {
                textBox_LineUpCode.Text = "请在此处粘贴阵容代码";
            }
        }

        /// <summary>
        /// 生成阵容码按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_生成阵容码_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> selectedHeroNames = new List<string>();
                foreach (LineUpUnit unit in _iLineUpService.GetCurrentSubLineUp().LineUpUnits)
                {
                    string name = unit.HeroName;
                    selectedHeroNames.Add(name);
                }
                if (selectedHeroNames == null || selectedHeroNames.Count == 0)
                {
                    MessageBox.Show("请先选择英雄！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 检查英雄数量，如果超过10个给出提示
                string countMessage = "";
                if (selectedHeroNames.Count > 10)
                {
                    countMessage = $"\n注意：您选择了{selectedHeroNames.Count}个英雄，阵容码最多支持10个，已自动截取前10个英雄。";
                }

                // 生成阵容码
                string lineupCode = LineUpParser.GenerateCode(selectedHeroNames);

                // 将生成的阵容码显示在文本框中
                textBox_LineUpCode.Text = lineupCode;

                // 自动复制到剪贴板
                Clipboard.SetText(lineupCode);

                int usedCount = Math.Min(selectedHeroNames.Count, 10);
                MessageBox.Show($"已导出阵容码！\n已导出的英雄数量: {usedCount}{countMessage}\n阵容码已复制到剪贴板。",
                                "成功",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出阵容码失败！\n错误信息: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 更新装备数据       

        /// <summary>
        /// 更新推荐装备数据异步方法
        /// </summary>
        /// <returns></returns>
        private async Task UpdateEquipmentsAsync()
        {
            TimeSpan timeDifference = DateTime.Now - _iAutomaticSettingsService.CurrentConfig.EquipmentLastUpdateTime;
            // 如果上次更新距离现在的时间小于配置的间隔时间，则跳过更新
            if (timeDifference.TotalHours <= _iManualSettingsService.CurrentConfig.UpdateEquipmentInterval)
            {
                //MessageBox.Show($"装备数据在 {_iAutoConfigService.CurrentConfig.LastUpdateTime:yyyy年MM月dd日HH:mm:ss} 刚刚更新过，已是最新，无需重复更新。",
                //                "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // 直接中断，不执行任何网络请求
            }

            // 询问用户是否进行更新         
            var r = MessageBox.Show(
                $"最后一次更新推荐装备数据的时间是：{_iAutomaticSettingsService.CurrentConfig.EquipmentLastUpdateTime:yyyy年MM月dd日HH:mm:ss}，距现在已经过去了{(int)((DateTime.Now - _iAutomaticSettingsService.CurrentConfig.EquipmentLastUpdateTime).TotalHours)}个小时,是否要获取最新的推荐装备数据？",
                "是否获取最新的推荐装备数据",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (r != DialogResult.Yes)
            {
                return;
            }

            // 创建进度条窗口，用于向用户反馈进度 
            var progressForm = new ProgressForm();
            IProgress<Tuple<int, string>> progress = new Progress<Tuple<int, string>>(update =>
            {
                progressForm.UpdateProgress(update.Item1, update.Item2);
            });

            try
            {

                progressForm.Show(this); // 显示进度窗口

                // 因为是全局单例，内部有 _isInitialized 标记，若已初始化则会瞬间跳过
                progress.Report(Tuple.Create(0, "正在初始化，获取最新游戏数据..."));
                await _iDynamicGameDataService.InitializeAsync();
                progress.Report(Tuple.Create(5, "基础数据获取成功！"));

                // 开始后台网络爬取（使用注入的爬虫服务，内部接入 HttpProvider 全局连接池）
                List<HeroEquipment> crawledData = await _iCrawlingService.GetEquipmentsAsync(progress);

                bool updateSuccess = false;
                if (crawledData != null && crawledData.Any())
                {
                    // 将爬取到的新数据，传递给我们注入的“数据中心”服务进行更新和保存 
                    _iHeroEquipmentDataService.UpdateDataFromCrawling(crawledData);
                    updateSuccess = true;
                }
                else
                {
                    MessageBox.Show("未能从网络获取到任何有效的装备数据，请检查网络连接或稍后再试。", "更新失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // 提示重启 
                if (updateSuccess)
                {
                    // 重新加载数据中心的内存
                    _iHeroEquipmentDataService.ReLoad();

                    _iAutomaticSettingsService.CurrentConfig.EquipmentLastUpdateTime = DateTime.Now;

                    _iAutomaticSettingsService.Save();
                    // 提示用户重启以确保所有状态都刷新
                    DialogResult result = MessageBox.Show(this,
                        "装备数据更新成功！\n\n为了确保所有组件都使用最新数据，建议重启程序。\n点击“确定”立即重启。",
                        "更新完成",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.OK)
                    {
                        // 重启应用程序
                        Application.Restart();
                        // 确保当前进程退出
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                // 捕获任何在流程中未被处理的异常 (现在也能捕获到 InitializeAsync 的网络错误)
                MessageBox.Show($"更新过程中发生未知错误: {ex.Message}", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 无论成功还是失败，都确保关闭进度窗口并恢复按钮 
                progressForm.Close();
            }
        }

        #endregion

        #region 装备展示
        private HeroPictureBox _hoveredHeroPictureBox = null;//当前悬停的英雄头像框

        private ToolTip _activeToolTip = null; //用于持有当前活动的ToolTip实例

        /// <summary>
        /// 定时器触发——>显示装备推荐ToolTip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // 计时器只触发一次

            if (_hoveredHeroPictureBox != null)
            {

                // 从 PictureBox 的 Tag 属性中获取 英雄名称
                string name = _hoveredHeroPictureBox.Tag as string;

                if (name != null)
                {
                    // 修复显示错误赛季装备推荐，先通过调用方法，获取到路径数组和当前索引
                    string[] currentPaths = _iheroDataService.GetFilePaths();
                    int currentIndex = _iheroDataService.GetFilePathsIndex();

                    // 进行有效性检查，防止数组越界
                    if (currentPaths == null || currentIndex < 0 || currentIndex >= currentPaths.Length)
                    {
                        return; // 如果路径或索引无效，直接中止
                    }

                    //使用获取到的变量来构建路径并获取赛季名
                    string currentSeasonName = new DirectoryInfo(currentPaths[currentIndex]).Name;

                    // 检查当前赛季是否是“允许显示”的赛季
                    if (currentSeasonName != _seasonForEquipmentTooltip)
                    {
                        return; // 如果不是，直接中止，不显示任何ToolTip
                    }

                    // 使用英雄名称去查找对应的 HeroEquipment 对象
                    HeroEquipment currentHeroEquipment = _iHeroEquipmentDataService.GetHeroEquipmentFromName(name);

                    if (currentHeroEquipment != null)
                    {
                        List<Image> images = _iHeroEquipmentDataService.GetImagesFromHeroEquipment(currentHeroEquipment);
                        // 确保有图片可供显示
                        if (images != null && images.Any())
                        {
                            // 创建并显示新的 ToolTip
                            EquipmentToolTip newToolTip = new EquipmentToolTip(images);
                            _activeToolTip = newToolTip; // 保存对新ToolTip的引用

                            // 计算显示位置 (在鼠标指针右下方)
                            Point toolTipPosition = _hoveredHeroPictureBox.PointToClient(Cursor.Position);
                            newToolTip.Show(" ", _hoveredHeroPictureBox, toolTipPosition.X + 15, toolTipPosition.Y + 15, 5000);
                        }
                    }
                }
            }
        }
        #endregion

        #region 更新动态坐标 
        private readonly WindowInteractionService _windowInteractionService;// 用于与窗口进行交互的服务
        private readonly CoordinateCalculationService _coordService;// 用于计算坐标的服务
        private readonly AutomationService _automationService;// 用于自动化操作的服务
        private bool _multiProcessWarningShown = false;// 用于防止多进程冲突警告重复弹出

        /// <summary>
        /// 定时器触发——>更新动态坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_UpdateCoordinates_Tick(object sender, EventArgs e)
        {
            // 1. 检查是否处于自动模式
            if (!_iManualSettingsService.CurrentConfig.IsUseDynamicCoordinates)
            {
                _automationService.SetTargetProcess(null);
                return;
            }

            Process targetProcess = null;
            bool processFound = false;
            Process[] processesByName = null; // 用于按名称查找的结果

            // 2.【ID】使用Process.GetProcesses()获取当前的系统进程快照，然后用irstOrDefault(p => p.Id == targetId)在这个快照中查找ID匹配的进程
            //    如果找到了，它会返回那个 Process 对象；如果没找到（因为进程已经关闭），它只会返回 null，而不会抛出任何异常。
            int targetId = _iManualSettingsService.CurrentConfig.TargetProcessId;
            if (targetId > 0)
            {
                // 获取当前所有进程的快照，然后从中查找
                Process pById = Process.GetProcesses().FirstOrDefault(p => p.Id == targetId);

                // 检查是否找到了进程，并且进程名是否匹配
                if (pById != null && pById.ProcessName.Equals(_iManualSettingsService.CurrentConfig.TargetProcessName, StringComparison.OrdinalIgnoreCase))
                {
                    targetProcess = pById;
                    processFound = true;
                }
                else
                {
                    // 如果没找到，或进程名不匹配（PID被重用），则清除ID
                    _iManualSettingsService.CurrentConfig.TargetProcessId = 0;
                }
            }

            // 3.【名称】如果按ID查找失败，则回退到按名称查找
            if (!processFound)
            {
                string targetName = _iManualSettingsService.CurrentConfig.TargetProcessName;
                if (string.IsNullOrEmpty(targetName))
                {
                    _automationService.SetTargetProcess(null);
                    return;
                }

                processesByName = Process.GetProcessesByName(targetName);

                if (processesByName.Length == 1)
                {
                    targetProcess = processesByName[0];
                    _iManualSettingsService.CurrentConfig.TargetProcessId = targetProcess.Id;
                }
                else if (processesByName.Length > 1)
                {
                    _automationService.SetTargetProcess(null);
                    if (!_multiProcessWarningShown)
                    {
                        _multiProcessWarningShown = true;
                        this.Invoke((Action)(() =>
                        {
                            MessageBox.Show(this,
                                $"检测到多个名为 '{targetName}' 的进程。\n\n自动坐标计算已暂停，请通过“设置”->“选择进程”来精确指定一个。",
                                "多进程冲突",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }));
                    }
                    return;
                }
                else
                {
                    targetProcess = null;
                }
            }

            // 4. 将最终确定的目标（或null）交给 AutomationService
            _automationService.SetTargetProcess(targetProcess);

            // 5. 重置多进程警告标志
            if (processesByName == null || processesByName.Length <= 1)
            {
                _multiProcessWarningShown = false;
            }

            // 6. 如果成功锁定，则更新 AppConfig
            if (_automationService.IsGameDetected)
            {
                var rectSlot1 = _automationService.GetTargetRectangle(UiElement.CardSlot1_Name);
                var rectSlot2 = _automationService.GetTargetRectangle(UiElement.CardSlot2_Name);
                var rectSlot3 = _automationService.GetTargetRectangle(UiElement.CardSlot3_Name);
                var rectSlot4 = _automationService.GetTargetRectangle(UiElement.CardSlot4_Name);
                var rectSlot5 = _automationService.GetTargetRectangle(UiElement.CardSlot5_Name);
                var rectRefresh = _automationService.GetTargetRectangle(UiElement.RefreshButton);

                if (rectSlot1.HasValue && rectSlot1.Value.Width > 0 && rectSlot1.Value.Height > 0)
                {
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X1 = rectSlot1.Value.X;
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_Y = rectSlot1.Value.Y;
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotWidth = rectSlot1.Value.Width;
                    _iAutomaticSettingsService.CurrentConfig.Height_CardScreenshot = rectSlot1.Value.Height;
                }
                if (rectSlot2.HasValue && rectSlot2.Value.Width > 0) _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X2 = rectSlot2.Value.X;
                if (rectSlot3.HasValue && rectSlot3.Value.Width > 0) _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X3 = rectSlot3.Value.X;
                if (rectSlot4.HasValue && rectSlot4.Value.Width > 0) _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X4 = rectSlot4.Value.X;
                if (rectSlot5.HasValue && rectSlot5.Value.Width > 0) _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X5 = rectSlot5.Value.X;

                if (rectRefresh.HasValue && rectRefresh.Value.Width > 0)
                {
                    _iAutomaticSettingsService.CurrentConfig.RefreshStoreButtonCoordinates_X = rectRefresh.Value.X + rectRefresh.Value.Width / 2;
                    _iAutomaticSettingsService.CurrentConfig.RefreshStoreButtonCoordinates_Y = rectRefresh.Value.Y + rectRefresh.Value.Height / 2;
                }
            }
        }

        #endregion

        #region 更新阵容数据
        /// <summary>
        /// 点击更新阵容推荐菜单项时的处理逻辑
        /// </summary>
        private async void 更新阵容推荐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 暂时禁用菜单项，防止用户重复点击
                更新阵容推荐ToolStripMenuItem.Enabled = false;

                // 调用异步更新逻辑
                await UpdateRecommendedLineUpsAsync();
            }
            finally
            {
                // 恢复菜单项可用状态
                更新阵容推荐ToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// 业务逻辑：从网络获取最新阵容数据并保存，完成后直接刷新不重启
        /// </summary>
        private async Task UpdateRecommendedLineUpsAsync()
        {
            // 1. 询问用户是否进行更新
            var r = MessageBox.Show(
                "是否要从网络获取最新的推荐阵容数据？\n\n注意：更新期间程序将处于静默运行状态，完成后会自动提示。",
                "确认获取最新阵容",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (r != DialogResult.Yes)
            {
                return;
            }

            try
            {
                //  确保基础数据服务初始化
                // 这里不再传递 progress，实现静默初始化
                await _iDynamicGameDataService.InitializeAsync();

                //  执行爬虫流程
                // 传入 null。LineupCrawlingService 内部使用 ?. 运算符，
                // 传入 null 后将不会触发任何进度报告逻辑，静默运行。
                List<RecommendedLineUp> crawledLineups = await _iLineupCrawlingService.GetRecommendedLineUpsAsync(null);

                if (crawledLineups != null && crawledLineups.Any())
                {
                    // 数据保存
                    _iRecommendedLineUpService.ClearAll();

                    // 批量添加爬取到的数据
                    int addedCount = _iRecommendedLineUpService.AddRecommendedLineUps(crawledLineups);

                    // 保存到 RecommendedLineUps.json
                    bool saveResult = _iRecommendedLineUpService.Save();

                    if (saveResult)
                    {
                        // 刷新内存数据
                        _iRecommendedLineUpService.ReLoad();

                        // 静默运行结束后的唯一提示
                        MessageBox.Show(this,
                            $"推荐阵容数据更新成功！\n\n共成功抓取并加载了 {addedCount} 套阵容数据。\n数据已实时生效，无需重启程序。",
                            "更新完成",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("未能抓取到有效的阵容数据，请检查网络连接或稍后再试。",
                                  "更新失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // 仅在发生严重错误时进行弹窗提示
                MessageBox.Show($"更新推荐阵容时发生未知错误:\n{ex.Message}",
                              "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }   
}
