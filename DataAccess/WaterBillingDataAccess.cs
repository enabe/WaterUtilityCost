using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    public class WaterBillingDataAccess
    {
        public static async Task<List<WaterBilling>> GetAllWaterBillingsAsync()
        {
            var waterBillings = new List<WaterBilling>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM WaterBillings ORDER BY BillingYearMonth DESC, BuildingName";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                waterBillings.Add(new WaterBilling
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
            return waterBillings;
        }

        public static async Task<WaterBilling?> GetWaterBillingByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM WaterBillings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new WaterBilling
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

        public static async Task AddWaterBillingAsync(WaterBilling waterBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO WaterBillings (BillingYearMonth, BuildingName, UsageAmount, StartDate, EndDate, BasicCharge, UsageCharge, TaxRate, CustomerNumber, CreatedAt, UpdatedAt)
                          VALUES (@BillingYearMonth, @BuildingName, @UsageAmount, @StartDate, @EndDate, @BasicCharge, @UsageCharge, @TaxRate, @CustomerNumber, GETDATE(), GETDATE())";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", waterBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", waterBilling.BuildingName);
            cmd.Parameters.AddWithValue("@UsageAmount", waterBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", waterBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", waterBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", waterBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@UsageCharge", waterBilling.UsageCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)waterBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", waterBilling.CustomerNumber);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateWaterBillingAsync(WaterBilling waterBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE WaterBillings SET BillingYearMonth = @BillingYearMonth, BuildingName = @BuildingName, UsageAmount = @UsageAmount,
                               StartDate = @StartDate, EndDate = @EndDate, BasicCharge = @BasicCharge, UsageCharge = @UsageCharge, TaxRate = @TaxRate, CustomerNumber = @CustomerNumber,
                               UpdatedAt = GETDATE()
                               WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", waterBilling.Id);
            cmd.Parameters.AddWithValue("@BillingYearMonth", waterBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", waterBilling.BuildingName);
            cmd.Parameters.AddWithValue("@UsageAmount", waterBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", waterBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", waterBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", waterBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@UsageCharge", waterBilling.UsageCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)waterBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", waterBilling.CustomerNumber);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteWaterBillingAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM WaterBillings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<List<string>> GetBuildingNamesAsync()
        {
            var buildingNames = new List<string>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT Name FROM Buildings ORDER BY Name";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                buildingNames.Add(reader.GetString(reader.GetOrdinal("Name")));
            }
            return buildingNames;
        }
    }
}



