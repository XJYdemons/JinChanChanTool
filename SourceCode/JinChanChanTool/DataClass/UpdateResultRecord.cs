namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 一次更新执行的结果记录。
    /// 由更新进程写入用户目录，主程序下次启动时读取并提示用户，读取后即删除。
    /// </summary>
    public class UpdateResultRecord
    {
        /// <summary>目标版本文本</summary>
        public string TargetVersion { get; set; } = string.Empty;

        /// <summary>更新是否成功</summary>
        public bool IsSuccess { get; set; }

        /// <summary>结果说明</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>完成时间</summary>
        public DateTimeOffset CompletedAt { get; set; }

        /// <summary>结果记录文件路径（读取后用于删除，不参与序列化）</summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string FilePath { get; set; } = string.Empty;
    }
}
