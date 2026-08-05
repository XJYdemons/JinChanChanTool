using JinChanChanTool.DataClass;
using JinChanChanTool.DataClass.StaticData;
using JinChanChanTool.Forms;
using JinChanChanTool.Forms.DisplayUIForm;
using JinChanChanTool.Services.AutoSetCoordinates;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Tools;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.MouseTools;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static JinChanChanTool.DataClass.LineUp;


namespace JinChanChanTool.Services
{
    public class CardService
    {
        /// <summary>
        /// 程序设置服务实例
        /// </summary>
        private readonly IManualSettingsService _iappConfigService;

        /// <summary>
        /// 自动设置服务实例
        /// </summary>
        private readonly IAutomaticSettingsService _iAutoConfigService;

        /// <summary>
        /// OCR结果纠正服务实例
        /// </summary>
        private readonly ICorrectionService _iCorrectionService;

        /// <summary>
        /// 英雄数据服务实例
        /// </summary>
        private readonly IHeroDataService _iheroDataService;

        /// <summary>
        /// 阵容数据服务实例
        /// </summary>
        private readonly ILineUpService _ilineUpService;

        //private KmBoxNet kmBox;

        //private KmBoxNetHelper km ;

        private QueuedOCRService _ocrService;

        public bool isHighLight = false;//是否开启 高亮提示 标志(初始false)
        public event Action<bool> isHighLightStatusChanged;
        public bool isGetCard = false;//是否开启 自动拿牌 标志(初始false)
        public event Action<bool> isGetCardStatusChanged;
        public bool isRefreshStore = false;//是否开启 自动刷新商店 标志(初始false)
        public event Action<bool> isRefreshStoreStatusChanged;
        public bool isPickupItems = false;//是否开启 自动拾取棋盘物品 标志(初始false)
        public event Action<bool> isPickupItemsStatusChanged;
        private CancellationTokenSource ctsHighLight = null;//控制高亮循环的取消令牌
        private CancellationTokenSource ctsGetCard = null;//控制拿牌循环的取消令牌
        private CancellationTokenSource ctsPickupItems = null;//控制拾取循环的取消令牌
        private string 上次拾取触发回合 = "";                  // 已触发过拾取的回合键（如 "3-1"），回合变化才再次触发
        private DateTime 上次拾取时间 = DateTime.MinValue;     // 触发冷却计时
        private int 回合识别失败计数 = 0;                       // 连续未识别到回合文本的累计次数（低频提示用）
        private const double PICKUP_TRIGGER_COOLDOWN_SECONDS = 15; // 同一回合触发拾取的最小间隔（秒）
        private const int PICKUP_POLL_INTERVAL_MS = 2000;      // 回合文本轮询间隔（毫秒）
        private const int PICKUP_CLICK_INTERVAL_MS = 600;      // 盲点坐标点击间隔（毫秒）
        // 触发拾取的回合（金铲铲开局野怪回合：1-2/1-3/1-4 打完掉落装备）
        // 在野怪回合的准备阶段触发：1-3 准备阶段捡 1-2 掉落物，1-4 准备阶段捡 1-3 掉落物
        // 注意：2-1 为海克斯选择阶段，不触发（避免选海克斯时鼠标乱动）
        private static readonly HashSet<string> PICKUP_TRIGGER_ROUNDS = new HashSet<string> { "1-2", "1-3", "1-4" };

        // 自动拾取棋盘物品：依赖自动坐标体系（由 MainForm 注入）
        public WindowInteractionService WindowInteraction { get; set; }
        public CoordinateCalculationService CoordService { get; set; }

        private bool 鼠标左键是否按下;
        private bool 本轮是否按下过鼠标;

        private int 未拿牌累积次数 = 0;

        private string[] 原始结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 纠正结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 上一轮结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 最近一次刷新轮商店状态 = new string[5] { "", "", "", "", "" };
        private bool[] 上一轮目标数组 = new bool[5] { false, false, false, false, false };
        private bool[] 当前目标数组 = new bool[5] { false, false, false, false, false };

        private const int 未刷新最大回合数 = 5;
        private const double 未刷新最大时间秒数 = 3.0;
        enum 刷新状态
        {
            未开始,
            刷新中,
            已结束
        }

        刷新状态 当前商店刷新状态 = 刷新状态.未开始;
        private int 未刷新累积次数 = 0;
        private int 从上次尝试刷新到目前为止经过的轮次 = 0;
        private Stopwatch 计时器 = Stopwatch.StartNew();

        public CardService(IManualSettingsService iAppConfigService, IAutomaticSettingsService iAutoConfigService, ICorrectionService iCorrectionService, IHeroDataService iHeroDataService, ILineUpService iLineUpService)
        {          
            _iappConfigService = iAppConfigService;
            _iAutoConfigService = iAutoConfigService;
            _iCorrectionService = iCorrectionService;
            _iheroDataService = iHeroDataService;
            _ilineUpService = iLineUpService;
            // 根据选中的按钮初始化OCR
            if (iAppConfigService.CurrentConfig.IsUseCPUForInference)
            {
                InitializeOcrService(QueuedOCRService.设备.CPU);
            }
            else if (iAppConfigService.CurrentConfig.IsUseGPUForInference)
            {
                InitializeOcrService(QueuedOCRService.设备.GPU);
            }
            else
            {
                InitializeOcrService(QueuedOCRService.设备.CPU);
            }
            //kmBox = new KmBoxNet();
            //kmBox.Init("192.168.2.188", "51200", "EA2A3CAB");
            //km = new KmBoxNetHelper(kmBox);           
        }

