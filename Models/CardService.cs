using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("CardServices", Schema = "doc")]
    public class CardService
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_Cards", IsClustered = false)]
        public int CardId { get; set; }

        [Column("service_id")]
        [Index("IX_ServiceId_Cards", IsClustered = false)]
        public int ServiceId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("pay_type")]
        public CardServicePayType PayType { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("tdate")]
        public DateTime Date { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; } 
    }

    public enum CardServicePayType
    {
        [Description("ნაღდი")]
        Cash,
        [Description("უნაღდო")]
        NotCash
    }
}