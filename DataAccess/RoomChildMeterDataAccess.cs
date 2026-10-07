using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;
using System.Data;

namespace WaterUtilityCost.DataAccess
{
    public class RoomChildMeterDataAccess
    {
        public static async Task<List<RoomChildMeter>> GetAllRoomChildMetersAsync()
        {
            var roomChildMeters = new List<RoomChildMeter>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, FloorId, ChildMeterId, CreatedAt, UpdatedAt
                          FROM RoomChildMeters ORDER BY Id DESC";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                roomChildMeters.Add(new RoomChildMeter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return roomChildMeters;
        }

        public static async Task<RoomChildMeter?> GetRoomChildMeterByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, FloorId, ChildMeterId, CreatedAt, UpdatedAt
                          FROM RoomChildMeters WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RoomChildMeter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        public static async Task<RoomChildMeter?> GetRoomChildMeterByChildMeterIdAsync(int childMeterId)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT TOP 1 Id, FloorId, ChildMeterId, CreatedAt, UpdatedAt
                          FROM RoomChildMeters
                          WHERE ChildMeterId = @ChildMeterId
                          ORDER BY Id DESC";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ChildMeterId", childMeterId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RoomChildMeter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FloorId = reader.GetInt32(reader.GetOrdinal("FloorId")),
                    ChildMeterId = reader.GetInt32(reader.GetOrdinal("ChildMeterId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }

            return null;
        }

        public static async Task<int> CreateRoomChildMeterAsync(RoomChildMeter roomChildMeter)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO RoomChildMeters (FloorId, ChildMeterId, CreatedAt, UpdatedAt)
                          VALUES (@FloorId, @ChildMeterId, GETDATE(), GETDATE());
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@FloorId", roomChildMeter.FloorId);
            cmd.Parameters.AddWithValue("@ChildMeterId", roomChildMeter.ChildMeterId);
            return (int)await cmd.ExecuteScalarAsync();
        }

        public static async Task<bool> UpdateRoomChildMeterAsync(RoomChildMeter roomChildMeter)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE RoomChildMeters SET 
                            FloorId = @FloorId, 
                            ChildMeterId = @ChildMeterId,
                            UpdatedAt = GETDATE()
                          WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", roomChildMeter.Id);
            cmd.Parameters.AddWithValue("@FloorId", roomChildMeter.FloorId);
            cmd.Parameters.AddWithValue("@ChildMeterId", roomChildMeter.ChildMeterId);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteRoomChildMeterAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM RoomChildMeters WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task ReplaceRoomChildMetersAsync(IEnumerable<RoomChildMeter> roomChildMeters)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using (var deleteCmd = new SqlCommand("DELETE FROM RoomChildMeters", connection, transaction))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                using (var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('[dbo].[RoomChildMeters]', RESEED, 0);", connection, transaction))
                {
                    await reseedCmd.ExecuteNonQueryAsync();
                }

                var insertQuery = @"INSERT INTO RoomChildMeters (FloorId, ChildMeterId, CreatedAt, UpdatedAt)
                                    VALUES (@FloorId, @ChildMeterId, @CreatedAt, @UpdatedAt)";
                using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                insertCmd.Parameters.Add("@FloorId", SqlDbType.Int);
                insertCmd.Parameters.Add("@ChildMeterId", SqlDbType.Int);
                insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime);
                insertCmd.Parameters.Add("@UpdatedAt", SqlDbType.DateTime);

                var now = DateTime.Now;
                foreach (var roomChildMeter in roomChildMeters)
                {
                    insertCmd.Parameters["@FloorId"].Value = roomChildMeter.FloorId;
                    insertCmd.Parameters["@ChildMeterId"].Value = roomChildMeter.ChildMeterId;
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







