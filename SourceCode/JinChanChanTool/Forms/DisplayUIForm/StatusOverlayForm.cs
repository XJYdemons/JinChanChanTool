using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Tools;
using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 状态显示窗口
    /// </summary>
    public partial class StatusOverlayForm : Form
    {
        //单例模式
        private static StatusOverlayForm _instance;
        public static StatusOverlayForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new StatusOverlayForm();
                }
                return _instance;
            }
        }
       
        private StatusOverlayForm()
        {
            InitializeComponent();
            DragHelper.EnableDragForChildren(panel2);
        }

        public IAutomaticSettingsService _iAutoConfigService;//自动设置数据服务对象
                                                             //
        private CardService _cardService; //自动拿牌服务
        // 同步标志，防止循环调用
        private bool _isSyncingHighlight = false;
        private bool _isSyncingGetCard = false;
        private bool _isSyncingRefresh = false;

        /// <summary>
        /// 高亮显示胶囊开关状态变更事件
        /// </summary>
        private void HighlightCapsuleSwitch_IsOnChanged(object sender, EventArgs e)
        {
            if (_isSyncingHighlight) return;
            _cardService.ToggleHighLight();
        }

        /// <summary>
        /// 自动拿牌胶囊开关状态变更事件
        /// </summary>
        private void AutoGetCardCapsuleSwitch_IsOnChanged(object sender, EventArgs e)
        {
            if (_isSyncingGetCard) return;
            _cardService.ToggleLoop();
        }

        /// <summary>
        /// 自动刷新商店胶囊开关状态变更事件
        /// </summary>
        private void AutoRefreshCapsuleSwitch_IsOnChanged(object sender, EventArgs e)
        {
            if (_isSyncingRefresh) return;
            _cardService.ToggleRefreshStore();
        }

        /// <summary>
        /// 更新状态显示
        /// </summary>
        public void UpdateStatus(string hotkey1, string hotkey2, string hotkey3, string hotkey4, string hotkey5)
        {           
            if (label_HotKey1.InvokeRequired)
            {
                label_HotKey1.Invoke(new Action(() => UpdateStatus(hotkey1,hotkey2,hotkey3,hotkey4,hotkey5)));
                return;
            }
            label_HotKey1.Text = hotkey1;
            label_HotKey2.Text = hotkey2;
            label_HotKey3.Text = hotkey3;
            label_HotKey4.Text = hotkey4;
            label_HotKey5.Text = hotkey5;
        }

        /// <summary>
        /// 自动拿牌状态变更回调
        /// </summary>
        private void OnStartLoop(bool isRunning)
        {
            if (capsuleSwitch_GetCard.InvokeRequired)
            {
                capsuleSwitch_GetCard.Invoke(new Action<bool>(OnStartLoop), isRunning);
                return;
            }
            _isSyncingGetCard = true;
            capsuleSwitch_GetCard.IsOn = isRunning;
            _isSyncingGetCard = false;
        }

        /// <summary>
        /// 自动刷新商店状态变更回调
        /// </summary>
        private void OnAutoRefreshStatusChanged(bool isRunning)
        {
            if (capsuleSwitch_RefreshStore.InvokeRequired)
            {
                capsuleSwitch_RefreshStore.Invoke(new Action<bool>(OnAutoRefreshStatusChanged), isRunning);
                return;
            }
            _isSyncingRefresh = true;
            capsuleSwitch_RefreshStore.IsOn = isRunning;
            _isSyncingRefresh = false;
        }

        /// <summary>
        /// 高亮显示状态变更回调
        /// </summary>
        private void OnHighlightStatusChanged(bool isRunning)
        {
            if (capsuleSwitch_HighLight.InvokeRequired)
            {
                capsuleSwitch_HighLight.Invoke(new Action<bool>(OnHighlightStatusChanged), isRunning);
                return;
            }
            _isSyncingHighlight = true;
            capsuleSwitch_HighLight.IsOn = isRunning;
            _isSyncingHighlight = false;
        }

       
        /// <summary>
        /// 初始化配置服务
        /// </summary>
        /// <param name="iAutoConfigService">自动配置服务</param>
        /// <param name="cardService">卡牌服务</param>
        public void InitializeObject(IAutomaticSettingsService iAutoConfigService, CardService cardService)
        {
            _iAutoConfigService = iAutoConfigService;
            _cardService = cardService;
            _cardService.isGetCardStatusChanged += OnStartLoop;
            _cardService.isRefreshStoreStatusChanged += OnAutoRefreshStatusChanged;
            _cardService.isHighLightStatusChanged += OnHighlightStatusChanged;
        }

        
     
        /// <summary>
        /// 显示窗体时应用保存的位置
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);           
            try
            {
                this.StartPosition = FormStartPosition.Manual;
                if (_iAutoConfigService.CurrentConfig.StatusOverlayFormLocation.X == -1 && _iAutoConfigService.CurrentConfig.StatusOverlayFormLocation.Y == -1)
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        0 /*- 10*/,
                        screen.Bottom-this.Height /*+ 10*/
                    );
                    return;
                }
                // 确保坐标在屏幕范围内
                if (Screen.AllScreens.Any(s => s.Bounds.Contains(_iAutoConfigService.CurrentConfig.StatusOverlayFormLocation)))
                {
                    this.Location = _iAutoConfigService.CurrentConfig.StatusOverlayFormLocation;
                }
                else
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    this.Location = new Point(
                        0 /*- 10*/,
                        screen.Bottom - this.Height /*+ 10*/
                    );
                }
            }
            catch
            {
                var screen = Screen.PrimaryScreen.Bounds;
                this.Location = new Point(
                        0 /*- 10*/,
                        screen.Bottom - this.Height /*+ 10*/
                    );
            }
        }

        #region 位置保存与读取

        /// <summary>
        /// Windows消息常量 - 窗口移动或大小调整结束
        /// </summary>
        private const int WM_EXITSIZEMOVE = 0x0232;

        /// <summary>
        /// 重写窗口过程以监听拖动结束消息
        /// </summary>
        /// <param name="m">Windows消息</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 监听窗口移动结束消息
            if (m.Msg == WM_EXITSIZEMOVE)
            {
                // 拖动结束,保存位置
                SaveFormLocation();
            }
        }

        /// <summary>
        /// 拖动结束时保存窗口位置到配置服务
        /// </summary>
        private void SaveFormLocation()
        {
            Debug.WriteLine($"StatusOverlayForm - 保存位置: {this.Location}");
            try
            {
                if (_iAutoConfigService != null)
                {
                    _iAutoConfigService.CurrentConfig.StatusOverlayFormLocation = this.Location;
                    _iAutoConfigService.Save();
                   
                }
                else
                {
                   
                }
            }
            catch (Exception ex)
            {
              
            }
        }

        #endregion

        #region 圆角实现
        // GDI32 API - 用于创建圆角效果
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        // 圆角半径
        private const int CORNER_RADIUS = 16;

        /// <summary>
        /// 在窗口句柄创建后应用圆角效果
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // 应用 GDI Region 圆角效果（支持 Windows 10 和 Windows 11）
            ApplyRoundedCorners();
        }

        /// <summary>
        /// 应用 GDI Region 圆角效果
        /// </summary>
        private void ApplyRoundedCorners()
        {
            try
            {
                // 创建圆角矩形区域
                IntPtr region = CreateRoundRectRgn(0, 0, Width, Height, CORNER_RADIUS, CORNER_RADIUS);

                if (region != IntPtr.Zero)
                {
                    SetWindowRgn(Handle, region, true);
                    // 注意：SetWindowRgn 会接管 region 的所有权，不需要手动删除

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 窗口大小改变时重新应用圆角
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 调整大小时重新创建圆角区域
            if (Handle != IntPtr.Zero)
            {
                ApplyRoundedCorners();
            }
        }
        #endregion
    }
}
