using DigitalTVBilling.CallCenter.InterfaceUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class HistoryDamageViewModel : ICallcenterModel
    {
        private readonly IUserDamage damageData;
        private readonly IUser users;

        public HistoryDamageViewModel(IUserDamage DamageData, IUser users)
        {
            damageData = DamageData;
            this.users = users;
        }
        public CallModel Execute()
        {
            return new CallModel
            {
                damage = damageData.Execute(),
                users = users.Result()
            };
        }
    }
}