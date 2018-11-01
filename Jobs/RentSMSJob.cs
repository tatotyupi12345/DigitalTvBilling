using DigitalTVBilling.N_layer_Rent.RentSms;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class RentSMSJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (DataContext _db = new DataContext())
            {
                _db.Database.CommandTimeout = 6000;
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                      
                        RentSMSLogic rentSMSLogic = new RentSMSLogic();
                        rentSMSLogic.SendSMSJob2(_db);
                        rentSMSLogic.SendSMSJob(_db);

                    }
                    catch
                    {

                    }
                }
            }
        }

    }
}