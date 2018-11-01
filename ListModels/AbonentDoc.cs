using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class AbonentDoc
    {
        public Customer customer { get; set; }
        public User user { get; set; }
        public SellerObject seller { get; set; }
        public List<string> cards { get; set; }
    }

    public enum DocType
    {

    }
}