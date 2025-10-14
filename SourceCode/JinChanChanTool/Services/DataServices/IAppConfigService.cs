using JinChanChanTool.DataClass;

namespace JinChanChanTool.Services.DataServices
{   
    public interface IAppConfigService
    {
        /// <summary>
        /// 当前的应用设置实例。
        /// </summary>
        AppConfig CurrentConfig { get; set; }

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
        void Save();

        /// <summary>
        /// 设置默认的应用设置。
        /// </summary>
        void SetDefaultConfig();

        /// <summary>
        /// 重新加载配置到对象。
        /// </summary>
        void ReLoad();
    }
    
}
