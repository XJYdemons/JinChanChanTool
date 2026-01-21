using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.DataClass.GPUEnvironments
{
    /// <summary>
    /// GPU信息数据类
    /// 存储检测到的NVIDIA显卡信息
    /// </summary>
    internal class GpuInfo
    {
        /// <summary>
        /// 是否检测到NVIDIA显卡
        /// </summary>
        public bool IsNvidiaGpuDetected { get; set; }

        /// <summary>
        /// 显卡名称（如"NVIDIA GeForce RTX 4060"）
        /// </summary>
        public string GpuName { get; set; } = string.Empty;

        /// <summary>
        /// 显卡驱动版本
        /// </summary>
        public string DriverVersion { get; set; } = string.Empty;

        /// <summary>
        /// 驱动支持的最高CUDA版本（如"12.6"、"12.9"）
        /// 通过nvidia-smi获取，这是最准确的方式
        /// </summary>
        public string MaxSupportedCudaVersion { get; set; } = string.Empty;

        /// <summary>
        /// 检查驱动是否支持指定的CUDA版本
        /// </summary>
        public bool IsDriverSupportsCuda(string cudaTag)
        {
            string maxVersion = MaxSupportedCudaVersion;
            if (string.IsNullOrEmpty(maxVersion))
                return true; // 无法确定时，假设支持

            // 解析版本号进行比较
            Version? maxVer = ParseVersion(maxVersion);
            Version? targetVer = cudaTag switch
            {
                "cu118" => new Version(11, 8),
                "cu126" => new Version(12, 6),
                "cu129" => new Version(12, 9),
                _ => null
            };

            if (maxVer == null || targetVer == null)
                return true;

            return maxVer >= targetVer;
        }

        private static Version? ParseVersion(string version)
        {
            if (Version.TryParse(version, out Version? ver))
                return ver;
            return null;
        }


        /// <summary>
        /// 显卡系列（如"RTX 40"、"RTX 30"、"GTX 16"等）
        /// </summary>
        public GpuSeries Series { get; set; } = GpuSeries.Unknown;

        /// <summary>
        /// SM计算能力版本（如61、75、86、89、120）
        /// </summary>
        public int SmVersion { get; set; }

        /// <summary>
        /// 是否支持GPU推理
        /// </summary>
        public bool IsSupportedForInference => IsNvidiaGpuDetected && SmVersion > 0;

        /// <summary>
        /// 获取SM版本的字符串表示（如"sm61"、"sm89"）
        /// </summary>
        public string SmVersionString => SmVersion > 0 ? $"sm{SmVersion}" : "unknown";

        ///// <summary>
        ///// 检测结果的友好描述
        ///// </summary>
        //public string Description
        //{
        //    get
        //    {
        //        if (!IsNvidiaGpuDetected)
        //        {
        //            return "未检测到NVIDIA显卡";
        //        }

        //        return $"{GpuName} ({SeriesDescription}, {SmVersionString})";
        //    }
        //}

        ///// <summary>
        ///// 显卡系列的友好描述
        ///// </summary>
        //public string SeriesDescription
        //{
        //    get
        //    {
        //        return Series switch
        //        {
        //            GpuSeries.GTX10 => "GTX 10系列",
        //            GpuSeries.GTX16 => "GTX 16系列",
        //            GpuSeries.RTX20 => "RTX 20系列",
        //            GpuSeries.RTX30 => "RTX 30系列",
        //            GpuSeries.RTX40 => "RTX 40系列",
        //            GpuSeries.RTX50 => "RTX 50系列",
        //            _ => "未知系列"
        //        };
        //    }
        //}
    }

    /// <summary>
    /// NVIDIA显卡系列枚举
    /// </summary>
    public enum GpuSeries
    {
        /// <summary>
        /// 未知系列
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// GTX 10系列（如GTX 1060、GTX 1080）
        /// SM计算能力：6.1
        /// </summary>
        GTX10 = 10,

        /// <summary>
        /// GTX 16系列（如GTX 1660、GTX 1650）
        /// SM计算能力：7.5
        /// </summary>
        GTX16 = 16,

        /// <summary>
        /// RTX 20系列（如RTX 2060、RTX 2080）
        /// SM计算能力：7.5
        /// </summary>
        RTX20 = 20,

        /// <summary>
        /// RTX 30系列（如RTX 3060、RTX 3080）
        /// SM计算能力：8.6
        /// </summary>
        RTX30 = 30,

        /// <summary>
        /// RTX 40系列（如RTX 4060、RTX 4090）
        /// SM计算能力：8.9
        /// </summary>
        RTX40 = 40,

        /// <summary>
        /// RTX 50系列（如RTX 5060、RTX 5090）
        /// SM计算能力：12.0（仅CUDA 12.9支持）
        /// </summary>
        RTX50 = 50
    }
}
