using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class HistoryDamagePartialViewModel
    {
        private readonly Damages damages;
        private readonly Users users;

        public HistoryDamagePartialViewModel( Damages damages,Users users)
        {
            this.damages = damages;
            this.users = users;
        }
        public CallModel Execute()
        {
            return new CallModel
            {
                users = users.Result(),
                damage = damages.Result()
            };
        }
    }
}