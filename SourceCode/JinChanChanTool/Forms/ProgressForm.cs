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

            //初始化逻辑
            this.StartPosition = FormStartPosition.CenterParent;
            this.ControlBox = false; // 禁用关闭按钮，防止用户在更新时关闭窗口
            this.Text = "正在更新数据...";
        }
        /// <summary>
        /// 一个线程安全的方法，用于从外部更新进度条和状态文本
        /// </summary>
        /// <param name="percentage">进度百分比 (0-100)</param>
        /// <param name="message">要显示的状态消息</param>
        public void UpdateProgress(int percentage, string message)
        {
            // 检查当前线程是否是UI线程
            if (this.InvokeRequired)
            {
                // 如果是工作线程，则调用UI线程来执行真正的更新操作
                this.Invoke(new Action(() => UpdateUI(percentage, message)));
            }
            else
            {
                // 如果已经是UI线程，直接更新
                UpdateUI(percentage, message);
            }
        }
        /// <summary>
        /// 实际执行UI更新的私有方法
        /// </summary>
        private void UpdateUI(int percentage, string message)
        {
            // 确保百分比值在 ProgressBar 的有效范围内
            if (percentage >= progressBar.Minimum && percentage <= progressBar.Maximum)
            {
                progressBar.Value = percentage;
            }

            // 更新 Label 的文本
            lblStatus.Text = message;
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }
    }
}