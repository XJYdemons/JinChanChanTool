using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.Update.Interface
{
    /// <summary>
    /// 程序本体更新服务接口。
    /// 与只更新推荐数据的 IAutoUpdateService 区分：本接口负责主程序版本的检查、下载与安装。
    /// </summary>
    public interface IProgramUpdateService
    {
        /// <summary>
        /// 查询远端最新版本，判断是否存在可用更新。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>检查结果（失败时 IsSucceeded 为 false，并带原因）</returns>
        Task<UpdateCheckResult> CheckForUpdateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 下载更新包并生成待应用清单（含差异文件与解压结果）。
        /// </summary>
        /// <param name="checkResult">检查结果（需包含选定的包资产）</param>
        /// <param name="progress">进度回调：(百分比, 状态文本)</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>待应用计划</returns>
        Task<UpdateApplyPlan> DownloadAndPrepareAsync(
            UpdateCheckResult checkResult,
            IProgress<Tuple<int, string>>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 启动独立的应用进程执行更新并关闭当前程序。
        /// </summary>
        /// <param name="plan">待应用计划</param>
        /// <returns>是否成功启动更新进程</returns>
        bool LaunchApplyProcess(UpdateApplyPlan plan);

        /// <summary>
        /// 清理过期的下载包、staging 目录与历史备份，避免长期占用磁盘。
        /// </summary>
        /// <param name="keepBackupCount">保留最近几次备份</param>
        void CleanupExpiredArtifacts(int keepBackupCount);

        /// <summary>
        /// 读取并消费最近一次更新留下的结果记录（读取后删除，保证只提示一次）。
        /// </summary>
        /// <returns>更新结果记录；不存在时返回 null</returns>
        DataClass.UpdateResultRecord? ConsumeLatestUpdateResult();
    }
}
