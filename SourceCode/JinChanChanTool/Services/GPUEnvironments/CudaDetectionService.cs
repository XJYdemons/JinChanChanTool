using JinChanChanTool.DataClass.GPUEnvironments;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.GPUEnvironments
{
    /// <summary>
    /// CUDA/cuDNN检测服务
    /// 检测系统中已安装的CUDA和cuDNN版本
    /// </summary>
    internal class CudaDetectionService
    {
        /// <summary>
        /// CUDA默认安装根目录
        /// </summary>
        private const string CUDA_ROOT_PATH = @"C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA";

        /// <summary>
        /// 检测CUDA和cuDNN安装状态
        /// </summary>
        /// <returns>CUDA环境信息</returns>
        public CudaInfo DetectCudaEnvironment()
        {
            CudaInfo cudaInfo = new CudaInfo();

            // 尝试从多个来源检测CUDA
            DetectCudaFromRegistry(cudaInfo);

            // 如果注册表没找到，尝试从目录检测
            if (!cudaInfo.IsCudaInstalled)
            {
                DetectCudaFromDirectory(cudaInfo);
            }

            // 如果注册表没找到，尝试从环境变量检测
            if (!cudaInfo.IsCudaInstalled)
            {
                DetectCudaFromEnvironment(cudaInfo);
            }

            // 检测cuDNN
            if (cudaInfo.IsCudaInstalled)
            {
                DetectCudnn(cudaInfo);
            }

            return cudaInfo;
        }

        /// <summary>
        /// 从注册表检测CUDA安装
        /// </summary>
        private void DetectCudaFromRegistry(CudaInfo cudaInfo)
        {
            try
            {
                // NVIDIA CUDA注册表路径
                using RegistryKey? cudaKey = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\NVIDIA Corporation\GPU Computing Toolkit\CUDA");

                if (cudaKey != null)
                {
                    // 获取所有版本子键
                    string[] versions = cudaKey.GetSubKeyNames();

                    // 找到最高版本
                    Version? highestVersion = null;
                    string? highestVersionPath = null;

                    foreach (string version in versions)
                    {
                        using RegistryKey? versionKey = cudaKey.OpenSubKey(version);
                        string? installDir = versionKey?.GetValue("InstallDir")?.ToString();

                        if (!string.IsNullOrEmpty(installDir) && Directory.Exists(installDir))
                        {
                            // 解析版本号
                            if (Version.TryParse(version.TrimStart('v'), out Version? parsedVersion))
                            {
                                if (highestVersion == null || parsedVersion > highestVersion)
                                {
                                    highestVersion = parsedVersion;
                                    highestVersionPath = installDir;
                                }
                            }
                        }
                    }

                    if (highestVersion != null && !string.IsNullOrEmpty(highestVersionPath))
                    {
                        cudaInfo.IsCudaInstalled = true;
                        cudaInfo.CudaVersion = $"{highestVersion.Major}.{highestVersion.Minor}";
                        cudaInfo.CudaPath = highestVersionPath;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"从注册表检测CUDA失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从默认安装目录检测CUDA
        /// </summary>
        private void DetectCudaFromDirectory(CudaInfo cudaInfo)
        {
            try
            {
                if (!Directory.Exists(CUDA_ROOT_PATH))
                {
                    return;
                }

                // 获取所有版本目录
                string[] versionDirs = Directory.GetDirectories(CUDA_ROOT_PATH, "v*");

                Version? highestVersion = null;
                string? highestVersionPath = null;

                foreach (string dir in versionDirs)
                {
                    string dirName = Path.GetFileName(dir);

                    // 解析版本号（格式如 v12.9, v11.8）
                    Match match = Regex.Match(dirName, @"v(\d+\.\d+)");
                    if (match.Success)
                    {
                        string versionStr = match.Groups[1].Value;
                        if (Version.TryParse(versionStr, out Version? parsedVersion))
                        {
                            // 验证目录中有关键文件
                            string nvccPath = Path.Combine(dir, "bin", "nvcc.exe");
                            if (File.Exists(nvccPath))
                            {
                                if (highestVersion == null || parsedVersion > highestVersion)
                                {
                                    highestVersion = parsedVersion;
                                    highestVersionPath = dir;
                                }
                            }
                        }
                    }
                }

                if (highestVersion != null && !string.IsNullOrEmpty(highestVersionPath))
                {
                    cudaInfo.IsCudaInstalled = true;
                    cudaInfo.CudaVersion = $"{highestVersion.Major}.{highestVersion.Minor}";
                    cudaInfo.CudaPath = highestVersionPath;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"从目录检测CUDA失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从环境变量检测CUDA
        /// </summary>
        private void DetectCudaFromEnvironment(CudaInfo cudaInfo)
        {
            try
            {
                // 检查CUDA_PATH环境变量
                string? cudaPath = Environment.GetEnvironmentVariable("CUDA_PATH");

                if (!string.IsNullOrEmpty(cudaPath) && Directory.Exists(cudaPath))
                {
                    // 从路径中解析版本号
                    Match match = Regex.Match(cudaPath, @"v(\d+\.\d+)");
                    if (match.Success)
                    {
                        cudaInfo.IsCudaInstalled = true;
                        cudaInfo.CudaVersion = match.Groups[1].Value;
                        cudaInfo.CudaPath = cudaPath;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"从环境变量检测CUDA失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检测cuDNN安装状态
        /// </summary>
        private void DetectCudnn(CudaInfo cudaInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(cudaInfo.CudaPath))
                {
                    return;
                }

                // cuDNN的关键DLL文件
                string cudnnDllPath = Path.Combine(cudaInfo.CudaPath, "bin", "cudnn64_9.dll");

                // 如果cudnn64_9.dll不存在，尝试查找其他版本
                if (!File.Exists(cudnnDllPath))
                {
                    cudnnDllPath = Path.Combine(cudaInfo.CudaPath, "bin", "cudnn64_8.dll");
                }

                if (!File.Exists(cudnnDllPath))
                {
                    // 搜索所有cudnn*.dll文件
                    string binPath = Path.Combine(cudaInfo.CudaPath, "bin");
                    if (Directory.Exists(binPath))
                    {
                        string[] cudnnFiles = Directory.GetFiles(binPath, "cudnn64_*.dll");
                        if (cudnnFiles.Length > 0)
                        {
                            cudnnDllPath = cudnnFiles[0];
                        }
                    }
                }

                if (File.Exists(cudnnDllPath))
                {
                    cudaInfo.IsCudnnInstalled = true;

                    // 尝试从文件名解析版本
                    string fileName = Path.GetFileName(cudnnDllPath);
                    Match match = Regex.Match(fileName, @"cudnn64_(\d+)\.dll");
                    if (match.Success)
                    {
                        string majorVersion = match.Groups[1].Value;
                        cudaInfo.CudnnVersion = majorVersion switch
                        {
                            "9" => "9.x",
                            "8" => "8.x",
                            _ => majorVersion
                        };
                    }

                    // 尝试从cudnn_version.h获取更精确的版本
                    DetectCudnnVersionFromHeader(cudaInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"检测cuDNN失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从cudnn_version.h头文件获取cuDNN精确版本
        /// </summary>
        private void DetectCudnnVersionFromHeader(CudaInfo cudaInfo)
        {
            try
            {
                string headerPath = Path.Combine(cudaInfo.CudaPath, "include", "cudnn_version.h");

                if (!File.Exists(headerPath))
                {
                    return;
                }

                string content = File.ReadAllText(headerPath);

                // 解析版本号宏定义
                Match majorMatch = Regex.Match(content, @"#define\s+CUDNN_MAJOR\s+(\d+)");
                Match minorMatch = Regex.Match(content, @"#define\s+CUDNN_MINOR\s+(\d+)");
                Match patchMatch = Regex.Match(content, @"#define\s+CUDNN_PATCHLEVEL\s+(\d+)");

                if (majorMatch.Success && minorMatch.Success)
                {
                    string major = majorMatch.Groups[1].Value;
                    string minor = minorMatch.Groups[1].Value;
                    string patch = patchMatch.Success ? patchMatch.Groups[1].Value : "0";

                    cudaInfo.CudnnVersion = $"{major}.{minor}.{patch}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"从头文件读取cuDNN版本失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取指定CUDA版本的安装路径
        /// </summary>
        /// <param name="cudaVersion">CUDA版本（如"12.9"）</param>
        /// <returns>安装路径，如果不存在返回null</returns>
        public string? GetCudaPathByVersion(string cudaVersion)
        {
            string path = Path.Combine(CUDA_ROOT_PATH, $"v{cudaVersion}");
            return Directory.Exists(path) ? path : null;
        }        
    }
}
