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
    public partial class ErrorForm : Form
    {
        private static ErrorForm _instance;
        
        public static ErrorForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new ErrorForm();
                }
                return _instance;
            }
            
        }
        private ErrorForm()
        {
            InitializeComponent();
            #region 自定义标题栏
            // 自定义标题栏,带图标、带标题、最小化按钮。
            CustomTitleBar titleBar = new CustomTitleBar(this, 32,null , "识别错误输出窗口",CustomTitleBar.ButtonOptions.Minimize);
            this.Controls.Add(titleBar);
            #endregion
            
            
        }
        public  TextBox GetTextBox()
        {           
            return textBox1;
        }
      
    }
}
