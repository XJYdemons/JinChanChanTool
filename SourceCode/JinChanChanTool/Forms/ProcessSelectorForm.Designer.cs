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
            SuspendLayout();
            // 
            // listBox_Processes
            // 
            listBox_Processes.Dock = DockStyle.Fill;
            listBox_Processes.FormattingEnabled = true;
            listBox_Processes.ItemHeight = 17;
            listBox_Processes.Location = new Point(0, 0);
            listBox_Processes.Name = "listBox_Processes";
            listBox_Processes.Size = new Size(607, 356);
            listBox_Processes.TabIndex = 0;
            // 
            // button_Select
            // 
            button_Select.Dock = DockStyle.Bottom;
            button_Select.Location = new Point(0, 333);
            button_Select.Name = "button_Select";
            button_Select.Size = new Size(607, 23);
            button_Select.TabIndex = 1;
            button_Select.Text = "选定此进程";
            button_Select.UseVisualStyleBackColor = true;
            // 
            // button_Refresh
            // 
            button_Refresh.Dock = DockStyle.Bottom;
            button_Refresh.Location = new Point(0, 310);
            button_Refresh.Name = "button_Refresh";
            button_Refresh.Size = new Size(607, 23);
            button_Refresh.TabIndex = 2;
            button_Refresh.Text = "刷新列表";
            button_Refresh.UseVisualStyleBackColor = true;
            // 
            // ProcessSelectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(607, 356);
            Controls.Add(button_Refresh);
            Controls.Add(button_Select);
            Controls.Add(listBox_Processes);
            Name = "ProcessSelectorForm";
            Text = "ProcessSelectorForm";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBox_Processes;
        private Button button_Select;
        private Button button_Refresh;
    }
}