using System;
using System.Drawing;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 電気料金請求情報登録・編集フォーム
    /// </summary>
    public partial class ElectricBillingForm : Form
    {
        private ElectricBilling _currentElectricBilling;
        private bool _isEditMode;


        public ElectricBillingForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _currentElectricBilling = new ElectricBilling(); // Initialize for new billing
            this.Load += ElectricBillingForm_Load;
        }

        public ElectricBillingForm(ElectricBilling electricBilling) : this()
        {
            _currentElectricBilling = electricBilling;
            _isEditMode = true;
        }

        private async void ElectricBillingForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingNamesAsync();
            if (_isEditMode)
            {
                LoadElectricBillingData();
            }
        }

        private async Task LoadBuildingNamesAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                cmbBuildingName.DataSource = buildings;
                cmbBuildingName.DisplayMember = "Name";
                cmbBuildingName.ValueMember = "Name"; // Use Name as value for simplicity
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ビル名の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "電気料金請求情報編集" : "電気料金請求情報登録";

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
        }

        private void LoadElectricBillingData()
        {
            txtBillingYearMonth.Text = _currentElectricBilling.BillingYearMonth;
            cmbBuildingName.SelectedValue = _currentElectricBilling.BuildingName;
            txtUsageAmount.Text = _currentElectricBilling.UsageAmount.ToString();
            dtpStartDate.Value = _currentElectricBilling.StartDate;
            dtpEndDate.Value = _currentElectricBilling.EndDate;
            txtBasicCharge.Text = _currentElectricBilling.BasicCharge.ToString();
            txtPowerCharge.Text = _currentElectricBilling.PowerCharge.ToString();
            txtTaxRate.Text = _currentElectricBilling.TaxRate.ToString();
            txtCustomerNumber.Text = _currentElectricBilling.CustomerNumber;
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtBillingYearMonth.Text) ||
                cmbBuildingName.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtUsageAmount.Text) ||
                string.IsNullOrWhiteSpace(txtBasicCharge.Text) ||
                string.IsNullOrWhiteSpace(txtPowerCharge.Text))
            {
                MessageBox.Show("全ての必須項目を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtBillingYearMonth.Text, @"^\d{4}-\d{2}$"))
            {
                MessageBox.Show("受領請求年月はYYYY-MM形式で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (!decimal.TryParse(txtPowerCharge.Text, out decimal powerCharge))
            {
                MessageBox.Show("使用料金は数値で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtTaxRate.Text) && !decimal.TryParse(txtTaxRate.Text, out decimal taxRate))
            {
                MessageBox.Show("税率は数値で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentElectricBilling.BillingYearMonth = txtBillingYearMonth.Text;
            _currentElectricBilling.BuildingName = cmbBuildingName.SelectedValue.ToString() ?? string.Empty;
            _currentElectricBilling.UsageAmount = usageAmount;
            _currentElectricBilling.StartDate = dtpStartDate.Value;
            _currentElectricBilling.EndDate = dtpEndDate.Value;
            _currentElectricBilling.BasicCharge = basicCharge;
            _currentElectricBilling.PowerCharge = powerCharge;
            _currentElectricBilling.TaxRate = string.IsNullOrWhiteSpace(txtTaxRate.Text) ? 0 : decimal.Parse(txtTaxRate.Text);
            _currentElectricBilling.CustomerNumber = txtCustomerNumber.Text;

            try
            {
                if (_isEditMode)
                {
                    await ElectricBillingDataAccess.UpdateElectricBillingAsync(_currentElectricBilling);
                }
                else
                {
                    await ElectricBillingDataAccess.AddElectricBillingAsync(_currentElectricBilling);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public ElectricBilling GetElectricBilling()
        {
            return _currentElectricBilling;
        }
    }
}


