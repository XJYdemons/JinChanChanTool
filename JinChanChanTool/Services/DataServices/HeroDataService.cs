using System.Text;
using System.Text.Json;
using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices
{
    public class HeroDataService : IHeroDataService
    {
        /// <summary>
        /// 本地文件路径列表
        /// </summary>
        public string[] Paths { get; set; }

        /// <summary>
        /// 文件路径索引
        /// </summary>
        public int PathIndex { get; set; }

        /// <summary>
        /// 默认图片路径
        /// </summary>
        public string DefaultImagePath { get; set; }

        /// <summary>
        /// 英雄数据对象列表
        /// </summary>
        public List<HeroData> HeroDatas { get; private set; }

        /// <summary>
        /// 英雄头像图片列表
        /// </summary>
        public List<Image> HeroImages { get; private set; }

        /// <summary>
        /// 职业对象列表
        /// </summary>
        public List<Profession> Professions { get; private set; }

        /// <summary>
        /// 特质对象列表
        /// </summary>
        public List<Peculiarity> Peculiarities { get; private set; }

        /// <summary>
        /// 图片到英雄数据对象的字典
        /// </summary>
        public Dictionary<Image, HeroData> ImageToHeroDataMap { get; private set; }

        /// <summary>
        /// 英雄数据对象到图片的字典
        /// </summary>
        public Dictionary<HeroData, Image> HeroDataToImageMap { get; private set; }
        
        public HeroDataService()
        {
            InitializePaths();            
            PathIndex = 0;           
            HeroDatas = new List<HeroData>();
            HeroImages = new List<Image>();
            Professions = new List<Profession>();
            Peculiarities = new List<Peculiarity>();
            ImageToHeroDataMap = new Dictionary<Image, HeroData>();
            HeroDataToImageMap = new Dictionary<HeroData, Image>();
        }

        /// <summary>
        /// 初始化本地文件路径列表与默认图片路径。
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
            Paths = subDirs;            
            DefaultImagePath = Path.Combine(Application.StartupPath, "Resources", "defaultHeroIcon.png");
        }

        /// <summary>
        /// 从本地加载到对象
        /// </summary>
        public void Load()
        {
            LoadFromJson();
            LoadImages();
            LoadProfessions();
            LoadPeculiarity();
            BuildImageAndHeroDataMap();
        }

        /// <summary>
        /// 建立字典联系
        /// </summary>
        private void BuildImageAndHeroDataMap()
        {
            for (int i = 0; i < HeroImages.Count; i++)
            {
                ImageToHeroDataMap[HeroImages[i]] = HeroDatas[i];
            }
            for (int i = 0; i < HeroDatas.Count; i++)
            {
                HeroDataToImageMap[HeroDatas[i]] = HeroImages[i];
            }
        }

        /// <summary>
        /// 从本地文件加载到对象列表，若失败则创建空的文件覆盖本地文件。
        /// </summary>
        private void LoadFromJson()
        {
            HeroDatas.Clear();
            if (Paths.Length > 0&&PathIndex<Paths.Length)
            {
                try
                {
                    string filePath = Path.Combine(Paths[PathIndex], "HeroData.json");
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"找不到英雄配置文件\"HeroData.json\"\n路径：\n{filePath}\n将创建新的文件。",
                                    "文件缺失",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                    );
                        Save();
                        return;
                    }
                    
                    string json = File.ReadAllText(filePath);
                    if(string.IsNullOrEmpty(json))
                    {
                        MessageBox.Show($"英雄配置文件\"HeroData.json\"内容为空。\n路径：\n{filePath}\n将创建新的文件。",
                                   "文件为空",
                                   MessageBoxButtons.OK, 
                                   MessageBoxIcon.Error
                                   );
                        Save();
                        return;
                    }
                    List<HeroData> temp =JsonSerializer.Deserialize<List<HeroData>>(json);
                    // 排序逻辑
                    temp
                        .OrderBy(h => h.Cost)//根据Cost排序
                        .ThenBy(h => h.HeroName) // Cost相同时按名称排序
                        .ToList();
                    HeroDatas = temp;
                }
                catch 
                {
                    MessageBox.Show($"英雄配置文件\"HeroData.json\"格式错误\n路径：\n{Path.Combine(Paths[PathIndex], "HeroData.json")}\n将创建新的文件。",
                                   "文件格式错误",
                                   MessageBoxButtons.OK, 
                                   MessageBoxIcon.Error
                                   );
                    Save();
                }
            }
            else
            {
                MessageBox.Show($"英雄配置文件夹不存在",
                    "文件夹不存在",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
        }

        /// <summary>
        /// 从英雄数据对象列表加载到职业对象列表
        /// </summary>
        private void LoadProfessions()
        {
            Professions.Clear();
            for (int i = 0; i < HeroDatas.Count; i++)
            {
                string[] result = HeroDatas[i].Profession.Split('|', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < result.Length; j++)
                {
                    var existingGroup = Professions.FirstOrDefault(p => string.Equals(p.Title, result[j], StringComparison.OrdinalIgnoreCase));
                    if (existingGroup != null)
                    {
                        existingGroup.HeroDatas.Add(HeroDatas[i]);
                    }
                    else
                    {
                        var newObject = new Profession
                        {
                            Title = result[j],
                            HeroDatas = new List<HeroData>()                          
                        };
                        newObject.HeroDatas.Add(HeroDatas[i]);
                        Professions.Add(newObject);
                    }
                }
            }
        }

        /// <summary>
        /// 从英雄数据对象列表加载到特质对象列表
        /// </summary>
        private void LoadPeculiarity()
        {
            Peculiarities.Clear();
            for (int i = 0; i < HeroDatas.Count; i++)
            {
                string[] result = HeroDatas[i].Peculiarity.Split('|', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < result.Length; j++)
                {
                    var existingGroup = Peculiarities.FirstOrDefault(p => string.Equals(p.Title, result[j], StringComparison.OrdinalIgnoreCase));
                    if (existingGroup != null)
                    {
                        existingGroup.HeroDatas.Add(HeroDatas[i]);
                    }
                    else
                    {
                        var newObject = new Peculiarity
                        {
                            Title = result[j],
                            HeroDatas = new List<HeroData>()
                        };
                        newObject.HeroDatas.Add(HeroDatas[i]);
                        Peculiarities.Add(newObject);
                    }
                }
            }
        }

        /// <summary>
        /// 从本地加载图片到英雄头像图片列表
        /// </summary>
        private void LoadImages()
        {
            HeroImages.Clear();
            // 使用StringBuilder收集错误信息            
            StringBuilder errors = new StringBuilder();
            string head = "";
            bool isCommonImageLost = false;
            bool isDefautlImageLost = false;
            for (int i = 0;i<HeroDatas.Count;i++)
            {
                try
                {                           
                    string imagePath = Path.Combine(Paths[PathIndex], "images", $"{HeroDatas[i].HeroName}.png");
                    Image image = Image.FromFile(imagePath);
                    HeroImages.Add(image);                                                                      
                }
                catch
                {
                    isCommonImageLost=true;
                    // 收集错误信息
                    errors.AppendLine($"图片缺失：{HeroDatas[i].HeroName}.png");                    
                    try
                    {                      
                        Image image = Image.FromFile(DefaultImagePath);
                        HeroImages.Add(image);
                    }
                    catch
                    {
                        isDefautlImageLost=true;                        
                        // 添加替代图片避免崩溃
                        HeroImages.Add(new Bitmap(64, 64));
                    }
                    
                }
            }

            // 如果有任何错误，弹出综合消息
            if (errors.Length > 0)
            {
                if(isCommonImageLost)
                {
                    head += $"缺失图片所在路径：{Path.Combine(Paths[PathIndex], "images")}\n";
                }
                if(isDefautlImageLost)
                {
                    head += $"缺失默认图片：{Path.Combine(Application.StartupPath, "Resources", "defaultHeroIcon.png")}\n";
                }
                MessageBox.Show($"{head}{errors}",
                                "图片加载错误",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 从对象保存到本地
        /// </summary>
        public void Save()
        {
            if (Paths.Length > 0 && PathIndex < Paths.Length)
            {
                try 
                {
                    string filePath = Path.Combine(Paths[PathIndex], "HeroData.json");
                    // 设置 JsonSerializerOptions 以保持中文字符的可读性
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true, // 格式化输出
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 保留中文字符
                    };
                    // 序列化数据
                    string json = JsonSerializer.Serialize(HeroDatas, options);
                    File.WriteAllText(filePath, json);
                }
                catch
                {
                    MessageBox.Show($"英雄配置文件\"HeroData.json\"保存失败\n路径：\n{Path.Combine(Paths[PathIndex], "HeroData.json")}",
                                  "文件保存失败",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error
                                  );
                }
            }
            else
            {
                MessageBox.Show($"英雄配置文件夹不存在",
                                   "文件夹不存在",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
            }              
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        public void ReLoad()
        {
            HeroDatas.Clear();
            HeroImages.Clear();
            Professions.Clear();
            Peculiarities.Clear();
            ImageToHeroDataMap.Clear();
            HeroDataToImageMap.Clear();
            Load();
        }
    }
}
