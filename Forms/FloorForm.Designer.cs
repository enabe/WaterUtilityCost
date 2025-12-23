namespace WaterUtilityCost.Forms
{
    partial class FloorForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBuilding;
        private System.Windows.Forms.ComboBox cmbBuilding;
        private System.Windows.Forms.Label lblFloorName;
        private System.Windows.Forms.TextBox txtFloorName;
        private System.Windows.Forms.Label lblFloorArea;
        private System.Windows.Forms.TextBox txtFloorArea;
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
            this.lblFloorName = new System.Windows.Forms.Label();
            this.txtFloorName = new System.Windows.Forms.TextBox();
            this.lblFloorArea = new System.Windows.Forms.Label();
            this.txtFloorArea = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.Location = new System.Drawing.Point(20, 20);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(87, 25);
            this.lblBuilding.TabIndex = 0;
            this.lblBuilding.Text = "ビル名:";
            // 
            // cmbBuilding
            // 
            this.cmbBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuilding.FormattingEnabled = true;
            this.cmbBuilding.Location = new System.Drawing.Point(156, 17);
            this.cmbBuilding.Name = "cmbBuilding";
            this.cmbBuilding.Size = new System.Drawing.Size(340, 33);
            this.cmbBuilding.TabIndex = 1;
            // 
            // lblFloorName
            // 
            this.lblFloorName.AutoSize = true;
            this.lblFloorName.Location = new System.Drawing.Point(20, 50);
            this.lblFloorName.Name = "lblFloorName";
            this.lblFloorName.Size = new System.Drawing.Size(87, 25);
            this.lblFloorName.TabIndex = 2;
            this.lblFloorName.Text = "部屋名:";
            // 
            // txtFloorName
            // 
            this.txtFloorName.Location = new System.Drawing.Point(156, 47);
            this.txtFloorName.Name = "txtFloorName";
            this.txtFloorName.Size = new System.Drawing.Size(340, 32);
            this.txtFloorName.TabIndex = 3;
            // 
            // lblFloorArea
            // 
            this.lblFloorArea.AutoSize = true;
            this.lblFloorArea.Location = new System.Drawing.Point(20, 80);
            this.lblFloorArea.Name = "lblFloorArea";
            this.lblFloorArea.Size = new System.Drawing.Size(135, 25);
            this.lblFloorArea.TabIndex = 4;
            this.lblFloorArea.Text = "部屋面積(㎡):";
            // 
            // txtFloorArea
            // 
            this.txtFloorArea.Location = new System.Drawing.Point(156, 77);
            this.txtFloorArea.Name = "txtFloorArea";
            this.txtFloorArea.Size = new System.Drawing.Size(120, 32);
            this.txtFloorArea.TabIndex = 5;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(290, 150);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(380, 150);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FloorForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(523, 200);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtFloorArea);
            this.Controls.Add(this.lblFloorArea);
            this.Controls.Add(this.txtFloorName);
            this.Controls.Add(this.lblFloorName);
            this.Controls.Add(this.cmbBuilding);
            this.Controls.Add(this.lblBuilding);
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "FloorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "部屋情報登録";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}








