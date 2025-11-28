using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices.Interface;
using System;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 状态显示窗口
    /// </summary>
    public partial class StatusOverlayForm : Form
    {
        //单例模式
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
        private Label lblStatus3;
        private Label lblStatus4;

        // 拖动相关变量
        private Point _dragStartPoint;
        private bool _dragging;

        public IAutomaticSettingsService _iAutoConfigService;//自动设置数据服务对象

        private StatusOverlayForm()
        {
            InitializeComponent();
            InitializeComponents();
        }

        /// <summary>
        /// 初始化自定义组件
        /// </summary>
        private void InitializeComponents()
        {
            // 窗体设置
            this.FormBorderStyle = FormBorderStyle.None;//无边框
            this.TopMost = true;//始终置顶
            this.ShowInTaskbar = false;//不在任务栏显示
            this.StartPosition = FormStartPosition.Manual;//手动设置位置
            this.DoubleBuffered = true;//启用双缓冲减少闪烁
            this.AutoScaleMode = AutoScaleMode.Dpi;            
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(0);

            // 使用一个容器布局（FlowLayoutPanel）让Label自动排列
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };

            lblStatus1 = new Label
            {
                Text = "开关1: 关闭",
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(2),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            lblStatus2 = new Label
            {
                Text = "开关2: 关闭",
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(2),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            lblStatus3 = new Label
            {
                Text = "",
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(2),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            lblStatus4 = new Label
            {
                Text = "",
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(2),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblStatus1);
            panel.Controls.Add(lblStatus2);
            panel.Controls.Add(lblStatus3);
            panel.Controls.Add(lblStatus4);
            this.Controls.Add(panel);
          
            // 鼠标事件处理
            this.MouseDown += StatusOverlayForm_MouseDown;
            this.MouseMove += StatusOverlayForm_MouseMove;
            this.MouseUp += StatusOverlayForm_MouseUp;
            panel.MouseDown += StatusOverlayForm_MouseDown;
            panel.MouseMove += StatusOverlayForm_MouseMove;
            panel.MouseUp += StatusOverlayForm_MouseUp;

            lblStatus1.MouseDown += StatusOverlayForm_MouseDown;
            lblStatus1.MouseMove += StatusOverlayForm_MouseMove;
            lblStatus1.MouseUp += StatusOverlayForm_MouseUp;

            lblStatus2.MouseDown += StatusOverlayForm_MouseDown;
            lblStatus2.MouseMove += StatusOverlayForm_MouseMove;
            lblStatus2.MouseUp += StatusOverlayForm_MouseUp;

            lblStatus3.MouseDown += StatusOverlayForm_MouseDown;
            lblStatus3.MouseMove += StatusOverlayForm_MouseMove;
            lblStatus3.MouseUp += StatusOverlayForm_MouseUp;

            lblStatus4.MouseDown += StatusOverlayForm_MouseDown;
            lblStatus4.MouseMove += StatusOverlayForm_MouseMove;
            lblStatus4.MouseUp += StatusOverlayForm_MouseUp;
        }

        // 更新状态显示
        public void UpdateStatus(bool status1, bool status2,string hotkey1,string hotkey2,string hotkey3,string hotkey4)
        {
            if (lblStatus1.InvokeRequired)
            {
                lblStatus1.Invoke(new Action(() => UpdateStatus(status1, status2,hotkey1,hotkey2,hotkey3,hotkey4)));
                return;
            }
            lblStatus1.Text = $"{hotkey1} - 自动拿牌: [{(status1 ? "开" : "关")}]";
            lblStatus1.ForeColor = status1 ? Color.LimeGreen : Color.White;

            lblStatus2.Text = $"{hotkey2} - 自动刷新商店: [{(status2 ? "开" : "关")}]";
            lblStatus2.ForeColor = status2 ? Color.LimeGreen : Color.White;

            lblStatus3.Text = $"{hotkey3} - 隐藏/召出主窗口";

            lblStatus4.Text = $"{hotkey4} - 自动D牌";
        }

        #region 拖动窗体功能
        /// <summary>
        /// 鼠标按下事件 - 开始拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusOverlayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
            }
        }

        /// <summary>
        /// 鼠标移动事件 - 处理拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusOverlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-_dragStartPoint.X, -_dragStartPoint.Y);
                this.Location = newLocation;
            }
        }

        /// <summary>
        /// 鼠标释放事件 - 结束拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusOverlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
            SaveFormLocation();
        }

        #endregion

        /// <summary>
        /// 初始化配置服务
        /// </summary>
        /// <param name="iAutoConfigService"></param>
        public void InitializeObject(IAutomaticSettingsService iAutoConfigService)
        {
            _iAutoConfigService = iAutoConfigService;            
        }

        /// <summary>
        /// 显示窗体时应用保存的位置
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);           
            try
            {
                this.StartPosition = FormStartPosition.Manual;
                if (_iAutoConfigService.CurrentConfig.StatusOverlayFormLocation.X == -1 && _iAutoConfigService.CurrentConfig.StatusOverlayFormLocation.Y == -1)
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Right - this.Width /*- 10*/,
                        screen.Top /*+ 10*/
                    );
                    return;
                }
                // 确保坐标在屏幕范围内
                if (Screen.AllScreens.Any(s => s.Bounds.Contains(_iAutoConfigService.CurrentConfig.StatusOverlayFormLocation)))
                {
                    this.Location = _iAutoConfigService.CurrentConfig.StatusOverlayFormLocation;
                }
                else
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Right - this.Width /*- 10*/,
                        screen.Top /*+ 10*/
                    );
                }
            }
            catch
            {
                var screen = Screen.PrimaryScreen.Bounds;
                this.Location = new Point(
                    screen.Right - this.Width /*- 10*/,
                    screen.Top /*+ 10*/
                );
            }
        }

        /// <summary>
        /// 重写创建控件时的行为，使窗口支持半透明
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cp;
            }
        }

        /// <summary>
        /// 设置窗口透明度
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 设置整个窗口的透明度（255为不透明，0为完全透明）
            SetWindowOpacity(168); // 提高透明度减少残影
        }

        /// <summary>
        /// 设置窗口透明度
        /// </summary>
        /// <param name="opacity"></param>
        private void SetWindowOpacity(byte opacity)
        {
            // 使用User32.dll中的SetLayeredWindowAttributes函数
            if (this.IsHandleCreated)
            {
                SetLayeredWindowAttributes(this.Handle, 0, opacity, LayeredWindowFlags.LWA_ALPHA);
            }
        }

        /// <summary>
        /// 绘制背景为纯色
        /// </summary>
        /// <param name="e"></param>
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
        #region 位置保存与读取
        
        /// <summary>
        /// 拖动结束时保存窗口位置到配置服务
        /// </summary>
        private void SaveFormLocation()
        {
            try
            {
                if (_iAutoConfigService != null)
                {
                    _iAutoConfigService.CurrentConfig.StatusOverlayFormLocation = this.Location;
                    _iAutoConfigService.Save();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
