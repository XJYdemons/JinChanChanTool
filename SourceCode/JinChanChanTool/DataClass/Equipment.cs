using System.Text.Json.Serialization;
namespace JinChanChanTool.DataClass
{
    public class Equipment
    {
        /// <summary>
        /// 装备名
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 装备类型
        /// </summary>
        public string EquipmentType { get; set; } = null!;

        /// <summary>
        /// 合成路径（两个散件名称），为空表示无合成路径
        /// </summary>
        public string[] SyntheticPathway { get; set; } = null!;

        /// <summary>
        /// 装备图片（释放或未加载时为 null）
        /// </summary>
        [JsonIgnore]
        public Bitmap? Image { get; set; }
    }
}
