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
    [Table("Reasons", Schema = "book")]
    public class Reason
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("text")]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }

        [Column("type")]
        [DisplayName("ტიპი:")]
        [Required(ErrorMessage = "აირჩიეთ ტიპი")]
        public ReasonType ReasonType { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        [NotMapped]
        public List<SelectListItem> GetReasonTypes
        {
            get
            {
                return (from ReasonType n in Enum.GetValues(typeof(ReasonType))
                        select new SelectListItem { Value = n.ToString(), Text = Utils.Utils.GetEnumDescription(n) }).ToList();
            }
        }
    }

    public enum ReasonType
    {
        [Description("დაზიანება")]
        Damage,
        [Description("გადადება")]
        Postpone,
        [Description("გაუქმება")]
        Cancel,
        [Description("ლოდინი")]
        Loading,
    }
}