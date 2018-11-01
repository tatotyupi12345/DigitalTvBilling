using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DigitalTVBilling.Utils;
using Quartz;
using System.IO;
using DigitalTVBilling.Models;
using DigitalTVBilling.ListModels;

namespace DigitalTVBilling.Jobs
{
    public class DisableCardsJob :  IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (DataContext _db = new DataContext())
            {
                _db.Database.CommandTimeout = 10000;
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {

                        //var _cards = _db.Cards.Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                        //{
                        //    CustomerType = c.Customer.Type,
                        //    IsBudget = c.Customer.IsBudget,
                        //    SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                        //    CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                        //    PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                        //    ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        //    Card = c,
                        //    MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                        //    ServiceAmount = c.CardCharges.Where(s => s.Status == CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        //    WithoutServiceAmount = c.CardCharges.Where(s => s.Status != CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        //    CardServices = c.CardServices.ToList()
                        //}).ToList();

                        var _cards = _db.Cards.Where(c => c.CardStatus != CardStatus.Canceled).ToList();
                        int? closed_card_limit = Convert.ToInt32(_db.Params.First(p => p.Name == "ClosedCardsLimit").Value);
                        int? blocked_card_limit = Convert.ToInt32(_db.Params.First(p => p.Name == "BlockedCardsLimit").Value);

                        if (closed_card_limit == null)
                            closed_card_limit = 60;

                        //foreach (Card _card in _cards.Where(c => c.CardStatus == CardStatus.Closed).ToList())
                        //{
                        //    DateTime dtnow = DateTime.Now;

                        //    if (_card != null)
                        //    {
                        //        TimeSpan diffSpan = dtnow - _card.CloseDate;
                        //        if(diffSpan.Days >= closed_card_limit)
                        //        { 
                        //        CardLog _log = new CardLog() { CardId = _card.Id, Date = DateTime.Now, Status = CardLogStatus.Blocked, UserId = 1, CardStatus = _card.CardStatus };
                        //        _db.CardLogs.Add(_log);

                        //        _card.CardStatus = CardStatus.Blocked;
                        //        _db.Entry(_card).State = EntityState.Modified;

                        //        //this.AddLoging(_db,
                        //        //     LogType.Card,
                        //        //     LogMode.CardDeal,
                        //        //     0,
                        //        //     _card.Card.Id,
                        //        //     _card.Card.AbonentNum + " - ბარათი დაიბლოკა",
                        //        //     new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაიბლოკა", old_val = "" } }
                        //        //  );

                        //        //public long AddLoging(DataContext _db, LogType type, LogMode mode, int user_id, long type_id, string type_value, List<LoggingData> items)
                        //        Logging _logging = new Logging
                        //        {
                        //            Tdate = DateTime.Now,
                        //            Type = LogType.Card,
                        //            UserId = 1,
                        //            Mode = LogMode.CardDeal,
                        //            TypeId = _card.Id,
                        //            TypeValue = _card.AbonentNum + " - ბარათი დაიბლოკა"
                        //        };
                        //        _db.Loggings.Add(_logging);
                        //        _db.SaveChanges();

                        //        _db.LoggingItems.AddRange(new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაიბლოკა", old_val = "" } }.Where(c => c.field != null).Select(c => new LoggingItem
                        //        {
                        //            LoggingId = _logging.Id,
                        //            ColumnDisplay = c.field.Replace(':', ' ').Trim(),
                        //            NewValue = c.new_val,
                        //            OldValue = c.old_val
                        //        }));

                        //        string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                        //        CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        //        _socket.Connect();
                        //        if (!_socket.SendCardStatus(Convert.ToInt32(_card.CardNum), false, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                        //        {
                        //            throw new Exception();
                        //        }
                        //        _socket.Disconnect();
                        //        }
                        //    }

                            
                        //}
                        
                        foreach (Card _card in _cards.Where(c => c.CardStatus == CardStatus.Blocked).ToList())
                        {
                            DateTime dtnow = DateTime.Now;

                            if (_card != null)
                            {
                                TimeSpan diffSpan = dtnow - _card.CloseDate;

                                if (diffSpan.Days >= blocked_card_limit)
                                {
                                    _card.CardStatus = CardStatus.Discontinued;
                                    _db.Entry(_card).State = EntityState.Modified;

                                    CardLog _log = new CardLog() { CardId = _card.Id, Date = DateTime.Now, Status = CardLogStatus.Discontinued, UserId = 1 };
                                    _db.CardLogs.Add(_log);

                                    //this.AddLoging(_db,
                                    //    LogType.Card,
                                    //    LogMode.CardDeal,
                                    //    user_id,
                                    //    _card.Id,
                                    //    _card.CardNum + " ის გაუქმება",
                                    //    new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათის გაუქმება", old_val = "" } }
                                    // );

                                    Logging _logging = new Logging
                                    {
                                        Tdate = DateTime.Now,
                                        Type = LogType.Card,
                                        UserId = 2,
                                        Mode = LogMode.CardDeal,
                                        TypeId = _card.Id,
                                        TypeValue = _card.CardNum + " ის შეწყვეტა"
                                    };
                                    _db.Loggings.Add(_logging);
                                    _db.SaveChanges();

                                    _db.LoggingItems.AddRange(new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათის შეწყევტა", old_val = "" } }.Where(c => c.field != null).Select(c => new LoggingItem
                                    {
                                        LoggingId = _logging.Id,
                                        ColumnDisplay = c.field.Replace(':', ' ').Trim(),
                                        NewValue = c.new_val,
                                        OldValue = c.old_val
                                    }));

                                    _db.SaveChanges();
                                }
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();

                        File.AppendAllText(@"C:\DigitalTVBilling\log.txt", "DisableCardsJob error: " + ex.InnerException.InnerException.Message);
                        //using (DataContext _db_log = new DataContext())
                        //{
                        //    _db_log.ChargeCrushLogs.Add(new ChargeCrushLog
                        //    {
                        //        Date = DateTime.Now,
                        //        CardNum = "",
                        //        ChargeCrushLogType = ChargeCrushLogType.Crush,
                        //        Text = ex.Message
                        //    });
                        //    _db_log.SaveChanges();
                        //}
                    }
                }
            }
        }
    }
}