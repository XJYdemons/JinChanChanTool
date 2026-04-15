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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            panel_子阵容展示区背景 = new Panel();
            button_变阵3 = new Button();
            button_变阵2 = new Button();
            button_变阵1 = new Button();
            flowLayoutPanel_子阵容展示 = new CustomFlowLayoutPanel();
            tabControl_英雄选择容器 = new TabControl();
            textBox_阵容码 = new TextBox();
            comboBox_阵容选择 = new ComboBox();
            comboBox_赛季选择 = new ComboBox();
            menuStrip_主窗口菜单 = new MenuStrip();
            toolStripMenuItem_设置 = new ToolStripMenuItem();
            toolStripMenuItem_帮助 = new ToolStripMenuItem();
            toolStripMenuItem_运行日志 = new ToolStripMenuItem();
            ToolStripMenuItem_用户手册 = new ToolStripMenuItem();
            ToolStripMenuItem_配置向导 = new ToolStripMenuItem();
            toolStripMenuItem_关于 = new ToolStripMenuItem();
            panel_用户区背景 = new Panel();
            roundedButton_导入 = new RoundedButton();
            roundedButton_编辑赛季装备 = new RoundedButton();
            roundedButton_编辑赛季英雄 = new RoundedButton();
            roundedButton_删除 = new RoundedButton();
            roundedButton_添加 = new RoundedButton();
            roundedButton_清空 = new RoundedButton();
            roundedButton_保存 = new RoundedButton();
            roundedButton_解析阵容码 = new RoundedButton();
            roundedButton_导出 = new RoundedButton();
            label_阵容选择 = new Label();
            label_自动刷新商店 = new Label();
            label_赛季 = new Label();
            capsuleSwitch_自动刷新商店 = new CapsuleSwitch();
            label_自动拿牌 = new Label();
            capsuleSwitch_自动拿牌 = new CapsuleSwitch();
            label_高亮显示 = new Label();
            capsuleSwitch_高亮显示 = new CapsuleSwitch();
            timer_装备推荐 = new System.Windows.Forms.Timer(components);
            timer_更新坐标 = new System.Windows.Forms.Timer(components);
            panel_窗体总背景 = new Panel();
            panel_窗体副背景 = new Panel();
            panel_标题栏背景 = new Panel();
            label_标题 = new Label();
            pictureBox_图标 = new PictureBox();
            button_最小化 = new Button();
            button_关闭 = new Button();
            panel_子阵容展示区背景.SuspendLayout();
            menuStrip_主窗口菜单.SuspendLayout();
            panel_用户区背景.SuspendLayout();
            panel_窗体总背景.SuspendLayout();
            panel_窗体副背景.SuspendLayout();
            panel_标题栏背景.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox_图标).BeginInit();
            SuspendLayout();
            // 
            // panel_子阵容展示区背景
            // 
            panel_子阵容展示区背景.AutoScroll = true;
            panel_子阵容展示区背景.BackColor = Color.White;
            panel_子阵容展示区背景.Controls.Add(button_变阵3);
            panel_子阵容展示区背景.Controls.Add(button_变阵2);
            panel_子阵容展示区背景.Controls.Add(button_变阵1);
            panel_子阵容展示区背景.Controls.Add(flowLayoutPanel_子阵容展示);
            panel_子阵容展示区背景.Location = new Point(5, 446);
            panel_子阵容展示区背景.Margin = new Padding(0);
            panel_子阵容展示区背景.Name = "panel_子阵容展示区背景";
            panel_子阵容展示区背景.Padding = new Padding(3);
            panel_子阵容展示区背景.Size = new Size(394, 185);
            panel_子阵容展示区背景.TabIndex = 10;
            // 
            // button_变阵3
            // 
            button_变阵3.FlatAppearance.BorderColor = Color.White;
            button_变阵3.FlatAppearance.BorderSize = 0;
            button_变阵3.FlatStyle = FlatStyle.Flat;
            button_变阵3.Location = new Point(157, 2);
            button_变阵3.Name = "button_变阵3";
            button_变阵3.Size = new Size(75, 25);
            button_变阵3.TabIndex = 3;
            button_变阵3.TabStop = false;
            button_变阵3.Text = "后期";
            button_变阵3.UseVisualStyleBackColor = true;
            button_变阵3.Click += button_变阵3_Click;
           
            // 
            // button_变阵2
            // 
            button_变阵2.FlatAppearance.BorderColor = Color.White;
            button_变阵2.FlatAppearance.BorderSize = 0;
            button_变阵2.FlatStyle = FlatStyle.Flat;
            button_变阵2.Location = new Point(81, 2);
            button_变阵2.Name = "button_变阵2";
            button_变阵2.Size = new Size(75, 25);
            button_变阵2.TabIndex = 2;
            button_变阵2.TabStop = false;
            button_变阵2.Text = "中期";
            button_变阵2.UseVisualStyleBackColor = true;
            button_变阵2.Click += button_变阵2_Click;
          
            // 
            // button_变阵1
            // 
            button_变阵1.FlatAppearance.BorderColor = Color.White;
            button_变阵1.FlatAppearance.BorderSize = 0;
            button_变阵1.FlatStyle = FlatStyle.Flat;
            button_变阵1.Location = new Point(5, 2);
            button_变阵1.Name = "button_变阵1";
            button_变阵1.Size = new Size(75, 25);
            button_变阵1.TabIndex = 1;
            button_变阵1.TabStop = false;
            button_变阵1.Text = "前期";
            button_变阵1.UseVisualStyleBackColor = true;
            button_变阵1.Click += button_变阵1_Click;
           
            // 
            // flowLayoutPanel_子阵容展示
            // 
            flowLayoutPanel_子阵容展示.BackColor = Color.Transparent;
            flowLayoutPanel_子阵容展示.Location = new Point(5, 28);
            flowLayoutPanel_子阵容展示.Margin = new Padding(3, 3, 3, 7);
            flowLayoutPanel_子阵容展示.Name = "flowLayoutPanel_子阵容展示";
            flowLayoutPanel_子阵容展示.Size = new Size(384, 152);
            flowLayoutPanel_子阵容展示.TabIndex = 0;
            // 
            // tabControl_英雄选择容器
            // 
            tabControl_英雄选择容器.Location = new Point(5, 146);
            tabControl_英雄选择容器.Margin = new Padding(5);
            tabControl_英雄选择容器.Name = "tabControl_英雄选择容器";
            tabControl_英雄选择容器.SelectedIndex = 0;
            tabControl_英雄选择容器.Size = new Size(394, 295);
            tabControl_英雄选择容器.TabIndex = 8;
            // 
            // textBox_阵容码
            // 
            textBox_阵容码.Font = new Font("Microsoft YaHei UI", 9F);
            textBox_阵容码.Location = new Point(7, 116);
            textBox_阵容码.Margin = new Padding(5);
            textBox_阵容码.Multiline = true;
            textBox_阵容码.Name = "textBox_阵容码";
            textBox_阵容码.Size = new Size(214, 25);
            textBox_阵容码.TabIndex = 0;
            textBox_阵容码.Text = "请在此处粘贴阵容代码";
            textBox_阵容码.Enter += textBox_LineUpCode_Enter;
            textBox_阵容码.Leave += textBox_LineUpCode_Leave;
            // 
            // comboBox_阵容选择
            // 
            comboBox_阵容选择.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBox_阵容选择.FormattingEnabled = true;
            comboBox_阵容选择.Items.AddRange(new object[] { "阵容1", "阵容2", "阵容3", "阵容4", "阵容5", "阵容6", "阵容7", "阵容8", "阵容9", "阵容10" });
            comboBox_阵容选择.Location = new Point(72, 86);
            comboBox_阵容选择.Margin = new Padding(2, 5, 2, 5);
            comboBox_阵容选择.Name = "comboBox_阵容选择";
            comboBox_阵容选择.Size = new Size(149, 25);
            comboBox_阵容选择.TabIndex = 1;
            comboBox_阵容选择.Text = "阵容1";
            comboBox_阵容选择.DropDownClosed += comboBox_LineUps_DropDownClosed;
            comboBox_阵容选择.KeyDown += comboBox_LineUps_KeyDown;
            comboBox_阵容选择.Leave += comboBox_LineUps_Leave;
            // 
            // comboBox_赛季选择
            // 
            comboBox_赛季选择.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_赛季选择.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBox_赛季选择.FormattingEnabled = true;
            comboBox_赛季选择.Items.AddRange(new object[] { "巨龙之牙", "符文之地" });
            comboBox_赛季选择.Location = new Point(72, 56);
            comboBox_赛季选择.Margin = new Padding(2, 5, 2, 5);
            comboBox_赛季选择.Name = "comboBox_赛季选择";
            comboBox_赛季选择.Size = new Size(149, 25);
            comboBox_赛季选择.TabIndex = 8;
            // 
            // menuStrip_主窗口菜单
            // 
            menuStrip_主窗口菜单.BackColor = Color.White;
            menuStrip_主窗口菜单.ImageScalingSize = new Size(24, 24);
            menuStrip_主窗口菜单.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_设置, toolStripMenuItem_帮助, toolStripMenuItem_关于 });
            menuStrip_主窗口菜单.Location = new Point(0, 0);
            menuStrip_主窗口菜单.Name = "menuStrip_主窗口菜单";
            menuStrip_主窗口菜单.Padding = new Padding(0, 2, 0, 2);
            menuStrip_主窗口菜单.Size = new Size(404, 25);
            menuStrip_主窗口菜单.TabIndex = 5;
            menuStrip_主窗口菜单.Text = "菜单栏1";
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
            toolStripMenuItem_帮助.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem_运行日志, ToolStripMenuItem_用户手册, ToolStripMenuItem_配置向导 });
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
            // ToolStripMenuItem_用户手册
            // 
            ToolStripMenuItem_用户手册.Name = "ToolStripMenuItem_用户手册";
            ToolStripMenuItem_用户手册.Size = new Size(124, 22);
            ToolStripMenuItem_用户手册.Text = "用户手册";
            ToolStripMenuItem_用户手册.Click += 用户手册ToolStripMenuItem_Click;
            // 
            // ToolStripMenuItem_配置向导
            // 
            ToolStripMenuItem_配置向导.Name = "ToolStripMenuItem_配置向导";
            ToolStripMenuItem_配置向导.Size = new Size(124, 22);
            ToolStripMenuItem_配置向导.Text = "配置向导";
            ToolStripMenuItem_配置向导.Click += 配置向导ToolStripMenuItem_Click;
            // 
            // toolStripMenuItem_关于
            // 
            toolStripMenuItem_关于.Name = "toolStripMenuItem_关于";
            toolStripMenuItem_关于.Size = new Size(44, 21);
            toolStripMenuItem_关于.Text = "关于";
            toolStripMenuItem_关于.Click += 关于ToolStripMenuItem_Click;
            // 
            // panel_用户区背景
            // 
            panel_用户区背景.BackColor = Color.White;
            panel_用户区背景.Controls.Add(roundedButton_导入);
            panel_用户区背景.Controls.Add(roundedButton_编辑赛季装备);
            panel_用户区背景.Controls.Add(roundedButton_编辑赛季英雄);
            panel_用户区背景.Controls.Add(roundedButton_删除);
            panel_用户区背景.Controls.Add(roundedButton_添加);
            panel_用户区背景.Controls.Add(roundedButton_清空);
            panel_用户区背景.Controls.Add(roundedButton_保存);
            panel_用户区背景.Controls.Add(roundedButton_解析阵容码);
            panel_用户区背景.Controls.Add(roundedButton_导出);
            panel_用户区背景.Controls.Add(label_阵容选择);
            panel_用户区背景.Controls.Add(label_自动刷新商店);
            panel_用户区背景.Controls.Add(label_赛季);
            panel_用户区背景.Controls.Add(capsuleSwitch_自动刷新商店);
            panel_用户区背景.Controls.Add(label_自动拿牌);
            panel_用户区背景.Controls.Add(comboBox_赛季选择);
            panel_用户区背景.Controls.Add(comboBox_阵容选择);
            panel_用户区背景.Controls.Add(capsuleSwitch_自动拿牌);
            panel_用户区背景.Controls.Add(textBox_阵容码);
            panel_用户区背景.Controls.Add(label_高亮显示);
            panel_用户区背景.Controls.Add(capsuleSwitch_高亮显示);
            panel_用户区背景.Controls.Add(panel_子阵容展示区背景);
            panel_用户区背景.Controls.Add(tabControl_英雄选择容器);
            panel_用户区背景.Controls.Add(menuStrip_主窗口菜单);
            panel_用户区背景.Location = new Point(0, 25);
            panel_用户区背景.Margin = new Padding(0);
            panel_用户区背景.Name = "panel_用户区背景";
            panel_用户区背景.Size = new Size(404, 633);
            panel_用户区背景.TabIndex = 5;
            // 
            // roundedButton_导入
            // 
            roundedButton_导入.BorderColor = SystemColors.ScrollBar;
            roundedButton_导入.BorderWidth = 1;
            roundedButton_导入.ButtonColor = Color.White;
            roundedButton_导入.CornerRadius = 3;
            roundedButton_导入.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_导入.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_导入.Location = new Point(359, 116);
            roundedButton_导入.Name = "roundedButton_导入";
            roundedButton_导入.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_导入.Size = new Size(40, 25);
            roundedButton_导入.TabIndex = 30;
            roundedButton_导入.Text = "导入";
            roundedButton_导入.TextColor = Color.Black;
            roundedButton_导入.TextFont = new Font("微软雅黑", 8F);
            roundedButton_导入.Click += roundedButton9_Click;
            // 
            // roundedButton_编辑赛季装备
            // 
            roundedButton_编辑赛季装备.BorderColor = SystemColors.ScrollBar;
            roundedButton_编辑赛季装备.BorderWidth = 1;
            roundedButton_编辑赛季装备.ButtonColor = Color.White;
            roundedButton_编辑赛季装备.CornerRadius = 3;
            roundedButton_编辑赛季装备.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_编辑赛季装备.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_编辑赛季装备.Location = new Point(316, 56);
            roundedButton_编辑赛季装备.Name = "roundedButton_编辑赛季装备";
            roundedButton_编辑赛季装备.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_编辑赛季装备.Size = new Size(83, 25);
            roundedButton_编辑赛季装备.TabIndex = 29;
            roundedButton_编辑赛季装备.Text = "编辑赛季装备";
            roundedButton_编辑赛季装备.TextColor = Color.Black;
            roundedButton_编辑赛季装备.TextFont = new Font("微软雅黑", 8F);
            roundedButton_编辑赛季装备.Click += roundedButton8_Click;
            // 
            // roundedButton_编辑赛季英雄
            // 
            roundedButton_编辑赛季英雄.BorderColor = SystemColors.ScrollBar;
            roundedButton_编辑赛季英雄.BorderWidth = 1;
            roundedButton_编辑赛季英雄.ButtonColor = Color.White;
            roundedButton_编辑赛季英雄.CornerRadius = 3;
            roundedButton_编辑赛季英雄.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_编辑赛季英雄.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_编辑赛季英雄.Location = new Point(230, 56);
            roundedButton_编辑赛季英雄.Name = "roundedButton_编辑赛季英雄";
            roundedButton_编辑赛季英雄.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_编辑赛季英雄.Size = new Size(83, 25);
            roundedButton_编辑赛季英雄.TabIndex = 28;
            roundedButton_编辑赛季英雄.Text = "编辑赛季英雄";
            roundedButton_编辑赛季英雄.TextColor = Color.Black;
            roundedButton_编辑赛季英雄.TextFont = new Font("微软雅黑", 8F);
            roundedButton_编辑赛季英雄.Click += roundedButton7_Click;
            // 
            // roundedButton_删除
            // 
            roundedButton_删除.BorderColor = SystemColors.ScrollBar;
            roundedButton_删除.BorderWidth = 1;
            roundedButton_删除.ButtonColor = Color.White;
            roundedButton_删除.CornerRadius = 3;
            roundedButton_删除.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_删除.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            roundedButton_删除.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_删除.Location = new Point(359, 86);
            roundedButton_删除.Name = "roundedButton_删除";
            roundedButton_删除.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_删除.Size = new Size(40, 25);
            roundedButton_删除.TabIndex = 27;
            roundedButton_删除.Text = "删除";
            roundedButton_删除.TextColor = Color.Black;
            roundedButton_删除.TextFont = new Font("微软雅黑", 8F);
            roundedButton_删除.Click += roundedButton6_Click;
            // 
            // roundedButton_添加
            // 
            roundedButton_添加.BorderColor = SystemColors.ScrollBar;
            roundedButton_添加.BorderWidth = 1;
            roundedButton_添加.ButtonColor = Color.White;
            roundedButton_添加.CornerRadius = 3;
            roundedButton_添加.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_添加.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_添加.Location = new Point(316, 86);
            roundedButton_添加.Name = "roundedButton_添加";
            roundedButton_添加.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_添加.Size = new Size(40, 25);
            roundedButton_添加.TabIndex = 26;
            roundedButton_添加.Text = "添加";
            roundedButton_添加.TextColor = Color.Black;
            roundedButton_添加.TextFont = new Font("微软雅黑", 8F);
            roundedButton_添加.Click += roundedButton2_Click;
            // 
            // roundedButton_清空
            // 
            roundedButton_清空.BorderColor = SystemColors.ScrollBar;
            roundedButton_清空.BorderWidth = 1;
            roundedButton_清空.ButtonColor = Color.White;
            roundedButton_清空.CornerRadius = 3;
            roundedButton_清空.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_清空.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_清空.Location = new Point(273, 86);
            roundedButton_清空.Name = "roundedButton_清空";
            roundedButton_清空.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_清空.Size = new Size(40, 25);
            roundedButton_清空.TabIndex = 25;
            roundedButton_清空.Text = "清空";
            roundedButton_清空.TextColor = Color.Black;
            roundedButton_清空.TextFont = new Font("微软雅黑", 8F);
            roundedButton_清空.Click += roundedButton5_Click;
            // 
            // roundedButton_保存
            // 
            roundedButton_保存.BorderColor = SystemColors.ScrollBar;
            roundedButton_保存.BorderWidth = 1;
            roundedButton_保存.ButtonColor = Color.White;
            roundedButton_保存.CornerRadius = 3;
            roundedButton_保存.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_保存.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            roundedButton_保存.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_保存.Location = new Point(230, 86);
            roundedButton_保存.Name = "roundedButton_保存";
            roundedButton_保存.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_保存.Size = new Size(40, 25);
            roundedButton_保存.TabIndex = 24;
            roundedButton_保存.Text = "保存";
            roundedButton_保存.TextColor = Color.Black;
            roundedButton_保存.TextFont = new Font("微软雅黑", 8F);
            roundedButton_保存.Click += roundedButton4_Click;
            // 
            // roundedButton_解析阵容码
            // 
            roundedButton_解析阵容码.BorderColor = SystemColors.ScrollBar;
            roundedButton_解析阵容码.BorderWidth = 1;
            roundedButton_解析阵容码.ButtonColor = Color.White;
            roundedButton_解析阵容码.CornerRadius = 3;
            roundedButton_解析阵容码.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_解析阵容码.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_解析阵容码.Location = new Point(230, 116);
            roundedButton_解析阵容码.Name = "roundedButton_解析阵容码";
            roundedButton_解析阵容码.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_解析阵容码.Size = new Size(83, 25);
            roundedButton_解析阵容码.TabIndex = 23;
            roundedButton_解析阵容码.Text = "解析阵容码";
            roundedButton_解析阵容码.TextColor = Color.Black;
            roundedButton_解析阵容码.TextFont = new Font("微软雅黑", 8F);
            roundedButton_解析阵容码.Click += roundedButton3_Click;
            // 
            // roundedButton_导出
            // 
            roundedButton_导出.BorderColor = SystemColors.ScrollBar;
            roundedButton_导出.BorderWidth = 1;
            roundedButton_导出.ButtonColor = Color.White;
            roundedButton_导出.CornerRadius = 3;
            roundedButton_导出.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_导出.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_导出.Location = new Point(316, 116);
            roundedButton_导出.Name = "roundedButton_导出";
            roundedButton_导出.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_导出.Size = new Size(40, 25);
            roundedButton_导出.TabIndex = 21;
            roundedButton_导出.Text = "导出";
            roundedButton_导出.TextColor = Color.Black;
            roundedButton_导出.TextFont = new Font("微软雅黑", 8F);
            roundedButton_导出.Click += roundedButton1_Click;
            // 
            // label_阵容选择
            // 
            label_阵容选择.Location = new Point(7, 86);
            label_阵容选择.Margin = new Padding(2, 5, 0, 5);
            label_阵容选择.Name = "label_阵容选择";
            label_阵容选择.Size = new Size(58, 25);
            label_阵容选择.TabIndex = 20;
            label_阵容选择.Text = "阵容选择";
            label_阵容选择.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_自动刷新商店
            // 
            label_自动刷新商店.Location = new Point(264, 31);
            label_自动刷新商店.Margin = new Padding(2, 5, 0, 5);
            label_自动刷新商店.Name = "label_自动刷新商店";
            label_自动刷新商店.Size = new Size(84, 20);
            label_自动刷新商店.TabIndex = 16;
            label_自动刷新商店.Text = "自动刷新商店";
            label_自动刷新商店.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_赛季
            // 
            label_赛季.Location = new Point(7, 56);
            label_赛季.Margin = new Padding(2, 5, 0, 5);
            label_赛季.Name = "label_赛季";
            label_赛季.Size = new Size(58, 25);
            label_赛季.TabIndex = 9;
            label_赛季.Text = "赛季选择";
            label_赛季.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_自动刷新商店
            // 
            capsuleSwitch_自动刷新商店.Location = new Point(349, 31);
            capsuleSwitch_自动刷新商店.Name = "capsuleSwitch_自动刷新商店";
            capsuleSwitch_自动刷新商店.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_自动刷新商店.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_自动刷新商店.ShowText = false;
            capsuleSwitch_自动刷新商店.Size = new Size(50, 20);
            capsuleSwitch_自动刷新商店.TabIndex = 15;
            capsuleSwitch_自动刷新商店.Text = "capsuleSwitch3";
            capsuleSwitch_自动刷新商店.TextColor = Color.DimGray;
            capsuleSwitch_自动刷新商店.ThumbColor = Color.White;
            capsuleSwitch_自动刷新商店.IsOnChanged += capsuleSwitch3_IsOnChanged;
            // 
            // label_自动拿牌
            // 
            label_自动拿牌.Location = new Point(138, 31);
            label_自动拿牌.Margin = new Padding(2, 5, 0, 5);
            label_自动拿牌.Name = "label_自动拿牌";
            label_自动拿牌.Size = new Size(62, 20);
            label_自动拿牌.TabIndex = 14;
            label_自动拿牌.Text = "自动拿牌";
            label_自动拿牌.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_自动拿牌
            // 
            capsuleSwitch_自动拿牌.Location = new Point(201, 31);
            capsuleSwitch_自动拿牌.Name = "capsuleSwitch_自动拿牌";
            capsuleSwitch_自动拿牌.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_自动拿牌.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_自动拿牌.ShowText = false;
            capsuleSwitch_自动拿牌.Size = new Size(50, 20);
            capsuleSwitch_自动拿牌.TabIndex = 13;
            capsuleSwitch_自动拿牌.Text = "capsuleSwitch2";
            capsuleSwitch_自动拿牌.TextColor = Color.DimGray;
            capsuleSwitch_自动拿牌.ThumbColor = Color.White;
            capsuleSwitch_自动拿牌.IsOnChanged += capsuleSwitch2_IsOnChanged;
            // 
            // label_高亮显示
            // 
            label_高亮显示.Location = new Point(7, 31);
            label_高亮显示.Margin = new Padding(2, 5, 0, 5);
            label_高亮显示.Name = "label_高亮显示";
            label_高亮显示.Size = new Size(62, 20);
            label_高亮显示.TabIndex = 12;
            label_高亮显示.Text = "高亮提示";
            label_高亮显示.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_高亮显示
            // 
            capsuleSwitch_高亮显示.Location = new Point(70, 31);
            capsuleSwitch_高亮显示.Name = "capsuleSwitch_高亮显示";
            capsuleSwitch_高亮显示.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_高亮显示.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_高亮显示.ShowText = false;
            capsuleSwitch_高亮显示.Size = new Size(50, 20);
            capsuleSwitch_高亮显示.TabIndex = 0;
            capsuleSwitch_高亮显示.Text = "capsuleSwitch1";
            capsuleSwitch_高亮显示.TextColor = Color.DimGray;
            capsuleSwitch_高亮显示.ThumbColor = Color.White;
            capsuleSwitch_高亮显示.IsOnChanged += capsuleSwitch1_IsOnChanged;
            // 
            // timer_装备推荐
            // 
            timer_装备推荐.Interval = 200;
            timer_装备推荐.Tick += toolTipTimer_Tick;
            // 
            // timer_更新坐标
            // 
            timer_更新坐标.Enabled = true;
            timer_更新坐标.Interval = 1000;
            timer_更新坐标.Tick += timer_UpdateCoordinates_Tick;
            // 
            // panel_窗体总背景
            // 
            panel_窗体总背景.BackColor = Color.FromArgb(250, 250, 250);
            panel_窗体总背景.Controls.Add(panel_窗体副背景);
            panel_窗体总背景.Dock = DockStyle.Fill;
            panel_窗体总背景.Location = new Point(0, 0);
            panel_窗体总背景.Margin = new Padding(0);
            panel_窗体总背景.Name = "panel_窗体总背景";
            panel_窗体总背景.Padding = new Padding(3, 3, 4, 4);
            panel_窗体总背景.Size = new Size(410, 670);
            panel_窗体总背景.TabIndex = 6;
            // 
            // panel_窗体副背景
            // 
            panel_窗体副背景.BackColor = Color.White;
            panel_窗体副背景.Controls.Add(panel_标题栏背景);
            panel_窗体副背景.Controls.Add(panel_用户区背景);
            panel_窗体副背景.Dock = DockStyle.Fill;
            panel_窗体副背景.Location = new Point(3, 3);
            panel_窗体副背景.Margin = new Padding(0);
            panel_窗体副背景.Name = "panel_窗体副背景";
            panel_窗体副背景.Size = new Size(403, 663);
            panel_窗体副背景.TabIndex = 6;
            // 
            // panel_标题栏背景
            // 
            panel_标题栏背景.Controls.Add(label_标题);
            panel_标题栏背景.Controls.Add(pictureBox_图标);
            panel_标题栏背景.Controls.Add(button_最小化);
            panel_标题栏背景.Controls.Add(button_关闭);
            panel_标题栏背景.Location = new Point(0, 0);
            panel_标题栏背景.Name = "panel_标题栏背景";
            panel_标题栏背景.Size = new Size(404, 25);
            panel_标题栏背景.TabIndex = 6;
            // 
            // label_标题
            // 
            label_标题.AutoSize = true;
            label_标题.Location = new Point(24, 1);
            label_标题.MinimumSize = new Size(80, 23);
            label_标题.Name = "label_标题";
            label_标题.Size = new Size(107, 23);
            label_标题.TabIndex = 10;
            label_标题.Text = "JinChanChanTool";
            label_标题.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox_图标
            // 
            pictureBox_图标.Image = (Image)resources.GetObject("pictureBox_图标.Image");
            pictureBox_图标.Location = new Point(5, 5);
            pictureBox_图标.Margin = new Padding(0);
            pictureBox_图标.Name = "pictureBox_图标";
            pictureBox_图标.Size = new Size(15, 15);
            pictureBox_图标.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox_图标.TabIndex = 9;
            pictureBox_图标.TabStop = false;
            // 
            // button_最小化
            // 
            button_最小化.FlatAppearance.BorderSize = 0;
            button_最小化.FlatStyle = FlatStyle.Flat;
            button_最小化.Location = new Point(354, 1);
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
            button_关闭.Location = new Point(379, 1);
            button_关闭.Margin = new Padding(0);
            button_关闭.Name = "button_关闭";
            button_关闭.Size = new Size(23, 23);
            button_关闭.TabIndex = 7;
            button_关闭.Text = "X";
            button_关闭.UseVisualStyleBackColor = true;
            button_关闭.Click += button_关闭_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(410, 670);
            Controls.Add(panel_窗体总背景);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip_主窗口菜单;
            Name = "MainForm";
            Text = " JinChanChanTool";
            TopMost = true;
            Load += Form1_Load;
            panel_子阵容展示区背景.ResumeLayout(false);
            menuStrip_主窗口菜单.ResumeLayout(false);
            menuStrip_主窗口菜单.PerformLayout();
            panel_用户区背景.ResumeLayout(false);
            panel_用户区背景.PerformLayout();
            panel_窗体总背景.ResumeLayout(false);
            panel_窗体副背景.ResumeLayout(false);
            panel_标题栏背景.ResumeLayout(false);
            panel_标题栏背景.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox_图标).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_用户区背景;
        private MenuStrip menuStrip_主窗口菜单;
        private ToolStripMenuItem toolStripMenuItem_设置;
        private ToolStripMenuItem toolStripMenuItem_帮助;
        private ToolStripMenuItem toolStripMenuItem_关于;
        private ComboBox comboBox_阵容选择;
        private ToolStripMenuItem toolStripMenuItem_运行日志;
        private ComboBox comboBox_赛季选择;
        private TabControl tabControl_英雄选择容器;
        private TextBox textBox_阵容码;
        private Panel panel_子阵容展示区背景;
        private CustomFlowLayoutPanel flowLayoutPanel_子阵容展示;
        private System.Windows.Forms.Timer timer_装备推荐;
        private Label label_赛季;
        private System.Windows.Forms.Timer timer_更新坐标;
        private Button button_变阵1;
        private Button button_变阵3;
        private Button button_变阵2;
        private CapsuleSwitch capsuleSwitch_高亮显示;
        private Label label_高亮显示;
        private Label label_自动刷新商店;
        private CapsuleSwitch capsuleSwitch_自动刷新商店;
        private Label label_自动拿牌;
        private CapsuleSwitch capsuleSwitch_自动拿牌;
        private Label label_阵容选择;
        private RoundedButton roundedButton_编辑赛季装备;
        private RoundedButton roundedButton_编辑赛季英雄;
        private RoundedButton roundedButton_删除;
        private RoundedButton roundedButton_添加;
        private RoundedButton roundedButton_清空;
        private RoundedButton roundedButton_保存;
        private RoundedButton roundedButton_解析阵容码;
        private RoundedButton roundedButton_导出;
        private Panel panel_窗体总背景;
        private Panel panel_窗体副背景;
        private Panel panel_标题栏背景;
        private Button button_关闭;
        private Label label_标题;
        private PictureBox pictureBox_图标;
        private Button button_最小化;
        private RoundedButton roundedButton_导入;
        private ToolStripMenuItem ToolStripMenuItem_用户手册;
        private ToolStripMenuItem ToolStripMenuItem_配置向导;
    }
}
