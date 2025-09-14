namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 英雄数据对象
    /// </summary>
    public class HeroData
    {                   
        public HeroData()
        {
            HeroName = "";
            Cost = 1;
            Profession = "";
            Peculiarity = "";            
        }

        /// <summary>
        /// 英雄名
        /// </summary>
        public string HeroName { get; set; }

        /// <summary>
        /// 花费
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
    }
}
