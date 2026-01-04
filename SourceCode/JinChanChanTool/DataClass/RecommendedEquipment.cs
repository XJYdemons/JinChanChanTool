using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 英雄装备数据对象
    /// </summary>
    public class RecommendedEquipment
    {
        /// <summary>
        /// 英雄的中文名
        /// </summary>
        [JsonPropertyName("heroName")]
        public string HeroName { get; set; }

        /// <summary>
        /// 该英雄的推荐装备列表 (通常包含3件装备的中文名)
        /// </summary>
        [JsonPropertyName("equipments")]
        public List<string> Equipments { get; set; }
    }
}
