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
    /// 部屋情報管理フォーム
    /// </summary>
    public partial class FloorForm : Form
    {
        private Floor _currentFloor;
        private bool _isEditMode;
        private bool _isCopyMode;

        public FloorForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
            _isCopyMode = false;
        }

        public FloorForm(Floor floor) : this()
        {
            _currentFloor = floor;
            _isEditMode = true;
            _isCopyMode = false;
            LoadFloorData();
        }

        /// <summary>
        /// 既存の部屋データをコピーして新規登録画面を開くコンストラクタ
        /// </summary>
        /// <param name="floor">コピー元の部屋データ</param>
        /// <param name="isCopyMode">コピーモード</param>
        public FloorForm(Floor floor, bool isCopyMode) : this()
        {
            if (isCopyMode)
            {
                _currentFloor = floor;
                _isEditMode = false;
                _isCopyMode = true;
                this.Text = "部屋情報登録";
            }
        }

        private void InitializeComponentAdditional()
        {
            // タイトルを設定（実行時に決定）
            if (_isEditMode)
            {
                this.Text = "部屋情報編集";
            }
            else
            {
                this.Text = "部屋情報登録";
            }

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            this.Load += FloorForm_Load;
        }

        private async void FloorForm_Load(object? sender, EventArgs e)
        {
            await LoadBuildingsAsync();

            if (_isEditMode && _currentFloor != null)
            {
                LoadFloorData();
            }
            else if (_isCopyMode && _currentFloor != null)
            {
                LoadFloorDataForCopy();
            }
        }

        private async Task LoadBuildingsAsync()
        {
            try
            {
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                // ビル名を表示するための匿名型リストを作成
                var buildingList = buildings.Select(b => new
                {
                    Id = b.Id,
                    DisplayName = b.Name
                }).ToList();
                
                cmbBuilding.DataSource = buildingList;
                cmbBuilding.DisplayMember = "DisplayName";
                cmbBuilding.ValueMember = "Id";

                if (_isEditMode && _currentFloor != null)
                {
                    cmbBuilding.SelectedValue = _currentFloor.BuildingId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ビル情報の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFloorData()
        {
            if (_currentFloor == null) return;

            if (cmbBuilding.Items.Count > 0)
            {
                cmbBuilding.SelectedValue = _currentFloor.BuildingId;
            }
            txtFloorName.Text = _currentFloor.FloorName;
            txtFloorArea.Text = _currentFloor.FloorArea.ToString();
        }

        /// <summary>
        /// コピーモード用のデータ読み込み（ビル名をコピーし、部屋名は空にする）
        /// </summary>
        private void LoadFloorDataForCopy()
        {
            if (_currentFloor == null) return;

            if (cmbBuilding.Items.Count > 0)
            {
                cmbBuilding.SelectedValue = _currentFloor.BuildingId;
            }

            // 部屋名は新規入力とする
            txtFloorName.Text = string.Empty;
            // 面積はコピーする
            txtFloorArea.Text = _currentFloor.FloorArea.ToString();
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // バリデーション
                if (cmbBuilding.SelectedValue == null)
                {
                    MessageBox.Show("ビル名を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFloorName.Text))
                {
                    MessageBox.Show("部屋名を入力してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtFloorArea.Text, out var floorArea) || floorArea < 0)
                {
                    MessageBox.Show("部屋面積を正しく入力してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var floor = new Floor
                {
                    BuildingId = (int)cmbBuilding.SelectedValue,
                    FloorName = txtFloorName.Text.Trim(),
                    FloorArea = floorArea
                };

                if (_isEditMode && _currentFloor != null)
                {
                    floor.Id = _currentFloor.Id;
                    await FloorDataAccess.UpdateFloorAsync(floor);
                    MessageBox.Show("部屋情報を更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await FloorDataAccess.AddFloorAsync(floor);
                    MessageBox.Show("部屋情報を登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


