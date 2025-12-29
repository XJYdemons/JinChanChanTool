using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.RecommendedEquipment.Interface
{
    public interface IHeroEquipmentDataService
    {
        /// <summary>
        /// 获取或设置所有可用赛季（数据文件夹）的路径列表。
        /// </summary>
        string[] Paths { get; set; }

        /// <summary>
        /// 获取或设置当前选中的赛季路径的索引。
        /// </summary>
       // int PathIndex { get; set; }

        /// <summary>
        /// 获取当前加载的英雄装备推荐数据列表。
        /// 主要用于遍历和展示所有英雄。
        /// </summary>
        List<HeroEquipment> HeroEquipments { get; }

        /// <summary>
        /// 获取一个从英雄装备对象到其对应装备图片列表的映射。
        /// 主要用于UI层根据一个具体的HeroEquipment对象，快速查找并展示其装备图片。
        /// </summary>
        Dictionary<HeroEquipment, List<Image>> EquipmentImageMap { get; }

        /// <summary>
        /// 从当前PathIndex指定的本地路径，加载所有数据（JSON和图片）。
        /// </summary>
        void Load();

        /// <summary>
        /// 将内存中的 HeroEquipments 数据保存到当前PathIndex指定的本地JSON文件中。
        /// </summary>
        void Save();

        /// <summary>
        /// 清空所有内存中的数据，然后重新执行 Load() 方法。
        /// </summary>
        void ReLoad();

        /// <summary>
        /// 接收从网络爬取到的新数据，更新内存中的状态，并将其持久化保存。
        /// </summary>
        /// <param name="crawledData">由 ICrawlingService 获取到的最新英雄装备数据列表。</param>
        void UpdateDataFromCrawling(List<HeroEquipment> crawledData);

        HeroEquipment GetHeroEquipmentFromName(string name);

        List<Image> GetImagesFromHeroEquipment(HeroEquipment heroEquipment);
    }
}
