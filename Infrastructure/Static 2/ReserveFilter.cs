using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2
{
    public class ReserveFilter
    {
        private readonly DateTime dateFrom;
        private readonly DateTime dateTo;
        private readonly int user_Id;

        public ReserveFilter(DateTime dateFrom, DateTime dateTo, int user_id)
        {
            this.dateFrom = dateFrom;
            this.dateTo = dateTo;
            user_Id = user_id;
        }
        public string FilterResult()
        {
            if (user_Id != 0)
            {
                return "where [reserve_answer]!=-1 and user_id=" + user_Id + " and tdate between '" + dateFrom.ToString("MM-dd-yyyy 00:01:ss") + "' and '" + dateTo.ToString("MM-dd-yyyy 23:59:ss") + "'";
            }
            else
            {
                return "where [reserve_answer]!=-1 and  tdate between '" + dateFrom.ToString("MM-dd-yyyy 00:01:ss") + "' and '" + dateTo.ToString("MM-dd-yyyy 23:59:ss") + "'";
            }
           
        }
    }
}