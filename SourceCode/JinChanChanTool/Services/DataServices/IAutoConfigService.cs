using JinChanChanTool.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.DataServices
{
    public interface IAutoConfigService
    {
        /// <summary>
        /// 当前的应用设置实例。
        /// </summary>
        AutoConfig CurrentConfig { get; set; }

        /// <summary>
        /// 从应用设置文件读取到对象。
        /// </summary>
        void Load();

        /// <summary>
        /// 保存当前的对象设置到本地。
        /// </summary>
        bool Save();

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
