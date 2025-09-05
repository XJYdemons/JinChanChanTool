using JinChanChanTool.Forms;
using JinChanChanTool.Tools.MouseTools;
using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Tools.KeyBoardTools;

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

            #region 小提示
            toolTip1.SetToolTip(pictureBox5, "当连接多台显示器时，该选项生效。\n选择截图目标（游戏窗口）所在的显示器。\n默认值：1 - \\\\.\\DISPLAY1");
            toolTip1.SetToolTip(pictureBox6, "需要填入6个数值，分别是X1-X5与Y。" +
                "\n程序使用OCR（光学字符识别）来匹配目标是否是指定奕子，因此需要对商店奕子的名称部分进行截图操作，这批数值决定了该在哪开始截图。" +
                                             "\nX1：代表商店从左到右数第1张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nX2：代表商店从左到右数第2张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nX3：代表商店从左到右数第3张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nX4：代表商店从左到右数第4张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nX5：代表商店从左到右数第5张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nY：代表商店所有（5张）奕子名称的截图起始点的纵坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。"
                                             );
            toolTip1.SetToolTip(pictureBox7, "宽和高的值表示从截图起点（图片左顶点）向右与向下分别截取的像素值。\n" +
                "宽：从截图起点向右截取图片的宽度。" +
                "\n高：从截图起点向下截取图片的高度。");

            toolTip1.SetToolTip(pictureBox10, "本程序刷新商店使用鼠标模拟移动点击的方式，因此我们要获取到鼠标移动的目标位置，即商店刷新\n按钮的中心坐标。（当然也可以是按钮的其他部位，不过中心部位是最不容易点击失误的地方）");
            #endregion

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
            comboBox1.Items.Clear();
            // 查询每个显示器的设备名称
            for (int i = 0; i < screens.Length; i++)
            {
                // 将显示器的序号和设备名称添加到显示器下拉框
                comboBox1.Items.Add($"{i + 1} - {screens[i].DeviceName}");
            }

            // 默认选择第一个显示器（如果有）
            if (screens.Length > 0)
            {
                comboBox1.SelectedIndex = 0;

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
            targetScreen = screens[comboBox1.SelectedIndex];
        }

        #endregion

        #region 通用组件方法与事件
        /// <summary>
        /// 更新所有文本框的显示内容为应用设置服务类中的值
        /// </summary>
        private void Update_AllComponents()
        {
            textBox1.Text = _iappConfigService.CurrentConfig.HotKey3;
            textBox2.Text = _iappConfigService.CurrentConfig.HotKey1;
            textBox3.Text = _iappConfigService.CurrentConfig.HotKey2;
            textBox19.Text = _iappConfigService.CurrentConfig.HotKey4;
            textBox4.Text = _iappConfigService.CurrentConfig.MaxOfChoices.ToString();
            textBox5.Text = _iappConfigService.CurrentConfig.CountOfLine.ToString();
            textBox8.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1.ToString();
            textBox9.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2.ToString();
            textBox10.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3.ToString();
            textBox11.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4.ToString();
            textBox12.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5.ToString();
            textBox13.Text = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY.ToString();
            textBox14.Text = _iappConfigService.CurrentConfig.Width_CardScreenshot.ToString();
            textBox15.Text = _iappConfigService.CurrentConfig.Height_CardScreenshot.ToString();
            textBox20.Text = _iappConfigService.CurrentConfig.Point_RefreshStoreX.ToString();
            textBox21.Text = _iappConfigService.CurrentConfig.Point_RefreshStoreY.ToString();
            checkBox1.Checked = _iappConfigService.CurrentConfig.HighCursorcontrol;
            checkBox2.Checked = _iappConfigService.CurrentConfig.AutoStopGet;
            checkBox3.Checked = _iappConfigService.CurrentConfig.AutoStopRefresh;
            radioButton1.Checked = _iappConfigService.CurrentConfig.MouseGetCard;
            radioButton2.Checked = _iappConfigService.CurrentConfig.KeyboardGetCard;
            radioButton4.Checked = _iappConfigService.CurrentConfig.MouseRefresh;
            radioButton3.Checked = _iappConfigService.CurrentConfig.KeyboardRefresh;
            radioButton6.Checked = _iappConfigService.CurrentConfig.UseCPU;
            radioButton5.Checked = _iappConfigService.CurrentConfig.UseGPU;
            textBox6.Text = _iappConfigService.CurrentConfig.GetCardKey1;
            textBox7.Text = _iappConfigService.CurrentConfig.GetCardKey2;
            textBox16.Text = _iappConfigService.CurrentConfig.GetCardKey3;
            textBox17.Text = _iappConfigService.CurrentConfig.GetCardKey4;
            textBox18.Text = _iappConfigService.CurrentConfig.GetCardKey5;
            textBox25.Text = _iappConfigService.CurrentConfig.RefreshKey;
        }

        /// <summary>
        /// 为所有文本框组件绑定事件
        /// </summary>
        private void Initialize_AllComponents()
        {
            textBox1.KeyDown += TextBox1_KeyDown;
            textBox1.Enter += TextBox_Enter;
            textBox1.Leave += TextBox_Leave;

            textBox2.KeyDown += TextBox2_KeyDown;
            textBox2.Enter += TextBox_Enter;
            textBox2.Leave += TextBox_Leave;

            textBox3.KeyDown += TextBox3_KeyDown;
            textBox3.Enter += TextBox_Enter;
            textBox3.Leave += TextBox_Leave;

            textBox19.KeyDown += TextBox19_KeyDown;
            textBox19.Enter += TextBox_Enter;
            textBox19.Leave += TextBox_Leave;

            textBox4.KeyDown += TextBox_KeyDown;
            textBox4.Enter += TextBox_Enter;
            textBox4.Leave += TextBox4_Leave;
            textBox5.KeyDown += TextBox_KeyDown;
            textBox5.Enter += TextBox_Enter;
            textBox5.Leave += TextBox5_Leave;
            textBox8.KeyDown += TextBox_KeyDown;
            textBox8.Enter += TextBox_Enter;
            textBox8.Leave += TextBox8_Leave;
            textBox9.KeyDown += TextBox_KeyDown;
            textBox9.Enter += TextBox_Enter;
            textBox9.Leave += TextBox9_Leave;
            textBox10.KeyDown += TextBox_KeyDown;
            textBox10.Enter += TextBox_Enter;
            textBox10.Leave += TextBox10_Leave;
            textBox11.KeyDown += TextBox_KeyDown;
            textBox11.Enter += TextBox_Enter;
            textBox11.Leave += TextBox11_Leave;
            textBox12.KeyDown += TextBox_KeyDown;
            textBox12.Enter += TextBox_Enter;
            textBox12.Leave += TextBox12_Leave;
            textBox13.KeyDown += TextBox_KeyDown;
            textBox13.Enter += TextBox_Enter;
            textBox13.Leave += TextBox13_Leave;
            textBox14.KeyDown += TextBox_KeyDown;
            textBox14.Enter += TextBox_Enter;
            textBox14.Leave += TextBox14_Leave;
            textBox15.KeyDown += TextBox_KeyDown;
            textBox15.Enter += TextBox_Enter;
            textBox15.Leave += TextBox15_Leave;
            textBox20.KeyDown += TextBox_KeyDown;
            textBox20.Enter += TextBox_Enter;
            textBox20.Leave += TextBox20_Leave;
            textBox21.KeyDown += TextBox_KeyDown;
            textBox21.Enter += TextBox_Enter;
            textBox21.Leave += TextBox21_Leave;
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            checkBox2.CheckedChanged += CheckBox2_CheckedChanged;
            checkBox3.CheckedChanged += CheckBox3_CheckedChanged;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            radioButton3.CheckedChanged += radioButton3_CheckedChanged;
            radioButton4.CheckedChanged += radioButton4_CheckedChanged;
            radioButton5.CheckedChanged += radioButton5_CheckedChanged;
            radioButton6.CheckedChanged += radioButton6_CheckedChanged;
            textBox6.KeyDown += TextBox6_KeyDown;            
            textBox6.Enter += TextBox_Enter;
            textBox6.Leave += TextBox_Leave;

            textBox7.KeyDown += TextBox7_KeyDown;
            textBox7.Enter += TextBox_Enter;
            textBox7.Leave += TextBox_Leave;

            textBox16.KeyDown += TextBox16_KeyDown;
            textBox16.Enter += TextBox_Enter;
            textBox16.Leave += TextBox_Leave;

            textBox17.KeyDown += TextBox17_KeyDown;
            textBox17.Enter += TextBox_Enter;
            textBox17.Leave += TextBox_Leave;

            textBox18.KeyDown += TextBox18_KeyDown;
            textBox18.Enter += TextBox_Enter;
            textBox18.Leave += TextBox_Leave;

            textBox25.KeyDown += TextBox25_KeyDown;
            textBox25.Enter += TextBox_Enter;
            textBox25.Leave += TextBox_Leave;
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
            if(GlobalHotkeyTool.IsRightKey(key))
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
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox4.Text);
                    if (result > 0 && result <= 60)
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
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox5.Text);
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
            if (string.IsNullOrWhiteSpace(textBox8.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1 = int.Parse(textBox8.Text);

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
            if (string.IsNullOrWhiteSpace(textBox9.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2 = int.Parse(textBox9.Text);

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
            if (string.IsNullOrWhiteSpace(textBox10.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3 = int.Parse(textBox10.Text);

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
            if (string.IsNullOrWhiteSpace(textBox11.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4 = int.Parse(textBox11.Text);

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
            if (string.IsNullOrWhiteSpace(textBox12.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5 = int.Parse(textBox12.Text);

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
            if (string.IsNullOrWhiteSpace(textBox13.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY = int.Parse(textBox13.Text);

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
            if (string.IsNullOrWhiteSpace(textBox14.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox14.Text);
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
            if (string.IsNullOrWhiteSpace(textBox15.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox15.Text);
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
            if (string.IsNullOrWhiteSpace(textBox20.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.Point_RefreshStoreX = int.Parse(textBox20.Text);

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
            if (string.IsNullOrWhiteSpace(textBox21.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.Point_RefreshStoreY = int.Parse(textBox21.Text);

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
            _iappConfigService.CurrentConfig.HighCursorcontrol = checkBox1.Checked;
        }
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.AutoStopGet = checkBox2.Checked;
        }
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.AutoStopRefresh = checkBox3.Checked;
        }
        #endregion

        #region 拿牌方式单选框改变
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox16.Enabled = false;
                textBox17.Enabled = false;
                textBox18.Enabled = false;
            }
            _iappConfigService.CurrentConfig.MouseGetCard = radioButton1.Checked;
        }


        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox16.Enabled = true;
                textBox17.Enabled = true;
                textBox18.Enabled = true;
            }
            _iappConfigService.CurrentConfig.KeyboardGetCard = radioButton2.Checked;
        }

        #endregion

        #region 刷新方式单选框改变
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                textBox25.Enabled = false;
            }
            _iappConfigService.CurrentConfig.MouseRefresh = radioButton4.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                textBox25.Enabled = true;
            }
            _iappConfigService.CurrentConfig.KeyboardRefresh = radioButton3.Checked;
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
            _iappConfigService.CurrentConfig.UseCPU = radioButton6.Checked;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {            
            _iappConfigService.CurrentConfig.UseGPU = radioButton5.Checked;
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

       
    }
}
