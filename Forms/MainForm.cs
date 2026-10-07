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
    /// メインフォーム - ビル管理システム
    /// </summary>
    public partial class MainForm : Form
    {
        private DataGridView _dgvBuildings;
        private Button _btnAdd;
        private Button _btnEdit;
        private Button _btnDelete;
        private Button _btnRefresh;
        private Button _btnClientManagement;
        private Button _btnInvoiceDetails;

        public MainForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            this.Load += MainForm_Load;
        }

        private void InitializeComponentAdditional()
        {
            this.Text = "ビル管理システム";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // データグリッドビュー
            _dgvBuildings = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(840, 350),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // ボタン
            _btnAdd = new Button { Text = "新規登録", Location = new Point(20, 20), Size = new Size(100, 30) };
            _btnEdit = new Button { Text = "編集", Location = new Point(130, 20), Size = new Size(100, 30) };
            _btnDelete = new Button { Text = "削除", Location = new Point(240, 20), Size = new Size(100, 30) };
            _btnRefresh = new Button { Text = "更新", Location = new Point(350, 20), Size = new Size(100, 30) };
            _btnClientManagement = new Button { Text = "取引先管理", Location = new Point(460, 20), Size = new Size(100, 30) };
            _btnInvoiceDetails = new Button { Text = "水道光熱費請求明細一覧", Location = new Point(570, 20), Size = new Size(180, 30) };

            // イベントハンドラー
            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnRefresh.Click += BtnRefresh_Click;
            _btnClientManagement.Click += BtnClientManagement_Click;
            _btnInvoiceDetails.Click += BtnInvoiceDetails_Click;
            _dgvBuildings.DoubleClick += DgvBuildings_DoubleClick;

            // コントロールを追加
            this.Controls.Add(_dgvBuildings);
            this.Controls.Add(_btnAdd);
            this.Controls.Add(_btnEdit);
            this.Controls.Add(_btnDelete);
            this.Controls.Add(_btnRefresh);
            this.Controls.Add(_btnClientManagement);
            this.Controls.Add(_btnInvoiceDetails);

            // ステータスバー
            var statusStrip = new StatusStrip();
            var statusLabel = new ToolStripStatusLabel("準備完了");
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();
        }

        private async Task LoadBuildingsAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();

                _dgvBuildings!.DataSource = buildings.Select(b => new
                {
                    Id = b.Id,
                    ビル名 = b.Name,
                    住所 = b.Address
                }).OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            var buildingId = Convert.ToInt32(selectedRow.Cells["Id"].Value);

            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var building = buildings.FirstOrDefault(b => b.Id == buildingId);

                if (building != null)
                {
                    var form = new BuildingForm(building);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        await LoadBuildingsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dgvBuildings!.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除するビルを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("本当に削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var selectedRow = _dgvBuildings.SelectedRows[0];
                var buildingId = Convert.ToInt32(selectedRow.Cells["Id"].Value);

                try
                {
                    await BuildingDataAccess.DeleteBuildingAsync(buildingId);
                    MessageBox.Show("ビル情報を削除しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void BtnClientManagement_Click(object? sender, EventArgs e)
        {
            var form = new ClientManagementForm();
            form.ShowDialog();
        }

        private void BtnInvoiceDetails_Click(object? sender, EventArgs e)
        {
            var form = new InvoiceDetailForm();
            form.ShowDialog();
        }

        private async void DgvBuildings_DoubleClick(object? sender, EventArgs e)
        {
            if (_dgvBuildings!.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = _dgvBuildings.SelectedRows[0];
            var buildingId = Convert.ToInt32(selectedRow.Cells["Id"].Value);

            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var building = buildings.FirstOrDefault(b => b.Id == buildingId);

                if (building != null)
                {
                    var form = new BuildingForm(building);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        await LoadBuildingsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

