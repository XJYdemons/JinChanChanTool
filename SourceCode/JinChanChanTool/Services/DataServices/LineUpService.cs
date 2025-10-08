using JinChanChanTool.DataClass;
using Newtonsoft.Json;
using System.Linq;

namespace JinChanChanTool.Services.DataServices
{
    public class LineUpService:ILineUpService
    {
        /// <summary>
        /// 文件路径列表
        /// </summary>
        private string[] paths;

        /// <summary>
        /// 阵容文件路径索引
        /// </summary>
        private int _pathIndex;

        /// <summary>
        /// LineUp对象列表，存储多个阵容信息。
        /// </summary>
        private List<LineUp> _lineUps;
              
        /// <summary>
        /// 阵容索引
        /// </summary>
        private int _lineUpIndex;
       
        /// <summary>
        /// 子阵容索引
        /// </summary>
        private int _subLineUpIndex;

        /// <summary>
        /// 英雄数据服务对象
        /// </summary>

        private IHeroDataService _iHeroDataService;

        /// <summary>
        /// 阵容数量       
        /// </summary>
        private int _countOfLineUps;

        /// <summary>
        /// 最大选择数量
        /// </summary>
        /// 
        private int _maxOfChoice;

        public event EventHandler LineUpChanged;

        public event EventHandler LineUpNameChanged;

