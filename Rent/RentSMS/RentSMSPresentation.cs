using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.N_layer_Rent.RentSms
{
    public class RentSMSPresentation
    {
        public RentSMSPresentation()
        {

        }
        public void SendSMS(DataContext _db,int card_id)
        {
            RentSMSLogic smsLogic = new RentSMSLogic();
            smsLogic.SendPaySMS(_db, card_id);
        }
    }
}