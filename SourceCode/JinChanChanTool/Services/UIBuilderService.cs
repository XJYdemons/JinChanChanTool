using JinChanChanTool.DataClass;
using JinChanChanTool.DIYComponents;
using JinChanChanTool.Services.DataServices;
using System.Diagnostics;

namespace JinChanChanTool.Services
{
    public class UIBuilderService
    {
        public List<HeroPictureBox> heroPictureBoxes { get; }
        public List<CheckBox> checkBoxes { get; }
        public List<Button> professionButtons { get; }
        public List<Button> peculiarityButtons { get; }
        public HeroPictureBox[,] subLineUpPictureBoxes { get; set; }
        public List<Panel> subLineUpPanels { get; }

        private Point[] subLineUpPictureBoxLocation ;
        private Size subLineUpPictureBoxSize;
        private Point[,] subLineUpPanelLocation ;
        private Size[] subLineUpPanelSizes;
        private Point[,] heroSelectorPoints;
        private Size heroPictureBoxSize ;
        private Size labelSize ;
        private Point[] professionAndPeculiarityButtonPoints ;
        private Size    professionAndPeculiarityButtonSize;
        private Color   cost1Color;
        private Color   cost2Color;
        private Color   cost3Color;
        private Color   cost4Color;
        private Color   cost5Color;

        private readonly Form1 _form;
        private readonly IHeroDataService _iHeroDataService;
        private readonly ILineUpService _iLineUpservice;
        private readonly IAppConfigService _iAppConfigService;
        private readonly TabPage _tabPage1;
        private readonly TabPage _tabPage2;
        private readonly TabPage _tabPage3;
        private readonly TabPage _tabPage4;
        private readonly TabPage _tabPage5;
        private readonly Panel _professionButtonPanel;
        private readonly Panel _peculiarityButtonPanel;

