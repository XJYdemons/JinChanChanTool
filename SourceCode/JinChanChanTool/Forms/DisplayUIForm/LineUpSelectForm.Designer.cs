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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LineUpSelectForm));
            panel_Border = new Panel();
            panel_Client = new Panel();
            panel_Main = new Panel();
            flowLayoutPanel_LineUps = new FlowLayoutPanel();
            panel_Filter = new Panel();
            label_UpdateTime = new Label();
            textBox_Search = new TextBox();
            label_Search = new Label();
            comboBox_SortBy = new ComboBox();
            label_SortBy = new Label();
            comboBox_TierFilter = new ComboBox();
            label_TierFilter = new Label();
            panel_Bottom = new Panel();
            button_Cancel = new Button();
            button_Confirm = new Button();
            label_Info = new Label();
            panel_TitleBar = new Panel();
            label_Title = new Label();
            button_Minimize = new Button();
            button_Close = new Button();
            panel_Border.SuspendLayout();
            panel_Client.SuspendLayout();
            panel_Main.SuspendLayout();
            panel_Filter.SuspendLayout();
            panel_Bottom.SuspendLayout();
            panel_TitleBar.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Border
            // 
            panel_Border.BackColor = Color.FromArgb(45, 45, 45);
            panel_Border.Controls.Add(panel_Client);
            panel_Border.Dock = DockStyle.Fill;
            panel_Border.Location = new Point(0, 0);
            panel_Border.Margin = new Padding(0);
            panel_Border.Name = "panel_Border";
            panel_Border.Padding = new Padding(3, 3, 4, 4);
            panel_Border.Size = new Size(1200, 650);
            panel_Border.TabIndex = 0;
            // 
            // panel_Client
            // 
            panel_Client.BackColor = Color.White;
            panel_Client.Controls.Add(panel_Main);
            panel_Client.Controls.Add(panel_Filter);
            panel_Client.Controls.Add(panel_Bottom);
            panel_Client.Controls.Add(panel_TitleBar);
            panel_Client.Dock = DockStyle.Fill;
            panel_Client.Location = new Point(3, 3);
            panel_Client.Margin = new Padding(0);
            panel_Client.Name = "panel_Client";
            panel_Client.Size = new Size(1193, 643);
            panel_Client.TabIndex = 0;
            // 
            // panel_Main
            // 
            panel_Main.BackColor = Color.FromArgb(37, 37, 38);
            panel_Main.Controls.Add(flowLayoutPanel_LineUps);
            panel_Main.Dock = DockStyle.Fill;
            panel_Main.Location = new Point(0, 63);
            panel_Main.Name = "panel_Main";
            panel_Main.Padding = new Padding(5);
            panel_Main.Size = new Size(1193, 530);
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
            flowLayoutPanel_LineUps.Size = new Size(1183, 520);
            flowLayoutPanel_LineUps.TabIndex = 0;
            flowLayoutPanel_LineUps.WrapContents = false;
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
            panel_Filter.Location = new Point(0, 25);
            panel_Filter.Name = "panel_Filter";
            panel_Filter.Size = new Size(1193, 38);
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
            // panel_Bottom
            // 
            panel_Bottom.BackColor = Color.FromArgb(45, 45, 48);
            panel_Bottom.Controls.Add(button_Cancel);
            panel_Bottom.Controls.Add(button_Confirm);
            panel_Bottom.Controls.Add(label_Info);
            panel_Bottom.Dock = DockStyle.Bottom;
            panel_Bottom.Location = new Point(0, 593);
            panel_Bottom.Name = "panel_Bottom";
            panel_Bottom.Size = new Size(1193, 50);
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
            // panel_TitleBar
            // 
            panel_TitleBar.BackColor = Color.FromArgb(45, 45, 48);
            panel_TitleBar.Controls.Add(label_Title);
            panel_TitleBar.Controls.Add(button_Minimize);
            panel_TitleBar.Controls.Add(button_Close);
            panel_TitleBar.Dock = DockStyle.Top;
            panel_TitleBar.Location = new Point(0, 0);
            panel_TitleBar.Name = "panel_TitleBar";
            panel_TitleBar.Size = new Size(1193, 25);
            panel_TitleBar.TabIndex = 3;
            // 
            // label_Title
            // 
            label_Title.AutoSize = true;
            label_Title.ForeColor = Color.White;
            label_Title.Location = new Point(4, 1);
            label_Title.MinimumSize = new Size(80, 23);
            label_Title.Name = "label_Title";
            label_Title.Size = new Size(80, 23);
            label_Title.TabIndex = 10;
            label_Title.Text = "推荐阵容";
            label_Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button_Minimize
            // 
            button_Minimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Minimize.FlatAppearance.BorderSize = 0;
            button_Minimize.FlatStyle = FlatStyle.Flat;
            button_Minimize.ForeColor = Color.White;
            button_Minimize.Location = new Point(1146, 1);
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
            button_Close.ForeColor = Color.White;
            button_Close.Location = new Point(1170, 1);
            button_Close.Margin = new Padding(0);
            button_Close.Name = "button_Close";
            button_Close.Size = new Size(23, 23);
            button_Close.TabIndex = 7;
            button_Close.Text = "X";
            button_Close.UseVisualStyleBackColor = true;
            button_Close.Click += Button_Close_Click;
            // 
            // LineUpSelectForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(1200, 650);
            Controls.Add(panel_Border);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "LineUpSelectForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "推荐阵容";
            panel_Border.ResumeLayout(false);
            panel_Client.ResumeLayout(false);
            panel_Main.ResumeLayout(false);
            panel_Filter.ResumeLayout(false);
            panel_Filter.PerformLayout();
            panel_Bottom.ResumeLayout(false);
            panel_Bottom.PerformLayout();
            panel_TitleBar.ResumeLayout(false);
            panel_TitleBar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Border;
        private Panel panel_Client;
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
        private Panel panel_TitleBar;
        private Label label_Title;
        private Button button_Minimize;
        private Button button_Close;
    }
}
