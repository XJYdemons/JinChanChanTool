using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinChanChanTool.Forms
{
    public partial class Selector : Form
    {
        private static Selector _instance;
        public static Selector Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new Selector();
                }
                return _instance;
            }
        }
        private Selector()
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
                panel.BackColor =Color.FromArgb(96, 223, 84); 
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
            SaveFormLocation();
        }
        #endregion

        public IAppConfigService _iAppConfigService;
        public void InitializeObject(IAppConfigService iAppConfigService)
        {           
            _iAppConfigService = iAppConfigService;
            ApplySavedLocation();
        }
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
        /// 从配置中读取并应用窗口位置
        /// </summary>
        private void ApplySavedLocation()
        {
            try
            {
                this.StartPosition = FormStartPosition.Manual;        
                if(_iAppConfigService.CurrentConfig.SelectorFormLocation.X==-1&& _iAppConfigService.CurrentConfig.SelectorFormLocation.Y == -1)
                {
                    this.Location = new Point(0, 0);
                    return;
                }
                // 确保坐标在屏幕范围内
                if (Screen.AllScreens.Any(s => s.Bounds.Contains(_iAppConfigService.CurrentConfig.SelectorFormLocation)))
                {
                    this.Location = _iAppConfigService.CurrentConfig.SelectorFormLocation;
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
        /// 拖动结束时保存窗口位置到配置服务
        /// </summary>
        private void SaveFormLocation()
        {           
            try
            {
                if (_iAppConfigService != null)
                {
                    _iAppConfigService.CurrentConfig.SelectorFormLocation = this.Location;
                    _iAppConfigService.Save();
                }
            }
            catch (Exception ex)
            {                
            }
        }

        #endregion
    }
}
