using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class PostChargeJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (DataContext _db = new DataContext())
            {
                string[] charge_vals = _db.Params.First(c => c.Name == "CardCharge").Value.Split(':');
                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);
                if (_db.CardCharges.Any(c => c.Tdate == today))
                {
                    File.AppendAllText(@"C:\GlobalTV\post.txt", ",exit" + today.ToString());
                    return;
                }

                File.AppendAllText(@"C:\GlobalTV\post.txt", "finish" + today.ToString());
            }

        }
    }
}