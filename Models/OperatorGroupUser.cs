using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("OperatorGroupUser", Schema = "dbo")]
    public class OperatorGroupUser
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [Column("d_id")]
        public int d_id { get; set; }

        [Column("name")]
        public string name { get; set; }
    }
}