        /// <summary>
        /// 根据传入的设备初始化OCR对象
        /// </summary>
        /// <param name="device"></param>
        private void InitializeOcrService(QueuedOCRService.设备 device)
        {
            try
            {
                // 指定 CPU 核心数为 4
                _ocrService = new QueuedOCRService(device, 4);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"OCR初始化失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 开始OCR循环_高亮模式
        /// </summary>
        public void StartHighLight()
        {
            if(isGetCard)
            {
                StopLoop();
            }
            isHighLight = true;
            ctsHighLight = new CancellationTokenSource();
            isHighLightStatusChanged?.Invoke(true);

            // 从配置中读取高亮颜色设置并应用到高亮窗体
            CardHighlightOverlayForm.Instance.UpdateColorSettings(
                _iappConfigService.CurrentConfig.HighlightColor1,
                _iappConfigService.CurrentConfig.HighlightColor2,
                _iappConfigService.CurrentConfig.HighlightBorderWidth,
                _iappConfigService.CurrentConfig.HighlightGradientSpeed
            );

            // 显示高亮覆盖层窗体
            CardHighlightOverlayForm.Instance.ShowOverlay();
            // 启动循环任务
            Task.Run(() => ProcessLoop_HighLight(ctsHighLight.Token), ctsHighLight.Token);
        }

        /// <summary>
        /// 停止OCR循环_高亮模式
        /// </summary>
        public void StopHighLight()
        {
            isHighLight = false;
            ctsHighLight?.Cancel();
            ctsHighLight?.Dispose();
            ctsHighLight = null;
            isHighLightStatusChanged?.Invoke(false);
            // 隐藏高亮覆盖层窗体
            CardHighlightOverlayForm.Instance.HideOverlay();
        }

        public void ToggleHighLight()
        {
            if (isHighLight)
                StopHighLight();
            else
                StartHighLight();
        }

        /// <summary>
        /// 开始OCR循环_自动拿牌
        /// </summary>
        public void StartLoop()
        {
            if(isHighLight)
            {
                StopHighLight();
            }
            isGetCard = true;
            ctsGetCard = new CancellationTokenSource();
            isGetCardStatusChanged?.Invoke(true);
            // 启动循环任务
            Task.Run(() => ProcessLoop_GetCard(ctsGetCard.Token), ctsGetCard.Token);
        }

        /// <summary>
        /// 停止OCR循环_自动拿牌
        /// </summary>
        public void StopLoop()
        {
            isGetCard = false;
            ctsGetCard?.Cancel();
            ctsGetCard?.Dispose();
            ctsGetCard = null;
            isGetCardStatusChanged?.Invoke(false);
        }

        public void ToggleLoop()
        {
            if (isGetCard)
                StopLoop();
            else
                StartLoop();
        }

        public void ToggleRefreshStore()
        {
            if (isRefreshStore)
            {
                AutoRefreshOff();
            }
            else
            {
                AutoRefreshOn();
            }

        }

        /// <summary>
        /// 开启自动刷新商店
        /// </summary>
        public void AutoRefreshOn()
        {
            isRefreshStore = true;
            未刷新累积次数 = 0;
            isRefreshStoreStatusChanged?.Invoke(true);
        }

        /// <summary>
        /// 关闭自动刷新商店
        /// </summary>
        public void AutoRefreshOff()
        {
            isRefreshStore = false;
            isRefreshStoreStatusChanged?.Invoke(false);
        }

        #region 自动拾取棋盘物品（PVE 野怪回合后盲点拾取掉落物）
        /// <summary>
        /// 切换自动拾取棋盘物品
        /// </summary>
        public void TogglePickupItems()
        {
            if (isPickupItems)
                StopPickupItems();
            else
                StartPickupItems();
        }

        /// <summary>
        /// 开始自动拾取循环：OCR 读回合号 → 检测到 X-1（刚打完 X-7 野怪）→ 盲点棋盘拾取
        /// </summary>
        public void StartPickupItems()
        {
            if (isPickupItems) return;
            isPickupItems = true;
            ctsPickupItems = new CancellationTokenSource();
            isPickupItemsStatusChanged?.Invoke(true);
            Task.Run(() => ProcessLoop_PickupItems(ctsPickupItems.Token), ctsPickupItems.Token);
        }

        /// <summary>
        /// 停止自动拾取循环
        /// </summary>
        public void StopPickupItems()
        {
            isPickupItems = false;
            ctsPickupItems?.Cancel();
            ctsPickupItems?.Dispose();
            ctsPickupItems = null;
            isPickupItemsStatusChanged?.Invoke(false);
        }

        /// <summary>
        /// 自动拾取主循环：每 2 秒读一次回合文本，检测到新的 X-1 回合（野怪 X-7 刚打完）时
        /// 对棋盘 8 个盲点坐标依次左键点击（触发英雄移动拾取），全程限定游戏窗口内 + 程序点击标记
        /// </summary>
        private async Task ProcessLoop_PickupItems(CancellationToken token)
        {
            int 轮次 = 0;
            while (isPickupItems && !token.IsCancellationRequested)
            {
                try
                {
                    轮次++;
                    // 0. 前置检查：窗口已绑定 + 坐标服务可用（自动设置坐标模式下才有）
                    if (WindowInteraction == null || CoordService == null || !WindowInteraction.IsWindowFound)
                    {
                        LogTool.Log("自动拾取物品：未绑定游戏窗口（请先使用“自动设置坐标”选择游戏进程），跳过本轮");
                        OutputForm.Instance.WriteLineOutputMessage("自动拾取物品：未绑定游戏窗口（请先使用“自动设置坐标”选择游戏进程），跳过本轮");
                        await Task.Delay(3000, token);
                        continue;
                    }
                    // 1. 截取回合文本区域并识别（如 "1-2"、"3-1"）
                    Rectangle? roundRect = CoordService.GetScaledRectangle(
                        JccCoordinateTemplates.RoundText, JccCoordinateTemplates.BaseResolution, GameMode.JCC);
                    if (roundRect == null)
                    {
                        await Task.Delay(2000, token);
                        continue;
                    }
                    string roundText;
                    using (Bitmap bmp = ImageProcessingTool.AreaScreenshots(roundRect.Value))
                    // 回合号是小字（约 20-30px 高），放大 3 倍后再 OCR 以提高识别率
                    using (Bitmap scaled = ScaleBitmap(bmp, 3))
                    {
                        roundText = (await _ocrService.RecognizeTextAsync(scaled)).Trim();
                        // 调试：识别失败时保存该区域截图到 Logs，便于标定回合文本区域坐标
                        if (!Regex.Match(roundText, @"(\d+)\s*[-–]\s*(\d+)").Success && 回合识别失败计数 % 20 == 0)
                        {
                            try
                            {
                                string dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                                System.IO.Directory.CreateDirectory(dir);
                                string file = System.IO.Path.Combine(dir, $"PickupRoundText_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                                bmp.Save(file, System.Drawing.Imaging.ImageFormat.Png);
                                LogTool.Log($"自动拾取物品：已保存回合文本调试截图 {file}");
                                OutputForm.Instance.WriteLineOutputMessage($"自动拾取物品：已保存回合文本调试截图 {file}");
                            }
                            catch (Exception) { }
                        }
                    }
                    Match m = Regex.Match(roundText, @"(\d+)\s*[-–]\s*(\d+)");
                    if (!m.Success)
                    {
                        // 连续识别失败累计提示（避免每轮刷屏），便于排查回合文本区域坐标是否标偏
                        回合识别失败计数++;
                        if (回合识别失败计数 % 60 == 1) // 约每 2 分钟提示一次
                        {
                            LogTool.Log($"自动拾取物品：已连续约 {回合识别失败计数 * PICKUP_POLL_INTERVAL_MS / 1000} 秒未识别到回合文本（最近识别内容：\"{roundText}\"），请检查回合文本区域坐标");
                            OutputForm.Instance.WriteLineOutputMessage($"自动拾取物品：长时间未识别到回合文本（最近识别内容：\"{roundText}\"），请检查回合文本区域坐标");
                        }
                        await Task.Delay(2000, token);
                        continue;
                    }
                    回合识别失败计数 = 0;
                    int stage = int.Parse(m.Groups[1].Value);
                    int round = int.Parse(m.Groups[2].Value);
                    string roundKey = $"{stage}-{round}";
                    // 2. 触发条件：当前回合 ∈ 拾取触发集合（开局野怪回合 1-2/1-3/1-4）
                    //    + 回合键与上次不同（同一回合只触发一次）+ 距上次拾取超过冷却
                    if (PICKUP_TRIGGER_ROUNDS.Contains(roundKey) && roundKey != 上次拾取触发回合
                        && (DateTime.Now - 上次拾取时间).TotalSeconds >= PICKUP_TRIGGER_COOLDOWN_SECONDS)
                    {
                        LogTool.Log($"自动拾取物品：检测到回合 {roundKey}（野怪回合），开始盲点拾取棋盘物品");
                        OutputForm.Instance.WriteLineOutputMessage($"自动拾取物品：检测到回合 {roundKey}（野怪回合），开始盲点拾取棋盘物品");
                        上次拾取触发回合 = roundKey;
                        上次拾取时间 = DateTime.Now;
                        await 盲点拾取一轮(token);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    LogTool.Log($"自动拾取物品异常:{ex.Message}");
                }
                await Task.Delay(PICKUP_POLL_INTERVAL_MS, token); // 每 2 秒一轮
            }
            StopPickupItems();
        }

        /// <summary>
        /// 对棋盘 8 个盲点坐标依次右键点击（英雄移动路径自动拾取掉落物）
        /// 点击使用 ClickRightOnce（带程序点击标记，不干扰用户操作检测），点间间隔防连点
        /// </summary>
        private async Task 盲点拾取一轮(CancellationToken token)
        {
            int clicked = 0;
            foreach (var profile in JccCoordinateTemplates.PickupPoints)
            {
                if (!isPickupItems) return; // 中途关闭则停止
                Rectangle? rect = CoordService.GetScaledRectangle(
                    profile, JccCoordinateTemplates.BaseResolution, GameMode.JCC);
                if (rect == null) continue;
                int x = rect.Value.X + rect.Value.Width / 2;
                int y = rect.Value.Y + rect.Value.Height / 2;
                MouseControlTool.SetMousePosition(x, y);
                await Task.Delay(30, token);
                await ClickRightOnce();
                clicked++;
                LogTool.Log($"自动拾取物品：已右键点击棋盘点 ({x},{y})");
                OutputForm.Instance.WriteLineOutputMessage($"自动拾取物品：已右键点击棋盘点 ({x},{y})");
                await Task.Delay(PICKUP_CLICK_INTERVAL_MS, token); // 点间间隔
            }
            LogTool.Log($"自动拾取物品：本轮盲点拾取完成，共点击 {clicked} 个位置");
            OutputForm.Instance.WriteLineOutputMessage($"自动拾取物品：本轮盲点拾取完成，共点击 {clicked} 个位置");
        }

        /// <summary>
        /// 模拟一次右键点击（带程序点击标记，与左键 ClickOneTime 同机制）
        /// </summary>
        private async Task ClickRightOnce()
        {
            MouseHookTool.IncrementProgramClickCount(); // 标记程序点击
            MouseControlTool.MakeMouseRightButtonDown();
            MouseControlTool.MakeMouseRightButtonUp();
            await Task.Delay(1);
            MouseHookTool.DecrementProgramClickCount();
        }

        /// <summary>
        /// 将图片按整数倍放大（用于小字 OCR 前提升识别率）
        /// </summary>
        private static Bitmap ScaleBitmap(Bitmap src, int scale)
        {
            Bitmap dst = new Bitmap(src.Width * scale, src.Height * scale);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dst))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(src, 0, 0, dst.Width, dst.Height);
            }
            return dst;
        }
        #endregion

        /// <summary>
        /// 鼠标左键按下事件处理
        /// </summary>
        public void MouseLeftButtonDown()
        {
            鼠标左键是否按下 = true;
            本轮是否按下过鼠标 = true;
        }

        /// <summary>
        /// 鼠标左键抬起事件处理
        /// </summary>
        public void MouseLeftButtonUp()
        {
            鼠标左键是否按下 = false;
        }

        /// <summary>
        /// OCR处理主循环
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ProcessLoop_HighLight(CancellationToken token)
        {            
            int 高亮模式循环计数 = 0;            
            while (isHighLight && !token.IsCancellationRequested)
            {
                try
                {
                    高亮模式循环计数++;
                    LogTool.Log($"轮次:{高亮模式循环计数}");
                    Debug.WriteLine($"轮次:{高亮模式循环计数}");
                    OutputForm.Instance.WriteLineOutputMessage($"轮次:{高亮模式循环计数}");
                    Bitmap[] bitmaps = CaptureAndSplit();
                    原始结果数组 = await RecognizeImages(bitmaps);
                    更新纠正结果数组(bitmaps);
                    // 释放图像资源
                    foreach (Bitmap image in bitmaps)
                    {
                        image.Dispose();
                    }
                    当前目标数组 = CompareResults(纠正结果数组);
                    #region 结果输出、日志
                    string[] 输出结果数组1 = new string[5] { "", "", "", "", "" };
                    string[] 输出结果数组2 = new string[5] { "", "", "", "", "" };
                    for (int i = 0; i < 原始结果数组.Length; i++)
                    {
                        输出结果数组1[i] = "“" + Regex.Replace(原始结果数组[i], @"[^\u4e00-\u9fa5a-zA-Z0-9]", "") + "”";
                    }
                    for (int i = 0; i < 纠正结果数组.Length; i++)
                    {
                        输出结果数组2[i] = "“" + 纠正结果数组[i] + "”";
                    }
                    LogTool.Log($"原始结果 1:{输出结果数组1[0],-8}({当前目标数组[0],-6})2:{输出结果数组1[1],-8}({当前目标数组[1],-6})3:{输出结果数组1[2],-8}({当前目标数组[2],-6})4:{输出结果数组1[3],-8}({当前目标数组[3],-6})5:{输出结果数组1[4],-8}({当前目标数组[4],-6})");
                    Debug.WriteLine($"原始结果 1:{输出结果数组1[0],-8}({当前目标数组[0],-6})2:{输出结果数组1[1],-8}({当前目标数组[1],-6})3:{输出结果数组1[2],-8}({当前目标数组[2],-6})4:{输出结果数组1[3],-8}({当前目标数组[3],-6})5:{输出结果数组1[4],-8}({当前目标数组[4],-6})");
                    OutputForm.Instance.WriteLineOutputMessage($"原始结果 1:{输出结果数组1[0],-8}({当前目标数组[0],-6})2:{输出结果数组1[1],-8}({当前目标数组[1],-6})3:{输出结果数组1[2],-8}({当前目标数组[2],-6})4:{输出结果数组1[3],-8}({当前目标数组[3],-6})5:{输出结果数组1[4],-8}({当前目标数组[4],-6})");
                    LogTool.Log($"纠正结果 1:{输出结果数组2[0],-8}({当前目标数组[0],-6})2:{输出结果数组2[1],-8}({当前目标数组[1],-6})3:{输出结果数组2[2],-8}({当前目标数组[2],-6})4:{输出结果数组2[3],-8}({当前目标数组[3],-6})5:{输出结果数组2[4],-8}({当前目标数组[4],-6})");
                    Debug.WriteLine($"纠正结果 1:{输出结果数组2[0],-8}({当前目标数组[0],-6})2:{输出结果数组2[1],-8}({当前目标数组[1],-6})3:{输出结果数组2[2],-8}({当前目标数组[2],-6})4:{输出结果数组2[3],-8}({当前目标数组[3],-6})5:{输出结果数组2[4],-8}({当前目标数组[4],-6})");
                    OutputForm.Instance.WriteLineOutputMessage($"纠正结果 1:{输出结果数组2[0],-8}({当前目标数组[0],-6})2:{输出结果数组2[1],-8}({当前目标数组[1],-6})3:{输出结果数组2[2],-8}({当前目标数组[2],-6})4:{输出结果数组2[3],-8}({当前目标数组[3],-6})5:{输出结果数组2[4],-8}({当前目标数组[4],-6})");

                    #endregion                        
                    // 更新高亮显示
                    Rectangle[] cardRectangles = CalculateCardRectangles();
                    CardHighlightOverlayForm.Instance.UpdateHighlight(当前目标数组, cardRectangles);
                }
                catch (OperationCanceledException)
                {
                    // 正常取消
                    break;
                }
                catch (Exception ex)
                {
                    //跳出循环
                    break;
                }
            }
            Debug.WriteLine($"高亮loop：{!token.IsCancellationRequested},{isHighLight}");
            StopHighLight();            
        }

        /// <summary>
        /// OCR处理主循环
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ProcessLoop_GetCard(CancellationToken token)
        {
            int 循环计数 = 0;            
            循环前的标志重置();
            while (isGetCard && !token.IsCancellationRequested)
            {
                try
                {
                    循环计数++;
                    LogTool.Log($"轮次:{循环计数}     未刷新累积次数：{未刷新累积次数}     未拿牌累积次数：{未拿牌累积次数}");
                    Debug.WriteLine($"轮次:{循环计数}     未刷新累积次数：{未刷新累积次数}     未拿牌累积次数：{未拿牌累积次数}");
                    OutputForm.Instance.WriteLineOutputMessage($"轮次:{循环计数}     未刷新累积次数：{未刷新累积次数}     未拿牌累积次数：{未拿牌累积次数}");
                    if (!自动停止拿牌()) return;
                    自动停止刷新商店();


                    Bitmap[] bitmaps = CaptureAndSplit();



                    原始结果数组 = await RecognizeImages(bitmaps);



                    更新纠正结果数组(bitmaps);


                    // 释放图像资源
                    foreach (Bitmap image in bitmaps)
                    {
                        image.Dispose();
                    }
                    当前目标数组 = CompareResults(纠正结果数组);
                    #region 结果输出、日志
                    string[] 输出结果数组1 = new string[5] { "", "", "", "", "" };
                    string[] 输出结果数组2 = new string[5] { "", "", "", "", "" };
                    for (int i = 0; i < 原始结果数组.Length; i++)
                    {
                        输出结果数组1[i] = "“" + Regex.Replace(原始结果数组[i], @"[^\u4e00-\u9fa5a-zA-Z0-9]", "") + "”";
                    }
                    for (int i = 0; i < 纠正结果数组.Length; i++)
                    {
                        输出结果数组2[i] = "“" + 纠正结果数组[i] + "”";
                    }
                    LogTool.Log($"原始结果 1:{输出结果数组1[0],-8}({当前目标数组[0],-6})2:{输出结果数组1[1],-8}({当前目标数组[1],-6})3:{输出结果数组1[2],-8}({当前目标数组[2],-6})4:{输出结果数组1[3],-8}({当前目标数组[3],-6})5:{输出结果数组1[4],-8}({当前目标数组[4],-6})");
                    Debug.WriteLine($"原始结果 1:{输出结果数组1[0],-8}({当前目标数组[0],-6})2:{输出结果数组1[1],-8}({当前目标数组[1],-6})3:{输出结果数组1[2],-8}({当前目标数组[2],-6})4:{输出结果数组1[3],-8}({当前目标数组[3],-6})5:{输出结果数组1[4],-8}({当前目标数组[4],-6})");
                    OutputForm.Instance.WriteLineOutputMessage($"原始结果 1:{输出结果数组1[0],-8}({当前目标数组[0],-6})2:{输出结果数组1[1],-8}({当前目标数组[1],-6})3:{输出结果数组1[2],-8}({当前目标数组[2],-6})4:{输出结果数组1[3],-8}({当前目标数组[3],-6})5:{输出结果数组1[4],-8}({当前目标数组[4],-6})");
                    LogTool.Log($"纠正结果 1:{输出结果数组2[0],-8}({当前目标数组[0],-6})2:{输出结果数组2[1],-8}({当前目标数组[1],-6})3:{输出结果数组2[2],-8}({当前目标数组[2],-6})4:{输出结果数组2[3],-8}({当前目标数组[3],-6})5:{输出结果数组2[4],-8}({当前目标数组[4],-6})");
                    Debug.WriteLine($"纠正结果 1:{输出结果数组2[0],-8}({当前目标数组[0],-6})2:{输出结果数组2[1],-8}({当前目标数组[1],-6})3:{输出结果数组2[2],-8}({当前目标数组[2],-6})4:{输出结果数组2[3],-8}({当前目标数组[3],-6})5:{输出结果数组2[4],-8}({当前目标数组[4],-6})");
                    OutputForm.Instance.WriteLineOutputMessage($"纠正结果 1:{输出结果数组2[0],-8}({当前目标数组[0],-6})2:{输出结果数组2[1],-8}({当前目标数组[1],-6})3:{输出结果数组2[2],-8}({当前目标数组[2],-6})4:{输出结果数组2[3],-8}({当前目标数组[3],-6})5:{输出结果数组2[4],-8}({当前目标数组[4],-6})");

                    #endregion

                    await GetCard(当前目标数组);

                    判断未拿牌并处理();
                    判断未刷新并处理();
                    更新上一轮结果数组与目标数组();

                    if (await 判断是否需要刷新商店并处理())
                    {

                        if (_iappConfigService.CurrentConfig.IsUseCPUForInference)
                        {
                            await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterRefreshStore_CPU);
                        }
                        else if (_iappConfigService.CurrentConfig.IsUseGPUForInference)
                        {
                            await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterRefreshStore_GPU);
                        }
                        else
                        {
                            await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterRefreshStore_CPU);
                        }

                    }
                    本轮是否按下过鼠标 = false;
                }
                catch (OperationCanceledException)
                {
                    // 正常取消
                    break;
                }
                catch (Exception ex)
                {
                    //跳出循环
                    break;
                }
            }
            Debug.WriteLine($"拿牌loop：{!token.IsCancellationRequested},{isGetCard}");
            StopLoop();
            AutoRefreshOff();
        }

        private bool 本轮与上轮商店状态是否相同()
        {
            for (int i = 0; i < 纠正结果数组.Length; i++)
            {
                if (纠正结果数组[i] != 上一轮结果数组[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool 本轮与上轮拿牌状态是否相同()
        {
            for (int i = 0; i < 当前目标数组.Length; i++)
            {
                if (当前目标数组[i] != 上一轮目标数组[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool 本轮与最近一次刷新轮商店状态是否相同()
        {
            for (int i = 0; i < 纠正结果数组.Length; i++)
            {
                if (纠正结果数组[i] != 最近一次刷新轮商店状态[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool 本轮是否存在目标卡()
        {
            for (int i = 0; i < 当前目标数组.Length; i++)
            {
                if (当前目标数组[i])
                {
                    return true;
                }
            }
            return false;
        }

        private bool 本轮是否为空()
        {
            int 空卡槽数 = 0;

            for (int i = 0; i < 纠正结果数组.Length; i++)
            {
                if (纠正结果数组[i] == "")
                {
                    空卡槽数++;
                }
            }
            if (空卡槽数 == 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void 循环前的标志重置()
        {
            未刷新累积次数 = 0;
            未拿牌累积次数 = 0;
            原始结果数组 = ["", "", "", "", ""];
            纠正结果数组 = ["", "", "", "", ""];
            上一轮结果数组 = ["", "", "", "", ""];
            最近一次刷新轮商店状态 = ["", "", "", "", ""];
            上一轮目标数组 = [false, false, false, false, false];
            当前目标数组 = [false, false, false, false, false];
            当前商店刷新状态 = 刷新状态.未开始;
            从上次尝试刷新到目前为止经过的轮次 = 0;
            计时器.Restart();

        }

        private void 更新纠正结果数组(Bitmap[] bitmaps)
        {
            // 定义ErrorImage文件夹路径
            string errorImagePath = Path.Combine(Application.StartupPath, "Logs", "ErrorImages");
            for (int i = 0; i < 原始结果数组.Length; i++)
            {
                纠正结果数组[i] = _iCorrectionService.ConvertToRightResult(原始结果数组[i], out bool isError, out string errorMessage);

                if (!isError)
                {
                    try
                    {
                        if (_iappConfigService.CurrentConfig.IsStopRefreshStoreWhenErrorCharacters)
                        {
                            LogTool.Log("由于识别错误关闭自动刷新！");
                            Debug.WriteLine("由于识别错误关闭自动刷新！");
                            OutputForm.Instance.WriteLineOutputMessage($"由于识别错误关闭自动刷新！" + "\r\n");
                            停止刷新商店();
                        }
                        // 更新UI
                        OutputForm.Instance.WriteLineErrorMessage(errorMessage + "\r\n图片已保存在“根目录/Logs/ErrorImages”中。");
                        // 动态计算文本区域宽度（每个字符约20像素，加上边距）
                        int estimatedCharWidth = 20;
                        int textAreaWidth = (errorMessage.Length - 2) * estimatedCharWidth + 20;
                        // 创建扩展后的位图（原图宽度 + 动态文本区域宽度）
                        int newWidth = bitmaps[i].Width + Math.Max(textAreaWidth, 1);
                        int newHeight = Math.Max(bitmaps[i].Height, 19);
                        // 创建字体和画刷
                        using (Font font = new Font("SimSun-ExtB", 14, FontStyle.Bold))
                        using (Brush brush = new SolidBrush(Color.Red))
                        using (Bitmap extendedBitmap = new Bitmap(newWidth, newHeight))
                        using (Graphics graphics = Graphics.FromImage(extendedBitmap))
                        {
                            // 设置高质量渲染
                            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            //绘制原始图像（左侧）
                            graphics.DrawImage(bitmaps[i], 0, 0);

                            //绘制文本区域背景（右侧）
                            graphics.FillRectangle(Brushes.White, bitmaps[i].Width, 0, textAreaWidth, newHeight);
                            //计算文本区域（右侧）
                            RectangleF textArea = new RectangleF(
                                bitmaps[i].Width, // 从原图右侧开始
                                0, // 顶部边距
                                textAreaWidth, // 宽度（减去边距）
                                newHeight// 高度（减去边距）
                            );
                            //绘制文本                            
                            graphics.DrawString(errorMessage, font, brush, textArea);
                            // 确保ErrorImage文件夹存在
                            if (!Directory.Exists(errorImagePath))
                            {
                                Directory.CreateDirectory(errorImagePath);
                            }
                            // 保存为PNG到ErrorImage文件夹                           
                            string filePath = Path.Combine(errorImagePath, $"{DateTime.Now:MM月dd日HH时mm分ss秒.fff毫秒}_{i + 1}号卡_{errorMessage}.png");
                            extendedBitmap.Save(filePath, ImageFormat.Png);
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if(_iappConfigService.CurrentConfig.IsSaveCapturedImages)
                {
                    string imagePath = Path.Combine(Application.StartupPath, "Logs", "Images");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }
                    // 保存为PNG到ErrorImage文件夹                           
                    string filePath = Path.Combine(imagePath, $"{DateTime.Now:MM月dd日HH时mm分ss秒.fff毫秒}_{i + 1}号卡_{纠正结果数组[i]}.png");
                    bitmaps[i].Save(filePath, ImageFormat.Png);
                }
            }
        }

        private void 更新上一轮结果数组与目标数组()
        {
            for (int i = 0; i < 5; i++)
            {
                上一轮结果数组[i] = 纠正结果数组[i];
                上一轮目标数组[i] = 当前目标数组[i];
            }
        }

        private void 更新最近一次刷新轮商店状态()
        {
            for (int i = 0; i < 最近一次刷新轮商店状态.Length; i++)
            {
                最近一次刷新轮商店状态[i] = 纠正结果数组[i];
            }
        }
        private bool 自动停止拿牌()
        {
            if (未拿牌累积次数 >= _iappConfigService.CurrentConfig.MaxTimesWithoutHeroPurchase && _iappConfigService.CurrentConfig.IsAutomaticStopHeroPurchase)
            {
                LogTool.Log("存在目标卡的情况下，连续数次商店状态和要拿的牌的位置也无变化，可能是金币不足或者备战席已满，将关闭自动拿牌功能！");
                Debug.WriteLine("存在目标卡的情况下，连续数次商店状态和要拿的牌的位置也无变化，可能是金币不足或者备战席已满，将关闭自动拿牌功能！");
                OutputForm.Instance.WriteLineOutputMessage($"存在目标卡的情况下，连续数次商店状态和要拿的牌的位置也无变化，可能是金币不足或者备战席已满，将关闭自动拿牌功能！");
                StopLoop();
                return false;
            }
            return true;
        }


        private void 自动停止刷新商店()
        {
            if (未刷新累积次数 >= _iappConfigService.CurrentConfig.MaxTimesWithoutRefreshStore && _iappConfigService.CurrentConfig.IsAutomaticStopRefreshStore && isRefreshStore)
            {
                LogTool.Log("自动刷新商店功能开启的情况下，连续数次商店状态无变化，可能金币数量不足，无法进行刷新，将关闭自动刷新功能！");
                Debug.WriteLine("自动刷新商店功能开启的情况下，连续数次商店状态无变化，可能金币数量不足，无法进行刷新，将关闭自动刷新功能！");
                OutputForm.Instance.WriteLineOutputMessage($"自动刷新商店功能开启的情况下，连续数次商店状态无变化，可能金币数量不足，无法进行刷新，将关闭自动刷新功能！");
                停止刷新商店();
            }
        }
        private void 停止刷新商店()
        {
            AutoRefreshOff();
        }
        private void 判断未拿牌并处理()
        {
            if (!本轮是否按下过鼠标 && !鼠标左键是否按下)
            {
                if (本轮是否存在目标卡() && 本轮与上轮拿牌状态是否相同() && 本轮与上轮商店状态是否相同())
                {
                    未拿牌累积次数++;
                }
                else
                {
                    未拿牌累积次数 = 0;
                }
            }
            else
            {
                未拿牌累积次数 = 0;
            }
        }

        private void 判断未刷新并处理()
        {
            if (!本轮是否按下过鼠标 && !鼠标左键是否按下)
            {
                if (当前商店刷新状态 == 刷新状态.刷新中)
                {
                    if (本轮与最近一次刷新轮商店状态是否相同())
                    {
                        从上次尝试刷新到目前为止经过的轮次++;
                        LogTool.Log($"发现商店有空或商店状态未变化，从上次尝试刷新到目前为止经过的轮次:{从上次尝试刷新到目前为止经过的轮次}");
                        Debug.WriteLine($"发现商店有空或商店状态未变化，从上次尝试刷新到目前为止经过的轮次:{从上次尝试刷新到目前为止经过的轮次}");
                        OutputForm.Instance.WriteLineOutputMessage($"发现商店有空或商店状态未变化，从上次尝试刷新到目前为止经过的轮次:{从上次尝试刷新到目前为止经过的轮次}");
                        if (从上次尝试刷新到目前为止经过的轮次 >= 未刷新最大回合数 || 计时器.Elapsed.TotalSeconds >= 未刷新最大时间秒数)
                        {
                            LogTool.Log($"轮次达到上限或者时间超时 - 轮次：{从上次尝试刷新到目前为止经过的轮次} - 上次时间:{计时器.Elapsed.TotalSeconds}");
                            Debug.WriteLine($"轮次达到上限或者时间超时 - 轮次：{从上次尝试刷新到目前为止经过的轮次} - 上次时间:{计时器.Elapsed.TotalSeconds}");
                            OutputForm.Instance.WriteLineOutputMessage($"轮次达到上限或者时间超时 - 轮次：{从上次尝试刷新到目前为止经过的轮次} - 上次时间:{计时器.Elapsed.TotalSeconds}");
                            从上次尝试刷新到目前为止经过的轮次 = 0;
                            当前商店刷新状态 = 刷新状态.未开始;
                            未刷新累积次数++;
                        }
                    }
                    else if (本轮是否为空())//上次刷新命令后本轮商店反而为空,可能是用户操作导致商店临时消失，不刷新，不处理。
                    {
                        LogTool.Log($"最近一次刷新商店轮数商店不为空的情况下，本轮商店状态为空。");
                        Debug.WriteLine($"最近一次刷新商店轮数商店不为空的情况下，本轮商店状态为空。");
                        OutputForm.Instance.WriteLineOutputMessage($"最近一次刷新商店轮数商店不为空的情况下，本轮商店状态为空。");
                    }
                    else
                    {
                        从上次尝试刷新到目前为止经过的轮次 = 0;
                        未刷新累积次数 = 0;
                        当前商店刷新状态 = 刷新状态.已结束;
                    }
                }
            }
            else
            {
                从上次尝试刷新到目前为止经过的轮次 = 0;
                未刷新累积次数 = 0;
            }
        }

        private async Task<bool> 判断是否需要刷新商店并处理()
        {
            if (!isRefreshStore)
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     刷新未开启，不刷新");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     刷新未开启，不刷新");
                OutputForm.Instance.WriteLineOutputMessage($"刷新判断前的刷新状态:{当前商店刷新状态}     刷新未开启，不刷新");
                return (false);
            }

            if (本轮是否按下过鼠标 || 鼠标左键是否按下)
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     鼠标左键被按下，本轮不刷新！");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     鼠标左键被按下，本轮不刷新！");
                OutputForm.Instance.WriteLineOutputMessage($"刷新判断前的刷新状态:{当前商店刷新状态}     鼠标左键被按下，本轮不刷新！");
                return (false);
            }

            if (本轮是否存在目标卡())
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     存在目标卡，本轮不刷新！");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     存在目标卡，本轮不刷新！");
                OutputForm.Instance.WriteLineOutputMessage($"刷新判断前的刷新状态:{当前商店刷新状态}     存在目标卡，本轮不刷新！");
                return (false);
            }

            if (当前商店刷新状态 == 刷新状态.刷新中)
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     商店刷新中，本轮不刷新！");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     商店刷新中，本轮不刷新！");
                OutputForm.Instance.WriteLineOutputMessage($"刷新判断前的刷新状态:{当前商店刷新状态}     商店刷新中，本轮不刷新！");
                return (false);
            }

            if (本轮是否为空())
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     商店为空，本轮不刷新！");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     商店为空，本轮不刷新！");
                OutputForm.Instance.WriteLineOutputMessage($"刷新判断前的刷新状态:{当前商店刷新状态}     商店为空，本轮不刷新！");
                return (false);
            }

            LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:刷新");
            Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:刷新");
            OutputForm.Instance.WriteLineOutputMessage($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:刷新");

            更新最近一次刷新轮商店状态();

            // 重置计时器和状态
            计时器.Restart();
            当前商店刷新状态 = 刷新状态.刷新中;

            // 执行刷新操作
            await 刷新商店();
            return (true);
        }

        private async Task 刷新商店()
        {

            if (_iappConfigService.CurrentConfig.IsMouseRefreshStore)
            {
                int X = 0;
                int Y = 0;
                if (_iappConfigService.CurrentConfig.IsUseDynamicCoordinates)
                {
                    X = Random.Shared.Next(_iAutoConfigService.CurrentConfig.RefreshStoreButtonRectangle.X + _iAutoConfigService.CurrentConfig.RefreshStoreButtonRectangle.Width / 5,
                         _iAutoConfigService.CurrentConfig.RefreshStoreButtonRectangle.X + _iAutoConfigService.CurrentConfig.RefreshStoreButtonRectangle.Width * 4 / 5);
                    Y = Random.Shared.Next(_iAutoConfigService.CurrentConfig.RefreshStoreButtonRectangle.Y + _iAutoConfigService.CurrentConfig.RefreshStoreButtonRectangle.Height / 5,
                        _iAutoConfigService.CurrentConfig.RefreshStoreButtonRectangle.Y + _iAutoConfigService.CurrentConfig.RefreshStoreButtonRectangle.Height * 4 / 5);
                }
                else if (_iappConfigService.CurrentConfig.IsUseFixedCoordinates)
                {
                    X = Random.Shared.Next(_iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.X + _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Width / 5,
                        _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.X + _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Width * 4 / 5);
                    Y = Random.Shared.Next(_iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Y + _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Height / 5,
                        _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Y + _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Height * 4 / 5);
                }
                else
                {
                    X = Random.Shared.Next(_iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.X + _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Width / 5,
                        _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.X + _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Width * 4 / 5);
                    Y = Random.Shared.Next(_iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Y + _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Height / 5,
                        _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Y + _iappConfigService.CurrentConfig.RefreshStoreButtonRectangle.Height * 4 / 5);
                }
                //设置鼠标位置
                //km.EncSetPositionSmooth(X, X, 10);
                //await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterOperation);
                //执行点击操作，逐个点击并等待
                //kmBox.EncLeft(true);
                //await Task.Delay(10);
                //kmBox.EncLeft(false);


                MouseControlTool.SetMousePosition(X, Y);
                await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterOperation);
                await ClickOneTime();

            }
            else if (_iappConfigService.CurrentConfig.IsKeyboardRefreshStore)
            {
                KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.RefreshStoreKey);
            }
        }

        /// <summary>
        /// 截取大图并分割成5份小图，返回含有5个元素的Bitmap数组。
        /// </summary>
        /// <returns></returns>
        private Bitmap[] CaptureAndSplit()
        {
            try
            {
                if (_iappConfigService.CurrentConfig.IsUseDynamicCoordinates)
                {
                    Rectangle[] rects = new Rectangle[]
                      {
                         _iAutoConfigService.CurrentConfig.HeroNameScreenshotRectangle_1,
                         _iAutoConfigService.CurrentConfig.HeroNameScreenshotRectangle_2,
                         _iAutoConfigService.CurrentConfig.HeroNameScreenshotRectangle_3,
                         _iAutoConfigService.CurrentConfig.HeroNameScreenshotRectangle_4,
                         _iAutoConfigService.CurrentConfig.HeroNameScreenshotRectangle_5
                     };
                    // 计算包含所有矩形的最小外接矩形(Bounding Box)
                    int minX = rects.Min(r => r.X);
                    int minY = rects.Min(r => r.Y);
                    int maxX = rects.Max(r => r.X + r.Width);
                    int maxY = rects.Max(r => r.Y + r.Height);

                    Rectangle boundingBox = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                    //截取大图
                    using (Bitmap bigImage = ImageProcessingTool.AreaScreenshots(boundingBox))
                    {
                        Bitmap[] bitmaps = new Bitmap[5];

                        for (int i = 0; i < 5; i++)
                        {
                            // 计算每个矩形相对于大图左上角的偏移量
                            int offsetX = rects[i].X - minX;
                            int offsetY = rects[i].Y - minY;

                            bitmaps[i] = ImageProcessingTool.CropBitmap(
                                bigImage,
                                offsetX,
                                offsetY,
                                rects[i].Width,
                                rects[i].Height
                            );
                        }

                        return bitmaps;
                    }
                }
                else
                {

                    Rectangle[] rects = new Rectangle[]
                     {
                         _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_1,
                         _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_2,
                         _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_3,
                         _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_4,
                         _iappConfigService.CurrentConfig.HeroNameScreenshotRectangle_5
                    };
                    // 计算包含所有矩形的最小外接矩形(Bounding Box)
                    int minX = rects.Min(r => r.X);
                    int minY = rects.Min(r => r.Y);
                    int maxX = rects.Max(r => r.X + r.Width);
                    int maxY = rects.Max(r => r.Y + r.Height);

                    Rectangle boundingBox = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                    //截取大图
                    using (Bitmap bigImage = ImageProcessingTool.AreaScreenshots(boundingBox))
                    {
                        Bitmap[] bitmaps = new Bitmap[5];

                        for (int i = 0; i < 5; i++)
                        {
                            // 计算每个矩形相对于大图左上角的偏移量
                            int offsetX = rects[i].X - minX;
                            int offsetY = rects[i].Y - minY;

                            bitmaps[i] = ImageProcessingTool.CropBitmap(
                                bigImage,
                                offsetX,
                                offsetY,
                                rects[i].Width,
                                rects[i].Height
                            );
                        }

                        return bitmaps;
                    }
                }

            }
            catch (Exception ex)
            {
                LogTool.Log($"截图失败: {ex.Message}. 请检查坐标配置。");
                var errorBitmaps = new Bitmap[5];
                for (int i = 0; i < 5; i++)
                {
                    errorBitmaps[i] = new Bitmap(10, 10);
                }
                return errorBitmaps;
            }
        }


        /// <summary>
        /// 传入一个Bitmap数组，异步排队识别后返回一个识别结果数组，含5个元素的String数组。
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <returns></returns>
        private async Task<string[]> RecognizeImages(Bitmap[] bitmaps)
        {
            // 创建识别任务
            Task<string>[] tasks = new Task<string>[bitmaps.Length];
            for (int i = 0; i < bitmaps.Length; i++)
            {
                tasks[i] = _ocrService.RecognizeTextAsync(bitmaps[i]);
            }
            // 等待所有任务完成
            string[] results = await Task.WhenAll(tasks);
            return results;
        }

        /// <summary>
        /// 将纠正后的OCR识别结果与阵容对象内勾选的英雄名作比较，返回含有5个元素的bool数组。
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private bool[] CompareResults(string[] results)
        {
            List<string> selectedHeros = new List<string>();
            foreach (LineUpUnit unit in _ilineUpService.GetCurrentSubLineUp().LineUpUnits)
            {
                string name = unit.HeroName;
                selectedHeros.Add(name);
            }

            bool[] 本轮牌库状态 = new bool[5] { false, false, false, false, false };
            for (int i = 0; i < results.Length; i++)
            {
                foreach (string j in selectedHeros)
                {
                    // 空字符串不允许匹配，使用包含匹配以容忍OCR识别结果带有多余字符的情况
                    if (!string.IsNullOrEmpty(results[i]) && !string.IsNullOrEmpty(j))
                    {
                        if (results[i]==j)
                        {
                            本轮牌库状态[i] = true;
                            
                            break;
                        }
                        else if(!_iappConfigService.CurrentConfig.IsStrictMatching&&results[i].Contains(j))
                        {
                            本轮牌库状态[i] = true;
                            纠正结果数组[i] = j;
                            break;
                        }                        
                    }
                }
            }
            return 本轮牌库状态;
        }

        /// <summary>
        /// 根据一个含有5个元素的bool数组，判断商店的5个槽位是否有要拿的牌，有则拿之。
        /// </summary>
        /// <param name="isGetArray"></param>
        /// <returns></returns>
        private async Task GetCard(bool[] isGetArray)
        {
            Rectangle[] rects = new Rectangle[]
                     {
                         _iappConfigService.CurrentConfig.HighLightRectangle_1,
                         _iappConfigService.CurrentConfig.HighLightRectangle_2,
                         _iappConfigService.CurrentConfig.HighLightRectangle_3,
                         _iappConfigService.CurrentConfig.HighLightRectangle_4,
                         _iappConfigService.CurrentConfig.HighLightRectangle_5
                    };
            Rectangle[] rects_Auto = new Rectangle[]
            {
                _iAutoConfigService.CurrentConfig.HighLightRectangle_1,
                _iAutoConfigService.CurrentConfig.HighLightRectangle_2,
                _iAutoConfigService.CurrentConfig.HighLightRectangle_3,
                _iAutoConfigService.CurrentConfig.HighLightRectangle_4,
                _iAutoConfigService.CurrentConfig.HighLightRectangle_5
            };
            for (int i = 0; i < isGetArray.Length; i++)
            {
                if (isGetArray[i])
                {
                    if (_iappConfigService.CurrentConfig.IsKeyboardHeroPurchase)
                    {

                        switch (i)
                        {
                            case 0:
                                KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.HeroPurchaseKey1);
                                break;
                            case 1:
                                KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.HeroPurchaseKey2);
                                break;
                            case 2:
                                KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.HeroPurchaseKey3);
                                break;
                            case 3:
                                KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.HeroPurchaseKey4);
                                break;
                            case 4:
                                KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.HeroPurchaseKey5);
                                break;
                        }

                        await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterOperation);

                    }
                    else if (_iappConfigService.CurrentConfig.IsMouseHeroPurchase)
                    {
                        int randomX, randomY;
                        if (_iappConfigService.CurrentConfig.IsUseDynamicCoordinates)
                        {
                            randomX = Random.Shared.Next(rects_Auto[i].Left + rects_Auto[i].Width / 3, rects_Auto[i].Left + rects_Auto[i].Width * 2 / 3);
                            randomY = Random.Shared.Next(rects_Auto[i].Top + rects_Auto[i].Height / 3, rects_Auto[i].Top + rects_Auto[i].Height * 2 / 3);
                        }
                        else if (_iappConfigService.CurrentConfig.IsUseFixedCoordinates)
                        {
                            randomX = Random.Shared.Next(rects[i].Left + rects[i].Width / 3, rects[i].Left + rects[i].Width * 2 / 3);
                            randomY = Random.Shared.Next(rects[i].Top + rects[i].Height / 3, rects[i].Top + rects[i].Height * 2 / 3);
                        }
                        else
                        {
                            randomX = Random.Shared.Next(rects[i].Left + rects[i].Width / 3, rects[i].Left + rects[i].Width * 2 / 3);
                            randomY = Random.Shared.Next(rects[i].Top + rects[i].Height / 3, rects[i].Top + rects[i].Height * 2 / 3);
                        }
                        //// 设置鼠标位置                       
                        //km.EncSetPositionSmooth(randomX, randomY,10);
                        //await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterOperation);
                        //// 执行点击操作，逐个点击并等待
                        //kmBox.EncLeft(true);
                        //await Task.Delay(10);
                        //kmBox.EncLeft(false);
                        //await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterOperation);

                        //设置鼠标位置
                        MouseControlTool.SetMousePosition(randomX, randomY);
                        await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterOperation);
                        await ClickOneTime();
                        await Task.Delay(_iappConfigService.CurrentConfig.DelayAfterOperation);

                    }
                }
            }
        }

        /// <summary>
        /// 点击1次
        /// </summary>
        /// <returns></returns>
        private async Task ClickOneTime()
        {
            MouseHookTool.IncrementProgramClickCount(); // 增加计数
            MouseControlTool.MakeMouseLeftButtonDown();
            MouseControlTool.MakeMouseLeftButtonUp();
            // 延迟后减少计数器
            await Task.Delay(1);

            MouseHookTool.DecrementProgramClickCount(); // 减少计数

        }

        /// <summary>
        /// 计算5个卡槽的矩形区域（用于高亮显示）
        /// </summary>
        /// <returns>包含5个Rectangle的数组</returns>
        private Rectangle[] CalculateCardRectangles()
        {
            Rectangle[] rectangles = new Rectangle[5];

            if (_iappConfigService.CurrentConfig.IsUseDynamicCoordinates)
            {
                rectangles = new Rectangle[]
                    {
                         _iAutoConfigService.CurrentConfig.HighLightRectangle_1,
                         _iAutoConfigService.CurrentConfig.HighLightRectangle_2,
                         _iAutoConfigService.CurrentConfig.HighLightRectangle_3,
                         _iAutoConfigService.CurrentConfig.HighLightRectangle_4,
                         _iAutoConfigService.CurrentConfig.HighLightRectangle_5
                   };
            }
            else
            {
                rectangles = new Rectangle[]
                 {
                         _iappConfigService.CurrentConfig.HighLightRectangle_1,
                         _iappConfigService.CurrentConfig.HighLightRectangle_2,
                         _iappConfigService.CurrentConfig.HighLightRectangle_3,
                         _iappConfigService.CurrentConfig.HighLightRectangle_4,
                         _iappConfigService.CurrentConfig.HighLightRectangle_5
                };
            }

            return rectangles;
        }
    }
}
