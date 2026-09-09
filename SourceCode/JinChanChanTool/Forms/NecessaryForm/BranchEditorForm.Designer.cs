namespace JinChanChanTool;

partial class BranchEditorForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel panel_主背景;
    private Panel panel_副背景;
    private Panel panel_客户区;
    private Panel panel_标题栏;
    private Label label_标题;
    private Button button_最小化;
    private Button button_关闭;
    private Label label_名称;
    private TextBox textBox_名称;
    private Label label_描述;
    private TextBox textBox_描述;
    private Panel panel_按钮;
    private Button button_确定;
    private Button button_取消;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        panel_主背景 = new Panel();
        panel_副背景 = new Panel();
        panel_客户区 = new Panel();
        panel_标题栏 = new Panel();
        label_标题 = new Label();
        button_最小化 = new Button();
        button_关闭 = new Button();
        label_名称 = new Label();
        textBox_名称 = new TextBox();
        label_描述 = new Label();
        textBox_描述 = new TextBox();
        panel_按钮 = new Panel();
        button_确定 = new Button();
        button_取消 = new Button();
        panel_主背景.SuspendLayout();
        panel_副背景.SuspendLayout();
        panel_客户区.SuspendLayout();
        panel_标题栏.SuspendLayout();
        panel_按钮.SuspendLayout();
        SuspendLayout();
        // panel_主背景
        panel_主背景.BackColor = Color.FromArgb(250, 250, 250);
        panel_主背景.Controls.Add(panel_副背景);
        panel_主背景.Dock = DockStyle.Fill;
        panel_主背景.Padding = new Padding(3, 3, 4, 4);
        panel_主背景.Size = new Size(380, 300);
        // panel_副背景
        panel_副背景.BackColor = Color.White;
        panel_副背景.Controls.Add(panel_客户区);
        panel_副背景.Controls.Add(panel_标题栏);
        panel_副背景.Dock = DockStyle.Fill;
        panel_副背景.Location = new Point(3, 3);
        panel_副背景.Size = new Size(373, 293);
        // panel_标题栏
        panel_标题栏.BackColor = Color.White;
        panel_标题栏.Controls.Add(label_标题);
        panel_标题栏.Controls.Add(button_最小化);
        panel_标题栏.Controls.Add(button_关闭);
        panel_标题栏.Location = new Point(0, 0);
        panel_标题栏.Size = new Size(373, 25);
        // label_标题
        label_标题.AutoSize = true;
        label_标题.Location = new Point(4, 1);
        label_标题.MinimumSize = new Size(80, 23);
        label_标题.Size = new Size(80, 23);
        label_标题.Text = "新建分支";
        label_标题.TextAlign = ContentAlignment.MiddleLeft;
        // button_最小化
        button_最小化.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button_最小化.FlatAppearance.BorderSize = 0;
        button_最小化.FlatStyle = FlatStyle.Flat;
        button_最小化.Location = new Point(326, 1);
        button_最小化.Size = new Size(23, 23);
        button_最小化.Text = "—";
        button_最小化.UseVisualStyleBackColor = true;
        button_最小化.Click += button_最小化_Click;
        // button_关闭
        button_关闭.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button_关闭.FlatAppearance.BorderSize = 0;
        button_关闭.FlatStyle = FlatStyle.Flat;
        button_关闭.Location = new Point(350, 1);
        button_关闭.Size = new Size(23, 23);
        button_关闭.Text = "X";
        button_关闭.UseVisualStyleBackColor = true;
        button_关闭.Click += button_关闭_Click;
        // panel_客户区
        panel_客户区.BackColor = Color.White;
        panel_客户区.Controls.Add(label_名称);
        panel_客户区.Controls.Add(textBox_名称);
        panel_客户区.Controls.Add(label_描述);
        panel_客户区.Controls.Add(textBox_描述);
        panel_客户区.Controls.Add(panel_按钮);
        panel_客户区.Location = new Point(0, 27);
        panel_客户区.Size = new Size(373, 266);
        // input controls
        label_名称.AutoSize = true;
        label_名称.Location = new Point(18, 11);
        label_名称.Text = "分支名称";
        textBox_名称.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBox_名称.Location = new Point(18, 35);
        textBox_名称.MaxLength = 7;
        textBox_名称.Size = new Size(337, 23);
        label_描述.AutoSize = true;
        label_描述.Location = new Point(18, 67);
        label_描述.Text = "描述";
        textBox_描述.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        textBox_描述.Location = new Point(18, 91);
        textBox_描述.Multiline = true;
        textBox_描述.MaxLength = 60;
        textBox_描述.ScrollBars = ScrollBars.Vertical;
        textBox_描述.Size = new Size(337, 125);
        // panel_按钮
        panel_按钮.BackColor = Color.White;
        panel_按钮.Controls.Add(button_取消);
        panel_按钮.Controls.Add(button_确定);
        panel_按钮.Location = new Point(0, 224);
        panel_按钮.Size = new Size(373, 45);
        button_确定.FlatAppearance.BorderColor = Color.LightGray;
        button_确定.FlatStyle = FlatStyle.Flat;
        button_确定.Location = new Point(207, 8);
        button_确定.Size = new Size(65, 27);
        button_确定.Text = "确定";
        button_确定.UseVisualStyleBackColor = true;
        button_确定.Click += button_确定_Click;
        button_取消.FlatAppearance.BorderColor = Color.LightGray;
        button_取消.FlatStyle = FlatStyle.Flat;
        button_取消.Location = new Point(283, 8);
        button_取消.Size = new Size(65, 27);
        button_取消.Text = "取消";
        button_取消.UseVisualStyleBackColor = true;
        button_取消.DialogResult = DialogResult.Cancel;
        // BranchEditorForm
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.White;
        ClientSize = new Size(380, 300);
        Controls.Add(panel_主背景);
        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "BranchEditorForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "新建分支";
        TopMost = true;
        panel_按钮.ResumeLayout(false);
        panel_标题栏.ResumeLayout(false);
        panel_标题栏.PerformLayout();
        panel_客户区.ResumeLayout(false);
        panel_客户区.PerformLayout();
        panel_副背景.ResumeLayout(false);
        panel_主背景.ResumeLayout(false);
        ResumeLayout(false);
    }
}
