namespace WaterUtilityCost.Forms
{
    partial class GasChildMeterReadingForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblMeterType;
        private System.Windows.Forms.TextBox txtMeterType;
        private System.Windows.Forms.Label lblBuildingName;
        private System.Windows.Forms.ComboBox cmbBuildingName;
        private System.Windows.Forms.Label lblChildMeterName;
        private System.Windows.Forms.ComboBox cmbChildMeterName;
        private System.Windows.Forms.Label lblReadingDate;
        private System.Windows.Forms.DateTimePicker dtpReadingDate;
        private System.Windows.Forms.Label lblMeterValue;
        private System.Windows.Forms.TextBox txtMeterValue;
        private System.Windows.Forms.Button btnSave;
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
            this.lblMeterType = new System.Windows.Forms.Label();
            this.txtMeterType = new System.Windows.Forms.TextBox();
            this.lblBuildingName = new System.Windows.Forms.Label();
            this.cmbBuildingName = new System.Windows.Forms.ComboBox();
            this.lblChildMeterName = new System.Windows.Forms.Label();
            this.cmbChildMeterName = new System.Windows.Forms.ComboBox();
            this.lblReadingDate = new System.Windows.Forms.Label();
            this.dtpReadingDate = new System.Windows.Forms.DateTimePicker();
            this.lblMeterValue = new System.Windows.Forms.Label();
            this.txtMeterValue = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblReadingDate
            // 
            this.lblReadingDate.AutoSize = true;
            this.lblReadingDate.Location = new System.Drawing.Point(20, 20);
            this.lblReadingDate.Name = "lblReadingDate";
            this.lblReadingDate.Size = new System.Drawing.Size(53, 12);
            this.lblReadingDate.TabIndex = 0;
            this.lblReadingDate.Text = "検針日:";
            // 
            // dtpReadingDate
            // 
            this.dtpReadingDate.Location = new System.Drawing.Point(150, 17);
            this.dtpReadingDate.Name = "dtpReadingDate";
            this.dtpReadingDate.Size = new System.Drawing.Size(200, 19);
            this.dtpReadingDate.TabIndex = 1;
            // 
            // lblMeterType
            // 
            this.lblMeterType.AutoSize = true;
            this.lblMeterType.Location = new System.Drawing.Point(20, 55);
            this.lblMeterType.Name = "lblMeterType";
            this.lblMeterType.Size = new System.Drawing.Size(65, 12);
            this.lblMeterType.TabIndex = 2;
            this.lblMeterType.Text = "メーター種別:";
            // 
            // txtMeterType
            // 
            this.txtMeterType.Location = new System.Drawing.Point(150, 52);
            this.txtMeterType.Name = "txtMeterType";
            this.txtMeterType.ReadOnly = true;
            this.txtMeterType.Size = new System.Drawing.Size(200, 19);
            this.txtMeterType.TabIndex = 3;
            // 
            // lblBuildingName
            // 
            this.lblBuildingName.AutoSize = true;
            this.lblBuildingName.Location = new System.Drawing.Point(20, 90);
            this.lblBuildingName.Name = "lblBuildingName";
            this.lblBuildingName.Size = new System.Drawing.Size(53, 12);
            this.lblBuildingName.TabIndex = 4;
            this.lblBuildingName.Text = "ビル名:";
            // 
            // cmbBuildingName
            // 
            this.cmbBuildingName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildingName.FormattingEnabled = true;
            this.cmbBuildingName.Location = new System.Drawing.Point(150, 87);
            this.cmbBuildingName.Name = "cmbBuildingName";
            this.cmbBuildingName.Size = new System.Drawing.Size(350, 20);
            this.cmbBuildingName.TabIndex = 5;
            // 
            // lblChildMeterName
            // 
            this.lblChildMeterName.AutoSize = true;
            this.lblChildMeterName.Location = new System.Drawing.Point(20, 125);
            this.lblChildMeterName.Name = "lblChildMeterName";
            this.lblChildMeterName.Size = new System.Drawing.Size(65, 12);
            this.lblChildMeterName.TabIndex = 6;
            this.lblChildMeterName.Text = "子メーター:";
            // 
            // cmbChildMeterName
            // 
            this.cmbChildMeterName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChildMeterName.FormattingEnabled = true;
            this.cmbChildMeterName.Location = new System.Drawing.Point(150, 122);
            this.cmbChildMeterName.Name = "cmbChildMeterName";
            this.cmbChildMeterName.Size = new System.Drawing.Size(350, 20);
            this.cmbChildMeterName.TabIndex = 7;
            // 
            // lblMeterValue
            // 
            this.lblMeterValue.AutoSize = true;
            this.lblMeterValue.Location = new System.Drawing.Point(20, 160);
            this.lblMeterValue.Name = "lblMeterValue";
            this.lblMeterValue.Size = new System.Drawing.Size(65, 12);
            this.lblMeterValue.TabIndex = 8;
            this.lblMeterValue.Text = "メーター値:";
            // 
            // txtMeterValue
            // 
            this.txtMeterValue.Location = new System.Drawing.Point(150, 157);
            this.txtMeterValue.Name = "txtMeterValue";
            this.txtMeterValue.Size = new System.Drawing.Size(200, 19);
            this.txtMeterValue.TabIndex = 9;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(150, 200);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 200);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // GasChildMeterReadingForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(550, 260);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtMeterValue);
            this.Controls.Add(this.lblMeterValue);
            this.Controls.Add(this.cmbChildMeterName);
            this.Controls.Add(this.lblChildMeterName);
            this.Controls.Add(this.cmbBuildingName);
            this.Controls.Add(this.lblBuildingName);
            this.Controls.Add(this.txtMeterType);
            this.Controls.Add(this.lblMeterType);
            this.Controls.Add(this.dtpReadingDate);
            this.Controls.Add(this.lblReadingDate);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GasChildMeterReadingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ガス子メータ検針データ登録";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

