using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices
{
    public interface ILineUpService
    {       
        /// <summary>
        /// 阵容文件路径索引
        /// </summary>
        int PathIndex { get; set; }

        /// <summary>
        /// LineUp对象列表
        /// </summary>
        List<LineUp> LineUps { get; }

        /// <summary>
        /// 英雄数量
        /// </summary>
        int CountOfHeros { get; }

        /// <summary>
        /// 阵容数量
        /// </summary>
        int CountOfLineUps { get; }

        /// <summary>
        /// 阵容索引
        /// </summary>
        int LineUpIndex { get; set; }

        /// <summary>
        /// 子阵容索引
        /// </summary>
        int SubLineUpIndex { get; set; }    

        /// <summary>
        /// 从本地文件加载
        /// </summary>
        void Load();

        /// <summary>
        /// 保存到本地
        /// </summary>
        void Save();

        /// <summary>
        /// 重新加载
        /// </summary>
        /// <param name="countOfHeros"></param>
        /// <param name="countOfLineUps"></param>
        void ReLoad(int countOfHeros, int countOfLineUps);
    }
}
