using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    }
}

