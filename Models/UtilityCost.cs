using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// 水道光熱費情報を表すエンティティクラス
    /// </summary>
    public class UtilityCost
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public DateTime RecordDate { get; set; } // 記録日
        public decimal WaterCost { get; set; } // 水道代
        public decimal ElectricityCost { get; set; } // 電気代
        public decimal GasCost { get; set; } // ガス代
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

