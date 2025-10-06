using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JinChanChanTool.Services
{
    /// <summary>
    /// 使用锚点布局模型，动态计算UI元素屏幕绝对坐标的服务。
    /// </summary>
    public class CoordinateCalculationService
    {
        private readonly GameWindowService _gameWindowService;

        #region P/Invoke for DPI
        [DllImport("User32.dll")]
        private static extern int GetDpiForWindow(IntPtr hWnd);
        private const int USER_DEFAULT_SCREEN_DPI = 96;
        #endregion

        /// <summary>
        /// 定义一个UI元素的基准档案，包含其相对于锚点的中心偏移量和原始尺寸。
        /// </summary>
        public readonly struct AnchorProfile
        {
            public readonly double OffsetX; // 元素中心点相对于锚点X的偏移
            public readonly double OffsetY; // 元素中心点相对于锚点Y的偏移
            public readonly int BaseWidth;
            public readonly int BaseHeight;

            public AnchorProfile(double offsetX, double offsetY, int baseWidth, int baseHeight)
            {
                OffsetX = offsetX;
                OffsetY = offsetY;
                BaseWidth = baseWidth;
                BaseHeight = baseHeight;
            }
        }

        public CoordinateCalculationService(GameWindowService gameWindowService)
        {
            _gameWindowService = gameWindowService ?? throw new ArgumentNullException(nameof(gameWindowService));
        }

        /// <summary>
        /// 核心计算方法：根据一个基准坐标，计算出在当前游戏窗口中的最终屏幕矩形。
        /// </summary>
        /// <param name="profile">UI元素的基准坐标（锚点偏移和尺寸）。</param>
        /// <param name="baseResolution">该基准坐标所对应的基准分辨率。</param>
        /// <param name="gameMode">当前的游戏模式，也可以用于调试输出。</param>
        /// <returns>计算出的屏幕绝对坐标矩形。如果找不到游戏窗口则返回null。</returns>
        public Rectangle? GetScaledRectangle(AnchorProfile profile, Size baseResolution, GameMode gameMode)
        {
            if (!_gameWindowService.IsWindowFound) return null;

            //
            double dpiScale = 1.0; // 默认缩放为 1.0，对JCC是正确的

            // 只有当游戏模式是TFT (DPI Aware)时，才获取并应用真实的DPI缩放
            if (gameMode == GameMode.TFT)
            {
                int windowDpi = GetDpiForWindow(_gameWindowService.WindowHandle);
                dpiScale = windowDpi / (double)USER_DEFAULT_SCREEN_DPI;
            }
            // --- 修改结束 ---

            // 对于JCC, dpiScale将永远是1.0, 从而避免了双重缩放
            // 对于TFT, 这里会使用正确的物理尺寸
            double physicalClientWidth = _gameWindowService.ClientWidth * dpiScale;
            double physicalClientHeight = _gameWindowService.ClientHeight * dpiScale;


            double scale = physicalClientHeight / baseResolution.Height;
            double scaledWidth = profile.BaseWidth * scale;
            double scaledHeight = profile.BaseHeight * scale;
            double scaledOffsetX = profile.OffsetX * scale;
            double scaledOffsetY = profile.OffsetY * scale;

            double currentAnchorX = _gameWindowService.ClientX + (physicalClientWidth / 2);
            double currentAnchorY = _gameWindowService.ClientY + physicalClientHeight;

            int finalX = (int)Math.Round(currentAnchorX + scaledOffsetX - (scaledWidth / 2));
            int finalY = (int)Math.Round(currentAnchorY + scaledOffsetY - (scaledHeight / 2));

            return new Rectangle(finalX, finalY, (int)Math.Round(scaledWidth), (int)Math.Round(scaledHeight));
        }
    }
}