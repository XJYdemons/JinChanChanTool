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
        //private const string TranslationsUrl = "https://data.metatft.com/lookups/TFTSet17_latest_zh_cn.json";
        //private const string UnitListUrl = "https://api-hc.metatft.com/tft-comps-api/unit_items_processed";
        //private const string GeneralTranslationsUrl = "https://data.metatft.com/locales/zh_cn.json";
        // Cloudflare Worker 加速地址
        private const string ProxyHost = "https://api.xiaoyumetatft.xyz";

        // TranslationsUrl now dynamically built from UnitList API tft_set response
        private const string UnitListUrl = ProxyHost + "/tft-comps-api/unit_items_processed";
        private const string GeneralTranslationsUrl = ProxyHost + "/locales/zh_cn.json";

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
                OutputForm.Instance.WriteLineOutputMessage("DynamicGameDataService: 开始初始化...");

                // Phase 1: fetch unit list + general translations in parallel
                var unitListTask = HttpProvider.Client.GetAsync(UnitListUrl, HttpCompletionOption.ResponseContentRead);
                var generalTask = HttpProvider.Client.GetAsync(GeneralTranslationsUrl, HttpCompletionOption.ResponseContentRead);

                await Task.WhenAll(unitListTask, generalTask);

                using var unitRes = await unitListTask;
                using var generalRes = await generalTask;

                unitRes.EnsureSuccessStatusCode();
                generalRes.EnsureSuccessStatusCode();

                string unitListJson = await unitRes.Content.ReadAsStringAsync();
                string generalJson = await generalRes.Content.ReadAsStringAsync();

                // Extract season from unit list to build dynamic TranslationsUrl
                using var doc = JsonDocument.Parse(unitListJson);
                string tftSet = doc.RootElement.GetProperty("tft_set").GetString();
                string seasonNum = tftSet.Replace("Set", "").Replace("TFT", "");
                string translationsUrl = $"{ProxyHost}/lookups/TFTSet{seasonNum}_latest_zh_cn.json";


                // Phase 2: fetch translations with the dynamic URL
                using var transRes = await HttpProvider.Client.GetAsync(translationsUrl, HttpCompletionOption.ResponseContentRead);
                transRes.EnsureSuccessStatusCode();

                string translationJson = await transRes.Content.ReadAsStringAsync();

                // Process all data
                ProcessUnitListData(unitListJson);
                ProcessTranslationData(translationJson);
                ProcessGeneralTranslationData(generalJson);

                _isInitialized = true;
                OutputForm.Instance.WriteLineOutputMessage("DynamicGameDataService: 初始化成功！");
            }
            catch (Exception ex)
            {
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