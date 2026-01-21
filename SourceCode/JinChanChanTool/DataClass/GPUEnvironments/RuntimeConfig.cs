using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.DataClass.GPUEnvironments
{
    /// <summary>
    /// 运行时配置数据类
    /// 定义CUDA版本、cuDNN版本和SM版本的映射关系
    /// </summary>
    internal class RuntimeConfig
    {
        /// <summary>
        /// CUDA版本标识（如"cu118"、"cu126"、"cu129"）
        /// </summary>
        public string CudaTag { get; set; } = string.Empty;

        /// <summary>
        /// cuDNN版本标识（如"cudnn89"、"cudnn95"、"cudnn910"）
        /// </summary>
        public string CudnnTag { get; set; } = string.Empty;

        /// <summary>
        /// SM版本标识（如"sm61"、"sm75"、"sm86"、"sm89"、"sm120"）
        /// </summary>
        public string SmTag { get; set; } = string.Empty;

        /// <summary>
        /// 运行时包名称（如"cu129_cudnn910_sm89"）
        /// </summary>
        public string RuntimePackageName => $"{CudaTag}_{CudnnTag}_{SmTag}";

        /// <summary>
        /// 运行时ZIP文件名（如"cu129_cudnn910_sm89.zip"）
        /// </summary>
        public string RuntimeZipFileName => $"{RuntimePackageName}.zip";

        ///// <summary>
        ///// CUDA安装包文件名
        ///// </summary>
        //public string CudaInstallerFileName
        //{
        //    get
        //    {
        //        return CudaTag switch
        //        {
        //            "cu118" => "cuda_11.8.0_522.06_windows.exe",
        //            "cu126" => "cuda_12.6.0_560.76_windows.exe",
        //            "cu129" => "cuda_12.9.0_576.02_windows.exe",
        //            _ => string.Empty
        //        };
        //    }
        //}

        ///// <summary>
        ///// cuDNN压缩包文件名
        ///// </summary>
        //public string CudnnZipFileName
        //{
        //    get
        //    {
        //        return CudnnTag switch
        //        {
        //            "cudnn89" => "cudnn-windows-x86_64-8.9.7.29_cuda11-archive.zip",
        //            "cudnn95" => "cudnn-windows-x86_64-9.5.0.50_cuda12-archive.zip",
        //            "cudnn910" => "cudnn-windows-x86_64-9.10.0.56_cuda12-archive.zip",
        //            _ => string.Empty
        //        };
        //    }
        //}

        /// <summary>
        /// CUDA版本的友好显示名称
        /// </summary>
        public string CudaDisplayName
        {
            get
            {
                return CudaTag switch
                {
                    "cu118" => "CUDA 11.8",
                    "cu126" => "CUDA 12.6",
                    "cu129" => "CUDA 12.9",
                    _ => "未知CUDA版本"
                };
            }
        }

        /// <summary>
        /// cuDNN版本的友好显示名称
        /// </summary>
        public string CudnnDisplayName
        {
            get
            {
                return CudnnTag switch
                {
                    "cudnn89" => "cuDNN 8.9.7",
                    "cudnn95" => "cuDNN 9.5.0",
                    "cudnn910" => "cuDNN 9.10.0",
                    _ => "未知cuDNN版本"
                };
            }
        }

        ///// <summary>
        ///// 支持的GPU系列描述
        ///// </summary>
        //public string SupportedGpuDescription
        //{
        //    get
        //    {
        //        return SmTag switch
        //        {
        //            "sm61" => "GTX 10系列",
        //            "sm75" => "GTX 16/RTX 20系列",
        //            "sm86" => "RTX 30系列",
        //            "sm89" => "RTX 40系列",
        //            "sm120" => "RTX 50系列",
        //            _ => "未知GPU系列"
        //        };
        //    }
        //}

        /// <summary>
        /// 根据GPU信息和CUDA版本选择生成运行时配置
        /// </summary>
        /// <param name="gpuInfo">GPU信息</param>
        /// <param name="cudaTag">CUDA版本标识（如为空则使用推荐版本）</param>
        /// <returns>运行时配置</returns>
        public static RuntimeConfig CreateFromGpuInfo(GpuInfo gpuInfo, string cudaTag = "cu129")
        {
            RuntimeConfig config = new RuntimeConfig
            {
                CudaTag = cudaTag,
                SmTag = gpuInfo.SmVersionString
            };

            // 根据CUDA版本设置cuDNN版本
            config.CudnnTag = cudaTag switch
            {
                "cu118" => "cudnn89",
                "cu126" => "cudnn95",
                "cu129" => "cudnn910",
                _ => "cudnn910"
            };

            // RTX 50系列只支持CUDA 12.9
            if (gpuInfo.Series == GpuSeries.RTX50 && cudaTag != "cu129")
            {
                config.CudaTag = "cu129";
                config.CudnnTag = "cudnn910";
            }

            return config;
        }

        /// <summary>
        /// 获取所有支持的CUDA版本选项
        /// </summary>
        /// <param name="gpuInfo">GPU信息</param>
        /// <returns>支持的CUDA版本列表</returns>
        public static List<CudaVersionOption> GetSupportedCudaVersions(GpuInfo gpuInfo)
        {
            List<CudaVersionOption> options = new List<CudaVersionOption>();
            string maxCuda = gpuInfo.MaxSupportedCudaVersion;

            // RTX 50系列只支持CUDA 12.9
            if (gpuInfo.Series == GpuSeries.RTX50)
            {
                if (gpuInfo.IsDriverSupportsCuda("cu129"))
                {
                    options.Add(new CudaVersionOption("cu129", "CUDA 12.9（推荐）", true));
                }
                else
                {
                    // 驱动不支持，提示需要更新
                    string hint = !string.IsNullOrEmpty(maxCuda) ? $"，当前最高支持{maxCuda}" : "";
                    options.Add(new CudaVersionOption("cu129",
                        $"CUDA 12.9（需要更新驱动{hint}）", true, false));
                }
                return options;
            }

            // 根据驱动支持情况添加版本选项
            bool hasRecommended = false;

            // CUDA 12.9
            if (gpuInfo.IsDriverSupportsCuda("cu129"))
            {
                options.Add(new CudaVersionOption("cu129", "CUDA 12.9（推荐，最新版）", true));
                hasRecommended = true;
            }
            else
            {
                options.Add(new CudaVersionOption("cu129",
                    $"CUDA 12.9（驱动不支持，需≥576）", false, false));
            }

            // CUDA 12.6
            if (gpuInfo.IsDriverSupportsCuda("cu126"))
            {
                string label = hasRecommended ? "CUDA 12.6" : "CUDA 12.6（推荐）";
                options.Add(new CudaVersionOption("cu126", label, !hasRecommended));
                if (!hasRecommended) hasRecommended = true;
            }
            else
            {
                options.Add(new CudaVersionOption("cu126",
                    $"CUDA 12.6（驱动不支持，需≥560）", false, false));
            }

            // CUDA 11.8
            if (gpuInfo.IsDriverSupportsCuda("cu118"))
            {
                string label = hasRecommended ? "CUDA 11.8" : "CUDA 11.8（推荐）";
                options.Add(new CudaVersionOption("cu118", label, !hasRecommended));
            }
            else
            {
                options.Add(new CudaVersionOption("cu118",
                    $"CUDA 11.8（驱动不支持，需≥520）", false, false));
            }

            return options;
        }
    }
    /// <summary>
    /// CUDA版本选项
    /// </summary>
    public class CudaVersionOption
    {
        /// <summary>
        /// CUDA版本标识
        /// </summary>
        public string CudaTag { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否为推荐版本
        /// </summary>
        public bool IsRecommended { get; set; }

        /// <summary>
        /// 驱动是否支持此版本
        /// </summary>
        public bool IsSupported { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CudaVersionOption(string cudaTag, string displayName, bool isRecommended, bool isSupported = true)
        {
            CudaTag = cudaTag;
            DisplayName = displayName;
            IsRecommended = isRecommended;
            IsSupported = isSupported;
        }

        /// <summary>
        /// ToString重写，用于ComboBox显示
        /// </summary>
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
