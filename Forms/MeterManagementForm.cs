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
    /// 親メーター管理フォーム
    /// </summary>
    public partial class MeterManagementForm : Form
    {
        public MeterManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += MeterManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvMeters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvMeters.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
        }

        private async void MeterManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadMetersAsync();
        }

        private async Task LoadMetersAsync()
        {
            try
            {
                var meters = await MeterDataAccess.GetAllMetersAsync();

                // 関連データを取得して表示用に整形
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                // DataSourceをnullにしてから列をクリア
                _dgvMeters!.DataSource = null;
                _dgvMeters.Columns.Clear();

                _dgvMeters.DataSource = meters.Select(m => new
                {
                    Id = m.Id,
                    ビル名 = buildings.FirstOrDefault(b => b.Id == m.BuildingId)?.Name ?? "",
                    メーター種別 = m.MeterType,
                    管理番号 = m.ManagementNumber
                }).OrderByDescending(x => x.Id).ToList();

                // MeterId列が存在する場合は削除
                if (_dgvMeters.Columns["MeterId"] != null)
                {
                    _dgvMeters.Columns.Remove("MeterId");
                }

                // ID列の幅を狭く設定
                if (_dgvMeters.Columns["Id"] != null)
                {
                    _dgvMeters.Columns["Id"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"メーターデータの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new MeterForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadMetersAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集するメーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var meter = await MeterDataAccess.GetMeterByIdAsync(id);
            if (meter != null)
            {
                var form = new MeterForm(meter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadMetersAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除するメーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var result = MessageBox.Show($"メーター（ID: {id}）を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var meterIdToDelete = (int)selectedRow.Cells["Id"].Value;
                    await MeterDataAccess.DeleteMeterAsync(meterIdToDelete);
                    await LoadMetersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadMetersAsync();
        }
    }
}






