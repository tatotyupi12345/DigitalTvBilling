using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Users", Schema="book")]
    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("login")]
        [StringLength(18)]
        [DisplayName("შესვლის სახელი:")]
        [Required(ErrorMessage = "შეიყვანეთ ლოგინი")]
        [MinLength(6, ErrorMessage = "შესვლის სახელი უნდა შეიცავდეს მინიმუმ 6 სიმბოლოს!")]
        //[StringLength(18, ErrorMessage = "კოდური სიტყვა უნდა შეიცავდეს მინიმუმ 6 სიმბოლოს", MinimumLength = 6)]
        //[RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "შესვლის სახელი უნდა შეიცავდეს ციფრს, მაღალ და დაბალ რეგისტრიან სიმბოლოებს!")]
        public string Login { get; set; }

        [Column("password", TypeName = "varchar")]
        [StringLength(18)]
        [DataType(DataType.Password)]
        [DisplayName("პაროლი:")]
        [MinLength(6, ErrorMessage = "პაროლი უნდა შეიცავდეს მინიმუმ 6 სიმბოლოს!")]
        //[StringLength(18, ErrorMessage = "კოდური სიტყვა უნდა შეიცავდეს მინიმუმ 6 სიმბოლოს", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "პაროლი უნდა შეიცავდეს ციფრს, მაღალ და დაბალ რეგისტრიან სიმბოლოებს!")]
        public string Password { get; set; }

        [Column("name")]
        [StringLength(100)]
        [DisplayName("სახელი:")]
        [Required(ErrorMessage = "შეიყვანეთ სახელი")]
        public string Name { get; set; }

        [Column("phone")]
        [DisplayName("ტელეფონი:")]
        [MaxLength(9, ErrorMessage = "შეიყვანეთ 9 ნიშნა რიცხვი")]
        [MinLength(9, ErrorMessage = "შეიყვანეთ 9 ნიშნა რიცხვი")]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "ტელეფონი არასწორია")]
        [Required(ErrorMessage = "შეიყვანეთ ტელეფონი")]
        public string Phone { get; set; }

        [Column("email")]
        [StringLength(100)]
        [DisplayName("ელ-ფოსტა:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "ველი უნდა იყოს ელ-ფოსტა")]
        [Required(ErrorMessage = "შეიყვანეთ ელ-ფოსტა")]
        public string Email { get; set; }

        [Column("type")]
        [DisplayName("ტიპი:")]
        [NoLog]
        public int Type { get; set; }

        [Column("hard_autorize")]
        [DisplayName("რთული ავტორიზაცია:")]
        public bool HardAutorize { get; set; }

        [Column("start_end")]
        public int start_end { get; set; }

        [ForeignKey("Type")]
        public UserType UserType { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        [NotMapped]
        [DisplayName("ტიპი")]
        public string TypeName { get; set; }

        [Column("object")]
        [DisplayName("ობიექტი:")]
        public int? @object { get; set; }

        [ForeignKey("object")]
        public SellerObject SellerObj { get; set; }

        [Column("code_word")]
        [DisplayName("კოდური სიტყვა:")]
        [StringLength(18, ErrorMessage = "კოდური სიტყვა უნდა შეიცავდეს მინიმუმ 6 სიმბოლოს", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "კოდური სიტყვა უნდა შეიცავდეს ციფრს, მაღალ და დაბალ რეგისტრიან სიმბოლოებს!")]
        [Required(ErrorMessage = "შეიყვანეთ კოდური სიტყვა")]
        public string CodeWord { get; set; }

        [Column("image")]
        [DisplayName("სურათი:")]
        public string image { get; set; }

        [NotMapped]
        public HttpPostedFileBase Picture { get; set; }

        public virtual ICollection<Logging> Loggings { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }

        public virtual ICollection<Card> Cards { get; set; }

        public virtual ICollection<Package> Packages { get; set; }

        public virtual ICollection<Channel> Channels { get; set; }

        public virtual ICollection<Subscribtion> Subscribes { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<Order> UserOrders { get; set; }

        public virtual ICollection<Order> GroupOrders { get; set; }
    }
}