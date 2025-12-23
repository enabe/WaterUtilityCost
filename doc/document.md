# 水道光熱費管理システム システム仕様書

## 1. システム概要

### 1.1 システム名
水道光熱費管理システム（WaterUtilityCost Management System）

### 1.2 目的
ビル管理において、水道・電気・ガスの料金請求データ、取引先情報、契約情報、メーター情報を一元管理するためのWindows Formsアプリケーションです。

### 1.3 技術スタック
- **開発言語**: C# (.NET Framework 4.8)
- **UIフレームワーク**: Windows Forms
- **データベース**: SQL Server Express
- **開発環境**: Visual Studio 2019以降
- **C#言語バージョン**: C# 8.0以降（Nullable参照型対応）

### 1.4 アーキテクチャ
- **3層アーキテクチャ**
  - **プレゼンテーション層**: Windows Forms（`Forms/`フォルダ）
  - **ビジネスロジック層**: モデルクラス（`Models/`フォルダ）
  - **データアクセス層**: データアクセスクラス（`DataAccess/`フォルダ）
  - **データベース層**: SQL Server Express

---

## 2. データベース設計

### 2.1 データベース情報
- **データベース名**: BuildingManagement
- **接続文字列**: `Data Source=ROLAN-PC\SQLEXPRESS;Initial Catalog=BuildingManagement;Integrated Security=True;`
- **詳細**: `Database/DATABASE_SCHEMA.md` を参照

### 2.2 テーブル一覧

| テーブル名 | 説明 |
|-----------|------|
| Buildings | ビル情報 |
| UtilityCosts | 光熱費記録（旧機能） |
| Clients | 取引先情報 |
| InvoiceDetails | 請求明細 |
| WaterBillings | 水道料金請求データ |
| ElectricBillings | 電気料金請求データ |
| GasBillings | ガス料金請求データ |
| Contracts | 契約情報 |
| Meters | メーター情報 |

### 2.3 主要なリレーションシップ

```
Buildings (1) ──< (N) UtilityCosts
Buildings (1) ──< (N) Contracts
Buildings (1) ──< (N) Meters
Clients (1) ──< (N) Contracts (LessorClientId)
Clients (1) ──< (N) Contracts (LesseeClientId)
Clients (1) ──< (N) Contracts (BillingClientId)
```

---

## 3. 機能一覧

### 3.1 ビル管理
**フォーム**: `BuildingManagementForm`, `BuildingForm`

**機能**:
- ビル情報の一覧表示
- ビル情報の新規登録
- ビル情報の編集
- ビル情報の削除
- ビル情報の更新（リフレッシュ）

**管理項目**:
- ビル名（必須）
- 住所
- 階数（必須）
- 建築日
- 面積
- 所有者
- 連絡先

### 3.2 取引先管理
**フォーム**: `ClientManagementForm`, `ClientForm`

**機能**:
- 取引先情報の一覧表示
- 取引先情報の新規登録
- 取引先情報の編集
- 取引先情報の削除
- 取引先情報の更新（リフレッシュ）

**管理項目**:
- 取引先ID
- 取引先対象（チェックボックス）
  - 貸主（IsLessor）
  - 借主（IsLessee）
  - 請求先（IsBillingTo）
- 郵便番号
- 住所
- 電話番号

### 3.3 水道光熱費請求明細一覧
**フォーム**: `InvoiceDetailForm`, `InvoiceDetailEditForm`

**機能**:
- 請求明細の一覧表示
- 請求明細の新規登録
- 請求明細の編集
- 請求明細の削除
- 請求明細の更新（リフレッシュ）
- CSVエクスポート機能

**管理項目**:
- 請求先（BillingTo）
- 貸主（Lessor）
- 建物名称（BuildingName）
- 借主（Lessee）
- 部屋番号（RoomNumber）
- 種別（Category）
- 内容（Content）
- 使用量（UsageAmount）
- 単位（Unit）
- 税込金額（TaxInclusiveAmount）
- 税率（TaxRate）
- 子メータ使用開始日（ChildMeterStartDate）
- 子メータ使用終了日（ChildMeterEndDate）
- 親メータ使用開始日（ParentMeterStartDate）
- 親メータ使用終了日（ParentMeterEndDate）
- 決定請求日（ConfirmedBillingDate）

**CSVエクスポート**:
- UTF-8 BOM付きCSV形式
- 全項目を出力
- 日付は `yyyy-MM-dd` 形式
- 数値は小数点以下2桁まで表示

### 3.4 水道料金管理
**フォーム**: `WaterBillingManagementForm`, `WaterBillingForm`

