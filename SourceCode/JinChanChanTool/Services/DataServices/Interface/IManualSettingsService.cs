using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices.Interface
{   
    public interface IManualSettingsService
    {
        /// <summary>
        /// 当前的应用设置实例。
        /// </summary>
        ManualSettings CurrentConfig { get; set; }

        /// <summary>
        /// 设置变更事件，当设置保存后触发。
        /// </summary>
        event EventHandler<ConfigChangedEventArgs> OnConfigSaved;

        /// <summary>
        /// 从应用设置文件读取到对象。
        /// </summary>
        void Load();

        /// <summary>
        /// 保存当前的对象设置到本地。
        /// </summary>
        bool Save(bool isManually);

        /// <summary>
        /// 设置默认的应用设置。
        /// </summary>
        void SetDefaultConfig();

        /// <summary>
        /// 重新加载配置到对象。
        /// </summary>
        void ReLoad();

        /// <summary>
        /// 内存中的设置相较于本地文件中的设置是否有改变。
        /// </summary>
        /// <returns></returns>
        bool IsChanged();
    }
    
}
