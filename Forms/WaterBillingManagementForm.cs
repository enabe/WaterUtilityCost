using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 水道料金請求データ管理フォーム
    /// </summary>
    public partial class WaterBillingManagementForm : Form
    {
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class WaterBillingGridRow
        {
            public int Id { get; set; }
            public string 受領請求年月 { get; set; } = string.Empty;
            public string 親メーター { get; set; } = string.Empty;
            public string ビル名 { get; set; } = string.Empty;
            public decimal 使用量 { get; set; }
            public DateTime 使用期間開始 { get; set; }
            public DateTime 使用期間終了 { get; set; }
            public decimal 基本料金 { get; set; }
            public decimal 使用料金 { get; set; }
            public decimal 合計金額 { get; set; }
            public decimal 税率 { get; set; }
            public string お客様番号 { get; set; } = string.Empty;
        }

        public WaterBillingManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += WaterBillingManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvWaterBillings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvWaterBillings.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnCopyAndAdd.Click += BtnCopyAndAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _dgvWaterBillings.DoubleClick += DgvWaterBillings_DoubleClick;
            _dgvWaterBillings.DataBindingComplete += DgvWaterBillings_DataBindingComplete;
            _dgvWaterBillings.ColumnHeaderMouseClick += DgvWaterBillings_ColumnHeaderMouseClick;
        }

        private async void WaterBillingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadWaterBillingsAsync();
        }

        private async Task LoadWaterBillingsAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var waterBillings = await WaterBillingDataAccess.GetAllWaterBillingsAsync();
                var meters = await MeterDataAccess.GetAllMetersAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var buildingNameById = buildings.ToDictionary(b => b.Id, b => b.Name);

                string GetParentMeterDisplay(int? parentMeterId)
                {
                    if (!parentMeterId.HasValue)
                    {
                        return string.Empty;
                    }

                    var meter = meters.FirstOrDefault(m => m.Id == parentMeterId.Value);
                    if (meter == null)
                    {
                        return string.Empty;
                    }

                    var meterName = string.IsNullOrWhiteSpace(meter.MeterName) ? $"ID:{meter.Id}" : meter.MeterName;
                    var buildingName = meter.BuildingId.HasValue && buildingNameById.TryGetValue(meter.BuildingId.Value, out var name)
                        ? name
                        : string.Empty;
                    return string.IsNullOrWhiteSpace(buildingName) ? meterName : $"{meterName} ({buildingName})";
                }

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvWaterBillings.SuspendLayout();
                    _dgvWaterBillings.Visible = false;
                }

                var rows = waterBillings.Select(wb => new WaterBillingGridRow
                {
                    Id = wb.Id,
                    受領請求年月 = wb.BillingYearMonth ?? "",
                    親メーター = GetParentMeterDisplay(wb.ParentMeterId),
                    ビル名 = wb.BuildingName ?? "",
                    使用量 = wb.UsageAmount,
                    使用期間開始 = wb.StartDate,
                    使用期間終了 = wb.EndDate,
                    基本料金 = wb.BasicCharge,
                    使用料金 = wb.UsageCharge,
                    合計金額 = wb.BasicCharge + wb.UsageCharge,
                    税率 = wb.TaxRate,
                    お客様番号 = wb.CustomerNumber ?? ""
                });

                _dgvWaterBillings.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

                // ID列の幅を狭く設定
                if (_dgvWaterBillings.Columns["Id"] != null)
                {
                    _dgvWaterBillings.Columns["Id"].Width = 40;
                }
                if (_dgvWaterBillings.Columns["税率"] != null)
                {
                    _dgvWaterBillings.Columns["税率"].FillWeight = 50;
                }

                if (_dgvWaterBillings.Columns["使用期間開始"] != null)
                {
                    _dgvWaterBillings.Columns["使用期間開始"].DefaultCellStyle.Format = "yyyy-MM-dd";
                }

                if (_dgvWaterBillings.Columns["使用期間終了"] != null)
                {
                    _dgvWaterBillings.Columns["使用期間終了"].DefaultCellStyle.Format = "yyyy-MM-dd";
                }

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvWaterBillings.Visible = true;
                    _dgvWaterBillings.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvWaterBillings.Visible = true;
                    _dgvWaterBillings.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"水道料金請求データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<WaterBillingGridRow> ApplySort(IEnumerable<WaterBillingGridRow> source)
        {
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => source.OrderBy(x => x.Id),
                ("Id", false) => source.OrderByDescending(x => x.Id),
                ("受領請求年月", true) => source.OrderBy(x => x.受領請求年月),
                ("受領請求年月", false) => source.OrderByDescending(x => x.受領請求年月),
                ("親メーター", true) => source.OrderBy(x => x.親メーター),
                ("親メーター", false) => source.OrderByDescending(x => x.親メーター),
                ("ビル名", true) => source.OrderBy(x => x.ビル名),
                ("ビル名", false) => source.OrderByDescending(x => x.ビル名),
                ("使用量", true) => source.OrderBy(x => x.使用量),
                ("使用量", false) => source.OrderByDescending(x => x.使用量),
                ("使用期間開始", true) => source.OrderBy(x => x.使用期間開始),
                ("使用期間開始", false) => source.OrderByDescending(x => x.使用期間開始),
                ("使用期間終了", true) => source.OrderBy(x => x.使用期間終了),
                ("使用期間終了", false) => source.OrderByDescending(x => x.使用期間終了),
                ("基本料金", true) => source.OrderBy(x => x.基本料金),
                ("基本料金", false) => source.OrderByDescending(x => x.基本料金),
                ("使用料金", true) => source.OrderBy(x => x.使用料金),
                ("使用料金", false) => source.OrderByDescending(x => x.使用料金),
                ("合計金額", true) => source.OrderBy(x => x.合計金額),
                ("合計金額", false) => source.OrderByDescending(x => x.合計金額),
                ("税率", true) => source.OrderBy(x => x.税率),
                ("税率", false) => source.OrderByDescending(x => x.税率),
                ("お客様番号", true) => source.OrderBy(x => x.お客様番号),
                ("お客様番号", false) => source.OrderByDescending(x => x.お客様番号),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvWaterBillings.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvWaterBillings.Columns.Contains(_sortColumn))
            {
                _dgvWaterBillings.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvWaterBillings_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvWaterBillings.Columns[e.ColumnIndex].Name;
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
            if (_dgvWaterBillings.SelectedRows.Count > 0 &&
                _dgvWaterBillings.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvWaterBillings.Rows.Count > 0)
            {
                currentScrollIndex = _dgvWaterBillings.FirstDisplayedScrollingRowIndex;
            }

            await LoadWaterBillingsAsync(selectedId, currentScrollIndex);
        }

        private void DgvWaterBillings_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvWaterBillings.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvWaterBillings.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvWaterBillings.Visible = true;
                        _dgvWaterBillings.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvWaterBillings.ClearSelection();
                targetRow.Selected = true;
                _dgvWaterBillings.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvWaterBillings.Rows.Count)
                {
                    _dgvWaterBillings.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvWaterBillings.Visible = true;
                    _dgvWaterBillings.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new WaterBillingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadWaterBillingsAsync();
            }
        }

        private async void BtnCopyAndAdd_Click(object? sender, EventArgs e)
        {
            if (_dgvWaterBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする水道料金請求データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvWaterBillings.SelectedRows[0];
            var waterBillingId = (int)selectedRow.Cells["Id"].Value;
            var waterBilling = await WaterBillingDataAccess.GetWaterBillingByIdAsync(waterBillingId);
            if (waterBilling != null)
            {
                var form = new WaterBillingForm(waterBilling, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadWaterBillingsAsync();
                }
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvWaterBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する水道料金請求データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvWaterBillings.SelectedRows[0];
            var waterBillingId = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvWaterBillings.FirstDisplayedScrollingRowIndex;

            var waterBilling = await WaterBillingDataAccess.GetWaterBillingByIdAsync(waterBillingId);
            if (waterBilling != null)
            {
                var form = new WaterBillingForm(waterBilling);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadWaterBillingsAsync(waterBillingId, currentScrollIndex);
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvWaterBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する水道料金請求データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvWaterBillings.SelectedRows[0];
            var buildingName = selectedRow.Cells["ビル名"].Value?.ToString() ?? "";
            var billingYearMonth = selectedRow.Cells["受領請求年月"].Value?.ToString() ?? "";
            var result = MessageBox.Show($"水道料金請求データ「{buildingName} - {billingYearMonth}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var waterBillingId = (int)selectedRow.Cells["Id"].Value;
                    await WaterBillingDataAccess.DeleteWaterBillingAsync(waterBillingId);
                    await LoadWaterBillingsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadWaterBillingsAsync();
        }

        private async void DgvWaterBillings_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvWaterBillings.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvWaterBillings.SelectedRows[0];
            var waterBillingId = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvWaterBillings.FirstDisplayedScrollingRowIndex;

            var waterBilling = await WaterBillingDataAccess.GetWaterBillingByIdAsync(waterBillingId);
            if (waterBilling != null)
            {
                var form = new WaterBillingForm(waterBilling);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadWaterBillingsAsync(waterBillingId, currentScrollIndex);
                }
            }
        }
    }
}


