using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.GPUEnvironments
{
    /// <summary>
    /// CUDA静默安装服务
    /// 负责下载并静默安装CUDA Toolkit
    /// </summary>
    internal class CudaInstallerService
    {
        /// <summary>
        /// CUDA默认安装根目录
        /// </summary>
        private const string CUDA_ROOT_PATH = @"C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA";
        /// <summary>
        /// 安装进度事件参数
        /// </summary>
        public class InstallProgressEventArgs : EventArgs
        {
            /// <summary>
            /// 当前阶段
            /// </summary>
            public InstallStage Stage { get; set; }

            /// <summary>
            /// 进度百分比（0-100）
            /// </summary>
            public int ProgressPercentage { get; set; }

            /// <summary>
            /// 状态消息
            /// </summary>
            public string Message { get; set; } = string.Empty;
        }

        /// <summary>
        /// 安装阶段枚举
        /// </summary>
        public enum InstallStage
        {
            /// <summary>
            /// 准备中
            /// </summary>
            Preparing,

            /// <summary>
            /// 下载中
            /// </summary>
            Downloading,

            /// <summary>
            /// 安装中
            /// </summary>
            Installing,

            /// <summary>
            /// 完成
            /// </summary>
            Completed,

            /// <summary>
            /// 失败
            /// </summary>
            Failed
        }

        /// <summary>
        /// 安装进度事件
        /// </summary>
        public event EventHandler<InstallProgressEventArgs>? ProgressChanged;

        /// <summary>
        /// 下载服务
        /// </summary>
        private readonly DownloadService _downloadService;

        /// <summary>
        /// 临时下载目录
        /// </summary>
        private readonly string _tempDownloadPath;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CudaInstallerService()
        {
            _downloadService = new DownloadService();

            // 使用软件根目录下的Downloads文件夹，便携版不污染其他路径
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            _tempDownloadPath = Path.Combine(appDir, "Downloads");

            // 确保下载目录存在
            if (!Directory.Exists(_tempDownloadPath))
            {
                Directory.CreateDirectory(_tempDownloadPath);
            }

            // 订阅下载进度
            _downloadService.ProgressChanged += OnDownloadProgressChanged;
        }

        /// <summary>
        /// 下载进度回调
        /// </summary>
        private void OnDownloadProgressChanged(object? sender, DownloadService.DownloadProgressEventArgs e)
        {
            int downloadProgress = e.TotalBytes > 0 ? (int)(e.BytesReceived * 100 / e.TotalBytes) : 0;

            string progressText;
            if (e.TotalBytes > 0)
            {
                progressText = $"正在下载CUDA [{downloadProgress}%]: {DownloadService.FormatFileSize(e.BytesReceived)} / " +
                    $"{DownloadService.FormatFileSize(e.TotalBytes)} ({DownloadService.FormatSpeed(e.SpeedBytesPerSecond)})";
            }
            else
            {
                // 服务器未返回文件大小时
                progressText = $"正在下载CUDA: 已下载 {DownloadService.FormatFileSize(e.BytesReceived)} " +
                    $"({DownloadService.FormatSpeed(e.SpeedBytesPerSecond)})";
            }

            ReportProgress(InstallStage.Downloading, downloadProgress, progressText);
        }

        /// <summary>
        /// 安装CUDA
        /// </summary>
        /// <param name="cudaTag">CUDA版本标识（如"cu129"）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否安装成功</returns>
        public async Task<bool> InstallCudaAsync(string cudaTag, CancellationToken cancellationToken = default)
        {
            try
            {
                ReportProgress(InstallStage.Preparing, 0, "正在准备安装CUDA...");

                // 获取下载信息
                (List<string> urls, string fileName) = _downloadService.GetCudaDownloadInfo(cudaTag);

                if (urls.Count == 0 || string.IsNullOrEmpty(fileName))
                {
                    ReportProgress(InstallStage.Failed, 0, "无法获取CUDA下载信息");
                    return false;
                }

                // 下载路径
                string installerPath = Path.Combine(_tempDownloadPath, fileName);

                // 检查是否已下载
                if (!File.Exists(installerPath) || new FileInfo(installerPath).Length < 1024 * 1024 * 100)
                {
                    ReportProgress(InstallStage.Downloading, 0, $"正在下载CUDA安装包...\n目标路径: {installerPath}");

                    bool downloadSuccess = await _downloadService.DownloadFileAsync(urls, installerPath, cancellationToken);

                    if (!downloadSuccess)
                    {
                        ReportProgress(InstallStage.Failed, 0, "CUDA下载失败");
                        return false;
                    }

                    ReportProgress(InstallStage.Downloading, 100, $"CUDA下载完成: {installerPath}");
                }
                else
                {
                    long fileSize = new FileInfo(installerPath).Length;
                    ReportProgress(InstallStage.Downloading, 100,
                        $"CUDA安装包已存在，跳过下载\n路径: {installerPath}\n大小: {DownloadService.FormatFileSize(fileSize)}");
                }

                // 执行静默安装
                ReportProgress(InstallStage.Installing, 0, "正在安装CUDA，请稍候...");

                bool installSuccess = await RunCudaSilentInstallAsync(installerPath, cudaTag, cancellationToken);

                if (installSuccess)
                {
                    // 配置环境变量
                    ReportProgress(InstallStage.Installing, 96, "正在配置环境变量...");
                    bool envConfigured = ConfigureCudaEnvironmentVariables(cudaTag);

                    if (envConfigured)
                    {
                        ReportProgress(InstallStage.Completed, 100, "CUDA安装完成，环境变量已配置");
                    }
                    else
                    {
                        ReportProgress(InstallStage.Completed, 100, "CUDA安装完成，但环境变量配置失败，请手动配置");
                    }

                    // 清理安装包（可选）
                    // File.Delete(installerPath);

                    return true;
                }
                else
                {
                    ReportProgress(InstallStage.Failed, 0, "CUDA安装失败");
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                ReportProgress(InstallStage.Failed, 0, "安装已取消");
                throw;
            }
            catch (Exception ex)
            {
                ReportProgress(InstallStage.Failed, 0, $"安装出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 执行CUDA静默安装
        /// </summary>
        private async Task<bool> RunCudaSilentInstallAsync(string installerPath, string cudaTag,
            CancellationToken cancellationToken)
        {
            try
            {
                // CUDA静默安装参数
                // -s: 静默安装
                // nvcc_X.X, cudart_X.X: 只安装必要的组件
                string cudaVersion = cudaTag switch
                {
                    "cu118" => "11.8",
                    "cu126" => "12.6",
                    "cu129" => "12.9",
                    _ => "12.9"
                };

                // 只安装必要的运行时组件
                string arguments = $"-s nvcc_{cudaVersion} cudart_{cudaVersion} cuobjdump_{cudaVersion} " +
                                   $"cupti_{cudaVersion} cublas_{cudaVersion} cufft_{cudaVersion} " +
                                   $"curand_{cudaVersion} cusolver_{cudaVersion} cusparse_{cudaVersion} " +
                                   $"npp_{cudaVersion} nvrtc_{cudaVersion} nvml_dev_{cudaVersion}";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = installerPath,
                    Arguments = arguments,
                    UseShellExecute = true,
                    Verb = "runas",  // 请求管理员权限
                    CreateNoWindow = true
                };

                // CUDA安装目标路径
                string cudaInstallPath = $@"C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v{cudaVersion}";

                ReportProgress(InstallStage.Installing, 0,
                    $"正在安装CUDA {cudaVersion}，这可能需要5-15分钟，请耐心等待...");

                using Process? process = Process.Start(startInfo);

                if (process == null)
                {
                    return false;
                }

                // 等待安装完成（设置超时30分钟）
                int timeoutMs = 30 * 60 * 1000;
                int checkIntervalMs = 3000;
                int elapsed = 0;
                DateTime startTime = DateTime.Now;

                // 安装阶段描述
                string[] installPhases = new[]
                {
                    "正在解压安装文件...",
                    "正在安装CUDA核心组件...",
                    "正在安装cuBLAS库...",
                    "正在安装cuFFT库...",
                    "正在安装cuRAND库...",
                    "正在安装cuSOLVER库...",
                    "正在安装cuSPARSE库...",
                    "正在安装NPP库...",
                    "正在配置环境变量...",
                    "正在完成安装..."
                };

                while (!process.HasExited && elapsed < timeoutMs)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        try { process.Kill(); } catch { }
                        throw new OperationCanceledException();
                    }

                    await Task.Delay(checkIntervalMs, cancellationToken);
                    elapsed += checkIntervalMs;

                    // 计算已用时间
                    TimeSpan elapsedTime = DateTime.Now - startTime;
                    string elapsedStr = elapsedTime.TotalMinutes >= 1
                        ? $"{(int)elapsedTime.TotalMinutes}分{elapsedTime.Seconds}秒"
                        : $"{elapsedTime.Seconds}秒";

                    // 根据时间和目录变化估算进度
                    int progress = EstimateCudaInstallProgress(cudaInstallPath, elapsed, timeoutMs);

                    // 根据进度选择阶段描述
                    int phaseIndex = Math.Min(progress / 10, installPhases.Length - 1);
                    string phaseDesc = installPhases[phaseIndex];

                    // 检测安装目录是否开始创建
                    string statusDetail = "";
                    if (Directory.Exists(cudaInstallPath))
                    {
                        try
                        {
                            int fileCount = Directory.GetFiles(cudaInstallPath, "*", SearchOption.AllDirectories).Length;
                            statusDetail = $" (已安装 {fileCount} 个文件)";
                        }
                        catch { }
                    }

                    ReportProgress(InstallStage.Installing, progress,
                        $"[已用时 {elapsedStr}] {phaseDesc}{statusDetail}");
                }

                if (!process.HasExited)
                {
                    try { process.Kill(); } catch { }
                    return false;
                }

                // 检查退出码
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CUDA安装失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 估算CUDA安装进度
        /// </summary>
        private int EstimateCudaInstallProgress(string cudaInstallPath, int elapsedMs, int timeoutMs)
        {
            // 基础进度：根据时间估算（假设平均安装时间为8分钟）
            int estimatedInstallTimeMs = 8 * 60 * 1000;
            int timeBasedProgress = Math.Min(90, elapsedMs * 100 / estimatedInstallTimeMs);

            // 如果安装目录存在，给予额外进度
            if (Directory.Exists(cudaInstallPath))
            {
                try
                {
                    // 检查关键文件是否存在
                    bool hasNvcc = File.Exists(Path.Combine(cudaInstallPath, "bin", "nvcc.exe"));
                    bool hasCudart = File.Exists(Path.Combine(cudaInstallPath, "bin", "cudart64_*.dll".Replace("*", ""))) ||
                                     Directory.GetFiles(Path.Combine(cudaInstallPath, "bin"), "cudart64_*.dll").Length > 0;

                    if (hasNvcc && hasCudart)
                    {
                        // 关键文件已存在，进度至少80%
                        timeBasedProgress = Math.Max(timeBasedProgress, 80);
                    }
                    else if (hasNvcc || hasCudart)
                    {
                        // 部分关键文件存在，进度至少50%
                        timeBasedProgress = Math.Max(timeBasedProgress, 50);
                    }
                    else
                    {
                        // 目录存在但关键文件不存在，进度至少20%
                        timeBasedProgress = Math.Max(timeBasedProgress, 20);
                    }
                }
                catch { }
            }

            return Math.Min(95, timeBasedProgress);
        }

        /// <summary>
        /// 配置CUDA环境变量
        /// </summary>
        /// <param name="cudaTag">CUDA版本标识</param>
        /// <returns>是否配置成功</returns>
        private bool ConfigureCudaEnvironmentVariables(string cudaTag)
        {
            try
            {
                string cudaVersion = cudaTag switch
                {
                    "cu118" => "11.8",
                    "cu126" => "12.6",
                    "cu129" => "12.9",
                    _ => "12.9"
                };

                string cudaPath = Path.Combine(CUDA_ROOT_PATH, $"v{cudaVersion}");
                string cudaBinPath = Path.Combine(cudaPath, "bin");
                string cudaLibPath = Path.Combine(cudaPath, "libnvvp");

                // 验证路径存在
                if (!Directory.Exists(cudaPath))
                {
                    System.Diagnostics.Debug.WriteLine($"CUDA路径不存在: {cudaPath}");
                    return false;
                }

                // 设置 CUDA_PATH 环境变量
                Environment.SetEnvironmentVariable("CUDA_PATH", cudaPath, EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable($"CUDA_PATH_V{cudaVersion.Replace(".", "_")}", cudaPath, EnvironmentVariableTarget.Machine);

                // 获取当前系统PATH
                string? currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);

                if (string.IsNullOrEmpty(currentPath))
                {
                    return false;
                }

                // 检查并添加CUDA bin路径到PATH
                bool pathModified = false;
                List<string> pathParts = currentPath.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

                // 移除旧的CUDA路径（如果存在）
                pathParts.RemoveAll(p => p.Contains(@"NVIDIA GPU Computing Toolkit\CUDA", StringComparison.OrdinalIgnoreCase));

                // 添加新的CUDA路径
                if (Directory.Exists(cudaBinPath))
                {
                    pathParts.Insert(0, cudaBinPath);
                    pathModified = true;
                }

                if (Directory.Exists(cudaLibPath))
                {
                    pathParts.Insert(1, cudaLibPath);
                    pathModified = true;
                }

                if (pathModified)
                {
                    string newPath = string.Join(";", pathParts);
                    Environment.SetEnvironmentVariable("Path", newPath, EnvironmentVariableTarget.Machine);
                }

                // 通知系统环境变量已更改
                NotifyEnvironmentChange();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"配置环境变量失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 通知系统环境变量已更改
        /// </summary>
        private void NotifyEnvironmentChange()
        {
            try
            {
                // 发送 WM_SETTINGCHANGE 消息通知系统环境变量已更改
                // 这需要P/Invoke，但由于权限问题可能不起作用
                // 用户可能需要重启命令行或注销重新登录

                // 尝试通过setx命令刷新环境变量
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c setx CUDA_REFRESH 1 >nul 2>&1 && reg delete HKCU\\Environment /v CUDA_REFRESH /f >nul 2>&1",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process? p = Process.Start(psi);
                p?.WaitForExit(3000);
            }
            catch { }
        }

        /// <summary>
        /// 报告安装进度
        /// </summary>
        private void ReportProgress(InstallStage stage, int progress, string message)
        {
            ProgressChanged?.Invoke(this, new InstallProgressEventArgs
            {
                Stage = stage,
                ProgressPercentage = progress,
                Message = message
            });
        }
    }
}
