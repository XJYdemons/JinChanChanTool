using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JinChanChanTool.Tools.KeyBoardTools
{
    public static class KeyboardControlTool
    {
        // 键盘事件常量
        private const int KEYEVENTF_KEYDOWN = 0x0000; // 按键按下
        private const int KEYEVENTF_KEYUP = 0x0002;   // 按键释放
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001; // 扩展键标识

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        /// <summary>
        /// 模拟单个按键的按下和释放（完整敲击）
        /// </summary>
        /// <param name="key">Keys枚举值</param>
        public static void PressKey(Keys key)
        {
            KeyDown(key);
            KeyUp(key);
        }

        /// <summary>
        /// 通过字符串键名模拟按键按下和释放(键名调用)
        /// </summary>
        /// <param name="keyName">键名字符串（如"F1"）</param>
        public static void PressKey(string keyName)
        {
            Keys key = ConvertKeyNameToEnumValue(keyName);
            PressKey(key);
        }
        /// <summary>
        /// 模拟按键按下
        /// </summary>
        /// <param name="key">Keys枚举值</param>
        public static void KeyDown(Keys key)
        {
            // 获取扫描码
            uint scanCode = MapVirtualKey((uint)key, 0);
            keybd_event((byte)key, (byte)scanCode, KEYEVENTF_KEYDOWN, 0);
        }

        /// <summary>
        /// 模拟按键按下(键名调用)
        /// </summary>
        /// <param name="key">Keys枚举值</param>
        public static void KeyDown(string keyName)
        {
            Keys key = ConvertKeyNameToEnumValue(keyName);
            KeyDown(key);
        }

        /// <summary>
        /// 模拟按键释放
        /// </summary>
        /// <param name="key">Keys枚举值</param>
        public static void KeyUp(Keys key)
        {
            // 获取扫描码
            uint scanCode = MapVirtualKey((uint)key, 0);
            keybd_event((byte)key, (byte)scanCode, KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// 模拟按键释放(键名调用)
        /// </summary>
        /// <param name="key">Keys枚举值</param>
        public static void KeyUp(string keyName)
        {
            Keys key = ConvertKeyNameToEnumValue(keyName);
            KeyUp(key);
        }

        

        /// <summary>
        /// 将字符串键名转换为Keys枚举值
        /// </summary>
        /// <param name="keyString">键名字符串</param>
        /// <returns>对应的Keys枚举值</returns>
        public static Keys ConvertKeyNameToEnumValue(string keyString)
        {
            return (Keys)Enum.Parse(typeof(Keys), keyString);
        }

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