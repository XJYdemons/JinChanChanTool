using System.Text.Json.Serialization;

namespace JinChanChanTool.Services.RecommendedEquipment
{
    /// <summary>
    /// 包含用于反序列化从 data.metatft.com 获取的翻译JSON文件的模型。
    /// 这个文件定义了英雄(units)和装备(items)的数据结构。
    /// </summary>

    /// <summary>
    /// 代表翻译JSON文件的顶层结构。
    /// </summary>
    public class TranslationData
    {
        /// <summary>
        /// 包含所有英雄翻译信息的字典。
        /// Key 是英雄的API名称 (例如 "TFT15_LeeSin")。
        /// Value 是包含该英雄具体翻译信息的对象。
        /// </summary>
        [JsonPropertyName("units")]
        public List<TranslationEntry> Units { get; set; }

        /// <summary>
        /// 包含所有装备翻译信息的字典。
        /// Key 是装备的API名称 (例如 "TFT_Item_Bloodthirster")。
        /// Value 是包含该装备具体翻译信息的对象。
        /// </summary>
        [JsonPropertyName("items")]
        public List<TranslationEntry> Items { get; set; }

        /// <summary>
        /// 包含所有羁绊翻译信息的列表。
        /// </summary>
        [JsonPropertyName("traits")]
        public List<TranslationEntry> Traits { get; set; }
    }

    /// <summary>
    /// 代表一个可翻译条目（英雄或装备）的通用结构。
    /// 只关心它的中文名称。
    /// </summary>
    public class TranslationEntry
    {

        [JsonPropertyName("apiName")]
        public string ApiName { get; set; }
        /// <summary>
        /// 条目的中文名称 (例如 "李青" 或 "饮血剑")。
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}