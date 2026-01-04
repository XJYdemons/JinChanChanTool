using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using System.Diagnostics;
using Windows.AI.MachineLearning;
using YamlDotNet.Core.Tokens;
using static JinChanChanTool.DataClass.LineUp;
namespace JinChanChanTool.Services
{
    /// <summary>
    /// UI构建服务
    /// </summary>
    public class UIBuilderService
    {
        #region 主窗口组件相关
        /// <summary>
        /// 主窗口英雄头像框列表
        /// </summary>
        public List<HeroPictureBox> MainForm_HeroPictureBoxes { get; }

        /// <summary>
        /// 主窗口英雄复选框列表
        /// </summary>
        public List<CheckBox> MainForm_CheckBoxes { get; }

        /// <summary>
        /// 主窗口按职业选择英雄按钮列表
        /// </summary>
        public List<Button> MainForm_ProfessionButtons { get; }

        /// <summary>
        /// 主窗口按特质选择英雄按钮列表
        /// </summary>
        public List<Button> MainForm_PeculiarityButtons { get; }

        /// <summary>
        /// 最大英雄选择数量
        /// </summary>
        int MaxHerosCount ;

        /// <summary>
        /// 主窗口英雄与装备图片框列表
        /// </summary>
        public List<HeroAndEquipmentPictureBox> MainForm_HeroAndEquipmentPictureBoxes { get; set; }

        /// <summary>
        /// 英雄名到主窗口英雄复选框的映射字典
        /// </summary>
        private Dictionary<string, CheckBox> nameToCheckBoxMap { get; set; }

        /// <summary>
        /// 主窗口存放英雄选择器、按职业和特质选择英雄按钮的容器
        /// </summary>
        private readonly TabControl _tabControl_HeroSelector;

        /// <summary>
        /// 存放不同费用英雄选择器的面板列表
        /// </summary>
        private List<Panel> costPanels { get; set; }

        /// <summary>
        /// 存放按职业选择英雄按钮的面板
        /// </summary>
        private Panel _professionButtonPanel;

        /// <summary>
        /// 存放按特质选择英雄按钮的面板
        /// </summary>
        private Panel _peculiarityButtonPanel;

        /// <summary>
        /// 主窗口中展示阵容的容器
        /// </summary>
        private CustomFlowLayoutPanel _lineUpPanel;

        //
        //主窗口英雄选择器常量
        //
        private const int MainForm_HeroSelectorColumns = 5; //每行英雄选择器数量
        private const int MainForm_HeroSelectorHorizontalSpacing = 4; //英雄选择器之间的水平间距
        private const int MainForm_HeroSelectorVerticalSpacing = 1; // 英雄选择器之间的垂直间距
        private static readonly Size MainForm_HeroPictureBoxSize = new Size(48, 48);//单个英雄选择器中图像框大小
        private static readonly Size MainForm_LabelSize = new Size(67, 19);//单个英雄选择器中名称标签大小
        private static readonly Size MainForm_CheckBoxSize = new Size(14, 14);//单个复选框大小

        //
        //主窗口按职业与特质选择英雄按钮常量
        //
        private const int MainForm_ButtonColumns = 2; //每行按钮数量
        private const int MainForm_ButtonHorizontalSpacing = 1; //按钮之间的水平间距
        private const int MainForm_ButtonVerticalSpacing = 10; // 按钮之间的垂直间距
        private static readonly Size MainForm_ProfessionAndPeculiarityButtonSize = new Size(83, 23);//单个按钮的大小
       
        #endregion

        #region 英雄选择面板相关
        /// <summary>
        /// 英雄选择面板列表
        /// </summary>
        private List<FlowLayoutPanel> SelectForm_HeroPanels { get; set; }

        /// <summary>
        /// 英雄选择面板中的英雄头像框列表
        /// </summary>
        public List<HeroPictureBox> SelectForm_HeroPictureBoxes { get; }

        //
        //半透明英雄选择面板常量
        //
        private Size SelectForm_HeroPictureBoxSize;//单个英雄选择器中图像框大小
        private int SelectForm_HeroPictureBoxHorizontalSpacing; //英雄选择器之间的水平间距
        private int SelectForm_HeroPanelsVerticalSpacing;//每个费用面板之间的垂直间距        
       
