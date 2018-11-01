using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using static DigitalTVBilling.Utils.SendMiniSMS;

namespace DigitalTVBilling.Jobs
{
    public class AutoSubscribJob : IJob
    {
        private long AddLoging(DataContext _db, LogType type, LogMode mode, int user_id, long type_id, string type_value, List<LoggingData> items)
        {
            Logging _logging = new Logging
            {
                Tdate = DateTime.Now,
                Type = type,
                UserId = user_id,
                Mode = mode,
                TypeId = type_id,
                TypeValue = type_value,
                LoggingItems = items.Where(c => c.field != null).Select(c => new LoggingItem
                {
                    ColumnDisplay = c.field.Replace(':', ' ').Trim(),
                    NewValue = c.new_val,
                    OldValue = c.old_val
                }).ToList()
            };
            _db.Loggings.Add(_logging);
            _db.SaveChanges();

            return _logging.Id;
        }

        public void Execute(IJobExecutionContext context)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            using (DataContext _db = new DataContext())
            {
                decimal jurid_limit_months = int.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value);
                string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();

                _db.Database.CommandTimeout = 6000;
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        List<AutoSubscribChangeCard> _subscribs = _db.AutoSubscribChangeCards.Include("Card").Where(c => today.Day >= c.SendDate.Value.Day && today.Month == c.SendDate.Value.Month && today.Year == c.SendDate.Value.Year).ToList();//.Where(c => c.SendDate.HasValue ? DbFunctions.DiffDays(today, c.SendDate.Value) == 0 : DbFunctions.DiffDays(today, c.Card.FinishDate) < 1).ToList();
                        foreach (AutoSubscribChangeCard _subscrib in _subscribs)
                        {
                            int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                            List<Param> _params = _db.Params.ToList();
                            string charge_time = _params.Where(p => p.Name == "CardCharge").First().Value;
                            int free_days = Convert.ToInt32(_params.Where(p => p.Name == "FreeDays").Select(p => p.Value).First());
                            //Card card = _db.Cards.Where(c => c.Id == _subscrib.CardId).First();
                            //original line
                            //card.FinishDate = Utils.Utils.GenerateFinishDate(card.Tdate, charge_time).AddDays(free_days);

                            SendMiniSMS sendMiniSMS = new SendMiniSMS();

                            short[] casIds = _subscrib.CasIds.Split(',').Select(c => short.Parse(c)).ToArray();
                            short[] old_casIds = { };

                            Subscribtion updSubscrbs = _db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == _subscrib.CardId).OrderByDescending(s => s.Tdate).FirstOrDefault();
                            if (updSubscrbs.SubscriptionPackages.Where(s => s.PackageId == 304071).FirstOrDefault() == null)
                            {


                                if (updSubscrbs != null)
                                {
                                    updSubscrbs.Status = false;
                                    _db.Entry(updSubscrbs).State = System.Data.Entity.EntityState.Modified;

                                    old_casIds = updSubscrbs.SubscriptionPackages.Select(c => (short)c.Package.CasId).ToArray();
                                }
                                _db.SaveChanges();

                                Subscribtion _new_sub = new Subscribtion()
                                {
                                    Amount = _subscrib.Amount,
                                    CardId = _subscrib.CardId,
                                    Status = true,
                                    Tdate = DateTime.Now,
                                    UserId = 1 /*_subscrib.UserId*/,
                                    SubscriptionPackages = _subscrib.PackageIds.Split(',').Select(s => new SubscriptionPackage { PackageId = int.Parse(s) }).ToList()
                                };
                                _db.Subscribtions.Add(_new_sub);
                                _db.SaveChanges();
                                //if (!_socket.SendEntitlementRequest(int.Parse(_subscrib.Card.CardNum), old_casIds, DateTime.SpecifyKind(_subscrib.Card.CasDate, DateTimeKind.Utc), false))
                                //{
                                //    throw new Exception("ბარათის პაკეტები ვერ შაიშალა:" + _subscrib.Card.CardNum);
                                //}

                                if (!_socket.SendEntitlementRequest(Convert.ToInt32(_subscrib.Card.CardNum), old_casIds, _subscrib.Card.FinishDate.AddHours(-4), _subscrib.Card.FinishDate.AddHours(-4), false))
                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                {
                                    throw new Exception("ბარათის პაკეტები ვერ წაიშალა:" + _subscrib.Card.CardNum);
                                }
                                sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(_subscrib.Card.CardNum), _subscrib.Card.Id, old_casIds, _subscrib.Card.FinishDate.AddHours(-4), _subscrib.Card.FinishDate.AddHours(-4), (int)-2, false, (int)StatusMiniSMS.AutoPackages);
                                //_subscrib.Card.CasDate = DateTime.Now;
                                //_db.Entry(_subscrib.Card).State = EntityState.Modified;
                                //_db.SaveChanges();

