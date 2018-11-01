using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class CancellationCardNum
    {
        public string Customer_Address { get; set; }
        public string Customer_City { get; set; }
        public string Customer_Village { get; set; }
        public string Customer_Region { get; set; }
        public string Customer_Phone1 { get; set; }
        public string Customer_Phone2 { get; set; }
        public string Customer_Desc { get; set; }
        public string Customer_District { get; set; }
        public string Customer_Code { get; set; }

        public string Customer_Type { get; set; }
        public string Customer_Name { get; set; }
        public string Customer_LastName { get; set; }
        public DateTime GetDate { get; set; }
        public List<string> CardNum { get; set; }
    }
}