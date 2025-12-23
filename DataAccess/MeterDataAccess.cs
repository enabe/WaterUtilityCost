using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    public class MeterDataAccess
    {
        public static async Task<List<Meter>> GetAllMetersAsync()
        {
            var meters = new List<Meter>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, BuildingId, ContractorId, MeterType, ManagementNumber,
                                CreatedAt, UpdatedAt
                          FROM Meters ORDER BY Id DESC";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var contractorIdOrdinal = reader.GetOrdinal("ContractorId");
                meters.Add(new Meter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    ContractorId = reader.IsDBNull(contractorIdOrdinal) ? (int?)null : reader.GetInt32(contractorIdOrdinal),
                    MeterId = string.Empty,
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
                    ManagementNumber = reader.IsDBNull(reader.GetOrdinal("ManagementNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("ManagementNumber")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return meters;
        }

        public static async Task<Meter?> GetMeterByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, BuildingId, ContractorId, MeterType, ManagementNumber,
                                CreatedAt, UpdatedAt
                          FROM Meters WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var contractorIdOrdinal = reader.GetOrdinal("ContractorId");
                return new Meter
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    ContractorId = reader.IsDBNull(contractorIdOrdinal) ? (int?)null : reader.GetInt32(contractorIdOrdinal),
                    MeterId = string.Empty,
                    MeterType = reader.IsDBNull(reader.GetOrdinal("MeterType")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterType")),
                    ManagementNumber = reader.IsDBNull(reader.GetOrdinal("ManagementNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("ManagementNumber")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        public static async Task<int> CreateMeterAsync(Meter meter)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO Meters (BuildingId, ContractorId, MeterType, ManagementNumber,
                                              CreatedAt, UpdatedAt)
                          VALUES (@BuildingId, @ContractorId, @MeterType, @ManagementNumber,
                                  @CreatedAt, @UpdatedAt);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", (object)meter.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractorId", (object)meter.ContractorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterType", string.IsNullOrEmpty(meter.MeterType) ? DBNull.Value : (object)meter.MeterType);
            cmd.Parameters.AddWithValue("@ManagementNumber", string.IsNullOrEmpty(meter.ManagementNumber) ? DBNull.Value : (object)meter.ManagementNumber);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            return (int)await cmd.ExecuteScalarAsync();
        }

        public static async Task<bool> UpdateMeterAsync(Meter meter)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE Meters SET 
                            BuildingId = @BuildingId, 
                            ContractorId = @ContractorId, 
                            MeterType = @MeterType, 
                            ManagementNumber = @ManagementNumber,
                            UpdatedAt = @UpdatedAt
                          WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", meter.Id);
            cmd.Parameters.AddWithValue("@BuildingId", (object)meter.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractorId", (object)meter.ContractorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterType", string.IsNullOrEmpty(meter.MeterType) ? DBNull.Value : (object)meter.MeterType);
            cmd.Parameters.AddWithValue("@ManagementNumber", string.IsNullOrEmpty(meter.ManagementNumber) ? DBNull.Value : (object)meter.ManagementNumber);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteMeterAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM Meters WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}

















