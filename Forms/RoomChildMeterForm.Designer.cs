namespace WaterUtilityCost.Forms
{
    partial class RoomChildMeterForm
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
        private System.Windows.Forms.Label lblParentMeter;
        private System.Windows.Forms.ComboBox cmbParentMeter;
        private System.Windows.Forms.Label lblChildMeter;
        private System.Windows.Forms.ComboBox cmbChildMeter;
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
            this.lblParentMeter = new System.Windows.Forms.Label();
            this.cmbParentMeter = new System.Windows.Forms.ComboBox();
            this.lblChildMeter = new System.Windows.Forms.Label();
            this.cmbChildMeter = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBuildingName
            // 
            this.lblBuildingName.AutoSize = true;
            this.lblBuildingName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBuildingName.Location = new System.Drawing.Point(20, 20);
            this.lblBuildingName.Name = "lblBuildingName";
            this.lblBuildingName.Size = new System.Drawing.Size(65, 25);
            this.lblBuildingName.TabIndex = 0;
            this.lblBuildingName.Text = "ビル名:";
            // 
            // cmbBuildingName
            // 
            this.cmbBuildingName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildingName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbBuildingName.FormattingEnabled = true;
            this.cmbBuildingName.Location = new System.Drawing.Point(150, 17);
            this.cmbBuildingName.Name = "cmbBuildingName";
            this.cmbBuildingName.Size = new System.Drawing.Size(400, 33);
            this.cmbBuildingName.TabIndex = 1;
            // 
            // lblRoomName
            // 
            this.lblRoomName.AutoSize = true;
            this.lblRoomName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblRoomName.Location = new System.Drawing.Point(20, 60);
            this.lblRoomName.Name = "lblRoomName";
            this.lblRoomName.Size = new System.Drawing.Size(65, 25);
            this.lblRoomName.TabIndex = 2;
            this.lblRoomName.Text = "部屋名:";
            // 
            // cmbRoomName
            // 
            this.cmbRoomName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoomName.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbRoomName.FormattingEnabled = true;
            this.cmbRoomName.Location = new System.Drawing.Point(150, 57);
            this.cmbRoomName.Name = "cmbRoomName";
            this.cmbRoomName.Size = new System.Drawing.Size(400, 33);
            this.cmbRoomName.TabIndex = 3;
            // 
            // lblMeterType
            // 
            this.lblMeterType.AutoSize = true;
            this.lblMeterType.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMeterType.Location = new System.Drawing.Point(20, 100);
            this.lblMeterType.Name = "lblMeterType";
            this.lblMeterType.Size = new System.Drawing.Size(101, 25);
            this.lblMeterType.TabIndex = 4;
            this.lblMeterType.Text = "メーター種別:";
            // 
            // cmbMeterType
            // 
            this.cmbMeterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMeterType.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbMeterType.FormattingEnabled = true;
            this.cmbMeterType.Location = new System.Drawing.Point(150, 97);
            this.cmbMeterType.Name = "cmbMeterType";
            this.cmbMeterType.Size = new System.Drawing.Size(400, 33);
            this.cmbMeterType.TabIndex = 5;
            // 
            // lblParentMeter
            // 
            this.lblParentMeter.AutoSize = true;
            this.lblParentMeter.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblParentMeter.Location = new System.Drawing.Point(20, 140);
            this.lblParentMeter.Name = "lblParentMeter";
            this.lblParentMeter.Size = new System.Drawing.Size(89, 25);
            this.lblParentMeter.TabIndex = 6;
            this.lblParentMeter.Text = "親メーター:";
            // 
            // cmbParentMeter
            // 
            this.cmbParentMeter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParentMeter.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbParentMeter.FormattingEnabled = true;
            this.cmbParentMeter.Location = new System.Drawing.Point(150, 137);
            this.cmbParentMeter.Name = "cmbParentMeter";
            this.cmbParentMeter.Size = new System.Drawing.Size(400, 33);
            this.cmbParentMeter.TabIndex = 7;
            // 
            // lblChildMeter
            // 
            this.lblChildMeter.AutoSize = true;
            this.lblChildMeter.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblChildMeter.Location = new System.Drawing.Point(20, 180);
            this.lblChildMeter.Name = "lblChildMeter";
            this.lblChildMeter.Size = new System.Drawing.Size(89, 25);
            this.lblChildMeter.TabIndex = 8;
            this.lblChildMeter.Text = "子メーター:";
            // 
            // cmbChildMeter
            // 
            this.cmbChildMeter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChildMeter.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbChildMeter.FormattingEnabled = true;
            this.cmbChildMeter.Location = new System.Drawing.Point(150, 177);
            this.cmbChildMeter.Name = "cmbChildMeter";
            this.cmbChildMeter.Size = new System.Drawing.Size(400, 33);
            this.cmbChildMeter.TabIndex = 9;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnSave.Location = new System.Drawing.Point(150, 230);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(260, 230);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // RoomChildMeterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 290);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbChildMeter);
            this.Controls.Add(this.lblChildMeter);
            this.Controls.Add(this.cmbParentMeter);
            this.Controls.Add(this.lblParentMeter);
            this.Controls.Add(this.cmbMeterType);
            this.Controls.Add(this.lblMeterType);
            this.Controls.Add(this.cmbRoomName);
            this.Controls.Add(this.lblRoomName);
            this.Controls.Add(this.cmbBuildingName);
            this.Controls.Add(this.lblBuildingName);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RoomChildMeterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "部屋別子メーター情報登録";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}


