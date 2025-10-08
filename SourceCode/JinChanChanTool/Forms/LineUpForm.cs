using JinChanChanTool.Services.DataServices;


namespace JinChanChanTool.Forms
{
    public partial class LineUpForm : Form
    {
        private static LineUpForm _instance;
        public static LineUpForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new LineUpForm();
                }
                return _instance;
            }
        }
        private LineUpForm()
        {
            InitializeComponent();
            // 鼠标事件处理
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseMove += panel1_MouseMove;
            panel1.MouseUp += panel1_MouseUp;
        }

        private void LineUpForm_Load(object sender, EventArgs e)
        {

        }

        #region 拖动窗体功能
        // 拖动相关变量
        private Point _dragStartPoint;
        private bool _dragging;
        // 鼠标按下事件 - 开始拖动
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = sender as Panel;
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
                panel.BackColor = Color.FromArgb(96, 223, 84);
            }
        }

        // 鼠标移动事件 - 处理拖动
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-_dragStartPoint.X, -_dragStartPoint.Y);
                this.Location = newLocation;
            }
        }

        // 鼠标释放事件 - 结束拖动
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            Panel panel = sender as Panel;
            panel.BackColor = Color.FromArgb(218, 218, 218);
            _dragging = false;
        }
        #endregion

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (panel2.Visible == true)
            {
                panel2.Visible = false;
                comboBox_LineUp.Visible = false;
                button_保存.Visible = false;
                button_清空.Visible = false;
            }
            else
            {
                panel2.Visible = true;
                comboBox_LineUp.Visible = true;
                button_保存.Visible = true;
                button_清空.Visible = true;
            }
        }
        private  ILineUpService _ilineUpService;
        public void InitializeObject(ILineUpService ilineUpService)
        {
            _ilineUpService= ilineUpService;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (_ilineUpService.Save())
            {
                MessageBox.Show("阵容已保存", "阵容已保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _ilineUpService.ClearCurrentSubLineUp();
        }
    }
}
