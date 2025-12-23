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
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    UsageCharge = reader.GetDecimal(reader.GetOrdinal("UsageCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return gasBillings;
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
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    UsageCharge = reader.GetDecimal(reader.GetOrdinal("UsageCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
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
            var query = @"INSERT INTO GasBillings (BillingYearMonth, BuildingName, UsageAmount, StartDate, EndDate, 
                              BasicCharge, UsageCharge, TaxRate, CustomerNumber, CreatedAt, UpdatedAt)
                              VALUES (@BillingYearMonth, @BuildingName, @UsageAmount, @StartDate, @EndDate, 
                              @BasicCharge, @UsageCharge, @TaxRate, @CustomerNumber, GETDATE(), GETDATE())";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", gasBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", gasBilling.BuildingName);
            cmd.Parameters.AddWithValue("@UsageAmount", gasBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", gasBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", gasBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", gasBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@UsageCharge", gasBilling.UsageCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)gasBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", gasBilling.CustomerNumber);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateGasBillingAsync(GasBilling gasBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE GasBillings SET BillingYearMonth = @BillingYearMonth, BuildingName = @BuildingName, UsageAmount = @UsageAmount,
                               StartDate = @StartDate, EndDate = @EndDate, BasicCharge = @BasicCharge, UsageCharge = @UsageCharge,
                               TaxRate = @TaxRate, CustomerNumber = @CustomerNumber,
                               UpdatedAt = GETDATE()
                               WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", gasBilling.Id);
            cmd.Parameters.AddWithValue("@BillingYearMonth", gasBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", gasBilling.BuildingName);
            cmd.Parameters.AddWithValue("@UsageAmount", gasBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", gasBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", gasBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", gasBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@UsageCharge", gasBilling.UsageCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)gasBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", gasBilling.CustomerNumber);
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
    }
}



