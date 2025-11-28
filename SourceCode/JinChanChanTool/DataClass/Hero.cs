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
        /// 职业，以“|”分割
        /// </summary>
        public string Profession { get; set; } = "";

        /// <summary>
        /// 特质，以“|”分割
        /// </summary>
        public string Peculiarity { get; set; } = "";

        public Hero()
        {
            HeroName = "";
            Cost = 1;
            Profession = "";
            Peculiarity = "";
        }

    }
}