        private const int SelectForm_BackgroundPanelbottomPadding = 2; //背景面板底部内边距
        private const int SelectForm_BackgroundPanelRightPadding = 2; //背景面板右侧内边距
        private const int SelectForm_FormRightPadding = 5;//透明窗体右侧内边距
        private const int SelectForm_FormbottomPadding =5;//透明窗体底部内边距
        #endregion

        #region 阵容窗口       
        private static readonly Size LineUpForm_HeroPictureBoxSize = new Size(28, 28);//单个英雄选择器中图像框大小

        /// <summary>
        /// 阵容窗口展示阵容的容器
        /// </summary>
        public CustomFlowLayoutPanel LineUpForm_LineUpPanel;

        /// <summary>
        /// LineUpForm窗口中的英雄与装备图片框列表
        /// </summary>
        public List<HeroAndEquipmentPictureBox> LineUpForm_HeroAndEquipmentPictureBoxes { get; set; }
        #endregion

        /// <summary>
        /// 主窗口实例
        /// </summary>
        private readonly MainForm _mainForm;

        /// <summary>
        /// 英雄数据服务对象
        /// </summary>
        private readonly IHeroDataService _iHeroDataService; 
               
        /// <summary>
        /// 用户应用设置数据服务对象
        /// </summary>
        private readonly IManualSettingsService _iManualSettingsService;

        public UIBuilderService(IHeroDataService iHeroDataService, IManualSettingsService iManualSettingsService, MainForm mainForm,TabControl tabControl_HeroSelector, CustomFlowLayoutPanel subLineUpPanel1,CustomFlowLayoutPanel LineUpPanel1,int maxHeroCount)
        {            
            MainForm_HeroPictureBoxes = new List<HeroPictureBox>();
            MainForm_CheckBoxes = new List<CheckBox>();
            MainForm_ProfessionButtons = new List<Button>();
            MainForm_PeculiarityButtons = new List<Button>();
            MaxHerosCount = maxHeroCount;
            MainForm_HeroAndEquipmentPictureBoxes = new List<HeroAndEquipmentPictureBox>();
            nameToCheckBoxMap = new Dictionary<string, CheckBox>();
            _tabControl_HeroSelector = tabControl_HeroSelector;                       
            costPanels = new List<Panel>();
            _lineUpPanel = subLineUpPanel1;

            SelectForm_HeroPanels = new List<FlowLayoutPanel>();
            SelectForm_HeroPictureBoxes = new List<HeroPictureBox>();           

            LineUpForm_LineUpPanel = LineUpPanel1;
            LineUpForm_HeroAndEquipmentPictureBoxes = new List<HeroAndEquipmentPictureBox>();

            _mainForm = mainForm;
            _iHeroDataService = iHeroDataService;
            _iManualSettingsService = iManualSettingsService;
        }

        /// <summary>
        /// 针对SelectForm的DPI缩放转换函数
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int Dpi_S(int i)
        {
            return SelectForm.Instance.LogicalToDeviceUnits(i);
        }

        /// <summary>
        /// 针对MainForm的DPI缩放转换函数
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int Dpi_M(int i)
        {
            return _mainForm.LogicalToDeviceUnits(i);
        }

        /// <summary>
        /// 针对LineUpForm的DPI缩放转换函数
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int Dpi_L(int i)
        {
            return LineUpForm.Instance.LogicalToDeviceUnits(i);
        }

