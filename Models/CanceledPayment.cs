using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("CanceledPayments", Schema = "doc")]
    public class CanceledPayment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_Payments", IsClustered = false)]
        public int CardId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }
    }
}