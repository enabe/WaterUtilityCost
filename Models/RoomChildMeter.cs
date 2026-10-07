using System;

namespace WaterUtilityCost.Models
{
    /// <summary>
    /// 部屋別子メーター情報を表すエンティティクラス
    /// </summary>
    public class RoomChildMeter
    {
        public int Id { get; set; }
        public int FloorId { get; set; } // 部屋ID (FloorsテーブルへのFK)
        public int ChildMeterId { get; set; } // 子メーターID (ChildMetersテーブルへのFK)
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}







