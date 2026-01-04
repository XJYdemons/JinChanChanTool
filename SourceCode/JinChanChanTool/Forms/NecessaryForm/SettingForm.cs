using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms;
using JinChanChanTool.Services;
using JinChanChanTool.Services.AutoSetCoordinates;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Services.LineupCrawling;

using JinChanChanTool.Services.ManuallySetCoordinates;
using JinChanChanTool.Services.RecommendedEquipment;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using JinChanChanTool.Tools;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.MouseTools;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace JinChanChanTool
{
    public partial class SettingForm : Form
    {
        /// <summary>
        /// 应用设置服务类实例，用于加载和保存应用设置。
        /// </summary>
        private readonly IManualSettingsService _iappConfigService;

        /// <summary>
        /// 推荐阵容数据服务实例
        /// </summary>
        private readonly IRecommendedLineUpService _iRecommendedLineUpService;

        private Screen targetScreen;//目标显示器
        private Screen[] screens;//显示器数组

        public SettingForm(IManualSettingsService iAppConfigService, IRecommendedLineUpService iRecommendedLineUpService)
        {
            InitializeComponent();
            DragHelper.EnableDragForChildren(panel3);

            ////隐藏图标
            //this.ShowIcon = false;

            // 获取所有连接的显示器  
            screens = Screen.AllScreens;
            //加载显示器到下拉框并按用户设置文件选中对应显示器
            LoadDisplays();

            //初始化应用设置服务类实例
            _iappConfigService = iAppConfigService;

            _iRecommendedLineUpService = iRecommendedLineUpService;
            //为组件绑定事件
            Initialize_AllComponents();

            //初始化显示文本
            Update_AllComponents();
        }

        /// <summary>
        /// 窗体关闭时触发 ——> 检查是否有未保存的设置
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 检查设置是否已修改
            if (_iappConfigService.IsChanged())
            {
                var result = MessageBox.Show(
                    "您有未保存的设置，是否要保存？",
                    "未保存的设置",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _iappConfigService.Save(true);
                }
                else if (result == DialogResult.Cancel)
                {
                    // 取消关闭操作
                    e.Cancel = true;
                    return;
                }
                else
                {
                    _iappConfigService.ReLoad();
                }

            }
            GlobalHotkeyTool.Enabled = true;
            base.OnFormClosing(e);
        }

        #region 显示器相关逻辑        
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
        }

        /// <summary>
        /// 选择显示器下拉框选择项改变时触发 ——> targetScreen值更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            targetScreen = screens[comboBox_选择显示器.SelectedIndex];
            _iappConfigService.CurrentConfig.SelectedScreenIndex = comboBox_选择显示器.SelectedIndex;
        }

        #endregion

        #region 通用组件方法与事件
        /// <summary>
        /// 更新所有文本框的显示内容为应用设置服务类中的值
        /// </summary>
        private void Update_AllComponents()
        {
            if (comboBox_选择显示器.Items.Count > _iappConfigService.CurrentConfig.SelectedScreenIndex)
            {
                comboBox_选择显示器.SelectedIndex = _iappConfigService.CurrentConfig.SelectedScreenIndex;
            }
            textBox_召出隐藏窗口快捷键.Text = _iappConfigService.CurrentConfig.HotKey3;
            textBox_自动拿牌快捷键.Text = _iappConfigService.CurrentConfig.HotKey1;
            textBox_自动刷新商店快捷键.Text = _iappConfigService.CurrentConfig.HotKey2;
            textBox_长按自动D牌快捷键.Text = _iappConfigService.CurrentConfig.HotKey4;
            textBox_高亮提示.Text = _iappConfigService.CurrentConfig.HotKey5;
            radioButton_手动设置坐标.Checked = _iappConfigService.CurrentConfig.IsUseFixedCoordinates;
            radioButton_自动设置坐标.Checked = _iappConfigService.CurrentConfig.IsUseDynamicCoordinates;

            capsuleSwitch1.IsOn = _iappConfigService.CurrentConfig.IsHighUserPriority;

            capsuleSwitch4.IsOn = _iappConfigService.CurrentConfig.IsAutomaticStopHeroPurchase;
            capsuleSwitch7.IsOn = _iappConfigService.CurrentConfig.IsAutomaticStopRefreshStore;
            capsuleSwitch8.IsOn = _iappConfigService.CurrentConfig.IsStopRefreshStoreWhenErrorCharacters;


            capsuleSwitch2.IsOn = _iappConfigService.CurrentConfig.IsMouseHeroPurchase;



            capsuleSwitch3.IsOn = _iappConfigService.CurrentConfig.IsKeyboardHeroPurchase;


            capsuleSwitch6.IsOn = _iappConfigService.CurrentConfig.IsMouseRefreshStore;
            capsuleSwitch5.IsOn = _iappConfigService.CurrentConfig.IsKeyboardRefreshStore;

            capsuleSwitch10.IsOn = _iappConfigService.CurrentConfig.IsUseCPUForInference;
            capsuleSwitch9.IsOn = _iappConfigService.CurrentConfig.IsUseGPUForInference;

            textBox_拿牌按键1.Text = _iappConfigService.CurrentConfig.HeroPurchaseKey1;
            textBox_拿牌按键2.Text = _iappConfigService.CurrentConfig.HeroPurchaseKey2;
            textBox_拿牌按键3.Text = _iappConfigService.CurrentConfig.HeroPurchaseKey3;
            textBox_拿牌按键4.Text = _iappConfigService.CurrentConfig.HeroPurchaseKey4;
            textBox_拿牌按键5.Text = _iappConfigService.CurrentConfig.HeroPurchaseKey5;
            textBox_刷新商店按键.Text = _iappConfigService.CurrentConfig.RefreshStoreKey;
            textBox_MaxTimesWithoutGetCard.Text = _iappConfigService.CurrentConfig.MaxTimesWithoutHeroPurchase.ToString();
            textBox_MaxTimesWithoutRefresh.Text = _iappConfigService.CurrentConfig.MaxTimesWithoutRefreshStore.ToString();
            textBox_DelayAfterMouseOperation.Text = _iappConfigService.CurrentConfig.DelayAfterOperation.ToString();
            textBox_CPUDelayAfterRefreshStore.Text = _iappConfigService.CurrentConfig.DelayAfterRefreshStore_CPU.ToString();
            textBox_GPUDelayAfterRefreshStore.Text = _iappConfigService.CurrentConfig.DelayAfterRefreshStore_GPU.ToString();
            capsuleSwitch11.IsOn = _iappConfigService.CurrentConfig.IsUseSelectForm;
            capsuleSwitch12.IsOn = _iappConfigService.CurrentConfig.IsUseLineUpForm;
            capsuleSwitch13.IsOn = _iappConfigService.CurrentConfig.IsUseStatusOverlayForm;
            capsuleSwitch14.IsOn = _iappConfigService.CurrentConfig.IsUseOutputForm;
            capsuleSwitch15.IsOn = _iappConfigService.CurrentConfig.IsAutomaticUpdateEquipment;
            textBox_更新推荐装备间隔.Text = _iappConfigService.CurrentConfig.UpdateEquipmentInterval.ToString();
            textBox_英雄头像框边长.Text = _iappConfigService.CurrentConfig.SelectFormHeroPictureBoxSize.ToString();
            textBox_英雄头像框水平间隔.Text = _iappConfigService.CurrentConfig.SelectFormHeroPictureBoxHorizontalSpacing.ToString();
            textBox_英雄头像框垂直间隔.Text = _iappConfigService.CurrentConfig.SelectFormHeroPanelsVerticalSpacing.ToString();
            capsuleSwitch16.IsOn = _iappConfigService.CurrentConfig.IsFilterLetters;
            capsuleSwitch17.IsOn = _iappConfigService.CurrentConfig.IsFilterNumbers;
        }

        /// <summary>
        /// 为所有文本框组件绑定事件
        /// </summary>
        private void Initialize_AllComponents()
        {
            textBox_召出隐藏窗口快捷键.KeyDown += textBox_召出隐藏窗口快捷键_KeyDown;
            textBox_召出隐藏窗口快捷键.Enter += TextBox_Enter;
            textBox_召出隐藏窗口快捷键.Leave += TextBox_Leave;

            textBox_自动拿牌快捷键.KeyDown += textBox_自动拿牌快捷键_KeyDown;
            textBox_自动拿牌快捷键.Enter += TextBox_Enter;
            textBox_自动拿牌快捷键.Leave += TextBox_Leave;

            textBox_自动刷新商店快捷键.KeyDown += textBox_自动刷新商店快捷键_KeyDown;
            textBox_自动刷新商店快捷键.Enter += TextBox_Enter;
            textBox_自动刷新商店快捷键.Leave += TextBox_Leave;

            textBox_长按自动D牌快捷键.KeyDown += textBox_长按自动D牌快捷键_KeyDown;
            textBox_长按自动D牌快捷键.Enter += TextBox_Enter;
            textBox_长按自动D牌快捷键.Leave += TextBox_Leave;

            textBox_高亮提示.KeyDown += textBox_高亮提示_KeyDown;
            textBox_高亮提示.Enter += TextBox_Enter;
            textBox_高亮提示.Leave += TextBox_Leave;


            radioButton_手动设置坐标.CheckedChanged += radioButton_手动设置坐标_CheckedChanged;

            radioButton_自动设置坐标.CheckedChanged += radioButton_自动设置坐标_CheckedChanged;


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

            textBox_刷新商店按键.KeyDown += textBox_刷新商店按键_KeyDown;
            textBox_刷新商店按键.Enter += TextBox_Enter;
            textBox_刷新商店按键.Leave += TextBox_Leave;

            textBox_MaxTimesWithoutGetCard.KeyDown += TextBox_KeyDown;
            textBox_MaxTimesWithoutGetCard.Enter += TextBox_Enter;
            textBox_MaxTimesWithoutGetCard.Leave += textBox_MaxTimesWithoutGetCard_Leave;

            textBox_MaxTimesWithoutRefresh.KeyDown += TextBox_KeyDown;
            textBox_MaxTimesWithoutRefresh.Enter += TextBox_Enter;
            textBox_MaxTimesWithoutRefresh.Leave += textBox_MaxTimesWithoutRefresh_Leave;

            textBox_DelayAfterMouseOperation.KeyDown += TextBox_KeyDown;
            textBox_DelayAfterMouseOperation.Enter += TextBox_Enter;
            textBox_DelayAfterMouseOperation.Leave += textBox_DelayAfterMouseOperation_Leave;

            textBox_CPUDelayAfterRefreshStore.KeyDown += TextBox_KeyDown;
            textBox_CPUDelayAfterRefreshStore.Enter += TextBox_Enter;
            textBox_CPUDelayAfterRefreshStore.Leave += textBox_CPUDelayAfterRefreshStore_Leave;

            textBox_GPUDelayAfterRefreshStore.KeyDown += TextBox_KeyDown;
            textBox_GPUDelayAfterRefreshStore.Enter += TextBox_Enter;
            textBox_GPUDelayAfterRefreshStore.Leave += textBox_GPUDelayAfterRefreshStore_Leave;


            textBox_更新推荐装备间隔.KeyDown += TextBox_KeyDown;
            textBox_更新推荐装备间隔.Enter += TextBox_Enter;
            textBox_更新推荐装备间隔.Leave += TextBox_更新推荐装备间隔_Leave;


            textBox_英雄头像框边长.KeyDown += TextBox_KeyDown;
            textBox_英雄头像框边长.Enter += TextBox_Enter;
            textBox_英雄头像框边长.Leave += textBox_英雄头像框边长_Leave;

            textBox_英雄头像框水平间隔.KeyDown += TextBox_KeyDown;
            textBox_英雄头像框水平间隔.Enter += TextBox_Enter;
            textBox_英雄头像框水平间隔.Leave += textBox_英雄头像框水平间隔_Leave;

            textBox_英雄头像框垂直间隔.KeyDown += TextBox_KeyDown;
            textBox_英雄头像框垂直间隔.Enter += TextBox_Enter;
            textBox_英雄头像框垂直间隔.Leave += textBox_英雄头像框垂直间隔_Leave;


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

        #region 快捷键设置
        #region 修改-召出隐藏窗口快捷键-逻辑

        /// <summary>
        /// 当textBox_召出隐藏窗口快捷键为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_召出隐藏窗口快捷键_KeyDown(object sender, KeyEventArgs e)
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

                if (key.ToString() != _iappConfigService.CurrentConfig.HotKey1 && key.ToString() != _iappConfigService.CurrentConfig.HotKey2 && key.ToString() != _iappConfigService.CurrentConfig.HotKey4 && key.ToString() != _iappConfigService.CurrentConfig.HotKey5)
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
        /// 当textBox_自动拿牌快捷键为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_自动拿牌快捷键_KeyDown(object sender, KeyEventArgs e)
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
                if (key.ToString() != _iappConfigService.CurrentConfig.HotKey3 && key.ToString() != _iappConfigService.CurrentConfig.HotKey2 && key.ToString() != _iappConfigService.CurrentConfig.HotKey4 && key.ToString() != _iappConfigService.CurrentConfig.HotKey5)
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
        /// 当textBox_自动刷新商店快捷键为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_自动刷新商店快捷键_KeyDown(object sender, KeyEventArgs e)
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
                if (key.ToString() != _iappConfigService.CurrentConfig.HotKey1 && key.ToString() != _iappConfigService.CurrentConfig.HotKey3 && key.ToString() != _iappConfigService.CurrentConfig.HotKey4 && key.ToString() != _iappConfigService.CurrentConfig.HotKey5)
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
        /// 当textBox_长按自动D牌快捷键为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_长按自动D牌快捷键_KeyDown(object sender, KeyEventArgs e)
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
                if (key.ToString() != _iappConfigService.CurrentConfig.HotKey1 && key.ToString() != _iappConfigService.CurrentConfig.HotKey2 && key.ToString() != _iappConfigService.CurrentConfig.HotKey3 && key.ToString() != _iappConfigService.CurrentConfig.HotKey5)
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

        #region 修改-长按高亮提示快捷键-逻辑

        #endregion
        /// <summary>
        /// 当textBox_高亮提示为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；若用户输入合法键值，则判断是否和已有快捷键重复，若否则更新数据类相关数据并更新显示UI。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_高亮提示_KeyDown(object sender, KeyEventArgs e)
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
                if (key.ToString() != _iappConfigService.CurrentConfig.HotKey1 && key.ToString() != _iappConfigService.CurrentConfig.HotKey2 && key.ToString() != _iappConfigService.CurrentConfig.HotKey3 && key.ToString() != _iappConfigService.CurrentConfig.HotKey4)
                {
                    _iappConfigService.CurrentConfig.HotKey5 = key.ToString();
                    Update_AllComponents();
                }

                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }
        #endregion

        #region 截图设置                      
        #region 坐标设置方式单选框改变
        /// <summary>
        /// 手动设置坐标单选框状态改变事件 ——> 若被选中则启用手动设置坐标相关组件并禁用自动设置坐标相关组件，同时更新数据类相关数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_手动设置坐标_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_手动设置坐标.Checked)
            {
                roundedButton4.Enabled = false;
                comboBox_选择显示器.Enabled = true;
                roundedButton1.Enabled = true;
                roundedButton2.Enabled = true;
                roundedButton3.Enabled = true;
            }
            _iappConfigService.CurrentConfig.IsUseFixedCoordinates = radioButton_手动设置坐标.Checked;
        }

        /// <summary>
        /// 自动设置坐标单选框状态改变事件 ——> 若被选中则启用自动设置坐标相关组件并禁用手动设置坐标相关组件，同时更新数据类相关数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_自动设置坐标_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_自动设置坐标.Checked)
            {
                roundedButton4.Enabled = true;
                comboBox_选择显示器.Enabled = false;
                roundedButton1.Enabled = false;
                roundedButton2.Enabled = false;
                roundedButton3.Enabled = false;
            }
            _iappConfigService.CurrentConfig.IsUseDynamicCoordinates = radioButton_自动设置坐标.Checked;
        }

        #endregion

        #region 快速设置坐标                    
        /// <summary>
        /// 快速设置奕子截图坐标与大小按钮_被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void roundedButton1_Click(object sender, EventArgs e)
        {
            using (var setter = new FastSettingPositionService(targetScreen))
            {
                try
                {
                    // 第一张卡片
                    var rect1 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第1张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_1 = rect1;

                    // 第二张卡片
                    var rect2 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第2张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_2 = rect2;

                    // 第三张卡片
                    var rect3 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第3张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_3 = rect3;

                    // 第四张卡片
                    var rect4 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第4张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_4 = rect4;

                    // 第五张卡片
                    var rect5 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第5张奕子卡片的英雄名称部分（不包括金币图标）");
                    _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_5 = rect5;
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
        private async void roundedButton2_Click(object sender, EventArgs e)
        {
            using (var setter = new FastSettingPositionService(targetScreen))
            {
                try
                {
                    Rectangle rectangle = await setter.WaitForDrawAsync("请框选商店刷新按钮");
                    _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle = rectangle;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现错误: {ex.Message}");
                }
            }
            Update_AllComponents();
        }
        /// <summary>
        /// 设置高亮提示框坐标按钮_被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void roundedButton3_Click(object sender, EventArgs e)
        {
            using (var setter = new FastSettingPositionService(targetScreen))
            {
                try
                {
                    // 第一张卡片
                    var rect1 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第1张奕子卡片的所有部分部分（包括英雄图片、名称）");
                    _iappConfigService.CurrentConfig.HighLightRectangle_1 = rect1;

                    // 第二张卡片
                    var rect2 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第2张奕子卡片的所有部分部分（包括英雄图片、名称）");
                    _iappConfigService.CurrentConfig.HighLightRectangle_2 = rect2;

                    // 第三张卡片
                    var rect3 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第3张奕子卡片的所有部分部分（包括英雄图片、名称）");
                    _iappConfigService.CurrentConfig.HighLightRectangle_3 = rect3;

                    // 第四张卡片
                    var rect4 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第4张奕子卡片的所有部分部分（包括英雄图片、名称）");
                    _iappConfigService.CurrentConfig.HighLightRectangle_4 = rect4;

                    // 第五张卡片
                    var rect5 = await setter.WaitForDrawAsync(
                        "请框选商店从左到右数第5张奕子卡片的所有部分部分（包括英雄图片、名称）");
                    _iappConfigService.CurrentConfig.HighLightRectangle_5 = rect5;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"出现错误: {ex.Message}");
                }
            }
            Update_AllComponents();
        }
        #endregion

        #region 选择进程             
        /// <summary>
        /// 选择进程按钮被单击时触发的事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void roundedButton4_Click(object sender, EventArgs e)
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
                        // 给用户反馈
                        string displayName = $"{selectedProcess.ProcessName} (ID: {selectedProcess.Id})";
                        MessageBox.Show($"已选择进程：{displayName}\n请点击“保存设置”来保存此选择。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        #endregion
        #endregion



        #region 拿牌相关设置
        #region 避免程序与用户争夺光标控制权            


        /// <summary>
        /// 当“避免程序与用户争夺光标控制权”复选框状态改变时触发的事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch1_IsOnChanged(object sender, EventArgs e)
        {

            _iappConfigService.CurrentConfig.IsHighUserPriority = capsuleSwitch1.IsOn;
        }
        #endregion

        #region 自动停止拿牌
        private void capsuleSwitch4_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsAutomaticStopHeroPurchase = capsuleSwitch4.IsOn;
            if (_iappConfigService.CurrentConfig.IsAutomaticStopHeroPurchase)
            {
                textBox_MaxTimesWithoutGetCard.Enabled = true;
            }
            else
            {
                textBox_MaxTimesWithoutGetCard.Enabled = false;
            }
        }

        /// <summary>
        /// 离开textBox_MaxTimesWithoutGetCard时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_MaxTimesWithoutGetCard_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_MaxTimesWithoutGetCard.Text))
            {
                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.MaxTimesWithoutHeroPurchase = int.Parse(textBox_MaxTimesWithoutGetCard.Text);
                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 自动停止刷新商店      
        /// <summary>
        /// 当“自动停止刷新商店”复选框状态改变时触发的事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch7_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsAutomaticStopRefreshStore = capsuleSwitch7.IsOn;
            if (_iappConfigService.CurrentConfig.IsAutomaticStopRefreshStore)
            {
                textBox_MaxTimesWithoutRefresh.Enabled = true;
            }
            else
            {
                textBox_MaxTimesWithoutRefresh.Enabled = false;
            }
        }

        /// <summary>
        /// 离开textBox_MaxTimesWithoutRefresh时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_MaxTimesWithoutRefresh_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_MaxTimesWithoutRefresh.Text))
            {
                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.MaxTimesWithoutRefreshStore = int.Parse(textBox_MaxTimesWithoutRefresh.Text);
                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }
        #endregion

        #region 当识别到错误字符时停止刷新商店               
        /// <summary>
        /// 当“当识别到错误字符时停止刷新商店”复选框状态改变时触发的事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch8_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsStopRefreshStoreWhenErrorCharacters = capsuleSwitch8.IsOn;
        }
        #endregion

        #region 修改-键鼠操作间隔时间-逻辑
        /// <summary>
        /// 离开textBox_DelayAfterMouseOperation时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_DelayAfterMouseOperation_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_DelayAfterMouseOperation.Text))
            {
                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.DelayAfterOperation = int.Parse(textBox_DelayAfterMouseOperation.Text);
                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }

        #endregion

        #region 修改-CPU推理模式下刷新商店后等待时间-逻辑
        /// <summary>
        /// 离开textBox_CPUDelayAfterRefreshStore时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_CPUDelayAfterRefreshStore_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_CPUDelayAfterRefreshStore.Text))
            {
                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.DelayAfterRefreshStore_CPU = int.Parse(textBox_CPUDelayAfterRefreshStore.Text);
                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }


        #endregion

        #region 修改-GPU推理模式下刷新商店后等待时间-逻辑
        /// <summary>
        /// 离开textBox_GPUDelayAfterRefreshStore时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_GPUDelayAfterRefreshStore_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_GPUDelayAfterRefreshStore.Text))
            {
                Update_AllComponents();
            }
            else
            {
                try
                {
                    _iappConfigService.CurrentConfig.DelayAfterRefreshStore_GPU = int.Parse(textBox_GPUDelayAfterRefreshStore.Text);
                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }


        #endregion

        #region 拿牌方式       
        private bool isUpdatingSwitch_鼠标模拟拿牌 = false;
        private bool isUpdatingSwitch_按键模拟拿牌 = false;

        /// <summary>
        /// 当“鼠标模拟拿牌”单选框状态改变时触发的事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch2_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsMouseHeroPurchase = capsuleSwitch2.IsOn;
            if (isUpdatingSwitch_鼠标模拟拿牌) return;
            拿牌方式变更_鼠标拿牌();
        }

        /// <summary>
        /// 当“按键模拟拿牌”单选框状态改变时触发的事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch3_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsKeyboardHeroPurchase = capsuleSwitch3.IsOn;
            if (isUpdatingSwitch_按键模拟拿牌) return;
            拿牌方式变更_按键拿牌();
        }

        private void 拿牌方式变更_鼠标拿牌()
        {
            if (_iappConfigService.CurrentConfig.IsMouseHeroPurchase)
            {
                textBox_拿牌按键1.Enabled = false;
                textBox_拿牌按键2.Enabled = false;
                textBox_拿牌按键3.Enabled = false;
                textBox_拿牌按键4.Enabled = false;
                textBox_拿牌按键5.Enabled = false;
                isUpdatingSwitch_按键模拟拿牌 = true;
                capsuleSwitch3.IsOn = false;
                isUpdatingSwitch_按键模拟拿牌 = false;
            }
            else
            {
                textBox_拿牌按键1.Enabled = true;
                textBox_拿牌按键2.Enabled = true;
                textBox_拿牌按键3.Enabled = true;
                textBox_拿牌按键4.Enabled = true;
                textBox_拿牌按键5.Enabled = true;
                isUpdatingSwitch_按键模拟拿牌 = true;
                capsuleSwitch3.IsOn = true;
                isUpdatingSwitch_按键模拟拿牌 = false;
            }
        }

        private void 拿牌方式变更_按键拿牌()
        {
            if (_iappConfigService.CurrentConfig.IsKeyboardHeroPurchase)
            {
                textBox_拿牌按键1.Enabled = true;
                textBox_拿牌按键2.Enabled = true;
                textBox_拿牌按键3.Enabled = true;
                textBox_拿牌按键4.Enabled = true;
                textBox_拿牌按键5.Enabled = true;

                isUpdatingSwitch_鼠标模拟拿牌 = true;
                capsuleSwitch2.IsOn = false;
                isUpdatingSwitch_鼠标模拟拿牌 = false;
            }
            else
            {
                textBox_拿牌按键1.Enabled = false;
                textBox_拿牌按键2.Enabled = false;
                textBox_拿牌按键3.Enabled = false;
                textBox_拿牌按键4.Enabled = false;
                textBox_拿牌按键5.Enabled = false;
                isUpdatingSwitch_鼠标模拟拿牌 = true;
                capsuleSwitch2.IsOn = true;
                isUpdatingSwitch_鼠标模拟拿牌 = false;
            }
        }
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
                _iappConfigService.CurrentConfig.HeroPurchaseKey1 = key.ToString();
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
                _iappConfigService.CurrentConfig.HeroPurchaseKey2 = key.ToString();
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
                _iappConfigService.CurrentConfig.HeroPurchaseKey3 = key.ToString();
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
                _iappConfigService.CurrentConfig.HeroPurchaseKey4 = key.ToString();
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
                _iappConfigService.CurrentConfig.HeroPurchaseKey5 = key.ToString();
                Update_AllComponents();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion
        #endregion

        #region 刷新方式
        private bool isUpdatingSwitch_鼠标模拟刷新商店 = false;
        private bool isUpdatingSwitch_按键模拟刷新商店 = false;

        /// <summary>
        /// 选择鼠标模拟刷新商店时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch6_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsMouseRefreshStore = capsuleSwitch6.IsOn;
            if (isUpdatingSwitch_鼠标模拟刷新商店) return;
            刷新方式变更_鼠标刷新();
        }

        /// <summary>
        /// 选择按键模拟刷新商店时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch5_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsKeyboardRefreshStore = capsuleSwitch5.IsOn;
            if (isUpdatingSwitch_按键模拟刷新商店) return;
            刷新方式变更_按键刷新();
        }

        private void 刷新方式变更_鼠标刷新()
        {
            if (_iappConfigService.CurrentConfig.IsMouseRefreshStore)
            {
                textBox_刷新商店按键.Enabled = false;
                isUpdatingSwitch_按键模拟刷新商店 = true;
                capsuleSwitch5.IsOn = false;
                isUpdatingSwitch_按键模拟刷新商店 = false;
            }
            else
            {
                textBox_刷新商店按键.Enabled = true;
                isUpdatingSwitch_按键模拟刷新商店 = true;
                capsuleSwitch5.IsOn = true;
                isUpdatingSwitch_按键模拟刷新商店 = false;
            }
        }

        private void 刷新方式变更_按键刷新()
        {
            if (_iappConfigService.CurrentConfig.IsKeyboardRefreshStore)
            {
                textBox_刷新商店按键.Enabled = true;
                isUpdatingSwitch_鼠标模拟刷新商店 = true;
                capsuleSwitch6.IsOn = false;
                isUpdatingSwitch_鼠标模拟刷新商店 = false;
            }
            else
            {
                textBox_刷新商店按键.Enabled = false;
                isUpdatingSwitch_鼠标模拟刷新商店 = true;
                capsuleSwitch6.IsOn = true;
                isUpdatingSwitch_鼠标模拟刷新商店 = false;
            }
        }
        #region 修改-按键刷新商店按键-逻辑

        /// <summary>
        /// 当TextBox_刷新商店按键为焦点的情况下有按键按下 ——> 若用户键入回车，则使该组件失焦；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_刷新商店按键_KeyDown(object sender, KeyEventArgs e)
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
                _iappConfigService.CurrentConfig.RefreshStoreKey = key.ToString();
                Update_AllComponents();
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音 
            }
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
        }

        #endregion
        #endregion
        #endregion

        #region OCR相关设置
        #region 打开OCR纠正列表编辑器             
        /// <summary>
        /// OCR结果纠正列表编辑器按钮_被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void roundedButton5_Click(object sender, EventArgs e)
        {
            var form = new CorrectionEditorForm(_iappConfigService);
            form.Owner = this;// 设置父窗口，这样配置窗口会显示在主窗口上方但不会阻止主窗口                  
            form.TopMost = true;// 确保窗口在最前面
            form.Show();// 显示窗口
        }
        #endregion

        #region 推理单选框改变
        private bool isUpdatingSwitch_CPU推理 = false;
        private bool isUpdatingSwitch_GPU推理 = false;
        /// <summary>
        /// 选择CPU推理时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch10_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsUseCPUForInference = capsuleSwitch10.IsOn;
            if (isUpdatingSwitch_CPU推理) return;
            推理方式变更_CPU();
        }

        /// <summary>
        /// 选择GPU推理时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch9_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsUseGPUForInference = capsuleSwitch9.IsOn;
            if (isUpdatingSwitch_GPU推理) return;
            推理方式变更_GPU();
        }

        private void 推理方式变更_CPU()
        {
            if (_iappConfigService.CurrentConfig.IsUseCPUForInference)
            {
                isUpdatingSwitch_GPU推理 = true;
                capsuleSwitch9.IsOn = false;
                isUpdatingSwitch_GPU推理 = false;
            }
            else
            {
                isUpdatingSwitch_GPU推理 = true;
                capsuleSwitch9.IsOn = true;
                isUpdatingSwitch_GPU推理 = false;
            }
        }

        private void 推理方式变更_GPU()
        {
            if (_iappConfigService.CurrentConfig.IsUseGPUForInference)
            {
                isUpdatingSwitch_CPU推理 = true;
                capsuleSwitch10.IsOn = false;
                isUpdatingSwitch_CPU推理 = false;
            }
            else
            {
                isUpdatingSwitch_CPU推理 = true;
                capsuleSwitch10.IsOn = true;
                isUpdatingSwitch_CPU推理 = false;
            }
        }
        #endregion

        #region 过滤字符
        private void capsuleSwitch16_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsFilterLetters = capsuleSwitch16.IsOn;
        }

        private void capsuleSwitch17_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsFilterNumbers = capsuleSwitch17.IsOn;
        }
        #endregion
        #endregion

        #region 窗口设置
        /// <summary>
        /// 勾选或取消勾选“使用选择窗口位置”复选框时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        private void capsuleSwitch11_IsOnChanged(object sender, EventArgs e)
        {           
            _iappConfigService.CurrentConfig.IsUseSelectForm = capsuleSwitch11.IsOn;
        }
        /// <summary>
        /// 勾选或取消勾选“使用阵容窗口位置”复选框时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch12_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsUseLineUpForm = capsuleSwitch12.IsOn;
        }



        /// <summary>
        /// 勾选或取消勾选“使用状态覆盖窗口位置”复选框时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        private void capsuleSwitch13_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsUseStatusOverlayForm = capsuleSwitch13.IsOn;
        }
        /// <summary>
        /// 勾选或取消勾选“使用错误输出窗口位置”复选框时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        private void capsuleSwitch14_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsUseOutputForm = capsuleSwitch14.IsOn;
        }
        #region 英雄选择面板UI设置
        /// <summary>
        /// 离开textBox_英雄头像框边长时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_英雄头像框边长_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_英雄头像框边长.Text))
            {
                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox_英雄头像框边长.Text);
                    if (result > 0)
                    {
                        _iappConfigService.CurrentConfig.SelectFormHeroPictureBoxSize = result;
                    }
                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }

        /// <summary>
        /// 离开textBox_英雄头像框水平间隔时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_英雄头像框水平间隔_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_英雄头像框水平间隔.Text))
            {
                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox_英雄头像框水平间隔.Text);
                    if (result >= 0)
                    {
                        _iappConfigService.CurrentConfig.SelectFormHeroPictureBoxHorizontalSpacing = result;
                    }
                    Update_AllComponents();
                }
                catch
                {
                    Update_AllComponents();
                }
            }
        }

        /// <summary>
        /// 离开textBox_英雄头像框垂直间隔时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_英雄头像框垂直间隔_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_英雄头像框垂直间隔.Text))
            {
                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox_英雄头像框垂直间隔.Text);
                    if (result >= 0)
                    {
                        _iappConfigService.CurrentConfig.SelectFormHeroPanelsVerticalSpacing = result;
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
        #endregion

        #region 其他设置
        #region 自动更新推荐装备设置
        /// <summary>
        /// 勾选或取消勾选“定时更新推荐装备”复选框时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capsuleSwitch15_IsOnChanged(object sender, EventArgs e)
        {
            _iappConfigService.CurrentConfig.IsAutomaticUpdateEquipment = capsuleSwitch15.IsOn;
            if (_iappConfigService.CurrentConfig.IsAutomaticUpdateEquipment)
            {
                textBox_更新推荐装备间隔.Enabled = true;

            }
            else
            {
                textBox_更新推荐装备间隔.Enabled = false;
            }
        }

        /// <summary>
        /// 离开textBox_更新推荐装备间隔时触发，若用户输入为空，则显示文本从数据类读取；若用户输入合法，则更新数据类数据并更新显示文本。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_更新推荐装备间隔_Leave(object sender, EventArgs e)
        {
            //启用全局热键
            GlobalHotkeyTool.Enabled = true;
            if (string.IsNullOrWhiteSpace(textBox_更新推荐装备间隔.Text))
            {

                Update_AllComponents();
            }
            else
            {
                try
                {
                    int result = int.Parse(textBox_更新推荐装备间隔.Text);
                    if (result > 0)
                    {
                        _iappConfigService.CurrentConfig.UpdateEquipmentInterval = result;
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
        #endregion

        #region 设置存取相关

        /// <summary>
        /// /保存设置按钮_被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            _iappConfigService.Save(true);
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
            Update_AllComponents();
        }
        #endregion

        private void SettingForm_Load(object sender, EventArgs e)
        {

        }

        #region 更新阵容推荐
        private async void roundedButton6_Click(object sender, EventArgs e)
        {
            try
            {
                // 暂时禁用菜单项，防止用户重复点击
                roundedButton6.Enabled = false;

                // 调用异步更新逻辑
                await UpdateRecommendedLineUpsAsync();
            }
            finally
            {
                // 恢复菜单项可用状态
                roundedButton6.Enabled = true;
            }
        }

        /// <summary>
        /// 业务逻辑：从网络获取最新阵容数据并保存，完成后直接刷新不重启
        /// </summary>
        private async Task UpdateRecommendedLineUpsAsync()
        {
            DynamicGameDataService _iDynamicGameDataService = new DynamicGameDataService();
            LineupCrawlingService _iLineupCrawlingService = new LineupCrawlingService(_iDynamicGameDataService);
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
            catch (Exception ex)
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

        #region 标题栏按钮事件
        private void button_最小化_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_关闭_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion






        private void panel_拿牌相关_Paint(object sender, PaintEventArgs e)
        {

        }

       
    }
}
