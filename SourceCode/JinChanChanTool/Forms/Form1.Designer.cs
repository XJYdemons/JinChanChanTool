namespace JinChanChanTool
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            button1 = new Button();
            button2 = new Button();
            label66 = new Label();
            label65 = new Label();
            comboBox1 = new ComboBox();
            tabControl8 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            tabPage4 = new TabPage();
            tabPage5 = new TabPage();
            tabPage6 = new TabPage();
            panel11 = new Panel();
            label4 = new Label();
            panel10 = new Panel();
            label3 = new Label();
            button4 = new Button();
            button3 = new Button();
            menuStrip1 = new MenuStrip();
            设置ToolStripMenuItem = new ToolStripMenuItem();
            帮助ToolStripMenuItem = new ToolStripMenuItem();
            运行日志ToolStripMenuItem = new ToolStripMenuItem();
            发送反馈ToolStripMenuItem = new ToolStripMenuItem();
            关于ToolStripMenuItem = new ToolStripMenuItem();
            comboBox2 = new ComboBox();
            txtLineupCode = new TextBox();
            btnParseLineup = new Button();
            tabControl8.SuspendLayout();
            tabPage6.SuspendLayout();
            panel11.SuspendLayout();
            panel10.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.FlatAppearance.BorderColor = Color.DarkGray;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(11, 56);
            button1.Name = "button1";
            button1.Size = new Size(75, 26);
            button1.TabIndex = 0;
            button1.TabStop = false;
            button1.Text = "启动";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.FlatAppearance.BorderColor = Color.DarkGray;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(101, 56);
            button2.Name = "button2";
            button2.Size = new Size(75, 26);
            button2.TabIndex = 3;
            button2.TabStop = false;
            button2.Text = "启动";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label66
            // 
            label66.Location = new Point(97, 31);
            label66.Name = "label66";
            label66.Size = new Size(85, 23);
            label66.TabIndex = 2;
            label66.Text = "自动刷新商店";
            label66.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label65
            // 
            label65.Location = new Point(11, 31);
            label65.Name = "label65";
            label65.Size = new Size(75, 23);
            label65.TabIndex = 0;
            label65.Text = "自动拿牌";
            label65.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // comboBox1
            // 
            comboBox1.BackColor = Color.DimGray;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.ForeColor = Color.White;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            comboBox1.Location = new Point(252, 57);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(140, 25);
            comboBox1.TabIndex = 194;
            comboBox1.TabStop = false;
            comboBox1.DropDownClosed += comboBox1_DropDownClosed;
            comboBox1.KeyDown += comboBox1_KeyDown;
            comboBox1.Leave += comboBox1_Leave;
            // 
            // tabControl8
            // 
            tabControl8.Controls.Add(tabPage1);
            tabControl8.Controls.Add(tabPage2);
            tabControl8.Controls.Add(tabPage3);
            tabControl8.Controls.Add(tabPage4);
            tabControl8.Controls.Add(tabPage5);
            tabControl8.Controls.Add(tabPage6);
            tabControl8.Location = new Point(11, 84);
            tabControl8.Name = "tabControl8";
            tabControl8.SelectedIndex = 0;
            tabControl8.Size = new Size(385, 300);
            tabControl8.TabIndex = 10;
            tabControl8.TabStop = false;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(377, 270);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "1费卡";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.Transparent;
            tabPage2.ForeColor = SystemColors.ControlText;
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(377, 270);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "2费卡";
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(377, 270);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "3费卡";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            tabPage4.Location = new Point(4, 26);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(377, 270);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "4费卡";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            tabPage5.Location = new Point(4, 26);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(377, 270);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "5费卡";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(panel11);
            tabPage6.Controls.Add(panel10);
            tabPage6.Location = new Point(4, 26);
            tabPage6.Name = "tabPage6";
            tabPage6.Padding = new Padding(3);
            tabPage6.Size = new Size(377, 270);
            tabPage6.TabIndex = 5;
            tabPage6.Text = "按职业和特质选择";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // panel11
            // 
            panel11.AutoScroll = true;
            panel11.Controls.Add(label4);
            panel11.Dock = DockStyle.Right;
            panel11.Location = new Point(190, 3);
            panel11.Name = "panel11";
            panel11.Size = new Size(184, 264);
            panel11.TabIndex = 1;
            // 
            // label4
            // 
            label4.Location = new Point(8, 5);
            label4.Name = "label4";
            label4.Size = new Size(151, 21);
            label4.TabIndex = 1;
            label4.Text = "--------- 特质 ---------";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel10
            // 
            panel10.AutoScroll = true;
            panel10.Controls.Add(label3);
            panel10.Dock = DockStyle.Left;
            panel10.Location = new Point(3, 3);
            panel10.Name = "panel10";
            panel10.Size = new Size(184, 264);
            panel10.TabIndex = 0;
            // 
            // label3
            // 
            label3.Location = new Point(8, 5);
            label3.Name = "label3";
            label3.Size = new Size(151, 21);
            label3.TabIndex = 1;
            label3.Text = "--------- 职业 ---------";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button4
            // 
            button4.FlatAppearance.BorderColor = Color.DarkGray;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Location = new Point(327, 31);
            button4.Name = "button4";
            button4.Size = new Size(65, 25);
            button4.TabIndex = 193;
            button4.TabStop = false;
            button4.Text = "保存";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.FlatAppearance.BorderColor = Color.DarkGray;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(252, 31);
            button3.Name = "button3";
            button3.Size = new Size(65, 25);
            button3.TabIndex = 192;
            button3.TabStop = false;
            button3.Text = "清空";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 设置ToolStripMenuItem, 帮助ToolStripMenuItem, 关于ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(399, 25);
            menuStrip1.TabIndex = 220;
            menuStrip1.Text = "menuStrip1";
            // 
            // 设置ToolStripMenuItem
            // 
            设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            设置ToolStripMenuItem.Size = new Size(44, 21);
            设置ToolStripMenuItem.Text = "设置";
            设置ToolStripMenuItem.Click += 设置ToolStripMenuItem_Click;
            // 
            // 帮助ToolStripMenuItem
            // 
            帮助ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 运行日志ToolStripMenuItem, 发送反馈ToolStripMenuItem });
            帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            帮助ToolStripMenuItem.Size = new Size(44, 21);
            帮助ToolStripMenuItem.Text = "帮助";
            // 
            // 运行日志ToolStripMenuItem
            // 
            运行日志ToolStripMenuItem.Name = "运行日志ToolStripMenuItem";
            运行日志ToolStripMenuItem.Size = new Size(124, 22);
            运行日志ToolStripMenuItem.Text = "运行日志";
            运行日志ToolStripMenuItem.Click += 运行日志ToolStripMenuItem_Click;
            // 
            // 发送反馈ToolStripMenuItem
            // 
            发送反馈ToolStripMenuItem.Name = "发送反馈ToolStripMenuItem";
            发送反馈ToolStripMenuItem.Size = new Size(124, 22);
            发送反馈ToolStripMenuItem.Text = "发送反馈";
            发送反馈ToolStripMenuItem.Click += 发送反馈ToolStripMenuItem_Click;
            // 
            // 关于ToolStripMenuItem
            // 
            关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            关于ToolStripMenuItem.Size = new Size(44, 21);
            关于ToolStripMenuItem.Text = "关于";
            关于ToolStripMenuItem.Click += 关于ToolStripMenuItem_Click;
            // 
            // comboBox2
            // 
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(252, 2);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(140, 25);
            comboBox2.TabIndex = 221;
            comboBox2.TabStop = false;
            // 
            // txtLineupCode
            // 
            txtLineupCode.Location = new Point(11, 568);
            txtLineupCode.Multiline = true;
            txtLineupCode.Name = "txtLineupCode";
            txtLineupCode.Size = new Size(306, 24);
            txtLineupCode.TabIndex = 222;
            txtLineupCode.Text = "请在此处粘贴阵容代码";
            // 
            // btnParseLineup
            // 
            btnParseLineup.FlatStyle = FlatStyle.Flat;
            btnParseLineup.Location = new Point(321, 568);
            btnParseLineup.Name = "btnParseLineup";
            btnParseLineup.Size = new Size(75, 24);
            btnParseLineup.TabIndex = 223;
            btnParseLineup.Text = "解析阵容";
            btnParseLineup.UseVisualStyleBackColor = true;
            btnParseLineup.Click += btnParseLineup_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(399, 598);
            Controls.Add(btnParseLineup);
            Controls.Add(txtLineupCode);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(button1);
            Controls.Add(label66);
            Controls.Add(label65);
            Controls.Add(menuStrip1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button4);
            Controls.Add(tabControl8);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "Form1";
            Text = " JinChanChanTool";
            TopMost = true;
            Load += Form1_Load;
            tabControl8.ResumeLayout(false);
            tabPage6.ResumeLayout(false);
            panel11.ResumeLayout(false);
            panel10.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button4;
        private Button button3;
        private Label label65;
        private Label label66;
        private TabControl tabControl8;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private ComboBox comboBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 设置ToolStripMenuItem;
        private ToolStripMenuItem 帮助ToolStripMenuItem;
        private ToolStripMenuItem 关于ToolStripMenuItem;
        private ToolStripMenuItem 运行日志ToolStripMenuItem;
        private ToolStripMenuItem 发送反馈ToolStripMenuItem;
        private TabPage tabPage6;
        private Panel panel10;
        private Label label3;
        private Panel panel11;
        private Label label4;
        private ComboBox comboBox2;
        private TextBox txtLineupCode;
        private Button btnParseLineup;
    }
}
