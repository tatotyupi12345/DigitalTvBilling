using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class Chat
    {
        public int Id { get; set; }
        public DateTime Tdate { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}