using JinChanChanTool.Services.AutoSetCoordinates;
using System.Diagnostics;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 用于选择进程的窗体
    /// </summary>
    public partial class ProcessSelectorForm : Form
    {
        private readonly ProcessDiscoveryService _processDiscoveryService;//进程发现服务

        public Process SelectedProcess { get; private set; }//选中的进程

        private class ProcessDisplayItem
        {
            public Process Process { get; }
            public string DisplayName => $"{Process.ProcessName} (ID: {Process.Id}) - {Process.MainWindowTitle}";
            public ProcessDisplayItem(Process process) { Process = process; }
            public override string ToString() => DisplayName;
        }

        /// <summary>
        /// 构造函数，它接收一个外部服务
        /// </summary>
        /// <param name="processDiscoveryService"></param>
        public ProcessSelectorForm(ProcessDiscoveryService processDiscoveryService)
        {
            InitializeComponent();
            #region 自定义标题栏
            CustomTitleBar titleBar = new CustomTitleBar(this, 32, null, "选择进程", CustomTitleBar.ButtonOptions.Close | CustomTitleBar.ButtonOptions.Minimize|CustomTitleBar.ButtonOptions.Maximize);
            this.Controls.Add(titleBar);
            #endregion
            _processDiscoveryService = processDiscoveryService;

            listBox_Processes.DisplayMember = "DisplayName";

            button_Refresh.Click += (s, e) => LoadProcesses();
            button_Select.Click += Button_Select_Click;

            this.Load += (s, e) => LoadProcesses();
        }

        /// <summary>
        /// 加载进程列表
        /// </summary>
        private void LoadProcesses()
        {
            listBox_Processes.Items.Clear();
            var processes = _processDiscoveryService.GetPotentiallyVisibleProcesses();
            foreach (var process in processes)
            {
                listBox_Processes.Items.Add(new ProcessDisplayItem(process));
            }
        }

        /// <summary>
        /// 选择按钮点击事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Select_Click(object sender, EventArgs e)
        {
            if (listBox_Processes.SelectedItem is ProcessDisplayItem selectedItem)
            {
                SelectedProcess = selectedItem.Process;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("请先在列表中选择一个进程！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }       
    }
}