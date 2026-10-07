namespace WaterUtilityCost.Forms
{
    partial class ChildMeterForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBuildingName;
        private System.Windows.Forms.ComboBox cmbBuildingName;
        private System.Windows.Forms.Label lblMeterType;
        private System.Windows.Forms.ComboBox cmbMeterType;
        private System.Windows.Forms.Label lblParentMeterName;
        private System.Windows.Forms.ComboBox cmbParentMeterName;
        private System.Windows.Forms.Label lblChildMeterName;
        private System.Windows.Forms.TextBox txtChildMeterName;
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
            this.lblBuildingName = new System.Windows.Forms.Label();
            this.cmbBuildingName = new System.Windows.Forms.ComboBox();
            this.lblMeterType = new System.Windows.Forms.Label();
            this.cmbMeterType = new System.Windows.Forms.ComboBox();
            this.lblParentMeterName = new System.Windows.Forms.Label();
            this.cmbParentMeterName = new System.Windows.Forms.ComboBox();
            this.lblChildMeterName = new System.Windows.Forms.Label();
            this.txtChildMeterName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBuildingName
            // 
            this.lblBuildingName.AutoSize = true;
            this.lblBuildingName.Location = new System.Drawing.Point(20, 20);
            this.lblBuildingName.Name = "lblBuildingName";
            this.lblBuildingName.Size = new System.Drawing.Size(53, 12);
            this.lblBuildingName.TabIndex = 0;
            this.lblBuildingName.Text = "ビル名:";
            // 
            // cmbBuildingName
            // 
            this.cmbBuildingName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildingName.FormattingEnabled = true;
            this.cmbBuildingName.Location = new System.Drawing.Point(150, 17);
            this.cmbBuildingName.Name = "cmbBuildingName";
            this.cmbBuildingName.Size = new System.Drawing.Size(350, 20);
            this.cmbBuildingName.TabIndex = 1;
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
            // lblParentMeterName
            // 
            this.lblParentMeterName.AutoSize = true;
            this.lblParentMeterName.Location = new System.Drawing.Point(20, 90);
            this.lblParentMeterName.Name = "lblParentMeterName";
            this.lblParentMeterName.Size = new System.Drawing.Size(77, 12);
            this.lblParentMeterName.TabIndex = 4;
            this.lblParentMeterName.Text = "親メーター名:";
            // 
            // cmbParentMeterName
            // 
            this.cmbParentMeterName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParentMeterName.FormattingEnabled = true;
            this.cmbParentMeterName.Location = new System.Drawing.Point(150, 87);
            this.cmbParentMeterName.Name = "cmbParentMeterName";
            this.cmbParentMeterName.Size = new System.Drawing.Size(350, 20);
            this.cmbParentMeterName.TabIndex = 5;
            // 
            // lblChildMeterName
            // 
            this.lblChildMeterName.AutoSize = true;
            this.lblChildMeterName.Location = new System.Drawing.Point(20, 125);
            this.lblChildMeterName.Name = "lblChildMeterName";
            this.lblChildMeterName.Size = new System.Drawing.Size(77, 12);
            this.lblChildMeterName.TabIndex = 6;
            this.lblChildMeterName.Text = "子メーター名:";
            // 
            // txtChildMeterName
            // 
            this.txtChildMeterName.Location = new System.Drawing.Point(150, 122);
            this.txtChildMeterName.Name = "txtChildMeterName";
            this.txtChildMeterName.Size = new System.Drawing.Size(350, 19);
            this.txtChildMeterName.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(150, 160);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 160);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ChildMeterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ClientSize = new System.Drawing.Size(550, 215);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtChildMeterName);
            this.Controls.Add(this.lblChildMeterName);
            this.Controls.Add(this.cmbParentMeterName);
            this.Controls.Add(this.lblParentMeterName);
            this.Controls.Add(this.cmbMeterType);
            this.Controls.Add(this.lblMeterType);
            this.Controls.Add(this.cmbBuildingName);
            this.Controls.Add(this.lblBuildingName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChildMeterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "子メーター情報登録";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}



