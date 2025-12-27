namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 手动设置数据类。
    /// </summary>
    public class ManualSettings : ICloneable, IEquatable<ManualSettings>
    {
        /// <summary>
        /// 选择的屏幕索引
        /// </summary>
        public int SelectedScreenIndex { get; set; }
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
        /// 商店售卖的第一张英雄的名称截图数据矩形
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_1 { get; set; }

        /// <summary>
        /// 商店售卖的第二张英雄的名称截图数据矩形
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_2 { get; set; }

        /// <summary>
        /// 商店售卖的第三张英雄名称的截图数据矩形
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_3 { get; set; }

        /// <summary>
        /// 商店售卖的第四张英雄名称的截图数据矩形
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_4 { get; set; }

        /// <summary>
        /// 商店售卖的第五张英雄名称的截图数据矩形
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_5 { get; set; }
      
        /// <summary>
        /// 刷新商店按钮的数据矩形
        /// </summary>
        public Rectangle RefreshStoreButtonRectangle { get; set; }

        /// <summary>
        /// 高亮显示商店需要购买的英雄按钮的数据矩形1
        /// </summary>
        public Rectangle HighLightRectangle_1 { get; set; }

        /// <summary>
        /// 高亮显示商店需要购买的英雄按钮的数据矩形2
        /// </summary>
        public Rectangle HighLightRectangle_2 { get; set; }

        /// <summary>
        /// 高亮显示商店需要购买的英雄按钮的数据矩形3
        /// </summary>
        public Rectangle HighLightRectangle_3 { get; set; }

        /// <summary>
        /// 高亮显示商店需要购买的英雄按钮的数据矩形4
        /// </summary>
        public Rectangle HighLightRectangle_4 { get; set; }

        /// <summary>
        /// 高亮显示商店需要购买的英雄按钮的数据矩形5
        /// </summary>
        public Rectangle HighLightRectangle_5 { get; set; }

        /// <summary>
        /// 是否启用高亮提示功能
        /// </summary>
        public bool IsUseHightLightPrompt { get; set; }       

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
        /// 半透明英雄面板英雄头像框的边长大小，单位：像素
        /// </summary>
        public int TransparentHeroPictureBoxSize { get; set; }

        /// <summary>
        /// 半透明英雄面板英雄头像框的水平间距，单位：像素
        /// </summary>
        public int TransparentHeroPictureBoxHorizontalSpacing { get; set; }

        /// <summary>
        /// 半透明英雄面板英雄头像框的垂直平间距，单位：像素
        /// </summary>
        public int TransparentHeroPanelsVerticalSpacing { get; set; }

        

        /// <summary>
        /// 创建默认设置的构造函数
        /// </summary>
        public ManualSettings()
        {
            HotKey1 = "F7";
            HotKey2 = "F8";
            HotKey3 = "Home";
            HotKey4 = "F9";                        
            SelectedScreenIndex = 0;
            HeroNameScreenshotRectangle_1 = new Rectangle(0, 0, 10, 10);
            HeroNameScreenshotRectangle_2 = new Rectangle(0, 0, 10, 10);
            HeroNameScreenshotRectangle_3 = new Rectangle(0, 0, 10, 10);
            HeroNameScreenshotRectangle_4 = new Rectangle(0, 0, 10, 10);
            HeroNameScreenshotRectangle_5 = new Rectangle(0, 0, 10, 10);                       
            RefreshStoreButtonRectangle = new Rectangle(0, 0, 10, 10);
            HighLightRectangle_1 = new Rectangle(0, 0, 10, 10);
            HighLightRectangle_2 = new Rectangle(0, 0, 10, 10);
            HighLightRectangle_3 = new Rectangle(0, 0, 10, 10);
            HighLightRectangle_4 = new Rectangle(0, 0, 10, 10);
            HighLightRectangle_5 = new Rectangle(0, 0, 10, 10);
            IsUseHightLightPrompt = false;
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
            MaxTimesWithoutHeroPurchase = 5;
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
            TransparentHeroPictureBoxSize = 36;
            TransparentHeroPictureBoxHorizontalSpacing = 0;
            TransparentHeroPanelsVerticalSpacing = 0;
            
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
                SelectedScreenIndex = this.SelectedScreenIndex,
                HeroNameScreenshotRectangle_1 = this.HeroNameScreenshotRectangle_1,
                HeroNameScreenshotRectangle_2 = this.HeroNameScreenshotRectangle_2,
                HeroNameScreenshotRectangle_3 = this.HeroNameScreenshotRectangle_3,
                HeroNameScreenshotRectangle_4 = this.HeroNameScreenshotRectangle_4,
                HeroNameScreenshotRectangle_5 = this.HeroNameScreenshotRectangle_5,             
                RefreshStoreButtonRectangle = this.RefreshStoreButtonRectangle,
                HighLightRectangle_1 = this.HighLightRectangle_1,
                HighLightRectangle_2 = this.HighLightRectangle_2,
                HighLightRectangle_3 = this.HighLightRectangle_3,
                HighLightRectangle_4 = this.HighLightRectangle_4,
                HighLightRectangle_5 = this.HighLightRectangle_5,
                IsUseHightLightPrompt = this.IsUseHightLightPrompt,             
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
                TransparentHeroPictureBoxSize = this.TransparentHeroPictureBoxSize,
                TransparentHeroPictureBoxHorizontalSpacing = this.TransparentHeroPictureBoxHorizontalSpacing,
                TransparentHeroPanelsVerticalSpacing = this.TransparentHeroPanelsVerticalSpacing,
                
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
                   SelectedScreenIndex == other.SelectedScreenIndex &&
                   HeroNameScreenshotRectangle_1 == other.HeroNameScreenshotRectangle_1 &&
                   HeroNameScreenshotRectangle_2 == other.HeroNameScreenshotRectangle_2 &&
                   HeroNameScreenshotRectangle_3 == other.HeroNameScreenshotRectangle_3 &&
                   HeroNameScreenshotRectangle_4 == other.HeroNameScreenshotRectangle_4 &&
                   HeroNameScreenshotRectangle_5 == other.HeroNameScreenshotRectangle_5 &&                  
                   RefreshStoreButtonRectangle == other.RefreshStoreButtonRectangle &&      
                   HighLightRectangle_1 == other.HighLightRectangle_1 &&
                   HighLightRectangle_2 == other.HighLightRectangle_2 &&
                   HighLightRectangle_3 == other.HighLightRectangle_3 &&
                   HighLightRectangle_4 == other.HighLightRectangle_4 &&
                   HighLightRectangle_5 == other.HighLightRectangle_5 &&
                   IsUseHightLightPrompt == other.IsUseHightLightPrompt &&               
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
                   TargetProcessId == other.TargetProcessId &&
                   TransparentHeroPictureBoxSize == other.TransparentHeroPictureBoxSize &&
                   TransparentHeroPictureBoxHorizontalSpacing == other.TransparentHeroPictureBoxHorizontalSpacing &&
                   TransparentHeroPanelsVerticalSpacing == other.TransparentHeroPanelsVerticalSpacing;
        }

        
        
    }
}
