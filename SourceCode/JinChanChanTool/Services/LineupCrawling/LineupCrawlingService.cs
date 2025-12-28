using JinChanChanTool.DataClass;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.LineupCrawling.Interface;
using JinChanChanTool.Services.RecommendedEquipment.Interface;
using JinChanChanTool.Services.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace JinChanChanTool.Services.LineupCrawling
{
    /// <summary>
    /// 核心阵容爬取服务。
    /// 包含爬取流、Tier评分算法、标签清洗以及站位坐标转换。
    /// </summary>
    public class LineupCrawlingService : ILineupCrawlingService
    {
        private readonly IDynamicGameDataService _gameDataService;

        /// <summary>
        /// 临时存储阵容ID与英雄API Key列表的映射，用于站位精准匹配
        /// </summary>
        private readonly ConcurrentDictionary<string, List<string>> _clusterHeroKeysMap = new();

        // API 路由常量
        private const string MetadataUrl = "https://api-hc.metatft.com/tft-comps-api/comps_data?queue=1100";
        private const string StatsUrl = "https://api-hc.metatft.com/tft-comps-api/comps_stats?queue=1100&patch=current&days=1&rank=CHALLENGER,DIAMOND,GRANDMASTER,MASTER&permit_filter_adjustment=true";
        private const string DetailUrlBase = "https://api-hc.metatft.com/tft-comps-api/comp_details?comp={0}&cluster_id={1}";

        public LineupCrawlingService(IDynamicGameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }



        /// <summary>
        ///  执行三步爬取逻辑。
        /// </summary>
        public async Task<List<RecommendedLineUp>> GetRecommendedLineUpsAsync(IProgress<Tuple<int, string>> progress)
        {
            _clusterHeroKeysMap.Clear();

            try
            {
                //  获取元数据 (Metadata)
                progress?.Report(Tuple.Create(10, "正在获取阵容基础数据..."));
                var metadata = await FetchMetadataAsync();
                if (metadata == null || !metadata.Any()) return new List<RecommendedLineUp>();

                //  获取统计数据 (Stats) 并计算评分
                progress?.Report(Tuple.Create(30, "正在获取实时统计并计算评级..."));
                var initialLineups = await FetchAndCalculateStatsAsync(metadata);
                if (!initialLineups.Any()) return new List<RecommendedLineUp>();

                // 获取阵容详情 (Tags & Positioning)
                // 筛选出评分较好的阵容进行详情抓取（或者全量抓取）
                progress?.Report(Tuple.Create(50, "正在获取阵容标签与站位详情..."));
                var finalLineups = await FetchLineupDetailsAsync(initialLineups, progress);

                progress?.Report(Tuple.Create(100, "阵容数据处理完毕！"));
                return finalLineups;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"阵容爬取流程发生严重错误: {ex.Message}");
                return new List<RecommendedLineUp>();
            }
        }

        #region 元数据处理
        private async Task<List<RecommendedLineUp>> FetchMetadataAsync()
        {
            using var response = await HttpProvider.Client.GetAsync(MetadataUrl, HttpCompletionOption.ResponseContentRead);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<CompsDataResponse>(json);
            var results = new List<RecommendedLineUp>();

            if (data?.Results?.Data?.ClusterDetails == null) return null;

            foreach (var kvp in data.Results.Data.ClusterDetails)
            {
                var detail = kvp.Value;
                var lineup = new RecommendedLineUp
                {
                    // 调用TranslateLineupName
                    LineUpName = TranslateLineupName(detail.Name),
                    Description = kvp.Key // 暂时存 ClusterID 方便后续匹配
                };

                // 处理 Units 和装备
                var unitKeys = detail.UnitsString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                _clusterHeroKeysMap[kvp.Key] = unitKeys;
                foreach (var uKey in unitKeys)
                {
                    string rawName = _gameDataService.HeroTranslations.GetValueOrDefault(uKey, uKey);
                    string heroName = rawName.Replace("·", "").Trim();

                    // 匹配该棋子的推荐装备
                    var build = detail.Builds?.FirstOrDefault(b => b.Unit == uKey);
                    string[] equips = ["", "", ""];
                    if (build?.BuildName != null)
                    {
                        for (int i = 0; i < Math.Min(3, build.BuildName.Count); i++)
                        {
                            equips[i] = _gameDataService.ItemTranslations.GetValueOrDefault(build.BuildName[i], build.BuildName[i]);
                        }
                    }
                    lineup.LineUpUnits.Add(new LineUpUnit(heroName, equips[0], equips[1], equips[2]));
                }
                results.Add(lineup);
            }
            return results;
        }

        private string TranslateLineupName(List<CompNameItem> nameItems)
        {
            if (nameItems == null || !nameItems.Any()) return "未知阵容";

            var nameParts = nameItems.Select(item =>
            {
                // 根据 DTO 中的 Type 字段精准匹配字典
                if (item.Type == "unit")
                    return _gameDataService.HeroTranslations.GetValueOrDefault(item.Name, item.Name);
                if (item.Type == "trait")
                    return _gameDataService.TraitTranslations.GetValueOrDefault(item.Name, item.Name);

                return item.Name; // 兜底返回原名
            });

            string rawName = string.Join(" ", nameParts);
            return rawName.Replace("·", "").Replace("  ", " ").Trim();
        }
        #endregion

        #region 统计计算逻辑
        private async Task<List<RecommendedLineUp>> FetchAndCalculateStatsAsync(List<RecommendedLineUp> metadata)
        {
            using var response = await HttpProvider.Client.GetAsync(StatsUrl, HttpCompletionOption.ResponseContentRead);
            if (!response.IsSuccessStatusCode) return metadata;

            var json = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<CompsStatsResponse>(json);
            if (stats?.Results == null || stats.Results.Count == 0) return metadata;

            // 分母: 总对局数
            double totalMatches = stats.Results[0].Places.FirstOrDefault();
            if (totalMatches <= 0) return metadata;

            var processedList = new List<RecommendedLineUp>();

            foreach (var lineup in metadata)
            {
                var stat = stats.Results.FirstOrDefault(s => s.Cluster == lineup.Description); // Description 存的是 ClusterID
                if (stat == null || stat.Count == 0) continue;

                double compCount = stat.Count;

                // 1. 平均名次 (Avg Rank) - 索引 0-7
                double weightedSum = 0;
                for (int i = 0; i < 8; i++)
                {
                    weightedSum += stat.Places[i] * (i + 1);
                }
                lineup.AverageRank = Math.Round(weightedSum / compCount, 2);

                // 2. 胜率 (Win Rate)
                lineup.WinRate = Math.Round((stat.Places[0] / compCount) * 100, 2);

                // 3. 前四率 (Top 4 Rate)
                double top4Count = stat.Places.Take(4).Sum();
                lineup.TopFourRate = Math.Round((top4Count / compCount) * 100, 2);

                // 4. 选择率 (Pick Rate) - 这里计算用于评分的比率值
                double pickRateRatio = (compCount / totalMatches) * 8;
                lineup.PickRate = Math.Round(pickRateRatio * 100, 2);

                // 5. 评级 (Tier) 计算
                double score = lineup.AverageRank - (1.6 * pickRateRatio);
                lineup.Tier = CalculateTier(score);

                processedList.Add(lineup);
            }

            return processedList;
        }

        private LineUpTier CalculateTier(double score)
        {
            if (score < 3.75) return LineUpTier.S;
            if (score < 4.15) return LineUpTier.A;
            if (score < 4.45) return LineUpTier.B;
            if (score < 5.00) return LineUpTier.C;
            return LineUpTier.D;
        }
        #endregion

        #region 详情、标签与站位转换
        private async Task<List<RecommendedLineUp>> FetchLineupDetailsAsync(List<RecommendedLineUp> lineups, IProgress<Tuple<int, string>> progress)
        {
            const int MAX_CONCURRENT = 10; // 并发数 10
            var semaphore = new SemaphoreSlim(MAX_CONCURRENT);
            var tasks = new List<Task>();
            var results = new ConcurrentBag<RecommendedLineUp>();
            int count = 0;

            foreach (var lineup in lineups)
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        string clusterId = lineup.Description; // 获取原始ID
                        string url = string.Format(DetailUrlBase, clusterId, clusterId.Substring(0, 3));

                        using var response = await HttpProvider.Client.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            var detail = JsonSerializer.Deserialize<CompDetailsResponse>(json);

                            if (detail?.Results != null)
                            {
                                // 解析标签
                                lineup.Tags = ProcessTags(detail.Results);
                                // 解析并打印站位
                                ProcessPositioning(lineup.LineUpName, detail.Results.Positioning, lineup.LineUpUnits, clusterId);
                            }
                        }
                        lineup.Description = ""; // 清空 ID
                        results.Add(lineup);
                    }
                    catch (Exception ex) { Debug.WriteLine($"详情抓取失败: {ex.Message}"); }
                    finally
                    {
                        Interlocked.Increment(ref count);
                        progress?.Report(Tuple.Create(50 + (int)((double)count / lineups.Count * 50), $"正在处理详情: {lineup.LineUpName}"));
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return results.OrderBy(l => l.Tier).ThenByDescending(l => l.WinRate).ToList();
        }

        private List<string> ProcessTags(CompDetailsResults results)
        {
            var tags = new List<string>();
            if (results.ProComps == null || results.ProComps.Count < 2) return tags;

            var titleImages = results.ProComps[1].Content?.InnerContent?.TitleImages;
            if (titleImages == null) return tags;

            foreach (var img in titleImages)
            {
                string raw = img.ApiName;
                if (string.IsNullOrEmpty(raw)) continue;

                // 阵容标签中莫名出现这个，直接过滤（这个是暗裔之镰），不翻译也不填充
                if (raw == "TFT16_TheDarkinScythe") continue;

                // 1. 处理 Reroll 特殊标签
                if (raw.Contains("Reroll"))
                {
                    if (raw.Contains("78")) tags.Add("7/8级d牌");
                    else if (raw.Contains("6")) tags.Add("6级d牌");
                    else if (raw.Contains("5")) tags.Add("5级d牌");
                    continue;
                }

                // 2. 清洗后缀并翻译
                string key = raw.Replace("TFT_Custom_Econ_", "")
                                .Replace("TFT_Custom_Difficulty_", "")
                                .Replace("_", " ");

                key = Regex.Replace(key, @"Fast(\d+)", "Fast $1");

                string translated = _gameDataService.CommonTranslations.GetValueOrDefault(key, key);
                tags.Add(translated);
            }
            return tags;
        }

        /// <summary>
        /// 使用匈牙利算法解决全局最优站位分布问题
        /// </summary>
        private void ProcessPositioning(string lineupName, PositioningData data, List<LineUpUnit> units, string clusterId)
        {
            if (data?.Units == null || !_clusterHeroKeysMap.TryGetValue(clusterId, out var heroKeys)) return;

            int numHeroes = Math.Min(units.Count, heroKeys.Count);
            const int numCells = 28; // 棋盘总格数

            // 1. 构建代价矩阵 [英雄数量 x 28个格子]
            double[,] costMatrix = new double[numHeroes, numCells];

            for (int i = 0; i < numHeroes; i++)
            {
                string apiKey = heroKeys[i];
                if (data.Units.TryGetValue(apiKey, out var posInfo))
                {
                    // 计算该英雄的总样本数用于归一化概率
                    double totalCount = posInfo.Positions.Sum(p => p.Count);

                    // 预填一个极大的代价（代表英雄从未去过的格子）
                    for (int j = 0; j < numCells; j++) costMatrix[i, j] = 999.0;

                    foreach (var pos in posInfo.Positions)
                    {
                        if (int.TryParse(pos.Cell.Replace("cell_", ""), out int cellId) && cellId >= 1 && cellId <= 28)
                        {
                            double prob = pos.Count / totalCount;
                            // 代价函数：Cost = -log(P)，加个极小值防止log(0)
                            costMatrix[i, cellId - 1] = -Math.Log(prob + 1e-9);
                        }
                    }
                }
            }

            // 2. 调用匈牙利算法求解最小权重指派
            int[] assignment = SolveHungarian(costMatrix, numHeroes, numCells);

            // 3. 解析结果并打印输出
            Debug.WriteLine($"\n--- 全局最优站位分布 (指派算法): {lineupName} ---");
            LogTool.Log($"--- 全局最优站位分布: {lineupName} ---");

            for (int i = 0; i < numHeroes; i++)
            {
                int cellId = assignment[i] + 1; // 索引转回 cell_ID
                if (cellId > 0)
                {
                    // 坐标转换公式
                    int row = 4 - (cellId - 1) / 7;
                    int col = (cellId - 1) % 7 + 1;

                    // 保存站位到 LineUpUnit
                    units[i].Position = (row, col);

                    string heroName = units[i].HeroName;
                    Debug.WriteLine($"棋子: {heroName.PadRight(6)} | 分配格: cell_{cellId.ToString().PadRight(2)} | 坐标: ({row}, {col})");
                }
            }
        }

        /// <summary>
        /// 匈牙利算法实现 (最小权重指派)
        /// </summary>
        private int[] SolveHungarian(double[,] costMatrix, int rows, int cols)
        {
            int[] match = new int[cols];
            for (int i = 0; i < cols; i++) match[i] = -1;

            double[] lx = new double[rows]; // 左集合顶标
            double[] ly = new double[cols]; // 右集合顶标

            // 初始化顶标：左集合顶标设为该行最小值，右集合为0
            for (int i = 0; i < rows; i++)
            {
                lx[i] = double.MaxValue;
                for (int j = 0; j < cols; j++)
                    if (costMatrix[i, j] < lx[i]) lx[i] = costMatrix[i, j];
            }

            for (int i = 0; i < rows; i++)
            {
                while (true)
                {
                    bool[] vx = new bool[rows];
                    bool[] vy = new bool[cols];
                    double[] slack = new double[cols];
                    for (int k = 0; k < cols; k++) slack[k] = double.MaxValue;

                    if (DfsHungarian(i, vx, vy, lx, ly, costMatrix, match, slack, rows, cols)) break;

                    // 如果无法匹配，修改顶标
                    double d = double.MaxValue;
                    for (int j = 0; j < cols; j++)
                        if (!vy[j] && slack[j] < d) d = slack[j];

                    if (d == double.MaxValue) break; // 理论上不会发生

                    for (int j = 0; j < rows; j++) if (vx[j]) lx[j] += d;
                    for (int j = 0; j < cols; j++)
                    {
                        if (vy[j]) ly[j] -= d;
                        else slack[j] -= d;
                    }
                }
            }

            // 返回结果：英雄索引 -> 格子索引
            int[] result = new int[rows];
            for (int i = 0; i < rows; i++) result[i] = -1;
            for (int j = 0; j < cols; j++)
                if (match[j] != -1) result[match[j]] = j;

            return result;
        }

        private bool DfsHungarian(int u, bool[] vx, bool[] vy, double[] lx, double[] ly, double[,] cost, int[] match, double[] slack, int rows, int cols)
        {
            vx[u] = true;
            for (int v = 0; v < cols; v++)
            {
                if (vy[v]) continue;
                double diff = cost[u, v] - (lx[u] + ly[v]);
                if (Math.Abs(diff) < 1e-7) // 相等子图
                {
                    vy[v] = true;
                    if (match[v] == -1 || DfsHungarian(match[v], vx, vy, lx, ly, cost, match, slack, rows, cols))
                    {
                        match[v] = u;
                        return true;
                    }
                }
                else
                {
                    if (slack[v] > diff) slack[v] = diff;
                }
            }
            return false;
        }
        #endregion
    }
}