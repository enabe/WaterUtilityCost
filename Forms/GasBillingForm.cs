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
        private sealed class ContractorComboItem
        {
            public int? Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private GasBilling _currentGasBilling;
        private bool _isEditMode;
        private bool _isCopyMode;
        private List<Meter> _parentMeters = new List<Meter>();
        private Dictionary<int, string> _buildingNameById = new Dictionary<int, string>();

        private ComboBox EnsureParentMeterCombo()
        {
            if (Controls["cmbParentMeter"] is ComboBox existingCombo)
            {
                return existingCombo;
            }

            var label = Controls["lblParentMeter"] as Label;
            if (label == null)
            {
                label = new Label
                {
                    Name = "lblParentMeter",
                    AutoSize = true,
                    Location = new Point(20, 55),
                    Text = "親メーター:*"
                };
                Controls.Add(label);
            }

            var combo = new ComboBox
            {
                Name = "cmbParentMeter",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(206, 52),
                Size = new Size(300, 33),
                TabIndex = 3
            };
            Controls.Add(combo);

            return combo;
        }


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

        /// <summary>
        /// 既存のガス料金請求データをコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        /// <param name="gasBilling">コピー元のガス料金請求データ</param>
        /// <param name="isCopyMode">コピーモード</param>
        public GasBillingForm(GasBilling gasBilling, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentGasBilling = gasBilling;
                _isEditMode = false;
                _isCopyMode = true;
                this.Text = "ガス料金請求情報登録";
            }
        }

        private async void GasBillingForm_Load(object? sender, EventArgs e)
        {
            await LoadParentMetersAsync();
            if (_isEditMode)
            {
                await LoadGasBillingDataAsync();
            }
            else if (_isCopyMode)
            {
                await LoadGasBillingDataForCopyAsync();
            }
        }

        private async Task LoadParentMetersAsync()
        {
            try
            {
                var meters = await MeterDataAccess.GetAllMetersAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                _buildingNameById = buildings.ToDictionary(b => b.Id, b => b.Name);

                _parentMeters = meters
                    .Where(m => string.Equals(m.MeterType, "ガス", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(m => string.IsNullOrWhiteSpace(m.MeterName) ? $"ID:{m.Id}" : m.MeterName, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(m => m.Id)
                    .ToList();

                var items = _parentMeters.Select(m =>
                {
                    var meterName = string.IsNullOrWhiteSpace(m.MeterName) ? $"ID:{m.Id}" : m.MeterName;
                    var buildingName = m.BuildingId.HasValue && _buildingNameById.TryGetValue(m.BuildingId.Value, out var name)
                        ? name
                        : string.Empty;
                    var display = string.IsNullOrWhiteSpace(buildingName)
                        ? meterName
                        : $"{meterName} ({buildingName})";
                    return new { m.Id, Display = display };
                }).ToList();

                var parentMeterCombo = EnsureParentMeterCombo();
                parentMeterCombo.SelectedIndexChanged -= ParentMeterCombo_SelectedIndexChanged;
                parentMeterCombo.DataSource = items;
                parentMeterCombo.DisplayMember = "Display";
                parentMeterCombo.ValueMember = "Id";
                parentMeterCombo.SelectedIndexChanged += ParentMeterCombo_SelectedIndexChanged;
                await LoadContractorsForGasBuildingAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"親メーターの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ParentMeterCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await LoadContractorsForGasBuildingAsync();
        }

        private async Task LoadContractorsForGasBuildingAsync()
        {
            var items = new List<ContractorComboItem>
            {
                new ContractorComboItem { Id = null, Name = string.Empty }
            };

            var allClients = await ClientDataAccess.GetAllClientsAsync();
            var contractors = allClients.Where(c => c.IsContractor).ToList();
            items.AddRange(contractors
                .OrderBy(c => c.Name)
                .Select(c => new ContractorComboItem { Id = c.Id, Name = c.Name }));

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

        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "ガス料金請求情報編集" : "ガス料金請求情報登録";

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            RemoveLegacyBuildingFloorControls();
        }

        private void RemoveLegacyBuildingFloorControls()
        {
            var legacyControls = new[]
            {
                "lblBuildingName",
                "cmbBuildingName",
                "lblFloorName",
                "cmbFloorName"
            };

            foreach (var name in legacyControls)
            {
                if (Controls[name] is Control control)
                {
                    Controls.Remove(control);
                    control.Dispose();
                }
            }
        }

        /// <summary>
        /// コピーモード用のデータ読み込み（親メーター・使用期間・税率・お客様番号・業者をコピーし、受領請求年月・使用量・料金は空にする）
        /// </summary>
        private async Task LoadGasBillingDataForCopyAsync()
        {
            var parentMeterCombo = EnsureParentMeterCombo();
            parentMeterCombo.SelectedIndexChanged -= ParentMeterCombo_SelectedIndexChanged;

            if (_currentGasBilling.ParentMeterId.HasValue)
            {
                parentMeterCombo.SelectedValue = _currentGasBilling.ParentMeterId.Value;
            }

            await LoadContractorsForGasBuildingAsync();

            if (_currentGasBilling.ContractorId.HasValue)
            {
                var contractorId = _currentGasBilling.ContractorId.Value;
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

            dtpStartDate.Value = _currentGasBilling.StartDate;
            dtpEndDate.Value = _currentGasBilling.EndDate;
            txtTaxRate.Text = _currentGasBilling.TaxRate.ToString();
            txtCustomerNumber.Text = _currentGasBilling.CustomerNumber ?? string.Empty;

            txtBillingYearMonth.Text = string.Empty;
            txtUsageAmount.Text = string.Empty;
            txtBasicCharge.Text = string.Empty;
            txtUsageCharge.Text = string.Empty;

            parentMeterCombo.SelectedIndexChanged += ParentMeterCombo_SelectedIndexChanged;
        }

        private async Task LoadGasBillingDataAsync()
        {
            txtBillingYearMonth.Text = _currentGasBilling.BillingYearMonth;
            if (_currentGasBilling.ParentMeterId.HasValue)
            {
                EnsureParentMeterCombo().SelectedValue = _currentGasBilling.ParentMeterId.Value;
            }

            await LoadContractorsForGasBuildingAsync();

            if (_currentGasBilling.ContractorId.HasValue)
            {
                var contractorId = _currentGasBilling.ContractorId.Value;
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
                EnsureParentMeterCombo().SelectedValue == null ||
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
            var parentMeterId = (int)EnsureParentMeterCombo().SelectedValue;
            var parentMeter = _parentMeters.FirstOrDefault(m => m.Id == parentMeterId);
            var buildingName = parentMeter?.BuildingId.HasValue == true
                && _buildingNameById.TryGetValue(parentMeter.BuildingId.Value, out var name)
                ? name
                : string.Empty;
            if (string.IsNullOrWhiteSpace(buildingName))
            {
                MessageBox.Show("選択した親メーターにビル情報が登録されていません。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? contractorId = cmbContractor.SelectedValue is int cid ? cid : (int?)null;

            _currentGasBilling.ParentMeterId = parentMeterId;
            _currentGasBilling.BuildingName = buildingName;
            _currentGasBilling.FloorName = string.Empty;
            _currentGasBilling.UsageAmount = usageAmount;
            _currentGasBilling.StartDate = dtpStartDate.Value;
            _currentGasBilling.EndDate = dtpEndDate.Value;
            _currentGasBilling.BasicCharge = basicCharge;
            _currentGasBilling.UsageCharge = usageCharge;
            _currentGasBilling.TaxRate = string.IsNullOrWhiteSpace(txtTaxRate.Text) ? 0 : decimal.Parse(txtTaxRate.Text);
            _currentGasBilling.CustomerNumber = txtCustomerNumber.Text;
            _currentGasBilling.ContractorId = contractorId;

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
