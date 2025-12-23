namespace WaterUtilityCost.Forms
{
    partial class OtherInvoiceDetailEditForm
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
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblTaxInclusiveAmount;
        private System.Windows.Forms.TextBox txtTaxInclusiveAmount;
        private System.Windows.Forms.Label lblTaxRate;
        private System.Windows.Forms.TextBox txtTaxRate;
        private System.Windows.Forms.Label lblContractor;
        private System.Windows.Forms.ComboBox cmbContractor;
        private System.Windows.Forms.Label lblInvoiceNumber;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
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
            this.lblCategory = new System.Windows.Forms.Label();
            this.txtCategory = new System.Windows.Forms.TextBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblTaxInclusiveAmount = new System.Windows.Forms.Label();
            this.txtTaxInclusiveAmount = new System.Windows.Forms.TextBox();
            this.lblTaxRate = new System.Windows.Forms.Label();
            this.txtTaxRate = new System.Windows.Forms.TextBox();
            this.lblContractor = new System.Windows.Forms.Label();
            this.cmbContractor = new System.Windows.Forms.ComboBox();
            this.lblInvoiceNumber = new System.Windows.Forms.Label();
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.lblConfirmedBillingDate = new System.Windows.Forms.Label();
            this.dtpConfirmedBillingDate = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBillingTo
            // 
            this.lblBillingTo.AutoSize = true;
            this.lblBillingTo.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBillingTo.Location = new System.Drawing.Point(20, 20);
            this.lblBillingTo.Name = "lblBillingTo";
            this.lblBillingTo.Size = new System.Drawing.Size(69, 25);
            this.lblBillingTo.TabIndex = 0;
            this.lblBillingTo.Text = "請求先:";
            // 
            // txtBillingTo
            // 
            this.txtBillingTo.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtBillingTo.Location = new System.Drawing.Point(150, 17);
            this.txtBillingTo.Name = "txtBillingTo";
            this.txtBillingTo.Size = new System.Drawing.Size(350, 32);
            this.txtBillingTo.TabIndex = 1;
            // 
            // lblLessor
            // 
            this.lblLessor.AutoSize = true;
            this.lblLessor.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblLessor.Location = new System.Drawing.Point(20, 60);
            this.lblLessor.Name = "lblLessor";
            this.lblLessor.Size = new System.Drawing.Size(55, 25);
            this.lblLessor.TabIndex = 2;
            this.lblLessor.Text = "貸主:";
            // 
            // txtLessor
            // 
            this.txtLessor.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtLessor.Location = new System.Drawing.Point(150, 57);
            this.txtLessor.Name = "txtLessor";
            this.txtLessor.Size = new System.Drawing.Size(350, 32);
            this.txtLessor.TabIndex = 3;
            // 
            // lblBuildingName
            // 
            this.lblBuildingName.AutoSize = true;
            this.lblBuildingName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBuildingName.Location = new System.Drawing.Point(20, 100);
            this.lblBuildingName.Name = "lblBuildingName";
            this.lblBuildingName.Size = new System.Drawing.Size(87, 25);
            this.lblBuildingName.TabIndex = 4;
            this.lblBuildingName.Text = "建物名称:";
            // 
            // txtBuildingName
            // 
            this.txtBuildingName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtBuildingName.Location = new System.Drawing.Point(150, 97);
            this.txtBuildingName.Name = "txtBuildingName";
            this.txtBuildingName.Size = new System.Drawing.Size(350, 32);
            this.txtBuildingName.TabIndex = 5;
            // 
            // lblLessee
            // 
            this.lblLessee.AutoSize = true;
            this.lblLessee.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblLessee.Location = new System.Drawing.Point(20, 140);
            this.lblLessee.Name = "lblLessee";
            this.lblLessee.Size = new System.Drawing.Size(55, 25);
            this.lblLessee.TabIndex = 6;
            this.lblLessee.Text = "借主:";
            // 
            // txtLessee
            // 
            this.txtLessee.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtLessee.Location = new System.Drawing.Point(150, 137);
            this.txtLessee.Name = "txtLessee";
            this.txtLessee.Size = new System.Drawing.Size(350, 32);
            this.txtLessee.TabIndex = 7;
            // 
            // lblRoomNumber
            // 
            this.lblRoomNumber.AutoSize = true;
            this.lblRoomNumber.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblRoomNumber.Location = new System.Drawing.Point(20, 180);
            this.lblRoomNumber.Name = "lblRoomNumber";
            this.lblRoomNumber.Size = new System.Drawing.Size(83, 25);
            this.lblRoomNumber.TabIndex = 8;
            this.lblRoomNumber.Text = "部屋番号:";
            // 
            // txtRoomNumber
            // 
            this.txtRoomNumber.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtRoomNumber.Location = new System.Drawing.Point(150, 177);
            this.txtRoomNumber.Name = "txtRoomNumber";
            this.txtRoomNumber.Size = new System.Drawing.Size(350, 32);
            this.txtRoomNumber.TabIndex = 9;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblCategory.Location = new System.Drawing.Point(20, 220);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(55, 25);
            this.lblCategory.TabIndex = 10;
            this.lblCategory.Text = "種別:";
            // 
            // txtCategory
            // 
            this.txtCategory.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtCategory.Location = new System.Drawing.Point(150, 217);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.Size = new System.Drawing.Size(350, 32);
            this.txtCategory.TabIndex = 11;
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblContent.Location = new System.Drawing.Point(20, 260);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(55, 25);
            this.lblContent.TabIndex = 12;
            this.lblContent.Text = "内容:";
            // 
            // txtContent
            // 
            this.txtContent.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtContent.Location = new System.Drawing.Point(150, 257);
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(350, 32);
            this.txtContent.TabIndex = 13;
            // 
            // lblTaxInclusiveAmount
            // 
            this.lblTaxInclusiveAmount.AutoSize = true;
            this.lblTaxInclusiveAmount.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTaxInclusiveAmount.Location = new System.Drawing.Point(20, 300);
            this.lblTaxInclusiveAmount.Name = "lblTaxInclusiveAmount";
            this.lblTaxInclusiveAmount.Size = new System.Drawing.Size(87, 25);
            this.lblTaxInclusiveAmount.TabIndex = 14;
            this.lblTaxInclusiveAmount.Text = "税込金額:";
            // 
            // txtTaxInclusiveAmount
            // 
            this.txtTaxInclusiveAmount.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtTaxInclusiveAmount.Location = new System.Drawing.Point(150, 297);
            this.txtTaxInclusiveAmount.Name = "txtTaxInclusiveAmount";
            this.txtTaxInclusiveAmount.Size = new System.Drawing.Size(350, 32);
            this.txtTaxInclusiveAmount.TabIndex = 15;
            // 
            // lblTaxRate
            // 
            this.lblTaxRate.AutoSize = true;
            this.lblTaxRate.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTaxRate.Location = new System.Drawing.Point(20, 340);
            this.lblTaxRate.Name = "lblTaxRate";
            this.lblTaxRate.Size = new System.Drawing.Size(55, 25);
            this.lblTaxRate.TabIndex = 16;
            this.lblTaxRate.Text = "税率:";
            // 
            // txtTaxRate
            // 
            this.txtTaxRate.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtTaxRate.Location = new System.Drawing.Point(150, 337);
            this.txtTaxRate.Name = "txtTaxRate";
            this.txtTaxRate.Size = new System.Drawing.Size(350, 32);
            this.txtTaxRate.TabIndex = 17;
            // 
            // lblContractor
            // 
            this.lblContractor.AutoSize = true;
            this.lblContractor.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblContractor.Location = new System.Drawing.Point(20, 380);
            this.lblContractor.Name = "lblContractor";
            this.lblContractor.Size = new System.Drawing.Size(55, 25);
            this.lblContractor.TabIndex = 18;
            this.lblContractor.Text = "業者:";
            // 
            // cmbContractor
            // 
            this.cmbContractor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbContractor.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbContractor.FormattingEnabled = true;
            this.cmbContractor.Location = new System.Drawing.Point(150, 377);
            this.cmbContractor.Name = "cmbContractor";
            this.cmbContractor.Size = new System.Drawing.Size(350, 33);
            this.cmbContractor.TabIndex = 19;
            // 
            // lblInvoiceNumber
            // 
            this.lblInvoiceNumber.AutoSize = true;
            this.lblInvoiceNumber.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblInvoiceNumber.Location = new System.Drawing.Point(20, 420);
            this.lblInvoiceNumber.Name = "lblInvoiceNumber";
            this.lblInvoiceNumber.Size = new System.Drawing.Size(111, 25);
            this.lblInvoiceNumber.TabIndex = 20;
            this.lblInvoiceNumber.Text = "インボイス番号:";
            // 
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtInvoiceNumber.Location = new System.Drawing.Point(150, 417);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.ReadOnly = true;
            this.txtInvoiceNumber.Size = new System.Drawing.Size(350, 32);
            this.txtInvoiceNumber.TabIndex = 21;
            // 
            // lblConfirmedBillingDate
            // 
            this.lblConfirmedBillingDate.AutoSize = true;
            this.lblConfirmedBillingDate.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblConfirmedBillingDate.Location = new System.Drawing.Point(20, 460);
            this.lblConfirmedBillingDate.Name = "lblConfirmedBillingDate";
            this.lblConfirmedBillingDate.Size = new System.Drawing.Size(111, 25);
            this.lblConfirmedBillingDate.TabIndex = 22;
            this.lblConfirmedBillingDate.Text = "決定請求日:";
            // 
            // dtpConfirmedBillingDate
            // 
            this.dtpConfirmedBillingDate.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpConfirmedBillingDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpConfirmedBillingDate.Location = new System.Drawing.Point(150, 457);
            this.dtpConfirmedBillingDate.Name = "dtpConfirmedBillingDate";
            this.dtpConfirmedBillingDate.ShowCheckBox = true;
            this.dtpConfirmedBillingDate.Size = new System.Drawing.Size(200, 32);
            this.dtpConfirmedBillingDate.TabIndex = 23;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnSave.Location = new System.Drawing.Point(150, 510);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.TabIndex = 24;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(260, 510);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // OtherInvoiceDetailEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 570);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dtpConfirmedBillingDate);
            this.Controls.Add(this.lblConfirmedBillingDate);
            this.Controls.Add(this.txtInvoiceNumber);
            this.Controls.Add(this.lblInvoiceNumber);
            this.Controls.Add(this.cmbContractor);
            this.Controls.Add(this.lblContractor);
            this.Controls.Add(this.txtTaxRate);
            this.Controls.Add(this.lblTaxRate);
            this.Controls.Add(this.txtTaxInclusiveAmount);
            this.Controls.Add(this.lblTaxInclusiveAmount);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.txtCategory);
            this.Controls.Add(this.lblCategory);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OtherInvoiceDetailEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "その他請求明細登録";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

