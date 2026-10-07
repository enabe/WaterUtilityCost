using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// その他請求明細一覧表示フォーム
    /// </summary>
    public partial class OtherInvoiceDetailForm : Form
    {
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class OtherInvoiceDetailGridRow
        {
            public int Id { get; set; }
            public string 請求先 { get; set; } = string.Empty;
            public string 貸主 { get; set; } = string.Empty;
            public string 建物名称 { get; set; } = string.Empty;
            public string 借主 { get; set; } = string.Empty;
            public string 部屋番号 { get; set; } = string.Empty;
            public string 種別 { get; set; } = string.Empty;
            public string 内容 { get; set; } = string.Empty;
            public decimal 税込金額 { get; set; }
            public decimal 税率 { get; set; }
            public string 業者 { get; set; } = string.Empty;
            public string インボイス番号 { get; set; } = string.Empty;
            public string 請求年月 { get; set; } = string.Empty;
            public DateTime? 請求予定日 { get; set; }
        }

        public OtherInvoiceDetailForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += OtherInvoiceDetailForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // 請求年月を現在の年月に設定
            var now = DateTime.Now;
            _dtpBillingYearMonth.Value = new DateTime(now.Year, now.Month, 1);
            
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvOtherInvoiceDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvOtherInvoiceDetails.ColumnHeadersHeight = 30;
            _dgvOtherInvoiceDetails.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            // 種別ComboBoxの初期化
            _cmbSearchCategory.Items.Add(""); // 空の選択肢（すべて）
            // その他請求明細なので、種別は動的に取得する必要があるが、とりあえず空で開始
            _cmbSearchCategory.SelectedIndex = 0; // 空を選択
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnCopy.Click += BtnCopy_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _btnSearch.Click += BtnSearch_Click;
            _btnClearSearch.Click += BtnClearSearch_Click;
            _dtpBillingYearMonth.ValueChanged += DtpBillingYearMonth_ValueChanged;
            _dgvOtherInvoiceDetails.DataBindingComplete += DgvOtherInvoiceDetails_DataBindingComplete;
            _dgvOtherInvoiceDetails.ColumnHeaderMouseClick += DgvOtherInvoiceDetails_ColumnHeaderMouseClick;
        }

        private async void OtherInvoiceDetailForm_Load(object? sender, EventArgs e)
        {
            await LoadOtherInvoiceDetailsAsync();
        }

        private async void DtpBillingYearMonth_ValueChanged(object? sender, EventArgs e)
        {
            await LoadOtherInvoiceDetailsAsync();
        }

        private async Task LoadOtherInvoiceDetailsAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var invoiceDetails = await InvoiceDetailDataAccess.GetOtherInvoiceDetailsAsync();

                // 請求年月でフィルタリング
                var billingYearMonthKey = InvoiceDetailDataAccess.FormatBillingYearMonth(
                    _dtpBillingYearMonth.Value.Year,
                    _dtpBillingYearMonth.Value.Month);
                invoiceDetails = invoiceDetails
                    .Where(i => string.Equals(i.BillingYearMonth, billingYearMonthKey, StringComparison.Ordinal))
                    .ToList();

                // 種別ComboBoxに動的に種別を追加（初回のみ）
                if (_cmbSearchCategory.Items.Count == 1) // 空の選択肢のみの場合
                {
                    var allInvoiceDetails = await InvoiceDetailDataAccess.GetOtherInvoiceDetailsAsync();
                    var categories = allInvoiceDetails
                        .Where(i => !string.IsNullOrWhiteSpace(i.Category))
                        .Select(i => i.Category)
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList();
                    
                    foreach (var category in categories)
                    {
                        _cmbSearchCategory.Items.Add(category);
                    }
                }

                // 検索条件でフィルタリング
                var filteredDetails = invoiceDetails.AsEnumerable();

                // 請求先でフィルタ
                if (!string.IsNullOrWhiteSpace(_txtSearchBillingTo.Text))
                {
                    var searchBillingTo = _txtSearchBillingTo.Text.ToLower();
                    filteredDetails = filteredDetails.Where(i =>
                        i.BillingTo != null && i.BillingTo.ToLower().Contains(searchBillingTo));
                }

                // 建物名でフィルタ
                if (!string.IsNullOrWhiteSpace(_txtSearchBuildingName.Text))
                {
                    var searchBuildingName = _txtSearchBuildingName.Text.ToLower();
                    filteredDetails = filteredDetails.Where(i =>
                        i.BuildingName != null && i.BuildingName.ToLower().Contains(searchBuildingName));
                }

                // 種別でフィルタ（検索条件として）
                if (_cmbSearchCategory.SelectedIndex > 0 && _cmbSearchCategory.SelectedItem != null)
                {
                    var selectedCategory = _cmbSearchCategory.SelectedItem.ToString();
                    filteredDetails = filteredDetails.Where(i =>
                        i.Category != null && i.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase));
                }

                var filteredList = filteredDetails.ToList();

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvOtherInvoiceDetails.SuspendLayout();
                    _dgvOtherInvoiceDetails.Visible = false;
                }

                var rows = filteredList.Select(i => new OtherInvoiceDetailGridRow
                {
                    Id = i.Id,
                    請求先 = i.BillingTo ?? "",
                    貸主 = i.Lessor ?? "",
                    建物名称 = i.BuildingName ?? "",
                    借主 = i.Lessee ?? "",
                    部屋番号 = i.RoomNumber ?? "",
                    種別 = i.Category ?? "",
                    内容 = i.Content ?? "",
                    税込金額 = i.TaxInclusiveAmount,
                    税率 = i.TaxRate,
                    業者 = i.Contractor ?? "",
                    インボイス番号 = i.InvoiceNumber ?? "",
                    請求年月 = i.BillingYearMonth ?? "",
                    請求予定日 = i.ConfirmedBillingDate
                });

                _dgvOtherInvoiceDetails!.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

                // ID列の幅を狭く設定
                if (_dgvOtherInvoiceDetails.Columns["Id"] != null)
                {
                    _dgvOtherInvoiceDetails.Columns["Id"].Width = 40;
                }

                if (_dgvOtherInvoiceDetails.Columns["請求予定日"] != null)
                {
                    _dgvOtherInvoiceDetails.Columns["請求予定日"].DefaultCellStyle.Format = "yyyy/MM/dd";
                }

                // 税込金額列を右詰め（ヘッダーは一覧共通で中央）
                if (_dgvOtherInvoiceDetails.Columns["税込金額"] != null)
                {
                    var taxCol = _dgvOtherInvoiceDetails.Columns["税込金額"];
                    taxCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    taxCol.DefaultCellStyle.Format = "N0";
                }

                foreach (DataGridViewColumn column in _dgvOtherInvoiceDetails.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // 税込金額の合計を計算して表示
                var totalAmount = filteredList.Sum(i => i.TaxInclusiveAmount);
                _lblTotalAmount.Text = $"合計金額: ¥{totalAmount:N0}";

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvOtherInvoiceDetails.Visible = true;
                    _dgvOtherInvoiceDetails.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvOtherInvoiceDetails.Visible = true;
                    _dgvOtherInvoiceDetails.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<OtherInvoiceDetailGridRow> ApplySort(IEnumerable<OtherInvoiceDetailGridRow> source)
        {
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => source.OrderBy(x => x.Id),
                ("Id", false) => source.OrderByDescending(x => x.Id),
                ("請求先", true) => source.OrderBy(x => x.請求先),
                ("請求先", false) => source.OrderByDescending(x => x.請求先),
                ("貸主", true) => source.OrderBy(x => x.貸主),
                ("貸主", false) => source.OrderByDescending(x => x.貸主),
                ("建物名称", true) => source.OrderBy(x => x.建物名称),
                ("建物名称", false) => source.OrderByDescending(x => x.建物名称),
                ("借主", true) => source.OrderBy(x => x.借主),
                ("借主", false) => source.OrderByDescending(x => x.借主),
                ("部屋番号", true) => source.OrderBy(x => x.部屋番号),
                ("部屋番号", false) => source.OrderByDescending(x => x.部屋番号),
                ("種別", true) => source.OrderBy(x => x.種別),
                ("種別", false) => source.OrderByDescending(x => x.種別),
                ("内容", true) => source.OrderBy(x => x.内容),
                ("内容", false) => source.OrderByDescending(x => x.内容),
                ("税込金額", true) => source.OrderBy(x => x.税込金額),
                ("税込金額", false) => source.OrderByDescending(x => x.税込金額),
                ("税率", true) => source.OrderBy(x => x.税率),
                ("税率", false) => source.OrderByDescending(x => x.税率),
                ("業者", true) => source.OrderBy(x => x.業者),
                ("業者", false) => source.OrderByDescending(x => x.業者),
                ("インボイス番号", true) => source.OrderBy(x => x.インボイス番号),
                ("インボイス番号", false) => source.OrderByDescending(x => x.インボイス番号),
                ("請求年月", true) => source.OrderBy(x => x.請求年月),
                ("請求年月", false) => source.OrderByDescending(x => x.請求年月),
                ("請求予定日", true) => source.OrderBy(x => x.請求予定日),
                ("請求予定日", false) => source.OrderByDescending(x => x.請求予定日),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvOtherInvoiceDetails.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvOtherInvoiceDetails.Columns.Contains(_sortColumn))
            {
                _dgvOtherInvoiceDetails.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvOtherInvoiceDetails_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvOtherInvoiceDetails.Columns[e.ColumnIndex].Name;
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
            if (_dgvOtherInvoiceDetails.SelectedRows.Count > 0 &&
                _dgvOtherInvoiceDetails.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvOtherInvoiceDetails.Rows.Count > 0)
            {
                currentScrollIndex = _dgvOtherInvoiceDetails.FirstDisplayedScrollingRowIndex;
            }

            await LoadOtherInvoiceDetailsAsync(selectedId, currentScrollIndex);
        }

        private void DgvOtherInvoiceDetails_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvOtherInvoiceDetails.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvOtherInvoiceDetails.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvOtherInvoiceDetails.Visible = true;
                        _dgvOtherInvoiceDetails.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvOtherInvoiceDetails.ClearSelection();
                targetRow.Selected = true;
                _dgvOtherInvoiceDetails.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvOtherInvoiceDetails.Rows.Count)
                {
                    _dgvOtherInvoiceDetails.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvOtherInvoiceDetails.Visible = true;
                    _dgvOtherInvoiceDetails.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadOtherInvoiceDetailsAsync();
        }

        private async void BtnSearch_Click(object? sender, EventArgs e)
        {
            await LoadOtherInvoiceDetailsAsync();
        }

        private async void BtnClearSearch_Click(object? sender, EventArgs e)
        {
            _txtSearchBillingTo.Text = "";
            _txtSearchBuildingName.Text = "";
            _cmbSearchCategory.SelectedIndex = 0;
            await LoadOtherInvoiceDetailsAsync();
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var billingYearMonth = InvoiceDetailDataAccess.FormatBillingYearMonth(
                _dtpBillingYearMonth.Value.Year,
                _dtpBillingYearMonth.Value.Month);
            var form = new OtherInvoiceDetailEditForm(billingYearMonth);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadOtherInvoiceDetailsAsync();
            }
        }

        private async void BtnCopy_Click(object? sender, EventArgs e)
        {
            if (_dgvOtherInvoiceDetails!.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする請求明細を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvOtherInvoiceDetails.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var invoiceDetail = await InvoiceDetailDataAccess.GetInvoiceDetailByIdAsync(id);
            if (invoiceDetail != null)
            {
                // コピーモードで新規登録フォームを開く
                var form = new OtherInvoiceDetailEditForm(invoiceDetail, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadOtherInvoiceDetailsAsync();
                }
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvOtherInvoiceDetails!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する請求明細を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvOtherInvoiceDetails.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvOtherInvoiceDetails.FirstDisplayedScrollingRowIndex;
            var invoiceDetail = await InvoiceDetailDataAccess.GetInvoiceDetailByIdAsync(id);
            if (invoiceDetail != null)
            {
                var form = new OtherInvoiceDetailEditForm(invoiceDetail);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadOtherInvoiceDetailsAsync(id, currentScrollIndex);
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvOtherInvoiceDetails!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する請求明細を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvOtherInvoiceDetails.SelectedRows[0];
            var buildingName = selectedRow.Cells["建物名称"].Value?.ToString() ?? "";
            var result = MessageBox.Show($"請求明細「{buildingName}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var id = (int)selectedRow.Cells["Id"].Value;
                    var success = await InvoiceDetailDataAccess.DeleteInvoiceDetailAsync(id);
                    if (success)
                    {
                        MessageBox.Show("請求明細を削除しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadOtherInvoiceDetailsAsync();
                    }
                    else
                    {
                        MessageBox.Show("請求明細の削除に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

