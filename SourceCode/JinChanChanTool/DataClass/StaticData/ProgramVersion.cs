using System.Reflection;

namespace JinChanChanTool.DataClass.StaticData
{
    /// <summary>
    /// 程序版本的单一来源。
    /// 版本号来自项目文件中的 &lt;Version&gt;，构建日期来自程序集元数据 BuildDate，
    /// 因此关于窗口、更新检查等所有位置都读取同一份数据，不再各自硬编码。
    /// </summary>
    public static class ProgramVersion
    {
        /// <summary>版本号解析失败时使用的兜底文本。</summary>
        private const string FallbackVersionText = "0.0.0";

        /// <summary>构建日期缺失时使用的兜底文本。</summary>
        private const string FallbackBuildDateText = "0000.00.00";

        /// <summary>程序集元数据中记录构建日期的键名（与项目文件中的 AssemblyMetadata 对应）。</summary>
        private const string BuildDateMetadataKey = "BuildDate";

        /// <summary>版本号展示时保留的段数（主版本.次版本.修订号）。</summary>
        private const int DisplayVersionFieldCount = 3;

        /// <summary>预发布版本后缀的分隔符（如 7.8.0-beta 中的 "-"）。</summary>
        private const char PrereleaseSeparator = '-';

        /// <summary>构建元数据后缀的分隔符（如 7.8.0+build 中的 "+"），不属于预发布。</summary>
        private const char BuildMetadataSeparator = '+';

        /// <summary>解析后的当前程序版本（纯数字，用于比较），解析失败时为 0.0.0。</summary>
        private static readonly Version ParsedVersion = ResolveVersion();

        /// <summary>原始版本文本，保留项目文件中填写的完整内容（含预发布后缀）。</summary>
        private static readonly string RawVersionTextValue = ResolveRawVersionText();

        /// <summary>解析后的构建日期文本。</summary>
        private static readonly string ResolvedBuildDateText = ResolveBuildDateText();

        /// <summary>
        /// 当前程序版本（纯数字，可用于比较）。
        /// </summary>
        public static Version Current => ParsedVersion;

        /// <summary>
        /// 纯数字版本文本，形如 7.7.0；预发布后缀会被剥离。
        /// 用于版本号比较与发布标签拼接，不用于界面展示。
        /// </summary>
        public static string VersionText => ParsedVersion.ToString(DisplayVersionFieldCount);

        /// <summary>
        /// 项目文件中填写的原始版本文本，形如 7.7.0 或 7.8.0-beta。
        /// 界面展示使用此值，做到“填什么显示什么”。
        /// </summary>
        public static string RawVersionText => RawVersionTextValue;

        /// <summary>
        /// 当前是否为预发布版本（版本号带 "-" 后缀，如 7.8.0-beta）。
        /// 预发布版本不参与自动更新。
        /// </summary>
        public static bool IsPrerelease => ContainsPrereleaseSuffix(RawVersionTextValue);

        /// <summary>
        /// 带 v 前缀的版本标签，形如 v7.7.0，用于与 GitHub 的 tag 比较。
        /// </summary>
        public static string TagText => "v" + VersionText;

        /// <summary>
        /// 构建日期文本，形如 2026.09.23。
        /// </summary>
        public static string BuildDateText => ResolvedBuildDateText;

        /// <summary>
        /// 关于窗口使用的完整展示文本，形如 7.7.0(2026.09.23) 或 7.8.0-beta(2026.09.27)。
        /// </summary>
        public static string DisplayText => $"{RawVersionTextValue}({BuildDateText})";

        /// <summary>
        /// 判断版本文本是否带有预发布后缀。
        /// 仅以 "-" 判定：SemVer 中 "+" 之后属于构建元数据，不代表预发布。
        /// </summary>
        /// <param name="versionText">版本文本</param>
        /// <returns>是否为预发布版本</returns>
        public static bool ContainsPrereleaseSuffix(string? versionText)
        {
            if (string.IsNullOrWhiteSpace(versionText))
            {
                return false;
            }

            string normalized = versionText.Trim();
            int prereleaseIndex = normalized.IndexOf(PrereleaseSeparator);
            if (prereleaseIndex < 0)
            {
                return false;
            }

            // 排除构建元数据形式（如 7.8.0+build-x）："-" 出现在 "+" 之后不算预发布
            int buildMetadataIndex = normalized.IndexOf(BuildMetadataSeparator);
            if (buildMetadataIndex >= 0 && prereleaseIndex > buildMetadataIndex)
            {
                return false;
            }

            // "-" 之后必须有内容才算预发布后缀
            return prereleaseIndex < normalized.Length - 1;
        }

