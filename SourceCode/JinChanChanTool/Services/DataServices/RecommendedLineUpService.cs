using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices.Interface;
using Newtonsoft.Json;

namespace JinChanChanTool.Services.DataServices
{
    /// <summary>
    /// 推荐阵容数据文件包装类（用于JSON序列化）
    /// </summary>
    internal class RecommendedLineUpDataFile
    {
        /// <summary>
        /// 数据更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 推荐阵容列表
        /// </summary>
        public List<RecommendedLineUp> LineUps { get; set; } = [];
    }

    /// <summary>
    /// 推荐阵容服务实现类
    /// </summary>
    public class RecommendedLineUpService : IRecommendedLineUpService
    {
        /// <summary>
        /// 文件路径列表
        /// </summary>
        private string[] _paths;

        /// <summary>
        /// 当前文件路径索引
        /// </summary>
        private int _pathIndex;

        /// <summary>
        /// 推荐阵容列表
        /// </summary>
        private List<RecommendedLineUp> _recommendedLineUps;

        /// <summary>
        /// 数据最后更新时间
        /// </summary>
        private DateTime _lastUpdateTime;

        /// <summary>
        /// 推荐阵容数据变更事件
        /// </summary>
        public event EventHandler DataChanged;

        /// <summary>
        /// 推荐阵容数据文件名
        /// </summary>
        private const string DataFileName = "RecommendedLineUps.json";

