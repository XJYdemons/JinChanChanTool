using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.DataClass
{
    public class AutoConfig : ICloneable, IEquatable<AutoConfig>
    {
        /// <summary>
        /// 克隆函数，返回一个object对象。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new AutoConfig
            {
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
                SelectorFormLocation = this.SelectorFormLocation,
                LineUpFormLocation = this.LineUpFormLocation,
                StatusOverlayFormLocation = this.StatusOverlayFormLocation,
                LastUpdateTime = this.LastUpdateTime,
                SelectSeason = this.SelectSeason,
                SelectedLineUpIndex = this.SelectedLineUpIndex
            };
        }

        /// <summary>
        /// 比较函数，比较二者的指定属性是否相等。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AutoConfig other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

           return  StartPoint_CardScreenshotX1 == other.StartPoint_CardScreenshotX1 &&
                   StartPoint_CardScreenshotX2 == other.StartPoint_CardScreenshotX2 &&
                   StartPoint_CardScreenshotX3 == other.StartPoint_CardScreenshotX3 &&
                   StartPoint_CardScreenshotX4 == other.StartPoint_CardScreenshotX4 &&
                   StartPoint_CardScreenshotX5 == other.StartPoint_CardScreenshotX5 &&
                   StartPoint_CardScreenshotY == other.StartPoint_CardScreenshotY &&
                   Width_CardScreenshot == other.Width_CardScreenshot &&
                   Height_CardScreenshot == other.Height_CardScreenshot &&
                   Point_RefreshStoreX == other.Point_RefreshStoreX &&
                   Point_RefreshStoreY == other.Point_RefreshStoreY &&
                   SelectorFormLocation == other.SelectorFormLocation &&
                   LineUpFormLocation == other.LineUpFormLocation &&
                   StatusOverlayFormLocation == other.StatusOverlayFormLocation &&
                   LastUpdateTime == other.LastUpdateTime&&
                   SelectSeason == other.SelectSeason&&
                   SelectedLineUpIndex == other.SelectedLineUpIndex;
        }
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
        /// 英雄选择面板位置
        /// </summary>
        public Point SelectorFormLocation { get; set; }

        /// <summary>
        /// 阵容选择面板位置
        /// </summary>        
        public Point LineUpFormLocation { get; set; }

        /// <summary>
        /// 状态显示面板位置
        /// </summary>
        public Point StatusOverlayFormLocation { get; set; }
       
        /// <summary>
        /// 推荐装备最后更新时间，精确到分钟，用于判断是否需要更新配置
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 上次选择的赛季
        /// </summary>
        public string SelectSeason { get; set; }

        /// <summary>
        /// 当前选择的阵容下标
        /// </summary>
        public int SelectedLineUpIndex { get; set; }

        /// <summary>
        /// 创建默认设置的构造函数
        /// </summary>
        public AutoConfig()
        {
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
            SelectorFormLocation = new Point(-1, -1);
            LineUpFormLocation = new Point(-1, -1);
            StatusOverlayFormLocation = new Point(-1, -1);          
            LastUpdateTime = new DateTime(2025, 11, 1, 2, 44, 0);
            SelectSeason = "英雄联盟传奇";
            SelectedLineUpIndex = 0;
        }
    }
}
