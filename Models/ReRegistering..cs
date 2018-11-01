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
    [Table("ReRegistering", Schema = "dbo")]
    public class ReRegistering
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [JsonIgnore]
        public int id { get; set; }

        [Column("name")]
        [StringLength(100)]
        [DisplayName("სახელი:")]
        public string name { get; set; }

        [Column("lastname")]
        [StringLength(100)]
        [DisplayName("გვარი:")]
        public string lastname { get; set; }

        [Column("code", TypeName = "varchar")]
        [StringLength(11)]
        [DisplayName("პ/ნ:")]
        public string code { get; set; }

        [Column("tdate")]
        [JsonIgnore]
        public DateTime tdate { get; set; }

        [Column("phone", TypeName = "varchar")]
        [StringLength(50)]
        [DisplayName("ტელეფონი 1:")]
        [MaxLength(9, ErrorMessage = "შეიყვანეთ 9 ნიშნა რიცხვი")]
        [MinLength(9, ErrorMessage = "შეიყვანეთ 9 ნიშნა რიცხვი")]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "ტელეფონი არასწორია")]
        public string phone { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Customers", IsClustered = false)]
        [JsonIgnore]
        public int user_id { get; set; }

        [Column("card_id")]
        [Index("IX_UserId_Customers", IsClustered = false)]
        [JsonIgnore]
        public int card_id { get; set; }
    }
}