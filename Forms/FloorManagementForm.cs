using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 部屋管理フォーム
    /// </summary>
    public partial class FloorManagementForm : Form
    {

        public FloorManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += FloorManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvFloors.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvFloors.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _dgvFloors.DoubleClick += DgvFloors_DoubleClick;
        }

        private async void FloorManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadFloorsAsync();
        }

        private async Task LoadFloorsAsync()
        {
            try
            {
                var floors = await FloorDataAccess.GetAllFloorsAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                _dgvFloors!.DataSource = floors.Select(f => new
                {
                    Id = f.Id,
                    ビル名 = buildings.FirstOrDefault(b => b.Id == f.BuildingId)?.Name ?? "",
                    部屋名 = f.FloorName,
                    部屋面積 = f.FloorArea
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvFloors.Columns["Id"] != null)
                {
                    _dgvFloors.Columns["Id"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new FloorForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadFloorsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvFloors!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する部屋を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvFloors.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var floor = await FloorDataAccess.GetFloorByIdAsync(id);
            if (floor != null)
            {
                var form = new FloorForm(floor);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadFloorsAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvFloors!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する部屋を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvFloors.SelectedRows[0];
            var floorName = selectedRow.Cells["部屋名"].Value?.ToString() ?? "";
            var result = MessageBox.Show($"部屋「{floorName}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var floorId = (int)selectedRow.Cells["Id"].Value;
                    await FloorDataAccess.DeleteFloorAsync(floorId);
                    await LoadFloorsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadFloorsAsync();
        }

        private async void DgvFloors_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvFloors!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvFloors.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var floor = await FloorDataAccess.GetFloorByIdAsync(id);
            if (floor != null)
            {
                var form = new FloorForm(floor);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadFloorsAsync();
                }
            }
        }
    }
}

