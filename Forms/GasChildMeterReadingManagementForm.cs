using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// ガス子メーター検針データ管理フォーム
    /// </summary>
    public partial class GasChildMeterReadingManagementForm : Form
    {
        private const string METER_TYPE = "ガス";
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        public GasChildMeterReadingManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += GasChildMeterReadingManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvGasChildMeterReadings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvGasChildMeterReadings.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _btnCopyAndAdd.Click += BtnCopyAndAdd_Click;
            _btnImportGasCsv.Click += BtnImportGasCsv_Click;
            _btnExportComparisonCsv.Click += BtnExportComparisonCsv_Click;
            _btnDeleteByYearMonth.Click += BtnDeleteByYearMonth_Click;
            _dgvGasChildMeterReadings.DoubleClick += DgvGasChildMeterReadings_DoubleClick;
            _dgvGasChildMeterReadings.DataBindingComplete += DgvGasChildMeterReadings_DataBindingComplete;
            _dgvGasChildMeterReadings.ColumnHeaderMouseClick += DgvGasChildMeterReadings_ColumnHeaderMouseClick;
        }

        private async void GasChildMeterReadingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadGasChildMeterReadingsAsync();
        }

        private async Task LoadGasChildMeterReadingsAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var readings = await ChildMeterReadingDataAccess.GetAllChildMeterReadingsAsync();
                
                // ガスのデータのみをフィルタリング
                var gasReadings = readings.Where(r => r.Type == METER_TYPE).ToList();

                // 関連データを取得して表示用に整形
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();

                gasReadings = SortReadingsForDisplay(gasReadings, buildings, childMeters);

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvGasChildMeterReadings.SuspendLayout();
                    _dgvGasChildMeterReadings.Visible = false;
                }

                _dgvGasChildMeterReadings!.DataSource = BuildGasReadingsDataTable(
                    gasReadings,
                    buildings,
                    childMeters);

                // ID列の幅を狭く設定
                if (_dgvGasChildMeterReadings.Columns["Id"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["Id"].Width = 40;
                }

                // ビル名列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["ビル名"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["ビル名"].Width = 150;
                }

                // 子メーター名列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["子メーター名"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["子メーター名"].Width = 200;
                }

                // 検針日列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["検針日"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["検針日"].Width = 120;
                }

                // メーター値列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["メーター値"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["メーター値"].Width = 120;
                }

                foreach (DataGridViewColumn column in _dgvGasChildMeterReadings.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                }

                UpdateSortGlyph();

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvGasChildMeterReadings.Visible = true;
                    _dgvGasChildMeterReadings.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvGasChildMeterReadings.Visible = true;
                    _dgvGasChildMeterReadings.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DgvGasChildMeterReadings_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvGasChildMeterReadings.Columns[e.ColumnIndex].Name;
            if (_sortColumn == clickedColumnName)
            {
                _sortAscending = !_sortAscending;
            }
            else
            {
                _sortColumn = clickedColumnName;
                _sortAscending = true;
            }

            int? selectedId = null;
            int? currentScrollIndex = null;
            if (_dgvGasChildMeterReadings.SelectedRows.Count > 0
                && _dgvGasChildMeterReadings.SelectedRows[0].Cells["Id"].Value != null
                && _dgvGasChildMeterReadings.SelectedRows[0].Cells["Id"].Value != DBNull.Value)
            {
                selectedId = Convert.ToInt32(_dgvGasChildMeterReadings.SelectedRows[0].Cells["Id"].Value);
            }

            if (_dgvGasChildMeterReadings.Rows.Count > 0)
            {
                currentScrollIndex = _dgvGasChildMeterReadings.FirstDisplayedScrollingRowIndex;
            }

            await LoadGasChildMeterReadingsAsync(selectedId, currentScrollIndex);
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvGasChildMeterReadings.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvGasChildMeterReadings.Columns.Contains(_sortColumn))
            {
                _dgvGasChildMeterReadings.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private List<ChildMeterReading> SortReadingsForDisplay(
            List<ChildMeterReading> source,
            List<Building> buildings,
            List<ChildMeter> childMeters)
        {
            string ResolveBuildingName(ChildMeterReading r)
            {
                var childMeter = r.ChildMeterId.HasValue
                    ? childMeters.FirstOrDefault(m => m.Id == r.ChildMeterId.Value)
                    : null;
                var building = childMeter?.BuildingId != null
                    ? buildings.FirstOrDefault(b => b.Id == childMeter.BuildingId.Value)
                    : null;
                return building?.Name ?? "";
            }

            string ResolveChildMeterName(ChildMeterReading r)
            {
                var childMeter = r.ChildMeterId.HasValue
                    ? childMeters.FirstOrDefault(m => m.Id == r.ChildMeterId.Value)
                    : null;
                return childMeter?.MeterName ?? "";
            }

            IEnumerable<ChildMeterReading> q = source;
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => q.OrderBy(r => r.Id).ToList(),
                ("Id", false) => q.OrderByDescending(r => r.Id).ToList(),
                ("ビル名", true) => q.OrderBy(r => ResolveBuildingName(r)).ToList(),
                ("ビル名", false) => q.OrderByDescending(r => ResolveBuildingName(r)).ToList(),
                ("子メーター名", true) => q.OrderBy(r => ResolveChildMeterName(r)).ToList(),
                ("子メーター名", false) => q.OrderByDescending(r => ResolveChildMeterName(r)).ToList(),
                ("検針日", true) => q.OrderBy(r => r.ReadingDate).ToList(),
                ("検針日", false) => q.OrderByDescending(r => r.ReadingDate).ToList(),
                ("メーター値", true) => q.OrderBy(r => r.MeterValue).ToList(),
                ("メーター値", false) => q.OrderByDescending(r => r.MeterValue).ToList(),
                _ => q.OrderByDescending(r => r.Id).ToList()
            };
        }

        private void DgvGasChildMeterReadings_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!_pendingSelectId.HasValue)
            {
                return;
            }

            var selectId = _pendingSelectId.Value;
            _pendingSelectId = null;
            var scrollIndex = _pendingScrollIndex;
            _pendingScrollIndex = null;
            ApplySelectionAndScroll(selectId, scrollIndex);
        }

        private void ApplySelectionAndScroll(int selectId, int? scrollIndex)
        {
            _dgvGasChildMeterReadings.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvGasChildMeterReadings.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row =>
                        row.Cells["Id"].Value != null
                        && row.Cells["Id"].Value != DBNull.Value
                        && Convert.ToInt32(row.Cells["Id"].Value) == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvGasChildMeterReadings.Visible = true;
                        _dgvGasChildMeterReadings.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvGasChildMeterReadings.ClearSelection();
                targetRow.Selected = true;
                _dgvGasChildMeterReadings.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvGasChildMeterReadings.Rows.Count)
                {
                    _dgvGasChildMeterReadings.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvGasChildMeterReadings.Visible = true;
                    _dgvGasChildMeterReadings.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new GasChildMeterReadingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadGasChildMeterReadingsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvGasChildMeterReadings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する検針データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvGasChildMeterReadings.SelectedRows[0];
            var id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var currentScrollIndex = _dgvGasChildMeterReadings.FirstDisplayedScrollingRowIndex;
            var reading = await ChildMeterReadingDataAccess.GetChildMeterReadingByIdAsync(id);
            if (reading != null)
            {
                var form = new GasChildMeterReadingForm(reading);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadGasChildMeterReadingsAsync(id, currentScrollIndex);
                }
            }
        }

        private async void BtnDeleteByYearMonth_Click(object? sender, EventArgs e)
        {
            using var dialog = new ReadingYearMonthSelectDialog();
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var year = dialog.SelectedYear;
            var month = dialog.SelectedMonth;

            try
            {
                var count = await ChildMeterReadingDataAccess.CountGasChildMeterReadingsByReadingYearMonthAsync(year, month);
                if (count == 0)
                {
                    MessageBox.Show(
                        $"{year}年{month:00}月のガス子メーター検針データはありません。",
                        "対象年月削除",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                var confirm = MessageBox.Show(
                    $"{year}年{month:00}月のガス子メーター検針データ {count} 件を削除します。{Environment.NewLine}この操作は取り消せません。よろしいですか？",
                    "確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                var deleted = await ChildMeterReadingDataAccess.DeleteGasChildMeterReadingsByReadingYearMonthAsync(year, month);
                await LoadGasChildMeterReadingsAsync();
                MessageBox.Show(
                    $"{deleted} 件を削除しました。",
                    "対象年月削除",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvGasChildMeterReadings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する検針データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvGasChildMeterReadings.SelectedRows[0];
            var id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var buildingName = selectedRow.Cells["ビル名"].Value?.ToString() ?? "";
            var childMeterName = selectedRow.Cells["子メーター名"].Value?.ToString() ?? "";
            var readingDate = selectedRow.Cells["検針日"].Value?.ToString() ?? "";
            
            var result = MessageBox.Show($"検針データ（ID: {id}、ビル: {buildingName}、子メーター: {childMeterName}、検針日: {readingDate}）を削除しますか？", 
                "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    await ChildMeterReadingDataAccess.DeleteChildMeterReadingAsync(id);
                    await LoadGasChildMeterReadingsAsync();
                    MessageBox.Show("検針データを削除しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadGasChildMeterReadingsAsync();
        }

        private async void DgvGasChildMeterReadings_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvGasChildMeterReadings!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvGasChildMeterReadings.SelectedRows[0];
            var id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var currentScrollIndex = _dgvGasChildMeterReadings.FirstDisplayedScrollingRowIndex;
            var reading = await ChildMeterReadingDataAccess.GetChildMeterReadingByIdAsync(id);
            if (reading != null)
            {
                var form = new GasChildMeterReadingForm(reading);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadGasChildMeterReadingsAsync(id, currentScrollIndex);
                }
            }
        }

        private async void BtnCopyAndAdd_Click(object? sender, EventArgs e)
        {
            if (_dgvGasChildMeterReadings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする検針データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvGasChildMeterReadings.SelectedRows[0];
            var id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var reading = await ChildMeterReadingDataAccess.GetChildMeterReadingByIdAsync(id);
            if (reading != null)
            {
                var form = new GasChildMeterReadingForm(reading, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadGasChildMeterReadingsAsync();
                }
            }
        }

        private static DataTable BuildGasReadingsDataTable(
            List<ChildMeterReading> gasReadings,
            List<Building> buildings,
            List<ChildMeter> childMeters)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("ビル名", typeof(string));
            table.Columns.Add("子メーター名", typeof(string));
            table.Columns.Add("検針日", typeof(string));
            table.Columns.Add("メーター値", typeof(decimal));

            foreach (var r in gasReadings)
            {
                var childMeter = r.ChildMeterId.HasValue
                    ? childMeters.FirstOrDefault(m => m.Id == r.ChildMeterId.Value)
                    : null;
                var building = childMeter?.BuildingId != null
                    ? buildings.FirstOrDefault(b => b.Id == childMeter.BuildingId.Value)
                    : null;
                table.Rows.Add(
                    r.Id,
                    building?.Name ?? "",
                    childMeter?.MeterName ?? "",
                    r.ReadingDate.ToString("yyyy-MM-dd"),
                    r.MeterValue);
            }

            return table;
        }
    }
}

