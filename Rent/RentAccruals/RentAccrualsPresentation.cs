using DigitalTVBilling.ListModels;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.N_layer.RentAccruals
{
    public class RentAccrualsPresentation
    {
        public RentAccrualsPresentation(DataContext db)
        {
            this._db = db;
        }
        public DataContext _db { get; set; }
        public decimal RentBalance(int card_id)
        {

            RentAccrualsLogic accrualsLogic = new RentAccrualsLogic();
            return accrualsLogic.returnRentBalance(_db, card_id);
        }
        public void CardAccruals(CardDetailData _card)
        {

            RentAccrualsLogic accrualsLogic = new RentAccrualsLogic();
            accrualsLogic.SaveRentAccruals(_db, _card);
        }
        public void RentFinishDat(CardDetailData _card)
        {

            RentAccrualsLogic accrualsLogic = new RentAccrualsLogic();
            accrualsLogic.SetFinishDate(_db, _card.Card.Id);
        }
        public decimal RentDayAmount()
        {
            RentAccrualsLogic accrualsLogic = new RentAccrualsLogic();
            return accrualsLogic.RentDayAmount(_db);
        }
    }
}