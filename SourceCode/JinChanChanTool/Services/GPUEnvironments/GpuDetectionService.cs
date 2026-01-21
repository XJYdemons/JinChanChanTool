using JinChanChanTool.DataClass.GPUEnvironments;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.GPUEnvironments
{
    /// <summary>
    /// GPU检测服务
    /// 通过WMI查询NVIDIA显卡信息，并映射到SM计算能力版本
    /// </summary>
    internal class GpuDetectionService
    {
        /// <summary>
        /// 检测系统中的NVIDIA显卡
        /// </summary>
        /// <returns>GPU信息</returns>
        public GpuInfo DetectGpu()
        {
            GpuInfo gpuInfo = new GpuInfo();

            try
            {
                // 使用WMI查询显卡信息
                using ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_VideoController");

                foreach (ManagementObject obj in searcher.Get())
                {
                    string? name = obj["Name"]?.ToString();
                    string? driverVersion = obj["DriverVersion"]?.ToString();

                    // 检查是否为NVIDIA显卡
                    if (!string.IsNullOrEmpty(name) && name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase))
                    {
                        gpuInfo.IsNvidiaGpuDetected = true;
                        gpuInfo.GpuName = name;
                        gpuInfo.DriverVersion = driverVersion ?? string.Empty;

                        // 解析显卡系列和SM版本
                        ParseGpuSeries(gpuInfo);

                        // 通过nvidia-smi获取驱动支持的最高CUDA版本
                        gpuInfo.MaxSupportedCudaVersion = DetectMaxCudaVersionFromNvidiaSmi();

                        // 找到第一个NVIDIA显卡就返回
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // WMI查询失败，记录错误但不抛出异常
                System.Diagnostics.Debug.WriteLine($"GPU检测失败: {ex.Message}");
            }

            return gpuInfo;
        }

        /// <summary>
        /// 通过nvidia-smi获取驱动支持的最高CUDA版本
        /// </summary>
        /// <returns>CUDA版本字符串，如"12.6"</returns>
        private string DetectMaxCudaVersionFromNvidiaSmi()
        {
            try
            {
                // nvidia-smi通常在系统PATH中，或者在固定位置
                string nvidiaSmiPath = FindNvidiaSmiPath();

                if (string.IsNullOrEmpty(nvidiaSmiPath))
                {
                    return string.Empty;
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = nvidiaSmiPath,
                    Arguments = "--query-gpu=driver_version --format=csv,noheader,nounits",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // 先尝试获取完整输出来解析CUDA版本
                psi.Arguments = "";  // 无参数运行nvidia-smi获取完整信息

                using Process? process = Process.Start(psi);
                if (process == null)
                {
                    return string.Empty;
                }

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit(5000);

                if (process.ExitCode != 0 || string.IsNullOrEmpty(output))
                {
                    return string.Empty;
                }

                // 解析nvidia-smi输出，查找CUDA Version
                // 输出格式类似: "CUDA Version: 12.6"
                Match match = Regex.Match(output, @"CUDA Version:\s*(\d+\.\d+)");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"nvidia-smi检测失败: {ex.Message}");
            }

            return string.Empty;
        }

        /// <summary>
        /// 查找nvidia-smi可执行文件路径
        /// </summary>
        private string FindNvidiaSmiPath()
        {
            // 常见的nvidia-smi路径
            string[] possiblePaths = new[]
            {
                "nvidia-smi",  // 系统PATH中
                @"C:\Windows\System32\nvidia-smi.exe",
                @"C:\Program Files\NVIDIA Corporation\NVSMI\nvidia-smi.exe"
            };

            foreach (string path in possiblePaths)
            {
                try
                {
                    // 检查是否可以执行
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using Process? process = Process.Start(psi);
                    if (process != null)
                    {
                        process.WaitForExit(3000);
                        if (process.ExitCode == 0)
                        {
                            return path;
                        }
                    }
                }
                catch
                {
                    // 继续尝试下一个路径
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 解析GPU名称，确定显卡系列和SM版本
        /// </summary>
        /// <param name="gpuInfo">GPU信息对象</param>
        private void ParseGpuSeries(GpuInfo gpuInfo)
        {
            string name = gpuInfo.GpuName.ToUpperInvariant();

            // RTX 50系列 (SM 12.0)
            if (IsRtx50Series(name))
            {
                gpuInfo.Series = GpuSeries.RTX50;
                gpuInfo.SmVersion = 120;
                return;
            }

            // RTX 40系列 (SM 8.9)
            if (IsRtx40Series(name))
            {
                gpuInfo.Series = GpuSeries.RTX40;
                gpuInfo.SmVersion = 89;
                return;
            }

            // RTX 30系列 (SM 8.6)
            if (IsRtx30Series(name))
            {
                gpuInfo.Series = GpuSeries.RTX30;
                gpuInfo.SmVersion = 86;
                return;
            }

            // RTX 20系列 (SM 7.5)
            if (IsRtx20Series(name))
            {
                gpuInfo.Series = GpuSeries.RTX20;
                gpuInfo.SmVersion = 75;
                return;
            }

            // GTX 16系列 (SM 7.5)
            if (IsGtx16Series(name))
            {
                gpuInfo.Series = GpuSeries.GTX16;
                gpuInfo.SmVersion = 75;
                return;
            }

            // GTX 10系列 (SM 6.1)
            if (IsGtx10Series(name))
            {
                gpuInfo.Series = GpuSeries.GTX10;
                gpuInfo.SmVersion = 61;
                return;
            }

            // 未知系列
            gpuInfo.Series = GpuSeries.Unknown;
            gpuInfo.SmVersion = 0;
        }

        /// <summary>
        /// 检查是否为RTX 50系列
        /// </summary>
        private bool IsRtx50Series(string name)
        {
            // RTX 5090, RTX 5080, RTX 5070, RTX 5060 等
            return Regex.IsMatch(name, @"RTX\s*50[0-9]{2}");
        }

        /// <summary>
        /// 检查是否为RTX 40系列
        /// </summary>
        private bool IsRtx40Series(string name)
        {
            // RTX 4090, RTX 4080, RTX 4070, RTX 4060 等
            return Regex.IsMatch(name, @"RTX\s*40[0-9]{2}");
        }

        /// <summary>
        /// 检查是否为RTX 30系列
        /// </summary>
        private bool IsRtx30Series(string name)
        {
            // RTX 3090, RTX 3080, RTX 3070, RTX 3060, RTX 3050 等
            return Regex.IsMatch(name, @"RTX\s*30[0-9]{2}");
        }

        /// <summary>
        /// 检查是否为RTX 20系列
        /// </summary>
        private bool IsRtx20Series(string name)
        {
            // RTX 2080, RTX 2070, RTX 2060 等
            return Regex.IsMatch(name, @"RTX\s*20[0-9]{2}");
        }

        /// <summary>
        /// 检查是否为GTX 16系列
        /// </summary>
        private bool IsGtx16Series(string name)
        {
            // GTX 1660, GTX 1650 等
            return Regex.IsMatch(name, @"GTX\s*16[0-9]{2}");
        }

        /// <summary>
        /// 检查是否为GTX 10系列
        /// </summary>
        private bool IsGtx10Series(string name)
        {
            // GTX 1080, GTX 1070, GTX 1060, GTX 1050 等
            return Regex.IsMatch(name, @"GTX\s*10[0-9]{2}");
        }
    }
}
