using System.Collections.Generic;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.DataServices
{
    /// <summary>
    /// 该服务负责从本地JSON文件中读取和提供网络爬取前所必需的映射数据。
    /// </summary>
    public interface IApiRequestPayloadDataService
    {
        /// <summary>
        /// 获取装备API Key到中文名的映射字典。
        /// Key: 装备的API Key (例如 "TFT_Item_GiantsSlayer")
        /// Value: 装备的中文名 (例如 "巨人杀手")
        /// </summary>
        Dictionary<string, string> EquipmentApiNameMap { get; }

        /// <summary>
        /// 获取英雄ID到API Key的映射字典。
        /// Key: 英雄的数字ID (例如 "915897")
        /// Value: 英雄的API Key (例如 "TFT15_KSante")
        /// </summary>
        Dictionary<string, string> HeroIdToKeyMap { get; }

        /// <summary>
        /// 获取英雄API Key到中文名的映射字典。
        /// Key: 英雄的API Key (例如 "TFT15_KSante")
        /// Value: 英雄的中文名 (例如 "奎桑提")
        /// </summary>
        Dictionary<string, string> HeroKeyToNameMap { get; }

        /// <summary>
        /// 异步加载所有必需的映射文件到内存中。
        /// </summary>
        /// <returns>一个表示异步操作的任务。</returns>
        Task LoadAllAsync();
    }
}