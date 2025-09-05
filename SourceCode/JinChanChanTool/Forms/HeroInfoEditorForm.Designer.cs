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
            dataGridView1 = new DataGridView();
            addButton = new Button();
            deleltButton = new Button();
            cancelButton = new Button();
            upButton = new Button();
            downButton = new Button();
            comboBox1 = new ComboBox();
            button1 = new Button();
            button5 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 37);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(660, 395);
            dataGridView1.TabIndex = 0;
            // 
            // addButton
            // 
            addButton.FlatStyle = FlatStyle.Flat;
            addButton.Location = new Point(676, 68);
            addButton.Name = "addButton";
            addButton.Size = new Size(75, 25);
            addButton.TabIndex = 1;
            addButton.Text = "添加";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += addButton_Click;
            // 
            // deleltButton
            // 
            deleltButton.FlatStyle = FlatStyle.Flat;
            deleltButton.Location = new Point(676, 99);
            deleltButton.Name = "deleltButton";
            deleltButton.Size = new Size(75, 25);
            deleltButton.TabIndex = 2;
            deleltButton.Text = "删除";
            deleltButton.UseVisualStyleBackColor = true;
            deleltButton.Click += deleltButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Location = new Point(676, 407);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 25);
            cancelButton.TabIndex = 4;
            cancelButton.Text = "退出";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // upButton
            // 
            upButton.FlatStyle = FlatStyle.Flat;
            upButton.Location = new Point(676, 130);
            upButton.Name = "upButton";
            upButton.Size = new Size(75, 25);
            upButton.TabIndex = 5;
            upButton.Text = "上移";
            upButton.UseVisualStyleBackColor = true;
            upButton.Click += upButton_Click;
            // 
            // downButton
            // 
            downButton.FlatStyle = FlatStyle.Flat;
            downButton.Location = new Point(676, 161);
            downButton.Name = "downButton";
            downButton.Size = new Size(75, 25);
            downButton.TabIndex = 6;
            downButton.Text = "下移";
            downButton.UseVisualStyleBackColor = true;
            downButton.Click += downButton_Click;
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(12, 6);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(660, 25);
            comboBox1.TabIndex = 7;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // button1
            // 
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(676, 37);
            button1.Name = "button1";
            button1.Size = new Size(75, 25);
            button1.TabIndex = 8;
            button1.Text = "保存";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button5
            // 
            button5.FlatStyle = FlatStyle.Flat;
            button5.Location = new Point(676, 6);
            button5.Name = "button5";
            button5.Size = new Size(75, 25);
            button5.TabIndex = 12;
            button5.Text = "打开目录";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // HeroInfoEditorForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(755, 438);
            ControlBox = false;
            Controls.Add(button5);
            Controls.Add(button1);
            Controls.Add(comboBox1);
            Controls.Add(downButton);
            Controls.Add(upButton);
            Controls.Add(cancelButton);
            Controls.Add(deleltButton);
            Controls.Add(addButton);
            Controls.Add(dataGridView1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "HeroInfoEditorForm";
            Text = "英雄数据文件编辑器";
            Load += HeroInfoEditorForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private Button addButton;
        private Button deleltButton;
        private Button cancelButton;
        private Button upButton;
        private Button downButton;
        private ComboBox comboBox1;
        private Button button1;
        private Button button5;
    }
}