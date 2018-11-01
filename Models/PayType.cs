using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("PayTypes", Schema = "book")]
    public class PayType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(255)]
        [DisplayName("დასახელება:")]
        public string Name { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}