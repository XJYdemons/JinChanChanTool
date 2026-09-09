using JinChanChanTool.Services.Localization;
using JinChanChanTool.Tools;
using System.Runtime.InteropServices;

namespace JinChanChanTool;

public partial class BranchEditorForm : Form
{
    private readonly ILocalizationService _iLocalizationService;

    public string BranchName => textBox_名称.Text.Trim();
    public string BranchDescription => textBox_描述.Text.Trim();

    public BranchEditorForm(ILocalizationService iLocalizationService, string title, string name = "", string description = "")
    {
        _iLocalizationService = iLocalizationService ?? throw new ArgumentNullException(nameof(iLocalizationService));
        InitializeComponent();
        ApplyLocalization(title);
        textBox_名称.Text = name;
        textBox_描述.Text = description;
        DragHelper.EnableDragForChildren(panel_标题栏);
        AcceptButton = button_确定;
        CancelButton = button_取消;
        Shown += (_, _) => { Activate(); BringToFront(); };
    }

    private void button_确定_Click(object sender, EventArgs e)
    {
        if (BranchName.Length is < 1 or > 7)
        {
            MessageBox.Show(this,
                _iLocalizationService.Get("BranchEditorForm.Msg.名称长度"),
                _iLocalizationService.Get("BranchEditorForm.MsgTitle.提示"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            textBox_名称.Focus();
            return;
        }
        DialogResult = DialogResult.OK;
        Close();
    }

    private void button_最小化_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;
    private void button_关闭_Click(object sender, EventArgs e) => Close();

    private void ApplyLocalization(string title)
    {
        label_标题.Text = title;
        Text = title;
        label_名称.Text = _iLocalizationService.Get("BranchEditorForm.Label.分支名称");
        label_描述.Text = _iLocalizationService.Get("BranchEditorForm.Label.描述");
        button_确定.Text = _iLocalizationService.Get("BranchEditorForm.Button.确定");
        button_取消.Text = _iLocalizationService.Get("BranchEditorForm.Button.取消");
    }

    #region 圆角实现
    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

    [DllImport("user32.dll")]
    private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

    private const int CORNER_RADIUS = 16;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ApplyRoundedCorners();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (Handle != IntPtr.Zero) ApplyRoundedCorners();
    }

    private void ApplyRoundedCorners()
    {
        try
        {
            IntPtr region = CreateRoundRectRgn(0, 0, Width, Height, CORNER_RADIUS, CORNER_RADIUS);
            if (region != IntPtr.Zero) SetWindowRgn(Handle, region, true);
        }
        catch
        {
            // 圆角失败时保留正常窗体显示。
        }
    }
    #endregion
}
