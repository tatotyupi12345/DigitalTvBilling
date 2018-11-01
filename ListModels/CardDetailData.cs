using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class CardDetailData
    {
        public CustomerType CustomerType { get; set; }
        public bool IsBudget { get; set; }
        public string CustomerName { get; set; }
        public IEnumerable<short> CasIds { get; set; }
        public double SubscribAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal RentAmount { get; set; }
        public decimal Amount { get; set; }
        public string CahrgeTime { get; set; }
        public decimal WithoutServiceAmount { get; set; }
        public List<CardService> CardServices { get; set; }
        public Card Card { get; set; }
        public double MinPrice { get; set; }
        public List<CardLog> CardLogs { get; set; }
        public Subscribtion Subscribtion { get; set; }
        public List<SubscriptionPackage> SubscriptionPackages { get; set; }
        public List<string> PackageNames { get; set; }
    }
}