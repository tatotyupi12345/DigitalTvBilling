using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class Form4_3_Report
    {
        public string city { get; set; }
        public string type { get; set; }
        public string coding { get; set; }
        public string package { get; set; }
        public int abonents_count { get; set; }
        public int decoders_count { get; set; }
        public int n_abonents_count { get; set; }
        public decimal money_in { get; set; }
        public decimal dxg { get; set; }
    }
}