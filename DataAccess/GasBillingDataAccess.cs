using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    public class GasBillingDataAccess
    {
        public static async Task<List<GasBilling>> GetAllGasBillingsAsync()
        {
            var gasBillings = new List<GasBilling>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM GasBillings ORDER BY BillingYearMonth DESC, BuildingName";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                gasBillings.Add(new GasBilling
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BillingYearMonth = reader.GetString(reader.GetOrdinal("BillingYearMonth")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    FloorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FloorName")),
                    ParentMeterId = reader.IsDBNull(reader.GetOrdinal("ParentMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentMeterId")),
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    UsageCharge = reader.GetDecimal(reader.GetOrdinal("UsageCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    ContractorId = TryGetContractorId(reader),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return gasBillings;
        }

        private static int? TryGetContractorId(SqlDataReader reader)
        {
            try
            {
                var ord = reader.GetOrdinal("ContractorId");
                return reader.IsDBNull(ord) ? (int?)null : reader.GetInt32(ord);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<GasBilling?> GetGasBillingByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM GasBillings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new GasBilling
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BillingYearMonth = reader.GetString(reader.GetOrdinal("BillingYearMonth")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    FloorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FloorName")),
                    ParentMeterId = reader.IsDBNull(reader.GetOrdinal("ParentMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentMeterId")),
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    UsageCharge = reader.GetDecimal(reader.GetOrdinal("UsageCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    ContractorId = TryGetContractorId(reader),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        public static async Task AddGasBillingAsync(GasBilling gasBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO GasBillings (BillingYearMonth, BuildingName, FloorName, ParentMeterId, UsageAmount, StartDate, EndDate, 
                              BasicCharge, UsageCharge, TaxRate, CustomerNumber, ContractorId, CreatedAt, UpdatedAt)
                              VALUES (@BillingYearMonth, @BuildingName, @FloorName, @ParentMeterId, @UsageAmount, @StartDate, @EndDate, 
                              @BasicCharge, @UsageCharge, @TaxRate, @CustomerNumber, @ContractorId, GETDATE(), GETDATE())";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", gasBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", gasBilling.BuildingName);
            cmd.Parameters.AddWithValue("@FloorName", gasBilling.FloorName);
            cmd.Parameters.AddWithValue("@ParentMeterId", (object)gasBilling.ParentMeterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UsageAmount", gasBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", gasBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", gasBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", gasBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@UsageCharge", gasBilling.UsageCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)gasBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", gasBilling.CustomerNumber);
            cmd.Parameters.AddWithValue("@ContractorId", gasBilling.ContractorId.HasValue ? gasBilling.ContractorId.Value : (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateGasBillingAsync(GasBilling gasBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE GasBillings SET BillingYearMonth = @BillingYearMonth, BuildingName = @BuildingName, FloorName = @FloorName, ParentMeterId = @ParentMeterId, UsageAmount = @UsageAmount,
                               StartDate = @StartDate, EndDate = @EndDate, BasicCharge = @BasicCharge, UsageCharge = @UsageCharge,
                               TaxRate = @TaxRate, CustomerNumber = @CustomerNumber, ContractorId = @ContractorId,
                               UpdatedAt = GETDATE()
                               WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", gasBilling.Id);
            cmd.Parameters.AddWithValue("@BillingYearMonth", gasBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", gasBilling.BuildingName);
            cmd.Parameters.AddWithValue("@FloorName", gasBilling.FloorName);
            cmd.Parameters.AddWithValue("@ParentMeterId", (object)gasBilling.ParentMeterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UsageAmount", gasBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", gasBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", gasBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", gasBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@UsageCharge", gasBilling.UsageCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)gasBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", gasBilling.CustomerNumber);
            cmd.Parameters.AddWithValue("@ContractorId", gasBilling.ContractorId.HasValue ? gasBilling.ContractorId.Value : (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteGasBillingAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM GasBillings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 請求年月が一致するガス料金請求データを取得
        /// </summary>
        /// <param name="billingYearMonth">請求年月（yyyy-MM形式）</param>
        /// <returns>ガス料金請求データのリスト</returns>
        public static async Task<List<GasBilling>> GetGasBillingsByBillingYearMonthAsync(string billingYearMonth)
        {
            var gasBillings = new List<GasBilling>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT * FROM GasBillings
                          WHERE BillingYearMonth = @BillingYearMonth
                          ORDER BY BuildingName, FloorName";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", billingYearMonth);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                gasBillings.Add(new GasBilling
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BillingYearMonth = reader.GetString(reader.GetOrdinal("BillingYearMonth")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    FloorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FloorName")),
                    ParentMeterId = reader.IsDBNull(reader.GetOrdinal("ParentMeterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentMeterId")),
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    UsageCharge = reader.GetDecimal(reader.GetOrdinal("UsageCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    ContractorId = TryGetContractorId(reader),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return gasBillings;
        }
    }
}



