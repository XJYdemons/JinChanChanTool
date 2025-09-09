using JinChanChanTool.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.DataServices
{
    public interface IHeroEquipmentDataService
    {
        string[] Paths { get; set;}
        int PathIndex { get; set;}
        List<HeroEquipment> HeroEquipments { get;set;}
        List<List<Image>> EquipmentsImages { get; set;}
        Dictionary<HeroEquipment,List<Image>> HeroEquipmentToImageListMap {  get; set;}      
        void Load();               
        void Save();
        void Reload();      
    }
}
