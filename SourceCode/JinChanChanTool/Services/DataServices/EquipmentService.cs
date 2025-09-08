using HtmlAgilityPack;
using JinChanChanTool.DataClass;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace JinChanChanTool.Services.DataServices
{
    /// <summary>
    /// 负责读取和提供装备推荐数据的服务
    /// </summary>
    public class EquipmentService
    {
        // 用于在内存中存储从JSON文件读取的装备数据
        // Key是英雄名(string), Value是推荐装备列表(List<string>)
        private Dictionary<string, List<string>> _equipmentData;

        /// <summary>
        /// 构造函数，当创建EquipmentService实例时，会自动加载数据
        /// </summary>
        public EquipmentService()
        {
            LoadData();
        }

        /// <summary>
        /// 从本地JSON文件加载数据到内存中
        /// </summary>
        private void LoadData()
        {
            try
            {
                // 使用相对路径定位JSON文件
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Equipment", "EquipmentData.json");

                if (File.Exists(jsonPath))
                {
                    string jsonString = File.ReadAllText(jsonPath, Encoding.UTF8);
                    // 使用 System.Text.Json 将JSON字符串反序列化为字典对象
                    _equipmentData = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString);
                }
                else
                {
                    // 如果文件不存在，则初始化一个空字典，防止程序出错
                    _equipmentData = new Dictionary<string, List<string>>();
                }
            }
            catch (Exception ex)
            {
                // 捕获可能发生的任何错误（如JSON格式错误），并初始化为空字典
                _equipmentData = new Dictionary<string, List<string>>();
                // 现在测试，后续可以改成记录错误日志
                Console.WriteLine($"加载装备数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 根据英雄名字获取推荐装备列表
        /// </summary>
        /// <param name="heroName">要查询的英雄名字</param>
        /// <returns>一个包含装备名称的列表；如果找不到该英雄，则返回一个空列表</returns>
        public List<string> GetItemsForHero(string heroName)
        {
            // 检查数据是否已加载以及是否包含该英雄
            if (_equipmentData != null && _equipmentData.TryGetValue(heroName, out var items))
            {
                return items;
            }

            // 无论任何情况，都不要返回null，返回一个空列表维持程序的正常
            return new List<string>();
        }

        /// <summary>
        /// 使用 Puppeteer Sharp 抓取 datatft.com (V7 - 融合了所有发现的最终版)
        /// </summary>
        public async Task<bool> UpdateDataFromWebAsync(IProgress<Tuple<int, string>> progress)
        {
            // 构建装备ID->中文名映射表
            var equipmentIdToNameMap = new Dictionary<string, string>
            {
                { "519", "死亡之刃" }, { "521", "巨人杀手" }, { "525", "朔极之矛" }, { "529", "饮血剑" }, { "535", "无尽之刃" },
                { "539", "鬼索的狂暴之刃" }, { "551", "最后的轻语" }, { "553", "灭世者的死亡之帽" }, { "555", "大天使之杖" }, { "561", "莫雷洛秘典" },
                { "565", "珠光护手" }, { "567", "蓝霸符" }, { "577", "正义之手" }, { "579", "棘刺背心" }, { "581", "石像鬼石板甲" },
                { "583", "日炎斗篷" }, { "595", "水银" }, { "597", "狂徒铠甲" }, { "607", "窃贼手套" }, { "608", "金铲铲冠冕" },
                { "609", "巨龙之爪" }, { "610", "海克斯科技枪刃" }, { "611", "离子火花" }, { "613", "泰坦的坚决" }, { "615", "夜之锋刃" },
                { "616", "圣盾使的誓约" }, { "617", "强袭者的链枷" }, { "618", "斯特拉克的挑战护手" }, { "619", "纳什之牙" }, { "620", "冕卫" },
                { "621", "适应性头盔" }, { "623", "薄暮法袍" }, { "624", "红霸符" }, { "625", "坚定之心" },
                { "1519", "光明版死亡之刃" }, { "1521", "光明版巨人杀手" }, { "1523", "光明版海克斯科技枪刃" }, { "1525", "光明版朔极之矛" }, { "1527", "光明版夜之锋刃" },
                { "1529", "光明版饮血剑" }, { "1535", "光明版无尽之刃" }, { "1539", "光明版鬼索的狂暴之刃" }, { "1543", "光明版泰坦的坚决" }, { "1551", "光明版最后的轻语" },
                { "1553", "光明版灭世者的死亡之帽" }, { "1555", "光明版大天使之杖" }, { "1559", "光明版离子火花" }, { "1561", "光明版莫雷洛秘典" }, { "1565", "光明版珠光护手" },
                { "1567", "光明版蓝霸符" }, { "1577", "光明版正义之手" }, { "1579", "光明版棘刺背心" }, { "1581", "光明版石像鬼石板甲" }, { "1583", "光明版日炎斗篷" },
                { "1595", "光明版水银" }, { "1597", "光明版狂徒铠甲" }, { "1607", "光明版窃贼手套" }, { "1616", "光明版圣盾使的誓约" }, { "1617", "光明版强袭者的链枷" },
                { "1618", "光明版斯特拉克的挑战护手" }, { "1619", "光明版纳什之牙" }, { "1620", "光明版冕卫" }, { "1621", "光明版适应性头盔" }, { "1623", "光明版薄暮法袍" },
                { "1624", "光明版红霸符" }, { "1625", "光明版坚定之心" },
                { "20002", "死亡之蔑" }, { "20003", "魔蕴" }, { "20004", "三相之力" }, { "20009", "金币收集者" }, { "20010", "中娅悖论" },
                { "20011", "碎舰者" }, { "20012", "冥火之拥" }, { "20013", "诡术师之镜" }, { "20014", "狙击手的专注" }, { "20015", "铁匠手套" },
                { "20017", "大亨之铠" }, { "20044", "黎明核心" }, { "20101", "投机者之刃" },
                { "2271", "狙神纹章" }, { "2272", "星之守护者纹章" }, { "2273", "刀锋领主纹章" }, { "2274", "法师纹章" }, { "2275", "圣盾使纹章" },
                { "2276", "决斗大师纹章" }, { "2277", "天才纹章" }, { "2278", "假面摔角手纹章" }, { "2279", "重量级斗士纹章" }, { "2280", "裁决使者纹章" },
                { "2281", "战斗学院纹章" }, { "2282", "水晶玫瑰纹章" }, { "2283", "主宰纹章" }, { "2284", "司令纹章" }, { "2285", "至高天纹章" },
                { "2286", "兵王纹章" }, { "2287", "斗魂战士纹章" }, { "2288", "护卫纹章" },
                { "30002", "飞升护符" }, { "30004", "鱼骨头" }, { "30005", "迷离风衣" }, { "30006", "视界专注" }, { "30007", "连指手套" },
                { "30008", "无终恨意" }, { "30009", "疾射火炮" }, { "30010", "激发之匣" }, { "30011", "卢登的激荡" }, { "30012", "密银黎明" },
                { "30013", "暗行者之爪" }, { "30015", "幽魂弯刀" }, { "30016", "枯萎珠宝" }, { "30017", "智慧末刃" }, { "30019", "巫妖之祸" },
                { "30020", "光盾徽章" }, { "30021", "探索者的护臂" }, { "30044", "光明版巨龙之爪" },
                { "31002", "金锅锅冠冕" }, { "31003", "金锅铲冠冕" }, { "31004", "烁刃" }, { "31005", "斯塔缇克电刃" }, { "31006", "巨型九头蛇" },
                { "31007", "顽强不屈" }, { "31008", "虚空之杖" }, { "31009", "光明版虚空之杖" }, { "31010", "海妖之怒" }, { "31011", "光明版海妖之怒" },
                { "31012", "振奋盔甲" }, { "31013", "光明版振奋盔甲" }
            };
            var newEquipmentData = new System.Collections.Concurrent.ConcurrentDictionary<string, List<string>>();

            try
            {
                System.Diagnostics.Debug.WriteLine("正在准备浏览器环境...");
                var browserFetcher = GetCustomBrowserFetcher();

                // v13+ 的写法：DownloadAsync 会返回 InstalledBrowser
                var installedBrowser = await browserFetcher.DownloadAsync();

                await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox" },
                    ExecutablePath = installedBrowser.GetExecutablePath() 
                });
                System.Diagnostics.Debug.WriteLine("浏览器准备就绪。");

                // 第一步：获取英雄列表
                var heroIdMap = new Dictionary<string, string>();
                await using (var page = await browser.NewPageAsync())
                {
                    System.Diagnostics.Debug.WriteLine("正在访问英雄列表页...");
                    string listUrl = "https://www.datatft.com/hero";
                    await page.GoToAsync(listUrl, new NavigationOptions { Timeout = 30000, WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } });
                    await page.WaitForSelectorAsync("tr.el-table__row", new WaitForSelectorOptions { Timeout = 30000 });
                    System.Diagnostics.Debug.WriteLine("英雄列表页加载完成。");
                    string listContent = await page.GetContentAsync();
                    var listDoc = new HtmlAgilityPack.HtmlDocument();
                    listDoc.LoadHtml(listContent);
                    var heroRows = listDoc.DocumentNode.SelectNodes("//tr[contains(@class, 'el-table__row')]");
                    if (heroRows == null) { return false; } // 获取失败
                    foreach (var row in heroRows)
                    {
                        var nameNode = row.SelectSingleNode(".//div[contains(@class, 'name-text')]");
                        var linkNode = row.SelectSingleNode(".//a");
                        if (nameNode != null && linkNode != null)
                        {
                            string chineseName = nameNode.InnerText.Trim();
                            string href = linkNode.GetAttributeValue("href", "");
                            string heroId = HttpUtility.ParseQueryString(new Uri("https://www.datatft.com" + href).Query).Get("id");
                            if (!string.IsNullOrEmpty(chineseName) && !string.IsNullOrEmpty(heroId))
                            {
                                heroIdMap[chineseName] = heroId;
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"成功构建英雄映射表，共 {heroIdMap.Count} 位英雄。");

                // 在这里应用黑名单
                var heroBlacklist = new List<string> { "璐璐" }; // 定义黑名单
                // 使用LINQ过滤掉黑名单中的英雄
                var heroesToScrape = heroIdMap
                    .Where(hero => !heroBlacklist.Contains(hero.Key))
                    .ToDictionary(hero => hero.Key, hero => hero.Value);

                System.Diagnostics.Debug.WriteLine($"应用黑名单后，将抓取 {heroesToScrape.Count} 位英雄的数据。");
                foreach (var blacklistedHero in heroIdMap.Keys.Except(heroesToScrape.Keys))
                {
                    System.Diagnostics.Debug.WriteLine($" -> 已跳过黑名单英雄: {blacklistedHero}");
                }


                // 第二步：并行抓取详情页
                const int MAX_CONCURRENT_TASKS = 5;
                var semaphore = new System.Threading.SemaphoreSlim(MAX_CONCURRENT_TASKS);
                var tasks = new List<Task>();
                var processedCount = 0;

                // 【修改】使用过滤后的英雄列表，并更新总数
                int totalHeroes = heroesToScrape.Count;
                if (totalHeroes == 0) // 如果过滤后没有英雄了
                {
                    MessageBox.Show("没有需要抓取的英雄数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true; // 可以认为任务成功完成
                }

                // 【修改】遍历过滤后的字典
                foreach (var heroPair in heroesToScrape)
                {
                    await semaphore.WaitAsync();
                    tasks.Add(Task.Run(async () =>
                    {
                        await ScrapeHeroDataAsync(
                            browser,
                            heroPair,
                            equipmentIdToNameMap,
                            newEquipmentData,
                            progress,
                            totalHeroes,
                            semaphore,
                            () => System.Threading.Interlocked.Increment(ref processedCount)
                        );
                    }));
                }
                await Task.WhenAll(tasks);

                // 第三步：保存数据
                if (!newEquipmentData.IsEmpty)
                {
                    var finalData = new Dictionary<string, List<string>>(newEquipmentData);
                    var options = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                    string jsonString = JsonSerializer.Serialize(finalData, options);
                    string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Equipment", "EquipmentData.json");
                    await File.WriteAllTextAsync(jsonPath, jsonString);
                    LoadData();
                    MessageBox.Show($"成功更新了 {newEquipmentData.Count} 位英雄的出装数据！", "更新完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true; // 成功时返回 true
                }
                else
                {
                    MessageBox.Show("未能抓取到任何有效的出装数据。", "更新失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false; // 数据为空时返回 false
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!!!!!!!!! 发生严重错误 !!!!!!!!!!\n{ex}");
                MessageBox.Show($"更新时发生错误: {ex.Message}", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // 发生异常时返回 false
            }
        }

        /// <summary>
        /// 抓取单个英雄数据的核心逻辑
        /// </summary>
        private async Task ScrapeHeroDataAsync(
            IBrowser browser,
            KeyValuePair<string, string> heroPair,
            Dictionary<string, string> equipmentIdToNameMap,
            System.Collections.Concurrent.ConcurrentDictionary<string, List<string>> newEquipmentData,
            IProgress<Tuple<int, string>> progress,
            int totalHeroes,
            System.Threading.SemaphoreSlim semaphore,
            Func<int> incrementAction // 接收一个返回int的委托
        )
        {
            await using var page = await browser.NewPageAsync();
            try
            {
                // 所有的抓取逻辑都放在 try 块里
                string chineseName = heroPair.Key;
                string heroId = heroPair.Value;
                string detailUrl = $"https://www.datatft.com/detail?id={heroId}&type=hero";
                await page.GoToAsync(detailUrl, new NavigationOptions { Timeout = 30000, WaitUntil = new[] { WaitUntilNavigation.Networkidle2 } });

                string tabSelector = "#tab-equipBuild";
                await page.WaitForSelectorAsync(tabSelector, new WaitForSelectorOptions { Timeout = 30000 });
                await page.ClickAsync(tabSelector);
                await Task.Delay(300);

                string scriptToExecute = @"() => {
                    const visibleTable = document.querySelector('div.table:not([style*=""display: none""])');
                    if (!visibleTable) return [];
                    const firstRow = visibleTable.querySelector('.el-table__row:first-child');
                    if (!firstRow) return [];
                    const imageNodes = firstRow.querySelectorAll('.name-wrapper img');
                    return Array.from(imageNodes).map(img => img.src);
                }";

                string[] imgSrcs = await page.EvaluateFunctionAsync<string[]>(scriptToExecute);

                if (imgSrcs.Any())
                {
                    var items = new List<string>();
                    var foundIds = new List<string>();
                    foreach (var src in imgSrcs.Take(3))
                    {
                        string equipId = Path.GetFileNameWithoutExtension(src);
                        foundIds.Add(equipId ?? "NULL");
                        if (equipmentIdToNameMap.ContainsKey(equipId))
                        {
                            items.Add(equipmentIdToNameMap[equipId]);
                        }
                    }
                    if (items.Count >= 3)
                    {
                        newEquipmentData[chineseName] = items;
                        System.Diagnostics.Debug.WriteLine($" -> 【{chineseName}】成功获取: {string.Join(", ", items)}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" -> !!! 处理【{heroPair.Key}】时出错: {ex.Message.Split('\n')[0]}");
            }
            finally
            {
                int currentProgress = incrementAction(); // 调用传入的委托来增加计数并获取最新值

                int percentage = (int)((double)currentProgress / totalHeroes * 100);
                string message = $"正在处理: {heroPair.Key} ({currentProgress}/{totalHeroes})";
                progress?.Report(Tuple.Create(percentage, message));

                semaphore.Release(); // 释放信号量
            }
        }

        /// <summary>
        /// 清理 Puppeteer Sharp 下载的浏览器缓存 
        /// </summary>
        public void CleanUpBrowserCache()
        {
            try
            {
                // 指定自定义的缓存目录 "Browser Cache"
                string customCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Browser Cache");
                // 如果目录不存在，则自动创建
                if (!Directory.Exists(customCachePath))
                {
                    Directory.CreateDirectory(customCachePath);
                    System.Diagnostics.Debug.WriteLine($"创建自定义浏览器缓存目录: {customCachePath}");
                }

                // 使用自定义缓存目录构造BrowserFetcher
                var browserFetcherOptions = new BrowserFetcherOptions
                {
                    Path = customCachePath
                };
                var browserFetcher = new BrowserFetcher(browserFetcherOptions);
                string cachePath = browserFetcher.CacheDir;

                System.Diagnostics.Debug.WriteLine($"准备清理浏览器缓存，路径: {cachePath}");

                if (Directory.Exists(cachePath))
                {
                    Directory.Delete(cachePath, true);
                    System.Diagnostics.Debug.WriteLine("浏览器缓存清理成功！");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("未找到浏览器缓存目录，无需清理。");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"清理浏览器缓存时发生错误: {ex.Message}");
            }
        }

        // 统一创建自定义的BrowserFetcher
        private BrowserFetcher GetCustomBrowserFetcher()
        {
            string customCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Browser Cache");
            if (!Directory.Exists(customCachePath))
            {
                Directory.CreateDirectory(customCachePath);
                System.Diagnostics.Debug.WriteLine($"创建自定义浏览器缓存目录: {customCachePath}");
            }
            var browserFetcherOptions = new BrowserFetcherOptions
            {
                Path = customCachePath
            };
            return new BrowserFetcher(browserFetcherOptions);
        }
    }
}
