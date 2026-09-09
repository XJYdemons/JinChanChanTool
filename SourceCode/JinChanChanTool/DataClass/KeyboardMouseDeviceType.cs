namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 键鼠操作设备类型。
    /// </summary>
    public enum KeyboardMouseDeviceType
    {
        /// <summary>
        /// 使用当前 Windows API 模拟输入。
        /// </summary>
        WinApi = 0,

        /// <summary>
        /// MAKCU 硬件盒子。
        /// </summary>
        Makcu = 1,

        /// <summary>
        /// KMbox 硬件盒子。
        /// </summary>
        KmBox = 2
    }
}
