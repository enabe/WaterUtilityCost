namespace WaterUtilityCost.Forms
{
    partial class ClientForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblClientTarget;
        private System.Windows.Forms.CheckBox chkIsLessor;
        private System.Windows.Forms.CheckBox chkIsLessee;
        private System.Windows.Forms.CheckBox chkIsBillingTo;
        private System.Windows.Forms.CheckBox chkIsContractor;
        private System.Windows.Forms.CheckBox chkIsAutoTransfer;
        private System.Windows.Forms.CheckBox chkIsBankTransfer;
        private System.Windows.Forms.Label lblTransferMethod;
        private System.Windows.Forms.Label lblBuildingName;
        private System.Windows.Forms.ComboBox cmbBuildingName;
        private System.Windows.Forms.Label lblRoomName;
        private System.Windows.Forms.ComboBox cmbRoomName;
        private System.Windows.Forms.Label lblPostalCode;
        private System.Windows.Forms.TextBox txtPostalCode;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblInvoiceNumber;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
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
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblClientTarget = new System.Windows.Forms.Label();
            this.chkIsLessor = new System.Windows.Forms.CheckBox();
            this.chkIsLessee = new System.Windows.Forms.CheckBox();
            this.chkIsBillingTo = new System.Windows.Forms.CheckBox();
            this.chkIsContractor = new System.Windows.Forms.CheckBox();
            this.chkIsAutoTransfer = new System.Windows.Forms.CheckBox();
            this.chkIsBankTransfer = new System.Windows.Forms.CheckBox();
            this.lblTransferMethod = new System.Windows.Forms.Label();
            this.lblBuildingName = new System.Windows.Forms.Label();
            this.cmbBuildingName = new System.Windows.Forms.ComboBox();
            this.lblRoomName = new System.Windows.Forms.Label();
            this.cmbRoomName = new System.Windows.Forms.ComboBox();
            this.lblPostalCode = new System.Windows.Forms.Label();
            this.txtPostalCode = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblInvoiceNumber = new System.Windows.Forms.Label();
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(87, 25);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "取引先名:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(163, 17);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 32);
            this.txtName.TabIndex = 1;
            // 
            // lblClientTarget
            // 
            this.lblClientTarget.AutoSize = true;
            this.lblClientTarget.Location = new System.Drawing.Point(20, 50);
            this.lblClientTarget.Name = "lblClientTarget";
            this.lblClientTarget.Size = new System.Drawing.Size(104, 25);
            this.lblClientTarget.TabIndex = 2;
            this.lblClientTarget.Text = "取引先対象:";
            // 
            // chkIsLessor
            // 
            this.chkIsLessor.AutoSize = true;
            this.chkIsLessor.Location = new System.Drawing.Point(163, 48);
            this.chkIsLessor.Name = "chkIsLessor";
            this.chkIsLessor.Size = new System.Drawing.Size(68, 29);
            this.chkIsLessor.TabIndex = 2;
            this.chkIsLessor.Text = "貸主";
            this.chkIsLessor.UseVisualStyleBackColor = true;
            // 
            // chkIsLessee
            // 
            this.chkIsLessee.AutoSize = true;
            this.chkIsLessee.Location = new System.Drawing.Point(253, 48);
            this.chkIsLessee.Name = "chkIsLessee";
            this.chkIsLessee.Size = new System.Drawing.Size(68, 29);
            this.chkIsLessee.TabIndex = 3;
            this.chkIsLessee.Text = "借主";
            this.chkIsLessee.UseVisualStyleBackColor = true;
            // 
            // chkIsBillingTo
            // 
            this.chkIsBillingTo.AutoSize = true;
            this.chkIsBillingTo.Location = new System.Drawing.Point(343, 48);
            this.chkIsBillingTo.Name = "chkIsBillingTo";
            this.chkIsBillingTo.Size = new System.Drawing.Size(85, 29);
            this.chkIsBillingTo.TabIndex = 4;
            this.chkIsBillingTo.Text = "請求先";
            this.chkIsBillingTo.UseVisualStyleBackColor = true;
            // 
            // chkIsContractor
            // 
            this.chkIsContractor.AutoSize = true;
            this.chkIsContractor.Location = new System.Drawing.Point(440, 48);
            this.chkIsContractor.Name = "chkIsContractor";
            this.chkIsContractor.Size = new System.Drawing.Size(68, 29);
            this.chkIsContractor.TabIndex = 5;
            this.chkIsContractor.Text = "業者";
            this.chkIsContractor.UseVisualStyleBackColor = true;
            // 
            // chkIsAutoTransfer
            // 
            this.chkIsAutoTransfer.AutoSize = true;
            this.chkIsAutoTransfer.Location = new System.Drawing.Point(163, 240);
            this.chkIsAutoTransfer.Name = "chkIsAutoTransfer";
            this.chkIsAutoTransfer.Size = new System.Drawing.Size(102, 29);
            this.chkIsAutoTransfer.TabIndex = 12;
            this.chkIsAutoTransfer.Text = "自動振込";
            this.chkIsAutoTransfer.UseVisualStyleBackColor = true;
            // 
            // chkIsBankTransfer
            // 
            this.chkIsBankTransfer.AutoSize = true;
            this.chkIsBankTransfer.Location = new System.Drawing.Point(283, 240);
            this.chkIsBankTransfer.Name = "chkIsBankTransfer";
            this.chkIsBankTransfer.Size = new System.Drawing.Size(102, 29);
            this.chkIsBankTransfer.TabIndex = 13;
            this.chkIsBankTransfer.Text = "口座振込";
            this.chkIsBankTransfer.UseVisualStyleBackColor = true;
            // 
            // lblTransferMethod
            // 
            this.lblTransferMethod.AutoSize = true;
            this.lblTransferMethod.Location = new System.Drawing.Point(20, 240);
            this.lblTransferMethod.Name = "lblTransferMethod";
            this.lblTransferMethod.Size = new System.Drawing.Size(104, 25);
            this.lblTransferMethod.TabIndex = 16;
            this.lblTransferMethod.Text = "振込方法:";
            // 
            // lblBuildingName
            // 
            this.lblBuildingName.AutoSize = true;
            this.lblBuildingName.Location = new System.Drawing.Point(20, 80);
            this.lblBuildingName.Name = "lblBuildingName";
            this.lblBuildingName.Size = new System.Drawing.Size(68, 25);
            this.lblBuildingName.TabIndex = 6;
            this.lblBuildingName.Text = "ビル名:";
            // 
            // cmbBuildingName
            // 
            this.cmbBuildingName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildingName.FormattingEnabled = true;
            this.cmbBuildingName.Location = new System.Drawing.Point(163, 77);
            this.cmbBuildingName.Name = "cmbBuildingName";
            this.cmbBuildingName.Size = new System.Drawing.Size(200, 33);
            this.cmbBuildingName.TabIndex = 6;
            // 
            // lblRoomName
            // 
            this.lblRoomName.AutoSize = true;
            this.lblRoomName.Location = new System.Drawing.Point(20, 110);
            this.lblRoomName.Name = "lblRoomName";
            this.lblRoomName.Size = new System.Drawing.Size(68, 25);
            this.lblRoomName.TabIndex = 7;
            this.lblRoomName.Text = "部屋名:";
            // 
            // cmbRoomName
            // 
            this.cmbRoomName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoomName.Enabled = false;
            this.cmbRoomName.FormattingEnabled = true;
            this.cmbRoomName.Location = new System.Drawing.Point(163, 107);
            this.cmbRoomName.Name = "cmbRoomName";
            this.cmbRoomName.Size = new System.Drawing.Size(200, 33);
            this.cmbRoomName.TabIndex = 7;
            // 
            // lblPostalCode
            // 
            this.lblPostalCode.AutoSize = true;
            this.lblPostalCode.Location = new System.Drawing.Point(20, 140);
            this.lblPostalCode.Name = "lblPostalCode";
            this.lblPostalCode.Size = new System.Drawing.Size(87, 25);
            this.lblPostalCode.TabIndex = 8;
            this.lblPostalCode.Text = "郵便番号:";
            // 
            // txtPostalCode
            // 
            this.txtPostalCode.Location = new System.Drawing.Point(163, 137);
            this.txtPostalCode.Name = "txtPostalCode";
            this.txtPostalCode.Size = new System.Drawing.Size(150, 32);
            this.txtPostalCode.TabIndex = 8;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(20, 170);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(53, 25);
            this.lblAddress.TabIndex = 9;
            this.lblAddress.Text = "住所:";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(163, 167);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(400, 32);
            this.txtAddress.TabIndex = 9;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(20, 200);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(87, 25);
            this.lblPhone.TabIndex = 10;
            this.lblPhone.Text = "電話番号:";
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(163, 197);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(200, 32);
            this.txtPhone.TabIndex = 10;
            // 
            // lblInvoiceNumber
            // 
            this.lblInvoiceNumber.AutoSize = true;
            this.lblInvoiceNumber.Location = new System.Drawing.Point(20, 230);
            this.lblInvoiceNumber.Name = "lblInvoiceNumber";
            this.lblInvoiceNumber.Size = new System.Drawing.Size(138, 25);
            this.lblInvoiceNumber.TabIndex = 11;
            this.lblInvoiceNumber.Text = "インボイス番号:";
            this.lblInvoiceNumber.Visible = false;
            // 
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.Location = new System.Drawing.Point(163, 227);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(200, 32);
            this.txtInvoiceNumber.TabIndex = 11;
            this.txtInvoiceNumber.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(350, 300);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(440, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 380);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblTransferMethod);
            this.Controls.Add(this.chkIsBankTransfer);
            this.Controls.Add(this.chkIsAutoTransfer);
            this.Controls.Add(this.txtInvoiceNumber);
            this.Controls.Add(this.lblInvoiceNumber);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtPostalCode);
            this.Controls.Add(this.lblPostalCode);
            this.Controls.Add(this.cmbRoomName);
            this.Controls.Add(this.lblRoomName);
            this.Controls.Add(this.cmbBuildingName);
            this.Controls.Add(this.lblBuildingName);
            this.Controls.Add(this.chkIsContractor);
            this.Controls.Add(this.chkIsBillingTo);
            this.Controls.Add(this.chkIsLessee);
            this.Controls.Add(this.chkIsLessor);
            this.Controls.Add(this.lblClientTarget);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "ClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "取引先情報登録";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

