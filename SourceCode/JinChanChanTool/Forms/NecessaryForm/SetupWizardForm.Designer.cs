using JinChanChanTool.DIYComponents;

namespace JinChanChanTool
{
    partial class SetupWizardForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupWizardForm));
            panel1 = new Panel();
            panel2 = new Panel();
            panel5 = new Panel();
            panel_8GPU环境配置 = new Panel();
            groupBoxEnvironment = new GroupBox();
            label_GPU状态 = new Label();
            label_cuDNN状态 = new Label();
            label_CUDA状态 = new Label();
            button_检测GPU环境 = new RoundedButton();
            groupBoxConfig = new GroupBox();
            labelCudaVersion = new Label();
            comboBoxCudaVersion = new ComboBox();
            button_一键配置 = new RoundedButton();
            richTextBoxLog = new RichTextBox();
            label24 = new Label();
            label27 = new Label();
            panel_9配置完成 = new Panel();
            richTextBox1 = new RichTextBox();
            label_完成说明 = new Label();
            label17 = new Label();
            panel_7选择OCR推理设备 = new Panel();
            label_CPU推理 = new Label();
            capsuleSwitch_CPU推理 = new CapsuleSwitch();
            label_CPU说明 = new Label();
            label_GPU推理 = new Label();
            capsuleSwitch_GPU推理 = new CapsuleSwitch();
            label_GPU说明 = new Label();
            label19 = new Label();
            label22 = new Label();
            panel_6选择刷新商店方式 = new Panel();
            label_鼠标刷新 = new Label();
            capsuleSwitch_鼠标刷新 = new CapsuleSwitch();
            label_按键刷新 = new Label();
            capsuleSwitch_按键刷新 = new CapsuleSwitch();
            label_刷新按键 = new Label();
            textBox_刷新按键 = new TextBox();
            label25 = new Label();
            label26 = new Label();
            Panel_5选择拿牌方式 = new Panel();
            label_鼠标拿牌 = new Label();
            capsuleSwitch_鼠标拿牌 = new CapsuleSwitch();
            label_按键拿牌 = new Label();
            capsuleSwitch_按键拿牌 = new CapsuleSwitch();
            label_拿牌按键1 = new Label();
            textBox_拿牌按键1 = new TextBox();
            label_拿牌按键2 = new Label();
            textBox_拿牌按键2 = new TextBox();
            label_拿牌按键3 = new Label();
            textBox_拿牌按键3 = new TextBox();
            label_拿牌按键4 = new Label();
            textBox_拿牌按键4 = new TextBox();
            label_拿牌按键5 = new Label();
            textBox_拿牌按键5 = new TextBox();
            label20 = new Label();
            label21 = new Label();
            panel_4手动设置坐标 = new Panel();
            label_设置高亮提示坐标 = new Label();
            roundedButton_设置高亮提示坐标 = new RoundedButton();
            label_设置刷新按钮坐标 = new Label();
            roundedButton_设置刷新按钮坐标 = new RoundedButton();
            comboBox_选择显示器 = new ComboBox();
            label15 = new Label();
            label_设置英雄名称坐标 = new Label();
            label14 = new Label();
            roundedButton_设置英雄名称坐标 = new RoundedButton();
            label16 = new Label();
            panel_3自动设置坐标 = new Panel();
            label_进程状态 = new Label();
            label10 = new Label();
            选择游戏窗口进程 = new RoundedButton();
            label12 = new Label();
            label13 = new Label();
            panel_2坐标设置模式 = new Panel();
            label7 = new Label();
            radioButton_手动设置坐标 = new RadioButton();
            label6 = new Label();
            radioButton_自动设置坐标 = new RadioButton();
            label8 = new Label();
            label9 = new Label();
            panel_1欢迎页 = new Panel();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            panel4 = new Panel();
            button_下一步 = new RoundedButton();
            button_上一步 = new RoundedButton();
            button_跳过向导 = new RoundedButton();
            button_完成 = new RoundedButton();
            panel3 = new Panel();
            label5 = new Label();
            pictureBox1 = new PictureBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel5.SuspendLayout();
            panel_8GPU环境配置.SuspendLayout();
            groupBoxEnvironment.SuspendLayout();
            groupBoxConfig.SuspendLayout();
            panel_9配置完成.SuspendLayout();
            panel_7选择OCR推理设备.SuspendLayout();
            panel_6选择刷新商店方式.SuspendLayout();
            Panel_5选择拿牌方式.SuspendLayout();
            panel_4手动设置坐标.SuspendLayout();
            panel_3自动设置坐标.SuspendLayout();
            panel_2坐标设置模式.SuspendLayout();
            panel_1欢迎页.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(250, 250, 250);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(3, 3, 4, 4);
            panel1.Size = new Size(751, 573);
            panel1.TabIndex = 6;
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.Controls.Add(panel5);
            panel2.Controls.Add(panel4);
            panel2.Controls.Add(panel3);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 3);
            panel2.Margin = new Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new Size(744, 566);
            panel2.TabIndex = 6;
            // 
            // panel5
            // 
            panel5.Controls.Add(panel_8GPU环境配置);
            panel5.Controls.Add(panel_9配置完成);
            panel5.Controls.Add(panel_7选择OCR推理设备);
            panel5.Controls.Add(panel_6选择刷新商店方式);
            panel5.Controls.Add(Panel_5选择拿牌方式);
            panel5.Controls.Add(panel_4手动设置坐标);
            panel5.Controls.Add(panel_3自动设置坐标);
            panel5.Controls.Add(panel_2坐标设置模式);
            panel5.Controls.Add(panel_1欢迎页);
            panel5.Location = new Point(3, 32);
            panel5.Name = "panel5";
            panel5.Size = new Size(738, 460);
            panel5.TabIndex = 8;
            // 
            // panel_8GPU环境配置
            // 
            panel_8GPU环境配置.Controls.Add(groupBoxEnvironment);
            panel_8GPU环境配置.Controls.Add(groupBoxConfig);
            panel_8GPU环境配置.Controls.Add(richTextBoxLog);
            panel_8GPU环境配置.Controls.Add(label24);
            panel_8GPU环境配置.Controls.Add(label27);
            panel_8GPU环境配置.Dock = DockStyle.Fill;
            panel_8GPU环境配置.Location = new Point(0, 0);
            panel_8GPU环境配置.Name = "panel_8GPU环境配置";
            panel_8GPU环境配置.Size = new Size(738, 460);
            panel_8GPU环境配置.TabIndex = 32;
            // 
            // groupBoxEnvironment
            // 
            groupBoxEnvironment.Controls.Add(label_GPU状态);
            groupBoxEnvironment.Controls.Add(label_cuDNN状态);
            groupBoxEnvironment.Controls.Add(label_CUDA状态);
            groupBoxEnvironment.Controls.Add(button_检测GPU环境);
            groupBoxEnvironment.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            groupBoxEnvironment.Location = new Point(13, 65);
            groupBoxEnvironment.Name = "groupBoxEnvironment";
            groupBoxEnvironment.Size = new Size(720, 110);
            groupBoxEnvironment.TabIndex = 12;
            groupBoxEnvironment.TabStop = false;
            groupBoxEnvironment.Text = "环境检测";
            // 
            // label_GPU状态
            // 
            label_GPU状态.AutoSize = true;
            label_GPU状态.Font = new Font("Microsoft YaHei UI", 9F);
            label_GPU状态.Location = new Point(6, 19);
            label_GPU状态.Name = "label_GPU状态";
            label_GPU状态.Size = new Size(75, 17);
            label_GPU状态.TabIndex = 7;
            label_GPU状态.Text = "显卡: 未检测";
            // 
            // label_cuDNN状态
            // 
            label_cuDNN状态.AutoSize = true;
            label_cuDNN状态.Font = new Font("Microsoft YaHei UI", 9F);
            label_cuDNN状态.Location = new Point(6, 79);
            label_cuDNN状态.Name = "label_cuDNN状态";
            label_cuDNN状态.Size = new Size(93, 17);
            label_cuDNN状态.TabIndex = 9;
            label_cuDNN状态.Text = "cuDNN: 未检测";
            // 
            // label_CUDA状态
            // 
            label_CUDA状态.AutoSize = true;
            label_CUDA状态.Font = new Font("Microsoft YaHei UI", 9F);
            label_CUDA状态.Location = new Point(6, 49);
            label_CUDA状态.Name = "label_CUDA状态";
            label_CUDA状态.Size = new Size(85, 17);
            label_CUDA状态.TabIndex = 8;
            label_CUDA状态.Text = "CUDA: 未检测";
            // 
            // button_检测GPU环境
            // 
            button_检测GPU环境.BorderColor = Color.FromArgb(200, 200, 200);
            button_检测GPU环境.BorderWidth = 1;
            button_检测GPU环境.ButtonColor = Color.FromArgb(24, 96, 251);
            button_检测GPU环境.CornerRadius = 5;
            button_检测GPU环境.DisabledColor = Color.FromArgb(160, 160, 160);
            button_检测GPU环境.HoverColor = Color.FromArgb(50, 120, 255);
            button_检测GPU环境.Location = new Point(555, 39);
            button_检测GPU环境.Name = "button_检测GPU环境";
            button_检测GPU环境.PressedColor = Color.FromArgb(20, 80, 220);
            button_检测GPU环境.Size = new Size(150, 35);
            button_检测GPU环境.TabIndex = 10;
            button_检测GPU环境.Text = "检测GPU环境";
            button_检测GPU环境.TextColor = Color.White;
            button_检测GPU环境.TextFont = new Font("微软雅黑", 10F);
            button_检测GPU环境.Click += button_检测GPU环境_Click;
            // 
            // groupBoxConfig
            // 
            groupBoxConfig.Controls.Add(labelCudaVersion);
            groupBoxConfig.Controls.Add(comboBoxCudaVersion);
            groupBoxConfig.Controls.Add(button_一键配置);
            groupBoxConfig.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            groupBoxConfig.Location = new Point(13, 181);
            groupBoxConfig.Name = "groupBoxConfig";
            groupBoxConfig.Size = new Size(720, 64);
            groupBoxConfig.TabIndex = 13;
            groupBoxConfig.TabStop = false;
            groupBoxConfig.Text = "安装配置";
            // 
            // labelCudaVersion
            // 
            labelCudaVersion.AutoSize = true;
            labelCudaVersion.Font = new Font("Microsoft YaHei UI", 9F);
            labelCudaVersion.Location = new Point(15, 30);
            labelCudaVersion.Name = "labelCudaVersion";
            labelCudaVersion.Size = new Size(69, 17);
            labelCudaVersion.TabIndex = 0;
            labelCudaVersion.Text = "CUDA版本:";
            // 
            // comboBoxCudaVersion
            // 
            comboBoxCudaVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCudaVersion.Font = new Font("Microsoft YaHei UI", 9F);
            comboBoxCudaVersion.FormattingEnabled = true;
            comboBoxCudaVersion.Location = new Point(100, 27);
            comboBoxCudaVersion.Name = "comboBoxCudaVersion";
            comboBoxCudaVersion.Size = new Size(200, 25);
            comboBoxCudaVersion.TabIndex = 1;
            // 
            // button_一键配置
            // 
            button_一键配置.BorderColor = Color.FromArgb(200, 200, 200);
            button_一键配置.BorderWidth = 1;
            button_一键配置.ButtonColor = Color.FromArgb(40, 167, 69);
            button_一键配置.CornerRadius = 5;
            button_一键配置.DisabledColor = Color.FromArgb(160, 160, 160);
            button_一键配置.HoverColor = Color.FromArgb(60, 187, 89);
            button_一键配置.Location = new Point(555, 22);
            button_一键配置.Name = "button_一键配置";
            button_一键配置.PressedColor = Color.FromArgb(30, 147, 59);
            button_一键配置.Size = new Size(150, 35);
            button_一键配置.TabIndex = 11;
            button_一键配置.Text = "一键配置";
            button_一键配置.TextColor = Color.White;
            button_一键配置.TextFont = new Font("微软雅黑", 10F);
            button_一键配置.Click += button_一键配置_Click;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBoxLog.BackColor = Color.FromArgb(30, 30, 30);
            richTextBoxLog.Font = new Font("Consolas", 9F);
            richTextBoxLog.ForeColor = Color.LightGray;
            richTextBoxLog.Location = new Point(13, 251);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.ReadOnly = true;
            richTextBoxLog.Size = new Size(720, 206);
            richTextBoxLog.TabIndex = 2;
            richTextBoxLog.Text = "";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.ForeColor = Color.Gray;
            label24.Location = new Point(13, 41);
            label24.Name = "label24";
            label24.Size = new Size(301, 17);
            label24.TabIndex = 3;
            label24.Text = "一键配置CUDA、cuDNN和运行时环境，启用GPU加速";
            label24.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label27.Location = new Point(10, 10);
            label27.Name = "label27";
            label27.Size = new Size(142, 28);
            label27.TabIndex = 0;
            label27.Text = "GPU环境配置";
            label27.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_9配置完成
            // 
            panel_9配置完成.Controls.Add(richTextBox1);
            panel_9配置完成.Controls.Add(label_完成说明);
            panel_9配置完成.Controls.Add(label17);
            panel_9配置完成.Dock = DockStyle.Fill;
            panel_9配置完成.Location = new Point(0, 0);
            panel_9配置完成.Name = "panel_9配置完成";
            panel_9配置完成.Size = new Size(738, 460);
            panel_9配置完成.TabIndex = 33;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.Black;
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Location = new Point(16, 81);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(717, 376);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "";
            // 
            // label_完成说明
            // 
            label_完成说明.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_完成说明.Location = new Point(13, 48);
            label_完成说明.Name = "label_完成说明";
            label_完成说明.Size = new Size(580, 30);
            label_完成说明.TabIndex = 3;
            label_完成说明.Text = "恭喜!您已完成所有配置,点击\"完成\"按钮开始使用JinChanChanTool";
            label_完成说明.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label17.Location = new Point(10, 10);
            label17.Name = "label17";
            label17.Size = new Size(96, 28);
            label17.TabIndex = 0;
            label17.Text = "配置完成";
            label17.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_7选择OCR推理设备
            // 
            panel_7选择OCR推理设备.Controls.Add(label_CPU推理);
            panel_7选择OCR推理设备.Controls.Add(capsuleSwitch_CPU推理);
            panel_7选择OCR推理设备.Controls.Add(label_CPU说明);
            panel_7选择OCR推理设备.Controls.Add(label_GPU推理);
            panel_7选择OCR推理设备.Controls.Add(capsuleSwitch_GPU推理);
            panel_7选择OCR推理设备.Controls.Add(label_GPU说明);
            panel_7选择OCR推理设备.Controls.Add(label19);
            panel_7选择OCR推理设备.Controls.Add(label22);
            panel_7选择OCR推理设备.Dock = DockStyle.Fill;
            panel_7选择OCR推理设备.Location = new Point(0, 0);
            panel_7选择OCR推理设备.Name = "panel_7选择OCR推理设备";
            panel_7选择OCR推理设备.Size = new Size(738, 460);
            panel_7选择OCR推理设备.TabIndex = 31;
            // 
            // label_CPU推理
            // 
            label_CPU推理.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_CPU推理.Location = new Point(16, 74);
            label_CPU推理.Name = "label_CPU推理";
            label_CPU推理.Size = new Size(100, 25);
            label_CPU推理.TabIndex = 8;
            label_CPU推理.Text = "CPU 推理";
            label_CPU推理.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_CPU推理
            // 
            capsuleSwitch_CPU推理.Location = new Point(126, 76);
            capsuleSwitch_CPU推理.Name = "capsuleSwitch_CPU推理";
            capsuleSwitch_CPU推理.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_CPU推理.OnColor = Color.FromArgb(24, 96, 251);
            capsuleSwitch_CPU推理.ShowText = false;
            capsuleSwitch_CPU推理.Size = new Size(50, 20);
            capsuleSwitch_CPU推理.TabIndex = 9;
            capsuleSwitch_CPU推理.Text = "capsuleSwitch_CPU推理";
            capsuleSwitch_CPU推理.TextColor = Color.White;
            capsuleSwitch_CPU推理.ThumbColor = Color.White;
            capsuleSwitch_CPU推理.IsOnChanged += capsuleSwitch_CPU推理_IsOnChanged;
            // 
            // label_CPU说明
            // 
            label_CPU说明.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_CPU说明.ForeColor = Color.FromArgb(100, 100, 100);
            label_CPU说明.Location = new Point(36, 104);
            label_CPU说明.Name = "label_CPU说明";
            label_CPU说明.Size = new Size(540, 40);
            label_CPU说明.TabIndex = 10;
            label_CPU说明.Text = "使用CPU进行OCR识别,速度中等但兼容性好,推荐大部分用户使用";
            // 
            // label_GPU推理
            // 
            label_GPU推理.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_GPU推理.Location = new Point(16, 164);
            label_GPU推理.Name = "label_GPU推理";
            label_GPU推理.Size = new Size(100, 25);
            label_GPU推理.TabIndex = 11;
            label_GPU推理.Text = "GPU 推理";
            label_GPU推理.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_GPU推理
            // 
            capsuleSwitch_GPU推理.Location = new Point(126, 166);
            capsuleSwitch_GPU推理.Name = "capsuleSwitch_GPU推理";
            capsuleSwitch_GPU推理.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_GPU推理.OnColor = Color.FromArgb(24, 96, 251);
            capsuleSwitch_GPU推理.ShowText = false;
            capsuleSwitch_GPU推理.Size = new Size(50, 20);
            capsuleSwitch_GPU推理.TabIndex = 12;
            capsuleSwitch_GPU推理.Text = "capsuleSwitch_GPU推理";
            capsuleSwitch_GPU推理.TextColor = Color.White;
            capsuleSwitch_GPU推理.ThumbColor = Color.White;
            capsuleSwitch_GPU推理.IsOnChanged += capsuleSwitch_GPU推理_IsOnChanged;
            // 
            // label_GPU说明
            // 
            label_GPU说明.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_GPU说明.ForeColor = Color.FromArgb(100, 100, 100);
            label_GPU说明.Location = new Point(36, 194);
            label_GPU说明.Name = "label_GPU说明";
            label_GPU说明.Size = new Size(540, 40);
            label_GPU说明.TabIndex = 13;
            label_GPU说明.Text = "使用GPU进行OCR识别,速度极快但需要NVIDIA显卡支持,高级用户可选择";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.ForeColor = Color.Gray;
            label19.Location = new Point(13, 41);
            label19.Name = "label19";
            label19.Size = new Size(190, 17);
            label19.TabIndex = 3;
            label19.Text = "选择用于OCR字符识别的计算设备";
            label19.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label22.Location = new Point(10, 10);
            label22.Name = "label22";
            label22.Size = new Size(184, 28);
            label22.TabIndex = 0;
            label22.Text = "选择OCR推理设备";
            label22.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_6选择刷新商店方式
            // 
            panel_6选择刷新商店方式.Controls.Add(label_鼠标刷新);
            panel_6选择刷新商店方式.Controls.Add(capsuleSwitch_鼠标刷新);
            panel_6选择刷新商店方式.Controls.Add(label_按键刷新);
            panel_6选择刷新商店方式.Controls.Add(capsuleSwitch_按键刷新);
            panel_6选择刷新商店方式.Controls.Add(label_刷新按键);
            panel_6选择刷新商店方式.Controls.Add(textBox_刷新按键);
            panel_6选择刷新商店方式.Controls.Add(label25);
            panel_6选择刷新商店方式.Controls.Add(label26);
            panel_6选择刷新商店方式.Dock = DockStyle.Fill;
            panel_6选择刷新商店方式.Location = new Point(0, 0);
            panel_6选择刷新商店方式.Name = "panel_6选择刷新商店方式";
            panel_6选择刷新商店方式.Size = new Size(738, 460);
            panel_6选择刷新商店方式.TabIndex = 30;
            // 
            // label_鼠标刷新
            // 
            label_鼠标刷新.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_鼠标刷新.Location = new Point(16, 71);
            label_鼠标刷新.Name = "label_鼠标刷新";
            label_鼠标刷新.Size = new Size(134, 25);
            label_鼠标刷新.TabIndex = 8;
            label_鼠标刷新.Text = "鼠标模拟刷新商店";
            label_鼠标刷新.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_鼠标刷新
            // 
            capsuleSwitch_鼠标刷新.Location = new Point(156, 73);
            capsuleSwitch_鼠标刷新.Name = "capsuleSwitch_鼠标刷新";
            capsuleSwitch_鼠标刷新.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_鼠标刷新.OnColor = Color.FromArgb(24, 96, 251);
            capsuleSwitch_鼠标刷新.ShowText = false;
            capsuleSwitch_鼠标刷新.Size = new Size(50, 20);
            capsuleSwitch_鼠标刷新.TabIndex = 9;
            capsuleSwitch_鼠标刷新.Text = "capsuleSwitch_鼠标刷新";
            capsuleSwitch_鼠标刷新.TextColor = Color.White;
            capsuleSwitch_鼠标刷新.ThumbColor = Color.White;
            capsuleSwitch_鼠标刷新.IsOnChanged += capsuleSwitch_鼠标刷新_IsOnChanged;
            // 
            // label_按键刷新
            // 
            label_按键刷新.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_按键刷新.Location = new Point(16, 116);
            label_按键刷新.Name = "label_按键刷新";
            label_按键刷新.Size = new Size(134, 25);
            label_按键刷新.TabIndex = 10;
            label_按键刷新.Text = "按键模拟刷新商店";
            label_按键刷新.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_按键刷新
            // 
            capsuleSwitch_按键刷新.Location = new Point(156, 118);
            capsuleSwitch_按键刷新.Name = "capsuleSwitch_按键刷新";
            capsuleSwitch_按键刷新.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_按键刷新.OnColor = Color.FromArgb(24, 96, 251);
            capsuleSwitch_按键刷新.ShowText = false;
            capsuleSwitch_按键刷新.Size = new Size(50, 20);
            capsuleSwitch_按键刷新.TabIndex = 11;
            capsuleSwitch_按键刷新.Text = "capsuleSwitch_按键刷新";
            capsuleSwitch_按键刷新.TextColor = Color.White;
            capsuleSwitch_按键刷新.ThumbColor = Color.White;
            capsuleSwitch_按键刷新.IsOnChanged += capsuleSwitch_按键刷新_IsOnChanged;
            // 
            // label_刷新按键
            // 
            label_刷新按键.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_刷新按键.Location = new Point(23, 147);
            label_刷新按键.Name = "label_刷新按键";
            label_刷新按键.Size = new Size(80, 25);
            label_刷新按键.TabIndex = 12;
            label_刷新按键.Text = "刷新商店按键";
            label_刷新按键.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox_刷新按键
            // 
            textBox_刷新按键.Enabled = false;
            textBox_刷新按键.Location = new Point(23, 172);
            textBox_刷新按键.Name = "textBox_刷新按键";
            textBox_刷新按键.Size = new Size(80, 23);
            textBox_刷新按键.TabIndex = 13;
            textBox_刷新按键.TabStop = false;
            textBox_刷新按键.Enter += TextBox_Enter;
            textBox_刷新按键.KeyDown += textBox_刷新按键_KeyDown;
            textBox_刷新按键.Leave += TextBox_Leave;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.ForeColor = Color.Gray;
            label25.Location = new Point(13, 41);
            label25.Name = "label25";
            label25.Size = new Size(260, 17);
            label25.TabIndex = 3;
            label25.Text = "选择使用模拟鼠标还是模拟键盘按键来刷新商店";
            label25.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label26.Location = new Point(10, 10);
            label26.Name = "label26";
            label26.Size = new Size(180, 28);
            label26.TabIndex = 0;
            label26.Text = "选择刷新商店方式";
            label26.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Panel_5选择拿牌方式
            // 
            Panel_5选择拿牌方式.Controls.Add(label_鼠标拿牌);
            Panel_5选择拿牌方式.Controls.Add(capsuleSwitch_鼠标拿牌);
            Panel_5选择拿牌方式.Controls.Add(label_按键拿牌);
            Panel_5选择拿牌方式.Controls.Add(capsuleSwitch_按键拿牌);
            Panel_5选择拿牌方式.Controls.Add(label_拿牌按键1);
            Panel_5选择拿牌方式.Controls.Add(textBox_拿牌按键1);
            Panel_5选择拿牌方式.Controls.Add(label_拿牌按键2);
            Panel_5选择拿牌方式.Controls.Add(textBox_拿牌按键2);
            Panel_5选择拿牌方式.Controls.Add(label_拿牌按键3);
            Panel_5选择拿牌方式.Controls.Add(textBox_拿牌按键3);
            Panel_5选择拿牌方式.Controls.Add(label_拿牌按键4);
            Panel_5选择拿牌方式.Controls.Add(textBox_拿牌按键4);
            Panel_5选择拿牌方式.Controls.Add(label_拿牌按键5);
            Panel_5选择拿牌方式.Controls.Add(textBox_拿牌按键5);
            Panel_5选择拿牌方式.Controls.Add(label20);
            Panel_5选择拿牌方式.Controls.Add(label21);
            Panel_5选择拿牌方式.Dock = DockStyle.Fill;
            Panel_5选择拿牌方式.Location = new Point(0, 0);
            Panel_5选择拿牌方式.Name = "Panel_5选择拿牌方式";
            Panel_5选择拿牌方式.Size = new Size(738, 460);
            Panel_5选择拿牌方式.TabIndex = 13;
            // 
            // label_鼠标拿牌
            // 
            label_鼠标拿牌.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_鼠标拿牌.Location = new Point(13, 99);
            label_鼠标拿牌.Name = "label_鼠标拿牌";
            label_鼠标拿牌.Size = new Size(100, 25);
            label_鼠标拿牌.TabIndex = 16;
            label_鼠标拿牌.Text = "模拟鼠标拿牌";
            label_鼠标拿牌.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_鼠标拿牌
            // 
            capsuleSwitch_鼠标拿牌.Location = new Point(123, 101);
            capsuleSwitch_鼠标拿牌.Name = "capsuleSwitch_鼠标拿牌";
            capsuleSwitch_鼠标拿牌.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_鼠标拿牌.OnColor = Color.FromArgb(24, 96, 251);
            capsuleSwitch_鼠标拿牌.ShowText = false;
            capsuleSwitch_鼠标拿牌.Size = new Size(50, 20);
            capsuleSwitch_鼠标拿牌.TabIndex = 17;
            capsuleSwitch_鼠标拿牌.Text = "capsuleSwitch_鼠标拿牌";
            capsuleSwitch_鼠标拿牌.TextColor = Color.White;
            capsuleSwitch_鼠标拿牌.ThumbColor = Color.White;
            capsuleSwitch_鼠标拿牌.IsOnChanged += capsuleSwitch_鼠标拿牌_IsOnChanged;
            // 
            // label_按键拿牌
            // 
            label_按键拿牌.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_按键拿牌.Location = new Point(13, 129);
            label_按键拿牌.Name = "label_按键拿牌";
            label_按键拿牌.Size = new Size(100, 25);
            label_按键拿牌.TabIndex = 18;
            label_按键拿牌.Text = "模拟按键拿牌";
            label_按键拿牌.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // capsuleSwitch_按键拿牌
            // 
            capsuleSwitch_按键拿牌.Location = new Point(123, 131);
            capsuleSwitch_按键拿牌.Name = "capsuleSwitch_按键拿牌";
            capsuleSwitch_按键拿牌.OffColor = Color.FromArgb(189, 189, 189);
            capsuleSwitch_按键拿牌.OnColor = Color.FromArgb(24, 96, 251);
            capsuleSwitch_按键拿牌.ShowText = false;
            capsuleSwitch_按键拿牌.Size = new Size(50, 20);
            capsuleSwitch_按键拿牌.TabIndex = 19;
            capsuleSwitch_按键拿牌.Text = "capsuleSwitch_按键拿牌";
            capsuleSwitch_按键拿牌.TextColor = Color.White;
            capsuleSwitch_按键拿牌.ThumbColor = Color.White;
            capsuleSwitch_按键拿牌.IsOnChanged += capsuleSwitch_按键拿牌_IsOnChanged;
            // 
            // label_拿牌按键1
            // 
            label_拿牌按键1.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_拿牌按键1.Location = new Point(16, 159);
            label_拿牌按键1.Name = "label_拿牌按键1";
            label_拿牌按键1.Size = new Size(80, 25);
            label_拿牌按键1.TabIndex = 20;
            label_拿牌按键1.Text = "拿牌按键1";
            label_拿牌按键1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox_拿牌按键1
            // 
            textBox_拿牌按键1.Enabled = false;
            textBox_拿牌按键1.Location = new Point(16, 185);
            textBox_拿牌按键1.Name = "textBox_拿牌按键1";
            textBox_拿牌按键1.Size = new Size(60, 23);
            textBox_拿牌按键1.TabIndex = 21;
            textBox_拿牌按键1.TabStop = false;
            textBox_拿牌按键1.Enter += TextBox_Enter;
            textBox_拿牌按键1.KeyDown += textBox_拿牌按键1_KeyDown;
            textBox_拿牌按键1.Leave += TextBox_Leave;
            // 
            // label_拿牌按键2
            // 
            label_拿牌按键2.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_拿牌按键2.Location = new Point(114, 159);
            label_拿牌按键2.Name = "label_拿牌按键2";
            label_拿牌按键2.Size = new Size(80, 25);
            label_拿牌按键2.TabIndex = 22;
            label_拿牌按键2.Text = "拿牌按键2";
            label_拿牌按键2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox_拿牌按键2
            // 
            textBox_拿牌按键2.Enabled = false;
            textBox_拿牌按键2.Location = new Point(114, 185);
            textBox_拿牌按键2.Name = "textBox_拿牌按键2";
            textBox_拿牌按键2.Size = new Size(60, 23);
            textBox_拿牌按键2.TabIndex = 23;
            textBox_拿牌按键2.TabStop = false;
            textBox_拿牌按键2.Enter += TextBox_Enter;
            textBox_拿牌按键2.KeyDown += textBox_拿牌按键2_KeyDown;
            textBox_拿牌按键2.Leave += TextBox_Leave;
            // 
            // label_拿牌按键3
            // 
            label_拿牌按键3.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_拿牌按键3.Location = new Point(216, 159);
            label_拿牌按键3.Name = "label_拿牌按键3";
            label_拿牌按键3.Size = new Size(80, 25);
            label_拿牌按键3.TabIndex = 24;
            label_拿牌按键3.Text = "拿牌按键3";
            label_拿牌按键3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox_拿牌按键3
            // 
            textBox_拿牌按键3.Enabled = false;
            textBox_拿牌按键3.Location = new Point(216, 185);
            textBox_拿牌按键3.Name = "textBox_拿牌按键3";
            textBox_拿牌按键3.Size = new Size(60, 23);
            textBox_拿牌按键3.TabIndex = 25;
            textBox_拿牌按键3.TabStop = false;
            textBox_拿牌按键3.Enter += TextBox_Enter;
            textBox_拿牌按键3.KeyDown += textBox_拿牌按键3_KeyDown;
            textBox_拿牌按键3.Leave += TextBox_Leave;
            // 
            // label_拿牌按键4
            // 
            label_拿牌按键4.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_拿牌按键4.Location = new Point(320, 159);
            label_拿牌按键4.Name = "label_拿牌按键4";
            label_拿牌按键4.Size = new Size(80, 25);
            label_拿牌按键4.TabIndex = 26;
            label_拿牌按键4.Text = "拿牌按键4";
            label_拿牌按键4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox_拿牌按键4
            // 
            textBox_拿牌按键4.Enabled = false;
            textBox_拿牌按键4.Location = new Point(320, 185);
            textBox_拿牌按键4.Name = "textBox_拿牌按键4";
            textBox_拿牌按键4.Size = new Size(60, 23);
            textBox_拿牌按键4.TabIndex = 27;
            textBox_拿牌按键4.TabStop = false;
            textBox_拿牌按键4.Enter += TextBox_Enter;
            textBox_拿牌按键4.KeyDown += textBox_拿牌按键4_KeyDown;
            textBox_拿牌按键4.Leave += TextBox_Leave;
            // 
            // label_拿牌按键5
            // 
            label_拿牌按键5.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label_拿牌按键5.Location = new Point(421, 159);
            label_拿牌按键5.Name = "label_拿牌按键5";
            label_拿牌按键5.Size = new Size(80, 25);
            label_拿牌按键5.TabIndex = 28;
            label_拿牌按键5.Text = "拿牌按键5";
            label_拿牌按键5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox_拿牌按键5
            // 
            textBox_拿牌按键5.Enabled = false;
            textBox_拿牌按键5.Location = new Point(421, 185);
            textBox_拿牌按键5.Name = "textBox_拿牌按键5";
            textBox_拿牌按键5.Size = new Size(60, 23);
            textBox_拿牌按键5.TabIndex = 29;
            textBox_拿牌按键5.TabStop = false;
            textBox_拿牌按键5.Enter += TextBox_Enter;
            textBox_拿牌按键5.KeyDown += textBox_拿牌按键5_KeyDown;
            textBox_拿牌按键5.Leave += TextBox_Leave;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.ForeColor = Color.Gray;
            label20.Location = new Point(13, 41);
            label20.Name = "label20";
            label20.Size = new Size(248, 51);
            label20.TabIndex = 3;
            label20.Text = "选择使用模拟鼠标还是模拟键盘按键购买英雄\r\n云顶之弈只能使用鼠标购买英雄\r\n金铲铲模拟器支持鼠标或键盘按钮购买英雄";
            label20.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label21.Location = new Point(10, 10);
            label21.Name = "label21";
            label21.Size = new Size(138, 28);
            label21.TabIndex = 0;
            label21.Text = "选择拿牌方式";
            label21.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_4手动设置坐标
            // 
            panel_4手动设置坐标.Controls.Add(label_设置高亮提示坐标);
            panel_4手动设置坐标.Controls.Add(roundedButton_设置高亮提示坐标);
            panel_4手动设置坐标.Controls.Add(label_设置刷新按钮坐标);
            panel_4手动设置坐标.Controls.Add(roundedButton_设置刷新按钮坐标);
            panel_4手动设置坐标.Controls.Add(comboBox_选择显示器);
            panel_4手动设置坐标.Controls.Add(label15);
            panel_4手动设置坐标.Controls.Add(label_设置英雄名称坐标);
            panel_4手动设置坐标.Controls.Add(label14);
            panel_4手动设置坐标.Controls.Add(roundedButton_设置英雄名称坐标);
            panel_4手动设置坐标.Controls.Add(label16);
            panel_4手动设置坐标.Dock = DockStyle.Fill;
            panel_4手动设置坐标.Location = new Point(0, 0);
            panel_4手动设置坐标.Name = "panel_4手动设置坐标";
            panel_4手动设置坐标.Size = new Size(738, 460);
            panel_4手动设置坐标.TabIndex = 12;
            // 
            // label_设置高亮提示坐标
            // 
            label_设置高亮提示坐标.ForeColor = Color.Red;
            label_设置高亮提示坐标.Location = new Point(149, 200);
            label_设置高亮提示坐标.Name = "label_设置高亮提示坐标";
            label_设置高亮提示坐标.Size = new Size(68, 28);
            label_设置高亮提示坐标.TabIndex = 10;
            label_设置高亮提示坐标.Text = "未设置";
            label_设置高亮提示坐标.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // roundedButton_设置高亮提示坐标
            // 
            roundedButton_设置高亮提示坐标.BorderColor = Color.FromArgb(200, 200, 200);
            roundedButton_设置高亮提示坐标.BorderWidth = 1;
            roundedButton_设置高亮提示坐标.ButtonColor = Color.FromArgb(255, 255, 255);
            roundedButton_设置高亮提示坐标.CornerRadius = 3;
            roundedButton_设置高亮提示坐标.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_设置高亮提示坐标.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_设置高亮提示坐标.Location = new Point(16, 200);
            roundedButton_设置高亮提示坐标.Name = "roundedButton_设置高亮提示坐标";
            roundedButton_设置高亮提示坐标.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_设置高亮提示坐标.Size = new Size(127, 28);
            roundedButton_设置高亮提示坐标.TabIndex = 9;
            roundedButton_设置高亮提示坐标.Text = "设置高亮提示坐标";
            roundedButton_设置高亮提示坐标.TextColor = Color.Black;
            roundedButton_设置高亮提示坐标.TextFont = new Font("微软雅黑", 10F);
            roundedButton_设置高亮提示坐标.Click += roundedButton_设置高亮提示坐标_Click;
            // 
            // label_设置刷新按钮坐标
            // 
            label_设置刷新按钮坐标.ForeColor = Color.Red;
            label_设置刷新按钮坐标.Location = new Point(149, 157);
            label_设置刷新按钮坐标.Name = "label_设置刷新按钮坐标";
            label_设置刷新按钮坐标.Size = new Size(68, 28);
            label_设置刷新按钮坐标.TabIndex = 8;
            label_设置刷新按钮坐标.Text = "未设置";
            label_设置刷新按钮坐标.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // roundedButton_设置刷新按钮坐标
            // 
            roundedButton_设置刷新按钮坐标.BorderColor = Color.FromArgb(200, 200, 200);
            roundedButton_设置刷新按钮坐标.BorderWidth = 1;
            roundedButton_设置刷新按钮坐标.ButtonColor = Color.FromArgb(255, 255, 255);
            roundedButton_设置刷新按钮坐标.CornerRadius = 3;
            roundedButton_设置刷新按钮坐标.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_设置刷新按钮坐标.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_设置刷新按钮坐标.Location = new Point(16, 157);
            roundedButton_设置刷新按钮坐标.Name = "roundedButton_设置刷新按钮坐标";
            roundedButton_设置刷新按钮坐标.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_设置刷新按钮坐标.Size = new Size(127, 28);
            roundedButton_设置刷新按钮坐标.TabIndex = 7;
            roundedButton_设置刷新按钮坐标.Text = "设置刷新按钮坐标";
            roundedButton_设置刷新按钮坐标.TextColor = Color.Black;
            roundedButton_设置刷新按钮坐标.TextFont = new Font("微软雅黑", 10F);
            roundedButton_设置刷新按钮坐标.Click += roundedButton_设置刷新按钮坐标_Click;
            // 
            // comboBox_选择显示器
            // 
            comboBox_选择显示器.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_选择显示器.FormattingEnabled = true;
            comboBox_选择显示器.Location = new Point(112, 71);
            comboBox_选择显示器.Name = "comboBox_选择显示器";
            comboBox_选择显示器.Size = new Size(149, 25);
            comboBox_选择显示器.TabIndex = 6;
            comboBox_选择显示器.SelectedIndexChanged += comboBox_选择显示器_SelectedIndexChanged;
            // 
            // label15
            // 
            label15.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label15.Location = new Point(16, 71);
            label15.Name = "label15";
            label15.Size = new Size(90, 25);
            label15.TabIndex = 5;
            label15.Text = "选择显示器";
            label15.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_设置英雄名称坐标
            // 
            label_设置英雄名称坐标.ForeColor = Color.Red;
            label_设置英雄名称坐标.Location = new Point(149, 116);
            label_设置英雄名称坐标.Name = "label_设置英雄名称坐标";
            label_设置英雄名称坐标.Size = new Size(68, 28);
            label_设置英雄名称坐标.TabIndex = 4;
            label_设置英雄名称坐标.Text = "未设置";
            label_设置英雄名称坐标.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.ForeColor = Color.Gray;
            label14.Location = new Point(13, 45);
            label14.Name = "label14";
            label14.Size = new Size(248, 17);
            label14.TabIndex = 3;
            label14.Text = "请依次点击下方按钮，按照提示框选相应区域";
            label14.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // roundedButton_设置英雄名称坐标
            // 
            roundedButton_设置英雄名称坐标.BorderColor = Color.FromArgb(200, 200, 200);
            roundedButton_设置英雄名称坐标.BorderWidth = 1;
            roundedButton_设置英雄名称坐标.ButtonColor = Color.FromArgb(255, 255, 255);
            roundedButton_设置英雄名称坐标.CornerRadius = 3;
            roundedButton_设置英雄名称坐标.DisabledColor = Color.FromArgb(160, 160, 160);
            roundedButton_设置英雄名称坐标.HoverColor = Color.FromArgb(232, 232, 232);
            roundedButton_设置英雄名称坐标.Location = new Point(16, 116);
            roundedButton_设置英雄名称坐标.Name = "roundedButton_设置英雄名称坐标";
            roundedButton_设置英雄名称坐标.PressedColor = Color.FromArgb(222, 222, 222);
            roundedButton_设置英雄名称坐标.Size = new Size(127, 28);
            roundedButton_设置英雄名称坐标.TabIndex = 2;
            roundedButton_设置英雄名称坐标.Text = "设置英雄名称坐标";
            roundedButton_设置英雄名称坐标.TextColor = Color.Black;
            roundedButton_设置英雄名称坐标.TextFont = new Font("微软雅黑", 10F);
            roundedButton_设置英雄名称坐标.Click += roundedButton_设置英雄名称坐标_Click;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label16.Location = new Point(10, 10);
            label16.Name = "label16";
            label16.Size = new Size(138, 28);
            label16.TabIndex = 0;
            label16.Text = "手动设置坐标";
            label16.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_3自动设置坐标
            // 
            panel_3自动设置坐标.Controls.Add(label_进程状态);
            panel_3自动设置坐标.Controls.Add(label10);
            panel_3自动设置坐标.Controls.Add(选择游戏窗口进程);
            panel_3自动设置坐标.Controls.Add(label12);
            panel_3自动设置坐标.Controls.Add(label13);
            panel_3自动设置坐标.Dock = DockStyle.Fill;
            panel_3自动设置坐标.Location = new Point(0, 0);
            panel_3自动设置坐标.Name = "panel_3自动设置坐标";
            panel_3自动设置坐标.Size = new Size(738, 460);
            panel_3自动设置坐标.TabIndex = 11;
            // 
            // label_进程状态
            // 
            label_进程状态.AutoSize = true;
            label_进程状态.ForeColor = Color.Red;
            label_进程状态.Location = new Point(16, 157);
            label_进程状态.Name = "label_进程状态";
            label_进程状态.Size = new Size(68, 17);
            label_进程状态.TabIndex = 4;
            label_进程状态.Text = "未选择进程";
            label_进程状态.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = Color.Gray;
            label10.Location = new Point(13, 72);
            label10.Name = "label10";
            label10.Size = new Size(272, 34);
            label10.TabIndex = 3;
            label10.Text = "请点击下方按钮，从列表中选择您的游戏进程窗口\r\n程序将自动计算并设置所有必要的坐标";
            label10.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // 选择游戏窗口进程
            // 
            选择游戏窗口进程.BorderColor = Color.FromArgb(200, 200, 200);
            选择游戏窗口进程.BorderWidth = 1;
            选择游戏窗口进程.ButtonColor = Color.FromArgb(255, 255, 255);
            选择游戏窗口进程.CornerRadius = 3;
            选择游戏窗口进程.DisabledColor = Color.FromArgb(160, 160, 160);
            选择游戏窗口进程.HoverColor = Color.FromArgb(232, 232, 232);
            选择游戏窗口进程.Location = new Point(16, 116);
            选择游戏窗口进程.Name = "选择游戏窗口进程";
            选择游戏窗口进程.PressedColor = Color.FromArgb(222, 222, 222);
            选择游戏窗口进程.Size = new Size(127, 35);
            选择游戏窗口进程.TabIndex = 2;
            选择游戏窗口进程.Text = "选择游戏窗口进程";
            选择游戏窗口进程.TextColor = Color.Black;
            选择游戏窗口进程.TextFont = new Font("微软雅黑", 10F);
            选择游戏窗口进程.Click += 选择游戏窗口进程_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.ForeColor = Color.Red;
            label12.Location = new Point(13, 45);
            label12.Name = "label12";
            label12.Size = new Size(596, 17);
            label12.TabIndex = 1;
            label12.Text = "注：自动设置坐标仅适用于任意分辨率下的端游英雄联盟与特定分辨率下运行于mumu模拟器上的金铲铲之战。";
            label12.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label13.Location = new Point(10, 10);
            label13.Name = "label13";
            label13.Size = new Size(138, 28);
            label13.TabIndex = 0;
            label13.Text = "自动设置坐标";
            label13.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_2坐标设置模式
            // 
            panel_2坐标设置模式.Controls.Add(label7);
            panel_2坐标设置模式.Controls.Add(radioButton_手动设置坐标);
            panel_2坐标设置模式.Controls.Add(label6);
            panel_2坐标设置模式.Controls.Add(radioButton_自动设置坐标);
            panel_2坐标设置模式.Controls.Add(label8);
            panel_2坐标设置模式.Controls.Add(label9);
            panel_2坐标设置模式.Dock = DockStyle.Fill;
            panel_2坐标设置模式.Location = new Point(0, 0);
            panel_2坐标设置模式.Name = "panel_2坐标设置模式";
            panel_2坐标设置模式.Size = new Size(738, 460);
            panel_2坐标设置模式.TabIndex = 10;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.Gray;
            label7.Location = new Point(42, 211);
            label7.Name = "label7";
            label7.Size = new Size(368, 34);
            label7.TabIndex = 5;
            label7.Text = "通过手动框选屏幕区域来设置坐标。\r\n适用于无法自动设置坐标的场景，或需要精确控制坐标的高级用户。";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // radioButton_手动设置坐标
            // 
            radioButton_手动设置坐标.AutoSize = true;
            radioButton_手动设置坐标.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            radioButton_手动设置坐标.Location = new Point(23, 179);
            radioButton_手动设置坐标.Name = "radioButton_手动设置坐标";
            radioButton_手动设置坐标.Size = new Size(124, 25);
            radioButton_手动设置坐标.TabIndex = 4;
            radioButton_手动设置坐标.Text = "手动设置坐标";
            radioButton_手动设置坐标.UseVisualStyleBackColor = true;
            radioButton_手动设置坐标.CheckedChanged += radioButton_手动设置坐标_CheckedChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.Gray;
            label6.Location = new Point(42, 121);
            label6.Name = "label6";
            label6.Size = new Size(452, 34);
            label6.TabIndex = 3;
            label6.Text = "通过选择游戏进程窗口，程序将自动计算并设置所有必要的坐标。\r\n适用于英雄联盟、mumu模拟器下运行的金铲铲之战，推荐首次使用者选择此模式。";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // radioButton_自动设置坐标
            // 
            radioButton_自动设置坐标.AutoSize = true;
            radioButton_自动设置坐标.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            radioButton_自动设置坐标.Location = new Point(23, 89);
            radioButton_自动设置坐标.Name = "radioButton_自动设置坐标";
            radioButton_自动设置坐标.Size = new Size(188, 25);
            radioButton_自动设置坐标.TabIndex = 2;
            radioButton_自动设置坐标.Text = "自动设置坐标（推荐）";
            radioButton_自动设置坐标.UseVisualStyleBackColor = true;
            radioButton_自动设置坐标.CheckedChanged += radioButton_自动设置坐标_CheckedChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = Color.Gray;
            label8.Location = new Point(13, 56);
            label8.Name = "label8";
            label8.Size = new Size(241, 17);
            label8.TabIndex = 1;
            label8.Text = "坐标包括OCR截图/点击/高亮/刷新商店坐标";
            label8.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label9.Location = new Point(10, 10);
            label9.Name = "label9";
            label9.Size = new Size(180, 28);
            label9.TabIndex = 0;
            label9.Text = "选择坐标设置模式";
            label9.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel_1欢迎页
            // 
            panel_1欢迎页.Controls.Add(label4);
            panel_1欢迎页.Controls.Add(label3);
            panel_1欢迎页.Controls.Add(label2);
            panel_1欢迎页.Controls.Add(label1);
            panel_1欢迎页.Dock = DockStyle.Fill;
            panel_1欢迎页.Location = new Point(0, 0);
            panel_1欢迎页.Name = "panel_1欢迎页";
            panel_1欢迎页.Size = new Size(738, 460);
            panel_1欢迎页.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.Gray;
            label4.Location = new Point(13, 121);
            label4.Name = "label4";
            label4.Size = new Size(316, 17);
            label4.TabIndex = 3;
            label4.Text = "您可以随时在“设置”中修改这些配置。点击“下一步”继续。";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 89);
            label3.Name = "label3";
            label3.Size = new Size(500, 17);
            label3.TabIndex = 2;
            label3.Text = "配置内容包括：OCR截图/点击/高亮/刷新商店坐标设置、拿牌/刷新方式、OCR推理设备等。";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 56);
            label2.Name = "label2";
            label2.Size = new Size(236, 17);
            label2.TabIndex = 1;
            label2.Text = "本向导将帮助您快速完成程序的初始配置。";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label1.Location = new Point(10, 10);
            label1.Name = "label1";
            label1.Size = new Size(407, 28);
            label1.TabIndex = 0;
            label1.Text = "欢迎使用 JinJinChanChanTool 配置向导";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            panel4.Controls.Add(button_下一步);
            panel4.Controls.Add(button_上一步);
            panel4.Controls.Add(button_跳过向导);
            panel4.Controls.Add(button_完成);
            panel4.Location = new Point(0, 498);
            panel4.Name = "panel4";
            panel4.Size = new Size(744, 65);
            panel4.TabIndex = 7;
            // 
            // button_下一步
            // 
            button_下一步.BorderColor = Color.FromArgb(200, 200, 200);
            button_下一步.BorderWidth = 1;
            button_下一步.ButtonColor = Color.FromArgb(24, 96, 251);
            button_下一步.CornerRadius = 5;
            button_下一步.DisabledColor = Color.FromArgb(160, 160, 160);
            button_下一步.HoverColor = Color.FromArgb(45, 135, 255);
            button_下一步.Location = new Point(627, 15);
            button_下一步.Name = "button_下一步";
            button_下一步.PressedColor = Color.FromArgb(19, 77, 201);
            button_下一步.Size = new Size(94, 32);
            button_下一步.TabIndex = 2;
            button_下一步.Text = "下一步";
            button_下一步.TextColor = Color.White;
            button_下一步.TextFont = new Font("微软雅黑", 10F);
            button_下一步.Click += button_下一步_Click;
            // 
            // button_上一步
            // 
            button_上一步.BorderColor = Color.FromArgb(200, 200, 200);
            button_上一步.BorderWidth = 1;
            button_上一步.ButtonColor = Color.FromArgb(255, 255, 255);
            button_上一步.CornerRadius = 5;
            button_上一步.DisabledColor = Color.FromArgb(160, 160, 160);
            button_上一步.HoverColor = Color.FromArgb(232, 232, 232);
            button_上一步.Location = new Point(518, 15);
            button_上一步.Name = "button_上一步";
            button_上一步.PressedColor = Color.FromArgb(222, 222, 222);
            button_上一步.Size = new Size(94, 32);
            button_上一步.TabIndex = 1;
            button_上一步.Text = "上一步";
            button_上一步.TextColor = Color.Black;
            button_上一步.TextFont = new Font("微软雅黑", 10F);
            button_上一步.Click += button_上一步_Click;
            // 
            // button_跳过向导
            // 
            button_跳过向导.BorderColor = Color.FromArgb(200, 200, 200);
            button_跳过向导.BorderWidth = 1;
            button_跳过向导.ButtonColor = Color.FromArgb(255, 255, 255);
            button_跳过向导.CornerRadius = 5;
            button_跳过向导.DisabledColor = Color.FromArgb(160, 160, 160);
            button_跳过向导.HoverColor = Color.FromArgb(232, 232, 232);
            button_跳过向导.Location = new Point(26, 15);
            button_跳过向导.Name = "button_跳过向导";
            button_跳过向导.PressedColor = Color.FromArgb(222, 222, 222);
            button_跳过向导.Size = new Size(94, 32);
            button_跳过向导.TabIndex = 0;
            button_跳过向导.Text = "跳过向导";
            button_跳过向导.TextColor = Color.Black;
            button_跳过向导.TextFont = new Font("微软雅黑", 10F);
            button_跳过向导.Click += button_跳过向导_Click;
            // 
            // button_完成
            // 
            button_完成.BorderColor = Color.FromArgb(200, 200, 200);
            button_完成.BorderWidth = 1;
            button_完成.ButtonColor = Color.FromArgb(24, 96, 251);
            button_完成.CornerRadius = 5;
            button_完成.DisabledColor = Color.FromArgb(160, 160, 160);
            button_完成.HoverColor = Color.FromArgb(45, 135, 255);
            button_完成.Location = new Point(627, 15);
            button_完成.Name = "button_完成";
            button_完成.PressedColor = Color.FromArgb(19, 77, 201);
            button_完成.Size = new Size(94, 32);
            button_完成.TabIndex = 3;
            button_完成.Text = "完成";
            button_完成.TextColor = Color.White;
            button_完成.TextFont = new Font("微软雅黑", 10F);
            button_完成.Click += button_完成_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(label5);
            panel3.Controls.Add(pictureBox1);
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(744, 30);
            panel3.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(26, 1);
            label5.MaximumSize = new Size(80, 28);
            label5.MinimumSize = new Size(80, 28);
            label5.Name = "label5";
            label5.Size = new Size(80, 28);
            label5.TabIndex = 10;
            label5.Text = "配置向导";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(5, 5);
            pictureBox1.Margin = new Padding(0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(18, 18);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // SetupWizardForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(751, 573);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SetupWizardForm";
            Text = " JinChanChanTool";
            TopMost = true;
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel_8GPU环境配置.ResumeLayout(false);
            panel_8GPU环境配置.PerformLayout();
            groupBoxEnvironment.ResumeLayout(false);
            groupBoxEnvironment.PerformLayout();
            groupBoxConfig.ResumeLayout(false);
            groupBoxConfig.PerformLayout();
            panel_9配置完成.ResumeLayout(false);
            panel_9配置完成.PerformLayout();
            panel_7选择OCR推理设备.ResumeLayout(false);
            panel_7选择OCR推理设备.PerformLayout();
            panel_6选择刷新商店方式.ResumeLayout(false);
            panel_6选择刷新商店方式.PerformLayout();
            Panel_5选择拿牌方式.ResumeLayout(false);
            Panel_5选择拿牌方式.PerformLayout();
            panel_4手动设置坐标.ResumeLayout(false);
            panel_4手动设置坐标.PerformLayout();
            panel_3自动设置坐标.ResumeLayout(false);
            panel_3自动设置坐标.PerformLayout();
            panel_2坐标设置模式.ResumeLayout(false);
            panel_2坐标设置模式.PerformLayout();
            panel_1欢迎页.ResumeLayout(false);
            panel_1欢迎页.PerformLayout();
            panel4.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Label label5;
        private PictureBox pictureBox1;
        private Panel panel4;
        private RoundedButton button_下一步;
        private RoundedButton button_上一步;
        private RoundedButton button_跳过向导;
        private Panel panel5;
        private Panel panel_1欢迎页;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label label3;
        private Panel panel_2坐标设置模式;
        private Label label8;
        private Label label9;
        private RadioButton radioButton_自动设置坐标;
        private Label label7;
        private RadioButton radioButton_手动设置坐标;
        private Label label6;
        private RoundedButton button_完成;
        private Panel panel_3自动设置坐标;
        private Label label12;
        private Label label13;
        private RoundedButton 选择游戏窗口进程;
        private Label label_进程状态;
        private Label label10;
        private Panel panel_4手动设置坐标;
        private Label label_设置英雄名称坐标;
        private Label label14;
        private RoundedButton roundedButton_设置英雄名称坐标;
        private Label label16;
        private ComboBox comboBox_选择显示器;
        private Label label15;
        private Label label_设置高亮提示坐标;
        private RoundedButton roundedButton_设置高亮提示坐标;
        private Label label_设置刷新按钮坐标;
        private RoundedButton roundedButton_设置刷新按钮坐标;
        private Panel Panel_5选择拿牌方式;
        private Label label20;
        private Label label21;
        private Label label_鼠标拿牌;
        private CapsuleSwitch capsuleSwitch_鼠标拿牌;
        private Label label_按键拿牌;
        private CapsuleSwitch capsuleSwitch_按键拿牌;
        private Label label_拿牌按键1;
        private TextBox textBox_拿牌按键1;
        private Label label_拿牌按键2;
        private TextBox textBox_拿牌按键2;
        private Label label_拿牌按键3;
        private TextBox textBox_拿牌按键3;
        private Label label_拿牌按键4;
        private TextBox textBox_拿牌按键4;
        private Label label_拿牌按键5;
        private TextBox textBox_拿牌按键5;
        private Panel panel_6选择刷新商店方式;
        private Label label25;
        private Label label26;
        private Label label_鼠标刷新;
        private CapsuleSwitch capsuleSwitch_鼠标刷新;
        private Label label_按键刷新;
        private CapsuleSwitch capsuleSwitch_按键刷新;
        private Label label_刷新按键;
        private TextBox textBox_刷新按键;
        private Panel panel_7选择OCR推理设备;
        private Label label19;
        private Label label22;
        private Label label_CPU推理;
        private CapsuleSwitch capsuleSwitch_CPU推理;
        private Label label_CPU说明;
        private Label label_GPU推理;
        private CapsuleSwitch capsuleSwitch_GPU推理;
        private Label label_GPU说明;
        private Panel panel_8GPU环境配置;
        private RoundedButton button_检测GPU环境;
        private RoundedButton button_一键配置;
        private Label label_GPU状态;
        private Label label_CUDA状态;
        private Label label_cuDNN状态;
        private Label label24;
        private Label label27;
        private Panel panel_9配置完成;
        private Label label17;
        private Label label_完成说明;
        private RichTextBox richTextBox1;
        private GroupBox groupBoxEnvironment;
       
        private GroupBox groupBoxConfig;
        private Label labelCudaVersion;
        private ComboBox comboBoxCudaVersion;
        private RichTextBox richTextBoxLog;
    }
}
