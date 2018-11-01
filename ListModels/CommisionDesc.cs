using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class CommisionDesc
    {
        public int card_id { get; set; }
        public List<double> amount { get; set; }
        public List<int> commision_type { get; set; }
    }
}