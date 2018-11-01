using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.N_layer_Rent.RentSms
{
    public class RentSMSData
    {
        public RentSMSData(DataContext db)
        {
            this._db = db;
        }
        public DataContext _db { get; set; }
        public CardDetailData returnCardDetailData(int card_id) // card balance
        {

            var ren_amount = decimal.Parse(_db.Params.First(p => p.Name == "Rent").Value);
            var cahrge = _db.Params.First(p => p.Name == "CardCharge").Value;
            return _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
            {
                PaymentAmount = c.Payments.Sum(p => (decimal?)p.PayRent) ?? 0,
                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.RentAmount).Sum() ?? 0,
                Amount = Math.Round((c.Payments.Sum(p => (decimal?)p.Amount) ?? 0) - (c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0), 2),
                Card = c,
                CustomerType = c.Customer.Type,
                IsBudget = c.Customer.IsBudget,
                RentAmount = ren_amount,
                CahrgeTime = cahrge,
                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                CardLogs = c.CardLogs.ToList()
            }).FirstOrDefault();

        }
        
        public decimal returnPayment(int card_id)
        {

            return _db.Payments.Where(c => c.CardId == card_id).OrderByDescending(cc => cc.Id).Select(s => s.PayRent).FirstOrDefault();
        }
        public decimal PaymentAmount(int card_id)
        {
            var pay = _db.Payments.Where(c => c.CardId == card_id).OrderByDescending(cc => cc.Id).Select(s => s).FirstOrDefault();
            return (pay.Amount+pay.PayRent);
        }
        public List<Card> returnSMSJob2(DataContext _db)
        {

            string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id where DATEDIFF(day, GETDATE(), cr.rent_finish_date)=2";
            int[] ids = _db.Database.SqlQuery<int>(sql).ToArray();
            return _db.Cards.Where(c => ids.Contains(c.Id)).Where(c =>c.Customer.Type != CustomerType.Technic).ToList();

        }
        public List<Card> returnSMSJob()
        {

            string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id where DATEDIFF(day, GETDATE(), cr.rent_finish_date)=0";
            int[] ids = _db.Database.SqlQuery<int>(sql).ToArray();
            return _db.Cards.Where(c => ids.Contains(c.Id)).Where(c => c.Customer.Type != CustomerType.Technic).ToList();

        }
       
        public MessageTemplate OnPayRent()
        {
            return _db.MessageTemplates.Where(m => m.Name == "OnPayRent").FirstOrDefault();
        }
        public MessageTemplate OnPayRentNotAmount()
        {
            return _db.MessageTemplates.Where(m => m.Name == "OnPayRentNotAmount").FirstOrDefault();
        }
        public MessageTemplate OnPayRentPaused()
        {
            return _db.MessageTemplates.Where(m => m.Name == "OnPayRentPaused").FirstOrDefault();
        }
        public MessageTemplate OnPayRentJob2()
        {
            return _db.MessageTemplates.Where(m => m.Name == "OnPayRentJob2").FirstOrDefault();
        }
        public MessageTemplate OnPayRentJob()
        {
            return _db.MessageTemplates.Where(m => m.Name == "OnPayRentJob").FirstOrDefault();
        }
    }
}