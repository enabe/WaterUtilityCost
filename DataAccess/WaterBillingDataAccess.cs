using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
                waterBillings.Add(CreateWaterBillingFromReader(reader));
            }
            return waterBillings;
        }

        private static WaterBilling CreateWaterBillingFromReader(SqlDataReader reader)
        {
            var ordinalDiff = reader.GetOrdinal("DifferenceAssignmentRoomName");
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
                ParentMeterId = TryGetParentMeterId(reader),
                DifferenceAssignmentRoomName = reader.IsDBNull(ordinalDiff) ? string.Empty : reader.GetString(ordinalDiff),
                ContractorId = TryGetContractorId(reader),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
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

        private static int? TryGetParentMeterId(SqlDataReader reader)
        {
            try
            {
                var ord = reader.GetOrdinal("ParentMeterId");
                return reader.IsDBNull(ord) ? (int?)null : reader.GetInt32(ord);
            }
            catch
            {
                return null;
            }
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
                return CreateWaterBillingFromReader(reader);
            }
            return null;
        }

        public static async Task AddWaterBillingAsync(WaterBilling waterBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO WaterBillings (BillingYearMonth, BuildingName, ParentMeterId, UsageAmount, StartDate, EndDate, BasicCharge, UsageCharge, TaxRate, CustomerNumber, DifferenceAssignmentRoomName, ContractorId, CreatedAt, UpdatedAt)
                          VALUES (@BillingYearMonth, @BuildingName, @ParentMeterId, @UsageAmount, @StartDate, @EndDate, @BasicCharge, @UsageCharge, @TaxRate, @CustomerNumber, @DifferenceAssignmentRoomName, @ContractorId, GETDATE(), GETDATE())";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", waterBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", waterBilling.BuildingName);
            cmd.Parameters.AddWithValue("@ParentMeterId", waterBilling.ParentMeterId.HasValue ? waterBilling.ParentMeterId.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UsageAmount", waterBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", waterBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", waterBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", waterBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@UsageCharge", waterBilling.UsageCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)waterBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", waterBilling.CustomerNumber);
            cmd.Parameters.AddWithValue("@DifferenceAssignmentRoomName", string.IsNullOrWhiteSpace(waterBilling.DifferenceAssignmentRoomName) ? (object)DBNull.Value : waterBilling.DifferenceAssignmentRoomName);
            cmd.Parameters.AddWithValue("@ContractorId", waterBilling.ContractorId.HasValue ? waterBilling.ContractorId.Value : (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateWaterBillingAsync(WaterBilling waterBilling)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE WaterBillings SET BillingYearMonth = @BillingYearMonth, BuildingName = @BuildingName, ParentMeterId = @ParentMeterId, UsageAmount = @UsageAmount,
                               StartDate = @StartDate, EndDate = @EndDate, BasicCharge = @BasicCharge, UsageCharge = @UsageCharge, TaxRate = @TaxRate, CustomerNumber = @CustomerNumber,
                               DifferenceAssignmentRoomName = @DifferenceAssignmentRoomName, ContractorId = @ContractorId, UpdatedAt = GETDATE()
                               WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", waterBilling.Id);
            cmd.Parameters.AddWithValue("@BillingYearMonth", waterBilling.BillingYearMonth);
            cmd.Parameters.AddWithValue("@BuildingName", waterBilling.BuildingName);
            cmd.Parameters.AddWithValue("@ParentMeterId", waterBilling.ParentMeterId.HasValue ? waterBilling.ParentMeterId.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UsageAmount", waterBilling.UsageAmount);
            cmd.Parameters.AddWithValue("@StartDate", waterBilling.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", waterBilling.EndDate);
            cmd.Parameters.AddWithValue("@BasicCharge", waterBilling.BasicCharge);
            cmd.Parameters.AddWithValue("@UsageCharge", waterBilling.UsageCharge);
            cmd.Parameters.AddWithValue("@TaxRate", (object)waterBilling.TaxRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", waterBilling.CustomerNumber);
            cmd.Parameters.AddWithValue("@DifferenceAssignmentRoomName", string.IsNullOrWhiteSpace(waterBilling.DifferenceAssignmentRoomName) ? (object)DBNull.Value : waterBilling.DifferenceAssignmentRoomName);
            cmd.Parameters.AddWithValue("@ContractorId", waterBilling.ContractorId.HasValue ? waterBilling.ContractorId.Value : (object)DBNull.Value);
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

        /// <summary>
        /// 請求年月が一致する水道料金請求データを取得
        /// </summary>
        /// <param name="billingYearMonth">請求年月（yyyy-MM形式）</param>
        /// <returns>水道料金請求データのリスト</returns>
        public static async Task<List<WaterBilling>> GetWaterBillingsByBillingYearMonthAsync(string billingYearMonth)
        {
            var waterBillings = new List<WaterBilling>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = @"SELECT * FROM WaterBillings
                          WHERE BillingYearMonth = @BillingYearMonth
                          ORDER BY BuildingName";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", billingYearMonth);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                waterBillings.Add(CreateWaterBillingFromReader(reader));
            }
            return waterBillings;
        }

        /// <summary>
        /// 指定ビル名の請求先の部屋名一覧を取得（差額割当先の選択用）
        /// </summary>
        public static async Task<List<string>> GetRoomNamesByBuildingNameAsync(string buildingName)
        {
            var clients = await ClientDataAccess.GetClientsByBuildingNameAndIsBillingToAsync(buildingName);
            return clients
                .Select(c => c.RoomName ?? string.Empty)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct()
                .OrderBy(r => r)
                .ToList();
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



