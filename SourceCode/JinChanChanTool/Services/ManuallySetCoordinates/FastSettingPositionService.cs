using JinChanChanTool.Tools.MouseTools;

public class FastSettingPositionService : IDisposable
{
    // 绘制状态
    private bool isDrawing = false;//是否处于绘制状态
    private bool waitClick = false;//是否处于等待点击状态

    // 坐标点
    private Point startPoint_Physical;//开始坐标（物理）
    private Point endPoint_Physical;//结束坐标（物理）
    private Point startPoint_Relative;//开始坐标（相对）
    private Point endPoint_Relative;//结束坐标（相对）

    // 矩形
    private Rectangle currentRectangle;// 用于保存绘制中的矩形 
    private Rectangle currentPhysicalRectangle;// 用于外传的物理矩形

    // 窗体组件
    private Form overlayForm;// 用于创建半透明的覆盖层的窗口
    private Form labelForm;//用于显示坐标信息的窗口
    private Label showTipLabel;//用于显示提示信息的Label
    private Label showPointLabel;// 用于显示坐标的Label

    // 目标屏幕
    private Screen targetScreen;

    public FastSettingPositionService(Screen screen)
    {
        targetScreen = screen;
        InitializeComponents();
    }

    /// <summary>
    /// 初始化绘制组件
    /// </summary>
    private void InitializeComponents()
    {
        overlayForm = new Form
        {
            FormBorderStyle = FormBorderStyle.None,// 无边框
            BackColor = Color.Black,// 设置背景颜色为黑色
            WindowState = FormWindowState.Maximized,// 设置为最大化窗体，覆盖整个屏幕
            ShowInTaskbar = false,// 不显示在任务栏
            ControlBox = false,// 禁用控制框
            TopMost = true,// 确保在最上层
            Opacity = 0.5// 设置透明度为 0.5
        };

        showTipLabel = new Label
        {
          ForeColor = Color.Red,// 设置字体颜色为红色
            Font = new Font("Arial", 14, FontStyle.Bold),// 设置字体大小和加粗
            Size = new Size(700, 70),
            TextAlign = ContentAlignment.TopCenter// 使文本居中
        };

        labelForm = new Form
        {
            FormBorderStyle = FormBorderStyle.None,// 无边框
            BackColor = Color.Blue,// 明亮的蓝色背景
            WindowState = FormWindowState.Normal,// 正常状态，避免最大化
            ShowInTaskbar = false,// 不显示在任务栏
            ControlBox = false,// 禁用控制框
            TopMost = true,// 确保在最上层
            Size = new Size(700, 30),
            Owner = overlayForm// 设置 overlayForm 为 labelForm 的父窗体
        };
        // 创建坐标显示标签
        showPointLabel = new Label
        {
            Dock = DockStyle.Top,// 铺满整个 labelForm
            ForeColor = Color.Black,// 设置字体颜色为白色
            BackColor = Color.White,// 明亮的蓝色背景
            Font = new Font("Arial", 10, FontStyle.Bold),// 设置字体大小和加粗
            AutoSize = false,
            Size = new Size(700, 30),
            TextAlign = ContentAlignment.MiddleCenter// 使文本居中
        };
        //将coordinateLabel添加到 labelForm
        labelForm.Controls.Add(showPointLabel);
        overlayForm.Controls.Add(showTipLabel);
        // 绑定鼠标事件到覆盖层窗体
        overlayForm.MouseDown += BackForm_MouseDown;
        overlayForm.MouseMove += BackForm_MouseMove;
        overlayForm.MouseUp += BackForm_MouseUp;
        overlayForm.Paint += BackForm_Paint;
    }

    /// <summary>
    /// 等待用户点击并返回点击位置
    /// </summary>
    /// <param name="prompt">提示信息</param>
    public async Task<Point> WaitForClickAsync(string prompt = "")
    {
        currentRectangle = Rectangle.Empty;
        waitClick = true;// 设置绘制状态

        SetupForms();
        showTipLabel.Text = prompt;
        
        // 等待直到绘制完成
        while (waitClick)
        {
            await Task.Delay(50);
        }
        // 绘制完成后，返回矩形信息
        return startPoint_Physical;
    }

