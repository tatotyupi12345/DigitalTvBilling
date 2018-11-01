using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Params", Schema = "config")]
    public class Param
    {
        [Key]
        [Column("name")]
        public string Name { get; set; }

        [Column("value")]
        [Required(ErrorMessage = "ველი ცარიელია")]
        public string Value { get; set; }

        [Column("desc")]
        public string Desc { get; set; }

        [Column("help")]
        public string Help { get; set; }
    }
}