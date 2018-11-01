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
using System.Globalization;
using System.Threading.Tasks;

namespace DigitalTVBilling.Jobs
{
    public class CardsChargeJob : IJob
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
                        //get parameters
                        var parameters = _db.Params.ToList();
                        string[] charge_vals = parameters.First(c => c.Name == "CardCharge").Value.Split(':');
                        string[] address = parameters.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                        int closed_card_days = int.Parse(parameters.First(c => c.Name == "ClosedCardDays").Value);
                        decimal closed_daily_amount = decimal.Parse(parameters.First(c => c.Name == "CloseCardDailyAmount").Value);
                        int closed_daily_amount_limit = int.Parse(parameters.First(c => c.Name == "CloseCardDailyAmountLimit").Value);
                        decimal jurid_limit_months = decimal.Parse(parameters.First(c => c.Name == "JuridLimitMonths").Value);
                        int service_days = int.Parse(parameters.First(c => c.Name == "ServiceDays").Value);
                        decimal close_amount = decimal.Parse(parameters.First(p => p.Name == "CloseCardAmount").Value);
                        int free_days = Convert.ToInt32(parameters.First(p => p.Name == "FreeDays").Value);
                        int block_card_days = Convert.ToInt32(parameters.First(p => p.Name == "BlockCardDays").Value);
                        int? closed_card_limit = Convert.ToInt32(parameters.First(p => p.Name == "ClosedCardsLimit").Value);
                        int jurid_pay_waiting_day = Convert.ToInt32(parameters.First(p => p.Name == "JuridPayWaitingDay").Value);
                        decimal rent_amount = Convert.ToInt32(parameters.First(p => p.Name == "RentAmount").Value);
                        decimal pen_daily_amount = rent_amount / (decimal)service_days;
                        DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                        CASSocket cas_socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };

                        var _cards = _db.Cards.Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                        {
                            CustomerType = c.Customer.Type,
                            SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                            CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            Card = c,
                            MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice)
                        }).ToList();

                        cas_socket.Connect();
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Closed))
                        {
                            Subscribtion sb = _db.Subscribtions.Where(s => s.CardId == _card.Card.Id && s.Status == true).First();
                            List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Include("Package").Where(s => s.SubscriptionId == sb.Id).ToList();
                            {
                                if (sbp.Any(p => p.Package.RentType == RentType.rent))
                                {
                                    if ((today - _card.Card.CloseDate).Days <= closed_card_limit)
                                    {
                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = Math.Round((decimal)pen_daily_amount, 2), Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });
                                    }
                                    else
                                    {
                                        if (!cas_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), false, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                        {
                                            _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                            {
                                                Date = today,
                                                CardNum = _card.Card.CardNum,
                                                ChargeCrushLogType = ChargeCrushLogType.Block,
                                                Text = _card.Card.CardNum + ": ბარათი ვერ დაიბლოკა! Cas შეცდომის კოდი: " + cas_socket.error_code + " , Exeption Message: " + cas_socket.ErrorEx
                                            });
                                        }
                                        else
                                        {
                                            CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.Blocked, UserId = 1, CardStatus = _card.Card.CardStatus };
                                            _db.CardLogs.Add(_log);

                                            _card.Card.CardStatus = CardStatus.Blocked;
                                            _db.Entry(_card.Card).State = EntityState.Modified;

                                            Logging _logging = new Logging
                                            {
                                                Tdate = DateTime.Now,
                                                Type = LogType.Card,
                                                UserId = 1,
                                                Mode = LogMode.CardDeal,
                                                TypeId = _card.Card.Id,
                                                TypeValue = _card.Card.AbonentNum + " - ბარათი დაიბლოკა"
                                            };
                                            _db.Loggings.Add(_logging);
                                            _db.SaveChanges();

                                            _db.LoggingItems.AddRange(new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაიბლოკა", old_val = "" } }.Where(c => c.field != null).Select(c => new LoggingItem
                                            {
                                                LoggingId = _logging.Id,
                                                ColumnDisplay = c.field.Replace(':', ' ').Trim(),
                                                NewValue = c.new_val,
                                                OldValue = c.old_val
                                            }));
                                        }
                                    }
                                }
                            }
                        }
                        _db.SaveChanges();
                        
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Paused))
                        {
                            if (_card.Card.PauseDate.AddDays(_card.Card.PauseDays) <= today)
                            {
                                bool status_sign = (_card.PaymentAmount - _card.ChargeAmount) >= (decimal)_card.MinPrice / 30m;

                                _card.Card.CardStatus = status_sign ? CardStatus.Active : CardStatus.Closed;
                                _card.Card.PauseDays = 0;


                                CardLog _log = new CardLog();

                                if (_card.Card.CardStatus == CardStatus.Closed)
                                {
                                    _log.CardId = _card.Card.Id;
                                    _log.Date = today;
                                    _log.Status = CardLogStatus.Close;
                                    _log.UserId = 1;

                                    _card.Card.CloseDate = today;
                                }
                                else if (_card.Card.CardStatus == CardStatus.Active)
                                {
                                    _log.CardId = _card.Card.Id;
                                    _log.Date = today;
                                    _log.Status = CardLogStatus.Open;
                                    _log.UserId = 1;
                                }

                                _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                                _db.CardLogs.Add(_log);

                                if (!cas_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(today, DateTimeKind.Utc)))
                                {
                                    _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                    {
                                        Date = today,
                                        CardNum = _card.Card.CardNum,
                                        ChargeCrushLogType = Models.ChargeCrushLogType.Open,
                                        Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                    });
                                }
                            }
                        }
                        _db.SaveChanges();

                        
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.FreeDays))
                        {
                            bool is_open = true;
                            if (_card.Card.Tdate.AddDays(free_days) < today)
                            {
                                decimal balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                double amount = _card.SubscribAmount - (_card.SubscribAmount * (double)_card.Card.Discount / 100);
                                if ((double)balance >= amount || _card.CustomerType == CustomerType.Technic)
                                {
                                    _card.Card.CardStatus = CardStatus.Active;
                                    _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Open, UserId = 1 });
                                    _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                                    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                }
                                else
                                {
                                    is_open = false;
                                    _card.Card.CardStatus = CardStatus.Closed;
                                    _card.Card.CloseDate = today;
                                    _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                                    _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Close, UserId = 1 });
                                }

                                if (is_open)
                                {
                                    if (!cas_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.Now.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                    {
                                        _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                        {
                                            Date = today,
                                            CardNum = _card.Card.CardNum,
                                            ChargeCrushLogType = is_open ? ChargeCrushLogType.EntitlementOpen : ChargeCrushLogType.EntitlementClose,
                                            Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                        });
                                    }
                                }
                            }
                        }
                        _db.SaveChanges();

                        
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Active))
                        {

                            if (_card.CustomerType != CustomerType.Juridical)
                            {
                                if (_card.Card.FinishDate <= today && _card.CustomerType != CustomerType.Technic)
                                {
                                    _card.Card.CardStatus = CardStatus.Closed;
                                    _card.Card.CloseDate = today;
                                    _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;

                                    _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Close, UserId = 1 });
                                }

                                if (_card.Card.CardStatus == CardStatus.Active && _card.CustomerType != CustomerType.Technic) 
                                {

                                    decimal amount = (decimal)(_card.SubscribAmount / service_days / Utils.Utils.divide_card_charge_interval);

                                    amount -= (amount * (decimal)_card.Card.Discount / 100);
                                    if (amount <= 0)
                                        continue;
                                    _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = today, Status = CardChargeStatus.Daily });
                                }

                            }
                            else
                            {
                                int pay_count = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count();
                                double min_price = pay_count == 1 ? _card.SubscribAmount : _card.MinPrice;
                                min_price -= (min_price * (double)_card.Card.Discount / 100);

                                decimal balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                DateTime _dt = DateTime.Now;

                                if (_dt.Day == 1)
                                {
                                    balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                    if (balance >= 0)
                                        Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                }

                                int diff_days = (_card.Card.FinishDate - _dt).Days;

                                if (_dt.Day > 1 && _dt.Day < jurid_pay_waiting_day)
                                {
                                    if (diff_days < 30)
                                    {
                                        balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                        if (balance >= 0)
                                        {
                                            Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                        }
                                    }
                                }

                                if (_dt.Day == jurid_pay_waiting_day)
                                {
                                    if (diff_days < 30)
                                    {
                                        DateTime f_datefrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                                        DateTime f_dateto = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10, 0, 0, 0);
                                        var CardCahrge_Balance = _db.CardCharges.Where(c => c.CardId == _card.Card.Id && c.Tdate >= f_datefrom && c.Tdate <= f_dateto).Select(s => s.Amount).Sum();
                                        balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                        balance = balance - CardCahrge_Balance;
                                        if (balance >= 0)
                                        {
                                            Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                        }
                                        else
                                        {
                                            _card.Card.CardStatus = CardStatus.Closed;
                                            _card.Card.CloseDate = today;
                                            _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                                            _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Close, UserId = 1 });
                                        }
                                    }
                                }

                                if (!cas_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.CloseDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                {
                                    _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                    {
                                        Date = today,
                                        CardNum = _card.Card.CardNum,
                                        ChargeCrushLogType = ChargeCrushLogType.EntitlementClose,
                                        Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                    });
                                }

                                if (_card.Card.CardStatus == CardStatus.Active && _card.CustomerType != CustomerType.Technic) 
                                {

                                    decimal amount = (decimal)(_card.SubscribAmount / service_days / Utils.Utils.divide_card_charge_interval);

                                    amount -= (amount * (decimal)_card.Card.Discount / 100);
                                    if (amount <= 0)
                                        continue;
                                    _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = today, Status = CardChargeStatus.Daily });
                                }
                            }
                        }
                        _db.SaveChanges();

                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Blocked))
                        {
                            Subscribtion sb = _db.Subscribtions.Where(s => s.CardId == _card.Card.Id && s.Status == true).First();
                            List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Include("Package").Where(s => s.SubscriptionId == sb.Id).ToList();

                                if (sbp.Any(p => p.Package.RentType == RentType.rent))
                                {
                                    _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = Math.Round((decimal)pen_daily_amount, 2), Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });
                                }
                        }
                        _db.SaveChanges();

                        cas_socket.Disconnect();
                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        using (StreamWriter writer = new StreamWriter("../path_to_error_log_file", true))
                        {
                            writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                               "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                        }
                        using (DataContext _db_log = new DataContext())
                        {
                            _db_log.ChargeCrushLogs.Add(new ChargeCrushLog
                            {
                                Date = DateTime.Now,
                                CardNum = "",
                                ChargeCrushLogType = ChargeCrushLogType.Crush,
                                Text = (ex.Message != null ? ex.Message : "null")
                            });
                            _db_log.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}