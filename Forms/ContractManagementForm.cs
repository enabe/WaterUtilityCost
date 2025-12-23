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
    /// 契約管理フォーム
    /// </summary>
    public partial class ContractManagementForm : Form
    {

        public ContractManagementForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += ContractManagementForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            // DataGridViewのヘッダー高さを調整（フォントサイズに合わせて）
            _dgvContracts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvContracts.ColumnHeadersHeight = 30;
            
            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
        }

        private async void ContractManagementForm_Load(object? sender, EventArgs e)
        {
            await LoadContractsAsync();
        }

        private async Task LoadContractsAsync()
        {
            try
            {
                var contracts = await ContractDataAccess.GetAllContractsAsync();
                var clients = await ClientDataAccess.GetAllClientsAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                _dgvContracts!.DataSource = contracts.Select(c => new
                {
                    Id = c.Id,
                    契約番号 = c.ContractNumber,
                    契約種別 = c.ContractType,
                    契約者名 = c.ContractorName,
                    貸主取引先 = GetClientName(clients, c.LessorClientId),
                    借主取引先 = GetClientName(clients, c.LesseeClientId),
                    請求取引先 = GetClientName(clients, c.BillingClientId),
                    対象開始日 = c.StartDate?.ToString("yyyy-MM-dd") ?? "",
                    対象終了日 = c.EndDate?.ToString("yyyy-MM-dd") ?? "",
                    契約状況 = c.ContractStatus,
                    締日 = c.ClosingDate?.ToString() ?? "",
                    ビル名 = GetBuildingName(buildings, c.BuildingId),
                    お客様番号 = c.CustomerNumber
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvContracts.Columns["Id"] != null)
                {
                    _dgvContracts.Columns["Id"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetClientName(List<Client> clients, int? clientId)
        {
            if (!clientId.HasValue) return "";
            var client = clients.FirstOrDefault(c => c.Id == clientId.Value);
            return client?.Name ?? "";
        }

        private string GetBuildingName(List<Building> buildings, int? buildingId)
        {
            if (!buildingId.HasValue) return "";
            var building = buildings.FirstOrDefault(b => b.Id == buildingId.Value);
            return building?.Name ?? "";
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var form = new ContractForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadContractsAsync();
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvContracts.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する契約を選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = _dgvContracts.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var contract = await ContractDataAccess.GetContractByIdAsync(id);

            if (contract != null)
            {
                using var form = new ContractForm(contract);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadContractsAsync();
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvContracts.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する契約を選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = _dgvContracts.SelectedRows[0];
            var id = (int)selectedRow.Cells["Id"].Value;
            var contractNumber = selectedRow.Cells["契約番号"].Value?.ToString() ?? "";

            var result = MessageBox.Show($"契約番号「{contractNumber}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    await ContractDataAccess.DeleteContractAsync(id);
                    MessageBox.Show("契約を削除しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadContractsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadContractsAsync();
        }
    }
}

