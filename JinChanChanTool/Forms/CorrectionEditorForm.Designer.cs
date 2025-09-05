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
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(12, 12);
            dataGridView.Name = "dataGridView";
            dataGridView.Size = new Size(360, 515);
            dataGridView.TabIndex = 0;
            // 
            // btnAdd
            // 
            btnAdd.FlatAppearance.BorderColor = Color.LightGray;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Location = new Point(378, 45);
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
            btnDelete.Location = new Point(378, 78);
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
            btnCancel.Location = new Point(378, 496);
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
            button1.Location = new Point(378, 12);
            button1.Name = "button1";
            button1.Size = new Size(75, 27);
            button1.TabIndex = 6;
            button1.Text = "保存";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // CorrectionEditorForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(459, 527);
            ControlBox = false;
            Controls.Add(button1);
            Controls.Add(btnCancel);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(dataGridView);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CorrectionEditorForm";
            ShowIcon = false;
            Text = "OCR结果纠正列表编辑器";
            Load += CorrectionEditorForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnCancel;
        private Button button1;
    }
}