using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("SellAttachmentsLogging", Schema = "dbo")]
    public class SellAttachmentsLogging
    {
        

            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            [Column("id")]
            public int Id { get; set; }

            [Column("sellattachment_id")]
            public int SellattachmentId { get; set; }

            [Column("sellattachment_name")]
            public string SellattachmentName { get; set; }

            [Column("tdate")]
            public DateTime Tdate { get; set; }

            [Column("user_id")]
            public int user_id { get; set; }

            [Column("price")]
            public int Price { get; set; }


    }
}