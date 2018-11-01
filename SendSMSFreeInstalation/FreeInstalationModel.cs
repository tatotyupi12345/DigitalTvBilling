using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.SendSMSFreeInstalation
{
    public class FreeInstalationModel
    {
        public int id { get; set; }
        public string phone1 { get; set; }
        public DateTime finish_date { get; set; }
    }
}