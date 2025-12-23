namespace WaterUtilityCost.Forms
{
    partial class ElectricChildMeterReadingForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblMeterType;
        private System.Windows.Forms.TextBox txtMeterType;
        private System.Windows.Forms.Label lblBuildingName;
        private System.Windows.Forms.ComboBox cmbBuildingName;
        private System.Windows.Forms.Label lblFloorName;
        private System.Windows.Forms.ComboBox cmbFloorName;
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
            this.lblFloorName = new System.Windows.Forms.Label();
            this.cmbFloorName = new System.Windows.Forms.ComboBox();
            this.lblReadingDate = new System.Windows.Forms.Label();
            this.dtpReadingDate = new System.Windows.Forms.DateTimePicker();
            this.lblMeterValue = new System.Windows.Forms.Label();
            this.txtMeterValue = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMeterType
            // 
            this.lblMeterType.AutoSize = true;
            this.lblMeterType.Location = new System.Drawing.Point(20, 20);
            this.lblMeterType.Name = "lblMeterType";
            this.lblMeterType.Size = new System.Drawing.Size(65, 12);
            this.lblMeterType.TabIndex = 0;
            this.lblMeterType.Text = "メーター種別:";
            // 
            // txtMeterType
            // 
            this.txtMeterType.Location = new System.Drawing.Point(150, 17);
            this.txtMeterType.Name = "txtMeterType";
            this.txtMeterType.ReadOnly = true;
            this.txtMeterType.Size = new System.Drawing.Size(200, 19);
            this.txtMeterType.TabIndex = 1;
            // 
            // lblBuildingName
            // 
            this.lblBuildingName.AutoSize = true;
            this.lblBuildingName.Location = new System.Drawing.Point(20, 55);
            this.lblBuildingName.Name = "lblBuildingName";
            this.lblBuildingName.Size = new System.Drawing.Size(53, 12);
            this.lblBuildingName.TabIndex = 2;
            this.lblBuildingName.Text = "ビル名:";
            // 
            // cmbBuildingName
            // 
            this.cmbBuildingName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildingName.FormattingEnabled = true;
            this.cmbBuildingName.Location = new System.Drawing.Point(150, 52);
            this.cmbBuildingName.Name = "cmbBuildingName";
            this.cmbBuildingName.Size = new System.Drawing.Size(350, 20);
            this.cmbBuildingName.TabIndex = 3;
            // 
            // lblFloorName
            // 
            this.lblFloorName.AutoSize = true;
            this.lblFloorName.Location = new System.Drawing.Point(20, 90);
            this.lblFloorName.Name = "lblFloorName";
            this.lblFloorName.Size = new System.Drawing.Size(65, 12);
            this.lblFloorName.TabIndex = 6;
            this.lblFloorName.Text = "部屋名:";
            // 
            // cmbFloorName
            // 
            this.cmbFloorName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFloorName.FormattingEnabled = true;
            this.cmbFloorName.Location = new System.Drawing.Point(150, 87);
            this.cmbFloorName.Name = "cmbFloorName";
            this.cmbFloorName.Size = new System.Drawing.Size(350, 20);
            this.cmbFloorName.TabIndex = 7;
            // 
            // lblReadingDate
            // 
            this.lblReadingDate.AutoSize = true;
            this.lblReadingDate.Location = new System.Drawing.Point(20, 125);
            this.lblReadingDate.Name = "lblReadingDate";
            this.lblReadingDate.Size = new System.Drawing.Size(53, 12);
            this.lblReadingDate.TabIndex = 8;
            this.lblReadingDate.Text = "検針日:";
            // 
            // dtpReadingDate
            // 
            this.dtpReadingDate.Location = new System.Drawing.Point(150, 122);
            this.dtpReadingDate.Name = "dtpReadingDate";
            this.dtpReadingDate.Size = new System.Drawing.Size(200, 19);
            this.dtpReadingDate.TabIndex = 9;
            // 
            // lblMeterValue
            // 
            this.lblMeterValue.AutoSize = true;
            this.lblMeterValue.Location = new System.Drawing.Point(20, 160);
            this.lblMeterValue.Name = "lblMeterValue";
            this.lblMeterValue.Size = new System.Drawing.Size(65, 12);
            this.lblMeterValue.TabIndex = 10;
            this.lblMeterValue.Text = "メーター値:";
            // 
            // txtMeterValue
            // 
            this.txtMeterValue.Location = new System.Drawing.Point(150, 157);
            this.txtMeterValue.Name = "txtMeterValue";
            this.txtMeterValue.Size = new System.Drawing.Size(200, 19);
            this.txtMeterValue.TabIndex = 11;
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
            // ElectricChildMeterReadingForm
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
            this.Controls.Add(this.dtpReadingDate);
            this.Controls.Add(this.lblReadingDate);
            this.Controls.Add(this.cmbFloorName);
            this.Controls.Add(this.lblFloorName);
            this.Controls.Add(this.cmbBuildingName);
            this.Controls.Add(this.lblBuildingName);
            this.Controls.Add(this.txtMeterType);
            this.Controls.Add(this.lblMeterType);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ElectricChildMeterReadingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "電気子メータ検針データ登録";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

