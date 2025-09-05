namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 自定义PictureBox，支持边框颜色和宽度设置
    /// </summary>
    public class HeroPictureBox : PictureBox
    {
        /// <summary>
        /// 默认边框颜色
        /// </summary>
        public Color BorderColor { get; set; } = SystemColors.Control;

        /// <summary>
        /// 默认边框宽度
        /// </summary>
        public int BorderWidth { get; set; } = 3;
        
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // 绘制边框
            // 确保边框宽度至少为1
            int drawWidth = Math.Max(1, BorderWidth);
            using (Pen pen = new Pen(BorderColor, drawWidth))
            {
                // 调整矩形位置和大小，确保粗边框完整显示
                Rectangle rect = new Rectangle(
                    drawWidth / 2,          // X位置向内偏移
                    drawWidth / 2,          // Y位置向内偏移
                    Width - drawWidth ,  // 宽度减去边框厚度
                    Height - drawWidth  // 高度减去边框厚度
                );
                e.Graphics.DrawRectangle(pen, rect);
            }
        }
    }
}
