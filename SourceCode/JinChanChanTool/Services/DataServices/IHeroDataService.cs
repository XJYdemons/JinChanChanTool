using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices
{
    public interface IHeroDataService
    {                         
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
        /// 获取对应费用的英雄对象列表
        /// </summary>
        /// <returns></returns>
        List<HeroData> GetHeroDatasFromCost(int cost);

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
        /// 获取职业对象列表
        /// </summary>
        /// <returns></returns>
        List<Profession> GetProfessions();

        /// <summary>
        /// 获取特质对象列表
        /// </summary>
        /// <returns></returns>
        List<Peculiarity> GetPeculiarities();
        
        /// <summary>
        /// 根据索引删除英雄
        /// </summary>
        /// <param name="index"></param>
        bool DeletHeroAtIndex(int index);

        /// <summary>
        /// 添加英雄
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="image"></param>
        bool AddHero(HeroData hero, Image image);

        /// <summary>
        /// 获取英雄数量
        /// </summary>
        /// <returns></returns>
        int GetHeroCount();

        /// <summary>
        /// 获取文件路径数组
        /// </summary>
        /// <returns></returns>
        string[] GetFilePaths();

        /// <summary>
        /// 获取默认图片文件路径
        /// </summary>
        /// <returns></returns>
        string GetDefaultImagePath();

        /// <summary>
        /// 设置文件路径索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool SetFilePathsIndex(int index);

        /// <summary>
        /// 获取文件路径索引
        /// </summary>
        /// <returns></returns>
        int GetFilePathsIndex();

        /// <summary>
        /// 获取英雄数据对象列表
        /// </summary>
        /// <returns></returns>
        List<HeroData> GetHeroDatas();

        /// <summary>
        /// 获取英雄字符哈希表
        /// </summary>
        /// <returns></returns>
        public HashSet<char> GetCharDictionary();
    }
}
