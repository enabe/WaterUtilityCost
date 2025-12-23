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
    /// 電気料金請求データ管理フォーム
    /// </summary>
    public partial class ElectricBillingManagementForm : Form
    {

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
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
        }

        private async void ElectricBillingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadElectricBillingsAsync();
        }

        private async Task LoadElectricBillingsAsync()
        {
            try
            {
                var electricBillings = await ElectricBillingDataAccess.GetAllElectricBillingsAsync();
                _dgvElectricBillings.DataSource = electricBillings.Select(eb => new
                {
                    eb.Id,
                    受領請求年月 = eb.BillingYearMonth,
                    ビル名 = eb.BuildingName,
                    使用量 = eb.UsageAmount,
                    使用期間開始 = eb.StartDate.ToString("yyyy-MM-dd"),
                    使用期間終了 = eb.EndDate.ToString("yyyy-MM-dd"),
                    基本料金 = eb.BasicCharge,
                    使用料金 = eb.PowerCharge,
                    税率 = eb.TaxRate,
                    お客様番号 = eb.CustomerNumber
                }).OrderByDescending(x => x.Id).ToList();

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"電気料金請求データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new ElectricBillingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadElectricBillingsAsync();
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

            var electricBilling = await ElectricBillingDataAccess.GetElectricBillingByIdAsync(electricBillingId);
            if (electricBilling != null)
            {
                var form = new ElectricBillingForm(electricBilling);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadElectricBillingsAsync();
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
    }
}


