using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Subscribes", Schema = "doc")]
    public class Subscribtion
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_Subscribes", IsClustered = false)]
        public int CardId { get; set; }

        [Column("amount")]
        public double Amount { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

        public ICollection<SubscriptionPackage> SubscriptionPackages { get; set; }
    }
}