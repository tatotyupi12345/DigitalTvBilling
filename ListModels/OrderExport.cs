using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class OrderExport
    {
        public string create_user { get; set; }
        public string abonent_name { get; set; }
        public string city { get; set; }
        public string raion { get; set; }
        public string region { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public DateTime get_date { get; set; }
        public DateTime tdate { get; set; }
        public int receivers_count { get; set; }
        public string comment { get; set; }
    }
}