**機能**:
- 水道料金請求データの一覧表示
- 水道料金請求データの新規登録
- 水道料金請求データの編集
- 水道料金請求データの削除
- 水道料金請求データの更新（リフレッシュ）

**管理項目**:
- 請求年月（BillingYearMonth: 必須、形式: "YYYY-MM"）
- ビル名（BuildingName: 必須、登録済みビル名から選択）
- 使用量（UsageAmount: 必須）
- 開始日（StartDate: 必須）
- 終了日（EndDate: 必須）
- 請求金額（BillingAmount: 必須）
- お客様番号（CustomerNumber: 必須）
- インボイス番号（InvoiceNumber: 必須）

### 3.5 電気料金管理
**フォーム**: `ElectricBillingManagementForm`, `ElectricBillingForm`

**機能**:
- 電気料金請求データの一覧表示
- 電気料金請求データの新規登録
- 電気料金請求データの編集
- 電気料金請求データの削除
- 電気料金請求データの更新（リフレッシュ）

**管理項目**:
- 請求年月（BillingYearMonth: 必須、形式: "YYYY-MM"）
- ビル名（BuildingName: 必須、登録済みビル名から選択）
- 使用量（UsageAmount: 必須）
- 開始日（StartDate: 必須）
- 終了日（EndDate: 必須）
- 基本料金（BasicCharge: 必須）
- 電力量料金（PowerCharge: 必須）
- 再エネ発電賦課金（RenewableEnergyCharge: 必須）
- 請求金額（BillingAmount: 必須）
- お客様番号（CustomerNumber: 必須）
- インボイス番号（InvoiceNumber: 必須）

### 3.6 ガス料金管理
**フォーム**: `GasBillingManagementForm`, `GasBillingForm`

**機能**:
- ガス料金請求データの一覧表示
- ガス料金請求データの新規登録
- ガス料金請求データの編集
- ガス料金請求データの削除
- ガス料金請求データの更新（リフレッシュ）

**管理項目**:
- 請求年月（BillingYearMonth: 必須、形式: "YYYY-MM"）
- ビル名（BuildingName: 必須、登録済みビル名から選択）
- 区画（District: 必須）
- 使用量（UsageAmount: 必須）
- 開始日（StartDate: 必須）
- 終了日（EndDate: 必須）
- 基本料金（BasicCharge: 必須）
- 使用料金（UsageCharge: 必須）
- 割引料金（DiscountCharge: 必須）
- 請求金額（BillingAmount: 必須）
- お客様番号（CustomerNumber: 必須）
- インボイス番号（InvoiceNumber: 必須）

### 3.7 契約管理
**フォーム**: `ContractManagementForm`, `ContractForm`

**機能**:
- 契約情報の一覧表示
- 契約情報の新規登録
- 契約情報の編集
- 契約情報の削除
- 契約情報の更新（リフレッシュ）

**管理項目**:
- 契約番号（ContractNumber）
- 契約種別（ContractType）
- 契約者名（ContractorName）
- 貸主取引先（LessorClientId: 取引先管理でIsLessor=trueの取引先から選択）
- 借主取引先（LesseeClientId: 取引先管理でIsLessee=trueの取引先から選択）
- 請求取引先（BillingClientId: 取引先管理でIsBillingTo=trueの取引先から選択）
- 対象開始日（StartDate）
- 対象終了日（EndDate）
- 契約状況（ContractStatus: ドロップダウンから選択）
  - 意向確認中
  - 契約書送付待ち
  - 契約書送付済み
  - 契約書返信済み
  - 契約終了
  - 途中解約
- 締日（ClosingDate: 1-31の数値）
- ビル（BuildingId: 登録済みビル名から選択）
- お客様番号（CustomerNumber）

### 3.8 親メーター管理
**フォーム**: `MeterManagementForm`, `MeterForm`

**機能**:
- 親メーター情報の一覧表示
- 親メーター情報の新規登録
- 親メーター情報の編集
- 親メーター情報の削除
- 親メーター情報の更新（リフレッシュ）

**管理項目**:
- ビル（BuildingId: 登録済みビル名から選択）
- メーターID（MeterId: 必須）
- メーター種別（MeterType: 必須、ドロップダウンから選択）
  - 定電圧(動力)
  - 低電圧(電灯)
  - 高圧電力
  - ガス
  - 水道
- 管理番号（ManagementNumber）

---

## 4. メニュー構成

