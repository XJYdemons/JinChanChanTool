using System.Text.Json.Serialization;

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// GitHub Release 的响应模型（只保留更新流程需要的字段）。
    /// </summary>
    public class UpdateReleaseInfo
    {
        /// <summary>发布标签，例如 v7.7.0</summary>
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = string.Empty;

        /// <summary>发布标题</summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>发布说明（Markdown 文本）</summary>
        [JsonPropertyName("body")]
        public string? Body { get; set; }

        /// <summary>是否为预发布版本，预发布不参与自动更新</summary>
        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        /// <summary>是否为草稿，草稿不参与自动更新</summary>
        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        /// <summary>发布时间（UTC）</summary>
        [JsonPropertyName("published_at")]
        public DateTimeOffset PublishedAt { get; set; }

        /// <summary>发布页地址</summary>
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        /// <summary>该发布包含的所有资产</summary>
        [JsonPropertyName("assets")]
        public List<UpdateAssetInfo> Assets { get; set; } = new();
    }
}
