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
        private sealed class ContractorComboItem
        {
            public int? Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private ElectricBilling _currentElectricBilling;
        private bool _isEditMode;
        private bool _isCopyMode;


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

        /// <summary>
        /// 既存の電気料金請求データをコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        /// <param name="electricBilling">コピー元の電気料金請求データ</param>
        /// <param name="isCopyMode">コピーモード</param>
        public ElectricBillingForm(ElectricBilling electricBilling, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentElectricBilling = electricBilling;
                _isEditMode = false;
                _isCopyMode = true;
                this.Text = "電気料金請求情報登録";
            }
        }

        private async void ElectricBillingForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingNamesAsync();
            if (_isEditMode)
            {
                await LoadElectricBillingDataAsync();
            }
            else if (_isCopyMode)
            {
                await LoadElectricBillingDataForCopyAsync();
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
                await LoadContractorsByBuildingAsync();
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
            cmbBuildingName.SelectedIndexChanged += CmbBuildingName_SelectedIndexChanged;
        }

        /// <summary>
        /// コピーモード用のデータ読み込み（ビル名・使用期間・税率・お客様番号・業者をコピーし、受領請求年月・使用量・料金は空にする）
        /// </summary>
        private async Task LoadElectricBillingDataForCopyAsync()
        {
            cmbBuildingName.SelectedIndexChanged -= CmbBuildingName_SelectedIndexChanged;

            cmbBuildingName.SelectedValue = _currentElectricBilling.BuildingName;
            await LoadContractorsByBuildingAsync();

            dtpStartDate.Value = _currentElectricBilling.StartDate;
            dtpEndDate.Value = _currentElectricBilling.EndDate;
            txtTaxRate.Text = _currentElectricBilling.TaxRate.ToString();
            txtCustomerNumber.Text = _currentElectricBilling.CustomerNumber ?? string.Empty;

            if (_currentElectricBilling.ContractorId.HasValue)
            {
                var contractorId = _currentElectricBilling.ContractorId.Value;
                if (cmbContractor.Items.Cast<object>()
                    .Select(i => i as ContractorComboItem)
                    .Any(i => i != null && i.Id == contractorId))
                {
                    cmbContractor.SelectedValue = contractorId;
                }
                else
                {
                    cmbContractor.SelectedIndex = 0;
                }
            }
            else
            {
                cmbContractor.SelectedIndex = 0;
            }

            txtBillingYearMonth.Text = string.Empty;
            txtUsageAmount.Text = string.Empty;
            txtBasicCharge.Text = string.Empty;
            txtPowerCharge.Text = string.Empty;

            cmbBuildingName.SelectedIndexChanged += CmbBuildingName_SelectedIndexChanged;
        }

        private async Task LoadElectricBillingDataAsync()
        {
            txtBillingYearMonth.Text = _currentElectricBilling.BillingYearMonth;
            cmbBuildingName.SelectedValue = _currentElectricBilling.BuildingName;
            await LoadContractorsByBuildingAsync();
            txtUsageAmount.Text = _currentElectricBilling.UsageAmount.ToString();
            dtpStartDate.Value = _currentElectricBilling.StartDate;
            dtpEndDate.Value = _currentElectricBilling.EndDate;
            txtBasicCharge.Text = _currentElectricBilling.BasicCharge.ToString();
            txtPowerCharge.Text = _currentElectricBilling.PowerCharge.ToString();
            txtTaxRate.Text = _currentElectricBilling.TaxRate.ToString();
            txtCustomerNumber.Text = _currentElectricBilling.CustomerNumber;
            if (_currentElectricBilling.ContractorId.HasValue)
            {
                var contractorId = _currentElectricBilling.ContractorId.Value;
                if (cmbContractor.Items.Cast<object>()
                    .Select(i => i as ContractorComboItem)
                    .Any(i => i != null && i.Id == contractorId))
                {
                    cmbContractor.SelectedValue = contractorId;
                }
                else
                {
                    cmbContractor.SelectedIndex = 0;
                }
            }
            else
            {
                cmbContractor.SelectedIndex = 0;
            }
        }

        private async void CmbBuildingName_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!_isEditMode)
            {
                await LoadContractorsByBuildingAsync();
            }
        }

        private async Task LoadContractorsByBuildingAsync()
        {
            var items = new List<ContractorComboItem>
            {
                new ContractorComboItem { Id = null, Name = string.Empty }
            };

            var allClients = await ClientDataAccess.GetAllClientsAsync();
            var contractors = allClients.Where(c => c.IsContractor).ToList();
            items.AddRange(contractors
                .OrderBy(c => c.Name)
                .Select(c => new ContractorComboItem
                {
                    Id = c.Id,
                    Name = c.Name
                }));

            int? selectedValue = null;
            if (cmbContractor.SelectedValue is int selectedId)
            {
                selectedValue = selectedId;
            }
            cmbContractor.DataSource = items;
            cmbContractor.DisplayMember = nameof(ContractorComboItem.Name);
            cmbContractor.ValueMember = nameof(ContractorComboItem.Id);

            if (selectedValue.HasValue && items.Any(x => x.Id == selectedValue.Value))
            {
                cmbContractor.SelectedValue = selectedValue.Value;
            }
            else
            {
                cmbContractor.SelectedIndex = 0;
            }
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

            int? contractorId = cmbContractor.SelectedValue is int id ? id : (int?)null;

            _currentElectricBilling.BillingYearMonth = txtBillingYearMonth.Text;
            _currentElectricBilling.BuildingName = cmbBuildingName.SelectedValue.ToString() ?? string.Empty;
            _currentElectricBilling.UsageAmount = usageAmount;
            _currentElectricBilling.StartDate = dtpStartDate.Value;
            _currentElectricBilling.EndDate = dtpEndDate.Value;
            _currentElectricBilling.BasicCharge = basicCharge;
            _currentElectricBilling.PowerCharge = powerCharge;
            _currentElectricBilling.TaxRate = string.IsNullOrWhiteSpace(txtTaxRate.Text) ? 0 : decimal.Parse(txtTaxRate.Text);
            _currentElectricBilling.CustomerNumber = txtCustomerNumber.Text;
            _currentElectricBilling.ContractorId = contractorId;

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


