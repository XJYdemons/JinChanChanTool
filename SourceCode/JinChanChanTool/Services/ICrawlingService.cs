using JinChanChanTool.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services
{
    public interface ICrawlingService
    {       
        /// <summary>
        /// 返回英雄装备数据对象列表
        /// </summary>
        /// <returns></returns>
        List<HeroEquipment> GetEquipments();
    }
}
