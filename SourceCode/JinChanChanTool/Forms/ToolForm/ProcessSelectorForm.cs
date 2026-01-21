using JinChanChanTool.Services.AutoSetCoordinates;
using JinChanChanTool.Tools;
using System.Diagnostics;
using System.Drawing;
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

            // 添加一个列用于显示进程信息（隐藏列头）
            listView_Processes.Columns.Add("进程", listView_Processes.Width - 25, HorizontalAlignment.Left);
            // 双击选择进程
            listView_Processes.DoubleClick += (s, e) => Button_Select_Click(s, e);

            button_Refresh.Click += (s, e) => LoadProcesses();
            button_Select.Click += Button_Select_Click;

            this.Load += (s, e) => LoadProcesses();
        }

        /// <summary>
        /// 加载进程列表
        /// </summary>
        private void LoadProcesses()
        {
            listView_Processes.Items.Clear();
            imageList_ProcessIcons.Images.Clear();

            List<Process> processes = _processDiscoveryService.GetPotentiallyVisibleProcesses();
            int imageIndex = 0;

            foreach (Process process in processes)
            {
                // 尝试获取进程图标
                Icon? processIcon = GetProcessIcon(process);
                if (processIcon != null)
                {
                    imageList_ProcessIcons.Images.Add(processIcon);
                }
                else
                {
                    // 使用默认应用程序图标
                    imageList_ProcessIcons.Images.Add(SystemIcons.Application);
                }

                // 创建 ListView 项，显示格式保持原样
                ProcessDisplayItem displayItem = new ProcessDisplayItem(process);
                ListViewItem listViewItem = new ListViewItem(displayItem.DisplayName)
                {
                    ImageIndex = imageIndex,
                    Tag = process // 将进程对象存储在 Tag 中以便后续获取
                };

                listView_Processes.Items.Add(listViewItem);
                imageIndex++;
            }
        }

        /// <summary>
        /// 获取进程的图标
        /// </summary>
        /// <param name="process">目标进程</param>
        /// <returns>进程图标，获取失败返回 null</returns>
        private Icon? GetProcessIcon(Process process)
        {
            try
            {
                // 尝试获取进程的主模块文件路径
                string? filePath = process.MainModule?.FileName;
                if (!string.IsNullOrEmpty(filePath))
                {
                    // 从可执行文件提取关联图标
                    return Icon.ExtractAssociatedIcon(filePath);
                }
            }
            catch (Exception)
            {
                // 权限不足或其他原因无法获取，忽略异常
            }
            return null;
        }

        /// <summary>
        /// 选择按钮点击事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Select_Click(object? sender, EventArgs e)
        {
            if (listView_Processes.SelectedItems.Count > 0 &&
                listView_Processes.SelectedItems[0].Tag is Process selectedProcess)
            {
                SelectedProcess = selectedProcess;
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