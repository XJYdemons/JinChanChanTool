namespace JinChanChanTool.Services.Localization
{
    /// <summary>
    /// 本地化服务接口，提供多语言文本获取功能。
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// 当前使用的语言代码（如 "zh-CN"）
        /// </summary>
        string CurrentLanguage { get; }

        /// <summary>
        /// 所有可用的语言列表（通过扫描 Languages 目录自动发现）
        /// </summary>
        List<LanguageInfo> AvailableLanguages { get; }

        /// <summary>
        /// 根据键获取当前语言的翻译文本。
        /// 回退顺序：当前语言 → zh-CN 基准语言 → 返回 key 本身。
        /// </summary>
        /// <param name="key">翻译键（如 "MainForm.Title"）</param>
        /// <returns>翻译后的文本</returns>
        string Get(string key);

        /// <summary>
        /// 根据键获取翻译文本，并使用 string.Format 填充参数。
        /// 回退顺序：当前语言 → zh-CN 基准语言 → 返回 key 本身。
        /// </summary>
        /// <param name="key">翻译键</param>
        /// <param name="args">格式化参数</param>
        /// <returns>格式化后的翻译文本</returns>
        string Get(string key, params object[] args);

        /// <summary>
        /// 加载语言文件。在程序启动时调用。
        /// 会扫描 Resources/Languages/ 目录发现所有可用语言，
        /// 并加载指定语言和基准语言（zh-CN）的翻译字典。
        /// </summary>
        /// <param name="languageCode">要加载的语言代码</param>
        void Load(string languageCode);
    }
}
