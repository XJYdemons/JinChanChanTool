using System.Text.Json;

namespace JinChanChanTool.Services.Localization
{
    /// <summary>
    /// 本地化服务实现，基于 JSON 语言文件提供多语言支持。
    /// 语言文件存放在 Resources/Languages/ 目录下，程序启动时自动扫描发现所有可用语言。
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        /// <summary>
        /// 基准语言代码（简体中文），作为所有翻译的回退语言
        /// </summary>
        private const string FallbackLanguageCode = "zh-CN";

        /// <summary>
        /// 语言文件中的元数据键名，用于存储语言信息
        /// </summary>
        private const string MetaKeyName = "$meta";

        /// <summary>
        /// 语言文件中的注释键名前缀，加载时跳过这些键
        /// </summary>
        private const string CommentKeyPrefix = "__comment_";

        /// <summary>
        /// 语言文件所在目录路径
        /// </summary>
        private readonly string _languagesDirectoryPath;

        /// <summary>
        /// 当前语言的翻译字典（key → 翻译文本）
        /// </summary>
        private Dictionary<string, string> _currentTranslations;

        /// <summary>
        /// 基准语言（zh-CN）的翻译字典，当前语言缺失某个键时回退到此字典
        /// </summary>
        private Dictionary<string, string> _fallbackTranslations;

        /// <summary>
        /// 当前使用的语言代码（如 "zh-CN"）
        /// </summary>
        public string CurrentLanguage { get; private set; }

        /// <summary>
        /// 所有可用的语言列表（通过扫描 Languages 目录自动发现）
        /// </summary>
        public List<LanguageInfo> AvailableLanguages { get; private set; }

        /// <summary>
        /// 创建本地化服务实例，初始化语言文件目录路径。
        /// </summary>
        public LocalizationService()
        {
            _languagesDirectoryPath = Path.Combine(
                Application.StartupPath, "Resources", "Languages");
            _currentTranslations = new Dictionary<string, string>();
            _fallbackTranslations = new Dictionary<string, string>();
            AvailableLanguages = new List<LanguageInfo>();
            CurrentLanguage = FallbackLanguageCode;
        }

        /// <summary>
        /// 加载语言文件。扫描可用语言，加载基准语言和目标语言的翻译字典。
        /// 如果目标语言文件不存在，自动回退到基准语言（zh-CN）。
        /// </summary>
        /// <param name="languageCode">要加载的语言代码</param>
        public void Load(string languageCode)
        {
            // 扫描语言目录，发现所有可用语言
            ScanAvailableLanguages();

            // 始终加载基准语言（用于回退）
            _fallbackTranslations = LoadLanguageFile(FallbackLanguageCode);

            // 确定实际使用的语言代码
            string targetLanguage = ResolveLanguageCode(languageCode);
            CurrentLanguage = targetLanguage;

            // 如果目标语言就是基准语言，直接复用同一个字典
            if (targetLanguage == FallbackLanguageCode)
            {
                _currentTranslations = _fallbackTranslations;
            }
            else
            {
                _currentTranslations = LoadLanguageFile(targetLanguage);
            }
        }

        /// <summary>
        /// 根据键获取当前语言的翻译文本。
        /// 回退顺序：当前语言 → zh-CN 基准语言 → 返回 key 本身（便于开发者发现遗漏）。
        /// </summary>
        /// <param name="key">翻译键（如 "MainForm.Title"）</param>
        /// <returns>翻译后的文本</returns>
        public string Get(string key)
        {
            // 优先从当前语言查找
            if (_currentTranslations.TryGetValue(key, out string? currentValue))
            {
                return currentValue;
            }

            // 回退到基准语言
            if (_fallbackTranslations.TryGetValue(key, out string? fallbackValue))
            {
                return fallbackValue;
            }

            // 最终回退：返回键名本身，便于开发者发现遗漏的翻译
            return key;
        }

        /// <summary>
        /// 根据键获取翻译文本，并使用 string.Format 填充参数。
        /// </summary>
        /// <param name="key">翻译键</param>
        /// <param name="args">格式化参数</param>
        /// <returns>格式化后的翻译文本</returns>
        public string Get(string key, params object[] args)
        {
            string template = Get(key);
            try
            {
                return string.Format(template, args);
            }
            catch (FormatException)
            {
                // 格式化失败时返回原始模板，避免因翻译文件中占位符错误导致崩溃
                return template;
            }
        }

