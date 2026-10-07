using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// 水道料金請求情報を表すエンティティクラス
    /// </summary>
    public class WaterBilling
    {
        public int Id { get; set; }
        public string BillingYearMonth { get; set; } = string.Empty; // 受領請求年月 (YYYY-MM)
        public string BuildingName { get; set; } = string.Empty; // ビル名
        public int? ParentMeterId { get; set; } // 親メーターID (MetersテーブルへのFK)
        public decimal UsageAmount { get; set; } // 使用量
        public DateTime StartDate { get; set; } // 開始日
        public DateTime EndDate { get; set; } // 終了日
        public decimal BasicCharge { get; set; } // 基本料金
        public decimal UsageCharge { get; set; } // 使用料金
        public decimal TaxRate { get; set; } // 税率
        public string CustomerNumber { get; set; } = string.Empty; // お客様番号
        /// <summary>差額（請求使用量－子メーター合計）を割り当てる部屋名。未指定時は差額は割り当てない。</summary>
        public string DifferenceAssignmentRoomName { get; set; } = string.Empty;
        public int? ContractorId { get; set; } // 業者（Clients.Id）
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}


