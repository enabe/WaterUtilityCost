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
    /// 電気料金請求データ管理フォーム
    /// </summary>
    public partial class ElectricBillingManagementForm : Form
    {
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class ElectricBillingGridRow
        {
            public int Id { get; set; }
            public string 受領請求年月 { get; set; } = string.Empty;
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

        public ElectricBillingManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += ElectricBillingManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvElectricBillings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvElectricBillings.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnCopyAndAdd.Click += BtnCopyAndAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _dgvElectricBillings.DoubleClick += DgvElectricBillings_DoubleClick;
            _dgvElectricBillings.DataBindingComplete += DgvElectricBillings_DataBindingComplete;
            _dgvElectricBillings.ColumnHeaderMouseClick += DgvElectricBillings_ColumnHeaderMouseClick;
        }

        private async void ElectricBillingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadElectricBillingsAsync();
        }

        private async Task LoadElectricBillingsAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var electricBillings = await ElectricBillingDataAccess.GetAllElectricBillingsAsync();
                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvElectricBillings.SuspendLayout();
                    _dgvElectricBillings.Visible = false;
                }

                var rows = electricBillings.Select(eb => new ElectricBillingGridRow
                {
                    Id = eb.Id,
                    受領請求年月 = eb.BillingYearMonth ?? "",
                    ビル名 = eb.BuildingName ?? "",
                    使用量 = eb.UsageAmount,
                    使用期間開始 = eb.StartDate,
                    使用期間終了 = eb.EndDate,
                    基本料金 = eb.BasicCharge,
                    使用料金 = eb.PowerCharge,
                    合計金額 = eb.BasicCharge + eb.PowerCharge,
                    税率 = eb.TaxRate,
                    お客様番号 = eb.CustomerNumber ?? ""
                });

                _dgvElectricBillings.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

                // 列幅の設定
                if (_dgvElectricBillings.Columns["Id"] != null)
                {
                    _dgvElectricBillings.Columns["Id"].Width = 40;
                }
                if (_dgvElectricBillings.Columns["受領請求年月"] != null)
                {
                    _dgvElectricBillings.Columns["受領請求年月"].Width = 100;
                }
                if (_dgvElectricBillings.Columns["ビル名"] != null)
                {
                    _dgvElectricBillings.Columns["ビル名"].Width = 150;
                }
                if (_dgvElectricBillings.Columns["税率"] != null)
                {
                    _dgvElectricBillings.Columns["税率"].FillWeight = 50;
                }

                if (_dgvElectricBillings.Columns["使用期間開始"] != null)
                {
                    _dgvElectricBillings.Columns["使用期間開始"].DefaultCellStyle.Format = "yyyy-MM-dd";
                }

                if (_dgvElectricBillings.Columns["使用期間終了"] != null)
                {
                    _dgvElectricBillings.Columns["使用期間終了"].DefaultCellStyle.Format = "yyyy-MM-dd";
                }

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvElectricBillings.Visible = true;
                    _dgvElectricBillings.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvElectricBillings.Visible = true;
                    _dgvElectricBillings.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"電気料金請求データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<ElectricBillingGridRow> ApplySort(IEnumerable<ElectricBillingGridRow> source)
        {
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => source.OrderBy(x => x.Id),
                ("Id", false) => source.OrderByDescending(x => x.Id),
                ("受領請求年月", true) => source.OrderBy(x => x.受領請求年月),
                ("受領請求年月", false) => source.OrderByDescending(x => x.受領請求年月),
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
            foreach (DataGridViewColumn column in _dgvElectricBillings.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvElectricBillings.Columns.Contains(_sortColumn))
            {
                _dgvElectricBillings.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvElectricBillings_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvElectricBillings.Columns[e.ColumnIndex].Name;
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
            if (_dgvElectricBillings.SelectedRows.Count > 0 &&
                _dgvElectricBillings.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvElectricBillings.Rows.Count > 0)
            {
                currentScrollIndex = _dgvElectricBillings.FirstDisplayedScrollingRowIndex;
            }

            await LoadElectricBillingsAsync(selectedId, currentScrollIndex);
        }

        private void DgvElectricBillings_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvElectricBillings.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvElectricBillings.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvElectricBillings.Visible = true;
                        _dgvElectricBillings.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvElectricBillings.ClearSelection();
                targetRow.Selected = true;
                _dgvElectricBillings.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvElectricBillings.Rows.Count)
                {
                    _dgvElectricBillings.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvElectricBillings.Visible = true;
                    _dgvElectricBillings.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new ElectricBillingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadElectricBillingsAsync();
            }
        }

        private async void BtnCopyAndAdd_Click(object? sender, EventArgs e)
        {
            if (_dgvElectricBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする電気料金請求データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvElectricBillings.SelectedRows[0];
            var electricBillingId = (int)selectedRow.Cells["Id"].Value;
            var electricBilling = await ElectricBillingDataAccess.GetElectricBillingByIdAsync(electricBillingId);
            if (electricBilling != null)
            {
                var form = new ElectricBillingForm(electricBilling, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadElectricBillingsAsync();
                }
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvElectricBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する電気料金請求データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvElectricBillings.SelectedRows[0];
            var electricBillingId = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvElectricBillings.FirstDisplayedScrollingRowIndex;

            var electricBilling = await ElectricBillingDataAccess.GetElectricBillingByIdAsync(electricBillingId);
            if (electricBilling != null)
            {
                var form = new ElectricBillingForm(electricBilling);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadElectricBillingsAsync(electricBillingId, currentScrollIndex);
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvElectricBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する電気料金請求データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvElectricBillings.SelectedRows[0];
            var buildingName = selectedRow.Cells["ビル名"].Value?.ToString() ?? "";
            var billingYearMonth = selectedRow.Cells["受領請求年月"].Value?.ToString() ?? "";
            var result = MessageBox.Show($"電気料金請求データ「{buildingName} - {billingYearMonth}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var electricBillingId = (int)selectedRow.Cells["Id"].Value;
                    await ElectricBillingDataAccess.DeleteElectricBillingAsync(electricBillingId);
                    await LoadElectricBillingsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadElectricBillingsAsync();
        }

        private async void DgvElectricBillings_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvElectricBillings.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvElectricBillings.SelectedRows[0];
            var electricBillingId = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvElectricBillings.FirstDisplayedScrollingRowIndex;

            var electricBilling = await ElectricBillingDataAccess.GetElectricBillingByIdAsync(electricBillingId);
            if (electricBilling != null)
            {
                var form = new ElectricBillingForm(electricBilling);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadElectricBillingsAsync(electricBillingId, currentScrollIndex);
                }
            }
        }
    }
}


