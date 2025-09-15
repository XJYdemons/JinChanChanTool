using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinChanChanTool.Forms
{
    public partial class StatusOverlayForm : Form
    {
        private static StatusOverlayForm _instance;
        public static StatusOverlayForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new StatusOverlayForm();
                }
                return _instance;
            }
        }
        // 状态标签
        private Label lblStatus1;
        private Label lblStatus2;        
        // 拖动相关变量
        private Point _dragStartPoint;
        private bool _dragging;

       
        private StatusOverlayForm()
        {
            InitializeComponent();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // 窗体设置
            this.FormBorderStyle = FormBorderStyle.None;// 无边框
            this.TopMost = true;// 始终置顶
            this.ShowInTaskbar = false;// 不在任务栏显示
            this.StartPosition = FormStartPosition.Manual;// 手动设置位置
            this.DoubleBuffered = true; // 启用双缓冲减少闪烁
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.Padding = new Padding(5);            
            // 设置窗口位置（右上角）
            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(
                screen.Right - this.Width - 10,
                screen.Top + 10
            );

            // 状态标签1 - 使用自动调整大小
            lblStatus1 = new Label
            {
                Text = "开关1: 关闭",
                ForeColor = Color.White,
                //Font = new Font("微软雅黑", 10 * dpiScale, FontStyle.Bold),
                //Location = new Point((int)(10 * dpiScale), (int)(10 * dpiScale)),
                Font = new Font("微软雅黑", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Margin = new Padding(5),
                AutoSize = true, // 启用自动调整大小
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent // 设置标签背景透明
            };

            // 状态标签2 - 使用自动调整大小
            lblStatus2 = new Label
            {
                Text = "开关2: 关闭",
                ForeColor = Color.White,
                //Font = new Font("微软雅黑", 10 * dpiScale, FontStyle.Bold),
                //Location = new Point((int)(10 * dpiScale), (int)(40 * dpiScale)),
                Font = new Font("微软雅黑", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Margin = new Padding(5),
                AutoSize = true, // 启用自动调整大小
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent // 设置标签背景透明
            };

            // 添加控件
            this.Controls.Add(lblStatus2);
            this.Controls.Add(lblStatus1);


            // 鼠标事件处理
            this.MouseDown += StatusOverlayForm_MouseDown;
            this.MouseMove += StatusOverlayForm_MouseMove;
            this.MouseUp += StatusOverlayForm_MouseUp;

            // 标签也响应拖动
            lblStatus1.MouseDown += StatusOverlayForm_MouseDown;
            lblStatus1.MouseMove += StatusOverlayForm_MouseMove;
            lblStatus1.MouseUp += StatusOverlayForm_MouseUp;

            lblStatus2.MouseDown += StatusOverlayForm_MouseDown;
            lblStatus2.MouseMove += StatusOverlayForm_MouseMove;
            lblStatus2.MouseUp += StatusOverlayForm_MouseUp;
        }
     
        // 更新状态显示
        public void UpdateStatus(bool status1, bool status2)
        {
            if (lblStatus1.InvokeRequired)
            {
                lblStatus1.Invoke(new Action(() => UpdateStatus(status1, status2)));
                return;
            }
            lblStatus1.Text = $"自动拿牌: {(status1 ? "开启" : "关闭")}";
            lblStatus1.ForeColor = status1 ? Color.LimeGreen : Color.White;

            lblStatus2.Text = $"自动刷新商店: {(status2 ? "开启" : "关闭")}";
            lblStatus2.ForeColor = status2 ? Color.LimeGreen : Color.White;
        }

        // 鼠标按下事件 - 开始拖动
        private void StatusOverlayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
            }
        }

        // 鼠标移动事件 - 处理拖动
        private void StatusOverlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-_dragStartPoint.X, -_dragStartPoint.Y);
                this.Location = newLocation;
            }
        }

        // 鼠标释放事件 - 结束拖动
        private void StatusOverlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        // 重写创建控件时的行为，使窗口支持半透明
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cp;
            }
        }

        // 设置窗口透明度
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 设置整个窗口的透明度（255为不透明，0为完全透明）
            SetWindowOpacity(168); // 提高透明度减少残影
        }

        private void SetWindowOpacity(byte opacity)
        {
            // 使用User32.dll中的SetLayeredWindowAttributes函数
            if (this.IsHandleCreated)
            {
                SetLayeredWindowAttributes(this.Handle, 0, opacity, LayeredWindowFlags.LWA_ALPHA);
            }
        }

        // 绘制背景为纯色
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 使用一个不透明的画刷填充背景
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(64, 64, 64)))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        #region Win32 API
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, LayeredWindowFlags dwFlags);

        private void StatusOverlayForm_Load(object sender, EventArgs e)
        {

        }

        [Flags]
        enum LayeredWindowFlags : uint
        {
            LWA_COLORKEY = 0x00000001,
            LWA_ALPHA = 0x00000002
        }
        #endregion       

    }
}
