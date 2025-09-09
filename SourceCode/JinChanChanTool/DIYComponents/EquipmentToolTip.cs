using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinChanChanTool.DIYComponents
{
    public class EquipmentToolTip:ToolTip
    {

        private List<Image> _images;
        // 定义一些常量，方便调整外观
        private const int ITEM_SIZE = 48; // 图片大小
        private const int PADDING = 5;    // 提示框内边距
        private const int MARGIN = 3;     // 图片间距
        
        public EquipmentToolTip(List<Image> images)
        {
            OwnerDraw = true;
            BackColor = Color.FromArgb(30,30,30);
            Popup += EquipmentToolTip_Popup;
            Draw += EquipmentToolTip_Draw;
            _images = images;
        }
        /// <summary>
        /// 在 ToolTip 弹出前触发，用于计算并设置其尺寸
        /// </summary>
        private void EquipmentToolTip_Popup(object sender, PopupEventArgs e)
        { 

        }

        /// <summary>
        /// 绘制 ToolTip 的内容
        /// </summary>
        private void EquipmentToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
           
          
        }
    }
}
