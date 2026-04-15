namespace JinChanChanTool.Services.Localization
{
    /// <summary>
    /// 语言信息数据类，描述一个可用的语言选项。
    /// </summary>
    public class LanguageInfo
    {
        /// <summary>
        /// 语言代码（如 "zh-CN"、"en-US"）
        /// </summary>
        public string LanguageCode { get; }

        /// <summary>
        /// 语言的本地化显示名称（如 "简体中文"、"English"），用于在 UI 中显示
        /// </summary>
        public string NativeName { get; }

        /// <summary>
        /// 创建语言信息实例。
        /// </summary>
        /// <param name="languageCode">语言代码</param>
        /// <param name="nativeName">语言的本地化显示名称</param>
        public LanguageInfo(string languageCode, string nativeName)
        {
            LanguageCode = languageCode;
            NativeName = nativeName;
        }

        /// <summary>
        /// 返回语言的本地化显示名称，用于 ComboBox 等控件的显示。
        /// </summary>
        public override string ToString() => NativeName;
    }
}
