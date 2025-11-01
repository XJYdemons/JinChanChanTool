namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 应用设置对象，提供克隆、比较方法，构造函数提供默认设置。
    /// </summary>
    public class AppConfig : ICloneable, IEquatable<AppConfig>
    {
        /// <summary>
        /// 克隆函数，返回一个object对象。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new AppConfig
            {
                HotKey1 = this.HotKey1,
                HotKey2 = this.HotKey2,
                HotKey3 = this.HotKey3,
                HotKey4 = this.HotKey4,
                StartPoint_CardScreenshotX1 = this.StartPoint_CardScreenshotX1,
                StartPoint_CardScreenshotX2 = this.StartPoint_CardScreenshotX2,
                StartPoint_CardScreenshotX3 = this.StartPoint_CardScreenshotX3,
                StartPoint_CardScreenshotX4 = this.StartPoint_CardScreenshotX4,
                StartPoint_CardScreenshotX5 = this.StartPoint_CardScreenshotX5,
                StartPoint_CardScreenshotY = this.StartPoint_CardScreenshotY,
                Width_CardScreenshot = this.Width_CardScreenshot,
                Height_CardScreenshot = this.Height_CardScreenshot,
                Point_RefreshStoreX = this.Point_RefreshStoreX,
                Point_RefreshStoreY = this.Point_RefreshStoreY,
                MaxOfChoices = this.MaxOfChoices,
                CountOfLine = this.CountOfLine,
                HighCursorcontrol = this.HighCursorcontrol,
                AutoStopGet = this.AutoStopGet,
                AutoStopRefresh = this.AutoStopRefresh,
                MouseGetCard = this.MouseGetCard,
                KeyboardGetCard =this.KeyboardGetCard,
                GetCardKey1 =this.GetCardKey1,
                GetCardKey2 =this.GetCardKey2,
                GetCardKey3 =this.GetCardKey3,
                GetCardKey4 =this.GetCardKey4,
                GetCardKey5 =this.GetCardKey5,
                MouseRefresh =this.MouseRefresh,
                KeyboardRefresh =this.KeyboardRefresh,
                RefreshKey =this.RefreshKey,
                UseCPU = this.UseCPU,
                UseGPU = this.UseGPU,
                UseFixedCoordinates = this.UseFixedCoordinates,
                UseDynamicCoordinates =this.UseDynamicCoordinates,               
                MaxTimesWithoutGetCard = this.MaxTimesWithoutGetCard,
                MaxTimesWithoutRefresh = this.MaxTimesWithoutRefresh,
                DelayAfterMouseOperation = this.DelayAfterMouseOperation,
                CPUDelayAfterRefreshStore = this.CPUDelayAfterRefreshStore,
                GPUDelayAfterRefreshStore = this.GPUDelayAfterRefreshStore,               
                UseSelectorForm = this.UseSelectorForm,
                UseLineUpForm = this.UseLineUpForm,
                UseStatusOverlayForm = this.UseStatusOverlayForm,
                UseErrorShowForm = this.UseErrorShowForm,
                StopRefreshWhenErrorCharacters =this.StopRefreshWhenErrorCharacters,               
                UpdateEquipmentInterval = this.UpdateEquipmentInterval,
                IsAutoUpdateEquipment = this.IsAutoUpdateEquipment,
                TargetProcessName = this.TargetProcessName,
                TargetProcessId = this.TargetProcessId,
            };
        }

        /// <summary>
        /// 比较函数，比较二者的指定属性是否相等。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AppConfig other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return HotKey1 == other.HotKey1 &&
                   HotKey2 == other.HotKey2 &&
                   HotKey3 == other.HotKey3 &&
                   HotKey4 == other.HotKey4 &&
                   StartPoint_CardScreenshotX1 == other.StartPoint_CardScreenshotX1 &&
                   StartPoint_CardScreenshotX2 == other.StartPoint_CardScreenshotX2 &&
                   StartPoint_CardScreenshotX3 == other.StartPoint_CardScreenshotX3 &&
                   StartPoint_CardScreenshotX4 == other.StartPoint_CardScreenshotX4 &&
                   StartPoint_CardScreenshotX5 == other.StartPoint_CardScreenshotX5 &&
                   StartPoint_CardScreenshotY == other.StartPoint_CardScreenshotY &&
                   Width_CardScreenshot == other.Width_CardScreenshot &&
                   Height_CardScreenshot == other.Height_CardScreenshot &&
                   Point_RefreshStoreX == other.Point_RefreshStoreX &&
                   Point_RefreshStoreY == other.Point_RefreshStoreY &&
                   MaxOfChoices == other.MaxOfChoices &&
                   CountOfLine == other.CountOfLine &&
                   HighCursorcontrol == other.HighCursorcontrol &&
                   AutoStopGet == other.AutoStopGet &&
                   AutoStopRefresh == other.AutoStopRefresh &&
                   MouseGetCard == other.MouseGetCard &&
                   KeyboardGetCard == other.KeyboardGetCard &&
                   GetCardKey1 == other.GetCardKey1 &&
                   GetCardKey2 == other.GetCardKey2 &&
                   GetCardKey3 == other.GetCardKey3 &&
                   GetCardKey4 == other.GetCardKey4 &&
                   GetCardKey5 == other.GetCardKey5 &&
                   MouseRefresh == other.MouseRefresh &&
                   KeyboardRefresh == other.KeyboardRefresh &&
                   RefreshKey == other.RefreshKey &&
                   UseCPU == other.UseCPU &&
                   UseGPU == other.UseGPU &&
                   UseFixedCoordinates == other.UseFixedCoordinates &&
                   UseDynamicCoordinates == other.UseDynamicCoordinates &&
                   MaxTimesWithoutGetCard == other.MaxTimesWithoutGetCard &&
                   MaxTimesWithoutRefresh == other.MaxTimesWithoutRefresh &&
                   DelayAfterMouseOperation == other.DelayAfterMouseOperation &&
                   CPUDelayAfterRefreshStore == other.CPUDelayAfterRefreshStore &&
                   GPUDelayAfterRefreshStore == other.GPUDelayAfterRefreshStore &&
                   UseSelectorForm == other.UseSelectorForm &&
                   UseLineUpForm == other.UseLineUpForm &&
                   UseStatusOverlayForm == other.UseStatusOverlayForm &&
                   UseErrorShowForm == other.UseErrorShowForm &&
                   StopRefreshWhenErrorCharacters == other.StopRefreshWhenErrorCharacters &&
                   UpdateEquipmentInterval == other.UpdateEquipmentInterval &&
                   IsAutoUpdateEquipment == other.IsAutoUpdateEquipment &&
                   TargetProcessName == other.TargetProcessName &&
                   TargetProcessId == other.TargetProcessId;
        }

        /// <summary>
        /// 快捷键1
        /// </summary>
        public  string HotKey1 { get; set; }

        /// <summary>
        /// 快捷键2
        /// </summary>
        public  string HotKey2 { get; set; }     

        /// <summary>
        /// 快捷键3
        /// </summary>
        public  string HotKey3 { get; set; }

        /// <summary>
        /// 快捷键4
        /// </summary>
        public string HotKey4 { get; set; }

        /// <summary>
        /// 商店第一张卡的起点坐标X
        /// </summary>
        public int StartPoint_CardScreenshotX1 { get; set; }

        /// <summary>
        /// 商店第二张卡的起点坐标X
        /// </summary>
        public int StartPoint_CardScreenshotX2 { get; set; }

        /// <summary>
        /// 商店第三张卡的起点坐标X
        /// </summary>
        public int StartPoint_CardScreenshotX3 { get; set; }

        /// <summary>
        /// 商店第四张卡的起点坐标X
        /// </summary>
        public int StartPoint_CardScreenshotX4 { get; set; }

        /// <summary>
        /// 商店第五张卡的起点坐标X
        /// </summary>
        public int StartPoint_CardScreenshotX5 { get; set; }

        /// <summary>
        /// 商店所有卡的起点坐标Y
        /// </summary>
        public int StartPoint_CardScreenshotY { get; set; }

        /// <summary>
        /// 商店卡的宽度
        /// </summary>
        public int Width_CardScreenshot { get; set; }

        /// <summary>
        /// 商店卡的高度
        /// </summary>
        public int Height_CardScreenshot { get; set; } 
        
        /// <summary>
        /// 刷新按钮X坐标
        /// </summary>
        public int Point_RefreshStoreX { get; set; }

        /// <summary>
        /// 刷新按钮Y坐标
        /// </summary>
        public int Point_RefreshStoreY { get; set; }

        /// <summary>
        /// 最大选择英雄数量
        /// </summary>
        public int MaxOfChoices { get; set; }

        /// <summary>
        /// 阵容数量
        /// </summary>
        public int CountOfLine { get; set; }

        /// <summary>
        /// 用户高光标控制权
        /// </summary>
        public bool HighCursorcontrol { get; set; }

        /// <summary>
        /// 自动停止拿牌
        /// </summary>
        public bool AutoStopGet { get; set; }
            
        /// <summary>
        /// 自动停止刷新商店
        /// </summary>
        public bool AutoStopRefresh { get; set; }

        /// <summary>
        /// 鼠标模拟拿牌
        /// </summary>
        public bool MouseGetCard {  get; set; }

        /// <summary>
        /// 键盘模拟拿牌
        /// </summary>
        public bool KeyboardGetCard { get; set; }

        /// <summary>
        /// 拿牌按键1
        /// </summary>
        public string GetCardKey1 { get; set; }

        /// <summary>
        /// 拿牌按键2
        /// </summary>
        public string GetCardKey2 { get; set; }

        /// <summary>
        /// 拿牌按键3
        /// </summary>
        public string GetCardKey3 { get; set; }

        /// <summary>
        /// 拿牌按键4
        /// </summary>
        public string GetCardKey4 { get; set; }

        /// <summary>
        /// 拿牌按键5
        /// </summary>
        public string GetCardKey5 { get; set; }

        /// <summary>
        /// 鼠标模拟刷新商店
        /// </summary>
        public bool MouseRefresh { get; set; }

        /// <summary>
        /// 键盘模拟刷新商店
        /// </summary>
        public bool KeyboardRefresh { get; set; }

        /// <summary>
        /// 刷新商店按键
        /// </summary>
        public string RefreshKey { get; set; }

        /// <summary>
        /// CPU推理
        /// </summary>
        public bool UseCPU { get; set; }

        /// <summary>
        /// GPU推理
        /// </summary>
        public bool UseGPU { get; set; }

        /// <summary>
        /// 使用固定坐标
        /// </summary>
        public bool UseFixedCoordinates { get; set; }

        /// <summary>
        /// 使用动态坐标
        /// </summary>
        public bool UseDynamicCoordinates { get; set; }

        /// <summary>
        /// 最大未拿牌次数，超过则自动停止拿牌
        /// </summary>
        public int MaxTimesWithoutGetCard { get; set; }

        /// <summary>
        /// 最大未刷新次数，超过则自动停止刷新
        /// </summary>
        public int MaxTimesWithoutRefresh { get; set; }

        /// <summary>
        /// 键鼠操作等待时间，单位毫秒
        /// </summary>
        public int DelayAfterMouseOperation { get; set; }

        /// <summary>
        /// CPU推理模式下，刷新商店后等待时间，单位毫秒
        /// </summary>
        public int CPUDelayAfterRefreshStore { get; set; }

        /// <summary>
        /// GPU推理模式下，刷新商店后等待时间，单位毫秒
        /// </summary>
        public int GPUDelayAfterRefreshStore { get; set; }

        /// <summary>
        /// 使用英雄选择面板
        /// </summary>
        public bool UseSelectorForm {  get; set; }

        /// <summary>
        /// 使用阵容选择面板
        /// </summary>
        public bool UseLineUpForm { get; set; }

        /// <summary>
        /// 使用状态显示面板
        /// </summary>
        public bool UseStatusOverlayForm {  get; set; }

        /// <summary>
        /// 使用错误信息展示窗口
        /// </summary>
        public bool UseErrorShowForm { get; set; }

        /// <summary>
        /// 当识别到错误字符时停止刷新商店
        /// </summary>
        public bool StopRefreshWhenErrorCharacters {  get; set; }

        /// <summary>
        /// 推荐装备更新频率，单位：小时
        /// </summary>
        public int UpdateEquipmentInterval { get; set; }

        /// <summary>
        /// 是否自动更新推荐装备
        /// </summary>
        public bool IsAutoUpdateEquipment { get; set; }

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
        public AppConfig()
        {
            HotKey1 = "F7";
            HotKey2 = "F8";
            HotKey3 = "Home";
            HotKey4 = "F9";
            MaxOfChoices = 10;
            CountOfLine = 10;
            StartPoint_CardScreenshotX1 = 549;
            StartPoint_CardScreenshotX2 = 755;
            StartPoint_CardScreenshotX3 = 961;
            StartPoint_CardScreenshotX4 = 1173;
            StartPoint_CardScreenshotX5 = 1380;
            StartPoint_CardScreenshotY = 1029;
            Width_CardScreenshot = 146;
            Height_CardScreenshot = 31;
            Point_RefreshStoreX = 441;
            Point_RefreshStoreY = 1027;
            HighCursorcontrol = true;
            AutoStopGet = true;
            AutoStopRefresh = true;
            MouseGetCard = true;
            KeyboardGetCard = false;
            GetCardKey1 = "Q";
            GetCardKey2 = "W";
            GetCardKey3 = "E";
            GetCardKey4 = "R";
            GetCardKey5 = "T";
            MouseRefresh = true;
            KeyboardRefresh = false;
            RefreshKey = "D";
            UseCPU = true;
            UseGPU = false;
            UseFixedCoordinates = true;
            UseDynamicCoordinates = false;          
            MaxTimesWithoutGetCard = 3;
            MaxTimesWithoutRefresh = 3;
            DelayAfterMouseOperation = 20;
            CPUDelayAfterRefreshStore = 308;
            GPUDelayAfterRefreshStore = 308;         
            UseSelectorForm = true;
            UseLineUpForm = true;
            UseStatusOverlayForm = true;
            UseErrorShowForm = true;
            StopRefreshWhenErrorCharacters = true;          
            UpdateEquipmentInterval = 12;
            IsAutoUpdateEquipment = true;
            TargetProcessName = "";
            TargetProcessId = 0;
        }
        }
}
