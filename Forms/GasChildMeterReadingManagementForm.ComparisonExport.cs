using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.DataAccess;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.Forms
{
    public partial class GasChildMeterReadingManagementForm
    {
        private async void BtnExportComparisonCsv_Click(object? sender, EventArgs e)
        {
            using var dialog = new ReadingYearMonthSelectDialog("比較対象年月:");
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var baseDate = new DateTime(dialog.SelectedYear, dialog.SelectedMonth, 1);
            var prevDate = baseDate.AddMonths(-1);
            var prevPrevDate = baseDate.AddMonths(-2);

            string? selectedFilePath;
            using (var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"ガス子メーター比較表_{baseDate:yyyyMM}.csv"
            })
            {
                if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                selectedFilePath = saveFileDialog.FileName;
            }

            try
            {
                statusLabel.Text = "比較表出力中...";
                var rowCount = await ExportComparisonCsvAsync(
                    selectedFilePath,
                    baseDate,
                    prevDate,
                    prevPrevDate);
                MessageBox.Show(
                    $"比較表CSVの出力が完了しました。（{rowCount} 件）",
                    "完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"比較表CSVの出力に失敗しました。{Environment.NewLine}{ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                statusLabel.Text = "準備完了";
            }
        }

        private async Task<int> ExportComparisonCsvAsync(
            string filePath,
            DateTime currentMonth,
            DateTime prevMonth,
            DateTime prevPrevMonth)
        {
            var readings = await ChildMeterReadingDataAccess.GetGasChildMeterReadingsByThreeMonthsAsync(
                currentMonth.Year,
                currentMonth.Month,
                prevMonth.Year,
                prevMonth.Month,
                prevPrevMonth.Year,
                prevPrevMonth.Month);

            var readingsByKey = readings
                .Where(r => r.ChildMeterId.HasValue)
                .GroupBy(r => (r.FloorId, r.ChildMeterId!.Value))
                .ToDictionary(g => g.Key, g => g.ToList());

            if (readingsByKey.Count == 0)
            {
                using var emptyWriter = new StreamWriter(filePath, false, Encoding.UTF8);
                emptyWriter.WriteLine(BuildComparisonCsvHeader());
                return 0;
            }

            var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
            var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
            var floors = await FloorDataAccess.GetAllFloorsAsync();
            var buildingById = buildings.ToDictionary(b => b.Id, b => b);
            var floorById = floors.ToDictionary(f => f.Id, f => f);
            var childMeterById = childMeters
                .Where(cm => string.Equals(cm.MeterType, METER_TYPE, StringComparison.Ordinal))
                .ToDictionary(cm => cm.Id, cm => cm);

            var exportTargets = readingsByKey.Keys
                .Where(key => childMeterById.ContainsKey(key.Item2))
                .Select(key =>
                {
                    var (floorId, childMeterId) = key;
                    var cm = childMeterById[childMeterId];
                    var floor = floorById.TryGetValue(floorId, out var f) ? f : null;
                    var building = floor != null && buildingById.TryGetValue(floor.BuildingId, out var b)
                        ? b
                        : (cm.BuildingId.HasValue && buildingById.TryGetValue(cm.BuildingId.Value, out var b2) ? b2 : null);
                    return new
                    {
                        FloorId = floorId,
                        ChildMeterId = childMeterId,
                        BuildingName = building?.Name ?? string.Empty,
                        ChildMeterName = cm.MeterName ?? string.Empty,
                        Readings = readingsByKey[key]
                    };
                })
                .OrderBy(x => x.BuildingName, StringComparer.Ordinal)
                .ThenBy(x => x.ChildMeterName, StringComparer.Ordinal)
                .ToList();

            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.WriteLine(BuildComparisonCsvHeader());

            foreach (var target in exportTargets)
            {
                var currentReading = GetLatestReadingForMonth(
                    target.Readings,
                    currentMonth.Year,
                    currentMonth.Month);
                var prevReading = GetLatestReadingForMonth(
                    target.Readings,
                    prevMonth.Year,
                    prevMonth.Month);
                var prevPrevReading = GetLatestReadingForMonth(
                    target.Readings,
                    prevPrevMonth.Year,
                    prevPrevMonth.Month);

                decimal? currentUsage = null;
                if (currentReading != null && prevReading != null)
                {
                    currentUsage = currentReading.MeterValue - prevReading.MeterValue;
                }

                decimal? prevUsage = null;
                if (prevReading != null && prevPrevReading != null)
                {
                    prevUsage = prevReading.MeterValue - prevPrevReading.MeterValue;
                }

                decimal? usageDiff = null;
                if (currentUsage.HasValue && prevUsage.HasValue)
                {
                    usageDiff = currentUsage.Value - prevUsage.Value;
                }

                decimal? ratio = null;
                if (currentUsage.HasValue && prevUsage.HasValue && prevUsage.Value != 0)
                {
                    ratio = currentUsage.Value / prevUsage.Value;
                }

                var check = ShouldMarkCheck(usageDiff, currentUsage, prevUsage) ? "チェック" : string.Empty;

                writer.WriteLine(string.Join(",",
                    EscapeCsvField(target.BuildingName),
                    EscapeCsvField(target.ChildMeterName),
                    check,
                    FormatCsvDate(currentReading?.ReadingDate),
                    FormatCsvDecimal(currentReading?.MeterValue),
                    FormatCsvDate(prevReading?.ReadingDate),
                    FormatCsvDecimal(prevReading?.MeterValue),
                    FormatCsvDate(prevPrevReading?.ReadingDate),
                    FormatCsvDecimal(prevPrevReading?.MeterValue),
                    FormatCsvDecimal(currentUsage),
                    FormatCsvDecimal(prevUsage),
                    FormatCsvDecimal(usageDiff),
                    FormatCsvDecimal(ratio)));
            }

            return exportTargets.Count;
        }

        private static string BuildComparisonCsvHeader()
        {
            return "建物名称,ガスメーター,チェック,当月検針日,当月M,前月検針日,前月M,前々月検針日,前々月M,当月使用量,前月使用量,使用量増減,前月比";
        }

        private static bool ShouldMarkCheck(decimal? usageDiff, decimal? currentUsage, decimal? prevUsage)
        {
            if (usageDiff.HasValue)
            {
                if (usageDiff.Value == 0 || usageDiff.Value < 0)
                {
                    return true;
                }
            }

            if (currentUsage.HasValue && prevUsage.HasValue && prevUsage.Value > 0)
            {
                if (currentUsage.Value >= prevUsage.Value * 2)
                {
                    return true;
                }
            }

            return false;
        }

        private static ChildMeterReading? GetLatestReadingForMonth(
            IEnumerable<ChildMeterReading> readings,
            int year,
            int month)
        {
            return readings
                .Where(r => r.ReadingDate.Year == year && r.ReadingDate.Month == month)
                .OrderByDescending(r => r.ReadingDate)
                .ThenByDescending(r => r.Id)
                .FirstOrDefault();
        }

        private static string FormatCsvDate(DateTime? date)
        {
            return date?.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private static string FormatCsvDecimal(decimal? value)
        {
            return value.HasValue
                ? value.Value.ToString("0.00", CultureInfo.InvariantCulture)
                : string.Empty;
        }

        private static string EscapeCsvField(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }
    }
}
