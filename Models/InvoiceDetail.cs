using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// 請求明細情報を表すエンティティクラス
    /// </summary>
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public string BillingTo { get; set; } = string.Empty; // 請求先
        public string Lessor { get; set; } = string.Empty; // 貸主
        public string BuildingName { get; set; } = string.Empty; // 建物名称
        public string Lessee { get; set; } = string.Empty; // 借主
        public string RoomNumber { get; set; } = string.Empty; // 部屋番号
        public decimal? RoomArea { get; set; } // 面積
        public string Category { get; set; } = string.Empty; // 種別
        public string Content { get; set; } = string.Empty; // 内容
        public decimal? ChildMeterUsage { get; set; } // 子メーター使用量
        public decimal? UsageAmount { get; set; } // 使用量
        public string Unit { get; set; } = string.Empty; // 単位
        public decimal TaxInclusiveAmount { get; set; } // 税込金額
        public decimal TaxRate { get; set; } // 税率
        public string Contractor { get; set; } = string.Empty; // 業者
        public string InvoiceNumber { get; set; } = string.Empty; // インボイス番号
        public DateTime? ChildMeterStartDate { get; set; } // 子メータ使用開始日
        public DateTime? ChildMeterEndDate { get; set; } // 子メータ使用終了日
        public DateTime? ParentMeterStartDate { get; set; } // 親メータ使用開始日
        public DateTime? ParentMeterEndDate { get; set; } // 親メータ使用終了日
        public DateTime? ConfirmedBillingDate { get; set; } // 請求予定日
        public string BillingYearMonth { get; set; } = string.Empty; // 請求年月（受領請求年月 YYYY-MM）
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}


