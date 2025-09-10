using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinChanChanTool.Services.DataServices
{
    /// <summary>
    /// 负责从本地JSON文件中读取和提供网络爬取前所必需的映射数据。
    /// 这是一个独立的、自包含的服务，专门处理配置文件的加载。
    /// </summary>
    public class ApiRequestPayloadDataService : IApiRequestPayloadDataService
    {
        // 接口中定义的公共属性
        public Dictionary<string, string> EquipmentApiNameMap { get; private set; }
        public Dictionary<string, string> HeroIdToKeyMap { get; private set; }
        public Dictionary<string, string> HeroKeyToNameMap { get; private set; }

        /// <summary>
        /// 构造函数，初始化所有字典以避免空引用异常。
        /// </summary>
        public ApiRequestPayloadDataService()
        {
            EquipmentApiNameMap = new Dictionary<string, string>();
            HeroIdToKeyMap = new Dictionary<string, string>();
            HeroKeyToNameMap = new Dictionary<string, string>();
        }

        /// <summary>
        /// (公共入口) 异步加载所有必需的映射文件到内存中。
        /// </summary>
        public async Task LoadAllAsync()
        {
            // 按顺序调用所有私有的加载方法。
            await LoadEquipmentApiNameMapAsync();
            await LoadHeroIdToKeyMapAsync();
            await LoadHeroKeyToNameMapAsync();
        }

        #region Private Loading Methods (待实现)

        /// <summary>
        /// 加载装备API Key到中文名的映射文件
        /// </summary>
        private async Task LoadEquipmentApiNameMapAsync()
        {
            // 获取程序的基目录
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // 构建文件的相对路径
            string jsonPath = Path.Combine(basePath, "Resources", "CrawlerFiles", "EquipmentNameMapping.json");

            try
            {
                if (!File.Exists(jsonPath))
                {
                    MessageBox.Show($"必需的映射文件不存在: {jsonPath}", "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // 加载失败时，保持为空字典而不是null
                    EquipmentApiNameMap = new Dictionary<string, string>();
                    return;
                }

                string jsonString = await File.ReadAllTextAsync(jsonPath, Encoding.UTF8);

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    // 使用 ?? 运算符，如果反序列化结果为null，则赋予一个空字典
                    EquipmentApiNameMap = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString) ?? new Dictionary<string, string>();
                    System.Diagnostics.Debug.WriteLine($"成功加载 {EquipmentApiNameMap.Count} 条装备API名称映射。");
                }
                else
                {
                    EquipmentApiNameMap = new Dictionary<string, string>();
                    MessageBox.Show("装备API名称映射文件为空。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                EquipmentApiNameMap = new Dictionary<string, string>(); // 发生任何错误都保证属性是一个有效的空字典
                MessageBox.Show($"加载装备API名称映射文件时发生错误: {ex.Message}", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///加载英雄ID到API Key的映射文件
        /// </summary>
        private async Task LoadHeroIdToKeyMapAsync()
        {
            // 获取程序的基目录
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // 构建 HeroNameMapping.json 的相对路径
            string jsonPath = Path.Combine(basePath, "Resources", "CrawlerFiles", "HeroNameMapping.json");

            try
            {
                if (!File.Exists(jsonPath))
                {
                    MessageBox.Show($"必需的英雄映射文件不存在: {jsonPath}", "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    HeroIdToKeyMap = new Dictionary<string, string>();
                    return;
                }

                string jsonString = await File.ReadAllTextAsync(jsonPath, Encoding.UTF8);

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    HeroIdToKeyMap = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString) ?? new Dictionary<string, string>();
                    System.Diagnostics.Debug.WriteLine($"成功加载 {HeroIdToKeyMap.Count} 条英雄ID到Key的映射。");
                }
                else
                {
                    HeroIdToKeyMap = new Dictionary<string, string>();
                    MessageBox.Show("英雄ID到Key的映射文件为空。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                HeroIdToKeyMap = new Dictionary<string, string>();
                MessageBox.Show($"加载英雄ID到Key的映射文件时发生错误: {ex.Message}", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 加载英雄API Key到中文名的映射文件
        /// </summary>
        private async Task LoadHeroKeyToNameMapAsync()
        {
            // 获取程序的基目录
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // 构建 HeroKeyToNameMapping.json 的相对路径
            string jsonPath = Path.Combine(basePath, "Resources", "CrawlerFiles", "HeroKeyToNameMapping.json");

            try
            {
                if (!File.Exists(jsonPath))
                {
                    MessageBox.Show($"必需的英雄Key->名称映射文件不存在: {jsonPath}", "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    HeroKeyToNameMap = new Dictionary<string, string>();
                    return;
                }

                string jsonString = await File.ReadAllTextAsync(jsonPath, Encoding.UTF8);

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    HeroKeyToNameMap = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString) ?? new Dictionary<string, string>();
                    System.Diagnostics.Debug.WriteLine($"成功加载 {HeroKeyToNameMap.Count} 条英雄Key->名称映射。");
                }
                else
                {
                    HeroKeyToNameMap = new Dictionary<string, string>();
                    MessageBox.Show("英雄Key->名称映射文件为空。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                HeroKeyToNameMap = new Dictionary<string, string>();
                MessageBox.Show($"加载英雄Key->名称映射文件时发生错误: {ex.Message}", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}