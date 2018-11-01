using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("ReceiverAttachments", Schema = "dbo")]
    public class ReceiverAttachment
    {
        
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }
            
        [Column("Image_path")]
        [DisplayName("სურათი:")]
        public string ImagePath { get; set; }

        [Column("Info")]
        public string Info { get; set; }

        [Column("Price")]
        [DisplayName("ღირებულება:")]
        public double Price { get; set; }

        //[NotMapped]
        //public string Logging { get; set; }

        [NotMapped]
        public HttpPostedFileBase Picture { get; set; }


    }
}