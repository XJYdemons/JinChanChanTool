using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services.DataServices.Interface;

namespace JinChanChanTool.Forms.DisplayUIForm
{
    public partial class EquipmentForm : Form
    {
        /// <summary>
        /// 装备数据服务
        /// </summary>
        private readonly IEquipmentService _equipmentService;

        /// <summary>
        /// 当前英雄名称
        /// </summary>
        private readonly string _heroName;

        /// <summary>
        /// 当前装备槽位索引
        /// </summary>
        private readonly int _equipmentIndex;

        /// <summary>
        /// 选中的装备名称
        /// </summary>
        public string SelectedEquipmentName { get; private set; }

        /// <summary>
        /// 装备图片框尺寸
        /// </summary>
        private const int EquipmentBoxSize = 48;

        /// <summary>
        /// 装备图片框间距
        /// </summary>
        private const int EquipmentBoxSpacing = 8;

        /// <summary>
        /// 自定义ToolTip控件（显示装备名和合成路径）
        /// </summary>
        private EquipmentInformationToolTip _toolTip;

        /// <summary>
        /// 标题栏高度
        /// </summary>
        private const int TitleBarHeight = 32;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="equipmentService">装备数据服务</param>
        /// <param name="heroName">英雄名称</param>
        /// <param name="equipmentIndex">装备槽位索引</param>
        public EquipmentForm(IEquipmentService equipmentService, string heroName, int equipmentIndex)
        {
            InitializeComponent();
            _equipmentService = equipmentService;
            _heroName = heroName;
            _equipmentIndex = equipmentIndex;
            SelectedEquipmentName = string.Empty;

            // 添加自定义标题栏
            string title = $"为 {heroName} 选择装备 (槽位 {equipmentIndex + 1})";
            CustomTitleBar titleBar = new CustomTitleBar(this, Dpi(TitleBarHeight), null, title, CustomTitleBar.ButtonOptions.Close);
            this.Controls.Add(titleBar);

            // 隐藏图标
            this.ShowIcon = false;

            // 初始化自定义ToolTip
            _toolTip = new EquipmentInformationToolTip(_equipmentService, this)
            {
                AutoPopDelay = 5000,
                InitialDelay = 300,
                ReshowDelay = 100,
                ShowAlways = true
            };

            BuildEquipmentUI();
        }

        /// <summary>
        /// DPI缩放转换
        /// </summary>
        private int Dpi(int value)
        {
            return this.LogicalToDeviceUnits(value);
        }

        /// <summary>
        /// 装备类型显示顺序
        /// </summary>
        private static readonly Dictionary<string, int> EquipmentTypeOrder = new Dictionary<string, int>
        {
            { "散件", 0 },
            { "普通装备", 1 },
            { "转职纹章", 2 },
            { "光明装备", 3 },
            { "奥恩神器", 4 }
        };

        /// <summary>
        /// 构建装备选择UI
        /// </summary>
        private void BuildEquipmentUI()
        {
            panelMain.Controls.Clear();

            List<Equipment> equipments = _equipmentService.GetEquipmentDatas();
            if (equipments == null || equipments.Count == 0)
            {
                Label noDataLabel = new Label
                {
                    Text = "暂无装备数据",
                    AutoSize = true,
                    Location = new Point(Dpi(20), Dpi(20))
                };
                panelMain.Controls.Add(noDataLabel);
                return;
            }

            // 按装备类型分组，按指定顺序排序（散件、普通装备、转职纹章、光明装备、奥恩神器），未定义的类型排在最后
            var groupedEquipments = equipments
                .GroupBy(e => string.IsNullOrEmpty(e.EquipmentType) ? "其他装备" : e.EquipmentType)
                .OrderBy(g => EquipmentTypeOrder.TryGetValue(g.Key, out int order) ? order : int.MaxValue)
                .ThenBy(g => g.Key)
                .ToList();

            int currentY = Dpi(10);

            // 首先添加"清空装备"选项
            currentY = AddClearEquipmentSection(currentY);

            // 按类型创建分组
            foreach (var group in groupedEquipments)
            {
                currentY = AddEquipmentGroup(group.Key, group.ToList(), currentY);
            }
        }

