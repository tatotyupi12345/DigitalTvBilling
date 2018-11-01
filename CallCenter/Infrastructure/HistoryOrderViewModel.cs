using DigitalTVBilling.CallCenter.InterfaceUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class HistoryOrderViewModel
    {
        private readonly IUserOrder orderData;
        private readonly IUser users;

        public HistoryOrderViewModel(IUserOrder orderData, IUser users)
        {
            this.orderData = orderData;
            this.users = users;
        }
        public CallModel Execute()
        {
            return new CallModel
            {
                order = orderData.Execute(),
                users = users.Result()
            };
        }
    }
}