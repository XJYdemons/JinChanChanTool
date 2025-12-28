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
            panel_Border = new Panel();
            panel_Client = new Panel();
            panelMain = new Panel();
            panel_TitleBar = new Panel();
            label_Title = new Label();
            button_Minimize = new Button();
            button_Close = new Button();
            panel_Border.SuspendLayout();
            panel_Client.SuspendLayout();
            panel_TitleBar.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Border
            // 
            panel_Border.BackColor = Color.FromArgb(250, 250, 250);
            panel_Border.Controls.Add(panel_Client);
            panel_Border.Dock = DockStyle.Fill;
            panel_Border.Location = new Point(0, 0);
            panel_Border.Margin = new Padding(0);
            panel_Border.Name = "panel_Border";
            panel_Border.Padding = new Padding(3, 3, 4, 4);
            panel_Border.Size = new Size(600, 500);
            panel_Border.TabIndex = 0;
            // 
            // panel_Client
            // 
            panel_Client.BackColor = Color.White;
            panel_Client.Controls.Add(panelMain);
            panel_Client.Controls.Add(panel_TitleBar);
            panel_Client.Dock = DockStyle.Fill;
            panel_Client.Location = new Point(3, 3);
            panel_Client.Margin = new Padding(0);
            panel_Client.Name = "panel_Client";
            panel_Client.Size = new Size(593, 493);
            panel_Client.TabIndex = 0;
            // 
            // panelMain
            // 
            panelMain.AutoScroll = true;
            panelMain.BackColor = Color.White;
            panelMain.Location = new Point(0, 27);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(593, 463);
            panelMain.TabIndex = 0;
            // 
            // panel_TitleBar
            // 
            panel_TitleBar.BackColor = Color.White;
            panel_TitleBar.Controls.Add(label_Title);
            panel_TitleBar.Controls.Add(button_Minimize);
            panel_TitleBar.Controls.Add(button_Close);
            panel_TitleBar.Location = new Point(0, 0);
            panel_TitleBar.Name = "panel_TitleBar";
            panel_TitleBar.Size = new Size(593, 25);
            panel_TitleBar.TabIndex = 1;
            // 
            // label_Title
            // 
            label_Title.AutoSize = true;
            label_Title.Location = new Point(4, 1);
            label_Title.MinimumSize = new Size(80, 23);
            label_Title.Name = "label_Title";
            label_Title.Size = new Size(80, 23);
            label_Title.TabIndex = 10;
            label_Title.Text = "选择装备";
            label_Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button_Minimize
            // 
            button_Minimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Minimize.FlatAppearance.BorderSize = 0;
            button_Minimize.FlatStyle = FlatStyle.Flat;
            button_Minimize.Location = new Point(546, 1);
            button_Minimize.Margin = new Padding(0);
            button_Minimize.Name = "button_Minimize";
            button_Minimize.Size = new Size(23, 23);
            button_Minimize.TabIndex = 8;
            button_Minimize.Text = "—";
            button_Minimize.UseVisualStyleBackColor = true;
            button_Minimize.Click += Button_Minimize_Click;
            // 
            // button_Close
            // 
            button_Close.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Close.FlatAppearance.BorderSize = 0;
            button_Close.FlatStyle = FlatStyle.Flat;
            button_Close.Location = new Point(570, 1);
            button_Close.Margin = new Padding(0);
            button_Close.Name = "button_Close";
            button_Close.Size = new Size(23, 23);
            button_Close.TabIndex = 7;
            button_Close.Text = "X";
            button_Close.UseVisualStyleBackColor = true;
            button_Close.Click += Button_Close_Click;
            // 
            // EquipmentForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(600, 500);
            Controls.Add(panel_Border);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EquipmentForm";
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            panel_Border.ResumeLayout(false);
            panel_Client.ResumeLayout(false);
            panel_TitleBar.ResumeLayout(false);
            panel_TitleBar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Border;
        private Panel panel_Client;
        private Panel panelMain;
        private Panel panel_TitleBar;
        private Label label_Title;
        private Button button_Minimize;
        private Button button_Close;
    }
}
