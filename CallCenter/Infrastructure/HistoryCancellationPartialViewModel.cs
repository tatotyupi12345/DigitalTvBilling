using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class HistoryCancellationPartialViewModel
    {
        private readonly Cancellations cancellations;
        private readonly Users users;

        public HistoryCancellationPartialViewModel(Cancellations cancellations,Users users)
        {
            this.cancellations = cancellations;
            this.users = users;
        }
        public CallModel Execute()
        {
            return new CallModel
            {
                users = users.Result(),
                cancellation = cancellations.Result()
            };
        }
    }
}