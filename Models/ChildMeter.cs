using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// 子メーター情報を表すエンティティクラス
    /// </summary>
    public class ChildMeter
    {
        public int Id { get; set; }
        public int? ParentMeterId { get; set; } // 親メーターID (MetersテーブルへのFK) - 後方互換性のため残す
        public int? BuildingId { get; set; } // ビルID (BuildingsテーブルへのFK)
        public int? RoomId { get; set; } // 部屋ID (FloorsテーブルへのFK)
        public string MeterType { get; set; } = string.Empty; // メーター種別（電気、ガス、水道）
        public string Notes { get; set; } = string.Empty; // 備考
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}



