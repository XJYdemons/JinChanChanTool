using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            // 设置窗体初始位置为屏幕左上角
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
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

        }
        #endregion

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
    }
}
