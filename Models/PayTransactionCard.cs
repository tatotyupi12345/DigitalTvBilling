using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("doc.PayTransactionCards")]
    public class PayTransactionCard
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("transaction_id")]
        [Index("IX_PayTransactionId_TransactionCards", IsClustered = false)]
        public long PayTransactionId { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_TransactionCards", IsClustered = false)]
        public int CardId { get; set; }

        [ForeignKey("PayTransactionId")]
        PayTransaction PayTransaction { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }
    }
}