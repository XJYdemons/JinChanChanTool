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
        private string[] paths;

        /// <summary>
        /// 文件路径索引
        /// </summary>
        private int pathIndex;

        /// <summary>
        /// 默认图片路径
        /// </summary>
        private string defaultImagePath;

        /// <summary>
        /// 英雄数据对象列表
        /// </summary>
        public List<HeroData> HeroDatas;

        /// <summary>
        /// 英雄头像图片列表
        /// </summary>
        private List<Image> HeroImages;

        /// <summary>
        /// 职业对象列表
        /// </summary>
        private List<Profession> professions;

        /// <summary>
        /// 特质对象列表
        /// </summary>
        private List<Peculiarity> peculiarities;

        /// <summary>
        /// 图片到英雄数据对象的字典
        /// </summary>
        private Dictionary<Image, HeroData> imageToHeroDataMap;

        /// <summary>
        /// 英雄数据对象到图片的字典
        /// </summary>
        private Dictionary<HeroData, Image> heroDataToImageMap;

        /// <summary>
        /// 英雄名称到对象的字典
        /// </summary>
        private Dictionary<string, HeroData> nameToHeroDataMap;

        /// <summary>
        /// 英雄名字符哈希表
        /// </summary>
        private HashSet<char> _charDictionary;
        #region 初始化
        public HeroDataService()
        {
            InitializePaths();            
            pathIndex = 0;           
            HeroDatas = new List<HeroData>();
            HeroImages = new List<Image>();
            professions = new List<Profession>();
            peculiarities = new List<Peculiarity>();
            imageToHeroDataMap = new Dictionary<Image, HeroData>();
            heroDataToImageMap = new Dictionary<HeroData, Image>();
            nameToHeroDataMap = new Dictionary<string, HeroData>();
            _charDictionary = new HashSet<char>();
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
            paths = subDirs;
            defaultImagePath = Path.Combine(Application.StartupPath, "Resources", "defaultHeroIcon.png");
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 从本地加载到对象
        /// </summary>
        public void Load()
        {
            LoadFromJson();
            LoadImages();
            LoadProfessions();
            LoadPeculiarity();
            BuildMap();
            LoadCharLib();
        }

        /// <summary>
        /// 从对象保存到本地
        /// </summary>
        public void Save()
        {
            if (paths.Length > 0 && pathIndex < paths.Length)
            {
                try
                {
                    string filePath = Path.Combine(paths[pathIndex], "HeroData.json");
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
                    MessageBox.Show($"英雄配置文件\"HeroData.json\"保存失败\n路径：\n{Path.Combine(paths[pathIndex], "HeroData.json")}",
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
            professions.Clear();
            peculiarities.Clear();
            imageToHeroDataMap.Clear();
            heroDataToImageMap.Clear();
            nameToHeroDataMap.Clear();
            _charDictionary.Clear();
            Load();
        }

        /// <summary>
        /// 从英雄名获取英雄对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HeroData GetHeroFromName(string name)
        {
            if (nameToHeroDataMap.TryGetValue(name, out HeroData hero))
            {
                return hero;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 从图像获取英雄对象
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public Image GetImageFromHero(HeroData hero)
        {
            if (heroDataToImageMap.TryGetValue(hero, out Image image))
            {
                return image;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 从英雄对象获取图像
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public HeroData GetHeroFromImage(Image image)
        {
            if (imageToHeroDataMap.TryGetValue(image, out HeroData hero))
            {
                return hero;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据索引删除英雄
        /// </summary>
        /// <param name="index"></param>
        public bool DeletHeroAtIndex(int index)
        {
            if (index < HeroDatas.Count && index >= 0)
            {
                HeroData hero = HeroDatas[index];
                if (heroDataToImageMap.ContainsKey(hero))
                {
                    Image image = heroDataToImageMap[hero];
                    heroDataToImageMap.Remove(hero);
                    if (imageToHeroDataMap.ContainsKey(image))
                    {
                        imageToHeroDataMap.Remove(image);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                if (nameToHeroDataMap.ContainsKey(hero.HeroName))
                {
                    nameToHeroDataMap.Remove(hero.HeroName);
                }
                else
                {
                    return true;
                }
                HeroDatas.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加英雄
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="image"></param>
        public bool AddHero(HeroData hero, Image image)
        {
            if (hero == null || image == null) return false;
            HeroDatas.Add(hero);
            heroDataToImageMap[hero] = image;
            imageToHeroDataMap[image] = hero;
            nameToHeroDataMap[hero.HeroName] = hero;
            return true;
        }

        /// <summary>
        /// 获取职业对象列表
        /// </summary>
        /// <returns></returns>
        public List<Profession> GetProfessions()
        {
            return professions;
        }

        /// <summary>
        /// 获取特质对象列表
        /// </summary>
        /// <returns></returns>
        public List<Peculiarity> GetPeculiarities()
        {
            return peculiarities;
        }

        /// <summary>
        /// 获取文件路径数组
        /// </summary>
        /// <returns></returns>
        public string[] GetFilePaths()
        {
            return paths;
        }

        /// <summary>
        /// 获取默认图片文件路径
        /// </summary>
        /// <returns></returns>
        public string GetDefaultImagePath()
        {
            return defaultImagePath;
        }

        /// <summary>
        /// 设置文件路径索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetFilePathsIndex(int index)
        {
            if (index >= 0 && index < paths.Length)
            {
                pathIndex = index;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取文件路径索引
        /// </summary>
        /// <returns></returns>
        public int GetFilePathsIndex()
        {
            return pathIndex;
        }

        /// <summary>
        /// 获取对应费用的英雄对象列表
        /// </summary>
        /// <returns></returns>
        public List<HeroData> GetHeroDatasFromCost(int cost)
        {
            return HeroDatas.Where(h => h.Cost == cost).ToList();
        }

        /// <summary>
        /// 获取英雄数量
        /// </summary>
        /// <returns></returns>
        public int GetHeroCount()
        {
            return HeroDatas.Count;
        }

        /// <summary>
        /// 获取英雄数据对象列表
        /// </summary>
        /// <returns></returns>
        public List<HeroData> GetHeroDatas()
        {
            return HeroDatas;
        }

        /// <summary>
        /// 获取英雄字符哈希表
        /// </summary>
        /// <returns></returns>
        public HashSet<char> GetCharDictionary()
        {
            return _charDictionary;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 从本地文件加载到对象列表，若失败则创建空的文件覆盖本地文件。
        /// </summary>
        private void LoadFromJson()
        {
            HeroDatas.Clear();
            if (paths.Length > 0 && pathIndex < paths.Length)
            {
                try
                {
                    string filePath = Path.Combine(paths[pathIndex], "HeroData.json");
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
                    if (string.IsNullOrEmpty(json))
                    {
                        MessageBox.Show($"英雄配置文件\"HeroData.json\"内容为空。\n路径：\n{filePath}\n将创建新的文件。",
                                   "文件为空",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
                        Save();
                        return;
                    }
                    List<HeroData> temp = JsonSerializer.Deserialize<List<HeroData>>(json);
                    // 排序逻辑
                    HeroDatas = temp
                                .OrderBy(h => h.Cost)
                                 .ToList();
                    // 设置 JsonSerializerOptions 以保持中文字符的可读性
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true, // 格式化输出
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 保留中文字符
                    };
                    // 序列化数据
                    string js = JsonSerializer.Serialize(HeroDatas, options);
                    File.WriteAllText(Path.Combine(Application.StartupPath, "Resources", "test.json"), js);
                }
                catch
                {
                    MessageBox.Show($"英雄配置文件\"HeroData.json\"格式错误\n路径：\n{Path.Combine(paths[pathIndex], "HeroData.json")}\n将创建新的文件。",
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
            for (int i = 0; i < HeroDatas.Count; i++)
            {
                try
                {
                    string imagePath = Path.Combine(paths[pathIndex], "images", $"{HeroDatas[i].HeroName}.png");
                    Image image = Image.FromFile(imagePath);
                    HeroImages.Add(image);
                }
                catch
                {
                    isCommonImageLost = true;
                    // 收集错误信息
                    errors.AppendLine($"图片缺失：{HeroDatas[i].HeroName}.png");
                    try
                    {
                        Image image = Image.FromFile(defaultImagePath);
                        HeroImages.Add(image);
                    }
                    catch
                    {
                        isDefautlImageLost = true;
                        // 添加替代图片避免崩溃
                        HeroImages.Add(new Bitmap(64, 64));
                    }

                }
            }

            // 如果有任何错误，弹出综合消息
            if (errors.Length > 0)
            {
                if (isCommonImageLost)
                {
                    head += $"缺失图片所在路径：{Path.Combine(paths[pathIndex], "images")}\n";
                }
                if (isDefautlImageLost)
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
        /// 从英雄数据对象列表加载到职业对象列表
        /// </summary>
        private void LoadProfessions()
        {
            professions.Clear();
            for (int i = 0; i < HeroDatas.Count; i++)
            {
                string[] result = HeroDatas[i].Profession.Split('|', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < result.Length; j++)
                {
                    Profession existingGroup = professions.FirstOrDefault(p => string.Equals(p.Title, result[j], StringComparison.OrdinalIgnoreCase));
                    if (existingGroup != null)
                    {
                        existingGroup.HeroNames.Add(HeroDatas[i].HeroName);
                    }
                    else
                    {
                        var newObject = new Profession
                        {
                            Title = result[j],
                            HeroNames = new List<string>()
                        };
                        newObject.HeroNames.Add(HeroDatas[i].HeroName);
                        professions.Add(newObject);
                    }
                }
            }
        }

        /// <summary>
        /// 从英雄数据对象列表加载到特质对象列表
        /// </summary>
        private void LoadPeculiarity()
        {
            peculiarities.Clear();
            for (int i = 0; i < HeroDatas.Count; i++)
            {
                string[] result = HeroDatas[i].Peculiarity.Split('|', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < result.Length; j++)
                {
                    var existingGroup = peculiarities.FirstOrDefault(p => string.Equals(p.Title, result[j], StringComparison.OrdinalIgnoreCase));
                    if (existingGroup != null)
                    {
                        existingGroup.HeroNames.Add(HeroDatas[i].HeroName);
                    }
                    else
                    {
                        var newObject = new Peculiarity
                        {
                            Title = result[j],
                            HeroNames = new List<string>()
                        };
                        newObject.HeroNames.Add(HeroDatas[i].HeroName);
                        peculiarities.Add(newObject);
                    }
                }
            }
        }

        /// <summary>
        /// 建立字典联系
        /// </summary>
        private void BuildMap()
        {
            for (int i = 0; i < HeroImages.Count; i++)
            {
                imageToHeroDataMap[HeroImages[i]] = HeroDatas[i];
            }
            for (int i = 0; i < HeroDatas.Count; i++)
            {
                heroDataToImageMap[HeroDatas[i]] = HeroImages[i];
            }
            foreach (HeroData hero in HeroDatas)
            {
                nameToHeroDataMap[hero.HeroName] = hero;
            }
        }

        /// <summary>
        /// 从英雄列表读取字库。
        /// </summary>
        private void LoadCharLib()
        {
            _charDictionary.Clear();
            if (HeroDatas.Count > 0)
            {
                foreach (HeroData hero in HeroDatas)
                {
                    if (!string.IsNullOrEmpty(hero.HeroName))
                    {
                        foreach (char c in hero.HeroName)
                        {
                            _charDictionary.Add(c);
                        }
                    }
                }
            }
        }
        #endregion


    }
}
