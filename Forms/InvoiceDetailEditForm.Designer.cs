namespace WaterUtilityCost.Forms
{
    partial class InvoiceDetailEditForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBillingTo;
        private System.Windows.Forms.TextBox txtBillingTo;
        private System.Windows.Forms.Label lblLessor;
        private System.Windows.Forms.TextBox txtLessor;
        private System.Windows.Forms.Label lblBuildingName;
        private System.Windows.Forms.TextBox txtBuildingName;
        private System.Windows.Forms.Label lblLessee;
        private System.Windows.Forms.TextBox txtLessee;
        private System.Windows.Forms.Label lblRoomNumber;
        private System.Windows.Forms.TextBox txtRoomNumber;
        private System.Windows.Forms.Label lblRoomArea;
        private System.Windows.Forms.TextBox txtRoomArea;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblChildMeterUsage;
        private System.Windows.Forms.TextBox txtChildMeterUsage;
        private System.Windows.Forms.Label lblUsageAmount;
        private System.Windows.Forms.TextBox txtUsageAmount;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.TextBox txtUnit;
        private System.Windows.Forms.Label lblTaxInclusiveAmount;
        private System.Windows.Forms.TextBox txtTaxInclusiveAmount;
        private System.Windows.Forms.Label lblTaxRate;
        private System.Windows.Forms.TextBox txtTaxRate;
        private System.Windows.Forms.Label lblChildMeterStartDate;
        private System.Windows.Forms.TextBox txtChildMeterStartDate;
        private System.Windows.Forms.Label lblChildMeterEndDate;
        private System.Windows.Forms.TextBox txtChildMeterEndDate;
        private System.Windows.Forms.Label lblParentMeterStartDate;
        private System.Windows.Forms.TextBox txtParentMeterStartDate;
        private System.Windows.Forms.Label lblParentMeterEndDate;
        private System.Windows.Forms.TextBox txtParentMeterEndDate;
        private System.Windows.Forms.Label lblBillingYearMonth;
        private System.Windows.Forms.TextBox txtBillingYearMonth;
        private System.Windows.Forms.Label lblConfirmedBillingDate;
        private System.Windows.Forms.DateTimePicker dtpConfirmedBillingDate;
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
            this.lblBillingTo = new System.Windows.Forms.Label();
            this.txtBillingTo = new System.Windows.Forms.TextBox();
            this.lblLessor = new System.Windows.Forms.Label();
            this.txtLessor = new System.Windows.Forms.TextBox();
            this.lblBuildingName = new System.Windows.Forms.Label();
            this.txtBuildingName = new System.Windows.Forms.TextBox();
            this.lblLessee = new System.Windows.Forms.Label();
            this.txtLessee = new System.Windows.Forms.TextBox();
            this.lblRoomNumber = new System.Windows.Forms.Label();
            this.txtRoomNumber = new System.Windows.Forms.TextBox();
            this.lblRoomArea = new System.Windows.Forms.Label();
            this.txtRoomArea = new System.Windows.Forms.TextBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.txtCategory = new System.Windows.Forms.TextBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblChildMeterUsage = new System.Windows.Forms.Label();
            this.txtChildMeterUsage = new System.Windows.Forms.TextBox();
            this.lblUsageAmount = new System.Windows.Forms.Label();
            this.txtUsageAmount = new System.Windows.Forms.TextBox();
            this.lblUnit = new System.Windows.Forms.Label();
            this.txtUnit = new System.Windows.Forms.TextBox();
            this.lblTaxInclusiveAmount = new System.Windows.Forms.Label();
            this.txtTaxInclusiveAmount = new System.Windows.Forms.TextBox();
            this.lblTaxRate = new System.Windows.Forms.Label();
            this.txtTaxRate = new System.Windows.Forms.TextBox();
            this.lblChildMeterStartDate = new System.Windows.Forms.Label();
            this.txtChildMeterStartDate = new System.Windows.Forms.TextBox();
            this.lblChildMeterEndDate = new System.Windows.Forms.Label();
            this.txtChildMeterEndDate = new System.Windows.Forms.TextBox();
            this.lblParentMeterStartDate = new System.Windows.Forms.Label();
            this.txtParentMeterStartDate = new System.Windows.Forms.TextBox();
            this.lblParentMeterEndDate = new System.Windows.Forms.Label();
            this.txtParentMeterEndDate = new System.Windows.Forms.TextBox();
            this.lblBillingYearMonth = new System.Windows.Forms.Label();
            this.txtBillingYearMonth = new System.Windows.Forms.TextBox();
            this.lblConfirmedBillingDate = new System.Windows.Forms.Label();
            this.dtpConfirmedBillingDate = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBillingTo
            // 
            this.lblBillingTo.AutoSize = true;
            this.lblBillingTo.Location = new System.Drawing.Point(20, 20);
            this.lblBillingTo.Name = "lblBillingTo";
            this.lblBillingTo.Size = new System.Drawing.Size(70, 25);
            this.lblBillingTo.TabIndex = 0;
            this.lblBillingTo.Text = "請求先:";
            // 
            // txtBillingTo
            // 
            this.txtBillingTo.Location = new System.Drawing.Point(201, 17);
            this.txtBillingTo.Name = "txtBillingTo";
            this.txtBillingTo.Size = new System.Drawing.Size(250, 32);
            this.txtBillingTo.TabIndex = 1;
            // 
            // lblLessor
            // 
            this.lblLessor.AutoSize = true;
            this.lblLessor.Location = new System.Drawing.Point(20, 55);
            this.lblLessor.Name = "lblLessor";
            this.lblLessor.Size = new System.Drawing.Size(53, 25);
            this.lblLessor.TabIndex = 2;
            this.lblLessor.Text = "貸主:";
            // 
            // txtLessor
            // 
            this.txtLessor.Location = new System.Drawing.Point(201, 52);
            this.txtLessor.Name = "txtLessor";
            this.txtLessor.Size = new System.Drawing.Size(250, 32);
            this.txtLessor.TabIndex = 3;
            // 
            // lblBuildingName
            // 
            this.lblBuildingName.AutoSize = true;
            this.lblBuildingName.Location = new System.Drawing.Point(20, 90);
            this.lblBuildingName.Name = "lblBuildingName";
            this.lblBuildingName.Size = new System.Drawing.Size(87, 25);
            this.lblBuildingName.TabIndex = 4;
            this.lblBuildingName.Text = "建物名称:";
            // 
            // txtBuildingName
            // 
            this.txtBuildingName.Location = new System.Drawing.Point(201, 87);
            this.txtBuildingName.Name = "txtBuildingName";
            this.txtBuildingName.Size = new System.Drawing.Size(250, 32);
            this.txtBuildingName.TabIndex = 5;
            // 
            // lblLessee
            // 
            this.lblLessee.AutoSize = true;
            this.lblLessee.Location = new System.Drawing.Point(20, 125);
            this.lblLessee.Name = "lblLessee";
            this.lblLessee.Size = new System.Drawing.Size(53, 25);
            this.lblLessee.TabIndex = 6;
            this.lblLessee.Text = "借主:";
            // 
            // txtLessee
            // 
            this.txtLessee.Location = new System.Drawing.Point(201, 122);
            this.txtLessee.Name = "txtLessee";
            this.txtLessee.Size = new System.Drawing.Size(250, 32);
            this.txtLessee.TabIndex = 7;
            // 
            // lblRoomNumber
            // 
            this.lblRoomNumber.AutoSize = true;
            this.lblRoomNumber.Location = new System.Drawing.Point(20, 160);
            this.lblRoomNumber.Name = "lblRoomNumber";
            this.lblRoomNumber.Size = new System.Drawing.Size(87, 25);
            this.lblRoomNumber.TabIndex = 8;
            this.lblRoomNumber.Text = "部屋番号:";
            // 
            // txtRoomNumber
            // 
            this.txtRoomNumber.Location = new System.Drawing.Point(201, 157);
            this.txtRoomNumber.Name = "txtRoomNumber";
            this.txtRoomNumber.Size = new System.Drawing.Size(250, 32);
            this.txtRoomNumber.TabIndex = 9;
            // 
            // lblRoomArea
            // 
            this.lblRoomArea.AutoSize = true;
            this.lblRoomArea.Location = new System.Drawing.Point(20, 195);
            this.lblRoomArea.Name = "lblRoomArea";
            this.lblRoomArea.Size = new System.Drawing.Size(83, 25);
            this.lblRoomArea.TabIndex = 10;
            this.lblRoomArea.Text = "面積(m3):";
            // 
            // txtRoomArea
            // 
            this.txtRoomArea.Location = new System.Drawing.Point(201, 192);
            this.txtRoomArea.Name = "txtRoomArea";
            this.txtRoomArea.Size = new System.Drawing.Size(150, 32);
            this.txtRoomArea.TabIndex = 11;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(20, 230);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(53, 25);
            this.lblCategory.TabIndex = 12;
            this.lblCategory.Text = "種別:";
            // 
            // txtCategory
            // 
            this.txtCategory.Location = new System.Drawing.Point(201, 227);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.Size = new System.Drawing.Size(250, 32);
            this.txtCategory.TabIndex = 13;
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Location = new System.Drawing.Point(20, 265);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(53, 25);
            this.lblContent.TabIndex = 14;
            this.lblContent.Text = "内容:";
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(201, 262);
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(250, 32);
            this.txtContent.TabIndex = 15;
            // 
            // lblChildMeterUsage
            // 
            this.lblChildMeterUsage.AutoSize = true;
            this.lblChildMeterUsage.Location = new System.Drawing.Point(20, 300);
            this.lblChildMeterUsage.Name = "lblChildMeterUsage";
            this.lblChildMeterUsage.Size = new System.Drawing.Size(87, 25);
            this.lblChildMeterUsage.TabIndex = 16;
            this.lblChildMeterUsage.Text = "子使用量:";
            // 
            // txtChildMeterUsage
            // 
            this.txtChildMeterUsage.Location = new System.Drawing.Point(201, 297);
            this.txtChildMeterUsage.Name = "txtChildMeterUsage";
            this.txtChildMeterUsage.Size = new System.Drawing.Size(150, 32);
            this.txtChildMeterUsage.TabIndex = 17;
            // 
            // lblUsageAmount
            // 
            this.lblUsageAmount.AutoSize = true;
            this.lblUsageAmount.Location = new System.Drawing.Point(20, 335);
            this.lblUsageAmount.Name = "lblUsageAmount";
            this.lblUsageAmount.Size = new System.Drawing.Size(70, 25);
            this.lblUsageAmount.TabIndex = 18;
            this.lblUsageAmount.Text = "使用量:";
            // 
            // txtUsageAmount
            // 
            this.txtUsageAmount.Location = new System.Drawing.Point(201, 332);
            this.txtUsageAmount.Name = "txtUsageAmount";
            this.txtUsageAmount.Size = new System.Drawing.Size(150, 32);
            this.txtUsageAmount.TabIndex = 19;
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(20, 370);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(53, 25);
            this.lblUnit.TabIndex = 20;
            this.lblUnit.Text = "単位:";
            // 
            // txtUnit
            // 
            this.txtUnit.Location = new System.Drawing.Point(201, 367);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(150, 32);
            this.txtUnit.TabIndex = 21;
            // 
            // lblTaxInclusiveAmount
            // 
            this.lblTaxInclusiveAmount.AutoSize = true;
            this.lblTaxInclusiveAmount.Location = new System.Drawing.Point(20, 405);
            this.lblTaxInclusiveAmount.Name = "lblTaxInclusiveAmount";
            this.lblTaxInclusiveAmount.Size = new System.Drawing.Size(87, 25);
            this.lblTaxInclusiveAmount.TabIndex = 22;
            this.lblTaxInclusiveAmount.Text = "税込金額:";
            // 
            // txtTaxInclusiveAmount
            // 
            this.txtTaxInclusiveAmount.Location = new System.Drawing.Point(201, 402);
            this.txtTaxInclusiveAmount.Name = "txtTaxInclusiveAmount";
            this.txtTaxInclusiveAmount.Size = new System.Drawing.Size(150, 32);
            this.txtTaxInclusiveAmount.TabIndex = 23;
            // 
            // lblTaxRate
            // 
            this.lblTaxRate.AutoSize = true;
            this.lblTaxRate.Location = new System.Drawing.Point(20, 440);
            this.lblTaxRate.Name = "lblTaxRate";
            this.lblTaxRate.Size = new System.Drawing.Size(85, 25);
            this.lblTaxRate.TabIndex = 24;
            this.lblTaxRate.Text = "税率(%):";
            // 
            // txtTaxRate
            // 
            this.txtTaxRate.Location = new System.Drawing.Point(201, 437);
            this.txtTaxRate.Name = "txtTaxRate";
            this.txtTaxRate.Size = new System.Drawing.Size(150, 32);
            this.txtTaxRate.TabIndex = 25;
            // 
            // lblChildMeterStartDate
            // 
            this.lblChildMeterStartDate.AutoSize = true;
            this.lblChildMeterStartDate.Location = new System.Drawing.Point(20, 475);
            this.lblChildMeterStartDate.Name = "lblChildMeterStartDate";
            this.lblChildMeterStartDate.Size = new System.Drawing.Size(172, 25);
            this.lblChildMeterStartDate.TabIndex = 26;
            this.lblChildMeterStartDate.Text = "子メータ使用開始日:";
            // 
            // txtChildMeterStartDate
            // 
            this.txtChildMeterStartDate.Location = new System.Drawing.Point(201, 472);
            this.txtChildMeterStartDate.Name = "txtChildMeterStartDate";
            this.txtChildMeterStartDate.Size = new System.Drawing.Size(150, 32);
            this.txtChildMeterStartDate.TabIndex = 27;
            // 
            // lblChildMeterEndDate
            // 
            this.lblChildMeterEndDate.AutoSize = true;
            this.lblChildMeterEndDate.Location = new System.Drawing.Point(20, 510);
            this.lblChildMeterEndDate.Name = "lblChildMeterEndDate";
            this.lblChildMeterEndDate.Size = new System.Drawing.Size(172, 25);
            this.lblChildMeterEndDate.TabIndex = 28;
            this.lblChildMeterEndDate.Text = "子メータ使用終了日:";
            // 
            // txtChildMeterEndDate
            // 
            this.txtChildMeterEndDate.Location = new System.Drawing.Point(201, 507);
            this.txtChildMeterEndDate.Name = "txtChildMeterEndDate";
            this.txtChildMeterEndDate.Size = new System.Drawing.Size(150, 32);
            this.txtChildMeterEndDate.TabIndex = 29;
            // 
            // lblParentMeterStartDate
            // 
            this.lblParentMeterStartDate.AutoSize = true;
            this.lblParentMeterStartDate.Location = new System.Drawing.Point(20, 545);
            this.lblParentMeterStartDate.Name = "lblParentMeterStartDate";
            this.lblParentMeterStartDate.Size = new System.Drawing.Size(172, 25);
            this.lblParentMeterStartDate.TabIndex = 30;
            this.lblParentMeterStartDate.Text = "親メータ使用開始日:";
            // 
            // txtParentMeterStartDate
            // 
            this.txtParentMeterStartDate.Location = new System.Drawing.Point(201, 542);
            this.txtParentMeterStartDate.Name = "txtParentMeterStartDate";
            this.txtParentMeterStartDate.Size = new System.Drawing.Size(150, 32);
            this.txtParentMeterStartDate.TabIndex = 31;
            // 
            // lblParentMeterEndDate
            // 
            this.lblParentMeterEndDate.AutoSize = true;
            this.lblParentMeterEndDate.Location = new System.Drawing.Point(20, 580);
            this.lblParentMeterEndDate.Name = "lblParentMeterEndDate";
            this.lblParentMeterEndDate.Size = new System.Drawing.Size(172, 25);
            this.lblParentMeterEndDate.TabIndex = 32;
            this.lblParentMeterEndDate.Text = "親メータ使用終了日:";
            // 
            // txtParentMeterEndDate
            // 
            this.txtParentMeterEndDate.Location = new System.Drawing.Point(201, 577);
            this.txtParentMeterEndDate.Name = "txtParentMeterEndDate";
            this.txtParentMeterEndDate.Size = new System.Drawing.Size(150, 32);
            this.txtParentMeterEndDate.TabIndex = 33;
            // 
            // lblBillingYearMonth
            // 
            this.lblBillingYearMonth.AutoSize = true;
            this.lblBillingYearMonth.Location = new System.Drawing.Point(20, 615);
            this.lblBillingYearMonth.Name = "lblBillingYearMonth";
            this.lblBillingYearMonth.Size = new System.Drawing.Size(185, 25);
            this.lblBillingYearMonth.TabIndex = 34;
            this.lblBillingYearMonth.Text = "請求年月 (YYYY-MM):";
            // 
            // txtBillingYearMonth
            // 
            this.txtBillingYearMonth.Location = new System.Drawing.Point(201, 610);
            this.txtBillingYearMonth.Name = "txtBillingYearMonth";
            this.txtBillingYearMonth.Size = new System.Drawing.Size(150, 32);
            this.txtBillingYearMonth.TabIndex = 35;
            // 
            // lblConfirmedBillingDate
            // 
            this.lblConfirmedBillingDate.AutoSize = true;
            this.lblConfirmedBillingDate.Location = new System.Drawing.Point(20, 653);
            this.lblConfirmedBillingDate.Name = "lblConfirmedBillingDate";
            this.lblConfirmedBillingDate.Size = new System.Drawing.Size(104, 25);
            this.lblConfirmedBillingDate.TabIndex = 36;
            this.lblConfirmedBillingDate.Text = "請求予定日:";
            // 
            // dtpConfirmedBillingDate
            // 
            this.dtpConfirmedBillingDate.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpConfirmedBillingDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpConfirmedBillingDate.Location = new System.Drawing.Point(201, 648);
            this.dtpConfirmedBillingDate.Name = "dtpConfirmedBillingDate";
            this.dtpConfirmedBillingDate.ShowCheckBox = true;
            this.dtpConfirmedBillingDate.Size = new System.Drawing.Size(200, 32);
            this.dtpConfirmedBillingDate.TabIndex = 37;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(500, 691);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 30);
            this.btnSave.TabIndex = 38;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(590, 691);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 39;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // InvoiceDetailEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 734);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dtpConfirmedBillingDate);
            this.Controls.Add(this.lblConfirmedBillingDate);
            this.Controls.Add(this.txtBillingYearMonth);
            this.Controls.Add(this.lblBillingYearMonth);
            this.Controls.Add(this.txtParentMeterEndDate);
            this.Controls.Add(this.lblParentMeterEndDate);
            this.Controls.Add(this.txtParentMeterStartDate);
            this.Controls.Add(this.lblParentMeterStartDate);
            this.Controls.Add(this.txtChildMeterEndDate);
            this.Controls.Add(this.lblChildMeterEndDate);
            this.Controls.Add(this.txtChildMeterStartDate);
            this.Controls.Add(this.lblChildMeterStartDate);
            this.Controls.Add(this.txtTaxRate);
            this.Controls.Add(this.lblTaxRate);
            this.Controls.Add(this.txtTaxInclusiveAmount);
            this.Controls.Add(this.lblTaxInclusiveAmount);
            this.Controls.Add(this.txtUnit);
            this.Controls.Add(this.lblUnit);
            this.Controls.Add(this.txtUsageAmount);
            this.Controls.Add(this.lblUsageAmount);
            this.Controls.Add(this.txtChildMeterUsage);
            this.Controls.Add(this.lblChildMeterUsage);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.txtCategory);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.txtRoomArea);
            this.Controls.Add(this.lblRoomArea);
            this.Controls.Add(this.txtRoomNumber);
            this.Controls.Add(this.lblRoomNumber);
            this.Controls.Add(this.txtLessee);
            this.Controls.Add(this.lblLessee);
            this.Controls.Add(this.txtBuildingName);
            this.Controls.Add(this.lblBuildingName);
            this.Controls.Add(this.txtLessor);
            this.Controls.Add(this.lblLessor);
            this.Controls.Add(this.txtBillingTo);
            this.Controls.Add(this.lblBillingTo);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "InvoiceDetailEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "請求明細登録";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
