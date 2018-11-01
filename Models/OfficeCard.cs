using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("OfficeCards", Schema = "book")]
    public class OfficeCard
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("card_num", TypeName = "varchar")]
        [StringLength(255)]
        [DisplayName("ბარათის №:")]
        [Required(ErrorMessage = "შეიყვანეთ ბარათის №")]
        [Range(0, Int32.MaxValue, ErrorMessage = "ბარათის № რიცხვი უნდა იყოს")]
        public string CardNum { get; set; }

        [Column("address")]
        [StringLength(255)]
        [DisplayName("მისამართი:")]
        [Required(ErrorMessage = "შეიყვანეთ მისამართი")]
        public string Address { get; set; }

        [Column("name")]
        [StringLength(100)]
        [Required(ErrorMessage = "შეიყვანეთ სახელი, გვარი")]
        [DisplayName("სახელი, გვარი:")]
        public string Name { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        [NotMapped]
        public List<int> Packages { get; set; }

    }
}