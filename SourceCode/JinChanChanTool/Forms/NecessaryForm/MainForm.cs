using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms;
using JinChanChanTool.Services;
using JinChanChanTool.Services.AutoSetCoordinates;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Services.RecommendedEquipment;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.LineUpCodeTools;
using JinChanChanTool.Tools.MouseTools;
using System.Diagnostics;
using System.Linq;

namespace JinChanChanTool
{
    public partial class MainForm : Form
    {
        #region 初始化相关
        /// <summary>
        /// 程序设置服务实例
        /// </summary>
        private readonly IManualSettingsService _iManualSettingsService;

        /// <summary>
        /// 自动设置服务实例
        /// </summary>
        private readonly IAutomaticSettingsService _iAutomaticSettingsService;

        /// <summary>
        /// OCR结果纠正服务实例
        /// </summary>
        private readonly ICorrectionService _iCorrectionService;

        /// <summary>
        /// 英雄数据服务实例
        /// </summary>
        private readonly IHeroDataService _iheroDataService;

        /// <summary>
        /// 阵容数据服务实例
        /// </summary>
        private readonly ILineUpService _ilineUpService;

        /// <summary>
        /// 英雄装备推荐数据服务实例
        /// </summary>
        private readonly IHeroEquipmentDataService _iheroEquipmentDataService;

        /// <summary>
        /// UI构建服务实例
        /// </summary>
        private readonly UIBuilderService _uiBuilderService;

        /// <summary>
        /// 自动拿牌服务
        /// </summary>
        private CardService _cardService;
        
        // 这个字段将作为开关，记录了哪个赛季文件夹的名字才允许显示装备推荐
        private string _seasonForEquipmentTooltip = "S15天下无双格斗大会"; // <-- 在这里硬编码指定赛季名
        
        public MainForm(IManualSettingsService iappConfigService,IAutomaticSettingsService iAutoConfigService, IHeroDataService iheroDataService, ILineUpService ilineUpService, ICorrectionService iCorrectionService, IHeroEquipmentDataService iheroEquipmentDataService)
        {
            InitializeComponent();
            #region 自定义标题栏
            // 自定义标题栏,带图标、带标题、最小化与关闭按钮。
            CustomTitleBar titleBar = new CustomTitleBar(this, 32, Image.FromFile(Path.Combine(Application.StartupPath, "Resources", "icon.ico")), "JinChanChanTool", CustomTitleBar.ButtonOptions.Close | CustomTitleBar.ButtonOptions.Minimize);
            this.Controls.Add(titleBar);
            #endregion
            
            LogTool.Log("主窗口已初始化！");

            #region 设置服务实例化并绑定事件
            _iManualSettingsService = iappConfigService;
            _iManualSettingsService.OnConfigSaved += OnConfigSaved;//绑定设置保存事件
            _iAutomaticSettingsService = iAutoConfigService;            
            #endregion

            #region OCR结果纠正服务实例化
            _iCorrectionService = iCorrectionService;
            #endregion

            #region 英雄数据服务实例化
            _iheroDataService = iheroDataService;
            #endregion

            #region 阵容数据服务实例化
            _ilineUpService = ilineUpService;
            _ilineUpService.LineUpChanged += LineUpChanged;
            _ilineUpService.LineUpNameChanged += LineUpNameChanged;
            _ilineUpService.SubLineUpIndexChanged += SubLineUpIndexChanged;
            #endregion

            #region 英雄装备数据服务实例化
            _iheroEquipmentDataService = iheroEquipmentDataService;
            #endregion

            #region UI构建服务实例化并构建UI并绑定事件           
            _uiBuilderService = new UIBuilderService(_iheroDataService, _iManualSettingsService,this, tabControl_HeroSelector,flowLayoutPanel_SubLineUp1, flowLayoutPanel__SubLineUp2, flowLayoutPanel__SubLineUp3,LineUpForm.Instance.flowLayoutPanel1, LineUpForm.Instance.flowLayoutPanel2, LineUpForm.Instance.flowLayoutPanel3);
            _uiBuilderService.BuildWhenStart();
            UIBindingWhenStart();
            #endregion

            #region 创建游戏窗口服务实例化并绑定事件
            _windowInteractionService = new WindowInteractionService();
            _coordService = new CoordinateCalculationService(_windowInteractionService); 
            _automationService = new AutomationService(_windowInteractionService, _coordService); 
            #endregion
        }      
        private async void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化赛季下拉框
            
