namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 手动设置数据类。
    /// </summary>
    public class ManualSettings : ICloneable, IEquatable<ManualSettings>
    {
        /// <summary>
        /// 快捷键1-自动拿牌功能
        /// </summary>
        public string HotKey1 { get; set; }

        /// <summary>
        /// 快捷键2-自动刷新商店功能
        /// </summary>
        public string HotKey2 { get; set; }

        /// <summary>
        /// 快捷键3-显示/隐藏主窗口
        /// </summary>
        public string HotKey3 { get; set; }

        /// <summary>
        /// 快捷键4-长按自动D牌功能
        /// </summary>
        public string HotKey4 { get; set; }

        /// <summary>
        /// 商店售卖的第一张英雄的名称截图起点坐标X
        /// </summary>
        public int HeroNameScreenshotCoordinates_X1 { get; set; }

        /// <summary>
        /// 商店售卖的第二张英雄的名称截图起点坐标X
        /// </summary>
        public int HeroNameScreenshotCoordinates_X2 { get; set; }

        /// <summary>
        /// 商店售卖的第三张英雄名称的截图起点坐标X
        /// </summary>
        public int HeroNameScreenshotCoordinates_X3 { get; set; }

        /// <summary>
        /// 商店售卖的第四张英雄名称的截图起点坐标X
        /// </summary>
        public int HeroNameScreenshotCoordinates_X4 { get; set; }

        /// <summary>
        /// 商店售卖的第五张英雄名称的截图起点坐标X
        /// </summary>
        public int HeroNameScreenshotCoordinates_X5 { get; set; }

        /// <summary>
        /// 商店售卖的所有英雄的名称截图起点坐标Y
        /// </summary>
        public int HeroNameScreenshotCoordinates_Y { get; set; }

        /// <summary>
        /// 商店售卖的所有英雄的名称截图的宽度
        /// </summary>
        public int HeroNameScreenshotWidth { get; set; }

        /// <summary>
        /// 商店售卖的所有英雄的名称截图的高度
        /// </summary>
        public int HeroNameScreenshotHeight { get; set; }

        /// <summary>
        /// 刷新商店按钮的X坐标
        /// </summary>
        public int RefreshStoreButtonCoordinates_X { get; set; }

        /// <summary>
        /// 刷新商店按钮的Y坐标
        /// </summary>
        public int RefreshStoreButtonCoordinates_Y { get; set; }

        /// <summary>
        /// 单个子阵容的最大英雄数量
        /// </summary>
        public int MaxHerosCount { get; set; }

        /// <summary>
        /// 阵容下拉框展示的最大阵容数量
        /// </summary>
        public int MaxLineUpCount { get; set; }

        /// <summary>
        /// 是否启用用户高优先级模式（该模式下程序会尽可能的减少与用户的鼠标争夺）
        /// </summary>
        public bool IsHighUserPriority { get; set; }

        /// <summary>
        /// 是否自动停止购买英雄(购买英雄失败一定次数后自动停止购买英雄)
        /// </summary>
        public bool IsAutomaticStopHeroPurchase { get; set; }

        /// <summary>
        /// 是否自动停止刷新商店(刷新商店失败一定次数后自动停止刷新商店)
        /// </summary>
        public bool IsAutomaticStopRefreshStore { get; set; }

        /// <summary>
        /// 是否使用鼠标模拟购买英雄
        /// </summary>
        public bool IsMouseHeroPurchase { get; set; }

        /// <summary>
        /// 是否使用键盘模拟购买英雄
        /// </summary>
        public bool IsKeyboardHeroPurchase { get; set; }

        /// <summary>
        /// 键盘购买英雄按键1
        /// </summary>
        public string HeroPurchaseKey1 { get; set; }

        /// <summary>
        /// 键盘购买英雄按键2
        /// </summary>
        public string HeroPurchaseKey2 { get; set; }

        /// <summary>
        /// 键盘购买英雄按键3
        /// </summary>
        public string HeroPurchaseKey3 { get; set; }

        /// <summary>
        /// 键盘购买英雄按键4
        /// </summary>
        public string HeroPurchaseKey4 { get; set; }

        /// <summary>
        /// 键盘购买英雄按键5
        /// </summary>
        public string HeroPurchaseKey5 { get; set; }

        /// <summary>
        /// 是否使用鼠标模拟刷新商店
        /// </summary>
        public bool IsMouseRefreshStore { get; set; }

        /// <summary>
        /// 是否使用键盘模拟刷新商店
        /// </summary>
        public bool IsKeyboardRefreshStore { get; set; }

        /// <summary>
        /// 键盘刷新商店按键
        /// </summary>
        public string RefreshStoreKey { get; set; }

        /// <summary>
        /// 是否使用CPU进行OCR识别
        /// </summary>
        public bool IsUseCPUForInference { get; set; }

        /// <summary>
        /// 是否使用GPU进行OCR识别
        /// </summary>
        public bool IsUseGPUForInference { get; set; }

        /// <summary>
        /// 是否使用固定坐标
        /// </summary>
        public bool IsUseFixedCoordinates { get; set; }

        /// <summary>
        /// 是否使用动态坐标
        /// </summary>
        public bool IsUseDynamicCoordinates { get; set; }

        /// <summary>
        /// 允许的最大购买英雄失败次数，自动停止购买英雄功能开启时生效，超过该次数则自动停止购买英雄。
        /// </summary>
        public int MaxTimesWithoutHeroPurchase { get; set; }

        /// <summary>
        /// 允许的最大刷新商店失败次数，自动停止刷新商店功能开启时生效，超过该次数则自动停止刷新商店。
        /// </summary>
        public int MaxTimesWithoutRefreshStore { get; set; }

        /// <summary>
        /// 操作等待时间，单位毫秒
        /// </summary>
        public int DelayAfterOperation { get; set; }

        /// <summary>
        /// CPU推理模式下，刷新商店后等待时间，单位毫秒
        /// </summary>
        public int DelayAfterRefreshStore_CPU { get; set; }

        /// <summary>
        /// GPU推理模式下，刷新商店后等待时间，单位毫秒
        /// </summary>
        public int DelayAfterRefreshStore_GPU { get; set; }

        /// <summary>
        /// 是否使用英雄选择面板
        /// </summary>
        public bool IsUseSelectForm { get; set; }

        /// <summary>
        /// 是否使用阵容选择面板
        /// </summary>
        public bool IsUseLineUpForm { get; set; }

        /// <summary>
        /// 是否使用状态显示面板
        /// </summary>
        public bool IsUseStatusOverlayForm { get; set; }

        /// <summary>
        /// 是否使用输出窗口
        /// </summary>
        public bool IsUseOutputForm { get; set; }

        /// <summary>
        /// 是否在识别到错误字符时停止刷新商店
        /// </summary>
        public bool IsStopRefreshStoreWhenErrorCharacters { get; set; }

        /// <summary>
        /// 推荐装备更新频率，单位：小时
        /// </summary>
        public int UpdateEquipmentInterval { get; set; }

        /// <summary>
        /// 是否自动更新推荐装备
        /// </summary>
        public bool IsAutomaticUpdateEquipment { get; set; }

        /// <summary>
        /// 自动模式下要锁定的目标进程的名称。
        /// </summary>
        public string TargetProcessName { get; set; }

        /// <summary>
        /// 用于多模拟器窗口时，用户在UI中精确选择的进程ID。优先级高于按名称查找。
        /// </summary>
        public int TargetProcessId { get; set; }

        /// <summary>
        /// 创建默认设置的构造函数
        /// </summary>
        public ManualSettings()
        {
            HotKey1 = "F7";
            HotKey2 = "F8";
            HotKey3 = "Home";
            HotKey4 = "F9";
            MaxHerosCount = 10;
            MaxLineUpCount = 10;
            HeroNameScreenshotCoordinates_X1 = 549;
            HeroNameScreenshotCoordinates_X2 = 755;
            HeroNameScreenshotCoordinates_X3 = 961;
            HeroNameScreenshotCoordinates_X4 = 1173;
            HeroNameScreenshotCoordinates_X5 = 1380;
            HeroNameScreenshotCoordinates_Y = 1029;
            HeroNameScreenshotWidth = 146;
            HeroNameScreenshotHeight = 31;
            RefreshStoreButtonCoordinates_X = 441;
            RefreshStoreButtonCoordinates_Y = 1027;
            IsHighUserPriority = true;
            IsAutomaticStopHeroPurchase = true;
            IsAutomaticStopRefreshStore = true;
            IsMouseHeroPurchase = true;
            IsKeyboardHeroPurchase = false;
            HeroPurchaseKey1 = "Q";
            HeroPurchaseKey2 = "W";
            HeroPurchaseKey3 = "E";
            HeroPurchaseKey4 = "R";
            HeroPurchaseKey5 = "T";
            IsMouseRefreshStore = true;
            IsKeyboardRefreshStore = false;
            RefreshStoreKey = "D";
            IsUseCPUForInference = true;
            IsUseGPUForInference = false;
            IsUseFixedCoordinates = true;
            IsUseDynamicCoordinates = false;
            MaxTimesWithoutHeroPurchase = 3;
            MaxTimesWithoutRefreshStore = 3;
            DelayAfterOperation = 20;
            DelayAfterRefreshStore_CPU = 308;
            DelayAfterRefreshStore_GPU = 308;
            IsUseSelectForm = true;
            IsUseLineUpForm = true;
            IsUseStatusOverlayForm = true;
            IsUseOutputForm = true;
            IsStopRefreshStoreWhenErrorCharacters = true;
            UpdateEquipmentInterval = 12;
            IsAutomaticUpdateEquipment = true;
            TargetProcessName = "";
            TargetProcessId = 0;
        }

        /// <summary>
        /// 克隆函数，返回一个object对象。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ManualSettings
            {
                HotKey1 = this.HotKey1,
                HotKey2 = this.HotKey2,
                HotKey3 = this.HotKey3,
                HotKey4 = this.HotKey4,
                HeroNameScreenshotCoordinates_X1 = this.HeroNameScreenshotCoordinates_X1,
                HeroNameScreenshotCoordinates_X2 = this.HeroNameScreenshotCoordinates_X2,
                HeroNameScreenshotCoordinates_X3 = this.HeroNameScreenshotCoordinates_X3,
                HeroNameScreenshotCoordinates_X4 = this.HeroNameScreenshotCoordinates_X4,
                HeroNameScreenshotCoordinates_X5 = this.HeroNameScreenshotCoordinates_X5,
                HeroNameScreenshotCoordinates_Y = this.HeroNameScreenshotCoordinates_Y,
                HeroNameScreenshotWidth = this.HeroNameScreenshotWidth,
                HeroNameScreenshotHeight = this.HeroNameScreenshotHeight,
                RefreshStoreButtonCoordinates_X = this.RefreshStoreButtonCoordinates_X,
                RefreshStoreButtonCoordinates_Y = this.RefreshStoreButtonCoordinates_Y,
                MaxHerosCount = this.MaxHerosCount,
                MaxLineUpCount = this.MaxLineUpCount,
                IsHighUserPriority = this.IsHighUserPriority,
                IsAutomaticStopHeroPurchase = this.IsAutomaticStopHeroPurchase,
                IsAutomaticStopRefreshStore = this.IsAutomaticStopRefreshStore,
                IsMouseHeroPurchase = this.IsMouseHeroPurchase,
                IsKeyboardHeroPurchase =this.IsKeyboardHeroPurchase,
                HeroPurchaseKey1 =this.HeroPurchaseKey1,
                HeroPurchaseKey2 =this.HeroPurchaseKey2,
                HeroPurchaseKey3 =this.HeroPurchaseKey3,
                HeroPurchaseKey4 =this.HeroPurchaseKey4,
                HeroPurchaseKey5 =this.HeroPurchaseKey5,
                IsMouseRefreshStore =this.IsMouseRefreshStore,
                IsKeyboardRefreshStore =this.IsKeyboardRefreshStore,
                RefreshStoreKey =this.RefreshStoreKey,
                IsUseCPUForInference = this.IsUseCPUForInference,
                IsUseGPUForInference = this.IsUseGPUForInference,
                IsUseFixedCoordinates = this.IsUseFixedCoordinates,
                IsUseDynamicCoordinates =this.IsUseDynamicCoordinates,               
                MaxTimesWithoutHeroPurchase = this.MaxTimesWithoutHeroPurchase,
                MaxTimesWithoutRefreshStore = this.MaxTimesWithoutRefreshStore,
                DelayAfterOperation = this.DelayAfterOperation,
                DelayAfterRefreshStore_CPU = this.DelayAfterRefreshStore_CPU,
                DelayAfterRefreshStore_GPU = this.DelayAfterRefreshStore_GPU,               
                IsUseSelectForm = this.IsUseSelectForm,
                IsUseLineUpForm = this.IsUseLineUpForm,
                IsUseStatusOverlayForm = this.IsUseStatusOverlayForm,
                IsUseOutputForm = this.IsUseOutputForm,
                IsStopRefreshStoreWhenErrorCharacters =this.IsStopRefreshStoreWhenErrorCharacters,               
                UpdateEquipmentInterval = this.UpdateEquipmentInterval,
                IsAutomaticUpdateEquipment = this.IsAutomaticUpdateEquipment,
                TargetProcessName = this.TargetProcessName,
                TargetProcessId = this.TargetProcessId,
            };
        }

        /// <summary>
        /// 比较函数，比较二者的指定属性是否相等。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ManualSettings other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return HotKey1 == other.HotKey1 &&
                   HotKey2 == other.HotKey2 &&
                   HotKey3 == other.HotKey3 &&
                   HotKey4 == other.HotKey4 &&
                   HeroNameScreenshotCoordinates_X1 == other.HeroNameScreenshotCoordinates_X1 &&
                   HeroNameScreenshotCoordinates_X2 == other.HeroNameScreenshotCoordinates_X2 &&
                   HeroNameScreenshotCoordinates_X3 == other.HeroNameScreenshotCoordinates_X3 &&
                   HeroNameScreenshotCoordinates_X4 == other.HeroNameScreenshotCoordinates_X4 &&
                   HeroNameScreenshotCoordinates_X5 == other.HeroNameScreenshotCoordinates_X5 &&
                   HeroNameScreenshotCoordinates_Y == other.HeroNameScreenshotCoordinates_Y &&
                   HeroNameScreenshotWidth == other.HeroNameScreenshotWidth &&
                   HeroNameScreenshotHeight == other.HeroNameScreenshotHeight &&
                   RefreshStoreButtonCoordinates_X == other.RefreshStoreButtonCoordinates_X &&
                   RefreshStoreButtonCoordinates_Y == other.RefreshStoreButtonCoordinates_Y &&
                   MaxHerosCount == other.MaxHerosCount &&
                   MaxLineUpCount == other.MaxLineUpCount &&
                   IsHighUserPriority == other.IsHighUserPriority &&
                   IsAutomaticStopHeroPurchase == other.IsAutomaticStopHeroPurchase &&
                   IsAutomaticStopRefreshStore == other.IsAutomaticStopRefreshStore &&
                   IsMouseHeroPurchase == other.IsMouseHeroPurchase &&
                   IsKeyboardHeroPurchase == other.IsKeyboardHeroPurchase &&
                   HeroPurchaseKey1 == other.HeroPurchaseKey1 &&
                   HeroPurchaseKey2 == other.HeroPurchaseKey2 &&
                   HeroPurchaseKey3 == other.HeroPurchaseKey3 &&
                   HeroPurchaseKey4 == other.HeroPurchaseKey4 &&
                   HeroPurchaseKey5 == other.HeroPurchaseKey5 &&
                   IsMouseRefreshStore == other.IsMouseRefreshStore &&
                   IsKeyboardRefreshStore == other.IsKeyboardRefreshStore &&
                   RefreshStoreKey == other.RefreshStoreKey &&
                   IsUseCPUForInference == other.IsUseCPUForInference &&
                   IsUseGPUForInference == other.IsUseGPUForInference &&
                   IsUseFixedCoordinates == other.IsUseFixedCoordinates &&
                   IsUseDynamicCoordinates == other.IsUseDynamicCoordinates &&
                   MaxTimesWithoutHeroPurchase == other.MaxTimesWithoutHeroPurchase &&
                   MaxTimesWithoutRefreshStore == other.MaxTimesWithoutRefreshStore &&
                   DelayAfterOperation == other.DelayAfterOperation &&
                   DelayAfterRefreshStore_CPU == other.DelayAfterRefreshStore_CPU &&
                   DelayAfterRefreshStore_GPU == other.DelayAfterRefreshStore_GPU &&
                   IsUseSelectForm == other.IsUseSelectForm &&
                   IsUseLineUpForm == other.IsUseLineUpForm &&
                   IsUseStatusOverlayForm == other.IsUseStatusOverlayForm &&
                   IsUseOutputForm == other.IsUseOutputForm &&
                   IsStopRefreshStoreWhenErrorCharacters == other.IsStopRefreshStoreWhenErrorCharacters &&
                   UpdateEquipmentInterval == other.UpdateEquipmentInterval &&
                   IsAutomaticUpdateEquipment == other.IsAutomaticUpdateEquipment &&
                   TargetProcessName == other.TargetProcessName &&
                   TargetProcessId == other.TargetProcessId;
        }

        
        
    }
}
