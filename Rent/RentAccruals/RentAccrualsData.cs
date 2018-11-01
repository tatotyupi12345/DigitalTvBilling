using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Rent.OSD;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.N_layer.RentAccruals
{
    public class RentAccrualsData
    {
        public RentAccrualsData(DataContext db)
        {
            this._db = db;
        }
        public DataContext _db { get; set; }
        public CardDetailData ReturncCardIfnfo(int card_id)
        {
            var ren_amount = decimal.Parse(_db.Params.First(p => p.Name == "Rent").Value);
            var cahrge = _db.Params.First(p => p.Name == "CardCharge").Value;
            return _db.Cards.Where(c => c.CardStatus != CardStatus.Canceled && c.Id == card_id).Select(c => new CardDetailData
            {
                CustomerType = c.Customer.Type,
                IsBudget = c.Customer.IsBudget,
                SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                PaymentAmount = c.Payments.Sum(p => (decimal?)p.PayRent) ?? 0,
                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.RentAmount).Sum() ?? 0,
                Card = c,
                RentAmount= ren_amount,
                CahrgeTime=cahrge,
                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                ServiceAmount = c.CardCharges.Where(s => s.Status == CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                WithoutServiceAmount = c.CardCharges.Where(s => s.Status != CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                CardServices = c.CardServices.ToList()
            }).FirstOrDefault();
        }
        public void SaveRentAccruals(CardDetailData _card)
        {
            try
            {
                var ren_amount = decimal.Parse(_db.Params.First(p => p.Name == "Rent").Value);
                int service_days = int.Parse(_db.Params.First(c => c.Name == "ServiceDays").Value);
                _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, RentAmount = Math.Round((decimal)ren_amount / service_days, 2), Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });

                _card.Card.CardStatus = CardStatus.Rent;
                _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;

                _db.SaveChanges();

            }
            catch
            {

            }
        }
        public void saveRentFinisheDate(CardDetailData _card)
        {
            try
            {
                    _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    SendOSD sendOSD = new SendOSD();
                    sendOSD.SendOSDCard(_db, _card);
            }
            catch
            {

            }
        }
        public int serviceDay()
        {
            return Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
        }
        public decimal RentAmount()
        {
            return decimal.Parse(_db.Params.First(p => p.Name == "Rent").Value);
        }
    }
}