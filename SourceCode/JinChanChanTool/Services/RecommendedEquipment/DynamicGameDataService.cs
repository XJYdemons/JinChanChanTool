using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.Network;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JinChanChanTool.Services.RecommendedEquipment
{
    /// <summary>
    /// 实现了 IDynamicGameDataService 接口。
    /// 改为使用全局 HttpProvider 管理的 HttpClient。
    /// </summary>
    public class DynamicGameDataService
    {
        private const string TranslationsUrl = "https://data.metatft.com/lookups/TFTSet16_pbe_zh_cn.json";
        private const string UnitListUrl = "https://api-hc.metatft.com/tft-comps-api/unit_items_processed";
        private const string GeneralTranslationsUrl = "https://data.metatft.com/locales/zh_cn.json";

        // 删除了本地 static readonly HttpClient _httpClient 实例

        private bool _isInitialized = false;

        #region IDynamicGameDataService 实现

        public Dictionary<string, string> HeroTranslations { get; private set; }
        public Dictionary<string, string> ItemTranslations { get; private set; }
        public Dictionary<string, string> TraitTranslations { get; private set; }
        public Dictionary<string, string> CommonTranslations { get; private set; }
        public List<string> CurrentSeasonHeroKeys { get; private set; }

        #endregion

        public DynamicGameDataService()
        {
            HeroTranslations = new Dictionary<string, string>();
            ItemTranslations = new Dictionary<string, string>();
            TraitTranslations = new Dictionary<string, string>();
            CommonTranslations = new Dictionary<string, string>();
            CurrentSeasonHeroKeys = new List<string>();
        }

        /// <summary>
        /// 异步初始化服务，从网络加载所有必需的数据。
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                Debug.WriteLine("DynamicGameDataService: 开始初始化...");
                LogTool.Log("DynamicGameDataService: 开始初始化...");
                OutputForm.Instance.WriteLineOutputMessage("DynamicGameDataService: 开始初始化...");

                var translationTask = HttpProvider.Client.GetAsync(TranslationsUrl, HttpCompletionOption.ResponseContentRead);
                var unitListTask = HttpProvider.Client.GetAsync(UnitListUrl, HttpCompletionOption.ResponseContentRead);
                var generalTask = HttpProvider.Client.GetAsync(GeneralTranslationsUrl, HttpCompletionOption.ResponseContentRead);

                await Task.WhenAll(translationTask, unitListTask, generalTask);

                // 获取结果后立即 Dispose 响应对象
                using var res1 = await translationTask;
                using var res2 = await unitListTask;
                using var res3 = await generalTask;

                res1.EnsureSuccessStatusCode();
                res2.EnsureSuccessStatusCode();
                res3.EnsureSuccessStatusCode();

                ProcessUnitListData(await res2.Content.ReadAsStringAsync());
                ProcessTranslationData(await res1.Content.ReadAsStringAsync());
                ProcessGeneralTranslationData(await res3.Content.ReadAsStringAsync());

                _isInitialized = true;
                Debug.WriteLine("DynamicGameDataService: 初始化成功！");
                LogTool.Log("DynamicGameDataService: 初始化成功！");
                OutputForm.Instance.WriteLineOutputMessage("DynamicGameDataService: 初始化成功！");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DynamicGameDataService: 初始化失败! 错误: {ex.Message}");
                LogTool.Log($"DynamicGameDataService: 初始化失败! 错误: {ex.Message}");
                OutputForm.Instance.WriteLineOutputMessage($"DynamicGameDataService: 初始化失败! 错误: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 解析通用翻译JSON，提取 common 节点下的标签翻译。
        /// </summary>
        private void ProcessGeneralTranslationData(string json)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<MetatftGeneralTranslation>(json, options);

            if (data == null || data.Common == null)
            {
                throw new InvalidOperationException("未能正确解析通用翻译数据(zh_cn.json)或数据格式无效。");
            }

            CommonTranslations = data.Common;

            Debug.WriteLine($"已加载 {CommonTranslations.Count} 条通用标签翻译。");
            LogTool.Log($"已加载 {CommonTranslations.Count} 条通用标签翻译。");
            OutputForm.Instance.WriteLineOutputMessage($"已加载 {CommonTranslations.Count} 条通用标签翻译。");
        }

        private void ProcessUnitListData(string json)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var unitListResponse = JsonSerializer.Deserialize<UnitListResponse>(json, options);

            if (unitListResponse == null || string.IsNullOrEmpty(unitListResponse.TftSet) || unitListResponse.Units == null)
            {
                throw new InvalidOperationException("未能正确解析英雄列表数据或数据格式无效。");
            }

            string seasonPrefix = unitListResponse.TftSet.Replace("Set", "");

            CurrentSeasonHeroKeys = unitListResponse.Units.Keys
                .Where(key => key.StartsWith(seasonPrefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Debug.WriteLine($"已确定当前赛季: {seasonPrefix}，找到 {CurrentSeasonHeroKeys.Count} 位英雄。");
            LogTool.Log($"已确定当前赛季: {seasonPrefix}，找到 {CurrentSeasonHeroKeys.Count} 位英雄。");
            OutputForm.Instance.WriteLineOutputMessage($"已确定当前赛季: {seasonPrefix}，找到 {CurrentSeasonHeroKeys.Count} 位英雄。");
        }

        private void ProcessTranslationData(string json)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var translationData = JsonSerializer.Deserialize<TranslationData>(json, options);

            if (translationData == null || translationData.Units == null ||translationData.Items == null || translationData.Traits == null)
            {
                throw new InvalidOperationException("未能正确解析翻译数据或数据格式无效。");
            }

            HeroTranslations = translationData.Units
                .Where(unit => !string.IsNullOrEmpty(unit.ApiName) && !string.IsNullOrEmpty(unit.Name))
                .GroupBy(unit => unit.ApiName)
                .ToDictionary(g => g.Key, g => g.First().Name);

            ItemTranslations = translationData.Items
                .Where(item => !string.IsNullOrEmpty(item.ApiName) && !string.IsNullOrEmpty(item.Name))
                .GroupBy(item => item.ApiName)
                .ToDictionary(g => g.Key, g => g.First().Name);

            TraitTranslations = translationData.Traits
                .Where(trait => !string.IsNullOrEmpty(trait.ApiName) && !string.IsNullOrEmpty(trait.Name))
                .GroupBy(trait => trait.ApiName)
                .ToDictionary(g => g.Key, g => g.First().Name);

            Debug.WriteLine($"已加载 {HeroTranslations.Count} 条英雄翻译、{ItemTranslations.Count} 条装备翻译和 {TraitTranslations.Count} 条羁绊翻译。");
            LogTool.Log($"已加载 {HeroTranslations.Count} 条英雄翻译、{ItemTranslations.Count} 条装备翻译和 {TraitTranslations.Count} 条羁绊翻译。");
            OutputForm.Instance.WriteLineOutputMessage($"已成功加载全量翻译数据（含 {TraitTranslations.Count} 条羁绊）。");
        }

        #region 内部数据模型

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