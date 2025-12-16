namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 纠正结果对象
    /// </summary>
    public class ResultMapping
    {
        /// <summary>
        /// 错误结果字符串列表
        /// </summary>
        public List<string> Incorrect { get; set; }

        /// <summary>
        /// 错误结果映射到的正确结果
        /// </summary>
        public string Correct { get; set; }
    }
}
