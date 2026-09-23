using System.Diagnostics;
using System.Text.Json;
using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Tools;

namespace JinChanChanTool.Services.DataServices
{   
    public class ManualSettingsService : IManualSettingsService
    {       
        /// <summary>
        /// 当前的应用设置实例。
        /// </summary>
        public ManualSettings CurrentConfig { get; set; }

        /// <summary>
        /// 设置变更事件，当设置保存后触发。
        /// </summary>
        public event EventHandler<ConfigChangedEventArgs> OnConfigSaved;

        /// <summary>
        /// 应用设置文件路径。
        /// </summary>
        private string filePath;

        /// <summary>
        /// JSON序列化选项（包含Color转换器）
        /// </summary>
        private readonly JsonSerializerOptions jsonOptions;

        #region 初始化
        public ManualSettingsService()
        {
            CurrentConfig = new ManualSettings();
            InitializePaths();

            // 初始化JSON序列化选项，添加Color转换器
            jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new ColorJsonConverter() }
            };
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
            filePath = Path.Combine(parentPath, "ManualSettings.json");            
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
                ManualSettings? oldConfig = null;
                if (File.Exists(filePath))
                {
                    try
                    {
                        string oldJson = File.ReadAllText(filePath);
                        if (!string.IsNullOrEmpty(oldJson))
                        {
                            oldConfig = JsonSerializer.Deserialize<ManualSettings>(oldJson, jsonOptions);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 配置文件损坏或格式不兼容：视为已变更，触发一次全量保存以覆盖坏文件
                        Debug.WriteLine($"[ManualSettingsService] 读取旧配置失败，将按已变更处理：{ex.Message}");
                        LogTool.Log($"[ManualSettingsService] 读取旧配置失败，将按已变更处理：{ex.Message}");
                    }
                }

                // 文件不存在或反序列化失败时 oldConfig 为 null，此时视为已变更
                if (oldConfig == null)
                {
                    return true;
                }

                return !oldConfig.Equals(CurrentConfig);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ManualSettingsService] 比较配置是否变更时发生异常：{ex.Message}");
                LogTool.Log($"[ManualSettingsService] 比较配置是否变更时发生异常：{ex.Message}");
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
                ManualSettings? oldConfig = null;
                if(File.Exists(filePath))
                {
                    try
                    {
                        string oldJson = File.ReadAllText(filePath);
                        if (!string.IsNullOrEmpty(oldJson))
                        {
                            oldConfig = JsonSerializer.Deserialize<ManualSettings>(oldJson, jsonOptions);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 旧文件损坏：仍继续写入新配置，差异计算按“全部字段变更”处理
                        Debug.WriteLine($"[ManualSettingsService] 读取旧配置失败，将按全部字段变更处理：{ex.Message}");
                        LogTool.Log($"[ManualSettingsService] 读取旧配置失败，将按全部字段变更处理：{ex.Message}");
                    }
                }
                // 设置 JsonSerializerOptions 以保持中文字符的可读性
                string json = JsonSerializer.Serialize(CurrentConfig, jsonOptions);
                File.WriteAllText(filePath, json);
                // 计算差异字段
                var changedFields = GetChangedFields(oldConfig, CurrentConfig);
                // 触发通知事件
                OnConfigSaved?.Invoke(this, new ConfigChangedEventArgs(changedFields,isManually));
                return true;
            }
            catch
            {
                MessageBox.Show($"用户应用设置文件\"{Path.GetFileName(filePath)}\"保存失败\n路径：\n{filePath}",
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
            CurrentConfig = new ManualSettings();
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
            CurrentConfig =new ManualSettings(); 
            try
            {
                //判断Json文件是否存在
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"找不到用户应用设置文件\"{Path.GetFileName(filePath)}\"\n路径：\n{filePath}\n将创建默认设置文件。",
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
                    MessageBox.Show($"用户应用设置文件\"{Path.GetFileName(filePath)}\"内容为空。\n路径：\n{filePath}\n将创建默认设置文件。",
                               "文件为空",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Warning
                               );
                    Save(false);
                    return;
                }
                // 反序列化可能返回 null（内容为字面量 "null" 等），此时保持现有配置并创建默认文件
                ManualSettings? loaded = JsonSerializer.Deserialize<ManualSettings>(json, jsonOptions);
                if (loaded == null)
                {
                    Debug.WriteLine("[ManualSettingsService] 用户应用设置反序列化结果为 null，将创建默认设置文件。");
                    LogTool.Log("[ManualSettingsService] 用户应用设置反序列化结果为 null，将创建默认设置文件。");
                    Save(false);
                    return;
                }

                CurrentConfig = loaded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ManualSettingsService] 用户应用设置文件读取失败：{ex.Message}");
                LogTool.Log($"[ManualSettingsService] 用户应用设置文件读取失败：{ex.Message}");
                MessageBox.Show($"用户应用设置文件\"{Path.GetFileName(filePath)}\"格式错误\n路径：\n{filePath}\n将创建默认设置文件。",
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
        private List<string> GetChangedFields(ManualSettings? oldConfig, ManualSettings newConfig)
        {
            var changed = new List<string>();

            if (oldConfig == null || newConfig == null)
                return changed;

            var properties = typeof(ManualSettings).GetProperties();

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

    /// <summary>
    /// 应用设置变更事件参数。
    /// </summary>
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
