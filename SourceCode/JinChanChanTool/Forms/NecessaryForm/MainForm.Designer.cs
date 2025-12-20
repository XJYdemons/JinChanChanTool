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
            button_ParseLineUp = new Button();
            textBox_LineUpCode = new TextBox();
            button_Clear = new Button();
            button_Save = new Button();
            label_自动拿牌 = new Label();
            button_GetCard = new Button();
            label_自动刷新 = new Label();
            button_Refresh = new Button();
            comboBox_SelectedLineUp = new ComboBox();
            comboBox_Season = new ComboBox();
            menuStrip_Main = new MenuStrip();
            toolStripMenuItem_设置 = new ToolStripMenuItem();
            toolStripMenuItem_帮助 = new ToolStripMenuItem();
            toolStripMenuItem_运行日志 = new ToolStripMenuItem();
            toolStripMenuItem_关于 = new ToolStripMenuItem();
            panel_BackGround = new Panel();
            panel_按钮面板 = new Panel();
            button_生成阵容码 = new Button();
            label_赛季 = new Label();
            toolTipTimer = new System.Windows.Forms.Timer(components);
            timer_UpdateCoordinates = new System.Windows.Forms.Timer(components);
            更新阵容推荐ToolStripMenuItem = new ToolStripMenuItem();
            panel_SubLineUpParent.SuspendLayout();
            menuStrip_Main.SuspendLayout();
            panel_BackGround.SuspendLayout();
            panel_按钮面板.SuspendLayout();
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
            panel_SubLineUpParent.Location = new Point(5, 442);
            panel_SubLineUpParent.Margin = new Padding(0);
            panel_SubLineUpParent.Name = "panel_SubLineUpParent";
            panel_SubLineUpParent.Padding = new Padding(3);
            panel_SubLineUpParent.Size = new Size(394, 189);
            panel_SubLineUpParent.TabIndex = 10;
            // 
            // button_变阵3
            // 
            button_变阵3.FlatStyle = FlatStyle.Flat;
            button_变阵3.Location = new Point(153, 11);
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
            button_变阵2.Location = new Point(79, 11);
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
            button_变阵1.Location = new Point(5, 11);
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
            flowLayoutPanel_SubLineUp.Location = new Point(5, 36);
            flowLayoutPanel_SubLineUp.Margin = new Padding(3, 3, 3, 7);
            flowLayoutPanel_SubLineUp.Name = "flowLayoutPanel_SubLineUp";
            flowLayoutPanel_SubLineUp.Size = new Size(384, 146);
            flowLayoutPanel_SubLineUp.TabIndex = 0;
            // 
            // tabControl_HeroSelector
            // 
            tabControl_HeroSelector.Location = new Point(5, 132);
            tabControl_HeroSelector.Margin = new Padding(5);
            tabControl_HeroSelector.Name = "tabControl_HeroSelector";
            tabControl_HeroSelector.SelectedIndex = 0;
            tabControl_HeroSelector.Size = new Size(394, 295);
            tabControl_HeroSelector.TabIndex = 8;
            // 
            // button_ParseLineUp
            // 
            button_ParseLineUp.FlatStyle = FlatStyle.Flat;
            button_ParseLineUp.Location = new Point(116, 67);
            button_ParseLineUp.Margin = new Padding(0);
            button_ParseLineUp.Name = "button_ParseLineUp";
            button_ParseLineUp.Size = new Size(50, 25);
            button_ParseLineUp.TabIndex = 1;
            button_ParseLineUp.Text = "解析";
            button_ParseLineUp.UseVisualStyleBackColor = true;
            button_ParseLineUp.Click += button_ParseLineUp_Click;
            // 
            // textBox_LineUpCode
            // 
            textBox_LineUpCode.Font = new Font("Microsoft YaHei UI", 7.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBox_LineUpCode.Location = new Point(5, 67);
            textBox_LineUpCode.Margin = new Padding(5);
            textBox_LineUpCode.Multiline = true;
            textBox_LineUpCode.Name = "textBox_LineUpCode";
            textBox_LineUpCode.Size = new Size(111, 25);
            textBox_LineUpCode.TabIndex = 0;
            textBox_LineUpCode.Text = "请在此处粘贴阵容代码";
            textBox_LineUpCode.Enter += textBox_LineUpCode_Enter;
            textBox_LineUpCode.Leave += textBox_LineUpCode_Leave;
            // 
            // button_Clear
            // 
            button_Clear.FlatStyle = FlatStyle.Flat;
            button_Clear.Location = new Point(316, 35);
            button_Clear.Margin = new Padding(5);
            button_Clear.Name = "button_Clear";
            button_Clear.Size = new Size(72, 27);
            button_Clear.TabIndex = 0;
            button_Clear.Text = "清空";
            button_Clear.UseVisualStyleBackColor = true;
            button_Clear.Click += button_Clear_Click;
            // 
            // button_Save
            // 
            button_Save.FlatStyle = FlatStyle.Flat;
            button_Save.Location = new Point(239, 35);
            button_Save.Margin = new Padding(5);
            button_Save.Name = "button_Save";
            button_Save.Size = new Size(72, 27);
            button_Save.TabIndex = 1;
            button_Save.Text = "保存";
            button_Save.UseVisualStyleBackColor = true;
            button_Save.Click += button_Save_Click;
            // 
            // label_自动拿牌
            // 
            label_自动拿牌.AutoSize = true;
            label_自动拿牌.Location = new Point(5, 5);
            label_自动拿牌.Margin = new Padding(2, 5, 0, 5);
            label_自动拿牌.MinimumSize = new Size(75, 25);
            label_自动拿牌.Name = "label_自动拿牌";
            label_自动拿牌.Size = new Size(75, 25);
            label_自动拿牌.TabIndex = 2;
            label_自动拿牌.Text = "自动拿牌";
            label_自动拿牌.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button_GetCard
            // 
            button_GetCard.FlatStyle = FlatStyle.Flat;
            button_GetCard.Location = new Point(5, 35);
            button_GetCard.Margin = new Padding(2, 5, 0, 5);
            button_GetCard.Name = "button_GetCard";
            button_GetCard.Size = new Size(75, 27);
            button_GetCard.TabIndex = 0;
            button_GetCard.Text = "启动";
            button_GetCard.UseVisualStyleBackColor = true;
            button_GetCard.Click += button_GetCard_Click;
            // 
            // label_自动刷新
            // 
            label_自动刷新.AutoSize = true;
            label_自动刷新.Location = new Point(91, 5);
            label_自动刷新.Margin = new Padding(2, 5, 0, 5);
            label_自动刷新.MinimumSize = new Size(75, 25);
            label_自动刷新.Name = "label_自动刷新";
            label_自动刷新.Size = new Size(75, 25);
            label_自动刷新.TabIndex = 3;
            label_自动刷新.Text = "自动刷新";
            label_自动刷新.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button_Refresh
            // 
            button_Refresh.FlatStyle = FlatStyle.Flat;
            button_Refresh.Location = new Point(91, 35);
            button_Refresh.Margin = new Padding(2, 5, 2, 5);
            button_Refresh.MinimumSize = new Size(75, 27);
            button_Refresh.Name = "button_Refresh";
            button_Refresh.Size = new Size(75, 27);
            button_Refresh.TabIndex = 1;
            button_Refresh.Text = "启动";
            button_Refresh.UseVisualStyleBackColor = true;
            button_Refresh.Click += button_Refresh_Click;
            // 
            // comboBox_SelectedLineUp
            // 
            comboBox_SelectedLineUp.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBox_SelectedLineUp.FormattingEnabled = true;
            comboBox_SelectedLineUp.Items.AddRange(new object[] { "阵容1", "阵容2", "阵容3", "阵容4", "阵容5", "阵容6", "阵容7", "阵容8", "阵容9", "阵容10" });
            comboBox_SelectedLineUp.Location = new Point(242, 68);
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
            comboBox_Season.Location = new Point(239, 5);
            comboBox_Season.Margin = new Padding(2, 5, 2, 5);
            comboBox_Season.Name = "comboBox_Season";
            comboBox_Season.Size = new Size(149, 25);
            comboBox_Season.TabIndex = 8;
            // 
            // menuStrip_Main
            // 
            menuStrip_Main.BackColor = Color.White;
            menuStrip_Main.ImageScalingSize = new Size(24, 24);
            menuStrip_Main.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_设置, toolStripMenuItem_帮助, toolStripMenuItem_关于, 更新阵容推荐ToolStripMenuItem });
            menuStrip_Main.Location = new Point(3, 2);
            menuStrip_Main.Name = "menuStrip_Main";
            menuStrip_Main.Size = new Size(401, 25);
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
            panel_BackGround.Controls.Add(panel_按钮面板);
            panel_BackGround.Controls.Add(panel_SubLineUpParent);
            panel_BackGround.Controls.Add(tabControl_HeroSelector);
            panel_BackGround.Controls.Add(menuStrip_Main);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(0, 0);
            panel_BackGround.Margin = new Padding(0);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Padding = new Padding(3, 2, 3, 5);
            panel_BackGround.Size = new Size(407, 640);
            panel_BackGround.TabIndex = 5;
            // 
            // panel_按钮面板
            // 
            panel_按钮面板.Controls.Add(button_生成阵容码);
            panel_按钮面板.Controls.Add(label_赛季);
            panel_按钮面板.Controls.Add(label_自动拿牌);
            panel_按钮面板.Controls.Add(comboBox_Season);
            panel_按钮面板.Controls.Add(button_Clear);
            panel_按钮面板.Controls.Add(label_自动刷新);
            panel_按钮面板.Controls.Add(button_GetCard);
            panel_按钮面板.Controls.Add(button_Refresh);
            panel_按钮面板.Controls.Add(comboBox_SelectedLineUp);
            panel_按钮面板.Controls.Add(button_Save);
            panel_按钮面板.Controls.Add(button_ParseLineUp);
            panel_按钮面板.Controls.Add(textBox_LineUpCode);
            panel_按钮面板.Location = new Point(5, 30);
            panel_按钮面板.Name = "panel_按钮面板";
            panel_按钮面板.Size = new Size(394, 97);
            panel_按钮面板.TabIndex = 11;
            // 
            // button_生成阵容码
            // 
            button_生成阵容码.FlatStyle = FlatStyle.Flat;
            button_生成阵容码.Font = new Font("Microsoft YaHei UI", 7.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button_生成阵容码.Location = new Point(168, 67);
            button_生成阵容码.Margin = new Padding(0);
            button_生成阵容码.Name = "button_生成阵容码";
            button_生成阵容码.Size = new Size(69, 25);
            button_生成阵容码.TabIndex = 10;
            button_生成阵容码.Text = "导出阵容码";
            button_生成阵容码.UseVisualStyleBackColor = true;
            button_生成阵容码.Click += button_生成阵容码_Click;
            // 
            // label_赛季
            // 
            label_赛季.AutoSize = true;
            label_赛季.Location = new Point(195, 5);
            label_赛季.Margin = new Padding(2, 5, 0, 5);
            label_赛季.MinimumSize = new Size(35, 25);
            label_赛季.Name = "label_赛季";
            label_赛季.Size = new Size(35, 25);
            label_赛季.TabIndex = 9;
            label_赛季.Text = "赛季";
            label_赛季.TextAlign = ContentAlignment.MiddleCenter;
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
            // 更新阵容推荐ToolStripMenuItem
            // 
            更新阵容推荐ToolStripMenuItem.Name = "更新阵容推荐ToolStripMenuItem";
            更新阵容推荐ToolStripMenuItem.Size = new Size(92, 21);
            更新阵容推荐ToolStripMenuItem.Text = "更新阵容推荐";
            更新阵容推荐ToolStripMenuItem.Click += 更新阵容推荐ToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(407, 640);
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
            panel_按钮面板.ResumeLayout(false);
            panel_按钮面板.PerformLayout();
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
        private Button button_Clear;
        private Button button_Save;
        private Button button_GetCard;
        private Button button_Refresh;
        private Label label_自动拿牌;
        private Label label_自动刷新;
        private TabControl tabControl_HeroSelector;
        private TextBox textBox_LineUpCode;
        private Button button_ParseLineUp;
        private Panel panel_SubLineUpParent;
        private CustomFlowLayoutPanel flowLayoutPanel_SubLineUp;
        private System.Windows.Forms.Timer toolTipTimer;
        private Panel panel_按钮面板;
        private Label label_赛季;
        private System.Windows.Forms.Timer timer_UpdateCoordinates;
        private Button button_生成阵容码;
        private Button button_变阵1;
        private Button button_变阵3;
        private Button button_变阵2;
        private ToolStripMenuItem 更新阵容推荐ToolStripMenuItem;
    }
}
