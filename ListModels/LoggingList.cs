using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class LoggingList
    {
        public long Id { get; set; }

        public DateTime Tdate { get; set; }

        public string UserGroupName { get; set; }

        public string UserName { get; set; }

        public LogType Type { get; set; }

        public LogMode Mode { get; set; }

        public string Value { get; set; }
    }
}