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
            pictureBox_程序图标 = new PictureBox();
            label_程序名 = new Label();
            label_架构 = new Label();
            label_版本号 = new Label();
            label_版权所有 = new Label();
            linkLabel_作者主页 = new LinkLabel();
            label_Github主页 = new Label();
            label_项目地址 = new Label();
            panel_总背景 = new Panel();
            panel_副背景 = new Panel();
            panel_标题栏背景 = new Panel();
            label_标题 = new Label();
            button_最小化 = new Button();
            button_关闭 = new Button();
            panel_客户区背景 = new Panel();
            panel_开发者 = new Panel();
            linkLabel_作者2主页 = new LinkLabel();
            linkLabel_作者1主页 = new LinkLabel();
            label_分割线_开发者 = new Label();
            label_开发者 = new Label();
            panel_软件信息 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox_程序图标).BeginInit();
            panel_总背景.SuspendLayout();
            panel_副背景.SuspendLayout();
            panel_标题栏背景.SuspendLayout();
            panel_客户区背景.SuspendLayout();
            panel_开发者.SuspendLayout();
            panel_软件信息.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox_程序图标
            // 
            pictureBox_程序图标.BackColor = Color.White;
            pictureBox_程序图标.Image = (Image)resources.GetObject("pictureBox_程序图标.Image");
            pictureBox_程序图标.Location = new Point(5, 5);
            pictureBox_程序图标.Name = "pictureBox_程序图标";
            pictureBox_程序图标.Size = new Size(50, 50);
            pictureBox_程序图标.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox_程序图标.TabIndex = 0;
            pictureBox_程序图标.TabStop = false;
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
            // label_架构
            // 
            label_架构.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label_架构.Location = new Point(255, 27);
            label_架构.Name = "label_架构";
            label_架构.Size = new Size(44, 17);
            label_架构.TabIndex = 2;
            label_架构.Text = "(64位)";
            label_架构.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label_版本号
            // 
            label_版本号.AutoSize = true;
            label_版本号.Location = new Point(5, 60);
            label_版本号.MinimumSize = new Size(174, 17);
            label_版本号.Name = "label_版本号";
            label_版本号.Size = new Size(174, 17);
            label_版本号.TabIndex = 3;
            label_版本号.Text = "版本  v7.1.0(2026.04.15)";
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
            // linkLabel_作者主页
            // 
            linkLabel_作者主页.AutoSize = true;
            linkLabel_作者主页.Location = new Point(81, 82);
            linkLabel_作者主页.MinimumSize = new Size(64, 17);
            linkLabel_作者主页.Name = "linkLabel_作者主页";
            linkLabel_作者主页.Size = new Size(64, 17);
            linkLabel_作者主页.TabIndex = 5;
            linkLabel_作者主页.TabStop = true;
            linkLabel_作者主页.Text = "XJY工作室";
            linkLabel_作者主页.TextAlign = ContentAlignment.MiddleLeft;
            linkLabel_作者主页.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label_Github主页
            // 
            label_Github主页.Anchor = AnchorStyles.Right;
            label_Github主页.AutoSize = true;
            label_Github主页.ForeColor = Color.Black;
            label_Github主页.Location = new Point(224, 82);
            label_Github主页.MinimumSize = new Size(70, 17);
            label_Github主页.Name = "label_Github主页";
            label_Github主页.Size = new Size(70, 17);
            label_Github主页.TabIndex = 7;
            label_Github主页.Text = "Github主页";
            label_Github主页.TextAlign = ContentAlignment.MiddleRight;
            label_Github主页.Click += label5_Click;
            label_Github主页.MouseEnter += label5_MouseEnter;
            label_Github主页.MouseLeave += label5_MouseLeave;
            // 
            // label_项目地址
            // 
            label_项目地址.Anchor = AnchorStyles.Right;
            label_项目地址.AutoSize = true;
            label_项目地址.ForeColor = Color.Black;
            label_项目地址.Location = new Point(238, 60);
            label_项目地址.MinimumSize = new Size(56, 17);
            label_项目地址.Name = "label_项目地址";
            label_项目地址.Size = new Size(56, 17);
            label_项目地址.TabIndex = 8;
            label_项目地址.Text = "项目地址";
            label_项目地址.TextAlign = ContentAlignment.MiddleRight;
            label_项目地址.Click += label6_Click;
            label_项目地址.MouseEnter += label6_MouseEnter;
            label_项目地址.MouseLeave += label6_MouseLeave;
            // 
            // panel_总背景
            // 
            panel_总背景.BackColor = Color.FromArgb(250, 250, 250);
            panel_总背景.Controls.Add(panel_副背景);
            panel_总背景.Dock = DockStyle.Fill;
            panel_总背景.Location = new Point(0, 0);
            panel_总背景.Margin = new Padding(0);
            panel_总背景.MinimumSize = new Size(314, 225);
            panel_总背景.Name = "panel_总背景";
            panel_总背景.Padding = new Padding(3, 3, 4, 4);
            panel_总背景.Size = new Size(318, 248);
            panel_总背景.TabIndex = 9;
            // 
            // panel_副背景
            // 
            panel_副背景.BackColor = Color.White;
            panel_副背景.Controls.Add(panel_标题栏背景);
            panel_副背景.Controls.Add(panel_客户区背景);
            panel_副背景.Dock = DockStyle.Fill;
            panel_副背景.Location = new Point(3, 3);
            panel_副背景.Margin = new Padding(0);
            panel_副背景.Name = "panel_副背景";
            panel_副背景.Size = new Size(311, 241);
            panel_副背景.TabIndex = 12;
            // 
            // panel_标题栏背景
            // 
            panel_标题栏背景.BackColor = Color.White;
            panel_标题栏背景.Controls.Add(label_标题);
            panel_标题栏背景.Controls.Add(button_最小化);
            panel_标题栏背景.Controls.Add(button_关闭);
            panel_标题栏背景.Location = new Point(0, 0);
            panel_标题栏背景.Name = "panel_标题栏背景";
            panel_标题栏背景.Size = new Size(311, 25);
            panel_标题栏背景.TabIndex = 11;
            // 
            // label_标题
            // 
            label_标题.AutoSize = true;
            label_标题.Location = new Point(4, 1);
            label_标题.MinimumSize = new Size(80, 23);
            label_标题.Name = "label_标题";
            label_标题.Size = new Size(80, 23);
            label_标题.TabIndex = 10;
            label_标题.Text = "关于";
            label_标题.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button_最小化
            // 
            button_最小化.FlatAppearance.BorderSize = 0;
            button_最小化.FlatStyle = FlatStyle.Flat;
            button_最小化.Location = new Point(262, 1);
            button_最小化.Margin = new Padding(0);
            button_最小化.Name = "button_最小化";
            button_最小化.Size = new Size(23, 23);
            button_最小化.TabIndex = 8;
            button_最小化.Text = "—";
            button_最小化.UseVisualStyleBackColor = true;
            button_最小化.Click += button_最小化_Click;
            // 
            // button_关闭
            // 
            button_关闭.FlatAppearance.BorderSize = 0;
            button_关闭.FlatStyle = FlatStyle.Flat;
            button_关闭.Location = new Point(286, 1);
            button_关闭.Margin = new Padding(0);
            button_关闭.Name = "button_关闭";
            button_关闭.Size = new Size(23, 23);
            button_关闭.TabIndex = 7;
            button_关闭.Text = "X";
            button_关闭.UseVisualStyleBackColor = true;
            button_关闭.Click += button_关闭_Click;
            // 
            // panel_客户区背景
            // 
            panel_客户区背景.BackColor = SystemColors.ButtonFace;
            panel_客户区背景.Controls.Add(panel_开发者);
            panel_客户区背景.Controls.Add(panel_软件信息);
            panel_客户区背景.Location = new Point(0, 26);
            panel_客户区背景.Margin = new Padding(0);
            panel_客户区背景.Name = "panel_客户区背景";
            panel_客户区背景.Size = new Size(311, 213);
            panel_客户区背景.TabIndex = 12;
            // 
            // panel_开发者
            // 
            panel_开发者.BackColor = Color.White;
            panel_开发者.Controls.Add(linkLabel_作者2主页);
            panel_开发者.Controls.Add(linkLabel_作者1主页);
            panel_开发者.Controls.Add(label_分割线_开发者);
            panel_开发者.Controls.Add(label_开发者);
            panel_开发者.Location = new Point(5, 115);
            panel_开发者.MinimumSize = new Size(301, 55);
            panel_开发者.Name = "panel_开发者";
            panel_开发者.Size = new Size(301, 92);
            panel_开发者.TabIndex = 10;
            panel_开发者.Paint += panel1_Paint;
            // 
            // linkLabel_作者2主页
            // 
            linkLabel_作者2主页.AutoSize = true;
            linkLabel_作者2主页.Location = new Point(5, 67);
            linkLabel_作者2主页.MinimumSize = new Size(80, 17);
            linkLabel_作者2主页.Name = "linkLabel_作者2主页";
            linkLabel_作者2主页.Size = new Size(80, 17);
            linkLabel_作者2主页.TabIndex = 4;
            linkLabel_作者2主页.TabStop = true;
            linkLabel_作者2主页.Text = "baobao";
            linkLabel_作者2主页.Click += linkLabel3_Click;
            // 
            // linkLabel_作者1主页
            // 
            linkLabel_作者1主页.AutoSize = true;
            linkLabel_作者1主页.Location = new Point(5, 45);
            linkLabel_作者1主页.MinimumSize = new Size(80, 17);
            linkLabel_作者1主页.Name = "linkLabel_作者1主页";
            linkLabel_作者1主页.Size = new Size(80, 17);
            linkLabel_作者1主页.TabIndex = 3;
            linkLabel_作者1主页.TabStop = true;
            linkLabel_作者1主页.Text = "XJYdemons";
            linkLabel_作者1主页.LinkClicked += linkLabel2_LinkClicked;
            // 
            // label_分割线_开发者
            // 
            label_分割线_开发者.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_分割线_开发者.Location = new Point(5, 30);
            label_分割线_开发者.MinimumSize = new Size(294, 10);
            label_分割线_开发者.Name = "label_分割线_开发者";
            label_分割线_开发者.Size = new Size(294, 10);
            label_分割线_开发者.TabIndex = 1;
            label_分割线_开发者.Text = "————————————————————————————————————";
            label_分割线_开发者.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_开发者
            // 
            label_开发者.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label_开发者.Location = new Point(5, 5);
            label_开发者.MinimumSize = new Size(294, 25);
            label_开发者.Name = "label_开发者";
            label_开发者.Size = new Size(294, 25);
            label_开发者.TabIndex = 0;
            label_开发者.Text = "开发者";
            label_开发者.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_软件信息
            // 
            panel_软件信息.BackColor = Color.White;
            panel_软件信息.Controls.Add(label_架构);
            panel_软件信息.Controls.Add(label_程序名);
            panel_软件信息.Controls.Add(label_Github主页);
            panel_软件信息.Controls.Add(label_项目地址);
            panel_软件信息.Controls.Add(linkLabel_作者主页);
            panel_软件信息.Controls.Add(label_版权所有);
            panel_软件信息.Controls.Add(pictureBox_程序图标);
            panel_软件信息.Controls.Add(label_版本号);
            panel_软件信息.Location = new Point(5, 5);
            panel_软件信息.MinimumSize = new Size(301, 105);
            panel_软件信息.Name = "panel_软件信息";
            panel_软件信息.Size = new Size(301, 105);
            panel_软件信息.TabIndex = 9;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(318, 248);
            Controls.Add(panel_总背景);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(314, 225);
            Name = "AboutForm";
            Text = "关于";
            TopMost = true;
            Load += AboutForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox_程序图标).EndInit();
            panel_总背景.ResumeLayout(false);
            panel_副背景.ResumeLayout(false);
            panel_标题栏背景.ResumeLayout(false);
            panel_标题栏背景.PerformLayout();
            panel_客户区背景.ResumeLayout(false);
            panel_开发者.ResumeLayout(false);
            panel_开发者.PerformLayout();
            panel_软件信息.ResumeLayout(false);
            panel_软件信息.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox_程序图标;
        private Label label_程序名;
        private Label label_架构;
        private Label label_版本号;
        private Label label_版权所有;
        private LinkLabel linkLabel_作者主页;
        private Label label_Github主页;
        private Label label_项目地址;
        private Panel panel_总背景;
        private Panel panel_软件信息;
        private Panel panel_开发者;
        private Label label_开发者;
        private LinkLabel linkLabel_作者1主页;
        private Label label_分割线_开发者;
        private LinkLabel linkLabel_作者2主页;
        private Panel panel_标题栏背景;
        private Label label_标题;
        private Button button_最小化;
        private Button button_关闭;
        private Panel panel_副背景;
        private Panel panel_客户区背景;
    }
}