using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.MouseTools;
using System.Diagnostics;

namespace JinChanChanTool
{
    public partial class SettingForm : Form
    {
        /// <summary>
        /// 应用设置服务类实例，用于加载和保存应用设置。
        /// </summary>
        private readonly IAppConfigService _iappConfigService;

        /// <summary>
        /// 用于存储旧的应用设置以便比较
        /// </summary>
        private AppConfig oldAppConfig;
       
        public SettingForm(IAppConfigService _iAppConfigService)
        {
            InitializeComponent();
            // 添加自定义标题栏
            CustomTitleBar titleBar = new CustomTitleBar(this, 32, null, "设置", CustomTitleBar.ButtonOptions.Close | CustomTitleBar.ButtonOptions.Minimize);
            this.Controls.Add(titleBar);
            //隐藏图标
            this.ShowIcon = false;

            //初始化应用设置服务类实例
            _iappConfigService = _iAppConfigService;

            // 克隆当前设置以备后续比较
            oldAppConfig = (AppConfig)_iappConfigService.CurrentConfig.Clone();

            //为组件绑定事件
            Initialize_AllComponents();

            //初始化显示文本
            Update_AllComponents();

            // 获取所有连接的显示器  
            screens = Screen.AllScreens;

            //加载显示器到下拉框并默认选中第一个显示器 
            LoadDisplays();
        }

        /// <summary>
        /// 窗体关闭时触发 ——> 检查是否有未保存的设置
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 检查设置是否已修改
            if (!IsSaved())
            {
                var result = MessageBox.Show(
                    "您有未保存的设置，是否要保存？",
                    "未保存的设置",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    oldAppConfig = (AppConfig)_iappConfigService.CurrentConfig.Clone();
                    _iappConfigService.Save();
                }
                else if (result == DialogResult.Cancel)
                {
                    // 取消关闭操作
                    e.Cancel = true;
                    return;
                }
                else
                {
                    // 如果选择"No"，则不保存直接关闭
                    _iappConfigService.CurrentConfig = oldAppConfig.Clone() as AppConfig;
                }

            }
            GlobalHotkeyTool.Enabled = true;
            base.OnFormClosing(e);
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

            // 默认选择第一个显示器（如果有）
            if (screens.Length > 0)
            {
                comboBox_选择显示器.SelectedIndex = 0;

                targetScreen = screens[0];
            }
        }

