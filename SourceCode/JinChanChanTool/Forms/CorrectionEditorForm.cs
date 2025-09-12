using System.Data;
using JinChanChanTool.Services.DataServices;
using JinChanChanTool.DataClass;

namespace JinChanChanTool
{
    /// <summary>
    /// OCR结果纠正列表编辑器
    /// </summary>
    public partial class CorrectionEditorForm : Form
    {
        /// <summary>
        /// OCR结果纠正服务对象
        /// </summary>
        private ICorrectionService _iCorrectionService;
        
        /// <summary>
        /// 是否发生改动的标志
        /// </summary>
        private bool isChanged;
       
        public CorrectionEditorForm()
        {
            InitializeComponent();
            // 添加自定义标题栏
            CustomTitleBar titleBar = new CustomTitleBar(this, null, "OCR结果纠正编辑器");
            this.Controls.Add(titleBar);
            //实例化OCR结果纠正列表服务对象
            _iCorrectionService = new CorrectionService();
            _iCorrectionService.Load();
            isChanged = false;
            InitializeDataGrid();
            LoadDataToDataGrid();
        }

        private void CorrectionEditorForm_Load(object sender, EventArgs e)
        {

        }       

        /// <summary>
        /// 在DataGridView中设置列并填充数据。
        /// </summary>
        private void InitializeDataGrid()
        {
            // 清除现有列
            dataGridView.Columns.Clear();
            // 添加纠正值列
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "纠正值",
                Name = "CorrectColumn",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.NotSortable // 禁用排序
            });
            // 添加原始值列
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "原始值",
                Name = "IncorrectColumn",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.NotSortable // 禁用排序
            });            
        }

        /// <summary>
        /// 填充数据到DataGrid
        /// </summary>
        private void LoadDataToDataGrid()
        {
            for (int i = 0; i < _iCorrectionService.ResultMappings.Count; i++)
            {
                for (int j = 0; j < _iCorrectionService.ResultMappings[i].Incorrect.Count; j++)
                {
                    int rowIndex = dataGridView.Rows.Add();
                    dataGridView.Rows[rowIndex].Cells["CorrectColumn"].Value = _iCorrectionService.ResultMappings[i].Correct;
                    dataGridView.Rows[rowIndex].Cells["IncorrectColumn"].Value = _iCorrectionService.ResultMappings[i].Incorrect[j];
                }
            }
        }

        /// <summary>
        /// 保存DataGridView中的映射关系到指定Json文件。
        /// </summary>
        private void SaveMappings()
        {           
            var groupedMappings = dataGridView.Rows
                    .Cast<DataGridViewRow>()
                    .Where(row => !row.IsNewRow)
                    .GroupBy(row => row.Cells["CorrectColumn"].Value?.ToString())
                    .Where(group => !string.IsNullOrEmpty(group.Key))
                    .Select(group => new ResultMapping
                    {
                        Correct = group.Key,
                        Incorrect = group.Select(row => row.Cells["IncorrectColumn"].Value?.ToString())
                                        .Where(value => !string.IsNullOrEmpty(value))
                                        .ToList()
                    })
                    .ToList();               
            _iCorrectionService.ResultMappings = groupedMappings;
            _iCorrectionService.Save();
        }

        /// <summary>
        /// 添加按钮触发事件，添加新行到DataGridView。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_Click(object sender, EventArgs e)
        {            
            // 添加新行
            dataGridView.Rows.Add();
            isChanged = true;
            // 滚动到最后一行
            dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.RowCount - 1;
            int focusIndex = dataGridView.RowCount - 2;
            //设置当前单元格为该行的第一列           
            dataGridView.CurrentCell = dataGridView.Rows[focusIndex].Cells[0];
        }

        /// <summary>
        /// 删除按钮触发事件，删除DataGridView中选中的行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, EventArgs e)
        {            
            var selectedRows = new List<DataGridViewRow>();
            // 获取通过行头选中的行
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    selectedRows.Add(row);
                }
            }
            // 获取通过单元格选中的行（但不在SelectedRows中的行）
            foreach (DataGridViewCell cell in dataGridView.SelectedCells)
            {
                if (!cell.OwningRow.IsNewRow && !selectedRows.Contains(cell.OwningRow))
                {
                    selectedRows.Add(cell.OwningRow);
                }
            }
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("请先选择要删除的行！","未选中删除行",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            // 记录删除前的滚动位置
            int firstDisplayedIndex = dataGridView.FirstDisplayedScrollingRowIndex;
            int lastDisplayedIndex = firstDisplayedIndex + dataGridView.DisplayedRowCount(false) - 1;
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
                dataGridView.Rows.RemoveAt(index);
            }
            isChanged = true;
            // 计算新的焦点行索引为删除行的上面一行
            int focusIndex = selectedIndices[selectedIndices.Count - 1] - 1;

            // 根据删除行位置决定是否滚动
            if (allInView)
            {
                // 所有删除行都在显示范围内，不滚动
                if (firstDisplayedIndex < dataGridView.RowCount)
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
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
                if (scrollIndex < dataGridView.RowCount)
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = scrollIndex;
                }
            }

            // 当焦点行索引有效时，设置当前单元格为该行的第一列
            if (focusIndex >= 0 && focusIndex < dataGridView.RowCount)
            {
                dataGridView.CurrentCell = dataGridView.Rows[focusIndex].Cells[0];
            }
            else
            {
                // 否则设置为第一行的第一列（如果存在）
                dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
            }
        }

        /// <summary>
        /// 退出按钮触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (isChanged)
            {
                var result = MessageBox.Show("存在未保存的更改，是否保存？", "未保存的更改", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveMappings();
                    isChanged = false;
                }
                else if(result == DialogResult.Cancel)
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
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            SaveMappings();
            isChanged = false;
            MessageBox.Show("保存成功！重启应用后生效。","保存成功",MessageBoxButtons.OK,MessageBoxIcon.Information);            
        }
    }
}