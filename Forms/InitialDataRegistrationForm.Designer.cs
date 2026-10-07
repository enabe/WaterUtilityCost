namespace WaterUtilityCost.Forms
{
    partial class InitialDataRegistrationForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button _btnBuildingRegistration;
        private System.Windows.Forms.Button _btnFloorRegistration;
        private System.Windows.Forms.Button _btnTenantRegistration;
        private System.Windows.Forms.Button _btnContractorRegistration;
        private System.Windows.Forms.Button _btnMeterRegistration;
        private System.Windows.Forms.Button _btnChildMeterRegistration;
        private System.Windows.Forms.Button _btnRoomChildMeterRegistration;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this._btnBuildingRegistration = new System.Windows.Forms.Button();
            this._btnFloorRegistration = new System.Windows.Forms.Button();
            this._btnTenantRegistration = new System.Windows.Forms.Button();
            this._btnContractorRegistration = new System.Windows.Forms.Button();
            this._btnMeterRegistration = new System.Windows.Forms.Button();
            this._btnChildMeterRegistration = new System.Windows.Forms.Button();
            this._btnRoomChildMeterRegistration = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("メイリオ", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.Location = new System.Drawing.Point(12, 20);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 36);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "初期データ登録";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _btnBuildingRegistration
            // 
            this._btnBuildingRegistration.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnBuildingRegistration.Location = new System.Drawing.Point(38, 82);
            this._btnBuildingRegistration.Margin = new System.Windows.Forms.Padding(4);
            this._btnBuildingRegistration.Name = "_btnBuildingRegistration";
            this._btnBuildingRegistration.Size = new System.Drawing.Size(410, 56);
            this._btnBuildingRegistration.TabIndex = 1;
            this._btnBuildingRegistration.Text = "ビル情報データ登録";
            this._btnBuildingRegistration.UseVisualStyleBackColor = true;
            // 
            // _btnFloorRegistration
            // 
            this._btnFloorRegistration.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnFloorRegistration.Location = new System.Drawing.Point(38, 152);
            this._btnFloorRegistration.Margin = new System.Windows.Forms.Padding(4);
            this._btnFloorRegistration.Name = "_btnFloorRegistration";
            this._btnFloorRegistration.Size = new System.Drawing.Size(410, 56);
            this._btnFloorRegistration.TabIndex = 2;
            this._btnFloorRegistration.Text = "部屋情報データ登録";
            this._btnFloorRegistration.UseVisualStyleBackColor = true;
            // 
            // _btnTenantRegistration
            // 
            this._btnTenantRegistration.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnTenantRegistration.Location = new System.Drawing.Point(38, 222);
            this._btnTenantRegistration.Margin = new System.Windows.Forms.Padding(4);
            this._btnTenantRegistration.Name = "_btnTenantRegistration";
            this._btnTenantRegistration.Size = new System.Drawing.Size(410, 56);
            this._btnTenantRegistration.TabIndex = 3;
            this._btnTenantRegistration.Text = "賃借人情報データ登録";
            this._btnTenantRegistration.UseVisualStyleBackColor = true;
            // 
            // _btnContractorRegistration
            // 
            this._btnContractorRegistration.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnContractorRegistration.Location = new System.Drawing.Point(38, 292);
            this._btnContractorRegistration.Margin = new System.Windows.Forms.Padding(4);
            this._btnContractorRegistration.Name = "_btnContractorRegistration";
            this._btnContractorRegistration.Size = new System.Drawing.Size(410, 56);
            this._btnContractorRegistration.TabIndex = 4;
            this._btnContractorRegistration.Text = "業者情報データ登録";
            this._btnContractorRegistration.UseVisualStyleBackColor = true;
            // 
            // _btnMeterRegistration
            // 
            this._btnMeterRegistration.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnMeterRegistration.Location = new System.Drawing.Point(38, 362);
            this._btnMeterRegistration.Margin = new System.Windows.Forms.Padding(4);
            this._btnMeterRegistration.Name = "_btnMeterRegistration";
            this._btnMeterRegistration.Size = new System.Drawing.Size(410, 56);
            this._btnMeterRegistration.TabIndex = 5;
            this._btnMeterRegistration.Text = "親メーター情報データ登録";
            this._btnMeterRegistration.UseVisualStyleBackColor = true;
            // 
            // _btnChildMeterRegistration
            // 
            this._btnChildMeterRegistration.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnChildMeterRegistration.Location = new System.Drawing.Point(38, 432);
            this._btnChildMeterRegistration.Margin = new System.Windows.Forms.Padding(4);
            this._btnChildMeterRegistration.Name = "_btnChildMeterRegistration";
            this._btnChildMeterRegistration.Size = new System.Drawing.Size(410, 56);
            this._btnChildMeterRegistration.TabIndex = 6;
            this._btnChildMeterRegistration.Text = "子メーター情報データ登録";
            this._btnChildMeterRegistration.UseVisualStyleBackColor = true;
            // 
            // _btnRoomChildMeterRegistration
            // 
            this._btnRoomChildMeterRegistration.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._btnRoomChildMeterRegistration.Location = new System.Drawing.Point(38, 502);
            this._btnRoomChildMeterRegistration.Margin = new System.Windows.Forms.Padding(4);
            this._btnRoomChildMeterRegistration.Name = "_btnRoomChildMeterRegistration";
            this._btnRoomChildMeterRegistration.Size = new System.Drawing.Size(410, 56);
            this._btnRoomChildMeterRegistration.TabIndex = 7;
            this._btnRoomChildMeterRegistration.Text = "部屋別子メーター管理データ登録";
            this._btnRoomChildMeterRegistration.UseVisualStyleBackColor = true;
            // 
            // InitialDataRegistrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 581);
            this.Controls.Add(this._btnRoomChildMeterRegistration);
            this.Controls.Add(this._btnChildMeterRegistration);
            this.Controls.Add(this._btnMeterRegistration);
            this.Controls.Add(this._btnContractorRegistration);
            this.Controls.Add(this._btnTenantRegistration);
            this.Controls.Add(this._btnFloorRegistration);
            this.Controls.Add(this._btnBuildingRegistration);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "InitialDataRegistrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "初期データ登録";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
