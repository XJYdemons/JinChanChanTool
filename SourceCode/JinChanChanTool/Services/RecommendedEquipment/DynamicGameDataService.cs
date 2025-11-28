using JinChanChanTool.Forms;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.RecommendedEquipment
{
    /// <summary>
    /// 实现了 IDynamicGameDataService 接口。
    /// 这个服务通过网络API获取动态的游戏数据，如翻译文件和英雄列表，
    /// 并将处理后的结果缓存在内存中，供应用程序的其他部分使用。
    /// </summary>
    public class DynamicGameDataService : IDynamicGameDataService
    {
        // 定义数据源的URL常量，便于管理和修改
        private const string TranslationsUrl = "https://data.metatft.com/lookups/TFTSet15_latest_zh_cn.json";
        private const string UnitListUrl = "https://api-hc.metatft.com/tft-comps-api/unit_items_processed";

        // 遵循最佳实践，在整个应用程序生命周期内共享一个HttpClient实例，以提高性能和避免套接字耗尽问题。
        private static readonly HttpClient _httpClient = new HttpClient();

        // 状态标记，防止重复执行昂贵的初始化网络请求。
        private bool _isInitialized = false;

        #region IDynamicGameDataService 实现

        public Dictionary<string, string> HeroTranslations { get; private set; }
        public Dictionary<string, string> ItemTranslations { get; private set; }
        public List<string> CurrentSeasonHeroKeys { get; private set; }

        #endregion

        /// <summary>
        /// 构造函数，初始化所有集合属性，确保它们在使用前不为null。
        /// </summary>
        public DynamicGameDataService()
        {
            HeroTranslations = new Dictionary<string, string>();
            ItemTranslations = new Dictionary<string, string>();
            CurrentSeasonHeroKeys = new List<string>();
        }

        /// <summary>
        /// 异步初始化服务，从网络加载所有必需的数据。
        /// 这个方法是幂等的，即多次调用也只会执行一次实际的数据加载。
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized)
            {
                return; // 如果已经初始化，则直接返回，避免重复工作。
            }

            try
            {                
                Debug.WriteLine("DynamicGameDataService: 开始初始化，准备从网络获取数据...");
                LogTool.Log("DynamicGameDataService: 开始初始化，准备从网络获取数据...");
                OutputForm.Instance.WriteLineOutputMessage("DynamicGameDataService: 开始初始化，准备从网络获取数据...");
                // 使用 Task.WhenAll 并行发起两个网络请求，可以显著缩短总等待时间。
                var translationTask = _httpClient.GetStringAsync(TranslationsUrl);
                var unitListTask = _httpClient.GetStringAsync(UnitListUrl);

                await Task.WhenAll(translationTask, unitListTask);

                // 从完成的任务中获取JSON字符串结果
                string translationJson = await translationTask;
                string unitListJson = await unitListTask;

                // 按顺序处理数据，先处理英雄列表以确定当前赛季
                ProcessUnitListData(unitListJson);
                ProcessTranslationData(translationJson);

                _isInitialized = true; // 标记初始化成功
                Debug.WriteLine("DynamicGameDataService: 初始化成功！");
                LogTool.Log("DynamicGameDataService: 初始化成功！");
                OutputForm.Instance.WriteLineOutputMessage("DynamicGameDataService: 初始化成功！");
            }
            catch (Exception ex)
            {
                // 如果在初始化过程中发生任何错误（如网络问题、JSON解析失败），记录错误并重新抛出。
                // 让上层调用者（如应用程序启动逻辑）知道初始化失败，并据此决定如何响应（例如，向用户显示错误消息）。
                Debug.WriteLine($"DynamicGameDataService: 初始化失败! 错误: {ex.Message}");
                LogTool.Log($"DynamicGameDataService: 初始化失败! 错误: {ex.Message}");
                OutputForm.Instance.WriteLineOutputMessage($"DynamicGameDataService: 初始化失败! 错误: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 解析从 unit_items_processed API 获取的JSON数据，提取当前赛季的英雄列表。
        /// </summary>
        private void ProcessUnitListData(string json)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var unitListResponse = JsonSerializer.Deserialize<UnitListResponse>(json, options);

            if (unitListResponse == null || string.IsNullOrEmpty(unitListResponse.TftSet) || unitListResponse.Units == null)
            {
                throw new InvalidOperationException("未能正确解析英雄列表数据或数据格式无效。");
            }

            // 从 "TFTSet15" 中推断出赛季前缀 "TFT15"
            string seasonPrefix = unitListResponse.TftSet.Replace("Set", "");

            // 筛选出所有以当前赛季前缀开头的英雄API Key
            CurrentSeasonHeroKeys = unitListResponse.Units.Keys
                .Where(key => key.StartsWith(seasonPrefix, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            Debug.WriteLine($"已确定当前赛季: {seasonPrefix}，找到 {CurrentSeasonHeroKeys.Count} 位英雄。");
            LogTool.Log($"已确定当前赛季: {seasonPrefix}，找到 {CurrentSeasonHeroKeys.Count} 位英雄。");
            OutputForm.Instance.WriteLineOutputMessage($"已确定当前赛季: {seasonPrefix}，找到 {CurrentSeasonHeroKeys.Count} 位英雄。");
        }

        /// <summary>
        /// 解析翻译JSON数据，填充英雄和装备的翻译字典。
        /// </summary>
        private void ProcessTranslationData(string json)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var translationData = JsonSerializer.Deserialize<TranslationData>(json, options);

            if (translationData == null || translationData.Units == null || translationData.Items == null)
            {
                throw new InvalidOperationException("未能正确解析翻译数据或数据格式无效。");
            }

            // 增加对重复Key的处理 
            // 直接 .ToDictionary()
            // 先按 ApiName 分组 (GroupBy)，然后从每个分组中取第一个元素 (.First()) 来创建字典。
            // 这样即使源文件有重复的ApiName，也只会取第一个，从而避免了“Key已存在”的错误。
            HeroTranslations = translationData.Units
                .Where(unit => !string.IsNullOrEmpty(unit.ApiName) && !string.IsNullOrEmpty(unit.Name))
                .GroupBy(unit => unit.ApiName) // 按ApiName分组
                .ToDictionary(g => g.Key, g => g.First().Name); // 使用分组的Key和该分组的第一个元素的Name来创建字典

            ItemTranslations = translationData.Items
                .Where(item => !string.IsNullOrEmpty(item.ApiName) && !string.IsNullOrEmpty(item.Name))
                .GroupBy(item => item.ApiName) // 对装备列表也进行同样的分组去重
                .ToDictionary(g => g.Key, g => g.First().Name);
            
            Debug.WriteLine($"已加载 {HeroTranslations.Count} 条英雄翻译和 {ItemTranslations.Count} 条装备翻译。");
            LogTool.Log($"已加载 {HeroTranslations.Count} 条英雄翻译和 {ItemTranslations.Count} 条装备翻译。");
            OutputForm.Instance.WriteLineOutputMessage($"已加载 {HeroTranslations.Count} 条英雄翻译和 {ItemTranslations.Count} 条装备翻译。");
        }

        #region 内部数据模型

        /// <summary>
        /// 用于反序列化 unit_items_processed API 响应的内部模型。
        /// </summary>
        private class UnitListResponse
        {
            [JsonPropertyName("tft_set")]
            public string TftSet { get; set; }

            [JsonPropertyName("units")]
            public Dictionary<string, object> Units { get; set; }
        }

        #endregion
    }
}