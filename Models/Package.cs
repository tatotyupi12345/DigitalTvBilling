using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Packages", Schema = "book")]
    public class Package
    {
        public Package()
        {
            this.MinPrice = 0;
        }

        [Key]
        [Column("id")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(255)]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        [DisplayName("დასახელება:")]
        public string Name { get; set; }

        [Column("price")]
        [DisplayName("ფასი:")]
        [Range(0, Int32.MaxValue, ErrorMessage = "შეივანეთ ფასი")]
        [Required(ErrorMessage = "შეიყვანეთ ფასი")]
        public double Price { get; set; }

        [Column("min_price")]
        [DisplayName("მინ. ფასი:")]
        [Range(0, Int32.MaxValue, ErrorMessage = "შეივანეთ მინ. ფასი")]
        [Required(ErrorMessage = "შეიყვანეთ მინ. ფასი")]
        public double MinPrice { get; set; }

        [Column("jurid_price")]
        [DisplayName("იურიდ. ფასი:")]
        [Range(0, Int32.MaxValue, ErrorMessage = "შეივანეთ იურიდ. ფასი")]
        [Required(ErrorMessage = "შეიყვანეთ იურიდ. ფასი")]
        public double JuridPrice { get; set; }

        [Column("cas_id")]
        [DisplayName("CAS ნომერი:")]
        [Required(ErrorMessage = "შეიყვანეთ CAS ნომერი")]
        public int CasId { get; set; }

        [Column("is_default")]
        [DisplayName("საწყისი პაკეტი:")]
        public bool IsDefault { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Packages", IsClustered = false)]
        public int UserId { get; set; }

        [Column("rent_type")]
        [DisplayName("ტიპი:")]
        [Required(ErrorMessage = "შეიყვანეთ ტიპი")]
        public RentType RentType { get; set; }

        //[Column("is_group")]
        //[DisplayName("ჯგუფი:")]
        //public bool is_group { get; set; }

        //[Column("binded_pack_ids")]
        //[DisplayName("მიბმული პაკეტები:")]
        //public string bind_package_ids { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        //[NotMapped]
        //public List<BindedPackages> BindedPackages { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        public virtual ICollection<PackageChannel> PackageChannels { get; set; }

        public virtual ICollection<SubscriptionPackage> SubscriptionPackages { get; set; }
    }

    public class BindedPackages
    {
        public bool Sign { get; set; }
        public int ID { get; set; }
    }

    public enum RentType
    {
        [Description("გაყიდვა")]
        [Display(Name = "გაყიდვა")]
        buy,
        [Description("გაქირავება")]
        [Display(Name = "გაქირავება")]
        rent,
        [Description("ტექნიკური")]
        [Display(Name = "ტექნიკური")]
        technic,
        [Description("ბლოკი")]
        [Display(Name = "ბლოკი")]
        block,
        [Description("აქცია")]
        [Display(Name = "აქცია")]
        share
    }
}