using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class LoggingData
    {
        public string field { get; set; }
        public string old_val { get; set; }
        public string new_val { get; set; }
        public string type { get; set; }
    }
}