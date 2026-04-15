namespace JinChanChanTool.Forms
{
    partial class ProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            progressBar = new ProgressBar();
            panel_背景 = new Panel();
            panel_标题栏 = new Panel();
            label_标题 = new Label();
            button_最小化 = new Button();
            button_关闭 = new Button();
            panel_背景.SuspendLayout();
            panel_标题栏.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar.Location = new Point(5, 34);
            progressBar.Margin = new Padding(5);
            progressBar.Name = "progressBar1";
            progressBar.Size = new Size(326, 27);
            progressBar.TabIndex = 0;
            // 
            // panel_BackGround
            // 
            panel_背景.BackColor = Color.White;
            panel_背景.Controls.Add(panel_标题栏);
            panel_背景.Controls.Add(progressBar);
            panel_背景.Dock = DockStyle.Fill;
            panel_背景.Location = new Point(0, 0);
            panel_背景.Margin = new Padding(0);
            panel_背景.Name = "panel_BackGround";
            panel_背景.Padding = new Padding(5);
            panel_背景.Size = new Size(336, 72);
            panel_背景.TabIndex = 2;
            // 
            // panel_标题栏
            // 
            panel_标题栏.Controls.Add(label_标题);
            panel_标题栏.Controls.Add(button_最小化);
            panel_标题栏.Controls.Add(button_关闭);
            panel_标题栏.Location = new Point(0, 0);
            panel_标题栏.Name = "panel_标题栏";
            panel_标题栏.Size = new Size(336, 25);
            panel_标题栏.TabIndex = 7;
            // 
            // label_标题
            // 
            label_标题.AutoSize = true;
            label_标题.Location = new Point(1, 1);
            label_标题.MinimumSize = new Size(80, 23);
            label_标题.Name = "label_标题";
            label_标题.Size = new Size(200, 23);
            label_标题.TabIndex = 10;
            label_标题.Text = "正在更新当前赛季英雄推荐装备数据";
            label_标题.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button_最小化
            // 
            button_最小化.FlatAppearance.BorderSize = 0;
            button_最小化.FlatStyle = FlatStyle.Flat;
            button_最小化.Location = new Point(290, 1);
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
            button_关闭.Location = new Point(313, 1);
            button_关闭.Margin = new Padding(0);
            button_关闭.Name = "button_关闭";
            button_关闭.Size = new Size(23, 23);
            button_关闭.TabIndex = 7;
            button_关闭.Text = "X";
            button_关闭.UseVisualStyleBackColor = true;
            button_关闭.Click += button_关闭_Click;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(336, 72);
            Controls.Add(panel_背景);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ProgressForm";
            Text = "获取进度";
            TopMost = true;
            panel_背景.ResumeLayout(false);
            panel_标题栏.ResumeLayout(false);
            panel_标题栏.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar progressBar;
        private Panel panel_背景;
        private Panel panel_标题栏;
        private Label label_标题;
        private Button button_最小化;
        private Button button_关闭;
    }
}