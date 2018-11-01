using DigitalTVBilling.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("CustomerSellAttachments", Schema = "dbo")]
    public class CustomerSellAttachments
    {
        public CustomerSellAttachments()
        {
            this.Tdate = DateTime.Now;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("tdate")]
        [JsonIgnore]
        public DateTime Tdate { get; set; }

        [Column("attachment_id")]
        public int AttachmentID { get; set; }

        [Column("customer_id")]
        public int CustomerID { get; set; }

        [Column("count")]
        public int Count { get; set; }

        [Column("info")]
        public string Info { get; set; }

        [Column("diler_id")]
        public int Diler_Id { get; set; }

        [Column("verify_status")]
        public short VerifyStatus { get; set; }

        [Column("status")]
        public SellAttachmentStatus status { get; set; }

        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        [ForeignKey("AttachmentID")]
        public SellAttachment Attachment { get; set; }
    }
    public enum SellAttachmentStatus
    {
        [Description("გაყიდვა")]
        [Display(Name = "გაყიდვა")]
        sell,
        [Description("დროებით სარგებლობაში")]
        [Display(Name = "დროებით სარგებლობაში")]
        temporary_use,
    }
}