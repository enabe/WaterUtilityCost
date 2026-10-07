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
        private bool _isCopyMode;

        /// <summary>
        /// 部屋別子メーター情報登録フォームのコンストラクタ（新規登録用）
        /// </summary>
        public RoomChildMeterForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _isCopyMode = false;
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
            _isCopyMode = false;
        }

        /// <summary>
        /// 既存の部屋別子メーター情報をコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        /// <param name="roomChildMeter">コピー元の部屋別子メーター情報</param>
        /// <param name="isCopyMode">コピーモード</param>
        public RoomChildMeterForm(RoomChildMeter roomChildMeter, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentRoomChildMeter = roomChildMeter;
                _isEditMode = false;
                _isCopyMode = true;
                this.Text = "部屋別子メーター情報登録";
            }
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
            if (_isEditMode)
            {
                await LoadRoomChildMeterData();
            }
            else if (_isCopyMode)
            {
                await LoadRoomChildMeterDataForCopy();
            }
            else
            {
                // 新規登録時にもメーター種別とビル名が選択されていれば親メーターを読み込む
                await LoadParentMetersAsync();
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
                    // メーター種別も選択されていれば親メーターを読み込む
                    await LoadParentMetersAsync();
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
            await LoadParentMetersAsync();
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
            await LoadChildMetersByParentMeterAsync();
        }

        /// <summary>
        /// 選択された親メーター名に紐づく子メーター一覧を読み込む
        /// </summary>
        private async Task LoadChildMetersByParentMeterAsync()
        {
            try
            {
                if (cmbBuildingName.SelectedValue == null || cmbMeterType.SelectedItem == null)
                {
                    cmbChildMeter.DataSource = null;
                    return;
                }

                var buildingId = (int)cmbBuildingName.SelectedValue;
                var meterType = cmbMeterType.SelectedItem.ToString() ?? string.Empty;

                int? parentMeterId = null;
                if (cmbParentMeter.SelectedValue is int parentId)
                {
                    parentMeterId = parentId;
                }

                var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
                
                // ビルIDとメーター種別でフィルタリングし、親メーターが選択されていればさらに絞り込む
                var filteredChildMeters = childMeters
                    .Where(cm => cm.BuildingId == buildingId && (cm.MeterType ?? "") == meterType)
                    .Where(cm => !parentMeterId.HasValue || (cm.ParentMeterId.HasValue && cm.ParentMeterId.Value == parentMeterId.Value))
                    .ToList();
                
                if (filteredChildMeters.Count == 0)
                {
                    cmbChildMeter.DataSource = new List<object>();
                    return;
                }
                
                var childMeterList = filteredChildMeters
                    .Select(cm =>
                    {
                        var displayText = string.IsNullOrEmpty(cm.MeterName) 
                            ? $"子メーターID: {cm.Id}" 
                            : cm.MeterName;
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
            await LoadParentMetersAsync();
        }

        /// <summary>
        /// 選択されたビル名とメーター種別に紐づく親メーター一覧を読み込む
        /// </summary>
        private async Task LoadParentMetersAsync()
        {
            try
            {
                // ビル名とメーター種別が選択されている場合のみ親メーターを表示
                if (cmbBuildingName.SelectedValue == null || cmbMeterType.SelectedItem == null)
                {
                    cmbParentMeter.DataSource = null;
                    return;
                }

                var buildingId = (int)cmbBuildingName.SelectedValue;
                var selectedMeterType = cmbMeterType.SelectedItem.ToString();
                var meters = await MeterDataAccess.GetAllMetersAsync();
                
                // 選択されたビル名とメーター種別が同じ親メーターをフィルタリング
                var filteredMeters = meters
                    .Where(m => 
                        m.BuildingId.HasValue && 
                        m.BuildingId.Value == buildingId &&
                        !string.IsNullOrEmpty(m.MeterType) &&
                        m.MeterType.Equals(selectedMeterType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                var meterList = filteredMeters
                    .Select(m => new
                    {
                        Id = m.Id,
                        DisplayText = string.IsNullOrEmpty(m.MeterName) ? $"親メーターID: {m.Id}" : m.MeterName
                    })
                    .ToList();
                
                // イベントハンドラーを一時的に無効化
                cmbParentMeter.SelectedIndexChanged -= CmbParentMeter_SelectedIndexChanged;
                
                cmbParentMeter.DisplayMember = "DisplayText";
                cmbParentMeter.ValueMember = "Id";
                cmbParentMeter.DataSource = meterList;
                
                // イベントハンドラーを再度有効化
                cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"親メーターデータの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 親メーター名選択変更時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void CmbParentMeter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await LoadChildMetersByParentMeterAsync();
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
            cmbBuildingName.SelectedIndexChanged += CmbBuildingName_SelectedIndexChanged;
            cmbRoomName.SelectedIndexChanged += CmbRoomName_SelectedIndexChanged;
            cmbMeterType.SelectedIndexChanged += CmbMeterType_SelectedIndexChanged;
            cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
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
                    
                    // メーター種別とビル名が設定された後に親メーターを読み込む
                    await LoadParentMetersAsync();
                    
                    // 親メーター名を設定（ChildMeterのParentMeterIdから親メーターを取得）
                    if (childMeter.ParentMeterId.HasValue)
                    {
                        // イベントハンドラーを一時的に無効化
                        cmbParentMeter.SelectedIndexChanged -= CmbParentMeter_SelectedIndexChanged;
                        cmbParentMeter.SelectedValue = childMeter.ParentMeterId.Value;
                        // イベントハンドラーを再度有効化
                        cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
                        
                        // 親メーター名が設定された後に子メーターを読み込む
                        await LoadChildMetersByParentMeterAsync();
                    }
                    
                    // 部屋名を設定（RoomChildMeterのFloorIdから取得）
                    if (_currentRoomChildMeter.FloorId > 0)
                    {
                        // イベントハンドラーを一時的に無効化
                        cmbRoomName.SelectedIndexChanged -= CmbRoomName_SelectedIndexChanged;
                        cmbRoomName.SelectedValue = _currentRoomChildMeter.FloorId;
                        // イベントハンドラーを再度有効化
                        cmbRoomName.SelectedIndexChanged += CmbRoomName_SelectedIndexChanged;
                    }
                    
                    // 子メーターを設定
                    if (cmbChildMeter.Items.Count > 0)
                    {
                        cmbChildMeter.SelectedValue = _currentRoomChildMeter.ChildMeterId;
                    }
                }
            }
        }

        /// <summary>
        /// コピーモード用のデータ読み込み（ビル名・部屋名・メーター種別・親メーターをコピーし、子メーターは未選択）
        /// </summary>
        private async Task LoadRoomChildMeterDataForCopy()
        {
            if (_currentRoomChildMeter == null) return;

            // ChildMeterから情報を取得
            var childMeter = await ChildMeterDataAccess.GetChildMeterByIdAsync(_currentRoomChildMeter.ChildMeterId);
            if (childMeter == null) return;

            // ビル名を設定
            if (childMeter.BuildingId.HasValue)
            {
                cmbBuildingName.SelectedIndexChanged -= CmbBuildingName_SelectedIndexChanged;
                cmbBuildingName.SelectedValue = childMeter.BuildingId.Value;
                cmbBuildingName.SelectedIndexChanged += CmbBuildingName_SelectedIndexChanged;
                await LoadRoomsByBuildingAsync();
            }

            // メーター種別を設定
            if (!string.IsNullOrEmpty(childMeter.MeterType))
            {
                cmbMeterType.SelectedIndexChanged -= CmbMeterType_SelectedIndexChanged;
                cmbMeterType.SelectedItem = childMeter.MeterType;
                cmbMeterType.SelectedIndexChanged += CmbMeterType_SelectedIndexChanged;
            }

            // メーター種別とビル名が設定された後に親メーターを読み込む
            await LoadParentMetersAsync();

            // 親メーター名を設定
            if (childMeter.ParentMeterId.HasValue)
            {
                cmbParentMeter.SelectedIndexChanged -= CmbParentMeter_SelectedIndexChanged;
                cmbParentMeter.SelectedValue = childMeter.ParentMeterId.Value;
                cmbParentMeter.SelectedIndexChanged += CmbParentMeter_SelectedIndexChanged;
                await LoadChildMetersByParentMeterAsync();
            }

            // 部屋名を設定
            if (_currentRoomChildMeter.FloorId > 0)
            {
                cmbRoomName.SelectedIndexChanged -= CmbRoomName_SelectedIndexChanged;
                cmbRoomName.SelectedValue = _currentRoomChildMeter.FloorId;
                cmbRoomName.SelectedIndexChanged += CmbRoomName_SelectedIndexChanged;
            }

            // 子メーターは未選択にする（新規入力）
            cmbChildMeter.SelectedIndex = -1;
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
                    MessageBox.Show("親メーター名を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbChildMeter.SelectedValue == null)
                {
                    MessageBox.Show("子メーター名を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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


