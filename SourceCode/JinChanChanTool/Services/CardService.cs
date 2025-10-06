using JinChanChanTool.Forms;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Tools;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.MouseTools;
using System.Diagnostics;
using System.Drawing.Imaging;
using JinChanChanTool.DataClass;


namespace JinChanChanTool.Services
{
    public class CardService
    {
        /// <summary>
        /// 程序设置服务实例
        /// </summary>
        private readonly IAppConfigService _iappConfigService;

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

        private readonly AutomationService _automationService;

        private readonly Button _button1;

        private readonly Button _button2;

        private QueuedOCRService _ocrService;

        private bool is自动拿牌 = false;//是否开启 自动拿牌 标志(初始false)
        private bool is自动刷新商店 = false;//是否开启 自动刷新商店 标志(初始false)               
        private CancellationTokenSource cts = new CancellationTokenSource();//控制拿牌D牌多线程操作的变量

        private bool 鼠标左键是否按下;
        private bool 本轮是否按下过鼠标;

        private int 未拿牌累积次数 = 0;

        private string[] 原始结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 纠正结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 上一轮结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 最近一次刷新轮商店状态 = new string[5] { "", "", "", "", "" };
        private bool[] 上一轮目标数组 = new bool[5] { false, false, false, false, false };
        private bool[] 当前目标数组 = new bool[5] { false, false, false, false, false };

        private const int 最大未拿牌次数CPU = 3;
        private const int 最大未刷新次数CPU = 3;
        private const int 未刷新最大回合数CPU = 5;
        private const double 未刷新最大时间秒数CPU = 2.0;
        private const int 操作间隔时间CPU = 20;
        private const int 最大未拿牌次数GPU = 3;
        private const int 最大未刷新次数GPU = 3;
        private const int 未刷新最大回合数GPU = 6;
        private const double 未刷新最大时间秒数GPU = 2.0;
        private const int 操作间隔时间GPU = 40;
        private const int 循环间隔时间 = 1;
        

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

