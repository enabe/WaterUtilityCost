namespace WaterUtilityCost.Forms
{
    partial class MeterForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBuilding;
        private System.Windows.Forms.ComboBox cmbBuilding;
        private System.Windows.Forms.Label lblMeterType;
        private System.Windows.Forms.ComboBox cmbMeterType;
        private System.Windows.Forms.Label lblMeterName;
        private System.Windows.Forms.TextBox txtMeterName;
        private System.Windows.Forms.Label lblManagementNumber;
        private System.Windows.Forms.TextBox txtManagementNumber;
        private System.Windows.Forms.Label lblContractor;
        private System.Windows.Forms.ComboBox cmbContractor;
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
            this.lblBuilding = new System.Windows.Forms.Label();
            this.cmbBuilding = new System.Windows.Forms.ComboBox();
            this.lblMeterType = new System.Windows.Forms.Label();
            this.cmbMeterType = new System.Windows.Forms.ComboBox();
            this.lblMeterName = new System.Windows.Forms.Label();
            this.txtMeterName = new System.Windows.Forms.TextBox();
            this.lblManagementNumber = new System.Windows.Forms.Label();
            this.txtManagementNumber = new System.Windows.Forms.TextBox();
            this.lblContractor = new System.Windows.Forms.Label();
            this.cmbContractor = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.Location = new System.Drawing.Point(20, 20);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(29, 12);
            this.lblBuilding.TabIndex = 0;
            this.lblBuilding.Text = "ビル名:";
            // 
            // cmbBuilding
            // 
            this.cmbBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuilding.FormattingEnabled = true;
            this.cmbBuilding.Location = new System.Drawing.Point(150, 17);
            this.cmbBuilding.Name = "cmbBuilding";
            this.cmbBuilding.Size = new System.Drawing.Size(350, 20);
            this.cmbBuilding.TabIndex = 1;
            // 
            // lblMeterType
            // 
            this.lblMeterType.AutoSize = true;
            this.lblMeterType.Location = new System.Drawing.Point(20, 55);
            this.lblMeterType.Name = "lblMeterType";
            this.lblMeterType.Size = new System.Drawing.Size(77, 12);
            this.lblMeterType.TabIndex = 2;
            this.lblMeterType.Text = "メーター種別:";
            // 
            // cmbMeterType
            // 
            this.cmbMeterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMeterType.FormattingEnabled = true;
            this.cmbMeterType.Location = new System.Drawing.Point(150, 52);
            this.cmbMeterType.Name = "cmbMeterType";
            this.cmbMeterType.Size = new System.Drawing.Size(350, 20);
            this.cmbMeterType.TabIndex = 3;
            // 
            // lblMeterName
            // 
            this.lblMeterName.AutoSize = true;
            this.lblMeterName.Location = new System.Drawing.Point(20, 90);
            this.lblMeterName.Name = "lblMeterName";
            this.lblMeterName.Size = new System.Drawing.Size(77, 12);
            this.lblMeterName.TabIndex = 4;
            this.lblMeterName.Text = "親メーター名:";
            // 
            // txtMeterName
            // 
            this.txtMeterName.Location = new System.Drawing.Point(150, 87);
            this.txtMeterName.Name = "txtMeterName";
            this.txtMeterName.Size = new System.Drawing.Size(350, 19);
            this.txtMeterName.TabIndex = 5;
            // 
            // lblManagementNumber
            // 
            this.lblManagementNumber.AutoSize = true;
            this.lblManagementNumber.Location = new System.Drawing.Point(20, 125);
            this.lblManagementNumber.Name = "lblManagementNumber";
            this.lblManagementNumber.Size = new System.Drawing.Size(65, 12);
            this.lblManagementNumber.TabIndex = 6;
            this.lblManagementNumber.Text = "管理番号:";
            // 
            // txtManagementNumber
            // 
            this.txtManagementNumber.Location = new System.Drawing.Point(150, 122);
            this.txtManagementNumber.Name = "txtManagementNumber";
            this.txtManagementNumber.Size = new System.Drawing.Size(350, 19);
            this.txtManagementNumber.TabIndex = 7;
            // 
            // lblContractor
            // 
            this.lblContractor.AutoSize = true;
            this.lblContractor.Location = new System.Drawing.Point(20, 160);
            this.lblContractor.Name = "lblContractor";
            this.lblContractor.Size = new System.Drawing.Size(41, 12);
            this.lblContractor.TabIndex = 8;
            this.lblContractor.Text = "業者:";
            // 
            // cmbContractor
            // 
            this.cmbContractor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbContractor.FormattingEnabled = true;
            this.cmbContractor.Location = new System.Drawing.Point(150, 157);
            this.cmbContractor.Name = "cmbContractor";
            this.cmbContractor.Size = new System.Drawing.Size(350, 20);
            this.cmbContractor.TabIndex = 9;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(150, 205);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 205);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // MeterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ClientSize = new System.Drawing.Size(550, 280);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbContractor);
            this.Controls.Add(this.lblContractor);
            this.Controls.Add(this.txtManagementNumber);
            this.Controls.Add(this.lblManagementNumber);
            this.Controls.Add(this.txtMeterName);
            this.Controls.Add(this.lblMeterName);
            this.Controls.Add(this.cmbMeterType);
            this.Controls.Add(this.lblMeterType);
            this.Controls.Add(this.cmbBuilding);
            this.Controls.Add(this.lblBuilding);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "メーター情報登録";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}






