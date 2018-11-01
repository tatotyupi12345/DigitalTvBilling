using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Towers", Schema = "book")]
    public class Tower
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }

        [Column("range")]
        [DisplayName("სიხშირე:")]
        public string Range { get; set; }

        [Column("towerLat")]
        [DisplayName("ანძა Lat:")]
        public double towerLat { get; set; }

        [Column("towerLon")]
        [DisplayName("ანძა Lon:")]
        public double towerLon { get; set; }

        [NotMapped]
        public List<string> Ranges { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}