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
        private System.Windows.Forms.Label lblRoomName;
        private System.Windows.Forms.ComboBox cmbRoomName;
        private System.Windows.Forms.Label lblMeterType;
        private System.Windows.Forms.ComboBox cmbMeterType;
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
            this.lblRoomName = new System.Windows.Forms.Label();
            this.cmbRoomName = new System.Windows.Forms.ComboBox();
            this.lblMeterType = new System.Windows.Forms.Label();
            this.cmbMeterType = new System.Windows.Forms.ComboBox();
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
            // lblRoomName
            // 
            this.lblRoomName.AutoSize = true;
            this.lblRoomName.Location = new System.Drawing.Point(20, 55);
            this.lblRoomName.Name = "lblRoomName";
            this.lblRoomName.Size = new System.Drawing.Size(53, 12);
            this.lblRoomName.TabIndex = 2;
            this.lblRoomName.Text = "部屋名:";
            // 
            // cmbRoomName
            // 
            this.cmbRoomName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoomName.FormattingEnabled = true;
            this.cmbRoomName.Location = new System.Drawing.Point(150, 52);
            this.cmbRoomName.Name = "cmbRoomName";
            this.cmbRoomName.Size = new System.Drawing.Size(350, 20);
            this.cmbRoomName.TabIndex = 3;
            // 
            // lblMeterType
            // 
            this.lblMeterType.AutoSize = true;
            this.lblMeterType.Location = new System.Drawing.Point(20, 90);
            this.lblMeterType.Name = "lblMeterType";
            this.lblMeterType.Size = new System.Drawing.Size(77, 12);
            this.lblMeterType.TabIndex = 4;
            this.lblMeterType.Text = "メーター種別:";
            // 
            // cmbMeterType
            // 
            this.cmbMeterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMeterType.FormattingEnabled = true;
            this.cmbMeterType.Location = new System.Drawing.Point(150, 87);
            this.cmbMeterType.Name = "cmbMeterType";
            this.cmbMeterType.Size = new System.Drawing.Size(350, 20);
            this.cmbMeterType.TabIndex = 5;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(150, 130);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ChildMeterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ClientSize = new System.Drawing.Size(550, 185);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbMeterType);
            this.Controls.Add(this.lblMeterType);
            this.Controls.Add(this.cmbRoomName);
            this.Controls.Add(this.lblRoomName);
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



