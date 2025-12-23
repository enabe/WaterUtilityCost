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
        }

        private async void OtherInvoiceDetailForm_Load(object? sender, EventArgs e)
        {
            await LoadOtherInvoiceDetailsAsync();
        }

        private async void DtpBillingYearMonth_ValueChanged(object? sender, EventArgs e)
        {
            await LoadOtherInvoiceDetailsAsync();
        }

        private async Task LoadOtherInvoiceDetailsAsync()
        {
            try
            {
                var invoiceDetails = await InvoiceDetailDataAccess.GetOtherInvoiceDetailsAsync();

                // 請求年月でフィルタリング（決定請求日が指定された年月のデータのみ）
                var billingYearMonth = _dtpBillingYearMonth.Value;
                invoiceDetails = invoiceDetails.Where(i =>
                    i.ConfirmedBillingDate.HasValue &&
                    i.ConfirmedBillingDate.Value.Year == billingYearMonth.Year &&
                    i.ConfirmedBillingDate.Value.Month == billingYearMonth.Month).ToList();

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

                _dgvOtherInvoiceDetails!.DataSource = filteredList.Select(i => new
                {
                    Id = i.Id,
                    請求先 = i.BillingTo,
                    貸主 = i.Lessor,
                    建物名称 = i.BuildingName,
                    借主 = i.Lessee,
                    部屋番号 = i.RoomNumber,
                    種別 = i.Category,
                    内容 = i.Content,
                    税込金額 = i.TaxInclusiveAmount,
                    税率 = i.TaxRate,
                    業者 = i.Contractor,
                    インボイス番号 = i.InvoiceNumber,
                    決定請求日 = i.ConfirmedBillingDate?.ToString("yyyy/MM/dd") ?? ""
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvOtherInvoiceDetails.Columns["Id"] != null)
                {
                    _dgvOtherInvoiceDetails.Columns["Id"].Width = 40;
                }

                // 税込金額の合計を計算して表示
                var totalAmount = filteredList.Sum(i => i.TaxInclusiveAmount);
                _lblTotalAmount.Text = $"合計金額: ¥{totalAmount:N0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            var form = new OtherInvoiceDetailEditForm();
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
            var invoiceDetail = await InvoiceDetailDataAccess.GetInvoiceDetailByIdAsync(id);
            if (invoiceDetail != null)
            {
                var form = new OtherInvoiceDetailEditForm(invoiceDetail);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadOtherInvoiceDetailsAsync();
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

