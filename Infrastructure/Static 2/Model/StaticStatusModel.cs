using DigitalTVBilling.CallCenter;
using DigitalTVBilling.ListModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2.Model
{
    public class StaticStatusModel
    {
        public List<IdName> _OrderDistinguished { get; set; }
        public List<IdName> _DamageDistinguished { get; set; }
        public Object Card_Status { get; set; }
        public List<CallUser> UserStatic { get; set; }
    }
}