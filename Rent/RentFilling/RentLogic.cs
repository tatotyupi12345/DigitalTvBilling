using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static DigitalTVBilling.Rent.RentModel;

namespace DigitalTVBilling.Rent
{
    public class RentLogic
    {
        public RentLogic(ResultRent re)
        {
            this.resultRent = re;
        }
        public ResultRent resultRent { get; set; }
        public void RentSavePayment()
        {
            RentData rentData = new RentData(resultRent._db);
            var cards = rentData.returnCard(resultRent.pay_data);
            RentSmsInfo rentSmS = new RentSmsInfo();
            List<Payment> payments = new List<Payment>();
            List<PayType> _payTypes = rentData.returnPayTypes();
            CardDetailData _cardAmount = rentData.returnCardDetailData(cards.Select(s => s.Id).FirstOrDefault());
            foreach (Card _card in cards)
            {
                var paym = new Payment
                {
                    CardId = _card.Id,
                    UserId = resultRent.user_id,
                    Tdate = DateTime.Now,
                    FileAttach = "",
                    Amount = 0,
                    PayRent = resultRent.pay_data.RentAmount,
                    LogCard = _card.Customer.Name + " " + _card.Customer.LastName + " ის ბარათზე - " + _card.CardNum,
                    LogCardNum = _card.CardNum,
                    LogPayType = _payTypes.FirstOrDefault(p => p.Id == resultRent.pay_data.PayType).Name,
                    PayTypeId = resultRent.pay_data.PayType
                };
                if (_cardAmount.Amount < 0)
                {

                    if ((_cardAmount.Amount + resultRent.pay_data.RentAmount) > 0)
                    {
                        paym.PayRent = (_cardAmount.Amount + resultRent.pay_data.RentAmount);
                        paym.Amount = (_cardAmount.Amount) * (-1);
                        _cardAmount.Amount = (_cardAmount.Amount) * (-1);
                    }
                    else
                    {
                        paym.Amount = resultRent.pay_data.RentAmount;
                        paym.PayRent = 0;
                    }
                }
                payments.Add(paym);
            }
            rentData.SavePayments(payments.Select(s => s).FirstOrDefault(), rentData.SavePayTransaction(resultRent.pay_data, resultRent.fromPay));
            if (_cardAmount.Amount >= 0)
                SetFinishDate(payments.Select(s => s.CardId).FirstOrDefault());
        }

        public DateTime? SetFinishDate(int card_id)
        {
            RentData rentData = new RentData(resultRent._db);
            CardDetailData _card = rentData.returnCardDetailData(card_id);

            if (_card != null)
            {

                decimal balance = Math.Round(_card.PaymentAmount - _card.ChargeAmount, 2);
                decimal amount = (decimal)_card.RentAmount;
                int day = 0;

                if (amount == 0)
                    return null;
                int service_days = rentData.serviceDay();
                while (true)
                {
                    int coeff = service_days;
                    decimal dayly_amount = amount / coeff;
                    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                    if (balance < dayly_amount)
                        break;
                    balance -= dayly_amount;
                    day++;
                }
                FinishDate finishDate = new FinishDate();
                _card.Card.RentFinishDate = finishDate.GenerateFinishDate(_card.CahrgeTime).AddDays(day);

                rentData.saveRentFinisheDate(_card);

                return _card.Card.FinishDate;
            }

            return null;
        }

    }
    public class FinishDate
    {
        public DateTime GenerateFinishDate(string charge_val)
        {
            string[] charge_vals = charge_val.Split(':');
            int hour = int.Parse(charge_vals[0]);
            DateTime f_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, int.Parse(charge_vals[1]), 0);
            return hour < 10 ? f_date.AddDays(1) : f_date;
        }
    }

}