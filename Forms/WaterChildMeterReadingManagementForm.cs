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
    /// 水道子メータ検針データ管理フォーム
    /// </summary>
    public partial class WaterChildMeterReadingManagementForm : Form
    {
        private const string METER_TYPE = "水道";

        public WaterChildMeterReadingManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += WaterChildMeterReadingManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvWaterChildMeterReadings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvWaterChildMeterReadings.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _dgvWaterChildMeterReadings.DoubleClick += DgvWaterChildMeterReadings_DoubleClick;
        }

        private async void WaterChildMeterReadingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadWaterChildMeterReadingsAsync();
        }

        private async Task LoadWaterChildMeterReadingsAsync()
        {
            try
            {
                var readings = await ChildMeterReadingDataAccess.GetAllChildMeterReadingsAsync();
                
                // 水道のデータのみをフィルタリング
                var waterReadings = readings.Where(r => r.Type == METER_TYPE).ToList();

                // 関連データを取得して表示用に整形
                var floors = await FloorDataAccess.GetAllFloorsAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                _dgvWaterChildMeterReadings!.DataSource = waterReadings.Select(r =>
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
                if (_dgvWaterChildMeterReadings.Columns["Id"] != null)
                {
                    _dgvWaterChildMeterReadings.Columns["Id"].Width = 40;
                }

                // ビル名列の幅を設定
                if (_dgvWaterChildMeterReadings.Columns["ビル名"] != null)
                {
                    _dgvWaterChildMeterReadings.Columns["ビル名"].Width = 150;
                }

                // 部屋名列の幅を設定
                if (_dgvWaterChildMeterReadings.Columns["部屋名"] != null)
                {
                    _dgvWaterChildMeterReadings.Columns["部屋名"].Width = 150;
                }

                // 検針日列の幅を設定
                if (_dgvWaterChildMeterReadings.Columns["検針日"] != null)
                {
                    _dgvWaterChildMeterReadings.Columns["検針日"].Width = 120;
                }

                // メーター値列の幅を設定
                if (_dgvWaterChildMeterReadings.Columns["メーター値"] != null)
                {
                    _dgvWaterChildMeterReadings.Columns["メーター値"].Width = 120;
                }

                // 作成日時列の幅を設定
                if (_dgvWaterChildMeterReadings.Columns["作成日時"] != null)
                {
                    _dgvWaterChildMeterReadings.Columns["作成日時"].Width = 150;
                }

                // 更新日時列の幅を設定
                if (_dgvWaterChildMeterReadings.Columns["更新日時"] != null)
                {
                    _dgvWaterChildMeterReadings.Columns["更新日時"].Width = 150;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new WaterChildMeterReadingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadWaterChildMeterReadingsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvWaterChildMeterReadings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する検針データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvWaterChildMeterReadings.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var reading = await ChildMeterReadingDataAccess.GetChildMeterReadingByIdAsync(id);
            if (reading != null)
            {
                var form = new WaterChildMeterReadingForm(reading);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadWaterChildMeterReadingsAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvWaterChildMeterReadings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する検針データを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvWaterChildMeterReadings.SelectedRows[0];
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
                    await LoadWaterChildMeterReadingsAsync();
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
            await LoadWaterChildMeterReadingsAsync();
        }

        private async void DgvWaterChildMeterReadings_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvWaterChildMeterReadings!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvWaterChildMeterReadings.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var reading = await ChildMeterReadingDataAccess.GetChildMeterReadingByIdAsync(id);
            if (reading != null)
            {
                var form = new WaterChildMeterReadingForm(reading);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadWaterChildMeterReadingsAsync();
                }
            }
        }
    }
}

