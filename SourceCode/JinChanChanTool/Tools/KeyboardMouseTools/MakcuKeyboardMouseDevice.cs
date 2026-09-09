using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using JinChanChanTool.DataClass;

namespace JinChanChanTool.Tools.KeyboardMouseTools
{
    /// <summary>
    /// 通过 Makcu 串口协议执行键鼠操作。
    /// </summary>
    public sealed class MakcuKeyboardMouseDevice : IKeyboardMouseDevice
    {
        public const int DefaultBaudRate = 115200;
        public const int ConnectionTimeoutMilliseconds = 500;

        private const int LeftClickReleaseDelayMilliseconds = 2;
        private const int SerialReadTimeoutMilliseconds = 50;
        private const string LineTerminator = "\r\n";

        private readonly object syncRoot = new();
        private SerialPort? serialPort;
        private bool disposed;
        private bool leftButtonPressed;
        private string lastError = string.Empty;

        public KeyboardMouseDeviceType DeviceType => KeyboardMouseDeviceType.Makcu;

        public string DisplayName => "Makcu";

        public bool IsAvailable
        {
            get
            {
                lock (syncRoot)
                {
                    return !disposed && OperatingSystem.IsWindows() && serialPort?.IsOpen == true;
                }
            }
        }

        /// <summary>
        /// 与参考 Makcu 实现兼容的连接状态别名。
        /// </summary>
        public bool IsConnected => IsAvailable;

        /// <summary>
        /// 当前连接的串口名称；未连接时为 null。
        /// </summary>
        public string? PortName
        {
            get
            {
                lock (syncRoot)
                {
                    return serialPort?.IsOpen == true ? serialPort.PortName : null;
                }
            }
        }

        /// <summary>
        /// 当前连接使用的波特率；未连接时为 null。
        /// </summary>
        public int? BaudRate
        {
            get
            {
                lock (syncRoot)
                {
                    return serialPort?.IsOpen == true ? serialPort.BaudRate : null;
                }
            }
        }

        /// <summary>
        /// 最近一次连接或发送失败的错误信息。
        /// </summary>
        public string LastError
        {
            get
            {
                lock (syncRoot)
                {
                    return lastError;
                }
            }
        }

        public MakcuKeyboardMouseDevice()
        {
        }

        /// <summary>
        /// 创建设备并尝试连接指定串口。连接失败时可通过 <see cref="LastError"/> 获取原因。
        /// </summary>
        public MakcuKeyboardMouseDevice(string portName, int baudRate = DefaultBaudRate)
        {
            TryConnect(portName, baudRate, out _);
        }

        /// <summary>
        /// 创建设备并尝试探测所有串口。适合应用启动时使用。
        /// </summary>
        public static MakcuKeyboardMouseDevice CreateAutoConnected(int baudRate = DefaultBaudRate)
        {
            MakcuKeyboardMouseDevice device = new();
            device.TryAutoConnect(baudRate, out _);
            return device;
        }

