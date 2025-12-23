using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// 契約情報を表すエンティティクラス
    /// </summary>
    public class Contract
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty; // 契約番号
        public string ContractType { get; set; } = string.Empty; // 契約種別
        public string ContractorName { get; set; } = string.Empty; // 契約者名
        public int? LessorClientId { get; set; } // 貸主取引先ID
        public int? LesseeClientId { get; set; } // 借主取引先ID
        public int? BillingClientId { get; set; } // 請求取引先ID
        public DateTime? StartDate { get; set; } // 対象開始日
        public DateTime? EndDate { get; set; } // 対象終了日
        public string ContractStatus { get; set; } = string.Empty; // 契約状況
        public int? ClosingDate { get; set; } // 締日（1-31）
        public int? BuildingId { get; set; } // ビルID
        public string CustomerNumber { get; set; } = string.Empty; // お客様番号
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}


