using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinChanChanTool.Forms
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
            // 添加自定义标题栏
            CustomTitleBar titleBar = new CustomTitleBar(this, 32, null, "正在更新当前赛季英雄推荐装备数据", CustomTitleBar.ButtonOptions.Close | CustomTitleBar.ButtonOptions.Minimize);
            this.Controls.Add(titleBar);
        }

        /// <summary>
        /// 用于从外部更新进度条和状态文本。
        /// </summary>
        /// <param name="percentage">进度百分比 (0-100)</param>
        /// <param name="statusText">要显示的状态文本</param>
        public void UpdateProgress(int percentage, string statusText)
        {
            // 检查调用是否在UI线程上，如果不是，则通过Invoke使其在UI线程上执行
            if (progressBar1.InvokeRequired)
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
            progressBar1.Value = percentage;

            // 更新 Label 的文本
            lblStatus.Text = statusText;

            // 强制UI立即重绘，以确保用户能看到最新的状态
            this.Update();
        }
    }
}
