using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.DataServices
{
    public class EquipmentService: IEquipmentService
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
        /// 装备数据对象列表
        /// </summary>
        public List<Equipment> Equipments;

        /// <summary>
        /// 装备名称到对象的字典
        /// </summary>
        private Dictionary<string, Equipment> nameToEquipmentMap;

        #region 初始化
        public EquipmentService()
        {
            InitializePaths();
            pathIndex = 0;
            Equipments = new List<Equipment>();
            nameToEquipmentMap = new Dictionary<string, Equipment>();
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
            BuildMap();           
        }

        /// <summary>
        /// 从对象保存到本地
        /// </summary>
        public void Save()
        {
            if (paths.Length > 0 && pathIndex < paths.Length)
            {
                string filePath = Path.Combine(paths[pathIndex], "Equipment.json");
                try
                {
                    
                    // 设置 JsonSerializerOptions 以保持中文字符的可读性
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true, // 格式化输出
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 保留中文字符
                    };
                    // 序列化数据
                    string json = JsonSerializer.Serialize(Equipments, options);
                    File.WriteAllText(filePath, json);
                }
                catch
                {
                    MessageBox.Show($"装备配置文件\"{Path.GetFileName(filePath)}\"保存失败\n路径：\n{filePath}",
                                  "文件保存失败",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error
                                  );
                }
            }
            else
            {
                MessageBox.Show($"装备配置文件夹不存在",
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
            Equipments.Clear();
            nameToEquipmentMap.Clear();
            Load();
        }

        /// <summary>
        /// 从装备名获取装备对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Equipment GetEquipmentFromName(string name)
        {
            if (nameToEquipmentMap.TryGetValue(name, out Equipment equipment))
            {
                return equipment;
            }
            else
            {
                return null;
            }
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
                for (int i = 0; i < paths.Length; i++)
                {
                    if (Path.GetFileName(paths[i]).Equals(season, StringComparison.OrdinalIgnoreCase))
                    {

                        selectedIndex = i;
                        isFound = true;                       
                        break;
                    }
                }
            }
            if (paths.Length > 0)
            {
                pathIndex = Math.Min(selectedIndex, paths.Length - 1);
            }
            return isFound;
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
        /// 获取装备数据对象列表
        /// </summary>
        /// <returns></returns>
        public List<Equipment> GetEquipmentDatas()
        {
            return Equipments;
        }

       
        #endregion

        #region 私有方法
        /// <summary>
        /// 从本地文件加载到对象列表，若失败则创建空的文件覆盖本地文件。
        /// </summary>
        private void LoadFromJson()
        {           
            Equipments.Clear();
            if (paths.Length > 0 && pathIndex < paths.Length)
            {
                string filePath = Path.Combine(paths[pathIndex], "Equipment.json");
                try
                {
                    
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"找不到装备配置文件\"{Path.GetFileName(filePath)}\"\n路径：\n{filePath}\n将创建新的文件。",
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
                        MessageBox.Show($"装备配置文件\"{Path.GetFileName(filePath)}\"内容为空。\n路径：\n{filePath}\n将创建新的文件。",
                                   "文件为空",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
                        Save();
                        return;
                    }
                    List<Equipment> temp = JsonSerializer.Deserialize<List<Equipment>>(json);
                    //合并同名英雄
                    var groupedHeroes = temp
                        .GroupBy(h => h.Name)
                        .Select(g =>
                        {
                            // 如果只有一个元素，直接返回
                            if (g.Count() == 1)
                                return g.First();

                            // 合并多个元素
                            var first = g.First();
                            var merged = new Equipment
                            {
                                Name = first.Name,
                                EquipmentType = first.EquipmentType,
                                SyntheticPathway = first.SyntheticPathway
                            };
                            return merged;
                        })
                    .ToList();
                    Equipments = groupedHeroes;                   
                }
                catch
                {
                    MessageBox.Show($"装备配置文件\"{Path.GetFileName(filePath)}\"格式错误\n路径：\n{filePath}\n将创建新的文件。",
                                   "文件格式错误",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
                    Save();
                }
            }
            else
            {
                MessageBox.Show($"装备配置文件夹不存在",
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
            StringBuilder errors = new StringBuilder();
            string head = "";
            bool isCommonImageLost = false;
           
            for (int i = 0; i < Equipments.Count; i++)
            {
                try
                {

                    string imagePath = Path.Combine(paths[pathIndex], "EquipmentImages", $"{Equipments[i].Name}.png");
                    Image image = Image.FromFile(imagePath);                    
                    Equipments[i].Image = new Bitmap(image);                    
                }
                catch
                {
                    isCommonImageLost = true;
                    // 收集错误信息
                    errors.AppendLine($"图片缺失：{Equipments[i].Name}.png");
                    try
                    {
                        Image image = new Bitmap(64, 64);                        
                        Equipments[i].Image = new Bitmap(image);
                    }
                    catch
                    {
                                              
                        Equipments[i].Image = new Bitmap(64, 64);
                    }

                }
            }

            // 如果有任何错误，弹出综合消息
            if (errors.Length > 0)
            {
                if (isCommonImageLost)
                {
                    head += $"缺失图片所在路径：{Path.Combine(paths[pathIndex], "EquipmentImages")}\n";
                }
               
                MessageBox.Show($"{head}{errors}",
                                "图片加载错误",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

       
        /// <summary>
        /// 建立字典联系
        /// </summary>
        private void BuildMap()
        {           
            foreach (Equipment equipment in Equipments)
            {
                nameToEquipmentMap[equipment.Name] = equipment;
            }
        }
              
        #endregion
    }
}
