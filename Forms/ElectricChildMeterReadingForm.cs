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
    /// 電気子メータ検針データ登録フォーム
    /// </summary>
    public partial class ElectricChildMeterReadingForm : Form
    {
        private int? _selectedBuildingId = null;
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

        private async void ElectricChildMeterReadingForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();
            
            // メーター種別を「電気」に固定表示
            txtMeterType.Text = METER_TYPE;
            txtMeterType.ReadOnly = true;
            
            if (_isEditMode && _currentReading != null)
            {
                await LoadReadingData();
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
                // フロア情報を取得してビルIDを取得
                var floor = await FloorDataAccess.GetFloorByIdAsync(_currentReading.FloorId);
                if (floor != null)
                {
                    // ビル名を設定
                    cmbBuildingName.SelectedValue = floor.BuildingId;
                    await LoadFloorsByBuildingIdAsync(floor.BuildingId);
                    
                    // フロア名を設定
                    cmbFloorName.SelectedValue = _currentReading.FloorId;
                }

                // その他のデータを設定
                dtpReadingDate.Value = _currentReading.ReadingDate;
                txtMeterValue.Text = _currentReading.MeterValue.ToString();
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
                // イベントハンドラーを一時的に無効化
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
                
                // イベントハンドラーを再度有効化
                cmbBuildingName.SelectedValueChanged += CmbBuildingName_SelectedValueChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ビル情報の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadFloorsByBuildingIdAsync(int buildingId)
        {
            try
            {
                var floors = await FloorDataAccess.GetFloorsByBuildingIdAsync(buildingId);
                
                var floorList = floors.Select(f => new
                {
                    Id = f.Id,
                    DisplayText = f.FloorName
                }).ToList();
                
                // DataSourceを設定する前にクリア
                cmbFloorName.DataSource = null;
                cmbFloorName.Items.Clear();
                
                if (floorList.Count > 0)
                {
                    cmbFloorName.DataSource = floorList;
                    cmbFloorName.DisplayMember = "DisplayText";
                    cmbFloorName.ValueMember = "Id";
                    cmbFloorName.Enabled = true;
                }
                else
                {
                    cmbFloorName.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"フロア情報の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "電気子メータ検針データ編集" : "電気子メータ検針データ登録";

            // イベントハンドラー
            cmbBuildingName.SelectedValueChanged += CmbBuildingName_SelectedValueChanged;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private async void CmbBuildingName_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (cmbBuildingName.SelectedValue == null)
            {
                cmbFloorName.DataSource = null;
                cmbFloorName.Enabled = false;
                return;
            }

            // SelectedValueを安全にint型に変換
            int buildingId;
            if (cmbBuildingName.SelectedValue is int)
            {
                buildingId = (int)cmbBuildingName.SelectedValue;
            }
            else if (int.TryParse(cmbBuildingName.SelectedValue.ToString(), out buildingId))
            {
                // 文字列から変換を試みる
            }
            else
            {
                return; // 変換できない場合は処理を中断
            }

            _selectedBuildingId = buildingId;
            await LoadFloorsByBuildingIdAsync(buildingId);
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // バリデーション
                if (cmbBuildingName.SelectedValue == null)
                {
                    MessageBox.Show("ビル名を選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbFloorName.SelectedValue == null)
                {
                    MessageBox.Show("フロア名を選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtMeterValue.Text, out decimal meterValue) || meterValue < 0)
                {
                    MessageBox.Show("メーター値を正しく入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // SelectedValueを安全にint型に変換
                int buildingId;
                if (cmbBuildingName.SelectedValue is int)
                {
                    buildingId = (int)cmbBuildingName.SelectedValue;
                }
                else if (!int.TryParse(cmbBuildingName.SelectedValue?.ToString(), out buildingId))
                {
                    MessageBox.Show("ビルIDの取得に失敗しました。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int floorId;
                if (cmbFloorName.SelectedValue is int)
                {
                    floorId = (int)cmbFloorName.SelectedValue;
                }
                else if (!int.TryParse(cmbFloorName.SelectedValue?.ToString(), out floorId))
                {
                    MessageBox.Show("フロアIDの取得に失敗しました。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ChildMeterテーブルから該当するIDを取得
                var childMeter = await ChildMeterDataAccess.GetChildMeterByBuildingRoomAndTypeAsync(buildingId, floorId, METER_TYPE);
                if (childMeter == null)
                {
                    MessageBox.Show($"指定されたビル名、部屋名、メーター種別（{METER_TYPE}）に一致する子メーターが見つかりませんでした。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_isEditMode && _currentReading != null)
                {
                    // 更新処理
                    _currentReading.FloorId = floorId;
                    _currentReading.ChildMeterId = childMeter.Id;
                    _currentReading.ReadingDate = dtpReadingDate.Value;
                    _currentReading.Type = METER_TYPE;
                    _currentReading.MeterValue = meterValue;

                    var success = await ChildMeterReadingDataAccess.UpdateChildMeterReadingAsync(_currentReading);
                    if (success)
                    {
                        MessageBox.Show("電気子メータ検針データを更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("電気子メータ検針データの更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // 新規登録処理
                    var reading = new ChildMeterReading
                    {
                        FloorId = floorId,
                        ChildMeterId = childMeter.Id,
                        ReadingDate = dtpReadingDate.Value,
                        Type = METER_TYPE,
                        MeterValue = meterValue
                    };

                    var id = await ChildMeterReadingDataAccess.CreateChildMeterReadingAsync(reading);
                    if (id > 0)
                    {
                        MessageBox.Show("電気子メータ検針データを登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
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
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

