using JinChanChanTool.DataClass;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 棋盘拖拽管理器 - 使用手动鼠标捕获替代 OLE DoDragDrop
    /// 解决 TransparencyKey 导致的 COMException 问题
    /// 协调 HexagonBoard 和 BenchPanel 之间的跨控件拖拽
    ///
    /// 拖拽场景：
    /// 1. HexagonCell → HexagonCell（棋盘内交换位置）
    /// 2. BenchSlot → HexagonCell（备战席到棋盘）
    /// 3. HexagonCell → BenchPanel（棋盘到备战席）
    /// </summary>
    public class BoardDragManager
    {
        // 拖拽启动阈值（像素），防止误触发
        private const int DRAG_THRESHOLD = 5;

        // 间隙命中测试的最大距离系数（相对于格子宽度）
        private const double GAP_HIT_TEST_RATIO = 0.6;

        /// <summary>
        /// 拖拽来源类型
        /// </summary>
        private enum DragSourceType
        {
            /// <summary>
            /// 无拖拽
            /// </summary>
            None,

            /// <summary>
            /// 来自棋盘六边形格子
            /// </summary>
            HexagonCell,

            /// <summary>
            /// 来自备战席格子
            /// </summary>
            BenchSlot
        }

        /// <summary>
        /// 命中测试结果类型
        /// </summary>
        private enum HitTestResultType
        {
            /// <summary>
            /// 未命中任何有效区域
            /// </summary>
            None,

            /// <summary>
            /// 命中棋盘格子
            /// </summary>
            HexagonCell,

            /// <summary>
            /// 命中备战席区域
            /// </summary>
            BenchPanel
        }

        // 拖拽状态
        private bool _isDragActive;          // 是否正在拖拽中
        private bool _isDragStartPending;    // 鼠标已按下，等待超过阈值启动拖拽
        private Point _mouseDownScreenPoint; // 鼠标按下时的屏幕坐标
        private DragSourceType _dragSourceType; // 拖拽来源类型
        private Control _capturedControl;    // 当前捕获鼠标的控件

        // 拖拽源数据
        private HexagonCell _sourceCell;     // 拖拽源格子（来自棋盘时）
        private BenchSlot _sourceBenchSlot;  // 拖拽源格子（来自备战席时）

        // 当前高亮的目标格子
        private HexagonCell _currentHighlightedCell;

        // 引用的控件
        private Form _hostForm;              // 宿主窗体
        private HexagonBoard _hexagonBoard;  // 棋盘控件
        private BenchPanel _benchPanel;      // 备战席控件

        /// <summary>
        /// 初始化拖拽管理器，绑定所有子控件的鼠标事件
        /// </summary>
        /// <param name="hostForm">宿主窗体（用于坐标转换和键盘监听）</param>
        /// <param name="hexagonBoard">棋盘控件</param>
        /// <param name="benchPanel">备战席控件</param>
        public void Initialize(Form hostForm, HexagonBoard hexagonBoard, BenchPanel benchPanel)
        {
            _hostForm = hostForm;
            _hexagonBoard = hexagonBoard;
            _benchPanel = benchPanel;

            // 为每个棋盘格子绑定鼠标事件
            foreach (HexagonCell cell in _hexagonBoard.GetAllCells())
            {
                cell.MouseDown += OnCellMouseDown;
                cell.MouseMove += OnMouseMove;
                cell.MouseUp += OnMouseUp;
            }

            // 为每个备战席格子绑定鼠标事件
            foreach (BenchSlot slot in _benchPanel.GetAllSlots())
            {
                slot.MouseDown += OnBenchSlotMouseDown;
                slot.MouseMove += OnMouseMove;
                slot.MouseUp += OnMouseUp;
            }

            // 监听 Escape 键取消拖拽
            _hostForm.KeyPreview = true;
            _hostForm.KeyDown += OnKeyDown;
        }

        /// <summary>
        /// 棋盘格子鼠标按下事件 - 准备从棋盘开始拖拽
        /// </summary>
        private void OnCellMouseDown(object sender, MouseEventArgs e)
        {
            // 只响应左键，且格子上有英雄
            if (e.Button != MouseButtons.Left) return;

            HexagonCell cell = (HexagonCell)sender;
            if (!cell.HasHero || !cell.IsPointInHexagon(e.Location)) return;

            // 记录拖拽源信息
            _dragSourceType = DragSourceType.HexagonCell;
            _sourceCell = cell;
            _sourceBenchSlot = null;
            _isDragStartPending = true;
            _isDragActive = false;
            _mouseDownScreenPoint = cell.PointToScreen(e.Location);

            // 捕获鼠标，使该控件接收所有鼠标消息（即使鼠标移出控件范围）
            _capturedControl = cell;
            cell.Capture = true;
        }

        /// <summary>
        /// 备战席格子鼠标按下事件 - 准备从备战席开始拖拽
        /// </summary>
        private void OnBenchSlotMouseDown(object sender, MouseEventArgs e)
        {
            // 只响应左键，且格子上有英雄
            if (e.Button != MouseButtons.Left) return;

            BenchSlot slot = (BenchSlot)sender;
            if (!slot.HasHero) return;

            // 记录拖拽源信息
            _dragSourceType = DragSourceType.BenchSlot;
            _sourceBenchSlot = slot;
            _sourceCell = null;
            _isDragStartPending = true;
            _isDragActive = false;
            _mouseDownScreenPoint = slot.PointToScreen(e.Location);

            // 捕获鼠标
            _capturedControl = slot;
            slot.Capture = true;
        }

        /// <summary>
        /// 鼠标移动事件 - 判断是否启动拖拽 + 更新目标高亮
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // 未按下或已完成，不处理
            if (!_isDragStartPending && !_isDragActive) return;

            Control senderControl = (Control)sender;
            Point currentScreenPoint = senderControl.PointToScreen(e.Location);

            // 阶段1：等待超过阈值才正式启动拖拽
            if (_isDragStartPending && !_isDragActive)
            {
                double deltaX = currentScreenPoint.X - _mouseDownScreenPoint.X;
                double deltaY = currentScreenPoint.Y - _mouseDownScreenPoint.Y;
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                if (distance <= DRAG_THRESHOLD) return;

                // 超过阈值，正式启动拖拽
                _isDragActive = true;
                _isDragStartPending = false;

                // 触发拖拽开始事件（保持现有事件架构）
                if (_dragSourceType == DragSourceType.HexagonCell && _sourceCell != null)
                {
                    _sourceCell.InvokeHeroDragStart();
                }
                else if (_dragSourceType == DragSourceType.BenchSlot && _sourceBenchSlot != null)
                {
                    _sourceBenchSlot.InvokeHeroDragStarted();
                }
            }

            // 阶段2：拖拽进行中，更新视觉反馈
            if (_isDragActive)
            {
                UpdateDropTargetHighlight(currentScreenPoint);
            }
        }

        /// <summary>
        /// 鼠标释放事件 - 执行放置或取消拖拽
        /// </summary>
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            // 只处理左键释放
            if (e.Button != MouseButtons.Left) return;

            // 未在拖拽流程中，不处理
            if (!_isDragStartPending && !_isDragActive) return;

            // 未达到拖拽阈值，视为普通点击，直接清理
            if (!_isDragActive)
            {
                CleanupDrag();
                return;
            }

            // 拖拽进行中，执行放置逻辑
            Control senderControl = (Control)sender;
            Point screenPoint = senderControl.PointToScreen(e.Location);

            // 命中测试确定最终目标
            HitTestResult hitResult = HitTest(screenPoint);

            // 根据拖拽源和目标类型执行对应操作
            ExecuteDrop(hitResult);

            // 清理所有拖拽状态
            CleanupDrag();
        }

        /// <summary>
        /// 键盘按下事件 - Escape 取消拖拽
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && (_isDragStartPending || _isDragActive))
            {
                CleanupDrag();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 更新拖拽目标的高亮显示
        /// </summary>
        /// <param name="screenPoint">当前鼠标屏幕坐标</param>
        private void UpdateDropTargetHighlight(Point screenPoint)
        {
            HitTestResult hitResult = HitTest(screenPoint);

            // 确定新的高亮目标格子
            HexagonCell newHighlightCell = null;

            if (hitResult.Type == HitTestResultType.HexagonCell && hitResult.TargetCell != null)
            {
                // 不高亮拖拽源自身
                bool isSameAsSource = (_dragSourceType == DragSourceType.HexagonCell
                    && hitResult.TargetCell == _sourceCell);

                if (!isSameAsSource)
                {
                    newHighlightCell = hitResult.TargetCell;
                }
            }

            // 更新高亮状态（只在变化时更新，避免不必要的重绘）
            if (_currentHighlightedCell != newHighlightCell)
            {
                // 清除旧高亮
                if (_currentHighlightedCell != null)
                {
                    _currentHighlightedCell.IsDropTarget = false;
                }

                // 设置新高亮
                if (newHighlightCell != null)
                {
                    newHighlightCell.IsDropTarget = true;
                }

                _currentHighlightedCell = newHighlightCell;
            }
        }

        /// <summary>
        /// 执行拖拽放置操作
        /// </summary>
        /// <param name="hitResult">命中测试结果</param>
        private void ExecuteDrop(HitTestResult hitResult)
        {
            // 场景1：HexagonCell → HexagonCell（棋盘内交换）
            if (_dragSourceType == DragSourceType.HexagonCell
                && hitResult.Type == HitTestResultType.HexagonCell
                && hitResult.TargetCell != null
                && hitResult.TargetCell != _sourceCell
                && _sourceCell != null)
            {
                // 通过目标格子触发 HeroPositionChanged 事件，复用现有事件链
                hitResult.TargetCell.InvokeHeroPositionChanged(
                    _sourceCell.Row, _sourceCell.Column,
                    _sourceCell.LineUpUnit
                );
                return;
            }

            // 场景2：BenchSlot → HexagonCell（备战席到棋盘）
            if (_dragSourceType == DragSourceType.BenchSlot
                && hitResult.Type == HitTestResultType.HexagonCell
                && hitResult.TargetCell != null
                && _sourceBenchSlot != null)
            {
                // 源位置 (0,0) 表示来自备战席
                hitResult.TargetCell.InvokeHeroPositionChanged(
                    0, 0,
                    _sourceBenchSlot.LineUpUnit
                );
                return;
            }

            // 场景3：HexagonCell → BenchPanel（棋盘到备战席）
            if (_dragSourceType == DragSourceType.HexagonCell
                && hitResult.Type == HitTestResultType.BenchPanel
                && _sourceCell != null
                && _sourceCell.HasHero
                && !_benchPanel.IsFull)
            {
                // 触发备战席的 HeroDroppedIn 事件
                _benchPanel.InvokeHeroDroppedIn(
                    _sourceCell.Row, _sourceCell.Column,
                    _sourceCell.LineUpUnit
                );
                return;
            }

            // 其他情况（无效目标、源=目标等）：不执行任何操作
        }

        /// <summary>
        /// 命中测试 - 根据屏幕坐标判断鼠标落在哪个目标上
        /// </summary>
        /// <param name="screenPoint">鼠标屏幕坐标</param>
        /// <returns>命中测试结果</returns>
        private HitTestResult HitTest(Point screenPoint)
        {
            // 1. 检查是否在棋盘区域内
            if (_hexagonBoard.Visible)
            {
                Point boardLocalPoint = _hexagonBoard.PointToClient(screenPoint);

                if (_hexagonBoard.ClientRectangle.Contains(boardLocalPoint))
                {
                    HexagonCell targetCell = FindCellAtPoint(boardLocalPoint);
                    if (targetCell != null)
                    {
                        return new HitTestResult(HitTestResultType.HexagonCell, targetCell);
                    }
                }
            }

            // 2. 检查是否在备战席区域内
            if (_benchPanel.Visible)
            {
                Point benchLocalPoint = _benchPanel.PointToClient(screenPoint);

                if (_benchPanel.ClientRectangle.Contains(benchLocalPoint))
                {
                    return new HitTestResult(HitTestResultType.BenchPanel, null);
                }
            }

            // 3. 未命中任何有效区域
            return new HitTestResult(HitTestResultType.None, null);
        }

        /// <summary>
        /// 在棋盘中查找指定坐标处的格子
        /// 先精确匹配六边形区域，若在间隙中则选择最近的格子
        /// </summary>
        /// <param name="boardLocalPoint">相对于棋盘控件的本地坐标</param>
        /// <returns>命中的格子，未命中返回 null</returns>
        private HexagonCell FindCellAtPoint(Point boardLocalPoint)
        {
            HexagonCell closestCell = null;
            double closestDistance = double.MaxValue;

            foreach (HexagonCell cell in _hexagonBoard.GetAllCells())
            {
                // 将棋盘坐标转换为格子本地坐标
                Point cellLocalPoint = new Point(
                    boardLocalPoint.X - cell.Location.X,
                    boardLocalPoint.Y - cell.Location.Y
                );

                // 精确匹配：点在格子矩形范围内且在六边形内
                if (cell.ClientRectangle.Contains(cellLocalPoint) && cell.IsPointInHexagon(cellLocalPoint))
                {
                    return cell;
                }

                // 计算到格子中心的距离（用于间隙回退）
                int cellCenterX = cell.Location.X + cell.Width / 2;
                int cellCenterY = cell.Location.Y + cell.Height / 2;
                double deltaX = boardLocalPoint.X - cellCenterX;
                double deltaY = boardLocalPoint.Y - cellCenterY;
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCell = cell;
                }
            }

            // 间隙回退：如果点在棋盘内但不在任何六边形内，
            // 选择最近的格子（距离不超过格子宽度的指定比例）
            if (closestCell != null && closestDistance < closestCell.Width * GAP_HIT_TEST_RATIO)
            {
                return closestCell;
            }

            return null;
        }

        /// <summary>
        /// 清理所有拖拽状态，重置为初始状态
        /// </summary>
        private void CleanupDrag()
        {
            // 释放鼠标捕获
            if (_capturedControl != null)
            {
                _capturedControl.Capture = false;
                _capturedControl = null;
            }

            // 清除目标高亮
            if (_currentHighlightedCell != null)
            {
                _currentHighlightedCell.IsDropTarget = false;
                _currentHighlightedCell = null;
            }

            // 重置所有状态
            _isDragActive = false;
            _isDragStartPending = false;
            _dragSourceType = DragSourceType.None;
            _sourceCell = null;
            _sourceBenchSlot = null;
        }

        /// <summary>
        /// 命中测试结果
        /// </summary>
        private struct HitTestResult
        {
            /// <summary>
            /// 命中的目标类型
            /// </summary>
            public HitTestResultType Type { get; }

            /// <summary>
            /// 命中的棋盘格子（仅当 Type 为 HexagonCell 时有值）
            /// </summary>
            public HexagonCell TargetCell { get; }

            public HitTestResult(HitTestResultType type, HexagonCell targetCell)
            {
                Type = type;
                TargetCell = targetCell;
            }
        }
    }
}