        /// <summary>
        /// 添加清空装备选项
        /// </summary>
        private int AddClearEquipmentSection(int startY)
        {
            // 分组标题
            Label titleLabel = new Label
            {
                Text = "操作",
                Font = new Font(this.Font.FontFamily, 10f, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(Dpi(10), startY)
            };
            panelMain.Controls.Add(titleLabel);

            int contentY = startY + titleLabel.Height + Dpi(5);

            // 清空装备按钮
            Button clearButton = new Button
            {
                Text = "清空此槽位装备",
                Size = new Size(Dpi(120), Dpi(30)),
                Location = new Point(Dpi(10), contentY),
                FlatStyle = FlatStyle.Flat,                
            };
            clearButton.FlatAppearance.BorderColor = Color.LightGray;
            clearButton.Click += (s, e) =>
            {
                SelectedEquipmentName = string.Empty;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            panelMain.Controls.Add(clearButton);

            return contentY + clearButton.Height + Dpi(15);
        }

        /// <summary>
        /// 添加装备分组
        /// </summary>
        private int AddEquipmentGroup(string groupName, List<Equipment> equipments, int startY)
        {
            // 分组标题
            Label titleLabel = new Label
            {
                Text = groupName,
                Font = new Font(this.Font.FontFamily, 10f, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(Dpi(10), startY)
            };
            panelMain.Controls.Add(titleLabel);

            // 分隔线
            Panel separator = new Panel
            {
                BackColor = Color.LightGray,
                Size = new Size(panelMain.Width - Dpi(20), 1),
                Location = new Point(Dpi(10), startY + titleLabel.Height + Dpi(2))
            };
            panelMain.Controls.Add(separator);

            // 装备容器FlowLayoutPanel
            FlowLayoutPanel flowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(Dpi(10), startY + titleLabel.Height + Dpi(8)),
                MaximumSize = new Size(panelMain.Width - Dpi(30), 0),
                BackColor = Color.White
            };

            foreach (var equipment in equipments)
            {
                Panel itemContainer = CreateEquipmentItem(equipment);
                flowPanel.Controls.Add(itemContainer);
            }

            panelMain.Controls.Add(flowPanel);

            return startY + titleLabel.Height + Dpi(8) + flowPanel.PreferredSize.Height + Dpi(15);
        }

        /// <summary>
        /// 创建单个装备项(仅图片，用Panel固定占位)
        /// </summary>
        private Panel CreateEquipmentItem(Equipment equipment)
        {
            int boxSize = Dpi(EquipmentBoxSize);
            int spacing = Dpi(EquipmentBoxSpacing);
            int containerSize = boxSize + spacing; // 容器比图片框稍大，留出变大空间

            // 固定大小的容器Panel
            Panel container = new Panel
            {
                Size = new Size(containerSize, containerSize),
                Margin = new Padding(1),
                BackColor = Color.White
            };

            // 装备图片框（居中放置）
            int offset = (containerSize - boxSize) / 2;
            HeroPictureBox pictureBox = new HeroPictureBox
            {
                Size = new Size(boxSize, boxSize),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = equipment.Image,
                Tag = equipment,
                BorderColor = GetEquipmentTypeColor(equipment.EquipmentType),
                BorderWidth = 2,
                Cursor = Cursors.Hand,
                Location = new Point(offset, offset),
                BackColor = Color.White
            };
            pictureBox.MouseEnter += EquipmentPictureBox_MouseEnter;
            pictureBox.MouseLeave += EquipmentPictureBox_MouseLeave;
            pictureBox.Click += EquipmentPictureBox_Click;

            // 添加ToolTip显示装备名称和合成路径
            _toolTip.SetEquipment(pictureBox);

            container.Controls.Add(pictureBox);
            return container;
        }

        /// <summary>
        /// 根据装备类型获取边框颜色
        /// </summary>
        private Color GetEquipmentTypeColor(string equipmentType)
        {
            return equipmentType switch
            {
                "光明装备" => Color.Gold,
                "普通装备" => Color.Gray,
                "虚空装备" => Color.Purple,
                "比尔装备" => Color.Orange,
                "纹章装备" => Color.DeepSkyBlue,
                "辅助装备" => Color.LimeGreen,
                _ => Color.DarkGray
            };
        }

        /// <summary>
        /// 装备图片框鼠标进入
        /// </summary>
        private void EquipmentPictureBox_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is HeroPictureBox pictureBox)
            {
                int boxSize = Dpi(EquipmentBoxSize);
                int expandSize = 4; // 扩大的像素数
                int newSize = boxSize + expandSize;
                int newOffset = (Dpi(EquipmentBoxSize + EquipmentBoxSpacing) - newSize) / 2;

                pictureBox.Size = new Size(newSize, newSize);
                pictureBox.Location = new Point(newOffset, newOffset);
            }
        }

        /// <summary>
        /// 装备图片框鼠标离开
        /// </summary>
        private void EquipmentPictureBox_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is HeroPictureBox pictureBox)
            {
                int boxSize = Dpi(EquipmentBoxSize);
                int containerSize = Dpi(EquipmentBoxSize + EquipmentBoxSpacing);
                int offset = (containerSize - boxSize) / 2;

                pictureBox.Size = new Size(boxSize, boxSize);
                pictureBox.Location = new Point(offset, offset);
            }
        }

        /// <summary>
        /// 装备图片框点击事件
        /// </summary>
        private void EquipmentPictureBox_Click(object? sender, EventArgs e)
        {
            if (sender is HeroPictureBox pictureBox && pictureBox.Tag is Equipment equipment)
            {
                SelectedEquipmentName = equipment.Name;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
