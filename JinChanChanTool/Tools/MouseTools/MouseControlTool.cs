using System.Runtime.InteropServices;

namespace JinChanChanTool.Tools.MouseTools
{
    public static class MouseControlTool
    {
        #region 添加鼠标按键声明
        public const int MOUSEEVENTF_LEFTDOWN = 0x02; // 鼠标左键按下
        public const int MOUSEEVENTF_LEFTUP = 0x04;   // 鼠标左键抬起
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        #endregion
        static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        
        /// <summary>
        /// 设置鼠标位置并单击左键
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="screens"></param>
        /// <param name="ID"></param>
        public static void SetMousePositionAndClickLeftButton(int x, int y)
        {            
            SetCursorPos(x, y);

            // 模拟鼠标左键按下
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);

            // 模拟鼠标左键抬起
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }        

        /// <summary>
        /// 设置鼠标位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="screens"></param>
        /// <param name="ID"></param>
        public static void SetMousePosition(int x, int y)
        {                       
            SetCursorPos(x, y);
        }

        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        public  static void MakeMouseLeftButtonDown()
        {
            // 模拟鼠标左键按下
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        /// <summary>
        /// 鼠标左键抬起
        /// </summary>
        public static void MakeMouseLeftButtonUp()
        {
            // 模拟鼠标左键抬起
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }       
    }
}
