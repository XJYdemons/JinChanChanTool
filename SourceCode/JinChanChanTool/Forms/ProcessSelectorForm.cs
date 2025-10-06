using JinChanChanTool.Services;
using System.Diagnostics;

namespace JinChanChanTool.Forms
{
    public partial class ProcessSelectorForm : Form
    {
        private readonly GameWindowService _gameWindowService;
        public Process SelectedProcess { get; private set; }

        // 需要一个辅助类来更好地在ListBox中显示信息
        private class ProcessDisplayItem
        {
            public Process Process { get; }
            public string DisplayName => $"{Process.ProcessName} (ID: {Process.Id}) - {Process.MainWindowTitle}";
            public ProcessDisplayItem(Process process) { Process = process; }
            public override string ToString() => DisplayName;
        }

        public ProcessSelectorForm(GameWindowService gameWindowService)
        {
            InitializeComponent();
            _gameWindowService = gameWindowService;

            // 设置ListBox的数据源显示方式
            listBox_Processes.DisplayMember = "DisplayName";

            // 为按钮绑定事件
            button_Refresh.Click += (s, e) => LoadProcesses();
            button_Select.Click += Button_Select_Click;

            // 窗体加载时自动刷新一次
            Load += (s, e) => LoadProcesses();
        }

        private void LoadProcesses()
        {
            listBox_Processes.Items.Clear();
            var processes = _gameWindowService.GetPotentiallyVisibleProcesses();
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
                this.DialogResult = DialogResult.OK; // 设置对话框结果，表示用户已选择
                this.Close(); // 关闭窗口
            }
            else
            {
                MessageBox.Show("请先在列表中选择一个进程！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}