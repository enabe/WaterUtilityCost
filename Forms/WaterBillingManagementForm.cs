using System;
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
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
        }

        private async void WaterBillingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadWaterBillingsAsync();
        }

        private async Task LoadWaterBillingsAsync()
        {
            try
            {
                var waterBillings = await WaterBillingDataAccess.GetAllWaterBillingsAsync();
                _dgvWaterBillings.DataSource = waterBillings.Select(wb => new
                {
                    wb.Id,
                    受領請求年月 = wb.BillingYearMonth,
                    ビル名 = wb.BuildingName,
                    使用量 = wb.UsageAmount,
                    使用期間開始 = wb.StartDate.ToString("yyyy-MM-dd"),
                    使用期間終了 = wb.EndDate.ToString("yyyy-MM-dd"),
                    基本料金 = wb.BasicCharge,
                    使用料金 = wb.UsageCharge,
                    税率 = wb.TaxRate,
                    お客様番号 = wb.CustomerNumber
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvWaterBillings.Columns["Id"] != null)
                {
                    _dgvWaterBillings.Columns["Id"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"水道料金請求データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new WaterBillingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadWaterBillingsAsync();
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

            var waterBilling = await WaterBillingDataAccess.GetWaterBillingByIdAsync(waterBillingId);
            if (waterBilling != null)
            {
                var form = new WaterBillingForm(waterBilling);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadWaterBillingsAsync();
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
    }
}


