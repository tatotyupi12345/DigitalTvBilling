using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class InvoicePost
    {
        public List<int> Cards { get; set; }
        public int Months { get; set; }
    }
}