using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class BalanceAmounList
    {
        public int id { get; set; }
        public string phone { get; set; }
        public string abonentName { get; set; }
        public string abonentNum { get; set; }
        public decimal balance { get; set; }
        public string packname { get; set; }
        public decimal packamount { get; set; }

    }
}