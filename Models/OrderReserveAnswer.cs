using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("OrderReserveAnswer", Schema = "dbo")]
    public class OrderReserveAnswer
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [Column("tdate")]
        public DateTime tdate { get; set; }

        [Column("order_id")]
        public int order_id { get; set; }

        [Column("reserve_answer")]
        public OrderCommitStatic reserve_answer { get; set; }

        [Column("user_id")]
        public int user_id { get; set; }
    }
}