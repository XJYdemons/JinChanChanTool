using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace 金铲铲助手
{

    public class ControlTools
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
        public static void SetMousePositionAndClickLeftButton(int x, int y, Screen[] screens, int ID)
        {
            // 获取指定显示器的工作区域
            Rectangle screenBounds = screens[ID].Bounds;
            int offsetX = screenBounds.Left;
            int offsetY = screenBounds.Top;
            // 移动鼠标到指定的坐标点
            SetCursorPos(x+ offsetX, y+ offsetY);
           
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
        public static void SetMousePosition(int x,int y, Screen[] screens, int ID)
        {  
            // 获取指定显示器的工作区域
            Rectangle screenBounds = screens[ID].Bounds;
            int offsetX = screenBounds.Left;
            int offsetY = screenBounds.Top;
            // 移动鼠标到指定的坐标点
            SetCursorPos(x+offsetX, y+offsetY);
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
