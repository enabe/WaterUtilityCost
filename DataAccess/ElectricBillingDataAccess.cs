using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    public class ElectricBillingDataAccess
    {
        public static async Task<List<ElectricBilling>> GetAllElectricBillingsAsync()
        {
            var electricBillings = new List<ElectricBilling>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, BillingYearMonth, BuildingName, UsageAmount, StartDate, EndDate, 
                                BasicCharge, PowerCharge, TaxRate, CustomerNumber, ContractorId, CreatedAt, UpdatedAt
                          FROM ElectricBillings ORDER BY BillingYearMonth DESC, BuildingName";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                electricBillings.Add(new ElectricBilling
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BillingYearMonth = reader.GetString(reader.GetOrdinal("BillingYearMonth")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    PowerCharge = reader.GetDecimal(reader.GetOrdinal("PowerCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    ContractorId = reader.IsDBNull(reader.GetOrdinal("ContractorId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ContractorId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return electricBillings;
        }

        public static async Task<ElectricBilling?> GetElectricBillingByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, BillingYearMonth, BuildingName, UsageAmount, StartDate, EndDate, 
                                BasicCharge, PowerCharge, TaxRate, CustomerNumber, ContractorId, CreatedAt, UpdatedAt
                          FROM ElectricBillings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ElectricBilling
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BillingYearMonth = reader.GetString(reader.GetOrdinal("BillingYearMonth")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    PowerCharge = reader.GetDecimal(reader.GetOrdinal("PowerCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    ContractorId = reader.IsDBNull(reader.GetOrdinal("ContractorId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ContractorId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        public static async Task AddElectricBillingAsync(ElectricBilling electricBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO ElectricBillings (BillingYearMonth, BuildingName, UsageAmount, StartDate, EndDate, 
                              BasicCharge, PowerCharge, TaxRate, CustomerNumber, ContractorId, CreatedAt, UpdatedAt)
                              VALUES (@BillingYearMonth, @BuildingName, @UsageAmount, @StartDate, @EndDate, 
                              @BasicCharge, @PowerCharge, @TaxRate, @CustomerNumber, @ContractorId, GETDATE(), GETDATE())";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", electricBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", electricBilling.BuildingName);
            cmd.Parameters.AddWithValue("@UsageAmount", electricBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", electricBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", electricBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", electricBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@PowerCharge", electricBilling.PowerCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)electricBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", electricBilling.CustomerNumber);
            cmd.Parameters.AddWithValue("@ContractorId", electricBilling.ContractorId.HasValue ? electricBilling.ContractorId.Value : (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateElectricBillingAsync(ElectricBilling electricBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE ElectricBillings SET BillingYearMonth = @BillingYearMonth, BuildingName = @BuildingName, UsageAmount = @UsageAmount,
                               StartDate = @StartDate, EndDate = @EndDate, BasicCharge = @BasicCharge, PowerCharge = @PowerCharge,
                               TaxRate = @TaxRate, CustomerNumber = @CustomerNumber, ContractorId = @ContractorId,
                               UpdatedAt = GETDATE()
                               WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", electricBilling.Id);
            cmd.Parameters.AddWithValue("@BillingYearMonth", electricBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", electricBilling.BuildingName);
            cmd.Parameters.AddWithValue("@UsageAmount", electricBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", electricBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", electricBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", electricBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@PowerCharge", electricBilling.PowerCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)electricBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", electricBilling.CustomerNumber);
            cmd.Parameters.AddWithValue("@ContractorId", electricBilling.ContractorId.HasValue ? electricBilling.ContractorId.Value : (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteElectricBillingAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM ElectricBillings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 請求年月が一致する電気料金請求データを取得
        /// </summary>
        /// <param name="billingYearMonth">請求年月（yyyy-MM形式）</param>
        /// <returns>電気料金請求データのリスト</returns>
        public static async Task<List<ElectricBilling>> GetElectricBillingsByBillingYearMonthAsync(string billingYearMonth)
        {
            var electricBillings = new List<ElectricBilling>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            var query = @"SELECT Id, BillingYearMonth, BuildingName, UsageAmount, StartDate, EndDate, 
                                BasicCharge, PowerCharge, TaxRate, CustomerNumber, ContractorId, CreatedAt, UpdatedAt
                          FROM ElectricBillings 
                          WHERE BillingYearMonth = @BillingYearMonth
                          ORDER BY BuildingName";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", billingYearMonth);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                electricBillings.Add(new ElectricBilling
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BillingYearMonth = reader.GetString(reader.GetOrdinal("BillingYearMonth")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    PowerCharge = reader.GetDecimal(reader.GetOrdinal("PowerCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    ContractorId = reader.IsDBNull(reader.GetOrdinal("ContractorId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ContractorId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return electricBillings;
        }

        /// <summary>
        /// 請求年月とStartDateの年月が一致する電気料金請求データを取得
        /// </summary>
        /// <param name="billingYearMonth">請求年月（yyyy-MM形式）</param>
        /// <returns>電気料金請求データのリスト</returns>
        public static async Task<List<ElectricBilling>> GetElectricBillingsByBillingYearMonthAndStartDateAsync(string billingYearMonth)
        {
            var electricBillings = new List<ElectricBilling>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            
            // 請求年月を年と月に分割
            var parts = billingYearMonth.Split('-');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int month))
            {
                return electricBillings;
            }
            
            var query = @"SELECT Id, BillingYearMonth, BuildingName, UsageAmount, StartDate, EndDate, 
                                BasicCharge, PowerCharge, TaxRate, CustomerNumber, ContractorId, CreatedAt, UpdatedAt
                          FROM ElectricBillings 
                          WHERE BillingYearMonth = @BillingYearMonth 
                          AND YEAR(StartDate) = @Year 
                          AND MONTH(StartDate) = @Month
                          ORDER BY BuildingName";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", billingYearMonth);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                electricBillings.Add(new ElectricBilling
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BillingYearMonth = reader.GetString(reader.GetOrdinal("BillingYearMonth")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    UsageAmount = reader.GetDecimal(reader.GetOrdinal("UsageAmount")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    BasicCharge = reader.GetDecimal(reader.GetOrdinal("BasicCharge")),
                    PowerCharge = reader.GetDecimal(reader.GetOrdinal("PowerCharge")),
                    TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                    CustomerNumber = reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    ContractorId = reader.IsDBNull(reader.GetOrdinal("ContractorId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ContractorId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return electricBillings;
        }
    }
}



