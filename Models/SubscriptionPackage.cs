using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("SubscriptionPackages", Schema = "doc")]
    public class SubscriptionPackage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("subscription_id")]
        [Index("IX_SubscriptionId_SubscriptionPackages", IsClustered = false)]
        public int SubscriptionId { get; set; }

        [Column("package_id")]
        public int PackageId { get; set; }

        [ForeignKey("SubscriptionId")]
        public Subscribtion Subscribtion { get; set; }

        [ForeignKey("PackageId")]
        public Package Package { get; set; }

    }
}