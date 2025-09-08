using Newtonsoft.Json;
using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices
{
    public class LineUpService:ILineUpService
    {
        /// <summary>
        /// 阵容文件路径索引
        /// </summary>
        public int PathIndex { get; set; }
        
        /// <summary>
        /// LineUp对象列表，存储多个阵容信息。
        /// </summary>
        public List<LineUp> LineUps { get; }

        /// <summary>
        /// 英雄数量
        /// </summary>
        public int CountOfHeros { get; private set; }

        /// <summary>
        /// 阵容数量       
        /// </summary>
        public int CountOfLineUps { get; private set; }

        /// <summary>
        /// 阵容索引
        /// </summary>
        public int LineUpIndex { get; set; }

        /// <summary>
        /// 子阵容索引
        /// </summary>
        public int SubLineUpIndex { get; set; }

        /// <summary>
        /// 文件路径列表
        /// </summary>
        private string[] paths;

        public LineUpService(int countOfHeros,int countOfLineUps)
        {
            InitializePaths();
            CountOfHeros =countOfHeros ;
            CountOfLineUps =countOfLineUps ;
            LineUpIndex = 0;
            SubLineUpIndex = 0;
            PathIndex = 0;                  
            LineUps=new List<LineUp>();
        }

        /// <summary>
        /// 初始化文件路径列表
        /// </summary>
        private void InitializePaths()
        {
            string parentPath = Path.Combine(Application.StartupPath, "Resources", "HeroDatas");
            // 确保目录存在
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }
            // 获取所有子目录名称（仅文件夹）
            string[] subDirs = Directory.GetDirectories(parentPath);
            for (int i = 0; i < subDirs.Length; i++)
            {
                subDirs[i] = Path.Combine(parentPath, subDirs[i]);
            }
            paths = subDirs;            
        }

        /// <summary>
        /// 从本地文件加载阵容
        /// </summary>
        public void Load()
        {
            LoadFromFile();            
        }

        /// <summary>
        /// 从本地文件加载阵容数据，若失败则创建默认阵容文件并加载。
        /// </summary>
        /// <param name="countOfHeros"></param>
        /// <param name="TotalLineups"></param>
        /// <returns></returns>
        private void LoadFromFile()
        {
            LineUps.Clear();
            if (paths.Length > 0&&PathIndex<paths.Length)
            {
                try
                {
                    string filePath = Path.Combine(paths[PathIndex], "LineUps.json");
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"找不到阵容文件\"LineUps.json\"\n路径：\n{filePath}\n将创建新的阵容文件。",
                                    "文件缺失",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                    );
                        LoadDefaultLineups();
                        Save();
                        return ;
                    }

                    string json = File.ReadAllText(filePath);
                    if (string.IsNullOrEmpty(json))
                    {
                        MessageBox.Show($"阵容文件\"LineUps.json\"内容为空。\n路径：\n{filePath}\n将创建新的阵容文件。",
                                   "文件为空",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
                        LoadDefaultLineups();
                        Save();
                        return ;
                    }
                    List<LineUp> temp = JsonConvert.DeserializeObject<List<LineUp>>(json);                   
                        if (temp.Count < CountOfLineUps)
                        {
                            LineUps.AddRange(temp);
                            for (int i = 0; i < CountOfLineUps - temp.Count; i++)
                            {
                                LineUps.Add(new LineUp { Name = $"阵容{i + 1 + temp.Count}", Checked = new bool[3, CountOfHeros] });
                            }
                        }
                        else if(temp.Count>CountOfLineUps)
                        {
                            for (int i = 0; i < CountOfLineUps; i++)
                            {
                                LineUps.Add(temp[i]);
                            }
                        }
                         else
                        {
                            LineUps.AddRange(temp);
                        }

                        for (int i = 0; i < LineUps.Count; i++)
                        {
                            if (LineUps[i].Checked.GetLength(1) != CountOfHeros)
                            {
                                MessageBox.Show($"阵容文件“LineUps.json”与英雄配置文件“HeroData.json”的英雄数量不一致，将创建新的阵容文件。",
                                    "阵容文件与英雄配置文件冲突",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            LoadDefaultLineups();
                            Save();
                            return ;
                            }
                        }                                      
                }
                catch
                {
                    MessageBox.Show($"阵容文件“LineUps.json”格式错误\n路径：\n{Path.Combine(paths[PathIndex], "LineUps.json")}\n将创建新的阵容文件。",
                                  "文件格式错误",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error
                                  );
                    LoadDefaultLineups();
                    Save();                    
                }
               
            }
            else
            {
                MessageBox.Show($"阵容配置文件夹不存在",
                    "文件夹不存在",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );                                
            }                                      
        }

        /// <summary>
        /// 创建默认阵容数据，并添加到阵容列表中。
        /// </summary>
        /// <param name="countOfHeros"></param>
        /// <param name="TotalLineups"></param>
        private void LoadDefaultLineups()
        {
            LineUps.Clear();
            if (CountOfHeros>0)
            {
                for (int i = 1; i <= CountOfLineUps; i++)
                {
                    LineUps.Add(new LineUp
                    {
                        Name = $"阵容{i}",
                        Checked = new bool[3, CountOfHeros]
                    });
                }
            }                        
        }

        /// <summary>
        /// 将当前阵容数据保存到本地文件。
        /// </summary>
        public void Save()
        {
            if (paths.Length > 0 && PathIndex < paths.Length)
            {
                try
                {
                    string filePath = Path.Combine(paths[PathIndex], "LineUps.json");
                    string json = JsonConvert.SerializeObject(LineUps, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                }
                catch
                {
                    MessageBox.Show($"阵容文件\"LineUps.json\"保存失败\n路径：\n{Path.Combine(paths[PathIndex], "LineUps.json")}",
                                  "文件保存失败",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error
                                  );
                }                                               
            }           
        }

        /// <summary>
        /// 重新加载，需要获取英雄数量与阵容数量
        /// </summary>
        /// <param name="countOfHeros"></param>
        /// <param name="countOfLineUps"></param>
        public void ReLoad(int countOfHeros, int countOfLineUps)
        {
            CountOfHeros = countOfHeros;
            CountOfLineUps = countOfLineUps;
            LineUpIndex = 0;
            SubLineUpIndex = 0;
            LineUps.Clear();
            Load();
        }

        /// <summary>
        /// 返回当前选中阵容的英雄下标列表
        /// </summary>
        /// <returns></returns>
        public List<int> SelectedIndexes()
        {
            List<int> selectedIndexes = new List<int>();
            for(int i = 0;i<CountOfHeros;i++)
            {
                if (LineUps[LineUpIndex].Checked[SubLineUpIndex, i])
                {
                    selectedIndexes.Add(i);
                }
            }
           return selectedIndexes; 
        }
    }
}
