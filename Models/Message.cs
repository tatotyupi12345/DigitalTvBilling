using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Messages", Schema = "book")]
    public class Message
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("tdate")]
        public DateTime Date { get; set; }

        [Column("message")]
        [DisplayName("შეტყობინება:")]
        [Required(ErrorMessage = "შეტყობინება ცარიელია")]
        public string MessageText { get; set; }

        [Column("message_type")]
        [DisplayName("შეტყობინების ტიპი:")]
        public string MessageType { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Messages", IsClustered = false)]
        public int UserId { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [NotMapped]
        public List<int> AbonentIds { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        [NotMapped]
        public string TemplateType { get; set; }

        [NotMapped]
        public List<MessageTemplate> Templates { get; set; }

        public ICollection<MessageAbonent> MessageAbonents { get; set; }
    }

    public enum MessageType
    {
        OSD,
        Email,
        SMS
    }
}