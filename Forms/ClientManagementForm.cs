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
        }

        private async void ClientManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadClientsAsync();
        }

        private async Task LoadClientsAsync()
        {
            try
            {
                var clients = await ClientDataAccess.GetAllClientsAsync();

                _dgvClients!.DataSource = clients.Select(c => new
                {
                    Id = c.Id,
                    取引先名 = c.Name,
                    取引先対象 = GetClientTargetText(c),
                    郵便番号 = c.PostalCode,
                    住所 = c.Address,
                    電話番号 = c.Phone
                }).OrderByDescending(x => x.Id).ToList();

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            var id = (int)selectedRow.Cells["Id"].Value;
            var client = await ClientDataAccess.GetClientByIdAsync(id);
            if (client != null)
            {
                var form = new ClientForm(client);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadClientsAsync();
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
                    var id = (int)selectedRow.Cells["Id"].Value;
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

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadClientsAsync();
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

