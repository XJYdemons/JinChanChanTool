using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 胶囊开关自定义控件，支持点击切换开关状态
    /// </summary>
    public class CapsuleSwitch : Control
    {
        private bool isOn = false;
        private Color onColor = Color.FromArgb(76, 175, 80); // #4CAF50 绿色
        private Color offColor = Color.FromArgb(189, 189, 189); // #BDBDBD 灰色
        private Color thumbColor = Color.White; // 滑块颜色
        private Color textColor = Color.White; // 文字颜色
        private bool showText = true; // 是否显示文字标签
        private Font textFont;

        /// <summary>
        /// 开关状态（true=开启，false=关闭）
        /// </summary>
        [Category("自定义外观")]
        [Description("开关状态（true=开启，false=关闭）")]
        [DefaultValue(false)]
        public bool IsOn
        {
            get => isOn;
            set
            {
                if (isOn != value)
                {
                    isOn = value;
                    Invalidate(); // 触发重绘
                    OnIsOnChanged(EventArgs.Empty); // 触发状态变更事件
                }
            }
        }

        /// <summary>
        /// 开启状态的背景颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("开启状态的背景颜色")]
        public Color OnColor
        {
            get => onColor;
            set
            {
                if (onColor != value)
                {
                    onColor = value;
                    if (isOn) Invalidate(); // 仅当开启状态时重绘
                }
            }
        }

        /// <summary>
        /// 关闭状态的背景颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("关闭状态的背景颜色")]
        public Color OffColor
        {
            get => offColor;
            set
            {
                if (offColor != value)
                {
                    offColor = value;
                    if (!isOn) Invalidate(); // 仅当关闭状态时重绘
                }
            }
        }

        /// <summary>
        /// 滑块颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("滑块颜色")]
        public Color ThumbColor
        {
            get => thumbColor;
            set
            {
                if (thumbColor != value)
                {
                    thumbColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 文字颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("文字颜色")]
        public Color TextColor
        {
            get => textColor;
            set
            {
                if (textColor != value)
                {
                    textColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 是否显示文字标签
        /// </summary>
        [Category("自定义外观")]
        [Description("是否在滑块上显示ON/OFF文字标签")]
        [DefaultValue(true)]
        public bool ShowText
        {
            get => showText;
            set
            {
                if (showText != value)
                {
                    showText = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 状态变更事件
        /// </summary>
        [Category("自定义行为")]
        [Description("开关状态变更时触发")]
        public event EventHandler IsOnChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CapsuleSwitch()
        {
            // 启用双缓冲以减少闪烁
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            // 设置默认尺寸
            Size = new Size(50, 20);

            // 初始化字体
            textFont = new Font("Arial", 7F, FontStyle.Bold, GraphicsUnit.Point);
        }

        /// <summary>
        /// 触发状态变更事件
        /// </summary>
        /// <param name="e">事件参数</param>
        protected virtual void OnIsOnChanged(EventArgs e)
        {
            IsOnChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 鼠标点击事件 - 切换开关状态
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // 点击任意位置切换状态
            if (e.Button == MouseButtons.Left)
            {
                IsOn = !IsOn;
            }
        }

        /// <summary>
        /// 重写OnPaint方法来绘制胶囊开关
        /// </summary>
        /// <param name="e">绘制事件参数</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias; // 抗锯齿
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // 计算尺寸
            int width = Width;
            int height = Height;
            int radius = height / 2; // 圆角半径为高度的一半，形成完美胶囊形状

            // 绘制背景胶囊
            Color backgroundColor = isOn ? onColor : offColor;
            using (GraphicsPath backgroundPath = GetRoundedRectangle(0, 0, width, height, radius))
            {
                using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
                {
                    graphics.FillPath(backgroundBrush, backgroundPath);
                }
            }

            // 计算滑块位置和尺寸
            int thumbDiameter = height - 4; // 滑块直径比高度小4像素，留出边距
            int thumbRadius = thumbDiameter / 2;
            int thumbY = 2; // 滑块Y位置（居中）

            // 滑块X位置：关闭时在左侧，开启时在右侧
            int thumbX = isOn ? (width - thumbDiameter - 2) : 2;

            // 绘制滑块
            using (SolidBrush thumbBrush = new SolidBrush(thumbColor))
            {
                graphics.FillEllipse(thumbBrush, thumbX, thumbY, thumbDiameter, thumbDiameter);
            }

            // 绘制文字标签（仅当 ShowText 为 true 时）
            if (showText)
            {
                string text = isOn ? "ON" : "OFF";
                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    // 测量文字尺寸
                    SizeF textSize = graphics.MeasureString(text, textFont);

                    // 计算文字位置：显示在滑块上，居中对齐
                    float textX = thumbX + (thumbDiameter - textSize.Width) / 2;
                    float textY = thumbY + (thumbDiameter - textSize.Height) / 2;

                    graphics.DrawString(text, textFont, textBrush, textX, textY);
                }
            }
        }

        /// <summary>
        /// 创建圆角矩形路径（胶囊形状）
        /// </summary>
        /// <param name="x">起始X坐标</param>
        /// <param name="y">起始Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="radius">圆角半径</param>
        /// <returns>圆角矩形路径</returns>
        private GraphicsPath GetRoundedRectangle(int x, int y, int width, int height, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            // 确保半径不超过高度的一半
            int diameter = radius * 2;
            if (diameter > height) diameter = height;
            if (diameter > width) diameter = width;

            Rectangle arc = new Rectangle(x, y, diameter, diameter);

            // 左上角圆弧
            path.AddArc(arc, 180, 90);

            // 右上角圆弧
            arc.X = x + width - diameter;
            path.AddArc(arc, 270, 90);

            // 右下角圆弧
            arc.Y = y + height - diameter;
            path.AddArc(arc, 0, 90);

            // 左下角圆弧
            arc.X = x;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                textFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
