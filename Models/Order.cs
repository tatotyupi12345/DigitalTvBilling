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
    [Table("Orders", Schema="doc")]
    public class Order
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("num")]
        public int Num { get; set; }

        [StringLength(11)]
        [Column("code")]
        public string Code { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("card_address")]
        public string CardAddress { get; set; }

        [Column("approve_user")]
        public string ApproveUser { get; set; }

        [Column("is_approved")]
        public bool IsApproved { get; set; }

        [Column("data")]
        public string Data { get; set; }

        [Column("receivers_count")]
        public int ReceiversCount { get; set; }

        [Column("get_date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime GetDate { get; set; }

        [Column("change_date")]
        public DateTime ChangeDate { get; set; }

        [Column("status")]
        public OrderStatus Status { get; set; }


        //[Column("poll")]
        //[Required(ErrorMessage = "შეავსეთ გამოკითხვა")]
        //public CallCentr Poll { get; set; }

        [Column("poll")]
        [Required(ErrorMessage = "შეიყვანეთ გვარი")]
        [DisplayName("გამოკითხვა:")]
        public CallCentr Poll { get; set; }

        [Column("to_go")]
        public int to_go { get; set; }

        [Column("montage_status")]
        public bool MontageStatus { get; set; }

        [Column("changer_user")]
        public string ChangerUser { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Orders", IsClustered = false)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserUser { get; set; }

        [Column("user_group_id")]
        [Index("IX_UserGroupId_CardDamages", IsClustered = false)]
        [Required(ErrorMessage = "აირჩიეთ ჯქუფი")]
        [DisplayName("ჯქუფი:")]
        public int UserGroupId { get; set; }

        [ForeignKey("UserGroupId")]
        public User UserGroup { get; set; }

        [NotMapped]
        public Customer Customer { get; set; }

        [Column("executor_id")]
        public int ExecutorID { get; set; }

        [Column("comment")]
        public string comment { get; set; }

        public ICollection<OrderReserveAnswer> OrderReserveAnswers { get; set; }
        public ICollection<OrderReason> OrderReasons { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<SelectListItem> GetPoll
        {
            get
            {
                return (from CallCentr n in Enum.GetValues(typeof(CallCentr))
                        select new SelectListItem { Value = n.ToString(), Text = Utils.Utils.GetEnumDescription(n) }).ToList();
            }
        }

    }

    public enum OrderStatus
    {
        [Description("დარეგისტრირდა")]
        Registered,
        [Description("მონტაჟი")]
        Montage,
        [Description("გაუქმება")]
        Canceled,
        [Description("დამუშავება")]
        Worked,
        [Description("გადადება")]
        Delayed,
        [Description("ლოდინი")]
        Loading,
        [Description("გაგზავნა")]
        Sended,
        [Description("დასრულება")]
        Closed,
        [Description("უფასო აქცია")]
        Promo
    }
    public enum CallCentr
    {
        [Description("")]
        Null=0,
        [Description("მეტრო")]
        Metro,
        [Description("სოციალური მედია")]
        SocialMedia,
        [Description("ტელევიზია")]
        TV,
        [Description("რადიო")]
        Radio,
        [Description("ინტერნეტგამოცემა")]
        OnlineEdition,
        [Description("გაზეთი")]
        Newspaper,
        [Description("სმს")]
        SMS,
        [Description("ახლობლისგან გავიგე")]
        ILearnedFrommyRelative,
        [Description("უარი პასუხზე")]
        RefuseToAnswer
    }
    public enum OrderCommitStatic
    {
        [Description("")]
        Null = 0,
        [Description("დასრულება")]
        Finish,
        [Description("სხვა:მიზეზი")]
        OtherReason,
        [Description("ვერ მივიღე სიგნალი")]
        Ididnotgetasignal,
        [Description("არ არის სიგანალი")]
        NotSiganali,
        [Description("სხვა ოპერატორი ისარგებლა")]
        AnotherOperatorHasBeenInstalled,
        [Description("გადაიფიქრა")]
        Change,
        [Description("არ აქვს ფული")]
        DoNotHaveMany,
        [Description("ვიზიტის გადადება")]
        PostponementVisit,
    }
}