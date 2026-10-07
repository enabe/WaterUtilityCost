using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.DataAccess;
using WaterUtilityCost.Models;
using System.Linq;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 請求書印刷フォーム
    /// </summary>
    public partial class InvoicePrintForm : Form
    {
        private const string BillingToAllOption = "（全て）";
        private const string BankTransferNotice = "お振込み口座：三井住友銀行　関内支店　普通999999　ｶ)ｾﾚｽｴｽﾃｰﾄ　※振込み手数料ご負担の上お支払い下さい。";
        private const string BillingToCellAddress = "B5";
        /// <summary>15行目：業者名とインボイス番号（C列、形式 業者名：番号）</summary>
        private const string InvoiceNumberCellAddress = "C15";
        private const string InvoiceDetailRoomCellAddress = "B20";
        private const string InvoiceDetailContentCellAddress = "H20";
        private const string InvoiceDetailUsageCellAddress = "AE20";
        private const string InvoiceDetailUnitCellAddress = "AJ20";
        private const string InvoiceDetailAmountCellAddress = "AM20";
        private const string InvoiceDetailTaxRateCellAddress = "AS20";
        private const int InvoiceDetailStartRow = 20;
        private const int InvoiceDetailStartRowForContinuation = 10;
        private const int InvoiceDetailPageBreakRow = 50;
        private const int InvoiceDetailRowsPerSheet = InvoiceDetailPageBreakRow - InvoiceDetailStartRow;

        // 請求一覧レイアウト（シート1の1～29行を1ブロックとし、ブロック先頭から見て7～26行目に明細20件／ブロック）
        private const int InvoiceListBlockRowCount = 29;
        /// <summary>各ブロック内でページ表記を出す行（例: BU27）</summary>
        private const int InvoiceListPageIndicatorRowInBlock = 27;
        private const string InvoiceListPageIndicatorColumn = "BU";
        private const int InvoiceListDataStartRow = 7;
        private const int InvoiceListDataEndRow = 26;
        private const int InvoiceListRowsPerPage = InvoiceListDataEndRow - InvoiceListDataStartRow + 1; // 20
        private const int InvoiceListMaxDataRows = 500;
        private sealed class ContractorInfo
        {
            public string Name { get; set; } = string.Empty;
            public string InvoiceNumber { get; set; } = string.Empty;
        }

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

            EnsureBillingToSelectionControls();
            AdjustLayoutForSelections();

            // イベントハンドラー
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            btnPrintInvoiceList.Click += BtnPrintInvoiceList_Click;
            btnCancel.Click += BtnCancel_Click;
            this.Load += InvoicePrintForm_Load;
        }

        private void EnsureBillingToSelectionControls()
        {
            var billingLabel = Controls["lblBillingTo"] as Label;
            if (billingLabel == null)
            {
                billingLabel = new Label
                {
                    Name = "lblBillingTo",
                    Text = "請求先:",
                    AutoSize = true,
                    Font = lblBillingYearMonth.Font,
                    Location = new Point(50, 110)
                };
                Controls.Add(billingLabel);
            }

            var billingCombo = Controls["cmbBillingTo"] as ComboBox;
            if (billingCombo == null)
            {
                billingCombo = new ComboBox
                {
                    Name = "cmbBillingTo",
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = dtpBillingYearMonth.Font,
                    Location = new Point(150, 107),
                    Size = new Size(250, 32)
                };
                Controls.Add(billingCombo);
            }

            billingLabel.Visible = true;
            billingCombo.Visible = true;
            billingLabel.BringToFront();
            billingCombo.BringToFront();
        }

        private void AdjustLayoutForSelections()
        {
            btnPrintInvoice.Location = new Point(50, 170);
            btnPrintInvoiceList.Location = new Point(270, 170);
            btnCancel.Location = new Point(420, 260);
            ClientSize = new Size(550, 340);
        }

        private async void InvoicePrintForm_Load(object? sender, EventArgs e)
        {
            await LoadBillingToNamesAsync();
        }

        private async Task LoadBillingToNamesAsync()
        {
            try
            {
                var combo = GetBillingToCombo();
                if (combo == null)
                {
                    return;
                }

                var billingToNames = new List<string> { BillingToAllOption };
                var clients = await ClientDataAccess.GetAllClientsAsync();
                billingToNames.AddRange(clients
                    .Where(c => c.IsBillingTo)
                    .Select(c => c.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct());

                combo.DataSource = billingToNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"請求先の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSelectedBillingTo()
        {
            var combo = GetBillingToCombo();
            var selected = combo?.SelectedItem?.ToString() ?? string.Empty;
            return selected == BillingToAllOption ? string.Empty : selected;
        }

        private ComboBox? GetBillingToCombo()
        {
            return Controls["cmbBillingTo"] as ComboBox;
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

        private async void BtnPrintInvoice_Click(object? sender, EventArgs e)
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
                
                var billingYearMonth = dtpBillingYearMonth.Value;
                var invoiceDetails = await InvoiceDetailDataAccess.GetInvoiceDetailsByBillingYearMonthAsync(billingYearMonth.Year, billingYearMonth.Month);
                var contractorInfoByBuildingAndCategory =
                    await BuildContractorInfoByBuildingAndCategoryAsync($"{billingYearMonth:yyyy-MM}");

                var selectedBillingTo = GetSelectedBillingTo();
                if (!string.IsNullOrEmpty(selectedBillingTo))
                {
                    invoiceDetails = invoiceDetails
                        .Where(d => string.Equals(d.BillingTo, selectedBillingTo, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (invoiceDetails.Count == 0)
                {
                    MessageBox.Show("対象の請求明細データがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!invoiceDetails.Any(d => !string.IsNullOrWhiteSpace(d.BillingTo)))
                {
                    MessageBox.Show("請求先が設定された請求明細データがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var billingToTransferMethodMap = await BuildBillingToTransferMethodMapAsync(invoiceDetails);

                // Excelファイルを直接印刷（COMオブジェクトを使用）
                PrintExcelFileDirectly(filePath, invoiceDetails, contractorInfoByBuildingAndCategory, billingToTransferMethodMap);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"印刷に失敗しました:\n\n{ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintExcelFileDirectly(
            string filePath,
            List<InvoiceDetail> invoiceDetails,
            Dictionary<string, Dictionary<string, ContractorInfo>> contractorInfoByBuildingAndCategory,
            Dictionary<string, BillingToTransferInfo> billingToTransferMethodMap)
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
                            var activePrinter = string.IsNullOrEmpty(selectedPrinter) ? Type.Missing : selectedPrinter;
                            
                            // ワークシートを取得
                            object? worksheets = workbook.GetType().InvokeMember("Worksheets", System.Reflection.BindingFlags.GetProperty, null, workbook, null);
                            if (worksheets != null)
                            {
                                object? worksheet = worksheets.GetType().InvokeMember("Item", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheets, new object[] { 1 });
                                if (worksheet != null)
                                {
                                    var printDate = DateTime.Now.ToString("yyyy/MM/dd");
                                    var billingGroups = invoiceDetails
                                        .Where(d => !string.IsNullOrWhiteSpace(d.BillingTo))
                                        .GroupBy(d => d.BillingTo)
                                        .OrderBy(g => g.Key)
                                        .ToList();

                                    foreach (var group in billingGroups)
                                    {
                                        var buildingName = group
                                            .Select(d => d.BuildingName)
                                            .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name)) ?? string.Empty;
                                        var billingMonth = dtpBillingYearMonth.Value;
                                        var totalAmount = group.Sum(d => d.TaxInclusiveAmount);
                                        var groupList = group.ToList();
                                        var contractorInfo = ResolveContractorInfoForInvoiceHeader(
                                            buildingName,
                                            groupList,
                                            contractorInfoByBuildingAndCategory);
                                        var contractorName = contractorInfo?.Name ?? groupList
                                            .Select(d => d.Contractor?.Trim())
                                            .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name)) ?? string.Empty;
                                        var invoiceNumber = contractorInfo?.InvoiceNumber ?? string.Join(" / ", groupList
                                            .Select(d => d.InvoiceNumber?.Trim())
                                            .Where(n => !string.IsNullOrWhiteSpace(n))
                                            .Distinct());
                                        var mapKey = GetBillingToMapKey(buildingName, group.Key);
                                        var transferInfo = billingToTransferMethodMap.TryGetValue(mapKey, out var info)
                                            ? info
                                            : BillingToTransferInfo.Unknown;
                                        var paymentDueDate = ResolveInvoicePaymentDueDate(transferInfo, groupList, billingMonth);
                                        var (bodyRows, footerRows) = BuildInvoiceDetailRowSections(groupList);
                                        var pages = PaginateInvoiceDetailRows(bodyRows, footerRows, InvoiceDetailRowsPerSheet);
                                        var totalPages = Math.Max(1, pages.Count);
                                        object? continuationWorksheet = null;
                                        if (totalPages > 1)
                                        {
                                            continuationWorksheet = worksheets.GetType().InvokeMember("Item", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheets, new object[] { "請求書2ページ以降" });
                                            if (continuationWorksheet == null)
                                            {
                                                MessageBox.Show(
                                                    "請求書2ページ以降のシート（シート名: 請求書2ページ以降）が見つかりません。",
                                                    "エラー",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                                                return;
                                            }
                                        }

                                        for (var pageIndex = 0; pageIndex < totalPages; pageIndex++)
                                        {
                                            var targetWorksheet = pageIndex == 0 ? worksheet : continuationWorksheet;
                                            if (targetWorksheet == null)
                                            {
                                                MessageBox.Show("請求書の印刷対象シートを取得できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                return;
                                            }

                                            ApplyInvoiceHeader(
                                                targetWorksheet,
                                                group.Key,
                                                buildingName,
                                                totalAmount,
                                                contractorName,
                                                invoiceNumber,
                                                paymentDueDate,
                                                printDate,
                                                pageIndex + 1,
                                                totalPages,
                                                transferInfo.Method == TransferMethod.Bank ? BankTransferNotice : null);
                                            var detailStartRow = pageIndex == 0 ? InvoiceDetailStartRow : InvoiceDetailStartRowForContinuation;
                                            WriteInvoiceDetailRows(
                                                targetWorksheet,
                                                pages[pageIndex],
                                                detailStartRow);

                                            // PrintOutメソッドを呼び出して印刷を実行
                                            targetWorksheet.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, targetWorksheet,
                                                new object[] {
                                                    Type.Missing,  // From
                                                    Type.Missing,  // To
                                                    Type.Missing,  // Copies
                                                    false,         // Preview
                                                    activePrinter, // ActivePrinter
                                                    false,         // PrintToFile
                                                    Type.Missing,  // Collate
                                                    Type.Missing,  // PrToFileName
                                                    false          // Background
                                                });
                                        }
                                    }
                                }
                                else
                                {
                                    // ワークシートが取得できない場合、ワークブックのPrintOutメソッドを使用
                                    workbook.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                        new object[] { 
                                            Type.Missing, Type.Missing, Type.Missing, false, activePrinter, false, Type.Missing, Type.Missing, false
                                        });
                                }
                            }
                            else
                            {
                                // ワークシートが取得できない場合、ワークブックのPrintOutメソッドを使用
                                workbook.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                    new object[] { 
                                        Type.Missing, Type.Missing, Type.Missing, false, activePrinter, false, Type.Missing, Type.Missing, false
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
                            MessageBox.Show($"印刷が完了しました。\n\nファイル: {fileName}\n請求年月: {dtpBillingYearMonth.Value:yyyy年MM月}", 
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
                    var detail = ex.InnerException != null
                        ? $"{ex.InnerException.Message}\n\n{ex.InnerException.StackTrace}"
                        : $"{ex.Message}\n\n{ex.StackTrace}";
                    MessageBox.Show($"Excelファイルの印刷中にエラーが発生しました:\n\n{detail}",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (COMException ex)
                {
                    // COM例外の場合、キャンセルの可能性をチェック
                    if (ex.ErrorCode == unchecked((int)0x80004004))
                    {
                        return;
                    }
                    MessageBox.Show($"Excelファイルの印刷中にエラーが発生しました:\n\n{ex.Message}\n\n{ex.StackTrace}",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
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

        private (List<InvoiceDetailRow> Body, List<InvoiceDetailRow> Footer) BuildInvoiceDetailRowSections(List<InvoiceDetail> invoiceDetails)
        {
            var orderedDetails = invoiceDetails
                .Where(d => !string.IsNullOrWhiteSpace(d.RoomNumber) || !string.IsNullOrWhiteSpace(d.Content))
                .OrderBy(d => d.RoomNumber)
                .ThenBy(d => GetUtilityOrder(d.Content))
                .ThenBy(d => d.Content)
                .ToList();

            var bodyRows = new List<InvoiceDetailRow>();
            foreach (var roomGroup in orderedDetails.GroupBy(d => d.RoomNumber ?? string.Empty))
            {
                var roomNumber = roomGroup.Key?.Trim() ?? string.Empty;
                var isFirstLine = true;
                var wroteAnyLine = false;
                var roomSubtotal = 0m;

                foreach (var detail in roomGroup)
                {
                    var content = detail.Content?.Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(roomNumber))
                    {
                        continue;
                    }

                    bodyRows.Add(new InvoiceDetailRow
                    {
                        RoomText = isFirstLine ? $"■{roomNumber}" : string.Empty,
                        Content = content,
                        UsageAmount = detail.UsageAmount,
                        Unit = detail.Unit?.Trim() ?? string.Empty,
                        TaxInclusiveAmount = detail.TaxInclusiveAmount,
                        TaxRate = FormatTaxRate(detail.TaxRate)
                    });

                    isFirstLine = false;
                    wroteAnyLine = true;
                    roomSubtotal += detail.TaxInclusiveAmount;
                }

                if (wroteAnyLine)
                {
                    bodyRows.Add(new InvoiceDetailRow
                    {
                        Content = "＜小　　計＞",
                        TaxInclusiveAmount = roomSubtotal
                    });
                    bodyRows.Add(new InvoiceDetailRow());
                }
            }

            var footerRows = new List<InvoiceDetailRow>();
            AppendUtilitySubtotalRows(footerRows, invoiceDetails);
            return (bodyRows, footerRows);
        }

        /// <summary>
        /// 光熱費小計・課税別合計を最終ページにまとめ、途中で切れないようページ分割する。
        /// </summary>
        private static List<List<InvoiceDetailRow>> PaginateInvoiceDetailRows(
            List<InvoiceDetailRow> bodyRows,
            List<InvoiceDetailRow> footerRows,
            int rowsPerPage)
        {
            var pages = new List<List<InvoiceDetailRow>>();
            var footerCount = footerRows.Count;
            var index = 0;

            if (bodyRows.Count == 0 && footerCount == 0)
            {
                pages.Add(new List<InvoiceDetailRow>());
                return pages;
            }

            while (index < bodyRows.Count)
            {
                var remainingBody = bodyRows.Count - index;
                if (remainingBody + footerCount <= rowsPerPage)
                {
                    var lastPage = bodyRows.Skip(index).ToList();
                    lastPage.AddRange(footerRows);
                    pages.Add(lastPage);
                    return pages;
                }

                var take = rowsPerPage;
                pages.Add(bodyRows.Skip(index).Take(take).ToList());
                index += take;
            }

            if (footerCount > 0)
            {
                pages.Add(new List<InvoiceDetailRow>(footerRows));
            }

            if (pages.Count == 0)
            {
                pages.Add(new List<InvoiceDetailRow>());
            }

            return pages;
        }

        /// <summary>
        /// 請求先単位の明細の最後に【光熱費の小計】（電気・ガス・水道の集計と使用期間）を追加する。
        /// </summary>
        private static void AppendUtilitySubtotalRows(List<InvoiceDetailRow> rows, List<InvoiceDetail> allDetails)
        {
            static List<InvoiceDetail> PickByContentPrefix(IEnumerable<InvoiceDetail> details, string prefix) =>
                details
                    .Where(d => !string.IsNullOrWhiteSpace(d.Content) &&
                                d.Content.Trim().StartsWith(prefix, StringComparison.Ordinal))
                    .ToList();

            var electricDetails = PickByContentPrefix(allDetails, "電気料金");
            var gasDetails = PickByContentPrefix(allDetails, "ガス料金");
            var waterDetails = PickByContentPrefix(allDetails, "水道料金");

            var electricSum = electricDetails.Sum(d => d.TaxInclusiveAmount);
            var gasSum = gasDetails.Sum(d => d.TaxInclusiveAmount);
            var waterSum = waterDetails.Sum(d => d.TaxInclusiveAmount);

            if (electricSum == 0m && gasSum == 0m && waterSum == 0m)
            {
                return;
            }

            rows.Add(new InvoiceDetailRow { RoomText = "【光熱費の小計】" });

            if (electricSum != 0m)
            {
                rows.Add(CreateUtilitySubtotalDetailRow(electricDetails, electricSum, "電気料金"));
            }

            if (gasSum != 0m)
            {
                rows.Add(CreateUtilitySubtotalDetailRow(gasDetails, gasSum, "ガス料金"));
            }

            if (waterSum != 0m)
            {
                rows.Add(CreateUtilitySubtotalDetailRow(waterDetails, waterSum, "水道料金"));
            }

            rows.Add(new InvoiceDetailRow());
            AppendTaxCategorySubtotalRows(rows, electricDetails.Concat(gasDetails).Concat(waterDetails));
        }

        private static InvoiceDetailRow CreateUtilitySubtotalDetailRow(
            List<InvoiceDetail> details,
            decimal sum,
            string labelPrefix)
        {
            var (start, end) = AggregateBillingPeriod(details);
            var content = start.HasValue && end.HasValue
                ? $"{labelPrefix}（{FormatJapaneseDate(start.Value)}～{FormatJapaneseDate(end.Value)}）"
                : labelPrefix;

            var taxRate = details.Count > 0 ? details[0].TaxRate : 0.1m;

            return new InvoiceDetailRow
            {
                Content = content,
                TaxInclusiveAmount = sum,
                TaxRate = FormatTaxRate(taxRate)
            };
        }

        private static string FormatJapaneseDate(DateTime d) => $"{d.Year}年{d.Month}月{d.Day}日";

        private static (DateTime? MinStart, DateTime? MaxEnd) AggregateBillingPeriod(IEnumerable<InvoiceDetail> details)
        {
            DateTime? minStart = null;
            DateTime? maxEnd = null;

            foreach (var d in details)
            {
                var start = d.ParentMeterStartDate;
                var end = d.ParentMeterEndDate;

                if (start.HasValue)
                {
                    minStart = !minStart.HasValue || start.Value < minStart.Value ? start : minStart;
                }

                if (end.HasValue)
                {
                    maxEnd = !maxEnd.HasValue || end.Value > maxEnd.Value ? end : maxEnd;
                }
            }

            return (minStart, maxEnd);
        }

        private static void AppendTaxCategorySubtotalRows(List<InvoiceDetailRow> rows, IEnumerable<InvoiceDetail> utilityDetails)
        {
            var detailList = utilityDetails.ToList();
            if (detailList.Count == 0)
            {
                return;
            }

            rows.Add(new InvoiceDetailRow { RoomText = "【課税別合計】", ClearContentCell = true });

            var taxableGroups = detailList
                .Where(d => NormalizeTaxRate(d.TaxRate) > 0m)
                .GroupBy(d => NormalizeTaxRate(d.TaxRate))
                .OrderByDescending(g => g.Key)
                .ToList();

            foreach (var g in taxableGroups)
            {
                var rate = g.Key;
                var taxInclusive = g.Sum(x => x.TaxInclusiveAmount);
                var taxExclusive = rate == 0m ? taxInclusive : decimal.Round(taxInclusive / (1m + rate), 0, MidpointRounding.AwayFromZero);
                var consumptionTax = taxInclusive - taxExclusive;
                var rateText = (rate * 100m).ToString("0");
                var lineText = $"＜{rateText}%課税＞　税別 {FormatYen(taxExclusive)}　消費税等 {FormatYen(consumptionTax)}　税込 {FormatYen(taxInclusive)}";

                rows.Add(new InvoiceDetailRow
                {
                    RoomText = lineText,
                    ClearContentCell = true
                });
            }

            var nonTaxAmount = detailList
                .Where(d => NormalizeTaxRate(d.TaxRate) == 0m)
                .Sum(d => d.TaxInclusiveAmount);

            if (nonTaxAmount != 0m)
            {
                rows.Add(new InvoiceDetailRow
                {
                    RoomText = $"＜不課税＞　{FormatYen(nonTaxAmount)}",
                    ClearContentCell = true
                });
            }
        }

        private static decimal NormalizeTaxRate(decimal taxRate)
        {
            if (taxRate <= 0m)
            {
                return 0m;
            }

            return taxRate >= 1m ? taxRate / 100m : taxRate;
        }

        private static string FormatYen(decimal amount) => $"¥{amount:N0}";

        private int GetUtilityOrder(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return 3;
            }

            var trimmed = content.Trim();
            if (trimmed.StartsWith("電気料金", StringComparison.Ordinal))
            {
                return 0;
            }
            if (trimmed.StartsWith("ガス料金", StringComparison.Ordinal))
            {
                return 1;
            }
            if (trimmed.StartsWith("水道料金", StringComparison.Ordinal))
            {
                return 2;
            }

            return 3;
        }

        private void WriteInvoiceDetailRows(object worksheet, IReadOnlyList<InvoiceDetailRow> pageRows, int detailStartRow)
        {
            ClearInvoiceDetailCells(worksheet, detailStartRow);

            for (var rowOffset = 0; rowOffset < pageRows.Count; rowOffset++)
            {
                var row = pageRows[rowOffset];
                var roomCellAddress = GetCellAddress(InvoiceDetailRoomCellAddress, rowOffset, detailStartRow);
                SetCellValue(worksheet, roomCellAddress, row.RoomText);
                if (!string.IsNullOrEmpty(row.RoomText) && row.ClearContentCell)
                {
                    ApplySummaryRoomCellDisplay(worksheet, roomCellAddress);
                }
                var contentCellAddress = GetCellAddress(InvoiceDetailContentCellAddress, rowOffset, detailStartRow);
                if (row.ClearContentCell || string.IsNullOrEmpty(row.Content))
                {
                    ClearCellContents(worksheet, contentCellAddress);
                }
                else
                {
                    SetCellValue(worksheet, contentCellAddress, row.Content);
                }

                if (row.UsageAmount.HasValue)
                {
                    SetCellValue(worksheet, GetCellAddress(InvoiceDetailUsageCellAddress, rowOffset, detailStartRow), row.UsageAmount.Value);
                }
                else
                {
                    SetCellValue(worksheet, GetCellAddress(InvoiceDetailUsageCellAddress, rowOffset, detailStartRow), string.Empty);
                }

                SetCellValue(worksheet, GetCellAddress(InvoiceDetailUnitCellAddress, rowOffset, detailStartRow), row.Unit);

                if (row.TaxInclusiveAmount.HasValue)
                {
                    SetCellValue(worksheet, GetCellAddress(InvoiceDetailAmountCellAddress, rowOffset, detailStartRow), row.TaxInclusiveAmount.Value);
                }
                else
                {
                    SetCellValue(worksheet, GetCellAddress(InvoiceDetailAmountCellAddress, rowOffset, detailStartRow), string.Empty);
                }

                SetCellValue(worksheet, GetCellAddress(InvoiceDetailTaxRateCellAddress, rowOffset, detailStartRow), row.TaxRate);
            }
        }

        private async Task<Dictionary<string, Dictionary<string, ContractorInfo>>> BuildContractorInfoByBuildingAndCategoryAsync(
            string billingYearMonthKey)
        {
            var electricBillings = await ElectricBillingDataAccess.GetElectricBillingsByBillingYearMonthAsync(billingYearMonthKey);
            var gasBillings = await GasBillingDataAccess.GetGasBillingsByBillingYearMonthAsync(billingYearMonthKey);
            gasBillings = gasBillings
                .GroupBy(b => b.ParentMeterId)
                .Select(g => g.OrderByDescending(x => x.Id).First())
                .ToList();
            var waterBillings = await WaterBillingDataAccess.GetWaterBillingsByBillingYearMonthAsync(billingYearMonthKey);

            var map = new Dictionary<string, Dictionary<string, ContractorInfo>>(StringComparer.OrdinalIgnoreCase);
            var contractorCache = new Dictionary<int, Client>();

            async Task AddBillingAsync(string? buildingName, string category, int? contractorId)
            {
                if (string.IsNullOrWhiteSpace(buildingName) || !contractorId.HasValue)
                {
                    return;
                }

                var trimmedBuilding = buildingName.Trim();
                var cid = contractorId.Value;
                if (!contractorCache.TryGetValue(cid, out var client))
                {
                    var loadedClient = await ClientDataAccess.GetClientByIdAsync(cid);
                    if (loadedClient == null)
                    {
                        return;
                    }

                    client = loadedClient;
                    contractorCache[cid] = client;
                }

                if (!map.TryGetValue(trimmedBuilding, out var inner))
                {
                    inner = new Dictionary<string, ContractorInfo>(StringComparer.Ordinal);
                    map[trimmedBuilding] = inner;
                }

                inner[category] = new ContractorInfo
                {
                    Name = client.Name?.Trim() ?? string.Empty,
                    InvoiceNumber = client.InvoiceNumber?.Trim() ?? string.Empty
                };
            }

            foreach (var b in electricBillings)
            {
                await AddBillingAsync(b.BuildingName, "電気", b.ContractorId);
            }

            foreach (var b in gasBillings)
            {
                await AddBillingAsync(b.BuildingName, "ガス", b.ContractorId);
            }

            foreach (var b in waterBillings)
            {
                await AddBillingAsync(b.BuildingName, "水道", b.ContractorId);
            }

            return map;
        }

        private static ContractorInfo? ResolveContractorInfoForInvoiceHeader(
            string buildingName,
            IReadOnlyList<InvoiceDetail> groupDetails,
            Dictionary<string, Dictionary<string, ContractorInfo>> contractorInfoByBuildingAndCategory)
        {
            if (string.IsNullOrWhiteSpace(buildingName))
            {
                return null;
            }

            if (!contractorInfoByBuildingAndCategory.TryGetValue(buildingName.Trim(), out var byCategory))
            {
                return null;
            }

            var categoryPriority = new[] { "電気", "ガス", "水道" };
            var present = new HashSet<string>(
                groupDetails.Select(d => d.Category?.Trim() ?? "").Where(c => c.Length > 0),
                StringComparer.Ordinal);
            foreach (var cat in categoryPriority)
            {
                if (present.Contains(cat) && byCategory.TryGetValue(cat, out var info))
                {
                    return info;
                }
            }

            foreach (var d in groupDetails)
            {
                var cat = d.Category?.Trim() ?? "";
                if (cat.Length > 0 && byCategory.TryGetValue(cat, out var info))
                {
                    return info;
                }
            }

            return null;
        }

        private void ApplyInvoiceHeader(
            object worksheet,
            string billingTo,
            string buildingName,
            decimal totalAmount,
            string contractorName,
            string invoiceNumber,
            DateTime paymentDueDate,
            string printDate,
            int currentPage,
            int totalPages,
            string? bankTransferNotice)
        {
            SetCellValue(worksheet, BillingToCellAddress, $"{billingTo}　　様");
            var contractorDisplay = string.IsNullOrWhiteSpace(contractorName) ? string.Empty : contractorName.Trim();
            var inv = string.IsNullOrWhiteSpace(invoiceNumber) ? string.Empty : invoiceNumber.Trim();
            string line15Display;
            if (!string.IsNullOrEmpty(contractorDisplay) && !string.IsNullOrEmpty(inv))
            {
                line15Display = $"{contractorDisplay}：{inv}";
            }
            else if (!string.IsNullOrEmpty(contractorDisplay))
            {
                line15Display = $"{contractorDisplay}：";
            }
            else if (!string.IsNullOrEmpty(inv))
            {
                line15Display = $"インボイス番号：{inv}";
            }
            else
            {
                line15Display = string.Empty;
            }

            SetCellValue(worksheet, InvoiceNumberCellAddress, line15Display);
            SetCellValue(worksheet, "B7", currentPage >= 2 ? $"建物名称　{buildingName}" : string.Empty);
            SetCellValue(worksheet, "F17", buildingName);
            if (currentPage == 1)
            {
                if (string.IsNullOrWhiteSpace(bankTransferNotice))
                {
                    SetCellDateValue(worksheet, "B12", paymentDueDate);
                }
                else
                {
                    SetCellValue(worksheet, "B12", bankTransferNotice);
                }
            }
            else
            {
                SetCellValue(worksheet, "B12", string.Empty);
            }
            SetCellValue(worksheet, "J10", totalAmount);
            SetCellValue(worksheet, "AM1", printDate);
            SetCellValue(worksheet, "AT1", $"{currentPage}/{totalPages}");
        }

        private void ClearInvoiceDetailCells(object worksheet, int detailStartRow)
        {
            ClearCellRange(worksheet, $"B{detailStartRow}:B200");
            ClearCellRange(worksheet, $"H{detailStartRow}:H200");
            ClearCellRange(worksheet, $"AE{detailStartRow}:AE200");
            ClearCellRange(worksheet, $"AJ{detailStartRow}:AJ200");
            ClearCellRange(worksheet, $"AM{detailStartRow}:AM200");
            ClearCellRange(worksheet, $"AS{detailStartRow}:AS200");
        }

        private void ClearCellRange(object worksheet, string rangeAddress)
        {
            object? range = GetWorksheetRange(worksheet, rangeAddress);
            if (range == null)
            {
                return;
            }

            if (CanClearRangeDirectly(range))
            {
                try
                {
                    range.GetType().InvokeMember("ClearContents", System.Reflection.BindingFlags.InvokeMethod, null, range, null);
                    return;
                }
                catch
                {
                    // 結合セルを含む場合はセル単位でクリアする
                }
            }

            ClearRangeCellByCell(range);
        }

        private void ClearCellContents(object worksheet, string cellAddress)
        {
            SetCellValue(worksheet, cellAddress, string.Empty);
        }

        private static object? GetWorksheetRange(object worksheet, string rangeAddress)
        {
            return worksheet.GetType().InvokeMember(
                "Range",
                System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod,
                null,
                worksheet,
                new object[] { rangeAddress });
        }

        private static bool CanClearRangeDirectly(object range)
        {
            try
            {
                var mergeCells = range.GetType().InvokeMember(
                    "MergeCells",
                    System.Reflection.BindingFlags.GetProperty,
                    null,
                    range,
                    null);
                return mergeCells is bool isMerged && !isMerged;
            }
            catch
            {
                return false;
            }
        }

        private static void ClearRangeCellByCell(object range)
        {
            object? rows = range.GetType().InvokeMember("Rows", System.Reflection.BindingFlags.GetProperty, null, range, null);
            object? columns = range.GetType().InvokeMember("Columns", System.Reflection.BindingFlags.GetProperty, null, range, null);
            if (rows == null || columns == null)
            {
                return;
            }

            var rowCount = Convert.ToInt32(rows.GetType().InvokeMember("Count", System.Reflection.BindingFlags.GetProperty, null, rows, null));
            var colCount = Convert.ToInt32(columns.GetType().InvokeMember("Count", System.Reflection.BindingFlags.GetProperty, null, columns, null));
            var clearedMergeAreas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var r = 1; r <= rowCount; r++)
            {
                for (var c = 1; c <= colCount; c++)
                {
                    object? cell = range.GetType().InvokeMember(
                        "Cells",
                        System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod,
                        null,
                        range,
                        new object[] { r, c });
                    if (cell == null)
                    {
                        continue;
                    }

                    var writableRange = ResolveWritableRange(cell);
                    if (writableRange == null)
                    {
                        continue;
                    }

                    var areaKey = GetRangeAddressKey(writableRange);
                    if (!string.IsNullOrEmpty(areaKey) && !clearedMergeAreas.Add(areaKey))
                    {
                        continue;
                    }

                    writableRange.GetType().InvokeMember(
                        "Value2",
                        System.Reflection.BindingFlags.SetProperty,
                        null,
                        writableRange,
                        new object[] { string.Empty });
                }
            }
        }

        private static string GetRangeAddressKey(object range)
        {
            try
            {
                var address = range.GetType().InvokeMember(
                    "Address",
                    System.Reflection.BindingFlags.GetProperty,
                    null,
                    range,
                    new object[] { false, false });
                return address?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static object? ResolveWritableRange(object? range)
        {
            if (range == null)
            {
                return null;
            }

            try
            {
                var mergeCells = range.GetType().InvokeMember(
                    "MergeCells",
                    System.Reflection.BindingFlags.GetProperty,
                    null,
                    range,
                    null);
                if (mergeCells is bool isMerged && isMerged)
                {
                    return range.GetType().InvokeMember(
                        "MergeArea",
                        System.Reflection.BindingFlags.GetProperty,
                        null,
                        range,
                        null);
                }
            }
            catch
            {
                // 結合判定に失敗した場合はそのまま使う
            }

            return range;
        }

        private void SetCellValue(object worksheet, string cellAddress, object value)
        {
            object? range = GetWorksheetRange(worksheet, cellAddress);
            range = ResolveWritableRange(range);
            if (range == null)
            {
                return;
            }

            var cellValue = value ?? string.Empty;
            if (value is string s && string.IsNullOrEmpty(s))
            {
                cellValue = string.Empty;
            }

            range.GetType().InvokeMember(
                "Value2",
                System.Reflection.BindingFlags.SetProperty,
                null,
                range,
                new object[] { cellValue });
        }

        private void SetCellDateValue(object worksheet, string cellAddress, DateTime date)
        {
            object? range = GetWorksheetRange(worksheet, cellAddress);
            range = ResolveWritableRange(range);
            if (range == null)
            {
                return;
            }

            try
            {
                range.GetType().InvokeMember("Formula", System.Reflection.BindingFlags.SetProperty, null, range, new object[] { string.Empty });
            }
            catch
            {
                // 数式が無い、または結合セルで設定できない場合がある
            }

            range.GetType().InvokeMember(
                "Value2",
                System.Reflection.BindingFlags.SetProperty,
                null,
                range,
                new object[] { date.ToOADate() });

            try
            {
                range.GetType().InvokeMember(
                    "NumberFormat",
                    System.Reflection.BindingFlags.SetProperty,
                    null,
                    range,
                    new object[] { "yyyy/mm/dd" });
            }
            catch
            {
                // 表示形式を設定できない場合は Excel 既定の日付表示に任せる
            }
        }

        /// <summary>課税別合計など B 列の1行文言の折り返しをオフにする。</summary>
        private void ApplySummaryRoomCellDisplay(object worksheet, string cellAddress)
        {
            object? range = GetWorksheetRange(worksheet, cellAddress);
            range = ResolveWritableRange(range);
            if (range == null)
            {
                return;
            }

            try
            {
                range.GetType().InvokeMember(
                    "WrapText",
                    System.Reflection.BindingFlags.SetProperty,
                    null,
                    range,
                    new object[] { false });
            }
            catch
            {
                // 結合セルによっては設定できない場合がある
            }
        }

        private static string FormatTaxRate(decimal taxRate)
        {
            if (taxRate == 0m)
            {
                return "不課税";
            }

            var percent = taxRate >= 1m ? Math.Floor(taxRate) : Math.Floor(taxRate * 100m);
            return $"{percent}％税込";
        }

        private string GetCellAddress(string startCell, int rowOffset, int? overrideStartRow = null)
        {
            var index = 0;
            while (index < startCell.Length && char.IsLetter(startCell[index]))
            {
                index++;
            }
            var column = startCell.Substring(0, index);
            var startRowText = startCell.Substring(index);
            if (!int.TryParse(startRowText, out var startRow))
            {
                startRow = InvoiceDetailStartRow;
            }
            if (overrideStartRow.HasValue)
            {
                startRow = overrideStartRow.Value;
            }
            return $"{column}{startRow + rowOffset}";
        }

        private static string GetBillingToMapKey(string? buildingName, string? billingTo)
        {
            var b = (buildingName ?? string.Empty).Trim();
            var t = (billingTo ?? string.Empty).Trim();
            return $"{b}\t{t}";
        }

        private async Task<Dictionary<string, BillingToTransferInfo>> BuildBillingToTransferMethodMapAsync(List<InvoiceDetail> invoiceDetails)
        {
            var map = new Dictionary<string, BillingToTransferInfo>(StringComparer.OrdinalIgnoreCase);
            var buildingClientsCache = new Dictionary<string, List<Client>>(StringComparer.OrdinalIgnoreCase);
            List<Client>? allBillingClients = null;

            var targets = invoiceDetails
                .Where(d => !string.IsNullOrWhiteSpace(d.BillingTo))
                .Select(d => new
                {
                    BuildingName = (d.BuildingName ?? string.Empty).Trim(),
                    BillingTo = (d.BillingTo ?? string.Empty).Trim()
                })
                .Distinct()
                .ToList();

            foreach (var target in targets)
            {
                var mapKey = GetBillingToMapKey(target.BuildingName, target.BillingTo);
                if (string.IsNullOrWhiteSpace(target.BillingTo))
                {
                    map[mapKey] = BillingToTransferInfo.Unknown;
                    continue;
                }

                Client? billingClient = null;
                if (!string.IsNullOrWhiteSpace(target.BuildingName))
                {
                    if (!buildingClientsCache.TryGetValue(target.BuildingName, out var clients))
                    {
                        clients = await ClientDataAccess.GetClientsByBuildingNameAsync(target.BuildingName);
                        buildingClientsCache[target.BuildingName] = clients;
                    }

                    billingClient = clients.FirstOrDefault(c =>
                        c.IsBillingTo &&
                        string.Equals((c.Name ?? string.Empty).Trim(), target.BillingTo, StringComparison.OrdinalIgnoreCase));
                }

                if (billingClient == null)
                {
                    allBillingClients ??= (await ClientDataAccess.GetAllClientsAsync())
                        .Where(c => c.IsBillingTo)
                        .ToList();
                    var nameMatches = allBillingClients
                        .Where(c => string.Equals((c.Name ?? string.Empty).Trim(), target.BillingTo, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    billingClient = nameMatches
                        .OrderByDescending(c => string.Equals((c.BuildingName ?? string.Empty).Trim(), target.BuildingName, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();
                }

                map[mapKey] = BillingToTransferInfo.FromClient(billingClient);
            }

            return map;
        }

        /// <summary>
        /// 請求書 B12 に表示する支払期限日。自動振替は請求予定日（27日）を優先し、口座振込は翌月末。
        /// </summary>
        private static DateTime ResolveInvoicePaymentDueDate(
            BillingToTransferInfo transferInfo,
            List<InvoiceDetail> groupDetails,
            DateTime billingMonth)
        {
            var billingMonthStart = new DateTime(billingMonth.Year, billingMonth.Month, 1);
            var defaultDueDate = billingMonthStart.AddMonths(2).AddDays(-1);
            var autoTransferDueDate = GetAutoTransferDueDate(billingMonthStart);

            if (transferInfo.Method == TransferMethod.Bank)
            {
                return defaultDueDate;
            }

            var isAutoTransfer = transferInfo.Method == TransferMethod.Auto
                || (transferInfo.BillingClient?.IsAutoTransfer ?? false);
            var confirmedDates = groupDetails
                .Where(d => d.ConfirmedBillingDate.HasValue)
                .Select(d => d.ConfirmedBillingDate!.Value.Date)
                .Distinct()
                .ToList();
            if (!isAutoTransfer && confirmedDates.Count > 0 && confirmedDates.All(IsAutoTransferPlannedBillingDate))
            {
                isAutoTransfer = true;
            }

            var confirmedBillingDate = groupDetails
                .Select(d => d.ConfirmedBillingDate)
                .FirstOrDefault(d => d.HasValue);

            if (isAutoTransfer)
            {
                if (confirmedBillingDate.HasValue)
                {
                    var confirmedDate = confirmedBillingDate.Value.Date;
                    if (IsAutoTransferPlannedBillingDate(confirmedDate))
                    {
                        return confirmedDate;
                    }
                }

                return autoTransferDueDate;
            }

            if (confirmedBillingDate.HasValue)
            {
                return confirmedBillingDate.Value.Date;
            }

            return defaultDueDate;
        }

        /// <summary>自動振替の請求予定日として印刷に使える日付か（27日、または月末以外）。</summary>
        private static bool IsAutoTransferPlannedBillingDate(DateTime date)
        {
            if (date.Day == 27)
            {
                return true;
            }

            return date.Day != DateTime.DaysInMonth(date.Year, date.Month);
        }

        private static DateTime GetAutoTransferDueDate(DateTime billingMonthStart)
        {
            var nextMonth = billingMonthStart.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 27);
        }

        private readonly struct BillingToTransferInfo
        {
            public static BillingToTransferInfo Unknown => new BillingToTransferInfo(TransferMethod.Unknown, null);

            public TransferMethod Method { get; }
            public Client? BillingClient { get; }

            public BillingToTransferInfo(TransferMethod method, Client? billingClient)
            {
                Method = method;
                BillingClient = billingClient;
            }

            public static BillingToTransferInfo FromClient(Client? billingClient)
            {
                if (billingClient == null)
                {
                    return Unknown;
                }

                // 両方ONの既存データがある場合は、自動振込を優先して日付を表示する。
                var method = billingClient.IsAutoTransfer
                    ? TransferMethod.Auto
                    : (billingClient.IsBankTransfer ? TransferMethod.Bank : TransferMethod.Unknown);
                return new BillingToTransferInfo(method, billingClient);
            }
        }

        private enum TransferMethod
        {
            Unknown = 0,
            Auto = 1,
            Bank = 2
        }

        private sealed class InvoiceDetailRow
        {
            public string RoomText { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            /// <summary>テンプレート H 列の数式を消し、B 列の1行文言だけを表示する。</summary>
            public bool ClearContentCell { get; set; }
            public decimal? UsageAmount { get; set; }
            public string Unit { get; set; } = string.Empty;
            public decimal? TaxInclusiveAmount { get; set; }
            public string TaxRate { get; set; } = string.Empty;
        }

        /// <summary>請求一覧の1行（部屋別集計）</summary>
        private sealed class InvoiceListRow
        {
            public string BuildingName { get; set; } = string.Empty;
            public string RoomName { get; set; } = string.Empty;
            public string BillingTo { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
            public decimal TaxExclusiveAmount { get; set; }
            public decimal ConsumptionTax { get; set; }
            public decimal ElectricBasic { get; set; }
            public decimal ElectricUsage { get; set; }
            public decimal GasAmount { get; set; }
            public decimal WaterAmount { get; set; }
            public decimal OtherAmount { get; set; }
        }

        /// <summary>請求明細を部屋別に集計し、請求一覧用の行リストを生成する。</summary>
        private List<InvoiceListRow> BuildInvoiceListRows(List<InvoiceDetail> invoiceDetails)
        {
            var rows = new List<InvoiceListRow>();
            var groups = invoiceDetails
                .Where(d => !string.IsNullOrWhiteSpace(d.BuildingName) || !string.IsNullOrWhiteSpace(d.RoomNumber))
                .GroupBy(d => new { BuildingName = d.BuildingName?.Trim() ?? string.Empty, RoomNumber = d.RoomNumber?.Trim() ?? string.Empty });

            foreach (var g in groups.OrderBy(x => x.Key.BuildingName).ThenBy(x => x.Key.RoomNumber))
            {
                var list = g.ToList();
                decimal totalAmount = 0m;
                decimal taxExclusiveSum = 0m;

                decimal electricBasic = 0m;
                decimal electricUsage = 0m;
                decimal gasAmount = 0m;
                decimal waterAmount = 0m;
                decimal otherAmount = 0m;

                foreach (var d in list)
                {
                    totalAmount += d.TaxInclusiveAmount;
                    decimal rate = d.TaxRate >= 1 ? d.TaxRate / 100m : d.TaxRate;
                    if (1 + rate != 0)
                        taxExclusiveSum += d.TaxInclusiveAmount / (1 + rate);

                    var content = d.Content?.Trim() ?? string.Empty;
                    if (content == "電気料金（基本料）")
                        electricBasic += d.TaxInclusiveAmount;
                    else if (content == "電気料金（従量電気料）")
                        electricUsage += d.TaxInclusiveAmount;
                    else if (content == "ガス料金")
                        gasAmount += d.TaxInclusiveAmount;
                    else if (content == "水道料金")
                        waterAmount += d.TaxInclusiveAmount;
                    else
                        otherAmount += d.TaxInclusiveAmount;
                }

                rows.Add(new InvoiceListRow
                {
                    BuildingName = g.Key.BuildingName,
                    RoomName = g.Key.RoomNumber,
                    BillingTo = list.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.BillingTo))?.BillingTo?.Trim() ?? string.Empty,
                    TotalAmount = totalAmount,
                    TaxExclusiveAmount = taxExclusiveSum,
                    ConsumptionTax = totalAmount - taxExclusiveSum,
                    ElectricBasic = electricBasic,
                    ElectricUsage = electricUsage,
                    GasAmount = gasAmount,
                    WaterAmount = waterAmount,
                    OtherAmount = otherAmount
                });
            }

            return rows;
        }

        private void ClearInvoiceListDataAreasInBlocks(object worksheet, int numBlocks)
        {
            for (var p = 0; p < numBlocks; p++)
            {
                var blockStart = 1 + p * InvoiceListBlockRowCount;
                var dataStart = blockStart + (InvoiceListDataStartRow - 1);
                var dataEnd = blockStart + (InvoiceListDataEndRow - 1);
                ClearCellRange(worksheet, $"C{dataStart}:BH{dataEnd}");
            }
        }

        private void DeleteInvoiceListRowsAfter(object worksheet, int lastKeptRow)
        {
            if (lastKeptRow < 1) return;
            var fromRow = lastKeptRow + 1;
            var toRow = lastKeptRow + 4000;
            try
            {
                object? rng = worksheet.GetType().InvokeMember("Range", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheet, new object[] { $"{fromRow}:{toRow}" });
                if (rng == null) return;
                object? entireRow = rng.GetType().InvokeMember("EntireRow", System.Reflection.BindingFlags.GetProperty, null, rng, null);
                if (entireRow != null)
                    entireRow.GetType().InvokeMember("Delete", System.Reflection.BindingFlags.InvokeMethod, null, entireRow, null);
            }
            catch
            {
                // 環境により削除に失敗することがある
            }
        }

        /// <summary>2ブロック目以降に、シート先頭の1～29行をコピーして貼り付ける。</summary>
        private void DuplicateInvoiceListTemplateBlocks(object worksheet, int numBlocks)
        {
            if (numBlocks <= 1) return;
            object? sourceRange = worksheet.GetType().InvokeMember("Range", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheet, new object[] { $"1:{InvoiceListBlockRowCount}" });
            if (sourceRange == null) return;
            for (var p = 1; p < numBlocks; p++)
            {
                var destRow = 1 + p * InvoiceListBlockRowCount;
                object? destTopLeft = worksheet.GetType().InvokeMember("Range", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheet, new object[] { $"A{destRow}" });
                if (destTopLeft == null) continue;
                try
                {
                    sourceRange.GetType().InvokeMember("Copy", System.Reflection.BindingFlags.InvokeMethod, null, sourceRange, new object[] { destTopLeft });
                }
                catch
                {
                    // コピー失敗時は当該ブロックが空のままになる
                }
            }
        }

        private void ApplyInvoiceListHeaderCellsPerBlock(object worksheet, DateTime billingDate, DateTime outputDate, int numBlocks)
        {
            for (var p = 0; p < numBlocks; p++)
            {
                var row3 = 3 + p * InvoiceListBlockRowCount;
                SetCellValue(worksheet, "AN" + row3, billingDate.ToString("yyyy'年'"));
                SetCellValue(worksheet, "AR" + row3, billingDate.ToString("MM'月分'"));
                SetCellValue(worksheet, "BS" + row3, outputDate.ToString("yyyy'年'MM'月'dd'日'"));

                var rowPage = InvoiceListPageIndicatorRowInBlock + p * InvoiceListBlockRowCount;
                SetCellValue(worksheet, InvoiceListPageIndicatorColumn + rowPage, $"{p + 1}／{numBlocks}ページ");
            }
        }

        private void WriteInvoiceListDataRows(object worksheet, List<InvoiceListRow> listRows)
        {
            for (var i = 0; i < listRows.Count && i < InvoiceListMaxDataRows; i++)
            {
                var row = listRows[i];
                var blockIndex = i / InvoiceListRowsPerPage;
                var rowInBlock = i % InvoiceListRowsPerPage;
                var blockStartRow = 1 + blockIndex * InvoiceListBlockRowCount;
                var r = blockStartRow + (InvoiceListDataStartRow - 1) + rowInBlock;
                SetCellValue(worksheet, "C" + r, row.BuildingName);
                SetCellValue(worksheet, "N" + r, row.RoomName);
                SetCellValue(worksheet, "R" + r, row.BillingTo);
                SetCellValue(worksheet, "AI" + r, row.TotalAmount);
                SetCellValue(worksheet, "AN" + r, row.TotalAmount);
                SetCellValue(worksheet, "AS" + r, row.ElectricBasic);
                SetCellValue(worksheet, "AX" + r, row.ElectricUsage);
                SetCellValue(worksheet, "BC" + r, row.GasAmount);
                SetCellValue(worksheet, "BH" + r, row.WaterAmount);
            }
        }

        /// <summary>各ブロック（29行）の直前に横改ページを入れる。1ブロック目の前は入れない。</summary>
        private void SetInvoiceListPageBreaks(object worksheet, int numBlocks)
        {
            if (numBlocks <= 1) return;

            try
            {
                worksheet.GetType().InvokeMember("ResetAllPageBreaks", System.Reflection.BindingFlags.InvokeMethod, null, worksheet, null);
            }
            catch
            {
                // 一部の Excel バージョンでは未サポート
            }

            try
            {
                object? pageSetup = worksheet.GetType().InvokeMember("PageSetup", System.Reflection.BindingFlags.GetProperty, null, worksheet, null);
                if (pageSetup != null)
                    pageSetup.GetType().InvokeMember("Zoom", System.Reflection.BindingFlags.SetProperty, null, pageSetup, new object[] { 100 });
            }
            catch
            {
                // テンプレートの PageSetup によっては設定できない場合がある
            }

            object? hPageBreaks = worksheet.GetType().InvokeMember("HPageBreaks", System.Reflection.BindingFlags.GetProperty, null, worksheet, null);
            object? rowsCollection = worksheet.GetType().InvokeMember("Rows", System.Reflection.BindingFlags.GetProperty, null, worksheet, null);
            if (hPageBreaks == null || rowsCollection == null)
                return;

            const int xlPageBreakManual = -4135;
            for (var p = 1; p < numBlocks; p++)
            {
                var breakRow = 1 + p * InvoiceListBlockRowCount;
                try
                {
                    object? rowRange = rowsCollection.GetType().InvokeMember("Item", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, rowsCollection, new object[] { breakRow });
                    if (rowRange != null)
                        hPageBreaks.GetType().InvokeMember("Add", System.Reflection.BindingFlags.InvokeMethod, null, hPageBreaks, new object[] { rowRange });
                }
                catch
                {
                    try
                    {
                        object? range = worksheet.GetType().InvokeMember("Range", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheet, new object[] { $"{breakRow}:{breakRow}" });
                        if (range != null)
                            range.GetType().InvokeMember("PageBreak", System.Reflection.BindingFlags.SetProperty, null, range, new object[] { xlPageBreakManual });
                    }
                    catch
                    {
                        // HPageBreaks.Add と PageBreak のいずれも不可
                    }
                }
            }
        }

        private void BuildInvoiceListSheetForPrint(object worksheet, List<InvoiceListRow> listRows, DateTime billingDate, DateTime outputDate)
        {
            var numBlocks = (listRows.Count + InvoiceListRowsPerPage - 1) / InvoiceListRowsPerPage;
            if (numBlocks < 1) return;

            var lastKeptRow = numBlocks * InvoiceListBlockRowCount;
            DeleteInvoiceListRowsAfter(worksheet, lastKeptRow);
            DuplicateInvoiceListTemplateBlocks(worksheet, numBlocks);
            ClearInvoiceListDataAreasInBlocks(worksheet, numBlocks);
            ApplyInvoiceListHeaderCellsPerBlock(worksheet, billingDate, outputDate, numBlocks);
            WriteInvoiceListDataRows(worksheet, listRows);
            SetInvoiceListPageBreaks(worksheet, numBlocks);
        }

        private async void BtnPrintInvoiceList_Click(object? sender, EventArgs e)
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

                var billingYearMonth = dtpBillingYearMonth.Value;
                var invoiceDetails = await InvoiceDetailDataAccess.GetInvoiceDetailsByBillingYearMonthAsync(billingYearMonth.Year, billingYearMonth.Month);
                var selectedBillingTo = GetSelectedBillingTo();
                if (!string.IsNullOrEmpty(selectedBillingTo))
                {
                    invoiceDetails = invoiceDetails
                        .Where(d => string.Equals(d.BillingTo, selectedBillingTo, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (invoiceDetails.Count == 0)
                {
                    MessageBox.Show("指定した請求年月の請求明細データがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var listRows = BuildInvoiceListRows(invoiceDetails);
                if (listRows.Count == 0)
                {
                    MessageBox.Show("部屋別に集計できる請求明細データがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                PrintExcelFileDirectlyForInvoiceList(filePath, listRows);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"印刷に失敗しました:\n\n{ex.Message}\n\nスタックトレース:\n{ex.StackTrace}",
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintExcelFileDirectlyForInvoiceList(string filePath, List<InvoiceListRow> listRows)
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
                            var activePrinter = string.IsNullOrEmpty(selectedPrinter) ? Type.Missing : selectedPrinter;
                            
                            // ワークシートを取得
                            object? worksheets = workbook.GetType().InvokeMember("Worksheets", System.Reflection.BindingFlags.GetProperty, null, workbook, null);
                            if (worksheets != null)
                            {
                                object? worksheet = worksheets.GetType().InvokeMember("Item", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.InvokeMethod, null, worksheets, new object[] { 1 });
                                if (worksheet != null)
                                {
                                    var billingDate = dtpBillingYearMonth.Value;
                                    var outputDate = DateTime.Now;
                                    BuildInvoiceListSheetForPrint(worksheet, listRows, billingDate, outputDate);

                                    // PrintOutメソッドを呼び出して印刷を実行
                                    worksheet.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, worksheet, 
                                        new object[] { 
                                            Type.Missing,  // From
                                            Type.Missing,  // To
                                            Type.Missing,  // Copies
                                            false,         // Preview
                                            activePrinter, // ActivePrinter
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
                                            Type.Missing, Type.Missing, Type.Missing, false, activePrinter, false, Type.Missing, Type.Missing, false
                                        });
                                }
                            }
                            else
                            {
                                // ワークシートが取得できない場合、ワークブックのPrintOutメソッドを使用
                                workbook.GetType().InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, workbook, 
                                    new object[] { 
                                        Type.Missing, Type.Missing, Type.Missing, false, activePrinter, false, Type.Missing, Type.Missing, false
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

