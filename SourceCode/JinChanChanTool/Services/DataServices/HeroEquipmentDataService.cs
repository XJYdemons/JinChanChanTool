using JinChanChanTool.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.DataServices
{
    public class HeroEquipmentDataService : IHeroEquipmentDataService
    {
        public string[] Paths {  get;  set; }
        public int PathIndex { get; set; }
        public List<HeroEquipment> HeroEquipments { get; set; }
        public List<List<Image>> EquipmentsImages { get; set; }
        public Dictionary<HeroEquipment, List<Image>> HeroEquipmentToImageListMap { get; set; }

        HeroEquipmentDataService()
        {
            InitializePaths();
            HeroEquipments = new List<HeroEquipment>();
            EquipmentsImages = new List<List<Image>>();
            HeroEquipmentToImageListMap = new Dictionary<HeroEquipment, List<Image>>();
            
        }

        private void BuildHeroEquipmentToImageListMap()
        {
           
        }

        private void InitializePaths()
        {
            
        }

        public void Load()
        {
            LoadFromJson();
            LoadImages();
            BuildHeroEquipmentToImageListMap();
        }

        private void LoadFromJson()
        {
            
        }

        private void LoadImages()
        {
            
        }

        public void Reload()
        {
            
        }

        public void Save()
        {
            HeroEquipments.Clear();
            EquipmentsImages.Clear();
            HeroEquipmentToImageListMap.Clear();
            Load();
        }
    }
}
