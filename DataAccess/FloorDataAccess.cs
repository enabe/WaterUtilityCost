using System;
using System.Collections.Generic;
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
    }
}








