using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices.Interface
{
    public interface ICorrectionService
    {
        /// <summary>
        /// 结果映射对象列表
        /// </summary>
        List<ResultMapping> ResultMappings { get; set; }

        /// <summary>
        /// 结果映射字典
        /// </summary>
        Dictionary<string, string> ResultDictionary { get;}  
        
        /// <summary>
        /// 从本地文件加载到对象
        /// </summary>
        void Load();

        /// <summary>
        /// 从对象保存到本地文件
        /// </summary>
        void Save();

        /// <summary>
        /// 重新加载
        /// </summary>
        void ReLoad();

        /// <summary>
        /// 根据结果映射字典，将OCR识别结果纠正为正确结果。
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        string ConvertToRightResult(string result, out bool isError, out string errorMessage);

        /// <summary>
        /// 设置英雄名字符哈希表
        /// </summary>
        /// <param name="CharDictionary"></param>
        void SetCharDictionary(HashSet<char> CharDictionary);
    }
}
