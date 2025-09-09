using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Tools;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.MouseTools;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Drawing.Imaging;
using JinChanChanTool.Tools.LineUpCodeTools;
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
        /// UI构建服务实例
        /// </summary>
        private readonly UIBuilderService _uiBuilderService;

        /// <summary>
        /// 自动拿牌服务
        /// </summary>
        private CardService _cardService;

        public Form1(IAppConfigService iappConfigService, IHeroDataService iheroDataService, ILineUpService ilineUpService, ICorrectionService iCorrectionService)
        {
            InitializeComponent();
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

            #region UI构建服务实例化并构建UI并绑定事件           
            _uiBuilderService = new UIBuilderService(this, tabPage1, tabPage2, tabPage3, tabPage4, tabPage5, panel10, panel11, _iheroDataService, _ilineUpService, _iappConfigService);
            UIBuildAndBidingEvents();
            #endregion                                                     
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化赛季下拉框
            comboBox2.Items.Clear();
            foreach (string name in _iheroDataService.Paths)
            {
                comboBox2.Items.Add(Path.GetFileName(name));
            }
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            #endregion

            #region 初始化阵容下拉框
            LoadNameFromLineUpsToComboBox();
            #endregion

            #region 加载阵容到UI            
            LoadImagesFromLineUpsToSubLineUpPictureBoxesAndSelectTheFirstSubLineUp();//分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容    
            #endregion  

            #region 初始化热键管理器并注册快捷键
            GlobalHotkeyTool.Initialize(this);
            RegisterHotKeys();//注册快捷键
            #endregion

            #region 自动拿牌服务实例化
            _cardService = new CardService(button1, button2, _iappConfigService, _iCorrectionService, _iheroDataService, _ilineUpService);
            #endregion    

            #region 初始化鼠标钩子并绑定事件
            MouseHookTool.Initialize();
            MouseHookTool.MouseLeftButtonDown += MouseHook_MouseLeftButtonDown;
            MouseHookTool.MouseLeftButtonUp += MouseHook_MouseLeftButtonUp;
            #endregion

            if (_iheroDataService.Paths.Length > _iheroDataService.PathIndex)
            {
                string initialSeasonPath = _iheroDataService.Paths[_iheroDataService.PathIndex];
                btnUpdateData.Tag = initialSeasonPath; // 假设你的更新按钮名叫 btnUpdateData
            }

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
                GlobalHotkeyTool.Register(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey1), () => button1_Click(this, EventArgs.Empty));
            }
            if (GlobalHotkeyTool.IsKeyAvailable(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey2)))
            {
                GlobalHotkeyTool.Register(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey2), () => button2_Click(this, EventArgs.Empty));
            }
            if (GlobalHotkeyTool.IsKeyAvailable(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey3)))
            {
                GlobalHotkeyTool.Register(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey3), () => ShowMainForm());
            }
            if (GlobalHotkeyTool.IsKeyAvailable(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey4)))
            {
                GlobalHotkeyTool.Register(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey4), () =>
                {
                    if (button1.Text == "启动")
                    {
                        button1_Click(button1, EventArgs.Empty);
                    }

                    if (button2.Text == "启动")
                    {
                        button2_Click(button2, EventArgs.Empty);
                    }
                });
            }
            GlobalHotkeyTool.RegisterKeyUp(GlobalHotkeyTool.ConvertKeyNameToEnumValue(hotKey4), () =>
            {
                if (button1.Text == "停止")
                {
                    button1_Click(button1, EventArgs.Empty);
                }

                if (button2.Text == "停止")
                {
                    button2_Click(button2, EventArgs.Empty);
                }
            });
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
            for (int i = 0; i < _iheroDataService.HeroDatas.Count; i++)
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

        /// <summary>
        /// 赛季下拉框选择项被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ilineUpService.PathIndex = comboBox2.SelectedIndex;
            _iheroDataService.PathIndex = comboBox2.SelectedIndex;
            _iheroDataService.ReLoad();
            _ilineUpService.ReLoad(_iheroDataService.HeroDatas.Count, _iappConfigService.CurrentConfig.CountOfLine);
            _uiBuilderService.UnBuild();
            UIBuildAndBidingEvents();
            LoadNameFromLineUpsToComboBox();
            LoadImagesFromLineUpsToSubLineUpPictureBoxesAndSelectTheFirstSubLineUp();//分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容
            if (_iheroDataService.Paths.Length > _iheroDataService.PathIndex)
            {
                string newSeasonPath = _iheroDataService.Paths[_iheroDataService.PathIndex];
                btnUpdateData.Tag = newSeasonPath;
            }                                                                         
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
        #endregion
        #endregion

        #region 拿牌相关
        /// <summary>
        /// 自动拿牌按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "停止")
            {
                button1.Text = "启动";
                comboBox2.Enabled = true;
                _cardService.StopLoop();
            }
            else
            {
                button1.Text = "停止";
                comboBox2.Enabled = false;
                _cardService.StartLoop();
            }
        }

        /// <summary>
        /// 自动刷新商店按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "停止")
            {
                button2.Text = "启动";
                _cardService.AutoRefreshOff();
            }
            else
            {
                button2.Text = "停止";
                _cardService.AutoRefreshOn();
            }
        }
        #endregion

        #region 英雄选择与阵容相关
        private bool waitForClear = false;//是否处于临时清空CheckBox状态，处于该状态下对CheckBox的状态改变将不会反映到LineUp对象        

        #region UI事件
        /// <summary>
        /// “清空”按钮_单击——>执行取消选择所有奕子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ClearCheckBox();
        }

        /// <summary>
        /// “保存阵容”按钮_单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            _ilineUpService.Save();
            MessageBox.Show("阵容已保存！");
        }

        /// <summary>
        /// 当下拉框被关闭（即选择了新的或没选）时触发——>记录当前选择的下拉框，并从中读取阵容组合到单选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                _ilineUpService.LineUpIndex = comboBox1.SelectedIndex;
            }
            //从本地阵容文件读取数据到_lineupManager
            _ilineUpService.Load();
            //从_lineupManager读取阵容名称到阵容下拉框，并将阵容下拉框当前选中项同步程序记录的值
            LoadNameFromLineUpsToComboBox();
            //分别加载阵容到三个子阵容英雄头像框，并且最后选择第一个子阵容
            LoadImagesFromLineUpsToSubLineUpPictureBoxesAndSelectTheFirstSubLineUp();
        }

        /// <summary>
        /// 离开下拉框时触发——>保存阵容名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_Leave(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > _ilineUpService.LineUpIndex)
            {
                comboBox1.Items[_ilineUpService.LineUpIndex] = comboBox1.Text;
            }
            LoadNameFromComboBoxToLineUps();
        }

        /// <summary>
        /// 当下拉框按下任意键时触发——>若该键是回车键则 保存阵容名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                if (comboBox1.Items.Count > _ilineUpService.LineUpIndex)
                {
                    comboBox1.Items[_ilineUpService.LineUpIndex] = comboBox1.Text;
                }
                LoadNameFromComboBoxToLineUps();
                this.ActiveControl = null;  // 将活动控件设置为null，下拉框失去焦点
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
            //如果处于切换阵容时，则会清空勾选框，此时不应触发该事件
            if (waitForClear)
            {
                return;
            }
            CheckBox checkBox_ = sender as CheckBox;
            HeroData hero = checkBox_.Tag as HeroData;
            //如果选中的奕子数量超过当前子阵容允许的最大数量，则取消本次勾选
            if (CountOfCheckedHero() > _uiBuilderService.subLineUpPictureBoxes.GetLength(1))
            {
                checkBox_.Checked = false;
            }
            //否则，更新当前LineUp对象
            else
            {
                _ilineUpService.LineUps[_ilineUpService.LineUpIndex].Checked[_ilineUpService.SubLineUpIndex, _iheroDataService.HeroDatas.IndexOf(hero)] = checkBox_.Checked;
                LoadImagesFromCheckBoxToSubLinePictureBoxes();
            }
        }

        #region 子阵容面板交互事件
        /// <summary>
        /// Panel鼠标进入——>size变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseEnter(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            Size newSize = new Size(_uiBuilderService.subLineUpPictureBoxes.GetLength(1) <= 10 ? _uiBuilderService.GetSubLineUpPanelSizes(0).Width + 1 : _uiBuilderService.GetSubLineUpPanelSizes(1).Width + 1, _uiBuilderService.subLineUpPictureBoxes.GetLength(1) <= 10 ? _uiBuilderService.GetSubLineUpPanelSizes(0).Height + 1 : _uiBuilderService.GetSubLineUpPanelSizes(1).Height + 1);
            panel.Size = this.LogicalToDeviceUnits(newSize);
        }

        /// <summary>
        /// Panel鼠标离开——>size还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseLeave(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            Size newSize = _uiBuilderService.subLineUpPictureBoxes.GetLength(1) <= 10 ? _uiBuilderService.GetSubLineUpPanelSizes(0) : _uiBuilderService.GetSubLineUpPanelSizes(1);
            panel.Size = this.LogicalToDeviceUnits(newSize);
        }

        /// <summary>
        /// Panel鼠标释放——>切换某个阵容下的子阵容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseUp(object sender, EventArgs e)
        {

            Panel panel = sender as Panel;
            panel.Size = this.LogicalToDeviceUnits(new Size(panel.Width - 1, panel.Height - 1));
            _ilineUpService.SubLineUpIndex = _uiBuilderService.subLineUpPanels.IndexOf(panel);
            SwitchSubLineUp();
            LoadCheckStateFromLineUpsToCheckBox();
        }

        /// <summary>
        /// Panel鼠标按下——>size变大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_MouseDown(object sender, EventArgs e)
        {
            Panel panel = (Panel)sender;
            panel.Size = this.LogicalToDeviceUnits(new Size(panel.Width + 1, panel.Height + 1));
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
                if (_iheroDataService.ImageToHeroDataMap.TryGetValue(image, out HeroData hero) && _uiBuilderService.checkBoxes[_iheroDataService.HeroDatas.IndexOf(hero)].Checked)
                {
                    _uiBuilderService.checkBoxes[_iheroDataService.HeroDatas.IndexOf(hero)].Checked = false;
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
        }

        /// <summary>
        /// 英雄头像框鼠标释放——>尺寸还原，通过图片溯源到hero对象，修改其对应的CheckBox勾选状态。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeroPictureBox_MouseUp(object sender, EventArgs e)
        {

            HeroPictureBox clickedBox = sender as HeroPictureBox;
            HeroData hero = clickedBox.Tag as HeroData;
            Size size = new Size(_uiBuilderService.GetHeroPictureBoxSize().Width + 1, _uiBuilderService.GetHeroPictureBoxSize().Height + 1);
            clickedBox.Size = this.LogicalToDeviceUnits(size);
            int index = _iheroDataService.HeroDatas.IndexOf(hero);
            _uiBuilderService.checkBoxes[index].Checked = !_uiBuilderService.checkBoxes[index].Checked;

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
        /// 使奕子选择单选框全部取消勾选，会改变当前Lineup对象
        /// </summary>
        private void ClearCheckBox()
        {
            for (int i = 0; i < _uiBuilderService.checkBoxes.Count; i++)
            {
                _uiBuilderService.checkBoxes[i].Checked = false;
            }
        }

        /// <summary>
        /// 临时使奕子单选框全部取消勾选，不改变当前Lineup对象
        /// </summary>
        private void TempClearCheckBox()
        {
            waitForClear = true;
            for (int i = 0; i < _uiBuilderService.checkBoxes.Count; i++)
            {
                _uiBuilderService.checkBoxes[i].Checked = false;
            }
            waitForClear = false;
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
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.SubLineUpIndex, i].Enabled = true;
            }
            _uiBuilderService.subLineUpPanels[_ilineUpService.SubLineUpIndex].BackColor = Color.LightBlue;
        }

        /// <summary>
        /// 根据指定的Profession选择英雄
        /// </summary>
        /// <param name="profession"></param>
        private void SelectHerosFromProfession(Profession profession)
        {
            for (int i = profession.HeroDatas.Count - 1; i >= 0; i--)
            {
                _uiBuilderService.checkBoxes[_iheroDataService.HeroDatas.IndexOf(profession.HeroDatas[i])].Checked = true;
            }
        }

        /// <summary>
        /// 根据指定的Peculiarity选择英雄
        /// </summary>
        /// <param name="peculiarity"></param>
        private void SelectHerosFromPeculiarity(Peculiarity peculiarity)
        {
            for (int i = peculiarity.HeroDatas.Count - 1; i >= 0; i--)
            {
                _uiBuilderService.checkBoxes[_iheroDataService.HeroDatas.IndexOf(peculiarity.HeroDatas[i])].Checked = true;
            }
        }

        /// <summary>
        /// 通过CheckBox的勾选状态计算当前选中的奕子数量
        /// </summary>
        /// <returns></returns>
        private int CountOfCheckedHero()
        {
            int countOfSelected = 0;
            for (int i = 0; i < _iheroDataService.HeroDatas.Count; i++)
            {
                if (_uiBuilderService.checkBoxes[_iheroDataService.HeroDatas.IndexOf(_iheroDataService.HeroDatas[i])].Checked)
                {
                    countOfSelected++;
                }
            }
            return countOfSelected;
        }

        /// <summary>
        /// 从CheckBox勾选框状态加载图片到子阵容英雄头像框
        /// </summary>
        private void LoadImagesFromCheckBoxToSubLinePictureBoxes()
        {
            List<Image> tempList_SubLineUpImage = new List<Image>();
            for (int i = 0; i < _iheroDataService.HeroDatas.Count; i++)
            {
                if (_uiBuilderService.checkBoxes[_iheroDataService.HeroDatas.IndexOf(_iheroDataService.HeroDatas[i])].Checked)
                {
                    tempList_SubLineUpImage.Add(_iheroDataService.HeroDataToImageMap[_iheroDataService.HeroDatas[i]]);
                }
            }
            // 使用 LINQ 排序并处理无效项
            var sortedImages = tempList_SubLineUpImage
                .Where(img => _iheroDataService.ImageToHeroDataMap.ContainsKey(img)) // 过滤无效项（可选）
                .OrderBy(img => _iheroDataService.HeroDatas.IndexOf(_iheroDataService.ImageToHeroDataMap[img]))
                .ToList();

            // 直接替换原列表（或使用 images.Sort 原地排序）
            tempList_SubLineUpImage.Clear();
            tempList_SubLineUpImage.AddRange(sortedImages);

            ClearSubLinePictureBoxs();

            for (int i = 0; i < tempList_SubLineUpImage.Count; i++)
            {
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.SubLineUpIndex, i].Image = tempList_SubLineUpImage[i];
                _iheroDataService.ImageToHeroDataMap.TryGetValue(tempList_SubLineUpImage[i], out HeroData hero);
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.SubLineUpIndex, i].BorderColor = _uiBuilderService.GetColor(hero.Cost);
            }
        }

        /// <summary>
        /// 清空选中的子阵容英雄头像框
        /// </summary>
        private void ClearSubLinePictureBoxs()
        {
            for (int i = 0; i < _uiBuilderService.subLineUpPictureBoxes.GetLength(1); i++)
            {
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.SubLineUpIndex, i].Image = null;
                _uiBuilderService.subLineUpPictureBoxes[_ilineUpService.SubLineUpIndex, i].BorderColor = SystemColors.Control;
            }

        }

        /// <summary>
        /// 从阵容管理器读取阵容名称到下拉框
        /// </summary>
        private void LoadNameFromLineUpsToComboBox()
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < _ilineUpService.LineUps.Count; i++)
            {
                comboBox1.Items.Add(_ilineUpService.LineUps[i].Name);
            }
            if (comboBox1.Items.Count > _ilineUpService.LineUpIndex)
            {
                comboBox1.SelectedIndex = _ilineUpService.LineUpIndex;
            }

        }

        /// <summary>
        /// 从阵容管理器读取到单选框
        /// </summary>
        private void LoadCheckStateFromLineUpsToCheckBox()
        {
            TempClearCheckBox();
            for (int i = 0; i < _uiBuilderService.checkBoxes.Count; i++)
            {
                _uiBuilderService.checkBoxes[i].Checked = _ilineUpService.LineUps[_ilineUpService.LineUpIndex].Checked[_ilineUpService.SubLineUpIndex, i];
            }
        }

        /// <summary>
        /// 从阵容下拉框更新阵容数据服务对象中的阵容名称
        /// </summary>
        private void LoadNameFromComboBoxToLineUps()
        {
            for (int i = 0; i < _ilineUpService.LineUps.Count; i++)
            {
                _ilineUpService.LineUps[i].Name = comboBox1.Items[i].ToString();
            }
        }

        /// <summary>
        /// 从阵容数据服务对象读取阵容数据到三个子阵容英雄头像框，并且选择第一个子阵容
        /// </summary>
        private void LoadImagesFromLineUpsToSubLineUpPictureBoxesAndSelectTheFirstSubLineUp()
        {
            _ilineUpService.SubLineUpIndex = 2;
            ClearSubLinePictureBoxs();
            LoadCheckStateFromLineUpsToCheckBox();
            _ilineUpService.SubLineUpIndex = 1;
            ClearSubLinePictureBoxs();
            LoadCheckStateFromLineUpsToCheckBox();
            _ilineUpService.SubLineUpIndex = 0;
            ClearSubLinePictureBoxs();
            LoadCheckStateFromLineUpsToCheckBox();
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
        private void btnParseLineup_Click(object sender, EventArgs e)
        {
            string lineupCode = txtLineupCode.Text.Trim();
            if (string.IsNullOrEmpty(lineupCode))
            {
                MessageBox.Show("请输入阵容代码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                List<int> heroIds;

                // 智能判断：根据开头的特定文本，决定使用哪个解析器
                if (lineupCode.StartsWith("【阵容码】"))
                {
                    // 调用金铲铲解析器

                    heroIds = MobileLineUpParser.Parse(lineupCode);
                }
                else
                {

                    heroIds = LineUpParser.ParseCode(lineupCode);
                }

                // 统一处理结果
                if (heroIds != null && heroIds.Count > 0)
                {
                    // 调用UI更新方法
                    UpdateHeroSelection(heroIds);
                    MessageBox.Show($"成功解析出 {heroIds.Count} 位英雄并已自动勾选！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 根据英雄ID列表，更新界面上CheckBox的勾选状态
        /// </summary>
        /// <param name="heroIdsToShow">需要被勾选的英雄ID列表</param>
        private void UpdateHeroSelection(List<int> heroIdsToShow)
        {
            //清空当前选择子阵容的勾选框与阵容数据对象
            ClearCheckBox();
            try
            {
                // 遍历由UI构建服务管理的所有英雄复选框
                foreach (CheckBox chkHero in _uiBuilderService.checkBoxes)
                {
                    // 从Tag属性中获取完整的HeroData对象
                    HeroData hero = chkHero.Tag as HeroData;
                    if (hero != null)
                    {
                        // 检查当前英雄的ID是否存在于要显示的ID列表中
                        if (heroIdsToShow.Contains(hero.ChessId))
                        {
                            chkHero.Checked = true;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 阵容码文本框进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLineupCode_Enter(object sender, EventArgs e)
        {
            txtLineupCode.Text = "";
        }

        /// <summary>
        /// 阵容码文本块离开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLineupCode_Leave(object sender, EventArgs e)
        {
            if (txtLineupCode.Text == "")
            {
                txtLineupCode.Text = "请在此处粘贴阵容代码";
            }
        }

        private async void btnUpdateData_Click(object sender, EventArgs e)
        {
            // 1. 直接获取全局唯一的 EquipmentService 实例
            var equipmentService = EquipmentService.Instance;
            var progressForm = new JinChanChanTool.Forms.ProgressForm();

            // 2. 定义 IProgress 的行为 (这部分不变)
            var progress = new Progress<Tuple<int, string>>(update =>
            {
                progressForm.UpdateProgress(update.Item1, update.Item2);
            });

            // 将 sender 转换为 Button 类型，以便我们访问它的属性
            var button = sender as Button;
            if (button == null) return; // 如果转换失败，直接退出

            // 3. 从被点击的按钮自己的 Tag 属性中，读取存好的赛季路径
            string currentSeasonPath = button.Tag as string;

            // 检查是否成功获取到了路径
            if (string.IsNullOrEmpty(currentSeasonPath))
            {
                MessageBox.Show("错误：未能获取当前赛季的有效路径，无法进行更新。", "更新失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. 在调用更新前，用我们拿到的路径来“告知” EquipmentService
            equipmentService.LoadDataForSeason(currentSeasonPath);

            // 5. 禁用按钮，并显示进度窗口
            button.Enabled = false;
            progressForm.Show(this);

            // 6. 开始后台更新，并将 progress 对象传递进去
            bool updateSuccess = await equipmentService.UpdateDataFromWebAsync(progress);

            // 7. 更新完成后，关闭进度窗口并恢复按钮
            progressForm.Close();
            button.Enabled = true;

            // 8. 重启和清理逻辑
            if (updateSuccess)
            {
                DialogResult result = MessageBox.Show(
                    "数据更新成功！为了使新数据生效，软件需要重启。\n\n点击“确定”立即重启，并清理浏览器缓存文件。",
                    "更新完成",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    equipmentService.CleanUpBrowserCache();
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }
        #endregion
    }
}