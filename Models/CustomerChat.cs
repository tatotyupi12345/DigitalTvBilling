using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("doc.CustomersChat")]
    public class CustomerChat
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("message")]
        public string MessageText { get; set; }

        [Column("customer_id")]
        [Index("IX_CustomerId_CustomersChat", IsClustered = false)]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_CustomersChat", IsClustered = false)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}