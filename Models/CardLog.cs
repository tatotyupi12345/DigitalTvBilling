using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("CardLogs", Schema = "doc")]
    public class CardLog
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_CardLogs", IsClustered = false)]
        public int CardId { get; set; }

        [Column("close_tdate")]
        public DateTime Date { get; set; }

        [Column("status")]
        public CardLogStatus Status { get; set; }

        [Column("card_status")]
        public CardStatus CardStatus { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_CardLogs", IsClustered = false)]
        public int UserId { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }

    public enum CardLogStatus
    {
        [Description("გაიხსნა")]
        Open,
        [Description("გაითიშა")]
        Close,
        [Description("დამონტაჟდა")]
        Montage,
        [Description("დაპაუზდა")]
        Pause,
        [Description("პაუზა მოიხსნა")]
        ClosePause,
        [Description("გაუქმდა")]
        Cancel,
        [Description("დაიბლოკა")]
        Blocked,
        [Description("ბლოკი მოიხსნა")]
        CloseBlock,
        [Description("გაითიშა მომს. გად. გამო")]
        CloseToService,
        [Description("დაიბლოკა მომს. გად. გამო")]
        BlockToService,
        [Description("გაიცა კრედიტი")]
        Credit,
        [Description("დაიბლოკა დავალ. გად. გამო")]
        BlockToMinusBalance,
        [Description("დაზიანება დაფიქსირდა")]
        DamageFixed,
        [Description("დაზიანება შესრულდა")]
        DamageCompleted,
        [Description("დაზიანება გადამოწმდა")]
        DamageApproved,
        [Description("თანხა გაიცა")]
        MoneyMove,
        [Description("იჯარა გაიხსნა")]
        RentOpen,
        [Description("შეწყვეტა")]
        Discontinued
    }
}