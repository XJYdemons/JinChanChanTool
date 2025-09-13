using JinChanChanTool.DataClass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;

namespace JinChanChanTool.Services.DataServices
{
    /// <summary>
    /// 负责管理本地的装备推荐数据，包括从JSON文件加载、保存数据，以及加载相关的装备图片。
    /// </summary>
    public class HeroEquipmentDataService : IHeroEquipmentDataService
    {
        //  接口定义的公共属性 
        public string[] Paths { get; set; }
        public int PathIndex { get; set; }
        public List<HeroEquipment> HeroEquipments { get; private set; }
        public Dictionary<HeroEquipment, List<Image>> EquipmentImageMap { get; private set; }
        private Dictionary<string,HeroEquipment> nameToHeroEquipmentMap { get; set; }
        /// <summary>
        /// 构造函数，初始化属性并设置路径。
        /// </summary>
        public HeroEquipmentDataService()
        {
            InitializePaths();
            PathIndex = 0; // 默认选择第一个赛季
            HeroEquipments = new List<HeroEquipment>();
            EquipmentImageMap = new Dictionary<HeroEquipment, List<Image>>();
            nameToHeroEquipmentMap= new Dictionary<string,HeroEquipment>();
            
        }

        /// <summary>
        /// (核心方法) 初始化所有可用的赛季（数据文件夹）路径。
        /// </summary>
        private void InitializePaths()
        {
            // 使用相对路径构建 "Resources/HeroDatas" 文件夹的路径
            string parentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "HeroDatas");

            if (!Directory.Exists(parentPath))
            {
                // 如果 HeroDatas 文件夹本身就不存在，创建一个空的
                Directory.CreateDirectory(parentPath);
                Paths = Array.Empty<string>(); // 路径列表为空
                return;
            }

            // 获取所有子目录的完整路径，这些子目录代表了不同的赛季
            Paths = Directory.GetDirectories(parentPath);
        }
        
        public HeroEquipment GetHeroEquipmentFromName(string name)
        {
            if(nameToHeroEquipmentMap.ContainsKey(name))
            {
                return nameToHeroEquipmentMap[name];
            }
            else
            {
                return null;
            }
        }

        public List<Image> GetImagesFromHeroEquipment(HeroEquipment heroEquipment)
        {
            if(EquipmentImageMap.ContainsKey(heroEquipment))
            {
                return EquipmentImageMap[heroEquipment];
            }
            else
            {
                return null; 
            }
        }
        private void BuildMap()
        {
            foreach(HeroEquipment heroEquipment in HeroEquipments)
            {
                nameToHeroEquipmentMap[heroEquipment.HeroName] = heroEquipment;
            }
        }

        /// <summary>
        /// 从当前PathIndex指定的本地路径，加载所有数据（JSON和图片）。
        /// 这是提供给外部（如UI层）调用的主要数据加载入口。
        /// </summary>
        public void Load()
        {
            System.Diagnostics.Debug.WriteLine($"HeroEquipmentDataService: 开始加载赛季索引 {PathIndex} 的数据...");

            // 步骤 1: 从 EquipmentData.json 文件加载英雄和装备的文本信息。
            // 这个方法会填充 HeroEquipments 列表。
            LoadFromJson();

            // 步骤 2: 根据刚刚加载的文本信息，去加载对应的装备图片。
            // 这个方法会使用 HeroEquipments 列表来构建 EquipmentImageMap 字典。
            LoadEquipmentImages();
            BuildMap();
            System.Diagnostics.Debug.WriteLine("HeroEquipmentDataService: 数据加载完成。");
        }

