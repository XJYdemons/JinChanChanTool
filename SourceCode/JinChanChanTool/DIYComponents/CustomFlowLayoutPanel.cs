using System.ComponentModel;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 自定义FlowLayoutPanel，支持设置边框颜色和宽度
    /// </summary>
    public class CustomFlowLayoutPanel : FlowLayoutPanel
    {
        private Color _borderColor = Color.Gray;
        private int _borderWidth = 1;

        /// <summary>
        /// 边框颜色
        /// </summary>
        [Category("自定义外观")]
        [Description("边框的颜色")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    Invalidate(); // 触发重绘
                }
            }
        }

        /// <summary>
        /// 边框宽度（像素）
        /// </summary>
        [Category("自定义外观")]
        [Description("边框的宽度（像素）")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                if (_borderWidth != value && value >= 0)
                {
                    _borderWidth = value;
                    Invalidate(); // 触发重绘
                }
            }
        }

        public CustomFlowLayoutPanel()
        {
            // 启用双缓冲以减少闪烁
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();
        }

        /// <summary>
        /// 重写OnPaint方法来绘制自定义边框
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 如果边框宽度为0，不绘制边框
            if (_borderWidth <= 0)
                return;

            using (Pen pen = new Pen(_borderColor, _borderWidth))
            {
                // 计算边框绘制的矩形区域
                // 需要根据边框宽度调整，确保边框完全在控件范围内
                float halfWidth = _borderWidth / 2f;
                Rectangle rect = new Rectangle(
                    (int)halfWidth,
                    (int)halfWidth,
                    Width - _borderWidth,
                    Height - _borderWidth
                );

                // 绘制边框
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        /// <summary>
        /// 重写OnPaintBackground以确保背景正确绘制
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
    }
}
