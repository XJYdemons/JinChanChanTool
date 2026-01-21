using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.GPUEnvironments
{
    /// <summary>
    /// 下载服务
    /// 支持多源下载、断点续传、进度回调
    /// </summary>
    internal class DownloadService
    {
        /// <summary>
        /// 下载进度事件参数
        /// </summary>
        public class DownloadProgressEventArgs : EventArgs
        {
            /// <summary>
            /// 已下载字节数
            /// </summary>
            public long BytesReceived { get; set; }

            /// <summary>
            /// 总字节数（如果已知）
            /// </summary>
            public long TotalBytes { get; set; }

            /// <summary>
            /// 下载进度百分比（0-100）
            /// </summary>
            public double ProgressPercentage => TotalBytes > 0 ? (double)BytesReceived / TotalBytes * 100 : 0;

            /// <summary>
            /// 当前下载速度（字节/秒）
            /// </summary>
            public long SpeedBytesPerSecond { get; set; }

            /// <summary>
            /// 当前使用的下载源
            /// </summary>
            public string CurrentSource { get; set; } = string.Empty;

            /// <summary>
            /// 状态消息
            /// </summary>
            public string Message { get; set; } = string.Empty;
        }

        /// <summary>
        /// 下载进度事件
        /// </summary>
        public event EventHandler<DownloadProgressEventArgs>? ProgressChanged;

        /// <summary>
        /// HTTP客户端
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 下载源配置
        /// </summary>
        private JObject? _downloadSources;

        /// <summary>
        /// 下载缓冲区大小（64KB）
        /// </summary>
        private const int BUFFER_SIZE = 65536;

        /// <summary>
        /// 单个源最大重试次数
        /// </summary>
        private const int MAX_RETRY_PER_SOURCE = 3;

        /// <summary>
        /// 测速时下载的数据量（256KB）
        /// </summary>
        private const int SPEED_TEST_BYTES = 256 * 1024;

        /// <summary>
        /// 测速超时时间（秒）
        /// </summary>
        private const int SPEED_TEST_TIMEOUT_SECONDS = 10;

        /// <summary>
        /// 最低可接受下载速度（字节/秒，10KB/s）
        /// 如果下载速度持续低于此值，将尝试切换到其他下载源
        /// </summary>
        private const long MIN_ACCEPTABLE_SPEED = 10 * 1024;

        /// <summary>
        /// 低速检测时间窗口（秒）
        /// 下载速度需要持续低于阈值此时长才会触发源切换
        /// </summary>
        private const int LOW_SPEED_DETECTION_WINDOW_SECONDS = 30;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DownloadService()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(30)
            };

            // 设置User-Agent
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            LoadDownloadSources();
        }

        /// <summary>
        /// 加载下载源配置
        /// </summary>
        private void LoadDownloadSources()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "DownloadSources.json");

                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    _downloadSources = JObject.Parse(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载下载源配置失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取运行时下载URLs
        /// </summary>
        /// <param name="runtimeFileName">运行时文件名（如"cu129_cudnn910_sm89.zip"）</param>
        /// <returns>下载URL列表</returns>
        public List<string> GetRuntimeDownloadUrls(string runtimeFileName)
        {
            List<string> urls = new List<string>();

            try
            {
                JArray? baseUrls = _downloadSources?["RuntimeSources"]?["BaseUrls"] as JArray;
                if (baseUrls != null)
                {
                    foreach (JToken url in baseUrls)
                    {
                        urls.Add(url.ToString() + runtimeFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取运行时下载URLs失败: {ex.Message}");
            }

            // 如果配置加载失败，使用默认URLs
            if (urls.Count == 0)
            {
                urls.Add($"https://github.com/XJYdemons/JinChanChanTool/releases/download/v4.5.3/{runtimeFileName}");
                urls.Add($"https://mirror.ghproxy.com/https://github.com/XJYdemons/JinChanChanTool/releases/download/v4.5.3/{runtimeFileName}");
            }

            return urls;
        }

        /// <summary>
        /// 获取CUDA下载URLs
        /// </summary>
        /// <param name="cudaTag">CUDA版本标识（如"cu129"）</param>
        /// <returns>下载URL和文件名</returns>
        public (List<string> Urls, string FileName) GetCudaDownloadInfo(string cudaTag)
        {
            List<string> urls = new List<string>();
            string fileName = string.Empty;

            try
            {
                JObject? cudaVersion = _downloadSources?["CudaSources"]?["Versions"]?[cudaTag] as JObject;
                if (cudaVersion != null)
                {
                    fileName = cudaVersion["FileName"]?.ToString() ?? string.Empty;
                    JArray? urlArray = cudaVersion["Urls"] as JArray;
                    if (urlArray != null)
                    {
                        foreach (JToken url in urlArray)
                        {
                            urls.Add(url.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取CUDA下载信息失败: {ex.Message}");
            }

            return (urls, fileName);
        }

        /// <summary>
        /// 获取cuDNN下载URLs
        /// </summary>
        /// <param name="cudnnTag">cuDNN版本标识（如"cudnn910"）</param>
        /// <returns>下载URL和文件名</returns>
        public (List<string> Urls, string FileName) GetCudnnDownloadInfo(string cudnnTag)
        {
            List<string> urls = new List<string>();
            string fileName = string.Empty;

            try
            {
                JObject? cudnnVersion = _downloadSources?["CudnnSources"]?["Versions"]?[cudnnTag] as JObject;
                if (cudnnVersion != null)
                {
                    fileName = cudnnVersion["FileName"]?.ToString() ?? string.Empty;
                    JArray? urlArray = cudnnVersion["Urls"] as JArray;
                    if (urlArray != null)
                    {
                        foreach (JToken url in urlArray)
                        {
                            urls.Add(url.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取cuDNN下载信息失败: {ex.Message}");
            }

            return (urls, fileName);
        }

        /// <summary>
        /// 从多个源下载文件（支持断点续传、自动测速选择最快源）
        /// </summary>
        /// <param name="urls">下载源URL列表</param>
        /// <param name="destinationPath">保存路径</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否下载成功</returns>
        public async Task<bool> DownloadFileAsync(List<string> urls, string destinationPath,
            CancellationToken cancellationToken = default)
        {
            // 确保目录存在
            string? directory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 临时文件路径（用于断点续传）
            string tempPath = destinationPath + ".downloading";

            // 获取已下载的字节数
            long existingBytes = 0;
            if (File.Exists(tempPath))
            {
                existingBytes = new FileInfo(tempPath).Length;
            }

            // 如果有多个源，先测速选择最快的源（每次下载都测速以确保选择当前最快的源）
            List<string> sortedUrls = urls;
            if (urls.Count > 1)
            {
                ReportProgress(0, 0, 0, "", "正在测试下载源速度，请稍候...");
                sortedUrls = await TestAndSortUrlsBySpeedAsync(urls, cancellationToken);
            }

            // 尝试每个下载源
            foreach (string url in sortedUrls)
            {
                for (int retry = 0; retry < MAX_RETRY_PER_SOURCE; retry++)
                {
                    try
                    {
                        ReportProgress(existingBytes, 0, 0, url, $"正在连接 {GetHostFromUrl(url)}...");

                        bool success = await DownloadFromUrlAsync(url, tempPath, existingBytes, cancellationToken);

                        if (success)
                        {
                            // 下载完成，重命名文件
                            if (File.Exists(destinationPath))
                            {
                                File.Delete(destinationPath);
                            }
                            File.Move(tempPath, destinationPath);
                            return true;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"下载失败 ({url}): {ex.Message}");

                        // 更新已下载的字节数（用于下次重试或切换源）
                        if (File.Exists(tempPath))
                        {
                            existingBytes = new FileInfo(tempPath).Length;
                        }

                        // 等待后重试
                        if (retry < MAX_RETRY_PER_SOURCE - 1)
                        {
                            ReportProgress(existingBytes, 0, 0, url, $"下载失败，{3}秒后重试...");
                            await Task.Delay(3000, cancellationToken);
                        }
                    }
                }

                ReportProgress(existingBytes, 0, 0, url, "切换到备用下载源...");
            }

            return false;
        }

        /// <summary>
        /// 从指定URL下载文件
        /// </summary>
        private async Task<bool> DownloadFromUrlAsync(string url, string tempPath, long existingBytes,
            CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            // 如果有已下载的部分，请求断点续传
            if (existingBytes > 0)
            {
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingBytes, null);
            }

            using HttpResponseMessage response = await _httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            // 检查是否支持断点续传
            bool isPartialContent = response.StatusCode == System.Net.HttpStatusCode.PartialContent;
            bool isOk = response.StatusCode == System.Net.HttpStatusCode.OK;

            if (!isPartialContent && !isOk)
            {
                throw new HttpRequestException($"服务器返回错误状态码: {response.StatusCode}");
            }

            // 如果服务器不支持断点续传且返回200，需要从头开始
            if (isOk && existingBytes > 0)
            {
                existingBytes = 0;
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }

            // 获取总大小
            long totalBytes = (response.Content.Headers.ContentLength ?? 0) + existingBytes;

            // 打开文件流（追加模式）
            using FileStream fileStream = new FileStream(tempPath,
                existingBytes > 0 ? FileMode.Append : FileMode.Create,
                FileAccess.Write, FileShare.None, BUFFER_SIZE, true);

            using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            byte[] buffer = new byte[BUFFER_SIZE];
            long bytesReceived = existingBytes;
            int bytesRead;

            DateTime lastReportTime = DateTime.Now;
            long lastReportBytes = bytesReceived;
            DateTime? lowSpeedStartTime = null;

            while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                bytesReceived += bytesRead;

                // 每500ms报告一次进度
                if ((DateTime.Now - lastReportTime).TotalMilliseconds >= 500)
                {
                    double elapsed = (DateTime.Now - lastReportTime).TotalSeconds;
                    long speed = (long)((bytesReceived - lastReportBytes) / elapsed);

                    ReportProgress(bytesReceived, totalBytes, speed, url, "正在下载...");

                    // 检测低速情况
                    if (speed < MIN_ACCEPTABLE_SPEED)
                    {
                        if (lowSpeedStartTime == null)
                        {
                            lowSpeedStartTime = DateTime.Now;
                        }
                        else if ((DateTime.Now - lowSpeedStartTime.Value).TotalSeconds >= LOW_SPEED_DETECTION_WINDOW_SECONDS)
                        {
                            // 速度持续过低，抛出异常以触发源切换
                            throw new HttpRequestException($"下载速度过低 ({FormatSpeed(speed)})，尝试切换下载源");
                        }
                    }
                    else
                    {
                        // 速度恢复正常，重置低速计时器
                        lowSpeedStartTime = null;
                    }

                    lastReportTime = DateTime.Now;
                    lastReportBytes = bytesReceived;
                }
            }

            // 最终进度报告
            ReportProgress(bytesReceived, totalBytes, 0, url, "下载完成");

            return true;
        }

        /// <summary>
        /// 报告下载进度
        /// </summary>
        private void ReportProgress(long bytesReceived, long totalBytes, long speed, string source, string message)
        {
            ProgressChanged?.Invoke(this, new DownloadProgressEventArgs
            {
                BytesReceived = bytesReceived,
                TotalBytes = totalBytes,
                SpeedBytesPerSecond = speed,
                CurrentSource = source,
                Message = message
            });
        }

        /// <summary>
        /// 从URL中提取主机名
        /// </summary>
        private string GetHostFromUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                return uri.Host;
            }
            catch
            {
                return url;
            }
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        /// <summary>
        /// 格式化下载速度
        /// </summary>
        public static string FormatSpeed(long bytesPerSecond)
        {
            return FormatFileSize(bytesPerSecond) + "/s";
        }

        /// <summary>
        /// 测试多个下载源的速度，并按速度排序返回
        /// </summary>
        /// <param name="urls">下载源URL列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>按速度排序的URL列表（最快的在前）</returns>
        private async Task<List<string>> TestAndSortUrlsBySpeedAsync(List<string> urls,
            CancellationToken cancellationToken)
        {
            List<(string Url, long Speed, bool Success)> results = new List<(string, long, bool)>();

            // 并行测试所有源的速度
            List<Task<(string Url, long Speed, bool Success)>> testTasks = urls
                .Select(url => TestDownloadSpeedAsync(url, cancellationToken))
                .ToList();

            try
            {
                (string Url, long Speed, bool Success)[] testResults = await Task.WhenAll(testTasks);
                results.AddRange(testResults);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                // 如果测速失败，返回原始顺序
                return urls;
            }

            // 报告测速结果
            foreach (var result in results.OrderByDescending(r => r.Speed))
            {
                string host = GetHostFromUrl(result.Url);
                if (result.Success)
                {
                    ReportProgress(0, 0, result.Speed, result.Url,
                        $"测速完成: {host} - {FormatSpeed(result.Speed)}");
                }
                else
                {
                    ReportProgress(0, 0, 0, result.Url,
                        $"测速完成: {host} - 连接失败");
                }
            }

            // 按速度排序，成功的在前，速度快的在前
            List<string> sortedUrls = results
                .OrderByDescending(r => r.Success)
                .ThenByDescending(r => r.Speed)
                .Select(r => r.Url)
                .ToList();

            // 报告选择的最快源
            if (sortedUrls.Count > 0)
            {
                var fastest = results.Where(r => r.Success).OrderByDescending(r => r.Speed).FirstOrDefault();
                if (fastest.Success)
                {
                    ReportProgress(0, 0, fastest.Speed, fastest.Url,
                        $"已选择最快源: {GetHostFromUrl(fastest.Url)} ({FormatSpeed(fastest.Speed)})");
                }
            }

            return sortedUrls;
        }

        /// <summary>
        /// 测试单个URL的下载速度
        /// </summary>
        /// <param name="url">下载URL</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>URL和测得的速度（字节/秒）</returns>
        private async Task<(string Url, long Speed, bool Success)> TestDownloadSpeedAsync(string url,
            CancellationToken cancellationToken)
        {
            try
            {
                using CancellationTokenSource timeoutCts = new CancellationTokenSource(
                    TimeSpan.FromSeconds(SPEED_TEST_TIMEOUT_SECONDS));
                using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken, timeoutCts.Token);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                // 只请求前256KB用于测速
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, SPEED_TEST_BYTES - 1);

                DateTime startTime = DateTime.Now;
                long bytesReceived = 0;

                using HttpResponseMessage response = await _httpClient.SendAsync(request,
                    HttpCompletionOption.ResponseHeadersRead, linkedCts.Token);

                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.PartialContent)
                {
                    return (url, 0, false);
                }

                using Stream contentStream = await response.Content.ReadAsStreamAsync(linkedCts.Token);
                byte[] buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, linkedCts.Token)) > 0)
                {
                    bytesReceived += bytesRead;
                    if (bytesReceived >= SPEED_TEST_BYTES)
                    {
                        break;
                    }
                }

                double elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
                long speed = elapsedSeconds > 0 ? (long)(bytesReceived / elapsedSeconds) : 0;

                return (url, speed, true);
            }
            catch
            {
                return (url, 0, false);
            }
        }
    }
}
