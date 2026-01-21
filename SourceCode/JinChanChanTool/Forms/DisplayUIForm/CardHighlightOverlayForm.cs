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
        /// 渐变流动速度（默认值）
        /// </summary>
        private const float GRADIENT_SPEED = 0.05f;

        /// <summary>
        /// 自定义高亮颜色1（渐变起始颜色）
        /// </summary>
        private Color customColor1 = Color.FromArgb(255, 190, 20);

        /// <summary>
        /// 自���义高亮颜色2（渐变结束颜色）
        /// </summary>
        private Color customColor2 = Color.FromArgb(255, 190, 20);

        /// <summary>
        /// 自定义边框宽度
        /// </summary>
        private int customBorderWidth = BORDER_WIDTH;

        /// <summary>
        /// 自定义渐变流动速度
        /// </summary>
        private float customGradientSpeed = GRADIENT_SPEED;

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
        /// 更新高亮颜色设置
        /// </summary>
        /// <param name="color1">渐变起始颜色</param>
        /// <param name="color2">渐变结束颜色</param>
        /// <param name="borderWidth">边框宽度</param>
        /// <param name="gradientSpeed">渐变流动速度</param>
        public void UpdateColorSettings(Color color1, Color color2, int borderWidth, float gradientSpeed)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateColorSettings(color1, color2, borderWidth, gradientSpeed)));
                return;
            }

            customColor1 = color1;
            customColor2 = color2;
            customBorderWidth = Math.Max(borderWidth, 1);
            customGradientSpeed = Math.Clamp(gradientSpeed, 0.01f, 0.2f);

            // 如果当前有高亮显示，立即重绘以应用新颜色
            if (this.Visible)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// 动画定时器事件处理
        /// </summary>
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // 更新渐变偏移量实现流动效果，使用自定义速度
            gradientOffset += customGradientSpeed;
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
            // 对于矩形边框，不使用抗锯齿以避免偶数宽度时出现品红色描边
            g.SmoothingMode = SmoothingMode.None;
            // 使用 Half 像素偏移模式确保边框在整数像素位置上
            g.PixelOffsetMode = PixelOffsetMode.Half;

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
            // 使用自定义颜色创建动态渐变效果
            float phase = gradientOffset * 2 * (float)Math.PI;

            // 从自定义颜色1提取RGB分量并应用动态波动
            int r1 = customColor1.R;
            int g1 = customColor1.G;
            int b1 = customColor1.B;

            // 应用正弦波动（±10%范围）
            int dynamicR1 = Math.Clamp((int)(r1 + r1 * 0.1 * Math.Sin(phase)), 0, 255);
            int dynamicG1 = Math.Clamp((int)(g1 + g1 * 0.1 * Math.Sin(phase)), 0, 255);
            int dynamicB1 = Math.Clamp((int)(b1 + b1 * 0.1 * Math.Cos(phase)), 0, 255);
            Color dynamicColor1 = Color.FromArgb(dynamicR1, dynamicG1, dynamicB1);

            // 从自定义颜色2提取RGB分量并应用动态波动（相位相反）
            float phase2 = phase + (float)Math.PI;
            int r2 = customColor2.R;
            int g2 = customColor2.G;
            int b2 = customColor2.B;

            int dynamicR2 = Math.Clamp((int)(r2 + r2 * 0.1 * Math.Sin(phase2)), 0, 255);
            int dynamicG2 = Math.Clamp((int)(g2 + g2 * 0.1 * Math.Sin(phase2)), 0, 255);
            int dynamicB2 = Math.Clamp((int)(b2 + b2 * 0.1 * Math.Cos(phase2)), 0, 255);
            Color dynamicColor2 = Color.FromArgb(dynamicR2, dynamicG2, dynamicB2);

            // 使用自定义边框宽度
            int penWidth = Math.Max(customBorderWidth, 1);
            // 计算偏移量（使边框完全在矩形外部）
            // DrawRectangle 的边框以矩形边界为中心线，向内外各扩展 penWidth/2
            // 要让边框的内侧正好在原矩形边界上，offset = penWidth/2（向上取整）
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
