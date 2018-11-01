using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using DigitalTVBilling.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalTVBilling.Models
{
    [Table("SellerObject", Schema = "dbo")]
    public class SellerObject
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Type")]
        [DisplayName("ტიპი:")]
        [Required(ErrorMessage = "მიუთითეთ ტიპი")]
        public SellerType type { get; set; }

        [Column("Name")]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string name { get; set; }

        [Column("City")]
        [DisplayName("სოფელი/ქალაქი:")]
        [Required(ErrorMessage = "შეიყვანეთ სოფელი/ქალაქი")]
        public string city { get; set; }

        [Column("Address")]
        [DisplayName("მისამართი:")]
        [Required(ErrorMessage = "შეიყვანეთ მისამართი")]
        public string address { get; set; }

        [Column("Region")]
        [DisplayName("რეგიონი:")]
        [Required(ErrorMessage = "რეგიონი")]
        public string region { get; set; }

        [Column("Info")]
        [DisplayName("ინფო:")]
        public string info { get; set; }

        [Column("IdentCode")]
        [DisplayName("საიდენტ. კოდი:")]
        public string ident_code { get; set; }

        [Column("Hostname")]
        [DisplayName("პასუხისმგებელი პირი:")]
        public string hostname { get; set; }

        [Column("Phone")]
        [DisplayName("მენეჯერის ტელ:")]
        [StringLength(50)]
        //[Required(ErrorMessage = "შეიყვანეთ ტელეფონი")]
        [MaxLength(9, ErrorMessage = "შეიყვანეთ 9 ნიშნა რიცხვი")]
        [MinLength(9, ErrorMessage = "შეიყვანეთ 9 ნიშნა რიცხვი")]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "ტელეფონი არასწორია")]
        public string phone { get; set; }
    }

    public enum SellerType
    {
        [Description("შ.პ.ს")]
        [Display(Name = "შ.პ.ს")]
        Ltd,

        [Description("ინდ. მეწარმე")]
        [Display(Name = "ინდ. მეწარმე")]
        hedger
    }
}