using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace JinChanChanTool.Tools
{
    public static class ImageProcessingTool
    {
        /// <summary>
        /// 区域截图
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap AreaScreenshots(int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                // 如果传入的尺寸无效，直接返回一个1x1的空白图片，防止程序崩溃
                return new Bitmap(1, 1, PixelFormat.Format24bppRgb);
            }

            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);//创建新图像
            using (Graphics screenshot = Graphics.FromImage(image))
            {
                screenshot.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }
            
            return image;
        }

        /// <summary>
        /// 裁剪位图
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap CropBitmap(Bitmap source, int offsetX, int offsetY, int width, int height)
        {
            // 创建目标位图
            Bitmap cropped = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(cropped))
            {
                // 设置高质量绘制参数
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // 绘制裁剪区域
                g.DrawImage(source,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(offsetX, offsetY, width, height),
                    GraphicsUnit.Pixel);
            }

            return cropped;
        }

        ///// <summary>
        ///// 区域截图(32位)
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="width"></param>
        ///// <param name="height"></param>
        ///// <returns></returns>
        //public static Bitmap AreaScreenshots(int x, int y, int width, int height)
        //{
        //    Bitmap image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);//创建新图像
        //    Graphics screenshot = Graphics.FromImage(image);// 从Bitmap对象创建一个Graphics对象。Graphics对象提供方法来绘画到这个Bitmap上。


        //    screenshot.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy); // 使用Graphics对象的CopyFromScreen方法从屏幕上指定位置(x, y)开始，复制指定大小(width, height)的区域到Bitmap。

        //    screenshot.Dispose();  // 释放Graphics对象的资源。完成绘画后，应当释放此资源以避免内存泄漏。
        //    return image;
        //}


        ///// <summary>
        ///// 裁剪位图（32位）
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="offsetX"></param>
        ///// <param name="offsetY"></param>
        ///// <param name="width"></param>
        ///// <param name="height"></param>
        ///// <returns></returns>
        //public static Bitmap CropBitmap(Bitmap source, int offsetX, int offsetY, int width, int height)
        //{
        //    // 创建目标位图
        //    Bitmap cropped = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        //    using (Graphics g = Graphics.FromImage(cropped))
        //    {
        //        // 设置高质量绘制参数
        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        //        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        //        // 绘制裁剪区域
        //        g.DrawImage(source,
        //            new Rectangle(0, 0, width, height),
        //            new Rectangle(offsetX, offsetY, width, height),
        //            GraphicsUnit.Pixel);
        //    }

        //    return cropped;
        //}
    }
}
