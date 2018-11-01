using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class ReceiverSubscribtion
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("CustomerID")]
        public int CustomerID { get; set; }

        [Column("ReceiverNumber")]
        [DisplayName("სერიული ნომერი:")]
        [Required(ErrorMessage = "შეიყვანეთ ნომერი")]
        public string ReceiverNumber { get; set; }
    }
}