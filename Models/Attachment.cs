using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Attachments", Schema = "book")]
    public class Attachment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Column("original_name")]
        [StringLength(35)]
        public string OriginalName { get; set; }

        [Column("payment_id")]
        public long PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
    }
}