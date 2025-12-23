using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OfficeOpenXml;

namespace WaterUtilityCost
{
    /// <summary>
    /// Excelファイルの内容を確認するための一時的なクラス
    /// </summary>
    public class ExcelChecker
    {
        public static void CheckInvoiceExcel()
        {
            var result = new StringBuilder();
            
            // 複数のパス候補を試す
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var currentDirectory = Environment.CurrentDirectory;
            
            var pathCandidates = new List<string>
            {
                // プロジェクトルートからの相対パス（実行ファイルから3階層上）
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "Excel", "請求書.xlsx")),
                // 現在の作業ディレクトリからの相対パス
                Path.Combine(currentDirectory, "Excel", "請求書.xlsx"),
                // 実行ファイルと同じディレクトリからの相対パス
                Path.Combine(baseDirectory, "Excel", "請求書.xlsx"),
                // プロジェクトルートからの相対パス（別の構造）
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", "Excel", "請求書.xlsx")),
                // 絶対パス（ユーザー指定のパス）
                @"C:\Users\渡辺詠介\Desktop\work\田邊\水道光熱費アプリ\Source\WaterUtilityCost\Excel\請求書.xlsx"
            };
            
            string? filePath = null;
            foreach (var candidate in pathCandidates)
            {
                if (File.Exists(candidate))
                {
                    filePath = candidate;
                    break;
                }
            }
            
            if (filePath == null)
            {
                var message = new StringBuilder();
                message.AppendLine("ファイルが見つかりません。");
                message.AppendLine();
                message.AppendLine("試したパス:");
                for (int i = 0; i < pathCandidates.Count; i++)
                {
                    message.AppendLine($"{i + 1}. {pathCandidates[i]}");
                }
                message.AppendLine();
                message.AppendLine($"現在の作業ディレクトリ: {currentDirectory}");
                message.AppendLine($"実行ファイルのディレクトリ: {baseDirectory}");
                
                MessageBox.Show(message.ToString(), "確認結果", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            result.AppendLine($"ファイルが見つかりました: {filePath}");
            result.AppendLine($"ファイルサイズ: {new FileInfo(filePath).Length} バイト");
            result.AppendLine();

            try
            {
                using var package = new ExcelPackage(new FileInfo(filePath));
                
                result.AppendLine($"シート数: {package.Workbook.Worksheets.Count}");
                result.AppendLine();

                bool hasData = false;
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    result.AppendLine($"シート名: {worksheet.Name}");
                    result.AppendLine($"使用範囲: {worksheet.Dimension?.Address ?? "なし"}");
                    
                    if (worksheet.Dimension != null)
                    {
                        hasData = true;
                        var startRow = worksheet.Dimension.Start.Row;
                        var endRow = worksheet.Dimension.End.Row;
                        var startCol = worksheet.Dimension.Start.Column;
                        var endCol = worksheet.Dimension.End.Column;
                        
                        result.AppendLine($"  行数: {endRow - startRow + 1} (開始行: {startRow}, 終了行: {endRow})");
                        result.AppendLine($"  列数: {endCol - startCol + 1} (開始列: {startCol}, 終了列: {endCol})");
                        
                        // 最初の5行を表示
                        result.AppendLine("  最初の5行の内容:");
                        for (int row = startRow; row <= Math.Min(startRow + 4, endRow); row++)
                        {
                            var rowValues = new System.Collections.Generic.List<string>();
                            for (int col = startCol; col <= Math.Min(startCol + 9, endCol); col++)
                            {
                                var cellValue = worksheet.Cells[row, col].Value;
                                var value = cellValue?.ToString() ?? "";
                                if (value.Length > 20) value = value.Substring(0, 20) + "...";
                                rowValues.Add(value);
                            }
                            result.AppendLine($"    行{row}: {string.Join(" | ", rowValues)}");
                        }
                        if (endRow > startRow + 4)
                        {
                            result.AppendLine($"    ... (他 {endRow - startRow - 4} 行)");
                        }
                    }
                    else
                    {
                        result.AppendLine("  データがありません");
                    }
                    result.AppendLine();
                }

                if (hasData)
                {
                    result.Insert(0, "【請求内容あり】\n\n");
                }
                else
                {
                    result.Insert(0, "【請求内容なし】\n\n");
                }

                MessageBox.Show(result.ToString(), "Excelファイル確認結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました:\n\n{ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

