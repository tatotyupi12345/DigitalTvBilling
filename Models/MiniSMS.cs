using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("MiniSMS", Schema="dbo")]
    public class MiniSMS
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [Column("tdate")]
        public DateTime tdate { get; set; }

        [Column("card_num")]
        public int card_num { get; set; }

        [Column("card_id")]
        public int card_id { get; set; }

        [Column("cas_ids")]
        public string cas_ids { get; set; }

        [Column("cas_date")]
        public DateTime cas_date { get; set; }

        [Column("finish_date")]
        public DateTime finish_date { get; set; }

        [Column("status")]
        public int status { get; set; }

        [Column("mini_sms")]
        public int mini_sms { get; set; }

        [Column("send_status")]
        public int send_status { get; set; }
    }
}