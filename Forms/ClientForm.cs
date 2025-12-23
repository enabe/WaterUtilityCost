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

        public ClientForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
        }

        public ClientForm(Client client) : this()
        {
            _currentClient = client;
            _isEditMode = true;
            LoadClientData();
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
            
            // 初期状態でインボイス番号フィールドの表示/非表示を設定
            UpdateInvoiceNumberVisibility();
        }
        
        private void UpdateInvoiceNumberVisibility()
        {
            bool isVisible = chkIsContractor.Checked;
            lblInvoiceNumber.Visible = isVisible;
            txtInvoiceNumber.Visible = isVisible;
            
            // フォームの高さを調整
            if (isVisible)
            {
                this.ClientSize = new System.Drawing.Size(600, 310);
            }
            else
            {
                this.ClientSize = new System.Drawing.Size(600, 280);
            }
        }
        
        private void ChkIsContractor_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateInvoiceNumberVisibility();
        }

        private void LoadClientData()
        {
            if (_currentClient != null)
            {
                txtName.Text = _currentClient.Name;
                chkIsLessor.Checked = _currentClient.IsLessor;
                chkIsLessee.Checked = _currentClient.IsLessee;
                chkIsBillingTo.Checked = _currentClient.IsBillingTo;
                chkIsContractor.Checked = _currentClient.IsContractor;
                txtInvoiceNumber.Text = _currentClient.InvoiceNumber;
                txtPostalCode.Text = _currentClient.PostalCode;
                txtAddress.Text = _currentClient.Address;
                txtPhone.Text = _currentClient.Phone;
                
                // インボイス番号フィールドの表示/非表示を更新
                UpdateInvoiceNumberVisibility();
            }
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
                _currentClient.InvoiceNumber = chkIsContractor.Checked ? txtInvoiceNumber.Text.Trim() : string.Empty;
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

