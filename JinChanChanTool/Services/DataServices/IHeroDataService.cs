using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices
{
    public interface IHeroDataService
    {
        /// <summary>
        /// 本地文件路径列表
        /// </summary>
        public string[] Paths { get; set; }

        /// <summary>
        /// 文件路径索引
        /// </summary>
        int PathIndex { get; set; }

        /// <summary>
        /// 默认图片路径
        /// </summary>
        string DefaultImagePath { get; set; }

        /// <summary>
        /// 英雄数据对象列表
        /// </summary>
        List<HeroData> HeroDatas { get; }

        /// <summary>
        /// 英雄头像图片列表
        /// </summary>
        List<Image> HeroImages { get; }

        /// <summary>
        /// 职业对象列表
        /// </summary>
        List<Profession> Professions { get; }

        /// <summary>
        /// 特质对象列表
        /// </summary>
        List<Peculiarity> Peculiarities { get; }

        /// <summary>
        /// 图片到英雄数据对象的字典
        /// </summary>
        Dictionary<Image, HeroData> ImageToHeroDataMap { get; }

        /// <summary>
        /// 英雄数据对象到图片的字典
        /// </summary>
        Dictionary<HeroData, Image> HeroDataToImageMap { get; }

        /// <summary>
        /// 从本地加载到对象
        /// </summary>
        void Load();

        /// <summary>
        /// 重新加载
        /// </summary>
        void ReLoad();

        /// <summary>
        /// 从对象保存到本地
        /// </summary>
        void Save();
    }
}
