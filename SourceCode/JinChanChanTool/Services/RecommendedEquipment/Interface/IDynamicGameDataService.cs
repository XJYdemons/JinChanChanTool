using System.Collections.Generic;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.RecommendedEquipment.Interface
{
    /// <summary>
    /// 为动态游戏数据服务定义接口。
    /// </summary>
    public interface IDynamicGameDataService
    {
        /// <summary>
        /// 获取一个字典，其中包含英雄API Key到中文名的映射。
        /// </summary>
        Dictionary<string, string> HeroTranslations { get; }

        /// <summary>
        /// 获取一个字典，其中包含装备API Key到中文名的映射。
        /// </summary>
        Dictionary<string, string> ItemTranslations { get; }

        /// <summary>
        /// 获取一个字典，其中包含羁绊API Key到中文名的映射。
        /// </summary>
        Dictionary<string, string> TraitTranslations { get; }

        /// <summary>
        /// 获取一个字典，其中包含通用标签（如难度、经济类型）的映射。
        /// </summary>
        Dictionary<string, string> CommonTranslations { get; }

        /// <summary>
        /// 获取一个列表，其中包含当前赛季所有英雄的API Key。
        /// </summary>
        List<string> CurrentSeasonHeroKeys { get; }

        /// <summary>
        /// 异步初始化服务。
        /// </summary>
        /// <returns>一个表示异步操作的 Task。</returns>
        Task InitializeAsync();
    }
}