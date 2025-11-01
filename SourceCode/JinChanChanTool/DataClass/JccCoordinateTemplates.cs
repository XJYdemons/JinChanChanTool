using System.Drawing;
using static JinChanChanTool.Services.AutoSetCoordinates.CoordinateCalculationService; // 引入AnchorProfile结构体

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 存放“金铲铲（模拟器）”的基准坐标模板。
    /// 所有坐标均以“底部居中”为锚点，基于 1600x910 的客户区分辨率。
    /// </summary>
    public static class JccCoordinateTemplates
    {
        /// <summary>
        /// 基准分辨率。
        /// </summary>
        public static readonly Size BaseResolution = new Size(1600, 910);

        // --- 核心功能区域 ---

        /// <summary>
        /// 商店购买经验值按钮
        /// </summary>
        public static readonly AnchorProfile ExpButton = new(-436.5, -118, 125, 50);

        /// <summary>
        /// 商店刷新按钮
        /// </summary>
        public static readonly AnchorProfile RefreshButton = new(-436.5, -45.5, 125, 45);

        /// <summary>
        /// 第一个英雄名字区域
        /// </summary>
        //public static readonly AnchorProfile CardSlot1_Name = new(-284, -28, 120, 30);
        public static readonly AnchorProfile CardSlot1_Name = new(-300, -29, 91, 26);

        /// <summary>
        /// 第二个英雄名字区域
        /// </summary>
        //public static readonly AnchorProfile CardSlot2_Name = new(-109, -28, 120, 30);
        public static readonly AnchorProfile CardSlot2_Name = new(-120, -29, 91, 24);

        /// <summary>
        /// 第三个英雄名字区域
        /// </summary>
        //public static readonly AnchorProfile CardSlot3_Name = new(66, -28, 120, 30);
        public static readonly AnchorProfile CardSlot3_Name = new(53, -30, 88, 27);

        /// <summary>
        /// 第四个英雄名字区域
        /// </summary>
        //public static readonly AnchorProfile CardSlot4_Name = new(236, -28, 110, 30);
        public static readonly AnchorProfile CardSlot4_Name = new(229, -28, 86, 27);

        /// <summary>
        /// 第五个英雄名字区域
        /// </summary>
        //public static readonly AnchorProfile CardSlot5_Name = new(413, -28, 110, 30);
        public static readonly AnchorProfile CardSlot5_Name = new(406, -29, 93, 26);

        // --- 备用区域 ---

        /// <summary>
        /// 第一个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot1_Click = new(-301.5, -47.5, 155, 125);

        /// <summary>
        /// 第二个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot2_Click = new(-124, -47.5, 150, 125);

        /// <summary>
        /// 第三个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot3_Click = new(53.5, -47.5, 155, 125);

        /// <summary>
        /// 第四个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot4_Click = new(229, -47.5, 150, 125);

        /// <summary>
        /// 第五个卡槽的可点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot5_Click = new(404, -47.5, 150, 125);

        /// <summary>
        /// 金币数量识别区域
        /// </summary>
        public static readonly AnchorProfile GoldAmount = new(17.5, -105, 65, 30);
    }
}