using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace JinChanChanTool.Services.RecommendedEquipment
{
    /// <summary>
    /// 装备推荐数据文件包装类（用于JSON序列化）
    /// </summary>
    internal class EquipmentDataFile
    {
        /// <summary>
        /// 数据更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 装备推荐数据（英雄名 -> 装备列表）
        /// </summary>
        public Dictionary<string, List<string>> Data { get; set; } = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// 负责管理本地的装备推荐数据，包括从JSON文件加载、保存数据，以及加载相关的装备图片。
    /// 这个服务现在通过硬编码的赛季名来定位目标文件夹，确保操作的确定性。
    /// </summary>
    public class HeroEquipmentDataService : IHeroEquipmentDataService
    {
        public string[] Paths { get; set; }
        public List<DataClass.RecommendedEquipment> HeroEquipments { get; private set; }
        public Dictionary<DataClass.RecommendedEquipment, List<Image>> EquipmentImageMap { get; private set; }
        private Dictionary<string, DataClass.RecommendedEquipment> nameToHeroEquipmentMap { get; set; }
        private int _pathIndex;

        /// <summary>
        /// 数据最后更新时间
        /// </summary>
        private DateTime _lastUpdateTime;

        /// <summary>
        /// 构造函数，初始化属性并扫描所有可用的赛季路径。
        /// </summary>
        public HeroEquipmentDataService()
        {
            InitializePaths();
            HeroEquipments = new List<DataClass.RecommendedEquipment>();
            EquipmentImageMap = new Dictionary<DataClass.RecommendedEquipment, List<Image>>();
            nameToHeroEquipmentMap = new Dictionary<string, DataClass.RecommendedEquipment>();
            _lastUpdateTime = DateTime.MinValue;
            _pathIndex = 0;
        }

        /// <summary>
        /// 初始化，扫描 "Resources/HeroDatas" 文件夹下的所有赛季目录。
        /// </summary>
        private void InitializePaths()
        {
            string parentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "HeroDatas");
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
                Paths = Array.Empty<string>();
                return;
            }
            Paths = Directory.GetDirectories(parentPath);
        }

        /// <summary>
        /// 获取当前赛季的完整路径。
        /// </summary>
        /// <returns>返回匹配的路径，如果找不到则返回null。</returns>
        private string GetCurrentSeasonPath()
        {
            if (Paths == null || Paths.Length == 0)
            {
                return null;
            }

            return Paths[Math.Min(_pathIndex, Paths.Length - 1)];
        }

        public bool SetFilePathsIndex(string season)
        {
            int selectedIndex = 0;
            bool isFound = false;
            if (!string.IsNullOrEmpty(season))
            {
                for (int i = 0; i < Paths.Length; i++)
                {
                    if (Path.GetFileName(Paths[i]).Equals(season, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        isFound = true;
                        break;
                    }
                }
            }

            if (Paths.Length > 0)
            {
                _pathIndex = Math.Min(selectedIndex, Paths.Length - 1);
            }

            return isFound;
        }

        public DataClass.RecommendedEquipment GetHeroEquipmentFromName(string name)
        {
            return nameToHeroEquipmentMap.TryGetValue(name, out var hero) ? hero : null;
        }

        public List<Image> GetImagesFromHeroEquipment(DataClass.RecommendedEquipment heroEquipment)
        {
            return EquipmentImageMap.TryGetValue(heroEquipment, out var images) ? images : null;
        }

        private void BuildMap()
        {
            nameToHeroEquipmentMap.Clear();
            foreach (DataClass.RecommendedEquipment heroEquipment in HeroEquipments)
            {
                nameToHeroEquipmentMap[heroEquipment.HeroName] = heroEquipment;
            }
        }

        public void Load()
        {
            OutputForm.Instance.WriteLineOutputMessage("HeroEquipmentDataService: 准备加载当前赛季装备推荐数据...");
            LoadFromJson();
            LoadEquipmentImages();
            BuildMap();
            OutputForm.Instance.WriteLineOutputMessage("HeroEquipmentDataService: 数据加载完成。");
        }

        public void Save()
        {
            string currentSeasonPath = GetCurrentSeasonPath();
            if (string.IsNullOrEmpty(currentSeasonPath))
            {
                OutputForm.Instance.WriteLineOutputMessage("错误: HeroEquipmentDataService - 无法保存，因为找不到当前赛季路径。");
                return;
            }

            string filePath = Path.Combine(currentSeasonPath, "EquipmentData.json");

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // 创建包装对象
                EquipmentDataFile dataFile = new EquipmentDataFile
                {
                    UpdateTime = _lastUpdateTime,
                    Data = HeroEquipments.ToDictionary(he => he.HeroName, he => he.Equipments)
                };

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string json = JsonSerializer.Serialize(dataFile, options);
                File.WriteAllText(filePath, json);
                OutputForm.Instance.WriteLineOutputMessage($"成功将 {dataFile.Data.Count} 条装备数据保存到 {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"装备配置文件 \"EquipmentData.json\" 保存失败。\n路径：{filePath}\n错误信息: {ex.Message}",
                               "文件保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ReLoad()
        {
            OutputForm.Instance.WriteLineOutputMessage("HeroEquipmentDataService: 正在执行 ReLoad...");
            HeroEquipments.Clear();
            EquipmentImageMap.Clear();
            Load();
        }

        public void UpdateDataFromCrawling(List<DataClass.RecommendedEquipment> crawledData)
        {
            if (crawledData == null)
            {
                OutputForm.Instance.WriteLineOutputMessage("警告: UpdateDataFromCrawling 接收到的数据为 null，已中止更新。");
                return;
            }
            OutputForm.Instance.WriteLineOutputMessage($"HeroEquipmentDataService: 接收到 {crawledData.Count} 条从网络爬取的新数据。");
            HeroEquipments = new List<DataClass.RecommendedEquipment>(crawledData);
            _lastUpdateTime = DateTime.Now;
            OutputForm.Instance.WriteLineOutputMessage("正在将新数据保存到本地文件...");
            Save();
        }

        /// <summary>
        /// 获取数据最后更新时间
        /// </summary>
        public DateTime GetLastUpdateTime()
        {
            return _lastUpdateTime;
        }

        /// <summary>
        /// 检查数据是否需要更新
        /// </summary>
        /// <param name="hours">更新间隔小时数</param>
        /// <returns>true 表示需要更新</returns>
        public bool NeedsUpdate(int hours = 12)
        {
            if (_lastUpdateTime == DateTime.MinValue)
            {
                return true;
            }

            return (DateTime.Now - _lastUpdateTime).TotalHours >= hours;
        }

        private void LoadFromJson()
        {
            HeroEquipments.Clear();
            string currentSeasonPath = GetCurrentSeasonPath();
            if (string.IsNullOrEmpty(currentSeasonPath))
            {
                OutputForm.Instance.WriteLineOutputMessage("警告: HeroEquipmentDataService - 无法加载JSON，因为找不到目标赛季路径。");
                return;
            }

            string filePath = Path.Combine(currentSeasonPath, "EquipmentData.json");

            try
            {
                if (!File.Exists(filePath))
                {
                    OutputForm.Instance.WriteLineOutputMessage($"提示: 文件 {filePath} 不存在，将创建一个新的空文件。");
                    Save();
                    return;
                }

                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    OutputForm.Instance.WriteLineOutputMessage($"提示: 文件 {filePath} 内容为空。");
                    return;
                }

                EquipmentDataFile dataFile = JsonSerializer.Deserialize<EquipmentDataFile>(json);
                if (dataFile == null || dataFile.Data == null || dataFile.Data.Count == 0)
                {
                    OutputForm.Instance.WriteLineOutputMessage($"提示: 文件 {filePath} 未包含有效的 Data 字段。");
                    return;
                }

                HeroEquipments = dataFile.Data
                    .Select(kvp => new DataClass.RecommendedEquipment { HeroName = kvp.Key, Equipments = kvp.Value })
                    .ToList();
                _lastUpdateTime = dataFile.UpdateTime;
                OutputForm.Instance.WriteLineOutputMessage($"成功从 {filePath} 加载了 {HeroEquipments.Count} 位英雄的装备数据（更新时间: {_lastUpdateTime}）。");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"装备配置文件 \"EquipmentData.json\" 格式错误或无法读取。\n路径：{filePath}\n错误信息: {ex.Message}\n将创建一个新的空文件。",
                               "文件加载错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HeroEquipments.Clear();
                Save();
            }
        }

        private void LoadEquipmentImages()
        {
            EquipmentImageMap.Clear();
            string currentSeasonPath = GetCurrentSeasonPath();
            if (string.IsNullOrEmpty(currentSeasonPath))
            {
                return;
            }

            string imagesFolderPath = Path.Combine(currentSeasonPath, "EquipmentImages");
            if (!Directory.Exists(imagesFolderPath))
            {
                OutputForm.Instance.WriteLineOutputMessage($"警告: 装备图片文件夹不存在: {imagesFolderPath}");
                return;
            }

            StringBuilder errors = new StringBuilder();
            foreach (var heroEquipment in HeroEquipments)
            {
                var imageListForHero = new List<Image>();
                foreach (var equipmentName in heroEquipment.Equipments)
                {
                    try
                    {
                        string imagePath = Path.Combine(imagesFolderPath, $"{equipmentName}.png");
                        Image image = Image.FromFile(imagePath);
                        imageListForHero.Add(image);
                    }
                    catch
                    {
                        errors.AppendLine($"图片缺失或损坏: {equipmentName}.png");
                        imageListForHero.Add(new Bitmap(64, 64));
                    }
                }
                EquipmentImageMap[heroEquipment] = imageListForHero;
            }

            if (errors.Length > 0)
            {
                string header = $"加载装备图片时发生错误。\n图片路径: {imagesFolderPath}\n\n";
                MessageBox.Show($"{header}{errors.ToString()}", "图片加载错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            OutputForm.Instance.WriteLineOutputMessage($"成功为 {EquipmentImageMap.Count} 位英雄构建了装备图片映射。");
        }
    }
}
