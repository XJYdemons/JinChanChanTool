using System.Text.Json.Serialization;

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 单个待更新文件的动作类型。
    /// </summary>
    public enum UpdateFileAction
    {
        /// <summary>覆盖安装目录中的同名文件</summary>
        Replace = 0,

        /// <summary>安装目录中不存在，属于新增文件</summary>
        Add = 1
    }

    /// <summary>
    /// 待更新清单中的单个文件条目。
    /// </summary>
    public class UpdateFileEntry
    {
        /// <summary>相对于安装目录的路径，统一使用 / 分隔</summary>
        public string RelativePath { get; set; } = string.Empty;

        /// <summary>本次动作</summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UpdateFileAction Action { get; set; }

        /// <summary>新文件字节数</summary>
        public long Size { get; set; }

        /// <summary>新文件的 SHA256（小写十六进制），用于应用后校验</summary>
        public string Sha256 { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新应用计划：由主程序写出、由独立的应用进程读取并执行。
    /// 计划文件位于安装目录之外，因此不会被更新过程覆盖。
    /// </summary>
    public class UpdateApplyPlan
    {
        /// <summary>当前计划结构版本，结构变更时递增以便旧版本程序识别</summary>
        public int SchemaVersion { get; set; } = CurrentSchemaVersion;

        /// <summary>目标版本文本，形如 7.7.0</summary>
        public string TargetVersion { get; set; } = string.Empty;

        /// <summary>解压后的新文件所在目录</summary>
        public string StagingDirectory { get; set; } = string.Empty;

        /// <summary>程序安装目录</summary>
        public string InstallDirectory { get; set; } = string.Empty;

        /// <summary>本次更新的备份目录</summary>
        public string BackupDirectory { get; set; } = string.Empty;

        /// <summary>发起更新的主进程 ID，应用进程需要等待其退出</summary>
        public int ParentProcessId { get; set; }

        /// <summary>计划创建时间</summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        /// <summary>安装成功后需要启动的可执行文件（相对于安装目录）</summary>
        public string ExecutableRelativePath { get; set; } = "JinChanChanTool.exe";

        /// <summary>更新结束后需要删除的临时目录（staging、下载目录等）</summary>
        public List<string> CleanupDirectories { get; set; } = new();

        /// <summary>需要更新的文件清单</summary>
        public List<UpdateFileEntry> Files { get; set; } = new();

        /// <summary>当前计划结构版本号</summary>
        public const int CurrentSchemaVersion = 1;
    }
}
