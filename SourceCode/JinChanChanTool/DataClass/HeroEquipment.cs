using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 英雄装备数据对象
    /// </summary>
    public class HeroEquipment
    {
        /// <summary>
        /// 英雄名称
        /// </summary>
        public string HeroName{ get; set; }
        /// <summary>
        /// 装备名称列表
        /// </summary>
        public  List<string> Equipments { get; set; }
    }
}
