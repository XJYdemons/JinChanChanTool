using JinChanChanTool.DataClass;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using JinChanChanTool.Services.DataServices;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 自定义PictureBox，支持边框颜色和宽度设置
    /// </summary>
    public class HeroPictureBox : PictureBox
    {
        /// <summary>
        /// 默认边框颜色
        /// </summary>
        public Color BorderColor { get; set; } = SystemColors.Control;

        /// <summary>
        /// 默认边框宽度
        /// </summary>
        public int BorderWidth { get; set; } = 3;

        #region 装备推荐功能
        public HeroData CurrentHero { get; set; }
        private ToolTip _equipmentToolTip;
        private List<string> _currentRecommendedItems;

        // 定义一些常量，方便调整外观
        private const int ITEM_SIZE = 48; // 图片大小
        private const int PADDING = 5;    // 提示框内边距
        private const int MARGIN = 3;     // 图片间距

        public HeroPictureBox()
        {
            _equipmentToolTip = new ToolTip();
            //开启 OwnerDraw 模式
            _equipmentToolTip.OwnerDraw = true;
            _equipmentToolTip.BackColor = Color.FromArgb(30, 30, 30);

            //订阅两个关键事件：Popup (用于确定尺寸) 和 Draw (用于绘制)
            _equipmentToolTip.Popup += EquipmentToolTip_Popup;
            _equipmentToolTip.Draw += EquipmentToolTip_Draw;

            // 将 ToolTip 关联到这个控件上。文本不用管，单纯只是为了激活它。
            _equipmentToolTip.SetToolTip(this, " ");
        }

        /// <summary>
        /// 在 ToolTip 弹出前触发，用于计算并设置其尺寸
        /// </summary>
        private void EquipmentToolTip_Popup(object sender, PopupEventArgs e)
        {
            // 检查控件是否关联了一个英雄
            if (CurrentHero == null)
            {
                e.Cancel = true; // 取消弹出
                return;
            }
            // 1. 直接通过单例 Instance，调用 GetItemsForHero 方法获取装备
            _currentRecommendedItems = EquipmentService.Instance.GetItemsForHero(CurrentHero.HeroName);

            // 2. 检查是否真的找到了推荐装备
            if (_currentRecommendedItems == null || !_currentRecommendedItems.Any())
            {
                e.Cancel = true; // 如果没找到，就取消本次弹出
                return;
            }

            // 3. 如果找到了，就用这个临时的列表来计算尺寸
            int itemCount = _currentRecommendedItems.Count;
            int width = (itemCount * ITEM_SIZE) + ((itemCount - 1) * MARGIN) + (PADDING * 2);
            int height = ITEM_SIZE + (PADDING * 2);

            // 设置 ToolTip 的最终尺寸
            e.ToolTipSize = new Size(width, height);
        }

        /// <summary>
        /// 绘制 ToolTip 的内容
        /// </summary>
        private void EquipmentToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            // 1. 绘制深色背景
            e.DrawBackground();

            // 检查是否有英雄数据
            if (CurrentHero == null) return;

            // 如果临时的装备列表是空的，也直接返回
            if (_currentRecommendedItems == null) return;

            // 2. 循环绘制每个装备图片
            int currentX = PADDING; // 起始X坐标
            foreach (var itemName in _currentRecommendedItems)
            {
                string imagePath = EquipmentService.Instance.GetEquipmentImagePath(itemName);
                if (File.Exists(imagePath))
                {
                    // 使用 using 语句确保图片资源被正确释放
                    using (Image itemImage = Image.FromFile(imagePath))
                    {
                        // 计算图片绘制的位置和大小
                        var targetRect = new Rectangle(currentX, PADDING, ITEM_SIZE, ITEM_SIZE);

                        // 将图片绘制到 ToolTip 的画布上
                        e.Graphics.DrawImage(itemImage, targetRect);

                        // 更新下一个图片的X坐标
                        currentX += ITEM_SIZE + MARGIN;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // 绘制边框
            // 确保边框宽度至少为1
            int drawWidth = Math.Max(1, BorderWidth);
            using (Pen pen = new Pen(BorderColor, drawWidth))
            {
                // 调整矩形位置和大小，确保粗边框完整显示
                Rectangle rect = new Rectangle(
                    drawWidth / 2,          // X位置向内偏移
                    drawWidth / 2,          // Y位置向内偏移
                    Width - drawWidth ,  // 宽度减去边框厚度
                    Height - drawWidth  // 高度减去边框厚度
                );
                e.Graphics.DrawRectangle(pen, rect);
            }
        }
    }
}
