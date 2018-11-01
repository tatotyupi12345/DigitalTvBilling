using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalTVBilling.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalTVBilling.Models
{
    [Table("Cities", Schema = "dbo")]
    public class City
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Name")]
        [StringLength(255)]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }

        [Column("Info")]
        [DisplayName("ინფო:")]
        public string Info { get; set; }

        [NotMapped]
        public string Logging { get; set; }
    }
}