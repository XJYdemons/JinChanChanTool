using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace JinChanChanTool.Services
{
    /// <summary>
    /// 基于队列的PaddleOCR文字识别服务，支持CPU和GPU设备。
    /// </summary>
    public class QueuedOCRService : IDisposable
    {
        private QueuedPaddleOcrAll _ocrQueue;      
        private CancellationTokenSource _cts;
        private int _cpuThreadCount;
        public enum 设备
        {
            CPU, GPU
        }

        /// <summary>
        /// 兼容原调用：保持无参构造器行为
        /// </summary>
        public QueuedOCRService(设备 device) : this(device, 0) { }

        /// <summary>
        /// 自动计算推荐值的线程数
        /// 可选传入 cpuThreadCount（<=0 表示自动计算推荐值）
        /// </summary>
        public QueuedOCRService(设备 device, int cpuThreadCount)
        {
            _cpuThreadCount = cpuThreadCount;
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

        /// <summary>
        /// 通过环境变量设置 Paddle/MKL/OpenBLAS/OpenMP 的线程数。threadCount <= 0 时采用自动推荐值（逻辑处理器数的一半，至少 1）。必须在创建 Paddle 设备之前调用（即在 InitializeOcrQueueCPU 的开头调用）。
        /// </summary>
        /// <param name="threadCount"></param>
        private void SetCpuThreadCount(int threadCount = 0)
        {
            int count = threadCount > 0 ? threadCount : Math.Max(1, Environment.ProcessorCount / 2);

            // 常用控制线程数的环境变量
            Environment.SetEnvironmentVariable("OMP_NUM_THREADS", count.ToString());
            Environment.SetEnvironmentVariable("MKL_NUM_THREADS", count.ToString());
            Environment.SetEnvironmentVariable("OPENBLAS_NUM_THREADS", count.ToString());

            // 额外的兼容变量（有些构建可能会检查）
            Environment.SetEnvironmentVariable("PADDLE_CPU_THREADS", count.ToString());          
        }

        /// <summary>
        /// 初始化基于CPU的OCR队列服务
        /// </summary>
        private void InitializeOcrQueueCPU()
        {
            // 先设置 CPU 线程数（如果 _cpuThreadCount <= 0，会自动计算推荐值）
            SetCpuThreadCount(_cpuThreadCount);
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

        /// <summary>
        /// 初始化基于GPU的OCR队列服务
        /// </summary>
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
