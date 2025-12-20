using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services;
using JinChanChanTool.Services.DataServices.Interface;

namespace JinChanChanTool.Forms.DisplayUIForm
{
    /// <summary>
    /// 推荐阵容选择窗体
    /// </summary>
    public partial class LineUpSelectForm : Form
    {
        private readonly IRecommendedLineUpService _recommendedLineUpService;
        private readonly IHeroDataService _heroDataService;
        private readonly IEquipmentService _equipmentService;
        private List<RecommendedLineUp> _allLineUps;
        private List<RecommendedLineUp> _filteredLineUps;
        private Panel? _selectedCard;

        // 卡片尺寸常量（96 DPI 下的基准值）
        private const int CARD_HEIGHT_BASE = 120;
        private const int HERO_SIZE_BASE = 52;
        private const int CARD_PADDING_BASE = 8;
        private const int TIER_WIDTH_BASE = 70;          // 评级区域宽度
        private const int NAME_WIDTH_BASE = 120;         // 名称+标签区域宽度
        private const int STATS_WIDTH_BASE = 140;        // 统计数据田字格宽度
        private const int DESC_WIDTH_BASE = 150;         // 描述区域宽度

        // 预计算DPI值
        private int _cardHeight;
        private int _heroSize;
        private int _padding;
        private int _tierWidth;
        private int _nameWidth;
        private int _statsWidth;
        private int _descWidth;
        private int _cardWidth;

        // 卡片对象池
        private readonly List<Panel> _cardPool = new List<Panel>();
        private int _activeCardCount = 0;

        // 搜索防抖计时器
        private System.Windows.Forms.Timer? _searchDebounceTimer;
        private const int SEARCH_DEBOUNCE_MS = 150;

        /// <summary>
        /// 用户选择的推荐阵容
        /// </summary>
        public RecommendedLineUp? SelectedLineUp { get; private set; }

        public LineUpSelectForm(IRecommendedLineUpService recommendedLineUpService, IHeroDataService heroDataService, IEquipmentService equipmentService)
        {
            InitializeComponent();

            // 启用双缓冲减少闪烁
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            _recommendedLineUpService = recommendedLineUpService;
            _heroDataService = heroDataService;
            _equipmentService = equipmentService;
            _allLineUps = new List<RecommendedLineUp>();
            _filteredLineUps = new List<RecommendedLineUp>();

            // 预计算DPI值
            PreCalculateDpiValues();

            // 初始化搜索防抖计时器
            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = SEARCH_DEBOUNCE_MS;
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            // 添加自定义标题栏
            string title = "推荐阵容";
            CustomTitleBar titleBar = new CustomTitleBar(this, Dpi(32), null, title, CustomTitleBar.ButtonOptions.Close);
            this.Controls.Add(titleBar);

            // 初始化筛选下拉框
            comboBox_TierFilter.SelectedIndex = 0;
            comboBox_SortBy.SelectedIndex = 0;

            // 加载数据
            LoadLineUpData();
        }

        /// <summary>
        /// 预计算DPI值，避免重复计算
        /// </summary>
        private void PreCalculateDpiValues()
        {
            _cardHeight = Dpi(CARD_HEIGHT_BASE);
            _heroSize = Dpi(HERO_SIZE_BASE);
            _padding = Dpi(CARD_PADDING_BASE);
            _tierWidth = Dpi(TIER_WIDTH_BASE);
            _nameWidth = Dpi(NAME_WIDTH_BASE);
            _statsWidth = Dpi(STATS_WIDTH_BASE);
            _descWidth = Dpi(DESC_WIDTH_BASE);
            _cardWidth = flowLayoutPanel_LineUps.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - Dpi(10);
        }

        /// <summary>
        /// DPI缩放转换
        /// </summary>
        private int Dpi(int value) => this.LogicalToDeviceUnits(value);

        /// <summary>
        /// 加载推荐阵容数据
        /// </summary>
        private void LoadLineUpData()
        {
            _allLineUps = _recommendedLineUpService.GetAllRecommendedLineUps();

            // 更新更新时间标签
            var lastUpdateTime = _recommendedLineUpService.GetLastUpdateTime();
            if (lastUpdateTime != DateTime.MinValue)
            {
                label_UpdateTime.Text = $"更新时间: {lastUpdateTime:yyyy-MM-dd HH:mm}";
            }
            else
            {
                label_UpdateTime.Text = "更新时间: 暂无数据";
            }

            // 应用筛选并刷新列表
            ApplyFilter();
        }

