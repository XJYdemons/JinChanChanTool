using static JinChanChanTool.Services.AutoSetCoordinates.CoordinateCalculationService;

namespace JinChanChanTool.DataClass.StaticData
{
    /// <summary>
    /// 存放“金铲铲（雷电模拟器）”的基准坐标模板。
    /// 所有坐标均以“底部居中”为锚点，基于 1600x900 的客户区分辨率。
    /// </summary>
    public static class JccLdCoordinateTemplates
    {
        /// <summary>
        /// 基准分辨率。
        /// </summary>
        public static readonly Size BaseResolution = new Size(1600, 900);

        // --- 核心功能区域 ---

        /// <summary>
        /// 商店购买经验值按钮。
        /// </summary>
        public static readonly AnchorProfile ExperienceButton = new(-487.5, -133.5, 153, 71);

        /// <summary>
        /// 商店刷新按钮。
        /// </summary>
        public static readonly AnchorProfile RefreshButton = new(-487.5, -50, 153, 70);

        /// <summary>
        /// 第一个英雄名字区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot1_Name = new(-330.5, -32.5, 125, 35);

        /// <summary>
        /// 第二个英雄名字区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot2_Name = new(-133, -32.5, 130, 35);

        /// <summary>
        /// 第三个英雄名字区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot3_Name = new(67, -32.5, 130, 35);

        /// <summary>
        /// 第四个英雄名字区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot4_Name = new(269, -32.5, 140, 35);

        /// <summary>
        /// 第五个英雄名字区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot5_Name = new(468, -32.5, 140, 35);

        // --- 备用区域 ---

        /// <summary>
        /// 第一个卡槽的高亮/点击区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot1_Click = new(-293.5, -91.5, 187, 155);

        /// <summary>
        /// 第二个卡槽的高亮/点击区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot2_Click = new(-98.5, -91.5, 187, 155);

        /// <summary>
        /// 第三个卡槽的高亮/点击区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot3_Click = new(96.5, -91.5, 187, 155);

        /// <summary>
        /// 第四个卡槽的高亮/点击区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot4_Click = new(291.5, -91.5, 187, 155);

        /// <summary>
        /// 第五个卡槽的高亮/点击区域。
        /// </summary>
        public static readonly AnchorProfile CardSlot5_Click = new(486.5, -91.5, 187, 155);

        /// <summary>
        /// 金币数量识别区域。
        /// </summary>
        public static readonly AnchorProfile GoldAmount = new(20, -201, 60, 30);
    }
}
