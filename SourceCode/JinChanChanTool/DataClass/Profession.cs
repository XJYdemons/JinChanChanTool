namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 职业对象
    /// </summary>
    public class Profession
    {
        /// <summary>
        /// 职业名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 属于该职业的英雄数据对象列表
        /// </summary>
        public List<String> HeroNames { get; set; }
        public Profession()
        {
            Title = "";
            HeroNames = new List<String>();
        }      
    }
}
