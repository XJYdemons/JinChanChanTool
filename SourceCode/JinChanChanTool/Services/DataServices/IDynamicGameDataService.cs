using System.Collections.Generic;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.DataServices
{
    /// <summary>
    /// 为动态游戏数据服务定义接口。
    /// 该服务负责从网络获取、处理并提供最新的游戏数据，
    /// 例如英雄列表、装备翻译等，取代了旧的基于本地文件的 IApiRequestPayloadDataService。
    /// </summary>
    public interface IDynamicGameDataService
    {
        /// <summary>
        /// 获取一个字典，其中包含英雄API Key到中文名的映射。
        /// 在调用 InitializeAsync() 成功后，此属性将被填充。
        /// </summary>
        Dictionary<string, string> HeroTranslations { get; }

        /// <summary>
        /// 获取一个字典，其中包含装备API Key到中文名的映射。
        /// 在调用 InitializeAsync() 成功后，此属性将被填充。
        /// </summary>
        Dictionary<string, string> ItemTranslations { get; }

        /// <summary>
        /// 获取一个列表，其中包含当前赛季所有英雄的API Key。
        /// 在调用 InitializeAsync() 成功后，此属性将被填充。
        /// </summary>
        List<string> CurrentSeasonHeroKeys { get; }

        /// <summary>
        /// 异步初始化服务。
        /// 此方法会从网络上拉取所有必需的游戏数据（翻译、英雄列表等），
        /// 并填充此接口暴露的各个属性。
        /// 应该在应用程序启动时调用一次。
        /// </summary>
        /// <returns>一个表示异步操作的 Task。</returns>
        Task InitializeAsync();
    }
}