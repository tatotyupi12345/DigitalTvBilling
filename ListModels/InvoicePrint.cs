using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class InvoicePrintItem
    {
        public string Name {get;set;}
        public double Amount {get;set;}
    }

    public class InvoicePrint
    {
        public string Num { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AbonentId { get; set; }
        public string AbonentCode { get; set; }
        public string AbonentEmail { get; set; }
        public string AbonentName { get; set; }
        public string AbonentAddress { get; set; }
        public string AbonentPhone { get; set; }
        public List<InvoicePrintItem> Items { get; set; }
    }
}