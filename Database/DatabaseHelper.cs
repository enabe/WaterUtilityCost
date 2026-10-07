using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterUtilityCost.Database
{
    /// <summary>
    /// データベース接続とクエリ実行を管理するクラス
    /// </summary>
    public class DatabaseHelper
    {
        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    var configConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"];
                    _connectionString = configConnectionString != null ? configConnectionString.ConnectionString 
                        : "Data Source=.\\SQLEXPRESS01;Initial Catalog=BuildingManagement;Integrated Security=True;";
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// データベース接続をテストする
        /// </summary>
        public static async Task<(bool Success, string ErrorMessage)> TestConnectionAsync()
        {
            try
            {
                // まずMasterデータベースに接続してSQL Serverへの接続をテスト
                var masterConnectionString = ConnectionString.Replace("Initial Catalog=BuildingManagement", "Initial Catalog=master");
                using var connection = new SqlConnection(masterConnectionString);
                await connection.OpenAsync();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// データベースとテーブルを作成する
        /// </summary>
        public static async Task InitializeDatabaseAsync()
        {
            try
            {
                // まずMasterデータベースに接続してデータベースの存在確認と作成
                var masterConnectionString = ConnectionString.Replace("Initial Catalog=BuildingManagement", "Initial Catalog=master");
                using var masterConnection = new SqlConnection(masterConnectionString);
                await masterConnection.OpenAsync();

                // データベースが存在しない場合は作成
                var checkDbQuery = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BuildingManagement') CREATE DATABASE BuildingManagement";
                using var checkDbCmd = new SqlCommand(checkDbQuery, masterConnection);
                await checkDbCmd.ExecuteNonQueryAsync();

                // BuildingManagementデータベースに接続
                using var connection = new SqlConnection(ConnectionString);
                await connection.OpenAsync();

                // Floorsカラムを削除するマイグレーション（コメントアウト - Floorsカラムは必須項目のため）
                // try
                // {
                //     var dropFloorsColumnQuery = @"
                //         IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND type in (N'U'))
                //         BEGIN
                //             -- FloorsTempカラムが残っている場合は削除
                //             IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND name = 'FloorsTemp')
                //             BEGIN
                //                 BEGIN TRY
                //                     ALTER TABLE [dbo].[Buildings] DROP COLUMN [FloorsTemp];
                //                 END TRY
                //                 BEGIN CATCH
                //                     -- エラーを無視して続行
                //                 END CATCH
                //             END
                //             
                //             -- Floorsカラムが存在する場合は削除
                //             IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND name = 'Floors')
                //             BEGIN
                //                 BEGIN TRY
                //                     ALTER TABLE [dbo].[Buildings] DROP COLUMN [Floors];
                //                 END TRY
                //                 BEGIN CATCH
                //                     -- エラーを無視して続行
                //                 END CATCH
                //             END
                //         END";
                //     using var dropCmd = new SqlCommand(dropFloorsColumnQuery, connection);
                //     dropCmd.CommandTimeout = 60;
                //     await dropCmd.ExecuteNonQueryAsync();
                // }
                // catch (Exception ex)
                // {
                //     // エラーをログに記録
                //     System.Diagnostics.Debug.WriteLine($"Floorsカラムの削除でエラーが発生しました: {ex.Message}");
                // }

                // Buildingsテーブルの作成
                var createBuildingsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND type in (N'U'))
                    CREATE TABLE [dbo].[Buildings] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [BuildingId] NVARCHAR(50),
                        [Name] NVARCHAR(100) NOT NULL,
                        [Address] NVARCHAR(200),
                        [Floors] INT NOT NULL DEFAULT 1,
                        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                        [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
                    )";

                // UtilityCostsテーブルの作成
                var createUtilityCostsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UtilityCosts]') AND type in (N'U'))
                    CREATE TABLE [dbo].[UtilityCosts] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [BuildingId] INT NOT NULL,
                        [RecordDate] DATETIME NOT NULL,
                        [WaterCost] DECIMAL(18,2) NOT NULL,
                        [ElectricityCost] DECIMAL(18,2) NOT NULL,
                        [GasCost] DECIMAL(18,2) NOT NULL,
                        [Notes] NVARCHAR(500),
                        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                        FOREIGN KEY ([BuildingId]) REFERENCES [dbo].[Buildings]([Id]) ON DELETE CASCADE
                    )";

                // Clientsテーブルの作成
                var createClientsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
                    CREATE TABLE [dbo].[Clients] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [Name] NVARCHAR(100),
                        [IsLessor] BIT NOT NULL DEFAULT 0,
                        [IsLessee] BIT NOT NULL DEFAULT 0,
                        [IsBillingTo] BIT NOT NULL DEFAULT 0,
                        [IsContractor] BIT NOT NULL DEFAULT 0,
                        [IsAutoTransfer] BIT NOT NULL DEFAULT 0,
                        [IsBankTransfer] BIT NOT NULL DEFAULT 0,
                        [InvoiceNumber] NVARCHAR(50),
                        [BuildingName] NVARCHAR(100),
                        [RoomName] NVARCHAR(100),
                        [PostalCode] NVARCHAR(10),
                        [Address] NVARCHAR(200),
                        [Phone] NVARCHAR(20),
                        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                        [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
                    )";

                // Clientsテーブルの既存カラムを削除して新しいカラムを追加（マイグレーション）
                var migrateClientsTable = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
                    BEGIN
                        -- 古いカラムが存在する場合は削除
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'CompanyName')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [CompanyName];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'ContactPerson')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [ContactPerson];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'Email')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [Email];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'BusinessType')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [BusinessType];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'Notes')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [Notes];
                        END
                        
                        -- 誤ったカラム名を削除（タイプミス修正）
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'lsLessor')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [lsLessor];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'lsLessee')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [lsLessee];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'lsBillingTo')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [lsBillingTo];
                        END
                        
                        -- ClientIdカラムを削除（存在する場合）
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'ClientId')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [ClientId];
                        END
                        
                        -- BuildingIdカラムと外部キー制約を削除（存在する場合）
                        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clients_Buildings')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP CONSTRAINT FK_Clients_Buildings;
                        END
                        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[Clients]') AND referenced_object_id = OBJECT_ID(N'[dbo].[Buildings]'))
                        BEGIN
                            DECLARE @fkName NVARCHAR(128);
                            SELECT @fkName = name FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[Clients]') AND referenced_object_id = OBJECT_ID(N'[dbo].[Buildings]');
                            IF @fkName IS NOT NULL
                            BEGIN
                                EXEC('ALTER TABLE [dbo].[Clients] DROP CONSTRAINT ' + @fkName);
                            END
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'BuildingId')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [BuildingId];
                        END
                        
                        -- FloorIdカラムと外部キー制約を削除（存在する場合）
                        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clients_Floors')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP CONSTRAINT FK_Clients_Floors;
                        END
                        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[Clients]') AND referenced_object_id = OBJECT_ID(N'[dbo].[Floors]'))
                        BEGIN
                            DECLARE @fkFloorName NVARCHAR(128);
                            SELECT @fkFloorName = name FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[Clients]') AND referenced_object_id = OBJECT_ID(N'[dbo].[Floors]');
                            IF @fkFloorName IS NOT NULL
                            BEGIN
                                EXEC('ALTER TABLE [dbo].[Clients] DROP CONSTRAINT ' + @fkFloorName);
                            END
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'FloorId')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] DROP COLUMN [FloorId];
                        END
                        
                        -- 新しいカラムを追加（存在しない場合のみ）
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'Name')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [Name] NVARCHAR(100);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'IsLessor')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [IsLessor] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'IsLessee')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [IsLessee] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'IsBillingTo')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [IsBillingTo] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'IsContractor')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [IsContractor] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'IsAutoTransfer')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [IsAutoTransfer] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'IsBankTransfer')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [IsBankTransfer] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'InvoiceNumber')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [InvoiceNumber] NVARCHAR(50);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'BuildingName')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [BuildingName] NVARCHAR(100);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'RoomName')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [RoomName] NVARCHAR(100);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'PostalCode')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [PostalCode] NVARCHAR(10);
                        END
                    END";

    // InvoiceDetailsテーブルの作成
    var createInvoiceDetailsTable = @"
        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND type in (N'U'))
        CREATE TABLE [dbo].[InvoiceDetails] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [BillingTo] NVARCHAR(100),
            [Lessor] NVARCHAR(100),
            [BuildingName] NVARCHAR(100),
            [Lessee] NVARCHAR(100),
            [RoomNumber] NVARCHAR(50),
            [RoomArea] DECIMAL(18,2),
            [Category] NVARCHAR(50),
            [Content] NVARCHAR(200),
            [ChildMeterUsage] DECIMAL(18,2),
            [UsageAmount] DECIMAL(18,2),
            [Unit] NVARCHAR(20),
            [TaxInclusiveAmount] DECIMAL(18,2),
            [TaxRate] DECIMAL(5,2),
            [ChildMeterStartDate] DATETIME,
            [ChildMeterEndDate] DATETIME,
            [ParentMeterStartDate] DATETIME,
            [ParentMeterEndDate] DATETIME,
            [ConfirmedBillingDate] DATETIME,
            [BillingYearMonth] NVARCHAR(7),
            [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
            [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
        )";

        // WaterBillingsテーブルの作成
        var createWaterBillingsTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
            CREATE TABLE [dbo].[WaterBillings] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [BillingYearMonth] NVARCHAR(7) NOT NULL,
                [BuildingName] NVARCHAR(100) NOT NULL,
                [UsageAmount] DECIMAL(18,2) NOT NULL,
                [StartDate] DATETIME NOT NULL,
                [EndDate] DATETIME NOT NULL,
                [BasicCharge] DECIMAL(18,2) NOT NULL,
                [UsageCharge] DECIMAL(18,2) NOT NULL,
                [TaxRate] DECIMAL(18,2) NULL,
                [CustomerNumber] NVARCHAR(50) NOT NULL,
                [ParentMeterId] INT NULL,
                [ContractorId] INT NULL,
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id])
            )";

        // ElectricBillingsテーブルの作成
        var createElectricBillingsTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND type in (N'U'))
            CREATE TABLE [dbo].[ElectricBillings] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [BillingYearMonth] NVARCHAR(7) NOT NULL,
                [BuildingName] NVARCHAR(100) NOT NULL,
                [UsageAmount] DECIMAL(18,2) NOT NULL,
                [StartDate] DATETIME NOT NULL,
                [EndDate] DATETIME NOT NULL,
                [BasicCharge] DECIMAL(18,2) NOT NULL,
                [PowerCharge] DECIMAL(18,2) NOT NULL,
                [CustomerNumber] NVARCHAR(50) NOT NULL,
                [TaxRate] DECIMAL(18,2) NULL,
                [ContractorId] INT NULL,
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id])
            )";

        // GasBillingsテーブルの作成
        var createGasBillingsTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
            CREATE TABLE [dbo].[GasBillings] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [BillingYearMonth] NVARCHAR(7) NOT NULL,
                [BuildingName] NVARCHAR(100) NOT NULL,
                [FloorName] NVARCHAR(100) NOT NULL,
                [ParentMeterId] INT NULL,
                [UsageAmount] DECIMAL(18,2) NOT NULL,
                [StartDate] DATETIME NOT NULL,
                [EndDate] DATETIME NOT NULL,
                [BasicCharge] DECIMAL(18,2) NOT NULL,
                [UsageCharge] DECIMAL(18,2) NOT NULL,
                [TaxRate] DECIMAL(18,2) NULL,
                [CustomerNumber] NVARCHAR(50) NOT NULL,
                [ContractorId] INT NULL,
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id])
            )";

        // Contractsテーブルの作成
        var createContractsTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND type in (N'U'))
            CREATE TABLE [dbo].[Contracts] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [ContractNumber] NVARCHAR(50),
                [ContractType] NVARCHAR(50),
                [ContractorName] NVARCHAR(100),
                [LessorClientId] INT,
                [LesseeClientId] INT,
                [BillingClientId] INT,
                [StartDate] DATETIME,
                [EndDate] DATETIME,
                [ContractStatus] NVARCHAR(50),
                [ClosingDate] INT,
                [BuildingId] INT,
                [CustomerNumber] NVARCHAR(50),
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([LessorClientId]) REFERENCES [dbo].[Clients]([Id]),
                FOREIGN KEY ([LesseeClientId]) REFERENCES [dbo].[Clients]([Id]),
                FOREIGN KEY ([BillingClientId]) REFERENCES [dbo].[Clients]([Id]),
                FOREIGN KEY ([BuildingId]) REFERENCES [dbo].[Buildings]([Id])
            )";

        // Metersテーブルの作成
        var createMetersTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND type in (N'U'))
            CREATE TABLE [dbo].[Meters] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [BuildingId] INT,
                [ContractorId] INT,
                [MeterType] NVARCHAR(50) NOT NULL,
                [ManagementNumber] NVARCHAR(50),
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([BuildingId]) REFERENCES [dbo].[Buildings]([Id]),
                FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id])
            )";

        // Floorsテーブルの作成
        var createFloorsTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Floors]') AND type in (N'U'))
            CREATE TABLE [dbo].[Floors] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [BuildingId] INT NOT NULL,
                [FloorName] NVARCHAR(100) NOT NULL,
                [FloorArea] DECIMAL(18,2),
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([BuildingId]) REFERENCES [dbo].[Buildings]([Id]) ON DELETE CASCADE
            )";

        // ChildMetersテーブルの作成
        var createChildMetersTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
            CREATE TABLE [dbo].[ChildMeters] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [ParentMeterId] INT,
                [Notes] NVARCHAR(500),
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([ParentMeterId]) REFERENCES [dbo].[Meters]([Id])
            )";

        // ChildMeterReadingsテーブルの作成
        var createChildMeterReadingsTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') AND type in (N'U'))
            CREATE TABLE [dbo].[ChildMeterReadings] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [FloorId] INT NOT NULL,
                [ReadingDate] DATETIME NOT NULL,
                [Type] NVARCHAR(50),
                [MeterValue] DECIMAL(18,2) NOT NULL,
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([FloorId]) REFERENCES [dbo].[Floors]([Id])
            )";

                using var cmd1 = new SqlCommand(createBuildingsTable, connection);
                await cmd1.ExecuteNonQueryAsync();

                // 既存のBuildingsテーブルにFloorsカラムを追加（存在しない場合）
                var addFloorsColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND name = 'Floors')
                        BEGIN
                            ALTER TABLE [dbo].[Buildings] ADD [Floors] INT NOT NULL DEFAULT 1;
                        END
                    END";
                using var cmd1migrate = new SqlCommand(addFloorsColumnQuery, connection);
                await cmd1migrate.ExecuteNonQueryAsync();

                // 既存のBuildingsテーブルからBuiltDate, Area, Owner, Contactカラムを削除（存在する場合）
                var removeBuildingColumnsQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND type in (N'U'))
                    BEGIN
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND name = 'BuiltDate')
                        BEGIN
                            ALTER TABLE [dbo].[Buildings] DROP COLUMN [BuiltDate];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND name = 'Area')
                        BEGIN
                            ALTER TABLE [dbo].[Buildings] DROP COLUMN [Area];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND name = 'Owner')
                        BEGIN
                            ALTER TABLE [dbo].[Buildings] DROP COLUMN [Owner];
                        END
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND name = 'Contact')
                        BEGIN
                            ALTER TABLE [dbo].[Buildings] DROP COLUMN [Contact];
                        END
                    END";
                using var cmd1removeColumns = new SqlCommand(removeBuildingColumnsQuery, connection);
                await cmd1removeColumns.ExecuteNonQueryAsync();

                using var cmd2 = new SqlCommand(createUtilityCostsTable, connection);
                await cmd2.ExecuteNonQueryAsync();

                using var cmd3 = new SqlCommand(createClientsTable, connection);
                await cmd3.ExecuteNonQueryAsync();

                using var cmd3migrate = new SqlCommand(migrateClientsTable, connection);
                await cmd3migrate.ExecuteNonQueryAsync();

                // BuildingNameとRoomNameカラムが確実に存在することを保証
                var ensureBuildingNameAndRoomNameColumns = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'BuildingName')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [BuildingName] NVARCHAR(100);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'RoomName')
                        BEGIN
                            ALTER TABLE [dbo].[Clients] ADD [RoomName] NVARCHAR(100);
                        END
                    END";
                using var cmd3ensureColumns = new SqlCommand(ensureBuildingNameAndRoomNameColumns, connection);
                await cmd3ensureColumns.ExecuteNonQueryAsync();

                // 列順序の並べ替えは適用済みのため IF 1 = 0 で無効化している。
                // この処理はClientsを一時テーブル経由で作り直すが、コピーが失敗してもCATCHが
                // 握り潰して続行するため、中身が空のClientsだけが残りデータが全消失する。
                // 参照元の外部キーも事前に削除されるため、後続のFK再作成がエラー547で失敗する。
                var reorderClientsTableColumns = @"
                    IF 1 = 0
                    BEGIN
                        BEGIN TRY
                            -- 一時テーブルが残っている場合は削除
                            IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients_Temp]') AND type in (N'U'))
                            BEGIN
                                DROP TABLE [dbo].[Clients_Temp];
                            END
                            
                            -- 一時テーブルを作成（新しい順序で）
                            CREATE TABLE [dbo].[Clients_Temp] (
                                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                [Name] NVARCHAR(100),
                                [IsLessor] BIT NOT NULL DEFAULT 0,
                                [IsLessee] BIT NOT NULL DEFAULT 0,
                                [IsBillingTo] BIT NOT NULL DEFAULT 0,
                                [IsContractor] BIT NOT NULL DEFAULT 0,
                                [IsAutoTransfer] BIT NOT NULL DEFAULT 0,
                                [IsBankTransfer] BIT NOT NULL DEFAULT 0,
                                [InvoiceNumber] NVARCHAR(50),
                                [BuildingName] NVARCHAR(100),
                                [RoomName] NVARCHAR(100),
                                [PostalCode] NVARCHAR(10),
                                [Address] NVARCHAR(200),
                                [Phone] NVARCHAR(20),
                                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
                            );
                            
                            -- データをコピー（IDENTITY_INSERTを使用）
                            IF EXISTS (SELECT 1 FROM [dbo].[Clients])
                            BEGIN
                                SET IDENTITY_INSERT [dbo].[Clients_Temp] ON;
                                
                                -- BuildingNameとRoomNameカラムの存在を確認
                                DECLARE @HasBuildingName BIT = 0;
                                DECLARE @HasRoomName BIT = 0;
                                
                                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'BuildingName')
                                BEGIN
                                    SET @HasBuildingName = 1;
                                END
                                
                                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'RoomName')
                                BEGIN
                                    SET @HasRoomName = 1;
                                END
                                
                                -- カラムの存在に応じてSELECT文を動的に構築
                                DECLARE @sqlCopy NVARCHAR(MAX);
                                
                                IF @HasBuildingName = 1 AND @HasRoomName = 1
                                BEGIN
                                    SET @sqlCopy = N'INSERT INTO [dbo].[Clients_Temp] ([Id], [Name], [IsLessor], [IsLessee], [IsBillingTo], [IsContractor], [IsAutoTransfer], [IsBankTransfer], [InvoiceNumber], [BuildingName], [RoomName], [PostalCode], [Address], [Phone], [CreatedAt], [UpdatedAt])
                                    SELECT [Id], [Name], [IsLessor], [IsLessee], [IsBillingTo], [IsContractor], ISNULL([IsAutoTransfer], 0), ISNULL([IsBankTransfer], 0), [InvoiceNumber], ISNULL([BuildingName], '''') AS [BuildingName], ISNULL([RoomName], '''') AS [RoomName], [PostalCode], [Address], [Phone], [CreatedAt], [UpdatedAt]
                                    FROM [dbo].[Clients]';
                                END
                                ELSE IF @HasBuildingName = 1
                                BEGIN
                                    SET @sqlCopy = N'INSERT INTO [dbo].[Clients_Temp] ([Id], [Name], [IsLessor], [IsLessee], [IsBillingTo], [IsContractor], [IsAutoTransfer], [IsBankTransfer], [InvoiceNumber], [BuildingName], [RoomName], [PostalCode], [Address], [Phone], [CreatedAt], [UpdatedAt])
                                    SELECT [Id], [Name], [IsLessor], [IsLessee], [IsBillingTo], [IsContractor], ISNULL([IsAutoTransfer], 0), ISNULL([IsBankTransfer], 0), [InvoiceNumber], ISNULL([BuildingName], '''') AS [BuildingName], '''' AS [RoomName], [PostalCode], [Address], [Phone], [CreatedAt], [UpdatedAt]
                                    FROM [dbo].[Clients]';
                                END
                                ELSE IF @HasRoomName = 1
                                BEGIN
                                    SET @sqlCopy = N'INSERT INTO [dbo].[Clients_Temp] ([Id], [Name], [IsLessor], [IsLessee], [IsBillingTo], [IsContractor], [IsAutoTransfer], [IsBankTransfer], [InvoiceNumber], [BuildingName], [RoomName], [PostalCode], [Address], [Phone], [CreatedAt], [UpdatedAt])
                                    SELECT [Id], [Name], [IsLessor], [IsLessee], [IsBillingTo], [IsContractor], ISNULL([IsAutoTransfer], 0), ISNULL([IsBankTransfer], 0), [InvoiceNumber], '''' AS [BuildingName], ISNULL([RoomName], '''') AS [RoomName], [PostalCode], [Address], [Phone], [CreatedAt], [UpdatedAt]
                                    FROM [dbo].[Clients]';
                                END
                                ELSE
                                BEGIN
                                    SET @sqlCopy = N'INSERT INTO [dbo].[Clients_Temp] ([Id], [Name], [IsLessor], [IsLessee], [IsBillingTo], [IsContractor], [IsAutoTransfer], [IsBankTransfer], [InvoiceNumber], [BuildingName], [RoomName], [PostalCode], [Address], [Phone], [CreatedAt], [UpdatedAt])
                                    SELECT [Id], [Name], [IsLessor], [IsLessee], [IsBillingTo], [IsContractor], ISNULL([IsAutoTransfer], 0), ISNULL([IsBankTransfer], 0), [InvoiceNumber], '''' AS [BuildingName], '''' AS [RoomName], [PostalCode], [Address], [Phone], [CreatedAt], [UpdatedAt]
                                    FROM [dbo].[Clients]';
                                END
                                
                                EXEC sp_executesql @sqlCopy;
                                SET IDENTITY_INSERT [dbo].[Clients_Temp] OFF;
                            END
                            
                            -- 外部キー制約を削除（参照しているテーブルから）
                            DECLARE @sql NVARCHAR(MAX) = '';
                            SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
                            FROM sys.foreign_keys
                            WHERE referenced_object_id = OBJECT_ID(N'[dbo].[Clients]');
                            IF @sql <> ''
                            BEGIN
                                EXEC sp_executesql @sql;
                            END
                            
                            -- 古いテーブルを削除
                            DROP TABLE [dbo].[Clients];
                            
                            -- 一時テーブルをリネーム
                            EXEC sp_rename '[dbo].[Clients_Temp]', 'Clients';
                            
                            -- 外部キー制約を再作成
                            -- Contractsテーブルの外部キー制約を再作成
                            IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND type in (N'U'))
                            BEGIN
                                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'LessorClientId')
                                AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Contracts_LessorClient')
                                BEGIN
                                    ALTER TABLE [dbo].[Contracts] ADD CONSTRAINT FK_Contracts_LessorClient FOREIGN KEY ([LessorClientId]) REFERENCES [dbo].[Clients]([Id]);
                                END
                                
                                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'LesseeClientId')
                                AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Contracts_LesseeClient')
                                BEGIN
                                    ALTER TABLE [dbo].[Contracts] ADD CONSTRAINT FK_Contracts_LesseeClient FOREIGN KEY ([LesseeClientId]) REFERENCES [dbo].[Clients]([Id]);
                                END
                                
                                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'BillingClientId')
                                AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Contracts_BillingClient')
                                BEGIN
                                    ALTER TABLE [dbo].[Contracts] ADD CONSTRAINT FK_Contracts_BillingClient FOREIGN KEY ([BillingClientId]) REFERENCES [dbo].[Clients]([Id]);
                                END
                            END
                            
                            -- Metersテーブルの外部キー制約を再作成
                            IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND type in (N'U'))
                            BEGIN
                                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND name = 'ContractorId')
                                AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Meters_Clients')
                                BEGIN
                                    ALTER TABLE [dbo].[Meters] ADD CONSTRAINT FK_Meters_Clients FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                                END
                            END
                        END TRY
                        BEGIN CATCH
                            -- エラーが発生した場合、一時テーブルを削除して続行
                            IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients_Temp]') AND type in (N'U'))
                            BEGIN
                                DROP TABLE [dbo].[Clients_Temp];
                            END
                            -- エラーを再スロー（重要なエラーの場合）
                            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
                            BEGIN
                                DECLARE @ErrorMessageClients NVARCHAR(4000) = ERROR_MESSAGE();
                                DECLARE @ErrorSeverityClients INT = ERROR_SEVERITY();
                                DECLARE @ErrorStateClients INT = ERROR_STATE();
                                RAISERROR(@ErrorMessageClients, @ErrorSeverityClients, @ErrorStateClients);
                            END
                        END CATCH
                    END";
                using var cmd3reorder = new SqlCommand(reorderClientsTableColumns, connection);
                cmd3reorder.CommandTimeout = 120;
                await cmd3reorder.ExecuteNonQueryAsync();

                using var cmd4 = new SqlCommand(createInvoiceDetailsTable, connection);
                await cmd4.ExecuteNonQueryAsync();

                // InvoiceDetailsテーブルにContractorとInvoiceNumberカラムを追加（存在しない場合）
                var addInvoiceDetailContractorColumnsQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND name = 'Contractor')
                        BEGIN
                            ALTER TABLE [dbo].[InvoiceDetails] ADD [Contractor] NVARCHAR(100);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND name = 'InvoiceNumber')
                        BEGIN
                            ALTER TABLE [dbo].[InvoiceDetails] ADD [InvoiceNumber] NVARCHAR(50);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND name = 'RoomArea')
                        BEGIN
                            ALTER TABLE [dbo].[InvoiceDetails] ADD [RoomArea] DECIMAL(18,2);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND name = 'ChildMeterUsage')
                        BEGIN
                            ALTER TABLE [dbo].[InvoiceDetails] ADD [ChildMeterUsage] DECIMAL(18,2);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND name = 'BillingYearMonth')
                        BEGIN
                            ALTER TABLE [dbo].[InvoiceDetails] ADD [BillingYearMonth] NVARCHAR(7);
                        END
                    END";
                using var cmd4migrate = new SqlCommand(addInvoiceDetailContractorColumnsQuery, connection);
                await cmd4migrate.ExecuteNonQueryAsync();

                using var cmd5 = new SqlCommand(createWaterBillingsTable, connection);
                await cmd5.ExecuteNonQueryAsync();

                using var cmd6 = new SqlCommand(createElectricBillingsTable, connection);
                await cmd6.ExecuteNonQueryAsync();

                // ElectricBillingsテーブルからBillingAmountカラムを削除（存在する場合）
                var removeElectricBillingAmountColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND type in (N'U'))
                    BEGIN
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND name = 'BillingAmount')
                        BEGIN
                            ALTER TABLE [dbo].[ElectricBillings] DROP COLUMN [BillingAmount];
                        END
                    END";
                using var cmd6migrate = new SqlCommand(removeElectricBillingAmountColumnQuery, connection);
                await cmd6migrate.ExecuteNonQueryAsync();

                // 列順序の並べ替えは適用済みのため IF 1 = 0 で無効化している。
                // 一時テーブルにContractorIdが無いため、実行するたびに契約者の紐付けが失われる。
                var reorderElectricBillingsColumnsQuery = @"
                    IF 1 = 0
                    BEGIN
                        BEGIN TRY
                            -- 一時テーブルが残っている場合は削除
                            IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings_Temp]') AND type in (N'U'))
                            BEGIN
                                DROP TABLE [dbo].[ElectricBillings_Temp];
                            END
                            
                            -- 一時テーブルを作成（新しい順序で）
                            CREATE TABLE [dbo].[ElectricBillings_Temp] (
                                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                [BillingYearMonth] NVARCHAR(7) NOT NULL,
                                [BuildingName] NVARCHAR(100) NOT NULL,
                                [UsageAmount] DECIMAL(18,2) NOT NULL,
                                [StartDate] DATETIME NOT NULL,
                                [EndDate] DATETIME NOT NULL,
                                [BasicCharge] DECIMAL(18,2) NOT NULL,
                                [PowerCharge] DECIMAL(18,2) NOT NULL,
                                [CustomerNumber] NVARCHAR(50) NOT NULL,
                                [TaxRate] DECIMAL(18,2) NULL,
                                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
                            );
                            
                            -- データをコピー（IDENTITY_INSERTを使用）
                            IF EXISTS (SELECT 1 FROM [dbo].[ElectricBillings])
                            BEGIN
                                SET IDENTITY_INSERT [dbo].[ElectricBillings_Temp] ON;
                                INSERT INTO [dbo].[ElectricBillings_Temp] ([Id], [BillingYearMonth], [BuildingName], [UsageAmount], [StartDate], [EndDate], [BasicCharge], [PowerCharge], [CustomerNumber], [TaxRate], [CreatedAt], [UpdatedAt])
                                SELECT [Id], [BillingYearMonth], [BuildingName], [UsageAmount], [StartDate], [EndDate], [BasicCharge], [PowerCharge], [CustomerNumber], [TaxRate], [CreatedAt], [UpdatedAt]
                                FROM [dbo].[ElectricBillings];
                                SET IDENTITY_INSERT [dbo].[ElectricBillings_Temp] OFF;
                            END
                            
                            -- 古いテーブルを削除
                            DROP TABLE [dbo].[ElectricBillings];
                            
                            -- 一時テーブルをリネーム
                            EXEC sp_rename '[dbo].[ElectricBillings_Temp]', 'ElectricBillings';
                        END TRY
                        BEGIN CATCH
                            -- エラーが発生した場合、一時テーブルを削除
                            IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings_Temp]') AND type in (N'U'))
                            BEGIN
                                DROP TABLE [dbo].[ElectricBillings_Temp];
                            END
                            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                            DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
                            DECLARE @ErrorState INT = ERROR_STATE();
                            RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
                        END CATCH
                    END";
                using var cmd6reorder = new SqlCommand(reorderElectricBillingsColumnsQuery, connection);
                cmd6reorder.CommandTimeout = 120;
                await cmd6reorder.ExecuteNonQueryAsync();

                // ElectricBillingsテーブルにTaxRateカラムを追加（存在しない場合）
                var addElectricBillingTaxRateColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND name = 'TaxRate')
                        BEGIN
                            ALTER TABLE [dbo].[ElectricBillings] ADD [TaxRate] DECIMAL(18,2) NULL;
                        END
                    END";
                using var cmd6migrate2 = new SqlCommand(addElectricBillingTaxRateColumnQuery, connection);
                await cmd6migrate2.ExecuteNonQueryAsync();

                // ElectricBillingsテーブルにContractorIdカラムと外部キーを追加（存在しない場合）
                var addElectricBillingContractorIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND name = 'ContractorId')
                        BEGIN
                            ALTER TABLE [dbo].[ElectricBillings] ADD [ContractorId] INT NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ElectricBillings_Clients_ContractorId')
                        BEGIN
                            DECLARE @electricFkSql NVARCHAR(MAX) = N'
                                IF EXISTS (SELECT 1 FROM [dbo].[ElectricBillings] e
                                           WHERE e.[ContractorId] IS NOT NULL
                                             AND NOT EXISTS (SELECT 1 FROM [dbo].[Clients] c WHERE c.[Id] = e.[ContractorId]))
                                BEGIN
                                    ALTER TABLE [dbo].[ElectricBillings] WITH NOCHECK
                                    ADD CONSTRAINT FK_ElectricBillings_Clients_ContractorId
                                    FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                                END
                                ELSE
                                BEGIN
                                    ALTER TABLE [dbo].[ElectricBillings]
                                    ADD CONSTRAINT FK_ElectricBillings_Clients_ContractorId
                                    FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                                END';
                            EXEC sp_executesql @electricFkSql;
                        END
                    END";
                using var cmd6migrate3 = new SqlCommand(addElectricBillingContractorIdColumnQuery, connection);
                await cmd6migrate3.ExecuteNonQueryAsync();

                // WaterBillingsテーブルからBillingAmountカラムを削除（存在する場合）
                var removeWaterBillingAmountColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    BEGIN
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND name = 'BillingAmount')
                        BEGIN
                            ALTER TABLE [dbo].[WaterBillings] DROP COLUMN [BillingAmount];
                        END
                    END";
                using var cmd5migrate = new SqlCommand(removeWaterBillingAmountColumnQuery, connection);
                await cmd5migrate.ExecuteNonQueryAsync();

                // WaterBillingsテーブルにBasicChargeとUsageChargeカラムを追加（存在しない場合）
                var addWaterBillingChargesColumnsQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND name = 'BasicCharge')
                        BEGIN
                            ALTER TABLE [dbo].[WaterBillings] ADD [BasicCharge] DECIMAL(18,2) NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND name = 'UsageCharge')
                        BEGIN
                            ALTER TABLE [dbo].[WaterBillings] ADD [UsageCharge] DECIMAL(18,2) NOT NULL DEFAULT 0;
                        END
                    END";
                using var cmd5migrate2 = new SqlCommand(addWaterBillingChargesColumnsQuery, connection);
                await cmd5migrate2.ExecuteNonQueryAsync();

                // WaterBillingsテーブルにTaxRateカラムを追加（存在しない場合）
                var addWaterBillingTaxRateColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND name = 'TaxRate')
                        BEGIN
                            ALTER TABLE [dbo].[WaterBillings] ADD [TaxRate] DECIMAL(18,2) NULL;
                        END
                    END";
                using var cmd5migrate3 = new SqlCommand(addWaterBillingTaxRateColumnQuery, connection);
                await cmd5migrate3.ExecuteNonQueryAsync();

                // WaterBillingsテーブルに差額割当先部屋名カラムを追加（存在しない場合）
                var addWaterBillingDifferenceAssignmentRoomQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND name = 'DifferenceAssignmentRoomName')
                        BEGIN
                            ALTER TABLE [dbo].[WaterBillings] ADD [DifferenceAssignmentRoomName] NVARCHAR(100) NULL;
                        END
                    END";
                using var cmd5migrate4 = new SqlCommand(addWaterBillingDifferenceAssignmentRoomQuery, connection);
                await cmd5migrate4.ExecuteNonQueryAsync();

                // WaterBillingsテーブルにContractorIdカラムと外部キーを追加（存在しない場合）
                var addWaterBillingContractorIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND name = 'ContractorId')
                        BEGIN
                            ALTER TABLE [dbo].[WaterBillings] ADD [ContractorId] INT NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_WaterBillings_Clients_ContractorId')
                        BEGIN
                            -- 参照先を失ったContractorIdが残っていても起動できるよう、
                            -- 不整合がある場合は検証なしで作成して既存データを保持する。
                            -- ContractorId列を直前に追加した場合に備え動的SQLで遅延コンパイルする。
                            DECLARE @waterFkSql NVARCHAR(MAX) = N'
                                IF EXISTS (SELECT 1 FROM [dbo].[WaterBillings] w
                                           WHERE w.[ContractorId] IS NOT NULL
                                             AND NOT EXISTS (SELECT 1 FROM [dbo].[Clients] c WHERE c.[Id] = w.[ContractorId]))
                                BEGIN
                                    ALTER TABLE [dbo].[WaterBillings] WITH NOCHECK
                                    ADD CONSTRAINT FK_WaterBillings_Clients_ContractorId
                                    FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                                END
                                ELSE
                                BEGIN
                                    ALTER TABLE [dbo].[WaterBillings]
                                    ADD CONSTRAINT FK_WaterBillings_Clients_ContractorId
                                    FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                                END';
                            EXEC sp_executesql @waterFkSql;
                        END
                    END";
                using var cmd5migrate5 = new SqlCommand(addWaterBillingContractorIdColumnQuery, connection);
                await cmd5migrate5.ExecuteNonQueryAsync();

                // WaterBillingsテーブルにParentMeterIdカラムを追加（存在しない場合）
                var addWaterBillingParentMeterIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND name = 'ParentMeterId')
                        BEGIN
                            ALTER TABLE [dbo].[WaterBillings] ADD [ParentMeterId] INT NULL;
                        END
                    END";
                using var cmd5migrate6 = new SqlCommand(addWaterBillingParentMeterIdColumnQuery, connection);
                await cmd5migrate6.ExecuteNonQueryAsync();

                using var cmd7 = new SqlCommand(createGasBillingsTable, connection);
                await cmd7.ExecuteNonQueryAsync();

                // GasBillingsテーブルからBillingAmountカラムを削除（存在する場合）
                var removeGasBillingAmountColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    BEGIN
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND name = 'BillingAmount')
                        BEGIN
                            ALTER TABLE [dbo].[GasBillings] DROP COLUMN [BillingAmount];
                        END
                    END";
                using var cmd7migrate = new SqlCommand(removeGasBillingAmountColumnQuery, connection);
                await cmd7migrate.ExecuteNonQueryAsync();

                // GasBillingsテーブルからDistrictカラムを削除（存在する場合）
                var removeGasBillingDistrictColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    BEGIN
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND name = 'District')
                        BEGIN
                            ALTER TABLE [dbo].[GasBillings] DROP COLUMN [District];
                        END
                    END";
                using var cmd7migrate2 = new SqlCommand(removeGasBillingDistrictColumnQuery, connection);
                await cmd7migrate2.ExecuteNonQueryAsync();

                // GasBillingsテーブルにTaxRateカラムを追加（存在しない場合）
                var addGasBillingTaxRateColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND name = 'TaxRate')
                        BEGIN
                            ALTER TABLE [dbo].[GasBillings] ADD [TaxRate] DECIMAL(18,2) NULL;
                        END
                    END";
                using var cmd7migrate3 = new SqlCommand(addGasBillingTaxRateColumnQuery, connection);
                await cmd7migrate3.ExecuteNonQueryAsync();

                // GasBillingsテーブルにFloorNameカラムを追加（存在しない場合）
                var addGasBillingFloorNameColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND name = 'FloorName')
                        BEGIN
                            ALTER TABLE [dbo].[GasBillings] ADD [FloorName] NVARCHAR(100) NOT NULL DEFAULT('');
                        END
                    END";
                using var cmd7migrate4 = new SqlCommand(addGasBillingFloorNameColumnQuery, connection);
                await cmd7migrate4.ExecuteNonQueryAsync();

                // GasBillingsテーブルにParentMeterIdカラムを追加（存在しない場合）
                var addGasBillingParentMeterIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND name = 'ParentMeterId')
                        BEGIN
                            ALTER TABLE [dbo].[GasBillings] ADD [ParentMeterId] INT NULL;
                        END
                    END";
                using var cmd7migrate5 = new SqlCommand(addGasBillingParentMeterIdColumnQuery, connection);
                await cmd7migrate5.ExecuteNonQueryAsync();

                // GasBillingsテーブルにContractorIdカラムと外部キーを追加（存在しない場合）
                var addGasBillingContractorIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND name = 'ContractorId')
                        BEGIN
                            ALTER TABLE [dbo].[GasBillings] ADD [ContractorId] INT NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_GasBillings_Clients_ContractorId')
                        BEGIN
                            DECLARE @gasFkSql NVARCHAR(MAX) = N'
                                IF EXISTS (SELECT 1 FROM [dbo].[GasBillings] g
                                           WHERE g.[ContractorId] IS NOT NULL
                                             AND NOT EXISTS (SELECT 1 FROM [dbo].[Clients] c WHERE c.[Id] = g.[ContractorId]))
                                BEGIN
                                    ALTER TABLE [dbo].[GasBillings] WITH NOCHECK
                                    ADD CONSTRAINT FK_GasBillings_Clients_ContractorId
                                    FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                                END
                                ELSE
                                BEGIN
                                    ALTER TABLE [dbo].[GasBillings]
                                    ADD CONSTRAINT FK_GasBillings_Clients_ContractorId
                                    FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                                END';
                            EXEC sp_executesql @gasFkSql;
                        END
                    END";
                using var cmd7migrate6 = new SqlCommand(addGasBillingContractorIdColumnQuery, connection);
                await cmd7migrate6.ExecuteNonQueryAsync();

                using var cmd8 = new SqlCommand(createContractsTable, connection);
                await cmd8.ExecuteNonQueryAsync();

                using var cmd9 = new SqlCommand(createMetersTable, connection);
                await cmd9.ExecuteNonQueryAsync();

                // MetersテーブルにContractorIdカラムを追加、MeterIdカラムを削除（存在する場合）
                var addContractorIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND name = 'ContractorId')
                        BEGIN
                            ALTER TABLE [dbo].[Meters] ADD [ContractorId] INT;
                            IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Meters_Clients')
                            BEGIN
                                ALTER TABLE [dbo].[Meters] ADD CONSTRAINT FK_Meters_Clients FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                            END
                        END
                        
                        -- MeterIdカラムを削除（存在する場合）
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND name = 'MeterId')
                        BEGIN
                            ALTER TABLE [dbo].[Meters] DROP COLUMN [MeterId];
                        END
                    END";
                using var cmd9migrate = new SqlCommand(addContractorIdColumnQuery, connection);
                await cmd9migrate.ExecuteNonQueryAsync();

                // MetersテーブルにMeterNameカラムを追加（存在しない場合）
                var addMeterNameColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND name = 'MeterName')
                        BEGIN
                            ALTER TABLE [dbo].[Meters] ADD [MeterName] NVARCHAR(100);
                        END
                    END";
                using var cmd9migrate2 = new SqlCommand(addMeterNameColumnQuery, connection);
                await cmd9migrate2.ExecuteNonQueryAsync();

                // 過去の破壊的なマイグレーションで削除されたままになっている外部キーを復元する。
                // CREATE TABLE時のFKは自動命名されるため、制約名ではなく参照列で存在を判定する。
                // 既存データに不整合があっても起動できるよう WITH NOCHECK で作成する。
                var restoreClientForeignKeysQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND type in (N'U'))
                    BEGIN
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'LessorClientId')
                           AND NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns fkc
                                           WHERE fkc.parent_object_id = OBJECT_ID(N'[dbo].[Contracts]')
                                             AND fkc.referenced_object_id = OBJECT_ID(N'[dbo].[Clients]')
                                             AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'LessorClientId')
                        BEGIN
                            ALTER TABLE [dbo].[Contracts] WITH NOCHECK
                            ADD CONSTRAINT FK_Contracts_LessorClient FOREIGN KEY ([LessorClientId]) REFERENCES [dbo].[Clients]([Id]);
                        END

                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'LesseeClientId')
                           AND NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns fkc
                                           WHERE fkc.parent_object_id = OBJECT_ID(N'[dbo].[Contracts]')
                                             AND fkc.referenced_object_id = OBJECT_ID(N'[dbo].[Clients]')
                                             AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'LesseeClientId')
                        BEGIN
                            ALTER TABLE [dbo].[Contracts] WITH NOCHECK
                            ADD CONSTRAINT FK_Contracts_LesseeClient FOREIGN KEY ([LesseeClientId]) REFERENCES [dbo].[Clients]([Id]);
                        END

                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'BillingClientId')
                           AND NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns fkc
                                           WHERE fkc.parent_object_id = OBJECT_ID(N'[dbo].[Contracts]')
                                             AND fkc.referenced_object_id = OBJECT_ID(N'[dbo].[Clients]')
                                             AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'BillingClientId')
                        BEGIN
                            ALTER TABLE [dbo].[Contracts] WITH NOCHECK
                            ADD CONSTRAINT FK_Contracts_BillingClient FOREIGN KEY ([BillingClientId]) REFERENCES [dbo].[Clients]([Id]);
                        END
                    END

                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND type in (N'U'))
                    BEGIN
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND name = 'ContractorId')
                           AND NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns fkc
                                           WHERE fkc.parent_object_id = OBJECT_ID(N'[dbo].[Meters]')
                                             AND fkc.referenced_object_id = OBJECT_ID(N'[dbo].[Clients]')
                                             AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'ContractorId')
                        BEGIN
                            ALTER TABLE [dbo].[Meters] WITH NOCHECK
                            ADD CONSTRAINT FK_Meters_Clients FOREIGN KEY ([ContractorId]) REFERENCES [dbo].[Clients]([Id]);
                        END
                    END";
                using var cmd9restoreFks = new SqlCommand(restoreClientForeignKeysQuery, connection);
                await cmd9restoreFks.ExecuteNonQueryAsync();

                using var cmd10 = new SqlCommand(createFloorsTable, connection);
                await cmd10.ExecuteNonQueryAsync();

                using var cmd11 = new SqlCommand(createChildMetersTable, connection);
                await cmd11.ExecuteNonQueryAsync();

                // ChildMetersテーブルにParentMeterId列を追加（存在しない場合）
                var addParentMeterIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    BEGIN
                        -- ParentMeterId列が存在しない場合、追加する
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'ParentMeterId')
                           AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'ParentMeterID')
                        BEGIN
                            ALTER TABLE [dbo].[ChildMeters] ADD [ParentMeterId] INT;
                            IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND referenced_object_id = OBJECT_ID(N'[dbo].[Meters]'))
                            BEGIN
                                ALTER TABLE [dbo].[ChildMeters] ADD CONSTRAINT FK_ChildMeters_Meters FOREIGN KEY ([ParentMeterId]) REFERENCES [dbo].[Meters]([Id]);
                            END
                        END
                    END";
                using var cmd11migrate1 = new SqlCommand(addParentMeterIdColumnQuery, connection);
                await cmd11migrate1.ExecuteNonQueryAsync();

                // ChildMetersテーブルのParentMeterID列をParentMeterIdに変更（存在する場合）
                var renameParentMeterIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    BEGIN
                        -- ParentMeterID列が存在し、ParentMeterId列が存在しない場合
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'ParentMeterID')
                           AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'ParentMeterId')
                        BEGIN
                            -- 外部キー制約を削除
                            DECLARE @fkName NVARCHAR(128);
                            SELECT @fkName = fk.name
                            FROM sys.foreign_keys fk
                            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                            WHERE fkc.parent_object_id = OBJECT_ID(N'[dbo].[ChildMeters]')
                              AND fkc.referenced_object_id = OBJECT_ID(N'[dbo].[Meters]')
                              AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'ParentMeterID';
                            
                            IF @fkName IS NOT NULL
                            BEGIN
                                DECLARE @dropFkSql NVARCHAR(MAX) = 'ALTER TABLE [dbo].[ChildMeters] DROP CONSTRAINT ' + QUOTENAME(@fkName);
                                EXEC sp_executesql @dropFkSql;
                            END
                            
                            -- 列名を変更
                            EXEC sp_rename '[dbo].[ChildMeters].[ParentMeterID]', 'ParentMeterId', 'COLUMN';
                            
                            -- 外部キー制約を再作成
                            IF NOT EXISTS (SELECT * FROM sys.foreign_keys fk
                                          INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                                          WHERE fkc.parent_object_id = OBJECT_ID(N'[dbo].[ChildMeters]')
                                            AND fkc.referenced_object_id = OBJECT_ID(N'[dbo].[Meters]'))
                            BEGIN
                                ALTER TABLE [dbo].[ChildMeters] ADD CONSTRAINT FK_ChildMeters_Meters FOREIGN KEY ([ParentMeterId]) REFERENCES [dbo].[Meters]([Id]);
                            END
                        END
                    END";
                using var cmd11migrate2 = new SqlCommand(renameParentMeterIdColumnQuery, connection);
                await cmd11migrate2.ExecuteNonQueryAsync();

                // ChildMetersテーブルからFloorId列を削除（存在する場合）
                var removeFloorIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    BEGIN
                        -- FloorId列が存在する場合、外部キー制約を削除してから列を削除
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'FloorId')
                        BEGIN
                            -- FloorIdに関連する外部キー制約を削除
                            DECLARE @fkNameFloor NVARCHAR(128);
                            SELECT @fkNameFloor = fk.name
                            FROM sys.foreign_keys fk
                            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                            WHERE fkc.parent_object_id = OBJECT_ID(N'[dbo].[ChildMeters]')
                              AND fkc.referenced_object_id = OBJECT_ID(N'[dbo].[Floors]')
                              AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'FloorId';
                            
                            IF @fkNameFloor IS NOT NULL
                            BEGIN
                                DECLARE @dropFkFloorSql NVARCHAR(MAX) = 'ALTER TABLE [dbo].[ChildMeters] DROP CONSTRAINT ' + QUOTENAME(@fkNameFloor);
                                EXEC sp_executesql @dropFkFloorSql;
                            END
                            
                            -- FloorId列を削除
                            ALTER TABLE [dbo].[ChildMeters] DROP COLUMN [FloorId];
                        END
                    END";
                using var cmd11migrate3 = new SqlCommand(removeFloorIdColumnQuery, connection);
                await cmd11migrate3.ExecuteNonQueryAsync();

                // ChildMetersテーブルにBuildingId、RoomId、MeterTypeカラムを追加（存在しない場合）
                var addChildMeterColumnsQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'BuildingId')
                        BEGIN
                            ALTER TABLE [dbo].[ChildMeters] ADD [BuildingId] INT NULL;
                            IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND referenced_object_id = OBJECT_ID(N'[dbo].[Buildings]'))
                            BEGIN
                                ALTER TABLE [dbo].[ChildMeters] ADD CONSTRAINT FK_ChildMeters_Buildings FOREIGN KEY ([BuildingId]) REFERENCES [dbo].[Buildings]([Id]);
                            END
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'RoomId')
                        BEGIN
                            ALTER TABLE [dbo].[ChildMeters] ADD [RoomId] INT NULL;
                            IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND referenced_object_id = OBJECT_ID(N'[dbo].[Floors]'))
                            BEGIN
                                ALTER TABLE [dbo].[ChildMeters] ADD CONSTRAINT FK_ChildMeters_Floors FOREIGN KEY ([RoomId]) REFERENCES [dbo].[Floors]([Id]);
                            END
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'MeterType')
                        BEGIN
                            ALTER TABLE [dbo].[ChildMeters] ADD [MeterType] NVARCHAR(50) NULL;
                        END
                    END";
                using var cmd11migrate4 = new SqlCommand(addChildMeterColumnsQuery, connection);
                await cmd11migrate4.ExecuteNonQueryAsync();

                // ChildMetersテーブルにMeterNameカラムを追加（存在しない場合）
                var addChildMeterNameColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'MeterName')
                        BEGIN
                            ALTER TABLE [dbo].[ChildMeters] ADD [MeterName] NVARCHAR(100);
                        END
                    END";
                using var cmd11migrate6 = new SqlCommand(addChildMeterNameColumnQuery, connection);
                await cmd11migrate6.ExecuteNonQueryAsync();

                // ChildMetersテーブルからRoomIdカラムと外部キー制約を削除（存在する場合）
                var dropRoomIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    BEGIN
                        -- 外部キー制約を削除
                        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'FK_ChildMeters_Floors')
                        BEGIN
                            ALTER TABLE [dbo].[ChildMeters] DROP CONSTRAINT FK_ChildMeters_Floors;
                        END
                        -- RoomIdカラムを削除
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND name = 'RoomId')
                        BEGIN
                            ALTER TABLE [dbo].[ChildMeters] DROP COLUMN [RoomId];
                        END
                    END";
                using var cmd11migrate5 = new SqlCommand(dropRoomIdColumnQuery, connection);
                await cmd11migrate5.ExecuteNonQueryAsync();

                using var cmd12 = new SqlCommand(createChildMeterReadingsTable, connection);
                await cmd12.ExecuteNonQueryAsync();

                // ChildMeterReadingsテーブルにChildMeterIdカラムを追加（存在しない場合）
                var addChildMeterIdColumnQuery = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') AND name = 'ChildMeterId')
                        BEGIN
                            ALTER TABLE [dbo].[ChildMeterReadings] ADD [ChildMeterId] INT NULL;
                            IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') AND referenced_object_id = OBJECT_ID(N'[dbo].[ChildMeters]'))
                            BEGIN
                                ALTER TABLE [dbo].[ChildMeterReadings] ADD CONSTRAINT FK_ChildMeterReadings_ChildMeters FOREIGN KEY ([ChildMeterId]) REFERENCES [dbo].[ChildMeters]([Id]);
                            END
                        END
                    END";
                using var cmd12migrate = new SqlCommand(addChildMeterIdColumnQuery, connection);
                await cmd12migrate.ExecuteNonQueryAsync();

        // RoomChildMetersテーブルの作成
        var createRoomChildMetersTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomChildMeters]') AND type in (N'U'))
            CREATE TABLE [dbo].[RoomChildMeters] (
                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                [FloorId] INT NOT NULL,
                [ChildMeterId] INT NOT NULL,
                [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                FOREIGN KEY ([FloorId]) REFERENCES [dbo].[Floors]([Id]),
                FOREIGN KEY ([ChildMeterId]) REFERENCES [dbo].[ChildMeters]([Id])
            )";
                using var cmd13 = new SqlCommand(createRoomChildMetersTable, connection);
                await cmd13.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"データベース初期化エラー: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// すべてのデータを削除し、IDをリセットする
        /// </summary>
        public static async Task DeleteAllDataAndResetIdsAsync()
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            // 外部キー制約を無効化
            var disableConstraintsQuery = @"
                EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'";
            
            // データ削除（外部キー制約の順序を考慮）
            var deleteDataQuery = @"
                -- 外部キーを持つテーブルから削除
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND type in (N'U'))
                    DELETE FROM [dbo].[InvoiceDetails];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    DELETE FROM [dbo].[WaterBillings];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND type in (N'U'))
                    DELETE FROM [dbo].[ElectricBillings];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    DELETE FROM [dbo].[GasBillings];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND type in (N'U'))
                    DELETE FROM [dbo].[Contracts];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND type in (N'U'))
                    DELETE FROM [dbo].[Meters];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') AND type in (N'U'))
                    DELETE FROM [dbo].[ChildMeterReadings];

                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomChildMeters]') AND type in (N'U'))
                    DELETE FROM [dbo].[RoomChildMeters];

                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    DELETE FROM [dbo].[ChildMeters];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Floors]') AND type in (N'U'))
                    DELETE FROM [dbo].[Floors];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UtilityCosts]') AND type in (N'U'))
                    DELETE FROM [dbo].[UtilityCosts];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
                    DELETE FROM [dbo].[Clients];
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND type in (N'U'))
                    DELETE FROM [dbo].[Buildings];";

            // IDENTITYカラムをリセット
            var resetIdentityQuery = @"
                -- IDENTITYカラムをリセット
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[Buildings]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[Clients]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UtilityCosts]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[UtilityCosts]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[InvoiceDetails]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[WaterBillings]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[ElectricBillings]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[GasBillings]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[ChildMeterReadings]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[Contracts]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[Meters]', RESEED, 0);

                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeters]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[ChildMeters]', RESEED, 0);

                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomChildMeters]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[RoomChildMeters]', RESEED, 0);
                
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Floors]') AND type in (N'U'))
                    DBCC CHECKIDENT ('[dbo].[Floors]', RESEED, 0);";

            // 外部キー制約を再有効化
            var enableConstraintsQuery = @"
                EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL'";

            try
            {
                // 外部キー制約を無効化
                using var cmdDisable = new SqlCommand(disableConstraintsQuery, connection);
                await cmdDisable.ExecuteNonQueryAsync();

                // データ削除
                using var cmdDelete = new SqlCommand(deleteDataQuery, connection);
                cmdDelete.CommandTimeout = 120; // タイムアウトを長めに設定
                await cmdDelete.ExecuteNonQueryAsync();

                // IDENTITYカラムをリセット
                using var cmdReset = new SqlCommand(resetIdentityQuery, connection);
                await cmdReset.ExecuteNonQueryAsync();

                // 外部キー制約を再有効化
                using var cmdEnable = new SqlCommand(enableConstraintsQuery, connection);
                await cmdEnable.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                // エラーが発生した場合でも、外部キー制約を再有効化を試みる
                try
                {
                    using var cmdEnable = new SqlCommand(enableConstraintsQuery, connection);
                    await cmdEnable.ExecuteNonQueryAsync();
                }
                catch
                {
                    // エラーを無視
                }
                throw new Exception($"データ削除エラー: {ex.Message}", ex);
            }
        }
    }
}

