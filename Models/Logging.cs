using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Logging", Schema = "config")]
    public class Logging
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Logging", IsClustered = false)]
        public int UserId { get; set; }

        [Column("type")]
        public LogType Type { get; set; }

        [Column("mode")]
        public LogMode Mode { get; set; }

        [Column("type_value")]
        public string TypeValue { get; set; }

        [Column("type_id")]
        [Index("IX_TypeId_Loggings", IsClustered = false)]
        public long TypeId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<LoggingItem> LoggingItems { get; set; }
    }

    public enum LogType
    {
        [Description("აბონენტი")]
        Abonent = 0,
        [Description("ბარათი")]
        Card = 1,
        [Description("გადახდა")]
        Payment = 2,
        [Description("პაკეტი")]
        Package = 3,
        [Description("არხი")]
        Channel = 4,
        [Description("შეტყობინება")]
        Message = 5,
        [Description("მომხმარებელი")]
        User = 6,
        [Description("მომხმარებლის ჯგუფი")]
        UserType = 7,
        [Description("რესივერი")]
        Receiver = 8,
        [Description("ანძა")]
        Tower = 9,
        [Description("ბარათის პაკეტი")]
        CardPackage = 10,
        [Description("მომხმარებლის მოქმედება")]
        UserAction = 11,
        [Description("პარამეტრები")]
        Settings = 12,
        [Description("გაწეული მომსახურება")]
        Service = 13,
        [Description("ბარათის მომსახურება")]
        CardService = 14,
        [Description("შეკვეთა")]
        Order = 15,
        [Description("მიზეზი")]
        Reason = 16,
        [Description("ოფისის ბარათი")]
        OfficeCard = 17,
        [Description("ინვოისი")]
        Invoice = 18,
        [Description("გადახდის სახეობა")]
        PayType = 19,

        [Description("ქალაქი")]
        City = 20,
        [Description("დაზიანება")]
        Damage = 21,
        [Description("გაუქმება")]
        Cancled = 23,
        [Description("გასაყიდი მოწყობილობები")]
        SellAttachments = 20
    }

    public enum LogMode
    {
        [Description("ცვლილება")]
        Change,
        [Description("დამატება")]
        Add,
        [Description("წაშლა")]
        Delete,
        [Description("ბარათის მოქმედებება")]
        CardDeal,
        [Description("ავტორიზაცია")]
        Autorize,
        [Description("უშედეგო ავტორიზაცია")]
        IncorrectAutorize,
        [Description("თავისუფალი თანხის გადატანა")]
        FreeMoney,

        [Description("გადაფორმება")]
        Forward,

        [Description("ბუღალტრულად გატარება")]
        Approve,
        [Description("ვერიფიკაცია")]
        Verify,

        [Description("აქსესუარის ვერიფიცირება")]
        VerifySellAttachments,

        [Description("იურიდიული ვერიფიკაცია")]
        JuridVerify,
    }
}