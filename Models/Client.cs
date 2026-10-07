using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// 取引先情報を表すエンティティクラス
    /// </summary>
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // 取引先名
        public bool IsLessor { get; set; } // 貸主
        public bool IsLessee { get; set; } // 借主
        public bool IsBillingTo { get; set; } // 請求先
        public bool IsContractor { get; set; } // 業者
        public bool IsAutoTransfer { get; set; } // 自動振込
        public bool IsBankTransfer { get; set; } // 口座振込
        public string InvoiceNumber { get; set; } = string.Empty; // インボイス番号
        public string BuildingName { get; set; } = string.Empty; // ビル名
        public string RoomName { get; set; } = string.Empty; // 部屋名
        public string PostalCode { get; set; } = string.Empty; // 郵便番号
        public string Address { get; set; } = string.Empty; // 住所
        public string Phone { get; set; } = string.Empty; // 電話番号
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

