using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OfficeOpenXml;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 請求データ登録フォーム
    /// </summary>
    public partial class BillingDataRegistrationForm : Form
    {
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

        public BillingDataRegistrationForm()
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
            btnRegister.Click += BtnRegister_Click;
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

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            var billingYearMonth = dtpBillingYearMonth.Value.ToString("yyyy-MM");
            var result = MessageBox.Show($"請求年月「{billingYearMonth}」のデータを登録しますか？", 
                "登録確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                // TODO: 実際の登録処理を実装
                MessageBox.Show($"請求年月「{billingYearMonth}」のデータを登録しました。", 
                    "登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

