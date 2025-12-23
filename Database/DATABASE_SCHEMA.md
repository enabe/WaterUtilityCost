# データベーススキーマドキュメント

## データベース情報

- **データベース名**: BuildingManagement
- **接続文字列**: `Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BuildingManagement;Integrated Security=True;`
- **データベースエンジン**: SQL Server (LocalDB)

## テーブル一覧

1. [Buildings](#buildings-テーブル) - ビル管理
2. [UtilityCosts](#utilitycosts-テーブル) - 光熱費記録
3. [Clients](#clients-テーブル) - 取引先管理
4. [InvoiceDetails](#invoicedetails-テーブル) - 請求明細
5. [WaterBillings](#waterbillings-テーブル) - 水道料金請求
6. [ElectricBillings](#electricbillings-テーブル) - 電気料金請求
7. [GasBillings](#gasbillings-テーブル) - ガス料金請求
8. [Contracts](#contracts-テーブル) - 契約管理
9. [Meters](#meters-テーブル) - 親メーター管理

---

## Buildings テーブル

ビル情報を管理するテーブルです。

### テーブル作成SQL

```sql
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND type in (N'U'))
CREATE TABLE [dbo].[Buildings] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(200),
    [Floors] INT NOT NULL,
    [BuiltDate] DATETIME,
    [Area] DECIMAL(18,2),
    [Owner] NVARCHAR(100),
    [Contact] NVARCHAR(50),
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| Name | NVARCHAR(100) | NO | ビル名 |
| Address | NVARCHAR(200) | YES | 住所 |
| Floors | INT | NO | 階数 |
| BuiltDate | DATETIME | YES | 建築日 |
| Area | DECIMAL(18,2) | YES | 面積 |
| Owner | NVARCHAR(100) | YES | 所有者 |
| Contact | NVARCHAR(50) | YES | 連絡先 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |
| UpdatedAt | DATETIME | NO | 更新日時（デフォルト: GETDATE()） |

---

## UtilityCosts テーブル

光熱費の記録を管理するテーブルです。

### テーブル作成SQL

```sql
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
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| BuildingId | INT | NO | ビルID（外部キー: Buildings.Id） |
| RecordDate | DATETIME | NO | 記録日 |
| WaterCost | DECIMAL(18,2) | NO | 水道料金 |
| ElectricityCost | DECIMAL(18,2) | NO | 電気料金 |
| GasCost | DECIMAL(18,2) | NO | ガス料金 |
| Notes | NVARCHAR(500) | YES | 備考 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |

### 外部キー制約

- `BuildingId` → `Buildings.Id` (ON DELETE CASCADE)

---

## Clients テーブル

取引先情報を管理するテーブルです。

### テーブル作成SQL

```sql
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
CREATE TABLE [dbo].[Clients] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ClientId] NVARCHAR(50),
    [IsLessor] BIT NOT NULL DEFAULT 0,
    [IsLessee] BIT NOT NULL DEFAULT 0,
    [IsBillingTo] BIT NOT NULL DEFAULT 0,
    [PostalCode] NVARCHAR(10),
    [Address] NVARCHAR(200),
    [Phone] NVARCHAR(20),
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| ClientId | NVARCHAR(50) | YES | 取引先ID |
| IsLessor | BIT | NO | 貸主フラグ（デフォルト: 0） |
| IsLessee | BIT | NO | 借主フラグ（デフォルト: 0） |
| IsBillingTo | BIT | NO | 請求先フラグ（デフォルト: 0） |
| PostalCode | NVARCHAR(10) | YES | 郵便番号 |
| Address | NVARCHAR(200) | YES | 住所 |
| Phone | NVARCHAR(20) | YES | 電話番号 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |
| UpdatedAt | DATETIME | NO | 更新日時（デフォルト: GETDATE()） |

### マイグレーションSQL

既存のテーブルに対して古いカラムを削除し、新しいカラムを追加するSQLです。

```sql
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
    
    -- 新しいカラムを追加（存在しない場合のみ）
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'ClientId')
    BEGIN
        ALTER TABLE [dbo].[Clients] ADD [ClientId] NVARCHAR(50);
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
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'PostalCode')
    BEGIN
        ALTER TABLE [dbo].[Clients] ADD [PostalCode] NVARCHAR(10);
    END
END
```

---

## InvoiceDetails テーブル

請求明細情報を管理するテーブルです。

### テーブル作成SQL

```sql
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND type in (N'U'))
CREATE TABLE [dbo].[InvoiceDetails] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [BillingTo] NVARCHAR(100),
    [Lessor] NVARCHAR(100),
    [BuildingName] NVARCHAR(100),
    [Lessee] NVARCHAR(100),
    [RoomNumber] NVARCHAR(50),
    [Category] NVARCHAR(50),
    [Content] NVARCHAR(200),
    [UsageAmount] DECIMAL(18,2),
    [Unit] NVARCHAR(20),
    [TaxInclusiveAmount] DECIMAL(18,2),
    [TaxRate] DECIMAL(5,2),
    [ChildMeterStartDate] DATETIME,
    [ChildMeterEndDate] DATETIME,
    [ParentMeterStartDate] DATETIME,
    [ParentMeterEndDate] DATETIME,
    [ConfirmedBillingDate] DATETIME,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| BillingTo | NVARCHAR(100) | YES | 請求先 |
| Lessor | NVARCHAR(100) | YES | 貸主 |
| BuildingName | NVARCHAR(100) | YES | 建物名称 |
| Lessee | NVARCHAR(100) | YES | 借主 |
| RoomNumber | NVARCHAR(50) | YES | 部屋番号 |
| Category | NVARCHAR(50) | YES | 種別 |
| Content | NVARCHAR(200) | YES | 内容 |
| UsageAmount | DECIMAL(18,2) | YES | 使用量 |
| Unit | NVARCHAR(20) | YES | 単位 |
| TaxInclusiveAmount | DECIMAL(18,2) | YES | 税込金額 |
| TaxRate | DECIMAL(5,2) | YES | 税率 |
| ChildMeterStartDate | DATETIME | YES | 子メータ使用開始日 |
| ChildMeterEndDate | DATETIME | YES | 子メータ使用終了日 |
| ParentMeterStartDate | DATETIME | YES | 親メータ使用開始日 |
| ParentMeterEndDate | DATETIME | YES | 親メータ使用終了日 |
| ConfirmedBillingDate | DATETIME | YES | 決定請求日 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |
| UpdatedAt | DATETIME | NO | 更新日時（デフォルト: GETDATE()） |

---

## WaterBillings テーブル

水道料金の請求データを管理するテーブルです。

### テーブル作成SQL

```sql
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
CREATE TABLE [dbo].[WaterBillings] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [BillingYearMonth] NVARCHAR(7) NOT NULL,
    [BuildingName] NVARCHAR(100) NOT NULL,
    [UsageAmount] DECIMAL(18,2) NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NOT NULL,
    [BillingAmount] DECIMAL(18,2) NOT NULL,
    [CustomerNumber] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| BillingYearMonth | NVARCHAR(7) | NO | 請求年月（例: "2024-01"） |
| BuildingName | NVARCHAR(100) | NO | ビル名 |
| UsageAmount | DECIMAL(18,2) | NO | 使用量 |
| StartDate | DATETIME | NO | 開始日 |
| EndDate | DATETIME | NO | 終了日 |
| BillingAmount | DECIMAL(18,2) | NO | 請求金額 |
| CustomerNumber | NVARCHAR(50) | NO | お客様番号 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |
| UpdatedAt | DATETIME | NO | 更新日時（デフォルト: GETDATE()） |

---

## ElectricBillings テーブル

電気料金の請求データを管理するテーブルです。

### テーブル作成SQL

```sql
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
    [BillingAmount] DECIMAL(18,2) NOT NULL,
    [CustomerNumber] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| BillingYearMonth | NVARCHAR(7) | NO | 請求年月（例: "2024-01"） |
| BuildingName | NVARCHAR(100) | NO | ビル名 |
| UsageAmount | DECIMAL(18,2) | NO | 使用量 |
| StartDate | DATETIME | NO | 開始日 |
| EndDate | DATETIME | NO | 終了日 |
| BasicCharge | DECIMAL(18,2) | NO | 基本料金 |
| PowerCharge | DECIMAL(18,2) | NO | 電力量料金 |
| BillingAmount | DECIMAL(18,2) | NO | 請求金額 |
| CustomerNumber | NVARCHAR(50) | NO | お客様番号 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |
| UpdatedAt | DATETIME | NO | 更新日時（デフォルト: GETDATE()） |

---

## GasBillings テーブル

ガス料金の請求データを管理するテーブルです。

### テーブル作成SQL

```sql
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') AND type in (N'U'))
CREATE TABLE [dbo].[GasBillings] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [BillingYearMonth] NVARCHAR(7) NOT NULL,
    [BuildingName] NVARCHAR(100) NOT NULL,
    [District] NVARCHAR(50) NOT NULL,
    [UsageAmount] DECIMAL(18,2) NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NOT NULL,
    [BasicCharge] DECIMAL(18,2) NOT NULL,
    [UsageCharge] DECIMAL(18,2) NOT NULL,
    [BillingAmount] DECIMAL(18,2) NOT NULL,
    [CustomerNumber] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| BillingYearMonth | NVARCHAR(7) | NO | 請求年月（例: "2024-01"） |
| BuildingName | NVARCHAR(100) | NO | ビル名 |
| District | NVARCHAR(50) | NO | 区画 |
| UsageAmount | DECIMAL(18,2) | NO | 使用量 |
| StartDate | DATETIME | NO | 開始日 |
| EndDate | DATETIME | NO | 終了日 |
| BasicCharge | DECIMAL(18,2) | NO | 基本料金 |
| UsageCharge | DECIMAL(18,2) | NO | 使用料金 |
| BillingAmount | DECIMAL(18,2) | NO | 請求金額 |
| CustomerNumber | NVARCHAR(50) | NO | お客様番号 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |
| UpdatedAt | DATETIME | NO | 更新日時（デフォルト: GETDATE()） |

---

## Contracts テーブル

契約情報を管理するテーブルです。

### テーブル作成SQL

```sql
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
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| ContractNumber | NVARCHAR(50) | YES | 契約番号 |
| ContractType | NVARCHAR(50) | YES | 契約種別 |
| ContractorName | NVARCHAR(100) | YES | 契約者名 |
| LessorClientId | INT | YES | 貸主取引先ID（外部キー: Clients.Id） |
| LesseeClientId | INT | YES | 借主取引先ID（外部キー: Clients.Id） |
| BillingClientId | INT | YES | 請求取引先ID（外部キー: Clients.Id） |
| StartDate | DATETIME | YES | 対象開始日 |
| EndDate | DATETIME | YES | 対象終了日 |
| ContractStatus | NVARCHAR(50) | YES | 契約状況 |
| ClosingDate | INT | YES | 締日 |
| BuildingId | INT | YES | ビルID（外部キー: Buildings.Id） |
| CustomerNumber | NVARCHAR(50) | YES | お客様番号 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |
| UpdatedAt | DATETIME | NO | 更新日時（デフォルト: GETDATE()） |

### 外部キー制約

- `LessorClientId` → `Clients.Id`
- `LesseeClientId` → `Clients.Id`
- `BillingClientId` → `Clients.Id`
- `BuildingId` → `Buildings.Id`

### 契約状況の選択肢

- 意向確認中
- 契約書送付待ち
- 契約書送付済み
- 契約書返信済み
- 契約終了
- 途中解約

---

## Meters テーブル

メーター情報を管理するテーブルです。

### テーブル作成SQL

```sql
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meters]') AND type in (N'U'))
CREATE TABLE [dbo].[Meters] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [BuildingId] INT,
    [MeterId] NVARCHAR(50) NOT NULL,
    [MeterType] NVARCHAR(50) NOT NULL,
    [ManagementNumber] NVARCHAR(50),
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([BuildingId]) REFERENCES [dbo].[Buildings]([Id])
)
```

### カラム説明

| カラム名 | データ型 | NULL許可 | 説明 |
|---------|---------|---------|------|
| Id | INT | NO | 主キー（自動採番） |
| BuildingId | INT | YES | ビルID（外部キー: Buildings.Id） |
| MeterId | NVARCHAR(50) | NO | メーターID |
| MeterType | NVARCHAR(50) | NO | メーター種別 |
| ManagementNumber | NVARCHAR(50) | YES | 管理番号 |
| CreatedAt | DATETIME | NO | 作成日時（デフォルト: GETDATE()） |
| UpdatedAt | DATETIME | NO | 更新日時（デフォルト: GETDATE()） |

### 外部キー制約

- `BuildingId` → `Buildings.Id`

### メーター種別の選択肢

- 定電圧(動力)
- 低電圧(電灯)
- 高圧電力
- ガス
- 水道

---

## テーブル作成順序

データベースを初期化する際の推奨順序：

1. Buildings
2. Clients
3. UtilityCosts
4. InvoiceDetails
5. WaterBillings
6. ElectricBillings
7. GasBillings
8. Contracts
9. Meters

**注意**: `Contracts`テーブルと`Meters`テーブルは`Buildings`と`Clients`テーブルに依存しているため、これらのテーブルが作成された後に作成する必要があります。

---

## 更新履歴

- 2024年: 初版作成
  - Buildings, UtilityCosts, Clients, InvoiceDetails, WaterBillings, ElectricBillings, GasBillings, Contracts, Meters テーブルを追加

















