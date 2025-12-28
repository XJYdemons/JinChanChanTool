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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputForm));
            textBox_ErrorMessage = new TextBox();
            panel_Background = new Panel();
            panel3 = new Panel();
            label29 = new Label();
            button2 = new Button();
            panel_Dragging = new Panel();
            textBox_OutPut = new TextBox();
            panel1 = new Panel();
            panel_Background.SuspendLayout();
            panel3.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_ErrorMessage
            // 
            textBox_ErrorMessage.Location = new Point(3, 28);
            textBox_ErrorMessage.Multiline = true;
            textBox_ErrorMessage.Name = "textBox_ErrorMessage";
            textBox_ErrorMessage.ScrollBars = ScrollBars.Vertical;
            textBox_ErrorMessage.Size = new Size(299, 120);
            textBox_ErrorMessage.TabIndex = 0;
            textBox_ErrorMessage.Text = "识别错误的字：\r\n";
            // 
            // panel_Background
            // 
            panel_Background.BackColor = Color.White;
            panel_Background.Controls.Add(panel3);
            panel_Background.Controls.Add(panel_Dragging);
            panel_Background.Controls.Add(textBox_OutPut);
            panel_Background.Controls.Add(textBox_ErrorMessage);
            panel_Background.Dock = DockStyle.Fill;
            panel_Background.Location = new Point(3, 3);
            panel_Background.Margin = new Padding(0);
            panel_Background.Name = "panel_Background";
            panel_Background.Size = new Size(999, 153);
            panel_Background.TabIndex = 1;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(label29);
            panel3.Controls.Add(button2);
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(994, 25);
            panel3.TabIndex = 212;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(4, 1);
            label29.MinimumSize = new Size(80, 23);
            label29.Name = "label29";
            label29.Size = new Size(80, 23);
            label29.TabIndex = 10;
            label29.Text = "输出窗口";
            label29.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(969, 1);
            button2.Margin = new Padding(0);
            button2.Name = "button2";
            button2.Size = new Size(23, 23);
            button2.TabIndex = 8;
            button2.Text = "—";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button_最小化_Click;
            // 
            // panel_Dragging
            // 
            panel_Dragging.BackColor = Color.LightGray;
            panel_Dragging.Cursor = Cursors.SizeWE;
            panel_Dragging.ForeColor = Color.LightGray;
            panel_Dragging.Location = new Point(305, 28);
            panel_Dragging.Name = "panel_Dragging";
            panel_Dragging.Size = new Size(4, 120);
            panel_Dragging.TabIndex = 2;
            // 
            // textBox_OutPut
            // 
            textBox_OutPut.Location = new Point(312, 28);
            textBox_OutPut.Multiline = true;
            textBox_OutPut.Name = "textBox_OutPut";
            textBox_OutPut.ScrollBars = ScrollBars.Vertical;
            textBox_OutPut.Size = new Size(679, 120);
            textBox_OutPut.TabIndex = 1;
            textBox_OutPut.Text = "输出：\r\n";
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(250, 250, 250);
            panel1.Controls.Add(panel_Background);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(3, 3, 4, 4);
            panel1.Size = new Size(1006, 160);
            panel1.TabIndex = 2;
            // 
            // OutputForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1006, 160);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "OutputForm";
            ShowInTaskbar = false;
            Text = "ErrorForm";
            TopMost = true;
            panel_Background.ResumeLayout(false);
            panel_Background.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion


        private TextBox textBox_ErrorMessage;
        private Panel panel_Background;
        private TextBox textBox_OutPut;
        private Panel panel_Dragging;
        private Panel panel3;
        private Label label29;
        private Button button2;
        private Panel panel1;
    }
}