        public static string[] GetPortNames()
        {
            try
            {
                return SerialPort.GetPortNames()
                    .OrderBy(GetPortNumber)
                    .ThenBy(port => port, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 连接指定的 Makcu 串口并通过版本命令校验设备。
        /// </summary>
        public bool TryConnect(string portName, int baudRate, out string error)
        {
            lock (syncRoot)
            {
                error = string.Empty;

                if (disposed)
                {
                    error = "Makcu 设备已释放。";
                    SetLastErrorNoLock(error);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(portName))
                {
                    error = "尚未选择 Makcu 串口。";
                    SetLastErrorNoLock(error);
                    return false;
                }

                if (baudRate <= 0)
                {
                    error = "Makcu 波特率无效。";
                    SetLastErrorNoLock(error);
                    return false;
                }

                if (!OperatingSystem.IsWindows())
                {
                    error = "Makcu 设备仅支持 Windows。";
                    SetLastErrorNoLock(error);
                    return false;
                }

                portName = portName.Trim();
                if (serialPort?.IsOpen == true &&
                    string.Equals(serialPort.PortName, portName, StringComparison.OrdinalIgnoreCase) &&
                    serialPort.BaudRate == baudRate)
                {
                    try
                    {
                        if (VerifyDeviceNoLock(out error))
                        {
                            SetLastErrorNoLock(string.Empty);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = $"验证 Makcu 串口 {portName} 失败：{ex.Message}";
                    }

                    DisconnectNoLock();
                    SetLastErrorNoLock(error);
                    return false;
                }

                DisconnectNoLock();

                SerialPort? candidate = null;
                try
                {
                    candidate = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
                    {
                        Encoding = Encoding.ASCII,
                        ReadTimeout = SerialReadTimeoutMilliseconds,
                        WriteTimeout = ConnectionTimeoutMilliseconds,
                        DtrEnable = false,
                        RtsEnable = false,
                        NewLine = LineTerminator
                    };

                    candidate.Open();
                    candidate.DiscardInBuffer();
                    candidate.DiscardOutBuffer();
                    serialPort = candidate;

                    if (!VerifyDeviceNoLock(out error))
                    {
                        DisconnectNoLock();
                        SetLastErrorNoLock(error);
                        return false;
                    }

                    // 关闭回显，避免自动操作期间串口接收缓冲区持续堆积。
                    candidate.Write("km.echo(0)" + LineTerminator);
                    leftButtonPressed = false;
                    SetLastErrorNoLock(string.Empty);
                    return true;
                }
                catch (Exception ex)
                {
                    error = $"打开 Makcu 串口 {portName} 失败：{ex.Message}";
                    DisconnectNoLock();
                    SetLastErrorNoLock(error);
                    return false;
                }
                finally
                {
                    if (candidate is not null && !ReferenceEquals(serialPort, candidate))
                    {
                        candidate.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 按默认波特率探测所有可用串口。
        /// </summary>
        public bool TryAutoConnect(out string error)
        {
            return TryAutoConnect(DefaultBaudRate, out error);
        }

        /// <summary>
        /// 按指定波特率探测所有可用串口。
        /// </summary>
        public bool TryAutoConnect(int baudRate, out string error)
        {
            error = string.Empty;

            if (baudRate <= 0)
            {
                error = "Makcu 波特率无效。";
                SetLastError(error);
                return false;
            }

            lock (syncRoot)
            {
                if (disposed)
                {
                    error = "Makcu 设备已释放。";
                    SetLastErrorNoLock(error);
                    return false;
                }

                if (serialPort?.IsOpen == true && serialPort.BaudRate == baudRate)
                {
                    try
                    {
                        if (VerifyDeviceNoLock(out error))
                        {
                            SetLastErrorNoLock(string.Empty);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = $"验证 Makcu 串口 {serialPort.PortName} 失败：{ex.Message}";
                    }

                    DisconnectNoLock();
                    SetLastErrorNoLock(error);
                }

                if (serialPort?.IsOpen == true)
                {
                    DisconnectNoLock();
                }
            }

            string[] ports = GetPortNames();
            if (ports.Length == 0)
            {
                error = "未找到可用的 Makcu 串口。";
                SetLastError(error);
                return false;
            }

            List<string> failures = new();
            foreach (string port in ports)
            {
                if (TryConnect(port, baudRate, out string attemptError))
                {
                    error = string.Empty;
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(attemptError))
                {
                    failures.Add($"{port}: {attemptError}");
                }
            }

            error = failures.Count == 0
                ? "未找到响应的 Makcu 设备。"
                : $"未找到响应的 Makcu 设备。{string.Join("；", failures)}";
            SetLastError(error);
            return false;
        }

        public void SetMousePosition(int x, int y)
        {
            ThrowIfFailed(TrySetMousePosition(x, y, out string error), error);
        }

        public void SetMousePositionAndClickLeftButton(int x, int y)
        {
            SetMousePosition(x, y);
            MouseLeftButtonDown();
            Thread.Sleep(LeftClickReleaseDelayMilliseconds);
            MouseLeftButtonUp();
        }

        public void MouseLeftButtonDown()
        {
            ThrowIfFailed(TryLeftButtonDown(out string error), error);
        }

        public void MouseLeftButtonUp()
        {
            ThrowIfFailed(TryLeftButtonUp(out string error), error);
        }

        public void KeyDown(Keys key)
        {
            ThrowIfFailed(TryKeyDown(key, out string error), error);
        }

        public void KeyUp(Keys key)
        {
            ThrowIfFailed(TryKeyUp(key, out string error), error);
        }

        public void PressKey(Keys key)
        {
            ThrowIfFailed(TryPressKey(key, out string error), error);
        }

        public void PressKey(string keyName)
        {
            ThrowIfFailed(TryPressKey(keyName, out string error), error);
        }

        /// <summary>
        /// 将绝对屏幕坐标转换为 Makcu 支持的相对移动命令。
        /// </summary>
        public bool TrySetMousePosition(int x, int y, out string error)
        {
            lock (syncRoot)
            {
                if (!IsConnectedNoLock())
                {
                    error = GetNotConnectedErrorNoLock();
                    SetLastErrorNoLock(error);
                    return false;
                }
            }

            Point currentPosition = Cursor.Position;
            long deltaX = (long)x - currentPosition.X;
            long deltaY = (long)y - currentPosition.Y;
            if (deltaX < int.MinValue || deltaX > int.MaxValue ||
                deltaY < int.MinValue || deltaY > int.MaxValue)
            {
                error = "Makcu 相对移动距离超出坐标范围。";
                SetLastError(error);
                return false;
            }

            return TryMove((int)deltaX, (int)deltaY, out error);
        }

        /// <summary>
        /// 将相对位移拆分为 Makcu 协议支持的短整型分段移动命令。
        /// </summary>
        public bool TryMove(int deltaX, int deltaY, out string error)
        {
            StringBuilder commands = new();
            int remainingX = deltaX;
            int remainingY = deltaY;

            do
            {
                int stepX = Math.Clamp(remainingX, short.MinValue, short.MaxValue);
                int stepY = Math.Clamp(remainingY, short.MinValue, short.MaxValue);
                commands.Append("km.move(")
                    .Append(stepX)
                    .Append(',')
                    .Append(stepY)
                    .Append(')')
                    .Append(LineTerminator);
                remainingX -= stepX;
                remainingY -= stepY;
            }
            while (remainingX != 0 || remainingY != 0);

            return TrySend(commands.ToString(), out error);
        }

        public bool TryLeftButtonDown(out string error)
        {
            return TrySetLeftButtonState(true, out error);
        }

        public bool TryLeftButtonUp(out string error)
        {
            return TrySetLeftButtonState(false, out error);
        }

        public bool TryLeftClick(out string error)
        {
            return TryClickLeftButton(out error);
        }

        public bool TryClickLeftButton(out string error)
        {
            if (!TryLeftButtonDown(out error))
            {
                return false;
            }

            Thread.Sleep(LeftClickReleaseDelayMilliseconds);
            return TryLeftButtonUp(out error);
        }

        public bool TryKeyDown(Keys key, out string error)
        {
            return TryKeyCommand("keydown", key, out error);
        }

        public bool TryKeyUp(Keys key, out string error)
        {
            return TryKeyCommand("keyup", key, out error);
        }

        public bool TryPressKey(Keys key, out string error)
        {
            return TryKeyCommand("press", key, out error);
        }

        public bool TryKeyDown(string keyName, out string error)
        {
            return TryKeyCommand("keydown", keyName, out error);
        }

        public bool TryKeyUp(string keyName, out string error)
        {
            return TryKeyCommand("keyup", keyName, out error);
        }

        public bool TryPressKey(string keyName, out string error)
        {
            return TryKeyCommand("press", keyName, out error);
        }

        public void Disconnect()
        {
            lock (syncRoot)
            {
                DisconnectNoLock();
            }
        }

        public void Dispose()
        {
            lock (syncRoot)
            {
                if (disposed)
                {
                    return;
                }

                DisconnectNoLock();
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        private bool TryKeyCommand(string commandName, Keys key, out string error)
        {
            if (!TryGetKeyName(key, out string keyName, out error))
            {
                SetLastError(error);
                return false;
            }

            return TryKeyCommand(commandName, keyName, out error);
        }

        private bool TryKeyCommand(string commandName, string keyName, out string error)
        {
            if (!TryNormalizeKeyName(keyName, out string normalizedKeyName, out error))
            {
                SetLastError(error);
                return false;
            }

            return TrySend($"km.{commandName}('{normalizedKeyName}'){LineTerminator}", out error);
        }

        private bool TrySetLeftButtonState(bool isPressed, out string error)
        {
            lock (syncRoot)
            {
                error = string.Empty;
                if (!IsConnectedNoLock())
                {
                    error = GetNotConnectedErrorNoLock();
                    SetLastErrorNoLock(error);
                    return false;
                }

                try
                {
                    DiscardPendingInputNoLock();
                    serialPort!.Write(isPressed ? "km.left(1)" + LineTerminator : "km.left(0)" + LineTerminator);
                    leftButtonPressed = isPressed;
                    SetLastErrorNoLock(string.Empty);
                    return true;
                }
                catch (Exception ex)
                {
                    TryReleaseLeftButtonNoLock();
                    error = $"Makcu 串口写入失败：{ex.Message}";
                    DisconnectNoLock();
                    SetLastErrorNoLock(error);
                    return false;
                }
            }
        }

        private bool TrySend(string commands, out string error)
        {
            lock (syncRoot)
            {
                error = string.Empty;
                if (!IsConnectedNoLock())
                {
                    error = GetNotConnectedErrorNoLock();
                    SetLastErrorNoLock(error);
                    return false;
                }

                try
                {
                    DiscardPendingInputNoLock();
                    serialPort!.Write(commands);
                    SetLastErrorNoLock(string.Empty);
                    return true;
                }
                catch (Exception ex)
                {
                    TryReleaseLeftButtonNoLock();
                    error = $"Makcu 串口写入失败：{ex.Message}";
                    DisconnectNoLock();
                    SetLastErrorNoLock(error);
                    return false;
                }
            }
        }

        private bool VerifyDeviceNoLock(out string error)
        {
            error = string.Empty;
            serialPort!.DiscardInBuffer();
            serialPort.Write("km.version()" + LineTerminator);

            StringBuilder response = new();
            Stopwatch timer = Stopwatch.StartNew();
            while (timer.ElapsedMilliseconds < ConnectionTimeoutMilliseconds)
            {
                if (serialPort.BytesToRead > 0)
                {
                    response.Append(serialPort.ReadExisting());
                    if (response.ToString().Contains("km.MAKCU", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                Thread.Sleep(10);
            }

            error = $"串口 {serialPort.PortName} 已打开，但未收到 Makcu 设备响应。请检查串口和波特率。";
            return false;
        }

        private void DiscardPendingInputNoLock()
        {
            if (IsConnectedNoLock() && serialPort!.BytesToRead > 0)
            {
                serialPort.DiscardInBuffer();
            }
        }

        private void TryReleaseLeftButtonNoLock()
        {
            try
            {
                if (IsConnectedNoLock())
                {
                    serialPort!.Write("km.left(0)" + LineTerminator);
                }
            }
            catch (Exception ex)
            {
                SetLastErrorNoLock($"释放 Makcu 左键状态失败：{ex.Message}");
            }
            finally
            {
                leftButtonPressed = false;
            }
        }

        private void DisconnectNoLock()
        {
            if (serialPort == null)
            {
                leftButtonPressed = false;
                return;
            }

            if (leftButtonPressed)
            {
                TryReleaseLeftButtonNoLock();
            }
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                SetLastErrorNoLock($"关闭 Makcu 串口失败：{ex.Message}");
            }
            finally
            {
                serialPort.Dispose();
                serialPort = null;
                leftButtonPressed = false;
            }
        }

        private bool IsConnectedNoLock()
        {
            return !disposed && serialPort?.IsOpen == true;
        }

        private string GetNotConnectedErrorNoLock()
        {
            return disposed ? "Makcu 设备已释放。" : "Makcu 尚未连接。";
        }

        private void SetLastError(string error)
        {
            lock (syncRoot)
            {
                SetLastErrorNoLock(error);
            }
        }

        private void SetLastErrorNoLock(string error)
        {
            lastError = error;
        }

        private static void ThrowIfFailed(bool succeeded, string error)
        {
            if (!succeeded)
            {
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(error) ? "Makcu 操作失败。" : error);
            }
        }

        private static bool TryNormalizeKeyName(string? keyName, out string normalizedKeyName, out string error)
        {
            normalizedKeyName = keyName?.Trim() ?? string.Empty;
            if (normalizedKeyName.Length == 0)
            {
                error = "Makcu 按键名称不能为空。";
                return false;
            }

            if (normalizedKeyName.Length > 32)
            {
                error = "Makcu 按键名称过长。";
                return false;
            }

            foreach (char character in normalizedKeyName)
            {
                bool isAsciiLetter = character is >= 'A' and <= 'Z' || character is >= 'a' and <= 'z';
                bool isAsciiDigit = character is >= '0' and <= '9';
                if (!isAsciiLetter && !isAsciiDigit && character != '_' && character != '-')
                {
                    error = "Makcu 按键名称包含不支持的字符。";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        private static bool TryGetKeyName(Keys key, out string keyName, out string error)
        {
            keyName = string.Empty;
            if (key == Keys.None)
            {
                error = "Makcu 按键不能为空。";
                return false;
            }

            if ((key & Keys.Modifiers) != Keys.None)
            {
                error = "Makcu 不支持将多个按键作为一个按键发送。";
                return false;
            }

            keyName = (key & Keys.KeyCode).ToString();
            return TryNormalizeKeyName(keyName, out keyName, out error);
        }

        private static int GetPortNumber(string portName)
        {
            if (portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(portName.AsSpan(3), out int number))
            {
                return number;
            }

            return int.MaxValue;
        }
    }
}
