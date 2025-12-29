using JinChanChanTool.DataClass;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.Services.DataServices.Interface;
using JinChanChanTool.Tools;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JinChanChanTool.Forms
{
    /// <summary>
    /// 装备数据文件编辑窗口
    /// </summary>
    public partial class EquipmentDataEditorForm : Form
    {
        /// <summary>
        ///装备数据服务实例
        /// </summary>
        private IEquipmentService _iEquipmentService;

        /// <summary>
        /// 默认图片
        /// </summary>
        private Image defaultImage;

        /// <summary>
        /// 是否发生改动的标志
        /// </summary>
        private bool isChanged;

        public EquipmentDataEditorForm()
        {
            InitializeComponent();
            DragHelper.EnableDragForChildren(panel3);

            isChanged = false;
            _iEquipmentService = new EquipmentService();

            // 获取所有数据集的名称（对应子目录名）
            comboBox_赛季文件选择器.Items.Clear();
            foreach (string path in _iEquipmentService.GetFilePaths())
            {
                comboBox_赛季文件选择器.Items.Add(Path.GetFileName(path));
            }
            if (comboBox_赛季文件选择器.Items.Count > 0)
            {
                comboBox_赛季文件选择器.SelectedIndex = 0;
                _iEquipmentService.SetFilePathsIndex(0);
            }
            // 加载或创建默认图片
            LoadDefaultImage();
            // 加载数据

            InitializeDataGridViewColumns(); // 只初始化列一次
            BindDataGridView(); // 绑定数据
        }

        private void EquipmentDataEditorForm_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// <summary>
        /// 加载默认图片
        /// </summary>
        private void LoadDefaultImage()
        {
            try
            {
                if (File.Exists(_iEquipmentService.GetDefaultImagePath()))
                {
                    defaultImage = Image.FromFile(_iEquipmentService.GetDefaultImagePath());
                }
                else
                {
                    MessageBox.Show($"找不到默认装备图片\"defaultHeroIcon.png\"\n路径：\n{_iEquipmentService.GetDefaultImagePath()}",
                                   "默认装备图片缺失",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error
                                   );
                    defaultImage = new Bitmap(64, 64);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"默认装备图片\"defaultHeroIcon.png\"加载失败\n路径：\n{_iEquipmentService.GetDefaultImagePath()}",
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
            dataGridView_装备数据编辑器.Columns.Clear();

            // 设置行高为32像素
            dataGridView_装备数据编辑器.RowTemplate.Height = 32;

            // 添加图片列
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "图片";
            imageColumn.Name = "Image";
            imageColumn.Width = 32;
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_装备数据编辑器.Columns.Add(imageColumn);

            // 添加装备名称列
            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.HeaderText = "装备名称";
            nameColumn.Name = "Name";
            nameColumn.DataPropertyName = "Name";
            nameColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_装备数据编辑器.Columns.Add(nameColumn);

            // 添加装备类型列
            DataGridViewTextBoxColumn equipmentTypeColumn = new DataGridViewTextBoxColumn();
            equipmentTypeColumn.HeaderText = "装备类型";
            equipmentTypeColumn.Name = "EquipmentType";
            equipmentTypeColumn.DataPropertyName = "EquipmentType";
            equipmentTypeColumn.Width = 150;
            equipmentTypeColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_装备数据编辑器.Columns.Add(equipmentTypeColumn);

            // 添加合成路径列（不绑定 DataPropertyName，因为是 string[] 类型）
            DataGridViewTextBoxColumn syntheticPathwayColumn = new DataGridViewTextBoxColumn();
            syntheticPathwayColumn.HeaderText = "合成路径";
            syntheticPathwayColumn.Name = "SyntheticPathway";
            syntheticPathwayColumn.Width = 250;
            syntheticPathwayColumn.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序
            dataGridView_装备数据编辑器.Columns.Add(syntheticPathwayColumn);

            // 绑定事件
            dataGridView_装备数据编辑器.CellFormatting += DataGridView_CellFormatting;
            dataGridView_装备数据编辑器.CellParsing += DataGridView_CellParsing;
            dataGridView_装备数据编辑器.CellValueChanged += DataGridView_CellValueChanged;
            dataGridView_装备数据编辑器.DataError += DataGridView_DataError;

            // 设置不为数据自动创建列
            dataGridView_装备数据编辑器.AutoGenerateColumns = false;
        }

        /// <summary>
        /// 绑定数据到DataGridView
        /// </summary>
        private void BindDataGridView()
        {
            // 清除当前单元格选择，避免在重新绑定时触发格式化事件的索引问题
            if (dataGridView_装备数据编辑器.CurrentCell != null)
            {
                dataGridView_装备数据编辑器.CurrentCell = null;
            }

            // 绑定数据
            dataGridView_装备数据编辑器.DataSource = null;
            dataGridView_装备数据编辑器.DataSource = new BindingList<Equipment>(_iEquipmentService.GetEquipmentDatas());
        }

        /// <summary>
        /// 在单元格格式化时动态加载图片和转换 string[] 类型属性。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView_装备数据编辑器.Rows.Count) return;

            string columnName = dataGridView_装备数据编辑器.Columns[e.ColumnIndex].Name;

            // 处理图片列
            if (columnName == "Image")
            {
                DataGridViewRow row = dataGridView_装备数据编辑器.Rows[e.RowIndex];
                string equipmentName = row.Cells["Name"].Value?.ToString();

                if (!string.IsNullOrEmpty(equipmentName))
                {
                    Equipment equipment = _iEquipmentService.GetEquipmentDatas().FirstOrDefault(h => h.Name == equipmentName);
                    Image image = equipment?.Image;
                    if (equipment != null && image != null)
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
            // 处理合成路径列 - 将 string[] 转换为用 + 分隔的字符串显示
            else if (columnName == "SyntheticPathway")
            {
                try
                {
                    Equipment equipment = dataGridView_装备数据编辑器.Rows[e.RowIndex].DataBoundItem as Equipment;
                    if (equipment != null && equipment.SyntheticPathway != null)
                    {
                        e.Value = string.Join("+", equipment.SyntheticPathway);
                        e.FormattingApplied = true;
                    }
                }
                catch
                {
                    // 忽略索引越界错误，可能在数据重新绑定过程中发生
                }
            }
        }

        /// <summary>
        /// 在单元格解析时将字符串转换回 string[] 类型。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dataGridView_装备数据编辑器.Columns[e.ColumnIndex].Name;

            // 处理合成路径列 - 将用 + 分隔的字符串解析为 string[]
            if (columnName == "SyntheticPathway")
            {
                Equipment equipment = dataGridView_装备数据编辑器.Rows[e.RowIndex].DataBoundItem as Equipment;
                if (equipment != null && e.Value != null)
                {
                    string input = e.Value.ToString();
                    equipment.SyntheticPathway = input.Split('+', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(s => s.Trim())
                                           .Where(s => !string.IsNullOrWhiteSpace(s))
                                           .ToArray();
                    e.ParsingApplied = true;
                    isChanged = true;
                }
            }
        }

        /// <summary>
        /// 当单元格值更改时，如果是装备名称列，刷新图片列。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 如果修改的是装备名称列，刷新图片
            if (e.ColumnIndex >= 0 && dataGridView_装备数据编辑器.Columns[e.ColumnIndex].Name == "Name")
            {
                dataGridView_装备数据编辑器.InvalidateRow(e.RowIndex);// 触发重绘，刷新图片
            }
        }

        /// <summary>
        /// 处理DataGridView中的数据错误，防止程序崩溃。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // 处理数据错误
            MessageBox.Show("数据输入错误，请检查格式！");
            e.ThrowException = false;
        }

        /// <summary>
        /// 添加新装备。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, EventArgs e)
        {
            // 添加新装备
            Equipment newEquipment = new Equipment()
            {
                Name = "",
                EquipmentType = "",
                SyntheticPathway = new string[0]
            };
            _iEquipmentService.AddEquipment(newEquipment, defaultImage);
            isChanged = true;
            // 刷新数据绑定
            BindDataGridView();

            // 滚动到最后一行
            dataGridView_装备数据编辑器.FirstDisplayedScrollingRowIndex = dataGridView_装备数据编辑器.RowCount - 1;
            int focusIndex = dataGridView_装备数据编辑器.RowCount - 2;
            // 当焦点行索引有效时，设置当前单元格为该行的第一列
            if (dataGridView_装备数据编辑器.RowCount - 1 >= 0)
            {
                dataGridView_装备数据编辑器.CurrentCell = dataGridView_装备数据编辑器.Rows[focusIndex].Cells[0];
            }
            else
            {
                // 否则设置为第一行的第一列（如果存在）
                dataGridView_装备数据编辑器.CurrentCell = dataGridView_装备数据编辑器.Rows[0].Cells[0];
            }
        }

        /// <summary>
        /// 删除选中的装备行，支持多选。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleltButton_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();

            // 获取通过行头选中的行
            foreach (DataGridViewRow row in dataGridView_装备数据编辑器.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    selectedRows.Add(row);
                }
            }

            // 获取通过单元格选中的行（但不在SelectedRows中的行）
            foreach (DataGridViewCell cell in dataGridView_装备数据编辑器.SelectedCells)
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
            int firstDisplayedIndex = dataGridView_装备数据编辑器.FirstDisplayedScrollingRowIndex;
            int lastDisplayedIndex = firstDisplayedIndex + dataGridView_装备数据编辑器.DisplayedRowCount(false) - 1;

            // 从后往前删除，避免索引问题
            List<int> selectedIndices = selectedRows
                .Select(r => r.Index)
                .OrderByDescending(i => i)
                .ToList();

            // 检查所有删除行是否都在显示范围内
            bool allInView = selectedIndices.All(index => index >= firstDisplayedIndex && index <= lastDisplayedIndex);

            // 找到最上面的删除行
            int minIndex = selectedIndices.Min();

            foreach (int index in selectedIndices)
            {
                _iEquipmentService.DeletEquipmentAtIndex(index);
            }
            isChanged = true;
            BindDataGridView();

            // 计算新的焦点行索引为删除的首行的上一行
            int focusIndex = selectedIndices[selectedIndices.Count - 1] - 1;

            // 确保焦点索引在有效范围内
            if (focusIndex >= dataGridView_装备数据编辑器.RowCount - 1)
            {
                focusIndex = dataGridView_装备数据编辑器.RowCount - 2;
            }

            // 当焦点行索引有效时，设置当前单元格为该行的第一列
            if (focusIndex >= 0 && dataGridView_装备数据编辑器.RowCount > 0)
            {
                dataGridView_装备数据编辑器.CurrentCell = dataGridView_装备数据编辑器.Rows[focusIndex].Cells[0];
            }
            else if (dataGridView_装备数据编辑器.RowCount > 0)
            {
                // 否则设置为第一行的第一列（如果存在）
                dataGridView_装备数据编辑器.CurrentCell = dataGridView_装备数据编辑器.Rows[0].Cells[0];
            }

            // 根据删除行位置决定是否滚动
            if (allInView)
            {
                // 所有删除行都在显示范围内，不滚动
                if (firstDisplayedIndex >= 0 && firstDisplayedIndex < dataGridView_装备数据编辑器.RowCount)
                {
                    dataGridView_装备数据编辑器.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
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
                if (scrollIndex < dataGridView_装备数据编辑器.RowCount)
                {
                    dataGridView_装备数据编辑器.FirstDisplayedScrollingRowIndex = scrollIndex;
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
                DialogResult result = MessageBox.Show("存在未保存的更改，是否保存？", "未保存的更改", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
            if (dataGridView_装备数据编辑器.CurrentCell == null)
            {
                MessageBox.Show("请先选择要移动的行！");
                return;
            }
            // 获取当前行索引
            int currentIndex = dataGridView_装备数据编辑器.CurrentCell.RowIndex;
            // 检查是否可以上移（不是第一行）
            if (currentIndex <= 0)
            {
                MessageBox.Show("已经是第一行，无法上移！");
                return;
            }
            // 记录滚动位置
            int firstDisplayedIndex = dataGridView_装备数据编辑器.FirstDisplayedScrollingRowIndex;
            // 交换数据源中的位置
            Equipment temp = _iEquipmentService.GetEquipmentDatas()[currentIndex];
            _iEquipmentService.GetEquipmentDatas()[currentIndex] = _iEquipmentService.GetEquipmentDatas()[currentIndex - 1];
            _iEquipmentService.GetEquipmentDatas()[currentIndex - 1] = temp;
            isChanged = true;
            BindDataGridView();

            // 重新选中移动后的行
            dataGridView_装备数据编辑器.ClearSelection();
            dataGridView_装备数据编辑器.Rows[currentIndex - 1].Selected = true;
            dataGridView_装备数据编辑器.CurrentCell = dataGridView_装备数据编辑器.Rows[currentIndex - 1].Cells[0];

            // 恢复滚动位置
            if (firstDisplayedIndex >= 0 && firstDisplayedIndex < dataGridView_装备数据编辑器.RowCount)
            {
                dataGridView_装备数据编辑器.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
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
            if (dataGridView_装备数据编辑器.CurrentCell == null)
            {
                MessageBox.Show("请先选择要移动的行！");
                return;
            }

            // 获取当前行索引
            int currentIndex = dataGridView_装备数据编辑器.CurrentCell.RowIndex;

            // 检查是否可以下移（不是最后一行）
            if (currentIndex >= _iEquipmentService.GetEquipmentCount() - 1)
            {
                MessageBox.Show("已经是最后一行，无法下移！");
                return;
            }

            // 记录滚动位置
            int firstDisplayedIndex = dataGridView_装备数据编辑器.FirstDisplayedScrollingRowIndex;

            // 交换数据源中的位置
            Equipment temp = _iEquipmentService.GetEquipmentDatas()[currentIndex];
            _iEquipmentService.GetEquipmentDatas()[currentIndex] = _iEquipmentService.GetEquipmentDatas()[currentIndex + 1];
            _iEquipmentService.GetEquipmentDatas()[currentIndex + 1] = temp;
            isChanged = true;
            BindDataGridView();

            // 重新选中移动后的行
            dataGridView_装备数据编辑器.ClearSelection();
            dataGridView_装备数据编辑器.Rows[currentIndex + 1].Selected = true;
            dataGridView_装备数据编辑器.CurrentCell = dataGridView_装备数据编辑器.Rows[currentIndex + 1].Cells[0];

            // 恢复滚动位置
            if (firstDisplayedIndex >= 0 && firstDisplayedIndex < dataGridView_装备数据编辑器.RowCount)
            {
                dataGridView_装备数据编辑器.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
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
                DialogResult result = MessageBox.Show("存在未保存的更改，是否保存？", "未保存的更改", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
            _iEquipmentService.SetFilePathsIndex(comboBox_赛季文件选择器.SelectedIndex);

            // 重新加载装备数据
            _iEquipmentService.ReLoad();

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
            dataGridView_装备数据编辑器.EndEdit();
            _iEquipmentService.Save();
        }

        /// <summary>
        /// 打开目录按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (_iEquipmentService.GetFilePaths().Length > 0 && _iEquipmentService.GetFilePathsIndex() < _iEquipmentService.GetFilePaths().Length)
            {
                string path = _iEquipmentService.GetFilePaths()[_iEquipmentService.GetFilePathsIndex()];
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

        #region 圆角实现
        // GDI32 API - 用于创建圆角效果
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        // 圆角半径
        private const int CORNER_RADIUS = 16;

        /// <summary>
        /// 在窗口句柄创建后应用圆角效果
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // 应用 GDI Region 圆角效果（支持 Windows 10 和 Windows 11）
            ApplyRoundedCorners();
        }

        /// <summary>
        /// 应用 GDI Region 圆角效果
        /// </summary>
        private void ApplyRoundedCorners()
        {
            try
            {
                // 创建圆角矩形区域
                IntPtr region = CreateRoundRectRgn(0, 0, Width, Height, CORNER_RADIUS, CORNER_RADIUS);

                if (region != IntPtr.Zero)
                {
                    SetWindowRgn(Handle, region, true);
                    // 注意：SetWindowRgn 会接管 region 的所有权，不需要手动删除

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 窗口大小改变时重新应用圆角
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 调整大小时重新创建圆角区域
            if (Handle != IntPtr.Zero)
            {
                ApplyRoundedCorners();
            }
        }
        #endregion
    }
}