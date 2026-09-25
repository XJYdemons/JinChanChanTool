using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// 更新路径策略：负责判断哪些路径属于“用户态数据”（更新时保留、只做增量合并），
    /// 以及校验更新清单中的相对路径是否安全（防止路径穿越写到安装目录之外）。
    /// 保护清单来自 Resources/UpdateSources.json，未配置时使用内置默认清单。
    /// </summary>
    public class UpdatePathPolicy
    {
        /// <summary>配置文件相对安装目录的路径</summary>
        private const string ConfigRelativePath = "Resources/UpdateSources.json";

        /// <summary>单个路径段通配符：可匹配任意一个目录名（例如 {season}）</summary>
        private const string SegmentWildcard = "{season}";

        /// <summary>多段通配符：匹配任意层级的剩余路径</summary>
        private const string RemainingWildcard = "**";

        /// <summary>内置的用户态文件保护清单（配置缺失时兜底）</summary>
        private static readonly string[] DefaultUserStateFiles =
        {
            "Resources/ManualSettings.json",
            "Resources/AutomaticSettings.json",
            "Resources/CorrectionsList.json",
            "Resources/HeroDatas/{season}/LineUps.json",
            "Resources/HeroDatas/{season}/EquipmentData.json",
            "Resources/HeroDatas/{season}/RecommendedLineUps.json",
            "Resources/HeroDatas/{season}/LineUpCodeDictionary.json"
        };

        /// <summary>内置的用户态目录保护清单（配置缺失时兜底）</summary>
        private static readonly string[] DefaultUserStateDirectories =
        {
            "Logs",
            "Downloads"
        };

        /// <summary>
        /// 用户态文件保护清单的加载自检。
        /// 配置里的路径一旦写错，代码会静默退回到内置清单；此处显式记录一条日志，
        /// 便于在排查“升级后配置被覆盖”时第一时间确认保护清单是否真正生效。
        /// </summary>
        static UpdatePathPolicy()
        {
            try
            {
                JObject? config = LoadConfig();
                JObject? updatePolicy = config?["UpdatePolicy"] as JObject;
                JArray? configuredFiles = updatePolicy?["UserStateFiles"] as JArray;

                if (config == null)
                {
                    LogTool.Log("[UpdatePathPolicy] 未找到更新源配置，用户态保护清单使用内置默认值。");
                }
                else if (configuredFiles == null || configuredFiles.Count == 0)
                {
                    LogTool.Log("[UpdatePathPolicy] 更新源配置中缺少 UserStateFiles，用户态保护清单使用内置默认值。");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdatePathPolicy] 保护清单自检失败：{ex.Message}");
            }
        }

        private readonly List<string> _userStateFilePatterns;
        private readonly List<string> _userStateDirectoryPatterns;

        /// <summary>
        /// 使用 Resources/UpdateSources.json 中的保护清单创建策略对象。
        /// </summary>
        public UpdatePathPolicy()
        {
            JObject? config = LoadConfig();
            JObject? updatePolicy = config?["UpdatePolicy"] as JObject;

            _userStateFilePatterns = MergePatterns(
                ReadStringList(updatePolicy, "UserStateFiles"),
                DefaultUserStateFiles);

            _userStateDirectoryPatterns = MergePatterns(
                ReadStringList(updatePolicy, "UserStateDirectories"),
                DefaultUserStateDirectories);
        }

        /// <summary>
        /// 判断相对路径是否为用户态数据（更新包内的同名文件必须被跳过）。
        /// </summary>
        /// <param name="relativePath">相对安装目录的路径（/ 分隔）</param>
        /// <returns>是否属于用户态数据</returns>
        public bool IsUserStatePath(string relativePath)
        {
            string normalized = NormalizeRelativePath(relativePath);
            if (normalized.Length == 0)
            {
                return false;
            }

            foreach (string pattern in _userStateFilePatterns)
            {
                if (MatchesPattern(normalized, pattern))
                {
                    return true;
                }
            }

            foreach (string directory in _userStateDirectoryPatterns)
            {
                string normalizedDirectory = NormalizeRelativePath(directory);
                if (normalizedDirectory.Length == 0)
                {
                    continue;
                }

                // 目录保护同时覆盖目录本身与其下所有内容
                if (normalized.Equals(normalizedDirectory, StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(normalizedDirectory + UpdateConstants.RelativePathSeparator, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断相对路径是否落在安装目录内部（防止更新包通过 ../ 写出目录）。
        /// </summary>
        /// <param name="relativePath">相对安装目录的路径</param>
        /// <returns>路径是否安全</returns>
        public bool IsSafeRelativePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return false;
            }

            // 绝对路径、盘符路径、UNC 路径一律拒绝
            if (Path.IsPathRooted(relativePath) ||
                relativePath.StartsWith(@"\\", StringComparison.Ordinal) ||
                relativePath.Contains(':'))
            {
                return false;
            }

            string[] segments = relativePath.Split(
                new[] { UpdateConstants.RelativePathSeparator, '\\' },
                StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                return false;
            }

            foreach (string segment in segments)
            {
                if (segment == ".." || segment == ".")
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 把相对路径规范化为 / 分隔、去掉首尾分隔符的形式。
        /// </summary>
        /// <param name="relativePath">原始相对路径</param>
        /// <returns>规范化后的路径</returns>
        public static string NormalizeRelativePath(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return string.Empty;
            }

            return relativePath
                .Replace('\\', UpdateConstants.RelativePathSeparator)
                .Trim(UpdateConstants.RelativePathSeparator)
                .Trim();
        }

        /// <summary>
        /// 按保护规则匹配路径：支持 {season} 单段通配与 ** 多段通配。
        /// </summary>
        private static bool MatchesPattern(string normalizedPath, string pattern)
        {
            string normalizedPattern = NormalizeRelativePath(pattern);
            if (normalizedPattern.Length == 0)
            {
                return false;
            }

            if (normalizedPattern.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (normalizedPattern.EndsWith(RemainingWildcard, StringComparison.Ordinal))
            {
                string prefix = normalizedPattern[..^RemainingWildcard.Length]
                    .TrimEnd(UpdateConstants.RelativePathSeparator);
                return normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }

            string[] patternSegments = normalizedPattern.Split(UpdateConstants.RelativePathSeparator);
            string[] pathSegments = normalizedPath.Split(UpdateConstants.RelativePathSeparator);

            if (patternSegments.Length != pathSegments.Length)
            {
                return false;
            }

            for (int i = 0; i < patternSegments.Length; i++)
            {
                string patternSegment = patternSegments[i];
                if (patternSegment == SegmentWildcard)
                {
                    continue;
                }

                if (!patternSegment.Equals(pathSegments[i], StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 读取配置中的字符串数组，读取失败时返回空列表。
        /// </summary>
        private static List<string> ReadStringList(JObject? source, string propertyName)
        {
            if (source?[propertyName] is not JArray array)
            {
                return new List<string>();
            }

            return array
                .Select(item => item.ToString())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item!)
                .ToList();
        }

        /// <summary>
        /// 合并配置清单与内置清单，去重后返回。
        /// </summary>
        private static List<string> MergePatterns(List<string> configured, IReadOnlyList<string> defaults)
        {
            List<string> merged = new List<string>();

            foreach (string item in configured.Concat(defaults))
            {
                string normalized = NormalizeRelativePath(item);
                if (normalized.Length == 0)
                {
                    continue;
                }

                if (!merged.Contains(normalized, StringComparer.OrdinalIgnoreCase))
                {
                    merged.Add(normalized);
                }
            }

            return merged;
        }

        /// <summary>
        /// 读取 Resources/UpdateSources.json，失败时返回 null（由内置清单兜底）。
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
                    return null;
                }

                return JObject.Parse(File.ReadAllText(configPath));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdatePathPolicy] 读取保护清单失败，使用内置清单：{ex.Message}");
                LogTool.Log($"[UpdatePathPolicy] 读取保护清单失败，使用内置清单：{ex.Message}");
                return null;
            }
        }
    }
}
