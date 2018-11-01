using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class PaymentList
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string AbonentName { get; set; }
        public string AbonentNum { get; set; }
        public string CardNum { get; set; }
        public string PayType { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string FileName { get; set; }
        public decimal RentAmount { get; set; }
    }
}