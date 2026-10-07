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
        private bool _isCopyMode;

        /// <summary>
        /// 子メーター情報登録フォームのコンストラクタ（新規登録用）
        /// </summary>
        public ChildMeterForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _isCopyMode = false;
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
            _isCopyMode = false;
        }

        /// <summary>
        /// 既存の子メーター情報をコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        /// <param name="childMeter">コピー元の子メーター情報</param>
        /// <param name="isCopyMode">コピーモード</param>
        public ChildMeterForm(ChildMeter childMeter, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentChildMeter = childMeter;
                _isEditMode = false;
                _isCopyMode = true;
                this.Text = "子メーター情報登録";
            }
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
            else if (_isCopyMode)
            {
                await LoadChildMeterDataForCopy();
            }
            else
            {
                // 新規登録時にもビル名とメーター種別が選択されていれば親メーターを読み込む
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
            await LoadParentMetersAsync();
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
                    cmbParentMeterName.DataSource = null;
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
                cmbParentMeterName.SelectedIndexChanged -= CmbParentMeterName_SelectedIndexChanged;
                
                cmbParentMeterName.DisplayMember = "DisplayText";
                cmbParentMeterName.ValueMember = "Id";
                cmbParentMeterName.DataSource = meterList;
                
                // イベントハンドラーを再度有効化
                cmbParentMeterName.SelectedIndexChanged += CmbParentMeterName_SelectedIndexChanged;
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
        private void CmbParentMeterName_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // 特に処理なし
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
                
                // イベントハンドラーを再度有効化
                cmbMeterType.SelectedIndexChanged += CmbMeterType_SelectedIndexChanged;
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
            cmbParentMeterName.SelectedIndexChanged += CmbParentMeterName_SelectedIndexChanged;
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
                    await LoadParentMetersAsync();
                }
                if (!string.IsNullOrEmpty(_currentChildMeter.MeterType))
                {
                    cmbMeterType.SelectedItem = _currentChildMeter.MeterType;
                    await LoadParentMetersAsync();
                }
                if (_currentChildMeter.ParentMeterId.HasValue)
                {
                    cmbParentMeterName.SelectedValue = _currentChildMeter.ParentMeterId.Value;
                }
                txtChildMeterName.Text = _currentChildMeter.MeterName;
            }
        }

        /// <summary>
        /// コピーモード用のデータ読み込み（ビル名・メーター種別・親メーターをコピーし、子メーター名は空にする）
        /// </summary>
        private async Task LoadChildMeterDataForCopy()
        {
            if (_currentChildMeter == null) return;

            if (_currentChildMeter.BuildingId.HasValue)
            {
                cmbBuildingName.SelectedValue = _currentChildMeter.BuildingId.Value;
                await LoadParentMetersAsync();
            }
            if (!string.IsNullOrEmpty(_currentChildMeter.MeterType))
            {
                cmbMeterType.SelectedItem = _currentChildMeter.MeterType;
                await LoadParentMetersAsync();
            }
            if (_currentChildMeter.ParentMeterId.HasValue)
            {
                cmbParentMeterName.SelectedValue = _currentChildMeter.ParentMeterId.Value;
            }

            // 子メーター名は新規入力とする
            txtChildMeterName.Text = string.Empty;
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
                if (cmbMeterType.SelectedItem == null)
                {
                    MessageBox.Show("メーター種別を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbParentMeterName.SelectedValue == null)
                {
                    MessageBox.Show("親メーター名を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの設定
                if (_currentChildMeter == null)
                {
                    _currentChildMeter = new ChildMeter();
                }

                _currentChildMeter.BuildingId = (int)cmbBuildingName.SelectedValue;
                _currentChildMeter.MeterType = cmbMeterType.SelectedItem.ToString() ?? string.Empty;
                _currentChildMeter.ParentMeterId = (int?)cmbParentMeterName.SelectedValue;
                _currentChildMeter.MeterName = txtChildMeterName.Text.Trim();

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

