using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("ChargeCrushLogs", Schema = "config")]
    public class ChargeCrushLog
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        public DateTime Date { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("card_num")]
        public string CardNum { get; set; }

        [Column("type")]
        public ChargeCrushLogType ChargeCrushLogType { get; set; }
    }

    public enum ChargeCrushLogType
    {
        [Description("დაბლოკვა")]
        Block,
        [Description("გახსნა")]
        Open,
        [Description("არხის გათიშვა")]
        EntitlementClose,
        [Description("არხის ჩართვა")]
        EntitlementOpen,
        [Description("სისტემური შეცდომა")]
        Crush,
        [Description("ინვიოსის გაგზავნა")]
        InvoiceNotSend
    }

}