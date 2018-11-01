using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("CardCharges", Schema = "doc")]
    public class CardCharge
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_CardCharges", IsClustered = false)]
        public int CardId { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("rent_amount")]
        public decimal RentAmount { get; set; }

        [Column("status")]
        public CardChargeStatus Status { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

    }

    public enum CardChargeStatus
    {
        [Description("დღიური დარიცხვა")]
        Daily,
        [Description("პაუზის დარიხხვა")]
        Pause,
        [Description("ჯარიმის დარიცხვა")]
        Pen,
        [Description("იჯარის დღიური დარიცხვა")]
        PenDaily,
        [Description("გაწეული მომსახურების დარიცხვა")]
        Service,
        [Description("თანხის დაბრუნება")]
        ReturnMoney,
        [Description("პაკეტის შეცვლა")]
        PacketChange,
        [Description("წინასწარი დარიცხვა")]
        PreChange,
        [Description("დაბრუნების საკომისიო")]
        ReturnComm,
        [Description("აქსესუარების ჯარიმა")]
        AccessoryCharge
    }
}