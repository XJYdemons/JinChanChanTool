namespace JinChanChanTool.Forms
{
    partial class OutputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox_ErrorMessage = new TextBox();
            panel_Background = new Panel();
            panel_Dragging = new Panel();
            textBox_OutPut = new TextBox();
            panel_Background.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_ErrorMessage
            // 
            textBox_ErrorMessage.Location = new Point(10, 10);
            textBox_ErrorMessage.Multiline = true;
            textBox_ErrorMessage.Name = "textBox_ErrorMessage";
            textBox_ErrorMessage.ScrollBars = ScrollBars.Vertical;
            textBox_ErrorMessage.Size = new Size(292, 120);
            textBox_ErrorMessage.TabIndex = 0;
            textBox_ErrorMessage.Text = "识别错误的字：\r\n";
            // 
            // panel_Background
            // 
            panel_Background.AutoScroll = true;
            panel_Background.AutoSize = true;
            panel_Background.BackColor = Color.White;
            panel_Background.Controls.Add(panel_Dragging);
            panel_Background.Controls.Add(textBox_OutPut);
            panel_Background.Controls.Add(textBox_ErrorMessage);
            panel_Background.Dock = DockStyle.Fill;
            panel_Background.Location = new Point(0, 0);
            panel_Background.Name = "panel_Background";
            panel_Background.Size = new Size(1000, 140);
            panel_Background.TabIndex = 1;
            // 
            // panel_Dragging
            // 
            panel_Dragging.BackColor = Color.LightGray;
            panel_Dragging.Cursor = Cursors.SizeWE;
            panel_Dragging.ForeColor = Color.LightGray;
            panel_Dragging.Location = new Point(305, 10);
            panel_Dragging.Name = "panel_Dragging";
            panel_Dragging.Size = new Size(4, 120);
            panel_Dragging.TabIndex = 2;
            // 
            // textBox_OutPut
            // 
            textBox_OutPut.Location = new Point(312, 10);
            textBox_OutPut.Multiline = true;
            textBox_OutPut.Name = "textBox_OutPut";
            textBox_OutPut.ScrollBars = ScrollBars.Vertical;
            textBox_OutPut.Size = new Size(669, 120);
            textBox_OutPut.TabIndex = 1;
            textBox_OutPut.Text = "输出：\r\n";
            // 
            // OutputForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(1000, 140);
            Controls.Add(panel_Background);
            FormBorderStyle = FormBorderStyle.None;
            Name = "OutputForm";
            ShowInTaskbar = false;
            Text = "ErrorForm";
            TopMost = true;
            panel_Background.ResumeLayout(false);
            panel_Background.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


        private TextBox textBox_ErrorMessage;
        private Panel panel_Background;
        private TextBox textBox_OutPut;
        private Panel panel_Dragging;
    }
}