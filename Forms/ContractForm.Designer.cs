namespace WaterUtilityCost.Forms
{
    partial class ContractForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // コントロールのフィールド定義（デザイナーで表示・編集可能にするため）
        private System.Windows.Forms.Label lblContractNumber;
        private System.Windows.Forms.TextBox txtContractNumber;
        private System.Windows.Forms.Label lblContractType;
        private System.Windows.Forms.TextBox txtContractType;
        private System.Windows.Forms.Label lblContractorName;
        private System.Windows.Forms.TextBox txtContractorName;
        private System.Windows.Forms.Label lblLessorClient;
        private System.Windows.Forms.ComboBox cmbLessorClient;
        private System.Windows.Forms.Label lblLesseeClient;
        private System.Windows.Forms.ComboBox cmbLesseeClient;
        private System.Windows.Forms.Label lblBillingClient;
        private System.Windows.Forms.ComboBox cmbBillingClient;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblContractStatus;
        private System.Windows.Forms.ComboBox cmbContractStatus;
        private System.Windows.Forms.Label lblClosingDate;
        private System.Windows.Forms.ComboBox cmbClosingDate;
        private System.Windows.Forms.Label lblBuilding;
        private System.Windows.Forms.ComboBox cmbBuilding;
        private System.Windows.Forms.Label lblCustomerNumber;
        private System.Windows.Forms.TextBox txtCustomerNumber;
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
            this.lblContractNumber = new System.Windows.Forms.Label();
            this.txtContractNumber = new System.Windows.Forms.TextBox();
            this.lblContractType = new System.Windows.Forms.Label();
            this.txtContractType = new System.Windows.Forms.TextBox();
            this.lblContractorName = new System.Windows.Forms.Label();
            this.txtContractorName = new System.Windows.Forms.TextBox();
            this.lblLessorClient = new System.Windows.Forms.Label();
            this.cmbLessorClient = new System.Windows.Forms.ComboBox();
            this.lblLesseeClient = new System.Windows.Forms.Label();
            this.cmbLesseeClient = new System.Windows.Forms.ComboBox();
            this.lblBillingClient = new System.Windows.Forms.Label();
            this.cmbBillingClient = new System.Windows.Forms.ComboBox();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblContractStatus = new System.Windows.Forms.Label();
            this.cmbContractStatus = new System.Windows.Forms.ComboBox();
            this.lblClosingDate = new System.Windows.Forms.Label();
            this.cmbClosingDate = new System.Windows.Forms.ComboBox();
            this.lblBuilding = new System.Windows.Forms.Label();
            this.cmbBuilding = new System.Windows.Forms.ComboBox();
            this.lblCustomerNumber = new System.Windows.Forms.Label();
            this.txtCustomerNumber = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblContractNumber
            // 
            this.lblContractNumber.AutoSize = true;
            this.lblContractNumber.Location = new System.Drawing.Point(20, 20);
            this.lblContractNumber.Name = "lblContractNumber";
            this.lblContractNumber.Size = new System.Drawing.Size(65, 12);
            this.lblContractNumber.TabIndex = 0;
            this.lblContractNumber.Text = "契約番号:";
            // 
            // txtContractNumber
            // 
            this.txtContractNumber.Location = new System.Drawing.Point(180, 17);
            this.txtContractNumber.Name = "txtContractNumber";
            this.txtContractNumber.Size = new System.Drawing.Size(350, 19);
            this.txtContractNumber.TabIndex = 1;
            // 
            // lblContractType
            // 
            this.lblContractType.AutoSize = true;
            this.lblContractType.Location = new System.Drawing.Point(20, 55);
            this.lblContractType.Name = "lblContractType";
            this.lblContractType.Size = new System.Drawing.Size(65, 12);
            this.lblContractType.TabIndex = 2;
            this.lblContractType.Text = "契約種別:";
            // 
            // txtContractType
            // 
            this.txtContractType.Location = new System.Drawing.Point(180, 52);
            this.txtContractType.Name = "txtContractType";
            this.txtContractType.Size = new System.Drawing.Size(350, 19);
            this.txtContractType.TabIndex = 3;
            // 
            // lblContractorName
            // 
            this.lblContractorName.AutoSize = true;
            this.lblContractorName.Location = new System.Drawing.Point(20, 90);
            this.lblContractorName.Name = "lblContractorName";
            this.lblContractorName.Size = new System.Drawing.Size(65, 12);
            this.lblContractorName.TabIndex = 4;
            this.lblContractorName.Text = "契約者名:";
            // 
            // txtContractorName
            // 
            this.txtContractorName.Location = new System.Drawing.Point(180, 87);
            this.txtContractorName.Name = "txtContractorName";
            this.txtContractorName.Size = new System.Drawing.Size(350, 19);
            this.txtContractorName.TabIndex = 5;
            // 
            // lblLessorClient
            // 
            this.lblLessorClient.AutoSize = true;
            this.lblLessorClient.Location = new System.Drawing.Point(20, 125);
            this.lblLessorClient.Name = "lblLessorClient";
            this.lblLessorClient.Size = new System.Drawing.Size(77, 12);
            this.lblLessorClient.TabIndex = 6;
            this.lblLessorClient.Text = "貸主取引先:";
            // 
            // cmbLessorClient
            // 
            this.cmbLessorClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLessorClient.FormattingEnabled = true;
            this.cmbLessorClient.Location = new System.Drawing.Point(180, 122);
            this.cmbLessorClient.Name = "cmbLessorClient";
            this.cmbLessorClient.Size = new System.Drawing.Size(350, 20);
            this.cmbLessorClient.TabIndex = 7;
            // 
            // lblLesseeClient
            // 
            this.lblLesseeClient.AutoSize = true;
            this.lblLesseeClient.Location = new System.Drawing.Point(20, 160);
            this.lblLesseeClient.Name = "lblLesseeClient";
            this.lblLesseeClient.Size = new System.Drawing.Size(77, 12);
            this.lblLesseeClient.TabIndex = 8;
            this.lblLesseeClient.Text = "借主取引先:";
            // 
            // cmbLesseeClient
            // 
            this.cmbLesseeClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLesseeClient.FormattingEnabled = true;
            this.cmbLesseeClient.Location = new System.Drawing.Point(180, 157);
            this.cmbLesseeClient.Name = "cmbLesseeClient";
            this.cmbLesseeClient.Size = new System.Drawing.Size(350, 20);
            this.cmbLesseeClient.TabIndex = 9;
            // 
            // lblBillingClient
            // 
            this.lblBillingClient.AutoSize = true;
            this.lblBillingClient.Location = new System.Drawing.Point(20, 195);
            this.lblBillingClient.Name = "lblBillingClient";
            this.lblBillingClient.Size = new System.Drawing.Size(77, 12);
            this.lblBillingClient.TabIndex = 10;
            this.lblBillingClient.Text = "請求取引先:";
            // 
            // cmbBillingClient
            // 
            this.cmbBillingClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBillingClient.FormattingEnabled = true;
            this.cmbBillingClient.Location = new System.Drawing.Point(180, 192);
            this.cmbBillingClient.Name = "cmbBillingClient";
            this.cmbBillingClient.Size = new System.Drawing.Size(350, 20);
            this.cmbBillingClient.TabIndex = 11;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(20, 230);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(77, 12);
            this.lblStartDate.TabIndex = 12;
            this.lblStartDate.Text = "対象開始日:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(180, 227);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.ShowCheckBox = true;
            this.dtpStartDate.Size = new System.Drawing.Size(350, 19);
            this.dtpStartDate.TabIndex = 13;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(20, 265);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(77, 12);
            this.lblEndDate.TabIndex = 14;
            this.lblEndDate.Text = "対象終了日:";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(180, 262);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.ShowCheckBox = true;
            this.dtpEndDate.Size = new System.Drawing.Size(350, 19);
            this.dtpEndDate.TabIndex = 15;
            // 
            // lblContractStatus
            // 
            this.lblContractStatus.AutoSize = true;
            this.lblContractStatus.Location = new System.Drawing.Point(20, 300);
            this.lblContractStatus.Name = "lblContractStatus";
            this.lblContractStatus.Size = new System.Drawing.Size(65, 12);
            this.lblContractStatus.TabIndex = 16;
            this.lblContractStatus.Text = "契約状況:";
            // 
            // cmbContractStatus
            // 
            this.cmbContractStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbContractStatus.FormattingEnabled = true;
            this.cmbContractStatus.Items.AddRange(new object[] {
            "意向確認中",
            "契約書送付待ち",
            "契約書送付済み",
            "契約書返信済み",
            "契約終了",
            "途中解約"});
            this.cmbContractStatus.Location = new System.Drawing.Point(180, 297);
            this.cmbContractStatus.Name = "cmbContractStatus";
            this.cmbContractStatus.Size = new System.Drawing.Size(350, 20);
            this.cmbContractStatus.TabIndex = 17;
            // 
            // lblClosingDate
            // 
            this.lblClosingDate.AutoSize = true;
            this.lblClosingDate.Location = new System.Drawing.Point(20, 335);
            this.lblClosingDate.Name = "lblClosingDate";
            this.lblClosingDate.Size = new System.Drawing.Size(41, 12);
            this.lblClosingDate.TabIndex = 18;
            this.lblClosingDate.Text = "締日:";
            // 
            // cmbClosingDate
            // 
            this.cmbClosingDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClosingDate.FormattingEnabled = true;
            this.cmbClosingDate.Location = new System.Drawing.Point(180, 332);
            this.cmbClosingDate.Name = "cmbClosingDate";
            this.cmbClosingDate.Size = new System.Drawing.Size(350, 20);
            this.cmbClosingDate.TabIndex = 19;
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.Location = new System.Drawing.Point(20, 370);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(41, 12);
            this.lblBuilding.TabIndex = 20;
            this.lblBuilding.Text = "ビル名:";
            // 
            // cmbBuilding
            // 
            this.cmbBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuilding.FormattingEnabled = true;
            this.cmbBuilding.Location = new System.Drawing.Point(180, 367);
            this.cmbBuilding.Name = "cmbBuilding";
            this.cmbBuilding.Size = new System.Drawing.Size(350, 20);
            this.cmbBuilding.TabIndex = 21;
            // 
            // lblCustomerNumber
            // 
            this.lblCustomerNumber.AutoSize = true;
            this.lblCustomerNumber.Location = new System.Drawing.Point(20, 405);
            this.lblCustomerNumber.Name = "lblCustomerNumber";
            this.lblCustomerNumber.Size = new System.Drawing.Size(77, 12);
            this.lblCustomerNumber.TabIndex = 22;
            this.lblCustomerNumber.Text = "お客様番号:";
            // 
            // txtCustomerNumber
            // 
            this.txtCustomerNumber.Location = new System.Drawing.Point(180, 402);
            this.txtCustomerNumber.Name = "txtCustomerNumber";
            this.txtCustomerNumber.Size = new System.Drawing.Size(350, 19);
            this.txtCustomerNumber.TabIndex = 23;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(180, 450);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 24;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(280, 450);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ContractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ClientSize = new System.Drawing.Size(600, 700);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtCustomerNumber);
            this.Controls.Add(this.lblCustomerNumber);
            this.Controls.Add(this.cmbBuilding);
            this.Controls.Add(this.lblBuilding);
            this.Controls.Add(this.cmbClosingDate);
            this.Controls.Add(this.lblClosingDate);
            this.Controls.Add(this.cmbContractStatus);
            this.Controls.Add(this.lblContractStatus);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.cmbBillingClient);
            this.Controls.Add(this.lblBillingClient);
            this.Controls.Add(this.cmbLesseeClient);
            this.Controls.Add(this.lblLesseeClient);
            this.Controls.Add(this.cmbLessorClient);
            this.Controls.Add(this.lblLessorClient);
            this.Controls.Add(this.txtContractorName);
            this.Controls.Add(this.lblContractorName);
            this.Controls.Add(this.txtContractType);
            this.Controls.Add(this.lblContractType);
            this.Controls.Add(this.txtContractNumber);
            this.Controls.Add(this.lblContractNumber);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContractForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "契約情報登録";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

