using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Cancellation", Schema = "dbo")]
    public class Cancellation
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
        public CancleStatus Status { get; set; }

        [Column("cancle_status")]
        public int cancle_status { get; set; }

        [Column("changer_user")]
        public string ChangerUser { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_Orders", IsClustered = false)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserUser { get; set; }

        [Column("to_go")]
        public int to_go { get; set; }

        [Column("comment")]
        public string comment { get; set; }

        [Column("user_group_id")]
        [Index("IX_UserGroupId_CardDamages", IsClustered = false)]
        [Required(ErrorMessage = "აირჩიეთ ჯქუფი")]
        [DisplayName("ჯქუფი:")]
        public int UserGroupId { get; set; }

        [Column("card_num")]
        public int card_num { get; set; }

        [Column("executor_id")]
        public int ExecutorID { get; set; }

        [ForeignKey("UserGroupId")]
        public User UserGroup { get; set; }

        [NotMapped]
        public Customer Customer { get; set; }



    }
    public enum CancleStatus
    {

        [Description("გაუქმება")]
        Cancel,
        [Description("ლოდინი")]
        loading,
        [Description("დასრულება")]
        Closed,
        [Description("გადაიფიქრა")]
        NotClosed,
        [Description("ნამდვილად გასაუქმებელი")]
        ReallyCancled,
        [Description("სერვის ცენტრი")]
        ServiceCenter

    }
}