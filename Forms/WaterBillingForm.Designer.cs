namespace WaterUtilityCost.Forms
{
    partial class WaterBillingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBillingYearMonth;
        private System.Windows.Forms.TextBox txtBillingYearMonth;
        private System.Windows.Forms.Label lblParentMeter;
        private System.Windows.Forms.ComboBox cmbParentMeter;
        private System.Windows.Forms.Label lblUsageAmount;
        private System.Windows.Forms.TextBox txtUsageAmount;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblBasicCharge;
        private System.Windows.Forms.TextBox txtBasicCharge;
        private System.Windows.Forms.Label lblUsageCharge;
        private System.Windows.Forms.TextBox txtUsageCharge;
        private System.Windows.Forms.Label lblTaxRate;
        private System.Windows.Forms.TextBox txtTaxRate;
        private System.Windows.Forms.Label lblCustomerNumber;
        private System.Windows.Forms.TextBox txtCustomerNumber;
        private System.Windows.Forms.Label lblContractor;
        private System.Windows.Forms.ComboBox cmbContractor;
        private System.Windows.Forms.Label lblDifferenceAssignmentRoom;
        private System.Windows.Forms.ComboBox cmbDifferenceAssignmentRoom;
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
            this.lblParentMeter = new System.Windows.Forms.Label();
            this.cmbParentMeter = new System.Windows.Forms.ComboBox();
            this.lblUsageAmount = new System.Windows.Forms.Label();
            this.txtUsageAmount = new System.Windows.Forms.TextBox();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblBasicCharge = new System.Windows.Forms.Label();
            this.txtBasicCharge = new System.Windows.Forms.TextBox();
            this.lblUsageCharge = new System.Windows.Forms.Label();
            this.txtUsageCharge = new System.Windows.Forms.TextBox();
            this.lblTaxRate = new System.Windows.Forms.Label();
            this.txtTaxRate = new System.Windows.Forms.TextBox();
            this.lblCustomerNumber = new System.Windows.Forms.Label();
            this.txtCustomerNumber = new System.Windows.Forms.TextBox();
            this.lblContractor = new System.Windows.Forms.Label();
            this.cmbContractor = new System.Windows.Forms.ComboBox();
            this.lblDifferenceAssignmentRoom = new System.Windows.Forms.Label();
            this.cmbDifferenceAssignmentRoom = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBillingYearMonth
            // 
            this.lblBillingYearMonth.AutoSize = true;
            this.lblBillingYearMonth.Location = new System.Drawing.Point(20, 20);
            this.lblBillingYearMonth.Name = "lblBillingYearMonth";
            this.lblBillingYearMonth.Size = new System.Drawing.Size(230, 25);
            this.lblBillingYearMonth.TabIndex = 0;
            this.lblBillingYearMonth.Text = "受領請求年月 (YYYY-MM):*";
            // 
            // txtBillingYearMonth
            // 
            this.txtBillingYearMonth.Location = new System.Drawing.Point(206, 17);
            this.txtBillingYearMonth.Name = "txtBillingYearMonth";
            this.txtBillingYearMonth.Size = new System.Drawing.Size(300, 32);
            this.txtBillingYearMonth.TabIndex = 1;
            // 
            // lblParentMeter
            // 
            this.lblParentMeter.AutoSize = true;
            this.lblParentMeter.Location = new System.Drawing.Point(20, 55);
            this.lblParentMeter.Name = "lblParentMeter";
            this.lblParentMeter.Size = new System.Drawing.Size(115, 25);
            this.lblParentMeter.TabIndex = 2;
            this.lblParentMeter.Text = "親メーター:*";
            // 
            // cmbParentMeter
            // 
            this.cmbParentMeter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParentMeter.FormattingEnabled = true;
            this.cmbParentMeter.Location = new System.Drawing.Point(206, 52);
            this.cmbParentMeter.Name = "cmbParentMeter";
            this.cmbParentMeter.Size = new System.Drawing.Size(300, 33);
            this.cmbParentMeter.TabIndex = 3;
            // 
            // lblUsageAmount
            // 
            this.lblUsageAmount.AutoSize = true;
            this.lblUsageAmount.Location = new System.Drawing.Point(20, 90);
            this.lblUsageAmount.Name = "lblUsageAmount";
            this.lblUsageAmount.Size = new System.Drawing.Size(81, 25);
            this.lblUsageAmount.TabIndex = 4;
            this.lblUsageAmount.Text = "使用量:*";
            // 
            // txtUsageAmount
            // 
            this.txtUsageAmount.Location = new System.Drawing.Point(206, 87);
            this.txtUsageAmount.Name = "txtUsageAmount";
            this.txtUsageAmount.Size = new System.Drawing.Size(300, 32);
            this.txtUsageAmount.TabIndex = 5;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(20, 125);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(135, 25);
            this.lblStartDate.TabIndex = 6;
            this.lblStartDate.Text = "使用期間(開始):";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.CustomFormat = "yyyy-MM-dd";
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDate.Location = new System.Drawing.Point(206, 122);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(300, 32);
            this.dtpStartDate.TabIndex = 7;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(20, 160);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(135, 25);
            this.lblEndDate.TabIndex = 8;
            this.lblEndDate.Text = "使用期間(終了):";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.CustomFormat = "yyyy-MM-dd";
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndDate.Location = new System.Drawing.Point(206, 157);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(300, 32);
            this.dtpEndDate.TabIndex = 9;
            // 
            // lblBasicCharge
            // 
            this.lblBasicCharge.AutoSize = true;
            this.lblBasicCharge.Location = new System.Drawing.Point(20, 195);
            this.lblBasicCharge.Name = "lblBasicCharge";
            this.lblBasicCharge.Size = new System.Drawing.Size(146, 25);
            this.lblBasicCharge.TabIndex = 10;
            this.lblBasicCharge.Text = "基本料金(税込):*";
            // 
            // txtBasicCharge
            // 
            this.txtBasicCharge.Location = new System.Drawing.Point(206, 192);
            this.txtBasicCharge.Name = "txtBasicCharge";
            this.txtBasicCharge.Size = new System.Drawing.Size(300, 32);
            this.txtBasicCharge.TabIndex = 11;
            // 
            // lblUsageCharge
            // 
            this.lblUsageCharge.AutoSize = true;
            this.lblUsageCharge.Location = new System.Drawing.Point(20, 230);
            this.lblUsageCharge.Name = "lblUsageCharge";
            this.lblUsageCharge.Size = new System.Drawing.Size(146, 25);
            this.lblUsageCharge.TabIndex = 12;
            this.lblUsageCharge.Text = "従量料金(税込):*";
            // 
            // txtUsageCharge
            // 
            this.txtUsageCharge.Location = new System.Drawing.Point(206, 227);
            this.txtUsageCharge.Name = "txtUsageCharge";
            this.txtUsageCharge.Size = new System.Drawing.Size(300, 32);
            this.txtUsageCharge.TabIndex = 13;
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
            this.txtTaxRate.Location = new System.Drawing.Point(206, 262);
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
            this.txtCustomerNumber.Location = new System.Drawing.Point(206, 297);
            this.txtCustomerNumber.Name = "txtCustomerNumber";
            this.txtCustomerNumber.Size = new System.Drawing.Size(300, 32);
            this.txtCustomerNumber.TabIndex = 17;
            // 
            // lblContractor
            // 
            this.lblContractor.AutoSize = true;
            this.lblContractor.Location = new System.Drawing.Point(20, 335);
            this.lblContractor.Name = "lblContractor";
            this.lblContractor.Size = new System.Drawing.Size(53, 25);
            this.lblContractor.TabIndex = 18;
            this.lblContractor.Text = "業者:";
            // 
            // cmbContractor
            // 
            this.cmbContractor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbContractor.FormattingEnabled = true;
            this.cmbContractor.Location = new System.Drawing.Point(206, 332);
            this.cmbContractor.Name = "cmbContractor";
            this.cmbContractor.Size = new System.Drawing.Size(300, 33);
            this.cmbContractor.TabIndex = 19;
            // 
            // lblDifferenceAssignmentRoom
            // 
            this.lblDifferenceAssignmentRoom.AutoSize = true;
            this.lblDifferenceAssignmentRoom.Location = new System.Drawing.Point(20, 370);
            this.lblDifferenceAssignmentRoom.Name = "lblDifferenceAssignmentRoom";
            this.lblDifferenceAssignmentRoom.Size = new System.Drawing.Size(206, 25);
            this.lblDifferenceAssignmentRoom.TabIndex = 20;
            this.lblDifferenceAssignmentRoom.Text = "差額割当先部屋（任意）:";
            // 
            // cmbDifferenceAssignmentRoom
            // 
            this.cmbDifferenceAssignmentRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDifferenceAssignmentRoom.FormattingEnabled = true;
            this.cmbDifferenceAssignmentRoom.Location = new System.Drawing.Point(206, 367);
            this.cmbDifferenceAssignmentRoom.Name = "cmbDifferenceAssignmentRoom";
            this.cmbDifferenceAssignmentRoom.Size = new System.Drawing.Size(300, 33);
            this.cmbDifferenceAssignmentRoom.TabIndex = 21;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(269, 420);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(379, 420);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(125, 30);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // WaterBillingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 470);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbDifferenceAssignmentRoom);
            this.Controls.Add(this.cmbContractor);
            this.Controls.Add(this.lblContractor);
            this.Controls.Add(this.lblDifferenceAssignmentRoom);
            this.Controls.Add(this.txtCustomerNumber);
            this.Controls.Add(this.lblCustomerNumber);
            this.Controls.Add(this.txtTaxRate);
            this.Controls.Add(this.lblTaxRate);
            this.Controls.Add(this.txtUsageCharge);
            this.Controls.Add(this.lblUsageCharge);
            this.Controls.Add(this.txtBasicCharge);
            this.Controls.Add(this.lblBasicCharge);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.txtUsageAmount);
            this.Controls.Add(this.lblUsageAmount);
            this.Controls.Add(this.cmbParentMeter);
            this.Controls.Add(this.lblParentMeter);
            this.Controls.Add(this.txtBillingYearMonth);
            this.Controls.Add(this.lblBillingYearMonth);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaterBillingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "水道料金請求データ登録";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}


