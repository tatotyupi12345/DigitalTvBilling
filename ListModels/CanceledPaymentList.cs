using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class CanceledPaymentList
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string FromCard { get; set; }
        public string CustomerName { get; set; }
        public string Code { get; set; }
    }
}