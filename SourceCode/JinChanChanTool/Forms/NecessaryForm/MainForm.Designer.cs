using JinChanChanTool.DIYComponents;

namespace JinChanChanTool
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            panel_SubLineUpParent = new Panel();
            button_变阵3 = new Button();
            button_变阵2 = new Button();
            button_变阵1 = new Button();
            flowLayoutPanel_SubLineUp = new CustomFlowLayoutPanel();
            tabControl_HeroSelector = new TabControl();
            textBox_LineUpCode = new TextBox();
            comboBox_SelectedLineUp = new ComboBox();
            comboBox_Season = new ComboBox();
            menuStrip_Main = new MenuStrip();
            toolStripMenuItem_设置 = new ToolStripMenuItem();
            toolStripMenuItem_帮助 = new ToolStripMenuItem();
            toolStripMenuItem_运行日志 = new ToolStripMenuItem();
            toolStripMenuItem_关于 = new ToolStripMenuItem();
            panel_BackGround = new Panel();
            roundedButton8 = new RoundedButton();
            roundedButton7 = new RoundedButton();
            roundedButton6 = new RoundedButton();
            roundedButton2 = new RoundedButton();
            roundedButton5 = new RoundedButton();
            roundedButton4 = new RoundedButton();
            roundedButton3 = new RoundedButton();
            roundedButton1 = new RoundedButton();
            label4 = new Label();
            label3 = new Label();
            label_赛季 = new Label();
            capsuleSwitch3 = new CapsuleSwitch();
            label2 = new Label();
            capsuleSwitch2 = new CapsuleSwitch();
            label1 = new Label();
            capsuleSwitch1 = new CapsuleSwitch();
            toolTipTimer = new System.Windows.Forms.Timer(components);
            timer_UpdateCoordinates = new System.Windows.Forms.Timer(components);
            panel_SubLineUpParent.SuspendLayout();
            menuStrip_Main.SuspendLayout();
            panel_BackGround.SuspendLayout();
            SuspendLayout();
            // 
            // panel_SubLineUpParent
            // 
            panel_SubLineUpParent.AutoScroll = true;
            panel_SubLineUpParent.BackColor = Color.White;
            panel_SubLineUpParent.Controls.Add(button_变阵3);
            panel_SubLineUpParent.Controls.Add(button_变阵2);
            panel_SubLineUpParent.Controls.Add(button_变阵1);
            panel_SubLineUpParent.Controls.Add(flowLayoutPanel_SubLineUp);
            panel_SubLineUpParent.Location = new Point(5, 446);
            panel_SubLineUpParent.Margin = new Padding(0);
            panel_SubLineUpParent.Name = "panel_SubLineUpParent";
            panel_SubLineUpParent.Padding = new Padding(3);
            panel_SubLineUpParent.Size = new Size(394, 177);
            panel_SubLineUpParent.TabIndex = 10;
            // 
            // button_变阵3
            // 
            button_变阵3.FlatStyle = FlatStyle.Flat;
            button_变阵3.Location = new Point(153, 2);
            button_变阵3.Name = "button_变阵3";
            button_变阵3.Size = new Size(75, 25);
            button_变阵3.TabIndex = 3;
            button_变阵3.Text = "后期";
            button_变阵3.UseVisualStyleBackColor = true;
            button_变阵3.Click += button_变阵3_Click;
            button_变阵3.MouseUp += button_变阵3_MouseUp;
            // 
            // button_变阵2
            // 
            button_变阵2.FlatStyle = FlatStyle.Flat;
            button_变阵2.Location = new Point(79, 2);
            button_变阵2.Name = "button_变阵2";
            button_变阵2.Size = new Size(75, 25);
            button_变阵2.TabIndex = 2;
            button_变阵2.Text = "中期";
            button_变阵2.UseVisualStyleBackColor = true;
            button_变阵2.Click += button_变阵2_Click;
            button_变阵2.MouseUp += button_变阵2_MouseUp;
            // 
            // button_变阵1
            // 
            button_变阵1.FlatStyle = FlatStyle.Flat;
            button_变阵1.Location = new Point(5, 2);
            button_变阵1.Name = "button_变阵1";
            button_变阵1.Size = new Size(75, 25);
            button_变阵1.TabIndex = 1;
            button_变阵1.Text = "前期";
            button_变阵1.UseVisualStyleBackColor = true;
            button_变阵1.Click += button_变阵1_Click;
            button_变阵1.MouseUp += button_变阵1_MouseUp;
            // 
            // flowLayoutPanel_SubLineUp
            // 
            flowLayoutPanel_SubLineUp.BackColor = Color.Transparent;
            flowLayoutPanel_SubLineUp.Location = new Point(5, 27);
            flowLayoutPanel_SubLineUp.Margin = new Padding(3, 3, 3, 7);
            flowLayoutPanel_SubLineUp.Name = "flowLayoutPanel_SubLineUp";
            flowLayoutPanel_SubLineUp.Size = new Size(384, 146);
            flowLayoutPanel_SubLineUp.TabIndex = 0;
            // 
            // tabControl_HeroSelector
            // 
            tabControl_HeroSelector.Location = new Point(5, 146);
            tabControl_HeroSelector.Margin = new Padding(5);
            tabControl_HeroSelector.Name = "tabControl_HeroSelector";
            tabControl_HeroSelector.SelectedIndex = 0;
            tabControl_HeroSelector.Size = new Size(394, 295);
            tabControl_HeroSelector.TabIndex = 8;
            // 
            // textBox_LineUpCode
            // 
            textBox_LineUpCode.Font = new Font("Microsoft YaHei UI", 9F);
            textBox_LineUpCode.Location = new Point(11, 116);
            textBox_LineUpCode.Margin = new Padding(5);
            textBox_LineUpCode.Multiline = true;
            textBox_LineUpCode.Name = "textBox_LineUpCode";
            textBox_LineUpCode.Size = new Size(214, 25);
            textBox_LineUpCode.TabIndex = 0;
            textBox_LineUpCode.Text = "请在此处粘贴阵容代码";
            textBox_LineUpCode.Enter += textBox_LineUpCode_Enter;
            textBox_LineUpCode.Leave += textBox_LineUpCode_Leave;
            // 
            // comboBox_SelectedLineUp
            // 
            comboBox_SelectedLineUp.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBox_SelectedLineUp.FormattingEnabled = true;
            comboBox_SelectedLineUp.Items.AddRange(new object[] { "阵容1", "阵容2", "阵容3", "阵容4", "阵容5", "阵容6", "阵容7", "阵容8", "阵容9", "阵容10" });
            comboBox_SelectedLineUp.Location = new Point(76, 86);
            comboBox_SelectedLineUp.Margin = new Padding(2, 5, 2, 5);
            comboBox_SelectedLineUp.Name = "comboBox_SelectedLineUp";
            comboBox_SelectedLineUp.Size = new Size(149, 25);
            comboBox_SelectedLineUp.TabIndex = 1;
            comboBox_SelectedLineUp.Text = "阵容1";
            comboBox_SelectedLineUp.DropDownClosed += comboBox_LineUps_DropDownClosed;
            comboBox_SelectedLineUp.KeyDown += comboBox_LineUps_KeyDown;
            comboBox_SelectedLineUp.Leave += comboBox_LineUps_Leave;
            // 
            // comboBox_Season
            // 
            comboBox_Season.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Season.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBox_Season.FormattingEnabled = true;
            comboBox_Season.Items.AddRange(new object[] { "巨龙之牙", "符文之地" });
            comboBox_Season.Location = new Point(76, 56);
            comboBox_Season.Margin = new Padding(2, 5, 2, 5);
            comboBox_Season.Name = "comboBox_Season";
            comboBox_Season.Size = new Size(149, 25);
            comboBox_Season.TabIndex = 8;
            // 
            // menuStrip_Main
            // 
            menuStrip_Main.BackColor = Color.White;
            menuStrip_Main.ImageScalingSize = new Size(24, 24);
            menuStrip_Main.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_设置, toolStripMenuItem_帮助, toolStripMenuItem_关于 });
            menuStrip_Main.Location = new Point(3, 2);
            menuStrip_Main.Name = "menuStrip_Main";
            menuStrip_Main.Size = new Size(398, 25);
            menuStrip_Main.TabIndex = 5;
            menuStrip_Main.Text = "菜单栏1";
            // 
            // toolStripMenuItem_设置
            // 
            toolStripMenuItem_设置.Name = "toolStripMenuItem_设置";
            toolStripMenuItem_设置.Size = new Size(44, 21);
            toolStripMenuItem_设置.Text = "设置";
            toolStripMenuItem_设置.Click += 设置ToolStripMenuItem_Click;
            // 
            // toolStripMenuItem_帮助
            // 
            toolStripMenuItem_帮助.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem_运行日志 });
            toolStripMenuItem_帮助.Name = "toolStripMenuItem_帮助";
            toolStripMenuItem_帮助.Size = new Size(44, 21);
            toolStripMenuItem_帮助.Text = "帮助";
            // 
            // toolStripMenuItem_运行日志
            // 
            toolStripMenuItem_运行日志.Name = "toolStripMenuItem_运行日志";
            toolStripMenuItem_运行日志.Size = new Size(124, 22);
            toolStripMenuItem_运行日志.Text = "运行日志";
            toolStripMenuItem_运行日志.Click += 运行日志ToolStripMenuItem_Click;
            // 
            // toolStripMenuItem_关于
            // 
            toolStripMenuItem_关于.Name = "toolStripMenuItem_关于";
            toolStripMenuItem_关于.Size = new Size(44, 21);
            toolStripMenuItem_关于.Text = "关于";
            toolStripMenuItem_关于.Click += 关于ToolStripMenuItem_Click;
            // 
            // panel_BackGround
            // 
            panel_BackGround.AutoSize = true;
            panel_BackGround.BackColor = Color.White;
            panel_BackGround.Controls.Add(roundedButton8);
            panel_BackGround.Controls.Add(roundedButton7);
            panel_BackGround.Controls.Add(roundedButton6);
            panel_BackGround.Controls.Add(roundedButton2);
            panel_BackGround.Controls.Add(roundedButton5);
            panel_BackGround.Controls.Add(roundedButton4);
            panel_BackGround.Controls.Add(roundedButton3);
            panel_BackGround.Controls.Add(roundedButton1);
            panel_BackGround.Controls.Add(label4);
            panel_BackGround.Controls.Add(label3);
            panel_BackGround.Controls.Add(label_赛季);
            panel_BackGround.Controls.Add(capsuleSwitch3);
            panel_BackGround.Controls.Add(label2);
            panel_BackGround.Controls.Add(comboBox_Season);
            panel_BackGround.Controls.Add(comboBox_SelectedLineUp);
            panel_BackGround.Controls.Add(capsuleSwitch2);
            panel_BackGround.Controls.Add(textBox_LineUpCode);
            panel_BackGround.Controls.Add(label1);
            panel_BackGround.Controls.Add(capsuleSwitch1);
            panel_BackGround.Controls.Add(panel_SubLineUpParent);
            panel_BackGround.Controls.Add(tabControl_HeroSelector);
            panel_BackGround.Controls.Add(menuStrip_Main);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(0, 0);
            panel_BackGround.Margin = new Padding(0);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Padding = new Padding(3, 2, 3, 5);
            panel_BackGround.Size = new Size(404, 629);
            panel_BackGround.TabIndex = 5;
            // 
            // roundedButton8
            // 
            roundedButton8.BorderColor = Color.Transparent;
            roundedButton8.ButtonColor = Color.FromArgb(0, 153, 255);
            roundedButton8.CornerRadius = 3;
            roundedButton8.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton8.HoverColor = Color.FromArgb(0, 141, 235);
            roundedButton8.Location = new Point(316, 56);
            roundedButton8.Name = "roundedButton8";
            roundedButton8.PressedColor = Color.FromArgb(0, 128, 214);
            roundedButton8.Size = new Size(83, 25);
            roundedButton8.TabIndex = 29;
            roundedButton8.Text = "编辑赛季装备";
            roundedButton8.TextColor = Color.White;
            roundedButton8.TextFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // roundedButton7
            // 
            roundedButton7.BorderColor = Color.Transparent;
            roundedButton7.ButtonColor = Color.FromArgb(0, 153, 255);
            roundedButton7.CornerRadius = 3;
            roundedButton7.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton7.HoverColor = Color.FromArgb(0, 141, 235);
            roundedButton7.Location = new Point(230, 56);
            roundedButton7.Name = "roundedButton7";
            roundedButton7.PressedColor = Color.FromArgb(0, 128, 214);
            roundedButton7.Size = new Size(83, 25);
            roundedButton7.TabIndex = 28;
            roundedButton7.Text = "编辑赛季英雄";
            roundedButton7.TextColor = Color.White;
            roundedButton7.TextFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // roundedButton6
            // 
            roundedButton6.BorderColor = Color.Transparent;
            roundedButton6.ButtonColor = Color.FromArgb(0, 153, 255);
            roundedButton6.CornerRadius = 3;
            roundedButton6.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton6.HoverColor = Color.FromArgb(0, 141, 235);
            roundedButton6.Location = new Point(359, 86);
            roundedButton6.Name = "roundedButton6";
            roundedButton6.PressedColor = Color.FromArgb(0, 128, 214);
            roundedButton6.Size = new Size(40, 25);
            roundedButton6.TabIndex = 27;
            roundedButton6.Text = "删除";
            roundedButton6.TextColor = Color.White;
            roundedButton6.TextFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            roundedButton6.Click += roundedButton6_Click;
            // 
            // roundedButton2
            // 
            roundedButton2.BorderColor = Color.Transparent;
            roundedButton2.ButtonColor = Color.FromArgb(0, 153, 255);
            roundedButton2.CornerRadius = 3;
            roundedButton2.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton2.HoverColor = Color.FromArgb(0, 141, 235);
            roundedButton2.Location = new Point(316, 86);
            roundedButton2.Name = "roundedButton2";
            roundedButton2.PressedColor = Color.FromArgb(0, 128, 214);
            roundedButton2.Size = new Size(40, 25);
            roundedButton2.TabIndex = 26;
            roundedButton2.Text = "添加";
            roundedButton2.TextColor = Color.White;
            roundedButton2.TextFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            roundedButton2.Click += roundedButton2_Click;
            // 
            // roundedButton5
            // 
            roundedButton5.BorderColor = Color.Transparent;
            roundedButton5.ButtonColor = Color.FromArgb(0, 153, 255);
            roundedButton5.CornerRadius = 3;
            roundedButton5.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton5.HoverColor = Color.FromArgb(0, 141, 235);
            roundedButton5.Location = new Point(273, 86);
            roundedButton5.Name = "roundedButton5";
            roundedButton5.PressedColor = Color.FromArgb(0, 128, 214);
            roundedButton5.Size = new Size(40, 25);
            roundedButton5.TabIndex = 25;
            roundedButton5.Text = "清空";
            roundedButton5.TextColor = Color.White;
            roundedButton5.TextFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            roundedButton5.Click += roundedButton5_Click;
            // 
            // roundedButton4
            // 
            roundedButton4.BorderColor = Color.Transparent;
            roundedButton4.ButtonColor = Color.FromArgb(0, 153, 255);
            roundedButton4.CornerRadius = 3;
            roundedButton4.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton4.HoverColor = Color.FromArgb(0, 141, 235);
            roundedButton4.Location = new Point(230, 86);
            roundedButton4.Name = "roundedButton4";
            roundedButton4.PressedColor = Color.FromArgb(0, 128, 214);
            roundedButton4.Size = new Size(40, 25);
            roundedButton4.TabIndex = 24;
            roundedButton4.Text = "保存";
            roundedButton4.TextColor = Color.White;
            roundedButton4.TextFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            roundedButton4.Click += roundedButton4_Click;
            // 
            // roundedButton3
            // 
            roundedButton3.BorderColor = Color.Transparent;
            roundedButton3.ButtonColor = Color.FromArgb(0, 153, 255);
            roundedButton3.CornerRadius = 3;
            roundedButton3.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton3.HoverColor = Color.FromArgb(0, 141, 235);
            roundedButton3.Location = new Point(230, 116);
            roundedButton3.Name = "roundedButton3";
            roundedButton3.PressedColor = Color.FromArgb(0, 128, 214);
            roundedButton3.Size = new Size(83, 25);
            roundedButton3.TabIndex = 23;
            roundedButton3.Text = "解析阵容码";
            roundedButton3.TextColor = Color.White;
            roundedButton3.TextFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            roundedButton3.Click += roundedButton3_Click;
            // 
            // roundedButton1
            // 
            roundedButton1.BorderColor = Color.Transparent;
            roundedButton1.ButtonColor = Color.FromArgb(0, 153, 255);
            roundedButton1.CornerRadius = 3;
            roundedButton1.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton1.HoverColor = Color.FromArgb(0, 141, 235);
            roundedButton1.Location = new Point(316, 116);
            roundedButton1.Name = "roundedButton1";
            roundedButton1.PressedColor = Color.FromArgb(0, 128, 214);
            roundedButton1.Size = new Size(83, 25);
            roundedButton1.TabIndex = 21;
            roundedButton1.Text = "导出阵容码";
            roundedButton1.TextColor = Color.White;
            roundedButton1.TextFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            roundedButton1.Click += roundedButton1_Click;
            // 
            // label4
            // 
            label4.Location = new Point(11, 86);
            label4.Margin = new Padding(2, 5, 0, 5);
            label4.Name = "label4";
            label4.Size = new Size(58, 25);
            label4.TabIndex = 20;
            label4.Text = "阵容选择";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.Location = new Point(259, 31);
            label3.Margin = new Padding(2, 5, 0, 5);
            label3.Name = "label3";
            label3.Size = new Size(84, 20);
            label3.TabIndex = 16;
            label3.Text = "自动刷新商店";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_赛季
            // 
            label_赛季.Location = new Point(11, 56);
            label_赛季.Margin = new Padding(2, 5, 0, 5);
            label_赛季.Name = "label_赛季";
            label_赛季.Size = new Size(58, 25);
            label_赛季.TabIndex = 9;
            label_赛季.Text = "赛季选择";
            label_赛季.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch3
            // 
            capsuleSwitch3.Location = new Point(344, 31);
            capsuleSwitch3.Name = "capsuleSwitch3";
            capsuleSwitch3.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch3.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch3.ShowText = false;
            capsuleSwitch3.Size = new Size(55, 20);
            capsuleSwitch3.TabIndex = 15;
            capsuleSwitch3.Text = "capsuleSwitch3";
            capsuleSwitch3.TextColor = Color.DimGray;
            capsuleSwitch3.ThumbColor = Color.White;
            capsuleSwitch3.IsOnChanged += capsuleSwitch3_IsOnChanged;
            // 
            // label2
            // 
            label2.Location = new Point(133, 31);
            label2.Margin = new Padding(2, 5, 0, 5);
            label2.Name = "label2";
            label2.Size = new Size(62, 20);
            label2.TabIndex = 14;
            label2.Text = "自动拿牌";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch2
            // 
            capsuleSwitch2.Location = new Point(196, 31);
            capsuleSwitch2.Name = "capsuleSwitch2";
            capsuleSwitch2.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch2.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch2.ShowText = false;
            capsuleSwitch2.Size = new Size(55, 20);
            capsuleSwitch2.TabIndex = 13;
            capsuleSwitch2.Text = "capsuleSwitch2";
            capsuleSwitch2.TextColor = Color.DimGray;
            capsuleSwitch2.ThumbColor = Color.White;
            capsuleSwitch2.IsOnChanged += capsuleSwitch2_IsOnChanged;
            // 
            // label1
            // 
            label1.Location = new Point(5, 31);
            label1.Margin = new Padding(2, 5, 0, 5);
            label1.Name = "label1";
            label1.Size = new Size(62, 20);
            label1.TabIndex = 12;
            label1.Text = "高亮显示";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch1
            // 
            capsuleSwitch1.Location = new Point(68, 31);
            capsuleSwitch1.Name = "capsuleSwitch1";
            capsuleSwitch1.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch1.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch1.ShowText = false;
            capsuleSwitch1.Size = new Size(55, 20);
            capsuleSwitch1.TabIndex = 0;
            capsuleSwitch1.Text = "capsuleSwitch1";
            capsuleSwitch1.TextColor = Color.DimGray;
            capsuleSwitch1.ThumbColor = Color.White;
            capsuleSwitch1.IsOnChanged += capsuleSwitch1_IsOnChanged;
            // 
            // toolTipTimer
            // 
            toolTipTimer.Interval = 200;
            toolTipTimer.Tick += toolTipTimer_Tick;
            // 
            // timer_UpdateCoordinates
            // 
            timer_UpdateCoordinates.Enabled = true;
            timer_UpdateCoordinates.Interval = 1000;
            timer_UpdateCoordinates.Tick += timer_UpdateCoordinates_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(404, 629);
            Controls.Add(panel_BackGround);
            FormBorderStyle = FormBorderStyle.None;
            MainMenuStrip = menuStrip_Main;
            Name = "MainForm";
            Text = " JinChanChanTool";
            TopMost = true;
            Load += Form1_Load;
            panel_SubLineUpParent.ResumeLayout(false);
            menuStrip_Main.ResumeLayout(false);
            menuStrip_Main.PerformLayout();
            panel_BackGround.ResumeLayout(false);
            panel_BackGround.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel_BackGround;
        private MenuStrip menuStrip_Main;
        private ToolStripMenuItem toolStripMenuItem_设置;
        private ToolStripMenuItem toolStripMenuItem_帮助;
        private ToolStripMenuItem toolStripMenuItem_关于;
        private ComboBox comboBox_SelectedLineUp;
        private ToolStripMenuItem toolStripMenuItem_运行日志;
        private ComboBox comboBox_Season;
        private TabControl tabControl_HeroSelector;
        private TextBox textBox_LineUpCode;
        private Panel panel_SubLineUpParent;
        private CustomFlowLayoutPanel flowLayoutPanel_SubLineUp;
        private System.Windows.Forms.Timer toolTipTimer;
        private Label label_赛季;
        private System.Windows.Forms.Timer timer_UpdateCoordinates;
        private Button button_变阵1;
        private Button button_变阵3;
        private Button button_变阵2;
        private CapsuleSwitch capsuleSwitch1;
        private Label label1;
        private Label label3;
        private CapsuleSwitch capsuleSwitch3;
        private Label label2;
        private CapsuleSwitch capsuleSwitch2;
        private Label label4;
        private RoundedButton roundedButton1;
        private RoundedButton roundedButton3;
        private RoundedButton roundedButton4;
        private RoundedButton roundedButton5;
        private RoundedButton roundedButton6;
        private RoundedButton roundedButton2;
        private RoundedButton roundedButton8;
        private RoundedButton roundedButton7;
    }
}
