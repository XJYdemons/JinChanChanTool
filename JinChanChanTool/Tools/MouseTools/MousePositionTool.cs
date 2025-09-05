using System.Runtime.InteropServices;

namespace JinChanChanTool.Tools.MouseTools
{
    public static class MousePositionTool
    {
        //声明Windows API函数
        /* 作用：获取鼠标的物理坐标
       参数：out POINT 结构体用于接收坐标
       返回：成功获取返回true，失败false
       坐标系统：基于整个虚拟桌面的绝对坐标（多显示器时可能包含负值）*/
        [DllImport("user32.dll")]       
        private static extern bool GetCursorPos(out POINT lpPoint);

        /* 作用：获取指定显示器的DPI值
        参数：
        - hmonitor: 显示器句柄
        - dpiType: 要获取的DPI类型
        - out dpi: 输出DPI值
        返回：成功返回0，失败返回错误码*/
        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(nint hmonitor, int dpiType, out uint dpi);

         /* 作用：根据坐标点获取对应的显示器句柄
         参数：
         - pt: 要查询的坐标点
         - flags: 搜索方式标志
         返回：显示器句柄*/
        [DllImport("user32.dll")]
        private static extern nint MonitorFromPoint(POINT pt, uint flags);
       
        //定义常量
        private const int MDT_EFFECTIVE_DPI = 0;// 获取当前生效的DPI值
        private const uint MONITOR_DEFAULTTOPRIMARY = 0x00000001;

        // 定义数据结构
        // 坐标数据结构
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;// 物理坐标X
            public int Y;// 物理坐标Y
        }

        /// <summary>
        /// 获取当前鼠标的物理坐标和逻辑坐标
        /// </summary>
        public static (Point Physical, Point Logical) GetCurrentCoordinates()
        {
            // 获取物理坐标
            GetCursorPos(out POINT physicalPoint);
            Point physical = new Point(physicalPoint.X, physicalPoint.Y);

            // 获取当前显示器DPI
            nint monitor = MonitorFromPoint(physicalPoint, MONITOR_DEFAULTTOPRIMARY);
            GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out uint dpi);
            float scalingFactor = dpi / 96f;

            // 计算逻辑坐标
            Point logical = new Point(
                (int)(physical.X / scalingFactor),
                (int)(physical.Y / scalingFactor)
            );

            return (physical, logical);
        }

    }
}