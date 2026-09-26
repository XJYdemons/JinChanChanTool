using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Tools.MouseTools
{
    public static class MouseControlTool
    {
        #region 常量声明
        public const int MOUSEEVENTF_LEFTDOWN = 0x02; // 鼠标左键按下
        public const int MOUSEEVENTF_LEFTUP = 0x04;   // 鼠标左键抬起
        private const int MOUSEEVENTF_MOVE = 0x0001;        // 鼠标移动
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;    // 使用绝对坐标
        private const int MOUSEEVENTF_VIRTUALDESK = 0x4000; // 坐标基于整个虚拟桌面而非主显示器

        /// <summary>
        /// SendInput 绝对坐标的归一化上限：0 对应虚拟桌面左/上边界，该值对应右/下边界。
        /// </summary>
        private const int AbsoluteCoordinateMaximum = 65535;

        /// <summary>
        /// 模拟鼠标点击时，左键按下与抬起之间的默认保持时间（毫秒）。
        /// </summary>
        /// <remarks>
        /// 部分游戏与模拟器会在同一输入采样周期内丢弃间隔过短的按下/抬起组合，
        /// 使点击被判定为无效。拿牌等业务会传入用户在设置中配置的保持时间覆盖此默认值。
        /// </remarks>
        public const int DefaultClickHoldMilliseconds = 30;

        /// <summary>
        /// 允许配置的鼠标按下保持时间下限（毫秒）。
        /// </summary>
        public const int MinimumClickHoldMilliseconds = 1;

        /// <summary>
        /// 允许配置的鼠标按下保持时间上限（毫秒）。
        /// </summary>
        /// <remarks>
        /// 上限用于拦截明显不合理的输入：保持时间过长会被游戏判定为长按拖拽。
        /// </remarks>
        public const int MaximumClickHoldMilliseconds = 500;

        // GetSystemMetrics 索引：虚拟桌面的边界与尺寸
        private const int SM_XVIRTUALSCREEN = 76;
        private const int SM_YVIRTUALSCREEN = 77;
        private const int SM_CXVIRTUALSCREEN = 78;
        private const int SM_CYVIRTUALSCREEN = 79;
        #endregion

        #region Win32 API 声明
        private const int INPUT_MOUSE = 0;

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public nint dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        #endregion

        #region 输入注入核心
        /// <summary>
        /// 构造一个鼠标按键输入事件。
        /// </summary>
        /// <param name="buttonFlag">按键标志，取值 MOUSEEVENTF_LEFTDOWN 或 MOUSEEVENTF_LEFTUP。</param>
        /// <returns>可直接提交给 SendInput 的输入事件。</returns>
        private static INPUT CreateButtonInput(int buttonFlag)
        {
            return new INPUT
            {
                type = INPUT_MOUSE,
                mi = new MOUSEINPUT
                {
                    dx = 0,
                    dy = 0,
                    mouseData = 0,
                    dwFlags = buttonFlag,
                    time = 0,
                    dwExtraInfo = nint.Zero
                }
            };
        }

        /// <summary>
        /// 发送单个鼠标输入事件，系统拒绝注入时记录日志并抛出异常。
        /// </summary>
        /// <param name="input">待注入的鼠标输入事件。</param>
        /// <param name="operationName">用于日志与异常信息的操作名称。</param>
        /// <exception cref="Win32Exception">SendInput 未接受该输入事件时抛出。</exception>
        private static void SendInputChecked(INPUT input, string operationName)
        {
            INPUT[] inputs = new INPUT[] { input };
            uint sentCount = SendInput(1, inputs, Marshal.SizeOf<INPUT>());
            if (sentCount == 1)
            {
                return;
            }

            // 失败通常源于目标窗口完整性级别高于本进程（UIPI 拦截），必须留痕而非静默忽略
            int errorCode = Marshal.GetLastWin32Error();
            string message = $"模拟{operationName}失败：系统未接受输入事件（Win32 错误码 {errorCode}）。";
            LogTool.Log($"[MouseControlTool] {message}");
            Debug.WriteLine($"[MouseControlTool] {message}");
            throw new Win32Exception(errorCode, message);
        }
        #endregion

        #region 公开操作
        /// <summary>
        /// 将鼠标移动到虚拟桌面中的绝对物理坐标。
        /// </summary>
        /// <param name="x">目标位置在虚拟桌面中的 X 物理坐标。</param>
        /// <param name="y">目标位置在虚拟桌面中的 Y 物理坐标。</param>
        /// <remarks>
        /// 使用 SendInput 的绝对移动而非 SetCursorPos：后者只改变光标位置，不会产生移动输入事件，
        /// 基于原始输入（Raw Input）的游戏窗口因此仍认为指针停留在旧坐标，随后的点击会落在旧位置而被忽略。
        /// </remarks>
        /// <exception cref="InvalidOperationException">无法获取虚拟桌面尺寸时抛出。</exception>
        /// <exception cref="Win32Exception">输入注入被系统拒绝时抛出。</exception>
        public static void MoveTo(int x, int y)
        {
            int virtualLeft = GetSystemMetrics(SM_XVIRTUALSCREEN);
            int virtualTop = GetSystemMetrics(SM_YVIRTUALSCREEN);
            int virtualWidth = GetSystemMetrics(SM_CXVIRTUALSCREEN);
            int virtualHeight = GetSystemMetrics(SM_CYVIRTUALSCREEN);

            if (virtualWidth <= 0 || virtualHeight <= 0)
            {
                string message = $"无法获取虚拟桌面尺寸（{virtualWidth}x{virtualHeight}），鼠标移动已取消。";
                LogTool.Log($"[MouseControlTool] {message}");
                Debug.WriteLine($"[MouseControlTool] {message}");
                throw new InvalidOperationException(message);
            }

            // 归一化除数：虚拟桌面右/下边界相对左/上边界的跨度，至少取 1 以避免除零
            int horizontalSpan = Math.Max(virtualWidth - 1, 1);
            int verticalSpan = Math.Max(virtualHeight - 1, 1);

            int normalizedX = (int)Math.Round((x - virtualLeft) * (double)AbsoluteCoordinateMaximum / horizontalSpan);
            int normalizedY = (int)Math.Round((y - virtualTop) * (double)AbsoluteCoordinateMaximum / verticalSpan);

            INPUT input = new INPUT
            {
                type = INPUT_MOUSE,
                mi = new MOUSEINPUT
                {
                    dx = Math.Clamp(normalizedX, 0, AbsoluteCoordinateMaximum),
                    dy = Math.Clamp(normalizedY, 0, AbsoluteCoordinateMaximum),
                    mouseData = 0,
                    dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_VIRTUALDESK,
                    time = 0,
                    dwExtraInfo = nint.Zero
                }
            };

            SendInputChecked(input, "移动鼠标光标");
        }

        /// <summary>
        /// 设置鼠标位置并单击左键。
        /// </summary>
        /// <param name="x">目标位置在虚拟桌面中的 X 物理坐标。</param>
        /// <param name="y">目标位置在虚拟桌面中的 Y 物理坐标。</param>
        public static void SetMousePositionAndClickLeftButton(int x, int y)
        {
            MoveTo(x, y);
            ClickLeft(DefaultClickHoldMilliseconds);
        }

        /// <summary>
        /// 设置鼠标位置。
        /// </summary>
        /// <param name="x">目标位置在虚拟桌面中的 X 物理坐标。</param>
        /// <param name="y">目标位置在虚拟桌面中的 Y 物理坐标。</param>
        public static void SetMousePosition(int x, int y)
        {
            MoveTo(x, y);
        }

        /// <summary>
        /// 鼠标左键按下。
        /// </summary>
        /// <exception cref="Win32Exception">输入注入被系统拒绝时抛出。</exception>
        public static void MakeMouseLeftButtonDown()
        {
            SendInputChecked(CreateButtonInput(MOUSEEVENTF_LEFTDOWN), "按下鼠标左键");
        }

        /// <summary>
        /// 鼠标左键抬起。
        /// </summary>
        /// <exception cref="Win32Exception">输入注入被系统拒绝时抛出。</exception>
        public static void MakeMouseLeftButtonUp()
        {
            SendInputChecked(CreateButtonInput(MOUSEEVENTF_LEFTUP), "抬起鼠标左键");
        }

        /// <summary>
        /// 完整单击一次鼠标左键，并在按下与抬起之间保持指定时间。
        /// </summary>
        /// <param name="holdMilliseconds">按下与抬起之间的保持时间（毫秒），0 表示不等待。</param>
        /// <remarks>
        /// 本方法同步等待，供不便使用 await 的调用方使用；拿牌循环使用分步接口自行异步等待。
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">保持时间为负数时抛出。</exception>
        /// <exception cref="Win32Exception">输入注入被系统拒绝时抛出。</exception>
        public static void ClickLeft(int holdMilliseconds)
        {
            if (holdMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(holdMilliseconds), holdMilliseconds, "鼠标按下保持时间不能为负数。");
            }

            MakeMouseLeftButtonDown();
            try
            {
                if (holdMilliseconds > 0)
                {
                    Thread.Sleep(holdMilliseconds);
                }
            }
            finally
            {
                // 即使等待过程被中断也必须抬起左键，避免鼠标停留在按下状态
                MakeMouseLeftButtonUp();
            }
        }
        #endregion
    }
}
