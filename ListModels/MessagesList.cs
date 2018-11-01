using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class MessagesList
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string MessageText { get; set; }
        public long Id { get; set; }
    }
}