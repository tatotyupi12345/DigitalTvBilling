using DigitalTVBilling.CallCenter.InterfaceUser;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class CancellationFileter
    {
        private readonly IFilterUser filterUser;

        public CancellationFileter(IFilterUser filterUser)
        {
            this.filterUser = filterUser;
        }

        public string Execute()
        {
            if (filterUser.check_user == true)
            {
                return  " (" + new DateFrom().Datetime() + " and executor_id=" + filterUser.user_id + ") or ( status=4 ) ";
            }
            else
            {
                return  "" + new DateFrom().Datetime() + " and status!=3 and executor_id=" + filterUser.user_id;
            }
        }
    }
}