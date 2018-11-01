using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("doc.AutoSubscribChangeCards")]
    public class AutoSubscribChangeCard
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_AutoSubscribChangeCards", IsClustered = false)]
        public int CardId { get; set; }

        [Column("cas_ids")]
        public string CasIds { get; set; }

        [Column("package_ids")]
        public string PackageIds { get; set; }

        [Column("package_names")]
        public string PackageNames { get; set; }

        [Column("amount")]
        public double Amount { get; set; }

        [Column("send_date")]
        public DateTime? SendDate { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_AutoSubscribChangeCards", IsClustered = false)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }
    }
}