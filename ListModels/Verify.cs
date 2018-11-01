using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class Verify
    {
        public Customer customer { get; set; }
        public bool is_customer_verified { get; set; }
        public List<Card> cards { get; set; }
    }
}