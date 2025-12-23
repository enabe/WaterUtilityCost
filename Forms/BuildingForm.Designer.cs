namespace WaterUtilityCost.Forms
{
    partial class BuildingForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBuildingId;
        private System.Windows.Forms.TextBox txtBuildingId;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblFloors;
        private System.Windows.Forms.TextBox txtFloors;
        private System.Windows.Forms.Label lblBuiltDate;
        private System.Windows.Forms.TextBox txtBuiltDate;
        private System.Windows.Forms.Label lblArea;
        private System.Windows.Forms.TextBox txtArea;
        private System.Windows.Forms.Label lblOwner;
        private System.Windows.Forms.TextBox txtOwner;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.TextBox txtContact;
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
            this.lblBuildingId = new System.Windows.Forms.Label();
            this.txtBuildingId = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblFloors = new System.Windows.Forms.Label();
            this.txtFloors = new System.Windows.Forms.TextBox();
            this.lblBuiltDate = new System.Windows.Forms.Label();
            this.txtBuiltDate = new System.Windows.Forms.TextBox();
            this.lblArea = new System.Windows.Forms.Label();
            this.txtArea = new System.Windows.Forms.TextBox();
            this.lblOwner = new System.Windows.Forms.Label();
            this.txtOwner = new System.Windows.Forms.TextBox();
            this.lblContact = new System.Windows.Forms.Label();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBuildingId
            // 
            this.lblBuildingId.AutoSize = true;
            this.lblBuildingId.Location = new System.Drawing.Point(20, 20);
            this.lblBuildingId.Name = "lblBuildingId";
            this.lblBuildingId.Size = new System.Drawing.Size(53, 12);
            this.lblBuildingId.TabIndex = 0;
            this.lblBuildingId.Text = "ビルID:";
            // 
            // txtBuildingId
            // 
            this.txtBuildingId.Location = new System.Drawing.Point(110, 17);
            this.txtBuildingId.Name = "txtBuildingId";
            this.txtBuildingId.Size = new System.Drawing.Size(340, 19);
            this.txtBuildingId.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 50);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(41, 12);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "ビル名:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(110, 47);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(340, 19);
            this.txtName.TabIndex = 3;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(20, 80);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(35, 12);
            this.lblAddress.TabIndex = 4;
            this.lblAddress.Text = "住所:";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(110, 77);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(340, 19);
            this.txtAddress.TabIndex = 5;
            // 
            // lblFloors
            // 
            this.lblFloors.AutoSize = true;
            this.lblFloors.Location = new System.Drawing.Point(20, 110);
            this.lblFloors.Name = "lblFloors";
            this.lblFloors.Size = new System.Drawing.Size(35, 12);
            this.lblFloors.TabIndex = 6;
            this.lblFloors.Text = "階数:";
            // 
            // txtFloors
            // 
            this.txtFloors.Location = new System.Drawing.Point(110, 107);
            this.txtFloors.Name = "txtFloors";
            this.txtFloors.Size = new System.Drawing.Size(120, 19);
            this.txtFloors.TabIndex = 7;
            // 
            // lblBuiltDate
            // 
            this.lblBuiltDate.AutoSize = true;
            this.lblBuiltDate.Location = new System.Drawing.Point(20, 140);
            this.lblBuiltDate.Name = "lblBuiltDate";
            this.lblBuiltDate.Size = new System.Drawing.Size(47, 12);
            this.lblBuiltDate.TabIndex = 8;
            this.lblBuiltDate.Text = "建設日:";
            // 
            // txtBuiltDate
            // 
            this.txtBuiltDate.Location = new System.Drawing.Point(110, 137);
            this.txtBuiltDate.Name = "txtBuiltDate";
            this.txtBuiltDate.Size = new System.Drawing.Size(140, 19);
            this.txtBuiltDate.TabIndex = 9;
            // 
            // lblArea
            // 
            this.lblArea.AutoSize = true;
            this.lblArea.Location = new System.Drawing.Point(20, 170);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(53, 12);
            this.lblArea.TabIndex = 8;
            this.lblArea.Text = "面積(㎡):";
            // 
            // txtArea
            // 
            this.txtArea.Location = new System.Drawing.Point(110, 167);
            this.txtArea.Name = "txtArea";
            this.txtArea.Size = new System.Drawing.Size(120, 19);
            this.txtArea.TabIndex = 10;
            // 
            // lblOwner
            // 
            this.lblOwner.AutoSize = true;
            this.lblOwner.Location = new System.Drawing.Point(20, 200);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(47, 12);
            this.lblOwner.TabIndex = 11;
            this.lblOwner.Text = "所有者:";
            // 
            // txtOwner
            // 
            this.txtOwner.Location = new System.Drawing.Point(110, 197);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(340, 19);
            this.txtOwner.TabIndex = 12;
            // 
            // lblContact
            // 
            this.lblContact.AutoSize = true;
            this.lblContact.Location = new System.Drawing.Point(20, 230);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(47, 12);
            this.lblContact.TabIndex = 13;
            this.lblContact.Text = "連絡先:";
            // 
            // txtContact
            // 
            this.txtContact.Location = new System.Drawing.Point(110, 227);
            this.txtContact.Name = "txtContact";
            this.txtContact.Size = new System.Drawing.Size(340, 19);
            this.txtContact.TabIndex = 14;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(290, 310);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(380, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // BuildingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(500, 430);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtContact);
            this.Controls.Add(this.lblContact);
            this.Controls.Add(this.txtOwner);
            this.Controls.Add(this.lblOwner);
            this.Controls.Add(this.txtArea);
            this.Controls.Add(this.lblArea);
            this.Controls.Add(this.txtBuiltDate);
            this.Controls.Add(this.lblBuiltDate);
            this.Controls.Add(this.txtFloors);
            this.Controls.Add(this.lblFloors);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtBuildingId);
            this.Controls.Add(this.lblBuildingId);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "BuildingForm";
            this.Text = "ビル情報登録";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}



