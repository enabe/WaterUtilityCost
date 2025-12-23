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
    /// ガス料金請求情報登録・編集フォーム
    /// </summary>
    public partial class GasBillingForm : Form
    {
        private GasBilling _currentGasBilling;
        private bool _isEditMode;


        public GasBillingForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _currentGasBilling = new GasBilling(); // Initialize for new billing
            this.Load += GasBillingForm_Load;
        }

        public GasBillingForm(GasBilling gasBilling) : this()
        {
            _currentGasBilling = gasBilling;
            _isEditMode = true;
        }

        private async void GasBillingForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingNamesAsync();
            if (_isEditMode)
            {
                LoadGasBillingData();
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
            this.Text = _isEditMode ? "ガス料金請求情報編集" : "ガス料金請求情報登録";

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
        }

        private void LoadGasBillingData()
        {
            txtBillingYearMonth.Text = _currentGasBilling.BillingYearMonth;
            cmbBuildingName.SelectedValue = _currentGasBilling.BuildingName;
            txtUsageAmount.Text = _currentGasBilling.UsageAmount.ToString();
            dtpStartDate.Value = _currentGasBilling.StartDate;
            dtpEndDate.Value = _currentGasBilling.EndDate;
            txtBasicCharge.Text = _currentGasBilling.BasicCharge.ToString();
            txtUsageCharge.Text = _currentGasBilling.UsageCharge.ToString();
            txtTaxRate.Text = _currentGasBilling.TaxRate.ToString();
            txtCustomerNumber.Text = _currentGasBilling.CustomerNumber;
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtBillingYearMonth.Text) ||
                cmbBuildingName.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtUsageAmount.Text) ||
                string.IsNullOrWhiteSpace(txtBasicCharge.Text) ||
                string.IsNullOrWhiteSpace(txtUsageCharge.Text))
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

            _currentGasBilling.BillingYearMonth = txtBillingYearMonth.Text;
            _currentGasBilling.BuildingName = cmbBuildingName.SelectedValue.ToString() ?? string.Empty;
            _currentGasBilling.UsageAmount = usageAmount;
            _currentGasBilling.StartDate = dtpStartDate.Value;
            _currentGasBilling.EndDate = dtpEndDate.Value;
            _currentGasBilling.BasicCharge = basicCharge;
            _currentGasBilling.UsageCharge = usageCharge;
            _currentGasBilling.TaxRate = string.IsNullOrWhiteSpace(txtTaxRate.Text) ? 0 : decimal.Parse(txtTaxRate.Text);
            _currentGasBilling.CustomerNumber = txtCustomerNumber.Text;

            try
            {
                if (_isEditMode)
                {
                    await GasBillingDataAccess.UpdateGasBillingAsync(_currentGasBilling);
                }
                else
                {
                    await GasBillingDataAccess.AddGasBillingAsync(_currentGasBilling);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public GasBilling GetGasBilling()
        {
            return _currentGasBilling;
        }
    }
}


