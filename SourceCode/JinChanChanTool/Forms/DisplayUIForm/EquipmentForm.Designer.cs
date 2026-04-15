namespace JinChanChanTool.Forms.DisplayUIForm
{
    partial class EquipmentForm
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
            panel客户区 = new Panel();
            panel_标题栏 = new Panel();
            label_标题 = new Label();
            button_最小化 = new Button();
            button_关闭 = new Button();
            panel_主背景.SuspendLayout();
            panel_副背景.SuspendLayout();
            panel_标题栏.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Border
            // 
            panel_主背景.BackColor = Color.FromArgb(250, 250, 250);
            panel_主背景.Controls.Add(panel_副背景);
            panel_主背景.Dock = DockStyle.Fill;
            panel_主背景.Location = new Point(0, 0);
            panel_主背景.Margin = new Padding(0);
            panel_主背景.Name = "panel_Border";
            panel_主背景.Padding = new Padding(3, 3, 4, 4);
            panel_主背景.Size = new Size(600, 500);
            panel_主背景.TabIndex = 0;
            // 
            // panel_Client
            // 
            panel_副背景.BackColor = Color.White;
            panel_副背景.Controls.Add(panel客户区);
            panel_副背景.Controls.Add(panel_标题栏);
            panel_副背景.Dock = DockStyle.Fill;
            panel_副背景.Location = new Point(3, 3);
            panel_副背景.Margin = new Padding(0);
            panel_副背景.Name = "panel_Client";
            panel_副背景.Size = new Size(593, 493);
            panel_副背景.TabIndex = 0;
            // 
            // panelMain
            // 
            panel客户区.AutoScroll = true;
            panel客户区.BackColor = Color.White;
            panel客户区.Location = new Point(0, 27);
            panel客户区.Name = "panelMain";
            panel客户区.Size = new Size(593, 463);
            panel客户区.TabIndex = 0;
            // 
            // panel_TitleBar
            // 
            panel_标题栏.BackColor = Color.White;
            panel_标题栏.Controls.Add(label_标题);
            panel_标题栏.Controls.Add(button_最小化);
            panel_标题栏.Controls.Add(button_关闭);
            panel_标题栏.Location = new Point(0, 0);
            panel_标题栏.Name = "panel_TitleBar";
            panel_标题栏.Size = new Size(593, 25);
            panel_标题栏.TabIndex = 1;
            // 
            // label_Title
            // 
            label_标题.AutoSize = true;
            label_标题.Location = new Point(4, 1);
            label_标题.MinimumSize = new Size(80, 23);
            label_标题.Name = "label_Title";
            label_标题.Size = new Size(80, 23);
            label_标题.TabIndex = 10;
            label_标题.Text = "选择装备";
            label_标题.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button_Minimize
            // 
            button_最小化.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_最小化.FlatAppearance.BorderSize = 0;
            button_最小化.FlatStyle = FlatStyle.Flat;
            button_最小化.Location = new Point(546, 1);
            button_最小化.Margin = new Padding(0);
            button_最小化.Name = "button_Minimize";
            button_最小化.Size = new Size(23, 23);
            button_最小化.TabIndex = 8;
            button_最小化.Text = "—";
            button_最小化.UseVisualStyleBackColor = true;
            button_最小化.Click += Button_Minimize_Click;
            // 
            // button_Close
            // 
            button_关闭.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_关闭.FlatAppearance.BorderSize = 0;
            button_关闭.FlatStyle = FlatStyle.Flat;
            button_关闭.Location = new Point(570, 1);
            button_关闭.Margin = new Padding(0);
            button_关闭.Name = "button_Close";
            button_关闭.Size = new Size(23, 23);
            button_关闭.TabIndex = 7;
            button_关闭.Text = "X";
            button_关闭.UseVisualStyleBackColor = true;
            button_关闭.Click += Button_Close_Click;
            // 
            // EquipmentForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(600, 500);
            Controls.Add(panel_主背景);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EquipmentForm";
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            panel_主背景.ResumeLayout(false);
            panel_副背景.ResumeLayout(false);
            panel_标题栏.ResumeLayout(false);
            panel_标题栏.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_主背景;
        private Panel panel_副背景;
        private Panel panel客户区;
        private Panel panel_标题栏;
        private Label label_标题;
        private Button button_最小化;
        private Button button_关闭;
    }
}
