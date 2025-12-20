using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services.DataServices.Interface;
using System.Reflection.Metadata;

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
        private bool _isDragged; // 标志位：是否发生了真正的拖动
        private const int DRAG_THRESHOLD = 3; // 拖动阈值（像素）

        /// <summary>
        /// 获取当前是否发生了拖动（用于区分拖动和点击）
        /// </summary>
        public bool IsDragged => _isDragged;

        public IAutomaticSettingsService _iAutoConfigService;// 自动设置数据服务对象
        private SelectForm()
        {           
            InitializeComponent();
            // 鼠标事件处理
           
            
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
        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _isDragged = false; // 重置拖动标志位
                _dragStartPoint = new Point(e.X, e.Y);
            }
        }

        /// <summary>
        /// 鼠标移动事件 - 处理拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                // 计算鼠标移动距离
                int deltaX = e.X - _dragStartPoint.X;
                int deltaY = e.Y - _dragStartPoint.Y;
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                // 只有移动距离超过阈值才认为是真正的拖动
                if (distance > DRAG_THRESHOLD)
                {
                    _isDragged = true;
                    Cursor = Cursors.SizeAll;

                    Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                    newLocation.Offset(-_dragStartPoint.X, -_dragStartPoint.Y);
                    this.Location = newLocation;
                }
            }
        }

        /// <summary>
        /// 鼠标释放事件 - 结束拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                _dragging = false;
                Cursor = Cursors.Arrow;

                if (_isDragged)
                {
                    SaveFormLocation();
                }

                // 延迟重置拖动标志位，让其他 MouseUp 事件处理器能够检查到
                BeginInvoke(new Action(() => _isDragged = false));
            }
        }

        public void 绑定拖动(Control 要拖动的控件)
        {
            要拖动的控件.MouseDown -= panel_MouseDown;
            要拖动的控件.MouseMove -= panel_MouseMove;
            要拖动的控件.MouseUp -= panel_MouseUp;
            要拖动的控件.MouseDown += panel_MouseDown;
            要拖动的控件.MouseMove += panel_MouseMove;
            要拖动的控件.MouseUp += panel_MouseUp;
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

        public void InitializeComponents(int numberOfTypes)
        {
           
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
