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
            panel_主背景 = new Panel();
            panel_副背景 = new Panel();
            panel_阵容区 = new Panel();
            flowLayoutPanel_阵容 = new FlowLayoutPanel();
            panel_筛选 = new Panel();
            label_更新时间 = new Label();
            textBox_搜索 = new TextBox();
            label_搜索 = new Label();
            comboBox_排序方式 = new ComboBox();
            label_排序方式 = new Label();
            comboBox_评级筛选 = new ComboBox();
            label_评级筛选 = new Label();
            panel_按钮区 = new Panel();
            button_取消 = new Button();
            button_应用 = new Button();
            label_阵容信息 = new Label();
            panel_标题栏 = new Panel();
            label_标题 = new Label();
            button_最小化 = new Button();
            button_关闭 = new Button();
            panel_主背景.SuspendLayout();
            panel_副背景.SuspendLayout();
            panel_阵容区.SuspendLayout();
            panel_筛选.SuspendLayout();
            panel_按钮区.SuspendLayout();
            panel_标题栏.SuspendLayout();
            SuspendLayout();
            // 
            // panel_主背景
            // 
            panel_主背景.BackColor = Color.FromArgb(45, 45, 45);
            panel_主背景.Controls.Add(panel_副背景);
            panel_主背景.Dock = DockStyle.Fill;
            panel_主背景.Location = new Point(0, 0);
            panel_主背景.Margin = new Padding(0);
            panel_主背景.Name = "panel_主背景";
            panel_主背景.Padding = new Padding(3, 3, 4, 4);
            panel_主背景.Size = new Size(1200, 650);
            panel_主背景.TabIndex = 0;
            // 
            // panel_副背景
            // 
            panel_副背景.BackColor = Color.White;
            panel_副背景.Controls.Add(panel_阵容区);
            panel_副背景.Controls.Add(panel_筛选);
            panel_副背景.Controls.Add(panel_按钮区);
            panel_副背景.Controls.Add(panel_标题栏);
            panel_副背景.Dock = DockStyle.Fill;
            panel_副背景.Location = new Point(3, 3);
            panel_副背景.Margin = new Padding(0);
            panel_副背景.Name = "panel_副背景";
            panel_副背景.Size = new Size(1193, 643);
            panel_副背景.TabIndex = 0;
            // 
            // panel_阵容区
            // 
            panel_阵容区.BackColor = Color.FromArgb(37, 37, 38);
            panel_阵容区.Controls.Add(flowLayoutPanel_阵容);
            panel_阵容区.Dock = DockStyle.Fill;
            panel_阵容区.Location = new Point(0, 63);
            panel_阵容区.Name = "panel_阵容区";
            panel_阵容区.Padding = new Padding(5);
            panel_阵容区.Size = new Size(1193, 530);
            panel_阵容区.TabIndex = 0;
            // 
            // flowLayoutPanel_阵容
            // 
            flowLayoutPanel_阵容.AutoScroll = true;
            flowLayoutPanel_阵容.BackColor = Color.FromArgb(37, 37, 38);
            flowLayoutPanel_阵容.Dock = DockStyle.Fill;
            flowLayoutPanel_阵容.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel_阵容.Location = new Point(5, 5);
            flowLayoutPanel_阵容.Name = "flowLayoutPanel_阵容";
            flowLayoutPanel_阵容.Size = new Size(1183, 520);
            flowLayoutPanel_阵容.TabIndex = 0;
            flowLayoutPanel_阵容.WrapContents = false;
            // 
            // panel_筛选
            // 
            panel_筛选.BackColor = Color.FromArgb(45, 45, 48);
            panel_筛选.Controls.Add(label_更新时间);
            panel_筛选.Controls.Add(textBox_搜索);
            panel_筛选.Controls.Add(label_搜索);
            panel_筛选.Controls.Add(comboBox_排序方式);
            panel_筛选.Controls.Add(label_排序方式);
            panel_筛选.Controls.Add(comboBox_评级筛选);
            panel_筛选.Controls.Add(label_评级筛选);
            panel_筛选.Dock = DockStyle.Top;
            panel_筛选.Location = new Point(0, 25);
            panel_筛选.Name = "panel_筛选";
            panel_筛选.Size = new Size(1193, 38);
            panel_筛选.TabIndex = 2;
            // 
            // label_更新时间
            // 
            label_更新时间.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_更新时间.AutoSize = true;
            label_更新时间.ForeColor = Color.Gray;
            label_更新时间.Location = new Point(1000, 10);
            label_更新时间.Name = "label_更新时间";
            label_更新时间.Size = new Size(68, 17);
            label_更新时间.TabIndex = 2;
            label_更新时间.Text = "更新时间: -";
            // 
            // textBox_搜索
            // 
            textBox_搜索.BackColor = Color.FromArgb(60, 60, 63);
            textBox_搜索.BorderStyle = BorderStyle.FixedSingle;
            textBox_搜索.ForeColor = Color.White;
            textBox_搜索.Location = new Point(473, 8);
            textBox_搜索.Name = "textBox_搜索";
            textBox_搜索.PlaceholderText = "阵容名称/标签/英雄名称/阵容描述...";
            textBox_搜索.Size = new Size(303, 23);
            textBox_搜索.TabIndex = 6;
            textBox_搜索.TextChanged += textBox_Search_TextChanged;
            // 
            // label_搜索
            // 
            label_搜索.AutoSize = true;
            label_搜索.ForeColor = Color.White;
            label_搜索.Location = new Point(428, 8);
            label_搜索.MinimumSize = new Size(35, 23);
            label_搜索.Name = "label_搜索";
            label_搜索.Size = new Size(35, 23);
            label_搜索.TabIndex = 5;
            label_搜索.Text = "搜索:";
            label_搜索.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // comboBox_排序方式
            // 
            comboBox_排序方式.BackColor = Color.FromArgb(60, 60, 63);
            comboBox_排序方式.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_排序方式.FlatStyle = FlatStyle.Flat;
            comboBox_排序方式.ForeColor = Color.White;
            comboBox_排序方式.FormattingEnabled = true;
            comboBox_排序方式.Items.AddRange(new object[] { "评级优先", "胜率", "前四率", "选取率", "平均名次" });
            comboBox_排序方式.Location = new Point(282, 8);
            comboBox_排序方式.Name = "comboBox_排序方式";
            comboBox_排序方式.Size = new Size(128, 25);
            comboBox_排序方式.TabIndex = 4;
            comboBox_排序方式.SelectedIndexChanged += comboBox_SortBy_SelectedIndexChanged;
            // 
            // label_排序方式
            // 
            label_排序方式.AutoSize = true;
            label_排序方式.ForeColor = Color.White;
            label_排序方式.Location = new Point(212, 8);
            label_排序方式.MinimumSize = new Size(59, 25);
            label_排序方式.Name = "label_排序方式";
            label_排序方式.Size = new Size(59, 25);
            label_排序方式.TabIndex = 3;
            label_排序方式.Text = "排序方式:";
            label_排序方式.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // comboBox_评级筛选
            // 
            comboBox_评级筛选.BackColor = Color.FromArgb(60, 60, 63);
            comboBox_评级筛选.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_评级筛选.FlatStyle = FlatStyle.Flat;
            comboBox_评级筛选.ForeColor = Color.White;
            comboBox_评级筛选.FormattingEnabled = true;
            comboBox_评级筛选.Items.AddRange(new object[] { "全部", "S", "A", "B", "C", "D" });
            comboBox_评级筛选.Location = new Point(85, 8);
            comboBox_评级筛选.Name = "comboBox_评级筛选";
            comboBox_评级筛选.Size = new Size(106, 25);
            comboBox_评级筛选.TabIndex = 1;
            comboBox_评级筛选.SelectedIndexChanged += comboBox_TierFilter_SelectedIndexChanged;
            // 
            // label_评级筛选
            // 
            label_评级筛选.AutoSize = true;
            label_评级筛选.ForeColor = Color.White;
            label_评级筛选.Location = new Point(15, 8);
            label_评级筛选.MinimumSize = new Size(59, 25);
            label_评级筛选.Name = "label_评级筛选";
            label_评级筛选.Size = new Size(59, 25);
            label_评级筛选.TabIndex = 0;
            label_评级筛选.Text = "评级筛选:";
            label_评级筛选.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel_按钮区
            // 
            panel_按钮区.BackColor = Color.FromArgb(45, 45, 48);
            panel_按钮区.Controls.Add(button_取消);
            panel_按钮区.Controls.Add(button_应用);
            panel_按钮区.Controls.Add(label_阵容信息);
            panel_按钮区.Dock = DockStyle.Bottom;
            panel_按钮区.Location = new Point(0, 593);
            panel_按钮区.Name = "panel_按钮区";
            panel_按钮区.Size = new Size(1193, 50);
            panel_按钮区.TabIndex = 1;
            // 
            // button_取消
            // 
            button_取消.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_取消.BackColor = Color.FromArgb(60, 60, 63);
            button_取消.FlatAppearance.BorderColor = Color.Gray;
            button_取消.FlatStyle = FlatStyle.Flat;
            button_取消.ForeColor = Color.White;
            button_取消.Location = new Point(1105, 10);
            button_取消.Name = "button_取消";
            button_取消.Size = new Size(80, 30);
            button_取消.TabIndex = 2;
            button_取消.Text = "取消";
            button_取消.UseVisualStyleBackColor = false;
            button_取消.Click += button_Cancel_Click;
            // 
            // button_应用
            // 
            button_应用.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_应用.BackColor = Color.FromArgb(60, 60, 63);
            button_应用.Enabled = false;
            button_应用.FlatAppearance.BorderColor = Color.Gray;
            button_应用.FlatStyle = FlatStyle.Flat;
            button_应用.ForeColor = Color.White;
            button_应用.Location = new Point(1015, 10);
            button_应用.Name = "button_应用";
            button_应用.Size = new Size(80, 30);
            button_应用.TabIndex = 1;
            button_应用.Text = "应用";
            button_应用.UseVisualStyleBackColor = false;
            button_应用.Click += button_Confirm_Click;
            // 
            // label_阵容信息
            // 
            label_阵容信息.AutoSize = true;
            label_阵容信息.ForeColor = Color.Silver;
            label_阵容信息.Location = new Point(15, 17);
            label_阵容信息.Name = "label_阵容信息";
            label_阵容信息.Size = new Size(176, 17);
            label_阵容信息.TabIndex = 0;
            label_阵容信息.Text = "请选择一个阵容导入到当前变阵";
            // 
            // panel_标题栏
            // 
            panel_标题栏.BackColor = Color.FromArgb(45, 45, 48);
            panel_标题栏.Controls.Add(label_标题);
            panel_标题栏.Controls.Add(button_最小化);
            panel_标题栏.Controls.Add(button_关闭);
            panel_标题栏.Dock = DockStyle.Top;
            panel_标题栏.Location = new Point(0, 0);
            panel_标题栏.Name = "panel_标题栏";
            panel_标题栏.Size = new Size(1193, 25);
            panel_标题栏.TabIndex = 3;
            // 
            // label_标题
            // 
            label_标题.AutoSize = true;
            label_标题.ForeColor = Color.White;
            label_标题.Location = new Point(4, 1);
            label_标题.MinimumSize = new Size(80, 23);
            label_标题.Name = "label_标题";
            label_标题.Size = new Size(80, 23);
            label_标题.TabIndex = 10;
            label_标题.Text = "推荐阵容";
            label_标题.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button_最小化
            // 
            button_最小化.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_最小化.FlatAppearance.BorderSize = 0;
            button_最小化.FlatStyle = FlatStyle.Flat;
            button_最小化.ForeColor = Color.White;
            button_最小化.Location = new Point(1146, 1);
            button_最小化.Margin = new Padding(0);
            button_最小化.Name = "button_最小化";
            button_最小化.Size = new Size(23, 23);
            button_最小化.TabIndex = 8;
            button_最小化.Text = "—";
            button_最小化.UseVisualStyleBackColor = true;
            button_最小化.Click += Button_Minimize_Click;
            // 
            // button_关闭
            // 
            button_关闭.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_关闭.FlatAppearance.BorderSize = 0;
            button_关闭.FlatStyle = FlatStyle.Flat;
            button_关闭.ForeColor = Color.White;
            button_关闭.Location = new Point(1170, 1);
            button_关闭.Margin = new Padding(0);
            button_关闭.Name = "button_关闭";
            button_关闭.Size = new Size(23, 23);
            button_关闭.TabIndex = 7;
            button_关闭.Text = "X";
            button_关闭.UseVisualStyleBackColor = true;
            button_关闭.Click += Button_Close_Click;
            // 
            // LineUpSelectForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(1200, 650);
            Controls.Add(panel_主背景);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "LineUpSelectForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "推荐阵容";
            panel_主背景.ResumeLayout(false);
            panel_副背景.ResumeLayout(false);
            panel_阵容区.ResumeLayout(false);
            panel_筛选.ResumeLayout(false);
            panel_筛选.PerformLayout();
            panel_按钮区.ResumeLayout(false);
            panel_按钮区.PerformLayout();
            panel_标题栏.ResumeLayout(false);
            panel_标题栏.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_主背景;
        private Panel panel_副背景;
        private Panel panel_阵容区;
        private FlowLayoutPanel flowLayoutPanel_阵容;
        private Panel panel_按钮区;
        private Button button_取消;
        private Button button_应用;
        private Label label_阵容信息;
        private Panel panel_筛选;
        private ComboBox comboBox_评级筛选;
        private Label label_评级筛选;
        private ComboBox comboBox_排序方式;
        private Label label_排序方式;
        private Label label_更新时间;
        private TextBox textBox_搜索;
        private Label label_搜索;
        private Panel panel_标题栏;
        private Label label_标题;
        private Button button_最小化;
        private Button button_关闭;
    }
}
