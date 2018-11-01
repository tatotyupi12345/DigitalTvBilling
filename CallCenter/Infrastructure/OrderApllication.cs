using DigitalTVBilling.CallCenter.InterfaceUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class OrderApllication
    {
        private readonly IFilterUser filter_User;

        public OrderApllication(IFilterUser filter_user)
        {
            filter_User = filter_user;
        }


        public string Execute()
        {
            string where = "";
            if (filter_User.check_user == true)
            {
                where= " (" + new DateFrom().Datetime() + " and status!=2 and executor_id=" + filter_User.user_id + ") or (change_date='2222-12-12 00:00:00.000') ";
            }
            else
            {
                where= "" + new DateFrom().Datetime() + " and status!=2 and  executor_id=" + filter_User.user_id;
            }
            return where;
        }
    }
}