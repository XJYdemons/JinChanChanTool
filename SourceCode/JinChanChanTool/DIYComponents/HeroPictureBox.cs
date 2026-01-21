using System.Drawing.Drawing2D;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 自定义PictureBox，支持边框颜色、宽度设置、选中状态滤镜和高质量图像渲染
    /// </summary>
    public class HeroPictureBox : PictureBox
    {
        private Color _borderColor = SystemColors.Control;//默认边框颜色

        public HeroPictureBox()
        {
            BorderStyle = BorderStyle.None;
            // 设置为 Normal 模式，由 OnPaint 自己绘制高质量缩放图像
            SizeMode = PictureBoxSizeMode.Normal;
            // 启用双缓冲减少闪烁
            DoubleBuffered = true;
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
            // 设置高质量渲染模式
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            // 自己绘制图片（高质量缩放，模拟 Zoom 模式）
            if (Image != null)
            {
                Rectangle destRect = CalculateZoomRectangle(Image, ClientRectangle);
                e.Graphics.DrawImage(Image, destRect);
            }

            // 绘制边框
            if (BorderWidth > 0)
            {
                using Pen pen = new Pen(BorderColor, BorderWidth);
                // 使用浮点坐标精确绘制边框，确保四边对称且紧贴控件边缘
                // 边框中心线在距离边缘 BorderWidth/2 的位置
                float halfWidth = BorderWidth / 2.0f;
                e.Graphics.DrawRectangle(pen,
                    halfWidth,
                    halfWidth,
                    Width - BorderWidth,
                    Height - BorderWidth);
            }

            // 如果选中状态，添加红色滤镜
            if (IsSelected)
            {
                using SolidBrush overlay = new SolidBrush(SelectionColor);
                e.Graphics.FillRectangle(overlay, ClientRectangle);
            }
        }

        /// <summary>
        /// 计算保持宽高比的缩放矩形（模拟 PictureBoxSizeMode.Zoom 的行为）
        /// </summary>
        /// <param name="image">要绘制的图像</param>
        /// <param name="containerRect">容器矩形</param>
        /// <returns>缩放后的目标矩形</returns>
        private Rectangle CalculateZoomRectangle(Image image, Rectangle containerRect)
        {
            float imageAspect = (float)image.Width / image.Height;
            float containerAspect = (float)containerRect.Width / containerRect.Height;

            int destWidth, destHeight;

            if (imageAspect > containerAspect)
            {
                // 图像更宽，以容器宽度为基准
                destWidth = containerRect.Width;
                destHeight = (int)(containerRect.Width / imageAspect);
            }
            else
            {
                // 图像更高，以容器高度为基准
                destHeight = containerRect.Height;
                destWidth = (int)(containerRect.Height * imageAspect);
            }

            // 居中显示
            int destX = containerRect.X + (containerRect.Width - destWidth) / 2;
            int destY = containerRect.Y + (containerRect.Height - destHeight) / 2;

            return new Rectangle(destX, destY, destWidth, destHeight);
        }
    }
}