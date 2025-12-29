using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices.Interface
{
    public interface IEquipmentService
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
        /// 获取文件路径数组
        /// </summary>
        /// <returns></returns>
        string[] GetFilePaths();

        /// <summary>
        /// 设置文件路径索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool SetFilePathsIndex(int index);

        /// <summary>
        /// 根据索引删除英雄
        /// </summary>
        /// <param name="index"></param>
        bool DeletEquipmentAtIndex(int index);

        /// <summary>
        /// 获取装备数量
        /// </summary>
        /// <returns></returns>
        public int GetEquipmentCount();
        

        /// <summary>
        /// 添加装备
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="image"></param>
        bool AddEquipment(Equipment equipment, Image image);

        /// <summary>
        /// 设置文件路径索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool SetFilePathsIndex(string Season);

        /// <summary>
        /// 获取文件路径索引
        /// </summary>
        /// <returns></returns>
        int GetFilePathsIndex();

        /// <summary>
        /// 获取默认图片文件路径
        /// </summary>
        /// <returns></returns>
        string GetDefaultImagePath();

        /// <summary>
        /// 获取装备数据对象列表
        /// </summary>
        /// <returns></returns>
        List<Equipment> GetEquipmentDatas();

        /// <summary>
        /// 通过装备名称获取到装备对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Equipment GetEquipmentFromName(string name);
    }
}
