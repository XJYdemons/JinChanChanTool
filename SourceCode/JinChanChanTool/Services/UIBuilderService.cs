using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services.DataServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace JinChanChanTool.Services
{
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
        /// 主窗口存放英雄选择器的面板、按职业和特质选择英雄按钮的面板
        /// </summary>
        private readonly Panel _panel_Cost1;
        private readonly Panel _panel_Cost2;
        private readonly Panel _panel_Cost3;
        private readonly Panel _panel_Cost4;
        private readonly Panel _panel_Cost5;
        private readonly Panel _professionButtonPanel;
        private readonly Panel _peculiarityButtonPanel;

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
        private readonly FlowLayoutPanel _heroPanel1;
        private readonly FlowLayoutPanel _heroPanel2;
        private readonly FlowLayoutPanel _heroPanel3;
        private readonly FlowLayoutPanel _heroPanel4;
        private readonly FlowLayoutPanel _heroPanel5;
        private static readonly Size transparentHeroPictureBoxSize = new Size(32, 32);//单个英雄选择器中图像框大小
        public List<HeroPictureBox> TransparentheroPictureBoxes { get; }
        #endregion

        #region 阵容面板       
        private static readonly Size lineUpHeroPictureBoxSize = new Size(28, 28);//单个英雄选择器中图像框大小
        public HeroPictureBox[,] lineUpPictureBoxes { get; set; }
        public List<FlowLayoutPanel> lineUpPanels { get; }
        #endregion
        /// <summary>
        /// 主窗口实例
        /// </summary>
        private readonly MainForm _form1;

        /// <summary>
        /// 英雄数据服务实例
        /// </summary>
        private readonly IHeroDataService _iHeroDataService; 
        
        /// <summary>
        /// 子阵容英雄头像框数量
        /// </summary>
        private readonly int _countOfSubLineUpPictureBox;
        
        /// <summary>
        /// 颜色常量字典
        /// </summary>
        private Dictionary<int, Color> CostToColorMap { get; set; }

        public UIBuilderService(MainForm form1,Panel panel_Cost1, Panel panel_Cost2, Panel panel_Cost3, Panel panel_Cost4, Panel panel_Cost5, Panel professionButtonPanel, Panel peculiarityButtonPanel, FlowLayoutPanel subLineUpPanel1, FlowLayoutPanel subLineUpPanel2, FlowLayoutPanel subLineUpPanel3, IHeroDataService iHeroDataService,int countOfSubLineUpPictureBox,FlowLayoutPanel HeroPanel1, FlowLayoutPanel HeroPanel2, FlowLayoutPanel HeroPanel3, FlowLayoutPanel HeroPanel4, FlowLayoutPanel HeroPanel5, FlowLayoutPanel LineUpPanel1, FlowLayoutPanel LineUpPanel2, FlowLayoutPanel LineUpPanel3)
        {
            _countOfSubLineUpPictureBox = countOfSubLineUpPictureBox;
            heroPictureBoxes = new List<HeroPictureBox>();
            checkBoxes = new List<CheckBox>();
            professionButtons = new List<Button>();
            peculiarityButtons = new List<Button>();            
            subLineUpPanels = new List<FlowLayoutPanel>();
            CostToColorMap = new Dictionary<int, Color>();
            NameToCheckBoxMap = new Dictionary<string, CheckBox>();
            TransparentheroPictureBoxes = new List<HeroPictureBox>();
            lineUpPanels= new List<FlowLayoutPanel>();
            _panel_Cost1 = panel_Cost1;
            _panel_Cost2 = panel_Cost2;
            _panel_Cost3 = panel_Cost3;
            _panel_Cost4 = panel_Cost4;
            _panel_Cost5 = panel_Cost5;
            _professionButtonPanel = professionButtonPanel;
            _peculiarityButtonPanel = peculiarityButtonPanel;
            subLineUpPanels.AddRange(new List<FlowLayoutPanel> { subLineUpPanel1, subLineUpPanel2, subLineUpPanel3 });
            _iHeroDataService = iHeroDataService;                      
            subLineUpPictureBoxes = new HeroPictureBox[3, countOfSubLineUpPictureBox];
            lineUpPictureBoxes = new HeroPictureBox[3, countOfSubLineUpPictureBox];
            _form1 = form1;
            _heroPanel1 = HeroPanel1;
            _heroPanel2 = HeroPanel2;
            _heroPanel3 = HeroPanel3;
            _heroPanel4 = HeroPanel4;
            _heroPanel5 = HeroPanel5;
            lineUpPanels.AddRange(new List<FlowLayoutPanel> { LineUpPanel1, LineUpPanel2, LineUpPanel3 });            
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

        #region 创建半透明英雄头像框
        public void CreateTransparentHeroPictureBox()
        {
           for(int i = 1;i<=5;i++)
            {
                CreateTransparentHeroPictureBoxGroup(i, _iHeroDataService.GetHeroDatasFromCost(i));
            }
        }

        private void CreateTransparentHeroPictureBoxGroup(int cost, List<HeroData> heroes)
        {
            FlowLayoutPanel panel = GetTransparentPanel(cost);
            if (panel == null) return;
            // 清空面板
            panel.Controls.Clear();

            foreach (var hero in heroes)
            {
                // 创建控件
                var pictureBox = CreateTransparentPictureBox(hero);

                panel.Controls.Add(pictureBox);

                TransparentheroPictureBoxes.Add(pictureBox);                                            
            }
        }

        private FlowLayoutPanel GetTransparentPanel(int cost)
        {
            return cost switch
            {
                1 => _heroPanel1,
                2 => _heroPanel2,
                3 => _heroPanel3,
                4 => _heroPanel4,
                5 => _heroPanel5,
                _ => null,
            };
        }
        
        private HeroPictureBox CreateTransparentPictureBox(HeroData hero)
        {
            HeroPictureBox pictureBox = new HeroPictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            pictureBox.BackColor = SystemColors.Control;
            pictureBox.BorderWidth = 1;
            pictureBox.Size = _form1.LogicalToDeviceUnits(transparentHeroPictureBoxSize);
            pictureBox.Image = _iHeroDataService.GetImageFromHero(hero);
            pictureBox.Tag = hero.HeroName;
            pictureBox.BorderColor = GetColor(hero.Cost);
            pictureBox.Padding = new Padding(0);
            pictureBox.Margin = new Padding(0, 0, 1, 0);
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
            heroPictureBox.Size = _form1.LogicalToDeviceUnits(lineUpHeroPictureBoxSize);
            heroPictureBox.Margin = new Padding(2); // 设置间距
            parentPanel.Controls.Add(heroPictureBox);
            return heroPictureBox;
        }

        /// <summary>
        /// 批量创建阵容头像框
        /// </summary>
        public void CreateLineUpComponents()
        {
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
        /// 创建英雄选择器组（按费用分组）
        /// </summary>
        public void CreateHeroSelectors()
        {
            for(int i =1;i<=5;i++)
            {
                CreateHeroSelectorGroup(i, _iHeroDataService.GetHeroDatasFromCost(i));
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
            checkBox.Tag = hero.HeroName;
            NameToCheckBoxMap[hero.HeroName] = checkBox;
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
                Button button = CreatButton((Point)_form1.LogicalToDeviceUnits((Size)(new Point(currentX, currentY))),item.Title,item);
             
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
            button.Size = _form1.LogicalToDeviceUnits( professionAndPeculiarityButtonSize);
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
            //  清除英雄选择器相关控件
            ClearHeroSelector(_panel_Cost1);
            ClearHeroSelector(_panel_Cost2);
            ClearHeroSelector(_panel_Cost3);
            ClearHeroSelector(_panel_Cost4);
            ClearHeroSelector(_panel_Cost5);

            //  清除职业和特质按钮
            ClearPanelControls(_professionButtonPanel);
            ClearPanelControls(_peculiarityButtonPanel);

            //  清除子阵容图片框
            ClearSubLineUpPictureBox();

            //  清除半透明英雄选择图片框
            ClearTransparentHeroPictureBox();

            ClearLineUpPictureBox();
            //  清空所有控件列表
            heroPictureBoxes.Clear();
            checkBoxes.Clear();
            professionButtons.Clear();
            peculiarityButtons.Clear();
            TransparentheroPictureBoxes.Clear();
            NameToCheckBoxMap.Clear();


            // 5. 重新初始化子阵容图片框数组
            subLineUpPictureBoxes = new HeroPictureBox[3, _countOfSubLineUpPictureBox];
            lineUpPictureBoxes = new HeroPictureBox[3, _countOfSubLineUpPictureBox];
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

        private void ClearLineUpPictureBox()
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
        }

        private void ClearTransparentHeroPictureBox()
        {
            _heroPanel1.Controls.Clear();
            _heroPanel2.Controls.Clear();
            _heroPanel3.Controls.Clear();
            _heroPanel4.Controls.Clear();
            _heroPanel5.Controls.Clear();
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
