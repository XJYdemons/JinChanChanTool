﻿using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Diagnostics;
using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 英雄数据文件编辑窗口
    /// </summary>
    public partial class HeroInfoEditorForm : Form
    {
        /// <summary>
        /// 英雄数据服务实例
        /// </summary>
        private IHeroDataService _iheroDataService;

        /// <summary>
        /// 默认图片
        /// </summary>
        private Image defaultImage;

        /// <summary>
        /// 是否发生改动的标志
        /// </summary>
        private bool isChanged;

        public HeroInfoEditorForm()
        {
            InitializeComponent();
            // 添加自定义标题栏
            CustomTitleBar titleBar = new CustomTitleBar(this,32, null, "OCR结果纠正编辑器", CustomTitleBar.ButtonOptions.None);
            this.Controls.Add(titleBar);
            isChanged = false;
            _iheroDataService = new HeroDataService();

            // 获取所有数据集的名称（对应子目录名）
            comboBox_赛季文件选择器.Items.Clear();
            foreach (var path in _iheroDataService.Paths)
            {
                comboBox_赛季文件选择器.Items.Add(Path.GetFileName(path));
            }
            if (comboBox_赛季文件选择器.Items.Count > 0)
            {
                comboBox_赛季文件选择器.SelectedIndex = 0;
                _iheroDataService.PathIndex = 0;
            }
            // 加载或创建默认图片
            LoadDefaultImage();
            // 加载数据

            InitializeDataGridViewColumns(); // 只初始化列一次
            BindDataGridView(); // 绑定数据
        }

        private void HeroInfoEditorForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载默认图片
        /// </summary>
        private void LoadDefaultImage()
        {
            try
            {
                if (File.Exists(_iheroDataService.DefaultImagePath))
                {
                    defaultImage = Image.FromFile(_iheroDataService.DefaultImagePath);
                }
                else
                {
                    MessageBox.Show($"找不到默认英雄图片\"defaultHeroIcon.png\"\n路径：\n{_iheroDataService.DefaultImagePath}",
                                   "默认英雄图片缺失",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
                    defaultImage = new Bitmap(64, 64);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"默认英雄图片\"defaultHeroIcon.png\"加载失败\n路径：\n{_iheroDataService.DefaultImagePath}",
                                    "加载默认图片失败",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                    );
                defaultImage = new Bitmap(64, 64);
            }
        }

        /// <summary>
        /// 初始化列
        /// </summary>
        private void InitializeDataGridViewColumns()
        {
            // 清除现有列
            dataGridView_英雄数据编辑器.Columns.Clear();

            // 设置行高为32像素
            dataGridView_英雄数据编辑器.RowTemplate.Height = 32;

            // 添加图片列
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "图片";
            imageColumn.Name = "Image";
            imageColumn.Width = 32;
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_英雄数据编辑器.Columns.Add(imageColumn);

            // 添加名称列
            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.HeaderText = "英雄名称";
            nameColumn.Name = "HeroName";
            nameColumn.DataPropertyName = "HeroName";
            nameColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_英雄数据编辑器.Columns.Add(nameColumn);

            // 添加费用列
            DataGridViewTextBoxColumn costColumn = new DataGridViewTextBoxColumn();
            costColumn.HeaderText = "费用";
            costColumn.Name = "Cost";
            costColumn.DataPropertyName = "Cost";
            costColumn.Width = 50;
            costColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_英雄数据编辑器.Columns.Add(costColumn);

            // 添加职业列
            DataGridViewTextBoxColumn professionColumn = new DataGridViewTextBoxColumn();
            professionColumn.HeaderText = "职业";
            professionColumn.Name = "Profession";
            professionColumn.DataPropertyName = "Profession";
            professionColumn.Width = 205;
            professionColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_英雄数据编辑器.Columns.Add(professionColumn);

            // 添加特性列
            DataGridViewTextBoxColumn peculiarityColumn = new DataGridViewTextBoxColumn();
            peculiarityColumn.HeaderText = "特性";
            peculiarityColumn.Name = "Peculiarity";
            peculiarityColumn.DataPropertyName = "Peculiarity";
            peculiarityColumn.Width = 205;
            peculiarityColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_英雄数据编辑器.Columns.Add(peculiarityColumn);

            // 添加ID列
            DataGridViewTextBoxColumn IDColumn = new DataGridViewTextBoxColumn();
            IDColumn.HeaderText = "ID";
            IDColumn.Name = "ChessId";
            IDColumn.DataPropertyName = "ChessId";
            IDColumn.Width = 50;
            IDColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_英雄数据编辑器.Columns.Add(IDColumn);

            // 绑定事件
            dataGridView_英雄数据编辑器.CellFormatting += DataGridView_CellFormatting;
            dataGridView_英雄数据编辑器.CellValueChanged += DataGridView_CellValueChanged;
            dataGridView_英雄数据编辑器.DataError += DataGridView_DataError;

            //设置不为数据自动创建列
            dataGridView_英雄数据编辑器.AutoGenerateColumns = false;
        }

        /// <summary>
        /// 绑定数据到DataGridView
        /// </summary>
        private void BindDataGridView()
        {
            // 绑定数据
            dataGridView_英雄数据编辑器.DataSource = null;
            dataGridView_英雄数据编辑器.DataSource = new BindingList<HeroData>(_iheroDataService.HeroDatas);
        }

        /// <summary>
        /// 在单元格格式化时动态加载图片。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //判断是否是名称列
            if (dataGridView_英雄数据编辑器.Columns[e.ColumnIndex].Name == "Image" && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_英雄数据编辑器.Rows[e.RowIndex];
                string heroName = row.Cells["HeroName"].Value?.ToString();

                //如果名称不为空
                if (!string.IsNullOrEmpty(heroName))
                {
                    //加载图片
                    HeroData hero = _iheroDataService.HeroDatas.FirstOrDefault(h => h.HeroName == heroName);
                    if (hero != null && _iheroDataService.HeroDataToImageMap.TryGetValue(hero, out Image image))
                    {
                        e.Value = image;
                    }
                    else
                    {
                        e.Value = defaultImage;
                    }
                }
                else
                {
                    e.Value = defaultImage;
                }

                e.FormattingApplied = true;
            }
        }

        /// <summary>
        /// 当单元格值更改时，如果是英雄名称列，刷新图片列。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 如果修改的是英雄名称列，刷新图片
            if (e.ColumnIndex >= 0 && dataGridView_英雄数据编辑器.Columns[e.ColumnIndex].Name == "HeroName")
            {
                dataGridView_英雄数据编辑器.InvalidateRow(e.RowIndex);// 触发重绘，刷新图片
            }
        }

        /// <summary>
        /// 处理DataGridView中的数据错误，防止程序崩溃。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // 处理数据错误（例如Cost列输入非数字值）
            if (dataGridView_英雄数据编辑器.Columns[e.ColumnIndex].Name == "Cost")
            {
                MessageBox.Show("费用必须为数字！");
                e.ThrowException = false;
            }
        }

        /// <summary>
        /// 添加新英雄。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, EventArgs e)
        {
            // 添加新英雄
            var newHero = new HeroData();
            _iheroDataService.HeroDatas.Add(newHero);
            _iheroDataService.HeroDataToImageMap[newHero] = defaultImage;
            _iheroDataService.ImageToHeroDataMap[defaultImage] = newHero;
            isChanged = true;
            // 刷新数据绑定
            BindDataGridView();

            // 滚动到最后一行
            dataGridView_英雄数据编辑器.FirstDisplayedScrollingRowIndex = dataGridView_英雄数据编辑器.RowCount - 1;
            int focusIndex = dataGridView_英雄数据编辑器.RowCount - 2;
            //当焦点行索引有效时，设置当前单元格为该行的第一列
            if (dataGridView_英雄数据编辑器.RowCount - 1 >= 0)
            {
                dataGridView_英雄数据编辑器.CurrentCell = dataGridView_英雄数据编辑器.Rows[focusIndex].Cells[0];
            }
            else
            {
                // 否则设置为第一行的第一列（如果存在）
                dataGridView_英雄数据编辑器.CurrentCell = dataGridView_英雄数据编辑器.Rows[0].Cells[0];
            }

        }

        /// <summary>
        /// 删除选中的英雄行，支持多选。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleltButton_Click(object sender, EventArgs e)
        {
            var selectedRows = new List<DataGridViewRow>();

            // 获取通过行头选中的行
            foreach (DataGridViewRow row in dataGridView_英雄数据编辑器.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    selectedRows.Add(row);
                }
            }

            // 获取通过单元格选中的行（但不在SelectedRows中的行）
            foreach (DataGridViewCell cell in dataGridView_英雄数据编辑器.SelectedCells)
            {
                if (!cell.OwningRow.IsNewRow && !selectedRows.Contains(cell.OwningRow))
                {
                    selectedRows.Add(cell.OwningRow);
                }
            }

            if (selectedRows.Count == 0)
            {
                MessageBox.Show("请先选择要删除的行！");
                return;
            }

            // 记录删除前的滚动位置
            int firstDisplayedIndex = dataGridView_英雄数据编辑器.FirstDisplayedScrollingRowIndex;
            int lastDisplayedIndex = firstDisplayedIndex + dataGridView_英雄数据编辑器.DisplayedRowCount(false) - 1;

            // 从后往前删除，避免索引问题
            var selectedIndices = selectedRows
                .Select(r => r.Index)
                .OrderByDescending(i => i)
                .ToList();

            // 检查所有删除行是否都在显示范围内
            bool allInView = selectedIndices.All(index => index >= firstDisplayedIndex && index <= lastDisplayedIndex);

            // 找到最上面的删除行
            int minIndex = selectedIndices.Min();

            foreach (int index in selectedIndices)
            {
                if (index < _iheroDataService.HeroDatas.Count)
                {
                    var hero = _iheroDataService.HeroDatas[index];
                    _iheroDataService.HeroDatas.RemoveAt(index);

                    if (_iheroDataService.HeroDataToImageMap.ContainsKey(hero))
                    {
                        var image = _iheroDataService.HeroDataToImageMap[hero];
                        _iheroDataService.HeroDataToImageMap.Remove(hero);
                        _iheroDataService.ImageToHeroDataMap.Remove(image);
                    }
                }
            }
            isChanged = true;
            BindDataGridView();

            // 计算新的焦点行索引为删除的首行的上一行
            int focusIndex = selectedIndices[selectedIndices.Count - 1] - 1;

            // 当焦点行索引有效时，设置当前单元格为该行的第一列
            if (focusIndex >= 0)
            {
                dataGridView_英雄数据编辑器.CurrentCell = dataGridView_英雄数据编辑器.Rows[focusIndex].Cells[0];
            }
            else
            {
                // 否则设置为第一行的第一列（如果存在）
                dataGridView_英雄数据编辑器.CurrentCell = dataGridView_英雄数据编辑器.Rows[0].Cells[0];
            }

            // 根据删除行位置决定是否滚动
            if (allInView)
            {
                // 所有删除行都在显示范围内，不滚动
                if (firstDisplayedIndex >= 0 && firstDisplayedIndex < dataGridView_英雄数据编辑器.RowCount)
                {
                    dataGridView_英雄数据编辑器.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
                }
            }
            else
            {
                // 有删除行不在显示范围内，滚动到最上面的删除行上面的那一行
                int scrollIndex = minIndex - 1;
                if (scrollIndex < 0)
                {
                    scrollIndex = 0;
                }
                if (scrollIndex < dataGridView_英雄数据编辑器.RowCount)
                {
                    dataGridView_英雄数据编辑器.FirstDisplayedScrollingRowIndex = scrollIndex;
                }
            }

        }

        /// <summary>
        /// 退出编辑并关闭窗口。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (isChanged)
            {
                var result = MessageBox.Show("存在未保存的更改，是否保存？", "未保存的更改", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Save();
                    isChanged = false;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
                else
                {

                }
            }
            this.Close();
        }

        /// <summary>
        /// 上移按钮点击事件，将当前单元格所在行向上移动一行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upButton_Click(object sender, EventArgs e)
        {
            // 检查是否有当前单元格
            if (dataGridView_英雄数据编辑器.CurrentCell == null)
            {
                MessageBox.Show("请先选择要移动的行！");
                return;
            }
            // 获取当前行索引
            int currentIndex = dataGridView_英雄数据编辑器.CurrentCell.RowIndex;
            // 检查是否可以上移（不是第一行）
            if (currentIndex <= 0)
            {
                MessageBox.Show("已经是第一行，无法上移！");
                return;
            }
            // 记录滚动位置
            int firstDisplayedIndex = dataGridView_英雄数据编辑器.FirstDisplayedScrollingRowIndex;
            // 交换数据源中的位置
            HeroData temp = _iheroDataService.HeroDatas[currentIndex];
            _iheroDataService.HeroDatas[currentIndex] = _iheroDataService.HeroDatas[currentIndex - 1];
            _iheroDataService.HeroDatas[currentIndex - 1] = temp;
            isChanged = true;
            BindDataGridView();

            // 重新选中移动后的行
            dataGridView_英雄数据编辑器.ClearSelection();
            dataGridView_英雄数据编辑器.Rows[currentIndex - 1].Selected = true;
            dataGridView_英雄数据编辑器.CurrentCell = dataGridView_英雄数据编辑器.Rows[currentIndex - 1].Cells[0];

            // 恢复滚动位置
            if (firstDisplayedIndex >= 0 && firstDisplayedIndex < dataGridView_英雄数据编辑器.RowCount)
            {
                dataGridView_英雄数据编辑器.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
            }

        }

        /// <summary>
        /// 下移按钮点击事件，将当前单元格所在行向下移动一行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downButton_Click(object sender, EventArgs e)
        {
            // 检查是否有当前单元格
            if (dataGridView_英雄数据编辑器.CurrentCell == null)
            {
                MessageBox.Show("请先选择要移动的行！");
                return;
            }

            // 获取当前行索引
            int currentIndex = dataGridView_英雄数据编辑器.CurrentCell.RowIndex;

            // 检查是否可以下移（不是最后一行）
            if (currentIndex >= _iheroDataService.HeroDatas.Count - 1)
            {
                MessageBox.Show("已经是最后一行，无法下移！");
                return;
            }

            // 记录滚动位置
            int firstDisplayedIndex = dataGridView_英雄数据编辑器.FirstDisplayedScrollingRowIndex;

            // 交换数据源中的位置
            HeroData temp = _iheroDataService.HeroDatas[currentIndex];
            _iheroDataService.HeroDatas[currentIndex] = _iheroDataService.HeroDatas[currentIndex + 1];
            _iheroDataService.HeroDatas[currentIndex + 1] = temp;
            isChanged = true;
            BindDataGridView();

            // 重新选中移动后的行
            dataGridView_英雄数据编辑器.ClearSelection();
            dataGridView_英雄数据编辑器.Rows[currentIndex + 1].Selected = true;
            dataGridView_英雄数据编辑器.CurrentCell = dataGridView_英雄数据编辑器.Rows[currentIndex + 1].Cells[0];

            // 恢复滚动位置
            if (firstDisplayedIndex >= 0 && firstDisplayedIndex < dataGridView_英雄数据编辑器.RowCount)
            {
                dataGridView_英雄数据编辑器.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
            }

        }

        /// <summary>
        /// 下拉框的当前选中项发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isChanged)
            {
                var result = MessageBox.Show("存在未保存的更改，是否保存？", "未保存的更改", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Save();
                    isChanged = false;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    isChanged = false;
                }
            }
            // 更新当前选中的数据集索引
            _iheroDataService.PathIndex = comboBox_赛季文件选择器.SelectedIndex;

            // 重新加载英雄数据
            _iheroDataService.ReLoad();

            BindDataGridView();
        }

        /// <summary>
        /// 保存按钮被按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Save();
            isChanged = false;
            MessageBox.Show("保存成功！重启应用后生效。", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 保存到本地
        /// </summary>
        private void Save()
        {
            // 结束编辑
            dataGridView_英雄数据编辑器.EndEdit();
            _iheroDataService.Save();
        }

        /// <summary>
        /// 打开目录按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (_iheroDataService.Paths.Length > 0 && _iheroDataService.PathIndex < _iheroDataService.Paths.Length)
            {
                string path = _iheroDataService.Paths[_iheroDataService.PathIndex];
                try
                {
                    // 使用资源管理器打开目录
                    Process.Start("explorer.exe", path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"打开目录失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("没有可用的数据集目录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}