        public UIBuilderService(Form1 form, TabPage tabPage1, TabPage tabPage2, TabPage tabPage3, TabPage tabPage4, TabPage tabPage5, Panel professionButtonPanel, Panel peculiarityButtonPanel, IHeroDataService iHeroDataService, ILineUpService iLineUpservice, IAppConfigService iAppConfigService)
        {
            _form = form;
            _iHeroDataService = iHeroDataService;
            _iLineUpservice = iLineUpservice;
            _iAppConfigService = iAppConfigService;
            _tabPage1 = tabPage1;
            _tabPage2 = tabPage2;
            _tabPage3 = tabPage3;
            _tabPage4 = tabPage4;
            _tabPage5 = tabPage5;
            _professionButtonPanel = professionButtonPanel;
            _peculiarityButtonPanel = peculiarityButtonPanel;
            heroPictureBoxes = new List<HeroPictureBox>();
            checkBoxes = new List<CheckBox>();
            professionButtons = new List<Button>();            
            peculiarityButtons = new List<Button>();
            subLineUpPictureBoxes = new HeroPictureBox[3, _iAppConfigService.CurrentConfig.MaxOfChoices];
            subLineUpPanels  = new List<Panel>();
            subLineUpPictureBoxLocation = new Point[60]
            {
            new Point(4  , 4),
            new Point(40 , 4),
            new Point(76 , 4),
            new Point(112, 4),
            new Point(148, 4),
            new Point(184, 4),
            new Point(220, 4),
            new Point(256, 4),
            new Point(292, 4),
            new Point(328, 4),
            new Point(364, 4),
            new Point(400, 4),
            new Point(436, 4),
            new Point(472, 4),
            new Point(508, 4),
            new Point(544, 4),
            new Point(580, 4),
            new Point(616, 4),
            new Point(652, 4),
            new Point(688, 4),
            new Point(724, 4),
            new Point(760, 4),
            new Point(796, 4),
            new Point(832, 4),
            new Point(868, 4),
            new Point(904, 4),
            new Point(940, 4),
            new Point(976, 4),
            new Point(1012, 4),
            new Point(1048, 4),
            new Point(1084, 4),
            new Point(1120, 4),
            new Point(1156, 4),
            new Point(1192, 4),
            new Point(1228, 4),
            new Point(1264, 4),
            new Point(1300, 4),
            new Point(1336, 4),
            new Point(1372, 4),
            new Point(1408, 4),
            new Point(1444, 4),
            new Point(1480, 4),
            new Point(1516, 4),
            new Point(1552, 4),
            new Point(1588, 4),
            new Point(1408, 4),
            new Point(1624, 4),
            new Point(1660, 4),
            new Point(1696, 4),
            new Point(1732, 4),
            new Point(1768, 4),
            new Point(1804, 4),
            new Point(1840, 4),
            new Point(1876, 4),
            new Point(1912, 4),
            new Point(1948, 4),
            new Point(1984, 4),
            new Point(2020, 4),
            new Point(2056, 4),
            new Point(2092, 4),
        };
            subLineUpPictureBoxSize = new Size(32, 32);
            subLineUpPanelLocation = new Point[2, 3]
        {
            { new Point(11 ,389),new Point(11 ,42+389+5),new Point(11 ,84+389+10)},//当阵容最大选择英雄数<=10时，panel面板的位置
            { new Point(11 ,389),new Point(11 ,42+389+5+18),new Point(11 ,84+389+10+36)}//当阵容最大选择英雄数>10时，panel面板的位置
        };
            subLineUpPanelSizes = new Size[] { new Size(380, 42), new Size(380, 60) };
            heroSelectorPoints = new Point[18, 3]
       {
            { new Point(12 ,6  ) , new Point(0  ,55 ) , new Point(29 ,75 )},
            { new Point(73 ,6  ) , new Point(61 ,55 ) , new Point(90 ,75 )},
            { new Point(134,6  ) , new Point(122,55 ) , new Point(151,75 )},
            { new Point(195,6  ) , new Point(183,55 ) , new Point(212,75 )},
            { new Point(256,6  ) , new Point(244,55 ) , new Point(273,75 )},
            { new Point(317,6  ) , new Point(305,55 ) , new Point(334,75 )},

            { new Point(12 ,94 ) , new Point(0  ,143) , new Point(29 ,163)},
            { new Point(73 ,94 ) , new Point(61 ,143) , new Point(90 ,163)},
            { new Point(134,94 ) , new Point(122,143) , new Point(151,163)},
            { new Point(195,94 ) , new Point(183,143) , new Point(212,163)},
            { new Point(256,94 ) , new Point(244,143) , new Point(273,163)},
            { new Point(317,94 ) , new Point(305,143) , new Point(334,163)},

            { new Point(12 ,182) , new Point(0  ,231) , new Point(29 ,251)},
            { new Point(73 ,182) , new Point(61 ,231) , new Point(90 ,251)},
            { new Point(134,182) , new Point(122,231) , new Point(151,251)},
            { new Point(195,182) , new Point(183,231) , new Point(212,251)},
            { new Point(256,182) , new Point(244,231) , new Point(273,251)},
            { new Point(317,182) , new Point(305,231) , new Point(334,251)}
       };
            heroPictureBoxSize = new Size(48, 48);
            labelSize = new Size(72, 19);
            professionAndPeculiarityButtonPoints = new Point[30]
                   {
                        new Point(8,29),
                        new Point(87,29),
                        new Point(8,59),
                        new Point(87,59),
                        new Point(8,89),
                        new Point(87,89),
                        new Point(8,119),
                        new Point(87,119),
                        new Point(8,149),
                        new Point(87,149),
                        new Point(8,179),
                        new Point(87,179),
                        new Point(8,209),
                        new Point(87,209),
                        new Point(8,239),
                        new Point(87,239),
                        new Point(8,269),
                        new Point(87,269),
                        new Point(8,299),
                        new Point(87,299),
                        new Point(8,329),
                        new Point(87,329),
                        new Point(8,359),
                        new Point(87,359),
                        new Point(8,389),
                        new Point(87,389),
                        new Point(8,419),
                        new Point(87,419),
                        new Point(8,449),
                        new Point(87,449),
                   };
            professionAndPeculiarityButtonSize = new Size(72, 23);
            cost1Color = Color.FromArgb(107, 104, 101);
            cost2Color = Color.FromArgb(5, 171, 117);
            cost3Color = Color.FromArgb(0, 133, 255);
            cost4Color = Color.FromArgb(175, 40, 195);
            cost5Color = Color.FromArgb(245, 158, 11);
        }

