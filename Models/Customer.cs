using Newtonsoft.Json;
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
    [Table("Customers", Schema="book")]
    public class Customer
    {
        public Customer()
        {
            this.JuridicalType = 99;
            this.Phone2 = string.Empty;
            this.Desc = string.Empty;
            this.Tdate = DateTime.Now;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [JsonIgnore]
        public int Id { get; set; }

        [Column("tdate")]
        [JsonIgnore]
        public DateTime Tdate { get; set; }

        [Column("name")]
        [StringLength(100)]
        [Required(ErrorMessage = "შეიყვანეთ სახელი")]
        [DisplayName("სახელი:")]
        public string Name { get; set; }

        [Column("lastname")]
        [StringLength(100)]
        [Required(ErrorMessage = "შეიყვანეთ გვარი")]
        [DisplayName("გვარი:")]
        public string LastName { get; set; }

        [Column("code", TypeName="varchar")]
        [StringLength(11)]
        [Required(ErrorMessage = "შეიყვანეთ პ/ნ")]
        [DisplayName("პ/ნ:")]
        public string Code { get; set; }

        [Column("address")]
        [StringLength(255)]
        [Required(ErrorMessage = "შეიყვანეთ იურ. მისამართი")]
        [DisplayName("იურ. მისამართი:")]
        public string Address { get; set; }

        [Column("type")]
        [DisplayName("ტიპი:")]
        public CustomerType Type { get; set; }

        [Column("juridical_type")]
        [DisplayName("იურ. სახეობა")]
        [Required(ErrorMessage = "აირჩიეთ იურ. სახეობა")]
        public int JuridicalType { get; set; }

        [Column("is_budget")]
        [DisplayName("საბიუჯეტო:")]
        public bool IsBudget { get; set; }

        [DisplayName("ხელშ. თარიღი:")]
        [Column("jurid_finish_date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JuridicalFinishDate { get; set; }

        [Column("city")]
        [DisplayName("სოფელი/ქალაქი:")]
        [Required(ErrorMessage = "შეიყვანეთ სოფელი/ქალაქი")]
        public string City { get; set; }

        [Column("village")]
        [DisplayName("რაიონი:")]
        public string Village { get; set; }

        [Column("district")]
        [DisplayName("უბანი:")]
        public string District { get; set; }

        [Column("email")]
        [DisplayName("Email:")]
        public string Email { get; set; }

        [Column("is_facktura")]
        [DisplayName("გამოიწეროს ფაქტურა:")]
        public bool IsFacktura { get; set; }

        [Column("region")]
        [DisplayName("რეგიონი:")]
        [Required(ErrorMessage = "შეიყვანეთ რეგიონი")]
        public string Region { get; set; }

        [Column("phone1", TypeName = "varchar")]
        [StringLength(50)]
        [DisplayName("ტელეფონი 1:")]
        [Required(ErrorMessage = "შეიყვანეთ ტელეფონი")]
        [MaxLength(9, ErrorMessage="შეიყვანეთ 9 ნიშნა რიცხვი")]
        [MinLength(9, ErrorMessage ="შეიყვანეთ 9 ნიშნა რიცხვი")]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage="ტელეფონი არასწორია")]
        public string Phone1 { get; set; }

        [Column("phone2", TypeName="varchar")]
        [StringLength(50)]
        [DisplayName("ტელეფონი 2:")]
        public string Phone2 { get; set; }

        [Column("desc")]
        [DisplayName("შენიშვნა:")]
        public string Desc { get; set; }

        [Column("security_code", TypeName = "varchar")]
        [StringLength(32)]
        [DataType(DataType.Password)]
        [DisplayName("კოდური სიტყვა:")]
        [Required(ErrorMessage = "შეიყვანეთ კოდური სიტყვა")]
        public string SecurityCode { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Customers", IsClustered = false)]
        [JsonIgnore]
        public int UserId { get; set; }

        [Column("verify_status")]
        [DisplayName("ვერიფიკაცია:")]
        public AbonentVerifyStatus VerifyStatus { get; set; }

        [Column("info")]
        [DisplayName("ინფო:")]
        public string Info { get; set; }

        [Column("attachment_approve_status")]
        [DisplayName("აქსესუარის აღრიცხვის სტატუსი:")]
        public short AttachmentApproveStatus { get; set; }

        [Column("buy_reason")]
        [DisplayName("ყიდვის მიზეზი:")]
        public AbonentBuyReason BuyReason { get; set; }

        [Column("is_satisfied")]
        [DisplayName("შედეგი:")]
        public AbonentSatisfiedStatus IsSatisfied { get; set; }

        [Column("signature")]
        [DisplayName("ხელმოწერა:")]
        public string signature { get; set; }

        [NotMapped]   
        public string signature_attachment { get; set; }

        [Column("temporary_use")]
        [DisplayName("დროებით სარგებლობაში:")]
        public int temporary_use { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public ICollection<Card> Cards { get; set; }

        public ICollection<Invoice> Invoices { get; set; }

        public ICollection<CustomerSellAttachments> CustomerSellAttachments { get; set; }


        [NotMapped]
        [JsonIgnore]
        public List<SelectListItem> GetJuridicalTypes
        {
            get
            {
                Dictionary<int, string> values = new Dictionary<int, string>()
                {
                    {1, "კომუნალური"},
                    {2, "კომერციული"}
                };

                return new List<SelectListItem>() { new SelectListItem() { Value = "99", Text = "" } }.Union(from KeyValuePair<int, string> n in values
                                                      select new SelectListItem { Value = n.Key.ToString(), Text = n.Value }).ToList();
            }
        }

        [NotMapped]
        [JsonIgnore]
        public List<SelectListItem> GetCustomerTypes
        {
            get
            {
                return (from CustomerType n in Enum.GetValues(typeof(CustomerType))
                        select new SelectListItem { Value = n.ToString(), Text = Utils.Utils.GetEnumDescription(n) }).ToList();
            }
        }
    }


    public enum CustomerType
    {
        [Description("ფიზიკური")]
        [Display(Name = "ფიზიკური")]
        Physical,
        [Description("იურიდიული")]
        [Display(Name = "იურიდიული")]
        Juridical,
        [Description("ტექნიკური")]
        [Display(Name = "ტექნიკური")]
        Technic 
    }

    public enum AbonentVerifyStatus
    {
        [Description("გასავლელი")]
        [Display(Name = "გასავლელი")]
        ForPass,
        [Description("გავლილი უშეცდომოდ")]
        [Display(Name = "გავლილი უშეცდომოდ")]
        Passed,
        [Description("გავლილი შეცდომით")]
        [Display(Name = "გავლილი შეცდომით")]
        PassedWithError,
        [Description("პრობლემა")]
        [Display(Name = "პრობლემა")]
        Problem,
        [Description("დარეკილი")]
        [Display(Name = "დარეკილი")]
        Called
    }

    //ტელევიზია - რომელი?

    // ფლაერი გადმომცეს

    // მეგობარმა, მეზობლემა, ახლობელმა მირჩია

    // ფეისბუქით

    // ინფო არხით

    // ინტერნეთ რეკლამით

    // დილერმა შემომთავაზა

    // ელიტ ელექტრინიქსის აქციით

    // SMS რეკლამა მომივიდა

    // ვნახე ბილბორდი

    // რადიო რეკლამა მოვისმინე

    // არ მახსოვს
    public enum AbonentBuyReason
    {
        [Description("დაუმოწმებელი")]
        [Display(Name = "დაუმოწმებელი")]
        Unverified,

        [Description("ფლაერი")]
        [Display(Name = "ფლაერი")]
        Flaer,

        [Description("მეგობარმა")]
        [Display(Name = "მეგობარმა")]
        Friends,

        [Description("ინფო არხი")]
        [Display(Name = "ინფო არხი")]
        InfoChannel,

        [Description("ინტერნეთ რეკლამა")]
        [Display(Name = "ინტერნეთ რეკლამა")]
        NetAdvertisement,

        [Description("დილერმა შემომთავაზა")]
        [Display(Name = "დილერმა შემომთავაზა")]
        DilerSuggestion,

        [Description("ელიტ ელექტრონიქსის აქცია")]
        [Display(Name = "ელიტ ელექტრინიქსის აქცია")]
        ElitShare,

        [Description("SMS რეკლამა")]
        [Display(Name = "SMS რეკლამა")]
        SmsAdvertisement,

        [Description("ვნახე ბილბორდი")]
        [Display(Name = "ვნახე ბილბორდი")]
        Billboard,

        [Description("რადიო რეკლამა")]
        [Display(Name = "რადიო რეკლამა")]
        RadioAdvertisement,

        [Description("არ მახსოვს")]
        [Display(Name = "არ მახსოვს")]
        DontRemember,

        [Description("ტელევიზია")]
        [Display(Name = "ტელევიზია")]
        Televizion,

        [Description("სხვა")]
        [Display(Name = "სხვა")]
        Other,


    }

    public enum AbonentSatisfiedStatus
    {
        [Description("დაუმოწმებელი")]
        [Display(Name = "დაუმოწმებელი")]
        Unverified,

        [Description("კმაყოფილი")]
        [Display(Name = "კმაყოფილი")]
        Satisfied,

        [Description("უკმაყოფილო")]
        [Display(Name = "უკმაყოფილო")]
        UnSatisfied
    }

}