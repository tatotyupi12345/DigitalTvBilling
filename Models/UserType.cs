using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("UserTypes", Schema = "book")]
    public class UserType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [DisplayName("დასახელება:")]
        [Required(ErrorMessage = "შეიყვანეთ დასახელება")]
        public string Name { get; set; }

        [NotMapped]
        public string Logging { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}