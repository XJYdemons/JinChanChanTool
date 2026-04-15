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
            textBox_错误信息 = new TextBox();
            panel_副背景 = new Panel();
            panel_标题栏 = new Panel();
            label_标题 = new Label();
            button_最小化 = new Button();
            panel_分割滑块 = new Panel();
            textBox_输出信息 = new TextBox();
            panel_主背景 = new Panel();
            panel_副背景.SuspendLayout();
            panel_标题栏.SuspendLayout();
            panel_主背景.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_ErrorMessage
            // 
            textBox_错误信息.Location = new Point(3, 28);
            textBox_错误信息.Multiline = true;
            textBox_错误信息.Name = "textBox_ErrorMessage";
            textBox_错误信息.ScrollBars = ScrollBars.Vertical;
            textBox_错误信息.Size = new Size(299, 120);
            textBox_错误信息.TabIndex = 0;
            textBox_错误信息.Text = "";
            // 
            // panel_Background
            // 
            panel_副背景.BackColor = Color.White;
            panel_副背景.Controls.Add(panel_标题栏);
            panel_副背景.Controls.Add(panel_分割滑块);
            panel_副背景.Controls.Add(textBox_输出信息);
            panel_副背景.Controls.Add(textBox_错误信息);
            panel_副背景.Dock = DockStyle.Fill;
            panel_副背景.Location = new Point(3, 3);
            panel_副背景.Margin = new Padding(0);
            panel_副背景.Name = "panel_Background";
            panel_副背景.Size = new Size(999, 153);
            panel_副背景.TabIndex = 1;
            // 
            // panel3
            // 
            panel_标题栏.BackColor = Color.White;
            panel_标题栏.Controls.Add(label_标题);
            panel_标题栏.Controls.Add(button_最小化);
            panel_标题栏.Location = new Point(0, 0);
            panel_标题栏.Name = "panel3";
            panel_标题栏.Size = new Size(994, 25);
            panel_标题栏.TabIndex = 212;
            // 
            // label29
            // 
            label_标题.AutoSize = true;
            label_标题.Location = new Point(4, 1);
            label_标题.MinimumSize = new Size(80, 23);
            label_标题.Name = "label29";
            label_标题.Size = new Size(80, 23);
            label_标题.TabIndex = 10;
            label_标题.Text = "输出窗口";
            label_标题.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button_最小化.FlatAppearance.BorderSize = 0;
            button_最小化.FlatStyle = FlatStyle.Flat;
            button_最小化.Location = new Point(969, 1);
            button_最小化.Margin = new Padding(0);
            button_最小化.Name = "button2";
            button_最小化.Size = new Size(23, 23);
            button_最小化.TabIndex = 8;
            button_最小化.Text = "—";
            button_最小化.UseVisualStyleBackColor = true;
            button_最小化.Click += button_最小化_Click;
            // 
            // panel_Dragging
            // 
            panel_分割滑块.BackColor = Color.LightGray;
            panel_分割滑块.Cursor = Cursors.SizeWE;
            panel_分割滑块.ForeColor = Color.LightGray;
            panel_分割滑块.Location = new Point(305, 28);
            panel_分割滑块.Name = "panel_Dragging";
            panel_分割滑块.Size = new Size(4, 120);
            panel_分割滑块.TabIndex = 2;
            // 
            // textBox_OutPut
            // 
            textBox_输出信息.Location = new Point(312, 28);
            textBox_输出信息.Multiline = true;
            textBox_输出信息.Name = "textBox_OutPut";
            textBox_输出信息.ScrollBars = ScrollBars.Vertical;
            textBox_输出信息.Size = new Size(679, 120);
            textBox_输出信息.TabIndex = 1;
            textBox_输出信息.Text = "";
            // 
            // panel1
            // 
            panel_主背景.BackColor = Color.FromArgb(250, 250, 250);
            panel_主背景.Controls.Add(panel_副背景);
            panel_主背景.Dock = DockStyle.Fill;
            panel_主背景.Location = new Point(0, 0);
            panel_主背景.Margin = new Padding(0);
            panel_主背景.Name = "panel1";
            panel_主背景.Padding = new Padding(3, 3, 4, 4);
            panel_主背景.Size = new Size(1006, 160);
            panel_主背景.TabIndex = 2;
            // 
            // OutputForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1006, 160);
            Controls.Add(panel_主背景);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "OutputForm";
            ShowInTaskbar = false;
            Text = "ErrorForm";
            TopMost = true;
            panel_副背景.ResumeLayout(false);
            panel_副背景.PerformLayout();
            panel_标题栏.ResumeLayout(false);
            panel_标题栏.PerformLayout();
            panel_主背景.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion


        private TextBox textBox_错误信息;
        private Panel panel_副背景;
        private TextBox textBox_输出信息;
        private Panel panel_分割滑块;
        private Panel panel_标题栏;
        private Label label_标题;
        private Button button_最小化;
        private Panel panel_主背景;
    }
}