### 4.1 メインメニュー（MenuForm）
**レイアウト**:
- タイトル: "水道光熱費管理システム"（中央配置）
- ボタン配置:
  - **1列目（左）**:
    - ビル管理
    - 取引先管理
    - 契約管理
    - 親メーター管理
  - **2列目（中央）**:
    - 電気料金管理
    - 水道料金管理
    - ガス料金管理
  - **3列目（右）**:
    - 水道光熱費請求明細一覧
  - **右下**:
    - 終了ボタン

### 4.2 各機能へのアクセス
すべての機能はメインメニューからアクセス可能です。各ボタンをクリックすると、対応する管理フォームが開きます。

---

## 5. データモデル

### 5.1 モデルクラス一覧

| モデルクラス | ファイル | 説明 |
|------------|---------|------|
| Building | `Models/Building.cs` | ビル情報 |
| UtilityCost | `Models/UtilityCost.cs` | 光熱費記録（旧機能） |
| Client | `Models/Client.cs` | 取引先情報 |
| InvoiceDetail | `Models/InvoiceDetail.cs` | 請求明細 |
| WaterBilling | `Models/WaterBilling.cs` | 水道料金請求データ |
| ElectricBilling | `Models/ElectricBilling.cs` | 電気料金請求データ |
| GasBilling | `Models/GasBilling.cs` | ガス料金請求データ |
| Contract | `Models/Contract.cs` | 契約情報 |
| Meter | `Models/Meter.cs` | メーター情報 |

### 5.2 共通プロパティ
各モデルクラスには以下の共通プロパティが含まれます:
- `CreatedAt`: 作成日時
- `UpdatedAt`: 更新日時（更新可能なモデルの場合）

---

## 6. データアクセス層

### 6.1 データアクセスクラス一覧

| クラス名 | ファイル | 対象モデル |
|---------|---------|-----------|
| BuildingDataAccess | `DataAccess/BuildingDataAccess.cs` | Building |
| ClientDataAccess | `DataAccess/ClientDataAccess.cs` | Client |
| InvoiceDetailDataAccess | `DataAccess/InvoiceDetailDataAccess.cs` | InvoiceDetail |
| WaterBillingDataAccess | `DataAccess/WaterBillingDataAccess.cs` | WaterBilling |
| ElectricBillingDataAccess | `DataAccess/ElectricBillingDataAccess.cs` | ElectricBilling |
| GasBillingDataAccess | `DataAccess/GasBillingDataAccess.cs` | GasBilling |
| ContractDataAccess | `DataAccess/ContractDataAccess.cs` | Contract |
| MeterDataAccess | `DataAccess/MeterDataAccess.cs` | Meter |

### 6.2 共通メソッド
各データアクセスクラスには以下のメソッドが実装されています:
- `GetAllAsync()`: 全件取得
- `GetByIdAsync(int id)`: ID指定取得
- `CreateAsync(Model model)`: 新規作成
- `UpdateAsync(Model model)`: 更新
- `DeleteAsync(int id)`: 削除

### 6.3 データベース接続管理
- **クラス**: `Database/DatabaseHelper.cs`
- **機能**:
  - データベース接続文字列の管理
  - データベース接続テスト
  - データベース初期化（テーブル作成）
  - マイグレーション処理

---

## 7. UI設計

### 7.1 Windows Forms Designer対応
すべてのフォームはWindows Forms Designerで編集可能です。

**フォーム構成**:
- **メインフォーム**: `.Designer.cs` ファイルにデザイナーコードを記述
- **追加初期化**: `.cs` ファイルの `InitializeComponentAdditional()` メソッドでイベントハンドラーを設定

### 7.2 共通UIパターン

#### 管理フォーム（一覧表示）
- **DataGridView**: データ一覧表示
- **ボタン**:
  - 新規登録
  - 編集
  - 削除
  - 更新（リフレッシュ）
- **StatusStrip**: ステータス表示（一部フォーム）

#### 登録・編集フォーム
- **入力コントロール**:
  - TextBox: テキスト入力
  - ComboBox: 選択入力（ビル名、取引先、種別など）
  - DateTimePicker: 日付入力
  - CheckBox: 真偽値入力（取引先対象など）
- **ボタン**:
  - 保存
  - キャンセル

### 7.3 バリデーション
- 必須項目のチェック
- 数値形式のチェック
- 日付形式のチェック
- エラーメッセージ表示（MessageBox）

---

## 8. エラーハンドリング

### 8.1 データベースエラー
- 接続エラー: 起動時にエラーメッセージを表示
- クエリエラー: 各操作でエラーメッセージを表示
- 外部キー制約エラー: 削除時にエラーメッセージを表示

