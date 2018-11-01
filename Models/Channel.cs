using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Channels", Schema = "book")]
    public class Channel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(255)]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Channels", IsClustered = false)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public virtual ICollection<PackageChannel> PackageChannels { get; set; }
    }
}