using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices.Interface;
using System.Diagnostics;

namespace JinChanChanTool.DIYComponents
{
    /// <summary>
    /// 显示装备信息专用的自定义提示框，显示装备名称和合成路径
    /// </summary>
    public class EquipmentInformationToolTip : ToolTip
    {
        private readonly IEquipmentService _equipmentService;
        private readonly Control _parentControl;

        private int IMAGE_SIZE => _parentControl.LogicalToDeviceUnits(32); //合成路径图像大小
        private int PADDING => _parentControl.LogicalToDeviceUnits(8); //内边距
        private int MARGIN => _parentControl.LogicalToDeviceUnits(4); //外边距
        private int TEXT_HEIGHT => _parentControl.LogicalToDeviceUnits(20); //文本高度
        private int PLUS_SIGN_WIDTH => _parentControl.LogicalToDeviceUnits(20); //"+"号的宽度

        public EquipmentInformationToolTip(IEquipmentService equipmentService, Control parentControl)
        {
            _equipmentService = equipmentService;
            _parentControl = parentControl;

            this.OwnerDraw = true;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;

            this.Popup += OnPopup;
            this.Draw += OnDraw;           
        }

        /// <summary>
        /// 设置当前要显示的装备
        /// </summary>
        public void SetEquipment(Control control)
        {
            // 设置一个占位文本（必须非空才能触发ToolTip显示），实际绘制由OnDraw处理
            // 装备对象通过控件的Tag属性获取
            this.SetToolTip(control, " ");
        }

        /// <summary>
        /// 刷新控件的ToolTip状态：有装备时设置ToolTip，无装备时移除ToolTip
        /// 用于在装备数据更新后调用，避免空装备槽在高DPI环境下的卡顿问题
        /// </summary>
        public void RefreshEquipmentToolTip(Control control)
        {
            if (control == null) return;

            if (control.Tag is Equipment)
            {
                // 有装备时设置ToolTip占位符
                this.SetToolTip(control, " ");
            }
            else
            {
                // 无装备时移除ToolTip，完全避免触发弹出事件
                this.SetToolTip(control, null);
            }
        }

        private void OnPopup(object? sender, PopupEventArgs e)
        {
            // 尽早检查关联控件是否有效
            if (e.AssociatedControl == null)
            {
                e.Cancel = true;
                return;
            }

            // 从关联控件的Tag获取当前装备
            Equipment equipment = e.AssociatedControl.Tag as Equipment;
            if (equipment == null)
            {
                e.Cancel = true;
                // 关键修复：主动隐藏ToolTip，避免在高DPI环境下可能的重复触发循环
                this.Hide(e.AssociatedControl);
                return;
            }

            int width, height;
            bool hasRecipe = equipment.SyntheticPathway != null && equipment.SyntheticPathway.Length >= 2;

            if (hasRecipe)
            {
                // 有合成路径：显示名称 + 两个散件图片
                int imagesWidth = IMAGE_SIZE * 2 + MARGIN * 2 + PLUS_SIGN_WIDTH;
                width = Math.Max(GetTextWidth(equipment.Name), imagesWidth) + PADDING * 2;
                height = TEXT_HEIGHT + MARGIN + IMAGE_SIZE + PADDING * 2;
            }
            else
            {
                // 无合成路径：只显示名称
                width = GetTextWidth(equipment.Name) + PADDING * 2;
                height = TEXT_HEIGHT + PADDING * 2;
            }

            e.ToolTipSize = new Size(width, height);
        }

        private void OnDraw(object? sender, DrawToolTipEventArgs e)
        {
            // 绘制背景
            using (var brush = new SolidBrush(Color.FromArgb(45, 45, 48)))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            // 绘制边框
            using (var pen = new Pen(Color.FromArgb(100, 100, 100), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

            // 从关联控件的Tag获取当前装备
            Equipment equipment = e.AssociatedControl?.Tag as Equipment;
            if (equipment == null) return;

            bool hasRecipe = equipment.SyntheticPathway != null && equipment.SyntheticPathway.Length >= 2;

            // 绘制装备名称
            using (var brush = new SolidBrush(Color.White))
            using (var font = new Font("Microsoft YaHei UI", 9f, FontStyle.Bold, GraphicsUnit.Point))
            {
                var textRect = new Rectangle(PADDING, PADDING, e.Bounds.Width - PADDING * 2, TEXT_HEIGHT);
                var format = new StringFormat { Alignment = StringAlignment.Center };
                e.Graphics.DrawString(equipment.Name, font, brush, textRect, format);
            }

            // 如果有合成路径，绘制散件图片
            if (hasRecipe)
            {
                int imagesY = PADDING + TEXT_HEIGHT + MARGIN;
                int totalImagesWidth = IMAGE_SIZE * 2 + MARGIN * 2 + PLUS_SIGN_WIDTH;
                int startX = (e.Bounds.Width - totalImagesWidth) / 2;

                // 获取第一个散件图片
                var component1 = _equipmentService.GetEquipmentFromName(equipment.SyntheticPathway[0]);
                if (component1?.Image != null)
                {
                    e.Graphics.DrawImage(component1.Image, new Rectangle(startX, imagesY, IMAGE_SIZE, IMAGE_SIZE));
                }

                // 绘制"+"号
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Microsoft YaHei UI", 12f, FontStyle.Bold, GraphicsUnit.Point))
                {
                    int plusX = startX + IMAGE_SIZE + MARGIN;
                    var plusRect = new Rectangle(plusX, imagesY, PLUS_SIGN_WIDTH, IMAGE_SIZE);
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString("+", font, brush, plusRect, format);
                }

                // 获取第二个散件图片
                var component2 = _equipmentService.GetEquipmentFromName(equipment.SyntheticPathway[1]);
                if (component2?.Image != null)
                {
                    int secondImageX = startX + IMAGE_SIZE + MARGIN + PLUS_SIGN_WIDTH + MARGIN;
                    e.Graphics.DrawImage(component2.Image, new Rectangle(secondImageX, imagesY, IMAGE_SIZE, IMAGE_SIZE));
                }
            }
        }

        private int GetTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return _parentControl.LogicalToDeviceUnits(80);
            using (var font = new Font("Microsoft YaHei UI", 9f, FontStyle.Bold, GraphicsUnit.Point))
            using (var g = Graphics.FromHwnd(_parentControl.Handle))
            {
                var size = g.MeasureString(text, font);
                return (int)Math.Ceiling(size.Width);
            }
        }
    }
}
