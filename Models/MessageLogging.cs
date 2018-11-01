using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("MessageLogging", Schema = "dbo")]
    public class MessageLogging
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [Column("card_id")]
        public int card_id { get; set; }

        [Column("tdate")]
        public DateTime tdate { get; set; }

        [Column("status")]
        public MessageLoggingStatus status { get; set; }

        [Column("message_id")]
        public int message_id { get; set; }

    }
    public enum MessageLoggingStatus
    {
        [Description("2-დღით ადრე გაფთხილება")]
        [Display(Name = "2-დღით ადრე გაფთხილება")]
        TwoPreWarn = 0,

        [Description("ავტომატური პაკეტები")]
        [Display(Name = "ავტომატური პაკეტები")]
        AutoSubscrib,

        [Description("1-დღით ადრე გაფთხილება")]
        [Display(Name = "1-დღით ადრე გაფთხილება")]
        OnePreWarn,

        [Description("8-₾-გათიშვა")]
        [Display(Name = "8-₾-გათიშვა")]
        OnShare8Disabling,

        [Description("8-₾-აქტივაცია")]
        [Display(Name = "8-₾-აქტივაცია")]
        OnShare8Active,

        [Description("გათიშვა ")]
        [Display(Name = "გათიშვა")]
        ReportDisabling,

        [Description("1 დღით ადრე უფასო ინსტალაცია ")]
        [Display(Name = "1 დღით ადრე უფასო ინსტალაცია")]
        FreeInstalation,

        [Description("პრომო პაკეტის ცვლილება აქტიური 8 გადასვლა")]
        [Display(Name = "პრომო პაკეტის ცვლილება აქტიური 8 გადასვლა")]
        PromoCahngeActive8,

        [Description("პრომო პაკეტის ცვლილება გათიშული 8 გადასვლა")]
        [Display(Name = "პრომო პაკეტის ცვლილება გათიშული 8 გადასვლა")]
        PromoCahngeClosed8
    }
}