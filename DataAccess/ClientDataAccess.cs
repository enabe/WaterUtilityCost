using System;
using System.Collections.Generic;
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
                        ISNULL(InvoiceNumber, '') AS InvoiceNumber,
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
                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
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
                        ISNULL(InvoiceNumber, '') AS InvoiceNumber,
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
                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
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
            var query = @"INSERT INTO Clients (Name, IsLessor, IsLessee, IsBillingTo, IsContractor, InvoiceNumber, PostalCode, Address, Phone, CreatedAt, UpdatedAt)
                         VALUES (@Name, @IsLessor, @IsLessee, @IsBillingTo, @IsContractor, @InvoiceNumber, @PostalCode, @Address, @Phone, @CreatedAt, @UpdatedAt);
                         SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Name", (object)client.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsLessor", client.IsLessor);
            cmd.Parameters.AddWithValue("@IsLessee", client.IsLessee);
            cmd.Parameters.AddWithValue("@IsBillingTo", client.IsBillingTo);
            cmd.Parameters.AddWithValue("@IsContractor", client.IsContractor);
            cmd.Parameters.AddWithValue("@InvoiceNumber", (object)client.InvoiceNumber ?? DBNull.Value);
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
                         InvoiceNumber = @InvoiceNumber, 
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
            cmd.Parameters.AddWithValue("@InvoiceNumber", (object)client.InvoiceNumber ?? DBNull.Value);
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
    }
}

