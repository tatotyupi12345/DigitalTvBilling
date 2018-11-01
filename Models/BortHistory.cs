using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("BortHistory", Schema = "dbo")]
    public class BortHistory
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("tdate")]
        [DisplayName("თარიღი:")]
        public DateTime Tdate { get; set; }

        [Column("ReturnedCardID")]
        public int ReturnedCardID { get; set; }

        [Column("BortID")]
        public int BortID { get; set; }

        [Column("Status")]
        public int Status { get; set; }

        [Column("Info")]
        public string Info { get; set; }

    }
}