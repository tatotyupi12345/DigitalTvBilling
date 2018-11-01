using DigitalTVBilling.SendSMSFreeInstalation;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class ReporFreeInstalation : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            FreeInstalationPresentation freeInstalationPresentation = new FreeInstalationPresentation();
            freeInstalationPresentation.ReturnResultSMS();
        }
    }
}