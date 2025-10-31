using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JinChanChanTool.Services.DataServices
{
    /// <summary>
    /// 包含用于反序列化从 api-hc.metatft.com 的 unit_detail API 获取的JSON响应的模型。
    /// </summary>

    /// <summary>
    /// 代表 unit_detail API 响应的顶层结构。
    /// 核心数据是装备组合列表。
    /// </summary>
    public class UnitDetailResponse
    {
        /// <summary>
        /// 包含该英雄所有被统计的装备组合的列表。
        /// </summary>
        [JsonPropertyName("builds")]
        public List<Build> Builds { get; set; }
    }

    /// <summary>
    /// 代表一个具体的装备组合及其统计数据。
    /// </summary>
    public class Build
    {
        /// <summary>
        /// 装备组合的API名称，以'|'分隔。
        /// 例如: "TFT_Item_Bloodthirster|TFT_Item_InfinityEdge|TFT_Item_SteraksGage"
        /// </summary>
        [JsonPropertyName("buildNames")]
        public string BuildNames { get; set; }

        /// <summary>
        /// 该装备组合被使用的总场次数。
        /// 计算热度的关键指标。
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }

        /// <summary>
        /// 一个包含8个整数的列表，分别代表获得第1名到第8名的场次数。
        /// places[0] = 第1名场次, places[1] = 第2名场次, 以此类推。
        /// 计算平均名次的关键数据。
        /// </summary>
        [JsonPropertyName("places")]
        public List<int> Places { get; set; }
    }
}