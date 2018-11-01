using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("MessageAbonents", Schema = "book")]
    public class MessageAbonent
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_MessageAbonents", IsClustered = false)]
        public int CardId { get; set; }

        [Column("message_id")]
        [Index("IX_MessageId_MessageAbonents", IsClustered = false)]
        public long MessageId { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

        [ForeignKey("MessageId")]
        public Message Message { get; set; }
    }
}