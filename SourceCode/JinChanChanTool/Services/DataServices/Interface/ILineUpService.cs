using JinChanChanTool.DataClass;
using static JinChanChanTool.DataClass.LineUp;

namespace JinChanChanTool.Services.DataServices.Interface
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
        /// 新增阵容
        /// </summary>
        /// <param name="lineUpName"></param>
        /// <returns></returns>
        public bool AddLineUp(string lineUpName);

        /// <summary>
        /// 删除当前阵容
        /// </summary>
        /// <returns></returns>
        public bool DeleteLineUp();

        /// <summary>
        /// 判断阵容名是否可用（不与现有阵容重名）
        /// </summary>
        /// <param name="name">待检查的阵容名</param>
        /// <returns>可用返回true，已存在返回false</returns>
        public bool IsLineUpNameAvailable(string name);

        /// <summary>
        /// 检查当前子阵容是否包含指定英雄名称，若包含则将其从子阵容删除，否则将其添加到子阵容。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool AddAndDeleteHero(string name, string[] equipments);

        /// <summary>
        /// 增加指定英雄名称到当前子阵容(指定装备)，若已存在则不再增加
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool AddHero(string name, string[] equipments);

        /// <summary>
        /// 增加指定英雄名称到当前子阵容(不指定装备)，若已存在则不再增加
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool AddHero(string name);
      
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
        /// 替换当前子阵容的英雄列表（用于从推荐阵容导入）
        /// </summary>
        /// <param name="lineUpUnits">要导入的英雄单位列表</param>
        /// <returns>是否替换成功</returns>
        bool ReplaceCurrentSubLineUp(List<LineUpUnit> lineUpUnits);

        /// <summary>
        /// 修改当前子阵容中指定英雄的装备
        /// </summary>
        /// <param name="heroName">英雄名称</param>
        /// <param name="equipmentIndex">装备槽位索引(0-2)</param>
        /// <param name="equipmentName">新装备名称</param>
        /// <returns>是否修改成功</returns>
        bool SetHeroEquipment(string heroName, int equipmentIndex, string equipmentName);

        /// <summary>
        /// 获取阵容集合
        /// </summary>
        /// <returns></returns>
        public List<LineUp> GetLineUps();

        /// <summary>
        /// 获取当前阵容对象
        /// </summary>
        /// <returns></returns>
        public LineUp GetCurrentLineUp();

        /// <summary>
        /// 获取当前变阵
        /// </summary>
        /// <returns></returns>
        SubLineUp GetCurrentSubLineUp();

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
        /// 设置当前变阵下标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool SetSubLineUpIndex(int index);

        /// <summary>
        /// 获取当前变阵下标
        /// </summary>
        /// <returns></returns>
        int GetSubLineUpIndex();

        /// <summary>
        /// 设置指定下标阵容名称
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool SetLineUpName(string name);

        /// <summary>
        /// 设置当前阵容下指定下标变阵名称
        /// </summary>
        /// <param name="index">变阵下标</param>
        /// <param name="name">新名称</param>
        /// <returns></returns>
        bool SetSubLineUpName(int index, string name);

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
        /// 设置文件路径索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool SetFilePathsIndex(string Season);
       
        /// <summary>
        /// 阵容改变事件
        /// </summary>
        event EventHandler LineUpChanged;

        /// <summary>
        /// 阵容名改变事件
        /// </summary>
        event EventHandler LineUpNameChanged;
    }
}
