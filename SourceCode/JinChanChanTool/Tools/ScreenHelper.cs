using System.Diagnostics;

namespace JinChanChanTool.Tools
{
    /// <summary>
    /// 屏幕信息辅助类。
    ///
    /// 背景：.NET 8 将 <see cref="Screen.PrimaryScreen"/> 标注为可空（无显示器的
    /// headless 会话下确实可能为 null），因此直接访问其 Bounds 会产生 CS8602 警告。
    /// 桌面 WinForms 应用中主屏几乎总是存在，本类把「取主屏工作区/边界」这一
    /// 重复逻辑集中到一处，既消除各处散落的可空解引用，也便于将来统一处理无主屏场景。
    /// </summary>
    public static class ScreenHelper
    {
        /// <summary>
        /// 主屏不可用时的回退边界（1024x768，原点在左上角）。
        /// 仅在无任何显示器的极端场景下使用，避免调用方拿到空引用。
        /// </summary>
        private static readonly Rectangle FallbackBounds = new Rectangle(0, 0, 1024, 768);

        /// <summary>
        /// 获取主屏边界；主屏不可用时返回回退边界并记录日志。
        /// </summary>
        /// <returns>主屏边界矩形</returns>
        public static Rectangle GetPrimaryScreenBounds()
        {
            Screen? primaryScreen = Screen.PrimaryScreen;
            if (primaryScreen == null)
            {
                LogTool.Log("[ScreenHelper] 未检测到主屏，使用回退边界。");
                Debug.WriteLine("[ScreenHelper] 未检测到主屏，使用回退边界。");
                return FallbackBounds;
            }

            return primaryScreen.Bounds;
        }

        /// <summary>
        /// 获取主屏工作区（排除任务栏等停靠区域）；主屏不可用时返回回退边界并记录日志。
        /// </summary>
        /// <returns>主屏工作区矩形</returns>
        public static Rectangle GetPrimaryScreenWorkingArea()
        {
            Screen? primaryScreen = Screen.PrimaryScreen;
            if (primaryScreen == null)
            {
                LogTool.Log("[ScreenHelper] 未检测到主屏，使用回退工作区。");
                Debug.WriteLine("[ScreenHelper] 未检测到主屏，使用回退工作区。");
                return FallbackBounds;
            }

            return primaryScreen.WorkingArea;
        }
    }
}