        /// <summary>
        /// 创建标签函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="费阶"></param>
        /// <param name="位置"></param>
        /// <returns></returns>
        private Label CreatLabel(HeroData hero, int 位置)
        {
            Label label = new Label();
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Font = new Font("Microsoft YaHei UI", 7F, FontStyle.Regular, GraphicsUnit.Point, 134);

            label.Text = hero.HeroName;
            label.Size = _form.LogicalToDeviceUnits(labelSize);
            label.Location = (Point)_form.LogicalToDeviceUnits((Size)heroSelectorPoints[位置, 1]);
            switch (hero.Cost)
            {
                case 1:
                    label.ForeColor = cost1Color;
                    _tabPage1.Controls.Add(label);
                    break;
                case 2:
                    label.ForeColor = cost2Color;
                    _tabPage2.Controls.Add(label);
                    break;
                case 3:
                    label.ForeColor = cost3Color;
                    _tabPage3.Controls.Add(label);
                    break;
                case 4:
                    label.ForeColor = cost4Color;
                    _tabPage4.Controls.Add(label);
                    break;
                case 5:
                    label.ForeColor = cost5Color;
                    _tabPage5.Controls.Add(label);
                    break;
            }

            return label;
        }

        /// <summary>
        /// 创建CheckBox函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="费阶"></param>
        /// <param name="位置"></param>
        /// <param name="数组下标"></param>
        /// <returns></returns>
        private CheckBox CreatCheckBox(HeroData hero, int 位置)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.UseVisualStyleBackColor = true;
            checkBox.TabStop = false;
            checkBox.FlatStyle = FlatStyle.Flat;

            checkBox.Size = _form.LogicalToDeviceUnits(new Size(14, 14));
            checkBox.Location = (Point)_form.LogicalToDeviceUnits((Size)heroSelectorPoints[位置, 2]);
            checkBox.Tag = hero;
            switch (hero.Cost)
            {
                case 1:
                    _tabPage1.Controls.Add(checkBox);
                    break;
                case 2:
                    _tabPage2.Controls.Add(checkBox);
                    break;
                case 3:
                    _tabPage3.Controls.Add(checkBox);
                    break;
                case 4:
                    _tabPage4.Controls.Add(checkBox);
                    break;
                case 5:
                    _tabPage5.Controls.Add(checkBox);
                    break;
            }

            return checkBox;
        }

        /// <summary>
        /// 创建HeroPictureBox函数
        /// </summary>
        /// <param name="图片路径"></param>
        /// <param name="费阶"></param>
        /// <param name="位置"></param>
        /// <returns></returns>
        private HeroPictureBox CreatPictureBox(HeroData hero, int 位置)
        {
            HeroPictureBox pictureBox = new HeroPictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            pictureBox.BackColor = SystemColors.Control;
            pictureBox.BorderWidth = 2;

            pictureBox.Size = _form.LogicalToDeviceUnits(heroPictureBoxSize);
            pictureBox.Image = _iHeroDataService.HeroDataToImageMap[hero];
            pictureBox.Location = (Point)_form.LogicalToDeviceUnits((Size)heroSelectorPoints[位置, 0]);
            pictureBox.Tag = hero;
            switch (hero.Cost)
            {
                case 1:
                    pictureBox.BorderColor = cost1Color;
                    _tabPage1.Controls.Add(pictureBox);
                    break;
                case 2:
                    pictureBox.BorderColor = cost2Color;
                    _tabPage2.Controls.Add(pictureBox);
                    break;
                case 3:
                    pictureBox.BorderColor = cost3Color;
                    _tabPage3.Controls.Add(pictureBox);
                    break;
                case 4:
                    pictureBox.BorderColor = cost4Color;
                    _tabPage4.Controls.Add(pictureBox);
                    break;
                case 5:
                    pictureBox.BorderColor = cost5Color;
                    _tabPage5.Controls.Add(pictureBox);
                    break;
            }

            return pictureBox;
        }