        public CardService(Button button1, Button button2, IAppConfigService iAppConfigService, ICorrectionService iCorrectionService, IHeroDataService iHeroDataService, ILineUpService iLineUpService, AutomationService automationService)
        {

            _button1 = button1;
            _button2 = button2;
            _iappConfigService = iAppConfigService;
            _iCorrectionService = iCorrectionService;
            _iheroDataService = iHeroDataService;
            _ilineUpService = iLineUpService;
            _automationService = automationService;
            // 根据选中的按钮初始化OCR
            if (iAppConfigService.CurrentConfig.UseCPU)
            {
                InitializeOcrService(QueuedOCRService.设备.CPU);
            }
            else if (iAppConfigService.CurrentConfig.UseGPU)
            {
                InitializeOcrService(QueuedOCRService.设备.GPU);
            }
            else
            {
                InitializeOcrService(QueuedOCRService.设备.CPU);
            }
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
        /// 开始OCR循环
        /// </summary>
        public void StartLoop()
        {
            is自动拿牌 = true;
            cts = new CancellationTokenSource();
            // 启动循环任务
            Task.Run(() => ProcessLoop(cts.Token), cts.Token);
        }

        /// <summary>
        /// 停止OCR循环
        /// </summary>
        public void StopLoop()
        {
            is自动拿牌 = false;
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        /// <summary>
        /// 开启自动刷新商店
        /// </summary>
        public  void AutoRefreshOn()
        {
            is自动刷新商店 = true;
            未刷新累积次数 = 0;
        }

        /// <summary>
        /// 关闭自动刷新商店
        /// </summary>
        public void AutoRefreshOff()
        {
            is自动刷新商店 = false;          
        }

        public void MouseLeftButtonDown()
        {
            鼠标左键是否按下 = true;
            本轮是否按下过鼠标 = true;
        }

        public void MouseLeftButtonUp()
        {
            鼠标左键是否按下 = false;
        }
            
        private async Task ProcessLoop(CancellationToken token)
        {
            int 循环计数 = 0;
            循环前的标志重置();
            while (is自动拿牌 && !token.IsCancellationRequested)
            {                
                try
                {
                    循环计数++;
                    

                    LogTool.Log($"轮次:{循环计数}     未刷新累积次数：{未刷新累积次数}     未拿牌累积次数：{未拿牌累积次数}");
                    Debug.WriteLine($"轮次:{循环计数}     未刷新累积次数：{未刷新累积次数}     未拿牌累积次数：{未拿牌累积次数}");
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
                    LogTool.Log($"原始结果 1:{原始结果数组[0],-8}({当前目标数组[0],-6})2:{原始结果数组[1],-8}({当前目标数组[1],-6})3:{原始结果数组[2],-8}({当前目标数组[2],-6})4:{原始结果数组[3],-8}({当前目标数组[3],-6})5:{原始结果数组[4],-8}({当前目标数组[4],-6})");
                    Debug.WriteLine($"原始结果 1:{原始结果数组[0],-8}({当前目标数组[0],-6})2:{原始结果数组[1],-8}({当前目标数组[1],-6})3:{原始结果数组[2],-8}({当前目标数组[2],-6})4:{原始结果数组[3],-8}({当前目标数组[3],-6})5:{原始结果数组[4],-8}({当前目标数组[4],-6})");
                    LogTool.Log($"纠正结果 1:{纠正结果数组[0],-8}({当前目标数组[0],-6})2:{纠正结果数组[1],-8}({当前目标数组[1],-6})3:{纠正结果数组[2],-8}({当前目标数组[2],-6})4:{纠正结果数组[3],-8}({当前目标数组[3],-6})5:{纠正结果数组[4],-8}({当前目标数组[4],-6})");
                    Debug.WriteLine($"纠正结果 1:{纠正结果数组[0],-8}({当前目标数组[0],-6})2:{纠正结果数组[1],-8}({当前目标数组[1],-6})3:{纠正结果数组[2],-8}({当前目标数组[2],-6})4:{纠正结果数组[3],-8}({当前目标数组[3],-6})5:{纠正结果数组[4],-8}({当前目标数组[4],-6})");
                    await GetCard(当前目标数组);
                    判断未拿牌并处理();                     
                    判断未刷新并处理();
                    更新上一轮结果数组与目标数组();
                    await 判断是否需要刷新商店并处理();
                    本轮是否按下过鼠标 = false;
                    await Task.Delay(循环间隔时间);//循环间隔
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
            {
                for (int i = 0; i < 原始结果数组.Length; i++)
                {
                    纠正结果数组[i] = _iCorrectionService.ConvertToRightResult(原始结果数组[i], out bool isError, out string errorMessage);

                    if (!isError)
                    {
                        try
                        {
                            停止刷新商店();
                            LogTool.Log("由于识别错误关闭自动刷新！");
                            Debug.WriteLine("由于识别错误关闭自动刷新！");
                            // 更新UI
                            ErrorForm.Instance.GetTextBox().Invoke((MethodInvoker)delegate
                            {
                                ErrorForm.Instance.GetTextBox().AppendText("\r\n" + errorMessage + "\r\n图片已保存在“根目录/Logs”中。");
                            });
                            // 创建扩展后的位图（原图宽度 + 300像素文本区域）
                            int textAreaWidth = bitmaps[i].Width; // 固定文本区域宽度
                            int newWidth = bitmaps[i].Width + textAreaWidth;
                            int newHeight = bitmaps[i].Height;
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
                                    1, // 顶部边距
                                    textAreaWidth-1, // 宽度（减去边距）
                                    newHeight-1 // 高度（减去边距）
                                );
                                //绘制文本
                                graphics.DrawString(errorMessage, font, brush, textArea);
                                // 保存为PNG
                                extendedBitmap.Save(Path.Combine(Application.StartupPath, "Logs", $"{DateTime.Now:HH_mm_ss.fff}_{i + 1}号卡.png"), ImageFormat.Png);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
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
            int 最大未拿牌次数 = 0;
            if(_iappConfigService.CurrentConfig.UseCPU)
            {
                最大未拿牌次数 = 最大未拿牌次数CPU;
            }
            else if(_iappConfigService.CurrentConfig.UseGPU)
            {
                最大未拿牌次数 = 最大未拿牌次数GPU;
            }
            else
            {
                最大未拿牌次数 = 最大未拿牌次数CPU;
            }
            if (未拿牌累积次数 >= 最大未拿牌次数 && _iappConfigService.CurrentConfig.AutoStopGet)
            {
                LogTool.Log("存在目标卡的情况下，连续数次商店状态和要拿的牌的位置也无变化，可能是金币不足或者备战席已满，将关闭自动拿牌功能！");
                Debug.WriteLine("存在目标卡的情况下，连续数次商店状态和要拿的牌的位置也无变化，可能是金币不足或者备战席已满，将关闭自动拿牌功能！");
                is自动拿牌 = false;
                _button1.Invoke((MethodInvoker)delegate
                {                    
                    _button1.Text = "启动";                                      
                });
                cts.Cancel();
                return false;
            }
            return true;
        }

        
        private void 自动停止刷新商店()
        {
            int 最大未刷新次数 = 0;
            if (_iappConfigService.CurrentConfig.UseCPU)
            {
                最大未刷新次数 = 最大未刷新次数CPU;
            }
            else if (_iappConfigService.CurrentConfig.UseGPU)
            {
                最大未刷新次数 = 最大未刷新次数GPU;
            }
            else
            {
                最大未刷新次数 = 最大未刷新次数CPU;
            }
            if (未刷新累积次数 >= 最大未刷新次数 && _iappConfigService.CurrentConfig.AutoStopRefresh)
            {
                LogTool.Log("自动刷新商店功能开启的情况下，连续数次商店状态无变化，可能金币数量不足，无法进行刷新，将关闭自动刷新功能！");
                Debug.WriteLine("自动刷新商店功能开启的情况下，连续数次商店状态无变化，可能金币数量不足，无法进行刷新，将关闭自动刷新功能！");
                停止刷新商店();
            }
        }
        private void 停止刷新商店()
        {
            is自动刷新商店 = false;
            _button2.Invoke((MethodInvoker)delegate
            {
                _button2.Text = "启动";
               
            });
        }
        private void  判断未拿牌并处理()
        {
            if(!本轮是否按下过鼠标 && !鼠标左键是否按下)
            {
                if(本轮是否存在目标卡()&& 本轮与上轮拿牌状态是否相同() &&本轮与上轮商店状态是否相同())
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
                if(当前商店刷新状态 == 刷新状态.刷新中)
                {
                    if(本轮与最近一次刷新轮商店状态是否相同())
                    {
                        从上次尝试刷新到目前为止经过的轮次++;
                        LogTool.Log($"发现商店有空或商店状态未变化，从上次尝试刷新到目前为止经过的轮次:{从上次尝试刷新到目前为止经过的轮次}");
                        Debug.WriteLine($"发现商店有空或商店状态未变化，从上次尝试刷新到目前为止经过的轮次:{从上次尝试刷新到目前为止经过的轮次}");
                        int 未刷新最大回合数 = 0;
                        double 未刷新最大时间秒数 = 0.0;
                        if (_iappConfigService.CurrentConfig.UseCPU)
                        {
                            未刷新最大回合数 = 未刷新最大回合数CPU;
                            未刷新最大时间秒数 = 未刷新最大时间秒数CPU;
                        }
                        else if(_iappConfigService.CurrentConfig.UseGPU)
                        {
                            未刷新最大回合数 = 未刷新最大回合数GPU;
                            未刷新最大时间秒数 = 未刷新最大时间秒数GPU;
                        }
                        else
                        {
                            未刷新最大回合数 = 未刷新最大回合数CPU;
                            未刷新最大时间秒数 = 未刷新最大时间秒数CPU;
                        }
                        if (从上次尝试刷新到目前为止经过的轮次 >= 未刷新最大回合数 || 计时器.Elapsed.TotalSeconds >= 未刷新最大时间秒数)
                        {
                            LogTool.Log($"轮次达到上限或者时间超时 - 轮次：{从上次尝试刷新到目前为止经过的轮次} - 上次时间:{计时器.Elapsed.TotalSeconds}");
                            Debug.WriteLine($"轮次达到上限或者时间超时 - 轮次：{从上次尝试刷新到目前为止经过的轮次} - 上次时间:{计时器.Elapsed.TotalSeconds}");
                            从上次尝试刷新到目前为止经过的轮次 = 0;
                            当前商店刷新状态 = 刷新状态.未开始;
                            未刷新累积次数++;
                        }
                    }
                    else if(本轮是否为空())//上次刷新命令后本轮商店反而为空,可能是用户操作导致商店临时消失，不刷新，不处理。
                    {
                        LogTool.Log($"最近一次刷新商店轮数商店不为空的情况下，本轮商店状态为空。");
                        Debug.WriteLine($"最近一次刷新商店轮数商店不为空的情况下，本轮商店状态为空。");
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

        private async Task 判断是否需要刷新商店并处理()
        {
            if (!is自动刷新商店)
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     刷新未开启，不刷新");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     刷新未开启，不刷新");
                return;
            }

            if (本轮是否按下过鼠标 || 鼠标左键是否按下)
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     鼠标左键被按下，本轮不刷新！");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     鼠标左键被按下，本轮不刷新！");
                return;
            }

            if (本轮是否存在目标卡())
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     存在目标卡，本轮不刷新！");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     存在目标卡，本轮不刷新！");
                return;
            }

            if (当前商店刷新状态 == 刷新状态.刷新中)
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     商店刷新中，本轮不刷新！");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     商店刷新中，本轮不刷新！");
                return;
            }

            if (本轮是否为空())
            {
                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     商店为空，本轮不刷新！");
                Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     商店为空，本轮不刷新！");
                return;
            }

            LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:刷新");
            Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:刷新");

            更新最近一次刷新轮商店状态();

            // 重置计时器和状态
            计时器.Restart();
            当前商店刷新状态 = 刷新状态.刷新中;

            // 执行刷新操作
            await 刷新商店();
        }

        //private async Task 刷新商店()
        //{
        //    if (_iappConfigService.CurrentConfig.MouseRefresh)
        //    {
        //        MouseControlTool.SetMousePosition(_iappConfigService.CurrentConfig.Point_RefreshStoreX, _iappConfigService.CurrentConfig.Point_RefreshStoreY);
        //        if(_iappConfigService.CurrentConfig.UseCPU)
        //        {
        //            await Task.Delay(操作间隔时间CPU);
        //            await ClickOneTime();
        //            await Task.Delay(操作间隔时间CPU);
        //        }
        //        else if(_iappConfigService.CurrentConfig.UseGPU)
        //        {
        //            await Task.Delay(操作间隔时间GPU);
        //            await ClickOneTime();
        //            await Task.Delay(操作间隔时间GPU);
        //        }
        //    }
        //    else if (_iappConfigService.CurrentConfig.KeyboardRefresh)
        //    {
        //        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.RefreshKey);
        //        await Task.Delay(操作间隔时间CPU);
        //    }
        //}

        private async Task 刷新商店()
        {
            Rectangle? rect = _automationService.GetTargetRectangle(UiElement.RefreshButton);
            if (!rect.HasValue) return; // 如果找不到按钮坐标，则不执行任何操作

            // 计算按钮中心点
            int centerX = rect.Value.X + rect.Value.Width / 2;
            int centerY = rect.Value.Y + rect.Value.Height / 2;

            if (_iappConfigService.CurrentConfig.MouseRefresh)
            {
                MouseControlTool.SetMousePosition(centerX, centerY);
                if (_iappConfigService.CurrentConfig.UseCPU)
                {
                    await Task.Delay(操作间隔时间CPU);
                    await ClickOneTime();
                    await Task.Delay(操作间隔时间CPU);
                }
                else if (_iappConfigService.CurrentConfig.UseGPU)
                {
                    await Task.Delay(操作间隔时间GPU);
                    await ClickOneTime();
                    await Task.Delay(操作间隔时间GPU);
                }
            }
            else if (_iappConfigService.CurrentConfig.KeyboardRefresh)
            {
                KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.RefreshKey);
                await Task.Delay(操作间隔时间CPU);
            }
        }

        /// <summary>
        /// 截取大图并分割成5份小图，返回含有5个元素的Bitmap数组。
        /// </summary>
        /// <returns></returns>
        //private Bitmap[] CaptureAndSplit()
        //{
        //    // 截取大图
        //    using Bitmap bigImage = ImageProcessingTool.AreaScreenshots(_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1, _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1) + _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);

        //    // 分割成5个小图
        //    Bitmap[] bitmaps = new Bitmap[5];
        //    bitmaps[0] = ImageProcessingTool.CropBitmap(bigImage, 0, 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
        //    bitmaps[1] = ImageProcessingTool.CropBitmap(bigImage, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1), 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
        //    bitmaps[2] = ImageProcessingTool.CropBitmap(bigImage, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1), 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
        //    bitmaps[3] = ImageProcessingTool.CropBitmap(bigImage, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1), 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
        //    bitmaps[4] = ImageProcessingTool.CropBitmap(bigImage, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1), 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
        //    return bitmaps;
        //}

        private Bitmap[] CaptureAndSplit()
        {
            Bitmap[] bitmaps = new Bitmap[5];
            UiElement[] nameSlots = {
        UiElement.CardSlot1_Name, UiElement.CardSlot2_Name, UiElement.CardSlot3_Name,
        UiElement.CardSlot4_Name, UiElement.CardSlot5_Name
    };

            for (int i = 0; i < 5; i++)
            {
                Rectangle? rect = _automationService.GetTargetRectangle(nameSlots[i]);
                if (rect.HasValue)
                {
                    bitmaps[i] = ImageProcessingTool.AreaScreenshots(rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height);
                }
                else
                {
                    // 如果获取坐标失败（例如游戏窗口突然关闭），创建一个空的bitmap以避免后续代码崩溃
                    bitmaps[i] = new Bitmap(10, 10);
                }
            }
            return bitmaps;
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
            List<string> selectedHeros = _ilineUpService.GetCurrentSubLineUp();
            bool[] 本轮牌库状态 = new bool[5] { false,false,false,false,false};
            for(int i =0;i<results.Length;i++)
            {                  
                foreach(string j in selectedHeros)
                {
                    if (results[i] ==j)
                    {
                        本轮牌库状态[i] = true;                        
                        break;
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
        //private async Task GetCard(bool[] isGetArray)
        //{
        //    for(int i = 0;i< isGetArray.Length;i++)
        //    {
        //        int x = 0;
        //        switch(i)
        //        {
        //            case 0:
        //                x = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1;
        //                break;
        //            case 1:
        //                x = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2;
        //                break;
        //            case 2:
        //                x = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3;
        //                break;
        //            case 3:
        //                x = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4;
        //                break;
        //            case 4:
        //                x = _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5;
        //                break;
        //        }
        //        if (isGetArray[i])
        //        {                    
        //            if (_iappConfigService.CurrentConfig.MouseGetCard)
        //            {
        //                // 鼠标操作
        //                MouseControlTool.SetMousePosition(x + _iappConfigService.CurrentConfig.Width_CardScreenshot / 2, _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY - _iappConfigService.CurrentConfig.Height_CardScreenshot * 2);
        //                if(_iappConfigService.CurrentConfig.UseCPU)
        //                {
        //                    await Task.Delay(操作间隔时间CPU);
        //                    // 执行点击操作，逐个点击并等待
        //                    await ClickOneTime();
        //                    await Task.Delay(操作间隔时间CPU);
        //                }
        //                else if(_iappConfigService.CurrentConfig.UseGPU)
        //                {
        //                    await Task.Delay(操作间隔时间GPU);
        //                    // 执行点击操作，逐个点击并等待
        //                    await ClickOneTime();
        //                    await Task.Delay(操作间隔时间GPU);
        //                }
        //            }
        //            else if (_iappConfigService.CurrentConfig.KeyboardGetCard)
        //            {
        //                switch (i)
        //                {
        //                    case 0:
        //                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey1);
        //                        break;
        //                    case 1:
        //                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey2);
        //                        break;
        //                    case 2:
        //                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey3);
        //                        break;
        //                    case 3:
        //                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey4);
        //                        break;
        //                    case 4:
        //                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey5);
        //                        break;
        //                }
        //                if (_iappConfigService.CurrentConfig.UseCPU)
        //                {
        //                    await Task.Delay(操作间隔时间CPU);
        //                }
        //                else if (_iappConfigService.CurrentConfig.UseGPU)
        //                {
        //                    await Task.Delay(操作间隔时间GPU);                           
        //                }                        
        //            }
        //        }
        //    }
        //}

        private async Task GetCard(bool[] isGetArray)
        {
            UiElement[] clickSlots = {
        UiElement.CardSlot1_Click, UiElement.CardSlot2_Click, UiElement.CardSlot3_Click,
        UiElement.CardSlot4_Click, UiElement.CardSlot5_Click
    };

            for (int i = 0; i < isGetArray.Length; i++)
            {
                if (isGetArray[i])
                {
                    if (_iappConfigService.CurrentConfig.MouseGetCard)
                    {
                        Rectangle? rect = _automationService.GetTargetRectangle(clickSlots[i]);
                        if (!rect.HasValue) continue; // 如果找不到卡槽，跳过本次拿牌

                        // 计算卡槽中心点
                        int centerX = rect.Value.X + rect.Value.Width / 2;
                        int centerY = rect.Value.Y + rect.Value.Height / 2;

                        MouseControlTool.SetMousePosition(centerX, centerY);
                        if (_iappConfigService.CurrentConfig.UseCPU)
                        {
                            await Task.Delay(操作间隔时间CPU);
                            await ClickOneTime();
                            await Task.Delay(操作间隔时间CPU);
                        }
                        else if (_iappConfigService.CurrentConfig.UseGPU)
                        {
                            await Task.Delay(操作间隔时间GPU);
                            await ClickOneTime();
                            await Task.Delay(操作间隔时间GPU);
                        }
                    }
                    else if (_iappConfigService.CurrentConfig.KeyboardGetCard)
                    {
                        // 键盘操作逻辑保持不变
                        switch (i)
                        {
                            case 0: KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey1); break;
                            case 1: KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey2); break;
                            case 2: KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey3); break;
                            case 3: KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey4); break;
                            case 4: KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey5); break;
                        }
                        if (_iappConfigService.CurrentConfig.UseCPU)
                        {
                            await Task.Delay(操作间隔时间CPU);
                        }
                        else if (_iappConfigService.CurrentConfig.UseGPU)
                        {
                            await Task.Delay(操作间隔时间GPU);
                        }
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
    }
}
