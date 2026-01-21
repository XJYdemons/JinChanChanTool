using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JinChanChanTool.Tools
{
    /// <summary>
    /// 拖动辅助类 - 让任何控件都能拖动其父窗口
    /// </summary>
    public static class DragHelper
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF012;
        private const int HTCAPTION = 0x0002;

        /// <summary>
        /// 让指定控件可以拖动其所在的窗口
        /// </summary>
        /// <param name="control">需要启用拖动的控件</param>
        /// <param name="mouseButton">响应的鼠标按钮（默认左键）</param>
        /// <example>
        /// // 让 label1 可以拖动窗口
        /// DragHelper.EnableDrag(label1);
        ///
        /// // 让 panel1 只响应右键拖动
        /// DragHelper.EnableDrag(panel1, MouseButtons.Right);
        /// </example>
        public static void EnableDrag(Control control, MouseButtons mouseButton = MouseButtons.Left)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            bool isDragging = false;
            Point dragStartPoint = Point.Empty;
            const int dragThreshold = 2; // 拖动阈值，防止误触发

           
            control.MouseDown += (sender, e) =>
            {
                if (e.Button == mouseButton)
                {
                    isDragging = false;
                    dragStartPoint = e.Location;
                }
            };
           
            control.MouseMove += (sender, e) =>
            {
                if (e.Button == mouseButton && !isDragging)
                {
                    // 计算移动距离
                    int deltaX = Math.Abs(e.X - dragStartPoint.X);
                    int deltaY = Math.Abs(e.Y - dragStartPoint.Y);

                    // 超过阈值才开始拖动
                    if (deltaX > dragThreshold || deltaY > dragThreshold)
                    {
                        Form? parentForm = control.FindForm();
                        if (parentForm != null)
                        {
                            isDragging = true;
                            ReleaseCapture();
                            SendMessage(parentForm.Handle, WM_SYSCOMMAND, SC_MOVE | HTCAPTION, 0);
                        }
                    }
                }
            };
          
            control.MouseUp += (sender, e) =>
            {
                if (e.Button == mouseButton)
                {
                    isDragging = false;
                }
            };
        }
        
        /// <summary>
        /// 批量为多个控件启用拖动功能
        /// </summary>
        /// <param name="controls">控件数组</param>
        /// <example>
        /// DragHelper.EnableDragForAll(label1, label2, panel1, pictureBox1);
        /// </example>
        public static void EnableDragForAll(params Control[] controls)
        {
            if (controls == null)
                throw new ArgumentNullException(nameof(controls));

            foreach (var control in controls)
            {
                EnableDrag(control);
            }
        }

        /// <summary>
        /// 批量为多个控件启用拖动功能
        /// </summary>
        /// <param name="controls">控件数组</param>
        /// <example>
        /// DragHelper.EnableDragForAll(label1, label2, panel1, pictureBox1);
        /// </example>
        public static void EnableDragForChildren(Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            EnableDrag(control);
            foreach (Control childControl in control.Controls)
            {
                EnableDrag(childControl);
            }
        }
    }
}
