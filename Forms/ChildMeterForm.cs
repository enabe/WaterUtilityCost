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
    /// 子メーター情報登録・編集フォーム
    /// </summary>
    public partial class ChildMeterForm : Form
    {
        private ChildMeter _currentChildMeter;
        private bool _isEditMode;

        /// <summary>
        /// 子メーター情報登録フォームのコンストラクタ（新規登録用）
        /// </summary>
        public ChildMeterForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _currentChildMeter = new ChildMeter();
            this.Load += ChildMeterForm_Load;
        }

        /// <summary>
        /// 子メーター情報編集フォームのコンストラクタ（編集用）
        /// </summary>
        /// <param name="childMeter">編集対象の子メーター情報</param>
        public ChildMeterForm(ChildMeter childMeter) : this()
        {
            _currentChildMeter = childMeter;
            _isEditMode = true;
        }

        /// <summary>
        /// フォーム読み込み時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void ChildMeterForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();
            LoadMeterTypes();
            if (_isEditMode)
            {
                await LoadChildMeterData();
            }
        }

        /// <summary>
        /// ビル一覧を読み込む
        /// </summary>
        private async Task LoadBuildingsAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var buildingList = buildings.Select(b => new { Id = b.Id, DisplayText = b.Name }).ToList();
                
                // イベントハンドラーを一時的に無効化（DataSource設定時のイベント発火を防ぐ）
                cmbBuildingName.SelectedIndexChanged -= CmbBuildingName_SelectedIndexChanged;
                
                cmbBuildingName.DisplayMember = "DisplayText";
                cmbBuildingName.ValueMember = "Id";
                cmbBuildingName.DataSource = buildingList;
                
                // イベントハンドラーを再度有効化
                cmbBuildingName.SelectedIndexChanged += CmbBuildingName_SelectedIndexChanged;
                
                // 新規登録時で、ビルが存在する場合は最初のビルの部屋を読み込む
                if (!_isEditMode && buildingList.Count > 0 && cmbBuildingName.SelectedValue != null)
                {
                    await LoadRoomsByBuildingAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ビルデータの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ビル名選択変更時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void CmbBuildingName_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await LoadRoomsByBuildingAsync();
        }

        /// <summary>
        /// 選択されたビルに紐づく部屋一覧を読み込む
        /// </summary>
        private async Task LoadRoomsByBuildingAsync()
        {
            try
            {
                if (cmbBuildingName.SelectedValue == null)
                {
                    cmbRoomName.DataSource = null;
                    return;
                }

                var buildingId = (int)cmbBuildingName.SelectedValue;
                var floors = await FloorDataAccess.GetFloorsByBuildingIdAsync(buildingId);
                var roomList = floors.Select(f => new { Id = f.Id, DisplayText = f.FloorName }).ToList();
                
                cmbRoomName.DisplayMember = "DisplayText";
                cmbRoomName.ValueMember = "Id";
                cmbRoomName.DataSource = roomList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"部屋データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// メーター種別一覧を読み込む
        /// </summary>
        private void LoadMeterTypes()
        {
            try
            {
                var meterTypes = new[] { "電気", "ガス", "水道" };
                cmbMeterType.DataSource = meterTypes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"メーター種別の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 追加のコンポーネント初期化処理
        /// </summary>
        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "子メーター情報編集" : "子メーター情報登録";

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        /// <summary>
        /// 子メーター情報をフォームに読み込む
        /// </summary>
        private async Task LoadChildMeterData()
        {
            if (_currentChildMeter != null)
            {
                if (_currentChildMeter.BuildingId.HasValue)
                {
                    cmbBuildingName.SelectedValue = _currentChildMeter.BuildingId.Value;
                    await LoadRoomsByBuildingAsync();
                    if (_currentChildMeter.RoomId.HasValue)
                    {
                        cmbRoomName.SelectedValue = _currentChildMeter.RoomId.Value;
                    }
                }
                if (!string.IsNullOrEmpty(_currentChildMeter.MeterType))
                {
                    cmbMeterType.SelectedItem = _currentChildMeter.MeterType;
                }
            }
        }

        /// <summary>
        /// 保存ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // バリデーション
                if (cmbBuildingName.SelectedValue == null)
                {
                    MessageBox.Show("ビル名を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbRoomName.SelectedValue == null)
                {
                    MessageBox.Show("部屋名を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbMeterType.SelectedItem == null)
                {
                    MessageBox.Show("メーター種別を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの設定
                if (_currentChildMeter == null)
                {
                    _currentChildMeter = new ChildMeter();
                }

                _currentChildMeter.BuildingId = (int)cmbBuildingName.SelectedValue;
                _currentChildMeter.RoomId = (int)cmbRoomName.SelectedValue;
                _currentChildMeter.MeterType = cmbMeterType.SelectedItem.ToString() ?? string.Empty;

                // 保存処理
                if (_isEditMode)
                {
                    var success = await ChildMeterDataAccess.UpdateChildMeterAsync(_currentChildMeter);
                    if (success)
                    {
                        MessageBox.Show("子メーター情報を更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("子メーター情報の更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var id = await ChildMeterDataAccess.CreateChildMeterAsync(_currentChildMeter);
                    if (id > 0)
                    {
                        MessageBox.Show("子メーター情報を登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("子メーター情報の登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// キャンセルボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

