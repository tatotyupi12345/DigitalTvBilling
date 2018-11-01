using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class AbonentList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public CustomerType Type { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Num { get; set; }
        public string CardNum { get; set; }
        public string DocNum { get; set; }
        public string ActivePacket { get; set; }
        public CardStatus Status { get; set; }
    }

}