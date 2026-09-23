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
        public const int MaxStackedHeroes = 6;
        // 六边形几何常量
        private const double SQRT3 = 1.7320508075688772; // √3
        private const int LONG_PRESS_DELAY = 450;

        // 格子在棋盘中的行列位置
        private int _row;
        private int _column;

        // 绑定的英雄数据
        private readonly List<StackedHeroDisplay> _heroes = [];
        private int _displayIndex = -1;
        private readonly System.Windows.Forms.Timer _longPressTimer;
        private Point _mouseDownPoint;
        private bool _isMouseDown;

        // 拖拽相关
        private bool _isDropTarget;

        // 外观设置
        private readonly Color _emptyFillColor = Color.FromArgb(40, 45, 55);
        private readonly Color _occupiedFillColor = Color.FromArgb(50, 60, 75);
        private readonly Color _hoverColor = Color.FromArgb(70, 85, 100);
        private readonly Color _dropTargetColor = Color.FromArgb(80, 180, 80);
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
            get => GetDisplayedHero()?.Unit;
            set
            {
                if (value == null)
                {
                    Clear();
                    return;
                }

                int index = _heroes.FindIndex(hero => ReferenceEquals(hero.Unit, value));
                if (index >= 0)
                {
                    _displayIndex = index;
                }
                else
                {
                    _heroes.Clear();
                    _heroes.Add(new StackedHeroDisplay(value, null, Color.FromArgb(100, 150, 180)));
                    _displayIndex = 0;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// 英雄头像图片
        /// </summary>
        public Image HeroImage
        {
            get => GetDisplayedHero()?.Image;
            set
            {
                StackedHeroDisplay displayedHero = GetDisplayedHero();
                if (displayedHero != null)
                {
                    displayedHero.Image = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// 边框颜色（根据英雄费用显示不同颜色）
        /// </summary>
        public Color HeroBorderColor
        {
            get => GetDisplayedHero()?.BorderColor ?? Color.FromArgb(100, 150, 180);
            set
            {
                StackedHeroDisplay displayedHero = GetDisplayedHero();
                if (displayedHero != null)
                {
                    displayedHero.BorderColor = value;
                }
                Invalidate();
            }
        }

        public IReadOnlyList<LineUpUnit> StackUnits => _heroes.Select(hero => hero.Unit).ToList();

        public bool HasStackCapacity => _heroes.Count < MaxStackedHeroes;

        public bool CanAcceptHero(LineUpUnit unit)
        {
            return unit != null && HasStackCapacity;
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
        public bool HasHero => GetDisplayedHero() != null;

        public bool IsLongPressTriggered { get; private set; }

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

        public event EventHandler<HeroStackSelectionRequestedEventArgs> HeroStackSelectionRequested;

        public HexagonCell()
        {
            // 启用双缓冲减少闪烁
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            // 默认大小
            Size = new Size(50, 58);

            _longPressTimer = new System.Windows.Forms.Timer { Interval = LONG_PRESS_DELAY };
            _longPressTimer.Tick += LongPressTimer_Tick;

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _longPressTimer.Dispose();
            }
            base.Dispose(disposing);
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

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            PointF[] hexPoints = GetHexagonPoints();
            StackedHeroDisplay displayedHero = GetDisplayedHero();

            Color fillColor = _isDropTarget
                ? _dropTargetColor
                : _isHovering
                    ? _hoverColor
                    : displayedHero != null ? _occupiedFillColor : _emptyFillColor;

            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                e.Graphics.FillPolygon(fillBrush, hexPoints);
            }

            if (displayedHero?.Image != null)
            {
                using GraphicsPath clipPath = GetHexagonPath();
                Region oldClip = e.Graphics.Clip;
                e.Graphics.SetClip(clipPath);
                e.Graphics.DrawImage(displayedHero.Image, 0, 0, Width, Height);
                e.Graphics.Clip = oldClip;
            }

            Color borderColor = displayedHero?.BorderColor ?? Color.FromArgb(80, 100, 120);
            int borderWidth = displayedHero == null ? 1 : 3;
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawPolygon(borderPen, hexPoints);
            }

            if (_heroes.Count > 1)
            {
                int badgeSize = Math.Max(16, Math.Min(22, Width / 3));
                Rectangle badgeBounds = new Rectangle(Width - badgeSize - 4, 4, badgeSize, badgeSize);
                using SolidBrush badgeBrush = new SolidBrush(Color.FromArgb(220, 20, 20, 20));
                using SolidBrush textBrush = new SolidBrush(Color.White);
                using Font badgeFont = new Font(Font.FontFamily, Math.Max(7, badgeSize / 2.2f), FontStyle.Bold);
                using StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.FillEllipse(badgeBrush, badgeBounds);
                e.Graphics.DrawString(_heroes.Count.ToString(), badgeFont, textBrush, badgeBounds, stringFormat);
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left || !HasHero || !IsPointInHexagon(e.Location))
            {
                return;
            }

            _mouseDownPoint = e.Location;
            _isMouseDown = true;
            IsLongPressTriggered = false;
            _longPressTimer.Start();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isMouseDown &&
                (Math.Abs(e.Location.X - _mouseDownPoint.X) > 5 || Math.Abs(e.Location.Y - _mouseDownPoint.Y) > 5))
            {
                _longPressTimer.Stop();
            }
        }

        /// <summary>
        /// 鼠标释放事件 - 右键清除
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isMouseDown = false;
            _longPressTimer.Stop();
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right && HasHero && IsPointInHexagon(e.Location))
            {
                // 触发清除事件
                HeroCleared?.Invoke(this, new HeroClearedEventArgs(_row, _column, LineUpUnit));
            }
        }

        private void LongPressTimer_Tick(object? sender, EventArgs e)
        {
            _longPressTimer.Stop();
            if (!_isMouseDown || !HasHero)
            {
                return;
            }

            IsLongPressTriggered = true;
            HeroStackSelectionRequested?.Invoke(this, new HeroStackSelectionRequestedEventArgs(this));
        }


        /// <summary>
        /// 触发英雄位置变更事件（供 BoardDragManager 调用）
        /// </summary>
        /// <param name="sourceRow">源位置行</param>
        /// <param name="sourceColumn">源位置列</param>
        /// <param name="movedUnit">被移动的阵容单位</param>
        public void InvokeHeroPositionChanged(int sourceRow, int sourceColumn, LineUpUnit movedUnit)
        {
            HeroPositionChanged?.Invoke(this, new HeroPositionChangedEventArgs(
                sourceRow, sourceColumn,
                _row, _column,
                movedUnit
            ));
        }

        /// <summary>
        /// 触发开始拖拽事件（供 BoardDragManager 调用）
        /// </summary>
        public void InvokeHeroDragStart()
        {
            HeroDragStart?.Invoke(this, new HeroDragStartEventArgs(this));
        }

        /// <summary>
        /// 清除格子上的英雄
        /// </summary>
        public void Clear()
        {
            _heroes.Clear();
            _displayIndex = -1;
            IsLongPressTriggered = false;
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
            SetHeroes([new StackedHeroDisplay(unit, image, borderColor)]);
        }

        /// <summary>
        /// 设置同一站位上的全部英雄。PositionLayer 越大的英雄默认显示在最上层。
        /// </summary>
        public void SetHeroes(IEnumerable<StackedHeroDisplay> heroes)
        {
            _heroes.Clear();
            if (heroes == null)
            {
                _displayIndex = -1;
                IsLongPressTriggered = false;
                Invalidate();
                return;
            }

            _heroes.AddRange(heroes.Where(hero => hero?.Unit != null).OrderBy(hero => hero.Unit.PositionLayer));
            _displayIndex = _heroes.Count - 1;
            IsLongPressTriggered = false;
            Invalidate();
        }

        /// <summary>普通左键在同一站位的英雄之间循环显示。</summary>
        public void CycleDisplayedHero()
        {
            if (_heroes.Count > 1)
            {
                _displayIndex = (_displayIndex - 1 + _heroes.Count) % _heroes.Count;
                Invalidate();
            }
            IsLongPressTriggered = false;
        }

        /// <summary>轮盘选择指定英雄作为当前显示英雄。</summary>
        public void ShowHero(LineUpUnit unit)
        {
            int index = _heroes.FindIndex(hero => ReferenceEquals(hero.Unit, unit));
            if (index >= 0)
            {
                _displayIndex = index;
                Invalidate();
            }
        }

        private StackedHeroDisplay GetDisplayedHero()
        {
            return _displayIndex >= 0 && _displayIndex < _heroes.Count ? _heroes[_displayIndex] : null;
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

    /// <summary>棋盘格子中一个可显示的英雄层。</summary>
    public sealed class StackedHeroDisplay
    {
        public LineUpUnit Unit { get; }
        public Image Image { get; set; }
        public Color BorderColor { get; set; }

        public StackedHeroDisplay(LineUpUnit unit, Image image, Color borderColor)
        {
            Unit = unit;
            Image = image;
            BorderColor = borderColor;
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

        // Alias used by the stacked-board implementation.
        public HexagonCell Cell => SourceCell;

        public HeroDragStartEventArgs(HexagonCell sourceCell)
        {
            SourceCell = sourceCell;
        }
    }

    public class HeroStackSelectionRequestedEventArgs : EventArgs
    {
        public HexagonCell Cell { get; }

        public HeroStackSelectionRequestedEventArgs(HexagonCell cell)
        {
            Cell = cell;
        }
    }
}
