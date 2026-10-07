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
    /// 取引先管理フォーム
    /// </summary>
    public partial class ClientManagementForm : Form
    {
        private int? _pendingSelectId;
        private int? _pendingScrollIndex;
        private bool _suppressDisplay;
        private string _sortColumn = "Id";
        private bool _sortAscending;

        private sealed class ClientGridRow
        {
            public int Id { get; set; }
            public string 取引先名 { get; set; } = string.Empty;
            public string 取引先対象 { get; set; } = string.Empty;
            public string ビル名 { get; set; } = string.Empty;
            public string 部屋名 { get; set; } = string.Empty;
            public string 郵便番号 { get; set; } = string.Empty;
            public string 住所 { get; set; } = string.Empty;
            public string 電話番号 { get; set; } = string.Empty;
        }

        public ClientManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += ClientManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvClients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvClients.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _btnCopyAndAdd.Click += BtnCopyAndAdd_Click;
            _dgvClients.DoubleClick += DgvClients_DoubleClick;
            _dgvClients.DataBindingComplete += DgvClients_DataBindingComplete;
            _dgvClients.ColumnHeaderMouseClick += DgvClients_ColumnHeaderMouseClick;
        }

        private async void ClientManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadClientsAsync();
        }

        private async Task LoadClientsAsync(int? selectId = null, int? scrollIndex = null)
        {
            try
            {
                var clients = await ClientDataAccess.GetAllClientsAsync();

                if (selectId.HasValue)
                {
                    _pendingSelectId = selectId;
                    _pendingScrollIndex = scrollIndex;
                    _suppressDisplay = true;
                    _dgvClients.SuspendLayout();
                    _dgvClients.Visible = false;
                }

                var rows = clients.Select(c => new ClientGridRow
                {
                    Id = c.Id,
                    取引先名 = c.Name ?? "",
                    取引先対象 = GetClientTargetText(c),
                    ビル名 = c.BuildingName ?? "",
                    部屋名 = c.RoomName ?? "",
                    郵便番号 = c.PostalCode ?? "",
                    住所 = c.Address ?? "",
                    電話番号 = c.Phone ?? ""
                });

                _dgvClients!.DataSource = ApplySort(rows).ToList();
                UpdateSortGlyph();

                // ID列の幅を狭く設定
                if (_dgvClients.Columns["Id"] != null)
                {
                    _dgvClients.Columns["Id"].Width = 40;
                }

                // 取引先名列の幅を広く設定
                if (_dgvClients.Columns["取引先名"] != null)
                {
                    _dgvClients.Columns["取引先名"].Width = 150;
                }

                if (!selectId.HasValue)
                {
                    _suppressDisplay = false;
                    _dgvClients.Visible = true;
                    _dgvClients.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                if (_suppressDisplay)
                {
                    _dgvClients.Visible = true;
                    _dgvClients.ResumeLayout();
                    _suppressDisplay = false;
                }
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<ClientGridRow> ApplySort(IEnumerable<ClientGridRow> source)
        {
            return (_sortColumn, _sortAscending) switch
            {
                ("Id", true) => source.OrderBy(x => x.Id),
                ("Id", false) => source.OrderByDescending(x => x.Id),
                ("取引先名", true) => source.OrderBy(x => x.取引先名),
                ("取引先名", false) => source.OrderByDescending(x => x.取引先名),
                ("取引先対象", true) => source.OrderBy(x => x.取引先対象),
                ("取引先対象", false) => source.OrderByDescending(x => x.取引先対象),
                ("ビル名", true) => source.OrderBy(x => x.ビル名),
                ("ビル名", false) => source.OrderByDescending(x => x.ビル名),
                ("部屋名", true) => source.OrderBy(x => x.部屋名),
                ("部屋名", false) => source.OrderByDescending(x => x.部屋名),
                ("郵便番号", true) => source.OrderBy(x => x.郵便番号),
                ("郵便番号", false) => source.OrderByDescending(x => x.郵便番号),
                ("住所", true) => source.OrderBy(x => x.住所),
                ("住所", false) => source.OrderByDescending(x => x.住所),
                ("電話番号", true) => source.OrderBy(x => x.電話番号),
                ("電話番号", false) => source.OrderByDescending(x => x.電話番号),
                _ => source.OrderByDescending(x => x.Id)
            };
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in _dgvClients.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (_dgvClients.Columns.Contains(_sortColumn))
            {
                _dgvClients.Columns[_sortColumn].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private async void DgvClients_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var clickedColumnName = _dgvClients.Columns[e.ColumnIndex].Name;
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
            if (_dgvClients.SelectedRows.Count > 0 &&
                _dgvClients.SelectedRows[0].Cells["Id"].Value is int id)
            {
                selectedId = id;
            }

            if (_dgvClients.Rows.Count > 0)
            {
                currentScrollIndex = _dgvClients.FirstDisplayedScrollingRowIndex;
            }

            await LoadClientsAsync(selectedId, currentScrollIndex);
        }

        private void DgvClients_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
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
            _dgvClients.BeginInvoke(new Action(() =>
            {
                var targetRow = _dgvClients.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row =>
                        row.Cells["Id"].Value != null
                        && row.Cells["Id"].Value != DBNull.Value
                        && Convert.ToInt32(row.Cells["Id"].Value) == selectId);
                if (targetRow == null)
                {
                    if (_suppressDisplay)
                    {
                        _dgvClients.Visible = true;
                        _dgvClients.ResumeLayout();
                        _suppressDisplay = false;
                    }
                    return;
                }

                _dgvClients.ClearSelection();
                targetRow.Selected = true;
                _dgvClients.CurrentCell = targetRow.Cells["Id"];

                var index = scrollIndex ?? targetRow.Index;
                if (index >= 0 && index < _dgvClients.Rows.Count)
                {
                    _dgvClients.FirstDisplayedScrollingRowIndex = index;
                }

                if (_suppressDisplay)
                {
                    _dgvClients.Visible = true;
                    _dgvClients.ResumeLayout();
                    _suppressDisplay = false;
                }
            }));
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new ClientForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadClientsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvClients!.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する取引先を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvClients.SelectedRows[0];
            var id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var currentScrollIndex = _dgvClients.FirstDisplayedScrollingRowIndex;
            var client = await ClientDataAccess.GetClientByIdAsync(id);
            if (client != null)
            {
                var form = new ClientForm(client);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadClientsAsync(id, currentScrollIndex);
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvClients!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する取引先を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvClients.SelectedRows[0];
            var clientName = selectedRow.Cells["取引先名"].Value?.ToString() ?? "ID:" + selectedRow.Cells["Id"].Value.ToString();
            var result = MessageBox.Show($"取引先「{clientName}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                    var success = await ClientDataAccess.DeleteClientAsync(id);
                    if (success)
                    {
                        MessageBox.Show("取引先を削除しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadClientsAsync();
                    }
                    else
                    {
                        MessageBox.Show("取引先の削除に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnCopyAndAdd_Click(object? sender, EventArgs e)
        {
            if (_dgvClients!.SelectedRows.Count == 0)
            {
                MessageBox.Show("コピーして追加する取引先を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = _dgvClients.SelectedRows[0];
            var id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var client = await ClientDataAccess.GetClientByIdAsync(id);
            if (client != null)
            {
                var form = new ClientForm(client, true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadClientsAsync();
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadClientsAsync();
        }

        private async void DgvClients_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvClients!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvClients.SelectedRows[0];
            var id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var currentScrollIndex = _dgvClients.FirstDisplayedScrollingRowIndex;
            var client = await ClientDataAccess.GetClientByIdAsync(id);
            if (client != null)
            {
                var form = new ClientForm(client);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadClientsAsync(id, currentScrollIndex);
                }
            }
        }

        private string GetClientTargetText(Client client)
        {
            var targets = new List<string>();
            if (client.IsLessor) targets.Add("貸主");
            if (client.IsLessee) targets.Add("借主");
            if (client.IsBillingTo) targets.Add("請求先");
            if (client.IsContractor) targets.Add("業者");
            return targets.Count > 0 ? string.Join("、", targets) : "";
        }
    }
}

