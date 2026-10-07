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
    /// ビル管理フォーム
    /// </summary>
    public partial class BuildingManagementForm : Form
    {
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class BuildingGridRow
        {
            public int Id { get; set; }
            public string ビルID { get; set; } = string.Empty;
            public string ビル名 { get; set; } = string.Empty;
            public string 住所 { get; set; } = string.Empty;
            public int 階数 { get; set; }
        }

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
            _dgvBuildings.DataBindingComplete += DgvBuildings_DataBindingComplete;
            _dgvBuildings.ColumnHeaderMouseClick += DgvBuildings_ColumnHeaderMouseClick;
        }

        private async void BuildingManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();
        }

        private async Task LoadBuildingsAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvBuildings.SuspendLayout();
                    _dgvBuildings.Visible = false;
                }

                var rows = buildings.Select(b => new BuildingGridRow
                {
                    Id = b.Id,
                    ビルID = b.BuildingId ?? "",
                    ビル名 = b.Name ?? "",
                    住所 = b.Address ?? "",
                    階数 = b.Floors
                });

                _dgvBuildings!.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

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

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvBuildings.Visible = true;
                    _dgvBuildings.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvBuildings.Visible = true;
                    _dgvBuildings.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<BuildingGridRow> ApplySort(IEnumerable<BuildingGridRow> source)
        {
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => source.OrderBy(x => x.Id),
                ("Id", false) => source.OrderByDescending(x => x.Id),
                ("ビルID", true) => source.OrderBy(x => x.ビルID),
                ("ビルID", false) => source.OrderByDescending(x => x.ビルID),
                ("ビル名", true) => source.OrderBy(x => x.ビル名),
                ("ビル名", false) => source.OrderByDescending(x => x.ビル名),
                ("住所", true) => source.OrderBy(x => x.住所),
                ("住所", false) => source.OrderByDescending(x => x.住所),
                ("階数", true) => source.OrderBy(x => x.階数),
                ("階数", false) => source.OrderByDescending(x => x.階数),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvBuildings.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvBuildings.Columns.Contains(_sortColumn))
            {
                _dgvBuildings.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvBuildings_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvBuildings.Columns[e.ColumnIndex].Name;
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
            if (_dgvBuildings.SelectedRows.Count > 0 &&
                _dgvBuildings.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvBuildings.Rows.Count > 0)
            {
                currentScrollIndex = _dgvBuildings.FirstDisplayedScrollingRowIndex;
            }

            await LoadBuildingsAsync(selectedId, currentScrollIndex);
        }

        private void DgvBuildings_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvBuildings.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvBuildings.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvBuildings.Visible = true;
                        _dgvBuildings.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvBuildings.ClearSelection();
                targetRow.Selected = true;
                _dgvBuildings.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvBuildings.Rows.Count)
                {
                    _dgvBuildings.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvBuildings.Visible = true;
                    _dgvBuildings.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
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
            var currentScrollIndex = _dgvBuildings.FirstDisplayedScrollingRowIndex;
            var building = await BuildingDataAccess.GetBuildingByIdAsync(id);
            if (building != null)
            {
                var form = new BuildingForm(building);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadBuildingsAsync(id, currentScrollIndex);
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
            var currentScrollIndex = _dgvBuildings.FirstDisplayedScrollingRowIndex;
            var building = await BuildingDataAccess.GetBuildingByIdAsync(id);
            if (building != null)
            {
                var form = new BuildingForm(building);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadBuildingsAsync(id, currentScrollIndex);
                }
            }
        }
    }
}

