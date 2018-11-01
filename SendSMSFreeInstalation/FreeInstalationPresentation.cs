using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.SendSMSFreeInstalation
{
    public class FreeInstalationPresentation
    {
        public void ReturnResultSMS()
        {
            FreeInstalationLogic instalationLogic = new FreeInstalationLogic();
            instalationLogic.ReturnLogicSMS();
        }
    }
}