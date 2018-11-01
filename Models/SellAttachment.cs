using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("SellAttachments", Schema = "dbo")]
    public class SellAttachment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("name")]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }

        [Column("value")]
        [DisplayName("რაოდენობა:")]
        [Required(ErrorMessage = "შეიყვანეთ რაოდენობა")]
        public int Value { get; set; }

        [Column("type")]
        public int? Type { get; set; }

        [Column("image_path")]
        [DisplayName("სურათი:")]
        public string ImagePath { get; set; }

        [Column("price")]
        public int Price { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        [NotMapped]
        public HttpPostedFileBase Picture { get; set; }
    }
}