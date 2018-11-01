using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("PackageChannels", Schema = "book")]
    public class PackageChannel
    {
        [Key]
        [Column("id")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("package_id")]
        [Index("IX_PackageId_PackageChannels", IsClustered = false)]
        public int PackageId { get; set; }

        [Column("channel_id")]
        [Index("IX_ChannelId_PackageChannels", IsClustered = false)]
        public int ChannelId { get; set; }

        [ForeignKey("PackageId")]
        public Package Package { get; set; }

        [ForeignKey("ChannelId")]
        public Channel Channel { get; set; }
    }
}