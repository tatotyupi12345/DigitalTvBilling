using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("CardDamages", Schema = "doc")]
    public class CardDamage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("card_id")]
        [Index("IX_CardId_CardDamages", IsClustered = false)]
        public int CardId { get; set; }

        [Column("desc")]
        public string Description { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; }

        [Column("status")]
        public CardDamageStatus Status { get; set; }

        [Column("cgd")]
        public string Cgd { get; set; }

        [Column("changer_user")]
        public string ChangerUser { get; set; }

        [Column("approve_user")]
        public string ApproveUser { get; set; }

        [Column("is_approved")]
        public bool IsApproved { get; set; }

        [Column("change_date")]
        public DateTime ChangeDate { get; set; }

        [Column("get_date")]
        public DateTime GetDate { get; set; }

        [Column("user_id")]
        [Index("IX_UserId_CardDamages", IsClustered = false)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column("user_group_id")]
        [Index("IX_UserGroupId_CardDamages", IsClustered = false)]
        public int UserGroupId { get; set; }

        [ForeignKey("UserGroupId")]
        public User UserGroup { get; set; }

        [NotMapped]
        public Customer Customer { get; set; }

        public ICollection<DamageReason> DamageReason { get; set; }

    }

    public enum CardDamageStatus
    {
        [Description("დარეგისტრირდა")]
        Registered,
        [Description("შესრულდა")]
        Executed,
        [Description("გადაიდო")]
        Loading,
        [Description("გაგზავნილია")]
        Sended,
        [Description("გაუქმდა")]
        Canceled,
        [Description("დემონტაჟი")]
        Demontaged,
        [Description("მუშავდება")]
        Worked
    }
}