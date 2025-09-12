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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CorrectionEditorForm));
            dataGridView = new DataGridView();
            btnAdd = new Button();
            btnDelete = new Button();
            btnCancel = new Button();
            button1 = new Button();
            panel_DataGridView = new Panel();
            panel_Buttons = new Panel();
            panel_BackGround = new Panel();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            panel_DataGridView.SuspendLayout();
            panel_Buttons.SuspendLayout();
            panel_BackGround.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(5, 5);
            dataGridView.Margin = new Padding(0);
            dataGridView.Name = "dataGridView";
            dataGridView.Size = new Size(378, 541);
            dataGridView.TabIndex = 0;
            // 
            // btnAdd
            // 
            btnAdd.FlatAppearance.BorderColor = Color.LightGray;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Location = new Point(3, 36);
            btnAdd.Margin = new Padding(0);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 27);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "添加";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += BtnAdd_Click;
            // 
            // btnDelete
            // 
            btnDelete.FlatAppearance.BorderColor = Color.LightGray;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(3, 69);
            btnDelete.Margin = new Padding(0);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 27);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "删除";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += BtnDelete_Click;
            // 
            // btnCancel
            // 
            btnCancel.FlatAppearance.BorderColor = Color.LightGray;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(3, 496);
            btnCancel.Margin = new Padding(0);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 27);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "退出";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += BtnCancel_Click;
            // 
            // button1
            // 
            button1.FlatAppearance.BorderColor = Color.LightGray;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(3, 3);
            button1.Margin = new Padding(0);
            button1.Name = "button1";
            button1.Size = new Size(75, 27);
            button1.TabIndex = 6;
            button1.Text = "保存";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel_DataGridView
            // 
            panel_DataGridView.AutoSize = true;
            panel_DataGridView.Controls.Add(dataGridView);
            panel_DataGridView.Dock = DockStyle.Left;
            panel_DataGridView.Location = new Point(0, 0);
            panel_DataGridView.Margin = new Padding(0);
            panel_DataGridView.MinimumSize = new Size(388, 551);
            panel_DataGridView.Name = "panel_DataGridView";
            panel_DataGridView.Padding = new Padding(5);
            panel_DataGridView.Size = new Size(388, 551);
            panel_DataGridView.TabIndex = 7;
            // 
            // panel_Buttons
            // 
            panel_Buttons.AutoSize = true;
            panel_Buttons.Controls.Add(btnCancel);
            panel_Buttons.Controls.Add(button1);
            panel_Buttons.Controls.Add(btnAdd);
            panel_Buttons.Controls.Add(btnDelete);
            panel_Buttons.Dock = DockStyle.Right;
            panel_Buttons.Location = new Point(390, 0);
            panel_Buttons.Margin = new Padding(0);
            panel_Buttons.Name = "panel_Buttons";
            panel_Buttons.Padding = new Padding(5);
            panel_Buttons.Size = new Size(83, 551);
            panel_Buttons.TabIndex = 8;
            // 
            // panel_BackGround
            // 
            panel_BackGround.AutoSize = true;
            panel_BackGround.Controls.Add(panel_DataGridView);
            panel_BackGround.Controls.Add(panel_Buttons);
            panel_BackGround.Dock = DockStyle.Fill;
            panel_BackGround.Location = new Point(0, 0);
            panel_BackGround.Margin = new Padding(0);
            panel_BackGround.Name = "panel_BackGround";
            panel_BackGround.Size = new Size(473, 551);
            panel_BackGround.TabIndex = 9;
            // 
            // CorrectionEditorForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(473, 551);
            Controls.Add(panel_BackGround);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CorrectionEditorForm";
            ShowIcon = false;
            Text = "OCR结果纠正列表编辑器";
            Load += CorrectionEditorForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            panel_DataGridView.ResumeLayout(false);
            panel_Buttons.ResumeLayout(false);
            panel_BackGround.ResumeLayout(false);
            panel_BackGround.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnCancel;
        private Button button1;
        private Panel panel_DataGridView;
        private Panel panel_Buttons;
        private Panel panel_BackGround;
    }
}