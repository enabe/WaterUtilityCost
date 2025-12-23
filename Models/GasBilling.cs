using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// ガス料金請求情報を表すエンティティクラス
    /// </summary>
    public class GasBilling
    {
        public int Id { get; set; }
        public string BillingYearMonth { get; set; } = string.Empty; // 受領請求年月 (YYYY-MM)
        public string BuildingName { get; set; } = string.Empty; // ビル名
        public decimal UsageAmount { get; set; } // 使用量
        public DateTime StartDate { get; set; } // 開始日
        public DateTime EndDate { get; set; } // 終了日
        public decimal BasicCharge { get; set; } // 基本料金
        public decimal UsageCharge { get; set; } // 使用料金
        public decimal TaxRate { get; set; } // 税率
        public string CustomerNumber { get; set; } = string.Empty; // お客様番号
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}



