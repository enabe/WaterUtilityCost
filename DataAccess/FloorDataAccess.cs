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
    /// フロア情報のデータアクセス層
    /// </summary>
    public class FloorDataAccess
    {
        /// <summary>
        /// すべてのフロアを取得
        /// </summary>
        public static async Task<List<Floor>> GetAllFloorsAsync()
        {
            var floors = new List<Floor>();

            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Floors ORDER BY BuildingId, FloorName";
            using var cmd = new SqlCommand(query, connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                floors.Add(new Floor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BuildingId = reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    FloorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FloorName")),
                    FloorArea = reader.IsDBNull(reader.GetOrdinal("FloorArea")) ? 0 : reader.GetDecimal(reader.GetOrdinal("FloorArea")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return floors;
        }

        /// <summary>
        /// IDでフロアを取得
        /// </summary>
        public static async Task<Floor?> GetFloorByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM Floors WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Floor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BuildingId = reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    FloorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FloorName")),
                    FloorArea = reader.IsDBNull(reader.GetOrdinal("FloorArea")) ? 0 : reader.GetDecimal(reader.GetOrdinal("FloorArea")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        /// <summary>
        /// ビルIDでフロアを取得
        /// </summary>
        public static async Task<List<Floor>> GetFloorsByBuildingIdAsync(int buildingId)
        {
            var floors = new List<Floor>();

            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Floors WHERE BuildingId = @BuildingId ORDER BY FloorName";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", buildingId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                floors.Add(new Floor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BuildingId = reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    FloorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FloorName")),
                    FloorArea = reader.IsDBNull(reader.GetOrdinal("FloorArea")) ? 0 : reader.GetDecimal(reader.GetOrdinal("FloorArea")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return floors;
        }

        /// <summary>
        /// ビル名でフロアを取得
        /// </summary>
        public static async Task<List<Floor>> GetFloorsByBuildingNameAsync(string buildingName)
        {
            var floors = new List<Floor>();

            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = @"SELECT f.Id, f.BuildingId, f.FloorName, f.FloorArea, f.CreatedAt, f.UpdatedAt
                          FROM Floors f
                          INNER JOIN Buildings b ON f.BuildingId = b.Id
                          WHERE b.Name = @BuildingName
                          ORDER BY f.FloorName";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingName", buildingName);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                floors.Add(new Floor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    BuildingId = reader.GetInt32(reader.GetOrdinal("BuildingId")),
                    FloorName = reader.IsDBNull(reader.GetOrdinal("FloorName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FloorName")),
                    FloorArea = reader.IsDBNull(reader.GetOrdinal("FloorArea")) ? 0 : reader.GetDecimal(reader.GetOrdinal("FloorArea")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return floors;
        }

        /// <summary>
        /// フロアを追加
        /// </summary>
        public static async Task<int> AddFloorAsync(Floor floor)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = @"INSERT INTO Floors (BuildingId, FloorName, FloorArea, CreatedAt, UpdatedAt)
                         VALUES (@BuildingId, @FloorName, @FloorArea, @CreatedAt, @UpdatedAt);
                         SELECT CAST(SCOPE_IDENTITY() as int)";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingId", floor.BuildingId);
            cmd.Parameters.AddWithValue("@FloorName", floor.FloorName);
            cmd.Parameters.AddWithValue("@FloorArea", floor.FloorArea);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        /// <summary>
        /// フロアを更新
        /// </summary>
        public static async Task UpdateFloorAsync(Floor floor)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = @"UPDATE Floors 
                         SET BuildingId = @BuildingId, FloorName = @FloorName, FloorArea = @FloorArea, UpdatedAt = @UpdatedAt
                         WHERE Id = @Id";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", floor.Id);
            cmd.Parameters.AddWithValue("@BuildingId", floor.BuildingId);
            cmd.Parameters.AddWithValue("@FloorName", floor.FloorName);
            cmd.Parameters.AddWithValue("@FloorArea", floor.FloorArea);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// フロアを削除
        /// </summary>
        public static async Task DeleteFloorAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Floors WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 部屋情報を全削除してIDを初期化し、CSVデータで再登録する
        /// </summary>
        public static async Task ReplaceFloorsAsync(IEnumerable<Floor> floors)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using (var deleteCmd = new SqlCommand("DELETE FROM Floors", connection, transaction))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                using (var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('[dbo].[Floors]', RESEED, 0);", connection, transaction))
                {
                    await reseedCmd.ExecuteNonQueryAsync();
                }

                var insertQuery = @"INSERT INTO Floors (BuildingId, FloorName, FloorArea, CreatedAt, UpdatedAt)
                                    VALUES (@BuildingId, @FloorName, @FloorArea, @CreatedAt, @UpdatedAt)";
                using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                insertCmd.Parameters.Add("@BuildingId", SqlDbType.Int);
                insertCmd.Parameters.Add("@FloorName", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@FloorArea", SqlDbType.Decimal);
                insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime);
                insertCmd.Parameters.Add("@UpdatedAt", SqlDbType.DateTime);
                insertCmd.Parameters["@FloorArea"].Precision = 18;
                insertCmd.Parameters["@FloorArea"].Scale = 2;

                var now = DateTime.Now;
                foreach (var floor in floors)
                {
                    insertCmd.Parameters["@BuildingId"].Value = floor.BuildingId;
                    insertCmd.Parameters["@FloorName"].Value = floor.FloorName;
                    insertCmd.Parameters["@FloorArea"].Value = floor.FloorArea;
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








