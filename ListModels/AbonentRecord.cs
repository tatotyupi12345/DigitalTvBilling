using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class AbonentRecord
    {
        public int ID { get; set; }
        public int Mode { get; set; }
        //public double SumPrice { get; set; }
        public DateTime tdate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerAddress { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string Package { get; set; }
        public string Group { get; set; }
        public string ObjectName { get; set; }
        public string ObjectAddress { get; set; }
        public SellerType? SType { get; set; }
        public System.Int64 row_num { get; set; }
        public int Status { get; set; }
    }
}