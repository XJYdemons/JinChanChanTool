using System.Drawing;
using System.Windows.Forms;

namespace JinChanChanTool
{
    public class CustomTitleBar : Panel
    {
        private readonly Form _form;
        private Point _dragStartPoint;
        private PictureBox _iconPictureBox;
        private Label _titleLabel;
        private Button _minButton;
        private Button _maxButton;
        private Button _closeButton;
        
        [Flags]
        public enum ButtonOptions
        {
            None = 0,
            Minimize = 1,
            Maximize = 2,
            Close = 4,
            All = Minimize | Maximize | Close
        }

        public CustomTitleBar(Form form,int height, Image icon = null, string title = null,ButtonOptions buttons = ButtonOptions.All)
        {
            _form = form;
            InitializeComponents(icon,height, title ?? "", buttons);
        }

        private void InitializeComponents(Image icon,int height, string title, ButtonOptions buttons)
        {           
            Height = height;
            MinimumSize = new Size(1, height);
            AutoSize = true;
            Dock = DockStyle.Top;
            BackColor = Color.White;
            Padding = new Padding(5,0,0,0);
            
            if (icon != null)
            {
                _iconPictureBox = new PictureBox
                {
                    Dock = DockStyle.Left,
                    Size = new(16, 16),                   
                    BackColor = Color.Transparent,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = icon
                };
                
                _iconPictureBox.MouseDown += TitleBar_MouseDown;
                _iconPictureBox.MouseMove += TitleBar_MouseMove;
            }
           
            _titleLabel = new Label
            {
                Text = title,
                ForeColor = Color.Black,
                Dock = DockStyle.Fill,
                Height = height,
                MinimumSize =new Size(1, height),
                BackColor = Color.Transparent,                
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(icon != null ? 3 : 0) 
            };
           
            if (buttons.HasFlag(ButtonOptions.Minimize))
            {
                _minButton = CreateButton("─", ButtonOptions.Minimize, height);
                _minButton.Click += (s, e) => _form.WindowState = FormWindowState.Minimized;
            }

            if (buttons.HasFlag(ButtonOptions.Maximize))
            {
                _maxButton = CreateButton("□", ButtonOptions.Maximize, height);
                _maxButton.Click += (s, e) => ToggleMaximize();
            }

            if (buttons.HasFlag(ButtonOptions.Close))
            {
                _closeButton = CreateButton("×", ButtonOptions.Close, height);
                _closeButton.Click += (s, e) => _form.Close();
            }
        
            if (_minButton != null) Controls.Add(_minButton);
            if (_maxButton != null) Controls.Add(_maxButton);           
            if (_closeButton != null) Controls.Add(_closeButton);
           
            Controls.Add(_titleLabel);

           
            if (_iconPictureBox != null) Controls.Add(_iconPictureBox);

          
            MouseDown += TitleBar_MouseDown;
            MouseMove += TitleBar_MouseMove;
            _titleLabel.MouseDown += TitleBar_MouseDown;
            _titleLabel.MouseMove += TitleBar_MouseMove;
        }

        private Button CreateButton(string text, ButtonOptions option,int height)
        {
            var button = new Button
            {
                Text = text,
                ForeColor = Color.Black,
                Dock = DockStyle.Right,
                Width = height,
                Height = height,
                MinimumSize = new Size(height, height),
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Tag = option // 存储按钮类型用于调试
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragStartPoint = new Point(e.X, e.Y);
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point diff = new Point(e.X - _dragStartPoint.X, e.Y - _dragStartPoint.Y);
                _form.Location = new Point(
                    _form.Location.X + diff.X,
                    _form.Location.Y + diff.Y
                );
            }
        }

        private void ToggleMaximize()
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
    }
}