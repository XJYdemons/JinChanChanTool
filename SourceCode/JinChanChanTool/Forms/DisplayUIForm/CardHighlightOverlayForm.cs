using System.Drawing.Drawing2D;

namespace JinChanChanTool.Forms.DisplayUIForm
{
    /// <summary>
    /// 卡牌高亮覆盖层窗体
    /// 用于在目标卡牌位置绘制流动发光边框效果
    /// </summary>
    public partial class CardHighlightOverlayForm : Form
    {
        /// <summary>
        /// 单例模式实例
        /// </summary>
        private static CardHighlightOverlayForm _instance;

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static CardHighlightOverlayForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new CardHighlightOverlayForm();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 目标卡牌状态数组（5个卡槽）
        /// </summary>
        private bool[] targetCards = new bool[5] { false, false, false, false, false };

        /// <summary>
        /// 卡牌矩形区域数组（5个卡槽）
        /// </summary>
        private Rectangle[] cardRectangles = new Rectangle[5];

        /// <summary>
        /// 渐变动画偏移量
        /// </summary>
        private float gradientOffset = 0f;

        /// <summary>
        /// 边框宽度（像素）
        /// </summary>
        private const int BORDER_WIDTH = 3;

        /// <summary>
        /// 渐变流动速度
        /// </summary>
        private const float GRADIENT_SPEED = 0.05f;

        /// <summary>
        /// 私有构造函数（单例模式）
        /// </summary>
        private CardHighlightOverlayForm()
        {
            InitializeComponent();
            InitializeFormSettings();
        }

        /// <summary>
        /// 初始化窗体设置
        /// </summary>
        private void InitializeFormSettings()
        {
            // 设置窗体覆盖整个主屏幕
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            this.Location = new Point(screenBounds.X, screenBounds.Y);
            this.Size = new Size(screenBounds.Width, screenBounds.Height);

            // 启用双缓冲减少闪烁
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// 更新高亮状态
        /// </summary>
        /// <param name="targets">目标卡牌布尔数组</param>
        /// <param name="rectangles">卡牌矩形区域数组</param>
        public void UpdateHighlight(bool[] targets, Rectangle[] rectangles)
        {
            if (targets == null || rectangles == null)
            {
                return;
            }

            // 确保在UI线程执行
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateHighlight(targets, rectangles)));
                return;
            }

            // 检查是否需要更新
            bool hasTargets = false;
            for (int i = 0; i < targets.Length && i < 5; i++)
            {
                targetCards[i] = targets[i];
                if (targets[i])
                {
                    hasTargets = true;
                }
            }

            for (int i = 0; i < rectangles.Length && i < 5; i++)
            {
                cardRectangles[i] = rectangles[i];
            }

            // 如果有目标卡，启动动画；否则停止
            if (hasTargets)
            {
                if (!animationTimer.Enabled)
                {
                    animationTimer.Start();
                }
            }
            else
            {
                animationTimer.Stop();
            }

            // 立即重绘
            this.Invalidate();
        }

        /// <summary>
        /// 清除所有高亮
        /// </summary>
        public void ClearHighlight()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ClearHighlight()));
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                targetCards[i] = false;
            }
            animationTimer.Stop();
            this.Invalidate();
        }

        /// <summary>
        /// 显示高亮覆盖层窗体
        /// </summary>
        public void ShowOverlay()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowOverlay()));
                return;
            }

            if (!this.Visible)
            {
                this.Show();
            }
        }

        /// <summary>
        /// 隐藏高亮覆盖层窗体并清除高亮
        /// </summary>
        public void HideOverlay()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HideOverlay()));
                return;
            }

            ClearHighlight();
            if (this.Visible)
            {
                this.Hide();
            }
        }

        /// <summary>
        /// 动画定时器事件处理
        /// </summary>
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // 更新渐变偏移量实现流动效果
            gradientOffset += GRADIENT_SPEED;
            if (gradientOffset >= 1.0f)
            {
                gradientOffset = 0f;
            }

            // 触发重绘
            this.Invalidate();
        }

        /// <summary>
        /// 重写绘制方法
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 遍历5个卡槽，绘制目标卡的高亮边框
            for (int i = 0; i < 5; i++)
            {
                if (targetCards[i] && cardRectangles[i].Width > 0 && cardRectangles[i].Height > 0)
                {
                    DrawGlowingBorder(g, cardRectangles[i]);
                }
            }
        }

        /// <summary>
        /// 绘制流动发光边框
        /// </summary>
        /// <param name="g">Graphics对象</param>
        /// <param name="rect">矩形区域</param>
        private void DrawGlowingBorder(Graphics g, Rectangle rect)
        {
            // 金色渐变
            float phase = gradientOffset * 2 * (float)Math.PI;
            int r = 255;  // 红色固定高
            int gVal = Math.Clamp((int)(190 + 40 * Math.Sin(phase)), 150, 230);  // 绿色在150-230波动
            int b = Math.Clamp((int)(20 + 20 * Math.Cos(phase)), 0, 40);  // 蓝色保持低值
            Color dynamicColor1 = Color.FromArgb(r, gVal, b);

            float phase2 = phase + (float)Math.PI;
            int gVal2 = Math.Clamp((int)(190 + 40 * Math.Sin(phase2)), 150, 230);
            int b2 = Math.Clamp((int)(20 + 20 * Math.Cos(phase2)), 0, 40);
            Color dynamicColor2 = Color.FromArgb(255, gVal2, b2);

            // 主边框宽度
            int penWidth = Math.Max(BORDER_WIDTH, 1);
            // 计算偏移量（使边框完全在矩形外部）
            int offset = (penWidth + 1) / 2;

            // 绘制渐变流动边框
            try
            {
                // 创建渐变画刷用于绘制四条边
                Rectangle brushRect = new Rectangle(rect.X - 10, rect.Y - 10, rect.Width + 20, rect.Height + 20);
                if (brushRect.Width > 0 && brushRect.Height > 0)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        brushRect,
                        dynamicColor1,
                        dynamicColor2,
                        gradientOffset * 360))
                    {
                        using (Pen borderPen = new Pen(brush, penWidth))
                        {
                            borderPen.LineJoin = LineJoin.Miter;
                            // 向外偏移，使边框完全在矩形外部
                            Rectangle outerRect = new Rectangle(
                                rect.X - offset,
                                rect.Y - offset,
                                rect.Width + offset * 2,
                                rect.Height + offset * 2
                            );
                            g.DrawRectangle(borderPen, outerRect);
                        }
                    }
                }
            }
            catch
            {
                // 如果渐变创建失败，使用纯色边框
                using (Pen fallbackPen = new Pen(dynamicColor1, penWidth))
                {
                    fallbackPen.LineJoin = LineJoin.Miter;
                    Rectangle outerRect = new Rectangle(
                        rect.X - offset,
                        rect.Y - offset,
                        rect.Width + offset * 2,
                        rect.Height + offset * 2
                    );
                    g.DrawRectangle(fallbackPen, outerRect);
                }
            }
        }

        /// <summary>
        /// 使窗口点击穿透
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // WS_EX_LAYERED | WS_EX_TRANSPARENT - 使窗口可以被点击穿透
                cp.ExStyle |= 0x00080000 | 0x00000020;
                return cp;
            }
        }
    }
}
