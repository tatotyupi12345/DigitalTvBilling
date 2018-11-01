using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("ReturnedCardAttachments", Schema = "dbo")]
    public class ReturnedCardAttachment
    {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            [Column("ID")]
            public int Id { get; set; }

            [Column("ReturnedCardsID")]
            public int ReturnedCardsID { get; set; }

            [Column("ReceiverAttachmentsID")]
            public int ReceiverAttachmentsID { get; set; }

            [ForeignKey("ReturnedCardsID")]
            public ReturnedCard ReturnedCard { get; set; }

            [ForeignKey("ReceiverAttachmentsID")]
            public ReceiverAttachment ReceiverAttachment { get; set; }

    }
}