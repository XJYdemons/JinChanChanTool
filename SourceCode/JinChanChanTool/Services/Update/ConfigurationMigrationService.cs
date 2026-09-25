using JinChanChanTool.DataClass;
using JinChanChanTool.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json;

namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// 配置增量合并服务。
    /// 目标：升级后保留用户的全部自定义内容，只把“新版本新增的设置项/数据条目”补进去。
    /// 规则：
    /// 1. 对象：新版本有的键而用户文件没有 → 补充；用户已有的键 → 一律保持原值；
    /// 2. 对象数组：按主键（Correct/Name/Id 等）匹配，用户缺少的条目才追加；已存在的条目只递归补齐缺失字段；
    /// 3. 用户文件损坏或解析失败 → 跳过并记录日志，绝不以默认值覆盖用户文件；
    /// 4. 每次合并前先备份为 xxx.bak_{版本}。
    /// </summary>
    public class ConfigurationMigrationService
    {
        /// <summary>设置文件相对安装目录的路径</summary>
        private const string ManualSettingsRelativePath = "Resources/ManualSettings.json";

        /// <summary>自动设置文件相对安装目录的路径</summary>
        private const string AutomaticSettingsRelativePath = "Resources/AutomaticSettings.json";

        /// <summary>OCR 纠正列表相对安装目录的路径</summary>
        private const string CorrectionsRelativePath = "Resources/CorrectionsList.json";

        /// <summary>数组条目匹配时依次尝试的主键名</summary>
        private static readonly string[] ArrayItemKeyCandidates =
        {
            "Correct",
            "Name",
            "HeroName",
            "EquipmentName",
            "Id",
            "Key"
        };

        /// <summary>
        /// 合并时忽略的派生只读键。
        /// Rectangle/Point 被序列化时会带出 Location、Size、Left、Top、Right、Bottom、IsEmpty 等只读成员，
        /// 这些成员在反序列化时不会被读取；把它们补进用户文件只会让配置变脏，因此一律跳过。
        /// </summary>
        private static readonly HashSet<string> IgnoredDerivedKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "IsEmpty",
            "Location",
            "Size",
            "Left",
            "Top",
            "Right",
            "Bottom"
        };

        /// <summary>每个用户配置文件保留的历史备份数量</summary>
        private const int KeepBackupCountPerFile = 3;

        /// <summary>备份文件名的前缀（附在完整文件名之后）</summary>
        private const string BackupInfix = ".bak_";

        private readonly string _installDirectory;

        /// <summary>
        /// 创建配置增量合并服务。
        /// </summary>
        /// <param name="installDirectory">程序安装目录</param>
        public ConfigurationMigrationService(string installDirectory)
        {
            _installDirectory = installDirectory;
        }

        /// <summary>
        /// 执行一次完整的增量合并：设置文件用当前版本的默认值合并，数据文件用更新包快照合并。
        /// </summary>
        /// <param name="defaultsSnapshotDirectory">更新包中用户态文件的默认值快照目录，可为空</param>
        /// <param name="versionText">当前版本号文本，用于备份文件命名</param>
        /// <returns>合并结果</returns>
        public ConfigurationMigrationResult Apply(string? defaultsSnapshotDirectory, string versionText)
        {
            ConfigurationMigrationResult result = new ConfigurationMigrationResult();

            try
            {
                // 设置文件：默认值来自当前版本的代码，始终可用
                MergeSettingsFile(
                    Path.Combine(_installDirectory, ManualSettingsRelativePath),
                    CreateManualSettingsDefaultsJson(),
                    versionText,
                    result);

                MergeSettingsFile(
                    Path.Combine(_installDirectory, AutomaticSettingsRelativePath),
                    CreateAutomaticSettingsDefaultsJson(),
                    versionText,
                    result);

                // 纠正列表：默认值来自更新包快照（用户态文件不会被更新覆盖，因此必须单独保留一份默认值）
                string snapshotCorrectionsPath = Path.Combine(
                    defaultsSnapshotDirectory ?? string.Empty,
                    CorrectionsRelativePath.Replace('/', Path.DirectorySeparatorChar));

                if (!string.IsNullOrEmpty(defaultsSnapshotDirectory) && File.Exists(snapshotCorrectionsPath))
                {
                    MergeListFile(
                        Path.Combine(_installDirectory, CorrectionsRelativePath),
                        snapshotCorrectionsPath,
                        versionText,
                        result);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                LogTool.Log($"[ConfigurationMigrationService] 配置增量合并失败：{ex.Message}");
                Debug.WriteLine($"[ConfigurationMigrationService] 配置增量合并失败：{ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// 合并一个设置文件：把默认 JSON 中用户缺失的成员补进去。
        /// </summary>
        private void MergeSettingsFile(
            string userFilePath,
            JObject defaults,
            string versionText,
            ConfigurationMigrationResult result)
        {
            if (!File.Exists(userFilePath))
            {
                // 用户文件不存在（首次运行）由各自的设置服务负责创建，这里不介入
                return;
            }

            MergeFileCore(userFilePath, defaults, versionText, result);
        }

        /// <summary>
        /// 合并一个列表文件：以默认值文件为模板，补齐用户缺少的条目与字段。
        /// </summary>
        private void MergeListFile(
            string userFilePath,
            string defaultsFilePath,
            string versionText,
            ConfigurationMigrationResult result)
        {
            try
            {
                if (!File.Exists(userFilePath))
                {
                    return;
                }

                JToken defaults = JToken.Parse(File.ReadAllText(defaultsFilePath));
                MergeFileCore(userFilePath, defaults, versionText, result);
            }
            catch (Exception ex)
            {
                LogTool.Log($"[ConfigurationMigrationService] 读取默认值文件失败（{defaultsFilePath}）：{ex.Message}");
                Debug.WriteLine($"[ConfigurationMigrationService] 读取默认值文件失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 合并核心逻辑：读取用户文件 → 补齐缺失成员 → 有变化才写回（写回前备份）。
        /// </summary>
        private void MergeFileCore(
            string userFilePath,
            JToken defaults,
            string versionText,
            ConfigurationMigrationResult result)
        {
            JToken? userRoot;

            try
            {
                string userJson = File.ReadAllText(userFilePath);
                if (string.IsNullOrWhiteSpace(userJson))
                {
                    result.SkippedFiles.Add(userFilePath);
                    LogTool.Log($"[ConfigurationMigrationService] 用户文件为空，已跳过合并：{userFilePath}");
                    return;
                }

                userRoot = JToken.Parse(userJson);
            }
            catch (Exception ex)
            {
                // 用户文件损坏时不覆盖、不删除，交由原有服务按既有逻辑处理
                result.SkippedFiles.Add(userFilePath);
                LogTool.Log($"[ConfigurationMigrationService] 用户文件解析失败，已跳过合并：{userFilePath}，{ex.Message}");
                Debug.WriteLine($"[ConfigurationMigrationService] 用户文件解析失败：{ex.Message}");
                return;
            }

            List<string> addedMembers = new List<string>();
            MergeToken(userRoot, defaults, string.Empty, addedMembers);

            if (addedMembers.Count == 0)
            {
                return;
            }

            BackupUserFile(userFilePath, versionText);

            string mergedJson = userRoot.ToString(Formatting.Indented);
            File.WriteAllText(userFilePath, mergedJson);

            result.MergedFiles.Add(userFilePath);
            result.AddedMembers.AddRange(addedMembers);

            LogTool.Log(
                $"[ConfigurationMigrationService] 已增量合并 {Path.GetFileName(userFilePath)}，" +
                $"新增 {addedMembers.Count} 项：{string.Join("、", addedMembers)}");
        }

        /// <summary>
        /// 递归合并：只补默认值中存在而用户数据中缺失的内容。
        /// </summary>
        /// <param name="userToken">用户数据（会被就地修改）</param>
        /// <param name="defaultToken">默认数据</param>
        /// <param name="pathPrefix">当前路径前缀，用于生成可读的变更项名称</param>
        /// <param name="addedMembers">收集新增项的列表</param>
        private static void MergeToken(
            JToken userToken,
            JToken defaultToken,
            string pathPrefix,
            List<string> addedMembers)
        {
            if (userToken is JObject userObject && defaultToken is JObject defaultObject)
            {
                foreach (JProperty defaultProperty in defaultObject.Properties())
                {
                    // 派生只读成员不参与合并（补了也不会被反序列化读取）
                    if (IgnoredDerivedKeys.Contains(defaultProperty.Name))
                    {
                        continue;
                    }

                    string childPath = string.IsNullOrEmpty(pathPrefix)
                        ? defaultProperty.Name
                        : $"{pathPrefix}.{defaultProperty.Name}";

                    JProperty? userProperty = userObject.Property(defaultProperty.Name);

                    if (userProperty == null)
                    {
                        // 用户文件缺少该字段：补上默认值（先剔除派生只读成员，避免污染配置文件）
                        JToken defaultValue = defaultProperty.Value.DeepClone();
                        RemoveIgnoredDerivedMembers(defaultValue);

                        userObject.Add(defaultProperty.Name, defaultValue);
                        addedMembers.Add(childPath);
                        continue;
                    }

                    // 字段已存在：只在两边都是对象/数组时递归补齐，标量值一律保留用户设置
                    MergeToken(userProperty.Value, defaultProperty.Value, childPath, addedMembers);
                }

                return;
            }

            if (userToken is JArray userArray && defaultToken is JArray defaultArray)
            {
                MergeArray(userArray, defaultArray, pathPrefix, addedMembers);
            }
        }

        /// <summary>
        /// 递归移除派生只读成员（Rectangle/Point 的 Location、Size、Left、Top、Right、Bottom、IsEmpty）。
        /// 这些成员在反序列化时不会被读取，写进用户配置只会造成配置膨胀与误判。
        /// </summary>
        /// <param name="token">待清理的 JSON 节点（就地修改）</param>
        private static void RemoveIgnoredDerivedMembers(JToken token)
        {
            if (token is JObject jsonObject)
            {
                foreach (JProperty property in jsonObject.Properties().ToList())
                {
                    if (IgnoredDerivedKeys.Contains(property.Name))
                    {
                        property.Remove();
                        continue;
                    }

                    RemoveIgnoredDerivedMembers(property.Value);
                }

                return;
            }

            if (token is JArray jsonArray)
            {
                foreach (JToken item in jsonArray)
                {
                    RemoveIgnoredDerivedMembers(item);
                }
            }
        }

        /// <summary>
        /// 合并对象数组：按主键匹配，用户缺少的条目才追加；已存在的条目继续递归补齐字段。
        /// </summary>
        private static void MergeArray(
            JArray userArray,
            JArray defaultArray,
            string pathPrefix,
            List<string> addedMembers)
        {
            string? keyName = ResolveArrayKeyName(defaultArray, userArray);
            if (keyName == null)
            {
                // 无法确定主键的数组不做处理，避免产生重复条目
                return;
            }

            HashSet<string> existingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (JToken item in userArray)
            {
                string? keyValue = item is JObject itemObject ? itemObject[keyName]?.ToString() : null;
                if (!string.IsNullOrEmpty(keyValue))
                {
                    existingKeys.Add(keyValue);
                }
            }

            foreach (JToken defaultItem in defaultArray)
            {
                if (defaultItem is not JObject defaultItemObject)
                {
                    continue;
                }

                string? keyValue = defaultItemObject[keyName]?.ToString();
                if (string.IsNullOrEmpty(keyValue))
                {
                    continue;
                }

                if (!existingKeys.Contains(keyValue))
                {
                    // 用户缺少该条目：整条追加（同样剔除派生只读成员）
                    JToken appendedItem = defaultItem.DeepClone();
                    RemoveIgnoredDerivedMembers(appendedItem);

                    userArray.Add(appendedItem);
                    addedMembers.Add($"{pathPrefix}[{keyValue}]");
                    existingKeys.Add(keyValue);
                    continue;
                }

                // 条目已存在：只补齐该条目内部缺失的字段
                foreach (JToken userItem in userArray)
                {
                    if (userItem is not JObject userItemObject)
                    {
                        continue;
                    }

                    if (!string.Equals(userItemObject[keyName]?.ToString(), keyValue, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    MergeToken(userItemObject, defaultItemObject, $"{pathPrefix}[{keyValue}]", addedMembers);
                    break;
                }
            }
        }

        /// <summary>
        /// 推断数组条目的主键名：优先使用候选键中默认值与用户数据都存在的键。
        /// </summary>
        private static string? ResolveArrayKeyName(JArray defaultArray, JArray userArray)
        {
            foreach (string candidate in ArrayItemKeyCandidates)
            {
                bool existsInDefaults = defaultArray
                    .OfType<JObject>()
                    .Any(item => item.Property(candidate) != null);

                if (!existsInDefaults)
                {
                    continue;
                }

                // 用户数组为空时也允许使用该主键（首次补全）
                if (userArray.Count == 0 ||
                    userArray.OfType<JObject>().All(item => item.Property(candidate) != null))
                {
                    return candidate;
                }
            }

            return null;
        }

        /// <summary>
        /// 备份用户文件为 xxx.bak_{版本}，并清理过多历史备份。
        /// </summary>
        private static void BackupUserFile(string userFilePath, string versionText)
        {
            try
            {
                string backupPath = $"{userFilePath}{BackupInfix}{versionText}";
                File.Copy(userFilePath, backupPath, overwrite: true);

                string? directory = Path.GetDirectoryName(userFilePath);
                if (string.IsNullOrEmpty(directory))
                {
                    return;
                }

                string fileName = Path.GetFileName(userFilePath);
                List<string> backups = Directory
                    .GetFiles(directory, $"{fileName}{BackupInfix}*")
                    .OrderByDescending(File.GetCreationTimeUtc)
                    .ToList();

                foreach (string expired in backups.Skip(KeepBackupCountPerFile))
                {
                    File.Delete(expired);
                }
            }
            catch (Exception ex)
            {
                // 备份失败不应阻断合并，但必须留下日志
                LogTool.Log($"[ConfigurationMigrationService] 备份用户文件失败（{userFilePath}）：{ex.Message}");
                Debug.WriteLine($"[ConfigurationMigrationService] 备份用户文件失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 生成 ManualSettings 的默认值 JSON（与设置服务使用相同的序列化选项，保证格式一致）。
        /// </summary>
        private static JObject CreateManualSettingsDefaultsJson()
        {
            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new ColorJsonConverter() }
            };

            string json = System.Text.Json.JsonSerializer.Serialize(new ManualSettings(), options);
            return JObject.Parse(json);
        }

        /// <summary>
        /// 生成 AutomaticSettings 的默认值 JSON。
        /// </summary>
        private static JObject CreateAutomaticSettingsDefaultsJson()
        {
            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = System.Text.Json.JsonSerializer.Serialize(new AutomaticSettings(), options);
            return JObject.Parse(json);
        }
    }
}
