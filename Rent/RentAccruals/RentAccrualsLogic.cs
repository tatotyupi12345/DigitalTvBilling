using DigitalTVBilling.ListModels;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.N_layer.RentAccruals
{
    public class RentAccrualsLogic
    {
        public RentAccrualsLogic( )
        {
           
        }
        
        public void SaveRentAccruals(DataContext _db,CardDetailData _card)
        {
            RentAccrualsData rentAccrualsData = new RentAccrualsData(_db);
            rentAccrualsData.SaveRentAccruals(_card);

        }
        public decimal returnRentBalance(DataContext _db, int card_id)
        {
            RentAccrualsData rentAccrualsData = new RentAccrualsData(_db);
            var cards = rentAccrualsData.ReturncCardIfnfo(card_id);
            return (Math.Round(cards.PaymentAmount- cards.ChargeAmount, 2));
        }
        public DateTime? SetFinishDate(DataContext _db, int card_id)
        {
            RentAccrualsData rentAccrualsData = new RentAccrualsData(_db);
            var _card = rentAccrualsData.ReturncCardIfnfo(card_id);

            if (_card != null)
            {

                decimal balance = Math.Round(_card.PaymentAmount - _card.ChargeAmount, 2);
                decimal amount = (decimal)_card.RentAmount;
                int day = 0;

                if (amount == 0)
                    return null;
                int service_days = rentAccrualsData.serviceDay();
                while (true)
                {
                    int coeff = service_days;// DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                    decimal dayly_amount = amount / coeff;
                    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                    if (balance < dayly_amount)
                        break;
                    balance -= dayly_amount;
                    day++;
                }
                FinishDate finishDate = new FinishDate();
                _card.Card.RentFinishDate = finishDate.GenerateFinishDate(_card.CahrgeTime).AddDays(day);

                rentAccrualsData.saveRentFinisheDate(_card);

                return _card.Card.FinishDate;
            }

            return null;
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
        public decimal RentDayAmount(DataContext _db)
        {
            RentAccrualsData rentAccrualsData = new RentAccrualsData(_db);
            return (decimal)(rentAccrualsData.RentAmount() / rentAccrualsData.serviceDay());
        }
    }
}