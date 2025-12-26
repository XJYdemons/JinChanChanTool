using JinChanChanTool.DataClass.StaticData;
using System.Diagnostics;
using static JinChanChanTool.Services.AutoSetCoordinates.CoordinateCalculationService;

namespace JinChanChanTool.Services.AutoSetCoordinates
{
    /// <summary>
    /// 当前检测到的游戏模式。
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// 未找到任何支持的游戏。
        /// </summary>
        None,
        /// <summary>
        /// 云顶之弈 PC 客户端。
        /// </summary>
        TFT,
        /// <summary>
        /// 金铲铲之战（模拟器）。
        /// </summary>
        JCC
    }

    /// <summary>
    /// 定义可请求坐标的UI元素。
    /// </summary>
    public enum UiElement
    {
        ExpButton,
        RefreshButton,
        CardSlot1_Name,
        CardSlot2_Name,
        CardSlot3_Name,
        CardSlot4_Name,
        CardSlot5_Name,
        CardSlot1_Click,
        CardSlot2_Click,
        CardSlot3_Click,
        CardSlot4_Click,
        CardSlot5_Click,
        GoldAmount,
        CardSlot1_Highlight,
        CardSlot2_Highlight,
        CardSlot3_Highlight,
        CardSlot4_Highlight,
        CardSlot5_Highlight
    }

    /// <summary>
    /// 自动化的总服务。
    /// 负责自动检测游戏进程，并提供正确的UI元素坐标。
    /// </summary>
    public class AutomationService
    {
        private readonly WindowInteractionService _windowInteractionService;
        private readonly CoordinateCalculationService _coordService;

        /// <summary>
        /// 当前检测到的游戏模式。
        /// </summary>
        public GameMode CurrentGameMode { get; private set; } = GameMode.None;

        /// <summary>
        /// 一个便捷属性，指示是否已成功检测到游戏窗口。
        /// </summary>
        public bool IsGameDetected => CurrentGameMode != GameMode.None;

        public AutomationService(WindowInteractionService windowInteractionService, CoordinateCalculationService coordService)
        {
            _windowInteractionService = windowInteractionService;
            _coordService = coordService;
        }
       
        /// <summary>
        /// 设置用户选择的进程为自动化目标。
        /// 此方法会智能判断进程类型，并采用相应的窗口查找策略。
        /// </summary>
        /// <param name="process">用户选择的进程。</param>
        public void SetTargetProcess(Process process)
        {
            if (process == null)
            {
                _windowInteractionService.SetTargetWindow(null); // 清除目标
                CurrentGameMode = GameMode.None;
                return;
            }

            // 检查进程名，决定使用哪种窗口查找策略
            if (process.ProcessName.Equals("League of Legends", StringComparison.OrdinalIgnoreCase))
            {
                // --- 对于云顶之弈，使用简单、直接的父窗口查找策略 ---
                if (_windowInteractionService.SetTargetWindow(process))
                {
                    CurrentGameMode = GameMode.TFT;
                }
                else
                {
                    CurrentGameMode = GameMode.None;
                }
            }
            else
            {
                // --- 对于模拟器或任何其他程序，使用更强大的子窗口查找策略 ---
                if (_windowInteractionService.SetTargetToBestChildWindow(process))
                {
                    CurrentGameMode = GameMode.JCC;
                }
                else
                {
                    CurrentGameMode = GameMode.None;
                }
            }
        }

        /// <summary>
        /// 获取指定UI元素在屏幕上的绝对坐标和大小。
        /// </summary>
        /// <param name="element">想要获取坐标的UI元素。</param>
        /// <returns>返回计算好的矩形区域，如果未检测到游戏则返回null。</returns>
        public Rectangle? GetTargetRectangle(UiElement element)
        {
            if (!IsGameDetected) return null;

            // 1. 根据当前游戏模式，选择正确的基准坐标模板和分辨率
            AnchorProfile profile;
            Size baseResolution;

            if (CurrentGameMode == GameMode.TFT)
            {
                profile = GetTftProfile(element);
                baseResolution = TftCoordinateTemplates.BaseResolution;
            }
            else // JCC
            {
                profile = GetJccProfile(element);
                baseResolution = JccCoordinateTemplates.BaseResolution;
            }

            // 2. 调用坐标计算服务来获取最终结果
            return _coordService.GetScaledRectangle(profile, baseResolution, CurrentGameMode);
        }

        /// <summary>
        /// 根据UI元素枚举，获取对应的云顶之弈坐标模板档案。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private AnchorProfile GetTftProfile(UiElement element)
        {
            switch (element)
            {
                case UiElement.ExpButton: return TftCoordinateTemplates.ExpButton;
                case UiElement.RefreshButton: return TftCoordinateTemplates.RefreshButton;
                case UiElement.CardSlot1_Name: return TftCoordinateTemplates.CardSlot1_Name;
                case UiElement.CardSlot2_Name: return TftCoordinateTemplates.CardSlot2_Name;
                case UiElement.CardSlot3_Name: return TftCoordinateTemplates.CardSlot3_Name;
                case UiElement.CardSlot4_Name: return TftCoordinateTemplates.CardSlot4_Name;
                case UiElement.CardSlot5_Name: return TftCoordinateTemplates.CardSlot5_Name;
                case UiElement.CardSlot1_Click: return TftCoordinateTemplates.CardSlot1_Click;
                case UiElement.CardSlot2_Click: return TftCoordinateTemplates.CardSlot2_Click;
                case UiElement.CardSlot3_Click: return TftCoordinateTemplates.CardSlot3_Click;
                case UiElement.CardSlot4_Click: return TftCoordinateTemplates.CardSlot4_Click;
                case UiElement.CardSlot5_Click: return TftCoordinateTemplates.CardSlot5_Click;
                case UiElement.GoldAmount: return TftCoordinateTemplates.GoldAmount;
                case UiElement.CardSlot1_Highlight: return TftCoordinateTemplates.CardSlot1_Click;
                case UiElement.CardSlot2_Highlight: return TftCoordinateTemplates.CardSlot2_Click;
                case UiElement.CardSlot3_Highlight: return TftCoordinateTemplates.CardSlot3_Click;
                case UiElement.CardSlot4_Highlight: return TftCoordinateTemplates.CardSlot4_Click;
                case UiElement.CardSlot5_Highlight: return TftCoordinateTemplates.CardSlot5_Click;
                default: throw new ArgumentOutOfRangeException(nameof(element), "未知的UI元素。");
            }
        }

        /// <summary>
        /// 根据UI元素枚举，获取对应的金铲铲之战坐标模板档案。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private AnchorProfile GetJccProfile(UiElement element)
        {
            switch (element)
            {
                case UiElement.ExpButton: return JccCoordinateTemplates.ExpButton;
                case UiElement.RefreshButton: return JccCoordinateTemplates.RefreshButton;
                case UiElement.CardSlot1_Name: return JccCoordinateTemplates.CardSlot1_Name;
                case UiElement.CardSlot2_Name: return JccCoordinateTemplates.CardSlot2_Name;
                case UiElement.CardSlot3_Name: return JccCoordinateTemplates.CardSlot3_Name;
                case UiElement.CardSlot4_Name: return JccCoordinateTemplates.CardSlot4_Name;
                case UiElement.CardSlot5_Name: return JccCoordinateTemplates.CardSlot5_Name;
                case UiElement.CardSlot1_Click: return JccCoordinateTemplates.CardSlot1_Click;
                case UiElement.CardSlot2_Click: return JccCoordinateTemplates.CardSlot2_Click;
                case UiElement.CardSlot3_Click: return JccCoordinateTemplates.CardSlot3_Click;
                case UiElement.CardSlot4_Click: return JccCoordinateTemplates.CardSlot4_Click;
                case UiElement.CardSlot5_Click: return JccCoordinateTemplates.CardSlot5_Click;
                case UiElement.GoldAmount: return JccCoordinateTemplates.GoldAmount;
                case UiElement.CardSlot1_Highlight: return JccCoordinateTemplates.CardSlot1_Click;
                case UiElement.CardSlot2_Highlight: return JccCoordinateTemplates.CardSlot2_Click;
                case UiElement.CardSlot3_Highlight: return JccCoordinateTemplates.CardSlot3_Click;
                case UiElement.CardSlot4_Highlight: return JccCoordinateTemplates.CardSlot4_Click;
                case UiElement.CardSlot5_Highlight: return JccCoordinateTemplates.CardSlot5_Click;
                default: throw new ArgumentOutOfRangeException(nameof(element), "未知的UI元素。");
            }
        }
    }
}