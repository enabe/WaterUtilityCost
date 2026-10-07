namespace WaterUtilityCost.Forms
{
    partial class GasChildMeterReadingManagementForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView _dgvGasChildMeterReadings;
        private System.Windows.Forms.Button _btnAdd;
        private System.Windows.Forms.Button _btnEdit;
        private System.Windows.Forms.Button _btnDelete;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Button _btnCopyAndAdd;
        private System.Windows.Forms.Button _btnImportGasCsv;
        private System.Windows.Forms.Button _btnExportComparisonCsv;
        private System.Windows.Forms.Button _btnDeleteByYearMonth;
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
            this._dgvGasChildMeterReadings = new System.Windows.Forms.DataGridView();
            this._btnAdd = new System.Windows.Forms.Button();
            this._btnEdit = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this._btnRefresh = new System.Windows.Forms.Button();
            this._btnCopyAndAdd = new System.Windows.Forms.Button();
            this._btnImportGasCsv = new System.Windows.Forms.Button();
            this._btnExportComparisonCsv = new System.Windows.Forms.Button();
            this._btnDeleteByYearMonth = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this._dgvGasChildMeterReadings)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgvGasChildMeterReadings
            // 
            this._dgvGasChildMeterReadings.AllowUserToAddRows = false;
            this._dgvGasChildMeterReadings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvGasChildMeterReadings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvGasChildMeterReadings.ColumnHeadersHeight = 29;
            this._dgvGasChildMeterReadings.Location = new System.Drawing.Point(20, 70);
            this._dgvGasChildMeterReadings.MultiSelect = false;
            this._dgvGasChildMeterReadings.Name = "_dgvGasChildMeterReadings";
            this._dgvGasChildMeterReadings.ReadOnly = true;
            this._dgvGasChildMeterReadings.RowHeadersWidth = 51;
            this._dgvGasChildMeterReadings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvGasChildMeterReadings.Size = new System.Drawing.Size(1136, 350);
            this._dgvGasChildMeterReadings.TabIndex = 0;
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
            // _btnCopyAndAdd
            // 
            this._btnCopyAndAdd.Location = new System.Drawing.Point(130, 20);
            this._btnCopyAndAdd.Name = "_btnCopyAndAdd";
            this._btnCopyAndAdd.Size = new System.Drawing.Size(120, 30);
            this._btnCopyAndAdd.TabIndex = 2;
            this._btnCopyAndAdd.Text = "コピーして追加";
            this._btnCopyAndAdd.UseVisualStyleBackColor = true;
            // 
            // _btnImportGasCsv
            // 
            this._btnImportGasCsv.Location = new System.Drawing.Point(833, 20);
            this._btnImportGasCsv.Name = "_btnImportGasCsv";
            this._btnImportGasCsv.Size = new System.Drawing.Size(170, 30);
            this._btnImportGasCsv.TabIndex = 6;
            this._btnImportGasCsv.Text = "ガスCSV読込";
            this._btnImportGasCsv.UseVisualStyleBackColor = true;
            // 
            // _btnExportComparisonCsv
            // 
            this._btnExportComparisonCsv.Location = new System.Drawing.Point(1016, 20);
            this._btnExportComparisonCsv.Name = "_btnExportComparisonCsv";
            this._btnExportComparisonCsv.Size = new System.Drawing.Size(140, 30);
            this._btnExportComparisonCsv.TabIndex = 7;
            this._btnExportComparisonCsv.Text = "比較表出力";
            this._btnExportComparisonCsv.UseVisualStyleBackColor = true;
            // 
            // _btnDeleteByYearMonth
            // 
            this._btnDeleteByYearMonth.Location = new System.Drawing.Point(693, 20);
            this._btnDeleteByYearMonth.Name = "_btnDeleteByYearMonth";
            this._btnDeleteByYearMonth.Size = new System.Drawing.Size(128, 30);
            this._btnDeleteByYearMonth.TabIndex = 8;
            this._btnDeleteByYearMonth.Text = "対象年月削除";
            this._btnDeleteByYearMonth.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 474);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1180, 26);
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(69, 20);
            this.statusLabel.Text = "準備完了";
            // 
            // GasChildMeterReadingManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 500);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this._btnExportComparisonCsv);
            this.Controls.Add(this._btnImportGasCsv);
            this.Controls.Add(this._btnDeleteByYearMonth);
            this.Controls.Add(this._btnCopyAndAdd);
            this.Controls.Add(this._btnRefresh);
            this.Controls.Add(this._btnDelete);
            this.Controls.Add(this._btnEdit);
            this.Controls.Add(this._btnAdd);
            this.Controls.Add(this._dgvGasChildMeterReadings);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "GasChildMeterReadingManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ガス子メーター検針データ管理";
            ((System.ComponentModel.ISupportInitialize)(this._dgvGasChildMeterReadings)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