        /// <summary>
        /// 应用筛选条件（立即执行，用于下拉框变更）
        /// </summary>
        private void ApplyFilter()
        {
            ApplyFilterInternal();
        }

        /// <summary>
        /// 应用筛选条件（带防抖，用于搜索框）
        /// </summary>
        private void ApplyFilterWithDebounce()
        {
            _searchDebounceTimer?.Stop();
            _searchDebounceTimer?.Start();
        }

        /// <summary>
        /// 实际执行筛选逻辑
        /// </summary>
        private void ApplyFilterInternal()
        {
            string selectedTier = comboBox_TierFilter.SelectedItem?.ToString() ?? "全部";

            if (selectedTier == "全部")
            {
                _filteredLineUps = _allLineUps.ToList();
            }
            else
            {
                // 将显示文本转换为枚举值
                LineUpTier? tierFilter = selectedTier switch
                {
                    "S" => LineUpTier.S,
                    "A" => LineUpTier.A,
                    "B" => LineUpTier.B,
                    "C" => LineUpTier.C,
                    "D" => LineUpTier.D,
                    _ => null
                };

                if (tierFilter.HasValue)
                {
                    _filteredLineUps = _allLineUps.Where(l => l.Tier == tierFilter.Value).ToList();
                }
                else
                {
                    _filteredLineUps = _allLineUps.ToList();
                }
            }

            // 应用搜索关键字筛选
            string searchKeyword = textBox_Search.Text.Trim();
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                _filteredLineUps = _filteredLineUps.Where(l =>
                    l.LineUpName.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                    l.Tags.Any(tag => tag.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase)) ||
                    l.Description.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                    l.LineUpUnits.Any(unit => unit.HeroName.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // 根据选择的排序方式排序
            string sortBy = comboBox_SortBy.SelectedItem?.ToString() ?? "评级优先";
            _filteredLineUps = sortBy switch
            {
                "胜率" => _filteredLineUps.OrderByDescending(l => l.WinRate).ToList(),
                "前四率" => _filteredLineUps.OrderByDescending(l => l.TopFourRate).ToList(),
                "选取率" => _filteredLineUps.OrderByDescending(l => l.PickRate).ToList(),
                "平均名次" => _filteredLineUps.OrderBy(l => l.AverageRank).ToList(),  // 名次越低越好
                _ => _filteredLineUps.OrderBy(l => l.Tier).ThenByDescending(l => l.WinRate).ToList()  // 评级优先
            };

            RefreshCards();
        }

        /// <summary>
        /// 搜索防抖计时器触发事件
        /// </summary>
        private void SearchDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _searchDebounceTimer?.Stop();
            ApplyFilterInternal();
        }

        /// <summary>
        /// 刷新卡片显示（使用对象池优化）
        /// </summary>
        private void RefreshCards()
        {
            flowLayoutPanel_LineUps.SuspendLayout();

            int requiredCount = _filteredLineUps.Count;

            // 确保对象池有足够的卡片
            while (_cardPool.Count < requiredCount)
            {
                var newCard = CreateEmptyCard();
                _cardPool.Add(newCard);
            }

            // 更新可见卡片的内容
            for (int i = 0; i < requiredCount; i++)
            {
                var card = _cardPool[i];
                var lineUp = _filteredLineUps[i];

                // 更新卡片内容
                UpdateCardContent(card, lineUp);

                // 如果卡片不在容器中，添加它
                if (!flowLayoutPanel_LineUps.Controls.Contains(card))
                {
                    flowLayoutPanel_LineUps.Controls.Add(card);
                }

                card.Visible = true;
            }

            // 隐藏多余的卡片（而不是销毁）
            for (int i = requiredCount; i < _cardPool.Count; i++)
            {
                if (flowLayoutPanel_LineUps.Controls.Contains(_cardPool[i]))
                {
                    _cardPool[i].Visible = false;
                }
            }

            // 重新排序控件以匹配筛选结果
            for (int i = 0; i < requiredCount; i++)
            {
                flowLayoutPanel_LineUps.Controls.SetChildIndex(_cardPool[i], i);
            }

            _activeCardCount = requiredCount;
            flowLayoutPanel_LineUps.ResumeLayout();

            // 更新信息标签
            if (_filteredLineUps.Count == 0)
            {
                label_Info.Text = "暂无推荐阵容数据";
            }
            else
            {
                label_Info.Text = $"共 {_filteredLineUps.Count} 套阵容，点击选择一个导入到当前变阵";
            }

            // 清空选择
            _selectedCard = null;
            SelectedLineUp = null;
            button_Confirm.BackColor = Color.FromArgb(60, 60, 63);
            button_Confirm.Enabled = false;
        }

