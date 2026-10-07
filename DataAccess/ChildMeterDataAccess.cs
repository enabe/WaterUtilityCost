using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    public class ChildMeterDataAccess
    {
        private static string? _parentMeterIdColumnName = null;

        private static async Task<string> GetParentMeterIdColumnNameAsync(SqlConnection connection)
        {
            if (_parentMeterIdColumnName != null)
                return _parentMeterIdColumnName;

            // 実際のデータベースの列名を確認（すべての列名を取得して該当するものを探す）
            var query = @"SELECT name FROM sys.columns 
                          WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]')";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var columnName = reader.GetString(0);
                // 大文字小文字を区別せずに比較
                if (string.Equals(columnName, "ParentMeterId", StringComparison.OrdinalIgnoreCase))
                {
                    _parentMeterIdColumnName = columnName; // 実際の列名をそのまま使用
                    break;
                }
            }
            
            // 見つからない場合、列が存在しない可能性がある
            // この場合、デフォルト値を使用（マイグレーションが実行されれば列が追加される）
            if (_parentMeterIdColumnName == null)
            {
                _parentMeterIdColumnName = "ParentMeterId";
            }
            
            return _parentMeterIdColumnName;
        }

        public static async Task<List<ChildMeter>> GetAllChildMetersAsync()
        {
            var childMeters = new List<ChildMeter>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            var columnName = await GetParentMeterIdColumnNameAsync(connection);
            var query = $@"SELECT Id, [{columnName}] AS ParentMeterId, 
                                BuildingId, MeterType, MeterName,
                                Notes, CreatedAt, UpdatedAt
                          FROM ChildMeters ORDER BY Id DESC";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                childMeters.Add(new ChildMeter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ParentMeterId = reader.IsDBNull(reader.GetOrdinal("ParentMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentMeterId")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
                    MeterName = reader.IsDBNull(reader.GetOrdinal("MeterName")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterName")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return childMeters;
        }

        public static async Task<ChildMeter?> GetChildMeterByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            var columnName = await GetParentMeterIdColumnNameAsync(connection);
            var query = $@"SELECT Id, [{columnName}] AS ParentMeterId, 
                                BuildingId, MeterType, MeterName,
                                Notes, CreatedAt, UpdatedAt
                          FROM ChildMeters WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ChildMeter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ParentMeterId = reader.IsDBNull(reader.GetOrdinal("ParentMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentMeterId")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
                    MeterName = reader.IsDBNull(reader.GetOrdinal("MeterName")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterName")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        public static async Task<int> CreateChildMeterAsync(ChildMeter childMeter)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            var columnName = await GetParentMeterIdColumnNameAsync(connection);
            var query = $@"INSERT INTO ChildMeters ([{columnName}], BuildingId, MeterType, MeterName, Notes,
                                              CreatedAt, UpdatedAt)
                          VALUES (@ParentMeterId, @BuildingId, @MeterType, @MeterName, @Notes,
                                  GETDATE(), GETDATE());
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ParentMeterId", (object)childMeter.ParentMeterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingId", (object)childMeter.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterType", (object)childMeter.MeterType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterName", string.IsNullOrEmpty(childMeter.MeterName) ? DBNull.Value : (object)childMeter.MeterName);
            cmd.Parameters.AddWithValue("@Notes", (object)childMeter.Notes ?? DBNull.Value);
            return (int)await cmd.ExecuteScalarAsync();
        }

        public static async Task<bool> UpdateChildMeterAsync(ChildMeter childMeter)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            var columnName = await GetParentMeterIdColumnNameAsync(connection);
            var query = $@"UPDATE ChildMeters SET 
                            [{columnName}] = @ParentMeterId, 
                            BuildingId = @BuildingId,
                            MeterType = @MeterType,
                            MeterName = @MeterName,
                            Notes = @Notes,
                            UpdatedAt = GETDATE()
                          WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", childMeter.Id);
            cmd.Parameters.AddWithValue("@ParentMeterId", (object)childMeter.ParentMeterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingId", (object)childMeter.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterType", (object)childMeter.MeterType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterName", string.IsNullOrEmpty(childMeter.MeterName) ? DBNull.Value : (object)childMeter.MeterName);
            cmd.Parameters.AddWithValue("@Notes", (object)childMeter.Notes ?? DBNull.Value);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<ChildMeter?> GetChildMeterByBuildingRoomAndTypeAsync(int buildingId, int roomId, string meterType)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            var columnName = await GetParentMeterIdColumnNameAsync(connection);
            var query = $@"SELECT Id, [{columnName}] AS ParentMeterId, 
                                BuildingId, MeterType, MeterName,
                                Notes, CreatedAt, UpdatedAt
                          FROM ChildMeters 
                          WHERE BuildingId = @BuildingId 
                            AND MeterType = @MeterType";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", buildingId);
            cmd.Parameters.AddWithValue("@MeterType", meterType);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ChildMeter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ParentMeterId = reader.IsDBNull(reader.GetOrdinal("ParentMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentMeterId")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
                    MeterName = reader.IsDBNull(reader.GetOrdinal("MeterName")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterName")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        public static async Task<List<ChildMeter>> GetChildMetersByBuildingAndTypeAsync(int buildingId, string meterType)
        {
            var childMeters = new List<ChildMeter>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var columnName = await GetParentMeterIdColumnNameAsync(connection);
            var query = $@"SELECT Id, [{columnName}] AS ParentMeterId, 
                                BuildingId, MeterType, MeterName,
                                Notes, CreatedAt, UpdatedAt
                          FROM ChildMeters
                          WHERE BuildingId = @BuildingId
                            AND MeterType = @MeterType
                          ORDER BY Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", buildingId);
            cmd.Parameters.AddWithValue("@MeterType", meterType);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                childMeters.Add(new ChildMeter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ParentMeterId = reader.IsDBNull(reader.GetOrdinal("ParentMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentMeterId")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
                    MeterName = reader.IsDBNull(reader.GetOrdinal("MeterName")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterName")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return childMeters;
        }

        public static async Task<bool> DeleteChildMeterAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            // トランザクションを使用して、関連レコードも一緒に削除
            using var transaction = connection.BeginTransaction();
            try
            {
                // まず、RoomChildMetersテーブルの関連レコードを削除
                // テーブルが存在するか確認してから削除
                var checkTableQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomChildMeters]') AND type in (N'U'))
                    BEGIN
                        SELECT COUNT(*) FROM RoomChildMeters WHERE ChildMeterId = @Id
                    END
                    ELSE
                    BEGIN
                        SELECT 0
                    END";
                using var cmdCheck = new SqlCommand(checkTableQuery, connection, transaction);
                cmdCheck.Parameters.AddWithValue("@Id", id);
                var count = (int)await cmdCheck.ExecuteScalarAsync();
                
                if (count > 0)
                {
                    var deleteRoomChildMetersQuery = "DELETE FROM RoomChildMeters WHERE ChildMeterId = @Id";
                    using var cmdRoomChildMeters = new SqlCommand(deleteRoomChildMetersQuery, connection, transaction);
                    cmdRoomChildMeters.Parameters.AddWithValue("@Id", id);
                    await cmdRoomChildMeters.ExecuteNonQueryAsync();
                }
                
                // 次に、ChildMetersテーブルのレコードを削除
                var query = "DELETE FROM ChildMeters WHERE Id = @Id";
                using var cmd = new SqlCommand(query, connection, transaction);
                cmd.Parameters.AddWithValue("@Id", id);
                var result = await cmd.ExecuteNonQueryAsync() > 0;
                
                // トランザクションをコミット
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                // エラーが発生した場合はロールバック
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    // ロールバックエラーは無視
                }
                throw new Exception($"子メーターの削除に失敗しました: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 子メーター情報を全削除してIDを初期化し、CSVデータで再登録する
        /// </summary>
        public static async Task ReplaceChildMetersAsync(IEnumerable<ChildMeter> childMeters)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var columnName = await GetParentMeterIdColumnNameAsync(connection);

            using var transaction = connection.BeginTransaction();
            try
            {
                var deleteReadingsQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') AND type in (N'U'))
                        DELETE FROM [dbo].[ChildMeterReadings];";
                using (var deleteReadingsCmd = new SqlCommand(deleteReadingsQuery, connection, transaction))
                {
                    await deleteReadingsCmd.ExecuteNonQueryAsync();
                }

                var deleteRoomChildMetersQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomChildMeters]') AND type in (N'U'))
                        DELETE FROM [dbo].[RoomChildMeters];";
                using (var deleteRoomChildMetersCmd = new SqlCommand(deleteRoomChildMetersQuery, connection, transaction))
                {
                    await deleteRoomChildMetersCmd.ExecuteNonQueryAsync();
                }

                using (var deleteCmd = new SqlCommand("DELETE FROM ChildMeters", connection, transaction))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                using (var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('[dbo].[ChildMeters]', RESEED, 0);", connection, transaction))
                {
                    await reseedCmd.ExecuteNonQueryAsync();
                }

                var insertQuery = $@"INSERT INTO ChildMeters ([{columnName}], BuildingId, MeterType, MeterName, Notes, CreatedAt, UpdatedAt)
                                    VALUES (@ParentMeterId, @BuildingId, @MeterType, @MeterName, @Notes, @CreatedAt, @UpdatedAt)";
                using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                insertCmd.Parameters.Add("@ParentMeterId", SqlDbType.Int);
                insertCmd.Parameters.Add("@BuildingId", SqlDbType.Int);
                insertCmd.Parameters.Add("@MeterType", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@MeterName", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 500);
                insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime);
                insertCmd.Parameters.Add("@UpdatedAt", SqlDbType.DateTime);

                var now = DateTime.Now;
                foreach (var childMeter in childMeters)
                {
                    insertCmd.Parameters["@ParentMeterId"].Value = childMeter.ParentMeterId.HasValue
                        ? childMeter.ParentMeterId.Value
                        : DBNull.Value;
                    insertCmd.Parameters["@BuildingId"].Value = childMeter.BuildingId.HasValue
                        ? childMeter.BuildingId.Value
                        : DBNull.Value;
                    insertCmd.Parameters["@MeterType"].Value = string.IsNullOrWhiteSpace(childMeter.MeterType)
                        ? DBNull.Value
                        : childMeter.MeterType;
                    insertCmd.Parameters["@MeterName"].Value = string.IsNullOrWhiteSpace(childMeter.MeterName)
                        ? DBNull.Value
                        : childMeter.MeterName;
                    insertCmd.Parameters["@Notes"].Value = string.IsNullOrWhiteSpace(childMeter.Notes)
                        ? DBNull.Value
                        : childMeter.Notes;
                    insertCmd.Parameters["@CreatedAt"].Value = now;
                    insertCmd.Parameters["@UpdatedAt"].Value = now;

                    await insertCmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}



