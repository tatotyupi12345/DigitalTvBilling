using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling
{
    public class CancelPayment
    {
        public int Type { get; set; }
        public int CardId { get; set; }
        public List<int> Cards { get; set; }
        public decimal Amount { get; set; }
    }
}