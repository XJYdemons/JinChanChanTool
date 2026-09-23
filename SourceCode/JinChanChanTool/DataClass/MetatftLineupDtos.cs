using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JinChanChanTool.DataClass
{
    // 元数据 (Metadata) 相关模型 

    public class CompsDataResponse
    {
        [JsonPropertyName("results")]
        public CompsDataResults Results { get; set; } = null!;
    }

    public class CompsDataResults
    {
        [JsonPropertyName("data")]
        public CompsDataDetail Data { get; set; } = null!;
    }

    public class CompsDataDetail
    {
        [JsonPropertyName("cluster_details")]
        public Dictionary<string, ClusterDetail> ClusterDetails { get; set; } = null!;
    }

    public class ClusterDetail
    {
        [JsonPropertyName("Cluster")]
        public int Cluster { get; set; }

        [JsonPropertyName("name")]
        public List<CompNameItem> Name { get; set; } = null!;

        [JsonPropertyName("units_string")]
        public string UnitsString { get; set; } = null!;

        [JsonPropertyName("builds")]
        public List<CompBuildInfo> Builds { get; set; } = null!;
    }

    public class CompBuildInfo
    {
        [JsonPropertyName("unit")]
        public string Unit { get; set; } = null!;

        [JsonPropertyName("buildName")]
        public List<string> BuildName { get; set; } = null!;
    }

    // 实时统计 (Stats) 相关模型

    public class CompsStatsResponse
    {
        [JsonPropertyName("results")]
        public List<CompStatResult> Results { get; set; } = null!;
    }

    public class CompStatResult
    {
        [JsonPropertyName("cluster")]
        public string Cluster { get; set; } = null!;

        [JsonPropertyName("places")]
        public List<int> Places { get; set; } = null!;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    // 阵容详情与站位 (Details) 相关模型 

    public class CompDetailsResponse
    {
        [JsonPropertyName("results")]
        public CompDetailsResults Results { get; set; } = null!;
    }

    public class CompDetailsResults
    {
        [JsonPropertyName("proComps")]
        public List<ProCompEntry> ProComps { get; set; } = null!;

        [JsonPropertyName("positioning")]
        public PositioningData Positioning { get; set; } = null!;
    }

    public class ProCompEntry
    {
        [JsonPropertyName("content")]
        public ProCompContent Content { get; set; } = null!;
    }

    public class ProCompContent
    {
        [JsonPropertyName("content")]
        public ProCompInnerContent InnerContent { get; set; } = null!;
    }

    public class ProCompInnerContent
    {
        [JsonPropertyName("titleImages")]
        public List<TitleImage> TitleImages { get; set; } = null!;
    }

    public class TitleImage
    {
        [JsonPropertyName("apiName")]
        public string ApiName { get; set; } = null!;
    }

    public class PositioningData
    {
        [JsonPropertyName("units")]
        public Dictionary<string, UnitPositioning> Units { get; set; } = null!;
    }

    public class UnitPositioning
    {
        [JsonPropertyName("positions")]
        public List<CellCount> Positions { get; set; } = null!;
    }

    public class CellCount
    {
        [JsonPropertyName("cell")]
        public string Cell { get; set; } = null!;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    // 通用翻译 (zh_cn.json) 相关模型 

    public class MetatftGeneralTranslation
    {
        [JsonPropertyName("common")]
        public Dictionary<string, string> Common { get; set; } = null!;
    }

    public class CompNameItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 类型：unit 代表英雄，trait 代表羁绊
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;
        
    }
}