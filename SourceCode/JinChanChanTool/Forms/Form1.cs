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
            #endregion

            #region 英雄装备数据服务实例化
            _iheroEquipmentDataService = iheroEquipmentDataService;
            #endregion

            #region UI构建服务实例化并构建UI并绑定事件           
            _uiBuilderService = new UIBuilderService(this, panel_1Cost, panel_2Cost, panel_3Cost, panel_4Cost, panel_5Cost, panel_SelectByProfession, panel_SelectByPeculiarity, flowLayoutPanel_SubLineUp1, flowLayoutPanel__SubLineUp2, flowLayoutPanel__SubLineUp3, _iheroDataService, _iappConfigService.CurrentConfig.MaxOfChoices);
            UIBuildAndBidingEvents();
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
                _uiBuilderService.subLineUpPanels[i].MouseEnter += Panel_MouseEnter;
                _uiBuilderService.subLineUpPanels[i].MouseLeave += Panel_MouseLeave;
                _uiBuilderService.subLineUpPanels[i].MouseDown += Panel_MouseDown;
                _uiBuilderService.subLineUpPanels[i].MouseUp += Panel_MouseUp;
            }
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

            StatusOverlayForm.Instance.UpdateStatus(status1, status2);
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
            if(ErrorForm.Instance.WindowState == FormWindowState.Minimized)
            {
                ErrorForm.Instance.WindowState = FormWindowState.Normal;
                ErrorForm.Instance.Show();
                ErrorForm.Instance.BringToFront();
            }             
            if(!ErrorForm.Instance.Visible)
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
            waitForLoad = true;
            ClearCheckBox();
            waitForLoad = false;
            ClearSubLinePictureBoxes();
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
            _ilineUpService.OrderCurrentSubLineUp();
            LoadSubLineUpToUI();
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
            if (comboBox_LineUps.SelectedIndex != -1)
            {
                _ilineUpService.SetLineUpIndex(comboBox_LineUps.SelectedIndex);
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
            if (comboBox_LineUps.Items.Count > _ilineUpService.GetLineUpIndex())
            {
                comboBox_LineUps.Items[_ilineUpService.GetLineUpIndex()] = comboBox_LineUps.Text;
            }
            UpdataNameFromComboBoxToLineUps();
        }

        /// <summary>
        /// 当下拉框按下任意键时触发——>若该键是回车键则 保存阵容名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_LineUps_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                if (comboBox_LineUps.Items.Count > _ilineUpService.GetLineUpIndex())
                {
                    comboBox_LineUps.Items[_ilineUpService.GetLineUpIndex()] = comboBox_LineUps.Text;
                }
                UpdataNameFromComboBoxToLineUps();
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
            SwitchSubLineUp();
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
                if (_ilineUpService.DeleteHero(name))
                {
                    LoadSubLineUpToUI();
                }
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
            if (_ilineUpService.AddAndDeleteHero(name))
            {
                _ilineUpService.OrderCurrentSubLineUp();
                LoadSubLineUpToUI();
            }
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
            }
        }

        /// <summary>
        /// 清空所有的子阵容英雄头像框
        /// </summary>
        private void ClearAllSubLinePictureBoxes()
        {
            for (int i = 0; i < _uiBuilderService.subLineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < _uiBuilderService.subLineUpPictureBoxes.GetLength(1); j++)
                {
                    _uiBuilderService.subLineUpPictureBoxes[i, j].Image = null;
                    _uiBuilderService.subLineUpPictureBoxes[i, j].BorderColor = SystemColors.Control;
                }
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
            }
            //禁用所有子阵容英雄头像框
            for (int i = 0; i < _uiBuilderService.subLineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < _uiBuilderService.subLineUpPictureBoxes.GetLength(1); j++)
                {
                    _uiBuilderService.subLineUpPictureBoxes[i, j].Enabled = false;
                }
            }
            //启用当前子阵容的英雄头像框，并设置当前子阵容面板的背景色
            for (int i = 0; i < _uiBuilderService.subLineUpPictureBoxes.GetLength(1); i++)
            {
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Enabled = true;
            }
            _uiBuilderService.subLineUpPanels[_ilineUpService.GetSubLineUpIndex()].BackColor = Color.LightBlue;
            LoadSubLineUpToUI();
        }

        /// <summary>
        /// 根据指定的Profession选择英雄
        /// </summary>
        /// <param name="profession"></param>
        private void SelectHerosFromProfession(Profession profession)
        {
            for (int i = profession.HeroNames.Count - 1; i >= 0; i--)
            {
                string name = profession.HeroNames[i];
                if (!_ilineUpService.AddHero(name))
                {
                    break;
                }
            }
            _ilineUpService.OrderCurrentSubLineUp();
            LoadSubLineUpToUI();
        }

        /// <summary>
        /// 根据指定的Peculiarity选择英雄
        /// </summary>
        /// <param name="peculiarity"></param>
        private void SelectHerosFromPeculiarity(Peculiarity peculiarity)
        {
            for (int i = peculiarity.HeroNames.Count - 1; i >= 0; i--)
            {
                string name = peculiarity.HeroNames[i];
                if (!_ilineUpService.AddHero(name))
                {
                    break;
                }
            }
            _ilineUpService.OrderCurrentSubLineUp();
            LoadSubLineUpToUI();
        }



        /// <summary>
        /// 从阵容数据服务对象读取阵容名称到下拉框
        /// </summary>
        private void LoadNameFromLineUpsToComboBox()
        {
            comboBox_LineUps.Items.Clear();
            for (int i = 0; i < _ilineUpService.GetMaxLineUpCount(); i++)
            {
                comboBox_LineUps.Items.Add(_ilineUpService.GetLineUpName(i));
            }
            if (_ilineUpService.GetLineUpIndex() < comboBox_LineUps.Items.Count)
            {
                comboBox_LineUps.SelectedIndex = _ilineUpService.GetLineUpIndex();
            }
        }

        /// <summary>
        /// 从阵容下拉框更新阵容数据服务对象中的阵容名称
        /// </summary>
        private void UpdataNameFromComboBoxToLineUps()
        {
            for (int i = 0; i < _ilineUpService.GetMaxSelect(); i++)
            {
                _ilineUpService.SetLineUpName(i, comboBox_LineUps.Items[i].ToString());
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
            SwitchSubLineUp();
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

            for (int i = 0; i < CurrentSubLinePictureBoxImages.Count; i++)
            {
                Image image = CurrentSubLinePictureBoxImages[i];
                HeroData hero = _iheroDataService.GetHeroFromImage(image);
                if (hero != null)
                {
                    _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].Image = image;
                    _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.GetSubLineUpIndex(), i].BorderColor = _uiBuilderService.GetColor(hero.Cost);
                }
            }
            waitForLoad = false;
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

                // 智能判断：根据开头的特定文本，决定使用哪个解析器
                if (lineupCode.StartsWith("【阵容码】"))
                {
                    // 调用金铲铲解析器

                    heroNames = MobileLineUpParser.Parse(lineupCode);
                }
                else
                {

                    heroNames = LineUpParser.ParseCode(lineupCode);
                }

                // 统一处理结果
                if (heroNames != null && heroNames.Count > 0)
                {
                    _ilineUpService.ClearCurrentSubLineUp();
                    foreach (string name in heroNames)
                    {
                        _ilineUpService.AddHero(name);
                    }
                    LoadSubLineUpToUI();
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
    }
}