        /// <summary>
        /// 创建空白卡片模板（预创建所有子控件）
        /// </summary>
        private Panel CreateEmptyCard()
        {
            var card = new Panel
            {
                Size = new Size(_cardWidth, _cardHeight),
                BackColor = Color.FromArgb(50, 50, 53),
                Margin = new Padding(0, 0, 0, Dpi(5)),
                Cursor = Cursors.Hand
            };

            int currentX = _padding;

            // ========== 1. 评级区域 ==========
            var tierPanel = new Panel
            {
                Name = "tierPanel",
                Location = new Point(currentX, _padding),
                Size = new Size(_tierWidth, _cardHeight - _padding * 2),
                BackColor = Color.Transparent
            };

            var lblTier = new Label
            {
                Name = "lblTier",
                Font = new Font("Microsoft YaHei UI", 16f, FontStyle.Bold, GraphicsUnit.Point),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            tierPanel.Controls.Add(lblTier);
            card.Controls.Add(tierPanel);
            currentX += _tierWidth + _padding;

            // ========== 2. 名称+标签区域 ==========
            var namePanel = new Panel
            {
                Name = "namePanel",
                Location = new Point(currentX, _padding),
                Size = new Size(_nameWidth, _cardHeight - _padding * 2),
                BackColor = Color.Transparent
            };

            var lblName = new Label
            {
                Name = "lblName",
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 10f, FontStyle.Bold, GraphicsUnit.Point),
                Location = new Point(0, Dpi((int)((_cardHeight - _padding * 2) * 0.3f))),
                AutoSize = true,
                MaximumSize = new Size(_nameWidth, 0)
            };
            namePanel.Controls.Add(lblName);

            var lblTags = new Label
            {
                Name = "lblTags",
                ForeColor = Color.FromArgb(100, 149, 237),
                Font = new Font("Microsoft YaHei UI", 8f, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(0, Dpi((int)((_cardHeight - _padding * 2) * 0.6f))),
                AutoSize = true,
                MaximumSize = new Size(_nameWidth, 0)
            };
            namePanel.Controls.Add(lblTags);

            card.Controls.Add(namePanel);
            currentX += _nameWidth + _padding;

            // ========== 3. 统计数据区域 ==========
            var statsPanel = new Panel
            {
                Name = "statsPanel",
                Location = new Point(currentX, _padding),
                Size = new Size(_statsWidth, _cardHeight - _padding * 2),
                BackColor = Color.Transparent
            };

            int cellWidth = _statsWidth / 2;
            int cellHeight = (_cardHeight - _padding * 2) / 2;

            var lblWinRate = new Label
            {
                Name = "lblWinRate",
                ForeColor = Color.FromArgb(144, 238, 144),
                Font = new Font("Microsoft YaHei UI", 8f, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(0, 0),
                Size = new Size(cellWidth, cellHeight),
                TextAlign = ContentAlignment.MiddleCenter
            };
            statsPanel.Controls.Add(lblWinRate);

            var lblTopFour = new Label
            {
                Name = "lblTopFour",
                ForeColor = Color.FromArgb(135, 206, 250),
                Font = new Font("Microsoft YaHei UI", 8f, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(cellWidth, 0),
                Size = new Size(cellWidth, cellHeight),
                TextAlign = ContentAlignment.MiddleCenter
            };
            statsPanel.Controls.Add(lblTopFour);

            var lblPickRate = new Label
            {
                Name = "lblPickRate",
                ForeColor = Color.FromArgb(255, 182, 193),
                Font = new Font("Microsoft YaHei UI", 8f, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(0, cellHeight),
                Size = new Size(cellWidth, cellHeight),
                TextAlign = ContentAlignment.MiddleCenter
            };
            statsPanel.Controls.Add(lblPickRate);

            var lblRank = new Label
            {
                Name = "lblRank",
                ForeColor = Color.FromArgb(255, 215, 0),
                Font = new Font("Microsoft YaHei UI", 8f, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(cellWidth, cellHeight),
                Size = new Size(cellWidth, cellHeight),
                TextAlign = ContentAlignment.MiddleCenter
            };
            statsPanel.Controls.Add(lblRank);

            card.Controls.Add(statsPanel);
            currentX += _statsWidth + _padding;

            // ========== 4. 英雄展示区域 ==========
            int heroAreaWidth = _cardWidth - currentX - _descWidth - _padding * 2;

            var heroPanel = new FlowLayoutPanel
            {
                Name = "heroPanel",
                Location = new Point(currentX, _padding),
                Size = new Size(heroAreaWidth, _cardHeight - _padding * 2),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false
            };
            card.Controls.Add(heroPanel);
            currentX += heroAreaWidth + _padding;

            // ========== 5. 描述区域 ==========
            var descPanel = new Panel
            {
                Name = "descPanel",
                Location = new Point(currentX, _padding),
                Size = new Size(_descWidth, _cardHeight - _padding * 2),
                BackColor = Color.Transparent
            };

            var lblDesc = new Label
            {
                Name = "lblDesc",
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 8f, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(Dpi(4), Dpi(4)),
                Size = new Size(_descWidth - Dpi(8), _cardHeight - _padding * 2 - Dpi(8)),
                AutoEllipsis = true
            };
            descPanel.Controls.Add(lblDesc);
            card.Controls.Add(descPanel);

            // 绑定点击事件
            card.Click += Card_Click;
            card.DoubleClick += Card_DoubleClick;
            BindClickEvents(card, tierPanel);
            BindClickEvents(card, namePanel);
            BindClickEvents(card, statsPanel);
            BindClickEvents(card, descPanel);

            return card;
        }

        /// <summary>
        /// 更新卡片内容（复用卡片）
        /// </summary>
        private void UpdateCardContent(Panel card, RecommendedLineUp lineUp)
        {
            card.Tag = lineUp;
            card.BackColor = Color.FromArgb(50, 50, 53);

            // 更新评级
            var tierPanel = card.Controls.Find("tierPanel", false).FirstOrDefault() as Panel;
            if (tierPanel != null)
            {
                var lblTier = tierPanel.Controls.Find("lblTier", false).FirstOrDefault() as Label;
                if (lblTier != null)
                {
                    lblTier.Text = lineUp.GetTierDisplayText();
                    lblTier.ForeColor = GetTierColor(lineUp.Tier);
                }
            }

            // 更新名称和标签
            var namePanel = card.Controls.Find("namePanel", false).FirstOrDefault() as Panel;
            if (namePanel != null)
            {
                var lblName = namePanel.Controls.Find("lblName", false).FirstOrDefault() as Label;
                if (lblName != null)
                {
                    lblName.Text = lineUp.LineUpName;
                }

                var lblTags = namePanel.Controls.Find("lblTags", false).FirstOrDefault() as Label;
                if (lblTags != null)
                {
                    lblTags.Text = lineUp.Tags.Count > 0 ? string.Join(" ", lineUp.Tags) : "";
                    lblTags.Visible = lineUp.Tags.Count > 0;
                }
            }

            // 更新统计数据
            var statsPanel = card.Controls.Find("statsPanel", false).FirstOrDefault() as Panel;
            if (statsPanel != null)
            {
                var lblWinRate = statsPanel.Controls.Find("lblWinRate", false).FirstOrDefault() as Label;
                if (lblWinRate != null) lblWinRate.Text = $"胜率\n{lineUp.WinRate:F1}%";

                var lblTopFour = statsPanel.Controls.Find("lblTopFour", false).FirstOrDefault() as Label;
                if (lblTopFour != null) lblTopFour.Text = $"前四率\n{lineUp.TopFourRate:F1}%";

                var lblPickRate = statsPanel.Controls.Find("lblPickRate", false).FirstOrDefault() as Label;
                if (lblPickRate != null) lblPickRate.Text = $"选取率\n{lineUp.PickRate:F1}%";

                var lblRank = statsPanel.Controls.Find("lblRank", false).FirstOrDefault() as Label;
                if (lblRank != null) lblRank.Text = $"平均名次\n{lineUp.AverageRank:F2}";
            }

            // 更新英雄展示区域
            var heroPanel = card.Controls.Find("heroPanel", false).FirstOrDefault() as FlowLayoutPanel;
            if (heroPanel != null)
            {
                UpdateHeroPanel(heroPanel, lineUp);
            }

            // 更新描述
            var descPanel = card.Controls.Find("descPanel", false).FirstOrDefault() as Panel;
            if (descPanel != null)
            {
                bool hasDescription = !string.IsNullOrWhiteSpace(lineUp.Description);
                descPanel.Visible = hasDescription;

                var lblDesc = descPanel.Controls.Find("lblDesc", false).FirstOrDefault() as Label;
                if (lblDesc != null)
                {
                    lblDesc.Text = lineUp.Description;
                }

                // 动态调整英雄区域宽度
                if (heroPanel != null)
                {
                    int currentX = _tierWidth + _nameWidth + _statsWidth + _padding * 4;
                    int heroAreaWidth = hasDescription
                        ? _cardWidth - currentX - _descWidth - _padding * 2
                        : _cardWidth - currentX - _padding;
                    heroPanel.Size = new Size(heroAreaWidth, _cardHeight - _padding * 2);
                }
            }
        }

        /// <summary>
        /// 更新英雄面板（使用对象池）
        /// </summary>
        private void UpdateHeroPanel(FlowLayoutPanel heroPanel, RecommendedLineUp lineUp)
        {
            // 按英雄费用升序排序（低费用在左边，高费用在右边）
            List<LineUpUnit> sortedUnits = lineUp.LineUpUnits
                .OrderBy(unit => _heroDataService.GetHeroFromName(unit.HeroName)?.Cost ?? 99)
                .ToList();

            int heroCount = sortedUnits.Count;

            // 确保有足够的 HeroAndEquipmentPictureBox
            while (heroPanel.Controls.Count < heroCount)
            {
                var heroPicBox = new HeroAndEquipmentPictureBox
                {
                    Size = new Size(_heroSize, _cardHeight - _padding * 2),
                    Margin = new Padding(0, 0, Dpi(2), 0)
                };
                heroPanel.Controls.Add(heroPicBox);
            }

            // 更新每个英雄框
            for (int i = 0; i < heroCount; i++)
            {
                var unit = sortedUnits[i];
                var hero = _heroDataService.GetHeroFromName(unit.HeroName);

                if (heroPanel.Controls[i] is HeroAndEquipmentPictureBox heroPicBox)
                {
                    heroPicBox.Visible = true;

                    if (hero != null)
                    {
                        Equipment? equip1 = null, equip2 = null, equip3 = null;
                        if (unit.EquipmentNames != null && unit.EquipmentNames.Length > 0)
                        {
                            if (unit.EquipmentNames.Length > 0 && !string.IsNullOrEmpty(unit.EquipmentNames[0]))
                                equip1 = _equipmentService.GetEquipmentFromName(unit.EquipmentNames[0]);
                            if (unit.EquipmentNames.Length > 1 && !string.IsNullOrEmpty(unit.EquipmentNames[1]))
                                equip2 = _equipmentService.GetEquipmentFromName(unit.EquipmentNames[1]);
                            if (unit.EquipmentNames.Length > 2 && !string.IsNullOrEmpty(unit.EquipmentNames[2]))
                                equip3 = _equipmentService.GetEquipmentFromName(unit.EquipmentNames[2]);
                        }
                        SetHeroAndEquipment(heroPicBox, hero, equip1, equip2, equip3);
                    }
                }
            }

            // 隐藏多余的英雄框
            for (int i = heroCount; i < heroPanel.Controls.Count; i++)
            {
                heroPanel.Controls[i].Visible = false;
            }
        }

        /// <summary>
        /// 为面板及其子控件绑定点击事件
        /// </summary>
        private void BindClickEvents(Panel card, Panel panel)
        {
            panel.Click += (s, e) => Card_Click(card, e);
            panel.DoubleClick += (s, e) => Card_DoubleClick(card, e);
            foreach (Control ctrl in panel.Controls)
            {
                ctrl.Click += (s, e) => Card_Click(card, e);
                ctrl.DoubleClick += (s, e) => Card_DoubleClick(card, e);
            }
        }

        /// <summary>
        /// 设置 HeroAndEquipmentPictureBox 的英雄和装备（简化版，不需要 UIBuilderService）
        /// </summary>
        private void SetHeroAndEquipment(HeroAndEquipmentPictureBox picBox, Hero hero, Equipment? equip1, Equipment? equip2, Equipment? equip3)
        {
            // 获取内部控件
            var heroPicBox = picBox.Controls.Find("heroPictureBox", true).FirstOrDefault() as HeroPictureBox;
            var equipPicBox1 = picBox.Controls.Find("equipmentPictureBox1", true).FirstOrDefault() as HeroPictureBox;
            var equipPicBox2 = picBox.Controls.Find("equipmentPictureBox2", true).FirstOrDefault() as HeroPictureBox;
            var equipPicBox3 = picBox.Controls.Find("equipmentPictureBox3", true).FirstOrDefault() as HeroPictureBox;

            if (heroPicBox != null)
            {
                heroPicBox.Image = hero.Image;
                heroPicBox.Tag = hero;
                heroPicBox.BorderColor = GetHeroCostColor(hero.Cost);
            }

            if (equipPicBox1 != null)
            {
                equipPicBox1.Image = equip1?.Image;
                equipPicBox1.Tag = equip1;
                equipPicBox1.BorderWidth = equip1 == null ? 1 : 0;
                equipPicBox1.BorderColor = Color.Gray;
            }

            if (equipPicBox2 != null)
            {
                equipPicBox2.Image = equip2?.Image;
                equipPicBox2.Tag = equip2;
                equipPicBox2.BorderWidth = equip2 == null ? 1 : 0;
                equipPicBox2.BorderColor = Color.Gray;
            }

            if (equipPicBox3 != null)
            {
                equipPicBox3.Image = equip3?.Image;
                equipPicBox3.Tag = equip3;
                equipPicBox3.BorderWidth = equip3 == null ? 1 : 0;
                equipPicBox3.BorderColor = Color.Gray;
            }
        }

        /// <summary>
        /// 根据英雄费用获取边框颜色
        /// </summary>
        private Color GetHeroCostColor(int cost)
        {
            return cost switch
            {
                1 => Color.FromArgb(128, 128, 128),   // 灰色
                2 => Color.FromArgb(0, 128, 0),       // 绿色
                3 => Color.FromArgb(0, 112, 192),     // 蓝色
                4 => Color.FromArgb(128, 0, 128),     // 紫色
                5 => Color.FromArgb(255, 215, 0),     // 金色
                _ => Color.Gray
            };
        }

        /// <summary>
        /// 根据等级获取颜色
        /// </summary>
        private Color GetTierColor(LineUpTier tier)
        {
            return tier switch
            {
                LineUpTier.S => Color.FromArgb(255, 215, 0),     // 金色
                LineUpTier.A => Color.FromArgb(138, 43, 226),    // 紫色
                LineUpTier.B => Color.FromArgb(60, 179, 113),    // 绿色
                LineUpTier.C => Color.FromArgb(100, 149, 237),   // 蓝色
                LineUpTier.D => Color.FromArgb(169, 169, 169),   // 深灰色
                _ => Color.White
            };
        }

        /// <summary>
        /// 卡片点击事件
        /// </summary>
        private void Card_Click(object? sender, EventArgs e)
        {
            if (sender is not Panel clickedCard) return;

            // 取消之前的选择
            if (_selectedCard != null)
            {
                _selectedCard.BackColor = Color.FromArgb(50, 50, 53);
            }
            
            // 设置新选择
            _selectedCard = clickedCard;
            _selectedCard.BackColor = Color.FromArgb(104, 142, 45);
            SelectedLineUp = clickedCard.Tag as RecommendedLineUp;
            button_Confirm.BackColor = Color.FromArgb(130, 189, 39);
            button_Confirm.Enabled = true;
            // 更新信息标签
            if (SelectedLineUp != null)
            {
                label_Info.Text = $"已选择: {SelectedLineUp.LineUpName} ({SelectedLineUp.LineUpUnits.Count}人口)";
            }
        }

        /// <summary>
        /// 卡片双击事件 - 快速确认
        /// </summary>
        private void Card_DoubleClick(object? sender, EventArgs e)
        {
            if (sender is Panel clickedCard)
            {
                SelectedLineUp = clickedCard.Tag as RecommendedLineUp;
                if (SelectedLineUp != null)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 筛选下拉框选择变更事件
        /// </summary>
        private void comboBox_TierFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// 排序下拉框选择变更事件
        /// </summary>
        private void comboBox_SortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// 搜索框文本变更事件（使用防抖）
        /// </summary>
        private void textBox_Search_TextChanged(object sender, EventArgs e)
        {
            ApplyFilterWithDebounce();
        }

        /// <summary>
        /// 确认按钮点击事件
        /// </summary>
        private void button_Confirm_Click(object sender, EventArgs e)
        {            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            SelectedLineUp = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
