using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("ReturnedCards", Schema = "dbo")]
    public class ReturnedCard
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("tdate")]
        [DisplayName("თარიღი:")]
        public DateTime Tdate { get; set; }

        [Column("card_id")]
        public int card_id { get; set; }

        [Column("commission")]
        public string commission { get; set; }

        [Column("user_id")]
        public int user_id { get; set; }

        [Column("bort_id")]
        public int bort_id { get; set; }

        [Column("commission_amount")]
        public double commission_amount { get; set; }

        [Column("returned_amount")]
        public double returned_amount { get; set; }

        [Column("reason")]
        public int reason { get; set; }

        [Column("info")]
        public string info { get; set; }

        [Column("pretentious")]
        public int pretentious { get; set; }

        [Column("approve")]
        public int approve { get; set; }

        [ForeignKey("card_id")]
        public Card Card { get; set; }

        [ForeignKey("bort_id")]
        public User User_bort { get; set; }

        [ForeignKey("user_id")]
        public User User { get; set; }

        public ICollection<ReturnedCardAttachment> ReturnedCardAttachments { get; set; }


    }

    public enum ReturnCardStatus
    {
        [Description("სხვა ოპერატორით შეცვალა")]
        [Display(Name = "სხვა ოპერატორით შეცვალა")]
        Change = 1,
        [Description("შეიცვალა საც.ადგილი/ჰქონდა აგარაკისთვის")]
        [Display(Name = "შეიცვალა საც.ადგილი/ჰქონდა აგარაკისთვის")]
        ChangeHome,
        [Description("ჰქონდა ტეკნიკური პრობლემა/არ იჭერდა კარგად")]
        [Display(Name = "ჰქონდა ტეკნიკური პრობლემა/არ იჭერდა კარგად")]
        TechProblem,
        [Description("არ აკმაყოფილებლა არხების ჩამონათვალი")]
        [Display(Name = "არ აკმაყოფილებლა არხების ჩამონათვალი")]
        ChannelList,
        [Description("უკმაყოფილო იყო მომსახურებით")]
        [Display(Name = "უკმაყოფილო იყო მომსახურებით")]
        BadService,
        [Description("პრობლემური გამოუსწორებელი")]
        [Display(Name = "პრობლემური გამოუსწორებელი")]
        Problemirreparable,
        [Description("ნაქირვები რესივერი შეიძინა")]
        [Display(Name = "ნაქირვები რესივერი შეიძინა")]
        BuyRented,
        [Description("სხვა მიზეზი")]
        [Display(Name = "სხვა მიზეზი")]
        Other,
        [Description("არ/ვერ გაგვცა პასუხი")]
        [Display(Name = "არ/ვერ გაგვცა პასუხი")]
        NoAsnwer,
        [Description("15-დან 8-ზე გადასვლა")]
        [Display(Name = "15-დან 8-ზე გადასვლა")]
        PackagesChange
        
    }

    public enum CommissionStatus
    {
        [Description("ნაღდი")]
        [Display(Name = "ნაღდი")]
        Cash = 2,

        [Description("ნაღდი/უნაღდო")]
        [Display(Name = "ნაღდი/უნაღდო")]
        Terminal = 18
    }
    public enum PretentiousAbonentStatus
    {
        [Description("არ არის პრეტენზიული")]
        [Display(Name = "არ არის აპრეტენზიული")]
        NotPretentious = 0,

        [Description("პრეტენზიული")]
        [Display(Name = "პრეტენზიული")]
        Pretentious


    }
}