        public event EventHandler SubLineUpIndexChanged;
        #region 初始化
        public LineUpService(IHeroDataService iHeroDataService,int countOfLineUps,int maxOfChoice)
        {
            _iHeroDataService = iHeroDataService;
            _countOfLineUps = countOfLineUps;
            _maxOfChoice = maxOfChoice;

            InitializePaths();
            _pathIndex = 0;
            _lineUpIndex = 0;
            _subLineUpIndex = 0;
            
            _lineUps=new List<LineUp>();           
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
        #endregion

        #region 公共方法
        /// <summary>
        /// 从本地文件加载阵容
        /// </summary>
        public void Load()
        {
            LoadFromFile();
        }

        /// <summary>
        /// 将当前阵容数据保存到本地文件。
        /// </summary>
        public bool Save()
        {
            if (paths.Length > 0 && _pathIndex < paths.Length)
            {
                try
                {
                    string filePath = Path.Combine(paths[_pathIndex], "LineUps.json");
                    string json = JsonConvert.SerializeObject(_lineUps, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                    NotifyLineUpNameChanged();
                    return true;
                }
                catch
                {
                    MessageBox.Show($"阵容文件\"LineUps.json\"保存失败\n路径：\n{Path.Combine(paths[_pathIndex], "LineUps.json")}",
                                  "文件保存失败",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error
                                  );
                    return false;
                }
            }
            else
            {
                MessageBox.Show($"路径不存在，保存失败！",
                                 "路径不存在",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error
                                 );
                return false;
            }
        }

        /// <summary>
        /// 重新加载，需要获取英雄数据服务对象
        /// </summary>
        /// <param name="countOfHeros"></param>
        /// <param name="countOfLineUps"></param>
        public void ReLoad(IHeroDataService heroDataService)
        {
            _iHeroDataService = heroDataService;
            _subLineUpIndex = 0;
            _lineUpIndex = 0;
            _lineUps = new List<LineUp>();                 
            LoadFromFile();
        }

        /// <summary>
        /// 获取当前子阵容
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentSubLineUp()
        {
            return _lineUps[_lineUpIndex].Selected[_subLineUpIndex];
        }

        /// <summary>
        /// 检查当前子阵容是否包含指定英雄名称，若包含则将其从子阵容删除，否则将其添加到子阵容。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddAndDeleteHero(string name)
        {
            if (GetCurrentSubLineUp().Contains(name))
            {
                GetCurrentSubLineUp().Remove(name);
                NotifyLineUpChanged();
                return true;
            }
            else
            {
                if (GetCurrentSubLineUp().Count < _maxOfChoice)
                {
                    GetCurrentSubLineUp().Add(name);
                    OrderCurrentSubLineUp();
                    NotifyLineUpChanged();
                    return true;
                }
                return false;
            }
        }
        
        /// <summary>
        /// 增加指定英雄名称到当前子阵容，若已存在则不再增加
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddHero(string name)
        {
            if (GetCurrentSubLineUp().Contains(name))
            {
                return false;
            }
            else
            {
                if (GetCurrentSubLineUp().Count < _maxOfChoice)
                {
                    GetCurrentSubLineUp().Add(name);
                    OrderCurrentSubLineUp();
                    NotifyLineUpChanged();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 批量增加指定英雄名称到当前子阵容，若已存在则不再增加
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public void AddHeros(List<string> names)
        {            
            for (int i = 0;i< names.Count;i++)
            {
                if (GetCurrentSubLineUp().Contains(names[i]))
                {
                    continue;
                }
                else
                {
                    if (GetCurrentSubLineUp().Count < _maxOfChoice)
                    {
                        GetCurrentSubLineUp().Add(names[i]);                        
                    }                   
                    else
                    {
                        break;
                    }
                }              
            }
            OrderCurrentSubLineUp();
            NotifyLineUpChanged();           
        }

        /// <summary>
        /// 从当前子阵容删除指定英雄名称，若不存在则不会删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteHero(string name)
        {
            if (GetCurrentSubLineUp().Contains(name))
            {
                GetCurrentSubLineUp().Remove(name);                
                NotifyLineUpChanged();
                return true;
            }
            else
            {
                return false;
            }
        }

       

        /// <summary>
        /// 清空当前子阵容
        /// </summary>
        public void ClearCurrentSubLineUp()
        {
            _lineUps[_lineUpIndex].Selected[_subLineUpIndex].Clear();          
            NotifyLineUpChanged();
        }

        /// <summary>
        /// 设置阵容下标
        /// </summary>
        /// <param name="lineUpIndex"></param>
        public bool SetLineUpIndex(int lineUpIndex)
        {
            if (lineUpIndex >= 0 && lineUpIndex < _countOfLineUps)
            {
                _lineUpIndex = lineUpIndex;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取阵容下标
        /// </summary>
        /// <returns></returns>
        public int GetLineUpIndex()
        {
            return _lineUpIndex;
        }

        /// <summary>
        /// 设置指定下标阵容名称
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>        
        public bool SetLineUpName(int index, string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && index >= 0 && index < _countOfLineUps)
            {
                _lineUps[index].Name = name;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取指定下标阵容名称
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetLineUpName(int index)
        {
            return _lineUps[index].Name;
        }

        /// <summary>
        /// 设置子阵容下标
        /// </summary>
        /// <param name="subLineUpIndex"></param>
        public bool SetSubLineUpIndex(int subLineUpIndex)
        {
            if (subLineUpIndex >= 0 && subLineUpIndex <= 2)
            {
                _subLineUpIndex = subLineUpIndex;
                NotifySubLineUpIndexChanged();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取子阵容下标
        /// </summary>
        /// <returns></returns>
        public int GetSubLineUpIndex()
        {
            return _subLineUpIndex;
        }

        /// <summary>
        /// 获取最大选择数量
        /// </summary>
        /// <returns></returns>
        public int GetMaxSelect()
        {
            return _maxOfChoice;
        }

        /// <summary>
        /// 获取最大阵容数量
        /// </summary>
        /// <returns></returns>
        public int GetMaxLineUpCount()
        {
            return _countOfLineUps;
        }

        /// <summary>
        /// 设置阵容文件路径下标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetFilePathIndex(int index)
        {
            if (index >= 0 && index < paths.Length)
            {
                _pathIndex = index;
                return true;
            }
            return false;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 从本地文件加载阵容数据，若失败则创建默认阵容文件并加载。
        /// </summary>
        /// <param name="countOfHeros"></param>
        /// <param name="TotalLineups"></param>
        /// <returns></returns>
        private void LoadFromFile()
        {
            _lineUps.Clear();
            if (paths.Length > 0&&_pathIndex<paths.Length)
            {
                try
                {
                    string filePath = Path.Combine(paths[_pathIndex], "LineUps.json");
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

                    //检查阵容数量是否满足设置中的个数
                    List<LineUp> temp = JsonConvert.DeserializeObject<List<LineUp>>(json);                   
                    if (temp.Count < _countOfLineUps)
                    {
                        _lineUps.AddRange(temp);
                        for (int i = 0; i < _countOfLineUps - temp.Count; i++)
                        {
                            _lineUps.Add(new LineUp { Name = $"阵容{i + 1 + temp.Count}", Selected = new List<string>[3] });
                        }
                     }
                    else if(temp.Count>_countOfLineUps)
                    {
                        for (int i = 0; i < _countOfLineUps; i++)
                        {
                             _lineUps.Add(temp[i]);
                        }
                    }
                    else
                    {
                         _lineUps.AddRange(temp);
                    }

                    //检查阵容数据是否与英雄数据冲突
                    foreach(LineUp lineUp in _lineUps)
                    {
                         foreach(List<string> subLineUp in lineUp.Selected)
                        {                            
                            foreach(string name in subLineUp)
                            {
                                if(_iHeroDataService.GetHeroFromName(name)==null)
                                {
                                    MessageBox.Show($"阵容文件“LineUps.json”与英雄配置文件“HeroData.json”不匹配，将创建新的阵容文件。",
                                             "阵容文件与英雄配置文件冲突",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Warning);
                                    LoadDefaultLineups();
                                    Save();
                                    return;
                                }
                            }
                         }
                    }

                    //将阵容按照Cost排序
                    foreach (LineUp lineUp in _lineUps)
                    {
                        for (int i = 0; i < lineUp.Selected.Length; i++)
                        {

                            lineUp.Selected[i] = lineUp.Selected[i]
                                .OrderBy(name => _iHeroDataService.GetHeroFromName(name).Cost)
                                .ToList();
                        }
                    }

                    //只保存设置中设置的最大选择英雄个数
                    foreach (LineUp lineUp in _lineUps)
                    {
                        for (int i = 0; i < lineUp.Selected.Length; i++)
                        {                            
                            // 如果成员数超过限制，保留前 n 个
                            if (lineUp.Selected[i].Count > _maxOfChoice)
                            {
                                lineUp.Selected[i] = lineUp.Selected[i]
                                    .Take(_maxOfChoice) // 只保留前 n 个
                                    .ToList();
                            }                                                     
                        }
                    }
                }
                catch
                {
                    MessageBox.Show($"阵容文件“LineUps.json”格式错误\n路径：\n{Path.Combine(paths[_pathIndex], "LineUps.json")}\n将创建新的阵容文件。",
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
            _lineUps.Clear();
            if (_iHeroDataService.GetHeroCount() > 0)
            {
                for (int i = 1; i <= _countOfLineUps; i++)
                {
                    _lineUps.Add(new LineUp
                    {
                        Name = $"阵容{i}",
                        Selected = new List<string>[3] { new List<string>(), new List<string>(), new List<string>() }
                    });
                }
            }
        }

        /// <summary>
        /// 按Cost升序排序当前子阵容的英雄
        /// </summary>
        private void OrderCurrentSubLineUp()
        {
            _lineUps[_lineUpIndex].Selected[_subLineUpIndex] = _lineUps[_lineUpIndex].Selected[_subLineUpIndex]
            .OrderBy(name => _iHeroDataService.GetHeroFromName(name).Cost)
            .ToList();
        }

        private void NotifyLineUpChanged()
        {
            LineUpChanged?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyLineUpNameChanged()
        {
            LineUpNameChanged?.Invoke(this, EventArgs.Empty);
        }
        private void NotifySubLineUpIndexChanged()
        {
            SubLineUpIndexChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion        
    }  
}
