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
    /// 契約情報登録・編集フォーム
    /// </summary>
    public partial class ContractForm : Form
    {
        private Contract _currentContract;
        private bool _isEditMode;

        public ContractForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _currentContract = new Contract();
        }

        public ContractForm(Contract contract) : this()
        {
            _currentContract = contract;
            _isEditMode = true;
            LoadContractData();
        }

        private void InitializeComponentAdditional()
        {
            this.Text = _isEditMode ? "契約情報編集" : "契約情報登録";
            
            // イベントハンドラーの設定
            this.btnSave.Click += BtnSave_Click;
            this.btnCancel.Click += BtnCancel_Click;

            // 締日の選択肢を追加
            cmbClosingDate.Items.AddRange(Enumerable.Range(1, 31).Select(i => i.ToString()).ToArray());

            // データの読み込み
            LoadComboBoxData();
        }

        private async void LoadComboBoxData()
        {
            try
            {
                // 貸主取引先の読み込み
                var allClients = await ClientDataAccess.GetAllClientsAsync();
                var lessorClients = allClients.Where(c => c.IsLessor).ToList();
                cmbLessorClient.DisplayMember = "Name";
                cmbLessorClient.ValueMember = "Id";
                cmbLessorClient.DataSource = lessorClients;

                // 借主取引先の読み込み
                var lesseeClients = allClients.Where(c => c.IsLessee).ToList();
                cmbLesseeClient.DisplayMember = "Name";
                cmbLesseeClient.ValueMember = "Id";
                cmbLesseeClient.DataSource = lesseeClients;

                // 請求取引先の読み込み
                var billingClients = allClients.Where(c => c.IsBillingTo).ToList();
                cmbBillingClient.DisplayMember = "Name";
                cmbBillingClient.ValueMember = "Id";
                cmbBillingClient.DataSource = billingClients;

                // ビル名の読み込み
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                cmbBuilding.DisplayMember = "Name";
                cmbBuilding.ValueMember = "Id";
                cmbBuilding.DataSource = buildings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadContractData()
        {
            if (_currentContract != null)
            {
                txtContractNumber.Text = _currentContract.ContractNumber;
                txtContractType.Text = _currentContract.ContractType;
                txtContractorName.Text = _currentContract.ContractorName;

                if (_currentContract.LessorClientId.HasValue)
                {
                    cmbLessorClient.SelectedValue = _currentContract.LessorClientId.Value;
                }

                if (_currentContract.LesseeClientId.HasValue)
                {
                    cmbLesseeClient.SelectedValue = _currentContract.LesseeClientId.Value;
                }

                if (_currentContract.BillingClientId.HasValue)
                {
                    cmbBillingClient.SelectedValue = _currentContract.BillingClientId.Value;
                }

                if (_currentContract.StartDate.HasValue)
                {
                    dtpStartDate.Checked = true;
                    dtpStartDate.Value = _currentContract.StartDate.Value;
                }
                else
                {
                    dtpStartDate.Checked = false;
                }

                if (_currentContract.EndDate.HasValue)
                {
                    dtpEndDate.Checked = true;
                    dtpEndDate.Value = _currentContract.EndDate.Value;
                }
                else
                {
                    dtpEndDate.Checked = false;
                }

                if (!string.IsNullOrEmpty(_currentContract.ContractStatus))
                {
                    cmbContractStatus.SelectedItem = _currentContract.ContractStatus;
                }

                if (_currentContract.ClosingDate.HasValue)
                {
                    cmbClosingDate.SelectedItem = _currentContract.ClosingDate.Value.ToString();
                }

                if (_currentContract.BuildingId.HasValue)
                {
                    cmbBuilding.SelectedValue = _currentContract.BuildingId.Value;
                }

                txtCustomerNumber.Text = _currentContract.CustomerNumber;
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                _currentContract.ContractNumber = txtContractNumber.Text;
                _currentContract.ContractType = txtContractType.Text;
                _currentContract.ContractorName = txtContractorName.Text;

                _currentContract.LessorClientId = cmbLessorClient.SelectedValue != null ? (int?)cmbLessorClient.SelectedValue : null;
                _currentContract.LesseeClientId = cmbLesseeClient.SelectedValue != null ? (int?)cmbLesseeClient.SelectedValue : null;
                _currentContract.BillingClientId = cmbBillingClient.SelectedValue != null ? (int?)cmbBillingClient.SelectedValue : null;

                _currentContract.StartDate = dtpStartDate.Checked ? dtpStartDate.Value : null;
                _currentContract.EndDate = dtpEndDate.Checked ? dtpEndDate.Value : null;

                _currentContract.ContractStatus = cmbContractStatus.SelectedItem?.ToString() ?? string.Empty;
                _currentContract.ClosingDate = cmbClosingDate.SelectedItem != null ? int.Parse(cmbClosingDate.SelectedItem.ToString()!) : null;
                _currentContract.BuildingId = cmbBuilding.SelectedValue != null ? (int?)cmbBuilding.SelectedValue : null;

                _currentContract.CustomerNumber = txtCustomerNumber.Text;

                if (_isEditMode)
                {
                    await ContractDataAccess.UpdateContractAsync(_currentContract);
                    MessageBox.Show("契約情報を更新しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await ContractDataAccess.CreateContractAsync(_currentContract);
                    MessageBox.Show("契約情報を登録しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

