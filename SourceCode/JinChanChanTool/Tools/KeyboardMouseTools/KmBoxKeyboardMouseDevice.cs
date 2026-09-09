using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using JinChanChanTool.DataClass;
using JinChanChanTool.Tools.MouseTools;

namespace JinChanChanTool.Tools.KeyboardMouseTools;

/// <summary>
/// 通过 kmboxNet UDP 协议执行键鼠操作。
/// 整体加密后发送，防止网络数据包被抓包特征识别，保护用户隐私。
/// </summary>
public sealed class KmBoxKeyboardMouseDevice : IKeyboardMouseDevice
{
    public const int DefaultPort = 0;
    public const int ConnectionTimeoutMilliseconds = 800;
    private const uint ConnectCommand = 0xaf3c2828;
    private const uint MouseMoveCommand = 0xaede7345;
    private const uint MouseLeftCommand = 0x9823ae8d;
    private const uint KeyboardCommand = 0x123c2c2f;

    private readonly object syncRoot = new();
    private UdpClient? client;
    private IPEndPoint? endpoint;
    private uint mac;
    private uint sequence;
    private int mouseButtons;
    private byte keyboardModifiers;
    private readonly byte[] keys = new byte[10];
    private bool disposed;
    private string lastError = string.Empty;

    public KeyboardMouseDeviceType DeviceType => KeyboardMouseDeviceType.KmBox;
    public string DisplayName => "KMbox";
    public bool IsAvailable { get { lock (syncRoot) return !disposed && client is not null && endpoint is not null; } }
    public bool IsConnected => IsAvailable;
    public string? IpAddress { get { lock (syncRoot) return endpoint?.Address.ToString(); } }
    public int? Port { get { lock (syncRoot) return endpoint?.Port; } }
    public string LastError { get { lock (syncRoot) return lastError; } }

    public KmBoxKeyboardMouseDevice() { }
    public KmBoxKeyboardMouseDevice(string ip, int port, string macAddress) => TryConnect(ip, port, macAddress, out _);

    public bool TryConnect(string ip, int port, string macAddress, out string error)
    {
        lock (syncRoot)
        {
            error = string.Empty;
            if (disposed) return Fail("KMbox 设备已释放。", out error);
            if (!IPAddress.TryParse(ip?.Trim(), out IPAddress? address) || address.AddressFamily != AddressFamily.InterNetwork)
                return Fail("KMbox IP 地址无效。", out error);
            if (port is < 1 or > 65535) return Fail("KMbox 端口无效。", out error);
            if (!TryParseMac(macAddress, out uint parsedMac)) return Fail("KMbox MAC 地址必须是 8 位十六进制。", out error);

            DisconnectNoLock();
            try
            {
                UdpClient candidate = new(AddressFamily.InterNetwork) { Client = { ReceiveTimeout = ConnectionTimeoutMilliseconds, SendTimeout = ConnectionTimeoutMilliseconds } };
                IPEndPoint target = new(address, port);
                candidate.Connect(target);
                mac = parsedMac;
                sequence = 0;
                client = candidate;
                endpoint = target;
                // 连接握手按协议约定使用明文 16 字节报文头，其余操作一律走加密路径。
                SendNoLock(ConnectCommand, ReadOnlySpan<byte>.Empty, true);
                lastError = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                DisconnectNoLock();
                return Fail($"连接 KMbox 失败：{ex.Message}", out error);
            }
        }
    }

    public void SetMousePosition(int x, int y)
    {
        lock (syncRoot)
        {
            Point current = MousePositionTool.GetCurrentCoordinates().Physical;

            long deltaX = (long)x - current.X;
            long deltaY = (long)y - current.Y;
            if (deltaX is < short.MinValue or > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "KMbox 单次相对移动不能超过 short 坐标范围。");
            }
            if (deltaY is < short.MinValue or > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "KMbox 单次相对移动不能超过 short 坐标范围。");
            }

