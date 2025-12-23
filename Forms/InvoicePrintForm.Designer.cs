namespace WaterUtilityCost.Forms
{
    partial class InvoicePrintForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBillingYearMonth;
        private System.Windows.Forms.DateTimePicker dtpBillingYearMonth;
        private System.Windows.Forms.Button btnPrintInvoice;
        private System.Windows.Forms.Button btnPrintInvoiceList;
        private System.Windows.Forms.Button btnCancel;

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
            this.lblBillingYearMonth = new System.Windows.Forms.Label();
            this.dtpBillingYearMonth = new System.Windows.Forms.DateTimePicker();
            this.btnPrintInvoice = new System.Windows.Forms.Button();
            this.btnPrintInvoiceList = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBillingYearMonth
            // 
            this.lblBillingYearMonth.AutoSize = true;
            this.lblBillingYearMonth.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBillingYearMonth.Location = new System.Drawing.Point(50, 50);
            this.lblBillingYearMonth.Name = "lblBillingYearMonth";
            this.lblBillingYearMonth.Size = new System.Drawing.Size(100, 24);
            this.lblBillingYearMonth.TabIndex = 0;
            this.lblBillingYearMonth.Text = "請求年月:";
            // 
            // dtpBillingYearMonth
            // 
            this.dtpBillingYearMonth.CustomFormat = "yyyy年MM月";
            this.dtpBillingYearMonth.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpBillingYearMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBillingYearMonth.Location = new System.Drawing.Point(150, 47);
            this.dtpBillingYearMonth.Name = "dtpBillingYearMonth";
            this.dtpBillingYearMonth.ShowUpDown = true;
            this.dtpBillingYearMonth.Size = new System.Drawing.Size(200, 31);
            this.dtpBillingYearMonth.TabIndex = 1;
            this.dtpBillingYearMonth.Value = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
            // 
            // btnPrintInvoice
            // 
            this.btnPrintInvoice.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnPrintInvoice.Location = new System.Drawing.Point(50, 120);
            this.btnPrintInvoice.Name = "btnPrintInvoice";
            this.btnPrintInvoice.Size = new System.Drawing.Size(200, 50);
            this.btnPrintInvoice.TabIndex = 2;
            this.btnPrintInvoice.Text = "請求書印刷";
            this.btnPrintInvoice.UseVisualStyleBackColor = true;
            // 
            // btnPrintInvoiceList
            // 
            this.btnPrintInvoiceList.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnPrintInvoiceList.Location = new System.Drawing.Point(270, 120);
            this.btnPrintInvoiceList.Name = "btnPrintInvoiceList";
            this.btnPrintInvoiceList.Size = new System.Drawing.Size(200, 50);
            this.btnPrintInvoiceList.TabIndex = 3;
            this.btnPrintInvoiceList.Text = "請求一覧印刷";
            this.btnPrintInvoiceList.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(420, 200);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "閉じる";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // InvoicePrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(550, 280);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPrintInvoiceList);
            this.Controls.Add(this.btnPrintInvoice);
            this.Controls.Add(this.dtpBillingYearMonth);
            this.Controls.Add(this.lblBillingYearMonth);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvoicePrintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "請求書印刷";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

