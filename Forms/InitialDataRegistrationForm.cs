using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterUtilityCost.DataAccess;
using WaterUtilityCost.Models;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 初期データ登録フォーム
    /// </summary>
    public partial class InitialDataRegistrationForm : Form
    {
        public InitialDataRegistrationForm()
        {
            InitializeComponent();
            InitializeComponentAdditional();
        }

        /// <summary>
        /// 追加のコンポーネント初期化処理
        /// </summary>
        private void InitializeComponentAdditional()
        {
            _btnBuildingRegistration.Click += BtnBuildingRegistration_Click;
            _btnFloorRegistration.Click += BtnFloorRegistration_Click;
            _btnTenantRegistration.Click += BtnTenantRegistration_Click;
            _btnContractorRegistration.Click += BtnContractorRegistration_Click;
            _btnMeterRegistration.Click += BtnMeterRegistration_Click;
            _btnChildMeterRegistration.Click += BtnChildMeterRegistration_Click;
            _btnRoomChildMeterRegistration.Click += BtnRoomChildMeterRegistration_Click;
        }

        /// <summary>
        /// ビル情報データ登録ボタンのクリックイベントハンドラー
        /// </summary>
        private async void BtnBuildingRegistration_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "ビル情報CSVファイルを選択",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                var buildings = LoadBuildingsFromCsv(openFileDialog.FileName);
                if (buildings.Count == 0)
                {
                    MessageBox.Show("登録対象のビル情報がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await BuildingDataAccess.ReplaceBuildingsAsync(buildings);

                MessageBox.Show($"ビル情報を登録しました。（{buildings.Count}件）", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ビル情報の登録に失敗しました。\n\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 部屋情報データ登録ボタンのクリックイベントハンドラー
        /// </summary>
        private async void BtnFloorRegistration_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "部屋情報CSVファイルを選択",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                var floors = await LoadFloorsFromCsvAsync(openFileDialog.FileName);
                if (floors.Count == 0)
                {
                    MessageBox.Show("登録対象の部屋情報がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await FloorDataAccess.ReplaceFloorsAsync(floors);

                MessageBox.Show($"部屋情報を登録しました。（{floors.Count}件）", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"部屋情報の登録に失敗しました。\n\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 賃借人情報データ登録ボタンのクリックイベントハンドラー
        /// </summary>
        private async void BtnTenantRegistration_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "賃借人情報CSVファイルを選択",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                var clients = await LoadClientsFromCsvAsync(openFileDialog.FileName);
                if (clients.Count == 0)
                {
                    MessageBox.Show("登録対象の賃借人情報がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await ClientDataAccess.ReplaceClientsAsync(clients);

                MessageBox.Show($"賃借人情報を登録しました。（{clients.Count}件）", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"賃借人情報の登録に失敗しました。\n\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 業者情報データ登録ボタンのクリックイベントハンドラー
        /// </summary>
        private async void BtnContractorRegistration_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "業者情報CSVファイルを選択",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                var clients = await LoadClientsFromCsvAsync(openFileDialog.FileName);
                if (clients.Count == 0)
                {
                    MessageBox.Show("登録対象の業者情報がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (var client in clients)
                {
                    // 業者情報登録では必ず業者フラグをONにする
                    client.IsContractor = true;
                }

                await ClientDataAccess.AddClientsAsync(clients);

                MessageBox.Show($"業者情報を追加しました。（{clients.Count}件）", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"業者情報の登録に失敗しました。\n\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 親メーター情報データ登録ボタンのクリックイベントハンドラー
        /// </summary>
        private async void BtnMeterRegistration_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "親メーター情報CSVファイルを選択",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                var meters = await LoadMetersFromCsvAsync(openFileDialog.FileName);
                if (meters.Count == 0)
                {
                    MessageBox.Show("登録対象の親メーター情報がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await MeterDataAccess.ReplaceMetersAsync(meters);

                MessageBox.Show($"親メーター情報を登録しました。（{meters.Count}件）", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"親メーター情報の登録に失敗しました。\n\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 子メーター情報データ登録ボタンのクリックイベントハンドラー
        /// </summary>
        private async void BtnChildMeterRegistration_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "子メーター情報CSVファイルを選択",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                var childMeters = await LoadChildMetersFromCsvAsync(openFileDialog.FileName);
                if (childMeters.Count == 0)
                {
                    MessageBox.Show("登録対象の子メーター情報がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await ChildMeterDataAccess.ReplaceChildMetersAsync(childMeters);

                MessageBox.Show($"子メーター情報を登録しました。（{childMeters.Count}件）", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"子メーター情報の登録に失敗しました。\n\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 部屋別子メーター管理データ登録ボタンのクリックイベントハンドラー
        /// </summary>
        private async void BtnRoomChildMeterRegistration_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "部屋別子メーター管理CSVファイルを選択",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                var roomChildMeters = await LoadRoomChildMetersFromCsvAsync(openFileDialog.FileName);
                if (roomChildMeters.Count == 0)
                {
                    MessageBox.Show("登録対象の部屋別子メーター管理データがありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await RoomChildMeterDataAccess.ReplaceRoomChildMetersAsync(roomChildMeters);

                MessageBox.Show($"部屋別子メーター管理データを登録しました。（{roomChildMeters.Count}件）", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"部屋別子メーター管理データの登録に失敗しました。\n\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static List<Building> LoadBuildingsFromCsv(string filePath)
        {
            var buildings = new List<Building>();
            using var reader = new StreamReader(filePath, Encoding.UTF8, true);

            var lineNumber = 0;
            var headerProcessed = false;
            var headerMap = new Dictionary<string, int>();

            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = ParseCsvLine(line);
                if (!headerProcessed)
                {
                    headerProcessed = true;
                    var detectedHeader = TryBuildHeaderMap(fields);
                    if (detectedHeader != null)
                    {
                        headerMap = detectedHeader;
                        continue;
                    }
                }

                var building = ParseBuilding(fields, headerMap, lineNumber);
                if (building != null)
                {
                    buildings.Add(building);
                }
            }

            return buildings;
        }

        private static async Task<List<Floor>> LoadFloorsFromCsvAsync(string filePath)
        {
            var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
            var buildingNameMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.Name))
                .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var buildingCodeMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.BuildingId))
                .GroupBy(b => b.BuildingId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var buildingsById = buildings.ToDictionary(b => b.Id);

            var floors = new List<Floor>();
            using var reader = new StreamReader(filePath, Encoding.UTF8, true);

            var lineNumber = 0;
            var headerProcessed = false;
            var headerMap = new Dictionary<string, int>();

            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = ParseCsvLine(line);
                if (!headerProcessed)
                {
                    headerProcessed = true;
                    var detectedHeader = TryBuildFloorHeaderMap(fields);
                    if (detectedHeader != null)
                    {
                        headerMap = detectedHeader;
                        continue;
                    }
                }

                var floor = ParseFloor(fields, headerMap, lineNumber, buildingNameMap, buildingCodeMap, buildingsById);
                if (floor != null)
                {
                    floors.Add(floor);
                }
            }

            return floors;
        }

        private static async Task<List<Client>> LoadClientsFromCsvAsync(string filePath)
        {
            var clients = new List<Client>();
            using var reader = new StreamReader(filePath, Encoding.UTF8, true);

            var lineNumber = 0;
            var headerProcessed = false;
            var headerMap = new Dictionary<string, int>();

            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = ParseCsvLine(line);
                if (!headerProcessed)
                {
                    headerProcessed = true;
                    var detectedHeader = TryBuildClientHeaderMap(fields);
                    if (detectedHeader != null)
                    {
                        headerMap = detectedHeader;
                        continue;
                    }
                }

                var client = ParseClient(fields, headerMap, lineNumber);
                if (client != null)
                {
                    clients.Add(client);
                }
            }

            return clients;
        }

        private static async Task<List<Meter>> LoadMetersFromCsvAsync(string filePath)
        {
            var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
            var buildingNameMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.Name))
                .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var buildingCodeMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.BuildingId))
                .GroupBy(b => b.BuildingId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var buildingsById = buildings.ToDictionary(b => b.Id);

            var allClients = await ClientDataAccess.GetAllClientsAsync();
            var clients = allClients
                .Where(c => c.IsContractor)
                .ToList();
            var contractorNameMap = clients
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .GroupBy(c => NormalizeNameKey(c.Name))
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var contractorsById = clients.ToDictionary(c => c.Id);
            var allClientNameMap = allClients
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .GroupBy(c => NormalizeNameKey(c.Name))
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var allClientsById = allClients.ToDictionary(c => c.Id);

            var meters = new List<Meter>();
            using var reader = new StreamReader(filePath, Encoding.UTF8, true);

            var lineNumber = 0;
            var headerProcessed = false;
            var headerMap = new Dictionary<string, int>();

            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = ParseCsvLine(line);
                if (!headerProcessed)
                {
                    headerProcessed = true;
                    var detectedHeader = TryBuildMeterHeaderMap(fields);
                    if (detectedHeader != null)
                    {
                        headerMap = detectedHeader;
                        continue;
                    }
                }

                var meter = await ParseMeterAsync(
                    fields,
                    headerMap,
                    lineNumber,
                    buildingNameMap,
                    buildingCodeMap,
                    buildingsById,
                    contractorNameMap,
                    contractorsById,
                    allClientNameMap,
                    allClientsById);
                if (meter != null)
                {
                    meters.Add(meter);
                }
            }

            return meters;
        }

        private static async Task<List<ChildMeter>> LoadChildMetersFromCsvAsync(string filePath)
        {
            var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
            var buildingNameMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.Name))
                .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var buildingCodeMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.BuildingId))
                .GroupBy(b => b.BuildingId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var buildingsById = buildings.ToDictionary(b => b.Id);

            var meters = await MeterDataAccess.GetAllMetersAsync();
            var metersById = meters.ToDictionary(m => m.Id);
            var metersByName = meters
                .Where(m => !string.IsNullOrWhiteSpace(m.MeterName))
                .GroupBy(m => NormalizeTextKey(m.MeterName))
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            var childMeters = new List<ChildMeter>();
            using var reader = new StreamReader(filePath, Encoding.UTF8, true);

            var lineNumber = 0;
            var headerProcessed = false;
            var headerMap = new Dictionary<string, int>();

            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = ParseCsvLine(line);
                if (!headerProcessed)
                {
                    headerProcessed = true;
                    var detectedHeader = TryBuildChildMeterHeaderMap(fields);
                    if (detectedHeader != null)
                    {
                        headerMap = detectedHeader;
                        continue;
                    }
                }

                var childMeter = ParseChildMeter(
                    fields,
                    headerMap,
                    lineNumber,
                    buildingNameMap,
                    buildingCodeMap,
                    buildingsById,
                    metersById,
                    metersByName);
                if (childMeter != null)
                {
                    childMeters.Add(childMeter);
                }
            }

            return childMeters;
        }

        private static async Task<List<RoomChildMeter>> LoadRoomChildMetersFromCsvAsync(string filePath)
        {
            var buildings = await BuildingDataAccess.GetAllBuildingsAsync();
            var buildingNameMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.Name))
                .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var buildingCodeMap = buildings
                .Where(b => !string.IsNullOrWhiteSpace(b.BuildingId))
                .GroupBy(b => b.BuildingId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var buildingsById = buildings.ToDictionary(b => b.Id);

            var floors = await FloorDataAccess.GetAllFloorsAsync();
            var floorsByBuilding = floors
                .GroupBy(f => f.BuildingId)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .Where(f => !string.IsNullOrWhiteSpace(f.FloorName))
                        .GroupBy(f => NormalizeTextKey(f.FloorName))
                        .ToDictionary(fg => fg.Key, fg => fg.First(), StringComparer.OrdinalIgnoreCase));

            var childMeters = await ChildMeterDataAccess.GetAllChildMetersAsync();
            var childMetersByBuilding = childMeters
                .Where(cm => cm.BuildingId.HasValue)
                .GroupBy(cm => cm.BuildingId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .Where(cm => !string.IsNullOrWhiteSpace(cm.MeterName))
                        .GroupBy(cm => NormalizeTextKey(cm.MeterName))
                        .ToDictionary(cg => cg.Key, cg => cg.ToList(), StringComparer.OrdinalIgnoreCase));

            var meters = await MeterDataAccess.GetAllMetersAsync();
            var parentMetersByBuilding = meters
                .Where(m => m.BuildingId.HasValue)
                .GroupBy(m => m.BuildingId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .Where(m => !string.IsNullOrWhiteSpace(m.MeterName))
                        .GroupBy(m => NormalizeTextKey(m.MeterName))
                        .ToDictionary(mg => mg.Key, mg => mg.ToList(), StringComparer.OrdinalIgnoreCase));

            var roomChildMeters = new List<RoomChildMeter>();
            using var reader = new StreamReader(filePath, Encoding.UTF8, true);

            var lineNumber = 0;
            var headerProcessed = false;
            var headerMap = new Dictionary<string, int>();

            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = ParseCsvLine(line);
                if (!headerProcessed)
                {
                    headerProcessed = true;
                    var detectedHeader = TryBuildRoomChildMeterHeaderMap(fields);
                    if (detectedHeader != null)
                    {
                        headerMap = detectedHeader;
                        continue;
                    }
                }

                var roomChildMeter = ParseRoomChildMeter(
                    fields,
                    headerMap,
                    lineNumber,
                    buildingNameMap,
                    buildingCodeMap,
                    buildingsById,
                    floorsByBuilding,
                    childMetersByBuilding,
                    parentMetersByBuilding);
                if (roomChildMeter != null)
                {
                    roomChildMeters.Add(roomChildMeter);
                }
            }

            return roomChildMeters;
        }

        private static Building? ParseBuilding(List<string> fields, Dictionary<string, int> headerMap, int lineNumber)
        {
            string GetValue(string key, int fallbackIndex)
            {
                if (headerMap.Count > 0 && headerMap.TryGetValue(key, out var index) && index < fields.Count)
                {
                    return fields[index];
                }

                return fallbackIndex < fields.Count ? fields[fallbackIndex] : string.Empty;
            }

            if (headerMap.Count == 0 && fields.Count < 2)
            {
                throw new InvalidOperationException($"CSVの列数が不足しています。（{lineNumber}行目）");
            }

            var buildingId = GetValue("BuildingId", 0)?.Trim();
            var name = GetValue("Name", 1)?.Trim();
            var address = GetValue("Address", 2)?.Trim();
            var floorsText = GetValue("Floors", 3)?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException($"ビル名が空の行があります。（{lineNumber}行目）");
            }

            var floors = 1;
            if (!string.IsNullOrWhiteSpace(floorsText))
            {
                if (!int.TryParse(floorsText, out floors) || floors <= 0)
                {
                    throw new InvalidOperationException($"階数が不正です。（{lineNumber}行目）");
                }
            }

            return new Building
            {
                BuildingId = buildingId ?? string.Empty,
                Name = name,
                Address = address ?? string.Empty,
                Floors = floors
            };
        }

        private static Floor? ParseFloor(
            List<string> fields,
            Dictionary<string, int> headerMap,
            int lineNumber,
            Dictionary<string, int> buildingNameMap,
            Dictionary<string, int> buildingCodeMap,
            Dictionary<int, Building> buildingsById)
        {
            string GetValue(string key, int fallbackIndex)
            {
                if (headerMap.Count > 0 && headerMap.TryGetValue(key, out var index) && index < fields.Count)
                {
                    return fields[index];
                }

                return fallbackIndex < fields.Count ? fields[fallbackIndex] : string.Empty;
            }

            if (headerMap.Count == 0 && fields.Count < 2)
            {
                throw new InvalidOperationException($"CSVの列数が不足しています。（{lineNumber}行目）");
            }

            var buildingIdText = GetValue("BuildingId", 0)?.Trim();
            var buildingNameText = GetValue("BuildingName", 0)?.Trim();
            var floorName = GetValue("FloorName", 1)?.Trim();
            var floorAreaText = GetValue("FloorArea", 2)?.Trim();

            if (string.IsNullOrWhiteSpace(buildingIdText) && string.IsNullOrWhiteSpace(buildingNameText))
            {
                throw new InvalidOperationException($"ビル情報が空の行があります。（{lineNumber}行目）");
            }

            if (string.IsNullOrWhiteSpace(floorName))
            {
                throw new InvalidOperationException($"部屋名が空の行があります。（{lineNumber}行目）");
            }

            var buildingId = ResolveBuildingId(buildingIdText, buildingNameText, buildingNameMap, buildingCodeMap, buildingsById, lineNumber);

            var floorArea = 0m;
            if (!string.IsNullOrWhiteSpace(floorAreaText))
            {
                if (!decimal.TryParse(floorAreaText, out floorArea) || floorArea < 0)
                {
                    throw new InvalidOperationException($"部屋面積が不正です。（{lineNumber}行目）");
                }
            }

            return new Floor
            {
                BuildingId = buildingId,
                FloorName = floorName,
                FloorArea = floorArea
            };
        }

        private static Client? ParseClient(List<string> fields, Dictionary<string, int> headerMap, int lineNumber)
        {
            string GetValue(string key, int fallbackIndex)
            {
                if (headerMap.Count > 0 && headerMap.TryGetValue(key, out var index) && index < fields.Count)
                {
                    return fields[index];
                }

                return fallbackIndex < fields.Count ? fields[fallbackIndex] : string.Empty;
            }

            if (headerMap.Count == 0 && fields.Count < 1)
            {
                throw new InvalidOperationException($"CSVの列数が不足しています。（{lineNumber}行目）");
            }

            var name = GetValue("Name", 0)?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                if (fields.All(field => string.IsNullOrWhiteSpace(field)))
                {
                    return null;
                }
                throw new InvalidOperationException($"取引先名が空の行があります。（{lineNumber}行目）");
            }

            var isLessor = ParseBoolean(GetValue("IsLessor", 1));
            var isLessee = ParseBoolean(GetValue("IsLessee", 2));
            var isBillingTo = ParseBoolean(GetValue("IsBillingTo", 3));
            var isContractor = ParseBoolean(GetValue("IsContractor", 4));

            var invoiceNumber = GetValue("InvoiceNumber", 5)?.Trim();
            var buildingName = GetValue("BuildingName", 6)?.Trim();
            var roomName = GetValue("RoomName", 7)?.Trim();
            var postalCode = GetValue("PostalCode", 8)?.Trim();
            var address = GetValue("Address", 9)?.Trim();
            var phone = GetValue("Phone", 10)?.Trim();

            return new Client
            {
                Name = name,
                IsLessor = isLessor,
                IsLessee = isLessee,
                IsBillingTo = isBillingTo,
                IsContractor = isContractor,
                InvoiceNumber = invoiceNumber ?? string.Empty,
                BuildingName = buildingName ?? string.Empty,
                RoomName = roomName ?? string.Empty,
                PostalCode = postalCode ?? string.Empty,
                Address = address ?? string.Empty,
                Phone = phone ?? string.Empty
            };
        }

        private static async Task<Meter?> ParseMeterAsync(
            List<string> fields,
            Dictionary<string, int> headerMap,
            int lineNumber,
            Dictionary<string, int> buildingNameMap,
            Dictionary<string, int> buildingCodeMap,
            Dictionary<int, Building> buildingsById,
            Dictionary<string, Client> contractorNameMap,
            Dictionary<int, Client> contractorsById,
            Dictionary<string, Client> allClientNameMap,
            Dictionary<int, Client> allClientsById)
        {
            string GetValue(string key, int fallbackIndex)
            {
                if (headerMap.Count > 0 && headerMap.TryGetValue(key, out var index) && index < fields.Count)
                {
                    return fields[index];
                }

                return fallbackIndex < fields.Count ? fields[fallbackIndex] : string.Empty;
            }

            if (headerMap.Count == 0 && fields.Count < 4)
            {
                throw new InvalidOperationException($"CSVの列数が不足しています。（{lineNumber}行目）");
            }

            var buildingIdText = GetValue("BuildingId", 0)?.Trim();
            var buildingNameText = GetValue("BuildingName", 0)?.Trim();
            var contractorIdText = GetValue("ContractorId", 1)?.Trim();
            var contractorNameText = GetValue("ContractorName", 1)?.Trim();
            var meterType = GetValue("MeterType", 2)?.Trim();
            var meterName = GetValue("MeterName", 3)?.Trim();
            var managementNumber = GetValue("ManagementNumber", 4)?.Trim();

            if (string.IsNullOrWhiteSpace(buildingIdText) && string.IsNullOrWhiteSpace(buildingNameText))
            {
                throw new InvalidOperationException($"ビル情報が空の行があります。（{lineNumber}行目）");
            }

            if (string.IsNullOrWhiteSpace(meterType))
            {
                throw new InvalidOperationException($"メーター種別が空の行があります。（{lineNumber}行目）");
            }

            var buildingId = ResolveBuildingId(buildingIdText, buildingNameText, buildingNameMap, buildingCodeMap, buildingsById, lineNumber);
            var contractorId = await ResolveContractorIdAsync(
                contractorIdText,
                contractorNameText,
                contractorNameMap,
                contractorsById,
                allClientNameMap,
                allClientsById,
                lineNumber);

            return new Meter
            {
                BuildingId = buildingId,
                ContractorId = contractorId,
                MeterType = meterType,
                MeterName = meterName ?? string.Empty,
                ManagementNumber = managementNumber ?? string.Empty
            };
        }

        private static ChildMeter? ParseChildMeter(
            List<string> fields,
            Dictionary<string, int> headerMap,
            int lineNumber,
            Dictionary<string, int> buildingNameMap,
            Dictionary<string, int> buildingCodeMap,
            Dictionary<int, Building> buildingsById,
            Dictionary<int, Meter> metersById,
            Dictionary<string, List<Meter>> metersByName)
        {
            string GetValue(string key, int fallbackIndex)
            {
                if (headerMap.Count > 0 && headerMap.TryGetValue(key, out var index) && index < fields.Count)
                {
                    return fields[index];
                }

                return fallbackIndex < fields.Count ? fields[fallbackIndex] : string.Empty;
            }

            if (headerMap.Count == 0 && fields.Count < 4)
            {
                throw new InvalidOperationException($"CSVの列数が不足しています。（{lineNumber}行目）");
            }

            var buildingIdText = GetValue("BuildingId", 0)?.Trim();
            var buildingNameText = GetValue("BuildingName", 0)?.Trim();
            var meterType = GetValue("MeterType", 1)?.Trim();
            var parentMeterNameText = GetValue("ParentMeterName", 2)?.Trim();
            var childMeterName = GetValue("ChildMeterName", 3)?.Trim();
            var notes = GetValue("Notes", 4)?.Trim();

            var hasBuildingInfo = !string.IsNullOrWhiteSpace(buildingIdText) || !string.IsNullOrWhiteSpace(buildingNameText);
            int? buildingId = null;
            if (hasBuildingInfo)
            {
                buildingId = ResolveBuildingId(buildingIdText, buildingNameText, buildingNameMap, buildingCodeMap, buildingsById, lineNumber);
            }

            if (string.IsNullOrWhiteSpace(meterType))
            {
                throw new InvalidOperationException($"メーター種別が空の行があります。（{lineNumber}行目）");
            }

            if (string.IsNullOrWhiteSpace(childMeterName))
            {
                throw new InvalidOperationException($"子メーター名が空の行があります。（{lineNumber}行目）");
            }

            if (string.IsNullOrWhiteSpace(parentMeterNameText))
            {
                throw new InvalidOperationException($"親メーター名が空の行があります。（{lineNumber}行目）");
            }

            var parentMeter = ResolveParentMeterByName(
                parentMeterNameText,
                metersByName,
                buildingId,
                meterType,
                lineNumber);

            if (!buildingId.HasValue)
            {
                if (parentMeter.BuildingId.HasValue)
                {
                    buildingId = parentMeter.BuildingId.Value;
                }
                else
                {
                    throw new InvalidOperationException($"ビル情報が空の行があります。（{lineNumber}行目）");
                }
            }

            return new ChildMeter
            {
                BuildingId = buildingId,
                ParentMeterId = parentMeter.Id,
                MeterType = meterType,
                MeterName = childMeterName,
                Notes = notes ?? string.Empty
            };
        }

        private static RoomChildMeter? ParseRoomChildMeter(
            List<string> fields,
            Dictionary<string, int> headerMap,
            int lineNumber,
            Dictionary<string, int> buildingNameMap,
            Dictionary<string, int> buildingCodeMap,
            Dictionary<int, Building> buildingsById,
            Dictionary<int, Dictionary<string, Floor>> floorsByBuilding,
            Dictionary<int, Dictionary<string, List<ChildMeter>>> childMetersByBuilding,
            Dictionary<int, Dictionary<string, List<Meter>>> parentMetersByBuilding)
        {
            string GetValue(string key, int fallbackIndex)
            {
                if (headerMap.Count > 0 && headerMap.TryGetValue(key, out var index) && index < fields.Count)
                {
                    return fields[index];
                }

                return fallbackIndex < fields.Count ? fields[fallbackIndex] : string.Empty;
            }

            if (headerMap.Count == 0 && fields.Count < 5)
            {
                throw new InvalidOperationException($"CSVの列数が不足しています。（{lineNumber}行目）");
            }

            var buildingIdText = GetValue("BuildingId", 0)?.Trim();
            var buildingNameText = GetValue("BuildingName", 0)?.Trim();
            var roomName = GetValue("RoomName", 1)?.Trim();
            var meterType = GetValue("MeterType", 2)?.Trim();
            var parentMeterName = GetValue("ParentMeterName", 3)?.Trim();
            var childMeterName = GetValue("ChildMeterName", 4)?.Trim();

            if (string.IsNullOrWhiteSpace(buildingIdText) && string.IsNullOrWhiteSpace(buildingNameText))
            {
                throw new InvalidOperationException($"ビル情報が空の行があります。（{lineNumber}行目）");
            }

            if (string.IsNullOrWhiteSpace(roomName))
            {
                throw new InvalidOperationException($"部屋名が空の行があります。（{lineNumber}行目）");
            }

            if (string.IsNullOrWhiteSpace(meterType))
            {
                throw new InvalidOperationException($"メーター種別が空の行があります。（{lineNumber}行目）");
            }

            if (string.IsNullOrWhiteSpace(childMeterName))
            {
                throw new InvalidOperationException($"子メーター名が空の行があります。（{lineNumber}行目）");
            }

            if (string.IsNullOrWhiteSpace(parentMeterName))
            {
                throw new InvalidOperationException($"親メーター名が空の行があります。（{lineNumber}行目）");
            }

            var buildingId = ResolveBuildingId(buildingIdText, buildingNameText, buildingNameMap, buildingCodeMap, buildingsById, lineNumber);
            var floorId = ResolveFloorId(buildingId, roomName, floorsByBuilding, lineNumber);
            var parentMeterId = ResolveParentMeterId(buildingId, parentMeterName, meterType, parentMetersByBuilding, lineNumber);
            var childMeterId = ResolveChildMeterId(buildingId, childMeterName, meterType, parentMeterId, childMetersByBuilding, lineNumber);

            return new RoomChildMeter
            {
                FloorId = floorId,
                ChildMeterId = childMeterId
            };
        }

        private static int ResolveFloorId(
            int buildingId,
            string roomName,
            Dictionary<int, Dictionary<string, Floor>> floorsByBuilding,
            int lineNumber)
        {
            if (!floorsByBuilding.TryGetValue(buildingId, out var floorsByName))
            {
                throw new InvalidOperationException($"部屋情報が見つかりません。（{lineNumber}行目: {roomName}）");
            }

            var normalizedRoomName = NormalizeTextKey(roomName);
            if (!floorsByName.TryGetValue(normalizedRoomName, out var floor))
            {
                throw new InvalidOperationException($"部屋情報が見つかりません。（{lineNumber}行目: {roomName}）");
            }

            return floor.Id;
        }

        private static int ResolveParentMeterId(
            int buildingId,
            string parentMeterName,
            string? meterType,
            Dictionary<int, Dictionary<string, List<Meter>>> parentMetersByBuilding,
            int lineNumber)
        {
            if (!parentMetersByBuilding.TryGetValue(buildingId, out var parentMetersByName))
            {
                throw new InvalidOperationException($"親メーター情報が見つかりません。（{lineNumber}行目: {parentMeterName}）");
            }

            var normalizedParentName = NormalizeTextKey(parentMeterName);
            if (!parentMetersByName.TryGetValue(normalizedParentName, out var candidates))
            {
                throw new InvalidOperationException($"親メーター情報が見つかりません。（{lineNumber}行目: {parentMeterName}）");
            }

            IEnumerable<Meter> filtered = candidates;
            if (!string.IsNullOrWhiteSpace(meterType))
            {
                filtered = filtered.Where(m => string.Equals(m.MeterType, meterType, StringComparison.OrdinalIgnoreCase));
            }

            var matches = filtered.ToList();
            if (matches.Count == 1)
            {
                return matches[0].Id;
            }
            if (matches.Count == 0)
            {
                throw new InvalidOperationException($"親メーター情報が見つかりません。（{lineNumber}行目: {parentMeterName}）");
            }
            throw new InvalidOperationException($"親メーター名が重複しています。（{lineNumber}行目: {parentMeterName}）");
        }

        private static int ResolveChildMeterId(
            int buildingId,
            string childMeterName,
            string? meterType,
            int parentMeterId,
            Dictionary<int, Dictionary<string, List<ChildMeter>>> childMetersByBuilding,
            int lineNumber)
        {
            if (!childMetersByBuilding.TryGetValue(buildingId, out var childMetersByName))
            {
                throw new InvalidOperationException($"子メーター情報が見つかりません。（{lineNumber}行目: {childMeterName}）");
            }

            var normalizedChildMeterName = NormalizeTextKey(childMeterName);
            if (!childMetersByName.TryGetValue(normalizedChildMeterName, out var candidates))
            {
                throw new InvalidOperationException($"子メーター情報が見つかりません。（{lineNumber}行目: {childMeterName}）");
            }

            IEnumerable<ChildMeter> filtered = candidates;
            if (!string.IsNullOrWhiteSpace(meterType))
            {
                filtered = filtered.Where(cm => string.Equals(cm.MeterType, meterType, StringComparison.OrdinalIgnoreCase));
            }
            filtered = filtered.Where(cm => cm.ParentMeterId.HasValue && cm.ParentMeterId.Value == parentMeterId);

            var matches = filtered.ToList();
            if (matches.Count == 1)
            {
                return matches[0].Id;
            }
            if (matches.Count == 0)
            {
                throw new InvalidOperationException($"子メーター情報が見つかりません。（{lineNumber}行目: {childMeterName}）");
            }
            throw new InvalidOperationException($"子メーター名が重複しています。（{lineNumber}行目: {childMeterName}）");
        }

        private static Meter ResolveParentMeterByName(
            string parentMeterNameText,
            Dictionary<string, List<Meter>> metersByName,
            int? buildingId,
            string? meterType,
            int lineNumber)
        {
            var normalizedName = NormalizeTextKey(parentMeterNameText);
            if (!metersByName.TryGetValue(normalizedName, out var candidates))
            {
                throw new InvalidOperationException($"親メーター名が存在しません。（{lineNumber}行目: {parentMeterNameText}）");
            }

            IEnumerable<Meter> filtered = candidates;
            if (buildingId.HasValue)
            {
                filtered = filtered.Where(m => m.BuildingId.HasValue && m.BuildingId.Value == buildingId.Value);
            }
            if (!string.IsNullOrWhiteSpace(meterType))
            {
                filtered = filtered.Where(m => string.Equals(m.MeterType, meterType, StringComparison.OrdinalIgnoreCase));
            }

            var matches = filtered.ToList();
            if (matches.Count == 1)
            {
                return matches[0];
            }
            if (matches.Count == 0)
            {
                throw new InvalidOperationException($"親メーター名が存在しません。（{lineNumber}行目: {parentMeterNameText}）");
            }
            throw new InvalidOperationException($"親メーター名が重複しています。（{lineNumber}行目: {parentMeterNameText}）");
        }

        private static int ResolveBuildingId(
            string? buildingIdText,
            string? buildingNameText,
            Dictionary<string, int> buildingNameMap,
            Dictionary<string, int> buildingCodeMap,
            Dictionary<int, Building> buildingsById,
            int lineNumber)
        {
            if (!string.IsNullOrWhiteSpace(buildingIdText))
            {
                if (buildingCodeMap.TryGetValue(buildingIdText, out var buildingId))
                {
                    return buildingId;
                }

                if (int.TryParse(buildingIdText, out var parsedId) && buildingsById.ContainsKey(parsedId))
                {
                    return parsedId;
                }
            }

            if (!string.IsNullOrWhiteSpace(buildingNameText))
            {
                if (buildingNameMap.TryGetValue(buildingNameText, out var buildingId))
                {
                    return buildingId;
                }
            }

            var displayValue = !string.IsNullOrWhiteSpace(buildingNameText) ? buildingNameText : buildingIdText;
            throw new InvalidOperationException($"ビル情報が見つかりません。（{lineNumber}行目: {displayValue}）");
        }

        private static Dictionary<string, int>? TryBuildHeaderMap(List<string> headers)
        {
            var map = new Dictionary<string, int>();
            for (var i = 0; i < headers.Count; i++)
            {
                var key = NormalizeHeader(headers[i]);
                if (IsHeaderMatch(key, "buildingid", "ビルid", "ビルｉｄ", "ビルID", "ビルＩＤ", "building_id"))
                {
                    map["BuildingId"] = i;
                }
                else if (IsHeaderMatch(key, "name", "ビル名", "建物名"))
                {
                    map["Name"] = i;
                }
                else if (IsHeaderMatch(key, "address", "住所"))
                {
                    map["Address"] = i;
                }
                else if (IsHeaderMatch(key, "floors", "階数", "フロア数"))
                {
                    map["Floors"] = i;
                }
            }

            return map.Count > 0 ? map : null;
        }

        private static Dictionary<string, int>? TryBuildFloorHeaderMap(List<string> headers)
        {
            var map = new Dictionary<string, int>();
            for (var i = 0; i < headers.Count; i++)
            {
                var key = NormalizeHeader(headers[i]);
                if (IsHeaderMatch(key, "buildingid", "ビルid", "ビルｉｄ", "ビルID", "ビルＩＤ", "building_id"))
                {
                    map["BuildingId"] = i;
                }
                else if (IsHeaderMatch(key, "buildingname", "ビル名", "建物名"))
                {
                    map["BuildingName"] = i;
                }
                else if (IsHeaderMatch(key, "floorname", "部屋名", "部屋番号", "フロア名"))
                {
                    map["FloorName"] = i;
                }
                else if (IsHeaderMatch(key, "floorarea", "部屋面積", "床面積", "面積", "専有面積"))
                {
                    map["FloorArea"] = i;
                }
            }

            return map.Count > 0 ? map : null;
        }

        private static Dictionary<string, int>? TryBuildClientHeaderMap(List<string> headers)
        {
            var map = new Dictionary<string, int>();
            for (var i = 0; i < headers.Count; i++)
            {
                var key = NormalizeHeader(headers[i]);
                if (IsHeaderMatch(key, "name", "取引先名", "賃借人名", "借主名", "業者名", "会社名"))
                {
                    map["Name"] = i;
                }
                else if (IsHeaderMatch(key, "islessor", "貸主", "貸主フラグ", "貸主区分"))
                {
                    map["IsLessor"] = i;
                }
                else if (IsHeaderMatch(key, "islessee", "借主", "賃借人", "借主フラグ", "借主区分"))
                {
                    map["IsLessee"] = i;
                }
                else if (IsHeaderMatch(key, "isbillingto", "請求先", "請求先フラグ", "請求先区分"))
                {
                    map["IsBillingTo"] = i;
                }
                else if (IsHeaderMatch(key, "iscontractor", "業者", "業者フラグ", "業者区分"))
                {
                    map["IsContractor"] = i;
                }
                else if (IsHeaderMatch(key, "invoicenumber", "インボイス番号", "請求番号"))
                {
                    map["InvoiceNumber"] = i;
                }
                else if (IsHeaderMatch(key, "buildingname", "ビル名", "建物名"))
                {
                    map["BuildingName"] = i;
                }
                else if (IsHeaderMatch(key, "roomname", "部屋名", "部屋番号"))
                {
                    map["RoomName"] = i;
                }
                else if (IsHeaderMatch(key, "postalcode", "郵便番号"))
                {
                    map["PostalCode"] = i;
                }
                else if (IsHeaderMatch(key, "address", "住所"))
                {
                    map["Address"] = i;
                }
                else if (IsHeaderMatch(key, "phone", "電話番号", "電話"))
                {
                    map["Phone"] = i;
                }
            }

            return map.Count > 0 ? map : null;
        }

        private static Dictionary<string, int>? TryBuildMeterHeaderMap(List<string> headers)
        {
            var map = new Dictionary<string, int>();
            for (var i = 0; i < headers.Count; i++)
            {
                var key = NormalizeHeader(headers[i]);
                if (IsHeaderMatch(key, "buildingid", "ビルid", "ビルｉｄ", "ビルID", "ビルＩＤ", "building_id"))
                {
                    map["BuildingId"] = i;
                }
                else if (IsHeaderMatch(key, "buildingname", "ビル名", "建物名"))
                {
                    map["BuildingName"] = i;
                }
                else if (IsHeaderMatch(key, "contractorid", "業者id", "業者ｉｄ", "業者ID", "業者ＩＤ", "contractor_id"))
                {
                    map["ContractorId"] = i;
                }
                else if (IsHeaderMatch(key, "contractorname", "業者名", "取引先名", "会社名"))
                {
                    map["ContractorName"] = i;
                }
                else if (IsHeaderMatch(key, "metertype", "メーター種別", "種別"))
                {
                    map["MeterType"] = i;
                }
                else if (IsHeaderMatch(key, "metername", "親メーター名", "メーター名"))
                {
                    map["MeterName"] = i;
                }
                else if (IsHeaderMatch(key, "managementnumber", "管理番号", "管理No", "管理no"))
                {
                    map["ManagementNumber"] = i;
                }
            }

            return map.Count > 0 ? map : null;
        }

        private static Dictionary<string, int>? TryBuildChildMeterHeaderMap(List<string> headers)
        {
            var map = new Dictionary<string, int>();
            for (var i = 0; i < headers.Count; i++)
            {
                var key = NormalizeHeader(headers[i]);
                if (IsHeaderMatch(key, "buildingid", "ビルid", "ビルｉｄ", "ビルID", "ビルＩＤ", "building_id"))
                {
                    map["BuildingId"] = i;
                }
                else if (IsHeaderMatch(key, "buildingname", "ビル名", "建物名"))
                {
                    map["BuildingName"] = i;
                }
                else if (IsHeaderMatch(key, "parentmetername", "親メーター名"))
                {
                    map["ParentMeterName"] = i;
                }
                else if (IsHeaderMatch(key, "metertype", "メーター種別", "種別"))
                {
                    map["MeterType"] = i;
                }
                else if (IsHeaderMatch(key, "parentmetername", "親メーター名"))
                {
                    map["ParentMeterName"] = i;
                }
                else if (IsHeaderMatch(key, "childmetername", "子メーター名", "メーター名"))
                {
                    map["ChildMeterName"] = i;
                }
                else if (IsHeaderMatch(key, "notes", "備考", "メモ"))
                {
                    map["Notes"] = i;
                }
            }

            return map.Count > 0 ? map : null;
        }

        private static Dictionary<string, int>? TryBuildRoomChildMeterHeaderMap(List<string> headers)
        {
            var map = new Dictionary<string, int>();
            for (var i = 0; i < headers.Count; i++)
            {
                var key = NormalizeHeader(headers[i]);
                if (IsHeaderMatch(key, "buildingid", "ビルid", "ビルｉｄ", "ビルID", "ビルＩＤ", "building_id"))
                {
                    map["BuildingId"] = i;
                }
                else if (IsHeaderMatch(key, "buildingname", "ビル名", "建物名"))
                {
                    map["BuildingName"] = i;
                }
                else if (IsHeaderMatch(key, "roomname", "部屋名", "部屋番号", "フロア名"))
                {
                    map["RoomName"] = i;
                }
                else if (IsHeaderMatch(key, "metertype", "メーター種別", "種別"))
                {
                    map["MeterType"] = i;
                }
                else if (IsHeaderMatch(key, "childmetername", "子メーター名", "メーター名"))
                {
                    map["ChildMeterName"] = i;
                }
            }

            return map.Count > 0 ? map : null;
        }

        private static async Task<int?> ResolveContractorIdAsync(
            string? contractorIdText,
            string? contractorNameText,
            Dictionary<string, Client> contractorNameMap,
            Dictionary<int, Client> contractorsById,
            Dictionary<string, Client> allClientNameMap,
            Dictionary<int, Client> allClientsById,
            int lineNumber)
        {
            if (!string.IsNullOrWhiteSpace(contractorIdText))
            {
                if (int.TryParse(contractorIdText, out var contractorId) && contractorsById.ContainsKey(contractorId))
                {
                    return contractorId;
                }
                if (int.TryParse(contractorIdText, out contractorId) && allClientsById.TryGetValue(contractorId, out var existingClient))
                {
                    await EnsureContractorFlagAsync(existingClient, allClientsById, allClientNameMap);
                    return contractorId;
                }
                throw new InvalidOperationException($"業者情報が見つかりません。（{lineNumber}行目: {contractorIdText}）業者情報を先に登録してください。");
            }

            if (!string.IsNullOrWhiteSpace(contractorNameText))
            {
                var normalizedName = NormalizeNameKey(contractorNameText);
                if (contractorNameMap.TryGetValue(normalizedName, out var existingClient))
                {
                    return existingClient.Id;
                }
                if (allClientNameMap.TryGetValue(normalizedName, out existingClient))
                {
                    await EnsureContractorFlagAsync(existingClient, allClientsById, allClientNameMap);
                    return existingClient.Id;
                }
                throw new InvalidOperationException($"業者情報が見つかりません。（{lineNumber}行目: {contractorNameText}）業者情報を先に登録してください。");
            }

            return null;
        }

        private static async Task EnsureContractorFlagAsync(
            Client client,
            Dictionary<int, Client> allClientsById,
            Dictionary<string, Client> allClientNameMap)
        {
            if (client.IsContractor)
            {
                return;
            }

            client.IsContractor = true;
            await ClientDataAccess.UpdateClientAsync(client);
            allClientsById[client.Id] = client;
            if (!string.IsNullOrWhiteSpace(client.Name))
            {
                var normalized = NormalizeNameKey(client.Name);
                if (!allClientNameMap.ContainsKey(normalized))
                {
                    allClientNameMap[normalized] = client;
                }
            }
        }

        private static string NormalizeHeader(string value)
        {
            return (value ?? string.Empty).Trim().Trim('\uFEFF').Replace(" ", string.Empty).ToLowerInvariant();
        }

        private static string NormalizeNameKey(string value)
        {
            var normalized = (value ?? string.Empty)
                .Trim()
                .Trim('\uFEFF')
                .Replace(" ", string.Empty)
                .Replace("　", string.Empty)
                .ToLowerInvariant();

            // 法人種別の表記ゆれを吸収
            var corporateTokens = new[]
            {
                "株式会社", "㈱", "(株)", "（株）",
                "有限会社", "㈲", "(有)", "（有）",
                "合同会社", "合名会社", "合資会社"
            };

            foreach (var token in corporateTokens)
            {
                normalized = normalized.Replace(token, string.Empty);
            }

            return normalized;
        }

        private static string NormalizeTextKey(string value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Trim('\uFEFF')
                .Replace(" ", string.Empty)
                .Replace("　", string.Empty)
                .ToLowerInvariant();
        }

        private static bool IsHeaderMatch(string normalized, params string[] candidates)
        {
            return candidates.Any(candidate => NormalizeHeader(candidate) == normalized);
        }

        private static bool ParseBoolean(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = value.Trim();
            if (bool.TryParse(normalized, out var boolValue))
            {
                return boolValue;
            }

            if (int.TryParse(normalized, out var intValue))
            {
                return intValue != 0;
            }

            return normalized is "1" or "○" or "〇" or "はい" or "有" or "true" or "TRUE";
        }

        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                    continue;
                }

                if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            result.Add(current.ToString().Trim());
            return result;
        }
    }
}
