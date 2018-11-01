using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Cards", Schema = "book")]
    public class Card
    {
        public Card()
        {
            this.Tdate = DateTime.Now;
            this.CallDate = DateTime.Now;
            this.CasDate = DateTime.Now;
            this.Discount = 0;
            this.HasFreeDays = true;
            this.JuridVerifyStatus = CardJuridicalVerifyStatus.None;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        [DisplayName("თარიღი:")]
        public DateTime Tdate { get; set; }

        [Column("abonent_num", TypeName = "varchar")]
        [StringLength(255)]
        [DisplayName("აბონენტის №:")]
        //[Required(ErrorMessage = "შეიყვანეთ აბონენტის №")]
        [Index("IX_CardAbonentNum_Cards", IsClustered = false, IsUnique = true)]
        //[RegularExpression(@"^[A-Z]+\d{5}$", ErrorMessage = "აბონენტის № არასწორია")]
        public string AbonentNum { get; set; }

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

        [Column("doc_num")]
        [DisplayName("ხელშეკრ. №:")]
        //[Required(ErrorMessage = "შეივანეთ ხელშ. №")]
        public string DocNum { get; set; }

        [Column("discount")]
        [DisplayName("ფასდაკლება:")]
        [Range(0, Int32.MaxValue, ErrorMessage = "შეიყვანეთ ფასდაკლება")]
        [Required(ErrorMessage = "შეივანეთ ფასდაკლება")]
        public double Discount { get; set; }

        [Column("group")]
        [DisplayName("ჯგუფი:")]
        [Range(0, Int32.MaxValue, ErrorMessage = "შეივანეთ ჯგუფი")]
        public int Group { get; set; }

        [Column("customer_id")]
        [DisplayName("აბონენტი:")]
        [Required(ErrorMessage = "აირჩიეთ აბონენტი")]
        [NoLog]
        [Index("IX_CustomerId_Cards", IsClustered = false)]
        public int CustomerId { get; set; }

        [Column("status")]
        [Index("IX_CardStatus_Cards", IsClustered = false)]
        [DisplayName("ბარათის სტატუსი:")]
        public CardStatus CardStatus { get; set; }

        [Column("city")]
        [DisplayName("სოფელი/ქალაქი:")]
        [Required(ErrorMessage = "შეიყვანეთ სოფელი/ქალაქი")]
        public string City { get; set; }

        [Column("village")]
        public string Village { get; set; }

        [Column("region")]
        public string Region { get; set; }

        [Column("close_date")]
        public DateTime CloseDate { get; set; }

        [Column("finish_date")]
        public DateTime FinishDate { get; set; }

        [Column("rent_finish_date")]
        public DateTime RentFinishDate { get; set; }

        [Column("pause_date")]
        public DateTime PauseDate { get; set; }

        [Column("pause_days")]
        public int PauseDays { get; set; }

        [Column("closed_is_pen")]
        [DisplayName("ჯარიმის დარიცხვა:")]
        public bool ClosedIsPen { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Cards", IsClustered = false)]
        public int UserId { get; set; }


        //[Required(ErrorMessage = "აირჩიეთ ანძა")]
        //[NoLog]
        [Column("tower_id")]
        [DisplayName("ანძა:")]
        public int TowerId { get; set; }

        [Column("receiver_id")]
        [DisplayName("რესივერი:")]
        [Required(ErrorMessage = "აირჩიეთ რესივერი")]
        [NoLog]
        public int ReceiverId { get; set; }

        [Column("latitude")]
        public double? Latitude { get; set; }

        [Column("longitude")]
        public double? Longitude { get; set; }

        [Column("mux1_level")]
        [DisplayName("მუქსები:")]
        public string mux1_level { get; set; }
        [Column("mux2_level")]
        public string mux2_level { get; set; }
        [Column("mux3_level")]
        public string mux3_level { get; set; }

        [Column("mux1_quality")]
        public string mux1_quality { get; set; }
        [Column("mux2_quality")]
        public string mux2_quality { get; set; }
        [Column("mux3_quality")]
        public string mux3_quality { get; set; }

        [Column("juridical_verify_status")]
        public string juridical_verify_status { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("TowerId")]
        public Tower Tower { get; set; }


        [ForeignKey("ReceiverId")]
        public Receiver Receiver { get; set; }

        [NotMapped]
        public string LogReceiver { get; set; }

        [NotMapped]
        [DisplayName("ანძა")]
        public string LogTower { get; set; }

        [Column("cas_date")]
        [NoLog]
        public DateTime CasDate { get; set; }

        [Column("mode")]
        [NoLog]
        public int Mode { get; set; }

        [Column("approve_status")]
        [NoLog]
        public int ApproveStatus { get; set; }

        [Column("auto_invoice")]
        [DisplayName("ავტომატური ინვოისი:")]
        public bool AutoInvoice { get; set; }

        [DisplayName("უფასო დღეები:")]
        [NotMapped]
        public bool HasFreeDays { get; set; }

        [Column("verify_status")]
        [DisplayName("ვერიფიკაცია:")]
        public CardVerifyStatus VerifyStatus { get; set; }

        [Column("juridical_verification")]
        [DisplayName("იურიდიული ვერიფიკაცია:")]
        public CardJuridicalVerifyStatus JuridVerifyStatus { get; set; }

        [Column("blocked_cards_verifiction")]
        [DisplayName("დაბლოკილი ბარათები ვერიფიკაცია:")]
        public CardBlockedCardsStatus BlockCardVerifyStatus { get; set; }

        [Column("info")]
        [DisplayName("ინფო:")]
        public string Info { get; set; }

        [Column("desc")]
        [DisplayName("აღწერა:")]
        public string Desc { get; set; }

        [Column("stopped_status")]
        [DisplayName("შემოწმების სტატუსი")]
        public StoppedCardStatus StoppedCheckStatus { get; set; }

        [Column("last_pause_type")]
        [DisplayName("ბოლო პაუზის ტიპი")]
        public PauseType LastPauseType { get; set; }

        [Column("has_used_free_pause_days")]
        public bool PauseFreeMonthUsed { get; set; }

        [Column("call_date")]
        [DisplayName("დარეკვის თარიღი:")]
        public DateTime CallDate { get; set; }

        [Column("none_free_pause_count_per_month")]
        [DisplayName("გამოყენებული პაუზის რაოდენობა:")]
        public short NonFreePausedCountPerMonth { get; set; }


        public ICollection<Payment> Payments { get; set; }

        public ICollection<Subscribtion> Subscribtions { get; set; }

        public ICollection<CardCharge> CardCharges { get; set; }

        public ICollection<CardLog> CardLogs { get; set; }

        public ICollection<CardService> CardServices { get; set; }

        public ICollection<MessageAbonent> MessageAbonent { get; set; }

        public ICollection<PayTransactionCard> PayTransactionCards { get; set; }

        public ICollection<CardDamage> CardDamages { get; set; }

        public ICollection<ReturnedCard> ReturnedCards { get; set; }

        public ICollection<JuridicalStatus> JuridicalStatus { get; set; }

        public ICollection<JuridicalLogging> JuridicalLoggings { get; set; }
    }

    public enum CardStatus
    {
        [Description("აქტიური")]
        [Display(Name = "აქტიური")]
        Active,
        [Description("გათიშული")]
        [Display(Name = "გათიშული")]
        Closed,
        [Description("დაპაუზებული")]
        [Display(Name = "დაპაუზებული")]
        Paused,
        [Description("მონტაჟი")]
        [Display(Name = "მონტაჟი")]
        Montage,
        [Description("გაუქმებული")]
        [Display(Name = "გაუქმებული")]
        Canceled,
        [Description("დაბლოკილი")]
        [Display(Name = "დაბლოკილი")]
        Blocked,
        [Description("უფასო დღეები")]
        [Display(Name = "უფასო დღეები")]
        FreeDays,
        [Description("იჯარა")]
        [Display(Name = "იჯარა")]
        Rent,
        [Description("შეწყვეტილი")]
        [Display(Name = "შეწყვეტილი")]
        Discontinued


    }

    public enum CardVerifyStatus
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

    public enum CardJuridicalVerifyStatus
    {
        [Description("სტატუსის გარეშე")]
        [Display(Name = "სტატუსის გარეშე")]
        None = -1,
        [Description("ჩაბარებული")]
        [Display(Name = "ჩაბარებული")]
        Delivered,
        [Description("გავლილი")]
        [Display(Name = "გავლილი")]
        Passed,
        [Description("პრობლემური გამოსწორებადი")]
        [Display(Name = "პრობლემური გამოსწორებადი")]
        FixableProblematic,
        [Description("პრობლემური გავლილი")]
        [Display(Name = "პრობლემური გავლილი")]
        PassedProblematic,
        [Description("პრობლემური გამოუსწორებელი")]
        [Display(Name = "პრობლემური გამოუსწორებელი")]
        NotFixableProblem,
        [Description("გაუქმებული")]
        [Display(Name = "გაუქმებული")]
        Stopped,
        [Description("აბონენტის მონაცემები არასწორია/არასრულია")]
        [Display(Name = "აბონენტის მონაცემები არასწორია/არასრულია")]
        InvalidIncomplete,
        [Description("ხელმოწერის გარეშე")]
        [Display(Name = "ხელმოწერის გარეშე")]
        WithoutSigning,
        [Description("პაკეტის არევა")]
        [Display(Name = "პაკეტის არევა")]
        PackageMessedUp,
        [Description("დანართის გარეშე")]
        [Display(Name = "დანართის გარეშე")]
        WithoutAnAttachment,
        [Description("ატვირთული")]
        [Display(Name = "ატვირთული")]
        Uploaded
    }
    public enum CardBlockedCardsStatus
    {
        [Description("შესამოწმებელი")]
        [Display(Name = "შესამოწმებელი")]
        Unchecked,

        [Description("გადაიხდის")]
        [Display(Name = "გადაიხდის")]
        GoingToPay,

        [Description("აპირებს გაუქმებას")]
        [Display(Name = "აპირებს გაუქმებას")]
        GoingToCancel,

        [Description("ტექნიკური პრობლემა")]
        [Display(Name = "ტექნიკური პრობლემა")]
        TechnicalProblem,

        [Description("ვერ დაუკავშირდა")]
        [Display(Name = "ვერ დაუკავშირდა")]
        UnConnected,

        [Description("აქვს საკუთრებაში")]
        [Display(Name = "აქვს საკუთრებაში")]
        Owner
    }
    public enum CardBlockedCardsVerifictionStatus
    {
        [Description("სტატუსის გარეშე")]
        [Display(Name = "სტატუსის გარეშე")]
        None = -1,
        [Description("ჩაბარებული")]
        [Display(Name = "ჩაბარებული")]
        Delivered,
        [Description("გავლილი")]
        [Display(Name = "გავლილი")]
        Passed,
        [Description("პრობლემური გამოსწორებადი")]
        [Display(Name = "პრობლემური გამოსწორებადი")]
        FixableProblematic,
        [Description("პრობლემური გავლილი")]
        [Display(Name = "პრობლემური გავლილი")]
        PassedProblematic,
        [Description("პრობლემური გამოუსწორებელი")]
        [Display(Name = "პრობლემური გამოუსწორებელი")]
        NotFixableProblem,
        [Description("შეწყვეტილი")]
        [Display(Name = "შეწყვეტილი")]
        Stopped
    }
    public enum StoppedCardStatus
    {
        [Description("შესამოწმებელი")]
        [Display(Name = "შესამოწმებელი")]
        Unchecked,

        [Description("გადაიხდის")]
        [Display(Name = "გადაიხდის")]
        GoingToPay,

        [Description("აპირებს გაუქმებას")]
        [Display(Name = "აპირებს გაუქმებას")]
        GoingToCancel,

        [Description("ტექნიკური პრობლემა")]
        [Display(Name = "ტექნიკური პრობლემა")]
        TechnicalProblem,

        [Description("ვერ დაუკავშირდა")]
        [Display(Name = "ვერ დაუკავშირდა")]
        UnConnected,

        [Description("აქვს საკუთრებაში")]
        [Display(Name = "აქვს საკუთრებაში")]
        Owner,
        [Description("გადადება")]
        [Display(Name = "გადადება")]
        Delay,
        [Description("პრომო")]
        [Display(Name = "პრომო")]
        Promo

    }

    public enum PauseType
    {
        [Description("გამოუყენებელი")]
        [Display(Name = "გამოუყენებელი")]
        UnUsed = 0,

        [Description("1 თვე უფასო")]
        [Display(Name = "1 თვე უფასო")]
        OneMonthFree = 1,

        [Description("1 თვე 3 ლარი")]
        [Display(Name = "1 თვე 3 ლარი")]
        OneMonth,

        [Description("3 თვე 9 ლარი")]
        [Display(Name = "3 თვე 9 ლარი")]
        ThreeMonth
    }
}