using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class ReturingValueCallModel
    {
        private readonly UserViewModel userView;

        public ReturingValueCallModel(UserViewModel userView) {
            this.userView = userView;
        }

        public CallModel Result()
        {
            return null;
        }
    }
}