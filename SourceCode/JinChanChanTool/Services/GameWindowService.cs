using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Services
{
    /// <summary>
    /// 负责查找和管理游戏窗口的服务。
    /// </summary>
    public class GameWindowService
    {
        #region P/Invoke Windows API

        // 定义Windows API所需的结构体
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

        // 导入所需的User32.dll函数
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        #endregion

        #region 公开属性

        /// <summary>
        /// 找到的游戏窗口的句柄。
        /// </summary>
        public IntPtr WindowHandle { get; private set; } = IntPtr.Zero;

        /// <summary>
        /// 游戏窗口客户区左上角在屏幕上的X坐标。
        /// </summary>
        public int ClientX { get; private set; }

        /// <summary>
        /// 游戏窗口客户区左上角在屏幕上的Y坐标。
        /// </summary>
        public int ClientY { get; private set; }

        /// <summary>
        /// 游戏窗口客户区的宽度。
        /// </summary>
        public int ClientWidth { get; private set; }

        /// <summary>
        /// 游戏窗口客户区的高度。
        /// </summary>
        public int ClientHeight { get; private set; }

        /// <summary>
        /// 指示是否成功找到了窗口。
        /// </summary>
        public bool IsWindowFound => WindowHandle != IntPtr.Zero;

        #endregion

        // --- 新增方法：获取所有可能的游戏进程 ---
        /// <summary>
        /// 获取当前系统中所有拥有可见主窗口的进程列表。
        /// </summary>
        /// <returns>一个 Process 列表。</returns>
        public List<Process> GetPotentiallyVisibleProcesses()
        {
            return Process.GetProcesses()
                .Where(p => p.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(p.MainWindowTitle))
                .OrderBy(p => p.ProcessName)
                .ToList();
        }

        // --- 重构方法：不再通过名字查找，而是直接设置 ---
        /// <summary>
        /// 根据用户选择的进程，设置其为主目标窗口并获取窗口信息。
        /// </summary>
        /// <param name="targetProcess">用户选择的目标进程。</param>
        /// <returns>如果成功获取窗口信息，则返回true。</returns>
        public bool SetTargetWindow(Process targetProcess)
        {
            WindowHandle = IntPtr.Zero; // 重置状态
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