using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.DataClass.GPUEnvironments
{
    /// <summary>
    /// CUDA环境信息数据类
    /// 存储检测到的CUDA和cuDNN安装状态
    /// </summary>
    internal class CudaInfo
    {
        /// <summary>
        /// 是否已安装CUDA
        /// </summary>
        public bool IsCudaInstalled { get; set; }

        /// <summary>
        /// CUDA版本（如"12.9"、"12.6"、"11.8"）
        /// </summary>
        public string CudaVersion { get; set; } = string.Empty;

        /// <summary>
        /// CUDA安装路径（如"C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v12.9"）
        /// </summary>
        public string CudaPath { get; set; } = string.Empty;

        /// <summary>
        /// 是否已安装cuDNN
        /// </summary>
        public bool IsCudnnInstalled { get; set; }

        /// <summary>
        /// cuDNN版本（如"9.10"、"9.5"、"8.9"）
        /// </summary>
        public string CudnnVersion { get; set; } = string.Empty;

        /// <summary>
        /// CUDA主版本号（如12、11）
        /// </summary>
        public int CudaMajorVersion
        {
            get
            {
                if (string.IsNullOrEmpty(CudaVersion))
                {
                    return 0;
                }

                string[] parts = CudaVersion.Split('.');
                if (parts.Length > 0 && int.TryParse(parts[0], out int major))
                {
                    return major;
                }

                return 0;
            }
        }

        /// <summary>
        /// CUDA次版本号（如9、6、8）
        /// </summary>
        public int CudaMinorVersion
        {
            get
            {
                if (string.IsNullOrEmpty(CudaVersion))
                {
                    return 0;
                }

                string[] parts = CudaVersion.Split('.');
                if (parts.Length > 1 && int.TryParse(parts[1], out int minor))
                {
                    return minor;
                }

                return 0;
            }
        }

        /// <summary>
        /// 是否为支持的CUDA版本（11.8、12.6、12.9）
        /// </summary>
        public bool IsSupportedCudaVersion
        {
            get
            {
                if (!IsCudaInstalled)
                {
                    return false;
                }

                // 支持的版本：11.8, 12.6, 12.9
                return (CudaMajorVersion == 11 && CudaMinorVersion >= 8) ||
                       (CudaMajorVersion == 12 && (CudaMinorVersion == 6 || CudaMinorVersion >= 9));
            }
        }

        /// <summary>
        /// 获取推荐的CUDA版本标识（用于匹配运行时包）
        /// 如"cu118"、"cu126"、"cu129"
        /// </summary>
        public string RecommendedCudaTag
        {
            get
            {
                if (!IsCudaInstalled)
                {
                    return string.Empty;
                }

                // 根据已安装的CUDA版本返回对应标识
                if (CudaMajorVersion == 11 && CudaMinorVersion >= 8)
                {
                    return "cu118";
                }
                else if (CudaMajorVersion == 12)
                {
                    if (CudaMinorVersion >= 9)
                    {
                        return "cu129";
                    }
                    else if (CudaMinorVersion >= 6)
                    {
                        return "cu126";
                    }
                }

                return string.Empty;
            }
        }
    }
}
