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
            listBox_Processes = new ListBox();
            button_Select = new Button();
            button_Refresh = new Button();
            panel_BackGround = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel_BackGround.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // listBox_Processes
            // 
            listBox_Processes.BorderStyle = BorderStyle.FixedSingle;
            listBox_Processes.Dock = DockStyle.Fill;
            listBox_Processes.FormattingEnabled = true;
            listBox_Processes.ItemHeight = 17;
            listBox_Processes.Location = new Point(0, 0);
            listBox_Processes.Margin = new Padding(0);
            listBox_Processes.Name = "listBox_Processes";
            listBox_Processes.Size = new Size(717, 292);
            listBox_Processes.TabIndex = 0;
            // 
            // button_Select
            // 
            button_Select.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button_Select.FlatStyle = FlatStyle.Flat;
            button_Select.Location = new Point(0, 300);
            button_Select.Margin = new Padding(0);
            button_Select.Name = "button_Select";
            button_Select.Size = new Size(717, 28);
            button_Select.TabIndex = 1;
            button_Select.Text = "选定此进程";
            button_Select.UseVisualStyleBackColor = true;
            // 
            // button_Refresh
            // 
            button_Refresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button_Refresh.FlatStyle = FlatStyle.Flat;
            button_Refresh.Location = new Point(0, 338);
            button_Refresh.Margin = new Padding(0);
            button_Refresh.Name = "button_Refresh";
            button_Refresh.Size = new Size(717, 28);
            button_Refresh.TabIndex = 2;
            button_Refresh.Text = "刷新列表";
            button_Refresh.UseVisualStyleBackColor = true;
            // 
            // panel_BackGround
            // 
            panel_BackGround.AutoScroll = true;
            panel_BackGround.AutoSize = true;
            panel_BackGround.BackColor = Color.White;
            panel_BackGround.Controls.Add(tableLayoutPanel1);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(0, 0);
            panel_BackGround.Margin = new Padding(0);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Padding = new Padding(5);
            panel_BackGround.Size = new Size(727, 376);
            panel_BackGround.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(listBox_Processes, 0, 0);           
            tableLayoutPanel1.Controls.Add(button_Refresh, 0, 1);
            tableLayoutPanel1.Controls.Add(button_Select, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(5, 5);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.Size = new Size(717, 366);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // ProcessSelectorForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(727, 376);
            Controls.Add(panel_BackGround);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ProcessSelectorForm";
            Text = "ProcessSelectorForm";
            TopMost = true;
            panel_BackGround.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox_Processes;
        private Button button_Select;
        private Button button_Refresh;
        private Panel panel_BackGround;
        private TableLayoutPanel tableLayoutPanel1;
    }
}