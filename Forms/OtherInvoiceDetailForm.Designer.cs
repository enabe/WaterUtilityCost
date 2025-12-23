namespace WaterUtilityCost.Forms
{
    partial class OtherInvoiceDetailForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView _dgvOtherInvoiceDetails;
        private System.Windows.Forms.Button _btnAdd;
        private System.Windows.Forms.Button _btnCopy;
        private System.Windows.Forms.Button _btnEdit;
        private System.Windows.Forms.Button _btnDelete;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Label _lblTotalAmount;
        private System.Windows.Forms.Label _lblBillingYearMonth;
        private System.Windows.Forms.DateTimePicker _dtpBillingYearMonth;
        private System.Windows.Forms.Label _lblSearchBillingTo;
        private System.Windows.Forms.TextBox _txtSearchBillingTo;
        private System.Windows.Forms.Label _lblSearchBuildingName;
        private System.Windows.Forms.TextBox _txtSearchBuildingName;
        private System.Windows.Forms.Label _lblSearchCategory;
        private System.Windows.Forms.ComboBox _cmbSearchCategory;
        private System.Windows.Forms.Button _btnSearch;
        private System.Windows.Forms.Button _btnClearSearch;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this._dgvOtherInvoiceDetails = new System.Windows.Forms.DataGridView();
            this._btnAdd = new System.Windows.Forms.Button();
            this._btnCopy = new System.Windows.Forms.Button();
            this._btnEdit = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this._btnRefresh = new System.Windows.Forms.Button();
            this._lblTotalAmount = new System.Windows.Forms.Label();
            this._lblBillingYearMonth = new System.Windows.Forms.Label();
            this._dtpBillingYearMonth = new System.Windows.Forms.DateTimePicker();
            this._lblSearchBillingTo = new System.Windows.Forms.Label();
            this._txtSearchBillingTo = new System.Windows.Forms.TextBox();
            this._lblSearchBuildingName = new System.Windows.Forms.Label();
            this._txtSearchBuildingName = new System.Windows.Forms.TextBox();
            this._lblSearchCategory = new System.Windows.Forms.Label();
            this._cmbSearchCategory = new System.Windows.Forms.ComboBox();
            this._btnSearch = new System.Windows.Forms.Button();
            this._btnClearSearch = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this._dgvOtherInvoiceDetails)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgvOtherInvoiceDetails
            // 
            this._dgvOtherInvoiceDetails.AllowUserToAddRows = false;
            this._dgvOtherInvoiceDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvOtherInvoiceDetails.ColumnHeadersHeight = 29;
            this._dgvOtherInvoiceDetails.Location = new System.Drawing.Point(20, 145);
            this._dgvOtherInvoiceDetails.MultiSelect = false;
            this._dgvOtherInvoiceDetails.Name = "_dgvOtherInvoiceDetails";
            this._dgvOtherInvoiceDetails.ReadOnly = true;
            this._dgvOtherInvoiceDetails.RowHeadersWidth = 51;
            this._dgvOtherInvoiceDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvOtherInvoiceDetails.Size = new System.Drawing.Size(1340, 424);
            this._dgvOtherInvoiceDetails.TabIndex = 0;
            // 
            // _btnAdd
            // 
            this._btnAdd.Location = new System.Drawing.Point(20, 20);
            this._btnAdd.Name = "_btnAdd";
            this._btnAdd.Size = new System.Drawing.Size(100, 30);
            this._btnAdd.TabIndex = 1;
            this._btnAdd.Text = "新規登録";
            this._btnAdd.UseVisualStyleBackColor = true;
            // 
            // _btnCopy
            // 
            this._btnCopy.Location = new System.Drawing.Point(130, 20);
            this._btnCopy.Name = "_btnCopy";
            this._btnCopy.Size = new System.Drawing.Size(120, 30);
            this._btnCopy.TabIndex = 2;
            this._btnCopy.Text = "コピーして追加";
            this._btnCopy.UseVisualStyleBackColor = true;
            // 
            // _btnEdit
            // 
            this._btnEdit.Location = new System.Drawing.Point(260, 20);
            this._btnEdit.Name = "_btnEdit";
            this._btnEdit.Size = new System.Drawing.Size(100, 30);
            this._btnEdit.TabIndex = 3;
            this._btnEdit.Text = "編集";
            this._btnEdit.UseVisualStyleBackColor = true;
            // 
            // _btnDelete
            // 
            this._btnDelete.Location = new System.Drawing.Point(370, 20);
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.Size = new System.Drawing.Size(100, 30);
            this._btnDelete.TabIndex = 4;
            this._btnDelete.Text = "削除";
            this._btnDelete.UseVisualStyleBackColor = true;
            // 
            // _btnRefresh
            // 
            this._btnRefresh.Location = new System.Drawing.Point(480, 20);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(100, 30);
            this._btnRefresh.TabIndex = 5;
            this._btnRefresh.Text = "更新";
            this._btnRefresh.UseVisualStyleBackColor = true;
            // 
            // _lblTotalAmount
            // 
            this._lblTotalAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lblTotalAmount.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblTotalAmount.Location = new System.Drawing.Point(1180, 112);
            this._lblTotalAmount.Name = "_lblTotalAmount";
            this._lblTotalAmount.Size = new System.Drawing.Size(180, 30);
            this._lblTotalAmount.TabIndex = 10;
            this._lblTotalAmount.Text = "合計金額: ¥0";
            this._lblTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblBillingYearMonth
            // 
            this._lblBillingYearMonth.AutoSize = true;
            this._lblBillingYearMonth.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblBillingYearMonth.Location = new System.Drawing.Point(20, 76);
            this._lblBillingYearMonth.Name = "_lblBillingYearMonth";
            this._lblBillingYearMonth.Size = new System.Drawing.Size(87, 25);
            this._lblBillingYearMonth.TabIndex = 6;
            this._lblBillingYearMonth.Text = "請求年月:";
            // 
            // _dtpBillingYearMonth
            // 
            this._dtpBillingYearMonth.CustomFormat = "yyyy年MM月";
            this._dtpBillingYearMonth.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._dtpBillingYearMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._dtpBillingYearMonth.Location = new System.Drawing.Point(110, 73);
            this._dtpBillingYearMonth.Name = "_dtpBillingYearMonth";
            this._dtpBillingYearMonth.ShowUpDown = true;
            this._dtpBillingYearMonth.Size = new System.Drawing.Size(150, 32);
            this._dtpBillingYearMonth.TabIndex = 7;
            this._dtpBillingYearMonth.Value = new System.DateTime(2025, 12, 1, 0, 0, 0, 0);
            // 
            // _lblSearchBillingTo
            // 
            this._lblSearchBillingTo.AutoSize = true;
            this._lblSearchBillingTo.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblSearchBillingTo.Location = new System.Drawing.Point(278, 76);
            this._lblSearchBillingTo.Name = "_lblSearchBillingTo";
            this._lblSearchBillingTo.Size = new System.Drawing.Size(70, 25);
            this._lblSearchBillingTo.TabIndex = 11;
            this._lblSearchBillingTo.Text = "請求先:";
            // 
            // _txtSearchBillingTo
            // 
            this._txtSearchBillingTo.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._txtSearchBillingTo.Location = new System.Drawing.Point(353, 73);
            this._txtSearchBillingTo.Name = "_txtSearchBillingTo";
            this._txtSearchBillingTo.Size = new System.Drawing.Size(150, 32);
            this._txtSearchBillingTo.TabIndex = 12;
            // 
            // _lblSearchBuildingName
            // 
            this._lblSearchBuildingName.AutoSize = true;
            this._lblSearchBuildingName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblSearchBuildingName.Location = new System.Drawing.Point(518, 76);
            this._lblSearchBuildingName.Name = "_lblSearchBuildingName";
            this._lblSearchBuildingName.Size = new System.Drawing.Size(70, 25);
            this._lblSearchBuildingName.TabIndex = 13;
            this._lblSearchBuildingName.Text = "建物名:";
            // 
            // _txtSearchBuildingName
            // 
            this._txtSearchBuildingName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._txtSearchBuildingName.Location = new System.Drawing.Point(593, 73);
            this._txtSearchBuildingName.Name = "_txtSearchBuildingName";
            this._txtSearchBuildingName.Size = new System.Drawing.Size(150, 32);
            this._txtSearchBuildingName.TabIndex = 14;
            // 
            // _lblSearchCategory
            // 
            this._lblSearchCategory.AutoSize = true;
            this._lblSearchCategory.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblSearchCategory.Location = new System.Drawing.Point(758, 76);
            this._lblSearchCategory.Name = "_lblSearchCategory";
            this._lblSearchCategory.Size = new System.Drawing.Size(53, 25);
            this._lblSearchCategory.TabIndex = 15;
            this._lblSearchCategory.Text = "種別:";
            // 
            // _cmbSearchCategory
            // 
            this._cmbSearchCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbSearchCategory.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._cmbSearchCategory.FormattingEnabled = true;
            this._cmbSearchCategory.Location = new System.Drawing.Point(819, 73);
            this._cmbSearchCategory.Name = "_cmbSearchCategory";
            this._cmbSearchCategory.Size = new System.Drawing.Size(120, 33);
            this._cmbSearchCategory.TabIndex = 16;
            // 
            // _btnSearch
            // 
            this._btnSearch.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnSearch.Location = new System.Drawing.Point(958, 71);
            this._btnSearch.Name = "_btnSearch";
            this._btnSearch.Size = new System.Drawing.Size(80, 35);
            this._btnSearch.TabIndex = 17;
            this._btnSearch.Text = "検索";
            this._btnSearch.UseVisualStyleBackColor = true;
            // 
            // _btnClearSearch
            // 
            this._btnClearSearch.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnClearSearch.Location = new System.Drawing.Point(1048, 71);
            this._btnClearSearch.Name = "_btnClearSearch";
            this._btnClearSearch.Size = new System.Drawing.Size(80, 35);
            this._btnClearSearch.TabIndex = 18;
            this._btnClearSearch.Text = "クリア";
            this._btnClearSearch.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 580);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1400, 26);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(69, 20);
            this.statusLabel.Text = "準備完了";
            // 
            // OtherInvoiceDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 606);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this._btnClearSearch);
            this.Controls.Add(this._btnSearch);
            this.Controls.Add(this._cmbSearchCategory);
            this.Controls.Add(this._lblSearchCategory);
            this.Controls.Add(this._txtSearchBuildingName);
            this.Controls.Add(this._lblSearchBuildingName);
            this.Controls.Add(this._txtSearchBillingTo);
            this.Controls.Add(this._lblSearchBillingTo);
            this.Controls.Add(this._dtpBillingYearMonth);
            this.Controls.Add(this._lblBillingYearMonth);
            this.Controls.Add(this._lblTotalAmount);
            this.Controls.Add(this._btnRefresh);
            this.Controls.Add(this._btnDelete);
            this.Controls.Add(this._btnEdit);
            this.Controls.Add(this._btnCopy);
            this.Controls.Add(this._btnAdd);
            this.Controls.Add(this._dgvOtherInvoiceDetails);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "OtherInvoiceDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "その他請求明細一覧";
            ((System.ComponentModel.ISupportInitialize)(this._dgvOtherInvoiceDetails)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}


