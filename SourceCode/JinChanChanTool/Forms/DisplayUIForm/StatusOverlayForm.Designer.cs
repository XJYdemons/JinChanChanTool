namespace JinChanChanTool.Forms
{
    partial class StatusOverlayForm
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
            panel_主背景 = new Panel();
            panel_副背景 = new Panel();
            label_隐藏召出主窗口 = new Label();
            label_HotKey3 = new Label();
            label_自动D牌 = new Label();
            label_HotKey4 = new Label();
            capsuleSwitch_自动刷新商店 = new JinChanChanTool.DIYComponents.CapsuleSwitch();
            label_自动刷新商店 = new Label();
            label_HotKey2 = new Label();
            capsuleSwitch_自动拿牌 = new JinChanChanTool.DIYComponents.CapsuleSwitch();
            label_自动拿牌 = new Label();
            label_HotKey1 = new Label();
            capsuleSwitch_高亮提示 = new JinChanChanTool.DIYComponents.CapsuleSwitch();
            label_高亮提示 = new Label();
            label_HotKey5 = new Label();
            panel_主背景.SuspendLayout();
            panel_副背景.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel_主背景.BackColor = Color.FromArgb(250, 250, 250);
            panel_主背景.Controls.Add(panel_副背景);
            panel_主背景.Dock = DockStyle.Fill;
            panel_主背景.Location = new Point(0, 0);
            panel_主背景.Margin = new Padding(0);
            panel_主背景.Name = "panel1";
            panel_主背景.Padding = new Padding(3, 3, 4, 4);
            panel_主背景.Size = new Size(256, 136);
            panel_主背景.TabIndex = 0;
            // 
            // panel2
            // 
            panel_副背景.BackColor = Color.White;
            panel_副背景.Controls.Add(label_隐藏召出主窗口);
            panel_副背景.Controls.Add(label_HotKey3);
            panel_副背景.Controls.Add(label_自动D牌);
            panel_副背景.Controls.Add(label_HotKey4);
            panel_副背景.Controls.Add(capsuleSwitch_自动刷新商店);
            panel_副背景.Controls.Add(label_自动刷新商店);
            panel_副背景.Controls.Add(label_HotKey2);
            panel_副背景.Controls.Add(capsuleSwitch_自动拿牌);
            panel_副背景.Controls.Add(label_自动拿牌);
            panel_副背景.Controls.Add(label_HotKey1);
            panel_副背景.Controls.Add(capsuleSwitch_高亮提示);
            panel_副背景.Controls.Add(label_高亮提示);
            panel_副背景.Controls.Add(label_HotKey5);
            panel_副背景.Dock = DockStyle.Fill;
            panel_副背景.Location = new Point(3, 3);
            panel_副背景.Margin = new Padding(0);
            panel_副背景.Name = "panel2";
            panel_副背景.Size = new Size(249, 129);
            panel_副背景.TabIndex = 0;
            // 
            // label9
            // 
            label_隐藏召出主窗口.AutoSize = true;
            label_隐藏召出主窗口.Location = new Point(83, 106);
            label_隐藏召出主窗口.MinimumSize = new Size(100, 20);
            label_隐藏召出主窗口.Name = "label9";
            label_隐藏召出主窗口.Size = new Size(100, 20);
            label_隐藏召出主窗口.TabIndex = 13;
            label_隐藏召出主窗口.Text = "隐藏/召出主窗口";
            label_隐藏召出主窗口.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_HotKey3
            // 
            label_HotKey3.AutoSize = true;
            label_HotKey3.ForeColor = Color.Gray;
            label_HotKey3.Location = new Point(9, 106);
            label_HotKey3.MinimumSize = new Size(69, 20);
            label_HotKey3.Name = "label_HotKey3";
            label_HotKey3.Size = new Size(69, 20);
            label_HotKey3.TabIndex = 12;
            label_HotKey3.Text = "Home";
            label_HotKey3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            label_自动D牌.AutoSize = true;
            label_自动D牌.Location = new Point(83, 81);
            label_自动D牌.MinimumSize = new Size(100, 20);
            label_自动D牌.Name = "label7";
            label_自动D牌.Size = new Size(100, 20);
            label_自动D牌.TabIndex = 10;
            label_自动D牌.Text = "自动D牌";
            label_自动D牌.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_HotKey4
            // 
            label_HotKey4.AutoSize = true;
            label_HotKey4.ForeColor = Color.Gray;
            label_HotKey4.Location = new Point(9, 81);
            label_HotKey4.MinimumSize = new Size(69, 20);
            label_HotKey4.Name = "label_HotKey4";
            label_HotKey4.Size = new Size(69, 20);
            label_HotKey4.TabIndex = 9;
            label_HotKey4.Text = "F9";
            label_HotKey4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_RefreshStore
            // 
            capsuleSwitch_自动刷新商店.Location = new Point(188, 56);
            capsuleSwitch_自动刷新商店.Name = "capsuleSwitch_RefreshStore";
            capsuleSwitch_自动刷新商店.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_自动刷新商店.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_自动刷新商店.ShowText = false;
            capsuleSwitch_自动刷新商店.Size = new Size(50, 20);
            capsuleSwitch_自动刷新商店.TabIndex = 8;
            capsuleSwitch_自动刷新商店.Text = "capsuleSwitch3";
            capsuleSwitch_自动刷新商店.TextColor = Color.White;
            capsuleSwitch_自动刷新商店.ThumbColor = Color.White;
            capsuleSwitch_自动刷新商店.IsOnChanged += AutoRefreshCapsuleSwitch_IsOnChanged;
            // 
            // label5
            // 
            label_自动刷新商店.AutoSize = true;
            label_自动刷新商店.Location = new Point(83, 56);
            label_自动刷新商店.MinimumSize = new Size(100, 20);
            label_自动刷新商店.Name = "label5";
            label_自动刷新商店.Size = new Size(100, 20);
            label_自动刷新商店.TabIndex = 7;
            label_自动刷新商店.Text = "自动刷新商店";
            label_自动刷新商店.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_HotKey2
            // 
            label_HotKey2.AutoSize = true;
            label_HotKey2.ForeColor = Color.Gray;
            label_HotKey2.Location = new Point(9, 56);
            label_HotKey2.MinimumSize = new Size(69, 20);
            label_HotKey2.Name = "label_HotKey2";
            label_HotKey2.Size = new Size(69, 20);
            label_HotKey2.TabIndex = 6;
            label_HotKey2.Text = "F8";
            label_HotKey2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_自动拿牌
            // 
            capsuleSwitch_自动拿牌.Location = new Point(188, 31);
            capsuleSwitch_自动拿牌.Name = "capsuleSwitch_自动拿牌";
            capsuleSwitch_自动拿牌.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_自动拿牌.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_自动拿牌.ShowText = false;
            capsuleSwitch_自动拿牌.Size = new Size(50, 20);
            capsuleSwitch_自动拿牌.TabIndex = 5;
            capsuleSwitch_自动拿牌.Text = "capsuleSwitch2";
            capsuleSwitch_自动拿牌.TextColor = Color.White;
            capsuleSwitch_自动拿牌.ThumbColor = Color.White;
            capsuleSwitch_自动拿牌.IsOnChanged += AutoGetCardCapsuleSwitch_IsOnChanged;
            // 
            // label3
            // 
            label_自动拿牌.AutoSize = true;
            label_自动拿牌.Location = new Point(83, 31);
            label_自动拿牌.MinimumSize = new Size(100, 20);
            label_自动拿牌.Name = "label3";
            label_自动拿牌.Size = new Size(100, 20);
            label_自动拿牌.TabIndex = 4;
            label_自动拿牌.Text = "自动拿牌";
            label_自动拿牌.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_HotKey1
            // 
            label_HotKey1.AutoSize = true;
            label_HotKey1.ForeColor = Color.Gray;
            label_HotKey1.Location = new Point(9, 31);
            label_HotKey1.MinimumSize = new Size(69, 20);
            label_HotKey1.Name = "label_HotKey1";
            label_HotKey1.Size = new Size(69, 20);
            label_HotKey1.TabIndex = 3;
            label_HotKey1.Text = "F7";
            label_HotKey1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_高亮提示
            // 
            capsuleSwitch_高亮提示.Location = new Point(188, 6);
            capsuleSwitch_高亮提示.Name = "capsuleSwitch_高亮提示";
            capsuleSwitch_高亮提示.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_高亮提示.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_高亮提示.ShowText = false;
            capsuleSwitch_高亮提示.Size = new Size(50, 20);
            capsuleSwitch_高亮提示.TabIndex = 2;
            capsuleSwitch_高亮提示.Text = "capsuleSwitch1";
            capsuleSwitch_高亮提示.TextColor = Color.White;
            capsuleSwitch_高亮提示.ThumbColor = Color.White;
            capsuleSwitch_高亮提示.IsOnChanged += HighlightCapsuleSwitch_IsOnChanged;
            // 
            // label2
            // 
            label_高亮提示.AutoSize = true;
            label_高亮提示.Location = new Point(83, 6);
            label_高亮提示.MinimumSize = new Size(100, 20);
            label_高亮提示.Name = "label2";
            label_高亮提示.Size = new Size(100, 20);
            label_高亮提示.TabIndex = 1;
            label_高亮提示.Text = "高亮提示";
            label_高亮提示.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_HotKey5
            // 
            label_HotKey5.AutoSize = true;
            label_HotKey5.ForeColor = Color.Gray;
            label_HotKey5.Location = new Point(9, 6);
            label_HotKey5.MinimumSize = new Size(69, 20);
            label_HotKey5.Name = "label_HotKey5";
            label_HotKey5.Size = new Size(69, 20);
            label_HotKey5.TabIndex = 0;
            label_HotKey5.Text = "F6";
            label_HotKey5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // StatusOverlayForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(256, 136);
            Controls.Add(panel_主背景);
            FormBorderStyle = FormBorderStyle.None;
            Name = "StatusOverlayForm";
            ShowInTaskbar = false;
            Text = "StatusOverlayForm";
            TopMost = true;
            panel_主背景.ResumeLayout(false);
            panel_副背景.ResumeLayout(false);
            panel_副背景.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_主背景;
        private Panel panel_副背景;
        private Label label_HotKey5;
        private DIYComponents.CapsuleSwitch capsuleSwitch_高亮提示;
        private Label label_高亮提示;
        private Label label_隐藏召出主窗口;
        private Label label_HotKey4;
        private Label label_自动D牌;
        private Label label_HotKey3;
        private DIYComponents.CapsuleSwitch capsuleSwitch_自动刷新商店;
        private Label label_自动刷新商店;
        private Label label_HotKey2;
        private DIYComponents.CapsuleSwitch capsuleSwitch_自动拿牌;
        private Label label_自动拿牌;
        private Label label_HotKey1;
    }
}