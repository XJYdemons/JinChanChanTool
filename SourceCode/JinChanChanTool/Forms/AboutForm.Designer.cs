namespace JinChanChanTool
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            linkLabel1 = new LinkLabel();
            label5 = new Label();
            label6 = new Label();
            panel_BackGround = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel_BackGround.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.Control;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(5, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 50);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label1.Location = new Point(61, 15);
            label1.Name = "label1";
            label1.Size = new Size(202, 28);
            label1.TabIndex = 1;
            label1.Text = "JinChanChanTool";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label2.Location = new Point(255, 26);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 2;
            label2.Text = "(64位)";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(0, 61);
            label3.MinimumSize = new Size(174, 17);
            label3.Name = "label3";
            label3.Size = new Size(174, 17);
            label3.TabIndex = 3;
            label3.Text = "版本  v4.5.3(2025.9.9)";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(0, 83);
            label4.MinimumSize = new Size(73, 17);
            label4.Name = "label4";
            label4.Size = new Size(76, 17);
            label4.TabIndex = 4;
            label4.Text = "版权所有 ©️ ";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(73, 83);
            linkLabel1.MinimumSize = new Size(64, 17);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(64, 17);
            linkLabel1.TabIndex = 5;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "XJY工作室";
            linkLabel1.TextAlign = ContentAlignment.MiddleLeft;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.Black;
            label5.Location = new Point(227, 83);
            label5.MinimumSize = new Size(70, 17);
            label5.Name = "label5";
            label5.Size = new Size(70, 17);
            label5.TabIndex = 7;
            label5.Text = "Github主页";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            label5.Click += label5_Click;
            label5.MouseEnter += label5_MouseEnter;
            label5.MouseLeave += label5_MouseLeave;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.Black;
            label6.Location = new Point(241, 61);
            label6.MinimumSize = new Size(56, 17);
            label6.Name = "label6";
            label6.Size = new Size(56, 17);
            label6.TabIndex = 8;
            label6.Text = "项目地址";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            label6.Click += label6_Click;
            label6.MouseEnter += label6_MouseEnter;
            label6.MouseLeave += label6_MouseLeave;
            // 
            // panel_BackGround
            // 
            panel_BackGround.AutoSize = true;
            panel_BackGround.BackColor = Color.White;
            panel_BackGround.Controls.Add(label2);
            panel_BackGround.Controls.Add(label6);
            panel_BackGround.Controls.Add(pictureBox1);
            panel_BackGround.Controls.Add(label5);
            panel_BackGround.Controls.Add(label1);
            panel_BackGround.Controls.Add(linkLabel1);
            panel_BackGround.Controls.Add(label4);
            panel_BackGround.Controls.Add(label3);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(0, 0);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Size = new Size(300, 111);
            panel_BackGround.TabIndex = 9;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(300, 111);
            Controls.Add(panel_BackGround);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "AboutForm";
            Text = "关于";
            TopMost = true;
            Load += AboutForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel_BackGround.ResumeLayout(false);
            panel_BackGround.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private LinkLabel linkLabel1;
        private Label label5;
        private Label label6;
        private Panel panel_BackGround;
    }
}