### 8.2 入力エラー
- 必須項目未入力: 警告メッセージを表示
- 形式エラー: 警告メッセージを表示

### 8.3 ファイルエラー（CSVエクスポート）
- ファイル保存エラー: エラーメッセージを表示
- 書き込みエラー: エラーメッセージを表示

---

## 9. セットアップと実行

### 9.1 必要要件
- .NET Framework 4.8
- Visual Studio 2019以降
- SQL Server Express（またはSQL Server以上）

### 9.2 セットアップ手順
1. Visual Studio でプロジェクトを開く
2. NuGetパッケージを復元（`dotnet restore`）
3. `app.config` の接続文字列を確認・変更
4. アプリケーションを実行（F5）

### 9.3 データベース初期化
アプリケーション起動時に自動的にデータベースとテーブルが作成されます。既存のテーブルがある場合は、既存のテーブルはそのまま使用されます。

### 9.4 接続文字列設定
`app.config` の `connectionStrings` セクションで接続文字列を設定できます。

**デフォルト設定**:
```xml
<connectionStrings>
  <add name="DefaultConnection" 
       connectionString="Data Source=ROLAN-PC\SQLEXPRESS;Initial Catalog=BuildingManagement;Integrated Security=True;" />
</connectionStrings>
```

---

## 10. ファイル構成

### 10.1 プロジェクト構造
```
WaterUtilityCost/
├── DataAccess/          # データアクセス層
│   ├── BuildingDataAccess.cs
│   ├── ClientDataAccess.cs
│   ├── ContractDataAccess.cs
│   ├── ElectricBillingDataAccess.cs
│   ├── GasBillingDataAccess.cs
│   ├── InvoiceDetailDataAccess.cs
│   ├── MeterDataAccess.cs
│   └── WaterBillingDataAccess.cs
├── Database/            # データベース管理
│   ├── DatabaseHelper.cs
│   └── DATABASE_SCHEMA.md
├── doc/                 # ドキュメント
│   └── document.md      # システム仕様書（本ファイル）
├── Forms/               # UIフォーム
│   ├── BuildingForm.cs / .Designer.cs
│   ├── BuildingManagementForm.cs / .Designer.cs
│   ├── ClientForm.cs / .Designer.cs
│   ├── ClientManagementForm.cs / .Designer.cs
│   ├── ContractForm.cs / .Designer.cs
│   ├── ContractManagementForm.cs / .Designer.cs
│   ├── ElectricBillingForm.cs / .Designer.cs
│   ├── ElectricBillingManagementForm.cs / .Designer.cs
│   ├── GasBillingForm.cs / .Designer.cs
│   ├── GasBillingManagementForm.cs / .Designer.cs
│   ├── InvoiceDetailEditForm.cs / .Designer.cs
│   ├── InvoiceDetailForm.cs / .Designer.cs
│   ├── MainForm.cs / .Designer.cs
│   ├── MenuForm.cs / .Designer.cs
│   ├── MeterForm.cs / .Designer.cs
│   ├── MeterManagementForm.cs / .Designer.cs
│   ├── WaterBillingForm.cs / .Designer.cs
│   └── WaterBillingManagementForm.cs / .Designer.cs
├── Models/              # データモデル
│   ├── Building.cs
│   ├── Client.cs
│   ├── Contract.cs
│   ├── ElectricBilling.cs
│   ├── GasBilling.cs
│   ├── InvoiceDetail.cs
│   ├── Meter.cs
│   ├── UtilityCost.cs
│   └── WaterBilling.cs
├── Program.cs           # エントリーポイント
├── app.config           # アプリケーション設定
├── WaterUtilityCost.csproj
└── README.md
```

---

## 11. 更新履歴

### 2024年
- **初期バージョン**: ビル管理機能の実装
- **取引先管理機能**: 追加
- **水道光熱費請求明細一覧機能**: 追加（CSVエクスポート機能含む）
- **水道料金管理機能**: 追加
- **電気料金管理機能**: 追加
- **ガス料金管理機能**: 追加
- **契約管理機能**: 追加
- **親メーター管理機能**: 追加
- **Windows Forms Designer対応**: 全フォームで対応完了
- **データベーススキーマドキュメント**: 作成
- **システム仕様書**: 作成

---

## 12. 今後の拡張予定

現在、特定の拡張予定はありません。必要に応じて機能追加を検討します。

---

## 13. 参考資料

- **データベーススキーマ**: `Database/DATABASE_SCHEMA.md`
- **README**: `README.md`



