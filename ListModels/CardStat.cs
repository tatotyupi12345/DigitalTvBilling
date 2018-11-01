using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalTVBilling.Models;

namespace DigitalTVBilling.ListModels
{
    public class CardStat
    {
        public Card card { get; set; }
        public Customer customer { get; set; }
        public Subscribtion subscribe { get; set; }
        public List<SubscriptionPackage> subscribePackages { get; set; }
        public Logging logging { get; set; }
        public User user { set; get; }
        public SellerObject seller { get; set; }
        public UserType userType { get; set; }
        public List<Package> packages { get; set; }
        public CardCharge card_change { get; set; }
        public int rowNumber { get; set; }
    }

    public class FilterDetails
    {
        public int? abonent { get; set; }
        public string abonent_select { get; set; }
        public string abonentName { get; set; }
        public int? abonentType { get; set; }
        public int? abonentStatus { get; set; }
        public int? package { get; set; }
        public int? saleType { get; set; }
        public int? logStatus { get; set; }
        public string region { get; set; }
        public string date_from { get; set; }
        public string date_to { get; set; }
    }

}