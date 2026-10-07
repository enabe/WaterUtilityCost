namespace WaterUtilityCost.Forms
{
    partial class InvoiceDetailForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView _dgvInvoiceDetails;
        private System.Windows.Forms.Button _btnAdd;
        private System.Windows.Forms.Button _btnEdit;
        private System.Windows.Forms.Button _btnCopy;
        private System.Windows.Forms.Button _btnDelete;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Button _btnExportCsv;
        private System.Windows.Forms.Button _btnExportComparisonCsv;
        private System.Windows.Forms.Button _btnExportYearComparisonCsv;
        private System.Windows.Forms.Label _lblBillingYearMonth;
        private System.Windows.Forms.DateTimePicker _dtpBillingYearMonth;
        private System.Windows.Forms.Label _lblTotalAmount;
        private System.Windows.Forms.Label _lblTotalUsage;
        private System.Windows.Forms.Label _lblTotalChildUsage;
        private System.Windows.Forms.Label _lblTotalArea;
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
            this._dgvInvoiceDetails = new System.Windows.Forms.DataGridView();
            this._btnAdd = new System.Windows.Forms.Button();
            this._btnEdit = new System.Windows.Forms.Button();
            this._btnCopy = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this._btnRefresh = new System.Windows.Forms.Button();
            this._btnExportCsv = new System.Windows.Forms.Button();
            this._btnExportComparisonCsv = new System.Windows.Forms.Button();
            this._btnExportYearComparisonCsv = new System.Windows.Forms.Button();
            this._lblBillingYearMonth = new System.Windows.Forms.Label();
            this._dtpBillingYearMonth = new System.Windows.Forms.DateTimePicker();
            this._lblTotalAmount = new System.Windows.Forms.Label();
            this._lblTotalUsage = new System.Windows.Forms.Label();
            this._lblTotalChildUsage = new System.Windows.Forms.Label();
            this._lblTotalArea = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this._dgvInvoiceDetails)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgvInvoiceDetails
            // 
            this._dgvInvoiceDetails.AllowUserToAddRows = false;
            this._dgvInvoiceDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvInvoiceDetails.ColumnHeadersHeight = 29;
            this._dgvInvoiceDetails.Location = new System.Drawing.Point(20, 141);
            this._dgvInvoiceDetails.MultiSelect = false;
            this._dgvInvoiceDetails.Name = "_dgvInvoiceDetails";
            this._dgvInvoiceDetails.ReadOnly = true;
            this._dgvInvoiceDetails.RowHeadersWidth = 51;
            this._dgvInvoiceDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvInvoiceDetails.Size = new System.Drawing.Size(1340, 470);
            this._dgvInvoiceDetails.TabIndex = 0;
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
            // _btnEdit
            // 
            this._btnEdit.Location = new System.Drawing.Point(264, 20);
            this._btnEdit.Name = "_btnEdit";
            this._btnEdit.Size = new System.Drawing.Size(100, 30);
            this._btnEdit.TabIndex = 2;
            this._btnEdit.Text = "編集";
            this._btnEdit.UseVisualStyleBackColor = true;
            this._btnEdit.Click += new System.EventHandler(this._btnEdit_Click);
            // 
            // _btnCopy
            // 
            this._btnCopy.Location = new System.Drawing.Point(130, 20);
            this._btnCopy.Name = "_btnCopy";
            this._btnCopy.Size = new System.Drawing.Size(120, 30);
            this._btnCopy.TabIndex = 19;
            this._btnCopy.Text = "コピーして追加";
            this._btnCopy.UseVisualStyleBackColor = true;
            // 
            // _btnDelete
            // 
            this._btnDelete.Location = new System.Drawing.Point(374, 20);
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.Size = new System.Drawing.Size(100, 30);
            this._btnDelete.TabIndex = 3;
            this._btnDelete.Text = "削除";
            this._btnDelete.UseVisualStyleBackColor = true;
            // 
            // _btnRefresh
            // 
            this._btnRefresh.Location = new System.Drawing.Point(490, 20);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(100, 30);
            this._btnRefresh.TabIndex = 4;
            this._btnRefresh.Text = "更新";
            this._btnRefresh.UseVisualStyleBackColor = true;
            // 
            // _btnExportCsv
            // 
            this._btnExportCsv.Location = new System.Drawing.Point(975, 20);
            this._btnExportCsv.Name = "_btnExportCsv";
            this._btnExportCsv.Size = new System.Drawing.Size(100, 30);
            this._btnExportCsv.TabIndex = 5;
            this._btnExportCsv.Text = "CSV出力";
            this._btnExportCsv.UseVisualStyleBackColor = true;
            this._btnExportCsv.Visible = false;
            // 
            // _btnExportComparisonCsv
            // 
            this._btnExportComparisonCsv.Location = new System.Drawing.Point(1085, 20);
            this._btnExportComparisonCsv.Name = "_btnExportComparisonCsv";
            this._btnExportComparisonCsv.Size = new System.Drawing.Size(130, 30);
            this._btnExportComparisonCsv.TabIndex = 8;
            this._btnExportComparisonCsv.Text = "前月比較CSV出力";
            this._btnExportComparisonCsv.UseVisualStyleBackColor = true;
            // 
            // _btnExportYearComparisonCsv
            // 
            this._btnExportYearComparisonCsv.Location = new System.Drawing.Point(1225, 20);
            this._btnExportYearComparisonCsv.Name = "_btnExportYearComparisonCsv";
            this._btnExportYearComparisonCsv.Size = new System.Drawing.Size(130, 30);
            this._btnExportYearComparisonCsv.TabIndex = 9;
            this._btnExportYearComparisonCsv.Text = "前年比較CSV出力";
            this._btnExportYearComparisonCsv.UseVisualStyleBackColor = true;
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
            // _lblTotalAmount
            // 
            this._lblTotalAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lblTotalAmount.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblTotalAmount.Location = new System.Drawing.Point(1140, 108);
            this._lblTotalAmount.Name = "_lblTotalAmount";
            this._lblTotalAmount.Size = new System.Drawing.Size(220, 30);
            this._lblTotalAmount.TabIndex = 10;
            this._lblTotalAmount.Text = "税込金額合計: ¥0";
            this._lblTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblTotalUsage
            // 
            this._lblTotalUsage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lblTotalUsage.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblTotalUsage.Location = new System.Drawing.Point(900, 108);
            this._lblTotalUsage.Name = "_lblTotalUsage";
            this._lblTotalUsage.Size = new System.Drawing.Size(230, 30);
            this._lblTotalUsage.TabIndex = 20;
            this._lblTotalUsage.Text = "使用量合計: 0";
            this._lblTotalUsage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblTotalChildUsage
            // 
            this._lblTotalChildUsage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lblTotalChildUsage.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblTotalChildUsage.Location = new System.Drawing.Point(670, 108);
            this._lblTotalChildUsage.Name = "_lblTotalChildUsage";
            this._lblTotalChildUsage.Size = new System.Drawing.Size(230, 30);
            this._lblTotalChildUsage.TabIndex = 21;
            this._lblTotalChildUsage.Text = "子使用量合計: 0";
            this._lblTotalChildUsage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblTotalArea
            // 
            this._lblTotalArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lblTotalArea.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblTotalArea.Location = new System.Drawing.Point(440, 108);
            this._lblTotalArea.Name = "_lblTotalArea";
            this._lblTotalArea.Size = new System.Drawing.Size(230, 30);
            this._lblTotalArea.TabIndex = 22;
            this._lblTotalArea.Text = "面積合計: 0";
            this._lblTotalArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblSearchBillingTo
            // 
            this._lblSearchBillingTo.AutoSize = true;
            this._lblSearchBillingTo.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblSearchBillingTo.Location = new System.Drawing.Point(280, 76);
            this._lblSearchBillingTo.Name = "_lblSearchBillingTo";
            this._lblSearchBillingTo.Size = new System.Drawing.Size(70, 25);
            this._lblSearchBillingTo.TabIndex = 11;
            this._lblSearchBillingTo.Text = "請求先:";
            // 
            // _txtSearchBillingTo
            // 
            this._txtSearchBillingTo.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._txtSearchBillingTo.Location = new System.Drawing.Point(355, 73);
            this._txtSearchBillingTo.Name = "_txtSearchBillingTo";
            this._txtSearchBillingTo.Size = new System.Drawing.Size(150, 32);
            this._txtSearchBillingTo.TabIndex = 12;
            // 
            // _lblSearchBuildingName
            // 
            this._lblSearchBuildingName.AutoSize = true;
            this._lblSearchBuildingName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblSearchBuildingName.Location = new System.Drawing.Point(520, 76);
            this._lblSearchBuildingName.Name = "_lblSearchBuildingName";
            this._lblSearchBuildingName.Size = new System.Drawing.Size(70, 25);
            this._lblSearchBuildingName.TabIndex = 13;
            this._lblSearchBuildingName.Text = "建物名:";
            // 
            // _txtSearchBuildingName
            // 
            this._txtSearchBuildingName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._txtSearchBuildingName.Location = new System.Drawing.Point(595, 73);
            this._txtSearchBuildingName.Name = "_txtSearchBuildingName";
            this._txtSearchBuildingName.Size = new System.Drawing.Size(150, 32);
            this._txtSearchBuildingName.TabIndex = 14;
            // 
            // _lblSearchCategory
            // 
            this._lblSearchCategory.AutoSize = true;
            this._lblSearchCategory.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._lblSearchCategory.Location = new System.Drawing.Point(760, 76);
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
            this._cmbSearchCategory.Location = new System.Drawing.Point(821, 73);
            this._cmbSearchCategory.Name = "_cmbSearchCategory";
            this._cmbSearchCategory.Size = new System.Drawing.Size(120, 33);
            this._cmbSearchCategory.TabIndex = 16;
            // 
            // _btnSearch
            // 
            this._btnSearch.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnSearch.Location = new System.Drawing.Point(960, 71);
            this._btnSearch.Name = "_btnSearch";
            this._btnSearch.Size = new System.Drawing.Size(80, 35);
            this._btnSearch.TabIndex = 17;
            this._btnSearch.Text = "検索";
            this._btnSearch.UseVisualStyleBackColor = true;
            // 
            // _btnClearSearch
            // 
            this._btnClearSearch.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnClearSearch.Location = new System.Drawing.Point(1050, 71);
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
            this.statusStrip.Location = new System.Drawing.Point(0, 620);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1400, 26);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(69, 20);
            this.statusLabel.Text = "準備完了";
            // 
            // InvoiceDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 646);
            this.Controls.Add(this._btnClearSearch);
            this.Controls.Add(this._btnSearch);
            this.Controls.Add(this._cmbSearchCategory);
            this.Controls.Add(this._lblSearchCategory);
            this.Controls.Add(this._txtSearchBuildingName);
            this.Controls.Add(this._lblSearchBuildingName);
            this.Controls.Add(this._txtSearchBillingTo);
            this.Controls.Add(this._lblSearchBillingTo);
            this.Controls.Add(this._lblTotalArea);
            this.Controls.Add(this._lblTotalChildUsage);
            this.Controls.Add(this._lblTotalUsage);
            this.Controls.Add(this._lblTotalAmount);
            this.Controls.Add(this._dtpBillingYearMonth);
            this.Controls.Add(this._lblBillingYearMonth);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this._btnExportYearComparisonCsv);
            this.Controls.Add(this._btnExportComparisonCsv);
            this.Controls.Add(this._btnExportCsv);
            this.Controls.Add(this._btnRefresh);
            this.Controls.Add(this._btnDelete);
            this.Controls.Add(this._btnCopy);
            this.Controls.Add(this._btnEdit);
            this.Controls.Add(this._btnAdd);
            this.Controls.Add(this._dgvInvoiceDetails);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "InvoiceDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "水道光熱費請求明細一覧";
            ((System.ComponentModel.ISupportInitialize)(this._dgvInvoiceDetails)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}


