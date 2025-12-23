using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    /// <summary>
    /// 契約情報のデータアクセス層
    /// </summary>
    public class ContractDataAccess
    {
        /// <summary>
        /// すべての契約を取得
        /// </summary>
        public static async Task<List<Contract>> GetAllContractsAsync()
        {
            var contracts = new List<Contract>();

            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Contracts ORDER BY ContractNumber";
            using var cmd = new SqlCommand(query, connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(new Contract
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ContractNumber = reader.IsDBNull(reader.GetOrdinal("ContractNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContractNumber")),
                    ContractType = reader.IsDBNull(reader.GetOrdinal("ContractType")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContractType")),
                    ContractorName = reader.IsDBNull(reader.GetOrdinal("ContractorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContractorName")),
                    LessorClientId = reader.IsDBNull(reader.GetOrdinal("LessorClientId")) ? null : reader.GetInt32(reader.GetOrdinal("LessorClientId")),
                    LesseeClientId = reader.IsDBNull(reader.GetOrdinal("LesseeClientId")) ? null : reader.GetInt32(reader.GetOrdinal("LesseeClientId")),
                    BillingClientId = reader.IsDBNull(reader.GetOrdinal("BillingClientId")) ? null : reader.GetInt32(reader.GetOrdinal("BillingClientId")),
                    StartDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    ContractStatus = reader.IsDBNull(reader.GetOrdinal("ContractStatus")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContractStatus")),
                    ClosingDate = reader.IsDBNull(reader.GetOrdinal("ClosingDate")) ? null : reader.GetInt32(reader.GetOrdinal("ClosingDate")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    CustomerNumber = reader.IsDBNull(reader.GetOrdinal("CustomerNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return contracts;
        }

        /// <summary>
        /// IDで契約を取得
        /// </summary>
        public static async Task<Contract?> GetContractByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM Contracts WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Contract
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ContractNumber = reader.IsDBNull(reader.GetOrdinal("ContractNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContractNumber")),
                    ContractType = reader.IsDBNull(reader.GetOrdinal("ContractType")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContractType")),
                    ContractorName = reader.IsDBNull(reader.GetOrdinal("ContractorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContractorName")),
                    LessorClientId = reader.IsDBNull(reader.GetOrdinal("LessorClientId")) ? null : reader.GetInt32(reader.GetOrdinal("LessorClientId")),
                    LesseeClientId = reader.IsDBNull(reader.GetOrdinal("LesseeClientId")) ? null : reader.GetInt32(reader.GetOrdinal("LesseeClientId")),
                    BillingClientId = reader.IsDBNull(reader.GetOrdinal("BillingClientId")) ? null : reader.GetInt32(reader.GetOrdinal("BillingClientId")),
                    StartDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    ContractStatus = reader.IsDBNull(reader.GetOrdinal("ContractStatus")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContractStatus")),
                    ClosingDate = reader.IsDBNull(reader.GetOrdinal("ClosingDate")) ? null : reader.GetInt32(reader.GetOrdinal("ClosingDate")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? null : reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    CustomerNumber = reader.IsDBNull(reader.GetOrdinal("CustomerNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("CustomerNumber")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        /// <summary>
        /// 契約を追加
        /// </summary>
        public static async Task<int> CreateContractAsync(Contract contract)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = @"INSERT INTO Contracts (ContractNumber, ContractType, ContractorName, LessorClientId, LesseeClientId, BillingClientId, 
                         StartDate, EndDate, ContractStatus, ClosingDate, BuildingId, CustomerNumber, CreatedAt, UpdatedAt)
                         VALUES (@ContractNumber, @ContractType, @ContractorName, @LessorClientId, @LesseeClientId, @BillingClientId, 
                         @StartDate, @EndDate, @ContractStatus, @ClosingDate, @BuildingId, @CustomerNumber, @CreatedAt, @UpdatedAt);
                         SELECT CAST(SCOPE_IDENTITY() as int)";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ContractNumber", (object)contract.ContractNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractType", (object)contract.ContractType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractorName", (object)contract.ContractorName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LessorClientId", (object)contract.LessorClientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LesseeClientId", (object)contract.LesseeClientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillingClientId", (object)contract.BillingClientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDate", (object)contract.StartDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", (object)contract.EndDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractStatus", (object)contract.ContractStatus ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClosingDate", (object)contract.ClosingDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingId", (object)contract.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", (object)contract.CustomerNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        /// <summary>
        /// 契約を更新
        /// </summary>
        public static async Task<bool> UpdateContractAsync(Contract contract)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = @"UPDATE Contracts 
                         SET ContractNumber = @ContractNumber, ContractType = @ContractType, ContractorName = @ContractorName,
                             LessorClientId = @LessorClientId, LesseeClientId = @LesseeClientId, BillingClientId = @BillingClientId,
                             StartDate = @StartDate, EndDate = @EndDate, ContractStatus = @ContractStatus, ClosingDate = @ClosingDate,
                             BuildingId = @BuildingId, CustomerNumber = @CustomerNumber, UpdatedAt = @UpdatedAt
                         WHERE Id = @Id";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", contract.Id);
            cmd.Parameters.AddWithValue("@ContractNumber", (object)contract.ContractNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractType", (object)contract.ContractType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractorName", (object)contract.ContractorName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LessorClientId", (object)contract.LessorClientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LesseeClientId", (object)contract.LesseeClientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillingClientId", (object)contract.BillingClientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDate", (object)contract.StartDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", (object)contract.EndDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContractStatus", (object)contract.ContractStatus ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClosingDate", (object)contract.ClosingDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingId", (object)contract.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerNumber", (object)contract.CustomerNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        /// <summary>
        /// 契約を削除
        /// </summary>
        public static async Task<bool> DeleteContractAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Contracts WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}















