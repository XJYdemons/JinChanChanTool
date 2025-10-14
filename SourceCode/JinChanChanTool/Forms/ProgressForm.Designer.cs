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
            progressBar1 = new ProgressBar();
            lblStatus = new Label();
            panel_BackGround = new Panel();
            panel_BackGround.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Dock = DockStyle.Fill;
            progressBar1.Location = new Point(5, 5);
            progressBar1.Margin = new Padding(5);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(326, 57);
            progressBar1.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(15, 48);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 17);
            lblStatus.TabIndex = 1;
            // 
            // panel_BackGround
            // 
            panel_BackGround.AutoScroll = true;
            panel_BackGround.AutoSize = true;
            panel_BackGround.BackColor = Color.White;
            panel_BackGround.Controls.Add(progressBar1);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(0, 0);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Padding = new Padding(5);
            panel_BackGround.Size = new Size(336, 67);
            panel_BackGround.TabIndex = 2;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(336, 67);
            Controls.Add(panel_BackGround);
            Controls.Add(lblStatus);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ProgressForm";
            Text = "获取进度";
            TopMost = true;
            panel_BackGround.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar1;
        private Label lblStatus;
        private Panel panel_BackGround;
    }
}