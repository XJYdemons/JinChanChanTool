namespace JinChanChanTool
{
    partial class CorrectionEditorForm
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
            dataGridView_结果纠正列表编辑器 = new DataGridView();
            button_添加 = new Button();
            button_删除 = new Button();
            button_取消 = new Button();
            button_保存 = new Button();
            panel_DataGridView = new Panel();
            panel_按钮 = new Panel();
            panel_副背景 = new Panel();
            panel_标题栏 = new Panel();
            label_标题 = new Label();
            panel_主背景 = new Panel();
            ((System.ComponentModel.ISupportInitialize)dataGridView_结果纠正列表编辑器).BeginInit();
            panel_DataGridView.SuspendLayout();
            panel_按钮.SuspendLayout();
            panel_副背景.SuspendLayout();
            panel_标题栏.SuspendLayout();
            panel_主背景.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView_结果纠正列表编辑器
            // 
            dataGridView_结果纠正列表编辑器.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_结果纠正列表编辑器.Location = new Point(5, 5);
            dataGridView_结果纠正列表编辑器.Margin = new Padding(0);
            dataGridView_结果纠正列表编辑器.Name = "dataGridView_结果纠正列表编辑器";
            dataGridView_结果纠正列表编辑器.Size = new Size(374, 533);
            dataGridView_结果纠正列表编辑器.TabIndex = 0;
            // 
            // button_添加
            // 
            button_添加.FlatAppearance.BorderColor = Color.LightGray;
            button_添加.FlatStyle = FlatStyle.Flat;
            button_添加.Location = new Point(5, 38);
            button_添加.Margin = new Padding(0);
            button_添加.Name = "button_添加";
            button_添加.Size = new Size(113, 27);
            button_添加.TabIndex = 1;
            button_添加.Text = "添加";
            button_添加.UseVisualStyleBackColor = true;
            button_添加.Click += BtnAdd_Click;
            // 
            // button_删除
            // 
            button_删除.FlatAppearance.BorderColor = Color.LightGray;
            button_删除.FlatStyle = FlatStyle.Flat;
            button_删除.Location = new Point(5, 71);
            button_删除.Margin = new Padding(0);
            button_删除.Name = "button_删除";
            button_删除.Size = new Size(113, 27);
            button_删除.TabIndex = 2;
            button_删除.Text = "删除";
            button_删除.UseVisualStyleBackColor = true;
            button_删除.Click += BtnDelete_Click;
            // 
            // button_取消
            // 
            button_取消.FlatAppearance.BorderColor = Color.LightGray;
            button_取消.FlatStyle = FlatStyle.Flat;
            button_取消.Location = new Point(5, 511);
            button_取消.Margin = new Padding(0);
            button_取消.Name = "button_取消";
            button_取消.Size = new Size(113, 27);
            button_取消.TabIndex = 5;
            button_取消.Text = "退出";
            button_取消.UseVisualStyleBackColor = true;
            button_取消.Click += BtnCancel_Click;
            // 
            // button_保存
            // 
            button_保存.FlatAppearance.BorderColor = Color.LightGray;
            button_保存.FlatStyle = FlatStyle.Flat;
            button_保存.Location = new Point(5, 5);
            button_保存.Margin = new Padding(0);
            button_保存.Name = "button_保存";
            button_保存.Size = new Size(113, 27);
            button_保存.TabIndex = 6;
            button_保存.Text = "保存";
            button_保存.UseVisualStyleBackColor = true;
            button_保存.Click += button1_Click;
            // 
            // panel_DataGridView
            // 
            panel_DataGridView.BackColor = Color.White;
            panel_DataGridView.Controls.Add(dataGridView_结果纠正列表编辑器);
            panel_DataGridView.Location = new Point(0, 26);
            panel_DataGridView.Margin = new Padding(0);
            panel_DataGridView.Name = "panel_DataGridView";
            panel_DataGridView.Padding = new Padding(5);
            panel_DataGridView.Size = new Size(384, 543);
            panel_DataGridView.TabIndex = 7;
            // 
            // panel_按钮
            // 
            panel_按钮.BackColor = Color.White;
            panel_按钮.Controls.Add(button_取消);
            panel_按钮.Controls.Add(button_保存);
            panel_按钮.Controls.Add(button_添加);
            panel_按钮.Controls.Add(button_删除);
            panel_按钮.Location = new Point(386, 26);
            panel_按钮.Margin = new Padding(0);
            panel_按钮.Name = "panel_按钮";
            panel_按钮.Padding = new Padding(5);
            panel_按钮.Size = new Size(123, 543);
            panel_按钮.TabIndex = 8;
            // 
            // panel_副背景
            // 
            panel_副背景.BackColor = Color.White;
            panel_副背景.Controls.Add(panel_标题栏);
            panel_副背景.Controls.Add(panel_DataGridView);
            panel_副背景.Controls.Add(panel_按钮);
            panel_副背景.Dock = DockStyle.Fill;
            panel_副背景.Location = new Point(3, 3);
            panel_副背景.Margin = new Padding(0);
            panel_副背景.MinimumSize = new Size(484, 553);
            panel_副背景.Name = "panel_副背景";
            panel_副背景.Size = new Size(509, 572);
            panel_副背景.TabIndex = 9;
            // 
            // panel_标题栏
            // 
            panel_标题栏.Controls.Add(label_标题);
            panel_标题栏.Location = new Point(0, 0);
            panel_标题栏.Name = "panel_标题栏";
            panel_标题栏.Size = new Size(509, 25);
            panel_标题栏.TabIndex = 16;
            // 
            // label_标题
            // 
            label_标题.AutoSize = true;
            label_标题.Location = new Point(4, 1);
            label_标题.MinimumSize = new Size(80, 23);
            label_标题.Name = "label_标题";
            label_标题.Size = new Size(118, 23);
            label_标题.TabIndex = 10;
            label_标题.Text = "OCR纠正列表编辑器";
            label_标题.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_主背景
            // 
            panel_主背景.BackColor = Color.FromArgb(250, 250, 250);
            panel_主背景.Controls.Add(panel_副背景);
            panel_主背景.Dock = DockStyle.Fill;
            panel_主背景.Location = new Point(0, 0);
            panel_主背景.Margin = new Padding(0);
            panel_主背景.Name = "panel_主背景";
            panel_主背景.Padding = new Padding(3, 3, 4, 4);
            panel_主背景.Size = new Size(516, 579);
            panel_主背景.TabIndex = 10;
            // 
            // CorrectionEditorForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(516, 579);
            Controls.Add(panel_主背景);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(484, 553);
            Name = "CorrectionEditorForm";
            ShowIcon = false;
            Text = "OCR结果纠正列表编辑器";
            Load += CorrectionEditorForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView_结果纠正列表编辑器).EndInit();
            panel_DataGridView.ResumeLayout(false);
            panel_按钮.ResumeLayout(false);
            panel_副背景.ResumeLayout(false);
            panel_标题栏.ResumeLayout(false);
            panel_标题栏.PerformLayout();
            panel_主背景.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView_结果纠正列表编辑器;
        private Button button_添加;
        private Button button_删除;
        private Button button_取消;
        private Button button_保存;
        private Panel panel_DataGridView;
        private Panel panel_按钮;
        private Panel panel_副背景;
        private Panel panel_标题栏;
        private Label label_标题;
        private Panel panel_主背景;
    }
}