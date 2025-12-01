using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Forms;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using System.Diagnostics;
using Windows.AI.MachineLearning;

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
        public List<HeroPictureBox> heroPictureBoxes { get; }

        /// <summary>
        /// 主窗口英雄复选框列表
        /// </summary>
        public List<CheckBox> checkBoxes { get; }

        /// <summary>
        /// 主窗口按职业选择英雄按钮列表
        /// </summary>
        public List<Button> professionButtons { get; }

        /// <summary>
        /// 主窗口按特质选择英雄按钮列表
        /// </summary>
        public List<Button> peculiarityButtons { get; }

        /// <summary>
        /// 主窗口子阵容头像框二维数组，第一维表示子阵容索引，第二维表示子阵容中的英雄头像框
        /// </summary>
        public HeroPictureBox[,] subLineUpPictureBoxes { get; set; }

        /// <summary>
        /// 英雄名到主窗口英雄复选框的映射字典
        /// </summary>
        private Dictionary<string, CheckBox> NameToCheckBoxMap { get; set; }

        /// <summary>
        /// 主窗口存放英雄选择器、按职业和特质选择英雄按钮的容器
        /// </summary>
        private readonly TabControl _tabControl_HeroSelector;

        /// <summary>
        /// 存放不同费用英雄选择器的面板列表
        /// </summary>
        private List<Panel> CostPanels { get; set; }

        /// <summary>
        /// 存放按职业选择英雄按钮的面板
        /// </summary>
        private Panel _professionButtonPanel;

        /// <summary>
        /// 存放按特质选择英雄按钮的面板
        /// </summary>
        private Panel _peculiarityButtonPanel;

        /// <summary>
        /// 主窗口子阵容头像框容器面板列表
        /// </summary>
        public List<FlowLayoutPanel> subLineUpPanels { get; }

        //
        //主窗口英雄选择器常量
        //
        private const int HeroSelectorColumns = 5; //每行英雄选择器数量
        private const int HeroSelectorHorizontalSpacing = 4; //英雄选择器之间的水平间距
        private const int HeroSelectorVerticalSpacing = 1; // 英雄选择器之间的垂直间距
        private static readonly Size heroPictureBoxSize = new Size(48, 48);//单个英雄选择器中图像框大小
        private static readonly Size labelSize = new Size(67, 19);//单个英雄选择器中名称标签大小
        private static readonly Size checkBoxSize = new Size(14, 14);//单个复选框大小

        //
        //主窗口按职业与特质选择英雄按钮常量
        //
        private const int columns = 2; //每行按钮数量
        private const int horizontalSpacing = 1; //按钮之间的水平间距
        private const int verticalSpacing = 10; // 按钮之间的垂直间距
        private static readonly Size professionAndPeculiarityButtonSize = new Size(83, 23);//单个按钮的大小

        //
        //主窗口子阵容头像框常量
        //
        private static readonly Size subLineUpPictureBoxSize = new Size(32, 32);//单个子阵容头像框的大小
        #endregion

        #region 半透明英雄选择面板相关
        /// <summary>
        /// 英雄选择面板列表
        /// </summary>
        private List<FlowLayoutPanel> HeroPanels { get; set; }

        /// <summary>
        /// 半透明英雄选择面板中的英雄头像框列表
        /// </summary>
        public List<HeroPictureBox> TransparentheroPictureBoxes { get; }

        //
        //半透明英雄选择面板常量
        //
        private Size transparentHeroPictureBoxSize;//单个英雄选择器中图像框大小
        private int transparentHeroPictureBoxHorizontalSpacing; //英雄选择器之间的水平间距
        private int transparentHeroPanelsVerticalSpacing;//每个费用面板之间的垂直间距        
        private int draggingBarWidth; //拖动条宽度
        private const int transparentBackgroundPanelbottomPadding = 5; //背景面板底部内边距
        private const int transparentBackgroundPanelRightPadding = 5; //背景面板右侧内边距
        private const int transparentFormRightPadding = 5;//透明窗体右侧内边距
        private const int transparentFormbottomPadding =5;//透明窗体底部内边距
        #endregion

        #region 阵容面板       
        private static readonly Size lineUpHeroPictureBoxSize = new Size(28, 28);//单个英雄选择器中图像框大小
        public HeroPictureBox[,] lineUpPictureBoxes { get; set; }
        public List<FlowLayoutPanel> lineUpPanels { get; }
        #endregion

        /// <summary>
        /// 主窗口实例
        /// </summary>
        private readonly MainForm _mainForm;

        /// <summary>
        /// 英雄数据服务实例
        /// </summary>
        private readonly IHeroDataService _iHeroDataService; 
               
        private readonly IManualSettingsService _iManualSettingsService;
        public UIBuilderService(IHeroDataService iHeroDataService, IManualSettingsService iManualSettingsService, MainForm mainForm,TabControl tabControl_HeroSelector, FlowLayoutPanel subLineUpPanel1, FlowLayoutPanel subLineUpPanel2, FlowLayoutPanel subLineUpPanel3,FlowLayoutPanel LineUpPanel1, FlowLayoutPanel LineUpPanel2, FlowLayoutPanel LineUpPanel3)
        {            
            heroPictureBoxes = new List<HeroPictureBox>();
            checkBoxes = new List<CheckBox>();
            professionButtons = new List<Button>();
            peculiarityButtons = new List<Button>();            
            subLineUpPanels = new List<FlowLayoutPanel>();          
            NameToCheckBoxMap = new Dictionary<string, CheckBox>();
            TransparentheroPictureBoxes = new List<HeroPictureBox>();
            lineUpPanels= new List<FlowLayoutPanel>();
            _tabControl_HeroSelector = tabControl_HeroSelector;
            CostPanels = new List<Panel>();
            subLineUpPanels.AddRange(new List<FlowLayoutPanel> { subLineUpPanel1, subLineUpPanel2, subLineUpPanel3 });
            _iHeroDataService = iHeroDataService;
            _iManualSettingsService = iManualSettingsService;
            subLineUpPictureBoxes = new HeroPictureBox[3, _iManualSettingsService.CurrentConfig.MaxHerosCount];
            lineUpPictureBoxes = new HeroPictureBox[3, _iManualSettingsService.CurrentConfig.MaxHerosCount];
            _mainForm = mainForm;
            HeroPanels= new List<FlowLayoutPanel>();           
            lineUpPanels.AddRange(new List<FlowLayoutPanel> { LineUpPanel1, LineUpPanel2, LineUpPanel3 });            
           
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
        #region 创建半透明英雄头像框
        /// <summary>
        /// 创建英雄选择面板
        /// </summary>
        public void CreatFlowLayoutPanel()
        {
            //移除原来的英雄选择面板
            while (SelectForm.Instance.panel_Background.Controls.Count > 0)
            {
                Control control = SelectForm.Instance.panel_Background.Controls[0];
                SelectForm.Instance.panel_Background.Controls.Remove(control);
                control.Dispose();
            }
            HeroPanels = new List<FlowLayoutPanel>();

            List<int> costTypeList = _iHeroDataService.GetCostType();
            int costTypeCount = costTypeList.Count;
            int MaxHeroCountOfOneType = 0;
            foreach(int i in costTypeList)
            {
                int count = _iHeroDataService.GetHeroDatasFromCost(i).Count;
                if (count > MaxHeroCountOfOneType) MaxHeroCountOfOneType = count;
            }

            // 使用Dpi()方法进行DPI缩放转换
            int pictureBoxSize = Dpi_S(_iManualSettingsService.CurrentConfig.TransparentHeroPictureBoxSize);
            transparentHeroPictureBoxSize = new Size(pictureBoxSize, pictureBoxSize);
            transparentHeroPictureBoxHorizontalSpacing = Dpi_S(_iManualSettingsService.CurrentConfig.TransparentHeroPictureBoxHorizontalSpacing);
            transparentHeroPanelsVerticalSpacing = Dpi_S(_iManualSettingsService.CurrentConfig.TransparentHeroPanelsVerticalSpacing);
            draggingBarWidth = Dpi_S(_iManualSettingsService.CurrentConfig.TransparentPanelDraggingBarWidth); //拖动条宽度

            // 对常量padding值也进行DPI转换
            int bottomPadding = Dpi_S(transparentBackgroundPanelbottomPadding);
            int rightPadding = Dpi_S(transparentBackgroundPanelRightPadding);
            int formRightPadding = Dpi_S(transparentFormRightPadding);
            int formBottomPadding = Dpi_S(transparentFormbottomPadding);

            int heroPanelHeight = transparentHeroPictureBoxSize.Height;
            int newDraggingBarHeight = costTypeCount * (heroPanelHeight + transparentHeroPanelsVerticalSpacing) - transparentHeroPanelsVerticalSpacing;
            int backgroundPanelHeight = costTypeCount * (heroPanelHeight + transparentHeroPanelsVerticalSpacing) - transparentHeroPanelsVerticalSpacing + bottomPadding;
            int backgroundPanelWidth = MaxHeroCountOfOneType * (transparentHeroPictureBoxSize.Width + transparentHeroPictureBoxHorizontalSpacing) - transparentHeroPictureBoxHorizontalSpacing + rightPadding;
            int formWidth = draggingBarWidth + backgroundPanelWidth + formRightPadding;
            int formHeight = backgroundPanelHeight + formBottomPadding;

            SelectForm.Instance.Size = new Size(formWidth, formHeight);
            SelectForm.Instance.draggingBar.Size = new Size(draggingBarWidth, newDraggingBarHeight);
            SelectForm.Instance.panel_Background.Location = new Point(draggingBarWidth, 0);
            SelectForm.Instance.panel_Background.Size = new Size(backgroundPanelWidth, backgroundPanelHeight);
            //SelectForm.Instance.BackColor = Color.Green;
            //SelectForm.Instance.panel_Background.BackColor = Color.Blue;

            for (int i = 0; i < costTypeList.Count; i++)
            {

                FlowLayoutPanel heroPanel = new FlowLayoutPanel();
                heroPanel.Location = new Point(0, i * (heroPanelHeight + transparentHeroPanelsVerticalSpacing));
                heroPanel.Size = new Size(MaxHeroCountOfOneType * (transparentHeroPictureBoxSize.Width + transparentHeroPictureBoxHorizontalSpacing) - transparentHeroPictureBoxHorizontalSpacing, heroPanelHeight);
                heroPanel.AutoSize = false;


                heroPanel.Margin = new Padding(0);
                heroPanel.Name = $"flowLayoutPanel{costTypeList[i]}";
                heroPanel.WrapContents = false;
                heroPanel.Tag = costTypeList[i];
                heroPanel.BackColor = Color.Transparent /*Color.FromArgb(255-(i*40),20*i, 20 * i)*/;
                SelectForm.Instance.panel_Background.Controls.Add(heroPanel);
                HeroPanels.Add(heroPanel);
            }
        }

        /// <summary>
        /// 创建英雄选择器组（按费用分组）
        /// </summary>
        public void CreateTransparentHeroPictureBox()
        {
            TransparentheroPictureBoxes.Clear();
            for (int i = 0; i < HeroPanels.Count; i++)
            {
                CreateTransparentHeroPictureBoxGroup(HeroPanels[i], _iHeroDataService.GetHeroDatasFromCost(Convert.ToInt32(HeroPanels[i].Tag)));
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

                TransparentheroPictureBoxes.Add(pictureBox);                                            
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
            pictureBox.Size = transparentHeroPictureBoxSize;
            pictureBox.Image = _iHeroDataService.GetImageFromHero(hero);
            pictureBox.Tag = hero.HeroName;
            pictureBox.BorderColor = GetColor(hero.Cost);
            pictureBox.Padding = new Padding(0);
            pictureBox.Margin = new Padding(0, 0, transparentHeroPictureBoxHorizontalSpacing, 0);
            return pictureBox;
        }
        #endregion

        #region 创建阵容英雄头像框
        /// <summary>
        /// 创建单个阵容英雄头像框
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private HeroPictureBox CreatLinePictureBox(Panel parentPanel, int index)
        {
            HeroPictureBox heroPictureBox = new HeroPictureBox();
            heroPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            heroPictureBox.Image = null;
            heroPictureBox.Size = _mainForm.LogicalToDeviceUnits(lineUpHeroPictureBoxSize);
            heroPictureBox.Margin = new Padding(2); // 设置间距
            parentPanel.Controls.Add(heroPictureBox);
            return heroPictureBox;
        }

        /// <summary>
        /// 批量创建阵容头像框
        /// </summary>
        public void CreateLineUpComponents()
        {
            // 清除子阵容图片框数组
            for (int i = 0; i < lineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < lineUpPictureBoxes.GetLength(1); j++)
                {
                    lineUpPictureBoxes[i, j]?.Dispose();
                    lineUpPictureBoxes[i, j] = null;
                }
            }
            lineUpPictureBoxes = new HeroPictureBox[3, _iManualSettingsService.CurrentConfig.MaxHerosCount];

            for (int i = 0; i < lineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < lineUpPictureBoxes.GetLength(1); j++)
                {
                    lineUpPictureBoxes[i, j] = CreatLinePictureBox(lineUpPanels[i], j);
                }
            }
        }

        #endregion

        #region 创建英雄选择器
        /// <summary>
        /// 创建英雄选择分页
        /// </summary>
        public void CreatTabPages()
        {                        
            foreach (TabPage tabPage in _tabControl_HeroSelector.TabPages.OfType<TabPage>().ToList())
            {
                tabPage.Dispose();
            }
            _tabControl_HeroSelector.TabPages.Clear();
            CostPanels = new List<Panel>();
            _professionButtonPanel = null;
            _peculiarityButtonPanel = null;

            List<int> costTypeList =_iHeroDataService.GetCostType();
            for(int i = 0;i<costTypeList.Count;i++)
            {
                Panel panel_Background = new Panel();
                panel_Background.AutoScroll = true;
                panel_Background.BackColor = Color.FromArgb(255, 255, 255);
                panel_Background.Dock = DockStyle.Fill;                
                panel_Background.Margin = new Padding(0);
                panel_Background.Name = $"panel_{costTypeList[i]}Cost";
                panel_Background.Tag = costTypeList[i];
                CostPanels.Add(panel_Background);
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
        public void CreateHeroSelectors()
        {
            heroPictureBoxes.Clear();
            checkBoxes.Clear();
            NameToCheckBoxMap.Clear();
            for (int i =0;i<CostPanels.Count;i++)
            {                
                CreateHeroSelectorGroup(CostPanels[i], _iHeroDataService.GetHeroDatasFromCost(Convert.ToInt32(CostPanels[i].Tag)));
            }           
        }

        /// <summary>
        /// 创建单个费用组的英雄选择器
        /// </summary>
        private void CreateHeroSelectorGroup(Panel costPanel, List<Hero> heroes)
        {                                   
            const int  startX = 0;//起始X坐标
            const int  startY = 0;//起始Y坐标
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
                pictureBox.Location =new Point(currentX + (groupWidth - pictureBox.Width) / 2, currentY);
                label.Location = new Point(currentX + (groupWidth - label.Width) / 2, currentY + pictureBox.Height);
                checkBox.Location = new Point(
                    currentX + (groupWidth - checkBox.Width) / 2,
                    currentY + pictureBox.Height + label.Height);
               
                costPanel.Controls.Add(pictureBox);
                costPanel.Controls.Add(label);
                costPanel.Controls.Add(checkBox);               
                heroPictureBoxes.Add(pictureBox);
                checkBoxes.Add(checkBox);

                // 更新位置
                currentX += groupWidth + HeroSelectorHorizontalSpacing;
                columnCount++;

                // 换行处理
                if (columnCount >= HeroSelectorColumns)
                {
                    columnCount = 0;
                    currentX = startX;
                    currentY += pictureBox.Height + label.Height + checkBox.Height + HeroSelectorVerticalSpacing;
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
            label.Size = _mainForm.LogicalToDeviceUnits(labelSize);
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
            checkBox.Size = _mainForm.LogicalToDeviceUnits(checkBoxSize);
            checkBox.Tag = hero.HeroName;
            NameToCheckBoxMap[hero.HeroName] = checkBox;
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
            pictureBox.Size = _mainForm.LogicalToDeviceUnits(heroPictureBoxSize);
            pictureBox.Image = _iHeroDataService.GetImageFromHero(hero);
            pictureBox.Tag = hero.HeroName;
            pictureBox.BorderColor = GetColor(hero.Cost);                                           
            return pictureBox;
        }
        #endregion

        #region 创建职业与特质按钮
        /// <summary>
        /// 创建职业与特质按钮
        /// </summary>
        public void CreateProfessionAndPeculiarityButtons()
        {
            professionButtons.Clear();            
            peculiarityButtons.Clear();
            // 创建职业按钮
            CreateButtonGroup(_professionButtonPanel, _iHeroDataService.GetProfessions(), professionButtons);

            // 创建特质按钮
            CreateButtonGroup(_peculiarityButtonPanel, _iHeroDataService.GetPeculiarities(), peculiarityButtons);
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
            const int startY = 22+10; // 起始Y坐标
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
                currentX += professionAndPeculiarityButtonSize.Width + horizontalSpacing;

                // 换行处理
                if (columnCount >= columns)
                {
                    columnCount = 0;
                    currentX = startX;
                    currentY += professionAndPeculiarityButtonSize.Height + verticalSpacing;
                }
            }
        }

        /// <summary>
        /// 创建单个按钮
        /// </summary>
        /// <returns></returns>
        private Button CreatButton(Point location,string text,object? tag)
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
            button.Size = new Size(Dpi_M(professionAndPeculiarityButtonSize.Width), Dpi_M(professionAndPeculiarityButtonSize.Height));
            button.Location = location;
            button.Text = text;
            button.Tag = tag;
            return button;
        }

        #endregion

        #region 创建子阵容英雄头像框
        /// <summary>
        /// 创建单个子阵容英雄头像框
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private HeroPictureBox CreatSubLinePictureBox(Panel parentPanel, int index)
        {
            HeroPictureBox heroPictureBox = new HeroPictureBox();
            heroPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            heroPictureBox.Image = null;
            heroPictureBox.Size =_mainForm.LogicalToDeviceUnits(subLineUpPictureBoxSize) ;
            heroPictureBox.Margin = new Padding(3); // 设置间距
            parentPanel.Controls.Add(heroPictureBox);                      
            return heroPictureBox;
        }

        /// <summary>
        /// 批量创建子阵容头像框
        /// </summary>
        public void CreateSubLineUpComponents()
        {
            // 清除子阵容图片框数组
            for (int i = 0; i < subLineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < subLineUpPictureBoxes.GetLength(1); j++)
                {
                    subLineUpPictureBoxes[i, j]?.Dispose();
                    subLineUpPictureBoxes[i, j] = null;
                }
            }
            subLineUpPictureBoxes = new HeroPictureBox[3, _iManualSettingsService.CurrentConfig.MaxHerosCount];

            for (int i = 0; i < subLineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < subLineUpPictureBoxes.GetLength(1); j++)
                {                  
                    subLineUpPictureBoxes[i, j] = CreatSubLinePictureBox(subLineUpPanels[i], j);
                }
            }
        }

        #endregion

        /// <summary>
        /// 在程序启动时构建所有UI组件
        /// </summary>
        public void BuildWhenStart()
        {
            CreatTabPages();//创建分页
            CreateHeroSelectors();//创建主窗口英雄选择器
            CreateProfessionAndPeculiarityButtons();//创建主窗口职业与特质按钮
            CreateSubLineUpComponents();//创建主窗口子阵容头像框
            CreateLineUpComponents();//创建阵容面板子阵容头像框
            CreatFlowLayoutPanel();//创建半透明英雄选择面板
            CreateTransparentHeroPictureBox();//创建半透明英雄头像框
        }

        public void ReBuild()
        {
            CreatTabPages();//创建分页
            CreateHeroSelectors();//创建主窗口英雄选择器
            CreateProfessionAndPeculiarityButtons();//创建主窗口职业与特质按钮                        
            CreatFlowLayoutPanel();//创建半透明英雄选择面板
            CreateTransparentHeroPictureBox();//创建半透明英雄头像框 
        }

        public Size GetSubLineUpPictureBoxSize()
        {
            return subLineUpPictureBoxSize;
        }

        public Size GetHeroPictureBoxSize()
        {
            return heroPictureBoxSize;
        }

        public Color GetColor(int index)
        {
            switch(index)
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
            if(NameToCheckBoxMap.ContainsKey(name))
            {
                return NameToCheckBoxMap[name];
            }
            else
            {
                return null;
            }
        }
    }
}
