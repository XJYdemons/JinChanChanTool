namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 更新检查的结果。
    /// </summary>
    public class UpdateCheckResult
    {
        /// <summary>当前程序版本文本，形如 7.6.1</summary>
        public string CurrentVersion { get; init; } = string.Empty;

        /// <summary>远端最新版本文本，形如 7.7.0；检查失败时为空</summary>
        public string LatestVersion { get; init; } = string.Empty;

        /// <summary>远端发布标签，形如 v7.7.0</summary>
        public string LatestTag { get; init; } = string.Empty;

        /// <summary>是否存在可用更新</summary>
        public bool HasUpdate { get; init; }

        /// <summary>检查是否成功完成（失败时 SkipReason 说明原因）</summary>
        public bool IsSucceeded { get; init; }

        /// <summary>未更新的原因说明（已是最新、被用户跳过、网络失败等）</summary>
        public string SkipReason { get; init; } = string.Empty;

        /// <summary>发布说明文本</summary>
        public string ReleaseNotes { get; init; } = string.Empty;

        /// <summary>发布页地址</summary>
        public string ReleasePageUrl { get; init; } = string.Empty;

        /// <summary>发布发布时间（本地时间）</summary>
        public DateTimeOffset PublishedAt { get; init; }

        /// <summary>选定的主程序包资产，无可用资产时为 null</summary>
        public UpdateAssetInfo? PackageAsset { get; init; }

        /// <summary>
        /// 创建“无需更新”的结果。
        /// </summary>
        /// <param name="currentVersion">当前版本文本</param>
        /// <param name="reason">原因说明</param>
        /// <returns>检查结果</returns>
        public static UpdateCheckResult NoUpdate(string currentVersion, string reason)
        {
            return new UpdateCheckResult
            {
                CurrentVersion = currentVersion,
                IsSucceeded = true,
                HasUpdate = false,
                SkipReason = reason
            };
        }

        /// <summary>
        /// 创建“检查失败”的结果。
        /// </summary>
        /// <param name="currentVersion">当前版本文本</param>
        /// <param name="reason">失败原因</param>
        /// <returns>检查结果</returns>
        public static UpdateCheckResult Failure(string currentVersion, string reason)
        {
            return new UpdateCheckResult
            {
                CurrentVersion = currentVersion,
                IsSucceeded = false,
                HasUpdate = false,
                SkipReason = reason
            };
        }
    }
}
