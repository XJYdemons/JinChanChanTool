using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices.Interface;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 备战席控件，用于显示位置为(0,0)的英雄
    /// 支持拖拽英雄到棋盘，以及从棋盘接收英雄
    /// </summary>
    public class BenchPanel : Panel
    {
        // 备战席常量
        private const int MaxHeroCount = 10;

        // 备战席格子数组
        private BenchSlot[] _slots;

        // 服务引用
        private IHeroDataService _heroDataService;

        // 当前绑定的子阵容
        private SubLineUp _currentSubLineUp;

        /// <summary>
        /// 英雄从备战席拖出事件
        /// </summary>
        public event EventHandler<BenchHeroDraggedOutEventArgs> HeroDraggedOut;

        /// <summary>
        /// 英雄位置变更事件（从棋盘拖到备战席时触发）
        /// </summary>
        public event EventHandler<BenchHeroDroppedInEventArgs> HeroDroppedIn;


        public BenchPanel()
        {
            // 启用双缓冲
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // 背景色
            BackColor = Color.FromArgb(35, 40, 50);

            // 允许拖放
            AllowDrop = true;

            // 初始化格子数组
            _slots = new BenchSlot[MaxHeroCount];

            // 创建所有格子
            CreateSlots();
        }

        /// <summary>
        /// 初始化服务引用
        /// </summary>
        public void InitializeServices(IHeroDataService heroDataService)
        {
            _heroDataService = heroDataService;
        }

        /// <summary>
        /// 根据英雄费用获取对应的边框颜色
        /// </summary>
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
        /// 创建所有备战席格子
        /// </summary>
        private void CreateSlots()
        {
            for (int i = 0; i < MaxHeroCount; i++)
            {
                BenchSlot slot = new BenchSlot(i);

                // 绑定事件
                slot.HeroDragStarted += Slot_HeroDragStarted;

                _slots[i] = slot;
                Controls.Add(slot);
            }
        }

        /// <summary>
        /// 布局所有格子
        /// </summary>
        private void LayoutSlots()
        {
            if (Width <= 0 || Height <= 0) return;

            int padding = LogicalToDeviceUnits(4);
            int spacing = LogicalToDeviceUnits(2);

            // 计算格子大小（正方形）
            int availableWidth = Width - padding * 2;
            int maxSlotWidth = (availableWidth - spacing * (MaxHeroCount - 1)) / MaxHeroCount;

            // 确保格子不超过高度
            int maxSlotHeight = Height - padding * 2;
            maxSlotWidth = Math.Min(maxSlotWidth, maxSlotHeight);

            // 计算起始位置（居中）
            int totalWidth = maxSlotWidth * MaxHeroCount + spacing * (MaxHeroCount - 1);
            int startX = padding + (availableWidth - totalWidth) / 2;
            int startY = (Height - maxSlotWidth) / 2;

            // 布局每个格子
            for (int i = 0; i < MaxHeroCount; i++)
            {
                int x = startX + i * (maxSlotWidth + spacing);
                _slots[i].Location = new Point(x, startY);
                _slots[i].Size = new Size(maxSlotWidth, maxSlotWidth);
            }
        }

        /// <summary>
        /// 控件大小变化时重新布局
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutSlots();
        }

        /// <summary>
        /// 绑定子阵容数据
        /// </summary>
        public void BindSubLineUp(SubLineUp subLineUp)
        {
            _currentSubLineUp = subLineUp;
            RefreshBench();
        }

        /// <summary>
        /// 刷新备战席显示
        /// </summary>
        public void RefreshBench()
        {
            // 先清空所有格子
            ClearAllSlots();

            if (_currentSubLineUp == null) return;

            // 获取所有位置为(0,0)的英雄
            int slotIndex = 0;
            foreach (LineUpUnit unit in _currentSubLineUp.LineUpUnits)
            {
                if (string.IsNullOrEmpty(unit.HeroName)) continue;

                // 只显示位置为(0,0)的英雄（在备战席）
                if (unit.Position.Item1 != 0 || unit.Position.Item2 != 0) continue;

                // 备战席已满
                if (slotIndex >= MaxHeroCount) break;

                // 获取英雄数据
                Hero hero = _heroDataService?.GetHeroFromName(unit.HeroName);
                if (hero == null) continue;

                // 获取边框颜色
                Color borderColor = GetColorFromCost(hero.Cost);

                // 设置格子
                _slots[slotIndex].SetHero(unit, hero.Image, borderColor);
                slotIndex++;
            }
        }

        /// <summary>
        /// 清空所有格子
        /// </summary>
        public void ClearAllSlots()
        {
            for (int i = 0; i < MaxHeroCount; i++)
            {
                _slots[i].Clear();
            }
        }

        /// <summary>
        /// 格子开始拖拽事件处理
        /// </summary>
        private void Slot_HeroDragStarted(object sender, BenchSlotDragEventArgs e)
        {
            HeroDraggedOut?.Invoke(this, new BenchHeroDraggedOutEventArgs(e.DraggedUnit, e.SlotIndex));
        }

        /// <summary>
        /// 获取备战席当前英雄数量
        /// </summary>
        private int GetBenchHeroCount()
        {
            if (_currentSubLineUp == null) return 0;

            int count = 0;
            foreach (LineUpUnit unit in _currentSubLineUp.LineUpUnits)
            {
                if (string.IsNullOrEmpty(unit.HeroName)) continue;
                if (unit.Position.Item1 == 0 && unit.Position.Item2 == 0)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 备战席是否已满
        /// </summary>
        public bool IsFull => GetBenchHeroCount() >= MaxHeroCount;

        /// <summary>
        /// 拖放进入事件
        /// </summary>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            // 接受来自棋盘格子的拖放（备战席未满时）
            if (e.Data?.GetData(typeof(HexagonCell)) is HexagonCell sourceCell && sourceCell.HasHero && !IsFull)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// 拖放完成事件
        /// </summary>
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            // 处理来自棋盘格子的拖放
            if (e.Data?.GetData(typeof(HexagonCell)) is HexagonCell sourceCell && sourceCell.HasHero)
            {
                // 触发位置变更事件
                HeroDroppedIn?.Invoke(this, new BenchHeroDroppedInEventArgs(
                    sourceCell.Row, sourceCell.Column,
                    sourceCell.LineUpUnit
                ));
            }
        }      
    }

    /// <summary>
    /// 备战席格子控件
    /// </summary>
    public class BenchSlot : Control
    {
        private int slotIndex;// 格子索引
        private LineUpUnit _lineUpUnit;// 绑定的阵容单位
        private Image _heroImage;// 英雄图片
        private Color _borderColor = Color.FromArgb(60, 70, 80);// 边框颜色

        private bool _isHovering;// 是否悬停
        private Color _emptyColor = Color.FromArgb(45, 50, 60);// 空格子背景色
        private Color _hoverColor = Color.FromArgb(60, 70, 85);// 鼠标悬停背景色

        /// <summary>
        /// 格子索引
        /// </summary>
        public int SlotIndex => slotIndex;

        /// <summary>
        /// 绑定的阵容单位
        /// </summary>
        public LineUpUnit LineUpUnit => _lineUpUnit;

        /// <summary>
        /// 是否有英雄
        /// </summary>
        public bool HasHero => _lineUpUnit != null && !string.IsNullOrEmpty(_lineUpUnit.HeroName);

        /// <summary>
        /// 开始拖拽事件
        /// </summary>
        public event EventHandler<BenchSlotDragEventArgs> HeroDragStarted;

        public BenchSlot(int index)
        {
            slotIndex = index;

            // 启用双缓冲
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// 设置英雄
        /// </summary>
        public void SetHero(LineUpUnit lineUpUnit, Image heroImage, Color borderColor)
        {
            _lineUpUnit = lineUpUnit;
            _heroImage = heroImage;
            _borderColor = borderColor;
            Invalidate();
        }

        /// <summary>
        /// 清除英雄
        /// </summary>
        public void Clear()
        {
            _lineUpUnit = null;
            _heroImage = null;
            _borderColor = Color.FromArgb(60, 70, 80);
            Invalidate();
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // 绘制背景
            Color bgColor = _isHovering ? _hoverColor : _emptyColor;
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, ClientRectangle);
            }

            // 绘制英雄图片
            if (_heroImage != null && HasHero)
            {
                g.DrawImage(_heroImage, 2, 2, Width - 4, Height - 4);
            }

            // 绘制边框
            int borderWidth = HasHero ? 2 : 1;
            using (Pen pen = new Pen(_borderColor, borderWidth))
            {
                g.DrawRectangle(pen, borderWidth / 2, borderWidth / 2, Width - borderWidth, Height - borderWidth);
            }
        }

        /// <summary>
        /// 鼠标进入
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovering = true;
            Invalidate();
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovering = false;
            Invalidate();
        }

        /// <summary>
        /// 鼠标按下 - 开始拖拽
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && HasHero)
            {
                // 触发拖拽事件
                HeroDragStarted?.Invoke(this, new BenchSlotDragEventArgs(_lineUpUnit, slotIndex));

                // 开始拖放
                DoDragDrop(this, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// 覆盖自动缩放方法，禁用DPI自动缩放
        /// 父控件BenchPanel会在OnResize中手动计算并设置正确的尺寸
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
    /// 备战席格子拖拽事件参数
    /// </summary>
    public class BenchSlotDragEventArgs : EventArgs
    {
        public LineUpUnit DraggedUnit { get; }
        public int SlotIndex { get; }

        public BenchSlotDragEventArgs(LineUpUnit unit, int slotIndex)
        {
            DraggedUnit = unit;
            SlotIndex = slotIndex;
        }
    }

    /// <summary>
    /// 备战席英雄拖出事件参数
    /// </summary>
    public class BenchHeroDraggedOutEventArgs : EventArgs
    {
        public LineUpUnit DraggedUnit { get; }
        public int SourceSlotIndex { get; }

        public BenchHeroDraggedOutEventArgs(LineUpUnit unit, int slotIndex)
        {
            DraggedUnit = unit;
            SourceSlotIndex = slotIndex;
        }
    }

    /// <summary>
    /// 备战席英雄位置变更事件参数（从棋盘拖到备战席时使用）
    /// </summary>
    public class BenchHeroDroppedInEventArgs : EventArgs
    {
        /// <summary>
        /// 源位置行（棋盘坐标）
        /// </summary>
        public int SourceRow { get; }

        /// <summary>
        /// 源位置列（棋盘坐标）
        /// </summary>
        public int SourceColumn { get; }

        /// <summary>
        /// 从棋盘拖来的英雄
        /// </summary>
        public LineUpUnit MovedUnit { get; }
       
        public BenchHeroDroppedInEventArgs(int sourceRow, int sourceColumn, LineUpUnit movedUnit)
        {
            SourceRow = sourceRow;
            SourceColumn = sourceColumn;
            MovedUnit = movedUnit;
        }
    }
}
