using System.Drawing.Drawing2D;
using JinChanChanTool.DataClass;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 尖顶六边形格子控件，用于蜂巢棋盘布局
    /// 支持显示英雄头像、拖拽移动、右键清除
    /// </summary>
    public class HexagonCell : Control
    {
        // 六边形几何常量
        private const double SQRT3 = 1.7320508075688772; // √3

        // 格子在棋盘中的行列位置
        private int _row;
        private int _column;

        // 绑定的英雄数据
        private LineUpUnit _lineUpUnit;
        private Image _heroImage;
        private Color _borderColor = Color.FromArgb(100, 150, 180);

        // 拖拽相关
        private bool _isDragging;
        private bool _isDropTarget;

        // 外观设置
        private Color _emptyFillColor = Color.FromArgb(40, 45, 55);
        private Color _occupiedFillColor = Color.FromArgb(50, 60, 75);
        private Color _hoverColor = Color.FromArgb(70, 85, 100);
        private Color _dropTargetColor = Color.FromArgb(80, 180, 80);
        private bool _isHovering;

        /// <summary>
        /// 格子所在行（0-3）
        /// </summary>
        public int Row
        {
            get => _row;
            set => _row = value;
        }

        /// <summary>
        /// 格子所在列（0-6）
        /// </summary>
        public int Column
        {
            get => _column;
            set => _column = value;
        }

        /// <summary>
        /// 绑定的阵容单位数据
        /// </summary>
        public LineUpUnit LineUpUnit
        {
            get => _lineUpUnit;
            set
            {
                _lineUpUnit = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 英雄头像图片
        /// </summary>
        public Image HeroImage
        {
            get => _heroImage;
            set
            {
                _heroImage = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 边框颜色（根据英雄费用显示不同颜色）
        /// </summary>
        public Color HeroBorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 是否为拖拽放置目标
        /// </summary>
        public bool IsDropTarget
        {
            get => _isDropTarget;
            set
            {
                if (_isDropTarget != value)
                {
                    _isDropTarget = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 格子是否有英雄
        /// </summary>
        public bool HasHero => _lineUpUnit != null && !string.IsNullOrEmpty(_lineUpUnit.HeroName);

        /// <summary>
        /// 英雄位置变更事件（拖拽完成时触发）
        /// </summary>
        public event EventHandler<HeroPositionChangedEventArgs> HeroPositionChanged;

        /// <summary>
        /// 英雄清除事件（右键清除时触发）
        /// </summary>
        public event EventHandler<HeroClearedEventArgs> HeroCleared;

        /// <summary>
        /// 开始拖拽事件
        /// </summary>
        public event EventHandler<HeroDragStartEventArgs> HeroDragStart;

        public HexagonCell()
        {
            // 启用双缓冲减少闪烁
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            // 默认大小
            Size = new Size(50, 58);

            // 允许接收拖放
            AllowDrop = true;
        }

        /// <summary>
        /// 控件大小变化时更新六边形区域
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateHexagonRegion();
        }

        /// <summary>
        /// 更新控件的六边形区域，使控件形状为六边形而非矩形
        /// </summary>
        private void UpdateHexagonRegion()
        {
            if (Width <= 0 || Height <= 0) return;

            using (GraphicsPath path = GetHexagonPath())
            {
                // 设置控件的Region为六边形，这样控件本身就是六边形
                // 六边形以外的区域不属于控件，不会遮挡其他控件
                Region = new Region(path);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="row">行位置</param>
        /// <param name="column">列位置</param>
        public HexagonCell(int row, int column) : this()
        {
            _row = row;
            _column = column;
        }

        /// <summary>
        /// 计算尖顶六边形的顶点
        /// </summary>
        private PointF[] GetHexagonPoints()
        {
            float cx = Width / 2f;
            float cy = Height / 2f;

            // 外接圆半径（基于控件高度）
            float radius = Math.Min(Width / (float)SQRT3, Height / 2f) * 0.95f;

            PointF[] points = new PointF[6];

            // 尖顶六边形，从顶部顶点开始顺时针
            for (int i = 0; i < 6; i++)
            {
                // 角度从-90度开始（指向上方），每隔60度
                double angleDeg = -90 + i * 60;
                double angleRad = angleDeg * Math.PI / 180;
                points[i] = new PointF(
                    cx + radius * (float)Math.Cos(angleRad),
                    cy + radius * (float)Math.Sin(angleRad)
                );
            }

            return points;
        }

        /// <summary>
        /// 创建六边形裁剪区域
        /// </summary>
        private GraphicsPath GetHexagonPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(GetHexagonPoints());
            return path;
        }

        /// <summary>
        /// 判断点是否在六边形内
        /// </summary>
        public bool IsPointInHexagon(Point pt)
        {
            using (GraphicsPath path = GetHexagonPath())
            {
                return path.IsVisible(pt);
            }
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            PointF[] hexPoints = GetHexagonPoints();

            // 确定填充颜色
            Color fillColor;
            if (_isDropTarget)
            {
                fillColor = _dropTargetColor;
            }
            else if (_isHovering)
            {
                fillColor = _hoverColor;
            }
            else if (HasHero)
            {
                fillColor = _occupiedFillColor;
            }
            else
            {
                fillColor = _emptyFillColor;
            }

            // 绘制六边形背景
            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                g.FillPolygon(fillBrush, hexPoints);
            }

            // 绘制英雄头像（填充整个六边形）
            if (_heroImage != null && HasHero)
            {
                using (GraphicsPath clipPath = GetHexagonPath())
                {
                    // 保存原始裁剪区域
                    Region oldClip = g.Clip;

                    // 设置六边形裁剪区域
                    g.SetClip(clipPath);

                    // 图片填充整个控件区域，由六边形Region裁剪
                    g.DrawImage(_heroImage, 0, 0, Width, Height);

                    // 恢复裁剪区域
                    g.Clip = oldClip;
                }
            }

            // 绘制边框
            Color borderColorToUse = HasHero ? _borderColor : Color.FromArgb(80, 100, 120);
            int borderWidth = HasHero ? 3 : 1;

            using (Pen borderPen = new Pen(borderColorToUse, borderWidth))
            {
                g.DrawPolygon(borderPen, hexPoints);
            }
        }

        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovering = true;
            Invalidate();
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovering = false;
            Invalidate();
        }

        /// <summary>
        /// 鼠标按下事件 - 开始拖拽
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && HasHero && IsPointInHexagon(e.Location))
            {
                _isDragging = true;

                // 触发开始拖拽事件
                HeroDragStart?.Invoke(this, new HeroDragStartEventArgs(this));

                // 开始拖放操作
                DoDragDrop(this, DragDropEffects.Move);

                _isDragging = false;
            }
        }

        /// <summary>
        /// 鼠标释放事件 - 右键清除
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right && HasHero && IsPointInHexagon(e.Location))
            {
                // 触发清除事件
                HeroCleared?.Invoke(this, new HeroClearedEventArgs(_row, _column, _lineUpUnit));
            }
        }

        /// <summary>
        /// 拖放进入事件
        /// </summary>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            // 接受来自棋盘格子的拖放
            if (e.Data?.GetData(typeof(HexagonCell)) is HexagonCell sourceCell && sourceCell != this)
            {
                e.Effect = DragDropEffects.Move;
                IsDropTarget = true;
            }
            // 接受来自备战席的拖放
            else if (e.Data?.GetData(typeof(BenchSlot)) is BenchSlot benchSlot && benchSlot.HasHero)
            {
                e.Effect = DragDropEffects.Move;
                IsDropTarget = true;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// 拖放离开事件
        /// </summary>
        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            IsDropTarget = false;
        }

        /// <summary>
        /// 拖放完成事件
        /// </summary>
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            IsDropTarget = false;

            // 处理来自棋盘格子的拖放
            if (e.Data?.GetData(typeof(HexagonCell)) is HexagonCell sourceCell && sourceCell != this)
            {
                // 触发位置变更事件
                HeroPositionChanged?.Invoke(this, new HeroPositionChangedEventArgs(
                    sourceCell.Row, sourceCell.Column,
                    _row, _column,
                    sourceCell.LineUpUnit
                ));
            }
            // 处理来自备战席的拖放
            else if (e.Data?.GetData(typeof(BenchSlot)) is BenchSlot benchSlot && benchSlot.HasHero)
            {
                // 从备战席拖到棋盘，源位置是(0,0)
                HeroPositionChanged?.Invoke(this, new HeroPositionChangedEventArgs(
                    0, 0,
                    _row, _column,
                    benchSlot.LineUpUnit
                ));
            }
        }

        /// <summary>
        /// 清除格子上的英雄
        /// </summary>
        public void Clear()
        {
            _lineUpUnit = null;
            _heroImage = null;
            _borderColor = Color.FromArgb(100, 150, 180);
            Invalidate();
        }

        /// <summary>
        /// 设置格子上的英雄
        /// </summary>
        /// <param name="unit">阵容单位</param>
        /// <param name="image">英雄头像</param>
        /// <param name="borderColor">边框颜色</param>
        public void SetHero(LineUpUnit unit, Image image, Color borderColor)
        {
            _lineUpUnit = unit;
            _heroImage = image;
            _borderColor = borderColor;
            Invalidate();
        }

        /// <summary>
        /// 覆盖自动缩放方法，禁用DPI自动缩放
        /// 父控件HexagonBoard会在OnResize中手动计算并设置正确的尺寸
        /// 避免与窗体的AutoScaleMode.Dpi产生双重缩放问题
        /// </summary>
        /// <param name="factor">缩放因子</param>
        /// <param name="specified">指定缩放的边界</param>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            // 不执行自动缩放，由父控件手动管理尺寸
        }
    }

    /// <summary>
    /// 英雄位置变更事件参数
    /// </summary>
    public class HeroPositionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 源位置行
        /// </summary>
        public int SourceRow { get; }

        /// <summary>
        /// 源位置列
        /// </summary>
        public int SourceColumn { get; }

        /// <summary>
        /// 目标位置行
        /// </summary>
        public int TargetRow { get; }

        /// <summary>
        /// 目标位置列
        /// </summary>
        public int TargetColumn { get; }

        /// <summary>
        /// 被移动的阵容单位
        /// </summary>
        public LineUpUnit MovedUnit { get; }

        public HeroPositionChangedEventArgs(int sourceRow, int sourceColumn, int targetRow, int targetColumn, LineUpUnit movedUnit)
        {
            SourceRow = sourceRow;
            SourceColumn = sourceColumn;
            TargetRow = targetRow;
            TargetColumn = targetColumn;
            MovedUnit = movedUnit;
        }
    }

    /// <summary>
    /// 英雄清除事件参数
    /// </summary>
    public class HeroClearedEventArgs : EventArgs
    {
        /// <summary>
        /// 格子行位置
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// 格子列位置
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// 被清除的阵容单位
        /// </summary>
        public LineUpUnit ClearedUnit { get; }

        public HeroClearedEventArgs(int row, int column, LineUpUnit clearedUnit)
        {
            Row = row;
            Column = column;
            ClearedUnit = clearedUnit;
        }
    }

    /// <summary>
    /// 英雄开始拖拽事件参数
    /// </summary>
    public class HeroDragStartEventArgs : EventArgs
    {
        /// <summary>
        /// 源格子
        /// </summary>
        public HexagonCell SourceCell { get; }

        public HeroDragStartEventArgs(HexagonCell sourceCell)
        {
            SourceCell = sourceCell;
        }
    }
}
