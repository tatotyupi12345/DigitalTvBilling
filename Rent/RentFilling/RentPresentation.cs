using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Rent
{
    public class RentPresentation
    {
        public int ResultRentSavePayment(RentModel.ResultRent resultRent)
        {
            RentLogic rentLogic = new RentLogic(resultRent);
            rentLogic.RentSavePayment(); 
            return 0;
        }
    }
}