using JinChanChanTool.Services;
using System.Diagnostics;

namespace JinChanChanTool.Forms
{
    public partial class ProcessSelectorForm : Form
    {
        private readonly ProcessDiscoveryService _processDiscoveryService;

        public Process SelectedProcess { get; private set; }

        private class ProcessDisplayItem
        {
            public Process Process { get; }
            public string DisplayName => $"{Process.ProcessName} (ID: {Process.Id}) - {Process.MainWindowTitle}";
            public ProcessDisplayItem(Process process) { Process = process; }
            public override string ToString() => DisplayName;
        }

        // 这是正确的构造函数，它接收一个外部服务
        public ProcessSelectorForm(ProcessDiscoveryService processDiscoveryService)
        {
            InitializeComponent();
            _processDiscoveryService = processDiscoveryService;

            listBox_Processes.DisplayMember = "DisplayName";

            button_Refresh.Click += (s, e) => LoadProcesses();
            button_Select.Click += Button_Select_Click;

            this.Load += (s, e) => LoadProcesses();
        }

        private void LoadProcesses()
        {
            listBox_Processes.Items.Clear();
            var processes = _processDiscoveryService.GetPotentiallyVisibleProcesses();
            foreach (var process in processes)
            {
                listBox_Processes.Items.Add(new ProcessDisplayItem(process));
            }
        }

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

        private void ProcessSelectorForm_Load(object sender, EventArgs e)
        {
            // 这个方法可以保留为空，或者删除（如果Designer.cs中没有对它的引用）
        }
    }
}