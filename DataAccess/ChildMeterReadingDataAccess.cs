using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    public class ChildMeterReadingDataAccess
    {
        public static async Task<List<ChildMeterReading>> GetAllChildMeterReadingsAsync()
        {
            var readings = new List<ChildMeterReading>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                CreatedAt, UpdatedAt
                          FROM ChildMeterReadings ORDER BY Id DESC";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add(new ChildMeterReading
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.IsDBNull(reader.GetOrdinal("ChildMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                    MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return readings;
        }

        public static async Task<ChildMeterReading?> GetChildMeterReadingByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                CreatedAt, UpdatedAt
                          FROM ChildMeterReadings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ChildMeterReading
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.IsDBNull(reader.GetOrdinal("ChildMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                    MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        public static async Task<int> CreateChildMeterReadingAsync(ChildMeterReading reading)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, @Type, @MeterValue,
                                  GETDATE(), GETDATE());
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@FloorId", reading.FloorId);
            cmd.Parameters.AddWithValue("@ChildMeterId", (object)reading.ChildMeterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReadingDate", reading.ReadingDate);
            cmd.Parameters.AddWithValue("@Type", (object)reading.Type ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterValue", reading.MeterValue);
            return (int)await cmd.ExecuteScalarAsync();
        }

        public static async Task<bool> UpdateChildMeterReadingAsync(ChildMeterReading reading)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE ChildMeterReadings SET 
                            FloorId = @FloorId,
                            ChildMeterId = @ChildMeterId, 
                            ReadingDate = @ReadingDate,
                            Type = @Type,
                            MeterValue = @MeterValue,
                            UpdatedAt = GETDATE()
                          WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", reading.Id);
            cmd.Parameters.AddWithValue("@FloorId", reading.FloorId);
            cmd.Parameters.AddWithValue("@ChildMeterId", (object)reading.ChildMeterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReadingDate", reading.ReadingDate);
            cmd.Parameters.AddWithValue("@Type", (object)reading.Type ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterValue", reading.MeterValue);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteChildMeterReadingAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM ChildMeterReadings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        /// <summary>
        /// 検針日の年月が一致する電気子メーター検針データの件数を取得する。
        /// </summary>
        public static async Task<int> CountElectricChildMeterReadingsByReadingYearMonthAsync(int year, int month)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"SELECT COUNT(*)
                          FROM ChildMeterReadings
                          WHERE Type = N'電気'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 検針日の年月が一致する電気子メーター検針データを一括削除する。
        /// </summary>
        /// <returns>削除件数</returns>
        public static async Task<int> DeleteElectricChildMeterReadingsByReadingYearMonthAsync(int year, int month)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"DELETE FROM ChildMeterReadings
                          WHERE Type = N'電気'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            return await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 検針日の年月が一致するガス子メーター検針データの件数を取得する。
        /// </summary>
        public static async Task<int> CountGasChildMeterReadingsByReadingYearMonthAsync(int year, int month)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"SELECT COUNT(*)
                          FROM ChildMeterReadings
                          WHERE Type = N'ガス'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 検針日の年月が一致するガス子メーター検針データを一括削除する。
        /// </summary>
        /// <returns>削除件数</returns>
        public static async Task<int> DeleteGasChildMeterReadingsByReadingYearMonthAsync(int year, int month)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"DELETE FROM ChildMeterReadings
                          WHERE Type = N'ガス'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            return await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 検針日の年月が一致する水道子メーター検針データの件数を取得する。
        /// </summary>
        public static async Task<int> CountWaterChildMeterReadingsByReadingYearMonthAsync(int year, int month)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"SELECT COUNT(*)
                          FROM ChildMeterReadings
                          WHERE Type = N'水道'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 検針日の年月が一致する水道子メーター検針データを一括削除する。
        /// </summary>
        /// <returns>削除件数</returns>
        public static async Task<int> DeleteWaterChildMeterReadingsByReadingYearMonthAsync(int year, int month)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"DELETE FROM ChildMeterReadings
                          WHERE Type = N'水道'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            return await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 電気子メーター検針（同一フロア・子メーター・年月）にメーター値を加算する。
        /// 既存が複数行の場合は値を合算して1行に統合する。
        /// </summary>
        public static async Task UpsertAddElectricChildMeterReadingValueAsync(
            SqlConnection connection,
            SqlTransaction? transaction,
            int floorId,
            int childMeterId,
            int year,
            int month,
            decimal additionalMeterValue,
            DateTime readingDateCandidate)
        {
            if (additionalMeterValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(additionalMeterValue));
            }

            var readingDateInMonth = ClampToYearMonth(readingDateCandidate, year, month);

            const string selectSql = @"SELECT Id, MeterValue, ReadingDate
                          FROM ChildMeterReadings
                          WHERE FloorId = @FloorId
                            AND ChildMeterId = @ChildMeterId
                            AND Type = N'電気'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";

            var existing = new List<(int Id, decimal MeterValue, DateTime ReadingDate)>();
            using (var selectCmd = new SqlCommand(selectSql, connection, transaction))
            {
                selectCmd.Parameters.AddWithValue("@FloorId", floorId);
                selectCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
                selectCmd.Parameters.AddWithValue("@Year", year);
                selectCmd.Parameters.AddWithValue("@Month", month);
                using var reader = await selectCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    existing.Add((
                        reader.GetInt32(reader.GetOrdinal("Id")),
                        reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                        reader.GetDateTime(reader.GetOrdinal("ReadingDate"))));
                }
            }

            if (existing.Count == 0)
            {
                var insertSql = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, N'電気', @MeterValue,
                                  GETDATE(), GETDATE())";
                using var insertCmd = new SqlCommand(insertSql, connection, transaction);
                insertCmd.Parameters.AddWithValue("@FloorId", floorId);
                insertCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
                insertCmd.Parameters.AddWithValue("@ReadingDate", readingDateInMonth);
                insertCmd.Parameters.AddWithValue("@MeterValue", additionalMeterValue);
                await insertCmd.ExecuteNonQueryAsync();
                return;
            }

            var totalValue = existing.Sum(x => x.MeterValue) + additionalMeterValue;
            var maxReadingDate = existing.Max(x => x.ReadingDate);
            if (readingDateInMonth > maxReadingDate)
            {
                maxReadingDate = readingDateInMonth;
            }

            foreach (var row in existing)
            {
                var delSql = "DELETE FROM ChildMeterReadings WHERE Id = @Id";
                using var delCmd = new SqlCommand(delSql, connection, transaction);
                delCmd.Parameters.AddWithValue("@Id", row.Id);
                await delCmd.ExecuteNonQueryAsync();
            }

            var insertMergedSql = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, N'電気', @MeterValue,
                                  GETDATE(), GETDATE())";
            using var insertMergedCmd = new SqlCommand(insertMergedSql, connection, transaction);
            insertMergedCmd.Parameters.AddWithValue("@FloorId", floorId);
            insertMergedCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
            insertMergedCmd.Parameters.AddWithValue("@ReadingDate", maxReadingDate);
            insertMergedCmd.Parameters.AddWithValue("@MeterValue", totalValue);
            await insertMergedCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 水道子メーター検針（同一フロア・子メーター・年月）にメーター値を加算する。
        /// 既存が複数行の場合は値を合算して1行に統合する。
        /// </summary>
        public static async Task UpsertAddWaterChildMeterReadingValueAsync(
            SqlConnection connection,
            SqlTransaction? transaction,
            int floorId,
            int childMeterId,
            int year,
            int month,
            decimal additionalMeterValue,
            DateTime readingDateCandidate)
        {
            if (additionalMeterValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(additionalMeterValue));
            }

            var readingDateInMonth = ClampToYearMonth(readingDateCandidate, year, month);

            const string selectSql = @"SELECT Id, MeterValue, ReadingDate
                          FROM ChildMeterReadings
                          WHERE FloorId = @FloorId
                            AND ChildMeterId = @ChildMeterId
                            AND Type = N'水道'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";

            var existing = new List<(int Id, decimal MeterValue, DateTime ReadingDate)>();
            using (var selectCmd = new SqlCommand(selectSql, connection, transaction))
            {
                selectCmd.Parameters.AddWithValue("@FloorId", floorId);
                selectCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
                selectCmd.Parameters.AddWithValue("@Year", year);
                selectCmd.Parameters.AddWithValue("@Month", month);
                using var reader = await selectCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    existing.Add((
                        reader.GetInt32(reader.GetOrdinal("Id")),
                        reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                        reader.GetDateTime(reader.GetOrdinal("ReadingDate"))));
                }
            }

            if (existing.Count == 0)
            {
                var insertSql = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, N'水道', @MeterValue,
                                  GETDATE(), GETDATE())";
                using var insertCmd = new SqlCommand(insertSql, connection, transaction);
                insertCmd.Parameters.AddWithValue("@FloorId", floorId);
                insertCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
                insertCmd.Parameters.AddWithValue("@ReadingDate", readingDateInMonth);
                insertCmd.Parameters.AddWithValue("@MeterValue", additionalMeterValue);
                await insertCmd.ExecuteNonQueryAsync();
                return;
            }

            var totalValue = existing.Sum(x => x.MeterValue) + additionalMeterValue;
            var maxReadingDate = existing.Max(x => x.ReadingDate);
            if (readingDateInMonth > maxReadingDate)
            {
                maxReadingDate = readingDateInMonth;
            }

            foreach (var row in existing)
            {
                var delSql = "DELETE FROM ChildMeterReadings WHERE Id = @Id";
                using var delCmd = new SqlCommand(delSql, connection, transaction);
                delCmd.Parameters.AddWithValue("@Id", row.Id);
                await delCmd.ExecuteNonQueryAsync();
            }

            var insertMergedSql = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, N'水道', @MeterValue,
                                  GETDATE(), GETDATE())";
            using var insertMergedCmd = new SqlCommand(insertMergedSql, connection, transaction);
            insertMergedCmd.Parameters.AddWithValue("@FloorId", floorId);
            insertMergedCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
            insertMergedCmd.Parameters.AddWithValue("@ReadingDate", maxReadingDate);
            insertMergedCmd.Parameters.AddWithValue("@MeterValue", totalValue);
            await insertMergedCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 水道子メーター検針（同一フロア・子メーター・年月）をCSV値で置き換える。
        /// 既存が複数行の場合は削除して1行に統合する。
        /// </summary>
        public static async Task UpsertReplaceWaterChildMeterReadingValueAsync(
            SqlConnection connection,
            SqlTransaction? transaction,
            int floorId,
            int childMeterId,
            int year,
            int month,
            decimal meterValue,
            DateTime readingDateCandidate)
        {
            if (meterValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(meterValue));
            }

            var readingDateInMonth = ClampToYearMonth(readingDateCandidate, year, month);

            const string selectSql = @"SELECT Id
                          FROM ChildMeterReadings
                          WHERE FloorId = @FloorId
                            AND ChildMeterId = @ChildMeterId
                            AND Type = N'水道'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";

            var existingIds = new List<int>();
            using (var selectCmd = new SqlCommand(selectSql, connection, transaction))
            {
                selectCmd.Parameters.AddWithValue("@FloorId", floorId);
                selectCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
                selectCmd.Parameters.AddWithValue("@Year", year);
                selectCmd.Parameters.AddWithValue("@Month", month);
                using var reader = await selectCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    existingIds.Add(reader.GetInt32(reader.GetOrdinal("Id")));
                }
            }

            foreach (var id in existingIds)
            {
                var delSql = "DELETE FROM ChildMeterReadings WHERE Id = @Id";
                using var delCmd = new SqlCommand(delSql, connection, transaction);
                delCmd.Parameters.AddWithValue("@Id", id);
                await delCmd.ExecuteNonQueryAsync();
            }

            var insertSql = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, N'水道', @MeterValue,
                                  GETDATE(), GETDATE())";
            using var insertCmd = new SqlCommand(insertSql, connection, transaction);
            insertCmd.Parameters.AddWithValue("@FloorId", floorId);
            insertCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
            insertCmd.Parameters.AddWithValue("@ReadingDate", readingDateInMonth);
            insertCmd.Parameters.AddWithValue("@MeterValue", meterValue);
            await insertCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// ガス子メーター検針（同一フロア・子メーター・年月）にメーター値を加算する。
        /// 既存が複数行の場合は値を合算して1行に統合する。
        /// </summary>
        public static async Task UpsertAddGasChildMeterReadingValueAsync(
            SqlConnection connection,
            SqlTransaction? transaction,
            int floorId,
            int childMeterId,
            int year,
            int month,
            decimal additionalMeterValue,
            DateTime readingDateCandidate)
        {
            if (additionalMeterValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(additionalMeterValue));
            }

            var readingDateInMonth = ClampToYearMonth(readingDateCandidate, year, month);

            const string selectSql = @"SELECT Id, MeterValue, ReadingDate
                          FROM ChildMeterReadings
                          WHERE FloorId = @FloorId
                            AND ChildMeterId = @ChildMeterId
                            AND Type = N'ガス'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";

            var existing = new List<(int Id, decimal MeterValue, DateTime ReadingDate)>();
            using (var selectCmd = new SqlCommand(selectSql, connection, transaction))
            {
                selectCmd.Parameters.AddWithValue("@FloorId", floorId);
                selectCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
                selectCmd.Parameters.AddWithValue("@Year", year);
                selectCmd.Parameters.AddWithValue("@Month", month);
                using var reader = await selectCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    existing.Add((
                        reader.GetInt32(reader.GetOrdinal("Id")),
                        reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                        reader.GetDateTime(reader.GetOrdinal("ReadingDate"))));
                }
            }

            if (existing.Count == 0)
            {
                var insertSql = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, N'ガス', @MeterValue,
                                  GETDATE(), GETDATE())";
                using var insertCmd = new SqlCommand(insertSql, connection, transaction);
                insertCmd.Parameters.AddWithValue("@FloorId", floorId);
                insertCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
                insertCmd.Parameters.AddWithValue("@ReadingDate", readingDateInMonth);
                insertCmd.Parameters.AddWithValue("@MeterValue", additionalMeterValue);
                await insertCmd.ExecuteNonQueryAsync();
                return;
            }

            var totalValue = existing.Sum(x => x.MeterValue) + additionalMeterValue;
            var maxReadingDate = existing.Max(x => x.ReadingDate);
            if (readingDateInMonth > maxReadingDate)
            {
                maxReadingDate = readingDateInMonth;
            }

            foreach (var row in existing)
            {
                var delSql = "DELETE FROM ChildMeterReadings WHERE Id = @Id";
                using var delCmd = new SqlCommand(delSql, connection, transaction);
                delCmd.Parameters.AddWithValue("@Id", row.Id);
                await delCmd.ExecuteNonQueryAsync();
            }

            var insertMergedSql = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, N'ガス', @MeterValue,
                                  GETDATE(), GETDATE())";
            using var insertMergedCmd = new SqlCommand(insertMergedSql, connection, transaction);
            insertMergedCmd.Parameters.AddWithValue("@FloorId", floorId);
            insertMergedCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
            insertMergedCmd.Parameters.AddWithValue("@ReadingDate", maxReadingDate);
            insertMergedCmd.Parameters.AddWithValue("@MeterValue", totalValue);
            await insertMergedCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// ガス子メーター検針（同一フロア・子メーター・年月）をCSV値で置き換える。
        /// 既存が複数行の場合は削除して1行に統合する。
        /// </summary>
        public static async Task UpsertReplaceGasChildMeterReadingValueAsync(
            SqlConnection connection,
            SqlTransaction? transaction,
            int floorId,
            int childMeterId,
            int year,
            int month,
            decimal meterValue,
            DateTime readingDateCandidate)
        {
            if (meterValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(meterValue));
            }

            var readingDateInMonth = ClampToYearMonth(readingDateCandidate, year, month);

            const string selectSql = @"SELECT Id
                          FROM ChildMeterReadings
                          WHERE FloorId = @FloorId
                            AND ChildMeterId = @ChildMeterId
                            AND Type = N'ガス'
                            AND YEAR(ReadingDate) = @Year
                            AND MONTH(ReadingDate) = @Month";

            var existingIds = new List<int>();
            using (var selectCmd = new SqlCommand(selectSql, connection, transaction))
            {
                selectCmd.Parameters.AddWithValue("@FloorId", floorId);
                selectCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
                selectCmd.Parameters.AddWithValue("@Year", year);
                selectCmd.Parameters.AddWithValue("@Month", month);
                using var reader = await selectCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    existingIds.Add(reader.GetInt32(reader.GetOrdinal("Id")));
                }
            }

            foreach (var id in existingIds)
            {
                var delSql = "DELETE FROM ChildMeterReadings WHERE Id = @Id";
                using var delCmd = new SqlCommand(delSql, connection, transaction);
                delCmd.Parameters.AddWithValue("@Id", id);
                await delCmd.ExecuteNonQueryAsync();
            }

            var insertSql = @"INSERT INTO ChildMeterReadings (FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                              CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, @ReadingDate, N'ガス', @MeterValue,
                                  GETDATE(), GETDATE())";
            using var insertCmd = new SqlCommand(insertSql, connection, transaction);
            insertCmd.Parameters.AddWithValue("@FloorId", floorId);
            insertCmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
            insertCmd.Parameters.AddWithValue("@ReadingDate", readingDateInMonth);
            insertCmd.Parameters.AddWithValue("@MeterValue", meterValue);
            await insertCmd.ExecuteNonQueryAsync();
        }

        private static DateTime ClampToYearMonth(DateTime candidate, int year, int month)
        {
            if (candidate.Year == year && candidate.Month == month)
            {
                return candidate;
            }

            return new DateTime(year, month, DateTime.DaysInMonth(year, month), candidate.Hour, candidate.Minute, candidate.Second, candidate.Kind);
        }

        public static async Task<List<(ChildMeterReading Reading, string BuildingName, string RoomName)>> GetElectricChildMeterReadingsByBuildingRoomAndMonthsAsync(
            string buildingName,
            string roomName,
            int year1,
            int month1,
            int year2,
            int month2)
        {
            var readings = new List<(ChildMeterReading Reading, string BuildingName, string RoomName)>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT r.Id, r.FloorId, r.ChildMeterId, r.ReadingDate, r.Type, r.MeterValue,
                                 r.CreatedAt, r.UpdatedAt,
                                 b.Name AS BuildingName, f.FloorName AS RoomName
                          FROM ChildMeterReadings r
                          INNER JOIN Floors f ON r.FloorId = f.Id
                          INNER JOIN Buildings b ON f.BuildingId = b.Id
                          WHERE b.Name = @BuildingName
                            AND f.FloorName = @RoomName
                            AND r.Type = @Type
                            AND (
                                (YEAR(r.ReadingDate) = @Year1 AND MONTH(r.ReadingDate) = @Month1)
                                OR
                                (YEAR(r.ReadingDate) = @Year2 AND MONTH(r.ReadingDate) = @Month2)
                            )
                          ORDER BY r.ReadingDate DESC, r.Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingName", buildingName);
            cmd.Parameters.AddWithValue("@RoomName", roomName);
            cmd.Parameters.AddWithValue("@Type", "電気");
            cmd.Parameters.AddWithValue("@Year1", year1);
            cmd.Parameters.AddWithValue("@Month1", month1);
            cmd.Parameters.AddWithValue("@Year2", year2);
            cmd.Parameters.AddWithValue("@Month2", month2);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add((
                    new ChildMeterReading
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                        ChildMeterId = reader.IsDBNull(reader.GetOrdinal("ChildMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                        ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                        Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                        MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                    },
                    reader.IsDBNull(reader.GetOrdinal("BuildingName")) ? string.Empty : reader.GetString(reader.GetOrdinal("BuildingName")),
                    reader.IsDBNull(reader.GetOrdinal("RoomName")) ? string.Empty : reader.GetString(reader.GetOrdinal("RoomName"))
                ));
            }

            return readings;
        }

        public static async Task<List<ChildMeterReading>> GetElectricChildMeterReadingsByBuildingAndMonthsAsync(
            string buildingName,
            int year1,
            int month1,
            int year2,
            int month2)
        {
            var readings = new List<ChildMeterReading>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT r.Id, r.FloorId, r.ChildMeterId, r.ReadingDate, r.Type, r.MeterValue,
                                 r.CreatedAt, r.UpdatedAt
                          FROM ChildMeterReadings r
                          INNER JOIN Floors f ON r.FloorId = f.Id
                          INNER JOIN Buildings b ON f.BuildingId = b.Id
                          WHERE b.Name = @BuildingName
                            AND r.Type = @Type
                            AND (
                                (YEAR(r.ReadingDate) = @Year1 AND MONTH(r.ReadingDate) = @Month1)
                                OR
                                (YEAR(r.ReadingDate) = @Year2 AND MONTH(r.ReadingDate) = @Month2)
                            )
                          ORDER BY r.ReadingDate DESC, r.Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingName", buildingName);
            cmd.Parameters.AddWithValue("@Type", "電気");
            cmd.Parameters.AddWithValue("@Year1", year1);
            cmd.Parameters.AddWithValue("@Month1", month1);
            cmd.Parameters.AddWithValue("@Year2", year2);
            cmd.Parameters.AddWithValue("@Month2", month2);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add(new ChildMeterReading
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.IsDBNull(reader.GetOrdinal("ChildMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                    MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return readings;
        }

        public static async Task<List<(ChildMeterReading Reading, string BuildingName, string RoomName)>> GetWaterChildMeterReadingsByChildMeterNameAndMonthsAsync(
            string childMeterName,
            int year1,
            int month1,
            int year2,
            int month2)
        {
            var readings = new List<(ChildMeterReading Reading, string BuildingName, string RoomName)>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT r.Id, r.FloorId, r.ChildMeterId, r.ReadingDate, r.Type, r.MeterValue,
                                 r.CreatedAt, r.UpdatedAt,
                                 b.Name AS BuildingName, f.FloorName AS RoomName
                          FROM ChildMeterReadings r
                          INNER JOIN ChildMeters cm ON r.ChildMeterId = cm.Id
                          INNER JOIN Floors f ON r.FloorId = f.Id
                          INNER JOIN Buildings b ON f.BuildingId = b.Id
                          WHERE cm.MeterName = @ChildMeterName
                            AND r.Type = @Type
                            AND (
                                (YEAR(r.ReadingDate) = @Year1 AND MONTH(r.ReadingDate) = @Month1)
                                OR
                                (YEAR(r.ReadingDate) = @Year2 AND MONTH(r.ReadingDate) = @Month2)
                            )
                          ORDER BY r.ReadingDate DESC, r.Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ChildMeterName", childMeterName);
            cmd.Parameters.AddWithValue("@Type", "水道");
            cmd.Parameters.AddWithValue("@Year1", year1);
            cmd.Parameters.AddWithValue("@Month1", month1);
            cmd.Parameters.AddWithValue("@Year2", year2);
            cmd.Parameters.AddWithValue("@Month2", month2);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add((
                    new ChildMeterReading
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                        ChildMeterId = reader.IsDBNull(reader.GetOrdinal("ChildMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                        ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                        Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                        MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                    },
                    reader.IsDBNull(reader.GetOrdinal("BuildingName")) ? string.Empty : reader.GetString(reader.GetOrdinal("BuildingName")),
                    reader.IsDBNull(reader.GetOrdinal("RoomName")) ? string.Empty : reader.GetString(reader.GetOrdinal("RoomName"))
                ));
            }

            return readings;
        }

        public static async Task<List<ChildMeterReading>> GetWaterChildMeterReadingsByChildMeterIdAndMonthsAsync(
            int childMeterId,
            int year1,
            int month1,
            int year2,
            int month2)
        {
            var readings = new List<ChildMeterReading>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT r.Id, r.FloorId, r.ChildMeterId, r.ReadingDate, r.Type, r.MeterValue,
                                 r.CreatedAt, r.UpdatedAt
                          FROM ChildMeterReadings r
                          WHERE r.ChildMeterId = @ChildMeterId
                            AND r.Type = @Type
                            AND (
                                (YEAR(r.ReadingDate) = @Year1 AND MONTH(r.ReadingDate) = @Month1)
                                OR
                                (YEAR(r.ReadingDate) = @Year2 AND MONTH(r.ReadingDate) = @Month2)
                            )
                          ORDER BY r.ReadingDate DESC, r.Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
            cmd.Parameters.AddWithValue("@Type", "水道");
            cmd.Parameters.AddWithValue("@Year1", year1);
            cmd.Parameters.AddWithValue("@Month1", month1);
            cmd.Parameters.AddWithValue("@Year2", year2);
            cmd.Parameters.AddWithValue("@Month2", month2);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add(new ChildMeterReading
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.IsDBNull(reader.GetOrdinal("ChildMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                    MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return readings;
        }

        /// <summary>
        /// 指定した3ヶ月分の水道子メーター検針データを取得する。
        /// </summary>
        public static async Task<List<ChildMeterReading>> GetWaterChildMeterReadingsByThreeMonthsAsync(
            int year1,
            int month1,
            int year2,
            int month2,
            int year3,
            int month3)
        {
            var readings = new List<ChildMeterReading>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"SELECT Id, FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                          CreatedAt, UpdatedAt
                                   FROM ChildMeterReadings
                                   WHERE Type = N'水道'
                                     AND ChildMeterId IS NOT NULL
                                     AND (
                                         (YEAR(ReadingDate) = @Year1 AND MONTH(ReadingDate) = @Month1)
                                         OR (YEAR(ReadingDate) = @Year2 AND MONTH(ReadingDate) = @Month2)
                                         OR (YEAR(ReadingDate) = @Year3 AND MONTH(ReadingDate) = @Month3)
                                     )
                                   ORDER BY ChildMeterId, ReadingDate DESC, Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year1", year1);
            cmd.Parameters.AddWithValue("@Month1", month1);
            cmd.Parameters.AddWithValue("@Year2", year2);
            cmd.Parameters.AddWithValue("@Month2", month2);
            cmd.Parameters.AddWithValue("@Year3", year3);
            cmd.Parameters.AddWithValue("@Month3", month3);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add(new ChildMeterReading
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                    MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return readings;
        }

        /// <summary>
        /// 指定した3ヶ月分の電気子メーター検針データを取得する。
        /// </summary>
        public static async Task<List<ChildMeterReading>> GetElectricChildMeterReadingsByThreeMonthsAsync(
            int year1,
            int month1,
            int year2,
            int month2,
            int year3,
            int month3)
        {
            var readings = new List<ChildMeterReading>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"SELECT Id, FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                          CreatedAt, UpdatedAt
                                   FROM ChildMeterReadings
                                   WHERE Type = N'電気'
                                     AND ChildMeterId IS NOT NULL
                                     AND (
                                         (YEAR(ReadingDate) = @Year1 AND MONTH(ReadingDate) = @Month1)
                                         OR (YEAR(ReadingDate) = @Year2 AND MONTH(ReadingDate) = @Month2)
                                         OR (YEAR(ReadingDate) = @Year3 AND MONTH(ReadingDate) = @Month3)
                                     )
                                   ORDER BY FloorId, ChildMeterId, ReadingDate DESC, Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year1", year1);
            cmd.Parameters.AddWithValue("@Month1", month1);
            cmd.Parameters.AddWithValue("@Year2", year2);
            cmd.Parameters.AddWithValue("@Month2", month2);
            cmd.Parameters.AddWithValue("@Year3", year3);
            cmd.Parameters.AddWithValue("@Month3", month3);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add(new ChildMeterReading
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                    MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return readings;
        }

        /// <summary>
        /// 指定した3ヶ月分のガス子メーター検針データを取得する。
        /// </summary>
        public static async Task<List<ChildMeterReading>> GetGasChildMeterReadingsByThreeMonthsAsync(
            int year1,
            int month1,
            int year2,
            int month2,
            int year3,
            int month3)
        {
            var readings = new List<ChildMeterReading>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            const string query = @"SELECT Id, FloorId, ChildMeterId, ReadingDate, Type, MeterValue,
                                          CreatedAt, UpdatedAt
                                   FROM ChildMeterReadings
                                   WHERE Type = N'ガス'
                                     AND ChildMeterId IS NOT NULL
                                     AND (
                                         (YEAR(ReadingDate) = @Year1 AND MONTH(ReadingDate) = @Month1)
                                         OR (YEAR(ReadingDate) = @Year2 AND MONTH(ReadingDate) = @Month2)
                                         OR (YEAR(ReadingDate) = @Year3 AND MONTH(ReadingDate) = @Month3)
                                     )
                                   ORDER BY FloorId, ChildMeterId, ReadingDate DESC, Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year1", year1);
            cmd.Parameters.AddWithValue("@Month1", month1);
            cmd.Parameters.AddWithValue("@Year2", year2);
            cmd.Parameters.AddWithValue("@Month2", month2);
            cmd.Parameters.AddWithValue("@Year3", year3);
            cmd.Parameters.AddWithValue("@Month3", month3);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add(new ChildMeterReading
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                    MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return readings;
        }

        public static async Task<List<(ChildMeterReading Reading, string BuildingName, string RoomName)>> GetGasChildMeterReadingsByChildMeterNameAndMonthsAsync(
            string childMeterName,
            int year1,
            int month1,
            int year2,
            int month2)
        {
            var readings = new List<(ChildMeterReading Reading, string BuildingName, string RoomName)>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT r.Id, r.FloorId, r.ChildMeterId, r.ReadingDate, r.Type, r.MeterValue,
                                 r.CreatedAt, r.UpdatedAt,
                                 b.Name AS BuildingName, f.FloorName AS RoomName
                          FROM ChildMeterReadings r
                          INNER JOIN ChildMeters cm ON r.ChildMeterId = cm.Id
                          INNER JOIN Floors f ON r.FloorId = f.Id
                          INNER JOIN Buildings b ON f.BuildingId = b.Id
                          WHERE cm.MeterName = @ChildMeterName
                            AND r.Type = @Type
                            AND (
                                (YEAR(r.ReadingDate) = @Year1 AND MONTH(r.ReadingDate) = @Month1)
                                OR
                                (YEAR(r.ReadingDate) = @Year2 AND MONTH(r.ReadingDate) = @Month2)
                            )
                          ORDER BY r.ReadingDate DESC, r.Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ChildMeterName", childMeterName);
            cmd.Parameters.AddWithValue("@Type", "ガス");
            cmd.Parameters.AddWithValue("@Year1", year1);
            cmd.Parameters.AddWithValue("@Month1", month1);
            cmd.Parameters.AddWithValue("@Year2", year2);
            cmd.Parameters.AddWithValue("@Month2", month2);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                readings.Add((
                    new ChildMeterReading
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                        ChildMeterId = reader.IsDBNull(reader.GetOrdinal("ChildMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                        ReadingDate = reader.GetDateTime(reader.GetOrdinal("ReadingDate")),
                        Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
                        MeterValue = reader.GetDecimal(reader.GetOrdinal("MeterValue")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                    },
                    reader.IsDBNull(reader.GetOrdinal("BuildingName")) ? string.Empty : reader.GetString(reader.GetOrdinal("BuildingName")),
                    reader.IsDBNull(reader.GetOrdinal("RoomName")) ? string.Empty : reader.GetString(reader.GetOrdinal("RoomName"))
                ));
            }

            return readings;
        }
    }
}

