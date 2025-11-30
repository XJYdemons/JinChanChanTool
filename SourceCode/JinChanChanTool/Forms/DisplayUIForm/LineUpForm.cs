using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services.DataServices.Interface;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 阵容选择与展示窗体
    /// </summary>
    public partial class LineUpForm : Form
    {
        //单例模式
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

        // 拖动相关变量
        private Point _dragStartPoint;
        private bool _dragging;

        private ILineUpService _ilineUpService;//阵容数据服务对象
        public IAutomaticSettingsService _iAutoConfigService;//自动设置数据服务对象

        private LineUpForm()
        {
            InitializeComponent();
            // 鼠标事件处理
            draggingBar.MouseDown += panel_拖动条_MouseDown;
            draggingBar.MouseMove += panel_拖动条_MouseMove;
            draggingBar.MouseUp += panel_拖动条_MouseUp;
        }

        private void LineUpForm_Load(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// 初始化阵容窗体所需的服务
        /// </summary>
        /// <param name="ilineUpService">阵容数据服务对象</param>
        /// <param name="iAutoConfigService">程序自动设置数据服务对象</param>        
        public void InitializeObject(ILineUpService ilineUpService, IAutomaticSettingsService iAutoConfigService)
        {
            _iAutoConfigService = iAutoConfigService;
            _ilineUpService = ilineUpService;
            ApplySavedLocation();
        }

        /// <summary>
        /// 从配置中读取并应用窗口位置
        /// </summary>
        private void ApplySavedLocation()
        {
            try
            {
                this.StartPosition = FormStartPosition.Manual;
                if (_iAutoConfigService.CurrentConfig.LineUpFormLocation.X == -1 && _iAutoConfigService.CurrentConfig.LineUpFormLocation.Y == -1)
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Right - this.Width,
                        screen.Bottom - this.Height
                    );
                    return;
                }
                // 确保坐标在屏幕范围内
                if (Screen.AllScreens.Any(s => s.Bounds.Contains(_iAutoConfigService.CurrentConfig.LineUpFormLocation)))
                {
                    this.Location = _iAutoConfigService.CurrentConfig.LineUpFormLocation;
                }
                else
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Right - this.Width,
                        screen.Bottom - this.Height
                    );
                }
            }
            catch
            {
                var screen = Screen.PrimaryScreen.Bounds;
                this.Location = new Point(
                    screen.Right - this.Width,
                    screen.Bottom - this.Height
                );
            }
        }

        #region 拖动窗体功能        
        /// <summary>
        /// 鼠标按下事件 - 开始拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel_拖动条_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Panel)
            {
                if (e.Button == MouseButtons.Left)
                {
                    draggingBar.BackColor = Color.FromArgb(96, 223, 84);
                    _dragging = true;
                    _dragStartPoint = new Point(e.X, e.Y);
                    //改变鼠标光标
                    Cursor = Cursors.SizeAll;
                }
            }            
        }

        /// <summary>
        /// 鼠标移动事件 - 处理拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel_拖动条_MouseMove(object sender, MouseEventArgs e)
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
        private void panel_拖动条_MouseUp(object sender, MouseEventArgs e)
        {
            draggingBar.BackColor = Color.FromArgb(218, 218, 218);
            _dragging = false;
            Cursor = Cursors.Arrow;
            SaveFormLocation();           
        }
        #endregion

        /// <summary>
        /// 双击拖动条显示或隐藏阵容选择面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (panel_子阵容容器.Visible == true)
            {
                panel_子阵容容器.Visible = false;
                comboBox_LineUp.Visible = false;
                button_保存.Visible = false;
                button_清空.Visible = false;
            }
            else
            {
                panel_子阵容容器.Visible = true;
                comboBox_LineUp.Visible = true;
                button_保存.Visible = true;
                button_清空.Visible = true;
            }
        }

        /// <summary>
        /// 保存当前阵容按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_保存_Click(object sender, EventArgs e)
        {
            if (_ilineUpService.Save())
            {
                MessageBox.Show("阵容已保存", "阵容已保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 清空当前阵容按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_清空_Click(object sender, EventArgs e)
        {
            _ilineUpService.ClearCurrentSubLineUp();
        }

        /// <summary>
        /// 拖动结束时保存窗口位置到配置服务
        /// </summary>
        private void SaveFormLocation()
        {          
            try
            {
                if (_iAutoConfigService != null)
                {
                    _iAutoConfigService.CurrentConfig.LineUpFormLocation = this.Location;
                    _iAutoConfigService.Save();
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
