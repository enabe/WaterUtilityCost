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
    /// その他請求明細登録・編集フォーム
    /// </summary>
    public partial class OtherInvoiceDetailEditForm : Form
    {
        private InvoiceDetail _currentInvoiceDetail;
        private bool _isEditMode;

        public OtherInvoiceDetailEditForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
        }

        public OtherInvoiceDetailEditForm(InvoiceDetail invoiceDetail) : this()
        {
            _currentInvoiceDetail = invoiceDetail;
            _isEditMode = true;
            // LoadInvoiceDetailDataはInitializeComponentAdditionalで呼ばれる
        }

        /// <summary>
        /// コピー用のコンストラクタ（新規登録モードでデータを設定）
        /// </summary>
        public OtherInvoiceDetailEditForm(InvoiceDetail invoiceDetail, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                // コピーモードの場合は新規登録として扱う
                _isEditMode = false;
                // データをコピー（IDは新しいものになるため除外）
                _currentInvoiceDetail = new InvoiceDetail
                {
                    BillingTo = invoiceDetail.BillingTo,
                    Lessor = invoiceDetail.Lessor,
                    BuildingName = invoiceDetail.BuildingName,
                    Lessee = invoiceDetail.Lessee,
                    RoomNumber = invoiceDetail.RoomNumber,
                    Category = invoiceDetail.Category,
                    Content = invoiceDetail.Content,
                    TaxInclusiveAmount = invoiceDetail.TaxInclusiveAmount,
                    TaxRate = invoiceDetail.TaxRate,
                    Contractor = invoiceDetail.Contractor,
                    InvoiceNumber = invoiceDetail.InvoiceNumber,
                    ConfirmedBillingDate = invoiceDetail.ConfirmedBillingDate
                };
                // LoadInvoiceDetailDataはInitializeComponentAdditionalで呼ばれる
            }
            else
            {
                // 通常の編集モード
                _currentInvoiceDetail = invoiceDetail;
                _isEditMode = true;
                // LoadInvoiceDetailDataはInitializeComponentAdditionalで呼ばれる
            }
        }

        private async void InitializeComponentAdditional()
        {
            // タイトルを設定（実行時に決定）
            if (_isEditMode)
            {
                this.Text = "その他請求明細編集";
            }
            else
            {
                this.Text = "その他請求明細登録";
            }

            // 業者ComboBoxの初期化
            await LoadContractorsAsync();

            // 業者選択時のイベントハンドラー
            cmbContractor.SelectedIndexChanged += CmbContractor_SelectedIndexChanged;

            // データを読み込む（編集モードまたはコピーモードの場合）
            if (_currentInvoiceDetail != null)
            {
                LoadInvoiceDetailData();
            }

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private async Task LoadContractorsAsync()
        {
            try
            {
                var clients = await ClientDataAccess.GetAllClientsAsync();
                var contractors = clients.Where(c => c.IsContractor).ToList();

                cmbContractor.Items.Clear();
                cmbContractor.Items.Add(new { Id = (int?)null, Name = "", InvoiceNumber = "" }); // 空の選択肢
                foreach (var contractor in contractors)
                {
                    cmbContractor.Items.Add(new { Id = (int?)contractor.Id, Name = contractor.Name, InvoiceNumber = contractor.InvoiceNumber ?? "" });
                }
                cmbContractor.DisplayMember = "Name";
                cmbContractor.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"業者の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbContractor_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbContractor.SelectedItem != null)
            {
                var selectedItem = cmbContractor.SelectedItem;
                var invoiceNumberProperty = selectedItem.GetType().GetProperty("InvoiceNumber");
                if (invoiceNumberProperty != null)
                {
                    var invoiceNumber = invoiceNumberProperty.GetValue(selectedItem)?.ToString() ?? "";
                    txtInvoiceNumber.Text = invoiceNumber;
                }
            }
        }

        private void LoadInvoiceDetailData()
        {
            if (_currentInvoiceDetail != null)
            {
                txtBillingTo.Text = _currentInvoiceDetail.BillingTo;
                txtLessor.Text = _currentInvoiceDetail.Lessor;
                txtBuildingName.Text = _currentInvoiceDetail.BuildingName;
                txtLessee.Text = _currentInvoiceDetail.Lessee;
                txtRoomNumber.Text = _currentInvoiceDetail.RoomNumber;
                txtCategory.Text = _currentInvoiceDetail.Category;
                txtContent.Text = _currentInvoiceDetail.Content;
                txtTaxInclusiveAmount.Text = _currentInvoiceDetail.TaxInclusiveAmount.ToString();
                txtTaxRate.Text = _currentInvoiceDetail.TaxRate.ToString();
                txtInvoiceNumber.Text = _currentInvoiceDetail.InvoiceNumber;
                dtpConfirmedBillingDate.Value = _currentInvoiceDetail.ConfirmedBillingDate ?? DateTime.Now;
                dtpConfirmedBillingDate.Checked = _currentInvoiceDetail.ConfirmedBillingDate.HasValue;

                // 業者ComboBoxの設定
                if (!string.IsNullOrEmpty(_currentInvoiceDetail.Contractor) && cmbContractor.Items.Count > 0)
                {
                    for (int i = 0; i < cmbContractor.Items.Count; i++)
                    {
                        var item = cmbContractor.Items[i];
                        var nameProperty = item.GetType().GetProperty("Name");
                        if (nameProperty != null)
                        {
                            var name = nameProperty.GetValue(item)?.ToString() ?? "";
                            if (name == _currentInvoiceDetail.Contractor)
                            {
                                cmbContractor.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else if (cmbContractor.Items.Count > 0)
                {
                    cmbContractor.SelectedIndex = 0; // 空を選択
                }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // バリデーション
                if (string.IsNullOrWhiteSpace(txtBillingTo.Text))
                {
                    MessageBox.Show("請求先を入力してください。", "バリデーションエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtBuildingName.Text))
                {
                    MessageBox.Show("建物名称を入力してください。", "バリデーションエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCategory.Text))
                {
                    MessageBox.Show("種別を入力してください。", "バリデーションエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtContent.Text))
                {
                    MessageBox.Show("内容を入力してください。", "バリデーションエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtTaxInclusiveAmount.Text, out decimal taxInclusiveAmount))
                {
                    MessageBox.Show("税込金額を正しく入力してください。", "バリデーションエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtTaxRate.Text, out decimal taxRate) || taxRate < 0)
                {
                    MessageBox.Show("税率を正しく入力してください。", "バリデーションエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの設定
                if (_currentInvoiceDetail == null)
                {
                    _currentInvoiceDetail = new InvoiceDetail();
                }

                _currentInvoiceDetail.BillingTo = txtBillingTo.Text.Trim();
                _currentInvoiceDetail.Lessor = txtLessor.Text.Trim();
                _currentInvoiceDetail.BuildingName = txtBuildingName.Text.Trim();
                _currentInvoiceDetail.Lessee = txtLessee.Text.Trim();
                _currentInvoiceDetail.RoomNumber = txtRoomNumber.Text.Trim();
                _currentInvoiceDetail.Category = txtCategory.Text.Trim();
                _currentInvoiceDetail.Content = txtContent.Text.Trim();
                _currentInvoiceDetail.TaxInclusiveAmount = taxInclusiveAmount;
                _currentInvoiceDetail.TaxRate = taxRate;
                _currentInvoiceDetail.InvoiceNumber = txtInvoiceNumber.Text.Trim();
                _currentInvoiceDetail.ConfirmedBillingDate = dtpConfirmedBillingDate.Checked ? dtpConfirmedBillingDate.Value : (DateTime?)null;

                // 業者名を取得
                if (cmbContractor.SelectedItem != null)
                {
                    var selectedItem = cmbContractor.SelectedItem;
                    var nameProperty = selectedItem.GetType().GetProperty("Name");
                    if (nameProperty != null)
                    {
                        _currentInvoiceDetail.Contractor = nameProperty.GetValue(selectedItem)?.ToString() ?? "";
                    }
                }

                // 保存処理
                if (_isEditMode)
                {
                    var success = await InvoiceDetailDataAccess.UpdateInvoiceDetailAsync(_currentInvoiceDetail);
                    if (success)
                    {
                        MessageBox.Show("その他請求明細を更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("その他請求明細の更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var id = await InvoiceDetailDataAccess.CreateInvoiceDetailAsync(_currentInvoiceDetail);
                    if (id > 0)
                    {
                        MessageBox.Show("その他請求明細を登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("その他請求明細の登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

