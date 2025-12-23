using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Models;
using WaterUtilityCost.DataAccess;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// ビル情報管理フォーム
    /// </summary>
    public partial class BuildingForm : Form
    {
        private Building _currentBuilding;
        private bool _isEditMode;

        /// <summary>
        /// ビル情報登録フォームのコンストラクタ（新規登録用）
        /// </summary>
        public BuildingForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
            _isEditMode = false;
        }

        /// <summary>
        /// ビル情報編集フォームのコンストラクタ（編集用）
        /// </summary>
        /// <param name="building">編集対象のビル情報</param>
        public BuildingForm(Building building) : this()
        {
            _currentBuilding = building;
            _isEditMode = true;
            LoadBuildingData();
        }

        /// <summary>
        /// 追加のコンポーネント初期化処理
        /// </summary>
        private void InitializeComponentAdditional()
        {
            // タイトルを設定（実行時に決定）
            if (_isEditMode)
            {
                this.Text = "ビル情報編集";
            }
            else
            {
                this.Text = "ビル情報登録";
            }

            // イベントハンドラー
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        /// <summary>
        /// キャンセルボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// ビル情報をフォームに読み込む
        /// </summary>
        private void LoadBuildingData()
        {
            if (_currentBuilding == null) return;

            txtBuildingId.Text = _currentBuilding.BuildingId;
            txtName.Text = _currentBuilding.Name;
            txtAddress.Text = _currentBuilding.Address;
            txtFloors.Text = _currentBuilding.Floors.ToString();
            if (_currentBuilding.BuiltDate != DateTime.MinValue)
                txtBuiltDate.Text = _currentBuilding.BuiltDate.ToString("yyyy-MM-dd");
            txtArea.Text = _currentBuilding.Area.ToString();
            txtOwner.Text = _currentBuilding.Owner;
            txtContact.Text = _currentBuilding.Contact;
        }

        /// <summary>
        /// 保存ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                DateTime builtDate = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(txtBuiltDate.Text))
                {
                    // 複数の日付形式に対応
                    string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "yyyyMMdd", "yyyy-M-d", "yyyy/M/d" };
                    if (!DateTime.TryParseExact(txtBuiltDate.Text.Trim(), formats, null, System.Globalization.DateTimeStyles.None, out builtDate))
                    {
                        // TryParseExactで失敗した場合、通常のTryParseを試す
                        if (!DateTime.TryParse(txtBuiltDate.Text.Trim(), out builtDate))
                        {
                            // 日付形式が正しくない場合でも、未入力として扱う（エラーにしない）
                            builtDate = DateTime.MinValue;
                        }
                    }
                }

                // 階数のバリデーション
                if (string.IsNullOrWhiteSpace(txtFloors.Text))
                {
                    MessageBox.Show("階数を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtFloors.Text, out var floors) || floors <= 0)
                {
                    MessageBox.Show("階数は正の整数で入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var building = new Building
                {
                    BuildingId = txtBuildingId.Text.Trim(),
                    Name = txtName.Text,
                    Address = txtAddress.Text,
                    Floors = floors,
                    BuiltDate = builtDate,
                    Area = decimal.TryParse(txtArea.Text, out var area) ? area : 0,
                    Owner = txtOwner.Text,
                    Contact = txtContact.Text
                };

                if (_isEditMode && _currentBuilding != null)
                {
                    building.Id = _currentBuilding.Id;
                    await BuildingDataAccess.UpdateBuildingAsync(building);
                    MessageBox.Show("ビル情報を更新しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await BuildingDataAccess.AddBuildingAsync(building);
                    MessageBox.Show("ビル情報を登録しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

