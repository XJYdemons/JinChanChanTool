using JinChanChanTool.DIYComponents;

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
            flowLayoutPanel1 = new CustomFlowLayoutPanel();
            button_清空 = new Button();
            button_保存 = new Button();
            comboBox_LineUpSelected = new ComboBox();
            button__变阵1 = new Button();
            button__变阵2 = new Button();
            button__变阵3 = new Button();
            button_阵容推荐 = new Button();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.BackColor = Color.FromArgb(45, 45, 48);
            flowLayoutPanel1.Location = new Point(2, 27);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(2);
            flowLayoutPanel1.Size = new Size(625, 64);
            flowLayoutPanel1.TabIndex = 4;
            flowLayoutPanel1.WrapContents = false;
            // 
            // button_清空
            // 
            button_清空.BackColor = Color.FromArgb(45, 45, 48);
            button_清空.FlatAppearance.BorderColor = Color.Gray;
            button_清空.FlatStyle = FlatStyle.Flat;
            button_清空.ForeColor = SystemColors.Control;
            button_清空.Location = new Point(464, 1);
            button_清空.Name = "button_清空";
            button_清空.Size = new Size(60, 25);
            button_清空.TabIndex = 9;
            button_清空.Text = "清空";
            button_清空.UseVisualStyleBackColor = false;
            button_清空.Click += button_清空_Click;
            // 
            // button_保存
            // 
            button_保存.BackColor = Color.FromArgb(45, 45, 48);
            button_保存.FlatAppearance.BorderColor = Color.Gray;
            button_保存.FlatStyle = FlatStyle.Flat;
            button_保存.ForeColor = SystemColors.Control;
            button_保存.Location = new Point(400, 1);
            button_保存.Name = "button_保存";
            button_保存.Size = new Size(60, 25);
            button_保存.TabIndex = 8;
            button_保存.Text = "保存";
            button_保存.UseVisualStyleBackColor = false;
            button_保存.Click += button_保存_Click;
            // 
            // comboBox_LineUpSelected
            // 
            comboBox_LineUpSelected.BackColor = Color.White;
            comboBox_LineUpSelected.ForeColor = Color.Black;
            comboBox_LineUpSelected.FormattingEnabled = true;
            comboBox_LineUpSelected.Location = new Point(226, 1);
            comboBox_LineUpSelected.Name = "comboBox_LineUpSelected";
            comboBox_LineUpSelected.Size = new Size(168, 25);
            comboBox_LineUpSelected.TabIndex = 7;
            // 
            // button__变阵1
            // 
            button__变阵1.BackColor = Color.FromArgb(45, 45, 48);
            button__变阵1.FlatAppearance.BorderColor = Color.Gray;
            button__变阵1.FlatStyle = FlatStyle.Flat;
            button__变阵1.ForeColor = Color.White;
            button__变阵1.Location = new Point(2, 2);
            button__变阵1.Name = "button__变阵1";
            button__变阵1.Size = new Size(75, 25);
            button__变阵1.TabIndex = 10;
            button__变阵1.Text = "前期";
            button__变阵1.UseVisualStyleBackColor = false;
            button__变阵1.Click += button__变阵1_Click;
            button__变阵1.MouseUp += button__变阵1_MouseUp;
            // 
            // button__变阵2
            // 
            button__变阵2.BackColor = Color.FromArgb(45, 45, 48);
            button__变阵2.FlatAppearance.BorderColor = Color.Gray;
            button__变阵2.FlatStyle = FlatStyle.Flat;
            button__变阵2.ForeColor = Color.White;
            button__变阵2.Location = new Point(76, 2);
            button__变阵2.Name = "button__变阵2";
            button__变阵2.Size = new Size(75, 25);
            button__变阵2.TabIndex = 11;
            button__变阵2.Text = "中期";
            button__变阵2.UseVisualStyleBackColor = false;
            button__变阵2.Click += button__变阵2_Click;
            button__变阵2.MouseUp += button__变阵2_MouseUp;
            // 
            // button__变阵3
            // 
            button__变阵3.BackColor = Color.FromArgb(45, 45, 48);
            button__变阵3.FlatAppearance.BorderColor = Color.Gray;
            button__变阵3.FlatStyle = FlatStyle.Flat;
            button__变阵3.ForeColor = Color.White;
            button__变阵3.Location = new Point(150, 2);
            button__变阵3.Name = "button__变阵3";
            button__变阵3.Size = new Size(75, 25);
            button__变阵3.TabIndex = 12;
            button__变阵3.Text = "后期";
            button__变阵3.UseVisualStyleBackColor = false;
            button__变阵3.Click += button__变阵3_Click;
            button__变阵3.MouseUp += button__变阵3_MouseUp;
            // 
            // button_阵容推荐
            // 
            button_阵容推荐.BackColor = Color.FromArgb(45, 45, 48);
            button_阵容推荐.FlatAppearance.BorderColor = Color.Gray;
            button_阵容推荐.FlatStyle = FlatStyle.Flat;
            button_阵容推荐.ForeColor = SystemColors.Control;
            button_阵容推荐.Location = new Point(530, 1);
            button_阵容推荐.Name = "button_阵容推荐";
            button_阵容推荐.Size = new Size(97, 25);
            button_阵容推荐.TabIndex = 13;
            button_阵容推荐.Text = "阵容推荐";
            button_阵容推荐.UseVisualStyleBackColor = false;
            button_阵容推荐.Click += button_阵容推荐_Click;
            // 
            // LineUpForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Magenta;
            ClientSize = new Size(632, 95);
            Controls.Add(button_阵容推荐);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(button__变阵3);
            Controls.Add(button__变阵2);
            Controls.Add(button__变阵1);
            Controls.Add(button_清空);
            Controls.Add(button_保存);
            Controls.Add(comboBox_LineUpSelected);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LineUpForm";
            ShowInTaskbar = false;
            Text = "LineUpForm";
            TopMost = true;
            TransparencyKey = Color.Magenta;
            Load += LineUpForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public CustomFlowLayoutPanel flowLayoutPanel1;
        private ComboBox comboBox_LineUpSelected;
        private Button button_清空;
        private Button button_保存;
        private Button button__变阵1;
        private Button button__变阵2;
        private Button button__变阵3;
        private Button button_阵容推荐;
    }
}
