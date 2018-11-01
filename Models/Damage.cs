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
    [Table("Damage", Schema = "dbo")]
    public class Damage
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
        [DisplayName("სტატუსი:")]
        public DamageStatus Status { get; set; }

        [Column("montage_status")]
        public bool MontageStatus { get; set; }

        [Column("montage_user_id")]
        public int montage_user_id { get; set; }

        [Column("changer_user")]
        public string ChangerUser { get; set; }

        [Column("to_go")]
        public int to_go { get; set; }

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

        //[Column("static_comment")]
        //public DamageCommitStatic static_comment { get; set; }

        //[Column("answers_tdate")]
        //public DateTime answers_tdate { get; set; }

        //public ICollection<OperatorGroupUser> OperatorGroupUsers { get; set; }
        public ICollection<DamageReserveAnswer> DamageReserveAnswers { get; set; }
        public ICollection<DamageReason> DamageReasons { get; set; }

        //public ICollection<OperatorGroupUser> OperatorGroupUsers { get; set; }



        //public enum DamageStatus
        //{
        //    [Description("დარეგისტრირდა")]
        //    Registered,
        //    [Description("მონტაჟი")]
        //    Montage,
        //    [Description("გაუქმება")]
        //    Canceled,
        //    [Description("დამუშავება")]
        //    Worked,
        //    [Description("გადადება")]
        //    Delayed,
        //    [Description("ლოდინი")]
        //    Loading,
        //    [Description("გაგზავნა")]
        //    Sended,
        //    [Description("დასრულება")]
        //    Closed,
        //    [Description("დაზიანება")]
        //    Damage
        //}
        [NotMapped]
    [JsonIgnore]
    public List<SelectListItem> GetDamageTypes
    {
        get
        {
            return (from DamageStatus n in Enum.GetValues(typeof(DamageStatus))
                    select new SelectListItem { Value = n.ToString(), Text = Utils.Utils.GetEnumDescription(n) }).ToList();
        }
    }

    }

    public enum DamageStatus
    {
        [Description("ლოდინი")]
        loading,
        [Description("ბარათის პრობლემა")]
        CardProblem,
        [Description("TV/AV გადაყვანა")]
        TVTransfer,
        [Description("sender-ის პრობლემა")]
        SenderProblem,
        [Description("ქარხნული წუნი")]
        FactoryDefects,
        [Description("პროგრამული განახლება")]
        ProgramNew,
        [Description("ანტენა/კაბელის პრობლემები")]
        AntenProblem,
        [Description("დასრულება")]
        Closed,
        [Description("მიზეზი სხვა")]
        OtherReason,
        [Description("სიგნალი არ არის")]
        NoSignal,
        [Description("თვითონ გამოსწორდა ისე რომ ინსტალატორი არ მივიდა")]
        ImprovedInstallerDidNot,
        [Description("დამუშავება")]
        Processing,
        [Description("მოუგვარებელი")]
        Unresolved,
        [Description("პროცესი")]
        Processd=-2,



    }
    public enum DamageCommitStatic
    {
        [Description("")]
        Null = 0,
        [Description("დასრულება")]
        Finish,
        [Description("სხვა:მიზეზი")]
        OtherReason,
        [Description("კაბელის დაზიანება")]
        CabelDamage,
        [Description("ანტენის პრობლემა")]
        AntennaProblem,
        [Description("ბოქსის ქარხნული წუნი/შეცვლა")]
        BoxFactor,
        [Description("პროგრამული განახლება")]
        SoftwareUpdate,
        [Description("კვების ბლოკის დაზიანება/გაყიდვა")]
        InjurySell,
        [Description("TV/AV")]
        TVAV,
        [Description("მისამართის შეცვლა")]
        ChangeAddress,
        [Description("აქსესუარის დაზიანება/გაყიდვა")]
        AccessoryDamageSale,
        [Description("სიგნალი აღარ აქვს ")]
        ThereisnoSignal
    }
}