namespace JinChanChanTool.Forms
{
    partial class SelectForm
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
            draggingBar = new Panel();
            panel_Background = new Panel();
            SuspendLayout();
            // 
            // draggingBar
            // 
            draggingBar.BackColor = Color.FromArgb(218, 218, 218);
            draggingBar.BorderStyle = BorderStyle.FixedSingle;
            draggingBar.Location = new Point(0, 0);
            draggingBar.Name = "draggingBar";
            draggingBar.Size = new Size(6, 164);
            draggingBar.TabIndex = 1;
            draggingBar.MouseDoubleClick += panel_MouseDoubleClick;
            // 
            // panel_Background
            // 
            panel_Background.Location = new Point(6, 0);
            panel_Background.Margin = new Padding(0);
            panel_Background.Name = "panel_Background";
            panel_Background.Size = new Size(608, 182);
            panel_Background.TabIndex = 5;
            // 
            // SelectForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Magenta;
            ClientSize = new Size(617, 195);
            Controls.Add(panel_Background);
            Controls.Add(draggingBar);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SelectForm";
            ShowInTaskbar = false;
            Text = "Selector";
            TopMost = true;
            TransparencyKey = Color.Magenta;
            Load += Selector_Load;
            ResumeLayout(false);
        }

        #endregion
        public Panel draggingBar;
        public Panel panel_Background;
    }
}