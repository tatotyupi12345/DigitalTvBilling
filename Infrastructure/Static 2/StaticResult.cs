using DigitalTVBilling.Infrastructure.Static_2.interfaceBox;
using DigitalTVBilling.Infrastructure.Static_2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2
{
    public class StaticResult : IStaticStatusModel
    {
        private readonly OrderDistinguishedAnswers orderDistinguished;
        private readonly DamageDistinguishedAnswers damageDistinguished;

        public StaticResult(OrderDistinguishedAnswers orderDistinguished,DamageDistinguishedAnswers damageDistinguished)
        {
            this.orderDistinguished = orderDistinguished;
            this.damageDistinguished = damageDistinguished;
        }

        public StaticStatusModel Result()
        {
            return new StaticStatusModel
            {
                _OrderDistinguished = orderDistinguished.Result(),
                _DamageDistinguished= damageDistinguished.Result(),

            };
        }
    }

}