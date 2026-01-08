using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms.DisplayUIForm;
using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Tools;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 阵容选择与展示窗体
    /// </summary>
    public partial class LineUpForm : Form
    {
        // 单例模式
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
        private bool _isDragged; // 标志位：是否发生了真正的拖动
        private const int DRAG_THRESHOLD = 2; // 拖动阈值（像素）

        /// <summary>
        /// 获取当前是否发生了拖动（用于区分拖动和点击）
        /// </summary>
        public bool IsDragged => _isDragged;

        // 棋盘展开/收起状态
        private bool _isBoardExpanded;

        // 窗体高度常量（逻辑像素）
        private const int COLLAPSED_HEIGHT = 95;
        private const int BOARD_HEIGHT = 200;
        private const int BENCH_HEIGHT = 50;

        private ILineUpService _ilineUpService; // 阵容数据服务对象
        public IAutomaticSettingsService _iAutoConfigService; // 自动设置数据服务对象
        private IRecommendedLineUpService _iRecommendedLineUpService; // 推荐阵容数据服务对象
        private IHeroDataService _heroDataService; // 英雄数据服务对象
        private IEquipmentService _equipmentService; // 装备数据服务对象

        private LineUpForm()
        {
            InitializeComponent();
            _isBoardExpanded = false;
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

            // 初始化棋盘服务
            hexagonBoard.InitializeServices(_heroDataService);

            // 绑定棋盘事件
            hexagonBoard.HeroPositionChanged += HexagonBoard_HeroPositionChanged;
            hexagonBoard.HeroCleared += HexagonBoard_HeroCleared;

            // 初始化备战席服务
            benchPanel.InitializeServices(_heroDataService);

            // 绑定备战席事件
            benchPanel.HeroDroppedIn += BenchPanel_HeroPositionChanged;

            ApplySavedLocation();
        }

        /// <summary>
        /// 棋盘英雄位置变更事件处理
        /// </summary>
        private void HexagonBoard_HeroPositionChanged(object sender, BoardHeroPositionChangedEventArgs e)
        {
            // 刷新备战席显示（因为可能有英雄从备战席拖到棋盘，或从棋盘交换到备战席）
            benchPanel.RefreshBench();
        }

        /// <summary>
        /// 棋盘英雄清除事件处理
        /// </summary>
        private void HexagonBoard_HeroCleared(object sender, BoardHeroClearedEventArgs e)
        {
            // 英雄被清除到备战席，刷新备战席显示
            benchPanel.RefreshBench();
        }

        /// <summary>
        /// 备战席英雄位置变更事件处理（从棋盘拖到备战席）
        /// </summary>
        private void BenchPanel_HeroPositionChanged(object sender, BenchHeroDroppedInEventArgs e)
        {
            // 将从棋盘拖来的英雄移到备战席（位置设为0,0）
            if (e.MovedUnit != null)
            {
                e.MovedUnit.Position = (0, 0);
            }

            // 刷新棋盘和备战席显示
            RefreshHexagonBoard();
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
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _isDragged = false; // 重置拖动标志位
                _dragStartPoint = new Point(e.X, e.Y);
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
                // 计算鼠标移动距离
                int deltaX = e.X - _dragStartPoint.X;
                int deltaY = e.Y - _dragStartPoint.Y;
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                // 只有移动距离超过阈值才认为是真正的拖动
                if (distance > DRAG_THRESHOLD)
                {
                    _isDragged = true;
                    flowLayoutPanel1.BorderColor = Color.FromArgb(96, 223, 84);
                    Cursor = Cursors.SizeAll;

                    Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                    newLocation.Offset(-_dragStartPoint.X, -_dragStartPoint.Y);
                    this.Location = newLocation;
                }
            }
        }

        /// <summary>
        /// 鼠标释放事件 - 结束拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                flowLayoutPanel1.BorderColor = Color.Gray;
                _dragging = false;
                Cursor = Cursors.Arrow;

                if (_isDragged)
                {
                    SaveFormLocation();
                }

                // 延迟重置拖动标志位，让其他 MouseUp 事件处理器能够检查到
                BeginInvoke(new Action(() => _isDragged = false));
            }
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
            if (selectedIndex == 0)
            {
                button__变阵1.Focus();
            }
            else if (selectedIndex == 1)
            {
                button__变阵2.Focus();
            }
            else if (selectedIndex == 2)
            {
                button__变阵3.Focus();
            }
            button__变阵1.Text = _ilineUpService.GetCurrentLineUp().SubLineUps[0].SubLineUpName;
            button__变阵2.Text = _ilineUpService.GetCurrentLineUp().SubLineUps[1].SubLineUpName;
            button__变阵3.Text = _ilineUpService.GetCurrentLineUp().SubLineUps[2].SubLineUpName;

            // 刷新棋盘显示
            RefreshHexagonBoard();
        }

        /// <summary>
        /// 刷新蜂巢棋盘和备战席显示
        /// </summary>
        public void RefreshHexagonBoard()
        {
            if (_ilineUpService == null) return;

            SubLineUp currentSubLineUp = _ilineUpService.GetCurrentSubLineUp();
            hexagonBoard.BindSubLineUp(currentSubLineUp);
            benchPanel.BindSubLineUp(currentSubLineUp);
        }

        /// <summary>
        /// 展开/收起按钮点击事件 - 切换棋盘显示状态
        /// </summary>
        private void button_展开收起_Click(object sender, EventArgs e)
        {
            ToggleBoardExpanded();
        }

        /// <summary>
        /// 切换棋盘展开/收起状态
        /// </summary>
        private void ToggleBoardExpanded()
        {
            _isBoardExpanded = !_isBoardExpanded;

            if (_isBoardExpanded)
            {
                // 展开棋盘和备战席
                int expandedHeight = LogicalToDeviceUnits(COLLAPSED_HEIGHT + BOARD_HEIGHT + BENCH_HEIGHT);
                int boardY = LogicalToDeviceUnits(COLLAPSED_HEIGHT - 2);
                int boardHeight = LogicalToDeviceUnits(BOARD_HEIGHT);
                int benchY = LogicalToDeviceUnits(COLLAPSED_HEIGHT - 2 + BOARD_HEIGHT);
                int benchHeight = LogicalToDeviceUnits(BENCH_HEIGHT);

                // 设置棋盘位置和大小
                hexagonBoard.Location = new Point(LogicalToDeviceUnits(2), boardY);
                hexagonBoard.Size = new Size(LogicalToDeviceUnits(628), boardHeight);
                hexagonBoard.Visible = true;

                // 设置备战席位置和大小
                benchPanel.Location = new Point(LogicalToDeviceUnits(2), benchY);
                benchPanel.Size = new Size(LogicalToDeviceUnits(628), benchHeight);
                benchPanel.Visible = true;

                this.ClientSize = new Size(LogicalToDeviceUnits(632), expandedHeight);

                button_展开收起.BackColor = Color.FromArgb(130, 189, 39);

                // 刷新棋盘和备战席数据
                RefreshHexagonBoard();
            }
            else
            {
                // 收起棋盘和备战席
                hexagonBoard.Visible = false;
                benchPanel.Visible = false;
                this.ClientSize = new Size(LogicalToDeviceUnits(632), LogicalToDeviceUnits(COLLAPSED_HEIGHT));

                button_展开收起.BackColor = Color.FromArgb(45, 45, 48);
            }
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
        /// 使用圆角效果和自定义标题栏。
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="defaultValue">输入框默认值</param>
        /// <returns>用户输入的文本，如果取消则返回null</returns>
        private string? PromptForSubLineUpName(string title, string defaultValue)
        {
            using RenameDialogForm prompt = new RenameDialogForm();
            prompt.StartPosition = FormStartPosition.CenterParent;
            prompt.InputText = defaultValue;

            return prompt.ShowDialog(this) == DialogResult.OK ? prompt.InputText : null;
        }

        /// <summary>
        /// 带圆角效果和自定义标题栏的重命名对话框
        /// </summary>
        private class RenameDialogForm : Form
        {
            // GDI32 API - 用于创建圆角效果
            [DllImport("gdi32.dll")]
            private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

            [DllImport("user32.dll")]
            private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

            // 圆角半径
            private const int CORNER_RADIUS = 16;

            // 边框颜色
            private static readonly Color BORDER_COLOR = Color.FromArgb(250, 250, 250);

            // 输入框控件引用
            private TextBox inputBox;

            /// <summary>
            /// 获取或设置输入框的文本
            /// </summary>
            public string InputText
            {
                get => inputBox.Text;
                set => inputBox.Text = value;
            }

            public RenameDialogForm()
            {
                InitializeComponents();
            }

            /// <summary>
            /// 初始化对话框组件
            /// </summary>
            private void InitializeComponents()
            {
                // 窗体基本设置
                TopMost = true;
                FormBorderStyle = FormBorderStyle.None;
                MinimizeBox = false;
                MaximizeBox = false;
                AutoScaleMode = AutoScaleMode.Dpi;
                BackColor = BORDER_COLOR;
                ClientSize = new Size(LogicalToDeviceUnits(280), LogicalToDeviceUnits(140));

                // 边框面板（最外层，通过Padding模拟边框）
                Panel borderPanel = new Panel
                {
                    BackColor = BORDER_COLOR,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(
                        LogicalToDeviceUnits(3),
                        LogicalToDeviceUnits(3),
                        LogicalToDeviceUnits(4),
                        LogicalToDeviceUnits(4)
                    )
                };

                // 客户区面板（白色背景）
                Panel clientPanel = new Panel
                {
                    BackColor = Color.White,
                    Dock = DockStyle.Fill
                };

                // 标题栏面板
                int titleBarHeight = LogicalToDeviceUnits(28);
                Panel titleBarPanel = new Panel
                {
                    BackColor = Color.White,
                    Dock = DockStyle.Top,
                    Height = titleBarHeight
                };

              

                // 标题标签
                Label titleLabel = new Label
                {
                    Text = "重命名",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(LogicalToDeviceUnits(8), 0),
                    Size = new Size(LogicalToDeviceUnits(100), titleBarHeight),
                    Font = new Font(Font.FontFamily, 9F, FontStyle.Regular, GraphicsUnit.Point)
                };

                // 关闭按钮
                Button closeButton = new Button
                {
                    Text = "X",
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(LogicalToDeviceUnits(25), LogicalToDeviceUnits(25)),
                    Location = new Point(LogicalToDeviceUnits(245), LogicalToDeviceUnits(1)),
                    TabStop = false
                };
                closeButton.FlatAppearance.BorderSize = 0;
                closeButton.Click += (s, e) =>
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                };

                titleBarPanel.Controls.Add(titleLabel);
                titleBarPanel.Controls.Add(closeButton);

                // 内容面板
                Panel contentPanel = new Panel
                {
                    BackColor = Color.White,
                    Dock = DockStyle.Fill
                };

                // 变阵名称标签
                Label textLabel = new Label
                {
                    Text = "变阵名称：",
                    AutoSize = true,
                    Location = new Point(LogicalToDeviceUnits(10), LogicalToDeviceUnits(10))
                };

                // 输入框
                inputBox = new TextBox
                {
                    Location = new Point(LogicalToDeviceUnits(10), LogicalToDeviceUnits(35)),
                    Width = LogicalToDeviceUnits(253)
                };

                // 确定按钮
                Button confirmButton = new Button
                {
                    Text = "确定",
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(LogicalToDeviceUnits(75), LogicalToDeviceUnits(28)),
                    Location = new Point(LogicalToDeviceUnits(100), LogicalToDeviceUnits(70)),
                    DialogResult = DialogResult.OK
                };
                confirmButton.FlatAppearance.BorderColor = Color.Gray;

                // 取消按钮
                Button cancelButton = new Button
                {
                    Text = "取消",
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(LogicalToDeviceUnits(75), LogicalToDeviceUnits(28)),
                    Location = new Point(LogicalToDeviceUnits(188), LogicalToDeviceUnits(70)),
                    DialogResult = DialogResult.Cancel
                };
                cancelButton.FlatAppearance.BorderColor = Color.Gray;

                contentPanel.Controls.Add(textLabel);
                contentPanel.Controls.Add(inputBox);
                contentPanel.Controls.Add(confirmButton);
                contentPanel.Controls.Add(cancelButton);

                // 组装控件层次结构
                clientPanel.Controls.Add(contentPanel);
                clientPanel.Controls.Add(titleBarPanel);
                borderPanel.Controls.Add(clientPanel);
                Controls.Add(borderPanel);

                // 设置默认按钮
                AcceptButton = confirmButton;
                CancelButton = cancelButton;
                DragHelper.EnableDragForChildren(titleBarPanel);
            }           

            /// <summary>
            /// 在窗口句柄创建后应用圆角效果
            /// </summary>
            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                ApplyRoundedCorners();
            }

            /// <summary>
            /// 窗口大小改变时重新应用圆角
            /// </summary>
            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                if (Handle != IntPtr.Zero)
                {
                    ApplyRoundedCorners();
                }
            }

            /// <summary>
            /// 应用GDI Region圆角效果
            /// </summary>
            private void ApplyRoundedCorners()
            {
                try
                {
                    IntPtr region = CreateRoundRectRgn(0, 0, Width, Height, CORNER_RADIUS, CORNER_RADIUS);
                    if (region != IntPtr.Zero)
                    {
                        SetWindowRgn(Handle, region, true);
                    }
                }
                catch
                {
                    // 圆角应用失败时静默处理，不影响功能使用
                }
            }
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
            //for (int i = 0; i < 10; i++)
            //{
            //    int r = random.Next(0, 4);
            //    LineUpTier l;
            //    switch (r)
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
            //    for (int j = 0; j < 10; j++)
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
            //        LineUpUnits = sb.LineUpUnits,
            //        Description = "描述"
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
