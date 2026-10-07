using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 電気子メータ検針データ登録フォーム
    /// </summary>
    public partial class ElectricChildMeterReadingForm : Form
    {
        private ChildMeterReading? _currentReading;
        private bool _isEditMode;
        private const string METER_TYPE = "電気";

        public ElectricChildMeterReadingForm()
        {
            InitializeComponent();
            _isEditMode = false;
            _currentReading = null;
            InitializeComponentAdditional();
            this.Load += ElectricChildMeterReadingForm_Load;
        }

        public ElectricChildMeterReadingForm(ChildMeterReading reading) : this()
        {
            _currentReading = reading;
            _isEditMode = true;
            this.Text = "電気子メータ検針データ編集";
        }

        /// <summary>
        /// 既存の検針データをコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        public ElectricChildMeterReadingForm(ChildMeterReading reading, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentReading = reading;
                _isEditMode = false;
                this.Text = "電気子メータ検針データ登録";
            }
        }

        private async void ElectricChildMeterReadingForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();

            txtMeterType.Text = METER_TYPE;
            txtMeterType.ReadOnly = true;

            if (_isEditMode && _currentReading != null)
            {
                await LoadReadingData();
            }
            else if (_currentReading != null)
            {
                await LoadReadingDataForCopy();
            }
            else
            {
                dtpReadingDate.Value = DateTime.Now;
            }
        }

        private async Task LoadReadingData()
        {
            if (_currentReading == null) return;

            try
            {
                if (_currentReading.ChildMeterId == null)
                {
                    MessageBox.Show("子メーター情報が見つかりませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var childMeter = await ChildMeterDataAccess.GetChildMeterByIdAsync(_currentReading.ChildMeterId.Value);
                if (childMeter == null || childMeter.BuildingId == null)
                {
                    MessageBox.Show("子メーター情報の取得に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                cmbBuildingName.SelectedValue = childMeter.BuildingId.Value;
                await LoadChildMetersByBuildingIdAsync(childMeter.BuildingId.Value);
                cmbChildMeterName.SelectedValue = childMeter.Id;

                dtpReadingDate.Value = _currentReading.ReadingDate;
                txtMeterValue.Text = _currentReading.MeterValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadReadingDataForCopy()
        {
            if (_currentReading == null) return;

            try
            {
                if (_currentReading.ChildMeterId != null)
                {
                    var childMeter = await ChildMeterDataAccess.GetChildMeterByIdAsync(_currentReading.ChildMeterId.Value);
                    if (childMeter != null && childMeter.BuildingId != null)
                    {
                        cmbBuildingName.SelectedValue = childMeter.BuildingId.Value;
                        await LoadChildMetersByBuildingIdAsync(childMeter.BuildingId.Value);
                        cmbChildMeterName.SelectedValue = childMeter.Id;
                    }
                }

                dtpReadingDate.Value = _currentReading.ReadingDate;
                txtMeterValue.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadBuildingsAsync()
        {
            try
            {
                cmbBuildingName.SelectedValueChanged -= CmbBuildingName_SelectedValueChanged;

                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var buildingList = buildings.Select(b => new
                {
                    Id = b.Id,
                    DisplayText = b.Name
                }).ToList();

                cmbBuildingName.DataSource = buildingList;
                cmbBuildingName.DisplayMember = "DisplayText";
                cmbBuildingName.ValueMember = "Id";

                cmbBuildingName.SelectedValueChanged += CmbBuildingName_SelectedValueChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ビル情報の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadChildMetersByBuildingIdAsync(int buildingId)
        {
            try
            {
                var childMeters = await ChildMeterDataAccess.GetChildMetersByBuildingAndTypeAsync(buildingId, METER_TYPE);

                var childMeterList = childMeters.Select(m => new
                {
                    Id = m.Id,
                    DisplayText = string.IsNullOrWhiteSpace(m.MeterName) ? $"ID:{m.Id}" : m.MeterName
                }).ToList();

                cmbChildMeterName.DataSource = null;
                cmbChildMeterName.Items.Clear();

                if (childMeterList.Count > 0)
                {
                    cmbChildMeterName.DataSource = childMeterList;
                    cmbChildMeterName.DisplayMember = "DisplayText";
                    cmbChildMeterName.ValueMember = "Id";
                    cmbChildMeterName.Enabled = true;
                }
                else
                {
                    cmbChildMeterName.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"子メーター情報の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "電気子メータ検針データ編集" : "電気子メータ検針データ登録";

            cmbBuildingName.SelectedValueChanged += CmbBuildingName_SelectedValueChanged;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private async void CmbBuildingName_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (cmbBuildingName.SelectedValue == null)
            {
                cmbChildMeterName.DataSource = null;
                cmbChildMeterName.Enabled = false;
                return;
            }

            int buildingId;
            if (cmbBuildingName.SelectedValue is int i)
                buildingId = i;
            else if (!int.TryParse(cmbBuildingName.SelectedValue.ToString(), out buildingId))
                return;

            await LoadChildMetersByBuildingIdAsync(buildingId);
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cmbBuildingName.SelectedValue == null)
                {
                    MessageBox.Show("ビル名を選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbChildMeterName.SelectedValue == null)
                {
                    MessageBox.Show("子メーターを選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtMeterValue.Text, out decimal meterValue) || meterValue < 0)
                {
                    MessageBox.Show("メーター値を正しく入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int childMeterId;
                if (cmbChildMeterName.SelectedValue is int ci)
                    childMeterId = ci;
                else if (!int.TryParse(cmbChildMeterName.SelectedValue?.ToString(), out childMeterId))
                {
                    MessageBox.Show("子メーターIDの取得に失敗しました。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var roomChildMeter = await RoomChildMeterDataAccess.GetRoomChildMeterByChildMeterIdAsync(childMeterId);
                if (roomChildMeter == null)
                {
                    MessageBox.Show("選択された子メーターに紐づく部屋が見つかりませんでした。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_isEditMode && _currentReading != null)
                {
                    _currentReading.FloorId = roomChildMeter.FloorId;
                    _currentReading.ChildMeterId = childMeterId;
                    _currentReading.ReadingDate = dtpReadingDate.Value;
                    _currentReading.Type = METER_TYPE;
                    _currentReading.MeterValue = meterValue;

                    var success = await ChildMeterReadingDataAccess.UpdateChildMeterReadingAsync(_currentReading);
                    if (success)
                    {
                        MessageBox.Show("電気子メータ検針データを更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("電気子メータ検針データの更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var reading = new ChildMeterReading
                    {
                        FloorId = roomChildMeter.FloorId,
                        ChildMeterId = childMeterId,
                        ReadingDate = dtpReadingDate.Value,
                        Type = METER_TYPE,
                        MeterValue = meterValue
                    };

                    var id = await ChildMeterReadingDataAccess.CreateChildMeterReadingAsync(reading);
                    if (id > 0)
                    {
                        MessageBox.Show("電気子メータ検針データを登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("電気子メータ検針データの登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
