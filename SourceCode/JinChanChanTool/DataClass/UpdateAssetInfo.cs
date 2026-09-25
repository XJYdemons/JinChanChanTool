using System.Text.Json.Serialization;

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// GitHub 发布包中的一个资产（下载文件）。
    /// 字段名与 GitHub Releases API 的 JSON 保持一致，由 JsonPropertyName 显式映射。
    /// </summary>
    public class UpdateAssetInfo
    {
        /// <summary>资产文件名，例如 JinChanChanTool_v7.7.0_Windows_x64.zip</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>资产直接下载地址</summary>
        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = string.Empty;

        /// <summary>资产字节数</summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }

        /// <summary>GitHub 提供的摘要，形如 "sha256:xxxx"，未提供时为空</summary>
        [JsonPropertyName("digest")]
        public string? Digest { get; set; }

        /// <summary>
        /// 摘要中的 SHA256 十六进制文本（小写）；未提供时为 null。
        /// </summary>
        public string? Sha256
        {
            get
            {
                const string sha256Prefix = "sha256:";
                if (string.IsNullOrWhiteSpace(Digest) ||
                    !Digest.StartsWith(sha256Prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                string value = Digest[sha256Prefix.Length..].Trim();
                return value.Length == 0 ? null : value.ToLowerInvariant();
            }
        }
    }
}
