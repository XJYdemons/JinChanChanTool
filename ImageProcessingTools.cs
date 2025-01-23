using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace 金铲铲助手
{
    public static class ImageProcessingTools
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
            Bitmap image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);//创建新图像
            Graphics screenshot = Graphics.FromImage(image);// 从Bitmap对象创建一个Graphics对象。Graphics对象提供方法来绘画到这个Bitmap上。


            screenshot.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy); // 使用Graphics对象的CopyFromScreen方法从屏幕上指定位置(x, y)开始，复制指定大小(width, height)的区域到Bitmap。

            screenshot.Dispose();  // 释放Graphics对象的资源。完成绘画后，应当释放此资源以避免内存泄漏。
            return image;
        }
        
        /// <summary>
        /// 区域截图（指定显示器）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="screens"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Bitmap AreaScreenshots(int x, int y, int width, int height, Screen[] screens, int ID)
        {                       
            // 获取指定显示器的工作区域
            Rectangle screenBounds = screens[ID].Bounds;
            int offsetX = screenBounds.Left;
            int offsetY = screenBounds.Top;                      
            Bitmap image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);//创建新图像
            Graphics screenshot = Graphics.FromImage(image);// 从Bitmap对象创建一个Graphics对象。Graphics对象提供方法来绘画到这个Bitmap上。


            screenshot.CopyFromScreen(x + offsetX, y + offsetY, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy); // 使用Graphics对象的CopyFromScreen方法从屏幕上指定位置(x, y)开始，复制指定大小(width, height)的区域到Bitmap。

            screenshot.Dispose();  // 释放Graphics对象的资源。完成绘画后，应当释放此资源以避免内存泄漏。
            return image;
        }


        /// <summary>
        /// 从Bitmap转到Image<Bgr,byte>
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Image<Bgr, byte> BitmapToImageBGR(Bitmap bitmap)
        {         
            // 直接从 Bitmap 转换到 Image<Bgr, byte>
            return bitmap.ToImage<Bgr, byte>();
        }
        
        /// <summary>
        /// Image<Gray,byte>转到Bitmap
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap ImageGRAYToBitmap(Image<Gray, byte> image)
        {
            return image.ToBitmap();
        }
       
        /// <summary>
        /// Image<Bgr,byte>转到Bitmap
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap ImageBGRToBitmap(Image<Bgr, byte> image)
        {
            return image.ToBitmap();
        }
        
        /// <summary>
        /// 处理图片：放大2倍-灰度-二值化-高斯模糊
        /// </summary>
        /// <param name="baseimage"></param>
        /// <returns></returns>
        public static Image<Gray, byte> ProcessImage_Scaling2x_Grayscale_Binarization_GaussianBlur(Image<Bgr, byte> baseimage)
        {            
            
            int newWidth = baseimage.Width * 2;
            int newHeight = baseimage.Height * 2;
            Image<Bgr, byte> 缩放图 = baseimage.Resize(newWidth, newHeight, Inter.Cubic);//缩放图片为2倍
            Image<Gray, byte> 灰度图 = 缩放图.Convert<Gray, byte>();//图片转灰度图
            Image<Gray, byte> 二值化图 = 灰度图.ThresholdBinary(new Gray(128), new Gray(255));//图片转二值图
            Image<Gray, byte> 高斯模糊图 = 二值化图.SmoothGaussian(5);//高斯模糊
                                                             //Image<Gray, byte> 边缘增强图 = 高斯模糊图.Canny(100, 50);//边缘处理           
            return 高斯模糊图;
        }
        
        /// <summary>
        /// 处理图片：放大2倍-灰度-二值化-高斯模糊
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Image<Gray, byte> ProcessImage_Scaling2x_Grayscale_Binarization_GaussianBlur(Bitmap image)
        {
            Image<Bgr, byte> baseimage = BitmapToImageBGR(image);
            int newWidth = baseimage.Width * 2;
            int newHeight = baseimage.Height * 2;
            Image<Bgr, byte> 缩放图 = baseimage.Resize(newWidth, newHeight, Inter.Cubic);//缩放图片为2倍
            Image<Gray, byte> 灰度图 = 缩放图.Convert<Gray, byte>();//图片转灰度图
            Image<Gray, byte> 二值化图 = 灰度图.ThresholdBinary(new Gray(128), new Gray(255));//图片转二值图
            Image<Gray, byte> 高斯模糊图 = 二值化图.SmoothGaussian(5);//高斯模糊
                                                             //Image<Gray, byte> 边缘增强图 = 高斯模糊图.Canny(100, 50);//边缘处理           
            return 高斯模糊图;
        }
       
        /// <summary>
        /// 处理图片：放大5倍-灰度-二值化-高斯模糊
        /// </summary>
        /// <param name="baseimage"></param>
        /// <returns></returns>
        public static Image<Gray, byte> ProcessImage_Scaling5x_Grayscale_Binarization_GaussianBlur(Image<Bgr, byte> baseimage)
        {

            int newWidth = baseimage.Width * 5;
            int newHeight = baseimage.Height * 5;
            Image<Bgr, byte> 缩放图 = baseimage.Resize(newWidth, newHeight, Inter.Cubic);//缩放图片为2倍
            Image<Gray, byte> 灰度图 = 缩放图.Convert<Gray, byte>();//图片转灰度图
            Image<Gray, byte> 二值化图 = 灰度图.ThresholdBinary(new Gray(128), new Gray(255));//图片转二值图
            Image<Gray, byte> 高斯模糊图 = 二值化图.SmoothGaussian(5);//高斯模糊
                                                                      
            return 高斯模糊图;
        }
        
        /// <summary>
        /// 处理图片：放大5倍-灰度-二值化-高斯模糊
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Image<Gray, byte> ProcessImage_Scaling5x_Grayscale_Binarization_GaussianBlur(Bitmap image)
        {
            Image<Bgr, byte> baseimage = BitmapToImageBGR(image);
            int newWidth = baseimage.Width * 5;
            int newHeight = baseimage.Height * 5;
            Image<Bgr, byte> 缩放图 = baseimage.Resize(newWidth, newHeight, Inter.Cubic);//缩放图片为2倍
            Image<Gray, byte> 灰度图 = 缩放图.Convert<Gray, byte>();//图片转灰度图
            Image<Gray, byte> 二值化图 = 灰度图.ThresholdBinary(new Gray(128), new Gray(255));//图片转二值图
            Image<Gray, byte> 高斯模糊图 = 二值化图.SmoothGaussian(5);//高斯模糊
                                                                   
            return 高斯模糊图;
        }
       
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="baseimage"></param>
        /// <param name="倍数"></param>
        /// <returns></returns>
        public static Image<Bgr, byte> Scaling(Image<Bgr, byte> baseimage,int 倍数)
        { 
              
            int newWidth = baseimage.Width * 倍数;
            int newHeight = baseimage.Height * 倍数;
            Image<Bgr, byte> 缩放图 = baseimage.Resize(newWidth, newHeight, Inter.Cubic);//缩放图片为n倍

            return 缩放图;
        }
       
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="image"></param>
        /// <param name="倍数"></param>
        /// <returns></returns>
        public static Image<Bgr, byte> Scaling(Bitmap image, int 倍数)
        {
            Image<Bgr, byte> baseimage = BitmapToImageBGR(image);
            int newWidth = baseimage.Width * 倍数;
            int newHeight = baseimage.Height * 倍数;
            Image<Bgr, byte> 缩放图 = baseimage.Resize(newWidth, newHeight, Inter.Cubic);//缩放图片为n倍

            return 缩放图;
        }
        
        /// <summary>
        /// 灰度
        /// </summary>
        /// <param name="baseImage"></param>
        /// <returns></returns>
        public static Image<Gray, byte> Grayscale(Image<Bgr, byte> baseImage)
        {
            Image<Gray, byte> 灰度图=baseImage.Convert<Gray, byte>();//图片转灰度图
            return 灰度图;
        }
        
        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="baseImage"></param>
        /// <returns></returns>
        public static Image<Gray, byte> Binarization(Image<Gray, byte> baseImage)
        {
            Image<Gray, byte> 二值化图 = baseImage.ThresholdBinary(new Gray(128), new Gray(255));//图片转二值图
            return 二值化图;
        }
        
        /// <summary>
        /// 高斯模糊
        /// </summary>
        /// <param name="baseImage"></param>
        /// <param name="模糊指数"></param>
        /// <returns></returns>
        public static Image<Gray, byte> GaussianBlur(Image<Gray, byte> baseImage,int 模糊指数)
        {
            Image<Gray, byte> 高斯模糊图 = baseImage.SmoothGaussian(模糊指数);//高斯模糊
            return 高斯模糊图;
        }
    }
}
