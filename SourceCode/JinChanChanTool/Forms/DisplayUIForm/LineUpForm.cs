using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms.DisplayUIForm;
using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.DIYComponents;
using System.Diagnostics;
namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 阵容选择与展示窗体
    /// </summary>
    public partial class LineUpForm : Form
    {
        //单例模式
        private static LineUpForm _instance;
        public static LineUpForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new LineUpForm();
                }
                return _instance;
            }
        }

        // 拖动相关变量
        private Point _dragStartPoint;
        private bool _dragging;

        private ILineUpService _ilineUpService;//阵容数据服务对象
        public IAutomaticSettingsService _iAutoConfigService;//自动设置数据服务对象
        private IRecommendedLineUpService _iRecommendedLineUpService;//推荐阵容数据服务对象
        private IHeroDataService _heroDataService;//英雄数据服务对象
        private IEquipmentService _equipmentService;//装备数据服务对象

        private LineUpForm()
        {
            InitializeComponent();

        }

        private void LineUpForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化阵容窗体所需的服务
        /// </summary>
        /// <param name="ilineUpService">阵容数据服务对象</param>
        /// <param name="iAutoConfigService">程序自动设置数据服务对象</param>
        /// <param name="iRecommendedLineUpService">推荐阵容数据服务对象</param>
        /// <param name="heroDataService">英雄数据服务对象</param>
        /// <param name="equipmentService">装备数据服务对象</param>
        public void InitializeObject(ILineUpService ilineUpService, IAutomaticSettingsService iAutoConfigService, IRecommendedLineUpService iRecommendedLineUpService, IHeroDataService heroDataService, IEquipmentService equipmentService)
        {
            _iAutoConfigService = iAutoConfigService;
            _ilineUpService = ilineUpService;
            _iRecommendedLineUpService = iRecommendedLineUpService;
            _heroDataService = heroDataService;
            _equipmentService = equipmentService;
            ApplySavedLocation();
        }

        /// <summary>
        /// 从配置中读取并应用窗口位置
        /// </summary>
        private void ApplySavedLocation()
        {
            try
            {
                this.StartPosition = FormStartPosition.Manual;
                if (_iAutoConfigService.CurrentConfig.LineUpFormLocation.X == -1 && _iAutoConfigService.CurrentConfig.LineUpFormLocation.Y == -1)
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Right - this.Width,
                        screen.Bottom - this.Height
                    );
                    return;
                }
                // 确保坐标在屏幕范围内
                if (Screen.AllScreens.Any(s => s.Bounds.Contains(_iAutoConfigService.CurrentConfig.LineUpFormLocation)))
                {
                    this.Location = _iAutoConfigService.CurrentConfig.LineUpFormLocation;
                }
                else
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Right - this.Width,
                        screen.Bottom - this.Height
                    );
                }
            }
            catch
            {
                var screen = Screen.PrimaryScreen.Bounds;
                this.Location = new Point(
                    screen.Right - this.Width,
                    screen.Bottom - this.Height
                );
            }
        }

        #region 拖动窗体功能        
        /// <summary>
        /// 鼠标按下事件 - 开始拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                _dragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
                flowLayoutPanel1.BorderColor = Color.FromArgb(96, 223, 84);
                Cursor = Cursors.SizeAll;
            }
        }

        /// <summary>
        /// 鼠标移动事件 - 处理拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-_dragStartPoint.X, -_dragStartPoint.Y);
                this.Location = newLocation;
            }
        }

        /// <summary>
        /// 鼠标释放事件 - 结束拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            flowLayoutPanel1.BorderColor = Color.Gray;
            _dragging = false;
            Cursor = Cursors.Arrow;
            SaveFormLocation();
        }

        public void 绑定拖动(Control 要拖动的控件)
        {
            要拖动的控件.MouseDown -= panel_MouseDown;
            要拖动的控件.MouseMove -= panel_MouseMove;
            要拖动的控件.MouseUp -= panel_MouseUp;
            要拖动的控件.MouseDown += panel_MouseDown;
            要拖动的控件.MouseMove += panel_MouseMove;
            要拖动的控件.MouseUp += panel_MouseUp;
        }
        #endregion

        /// <summary>
        /// 保存当前阵容按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_保存_Click(object sender, EventArgs e)
        {
            if (_ilineUpService.Save())
            {
                MessageBox.Show("阵容已保存", "阵容已保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 清空当前阵容按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_清空_Click(object sender, EventArgs e)
        {
            _ilineUpService.ClearCurrentSubLineUp();
        }

        /// <summary>
        /// 拖动结束时保存窗口位置到配置服务
        /// </summary>
        private void SaveFormLocation()
        {
            try
            {
                if (_iAutoConfigService != null)
                {
                    _iAutoConfigService.CurrentConfig.LineUpFormLocation = this.Location;
                    _iAutoConfigService.Save();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public ComboBox GetLineUpSelectedComboBox()
        {
            return comboBox_LineUpSelected;
        }

        /// <summary>
        /// 更新变阵按钮的选中状态和名称
        /// </summary>
        public void UpdateSubLineUpButtons(int selectedIndex)
        {
            Color selectedColor = Color.FromArgb(130, 189, 39);
            Color normalColor = Color.FromArgb(45, 45, 48);

            button__变阵1.BackColor = selectedIndex == 0 ? selectedColor : normalColor;
            button__变阵2.BackColor = selectedIndex == 1 ? selectedColor : normalColor;
            button__变阵3.BackColor = selectedIndex == 2 ? selectedColor : normalColor;

            button__变阵1.Text = _ilineUpService.GetCurrentLineUp().SubLineUps[0].SubLineUpName;
            button__变阵2.Text = _ilineUpService.GetCurrentLineUp().SubLineUps[1].SubLineUpName;
            button__变阵3.Text = _ilineUpService.GetCurrentLineUp().SubLineUps[2].SubLineUpName;
        }


        private void button__变阵1_Click(object sender, EventArgs e)
        {
            _ilineUpService.SetSubLineUpIndex(0);

        }

        private void button__变阵1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RenameSubLineUp(0);
            }
        }


        private void button__变阵2_Click(object sender, EventArgs e)
        {
            _ilineUpService.SetSubLineUpIndex(1);
        }

        private void button__变阵2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RenameSubLineUp(1);
            }

        }


        private void button__变阵3_Click(object sender, EventArgs e)
        {
            _ilineUpService.SetSubLineUpIndex(2);
        }

        private void button__变阵3_MouseUp(object sender, MouseEventArgs e)
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
            LineUp currentLineUp = _ilineUpService.GetCurrentLineUp();
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
            _ilineUpService.SetSubLineUpName(index, newName.Trim());
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

        /// <summary>
        /// 阵容推荐按钮点击事件 - 打开推荐阵容选择窗口
        /// </summary>
        private void button_阵容推荐_Click(object sender, EventArgs e)
        {
            if (_iRecommendedLineUpService == null || _heroDataService == null || _equipmentService == null)
            {
                MessageBox.Show("相关服务未初始化！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Random random = new Random();
            //float min = 0.0f;
            //float max = 100.0f;
            //for (int i = 0;i<10;i++)
            //{
            //    int r = random.Next(0, 4);
            //    LineUpTier l;
            //    switch(r)
            //    {
            //        case 0:
            //            l = LineUpTier.S;
            //            break;
            //        case 1:
            //            l = LineUpTier.A;
            //            break;
            //        case 2:
            //            l = LineUpTier.B;
            //            break;
            //        case 3:
            //            l = LineUpTier.C;
            //            break;
            //        case 4:
            //            l = LineUpTier.D;
            //            break;
            //        default:
            //            l = LineUpTier.D;
            //            break;
            //    }               
            //    SubLineUp sb = new SubLineUp("前期");
            //    for(int j =0;j<10;j++)
            //    {
            //        int index = random.Next(0, _heroDataService.GetHeroCount() - 1);
            //        int index2 = random.Next(0, _equipmentService.GetEquipmentDatas().Count - 1);
            //        int index3 = random.Next(0, _equipmentService.GetEquipmentDatas().Count - 1);
            //        int index4 = random.Next(0, _equipmentService.GetEquipmentDatas().Count - 1);
            //        sb.Add(_heroDataService.GetHeroDatas()[index].HeroName, [$"{_equipmentService.GetEquipmentDatas()[index2].Name}", $"{_equipmentService.GetEquipmentDatas()[index3].Name}", $"{_equipmentService.GetEquipmentDatas()[index4].Name}"]);
            //    }
            //    RecommendedLineUp re = new RecommendedLineUp
            //    {
            //        LineUpName = $"阵容{i}",
            //        Tier = l,
            //        WinRate = (float)(random.NextDouble() * (max - min) + min),
            //        AverageRank = (float)(random.NextDouble() * (0 - 8) + 0),
            //        PickRate = (float)(random.NextDouble() * (max - min) + min),
            //        TopFourRate = (float)(random.NextDouble() * (max - min) + min),
            //        Tags = ["简单", "速9"],
            //        LineUpUnits = sb.LineUpUnits 
            //    };
            //    _iRecommendedLineUpService.AddRecommendedLineUp(re);
            //}          
            //_iRecommendedLineUpService.Save();

            // 检查是否有推荐阵容数据
            if (_iRecommendedLineUpService.GetCount() == 0)
            {
                MessageBox.Show("暂无推荐阵容数据。\n\n请先通过数据爬取工具获取推荐阵容数据后再试。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 打开推荐阵容选择窗口
            using (var selectForm = new LineUpSelectForm(_iRecommendedLineUpService, _heroDataService, _equipmentService))
            {
                selectForm.TopMost = true;
                if (selectForm.ShowDialog(this) == DialogResult.OK && selectForm.SelectedLineUp != null)
                {
                    // 用户选择了阵容，替换当前子阵容
                    var selectedLineUp = selectForm.SelectedLineUp;

                    // 将推荐阵容的英雄列表导入到当前子阵容
                    if (!_ilineUpService.ReplaceCurrentSubLineUp(selectedLineUp.LineUpUnits))
                    {
                        MessageBox.Show("应用阵容失败，请稍后重试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }                 
                }
            }
        }
    }
}
