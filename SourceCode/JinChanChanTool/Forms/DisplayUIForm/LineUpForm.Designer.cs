using JinChanChanTool.DIYComponents;

namespace JinChanChanTool.Forms
{
    partial class LineUpForm
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
            flowLayoutPanel_阵容展示 = new CustomFlowLayoutPanel();
            button_清空 = new Button();
            button_保存 = new Button();
            comboBox_阵容选择 = new ComboBox();
            button_阵容推荐 = new Button();
            button_展开收起 = new Button();
            hexagonBoard = new HexagonBoard();
            benchPanel = new BenchPanel();
            flowLayoutPanel_装备散件展示 = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel_阵容展示.BackColor = Color.FromArgb(1, 1, 1);
            flowLayoutPanel_阵容展示.BorderWidth = 0;
            flowLayoutPanel_阵容展示.Location = new Point(2, 27);
            flowLayoutPanel_阵容展示.Margin = new Padding(0);
            flowLayoutPanel_阵容展示.Name = "flowLayoutPanel1";
            flowLayoutPanel_阵容展示.Padding = new Padding(2);
            flowLayoutPanel_阵容展示.Size = new Size(426, 64);
            flowLayoutPanel_阵容展示.TabIndex = 4;
            flowLayoutPanel_阵容展示.WrapContents = false;
            // 
            // button_清空
            // 
            button_清空.BackColor = Color.FromArgb(45, 45, 48);
            button_清空.FlatAppearance.BorderColor = Color.Gray;
            button_清空.FlatStyle = FlatStyle.Flat;
            button_清空.ForeColor = SystemColors.Control;
            button_清空.Location = new Point(224, 1);
            button_清空.Name = "button_清空";
            button_清空.Size = new Size(60, 25);
            button_清空.TabIndex = 9;
            button_清空.Text = "清空";
            button_清空.UseVisualStyleBackColor = false;
            button_清空.Click += button_清空_Click;
            // 
            // button_保存
            // 
            button_保存.BackColor = Color.FromArgb(45, 45, 48);
            button_保存.FlatAppearance.BorderColor = Color.Gray;
            button_保存.FlatStyle = FlatStyle.Flat;
            button_保存.ForeColor = SystemColors.Control;
            button_保存.Location = new Point(162, 1);
            button_保存.Name = "button_保存";
            button_保存.Size = new Size(60, 25);
            button_保存.TabIndex = 8;
            button_保存.Text = "保存";
            button_保存.UseVisualStyleBackColor = false;
            button_保存.Click += button_保存_Click;
            // 
            // comboBox_LineUpSelected
            // 
            comboBox_阵容选择.BackColor = Color.White;
            comboBox_阵容选择.ForeColor = Color.Black;
            comboBox_阵容选择.FormattingEnabled = true;
            comboBox_阵容选择.Location = new Point(2, 1);
            comboBox_阵容选择.Name = "comboBox_LineUpSelected";
            comboBox_阵容选择.Size = new Size(158, 25);
            comboBox_阵容选择.TabIndex = 7;
            // 
            // button_阵容推荐
            // 
            button_阵容推荐.BackColor = Color.FromArgb(45, 45, 48);
            button_阵容推荐.FlatAppearance.BorderColor = Color.Gray;
            button_阵容推荐.FlatStyle = FlatStyle.Flat;
            button_阵容推荐.ForeColor = SystemColors.Control;
            button_阵容推荐.Location = new Point(286, 1);
            button_阵容推荐.Name = "button_阵容推荐";
            button_阵容推荐.Size = new Size(80, 25);
            button_阵容推荐.TabIndex = 13;
            button_阵容推荐.Text = "阵容推荐";
            button_阵容推荐.UseVisualStyleBackColor = false;
            button_阵容推荐.Click += button_阵容推荐_Click;
            // 
            // button_展开收起
            // 
            button_展开收起.BackColor = Color.FromArgb(45, 45, 48);
            button_展开收起.FlatAppearance.BorderColor = Color.Gray;
            button_展开收起.FlatStyle = FlatStyle.Flat;
            button_展开收起.ForeColor = SystemColors.Control;
            button_展开收起.Location = new Point(368, 1);
            button_展开收起.Name = "button_展开收起";
            button_展开收起.Size = new Size(60, 25);
            button_展开收起.TabIndex = 14;
            button_展开收起.Text = "站位";
            button_展开收起.UseVisualStyleBackColor = false;
            button_展开收起.Click += button_展开收起_Click;
            // 
            // hexagonBoard
            // 
            hexagonBoard.BackColor = Color.FromArgb(1, 1, 1);
            hexagonBoard.Location = new Point(2, 93);
            hexagonBoard.Name = "hexagonBoard";
            hexagonBoard.Size = new Size(319, 150);
            hexagonBoard.TabIndex = 15;
            hexagonBoard.Visible = false;
            //
            // benchPanel
            //
            benchPanel.BackColor = Color.FromArgb(1, 1, 1);
            benchPanel.Location = new Point(2, 293);
            benchPanel.Name = "benchPanel";
            benchPanel.Size = new Size(319, 34);
            benchPanel.TabIndex = 16;
            benchPanel.Visible = false;
            // 
            // componentPanel
            // 
            flowLayoutPanel_装备散件展示.AutoScroll = true;
            flowLayoutPanel_装备散件展示.BackColor = Color.FromArgb(1, 1, 1);
            flowLayoutPanel_装备散件展示.Location = new Point(480, 93);
            flowLayoutPanel_装备散件展示.Name = "componentPanel";
            flowLayoutPanel_装备散件展示.Size = new Size(150, 250);
            flowLayoutPanel_装备散件展示.TabIndex = 17;
            flowLayoutPanel_装备散件展示.Visible = false;
            // 
            // LineUpForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(1, 1, 1);
            ClientSize = new Size(430, 95);
            Controls.Add(flowLayoutPanel_装备散件展示);
            Controls.Add(benchPanel);
            Controls.Add(hexagonBoard);
            Controls.Add(button_展开收起);
            Controls.Add(button_阵容推荐);
            Controls.Add(flowLayoutPanel_阵容展示);
            Controls.Add(button_清空);
            Controls.Add(button_保存);
            Controls.Add(comboBox_阵容选择);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LineUpForm";
            ShowInTaskbar = false;
            Text = "LineUpForm";
            TopMost = true;
            TransparencyKey = Color.FromArgb(1, 1, 1);
            Load += LineUpForm_Load;
            ResumeLayout(false);
        }

        #endregion
        public CustomFlowLayoutPanel flowLayoutPanel_阵容展示;
        private ComboBox comboBox_阵容选择;
        private Button button_清空;
        private Button button_保存;
        private Button button_阵容推荐;
        private Button button_展开收起;
        private HexagonBoard hexagonBoard;
        private BenchPanel benchPanel;
        private FlowLayoutPanel flowLayoutPanel_装备散件展示;
    }
}
