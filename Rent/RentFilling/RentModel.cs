using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Rent
{
    public class RentModel
    {
        public RentModel() { }

        public class ResultRent
        {
            public PaymentData pay_data { get; set; }
            public DataContext _db { get; set; }
            public int user_id { get; set; }
            public bool fromPay { get; set; }
            public string posted_file { get; set; }
        }
        public class RentSmsInfo
        {
            public string phone { get; set; }
            public string onPayMsg { get; set; }
            public string onPayMsg_geo { get; set; }
        }
    }
}