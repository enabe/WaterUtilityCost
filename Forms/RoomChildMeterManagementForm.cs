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
        private int? _baseBuildingWidth;
        private int? _baseRoomNameWidth;
        private int? _baseParentMeterWidth;
        private int? _baseChildMeterWidth;
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class RoomChildMeterGridRow
        {
            public int Id { get; set; }
            public string ビル名 { get; set; } = string.Empty;
            public string 部屋名 { get; set; } = string.Empty;
            public string メーター種別 { get; set; } = string.Empty;
            public string 親メーター名 { get; set; } = string.Empty;
            public string 子メーター名 { get; set; } = string.Empty;
        }

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
            _btnCopyAndAdd.Click += BtnCopyAndAdd_Click;
            _dgvRoomChildMeters.DoubleClick += DgvRoomChildMeters_DoubleClick;
            _dgvRoomChildMeters.DataBindingComplete += DgvRoomChildMeters_DataBindingComplete;
            _dgvRoomChildMeters.ColumnHeaderMouseClick += DgvRoomChildMeters_ColumnHeaderMouseClick;
        }

        private async void RoomChildMeterManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadRoomChildMetersAsync();
        }

        private async Task LoadRoomChildMetersAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var roomChildMeters = await RoomChildMeterDataAccess.GetAllRoomChildMetersAsync();
                var floors = await FloorDataAccess.GetAllFloorsAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
                var meters = await MeterDataAccess.GetAllMetersAsync();

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvRoomChildMeters.SuspendLayout();
                    _dgvRoomChildMeters.Visible = false;
                }

                var rows = roomChildMeters.Select(rcm =>
                {
                    var floor = floors.FirstOrDefault(f => f.Id == rcm.FloorId);
                    var building = floor != null ? buildings.FirstOrDefault(b => b.Id == floor.BuildingId) : null;
                    var buildingName = building?.Name ?? "";
                    var roomName = floor?.FloorName ?? "";

                    var childMeter = childMeters.FirstOrDefault(cm => cm.Id == rcm.ChildMeterId);
                    var meterType = childMeter?.MeterType ?? "";

                    var parentMeterName = "";
                    if (childMeter?.ParentMeterId.HasValue == true)
                    {
                        var parentMeter = meters.FirstOrDefault(m => m.Id == childMeter.ParentMeterId.Value);
                        parentMeterName = parentMeter != null && !string.IsNullOrEmpty(parentMeter.MeterName)
                            ? parentMeter.MeterName
                            : parentMeter != null
                                ? $"親メーターID: {parentMeter.Id}"
                                : "";
                    }

                    var childMeterName = childMeter != null && !string.IsNullOrEmpty(childMeter.MeterName)
                        ? childMeter.MeterName
                        : childMeter != null
                            ? $"子メーターID: {childMeter.Id}"
                            : "";

                    return new RoomChildMeterGridRow
                    {
                        Id = rcm.Id,
                        ビル名 = buildingName,
                        部屋名 = roomName,
                        メーター種別 = meterType,
                        親メーター名 = parentMeterName,
                        子メーター名 = childMeterName
                    };
                });

                _dgvRoomChildMeters!.DataSource = ApplySort(rows).ToList();

                // ID列の幅を狭く設定
                if (_dgvRoomChildMeters.Columns["Id"] != null)
                {
                    _dgvRoomChildMeters.Columns["Id"].Width = 40;
                }
                
                // 現在の幅を初回のみ保持
                if (_baseBuildingWidth == null && _dgvRoomChildMeters.Columns["ビル名"] != null)
                {
                    _baseBuildingWidth = _dgvRoomChildMeters.Columns["ビル名"].Width;
                }
                if (_baseRoomNameWidth == null && _dgvRoomChildMeters.Columns["部屋名"] != null)
                {
                    _baseRoomNameWidth = _dgvRoomChildMeters.Columns["部屋名"].Width;
                }
                // ビル名列の幅は固定（初回の幅を維持）
                if (_dgvRoomChildMeters.Columns["ビル名"] != null)
                {
                    var baseWidth = _baseBuildingWidth ?? _dgvRoomChildMeters.Columns["ビル名"].Width;
                    _dgvRoomChildMeters.Columns["ビル名"].Width = baseWidth;
                }

                if (_baseParentMeterWidth == null && _dgvRoomChildMeters.Columns["親メーター名"] != null)
                {
                    _baseParentMeterWidth = _dgvRoomChildMeters.Columns["親メーター名"].Width;
                }
                if (_baseChildMeterWidth == null && _dgvRoomChildMeters.Columns["子メーター名"] != null)
                {
                    _baseChildMeterWidth = _dgvRoomChildMeters.Columns["子メーター名"].Width;
                }

                // 部屋名列の幅を60pt狭く設定
                if (_dgvRoomChildMeters.Columns["部屋名"] != null)
                {
                    var baseWidth = _baseRoomNameWidth ?? _dgvRoomChildMeters.Columns["部屋名"].Width;
                    _dgvRoomChildMeters.Columns["部屋名"].Width = Math.Max(20, baseWidth - 60);
                }
                
                if (_dgvRoomChildMeters.Columns["メーター種別"] != null)
                {
                    _dgvRoomChildMeters.Columns["メーター種別"].FillWeight = 60;
                }

                // 親メーター名列の幅を20pt狭く設定
                if (_dgvRoomChildMeters.Columns["親メーター名"] != null)
                {
                    var baseWidth = _baseParentMeterWidth ?? _dgvRoomChildMeters.Columns["親メーター名"].Width;
                    _dgvRoomChildMeters.Columns["親メーター名"].Width = Math.Max(20, baseWidth - 20);
                }

                // 子メーター名列の幅を20pt狭く設定
                if (_dgvRoomChildMeters.Columns["子メーター名"] != null)
                {
                    var baseWidth = _baseChildMeterWidth ?? _dgvRoomChildMeters.Columns["子メーター名"].Width;
                    _dgvRoomChildMeters.Columns["子メーター名"].Width = Math.Max(20, baseWidth - 20);
                }

                UpdateSortGlyph();

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvRoomChildMeters.Visible = true;
                    _dgvRoomChildMeters.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvRoomChildMeters.Visible = true;
                    _dgvRoomChildMeters.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvRoomChildMeters_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvRoomChildMeters.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvRoomChildMeters.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvRoomChildMeters.Visible = true;
                        _dgvRoomChildMeters.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvRoomChildMeters.ClearSelection();
                targetRow.Selected = true;
                _dgvRoomChildMeters.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvRoomChildMeters.Rows.Count)
                {
                    _dgvRoomChildMeters.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvRoomChildMeters.Visible = true;
                    _dgvRoomChildMeters.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private IEnumerable<RoomChildMeterGridRow> ApplySort(IEnumerable<RoomChildMeterGridRow> source)
        {
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => source.OrderBy(x => x.Id),
                ("Id", false) => source.OrderByDescending(x => x.Id),
                ("ビル名", true) => source.OrderBy(x => x.ビル名),
                ("ビル名", false) => source.OrderByDescending(x => x.ビル名),
                ("部屋名", true) => source.OrderBy(x => x.部屋名),
                ("部屋名", false) => source.OrderByDescending(x => x.部屋名),
                ("メーター種別", true) => source.OrderBy(x => x.メーター種別),
                ("メーター種別", false) => source.OrderByDescending(x => x.メーター種別),
                ("親メーター名", true) => source.OrderBy(x => x.親メーター名),
                ("親メーター名", false) => source.OrderByDescending(x => x.親メーター名),
                ("子メーター名", true) => source.OrderBy(x => x.子メーター名),
                ("子メーター名", false) => source.OrderByDescending(x => x.子メーター名),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvRoomChildMeters.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvRoomChildMeters.Columns.Contains(_sortColumn))
            {
                _dgvRoomChildMeters.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvRoomChildMeters_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvRoomChildMeters.Columns[e.ColumnIndex].Name;
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
            if (_dgvRoomChildMeters.SelectedRows.Count > 0 &&
                _dgvRoomChildMeters.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvRoomChildMeters.Rows.Count > 0)
            {
                currentScrollIndex = _dgvRoomChildMeters.FirstDisplayedScrollingRowIndex;
            }

            await LoadRoomChildMetersAsync(selectedId, currentScrollIndex);
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
            var currentScrollIndex = _dgvRoomChildMeters.FirstDisplayedScrollingRowIndex;
            var roomChildMeter = await RoomChildMeterDataAccess.GetRoomChildMeterByIdAsync(id);
            if (roomChildMeter != null)
            {
                var form = new RoomChildMeterForm(roomChildMeter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadRoomChildMetersAsync(id, currentScrollIndex);
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

        private async void BtnCopyAndAdd_Click(object? sender, EventArgs e)
        {
            if (_dgvRoomChildMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする部屋別子メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvRoomChildMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var roomChildMeter = await RoomChildMeterDataAccess.GetRoomChildMeterByIdAsync(id);
            if (roomChildMeter != null)
            {
                var form = new RoomChildMeterForm(roomChildMeter, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadRoomChildMetersAsync();
                }
            }
        }

        private async void DgvRoomChildMeters_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvRoomChildMeters!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvRoomChildMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvRoomChildMeters.FirstDisplayedScrollingRowIndex;
            var roomChildMeter = await RoomChildMeterDataAccess.GetRoomChildMeterByIdAsync(id);
            if (roomChildMeter != null)
            {
                var form = new RoomChildMeterForm(roomChildMeter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadRoomChildMetersAsync(id, currentScrollIndex);
                }
            }
        }
    }
}







