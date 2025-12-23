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
    /// ガス子メータ検針データ管理フォーム
    /// </summary>
    public partial class GasChildMeterReadingManagementForm : Form
    {
        private const string METER_TYPE = "ガス";

        public GasChildMeterReadingManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += GasChildMeterReadingManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvGasChildMeterReadings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvGasChildMeterReadings.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _dgvGasChildMeterReadings.DoubleClick += DgvGasChildMeterReadings_DoubleClick;
        }

        private async void GasChildMeterReadingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadGasChildMeterReadingsAsync();
        }

        private async Task LoadGasChildMeterReadingsAsync()
        {
            try
            {
                var readings = await ChildMeterReadingDataAccess.GetAllChildMeterReadingsAsync();
                
                // ガスのデータのみをフィルタリング
                var gasReadings = readings.Where(r => r.Type == METER_TYPE).ToList();

                // 関連データを取得して表示用に整形
                var floors = await FloorDataAccess.GetAllFloorsAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                _dgvGasChildMeterReadings!.DataSource = gasReadings.Select(r =>
                {
                    var floor = floors.FirstOrDefault(f => f.Id == r.FloorId);
                    var building = floor != null ? buildings.FirstOrDefault(b => b.Id == floor.BuildingId) : null;
                    return new
                    {
                        Id = r.Id,
                        ビル名 = building?.Name ?? "",
                        部屋名 = floor?.FloorName ?? "",
                        検針日 = r.ReadingDate.ToString("yyyy-MM-dd"),
                        メーター値 = r.MeterValue,
                        作成日時 = r.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        更新日時 = r.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvGasChildMeterReadings.Columns["Id"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["Id"].Width = 40;
                }

                // ビル名列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["ビル名"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["ビル名"].Width = 150;
                }

                // 部屋名列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["部屋名"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["部屋名"].Width = 150;
                }

                // 検針日列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["検針日"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["検針日"].Width = 120;
                }

                // メーター値列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["メーター値"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["メーター値"].Width = 120;
                }

                // 作成日時列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["作成日時"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["作成日時"].Width = 150;
                }

                // 更新日時列の幅を設定
                if (_dgvGasChildMeterReadings.Columns["更新日時"] != null)
                {
                    _dgvGasChildMeterReadings.Columns["更新日時"].Width = 150;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new GasChildMeterReadingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadGasChildMeterReadingsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvGasChildMeterReadings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する検針データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvGasChildMeterReadings.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var reading = await ChildMeterReadingDataAccess.GetChildMeterReadingByIdAsync(id);
            if (reading != null)
            {
                var form = new GasChildMeterReadingForm(reading);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadGasChildMeterReadingsAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvGasChildMeterReadings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する検針データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvGasChildMeterReadings.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var buildingName = selectedRow.Cells["ビル名"].Value?.ToString() ?? "";
            var roomName = selectedRow.Cells["部屋名"].Value?.ToString() ?? "";
            var readingDate = selectedRow.Cells["検針日"].Value?.ToString() ?? "";
            
            var result = MessageBox.Show($"検針データ（ID: {id}、ビル: {buildingName}、部屋: {roomName}、検針日: {readingDate}）を削除しますか？", 
                "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    await ChildMeterReadingDataAccess.DeleteChildMeterReadingAsync(id);
                    await LoadGasChildMeterReadingsAsync();
                    MessageBox.Show("検針データを削除しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadGasChildMeterReadingsAsync();
        }

        private async void DgvGasChildMeterReadings_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvGasChildMeterReadings!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvGasChildMeterReadings.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var reading = await ChildMeterReadingDataAccess.GetChildMeterReadingByIdAsync(id);
            if (reading != null)
            {
                var form = new GasChildMeterReadingForm(reading);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadGasChildMeterReadingsAsync();
                }
            }
        }
    }
}