        #region 创建主窗口英雄选择器
        /// <summary>
        /// 创建英雄选择分页
        /// </summary>
        public void MainForm_CreatTabPages()
        {
            foreach (TabPage tabPage in _tabControl_HeroSelector.TabPages.OfType<TabPage>().ToList())
            {
                tabPage.Dispose();
            }
            _tabControl_HeroSelector.TabPages.Clear();
            costPanels = new List<Panel>();
            _professionButtonPanel = null;
            _peculiarityButtonPanel = null;

            List<int> costTypeList = _iHeroDataService.GetCostType();
            for (int i = 0; i < costTypeList.Count; i++)
            {
                Panel panel_Background = new Panel();
                panel_Background.AutoScroll = true;
                panel_Background.BackColor = Color.FromArgb(255, 255, 255);
                panel_Background.Dock = DockStyle.Fill;
                panel_Background.Margin = new Padding(0);
                panel_Background.Name = $"panel_{costTypeList[i]}Cost";
                panel_Background.Tag = costTypeList[i];
                costPanels.Add(panel_Background);
                TabPage tabPage = new TabPage();
                tabPage.SuspendLayout();
                tabPage.BackColor = Color.White;
                tabPage.Controls.Add(panel_Background);
                tabPage.Name = $"tabPage_{costTypeList[i]}Cost";
                tabPage.Padding = new Padding(3);
                tabPage.Margin = new Padding(3);
                tabPage.Text = $"{costTypeList[i]}费";
                tabPage.Tag = costTypeList[i];
                _tabControl_HeroSelector.TabPages.Add(tabPage);
                tabPage.ResumeLayout(false);

            }
            Label label_职业 = new Label();
            label_职业.Dock = DockStyle.Top;
            label_职业.Location = new Point(0, 0);
            label_职业.Name = "label_职业";
            label_职业.Size = new Size(Dpi_M(184), Dpi_M(21));
            label_职业.Text = "--------- 职业 ---------";
            label_职业.TextAlign = ContentAlignment.MiddleCenter;

            Panel panel_SelectByProfession = new Panel();
            panel_SelectByProfession.SuspendLayout();
            panel_SelectByProfession.AutoScroll = true;
            panel_SelectByProfession.Controls.Add(label_职业);
            panel_SelectByProfession.Dock = DockStyle.Left;
            panel_SelectByProfession.Location = new Point(Dpi_M(3), Dpi_M(3));
            panel_SelectByProfession.Name = "panel_SelectByProfession";
            panel_SelectByProfession.Size = new Size(Dpi_M(184), Dpi_M(259));
            panel_SelectByProfession.ResumeLayout(false);
            _professionButtonPanel = panel_SelectByProfession;

            Label label_特质 = new Label();
            label_特质.Dock = DockStyle.Top;
            label_特质.Location = new Point(0, 0);
            label_特质.Name = "label_特质";
            label_特质.Size = new Size(Dpi_M(184), Dpi_M(21));
            label_特质.Text = "--------- 特质 ---------";
            label_特质.TextAlign = ContentAlignment.MiddleCenter;

            Panel panel_SelectByPeculiarity = new Panel();
            panel_SelectByPeculiarity.SuspendLayout();
            panel_SelectByPeculiarity.AutoScroll = true;
            panel_SelectByPeculiarity.Controls.Add(label_特质);
            panel_SelectByPeculiarity.Dock = DockStyle.Right;
            panel_SelectByPeculiarity.Location = new Point(Dpi_M(199), Dpi_M(3));
            panel_SelectByPeculiarity.Name = "panel_SelectByPeculiarity";
            panel_SelectByPeculiarity.Size = new Size(Dpi_M(184), Dpi_M(259));
            panel_SelectByPeculiarity.ResumeLayout(false);
            _peculiarityButtonPanel = panel_SelectByPeculiarity;

            TabPage tabPage_SelectByProfessionAndPeculiarity = new TabPage();
            tabPage_SelectByProfessionAndPeculiarity.SuspendLayout();
            tabPage_SelectByProfessionAndPeculiarity.BackColor = Color.White;
            tabPage_SelectByProfessionAndPeculiarity.Controls.Add(panel_SelectByProfession);
            tabPage_SelectByProfessionAndPeculiarity.Controls.Add(panel_SelectByPeculiarity);
            tabPage_SelectByProfessionAndPeculiarity.Name = "tabPage_SelectByProfessionAndPeculiarity";
            tabPage_SelectByProfessionAndPeculiarity.Padding = new Padding(3);
            tabPage_SelectByProfessionAndPeculiarity.Margin = new Padding(3);
            tabPage_SelectByProfessionAndPeculiarity.Text = "按职业和特质选择";
            tabPage_SelectByProfessionAndPeculiarity.Tag = 10000;
            _tabControl_HeroSelector.TabPages.Add(tabPage_SelectByProfessionAndPeculiarity);
            tabPage_SelectByProfessionAndPeculiarity.ResumeLayout(false);


        }

        /// <summary>
        /// 创建英雄选择器组（按费用分组）
        /// </summary>
        public void MainForm_CreateHeroSelectors()
        {
            MainForm_HeroPictureBoxes.Clear();
            MainForm_CheckBoxes.Clear();
            nameToCheckBoxMap.Clear();
            for (int i = 0; i < costPanels.Count; i++)
            {
                CreateHeroSelectorGroup(costPanels[i], _iHeroDataService.GetHeroDatasFromCost(Convert.ToInt32(costPanels[i].Tag)));
            }
        }

