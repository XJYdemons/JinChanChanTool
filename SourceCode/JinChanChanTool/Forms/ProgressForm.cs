using JinChanChanTool.Services.Localization;
using JinChanChanTool.Tools;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 用于显示进度的窗体
    /// </summary>
    public partial class ProgressForm : Form
    {
        public ProgressForm(ILocalizationService iLocalizationService)
        {
            InitializeComponent();
            DragHelper.EnableDragForChildren(panel_标题栏);
            ApplyLocalization(iLocalizationService);
        }

        /// <summary>
        /// 应用本地化文本
        /// </summary>
        /// <param name="iLocalizationService">本地化服务对象</param>
        private void ApplyLocalization(ILocalizationService iLocalizationService)
        {
            label_标题.Text = iLocalizationService.Get("ProgressForm.标题");
            this.Text = iLocalizationService.Get("ProgressForm.窗口标题");
        }

        /// <summary>
        /// 用于从外部更新进度条和状态文本。
        /// </summary>
        /// <param name="percentage">进度百分比 (0-100)</param>
        /// <param name="statusText">要显示的状态文本</param>
        public void UpdateProgress(int percentage, string statusText)
        {
            // 检查调用是否在UI线程上，如果不是，则通过Invoke使其在UI线程上执行
            if (progressBar.InvokeRequired)
            {
                // 创建一个委托，并异步调用此方法本身
                this.Invoke(new Action(() => UpdateProgress(percentage, statusText)));
                return;
            }

            // --- 以下代码确保在UI线程上执行 ---

            // 限制百分比在0到100之间
            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;

            // 更新 ProgressBar 的值
            progressBar.Value = percentage;

                       

            // 强制UI立即重绘，以确保用户能看到最新的状态
            this.Update();
        }

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
    }
}
