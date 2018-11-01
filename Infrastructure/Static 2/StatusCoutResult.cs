using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2
{
    public class StatusCoutResult
    {
        private readonly StatusResult statusResult;

        public StatusCoutResult(StatusResult statusResult)
        {
            this.statusResult = statusResult;
        }

        public object Result()
        {
            return new
            {
                Active=statusResult.Result(),

            };
        }
    }
}