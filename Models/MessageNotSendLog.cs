using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("MessageNotSendLogs", Schema = "book")]
    public class MessageNotSendLog
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("message_type")]
        public MessageType MessageType { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_MessageNotSendLogs", IsClustered = false)]
        public int CardId { get; set; }

        [Column("message_id")]
        [Index("IX_MessageId_MessageNotSendLogs", IsClustered = false)]
        public long MessageId { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

        [ForeignKey("MessageId")]
        public Message Message { get; set; }
    }
}