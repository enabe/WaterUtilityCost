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
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class MeterGridRow
        {
            public int Id { get; set; }
            public string ビル名 { get; set; } = string.Empty;
            public string メーター種別 { get; set; } = string.Empty;
            public string 親メーター名 { get; set; } = string.Empty;
        }

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
            _btnCopyAndAdd.Click += BtnCopyAndAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _dgvMeters.DoubleClick += DgvMeters_DoubleClick;
            _dgvMeters.DataBindingComplete += DgvMeters_DataBindingComplete;
            _dgvMeters.ColumnHeaderMouseClick += DgvMeters_ColumnHeaderMouseClick;
        }

        private async void MeterManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadMetersAsync();
        }

        private async Task LoadMetersAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var meters = await MeterDataAccess.GetAllMetersAsync();

                // 関連データを取得して表示用に整形
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvMeters.SuspendLayout();
                    _dgvMeters.Visible = false;
                }

                // DataSourceをnullにしてから列をクリア
                _dgvMeters!.DataSource = null;
                _dgvMeters.Columns.Clear();

                var rows = meters.Select(m => new MeterGridRow
                {
                    Id = m.Id,
                    ビル名 = buildings.FirstOrDefault(b => b.Id == m.BuildingId)?.Name ?? "",
                    メーター種別 = m.MeterType ?? "",
                    親メーター名 = m.MeterName ?? ""
                });

                _dgvMeters.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

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
                if (_dgvMeters.Columns["メーター種別"] != null)
                {
                    _dgvMeters.Columns["メーター種別"].FillWeight = 60;
                }

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvMeters.Visible = true;
                    _dgvMeters.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvMeters.Visible = true;
                    _dgvMeters.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"メーターデータの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<MeterGridRow> ApplySort(IEnumerable<MeterGridRow> source)
        {
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => source.OrderBy(x => x.Id),
                ("Id", false) => source.OrderByDescending(x => x.Id),
                ("ビル名", true) => source.OrderBy(x => x.ビル名),
                ("ビル名", false) => source.OrderByDescending(x => x.ビル名),
                ("メーター種別", true) => source.OrderBy(x => x.メーター種別),
                ("メーター種別", false) => source.OrderByDescending(x => x.メーター種別),
                ("親メーター名", true) => source.OrderBy(x => x.親メーター名),
                ("親メーター名", false) => source.OrderByDescending(x => x.親メーター名),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvMeters.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvMeters.Columns.Contains(_sortColumn))
            {
                _dgvMeters.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvMeters_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvMeters.Columns[e.ColumnIndex].Name;
            if (_sortColumn == clickedColumnName)
            {
                _sortAscending = !_sortAscending;
            }
            else
            {
                _sortColumn = clickedColumnName;
                _sortAscending = true;
            }

            int? selectedId = null;
            int? currentScrollIndex = null;
            if (_dgvMeters.SelectedRows.Count > 0 &&
                _dgvMeters.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvMeters.Rows.Count > 0)
            {
                currentScrollIndex = _dgvMeters.FirstDisplayedScrollingRowIndex;
            }

            await LoadMetersAsync(selectedId, currentScrollIndex);
        }

        private void DgvMeters_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!_pendingSelectId.HasValue)
            {
                return;
            }

            var selectId = _pendingSelectId.Value;
            _pendingSelectId = null;
            var scrollIndex = _pendingScrollIndex;
            _pendingScrollIndex = null;
            ApplySelectionAndScroll(selectId, scrollIndex);
        }

        private void ApplySelectionAndScroll(int selectId, int? scrollIndex)
        {
            _dgvMeters.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvMeters.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvMeters.Visible = true;
                        _dgvMeters.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvMeters.ClearSelection();
                targetRow.Selected = true;
                _dgvMeters.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvMeters.Rows.Count)
                {
                    _dgvMeters.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvMeters.Visible = true;
                    _dgvMeters.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new MeterForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadMetersAsync();
            }
        }

        private async void BtnCopyAndAdd_Click(object? sender, EventArgs e)
        {
            if (_dgvMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする親メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var meter = await MeterDataAccess.GetMeterByIdAsync(id);
            if (meter != null)
            {
                var form = new MeterForm(meter, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadMetersAsync();
                }
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
            var currentScrollIndex = _dgvMeters.FirstDisplayedScrollingRowIndex;
            var meter = await MeterDataAccess.GetMeterByIdAsync(id);
            if (meter != null)
            {
                var form = new MeterForm(meter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadMetersAsync(id, currentScrollIndex);
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

        private async void DgvMeters_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvMeters!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvMeters.FirstDisplayedScrollingRowIndex;
            var meter = await MeterDataAccess.GetMeterByIdAsync(id);
            if (meter != null)
            {
                var form = new MeterForm(meter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadMetersAsync(id, currentScrollIndex);
                }
            }
        }
    }
}






