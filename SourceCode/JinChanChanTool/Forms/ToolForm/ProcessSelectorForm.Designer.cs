namespace JinChanChanTool.Forms
{
    partial class ProcessSelectorForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessSelectorForm));
            listView_Processes = new ListView();
            imageList_ProcessIcons = new ImageList(components);
            button_选定此进程 = new Button();
            button_刷新 = new Button();
            panel_副背景 = new Panel();
            panel_标题栏 = new Panel();
            label_标题 = new Label();
            button_最小化 = new Button();
            button_关闭 = new Button();
            panel_主背景 = new Panel();
            panel_副背景.SuspendLayout();
            panel_标题栏.SuspendLayout();
            panel_主背景.SuspendLayout();
            SuspendLayout();
            // 
            // listView_Processes
            // 
            listView_Processes.BorderStyle = BorderStyle.FixedSingle;
            listView_Processes.FullRowSelect = true;
            listView_Processes.HeaderStyle = ColumnHeaderStyle.None;
            listView_Processes.Location = new Point(5, 26);
            listView_Processes.Margin = new Padding(0);
            listView_Processes.MultiSelect = false;
            listView_Processes.Name = "listView_Processes";
            listView_Processes.Size = new Size(718, 274);
            listView_Processes.SmallImageList = imageList_ProcessIcons;
            listView_Processes.TabIndex = 0;
            listView_Processes.UseCompatibleStateImageBehavior = false;
            listView_Processes.View = View.Details;
            // 
            // imageList_ProcessIcons
            // 
            imageList_ProcessIcons.ColorDepth = ColorDepth.Depth32Bit;
            imageList_ProcessIcons.ImageSize = new Size(16, 16);
            imageList_ProcessIcons.TransparentColor = Color.Transparent;
            // 
            // button_Select
            // 
            button_选定此进程.FlatStyle = FlatStyle.Flat;
            button_选定此进程.Location = new Point(5, 336);
            button_选定此进程.Margin = new Padding(0);
            button_选定此进程.Name = "button_Select";
            button_选定此进程.Size = new Size(718, 28);
            button_选定此进程.TabIndex = 1;
            button_选定此进程.Text = "选定此进程";
            button_选定此进程.UseVisualStyleBackColor = true;
            // 
            // button_Refresh
            // 
            button_刷新.FlatStyle = FlatStyle.Flat;
            button_刷新.Location = new Point(5, 304);
            button_刷新.Margin = new Padding(0);
            button_刷新.Name = "button_Refresh";
            button_刷新.Size = new Size(718, 28);
            button_刷新.TabIndex = 2;
            button_刷新.Text = "刷新列表";
            button_刷新.UseVisualStyleBackColor = true;
            // 
            // panel_BackGround
            // 
            panel_副背景.BackColor = Color.White;
            panel_副背景.Controls.Add(panel_标题栏);
            panel_副背景.Controls.Add(button_选定此进程);
            panel_副背景.Controls.Add(listView_Processes);
            panel_副背景.Controls.Add(button_刷新);
            panel_副背景.Dock = DockStyle.Fill;
            panel_副背景.Location = new Point(3, 3);
            panel_副背景.Margin = new Padding(0);
            panel_副背景.Name = "panel_BackGround";
            panel_副背景.Size = new Size(728, 368);
            panel_副背景.TabIndex = 3;
            // 
            // panel3
            // 
            panel_标题栏.BackColor = Color.White;
            panel_标题栏.Controls.Add(label_标题);
            panel_标题栏.Controls.Add(button_最小化);
            panel_标题栏.Controls.Add(button_关闭);
            panel_标题栏.Location = new Point(0, 0);
            panel_标题栏.Name = "panel3";
            panel_标题栏.Size = new Size(728, 25);
            panel_标题栏.TabIndex = 212;
            // 
            // label29
            // 
            label_标题.AutoSize = true;
            label_标题.Location = new Point(4, 1);
            label_标题.MinimumSize = new Size(80, 23);
            label_标题.Name = "label29";
            label_标题.Size = new Size(104, 23);
            label_标题.TabIndex = 10;
            label_标题.Text = "选择游戏窗口进程";
            label_标题.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            button_最小化.FlatAppearance.BorderSize = 0;
            button_最小化.FlatStyle = FlatStyle.Flat;
            button_最小化.Location = new Point(678, 1);
            button_最小化.Margin = new Padding(0);
            button_最小化.Name = "button2";
            button_最小化.Size = new Size(23, 23);
            button_最小化.TabIndex = 8;
            button_最小化.Text = "—";
            button_最小化.UseVisualStyleBackColor = true;
            button_最小化.Click += button_最小化_Click;
            // 
            // button3
            // 
            button_关闭.FlatAppearance.BorderSize = 0;
            button_关闭.FlatStyle = FlatStyle.Flat;
            button_关闭.Location = new Point(702, 1);
            button_关闭.Margin = new Padding(0);
            button_关闭.Name = "button3";
            button_关闭.Size = new Size(23, 23);
            button_关闭.TabIndex = 7;
            button_关闭.Text = "X";
            button_关闭.UseVisualStyleBackColor = true;
            button_关闭.Click += button_关闭_Click;
            // 
            // panel1
            // 
            panel_主背景.BackColor = Color.FromArgb(250, 250, 250);
            panel_主背景.Controls.Add(panel_副背景);
            panel_主背景.Dock = DockStyle.Fill;
            panel_主背景.Location = new Point(0, 0);
            panel_主背景.Margin = new Padding(0);
            panel_主背景.Name = "panel1";
            panel_主背景.Padding = new Padding(3, 3, 4, 4);
            panel_主背景.Size = new Size(735, 375);
            panel_主背景.TabIndex = 4;
            // 
            // ProcessSelectorForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(735, 375);
            Controls.Add(panel_主背景);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ProcessSelectorForm";
            Text = "ProcessSelectorForm";
            TopMost = true;
            panel_副背景.ResumeLayout(false);
            panel_标题栏.ResumeLayout(false);
            panel_标题栏.PerformLayout();
            panel_主背景.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView listView_Processes;
        private ImageList imageList_ProcessIcons;
        private Button button_选定此进程;
        private Button button_刷新;
        private Panel panel_副背景;
        private Panel panel_标题栏;
        private Label label_标题;
        private Button button_最小化;
        private Button button_关闭;
        private Panel panel_主背景;
    }
}