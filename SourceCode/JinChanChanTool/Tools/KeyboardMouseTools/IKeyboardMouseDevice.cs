using System.Windows.Forms;
using JinChanChanTool.DataClass;

namespace JinChanChanTool.Tools.KeyboardMouseTools
{
    /// <summary>
    /// 键鼠操作设备的统一接口。
    /// </summary>
    /// <remarks>
    /// 坐标使用 Windows 虚拟桌面的绝对屏幕坐标。实现可以是本机 API、USB
    /// 设备或网络设备，业务层不应依赖具体设备协议。
    /// </remarks>
    public interface IKeyboardMouseDevice : IDisposable
    {
        /// <summary>
        /// 设备类型。
        /// </summary>
        KeyboardMouseDeviceType DeviceType { get; }

        /// <summary>
        /// 用于日志和设置界面展示的设备名称。
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 当前设备是否可用于执行输入操作。
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// 将鼠标移动到虚拟桌面中的绝对坐标。
        /// </summary>
        void SetMousePosition(int x, int y);

        /// <summary>
        /// 将鼠标移动到指定绝对坐标并单击左键。
        /// </summary>
        void SetMousePositionAndClickLeftButton(int x, int y);

        /// <summary>
        /// 按下鼠标左键。
        /// </summary>
        void MouseLeftButtonDown();

        /// <summary>
        /// 释放鼠标左键。
        /// </summary>
        void MouseLeftButtonUp();

        /// <summary>
        /// 按下键盘按键。
        /// </summary>
        void KeyDown(Keys key);

        /// <summary>
        /// 释放键盘按键。
        /// </summary>
        void KeyUp(Keys key);

        /// <summary>
        /// 完整按下一次并释放键盘按键。
        /// </summary>
        void PressKey(Keys key);

        /// <summary>
        /// 根据键名完整按下一次并释放键盘按键。
        /// </summary>
        void PressKey(string keyName);
    }
}
