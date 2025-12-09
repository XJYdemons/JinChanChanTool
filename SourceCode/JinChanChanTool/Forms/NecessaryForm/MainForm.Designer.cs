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
            flowLayoutPanel__SubLineUp3 = new FlowLayoutPanel();
            flowLayoutPanel__SubLineUp2 = new FlowLayoutPanel();
            flowLayoutPanel_SubLineUp1 = new FlowLayoutPanel();
            tabControl_HeroSelector = new TabControl();
            button_ParseLineUp = new Button();
            textBox_LineUpCode = new TextBox();
            button_Clear = new Button();
            button_Save = new Button();
            label_自动拿牌 = new Label();
            button_GetCard = new Button();
            label_自动刷新 = new Label();
            button_Refresh = new Button();
            comboBox_LineUps = new ComboBox();
            comboBox_HeroPool = new ComboBox();
            menuStrip_Main = new MenuStrip();
            toolStripMenuItem_设置 = new ToolStripMenuItem();
            toolStripMenuItem_帮助 = new ToolStripMenuItem();
            toolStripMenuItem_运行日志 = new ToolStripMenuItem();
            toolStripMenuItem_关于 = new ToolStripMenuItem();
            panel_BackGround = new Panel();
            this.panel_按钮面板 = new Panel();
            button_生成阵容码 = new Button();
            this.label_赛季 = new Label();
            toolTipTimer = new System.Windows.Forms.Timer(components);
            timer_UpdateCoordinates = new System.Windows.Forms.Timer(components);
            panel_SubLineUpParent.SuspendLayout();
            menuStrip_Main.SuspendLayout();
            panel_BackGround.SuspendLayout();
            this.panel_按钮面板.SuspendLayout();
            SuspendLayout();
            // 
            // panel_SubLineUpParent
            // 
            panel_SubLineUpParent.AutoScroll = true;
            panel_SubLineUpParent.BackColor = Color.White;
            panel_SubLineUpParent.Controls.Add(flowLayoutPanel__SubLineUp3);
            panel_SubLineUpParent.Controls.Add(flowLayoutPanel__SubLineUp2);
            panel_SubLineUpParent.Controls.Add(flowLayoutPanel_SubLineUp1);
            panel_SubLineUpParent.Location = new Point(5, 432);
            panel_SubLineUpParent.Margin = new Padding(0);
            panel_SubLineUpParent.Name = "panel_SubLineUpParent";
            panel_SubLineUpParent.Padding = new Padding(3);
            panel_SubLineUpParent.Size = new Size(394, 152);
            panel_SubLineUpParent.TabIndex = 10;
            // 
            // flowLayoutPanel__SubLineUp3
            // 
            flowLayoutPanel__SubLineUp3.AutoSize = true;
            flowLayoutPanel__SubLineUp3.BackColor = Color.White;
            flowLayoutPanel__SubLineUp3.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel__SubLineUp3.Location = new Point(5, 91);
            flowLayoutPanel__SubLineUp3.Margin = new Padding(3, 3, 3, 7);
            flowLayoutPanel__SubLineUp3.Name = "flowLayoutPanel__SubLineUp3";
            flowLayoutPanel__SubLineUp3.Size = new Size(384, 38);
            flowLayoutPanel__SubLineUp3.TabIndex = 2;
            flowLayoutPanel__SubLineUp3.WrapContents = false;
            // 
            // flowLayoutPanel__SubLineUp2
            // 
            flowLayoutPanel__SubLineUp2.AutoSize = true;
            flowLayoutPanel__SubLineUp2.BackColor = Color.White;
            flowLayoutPanel__SubLineUp2.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel__SubLineUp2.Location = new Point(5, 48);
            flowLayoutPanel__SubLineUp2.Margin = new Padding(3, 3, 3, 7);
            flowLayoutPanel__SubLineUp2.Name = "flowLayoutPanel__SubLineUp2";
            flowLayoutPanel__SubLineUp2.Size = new Size(384, 38);
            flowLayoutPanel__SubLineUp2.TabIndex = 1;
            flowLayoutPanel__SubLineUp2.WrapContents = false;
            // 
            // flowLayoutPanel_SubLineUp1
            // 
            flowLayoutPanel_SubLineUp1.AutoSize = true;
            flowLayoutPanel_SubLineUp1.BackColor = Color.White;
            flowLayoutPanel_SubLineUp1.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel_SubLineUp1.Location = new Point(5, 5);
            flowLayoutPanel_SubLineUp1.Margin = new Padding(3, 3, 3, 7);
            flowLayoutPanel_SubLineUp1.Name = "flowLayoutPanel_SubLineUp1";
            flowLayoutPanel_SubLineUp1.Size = new Size(384, 38);
            flowLayoutPanel_SubLineUp1.TabIndex = 0;
            flowLayoutPanel_SubLineUp1.WrapContents = false;
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
            // comboBox_LineUps
            // 
            comboBox_LineUps.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBox_LineUps.FormattingEnabled = true;
            comboBox_LineUps.Items.AddRange(new object[] { "阵容1", "阵容2", "阵容3", "阵容4", "阵容5", "阵容6", "阵容7", "阵容8", "阵容9", "阵容10" });
            comboBox_LineUps.Location = new Point(242, 68);
            comboBox_LineUps.Margin = new Padding(2, 5, 2, 5);
            comboBox_LineUps.Name = "comboBox_LineUps";
            comboBox_LineUps.Size = new Size(149, 25);
            comboBox_LineUps.TabIndex = 1;
            comboBox_LineUps.Text = "阵容1";
            comboBox_LineUps.DropDownClosed += comboBox_LineUps_DropDownClosed;
            comboBox_LineUps.KeyDown += comboBox_LineUps_KeyDown;
            comboBox_LineUps.Leave += comboBox_LineUps_Leave;
            // 
            // comboBox_HeroPool
            // 
            comboBox_HeroPool.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_HeroPool.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBox_HeroPool.FormattingEnabled = true;
            comboBox_HeroPool.Items.AddRange(new object[] { "巨龙之牙", "符文之地" });
            comboBox_HeroPool.Location = new Point(239, 5);
            comboBox_HeroPool.Margin = new Padding(2, 5, 2, 5);
            comboBox_HeroPool.Name = "comboBox_HeroPool";
            comboBox_HeroPool.Size = new Size(149, 25);
            comboBox_HeroPool.TabIndex = 8;
            // 
            // menuStrip_Main
            // 
            menuStrip_Main.BackColor = Color.White;
            menuStrip_Main.ImageScalingSize = new Size(24, 24);
            menuStrip_Main.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_设置, toolStripMenuItem_帮助, toolStripMenuItem_关于 });
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
            panel_BackGround.Controls.Add(this.panel_按钮面板);
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
            this.panel_按钮面板.Controls.Add(button_生成阵容码);
            this.panel_按钮面板.Controls.Add(this.label_赛季);
            this.panel_按钮面板.Controls.Add(label_自动拿牌);
            this.panel_按钮面板.Controls.Add(comboBox_HeroPool);
            this.panel_按钮面板.Controls.Add(button_Clear);
            this.panel_按钮面板.Controls.Add(label_自动刷新);
            this.panel_按钮面板.Controls.Add(button_GetCard);
            this.panel_按钮面板.Controls.Add(button_Refresh);
            this.panel_按钮面板.Controls.Add(comboBox_LineUps);
            this.panel_按钮面板.Controls.Add(button_Save);
            this.panel_按钮面板.Controls.Add(button_ParseLineUp);
            this.panel_按钮面板.Controls.Add(textBox_LineUpCode);
            this.panel_按钮面板.Location = new Point(5, 30);
            this.panel_按钮面板.Name = "panel_按钮面板";
            this.panel_按钮面板.Size = new Size(394, 97);
            this.panel_按钮面板.TabIndex = 11;
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
            button_生成阵容码.Text = "生成阵容码";
            button_生成阵容码.UseVisualStyleBackColor = true;
            button_生成阵容码.Click += button_生成阵容码_Click;
            // 
            // label_赛季
            // 
            this.label_赛季.AutoSize = true;
            this.label_赛季.Location = new Point(195, 5);
            this.label_赛季.Margin = new Padding(2, 5, 0, 5);
            this.label_赛季.MinimumSize = new Size(35, 25);
            this.label_赛季.Name = "label_赛季";
            this.label_赛季.Size = new Size(35, 25);
            this.label_赛季.TabIndex = 9;
            this.label_赛季.Text = "赛季";
            this.label_赛季.TextAlign = ContentAlignment.MiddleCenter;
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
            ClientSize = new Size(407, 640);
            Controls.Add(panel_BackGround);
            FormBorderStyle = FormBorderStyle.None;
            MainMenuStrip = menuStrip_Main;
            Name = "MainForm";
            Text = " JinChanChanTool";
            TopMost = true;
            Load += Form1_Load;
            panel_SubLineUpParent.ResumeLayout(false);
            panel_SubLineUpParent.PerformLayout();
            menuStrip_Main.ResumeLayout(false);
            menuStrip_Main.PerformLayout();
            panel_BackGround.ResumeLayout(false);
            panel_BackGround.PerformLayout();
            this.panel_按钮面板.ResumeLayout(false);
            this.panel_按钮面板.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel_BackGround;
        private MenuStrip menuStrip_Main;
        private ToolStripMenuItem toolStripMenuItem_设置;
        private ToolStripMenuItem toolStripMenuItem_帮助;
        private ToolStripMenuItem toolStripMenuItem_关于;
        private ComboBox comboBox_LineUps;
        private ToolStripMenuItem toolStripMenuItem_运行日志;
        private ComboBox comboBox_HeroPool;
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
        private FlowLayoutPanel flowLayoutPanel__SubLineUp3;
        private FlowLayoutPanel flowLayoutPanel__SubLineUp2;
        private FlowLayoutPanel flowLayoutPanel_SubLineUp1;
        private System.Windows.Forms.Timer toolTipTimer;
        private Panel panel_按钮面板;
        private Label label_赛季;
        private System.Windows.Forms.Timer timer_UpdateCoordinates;
        private Button button_生成阵容码;
    }
}
