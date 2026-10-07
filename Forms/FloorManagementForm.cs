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
    /// 部屋管理フォーム
    /// </summary>
    public partial class FloorManagementForm : Form
    {
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class FloorGridRow
        {
            public int Id { get; set; }
            public string ビル名 { get; set; } = string.Empty;
            public string 部屋名 { get; set; } = string.Empty;
            public decimal 部屋面積 { get; set; }
        }

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
            _btnCopyAndAdd.Click += BtnCopyAndAdd_Click;
            _dgvFloors.DoubleClick += DgvFloors_DoubleClick;
            _dgvFloors.DataBindingComplete += DgvFloors_DataBindingComplete;
            _dgvFloors.ColumnHeaderMouseClick += DgvFloors_ColumnHeaderMouseClick;
        }

        private async void FloorManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadFloorsAsync();
        }

        private async Task LoadFloorsAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var floors = await FloorDataAccess.GetAllFloorsAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                if (selectId.HasValue)
                {
                    _suppressDisplay = true;
                    _dgvFloors.SuspendLayout();
                    _dgvFloors.Visible = false;
                }

                var rows = floors.Select(f => new FloorGridRow
                {
                    Id = f.Id,
                    ビル名 = buildings.FirstOrDefault(b => b.Id == f.BuildingId)?.Name ?? "",
                    部屋名 = f.FloorName,
                    部屋面積 = f.FloorArea
                });

                _dgvFloors!.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

                // ID列の幅を狭く設定
                if (_dgvFloors.Columns["Id"] != null)
                {
                    _dgvFloors.Columns["Id"].Width = 40;
                }

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    await Task.Delay(50);
                    ApplySelectionAndScroll(selectId.Value, scrollIndex);
                }
                else
                {
                    _suppressDisplay = false;
                    _dgvFloors.Visible = true;
                    _dgvFloors.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvFloors_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvFloors.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvFloors.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    return;
                }

                _dgvFloors.ClearSelection();
                targetRow.Selected = true;
                _dgvFloors.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvFloors.Rows.Count)
                {
                    _dgvFloors.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvFloors.Visible = true;
                    _dgvFloors.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private IEnumerable<FloorGridRow> ApplySort(IEnumerable<FloorGridRow> source)
        {
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => source.OrderBy(x => x.Id),
                ("Id", false) => source.OrderByDescending(x => x.Id),
                ("ビル名", true) => source.OrderBy(x => x.ビル名),
                ("ビル名", false) => source.OrderByDescending(x => x.ビル名),
                ("部屋名", true) => source.OrderBy(x => x.部屋名),
                ("部屋名", false) => source.OrderByDescending(x => x.部屋名),
                ("部屋面積", true) => source.OrderBy(x => x.部屋面積),
                ("部屋面積", false) => source.OrderByDescending(x => x.部屋面積),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvFloors.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvFloors.Columns.Contains(_sortColumn))
            {
                _dgvFloors.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvFloors_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvFloors.Columns[e.ColumnIndex].Name;
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
            if (_dgvFloors.SelectedRows.Count > 0 &&
                _dgvFloors.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvFloors.Rows.Count > 0)
            {
                currentScrollIndex = _dgvFloors.FirstDisplayedScrollingRowIndex;
            }

            await LoadFloorsAsync(selectedId, currentScrollIndex);
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
            var currentScrollIndex = _dgvFloors.FirstDisplayedScrollingRowIndex;
            var floor = await FloorDataAccess.GetFloorByIdAsync(id);
            if (floor != null)
            {
                var form = new FloorForm(floor);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadFloorsAsync(id, currentScrollIndex);
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

        private async void BtnCopyAndAdd_Click(object? sender, EventArgs e)
        {
            if (_dgvFloors!.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする部屋を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvFloors.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var floor = await FloorDataAccess.GetFloorByIdAsync(id);
            if (floor != null)
            {
                var form = new FloorForm(floor, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadFloorsAsync(id);
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
            var currentScrollIndex = _dgvFloors.FirstDisplayedScrollingRowIndex;
            var floor = await FloorDataAccess.GetFloorByIdAsync(id);
            if (floor != null)
            {
                var form = new FloorForm(floor);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadFloorsAsync(id, currentScrollIndex);
                }
            }
        }
    }
}

