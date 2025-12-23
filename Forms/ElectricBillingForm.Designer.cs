namespace WaterUtilityCost.Forms
{
    partial class ElectricBillingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBillingYearMonth;
        private System.Windows.Forms.TextBox txtBillingYearMonth;
        private System.Windows.Forms.Label lblBuildingName;
        private System.Windows.Forms.ComboBox cmbBuildingName;
        private System.Windows.Forms.Label lblUsageAmount;
        private System.Windows.Forms.TextBox txtUsageAmount;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblBasicCharge;
        private System.Windows.Forms.TextBox txtBasicCharge;
        private System.Windows.Forms.Label lblPowerCharge;
        private System.Windows.Forms.TextBox txtPowerCharge;
        private System.Windows.Forms.Label lblTaxRate;
        private System.Windows.Forms.TextBox txtTaxRate;
        private System.Windows.Forms.Label lblCustomerNumber;
        private System.Windows.Forms.TextBox txtCustomerNumber;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblBillingYearMonth = new System.Windows.Forms.Label();
            this.txtBillingYearMonth = new System.Windows.Forms.TextBox();
            this.lblBuildingName = new System.Windows.Forms.Label();
            this.cmbBuildingName = new System.Windows.Forms.ComboBox();
            this.lblUsageAmount = new System.Windows.Forms.Label();
            this.txtUsageAmount = new System.Windows.Forms.TextBox();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblBasicCharge = new System.Windows.Forms.Label();
            this.txtBasicCharge = new System.Windows.Forms.TextBox();
            this.lblPowerCharge = new System.Windows.Forms.Label();
            this.txtPowerCharge = new System.Windows.Forms.TextBox();
            this.lblTaxRate = new System.Windows.Forms.Label();
            this.txtTaxRate = new System.Windows.Forms.TextBox();
            this.lblCustomerNumber = new System.Windows.Forms.Label();
            this.txtCustomerNumber = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBillingYearMonth
            // 
            this.lblBillingYearMonth.AutoSize = true;
            this.lblBillingYearMonth.Location = new System.Drawing.Point(20, 20);
            this.lblBillingYearMonth.Name = "lblBillingYearMonth";
            this.lblBillingYearMonth.Size = new System.Drawing.Size(185, 25);
            this.lblBillingYearMonth.TabIndex = 0;
            this.lblBillingYearMonth.Text = "受領請求年月 (YYYY-MM):*";
            // 
            // txtBillingYearMonth
            // 
            this.txtBillingYearMonth.Location = new System.Drawing.Point(208, 17);
            this.txtBillingYearMonth.Name = "txtBillingYearMonth";
            this.txtBillingYearMonth.Size = new System.Drawing.Size(300, 32);
            this.txtBillingYearMonth.TabIndex = 1;
            // 
            // lblBuildingName
            // 
            this.lblBuildingName.AutoSize = true;
            this.lblBuildingName.Location = new System.Drawing.Point(20, 55);
            this.lblBuildingName.Name = "lblBuildingName";
            this.lblBuildingName.Size = new System.Drawing.Size(70, 25);
            this.lblBuildingName.TabIndex = 2;
            this.lblBuildingName.Text = "ビル名:*";
            // 
            // cmbBuildingName
            // 
            this.cmbBuildingName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildingName.FormattingEnabled = true;
            this.cmbBuildingName.Location = new System.Drawing.Point(208, 52);
            this.cmbBuildingName.Name = "cmbBuildingName";
            this.cmbBuildingName.Size = new System.Drawing.Size(300, 33);
            this.cmbBuildingName.TabIndex = 3;
            // 
            // lblUsageAmount
            // 
            this.lblUsageAmount.AutoSize = true;
            this.lblUsageAmount.Location = new System.Drawing.Point(20, 90);
            this.lblUsageAmount.Name = "lblUsageAmount";
            this.lblUsageAmount.Size = new System.Drawing.Size(70, 25);
            this.lblUsageAmount.TabIndex = 4;
            this.lblUsageAmount.Text = "使用量:*";
            // 
            // txtUsageAmount
            // 
            this.txtUsageAmount.Location = new System.Drawing.Point(208, 87);
            this.txtUsageAmount.Name = "txtUsageAmount";
            this.txtUsageAmount.Size = new System.Drawing.Size(300, 32);
            this.txtUsageAmount.TabIndex = 5;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(20, 125);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(70, 25);
            this.lblStartDate.TabIndex = 6;
            this.lblStartDate.Text = "使用期間(開始):";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(208, 122);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(300, 32);
            this.dtpStartDate.TabIndex = 7;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(20, 160);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(70, 25);
            this.lblEndDate.TabIndex = 8;
            this.lblEndDate.Text = "使用期間(終了):";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(208, 157);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(300, 32);
            this.dtpEndDate.TabIndex = 9;
            // 
            // lblBasicCharge
            // 
            this.lblBasicCharge.AutoSize = true;
            this.lblBasicCharge.Location = new System.Drawing.Point(20, 195);
            this.lblBasicCharge.Name = "lblBasicCharge";
            this.lblBasicCharge.Size = new System.Drawing.Size(87, 25);
            this.lblBasicCharge.TabIndex = 10;
            this.lblBasicCharge.Text = "基本料金:*";
            // 
            // txtBasicCharge
            // 
            this.txtBasicCharge.Location = new System.Drawing.Point(208, 192);
            this.txtBasicCharge.Name = "txtBasicCharge";
            this.txtBasicCharge.Size = new System.Drawing.Size(300, 32);
            this.txtBasicCharge.TabIndex = 11;
            // 
            // lblPowerCharge
            // 
            this.lblPowerCharge.AutoSize = true;
            this.lblPowerCharge.Location = new System.Drawing.Point(20, 230);
            this.lblPowerCharge.Name = "lblPowerCharge";
            this.lblPowerCharge.Size = new System.Drawing.Size(104, 25);
            this.lblPowerCharge.TabIndex = 12;
            this.lblPowerCharge.Text = "使用料金:*";
            // 
            // txtPowerCharge
            // 
            this.txtPowerCharge.Location = new System.Drawing.Point(208, 227);
            this.txtPowerCharge.Name = "txtPowerCharge";
            this.txtPowerCharge.Size = new System.Drawing.Size(300, 32);
            this.txtPowerCharge.TabIndex = 13;
            // 
            // lblTaxRate
            // 
            this.lblTaxRate.AutoSize = true;
            this.lblTaxRate.Location = new System.Drawing.Point(20, 265);
            this.lblTaxRate.Name = "lblTaxRate";
            this.lblTaxRate.Size = new System.Drawing.Size(53, 25);
            this.lblTaxRate.TabIndex = 14;
            this.lblTaxRate.Text = "税率:";
            // 
            // txtTaxRate
            // 
            this.txtTaxRate.Location = new System.Drawing.Point(208, 262);
            this.txtTaxRate.Name = "txtTaxRate";
            this.txtTaxRate.Size = new System.Drawing.Size(300, 32);
            this.txtTaxRate.TabIndex = 15;
            // 
            // lblCustomerNumber
            // 
            this.lblCustomerNumber.AutoSize = true;
            this.lblCustomerNumber.Location = new System.Drawing.Point(20, 300);
            this.lblCustomerNumber.Name = "lblCustomerNumber";
            this.lblCustomerNumber.Size = new System.Drawing.Size(104, 25);
            this.lblCustomerNumber.TabIndex = 16;
            this.lblCustomerNumber.Text = "お客様番号:";
            // 
            // txtCustomerNumber
            // 
            this.txtCustomerNumber.Location = new System.Drawing.Point(208, 297);
            this.txtCustomerNumber.Name = "txtCustomerNumber";
            this.txtCustomerNumber.Size = new System.Drawing.Size(300, 32);
            this.txtCustomerNumber.TabIndex = 17;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(272, 345);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(382, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(126, 30);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ElectricBillingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 397);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtCustomerNumber);
            this.Controls.Add(this.lblCustomerNumber);
            this.Controls.Add(this.txtTaxRate);
            this.Controls.Add(this.lblTaxRate);
            this.Controls.Add(this.txtPowerCharge);
            this.Controls.Add(this.lblPowerCharge);
            this.Controls.Add(this.txtBasicCharge);
            this.Controls.Add(this.lblBasicCharge);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.txtUsageAmount);
            this.Controls.Add(this.lblUsageAmount);
            this.Controls.Add(this.cmbBuildingName);
            this.Controls.Add(this.lblBuildingName);
            this.Controls.Add(this.txtBillingYearMonth);
            this.Controls.Add(this.lblBillingYearMonth);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ElectricBillingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "電気料金請求情報登録";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