    /// <summary>
    /// 等待用户绘制矩形并返回矩形信息
    /// </summary>
    /// <param name="prompt">提示信息</param>
    public async Task<Rectangle> WaitForDrawAsync(string prompt = "")
    {
        currentRectangle = Rectangle.Empty;
        isDrawing = true;// 设置绘制状态

        SetupForms();
        showTipLabel.Text = prompt;
        
        // 等待直到绘制完成
        while (isDrawing)
        {
            await Task.Delay(50);
        }
        // 绘制完成后，返回矩形信息
        return currentPhysicalRectangle;
    }

    /// <summary>
    /// 设置窗体位置和大小
    /// </summary>
    private void SetupForms()
    {
        // 设置 overlayForm 的位置和大小
        overlayForm.StartPosition = FormStartPosition.Manual;
        overlayForm.Location = new Point(targetScreen.Bounds.Location.X, targetScreen.Bounds.Location.Y);// 设置位置为显示器的左上角
        overlayForm.Size = targetScreen.Bounds.Size;// 设置窗体的大小为显示器的分辨率
        // 设置 labelForm 的位置
        labelForm.StartPosition = FormStartPosition.Manual;
        labelForm.Location = new Point(
            targetScreen.Bounds.X + targetScreen.Bounds.Width / 2 - labelForm.Width / 2,
            targetScreen.Bounds.Top
        );

        showTipLabel.Location = new(overlayForm.Size.Width / 2 - showTipLabel.Size.Width / 2, 60);
        

        overlayForm.Show();
        labelForm.Show();
        labelForm.BringToFront();
        overlayForm.Cursor = Cursors.Cross;
        showTipLabel.Show();
    }

    private void BackForm_MouseDown(object sender, MouseEventArgs e)
    {
        if ((waitClick || isDrawing) && e.Button == MouseButtons.Left)
        {
            startPoint_Physical = MousePositionTool.GetCurrentCoordinates().Physical;// 记录鼠标按下时的位置
            startPoint_Relative = e.Location;//记录鼠标按下时的相对位置

            if (isDrawing)
            {
                currentRectangle = new Rectangle(startPoint_Relative, new Size(0, 0));// 初始化绘制矩形
                currentPhysicalRectangle = new Rectangle(startPoint_Physical, new Size(0, 0));// 初始化物理矩形
            }
        }
    }

    private void BackForm_MouseMove(object sender, MouseEventArgs e)
    {
        // 如果按下鼠标并正在绘制矩形，labelForm 固定在矩形的左上角上方
        if (isDrawing && e.Button == MouseButtons.Left)
        {
            endPoint_Physical = MousePositionTool.GetCurrentCoordinates().Physical;
            endPoint_Relative = e.Location;//记录鼠标按下时的相对位置

            // 更新矩形的大小
            currentPhysicalRectangle.Width = Math.Abs(endPoint_Physical.X - startPoint_Physical.X);
            currentPhysicalRectangle.Height = Math.Abs(endPoint_Physical.Y - startPoint_Physical.Y);

            currentRectangle.Width = Math.Abs(endPoint_Relative.X - startPoint_Relative.X);
            currentRectangle.Height = Math.Abs(endPoint_Relative.Y - startPoint_Relative.Y);

            overlayForm.Invalidate();// 请求重绘
        }

        showPointLabel.Text = $"鼠标坐标X：{MousePositionTool.GetCurrentCoordinates().Physical.X}, " +
                             $"鼠标坐标Y：{MousePositionTool.GetCurrentCoordinates().Physical.Y}   " +
                             $"宽度：{currentRectangle.Width}, 高度：{currentRectangle.Height}";
    }

    private void BackForm_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            overlayForm.Cursor = Cursors.Default;// 恢复鼠标指针
            overlayForm.Hide();
            labelForm.Hide();

            if (waitClick)
            {
                waitClick = false;
            }
            if (isDrawing)
            {
                isDrawing = false;// 结束绘制状态 
            }
        }
    }

    private void BackForm_Paint(object sender, PaintEventArgs e)
    {
        if (isDrawing && currentRectangle != Rectangle.Empty)
        {
            // 创建一个填充矩形的画刷 
            e.Graphics.FillRectangle(Brushes.White, currentRectangle);
            // 绘制矩形的边框
            e.Graphics.DrawRectangle(Pens.Red, currentRectangle);
        }
    }

    public void Dispose()
    {        
        overlayForm?.Dispose();
        labelForm?.Dispose();
    }
}