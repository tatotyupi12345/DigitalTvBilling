using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("AutoMessageTemplates", Schema = "book")]
    public class AutoMessageTemplate
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("message")]
        public string MessageText { get; set; }

        [Column("type")]
        public MessageType MessageType { get; set; }

        [Column("query")]
        public string Query { get; set; }

        [Column("is_disposable")]
        public bool IsDisposable { get; set; }
    }
}