using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services.DataServices;
using System.Diagnostics;

namespace JinChanChanTool.Services
{
    public class UIBuilderService
    {
        private readonly Form1 _form1;
        public List<HeroPictureBox> heroPictureBoxes { get; }
        public List<CheckBox> checkBoxes { get; }
        public List<Button> professionButtons { get; }
        public List<Button> peculiarityButtons { get; }
        public HeroPictureBox[,] subLineUpPictureBoxes { get; set; }
        public List<FlowLayoutPanel> subLineUpPanels { get; }

        private readonly IHeroDataService _iHeroDataService;
        private readonly ILineUpService _iLineUpservice;
        private readonly IAppConfigService _iAppConfigService;
        private readonly IHeroEquipmentDataService _iHeroEquipmentDataService;
        private readonly Panel _panel_Cost1;
        private readonly Panel _panel_Cost2;
        private readonly Panel _panel_Cost3;
        private readonly Panel _panel_Cost4;
        private readonly Panel _panel_Cost5;        
        private readonly Panel _professionButtonPanel;
        private readonly Panel _peculiarityButtonPanel;
        
        
        //
        //英雄选择器常量
        //
        private const int HeroSelectorColumns = 5; //每行英雄选择器数量
        private const int HeroSelectorHorizontalSpacing = 4; //英雄选择器之间的水平间距
        private const int HeroSelectorVerticalSpacing = 1; // 英雄选择器之间的垂直间距
        private static readonly Size heroPictureBoxSize = new Size(48, 48);//单个英雄选择器中图像框大小
        private static readonly Size labelSize = new Size(67, 19);//单个英雄选择器中名称标签大小
        private static readonly Size checkBoxSize = new Size(14, 14);//单个复选框大小

        //
        //按职业与特质选择英雄按钮常量
        //
        private const int columns = 2; //每行按钮数量
        private const int horizontalSpacing = 1; //按钮之间的水平间距
        private const int verticalSpacing = 10; // 按钮之间的垂直间距
        private static readonly Size professionAndPeculiarityButtonSize= new Size(83, 23);//单个按钮的大小

        //
        //子阵容常量
        //
        private static readonly Size subLineUpPictureBoxSize = new Size(32, 32);//单个子阵容头像框的大小

        /// <summary>
        /// 颜色常量字典
        /// </summary>
        private Dictionary<int, Color> CostToColorMap { get; set; }

        public UIBuilderService(Form1 form1,Panel panel_Cost1, Panel panel_Cost2, Panel panel_Cost3, Panel panel_Cost4, Panel panel_Cost5, Panel professionButtonPanel, Panel peculiarityButtonPanel, FlowLayoutPanel subLineUpPanel1, FlowLayoutPanel subLineUpPanel2, FlowLayoutPanel subLineUpPanel3, IHeroDataService iHeroDataService, ILineUpService iLineUpservice, IAppConfigService iAppConfigService, IHeroEquipmentDataService iHeroEquipmentDataService)
        {
            heroPictureBoxes = new List<HeroPictureBox>();
            checkBoxes = new List<CheckBox>();
            professionButtons = new List<Button>();
            peculiarityButtons = new List<Button>();            
            subLineUpPanels = new List<FlowLayoutPanel>();
            CostToColorMap = new Dictionary<int, Color>();
            _panel_Cost1 = panel_Cost1;
            _panel_Cost2 = panel_Cost2;
            _panel_Cost3 = panel_Cost3;
            _panel_Cost4 = panel_Cost4;
            _panel_Cost5 = panel_Cost5;
            _professionButtonPanel = professionButtonPanel;
            _peculiarityButtonPanel = peculiarityButtonPanel;
            subLineUpPanels.AddRange(new List<FlowLayoutPanel> { subLineUpPanel1, subLineUpPanel2, subLineUpPanel3 });
            _iHeroDataService = iHeroDataService;
            _iLineUpservice = iLineUpservice;
            _iAppConfigService = iAppConfigService;
            subLineUpPictureBoxes = new HeroPictureBox[3, _iAppConfigService.CurrentConfig.MaxOfChoices];
            _iHeroEquipmentDataService = iHeroEquipmentDataService;
            _form1 = form1;
            BuildCostToColorMap();
        }

