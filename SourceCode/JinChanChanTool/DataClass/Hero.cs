using System.Text.Json.Serialization;

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 英雄数据对象
    /// </summary>
    public class Hero
    {
        /// <summary>
        /// 英雄名
        /// </summary>
        public string HeroName { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// 职业列表
        /// </summary>
        public List<string> Profession { get; set; } = new List<string>();

        /// <summary>
        /// 特质列表
        /// </summary>
        public List<string> Peculiarity { get; set; } = new List<string>();

        /// <summary>
        /// 英雄图片
        /// </summary>
        [JsonIgnore]
        public Bitmap Image { get; set; }

        public Hero()
        {
            HeroName = "";
            Cost = 1;
            Profession = new List<string>();
            Peculiarity = new List<string>();
            Image = null;
        }
    }
}
