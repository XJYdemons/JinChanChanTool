using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Services
{
    /// <summary>
    /// 负责与一个具体的目标窗口进行交互，获取其位置和大小等信息。
    /// </summary>
    public class WindowInteractionService
    {
        #region P/Invoke Windows API

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        #endregion

        #region 公开属性

        public IntPtr WindowHandle { get; private set; } = IntPtr.Zero;
        public int ClientX { get; private set; }
        public int ClientY { get; private set; }
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }
        public bool IsWindowFound => WindowHandle != IntPtr.Zero;

        #endregion

        /// <summary>
        /// 根据用户选择的进程，设置其为主目标窗口并更新窗口信息。
        /// </summary>
        /// <param name="targetProcess">用户选择的目标进程。</param>
        /// <returns>如果成功获取窗口信息，则返回true。</returns>
        public bool SetTargetWindow(Process targetProcess)
        {
            // 重置状态
            WindowHandle = IntPtr.Zero;
            if (targetProcess == null || targetProcess.MainWindowHandle == IntPtr.Zero)
            {
                return false;
            }

            WindowHandle = targetProcess.MainWindowHandle;

            if (!GetClientRect(WindowHandle, out RECT clientRect))
            {
                WindowHandle = IntPtr.Zero;
                return false;
            }

            ClientWidth = clientRect.Right - clientRect.Left;
            ClientHeight = clientRect.Bottom - clientRect.Top;

            POINT clientTopLeft = new POINT { X = 0, Y = 0 };
            if (!ClientToScreen(WindowHandle, ref clientTopLeft))
            {
                WindowHandle = IntPtr.Zero;
                return false;
            }

            ClientX = clientTopLeft.X;
            ClientY = clientTopLeft.Y;

            return true;
        }
    }
}