        /// <summary>
        /// 构建从Cost到颜色的字典
        /// </summary>
        private void BuildCostToColorMap()
        {
            CostToColorMap[1] = Color.FromArgb(107, 104, 101);
            CostToColorMap[2] = Color.FromArgb(5, 171, 117);
            CostToColorMap[3] = Color.FromArgb(0, 133, 255);
            CostToColorMap[4] = Color.FromArgb(175, 40, 195);
            CostToColorMap[5] = Color.FromArgb(245, 158, 11);
        }

        #region 创建英雄选择器
        /// <summary>
        /// 创建英雄选择器组（按费用分组）
        /// </summary>
        public void CreateHeroSelectors()
        {
            // 按费用分组
            var costGroups = _iHeroDataService.HeroDatas
                .GroupBy(h => h.Cost)
                .OrderBy(g => g.Key)
                .ToList();

            // 创建每个费用组
            foreach (var group in costGroups)
            {
                CreateHeroSelectorGroup(group.Key, group.ToList());
            }
        }

        /// <summary>
        /// 创建单个费用组的英雄选择器
        /// </summary>
        private void CreateHeroSelectorGroup(int cost, List<HeroData> heroes)
        {
            // 获取对应的费用面板
            Panel panel = GetCostPanel(cost);
            if (panel == null) return;

            // 清空面板
            panel.Controls.Clear();
            
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
               
                panel.Controls.Add(pictureBox);
                panel.Controls.Add(label);
                panel.Controls.Add(checkBox);               
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
        /// 获取对应费用的面板
        /// </summary>
        private Panel GetCostPanel(int cost)
        {
            return cost switch
            {
                1 => _panel_Cost1,
                2 => _panel_Cost2,
                3 => _panel_Cost3,
                4 => _panel_Cost4,
                5 => _panel_Cost5,
                _ => null,
            };
        }

        /// <summary>
        /// 创建标签函数
        /// </summary>
        private Label CreatLabel(HeroData hero)
        {
            Label label = new Label();
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Font = new Font("Microsoft YaHei UI", 7F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label.Text = hero.HeroName;
            label.Size = _form1.LogicalToDeviceUnits(labelSize);
            label.ForeColor = GetColor(hero.Cost);           
            return label;
        }

        /// <summary>
        /// 创建CheckBox函数
        /// </summary>
        private CheckBox CreatCheckBox(HeroData hero)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.UseVisualStyleBackColor = true;
            checkBox.TabStop = false;
            checkBox.FlatStyle = FlatStyle.Flat;
            checkBox.Size = _form1.LogicalToDeviceUnits(checkBoxSize);
            checkBox.Tag = hero;
            return checkBox;
        }

        /// <summary>
        /// 创建HeroPictureBox函数
        /// </summary>
        private HeroPictureBox CreatPictureBox(HeroData hero)
        {
            HeroPictureBox pictureBox = new HeroPictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            pictureBox.BackColor = SystemColors.Control;
            pictureBox.BorderWidth = 2;
            pictureBox.Size = _form1.LogicalToDeviceUnits(heroPictureBoxSize);
            pictureBox.Image = _iHeroDataService.HeroDataToImageMap[hero];
            pictureBox.Tag = hero;
            pictureBox.BorderColor = GetColor(hero.Cost);

            // 1. 从新的数据服务中，根据英雄名查找对应的装备信息对象
            var heroEquipment = _iHeroEquipmentDataService.HeroEquipments
                .FirstOrDefault(he => he.HeroName == hero.HeroName);

            if (heroEquipment != null)
            {
                // 2. 使用该对象作为键，从图片映射字典中获取预加载好的图片列表
                if (_iHeroEquipmentDataService.EquipmentImageMap.TryGetValue(heroEquipment, out var equipmentImages))
                {
                    // 3. 创建新的、纯展示的 ToolTip 实例，并将图片列表传入
                    var equipmentToolTip = new JinChanChanTool.DIYComponents.EquipmentToolTip(equipmentImages);

                    // 4. 将这个 ToolTip 关联到 PictureBox 上
                    equipmentToolTip.SetToolTip(pictureBox, " "); // 文本内容不重要，只是为了激活
                }
            }

            return pictureBox;
        }
        #endregion

        #region 创建职业与特质按钮
        /// <summary>
        /// 创建职业与特质按钮
        /// </summary>
        public void CreateProfessionAndPeculiarityButtons()
        {
            // 创建职业按钮
            CreateButtonGroup(_professionButtonPanel, _iHeroDataService.Professions, professionButtons);

            // 创建特质按钮
            CreateButtonGroup(_peculiarityButtonPanel, _iHeroDataService.Peculiarities, peculiarityButtons);
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
                Button button = CreatButton();

                // 设置按钮位置
                button.Location = (Point)_form1.LogicalToDeviceUnits((Size)(new Point(currentX, currentY)));

                // 设置按钮文本和标签                
                dynamic item = items[i];
                button.Text = item.Title;
                button.Tag = item;

                // 添加到面板和列表
                panel.Controls.Add(button);
                buttonList.Add(button);

                // 更新位置
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
        private Button CreatButton()
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
            button.Size = _form1.LogicalToDeviceUnits( professionAndPeculiarityButtonSize);
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
            heroPictureBox.Size =_form1.LogicalToDeviceUnits(subLineUpPictureBoxSize) ;
            heroPictureBox.Margin = new Padding(3); // 设置间距
            parentPanel.Controls.Add(heroPictureBox);                      
            return heroPictureBox;
        }

        /// <summary>
        /// 批量创建子阵容头像框
        /// </summary>
        public void CreateSubLineUpComponents()
        {                                 
            for (int i = 0; i < subLineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < subLineUpPictureBoxes.GetLength(1); j++)
                {                  
                    subLineUpPictureBoxes[i, j] = CreatSubLinePictureBox(subLineUpPanels[i], j);
                }
            }
        }

