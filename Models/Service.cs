using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Service", Schema = "book")]
    public class Service
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }

        [Column("amount")]
        [DisplayName("თანხა:")]
        [Required(ErrorMessage = "შეივანეთ თანხა")]
        [Range(0, Int32.MaxValue, ErrorMessage = "შეივანეთ თანხა")]
        public decimal Amount { get; set; }

        [Column("is_edit")]
        [DisplayName("რედაქტირება:")]
        public bool IsEdit { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        public ICollection<CardService> CardServices { get; set; }
    }
}