        public void CreateHeroSelectors()
        {
            int m1 = 0;//1费计数
            int m2 = 0;//2费计数
            int m3 = 0;//3费计数
            int m4 = 0;//4费计数
            int m5 = 0;//5费计数

            for (int i = 0; i < _iHeroDataService.HeroDatas.Count; i++)
            {
                switch (_iHeroDataService.HeroDatas[i].Cost)
                {
                    case 1:
                        CreatLabel(_iHeroDataService.HeroDatas[i], m1);
                        heroPictureBoxes.Add(CreatPictureBox(_iHeroDataService.HeroDatas[i], m1));
                        checkBoxes.Add(CreatCheckBox(_iHeroDataService.HeroDatas[i], m1));
                        m1++;
                        break;
                    case 2:
                        CreatLabel(_iHeroDataService.HeroDatas[i], m2);
                        heroPictureBoxes.Add(CreatPictureBox(_iHeroDataService.HeroDatas[i], m2));
                        checkBoxes.Add(CreatCheckBox(_iHeroDataService.HeroDatas[i], m2));
                        m2++;
                        break;
                    case 3:
                        CreatLabel(_iHeroDataService.HeroDatas[i], m3);
                        heroPictureBoxes.Add(CreatPictureBox(_iHeroDataService.HeroDatas[i], m3));
                        checkBoxes.Add(CreatCheckBox(_iHeroDataService.HeroDatas[i], m3));
                        m3++;
                        break;
                    case 4:
                        CreatLabel(_iHeroDataService.HeroDatas[i], m4);
                        heroPictureBoxes.Add(CreatPictureBox(_iHeroDataService.HeroDatas[i], m4));
                        checkBoxes.Add(CreatCheckBox(_iHeroDataService.HeroDatas[i], m4));
                        m4++;
                        break;
                    case 5:
                        CreatLabel(_iHeroDataService.HeroDatas[i], m5);
                        heroPictureBoxes.Add(CreatPictureBox(_iHeroDataService.HeroDatas[i], m5));
                        checkBoxes.Add(CreatCheckBox(_iHeroDataService.HeroDatas[i], m5));
                        m5++;
                        break;

                }

            }

        }

        /// <summary>
        /// 创建职业按钮
        /// </summary>
        /// <param name="位置"></param>
        /// <param name="职业或特质"></param>
        /// <returns></returns>
        private Button CreatProfessionButton(Panel panel, Profession profession, int 位置)
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

            button.Size = _form.LogicalToDeviceUnits(professionAndPeculiarityButtonSize);
            button.Location = (Point)_form.LogicalToDeviceUnits((Size)professionAndPeculiarityButtonPoints[位置]);
            button.Text = profession.Title;
            button.Tag = profession;
            panel.Controls.Add(button);

