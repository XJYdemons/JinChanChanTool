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
            panel1 = new Panel();
            panel2 = new Panel();
            label9 = new Label();
            label_HotKey3 = new Label();
            label7 = new Label();
            label_HotKey4 = new Label();
            capsuleSwitch_RefreshStore = new JinChanChanTool.DIYComponents.CapsuleSwitch();
            label5 = new Label();
            label_HotKey2 = new Label();
            capsuleSwitch_GetCard = new JinChanChanTool.DIYComponents.CapsuleSwitch();
            label3 = new Label();
            label_HotKey1 = new Label();
            capsuleSwitch_HighLight = new JinChanChanTool.DIYComponents.CapsuleSwitch();
            label2 = new Label();
            label_HotKey5 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(250, 250, 250);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(3, 3, 4, 4);
            panel1.Size = new Size(256, 136);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.Controls.Add(label9);
            panel2.Controls.Add(label_HotKey3);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(label_HotKey4);
            panel2.Controls.Add(capsuleSwitch_RefreshStore);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label_HotKey2);
            panel2.Controls.Add(capsuleSwitch_GetCard);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label_HotKey1);
            panel2.Controls.Add(capsuleSwitch_HighLight);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label_HotKey5);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 3);
            panel2.Margin = new Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new Size(249, 129);
            panel2.TabIndex = 0;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(83, 106);
            label9.MinimumSize = new Size(100, 20);
            label9.Name = "label9";
            label9.Size = new Size(100, 20);
            label9.TabIndex = 13;
            label9.Text = "隐藏/召出主窗口";
            label9.TextAlign = ContentAlignment.MiddleLeft;
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
            label7.AutoSize = true;
            label7.Location = new Point(83, 81);
            label7.MinimumSize = new Size(100, 20);
            label7.Name = "label7";
            label7.Size = new Size(100, 20);
            label7.TabIndex = 10;
            label7.Text = "自动D牌";
            label7.TextAlign = ContentAlignment.MiddleLeft;
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
            capsuleSwitch_RefreshStore.Location = new Point(188, 56);
            capsuleSwitch_RefreshStore.Name = "capsuleSwitch_RefreshStore";
            capsuleSwitch_RefreshStore.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_RefreshStore.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_RefreshStore.ShowText = false;
            capsuleSwitch_RefreshStore.Size = new Size(50, 20);
            capsuleSwitch_RefreshStore.TabIndex = 8;
            capsuleSwitch_RefreshStore.Text = "capsuleSwitch3";
            capsuleSwitch_RefreshStore.TextColor = Color.White;
            capsuleSwitch_RefreshStore.ThumbColor = Color.White;
            capsuleSwitch_RefreshStore.IsOnChanged += AutoRefreshCapsuleSwitch_IsOnChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(83, 56);
            label5.MinimumSize = new Size(100, 20);
            label5.Name = "label5";
            label5.Size = new Size(100, 20);
            label5.TabIndex = 7;
            label5.Text = "自动刷新商店";
            label5.TextAlign = ContentAlignment.MiddleLeft;
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
            // capsuleSwitch_GetCard
            // 
            capsuleSwitch_GetCard.Location = new Point(188, 31);
            capsuleSwitch_GetCard.Name = "capsuleSwitch_GetCard";
            capsuleSwitch_GetCard.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_GetCard.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_GetCard.ShowText = false;
            capsuleSwitch_GetCard.Size = new Size(50, 20);
            capsuleSwitch_GetCard.TabIndex = 5;
            capsuleSwitch_GetCard.Text = "capsuleSwitch2";
            capsuleSwitch_GetCard.TextColor = Color.White;
            capsuleSwitch_GetCard.ThumbColor = Color.White;
            capsuleSwitch_GetCard.IsOnChanged += AutoGetCardCapsuleSwitch_IsOnChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(83, 31);
            label3.MinimumSize = new Size(100, 20);
            label3.Name = "label3";
            label3.Size = new Size(100, 20);
            label3.TabIndex = 4;
            label3.Text = "自动拿牌";
            label3.TextAlign = ContentAlignment.MiddleLeft;
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
            // capsuleSwitch_HighLight
            // 
            capsuleSwitch_HighLight.Location = new Point(188, 6);
            capsuleSwitch_HighLight.Name = "capsuleSwitch_HighLight";
            capsuleSwitch_HighLight.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_HighLight.OnColor = Color.FromArgb(76, 175, 80);
            capsuleSwitch_HighLight.ShowText = false;
            capsuleSwitch_HighLight.Size = new Size(50, 20);
            capsuleSwitch_HighLight.TabIndex = 2;
            capsuleSwitch_HighLight.Text = "capsuleSwitch1";
            capsuleSwitch_HighLight.TextColor = Color.White;
            capsuleSwitch_HighLight.ThumbColor = Color.White;
            capsuleSwitch_HighLight.IsOnChanged += HighlightCapsuleSwitch_IsOnChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(83, 6);
            label2.MinimumSize = new Size(100, 20);
            label2.Name = "label2";
            label2.Size = new Size(100, 20);
            label2.TabIndex = 1;
            label2.Text = "高亮显示";
            label2.TextAlign = ContentAlignment.MiddleLeft;
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
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "StatusOverlayForm";
            ShowInTaskbar = false;
            Text = "StatusOverlayForm";
            TopMost = true;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Label label_HotKey5;
        private DIYComponents.CapsuleSwitch capsuleSwitch_HighLight;
        private Label label2;
        private Label label9;
        private Label label_HotKey4;
        private Label label7;
        private Label label_HotKey3;
        private DIYComponents.CapsuleSwitch capsuleSwitch_RefreshStore;
        private Label label5;
        private Label label_HotKey2;
        private DIYComponents.CapsuleSwitch capsuleSwitch_GetCard;
        private Label label3;
        private Label label_HotKey1;
    }
}