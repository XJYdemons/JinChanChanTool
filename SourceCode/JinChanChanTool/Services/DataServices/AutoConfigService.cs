using JinChanChanTool.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.DataServices
{
    public class AutoConfigService:IAutoConfigService
    {
        /// <summary>
        /// 当前的应用设置实例。
        /// </summary>
        public AutoConfig CurrentConfig { get; set; }
     
        /// <summary>
        /// 应用设置文件路径。
        /// </summary>
        private string filePath;

        #region 初始化
        public AutoConfigService()
        {
            CurrentConfig = new AutoConfig();
            InitializePaths();
        }

        /// <summary>
        /// 初始化本地文件路径。
        /// </summary>
        private void InitializePaths()
        {
            string parentPath = Path.Combine(Application.StartupPath, "Resources");
            // 确保目录存在
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }
            filePath = Path.Combine(parentPath, "AutoConfig.json");
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 从应用设置文件读取到对象。
        /// </summary>
        public void Load()
        {
            LoadFromFile();
        }

        /// <summary>
        /// 保存当前的对象设置到本地。
        /// </summary>
        public bool Save()
        {
            try
            {                
                // 设置 JsonSerializerOptions 以保持中文字符的可读性
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string json = JsonSerializer.Serialize(CurrentConfig, options);
                File.WriteAllText(filePath, json);               
                return true;
            }
            catch
            {
                MessageBox.Show($"自动应用配置文件\"AutoConfig.json\"保存失败\n路径：\n{filePath}",
                                  "文件保存失败",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error
                                  );
                return false;
            }

        }

        /// <summary>
        ///读取默认的应用设置。
        /// </summary>
        public void SetDefaultConfig()
        {
            CurrentConfig = new AutoConfig();
        }

        /// <summary>
        /// 重新从应用设置文件读取到对象。
        /// </summary>
        public void ReLoad()
        {
            CurrentConfig = null;
            Load();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 从应用设置文件读取到对象，若失败则读取默认设置并保存到本地。
        /// </summary>
        private void LoadFromFile()
        {
            CurrentConfig = new AutoConfig();
            try
            {
                //判断Json文件是否存在
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"找不到自动应用配置文件\"AutoConfig.json\"\n路径：\n{filePath}\n将创建默认配置文件。",
                                    "文件不存在",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                    );
                    Save();
                    return;
                }
                string json = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(json))
                {
                    MessageBox.Show($"自动应用配置文件\"AutoConfig.json\"内容为空。\n路径：\n{filePath}\n将创建默认配置文件。",
                               "文件为空",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Warning
                               );
                    Save();
                    return;
                }
                CurrentConfig = JsonSerializer.Deserialize<AutoConfig>(json);
            }
            catch
            {
                MessageBox.Show($"自动应用配置文件\"AutoConfig.json\"格式错误\n路径：\n{filePath}\n将创建默认配置文件。",
                                   "文件格式错误",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning
                                   );
                Save();
            }
        }       
        #endregion
    }
}
