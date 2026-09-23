using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.Network;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using System.Diagnostics;
using System.Linq;
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
        private const string SeasonSourceUrl = ProxyHost + "/tft-comps-api/comps_data?queue=1100";
        private const string GeneralTranslationsUrl = ProxyHost + "/locales/zh_cn.json";

        // 删除了本地 static readonly HttpClient _httpClient 实例

        private bool _isInitialized = false;

        #region IDynamicGameDataService 实现

        public Dictionary<string, string> HeroTranslations { get; private set; }
        public Dictionary<string, string> ItemTranslations { get; private set; }
        public Dictionary<string, string> TraitTranslations { get; private set; }
        public Dictionary<string, string> CommonTranslations { get; private set; }
        public List<string> CurrentSeasonHeroKeys { get; private set; }
        public Dictionary<string, string> LineUpCodeToName { get; private set; }

        private Dictionary<string, string> _heroTranslationAliases;
        private List<string> _seasonHeroKeysFromTranslations;

        #endregion

        public DynamicGameDataService()
        {
            HeroTranslations = new Dictionary<string, string>();
            ItemTranslations = new Dictionary<string, string>();
            TraitTranslations = new Dictionary<string, string>();
            CommonTranslations = new Dictionary<string, string>();
            CurrentSeasonHeroKeys = new List<string>();
            LineUpCodeToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _heroTranslationAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _seasonHeroKeysFromTranslations = new List<string>();
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

                // Phase 1: fetch the season source, unit list and general translations in parallel.
                var unitListTask = HttpProvider.Client.GetAsync(UnitListUrl, HttpCompletionOption.ResponseContentRead);
                var seasonSourceTask = HttpProvider.Client.GetAsync(SeasonSourceUrl, HttpCompletionOption.ResponseContentRead);
                var generalTask = HttpProvider.Client.GetAsync(GeneralTranslationsUrl, HttpCompletionOption.ResponseContentRead);

                await Task.WhenAll(unitListTask, seasonSourceTask, generalTask);

                using var unitRes = await unitListTask;
                using var seasonSourceRes = await seasonSourceTask;
                using var generalRes = await generalTask;

                unitRes.EnsureSuccessStatusCode();
                seasonSourceRes.EnsureSuccessStatusCode();
                generalRes.EnsureSuccessStatusCode();

                string unitListJson = await unitRes.Content.ReadAsStringAsync();
                string seasonSourceJson = await seasonSourceRes.Content.ReadAsStringAsync();
                string generalJson = await generalRes.Content.ReadAsStringAsync();

                // comps_data is the authoritative source for the season used by lineup recommendations.
                using var seasonDoc = JsonDocument.Parse(seasonSourceJson);
                // GetString 可能返回 null，null 时与下方赛季编号校验保持一致：记录日志后抛出，由外层 catch 统一处理
                string? tftSet = seasonDoc.RootElement.GetProperty("tft_set").GetString();
                if (tftSet == null)
                {
                    LogTool.Log("[DynamicGameDataService] InitializeAsync 赛季数据源中的 tft_set 字段为 null。");
                    Debug.WriteLine("[DynamicGameDataService] InitializeAsync 赛季数据源中的 tft_set 字段为 null。");
                    throw new InvalidOperationException("赛季数据源缺少有效的 tft_set 字段。");
                }
                string seasonNum = new string(tftSet.Where(char.IsDigit).ToArray());
                if (string.IsNullOrWhiteSpace(seasonNum))
                {
                    throw new InvalidOperationException($"赛季标识“{tftSet}”不包含有效的赛季编号。");
                }
                string translationsUrl = $"{ProxyHost}/lookups/TFTSet{seasonNum}_latest_zh_cn.json";


                // Phase 2: fetch translations with the dynamic URL
                using var transRes = await HttpProvider.Client.GetAsync(translationsUrl, HttpCompletionOption.ResponseContentRead);
                transRes.EnsureSuccessStatusCode();

                string translationJson = await transRes.Content.ReadAsStringAsync();

                // Process all data
                ProcessTranslationData(translationJson);
                ProcessCurrentSeasonHeroKeys(seasonSourceJson, unitListJson);
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
        /// 获取指定赛季的阵容码字典数据。
        /// 保持与推荐数据相同的代理地址和 HttpProvider 请求链路。
        /// </summary>
        public async Task<IReadOnlyDictionary<string, string>> GetLineUpCodeToNameAsync(string season)
        {
            string seasonNumber = new string((season ?? string.Empty).Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(seasonNumber))
            {
                throw new ArgumentException($"赛季标识“{season}”格式无效。", nameof(season));
            }

            string translationsUrl = $"{ProxyHost}/lookups/TFTSet{seasonNumber}_latest_zh_cn.json";
            using var response = await HttpProvider.Client.GetAsync(translationsUrl, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return ExtractLineUpCodeToName(json);
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

        private void ProcessCurrentSeasonHeroKeys(string seasonJson, string unitListJson)
        {
            if (_seasonHeroKeysFromTranslations.Count > 0)
            {
                CurrentSeasonHeroKeys = _seasonHeroKeysFromTranslations;
                OutputForm.Instance.WriteLineOutputMessage($"已从赛季翻译数据确定当前赛季英雄，找到 {CurrentSeasonHeroKeys.Count} 位英雄。");
                return;
            }

            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var document = JsonDocument.Parse(seasonJson))
            {
                if (document.RootElement.TryGetProperty("results", out var results) &&
                    results.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("cluster_details", out var clusterDetails))
                {
                    foreach (var cluster in clusterDetails.EnumerateObject())
                    {
                        if (!cluster.Value.TryGetProperty("units_string", out var unitsString)) continue;

                        foreach (string key in unitsString.GetString()?
                            .Split(", ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
                        {
                            keys.Add(key);
                        }
                    }
                }
            }

            if (keys.Count > 0)
            {
                CurrentSeasonHeroKeys = keys.ToList();
                OutputForm.Instance.WriteLineOutputMessage($"已从阵容数据确定当前赛季英雄，找到 {CurrentSeasonHeroKeys.Count} 位英雄。");
                return;
            }

            ProcessUnitListData(unitListJson);
        }

        private void ProcessTranslationData(string json)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var translationData = JsonSerializer.Deserialize<TranslationData>(json, options);

            if (translationData == null || translationData.Units == null ||translationData.Items == null || translationData.Traits == null)
            {
                throw new InvalidOperationException("未能正确解析翻译数据或数据格式无效。");
            }

            var translatedUnits = translationData.Units
                .Where(unit => !string.IsNullOrEmpty(unit.ApiName) && !string.IsNullOrEmpty(unit.Name))
                .ToList();

            HeroTranslations = translatedUnits
                .SelectMany(unit => new[] { unit.ApiName }
                    .Concat(unit.AssetNames ?? [] )
                    .Where(key => !string.IsNullOrWhiteSpace(key))
                    .Select(key => new KeyValuePair<string, string>(key, unit.Name)))
                .GroupBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().Value, StringComparer.OrdinalIgnoreCase);

            _seasonHeroKeysFromTranslations = translatedUnits
                .Where(unit => unit.ShopUnit)
                .Select(unit => unit.AssetNames?.FirstOrDefault() ?? unit.ApiName)
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            LineUpCodeToName = new Dictionary<string, string>(ExtractLineUpCodeToName(json), StringComparer.OrdinalIgnoreCase);

            _heroTranslationAliases = translatedUnits
                .SelectMany(unit => new[] { unit.ApiName }
                    .Concat(unit.AssetNames ?? [])
                    .Where(key => !string.IsNullOrWhiteSpace(key))
                    .Select(key => new KeyValuePair<string, string>(NormalizeHeroApiKey(key), unit.Name)))
                .Where(pair => pair.Key.Length > 0)
                .GroupBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().Value, StringComparer.OrdinalIgnoreCase);

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

        private static IReadOnlyDictionary<string, string> ExtractLineUpCodeToName(string json)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("units", out JsonElement unitsElement) ||
                unitsElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("翻译数据不包含有效的英雄列表。");
            }

            bool hasShopUnitProperty = unitsElement.EnumerateArray()
                .Any(unit => unit.TryGetProperty("shopUnit", out _));
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            TranslationData? translationData = JsonSerializer.Deserialize<TranslationData>(json, options);
            if (translationData?.Units == null)
            {
                throw new InvalidOperationException("未能正确解析阵容码字典数据。");
            }

            return translationData.Units
                .Where(unit => !string.IsNullOrWhiteSpace(unit.Code) && !string.IsNullOrWhiteSpace(unit.Name))
                .Where(unit => !hasShopUnitProperty || unit.ShopUnit)
                .GroupBy(unit => unit.Code, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().Name, StringComparer.OrdinalIgnoreCase);
        }

        public string GetHeroTranslation(string apiName)
        {
            if (string.IsNullOrWhiteSpace(apiName)) return string.Empty;

            // 字典取值为 null 时视为未命中，记录日志后回退到别名或原始名称，避免把 null 当作翻译结果返回
            if (HeroTranslations.TryGetValue(apiName, out string? translatedName) && translatedName != null)
            {
                return translatedName;
            }

            string normalizedKey = NormalizeHeroApiKey(apiName);
            if (_heroTranslationAliases.TryGetValue(normalizedKey, out string? aliasName) && aliasName != null)
            {
                return aliasName;
            }

            // 未命中翻译属正常情况（新英雄上线早于翻译表更新），回退为原始名称。
            // 该路径会被每个英雄调用一次，故只写调试输出，避免污染用户日志
            Debug.WriteLine($"[DynamicGameDataService] GetHeroTranslation 未命中英雄翻译，回退为原始名称：{apiName}");
            return apiName;
        }

        private static string NormalizeHeroApiKey(string apiName)
        {
            string normalized = apiName.Trim();
            int underscoreIndex = normalized.IndexOf('_');
            if (normalized.StartsWith("DA_", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized[3..];
            }
            else if (normalized.StartsWith("TFT", StringComparison.OrdinalIgnoreCase) && underscoreIndex >= 0)
            {
                normalized = normalized[(underscoreIndex + 1)..];
            }

            normalized = normalized.Replace("_AD", string.Empty, StringComparison.OrdinalIgnoreCase)
                                   .Replace("_AP", string.Empty, StringComparison.OrdinalIgnoreCase)
                                   .Replace("Small", string.Empty, StringComparison.OrdinalIgnoreCase);
            return new string(normalized.Where(character => !char.IsDigit(character) && character != '_').ToArray());
        }

        #region 内部数据模型

        private class UnitListResponse
        {
            [JsonPropertyName("tft_set")]
            public string TftSet { get; set; } = null!;

            [JsonPropertyName("units")]
            public Dictionary<string, object> Units { get; set; } = null!;
        }

        #endregion
    }
}
