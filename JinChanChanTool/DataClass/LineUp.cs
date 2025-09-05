namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 阵容数据对象
    /// </summary>
    public class LineUp
    {
        /// <summary>
        /// 阵容名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 阵容勾选状态
        /// </summary>
        public bool[,] Checked { get; set; }
    }
}
