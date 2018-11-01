using DigitalTVBilling.ListModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class AbonentDetailInfo
    {
        public decimal Balanse { get; set; }
        public decimal CanceledCardAmount { get; set; }
        public decimal RentBalanse { get; set; }
        public DateTime FinishDate { get; set; }
        public List<Chat> Chats { get; set; }
    }
}