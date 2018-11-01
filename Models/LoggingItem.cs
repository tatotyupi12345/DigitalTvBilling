using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("LoggingItems", Schema = "config")]
    public class LoggingItem
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("logging_id")]
        [Index("IX_LoggingId_loggingItems", IsClustered = false)]
        public long LoggingId { get; set; }

        [Column("column_display")]
        public string ColumnDisplay { get; set; }

        [Column("old_value")]
        public string OldValue { get; set; }

        [Column("new_value")]
        public string NewValue { get; set; }

        [ForeignKey("LoggingId")]
        public Logging Logging { get; set; }
    }
}