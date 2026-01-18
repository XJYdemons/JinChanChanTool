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
            label_程序名 = new Label();
            label_64位 = new Label();
            label_版本号 = new Label();
            label_版权所有 = new Label();
            linkLabel1 = new LinkLabel();
            label_Github主页 = new Label();
            label_项目地址 = new Label();
            panel_BackGround = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            label5 = new Label();
            button2 = new Button();
            button1 = new Button();
            panel4 = new Panel();
            panel1 = new Panel();
            linkLabel3 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            label2 = new Label();
            label1 = new Label();
            panel_About = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel_BackGround.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel1.SuspendLayout();
            panel_About.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.White;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(5, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 50);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // label_程序名
            // 
            label_程序名.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label_程序名.Location = new Point(60, 16);
            label_程序名.Name = "label_程序名";
            label_程序名.Size = new Size(197, 28);
            label_程序名.TabIndex = 1;
            label_程序名.Text = "JinChanChanTool";
            label_程序名.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_64位
            // 
            label_64位.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label_64位.Location = new Point(255, 27);
            label_64位.Name = "label_64位";
            label_64位.Size = new Size(44, 17);
            label_64位.TabIndex = 2;
            label_64位.Text = "(64位)";
            label_64位.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label_版本号
            // 
            label_版本号.AutoSize = true;
            label_版本号.Location = new Point(5, 60);
            label_版本号.MinimumSize = new Size(174, 17);
            label_版本号.Name = "label_版本号";
            label_版本号.Size = new Size(174, 17);
            label_版本号.TabIndex = 3;
            label_版本号.Text = "版本  v7.3.1beta(2026.01.17)";
            label_版本号.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_版权所有
            // 
            label_版权所有.AutoSize = true;
            label_版权所有.Location = new Point(5, 82);
            label_版权所有.MinimumSize = new Size(73, 17);
            label_版权所有.Name = "label_版权所有";
            label_版权所有.Size = new Size(76, 17);
            label_版权所有.TabIndex = 4;
            label_版权所有.Text = "版权所有 ©️ ";
            label_版权所有.TextAlign = ContentAlignment.MiddleRight;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(78, 82);
            linkLabel1.MinimumSize = new Size(64, 17);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(64, 17);
            linkLabel1.TabIndex = 5;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "XJY工作室";
            linkLabel1.TextAlign = ContentAlignment.MiddleLeft;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label_Github主页
            // 
            label_Github主页.AutoSize = true;
            label_Github主页.ForeColor = Color.Black;
            label_Github主页.Location = new Point(224, 82);
            label_Github主页.MinimumSize = new Size(70, 17);
            label_Github主页.Name = "label_Github主页";
            label_Github主页.Size = new Size(70, 17);
            label_Github主页.TabIndex = 7;
            label_Github主页.Text = "Github主页";
            label_Github主页.TextAlign = ContentAlignment.MiddleLeft;
            label_Github主页.Click += label5_Click;
            label_Github主页.MouseEnter += label5_MouseEnter;
            label_Github主页.MouseLeave += label5_MouseLeave;
            // 
            // label_项目地址
            // 
            label_项目地址.AutoSize = true;
            label_项目地址.ForeColor = Color.Black;
            label_项目地址.Location = new Point(238, 60);
            label_项目地址.MinimumSize = new Size(56, 17);
            label_项目地址.Name = "label_项目地址";
            label_项目地址.Size = new Size(56, 17);
            label_项目地址.TabIndex = 8;
            label_项目地址.Text = "项目地址";
            label_项目地址.TextAlign = ContentAlignment.MiddleLeft;
            label_项目地址.Click += label6_Click;
            label_项目地址.MouseEnter += label6_MouseEnter;
            label_项目地址.MouseLeave += label6_MouseLeave;
            // 
            // panel_BackGround
            // 
            panel_BackGround.BackColor = Color.FromArgb(250, 250, 250);
            panel_BackGround.Controls.Add(panel2);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(0, 0);
            panel_BackGround.Margin = new Padding(0);
            panel_BackGround.MinimumSize = new Size(314, 225);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Padding = new Padding(3, 3, 4, 4);
            panel_BackGround.Size = new Size(318, 248);
            panel_BackGround.TabIndex = 9;
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(panel4);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 3);
            panel2.Margin = new Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new Size(311, 241);
            panel2.TabIndex = 12;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(label5);
            panel3.Controls.Add(button2);
            panel3.Controls.Add(button1);
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(311, 25);
            panel3.TabIndex = 11;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(4, 1);
            label5.MinimumSize = new Size(80, 23);
            label5.Name = "label5";
            label5.Size = new Size(80, 23);
            label5.TabIndex = 10;
            label5.Text = "关于";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(262, 1);
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
            button1.Location = new Point(286, 1);
            button1.Margin = new Padding(0);
            button1.Name = "button1";
            button1.Size = new Size(23, 23);
            button1.TabIndex = 7;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button_关闭_Click;
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.ButtonFace;
            panel4.Controls.Add(panel1);
            panel4.Controls.Add(panel_About);
            panel4.Location = new Point(0, 26);
            panel4.Margin = new Padding(0);
            panel4.Name = "panel4";
            panel4.Size = new Size(311, 213);
            panel4.TabIndex = 12;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(linkLabel3);
            panel1.Controls.Add(linkLabel2);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(5, 115);
            panel1.MinimumSize = new Size(301, 55);
            panel1.Name = "panel1";
            panel1.Size = new Size(301, 92);
            panel1.TabIndex = 10;
            panel1.Paint += panel1_Paint;
            // 
            // linkLabel3
            // 
            linkLabel3.AutoSize = true;
            linkLabel3.Location = new Point(5, 67);
            linkLabel3.MinimumSize = new Size(80, 17);
            linkLabel3.Name = "linkLabel3";
            linkLabel3.Size = new Size(80, 17);
            linkLabel3.TabIndex = 4;
            linkLabel3.TabStop = true;
            linkLabel3.Text = "baobao";
            linkLabel3.Click += linkLabel3_Click;
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(5, 45);
            linkLabel2.MinimumSize = new Size(80, 17);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(80, 17);
            linkLabel2.TabIndex = 3;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "XJYdemons";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // label2
            // 
            label2.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label2.Location = new Point(5, 30);
            label2.MinimumSize = new Size(294, 10);
            label2.Name = "label2";
            label2.Size = new Size(294, 10);
            label2.TabIndex = 1;
            label2.Text = "————————————————————————————————————";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label1.Location = new Point(5, 5);
            label1.MinimumSize = new Size(294, 25);
            label1.Name = "label1";
            label1.Size = new Size(294, 25);
            label1.TabIndex = 0;
            label1.Text = "开发者";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_About
            // 
            panel_About.BackColor = Color.White;
            panel_About.Controls.Add(label_64位);
            panel_About.Controls.Add(label_程序名);
            panel_About.Controls.Add(label_Github主页);
            panel_About.Controls.Add(label_项目地址);
            panel_About.Controls.Add(linkLabel1);
            panel_About.Controls.Add(label_版权所有);
            panel_About.Controls.Add(pictureBox1);
            panel_About.Controls.Add(label_版本号);
            panel_About.Location = new Point(5, 5);
            panel_About.MinimumSize = new Size(301, 105);
            panel_About.Name = "panel_About";
            panel_About.Size = new Size(301, 105);
            panel_About.TabIndex = 9;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(318, 248);
            Controls.Add(panel_BackGround);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(314, 225);
            Name = "AboutForm";
            Text = "关于";
            TopMost = true;
            Load += AboutForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel_BackGround.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel_About.ResumeLayout(false);
            panel_About.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Label label_程序名;
        private Label label_64位;
        private Label label_版本号;
        private Label label_版权所有;
        private LinkLabel linkLabel1;
        private Label label_Github主页;
        private Label label_项目地址;
        private Panel panel_BackGround;
        private Panel panel_About;
        private Panel panel1;
        private Label label1;
        private LinkLabel linkLabel2;
        private Label label2;
        private LinkLabel linkLabel3;
        private Panel panel3;
        private Label label5;
        private Button button2;
        private Button button1;
        private Panel panel2;
        private Panel panel4;
    }
}