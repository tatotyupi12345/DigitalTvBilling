using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public class RetVal
    {
        public int code { get; set; }
        public string errorstr { get; set; }
        public List<Dictionary<string, string>> retvals { get; set; }
    }
}