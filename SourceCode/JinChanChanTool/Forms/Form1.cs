using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms;
using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Tools;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.LineUpCodeTools;
using JinChanChanTool.Tools.MouseTools;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Xml.Linq;
namespace JinChanChanTool
{
    public partial class Form1 : Form
    {
        #region 初始化相关
        /// <summary>
        /// 程序设置服务实例
        /// </summary>
        private readonly IAppConfigService _iappConfigService;

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

        public Form1(IAppConfigService iappConfigService, IHeroDataService iheroDataService, ILineUpService ilineUpService, ICorrectionService iCorrectionService, IHeroEquipmentDataService iheroEquipmentDataService)
        {
            InitializeComponent();
            #region 自定义标题栏
            // 自定义标题栏,带图标、带标题、最小化与关闭按钮。
            CustomTitleBar titleBar = new CustomTitleBar(this, 32, Image.FromFile(Path.Combine(Application.StartupPath, "Resources", "icon.ico")), "JinChanChanTool", CustomTitleBar.ButtonOptions.Close | CustomTitleBar.ButtonOptions.Minimize);
            this.Controls.Add(titleBar);
            #endregion

            LogTool.Log("主窗口已初始化！");

            #region 设置服务实例化并绑定事件
            _iappConfigService = iappConfigService;
            _iappConfigService.OnConfigSaved += OnConfigSaved;//绑定设置保存事件
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
            _uiBuilderService = new UIBuilderService(this, panel_1Cost, panel_2Cost, panel_3Cost, panel_4Cost, panel_5Cost, panel_SelectByProfession, panel_SelectByPeculiarity, flowLayoutPanel_SubLineUp1, flowLayoutPanel__SubLineUp2, flowLayoutPanel__SubLineUp3, _iheroDataService, _iappConfigService.CurrentConfig.MaxOfChoices, Selector.Instance.flowLayoutPanel1, Selector.Instance.flowLayoutPanel2, Selector.Instance.flowLayoutPanel3, Selector.Instance.flowLayoutPanel4, Selector.Instance.flowLayoutPanel5,LineUpForm.Instance.flowLayoutPanel1, LineUpForm.Instance.flowLayoutPanel2, LineUpForm.Instance.flowLayoutPanel3);
            UIBuildAndBidingEvents();
            #endregion

            #region 创建游戏窗口服务实例化并绑定事件
            _windowInteractionService = new WindowInteractionService();
            _coordService = new CoordinateCalculationService(_windowInteractionService); 
            _automationService = new AutomationService(_windowInteractionService, _coordService); 
            #endregion
        }      
        private void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化赛季下拉框
            comboBox_HeroPool.Items.Clear();
            foreach (string name in _iheroDataService.GetFilePaths())
            {
                comboBox_HeroPool.Items.Add(Path.GetFileName(name));
            }
            if (comboBox_HeroPool.Items.Count > 0)
            {
                comboBox_HeroPool.SelectedIndex = 0;
            }
            comboBox_HeroPool.SelectedIndexChanged += comboBox_HeroPool_SelectedIndexChanged;
            #endregion

            #region 初始化阵容下拉框
            LoadNameFromLineUpsToComboBox();
            #endregion

            #region 加载阵容到UI            
            LoadLineUpToUI();//分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容    
            #endregion  

            #region 初始化热键管理器并注册快捷键
            GlobalHotkeyTool.Initialize(this);
            RegisterHotKeys();//注册快捷键
            #endregion

            #region 自动拿牌服务实例化
            _cardService = new CardService(button_GetCard, button_Refresh, _iappConfigService, _iCorrectionService, _iheroDataService, _ilineUpService);            
            #endregion    

            #region 初始化鼠标钩子并绑定事件
            MouseHookTool.Initialize();
            MouseHookTool.MouseLeftButtonDown += MouseHook_MouseLeftButtonDown;
            MouseHookTool.MouseLeftButtonUp += MouseHook_MouseLeftButtonUp;
            #endregion

