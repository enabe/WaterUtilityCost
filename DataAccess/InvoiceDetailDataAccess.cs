using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    public class InvoiceDetailDataAccess
    {
        private static string GetStringValueSafely(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static decimal GetDecimalValueSafely(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private static decimal? GetNullableDecimalValueSafely(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private static InvoiceDetail MapInvoiceDetail(SqlDataReader reader)
        {
            return new InvoiceDetail
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                BillingTo = reader.IsDBNull(reader.GetOrdinal("BillingTo")) ? string.Empty : reader.GetString(reader.GetOrdinal("BillingTo")),
                Lessor = reader.IsDBNull(reader.GetOrdinal("Lessor")) ? string.Empty : reader.GetString(reader.GetOrdinal("Lessor")),
                BuildingName = reader.IsDBNull(reader.GetOrdinal("BuildingName")) ? string.Empty : reader.GetString(reader.GetOrdinal("BuildingName")),
                Lessee = reader.IsDBNull(reader.GetOrdinal("Lessee")) ? string.Empty : reader.GetString(reader.GetOrdinal("Lessee")),
                RoomNumber = reader.IsDBNull(reader.GetOrdinal("RoomNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("RoomNumber")),
                RoomArea = GetNullableDecimalValueSafely(reader, "RoomArea"),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? string.Empty : reader.GetString(reader.GetOrdinal("Category")),
                Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? string.Empty : reader.GetString(reader.GetOrdinal("Content")),
                ChildMeterUsage = GetNullableDecimalValueSafely(reader, "ChildMeterUsage"),
                UsageAmount = GetNullableDecimalValueSafely(reader, "UsageAmount"),
                Unit = reader.IsDBNull(reader.GetOrdinal("Unit")) ? string.Empty : reader.GetString(reader.GetOrdinal("Unit")),
                TaxInclusiveAmount = reader.IsDBNull(reader.GetOrdinal("TaxInclusiveAmount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxInclusiveAmount")),
                TaxRate = reader.IsDBNull(reader.GetOrdinal("TaxRate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                Contractor = GetStringValueSafely(reader, "Contractor"),
                InvoiceNumber = GetStringValueSafely(reader, "InvoiceNumber"),
                ChildMeterStartDate = reader.IsDBNull(reader.GetOrdinal("ChildMeterStartDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ChildMeterStartDate")),
                ChildMeterEndDate = reader.IsDBNull(reader.GetOrdinal("ChildMeterEndDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ChildMeterEndDate")),
                ParentMeterStartDate = reader.IsDBNull(reader.GetOrdinal("ParentMeterStartDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ParentMeterStartDate")),
                ParentMeterEndDate = reader.IsDBNull(reader.GetOrdinal("ParentMeterEndDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ParentMeterEndDate")),
                ConfirmedBillingDate = reader.IsDBNull(reader.GetOrdinal("ConfirmedBillingDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ConfirmedBillingDate")),
                BillingYearMonth = GetStringValueSafely(reader, "BillingYearMonth"),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }

        public static string FormatBillingYearMonth(int year, int month) =>
            $"{year:D4}-{month:D2}";

        public static async Task<List<InvoiceDetail>> GetAllInvoiceDetailsAsync()
        {
            var invoiceDetails = new List<InvoiceDetail>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM InvoiceDetails ORDER BY BillingYearMonth DESC, ConfirmedBillingDate DESC, BuildingName, RoomNumber";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                invoiceDetails.Add(MapInvoiceDetail(reader));
            }
            return invoiceDetails;
        }

        public static async Task<List<InvoiceDetail>> GetInvoiceDetailsByBillingYearMonthAsync(int year, int month)
        {
            var billingYearMonth = FormatBillingYearMonth(year, month);
            var invoiceDetails = new List<InvoiceDetail>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT * FROM InvoiceDetails 
                         WHERE BillingYearMonth = @BillingYearMonth
                         ORDER BY ConfirmedBillingDate DESC, BuildingName, RoomNumber";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BillingYearMonth", billingYearMonth);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                invoiceDetails.Add(MapInvoiceDetail(reader));
            }
            return invoiceDetails;
        }

        public static async Task<InvoiceDetail?> GetInvoiceDetailByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM InvoiceDetails WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapInvoiceDetail(reader);
            }
            return null;
        }

        public static async Task<int> CreateInvoiceDetailAsync(InvoiceDetail invoiceDetail)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO InvoiceDetails (
                         BillingTo, Lessor, BuildingName, Lessee, RoomNumber, RoomArea, Category, Content,
                         ChildMeterUsage, UsageAmount, Unit, TaxInclusiveAmount, TaxRate, Contractor, InvoiceNumber,
                         ChildMeterStartDate, ChildMeterEndDate, ParentMeterStartDate, ParentMeterEndDate, ConfirmedBillingDate,
                         BillingYearMonth, CreatedAt, UpdatedAt
                         ) VALUES (
                         @BillingTo, @Lessor, @BuildingName, @Lessee, @RoomNumber, @RoomArea, @Category, @Content,
                         @ChildMeterUsage, @UsageAmount, @Unit, @TaxInclusiveAmount, @TaxRate, @Contractor, @InvoiceNumber,
                         @ChildMeterStartDate, @ChildMeterEndDate, @ParentMeterStartDate, @ParentMeterEndDate, @ConfirmedBillingDate,
                         @BillingYearMonth, @CreatedAt, @UpdatedAt);
                         SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            AddInvoiceDetailParameters(cmd, invoiceDetail);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            return (int)await cmd.ExecuteScalarAsync();
        }

        public static async Task<bool> UpdateInvoiceDetailAsync(InvoiceDetail invoiceDetail)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE InvoiceDetails SET 
                         BillingTo = @BillingTo, Lessor = @Lessor, BuildingName = @BuildingName, 
                         Lessee = @Lessee, RoomNumber = @RoomNumber, RoomArea = @RoomArea, Category = @Category, Content = @Content,
                         ChildMeterUsage = @ChildMeterUsage, UsageAmount = @UsageAmount, Unit = @Unit, TaxInclusiveAmount = @TaxInclusiveAmount, TaxRate = @TaxRate,
                         Contractor = @Contractor, InvoiceNumber = @InvoiceNumber,
                         ChildMeterStartDate = @ChildMeterStartDate, ChildMeterEndDate = @ChildMeterEndDate,
                         ParentMeterStartDate = @ParentMeterStartDate, ParentMeterEndDate = @ParentMeterEndDate,
                         ConfirmedBillingDate = @ConfirmedBillingDate, BillingYearMonth = @BillingYearMonth, UpdatedAt = @UpdatedAt
                         WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", invoiceDetail.Id);
            AddInvoiceDetailParameters(cmd, invoiceDetail);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static void AddInvoiceDetailParameters(SqlCommand cmd, InvoiceDetail invoiceDetail)
        {
            cmd.Parameters.AddWithValue("@BillingTo", (object)invoiceDetail.BillingTo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Lessor", (object)invoiceDetail.Lessor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingName", (object)invoiceDetail.BuildingName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Lessee", (object)invoiceDetail.Lessee ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomNumber", (object)invoiceDetail.RoomNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomArea", invoiceDetail.RoomArea.HasValue ? invoiceDetail.RoomArea.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", (object)invoiceDetail.Category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Content", (object)invoiceDetail.Content ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChildMeterUsage", invoiceDetail.ChildMeterUsage.HasValue ? invoiceDetail.ChildMeterUsage.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UsageAmount", invoiceDetail.UsageAmount.HasValue ? invoiceDetail.UsageAmount.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", (object)invoiceDetail.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TaxInclusiveAmount", invoiceDetail.TaxInclusiveAmount);
            cmd.Parameters.AddWithValue("@TaxRate", invoiceDetail.TaxRate);
            cmd.Parameters.AddWithValue("@Contractor", (object)invoiceDetail.Contractor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InvoiceNumber", (object)invoiceDetail.InvoiceNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChildMeterStartDate", (object)invoiceDetail.ChildMeterStartDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChildMeterEndDate", (object)invoiceDetail.ChildMeterEndDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ParentMeterStartDate", (object)invoiceDetail.ParentMeterStartDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ParentMeterEndDate", (object)invoiceDetail.ParentMeterEndDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ConfirmedBillingDate", (object)invoiceDetail.ConfirmedBillingDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillingYearMonth", string.IsNullOrWhiteSpace(invoiceDetail.BillingYearMonth) ? (object)DBNull.Value : invoiceDetail.BillingYearMonth.Trim());
        }

        public static async Task<bool> DeleteInvoiceDetailAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM InvoiceDetails WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        /// <summary>
        /// 種別が電気、水道、ガス以外の請求明細を取得
        /// </summary>
        public static async Task<List<InvoiceDetail>> GetOtherInvoiceDetailsAsync()
        {
            var invoiceDetails = new List<InvoiceDetail>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT * FROM InvoiceDetails 
                         WHERE Category NOT IN ('電気', '水道', 'ガス') OR Category IS NULL OR Category = ''
                         ORDER BY BillingYearMonth DESC, ConfirmedBillingDate DESC, BuildingName, RoomNumber";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                invoiceDetails.Add(MapInvoiceDetail(reader));
            }
            return invoiceDetails;
        }
    }
}
