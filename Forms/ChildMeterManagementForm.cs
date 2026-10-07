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
    /// 子メーター管理フォーム
    /// </summary>
    public partial class ChildMeterManagementForm : Form
    {
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class ChildMeterGridRow
        {
            public int Id { get; set; }
            public string ビル名 { get; set; } = string.Empty;
            public string メーター種別 { get; set; } = string.Empty;
            public string 親メーター名 { get; set; } = string.Empty;
            public string 子メーター名 { get; set; } = string.Empty;
        }
        /// <summary>
        /// 子メーター管理フォームのコンストラクタ
        /// </summary>
        public ChildMeterManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += ChildMeterManagementForm_Load;
        }

        /// <summary>
        /// 追加のコンポーネント初期化処理
        /// </summary>
        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvChildMeters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvChildMeters.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _btnCopyAndAdd.Click += BtnCopyAndAdd_Click;
            _dgvChildMeters.DoubleClick += DgvChildMeters_DoubleClick;
            _dgvChildMeters.DataBindingComplete += DgvChildMeters_DataBindingComplete;
            _dgvChildMeters.ColumnHeaderMouseClick += DgvChildMeters_ColumnHeaderMouseClick;
        }

        /// <summary>
        /// フォーム読み込み時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void ChildMeterManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadChildMetersAsync();
        }

        /// <summary>
        /// 子メーター一覧を読み込む
        /// </summary>
        private async Task LoadChildMetersAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var meters = await MeterDataAccess.GetAllMetersAsync();

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvChildMeters.SuspendLayout();
                    _dgvChildMeters.Visible = false;
                }

                var rows = childMeters.Select(cm =>
                {
                    var building = buildings.FirstOrDefault(b => b.Id == cm.BuildingId);
                    var parentMeter = cm.ParentMeterId.HasValue
                        ? meters.FirstOrDefault(m => m.Id == cm.ParentMeterId.Value)
                        : null;
                    var parentMeterName = parentMeter != null && !string.IsNullOrEmpty(parentMeter.MeterName)
                        ? parentMeter.MeterName
                        : parentMeter != null
                            ? $"親メーターID: {parentMeter.Id}"
                            : "";
                    return new ChildMeterGridRow
                    {
                        Id = cm.Id,
                        ビル名 = building?.Name ?? "",
                        メーター種別 = cm.MeterType ?? "",
                        親メーター名 = parentMeterName,
                        子メーター名 = cm.MeterName ?? ""
                    };
                });

                _dgvChildMeters!.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

                // ID列の幅を狭く設定
                if (_dgvChildMeters.Columns["Id"] != null)
                {
                    _dgvChildMeters.Columns["Id"].Width = 40;
                }
                if (_dgvChildMeters.Columns["メーター種別"] != null)
                {
                    _dgvChildMeters.Columns["メーター種別"].FillWeight = 60;
                }

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvChildMeters.Visible = true;
                    _dgvChildMeters.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvChildMeters.Visible = true;
                    _dgvChildMeters.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"子メーターデータの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvChildMeters_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvChildMeters.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvChildMeters.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["Id"].Value is int id && id == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvChildMeters.Visible = true;
                        _dgvChildMeters.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvChildMeters.ClearSelection();
                targetRow.Selected = true;
                _dgvChildMeters.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvChildMeters.Rows.Count)
                {
                    _dgvChildMeters.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvChildMeters.Visible = true;
                    _dgvChildMeters.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private IEnumerable<ChildMeterGridRow> ApplySort(IEnumerable<ChildMeterGridRow> source)
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
                ("子メーター名", true) => source.OrderBy(x => x.子メーター名),
                ("子メーター名", false) => source.OrderByDescending(x => x.子メーター名),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvChildMeters.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvChildMeters.Columns.Contains(_sortColumn))
            {
                _dgvChildMeters.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvChildMeters_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvChildMeters.Columns[e.ColumnIndex].Name;
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
            if (_dgvChildMeters.SelectedRows.Count > 0 &&
                _dgvChildMeters.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvChildMeters.Rows.Count > 0)
            {
                currentScrollIndex = _dgvChildMeters.FirstDisplayedScrollingRowIndex;
            }

            await LoadChildMetersAsync(selectedId, currentScrollIndex);
        }

        /// <summary>
        /// 新規登録ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            int? selectedId = null;
            int? currentScrollIndex = null;
            if (_dgvChildMeters.SelectedRows.Count > 0 &&
                _dgvChildMeters.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }
            if (_dgvChildMeters.Rows.Count > 0)
            {
                currentScrollIndex = _dgvChildMeters.FirstDisplayedScrollingRowIndex;
            }

            var form = new ChildMeterForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadChildMetersAsync(selectedId, currentScrollIndex);
            }
        }

        /// <summary>
        /// 編集ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvChildMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する子メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvChildMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvChildMeters.FirstDisplayedScrollingRowIndex;
            var childMeter = await ChildMeterDataAccess.GetChildMeterByIdAsync(id);
            if (childMeter != null)
            {
                var form = new ChildMeterForm(childMeter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadChildMetersAsync(id, currentScrollIndex);
                }
            }
        }

        /// <summary>
        /// 削除ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvChildMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する子メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvChildMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var result = MessageBox.Show($"子メーター（ID: {id}）を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    await ChildMeterDataAccess.DeleteChildMeterAsync(id);
                    await LoadChildMetersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 更新ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadChildMetersAsync();
        }

        private async void BtnCopyAndAdd_Click(object? sender, EventArgs e)
        {
            if (_dgvChildMeters!.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーする子メーターを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvChildMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvChildMeters.FirstDisplayedScrollingRowIndex;
            var childMeter = await ChildMeterDataAccess.GetChildMeterByIdAsync(id);
            if (childMeter != null)
            {
                var form = new ChildMeterForm(childMeter, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadChildMetersAsync(id, currentScrollIndex);
                }
            }
        }

        /// <summary>
        /// DataGridViewのダブルクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void DgvChildMeters_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvChildMeters!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvChildMeters.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var currentScrollIndex = _dgvChildMeters.FirstDisplayedScrollingRowIndex;
            var childMeter = await ChildMeterDataAccess.GetChildMeterByIdAsync(id);
            if (childMeter != null)
            {
                var form = new ChildMeterForm(childMeter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadChildMetersAsync(id, currentScrollIndex);
                }
            }
        }
    }
}



