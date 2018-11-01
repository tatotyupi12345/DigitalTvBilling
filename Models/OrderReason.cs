using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("OrderReasons", Schema = "doc")]
    public class OrderReason
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("order_id")]
        [Index("IX_OrderId_OrderReasons", IsClustered = false)]
        public int OrderId { get; set; }

        [Column("reason_id")]
        [Index("IX_ReasonId_OrderDamageReasons", IsClustered = false)]
        public int ReasonId { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [ForeignKey("ReasonId")]
        public Reason Reason { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}