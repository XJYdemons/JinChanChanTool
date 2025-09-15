using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace JinChanChanTool.Services
{
    public class QueuedOCRService : IDisposable
    {
        private QueuedPaddleOcrAll _ocrQueue;      
        private CancellationTokenSource _cts;                     
        public enum 设备
        {
            CPU, GPU
        }

        /// <summary>
        /// 初始化OCR队列服务
        /// </summary>
        public QueuedOCRService(设备 device)
        {
            switch (device)
            {
                case 设备.CPU:
                    InitializeOcrQueueCPU();
                    break;
                case 设备.GPU:
                    InitializeOcrQueueGPU();
                    break;
                default:
                    InitializeOcrQueueCPU();
                    break;
            }
        }

        private void InitializeOcrQueueCPU()
        {
            // 创建OCR工厂方法
            Func<PaddleOcrAll> factory = () =>
            {
                DetectionModel de = new FileDetectionModel(
                    Path.Combine(Application.StartupPath, "Resources\\Models\\PP-OCRv5_mobile_det_infer"),
                    ModelVersion.V5);

                RecognizationModel re = new FileRecognizationModel(
                    Path.Combine(Application.StartupPath, "Resources\\Models\\PP-OCRv5_mobile_rec_infer"),
                    Path.Combine(Application.StartupPath, "Resources\\Models", "ppocr_keys_v5.txt"),
                    ModelVersion.V5);
                
                // 创建完整OCR实例
                return new PaddleOcrAll(
                    new FullOcrModel(de, re),
                    PaddleDevice.Mkldnn());
            };

            // 创建队列服务
            _ocrQueue = new QueuedPaddleOcrAll(
                factory: factory,
                consumerCount: 1,     // 工作线程
                boundedCapacity: 64); // 队列容量

            _cts = new CancellationTokenSource();
          
        }
        private void InitializeOcrQueueGPU()
        {
            // 创建OCR工厂方法
            Func<PaddleOcrAll> factory = () =>
            {
                DetectionModel de = new FileDetectionModel(
                    Path.Combine(Application.StartupPath, "Resources\\Models\\PP-OCRv5_mobile_det_infer"),
                    ModelVersion.V5);

                RecognizationModel re = new FileRecognizationModel(
                    Path.Combine(Application.StartupPath, "Resources\\Models\\PP-OCRv5_mobile_rec_infer"),
                    Path.Combine(Application.StartupPath, "Resources\\Models", "ppocr_keys_v5.txt"),
                    ModelVersion.V5);

                // 创建完整OCR实例
                return new PaddleOcrAll(
                    new FullOcrModel(de, re),
                    PaddleDevice.Gpu());
            };

            // 创建队列服务
            _ocrQueue = new QueuedPaddleOcrAll(
                factory: factory,
                consumerCount: 1,     // 工作线程
                boundedCapacity: 64); // 队列容量

            _cts = new CancellationTokenSource();
            
        }

        /// <summary>
        /// 识别图像中的文字（异步）
        /// </summary>
        /// <param name="bitmap">要识别的图像</param>
        /// <param name="recognizeBatchSize">批量识别大小</param>
        /// <param name="configure">OCR配置操作</param>
        /// <returns>识别结果</returns>
        public async Task<string> RecognizeTextAsync(Bitmap bitmap, int recognizeBatchSize = 0, Action<PaddleOcrAll> configure = null)
        {                                   
                // 转换图像格式
                using Mat src = BitmapToMat(bitmap);
                // 提交OCR请求
                PaddleOcrResult result = await _ocrQueue.Run(
                    src: src,
                    recognizeBatchSize: recognizeBatchSize,
                    configure: configure,
                    cancellationToken: _cts.Token);               
                return result.Text;                      
        }

        /// <summary>
        /// 将Bitmap转换为OpenCV Mat对象
        /// </summary>
        private Mat BitmapToMat(Bitmap bitmap)
        {
            // 确保Bitmap格式为24bppRgb（3通道）
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
            {
                using Bitmap converted = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(converted))
                {
                    g.DrawImage(bitmap, 0, 0);
                }
                return BitmapToMat(converted);
            }

            // 锁定Bitmap数据
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                // 创建3通道Mat对象
                Mat mat = Mat.FromPixelData(
                    rows: bitmap.Height,
                    cols: bitmap.Width,
                    type: MatType.CV_8UC3, // 8位无符号4通道
                    data: bmpData.Scan0,
                    step: bmpData.Stride
                );

                return mat.Clone();
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }
        }

        ///// <summary>
        ///// 将Bitmap转换为OpenCV Mat对象（32位）
        ///// </summary>
        //private Mat BitmapToMat(Bitmap bitmap)
        //{
        //    // 确保Bitmap格式为32bppArgb（OpenCV兼容格式）
        //    if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
        //    {
        //        using Bitmap converted = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
        //        using (Graphics g = Graphics.FromImage(converted))
        //        {
        //            g.DrawImage(bitmap, 0, 0);
        //        }
        //        return BitmapToMat(converted);
        //    }

        //    // 锁定Bitmap数据
        //    BitmapData bmpData = bitmap.LockBits(
        //        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //        ImageLockMode.ReadOnly,
        //        bitmap.PixelFormat);

        //    try
        //    {
        //        // 创建Mat对象
        //        Mat mat = Mat.FromPixelData(
        //            rows: bitmap.Height,
        //            cols: bitmap.Width,
        //            type: MatType.CV_8UC4, // 8位无符号4通道
        //            data: bmpData.Scan0,
        //            step: bmpData.Stride
        //        );

        //        // 转换为BGR格式（OpenCV默认格式）
        //        return mat.CvtColor(ColorConversionCodes.BGRA2BGR);
        //    }
        //    finally
        //    {
        //        bitmap.UnlockBits(bmpData);
        //    }
        //}

        /// <summary>
        /// 取消所有待处理的OCR请求
        /// </summary>
        public void CancelAll()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _ocrQueue?.Dispose();
            _cts?.Dispose();
        }
    }
}
