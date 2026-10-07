using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// ビル情報を表すエンティティクラス
    /// </summary>
    public class Building
    {
        public int Id { get; set; }
        public string BuildingId { get; set; } = string.Empty; // ビルID
        public string Name { get; set; }
        public string Address { get; set; }
        public int Floors { get; set; } // 階数
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

