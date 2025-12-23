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
    /// 水道料金請求データ登録・編集フォーム
    /// </summary>
    public partial class WaterBillingForm : Form
    {
        private WaterBilling _currentWaterBilling;
        private bool _isEditMode;


        public WaterBillingForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _currentWaterBilling = new WaterBilling();
        }

        public WaterBillingForm(WaterBilling waterBilling) : this()
        {
            _currentWaterBilling = waterBilling;
            _isEditMode = true;
        }

        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "水道料金請求データ編集" : "水道料金請求データ登録";

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;

            // フォーム読み込み時にビル名を取得
            this.Load += WaterBillingForm_Load;
        }

        private async void WaterBillingForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingNamesAsync();
            if (_isEditMode)
            {
                LoadWaterBillingData();
            }
        }

        private async Task LoadBuildingNamesAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                cmbBuildingName.DataSource = buildings;
                cmbBuildingName.DisplayMember = "Name";
                cmbBuildingName.ValueMember = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ビル名の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWaterBillingData()
        {
            txtBillingYearMonth.Text = _currentWaterBilling.BillingYearMonth;
            cmbBuildingName.SelectedValue = _currentWaterBilling.BuildingName;
            txtUsageAmount.Text = _currentWaterBilling.UsageAmount.ToString();
            dtpStartDate.Value = _currentWaterBilling.StartDate;
            dtpEndDate.Value = _currentWaterBilling.EndDate;
            txtBasicCharge.Text = _currentWaterBilling.BasicCharge.ToString();
            txtUsageCharge.Text = _currentWaterBilling.UsageCharge.ToString();
            txtTaxRate.Text = _currentWaterBilling.TaxRate.ToString();
            txtCustomerNumber.Text = _currentWaterBilling.CustomerNumber;
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            // バリデーション
            if (string.IsNullOrWhiteSpace(txtBillingYearMonth.Text))
            {
                MessageBox.Show("受領請求年月は必須です。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbBuildingName.SelectedValue == null)
            {
                MessageBox.Show("ビル名は必須です。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtUsageAmount.Text, out decimal usageAmount))
            {
                MessageBox.Show("使用量は数値で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtBasicCharge.Text, out decimal basicCharge))
            {
                MessageBox.Show("基本料金は数値で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtUsageCharge.Text, out decimal usageCharge))
            {
                MessageBox.Show("使用料金は数値で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtTaxRate.Text) && !decimal.TryParse(txtTaxRate.Text, out decimal taxRate))
            {
                MessageBox.Show("税率は数値で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpStartDate.Value >= dtpEndDate.Value)
            {
                MessageBox.Show("終了日は開始日より後の日付を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentWaterBilling.BillingYearMonth = txtBillingYearMonth.Text;
            _currentWaterBilling.BuildingName = cmbBuildingName.SelectedValue?.ToString() ?? string.Empty;
            _currentWaterBilling.UsageAmount = usageAmount;
            _currentWaterBilling.StartDate = dtpStartDate.Value;
            _currentWaterBilling.EndDate = dtpEndDate.Value;
            _currentWaterBilling.BasicCharge = basicCharge;
            _currentWaterBilling.UsageCharge = usageCharge;
            _currentWaterBilling.TaxRate = string.IsNullOrWhiteSpace(txtTaxRate.Text) ? 0 : decimal.Parse(txtTaxRate.Text);
            _currentWaterBilling.CustomerNumber = txtCustomerNumber.Text;

            try
            {
                if (_isEditMode)
                {
                    await WaterBillingDataAccess.UpdateWaterBillingAsync(_currentWaterBilling);
                }
                else
                {
                    await WaterBillingDataAccess.AddWaterBillingAsync(_currentWaterBilling);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public WaterBilling GetWaterBilling()
        {
            return _currentWaterBilling;
        }
    }
}
