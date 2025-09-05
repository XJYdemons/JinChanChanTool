using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Tools;
using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.MouseTools;
using JinChanChanTool.Tools.OCRTools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.Services
{
    public class CardService
    {
        private GPU_OCRObject _ocrObject;
        private bool is自动拿牌 = false;//是否开启 自动拿牌 标志(初始false)
        private bool is自动刷新商店 = false;//是否开启 自动刷新商店 标志(初始false)       
        private bool isTheAsynRunning = false; //“自动拿牌异步任务”是否运行的标志(初始false)
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();//控制拿牌D牌多线程操作的变量

        private bool 鼠标左键是否按下;
        private bool 本轮是否按下过鼠标;

        private bool 本轮是否存在目标卡 = false;
        private int 未拿牌累积次数 = 0;
        private int 未刷新累积次数 = 0;

        private string[] 原始结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 上一次结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 纠正结果数组 = new string[5] { "", "", "", "", "" };
        private string[] 最近一次刷新轮商店状态 = new string[5] { "", "", "", "", "" };
        private bool[] lastOrderCard = new bool[5] { false, false, false, false, false };
        private bool[] currentOrderCard = new bool[5] { false, false, false, false, false };

        private const int 最大未拿牌次数 = 3;
        private const int 最大未刷新次数 = 3;
        private int 操作间隔时间 = 20;

        enum 刷新状态
        {
            未开始,
            刷新中,
            已结束
        }

        刷新状态 当前商店刷新状态 = 刷新状态.未开始;

        private int 从上次尝试刷新到目前为止经过的轮次 = 0;
        private Stopwatch 计时器 = Stopwatch.StartNew();
        private int 循环计数 = 0;
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
        
        private readonly Button _button1;
        private readonly Button _button2;
        public CardService(Button button1, Button button2, IAppConfigService iAppConfigService,ICorrectionService iCorrectionService,IHeroDataService iHeroDataService,ILineUpService iLineUpService)
        {
           
            _button1 = button1;
            _button2 = button2;
            _iappConfigService = iAppConfigService;
            _iCorrectionService = iCorrectionService;              
            _iheroDataService = iHeroDataService;
            _ilineUpService = iLineUpService;
            _ocrObject = new GPU_OCRObject(4);
        }
      
       public  async void AutoGetCard()
        {
            is自动拿牌 = !is自动拿牌;
            // 如果已经在运行，直接返回，避免重复启动任务
            if (isTheAsynRunning)
            {
                cancellationTokenSource.Cancel(); // 停止当前任务
                isTheAsynRunning = false; // 重置状态
                return;
            }
            // 立即重新创建一个新的 CancellationTokenSource
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            isTheAsynRunning = true;

            未刷新累积次数 = 0;
            未拿牌累积次数 = 0;
            上一次结果数组 = ["", "", "", "", ""];
            纠正结果数组 = ["", "", "", "", ""];
            原始结果数组 = ["", "", "", "", ""];
            最近一次刷新轮商店状态 = ["", "", "", "", ""];
            lastOrderCard = [false, false, false, false, false];
            currentOrderCard = [false, false, false, false, false];
            当前商店刷新状态 = 刷新状态.未开始;
            从上次尝试刷新到目前为止经过的轮次 = 0;
            循环计数 = 0;
            计时器.Restart();
            本轮是否按下过鼠标 = false;
            if (is自动拿牌)
            {
                // 启动一个新的后台任务来执行循环操作
                await Task.Run(async () =>
                {

                    while (!token.IsCancellationRequested)
                    {                       
                        本轮是否按下过鼠标 = false;
                        循环计数++;
                        本轮是否存在目标卡 = false;
                        纠正结果数组 = ["", "", "", "", ""];
                        原始结果数组 = ["", "", "", "", ""];
                        LogTool.Log($"轮次:{循环计数}     未刷新累积次数：{未刷新累积次数}     未拿牌累积次数：{未拿牌累积次数}");
                        //Debug.WriteLine($"轮次:{循环计数}     未刷新累积次数：{未刷新累积次数}     未拿牌累积次数：{未拿牌累积次数}");
                        // 检查商店未拿牌累积次数
                        if (未拿牌累积次数 >= 最大未拿牌次数 && _iappConfigService.CurrentConfig.AutoStopGet)
                        {
                            LogTool.Log("存在目标卡的情况下，连续数次商店状态和要拿的牌的位置也无变化，可能是金币不足或者备战席已满，将关闭自动拿牌功能！");
                            //Debug.WriteLine("存在目标卡的情况下，连续数次商店状态和要拿的牌的位置也无变化，可能是金币不足或者备战席已满，将关闭自动拿牌功能！");
                            _button1.Invoke((MethodInvoker)delegate
                            {
                                is自动拿牌 = false;
                                _button1.Text = "启动";
                                cancellationTokenSource.Cancel();
                                isTheAsynRunning = false;
                            });
                            return;
                        }
                        // 检查商店未刷新累积次数
                        if (未刷新累积次数 >= 最大未刷新次数 && _iappConfigService.CurrentConfig.AutoStopRefresh)
                        {
                            LogTool.Log("自动刷新商店功能开启的情况下，连续数次商店状态无变化，可能金币数量不足，无法进行刷新，将关闭自动刷新功能！");
                            //Debug.WriteLine("自动刷新商店功能开启的情况下，连续数次商店状态无变化，可能金币数量不足，无法进行刷新，将关闭自动刷新功能！");
                            is自动刷新商店 = false;
                            _button2.Invoke((MethodInvoker)delegate
                            {
                                _button2.Text = "启动";
                            });
                            未刷新累积次数 = 0;
                        }
                        Bitmap[] bitmaps = new Bitmap[5];
                        Bitmap bigImage = ImageProcessingTool.AreaScreenshots(_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1, _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1) + _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
                        bitmaps[0] = ImageProcessingTool.CropBitmap(bigImage, 0, 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
                        bitmaps[1] = ImageProcessingTool.CropBitmap(bigImage, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1), 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
                        bitmaps[2] = ImageProcessingTool.CropBitmap(bigImage, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1), 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
                        bitmaps[3] = ImageProcessingTool.CropBitmap(bigImage, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1), 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
                        bitmaps[4] = ImageProcessingTool.CropBitmap(bigImage, (_iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5 - _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1), 0, _iappConfigService.CurrentConfig.Width_CardScreenshot, _iappConfigService.CurrentConfig.Height_CardScreenshot);
                        bigImage.Dispose();
                        // 逐个执行每个卡片的任务
                        currentOrderCard[0] = await OCR_Compare_GetCard(bitmaps[0], _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX1, 1);
                        currentOrderCard[1] = await OCR_Compare_GetCard(bitmaps[1], _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX2, 2);
                        currentOrderCard[2] = await OCR_Compare_GetCard(bitmaps[2], _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX3, 3);
                        currentOrderCard[3] = await OCR_Compare_GetCard(bitmaps[3], _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX4, 4);
                        currentOrderCard[4] = await OCR_Compare_GetCard(bitmaps[4], _iappConfigService.CurrentConfig.StartPoint_CardScreenshotX5, 5);
                        for (int j = 0; j < bitmaps.Length; j++)
                        {
                            if (bitmaps[j] != null)
                            {
                                bitmaps[j].Dispose();
                                bitmaps[j] = null; // 避免重复释放
                            }
                        }

                        LogTool.Log($"原始结果 1:{原始结果数组[0],-8}({currentOrderCard[0],-6})2:{原始结果数组[1],-8}({currentOrderCard[1],-6})3:{原始结果数组[2],-8}({currentOrderCard[2],-6})4:{原始结果数组[3],-8}({currentOrderCard[3],-6})5:{原始结果数组[4],-8}({currentOrderCard[4],-6})");
                        //Debug.WriteLine($"原始结果 1:{noRightCardResult[0],-8}({currentOrderCard[0],-6})2:{noRightCardResult[1],-8}({currentOrderCard[1],-6})3:{noRightCardResult[2],-8}({currentOrderCard[2],-6})4:{noRightCardResult[3],-8}({currentOrderCard[3],-6})5:{noRightCardResult[4],-8}({currentOrderCard[4],-6})");
                        LogTool.Log($"纠正结果 1:{纠正结果数组[0],-8}({currentOrderCard[0],-6})2:{纠正结果数组[1],-8}({currentOrderCard[1],-6})3:{纠正结果数组[2],-8}({currentOrderCard[2],-6})4:{纠正结果数组[3],-8}({currentOrderCard[3],-6})5:{纠正结果数组[4],-8}({currentOrderCard[4],-6})");
                        //Debug.WriteLine($"纠正结果 1:{currentCardResult[0],-8}({currentOrderCard[0],-6})2:{currentCardResult[1],-8}({currentOrderCard[1],-6})3:{currentCardResult[2],-8}({currentOrderCard[2],-6})4:{currentCardResult[3],-8}({currentOrderCard[3],-6})5:{currentCardResult[4],-8}({currentOrderCard[4],-6})");
                        if ((!本轮是否按下过鼠标) && (!鼠标左键是否按下) && 本轮是否存在目标卡 && 本轮与上轮拿牌状态是否相同() && 本轮与上轮商店状态是否相同())
                        {
                            未拿牌累积次数++;
                        }
                        else
                        {
                            未拿牌累积次数 = 0;
                        }


                        if (当前商店刷新状态 == 刷新状态.刷新中)
                        {
                            if ((!本轮是否按下过鼠标) && (!鼠标左键是否按下) && 本轮与最近一次刷新轮商店状态是否相同())
                            {

                                从上次尝试刷新到目前为止经过的轮次++;
                                LogTool.Log($"发现商店有空或商店状态未变化，从上次尝试刷新到目前为止经过的轮次:{从上次尝试刷新到目前为止经过的轮次}");
                                //Debug.WriteLine($"发现商店有空或商店状态未变化，从上次尝试刷新到目前为止经过的轮次:{从上次尝试刷新到目前为止经过的轮次}");
                                if (从上次尝试刷新到目前为止经过的轮次 >= 5 || 计时器.Elapsed.TotalSeconds >= 2.0)
                                {
                                    LogTool.Log($"轮次达到上限或者时间超时 - 轮次：{从上次尝试刷新到目前为止经过的轮次} - 上次时间:{计时器.Elapsed.TotalSeconds}");
                                    //Debug.WriteLine($"轮次达到上限或者时间超时 - 轮次：{从上次尝试刷新到目前为止经过的轮次} - 上次时间:{计时器.Elapsed.TotalSeconds}");
                                    从上次尝试刷新到目前为止经过的轮次 = 0;
                                    当前商店刷新状态 = 刷新状态.未开始;
                                    未刷新累积次数++;
                                }
                            }
                            else if ((!本轮是否按下过鼠标) && (!鼠标左键是否按下) && 本轮是否为空())
                            {
                                LogTool.Log($"最近一次刷新商店轮数商店不为空的情况下，本轮商店状态为空。");
                                //Debug.WriteLine($"最近一次刷新商店轮数商店不为空的情况下，本轮商店状态为空。");
                            }
                            else
                            {
                                从上次尝试刷新到目前为止经过的轮次 = 0;
                                当前商店刷新状态 = 刷新状态.已结束;
                                未刷新累积次数 = 0;
                            }
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            上一次结果数组[i] = 纠正结果数组[i];
                            lastOrderCard[i] = currentOrderCard[i];
                        }

                        if (is自动刷新商店 && (!本轮是否存在目标卡) && 当前商店刷新状态 != 刷新状态.刷新中 && (!本轮是否为空()))
                        {
                            if (!本轮是否按下过鼠标 && !鼠标左键是否按下)
                            {
                                for (int i = 0; i < 最近一次刷新轮商店状态.Length; i++)
                                {
                                    最近一次刷新轮商店状态[i] = 纠正结果数组[i];
                                }
                                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:刷新");
                                //Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:刷新");
                                计时器.Restart();
                                if(_iappConfigService.CurrentConfig.MouseRefresh)
                                {
                                    MouseControlTool.SetMousePosition(_iappConfigService.CurrentConfig.Point_RefreshStoreX, _iappConfigService.CurrentConfig.Point_RefreshStoreY);
                                    await Task.Delay(操作间隔时间);
                                    await ClickOneTime();
                                    await Task.Delay(操作间隔时间);
                                }
                               else if(_iappConfigService.CurrentConfig.KeyboardRefresh)
                                {
                                    KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.RefreshKey);
                                }
                                当前商店刷新状态 = 刷新状态.刷新中;
                            }
                            else
                            {
                                LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     鼠标左键被按下，本轮不刷新！");
                                //Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     鼠标左键被按下，本轮不刷新！");
                            }
                        }
                        else
                        {
                            LogTool.Log($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:不刷新");
                            //Debug.WriteLine($"刷新判断前的刷新状态:{当前商店刷新状态}     本轮操作:不刷新");
                        }
                    }
                }, token);

            }
        }
        public  void AutoRefresh()
        {
            is自动刷新商店 = !is自动刷新商店;
            未刷新累积次数 = 0;
        }
        private bool 本轮与上轮商店状态是否相同()
        {
            for (int i = 0; i < 纠正结果数组.Length; i++)
            {
                if (纠正结果数组[i] != 上一次结果数组[i])
                {
                    return false;
                }
            }
            return true;
        }
        private bool 本轮与上轮拿牌状态是否相同()
        {
            for (int i = 0; i < currentOrderCard.Length; i++)
            {
                if (currentOrderCard[i] != lastOrderCard[i])
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
       
        /// <summary>
        /// 异步处理每张卡片的截图、OCR识别和UI更新
        /// </summary>
        /// <param name="startPointX"></param>
        /// <param name="startPointY"></param>
        /// <param name="cardID"></param>
        /// <param name="pictureBox"></param>
        /// <param name="textBox"></param>
        /// <returns></returns>
        private async Task<bool> OCR_Compare_GetCard(Bitmap card, int startPointX, int resultID)
        {
            string result = "";
            if (_iappConfigService.CurrentConfig.UseCPU)
            {
               result =CPU_OCRTool.OCRRecognition(card);
            }
            else if(_iappConfigService.CurrentConfig.UseGPU)
            {
                result = _ocrObject.RecognizeBitmap(card);
            }
            else
            {
                return false;
            }

            Debug.WriteLine(result);
            原始结果数组[resultID - 1] = result;
            result = _iCorrectionService.ConvertToRightResult(result);
            // 调用判断并拿牌
            纠正结果数组[resultID - 1] = result;
            return await CompareAndGetCard(result, startPointX,resultID);
        }

        /// <summary>
        /// 比较并拿牌
        /// </summary>
        /// <param name="result"></param>
        /// <param name="startPoint_CardScreenshotX"></param>
        /// <returns></returns>
        private async Task<bool> CompareAndGetCard(string result, int startPoint_CardScreenshotX,int resultID)
        {
            for (int i = 0; i < _iheroDataService.HeroDatas.Count; i++)
            {
                if (_ilineUpService.LineUps[_ilineUpService.LineUpIndex].Checked[_ilineUpService.SubLineUpIndex,i])
                {
                    if (result == _iheroDataService.HeroDatas[i].HeroName)
                    {
                        本轮是否存在目标卡 = true;
                        if (!本轮是否按下过鼠标 && !鼠标左键是否按下)
                        {
                            if(_iappConfigService.CurrentConfig.MouseGetCard)
                            {
                                // 鼠标操作
                                MouseControlTool.SetMousePosition(startPoint_CardScreenshotX + _iappConfigService.CurrentConfig.Width_CardScreenshot / 2, _iappConfigService.CurrentConfig.StartPoint_CardScreenshotY - _iappConfigService.CurrentConfig.Height_CardScreenshot * 2);
                                await Task.Delay(操作间隔时间);
                                // 执行点击操作，逐个点击并等待
                                await ClickOneTime();
                                await Task.Delay(操作间隔时间);
                            }
                           else if(_iappConfigService.CurrentConfig.KeyboardGetCard)
                            {
                                switch (resultID)
                                {
                                    case 1:
                                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey1);
                                        break;
                                    case 2:
                                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey2);
                                        break;
                                    case 3:
                                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey3);
                                        break;
                                    case 4:
                                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey4);
                                        break;
                                    case 5:
                                        KeyboardControlTool.PressKey(_iappConfigService.CurrentConfig.GetCardKey5);
                                        break;
                                }
                                await Task.Delay(操作间隔时间);
                            }                                                           
                        }
                        return true;
                    }

                }
            }
            return false;
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
        public void MouseLeftButtonDown()
        {
            鼠标左键是否按下 = true;
            本轮是否按下过鼠标 = true;
        }
        public void MouseLeftButtonUp()
        {
            鼠标左键是否按下 = false;            
        }
    }
}
