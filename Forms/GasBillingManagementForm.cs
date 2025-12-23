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
    /// ガス料金請求データ管理フォーム
    /// </summary>
    public partial class GasBillingManagementForm : Form
    {

        public GasBillingManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += GasBillingManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvGasBillings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvGasBillings.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
        }

        private async void GasBillingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadGasBillingsAsync();
        }

        private async Task LoadGasBillingsAsync()
        {
            try
            {
                var gasBillings = await GasBillingDataAccess.GetAllGasBillingsAsync();
                _dgvGasBillings.DataSource = gasBillings.Select(gb => new
                {
                    gb.Id,
                    受領請求年月 = gb.BillingYearMonth,
                    ビル名 = gb.BuildingName,
                    使用量 = gb.UsageAmount,
                    使用期間開始 = gb.StartDate.ToString("yyyy-MM-dd"),
                    使用期間終了 = gb.EndDate.ToString("yyyy-MM-dd"),
                    基本料金 = gb.BasicCharge,
                    使用料金 = gb.UsageCharge,
                    税率 = gb.TaxRate,
                    お客様番号 = gb.CustomerNumber
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvGasBillings.Columns["Id"] != null)
                {
                    _dgvGasBillings.Columns["Id"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ガス料金請求データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new GasBillingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadGasBillingsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvGasBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集するガス料金請求データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvGasBillings.SelectedRows[0];
            var gasBillingId = (int)selectedRow.Cells["Id"].Value;

            var gasBilling = await GasBillingDataAccess.GetGasBillingByIdAsync(gasBillingId);
            if (gasBilling != null)
            {
                var form = new GasBillingForm(gasBilling);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadGasBillingsAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvGasBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除するガス料金請求データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvGasBillings.SelectedRows[0];
            var buildingName = selectedRow.Cells["ビル名"].Value?.ToString() ?? "";
            var billingYearMonth = selectedRow.Cells["受領請求年月"].Value?.ToString() ?? "";
            var result = MessageBox.Show($"ガス料金請求データ「{buildingName} - {billingYearMonth}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var gasBillingId = (int)selectedRow.Cells["Id"].Value;
                    await GasBillingDataAccess.DeleteGasBillingAsync(gasBillingId);
                    await LoadGasBillingsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadGasBillingsAsync();
        }
    }
}