        #region 私有方法

        /// <summary>
        /// 扫描 Resources/Languages/ 目录下的所有 JSON 文件，
        /// 读取每个文件的 $meta 信息构建可用语言列表。
        /// </summary>
        private void ScanAvailableLanguages()
        {
            AvailableLanguages = new List<LanguageInfo>();

            if (!Directory.Exists(_languagesDirectoryPath))
            {
                return;
            }

            string[] languageFiles = Directory.GetFiles(_languagesDirectoryPath, "*.json");

            foreach (string filePath in languageFiles)
            {
                LanguageInfo? languageInfo = ReadLanguageMetadata(filePath);
                if (languageInfo != null)
                {
                    AvailableLanguages.Add(languageInfo);
                }
            }
        }

        /// <summary>
        /// 从语言文件中读取 $meta 元数据，提取语言代码和显示名称。
        /// </summary>
        /// <param name="filePath">语言文件的完整路径</param>
        /// <returns>语言信息对象，读取失败时返回 null</returns>
        private LanguageInfo? ReadLanguageMetadata(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                using JsonDocument document = JsonDocument.Parse(json);
                JsonElement root = document.RootElement;

                if (!root.TryGetProperty(MetaKeyName, out JsonElement metaElement))
                {
                    return null;
                }

                string languageCode = metaElement.TryGetProperty("languageCode", out JsonElement codeElement)
                    ? codeElement.GetString() ?? Path.GetFileNameWithoutExtension(filePath)
                    : Path.GetFileNameWithoutExtension(filePath);

                string nativeName = metaElement.TryGetProperty("nativeName", out JsonElement nameElement)
                    ? nameElement.GetString() ?? languageCode
                    : languageCode;

                return new LanguageInfo(languageCode, nativeName);
            }
            catch
            {
                // 语言文件格式错误时跳过该文件，不影响其他语言的加载
                return null;
            }
        }

        /// <summary>
        /// 加载指定语言代码对应的 JSON 文件，解析为翻译字典。
        /// 跳过 $meta 元数据和 __comment_ 前缀的注释键。
        /// </summary>
        /// <param name="languageCode">语言代码（如 "zh-CN"）</param>
        /// <returns>翻译字典（key → 翻译文本），加载失败时返回空字典</returns>
        private Dictionary<string, string> LoadLanguageFile(string languageCode)
        {
            Dictionary<string, string> translations = new Dictionary<string, string>();

            string filePath = Path.Combine(_languagesDirectoryPath, $"{languageCode}.json");

            if (!File.Exists(filePath))
            {
                return translations;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                using JsonDocument document = JsonDocument.Parse(json);
                JsonElement root = document.RootElement;

                foreach (JsonProperty property in root.EnumerateObject())
                {
                    string propertyName = property.Name;

                    // 跳过元数据对象
                    if (propertyName == MetaKeyName)
                    {
                        continue;
                    }

                    // 跳过注释键
                    if (propertyName.StartsWith(CommentKeyPrefix))
                    {
                        continue;
                    }

                    // 仅处理字符串类型的值
                    if (property.Value.ValueKind == JsonValueKind.String)
                    {
                        string? value = property.Value.GetString();
                        if (value != null)
                        {
                            translations[propertyName] = value;
                        }
                    }
                }
            }
            catch
            {
                // 语言文件解析失败时返回空字典，后续通过回退机制处理
            }

            return translations;
        }

        /// <summary>
        /// 解析语言代码，确保返回一个有效的、可用的语言代码。
        /// 如果指定的语言不可用，回退到基准语言。
        /// </summary>
        /// <param name="languageCode">用户指定的语言代码</param>
        /// <returns>实际可用的语言代码</returns>
        private string ResolveLanguageCode(string languageCode)
        {
            // 如果语言代码为空或 null，使用基准语言
            if (string.IsNullOrEmpty(languageCode))
            {
                return FallbackLanguageCode;
            }

            // 检查语言文件是否存在
            string filePath = Path.Combine(_languagesDirectoryPath, $"{languageCode}.json");
            if (File.Exists(filePath))
            {
                return languageCode;
            }

            // 语言文件不存在，回退到基准语言
            return FallbackLanguageCode;
        }

        #endregion
    }
}
