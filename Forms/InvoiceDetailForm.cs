using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;
using System.Globalization;
using System.IO;
using System.Text;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 水道光熱費請求明細一覧表示フォーム
    /// </summary>
    public partial class InvoiceDetailForm : Form
    {
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class InvoiceDetailGridRow
        {
            public int Id { get; set; }
            public string 請求先 { get; set; } = string.Empty;
            public string 貸主 { get; set; } = string.Empty;
            public string 建物名称 { get; set; } = string.Empty;
            public string 借主 { get; set; } = string.Empty;
            public string 部屋番号 { get; set; } = string.Empty;
            public decimal? 面積_m3 { get; set; }
            public string 種別 { get; set; } = string.Empty;
            public string 内容 { get; set; } = string.Empty;
            public decimal? 子使用量 { get; set; }
            public decimal? 使用量 { get; set; }
            public string 単位 { get; set; } = string.Empty;
            public decimal 税込金額 { get; set; }
            public decimal 税率 { get; set; }
            public DateTime? 子メータ開始 { get; set; }
            public DateTime? 子メータ終了 { get; set; }
            public DateTime? 親メータ開始 { get; set; }
            public DateTime? 親メータ終了 { get; set; }
            public string 請求年月 { get; set; } = string.Empty;
            public DateTime? 請求予定日 { get; set; }
        }

        public InvoiceDetailForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += InvoiceDetailForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // 請求年月を現在の年月に設定
            var now = DateTime.Now;
            _dtpBillingYearMonth.Value = new DateTime(now.Year, now.Month, 1);
            
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvInvoiceDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvInvoiceDetails.ColumnHeadersHeight = 30;
            _dgvInvoiceDetails.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            // 種別ComboBoxの初期化
            _cmbSearchCategory.Items.Add(""); // 空の選択肢（すべて）
            _cmbSearchCategory.Items.Add("電気");
            _cmbSearchCategory.Items.Add("水道");
            _cmbSearchCategory.Items.Add("ガス");
            _cmbSearchCategory.SelectedIndex = 0; // 空を選択
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnCopy.Click += BtnCopy_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _btnExportCsv.Click += BtnExportCsv_Click;
            _btnExportComparisonCsv.Click += BtnExportComparisonCsv_Click;
            _btnExportYearComparisonCsv.Click += BtnExportYearComparisonCsv_Click;
            _dtpBillingYearMonth.ValueChanged += DtpBillingYearMonth_ValueChanged;
            _btnSearch.Click += BtnSearch_Click;
            _btnClearSearch.Click += BtnClearSearch_Click;
            _dgvInvoiceDetails.DataBindingComplete += DgvInvoiceDetails_DataBindingComplete;
            _dgvInvoiceDetails.DoubleClick += DgvInvoiceDetails_DoubleClick;
            _dgvInvoiceDetails.ColumnHeaderMouseClick += DgvInvoiceDetails_ColumnHeaderMouseClick;
        }

        private async void DtpBillingYearMonth_ValueChanged(object? sender, EventArgs e)
        {
            await LoadInvoiceDetailsAsync();
        }

        private async void InvoiceDetailForm_Load(object? sender, EventArgs e)
        {
            await LoadInvoiceDetailsAsync();
        }

        private async Task LoadInvoiceDetailsAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var billingYearMonth = _dtpBillingYearMonth.Value;
                var invoiceDetails = await InvoiceDetailDataAccess.GetInvoiceDetailsByBillingYearMonthAsync(billingYearMonth.Year, billingYearMonth.Month);

                // 検索条件でフィルタリング
                var filteredDetails = invoiceDetails.AsEnumerable();
                
                // 種別を水道、電気、ガスのみに制限
                var allowedCategories = new[] { "水道", "電気", "ガス" };
                filteredDetails = filteredDetails.Where(i => 
                    i.Category != null && allowedCategories.Any(c => c.Equals(i.Category, StringComparison.OrdinalIgnoreCase)));
                
                // 請求先でフィルタ
                if (!string.IsNullOrWhiteSpace(_txtSearchBillingTo.Text))
                {
                    var searchBillingTo = NormalizeSearchKey(_txtSearchBillingTo.Text);
                    filteredDetails = filteredDetails.Where(i =>
                        !string.IsNullOrEmpty(i.BillingTo) &&
                        NormalizeSearchKey(i.BillingTo).Contains(searchBillingTo));
                }
                
                // 建物名でフィルタ
                if (!string.IsNullOrWhiteSpace(_txtSearchBuildingName.Text))
                {
                    var searchBuildingName = NormalizeSearchKey(_txtSearchBuildingName.Text);
                    filteredDetails = filteredDetails.Where(i =>
                        !string.IsNullOrEmpty(i.BuildingName) &&
                        NormalizeSearchKey(i.BuildingName).Contains(searchBuildingName));
                }
                
                // 種別でフィルタ（検索条件として）
                var hasCategoryFilter = _cmbSearchCategory.SelectedIndex > 0 && _cmbSearchCategory.SelectedItem != null;
                if (hasCategoryFilter)
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
                    _dgvInvoiceDetails.SuspendLayout();
                    _dgvInvoiceDetails.Visible = false;
                }

                var rows = filteredList.Select(i => new InvoiceDetailGridRow
                {
                    Id = i.Id,
                    請求先 = i.BillingTo ?? "",
                    貸主 = i.Lessor ?? "",
                    建物名称 = i.BuildingName ?? "",
                    借主 = i.Lessee ?? "",
                    部屋番号 = i.RoomNumber ?? "",
                    面積_m3 = i.RoomArea,
                    種別 = i.Category ?? "",
                    内容 = i.Content ?? "",
                    子使用量 = i.ChildMeterUsage,
                    使用量 = i.UsageAmount,
                    単位 = i.Unit ?? "",
                    税込金額 = i.TaxInclusiveAmount,
                    税率 = i.TaxRate,
                    子メータ開始 = i.ChildMeterStartDate,
                    子メータ終了 = i.ChildMeterEndDate,
                    親メータ開始 = i.ParentMeterStartDate,
                    親メータ終了 = i.ParentMeterEndDate,
                    請求年月 = i.BillingYearMonth ?? "",
                    請求予定日 = i.ConfirmedBillingDate
                });

                _dgvInvoiceDetails!.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

                // 列幅を手動調整できるようにして横スクロールを有効化
                _dgvInvoiceDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                _dgvInvoiceDetails.ScrollBars = ScrollBars.Both;
                foreach (DataGridViewColumn column in _dgvInvoiceDetails.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }

                // ID列の幅を狭く設定
                if (_dgvInvoiceDetails.Columns["Id"] != null)
                {
                    _dgvInvoiceDetails.Columns["Id"].Width = 40;
                }

                // 面積(m3)列の見出しを整形
                if (_dgvInvoiceDetails.Columns["面積_m3"] != null)
                {
                    var areaCol = _dgvInvoiceDetails.Columns["面積_m3"];
                    areaCol.HeaderText = "面積(m3)";
                    areaCol.DefaultCellStyle.Format = "N2";
                    areaCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (_dgvInvoiceDetails.Columns["子使用量"] != null)
                {
                    _dgvInvoiceDetails.Columns["子使用量"].DefaultCellStyle.Format = "N2";
                    _dgvInvoiceDetails.Columns["子使用量"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (_dgvInvoiceDetails.Columns["使用量"] != null)
                {
                    _dgvInvoiceDetails.Columns["使用量"].DefaultCellStyle.Format = "N2";
                    _dgvInvoiceDetails.Columns["使用量"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                foreach (var dateColumnName in new[] { "子メータ開始", "子メータ終了", "親メータ開始", "親メータ終了", "請求予定日" })
                {
                    if (_dgvInvoiceDetails.Columns[dateColumnName] != null)
                    {
                        _dgvInvoiceDetails.Columns[dateColumnName].DefaultCellStyle.Format = "yyyy-MM-dd";
                    }
                }

                // 税込金額列を右詰め（ヘッダーは一覧共通で中央）
                if (_dgvInvoiceDetails.Columns["税込金額"] != null)
                {
                    var taxCol = _dgvInvoiceDetails.Columns["税込金額"];
                    taxCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    taxCol.DefaultCellStyle.Format = "N0";
                }

                foreach (DataGridViewColumn column in _dgvInvoiceDetails.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // 税込金額の合計を計算して表示
                var totalAmount = filteredList.Sum(i => i.TaxInclusiveAmount);
                _lblTotalAmount.Text = $"税込金額合計: ¥{totalAmount:N0}";

                // 使用量の合計を表示
                if (hasCategoryFilter)
                {
                    var totalUsage = filteredList.Sum(i => i.UsageAmount ?? 0);
                    _lblTotalUsage.Text = $"使用量合計: {totalUsage:N2}";
                }
                else
                {
                    _lblTotalUsage.Text = "使用量合計: ー";
                }
                // 子使用量の合計を表示
                if (hasCategoryFilter)
                {
                    var totalChildUsage = filteredList.Sum(i => i.ChildMeterUsage ?? 0);
                    _lblTotalChildUsage.Text = $"子使用量合計: {totalChildUsage:N2}";
                }
                else
                {
                    _lblTotalChildUsage.Text = "子使用量合計: ー";
                }
                // 面積の合計を表示
                if (hasCategoryFilter)
                {
                    var totalArea = filteredList.Sum(i => i.RoomArea ?? 0);
                    _lblTotalArea.Text = $"面積合計: {totalArea:N2}";
                }
                else
                {
                    _lblTotalArea.Text = "面積合計: ー";
                }
                _lblTotalAmount.Visible = true;
                _lblTotalUsage.Visible = true;
                _lblTotalChildUsage.Visible = true;
                _lblTotalArea.Visible = true;
                _lblTotalAmount.BringToFront();
                _lblTotalUsage.BringToFront();
                _lblTotalChildUsage.BringToFront();
                _lblTotalArea.BringToFront();

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvInvoiceDetails.Visible = true;
                    _dgvInvoiceDetails.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvInvoiceDetails.Visible = true;
                    _dgvInvoiceDetails.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<InvoiceDetailGridRow> ApplySort(IEnumerable<InvoiceDetailGridRow> source)
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
                ("面積_m3", true) => source.OrderBy(x => x.面積_m3),
                ("面積_m3", false) => source.OrderByDescending(x => x.面積_m3),
                ("種別", true) => source.OrderBy(x => x.種別),
                ("種別", false) => source.OrderByDescending(x => x.種別),
                ("内容", true) => source.OrderBy(x => x.内容),
                ("内容", false) => source.OrderByDescending(x => x.内容),
                ("子使用量", true) => source.OrderBy(x => x.子使用量),
                ("子使用量", false) => source.OrderByDescending(x => x.子使用量),
                ("使用量", true) => source.OrderBy(x => x.使用量),
                ("使用量", false) => source.OrderByDescending(x => x.使用量),
                ("単位", true) => source.OrderBy(x => x.単位),
                ("単位", false) => source.OrderByDescending(x => x.単位),
                ("税込金額", true) => source.OrderBy(x => x.税込金額),
                ("税込金額", false) => source.OrderByDescending(x => x.税込金額),
                ("税率", true) => source.OrderBy(x => x.税率),
                ("税率", false) => source.OrderByDescending(x => x.税率),
                ("子メータ開始", true) => source.OrderBy(x => x.子メータ開始),
                ("子メータ開始", false) => source.OrderByDescending(x => x.子メータ開始),
                ("子メータ終了", true) => source.OrderBy(x => x.子メータ終了),
                ("子メータ終了", false) => source.OrderByDescending(x => x.子メータ終了),
                ("親メータ開始", true) => source.OrderBy(x => x.親メータ開始),
                ("親メータ開始", false) => source.OrderByDescending(x => x.親メータ開始),
                ("親メータ終了", true) => source.OrderBy(x => x.親メータ終了),
                ("親メータ終了", false) => source.OrderByDescending(x => x.親メータ終了),
                ("請求年月", true) => source.OrderBy(x => x.請求年月),
                ("請求年月", false) => source.OrderByDescending(x => x.請求年月),
                ("請求予定日", true) => source.OrderBy(x => x.請求予定日),
                ("請求予定日", false) => source.OrderByDescending(x => x.請求予定日),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvInvoiceDetails.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvInvoiceDetails.Columns.Contains(_sortColumn))
            {
                _dgvInvoiceDetails.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvInvoiceDetails_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvInvoiceDetails.Columns[e.ColumnIndex].Name;
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
            if (_dgvInvoiceDetails.SelectedRows.Count > 0 &&
                _dgvInvoiceDetails.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvInvoiceDetails.Rows.Count > 0)
            {
                currentScrollIndex = _dgvInvoiceDetails.FirstDisplayedScrollingRowIndex;
            }

            await LoadInvoiceDetailsAsync(selectedId, currentScrollIndex);
        }

        private static string NormalizeSearchKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value
                .Normalize(NormalizationForm.FormKC)
                .Replace(" ", string.Empty)
                .Replace("　", string.Empty)
                .ToLowerInvariant();
        }

        private void DgvInvoiceDetails_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvInvoiceDetails.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvInvoiceDetails.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvInvoiceDetails.Visible = true;
                        _dgvInvoiceDetails.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvInvoiceDetails.ClearSelection();
                targetRow.Selected = true;
                _dgvInvoiceDetails.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvInvoiceDetails.Rows.Count)
                {
                    _dgvInvoiceDetails.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvInvoiceDetails.Visible = true;
                    _dgvInvoiceDetails.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private async void BtnSearch_Click(object? sender, EventArgs e)
        {
            await LoadInvoiceDetailsAsync();
        }

        private async void BtnClearSearch_Click(object? sender, EventArgs e)
        {
            _txtSearchBillingTo.Text = "";
            _txtSearchBuildingName.Text = "";
            _cmbSearchCategory.SelectedIndex = 0;
            await LoadInvoiceDetailsAsync();
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var billingYearMonth = InvoiceDetailDataAccess.FormatBillingYearMonth(
                _dtpBillingYearMonth.Value.Year,
                _dtpBillingYearMonth.Value.Month);
            var form = new InvoiceDetailEditForm(billingYearMonth);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadInvoiceDetailsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvInvoiceDetails!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する請求明細を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvInvoiceDetails.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvInvoiceDetails.FirstDisplayedScrollingRowIndex;
            var invoiceDetail = await InvoiceDetailDataAccess.GetInvoiceDetailByIdAsync(id);
            if (invoiceDetail != null)
            {
                var form = new InvoiceDetailEditForm(invoiceDetail);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadInvoiceDetailsAsync(id, currentScrollIndex);
                }
            }
        }

        private async void DgvInvoiceDetails_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvInvoiceDetails!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvInvoiceDetails.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvInvoiceDetails.FirstDisplayedScrollingRowIndex;
            var invoiceDetail = await InvoiceDetailDataAccess.GetInvoiceDetailByIdAsync(id);
            if (invoiceDetail != null)
            {
                var form = new InvoiceDetailEditForm(invoiceDetail);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadInvoiceDetailsAsync(id, currentScrollIndex);
                }
            }
        }

        private async void BtnCopy_Click(object? sender, EventArgs e)
        {
            if (_dgvInvoiceDetails!.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする請求明細を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvInvoiceDetails.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var invoiceDetail = await InvoiceDetailDataAccess.GetInvoiceDetailByIdAsync(id);
            if (invoiceDetail != null)
            {
                // コピーモードで新規登録フォームを開く
                var form = new InvoiceDetailEditForm(invoiceDetail, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadInvoiceDetailsAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvInvoiceDetails!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する請求明細を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvInvoiceDetails.SelectedRows[0];
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
                        await LoadInvoiceDetailsAsync();
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

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadInvoiceDetailsAsync();
        }

        private async void BtnExportCsv_Click(object? sender, EventArgs e)
        {
            try
            {
                // ファイル保存ダイアログをUIスレッドで実行（STAモードが必要）
                string? selectedFilePath = (string?)this.Invoke(new Func<string?>(() =>
                {
                    using var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = true,
                        FileName = $"水道光熱費請求明細一覧_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                    };

                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        return saveFileDialog.FileName;
                    }
                    return null;
                }));

                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    await ExportToCsvAsync(selectedFilePath);
                    MessageBox.Show("CSVファイルの出力が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CSV出力に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ExportToCsvAsync(string filePath)
        {
            var billingYearMonth = _dtpBillingYearMonth.Value;
            var details = await InvoiceDetailDataAccess.GetInvoiceDetailsByBillingYearMonthAsync(billingYearMonth.Year, billingYearMonth.Month);
            
            // UIスレッドでファイル操作を同期的に実行（STAモードが必要なため）
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            
            // ヘッダー行
            writer.WriteLine("ID,請求先,貸主,建物名称,借主,部屋番号,面積(m3),種別,内容,子使用量,使用量,単位,税込金額,税率,子メータ使用開始日,子メータ使用終了日,親メータ使用開始日,親メータ使用終了日,請求年月,請求予定日,登録日,更新日");
            
            // データ行
            foreach (var detail in details)
            {
                var line = string.Join(",",
                    detail.Id,
                    EscapeCsvField(detail.BillingTo),
                    EscapeCsvField(detail.Lessor),
                    EscapeCsvField(detail.BuildingName),
                    EscapeCsvField(detail.Lessee),
                    EscapeCsvField(detail.RoomNumber),
                    detail.RoomArea?.ToString() ?? "",
                    EscapeCsvField(detail.Category),
                    EscapeCsvField(detail.Content),
                    detail.ChildMeterUsage?.ToString() ?? "",
                    detail.UsageAmount?.ToString() ?? "",
                    EscapeCsvField(detail.Unit),
                    detail.TaxInclusiveAmount.ToString(),
                    detail.TaxRate.ToString(),
                    detail.ChildMeterStartDate?.ToString("yyyy-MM-dd") ?? "",
                    detail.ChildMeterEndDate?.ToString("yyyy-MM-dd") ?? "",
                    detail.ParentMeterStartDate?.ToString("yyyy-MM-dd") ?? "",
                    detail.ParentMeterEndDate?.ToString("yyyy-MM-dd") ?? "",
                    EscapeCsvField(detail.BillingYearMonth),
                    detail.ConfirmedBillingDate?.ToString("yyyy-MM-dd") ?? "",
                    detail.CreatedAt != default(DateTime) ? detail.CreatedAt.ToString("yyyy-MM-dd") : "",
                    detail.UpdatedAt != default(DateTime) ? detail.UpdatedAt.ToString("yyyy-MM-dd") : ""
                );
                writer.WriteLine(line);
            }
        }

        private async void BtnExportComparisonCsv_Click(object? sender, EventArgs e)
        {
            try
            {
                // ファイル保存ダイアログをUIスレッドで実行（STAモードが必要）
                string? selectedFilePath = (string?)this.Invoke(new Func<string?>(() =>
                {
                    var billingYearMonth = _dtpBillingYearMonth.Value;
                    using var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = true,
                        FileName = $"請求明細前月比較_{billingYearMonth:yyyyMM}_{billingYearMonth.AddMonths(-1):yyyyMM}.csv"
                    };

                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        return saveFileDialog.FileName;
                    }
                    return null;
                }));

                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    await ExportComparisonCsvAsync(selectedFilePath);
                    MessageBox.Show("前月比較CSVファイルの出力が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"前月比較CSV出力に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ExportComparisonCsvAsync(string filePath)
        {
            var billingYearMonth = _dtpBillingYearMonth.Value;
            var currentYear = billingYearMonth.Year;
            var currentMonth = billingYearMonth.Month;
            
            // 前月の年月を計算
            var previousMonthDate = billingYearMonth.AddMonths(-1);
            var previousYear = previousMonthDate.Year;
            var previousMonth = previousMonthDate.Month;

            // 当月と前月のデータを取得
            var currentDetails = await InvoiceDetailDataAccess.GetInvoiceDetailsByBillingYearMonthAsync(currentYear, currentMonth);
            var previousDetails = await InvoiceDetailDataAccess.GetInvoiceDetailsByBillingYearMonthAsync(previousYear, previousMonth);

            // マッチングキーで辞書を作成（建物名、部屋番号、種別、内容の組み合わせ）
            var previousDict = previousDetails.ToDictionary(
                d => new { 
                    BuildingName = d.BuildingName ?? "", 
                    RoomNumber = d.RoomNumber ?? "", 
                    Category = d.Category ?? "", 
                    Content = d.Content ?? "" 
                },
                d => d
            );

            // UIスレッドでファイル操作を同期的に実行（STAモードが必要なため）
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            
            // ヘッダー行
            writer.WriteLine("請求先,貸主,建物名,借主,部屋番号,種別,内容,前月使用量,当月使用量,前月税込金額,当月税込金額");
            
            // 当月データを基準に比較データを作成
            foreach (var current in currentDetails)
            {
                var key = new { 
                    BuildingName = current.BuildingName ?? "", 
                    RoomNumber = current.RoomNumber ?? "", 
                    Category = current.Category ?? "", 
                    Content = current.Content ?? "" 
                };
                
                // 前月データを検索
                InvoiceDetail? previous = null;
                if (previousDict.TryGetValue(key, out var prev))
                {
                    previous = prev;
                }

                var line = string.Join(",",
                    EscapeCsvField(current.BillingTo),
                    EscapeCsvField(current.Lessor),
                    EscapeCsvField(current.BuildingName),
                    EscapeCsvField(current.Lessee),
                    EscapeCsvField(current.RoomNumber),
                    EscapeCsvField(current.Category),
                    EscapeCsvField(current.Content),
                    previous != null ? previous.UsageAmount?.ToString() ?? "" : "",
                    current.UsageAmount?.ToString() ?? "",
                    previous != null ? previous.TaxInclusiveAmount.ToString() : "",
                    current.TaxInclusiveAmount.ToString()
                );
                writer.WriteLine(line);
            }

            // 前月にのみ存在するデータも追加（当月に存在しないもの）
            foreach (var previous in previousDetails)
            {
                var key = new { 
                    BuildingName = previous.BuildingName ?? "", 
                    RoomNumber = previous.RoomNumber ?? "", 
                    Category = previous.Category ?? "", 
                    Content = previous.Content ?? "" 
                };
                
                // 当月に存在しない場合のみ追加
                if (!currentDetails.Any(c => 
                    (c.BuildingName ?? "") == key.BuildingName &&
                    (c.RoomNumber ?? "") == key.RoomNumber &&
                    (c.Category ?? "") == key.Category &&
                    (c.Content ?? "") == key.Content))
                {
                    var line = string.Join(",",
                        EscapeCsvField(previous.BillingTo),
                        EscapeCsvField(previous.Lessor),
                        EscapeCsvField(previous.BuildingName),
                        EscapeCsvField(previous.Lessee),
                        EscapeCsvField(previous.RoomNumber),
                        EscapeCsvField(previous.Category),
                        EscapeCsvField(previous.Content),
                    previous.UsageAmount?.ToString() ?? "",
                        "",
                        previous.TaxInclusiveAmount.ToString(),
                        ""
                    );
                    writer.WriteLine(line);
                }
            }
        }

        private async void BtnExportYearComparisonCsv_Click(object? sender, EventArgs e)
        {
            try
            {
                // ファイル保存ダイアログをUIスレッドで実行（STAモードが必要）
                string? selectedFilePath = (string?)this.Invoke(new Func<string?>(() =>
                {
                    var billingYearMonth = _dtpBillingYearMonth.Value;
                    var previousYearMonth = billingYearMonth.AddYears(-1);
                    using var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = true,
                        FileName = $"請求明細前年比較_{billingYearMonth:yyyyMM}_{previousYearMonth:yyyyMM}.csv"
                    };

                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        return saveFileDialog.FileName;
                    }
                    return null;
                }));

                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    await ExportYearComparisonCsvAsync(selectedFilePath);
                    MessageBox.Show("前年比較CSVファイルの出力が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"前年比較CSV出力に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ExportYearComparisonCsvAsync(string filePath)
        {
            var billingYearMonth = _dtpBillingYearMonth.Value;
            var currentYear = billingYearMonth.Year;
            var currentMonth = billingYearMonth.Month;
            
            // 前年の年月を計算
            var previousYearDate = billingYearMonth.AddYears(-1);
            var previousYearValue = previousYearDate.Year;
            var previousMonth = previousYearDate.Month;

            // 当月と前年のデータを取得
            var currentDetails = await InvoiceDetailDataAccess.GetInvoiceDetailsByBillingYearMonthAsync(currentYear, currentMonth);
            var previousYearDetails = await InvoiceDetailDataAccess.GetInvoiceDetailsByBillingYearMonthAsync(previousYearValue, previousMonth);

            // マッチングキーで辞書を作成（請求先、貸主、建物名、借主、部屋番号、種別、内容の組み合わせ）
            var previousYearDict = previousYearDetails.ToDictionary(
                d => new { 
                    BillingTo = d.BillingTo ?? "",
                    Lessor = d.Lessor ?? "",
                    BuildingName = d.BuildingName ?? "", 
                    Lessee = d.Lessee ?? "",
                    RoomNumber = d.RoomNumber ?? "", 
                    Category = d.Category ?? "", 
                    Content = d.Content ?? "" 
                },
                d => d
            );

            // UIスレッドでファイル操作を同期的に実行（STAモードが必要なため）
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            
            // ヘッダー行
            writer.WriteLine("請求先,貸主,建物名,借主,部屋番号,種別,内容,前年使用量,当月使用量,前年税込金額,当月税込金額");
            
            // 当月データを基準に比較データを作成
            foreach (var current in currentDetails)
            {
                var key = new { 
                    BillingTo = current.BillingTo ?? "",
                    Lessor = current.Lessor ?? "",
                    BuildingName = current.BuildingName ?? "", 
                    Lessee = current.Lessee ?? "",
                    RoomNumber = current.RoomNumber ?? "", 
                    Category = current.Category ?? "", 
                    Content = current.Content ?? "" 
                };
                
                // 前年データを検索
                InvoiceDetail? previousYearDetail = null;
                if (previousYearDict.TryGetValue(key, out var prev))
                {
                    previousYearDetail = prev;
                }

                var line = string.Join(",",
                    EscapeCsvField(current.BillingTo),
                    EscapeCsvField(current.Lessor),
                    EscapeCsvField(current.BuildingName),
                    EscapeCsvField(current.Lessee),
                    EscapeCsvField(current.RoomNumber),
                    EscapeCsvField(current.Category),
                    EscapeCsvField(current.Content),
                    previousYearDetail != null ? FormatCsvUsageAmount(previousYearDetail.UsageAmount) : "",
                    FormatCsvUsageAmount(current.UsageAmount),
                    previousYearDetail != null ? FormatCsvTaxAmount(previousYearDetail.TaxInclusiveAmount) : "",
                    FormatCsvTaxAmount(current.TaxInclusiveAmount)
                );
                writer.WriteLine(line);
            }

            // 前年にのみ存在するデータも追加（当月に存在しないもの）
            foreach (var previousYearDetail in previousYearDetails)
            {
                var key = new { 
                    BillingTo = previousYearDetail.BillingTo ?? "",
                    Lessor = previousYearDetail.Lessor ?? "",
                    BuildingName = previousYearDetail.BuildingName ?? "", 
                    Lessee = previousYearDetail.Lessee ?? "",
                    RoomNumber = previousYearDetail.RoomNumber ?? "", 
                    Category = previousYearDetail.Category ?? "", 
                    Content = previousYearDetail.Content ?? "" 
                };
                
                // 当月に存在しない場合のみ追加
                if (!currentDetails.Any(c => 
                    (c.BillingTo ?? "") == key.BillingTo &&
                    (c.Lessor ?? "") == key.Lessor &&
                    (c.BuildingName ?? "") == key.BuildingName &&
                    (c.Lessee ?? "") == key.Lessee &&
                    (c.RoomNumber ?? "") == key.RoomNumber &&
                    (c.Category ?? "") == key.Category &&
                    (c.Content ?? "") == key.Content))
                {
                    var line = string.Join(",",
                        EscapeCsvField(previousYearDetail.BillingTo),
                        EscapeCsvField(previousYearDetail.Lessor),
                        EscapeCsvField(previousYearDetail.BuildingName),
                        EscapeCsvField(previousYearDetail.Lessee),
                        EscapeCsvField(previousYearDetail.RoomNumber),
                        EscapeCsvField(previousYearDetail.Category),
                        EscapeCsvField(previousYearDetail.Content),
                    FormatCsvUsageAmount(previousYearDetail.UsageAmount),
                        "",
                        FormatCsvTaxAmount(previousYearDetail.TaxInclusiveAmount),
                        ""
                    );
                    writer.WriteLine(line);
                }
            }
        }

        private static string FormatCsvUsageAmount(decimal? usageAmount)
        {
            return usageAmount.HasValue
                ? usageAmount.Value.ToString("F2", CultureInfo.InvariantCulture)
                : "";
        }

        private static string FormatCsvTaxAmount(decimal taxInclusiveAmount)
        {
            return Math.Floor(taxInclusiveAmount).ToString("F0", CultureInfo.InvariantCulture);
        }

        private static string EscapeCsvField(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            
            // カンマ、ダブルクォート、改行が含まれている場合はダブルクォートで囲む
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                // ダブルクォートをエスケープ
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            
            return value;
        }

        private void _btnEdit_Click(object sender, EventArgs e)
        {

        }
    }
}

