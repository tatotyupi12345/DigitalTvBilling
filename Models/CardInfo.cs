using DigitalTVBilling.ListModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class CardInfo
    {
        public List<Subscribtion> Subscribtions { get; set; }
        public List<CardLog> CardLogs { get; set; }
        public List<Payment> Payments { get; set; }
        public List<CardCharge> OtherCharges { get; set; }
        public List<Balance> Balances { get; set; }
        public List<CardServicesList> CardServices { get; set; }
    }
}