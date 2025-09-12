using JinChanChanTool.DataClass;
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

        HashSet<char> CharDictionary { get; set; }  

        HashSet<string> Errorresult { get; set; }
        /// <summary>
        /// OCR结果纠正列表文件路径
        /// </summary>
        private string filePath;

        /// <summary>
        /// 字库文件路径
        /// </summary>
        private string dirPath;

        public CorrectionService()
        {            
            ResultMappings = new List<ResultMapping>();
            CharDictionary = new HashSet<char>();
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
            dirPath= Path.Combine(parentPath, "CharDir.txt");
        }

        /// <summary>
        /// 加载结果映射对象并填充字典。
        /// </summary>
        public  void Load()
        {
            LoadFromFile();
            LoadCharLib();
            BuildDictionary();
        }
        
        /// <summary>
        /// 从本地文件读取字库。
        /// </summary>
        private void LoadCharLib()
        {
            CharDictionary.Clear();
            try
            {
                //判断文件是否存在
                if (!File.Exists(dirPath))
                {
                    MessageBox.Show($"找不到字库文件\"CharDir.txt\"\n路径：\n{dirPath}",
                                    "文件不存在",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                    );                   
                    return;
                }
                HashSet<char> dir = new HashSet<char>();
                foreach(string line in File.ReadAllLines(dirPath, Encoding.UTF8))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        foreach(char c in line.Trim())
                        {
                            dir.Add(c);
                        }
                    }
                }
                if(dir.Count == 0)
                {
                    MessageBox.Show($"字库文件\"CharDir.txt\"内容为空。\n路径：\n{dirPath}",
                               "文件为空",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error
                               );
                    return;
                }
                CharDictionary = dir;
            }
            catch
            {
                MessageBox.Show($"字库文件\"CharDir.txt\"格式错误\n路径：\n{dirPath}\n",
                                   "文件格式错误",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
                
            }
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
            CharDictionary.Clear();           
            Errorresult.Clear();
            Load();
        }

        /// <summary>
        /// 根据结果映射字典，将OCR识别结果纠正为正确结果。
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public  string ConvertToRightResult(string result)
        {           
            // 清理输入字符串
            result = result.Replace(" ", "").Replace("?", "");
            // 查找映射
            if (ResultDictionary.TryGetValue(result, out var correctValue))
            {
                return correctValue;
            }
            else
            {
                UpdataErrorDir(result);
                return result;
            }                                       
        }
        private void UpdataErrorDir(string result)
        {
            string errorResult = "";
            foreach (char c in result)
            {
                if (!CharDictionary.Contains(c))
                {
                    errorResult += $"\"{c}\"";
                }
            }
            if (!string.IsNullOrWhiteSpace(errorResult))
            {
                errorResult += $" => {result}";
            }
            if(!Errorresult.Contains(errorResult))
            {
                Errorresult.Add(errorResult);
                MessageBox.Show($"OCR识别结果中含有非英雄名称字库中的字符：\n{errorResult}\n请将其添加到结果纠正列表，程序运行期间，对于这个识别错误的字符，只会提示一次。若愿意助力本程序开发，请于QQ群954285837提交该错误，" +
                    $"谢谢。", "非英雄名称字库中的字符",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
            }
        }
    }
}
