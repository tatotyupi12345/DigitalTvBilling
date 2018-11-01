using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("UserPermissions", Schema = "book")]
    public class UserPermission
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("tag")]
        public string Tag { get; set; }

        [Column("group")]
        public string Group { get; set; }

        [Column("sign")]
        public bool Sign { get; set; }

        [Column("type")]
        public int Type { get; set; }

        [ForeignKey("Type")]
        public UserType UserType { get; set; }
    }
}