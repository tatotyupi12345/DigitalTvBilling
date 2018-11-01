using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("InvoiceLogging", Schema = "dbo")]
    public class InvoceLogging
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("tdate")]
        public DateTime tdate { get; set; }

        [Column("custumer_id")]
        public int custumer_id { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("invoce_code")]
        public int invoce_code { get; set; }

        [Column("send_email")]
        public bool send_email { get; set; }

        [Column("Confirmation")]
        public int confirmation { get; set; }

        [Column("send_sms")]
        public bool send_sms { get; set; }

    }

    public enum InvoceType
    {
        [Description("მაილის გაგზავნა წარმატებით")]
        Send_Yes,
        [Description("მაილის გაგზავნა ვერ მოხერხდა")]
        Send_No
    }
}