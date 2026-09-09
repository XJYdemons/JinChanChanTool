using JinChanChanTool.DataClass;

namespace JinChanChanTool.Tools.KeyboardMouseTools
{
    /// <summary>
    /// 创建键鼠操作设备实例。
    /// </summary>
    public static class KeyboardMouseDeviceFactory
    {
        /// <summary>
        /// 创建指定类型的设备。
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// 指定设备类型尚未提供实现。
        /// </exception>
        public static IKeyboardMouseDevice Create(KeyboardMouseDeviceType deviceType)
        {
            return deviceType switch
            {
                KeyboardMouseDeviceType.WinApi => new WinApiKeyboardMouseDevice(),
                // 普通创建只构造设备对象，不自动扫描串口。设置页面会调用此方法读取设备元数据，
                // 避免页面初始化时占用 Makcu 串口。
                KeyboardMouseDeviceType.Makcu => new MakcuKeyboardMouseDevice(),
                KeyboardMouseDeviceType.KmBox => new KmBoxKeyboardMouseDevice(),
                _ => throw new NotSupportedException($"键鼠设备“{deviceType}”尚未实现。")
            };
        }

        /// <summary>
        /// 创建设置中指定的设备；当配置来自旧版本或包含未知枚举值时使用 WinAPI。
        /// Makcu 使用传入的串口和波特率连接；未指定串口时自动探测。
        /// </summary>
        public static IKeyboardMouseDevice CreateOrFallback(
            KeyboardMouseDeviceType deviceType,
            string? makcuPortName = null,
            int makcuBaudRate = MakcuKeyboardMouseDevice.DefaultBaudRate,
            string? kmBoxIp = null,
            int kmBoxPort = KmBoxKeyboardMouseDevice.DefaultPort,
            string? kmBoxMac = null)
        {
            if (!Enum.IsDefined(deviceType))
            {
                return Create(KeyboardMouseDeviceType.WinApi);
            }

            return deviceType switch
            {
                KeyboardMouseDeviceType.Makcu => CreateMakcu(makcuPortName, makcuBaudRate),
                KeyboardMouseDeviceType.KmBox => CreateKmBox(kmBoxIp, kmBoxPort, kmBoxMac),
                _ => Create(deviceType)
            };
        }

        private static KmBoxKeyboardMouseDevice CreateKmBox(string? ip, int port, string? mac)
        {
            KmBoxKeyboardMouseDevice device = new();
            if (!string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(mac))
                device.TryConnect(ip, port, mac, out _);
            return device;
        }

        private static MakcuKeyboardMouseDevice CreateMakcu(string? portName, int baudRate)
        {
            MakcuKeyboardMouseDevice device = new();
            if (string.IsNullOrWhiteSpace(portName))
            {
                device.TryAutoConnect(baudRate, out _);
            }
            else
            {
                device.TryConnect(portName, baudRate, out _);
            }

            return device;
        }

        /// <summary>
        /// 判断设备类型是否已有可用实现。
        /// </summary>
        public static bool IsSupported(KeyboardMouseDeviceType deviceType)
        {
            return deviceType is KeyboardMouseDeviceType.WinApi or KeyboardMouseDeviceType.Makcu or KeyboardMouseDeviceType.KmBox;
        }

        /// <summary>
        /// 获取当前版本已经提供实现的键鼠设备类型。
        /// </summary>
        public static IReadOnlyList<KeyboardMouseDeviceType> GetSupportedDeviceTypes()
        {
            return Enum.GetValues<KeyboardMouseDeviceType>()
                .Where(IsSupported)
                .ToArray();
        }
    }
}
