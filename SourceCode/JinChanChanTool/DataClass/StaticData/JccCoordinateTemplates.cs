using static JinChanChanTool.Services.AutoSetCoordinates.CoordinateCalculationService; // 引入AnchorProfile结构体

namespace JinChanChanTool.DataClass.StaticData
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
        public static readonly AnchorProfile ExperienceButton = new(-436.5, -118, 125, 50);

        /// <summary>
        /// 商店刷新按钮
        /// </summary>
        public static readonly AnchorProfile RefreshButton = new(-436.5, -45.5, 125, 45);

        /// <summary>
        /// 第一个英雄名字区域
        /// </summary>       
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
        /// 第一个卡槽的高亮/点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot1_Click = new(-262.8, -81.9, 166, 136);

        /// <summary>
        /// 第二个卡槽的高亮/点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot2_Click = new(-88.4, -81.9, 166, 136);

        /// <summary>
        /// 第三个卡槽的高亮/点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot3_Click = new(86.4, -81.9, 166, 136);

        /// <summary>
        /// 第四个卡槽的高亮/点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot4_Click = new(261.3, -81.9, 166, 136);

        /// <summary>
        /// 第五个卡槽的高亮/点击区域
        /// </summary>
        public static readonly AnchorProfile CardSlot5_Click = new(436.1, -81.9, 166, 136);

        /// <summary>
        /// 金币数量识别区域
        /// </summary>
        public static readonly AnchorProfile GoldAmount = new(17.5, -105, 65, 30);

        // --- 自动拾取棋盘物品（PVE 野怪后盲点拾取掉落物）---
        // 说明：坐标基于 1600x910 基准 + 底部居中锚点，经 CoordinateCalculationService 缩放适配实际窗口
        // 注：以下偏移为初版估算值，需按实际游戏画面微调（可进对局截图确认）

        /// <summary>
        /// 回合文本识别区域（左上角回合号，如 "2-1"），用于判断野怪回合结束时机
        /// </summary>
        public static readonly AnchorProfile RoundText = new(-740, -860, 130, 42);

        /// <summary>
        /// 棋盘盲点拾取网格（8 个点，左键点击触发英雄移动拾取）
        /// 覆盖棋盘中央 3x3 网格（去中心），英雄移动路径自动拾取掉落物
        /// </summary>
        public static readonly AnchorProfile[] PickupPoints = new AnchorProfile[]
        {
            new(-300, -530, 20, 20),   // 左上
            new(0,    -530, 20, 20),   // 上中
            new(300,  -530, 20, 20),   // 右上
            new(-300, -390, 20, 20),   // 左中
            new(0,    -390, 20, 20),   // 正中
            new(300,  -390, 20, 20),   // 右中
            new(-150, -250, 20, 20),   // 左下
            new(150,  -250, 20, 20),   // 右下
        };
    }
}