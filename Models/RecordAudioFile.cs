using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    [Table("RecordAudioFile", Schema = "dbo")]
    public class RecordAudioFile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("card_id")]
        public int card_id { get; set; }

        [Column("audio_name")]
        public string audio_name { get; set; }

        [Column("info")]
        public string info { get; set; }

        [Column("status")]
        public PackagesChargesStatus Status { get; set; }

        [Column("verify_status")]
        public int verify_status { get; set; }

    }

    public enum PackagesChargesStatus
    {
        [Description("პაკეტის ცვლილება 8-დან 15-ზე")]
        CardPackageCharges,
        [Description("ბარათი დაპაუზდა")]
        CardPaused,
    }
}
