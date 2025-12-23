using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// 子メーター検針情報を表すエンティティクラス
    /// </summary>
    public class ChildMeterReading
    {
        public int Id { get; set; }
        public int FloorId { get; set; } // フロアID (FloorsテーブルへのFK)
        public int? ChildMeterId { get; set; } // 子メーターID (ChildMetersテーブルへのFK)
        public DateTime ReadingDate { get; set; } // 検針日
        public string Type { get; set; } = string.Empty; // 種別（電気、低圧電力、水道、ガス）
        public decimal MeterValue { get; set; } // メーター値
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

