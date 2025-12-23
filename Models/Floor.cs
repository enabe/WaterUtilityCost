using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// フロア情報を表すエンティティクラス
    /// </summary>
    public class Floor
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string FloorName { get; set; }
        public decimal FloorArea { get; set; } // フロア面積（㎡）
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}








