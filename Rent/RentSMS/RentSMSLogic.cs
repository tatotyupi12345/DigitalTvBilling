using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigitalTVBilling.N_layer_Rent.RentSms
{
    public class RentSMSLogic
    {
        public RentSMSLogic()
        {

        }

        public RentSMSInfo SendSMSPay(DataContext _db, int card_id)
        {
            MessageTemplate msg = null;
            RentSMSInfo rentSmsInfo = new RentSMSInfo();
            RentSMSData rentSMSData = new RentSMSData(_db);
            CardDetailData _card = rentSMSData.returnCardDetailData(card_id);
            if ((_card.Card.CardStatus == CardStatus.Rent ) || _card.Card.CardStatus == CardStatus.Active)
            {
                msg = rentSMSData.OnPayRent();
                if (msg != null)
                    rentSmsInfo.onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(rentSMSData.PaymentAmount(card_id), 2), _card.Card.RentFinishDate.ToString("dd/MM/yyyy"));

                rentSmsInfo.phone = _card.Card.Customer.Phone1;
            }
            if ((_card.Card.CardStatus == CardStatus.Closed && rentSMSData.returnPayment(card_id) <= _card.RentAmount))
            {
                msg = rentSMSData.OnPayRentNotAmount();
                if (msg != null)
                    rentSmsInfo.onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(rentSMSData.PaymentAmount(card_id), 2), Math.Round(_card.RentAmount - rentSMSData.returnPayment( card_id), 2));

                rentSmsInfo.phone = _card.Card.Customer.Phone1;
            }
            if (_card.Card.CardStatus == CardStatus.Paused)
            {
                msg = rentSMSData.OnPayRentPaused();
                if (msg != null)
                    rentSmsInfo.onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(rentSMSData.returnPayment( card_id), 2), _card.Card.PauseDate.ToString("dd/MM/yyyy"));

                rentSmsInfo.phone = _card.Card.Customer.Phone1;
            }
            rentSmsInfo.phone = _card.Card.Customer.Phone1;
            return rentSmsInfo;
        }
        public void SendPaySMS(DataContext _db, int card_id)
        {
            RentSMSInfo rentSMS = SendSMSPay(_db, card_id);
            RentSMSData rentSMSData = new RentSMSData(_db);
            CardDetailData _card = rentSMSData.returnCardDetailData(card_id);
            if (rentSMS.onPayMsg != null)
            {
                Task.Run(async () => { await Utils.Utils.sendMessage(_card.Card.Customer.Phone1, rentSMS.onPayMsg); }).Wait();
                Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "ijaris gadaxda "+ _card.Card.AbonentNum); }).Wait();
                Task.Run(async () => { await Utils.Utils.sendMessage("571711305", "ijaris gadaxda " +_card.Card.AbonentNum); }).Wait();
            }
        }
        public void SendSMSJob2(DataContext _db)
        {
            RentSMSData rentSMSData = new RentSMSData(_db);
            MessageTemplate msg = rentSMSData.OnPayRentJob2();
            var cards = rentSMSData.returnSMSJob2(_db);
            foreach (Card card in cards)
            {

                Task.Run(async () => { await Utils.Utils.sendMessage("599843508"/*card.Customer.Phone1*/, String.Format(msg.Desc, card.RentFinishDate.ToString("dd/MM/yyyy"), card.AbonentNum)); }).Wait();
            }
        }
        public void SendSMSJob(DataContext _db)
        {
            RentSMSData rentSMSData = new RentSMSData(_db);
            MessageTemplate msg = rentSMSData.OnPayRentJob();
            var cards = rentSMSData.returnSMSJob();
            foreach (Card card in cards)
            {

                Task.Run(async () => { await Utils.Utils.sendMessage("599843508"/*card.Customer.Phone1*/, String.Format(msg.Desc, card.RentFinishDate.ToString("dd/MM/yyyy"), card.AbonentNum)); }).Wait();
            }
        }
    }
}