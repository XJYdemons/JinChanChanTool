using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices.Interface
{
    /// <summary>
    /// 推荐阵容服务接口
    /// </summary>
    public interface IRecommendedLineUpService
    {
        /// <summary>
        /// 从本地文件加载推荐阵容数据
        /// </summary>
        void Load();

        /// <summary>
        /// 将推荐阵容数据保存到本地文件
        /// </summary>
        /// <returns>保存是否成功</returns>
        bool Save();

        /// <summary>
        /// 重新加载推荐阵容数据
        /// </summary>
        void ReLoad();

        /// <summary>
        /// 获取所有推荐阵容
        /// </summary>
        /// <returns>推荐阵容列表</returns>
        List<RecommendedLineUp> GetAllRecommendedLineUps();

        /// <summary>
        /// 根据优先级获取推荐阵容
        /// </summary>
        /// <param name="tier">阵容优先级</param>
        /// <returns>符合条件的推荐阵容列表</returns>
        List<RecommendedLineUp> GetRecommendedLineUpsByTier(LineUpTier tier);

        /// <summary>
        /// 根据标签获取推荐阵容
        /// </summary>
        /// <param name="tag">阵容标签</param>
        /// <returns>符合条件的推荐阵容列表</returns>
        List<RecommendedLineUp> GetRecommendedLineUpsByTag(string tag);

        /// <summary>
        /// 根据名称获取推荐阵容
        /// </summary>
        /// <param name="name">阵容名称</param>
        /// <returns>推荐阵容对象，如不存在则返回null</returns>
        RecommendedLineUp? GetRecommendedLineUpByName(string name);

        /// <summary>
        /// 添加推荐阵容
        /// </summary>
        /// <param name="lineUp">要添加的推荐阵容</param>
        /// <returns>添加是否成功</returns>
        bool AddRecommendedLineUp(RecommendedLineUp lineUp);

        /// <summary>
        /// 批量添加推荐阵容（用于爬虫数据导入）
        /// </summary>
        /// <param name="lineUps">要添加的推荐阵容列表</param>
        /// <returns>成功添加的数量</returns>
        int AddRecommendedLineUps(List<RecommendedLineUp> lineUps);

        /// <summary>
        /// 删除推荐阵容
        /// </summary>
        /// <param name="name">要删除的阵容名称</param>
        /// <returns>删除是否成功</returns>
        bool RemoveRecommendedLineUp(string name);

        /// <summary>
        /// 清空所有推荐阵容
        /// </summary>
        void ClearAll();

        /// <summary>
        /// 更新推荐阵容
        /// </summary>
        /// <param name="lineUp">更新后的推荐阵容对象</param>
        /// <returns>更新是否成功</returns>
        bool UpdateRecommendedLineUp(RecommendedLineUp lineUp);

        /// <summary>
        /// 获取推荐阵容数量
        /// </summary>
        /// <returns>推荐阵容数量</returns>
        int GetCount();

        /// <summary>
        /// 获取数据最后更新时间
        /// </summary>
        /// <returns>最后更新时间</returns>
        DateTime GetLastUpdateTime();

        /// <summary>
        /// 设置文件路径索引（根据赛季选择）
        /// </summary>
        /// <param name="season">赛季名称</param>
        /// <returns>是否找到对应赛季</returns>
        bool SetFilePathsIndex(string season);

        /// <summary>
        /// 检查数据是否需要更新（超过指定小时数）
        /// </summary>
        /// <param name="hours">小时数阈值</param>
        /// <returns>是否需要更新</returns>
        bool NeedsUpdate(int hours = 24);

        /// <summary>
        /// 推荐阵容数据变更事件
        /// </summary>
        event EventHandler DataChanged;
    }
}
