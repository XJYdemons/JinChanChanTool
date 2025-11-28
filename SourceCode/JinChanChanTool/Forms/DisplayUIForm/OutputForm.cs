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
            #region 自定义标题栏
            // 自定义标题栏,带图标、带标题、最小化按钮。
            CustomTitleBar titleBar = new CustomTitleBar(this, 32,null , "输出窗口",CustomTitleBar.ButtonOptions.None);
            this.Controls.Add(titleBar);
            #endregion            

            //绑定拖动事件            
            panel_Dragging.MouseDown += Panel_Dragging_MouseDown;
            panel_Dragging.MouseMove += Panel_Dragging_MouseMove;
            panel_Dragging.MouseUp += Panel_Dragging_MouseUp;
            panel_Dragging.MouseLeave += Panel_Dragging_MouseLeave;
         
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
    }
}
