using DocumentFormat.OpenXml.CustomXmlSchemaReferences;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("JuridicalDocsInfo", Schema = "dbo")]
    public class JuridicalDocsInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [Column("card_id")]
        public int card_id { get; set; }

        [Column("abonent_name")]
        public string abonent_name { get; set; }

        [Column("subscription_docs")]
        public string subscription_docs { get; set; }

        [Column("attachment_docs")]
        public string attachment_docs { get; set; }

        [Column("re_registering_docs")]
        public string re_registering_docs { get; set; }

        [Column("cancled_docs")]
        public string cancled_docs { get; set; }
    }
}