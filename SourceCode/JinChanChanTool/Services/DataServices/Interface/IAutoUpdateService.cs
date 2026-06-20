namespace JinChanChanTool.Services.DataServices.Interface
{
    /// <summary>
    /// 自动更新服务接口
    /// </summary>
    public interface IAutoUpdateService
    {
        /// <summary>
        /// 检查并在后台更新数据（如果需要）
        /// </summary>
        Task CheckAndUpdateAsync();
    }
}
