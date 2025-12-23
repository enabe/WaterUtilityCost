namespace WaterUtilityCost.Forms
{
    partial class BillingDataRegistrationForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBillingYearMonth;
        private System.Windows.Forms.DateTimePicker dtpBillingYearMonth;
        private System.Windows.Forms.Button btnRegister;
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
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBillingYearMonth
            // 
            this.lblBillingYearMonth.AutoSize = true;
            this.lblBillingYearMonth.Location = new System.Drawing.Point(50, 50);
            this.lblBillingYearMonth.Name = "lblBillingYearMonth";
            this.lblBillingYearMonth.Size = new System.Drawing.Size(65, 12);
            this.lblBillingYearMonth.TabIndex = 0;
            this.lblBillingYearMonth.Text = "請求年月:";
            // 
            // dtpBillingYearMonth
            // 
            this.dtpBillingYearMonth.CustomFormat = "yyyy年MM月";
            this.dtpBillingYearMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBillingYearMonth.Location = new System.Drawing.Point(150, 47);
            this.dtpBillingYearMonth.Name = "dtpBillingYearMonth";
            this.dtpBillingYearMonth.ShowUpDown = true;
            this.dtpBillingYearMonth.Size = new System.Drawing.Size(200, 19);
            this.dtpBillingYearMonth.TabIndex = 1;
            this.dtpBillingYearMonth.Value = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(220, 200);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(100, 30);
            this.btnRegister.TabIndex = 4;
            this.btnRegister.Text = "登録";
            this.btnRegister.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(330, 200);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // BillingDataRegistrationForm
            // 
            this.AcceptButton = this.btnRegister;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.dtpBillingYearMonth);
            this.Controls.Add(this.lblBillingYearMonth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BillingDataRegistrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "請求データ登録";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

