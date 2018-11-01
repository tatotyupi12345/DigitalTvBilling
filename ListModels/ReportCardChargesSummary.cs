using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class ReportCardChargesSummary
    {
        public string abonent { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public string phone { get; set; }
        public string status { get; set; }
        public string abonent_num { get; set; }
        public string card_num { get; set; }
        public List<string> packets { get; set; }
        public DateTime packet_date { get; set; }
        public decimal? amount { get; set; } = 0;
    }
}