        /// <summary>
        /// 创建单个费用组的英雄选择器
        /// </summary>
        private void CreateHeroSelectorGroup(Panel costPanel, List<Hero> heroes)
        {
            const int startX = 0;//起始X坐标
            const int startY = 0;//起始Y坐标
            int currentX = startX;//当前X坐标
            int currentY = startY;//当前Y坐标
            int columnCount = 0;//当前列数

            // 创建每个英雄的选择器
            foreach (var hero in heroes)
            {
                // 创建控件
                var pictureBox = CreatPictureBox(hero);
                var label = CreatLabel(hero);
                var checkBox = CreatCheckBox(hero);

                // 计算控件组的总宽度（取三者中最宽的值）
                int groupWidth = Math.Max(pictureBox.Width, Math.Max(label.Width, checkBox.Width));

                // 设置水平居中
                pictureBox.Location = new Point(currentX + (groupWidth - pictureBox.Width) / 2, currentY);
                label.Location = new Point(currentX + (groupWidth - label.Width) / 2, currentY + pictureBox.Height);
                checkBox.Location = new Point(
                    currentX + (groupWidth - checkBox.Width) / 2,
                    currentY + pictureBox.Height + label.Height);

                costPanel.Controls.Add(pictureBox);
                costPanel.Controls.Add(label);
                costPanel.Controls.Add(checkBox);
                MainForm_HeroPictureBoxes.Add(pictureBox);
                MainForm_CheckBoxes.Add(checkBox);

                // 更新位置
                currentX += groupWidth + MainForm_HeroSelectorHorizontalSpacing;
                columnCount++;

                // 换行处理
                if (columnCount >= MainForm_HeroSelectorColumns)
                {
                    columnCount = 0;
                    currentX = startX;
                    currentY += pictureBox.Height + label.Height + checkBox.Height + MainForm_HeroSelectorVerticalSpacing;
                }
            }
        }

        /// <summary>
        /// 创建标签函数
        /// </summary>
        private Label CreatLabel(Hero hero)
        {
            Label label = new Label();
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Font = new Font("Microsoft YaHei UI", 7F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label.Text = hero.HeroName;
            label.Size = _mainForm.LogicalToDeviceUnits(MainForm_LabelSize);
            label.ForeColor = GetColor(hero.Cost);
            return label;
        }

        /// <summary>
        /// 创建CheckBox函数
        /// </summary>
        private CheckBox CreatCheckBox(Hero hero)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.UseVisualStyleBackColor = true;
            checkBox.TabStop = false;
            checkBox.FlatStyle = FlatStyle.Flat;
            checkBox.Size = _mainForm.LogicalToDeviceUnits(MainForm_CheckBoxSize);
            checkBox.Tag = hero.HeroName;
            nameToCheckBoxMap[hero.HeroName] = checkBox;
            return checkBox;
        }

        /// <summary>
        /// 创建HeroPictureBox函数
        /// </summary>
        private HeroPictureBox CreatPictureBox(Hero hero)
        {
            HeroPictureBox pictureBox = new HeroPictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            pictureBox.BackColor = SystemColors.Control;
            pictureBox.BorderWidth = 2;
            pictureBox.Size = _mainForm.LogicalToDeviceUnits(MainForm_HeroPictureBoxSize);
            pictureBox.Image =/* _iHeroDataService.GetImageFromHero(hero);*/hero.Image;
            pictureBox.Tag = hero.HeroName;
            pictureBox.BorderColor = GetColor(hero.Cost);
            return pictureBox;
        }
        #endregion

        #region 创建职业与特质按钮
        /// <summary>
        /// 创建职业与特质按钮
        /// </summary>
        public void MainForm_CreateProfessionAndPeculiarityButtons()
        {
            MainForm_ProfessionButtons.Clear();
            MainForm_PeculiarityButtons.Clear();
            // 创建职业按钮
            CreateButtonGroup(_professionButtonPanel, _iHeroDataService.GetProfessions(), MainForm_ProfessionButtons);

            // 创建特质按钮
            CreateButtonGroup(_peculiarityButtonPanel, _iHeroDataService.GetPeculiarities(), MainForm_PeculiarityButtons);
        }

