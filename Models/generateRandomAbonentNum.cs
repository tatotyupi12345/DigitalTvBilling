using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class generateRandomAbonentNum
    {
        [Column("generated_random_id")]
        public int generateId { get; set; }
    }
}