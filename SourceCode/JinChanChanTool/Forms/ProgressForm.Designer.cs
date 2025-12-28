namespace JinChanChanTool.Forms
{
    partial class ProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            progressBar1 = new ProgressBar();
            lblStatus = new Label();
            panel_BackGround = new Panel();
            panel3 = new Panel();
            label5 = new Label();
            button2 = new Button();
            button1 = new Button();
            panel_BackGround.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(5, 34);
            progressBar1.Margin = new Padding(5);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(326, 27);
            progressBar1.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(15, 48);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 17);
            lblStatus.TabIndex = 1;
            // 
            // panel_BackGround
            // 
            panel_BackGround.BackColor = Color.White;
            panel_BackGround.Controls.Add(panel3);
            panel_BackGround.Controls.Add(progressBar1);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(0, 0);
            panel_BackGround.Margin = new Padding(0);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Padding = new Padding(5);
            panel_BackGround.Size = new Size(336, 72);
            panel_BackGround.TabIndex = 2;
            // 
            // panel3
            // 
            panel3.Controls.Add(label5);
            panel3.Controls.Add(button2);
            panel3.Controls.Add(button1);
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(336, 25);
            panel3.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(1, 1);
            label5.MinimumSize = new Size(80, 23);
            label5.Name = "label5";
            label5.Size = new Size(200, 23);
            label5.TabIndex = 10;
            label5.Text = "正在更新当前赛季英雄推荐装备数据";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(290, 1);
            button2.Margin = new Padding(0);
            button2.Name = "button2";
            button2.Size = new Size(23, 23);
            button2.TabIndex = 8;
            button2.Text = "—";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button_最小化_Click;
            // 
            // button1
            // 
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(313, 1);
            button1.Margin = new Padding(0);
            button1.Name = "button1";
            button1.Size = new Size(23, 23);
            button1.TabIndex = 7;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button_关闭_Click;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(336, 72);
            Controls.Add(panel_BackGround);
            Controls.Add(lblStatus);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ProgressForm";
            Text = "获取进度";
            TopMost = true;
            panel_BackGround.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar1;
        private Label lblStatus;
        private Panel panel_BackGround;
        private Panel panel3;
        private Label label5;
        private Button button2;
        private Button button1;
    }
}