using JinChanChanTool.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services
{
    public class CrawlingService : ICrawlingService
    {
        /// <summary>
        /// 英雄装备数据对象列表
        /// </summary>
        private List<HeroEquipment> HeroEquipments { get; }

        /// <summary>
        /// 返回英雄装备数据对象列表
        /// </summary>
        /// <returns></returns>
        public List<HeroEquipment> GetEquipments()
        {
            
            return new List<HeroEquipment>();
        }
    }
}
