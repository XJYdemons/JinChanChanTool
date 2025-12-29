using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 圆角按钮自定义控件，支持鼠标悬停、按下效果和自定义外观
    /// </summary>
    public class RoundedButton : Control
    {
        // 按钮状态枚举
        private enum ButtonState
        {
            Normal,   // 正常状态
            Hovered,  // 鼠标悬停状态
            Pressed   // 鼠标按下状态
        }

        // 私有字段
        private ButtonState currentState = ButtonState.Normal;
        private int cornerRadius = 5;
        private Color buttonColor = Color.FromArgb(255, 255, 255); // 默认蓝色
        private Color hoverColor = Color.FromArgb(232, 232, 232); // 悬停时亮蓝色
        private Color pressedColor = Color.FromArgb(222, 222, 222); // 按下时深蓝色
        private Color disabledColor = Color.FromArgb(160, 160, 160); // 禁用时灰色
        private Color textColor = Color.Black;
        private Color borderColor = Color.FromArgb(200, 200, 200);
        private int borderWidth = 1;
        private Font textFont;

        /// <summary>
        /// 圆角半径（像素）
        /// </summary>
        [Category("自定义外观")]
        [Description("按钮的圆角半径（像素）")]
        [DefaultValue(10)]
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                if (cornerRadius != value && value >= 0)
                {
                    cornerRadius = value;
                    Invalidate(); // 触发重绘
                }
            }
        }

        /// <summary>
        /// 按钮文本
        /// </summary>
        [Category("自定义外观")]
        [Description("按钮显示的文本")]
        public override string Text
        {
            get => base.Text;
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    Invalidate(); // 触发重绘
                }
            }
        }

        /// <summary>
        /// 按钮背景颜色（正常状态）
        /// </summary>
        [Category("自定义外观")]
        [Description("按钮在正常状态下的背景颜色")]
        public Color ButtonColor
        {
            get => buttonColor;
            set
            {
                if (buttonColor != value)
                {
                    buttonColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 鼠标悬停时的背景颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("鼠标悬停时的背景颜色")]
        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                if (hoverColor != value)
                {
                    hoverColor = value;
                    if (currentState == ButtonState.Hovered)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标按下时的背景颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("鼠标按下时的背景颜色")]
        public Color PressedColor
        {
            get => pressedColor;
            set
            {
                if (pressedColor != value)
                {
                    pressedColor = value;
                    if (currentState == ButtonState.Pressed)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// 禁用状态的背景颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("按钮禁用时的背景颜色")]
        public Color DisabledColor
        {
            get => disabledColor;
            set
            {
                if (disabledColor != value)
                {
                    disabledColor = value;
                    if (!Enabled)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// 文本颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("按钮文本的颜色")]
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
        /// 边框颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("按钮边框的颜色")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 边框宽度（像素）
        /// </summary>
        [Category("自定义外观")]
        [Description("按钮边框的宽度（像素），0表示无边框")]
        [DefaultValue(0)]
        public int BorderWidth
        {
            get => borderWidth;
            set
            {
                if (borderWidth != value && value >= 0)
                {
                    borderWidth = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 文本字体
        /// </summary>
        [Category("自定义外观")]
        [Description("按钮文本的字体")]
        public Font TextFont
        {
            get => textFont;
            set
            {
                if (textFont != value && value != null)
                {
                    textFont?.Dispose();
                    textFont = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RoundedButton()
        {
            // 启用双缓冲以减少闪烁
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            // 设置默认尺寸
            Size = new Size(100, 35);

            // 初始化默认字体
            textFont = new Font("微软雅黑", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // 设置默认文本
            Text = "按钮";
        }

        /// <summary>
        /// 重写OnPaint方法来绘制圆角按钮
        /// </summary>
        /// <param name="e">绘制事件参数</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias; // 启用抗锯齿
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality; // 高质量像素偏移

            // 确定当前背景颜色（禁用时也保持正常背景色）
            Color backgroundColor = currentState switch
            {
                ButtonState.Pressed when Enabled => pressedColor,
                ButtonState.Hovered when Enabled => hoverColor,
                _ => buttonColor
            };

            // 计算绘制区域
            // 使用 ClientRectangle 减1确保不超出边界，再减去边框占用空间
            RectangleF drawRect;
            if (borderWidth > 0)
            {
                // 有边框时，内缩半个边框宽度
                float halfBorder = borderWidth / 2f;
                drawRect = new RectangleF(
                    halfBorder,
                    halfBorder,
                    Width - borderWidth - 1,
                    Height - borderWidth - 1
                );
            }
            else
            {
                // 无边框时，使用完整区域减1像素
                drawRect = new RectangleF(0, 0, Width - 1, Height - 1);
            }

            // 绘制按钮背景
            using (GraphicsPath backgroundPath = GetRoundedRectangle(drawRect, cornerRadius))
            {
                using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
                {
                    graphics.FillPath(backgroundBrush, backgroundPath);
                }

                // 绘制边框（如果有）
                if (borderWidth > 0 && borderColor != Color.Transparent)
                {
                    using (Pen borderPen = new Pen(borderColor, borderWidth))
                    {
                        graphics.DrawPath(borderPen, backgroundPath);
                    }
                }
            }

            // 绘制文本（居中对齐）
            if (!string.IsNullOrEmpty(Text))
            {
                // 确定文本颜色（禁用时使用灰色）
                Color currentTextColor = Enabled ? textColor : Color.FromArgb(200, 200, 200);

                using (SolidBrush textBrush = new SolidBrush(currentTextColor))
                {
                    using (StringFormat stringFormat = new StringFormat())
                    {
                        stringFormat.Alignment = StringAlignment.Center; // 水平居中
                        stringFormat.LineAlignment = StringAlignment.Center; // 垂直居中

                        Rectangle textRect = new Rectangle(0, 0, Width, Height);
                        graphics.DrawString(Text, textFont, textBrush, textRect, stringFormat);
                    }
                }
            }
        }

        /// <summary>
        /// 创建圆角矩形路径
        /// </summary>
        /// <param name="rect">矩形区域</param>
        /// <param name="radius">圆角半径</param>
        /// <returns>圆角矩形路径</returns>
        private GraphicsPath GetRoundedRectangle(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            // 限制圆角半径不超过宽度和高度的一半
            float effectiveRadius = Math.Min(radius, Math.Min(rect.Width / 2f, rect.Height / 2f));

            // 如果圆角半径为0，直接返回矩形
            if (effectiveRadius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // 计算圆角直径
            float diameter = effectiveRadius * 2f;

            // 从左上角开始，顺时针绘制四个圆角
            // 左上角圆弧（180° -> 270°）
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);

            // 右上角圆弧（270° -> 0°）
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);

            // 右下角圆弧（0° -> 90°）
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);

            // 左下角圆弧（90° -> 180°）
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);

            // 闭合路径（AddArc 会自动连接直线段）
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// 鼠标进入事件 - 切换到悬停状态
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (Enabled)
            {
                currentState = ButtonState.Hovered;
                Cursor = Cursors.Hand; // 显示手型光标
                Invalidate();
            }
        }

        /// <summary>
        /// 鼠标离开事件 - 恢复正常状态
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (Enabled)
            {
                currentState = ButtonState.Normal;
                Cursor = Cursors.Default; // 恢复默认光标
                Invalidate();
            }
        }

        /// <summary>
        /// 鼠标按下事件 - 切换到按下状态
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (Enabled && e.Button == MouseButtons.Left)
            {
                currentState = ButtonState.Pressed;
                Invalidate();
            }
        }

        /// <summary>
        /// 鼠标释放事件 - 恢复悬停状态
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (Enabled && e.Button == MouseButtons.Left)
            {
                // 判断鼠标是否仍在按钮范围内
                if (ClientRectangle.Contains(e.Location))
                {
                    currentState = ButtonState.Hovered;
                }
                else
                {
                    currentState = ButtonState.Normal;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// 启用状态变更事件 - 禁用时恢复正常状态
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (!Enabled)
            {
                currentState = ButtonState.Normal;
                Cursor = Cursors.Default;
            }

            Invalidate();
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
