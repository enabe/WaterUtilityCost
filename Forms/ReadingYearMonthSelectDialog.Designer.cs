namespace WaterUtilityCost.Forms
{
    partial class ReadingYearMonthSelectDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label _lblYearMonth;
        private System.Windows.Forms.DateTimePicker _dtpYearMonth;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._lblYearMonth = new System.Windows.Forms.Label();
            this._dtpYearMonth = new System.Windows.Forms.DateTimePicker();
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lblYearMonth
            // 
            this._lblYearMonth.AutoSize = true;
            this._lblYearMonth.Location = new System.Drawing.Point(20, 28);
            this._lblYearMonth.Name = "_lblYearMonth";
            this._lblYearMonth.Size = new System.Drawing.Size(121, 25);
            this._lblYearMonth.TabIndex = 0;
            this._lblYearMonth.Text = "削除する検針年月:";
            // 
            // _dtpYearMonth
            // 
            this._dtpYearMonth.CustomFormat = "yyyy年MM月";
            this._dtpYearMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._dtpYearMonth.Location = new System.Drawing.Point(160, 24);
            this._dtpYearMonth.Name = "_dtpYearMonth";
            this._dtpYearMonth.ShowUpDown = true;
            this._dtpYearMonth.Size = new System.Drawing.Size(180, 32);
            this._dtpYearMonth.TabIndex = 1;
            // 
            // _btnOk
            // 
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOk.Location = new System.Drawing.Point(120, 80);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(100, 34);
            this._btnOk.TabIndex = 2;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = true;
            // 
            // _btnCancel
            // 
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(240, 80);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(100, 34);
            this._btnCancel.TabIndex = 3;
            this._btnCancel.Text = "キャンセル";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // ReadingYearMonthSelectDialog
            // 
            this.AcceptButton = this._btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(370, 135);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOk);
            this.Controls.Add(this._dtpYearMonth);
            this.Controls.Add(this._lblYearMonth);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReadingYearMonthSelectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "対象年月の指定";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
