using JinChanChanTool.DataClass;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace JinChanChanTool.Services.DataServices
{
    public class CorrectionService:ICorrectionService
    {
        /// <summary>
        /// 结果映射对象列表
        /// </summary>
        public List<ResultMapping> ResultMappings { get; set; }

        /// <summary>
        /// 结果映射字典
        /// </summary>
        public Dictionary<string, string> ResultDictionary { get; }

        /// <summary>
        /// 英雄名称字符字典
        /// </summary>
        private HashSet<char> _charDictionary;

        /// <summary>
        /// 识别错误结果存放列表
        /// </summary>
        HashSet<string> Errorresult { get; set; }
        /// <summary>        
        /// OCR结果纠正列表文件路径
        /// </summary>
        private string filePath;

       
        public CorrectionService()
        {          
            ResultMappings = new List<ResultMapping>();
            _charDictionary = new HashSet<char>();
            ResultDictionary = new Dictionary<string, string>();
            Errorresult =new HashSet<string>();
            InitializePaths();                     
        }

        /// <summary>
        /// 初始化本地文件路径
        /// </summary>
        private void InitializePaths()
        {
            string parentPath = Path.Combine(Application.StartupPath, "Resources");
            // 确保目录存在
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }
            filePath = Path.Combine(parentPath, "CorrectionsList.json");          
        }

        /// <summary>
        /// 加载结果映射对象并填充字典。
        /// </summary>
        public  void Load()
        {
            LoadFromFile();           
            BuildDictionary();
        }
        
        /// <summary>
        /// 从本地文件读取字库。
        /// </summary>
        public void SetCharDictionary(HashSet<char> CharDictionary)
        {
            _charDictionary = CharDictionary;          
        }

        /// <summary>
        /// 从本地文件读取到结果映射对象，若失败则覆盖空的文件。
        /// </summary>
        private void LoadFromFile()
        {
            ResultMappings.Clear();
            try
            {
                //判断Json文件是否存在
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"找不到OCR结果纠正列表文件\"CorrectionsList.json\"\n路径：\n{filePath}\n将创建新的文件。",
                                    "文件不存在",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                    );
                    Save();
                    return;
                }
                string json = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(json))
                {
                    MessageBox.Show($"OCR结果纠正列表文件\"CorrectionsList.json\"内容为空。\n路径：\n{filePath}\n将创建新的文件。",
                               "文件为空",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error
                               );
                    Save();
                    return;
                }

                ResultMappings = JsonSerializer.Deserialize<List<ResultMapping>>(json);
            }
            catch
            {
                MessageBox.Show($"OCR结果纠正列表文件\"CorrectionsList.json\"格式错误\n路径：\n{filePath}\n将创建新的文件。",
                                   "文件格式错误",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
                Save();
            }
        }

        /// <summary>
        /// 从当前结果映射对象保存到本地
        /// </summary>
        public void Save()
        {
            try
            {
                // 设置 JsonSerializerOptions 以保持中文字符的可读性
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                // 直接序列化 List<NameMapping>
                string json = JsonSerializer.Serialize(ResultMappings, options);
                File.WriteAllText(filePath, json);
            }
            catch
            {               
                MessageBox.Show($"OCR结果纠正列表文件\"CorrectionsList.json\"保存失败\n路径：\n{filePath}",
                                  "文件保存失败",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error
                                  );
            }
        }

        /// <summary>
        /// 填充字典
        /// </summary>
        private void BuildDictionary()
        {
            ResultDictionary.Clear();
            for (int i = 0; i < ResultMappings.Count; i++)
            {
                for (int j = 0; j < ResultMappings[i].Incorrect.Count; j++)
                {
                    if (!string.IsNullOrEmpty(ResultMappings[i].Incorrect[j]) && !string.IsNullOrEmpty(ResultMappings[i].Correct))
                    {
                        ResultDictionary[ResultMappings[i].Incorrect[j]] = ResultMappings[i].Correct;
                    }
                }
            }
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        public void ReLoad()
        {
            ResultMappings.Clear();
            ResultDictionary.Clear();           
            _charDictionary.Clear();           
            Errorresult.Clear();
            Load();
        }

        /// <summary>
        /// 根据结果映射字典，将OCR识别结果纠正为正确结果。
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public  string ConvertToRightResult(string result,out bool isError,out string errorMessage)
        {           
            // 清理输入字符串
            isError = true;
            errorMessage = null;
            result = result.Replace(" ", "").Replace("?", "");
            // 查找映射
            if (ResultDictionary.TryGetValue(result, out var correctValue))
            {
                return correctValue;
            }
            else
            {
                errorMessage = UpdataErrorDir(result);
                if(!string.IsNullOrWhiteSpace(errorMessage))
                {
                    isError = false;
                }
                return result;
            }                                       
        }

        private string UpdataErrorDir(string result)
        {          
            foreach (char c in result)
            {
                if (!_charDictionary.Contains(c))
                { 
                    if(!Errorresult.Contains(result))
                    {
                        Errorresult.Add(result);
                        return $"\"{result}\"";
                    }                    
                }
            }                                              
                return null                ;                       
        }
    }
}
