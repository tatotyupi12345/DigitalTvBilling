﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("JuridicalLogging", Schema = "dbo")]
    public class JuridicalLogging
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [Column("tdate")]
        public DateTime tdate { get; set; }

        [Column("user_id")]
        public int user_id { get; set; }

        [Column("card_id")]
        public int card_id { get; set; }

        [Column("status")]
        public int status { get; set; }

        [Column("name")]
        public string name { get; set; }

        [ForeignKey("card_id")]
        public Card Card { get; set; }
    }
}