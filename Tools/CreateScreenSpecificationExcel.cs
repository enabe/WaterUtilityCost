using System;
using System.IO;
using ClosedXML.Excel;

namespace WaterUtilityCost.Tools
{
    /// <summary>
    /// 画面仕様書Excelファイルを作成するツール
    /// </summary>
    public class CreateScreenSpecificationExcel
    {
        public static void CreateExcel(string outputPath)
        {
            using var workbook = new XLWorkbook();
            
            // 目次シートを作成
            var indexSheet = workbook.Worksheets.Add("目次");
            CreateIndexSheet(indexSheet);
            
            // 各画面の仕様シートを作成
            CreateMenuFormSheet(workbook);
            CreateBuildingManagementSheet(workbook);
            CreateFloorManagementSheet(workbook);
            CreateClientManagementSheet(workbook);
            CreateContractManagementSheet(workbook);
            CreateMeterManagementSheet(workbook);
            CreateWaterBillingManagementSheet(workbook);
            CreateElectricBillingManagementSheet(workbook);
            CreateGasBillingManagementSheet(workbook);
            CreateInvoiceDetailSheet(workbook);
            CreateBillingDataRegistrationSheet(workbook);
            
            workbook.SaveAs(outputPath);
        }
        
        private static void CreateIndexSheet(IXLWorksheet sheet)
        {
            sheet.Cell(1, 1).Value = "画面仕様書 - 目次";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "No.";
            sheet.Cell(3, 2).Value = "画面名";
            sheet.Cell(3, 3).Value = "シート名";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            sheet.Cell(3, 3).Style.Font.Bold = true;
            
            var screens = new[]
            {
                new { No = 1, Name = "メインメニュー", SheetName = "メインメニュー" },
                new { No = 2, Name = "ビル管理", SheetName = "ビル管理" },
                new { No = 3, Name = "部屋管理", SheetName = "部屋管理" },
                new { No = 4, Name = "取引先管理", SheetName = "取引先管理" },
                new { No = 5, Name = "契約管理", SheetName = "契約管理" },
                new { No = 6, Name = "親メーター管理", SheetName = "親メーター管理" },
                new { No = 7, Name = "水道料金管理", SheetName = "水道料金管理" },
                new { No = 8, Name = "電気料金管理", SheetName = "電気料金管理" },
                new { No = 9, Name = "ガス料金管理", SheetName = "ガス料金管理" },
                new { No = 10, Name = "水道光熱費請求明細一覧", SheetName = "水道光熱費請求明細一覧" },
                new { No = 11, Name = "請求データ登録", SheetName = "請求データ登録" }
            };
            
            int row = 4;
            foreach (var screen in screens)
            {
                sheet.Cell(row, 1).Value = screen.No;
                sheet.Cell(row, 2).Value = screen.Name;
                sheet.Cell(row, 3).Value = screen.SheetName;
                row++;
            }
            
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateMenuFormSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("メインメニュー");
            
            sheet.Cell(1, 1).Value = "メインメニュー";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "メインメニュー" },
                new { Item = "フォームクラス", Content = "MenuForm" },
                new { Item = "説明", Content = "アプリケーションのメイン画面。各機能へのアクセスを提供します。" },
                new { Item = "レイアウト", Content = "タイトル: 「水道光熱費管理システム」（中央配置、フォントサイズ16）" },
                new { Item = "ボタン配置", Content = "1列目（左）: ビル管理、部屋管理、取引先管理、契約管理、親メーター管理" },
                new { Item = "", Content = "2列目（中央）: 電気料金管理、水道料金管理、ガス料金管理" },
                new { Item = "", Content = "3列目（右）: 水道光熱費請求明細一覧、請求データ登録" },
                new { Item = "", Content = "右下: 終了ボタン" },
                new { Item = "機能", Content = "各ボタンをクリックすると、対応する管理フォームが開きます。" },
                new { Item = "画像", Content = "※ここにスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            // 画像用のセルを大きくする
            sheet.Row(13).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateBuildingManagementSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("ビル管理");
            
            sheet.Cell(1, 1).Value = "ビル管理";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "ビル管理" },
                new { Item = "フォームクラス", Content = "BuildingManagementForm, BuildingForm" },
                new { Item = "説明", Content = "ビル情報の一覧表示、新規登録、編集、削除を行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、ビル名（幅150px）、住所（幅150px）、階数（幅50px）、建築日、面積、所有者、連絡先、登録日" },
                new { Item = "管理項目", Content = "ビル名（必須）、住所、階数（必須、文字列形式：B1、B2など可）、建築日、面積、所有者、連絡先" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）" },
                new { Item = "機能", Content = "・新規登録: BuildingFormを開いて新規ビルを登録" },
                new { Item = "", Content = "・編集: 選択したビルの情報を編集" },
                new { Item = "", Content = "・削除: 選択したビルを削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(14).Height = 300;
            sheet.Row(15).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateFloorManagementSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("部屋管理");
            
            sheet.Cell(1, 1).Value = "部屋管理";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "部屋管理" },
                new { Item = "フォームクラス", Content = "FloorManagementForm, FloorForm" },
                new { Item = "説明", Content = "部屋情報の一覧表示、新規登録、編集、削除を行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、ビル名、部屋名、部屋面積、登録日" },
                new { Item = "管理項目", Content = "ビル管理の選択（ビル名と階数を表示）、部屋名（必須）、部屋面積" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）" },
                new { Item = "機能", Content = "・新規登録: FloorFormを開いて新規部屋を登録" },
                new { Item = "", Content = "・編集: 選択した部屋の情報を編集" },
                new { Item = "", Content = "・削除: 選択した部屋を削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(14).Height = 300;
            sheet.Row(15).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateClientManagementSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("取引先管理");
            
            sheet.Cell(1, 1).Value = "取引先管理";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "取引先管理" },
                new { Item = "フォームクラス", Content = "ClientManagementForm, ClientForm" },
                new { Item = "説明", Content = "取引先情報の一覧表示、新規登録、編集、削除を行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、取引先名（幅150px）、ビル名（幅150px）、フロア名（幅100px）、取引先対象、郵便番号、住所、電話番号、登録日" },
                new { Item = "管理項目", Content = "取引先名（必須）、ビル名の選択（ビル名と階数を表示）、フロア名の選択（選択したビルに紐づいたフロア名）、取引先対象（貸主、借主、請求先のチェックボックス）、郵便番号、住所、電話番号" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）" },
                new { Item = "機能", Content = "・新規登録: ClientFormを開いて新規取引先を登録" },
                new { Item = "", Content = "・編集: 選択した取引先の情報を編集" },
                new { Item = "", Content = "・削除: 選択した取引先を削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(14).Height = 300;
            sheet.Row(15).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateContractManagementSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("契約管理");
            
            sheet.Cell(1, 1).Value = "契約管理";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "契約管理" },
                new { Item = "フォームクラス", Content = "ContractManagementForm, ContractForm" },
                new { Item = "説明", Content = "契約情報の一覧表示、新規登録、編集、削除を行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、貸主、借主、請求先、ビル名、部屋番号、契約開始日、契約終了日、登録日" },
                new { Item = "管理項目", Content = "貸主（取引先から選択）、借主（取引先から選択）、請求先（取引先から選択）、ビル（ビル管理から選択）、部屋番号、契約開始日、契約終了日" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）" },
                new { Item = "機能", Content = "・新規登録: ContractFormを開いて新規契約を登録" },
                new { Item = "", Content = "・編集: 選択した契約の情報を編集" },
                new { Item = "", Content = "・削除: 選択した契約を削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(14).Height = 300;
            sheet.Row(15).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateMeterManagementSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("親メーター管理");
            
            sheet.Cell(1, 1).Value = "親メーター管理";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "親メーター管理" },
                new { Item = "フォームクラス", Content = "MeterManagementForm, MeterForm" },
                new { Item = "説明", Content = "メーター情報の一覧表示、新規登録、編集、削除を行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、ビル名、メーターID、メーター種別、管理番号、登録日" },
                new { Item = "管理項目", Content = "ビル（ビル管理から選択）、メーターID（必須）、メーター種別（必須、ドロップダウン：定電圧(動力)、低電圧(電灯)、高圧電力、ガス、水道）、管理番号" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）" },
                new { Item = "機能", Content = "・新規登録: MeterFormを開いて新規メーターを登録" },
                new { Item = "", Content = "・編集: 選択したメーターの情報を編集" },
                new { Item = "", Content = "・削除: 選択したメーターを削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(14).Height = 300;
            sheet.Row(15).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateWaterBillingManagementSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("水道料金管理");
            
            sheet.Cell(1, 1).Value = "水道料金管理";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "水道料金管理" },
                new { Item = "フォームクラス", Content = "WaterBillingManagementForm, WaterBillingForm" },
                new { Item = "説明", Content = "水道料金請求データの一覧表示、新規登録、編集、削除を行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、請求年月、ビル名、使用量、開始日、終了日、請求金額、お客様番号、インボイス番号、登録日" },
                new { Item = "管理項目", Content = "請求年月（必須、形式: YYYY-MM）、ビル名（必須、登録済みビル名から選択）、使用量（必須）、開始日（必須）、終了日（必須）、請求金額（必須）、お客様番号（必須）、インボイス番号（必須）" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）" },
                new { Item = "機能", Content = "・新規登録: WaterBillingFormを開いて新規水道料金請求データを登録" },
                new { Item = "", Content = "・編集: 選択した水道料金請求データの情報を編集" },
                new { Item = "", Content = "・削除: 選択した水道料金請求データを削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(14).Height = 300;
            sheet.Row(15).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateElectricBillingManagementSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("電気料金管理");
            
            sheet.Cell(1, 1).Value = "電気料金管理";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "電気料金管理" },
                new { Item = "フォームクラス", Content = "ElectricBillingManagementForm, ElectricBillingForm" },
                new { Item = "説明", Content = "電気料金請求データの一覧表示、新規登録、編集、削除を行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、請求年月、ビル名、使用量、開始日、終了日、基本料金、使用料金、再エネ発電賦課金、請求金額、お客様番号、インボイス番号、登録日" },
                new { Item = "管理項目", Content = "請求年月（必須、形式: YYYY-MM）、ビル名（必須、登録済みビル名から選択）、使用量（必須）、開始日（必須）、終了日（必須）、基本料金（必須）、使用料金（必須）、再エネ発電賦課金（必須）、請求金額（必須）、お客様番号（必須）、インボイス番号（必須）" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）" },
                new { Item = "機能", Content = "・新規登録: ElectricBillingFormを開いて新規電気料金請求データを登録" },
                new { Item = "", Content = "・編集: 選択した電気料金請求データの情報を編集" },
                new { Item = "", Content = "・削除: 選択した電気料金請求データを削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(14).Height = 300;
            sheet.Row(15).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateGasBillingManagementSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("ガス料金管理");
            
            sheet.Cell(1, 1).Value = "ガス料金管理";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "ガス料金管理" },
                new { Item = "フォームクラス", Content = "GasBillingManagementForm, GasBillingForm" },
                new { Item = "説明", Content = "ガス料金請求データの一覧表示、新規登録、編集、削除を行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、請求年月、ビル名、使用量、開始日、終了日、請求金額、お客様番号、インボイス番号、登録日" },
                new { Item = "管理項目", Content = "請求年月（必須、形式: YYYY-MM）、ビル名（必須、登録済みビル名から選択）、使用量（必須）、開始日（必須）、終了日（必須）、請求金額（必須）、お客様番号（必須）、インボイス番号（必須）" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）" },
                new { Item = "機能", Content = "・新規登録: GasBillingFormを開いて新規ガス料金請求データを登録" },
                new { Item = "", Content = "・編集: 選択したガス料金請求データの情報を編集" },
                new { Item = "", Content = "・削除: 選択したガス料金請求データを削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(14).Height = 300;
            sheet.Row(15).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateInvoiceDetailSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("水道光熱費請求明細一覧");
            
            sheet.Cell(1, 1).Value = "水道光熱費請求明細一覧";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "水道光熱費請求明細一覧" },
                new { Item = "フォームクラス", Content = "InvoiceDetailForm, InvoiceDetailEditForm" },
                new { Item = "説明", Content = "請求明細の一覧表示、新規登録、編集、削除、CSVエクスポートを行います。" },
                new { Item = "一覧表示項目", Content = "ID（幅50px）、請求先、貸主、建物名称、借主、部屋番号、種別、内容、使用量、単位、税込金額、税率、子メータ使用開始日、子メータ使用終了日、親メータ使用開始日、親メータ使用終了日、決定請求日、登録日" },
                new { Item = "管理項目", Content = "請求先、貸主、建物名称、借主、部屋番号、種別、内容、使用量、単位、税込金額、税率、子メータ使用開始日、子メータ使用終了日、親メータ使用開始日、親メータ使用終了日、決定請求日" },
                new { Item = "ボタン", Content = "新規登録、編集、削除、更新（リフレッシュ）、CSVエクスポート" },
                new { Item = "機能", Content = "・新規登録: InvoiceDetailEditFormを開いて新規請求明細を登録" },
                new { Item = "", Content = "・編集: 選択した請求明細の情報を編集" },
                new { Item = "", Content = "・削除: 選択した請求明細を削除（確認ダイアログ表示）" },
                new { Item = "", Content = "・更新: データベースから最新のデータを読み込み" },
                new { Item = "", Content = "・CSVエクスポート: UTF-8 BOM付きCSV形式で全項目を出力（日付はyyyy-MM-dd形式、数値は小数点以下2桁）" },
                new { Item = "画像（一覧）", Content = "※ここに一覧画面のスクリーンショット画像を貼り付けてください" },
                new { Item = "画像（登録/編集）", Content = "※ここに登録/編集画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(16).Height = 300;
            sheet.Row(17).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
        
        private static void CreateBillingDataRegistrationSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("請求データ登録");
            
            sheet.Cell(1, 1).Value = "請求データ登録";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            sheet.Cell(3, 1).Value = "項目";
            sheet.Cell(3, 2).Value = "内容";
            sheet.Cell(3, 1).Style.Font.Bold = true;
            sheet.Cell(3, 2).Style.Font.Bold = true;
            
            var items = new[]
            {
                new { Item = "画面名", Content = "請求データ登録" },
                new { Item = "フォームクラス", Content = "BillingDataRegistrationForm" },
                new { Item = "説明", Content = "請求書の印刷を行います。印刷前に請求書.xlsxのAN1セルに印刷日を挿入します。" },
                new { Item = "機能", Content = "・請求書印刷: プリンタ選択ダイアログを表示し、選択したプリンタで請求書を印刷" },
                new { Item = "", Content = "・印刷日挿入: 印刷前に請求書.xlsxのAN1セルに現在の日付（yyyy/MM/dd形式）を挿入" },
                new { Item = "", Content = "・請求一覧印刷: プリンタ選択ダイアログを表示し、選択したプリンタで請求一覧を印刷" },
                new { Item = "画像", Content = "※ここに画面のスクリーンショット画像を貼り付けてください" }
            };
            
            int row = 4;
            foreach (var item in items)
            {
                sheet.Cell(row, 1).Value = item.Item;
                sheet.Cell(row, 2).Value = item.Content;
                row++;
            }
            
            sheet.Row(10).Height = 300;
            sheet.Column(2).Width = 60;
            sheet.Columns().AdjustToContents();
        }
    }
}