            comboBox_HeroPool.Items.Clear();
            foreach (string name in _iheroDataService.GetFilePaths())
            {
                comboBox_HeroPool.Items.Add(Path.GetFileName(name));
            }
            int selectedIndex = 0;
            if (!string.IsNullOrEmpty(_iAutomaticSettingsService.CurrentConfig.SelectedSeason))
            {
                for (int i = 0;i<comboBox_HeroPool.Items.Count;i++)
                {
                    if (comboBox_HeroPool.Items[i].ToString().Equals(_iAutomaticSettingsService.CurrentConfig.SelectedSeason, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }
            if (comboBox_HeroPool.Items.Count > 0)
            {
                comboBox_HeroPool.SelectedIndex = Math.Min(selectedIndex, comboBox_HeroPool.Items.Count - 1);
            }
            comboBox_HeroPool.SelectedIndexChanged += comboBox_HeroPool_SelectedIndexChanged;
            #endregion

            #region 初始化阵容下拉框
            LoadNameFromLineUpsToComboBox();
            #endregion

            #region 加载阵容到UI            
            LoadLineUpToUI();//分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容    
            #endregion  
            
            #region 自动拿牌服务实例化
            _cardService = new CardService(button_GetCard, button_Refresh, _iManualSettingsService,_iAutomaticSettingsService, _iCorrectionService, _iheroDataService, _ilineUpService);
            _cardService.isGetCardStatusChanged += OnIsGetCardChanged;
            _cardService.isRefreshStoreStatusChanged += OnAutoRefreshStatusChanged;
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

            #region 初始化状态显示窗口
            StatusOverlayForm.Instance.InitializeObject(_iAutomaticSettingsService,_cardService);
            if(_iManualSettingsService.CurrentConfig.IsUseStatusOverlayForm)
            {
                StatusOverlayForm.Instance.Show();
                UpdateOverlayStatus();
            }
            #endregion

            #region 初始化英雄选择窗口
            SelectForm.Instance.InitializeObject(_iAutomaticSettingsService);
            if(_iManualSettingsService.CurrentConfig.IsUseSelectForm)
            {
                SelectForm.Instance.Show();
            }
            #endregion

            #region 初始化阵容选择窗口
            LineUpForm.Instance.InitializeObject(_ilineUpService, _iAutomaticSettingsService);
            if(_iManualSettingsService.CurrentConfig.IsUseLineUpForm)
            {
                LineUpForm.Instance.Show();
            }
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
        /// 当程序关闭时执行——>注销所有快捷键
        /// </summary>
        /// <param name="e"></param>      
        protected override void OnFormClosing(FormClosingEventArgs e)
        { 
            // 注销热键            
            GlobalHotkeyTool.Dispose();
            MouseHookTool.Dispose();
            LogTool.Log("关闭！");
            base.OnFormClosing(e);
        }

        /// <summary>
        /// 当设置被保存时触发，询问用户是否重启应用。
        /// </summary>
        private void OnConfigSaved(object sender, ConfigChangedEventArgs e)
        {            
            // 确保在UI线程执行
            if (InvokeRequired)
            {
                Invoke(new Action<object, ConfigChangedEventArgs>(OnConfigSaved), sender, e);
                return;
            }            
            if(e.ChangedFields.Count == 0)
            {               
                if(e.IsManualChange)
                {
                    MessageBox.Show("保存成功！本次保存无设置项发生修改。", "无设置项发生修改", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            //Debug.WriteLine("-----------------------");
            //foreach (var f in e.ChangedFields)
            //{
            //    Debug.WriteLine(f.ToString());
            //}

            if (e.IsManualChange)
            {
                MessageBox.Show("设置保存成功！", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
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
                if(_iManualSettingsService.CurrentConfig.IsUseSelectForm)
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
           
            if(e.ChangedFields.Contains("TransparentHeroPictureBoxSize")||
               e.ChangedFields.Contains("TransparentHeroPictureBoxHorizontalSpacing")||
               e.ChangedFields.Contains("TransparentHeroPanelsVerticalSpacing")||
               e.ChangedFields.Contains("TransparentPanelDraggingBarWidth"))
            {
                _uiBuilderService.ReBuild();
                UIReBinding();
            }
           
            //如果变更的是需要重启才能生效的设置，则询问用户是否重启应用
            if (e.ChangedFields.Contains("MaxHerosCount")||
                e.ChangedFields.Contains("MaxLineUpCount") ||
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
        private void UIBindingWhenStart()
        {
            #region 主窗口UI事件绑定
            //为主窗口每个奕子单选框绑定事件——选择状态改变时触发
            for (int i = 0; i < _uiBuilderService.checkBoxes.Count; i++)
            {
                _uiBuilderService.checkBoxes[i].CheckedChanged += CheckBoxCheckedChanged;
            }

            //为主窗口英雄头像框绑定交互事件
            for (int i = 0; i < _uiBuilderService.heroPictureBoxes.Count; i++)
            {

                _uiBuilderService.heroPictureBoxes[i].MouseEnter += HeroPictureBox_MouseEnter;
                _uiBuilderService.heroPictureBoxes[i].MouseLeave += HeroPictureBox_MouseLeave;
                _uiBuilderService.heroPictureBoxes[i].MouseDown += HeroPictureBox_MouseDown;
                _uiBuilderService.heroPictureBoxes[i].MouseUp += HeroPictureBox_MouseUp;
            }

            //为主窗口按职业选择英雄按钮添加点击事件
            for (int i = 0; i < _uiBuilderService.professionButtons.Count; i++)
            {
                _uiBuilderService.professionButtons[i].Click += ProfessionButtonClick;
            }

            //为主窗口按特质选择英雄按钮添加点击事件
            for (int i = 0; i < _uiBuilderService.peculiarityButtons.Count; i++)
            {
                _uiBuilderService.peculiarityButtons[i].Click += PeculiarityButtonClick;
            }

            //为主窗口子阵容英雄头像框绑定交互事件            
            for (int j = 0; j < _uiBuilderService.subLineUpPictureBoxes.GetLength(0); j++)
            {
                for (int i = 0; i < _uiBuilderService.subLineUpPictureBoxes.GetLength(1); i++)
                {
                    _uiBuilderService.subLineUpPictureBoxes[j, i].MouseEnter += SubLinePictureBox_MouseEnter;
                    _uiBuilderService.subLineUpPictureBoxes[j, i].MouseLeave += SubLinePictureBox_MouseLeave;
                    _uiBuilderService.subLineUpPictureBoxes[j, i].MouseDown += SubLinePictureBox_MouseDown;
                    _uiBuilderService.subLineUpPictureBoxes[j, i].MouseUp += SubLinePictureBox_MouseUp;
                }
            }

            //为主窗口子阵容面板绑定交互事件
            for (int i = 0; i < _uiBuilderService.subLineUpPanels.Count; i++)
            {
                _uiBuilderService.subLineUpPanels[i].MouseEnter += Panel_MouseEnter;
                _uiBuilderService.subLineUpPanels[i].MouseLeave += Panel_MouseLeave;
                _uiBuilderService.subLineUpPanels[i].MouseDown += Panel_MouseDown;
                _uiBuilderService.subLineUpPanels[i].MouseUp += Panel_MouseUp;
            }
            #endregion

            #region 半透明英雄选择窗口UI事件
            //为英雄选择窗口英雄头像框绑定交互事件
            for (int i = 0; i < _uiBuilderService.TransparentheroPictureBoxes.Count; i++)
            {
                _uiBuilderService.TransparentheroPictureBoxes[i].MouseUp += TransparentheroPictureBoxes_Click;
                SelectForm.Instance.绑定拖动(_uiBuilderService.TransparentheroPictureBoxes[i]);

            }


            #endregion

            #region 阵容窗口UI事件
            //为阵容窗口子阵容面板绑定交互事件
            for (int i = 0; i < _uiBuilderService.lineUpPanels.Count; i++)
            {
                _uiBuilderService.lineUpPanels[i].MouseUp += LineUpPanel_MouseUp;
            }

            //为阵容窗口英雄头像框绑定交互事件
            for (int j = 0; j < _uiBuilderService.lineUpPictureBoxes.GetLength(0); j++)
            {
                for (int i = 0; i < _uiBuilderService.lineUpPictureBoxes.GetLength(1); i++)
                {
                    _uiBuilderService.lineUpPictureBoxes[j, i].MouseUp += LinePictureBox_MouseUp;
                }
            }

            //为阵容下拉框绑定事件
            LineUpForm.Instance.comboBox_LineUp.DropDownClosed += comboBox_LineUps_DropDownClosed;
            LineUpForm.Instance.comboBox_LineUp.Leave += comboBox_LineUps_Leave;
            LineUpForm.Instance.comboBox_LineUp.KeyDown += comboBox_LineUps_KeyDown;
            #endregion
        }

        private void UIReBinding()
        {
            #region 主窗口UI事件绑定
            //为主窗口每个奕子单选框绑定事件——选择状态改变时触发
            for (int i = 0; i < _uiBuilderService.checkBoxes.Count; i++)
            {
                _uiBuilderService.checkBoxes[i].CheckedChanged += CheckBoxCheckedChanged;
            }

            //为主窗口英雄头像框绑定交互事件
            for (int i = 0; i < _uiBuilderService.heroPictureBoxes.Count; i++)
            {

                _uiBuilderService.heroPictureBoxes[i].MouseEnter += HeroPictureBox_MouseEnter;
                _uiBuilderService.heroPictureBoxes[i].MouseLeave += HeroPictureBox_MouseLeave;
                _uiBuilderService.heroPictureBoxes[i].MouseDown += HeroPictureBox_MouseDown;
                _uiBuilderService.heroPictureBoxes[i].MouseUp += HeroPictureBox_MouseUp;
            }

            //为主窗口按职业选择英雄按钮添加点击事件
            for (int i = 0; i < _uiBuilderService.professionButtons.Count; i++)
            {
                _uiBuilderService.professionButtons[i].Click += ProfessionButtonClick;
            }

            //为主窗口按特质选择英雄按钮添加点击事件
            for (int i = 0; i < _uiBuilderService.peculiarityButtons.Count; i++)
            {
                _uiBuilderService.peculiarityButtons[i].Click += PeculiarityButtonClick;
            }
            #endregion

            #region 半透明英雄选择窗口UI事件
            //为英雄选择窗口英雄头像框绑定交互事件
            for (int i = 0; i < _uiBuilderService.TransparentheroPictureBoxes.Count; i++)
            {
                _uiBuilderService.TransparentheroPictureBoxes[i].MouseUp += TransparentheroPictureBoxes_Click;
                SelectForm.Instance.绑定拖动(_uiBuilderService.TransparentheroPictureBoxes[i]);
            }


            #endregion

            #region 阵容窗口UI事件          
            //为阵容窗口英雄头像框绑定交互事件
            for (int j = 0; j < _uiBuilderService.lineUpPictureBoxes.GetLength(0); j++)
            {
                for (int i = 0; i < _uiBuilderService.lineUpPictureBoxes.GetLength(1); i++)
                {                    
                    _uiBuilderService.lineUpPictureBoxes[j, i].MouseUp += LinePictureBox_MouseUp;
                }
            }           
            #endregion
        }
        #endregion

        #region 状态提示窗口
        /// <summary>
        /// 更新状态显示
        /// </summary>
        public void UpdateOverlayStatus()
        {            
            StatusOverlayForm.Instance.UpdateStatus(_iManualSettingsService.CurrentConfig.HotKey1,_iManualSettingsService.CurrentConfig.HotKey2, _iManualSettingsService.CurrentConfig.HotKey3, _iManualSettingsService.CurrentConfig.HotKey4);
        }
        #endregion

        #region 窗口、菜单项相关
        private SettingForm _settingFormInstance = null; // 保存窗口实例的字段
        private AboutForm _aboutFormInstance = null;
        #region UI事件
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
        /// 菜单项“关于”被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutForm();
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
      
        #endregion

        #region 方法
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

      
        #endregion
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

        private void OnIsGetCardChanged(bool isRunning)
        {
            // 确保在UI线程上更新
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(OnIsGetCardChanged), isRunning);
                return;
            }
            button_GetCard.Text = isRunning ? "停止" : "开启";           
        }
       
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
        #region UI事件       
        /// <summary>
        /// “清空”按钮_单击——>执行取消选择所有奕子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Clear_Click(object sender, EventArgs e)
        {
            _ilineUpService.ClearCurrentSubLineUp();          
        }

        /// <summary>
        /// “保存阵容”按钮_单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Save_Click(object sender, EventArgs e)
        {
            if (_ilineUpService.Save())
            {
                MessageBox.Show("阵容已保存", "阵容已保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

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
                if (!_ilineUpService.AddHero(name))
                {
                    waitForLoad = true;
                    _checkBox.Checked = false;
                    waitForLoad = false;
                }
            }
            else
            {
                _ilineUpService.DeleteHero(name);
            }

        }

        /// <summary>
        /// 赛季下拉框选择项被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_HeroPool_SelectedIndexChanged(object sender, EventArgs e)
        {
            _iAutomaticSettingsService.CurrentConfig.SelectedSeason = comboBox_HeroPool.Items[comboBox_HeroPool.SelectedIndex].ToString();
            _iAutomaticSettingsService.Save();
            _iheroDataService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason);
            _iheroDataService.ReLoad();
            _iCorrectionService.SetCharDictionary(_iheroDataService.GetCharDictionary());
            if (!_ilineUpService.SetFilePathsIndex(_iAutomaticSettingsService.CurrentConfig.SelectedSeason))
            {
                Debug.WriteLine($"严重错误：阵容数据服务对象所读取的阵容中，未包含赛季名为“{_iAutomaticSettingsService.CurrentConfig.SelectedSeason}”的赛季！");
                LogTool.Log($"严重错误：阵容数据服务对象所读取的阵容中，未包含赛季名为“{_iAutomaticSettingsService.CurrentConfig.SelectedSeason}”的赛季！");
                OutputForm.Instance.WriteLineOutputMessage($"严重错误：阵容数据服务对象所读取的阵容中，未包含赛季名为“{_iAutomaticSettingsService.CurrentConfig.SelectedSeason}”的赛季！");
                MessageBox.Show($"严重错误：阵容数据服务对象所读取的阵容中，未包含赛季名为“{_iAutomaticSettingsService.CurrentConfig.SelectedSeason}”的赛季！","严重错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }            
            _ilineUpService.ReLoad(_iheroDataService);      
            _iAutomaticSettingsService.CurrentConfig.SelectedLineUpIndex = _ilineUpService.GetLineUpIndex();
            _iAutomaticSettingsService.Save();

            _uiBuilderService.ReBuild();
            UIReBinding();           

            LoadNameFromLineUpsToComboBox();
            LoadLineUpToUI();//分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容            
        }

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
                _ilineUpService.SetLineUpIndex(comboBox.SelectedIndex);
                _iAutomaticSettingsService.CurrentConfig.SelectedLineUpIndex = _ilineUpService.GetLineUpIndex();
                _iAutomaticSettingsService.Save();
            }
            
            //从本地阵容文件读取数据到_lineupManager
            _ilineUpService.Load();
            //读取阵容名称到阵容下拉框，并将阵容下拉框当前选中项同步程序记录的值
            LoadNameFromLineUpsToComboBox();          
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
            if (comboBox.Items.Count > _ilineUpService.GetLineUpIndex())
            {
                comboBox.Items[_ilineUpService.GetLineUpIndex()] = comboBox.Text;
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
                if (comboBox.Items.Count > _ilineUpService.GetLineUpIndex())
                {
                    comboBox.Items[_ilineUpService.GetLineUpIndex()] = comboBox.Text;
                }
                UpdataNameFromComboBoxToLineUps(comboBox);
                this.ActiveControl = null;  // 将活动控件设置为null，下拉框失去焦点
            }

        }
        #endregion

        #region 子阵容面板交互事件
        /// <summary>
        /// Panel鼠标进入——>size变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseEnter(object sender, EventArgs e)
        {
            Panel panel = sender as FlowLayoutPanel;
            if (panel.BackColor == SystemColors.Control)
            {
                panel.BackColor = Color.FromArgb(255, (int)(173 * 1.05), (int)(216 * 1.05), (int)(230 * 1.05));
            }
        }

        /// <summary>
        /// Panel鼠标离开——>size还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseLeave(object sender, EventArgs e)
        {
            FlowLayoutPanel panel = sender as FlowLayoutPanel;
            if (panel.BackColor == Color.FromArgb(255, (int)(173 * 1.05), (int)(216 * 1.05), (int)(230 * 1.05)))
            {
                panel.BackColor = SystemColors.Control;
            }
        }

        /// <summary>
        /// Panel鼠标释放——>切换某个阵容下的子阵容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseUp(object sender, EventArgs e)
        {
            FlowLayoutPanel panel = sender as FlowLayoutPanel;
            panel.BackColor = Color.SkyBlue;
            _ilineUpService.SetSubLineUpIndex(_uiBuilderService.subLineUpPanels.IndexOf(panel));
           
        }

        /// <summary>
        /// Panel鼠标按下——>size变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseDown(object sender, EventArgs e)
        {
            FlowLayoutPanel panel = sender as FlowLayoutPanel;
            panel.BackColor = Color.FromArgb(255, (int)(173 * 0.9), (int)(216 * 0.9), (int)(230 * 0.9));
        }
        #endregion

        #region 子阵容英雄头像框交互事件
        /// <summary>
        /// （自定义）英雄头像框鼠标进入——>尺寸变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubLinePictureBox_MouseEnter(object sender, EventArgs e)
        {
            HeroPictureBox clickedBox = sender as HeroPictureBox;
            Size size = new Size(_uiBuilderService.GetSubLineUpPictureBoxSize().Width + 1, _uiBuilderService.GetSubLineUpPictureBoxSize().Height + 1);
            clickedBox.Size = this.LogicalToDeviceUnits(size);
        }

        /// <summary>
        ///  （自定义）英雄头像框鼠标离开——>尺寸还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubLinePictureBox_MouseLeave(object sender, EventArgs e)
        {
            HeroPictureBox clickedBox = sender as HeroPictureBox;
            Size size = new Size(_uiBuilderService.GetSubLineUpPictureBoxSize().Width, _uiBuilderService.GetSubLineUpPictureBoxSize().Height);
            clickedBox.Size = this.LogicalToDeviceUnits(size);
        }

        /// <summary>
        ///  （自定义）英雄头像框鼠标释放——>尺寸还原，
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubLinePictureBox_MouseUp(object sender, EventArgs e)
        {
            HeroPictureBox pictureBox_ = sender as HeroPictureBox;
            Image image = pictureBox_.Image;
            Size size = new Size(_uiBuilderService.GetSubLineUpPictureBoxSize().Width + 1, _uiBuilderService.GetSubLineUpPictureBoxSize().Height + 1);
            pictureBox_.Size = this.LogicalToDeviceUnits(size);
            if (image != null)
            {
                string name = _iheroDataService.GetHeroFromImage(image).HeroName;
                _ilineUpService.DeleteHero(name);
            }
        }

        /// <summary>
        ///  （自定义）英雄头像框鼠标按下——>尺寸变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubLinePictureBox_MouseDown(object sender, EventArgs e)
        {
            HeroPictureBox clickedBox = sender as HeroPictureBox;
            Size size = new Size(_uiBuilderService.GetSubLineUpPictureBoxSize().Width + 2, _uiBuilderService.GetSubLineUpPictureBoxSize().Height + 2);
            clickedBox.Size = this.LogicalToDeviceUnits(size);
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
        private void HeroPictureBox_MouseUp(object sender, EventArgs e)
        {

            HeroPictureBox clickedBox = sender as HeroPictureBox;
            //HeroData hero = clickedBox.Tag as HeroData;
            Size size = new Size(_uiBuilderService.GetHeroPictureBoxSize().Width + 1, _uiBuilderService.GetHeroPictureBoxSize().Height + 1);
            clickedBox.Size = this.LogicalToDeviceUnits(size);
            string name = clickedBox.Tag as string;
            _ilineUpService.AddAndDeleteHero(name);

        }

        /// <summary>
        /// 英雄头像框鼠标按下——>尺寸变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeroPictureBox_MouseDown(object sender, EventArgs e)
        {
            HeroPictureBox clickedBox = sender as HeroPictureBox;
            Size size = new Size(_uiBuilderService.GetHeroPictureBoxSize().Width + 2, _uiBuilderService.GetHeroPictureBoxSize().Height + 2);
            clickedBox.Size = this.LogicalToDeviceUnits(size);
        }
        #endregion

        #region 透明面板英雄头像框交互事件
        /// <summary>
        /// 透明面板英雄头像框单击——>通过图片溯源到hero对象，添加或删除该对象。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransparentheroPictureBoxes_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                HeroPictureBox clickedBox = sender as HeroPictureBox;
                string name = clickedBox.Tag as string;
                _ilineUpService.AddAndDeleteHero(name);
            }            
        }
        #endregion

        #region 阵容面板交互事件
        /// <summary>
        /// 阵容窗口子阵容面板鼠标释放——>切换某个阵容下的子阵容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineUpPanel_MouseUp(object sender, EventArgs e)
        {
            FlowLayoutPanel panel = sender as FlowLayoutPanel;
            panel.BackColor = Color.SkyBlue;
            _ilineUpService.SetSubLineUpIndex(_uiBuilderService.lineUpPanels.IndexOf(panel));
            
        }

        /// <summary>
        /// 阵容窗口英雄头像框鼠标释放——>尺寸还原，通过图片溯源到hero对象，取消选中该对象。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinePictureBox_MouseUp(object sender, EventArgs e)
        {
            HeroPictureBox pictureBox_ = sender as HeroPictureBox;
            Image image = pictureBox_.Image;                        
            if (image != null)
            {
                string name = _iheroDataService.GetHeroFromImage(image).HeroName;
                _ilineUpService.DeleteHero(name);
            }
        }
        #endregion
        #endregion

        #region 方法
        /// <summary>
        /// 使奕子复选框全部取消勾选
        /// </summary>
        private void ClearCheckBox()
        {
            for (int i = 0; i < _uiBuilderService.checkBoxes.Count; i++)
            {
                _uiBuilderService.checkBoxes[i].Checked = false;
            }
        }
       
        /// <summary>
        /// 清空选中的子阵容的英雄头像框
        /// </summary>
        private void ClearSubLinePictureBoxes()
        {
            for (int i = 0; i < _uiBuilderService.subLineUpPictureBoxes.GetLength(1); i++)
            {
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Image = null;
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].BorderColor = SystemColors.Control;
                _uiBuilderService.lineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Image = null;
                _uiBuilderService.lineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].BorderColor = SystemColors.Control;
            }
        }
       
        /// <summary>
        /// 切换子阵容
        /// </summary>
        void SwitchSubLineUp()
        {
            //清空所有子阵容面板的背景色
            for (int i = 0; i < _uiBuilderService.subLineUpPanels.Count; i++)
            {
                _uiBuilderService.subLineUpPanels[i].BackColor = SystemColors.Control;
                _uiBuilderService.lineUpPanels[i].BackColor = SystemColors.Control;
            }
            //禁用所有子阵容英雄头像框
            for (int i = 0; i < _uiBuilderService.subLineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < _uiBuilderService.subLineUpPictureBoxes.GetLength(1); j++)
                {
                    _uiBuilderService.subLineUpPictureBoxes[i, j].Enabled = false;
                    _uiBuilderService.lineUpPictureBoxes[i, j].Enabled = false;
                }
            }
            //启用当前子阵容的英雄头像框，并设置当前子阵容面板的背景色
            for (int i = 0; i < _uiBuilderService.subLineUpPictureBoxes.GetLength(1); i++)
            {
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Enabled = true;
                _uiBuilderService.lineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Enabled = true;
            }
            _uiBuilderService.subLineUpPanels[_ilineUpService.GetSubLineUpIndex()].BackColor = Color.LightBlue;
            _uiBuilderService.lineUpPanels[_ilineUpService.GetSubLineUpIndex()].BackColor = Color.LightBlue;
            LoadSubLineUpToUI();
        }

        /// <summary>
        /// 根据指定的Profession选择英雄
        /// </summary>
        /// <param name="profession"></param>
        private void SelectHerosFromProfession(Profession profession)
        {
            _ilineUpService.AddHeros(profession.HeroNames);
        }

        /// <summary>
        /// 根据指定的Peculiarity选择英雄
        /// </summary>
        /// <param name="peculiarity"></param>
        private void SelectHerosFromPeculiarity(Peculiarity peculiarity)
        {
            _ilineUpService.AddHeros(peculiarity.HeroNames);
        }

        /// <summary>
        /// 从阵容数据服务对象读取阵容名称到下拉框
        /// </summary>
        private void LoadNameFromLineUpsToComboBox()
        {
            comboBox_LineUps.Items.Clear();
            LineUpForm.Instance.comboBox_LineUp.Items.Clear();
            for (int i = 0; i < _ilineUpService.GetMaxLineUpCount(); i++)
            {
                comboBox_LineUps.Items.Add(_ilineUpService.GetLineUpName(i));
                LineUpForm.Instance.comboBox_LineUp.Items.Add(_ilineUpService.GetLineUpName(i));
            }
            if (_ilineUpService.GetLineUpIndex() < comboBox_LineUps.Items.Count)
            {
                comboBox_LineUps.SelectedIndex = _ilineUpService.GetLineUpIndex();
            }
            if (_ilineUpService.GetLineUpIndex() < LineUpForm.Instance.comboBox_LineUp.Items.Count)
            {
                LineUpForm.Instance.comboBox_LineUp.SelectedIndex = _ilineUpService.GetLineUpIndex();
            }
        }

        /// <summary>
        /// 从阵容下拉框更新阵容数据服务对象中的阵容名称
        /// </summary>
        private void UpdataNameFromComboBoxToLineUps(ComboBox comboBox)
        {
            for (int i = 0; i < _ilineUpService.GetMaxLineUpCount(); i++)
            {
                _ilineUpService.SetLineUpName(i, comboBox.Items[i].ToString());
            }
        }

        /// <summary>
        /// 加载当前选中的阵容对象的三个子阵容到UI，并默认选择第1个子阵容
        /// </summary>
        private void LoadLineUpToUI()
        {
            _ilineUpService.SetSubLineUpIndex(2);
            LoadSubLineUpToUI();
            _ilineUpService.SetSubLineUpIndex(1);
            LoadSubLineUpToUI();
            _ilineUpService.SetSubLineUpIndex(0);
            LoadSubLineUpToUI();            
        }

        /// <summary>
        /// 加载当前选中子阵容到UI(奕子复选框，子阵容头像框)
        /// </summary>
        public void LoadSubLineUpToUI()
        {
            List<Image> CurrentSubLinePictureBoxImages = new List<Image>();
            waitForLoad = true;
            ClearCheckBox();
            ClearSubLinePictureBoxes();
            foreach (string name in _ilineUpService.GetCurrentSubLineUp())
            {
                CheckBox checkBox = _uiBuilderService.GetCheckBoxFromName(name);
                Image image = _iheroDataService.GetImageFromHero(_iheroDataService.GetHeroFromName(name));
                if (checkBox != null)
                {
                    checkBox.Checked = true;
                }
                if (image != null)
                {
                    CurrentSubLinePictureBoxImages.Add(image);
                }
            }
            foreach(HeroPictureBox heroPictureBox in _uiBuilderService.TransparentheroPictureBoxes)
            {
                if(_ilineUpService.GetCurrentSubLineUp().Contains(heroPictureBox.Tag as string))
                {
                   
                    heroPictureBox.IsSelected = true;
                }
                else
                {
                   
                    heroPictureBox.IsSelected = false;
                }
            }

            for (int i = 0; i < CurrentSubLinePictureBoxImages.Count; i++)
            {
                Image image = CurrentSubLinePictureBoxImages[i];
                Hero hero = _iheroDataService.GetHeroFromImage(image);
                if (hero != null)
                {
                    _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Image = image;
                    _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].BorderColor = _uiBuilderService.GetColor(hero.Cost);
                    _uiBuilderService.lineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Image = image;
                    _uiBuilderService.lineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].BorderColor = _uiBuilderService.GetColor(hero.Cost);

                }
            }
            waitForLoad = false;           
        }

        /// <summary>
        /// 当iLineUpService中的当前子阵容被修改时触发——>重新加载当前子阵容到UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineUpChanged(object sender, EventArgs e)
        {
            LoadSubLineUpToUI();
        }

        /// <summary>
        /// 当iLineUpService中的阵容被保存时触发——>重新从本地阵容文件读取数据到_lineupManager，并刷新阵容下拉框和UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineUpNameChanged(object sender, EventArgs e)
        {
            //从本地阵容文件读取数据到_lineupManager
            _ilineUpService.Load();
            //读取阵容名称到阵容下拉框，并将阵容下拉框当前选中项同步程序记录的值
            LoadNameFromLineUpsToComboBox();
            //分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容
            LoadLineUpToUI();
        }

        /// <summary>
        /// 当iLineUpService中的当前子阵容下标被修改时触发——>切换子阵容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubLineUpIndexChanged(object sender, EventArgs e)
        {
            SwitchSubLineUp();
        }
        
        #endregion
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
                    _ilineUpService.ClearCurrentSubLineUp();
                    _ilineUpService.AddHeros(heroNames);
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
            if (timeDifference.TotalHours<=_iManualSettingsService.CurrentConfig.UpdateEquipmentInterval)
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

                // 创建新的动态数据服务
                IDynamicGameDataService gameDataService = new DynamicGameDataService();

                // 初始化服务，从网络获取英雄列表和翻译等基础数据                
                progress.Report(Tuple.Create(0, "正在初始化，获取最新游戏数据..."));
                await gameDataService.InitializeAsync();
                progress.Report(Tuple.Create(5, "基础数据获取成功！")); // 给用户一个初始反馈

                // 创建网络爬取服务，并将新的 gameDataService 注入进去
                ICrawlingService crawlingService = new CrawlingService(gameDataService);

                // 开始后台网络爬取，并等待结果
                List<HeroEquipment> crawledData = await crawlingService.GetEquipmentsAsync(progress);

                bool updateSuccess = false;
                if (crawledData != null && crawledData.Any())
                {
                    // 将爬取到的新数据，传递给我们注入的“数据中心”服务进行更新和保存 
                    _iheroEquipmentDataService.UpdateDataFromCrawling(crawledData);
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
                    _iheroEquipmentDataService.ReLoad();

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
                    HeroEquipment currentHeroEquipment = _iheroEquipmentDataService.GetHeroEquipmentFromName(name);

                    if (currentHeroEquipment != null)
                    {
                        List<Image> images = _iheroEquipmentDataService.GetImagesFromHeroEquipment(currentHeroEquipment);
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
            // 检查是否处于自动坐标模式
            if (!_iManualSettingsService.CurrentConfig.IsUseDynamicCoordinates)
            {
                _automationService.SetTargetProcess(null);
                return;
            }

            Process targetProcess = null;
            bool processFound = false;

            Process[] processes = null;

            // ID 优先,检查用户上次精确选择的进程ID是否依然有效
            int targetId = _iManualSettingsService.CurrentConfig.TargetProcessId;
            if (targetId > 0)
            {               
                var runningProcessIds = Process.GetProcesses().Select(p => p.Id).ToHashSet();

                // 检查保存的ID是否存在于这个集合中
                if (runningProcessIds.Contains(targetId))
                {
                    // 只有在确定进程ID存在时，才安全地调用 GetProcessById
                    try
                    {
                        Process p = Process.GetProcessById(targetId);
                        if (p.ProcessName.Equals(_iManualSettingsService.CurrentConfig.TargetProcessName, StringComparison.OrdinalIgnoreCase))
                        {
                            targetProcess = p;
                            processFound = true;
                        }
                    }
                    catch
                    {
                        // 即使ID存在，GetProcessById也可能因为权限等乱七八糟的问题失败，留一个空的catch以防万一
                    }
                }
                else
                {
                    // 如果ID不存在，说明进程已关闭
                    _iManualSettingsService.CurrentConfig.TargetProcessId = 0;
                }
            }

            // 如果在按ID查找时未找到进程，则按名称查找
            if (!processFound)
            {
                string targetName = _iManualSettingsService.CurrentConfig.TargetProcessName;
                if (string.IsNullOrEmpty(targetName))
                {
                    _automationService.SetTargetProcess(null);
                    return;
                }

                processes = Process.GetProcessesByName(targetName);

                if (processes.Length == 1)
                {
                    targetProcess = processes[0];
                    // 找到了唯一匹配的进程，更新我们的精确ID以便下次快速查找
                    _iManualSettingsService.CurrentConfig.TargetProcessId = targetProcess.Id;
                }
                else if (processes.Length > 1)
                {
                    // 【多进程冲突处理】
                    _automationService.SetTargetProcess(null); // 暂停坐标更新
                    if (!_multiProcessWarningShown)
                    {
                        _multiProcessWarningShown = true; // 先设置标志，防止重复弹窗
                        this.Invoke((Action)(() => {
                            MessageBox.Show(this,
                                $"检测到多个名为 '{targetName}' 的进程。\n\n自动坐标计算已暂停，请通过“设置”->“选择进程”来精确指定一个。",
                                "多进程冲突",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }));
                    }
                    return; // 直接返回，不进行后续的坐标更新
                }
                else // processes.Length == 0
                {
                    targetProcess = null;
                }
            }

            // 将最终确定的目标（或null）交给 AutomationService
            _automationService.SetTargetProcess(targetProcess);

            // 检查 processes 是否为 null，以避免在 ID 优先路径成功时出错
            if (processes == null || processes.Length <= 1)
            {
                _multiProcessWarningShown = false;
            }

            // 如果成功锁定了有效窗口（无论是父窗口还是子窗口），则更新 AppConfig
            if (_automationService.IsGameDetected)
            {
                var rectSlot1 = _automationService.GetTargetRectangle(UiElement.CardSlot1_Name);
                var rectSlot2 = _automationService.GetTargetRectangle(UiElement.CardSlot2_Name);
                var rectSlot3 = _automationService.GetTargetRectangle(UiElement.CardSlot3_Name);
                var rectSlot4 = _automationService.GetTargetRectangle(UiElement.CardSlot4_Name);
                var rectSlot5 = _automationService.GetTargetRectangle(UiElement.CardSlot5_Name);
                var rectRefresh = _automationService.GetTargetRectangle(UiElement.RefreshButton);


                if (rectSlot1.HasValue)
                {
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X1 = rectSlot1.Value.X;
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_Y = rectSlot1.Value.Y;
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotWidth = rectSlot1.Value.Width;
                    _iAutomaticSettingsService.CurrentConfig.Height_CardScreenshot = rectSlot1.Value.Height;                   
                }

                if (rectSlot2.HasValue)
                {
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X2 = rectSlot2.Value.X;                  
                }

                if (rectSlot3.HasValue)
                {
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X3 = rectSlot3.Value.X;                    
                }

                if (rectSlot4.HasValue)
                {
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X4 = rectSlot4.Value.X;                   
                }

                if (rectSlot5.HasValue)
                {
                    _iAutomaticSettingsService.CurrentConfig.HeroNameScreenshotCoordinates_X5 = rectSlot5.Value.X;                   
                }
                // --- 刷新按钮中心点 ---
                if (rectRefresh.HasValue)
                {
                    _iAutomaticSettingsService.CurrentConfig.RefreshStoreButtonCoordinates_X = rectRefresh.Value.X + rectRefresh.Value.Width / 2;
                    _iAutomaticSettingsService.CurrentConfig.RefreshStoreButtonCoordinates_Y = rectRefresh.Value.Y + rectRefresh.Value.Height / 2;                   
                }
            }
        }

        #endregion
    }
}
