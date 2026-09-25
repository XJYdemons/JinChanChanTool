using JinChanChanTool.DataClass;
using JinChanChanTool.Services.Network;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// GitHub 发布信息客户端。
    /// 负责读取 Resources/UpdateSources.json 中的仓库与镜像配置，
    /// 依次尝试各镜像地址获取最新发布信息，并按规则挑选主程序包资产。
    /// </summary>
    public class GitHubReleaseClient
    {
        /// <summary>配置文件相对安装目录的路径</summary>
        private const string ConfigRelativePath = "Resources/UpdateSources.json";

        /// <summary>默认仓库所有者</summary>
        private const string DefaultOwner = "XJYdemons";

        /// <summary>默认仓库名称</summary>
        private const string DefaultRepository = "JinChanChanTool";

        /// <summary>默认 API 基地址</summary>
        private const string DefaultApiBaseUrl = "https://api.github.com";

        /// <summary>默认请求超时（秒）</summary>
        private const int DefaultTimeoutSeconds = 20;

        /// <summary>默认的主程序包文件名匹配规则</summary>
        private const string DefaultPackageNamePattern =
            @"^JinChanChanTool_v(?<version>[0-9]+\.[0-9]+\.[0-9]+)_Windows_x64\.zip$";

        /// <summary>默认的资产排除关键字</summary>
        private const string DefaultExcludedNameKeyword = "SourceCode";

        /// <summary>GitHub API 要求的固定版本请求头</summary>
        private const string GitHubApiVersion = "2022-11-28";

        /// <summary>User-Agent 中使用的应用标识</summary>
        private const string UserAgentProduct = "JinChanChanTool-Updater";

        /// <summary>GitHub 响应中需要忽略大小写匹配的 JSON 选项</summary>
        private static readonly JsonSerializerOptions ReleaseJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        private readonly string _owner;
        private readonly string _repository;
        private readonly IReadOnlyList<string> _apiBaseUrls;
        private readonly IReadOnlyList<string> _downloadMirrorPrefixes;
        private readonly int _timeoutSeconds;
        private readonly string _packageNamePattern;
        private readonly string _excludedNameKeyword;

        /// <summary>
        /// 使用 Resources/UpdateSources.json 中的配置创建客户端。
        /// </summary>
        public GitHubReleaseClient()
        {
            JObject? config = LoadConfig();
            JObject? gitHubSources = config?["GitHubSources"] as JObject;
            JObject? updatePolicy = config?["UpdatePolicy"] as JObject;

            _owner = ReadString(gitHubSources, "Owner", DefaultOwner);
            _repository = ReadString(gitHubSources, "Repository", DefaultRepository);
            _apiBaseUrls = ReadStringList(gitHubSources, "ApiBaseUrls", new[] { DefaultApiBaseUrl });
            _downloadMirrorPrefixes = ReadStringList(
                gitHubSources,
                "DownloadMirrorPrefixes",
                new[] { string.Empty });
            _timeoutSeconds = ReadInt(gitHubSources, "RequestTimeoutSeconds", DefaultTimeoutSeconds);
            _packageNamePattern = ReadString(updatePolicy, "PackageNamePattern", DefaultPackageNamePattern);
            _excludedNameKeyword = ReadString(updatePolicy, "ExcludedNamePattern", DefaultExcludedNameKeyword);
        }

        /// <summary>
        /// 获取仓库最新发布信息。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>发布信息；全部镜像失败时返回 null</returns>
        public async Task<UpdateReleaseInfo?> GetLatestReleaseAsync(CancellationToken cancellationToken = default)
        {
            foreach (string apiBaseUrl in _apiBaseUrls)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string normalizedBaseUrl = apiBaseUrl.TrimEnd('/');
                string requestUrl = $"{normalizedBaseUrl}/repos/{_owner}/{_repository}/releases/latest";

                try
                {
                    string json = await GetStringAsync(requestUrl, cancellationToken).ConfigureAwait(false);

                    // 使用 System.Text.Json 反序列化：DTO 上的 JsonPropertyName 特性与之配套，
                    // 换成 Newtonsoft 会导致 tag_name 等字段映射不到。
                    UpdateReleaseInfo? release = JsonSerializer.Deserialize<UpdateReleaseInfo>(json, ReleaseJsonOptions);

                    if (release != null && !string.IsNullOrWhiteSpace(release.TagName))
                    {
                        return release;
                    }

                    LogTool.Log($"[GitHubReleaseClient] 镜像返回内容无法解析：{normalizedBaseUrl}");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // 单个镜像失败不影响后续镜像，继续尝试
                    LogTool.Log($"[GitHubReleaseClient] 检查更新失败（{normalizedBaseUrl}）：{ex.Message}");
                    Debug.WriteLine($"[GitHubReleaseClient] 检查更新失败（{normalizedBaseUrl}）：{ex.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// 在发布信息中挑选主程序包资产（排除源码包等非更新包）。
        /// </summary>
        /// <param name="release">发布信息</param>
        /// <returns>匹配到的资产；无匹配时返回 null</returns>
        public UpdateAssetInfo? SelectPackageAsset(UpdateReleaseInfo? release)
        {
            if (release?.Assets == null || release.Assets.Count == 0)
            {
                return null;
            }

            Regex pattern;
            try
            {
                pattern = new Regex(_packageNamePattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
            catch (ArgumentException ex)
            {
                // 配置中的正则非法时退回到内置规则，避免因配置错误导致更新不可用
                LogTool.Log($"[GitHubReleaseClient] 更新包匹配规则非法，已使用内置规则：{ex.Message}");
                pattern = new Regex(DefaultPackageNamePattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }

            foreach (UpdateAssetInfo asset in release.Assets)
            {
                if (string.IsNullOrWhiteSpace(asset.Name) ||
                    string.IsNullOrWhiteSpace(asset.BrowserDownloadUrl))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(_excludedNameKeyword) &&
                    asset.Name.Contains(_excludedNameKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (pattern.IsMatch(asset.Name))
                {
                    return asset;
                }
            }

            return null;
        }

        /// <summary>
        /// 为一个直链生成按优先级排列的候选下载地址（原始地址 + 各镜像前缀）。
        /// </summary>
        /// <param name="downloadUrl">GitHub 原始下载地址</param>
        /// <returns>候选地址列表，已去重</returns>
        public List<string> BuildDownloadUrls(string downloadUrl)
        {
            List<string> urls = new List<string>();

            if (string.IsNullOrWhiteSpace(downloadUrl))
            {
                return urls;
            }

            foreach (string prefix in _downloadMirrorPrefixes)
            {
                string candidate = string.IsNullOrEmpty(prefix) ? downloadUrl : prefix.TrimEnd('/') + "/" + downloadUrl;
                if (!urls.Contains(candidate, StringComparer.OrdinalIgnoreCase))
                {
                    urls.Add(candidate);
                }
            }

            // 配置未提供任何前缀时，至少保证原始地址可用
            if (urls.Count == 0)
            {
                urls.Add(downloadUrl);
            }

            return urls;
        }

        /// <summary>
        /// 读取字符串配置项。
        /// </summary>
        private static string ReadString(JObject? source, string propertyName, string defaultValue)
        {
            string? value = source?[propertyName]?.ToString();
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        /// <summary>
        /// 读取整型配置项，非法值回退到默认值。
        /// </summary>
        private static int ReadInt(JObject? source, string propertyName, int defaultValue)
        {
            JToken? token = source?[propertyName];
            if (token == null)
            {
                return defaultValue;
            }

            try
            {
                int value = token.Value<int>();
                return value > 0 ? value : defaultValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GitHubReleaseClient] 配置项 {propertyName} 解析失败：{ex.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 读取字符串数组配置项，空数组回退到默认值。
        /// </summary>
        private static IReadOnlyList<string> ReadStringList(
            JObject? source,
            string propertyName,
            IReadOnlyList<string> defaultValue)
        {
            if (source?[propertyName] is not JArray array)
            {
                return defaultValue;
            }

            List<string> values = array
                .Select(item => item.ToString())
                .Where(item => item != null)
                .Select(item => item!)
                .ToList();

            return values.Count == 0 ? defaultValue : values;
        }

        /// <summary>
        /// 读取 Resources/UpdateSources.json，失败时返回 null（由各配置项使用默认值）。
        /// </summary>
        private static JObject? LoadConfig()
        {
            try
            {
                string configPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    ConfigRelativePath.Replace('/', Path.DirectorySeparatorChar));

                if (!File.Exists(configPath))
                {
                    LogTool.Log($"[GitHubReleaseClient] 未找到更新源配置，使用内置默认值：{configPath}");
                    return null;
                }

                return JObject.Parse(File.ReadAllText(configPath));
            }
            catch (Exception ex)
            {
                LogTool.Log($"[GitHubReleaseClient] 读取更新源配置失败，使用内置默认值：{ex.Message}");
                Debug.WriteLine($"[GitHubReleaseClient] 读取更新源配置失败：{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 发起一次 GET 请求并返回响应文本。
        /// 使用独立 HttpClient：更新检查不应携带业务接口的签名头，避免请求被拒。
        /// </summary>
        private async Task<string> GetStringAsync(string requestUrl, CancellationToken cancellationToken)
        {
            using HttpClientHandler handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10,
                AutomaticDecompression = DecompressionMethods.All
            };

            using HttpClient client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(_timeoutSeconds)
            };

            client.DefaultRequestHeaders.Add("User-Agent", $"{UserAgentProduct}/{DataClass.StaticData.ProgramVersion.VersionText}");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", GitHubApiVersion);

            using HttpResponseMessage response = await client
                .GetAsync(requestUrl, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 网络请求所用的共享 HttpClient（供下载环节复用同一连接池配置）。
        /// </summary>
        public static HttpClient CreateHttpClient(int timeoutSeconds = DefaultTimeoutSeconds)
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10,
                AutomaticDecompression = DecompressionMethods.All
            };

            HttpClient client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };

            client.DefaultRequestHeaders.Add("User-Agent", $"{UserAgentProduct}/{DataClass.StaticData.ProgramVersion.VersionText}");
            return client;
        }
    }
}
