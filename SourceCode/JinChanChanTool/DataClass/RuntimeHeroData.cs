using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 这是一个在程序运行时使用的英雄数据类，它继承了HeroData的所有属性，并额外增加了装备推荐列表。
    /// 这个类的目的主要是为了将存储数据和运行时数据分离，避免污染原始的HeroData.json文件。
    /// </summary>
    public class RuntimeHeroData : HeroData
    {
        public List<string> RecommendedItems { get; set; } = new List<string>();
        public RuntimeHeroData(HeroData original)
        {
            this.HeroName = original.HeroName;
            this.Cost = original.Cost;
            this.Profession = original.Profession;
            this.Peculiarity = original.Peculiarity;
            this.ChessId = original.ChessId;
        }
    }
}
