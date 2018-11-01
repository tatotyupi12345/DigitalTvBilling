using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Models
{
    public class Abonent
    {
        
        public int UserID { get; set; }
        public Customer Customer { get; set; }
        public List<Card> Cards { get; set; }
        public AbonentDetailInfo AbonentDetailInfo { get; set; }
        public string Logging { get; set; }
        public AutoSubscribChangeCard autoSubscribChange { get; set; }

        public bool isFromDiler { get; set; }
        public DilerCards dilerCards { get; set; }

        public List<SellAttachment> attachments { get; set; }

    }

    public class DilerCards
    {
        public int diler_id { get; set; }
        public List<int> card_id { get; set; }
    }
}