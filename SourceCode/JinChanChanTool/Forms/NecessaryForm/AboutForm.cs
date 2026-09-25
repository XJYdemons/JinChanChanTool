using JinChanChanTool.DataClass.StaticData;
using JinChanChanTool.Services.Localization;
using JinChanChanTool.Tools;
using System.Diagnostics;

namespace JinChanChanTool
{
    /// <summary>
    /// 关于窗口
    /// </summary>
    public partial class AboutForm : Form
    {
        private readonly ILocalizationService _iLocalizationService;

        public AboutForm(ILocalizationService iLocalizationService)
        {
            InitializeComponent();
            DragHelper.EnableDragForChildren(panel_标题栏背景);
            _iLocalizationService = iLocalizationService;
            ApplyLocalization();
        }

        /// <summary>
        /// 应用本地化文本
        /// </summary>
        private void ApplyLocalization()
        {
            label_标题.Text = _iLocalizationService.Get("AboutForm.标题");
            label_架构.Text = _iLocalizationService.Get("AboutForm.Label.架构");
            // 版本号取自程序集元数据，保留项目文件中填写的完整内容（含 -beta 等后缀）
            label_版本号.Text = _iLocalizationService.Get(
                "AboutForm.Label.版本格式",
                ProgramVersion.RawVersionText,
                ProgramVersion.BuildDateText);
            label_版权所有.Text = _iLocalizationService.Get("AboutForm.Label.版权所有");
            label_项目地址.Text = _iLocalizationService.Get("AboutForm.Label.项目地址");
            label_Github主页.Text = _iLocalizationService.Get("AboutForm.Label.Github主页");
            label_开发者.Text = _iLocalizationService.Get("AboutForm.Label.开发者");                        
        }

        /// <summary>
        /// 打开B站主页。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
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
        private void label5_MouseEnter(object? sender, EventArgs e)
        {
            label_Github主页.Cursor = Cursors.Hand;
            label_Github主页.ForeColor = Color.Blue;
        }

        /// <summary>
        /// 当鼠标离开label5时，恢复默认光标形状并将文字颜色改回黑色。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label5_MouseLeave(object? sender, EventArgs e)
        {
            label_Github主页.Cursor = Cursors.Default;
            label_Github主页.ForeColor = Color.Black;
        }

        /// <summary>
        /// 打开GitHub主页。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label5_Click(object? sender, EventArgs e)
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
        private void label6_Click(object? sender, EventArgs e)
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
        private void label6_MouseEnter(object? sender, EventArgs e)
        {
            label_项目地址.Cursor = Cursors.Hand;
            label_项目地址.ForeColor = Color.Blue;
        }

        /// <summary>
        /// 当鼠标离开label6时，恢复默认光标形状并将文字颜色改回黑色。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label6_MouseLeave(object? sender, EventArgs e)
        {
            label_项目地址.Cursor = Cursors.Default;
            label_项目地址.ForeColor = Color.Black;
        }

        private void AboutForm_Load(object? sender, EventArgs e)
        {

        }

        /// <summary>
        /// 打开GitHub主页。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel2_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
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
        private void linkLabel3_Click(object? sender, EventArgs e)
        {

            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/baolibaobao", //需要打开的URL
                UseShellExecute = true  //系统自动识别文件类型并调用关联程序打开
            });
        }

        #region 圆角实现
        /// <summary>
        /// 在窗口句柄创建后应用圆角效果
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // 应用 GDI Region 圆角效果（支持 Windows 10 和 Windows 11）
            RoundedCornerHelper.Apply(this);
        }

        /// <summary>
        /// 窗口大小改变时重新应用圆角
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 调整大小时重新创建圆角区域
            RoundedCornerHelper.Apply(this);
        }
        #endregion

        #region 标题栏按钮事件
        private void button_最小化_Click(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_关闭_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void panel1_Paint(object? sender, PaintEventArgs e)
        {

        }
    }
}
