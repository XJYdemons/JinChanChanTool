using JinChanChanTool.DataClass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.LineupCrawling.Interface
{
    /// <summary>
    /// 阵容推荐爬取服务接口。
    /// 负责从远程服务器抓取、解析并计算最新的阵容数据。
    /// </summary>
    public interface ILineupCrawlingService
    {
        /// <summary>
        /// 异步执行完整的阵容数据爬取流程。
        /// 包含元数据获取、实时统计计算、阵容详情抓取以及标签站位解析。
        /// </summary>
        /// <param name="progress">用于向UI反馈进度的对象 (百分比, 当前操作描述)。</param>
        /// <returns>
        /// 一个表示异步操作的任务。
        /// 任务结果是包含所有解析后的阵容对象列表 (List<RecommendedLineUp>)。
        /// </returns>
        Task<List<RecommendedLineUp>> GetRecommendedLineUpsAsync(IProgress<Tuple<int, string>> progress);
    }
}