        /// <summary>
        /// 创建按钮组（职业或特质）
        /// </summary>
        /// <param name="panel">面板容器</param>
        /// <param name="items">按钮数据列表</param>
        /// <param name="buttonList">按钮列表</param>
        private void CreateButtonGroup<T>(Panel panel, List<T> items, List<Button> buttonList)
        {
            // 清空面板和列表
            buttonList.Clear();

            const int startX = 0; // 起始X坐标
            const int startY = 22 + 10; // 起始Y坐标
            int currentX = startX;//当前X坐标
            int currentY = startY;//当前Y坐标
            int columnCount = 0;//当前列数

            // 创建每个按钮
            for (int i = 0; i < items.Count; i++)
            {
                dynamic item = items[i];
                Button button = CreatButton(new Point(Dpi_M(currentX), Dpi_M(currentY)), item.Title, item);

                // 添加到面板和列表
                panel.Controls.Add(button);
                buttonList.Add(button);

                // 更新位置（使用逻辑像素计算，最后转换）
                columnCount++;
                currentX += MainForm_ProfessionAndPeculiarityButtonSize.Width + MainForm_ButtonHorizontalSpacing;

                // 换行处理
                if (columnCount >= MainForm_ButtonColumns)
                {
                    columnCount = 0;
                    currentX = startX;
                    currentY += MainForm_ProfessionAndPeculiarityButtonSize.Height + MainForm_ButtonVerticalSpacing;
                }
            }
        }

