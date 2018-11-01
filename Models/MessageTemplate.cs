using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("MessageTemplates", Schema = "book")]
    public class MessageTemplate
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }

        [Column("desc")]
        [DisplayName("აღწერა:")]
        [Required(ErrorMessage = "შეიყვანეთ აღწერა")]
        public string Desc { get; set; }

        [Column("message_status")]
        [DisplayName("სტატუსი:")]
        public MessageStatus message_status { get; set; }

    }
    public enum MessageStatus
    {
        [Description("სტანდარტული")]
        [Display(Name = "სტანდარტული")]
        Standard = 0,
        [Description("პრობლემური")]
        [Display(Name = "პრობლემური")]
        Problematic,
    }
}