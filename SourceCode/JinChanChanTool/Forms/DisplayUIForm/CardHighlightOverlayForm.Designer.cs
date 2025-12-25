namespace JinChanChanTool.Forms.DisplayUIForm
{
    partial class CardHighlightOverlayForm
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
            components = new System.ComponentModel.Container();
            animationTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            //
            // animationTimer
            //
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;
            //
            // CardHighlightOverlayForm
            //
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Magenta;
            ClientSize = new Size(800, 600);
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
            Name = "CardHighlightOverlayForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "CardHighlightOverlay";
            TopMost = true;
            TransparencyKey = Color.Magenta;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer animationTimer;
    }
}
