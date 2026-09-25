namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// 更新过程使用的固定文案。
    /// 更新在独立进程中执行，且发生在主程序退出之后，此时本地化服务可能已不可用，
    /// 因此这里集中存放固定文案，避免在更新流程中散落硬编码字符串。
    /// 关于窗口的版本展示仍走本地化文件（键 AboutForm.Label.版本格式）。
    /// </summary>
    public static class UpdateMessages
    {
        /// <summary>检查更新时的标题</summary>
        public const string CheckTitle = "检查更新";

        /// <summary>正在检查更新的提示</summary>
        public const string Checking = "正在检查最新版本...";

        /// <summary>已是最新版本的提示格式，{0}=当前版本</summary>
        public const string AlreadyLatest = "当前已是最新版本（{0}）。";

        /// <summary>发现新版本的提示格式，{0}=当前版本，{1}=最新版本</summary>
        public const string UpdateAvailable = "发现新版本：{0} → {1}。";

        /// <summary>检查失败的提示格式，{0}=原因</summary>
        public const string CheckFailed = "检查更新失败：{0}";

        /// <summary>是否下载并安装更新的询问格式，{0}=当前版本，{1}=最新版本</summary>
        public const string ConfirmUpdate = "发现新版本 {1}（当前 {0}）。\n\n是否立即下载并更新？\n\n更新会保留您的全部设置与阵容数据。";

        /// <summary>更新包较大时的额外提示</summary>
        public const string PackageSizeHint = "\n\n更新包约 {0} MB，下载期间可继续使用程序。";

        /// <summary>用户选择暂不更新时的记录提示</summary>
        public const string UpdatePostponed = "已暂不更新，可随时在“帮助 - 检查更新”中再次发起。";

        /// <summary>下载完成后的确认提示格式，{0}=最新版本</summary>
        public const string ReadyToInstall = "更新包已准备完成（{0}）。\n\n点击“确定”后程序将退出并自动完成更新，随后会自动重新启动。";

        /// <summary>启动更新进程失败时的提示</summary>
        public const string LaunchFailed = "启动更新进程失败，更新未执行。请手动重新下载发布包覆盖安装。";

        /// <summary>更新流程异常的提示格式，{0}=异常信息</summary>
        public const string UpdateError = "更新过程中出现错误：{0}";

        /// <summary>无需更新时的标题</summary>
        public const string NoUpdateTitle = "无需更新";

        /// <summary>更新准备完成时的标题</summary>
        public const string ReadyTitle = "准备更新";

        /// <summary>错误提示标题</summary>
        public const string ErrorTitle = "更新失败";

        /// <summary>更新结果标记文件读取失败时的提示</summary>
        public const string ResultFileUnreadable = "无法读取更新结果记录。";

        /// <summary>更新成功后的提示格式，{0}=版本号</summary>
        public const string UpdateSucceeded = "已成功更新到版本 {0}。";

        /// <summary>更新失败后的提示格式，{0}=原因</summary>
        public const string UpdateFailed = "更新未完成：{0}\n\n程序已恢复到更新前的状态，请重试或手动下载发布包。";
    }
}
