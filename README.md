# ビル管理システム

C# Windows Forms と SQL Server を使用したビル管理システムです。

## 機能

- ビル情報の登録・編集・削除
- ビル一覧の表示
- 階数、面積、所有者、連絡先などの情報管理

## 必要要件

- .NET Framework 4.8
- Visual Studio 2019 またはそれ以降
- SQL Server Express（またはSQL Server以上）

## セットアップ

1. Visual Studio でプロジェクトを開く
2. NuGet パッケージを復元（ツール > NuGetパッケージマネージャー > パッケージマネージャーコンソールで `dotnet restore` を実行）
3. `app.config` の接続文字列を確認・変更
4. アプリケーションを実行

## データベース接続設定

`app.config` の `connectionStrings` セクションで接続文字列を設定できます。

デフォルト設定：
```
Data Source=ROLAN-PC\SQLEXPRESS;Initial Catalog=BuildingManagement;Integrated Security=True;
```

## ビルドと実行

```
dotnet build
dotnet run
```

または Visual Studio から F5 キーで実行

