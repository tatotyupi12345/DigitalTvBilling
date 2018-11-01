using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Rent.OSD;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DigitalTVBilling.Rent.RentModel;

namespace DigitalTVBilling.Rent
{
    public class RentData
    {
        public RentData(DataContext _db)
        {
            this._db = _db;
        }
        private DataContext _db { get; set; }

        public List<Card> returnCard(PaymentData pay_data)
        {

            return _db.Cards.Include("Customer").Include("Subscribtions").Where(c => pay_data.Cards.Contains(c.Id)).ToList();
        }
        public List<PayType> returnPayTypes()
        {
            return _db.Database.SqlQuery<PayType>("SELECT * FROM book.PayTypes").ToList();
        }
        public long SavePayTransaction(PaymentData pay_data, bool fromPay)
        {

            long pay_transaction_id = 0;
            if (fromPay)
            {
                if (!_db.PayTransactions.Any(t => t.TransactionId == pay_data.TransactionId))
                {
                    PayTransaction pay_tran = new PayTransaction
                    {
                        Amount = pay_data.Amount,
                        Tdate = DateTime.Now,
                        TransactionId = pay_data.TransactionId,
                        PayTransactionCards = pay_data.Cards.Select(c => new PayTransactionCard { CardId = c }).ToList()
                    };
                    _db.PayTransactions.Add(pay_tran);
                    _db.SaveChanges();
                    pay_transaction_id = pay_tran.Id;
                }
                else
                {
                    return 0;
                }
            }

            return pay_transaction_id;
        }
        public void SavePayments(Payment payments, long pay_transaction_id)
        {
            try
            {
                payments.PayTransaction = pay_transaction_id;
                _db.Payments.Add(payments);
                _db.SaveChanges();
            }
            catch
            {

            }
        }
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
        public void saveRentFinisheDate(CardDetailData _card)
        {
            try
            {
                decimal balance = Math.Round(_card.PaymentAmount - _card.ChargeAmount, 2);
                decimal amount = (decimal)_card.RentAmount;
                if ((_card.Amount >= 0 && _card.Card.CardStatus == CardStatus.Rent && balance >= amount) || (_card.Amount >= 0 && _card.Card.CardStatus == CardStatus.Closed && balance >= amount) || (_card.Amount >= 0 && _card.Card.CardStatus == CardStatus.Blocked && balance >= amount))
                {
                    _card.Card.CardStatus = CardStatus.Rent;
                    _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    SendOSD sendOSD = new SendOSD();
                    sendOSD.SendOSDCard(_db, _card);
                }
                else
                {
                    _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            catch
            {

            }
        }
        public int serviceDay()
        {
            return Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
        }
    }

}