using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class CardServicesList
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CardNum { get; set; }
        public DateTime Date { get; set; }
        public CardServicePayType PayType { get; set; }
    }
}