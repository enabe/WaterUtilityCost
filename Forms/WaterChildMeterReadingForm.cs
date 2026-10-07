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
    /// 水道子メータ検針データ登録フォーム
    /// </summary>
    public partial class WaterChildMeterReadingForm : Form
    {
        private ChildMeterReading? _currentReading;
        private bool _isEditMode;
        private const string METER_TYPE = "水道";

        public WaterChildMeterReadingForm()
        {
            InitializeComponent();
            _isEditMode = false;
            _currentReading = null;
            InitializeComponentAdditional();
            this.Load += WaterChildMeterReadingForm_Load;
        }

        public WaterChildMeterReadingForm(ChildMeterReading reading) : this()
        {
            _currentReading = reading;
            _isEditMode = true;
            this.Text = "水道子メータ検針データ編集";
        }

        /// <summary>
        /// 既存の検針データをコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        /// <param name="reading">コピー元の検針データ</param>
        /// <param name="isCopyMode">コピーモード（trueの場合、検針日を現在の日付に、メーター値を空にする）</param>
        public WaterChildMeterReadingForm(ChildMeterReading reading, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentReading = reading; // ビル名と部屋名をコピーするために保持
                _isEditMode = false; // 新規登録モード
                this.Text = "水道子メータ検針データ登録";
            }
        }

        private async void WaterChildMeterReadingForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();
            
            // メーター種別を「水道」に固定表示
            txtMeterType.Text = METER_TYPE;
            txtMeterType.ReadOnly = true;
            
            if (_isEditMode && _currentReading != null)
            {
                await LoadReadingData();
            }
            else if (_currentReading != null)
            {
                // コピーモード：ビル名と部屋名をコピーし、検針日は現在の日付、メーター値は空にする
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

                // ビル名を設定
                cmbBuildingName.SelectedValue = childMeter.BuildingId.Value;
                await LoadChildMetersByBuildingIdAsync(childMeter.BuildingId.Value);

                // 子メーターを設定
                cmbChildMeterName.SelectedValue = childMeter.Id;

                // その他のデータを設定
                dtpReadingDate.Value = _currentReading.ReadingDate;
                txtMeterValue.Text = _currentReading.MeterValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// コピーモード用のデータ読み込み（ビル名・子メーターをコピーし、検針日はコピー元を引き継ぎ、メーター値は空にする）
        /// </summary>
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
                        // ビル名を設定
                        cmbBuildingName.SelectedValue = childMeter.BuildingId.Value;
                        await LoadChildMetersByBuildingIdAsync(childMeter.BuildingId.Value);
                        // 子メーターを設定
                        cmbChildMeterName.SelectedValue = childMeter.Id;
                    }
                }

                // 検針日はコピー元を引き継ぐ
                dtpReadingDate.Value = _currentReading.ReadingDate;
                // メーター値は空にする
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

                // DataSourceを設定する前にクリア
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
            this.Text = _isEditMode ? "水道子メータ検針データ編集" : "水道子メータ検針データ登録";

            // イベントハンドラー
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

            await LoadChildMetersByBuildingIdAsync(buildingId);
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

                int childMeterId;
                if (cmbChildMeterName.SelectedValue is int)
                {
                    childMeterId = (int)cmbChildMeterName.SelectedValue;
                }
                else if (!int.TryParse(cmbChildMeterName.SelectedValue?.ToString(), out childMeterId))
                {
                    MessageBox.Show("子メーターIDの取得に失敗しました。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var roomChildMeter = await RoomChildMeterDataAccess.GetRoomChildMeterByChildMeterIdAsync(childMeterId);
                int floorId;
                if (roomChildMeter != null)
                {
                    floorId = roomChildMeter.FloorId;
                }
                else
                {
                    var floors = await FloorDataAccess.GetFloorsByBuildingIdAsync(buildingId);
                    var defaultFloor = floors.FirstOrDefault();
                    if (defaultFloor == null)
                    {
                        MessageBox.Show("選択されたビルに部屋が登録されていません。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    floorId = defaultFloor.Id;
                }

                if (_isEditMode && _currentReading != null)
                {
                    // 更新処理
                    _currentReading.FloorId = floorId;
                    _currentReading.ChildMeterId = childMeterId;
                    _currentReading.ReadingDate = dtpReadingDate.Value;
                    _currentReading.Type = METER_TYPE;
                    _currentReading.MeterValue = meterValue;

                    var success = await ChildMeterReadingDataAccess.UpdateChildMeterReadingAsync(_currentReading);
                    if (success)
                    {
                        MessageBox.Show("水道子メータ検針データを更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("水道子メータ検針データの更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // 新規登録処理
                    var reading = new ChildMeterReading
                    {
                        FloorId = floorId,
                        ChildMeterId = childMeterId,
                        ReadingDate = dtpReadingDate.Value,
                        Type = METER_TYPE,
                        MeterValue = meterValue
                    };

                    var id = await ChildMeterReadingDataAccess.CreateChildMeterReadingAsync(reading);
                    if (id > 0)
                    {
                        MessageBox.Show("水道子メータ検針データを登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("水道子メータ検針データの登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