        /// <summary>
        /// 将内存中的 HeroEquipments 数据保存到当前PathIndex指定的本地JSON文件中。
        /// </summary>
        public void Save()
        {
            if (Paths == null || Paths.Length == 0 || PathIndex >= Paths.Length)
            {
                System.Diagnostics.Debug.WriteLine("错误: HeroEquipmentDataService - 无法保存，没有任何有效的赛季路径。");
                return;
            }

            string filePath = Path.Combine(Paths[PathIndex], "EquipmentData.json");

            try
            {
                // 确保要写入的目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // 为了匹配JSON结构 (英雄名 -> 装备列表), 
                // 需要先将 List<HeroEquipment> 转换回 Dictionary<string, List<string>>
                var dataToSave = HeroEquipments
                    .OrderBy(he => he.HeroName) // 保存前按英雄名排序，确保JSON文件内容顺序稳定
                    .ToDictionary(he => he.HeroName, he => he.Equipments);

                // 设置JsonSerializerOptions以保持中文字符的可读性和格式化
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true, // 格式化输出 (带缩进)
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 保留中文字符，不进行编码
                };

                // 序列化数据并写入文件
                string json = JsonSerializer.Serialize(dataToSave, options);
                File.WriteAllText(filePath, json);
                System.Diagnostics.Debug.WriteLine($"成功将 {dataToSave.Count} 条装备数据保存到 {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"装备配置文件 \"EquipmentData.json\" 保存失败。\n路径：{filePath}\n错误信息: {ex.Message}",
                               "文件保存失败",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///  清空所有内存中的数据，然后重新执行 Load() 方法。
        /// 用于在数据更新或切换赛季后刷新服务状态。
        /// </summary>
        public void ReLoad()
        {
            System.Diagnostics.Debug.WriteLine("HeroEquipmentDataService: 正在执行 ReLoad...");

            // 清空所有核心数据集合
            HeroEquipments.Clear();
            EquipmentImageMap.Clear();

            // 重新调用 Load() 方法，以加载新路径或已更新文件的数据
            Load();
        }

        /// <summary>
        /// 接收从网络爬取到的新数据，更新内存中的状态，并将其持久化保存。
        /// </summary>
        /// <param name="crawledData">由 ICrawlingService 获取到的最新英雄装备数据列表。</param>
        public void UpdateDataFromCrawling(List<HeroEquipment> crawledData)
        {
            // 检查传入的数据是否有效
            if (crawledData == null)
            {
                System.Diagnostics.Debug.WriteLine("警告: UpdateDataFromCrawling 接收到的数据为 null，已中止更新。");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"HeroEquipmentDataService: 接收到 {crawledData.Count} 条从网络爬取的新数据。");

            // 用新的数据完全覆盖内存中的旧数据
            // 创建一个新的列表实例，而不是直接赋值，这在某些UI绑定场景下更安全
            this.HeroEquipments = new List<HeroEquipment>(crawledData);

            // 调用 Save() 方法，将刚刚更新到内存的数据存储JSON文件中
            System.Diagnostics.Debug.WriteLine("正在将新数据保存到本地文件...");
            Save();
        }



    

        /// <summary>
        /// 从 EquipmentData.json 文件加载数据并填充 HeroEquipments 列表。
        /// </summary>
        private void LoadFromJson()
        {
            HeroEquipments.Clear(); // 每次加载前先清空旧数据

            // 检查路径列表是否有效，以及当前索引是否在范围内
            if (Paths == null || Paths.Length == 0 || PathIndex >= Paths.Length)
            {
                // 这里不弹出消息框，让UI层决定如何处理无数据的情况
                System.Diagnostics.Debug.WriteLine("警告: HeroEquipmentDataService - 没有任何有效的赛季路径可供加载。");
                return;
            }

            // 构建当前选中赛季的 EquipmentData.json 文件的完整路径
            string filePath = Path.Combine(Paths[PathIndex], "EquipmentData.json");

            try
            {
                if (!File.Exists(filePath))
                {
                    // 如果文件不存在，可能是第一次运行或数据被删除。创建一个空的。
                    System.Diagnostics.Debug.WriteLine($"提示: 文件 {filePath} 不存在，将创建一个新的空文件。");
                    Save(); // 调用Save会创建一个空的JSON文件
                    return;
                }

                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    // 文件存在但内容为空
                    System.Diagnostics.Debug.WriteLine($"提示: 文件 {filePath} 内容为空。");
                    return; // HeroEquipments 保持为空
                }

                // 反序列化JSON到字典，JSON结构是 英雄名 -> 装备列表
                var dataDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

                if (dataDict != null)
                {
                    // 将字典转换为我们标准的 List<HeroEquipment> 格式
                    HeroEquipments = dataDict
                        .Select(kvp => new HeroEquipment { HeroName = kvp.Key, Equipments = kvp.Value })
                        .OrderBy(he => he.HeroName) // 按英雄名排序，保持列表顺序稳定
                        .ToList();
                    System.Diagnostics.Debug.WriteLine($"成功从 {filePath} 加载了 {HeroEquipments.Count} 位英雄的装备数据。");
                }
            }
            catch (Exception ex)
            {
                // 参考 HeroDataService，在出错时弹窗提示并尝试创建一个新的空文件
                MessageBox.Show($"装备配置文件 \"EquipmentData.json\" 格式错误或无法读取。\n路径：{filePath}\n错误信息: {ex.Message}\n将创建一个新的空文件。",
                               "文件加载错误",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                HeroEquipments.Clear(); // 确保数据被清空
                Save(); // 尝试保存一个干净的空文件来覆盖损坏的文件
            }
        }

        /// <summary>
        ///根据 HeroEquipments 列表加载装备图片，并构建 EquipmentImageMap。
        /// </summary>
        private void LoadEquipmentImages()
        {
            EquipmentImageMap.Clear(); // 每次加载前先清空旧的映射

            if (Paths == null || Paths.Length == 0 || PathIndex >= Paths.Length)
            {
                return; // 没有有效路径，无法加载图片
            }

            // 构建当前选中赛季的装备图片文件夹的完整路径
            string imagesFolderPath = Path.Combine(Paths[PathIndex], "EquipmentImages");
            if (!Directory.Exists(imagesFolderPath))
            {
                System.Diagnostics.Debug.WriteLine($"警告: 装备图片文件夹不存在: {imagesFolderPath}");
                return;
            }

            // 参照HeroDataService中的错误处理模式，用于收集所有加载失败的图片信息
            StringBuilder errors = new StringBuilder();

            // 遍历每一个英雄的装备信息
            foreach (var heroEquipment in this.HeroEquipments)
            {
                var imageListForHero = new List<Image>();

                // 遍历该英雄的每一件推荐装备
                foreach (var equipmentName in heroEquipment.Equipments)
                {
                    try
                    {
                        // 构建单个装备图片的完整路径 (例如 ...\EquipmentImages\无尽之刃.png)
                        string imagePath = Path.Combine(imagesFolderPath, $"{equipmentName}.png");

                        Image image = Image.FromFile(imagePath);
                        imageListForHero.Add(image);
                    }
                    catch
                    {
                        // 如果图片加载失败 (文件不存在、文件损坏等)
                        errors.AppendLine($"图片缺失或损坏: {equipmentName}.png");
                        // 添加一个占位符图片，以防UI崩溃
                        imageListForHero.Add(new Bitmap(64, 64)); // 创建一个64x64的空白图片作为替代
                    }
                }

                // 将当前英雄的装备信息对象和其对应的图片列表，添加到映射字典中
                EquipmentImageMap[heroEquipment] = imageListForHero;
            }

            // 如果在加载过程中收集到了任何错误，就弹出一个汇总的消息框
            if (errors.Length > 0)
            {
                string header = $"加载装备图片时发生错误。\n图片路径: {imagesFolderPath}\n\n";
                MessageBox.Show($"{header}{errors.ToString()}",
                                "图片加载错误",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }

            System.Diagnostics.Debug.WriteLine($"成功为 {EquipmentImageMap.Count} 位英雄构建了装备图片映射。");
        }

    }
}