using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 請求明細登録・編集フォーム
    /// </summary>
    public partial class InvoiceDetailEditForm : Form
    {
        private InvoiceDetail _currentInvoiceDetail;
        private bool _isEditMode;

        public InvoiceDetailEditForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
        }

        public InvoiceDetailEditForm(InvoiceDetail invoiceDetail) : this()
        {
            _currentInvoiceDetail = invoiceDetail;
            _isEditMode = true;
            LoadInvoiceDetailData();
        }

        /// <summary>
        /// コピー用のコンストラクタ（新規登録モードでデータを設定）
        /// </summary>
        public InvoiceDetailEditForm(InvoiceDetail invoiceDetail, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                // コピーモードの場合は新規登録として扱う
                _isEditMode = false;
                this.Text = "請求明細登録（コピー）";
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
                    UsageAmount = invoiceDetail.UsageAmount,
                    Unit = invoiceDetail.Unit,
                    TaxInclusiveAmount = invoiceDetail.TaxInclusiveAmount,
                    TaxRate = invoiceDetail.TaxRate,
                    ChildMeterStartDate = invoiceDetail.ChildMeterStartDate,
                    ChildMeterEndDate = invoiceDetail.ChildMeterEndDate,
                    ParentMeterStartDate = invoiceDetail.ParentMeterStartDate,
                    ParentMeterEndDate = invoiceDetail.ParentMeterEndDate,
                    ConfirmedBillingDate = invoiceDetail.ConfirmedBillingDate
                };
                LoadInvoiceDetailData();
            }
            else
            {
                // 通常の編集モード
                _currentInvoiceDetail = invoiceDetail;
                _isEditMode = true;
                LoadInvoiceDetailData();
            }
        }

        private void InitializeComponentAdditional()
        {
            // タイトルを設定（実行時に決定）
            if (_isEditMode)
            {
                this.Text = "請求明細編集";
            }
            else
            {
                this.Text = "請求明細登録";
            }

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
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
                txtUsageAmount.Text = _currentInvoiceDetail.UsageAmount.ToString();
                txtUnit.Text = _currentInvoiceDetail.Unit;
                txtTaxInclusiveAmount.Text = _currentInvoiceDetail.TaxInclusiveAmount.ToString();
                txtTaxRate.Text = _currentInvoiceDetail.TaxRate.ToString();
                txtChildMeterStartDate.Text = _currentInvoiceDetail.ChildMeterStartDate?.ToString("yyyy-MM-dd") ?? "";
                txtChildMeterEndDate.Text = _currentInvoiceDetail.ChildMeterEndDate?.ToString("yyyy-MM-dd") ?? "";
                txtParentMeterStartDate.Text = _currentInvoiceDetail.ParentMeterStartDate?.ToString("yyyy-MM-dd") ?? "";
                txtParentMeterEndDate.Text = _currentInvoiceDetail.ParentMeterEndDate?.ToString("yyyy-MM-dd") ?? "";
                dtpConfirmedBillingDate.Value = _currentInvoiceDetail.ConfirmedBillingDate ?? DateTime.Now;
                dtpConfirmedBillingDate.Checked = _currentInvoiceDetail.ConfirmedBillingDate.HasValue;
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
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
                
                decimal.TryParse(txtUsageAmount.Text, out decimal usageAmount);
                _currentInvoiceDetail.UsageAmount = usageAmount;
                
                _currentInvoiceDetail.Unit = txtUnit.Text.Trim();
                
                decimal.TryParse(txtTaxInclusiveAmount.Text, out decimal taxInclusiveAmount);
                _currentInvoiceDetail.TaxInclusiveAmount = taxInclusiveAmount;
                
                decimal.TryParse(txtTaxRate.Text, out decimal taxRate);
                _currentInvoiceDetail.TaxRate = taxRate;

                DateTime? childMeterStartDate = null;
                if (DateTime.TryParse(txtChildMeterStartDate.Text, out DateTime csd))
                    childMeterStartDate = csd;
                _currentInvoiceDetail.ChildMeterStartDate = childMeterStartDate;

                DateTime? childMeterEndDate = null;
                if (DateTime.TryParse(txtChildMeterEndDate.Text, out DateTime ced))
                    childMeterEndDate = ced;
                _currentInvoiceDetail.ChildMeterEndDate = childMeterEndDate;

                DateTime? parentMeterStartDate = null;
                if (DateTime.TryParse(txtParentMeterStartDate.Text, out DateTime psd))
                    parentMeterStartDate = psd;
                _currentInvoiceDetail.ParentMeterStartDate = parentMeterStartDate;

                DateTime? parentMeterEndDate = null;
                if (DateTime.TryParse(txtParentMeterEndDate.Text, out DateTime ped))
                    parentMeterEndDate = ped;
                _currentInvoiceDetail.ParentMeterEndDate = parentMeterEndDate;

                _currentInvoiceDetail.ConfirmedBillingDate = dtpConfirmedBillingDate.Checked ? dtpConfirmedBillingDate.Value : (DateTime?)null;

                // 保存処理
                if (_isEditMode)
                {
                    var success = await InvoiceDetailDataAccess.UpdateInvoiceDetailAsync(_currentInvoiceDetail);
                    if (success)
                    {
                        MessageBox.Show("請求明細を更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("請求明細の更新に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var id = await InvoiceDetailDataAccess.CreateInvoiceDetailAsync(_currentInvoiceDetail);
                    if (id > 0)
                    {
                        MessageBox.Show("請求明細を登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("請求明細の登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


