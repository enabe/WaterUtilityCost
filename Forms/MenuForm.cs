using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Database;
using WaterUtilityCost.Tools;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// メニュー画面
    /// </summary>
    public partial class MenuForm : Form
    {

        public MenuForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
        }

        /// <summary>
        /// 追加のコンポーネント初期化処理
        /// </summary>
        private void InitializeComponentAdditional()
        {
            // イベントハンドラー
            _btnBuildingManagement.Click += BtnBuildingManagement_Click;
            _btnFloorManagement.Click += BtnFloorManagement_Click;
            _btnClientManagement.Click += BtnClientManagement_Click;
            _btnInvoiceDetails.Click += BtnInvoiceDetails_Click;
            _btnOtherInvoiceDetails.Click += BtnOtherInvoiceDetails_Click;
            _btnWaterBillingManagement.Click += BtnWaterBillingManagement_Click;
            _btnElectricBillingManagement.Click += BtnElectricBillingManagement_Click;
            _btnGasBillingManagement.Click += BtnGasBillingManagement_Click;
            _btnElectricChildMeterReadingManagement.Click += BtnElectricChildMeterReadingManagement_Click;
            _btnGasChildMeterReadingManagement.Click += BtnGasChildMeterReadingManagement_Click;
            _btnWaterChildMeterReadingManagement.Click += BtnWaterChildMeterReadingManagement_Click;
            _btnContractManagement.Click += BtnContractManagement_Click;
            _btnMeterManagement.Click += BtnMeterManagement_Click;
            _btnChildMeterManagement.Click += BtnChildMeterManagement_Click;
            _btnRoomChildMeterManagement.Click += BtnRoomChildMeterManagement_Click;
            _btnBillingDataRegistration.Click += BtnBillingDataRegistration_Click;
            _btnInvoicePrint.Click += BtnInvoicePrint_Click;
            _btnCreateScreenSpecification.Click += BtnCreateScreenSpecification_Click;
            _btnDeleteAllData.Click += BtnDeleteAllData_Click;
            _btnExit.Click += BtnExit_Click;
        }
        
        /// <summary>
        /// 画面仕様書作成ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnCreateScreenSpecification_Click(object? sender, EventArgs e)
        {
            try
            {
                // 保存先を選択
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "Excelファイル (*.xlsx)|*.xlsx|すべてのファイル (*.*)|*.*",
                    FileName = "画面仕様書.xlsx",
                    Title = "画面仕様書の保存先を選択"
                };
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Excelファイルを作成
                    CreateScreenSpecificationExcel.CreateExcel(saveDialog.FileName);
                    
                    MessageBox.Show(
                        $"画面仕様書Excelファイルを作成しました。\n\nファイル: {saveDialog.FileName}\n\n各シートの「画像」セルにスクリーンショット画像を貼り付けてください。",
                        "完了",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"画面仕様書の作成中にエラーが発生しました。\n\nエラー: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ビル管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnBuildingManagement_Click(object? sender, EventArgs e)
        {
            var form = new BuildingManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 部屋管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnFloorManagement_Click(object? sender, EventArgs e)
        {
            var form = new FloorManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 取引先管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnClientManagement_Click(object? sender, EventArgs e)
        {
            var form = new ClientManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 請求明細一覧ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnInvoiceDetails_Click(object? sender, EventArgs e)
        {
            var form = new InvoiceDetailForm();
            form.ShowDialog();
        }

        /// <summary>
        /// その他請求明細一覧ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnOtherInvoiceDetails_Click(object? sender, EventArgs e)
        {
            var form = new OtherInvoiceDetailForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 水道料金管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnWaterBillingManagement_Click(object? sender, EventArgs e)
        {
            var form = new WaterBillingManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 電気料金管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnElectricBillingManagement_Click(object? sender, EventArgs e)
        {
            var form = new ElectricBillingManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// ガス料金管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnGasBillingManagement_Click(object? sender, EventArgs e)
        {
            var form = new GasBillingManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 電気子メータ検針データ管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnElectricChildMeterReadingManagement_Click(object? sender, EventArgs e)
        {
            var form = new ElectricChildMeterReadingManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// ガス子メータ検針データ管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnGasChildMeterReadingManagement_Click(object? sender, EventArgs e)
        {
            var form = new GasChildMeterReadingManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 水道子メータ検針データ管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnWaterChildMeterReadingManagement_Click(object? sender, EventArgs e)
        {
            var form = new WaterChildMeterReadingManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 契約管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnContractManagement_Click(object? sender, EventArgs e)
        {
            var form = new ContractManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 親メーター管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnMeterManagement_Click(object? sender, EventArgs e)
        {
            var form = new MeterManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 子メーター管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnChildMeterManagement_Click(object? sender, EventArgs e)
        {
            try
            {
                var form = new ChildMeterManagementForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"子メーター管理フォームの表示に失敗しました: {ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 部屋別子メーター管理ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnRoomChildMeterManagement_Click(object? sender, EventArgs e)
        {
            var form = new RoomChildMeterManagementForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 請求データ登録ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnBillingDataRegistration_Click(object? sender, EventArgs e)
        {
            var form = new BillingDataRegistrationForm();
            form.ShowDialog();
        }

        /// <summary>
        /// 請求書印刷ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnInvoicePrint_Click(object? sender, EventArgs e)
        {
            var form = new InvoicePrintForm();
            form.ShowDialog();
        }

        /// <summary>
        /// データ削除ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private async void BtnDeleteAllData_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "すべてのデータを削除し、IDをリセットします。\n\nこの操作は取り消せません。\n本当に実行しますか？",
                "データ削除の確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // 再度確認
                var confirmResult = MessageBox.Show(
                    "最終確認：すべてのデータが削除されます。\n\n本当に実行しますか？",
                    "最終確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Stop);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        await DatabaseHelper.DeleteAllDataAndResetIdsAsync();
                        MessageBox.Show(
                            "すべてのデータを削除し、IDをリセットしました。",
                            "完了",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"データ削除中にエラーが発生しました。\n\nエラー: {ex.Message}",
                            "エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 終了ボタンのクリックイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        private void BtnExit_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("アプリケーションを終了しますか？", "確認", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}



