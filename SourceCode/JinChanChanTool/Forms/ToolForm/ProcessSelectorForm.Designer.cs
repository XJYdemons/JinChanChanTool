namespace JinChanChanTool.Forms
{
    partial class ProcessSelectorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessSelectorForm));
            listBox_Processes = new ListBox();
            button_Select = new Button();
            button_Refresh = new Button();
            panel_BackGround = new Panel();
            panel3 = new Panel();
            label29 = new Label();
            button2 = new Button();
            button3 = new Button();
            panel1 = new Panel();
            panel_BackGround.SuspendLayout();
            panel3.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // listBox_Processes
            // 
            listBox_Processes.BorderStyle = BorderStyle.FixedSingle;
            listBox_Processes.FormattingEnabled = true;
            listBox_Processes.ItemHeight = 17;
            listBox_Processes.Location = new Point(5, 26);
            listBox_Processes.Margin = new Padding(0);
            listBox_Processes.Name = "listBox_Processes";
            listBox_Processes.Size = new Size(718, 274);
            listBox_Processes.TabIndex = 0;
            // 
            // button_Select
            // 
            button_Select.FlatStyle = FlatStyle.Flat;
            button_Select.Location = new Point(5, 336);
            button_Select.Margin = new Padding(0);
            button_Select.Name = "button_Select";
            button_Select.Size = new Size(718, 28);
            button_Select.TabIndex = 1;
            button_Select.Text = "选定此进程";
            button_Select.UseVisualStyleBackColor = true;
            // 
            // button_Refresh
            // 
            button_Refresh.FlatStyle = FlatStyle.Flat;
            button_Refresh.Location = new Point(5, 304);
            button_Refresh.Margin = new Padding(0);
            button_Refresh.Name = "button_Refresh";
            button_Refresh.Size = new Size(718, 28);
            button_Refresh.TabIndex = 2;
            button_Refresh.Text = "刷新列表";
            button_Refresh.UseVisualStyleBackColor = true;
            // 
            // panel_BackGround
            // 
            panel_BackGround.BackColor = Color.White;
            panel_BackGround.Controls.Add(panel3);
            panel_BackGround.Controls.Add(button_Select);
            panel_BackGround.Controls.Add(listBox_Processes);
            panel_BackGround.Controls.Add(button_Refresh);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(3, 3);
            panel_BackGround.Margin = new Padding(0);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Size = new Size(728, 368);
            panel_BackGround.TabIndex = 3;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(label29);
            panel3.Controls.Add(button2);
            panel3.Controls.Add(button3);
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(728, 25);
            panel3.TabIndex = 212;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(4, 1);
            label29.MinimumSize = new Size(80, 23);
            label29.Name = "label29";
            label29.Size = new Size(80, 23);
            label29.TabIndex = 10;
            label29.Text = "设置";
            label29.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(678, 1);
            button2.Margin = new Padding(0);
            button2.Name = "button2";
            button2.Size = new Size(23, 23);
            button2.TabIndex = 8;
            button2.Text = "—";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button_最小化_Click;
            // 
            // button3
            // 
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(702, 1);
            button3.Margin = new Padding(0);
            button3.Name = "button3";
            button3.Size = new Size(23, 23);
            button3.TabIndex = 7;
            button3.Text = "X";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button_关闭_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(250, 250, 250);
            panel1.Controls.Add(panel_BackGround);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(3, 3, 4, 4);
            panel1.Size = new Size(735, 375);
            panel1.TabIndex = 4;
            // 
            // ProcessSelectorForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(735, 375);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ProcessSelectorForm";
            Text = "ProcessSelectorForm";
            TopMost = true;
            panel_BackGround.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBox_Processes;
        private Button button_Select;
        private Button button_Refresh;
        private Panel panel_BackGround;
        private Panel panel3;
        private Label label29;
        private Button button2;
        private Button button3;
        private Panel panel1;
    }
}