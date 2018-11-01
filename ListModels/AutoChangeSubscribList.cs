using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class AutoChangeSubscribList
    {
        public int Id { get; set; }
        public string Abonent { get; set; }
        public string AbonentNum { get; set; }
        public string PackageNames { get; set; }
        public DateTime SendDate { get; set; } 
    }
}