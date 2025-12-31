using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinChanChanTool.Forms
{
    public partial class OutputForm : Form
    {
        // 单例模式
        private static OutputForm _instance;
        
        public static OutputForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new OutputForm();
                }
                return _instance;
            }
            
        }

        // 拖动相关的变量
        private bool IsDragging = false;
        private const int MinWidth = 100; // 最小宽度限制

        private OutputForm()
        {
            InitializeComponent();
            DragHelper.EnableDragForChildren(panel3);

            //绑定拖动事件            
            panel_Dragging.MouseDown += Panel_Dragging_MouseDown;
            panel_Dragging.MouseMove += Panel_Dragging_MouseMove;
            panel_Dragging.MouseUp += Panel_Dragging_MouseUp;
            panel_Dragging.MouseLeave += Panel_Dragging_MouseLeave;
         
        }
        public IAutomaticSettingsService _iAutoConfigService;//自动设置数据服务对象
        /// <summary>
        /// 初始化配置服务
        /// </summary>
        /// <param name="iAutoConfigService">自动配置服务</param>
        /// <param name="cardService">卡牌服务</param>
        public void InitializeObject(IAutomaticSettingsService iAutoConfigService)
        {
            _iAutoConfigService = iAutoConfigService;            
        }

        /// <summary>
        /// 向错误信息文本框中写入错误信息
        /// </summary>
        /// <param name="message"></param>
        public void WriteErrorMessage(string message)
        {
            textBox_ErrorMessage.Invoke((MethodInvoker)delegate
            {
                textBox_ErrorMessage.AppendText(message);
            });
        }

        /// <summary>
        /// 向错误信息文本框中写入错误信息（自动换行）
        /// </summary>
        /// <param name="message"></param>
        public void WriteLineErrorMessage(string message)
        {
            textBox_ErrorMessage.Invoke((MethodInvoker)delegate
            {
                textBox_ErrorMessage.AppendText(message+"\r\n");
            });
        }

        /// <summary>
        /// 向输出信息文本框中写入输出信息
        /// </summary>
        /// <param name="message"></param>
        public void WriteOutputMessage(string message)
        {
            textBox_OutPut.Invoke((MethodInvoker)delegate
            {
                textBox_OutPut.AppendText(message);
            });
        }

        /// <summary>
        /// 向输出信息文本框中写入输出信息（自动换行）
        /// </summary>
        /// <param name="message"></param>
        public void WriteLineOutputMessage(string message)
        {
            textBox_OutPut.Invoke((MethodInvoker)delegate
            {
                textBox_OutPut.AppendText(message + "\r\n");
            });
        }

        /// <summary>
        /// 鼠标按下时开始拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Dragging_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsDragging = true;                     
                panel_Dragging.Capture = true;
            }
        }

        /// <summary>
        /// 鼠标移动时调整两个TextBox的宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Dragging_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragging)
            {
                int OffsetX = e.X;
                int NewDraggingBarLeft = panel_Dragging.Left + OffsetX;
                int NewErrorMessageTextBoxWidth = textBox_ErrorMessage.Width + OffsetX;
                int NewOutputTextBoxWidth = textBox_OutPut.Width - OffsetX;
                int NewOutputTextBoxLeft = textBox_OutPut.Left + OffsetX;
                if (NewErrorMessageTextBoxWidth < MinWidth || NewOutputTextBoxWidth < MinWidth)
                {
                    // 达到最小宽度，停止调整
                    return;
                }
                panel_Dragging.Left = NewDraggingBarLeft;
                textBox_ErrorMessage.Width = NewErrorMessageTextBoxWidth;
                if (OffsetX >= 0)
                {
                    textBox_OutPut.Width = NewOutputTextBoxWidth;
                    textBox_OutPut.Left = NewOutputTextBoxLeft;
                }
                else
                {
                    textBox_OutPut.Left = NewOutputTextBoxLeft;
                    textBox_OutPut.Width = NewOutputTextBoxWidth;
                }
                
            }
        }

        /// <summary>
        /// 鼠标松开时停止拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Dragging_MouseUp(object sender, MouseEventArgs e)
        {
            IsDragging = false;
            panel_Dragging.Capture = false;
        }

        /// <summary>
        /// 鼠标离开拖动区域时停止拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Dragging_MouseLeave(object sender, EventArgs e)
        {
            // 鼠标离开时停止拖动
            if (IsDragging)
            {
                IsDragging = false;
                panel_Dragging.Capture = false;
            }
        }

        #region 圆角实现
        // GDI32 API - 用于创建圆角效果
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        // 圆角半径
        private const int CORNER_RADIUS = 16;

        /// <summary>
        /// 在窗口句柄创建后应用圆角效果
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // 应用 GDI Region 圆角效果（支持 Windows 10 和 Windows 11）
            ApplyRoundedCorners();
        }

        /// <summary>
        /// 应用 GDI Region 圆角效果
        /// </summary>
        private void ApplyRoundedCorners()
        {
            try
            {
                // 创建圆角矩形区域
                IntPtr region = CreateRoundRectRgn(0, 0, Width, Height, CORNER_RADIUS, CORNER_RADIUS);

                if (region != IntPtr.Zero)
                {
                    SetWindowRgn(Handle, region, true);
                    // 注意：SetWindowRgn 会接管 region 的所有权，不需要手动删除

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 窗口大小改变时重新应用圆角
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 调整大小时重新创建圆角区域
            if (Handle != IntPtr.Zero)
            {
                ApplyRoundedCorners();
            }
        }
        #endregion

        #region 标题栏按钮事件
        private void button_最小化_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region 位置保存与读取

        /// <summary>
        /// Windows消息常量 - 窗口移动或大小调整结束
        /// </summary>
        private const int WM_EXITSIZEMOVE = 0x0232;

        /// <summary>
        /// 重写窗口过程以监听拖动结束消息
        /// </summary>
        /// <param name="m">Windows消息</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 监听窗口移动结束消息
            if (m.Msg == WM_EXITSIZEMOVE)
            {
                // 拖动结束,保存位置
                SaveFormLocation();
            }
        }

        /// <summary>
        /// 拖动结束时保存窗口位置到配置服务
        /// </summary>
        private void SaveFormLocation()
        {
            //Debug.WriteLine($"StatusOverlayForm - 保存位置: {this.Location}");
            try
            {
                if (_iAutoConfigService != null)
                {
                    _iAutoConfigService.CurrentConfig.OutputFormLocation = this.Location;
                    _iAutoConfigService.Save();

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
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
                if (_iAutoConfigService.CurrentConfig.OutputFormLocation.X == -1 && _iAutoConfigService.CurrentConfig.OutputFormLocation.Y == -1)
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Left + screen.Width / 2 - this.Width / 2 /*- 10*/,
                        screen.Top  /*+ 10*/
                    );
                    return;
                }
                // 确保坐标在屏幕范围内
                if (Screen.AllScreens.Any(s => s.Bounds.Contains(_iAutoConfigService.CurrentConfig.OutputFormLocation)))
                {
                    this.Location = _iAutoConfigService.CurrentConfig.OutputFormLocation;
                }
                else
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        screen.Left + screen.Width / 2 - this.Width / 2 /*- 10*/,
                        screen.Top  /*+ 10*/
                    );
                }
            }
            catch
            {
                var screen = Screen.PrimaryScreen.Bounds;
                this.Location = new Point(
                    screen.Left + screen.Width / 2 - this.Width / 2 /*- 10*/,
                    screen.Top  /*+ 10*/
                );
            }
        }
        #endregion
    }
}
