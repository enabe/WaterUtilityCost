using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using WaterUtilityCost.DataAccess;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 請求明細データ作成フォーム
    /// </summary>
    public partial class BillingDataRegistrationForm : Form
    {
        private const string DebugTag = "LOG-V3";
        /// <summary>服部ビル：電気は親請求使用量で按分。水道は専用の貸主／借主2明細。</summary>
        private const string HattoriBuildingName = "服部ビル";
        private const string HattoriWaterLesseeDisplayName = "株式会社王府井";
        private const string HattoriWaterLessorClientName = "有限会社　服部牛肉店";
        private const string HattoriWaterLessorFloorName = "貸主";
        private const string HattoriWaterLesseeRoomNumber = "1F/2F";
        /// <summary>水道：親請求は画面月の前月、子メーター検針は終了＝前月・開始＝3か月前。明細の請求年月は画面月のまま。</summary>
        private static readonly string[] LagMonthWaterBuildingNames = { "岩本ビル", "ビラ・プェルタ" };
        /// <summary>水道：単価＝(基本+従量)÷親使用量、請求金額＝部屋別生検針差分×単価。差額割当なし。</summary>
        private const string PlazaCeresse37FWaterParentMeterName = "親メーター_プラザセレス石川Ⅰ_3-7F_水道";
        /// <summary>電気：請求明細データを自動作成しない建物。</summary>
        private static readonly string[] ElectricBillingExcludedBuildingNames = { "プラザセレス石川Ⅰビル" };
        private List<List<string>> _excelData = new List<List<string>>();
        
        // Excelフォーマット情報を保持
        private class CellFormatInfo
        {
            public string Text { get; set; } = "";
            public Color? BackgroundColor { get; set; }
            public Color? ForegroundColor { get; set; }
            public float FontSize { get; set; } = 9;
            public string FontName { get; set; } = "メイリオ";
            public bool IsBold { get; set; }
            public bool IsMerged { get; set; }
            public int MergeColumns { get; set; } = 1;
            public int MergeRows { get; set; } = 1;
            public System.Drawing.ContentAlignment? Alignment { get; set; }
            public bool HasBorder { get; set; }
        }
        
        private List<List<CellFormatInfo>> _excelFormatData = new List<List<CellFormatInfo>>();
        private Dictionary<int, float> _columnWidths = new Dictionary<int, float>();
        private Dictionary<int, float> _rowHeights = new Dictionary<int, float>();

        private class ElectricBillingDetailResult
        {
            public ElectricBilling Billing { get; set; } = new ElectricBilling();
            public Client Client { get; set; } = new Client();
            public InvoiceDetail InvoiceDetail { get; set; } = new InvoiceDetail();
            public decimal RoomUsage { get; set; }
            public decimal RoomArea { get; set; }
            public decimal TotalArea { get; set; }
            public decimal RoomShareRatio { get; set; }
            public decimal RoomBasicCharge { get; set; }
            public decimal ChildMeterUsageTotal { get; set; }
            public decimal SharedUsageTotal { get; set; }
            public decimal SharedUsage { get; set; }
            public decimal RoomUsageTotal { get; set; }
            public decimal RoomUnitPrice { get; set; }
            public decimal RoomPowerCharge { get; set; }
            public decimal RoomElectricCharge { get; set; }
        }

        private class WaterBillingDetailResult
        {
            public WaterBilling Billing { get; set; } = new WaterBilling();
            public Client Client { get; set; } = new Client();
            public InvoiceDetail InvoiceDetail { get; set; } = new InvoiceDetail();
            public decimal RoomUsage { get; set; }
            public decimal RoomArea { get; set; }
            public decimal TotalArea { get; set; }
            public decimal RoomShareRatio { get; set; }
            public decimal RoomUsageTotal { get; set; }
            public decimal RoomUnitPrice { get; set; }
            public decimal RoomBasicCharge { get; set; }
            public decimal RoomUsageCharge { get; set; }
            public decimal RoomWaterCharge { get; set; }
            public string MeterType { get; set; } = "水道";
        }

        private class GasBillingDetailResult
        {
            public GasBilling Billing { get; set; } = new GasBilling();
            public Client Client { get; set; } = new Client();
            public InvoiceDetail InvoiceDetail { get; set; } = new InvoiceDetail();
            public decimal RoomUsage { get; set; }
            public decimal RoomArea { get; set; }
            public decimal TotalArea { get; set; }
            public decimal RoomShareRatio { get; set; }
            public decimal RoomBasicCharge { get; set; }
            public decimal RoomUsageTotal { get; set; }
            public decimal RoomUnitPrice { get; set; }
            public decimal RoomUsageCharge { get; set; }
            public decimal RoomGasCharge { get; set; }
            public string MeterType { get; set; } = "ガス";
        }

        public BillingDataRegistrationForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
        }

        private void InitializeComponentAdditional()
        {
            this.Text = $"{this.Text} [{DebugTag}]";
            // 請求年月を現在の年月に設定
            var now = DateTime.Now;
            dtpBillingYearMonth.Value = new DateTime(now.Year, now.Month, 1);

            // イベントハンドラー
            btnCreateElectric.Click += BtnCreateElectric_Click;
            btnCreateGas.Click += BtnCreateGas_Click;
            btnCreateWater.Click += BtnCreateWater_Click;
            btnCancel.Click += BtnCancel_Click;
        }


        private bool LoadExcelData(string filePath)
        {
            _excelData.Clear();
            _excelFormatData.Clear();
            _columnWidths.Clear();
            _rowHeights.Clear();
            
            ExcelPackage? package = null;
            
            try
            {
                // ファイルの存在確認
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"ファイルが見つかりません:\n{filePath}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                
                // ファイルをバイト配列に読み込む（メモリに完全に読み込んでから処理することで、AccessViolationExceptionを回避）
                byte[] fileBytes;
                try
                {
                    fileBytes = File.ReadAllBytes(filePath);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"ファイルへのアクセスが拒否されました:\n\nファイルパス: {filePath}\n\nエラー詳細:\n{ex.Message}\n\nファイルが他のプログラムで開かれている可能性があります。", 
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"ファイルの読み込みに失敗しました:\n\nファイルパス: {filePath}\n\nエラー詳細:\n{ex.Message}\n\nファイルが他のプログラムで開かれている可能性があります。", 
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                
                // バイト配列からMemoryStreamを作成してExcelPackageを作成（より安全）
                using (var memoryStream = new MemoryStream(fileBytes))
                {
                    package = new ExcelPackage(memoryStream);
                
                    // シートの存在確認
                    if (package.Workbook.Worksheets == null || package.Workbook.Worksheets.Count == 0)
                    {
                        MessageBox.Show("Excelファイルにシートがありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    
                    // 最初のシートを安全に取得
                    ExcelWorksheet? worksheet = null;
                    string? worksheetName = null;
                    
                    try
                    {
                        // FirstOrDefault()を使用して最初のシートを取得
                        worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        
                        // FirstOrDefault()が失敗した場合、インデックスで取得を試みる
                        if (worksheet == null && package.Workbook.Worksheets.Count > 0)
                        {
                            try
                            {
                                worksheet = package.Workbook.Worksheets[0];
                            }
                            catch (System.AccessViolationException)
                            {
                                // AccessViolationExceptionが発生した場合、列挙で取得を試みる
                                foreach (var sheet in package.Workbook.Worksheets)
                                {
                                    try
                                    {
                                        worksheet = sheet;
                                        worksheetName = sheet.Name;
                                        break;
                                    }
                                    catch
                                    {
                                        // 個別のシートアクセスエラーは無視して続行
                                        continue;
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // インデックスアクセスが失敗した場合、列挙で取得を試みる
                                foreach (var sheet in package.Workbook.Worksheets)
                                {
                                    worksheet = sheet;
                                    worksheetName = sheet.Name;
                                    break;
                                }
                            }
                        }
                        
                        if (worksheet != null)
                        {
                            worksheetName = worksheet.Name;
                        }
                    }
                    catch (System.AccessViolationException)
                    {
                        // AccessViolationExceptionが発生した場合、列挙で取得を試みる
                        foreach (var sheet in package.Workbook.Worksheets)
                        {
                            try
                            {
                                worksheet = sheet;
                                worksheetName = sheet.Name;
                                break;
                            }
                            catch
                            {
                                // 個別のシートアクセスエラーは無視して続行
                                continue;
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // インデックスアクセスが失敗した場合、列挙で取得を試みる
                        foreach (var sheet in package.Workbook.Worksheets)
                        {
                            worksheet = sheet;
                            worksheetName = sheet.Name;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        // その他のエラーの場合、列挙で取得を試みる
                        foreach (var sheet in package.Workbook.Worksheets)
                        {
                            worksheet = sheet;
                            worksheetName = sheet.Name;
                            break;
                        }
                    }
                    
                    if (worksheet == null)
                    {
                        MessageBox.Show("Excelファイルからシートを取得できませんでした。\n\nシートが存在するか確認してください。", 
                            "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    
                    // Dimensionの取得を安全に行う
                    ExcelAddressBase? dimension = null;
                    try
                    {
                        dimension = worksheet.Dimension;
                    }
                    catch (System.AccessViolationException)
                    {
                        MessageBox.Show($"Excelファイルのシート「{worksheetName ?? "不明"}」のデータ範囲を取得できませんでした。\n\nファイルが破損している可能性があります。", 
                            "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    
                    if (dimension == null)
                    {
                        MessageBox.Show($"Excelファイルのシート「{worksheetName ?? "不明"}」にデータがありません。", 
                            "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    
                    var startRow = dimension.Start.Row;
                    var endRow = dimension.End.Row;
                    var startCol = dimension.Start.Column;
                    var endCol = dimension.End.Column;
                    
                    // データとフォーマット情報を先に読み込んで、usingブロックの外で使用できるようにする
                    var tempData = new List<List<string>>();
                    var tempFormatData = new List<List<CellFormatInfo>>();
                    var tempColumnWidths = new Dictionary<int, float>();
                    var tempRowHeights = new Dictionary<int, float>();
                    
                    // 列幅を取得
                    for (int col = startCol; col <= endCol; col++)
                    {
                        try
                        {
                            var column = worksheet.Column(col);
                            if (column != null && column.Width > 0)
                            {
                                tempColumnWidths[col] = (float)column.Width;
                            }
                        }
                        catch
                        {
                            // 列幅取得エラーは無視
                        }
                    }
                    
                    // 行ごとに処理（メモリ使用量を抑える）
                    for (int row = startRow; row <= endRow; row++)
                    {
                        var rowData = new List<string>();
                        var rowFormatData = new List<CellFormatInfo>();
                        
                        // 行高を取得
                        try
                        {
                            var rowObj = worksheet.Row(row);
                            if (rowObj != null && rowObj.Height > 0)
                            {
                                tempRowHeights[row] = (float)rowObj.Height;
                            }
                        }
                        catch
                        {
                            // 行高取得エラーは無視
                        }
                        
                        try
                        {
                            for (int col = startCol; col <= endCol; col++)
                            {
                                var cellInfo = new CellFormatInfo();
                                
                                try
                                {
                                    // Textプロパティを使用（より安全）
                                    var cell = worksheet.Cells[row, col];
                                    
                                    try
                                    {
                                        // セル値の取得（Textプロパティを優先、失敗した場合はValueプロパティを使用）
                                        try
                                        {
                                            cellInfo.Text = cell.Text ?? "";
                                        }
                                        catch
                                        {
                                            // Textプロパティが失敗した場合、Valueプロパティを使用
                                            try
                                            {
                                                var value = cell.Value;
                                                if (value != null)
                                                {
                                                    cellInfo.Text = value.ToString() ?? "";
                                                }
                                                else
                                                {
                                                    cellInfo.Text = "";
                                                }
                                            }
                                            catch
                                            {
                                                cellInfo.Text = "";
                                            }
                                        }
                                        
                                        // フォーマット情報を取得
                                        try
                                        {
                                            var style = cell.Style;
                                            
                                            // 背景色
                                            if (style.Fill.BackgroundColor.Rgb != null)
                                            {
                                                var rgb = style.Fill.BackgroundColor.Rgb;
                                                if (rgb.Length == 6)
                                                {
                                                    var r = Convert.ToInt32(rgb.Substring(0, 2), 16);
                                                    var g = Convert.ToInt32(rgb.Substring(2, 2), 16);
                                                    var b = Convert.ToInt32(rgb.Substring(4, 2), 16);
                                                    cellInfo.BackgroundColor = Color.FromArgb(r, g, b);
                                                }
                                            }
                                            
                                            // フォント情報
                                            if (style.Font != null)
                                            {
                                                cellInfo.IsBold = style.Font.Bold;
                                                
                                                // フォントサイズ
                                                if (style.Font.Size > 0)
                                                {
                                                    cellInfo.FontSize = (float)style.Font.Size;
                                                }
                                                
                                                // フォント名（日本語フォントを優先）
                                                if (!string.IsNullOrEmpty(style.Font.Name))
                                                {
                                                    string excelFontName = style.Font.Name;
                                                    // Excelのフォント名が日本語フォントでない場合、メイリオを使用
                                                    if (!excelFontName.Contains("メイリオ") && !excelFontName.Contains("MS Gothic") && 
                                                        !excelFontName.Contains("MS PGothic") && !excelFontName.Contains("MS UI Gothic") &&
                                                        !excelFontName.Contains("Yu Gothic") && !excelFontName.Contains("游ゴシック") &&
                                                        !excelFontName.Contains("ヒラギノ") && !excelFontName.Contains("Hiragino"))
                                                    {
                                                        cellInfo.FontName = "メイリオ"; // 日本語フォントにフォールバック
                                                    }
                                                    else
                                                    {
                                                        cellInfo.FontName = excelFontName;
                                                    }
                                                }
                                                else
                                                {
                                                    cellInfo.FontName = "メイリオ"; // デフォルトはメイリオ
                                                }
                                                
                                                // フォント色
                                                if (style.Font.Color.Rgb != null)
                                                {
                                                    var rgb = style.Font.Color.Rgb;
                                                    if (rgb.Length == 6)
                                                    {
                                                        var r = Convert.ToInt32(rgb.Substring(0, 2), 16);
                                                        var g = Convert.ToInt32(rgb.Substring(2, 2), 16);
                                                        var b = Convert.ToInt32(rgb.Substring(4, 2), 16);
                                                        cellInfo.ForegroundColor = Color.FromArgb(r, g, b);
                                                    }
                                                }
                                            }
                                            
                                            // 配置
                                            if (style.HorizontalAlignment == OfficeOpenXml.Style.ExcelHorizontalAlignment.Center)
                                            {
                                                cellInfo.Alignment = ContentAlignment.MiddleCenter;
                                            }
                                            else if (style.HorizontalAlignment == OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                                            {
                                                cellInfo.Alignment = ContentAlignment.MiddleRight;
                                            }
                                            else
                                            {
                                                cellInfo.Alignment = ContentAlignment.MiddleLeft;
                                            }
                                            
                                            // 罫線
                                            cellInfo.HasBorder = style.Border.Left.Style != OfficeOpenXml.Style.ExcelBorderStyle.None ||
                                                                style.Border.Right.Style != OfficeOpenXml.Style.ExcelBorderStyle.None ||
                                                                style.Border.Top.Style != OfficeOpenXml.Style.ExcelBorderStyle.None ||
                                                                style.Border.Bottom.Style != OfficeOpenXml.Style.ExcelBorderStyle.None;
                                        }
                                        catch
                                        {
                                            // フォーマット情報取得エラーは無視
                                        }
                                        
                                        // セル結合情報を取得
                                        try
                                        {
                                            var mergedCells = worksheet.MergedCells;
                                            if (mergedCells != null && mergedCells.Count > 0)
                                            {
                                                var cellAddress = cell.Address;
                                                foreach (var mergedRange in mergedCells)
                                                {
                                                    try
                                                    {
                                                        var range = worksheet.Cells[mergedRange];
                                                        if (range.Start.Address == cellAddress)
                                                        {
                                                            cellInfo.IsMerged = true;
                                                            cellInfo.MergeColumns = range.Columns;
                                                            cellInfo.MergeRows = range.Rows;
                                                            break;
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        // 個別の結合範囲の処理エラーは無視
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // セル結合情報取得エラーは無視
                                        }
                                    }
                                    catch (System.AccessViolationException)
                                    {
                                        // AccessViolationExceptionの場合は空文字を使用
                                        cellInfo.Text = "";
                                    }
                                    catch
                                    {
                                        // その他のエラーの場合も空文字を使用
                                        cellInfo.Text = "";
                                    }
                                }
                                catch (System.AccessViolationException)
                                {
                                    // AccessViolationExceptionの場合は空のセル情報を作成
                                    cellInfo.Text = "";
                                }
                                catch
                                {
                                    // その他のセルアクセスエラーの場合は空のセル情報を作成
                                    cellInfo.Text = "";
                                }
                                
                                rowData.Add(cellInfo.Text);
                                rowFormatData.Add(cellInfo);
                            }
                        }
                        catch (System.AccessViolationException)
                        {
                            // 行全体のアクセスエラーの場合、空の行を追加
                            for (int col = startCol; col <= endCol; col++)
                            {
                                rowData.Add("");
                                rowFormatData.Add(new CellFormatInfo());
                            }
                        }
                        catch
                        {
                            // その他の行アクセスエラーの場合、空の行を追加
                            for (int col = startCol; col <= endCol; col++)
                            {
                                rowData.Add("");
                                rowFormatData.Add(new CellFormatInfo());
                            }
                        }
                        
                        tempData.Add(rowData);
                        tempFormatData.Add(rowFormatData);
                    }
                    
                    // データをコピー（usingブロックの外で使用するため）
                    _excelData = tempData;
                    _excelFormatData = tempFormatData;
                    _columnWidths = tempColumnWidths;
                    _rowHeights = tempRowHeights;
                }
                
                return true;
            }
            catch (System.AccessViolationException ex)
            {
                MessageBox.Show($"メモリアクセスエラーが発生しました:\n\nファイルパス: {filePath}\n\nエラー詳細:\n{ex.Message}\n\nExcelファイルが破損している可能性があります。\n別のファイルで試すか、ファイルを再作成してください。", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"ファイルへのアクセスが拒否されました:\n\nファイルパス: {filePath}\n\nエラー詳細:\n{ex.Message}\n\nファイルが他のプログラムで開かれている可能性があります。", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"ファイルの読み込みに失敗しました:\n\nファイルパス: {filePath}\n\nエラー詳細:\n{ex.Message}\n\nファイルが他のプログラムで開かれている可能性があります。", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show($"Excelファイルのシートにアクセスできませんでした:\n\nファイルパス: {filePath}\n\nエラー詳細:\n{ex.Message}\n\nExcelファイルにシートが存在するか確認してください。", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                var errorType = ex.GetType().Name;
                MessageBox.Show($"Excelファイルの読み込みに失敗しました:\n\nファイルパス: {filePath}\n\nエラータイプ: {errorType}\nエラー詳細:\n{ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // リソースを確実に解放
                if (package != null)
                {
                    try
                    {
                        package.Dispose();
                    }
                    catch
                    {
                        // 破棄時のエラーは無視
                    }
                    package = null;
                }
                
                // メモリを強制的に解放
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private enum BillingTarget
        {
            Water,
            Gas,
            Electric
        }

        private async void BtnCreateWater_Click(object? sender, EventArgs e)
        {
            await RunBillingAsync(BillingTarget.Water, "水道");
        }

        private async void BtnCreateGas_Click(object? sender, EventArgs e)
        {
            await RunBillingAsync(BillingTarget.Gas, "ガス");
        }

        private async void BtnCreateElectric_Click(object? sender, EventArgs e)
        {
            await RunBillingAsync(BillingTarget.Electric, "電気");
        }

        private async Task RunBillingAsync(BillingTarget target, string label)
        {
            try
            {
                await RegisterBillingDetailsAsync(target);
                MessageBox.Show($"{label}の請求明細データ作成が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                var errorDetails = ex.ToString();
                try
                {
                    var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    var tempPath = Path.GetTempPath();
                    var logPath = Path.Combine(desktopPath, "billing_detail_error.log");
                    var tempLogPath = Path.Combine(tempPath, "billing_detail_error.log");
                    var logText = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{label}]\n{errorDetails}\n\n";

                    var logWrittenPath = string.Empty;
                    try
                    {
                        File.AppendAllText(logPath, logText);
                        logWrittenPath = logPath;
                    }
                    catch
                    {
                        File.AppendAllText(tempLogPath, logText);
                        logWrittenPath = tempLogPath;
                    }

                    MessageBox.Show(
                        $"[LOG-V3]{label}の請求明細データ作成中にエラーが発生しました。\n\n[詳細]\n{errorDetails}\n\nログ出力先: {logWrittenPath}\nアプリ実行フォルダ: {AppDomain.CurrentDomain.BaseDirectory}\nEXE: {System.Reflection.Assembly.GetExecutingAssembly().Location}",
                        "エラー(詳細)[LOG-V3]",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch
                {
                    MessageBox.Show(
                        $"[LOG-V3]{label}の請求明細データ作成中にエラーが発生しました。\n\n[詳細]\n{errorDetails}\n\nアプリ実行フォルダ: {AppDomain.CurrentDomain.BaseDirectory}\nEXE: {System.Reflection.Assembly.GetExecutingAssembly().Location}",
                        "エラー(詳細)[LOG-V3]",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private async Task RegisterBillingDetailsAsync(BillingTarget target)
        {
            var billingDate = new DateTime(dtpBillingYearMonth.Value.Year, dtpBillingYearMonth.Value.Month, 1);
            var billingYearMonth = billingDate.ToString("yyyy-MM");
            var prevMonthDate = billingDate.AddMonths(-1);
            var prevTwoMonthsDate = billingDate.AddMonths(-2);
            if (target == BillingTarget.Water)
            {
                var prevBillingYearMonth = prevMonthDate.ToString("yyyy-MM");
                var prevThreeMonthsDate = billingDate.AddMonths(-3);

                // 1. 水道料金請求データを取得（通常・服部は画面月、岩本ビル等は前月）
                var waterBillingsCurrent = await WaterBillingDataAccess.GetWaterBillingsByBillingYearMonthAsync(billingYearMonth);
                var waterBillingsPrev = await WaterBillingDataAccess.GetWaterBillingsByBillingYearMonthAsync(prevBillingYearMonth);

                foreach (var hattoriBilling in waterBillingsCurrent.Where(b => IsHattoriBuilding(b.BuildingName)))
                {
                    await RegisterHattoriWaterInvoiceDetailsAsync(
                        hattoriBilling,
                        billingDate,
                        prevTwoMonthsDate,
                        billingYearMonth);
                }

                var normalWaterBillings = DeduplicateWaterBillingsByParentMeter(
                    waterBillingsCurrent
                        .Where(b => !IsHattoriBuilding(b.BuildingName) && !IsLagMonthWaterBuilding(b.BuildingName))
                        .ToList());
                var lagWaterBillings = DeduplicateWaterBillingsByParentMeter(
                    waterBillingsPrev
                        .Where(b => IsLagMonthWaterBuilding(b.BuildingName))
                        .ToList());

                if (normalWaterBillings.Count > 0)
                {
                    await ProcessWaterBillingDetailsAsync(
                        normalWaterBillings,
                        billingDate,
                        billingYearMonth,
                        billingDate.Year,
                        billingDate.Month,
                        prevTwoMonthsDate.Year,
                        prevTwoMonthsDate.Month);
                }

                if (lagWaterBillings.Count > 0)
                {
                    await ProcessWaterBillingDetailsAsync(
                        lagWaterBillings,
                        billingDate,
                        billingYearMonth,
                        prevMonthDate.Year,
                        prevMonthDate.Month,
                        prevThreeMonthsDate.Year,
                        prevThreeMonthsDate.Month);
                }
            }

            if (target == BillingTarget.Gas)
            {
                // 1. 指定した請求年月と同じガス料金請求データを取得する
                var gasBillings = await GasBillingDataAccess.GetGasBillingsByBillingYearMonthAsync(billingYearMonth);
                // 親メーター×請求年月でユニーク化（同一親メーターの重複を排除）
                gasBillings = gasBillings
                    .GroupBy(b => b.ParentMeterId)
                    .Select(g => g.OrderByDescending(x => x.Id).First())
                    .ToList();
                if (gasBillings.Count > 0)
                {
                    var meters = await MeterDataAccess.GetAllMetersAsync();
                    var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
                    var buildingNameById = buildings.ToDictionary(b => b.Id, b => b.Name);

                    // 2. 取得したガス請求データと同じビル名の取引先データで請求先のものを取得してガス請求明細データとする
                    var gasBillingDetails = new List<GasBillingDetailResult>();
                    foreach (var billing in gasBillings)
                    {
                        var parentMeter = billing.ParentMeterId.HasValue
                            ? meters.FirstOrDefault(m => m.Id == billing.ParentMeterId.Value)
                            : null;
                        var buildingName = parentMeter?.BuildingId.HasValue == true
                            && buildingNameById.TryGetValue(parentMeter.BuildingId.Value, out var name)
                            ? name
                            : billing.BuildingName;
                        if (string.IsNullOrWhiteSpace(buildingName))
                        {
                            continue;
                        }

                        billing.BuildingName = buildingName;
                        var clients = await ClientDataAccess.GetClientsByBuildingNameAndIsBillingToAsync(buildingName);
                        foreach (var client in clients)
                        {
                            gasBillingDetails.Add(new GasBillingDetailResult
                            {
                                Billing = billing,
                                Client = client
                            });
                        }
                    }

                    if (gasBillingDetails.Count > 0)
                    {
                        var gasBuildingFloorsCache = new Dictionary<string, List<Floor>>();
                        var gasBuildingClientsCache = new Dictionary<string, List<Client>>();
                        var gasContractorClientCache = new Dictionary<int, Client?>();
                        var gasRoomChildMeters = await RoomChildMeterDataAccess.GetAllRoomChildMetersAsync();
                        var gasChildMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
                        var gasAllFloors = await FloorDataAccess.GetAllFloorsAsync();
                        var gasFloorById = gasAllFloors.ToDictionary(f => f.Id);

                        foreach (var detail in gasBillingDetails)
                        {
                            var buildingName = detail.Billing.BuildingName;
                            var roomName = detail.Client.RoomName;

                            if (!gasBuildingFloorsCache.TryGetValue(buildingName, out var floors))
                            {
                                floors = await FloorDataAccess.GetFloorsByBuildingNameAsync(buildingName);
                                gasBuildingFloorsCache[buildingName] = floors;
                            }

                            var targetChildMeters = gasChildMeters
                                .Where(cm => cm != null
                                    && cm.MeterType == "ガス"
                                    && (!detail.Billing.ParentMeterId.HasValue || cm.ParentMeterId == detail.Billing.ParentMeterId))
                                .ToList();
                            var gasChildMeterNameById = targetChildMeters
                                .GroupBy(cm => cm.Id)
                                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.MeterName ?? string.Empty);

                            var roomFloor = floors.FirstOrDefault(f => f.FloorName == roomName);
                            var roomChildMeter = roomFloor != null
                                ? gasRoomChildMeters.FirstOrDefault(rcm => rcm.FloorId == roomFloor.Id)
                                : null;
                            var roomChildMeterInfo = roomChildMeter != null
                                ? targetChildMeters.FirstOrDefault(cm => cm.Id == roomChildMeter.ChildMeterId)
                                : null;

                            // 3. ガス請求明細データの子メーター名と同じガス子メーター検針データで検針日の年月が請求年月の当月、前月のデータを取得する
                            var targetChildMeterName = roomChildMeterInfo?.MeterName?.Trim() ?? string.Empty;
                            var roomReadings = !string.IsNullOrWhiteSpace(targetChildMeterName)
                                ? await ChildMeterReadingDataAccess.GetGasChildMeterReadingsByChildMeterNameAndMonthsAsync(
                                    targetChildMeterName,
                                    billingDate.Year,
                                    billingDate.Month,
                                    prevMonthDate.Year,
                                    prevMonthDate.Month)
                                : new List<(ChildMeterReading Reading, string BuildingName, string RoomName)>();
                            if (!string.IsNullOrWhiteSpace(targetChildMeterName))
                            {
                                roomReadings = roomReadings
                                    .Where(r =>
                                    {
                                        if (!r.Reading.ChildMeterId.HasValue)
                                        {
                                            return false;
                                        }

                                        return gasChildMeterNameById.TryGetValue(r.Reading.ChildMeterId.Value, out var meterName)
                                            && string.Equals(meterName, targetChildMeterName, StringComparison.OrdinalIgnoreCase);
                                    })
                                    .ToList();
                            }
                            else
                            {
                                roomReadings = new List<(ChildMeterReading Reading, string BuildingName, string RoomName)>();
                            }

                            var currentRoomReading = GetLatestReadingForMonth(roomReadings.Select(r => r.Reading), billingDate.Year, billingDate.Month);
                            var prevRoomReading = GetLatestReadingForMonth(roomReadings.Select(r => r.Reading), prevMonthDate.Year, prevMonthDate.Month);
                            var currentRoomValue = currentRoomReading?.MeterValue ?? 0;
                            var prevRoomValue = prevRoomReading?.MeterValue ?? 0;

                            // 4. ガス請求明細データの当月データのメーター値から前月データのメーター値を引いた値を計算して部屋別使用量とする
                            detail.RoomUsage = currentRoomValue - prevRoomValue;

                            // 5. ガス請求明細データの子メーター名と同じ部屋別子メーター管理データの部屋面積合計を専有面積合計とする
                            if (!string.IsNullOrWhiteSpace(targetChildMeterName))
                            {
                                var targetChildMeterIds = targetChildMeters
                                    .Where(cm => string.Equals(cm.MeterName, targetChildMeterName, StringComparison.OrdinalIgnoreCase))
                                    .Select(cm => cm.Id)
                                    .ToHashSet();
                                detail.TotalArea = gasRoomChildMeters
                                    .Where(rcm => targetChildMeterIds.Contains(rcm.ChildMeterId))
                                    .Select(rcm => gasFloorById.TryGetValue(rcm.FloorId, out var floor) ? floor.FloorArea : 0)
                                    .Sum();
                            }
                            else
                            {
                                detail.TotalArea = 0;
                            }

                            detail.RoomArea = roomFloor?.FloorArea ?? 0;

                            // 6. ガス請求明細データの部屋面積÷専有面積合計を計算して部屋別専有割合とする
                            detail.RoomShareRatio = detail.TotalArea > 0 ? detail.RoomArea / detail.TotalArea : 0;

                            // 7. ガス請求データの基本料金×部屋別専有割合を計算して部屋別基本料金とする
                            detail.RoomBasicCharge = detail.Billing.BasicCharge * detail.RoomShareRatio;

                            // 8. ガス請求明細データの部屋別使用量×部屋別専有割合を部屋別使用量計とする
                            detail.RoomUsageTotal = detail.RoomUsage * detail.RoomShareRatio;

                            // 9. ガス請求データの使用料金÷部屋別使用量を部屋別使用金額単価とする
                            detail.RoomUnitPrice = detail.RoomUsage != 0 ? detail.Billing.UsageCharge / detail.RoomUsage : 0;

                            // 10. ガス請求明細データの部屋別使用金額単価×部屋別使用量計を部屋別使用金額とする
                            detail.RoomUsageCharge = detail.RoomUnitPrice * detail.RoomUsageTotal;

                            // 11. 部屋別基本料金＋部屋別使用金額を部屋別ガス料金とする
                            detail.RoomGasCharge = detail.RoomBasicCharge + detail.RoomUsageCharge;

                            if (!gasBuildingClientsCache.TryGetValue(buildingName, out var buildingClients))
                            {
                                buildingClients = await ClientDataAccess.GetClientsByBuildingNameAsync(buildingName);
                                gasBuildingClientsCache[buildingName] = buildingClients;
                            }

                            var lessorName = buildingClients.FirstOrDefault(c => c.IsLessor)?.Name ?? string.Empty;
                            var lesseeName = buildingClients
                                .FirstOrDefault(c => c.IsLessee && string.Equals(c.RoomName, roomName, StringComparison.OrdinalIgnoreCase))
                                ?.Name ?? string.Empty;
                            Client? contractorClientGas = null;
                            if (detail.Billing.ContractorId.HasValue)
                            {
                                var contractorIdGas = detail.Billing.ContractorId.Value;
                                if (!gasContractorClientCache.TryGetValue(contractorIdGas, out contractorClientGas))
                                {
                                    contractorClientGas = await ClientDataAccess.GetClientByIdAsync(contractorIdGas);
                                    gasContractorClientCache[contractorIdGas] = contractorClientGas;
                                }
                            }
                            contractorClientGas ??= buildingClients.FirstOrDefault(c => c.IsContractor);
                            var contractorName = contractorClientGas?.Name ?? string.Empty;
                            var contractorInvoiceNumberGas = contractorClientGas?.InvoiceNumber ?? string.Empty;

                            // データ取得時にそれぞれの項目をガス請求明細データに登録する
                            // 子使用量＝子メーターの使用量（按分前）。複数部屋共有時は同じメーターの合計が各室に表示される
                            detail.InvoiceDetail = new InvoiceDetail
                            {
                                BillingTo = detail.Client.Name,
                                Lessor = lessorName,
                                BuildingName = buildingName,
                                Lessee = lesseeName,
                                RoomNumber = roomName,
                                RoomArea = detail.RoomArea,
                                Category = "ガス",
                                Content = "ガス料金",
                                ChildMeterUsage = detail.RoomUsage,
                                UsageAmount = detail.RoomUsageTotal,
                                Unit = "m3",
                                Contractor = contractorName,
                                InvoiceNumber = contractorInvoiceNumberGas,
                                ChildMeterStartDate = prevRoomReading?.ReadingDate.AddDays(-1),
                                ChildMeterEndDate = currentRoomReading?.ReadingDate,
                                ParentMeterStartDate = detail.Billing.StartDate,
                                ParentMeterEndDate = detail.Billing.EndDate,
                                TaxInclusiveAmount = detail.RoomGasCharge,
                                TaxRate = detail.Billing.TaxRate,
                                BillingYearMonth = billingYearMonth,
                                ConfirmedBillingDate = GetPlannedBillingDateByTransferMethod(billingDate, detail.Client)
                            };
                        }

                        foreach (var detail in gasBillingDetails)
                        {
                            if (detail.InvoiceDetail.TaxInclusiveAmount == 0)
                            {
                                continue;
                            }

                            await InvoiceDetailDataAccess.CreateInvoiceDetailAsync(detail.InvoiceDetail);
                        }
                    }
                }
            }

            if (target == BillingTarget.Electric)
            {
                // 1. 指定した請求年月と同じ電気料金請求データを取得する
                var electricBillings = await ElectricBillingDataAccess.GetElectricBillingsByBillingYearMonthAsync(billingYearMonth);
                if (electricBillings.Count == 0)
                {
                    return;
                }

                // 2. 取得した電気請求データと同じビル名の取引先データで請求先のものを取得して電気請求明細データとする
                var billingDetails = new List<ElectricBillingDetailResult>();
                foreach (var billing in electricBillings)
                {
                    if (string.IsNullOrWhiteSpace(billing.BuildingName))
                    {
                        continue;
                    }

                    if (IsElectricBillingExcludedBuilding(billing.BuildingName))
                    {
                        continue;
                    }

                    var clients = await ClientDataAccess.GetClientsByBuildingNameAndIsBillingToAsync(billing.BuildingName);
                    foreach (var client in clients)
                    {
                        billingDetails.Add(new ElectricBillingDetailResult
                        {
                            Billing = billing,
                            Client = client
                        });
                    }
                }

                if (billingDetails.Count == 0)
                {
                    return;
                }

                var buildingFloorsCache = new Dictionary<string, List<Floor>>();
                var buildingChildReadingsCache = new Dictionary<string, List<ChildMeterReading>>();
                var buildingClientsCache = new Dictionary<string, List<Client>>();
                var contractorClientCache = new Dictionary<int, Client?>();

                foreach (var detail in billingDetails)
                {
                    var buildingName = detail.Billing.BuildingName;
                    var roomName = detail.Client.RoomName;

                    // 3. 電気請求明細データと同じビル名、部屋名の電気子メーター検針データで検針日が請求年月の当月、前月のデータを取得する
                    var roomReadings = await ChildMeterReadingDataAccess.GetElectricChildMeterReadingsByBuildingRoomAndMonthsAsync(
                        buildingName,
                        roomName,
                        billingDate.Year,
                        billingDate.Month,
                        prevMonthDate.Year,
                        prevMonthDate.Month);

                    var currentRoomReading = GetLatestReadingForMonth(roomReadings.Select(r => r.Reading), billingDate.Year, billingDate.Month);
                    var prevRoomReading = GetLatestReadingForMonth(roomReadings.Select(r => r.Reading), prevMonthDate.Year, prevMonthDate.Month);
                    var currentRoomValue = currentRoomReading?.MeterValue ?? 0;
                    var prevRoomValue = prevRoomReading?.MeterValue ?? 0;

                    // 4. 電気請求明細データの当月データのメーター値から前月データのメーター値を引いた値を計算して部屋別使用量とする
                    detail.RoomUsage = currentRoomValue - prevRoomValue;

                    if (!buildingClientsCache.TryGetValue(buildingName, out var buildingClients))
                    {
                        buildingClients = await ClientDataAccess.GetClientsByBuildingNameAsync(buildingName);
                        buildingClientsCache[buildingName] = buildingClients;
                    }

                    var lessorName = buildingClients.FirstOrDefault(c => c.IsLessor)?.Name ?? string.Empty;
                    var lesseeName = buildingClients
                        .FirstOrDefault(c => c.IsLessee && string.Equals(c.RoomName, roomName, StringComparison.OrdinalIgnoreCase))
                        ?.Name ?? string.Empty;
                    Client? contractorClient = null;
                    if (detail.Billing.ContractorId.HasValue)
                    {
                        var contractorId = detail.Billing.ContractorId.Value;
                        if (!contractorClientCache.TryGetValue(contractorId, out contractorClient))
                        {
                            contractorClient = await ClientDataAccess.GetClientByIdAsync(contractorId);
                            contractorClientCache[contractorId] = contractorClient;
                        }
                    }
                    contractorClient ??= buildingClients.FirstOrDefault(c => c.IsContractor);
                    var contractorName = contractorClient?.Name ?? string.Empty;
                    var contractorInvoiceNumber = contractorClient?.InvoiceNumber ?? string.Empty;

                    if (IsHattoriBuilding(buildingName))
                    {
                        // 服部ビル: 面積・共有按分は行わず、親請求の使用量で基本料・使用料金を按分する
                        if (!buildingFloorsCache.TryGetValue(buildingName, out var hattoriFloors))
                        {
                            hattoriFloors = await FloorDataAccess.GetFloorsByBuildingNameAsync(buildingName);
                            buildingFloorsCache[buildingName] = hattoriFloors;
                        }

                        var roomFloor = hattoriFloors.FirstOrDefault(f => f.FloorName == roomName);
                        detail.RoomArea = roomFloor?.FloorArea ?? 0;
                        detail.RoomUsageTotal = detail.RoomUsage;

                        var parentUsage = detail.Billing.UsageAmount;
                        if (parentUsage != 0)
                        {
                            detail.RoomBasicCharge = detail.RoomUsage * (detail.Billing.BasicCharge / parentUsage);
                            detail.RoomPowerCharge = detail.RoomUsage * (detail.Billing.PowerCharge / parentUsage);
                        }
                        else
                        {
                            detail.RoomBasicCharge = 0;
                            detail.RoomPowerCharge = 0;
                        }

                        detail.RoomElectricCharge = detail.RoomBasicCharge + detail.RoomPowerCharge;

                        detail.InvoiceDetail = new InvoiceDetail
                        {
                            BillingTo = detail.Client.Name,
                            Lessor = lessorName,
                            BuildingName = buildingName,
                            Lessee = lesseeName,
                            RoomNumber = roomName,
                            RoomArea = detail.RoomArea,
                            Category = "電気",
                            ChildMeterUsage = detail.RoomUsage,
                            UsageAmount = null,
                            Unit = string.Empty,
                            Contractor = contractorName,
                            InvoiceNumber = contractorInvoiceNumber,
                            ChildMeterStartDate = prevRoomReading?.ReadingDate.AddDays(-1),
                            ChildMeterEndDate = currentRoomReading?.ReadingDate,
                            ParentMeterStartDate = detail.Billing.StartDate,
                            ParentMeterEndDate = detail.Billing.EndDate,
                            BillingYearMonth = billingYearMonth,
                            ConfirmedBillingDate = GetPlannedBillingDateByTransferMethod(billingDate, detail.Client)
                        };

                        continue;
                    }

                    if (!buildingFloorsCache.TryGetValue(buildingName, out var floors))
                    {
                        floors = await FloorDataAccess.GetFloorsByBuildingNameAsync(buildingName);
                        buildingFloorsCache[buildingName] = floors;
                    }

                    // 5. 電気請求明細データのビル名を同じ部屋データを取得して面積の合計を計算して専有面積合計とする
                    detail.TotalArea = floors.Sum(f => f.FloorArea);

                    var roomFloorDefault = floors.FirstOrDefault(f => f.FloorName == roomName);
                    detail.RoomArea = roomFloorDefault?.FloorArea ?? 0;

                    // 6. 電気請求明細データの部屋面積÷専有面積合計を計算して部屋別共有割合とする
                    detail.RoomShareRatio = detail.TotalArea > 0 ? detail.RoomArea / detail.TotalArea : 0;

                    // 7. 電気請求明細データの基本料金×部屋別共有割合を計算して部屋別基本料金とする
                    detail.RoomBasicCharge = detail.Billing.BasicCharge * detail.RoomShareRatio;

                    if (!buildingChildReadingsCache.TryGetValue(buildingName, out var buildingReadings))
                    {
                        buildingReadings = await ChildMeterReadingDataAccess.GetElectricChildMeterReadingsByBuildingAndMonthsAsync(
                            buildingName,
                            billingDate.Year,
                            billingDate.Month,
                            prevMonthDate.Year,
                            prevMonthDate.Month);
                        buildingChildReadingsCache[buildingName] = buildingReadings;
                    }

                    // 8. 電気請求明細データと同じビル名の電気子メーター検針データで検針日が請求年月の当月、前月のデータを取得する
                    var currentBuildingTotal = GetTotalMeterValueForMonth(buildingReadings, billingDate.Year, billingDate.Month);
                    var prevBuildingTotal = GetTotalMeterValueForMonth(buildingReadings, prevMonthDate.Year, prevMonthDate.Month);

                    // 9. 取得した当月データのメーター値の合計から前月データのメーター値の合計を引いた値を計算して子メーター使用量合計とする
                    detail.ChildMeterUsageTotal = currentBuildingTotal - prevBuildingTotal;

                    // 10. 取得した電気請求データの使用量－子メーター使用量合計を計算して共有部分使用量合計とする
                    detail.SharedUsageTotal = detail.Billing.UsageAmount - detail.ChildMeterUsageTotal;

                    // 11. 電気請求明細データの部屋別共有割合×共有部分使用量合計を計算して共有部分使用量とする
                    detail.SharedUsage = detail.RoomShareRatio * detail.SharedUsageTotal;

                    // 12. 電気請求明細データの部屋別使用量＋共有部分使用量を計算して部屋別使用量計とする
                    detail.RoomUsageTotal = detail.RoomUsage + detail.SharedUsage;

                    // データ取得時にそれぞれの項目を電気請求明細データに登録する
                    detail.InvoiceDetail = new InvoiceDetail
                    {
                        BillingTo = detail.Client.Name,
                        Lessor = lessorName,
                        BuildingName = buildingName,
                        Lessee = lesseeName,
                        RoomNumber = roomName,
                        RoomArea = detail.RoomArea,
                        Category = "電気",
                        ChildMeterUsage = detail.RoomUsage,
                        UsageAmount = null,
                        Unit = string.Empty,
                        Contractor = contractorName,
                        InvoiceNumber = contractorInvoiceNumber,
                        ChildMeterStartDate = prevRoomReading?.ReadingDate.AddDays(-1),
                        ChildMeterEndDate = currentRoomReading?.ReadingDate,
                        ParentMeterStartDate = detail.Billing.StartDate,
                        ParentMeterEndDate = detail.Billing.EndDate,
                        BillingYearMonth = billingYearMonth
                    };
                }

                var totalRoomUsageByBuilding = billingDetails
                    .Where(d => !IsHattoriBuilding(d.Billing.BuildingName))
                    .GroupBy(d => d.Billing.BuildingName)
                    .ToDictionary(g => g.Key, g => g.Sum(d => d.RoomUsageTotal));

                var electricInvoiceDetails = new List<InvoiceDetail>();
                foreach (var detail in billingDetails)
                {
                    if (!IsHattoriBuilding(detail.Billing.BuildingName))
                    {
                        // 13. 電気請求明細データの使用料金÷部屋別使用量合計を計算して部屋別従量単価とする
                        var totalRoomUsage = totalRoomUsageByBuilding.TryGetValue(detail.Billing.BuildingName, out var totalUsage)
                            ? totalUsage
                            : 0;
                        detail.RoomUnitPrice = totalRoomUsage != 0 ? detail.Billing.PowerCharge / totalRoomUsage : 0;

                        // 14. 電気請求明細データの部屋別従量単価×部屋別使用量計を計算して部屋別従量金額とする
                        detail.RoomPowerCharge = detail.RoomUnitPrice * detail.RoomUsageTotal;

                        // 15. 電気請求明細データの部屋別基本料金＋部屋別従量金額を計算して部屋別電気料金とする
                        detail.RoomElectricCharge = detail.RoomBasicCharge + detail.RoomPowerCharge;
                    }

                    var baseDetail = detail.InvoiceDetail;

                    var basicDetail = new InvoiceDetail
                    {
                        BillingTo = baseDetail.BillingTo,
                        Lessor = baseDetail.Lessor,
                        BuildingName = baseDetail.BuildingName,
                        Lessee = baseDetail.Lessee,
                        RoomNumber = baseDetail.RoomNumber,
                        RoomArea = null,
                        Category = baseDetail.Category,
                        Content = "電気料金（基本料）",
                        ChildMeterUsage = null,
                        UsageAmount = null,
                        Unit = string.Empty,
                        TaxInclusiveAmount = detail.RoomBasicCharge,
                        TaxRate = detail.Billing.TaxRate,
                        Contractor = baseDetail.Contractor,
                        InvoiceNumber = baseDetail.InvoiceNumber,
                        ChildMeterStartDate = baseDetail.ChildMeterStartDate,
                        ChildMeterEndDate = baseDetail.ChildMeterEndDate,
                        ParentMeterStartDate = baseDetail.ParentMeterStartDate,
                        ParentMeterEndDate = baseDetail.ParentMeterEndDate,
                        BillingYearMonth = billingYearMonth,
                        ConfirmedBillingDate = GetPlannedBillingDateByTransferMethod(billingDate, detail.Client)
                    };

                    var usageDetail = new InvoiceDetail
                    {
                        BillingTo = baseDetail.BillingTo,
                        Lessor = baseDetail.Lessor,
                        BuildingName = baseDetail.BuildingName,
                        Lessee = baseDetail.Lessee,
                        RoomNumber = baseDetail.RoomNumber,
                        RoomArea = baseDetail.RoomArea,
                        Category = baseDetail.Category,
                        Content = "電気料金（従量電気料）",
                        ChildMeterUsage = baseDetail.ChildMeterUsage,
                        UsageAmount = detail.RoomUsageTotal,
                        Unit = "kWh",
                        TaxInclusiveAmount = detail.RoomPowerCharge,
                        TaxRate = detail.Billing.TaxRate,
                        Contractor = baseDetail.Contractor,
                        InvoiceNumber = baseDetail.InvoiceNumber,
                        ChildMeterStartDate = baseDetail.ChildMeterStartDate,
                        ChildMeterEndDate = baseDetail.ChildMeterEndDate,
                        ParentMeterStartDate = baseDetail.ParentMeterStartDate,
                        ParentMeterEndDate = baseDetail.ParentMeterEndDate,
                        BillingYearMonth = billingYearMonth,
                        ConfirmedBillingDate = GetPlannedBillingDateByTransferMethod(billingDate, detail.Client)
                    };

                    electricInvoiceDetails.Add(basicDetail);
                    electricInvoiceDetails.Add(usageDetail);
                }

                // 電気請求明細データを水道光熱費請求明細テーブルに登録する
                foreach (var detail in electricInvoiceDetails)
                {
                    if (detail.TaxInclusiveAmount == 0)
                    {
                        continue;
                    }

                    await InvoiceDetailDataAccess.CreateInvoiceDetailAsync(detail);
                }
            }
        }

        private static decimal GetLatestMeterValueForMonth(IEnumerable<ChildMeterReading> readings, int year, int month)
        {
            var reading = readings.FirstOrDefault(r => r.ReadingDate.Year == year && r.ReadingDate.Month == month);
            return reading?.MeterValue ?? 0;
        }

        private static ChildMeterReading? GetLatestReadingForMonth(IEnumerable<ChildMeterReading> readings, int year, int month)
        {
            return readings
                .Where(r => r.ReadingDate.Year == year && r.ReadingDate.Month == month)
                .OrderByDescending(r => r.ReadingDate)
                .ThenByDescending(r => r.Id)
                .FirstOrDefault();
        }

        private static string FormatDate(DateTime? date)
        {
            return date?.ToString("yyyy-MM-dd") ?? string.Empty;
        }

        private static decimal GetTotalMeterValueForMonth(IEnumerable<ChildMeterReading> readings, int year, int month)
        {
            return readings
                .Where(r => r.ReadingDate.Year == year && r.ReadingDate.Month == month)
                .Sum(r => r.MeterValue);
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private static bool IsLagMonthWaterBuilding(string? buildingName)
        {
            var trimmed = buildingName?.Trim() ?? string.Empty;
            return LagMonthWaterBuildingNames.Any(name =>
                string.Equals(trimmed, name, StringComparison.Ordinal));
        }

        private static List<WaterBilling> DeduplicateWaterBillingsByParentMeter(List<WaterBilling> billings) =>
            billings
                .GroupBy(b => b.ParentMeterId ?? -b.Id)
                .Select(g => g.OrderByDescending(x => x.Id).First())
                .ToList();

        private static string ResolveWaterBillingBuildingName(
            WaterBilling billing,
            IReadOnlyList<Meter> meters,
            IReadOnlyDictionary<int, string> buildingNameById)
        {
            if (!string.IsNullOrWhiteSpace(billing.BuildingName))
            {
                return billing.BuildingName.Trim();
            }

            if (!billing.ParentMeterId.HasValue)
            {
                return string.Empty;
            }

            var parentMeter = meters.FirstOrDefault(m => m.Id == billing.ParentMeterId.Value);
            if (parentMeter?.BuildingId.HasValue == true
                && buildingNameById.TryGetValue(parentMeter.BuildingId.Value, out var buildingName))
            {
                return buildingName;
            }

            return string.Empty;
        }

        private static bool IsPlazaCeresse37FRawUsageWaterBilling(WaterBilling billing, IReadOnlyList<Meter> meters)
        {
            if (!billing.ParentMeterId.HasValue)
            {
                return false;
            }

            var parentMeter = meters.FirstOrDefault(m => m.Id == billing.ParentMeterId.Value);
            return string.Equals(
                parentMeter?.MeterName?.Trim(),
                PlazaCeresse37FWaterParentMeterName,
                StringComparison.Ordinal);
        }

        private static (decimal Usage, DateTime? StartReadingDate, DateTime? EndReadingDate) GetFloorWaterUsageFromReadings(
            IEnumerable<ChildMeterReading> readings,
            int floorId,
            int endYear,
            int endMonth,
            int startYear,
            int startMonth)
        {
            var floorReadings = readings.Where(r => r.FloorId == floorId).ToList();
            var endReading = GetLatestReadingForMonth(floorReadings, endYear, endMonth);
            var startReading = GetLatestReadingForMonth(floorReadings, startYear, startMonth);
            var usage = (endReading?.MeterValue ?? 0) - (startReading?.MeterValue ?? 0);
            return (usage, startReading?.ReadingDate, endReading?.ReadingDate);
        }

        /// <summary>
        /// 水道請求明細を按分作成する。子メーター検針の終了月・開始月は呼び出し側で指定する。
        /// </summary>
        private async Task ProcessWaterBillingDetailsAsync(
            List<WaterBilling> waterBillingsSource,
            DateTime billingDate,
            string billingYearMonth,
            int childMeterEndYear,
            int childMeterEndMonth,
            int childMeterStartYear,
            int childMeterStartMonth)
        {
            var meters = await MeterDataAccess.GetAllMetersAsync();
            var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
            var buildingNameById = buildings.ToDictionary(b => b.Id, b => b.Name);
            var roomChildMeters = await RoomChildMeterDataAccess.GetAllRoomChildMetersAsync();
            var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
            var allFloors = await FloorDataAccess.GetAllFloorsAsync();
            var floorById = allFloors.ToDictionary(f => f.Id);
            var childMeterById = childMeters
                .Where(cm => cm != null)
                .GroupBy(cm => cm.Id)
                .ToDictionary(g => g.Key, g => g.First());

            var waterBillingDetails = new List<WaterBillingDetailResult>();
            foreach (var billing in waterBillingsSource)
            {
                var buildingName = ResolveWaterBillingBuildingName(billing, meters, buildingNameById);
                if (string.IsNullOrWhiteSpace(buildingName))
                {
                    continue;
                }

                billing.BuildingName = buildingName;
                var floors = await FloorDataAccess.GetFloorsByBuildingNameAsync(buildingName);
                var targetChildMeterIds = childMeters
                    .Where(cm => cm != null
                        && cm.MeterType == "水道"
                        && (!billing.ParentMeterId.HasValue || cm.ParentMeterId == billing.ParentMeterId))
                    .Select(cm => cm.Id)
                    .ToHashSet();
                var targetFloorIds = roomChildMeters
                    .Where(rcm => targetChildMeterIds.Contains(rcm.ChildMeterId))
                    .Select(rcm => rcm.FloorId)
                    .ToHashSet();

                var isPlazaCeresse37F = IsPlazaCeresse37FRawUsageWaterBilling(billing, meters);
                var clients = await ClientDataAccess.GetClientsByBuildingNameAsync(buildingName);
                foreach (var client in clients.Where(c => c.IsBillingTo))
                {
                    var roomName = client.RoomName ?? string.Empty;
                    var isDifferenceAssignmentRoom = !isPlazaCeresse37F
                        && !string.IsNullOrWhiteSpace(billing.DifferenceAssignmentRoomName)
                        && string.Equals(billing.DifferenceAssignmentRoomName.Trim(), roomName, StringComparison.OrdinalIgnoreCase);

                    if (billing.ParentMeterId.HasValue)
                    {
                        var roomFloor = floors.FirstOrDefault(f => f.FloorName == roomName);
                        if (!isDifferenceAssignmentRoom
                            && (roomFloor == null || !targetFloorIds.Contains(roomFloor.Id)))
                        {
                            continue;
                        }
                    }

                    waterBillingDetails.Add(new WaterBillingDetailResult
                    {
                        Billing = billing,
                        Client = client
                    });
                }
            }

            if (waterBillingDetails.Count == 0)
            {
                return;
            }

            var waterBuildingFloorsCache = new Dictionary<string, List<Floor>>();
            var waterBuildingClientsCache = new Dictionary<string, List<Client>>();
            var waterContractorClientCache = new Dictionary<int, Client?>();
            var waterBillingFloorIdsCache = new Dictionary<int, HashSet<int>>();
            var waterChildMeterUsageCache = new Dictionary<(int BillingId, int ChildMeterId, int EndYear, int EndMonth, int StartYear, int StartMonth), (decimal Usage, decimal TotalArea, DateTime? StartReadingDate, DateTime? EndReadingDate)>();
            var waterFloorUsageCache = new Dictionary<(int BillingId, int FloorId, int ChildMeterId, int EndYear, int EndMonth, int StartYear, int StartMonth), (decimal Usage, DateTime? StartReadingDate, DateTime? EndReadingDate)>();

            var billingRoomNamesByBillingId = waterBillingDetails
                .GroupBy(d => d.Billing.Id)
                .ToDictionary(
                    g => g.Key,
                    g => new HashSet<string>(
                        g.Select(d => d.Client.RoomName ?? string.Empty),
                        StringComparer.OrdinalIgnoreCase));

            foreach (var detail in waterBillingDetails)
            {
                var buildingName = detail.Billing.BuildingName;
                var roomName = detail.Client.RoomName;

                if (!waterBuildingFloorsCache.TryGetValue(buildingName, out var floors))
                {
                    floors = await FloorDataAccess.GetFloorsByBuildingNameAsync(buildingName);
                    waterBuildingFloorsCache[buildingName] = floors;
                }

                if (!waterBillingFloorIdsCache.TryGetValue(detail.Billing.Id, out var billingFloorIds))
                {
                    var billingRoomNames = billingRoomNamesByBillingId.TryGetValue(detail.Billing.Id, out var set)
                        ? set
                        : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    billingFloorIds = floors
                        .Where(f => billingRoomNames.Contains(f.FloorName ?? string.Empty))
                        .Select(f => f.Id)
                        .ToHashSet();
                    waterBillingFloorIdsCache[detail.Billing.Id] = billingFloorIds;
                }

                var roomFloor = floors.FirstOrDefault(f => f.FloorName == roomName);
                var roomChildMeter = roomFloor != null
                    ? roomChildMeters.FirstOrDefault(rcm =>
                        rcm.FloorId == roomFloor.Id
                        && childMeterById.TryGetValue(rcm.ChildMeterId, out var cm)
                        && cm.MeterType == "水道"
                        && (!detail.Billing.ParentMeterId.HasValue || cm.ParentMeterId == detail.Billing.ParentMeterId))
                    : null;
                var isPlazaCeresse37FDetail = IsPlazaCeresse37FRawUsageWaterBilling(detail.Billing, meters);
                var isDifferenceAssignmentRoom = !isPlazaCeresse37FDetail
                    && !string.IsNullOrWhiteSpace(detail.Billing.DifferenceAssignmentRoomName)
                    && string.Equals((detail.Billing.DifferenceAssignmentRoomName ?? string.Empty).Trim(), roomName ?? string.Empty, StringComparison.OrdinalIgnoreCase);

                if (roomChildMeter == null || roomChildMeter.ChildMeterId <= 0)
                {
                    if (!isDifferenceAssignmentRoom)
                    {
                        continue;
                    }
                    detail.RoomArea = roomFloor?.FloorArea ?? 0;
                    var totalAreaBillingRooms = floors
                        .Where(f => billingFloorIds.Contains(f.Id))
                        .Sum(f => f.FloorArea);
                    detail.TotalArea = totalAreaBillingRooms;
                    detail.RoomShareRatio = totalAreaBillingRooms > 0 ? detail.RoomArea / totalAreaBillingRooms : 0;
                    detail.RoomUsage = 0;
                    detail.RoomUsageTotal = 0;
                    if (!waterBuildingClientsCache.TryGetValue(buildingName, out var buildingClientsForDiff))
                    {
                        buildingClientsForDiff = await ClientDataAccess.GetClientsByBuildingNameAsync(buildingName);
                        waterBuildingClientsCache[buildingName] = buildingClientsForDiff;
                    }
                    var lessorNameDiff = buildingClientsForDiff.FirstOrDefault(c => c.IsLessor)?.Name ?? string.Empty;
                    var lesseeNameDiff = buildingClientsForDiff
                        .FirstOrDefault(c => c.IsLessee && string.Equals(c.RoomName, roomName, StringComparison.OrdinalIgnoreCase))
                        ?.Name ?? string.Empty;
                    Client? contractorClientDiff = null;
                    if (detail.Billing.ContractorId.HasValue)
                    {
                        var contractorIdDiff = detail.Billing.ContractorId.Value;
                        if (!waterContractorClientCache.TryGetValue(contractorIdDiff, out contractorClientDiff))
                        {
                            contractorClientDiff = await ClientDataAccess.GetClientByIdAsync(contractorIdDiff);
                            waterContractorClientCache[contractorIdDiff] = contractorClientDiff;
                        }
                    }
                    contractorClientDiff ??= buildingClientsForDiff.FirstOrDefault(c => c.IsContractor);
                    var contractorNameDiff = contractorClientDiff?.Name ?? string.Empty;
                    var contractorInvoiceNumberDiff = contractorClientDiff?.InvoiceNumber ?? string.Empty;
                    detail.InvoiceDetail = new InvoiceDetail
                    {
                        BillingTo = detail.Client.Name,
                        Lessor = lessorNameDiff,
                        BuildingName = buildingName,
                        Lessee = lesseeNameDiff,
                        RoomNumber = roomName,
                        RoomArea = detail.RoomArea,
                        Category = "水道",
                        Content = "水道料金",
                        ChildMeterUsage = 0,
                        UsageAmount = 0,
                        Unit = "m3",
                        Contractor = contractorNameDiff,
                        InvoiceNumber = contractorInvoiceNumberDiff,
                        ParentMeterStartDate = detail.Billing.StartDate,
                        ParentMeterEndDate = detail.Billing.EndDate,
                        BillingYearMonth = billingYearMonth
                    };
                    continue;
                }

                var childMeterId = roomChildMeter.ChildMeterId;
                decimal floorUsage;
                DateTime? childMeterStartDate;
                DateTime? childMeterEndDate;
                decimal totalArea;
                if (isPlazaCeresse37FDetail && roomFloor != null)
                {
                    var floorUsageCacheKey = (
                        detail.Billing.Id,
                        roomFloor.Id,
                        childMeterId,
                        childMeterEndYear,
                        childMeterEndMonth,
                        childMeterStartYear,
                        childMeterStartMonth);
                    if (!waterFloorUsageCache.TryGetValue(floorUsageCacheKey, out var floorUsageInfo))
                    {
                        var readings = await ChildMeterReadingDataAccess.GetWaterChildMeterReadingsByChildMeterIdAndMonthsAsync(
                            childMeterId,
                            childMeterEndYear,
                            childMeterEndMonth,
                            childMeterStartYear,
                            childMeterStartMonth);
                        floorUsageInfo = GetFloorWaterUsageFromReadings(
                            readings,
                            roomFloor.Id,
                            childMeterEndYear,
                            childMeterEndMonth,
                            childMeterStartYear,
                            childMeterStartMonth);
                        waterFloorUsageCache[floorUsageCacheKey] = floorUsageInfo;
                    }

                    floorUsage = floorUsageInfo.Usage;
                    childMeterStartDate = floorUsageInfo.StartReadingDate;
                    childMeterEndDate = floorUsageInfo.EndReadingDate;
                    totalArea = 0;
                }
                else
                {
                    var usageCacheKey = (detail.Billing.Id, childMeterId, childMeterEndYear, childMeterEndMonth, childMeterStartYear, childMeterStartMonth);
                    if (!waterChildMeterUsageCache.TryGetValue(usageCacheKey, out var usageInfo))
                    {
                        var readings = await ChildMeterReadingDataAccess.GetWaterChildMeterReadingsByChildMeterIdAndMonthsAsync(
                            childMeterId,
                            childMeterEndYear,
                            childMeterEndMonth,
                            childMeterStartYear,
                            childMeterStartMonth);
                        var endReading = GetLatestReadingForMonth(readings, childMeterEndYear, childMeterEndMonth);
                        var startReading = GetLatestReadingForMonth(readings, childMeterStartYear, childMeterStartMonth);
                        var endValue = endReading?.MeterValue ?? 0;
                        var startValue = startReading?.MeterValue ?? 0;
                        var roomUsage = endValue - startValue;
                        totalArea = roomChildMeters
                            .Where(rcm => rcm.ChildMeterId == childMeterId && billingFloorIds.Contains(rcm.FloorId))
                            .Select(rcm => rcm.FloorId)
                            .Distinct()
                            .Select(id => floorById.TryGetValue(id, out var floor) ? floor.FloorArea : 0)
                            .Sum();
                        usageInfo = (roomUsage, totalArea, startReading?.ReadingDate, endReading?.ReadingDate);
                        waterChildMeterUsageCache[usageCacheKey] = usageInfo;
                    }

                    floorUsage = usageInfo.Usage;
                    childMeterStartDate = usageInfo.StartReadingDate;
                    childMeterEndDate = usageInfo.EndReadingDate;
                    totalArea = usageInfo.TotalArea;
                }

                detail.RoomUsage = floorUsage;
                detail.TotalArea = totalArea;
                detail.InvoiceDetail.ChildMeterStartDate = childMeterStartDate?.AddDays(-1);
                detail.InvoiceDetail.ChildMeterEndDate = childMeterEndDate;

                detail.RoomArea = roomFloor?.FloorArea ?? 0;
                if (isPlazaCeresse37FDetail)
                {
                    detail.RoomShareRatio = 0;
                    detail.RoomUsageTotal = floorUsage;
                }
                else
                {
                    detail.RoomShareRatio = detail.TotalArea > 0 ? detail.RoomArea / detail.TotalArea : 0;
                    detail.RoomUsageTotal = detail.RoomUsage * detail.RoomShareRatio;
                }

                if (!waterBuildingClientsCache.TryGetValue(buildingName, out var buildingClients))
                {
                    buildingClients = await ClientDataAccess.GetClientsByBuildingNameAsync(buildingName);
                    waterBuildingClientsCache[buildingName] = buildingClients;
                }

                var lessorName = buildingClients.FirstOrDefault(c => c.IsLessor)?.Name ?? string.Empty;
                var lesseeName = buildingClients
                    .FirstOrDefault(c => c.IsLessee && string.Equals(c.RoomName, roomName, StringComparison.OrdinalIgnoreCase))
                    ?.Name ?? string.Empty;
                Client? contractorClientWater = null;
                if (detail.Billing.ContractorId.HasValue)
                {
                    var contractorIdWater = detail.Billing.ContractorId.Value;
                    if (!waterContractorClientCache.TryGetValue(contractorIdWater, out contractorClientWater))
                    {
                        contractorClientWater = await ClientDataAccess.GetClientByIdAsync(contractorIdWater);
                        waterContractorClientCache[contractorIdWater] = contractorClientWater;
                    }
                }
                contractorClientWater ??= buildingClients.FirstOrDefault(c => c.IsContractor);
                var contractorName = contractorClientWater?.Name ?? string.Empty;
                var contractorInvoiceNumberWater = contractorClientWater?.InvoiceNumber ?? string.Empty;

                var childMeterOnlyAmount = isPlazaCeresse37FDetail
                    ? floorUsage
                    : detail.RoomUsage * detail.RoomShareRatio;
                detail.InvoiceDetail = new InvoiceDetail
                {
                    BillingTo = detail.Client.Name,
                    Lessor = lessorName,
                    BuildingName = buildingName,
                    Lessee = lesseeName,
                    RoomNumber = roomName,
                    RoomArea = detail.RoomArea,
                    Category = "水道",
                    Content = "水道料金",
                    ChildMeterUsage = childMeterOnlyAmount,
                    UsageAmount = detail.RoomUsageTotal,
                    Unit = "m3",
                    Contractor = contractorName,
                    InvoiceNumber = contractorInvoiceNumberWater,
                    ParentMeterStartDate = detail.Billing.StartDate,
                    ParentMeterEndDate = detail.Billing.EndDate,
                    BillingYearMonth = billingYearMonth
                };
            }

            var waterTotalRoomUsageByBillingId = waterBillingDetails
                .GroupBy(d => d.Billing.Id)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.RoomUsageTotal));

            foreach (var billingId in waterTotalRoomUsageByBillingId.Keys.ToList())
            {
                var totalRoomUsage = waterTotalRoomUsageByBillingId[billingId];
                var billing = waterBillingDetails.First(d => d.Billing.Id == billingId).Billing;
                if (IsPlazaCeresse37FRawUsageWaterBilling(billing, meters))
                {
                    continue;
                }

                var difference = billing.UsageAmount - totalRoomUsage;
                if (difference == 0 || string.IsNullOrWhiteSpace(billing.DifferenceAssignmentRoomName))
                {
                    continue;
                }

                var targetDetail = waterBillingDetails.FirstOrDefault(d =>
                    d.Billing.Id == billingId &&
                    string.Equals(d.Client.RoomName ?? string.Empty, billing.DifferenceAssignmentRoomName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (targetDetail == null)
                {
                    continue;
                }

                targetDetail.RoomUsageTotal += difference;
                targetDetail.InvoiceDetail.UsageAmount = targetDetail.RoomUsageTotal;
                waterTotalRoomUsageByBillingId[billingId] = totalRoomUsage + difference;
            }

            foreach (var detail in waterBillingDetails)
            {
                if (IsPlazaCeresse37FRawUsageWaterBilling(detail.Billing, meters))
                {
                    var parentUsage = detail.Billing.UsageAmount;
                    var parentTotalCharge = detail.Billing.BasicCharge + detail.Billing.UsageCharge;
                    detail.RoomUnitPrice = parentUsage != 0 ? parentTotalCharge / parentUsage : 0;
                    detail.RoomBasicCharge = 0;
                    detail.RoomUsageCharge = detail.RoomUnitPrice * detail.RoomUsageTotal;
                    detail.RoomWaterCharge = detail.RoomUsageCharge;
                }
                else
                {
                    var totalRoomUsage = waterTotalRoomUsageByBillingId.TryGetValue(detail.Billing.Id, out var totalUsage)
                        ? totalUsage
                        : 0;
                    detail.RoomUnitPrice = totalRoomUsage != 0 ? detail.Billing.UsageCharge / totalRoomUsage : 0;
                    detail.RoomUsageCharge = detail.RoomUnitPrice * detail.RoomUsageTotal;
                    detail.RoomBasicCharge = detail.Billing.BasicCharge * detail.RoomShareRatio;
                    detail.RoomWaterCharge = detail.RoomBasicCharge + detail.RoomUsageCharge;
                }

                detail.InvoiceDetail.TaxInclusiveAmount = detail.RoomWaterCharge;
                detail.InvoiceDetail.TaxRate = detail.Billing.TaxRate;
                detail.InvoiceDetail.BillingYearMonth = billingYearMonth;
                detail.InvoiceDetail.ConfirmedBillingDate = GetPlannedBillingDateByTransferMethod(billingDate, detail.Client);
            }

            foreach (var detail in waterBillingDetails)
            {
                if (detail.InvoiceDetail.TaxInclusiveAmount == 0)
                {
                    continue;
                }

                await InvoiceDetailDataAccess.CreateInvoiceDetailAsync(detail.InvoiceDetail);
            }
        }

        private static bool IsHattoriBuilding(string? buildingName) =>
            string.Equals(buildingName?.Trim(), HattoriBuildingName, StringComparison.Ordinal);

        private static bool IsElectricBillingExcludedBuilding(string? buildingName)
        {
            var trimmed = buildingName?.Trim() ?? string.Empty;
            return ElectricBillingExcludedBuildingNames.Any(name =>
                string.Equals(trimmed, name, StringComparison.Ordinal));
        }

        /// <summary>
        /// 請求予定日（電気・水道・ガス）。自動引き落とし＝受領請求年月の翌月27日、口座振込＝翌月末。
        /// 両方ONの場合は自動引き落としを優先する。
        /// </summary>
        private static DateTime GetPlannedBillingDateByTransferMethod(DateTime billingDate, Client billingClient)
        {
            if (billingClient.IsAutoTransfer)
            {
                var nextMonth = billingDate.AddMonths(1);
                return new DateTime(nextMonth.Year, nextMonth.Month, 27);
            }

            return billingDate.AddMonths(2).AddDays(-1);
        }

        private static Client? FindBillingClientByName(IEnumerable<Client> clients, string name)
        {
            var normalized = (name ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(normalized))
            {
                return null;
            }

            return clients.FirstOrDefault(c =>
                c.IsBillingTo
                && string.Equals((c.Name ?? string.Empty).Trim(), normalized, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 服部ビル水道：単価＝親使用料金÷親使用量、貸主＝子メーター使用量×単価、借主＝親使用料金−貸主料金。明細2件（借主・貸主）。
        /// </summary>
        private static async Task RegisterHattoriWaterInvoiceDetailsAsync(
            WaterBilling billing,
            DateTime billingDate,
            DateTime prevTwoMonthsDate,
            string billingYearMonth)
        {
            var buildingName = billing.BuildingName?.Trim() ?? string.Empty;
            if (!IsHattoriBuilding(buildingName))
            {
                return;
            }

            var floors = await FloorDataAccess.GetFloorsByBuildingNameAsync(buildingName);
            var lessorFloor = floors.FirstOrDefault(f =>
                string.Equals((f.FloorName ?? string.Empty).Trim(), HattoriWaterLessorFloorName, StringComparison.Ordinal));

            decimal lessorUsage = 0;
            DateTime? childMeterStartDate = null;
            DateTime? childMeterEndDate = null;

            var roomChildMeters = await RoomChildMeterDataAccess.GetAllRoomChildMetersAsync();
            var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
            var childMeterById = childMeters
                .Where(cm => cm != null)
                .GroupBy(cm => cm.Id)
                .ToDictionary(g => g.Key, g => g.First());

            if (lessorFloor != null)
            {
                var roomChildMeter = roomChildMeters.FirstOrDefault(rcm =>
                    rcm.FloorId == lessorFloor.Id
                    && childMeterById.TryGetValue(rcm.ChildMeterId, out var cm)
                    && cm.MeterType == "水道");

                if (roomChildMeter != null && roomChildMeter.ChildMeterId > 0)
                {
                    var readings = await ChildMeterReadingDataAccess.GetWaterChildMeterReadingsByChildMeterIdAndMonthsAsync(
                        roomChildMeter.ChildMeterId,
                        billingDate.Year,
                        billingDate.Month,
                        prevTwoMonthsDate.Year,
                        prevTwoMonthsDate.Month);
                    var currentReading = GetLatestReadingForMonth(readings, billingDate.Year, billingDate.Month);
                    var prevTwoReading = GetLatestReadingForMonth(readings, prevTwoMonthsDate.Year, prevTwoMonthsDate.Month);
                    var currentValue = currentReading?.MeterValue ?? 0;
                    var prevTwoValue = prevTwoReading?.MeterValue ?? 0;
                    lessorUsage = currentValue - prevTwoValue;
                    childMeterStartDate = prevTwoReading?.ReadingDate.AddDays(-1);
                    childMeterEndDate = currentReading?.ReadingDate;
                }
            }

            var buildingClients = await ClientDataAccess.GetClientsByBuildingNameAsync(buildingName);

            Client? contractorClientHattori = null;
            if (billing.ContractorId.HasValue)
            {
                contractorClientHattori = await ClientDataAccess.GetClientByIdAsync(billing.ContractorId.Value);
            }
            contractorClientHattori ??= buildingClients.FirstOrDefault(c => c.IsContractor);
            var contractorName = contractorClientHattori?.Name ?? string.Empty;
            var contractorInvoiceNumberHattori = contractorClientHattori?.InvoiceNumber ?? string.Empty;

            var parentUsage = billing.UsageAmount;
            var parentUsageCharge = billing.UsageCharge;
            var unitPrice = parentUsage != 0 ? parentUsageCharge / parentUsage : 0m;
            var lessorCharge = lessorUsage * unitPrice;
            var lesseeUsageCharge = parentUsageCharge - lessorCharge;
            var lesseeUsageAmount = parentUsage - lessorUsage;

            var lesseeBillingClient = FindBillingClientByName(buildingClients, HattoriWaterLesseeDisplayName)
                ?? new Client();
            var lessorBillingClient = FindBillingClientByName(buildingClients, HattoriWaterLessorClientName)
                ?? new Client();

            var lesseeDetail = new InvoiceDetail
            {
                BillingTo = HattoriWaterLesseeDisplayName,
                Lessor = string.Empty,
                BuildingName = buildingName,
                Lessee = HattoriWaterLesseeDisplayName,
                RoomNumber = HattoriWaterLesseeRoomNumber,
                RoomArea = null,
                Category = "水道",
                Content = "水道料金",
                ChildMeterUsage = null,
                UsageAmount = lesseeUsageAmount,
                Unit = "m3",
                TaxInclusiveAmount = lesseeUsageCharge,
                TaxRate = billing.TaxRate,
                Contractor = contractorName,
                InvoiceNumber = contractorInvoiceNumberHattori,
                ChildMeterStartDate = childMeterStartDate,
                ChildMeterEndDate = childMeterEndDate,
                ParentMeterStartDate = billing.StartDate,
                ParentMeterEndDate = billing.EndDate,
                BillingYearMonth = billingYearMonth,
                ConfirmedBillingDate = GetPlannedBillingDateByTransferMethod(billingDate, lesseeBillingClient)
            };

            var lessorDetail = new InvoiceDetail
            {
                BillingTo = HattoriWaterLessorClientName,
                Lessor = string.Empty,
                BuildingName = buildingName,
                Lessee = string.Empty,
                RoomNumber = string.Empty,
                RoomArea = null,
                Category = "水道",
                Content = "水道料金",
                ChildMeterUsage = null,
                UsageAmount = lessorUsage,
                Unit = "m3",
                TaxInclusiveAmount = lessorCharge,
                TaxRate = billing.TaxRate,
                Contractor = contractorName,
                InvoiceNumber = contractorInvoiceNumberHattori,
                ChildMeterStartDate = childMeterStartDate,
                ChildMeterEndDate = childMeterEndDate,
                ParentMeterStartDate = billing.StartDate,
                ParentMeterEndDate = billing.EndDate,
                BillingYearMonth = billingYearMonth,
                ConfirmedBillingDate = GetPlannedBillingDateByTransferMethod(billingDate, lessorBillingClient)
            };

            if (lesseeDetail.TaxInclusiveAmount != 0)
            {
                await InvoiceDetailDataAccess.CreateInvoiceDetailAsync(lesseeDetail);
            }

            if (lessorDetail.TaxInclusiveAmount != 0)
            {
                await InvoiceDetailDataAccess.CreateInvoiceDetailAsync(lessorDetail);
            }
        }

        private static bool NamesEqualForHattoriWater(string? a, string b)
        {
            var na = NormalizeHattoriWaterName(a);
            var nb = NormalizeHattoriWaterName(b);
            return string.Equals(na, nb, StringComparison.Ordinal);
        }

        private static string NormalizeHattoriWaterName(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            var s = name.Trim();
            return s.Replace("\u3000", " ").Replace("  ", " ");
        }
    }
}

