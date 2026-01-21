using JinChanChanTool.DataClass.GPUEnvironments;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.GPUEnvironments
{
    /// <summary>
    /// 运行时部署服务
    /// 负责下载并部署PaddleInference GPU运行时DLL文件到主程序目录
    /// </summary>
    internal class RuntimeDeployService
    {
        /// <summary>
        /// 部署进度事件参数
        /// </summary>
        public class DeployProgressEventArgs : EventArgs
        {
            /// <summary>
            /// 当前阶段
            /// </summary>
            public DeployStage Stage { get; set; }

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
        /// 部署阶段枚举
        /// </summary>
        public enum DeployStage
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
            /// 解压中
            /// </summary>
            Extracting,

            /// <summary>
            /// 部署中
            /// </summary>
            Deploying,

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
        /// 部署进度事件
        /// </summary>
        public event EventHandler<DeployProgressEventArgs>? ProgressChanged;

        /// <summary>
        /// 下载服务
        /// </summary>
        private readonly DownloadService _downloadService;

        /// <summary>
        /// 临时下载目录
        /// </summary>
        private readonly string _tempDownloadPath;

        /// <summary>
        /// 需要部署的运行时DLL文件列表
        /// </summary>
        private static readonly string[] RUNTIME_DLLS = new[]
        {
            "paddle_inference_c.dll",
            "common.dll",
            "paddle2onnx.dll"
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeDeployService()
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
                progressText = $"正在下载运行时 [{downloadProgress}%]: {DownloadService.FormatFileSize(e.BytesReceived)} / " +
                    $"{DownloadService.FormatFileSize(e.TotalBytes)} ({DownloadService.FormatSpeed(e.SpeedBytesPerSecond)})";
            }
            else
            {
                progressText = $"正在下载运行时: 已下载 {DownloadService.FormatFileSize(e.BytesReceived)} " +
                    $"({DownloadService.FormatSpeed(e.SpeedBytesPerSecond)})";
            }

            ReportProgress(DeployStage.Downloading, downloadProgress, progressText);
        }

        /// <summary>
        /// 部署运行时到主程序目录
        /// </summary>
        /// <param name="runtimeConfig">运行时配置</param>
        /// <param name="targetDirectory">目标目录（主程序目录）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否部署成功</returns>
        public async Task<bool> DeployRuntimeAsync(RuntimeConfig runtimeConfig, string targetDirectory,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ReportProgress(DeployStage.Preparing, 0, "正在准备部署运行时...");

                // 验证目标目录
                if (string.IsNullOrEmpty(targetDirectory) || !Directory.Exists(targetDirectory))
                {
                    ReportProgress(DeployStage.Failed, 0, "目标目录无效");
                    return false;
                }

                // 获取下载URLs
                string runtimeFileName = runtimeConfig.RuntimeZipFileName;
                List<string> urls = _downloadService.GetRuntimeDownloadUrls(runtimeFileName);

                if (urls.Count == 0)
                {
                    ReportProgress(DeployStage.Failed, 0, "无法获取运行时下载地址");
                    return false;
                }

                // 下载路径
                string zipPath = Path.Combine(_tempDownloadPath, runtimeFileName);

                // 检查是否已下载
                if (!File.Exists(zipPath) || new FileInfo(zipPath).Length < 1024 * 10)
                {
                    ReportProgress(DeployStage.Downloading, 0, $"正在下载运行时文件...\n目标路径: {zipPath}");

                    bool downloadSuccess = await _downloadService.DownloadFileAsync(urls, zipPath, cancellationToken);

                    if (!downloadSuccess)
                    {
                        ReportProgress(DeployStage.Failed, 0, "运行时下载失败");
                        return false;
                    }

                    ReportProgress(DeployStage.Downloading, 100, $"运行时下载完成: {zipPath}");
                }
                else
                {
                    long fileSize = new FileInfo(zipPath).Length;
                    ReportProgress(DeployStage.Downloading, 100,
                        $"运行时文件已存在，跳过下载\n路径: {zipPath}\n大小: {DownloadService.FormatFileSize(fileSize)}");
                }

                // 解压并部署
                ReportProgress(DeployStage.Extracting, 0, "正在解压运行时文件...");

                bool deploySuccess = await ExtractAndDeployRuntimeAsync(zipPath, targetDirectory, cancellationToken);

                if (deploySuccess)
                {
                    ReportProgress(DeployStage.Completed, 100, "运行时部署完成");

                    // 清理压缩包（可选）
                    // File.Delete(zipPath);

                    return true;
                }
                else
                {
                    ReportProgress(DeployStage.Failed, 0, "运行时部署失败");
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                ReportProgress(DeployStage.Failed, 0, "部署已取消");
                throw;
            }
            catch (Exception ex)
            {
                ReportProgress(DeployStage.Failed, 0, $"部署出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 解压并部署运行时文件
        /// </summary>
        private async Task<bool> ExtractAndDeployRuntimeAsync(string zipPath, string targetDirectory,
            CancellationToken cancellationToken)
        {
            string extractPath = Path.Combine(_tempDownloadPath, "runtime_extract");

            try
            {
                // 清理之前的解压目录
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }

                Directory.CreateDirectory(extractPath);

                // 使用带进度的解压方法
                await ExtractZipWithProgressAsync(zipPath, extractPath, cancellationToken);

                ReportProgress(DeployStage.Deploying, 0, "正在部署运行时文件...");

                // 查找并复制DLL文件
                int copiedCount = 0;
                int totalCount = RUNTIME_DLLS.Length;

                foreach (string dllName in RUNTIME_DLLS)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // 在解压目录中查找DLL
                    string[] foundFiles = Directory.GetFiles(extractPath, dllName, SearchOption.AllDirectories);

                    if (foundFiles.Length > 0)
                    {
                        string sourceFile = foundFiles[0];
                        string destFile = Path.Combine(targetDirectory, dllName);

                        // 获取文件大小用于显示
                        long fileSize = new FileInfo(sourceFile).Length;
                        string fileSizeStr = DownloadService.FormatFileSize(fileSize);

                        ReportProgress(DeployStage.Deploying, copiedCount * 100 / totalCount,
                            $"正在部署: {dllName} ({fileSizeStr})...");

                        // 备份原文件（如果存在）
                        if (File.Exists(destFile))
                        {
                            string backupFile = destFile + ".backup";
                            if (File.Exists(backupFile))
                            {
                                File.Delete(backupFile);
                            }
                            File.Move(destFile, backupFile);
                        }

                        // 复制新文件（大文件使用流复制以便报告进度）
                        await CopyFileWithProgressAsync(sourceFile, destFile, dllName, cancellationToken);

                        copiedCount++;

                        int progress = copiedCount * 100 / totalCount;
                        ReportProgress(DeployStage.Deploying, progress, $"已部署: {dllName} ({copiedCount}/{totalCount})");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"未找到DLL文件: {dllName}");
                    }
                }

                // 清理解压目录
                ReportProgress(DeployStage.Deploying, 95, "正在清理临时文件...");
                Directory.Delete(extractPath, true);

                // 检查是否成功部署了足够的文件
                if (copiedCount < 1)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"部署运行时失败: {ex.Message}");

                // 清理
                try
                {
                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }
                }
                catch { }

                return false;
            }
        }

        /// <summary>
        /// 带进度报告的ZIP解压
        /// </summary>
        private async Task ExtractZipWithProgressAsync(string zipPath, string extractPath,
            CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                using ZipArchive archive = ZipFile.OpenRead(zipPath);
                int totalEntries = archive.Entries.Count;
                int extractedEntries = 0;
                long totalSize = archive.Entries.Sum(e => e.Length);
                long extractedSize = 0;
                DateTime lastReportTime = DateTime.Now;

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string destinationPath = Path.Combine(extractPath, entry.FullName);

                    // 如果是目录，创建目录
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        // 确保父目录存在
                        string? parentDir = Path.GetDirectoryName(destinationPath);
                        if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
                        {
                            Directory.CreateDirectory(parentDir);
                        }

                        // 解压文件
                        entry.ExtractToFile(destinationPath, true);
                        extractedSize += entry.Length;
                    }

                    extractedEntries++;

                    // 每500ms报告一次进度
                    if ((DateTime.Now - lastReportTime).TotalMilliseconds >= 500 ||
                        extractedEntries == totalEntries)
                    {
                        int progress = totalSize > 0 ? (int)(extractedSize * 100 / totalSize) : 0;
                        string extractedSizeStr = DownloadService.FormatFileSize(extractedSize);
                        string totalSizeStr = DownloadService.FormatFileSize(totalSize);

                        ReportProgress(DeployStage.Extracting, progress,
                            $"正在解压运行时: {extractedSizeStr}/{totalSizeStr} ({progress}%)");
                        lastReportTime = DateTime.Now;
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// 带进度报告的文件复制（用于大文件）
        /// </summary>
        private async Task CopyFileWithProgressAsync(string sourceFile, string destFile,
            string fileName, CancellationToken cancellationToken)
        {
            const int bufferSize = 1024 * 1024; // 1MB buffer
            FileInfo fileInfo = new FileInfo(sourceFile);
            long totalBytes = fileInfo.Length;

            // 小文件直接复制
            if (totalBytes < bufferSize * 2)
            {
                File.Copy(sourceFile, destFile, true);
                return;
            }

            // 大文件使用流复制
            await Task.Run(async () =>
            {
                using FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
                using FileStream destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write);

                byte[] buffer = new byte[bufferSize];
                long copiedBytes = 0;
                int bytesRead;
                DateTime lastReportTime = DateTime.Now;

                while ((bytesRead = await sourceStream.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    await destStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                    copiedBytes += bytesRead;

                    // 每500ms报告一次进度
                    if ((DateTime.Now - lastReportTime).TotalMilliseconds >= 500)
                    {
                        int progress = (int)(copiedBytes * 100 / totalBytes);
                        string copiedStr = DownloadService.FormatFileSize(copiedBytes);
                        string totalStr = DownloadService.FormatFileSize(totalBytes);

                        ReportProgress(DeployStage.Deploying, progress,
                            $"正在复制 {fileName}: {copiedStr}/{totalStr}");
                        lastReportTime = DateTime.Now;
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// 验证目标目录是否为有效的主程序目录
        /// </summary>
        /// <param name="directory">目录路径</param>
        /// <returns>验证结果</returns>
        public static (bool IsValid, string Message) ValidateTargetDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return (false, "目录路径为空");
            }

            if (!Directory.Exists(directory))
            {
                return (false, "目录不存在");
            }

            // 检查是否存在主程序EXE文件
            string[] exeFiles = Directory.GetFiles(directory, "*.exe");
            if (exeFiles.Length == 0)
            {
                return (false, "未找到可执行文件");
            }

            // 检查是否存在JinChanChanTool相关文件
            bool hasMainExe = exeFiles.Any(f =>
                Path.GetFileName(f).Contains("JinChanChanTool", StringComparison.OrdinalIgnoreCase));

            if (!hasMainExe)
            {
                // 不强制要求，只是警告
                return (true, "警告: 未检测到金铲铲助手主程序，请确认目录正确");
            }

            return (true, "目录验证通过");
        }

        /// <summary>
        /// 报告部署进度
        /// </summary>
        private void ReportProgress(DeployStage stage, int progress, string message)
        {
            ProgressChanged?.Invoke(this, new DeployProgressEventArgs
            {
                Stage = stage,
                ProgressPercentage = progress,
                Message = message
            });
        }
    }
}
