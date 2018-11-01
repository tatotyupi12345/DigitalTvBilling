using DigitalTVBilling.ListModels;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Models
{
    [Table("Payments", Schema = "doc")]
    public class Payment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("tdate")]
        [DisplayName("თარიღი:")]
        public DateTime Tdate { get; set; }

        [Column("card_id")]
        [DisplayName("ბარათის №:")]
        [Index("IX_CardId_Payments", IsClustered = false)]
        [NoLog]
        public int CardId { get; set; }

        [Column("pay_type_id")]
        [DisplayName("გადახდის სახეობა:")]
        [NoLog]
        public int PayTypeId { get; set; }

        [Column("amount")]
        [DisplayName("თანხა:")]
        public decimal Amount { get; set; }

        [Column("pay_transaction")]
        [NoLog]
        public long PayTransaction { get; set; }

        [Column("file_attach")]
        [NoLog]
        public string FileAttach { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Payments", IsClustered = false)]
        public int UserId { get; set; }

        [Column("pay_rent")]
        [DisplayName("იჯარა:")]
        [NoLog]
        public decimal PayRent { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

        [ForeignKey("PayTypeId")]
        public PayType PayType { get; set; }

        [NotMapped]
        public List<IdName> Cards { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        [NotMapped]
        public string LogCard { get; set; }

        [NotMapped]
        [DisplayName("გადახდის სახეობა")]
        public string LogPayType { get; set; }

        [NotMapped]
        [DisplayName("ბარათის №")]
        public string LogCardNum { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }

    }
}