                                AddLoging(_db, LogType.CardPackage, LogMode.Add, 1 /*_subscrib.UserId*/, _subscrib.CardId, "ავტომატური პაკეტის მიბმა ბარათზე:" + _subscrib.Card.AbonentNum, new List<LoggingData>());

                                //nonoriginal line
                                _db.Entry(_subscrib.Card).State = EntityState.Modified;
                                _db.SaveChanges();

                                //_db.SaveChanges();
                                //Utils.Utils.SetFinishDate(_db, decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value), _subscrib.CardId);

                                var card = _db.Cards.Where(c => c.Id == _subscrib.CardId).Include("Customer").Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                                {
                                    PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                                    ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                    Card = c,
                                    CustomerType = c.Customer.Type,
                                    IsBudget = c.Customer.IsBudget,
                                    SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                                    MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                    CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId)
                                }).FirstOrDefault();

                                if (card != null)
                                {


                                    _db.AutoSubscribChangeCards.Remove(_subscrib);
                                    _db.Entry(_subscrib).State = EntityState.Deleted;
                                    _db.SaveChanges();

                                    if (_db.Customers.Where(c => c.Id == card.Card.CustomerId).Single().Type != CustomerType.Juridical && card.Card.CardStatus != CardStatus.Paused)
                                    {
                                        card.Card.CasDate = DateTime.Now;
                                        _db.Entry(card.Card).State = EntityState.Modified;
                                        _db.SaveChanges();

                                        decimal balance = Utils.Utils.GetBalance(card.PaymentAmount, card.ChargeAmount);
                                        decimal amount = (decimal)card.SubscribAmount;
                                        decimal _amount_coef = amount / service_days;
                                        if (balance >= _amount_coef)
                                        {
                                            Utils.Utils.SetFinishDate(_db, jurid_limit_months, card.Card.Id);

                                            card.Card.CardStatus = CardStatus.Active;
                                            _db.Entry(card.Card).State = EntityState.Modified;
                                            //_db.CardLogs.Add(new CardLog() { CardId = card.Card.Id, Date = today, Status = CardLogStatus.Open, UserId = 1 });
                                            _db.SaveChanges();

                                            //if (!_socket.SendEntitlementRequest(int.Parse(_subscrib.Card.CardNum), casIds, DateTime.SpecifyKind(_subscrib.Card.CasDate, DateTimeKind.Utc), true))
                                            if (!_socket.SendEntitlementRequest(int.Parse(card.Card.CardNum), casIds, card.Card.CloseDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), true))
                                            {
                                                throw new Exception("ბარათის პაკეტები ვერ გააქტიურდა:" + card.Card.CardNum);
                                            }
                                            sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(card.Card.CardNum), card.Card.Id, card.CasIds.ToArray(), card.Card.CloseDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), (int)-2, true, (int)StatusMiniSMS.AutoPackages);
                                        }
                                        else
                                        {
                                            card.Card.CardStatus = CardStatus.Closed;
                                            card.Card.CloseDate = DateTime.Now;
                                            //card.Card.FinishDate = DateTime.Now;
                                            _db.Entry(card.Card).State = EntityState.Modified;
                                            _db.CardLogs.Add(new CardLog() { CardId = card.Card.Id, Date = today, Status = CardLogStatus.Close, UserId = 1 });
                                            _db.SaveChanges();
                                        }
                                    }
                                }
                            }

                            else
                            {
                                _db.AutoSubscribChangeCards.Remove(_subscrib);
                                _db.Entry(_subscrib).State = EntityState.Deleted;
                                _db.SaveChanges();
                            }
                        }
                        tran.Commit();
                        
                    }
                    catch (Exception ex)
                    {

                        tran.Rollback();
                        Utils.Utils.ErrorLogging(ex, @"C:\DigitalTV\AutoSubscribLog.txt");
                    }
                }

                _socket.Disconnect();
            }
        }
    }
}