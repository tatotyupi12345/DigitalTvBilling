using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("Invoices", Schema = "book")]
    public class Invoice
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("num")]
        public string Num { get; set; }

        [Column("tdate")]
        public DateTime Tdate { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("abonent_nums")]
        public string AbonentNums { get; set; }

        [Column("customer_id")]
        [Index("IX_CustomerId_Customers", IsClustered = false)]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}