            #region 初始化状态显示窗口
            // 创建并显示状态窗口
            StatusOverlayForm.Instance.Show();


            UpdateOverlayStatus();
            #endregion         

            ShowErrorForm();          
            Selector.Instance.Show();
            LineUpForm.Instance.InitializeObject(_ilineUpService);
            LineUpForm.Instance.Show();
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
        private void OnConfigSaved()
        {
            // 确保在UI线程执行
            if (InvokeRequired)
            {
                Invoke(new Action(OnConfigSaved));
                return;
            }

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

        #endregion

        #region 快捷键注册
        /// <summary>
        /// 注册快捷键
        /// </summary>
        private void RegisterHotKeys()
        {
            string hotKey1 = _iappConfigService.CurrentConfig.HotKey1;
            string hotKey2 = _iappConfigService.CurrentConfig.HotKey2;
            string hotKey3 = _iappConfigService.CurrentConfig.HotKey3;
            string hotKey4 = _iappConfigService.CurrentConfig.HotKey4;
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
                    if (button_GetCard.Text == "启动")
                    {
                        button_GetCard_Click(button_GetCard, EventArgs.Empty);
                    }

                    if (button_Refresh.Text == "启动")
                    {
                        button_Refresh_Click(button_Refresh, EventArgs.Empty);
                    }
                });
            }
            GlobalHotkeyTool.RegisterKeyUp(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey4), () =>
            {
                if (button_GetCard.Text == "停止")
                {
                    button_GetCard_Click(button_GetCard, EventArgs.Empty);
                }

                if (button_Refresh.Text == "停止")
                {
                    button_Refresh_Click(button_Refresh, EventArgs.Empty);
                }
            });
        }
        #endregion

        #region UI构建
        /// <summary>
        /// 构建UI并绑定事件
        /// </summary>
        private void UIBuildAndBidingEvents()
        {
            _uiBuilderService.CreateHeroSelectors();
            _uiBuilderService.CreateProfessionAndPeculiarityButtons();           
            _uiBuilderService.CreateSubLineUpComponents();
            _uiBuilderService.CreateLineUpComponents();
            _uiBuilderService.CreateTransparentHeroPictureBox();
            //为每个奕子单选框绑定事件——选择状态改变时触发
            for (int i = 0; i < _uiBuilderService.checkBoxes.Count; i++)
            {
                _uiBuilderService.checkBoxes[i].CheckedChanged += CheckBoxCheckedChanged;

            }

            //为英雄头像框绑定交互事件
            for (int i = 0; i < _iheroDataService.GetHeroCount(); i++)
            {
                _uiBuilderService.heroPictureBoxes[i].MouseEnter += HeroPictureBox_MouseEnter;
                _uiBuilderService.heroPictureBoxes[i].MouseLeave += HeroPictureBox_MouseLeave;
                _uiBuilderService.heroPictureBoxes[i].MouseDown += HeroPictureBox_MouseDown;
                _uiBuilderService.heroPictureBoxes[i].MouseUp += HeroPictureBox_MouseUp;
            }

            for (int i = 0; i < _uiBuilderService.professionButtons.Count; i++)
            {
                _uiBuilderService.professionButtons[i].Click += ProfessionButtonClick;
            }

            for (int i = 0; i < _uiBuilderService.peculiarityButtons.Count; i++)
            {
                _uiBuilderService.peculiarityButtons[i].Click += PeculiarityButtonClick;
            }

            //为子阵容英雄头像框绑定交互事件            
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

            //为子阵容面板绑定交互事件
            for (int i = 0; i < _uiBuilderService.subLineUpPanels.Count; i++)
            {
                _uiBuilderService.subLineUpPanels[i].MouseEnter -= Panel_MouseEnter;
                _uiBuilderService.subLineUpPanels[i].MouseLeave -= Panel_MouseLeave;
                _uiBuilderService.subLineUpPanels[i].MouseDown -= Panel_MouseDown;
                _uiBuilderService.subLineUpPanels[i].MouseUp -= Panel_MouseUp;
                _uiBuilderService.subLineUpPanels[i].MouseEnter += Panel_MouseEnter;
                _uiBuilderService.subLineUpPanels[i].MouseLeave += Panel_MouseLeave;
                _uiBuilderService.subLineUpPanels[i].MouseDown += Panel_MouseDown;
                _uiBuilderService.subLineUpPanels[i].MouseUp += Panel_MouseUp;
            }

            //为英雄头像框绑定交互事件
            for (int i = 0; i < _iheroDataService.GetHeroCount(); i++)
            {
                _uiBuilderService.TransparentheroPictureBoxes[i].Click += TransparentheroPictureBoxes_Click;
                
            }

            for(int i =0;i<_uiBuilderService.lineUpPanels.Count;i++)
            {
                _uiBuilderService.lineUpPanels[i].MouseUp -= LineUpPanel_MouseUp;
                _uiBuilderService.lineUpPanels[i].MouseUp += LineUpPanel_MouseUp;
            }

            for (int j = 0; j < _uiBuilderService.lineUpPictureBoxes.GetLength(0); j++)
            {
                for (int i = 0; i < _uiBuilderService.lineUpPictureBoxes.GetLength(1); i++)
                {
                    _uiBuilderService.lineUpPictureBoxes[j, i].MouseUp -= LinePictureBox_MouseUp;
                    _uiBuilderService.lineUpPictureBoxes[j, i].MouseUp += LinePictureBox_MouseUp;
                }
            }
            LineUpForm.Instance.comboBox_LineUp.DropDownClosed -= comboBox_LineUps_DropDownClosed;
            LineUpForm.Instance.comboBox_LineUp.DropDownClosed += comboBox_LineUps_DropDownClosed;

            LineUpForm.Instance.comboBox_LineUp.Leave -= comboBox_LineUps_Leave;
            LineUpForm.Instance.comboBox_LineUp.Leave += comboBox_LineUps_Leave;

            LineUpForm.Instance.comboBox_LineUp.KeyDown -= comboBox_LineUps_KeyDown;
            LineUpForm.Instance.comboBox_LineUp.KeyDown += comboBox_LineUps_KeyDown;
        }
        #endregion

        #region 状态提示窗口
        /// <summary>
        /// 更新状态显示
        /// </summary>
        public void UpdateOverlayStatus()
        {
            // 获取两个开关的状态
            bool status1 = button_GetCard.Text == "停止"; // 假设按钮1是第一个开关
            bool status2 = button_Refresh.Text == "停止"; // 假设按钮2是第二个开关

            StatusOverlayForm.Instance.UpdateStatus(status1, status2,_iappConfigService.CurrentConfig.HotKey1,_iappConfigService.CurrentConfig.HotKey2, _iappConfigService.CurrentConfig.HotKey3, _iappConfigService.CurrentConfig.HotKey4);
        }
        #endregion

        #region 窗口、菜单项相关
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
        /// 菜单项“帮助-发送反馈”被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 发送反馈ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelpForm();
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
        /// 菜单项“识别错误输出窗口”触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_识别错误输出窗口_Click(object sender, EventArgs e)
        {
            ShowErrorForm();
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
            SettingForm _settingForm = new SettingForm(_iappConfigService);
            _settingForm.TopMost = true;
            _settingForm.Show();
            _settingForm.BringToFront();
        }


        private void ShowAboutForm()
        {
            AboutForm _aboutForm = new AboutForm();
            _aboutForm.TopMost = true;
            _aboutForm.Show();
            _aboutForm.BringToFront();
        }

        private void ShowHelpForm()
        {
            HelpForm _helpForm = new HelpForm();
            _helpForm.TopMost = true;
            _helpForm.Show();
            _helpForm.BringToFront();
        }

        private void ShowErrorForm()
        {
            if (ErrorForm.Instance.WindowState == FormWindowState.Minimized)
            {
                ErrorForm.Instance.WindowState = FormWindowState.Normal;
                ErrorForm.Instance.Show();
                ErrorForm.Instance.BringToFront();
            }
            if (!ErrorForm.Instance.Visible)
            {
                ErrorForm.Instance.Show();
                ErrorForm.Instance.BringToFront();
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
            if (button_GetCard.Text == "停止")
            {
                button_GetCard.Text = "启动";
                comboBox_HeroPool.Enabled = true;
                _cardService.StopLoop();
            }
            else
            {
                button_GetCard.Text = "停止";
                comboBox_HeroPool.Enabled = false;
                _cardService.StartLoop();
            }
            UpdateOverlayStatus();
        }

        /// <summary>
        /// 自动刷新商店按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button_Refresh_Click(object sender, EventArgs e)
        {
            LogTool.Log("手动切换自动刷新！");
            Debug.WriteLine("手动切换自动刷新！");
            if (button_Refresh.Text == "停止")
            {
                button_Refresh.Text = "启动";
                _cardService.AutoRefreshOff();
            }
            else
            {
                button_Refresh.Text = "停止";
                _cardService.AutoRefreshOn();
            }
            UpdateOverlayStatus();

        }
        private void button_GetCard_TextChanged(object sender, EventArgs e)
        {
            UpdateOverlayStatus();
        }

        private void button_Refresh_TextChanged(object sender, EventArgs e)
        {
            UpdateOverlayStatus();
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
            if (_iappConfigService.CurrentConfig.HighCursorcontrol)
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

            _iheroDataService.SetFilePathsIndex(comboBox_HeroPool.SelectedIndex);
            _iheroDataService.ReLoad();
            _iCorrectionService.SetCharDictionary(_iheroDataService.GetCharDictionary());
            if (!_ilineUpService.SetFilePathIndex(comboBox_HeroPool.SelectedIndex))
            {
                Debug.WriteLine("阵容文件路径下标设置失败，给定的下标不合法");
            }            
            _ilineUpService.ReLoad(_iheroDataService);           
            _uiBuilderService.UnBuild();            
            UIBuildAndBidingEvents();
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
        private void TransparentheroPictureBoxes_Click(object sender, EventArgs e)
        {
            HeroPictureBox clickedBox = sender as HeroPictureBox;
            string name = clickedBox.Tag as string;
            _ilineUpService.AddAndDeleteHero(name);
        }
        #endregion

        #region 阵容面板交互事件
        private void LineUpPanel_MouseUp(object sender, EventArgs e)
        {
            FlowLayoutPanel panel = sender as FlowLayoutPanel;
            panel.BackColor = Color.SkyBlue;
            _ilineUpService.SetSubLineUpIndex(_uiBuilderService.lineUpPanels.IndexOf(panel));
            
        }

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
                HeroData hero = _iheroDataService.GetHeroFromImage(image);
                if (hero != null)
                {
                    _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Image = image;
                    _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].BorderColor = _uiBuilderService.GetColor(hero.Cost);
                    _uiBuilderService.lineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Image = image;
                    _uiBuilderService.lineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].BorderColor = _uiBuilderService.GetColor(hero.Cost);

                }
            }
            waitForLoad = false;
            //输出当前阵容
            string lineup = "";
            for (int i = 0; i < _ilineUpService.GetCurrentSubLineUp().Count; i++)
            {
                lineup += _ilineUpService.GetCurrentSubLineUp()[i] + "    ";
            }
            Debug.WriteLine($"当前阵容：{lineup}");
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
        /// 处理“更新装备数据”按钮的点击事件。
        /// 这是协调所有新服务完成数据更新流程的总入口。
        /// </summary>
        private async void toolStripMenuItem_GetEquipments_Click(object sender, EventArgs e)
        {

            // 将 sender 转换为 Button 类型，以便我们访问它的属性
            var _toolStripMenuItem = sender as ToolStripMenuItem;
            if (_toolStripMenuItem == null) return;

            // 检查本地缓存的文件修改时间，避免频繁请求
            try
            {
                // 1. 获取当前赛季的 EquipmentData.json 文件路径
                string currentSeasonPath = _iheroEquipmentDataService.Paths[_iheroEquipmentDataService.PathIndex];
                string dataFilePath = Path.Combine(currentSeasonPath, "EquipmentData.json");

                // 2. 检查文件是否存在以及上次修改时间
                if (File.Exists(dataFilePath))
                {
                    DateTime lastUpdateTime = File.GetLastWriteTime(dataFilePath);
                    // 3. 设置一个缓存有效期，例如 6 小时
                    if (DateTime.Now - lastUpdateTime < TimeSpan.FromHours(6))
                    {
                        MessageBox.Show($"装备数据在 {lastUpdateTime:G} 刚刚更新过，已是最新，无需重复更新。",
                                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return; // 直接中断，不执行任何网络请求
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果检查文件路径或时间戳时出错，记录日志并允许用户继续尝试更新
                System.Diagnostics.Debug.WriteLine($"检查缓存时出错: {ex.Message}");
            }

            // 禁用按钮，防止用户重复点击
            _toolStripMenuItem.Enabled = false;
            // 创建进度条窗口，用于向用户反馈进度
            var progressForm = new JinChanChanTool.Forms.ProgressForm();
            var progress = new Progress<Tuple<int, string>>(update =>
            {
                progressForm.UpdateProgress(update.Item1, update.Item2);
            });

            try
            {

                progressForm.Show(this); // 显示进度窗口

                // 实时创建和注入服务 (遵循单一职责)
                // 1. 创建本地配置数据服务，并加载所有映射文件
                IApiRequestPayloadDataService payloadDataService = new ApiRequestPayloadDataService();
                await payloadDataService.LoadAllAsync();

                // 2. 创建网络爬取服务，并将 payloadDataService 注入进去
                ICrawlingService crawlingService = new CrawlingService(payloadDataService);

                // 3. 开始后台网络爬取，并等待结果
                List<HeroEquipment> crawledData = await crawlingService.GetEquipmentsAsync(progress);

                bool updateSuccess = false;
                if (crawledData != null && crawledData.Any())
                {
                    // 4. 将爬取到的新数据，传递给我们注入的“数据中心”服务进行更新和保存
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
                    // 5. 重新加载数据中心的内存（特别是重新加载图片）
                    _iheroEquipmentDataService.ReLoad();

                    // 6. 提示用户重启以确保所有状态都刷新
                    DialogResult result = MessageBox.Show(this,
                        "装备数据更新成功！\n\n为了确保所有组件都使用最新数据，建议重启程序。\n点击“确定”立即重启。",
                        "更新完成",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.OK)
                    {
                        Application.Restart();
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                // 捕获任何在流程中未被处理的异常
                MessageBox.Show($"更新过程中发生未知错误: {ex.Message}", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 无论成功还是失败，都确保关闭进度窗口并恢复按钮
                progressForm.Close();
                _toolStripMenuItem.Enabled = true;
            }
        }
        #endregion

        #region 装备展示
        private PictureBox _hoveredHeroPictureBox = null;//当前悬停的英雄头像框

        private ToolTip _activeToolTip = null; //用于持有当前活动的ToolTip实例
        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // 计时器只触发一次

            if (_hoveredHeroPictureBox != null)
            {

                // 步骤 1: 从 PictureBox 的 Tag 属性中获取 HeroData 对象
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

                    // 步骤 2: 使用 HeroData 的名字，去查找对应的 HeroEquipment 对象
                    HeroEquipment currentHeroEquipment = _iheroEquipmentDataService.GetHeroEquipmentFromName(name);

                    if (currentHeroEquipment != null)
                    {
                        List<Image> images = _iheroEquipmentDataService.GetImagesFromHeroEquipment(currentHeroEquipment);
                        // 确保有图片可供显示
                        if (images != null && images.Any())
                        {
                            // 步骤 3: 创建并显示新的 ToolTip
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
        private readonly WindowInteractionService _windowInteractionService;
        private readonly CoordinateCalculationService _coordService;
        private readonly AutomationService _automationService;

        private bool _multiProcessWarningShown = false;
        private void timer_UpdateCoordinates_Tick(object sender, EventArgs e)
        {            
            // 1. 检查是否处于自动模式
            if (!_iappConfigService.CurrentConfig.UseDynamicCoordinates)
            {
                _automationService.SetTargetProcess(null);
                return;
            }

            Process targetProcess = null;
            bool processFound = false;

            Process[] processes = null;

            // 2.ID 优先,检查用户上次精确选择的进程ID是否依然有效
            int targetId = _iappConfigService.CurrentConfig.TargetProcessId;
            if (targetId > 0)
            {
                //try
                //{
                //    Process p = Process.GetProcessById(targetId);
                //    // 确保进程名也匹配，防止PID被系统重用给其他程序
                //    if (p.ProcessName.Equals(_iappConfigService.CurrentConfig.TargetProcessName, StringComparison.OrdinalIgnoreCase))
                //    {
                //        targetProcess = p;
                //        processFound = true;
                //    }
                //}
                //catch (ArgumentException) // 进程已关闭会抛出此异常
                //{
                //    // ID无效，清除它，让程序回退到按名称查找
                //    _iappConfigService.CurrentConfig.TargetProcessId = 0;
                //}
                var runningProcessIds = Process.GetProcesses().Select(p => p.Id).ToHashSet();

                // 检查保存的ID是否存在于这个集合中
                if (runningProcessIds.Contains(targetId))
                {
                    // 只有在确定进程ID存在时，才安全地调用 GetProcessById
                    try
                    {
                        Process p = Process.GetProcessById(targetId);
                        if (p.ProcessName.Equals(_iappConfigService.CurrentConfig.TargetProcessName, StringComparison.OrdinalIgnoreCase))
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
                    _iappConfigService.CurrentConfig.TargetProcessId = 0;
                }
            }

            // 3.名称查找,如果按ID查找失败，则回退到按名称查找
            if (!processFound)
            {
                string targetName = _iappConfigService.CurrentConfig.TargetProcessName;
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
                    _iappConfigService.CurrentConfig.TargetProcessId = targetProcess.Id;
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

            // 4. 将最终确定的目标（或null）交给 AutomationService
            _automationService.SetTargetProcess(targetProcess);

            // 检查 processes 是否为 null，以避免在 ID 优先路径成功时出错
            if (processes == null || processes.Length <= 1)
            {
                _multiProcessWarningShown = false;
            }

            // 5. 如果成功锁定了有效窗口（无论是父窗口还是子窗口），则更新 AppConfig
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
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1 = rectSlot1.Value.X;
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY = rectSlot1.Value.Y;
                    _iappConfigService.CurrentConfig.Width_CardScreenshot = rectSlot1.Value.Width;
                    _iappConfigService.CurrentConfig.Height_CardScreenshot = rectSlot1.Value.Height;                   
                }

                if (rectSlot2.HasValue)
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2 = rectSlot2.Value.X;                  
                }

                if (rectSlot3.HasValue)
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3 = rectSlot3.Value.X;                    
                }

                if (rectSlot4.HasValue)
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4 = rectSlot4.Value.X;                   
                }

                if (rectSlot5.HasValue)
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5 = rectSlot5.Value.X;                   
                }
                // --- 刷新按钮中心点 ---
                if (rectRefresh.HasValue)
                {
                    _iappConfigService.CurrentConfig.Point_RefreshStoreX = rectRefresh.Value.X + rectRefresh.Value.Width / 2;
                    _iappConfigService.CurrentConfig.Point_RefreshStoreY = rectRefresh.Value.Y + rectRefresh.Value.Height / 2;                   
                }
            }
        }

        #endregion
    }
}
