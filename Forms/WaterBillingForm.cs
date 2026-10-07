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
    /// 水道料金請求データ登録・編集フォーム
    /// </summary>
    public partial class WaterBillingForm : Form
    {
        private sealed class ContractorComboItem
        {
            public int? Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private WaterBilling _currentWaterBilling;
        private bool _isEditMode;
        private bool _isCopyMode;
        private List<Meter> _parentMeters = new List<Meter>();
        private Dictionary<int, string> _buildingNameById = new Dictionary<int, string>();

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
            this.Text = "水道料金請求データ編集";
        }

        public WaterBillingForm(WaterBilling waterBilling, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentWaterBilling = waterBilling;
                _isEditMode = false;
                _isCopyMode = true;
                this.Text = "水道料金請求データ登録";
            }
        }

        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "水道料金請求データ編集" : "水道料金請求データ登録";

            btnSave.Click += BtnSave_Click;
            cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
            this.Load += WaterBillingForm_Load;
        }

        private async void WaterBillingForm_Load(object? sender, EventArgs e)
        {
            await LoadParentMetersAsync();
            if (_isEditMode)
            {
                await LoadWaterBillingDataAsync();
            }
            else if (_isCopyMode)
            {
                await LoadWaterBillingDataForCopyAsync();
            }
            else
            {
                cmbDifferenceAssignmentRoom.Items.Clear();
                cmbDifferenceAssignmentRoom.Items.Add("（なし）");
                cmbDifferenceAssignmentRoom.SelectedIndex = 0;
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
                    .Where(m => string.Equals(m.MeterType, "水道", StringComparison.OrdinalIgnoreCase))
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

                cmbParentMeter.SelectedIndexChanged -= CmbParentMeter_SelectedIndexChanged;
                cmbParentMeter.DataSource = items;
                cmbParentMeter.DisplayMember = "Display";
                cmbParentMeter.ValueMember = "Id";
                cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"親メーターの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CmbParentMeter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await LoadRoomNamesForSelectedParentMeterAsync();
            await LoadContractorsByBuildingAsync();
        }

        private string? GetSelectedBuildingName()
        {
            if (cmbParentMeter.SelectedValue is not int parentMeterId)
            {
                return null;
            }

            var parentMeter = _parentMeters.FirstOrDefault(m => m.Id == parentMeterId);
            if (parentMeter?.BuildingId.HasValue != true)
            {
                return null;
            }

            return _buildingNameById.TryGetValue(parentMeter.BuildingId.Value, out var buildingName)
                ? buildingName
                : null;
        }

        private async Task LoadRoomNamesForSelectedParentMeterAsync()
        {
            var buildingName = GetSelectedBuildingName();
            if (string.IsNullOrWhiteSpace(buildingName))
            {
                cmbDifferenceAssignmentRoom.Items.Clear();
                cmbDifferenceAssignmentRoom.Items.Add("（なし）");
                cmbDifferenceAssignmentRoom.SelectedIndex = 0;
                return;
            }

            try
            {
                var rooms = await GetRoomNamesForParentMeterAsync(buildingName);
                cmbDifferenceAssignmentRoom.Items.Clear();
                cmbDifferenceAssignmentRoom.Items.Add("（なし）");
                foreach (var room in rooms)
                {
                    cmbDifferenceAssignmentRoom.Items.Add(room);
                }
                cmbDifferenceAssignmentRoom.SelectedIndex = 0;
            }
            catch
            {
                cmbDifferenceAssignmentRoom.Items.Clear();
                cmbDifferenceAssignmentRoom.Items.Add("（なし）");
                cmbDifferenceAssignmentRoom.SelectedIndex = 0;
            }
        }

        private async Task<List<string>> GetRoomNamesForParentMeterAsync(string buildingName)
        {
            if (cmbParentMeter.SelectedValue is not int parentMeterId)
            {
                return await WaterBillingDataAccess.GetRoomNamesByBuildingNameAsync(buildingName);
            }

            var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
            var roomChildMeters = await RoomChildMeterDataAccess.GetAllRoomChildMetersAsync();
            var floors = await FloorDataAccess.GetFloorsByBuildingNameAsync(buildingName);
            var targetChildMeterIds = childMeters
                .Where(cm => cm.MeterType == "水道" && cm.ParentMeterId == parentMeterId)
                .Select(cm => cm.Id)
                .ToHashSet();
            var targetFloorIds = roomChildMeters
                .Where(rcm => targetChildMeterIds.Contains(rcm.ChildMeterId))
                .Select(rcm => rcm.FloorId)
                .ToHashSet();

            var clients = await ClientDataAccess.GetClientsByBuildingNameAndIsBillingToAsync(buildingName);
            return clients
                .Where(c =>
                {
                    var roomName = c.RoomName ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(roomName))
                    {
                        return false;
                    }

                    var floor = floors.FirstOrDefault(f => f.FloorName == roomName);
                    return floor != null && targetFloorIds.Contains(floor.Id);
                })
                .Select(c => c.RoomName ?? string.Empty)
                .Distinct()
                .OrderBy(r => r)
                .ToList();
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

        private async Task LoadWaterBillingDataForCopyAsync()
        {
            cmbParentMeter.SelectedIndexChanged -= CmbParentMeter_SelectedIndexChanged;
            try
            {
                if (_currentWaterBilling.ParentMeterId.HasValue)
                {
                    cmbParentMeter.SelectedValue = _currentWaterBilling.ParentMeterId.Value;
                }

                await LoadRoomNamesForSelectedParentMeterAsync();
                await LoadContractorsByBuildingAsync();

                var diffRoom = _currentWaterBilling.DifferenceAssignmentRoomName ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(diffRoom) && cmbDifferenceAssignmentRoom.Items.IndexOf(diffRoom) >= 0)
                {
                    cmbDifferenceAssignmentRoom.SelectedItem = diffRoom;
                }
                else
                {
                    cmbDifferenceAssignmentRoom.SelectedIndex = 0;
                }
            }
            finally
            {
                cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
            }

            if (_currentWaterBilling.ContractorId.HasValue)
            {
                var contractorId = _currentWaterBilling.ContractorId.Value;
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

            dtpStartDate.Value = _currentWaterBilling.StartDate;
            dtpEndDate.Value = _currentWaterBilling.EndDate;
            txtTaxRate.Text = _currentWaterBilling.TaxRate.ToString();
            txtCustomerNumber.Text = _currentWaterBilling.CustomerNumber ?? string.Empty;

            txtBillingYearMonth.Text = string.Empty;
            txtUsageAmount.Text = string.Empty;
            txtBasicCharge.Text = string.Empty;
            txtUsageCharge.Text = string.Empty;
        }

        private async Task LoadWaterBillingDataAsync()
        {
            txtBillingYearMonth.Text = _currentWaterBilling.BillingYearMonth;
            cmbParentMeter.SelectedIndexChanged -= CmbParentMeter_SelectedIndexChanged;
            try
            {
                if (_currentWaterBilling.ParentMeterId.HasValue)
                {
                    cmbParentMeter.SelectedValue = _currentWaterBilling.ParentMeterId.Value;
                }

                await LoadRoomNamesForSelectedParentMeterAsync();
                await LoadContractorsByBuildingAsync();

                var diffRoom = _currentWaterBilling.DifferenceAssignmentRoomName ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(diffRoom) && cmbDifferenceAssignmentRoom.Items.IndexOf(diffRoom) >= 0)
                {
                    cmbDifferenceAssignmentRoom.SelectedItem = diffRoom;
                }
                else
                {
                    cmbDifferenceAssignmentRoom.SelectedIndex = 0;
                }
            }
            finally
            {
                cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
            }

            if (_currentWaterBilling.ContractorId.HasValue)
            {
                var contractorId = _currentWaterBilling.ContractorId.Value;
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
            if (string.IsNullOrWhiteSpace(txtBillingYearMonth.Text))
            {
                MessageBox.Show("受領請求年月は必須です。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbParentMeter.SelectedValue == null)
            {
                MessageBox.Show("親メーターは必須です。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (!string.IsNullOrWhiteSpace(txtTaxRate.Text) && !decimal.TryParse(txtTaxRate.Text, out _))
            {
                MessageBox.Show("税率は数値で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpStartDate.Value >= dtpEndDate.Value)
            {
                MessageBox.Show("終了日は開始日より後の日付を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var parentMeterId = (int)cmbParentMeter.SelectedValue;
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

            _currentWaterBilling.BillingYearMonth = txtBillingYearMonth.Text;
            _currentWaterBilling.ParentMeterId = parentMeterId;
            _currentWaterBilling.BuildingName = buildingName;
            _currentWaterBilling.UsageAmount = usageAmount;
            _currentWaterBilling.StartDate = dtpStartDate.Value;
            _currentWaterBilling.EndDate = dtpEndDate.Value;
            _currentWaterBilling.BasicCharge = basicCharge;
            _currentWaterBilling.UsageCharge = usageCharge;
            _currentWaterBilling.TaxRate = string.IsNullOrWhiteSpace(txtTaxRate.Text) ? 0 : decimal.Parse(txtTaxRate.Text);
            _currentWaterBilling.CustomerNumber = txtCustomerNumber.Text ?? string.Empty;
            _currentWaterBilling.ContractorId = contractorId;
            var selectedRoom = cmbDifferenceAssignmentRoom.SelectedItem?.ToString();
            _currentWaterBilling.DifferenceAssignmentRoomName = (string.IsNullOrEmpty(selectedRoom) || selectedRoom == "（なし）") ? string.Empty : selectedRoom;

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