            return button;
        }

        /// <summary>
        /// 创建特质按钮
        /// </summary>
        /// <param name="位置"></param>
        /// <param name="职业或特质"></param>
        /// <returns></returns>
        private Button CreatPeculiarityButton(Panel panel, Peculiarity peculiarity, int 位置)
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

            button.Size = _form.LogicalToDeviceUnits(professionAndPeculiarityButtonSize);
            button.Location = (Point)_form.LogicalToDeviceUnits((Size)professionAndPeculiarityButtonPoints[位置]);
            button.Text = peculiarity.Title;
            button.Tag = peculiarity;
            panel.Controls.Add(button);

            return button;
        }
        public void CreateProfessionAndPeculiarityButtons()
        {
            for (int i = 0; i < _iHeroDataService.Professions.Count; i++)
            {
                professionButtons.Add(CreatProfessionButton(_professionButtonPanel, _iHeroDataService.Professions[i], i));

            }
            for (int i = 0; i < _iHeroDataService.Peculiarities.Count; i++)
            {
                peculiarityButtons.Add(CreatPeculiarityButton(_peculiarityButtonPanel, _iHeroDataService.Peculiarities[i], i));
            }
        }

        /// <summary>
        /// 创建子阵容面板
        /// </summary>
        /// <param name="name"></param>
        /// <param name="locationIndex"></param>
        /// <returns></returns>
        private Panel CreatSubLinePanel(int locationIndex)
        {
            Panel panel = new Panel();
            _form.Controls.Add(panel);

            panel.BackColor = SystemColors.Control;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.TabStop = false;

            if (_iAppConfigService.CurrentConfig.MaxOfChoices <= 10)
            {               
                panel.AutoScroll = false;
                panel.Size = _form.LogicalToDeviceUnits(subLineUpPanelSizes[0]);
                panel.Location = (Point)_form.LogicalToDeviceUnits((Size)subLineUpPanelLocation[0, locationIndex]);
            }
            else
            {                
                panel.AutoScroll = true;
                panel.Size = _form.LogicalToDeviceUnits(subLineUpPanelSizes[1]);
                panel.Location = (Point)_form.LogicalToDeviceUnits((Size)subLineUpPanelLocation[1, locationIndex]);
            }

            return panel;
        }

        /// <summary>
        /// 创建子阵容英雄头像框
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private HeroPictureBox CreatSubLinePictureBox(Panel parentPanel, int index)
        {
            HeroPictureBox heroPictureBox = new HeroPictureBox();
            heroPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            heroPictureBox.Image = null;

            parentPanel.Controls.Add(heroPictureBox);
            heroPictureBox.Size = _form.LogicalToDeviceUnits(subLineUpPictureBoxSize);
            heroPictureBox.Location = (Point)_form.LogicalToDeviceUnits((Size)subLineUpPictureBoxLocation[index]);

            return heroPictureBox;
        }

        public void CreateSubLineUpComponents()
        {
            subLineUpPanels.Add(CreatSubLinePanel(0));
            subLineUpPanels.Add(CreatSubLinePanel(1));
            subLineUpPanels.Add(CreatSubLinePanel(2));
            Debug.WriteLine($"subLineUpPictureBoxes.GetLength0:{subLineUpPictureBoxes.GetLength(0)}");
            Debug.WriteLine($"subLineUpPictureBoxes.GetLength1:{subLineUpPictureBoxes.GetLength(1)}");
            for (int i = 0; i < subLineUpPictureBoxes.GetLength(0); i++)
            {
                for (int j = 0; j < subLineUpPictureBoxes.GetLength(1); j++)
                {
                    subLineUpPictureBoxes[i, j] = CreatSubLinePictureBox(subLineUpPanels[i], j);
                }
            }
        }

        public void UnBuild()
        {
            // 1. 清除英雄选择器相关控件
            ClearTabPageControls(_tabPage1);
            ClearTabPageControls(_tabPage2);
            ClearTabPageControls(_tabPage3);
            ClearTabPageControls(_tabPage4);
            ClearTabPageControls(_tabPage5);

            // 2. 清除职业和特质按钮
            ClearPanelControls(_professionButtonPanel);
            ClearPanelControls(_peculiarityButtonPanel);

            // 3. 清除子阵容面板和图片框
            ClearSubLineUpComponents();

            // 4. 清空所有控件列表
            heroPictureBoxes.Clear();
            checkBoxes.Clear();
            professionButtons.Clear();
            peculiarityButtons.Clear();
            subLineUpPanels.Clear();
            
            // 5. 重新初始化子阵容图片框数组
            subLineUpPictureBoxes = new HeroPictureBox[3, _iAppConfigService.CurrentConfig.MaxOfChoices];
        }

        private void ClearTabPageControls(TabPage tabPage)
        {
            // 创建临时列表避免修改集合时遍历
            var controlsToRemove = new List<Control>();
            foreach (Control control in tabPage.Controls)
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
                tabPage.Controls.Remove(control);
                control.Dispose();
            }
        }

        private void ClearPanelControls(Panel panel)
        {
            // 创建临时列表避免修改集合时遍历
            var controlsToRemove = new List<Control>();
            foreach (Control control in panel.Controls)
            {
                // 只移除我们创建的控件（标签、复选框、图片框）
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

        private void ClearSubLineUpComponents()
        {
            // 移除并释放所有子阵容面板
            foreach (var panel in subLineUpPanels)
            {
                if (_form.Controls.Contains(panel))
                {
                    _form.Controls.Remove(panel);
                }
                panel.Dispose();
            }

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
       public Size GetSubLineUpPanelSizes(int index)
        {
            switch (index)
            {
                case 0:
                    return subLineUpPanelSizes[0];
                    break;
                case 1:
                    return subLineUpPanelSizes[1];
                    break;              
                default:
                    return subLineUpPanelSizes[0];
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
            switch(index)
            {               
                case 1:
                    return cost1Color;
                    break;
                case 2:
                    return cost2Color;
                    break;
                case 3:
                    return cost3Color;
                    break;
                case 4:
                    return cost4Color;
                    break;
                case 5:
                    return cost5Color;
                    break;
                default:
                    return SystemColors.Control;
            }
        }
    }
}
