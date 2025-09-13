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

        /// <summary>
        /// 从英雄名获取英雄对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        HeroData GetHeroFromName(string name);

        /// <summary>
        /// 从图像获取英雄对象
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        Image GetImageFromHero(HeroData hero);

        /// <summary>
        /// 从英雄对象获取图像
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        HeroData GetHeroFromImage(Image image);

        /// <summary>
        /// 根据索引删除英雄
        /// </summary>
        /// <param name="index"></param>
        void DeletHeroAtIndex(int index);

        /// <summary>
        /// 添加英雄
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="image"></param>
        void AddHero(HeroData hero, Image image);
    }
}
