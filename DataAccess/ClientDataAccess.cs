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
    public class ClientDataAccess
    {
        public static async Task<List<Client>> GetAllClientsAsync()
        {
            var clients = new List<Client>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, 
                        ISNULL(Name, '') AS Name,
                        ISNULL(CAST(IsLessor AS BIT), 0) AS IsLessor,
                        ISNULL(CAST(IsLessee AS BIT), 0) AS IsLessee,
                        ISNULL(CAST(IsBillingTo AS BIT), 0) AS IsBillingTo,
                        ISNULL(CAST(IsContractor AS BIT), 0) AS IsContractor,
                        ISNULL(CAST(IsAutoTransfer AS BIT), 0) AS IsAutoTransfer,
                        ISNULL(CAST(IsBankTransfer AS BIT), 0) AS IsBankTransfer,
                        ISNULL(InvoiceNumber, '') AS InvoiceNumber,
                        ISNULL(BuildingName, '') AS BuildingName,
                        ISNULL(RoomName, '') AS RoomName,
                        ISNULL(PostalCode, '') AS PostalCode,
                        ISNULL(Address, '') AS Address,
                        ISNULL(Phone, '') AS Phone,
                        CreatedAt, UpdatedAt
                        FROM Clients 
                        ORDER BY Name";
            using var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                clients.Add(new Client
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    IsLessor = reader.GetBoolean(reader.GetOrdinal("IsLessor")),
                    IsLessee = reader.GetBoolean(reader.GetOrdinal("IsLessee")),
                    IsBillingTo = reader.GetBoolean(reader.GetOrdinal("IsBillingTo")),
                    IsContractor = reader.GetBoolean(reader.GetOrdinal("IsContractor")),
                    IsAutoTransfer = reader.GetBoolean(reader.GetOrdinal("IsAutoTransfer")),
                    IsBankTransfer = reader.GetBoolean(reader.GetOrdinal("IsBankTransfer")),
                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                    PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return clients;
        }

        public static async Task<Client?> GetClientByIdAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, 
                        ISNULL(Name, '') AS Name,
                        ISNULL(CAST(IsLessor AS BIT), 0) AS IsLessor,
                        ISNULL(CAST(IsLessee AS BIT), 0) AS IsLessee,
                        ISNULL(CAST(IsBillingTo AS BIT), 0) AS IsBillingTo,
                        ISNULL(CAST(IsContractor AS BIT), 0) AS IsContractor,
                        ISNULL(CAST(IsAutoTransfer AS BIT), 0) AS IsAutoTransfer,
                        ISNULL(CAST(IsBankTransfer AS BIT), 0) AS IsBankTransfer,
                        ISNULL(InvoiceNumber, '') AS InvoiceNumber,
                        ISNULL(BuildingName, '') AS BuildingName,
                        ISNULL(RoomName, '') AS RoomName,
                        ISNULL(PostalCode, '') AS PostalCode,
                        ISNULL(Address, '') AS Address,
                        ISNULL(Phone, '') AS Phone,
                        CreatedAt, UpdatedAt
                        FROM Clients 
                        WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Client
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    IsLessor = reader.GetBoolean(reader.GetOrdinal("IsLessor")),
                    IsLessee = reader.GetBoolean(reader.GetOrdinal("IsLessee")),
                    IsBillingTo = reader.GetBoolean(reader.GetOrdinal("IsBillingTo")),
                    IsContractor = reader.GetBoolean(reader.GetOrdinal("IsContractor")),
                    IsAutoTransfer = reader.GetBoolean(reader.GetOrdinal("IsAutoTransfer")),
                    IsBankTransfer = reader.GetBoolean(reader.GetOrdinal("IsBankTransfer")),
                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                    PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }
            return null;
        }

        public static async Task<int> CreateClientAsync(Client client)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO Clients (Name, IsLessor, IsLessee, IsBillingTo, IsContractor, IsAutoTransfer, IsBankTransfer, InvoiceNumber, BuildingName, RoomName, PostalCode, Address, Phone, CreatedAt, UpdatedAt)
                         VALUES (@Name, @IsLessor, @IsLessee, @IsBillingTo, @IsContractor, @IsAutoTransfer, @IsBankTransfer, @InvoiceNumber, @BuildingName, @RoomName, @PostalCode, @Address, @Phone, @CreatedAt, @UpdatedAt);
                         SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Name", (object)client.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsLessor", client.IsLessor);
            cmd.Parameters.AddWithValue("@IsLessee", client.IsLessee);
            cmd.Parameters.AddWithValue("@IsBillingTo", client.IsBillingTo);
            cmd.Parameters.AddWithValue("@IsContractor", client.IsContractor);
            cmd.Parameters.AddWithValue("@IsAutoTransfer", client.IsAutoTransfer);
            cmd.Parameters.AddWithValue("@IsBankTransfer", client.IsBankTransfer);
            cmd.Parameters.AddWithValue("@InvoiceNumber", (object)client.InvoiceNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingName", (object)client.BuildingName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomName", (object)client.RoomName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", (object)client.PostalCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object)client.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", (object)client.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            return (int)await cmd.ExecuteScalarAsync();
        }

        public static async Task<bool> UpdateClientAsync(Client client)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE Clients SET 
                         Name = @Name,
                         IsLessor = @IsLessor, 
                         IsLessee = @IsLessee, 
                         IsBillingTo = @IsBillingTo, 
                         IsContractor = @IsContractor, 
                         IsAutoTransfer = @IsAutoTransfer,
                         IsBankTransfer = @IsBankTransfer,
                         InvoiceNumber = @InvoiceNumber, 
                         BuildingName = @BuildingName,
                         RoomName = @RoomName,
                         PostalCode = @PostalCode, 
                         Address = @Address, 
                         Phone = @Phone, 
                         UpdatedAt = @UpdatedAt
                         WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", client.Id);
            cmd.Parameters.AddWithValue("@Name", (object)client.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsLessor", client.IsLessor);
            cmd.Parameters.AddWithValue("@IsLessee", client.IsLessee);
            cmd.Parameters.AddWithValue("@IsBillingTo", client.IsBillingTo);
            cmd.Parameters.AddWithValue("@IsContractor", client.IsContractor);
            cmd.Parameters.AddWithValue("@IsAutoTransfer", client.IsAutoTransfer);
            cmd.Parameters.AddWithValue("@IsBankTransfer", client.IsBankTransfer);
            cmd.Parameters.AddWithValue("@InvoiceNumber", (object)client.InvoiceNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BuildingName", (object)client.BuildingName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomName", (object)client.RoomName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", (object)client.PostalCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object)client.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", (object)client.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteClientAsync(int id)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM Clients WHERE Id = @Id";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        /// <summary>
        /// 取引先情報を追加登録する（既存データは残す）
        /// </summary>
        public static async Task AddClientsAsync(IEnumerable<Client> clients)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                var insertQuery = @"INSERT INTO Clients (Name, IsLessor, IsLessee, IsBillingTo, IsContractor, IsAutoTransfer, IsBankTransfer, InvoiceNumber, BuildingName, RoomName, PostalCode, Address, Phone, CreatedAt, UpdatedAt)
                                    VALUES (@Name, @IsLessor, @IsLessee, @IsBillingTo, @IsContractor, @IsAutoTransfer, @IsBankTransfer, @InvoiceNumber, @BuildingName, @RoomName, @PostalCode, @Address, @Phone, @CreatedAt, @UpdatedAt)";
                using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                insertCmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@IsLessor", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsLessee", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsBillingTo", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsContractor", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsAutoTransfer", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsBankTransfer", SqlDbType.Bit);
                insertCmd.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@BuildingName", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@RoomName", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar, 10);
                insertCmd.Parameters.Add("@Address", SqlDbType.NVarChar, 200);
                insertCmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20);
                insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime);
                insertCmd.Parameters.Add("@UpdatedAt", SqlDbType.DateTime);

                var now = DateTime.Now;
                foreach (var client in clients)
                {
                    insertCmd.Parameters["@Name"].Value = client.Name;
                    insertCmd.Parameters["@IsLessor"].Value = client.IsLessor;
                    insertCmd.Parameters["@IsLessee"].Value = client.IsLessee;
                    insertCmd.Parameters["@IsBillingTo"].Value = client.IsBillingTo;
                    insertCmd.Parameters["@IsContractor"].Value = client.IsContractor;
                    insertCmd.Parameters["@IsAutoTransfer"].Value = client.IsAutoTransfer;
                    insertCmd.Parameters["@IsBankTransfer"].Value = client.IsBankTransfer;
                    insertCmd.Parameters["@InvoiceNumber"].Value = string.IsNullOrWhiteSpace(client.InvoiceNumber)
                        ? DBNull.Value
                        : client.InvoiceNumber;
                    insertCmd.Parameters["@BuildingName"].Value = string.IsNullOrWhiteSpace(client.BuildingName)
                        ? DBNull.Value
                        : client.BuildingName;
                    insertCmd.Parameters["@RoomName"].Value = string.IsNullOrWhiteSpace(client.RoomName)
                        ? DBNull.Value
                        : client.RoomName;
                    insertCmd.Parameters["@PostalCode"].Value = string.IsNullOrWhiteSpace(client.PostalCode)
                        ? DBNull.Value
                        : client.PostalCode;
                    insertCmd.Parameters["@Address"].Value = string.IsNullOrWhiteSpace(client.Address)
                        ? DBNull.Value
                        : client.Address;
                    insertCmd.Parameters["@Phone"].Value = string.IsNullOrWhiteSpace(client.Phone)
                        ? DBNull.Value
                        : client.Phone;
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

        /// <summary>
        /// 取引先情報を全削除してIDを初期化し、CSVデータで再登録する
        /// </summary>
        public static async Task ReplaceClientsAsync(IEnumerable<Client> clients)
        {
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using (var deleteCmd = new SqlCommand("DELETE FROM Clients", connection, transaction))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                using (var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('[dbo].[Clients]', RESEED, 0);", connection, transaction))
                {
                    await reseedCmd.ExecuteNonQueryAsync();
                }

                var insertQuery = @"INSERT INTO Clients (Name, IsLessor, IsLessee, IsBillingTo, IsContractor, IsAutoTransfer, IsBankTransfer, InvoiceNumber, BuildingName, RoomName, PostalCode, Address, Phone, CreatedAt, UpdatedAt)
                                    VALUES (@Name, @IsLessor, @IsLessee, @IsBillingTo, @IsContractor, @IsAutoTransfer, @IsBankTransfer, @InvoiceNumber, @BuildingName, @RoomName, @PostalCode, @Address, @Phone, @CreatedAt, @UpdatedAt)";
                using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                insertCmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@IsLessor", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsLessee", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsBillingTo", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsContractor", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsAutoTransfer", SqlDbType.Bit);
                insertCmd.Parameters.Add("@IsBankTransfer", SqlDbType.Bit);
                insertCmd.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@BuildingName", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@RoomName", SqlDbType.NVarChar, 100);
                insertCmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar, 10);
                insertCmd.Parameters.Add("@Address", SqlDbType.NVarChar, 200);
                insertCmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20);
                insertCmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime);
                insertCmd.Parameters.Add("@UpdatedAt", SqlDbType.DateTime);

                var now = DateTime.Now;
                foreach (var client in clients)
                {
                    insertCmd.Parameters["@Name"].Value = client.Name;
                    insertCmd.Parameters["@IsLessor"].Value = client.IsLessor;
                    insertCmd.Parameters["@IsLessee"].Value = client.IsLessee;
                    insertCmd.Parameters["@IsBillingTo"].Value = client.IsBillingTo;
                    insertCmd.Parameters["@IsContractor"].Value = client.IsContractor;
                    insertCmd.Parameters["@IsAutoTransfer"].Value = client.IsAutoTransfer;
                    insertCmd.Parameters["@IsBankTransfer"].Value = client.IsBankTransfer;
                    insertCmd.Parameters["@InvoiceNumber"].Value = string.IsNullOrWhiteSpace(client.InvoiceNumber)
                        ? DBNull.Value
                        : client.InvoiceNumber;
                    insertCmd.Parameters["@BuildingName"].Value = string.IsNullOrWhiteSpace(client.BuildingName)
                        ? DBNull.Value
                        : client.BuildingName;
                    insertCmd.Parameters["@RoomName"].Value = string.IsNullOrWhiteSpace(client.RoomName)
                        ? DBNull.Value
                        : client.RoomName;
                    insertCmd.Parameters["@PostalCode"].Value = string.IsNullOrWhiteSpace(client.PostalCode)
                        ? DBNull.Value
                        : client.PostalCode;
                    insertCmd.Parameters["@Address"].Value = string.IsNullOrWhiteSpace(client.Address)
                        ? DBNull.Value
                        : client.Address;
                    insertCmd.Parameters["@Phone"].Value = string.IsNullOrWhiteSpace(client.Phone)
                        ? DBNull.Value
                        : client.Phone;
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

        /// <summary>
        /// ビル名と取引先対象が請求先の取引先データを取得
        /// </summary>
        /// <param name="buildingName">ビル名</param>
        /// <returns>取引先データのリスト</returns>
        public static async Task<List<Client>> GetClientsByBuildingNameAndIsBillingToAsync(string buildingName)
        {
            var clients = new List<Client>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, 
                        ISNULL(Name, '') AS Name,
                        ISNULL(CAST(IsLessor AS BIT), 0) AS IsLessor,
                        ISNULL(CAST(IsLessee AS BIT), 0) AS IsLessee,
                        ISNULL(CAST(IsBillingTo AS BIT), 0) AS IsBillingTo,
                        ISNULL(CAST(IsContractor AS BIT), 0) AS IsContractor,
                        ISNULL(CAST(IsAutoTransfer AS BIT), 0) AS IsAutoTransfer,
                        ISNULL(CAST(IsBankTransfer AS BIT), 0) AS IsBankTransfer,
                        ISNULL(InvoiceNumber, '') AS InvoiceNumber,
                        ISNULL(BuildingName, '') AS BuildingName,
                        ISNULL(RoomName, '') AS RoomName,
                        ISNULL(PostalCode, '') AS PostalCode,
                        ISNULL(Address, '') AS Address,
                        ISNULL(Phone, '') AS Phone,
                        CreatedAt, UpdatedAt
                        FROM Clients 
                        WHERE BuildingName = @BuildingName 
                        AND IsBillingTo = 1
                        ORDER BY Name";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingName", buildingName);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                clients.Add(new Client
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    IsLessor = reader.GetBoolean(reader.GetOrdinal("IsLessor")),
                    IsLessee = reader.GetBoolean(reader.GetOrdinal("IsLessee")),
                    IsBillingTo = reader.GetBoolean(reader.GetOrdinal("IsBillingTo")),
                    IsContractor = reader.GetBoolean(reader.GetOrdinal("IsContractor")),
                    IsAutoTransfer = reader.GetBoolean(reader.GetOrdinal("IsAutoTransfer")),
                    IsBankTransfer = reader.GetBoolean(reader.GetOrdinal("IsBankTransfer")),
                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                    PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return clients;
        }

        /// <summary>
        /// ビル名で取引先データを取得
        /// </summary>
        public static async Task<List<Client>> GetClientsByBuildingNameAsync(string buildingName)
        {
            var clients = new List<Client>();
            using var connection = new SqlConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, 
                        ISNULL(Name, '') AS Name,
                        ISNULL(CAST(IsLessor AS BIT), 0) AS IsLessor,
                        ISNULL(CAST(IsLessee AS BIT), 0) AS IsLessee,
                        ISNULL(CAST(IsBillingTo AS BIT), 0) AS IsBillingTo,
                        ISNULL(CAST(IsContractor AS BIT), 0) AS IsContractor,
                        ISNULL(CAST(IsAutoTransfer AS BIT), 0) AS IsAutoTransfer,
                        ISNULL(CAST(IsBankTransfer AS BIT), 0) AS IsBankTransfer,
                        ISNULL(InvoiceNumber, '') AS InvoiceNumber,
                        ISNULL(BuildingName, '') AS BuildingName,
                        ISNULL(RoomName, '') AS RoomName,
                        ISNULL(PostalCode, '') AS PostalCode,
                        ISNULL(Address, '') AS Address,
                        ISNULL(Phone, '') AS Phone,
                        CreatedAt, UpdatedAt
                        FROM Clients 
                        WHERE BuildingName = @BuildingName
                        ORDER BY Name";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BuildingName", buildingName);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                clients.Add(new Client
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    IsLessor = reader.GetBoolean(reader.GetOrdinal("IsLessor")),
                    IsLessee = reader.GetBoolean(reader.GetOrdinal("IsLessee")),
                    IsBillingTo = reader.GetBoolean(reader.GetOrdinal("IsBillingTo")),
                    IsContractor = reader.GetBoolean(reader.GetOrdinal("IsContractor")),
                    IsAutoTransfer = reader.GetBoolean(reader.GetOrdinal("IsAutoTransfer")),
                    IsBankTransfer = reader.GetBoolean(reader.GetOrdinal("IsBankTransfer")),
                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                    RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                    PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }
            return clients;
        }
    }
}