        /// <summary>
        /// 选择显示器下拉框选择项改变时触发 ——> targetScreen值更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            targetScreen = screens[comboBox_选择显示器.SelectedIndex];
        }

        #endregion

        #region 通用组件方法与事件
        /// <summary>
        /// 更新所有文本框的显示内容为应用设置服务类中的值
        /// </summary>
        private void Update_AllComponents()
        {
            textBox_召出隐藏窗口快捷键.Text = _iappConfigService.CurrentConfig.HotKey3;
            textBox_自动拿牌快捷键.Text = _iappConfigService.CurrentConfig.HotKey1;
            textBox_自动刷新商店快捷键.Text = _iappConfigService.CurrentConfig.HotKey2;
            textBox_长按自动D牌快捷键.Text = _iappConfigService.CurrentConfig.HotKey4;
            radioButton_手动设置坐标.Checked = _iappConfigService.CurrentConfig.UseFixedCoordinates;
            radioButton_自动设置坐标.Checked = _iappConfigService.CurrentConfig.UseDynamicCoordinates;
            textBox_最大选择数量.Text = _iappConfigService.CurrentConfig.MaxOfChoices.ToString();
            textBox_最大阵容数量.Text = _iappConfigService.CurrentConfig.CountOfLine.ToString();
            textBox_拿牌坐标X1.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1.ToString();
            textBox_拿牌坐标X2.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2.ToString();
            textBox_拿牌坐标X3.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3.ToString();
            textBox_拿牌坐标X4.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4.ToString();
            textBox_拿牌坐标X5.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5.ToString();
            textBox_拿牌坐标Y.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY.ToString();
            textBox_奕子截图宽度.Text = _iappConfigService.CurrentConfig.Width_CardScreenshot.ToString();
            textBox_奕子截图高度.Text = _iappConfigService.CurrentConfig.Height_CardScreenshot.ToString();
            textBox_商店刷新按钮坐标X.Text = _iappConfigService.CurrentConfig.Point_RefreshStoreX.ToString();
            textBox_商店刷新按钮坐标Y.Text = _iappConfigService.CurrentConfig.Point_RefreshStoreY.ToString();
            checkBox_避免程序与用户争夺光标控制权.Checked = _iappConfigService.CurrentConfig.HighCursorcontrol;
            checkBox_备战席满或金币不足时自动停止拿牌.Checked = _iappConfigService.CurrentConfig.AutoStopGet;
            checkBox_自动停止刷新商店.Checked = _iappConfigService.CurrentConfig.AutoStopRefresh;
            radioButton_鼠标模拟拿牌.Checked = _iappConfigService.CurrentConfig.MouseGetCard;
            radioButton_按键模拟拿牌.Checked = _iappConfigService.CurrentConfig.KeyboardGetCard;
            radioButton_鼠标模拟刷新商店.Checked = _iappConfigService.CurrentConfig.MouseRefresh;
            radioButton_按键模拟刷新商店.Checked = _iappConfigService.CurrentConfig.KeyboardRefresh;
            radioButton_CPU推理.Checked = _iappConfigService.CurrentConfig.UseCPU;
            radioButton__CPU推理.Checked = _iappConfigService.CurrentConfig.UseGPU;
            textBox_拿牌按键1.Text = _iappConfigService.CurrentConfig.GetCardKey1;
            textBox_拿牌按键2.Text = _iappConfigService.CurrentConfig.GetCardKey2;
            textBox_拿牌按键3.Text = _iappConfigService.CurrentConfig.GetCardKey3;
            textBox_拿牌按键4.Text = _iappConfigService.CurrentConfig.GetCardKey4;
            textBox_拿牌按键5.Text = _iappConfigService.CurrentConfig.GetCardKey5;
            textBox_刷新商店按键.Text = _iappConfigService.CurrentConfig.RefreshKey;
        }

        /// <summary>
        /// 为所有文本框组件绑定事件
        /// </summary>
        private void Initialize_AllComponents()
        {
            textBox_召出隐藏窗口快捷键.KeyDown += TextBox1_KeyDown;
            textBox_召出隐藏窗口快捷键.Enter += TextBox_Enter;
            textBox_召出隐藏窗口快捷键.Leave += TextBox_Leave;

            textBox_自动拿牌快捷键.KeyDown += TextBox2_KeyDown;
            textBox_自动拿牌快捷键.Enter += TextBox_Enter;
            textBox_自动拿牌快捷键.Leave += TextBox_Leave;

            textBox_自动刷新商店快捷键.KeyDown += TextBox3_KeyDown;
            textBox_自动刷新商店快捷键.Enter += TextBox_Enter;
            textBox_自动刷新商店快捷键.Leave += TextBox_Leave;

            textBox_长按自动D牌快捷键.KeyDown += TextBox19_KeyDown;
            textBox_长按自动D牌快捷键.Enter += TextBox_Enter;
            textBox_长按自动D牌快捷键.Leave += TextBox_Leave;

            radioButton_手动设置坐标.CheckedChanged += radioButton_手动设置坐标_CheckedChanged;

            radioButton_自动设置坐标.CheckedChanged += radioButton_自动设置坐标_CheckedChanged;

            textBox_最大选择数量.KeyDown += TextBox_KeyDown;
            textBox_最大选择数量.Enter += TextBox_Enter;
            textBox_最大选择数量.Leave += TextBox4_Leave;
            textBox_最大阵容数量.KeyDown += TextBox_KeyDown;
            textBox_最大阵容数量.Enter += TextBox_Enter;
            textBox_最大阵容数量.Leave += TextBox5_Leave;
            textBox_拿牌坐标X1.KeyDown += TextBox_KeyDown;
            textBox_拿牌坐标X1.Enter += TextBox_Enter;
            textBox_拿牌坐标X1.Leave += TextBox8_Leave;
            textBox_拿牌坐标X2.KeyDown += TextBox_KeyDown;
            textBox_拿牌坐标X2.Enter += TextBox_Enter;
            textBox_拿牌坐标X2.Leave += TextBox9_Leave;
            textBox_拿牌坐标X3.KeyDown += TextBox_KeyDown;
            textBox_拿牌坐标X3.Enter += TextBox_Enter;
            textBox_拿牌坐标X3.Leave += TextBox10_Leave;
            textBox_拿牌坐标X4.KeyDown += TextBox_KeyDown;
            textBox_拿牌坐标X4.Enter += TextBox_Enter;
            textBox_拿牌坐标X4.Leave += TextBox11_Leave;
            textBox_拿牌坐标X5.KeyDown += TextBox_KeyDown;
            textBox_拿牌坐标X5.Enter += TextBox_Enter;
            textBox_拿牌坐标X5.Leave += TextBox12_Leave;
            textBox_拿牌坐标Y.KeyDown += TextBox_KeyDown;
            textBox_拿牌坐标Y.Enter += TextBox_Enter;
            textBox_拿牌坐标Y.Leave += TextBox13_Leave;
            textBox_奕子截图宽度.KeyDown += TextBox_KeyDown;
            textBox_奕子截图宽度.Enter += TextBox_Enter;
            textBox_奕子截图宽度.Leave += TextBox14_Leave;
            textBox_奕子截图高度.KeyDown += TextBox_KeyDown;
            textBox_奕子截图高度.Enter += TextBox_Enter;
            textBox_奕子截图高度.Leave += TextBox15_Leave;
            textBox_商店刷新按钮坐标X.KeyDown += TextBox_KeyDown;
            textBox_商店刷新按钮坐标X.Enter += TextBox_Enter;
            textBox_商店刷新按钮坐标X.Leave += TextBox20_Leave;
            textBox_商店刷新按钮坐标Y.KeyDown += TextBox_KeyDown;
            textBox_商店刷新按钮坐标Y.Enter += TextBox_Enter;
            textBox_商店刷新按钮坐标Y.Leave += TextBox21_Leave;
            checkBox_避免程序与用户争夺光标控制权.CheckedChanged += CheckBox1_CheckedChanged;
            checkBox_备战席满或金币不足时自动停止拿牌.CheckedChanged += CheckBox2_CheckedChanged;
            checkBox_自动停止刷新商店.CheckedChanged += CheckBox3_CheckedChanged;
            radioButton_鼠标模拟拿牌.CheckedChanged += radioButton1_CheckedChanged;
            radioButton_按键模拟拿牌.CheckedChanged += radioButton2_CheckedChanged;
            radioButton_按键模拟刷新商店.CheckedChanged += radioButton3_CheckedChanged;
            radioButton_鼠标模拟刷新商店.CheckedChanged += radioButton4_CheckedChanged;
            radioButton__CPU推理.CheckedChanged += radioButton5_CheckedChanged;
            radioButton_CPU推理.CheckedChanged += radioButton6_CheckedChanged;
            textBox_拿牌按键1.KeyDown += TextBox6_KeyDown;
            textBox_拿牌按键1.Enter += TextBox_Enter;
            textBox_拿牌按键1.Leave += TextBox_Leave;

            textBox_拿牌按键2.KeyDown += TextBox7_KeyDown;
            textBox_拿牌按键2.Enter += TextBox_Enter;
            textBox_拿牌按键2.Leave += TextBox_Leave;

            textBox_拿牌按键3.KeyDown += TextBox16_KeyDown;
            textBox_拿牌按键3.Enter += TextBox_Enter;
            textBox_拿牌按键3.Leave += TextBox_Leave;

            textBox_拿牌按键4.KeyDown += TextBox17_KeyDown;
            textBox_拿牌按键4.Enter += TextBox_Enter;
            textBox_拿牌按键4.Leave += TextBox_Leave;

            textBox_拿牌按键5.KeyDown += TextBox18_KeyDown;
            textBox_拿牌按键5.Enter += TextBox_Enter;
            textBox_拿牌按键5.Leave += TextBox_Leave;

            textBox_刷新商店按键.KeyDown += TextBox25_KeyDown;
            textBox_刷新商店按键.Enter += TextBox_Enter;
            textBox_刷新商店按键.Leave += TextBox_Leave;
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

        #region 修改-召出隐藏窗口快捷键-逻辑

        /// <summary>
        /// 当TextBox1为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (GlobalHotkeyTool.IsRightKey(key))
            {
                if ((key.ToString() != _iappConfigService.CurrentConfig.HotKey1) && (key.ToString() != _iappConfigService.CurrentConfig.HotKey2) && (key.ToString() != _iappConfigService.CurrentConfig.HotKey4))
                {
                    _iappConfigService.CurrentConfig.HotKey3 = key.ToString();
                    Update_AllComponents();
                }

                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音
            }

            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion

        #region 修改-自动拿牌快捷键-逻辑      
        /// <summary>
        /// 当TextBox2为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {

            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (GlobalHotkeyTool.IsRightKey(key))
            {
                if ((key.ToString() != _iappConfigService.CurrentConfig.HotKey2) && (key.ToString() != _iappConfigService.CurrentConfig.HotKey3) && (key.ToString() != _iappConfigService.CurrentConfig.HotKey4))
                {
                    _iappConfigService.CurrentConfig.HotKey1 = key.ToString();
                    Update_AllComponents();
                }

                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音  

            }

            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }
        #endregion

        #region 修改-自动刷新商店快捷键-逻辑
        /// <summary>
        /// 当TextBox3为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox3_KeyDown(object sender, KeyEventArgs e)
        {

            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (GlobalHotkeyTool.IsRightKey(key))
            {
                if ((key.ToString() != _iappConfigService.CurrentConfig.HotKey1) && (key.ToString() != _iappConfigService.CurrentConfig.HotKey3) && (key.ToString() != _iappConfigService.CurrentConfig.HotKey4))
                {
                    _iappConfigService.CurrentConfig.HotKey2 = key.ToString();
                    Update_AllComponents();
                }

                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }

            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }
        #endregion

        #region 修改-长按自动D牌快捷键-逻辑
        /// <summary>
        /// 当TextBox19为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox19_KeyDown(object sender, KeyEventArgs e)
        {

            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (GlobalHotkeyTool.IsRightKey(key))
            {
                if ((key.ToString() != _iappConfigService.CurrentConfig.HotKey1) && (key.ToString() != _iappConfigService.CurrentConfig.HotKey3) && (key.ToString() != _iappConfigService.CurrentConfig.HotKey2))
                {
                    _iappConfigService.CurrentConfig.HotKey4 = key.ToString();
                    Update_AllComponents();
                }

                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }
        #endregion

        #region 修改-最大选择数量
        /// <summary>
        /// 离开TextBox4时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox4_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_最大选择数量.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox_最大选择数量.Text);
                    if (result > 0 && result <= 100)
                    {
                        _iappConfigService.CurrentConfig.MaxOfChoices = result;
                    }
                    Update_AllComponents();

                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-最大阵容数量
        /// <summary>
        /// 离开TextBox5时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox5_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_最大阵容数量.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox_最大阵容数量.Text);
                    if (result > 0 && result <= 100)
                    {
                        _iappConfigService.CurrentConfig.CountOfLine = result;
                    }
                    Update_AllComponents();

                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-奕子起点坐标X1-逻辑
        /// <summary>
        /// 离开TextBox8时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox8_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_拿牌坐标X1.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1 = int.Parse(textBox_拿牌坐标X1.Text);

                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-奕子起点坐标X2-逻辑
        /// <summary>
        /// 离开TextBox9时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox9_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_拿牌坐标X2.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2 = int.Parse(textBox_拿牌坐标X2.Text);

                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-奕子起点坐标X3-逻辑
        /// <summary>
        /// 离开TextBox10时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox10_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_拿牌坐标X3.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3 = int.Parse(textBox_拿牌坐标X3.Text);

                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-奕子起点坐标X4-逻辑
        /// <summary>
        /// 离开TextBox11时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox11_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_拿牌坐标X4.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4 = int.Parse(textBox_拿牌坐标X4.Text);

                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-奕子起点坐标X5-逻辑
        /// <summary>
        /// 离开TextBox12时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox12_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_拿牌坐标X5.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5 = int.Parse(textBox_拿牌坐标X5.Text);

                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-奕子起点坐标Y-逻辑
        /// <summary>
        /// 离开TextBox13时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox13_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_拿牌坐标Y.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY = int.Parse(textBox_拿牌坐标Y.Text);

                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-奕子截图宽度-逻辑
        /// <summary>
        /// 离开TextBox14时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox14_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_奕子截图宽度.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox_奕子截图宽度.Text);
                    if (result > 0)
                    {
                        _iappConfigService.CurrentConfig.Width_CardScreenshot = result;
                    }
                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-奕子截图高度-逻辑
        /// <summary>
        /// 离开TextBox15时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox15_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_奕子截图高度.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox_奕子截图高度.Text);
                    if (result > 0)
                    {
                        _iappConfigService.CurrentConfig.Height_CardScreenshot = result;
                    }


                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-商店刷新按钮坐标X-逻辑
        /// <summary>
        /// 离开TextBox20时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox20_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_商店刷新按钮坐标X.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.Point_RefreshStoreX = int.Parse(textBox_商店刷新按钮坐标X.Text);

                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-商店刷新按钮坐标Y-逻辑
        /// <summary>
        /// 离开TextBox21时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox21_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_商店刷新按钮坐标Y.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.Point_RefreshStoreY = int.Parse(textBox_商店刷新按钮坐标Y.Text);

                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 修改-单选框-逻辑
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.HighCursorcontrol = checkBox_避免程序与用户争夺光标控制权.Checked;
        }
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.AutoStopGet = checkBox_备战席满或金币不足时自动停止拿牌.Checked;
        }
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.AutoStopRefresh = checkBox_自动停止刷新商店.Checked;
        }
        #endregion

        #region 拿牌方式单选框改变
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_鼠标模拟拿牌.Checked)
            {
                textBox_拿牌按键1.Enabled = false;
                textBox_拿牌按键2.Enabled = false;
                textBox_拿牌按键3.Enabled = false;
                textBox_拿牌按键4.Enabled = false;
                textBox_拿牌按键5.Enabled = false;
            }
            _iappConfigService.CurrentConfig.MouseGetCard = radioButton_鼠标模拟拿牌.Checked;
        }


        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_按键模拟拿牌.Checked)
            {
                textBox_拿牌按键1.Enabled = true;
                textBox_拿牌按键2.Enabled = true;
                textBox_拿牌按键3.Enabled = true;
                textBox_拿牌按键4.Enabled = true;
                textBox_拿牌按键5.Enabled = true;
            }
            _iappConfigService.CurrentConfig.KeyboardGetCard = radioButton_按键模拟拿牌.Checked;
        }

        #endregion

        #region 刷新方式单选框改变
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_鼠标模拟刷新商店.Checked)
            {
                textBox_刷新商店按键.Enabled = false;
            }
            _iappConfigService.CurrentConfig.MouseRefresh = radioButton_鼠标模拟刷新商店.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_按键模拟刷新商店.Checked)
            {
                textBox_刷新商店按键.Enabled = true;
            }
            _iappConfigService.CurrentConfig.KeyboardRefresh = radioButton_按键模拟刷新商店.Checked;
        }
        #endregion

        #region 修改-按键模拟拿牌-按键1-逻辑

        /// <summary>
        /// 当TextBox6为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox6_KeyDown(object sender, KeyEventArgs e)
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
                _iappConfigService.CurrentConfig.GetCardKey1 = key.ToString();
                Update_AllComponents();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion

        #region 修改-按键模拟拿牌-按键2-逻辑

        /// <summary>
        /// 当TextBox7为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox7_KeyDown(object sender, KeyEventArgs e)
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
                _iappConfigService.CurrentConfig.GetCardKey2 = key.ToString();
                Update_AllComponents();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion

        #region 修改-按键模拟拿牌-按键3-逻辑

        /// <summary>
        /// 当TextBox16为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox16_KeyDown(object sender, KeyEventArgs e)
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
                _iappConfigService.CurrentConfig.GetCardKey3 = key.ToString();
                Update_AllComponents();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion

        #region 修改-按键模拟拿牌-按键4-逻辑

        /// <summary>
        /// 当TextBox17为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox17_KeyDown(object sender, KeyEventArgs e)
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
                _iappConfigService.CurrentConfig.GetCardKey4 = key.ToString();
                Update_AllComponents();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion

        #region 修改-按键模拟拿牌-按键5-逻辑

        /// <summary>
        /// 当TextBox18为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox18_KeyDown(object sender, KeyEventArgs e)
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
                _iappConfigService.CurrentConfig.GetCardKey5 = key.ToString();
                Update_AllComponents();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion

        #region 修改-按键刷新商店按键-逻辑

        /// <summary>
        /// 当TextBox25为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox25_KeyDown(object sender, KeyEventArgs e)
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
                _iappConfigService.CurrentConfig.RefreshKey = key.ToString();
                Update_AllComponents();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion

        #region 推理单选框改变
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.UseCPU = radioButton_CPU推理.Checked;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.UseGPU = radioButton__CPU推理.Checked;
        }

        #endregion

        #region 坐标设置方式单选框改变
        private void radioButton_手动设置坐标_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_手动设置坐标.Checked)
            {
                button_选择进程.Enabled = false;
                comboBox_选择显示器.Enabled = true;
                button_快速设置奕子截图坐标与大小.Enabled = true;
                textBox_拿牌坐标X1.Enabled = true;
                textBox_拿牌坐标X2.Enabled = true;
                textBox_拿牌坐标X3.Enabled = true;
                textBox_拿牌坐标X4.Enabled = true;
                textBox_拿牌坐标X5.Enabled = true;
                textBox_拿牌坐标Y.Enabled = true;
                textBox_奕子截图宽度.Enabled = true;
                textBox_奕子截图高度.Enabled = true;
                button_快速设置商店刷新按钮坐标.Enabled = true;
                textBox_商店刷新按钮坐标X.Enabled = true;
                textBox_商店刷新按钮坐标Y.Enabled = true;
            }
            _iappConfigService.CurrentConfig.UseFixedCoordinates = radioButton_手动设置坐标.Checked;
        }


        private void radioButton_自动设置坐标_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_自动设置坐标.Checked)
            {
                button_选择进程.Enabled = true;
                comboBox_选择显示器.Enabled = false;
                button_快速设置奕子截图坐标与大小.Enabled = false;
                textBox_拿牌坐标X1.Enabled = false;
                textBox_拿牌坐标X2.Enabled = false;
                textBox_拿牌坐标X3.Enabled = false;
                textBox_拿牌坐标X4.Enabled = false;
                textBox_拿牌坐标X5.Enabled = false;
                textBox_拿牌坐标Y.Enabled = false;
                textBox_奕子截图宽度.Enabled = false;
                textBox_奕子截图高度.Enabled = false;
                button_快速设置商店刷新按钮坐标.Enabled = false;
                textBox_商店刷新按钮坐标X.Enabled = false;
                textBox_商店刷新按钮坐标Y.Enabled = false;
            }
            _iappConfigService.CurrentConfig.UseDynamicCoordinates = radioButton_自动设置坐标.Checked;
        }

        #endregion

        #region 快速设置坐标      
        /// <summary>
        /// 快速设置奕子截图坐标与大小按钮_被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button3_Click(object sender, EventArgs e)
        {
            using (var setter = new FastSettingPositionService(targetScreen))
            {
                try
                {
                    // 第一张卡片
                    var rect1 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第1张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1 = rect1.X;

                    // 第二张卡片
                    var rect2 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第2张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2 = rect2.X;

                    // 第三张卡片
                    var rect3 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第3张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3 = rect3.X;

                    // 第四张卡片
                    var rect4 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第4张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4 = rect4.X;

                    // 第五张卡片（同时获取高度）
                    var rect5 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第5张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5 = rect5.X;
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY = rect5.Y;

                    if (rect5.Width > 0 && rect5.Height > 0)
                    {
                        _iappConfigService.CurrentConfig.Width_CardScreenshot = rect5.Width;
                        _iappConfigService.CurrentConfig.Height_CardScreenshot = rect5.Height;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现错误: {ex.Message}");
                }
            }
            Update_AllComponents();
        }



        /// <summary>
        /// 快速设置商店刷新按钮坐标按钮_被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button5_Click(object sender, EventArgs e)
        {
            using (var setter = new FastSettingPositionService(targetScreen))
            {
                try
                {
                    var point = await setter.WaitForClickAsync("请点击商店刷新按钮的中心点");
                    _iappConfigService.CurrentConfig.Point_RefreshStoreX = point.X;
                    _iappConfigService.CurrentConfig.Point_RefreshStoreY = point.Y;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现错误: {ex.Message}");
                }
            }
            Update_AllComponents();
        }

        #endregion

        #region 设置存取相关

        /// <summary>
        /// /保存设置按钮_被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("已保存设置到本地！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            oldAppConfig = (AppConfig)_iappConfigService.CurrentConfig.Clone();
            _iappConfigService.Save();

        }

        /// <summary>
        /// 还原默认设置按钮_被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("确定要恢复默认设置吗？", "确认恢复默认设置", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult != DialogResult.Yes)
            {
                return; // 用户取消操作
            }
            _iappConfigService.SetDefaultConfig();
            oldAppConfig = (AppConfig)_iappConfigService.CurrentConfig.Clone();
            Update_AllComponents();
            _iappConfigService.Save();
        }

        /// <summary>
        /// 判断当前设置是否已保存
        /// </summary>
        /// <returns></returns>
        private bool IsSaved()
        {
            AppConfig current = _iappConfigService.CurrentConfig;
            AppConfig original = oldAppConfig;
            return current.Equals(original);
        }
        #endregion

        #region 打开OCR纠正列表编辑器
        private void button4_Click(object sender, EventArgs e)
        {
            var form = new CorrectionEditorForm();
            form.Owner = this;// 设置父窗口，这样配置窗口会显示在主窗口上方但不会阻止主窗口                  
            form.TopMost = true;// 确保窗口在最前面
            form.Show();// 显示窗口
        }
        #endregion

        #region 打开英雄英雄配置文件编辑器
        private void button6_Click_1(object sender, EventArgs e)
        {
            var form = new HeroInfoEditorForm();
            form.Owner = this;// 设置父窗口，这样配置窗口会显示在主窗口上方但不会阻止主窗口                  
            form.TopMost = true;// 确保窗口在最前面
            form.Show();// 显示窗口
        }
        #endregion

        private void SettingForm_Load(object sender, EventArgs e)
        {

        }

        #region 选择进程
        private void button_选择进程_Click(object sender, EventArgs e)
        {
            // 1. 实时创建进程发现服务
            var discoveryService = new ProcessDiscoveryService();

            // 2. 创建并显示进程选择窗体
            using (var processForm = new ProcessSelectorForm(discoveryService))
            {
                if (processForm.ShowDialog(this) == DialogResult.OK)
                {
                    var selectedProcess = processForm.SelectedProcess;
                    if (selectedProcess != null)
                    {                      
                        // 同时保存 Name 和 ID
                        _iappConfigService.CurrentConfig.TargetProcessName = selectedProcess.ProcessName;
                        _iappConfigService.CurrentConfig.TargetProcessId = selectedProcess.Id;

                        // 4. 给用户反馈
                        string displayName = $"{selectedProcess.ProcessName} (ID: {selectedProcess.Id})";
                        MessageBox.Show($"已选择进程：{displayName}\n请记得点击“保存设置”来保存此选择。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        #endregion
    }
}
