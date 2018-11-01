using DigitalTVBilling.CallCenter.InterfaceUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class HistoryCancellationViewModel : ICallcenterModel
    {
        private readonly IUserCancellation cancellationData;
        private readonly IUser users;

        public HistoryCancellationViewModel(IUserCancellation CancellationData, IUser users)
        {
            cancellationData = CancellationData;
            this.users = users;
        }
        public CallModel Execute()
        {
            return new CallModel
            {
                cancellation = cancellationData.Execute(),
                users = users.Result()
            };
            
        }
    }
}