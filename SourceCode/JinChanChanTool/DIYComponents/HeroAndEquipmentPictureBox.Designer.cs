using System.Drawing;
using System.Windows.Forms;

namespace JinChanChanTool.DIYComponents
{
    partial class HeroAndEquipmentPictureBox
    {
        private System.ComponentModel.IContainer components = null;

        public Panel panelHero;
        public Panel panelEquip;
        public HeroPictureBox heroPictureBox;
        public HeroPictureBox equipmentPictureBox1;
        public HeroPictureBox equipmentPictureBox2;
        public HeroPictureBox equipmentPictureBox3;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelHero = new Panel();
            heroPictureBox = new HeroPictureBox();
            panelEquip = new Panel();
            equipmentPictureBox1 = new HeroPictureBox();
            equipmentPictureBox2 = new HeroPictureBox();
            equipmentPictureBox3 = new HeroPictureBox();
            panelHero.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)heroPictureBox).BeginInit();
            panelEquip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)equipmentPictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)equipmentPictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)equipmentPictureBox3).BeginInit();
            SuspendLayout();
            // 
            // panelHero
            // 
            panelHero.Controls.Add(heroPictureBox);
            panelHero.Location = new Point(0, 0);
            panelHero.Name = "panelHero";
            panelHero.Size = new Size(200, 100);
            panelHero.TabIndex = 0;
            // 
            // heroPictureBox
            // 
            heroPictureBox.BorderColor = Color.Gray;
            heroPictureBox.BorderWidth = 3;
            heroPictureBox.IsSelected = false;
            heroPictureBox.Location = new Point(0, 0);
            heroPictureBox.Name = "heroPictureBox";
            heroPictureBox.SelectionColor = Color.FromArgb(125, 255, 0, 0);
            heroPictureBox.Size = new Size(100, 50);
            heroPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            heroPictureBox.TabIndex = 0;
            heroPictureBox.TabStop = false;
            // 
            // panelEquip
            // 
            panelEquip.Controls.Add(equipmentPictureBox1);
            panelEquip.Controls.Add(equipmentPictureBox2);
            panelEquip.Controls.Add(equipmentPictureBox3);
            panelEquip.Location = new Point(0, 0);
            panelEquip.Name = "panelEquip";
            panelEquip.Size = new Size(200, 100);
            panelEquip.TabIndex = 1;
            // 
            // equipmentPictureBox1
            // 
            equipmentPictureBox1.BackColor = Color.Transparent;
            equipmentPictureBox1.BorderColor = Color.Gray;
            equipmentPictureBox1.BorderWidth = 1;
            equipmentPictureBox1.IsSelected = false;
            equipmentPictureBox1.Location = new Point(0, 0);
            equipmentPictureBox1.Margin = new Padding(0);
            equipmentPictureBox1.Name = "equipmentPictureBox1";
            equipmentPictureBox1.SelectionColor = Color.FromArgb(125, 255, 0, 0);
            equipmentPictureBox1.Size = new Size(100, 50);
            equipmentPictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            equipmentPictureBox1.TabIndex = 0;
            equipmentPictureBox1.TabStop = false;
            // 
            // equipmentPictureBox2
            // 
            equipmentPictureBox2.BackColor = Color.Transparent;
            equipmentPictureBox2.BorderColor = Color.Gray;
            equipmentPictureBox2.BorderWidth = 1;
            equipmentPictureBox2.IsSelected = false;
            equipmentPictureBox2.Location = new Point(0, 0);
            equipmentPictureBox2.Margin = new Padding(0);
            equipmentPictureBox2.Name = "equipmentPictureBox2";
            equipmentPictureBox2.SelectionColor = Color.FromArgb(125, 255, 0, 0);
            equipmentPictureBox2.Size = new Size(100, 50);
            equipmentPictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            equipmentPictureBox2.TabIndex = 1;
            equipmentPictureBox2.TabStop = false;
            // 
            // equipmentPictureBox3
            // 
            equipmentPictureBox3.BackColor = Color.Transparent;
            equipmentPictureBox3.BorderColor = Color.Gray;
            equipmentPictureBox3.BorderWidth = 1;
            equipmentPictureBox3.IsSelected = false;
            equipmentPictureBox3.Location = new Point(0, 0);
            equipmentPictureBox3.Margin = new Padding(0);
            equipmentPictureBox3.Name = "equipmentPictureBox3";
            equipmentPictureBox3.SelectionColor = Color.FromArgb(125, 255, 0, 0);
            equipmentPictureBox3.Size = new Size(100, 50);
            equipmentPictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            equipmentPictureBox3.TabIndex = 2;
            equipmentPictureBox3.TabStop = false;
            // 
            // HeroAndEquipmentPictureBox
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panelHero);
            Controls.Add(panelEquip);
            Name = "HeroAndEquipmentPictureBox";
            Size = new Size(480, 640);
            panelHero.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)heroPictureBox).EndInit();
            panelEquip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)equipmentPictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)equipmentPictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)equipmentPictureBox3).EndInit();
            ResumeLayout(false);
        }
    }
}
