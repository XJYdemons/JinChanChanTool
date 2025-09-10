using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace JinChanChanTool.DIYComponents 
{
    /// <summary>
    /// 只负责展示的自定义装备提示组件。
    /// 它完全与数据来源解耦，只接收一个图片列表并将其绘制出来。
    /// </summary>
    public class EquipmentToolTip : ToolTip
    {
        // --- 内部状态 ---
        private readonly List<Image> _images;

        // --- 外观常量 ---
        private const int IMAGE_SIZE = 48;
        private const int PADDING = 5;
        private const int MARGIN = 3;

        /// <summary>
        /// 构造函数，接收一个图片列表作为唯一的依赖。
        /// </summary>
        /// <param name="imagesToShow">需要在提示框中绘制的图片列表。</param>
        public EquipmentToolTip(List<Image> imagesToShow)
        {
            _images = imagesToShow;

            // 核心设置：开启自定义绘制模式
            this.OwnerDraw = true;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // 订阅绘制所需的两个关键事件
            this.Popup += OnPopup;
            this.Draw += OnDraw;
        }

        /// <summary>
        /// 在提示框弹出前，根据图片数量计算并设置其最终尺寸。
        /// </summary>
        private void OnPopup(object sender, PopupEventArgs e)
        {
            // 如果没有图片，或者关联的控件被禁用，则取消弹出
            if (_images == null || !_images.Any() || e.AssociatedControl.Enabled == false)
            {
                e.Cancel = true;
                return;
            }

            int itemCount = _images.Count;
            int width = (itemCount * IMAGE_SIZE) + ((itemCount - 1) * MARGIN) + (PADDING * 2);
            int height = IMAGE_SIZE + (PADDING * 2);

            e.ToolTipSize = new Size(width, height);
        }

        /// <summary>
        /// 执行具体的绘制操作。
        /// </summary>
        private void OnDraw(object sender, DrawToolTipEventArgs e)
        {
            // 绘制深色背景
            e.DrawBackground();

            if (_images == null) return;

            // 循环绘制每个图片
            int currentX = PADDING;
            foreach (var image in _images)
            {
                if (image != null)
                {
                    var targetRect = new Rectangle(currentX, PADDING, IMAGE_SIZE, IMAGE_SIZE);
                    e.Graphics.DrawImage(image, targetRect);
                    currentX += IMAGE_SIZE + MARGIN;
                }
            }
        }
    }
}