        #endregion

        #region 清理UI
        public void UnBuild()
        {
            // 1. 清除英雄选择器相关控件
            ClearHeroSelector(_panel_Cost1);
            ClearHeroSelector(_panel_Cost2);
            ClearHeroSelector(_panel_Cost3);
            ClearHeroSelector(_panel_Cost4);
            ClearHeroSelector(_panel_Cost5);

            // 2. 清除职业和特质按钮
            ClearPanelControls(_professionButtonPanel);
            ClearPanelControls(_peculiarityButtonPanel);

            // 3. 清除子阵容图片框
            ClearSubLineUpPictureBox();

            // 4. 清空所有控件列表
            heroPictureBoxes.Clear();
            checkBoxes.Clear();
            professionButtons.Clear();
            peculiarityButtons.Clear();
           
            
            // 5. 重新初始化子阵容图片框数组
            subLineUpPictureBoxes = new HeroPictureBox[3, _iAppConfigService.CurrentConfig.MaxOfChoices];
        }

        /// <summary>
        /// 清理英雄选择器
        /// </summary>
        /// <param name="panel"></param>
        private void ClearHeroSelector(Panel panel)
        {
            // 创建临时列表避免修改集合时遍历
            var controlsToRemove = new List<Control>();
            foreach (Control control in panel.Controls)
            {
                // 只移除我们创建的控件（标签、复选框、图片框）
                if (control is Label || control is CheckBox || control is HeroPictureBox)
                {
                    controlsToRemove.Add(control);
                }
            }

            // 移除并释放控件
            foreach (var control in controlsToRemove)
            {
                panel.Controls.Remove(control);
                control.Dispose();
            }
        }

        /// <summary>
        /// 清理按职业与特质选择英雄按钮
        /// </summary>
        /// <param name="panel"></param>
        private void ClearPanelControls(Panel panel)
        {
            // 创建临时列表避免修改集合时遍历
            var controlsToRemove = new List<Control>();
            foreach (Control control in panel.Controls)
            {
                // 只移除我们创建的控件（按钮）
                if (control is Button)
                {
                    controlsToRemove.Add(control);
                }                
            }

            // 移除并释放控件
            foreach (var control in controlsToRemove)
            {
                panel.Controls.Remove(control);
                control.Dispose();
            }
        }

        /// <summary>
        /// 清理子阵容头像框
        /// </summary>
        private void ClearSubLineUpPictureBox()
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
        }

        #endregion

        public Size GetSubLineUpPanelSizes(int index)
        {
            if(! (subLineUpPanels.Count>0))return new Size(1, 1);
            switch (index)
            {
                case 0:
                    return subLineUpPanels[0].Size;
                    break;
                case 1:
                    return subLineUpPanels[0].Size;
                    break;              
                default:
                    return subLineUpPanels[0].Size;
            }
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
            CostToColorMap.TryGetValue(index, out Color color);
            return color;
        }
    }
}
