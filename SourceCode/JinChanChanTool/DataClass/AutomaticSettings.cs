using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.DataClass
{
    public class AutomaticSettings : ICloneable, IEquatable<AutomaticSettings>
    {
        /// <summary>
        /// 商店售卖的第一张英雄的名称截图起点坐标X
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_1 { get; set; }

        /// <summary>
        /// 商店售卖的第二张英雄的名称截图起点坐标X
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_2 { get; set; }

        /// <summary>
        /// 商店售卖的第三张英雄的名称截图起点坐标X
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_3 { get; set; }

        /// <summary>
        /// 商店售卖的第四张英雄的名称截图起点坐标X
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_4 { get; set; }

        /// <summary>
        /// 商店售卖的第五张英雄的名称截图起点坐标X
        /// </summary>
        public Rectangle HeroNameScreenshotRectangle_5 { get; set; }

        /// <summary>
        /// 刷新商店按钮的数据矩形
        /// </summary>
        public Rectangle RefreshStoreButtonRectangle { get; set; }

        /// <summary>
        /// 英雄选择面板位置
        /// </summary>
        public Point SelectFormLocation { get; set; }

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
        public DateTime EquipmentLastUpdateTime { get; set; }

        /// <summary>
        /// 上次选择的赛季
        /// </summary>
        public string SelectedSeason { get; set; }

        /// <summary>
        /// 当前选择的阵容下标
        /// </summary>
        public int SelectedLineUpIndex { get; set; }

        /// <summary>
        /// 创建默认设置的构造函数
        /// </summary>
        public AutomaticSettings()
        {
            HeroNameScreenshotRectangle_1 = new Rectangle(0, 0, 10, 10);
            HeroNameScreenshotRectangle_2 = new Rectangle(0, 0, 10, 10);
            HeroNameScreenshotRectangle_3 = new Rectangle(0, 0, 10, 10);
            HeroNameScreenshotRectangle_4 = new Rectangle(0, 0, 10, 10);
            HeroNameScreenshotRectangle_5 = new Rectangle(0, 0, 10, 10);
            RefreshStoreButtonRectangle = new Rectangle(0, 0, 10, 10);
            SelectFormLocation = new Point(-1, -1);
            LineUpFormLocation = new Point(-1, -1);
            StatusOverlayFormLocation = new Point(-1, -1);
            EquipmentLastUpdateTime = new DateTime(2025, 11, 1, 2, 44, 0);
            SelectedSeason = "英雄联盟传奇";
            SelectedLineUpIndex = 0;
        }

        /// <summary>
        /// 克隆函数，返回一个object对象。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new AutomaticSettings
            {
                HeroNameScreenshotRectangle_1 = this.HeroNameScreenshotRectangle_1,
                HeroNameScreenshotRectangle_2 = this.HeroNameScreenshotRectangle_2,
                HeroNameScreenshotRectangle_3 = this.HeroNameScreenshotRectangle_3,
                HeroNameScreenshotRectangle_4 = this.HeroNameScreenshotRectangle_4,
                HeroNameScreenshotRectangle_5 = this.HeroNameScreenshotRectangle_5,
                RefreshStoreButtonRectangle = this.RefreshStoreButtonRectangle,
                SelectFormLocation = this.SelectFormLocation,
                LineUpFormLocation = this.LineUpFormLocation,
                StatusOverlayFormLocation = this.StatusOverlayFormLocation,
                EquipmentLastUpdateTime = this.EquipmentLastUpdateTime,
                SelectedSeason = this.SelectedSeason,
                SelectedLineUpIndex = this.SelectedLineUpIndex
            };
        }

        /// <summary>
        /// 比较函数，比较二者的指定属性是否相等。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AutomaticSettings other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

           return  HeroNameScreenshotRectangle_1 == other.HeroNameScreenshotRectangle_1 &&
                   HeroNameScreenshotRectangle_2 == other.HeroNameScreenshotRectangle_2 &&
                   HeroNameScreenshotRectangle_3 == other.HeroNameScreenshotRectangle_3 &&
                   HeroNameScreenshotRectangle_4 == other.HeroNameScreenshotRectangle_4 &&
                   HeroNameScreenshotRectangle_5 == other.HeroNameScreenshotRectangle_5 &&
                   RefreshStoreButtonRectangle == other.RefreshStoreButtonRectangle &&
                   SelectFormLocation == other.SelectFormLocation &&
                   LineUpFormLocation == other.LineUpFormLocation &&
                   StatusOverlayFormLocation == other.StatusOverlayFormLocation &&
                   EquipmentLastUpdateTime == other.EquipmentLastUpdateTime&&
                   SelectedSeason == other.SelectedSeason&&
                   SelectedLineUpIndex == other.SelectedLineUpIndex;
        }                
    }
}
