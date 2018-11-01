using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter
{
    public class CallPresentation
    {
        public CallPresentation()
        {

        }

        public CallModel EndResult()
        {
            CallLogic callLogic = new CallLogic();
            return callLogic.ReturnResult();

        }

        //public CallModel EndResultOrder(FilterUser filterUser) {
        //    CallLogic callLogic = new CallLogic();
        //    return callLogic.ReturnOrderResult(filterUser);
        //}
        
        //public CallModel EndResultDamage(FilterUser filterUser)
        //{
        //    CallLogic callLogic = new CallLogic();
        //    return callLogic.ReturnDamageResult(filterUser);
        //}
        //public CallModel EndResultCancellation(FilterUser filterUser)
        //{
        //    CallLogic callLogic = new CallLogic();
        //    return callLogic.ReturnCancellationResult(filterUser);
        //}
    }
}