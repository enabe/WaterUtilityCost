using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 部屋別子メーター管理フォーム
    /// </summary>
    public partial class RoomChildMeterManagementForm : Form
    {
        public RoomChildMeterManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += RoomChildMeterManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvRoomChildMeters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvRoomChildMeters.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
        }

        private async void RoomChildMeterManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadRoomChildMetersAsync();
        }

        private async Task LoadRoomChildMetersAsync()
        {
            try
            {
                var roomChildMeters = await RoomChildMeterDataAccess.GetAllRoomChildMetersAsync();
                var floors = await FloorDataAccess.GetAllFloorsAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();

                _dgvRoomChildMeters!.DataSource = roomChildMeters.Select(rcm =>
                {
                    var floor = floors.FirstOrDefault(f => f.Id == rcm.FloorId);
                    var building = floor != null ? buildings.FirstOrDefault(b => b.Id == floor.BuildingId) : null;
                    var buildingName = building?.Name ?? $"ビルID: {floor?.BuildingId ?? 0}";
                    var roomName = floor?.FloorName ?? "";
                    
                    var childMeter = childMeters.FirstOrDefault(cm => cm.Id == rcm.ChildMeterId);
                    var parentMeterId = childMeter?.ParentMeterId?.ToString() ?? "なし";
                    var childMeterId = childMeter?.Id.ToString() ?? "";
                    
                    return new
                    {
                        Id = rcm.Id,
                        ビル名 = buildingName,
                        部屋名 = roomName,
                        親メーターID = parentMeterId,
                        子メーターID = childMeterId
                    };
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvRoomChildMeters.Columns["Id"] != null)
                {
                    _dgvRoomChildMeters.Columns["Id"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new RoomChildMeterForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadRoomChildMetersAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvRoomChildMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する部屋別子メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvRoomChildMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var roomChildMeter = await RoomChildMeterDataAccess.GetRoomChildMeterByIdAsync(id);
            if (roomChildMeter != null)
            {
                var form = new RoomChildMeterForm(roomChildMeter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadRoomChildMetersAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvRoomChildMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する部屋別子メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvRoomChildMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var result = MessageBox.Show($"部屋別子メーター（ID: {id}）を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    await RoomChildMeterDataAccess.DeleteRoomChildMeterAsync(id);
                    await LoadRoomChildMetersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadRoomChildMetersAsync();
        }
    }
}





