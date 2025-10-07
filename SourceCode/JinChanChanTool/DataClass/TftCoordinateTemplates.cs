using System.Drawing;
using static JinChanChanTool.Services.CoordinateCalculationService; // 引入AnchorProfile结构体

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 存放“云顶之弈”PC客户端的基准坐标模板。
    /// 所有坐标均以“底部居中”为锚点，基于 1920x1080 的客户区分辨率。
    /// </summary>
    public static class TftCoordinateTemplates
    {
        /// <summary>
        /// 基准分辨率。
        /// </summary>
        public static readonly Size BaseResolution = new Size(1920, 1080);

        // --- 核心功能区域 ---

        /// <summary>
        /// 商店购买经验值按钮
        /// </summary>
        public static readonly AnchorProfile ExpButton = new(-595, -120, 170, 50);

        /// <summary>
        /// 商店刷新按钮
        /// </summary>
        public static readonly AnchorProfile RefreshButton = new(-597.5, -47.5, 175, 55);

        /// <summary>
        /// 第一个英雄名字区域
        /// </summary>
        public static readonly AnchorProfile CardSlot1_Name = new(-415, -25, 120, 30);

        /// <summary>
        /// 第二个英雄名字区域
        /// </summary>
        public static readonly AnchorProfile CardSlot2_Name = new(-217.5, -25, 115, 30);

        /// <summary>
        /// 第三个英雄名字区域
        /// </summary>
        public static readonly AnchorProfile CardSlot3_Name = new(-10, -25, 130, 30);

        /// <summary>
        /// 第四个英雄名字区域
        /// </summary>
        public static readonly AnchorProfile CardSlot4_Name = new(185, -25, 120, 30);

        /// <summary>
        /// 第五个英雄名字区域
        /// </summary>
        public static readonly AnchorProfile CardSlot5_Name = new(390, -25, 130, 30);

        // --- 备用区域 ---

        /// <summary>
        /// 第一个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot1_Click = new(-392.5, -82.5, 165, 125);

        /// <summary>
        /// 第二个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot2_Click = new(-190, -82.5, 160, 125);

        /// <summary>
        /// 第三个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot3_Click = new(-5, -82.5, 130, 125);

        /// <summary>
        /// 第四个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot4_Click = new(210, -82.5, 160, 125);

        /// <summary>
        /// 第五个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot5_Click = new(415, -82.5, 170, 125);

        /// <summary>
        /// 金币数量识别区域
        /// </summary>
        public static readonly AnchorProfile GoldAmount = new(22.5, -185, 65, 30);
    }
}