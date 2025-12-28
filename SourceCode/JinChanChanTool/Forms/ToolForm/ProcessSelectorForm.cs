using JinChanChanTool.Services.AutoSetCoordinates;
using JinChanChanTool.Tools;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
            DragHelper.EnableDragForChildren(panel3);
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
    }
}