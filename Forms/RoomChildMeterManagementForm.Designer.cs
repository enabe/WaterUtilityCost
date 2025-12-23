namespace WaterUtilityCost.Forms
{
    partial class RoomChildMeterManagementForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView _dgvRoomChildMeters;
        private System.Windows.Forms.Button _btnAdd;
        private System.Windows.Forms.Button _btnEdit;
        private System.Windows.Forms.Button _btnDelete;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;

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
            this._dgvRoomChildMeters = new System.Windows.Forms.DataGridView();
            this._btnAdd = new System.Windows.Forms.Button();
            this._btnEdit = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this._btnRefresh = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this._dgvRoomChildMeters)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgvRoomChildMeters
            // 
            this._dgvRoomChildMeters.AllowUserToAddRows = false;
            this._dgvRoomChildMeters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvRoomChildMeters.Location = new System.Drawing.Point(20, 70);
            this._dgvRoomChildMeters.MultiSelect = false;
            this._dgvRoomChildMeters.Name = "_dgvRoomChildMeters";
            this._dgvRoomChildMeters.ReadOnly = true;
            this._dgvRoomChildMeters.RowHeadersWidth = 51;
            this._dgvRoomChildMeters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvRoomChildMeters.Size = new System.Drawing.Size(960, 450);
            this._dgvRoomChildMeters.TabIndex = 0;
            // 
            // _btnAdd
            // 
            this._btnAdd.Location = new System.Drawing.Point(20, 20);
            this._btnAdd.Name = "_btnAdd";
            this._btnAdd.Size = new System.Drawing.Size(100, 30);
            this._btnAdd.TabIndex = 1;
            this._btnAdd.Text = "新規登録";
            this._btnAdd.UseVisualStyleBackColor = true;
            // 
            // _btnEdit
            // 
            this._btnEdit.Location = new System.Drawing.Point(130, 20);
            this._btnEdit.Name = "_btnEdit";
            this._btnEdit.Size = new System.Drawing.Size(100, 30);
            this._btnEdit.TabIndex = 2;
            this._btnEdit.Text = "編集";
            this._btnEdit.UseVisualStyleBackColor = true;
            // 
            // _btnDelete
            // 
            this._btnDelete.Location = new System.Drawing.Point(240, 20);
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.Size = new System.Drawing.Size(100, 30);
            this._btnDelete.TabIndex = 3;
            this._btnDelete.Text = "削除";
            this._btnDelete.UseVisualStyleBackColor = true;
            // 
            // _btnRefresh
            // 
            this._btnRefresh.Location = new System.Drawing.Point(350, 20);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(100, 30);
            this._btnRefresh.TabIndex = 4;
            this._btnRefresh.Text = "更新";
            this._btnRefresh.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 578);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1000, 22);
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(50, 17);
            this.statusLabel.Text = "準備完了";
            // 
            // RoomChildMeterManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("メイリオ", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this._btnRefresh);
            this.Controls.Add(this._btnDelete);
            this.Controls.Add(this._btnEdit);
            this.Controls.Add(this._btnAdd);
            this.Controls.Add(this._dgvRoomChildMeters);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "RoomChildMeterManagementForm";
            this.Text = "部屋別子メーター管理";
            ((System.ComponentModel.ISupportInitialize)(this._dgvRoomChildMeters)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}





