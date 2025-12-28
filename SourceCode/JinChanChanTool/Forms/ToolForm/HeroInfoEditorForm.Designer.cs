namespace JinChanChanTool.Forms
{
    partial class HeroInfoEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeroInfoEditorForm));
            dataGridView_英雄数据编辑器 = new DataGridView();
            button_添加 = new Button();
            button_删除 = new Button();
            button_退出 = new Button();
            button_上移 = new Button();
            button_下移 = new Button();
            comboBox_赛季文件选择器 = new ComboBox();
            button_保存 = new Button();
            button_打开目录 = new Button();
            panel_BackGround = new Panel();
            panel3 = new Panel();
            label5 = new Label();
            panel_Buttons = new Panel();
            panel_DataGridView = new Panel();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)dataGridView_英雄数据编辑器).BeginInit();
            panel_BackGround.SuspendLayout();
            panel3.SuspendLayout();
            panel_Buttons.SuspendLayout();
            panel_DataGridView.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView_英雄数据编辑器
            // 
            dataGridView_英雄数据编辑器.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_英雄数据编辑器.Location = new Point(5, 35);
            dataGridView_英雄数据编辑器.Name = "dataGridView_英雄数据编辑器";
            dataGridView_英雄数据编辑器.Size = new Size(711, 395);
            dataGridView_英雄数据编辑器.TabIndex = 0;
            dataGridView_英雄数据编辑器.CellContentClick += dataGridView1_CellContentClick;
            // 
            // button_添加
            // 
            button_添加.FlatStyle = FlatStyle.Flat;
            button_添加.Location = new Point(5, 67);
            button_添加.Name = "button_添加";
            button_添加.Size = new Size(75, 25);
            button_添加.TabIndex = 1;
            button_添加.Text = "添加";
            button_添加.UseVisualStyleBackColor = true;
            button_添加.Click += addButton_Click;
            // 
            // button_删除
            // 
            button_删除.FlatStyle = FlatStyle.Flat;
            button_删除.Location = new Point(5, 98);
            button_删除.Name = "button_删除";
            button_删除.Size = new Size(75, 25);
            button_删除.TabIndex = 2;
            button_删除.Text = "删除";
            button_删除.UseVisualStyleBackColor = true;
            button_删除.Click += deleltButton_Click;
            // 
            // button_退出
            // 
            button_退出.FlatStyle = FlatStyle.Flat;
            button_退出.Location = new Point(5, 405);
            button_退出.Name = "button_退出";
            button_退出.Size = new Size(75, 25);
            button_退出.TabIndex = 4;
            button_退出.Text = "退出";
            button_退出.UseVisualStyleBackColor = true;
            button_退出.Click += cancelButton_Click;
            // 
            // button_上移
            // 
            button_上移.FlatStyle = FlatStyle.Flat;
            button_上移.Location = new Point(5, 129);
            button_上移.Name = "button_上移";
            button_上移.Size = new Size(75, 25);
            button_上移.TabIndex = 5;
            button_上移.Text = "上移";
            button_上移.UseVisualStyleBackColor = true;
            button_上移.Click += upButton_Click;
            // 
            // button_下移
            // 
            button_下移.FlatStyle = FlatStyle.Flat;
            button_下移.Location = new Point(5, 160);
            button_下移.Name = "button_下移";
            button_下移.Size = new Size(75, 25);
            button_下移.TabIndex = 6;
            button_下移.Text = "下移";
            button_下移.UseVisualStyleBackColor = true;
            button_下移.Click += downButton_Click;
            // 
            // comboBox_赛季文件选择器
            // 
            comboBox_赛季文件选择器.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_赛季文件选择器.FormattingEnabled = true;
            comboBox_赛季文件选择器.Location = new Point(5, 5);
            comboBox_赛季文件选择器.Name = "comboBox_赛季文件选择器";
            comboBox_赛季文件选择器.Size = new Size(711, 25);
            comboBox_赛季文件选择器.TabIndex = 7;
            comboBox_赛季文件选择器.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // button_保存
            // 
            button_保存.FlatStyle = FlatStyle.Flat;
            button_保存.Location = new Point(5, 36);
            button_保存.Name = "button_保存";
            button_保存.Size = new Size(75, 25);
            button_保存.TabIndex = 8;
            button_保存.Text = "保存";
            button_保存.UseVisualStyleBackColor = true;
            button_保存.Click += button1_Click;
            // 
            // button_打开目录
            // 
            button_打开目录.FlatStyle = FlatStyle.Flat;
            button_打开目录.Location = new Point(5, 5);
            button_打开目录.Name = "button_打开目录";
            button_打开目录.Size = new Size(75, 25);
            button_打开目录.TabIndex = 12;
            button_打开目录.Text = "打开目录";
            button_打开目录.UseVisualStyleBackColor = true;
            button_打开目录.Click += button5_Click;
            // 
            // panel_BackGround
            // 
            panel_BackGround.BackColor = Color.White;
            panel_BackGround.Controls.Add(panel3);
            panel_BackGround.Controls.Add(panel_Buttons);
            panel_BackGround.Controls.Add(panel_DataGridView);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(3, 3);
            panel_BackGround.Margin = new Padding(0);
            panel_BackGround.MinimumSize = new Size(821, 445);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Size = new Size(822, 465);
            panel_BackGround.TabIndex = 13;
            // 
            // panel3
            // 
            panel3.Controls.Add(label5);
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(811, 25);
            panel3.TabIndex = 15;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(4, 1);
            label5.MinimumSize = new Size(80, 23);
            label5.Name = "label5";
            label5.Size = new Size(116, 23);
            label5.TabIndex = 10;
            label5.Text = "英雄配置文件编辑器";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_Buttons
            // 
            panel_Buttons.AutoScroll = true;
            panel_Buttons.BackColor = Color.White;
            panel_Buttons.Controls.Add(button_下移);
            panel_Buttons.Controls.Add(button_退出);
            panel_Buttons.Controls.Add(button_打开目录);
            panel_Buttons.Controls.Add(button_删除);
            panel_Buttons.Controls.Add(button_保存);
            panel_Buttons.Controls.Add(button_上移);
            panel_Buttons.Controls.Add(button_添加);
            panel_Buttons.Location = new Point(726, 26);
            panel_Buttons.MinimumSize = new Size(85, 435);
            panel_Buttons.Name = "panel_Buttons";
            panel_Buttons.Size = new Size(85, 435);
            panel_Buttons.TabIndex = 14;
            // 
            // panel_DataGridView
            // 
            panel_DataGridView.AutoScroll = true;
            panel_DataGridView.BackColor = Color.White;
            panel_DataGridView.Controls.Add(comboBox_赛季文件选择器);
            panel_DataGridView.Controls.Add(dataGridView_英雄数据编辑器);
            panel_DataGridView.Location = new Point(0, 26);
            panel_DataGridView.MinimumSize = new Size(721, 435);
            panel_DataGridView.Name = "panel_DataGridView";
            panel_DataGridView.Size = new Size(721, 435);
            panel_DataGridView.TabIndex = 13;
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
            panel1.Size = new Size(829, 472);
            panel1.TabIndex = 14;
            // 
            // HeroInfoEditorForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(829, 472);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(821, 445);
            Name = "HeroInfoEditorForm";
            Text = "英雄数据文件编辑器";
            Load += HeroInfoEditorForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView_英雄数据编辑器).EndInit();
            panel_BackGround.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel_Buttons.ResumeLayout(false);
            panel_DataGridView.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView_英雄数据编辑器;
        private Button button_添加;
        private Button button_删除;
        private Button button_退出;
        private Button button_上移;
        private Button button_下移;
        private ComboBox comboBox_赛季文件选择器;
        private Button button_保存;
        private Button button_打开目录;
        private Panel panel_BackGround;
        private Panel panel_Buttons;
        private Panel panel_DataGridView;
        private Panel panel1;
        private Panel panel3;
        private Label label5;
    }
}