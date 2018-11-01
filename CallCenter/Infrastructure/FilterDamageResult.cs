using DigitalTVBilling.CallCenter.InterfaceUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public interface ISearchResult
    {
        string Result();
    }
    public class FilterDamageResult : ISearchResult
    {
        private readonly IFilterUser _FilterUser;
        private readonly DateFromTo dateFrom;

        public FilterDamageResult(IFilterUser _filterUser, DateFromTo dateFrom)
        {
            _FilterUser = _filterUser;
            this.dateFrom = dateFrom;
           // _FilterUser = _filterUser;
        }
        public string Result()
        {
            if (_FilterUser.check_user == true)
            {
                return " (" + dateFrom.Datetime() + " and executor_id=" + _FilterUser.user_id + ") or (change_date='2222-12-12 00:00:00.000') ";
            }
            else
            {
                return "" + dateFrom.Datetime() + " and status!=7 and executor_id=" + _FilterUser.user_id;
            }
        }
    }

}