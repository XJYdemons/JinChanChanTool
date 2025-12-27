using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.RecommendedEquipment.Interface
{
    /// <summary>
    /// 该服务封装了所有与网络API交互的逻辑，
    /// 负责从远程服务器获取最新的英雄推荐出装数据。
    /// </summary>
    public interface ICrawlingService
    {
        /// <summary>
        /// 异步执行完整的网络爬取流程。
        /// </summary>
        /// <returns>
        /// 一个表示异步操作的任务。
        /// 任务的结果是一个包含了所有已成功获取的英雄及其推荐装备信息的列表 (List<HeroEquipment>)。
        /// 如果爬取过程中发生严重错误，可能会返回一个空列表。
        /// 添加 progress 参数
        /// </returns>
        Task<List<HeroEquipment>> GetEquipmentsAsync(IProgress<Tuple<int, string>> progress);
    }
}
