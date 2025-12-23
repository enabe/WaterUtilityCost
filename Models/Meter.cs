using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// メーター情報を表すエンティティクラス
    /// </summary>
    public class Meter
    {
        public int Id { get; set; }
        public int? BuildingId { get; set; } // ビルID (BuildingsテーブルへのFK)
        public int? ContractorId { get; set; } // 業者ID (ClientsテーブルへのFK、IsContractor=trueの取引先)
        public string MeterId { get; set; } = string.Empty; // メーターID
        public string MeterType { get; set; } = string.Empty; // メーター種別
        public string ManagementNumber { get; set; } = string.Empty; // 管理番号
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

















