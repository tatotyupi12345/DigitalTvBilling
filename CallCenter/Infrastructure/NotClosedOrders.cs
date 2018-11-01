using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class NotClosedOrders
    {
        private readonly FilterUser _FilterUser;
        private readonly DateFrom dateFrom;

        public NotClosedOrders(FilterUser _filterUser ,DateFrom dateFrom)
        {
            _FilterUser = _filterUser;
            this.dateFrom = dateFrom;
        }
            public string Result()
            {
                return " status=0 ";
            }
        
    }
}