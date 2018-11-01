using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2
{
    public class StatusResult
    {
        private readonly IStaticStatus status_Count;

        public StatusResult(IStaticStatus status_count)
        {
            status_Count = status_count;
        }
        public List<int> Result()
        {
          return  status_Count.Status();
        }
    }
}