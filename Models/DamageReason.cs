using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("DamageReasons", Schema = "doc")]
    public class DamageReason
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("damage_id")]
        [Index("IX_DamageId_DamageReasons", IsClustered = false)]
        public int DamageId { get; set; }

        [Column("reason_id")]
        [Index("IX_ReasonId_OrderDamageReasons", IsClustered = false)]
        public int ReasonId { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [ForeignKey("ReasonId")]
        public Reason Reason { get; set; }

        [ForeignKey("DamageId")]
        public CardDamage CardDamage { get; set; }
    }
}