using DigitalTVBilling.CallCenter.InterfaceUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class HistoryOrderPartialViewModel : ICallcenterModel
    {
        private readonly Orders orders;
        private readonly Users users;

        public HistoryOrderPartialViewModel(Orders orders,Users users)
        {
            this.orders = orders;
            this.users = users;
        }
        public CallModel Execute()
        {
            return new CallModel
            {
                users=users.Result(),
                order=orders.Result()
            };
        }
    }
}