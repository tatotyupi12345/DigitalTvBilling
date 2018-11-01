using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class DamageApllication
    {
        private readonly FilterUser filterUser;
        private readonly DamageData returnedDamageData;

        public DamageApllication( FilterUser filterUser, DamageData returnedDamageData  )
        {
            this.filterUser = filterUser;
            this.returnedDamageData = returnedDamageData;
        }
        //public CallModel Execute()
        //{
        //    string where = "";
        //    if (filterUser.check_user == true)
        //    {
        //        where = " (" + new DateFrom().Datetime() + " and executor_id=" + filterUser.user_id + ") or (change_date='2222-12-12 00:00:00.000') ";
        //    }
        //    else
        //    {
        //        where = "" + new DateFrom().Datetime() + " and executor_id=" + filterUser.user_id;
        //    }
        //    return returnedDamageData.Execute(where);
        //}
        //return
        //      PartialView(
        //          "~/Views/CallCenter/_HistoryDamage.cshtml",
        //           new FileterOrderData (
        //                    filterUser,
        //                    new DateFrom()
        // return
        //PartialView(
        //        "~/Views/CallCenter/_HistoryDamage.cshtml",
        //            new DamageApllication(
        //                    filterUser,
        //                    new DamageData(
        //                        new DataContext()
        //                        )
        //                ).Execute()
        //    );
    }
}