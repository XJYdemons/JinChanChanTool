using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.DataServices.Interface;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using static JinChanChanTool.DataClass.LineUp;

namespace JinChanChanTool.Services.DataServices
{
    public class LineUpService:ILineUpService
    {
        /// <summary>
        /// 文件路径列表
        /// </summary>
        private string[] _paths;

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
        /// 变阵索引
        /// </summary>
        private int 变阵索引;

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

        /// <summary>
        /// 阵容改变事件
        /// </summary>
        public event EventHandler LineUpChanged;

        /// <summary>
        /// 阵容名改变事件
        /// </summary>
        public event EventHandler LineUpNameChanged;
       
        #region 初始化
        public LineUpService(IHeroDataService iHeroDataService,int countOfLineUps,int maxOfChoice,int lineUpIndex)
        {
            _iHeroDataService = iHeroDataService;
            _countOfLineUps = countOfLineUps;
            _maxOfChoice = maxOfChoice;
            InitializePaths();
            _pathIndex = 0;
            变阵索引 = 0;
            _lineUpIndex = lineUpIndex;                        
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
            _paths = subDirs;            
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
            if (_paths.Length > 0 && _pathIndex < _paths.Length)
            {
                string filePath = Path.Combine(_paths[_pathIndex], "LineUps.json");
                try
                {
                    
                    string json = JsonConvert.SerializeObject(_lineUps, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                    NotifyLineUpNameChanged();
                    return true;
                }
                catch
                {
                    MessageBox.Show($"阵容文件\"{Path.GetFileName(filePath)}\"保存失败\n路径：\n{filePath}",
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
            变阵索引 = 0;
            _lineUpIndex = 0;
            _lineUps = new List<LineUp>();                 
            LoadFromFile();
        }

        /// <summary>
        /// 检查当前子阵容是否包含指定英雄名称，若包含则将其从子阵容删除，否则将其添加到子阵容。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddAndDeleteHero(string name, string[] equipments)
        {
            if (GetCurrentSubLineUp().Contains(name))
            {
                GetCurrentSubLineUp().Remove(name);
                NotifyLineUpChanged();
                return true;
            }
            else
            {
                if (GetCurrentSubLineUp().LineUpUnits.Count < _maxOfChoice)
                {
                    GetCurrentSubLineUp().Add(name, equipments);
                    OrderCurrentSubLineUp();
                    NotifyLineUpChanged();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 增加指定英雄名称到当前子阵容(指定装备)，若已存在则不再增加
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddHero(string name, string[] equipments)
        {
            if (GetCurrentSubLineUp().Contains(name))
            {
                return false;
            }
            else
            {
                if (GetCurrentSubLineUp().LineUpUnits.Count < _maxOfChoice)
                {
                    GetCurrentSubLineUp().Add(name, equipments);
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
        /// 增加指定英雄名称到当前子阵容(不指定装备)，若已存在则不再增加
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
                if (GetCurrentSubLineUp().LineUpUnits.Count < _maxOfChoice)
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
            _lineUps[_lineUpIndex].SubLineUps[变阵索引].LineUpUnits.Clear();
            NotifyLineUpChanged();
        }

        /// <summary>
        /// 替换当前子阵容的英雄列表（用于从推荐阵容导入）
        /// </summary>
        /// <param name="lineUpUnits">要导入的英雄单位列表</param>
        /// <returns>是否替换成功</returns>
        public bool ReplaceCurrentSubLineUp(List<LineUpUnit> lineUpUnits)
        {
            if (lineUpUnits == null)
            {
                return false;
            }

            // 清空当前子阵容
            _lineUps[_lineUpIndex].SubLineUps[变阵索引].LineUpUnits.Clear();

            // 添加新的英雄单位（限制最大数量）
            int count = Math.Min(lineUpUnits.Count, _maxOfChoice);
            for (int i = 0; i < count; i++)
            {
                var unit = lineUpUnits[i];
                // 验证英雄是否存在
                if (_iHeroDataService.GetHeroFromName(unit.HeroName) != null)
                {
                    // 创建新的LineUpUnit，避免引用问题
                    var newUnit = new LineUpUnit
                    {
                        HeroName = unit.HeroName,
                        EquipmentNames = unit.EquipmentNames?.ToArray() ?? ["", "", ""]
                    };
                    _lineUps[_lineUpIndex].SubLineUps[变阵索引].LineUpUnits.Add(newUnit);
                }
            }

            // 按Cost排序
            OrderCurrentSubLineUp();
            NotifyLineUpChanged();
            return true;
        }

        /// <summary>
        /// 修改当前子阵容中指定英雄的装备
        /// </summary>
        /// <param name="heroName">英雄名称</param>
        /// <param name="equipmentIndex">装备槽位索引(0-2)</param>
        /// <param name="equipmentName">新装备名称</param>
        /// <returns>是否修改成功</returns>
        public bool SetHeroEquipment(string heroName, int equipmentIndex, string equipmentName)
        {
            bool result = GetCurrentSubLineUp().SetEquipment(heroName, equipmentIndex, equipmentName);
            if (result)
            {
                NotifyLineUpChanged();
            }
            return result;
        }

        /// <summary>
        /// 获取阵容集合
        /// </summary>
        /// <returns></returns>
        public List<LineUp> GetLineUps()
        {
            return _lineUps;
        }

        /// <summary>
        /// 获取当前阵容对象
        /// </summary>
        /// <returns></returns>
        public LineUp GetCurrentLineUp()
        {
            return _lineUps[_lineUpIndex];
        }

        /// <summary>
        /// 获取当前变阵
        /// </summary>
        /// <returns></returns>
        public SubLineUp GetCurrentSubLineUp()
        {                       
            return _lineUps[_lineUpIndex].SubLineUps[变阵索引];
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
                变阵索引 = 0;
                
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
        /// 设置当前变阵下标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetSubLineUpIndex(int index)
        {
            if (index >= 0 && index < 3)
            {
                变阵索引 = index;
                NotifyLineUpChanged();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前变阵下标
        /// </summary>
        /// <returns></returns>
        public int GetSubLineUpIndex()
        {
            return 变阵索引;
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
                _lineUps[index].LineUpName = name;              
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置当前阵容下指定变阵名称
        /// </summary>
        /// <param name="index">变阵下标</param>
        /// <param name="name">新名称</param>
        /// <returns></returns>
        public bool SetSubLineUpName(int index, string name)
        {
            if (index < 0 || index >= GetCurrentLineUp().SubLineUps.Length)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            _lineUps[_lineUpIndex].SubLineUps[index].SubLineUpName = name.Trim();
            NotifyLineUpChanged();
            return true;
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
        /// 设置文件路径索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetFilePathsIndex(string season)
        {
            int selectedIndex = 0;
            bool isFound = false;
            if (!string.IsNullOrEmpty(season))
            {
                for (int i = 0; i < _paths.Length; i++)
                {
                    if (Path.GetFileName(_paths[i]).Equals(season, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        isFound = true;
                        break;
                    }
                }
            }
            if (_paths.Length > 0)
            {
                _pathIndex = Math.Min(selectedIndex, _paths.Length - 1);
            }
            return isFound;
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
            if (_paths.Length > 0&&_pathIndex<_paths.Length)
            {
                string filePath = Path.Combine(_paths[_pathIndex], "LineUps.json");
                try
                {
                    
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"找不到阵容文件\"{Path.GetFileName(filePath)}\"\n路径：\n{filePath}\n将创建新的阵容文件。",
                                    "文件缺失",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                    );
                        LoadDefaultLineups();
                        Save();
                        return ;
                    }

                    string json = File.ReadAllText(filePath);
                    if (string.IsNullOrEmpty(json))
                    {
                        MessageBox.Show($"阵容文件\"{Path.GetFileName(filePath)}\"内容为空。\n路径：\n{filePath}\n将创建新的阵容文件。",
                                   "文件为空",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning
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
                            _lineUps.Add(new LineUp ($"阵容{i + 1 + temp.Count}"));
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
                    foreach (LineUp lineUp in _lineUps)
                    {                        
                        for(int i =0;i<lineUp.SubLineUps.Length;i++)
                        {
                            foreach (LineUpUnit sUnit in lineUp.SubLineUps[i].LineUpUnits)
                            {
                                if (_iHeroDataService.GetHeroFromName(sUnit.HeroName) == null)
                                {
                                    MessageBox.Show($"阵容文件“{Path.GetFileName(filePath)}”与英雄配置文件“HeroData.json”不匹配，将创建新的阵容文件。",
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
                        for (int i = 0; i < lineUp.SubLineUps.Length; i++)
                        {
                            List<LineUpUnit> newList = lineUp.SubLineUps[i].LineUpUnits.OrderBy(unit => _iHeroDataService.GetHeroFromName(unit.HeroName).Cost).ToList();
                            lineUp.SubLineUps[i].LineUpUnits.Clear();
                            lineUp.SubLineUps[i].LineUpUnits.AddRange(newList);
                        }
                                               
                    }  
                    
                    //只保存设置中设置的最大选择英雄个数
                    foreach (LineUp lineUp in _lineUps)
                    {
                        for (int i = 0; i < lineUp.SubLineUps.Length; i++)
                        {
                            // 如果成员数超过限制，保留前 n 个
                            if (lineUp.SubLineUps[i].LineUpUnits.Count > _maxOfChoice)
                            {
                                List<LineUpUnit> newList = lineUp.SubLineUps[i].LineUpUnits.Take(_maxOfChoice).ToList();
                                lineUp.SubLineUps[i].LineUpUnits.Clear();
                                lineUp.SubLineUps[i].LineUpUnits.AddRange(newList);
                            }
                        }
                                                                                
                    }
                }
                catch
                {
                    MessageBox.Show($"阵容文件“{Path.GetFileName(filePath)}”格式错误\n路径：\n{filePath}\n将创建新的阵容文件。",
                                  "文件格式错误",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning
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
            for (int i = 1; i <= _countOfLineUps; i++)
            {
                _lineUps.Add(new LineUp($"阵容{i}"));                   
            }           
        }

        /// <summary>
        /// 按Cost升序排序当前子阵容的英雄
        /// </summary>
        private void OrderCurrentSubLineUp()
        {                      
            List<LineUpUnit> newList = GetCurrentSubLineUp().LineUpUnits.OrderBy(unit => _iHeroDataService.GetHeroFromName(unit.HeroName).Cost).ToList();
            GetCurrentSubLineUp().LineUpUnits.Clear();
            GetCurrentSubLineUp().LineUpUnits.AddRange(newList);           
        }

        private void NotifyLineUpChanged()
        {
            LineUpChanged?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyLineUpNameChanged()
        {
            LineUpNameChanged?.Invoke(this, EventArgs.Empty);
        }
       
        #endregion        
    }  
}
