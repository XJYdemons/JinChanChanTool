namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 自定义PictureBox，支持边框颜色、宽度设置和选中状态滤镜
    /// </summary>
    public class HeroPictureBox : PictureBox
    {
        private Color _borderColor = SystemColors.Control;//默认边框颜色

        public HeroPictureBox()
        {
            BorderStyle = BorderStyle.None;
            SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// 边框颜色（自动重绘）
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    Invalidate();  // 触发重绘
                }
            }
        }

        private int _borderWidth = 3;//默认边框宽度

        /// <summary>
        /// 边框宽度（自动重绘）
        /// </summary>
        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                value = Math.Max(0, value);
                if (_borderWidth != value)
                {
                    _borderWidth = value;
                    Invalidate();
                }
            }
        }

        private bool _isSelected = false;//默认不选中 

        /// <summary>
        /// 是否选中（自动重绘）
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    Invalidate();  // 触发重绘
                }
            }
        }

        private Color _selectionColor = Color.FromArgb(125, 255, 0, 0); // 半透明红色滤镜

        /// <summary>
        /// 选中状态滤镜颜色（默认半透明红色）
        /// </summary>
        public Color SelectionColor
        {
            get => _selectionColor;
            set
            {
                if (_selectionColor != value)
                {
                    _selectionColor = value;
                    if (_isSelected) Invalidate(); // 仅当选中时重绘
                }
            }
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // 先绘制基础图片
            base.OnPaint(e);

            // 绘制边框
            if (BorderWidth > 0)
            {
                using var pen = new Pen(BorderColor, BorderWidth);
                Rectangle rect = new Rectangle(
                    BorderWidth / 2,
                    BorderWidth / 2,
                    Width - BorderWidth,
                    Height - BorderWidth
                );
                e.Graphics.DrawRectangle(pen, rect);
            }
          
            // 如果选中状态，添加红色滤镜
            if (IsSelected)
            {
                using var overlay = new SolidBrush(SelectionColor);
                e.Graphics.FillRectangle(overlay, ClientRectangle);
            }
        }
    }
}