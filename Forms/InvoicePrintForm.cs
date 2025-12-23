using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 請求書印刷フォーム
    /// </summary>
    public partial class InvoicePrintForm : Form
    {
        public InvoicePrintForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
        }

        private void InitializeComponentAdditional()
        {
            // 請求年月を現在の年月に設定
            var now = DateTime.Now;
            dtpBillingYearMonth.Value = new DateTime(now.Year, now.Month, 1);

            // イベントハンドラー
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            btnPrintInvoiceList.Click += BtnPrintInvoiceList_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private string? GetExcelFilePath()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var currentDirectory = Environment.CurrentDirectory;
            
            var pathCandidates = new List<string>
            {
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "Excel", "請求書.xlsx")),
                Path.Combine(currentDirectory, "Excel", "請求書.xlsx"),
                Path.Combine(baseDirectory, "Excel", "請求書.xlsx"),
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", "Excel", "請求書.xlsx")),
                @"C:\Users\渡辺詠介\Desktop\work\田邊\水道光熱費アプリ\Source\WaterUtilityCost\Excel\請求書.xlsx"
            };
            
            foreach (var candidate in pathCandidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
            
            return null;
        }

        private string? GetInvoiceListExcelFilePath()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var currentDirectory = Environment.CurrentDirectory;
            
            var pathCandidates = new List<string>
            {
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "Excel", "請求一覧.xlsx")),
                Path.Combine(currentDirectory, "Excel", "請求一覧.xlsx"),
                Path.Combine(baseDirectory, "Excel", "請求一覧.xlsx"),
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", "Excel", "請求一覧.xlsx")),
                @"C:\Users\渡辺詠介\Desktop\work\田邊\水道光熱費アプリ\Source\WaterUtilityCost\Excel\請求一覧.xlsx"
            };
            
            foreach (var candidate in pathCandidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
            
            return null;
        }

        private void BtnPrintInvoice_Click(object? sender, EventArgs e)
        {
            try
            {
                var filePath = GetExcelFilePath();
                
                if (filePath == null)
                {
                    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var currentDirectory = Environment.CurrentDirectory;
                    var message = new System.Text.StringBuilder();
                    message.AppendLine("請求書.xlsxファイルが見つかりません。");
                    message.AppendLine();
                    message.AppendLine("確認したパス:");
                    message.AppendLine($"1. {Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "Excel", "請求書.xlsx"))}");
                    message.AppendLine($"2. {Path.Combine(currentDirectory, "Excel", "請求書.xlsx")}");
                    message.AppendLine($"3. {Path.Combine(baseDirectory, "Excel", "請求書.xlsx")}");
                    message.AppendLine();
                    message.AppendLine($"現在の作業ディレクトリ: {currentDirectory}");
                    message.AppendLine($"実行ファイルのディレクトリ: {baseDirectory}");
                    message.AppendLine();
                    message.AppendLine("Excelフォルダに請求書.xlsxファイルを配置してください。");
                    
                    MessageBox.Show(message.ToString(), "ファイルが見つかりません", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Excelファイルを直接印刷（COMオブジェクトを使用）
                PrintExcelFileDirectly(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"印刷に失敗しました:\n\n{ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintExcelFileDirectly(string filePath)
        {
            // Excel COMオブジェクトの型を動的に取得
            Type? excelType = Type.GetTypeFromProgID("Excel.Application");
            if (excelType == null)
            {
                MessageBox.Show("Excelがインストールされていないか、利用できません。\n\nExcelをインストールしてから再度お試しください。", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            object? excelApp = null;
            object? workbooks = null;
            object? workbook = null;

            try
            {
                // Excelアプリケーションを起動
                excelApp = Activator.CreateInstance(excelType);
                if (excelApp == null)
                {
                    MessageBox.Show("Excelアプリケーションを起動できませんでした。", 
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Excelを非表示で実行（印刷ダイアログを表示するため、DisplayAlertsはtrueのまま）
                excelType.InvokeMember("Visible", System.Reflection.BindingFlags.SetProperty, null, excelApp, new object[] { false });
                excelType.InvokeMember("DisplayAlerts", System.Reflection.BindingFlags.SetProperty, null, excelApp, new object[] { true });
                excelType.InvokeMember("ScreenUpdating", System.Reflection.BindingFlags.SetProperty, null, excelApp, new object[] { true });

                // ワークブックを開く
                workbooks = excelType.InvokeMember("Workbooks", System.Reflection.BindingFlags.GetProperty, null, excelApp, null);
                if (workbooks == null)
                {
                    MessageBox.Show("Excelワークブックを取得できませんでした。", 
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ファイルを開く
                workbook = workbooks.GetType().InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, workbooks, 
                    new object[] { filePath, false, true });

                if (workbook == null)
                {
                    MessageBox.Show("Excelファイルを開けませんでした。", 
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 印刷を実行（プリンタ選択ダイアログを表示）
                try
                {
                    // プリンタ選択ダイアログを表示
                    using (PrintDialog printDialog = new PrintDialog())
                    {
                        printDialog.AllowPrintToFile = false;
                        printDialog.AllowSelection = false;
                        printDialog.AllowSomePages = false;
                        printDialog.UseEXDialog = false;
                        
                        if (printDialog.ShowDialog() == DialogResult.OK)
                        {
                            var selectedPrinter = printDialog.PrinterSettings.PrinterName;
                            
                            // 選択されたプリンタをExcelのActivePrinterに設定
                            if (!string.IsNullOrEmpty(selectedPrinter))
                            {
                                excelType.InvokeMember("ActivePrinter", System.Reflection.BindingFlags.SetProperty, null, excelApp, new object[] { selectedPrinter });
                            }
                            
                            // ワークシートを取得
                            object? worksheets = workbook.GetType().InvokeMember("Worksheets", System.Reflection.BindingFlags.GetProperty, null, workbook, null);
                            if (worksheets != null)
                            {
                                object? worksheet = worksheets.GetType().InvokeMember("Item", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheets, new object[] { 1 });
                                if (worksheet != null)
                                {
                                    // AN1セルに請求年月を設定
                                    var billingYearMonth = dtpBillingYearMonth.Value.ToString("yyyy/MM");
                                    object? range = worksheet.GetType().InvokeMember("Range", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheet, new object[] { "AN1" });
                                    if (range != null)
                                    {
                                        range.GetType().InvokeMember("Value", System.Reflection.BindingFlags.SetProperty, null, range, new object[] { billingYearMonth });
                                    }
                                    
                                    // PrintOutメソッドを呼び出して印刷を実行
                                    worksheet.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, worksheet, 
                                        new object[] { 
                                            Type.Missing,  // From
                                            Type.Missing,  // To
                                            Type.Missing,  // Copies
                                            false,         // Preview
                                            Type.Missing,  // ActivePrinter
                                            false,         // PrintToFile
                                            Type.Missing,  // Collate
                                            Type.Missing,  // PrToFileName
                                            false          // Background
                                        });
                                }
                                else
                                {
                                    // ワークシートが取得できない場合、ワークブックのPrintOutメソッドを使用
                                    workbook.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                        new object[] { 
                                            Type.Missing, Type.Missing, Type.Missing, false, Type.Missing, false, Type.Missing, Type.Missing, false
                                        });
                                }
                            }
                            else
                            {
                                // ワークシートが取得できない場合、ワークブックのPrintOutメソッドを使用
                                workbook.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                    new object[] { 
                                        Type.Missing, Type.Missing, Type.Missing, false, Type.Missing, false, Type.Missing, Type.Missing, false
                                    });
                            }
                            
                            // 印刷が完了したら、Excelファイルを閉じる
                            if (workbook != null)
                            {
                                workbook.GetType().InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                    new object[] { false });
                            }
                            
                            // 印刷が完了したことを示すメッセージを表示
                            string fileName = Path.GetFileName(filePath);
                            MessageBox.Show($"印刷が完了しました。\n\nファイル: {fileName}\n請求年月: {dtpBillingYearMonth.Value.ToString("yyyy年MM月")}", 
                                "印刷完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    // ユーザーが印刷ダイアログでキャンセルした場合など
                    if (ex.InnerException != null && 
                        (ex.InnerException.Message.Contains("キャンセル") || 
                         ex.InnerException.Message.Contains("Cancel") ||
                         ex.InnerException.Message.Contains("0x80004004")))
                    {
                        return;
                    }
                    throw;
                }
                catch (COMException ex)
                {
                    // COM例外の場合、キャンセルの可能性をチェック
                    if (ex.ErrorCode == unchecked((int)0x80004004))
                    {
                        return;
                    }
                    throw;
                }
            }
            catch (COMException ex)
            {
                MessageBox.Show($"Excelの操作中にエラーが発生しました:\n\n{ex.Message}\n\nExcelがインストールされているか確認してください。", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excelファイルの印刷中にエラーが発生しました:\n\n{ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // リソースを解放
                try
                {
                    if (workbook != null)
                    {
                        try
                        {
                            workbook.GetType().InvokeMember("Name", System.Reflection.BindingFlags.GetProperty, null, workbook, null);
                            workbook.GetType().InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                new object[] { false });
                        }
                        catch { }
                        Marshal.ReleaseComObject(workbook);
                    }
                }
                catch { }

                try
                {
                    if (workbooks != null)
                    {
                        Marshal.ReleaseComObject(workbooks);
                    }
                }
                catch { }

                try
                {
                    if (excelApp != null)
                    {
                        excelType?.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, excelApp, null);
                        Marshal.ReleaseComObject(excelApp);
                    }
                }
                catch { }

                // COMオブジェクトのファイナライズを強制
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void BtnPrintInvoiceList_Click(object? sender, EventArgs e)
        {
            try
            {
                var filePath = GetInvoiceListExcelFilePath();
                
                if (filePath == null)
                {
                    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var currentDirectory = Environment.CurrentDirectory;
                    var message = new System.Text.StringBuilder();
                    message.AppendLine("請求一覧.xlsxファイルが見つかりません。");
                    message.AppendLine();
                    message.AppendLine("確認したパス:");
                    message.AppendLine($"1. {Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "Excel", "請求一覧.xlsx"))}");
                    message.AppendLine($"2. {Path.Combine(currentDirectory, "Excel", "請求一覧.xlsx")}");
                    message.AppendLine($"3. {Path.Combine(baseDirectory, "Excel", "請求一覧.xlsx")}");
                    message.AppendLine();
                    message.AppendLine($"現在の作業ディレクトリ: {currentDirectory}");
                    message.AppendLine($"実行ファイルのディレクトリ: {baseDirectory}");
                    message.AppendLine();
                    message.AppendLine("Excelフォルダに請求一覧.xlsxファイルを配置してください。");
                    
                    MessageBox.Show(message.ToString(), "ファイルが見つかりません", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Excelファイルを直接印刷（COMオブジェクトを使用、請求年月を設定）
                PrintExcelFileDirectlyForInvoiceList(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"印刷に失敗しました:\n\n{ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintExcelFileDirectlyForInvoiceList(string filePath)
        {
            // Excel COMオブジェクトの型を動的に取得
            Type? excelType = Type.GetTypeFromProgID("Excel.Application");
            if (excelType == null)
            {
                MessageBox.Show("Excelがインストールされていないか、利用できません。\n\nExcelをインストールしてから再度お試しください。", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            object? excelApp = null;
            object? workbooks = null;
            object? workbook = null;

            try
            {
                // Excelアプリケーションを起動
                excelApp = Activator.CreateInstance(excelType);
                if (excelApp == null)
                {
                    MessageBox.Show("Excelアプリケーションを起動できませんでした。", 
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Excelを非表示で実行（印刷ダイアログを表示するため、DisplayAlertsはtrueのまま）
                excelType.InvokeMember("Visible", System.Reflection.BindingFlags.SetProperty, null, excelApp, new object[] { false });
                excelType.InvokeMember("DisplayAlerts", System.Reflection.BindingFlags.SetProperty, null, excelApp, new object[] { true });
                excelType.InvokeMember("ScreenUpdating", System.Reflection.BindingFlags.SetProperty, null, excelApp, new object[] { true });

                // ワークブックを開く
                workbooks = excelType.InvokeMember("Workbooks", System.Reflection.BindingFlags.GetProperty, null, excelApp, null);
                if (workbooks == null)
                {
                    MessageBox.Show("Excelワークブックを取得できませんでした。", 
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ファイルを開く
                workbook = workbooks.GetType().InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, workbooks, 
                    new object[] { filePath, false, true });

                if (workbook == null)
                {
                    MessageBox.Show("Excelファイルを開けませんでした。", 
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 印刷を実行（プリンタ選択ダイアログを表示）
                try
                {
                    // プリンタ選択ダイアログを表示
                    using (PrintDialog printDialog = new PrintDialog())
                    {
                        printDialog.AllowPrintToFile = false;
                        printDialog.AllowSelection = false;
                        printDialog.AllowSomePages = false;
                        printDialog.UseEXDialog = false;
                        
                        if (printDialog.ShowDialog() == DialogResult.OK)
                        {
                            var selectedPrinter = printDialog.PrinterSettings.PrinterName;
                            
                            // 選択されたプリンタをExcelのActivePrinterに設定
                            if (!string.IsNullOrEmpty(selectedPrinter))
                            {
                                excelType.InvokeMember("ActivePrinter", System.Reflection.BindingFlags.SetProperty, null, excelApp, new object[] { selectedPrinter });
                            }
                            
                            // ワークシートを取得
                            object? worksheets = workbook.GetType().InvokeMember("Worksheets", System.Reflection.BindingFlags.GetProperty, null, workbook, null);
                            if (worksheets != null)
                            {
                                object? worksheet = worksheets.GetType().InvokeMember("Item", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheets, new object[] { 1 });
                                if (worksheet != null)
                                {
                                    // AN1セルに請求年月を設定（請求一覧印刷の場合）
                                    var billingYearMonth = dtpBillingYearMonth.Value.ToString("yyyy/MM");
                                    object? range = worksheet.GetType().InvokeMember("Range", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheet, new object[] { "AN1" });
                                    if (range != null)
                                    {
                                        range.GetType().InvokeMember("Value", System.Reflection.BindingFlags.SetProperty, null, range, new object[] { billingYearMonth });
                                    }
                                    
                                    // PrintOutメソッドを呼び出して印刷を実行
                                    worksheet.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, worksheet, 
                                        new object[] { 
                                            Type.Missing,  // From
                                            Type.Missing,  // To
                                            Type.Missing,  // Copies
                                            false,         // Preview
                                            Type.Missing,  // ActivePrinter
                                            false,         // PrintToFile
                                            Type.Missing,  // Collate
                                            Type.Missing,  // PrToFileName
                                            false          // Background
                                        });
                                }
                                else
                                {
                                    // ワークシートが取得できない場合、ワークブックのPrintOutメソッドを使用
                                    workbook.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                        new object[] { 
                                            Type.Missing, Type.Missing, Type.Missing, false, Type.Missing, false, Type.Missing, Type.Missing, false
                                        });
                                }
                            }
                            else
                            {
                                // ワークシートが取得できない場合、ワークブックのPrintOutメソッドを使用
                                workbook.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                    new object[] { 
                                        Type.Missing, Type.Missing, Type.Missing, false, Type.Missing, false, Type.Missing, Type.Missing, false
                                    });
                            }
                            
                            // 印刷が完了したら、Excelファイルを閉じる
                            if (workbook != null)
                            {
                                workbook.GetType().InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                    new object[] { false });
                            }
                            
                            // 印刷が完了したことを示すメッセージを表示
                            string fileName = Path.GetFileName(filePath);
                            MessageBox.Show($"印刷が完了しました。\n\nファイル: {fileName}\n請求年月: {dtpBillingYearMonth.Value.ToString("yyyy年MM月")}", 
                                "印刷完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    // ユーザーが印刷ダイアログでキャンセルした場合など
                    if (ex.InnerException != null && 
                        (ex.InnerException.Message.Contains("キャンセル") || 
                         ex.InnerException.Message.Contains("Cancel") ||
                         ex.InnerException.Message.Contains("0x80004004")))
                    {
                        return;
                    }
                    throw;
                }
                catch (COMException ex)
                {
                    // COM例外の場合、キャンセルの可能性をチェック
                    if (ex.ErrorCode == unchecked((int)0x80004004))
                    {
                        return;
                    }
                    throw;
                }
            }
            catch (COMException ex)
            {
                MessageBox.Show($"Excelの操作中にエラーが発生しました:\n\n{ex.Message}\n\nExcelがインストールされているか確認してください。", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excelファイルの印刷中にエラーが発生しました:\n\n{ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // リソースを解放
                try
                {
                    if (workbook != null)
                    {
                        try
                        {
                            workbook.GetType().InvokeMember("Name", System.Reflection.BindingFlags.GetProperty, null, workbook, null);
                            workbook.GetType().InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                new object[] { false });
                        }
                        catch { }
                        Marshal.ReleaseComObject(workbook);
                    }
                }
                catch { }

                try
                {
                    if (workbooks != null)
                    {
                        Marshal.ReleaseComObject(workbooks);
                    }
                }
                catch { }

                try
                {
                    if (excelApp != null)
                    {
                        excelType?.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, excelApp, null);
                        Marshal.ReleaseComObject(excelApp);
                    }
                }
                catch { }

                // COMオブジェクトのファイナライズを強制
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

