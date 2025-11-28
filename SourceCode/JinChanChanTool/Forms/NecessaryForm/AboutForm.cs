using System.Diagnostics;

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
            // 添加自定义标题栏
            CustomTitleBar titleBar = new CustomTitleBar(this, 32,null, "关于", CustomTitleBar.ButtonOptions.Close | CustomTitleBar.ButtonOptions.Minimize);
            this.Controls.Add(titleBar);
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
    }
}
