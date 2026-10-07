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
    /// メーター情報登録・編集フォーム
    /// </summary>
    public partial class MeterForm : Form
    {
        private Meter _currentMeter;
        private bool _isEditMode;
        private bool _isCopyMode;

        public MeterForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _currentMeter = new Meter();
            this.Load += MeterForm_Load;
        }

        public MeterForm(Meter meter) : this()
        {
            _currentMeter = meter;
            _isEditMode = true;
        }

        /// <summary>
        /// 既存の親メーター情報をコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        /// <param name="meter">コピー元の親メーター情報</param>
        /// <param name="isCopyMode">コピーモード</param>
        public MeterForm(Meter meter, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentMeter = meter;
                _isEditMode = false;
                _isCopyMode = true;
                this.Text = "メーター情報登録";
            }
        }

        private async void MeterForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingNamesAsync();
            await LoadContractorsAsync();
            
            if (_isEditMode)
            {
                LoadMeterData();
            }
            else if (_isCopyMode)
            {
                LoadMeterDataForCopy();
            }
        }

        private async Task LoadBuildingNamesAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                cmbBuilding.DisplayMember = "Name";
                cmbBuilding.ValueMember = "Id";
                cmbBuilding.DataSource = buildings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ビル名の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadContractorsAsync()
        {
            try
            {
                var clients = await ClientDataAccess.GetAllClientsAsync();
                // 業者（IsContractor=true）のみをフィルタリング
                var contractors = clients.Where(c => c.IsContractor).ToList();
                
                cmbContractor.Items.Clear();
                cmbContractor.Items.Add(new { Id = (int?)null, Name = "" });
                
                foreach (var contractor in contractors)
                {
                    cmbContractor.Items.Add(new { Id = (int?)contractor.Id, Name = contractor.Name });
                }
                
                cmbContractor.DisplayMember = "Name";
                cmbContractor.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"業者の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "メーター情報編集" : "メーター情報登録";

            // メーター種別の選択肢を追加
            cmbMeterType.Items.AddRange(new string[] {
                "電気",
                "ガス",
                "水道"
            });

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        /// <summary>
        /// コピーモード用のデータ読み込み（ビル名・メーター種別・管理番号・業者をコピーし、親メーター名は空にする）
        /// </summary>
        private void LoadMeterDataForCopy()
        {
            if (_currentMeter == null) return;

            if (_currentMeter.BuildingId.HasValue)
            {
                cmbBuilding.SelectedValue = _currentMeter.BuildingId.Value;
            }
            cmbMeterType.Text = _currentMeter.MeterType ?? string.Empty;
            txtManagementNumber.Text = _currentMeter.ManagementNumber ?? string.Empty;

            if (cmbContractor.Items.Count > 0)
            {
                if (_currentMeter.ContractorId.HasValue)
                {
                    for (int i = 0; i < cmbContractor.Items.Count; i++)
                    {
                        var item = cmbContractor.Items[i];
                        var idProperty = item.GetType().GetProperty("Id");
                        if (idProperty != null)
                        {
                            var id = idProperty.GetValue(item) as int?;
                            if (id == _currentMeter.ContractorId.Value)
                            {
                                cmbContractor.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    cmbContractor.SelectedIndex = 0;
                }
            }

            txtMeterName.Text = string.Empty;
        }

        /// <summary>
        /// メーター情報をフォームに読み込む
        /// </summary>
        private void LoadMeterData()
        {
            if (_currentMeter != null)
            {
                if (_currentMeter.BuildingId.HasValue)
                {
                    cmbBuilding.SelectedValue = _currentMeter.BuildingId.Value;
                }
                cmbMeterType.Text = _currentMeter.MeterType;
                txtMeterName.Text = _currentMeter.MeterName;
                txtManagementNumber.Text = _currentMeter.ManagementNumber;
                
                // 業者選択（コンボボックスにアイテムが存在する場合のみ）
                if (cmbContractor.Items.Count > 0)
                {
                    if (_currentMeter.ContractorId.HasValue)
                    {
                        for (int i = 0; i < cmbContractor.Items.Count; i++)
                        {
                            var item = cmbContractor.Items[i];
                            var idProperty = item.GetType().GetProperty("Id");
                            if (idProperty != null)
                            {
                                var id = idProperty.GetValue(item) as int?;
                                if (id == _currentMeter.ContractorId.Value)
                                {
                                    cmbContractor.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        cmbContractor.SelectedIndex = 0;
                    }
                }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // バリデーション
                if (cmbBuilding.SelectedValue == null)
                {
                    MessageBox.Show("ビル名を選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(cmbMeterType.Text))
                {
                    MessageBox.Show("メーター種別を選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの設定
                if (_currentMeter == null)
                {
                    _currentMeter = new Meter();
                }

                _currentMeter.BuildingId = (int?)cmbBuilding.SelectedValue;
                _currentMeter.MeterId = string.Empty;
                _currentMeter.MeterType = cmbMeterType.Text.Trim();
                _currentMeter.MeterName = txtMeterName.Text.Trim();
                _currentMeter.ManagementNumber = txtManagementNumber.Text.Trim();
                
                // 業者IDを取得
                if (cmbContractor.SelectedItem != null)
                {
                    var contractorIdProperty = cmbContractor.SelectedItem.GetType().GetProperty("Id");
                    _currentMeter.ContractorId = contractorIdProperty?.GetValue(cmbContractor.SelectedItem) as int?;
                }
                else
                {
                    _currentMeter.ContractorId = null;
                }

                // 保存処理
                if (_isEditMode)
                {
                    var success = await MeterDataAccess.UpdateMeterAsync(_currentMeter);
                    if (success)
                    {
                        MessageBox.Show("メーター情報を更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("メーター情報の更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var id = await MeterDataAccess.CreateMeterAsync(_currentMeter);
                    if (id > 0)
                    {
                        MessageBox.Show("メーター情報を登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("メーター情報の登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

















