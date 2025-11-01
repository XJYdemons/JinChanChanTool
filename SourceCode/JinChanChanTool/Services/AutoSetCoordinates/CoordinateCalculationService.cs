using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JinChanChanTool.Services.AutoSetCoordinates
{
    /// <summary>
    /// 使用锚点布局模型，动态计算UI元素屏幕绝对坐标的服务。
    /// </summary>
    public class CoordinateCalculationService
    {
        private readonly WindowInteractionService _windowInteractionService;

        #region P/Invoke for DPI
        [DllImport("User32.dll")]
        private static extern int GetDpiForWindow(nint hWnd);
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

        public CoordinateCalculationService(WindowInteractionService windowInteractionService)
        {
            _windowInteractionService = windowInteractionService ?? throw new ArgumentNullException(nameof(windowInteractionService));
        }

        

        public Rectangle? GetScaledRectangle(AnchorProfile profile, Size baseResolution, GameMode gameMode)
        {
            if (!_windowInteractionService.IsWindowFound) return null;

            double dpiScale = 1.0;
            if (gameMode == GameMode.TFT)
            {
                int windowDpi = GetDpiForWindow(_windowInteractionService.WindowHandle);
                dpiScale = windowDpi / (double)USER_DEFAULT_SCREEN_DPI;
            }

            double physicalClientWidth = _windowInteractionService.ClientWidth * dpiScale;
            double physicalClientHeight = _windowInteractionService.ClientHeight * dpiScale;

            double scale = physicalClientHeight / baseResolution.Height;
            double scaledWidth = profile.BaseWidth * scale;
            double scaledHeight = profile.BaseHeight * scale;
            double scaledOffsetX = profile.OffsetX * scale;
            double scaledOffsetY = profile.OffsetY * scale;

            double currentAnchorX = _windowInteractionService.ClientX + physicalClientWidth / 2;
            double currentAnchorY = _windowInteractionService.ClientY + physicalClientHeight;

            int finalX = (int)Math.Round(currentAnchorX + scaledOffsetX - scaledWidth / 2);
            int finalY = (int)Math.Round(currentAnchorY + scaledOffsetY - scaledHeight / 2);

            // --- 在这里添加调试代码 ---
            //Debug.WriteLine($"--- [CoordinateCalculationService] Calculation Details for '{gameMode}' ---");
            //Debug.WriteLine($"输入: BaseWidth={profile.BaseWidth}, OffsetX={profile.OffsetX}");
            //Debug.WriteLine($"窗口数据: ClientX={_windowInteractionService.ClientX}, ClientWidth={_windowInteractionService.ClientWidth}");
            //Debug.WriteLine($"计算中间值: dpiScale={dpiScale}, physicalClientWidth={physicalClientWidth}, scale={scale}, scaledOffsetX={scaledOffsetX}, scaledWidth={scaledWidth}");
            //Debug.WriteLine($"锚点计算: currentAnchorX={currentAnchorX}");
            //Debug.WriteLine($"最终输出坐标 (FinalX, FinalY): {finalX}, {finalY}");
            //Debug.WriteLine("-------------------------------------------------");
            // --- 调试代码结束 ---

            return new Rectangle(finalX, finalY, (int)Math.Round(scaledWidth), (int)Math.Round(scaledHeight));
        }
    }
}