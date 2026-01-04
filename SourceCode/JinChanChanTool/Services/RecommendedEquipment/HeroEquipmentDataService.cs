using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace JinChanChanTool.Services.RecommendedEquipment
{
    /// <summary>
    /// 负责管理本地的装备推荐数据，包括从JSON文件加载、保存数据，以及加载相关的装备图片。
    /// 这个服务现在通过硬编码的赛季名来定位目标文件夹，确保操作的确定性。
    /// </summary>
    public class HeroEquipmentDataService : IHeroEquipmentDataService
    {
        // 修改：硬编码目标赛季文件夹名 
        // 以后赛季更新时，只需要修改下面这一行字符串即可。
        private readonly string _targetSeasonFolderName = "英雄联盟传奇";

        public string[] Paths { get; set; }
        public List<DataClass.RecommendedEquipment> HeroEquipments { get; private set; }
        public Dictionary<DataClass.RecommendedEquipment, List<Image>> EquipmentImageMap { get; private set; }
        private Dictionary<string, DataClass.RecommendedEquipment> nameToHeroEquipmentMap { get; set; }

        /// <summary>
        /// 构造函数，初始化属性并扫描所有可用的赛季路径。
        /// </summary>
        public HeroEquipmentDataService()
        {
            InitializePaths();
            HeroEquipments = new List<DataClass.RecommendedEquipment>();
            EquipmentImageMap = new Dictionary<DataClass.RecommendedEquipment, List<Image>>();
            nameToHeroEquipmentMap = new Dictionary<string, DataClass.RecommendedEquipment>();
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
        /// 获取当前目标赛季的完整路径。
        /// 优先在所有找到的路径中，寻找与 _targetSeasonFolderName 匹配的那个。
        /// </summary>
        /// <returns>返回匹配的路径，如果找不到则返回null。</returns>
        private string GetCurrentSeasonPath()
        {
            if (Paths == null || Paths.Length == 0)
            {
                return null;
            }

            // 使用 LINQ 的 FirstOrDefault 在所有扫描到的目录中查找与目标赛季名匹配的路径。
            // ToLower() 这样就用于不区分大小写的比较。
            string? targetPath = Paths.FirstOrDefault(p =>new DirectoryInfo(p).Name.Trim().ToLower() == _targetSeasonFolderName.Trim().ToLower());

            if (!string.IsNullOrEmpty(targetPath))
            {
                return targetPath; // 找到了匹配的文件夹路径
            }

            // 如果找不到，输出警告，让开发者知道需要创建对应的文件夹。
            Debug.WriteLine($"警告: 未在 Resources/HeroDatas/ 中找到名为 '{_targetSeasonFolderName}' 的赛季文件夹。");
            LogTool.Log($"警告: 未在 Resources/HeroDatas/ 中找到名为 '{_targetSeasonFolderName}' 的赛季文件夹。");
            OutputForm.Instance.WriteLineOutputMessage($"警告: 未在 Resources/HeroDatas/ 中找到名为 '{_targetSeasonFolderName}' 的赛季文件夹。");
            return null;
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
            Debug.WriteLine($"HeroEquipmentDataService: 准备从目标赛季 '{_targetSeasonFolderName}' 加载数据...");
            LogTool.Log($"HeroEquipmentDataService: 准备从目标赛季 '{_targetSeasonFolderName}' 加载数据...");
            OutputForm.Instance.WriteLineOutputMessage($"HeroEquipmentDataService: 准备从目标赛季 '{_targetSeasonFolderName}' 加载数据...");
            LoadFromJson();
            LoadEquipmentImages();
            BuildMap();
            Debug.WriteLine("HeroEquipmentDataService: 数据加载完成。");
            LogTool.Log("HeroEquipmentDataService: 数据加载完成。");
            OutputForm.Instance.WriteLineOutputMessage("HeroEquipmentDataService: 数据加载完成。");
        }

        public void Save()
        {
            string currentSeasonPath = GetCurrentSeasonPath();
            if (string.IsNullOrEmpty(currentSeasonPath))
            {
                Debug.WriteLine($"错误: HeroEquipmentDataService - 无法保存，因为找不到目标赛季路径 '{_targetSeasonFolderName}'。");
                LogTool.Log($"错误: HeroEquipmentDataService - 无法保存，因为找不到目标赛季路径 '{_targetSeasonFolderName}'。");
                OutputForm.Instance.WriteLineOutputMessage($"错误: HeroEquipmentDataService - 无法保存，因为找不到目标赛季路径 '{_targetSeasonFolderName}'。");
                return;
            }

            string filePath = Path.Combine(currentSeasonPath, "EquipmentData.json");

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                var dataToSave = HeroEquipments.ToDictionary(he => he.HeroName, he => he.Equipments);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string json = JsonSerializer.Serialize(dataToSave, options);
                File.WriteAllText(filePath, json);
                Debug.WriteLine($"成功将 {dataToSave.Count} 条装备数据保存到 {filePath}");
                LogTool.Log($"成功将 {dataToSave.Count} 条装备数据保存到 {filePath}");
                OutputForm.Instance.WriteLineOutputMessage($"成功将 {dataToSave.Count} 条装备数据保存到 {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"装备配置文件 \"EquipmentData.json\" 保存失败。\n路径：{filePath}\n错误信息: {ex.Message}",
                               "文件保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ReLoad()
        {
            Debug.WriteLine("HeroEquipmentDataService: 正在执行 ReLoad...");
            LogTool.Log("HeroEquipmentDataService: 正在执行 ReLoad...");
            OutputForm.Instance.WriteLineOutputMessage("HeroEquipmentDataService: 正在执行 ReLoad...");
            HeroEquipments.Clear();
            EquipmentImageMap.Clear();
            Load();
        }

        public void UpdateDataFromCrawling(List<DataClass.RecommendedEquipment> crawledData)
        {
            if (crawledData == null)
            {
                Debug.WriteLine("警告: UpdateDataFromCrawling 接收到的数据为 null，已中止更新。");
                LogTool.Log("警告: UpdateDataFromCrawling 接收到的数据为 null，已中止更新。");
                OutputForm.Instance.WriteLineOutputMessage("警告: UpdateDataFromCrawling 接收到的数据为 null，已中止更新。");
                return;
            }
            Debug.WriteLine($"HeroEquipmentDataService: 接收到 {crawledData.Count} 条从网络爬取的新数据。");
            LogTool.Log($"HeroEquipmentDataService: 接收到 {crawledData.Count} 条从网络爬取的新数据。");
            OutputForm.Instance.WriteLineOutputMessage($"HeroEquipmentDataService: 接收到 {crawledData.Count} 条从网络爬取的新数据。");
            HeroEquipments = new List<DataClass.RecommendedEquipment>(crawledData);
            Debug.WriteLine("正在将新数据保存到本地文件...");
            LogTool.Log("正在将新数据保存到本地文件...");
            OutputForm.Instance.WriteLineOutputMessage("正在将新数据保存到本地文件...");
            Save();
        }

        private void LoadFromJson()
        {
            HeroEquipments.Clear();
            string currentSeasonPath = GetCurrentSeasonPath();
            if (string.IsNullOrEmpty(currentSeasonPath))
            {
                Debug.WriteLine("警告: HeroEquipmentDataService - 无法加载JSON，因为找不到目标赛季路径。");
                LogTool.Log("警告: HeroEquipmentDataService - 无法加载JSON，因为找不到目标赛季路径。");
                OutputForm.Instance.WriteLineOutputMessage("警告: HeroEquipmentDataService - 无法加载JSON，因为找不到目标赛季路径。");
                return;
            }

            string filePath = Path.Combine(currentSeasonPath, "EquipmentData.json");

            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"提示: 文件 {filePath} 不存在，将创建一个新的空文件。");
                    LogTool.Log($"提示: 文件 {filePath} 不存在，将创建一个新的空文件。");
                    OutputForm.Instance.WriteLineOutputMessage($"提示: 文件 {filePath} 不存在，将创建一个新的空文件。");
                    Save();
                    return;
                }

                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.WriteLine($"提示: 文件 {filePath} 内容为空。");
                    LogTool.Log($"提示: 文件 {filePath} 内容为空。");
                    OutputForm.Instance.WriteLineOutputMessage($"提示: 文件 {filePath} 内容为空。");
                    return;
                }

                var dataDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
                if (dataDict != null)
                {
                    HeroEquipments = dataDict
                        .Select(kvp => new RecommendedEquipment { HeroName = kvp.Key, Equipments = kvp.Value })
                        .ToList();
                    Debug.WriteLine($"成功从 {filePath} 加载了 {HeroEquipments.Count} 位英雄的装备数据。");
                    LogTool.Log($"成功从 {filePath} 加载了 {HeroEquipments.Count} 位英雄的装备数据。");
                    OutputForm.Instance.WriteLineOutputMessage($"成功从 {filePath} 加载了 {HeroEquipments.Count} 位英雄的装备数据。");
                }
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
                Debug.WriteLine($"警告: 装备图片文件夹不存在: {imagesFolderPath}");
                LogTool.Log($"警告: 装备图片文件夹不存在: {imagesFolderPath}");
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

            Debug.WriteLine($"成功为 {EquipmentImageMap.Count} 位英雄构建了装备图片映射。");
            LogTool.Log($"成功为 {EquipmentImageMap.Count} 位英雄构建了装备图片映射。");
            OutputForm.Instance.WriteLineOutputMessage($"成功为 {EquipmentImageMap.Count} 位英雄构建了装备图片映射。");
        }
    }
}