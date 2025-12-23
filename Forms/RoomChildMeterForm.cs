using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 部屋別子メーター情報登録・編集フォーム
    /// </summary>
    public partial class RoomChildMeterForm : Form
    {
        private RoomChildMeter _currentRoomChildMeter;
        private bool _isEditMode;

        /// <summary>
        /// 部屋別子メーター情報登録フォームのコンストラクタ（新規登録用）
        /// </summary>
        public RoomChildMeterForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _currentRoomChildMeter = new RoomChildMeter();
            this.Load += RoomChildMeterForm_Load;
        }

        /// <summary>
        /// 部屋別子メーター情報編集フォームのコンストラクタ（編集用）
        /// </summary>
        /// <param name="roomChildMeter">編集対象の部屋別子メーター情報</param>
        public RoomChildMeterForm(RoomChildMeter roomChildMeter) : this()
        {
            _currentRoomChildMeter = roomChildMeter;
            _isEditMode = true;
            LoadRoomChildMeterData();
        }

        /// <summary>
        /// フォーム読み込み時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void RoomChildMeterForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();
            LoadMeterTypes();
            await LoadParentMetersAsync();
            if (_isEditMode)
            {
                await LoadRoomChildMeterData();
            }
            else
            {
                // 新規登録時にもメーター種別とビル名が選択されていれば子メーターを読み込む
                await LoadChildMetersByMeterTypeAndBuildingAsync();
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
                
                // イベントハンドラーを一時的に無効化
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
                    // メーター種別も選択されていれば子メーターを読み込む
                    await LoadChildMetersByMeterTypeAndBuildingAsync();
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
            await LoadChildMetersByMeterTypeAndBuildingAsync();
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
                
                // イベントハンドラーを一時的に無効化
                cmbRoomName.SelectedIndexChanged -= CmbRoomName_SelectedIndexChanged;
                
                cmbRoomName.DisplayMember = "DisplayText";
                cmbRoomName.ValueMember = "Id";
                cmbRoomName.DataSource = roomList;
                
                // イベントハンドラーを再度有効化
                cmbRoomName.SelectedIndexChanged += CmbRoomName_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"部屋データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 部屋名選択変更時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void CmbRoomName_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // 部屋名選択時は特に処理しない（メーター種別とビル名で子メーターを表示）
        }

        /// <summary>
        /// 選択されたメーター種別とビル名に紐づく子メーター一覧を読み込む
        /// </summary>
        private async Task LoadChildMetersByMeterTypeAndBuildingAsync()
        {
            try
            {
                // メーター種別とビル名が選択されている場合のみ子メーターを表示
                if (cmbMeterType.SelectedItem == null || cmbBuildingName.SelectedValue == null)
                {
                    cmbChildMeter.DataSource = null;
                    return;
                }

                var selectedMeterType = cmbMeterType.SelectedItem.ToString();
                var buildingId = (int)cmbBuildingName.SelectedValue;
                var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
                var floors = await FloorDataAccess.GetAllFloorsAsync();
                
                // 選択されたメーター種別とビル名が同じ子メーターをフィルタリング
                var filteredChildMeters = childMeters
                    .Where(cm => 
                        cm.BuildingId.HasValue && 
                        cm.BuildingId.Value == buildingId &&
                        !string.IsNullOrEmpty(cm.MeterType) &&
                        cm.MeterType.Equals(selectedMeterType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                if (filteredChildMeters.Count == 0)
                {
                    cmbChildMeter.DataSource = new List<object>();
                    return;
                }
                
                var childMeterList = filteredChildMeters
                    .Select(cm =>
                    {
                        string displayText;
                        string roomName = string.Empty;
                        
                        if (cm.RoomId.HasValue)
                        {
                            var roomIdValue = cm.RoomId.Value;
                            // 部屋を検索
                            var floor = floors.FirstOrDefault(f => f.Id == roomIdValue);
                            
                            if (floor != null)
                            {
                                // 部屋名が空でない場合は部屋名を表示、空の場合は部屋IDを表示
                                if (!string.IsNullOrWhiteSpace(floor.FloorName))
                                {
                                    roomName = floor.FloorName;
                                }
                                else
                                {
                                    roomName = $"部屋ID: {roomIdValue}";
                                }
                            }
                            else
                            {
                                // 部屋が見つからない場合でも、部屋IDを表示
                                roomName = $"部屋ID: {roomIdValue}";
                            }
                        }
                        
                        // 部屋名のみを表示する
                        if (!string.IsNullOrEmpty(roomName))
                        {
                            displayText = roomName;
                        }
                        else
                        {
                            displayText = $"子メーターID: {cm.Id}";
                        }
                        
                        return new
                        {
                            Id = cm.Id,
                            DisplayText = displayText
                        };
                    })
                    .ToList();
                
                if (childMeterList.Count > 0)
                {
                    cmbChildMeter.DisplayMember = "DisplayText";
                    cmbChildMeter.ValueMember = "Id";
                    cmbChildMeter.DataSource = childMeterList;
                }
                else
                {
                    cmbChildMeter.DataSource = new List<object>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"子メーターデータの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                
                // イベントハンドラーを一時的に無効化
                cmbMeterType.SelectedIndexChanged -= CmbMeterType_SelectedIndexChanged;
                
                cmbMeterType.DataSource = meterTypes;
                
                // 新規登録時には最初のメーター種別を選択
                if (!_isEditMode && meterTypes.Length > 0)
                {
                    cmbMeterType.SelectedIndex = 0;
                }
                
                // イベントハンドラーを再度有効化
                cmbMeterType.SelectedIndexChanged += CmbMeterType_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"メーター種別の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// メーター種別選択変更時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void CmbMeterType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await LoadChildMetersByMeterTypeAndBuildingAsync();
        }

        /// <summary>
        /// 親メーター一覧を読み込む
        /// </summary>
        private async Task LoadParentMetersAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var buildingList = buildings.Select(b => new { Id = b.Id, DisplayText = b.Name }).ToList();
                
                // イベントハンドラーを一時的に無効化
                cmbParentMeter.SelectedIndexChanged -= CmbParentMeter_SelectedIndexChanged;
                
                cmbParentMeter.DisplayMember = "DisplayText";
                cmbParentMeter.ValueMember = "Id";
                cmbParentMeter.DataSource = buildingList;
                
                // イベントハンドラーを再度有効化
                cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"親メーターデータの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 親メーター選択変更時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void CmbParentMeter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // 親メーター選択時は特に処理しない（部屋名選択時に子メーターを表示）
        }

        /// <summary>
        /// 追加のコンポーネント初期化処理
        /// </summary>
        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "部屋別子メーター情報編集" : "部屋別子メーター情報登録";

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        /// <summary>
        /// 部屋別子メーター情報をフォームに読み込む
        /// </summary>
        private async Task LoadRoomChildMeterData()
        {
            if (_currentRoomChildMeter != null)
            {
                // ChildMeterから情報を取得
                var childMeter = await ChildMeterDataAccess.GetChildMeterByIdAsync(_currentRoomChildMeter.ChildMeterId);
                if (childMeter != null)
                {
                    // ビル名を設定
                    if (childMeter.BuildingId.HasValue)
                    {
                        // イベントハンドラーを一時的に無効化
                        cmbBuildingName.SelectedIndexChanged -= CmbBuildingName_SelectedIndexChanged;
                        cmbBuildingName.SelectedValue = childMeter.BuildingId.Value;
                        // イベントハンドラーを再度有効化
                        cmbBuildingName.SelectedIndexChanged += CmbBuildingName_SelectedIndexChanged;
                        await LoadRoomsByBuildingAsync();
                    }
                    
                    // メーター種別を設定
                    if (!string.IsNullOrEmpty(childMeter.MeterType))
                    {
                        // イベントハンドラーを一時的に無効化
                        cmbMeterType.SelectedIndexChanged -= CmbMeterType_SelectedIndexChanged;
                        cmbMeterType.SelectedItem = childMeter.MeterType;
                        // イベントハンドラーを再度有効化
                        cmbMeterType.SelectedIndexChanged += CmbMeterType_SelectedIndexChanged;
                    }
                    
                    // 親メーターを設定（ChildMeterのParentMeterIdから親メーターを取得し、そのビルIDを設定）
                    if (childMeter.ParentMeterId.HasValue)
                    {
                        var parentMeter = await MeterDataAccess.GetMeterByIdAsync(childMeter.ParentMeterId.Value);
                        if (parentMeter != null && parentMeter.BuildingId.HasValue)
                        {
                            // イベントハンドラーを一時的に無効化
                            cmbParentMeter.SelectedIndexChanged -= CmbParentMeter_SelectedIndexChanged;
                            cmbParentMeter.SelectedValue = parentMeter.BuildingId.Value;
                            // イベントハンドラーを再度有効化
                            cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
                        }
                    }
                    
                    // 部屋名を設定
                    if (childMeter.RoomId.HasValue)
                    {
                        // イベントハンドラーを一時的に無効化
                        cmbRoomName.SelectedIndexChanged -= CmbRoomName_SelectedIndexChanged;
                        cmbRoomName.SelectedValue = childMeter.RoomId.Value;
                        // イベントハンドラーを再度有効化
                        cmbRoomName.SelectedIndexChanged += CmbRoomName_SelectedIndexChanged;
                    }
                    
                    // メーター種別とビル名が設定された後に子メーターを読み込む
                    await LoadChildMetersByMeterTypeAndBuildingAsync();
                    
                    // 子メーターを設定
                    if (cmbChildMeter.Items.Count > 0)
                    {
                        cmbChildMeter.SelectedValue = _currentRoomChildMeter.ChildMeterId;
                    }
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
                if (cmbParentMeter.SelectedValue == null)
                {
                    MessageBox.Show("親メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbChildMeter.SelectedValue == null)
                {
                    MessageBox.Show("子メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの設定
                if (_currentRoomChildMeter == null)
                {
                    _currentRoomChildMeter = new RoomChildMeter();
                }

                _currentRoomChildMeter.FloorId = (int)cmbRoomName.SelectedValue;
                _currentRoomChildMeter.ChildMeterId = (int)cmbChildMeter.SelectedValue;

                // 保存処理
                if (_isEditMode)
                {
                    var success = await RoomChildMeterDataAccess.UpdateRoomChildMeterAsync(_currentRoomChildMeter);
                    if (success)
                    {
                        MessageBox.Show("部屋別子メーター情報を更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("部屋別子メーター情報の更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var id = await RoomChildMeterDataAccess.CreateRoomChildMeterAsync(_currentRoomChildMeter);
                    if (id > 0)
                    {
                        MessageBox.Show("部屋別子メーター情報を登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("部屋別子メーター情報の登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


