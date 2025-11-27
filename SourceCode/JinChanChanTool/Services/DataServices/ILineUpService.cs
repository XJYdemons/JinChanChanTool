using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices
{
    public interface ILineUpService
    {               
        /// <summary>
        /// 从本地文件加载阵容
        /// </summary>
        void Load();

        /// <summary>
        /// 将当前阵容数据保存到本地文件。
        /// </summary>
        bool Save();

        /// <summary>
        /// 重新加载，需要获取英雄数据服务对象
        /// </summary>
        /// <param name="countOfHeros"></param>
        /// <param name="countOfLineUps"></param>
        void ReLoad(IHeroDataService heroDataService);

        /// <summary>
        /// 获取当前子阵容
        /// </summary>
        /// <returns></returns>
        List<string> GetCurrentSubLineUp();

        /// <summary>
        /// 检查当前子阵容是否包含指定英雄名称，若包含则将其从子阵容删除，否则将其添加到子阵容。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool AddAndDeleteHero(string name);

        /// <summary>
        /// 增加指定英雄名称到当前子阵容，若已存在则不再增加
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool AddHero(string name);

        /// <summary>
        /// 批量增加指定英雄名称到当前子阵容，若已存在则不再增加
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        void AddHeros(List<string> names);

        /// <summary>
        /// 从当前子阵容删除指定英雄名称，若不存在则不会删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DeleteHero(string name);
       
        /// <summary>
        /// 清空当前子阵容
        /// </summary>
        void ClearCurrentSubLineUp();

        /// <summary>
        /// 设置阵容下标
        /// </summary>
        /// <param name="lineUpIndex"></param>
        bool SetLineUpIndex(int lineUpIndex);

        /// <summary>
        /// 获取阵容下标
        /// </summary>
        /// <returns></returns>
        int GetLineUpIndex();

        /// <summary>
        /// 设置指定下标阵容名称
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool SetLineUpName(int index, string name);

        /// <summary>
        /// 获取指定下标阵容名称
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetLineUpName(int index);
        
        /// <summary>
        /// 设置子阵容下标
        /// </summary>
        /// <param name="subLineUpIndex"></param>
        bool SetSubLineUpIndex(int subLineUpIndex);

        /// <summary>
        /// 获取子阵容下标
        /// </summary>
        /// <returns></returns>
        int GetSubLineUpIndex();
        
        /// <summary>
        /// 获取最大选择数量
        /// </summary>
        /// <returns></returns>
        int GetMaxSelect();

        /// <summary>
        /// 获取最大阵容数量
        /// </summary>
        /// <returns></returns>
        int GetMaxLineUpCount();

        /// <summary>
        /// 设置阵容文件路径下标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>        
        bool SetFilePathsIndex(int index);

        /// <summary>
        /// 设置文件路径索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool SetFilePathsIndex(string Season);

        event EventHandler LineUpChanged;

        event EventHandler LineUpNameChanged;

        event EventHandler SubLineUpIndexChanged;
    }
}
