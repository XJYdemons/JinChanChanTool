using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms.DisplayUIForm;
using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices;
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
        private const int BOARD_HEIGHT = 150;
        private const int BENCH_HEIGHT = 35;

        private ILineUpService _ilineUpService; // 阵容数据服务对象
        public IAutomaticSettingsService _iAutoConfigService; // 自动设置数据服务对象
        private IRecommendedLineUpService _iRecommendedLineUpService; // 推荐阵容数据服务对象
        private IHeroDataService _heroDataService; // 英雄数据服务对象
        private IEquipmentService _equipmentService; // 装备数据服务对象

        private LineUpForm()
        {
            InitializeComponent();
            _isBoardExpanded = false;            
            DragHelper.EnableDrag(button_保存);
            DragHelper.EnableDrag(button_清空);
            DragHelper.EnableDrag(button_阵容推荐);
            DragHelper.EnableDrag(button_展开收起);
            
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

        #region 棋盘UI相关
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
        /// 刷新蜂巢棋盘和备战席显示
        /// </summary>
        public void RefreshHexagonBoard()
        {
            if (_ilineUpService == null) return;

            SubLineUp currentSubLineUp = _ilineUpService.GetCurrentSubLineUp();
            hexagonBoard.BindSubLineUp(currentSubLineUp);
            benchPanel.BindSubLineUp(currentSubLineUp);

            // 同步刷新散件面板
            if (_isBoardExpanded && componentPanel != null && componentPanel.Visible)
            {
                RefreshComponentPanel();
            }
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
                hexagonBoard.BackColor = Color.FromArgb(1, 1, 1);
                benchPanel.BackColor = Color.FromArgb(1, 1, 1);
                componentPanel.BackColor = Color.FromArgb(1, 1, 1);
                //hexagonBoard.BackColor = Color.FromArgb(30,35, 45);
                //benchPanel.BackColor = Color.FromArgb(30, 35, 45);
                //componentPanel.BackColor = Color.FromArgb(30, 35, 45);
                // 展开棋盘和备战席
                int boardHeight = LogicalToDeviceUnits(BOARD_HEIGHT);
                int benchHeight = LogicalToDeviceUnits(BENCH_HEIGHT);

                // 棋盘紧贴 flowLayoutPanel1 底部
                int boardY = flowLayoutPanel1.Location.Y + flowLayoutPanel1.Height;
                // 备战席紧贴棋盘底部
                int benchY = boardY + boardHeight;
                // 窗体总高度
                int expandedHeight = benchY + benchHeight;

                // 计算棋盘和备战席的宽度（面板宽度的3/4）
                int boardWidth = flowLayoutPanel1.Width * 3 / 4;

                // 设置棋盘位置和大小
                hexagonBoard.Location = new Point(flowLayoutPanel1.Location.X, boardY);
                hexagonBoard.Size = new Size(boardWidth, boardHeight);
                hexagonBoard.Visible = true;

                // 设置备战席位置和大小
                benchPanel.Location = new Point(flowLayoutPanel1.Location.X, benchY);
                benchPanel.Size = new Size(boardWidth, benchHeight);
                benchPanel.Visible = true;

                // 设置散件面板位置和大小（右侧1/4宽度区域）
                int componentPanelX = flowLayoutPanel1.Location.X + boardWidth;
                int componentPanelWidth = flowLayoutPanel1.Width - boardWidth;
                int componentPanelHeight = boardHeight + benchHeight;
                componentPanel.Location = new Point(componentPanelX, boardY);
                componentPanel.Size = new Size(componentPanelWidth, componentPanelHeight);
                componentPanel.Visible = true;

                this.ClientSize = new Size(LogicalToDeviceUnits(632), expandedHeight);

                button_展开收起.BackColor = Color.FromArgb(130, 189, 39);

                // 刷新棋盘和备战席数据
                RefreshHexagonBoard();

                // 刷新散件面板数据
                RefreshComponentPanel();
            }
            else
            {
                // 收起棋盘和备战席
                hexagonBoard.Visible = false;
                benchPanel.Visible = false;
                componentPanel.Visible = false;
                this.ClientSize = new Size(LogicalToDeviceUnits(632), LogicalToDeviceUnits(COLLAPSED_HEIGHT));

                button_展开收起.BackColor = Color.FromArgb(45, 45, 48);
            }
        }

        /// <summary>
        /// 更新变阵按钮的选中状态和名称
        /// </summary>
        public void 更新棋盘显示(int selectedIndex)
        {
            // 刷新棋盘显示
            RefreshHexagonBoard();
        }
        #endregion

        #region 散件需求面板相关
        /// <summary>
        /// 计算当前阵容所需的散件及数量
        /// </summary>
        /// <returns>散件名称与数量的字典，按数量降序排序</returns>
        private Dictionary<string, int> CalculateRequiredComponents()
        {
            Dictionary<string, int> componentCounts = new Dictionary<string, int>();

            if (_ilineUpService == null || _equipmentService == null)
            {
                return componentCounts;
            }

            try
            {
                SubLineUp currentSubLineUp = _ilineUpService.GetCurrentSubLineUp();
                if (currentSubLineUp == null || currentSubLineUp.LineUpUnits == null)
                {
                    return componentCounts;
                }

                // 遍历当前阵容中的每个英雄单位
                foreach (LineUpUnit unit in currentSubLineUp.LineUpUnits)
                {
                    if (unit.EquipmentNames == null)
                    {
                        continue;
                    }

                    // 遍历英雄的3个装备槽位
                    foreach (string equipmentName in unit.EquipmentNames)
                    {
                        // 跳过空装备
                        if (string.IsNullOrEmpty(equipmentName))
                        {
                            continue;
                        }

                        // 获取装备对象
                        Equipment equipment = _equipmentService.GetEquipmentFromName(equipmentName);
                        if (equipment == null)
                        {
                            continue;
                        }

                        // 判断是否有合成路径
                        if (equipment.SyntheticPathway != null && equipment.SyntheticPathway.Length >= 2)
                        {
                            // 有合成路径，拆分为两个散件
                            foreach (string componentName in equipment.SyntheticPathway)
                            {
                                if (!string.IsNullOrEmpty(componentName))
                                {
                                    if (componentCounts.ContainsKey(componentName))
                                    {
                                        componentCounts[componentName]++;
                                    }
                                    else
                                    {
                                        componentCounts[componentName] = 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                            if (equipment.EquipmentType == "散件")
                            {
                                if (componentCounts.ContainsKey(equipmentName))
                                {
                                    componentCounts[equipmentName]++;
                                }
                                else
                                {
                                    componentCounts[equipmentName] = 1;
                                }
                            }                                                        
                        }
                    }
                }

                // 按数量降序排序并返回
                return componentCounts.OrderByDescending(kvp => kvp.Value)
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch (Exception)
            {
                // 异常处理：记录日志或静默失败
                return componentCounts;
            }
        }

        /// <summary>
        /// 刷新散件需求面板的显示
        /// </summary>
        private void RefreshComponentPanel()
        {
            if (componentPanel == null)
            {
                return;
            }

            try
            {
                // 暂停布局更新
                componentPanel.SuspendLayout();

                // 清空现有控件
                componentPanel.Controls.Clear();

                // 如果面板不可见，直接返回
                if (!componentPanel.Visible)
                {
                    componentPanel.ResumeLayout(false);
                    return;
                }

                // 获取面板实际客户区宽度（ClientSize会自动考虑滚动条是否存在）
                int availableWidth = componentPanel.ClientSize.Width;
                if (availableWidth <= 0)
                {
                    componentPanel.ResumeLayout(false);
                    return;
                }

                // 计算布局参数（优化为紧凑布局，确保一行能放两个）
                int itemGap = LogicalToDeviceUnits(3); // 两个散件项之间的间距
                int sidePadding = LogicalToDeviceUnits(3); // 左右边距

                // 强制一行显示两个散件项，紧凑布局
                int itemWidth = (availableWidth - sidePadding * 2 - itemGap) / 2;

                // 根据实际itemWidth自适应调整图片大小
                int componentImageSize = LogicalToDeviceUnits(18); // 散件图片大小（紧凑）
                int innerPadding = LogicalToDeviceUnits(2); // 散件项内部边距

                // 设置Padding
                componentPanel.Padding = new Padding(sidePadding, 0, 0, 0);

                // 创建标题标签
                Label titleLabel = new Label
                {
                    Text = "散件数量",
                    ForeColor = Color.White,
                    Font = new Font("微软雅黑", 10, FontStyle.Bold),
                    Width = availableWidth,
                    Height = LogicalToDeviceUnits(24),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, LogicalToDeviceUnits(5), 0, LogicalToDeviceUnits(5))
                };
                componentPanel.Controls.Add(titleLabel);

                // 计算所需散件
                Dictionary<string, int> requiredComponents = CalculateRequiredComponents();

                // 如果没有散件需求，只显示标题
                if (requiredComponents.Count == 0)
                {
                    componentPanel.ResumeLayout(false);
                    return;
                }

                // 计算散件项高度（componentImageSize和innerPadding已在前面定义）
                int itemHeight = componentImageSize + LogicalToDeviceUnits(6); // 散件项高度（紧凑）

                // 为每个散件创建显示项
                int itemIndex = 0;
                foreach (KeyValuePair<string, int> kvp in requiredComponents)
                {
                    string componentName = kvp.Key;
                    int count = kvp.Value;

                    // 获取散件装备对象
                    Equipment component = _equipmentService.GetEquipmentFromName(componentName);
                    if (component == null)
                    {
                        continue;
                    }

                    // 计算边距（第一列右边有gap，第二列右边无gap）
                    bool isFirstColumn = (itemIndex % 2 == 0);
                    int rightMargin = isFirstColumn ? itemGap : 0;

                    // 创建散件项容器Panel
                    Panel itemPanel = new Panel
                    {
                        Width = itemWidth,
                        Height = itemHeight,
                        Margin = new Padding(0, LogicalToDeviceUnits(2), rightMargin, 0),
                        BackColor = Color.Transparent
                    };

                    // 创建散件图片框
                    PictureBox componentPictureBox = new PictureBox
                    {
                        Width = componentImageSize,
                        Height = componentImageSize,
                        Left = innerPadding,
                        Top = (itemHeight - componentImageSize) / 2,
                        Image = component.Image,
                        Tag = component,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackColor = Color.Transparent
                    };

                    // 创建数量标签（使用AutoSize确保文字完整显示，缩小字号适应紧凑布局）
                    Label countLabel = new Label
                    {
                        Text = $"x{count}",
                        ForeColor = Color.White,
                        Font = new Font("微软雅黑", 9, FontStyle.Bold),
                        AutoSize = true,
                        BackColor = Color.Transparent
                    };

                    // 先添加标签以计算实际大小
                    itemPanel.Controls.Add(countLabel);

                    // 设置标签位置（在图片右侧，垂直居中）
                    countLabel.Left = innerPadding + componentImageSize + innerPadding;
                    countLabel.Top = (itemHeight - countLabel.Height) / 2;

                   

                    // 添加图片框
                    itemPanel.Controls.Add(componentPictureBox);

                    // 将散件项添加到主面板
                    componentPanel.Controls.Add(itemPanel);

                    itemIndex++;
                }

                // 恢复布局更新
                componentPanel.ResumeLayout(true);
            }
            catch (Exception)
            {
                // 异常处理：确保恢复布局
                componentPanel.ResumeLayout(false);
            }
        }
        #endregion

        #region 按钮交互
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

        public ComboBox GetLineUpSelectedComboBox()
        {
            return comboBox_LineUpSelected;
        }
        #endregion

        #region 位置保存与读取

        /// <summary>
        /// Windows消息常量 - 窗口移动或大小调整结束
        /// </summary>
        private const int WM_EXITSIZEMOVE = 0x0232;

        /// <summary>
        /// 重写窗口过程以监听拖动结束消息
        /// </summary>
        /// <param name="m">Windows消息</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 监听窗口移动结束消息
            if (m.Msg == WM_EXITSIZEMOVE)
            {
                // 拖动结束,保存位置
                SaveFormLocation();
            }
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
      
        /// <summary>
        /// 显示窗体时应用保存的位置
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                this.StartPosition = FormStartPosition.Manual;
                if (_iAutoConfigService.CurrentConfig.LineUpFormLocation.X == -1 && _iAutoConfigService.CurrentConfig.LineUpFormLocation.Y == -1)
                {
                    // 计算完全展开状态的窗体高度（包括棋盘和备战席）
                    int boardHeight = LogicalToDeviceUnits(BOARD_HEIGHT);
                    int benchHeight = LogicalToDeviceUnits(BENCH_HEIGHT);
                    int boardY = flowLayoutPanel1.Location.Y + flowLayoutPanel1.Height;
                    int benchY = boardY + boardHeight;
                    int expandedHeight = benchY + benchHeight;

                    // 使用完全展开的高度来定位窗体
                    Rectangle screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Right - this.Width,
                        screen.Bottom - expandedHeight
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
                    // 计算完全展开状态的窗体高度（包括棋盘和备战席）
                    int boardHeight = LogicalToDeviceUnits(BOARD_HEIGHT);
                    int benchHeight = LogicalToDeviceUnits(BENCH_HEIGHT);
                    int boardY = flowLayoutPanel1.Location.Y + flowLayoutPanel1.Height;
                    int benchY = boardY + boardHeight;
                    int expandedHeight = benchY + benchHeight;

                    // 使用完全展开的高度来定位窗体
                    Rectangle screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Right - this.Width,
                        screen.Bottom - expandedHeight
                    );
                }
            }
            catch
            {
                // 计算完全展开状态的窗体高度（包括棋盘和备战席）
                int boardHeight = LogicalToDeviceUnits(BOARD_HEIGHT);
                int benchHeight = LogicalToDeviceUnits(BENCH_HEIGHT);
                int boardY = flowLayoutPanel1.Location.Y + flowLayoutPanel1.Height;
                int benchY = boardY + boardHeight;
                int expandedHeight = benchY + benchHeight;

                // 使用完全展开的高度来定位窗体
                Rectangle screen = Screen.PrimaryScreen.Bounds;
                this.Location = new Point(
                    screen.Right - this.Width,
                    screen.Bottom - expandedHeight
                );
            }
        }
        #endregion
    }
}
