using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WaterUtilityCost.Database;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.DataAccess
{
    /// <summary>
    /// ビル情報のデータアクセス層
    /// </summary>
    public class BuildingDataAccess
    {
        /// <summary>
        /// すべてのビルを取得
        /// </summary>
        public static async Task<List<Building>> GetAllBuildingsAsync()
        {
            var buildings = new List<Building>();

            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Buildings ORDER BY Name";
            using var cmd = new SqlCommand(query, connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                buildings.Add(new Building
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? string.Empty : reader.GetString(reader.GetOrdinal("BuildingId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? string.Empty : reader.GetString(reader.GetOrdinal("Address")),
                    Floors = reader.IsDBNull(reader.GetOrdinal("Floors")) ? 0 : reader.GetInt32(reader.GetOrdinal("Floors")),
                    BuiltDate = reader.IsDBNull(reader.GetOrdinal("BuiltDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("BuiltDate")),
                    Area = reader.IsDBNull(reader.GetOrdinal("Area")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Area")),
                    Owner = reader.IsDBNull(reader.GetOrdinal("Owner")) ? string.Empty : reader.GetString(reader.GetOrdinal("Owner")),
                    Contact = reader.IsDBNull(reader.GetOrdinal("Contact")) ? string.Empty : reader.GetString(reader.GetOrdinal("Contact")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return buildings;
        }

        /// <summary>
        /// IDでビルを取得
        /// </summary>
        public static async Task<Building?> GetBuildingByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM Buildings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Building
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BuildingId = reader.IsDBNull(reader.GetOrdinal("BuildingId")) ? string.Empty : reader.GetString(reader.GetOrdinal("BuildingId")),
                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                    Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? string.Empty : reader.GetString(reader.GetOrdinal("Address")),
                    Floors = reader.IsDBNull(reader.GetOrdinal("Floors")) ? 0 : reader.GetInt32(reader.GetOrdinal("Floors")),
                    BuiltDate = reader.IsDBNull(reader.GetOrdinal("BuiltDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("BuiltDate")),
                    Area = reader.IsDBNull(reader.GetOrdinal("Area")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Area")),
                    Owner = reader.IsDBNull(reader.GetOrdinal("Owner")) ? string.Empty : reader.GetString(reader.GetOrdinal("Owner")),
                    Contact = reader.IsDBNull(reader.GetOrdinal("Contact")) ? string.Empty : reader.GetString(reader.GetOrdinal("Contact")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        /// <summary>
        /// ビルを追加
        /// </summary>
        public static async Task<int> AddBuildingAsync(Building building)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = @"INSERT INTO Buildings (BuildingId, Name, Address, Floors, BuiltDate, Area, Owner, Contact, CreatedAt, UpdatedAt)
                         VALUES (@BuildingId, @Name, @Address, @Floors, @BuiltDate, @Area, @Owner, @Contact, @CreatedAt, @UpdatedAt);
                         SELECT CAST(SCOPE_IDENTITY() as int)";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", (object)building.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Name", building.Name);
            cmd.Parameters.AddWithValue("@Address", (object)building.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Floors", building.Floors);
            // DateTime.MinValueまたはSQL ServerのDateTime範囲外の場合はNULLとして扱う
            // SQL ServerのDateTime型は1753-01-01から9999-12-31まで
            var sqlMinDate = new DateTime(1753, 1, 1);
            var builtDateValue = (building.BuiltDate == DateTime.MinValue || building.BuiltDate < sqlMinDate) 
                ? DBNull.Value 
                : (object)building.BuiltDate;
            cmd.Parameters.AddWithValue("@BuiltDate", builtDateValue);
            cmd.Parameters.AddWithValue("@Area", building.Area);
            cmd.Parameters.AddWithValue("@Owner", (object)building.Owner ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Contact", (object)building.Contact ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        /// <summary>
        /// ビルを更新
        /// </summary>
        public static async Task UpdateBuildingAsync(Building building)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = @"UPDATE Buildings 
                         SET BuildingId = @BuildingId, Name = @Name, Address = @Address, Floors = @Floors, BuiltDate = @BuiltDate,
                             Area = @Area, Owner = @Owner, Contact = @Contact, UpdatedAt = @UpdatedAt
                         WHERE Id = @Id";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", building.Id);
            cmd.Parameters.AddWithValue("@BuildingId", (object)building.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Name", building.Name);
            cmd.Parameters.AddWithValue("@Address", (object)building.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Floors", building.Floors);
            // DateTime.MinValueまたはSQL ServerのDateTime範囲外の場合はNULLとして扱う
            // SQL ServerのDateTime型は1753-01-01から9999-12-31まで
            var sqlMinDate = new DateTime(1753, 1, 1);
            var builtDateValue = (building.BuiltDate == DateTime.MinValue || building.BuiltDate < sqlMinDate) 
                ? DBNull.Value 
                : (object)building.BuiltDate;
            cmd.Parameters.AddWithValue("@BuiltDate", builtDateValue);
            cmd.Parameters.AddWithValue("@Area", building.Area);
            cmd.Parameters.AddWithValue("@Owner", (object)building.Owner ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Contact", (object)building.Contact ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// ビルを削除
        /// </summary>
        public static async Task DeleteBuildingAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Buildings WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}