            SendMouseMove((short)deltaX, (short)deltaY);
        }
    }
    public void SetMousePositionAndClickLeftButton(int x, int y) { SetMousePosition(x, y); MouseLeftButtonDown(); Thread.Sleep(2); MouseLeftButtonUp(); }
    public void MouseLeftButtonDown() { mouseButtons |= 1; SendMouse(MouseLeftCommand); }
    public void MouseLeftButtonUp() { mouseButtons &= ~1; SendMouse(MouseLeftCommand); }
    public void KeyDown(Keys key) { UpdateKey(key, true); SendKeyboard(); }
    public void KeyUp(Keys key) { UpdateKey(key, false); SendKeyboard(); }
    public void PressKey(Keys key) { KeyDown(key); Thread.Sleep(10); KeyUp(key); }
    public void PressKey(string keyName)
    {
        if (!Enum.TryParse(keyName, true, out Keys key)) throw new ArgumentException("无效的键名。", nameof(keyName));
        PressKey(key);
    }

    private void SendMouseMove(short x, short y)
    {       
        Span<byte> payload = stackalloc byte[56];
        payload.Clear();
        BinaryPrimitives.WriteInt32LittleEndian(payload[4..8], x);
        BinaryPrimitives.WriteInt32LittleEndian(payload[8..12], y);
        SendEncrypted(MouseMoveCommand, payload);
    }
    private void SendMouse(uint command)
    {       
        Span<byte> payload = stackalloc byte[56];
        payload.Clear();
        BinaryPrimitives.WriteInt32LittleEndian(payload, mouseButtons);
        SendEncrypted(command, payload);
    }
    private void SendKeyboard()
    {        
        Span<byte> payload = stackalloc byte[12];
        payload.Clear();
        payload[0] = keyboardModifiers;
        keys.CopyTo(payload[2..]);
        SendEncrypted(KeyboardCommand, payload);
    }
    private void UpdateKey(Keys key, bool down)
    {
        int hid = ToHid(key);
        if (hid <= 0) throw new ArgumentException($"KMbox 不支持按键 {key}。", nameof(key));
        if (hid >= 0xe0 && hid <= 0xe7)
        {
            byte mask = (byte)(1 << (hid - 0xe0));
            if (down) keyboardModifiers |= mask; else keyboardModifiers &= (byte)~mask;
            return;
        }
        int index = Array.IndexOf(keys, (byte)hid);
        if (down) { if (index < 0) { index = Array.IndexOf(keys, (byte)0); if (index >= 0) keys[index] = (byte)hid; } }
        else if (index >= 0) Array.Copy(keys, index + 1, keys, index, keys.Length - index - 1);
    }
    /// <summary>
    /// 以加密报文发送键鼠操作。所有操作必须经由本方法发送， 
    /// 无法通过抓包特征识别盒子行为。
    /// </summary>
    private void SendEncrypted(uint command, ReadOnlySpan<byte> payload)
    {
        lock (syncRoot)
        {
            if (!IsAvailable) throw new InvalidOperationException("KMbox 尚未连接。");
            SendNoLock(command, payload, false, true);
        }
    }
    /// <summary>
    /// 组装并发送一个 kmboxNet 报文。报文头为 {mac, rand, indexpts, cmd}（均为小端 uint32），
    /// 加密报文固定 128 字节。仅连接握手使用明文；其余操作必须传入 encrypted=true。
    /// </summary>
    private void SendNoLock(uint command, ReadOnlySpan<byte> payload, bool connect, bool encrypted = false)
    {
        byte[] packet = new byte[encrypted ? 128 : 16 + payload.Length];
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(0, 4), mac);
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(4, 4), (uint)Random.Shared.Next());
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(8, 4), ++sequence);
        BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(12, 4), command);
        payload.CopyTo(packet.AsSpan(16));
        if (encrypted)
        {
            EncryptPacket(packet, mac);
        }
        client!.Send(packet, packet.Length);
        try { client.Receive(ref endpoint!); }
        catch (SocketException ex) { if (connect) throw new InvalidOperationException("未收到 KMbox 响应。", ex); throw new InvalidOperationException("KMbox 数据发送失败。", ex); }
    }

    /// <summary>
    /// 对 128 字节报文整体加密。
    /// 完全一致：XXTEA 变体，6 轮迭代，delta=0x9E3779B9，密钥由盒子 MAC 派生
    /// （前 4 字节为 MAC 的大端字节序，其余 12 字节为 0）。
    /// </summary>
    private static void EncryptPacket(byte[] packet, uint macAddress)
    {
        Span<uint> data = stackalloc uint[32];
        for (int index = 0; index < data.Length; index++)
        {
            data[index] = BinaryPrimitives.ReadUInt32LittleEndian(packet.AsSpan(index * sizeof(uint), sizeof(uint)));
        }

        Span<byte> keyBytes = stackalloc byte[16];
        // 显式清零：避免依赖栈内存默认清零行为。
        keyBytes.Clear();
        BinaryPrimitives.WriteUInt32BigEndian(keyBytes, macAddress);
        Span<uint> key = stackalloc uint[4];
        for (int index = 0; index < key.Length; index++)
        {
            key[index] = BinaryPrimitives.ReadUInt32LittleEndian(keyBytes[(index * sizeof(uint))..]);
        }

        const uint delta = 2654435769u;
        uint sum = 0;
        uint previous = data[31];
        for (int round = 0; round < 6; round++)
        {
            sum += delta;
            uint keyIndex = (sum >> 2) & 3;
            for (int index = 0; index < data.Length - 1; index++)
            {
                uint next = data[index + 1];
                data[index] += ((previous >> 5 ^ next << 2) + (next >> 3 ^ previous << 4)) ^
                               ((sum ^ next) + (key[((index & 3) ^ (int)keyIndex)] ^ previous));
                previous = data[index];
            }

            uint first = data[0];
            data[^1] += ((previous >> 5 ^ first << 2) + (first >> 3 ^ previous << 4)) ^
                        ((sum ^ first) + (key[((data.Length - 1) & 3) ^ (int)keyIndex] ^ previous));
            previous = data[^1];
        }

        for (int index = 0; index < data.Length; index++)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(packet.AsSpan(index * sizeof(uint), sizeof(uint)), data[index]);
        }
    }
    private static bool TryParseMac(string? value, out uint result)
    {
        result = 0;
        string normalized = value?.Trim().Replace("-", string.Empty).Replace(":", string.Empty) ?? string.Empty;
        return normalized.Length == 8 && uint.TryParse(normalized, System.Globalization.NumberStyles.HexNumber, null, out result);
    }
    private bool Fail(string message, out string error) { lastError = error = message; return false; }
    private void DisconnectNoLock()
    {
        client?.Dispose();
        client = null;
        endpoint = null;
        Array.Clear(keys);
        mouseButtons = 0;
        keyboardModifiers = 0;
    }
    public void Dispose() { lock (syncRoot) { if (!disposed) { disposed = true; DisconnectNoLock(); } } }

    private static int ToHid(Keys key)
    {
        key &= Keys.KeyCode;
        if (key is >= Keys.A and <= Keys.Z) return 4 + ((int)key - (int)Keys.A);
        if (key is >= Keys.D0 and <= Keys.D9) return 0x1e + ((int)key - (int)Keys.D0);
        return key switch { Keys.LControlKey => 0xe0, Keys.LShiftKey => 0xe1, Keys.LMenu => 0xe2, Keys.RControlKey => 0xe4, Keys.RShiftKey => 0xe5, Keys.RMenu => 0xe6, Keys.Enter => 0x28, Keys.Escape => 0x29, Keys.Back => 0x2a, Keys.Tab => 0x2b, Keys.Space => 0x2c, Keys.F1 => 0x3a, Keys.F2 => 0x3b, Keys.F3 => 0x3c, Keys.F4 => 0x3d, Keys.F5 => 0x3e, Keys.F6 => 0x3f, Keys.F7 => 0x40, Keys.F8 => 0x41, Keys.F9 => 0x42, Keys.F10 => 0x43, Keys.F11 => 0x44, Keys.F12 => 0x45, Keys.Left => 0x50, Keys.Right => 0x4f, Keys.Up => 0x52, Keys.Down => 0x51, Keys.Delete => 0x4c, Keys.Home => 0x4a, Keys.End => 0x4d, Keys.PageUp => 0x4b, Keys.PageDown => 0x4e, _ => 0 };
    }
}
