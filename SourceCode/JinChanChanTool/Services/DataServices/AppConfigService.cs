using System.Text.Json;
using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices
{   
    public class AppConfigService : IAppConfigService
    {       
        /// <summary>
        /// 当前的应用设置实例。
        /// </summary>
        public AppConfig CurrentConfig { get; set; }

        /// <summary>
        /// 设置变更事件，当设置保存后触发。
        /// </summary>
        public event EventHandler<ConfigChangedEventArgs> OnConfigSaved;

        /// <summary>
        /// 应用设置文件路径。
        /// </summary>
        private string filePath;

        #region 初始化
        public AppConfigService()
        {
            CurrentConfig = new AppConfig();
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
            filePath = Path.Combine(parentPath, "AppConfig.json");
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
        /// 内存中的设置相较于本地文件中的设置是否有改变。
        /// </summary>
        /// <returns></returns>
        public bool IsChanged()
        {
            try
            {
                // 读取旧配置副本（用于比较）
                AppConfig oldConfig = null;
                if (File.Exists(filePath))
                {
                    try
                    {
                        string oldJson = File.ReadAllText(filePath);
                        if (!string.IsNullOrEmpty(oldJson))
                        {
                            oldConfig = JsonSerializer.Deserialize<AppConfig>(oldJson);
                        }
                    }
                    catch
                    {

                    }
                }
               if(oldConfig.Equals(CurrentConfig))
                {
                    return false;
                    
                }
               else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保存当前的对象设置到本地。
        /// </summary>
        public bool Save(bool isManually)
        {
            try
            {
                // 读取旧配置副本（用于比较）
                AppConfig oldConfig = null;
                if(File.Exists(filePath))
                {
                    try
                    {
                        string oldJson = File.ReadAllText(filePath);
                        if (!string.IsNullOrEmpty(oldJson))
                        {
                            oldConfig = JsonSerializer.Deserialize<AppConfig>(oldJson);
                        }
                    }
                    catch
                    {

                    }
                }
                // 设置 JsonSerializerOptions 以保持中文字符的可读性
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string json = JsonSerializer.Serialize(CurrentConfig, options);
                File.WriteAllText(filePath, json);
                // 计算差异字段
                var changedFields = GetChangedFields(oldConfig, CurrentConfig);
                // 触发通知事件
                OnConfigSaved?.Invoke(this, new ConfigChangedEventArgs(changedFields,isManually));
                return true;
            }
            catch
            {
                MessageBox.Show($"用户应用设置文件\"AppConfig.json\"保存失败\n路径：\n{filePath}",
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
            CurrentConfig = new AppConfig();
        }

        /// <summary>
        /// 重新从从应用设置文件读取到对象。
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
            CurrentConfig =new AppConfig(); 
            try
            {
                //判断Json文件是否存在
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"找不到用户应用设置文件\"AppConfig.json\"\n路径：\n{filePath}\n将创建默认设置文件。",
                                    "文件不存在",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                    );
                    Save(false);
                    return;
                }
                string json = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(json))
                {
                    MessageBox.Show($"用户应用设置文件\"AppConfig.json\"内容为空。\n路径：\n{filePath}\n将创建默认设置文件。",
                               "文件为空",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Warning
                               );
                    Save(false);
                    return;
                }
                CurrentConfig = JsonSerializer.Deserialize<AppConfig>(json);                
            }
            catch
            {
                MessageBox.Show($"用户应用设置文件\"AppConfig.json\"格式错误\n路径：\n{filePath}\n将创建默认设置文件。",
                                   "文件格式错误",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning
                                   );
                Save(false);
            }
        }

        /// <summary>
        /// 比较两个 AppConfig，返回所有值不同的属性名。
        /// </summary>
        private List<string> GetChangedFields(AppConfig oldConfig, AppConfig newConfig)
        {
            var changed = new List<string>();

            if (oldConfig == null || newConfig == null)
                return changed;

            var properties = typeof(AppConfig).GetProperties();

            foreach (var prop in properties)
            {
                var oldValue = prop.GetValue(oldConfig);
                var newValue = prop.GetValue(newConfig);

                if (oldValue == null && newValue == null)
                    continue;

                if ((oldValue == null && newValue != null) ||
                    (oldValue != null && !oldValue.Equals(newValue)))
                {
                    changed.Add(prop.Name);
                }
            }

            return changed;
        }
        #endregion



    }
    public class ConfigChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 被修改的字段名列表。
        /// </summary>
        public List<string> ChangedFields { get; }

        /// <summary>
        /// 是否是手动触发的变更（true：手动保存，false：自动保存）
        /// </summary>
        public bool IsManualChange { get; }

        public ConfigChangedEventArgs(List<string> changedFields, bool isManualChange)
        {
            ChangedFields = changedFields;
            IsManualChange = isManualChange;
        }
    }
}
