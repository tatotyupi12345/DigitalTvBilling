using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class CardEditCasData
    {
        public int CardId { get; set; }
        public CardStatus Status { get; set; }
        public int CardNum { get; set; }
        public short[] CasIds { get; set; }
        public DateTime FinishDate { get; set; }
        public DateTime OldFinishDate { get; set; }
        public DateTime Date { get; set; }
        public int ResendCard { get; set; }
        public short[] DeactiveCasIds { get; set; }
        public bool unblock { get; set; }
    }
}