using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Tools.MouseTools
{
    public static class MouseHookTool
    {
        #region Win32 API 声明
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, nint hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(nint hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint GetModuleHandle(string lpModuleName);

        private delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);
        #endregion

        #region 静态核心实现
        private static nint _hookId = nint.Zero;
        private static LowLevelMouseProc _mouseProc;
        private static readonly object _counterLock = new object();
        private static long _programClickCounter = 0;

        // 静态事件
        public static event EventHandler MouseLeftButtonDown;
        public static event EventHandler MouseLeftButtonUp;

        /// <summary>
        /// 初始化鼠标钩子（必须在主窗口加载时调用）
        /// </summary>
        public static void Initialize()
        {
            if (_hookId != nint.Zero) return; // 防止重复初始化

            _mouseProc = HookCallback;
            SetHook();
        }

      
        private static void SetHook()
        {            
            nint hModule = Marshal.GetHINSTANCE(typeof(MouseHookTool).Module);
            _hookId = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, hModule, 0);
        }

        private static nint HookCallback(int nCode, nint wParam, nint lParam)
        {
            if (nCode < 0) 
                return CallNextHookEx(_hookId, nCode, wParam, lParam);

            
            bool isProgramEvent;
            lock (_counterLock)
            {
                isProgramEvent = _programClickCounter > 0;
            }

            if (!isProgramEvent)
            {
                
                if (wParam == (nint)WM_LBUTTONDOWN)
                {
                    MouseLeftButtonDown?.Invoke(null, EventArgs.Empty);                    
                }                   
                else if (wParam == (nint)WM_LBUTTONUP)
                {
                    MouseLeftButtonUp?.Invoke(null, EventArgs.Empty);                   
                }                    
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        /// <summary>
        /// 程序点击计数器（只读）
        /// </summary>
        public static long ProgramClickCounter
        {
            get
            {
                lock (_counterLock)
                {
                    return _programClickCounter;
                }
            }
        }

        /// <summary>
        /// 增加程序自身点击计数
        /// </summary>
        public static void IncrementProgramClickCount()
        {
            lock (_counterLock)
            {
                _programClickCounter++;
            }
        }

        /// <summary>
        /// 减少程序自身点击计数
        /// </summary>
        public static void DecrementProgramClickCount()
        {
            lock (_counterLock)
            {
                if (_programClickCounter > 0)
                    _programClickCounter--;
            }
        }

        /// <summary>
        /// 清理鼠标钩子资源（程序退出前必须调用）
        /// </summary>
        public static void Dispose()
        {
            if (_hookId != nint.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = nint.Zero;
            }
        }
        #endregion
    }
}