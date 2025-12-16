using JinChanChanTool.DIYComponents;

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
            panel_Background = new CustomPanel();
            SuspendLayout();
            // 
            // panel_Background
            // 
            panel_Background.BorderColor = Color.Magenta;
            panel_Background.Location = new Point(0, 0);
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
            ClientSize = new Size(611, 195);
            Controls.Add(panel_Background);
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
        public CustomPanel panel_Background;
    }
}