        /// <summary>
        /// 从程序集元数据读取原始版本文本（保留预发布后缀）。
        /// 顺序：InformationalVersion → 纯数字程序集版本 → 兜底文本。
        /// </summary>
        private static string ResolveRawVersionText()
        {
            try
            {
                string? informational = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

                if (!string.IsNullOrWhiteSpace(informational))
                {
                    // InformationalVersion 可能带 "+提交号"，截掉它但保留 "-预发布" 后缀
                    string normalized = TrimBuildMetadata(informational.Trim());
                    if (normalized.Length > 0)
                    {
                        return normalized;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProgramVersion] 读取原始版本文本失败：{ex.Message}");
            }

            // 回退到已解析的纯数字版本，保证界面始终有可展示的内容
            return ParsedVersion.ToString(DisplayVersionFieldCount);
        }

        /// <summary>
        /// 截掉构建元数据后缀（"+" 及其之后的内容），保留预发布后缀。
        /// </summary>
        /// <param name="versionText">版本文本</param>
        /// <returns>去掉构建元数据后的文本</returns>
        private static string TrimBuildMetadata(string versionText)
        {
            int buildMetadataIndex = versionText.IndexOf(BuildMetadataSeparator);
            return buildMetadataIndex >= 0 ? versionText[..buildMetadataIndex] : versionText;
        }

        /// <summary>
        /// 把发布标签（如 "v7.7.0"、"7.7"、"v7.8.0-beta"）解析为可比较的版本对象。
        /// 预发布后缀会被剥离，因此 v7.8.0-beta 解析结果与 v7.8.0 相同。
        /// 调用方若需区分预发布，应另行检查标签文本。
        /// </summary>
        /// <param name="tag">发布标签文本</param>
        /// <param name="version">解析结果</param>
        /// <returns>是否解析成功</returns>
        public static bool TryParseTag(string? tag, out Version version)
        {
            version = new Version(0, 0);
            if (string.IsNullOrWhiteSpace(tag))
            {
                return false;
            }

            // 去掉 v 前缀，并截掉可能存在的预发布后缀（-beta）与构建元数据后缀（+abc123）
            string normalized = tag.Trim().TrimStart('v', 'V');
            int suffixIndex = normalized.IndexOfAny(new[] { PrereleaseSeparator, BuildMetadataSeparator });
            if (suffixIndex >= 0)
            {
                normalized = normalized[..suffixIndex];
            }

            if (!Version.TryParse(normalized, out Version? parsed) || parsed == null)
            {
                return false;
            }

            // 统一补齐到三段，保证 7.7 与 7.7.0 视为同一版本
            version = new Version(parsed.Major, parsed.Minor, Math.Max(parsed.Build, 0));
            return true;
        }

        /// <summary>
        /// 从程序集元数据读取版本号，失败时回退到兜底文本。
        /// </summary>
        private static Version ResolveVersion()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                // InformationalVersion 可能带有 "+提交号" 后缀，交由 TryParseTag 统一清理
                string? informational = assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

                if (TryParseTag(informational, out Version fromInformational))
                {
                    return fromInformational;
                }

                Version? assemblyVersion = assembly.GetName().Version;
                if (assemblyVersion != null)
                {
                    return new Version(
                        assemblyVersion.Major,
                        assemblyVersion.Minor,
                        Math.Max(assemblyVersion.Build, 0));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProgramVersion] 读取程序集版本失败：{ex.Message}");
            }

            return Version.Parse(FallbackVersionText);
        }

        /// <summary>
        /// 读取构建日期：优先使用程序集元数据，缺失时回退到可执行文件最后写入时间。
        /// </summary>
        private static string ResolveBuildDateText()
        {
            try
            {
                AssemblyMetadataAttribute? metadata = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes<AssemblyMetadataAttribute>()
                    .FirstOrDefault(attribute =>
                        string.Equals(attribute.Key, BuildDateMetadataKey, StringComparison.OrdinalIgnoreCase));

                if (metadata != null && !string.IsNullOrWhiteSpace(metadata.Value))
                {
                    return metadata.Value.Trim();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProgramVersion] 读取构建日期元数据失败：{ex.Message}");
            }

            try
            {
                // 兜底：使用主程序集文件的最后写入时间（发布包的构建日期即文件时间）
                string? assemblyPath = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(assemblyPath) && File.Exists(assemblyPath))
                {
                    return File.GetLastWriteTime(assemblyPath).ToString("yyyy.MM.dd");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProgramVersion] 读取主程序集文件时间失败：{ex.Message}");
            }

            return FallbackBuildDateText;
        }
    }
}