        /// <summary>
        /// 创建单个按钮
        /// </summary>
        /// <returns></returns>
        private Button CreatButton(Point location, string text, object? tag)
        {
            Button button = new Button();
            button.TabStop = false;
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.Transparent;
            button.FlatAppearance.BorderColor = Color.Gray;
            button.Font = new Font("Microsoft YaHei UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button.ForeColor = SystemColors.ControlText;
            button.Margin = new Padding(0, 0, 0, 0);
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Size = new Size(Dpi_M(MainForm_ProfessionAndPeculiarityButtonSize.Width), Dpi_M(MainForm_ProfessionAndPeculiarityButtonSize.Height));
            button.Location = location;
            button.Text = text;
            button.Tag = tag;
            return button;
        }

        #endregion

        #region 创建主窗口阵容英雄头像框
        /// <summary>
        /// 创建单个英雄与装备图片框
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private HeroAndEquipmentPictureBox CreatHeroAndEquipmentPicturebox(Panel parentPanel)
        {
            HeroAndEquipmentPictureBox heroAndEquipmentPictureBox = new HeroAndEquipmentPictureBox();
            heroAndEquipmentPictureBox.Size = new(Dpi_M(62), Dpi_M(62));
            heroAndEquipmentPictureBox.Margin = new Padding(7); // 设置间距
            parentPanel.Controls.Add(heroAndEquipmentPictureBox);
            return heroAndEquipmentPictureBox;
        }

        /// <summary>
        /// 批量创建子阵容头像框
        /// </summary>
        public void MainForm_CreatHeroAndEquipmentPictureboxes()
        {

            for (int i = 0; i < MaxHerosCount; i++)
            {

                MainForm_HeroAndEquipmentPictureBoxes.Add(CreatHeroAndEquipmentPicturebox(_lineUpPanel));

            }
        }       
        #endregion

        #region 创建英雄选择窗口英雄头像框
        /// <summary>
        /// 创建英雄选择面板
        /// </summary>
        public void SelectForm_CreatFlowLayoutPanel()
        {
            //移除原来的英雄选择面板
            while (SelectForm.Instance.panel_Background.Controls.Count > 0)
            {
                Control control = SelectForm.Instance.panel_Background.Controls[0];
                SelectForm.Instance.panel_Background.Controls.Remove(control);
                control.Dispose();
            }
            SelectForm_HeroPanels = new List<FlowLayoutPanel>();

            List<int> costTypeList = _iHeroDataService.GetCostType();
            int costTypeCount = costTypeList.Count;
            int MaxHeroCountOfOneType = 0;
            foreach(int i in costTypeList)
            {
                int count = _iHeroDataService.GetHeroDatasFromCost(i).Count;
                if (count > MaxHeroCountOfOneType) MaxHeroCountOfOneType = count;
            }

            // 使用Dpi()方法进行DPI缩放转换
            int pictureBoxSize = Dpi_S(_iManualSettingsService.CurrentConfig.SelectFormHeroPictureBoxSize);
            SelectForm_HeroPictureBoxSize = new Size(pictureBoxSize, pictureBoxSize);
            SelectForm_HeroPictureBoxHorizontalSpacing = Dpi_S(_iManualSettingsService.CurrentConfig.SelectFormHeroPictureBoxHorizontalSpacing);
            SelectForm_HeroPanelsVerticalSpacing = Dpi_S(_iManualSettingsService.CurrentConfig.SelectFormHeroPanelsVerticalSpacing);
           

            // 对常量padding值也进行DPI转换
            int bottomPadding = Dpi_S(SelectForm_BackgroundPanelbottomPadding);
            int rightPadding = Dpi_S(SelectForm_BackgroundPanelRightPadding);
            int formRightPadding = Dpi_S(SelectForm_FormRightPadding);
            int formBottomPadding = Dpi_S(SelectForm_FormbottomPadding);

            int heroPanelHeight = SelectForm_HeroPictureBoxSize.Height;
            int newDraggingBarHeight = costTypeCount * (heroPanelHeight + SelectForm_HeroPanelsVerticalSpacing) - SelectForm_HeroPanelsVerticalSpacing;
            int backgroundPanelHeight = costTypeCount * (heroPanelHeight + SelectForm_HeroPanelsVerticalSpacing) - SelectForm_HeroPanelsVerticalSpacing + bottomPadding;
            int backgroundPanelWidth = MaxHeroCountOfOneType * (SelectForm_HeroPictureBoxSize.Width + SelectForm_HeroPictureBoxHorizontalSpacing) - SelectForm_HeroPictureBoxHorizontalSpacing + rightPadding;
            int formWidth = backgroundPanelWidth + formRightPadding;
            int formHeight = backgroundPanelHeight + formBottomPadding;

            SelectForm.Instance.Size = new Size(formWidth, formHeight);            
            SelectForm.Instance.panel_Background.Location = new Point(0, 0);
            SelectForm.Instance.panel_Background.Size = new Size(backgroundPanelWidth, backgroundPanelHeight);
            //SelectForm.Instance.BackColor = Color.Green;
            //SelectForm.Instance.panel_Background.BackColor = Color.Blue;

            for (int i = 0; i < costTypeList.Count; i++)
            {

                FlowLayoutPanel heroPanel = new FlowLayoutPanel();
                heroPanel.Location = new Point(1, 1+i * (heroPanelHeight + SelectForm_HeroPanelsVerticalSpacing));
                heroPanel.Size = new Size(MaxHeroCountOfOneType * (SelectForm_HeroPictureBoxSize.Width + SelectForm_HeroPictureBoxHorizontalSpacing) - SelectForm_HeroPictureBoxHorizontalSpacing, heroPanelHeight);
                heroPanel.AutoSize = false;


                heroPanel.Margin = new Padding(0);
                heroPanel.Name = $"flowLayoutPanel{costTypeList[i]}";
                heroPanel.WrapContents = false;
                heroPanel.Tag = costTypeList[i];
                heroPanel.BackColor = Color.Transparent /*Color.FromArgb(255-(i*40),20*i, 20 * i)*/;
                SelectForm.Instance.panel_Background.Controls.Add(heroPanel);
                SelectForm_HeroPanels.Add(heroPanel);
            }
        }

        /// <summary>
        /// 创建英雄选择器组（按费用分组）
        /// </summary>
        public void SelectForm_CreateHeroPictureBox()
        {
            SelectForm_HeroPictureBoxes.Clear();
            for (int i = 0; i < SelectForm_HeroPanels.Count; i++)
            {
                CreateTransparentHeroPictureBoxGroup(SelectForm_HeroPanels[i], _iHeroDataService.GetHeroDatasFromCost(Convert.ToInt32(SelectForm_HeroPanels[i].Tag)));
            }
        }

        /// <summary>
        /// 创建单个费用组的半透明英雄头像框
        /// </summary>
        /// <param name="heroPanel"></param>
        /// <param name="heroes"></param>
        private void CreateTransparentHeroPictureBoxGroup(FlowLayoutPanel heroPanel, List<Hero> heroes)
        {
            FlowLayoutPanel _heroPanel = heroPanel;
            if (_heroPanel == null) return;
            // 清空面板
            _heroPanel.Controls.Clear();

            foreach (var hero in heroes)
            {
                // 创建控件
                var pictureBox = CreateTransparentPictureBox(hero);

                _heroPanel.Controls.Add(pictureBox);

                SelectForm_HeroPictureBoxes.Add(pictureBox);                                            
            }
        }

        /// <summary>
        /// 创建单个半透明英雄头像框
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        private HeroPictureBox CreateTransparentPictureBox(Hero hero)
        {
            HeroPictureBox pictureBox = new HeroPictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            pictureBox.BackColor = SystemColors.Control;
            pictureBox.BorderWidth = 1;
            pictureBox.Size = SelectForm_HeroPictureBoxSize;
            pictureBox.Image = /*_iHeroDataService.GetImageFromHero(hero);*/hero.Image;
            pictureBox.Tag = hero.HeroName;
            pictureBox.BorderColor = GetColor(hero.Cost);
            pictureBox.Padding = new Padding(0);
            pictureBox.Margin = new Padding(0, 0, SelectForm_HeroPictureBoxHorizontalSpacing, 0);
            return pictureBox;
        }
        #endregion
       
        #region 创建阵容窗口阵容英雄头像框
        /// <summary>
        /// 创建单个LineUpForm英雄装备头像框
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <returns></returns>
        private HeroAndEquipmentPictureBox CreatLineUpFormHeroAndEquipmentPicturebox(CustomFlowLayoutPanel parentPanel)
        {
            HeroAndEquipmentPictureBox heroAndEquipmentPictureBox = new HeroAndEquipmentPictureBox();
            heroAndEquipmentPictureBox.Size = new(Dpi_L(62), Dpi_L(62));
            heroAndEquipmentPictureBox.Margin = new Padding(0);
            parentPanel.Controls.Add(heroAndEquipmentPictureBox);
            return heroAndEquipmentPictureBox;
        }

        /// <summary>
        /// 批量创建LineUpForm英雄装备头像框
        /// </summary>
        public void LineUpForm_CreatHeroAndEquipmentPictureboxes()
        {
            for (int i = 0; i < MaxHerosCount; i++)
            {
                LineUpForm_HeroAndEquipmentPictureBoxes.Add(CreatLineUpFormHeroAndEquipmentPicturebox(LineUpForm_LineUpPanel));
            }
        }
        #endregion

        /// <summary>
        /// 在程序启动时构建所有UI组件
        /// </summary>
        public void FirstBuilding()
        {
            MainForm_CreatTabPages();//创建分页
            MainForm_CreateHeroSelectors();//创建主窗口英雄选择器
            MainForm_CreateProfessionAndPeculiarityButtons();//创建主窗口职业与特质按钮
            MainForm_CreatHeroAndEquipmentPictureboxes();//创建主窗口阵容展示UI
            LineUpForm_CreatHeroAndEquipmentPictureboxes();//创建阵容窗口阵容展示UI         
            SelectForm_CreatFlowLayoutPanel();//创建半透明英雄选择面板
            SelectForm_CreateHeroPictureBox();//创建半透明英雄头像框
        }

        /// <summary>
        /// 重新构建UI(排除阵容展示UI的构建)
        /// </summary>
        public void ReBuild()
        {
            MainForm_CreatTabPages();//创建分页
            MainForm_CreateHeroSelectors();//创建主窗口英雄选择器
            MainForm_CreateProfessionAndPeculiarityButtons();//创建主窗口职业与特质按钮                        
            SelectForm_CreatFlowLayoutPanel();//创建半透明英雄选择面板
            SelectForm_CreateHeroPictureBox();//创建半透明英雄头像框 
        }

        public Size GetHeroPictureBoxSize()
        {
            return MainForm_HeroPictureBoxSize;
        }

        public Color GetColor(int cost)
        {
            switch(cost)
            {
                case 1:
                    return Color.FromArgb(107, 104, 101);
                case 2:
                    return Color.FromArgb(5, 171, 117);
                case 3:
                    return Color.FromArgb(0, 133, 255);
                case 4:
                    return Color.FromArgb(175, 40, 195);
                case 5:
                    return Color.FromArgb(245, 158, 11);
                default:
                    return Color.FromArgb(255, 64, 0);
            }
                                 
        }
        
        public CheckBox GetCheckBoxFromName(string name)
        {
            if(nameToCheckBoxMap.ContainsKey(name))
            {
                return nameToCheckBoxMap[name];
            }
            else
            {
                return null;
            }
        }
    }
}
