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

            var query = @"INSERT INTO Buildings (BuildingId, Name, Address, Floors, CreatedAt, UpdatedAt)
                         VALUES (@BuildingId, @Name, @Address, @Floors, @CreatedAt, @UpdatedAt);
                         SELECT CAST(SCOPE_IDENTITY() as int)";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", (object)building.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Name", building.Name);
            cmd.Parameters.AddWithValue("@Address", (object)building.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Floors", building.Floors);
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
                         SET BuildingId = @BuildingId, Name = @Name, Address = @Address, Floors = @Floors,
                             UpdatedAt = @UpdatedAt
                         WHERE Id = @Id";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", building.Id);
            cmd.Parameters.AddWithValue("@BuildingId", (object)building.BuildingId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Name", building.Name);
            cmd.Parameters.AddWithValue("@Address", (object)building.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Floors", building.Floors);
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

        /// <summary>
        /// ビル情報を全削除してIDを初期化し、CSVデータで再登録する
        /// </summary>
        public static async Task ReplaceBuildingsAsync(IEnumerable<Building> buildings)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using (var deleteCmd = new SqlCommand("DELETE FROM Buildings", connection, transaction))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                using (var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('[dbo].[Buildings]', RESEED, 0);", connection, transaction))
                {
                    await reseedCmd.ExecuteNonQueryAsync();
                }

                var insertQuery = @"INSERT INTO Buildings (BuildingId, Name, Address, Floors, CreatedAt, UpdatedAt)
                                    VALUES (@BuildingId, @Name, @Address, @Floors, @CreatedAt, @UpdatedAt)";
                using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                insertCmd.Parameters.Add("@BuildingId", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@Address", SqlDbType.NVarChar, 200);
                insertCmd.Parameters.Add("@Floors", SqlDbType.Int);
                insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime);
                insertCmd.Parameters.Add("@UpdatedAt", SqlDbType.DateTime);

                var now = DateTime.Now;
                foreach (var building in buildings)
                {
                    insertCmd.Parameters["@BuildingId"].Value = string.IsNullOrWhiteSpace(building.BuildingId)
                        ? DBNull.Value
                        : building.BuildingId;
                    insertCmd.Parameters["@Name"].Value = building.Name;
                    insertCmd.Parameters["@Address"].Value = string.IsNullOrWhiteSpace(building.Address)
                        ? DBNull.Value
                        : building.Address;
                    insertCmd.Parameters["@Floors"].Value = building.Floors;
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

