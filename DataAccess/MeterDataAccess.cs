using System;
using System.Collections.Generic;
using System.Data;
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
            var query = @"SELECT Id, BuildingId, ContractorId, MeterType, MeterName, ManagementNumber,
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
                    MeterName = reader.IsDBNull(reader.GetOrdinal("MeterName")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterName")),
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
            var query = @"SELECT Id, BuildingId, ContractorId, MeterType, MeterName, ManagementNumber,
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
                    MeterName = reader.IsDBNull(reader.GetOrdinal("MeterName")) ? string.Empty : reader.GetString(reader.GetOrdinal("MeterName")),
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
            var query = @"INSERT INTO Meters (BuildingId, ContractorId, MeterType, MeterName, ManagementNumber,
                                              CreatedAt, UpdatedAt)
                          VALUES (@BuildingId, @ContractorId, @MeterType, @MeterName, @ManagementNumber,
                                  @CreatedAt, @UpdatedAt);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", (object)meter.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractorId", (object)meter.ContractorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterType", string.IsNullOrEmpty(meter.MeterType) ? DBNull.Value : (object)meter.MeterType);
            cmd.Parameters.AddWithValue("@MeterName", string.IsNullOrEmpty(meter.MeterName) ? DBNull.Value : (object)meter.MeterName);
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
                            MeterName = @MeterName,
                            ManagementNumber = @ManagementNumber,
                            UpdatedAt = @UpdatedAt
                          WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", meter.Id);
            cmd.Parameters.AddWithValue("@BuildingId", (object)meter.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractorId", (object)meter.ContractorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeterType", string.IsNullOrEmpty(meter.MeterType) ? DBNull.Value : (object)meter.MeterType);
            cmd.Parameters.AddWithValue("@MeterName", string.IsNullOrEmpty(meter.MeterName) ? DBNull.Value : (object)meter.MeterName);
            cmd.Parameters.AddWithValue("@ManagementNumber", string.IsNullOrEmpty(meter.ManagementNumber) ? DBNull.Value : (object)meter.ManagementNumber);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteMeterAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var deleteRoomChildMetersQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomChildMeters]') AND type in (N'U'))
                    BEGIN
                        DELETE rcm
                        FROM RoomChildMeters rcm
                        INNER JOIN ChildMeters cm ON rcm.ChildMeterId = cm.Id
                        WHERE cm.ParentMeterId = @Id
                    END";
                using (var cmdRoomChildMeters = new SqlCommand(deleteRoomChildMetersQuery, connection, transaction))
                {
                    cmdRoomChildMeters.Parameters.AddWithValue("@Id", id);
                    await cmdRoomChildMeters.ExecuteNonQueryAsync();
                }

                var deleteChildMeterReadingsQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') AND type in (N'U'))
                    BEGIN
                        DELETE r
                        FROM ChildMeterReadings r
                        INNER JOIN ChildMeters cm ON r.ChildMeterId = cm.Id
                        WHERE cm.ParentMeterId = @Id
                    END";
                using (var cmdChildMeterReadings = new SqlCommand(deleteChildMeterReadingsQuery, connection, transaction))
                {
                    cmdChildMeterReadings.Parameters.AddWithValue("@Id", id);
                    await cmdChildMeterReadings.ExecuteNonQueryAsync();
                }

                var deleteChildMetersQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    BEGIN
                        DELETE FROM ChildMeters WHERE ParentMeterId = @Id
                    END";
                using (var cmdChildMeters = new SqlCommand(deleteChildMetersQuery, connection, transaction))
                {
                    cmdChildMeters.Parameters.AddWithValue("@Id", id);
                    await cmdChildMeters.ExecuteNonQueryAsync();
                }

                var deleteGasBillingsQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    BEGIN
                        DELETE FROM GasBillings WHERE ParentMeterId = @Id
                    END";
                using (var cmdGasBillings = new SqlCommand(deleteGasBillingsQuery, connection, transaction))
                {
                    cmdGasBillings.Parameters.AddWithValue("@Id", id);
                    await cmdGasBillings.ExecuteNonQueryAsync();
                }

                var deleteWaterBillingsQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    BEGIN
                        DELETE FROM WaterBillings WHERE ParentMeterId = @Id
                    END";
                using (var cmdWaterBillings = new SqlCommand(deleteWaterBillingsQuery, connection, transaction))
                {
                    cmdWaterBillings.Parameters.AddWithValue("@Id", id);
                    await cmdWaterBillings.ExecuteNonQueryAsync();
                }

                var deleteMeterQuery = "DELETE FROM Meters WHERE Id = @Id";
                using var cmd = new SqlCommand(deleteMeterQuery, connection, transaction);
                cmd.Parameters.AddWithValue("@Id", id);
                var affected = await cmd.ExecuteNonQueryAsync() > 0;

                transaction.Commit();
                return affected;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 親メーター情報を全削除してIDを初期化し、CSVデータで再登録する
        /// </summary>
        public static async Task ReplaceMetersAsync(IEnumerable<Meter> meters)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using (var deleteCmd = new SqlCommand("DELETE FROM Meters", connection, transaction))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                using (var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('[dbo].[Meters]', RESEED, 0);", connection, transaction))
                {
                    await reseedCmd.ExecuteNonQueryAsync();
                }

                var insertQuery = @"INSERT INTO Meters (BuildingId, ContractorId, MeterType, MeterName, ManagementNumber, CreatedAt, UpdatedAt)
                                    VALUES (@BuildingId, @ContractorId, @MeterType, @MeterName, @ManagementNumber, @CreatedAt, @UpdatedAt)";
                using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                insertCmd.Parameters.Add("@BuildingId", SqlDbType.Int);
                insertCmd.Parameters.Add("@ContractorId", SqlDbType.Int);
                insertCmd.Parameters.Add("@MeterType", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@MeterName", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@ManagementNumber", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime);
                insertCmd.Parameters.Add("@UpdatedAt", SqlDbType.DateTime);

                var now = DateTime.Now;
                foreach (var meter in meters)
                {
                    insertCmd.Parameters["@BuildingId"].Value = meter.BuildingId.HasValue
                        ? meter.BuildingId.Value
                        : DBNull.Value;
                    insertCmd.Parameters["@ContractorId"].Value = meter.ContractorId.HasValue
                        ? meter.ContractorId.Value
                        : DBNull.Value;
                    insertCmd.Parameters["@MeterType"].Value = string.IsNullOrWhiteSpace(meter.MeterType)
                        ? DBNull.Value
                        : meter.MeterType;
                    insertCmd.Parameters["@MeterName"].Value = string.IsNullOrWhiteSpace(meter.MeterName)
                        ? DBNull.Value
                        : meter.MeterName;
                    insertCmd.Parameters["@ManagementNumber"].Value = string.IsNullOrWhiteSpace(meter.ManagementNumber)
                        ? DBNull.Value
                        : meter.ManagementNumber;
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

















