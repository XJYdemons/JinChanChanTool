namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 特质对象
    /// </summary>
    public class Peculiarity
    {
        /// <summary>
        /// 特质名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 属于该特质的英雄数据对象列表
        /// </summary>
        public List<String> HeroNames { get; set; }
        public Peculiarity()
        {
            Title = "";
            HeroNames = new List<String>();            
        }        
    }
}
