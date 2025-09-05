namespace JinChanChanTool
{
    partial class HelpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
            textBox1 = new TextBox();
            button1 = new Button();
            label1 = new Label();
            textBox2 = new TextBox();
            label2 = new Label();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(87, 50);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(273, 216);
            textBox1.TabIndex = 0;
            textBox1.TabStop = false;
            // 
            // button1
            // 
            button1.FlatAppearance.BorderColor = Color.DarkGray;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(184, 272);
            button1.Name = "button1";
            button1.Size = new Size(75, 28);
            button1.TabIndex = 1;
            button1.TabStop = false;
            button1.Text = "发送反馈";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(56, 23);
            label1.TabIndex = 2;
            label1.Text = "发送者：";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(87, 9);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(273, 23);
            textBox2.TabIndex = 3;
            textBox2.TabStop = false;
            // 
            // label2
            // 
            label2.Location = new Point(12, 50);
            label2.Name = "label2";
            label2.Size = new Size(69, 23);
            label2.TabIndex = 4;
            label2.Text = "反馈内容：";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // HelpForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(368, 309);
            Controls.Add(label2);
            Controls.Add(textBox2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(textBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "HelpForm";
            Text = "反馈";
            TopMost = true;
            Load += HelpForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button button1;
        private Label label1;
        private TextBox textBox2;
        private Label label2;
    }
}