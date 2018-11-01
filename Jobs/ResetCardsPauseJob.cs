using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class ResetCardsPauseJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            using (DataContext _db = new DataContext())
            {
                _db.Database.CommandTimeout = 6000;
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        foreach (var card in _db.Cards.Where(c=>c.CardStatus != CardStatus.Canceled && c.CardStatus != CardStatus.Blocked && c.CardStatus != CardStatus.Paused))
                        {
                            var dateSpan = DateTimeSpan.CompareDates(card.Tdate, DateTime.Now);
                            if (dateSpan.Months == 0 && dateSpan.Days == 1)
                            {
                                card.LastPauseType = 0;
                                card.PauseFreeMonthUsed = false;
                                _db.Entry(card).State = System.Data.Entity.EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                    }
                }
            }
        }
    }
}