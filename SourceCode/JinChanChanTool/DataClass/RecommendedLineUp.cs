using System.Text.Json.Serialization;

namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 推荐阵容评级枚举
    /// </summary>
    public enum LineUpTier
    {
        S,
        A,
        B,
        C,
        D
    }

    /// <summary>
    /// 推荐阵容数据对象
    /// </summary>
    public class RecommendedLineUp
    {
        /// <summary>
        /// 阵容名称
        /// </summary>
        public string LineUpName { get; set; } = "";

        /// <summary>
        /// 阵容内容（英雄与装备列表）
        /// </summary>
        public List<LineUpUnit> LineUpUnits { get; set; } = [];

        /// <summary>
        /// 阵容评级 (S, A, B,C, D)
        /// </summary>
        public LineUpTier Tier { get; set; } = LineUpTier.A;

        /// <summary>
        /// 胜率 (0-100的百分比)
        /// </summary>
        public double WinRate { get; set; }

        /// <summary>
        /// 平均名次 (1.0-8.0)
        /// </summary>
        public double AverageRank { get; set; }

        /// <summary>
        /// 选取率 (0-100的百分比)
        /// </summary>
        public double PickRate { get; set; }

        /// <summary>
        /// 前四率 (0-100的百分比)
        /// </summary>
        public double TopFourRate { get; set; }

        /// <summary>
        /// 阵容标签列表（如：困难、速9、8级等）
        /// </summary>
        public List<string> Tags { get; set; } = [];

        /// <summary>
        /// 阵容描述/备注
        /// </summary>                
        public string Description { get; set; } = "";
              
        /// <summary>
        /// 获取优先级的显示文本
        /// </summary>
        public string GetTierDisplayText()
        {
            return Tier switch
            {
                LineUpTier.S => "S",
                LineUpTier.A => "A",
                LineUpTier.B => "B",
                LineUpTier.C => "C",
                LineUpTier.D => "D",
                _ => "T10086"
            };
        }
    }
}
