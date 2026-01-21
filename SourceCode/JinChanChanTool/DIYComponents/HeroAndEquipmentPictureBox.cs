using JinChanChanTool.DataClass;
using JinChanChanTool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 用于显示英雄及其三件装备的复合自定义组件
    /// </summary>
    public partial class HeroAndEquipmentPictureBox : UserControl
    {
        private bool _resizing;//防重入标志，用于防止在布局调整过程中发生无限递归或不必要的重复计算。
        public List<HeroPictureBox> EquipmentPictureBoxes { get; set; }


        public void SetHero(Hero hero, UIBuilderService ui,Equipment equipment1=null,Equipment equipment2=null,Equipment equipment3=null)
        {


            heroPictureBox.Image = hero.Image;
            heroPictureBox.Tag = hero;
            heroPictureBox.BorderColor = ui.GetColor(hero.Cost);

            // 设置装备图片框的Image和Tag（Tag存储Equipment对象用于ToolTip显示）
            equipmentPictureBox1.Image = equipment1?.Image;
            equipmentPictureBox1.Tag = equipment1;
            equipmentPictureBox1.BorderWidth = equipment1 == null ? 1 : 0;

            equipmentPictureBox2.Image = equipment2?.Image;
            equipmentPictureBox2.Tag = equipment2;
            equipmentPictureBox2.BorderWidth = equipment2 == null ? 1 : 0;

            equipmentPictureBox3.Image = equipment3?.Image;
            equipmentPictureBox3.Tag = equipment3;
            equipmentPictureBox3.BorderWidth = equipment3 == null ? 1 : 0;

            equipmentPictureBox1.BorderColor = Color.Gray;
            equipmentPictureBox2.BorderColor = Color.Gray;
            equipmentPictureBox3.BorderColor = Color.Gray;

        }

        public HeroAndEquipmentPictureBox()
        {
            InitializeComponent();

            EquipmentPictureBoxes = new List<HeroPictureBox>
            {
                equipmentPictureBox1,
                equipmentPictureBox2,
                equipmentPictureBox3
            };

            Resize += HeroAndEquipmentPictureBox_Resize;
        }

        /// <summary>
        /// 响应DPI变化（PerMonitorV2模式下跨显示器时触发）
        /// </summary>
        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            // DPI变化后重新布局
            LayoutChildren();
        }

        private void HeroAndEquipmentPictureBox_Resize(object? sender, EventArgs e)
        {
            if (_resizing) return;
            _resizing = true;

            LayoutChildren();

            _resizing = false;
        }


        private void LayoutChildren()
        {
            int width = Width;
            int height = Height;

            if (width <= 0 || height <= 0) return;

            // 设计比例常量（基于组件宽度）
            const float EQUIP_TO_HERO_RATIO = 16f / 52f;    // 装备高度相对于组件宽度的比例
            const float MARGIN_TO_HERO_RATIO = 2f / 52f;    // 边距相对于组件宽度的比例

            // 使用组件宽度作为内容宽度（填满整个宽度，无左右空白）
            int contentWidth = width;

            // 英雄头像使用组件宽度，并保持正方形
            int heroSize = contentWidth;

            // 根据组件宽度计算其他尺寸
            int margin = Math.Max((int)Math.Round(contentWidth * MARGIN_TO_HERO_RATIO), 1);
            int equipHeight = Math.Max((int)Math.Round(contentWidth * EQUIP_TO_HERO_RATIO), 1);

            /* ---------- Hero 区（左上角对齐，宽度填满，高度等于宽度形成正方形）---------- */
            panelHero.Bounds = new Rectangle(0, margin, heroSize, heroSize);
            heroPictureBox.Bounds = new Rectangle(0, 0, heroSize, heroSize);

            /* ---------- Equip 区（在英雄下方，宽度填满）---------- */
            int equipY = margin + heroSize + margin;
            panelEquip.Bounds = new Rectangle(0, equipY, contentWidth, equipHeight);

            // 装备尺寸（正方形，使用装备区高度）
            int equipSize = equipHeight;

            // 计算三个装备之间的总间隙并均分
            int totalGap = contentWidth - 3 * equipSize;
            int gap1 = totalGap / 2;  // 向下取整，左侧间隙

            // 第1号装备：左对齐
            equipmentPictureBox1.Bounds = new Rectangle(0, 0, equipSize, equipSize);

            // 第2号装备：居中
            int equip2X = equipSize + gap1;
            equipmentPictureBox2.Bounds = new Rectangle(equip2X, 0, equipSize, equipSize);

            // 第3号装备：右对齐
            int equip3X = contentWidth - equipSize;
            equipmentPictureBox3.Bounds = new Rectangle(equip3X, 0, equipSize, equipSize);
        }



        public void Clear()
        {

            heroPictureBox.Image = null ;
            heroPictureBox.Tag = null;
            heroPictureBox.BorderColor = Color.Transparent ;

                 equipmentPictureBox1.Image = null;
                equipmentPictureBox1.Tag = null;
            equipmentPictureBox1.BorderColor = Color.Transparent;

            equipmentPictureBox2.Image = null;
                equipmentPictureBox2.Tag = null;
            equipmentPictureBox2.BorderColor = Color.Transparent;

            equipmentPictureBox3.Image = null;
                equipmentPictureBox3.Tag = null;
            equipmentPictureBox3.BorderColor = Color.Transparent;
        }

        /// <summary>
        /// 为所有子控件绑定拖动事件，确保整个组件任意位置都可以拖动窗体
        /// </summary>
        /// <param name="bindAction">绑定拖动的委托方法，通常是窗体的"绑定拖动"方法</param>
        public void BindFormDrag(Action<Control> bindAction)
        {
            // 为主容器绑定拖动
            bindAction(this);
            bindAction(panelHero);
            bindAction(panelEquip);

            // 为英雄头像框绑定拖动
            bindAction(heroPictureBox);

            // 为所有装备框绑定拖动
            bindAction(equipmentPictureBox1);
            bindAction(equipmentPictureBox2);
            bindAction(equipmentPictureBox3);
        }
    }
}
