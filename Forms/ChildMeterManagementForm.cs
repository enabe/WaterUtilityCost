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
        private async Task LoadChildMetersAsync()
        {
            try
            {
                var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
                var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                var floors = await FloorDataAccess.GetAllFloorsAsync();

                _dgvChildMeters!.DataSource = childMeters.Select(cm =>
                {
                    var building = buildings.FirstOrDefault(b => b.Id == cm.BuildingId);
                    var floor = floors.FirstOrDefault(f => f.Id == cm.RoomId);
                    return new
                    {
                        Id = cm.Id,
                        ビル名 = building?.Name ?? "",
                        部屋名 = floor?.FloorName ?? "",
                        メーター種別 = cm.MeterType ?? ""
                    };
                }).OrderByDescending(x => x.Id).ToList();

                // ID列の幅を狭く設定
                if (_dgvChildMeters.Columns["Id"] != null)
                {
                    _dgvChildMeters.Columns["Id"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"子メーターデータの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 新規登録ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new ChildMeterForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadChildMetersAsync();
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
            var childMeter = await ChildMeterDataAccess.GetChildMeterByIdAsync(id);
            if (childMeter != null)
            {
                var form = new ChildMeterForm(childMeter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadChildMetersAsync();
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
    }
}