        #region 初始化
        public RecommendedLineUpService()
        {
            InitializePaths();
            _pathIndex = 0;
            _recommendedLineUps = new List<RecommendedLineUp>();
            _lastUpdateTime = DateTime.MinValue;
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
            // 获取所有子目录（Directory.GetDirectories已返回完整路径）
            _paths = Directory.GetDirectories(parentPath);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 从本地文件加载推荐阵容数据
        /// </summary>
        public void Load()
        {
            LoadFromFile();
        }

        /// <summary>
        /// 将推荐阵容数据保存到本地文件
        /// </summary>
        public bool Save()
        {
            if (_paths.Length > 0 && _pathIndex < _paths.Length)
            {
                string filePath = Path.Combine(_paths[_pathIndex], DataFileName);
                try
                {
                    // 更新保存时间
                    _lastUpdateTime = DateTime.Now;

                    // 创建包装对象
                    var dataFile = new RecommendedLineUpDataFile
                    {
                        UpdateTime = _lastUpdateTime,
                        LineUps = _recommendedLineUps
                    };

                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
                    };

                    string json = JsonConvert.SerializeObject(dataFile, settings);
                    File.WriteAllText(filePath, json);
                    NotifyDataChanged();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"推荐阵容文件\"{DataFileName}\"保存失败\n路径：\n{filePath}\n错误：{ex.Message}",
                                  "文件保存失败",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("路径不存在，保存失败！",
                                 "路径不存在",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 重新加载推荐阵容数据
        /// </summary>
        public void ReLoad()
        {
            _recommendedLineUps.Clear();
            LoadFromFile();
        }

        /// <summary>
        /// 获取所有推荐阵容
        /// </summary>
        public List<RecommendedLineUp> GetAllRecommendedLineUps()
        {
            return _recommendedLineUps.ToList();
        }

        /// <summary>
        /// 根据优先级获取推荐阵容
        /// </summary>
        public List<RecommendedLineUp> GetRecommendedLineUpsByTier(LineUpTier tier)
        {
            return _recommendedLineUps.Where(l => l.Tier == tier).ToList();
        }

        /// <summary>
        /// 根据标签获取推荐阵容
        /// </summary>
        public List<RecommendedLineUp> GetRecommendedLineUpsByTag(string tag)
        {
            return _recommendedLineUps.Where(l => l.Tags.Contains(tag)).ToList();
        }

        /// <summary>
        /// 根据名称获取推荐阵容
        /// </summary>
        public RecommendedLineUp? GetRecommendedLineUpByName(string name)
        {
            return _recommendedLineUps.FirstOrDefault(l => l.LineUpName == name);
        }

        /// <summary>
        /// 添加推荐阵容
        /// </summary>
        public bool AddRecommendedLineUp(RecommendedLineUp lineUp)
        {
            if (lineUp == null || string.IsNullOrWhiteSpace(lineUp.LineUpName))
            {
                return false;
            }

            // 检查是否已存在同名阵容
            if (_recommendedLineUps.Any(l => l.LineUpName == lineUp.LineUpName))
            {
                return false;
            }

            _recommendedLineUps.Add(lineUp);
            NotifyDataChanged();
            return true;
        }

        /// <summary>
        /// 批量添加推荐阵容（用于爬虫数据导入）
        /// </summary>
        public int AddRecommendedLineUps(List<RecommendedLineUp> lineUps)
        {
            if (lineUps == null || lineUps.Count == 0)
            {
                return 0;
            }

            int addedCount = 0;
            foreach (var lineUp in lineUps)
            {
                if (lineUp != null && !string.IsNullOrWhiteSpace(lineUp.LineUpName))
                {
                    // 如果已存在同名阵容，则更新；否则添加
                    var existing = _recommendedLineUps.FirstOrDefault(l => l.LineUpName == lineUp.LineUpName);
                    if (existing != null)
                    {
                        // 更新现有阵容
                        int index = _recommendedLineUps.IndexOf(existing);
                        _recommendedLineUps[index] = lineUp;
                    }
                    else
                    {
                        // 添加新阵容
                        _recommendedLineUps.Add(lineUp);
                    }
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                NotifyDataChanged();
            }

            return addedCount;
        }

        /// <summary>
        /// 删除推荐阵容
        /// </summary>
        public bool RemoveRecommendedLineUp(string name)
        {
            var lineUp = _recommendedLineUps.FirstOrDefault(l => l.LineUpName == name);
            if (lineUp != null)
            {
                _recommendedLineUps.Remove(lineUp);
                NotifyDataChanged();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清空所有推荐阵容
        /// </summary>
        public void ClearAll()
        {
            _recommendedLineUps.Clear();
            NotifyDataChanged();
        }

        /// <summary>
        /// 更新推荐阵容
        /// </summary>
        public bool UpdateRecommendedLineUp(RecommendedLineUp lineUp)
        {
            if (lineUp == null || string.IsNullOrWhiteSpace(lineUp.LineUpName))
            {
                return false;
            }

            var existing = _recommendedLineUps.FirstOrDefault(l => l.LineUpName == lineUp.LineUpName);
            if (existing != null)
            {
                int index = _recommendedLineUps.IndexOf(existing);
                _recommendedLineUps[index] = lineUp;
                NotifyDataChanged();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取推荐阵容数量
        /// </summary>
        public int GetCount()
        {
            return _recommendedLineUps.Count;
        }

        /// <summary>
        /// 获取数据最后更新时间
        /// </summary>
        public DateTime GetLastUpdateTime()
        {
            return _lastUpdateTime;
        }

        /// <summary>
        /// 设置文件路径索引
        /// </summary>
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

        /// <summary>
        /// 检查数据是否需要更新
        /// </summary>
        public bool NeedsUpdate(int hours = 24)
        {
            if (_lastUpdateTime == DateTime.MinValue)
            {
                return true;
            }

            return (DateTime.Now - _lastUpdateTime).TotalHours >= hours;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 从本地文件加载推荐阵容数据
        /// </summary>
        private void LoadFromFile()
        {
            _recommendedLineUps.Clear();

            if (_paths.Length > 0 && _pathIndex < _paths.Length)
            {
                string filePath = Path.Combine(_paths[_pathIndex], DataFileName);
                try
                {
                    if (!File.Exists(filePath))
                    {
                        // 文件不存在时不显示错误，只是初始化为空列表
                        _recommendedLineUps = new List<RecommendedLineUp>();
                        _lastUpdateTime = DateTime.MinValue;
                        return;
                    }

                    string json = File.ReadAllText(filePath);
                    if (string.IsNullOrEmpty(json))
                    {
                        _recommendedLineUps = new List<RecommendedLineUp>();
                        _lastUpdateTime = DateTime.MinValue;
                        return;
                    }

                    var settings = new JsonSerializerSettings
                    {
                        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
                    };
                  
                    var dataFile = JsonConvert.DeserializeObject<RecommendedLineUpDataFile>(json, settings);
                    if (dataFile != null && dataFile.LineUps != null)
                    {
                        _recommendedLineUps = dataFile.LineUps;
                        _lastUpdateTime = dataFile.UpdateTime;
                    }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"推荐阵容文件\"{DataFileName}\"加载失败\n路径：\n{filePath}\n错误：{ex.Message}",
                                  "文件加载失败",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    _recommendedLineUps = new List<RecommendedLineUp>();
                    _lastUpdateTime = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// 触发数据变更事件
        /// </summary>
        private void NotifyDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
