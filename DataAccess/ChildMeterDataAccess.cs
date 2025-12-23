using System;
using System.Collections.Generic;
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
                                BuildingId, RoomId, MeterType,
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
                    RoomId = reader.IsDBNull(reader.GetOrdinal("RoomId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomId")),
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
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
                                BuildingId, RoomId, MeterType,
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
                    RoomId = reader.IsDBNull(reader.GetOrdinal("RoomId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomId")),
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
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
            var query = $@"INSERT INTO ChildMeters ([{columnName}], BuildingId, RoomId, MeterType, Notes,
                                              CreatedAt, UpdatedAt)
                          VALUES (@ParentMeterId, @BuildingId, @RoomId, @MeterType, @Notes,
                                  GETDATE(), GETDATE());
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ParentMeterId", (object)childMeter.ParentMeterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingId", (object)childMeter.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomId", (object)childMeter.RoomId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterType", (object)childMeter.MeterType ?? DBNull.Value);
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
                            RoomId = @RoomId,
                            MeterType = @MeterType,
                            Notes = @Notes,
                            UpdatedAt = GETDATE()
                          WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", childMeter.Id);
            cmd.Parameters.AddWithValue("@ParentMeterId", (object)childMeter.ParentMeterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingId", (object)childMeter.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomId", (object)childMeter.RoomId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterType", (object)childMeter.MeterType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object)childMeter.Notes ?? DBNull.Value);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<ChildMeter?> GetChildMeterByBuildingRoomAndTypeAsync(int buildingId, int roomId, string meterType)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            var columnName = await GetParentMeterIdColumnNameAsync(connection);
            var query = $@"SELECT Id, [{columnName}] AS ParentMeterId, 
                                BuildingId, RoomId, MeterType,
                                Notes, CreatedAt, UpdatedAt
                          FROM ChildMeters 
                          WHERE BuildingId = @BuildingId 
                            AND RoomId = @RoomId 
                            AND MeterType = @MeterType";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", buildingId);
            cmd.Parameters.AddWithValue("@RoomId", roomId);
            cmd.Parameters.AddWithValue("@MeterType", meterType);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ChildMeter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ParentMeterId = reader.IsDBNull(reader.GetOrdinal("ParentMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentMeterId")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    RoomId = reader.IsDBNull(reader.GetOrdinal("RoomId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomId")),
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
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
    }
}



