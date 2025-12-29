using JinChanChanTool.Tools;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JinChanChanTool
{
    /// <summary>
    /// 关于窗口
    /// </summary>
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            DragHelper.EnableDragForChildren(panel3);
        }

        /// <summary>
        /// 打开B站主页。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://space.bilibili.com/173882688", //需要打开的URL
                UseShellExecute = true  //系统自动识别文件类型并调用关联程序打开
            });

        }

        /// <summary>
        /// 当鼠标进入label5时，改变光标形状并更改文字颜色以提示用户该标签是可点击的链接。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label5_MouseEnter(object sender, EventArgs e)
        {
            label_Github主页.Cursor = Cursors.Hand;
            label_Github主页.ForeColor = Color.Blue;
        }

        /// <summary>
        /// 当鼠标离开label5时，恢复默认光标形状并将文字颜色改回黑色。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label5_MouseLeave(object sender, EventArgs e)
        {
            label_Github主页.Cursor = Cursors.Default;
            label_Github主页.ForeColor = Color.Black;
        }

        /// <summary>
        /// 打开GitHub主页。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label5_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/XJYdemons", // 需要打开的URL
                UseShellExecute = true  //系统自动识别文件类型并调用关联程序打开
            });
        }

        /// <summary>
        /// 打开GitHub项目主页。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label6_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/XJYdemons/Jin-chan-chan-Tools", // 需要打开的URL
                UseShellExecute = true  //系统自动识别文件类型并调用关联程序打开
            });
        }

        /// <summary>
        /// 当鼠标进入label6时，改变光标形状并更改文字颜色以提示用户该标签是可点击的链接。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label6_MouseEnter(object sender, EventArgs e)
        {
            label_项目地址.Cursor = Cursors.Hand;
            label_项目地址.ForeColor = Color.Blue;
        }

        /// <summary>
        /// 当鼠标离开label6时，恢复默认光标形状并将文字颜色改回黑色。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label6_MouseLeave(object sender, EventArgs e)
        {
            label_项目地址.Cursor = Cursors.Default;
            label_项目地址.ForeColor = Color.Black;
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 打开GitHub主页。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/XJYdemons", //需要打开的URL
                UseShellExecute = true  //系统自动识别文件类型并调用关联程序打开
            });
        }

        /// <summary>
        /// 打开GitHub主页。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel3_Click(object sender, EventArgs e)
        {

            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/baolibaobao", //需要打开的URL
                UseShellExecute = true  //系统自动识别文件类型并调用关联程序打开
            });
        }

        #region 圆角实现
        // GDI32 API - 用于创建圆角效果
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        // 圆角半径
        private const int CORNER_RADIUS = 16;

        /// <summary>
        /// 在窗口句柄创建后应用圆角效果
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // 应用 GDI Region 圆角效果（支持 Windows 10 和 Windows 11）
            ApplyRoundedCorners();
        }

        /// <summary>
        /// 应用 GDI Region 圆角效果
        /// </summary>
        private void ApplyRoundedCorners()
        {
            try
            {
                // 创建圆角矩形区域
                IntPtr region = CreateRoundRectRgn(0, 0, Width, Height, CORNER_RADIUS, CORNER_RADIUS);

                if (region != IntPtr.Zero)
                {
                    SetWindowRgn(Handle, region, true);
                    // 注意：SetWindowRgn 会接管 region 的所有权，不需要手动删除

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 窗口大小改变时重新应用圆角
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 调整大小时重新创建圆角区域
            if (Handle != IntPtr.Zero)
            {
                ApplyRoundedCorners();
            }
        }
        #endregion

        #region 标题栏按钮事件
        private void button_最小化_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_关闭_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
