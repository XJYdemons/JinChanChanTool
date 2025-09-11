using System.Drawing;
using System.Windows.Forms;

namespace JinChanChanTool
{
    public class CustomTitleBar : Panel
    {
        private readonly Form _hostForm;
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

        public CustomTitleBar(Form hostForm, Image icon = null, string title = null,
                              ButtonOptions buttons = ButtonOptions.All)
        {
            _hostForm = hostForm;
            InitializeComponents(icon, title ?? hostForm.Text, buttons);
        }

        private void InitializeComponents(Image icon, string title, ButtonOptions buttons)
        {           
            Height = 30;
            Dock = DockStyle.Top;
            BackColor = Color.White;
            Padding = new Padding(6,0,0,0);
            
            if (icon != null)
            {
                _iconPictureBox = new PictureBox
                {
                    Dock = DockStyle.Left,
                    Size = new(15, 15),                   
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
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(icon != null ? 5 : 10, 0, 0, 0) 
            };
           
            if (buttons.HasFlag(ButtonOptions.Minimize))
            {
                _minButton = CreateButton("─", ButtonOptions.Minimize);
                _minButton.Click += (s, e) => _hostForm.WindowState = FormWindowState.Minimized;
            }

            if (buttons.HasFlag(ButtonOptions.Maximize))
            {
                _maxButton = CreateButton("□", ButtonOptions.Maximize);
                _maxButton.Click += (s, e) => ToggleMaximize();
            }

            if (buttons.HasFlag(ButtonOptions.Close))
            {
                _closeButton = CreateButton("×", ButtonOptions.Close);
                _closeButton.Click += (s, e) => _hostForm.Close();
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

        private Button CreateButton(string text, ButtonOptions option)
        {
            var button = new Button
            {
                Text = text,
                ForeColor = Color.Black,
                Dock = DockStyle.Right,
                Width = 40,
                FlatStyle = FlatStyle.Flat,
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
                _hostForm.Location = new Point(
                    _hostForm.Location.X + diff.X,
                    _hostForm.Location.Y + diff.Y
                );
            }
        }

        private void ToggleMaximize()
        {
            if (_hostForm.WindowState == FormWindowState.Maximized)
            {
                _hostForm.WindowState = FormWindowState.Normal;
                _maxButton.Text = "□";
            }
            else
            {
                _hostForm.WindowState = FormWindowState.Maximized;
                _maxButton.Text = "❐";
            }
        }
    }
}