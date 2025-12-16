namespace JinChanChanTool.Forms.DisplayUIForm
{
    partial class LineUpSelectForm
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
            panel_Main = new Panel();
            flowLayoutPanel_LineUps = new FlowLayoutPanel();
            panel_Bottom = new Panel();
            button_Cancel = new Button();
            button_Confirm = new Button();
            label_Info = new Label();
            panel_Filter = new Panel();
            label_UpdateTime = new Label();
            textBox_Search = new TextBox();
            label_Search = new Label();
            comboBox_SortBy = new ComboBox();
            label_SortBy = new Label();
            comboBox_TierFilter = new ComboBox();
            label_TierFilter = new Label();
            panel_Main.SuspendLayout();
            panel_Bottom.SuspendLayout();
            panel_Filter.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Main
            // 
            panel_Main.BackColor = Color.FromArgb(37, 37, 38);
            panel_Main.Controls.Add(flowLayoutPanel_LineUps);
            panel_Main.Dock = DockStyle.Fill;
            panel_Main.Location = new Point(0, 38);
            panel_Main.Name = "panel_Main";
            panel_Main.Padding = new Padding(5);
            panel_Main.Size = new Size(1200, 562);
            panel_Main.TabIndex = 0;
            // 
            // flowLayoutPanel_LineUps
            // 
            flowLayoutPanel_LineUps.AutoScroll = true;
            flowLayoutPanel_LineUps.BackColor = Color.FromArgb(37, 37, 38);
            flowLayoutPanel_LineUps.Dock = DockStyle.Fill;
            flowLayoutPanel_LineUps.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel_LineUps.Location = new Point(5, 5);
            flowLayoutPanel_LineUps.Name = "flowLayoutPanel_LineUps";
            flowLayoutPanel_LineUps.Size = new Size(1190, 552);
            flowLayoutPanel_LineUps.TabIndex = 0;
            flowLayoutPanel_LineUps.WrapContents = false;
            // 
            // panel_Bottom
            // 
            panel_Bottom.BackColor = Color.FromArgb(45, 45, 48);
            panel_Bottom.Controls.Add(button_Cancel);
            panel_Bottom.Controls.Add(button_Confirm);
            panel_Bottom.Controls.Add(label_Info);
            panel_Bottom.Dock = DockStyle.Bottom;
            panel_Bottom.Location = new Point(0, 600);
            panel_Bottom.Name = "panel_Bottom";
            panel_Bottom.Size = new Size(1200, 50);
            panel_Bottom.TabIndex = 1;
            // 
            // button_Cancel
            // 
            button_Cancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Cancel.BackColor = Color.FromArgb(60, 60, 63);
            button_Cancel.FlatAppearance.BorderColor = Color.Gray;
            button_Cancel.FlatStyle = FlatStyle.Flat;
            button_Cancel.ForeColor = Color.White;
            button_Cancel.Location = new Point(1105, 10);
            button_Cancel.Name = "button_Cancel";
            button_Cancel.Size = new Size(80, 30);
            button_Cancel.TabIndex = 2;
            button_Cancel.Text = "取消";
            button_Cancel.UseVisualStyleBackColor = false;
            button_Cancel.Click += button_Cancel_Click;
            // 
            // button_Confirm
            // 
            button_Confirm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Confirm.BackColor = Color.FromArgb(60, 60, 63);
            button_Confirm.Enabled = false;
            button_Confirm.FlatAppearance.BorderColor = Color.Gray;
            button_Confirm.FlatStyle = FlatStyle.Flat;
            button_Confirm.ForeColor = Color.White;
            button_Confirm.Location = new Point(1015, 10);
            button_Confirm.Name = "button_Confirm";
            button_Confirm.Size = new Size(80, 30);
            button_Confirm.TabIndex = 1;
            button_Confirm.Text = "应用";
            button_Confirm.UseVisualStyleBackColor = false;
            button_Confirm.Click += button_Confirm_Click;
            // 
            // label_Info
            // 
            label_Info.AutoSize = true;
            label_Info.ForeColor = Color.Silver;
            label_Info.Location = new Point(15, 17);
            label_Info.Name = "label_Info";
            label_Info.Size = new Size(176, 17);
            label_Info.TabIndex = 0;
            label_Info.Text = "请选择一个阵容导入到当前变阵";
            // 
            // panel_Filter
            // 
            panel_Filter.BackColor = Color.FromArgb(45, 45, 48);
            panel_Filter.Controls.Add(label_UpdateTime);
            panel_Filter.Controls.Add(textBox_Search);
            panel_Filter.Controls.Add(label_Search);
            panel_Filter.Controls.Add(comboBox_SortBy);
            panel_Filter.Controls.Add(label_SortBy);
            panel_Filter.Controls.Add(comboBox_TierFilter);
            panel_Filter.Controls.Add(label_TierFilter);
            panel_Filter.Dock = DockStyle.Top;
            panel_Filter.Location = new Point(0, 0);
            panel_Filter.Name = "panel_Filter";
            panel_Filter.Size = new Size(1200, 38);
            panel_Filter.TabIndex = 2;
            // 
            // label_UpdateTime
            // 
            label_UpdateTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_UpdateTime.AutoSize = true;
            label_UpdateTime.ForeColor = Color.Gray;
            label_UpdateTime.Location = new Point(1000, 10);
            label_UpdateTime.Name = "label_UpdateTime";
            label_UpdateTime.Size = new Size(68, 17);
            label_UpdateTime.TabIndex = 2;
            label_UpdateTime.Text = "更新时间: -";
            // 
            // textBox_Search
            // 
            textBox_Search.BackColor = Color.FromArgb(60, 60, 63);
            textBox_Search.BorderStyle = BorderStyle.FixedSingle;
            textBox_Search.ForeColor = Color.White;
            textBox_Search.Location = new Point(420, 8);
            textBox_Search.Name = "textBox_Search";
            textBox_Search.PlaceholderText = "阵容名称/标签/英雄名称/阵容描述...";
            textBox_Search.Size = new Size(218, 23);
            textBox_Search.TabIndex = 6;
            textBox_Search.TextChanged += textBox_Search_TextChanged;
            // 
            // label_Search
            // 
            label_Search.AutoSize = true;
            label_Search.ForeColor = Color.White;
            label_Search.Location = new Point(375, 8);
            label_Search.MinimumSize = new Size(35, 23);
            label_Search.Name = "label_Search";
            label_Search.Size = new Size(35, 23);
            label_Search.TabIndex = 5;
            label_Search.Text = "搜索:";
            label_Search.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // comboBox_SortBy
            // 
            comboBox_SortBy.BackColor = Color.FromArgb(60, 60, 63);
            comboBox_SortBy.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SortBy.FlatStyle = FlatStyle.Flat;
            comboBox_SortBy.ForeColor = Color.White;
            comboBox_SortBy.FormattingEnabled = true;
            comboBox_SortBy.Items.AddRange(new object[] { "评级优先", "胜率", "前四率", "选取率", "平均名次" });
            comboBox_SortBy.Location = new Point(255, 8);
            comboBox_SortBy.Name = "comboBox_SortBy";
            comboBox_SortBy.Size = new Size(100, 25);
            comboBox_SortBy.TabIndex = 4;
            comboBox_SortBy.SelectedIndexChanged += comboBox_SortBy_SelectedIndexChanged;
            // 
            // label_SortBy
            // 
            label_SortBy.AutoSize = true;
            label_SortBy.ForeColor = Color.White;
            label_SortBy.Location = new Point(185, 8);
            label_SortBy.MinimumSize = new Size(59, 25);
            label_SortBy.Name = "label_SortBy";
            label_SortBy.Size = new Size(59, 25);
            label_SortBy.TabIndex = 3;
            label_SortBy.Text = "排序方式:";
            label_SortBy.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // comboBox_TierFilter
            // 
            comboBox_TierFilter.BackColor = Color.FromArgb(60, 60, 63);
            comboBox_TierFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_TierFilter.FlatStyle = FlatStyle.Flat;
            comboBox_TierFilter.ForeColor = Color.White;
            comboBox_TierFilter.FormattingEnabled = true;
            comboBox_TierFilter.Items.AddRange(new object[] { "全部", "S", "A", "B", "C", "D" });
            comboBox_TierFilter.Location = new Point(85, 8);
            comboBox_TierFilter.Name = "comboBox_TierFilter";
            comboBox_TierFilter.Size = new Size(80, 25);
            comboBox_TierFilter.TabIndex = 1;
            comboBox_TierFilter.SelectedIndexChanged += comboBox_TierFilter_SelectedIndexChanged;
            // 
            // label_TierFilter
            // 
            label_TierFilter.AutoSize = true;
            label_TierFilter.ForeColor = Color.White;
            label_TierFilter.Location = new Point(15, 8);
            label_TierFilter.MinimumSize = new Size(59, 25);
            label_TierFilter.Name = "label_TierFilter";
            label_TierFilter.Size = new Size(59, 25);
            label_TierFilter.TabIndex = 0;
            label_TierFilter.Text = "评级筛选:";
            label_TierFilter.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LineUpSelectForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(1200, 650);
            Controls.Add(panel_Main);
            Controls.Add(panel_Filter);
            Controls.Add(panel_Bottom);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LineUpSelectForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "推荐阵容";
            panel_Main.ResumeLayout(false);
            panel_Bottom.ResumeLayout(false);
            panel_Bottom.PerformLayout();
            panel_Filter.ResumeLayout(false);
            panel_Filter.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Main;
        private FlowLayoutPanel flowLayoutPanel_LineUps;
        private Panel panel_Bottom;
        private Button button_Cancel;
        private Button button_Confirm;
        private Label label_Info;
        private Panel panel_Filter;
        private ComboBox comboBox_TierFilter;
        private Label label_TierFilter;
        private ComboBox comboBox_SortBy;
        private Label label_SortBy;
        private Label label_UpdateTime;
        private TextBox textBox_Search;
        private Label label_Search;
    }
}
