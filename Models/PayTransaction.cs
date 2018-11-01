using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("doc.PayTransactions")]
    public class PayTransaction
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("transaction_id")]
        [Index("IX_TransactionId_PayTransactions", IsClustered = false, IsUnique = true)]
        public string TransactionId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        public ICollection<PayTransactionCard> PayTransactionCards { get; set; }
    }
}