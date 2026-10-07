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
    /// 取引先情報管理フォーム
    /// </summary>
    public partial class ClientForm : Form
    {
        private Client _currentClient;
        private bool _isEditMode;
        private bool _isCopyMode;

        public ClientForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _isCopyMode = false;
        }

        public ClientForm(Client client) : this()
        {
            _currentClient = client;
            _isEditMode = true;
            _isCopyMode = false;
            // LoadClientDataはInitializeComponentAdditionalの後に呼ばれるため、ここでは呼ばない
            // InitializeComponentAdditional内でLoadBuildingsAsyncが呼ばれ、そこで既存の値を設定する
        }

        /// <summary>
        /// 既存の取引先データをコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        /// <param name="client">コピー元の取引先データ</param>
        /// <param name="isCopyMode">コピーモード</param>
        public ClientForm(Client client, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentClient = client;
                _isEditMode = false;
                _isCopyMode = true;
                this.Text = "取引先情報登録";
            }
        }

        private async void InitializeComponentAdditional()
        {
            // タイトルを設定（実行時に決定）
            if (_isEditMode)
            {
                this.Text = "取引先情報編集";
            }
            else
            {
                this.Text = "取引先情報登録";
            }

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            chkIsContractor.CheckedChanged += ChkIsContractor_CheckedChanged;
            chkIsBillingTo.CheckedChanged += ChkIsBillingTo_CheckedChanged;
            chkIsAutoTransfer.CheckedChanged += ChkIsAutoTransfer_CheckedChanged;
            chkIsBankTransfer.CheckedChanged += ChkIsBankTransfer_CheckedChanged;
            cmbBuildingName.SelectedValueChanged += CmbBuildingName_SelectedValueChanged;
            
            // ビル名と部屋名のComboBoxを初期化
            await LoadBuildingsAsync();

            if (_isCopyMode && _currentClient != null)
            {
                LoadClientDataForCopy();
            }
            
            // 初期状態でインボイス番号フィールドの表示/非表示を設定
            UpdateInvoiceNumberVisibility();
            EnsureDefaultTransferSelection();
            UpdateTransferOptionsState();
        }
        
        /// <summary>
        /// ビル一覧を読み込む
        /// </summary>
        private async Task LoadBuildingsAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var buildingList = buildings.Select(b => new
                {
                    Id = b.Id,
                    DisplayText = b.Name
                }).ToList();

                // 空白選択肢を先頭に追加
                buildingList.Insert(0, new { Id = 0, DisplayText = "" });
                
                // イベントハンドラーを一時的に無効化
                cmbBuildingName.SelectedValueChanged -= CmbBuildingName_SelectedValueChanged;
                
                cmbBuildingName.DisplayMember = "DisplayText";
                cmbBuildingName.ValueMember = "Id";
                cmbBuildingName.DataSource = buildingList;
                
                // イベントハンドラーを再度有効化
                cmbBuildingName.SelectedValueChanged += CmbBuildingName_SelectedValueChanged;
                
                // 編集モードの場合は既存の値を設定
                if ((_isEditMode || _isCopyMode) && _currentClient != null && !string.IsNullOrEmpty(_currentClient.BuildingName))
                {
                    var selectedBuilding = buildings.FirstOrDefault(b => b.Name == _currentClient.BuildingName);
                    if (selectedBuilding != null)
                    {
                        // イベントハンドラーを一時的に無効化して値を設定
                        cmbBuildingName.SelectedValueChanged -= CmbBuildingName_SelectedValueChanged;
                        cmbBuildingName.SelectedValue = selectedBuilding.Id;
                        cmbBuildingName.SelectedValueChanged += CmbBuildingName_SelectedValueChanged;
                        
                        await LoadRoomsByBuildingIdAsync(selectedBuilding.Id);
                        
                        // 部屋名も設定
                        if (!string.IsNullOrEmpty(_currentClient.RoomName))
                        {
                            var floors = await FloorDataAccess.GetFloorsByBuildingIdAsync(selectedBuilding.Id);
                            var selectedFloor = floors.FirstOrDefault(f => f.FloorName == _currentClient.RoomName);
                            if (selectedFloor != null)
                            {
                                cmbRoomName.SelectedValue = selectedFloor.Id;
                            }
                        }
                    }
                }
                else if ((_isEditMode || _isCopyMode) && _currentClient != null && string.IsNullOrEmpty(_currentClient.BuildingName))
                {
                    // 空白選択を設定
                    cmbBuildingName.SelectedValue = 0;
                    cmbRoomName.DataSource = null;
                    cmbRoomName.Enabled = false;
                }
                
                // 編集モードの場合は他のフィールドも設定
                if (_isEditMode && _currentClient != null)
                {
                    txtName.Text = _currentClient.Name;
                    chkIsLessor.Checked = _currentClient.IsLessor;
                    chkIsLessee.Checked = _currentClient.IsLessee;
                    chkIsBillingTo.Checked = _currentClient.IsBillingTo;
                    chkIsContractor.Checked = _currentClient.IsContractor;
                    chkIsAutoTransfer.Checked = _currentClient.IsAutoTransfer;
                    chkIsBankTransfer.Checked = _currentClient.IsBankTransfer;
                    txtInvoiceNumber.Text = _currentClient.InvoiceNumber;
                    txtPostalCode.Text = _currentClient.PostalCode;
                    txtAddress.Text = _currentClient.Address;
                    txtPhone.Text = _currentClient.Phone;
                    UpdateInvoiceNumberVisibility();
                    EnsureDefaultTransferSelection();
                    UpdateTransferOptionsState();
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
        private async void CmbBuildingName_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (cmbBuildingName.SelectedValue == null)
            {
                cmbRoomName.DataSource = null;
                cmbRoomName.Enabled = false;
                return;
            }
            
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
                return;
            }

            if (buildingId == 0)
            {
                cmbRoomName.DataSource = null;
                cmbRoomName.Enabled = false;
                return;
            }
            
            await LoadRoomsByBuildingIdAsync(buildingId);
        }
        
        /// <summary>
        /// ビルIDに紐づく部屋一覧を読み込む
        /// </summary>
        private async Task LoadRoomsByBuildingIdAsync(int buildingId)
        {
            try
            {
                var floors = await FloorDataAccess.GetFloorsByBuildingIdAsync(buildingId);
                var floorList = floors.Select(f => new
                {
                    Id = f.Id,
                    DisplayText = f.FloorName
                }).ToList();

                // 空白選択肢を先頭に追加
                floorList.Insert(0, new { Id = 0, DisplayText = "" });
                
                cmbRoomName.DisplayMember = "DisplayText";
                cmbRoomName.ValueMember = "Id";
                cmbRoomName.DataSource = floorList;
                cmbRoomName.Enabled = true;
                
                // 編集モードの場合は既存の値を設定
                if ((_isEditMode || _isCopyMode) && _currentClient != null && !string.IsNullOrEmpty(_currentClient.RoomName))
                {
                    var selectedFloor = floors.FirstOrDefault(f => f.FloorName == _currentClient.RoomName);
                    if (selectedFloor != null)
                    {
                        cmbRoomName.SelectedValue = selectedFloor.Id;
                    }
                }
                else if ((_isEditMode || _isCopyMode) && _currentClient != null && string.IsNullOrEmpty(_currentClient.RoomName))
                {
                    cmbRoomName.SelectedValue = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"部屋データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateInvoiceNumberVisibility()
        {
            bool isVisible = chkIsContractor.Checked;
            lblInvoiceNumber.Visible = isVisible;
            txtInvoiceNumber.Visible = isVisible;
            UpdateTransferMethodLayout(isVisible);
            
            // フォームの高さを調整
            if (isVisible)
            {
                this.ClientSize = new System.Drawing.Size(600, 400);
            }
            else
            {
                this.ClientSize = new System.Drawing.Size(600, 370);
            }
        }

        private void UpdateTransferMethodLayout(bool invoiceVisible)
        {
            var y = invoiceVisible ? 270 : 240;
            lblTransferMethod.Location = new Point(lblTransferMethod.Location.X, y);
            chkIsAutoTransfer.Location = new Point(chkIsAutoTransfer.Location.X, y);
            chkIsBankTransfer.Location = new Point(chkIsBankTransfer.Location.X, y);
        }

        private void UpdateTransferOptionsState()
        {
            var enabled = chkIsBillingTo.Checked;
            chkIsAutoTransfer.Enabled = enabled;
            chkIsBankTransfer.Enabled = enabled;
        }

        private void EnsureDefaultTransferSelection()
        {
            if (!chkIsAutoTransfer.Checked && !chkIsBankTransfer.Checked)
            {
                chkIsAutoTransfer.Checked = true;
            }
        }
        
        private void ChkIsContractor_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateInvoiceNumberVisibility();
        }

        private void ChkIsBillingTo_CheckedChanged(object? sender, EventArgs e)
        {
            if (chkIsBillingTo.Checked)
            {
                EnsureDefaultTransferSelection();
            }
            UpdateTransferOptionsState();
        }

        private void ChkIsAutoTransfer_CheckedChanged(object? sender, EventArgs e)
        {
            if (chkIsAutoTransfer.Checked)
            {
                chkIsBankTransfer.Checked = false;
            }
        }

        private void ChkIsBankTransfer_CheckedChanged(object? sender, EventArgs e)
        {
            if (chkIsBankTransfer.Checked)
            {
                chkIsAutoTransfer.Checked = false;
            }
        }

        /// <summary>
        /// コピーモード用のデータ読み込み（取引先名は空にする）
        /// </summary>
        private void LoadClientDataForCopy()
        {
            if (_currentClient == null) return;

            txtName.Text = _currentClient.Name;
            chkIsLessor.Checked = _currentClient.IsLessor;
            chkIsLessee.Checked = _currentClient.IsLessee;
            chkIsBillingTo.Checked = _currentClient.IsBillingTo;
            chkIsContractor.Checked = _currentClient.IsContractor;
            chkIsAutoTransfer.Checked = _currentClient.IsAutoTransfer;
            chkIsBankTransfer.Checked = _currentClient.IsBankTransfer;
            txtInvoiceNumber.Text = _currentClient.InvoiceNumber;
            txtPostalCode.Text = _currentClient.PostalCode;
            txtAddress.Text = _currentClient.Address;
            txtPhone.Text = _currentClient.Phone;
            UpdateInvoiceNumberVisibility();
            EnsureDefaultTransferSelection();
            UpdateTransferOptionsState();
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // データの設定
                if (_currentClient == null)
                {
                    _currentClient = new Client();
                }

                _currentClient.Name = txtName.Text.Trim();

                _currentClient.IsLessor = chkIsLessor.Checked;
                _currentClient.IsLessee = chkIsLessee.Checked;
                _currentClient.IsBillingTo = chkIsBillingTo.Checked;
                _currentClient.IsContractor = chkIsContractor.Checked;

                if (_currentClient.IsBillingTo && !chkIsAutoTransfer.Checked && !chkIsBankTransfer.Checked)
                {
                    MessageBox.Show("請求先の場合は「自動振込」または「口座振込」のどちらかを選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _currentClient.IsAutoTransfer = chkIsBillingTo.Checked && chkIsAutoTransfer.Checked;
                _currentClient.IsBankTransfer = chkIsBillingTo.Checked && chkIsBankTransfer.Checked;
                _currentClient.InvoiceNumber = chkIsContractor.Checked ? txtInvoiceNumber.Text.Trim() : string.Empty;
                
                // ビル名と部屋名をComboBoxから取得
                if (cmbBuildingName.SelectedItem != null)
                {
                    var buildingItem = cmbBuildingName.SelectedItem;
                    var displayTextProperty = buildingItem.GetType().GetProperty("DisplayText");
                    _currentClient.BuildingName = displayTextProperty?.GetValue(buildingItem)?.ToString() ?? string.Empty;
                }
                else
                {
                    _currentClient.BuildingName = string.Empty;
                }
                
                if (cmbRoomName.SelectedItem != null)
                {
                    var roomItem = cmbRoomName.SelectedItem;
                    var displayTextProperty = roomItem.GetType().GetProperty("DisplayText");
                    _currentClient.RoomName = displayTextProperty?.GetValue(roomItem)?.ToString() ?? string.Empty;
                }
                else
                {
                    _currentClient.RoomName = string.Empty;
                }
                
                _currentClient.PostalCode = txtPostalCode.Text.Trim();
                _currentClient.Address = txtAddress.Text.Trim();
                _currentClient.Phone = txtPhone.Text.Trim();

                // 保存処理
                if (_isEditMode)
                {
                    var success = await ClientDataAccess.UpdateClientAsync(_currentClient);
                    if (success)
                    {
                        MessageBox.Show("取引先情報を更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("取引先情報の更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var id = await ClientDataAccess.CreateClientAsync(_currentClient);
                    if (id > 0)
                    {
                        MessageBox.Show("取引先情報を登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("取引先情報の登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

