using JinChanChanTool.Services.DataServices.Interface;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 选择英雄窗体
    /// </summary>
    public partial class SelectForm : Form
    {
        //单例模式
        private static SelectForm _instance;
        public static SelectForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new SelectForm();
                }
                return _instance;
            }
        }

        // 拖动相关变量
        private Point _dragStartPoint;
        private bool _dragging;

        public IAutomaticSettingsService _iAutoConfigService;// 自动设置数据服务对象
        private SelectForm()
        {           
            InitializeComponent();
            // 鼠标事件处理
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseMove += panel1_MouseMove;
            panel1.MouseUp += panel1_MouseUp;
        }

        private void Selector_Load(object sender, EventArgs e)
        {                       
        }

        #region 拖动窗体功能
        
        /// <summary>
        /// 鼠标按下事件 - 开始拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = sender as Panel;
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
                panel.BackColor =Color.FromArgb(96, 223, 84); 
            }
        }

        /// <summary>
        /// 鼠标移动事件 - 处理拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseMove(object sender, MouseEventArgs e)
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
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            Panel panel = sender as Panel;
            panel.BackColor = Color.FromArgb(218, 218, 218);
            _dragging = false;
            SaveFormLocation();
        }
        #endregion

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="iAutoConfigService"></param>
        public void InitializeObject(IAutomaticSettingsService iAutoConfigService)
        {           
            _iAutoConfigService = iAutoConfigService;
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
                if (_iAutoConfigService.CurrentConfig.SelectFormLocation.X == -1 && _iAutoConfigService.CurrentConfig.SelectFormLocation.Y == -1)
                {
                    this.Location = new Point(0, 0);
                    return;
                }
                // 确保坐标在屏幕范围内
                if (Screen.AllScreens.Any(s => s.Bounds.Contains(_iAutoConfigService.CurrentConfig.SelectFormLocation)))
                {
                    this.Location = _iAutoConfigService.CurrentConfig.SelectFormLocation;
                }
                else
                {
                    this.Location = new Point(0, 0); // 超出屏幕则重置为左上角                        
                }
            }
            catch
            {
                this.Location = new Point(0, 0); // 出错时兜底
            }
        }

        /// <summary>
        /// 面板双击事件，切换可见性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (panel2.Visible == true)
            {
                panel2.Visible = false;
            }
            else
            {
                panel2.Visible = true;
            }
        }

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
                    _iAutoConfigService.CurrentConfig.SelectFormLocation = this.Location;
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
