using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JinChanChanTool.Tools.KeyBoardTools
{
    public static class GlobalHotkeyTool
    {
        #region Win32 API 声明
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public nint dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint SetWindowsHookEx(int idHook, HookProc lpfn, nint hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(nint hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern nint GetModuleHandle(string lpModuleName);

        private delegate nint HookProc(int nCode, nint wParam, nint lParam);
        #endregion

        #region 静态核心实现
        private static Control _invokeControl;
        private static nint _hookId = nint.Zero;
        private static readonly object _lock = new object();
        private static HookProc _hookProcDelegate;
        private static GCHandle _hookHandle;

        private static readonly HashSet<Keys> _registeredKeys = new HashSet<Keys>();
        private static readonly Dictionary<Keys, Action> _callbacks = new Dictionary<Keys, Action>();
        private static readonly Dictionary<Keys, Action> _keyUpCallbacks = new Dictionary<Keys, Action>();

        // 使用新的状态跟踪方式
        private static readonly HashSet<Keys> _activeKeys = new HashSet<Keys>();

        /// <summary>
        /// 初始化全局热键系统
        /// </summary>
        public static void Initialize(Control invokeControl)
        {
            if (_hookId != nint.Zero) return;

            _invokeControl = invokeControl ?? throw new ArgumentNullException(nameof(invokeControl));
            _hookProcDelegate = HookCallback;
            _hookHandle = GCHandle.Alloc(_hookProcDelegate);
            InstallHook();
        }

        private static void InstallHook()
        {
            using var process = Process.GetCurrentProcess();
            _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProcDelegate,
                GetModuleHandle(process.MainModule.ModuleName), 0);

            if (_hookId == nint.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"钩子安装失败 (错误码: 0x{error:X8})");
            }
        }

        private static nint HookCallback(int nCode, nint wParam, nint lParam)
        {
            if (!Enabled || nCode < 0)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);

            var kbd = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            var vkCode = (Keys)kbd.vkCode;

            lock (_lock)
            {
                if (_registeredKeys.Contains(vkCode))
                {
                    switch ((int)wParam)
                    {
                        case WM_KEYDOWN:
                            // 检查该按键是否注册了抬起事件
                            bool hasKeyUpCallback = _keyUpCallbacks.ContainsKey(vkCode);

                            // 只有当按键尚未激活或没有注册抬起事件时才触发
                            if (!(hasKeyUpCallback &&_activeKeys.Contains(vkCode)))
                            {
                                if (_callbacks.TryGetValue(vkCode, out var downCallback))
                                {
                                    SafeInvoke(downCallback);

                                    // 只有注册了抬起事件的按键才需要状态跟踪
                                    if (hasKeyUpCallback)
                                    {
                                        _activeKeys.Add(vkCode);
                                    }
                                    return 1; // 阻止事件传递
                                }
                            }
                            break;
                        case WM_KEYUP:
                            if (_keyUpCallbacks.TryGetValue(vkCode, out var upCallback))
                            {
                                SafeInvoke(upCallback);
                                // 移除按键激活状态
                                _activeKeys.Remove(vkCode);
                                return 1; // 阻止事件传递
                            }
                            break;
                    }
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void SafeInvoke(Action callback)
        {
            try
            {
                if (callback == null) return;

                if (_invokeControl?.InvokeRequired == true)
                    _invokeControl.BeginInvoke(callback);
                else
                    callback();
            }
            catch (ObjectDisposedException) { }
        }

        public static bool IsKeyAvailable(Keys key)
        {
            lock (_lock) return !_registeredKeys.Contains(key);
        }

        /// <summary>
        /// 注册按键按下事件
        /// </summary>
        public static void Register(Keys key, Action callback)
        {
            if (key == Keys.None)
                throw new ArgumentException("主键不能为Keys.None");

            lock (_lock)
            {
                if (_registeredKeys.Contains(key))
                    throw new ArgumentException($"热键 {key} 已注册");

                _registeredKeys.Add(key);
                _callbacks[key] = callback;
            }
        }

        /// <summary>
        /// 注册按键抬起事件
        /// </summary>
        public static void RegisterKeyUp(Keys key, Action callback)
        {
            if (key == Keys.None)
                throw new ArgumentException("主键不能为Keys.None");

            lock (_lock)
            {
                if (!_registeredKeys.Contains(key))
                    throw new ArgumentException($"必须先注册按键按下事件: {key}");

                _keyUpCallbacks[key] = callback;
            }
        }

        public static void Unregister(Keys key)
        {
            lock (_lock)
            {
                _registeredKeys.Remove(key);
                _callbacks.Remove(key);
                _keyUpCallbacks.Remove(key);

                // 移除按键激活状态
                _activeKeys.Remove(key);
            }
        }

        /// <summary>
        /// 注销所有已注册的快捷键（包括按下与抬起事件）
        /// </summary>
        public static void UnregisterAll()
        {
            lock (_lock)
            {
                _registeredKeys.Clear();
                _callbacks.Clear();
                _keyUpCallbacks.Clear();
                _activeKeys.Clear();
            }
        }

        /// <summary>
        /// 清理全局热键系统资源
        /// </summary>
        public static void Dispose()
        {
            if (_hookId != nint.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = nint.Zero;
            }

            if (_hookHandle.IsAllocated)
                _hookHandle.Free();

            UnregisterAll();
        }
        #endregion

        #region 启用/禁用控制
        private static volatile bool _enabled = true;

        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public static void Enable() => Enabled = true;
        public static void Disable() => Enabled = false;
        #endregion

        /// <summary>
        /// 将按键名称字符串转换为 Keys 枚举值。
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns></returns>
        public static Keys ConvertKeyNameToEnumValue(string keyString)
        {
            return (Keys)Enum.Parse(typeof(Keys), keyString);
        }

        /// <summary>
        /// 判断指定按键是否为支持的“正确”按键。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsRightKey(Keys key)
        {
            switch (key)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.A:
                case Keys.B:
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.F:
                case Keys.G:
                case Keys.H:
                case Keys.I:
                case Keys.J:
                case Keys.K:
                case Keys.L:
                case Keys.M:
                case Keys.N:
                case Keys.O:
                case Keys.P:
                case Keys.Q:
                case Keys.R:
                case Keys.S:
                case Keys.T:
                case Keys.U:
                case Keys.V:
                case Keys.W:
                case Keys.X:
                case Keys.Y:
                case Keys.Z:
                case Keys.Space:
                case Keys.Tab:
                case Keys.Insert:
                case Keys.Home:
                case Keys.End:
                case Keys.PageUp:
                case Keys.Next:
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    return true;
                default:
                    return false;
            }
        }
    }
}