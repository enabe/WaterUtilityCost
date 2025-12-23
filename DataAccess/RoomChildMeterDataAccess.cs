using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

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
    }
}





