using JinChanChanTool.Tools.KeyBoardTools;
using JinChanChanTool.Tools.MouseTools;
using JinChanChanTool.DataClass;
using System.Windows.Forms;

namespace JinChanChanTool.Tools.KeyboardMouseTools
{
    /// <summary>
    /// 基于 Windows API 的键鼠操作设备实现。
    /// </summary>
    public sealed class WinApiKeyboardMouseDevice : IKeyboardMouseDevice
    {
        /// <inheritdoc />
        public KeyboardMouseDeviceType DeviceType => KeyboardMouseDeviceType.WinApi;

        /// <inheritdoc />
        public string DisplayName => "WinAPI";

        /// <inheritdoc />
        public bool IsAvailable => OperatingSystem.IsWindows();

        /// <inheritdoc />
        public void SetMousePosition(int x, int y)
        {
            MouseControlTool.SetMousePosition(x, y);
        }

        /// <inheritdoc />
        /// <remarks>
        /// 使用带按下保持时间的点击：部分游戏与模拟器会丢弃间隔过短的按下/抬起组合。
        /// </remarks>
        public void SetMousePositionAndClickLeftButton(int x, int y)
        {
            MouseControlTool.SetMousePositionAndClickLeftButton(x, y);
        }

        /// <inheritdoc />
        public void MouseLeftButtonDown()
        {
            MouseControlTool.MakeMouseLeftButtonDown();
        }

        /// <inheritdoc />
        public void MouseLeftButtonUp()
        {
            MouseControlTool.MakeMouseLeftButtonUp();
        }

        /// <inheritdoc />
        public void KeyDown(Keys key)
        {
            KeyboardControlTool.KeyDown(key);
        }

        /// <inheritdoc />
        public void KeyUp(Keys key)
        {
            KeyboardControlTool.KeyUp(key);
        }

        /// <inheritdoc />
        public void PressKey(Keys key)
        {
            KeyboardControlTool.PressKey(key);
        }

        /// <inheritdoc />
        public void PressKey(string keyName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(keyName);
            KeyboardControlTool.PressKey(keyName);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // WinAPI 不持有需要释放的设备资源。
        }
    }
}
