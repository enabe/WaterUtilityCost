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
    /// ビル管理フォーム
    /// </summary>
    public partial class BuildingManagementForm : Form
    {

        public BuildingManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += BuildingManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvBuildings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvBuildings.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _dgvBuildings.DoubleClick += DgvBuildings_DoubleClick;
        }

        private async void BuildingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();
        }

        private async Task LoadBuildingsAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                _dgvBuildings!.DataSource = buildings.Select(b => new
                {
                    Id = b.Id,
                    ビルID = b.BuildingId,
                    ビル名 = b.Name,
                    住所 = b.Address,
                    階数 = b.Floors,
                    建設日 = b.BuiltDate != DateTime.MinValue ? b.BuiltDate.ToString("yyyy-MM-dd") : "",
                    面積 = b.Area,
                    所有者 = b.Owner,
                    連絡先 = b.Contact
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvBuildings.Columns["Id"] != null)
                {
                    _dgvBuildings.Columns["Id"].Width = 40;
                }

                // ビルID列の幅を設定
                if (_dgvBuildings.Columns["ビルID"] != null)
                {
                    _dgvBuildings.Columns["ビルID"].Width = 100;
                }

                // ビル名列の幅を広く設定
                if (_dgvBuildings.Columns["ビル名"] != null)
                {
                    _dgvBuildings.Columns["ビル名"].Width = 150;
                }

                // 住所列の幅を広く設定
                if (_dgvBuildings.Columns["住所"] != null)
                {
                    _dgvBuildings.Columns["住所"].Width = 150;
                }

                // 階数列の幅を設定
                if (_dgvBuildings.Columns["階数"] != null)
                {
                    _dgvBuildings.Columns["階数"].Width = 60;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new BuildingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadBuildingsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvBuildings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集するビルを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvBuildings.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var building = await BuildingDataAccess.GetBuildingByIdAsync(id);
            if (building != null)
            {
                var form = new BuildingForm(building);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadBuildingsAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvBuildings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除するビルを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvBuildings.SelectedRows[0];
            var buildingName = selectedRow.Cells["ビル名"].Value?.ToString() ?? "";
            var result = MessageBox.Show($"ビル「{buildingName}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var buildingId = (int)selectedRow.Cells["Id"].Value;
                    await BuildingDataAccess.DeleteBuildingAsync(buildingId);
                    await LoadBuildingsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();
        }

        private async void DgvBuildings_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvBuildings!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvBuildings.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var building = await BuildingDataAccess.GetBuildingByIdAsync(id);
            if (building != null)
            {
                var form = new BuildingForm(building);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadBuildingsAsync();
                }
            }
        }
    }
}

