using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class test :ISearchResult
    {
        private readonly FilterUser _FilterUser;
        private readonly DateFrom dateFrom;

        public test(FilterUser _filterUser, DateFrom dateFrom) {
            _FilterUser = _filterUser;
            this.dateFrom = dateFrom;
        }
        public string Result()
        {
            return "";
        }
        public void Execute()
        {

        }

    }
}