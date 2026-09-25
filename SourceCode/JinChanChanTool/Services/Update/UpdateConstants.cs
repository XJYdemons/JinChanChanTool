namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// 更新流程共用常量，避免在多处出现魔法字符串。
    /// </summary>
    public static class UpdateConstants
    {
        /// <summary>主程序可执行文件名</summary>
        public const string ExecutableFileName = "JinChanChanTool.exe";

        /// <summary>主程序集文件名</summary>
        public const string AssemblyFileName = "JinChanChanTool.dll";

        /// <summary>资源目录名</summary>
        public const string ResourcesDirectoryName = "Resources";

        /// <summary>更新相关文件所在的顶层目录名（位于安装目录之外）</summary>
        public const string UpdateRootDirectoryName = "Update";

        /// <summary>暂存包所在子目录名</summary>
        public const string PackagesDirectoryName = "Packages";

        /// <summary>日志子目录名</summary>
        public const string LogsDirectoryName = "Logs";

        /// <summary>用户态文件默认值快照子目录名</summary>
        public const string DefaultsDirectoryName = "Defaults";

        /// <summary>历史备份子目录名</summary>
        public const string BackupsDirectoryName = "Backups";

        /// <summary>默认值快照的保留份数</summary>
        public const int KeepDefaultsCount = 3;

        /// <summary>记录最近一次默认值快照位置的指针文件名</summary>
        public const string SnapshotPointerFileName = "latest_defaults_snapshot.txt";

        /// <summary>计划文件名</summary>
        public const string PlanFileName = "pending_update.json";

        /// <summary>相对路径统一使用的分隔符</summary>
        public const char RelativePathSeparator = '/';

        /// <summary>启动更新应用流程的命令行开关</summary>
        public const string ApplyUpdateArgument = "--apply-update";

        /// <summary>zip 包扩展名</summary>
        public const string PackageExtension = ".zip";

        /// <summary>下载临时文件扩展名（断点续传使用）</summary>
        public const string DownloadingExtension = ".downloading";

        /// <summary>更新前备份文件/目录时附加的后缀前缀</summary>
        public const string BackupSuffixPrefix = ".bak_";

        /// <summary>被替换文件临时重命名时使用的后缀（用于替换正在运行的映像）</summary>
        public const string ReplacedFileSuffix = ".old";

        /// <summary>更新完成标记文件名的前缀</summary>
        public const string UpdateResultFilePrefix = "update_result_";

        /// <summary>更新成功后保留的历史备份数量</summary>
        public const int KeepBackupCount = 2;

        /// <summary>字节到兆字节的换算基数</summary>
        public const long BytesPerMegabyte = 1024L * 1024L;

        /// <summary>判断是否为更新流程使用的相对路径（包内路径一律以 / 分隔）</summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>是否为更新流程路径</returns>
        public static bool IsUpdatePath(string relativePath)
        {
            return !string.IsNullOrWhiteSpace(relativePath) &&
                   relativePath.IndexOf(RelativePathSeparator) >= 0;
        }
    }

    /// <summary>
    /// 更新应用进程的退出码，便于父进程与批处理判断更新结果。
    /// </summary>
    public static class UpdateExitCode
    {
        /// <summary>更新成功</summary>
        public const int Success = 0;

        /// <summary>更新失败</summary>
        public const int Failed = 1;

        /// <summary>当前进程并非更新应用模式</summary>
        public const int NotApplyMode = -1;
    }
}
