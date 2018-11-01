using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class Balance
    {
        public DateTime Tdate { get; set; }
        public decimal OutAmount { get; set; }
        public CardChargeStatus OutAmountStatus { get; set; }
        public decimal InAmount { get; set; }
        public string CardName { get; set; }
        public decimal RAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal RentAmount { get; set; }
        public decimal InRentAmount { get; set; }
    }
}