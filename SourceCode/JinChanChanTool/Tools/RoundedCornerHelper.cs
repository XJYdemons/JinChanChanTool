using System.Runtime.InteropServices;

namespace JinChanChanTool.Tools
{
    /// <summary>
    /// 无边框窗体的圆角辅助类。
    /// 统一封装 GDI32 的圆角区域创建与设置逻辑，避免各窗体重复实现时遗漏 GDI 句柄释放。
    /// </summary>
    public static class RoundedCornerHelper
    {
        /// <summary>
        /// 创建圆角矩形区域。由 gdi32.dll 导出。
        /// </summary>
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        /// <summary>
        /// 为窗口设置显示区域。由 user32.dll 导出。
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        /// <summary>
        /// 删除 GDI 对象。由 gdi32.dll 导出。
        /// </summary>
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// 默认圆角半径（逻辑像素）。
        /// </summary>
        public const int DefaultCornerRadius = 16;

        /// <summary>
        /// 为指定窗体应用圆角显示区域。
        /// </summary>
        /// <param name="form">目标窗体</param>
        /// <param name="cornerRadius">圆角半径，默认 <see cref="DefaultCornerRadius"/></param>
        /// <remarks>
        /// 调用方需保证窗体句柄已创建。尺寸无效或窗体已释放时该方法不产生任何效果。
        /// </remarks>
        public static void Apply(Form form, int cornerRadius = DefaultCornerRadius)
        {
            if (form == null || form.IsDisposed || !form.IsHandleCreated)
            {
                return;
            }

            int width = form.Width;
            int height = form.Height;
            if (width <= 0 || height <= 0)
            {
                // 尺寸无效时创建区域会得到空区域，使窗口完全不可见，因此直接跳过。
                return;
            }

            IntPtr region = IntPtr.Zero;
            try
            {
                region = CreateRoundRectRgn(0, 0, width, height, cornerRadius, cornerRadius);
                if (region == IntPtr.Zero)
                {
                    return;
                }

                // SetWindowRgn 成功时接管 region 的所有权，不需要也不能再手动删除；
                // 失败时所有权仍属于调用方，必须自行释放，否则会泄漏 GDI 句柄。
                if (SetWindowRgn(form.Handle, region, true) == 0)
                {
                    DeleteObject(region);
                }
            }
            catch
            {
                // 圆角失败时保留正常窗体显示。
                if (region != IntPtr.Zero)
                {
                    DeleteObject(region);
                }
            }
        }
    }
}
