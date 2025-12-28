using System.Runtime.InteropServices;

namespace JinChanChanTool
{
    /// <summary>
    /// 自定义标题栏控件
    /// </summary>
    public class CustomTitleBar : Panel
    {
        private readonly Form _form;// 所属窗体       
        private PictureBox _iconPictureBox;// 图标控件
        private Label _titleLabel;// 标题标签
        private Button _minButton;// 最小化按钮
        private Button _maxButton;// 最大化按钮
        private Button _closeButton;// 关闭按钮

        /// <summary>
        /// 定义标题栏按钮选项枚举
        /// </summary>
        [Flags]
        public enum ButtonOptions
        {
            None = 0,
            Minimize = 1,
            Maximize = 2,
            Close = 4,
            All = Minimize | Maximize | Close
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="form">所属的窗体</param>
        /// <param name="height">标题栏的高度</param>
        /// <param name="icon">标题栏的图标（默认为空）</param>
        /// <param name="title">标题栏标题（默认为空）</param>
        /// <param name="buttons">标题栏按钮（默认包括最小化、最大化、关闭）</param>
        public CustomTitleBar(Form form, int height, Image icon = null, string title = null, ButtonOptions buttons = ButtonOptions.All)
        {
            _form = form;
            InitializeComponents(icon, height, title ?? "", buttons);
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="height"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        private void InitializeComponents(Image icon, int height, string title, ButtonOptions buttons)
        {
            Height = height;//控件高度
            MinimumSize = new Size(1, height);//控件最小尺寸
            AutoSize = true;//自动调整大小
            Dock = DockStyle.Top;//停靠在顶部
            BackColor = Color.White;//背景颜色
            Padding = new Padding(5, 0, 0, 0);//内边距

            // 创建图标控件
            if (icon != null)
            {
                _iconPictureBox = new PictureBox
                {
                    Dock = DockStyle.Left,// 停靠在左侧
                    Size = new(16, 16),//图标大小     
                    BackColor = Color.Transparent,//背景透明
                    SizeMode = PictureBoxSizeMode.Zoom,//图标缩放模式
                    Image = icon//设置图标
                };

                // 添加拖动事件
                _iconPictureBox.MouseDown += TitleBar_MouseDown;
                _iconPictureBox.MouseMove += TitleBar_MouseMove;
                _iconPictureBox.MouseMove += TitleBar_MouseUp;
            }

            // 创建标题标签
            _titleLabel = new Label
            {
                Text = title,// 设置标题文本
                ForeColor = Color.Black,// 字体颜色
                Dock = DockStyle.Fill,// 填充剩余空间
                Height = height,// 高度
                MinimumSize = new Size(1, height),// 最小尺寸
                BackColor = Color.Transparent,//背景透明                
                TextAlign = ContentAlignment.MiddleLeft,// 文本对齐方式水平靠左垂直居中
                Padding = new Padding(icon != null ? 3 : 0) // 内边距，图标存在时左侧留出间距
            };

            // 最小化、最大化、关闭按钮
            if (buttons.HasFlag(ButtonOptions.Minimize))
            {
                _minButton = CreateButton("─", ButtonOptions.Minimize, height);
                _minButton.Click += MinButton_Click;
            }

            if (buttons.HasFlag(ButtonOptions.Maximize))
            {
                _maxButton = CreateButton("□", ButtonOptions.Maximize, height);
                _maxButton.Click += MaxButton_Click;
            }

            if (buttons.HasFlag(ButtonOptions.Close))
            {
                _closeButton = CreateButton("×", ButtonOptions.Close, height);
                _closeButton.Click += CloseButton_Click;
            }

            if (_minButton != null) Controls.Add(_minButton);
            if (_maxButton != null) Controls.Add(_maxButton);
            if (_closeButton != null) Controls.Add(_closeButton);
            Controls.Add(_titleLabel);
            if (_iconPictureBox != null) Controls.Add(_iconPictureBox);

            MouseDown += TitleBar_MouseDown;
            MouseMove += TitleBar_MouseMove;
            MouseMove += TitleBar_MouseUp;
            _titleLabel.MouseDown += TitleBar_MouseDown;
            _titleLabel.MouseMove += TitleBar_MouseMove;
            _titleLabel.MouseMove += TitleBar_MouseUp;
        }

        /// <summary>
        /// 创建按钮的辅助方法
        /// </summary>
        /// <param name="text"></param>
        /// <param name="option"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Button CreateButton(string text, ButtonOptions option, int height)
        {
            var button = new Button
            {
                Text = text,// 按钮文本
                ForeColor = Color.Black,// 字体颜色
                Dock = DockStyle.Right,// 停靠在右侧
                Width = height,// 按钮宽度
                Height = height,// 按钮高度
                MinimumSize = new Size(height, height), // 按钮最小尺寸
                AutoSize = true,// 自动调整大小
                FlatStyle = FlatStyle.Flat,// 扁平样式
                TextAlign = ContentAlignment.MiddleCenter,// 文本居中
                BackColor = Color.Transparent,// 背景透明
                Tag = option, // 存储按钮类型用于调试               
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        /// <summary>
        /// 最小化按钮点击事件处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinButton_Click(object sender, EventArgs e)
        {
            _form.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 最大化按钮点击事件处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaxButton_Click(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Maximized)
            {
                _form.WindowState = FormWindowState.Normal;
                _maxButton.Text = "□";
            }
            else
            {
                _form.WindowState = FormWindowState.Maximized;
                _maxButton.Text = "❐";
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF012;
        private const int HTCAPTION = 0x0002;
        bool isDragging = false;
        Point dragStartPoint = Point.Empty;
        const int dragThreshold = 2; // 拖动阈值，防止误触发
        /// <summary>
        /// 关闭按钮点击事件处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            _form.Close();
        }

        /// <summary>
        /// 标题栏鼠标按下事件处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                dragStartPoint = e.Location;
            }
        }

        /// <summary>
        /// 标题栏鼠标移动事件处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !isDragging)
            {
                // 计算移动距离
                int deltaX = Math.Abs(e.X - dragStartPoint.X);
                int deltaY = Math.Abs(e.Y - dragStartPoint.Y);

                // 超过阈值才开始拖动
                if (deltaX > dragThreshold || deltaY > dragThreshold)
                {

                    if (_form != null)
                    {
                        isDragging = true;
                        ReleaseCapture();
                        SendMessage(_form.Handle, WM_SYSCOMMAND, SC_MOVE | HTCAPTION, 0);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }
    }
}