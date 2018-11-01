using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public interface DateFromTo
    {
        string  Datetime();
    }
    public class DateFrom : DateFromTo
    {
        public string Datetime()
        {
            DateTime dfrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime dTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
            return "change_date between '" + dfrom + "' and '" + dTo + "'";
        }
    }
}