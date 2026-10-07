using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.DataAccess;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.Forms
{
    public partial class ElectricChildMeterReadingManagementForm
    {
        private const string ParkingRemapBuildingName = "パークセレス根岸";
        private const string KoenanBuildingName = "光南ビル";
        private enum ElectricCsvImportKind
        {
            ElectricLight,
            LowVoltage
        }

        private async void BtnImportElectricLightCsv_Click(object? sender, EventArgs e)
        {
            await ImportElectricChildMeterCsvAsync("電気電灯CSVファイルを選択", ElectricCsvImportKind.ElectricLight);
        }

        private async void BtnImportLowVoltageCsv_Click(object? sender, EventArgs e)
        {
            await ImportElectricChildMeterCsvAsync("低電圧CSVファイルを選択", ElectricCsvImportKind.LowVoltage);
        }

        private async Task ImportElectricChildMeterCsvAsync(string dialogTitle, ElectricCsvImportKind importKind)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = dialogTitle,
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            statusLabel.Text = "CSV読込中...";
            statusStrip.Refresh();
            try
            {
                var errors = new List<string>();
                var applied = await ProcessElectricChildMeterReadingCsvAsync(openFileDialog.FileName, errors, importKind);
                if (errors.Count > 0)
                {
                    var msg = string.Join(Environment.NewLine, errors.Take(50));
                    if (errors.Count > 50)
                    {
                        msg += Environment.NewLine + $"...他 {errors.Count - 50} 件";
                    }

                    MessageBox.Show(
                        $"処理件数: {applied}。{Environment.NewLine}{msg}",
                        "CSV読込",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else if (applied == 0)
                {
                    MessageBox.Show("取込対象のデータがありませんでした。", "CSV読込", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"CSVから検針データを反映しました。（{applied} 件）", "CSV読込", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await LoadElectricChildMeterReadingsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CSVの読込に失敗しました。{Environment.NewLine}{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                statusLabel.Text = "準備完了";
            }
        }

        private static async Task<int> ProcessElectricChildMeterReadingCsvAsync(
            string filePath,
            List<string> errors,
            ElectricCsvImportKind importKind)
        {
            var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
            var buildingNameMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.Name))
                .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);
            var buildingCodeMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.BuildingId))
                .GroupBy(b => b.BuildingId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);
            var buildingsById = buildings.ToDictionary(b => b.Id);

            var floors = await FloorDataAccess.GetAllFloorsAsync();
            var floorsByBuildingId = floors
                .GroupBy(f => f.BuildingId)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .Where(f => !string.IsNullOrWhiteSpace(f.FloorName))
                        .GroupBy(f => NormalizeTextKey(f.FloorName))
                        .ToDictionary(fg => fg.Key, fg => fg.First(), StringComparer.OrdinalIgnoreCase));

            var roomChildMeters = await RoomChildMeterDataAccess.GetAllRoomChildMetersAsync();
            var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();

            var parsedRows = new List<CsvReadingWorkRow>();
            using (var reader = CreateCsvStreamReader(filePath))
            {
                // 1行目はタイトル行として内容を検証せず読み飛ばす。2行目以降がデータ（A:ビルID B:建物名 C:部屋名 D:検針日 E:メーター値）。
                _ = await reader.ReadLineAsync();

                var lineNumber = 1;
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null)
                    {
                        break;
                    }

                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var fields = SplitCsvFields(line);
                    if (fields.Count < 5)
                    {
                        var bnShort = fields.Count > 1 ? fields[1].Trim() : string.Empty;
                        var rmShort = fields.Count > 2 ? fields[2].Trim() : string.Empty;
                        errors.Add(
                            $"{lineNumber}行目 建物名「{bnShort}」部屋名「{rmShort}」: 列数が不足しています（A〜E列の5列が必要です）。");
                        continue;
                    }

                    var buildingIdText = fields[0].Trim();
                    var buildingNameText = fields[1].Trim();
                    var roomNameRaw = fields[2].Trim();
                    var dateText = fields[3].Trim();
                    var meterText = fields[4].Trim();

                    // 部屋名が空の行は取り込まない（エラーにもしない）
                    if (string.IsNullOrWhiteSpace(roomNameRaw))
                    {
                        continue;
                    }

                    if (!TryParseReadingDateMmDdYyyy(dateText, out var readingDate))
                    {
                        errors.Add(
                            $"{lineNumber}行目 建物名「{buildingNameText}」部屋名「{roomNameRaw}」: 検針日の形式が不正です（MM/dd/yyyy 想定）: {dateText}");
                        continue;
                    }

                    if (!decimal.TryParse(meterText, NumberStyles.Number, CultureInfo.InvariantCulture, out var meterValue) || meterValue < 0)
                    {
                        errors.Add(
                            $"{lineNumber}行目 建物名「{buildingNameText}」部屋名「{roomNameRaw}」: メーター値が不正です: {meterText}");
                        continue;
                    }

                    var buildingId = ResolveBuildingIdFromCsv(
                        buildingIdText,
                        buildingNameText,
                        roomNameRaw,
                        buildingNameMap,
                        buildingCodeMap,
                        buildingsById,
                        lineNumber,
                        errors);
                    if (buildingId < 0)
                    {
                        continue;
                    }

                    var roomForFloor = ApplyParkingRoomRemap(buildingNameText, roomNameRaw);
                    roomForFloor = ApplyKoenanBuildingRoomRemap(buildingNameText, roomForFloor);
                    if (importKind == ElectricCsvImportKind.ElectricLight)
                    {
                        roomForFloor = ApplyAdditionalBuildingRoomRemap(buildingNameText, roomForFloor);
                    }
                    else if (importKind == ElectricCsvImportKind.LowVoltage)
                    {
                        roomForFloor = ApplyLowVoltageBuildingRoomRemap(buildingNameText, roomForFloor);
                    }

                    var buildingDisplayName = buildingsById.TryGetValue(buildingId, out var bld)
                        ? (bld.Name ?? string.Empty).Trim()
                        : buildingNameText;
                    parsedRows.Add(new CsvReadingWorkRow(
                        buildingId,
                        buildingDisplayName,
                        roomNameRaw,
                        roomForFloor,
                        readingDate,
                        meterValue));
                }
            }

            if (parsedRows.Count == 0)
            {
                return 0;
            }

            var aggregated = new Dictionary<(int BuildingId, string RoomNormKey, int Year, int Month), ElectricCsvAggregatedEntry>();
            foreach (var row in parsedRows)
            {
                var roomKey = NormalizeTextKey(row.RoomNameForFloor);
                var key = (row.BuildingId, roomKey, row.ReadingDate.Year, row.ReadingDate.Month);
                if (!aggregated.TryGetValue(key, out var agg))
                {
                    aggregated[key] = new ElectricCsvAggregatedEntry
                    {
                        TotalMeterValue = row.MeterValue,
                        MaxReadingDateInMonth = row.ReadingDate,
                        BuildingDisplayName = row.BuildingDisplayName,
                        RoomDisplayName = row.RoomDisplayName,
                        RoomNameForFloor = row.RoomNameForFloor
                    };
                }
                else
                {
                    agg.TotalMeterValue += row.MeterValue;
                    if (row.ReadingDate > agg.MaxReadingDateInMonth)
                    {
                        agg.MaxReadingDateInMonth = row.ReadingDate;
                    }
                }
            }

            var upsertCommands = new List<ElectricCsvUpsertCommand>();
            foreach (var kvp in aggregated)
            {
                var (buildingId, roomNormKey, year, month) = kvp.Key;
                var agg = kvp.Value;
                if (!floorsByBuildingId.TryGetValue(buildingId, out var floorsByName))
                {
                    errors.Add(
                        $"建物名「{agg.BuildingDisplayName}」部屋名「{agg.RoomDisplayName}」検針年月 {year}-{month:D2}: 部屋マスタが見つかりません。（ビルID:{buildingId}）");
                    continue;
                }

                if (!floorsByName.TryGetValue(roomNormKey, out var floor))
                {
                    var roomNote = string.Equals(agg.RoomDisplayName, agg.RoomNameForFloor, StringComparison.Ordinal)
                        ? string.Empty
                        : $"（照合用:「{agg.RoomNameForFloor}」）";
                    errors.Add(
                        $"建物名「{agg.BuildingDisplayName}」部屋名「{agg.RoomDisplayName}」{roomNote}検針年月 {year}-{month:D2}: 部屋名が一致するフロアがありません。");
                    continue;
                }

                var childMeterId = ResolveElectricChildMeterIdForFloor(floor.Id, roomChildMeters, childMeters);
                if (!childMeterId.HasValue)
                {
                    var roomNote2 = string.Equals(agg.RoomDisplayName, agg.RoomNameForFloor, StringComparison.Ordinal)
                        ? string.Empty
                        : $"（照合用:「{agg.RoomNameForFloor}」）";
                    errors.Add(
                        $"建物名「{agg.BuildingDisplayName}」部屋名「{agg.RoomDisplayName}」{roomNote2}検針年月 {year}-{month:D2}: 電気子メーター（部屋紐付け）が見つかりません。");
                    continue;
                }

                upsertCommands.Add(new ElectricCsvUpsertCommand(
                    floor.Id,
                    childMeterId.Value,
                    year,
                    month,
                    agg.TotalMeterValue,
                    agg.MaxReadingDateInMonth));
            }

            if (upsertCommands.Count == 0)
            {
                return 0;
            }

            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var cmd in upsertCommands)
                {
                    await ChildMeterReadingDataAccess.UpsertAddElectricChildMeterReadingValueAsync(
                        connection,
                        transaction,
                        cmd.FloorId,
                        cmd.ChildMeterId,
                        cmd.Year,
                        cmd.Month,
                        cmd.AdditionalMeterValue,
                        cmd.ReadingDateCandidate);
                }

                transaction.Commit();
                return upsertCommands.Count;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private sealed class CsvReadingWorkRow
        {
            public int BuildingId { get; }
            public string BuildingDisplayName { get; }
            /// <summary>CSVの部屋名（エラー表示用）</summary>
            public string RoomDisplayName { get; }
            /// <summary>フロア照合用（駐車場置換後など）</summary>
            public string RoomNameForFloor { get; }
            public DateTime ReadingDate { get; }
            public decimal MeterValue { get; }

            public CsvReadingWorkRow(
                int buildingId,
                string buildingDisplayName,
                string roomDisplayName,
                string roomNameForFloor,
                DateTime readingDate,
                decimal meterValue)
            {
                BuildingId = buildingId;
                BuildingDisplayName = buildingDisplayName;
                RoomDisplayName = roomDisplayName;
                RoomNameForFloor = roomNameForFloor;
                ReadingDate = readingDate;
                MeterValue = meterValue;
            }
        }

        private sealed class ElectricCsvAggregatedEntry
        {
            public decimal TotalMeterValue;
            public DateTime MaxReadingDateInMonth;
            public string BuildingDisplayName = string.Empty;
            public string RoomDisplayName = string.Empty;
            public string RoomNameForFloor = string.Empty;
        }

        private sealed class ElectricCsvUpsertCommand
        {
            public int FloorId { get; }
            public int ChildMeterId { get; }
            public int Year { get; }
            public int Month { get; }
            public decimal AdditionalMeterValue { get; }
            public DateTime ReadingDateCandidate { get; }

            public ElectricCsvUpsertCommand(
                int floorId,
                int childMeterId,
                int year,
                int month,
                decimal additionalMeterValue,
                DateTime readingDateCandidate)
            {
                FloorId = floorId;
                ChildMeterId = childMeterId;
                Year = year;
                Month = month;
                AdditionalMeterValue = additionalMeterValue;
                ReadingDateCandidate = readingDateCandidate;
            }
        }

        private static string ApplyParkingRoomRemap(string? csvBuildingName, string? csvRoomName)
        {
            var bn = csvBuildingName?.Trim() ?? string.Empty;
            var rn = csvRoomName?.Trim() ?? string.Empty;
            var buildingMatch = string.Equals(bn, ParkingRemapBuildingName, StringComparison.Ordinal);

            if (!buildingMatch)
            {
                return rn;
            }

            // パークセレス根岸：特定の駐車場表記をフロア名「駐車場」に寄せる
            if (rn is "駐車場１" or "駐車場２" or "駐車場動力計" or "駐車場1" or "駐車場2")
            {
                return "駐車場";
            }

            return rn;
        }

        /// <summary>
        /// 光南ビル: CSVの部屋名をフロアマスタ（Floors.FloorName）に合わせて置換する。
        /// </summary>
        private static string ApplyKoenanBuildingRoomRemap(string? csvBuildingName, string roomAfterParkingRemap)
        {
            var bn = (csvBuildingName ?? string.Empty).Trim();
            if (!string.Equals(bn, KoenanBuildingName, StringComparison.Ordinal))
            {
                return roomAfterParkingRemap;
            }

            var rn = roomAfterParkingRemap.Trim();
            if (string.Equals(rn, "2F", StringComparison.OrdinalIgnoreCase))
            {
                return "2F-A";
            }

            if (string.Equals(rn, "3F", StringComparison.OrdinalIgnoreCase))
            {
                return "3F-A";
            }

            if (string.Equals(rn, "4F", StringComparison.OrdinalIgnoreCase))
            {
                return "4F-A";
            }

            if (string.Equals(rn, "5F", StringComparison.OrdinalIgnoreCase))
            {
                return "5F-A";
            }

            if (string.Equals(rn, "6F", StringComparison.OrdinalIgnoreCase))
            {
                return "6F-A";
            }

            return roomAfterParkingRemap;
        }

        /// <summary>
        /// 追加要件: 建物ごとの部屋名ゆれをフロアマスタに合わせて置換する。
        /// </summary>
        private static string ApplyAdditionalBuildingRoomRemap(string? csvBuildingName, string roomName)
        {
            var bn = (csvBuildingName ?? string.Empty).Trim();
            var rn = roomName.Trim();

            if (string.Equals(bn, "岩本ビル", StringComparison.Ordinal))
            {
                if (string.Equals(rn, "2FA", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(rn, "2FB", StringComparison.OrdinalIgnoreCase))
                {
                    return "2FA";
                }
            }

            if (string.Equals(bn, "北村第２ビル", StringComparison.Ordinal))
            {
                if (string.Equals(rn, "301-2", StringComparison.OrdinalIgnoreCase))
                {
                    return "301";
                }
            }

            if (string.Equals(bn, "新翁ビル", StringComparison.Ordinal))
            {
                if (string.Equals(rn, "1FA", StringComparison.OrdinalIgnoreCase))
                {
                    return "1F";
                }
            }

            if (string.Equals(bn, "山下町信濃屋店舗", StringComparison.Ordinal))
            {
                if (string.Equals(rn, "１F", StringComparison.Ordinal))
                {
                    return "1";
                }

                if (string.Equals(rn, "２F", StringComparison.Ordinal))
                {
                    return "2";
                }
            }

            return roomName;
        }

        /// <summary>
        /// 低電圧CSV専用の建物別部屋名変換。
        /// </summary>
        private static string ApplyLowVoltageBuildingRoomRemap(string? csvBuildingName, string roomName)
        {
            var bn = (csvBuildingName ?? string.Empty).Trim();
            var rn = roomName.Trim();

            if (string.Equals(bn, "岩本ビル", StringComparison.Ordinal))
            {
                if (string.Equals(rn, "1F　M-1", StringComparison.Ordinal))
                {
                    return "1F";
                }

                if (string.Equals(rn, "2FＡ", StringComparison.Ordinal))
                {
                    return "2FA";
                }

                if (string.Equals(rn, "3FB　片岡特許", StringComparison.Ordinal))
                {
                    return "3FB";
                }

                if (string.Equals(rn, "B1F　ML-B1", StringComparison.Ordinal))
                {
                    return "B1F";
                }
            }

            if (string.Equals(bn, "服部ビル", StringComparison.Ordinal))
            {
                if (string.Equals(rn, "２Ｆ", StringComparison.Ordinal))
                {
                    return "2F";
                }
            }

            return roomName;
        }

        private static int ResolveBuildingIdFromCsv(
            string? buildingIdText,
            string? buildingNameText,
            string roomNameCsv,
            Dictionary<string, int> buildingNameMap,
            Dictionary<string, int> buildingCodeMap,
            Dictionary<int, Building> buildingsById,
            int lineNumber,
            List<string> errors)
        {
            if (!string.IsNullOrWhiteSpace(buildingIdText))
            {
                if (buildingCodeMap.TryGetValue(buildingIdText.Trim(), out var buildingId))
                {
                    return buildingId;
                }

                if (int.TryParse(buildingIdText.Trim(), out var parsedId) && buildingsById.ContainsKey(parsedId))
                {
                    return parsedId;
                }
            }

            if (!string.IsNullOrWhiteSpace(buildingNameText))
            {
                if (buildingNameMap.TryGetValue(buildingNameText.Trim(), out var buildingId))
                {
                    return buildingId;
                }
            }

            errors.Add(
                $"{lineNumber}行目 建物名「{buildingNameText}」部屋名「{roomNameCsv}」: ビルが見つかりません（ビルID:{buildingIdText}）");
            return -1;
        }

        private static int? ResolveElectricChildMeterIdForFloor(
            int floorId,
            List<RoomChildMeter> roomChildMeters,
            List<ChildMeter> childMeters)
        {
            var meterById = childMeters.ToDictionary(cm => cm.Id);
            var electricIds = roomChildMeters
                .Where(rcm => rcm.FloorId == floorId)
                .Select(rcm => rcm.ChildMeterId)
                .Distinct()
                .Where(id => meterById.TryGetValue(id, out var cm)
                             && string.Equals(cm.MeterType?.Trim(), METER_TYPE, StringComparison.OrdinalIgnoreCase))
                .OrderBy(id => id)
                .ToList();

            if (electricIds.Count == 0)
            {
                return null;
            }

            return electricIds[0];
        }

        private static bool TryParseReadingDateMmDdYyyy(string text, out DateTime dt)
        {
            return DateTime.TryParseExact(
                text.Trim(),
                new[] { "MM/dd/yyyy", "M/d/yyyy", "MM/d/yyyy", "M/dd/yyyy" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dt);
        }

        /// <summary>
        /// Excelの「CSV（カンマ区切り）」は日本語環境で Shift_JIS になることが多い。
        /// UTF-8 BOM 付きは UTF-8、無BOMは厳密UTF-8として解釈できなければ CP932 で読む。
        /// </summary>
        private static StreamReader CreateCsvStreamReader(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            if (bytes.Length == 0)
            {
                return new StreamReader(new MemoryStream(Array.Empty<byte>()), new UTF8Encoding(false), detectEncodingFromByteOrderMarks: false);
            }

            var payloadOffset = 0;
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                payloadOffset = 3;
                return new StreamReader(
                    new MemoryStream(bytes, payloadOffset, bytes.Length - payloadOffset, writable: false),
                    new UTF8Encoding(false),
                    detectEncodingFromByteOrderMarks: false);
            }

            var encoding = IsStrictValidUtf8(bytes, payloadOffset, bytes.Length - payloadOffset)
                ? new UTF8Encoding(false)
                : GetShiftJisOrUtf8Fallback();

            return new StreamReader(
                new MemoryStream(bytes, payloadOffset, bytes.Length - payloadOffset, writable: false),
                encoding,
                detectEncodingFromByteOrderMarks: false);
        }

        private static bool IsStrictValidUtf8(byte[] bytes, int index, int count)
        {
            if (count <= 0)
            {
                return true;
            }

            try
            {
                var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
                utf8.GetString(bytes, index, count);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static Encoding GetShiftJisOrUtf8Fallback()
        {
            try
            {
                return Encoding.GetEncoding(932);
            }
            catch
            {
                return new UTF8Encoding(false);
            }
        }

        /// <summary>
        /// カンマ区切り（引用符対応）またはタブのみ区切りの1行を分割する。
        /// </summary>
        private static List<string> SplitCsvFields(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return new List<string>();
            }

            var hasTab = line.IndexOf('\t') >= 0;
            var hasComma = line.IndexOf(',') >= 0;
            if (hasTab && (!hasComma || line.Split('\t').Length > line.Split(',').Length))
            {
                return line.Split('\t').Select(c => c.Trim().Trim('\uFEFF')).ToList();
            }

            return ParseCsvLine(line);
        }

        private static string NormalizeTextKey(string value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Trim('\uFEFF')
                .Replace(" ", string.Empty)
                .Replace("　", string.Empty)
                .ToLowerInvariant();
        }

        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }

                    continue;
                }

                if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            result.Add(current.ToString().Trim());
            return result;
        }
    }
}
