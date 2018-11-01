using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling
{
    public class TrapStat
    {
        public class Towers
        {
            public int id { get; set; }
            public string name { get; set; }

            public bool A { get; set; }
            public bool B { get; set; }
            public bool C { get; set; }
            public bool D { get; set; }
            public bool E { get; set; }
            public bool F { get; set; }
            public bool G { get; set; }
            public bool H { get; set; }
            public bool J { get; set; }
            public bool K { get; set; }

            public bool MSE { get; set; }
            public bool RSSI { get; set; }
        }
    }
}