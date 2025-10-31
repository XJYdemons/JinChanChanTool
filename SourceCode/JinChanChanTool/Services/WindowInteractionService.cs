using System;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // EnumChildWindows 需要一个回调委托
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);
        private const uint GA_ROOT = 2;
        [DllImport("user32.dll")] private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        #endregion

        #region 公开属性

        public IntPtr WindowHandle { get; private set; } = IntPtr.Zero;
        public int ClientX { get; private set; }
        public int ClientY { get; private set; }
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }
        public bool IsWindowFound => WindowHandle != IntPtr.Zero;

        private List<IntPtr> _candidateChildren;

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
       
        public bool SetTargetToBestChildWindow(Process parentProcess)
        {
            WindowHandle = IntPtr.Zero;
            if (parentProcess == null || parentProcess.MainWindowHandle == IntPtr.Zero)
            {
                //Debug.WriteLine("[日志] 失敗：传入的父进程为null或没有主窗口。");
                return false;
            }

            IntPtr parentHwnd = parentProcess.MainWindowHandle;
            //Debug.WriteLine($"[日志] 开始侦察父窗口 (句柄: {parentHwnd}) 的后代...");
            var candidateChildren = new List<(IntPtr Hwnd, int Depth, long Area, string ClassName)>();

            EnumChildWindows(parentHwnd, (hWnd, lParam) => {
                //Debug.WriteLine($"  -> 发现一个子窗口 (句柄: {hWnd})");

                if (!IsWindowVisible(hWnd) || !IsWindowEnabled(hWnd))
                {
                    //Debug.WriteLine($"     [排除] 原因：不可见或被禁用。");
                    return true;
                }

                GetWindowRect(hWnd, out RECT rect);
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                if (width <= 100 || height <= 100)
                {
                    //Debug.WriteLine($"     [排除] 原因：尺寸过小 ({width}x{height})。");
                    return true;
                }

                int depth = 0;
                IntPtr current = hWnd;
                while ((current = GetParent(current)) != parentHwnd && current != IntPtr.Zero)
                {
                    depth++;
                }

                long area = (long)width * height;

                // 获取类名用于调试
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);

                //Debug.WriteLine($"     [候选] 类名: {className}, 尺寸: {width}x{height}, 深度: {depth}, 面积: {area}");
                candidateChildren.Add((hWnd, depth, area, className.ToString()));

                return true;
            }, IntPtr.Zero);

            if (candidateChildren.Count == 0)
            {
                //Debug.WriteLine("[日志] 警告：没有找到任何合适的子窗口，将尝试使用父窗口本身。");
                GetWindowRect(parentHwnd, out RECT parentRect);
                long parentArea = (long)(parentRect.Right - parentRect.Left) * (parentRect.Bottom - parentRect.Top);
                candidateChildren.Add((parentHwnd, -1, parentArea, "父窗口"));
            }

            var sortedCandidates = candidateChildren.OrderByDescending(c => c.Depth).ThenByDescending(c => c.Area);
            var bestCandidate = sortedCandidates.First();
            IntPtr bestHwnd = bestCandidate.Hwnd;

            //Debug.WriteLine($"[日志] 决策结果：选择的最佳窗口是 -> 类名: {bestCandidate.ClassName}, 句柄: {bestHwnd}, 深度: {bestCandidate.Depth}, 面积: {bestCandidate.Area}");

            WindowHandle = bestHwnd;

            if (!GetClientRect(WindowHandle, out RECT clientRect))
            {
                //Debug.WriteLine("[日志] 致命错误：GetClientRect 失败！");
                WindowHandle = IntPtr.Zero;
                return false;
            }

            ClientWidth = clientRect.Right - clientRect.Left;
            ClientHeight = clientRect.Bottom - clientRect.Top;

            POINT clientTopLeft = new POINT { X = 0, Y = 0 };
            if (!ClientToScreen(WindowHandle, ref clientTopLeft))
            {
                //Debug.WriteLine("[日志] 致命错误：ClientToScreen 失败！");
                WindowHandle = IntPtr.Zero;
                return false;
            }

            ClientX = clientTopLeft.X;
            ClientY = clientTopLeft.Y;

            //Debug.WriteLine($"[日志] 成功获取窗口信息：尺寸 {ClientWidth}x{ClientHeight} @ 屏幕坐标 ({ClientX},{ClientY})");
            return true;
        }
    }
}