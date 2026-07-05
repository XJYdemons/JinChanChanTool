using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Services.AutoSetCoordinates
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
        private static extern bool GetClientRect(nint hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(nint hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(nint hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowEnabled(nint hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(nint hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

        // EnumChildWindows 需要一个回调委托
        private delegate bool EnumWindowsProc(nint hWnd, nint lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(nint hwndParent, EnumWindowsProc lpEnumFunc, nint lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint GetAncestor(nint hwnd, uint gaFlags);
        private const uint GA_ROOT = 2;
        [DllImport("user32.dll")] private static extern nint GetParent(nint hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(nint hWnd, StringBuilder lpClassName, int nMaxCount);

        #endregion

        #region 公开属性

        public nint WindowHandle { get; private set; } = nint.Zero;
        public int ClientX { get; private set; }
        public int ClientY { get; private set; }
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }
        public bool IsWindowFound => WindowHandle != nint.Zero;

        private List<nint> _candidateChildren;

        #endregion

        /// <summary>
        /// 根据用户选择的进程，设置其为主目标窗口并更新窗口信息。
        /// </summary>
        /// <param name="targetProcess">用户选择的目标进程。</param>
        /// <returns>如果成功获取窗口信息，则返回true。</returns>
        public bool SetTargetWindow(Process? targetProcess)
        {
            // 重置状态
            WindowHandle = nint.Zero;
            if (targetProcess == null || targetProcess.MainWindowHandle == nint.Zero)
            {
                return false;
            }

            return SetTargetWindowHandle(targetProcess.MainWindowHandle);
        }

        /// <summary>
        /// 设置目标窗口为雷电模拟器内的游戏渲染窗口。
        /// </summary>
        /// <param name="parentProcess"></param>
        /// <returns></returns>
        public bool SetTargetToLdPlayerGameWindow(Process? parentProcess)
        {
            WindowHandle = nint.Zero;
            if (parentProcess == null)
            {
                return false;
            }

            var ldMainFrames = new List<nint>();
            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                if (processId != parentProcess.Id || !IsWindowVisible(hWnd))
                {
                    return true;
                }

                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                if (className.ToString().Equals("LDPlayerMainFrame", StringComparison.OrdinalIgnoreCase))
                {
                    ldMainFrames.Add(hWnd);
                }

                return true;
            }, nint.Zero);

            foreach (nint frameHwnd in ldMainFrames)
            {
                nint? renderWindow = FindLdPlayerRenderWindow(frameHwnd);
                if (renderWindow.HasValue && SetTargetWindowHandle(renderWindow.Value))
                {
                    return true;
                }
            }

            foreach (nint frameHwnd in ldMainFrames)
            {
                if (SetTargetWindowHandle(frameHwnd))
                {
                    return true;
                }
            }

            return SetTargetToBestChildWindow(parentProcess);
        }

        /// <summary>
        /// 设置目标窗口为指定父进程的最佳子窗口。
        /// </summary>
        /// <param name="parentProcess"></param>
        /// <returns></returns>
        public bool SetTargetToBestChildWindow(Process? parentProcess)
        {
            WindowHandle = nint.Zero;
            if (parentProcess == null || parentProcess.MainWindowHandle == nint.Zero)
            {
                //Debug.WriteLine("[日志] 失敗：传入的父进程为null或没有主窗口。");
                return false;
            }

            nint parentHwnd = parentProcess.MainWindowHandle;
            //Debug.WriteLine($"[日志] 开始侦察父窗口 (句柄: {parentHwnd}) 的后代...");
            var candidateChildren = new List<(nint Hwnd, int Depth, long Area, string ClassName)>();

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
                nint current = hWnd;
                while ((current = GetParent(current)) != parentHwnd && current != nint.Zero)
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
            }, nint.Zero);

            if (candidateChildren.Count == 0)
            {
                //Debug.WriteLine("[日志] 警告：没有找到任何合适的子窗口，将尝试使用父窗口本身。");
                GetWindowRect(parentHwnd, out RECT parentRect);
                long parentArea = (long)(parentRect.Right - parentRect.Left) * (parentRect.Bottom - parentRect.Top);
                candidateChildren.Add((parentHwnd, -1, parentArea, "父窗口"));
            }

            var sortedCandidates = candidateChildren.OrderByDescending(c => c.Depth).ThenByDescending(c => c.Area);
            var bestCandidate = sortedCandidates.First();
            nint bestHwnd = bestCandidate.Hwnd;

            //Debug.WriteLine($"[日志] 决策结果：选择的最佳窗口是 -> 类名: {bestCandidate.ClassName}, 句柄: {bestHwnd}, 深度: {bestCandidate.Depth}, 面积: {bestCandidate.Area}");

            return SetTargetWindowHandle(bestHwnd);
        }

        private nint? FindLdPlayerRenderWindow(nint frameHwnd)
        {
            var candidates = new List<(nint Hwnd, int Width, int Height, long Area, bool IsRenderWindow, bool IsGameResolution)>();
            EnumChildWindows(frameHwnd, (hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd))
                {
                    return true;
                }

                GetWindowRect(hWnd, out RECT rect);
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                if (width <= 100 || height <= 100)
                {
                    return true;
                }

                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                bool isRenderWindow = className.ToString().Equals("RenderWindow", StringComparison.OrdinalIgnoreCase);
                bool isGameResolution = width == 1600 && height == 900;
                candidates.Add((hWnd, width, height, (long)width * height, isRenderWindow, isGameResolution));

                return true;
            }, nint.Zero);

            if (candidates.Count == 0)
            {
                return null;
            }

            return candidates
                .OrderByDescending(c => c.IsRenderWindow)
                .ThenByDescending(c => c.IsGameResolution)
                .ThenByDescending(c => c.Area)
                .First()
                .Hwnd;
        }

        private bool SetTargetWindowHandle(nint hWnd)
        {
            WindowHandle = hWnd;
            if (!GetClientRect(WindowHandle, out RECT clientRect))
            {
                //Debug.WriteLine("[日志] 致命错误：GetClientRect 失败！");
                WindowHandle = nint.Zero;
                return false;
            }

            ClientWidth = clientRect.Right - clientRect.Left;
            ClientHeight = clientRect.Bottom - clientRect.Top;

            POINT clientTopLeft = new POINT { X = 0, Y = 0 };
            if (!ClientToScreen(WindowHandle, ref clientTopLeft))
            {
                //Debug.WriteLine("[日志] 致命错误：ClientToScreen 失败！");
                WindowHandle = nint.Zero;
                return false;
            }

            ClientX = clientTopLeft.X;
            ClientY = clientTopLeft.Y;

            //Debug.WriteLine($"[日志] 成功获取窗口信息：尺寸 {ClientWidth}x{ClientHeight} @ 屏幕坐标 ({ClientX},{ClientY})");
            return true;
        }
    }
}
