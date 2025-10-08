namespace JinChanChanTool.Forms
{
    partial class LineUpForm
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
            panel1 = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            panel2 = new Panel();
            flowLayoutPanel3 = new FlowLayoutPanel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            button_清空 = new Button();
            button_保存 = new Button();
            comboBox_LineUp = new ComboBox();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(218, 218, 218);
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Location = new Point(0, 30);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(6, 108);
            panel1.TabIndex = 1;
            panel1.MouseDoubleClick += panel1_MouseDoubleClick;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.BackColor = SystemColors.Control;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(387, 33);
            flowLayoutPanel1.TabIndex = 4;
            flowLayoutPanel1.WrapContents = false;
            // 
            // panel2
            // 
            panel2.AutoScroll = true;
            panel2.Controls.Add(flowLayoutPanel3);
            panel2.Controls.Add(flowLayoutPanel2);
            panel2.Controls.Add(flowLayoutPanel1);
            panel2.Location = new Point(6, 30);
            panel2.Margin = new Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new Size(391, 126);
            panel2.TabIndex = 5;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.AutoSize = true;
            flowLayoutPanel3.BackColor = SystemColors.Control;
            flowLayoutPanel3.Location = new Point(0, 74);
            flowLayoutPanel3.Margin = new Padding(0);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(387, 33);
            flowLayoutPanel3.TabIndex = 6;
            flowLayoutPanel3.WrapContents = false;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.BackColor = SystemColors.Control;
            flowLayoutPanel2.Location = new Point(0, 37);
            flowLayoutPanel2.Margin = new Padding(0);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(387, 33);
            flowLayoutPanel2.TabIndex = 5;
            flowLayoutPanel2.WrapContents = false;
            // 
            // button_清空
            // 
            button_清空.BackColor = SystemColors.Control;
            button_清空.FlatStyle = FlatStyle.Flat;
            button_清空.Location = new Point(253, 0);
            button_清空.Name = "button_清空";
            button_清空.Size = new Size(75, 25);
            button_清空.TabIndex = 9;
            button_清空.Text = "清空";
            button_清空.UseVisualStyleBackColor = false;
            button_清空.Click += button2_Click;
            // 
            // button_保存
            // 
            button_保存.BackColor = SystemColors.Control;
            button_保存.FlatStyle = FlatStyle.Flat;
            button_保存.Location = new Point(173, 0);
            button_保存.Name = "button_保存";
            button_保存.Size = new Size(75, 25);
            button_保存.TabIndex = 8;
            button_保存.Text = "保存";
            button_保存.UseVisualStyleBackColor = false;
            button_保存.Click += button1_Click;
            // 
            // comboBox_LineUp
            // 
            comboBox_LineUp.FormattingEnabled = true;
            comboBox_LineUp.Location = new Point(0, 0);
            comboBox_LineUp.Name = "comboBox_LineUp";
            comboBox_LineUp.Size = new Size(168, 25);
            comboBox_LineUp.TabIndex = 7;
            // 
            // LineUpForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Magenta;
            ClientSize = new Size(399, 157);
            Controls.Add(button_清空);
            Controls.Add(button_保存);
            Controls.Add(panel2);
            Controls.Add(comboBox_LineUp);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LineUpForm";
            Text = "Selector";
            TopMost = true;
            TransparencyKey = Color.Magenta;
            Load += LineUpForm_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel1;
        private Panel panel2;
        public FlowLayoutPanel flowLayoutPanel1;
        public FlowLayoutPanel flowLayoutPanel3;
        public FlowLayoutPanel flowLayoutPanel2;
        public ComboBox comboBox_LineUp;
        private Button button_清空;
        private Button button_保存;
    }
}