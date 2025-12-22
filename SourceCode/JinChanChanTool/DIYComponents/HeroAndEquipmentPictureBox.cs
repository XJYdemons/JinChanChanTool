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

            // 设计比例常量
            // 装备大小：17/52 ≈ 0.33，装备区超出英雄区约 15%~20% 的装备宽度
            const float EQUIP_TO_HERO_RATIO = 20f / 52f;    // 装备大小相对于英雄头像的比例
            const float MARGIN_TO_HERO_RATIO = 2f / 52f;    // 边距相对于英雄头像的比例
            const float GAP_TO_EQUIP_RATIO = 2f / 20f;      // 装备间距相对于装备大小的比例

            // 计算英雄头像可用的最大尺寸
            // 高度方向：margin + heroSize + margin + equipSize + margin = height
            float heightRatio = 3 * MARGIN_TO_HERO_RATIO + 1 + EQUIP_TO_HERO_RATIO;
            int heroSizeFromHeight = (int)(height / heightRatio);

            // 宽度方向：装备区可能比英雄区宽，需要考虑装备区的宽度
            // 装备区宽度 = 3 * equipSize + 2 * equipGap = heroSize * (3 * EQUIP_TO_HERO_RATIO + 2 * EQUIP_TO_HERO_RATIO * GAP_TO_EQUIP_RATIO)
            float equipAreaWidthRatio = 3 * EQUIP_TO_HERO_RATIO + 2 * EQUIP_TO_HERO_RATIO * GAP_TO_EQUIP_RATIO;
            // 取装备区和英雄区中较宽的作为约束
            float maxWidthRatio = Math.Max(1f, equipAreaWidthRatio);
            int heroSizeFromWidth = (int)(width / maxWidthRatio);

            // 取较小值确保不超出边界
            int heroSize = Math.Min(heroSizeFromHeight, heroSizeFromWidth);

            // 确保英雄头像至少有 1 像素
            heroSize = Math.Max(heroSize, 1);

            // 根据英雄头像计算其他尺寸
            int margin = Math.Max((int)(heroSize * MARGIN_TO_HERO_RATIO), 1);
            int equipSize = Math.Max((int)(heroSize * EQUIP_TO_HERO_RATIO), 1);
            int equipGap = Math.Max((int)(equipSize * GAP_TO_EQUIP_RATIO), 1);

            // 3个装备 + 2个间距的总宽度
            int totalEquipWidth = 3 * equipSize + 2 * equipGap;

            // 确保装备区与英雄区的差值为偶数，使左右超出对称
            // 如果差值是奇数，减小 totalEquipWidth 1像素（通过减小最后一个间距实现）
            int overflow = totalEquipWidth - heroSize;
            bool adjustLastGap = (overflow % 2 != 0);
            if (adjustLastGap)
            {
                totalEquipWidth -= 1;
            }

            // 计算内容总高度（用于垂直居中）
            int totalContentHeight = margin + heroSize + margin + equipSize + margin;
            int topOffset = Math.Max(0, (height - totalContentHeight) / 2);

            /* ---------- Hero 区（水平居中，垂直居中）---------- */
            int heroX = (width - heroSize) / 2;
            int heroY = topOffset + margin;

            panelHero.Bounds = new Rectangle(heroX, heroY, heroSize, heroSize);
            heroPictureBox.Bounds = new Rectangle(0, 0, heroSize, heroSize);

            /* ---------- Equip 区（居中于英雄区，确保左右超出对称）---------- */
            int equipY = heroY + heroSize + margin;
            int equipStartX = heroX + (heroSize - totalEquipWidth) / 2;

            panelEquip.Bounds = new Rectangle(equipStartX, equipY, totalEquipWidth, equipSize);

            // 第1号装备
            equipmentPictureBox1.Bounds = new Rectangle(0, 0, equipSize, equipSize);

            // 第2号装备（居中于整个结构）
            equipmentPictureBox2.Bounds = new Rectangle(equipSize + equipGap, 0, equipSize, equipSize);

            // 第3号装备（如果调整过，最后一个间距减1像素）
            int lastGap = adjustLastGap ? equipGap - 1 : equipGap;
            equipmentPictureBox3.Bounds = new Rectangle(equipSize + equipGap + equipSize + lastGap, 0, equipSize, equipSize);
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
