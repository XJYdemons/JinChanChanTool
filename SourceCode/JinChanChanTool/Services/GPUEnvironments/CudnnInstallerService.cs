using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.GPUEnvironments
{
    /// <summary>
    /// cuDNN安装服务
    /// 负责下载cuDNN并解压到CUDA目录
    /// </summary>
    internal class CudnnInstallerService
    {
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
            /// 解压中
            /// </summary>
            Extracting,

            /// <summary>
            /// 复制文件中
            /// </summary>
            CopyingFiles,

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
        public CudnnInstallerService()
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
                progressText = $"正在下载cuDNN [{downloadProgress}%]: {DownloadService.FormatFileSize(e.BytesReceived)} / " +
                    $"{DownloadService.FormatFileSize(e.TotalBytes)} ({DownloadService.FormatSpeed(e.SpeedBytesPerSecond)})";
            }
            else
            {
                progressText = $"正在下载cuDNN: 已下载 {DownloadService.FormatFileSize(e.BytesReceived)} " +
                    $"({DownloadService.FormatSpeed(e.SpeedBytesPerSecond)})";
            }

            ReportProgress(InstallStage.Downloading, downloadProgress, progressText);
        }

        /// <summary>
        /// 安装cuDNN
        /// </summary>
        /// <param name="cudnnTag">cuDNN版本标识（如"cudnn910"）</param>
        /// <param name="cudaPath">CUDA安装路径</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否安装成功</returns>
        public async Task<bool> InstallCudnnAsync(string cudnnTag, string cudaPath,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ReportProgress(InstallStage.Preparing, 0, "正在准备安装cuDNN...");

                // 验证CUDA路径
                if (string.IsNullOrEmpty(cudaPath) || !Directory.Exists(cudaPath))
                {
                    ReportProgress(InstallStage.Failed, 0, "CUDA安装路径无效");
                    return false;
                }

                // 获取下载信息
                (List<string> urls, string fileName) = _downloadService.GetCudnnDownloadInfo(cudnnTag);

                if (urls.Count == 0 || string.IsNullOrEmpty(fileName))
                {
                    ReportProgress(InstallStage.Failed, 0, "无法获取cuDNN下载信息");
                    return false;
                }

                // 下载路径
                string zipPath = Path.Combine(_tempDownloadPath, fileName);

                // 检查是否已下载
                if (!File.Exists(zipPath) || new FileInfo(zipPath).Length < 1024 * 1024 * 10)
                {
                    ReportProgress(InstallStage.Downloading, 0, $"正在下载cuDNN...\n目标路径: {zipPath}");

                    bool downloadSuccess = await _downloadService.DownloadFileAsync(urls, zipPath, cancellationToken);

                    if (!downloadSuccess)
                    {
                        ReportProgress(InstallStage.Failed, 0, "cuDNN下载失败");
                        return false;
                    }

                    ReportProgress(InstallStage.Downloading, 100, $"cuDNN下载完成: {zipPath}");
                }
                else
                {
                    long fileSize = new FileInfo(zipPath).Length;
                    ReportProgress(InstallStage.Downloading, 100,
                        $"cuDNN压缩包已存在，跳过下载\n路径: {zipPath}\n大小: {DownloadService.FormatFileSize(fileSize)}");
                }

                // 解压并复制文件
                ReportProgress(InstallStage.Extracting, 0, "正在解压cuDNN...");

                bool extractSuccess = await ExtractAndCopyCudnnAsync(zipPath, cudaPath, cancellationToken);

                if (extractSuccess)
                {
                    ReportProgress(InstallStage.Completed, 100, "cuDNN安装完成");

                    // 清理压缩包（可选）
                    // File.Delete(zipPath);

                    return true;
                }
                else
                {
                    ReportProgress(InstallStage.Failed, 0, "cuDNN安装失败");
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
        /// 解压cuDNN并复制到CUDA目录
        /// </summary>
        private async Task<bool> ExtractAndCopyCudnnAsync(string zipPath, string cudaPath,
            CancellationToken cancellationToken)
        {
            string extractPath = Path.Combine(_tempDownloadPath, "cudnn_extract");

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

                ReportProgress(InstallStage.CopyingFiles, 0, "正在复制cuDNN文件到CUDA目录...");

                // 找到解压后的目录（通常有一个子目录）
                string[] extractedDirs = Directory.GetDirectories(extractPath);
                string sourceDir = extractedDirs.Length > 0 ? extractedDirs[0] : extractPath;

                // 统计要复制的文件总数
                int totalFiles = 0;
                string sourceBin = Path.Combine(sourceDir, "bin");
                string sourceInclude = Path.Combine(sourceDir, "include");
                string sourceLib = Path.Combine(sourceDir, "lib");

                if (Directory.Exists(sourceBin))
                    totalFiles += Directory.GetFiles(sourceBin, "*", SearchOption.AllDirectories).Length;
                if (Directory.Exists(sourceInclude))
                    totalFiles += Directory.GetFiles(sourceInclude, "*", SearchOption.AllDirectories).Length;
                if (Directory.Exists(sourceLib))
                    totalFiles += Directory.GetFiles(sourceLib, "*", SearchOption.AllDirectories).Length;

                int copiedFiles = 0;

                // 复制bin目录（DLL文件）
                string destBin = Path.Combine(cudaPath, "bin");
                if (Directory.Exists(sourceBin))
                {
                    copiedFiles = await CopyDirectoryWithProgressAsync(sourceBin, destBin,
                        copiedFiles, totalFiles, "DLL文件", cancellationToken);
                }

                // 复制include目录（头文件）
                string destInclude = Path.Combine(cudaPath, "include");
                if (Directory.Exists(sourceInclude))
                {
                    copiedFiles = await CopyDirectoryWithProgressAsync(sourceInclude, destInclude,
                        copiedFiles, totalFiles, "头文件", cancellationToken);
                }

                // 复制lib目录（库文件）
                string destLib = Path.Combine(cudaPath, "lib");
                if (Directory.Exists(sourceLib))
                {
                    copiedFiles = await CopyDirectoryWithProgressAsync(sourceLib, destLib,
                        copiedFiles, totalFiles, "库文件", cancellationToken);
                }

                ReportProgress(InstallStage.CopyingFiles, 100, $"已复制 {copiedFiles} 个文件");

                // 清理解压目录
                Directory.Delete(extractPath, true);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解压cuDNN失败: {ex.Message}");

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
                    }

                    extractedEntries++;

                    // 每500ms或每10%报告一次进度
                    if ((DateTime.Now - lastReportTime).TotalMilliseconds >= 500 ||
                        extractedEntries == totalEntries)
                    {
                        int progress = totalEntries > 0 ? extractedEntries * 100 / totalEntries : 0;
                        ReportProgress(InstallStage.Extracting, progress,
                            $"正在解压cuDNN: {extractedEntries}/{totalEntries} 个文件 ({progress}%)");
                        lastReportTime = DateTime.Now;
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// 带进度报告的目录复制
        /// </summary>
        private async Task<int> CopyDirectoryWithProgressAsync(string sourceDir, string destDir,
            int currentCopied, int totalFiles, string fileType, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                // 确保目标目录存在
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                int copiedCount = currentCopied;
                DateTime lastReportTime = DateTime.Now;

                // 复制所有文件
                foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string relativePath = file.Substring(sourceDir.Length).TrimStart(Path.DirectorySeparatorChar);
                    string destFile = Path.Combine(destDir, relativePath);

                    // 确保父目录存在
                    string? parentDir = Path.GetDirectoryName(destFile);
                    if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
                    {
                        Directory.CreateDirectory(parentDir);
                    }

                    // 如果目标文件存在，先删除
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }

                    File.Copy(file, destFile);
                    copiedCount++;

                    // 每500ms报告一次进度
                    if ((DateTime.Now - lastReportTime).TotalMilliseconds >= 500)
                    {
                        int progress = totalFiles > 0 ? copiedCount * 100 / totalFiles : 0;
                        ReportProgress(InstallStage.CopyingFiles, progress,
                            $"正在复制{fileType}: {copiedCount}/{totalFiles} ({progress}%)");
                        lastReportTime = DateTime.Now;
                    }
                }

                return copiedCount;
            }, cancellationToken);
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
