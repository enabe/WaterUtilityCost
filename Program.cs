using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.Database;
using WaterUtilityCost.Forms;

namespace WaterUtilityCost
{
    /// <summary>
    /// アプリケーションのエントリーポイント
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメインエントリーポイント
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // データベース接続テスト（同期実行）
                var connectionTask = DatabaseHelper.TestConnectionAsync();
                var (isConnected, errorMessage) = connectionTask.GetAwaiter().GetResult();

                if (!isConnected)
                {
                    var message = $"データベースに接続できません。\n\n接続文字列: {DatabaseHelper.ConnectionString}\n\nエラー詳細:\n{errorMessage}\n\n確認事項:\n" +
                        "1. SQL Server Expressが起動しているか確認してください\n" +
                        "2. インスタンス名 '.\\SQLEXPRESS01' が正しいか確認してください\n" +
                        "3. SQL Server Browserサービスが起動しているか確認してください\n" +
                        "4. Windows認証でアクセス権限があるか確認してください";
                    MessageBox.Show(message, "データベース接続エラー", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // データベース初期化（同期実行）
                try
                {
                    DatabaseHelper.InitializeDatabaseAsync().GetAwaiter().GetResult();
                }
                catch (Exception dbEx)
                {
                    var dbErrorMessage = $"データベース初期化エラー: {dbEx.Message}";
                    if (dbEx.InnerException != null)
                    {
                        dbErrorMessage += $"\n\n内部エラー: {dbEx.InnerException.Message}";
                    }
                    dbErrorMessage += $"\n\nスタックトレース:\n{dbEx.StackTrace}";
                    MessageBox.Show(dbErrorMessage, "データベース初期化エラー", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Excelファイルの確認（一時的）
                // ExcelChecker.CheckInvoiceExcel();

                // メニューフォームを起動
                Application.Run(new MenuForm());
            }
            catch (Exception ex)
            {
                var errorMessage = $"アプリケーション起動エラー: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\n内部エラー: {ex.InnerException.Message}";
                }
                errorMessage += $"\n\nスタックトレース:\n{ex.StackTrace}";
                MessageBox.Show(errorMessage, "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

