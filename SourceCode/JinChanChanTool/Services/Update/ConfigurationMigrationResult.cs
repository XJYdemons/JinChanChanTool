namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// 配置增量合并的结果。
    /// </summary>
    public class ConfigurationMigrationResult
    {
        /// <summary>实际写入过合并内容的文件列表</summary>
        public List<string> MergedFiles { get; } = new();

        /// <summary>因文件缺失或解析失败而跳过的文件列表</summary>
        public List<string> SkippedFiles { get; } = new();

        /// <summary>本次补齐的成员路径（形如 SettingForm.Panel.Button 或 List[Key]）</summary>
        public List<string> AddedMembers { get; } = new();

        /// <summary>合并过程中的错误说明，无错误时为空</summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 是否产生了任何变更。
        /// </summary>
        public bool HasChanges => MergedFiles.Count > 0 && AddedMembers.Count > 0;
    }
}
