using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices.Interface;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 蜂巢棋盘控件，用于展示4行7列的尖顶六边形格子
    /// 支持英雄位置拖拽、右键清除
    /// </summary>
    public class HexagonBoard : Panel
    {
        // 棋盘常量
        private const int ROWS = 4;
        private const int COLUMNS = 7;
        private const double SQRT3 = 1.7320508075688772;

        // 六边形格子数组
        private HexagonCell[,] _cells;

        // 服务引用
        private IHeroDataService _heroDataService;

        // 当前绑定的子阵容
        private SubLineUp _currentSubLineUp;

        /// <summary>
        /// 英雄位置变更事件（向外部通知数据变更）
        /// </summary>
        public event EventHandler<BoardHeroPositionChangedEventArgs> HeroPositionChanged;

        /// <summary>
        /// 英雄被清除事件
        /// </summary>
        public event EventHandler<BoardHeroClearedEventArgs> HeroCleared;
       

        public HexagonBoard()
        {
            // 启用双缓冲
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // 背景色
            BackColor = Color.FromArgb(30, 35, 45);

            // 初始化格子数组
            _cells = new HexagonCell[ROWS, COLUMNS];

            // 创建所有格子
            CreateCells();
        }

        /// <summary>
        /// 初始化服务引用
        /// </summary>
        /// <param name="heroDataService">英雄数据服务</param>
        public void InitializeServices(IHeroDataService heroDataService)
        {
            _heroDataService = heroDataService;
        }

        /// <summary>
        /// 根据英雄费用获取对应的边框颜色
        /// </summary>
        /// <param name="cost">英雄费用</param>
        /// <returns>对应的颜色</returns>
        private static Color GetColorFromCost(int cost)
        {
            return cost switch
            {
                1 => Color.FromArgb(107, 104, 101),
                2 => Color.FromArgb(5, 171, 117),
                3 => Color.FromArgb(0, 133, 255),
                4 => Color.FromArgb(175, 40, 195),
                5 => Color.FromArgb(245, 158, 11),
                _ => Color.FromArgb(255, 64, 0)
            };
        }

        /// <summary>
        /// 创建所有六边形格子
        /// </summary>
        private void CreateCells()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    // 棋盘坐标从(1,1)开始，数组索引从0开始
                    // 所以Cell的Row/Column = 数组索引 + 1
                    HexagonCell cell = new HexagonCell(row + 1, col + 1);

                    // 绑定事件
                    cell.HeroPositionChanged += Cell_HeroPositionChanged;
                    cell.HeroCleared += Cell_HeroCleared;

                    _cells[row, col] = cell;
                    Controls.Add(cell);
                }
            }
        }

        /// <summary>
        /// 布局所有格子（根据控件大小自适应）
        /// </summary>
        private void LayoutCells()
        {
            if (Width <= 0 || Height <= 0) return;

            // 计算格子大小
            // 总宽度需要容纳7.5个格子宽度（因为偶数行偏移半个格子）
            // 总高度需要容纳 1 + 3 * 0.75 = 3.25 个格子高度

            int padding = LogicalToDeviceUnits(8);
            int availableWidth = Width - padding * 2;
            int availableHeight = Height - padding * 2;

            // 根据宽度计算格子宽度（需要7.5个格子宽度）
            double cellWidthFromWidth = availableWidth / 7.5;

            // 根据高度计算格子宽度（高度 = 宽度 / √3 * 2，需要3.25个格子高度）
            // 格子高度 H = W / √3 * 2 = W * 2 / √3
            // 总高度 = H * (1 + 0.75 * 3) = H * 3.25
            // 所以 H = availableHeight / 3.25
            // W = H * √3 / 2
            double cellHeightFromHeight = availableHeight / 3.25;
            double cellWidthFromHeight = cellHeightFromHeight * SQRT3 / 2;

            // 取较小值确保不超出边界
            double cellWidth = Math.Min(cellWidthFromWidth, cellWidthFromHeight);
            double cellHeight = cellWidth * 2 / SQRT3;

            int intCellWidth = (int)cellWidth;
            int intCellHeight = (int)cellHeight;

            // 计算实际使用的总尺寸，用于居中
            double totalWidth = 7 * cellWidth + cellWidth / 2;
            double totalHeight = cellHeight + 3 * cellHeight * 0.75;

            int startX = padding + (int)((availableWidth - totalWidth) / 2);
            int startY = padding + (int)((availableHeight - totalHeight) / 2);

            // 布局每个格子
            for (int row = 0; row < ROWS; row++)
            {
                // 偶数行（0、2）不偏移，奇数行（1、3）向右偏移半个格子
                double xOffset = (row % 2 == 1) ? cellWidth / 2 : 0;

                for (int col = 0; col < COLUMNS; col++)
                {
                    int x = startX + (int)(col * cellWidth + xOffset);
                    int y = startY + (int)(row * cellHeight * 0.75);

                    _cells[row, col].Location = new Point(x, y);
                    _cells[row, col].Size = new Size(intCellWidth, intCellHeight);
                }
            }
        }

        /// <summary>
        /// 控件大小变化时重新布局
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutCells();
        }

        /// <summary>
        /// 绑定子阵容数据到棋盘
        /// </summary>
        /// <param name="subLineUp">子阵容数据</param>
        public void BindSubLineUp(SubLineUp subLineUp)
        {
            _currentSubLineUp = subLineUp;
            RefreshBoard();
        }

        /// <summary>
        /// 刷新棋盘显示
        /// </summary>
        public void RefreshBoard()
        {
            // 先清空所有格子
            ClearAllCells();

            if (_currentSubLineUp == null) return;

            // 遍历子阵容中的英雄，放置到对应位置
            foreach (LineUpUnit unit in _currentSubLineUp.LineUpUnits)
            {
                if (string.IsNullOrEmpty(unit.HeroName)) continue;

                int row = unit.Position.Item1;
                int col = unit.Position.Item2;

                // (0,0)表示在备战席，不在棋盘上显示
                if (row == 0 && col == 0) continue;

                // 验证位置有效性：棋盘坐标从(1,1)到(4,7)
                if (row < 1 || row > ROWS || col < 1 || col > COLUMNS) continue;

                // 获取英雄数据和图片
                Hero hero = _heroDataService?.GetHeroFromName(unit.HeroName);
                if (hero == null) continue;

                // 获取边框颜色
                Color borderColor = GetColorFromCost(hero.Cost);

                // 数组索引 = 坐标 - 1
                _cells[row - 1, col - 1].SetHero(unit, hero.Image, borderColor);
            }
        }

        /// <summary>
        /// 清空所有格子
        /// </summary>
        public void ClearAllCells()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    _cells[row, col].Clear();
                }
            }
        }

        /// <summary>
        /// 格子英雄位置变更事件处理
        /// </summary>
        private void Cell_HeroPositionChanged(object sender, HeroPositionChangedEventArgs e)
        {
            if (_currentSubLineUp == null) return;

            // e.TargetRow/Column 是棋盘坐标(1-4, 1-7)，需要转换为数组索引
            int targetArrayRow = e.TargetRow - 1;
            int targetArrayCol = e.TargetColumn - 1;

            // 验证目标位置有效性
            if (targetArrayRow < 0 || targetArrayRow >= ROWS || targetArrayCol < 0 || targetArrayCol >= COLUMNS)
            {
                return;
            }

            // 获取目标格子
            HexagonCell targetCell = _cells[targetArrayRow, targetArrayCol];

            // 检查目标位置是否已有英雄
            LineUpUnit targetUnit = targetCell.LineUpUnit;

            // 更新源英雄的位置（使用棋盘坐标）
            if (e.MovedUnit != null)
            {
                e.MovedUnit.Position = (e.TargetRow, e.TargetColumn);
            }

            // 如果目标位置有英雄，交换位置
            // 源位置(0,0)表示从备战席拖来，目标英雄移到备战席
            if (targetUnit != null && !string.IsNullOrEmpty(targetUnit.HeroName))
            {
                targetUnit.Position = (e.SourceRow, e.SourceColumn);
            }

            // 刷新显示
            RefreshBoard();

            // 触发外部事件（用于通知备战席刷新）
            HeroPositionChanged?.Invoke(this, new BoardHeroPositionChangedEventArgs(
                e.SourceRow, e.SourceColumn,
                e.TargetRow, e.TargetColumn,
                e.MovedUnit, targetUnit
            ));
        }

        /// <summary>
        /// 格子英雄清除事件处理
        /// </summary>
        private void Cell_HeroCleared(object sender, HeroClearedEventArgs e)
        {
            if (_currentSubLineUp == null || e.ClearedUnit == null) return;

            // 重置位置为默认值
            e.ClearedUnit.Position = (0, 0);

            // 刷新显示
            RefreshBoard();

            // 触发外部事件
            HeroCleared?.Invoke(this, new BoardHeroClearedEventArgs(e.Row, e.Column, e.ClearedUnit));
        }

        /// <summary>
        /// 获取指定位置的格子（使用棋盘坐标1-4, 1-7）
        /// </summary>
        /// <param name="row">行坐标（1-4）</param>
        /// <param name="column">列坐标（1-7）</param>
        /// <returns>对应的格子，如果坐标无效则返回null</returns>
        public HexagonCell GetCell(int row, int column)
        {
            // 转换为数组索引
            int arrayRow = row - 1;
            int arrayCol = column - 1;

            if (arrayRow < 0 || arrayRow >= ROWS || arrayCol < 0 || arrayCol >= COLUMNS)
            {
                return null;
            }
            return _cells[arrayRow, arrayCol];
        }

        /// <summary>
        /// 计算棋盘推荐高度（根据宽度）
        /// </summary>
        public static int CalculateHeightFromWidth(int width, int padding = 8)
        {
            int availableWidth = width - padding * 2;
            double cellWidth = availableWidth / 7.5;
            double cellHeight = cellWidth * 2 / SQRT3;
            double totalHeight = cellHeight * 3.25;
            return (int)totalHeight + padding * 2;
        }
    }

    /// <summary>
    /// 棋盘英雄位置变更事件参数
    /// </summary>
    public class BoardHeroPositionChangedEventArgs : EventArgs
    {
        public int SourceRow { get; }
        public int SourceColumn { get; }
        public int TargetRow { get; }
        public int TargetColumn { get; }
        public LineUpUnit MovedUnit { get; }
        public LineUpUnit SwappedUnit { get; }

        public BoardHeroPositionChangedEventArgs(int sourceRow, int sourceColumn, int targetRow, int targetColumn, LineUpUnit movedUnit, LineUpUnit swappedUnit)
        {
            SourceRow = sourceRow;
            SourceColumn = sourceColumn;
            TargetRow = targetRow;
            TargetColumn = targetColumn;
            MovedUnit = movedUnit;
            SwappedUnit = swappedUnit;
        }
    }

    /// <summary>
    /// 棋盘英雄清除事件参数
    /// </summary>
    public class BoardHeroClearedEventArgs : EventArgs
    {
        public int Row { get; }
        public int Column { get; }
        public LineUpUnit ClearedUnit { get; }

        public BoardHeroClearedEventArgs(int row, int column, LineUpUnit clearedUnit)
        {
            Row = row;
            Column = column;
            ClearedUnit = clearedUnit;
        }
    }
}
