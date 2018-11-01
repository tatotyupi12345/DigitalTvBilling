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
using DigitalTVBilling.N_layer.RentAccruals;
using static DigitalTVBilling.Utils.SendMiniSMS;

namespace DigitalTVBilling.Jobs
{
    public class ChargeCardJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                AutoSubscribJob autosubscr = new AutoSubscribJob();
                autosubscr.Execute(null);
            }
            catch (Exception ex)
            {

            }
            int coeff = 30;// DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            using (DataContext _db = new DataContext())
            {
                _db.Database.CommandTimeout = 10000;
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {

                        var _params = _db.Params.ToList();
                        string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                        string[] address = _params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                        int closed_card_days = int.Parse(_params.First(c => c.Name == "ClosedCardDays").Value);
                        decimal closed_daily_amount = decimal.Parse(_params.First(c => c.Name == "CloseCardDailyAmount").Value);
                        int closed_daily_amount_limit = int.Parse(_params.First(c => c.Name == "CloseCardDailyAmountLimit").Value);
                        decimal jurid_limit_months = decimal.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);
                        int service_days = int.Parse(_params.First(c => c.Name == "ServiceDays").Value);
                        decimal close_amount = decimal.Parse(_params.First(p => p.Name == "CloseCardAmount").Value);
                        int free_days = Convert.ToInt32(_params.First(p => p.Name == "FreeDays").Value);
                        int block_card_days = Convert.ToInt32(_params.First(p => p.Name == "BlockCardDays").Value);
                        coeff = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                        int? closed_card_limit = Convert.ToInt32(_db.Params.First(p => p.Name == "ClosedCardsLimit").Value);

                        if (closed_card_limit == null)
                            closed_card_limit = 60;

                        File.AppendAllText(@"C:\DigitalTV\log.txt", "start: " + DateTime.Now.ToString() + "\r\n");

                        DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0); //DateTime.Parse("2016-12-16 00:01:00", CultureInfo.InvariantCulture);// 
                        //if (_db.CardCharges.Any(c => c.Tdate == today))
                        //{
                        //    File.AppendAllText(@"C:\TVMobile\log.txt", "return ");
                        //    return;
                        //}
                        var _blocked_cards = _db.Customers.Select(c => new { Cards = c.Cards.Where(p => p.CardStatus != CardStatus.Canceled && p.CardStatus != CardStatus.Blocked) })
                            .Where(c => c.Cards.Any(s => (s.Payments.Select(p => p.Amount).DefaultIfEmpty().Sum() - s.CardCharges.Select(p => p.Amount).DefaultIfEmpty().Sum() < 0 && c.Cards.Any(p => p.CardStatus == CardStatus.Closed))))
                            .SelectMany(c => c.Cards).GroupBy(c => c.CustomerId).ToList();

                        SendMiniSMS sendMiniSMS = new SendMiniSMS();
                        CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        //_socket.Connect();

                        //foreach (var group in _blocked_cards)
                        //{
                        //    if (!group.Any(g => g.CardStatus == CardStatus.Closed && (today - g.CloseDate).Days == block_card_days))
                        //        continue;
                        //    foreach (Card _card in group)
                        //    {
                        //        if (_card.CardStatus == CardStatus.Active)
                        //        {
                        //            //original code
                        //            //decimal amount = (decimal)(_db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == _card.Id).FirstOrDefault(s => s.Status).Amount / coeff);

                        //            decimal amount = (decimal)(_db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == _card.Id).FirstOrDefault(s => s.Status).Amount / coeff/ Utils.Utils.divide_card_charge_interval);
                        //            amount -= (amount * (decimal)_card.Discount / 100);
                        //            if (amount <= 0)
                        //                continue;
                        //            _db.CardCharges.Add(new CardCharge() { CardId = _card.Id, Amount = amount, Tdate = today, Status = CardChargeStatus.Daily });
                        //        }

                        //        _db.CardLogs.Add(new CardLog() { CardId = _card.Id, Date = today, Status = CardLogStatus.BlockToMinusBalance, UserId = 1, CardStatus = _card.CardStatus });

                        //        _card.CardStatus = CardStatus.Blocked;
                        //        _db.Entry(_card).State = System.Data.Entity.EntityState.Modified;

                        //        if (!_socket.SendCardStatus(Convert.ToInt32(_card.CardNum), false, DateTime.SpecifyKind(today, DateTimeKind.Utc)))
                        //        {
                        //            _db.ChargeCrushLogs.Add(new ChargeCrushLog
                        //            {
                        //                Date = today,
                        //                CardNum = _card.CardNum,
                        //                ChargeCrushLogType = Models.ChargeCrushLogType.Block,
                        //                Text = "ბარათი: " + _card.CardNum + ";" + _card.AbonentNum
                        //            });
                        //        }
                        //    }
                        //}

                        _db.SaveChanges();
                        //_socket.Disconnect();


                        var _cards = _db.Cards.Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                        {
                            CustomerType = c.Customer.Type,
                            IsBudget = c.Customer.IsBudget,
                            SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                            CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            Card = c,
                            MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                            ServiceAmount = c.CardCharges.Where(s => s.Status == CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            WithoutServiceAmount = c.CardCharges.Where(s => s.Status != CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            CardServices = c.CardServices.ToList()
                        }).ToList();

                        _socket.Connect();
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Closed))
                        {
                            //if(_card.Card.ClosedIsPen != null)
                            //if ((bool)_card.Card.ClosedIsPen)
                            //{
                            //    if (!(_card.CustomerType == CustomerType.Juridical && _card.IsBudget))
                            //    {
                            //        if (closed_daily_amount > 0)
                            //        {
                            //            if ((today - _card.Card.CloseDate).Days <= closed_daily_amount_limit)
                            //            {
                            //                _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = closed_daily_amount, Tdate = today, Status = CardChargeStatus.PenDaily });
                            //            }
                            //        }
                            //    }

                            //    if ((today - _card.Card.CloseDate).Days == closed_card_days)
                            //    {
                            //        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = close_amount, Tdate = today, Status = CardChargeStatus.Pen });
                            //    }
                            //}

                            Subscribtion sb = _db.Subscribtions.Where(s => s.CardId == _card.Card.Id && s.Status == true).First();
                            List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Include("Package").Where(s => s.SubscriptionId == sb.Id).ToList();
                            //Package package = _db.Packages.Where(p => p.Id == sbp.PackageId).First();

                            //if (_card.CustomerType != CustomerType.Juridical)
                            {
                                //Subscribtion subscribe = _card.Card.Subscribtions.Where(s => s.Status == true).FirstOrDefault();
                                //if(subscribe != null)
                                //{
                                //    if(subscribe.SubscriptionPackages.Any(s => s.Package.Name.Contains("აქცია 8")) && (subscribe.Tdate.Day == today.Day && subscribe.Tdate.Month == today.Month && subscribe.Tdate.Year == today.Year))
                                //    {
                                //        continue;
                                //    }
                                //}

                                if (sbp.Any(p=>p.Package.RentType == RentType.rent))
                                {
                                    if ((today - _card.Card.CloseDate).Days <= closed_card_limit)
                                    {
                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = Math.Round((decimal)0.10, 2), Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });
                                    }
                                    else
                                    {

                                        //this.AddLoging(_db,
                                        //     LogType.Card,
                                        //     LogMode.CardDeal,
                                        //     user_id,
                                        //     _card.Card.Id,
                                        //     _card.Card.AbonentNum + " - ბარათი დაიბლოკა",
                                        //     new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაიბლოკა", old_val = "" } }
                                        //  );


                                        if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), false, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                        {
                                            //throw new Exception(_card.Card.CardNum + ": ბარათი ვერ დაიბლოკა!");

                                            _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                            {
                                                Date = today,
                                                CardNum = _card.Card.CardNum,
                                                ChargeCrushLogType = ChargeCrushLogType.Block,
                                                Text = _card.Card.CardNum + ": ბარათი ვერ დაიბლოკა! Cas შეცდომის კოდი: " + _socket.error_code + " , Exeption Message: " + _socket.ErrorEx
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
                                                TypeValue = _card.Card.AbonentNum + " - ბარათი დაიბლოკა(სისტემა)"
                                            };
                                            _db.Loggings.Add(_logging);
                                            _db.SaveChanges();

                                            _db.LoggingItems.AddRange(new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაიბლოკა(სისტემა)", old_val = "" } }.Where(c => c.field != null).Select(c => new LoggingItem
                                            {
                                                LoggingId = _logging.Id,
                                                ColumnDisplay = c.field.Replace(':', ' ').Trim(),
                                                NewValue = c.new_val,
                                                OldValue = c.old_val
                                            }));

                                            decimal balance = (_card.PaymentAmount - _card.ChargeAmount);
                                            string phone = _db.Customers.Where(c => c.Id == _card.Card.CustomerId).First().Phone1;
                                            //MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "OnBlock");
                                            //string message_desc = message.Desc; // String.Format(message.Desc, Math.Round((balance), 2));
                                            //Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                                        }
                                    }
                                }
                            }
                        }
                        _db.SaveChanges();
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Rent))
                        {
                            if (_card.Card.RentFinishDate <= today && _card.CustomerType != CustomerType.Technic)
                            {
                                _card.Card.CardStatus = CardStatus.Closed;
                                _card.Card.CloseDate = today;
                                _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                                _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = Math.Round((decimal)0.10, 2), Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });
                                _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Close, UserId = 1 });
                            }
                            else
                            {
                                RentAccrualsPresentation rentAccrualsPresentation = new RentAccrualsPresentation(_db);
                                if (rentAccrualsPresentation.RentBalance( _card.Card.Id) >= rentAccrualsPresentation.RentDayAmount())
                                {
                                    rentAccrualsPresentation.CardAccruals( _card);
                                }
                            }

                        }
                        _db.SaveChanges();
                        //paused_cards
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Paused))
                        {
                            if (_card.Card.PauseDate.AddDays(_card.Card.PauseDays) <= today)
                            {
                                bool status_sign = (_card.PaymentAmount - _card.ChargeAmount) >= (decimal)_card.MinPrice / 30m;

                                _card.Card.CardStatus = status_sign ? CardStatus.Active : CardStatus.Closed;
                                _card.Card.PauseDays = 0;


                                CardLog _log = new CardLog(); /*{ CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Open, UserId = 1 };*/
                                RentAccrualsPresentation rentAccrualsPresentation = new RentAccrualsPresentation(_db);
                                if (_card.Card.CardStatus == CardStatus.Closed && rentAccrualsPresentation.RentBalance( _card.Card.Id) >= rentAccrualsPresentation.RentDayAmount())
                                {
                                    rentAccrualsPresentation.CardAccruals( _card);
                                    rentAccrualsPresentation.RentFinishDat( _card);
                                }
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

                                if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(today, DateTimeKind.Utc)))
                                {
                                    _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                    {
                                        Date = today,
                                        CardNum = _card.Card.CardNum,
                                        ChargeCrushLogType = Models.ChargeCrushLogType.Open,
                                        Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                    });
                                }

                                //if (status_sign)
                                //{
                                //    if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false))
                                //    {
                                //        _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                //        {
                                //            Date = today,
                                //            CardNum = _card.Card.CardNum,
                                //            ChargeCrushLogType = Models.ChargeCrushLogType.EntitlementClose,
                                //            Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                //        });
                                //    }

                                //    System.Threading.Thread.Sleep(2000);
                                //    if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                //    {
                                //        _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                //        {
                                //            Date = today,
                                //            CardNum = _card.Card.CardNum,
                                //            ChargeCrushLogType = Models.ChargeCrushLogType.EntitlementOpen,
                                //            Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                //        });
                                //    }
                                //}
                            }
                        }
                        _db.SaveChanges();

                        //montaged c
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.FreeDays))
                        {
                            bool is_open = true;
                            if (_card.Card.Tdate.AddDays(free_days) < today)
                            {
                                decimal balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                double amount = _card.SubscribAmount - (_card.SubscribAmount * (double)_card.Card.Discount / 100);
                                if ((double)balance >= amount || _card.IsBudget || _card.CustomerType == CustomerType.Technic)
                                {
                                    _card.Card.CardStatus = CardStatus.Active;
                                    _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Open, UserId = 1 });
                                    _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                                    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);

                                    //original code
                                    //if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.Now))
                                    //{

                                    //}
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
                                    if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.Now.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                    {
                                        //throw new Exception("ბარათი ვერ გააქტიურდა:" + _card.Card.CardNum);
                                        _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                        {
                                            Date = today,
                                            CardNum = _card.Card.CardNum,
                                            ChargeCrushLogType = is_open ? ChargeCrushLogType.EntitlementOpen : ChargeCrushLogType.EntitlementClose,
                                            Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                        });
                                    }

                                    //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false))
                                    //{
                                    //    _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                    //    {
                                    //        Date = today,
                                    //        CardNum = _card.Card.CardNum,
                                    //        ChargeCrushLogType = is_open ? ChargeCrushLogType.EntitlementOpen : ChargeCrushLogType.EntitlementClose,
                                    //        Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                    //    });
                                    //}
                                }
                            }
                            else
                            {
                                foreach (var c_s in _card.CardServices)
                                {
                                    if (c_s.IsActive || Utils.Utils.GetServiceBalance(_card.PaymentAmount, _card.ServiceAmount, _card.WithoutServiceAmount) < 0)
                                    {
                                        if (c_s.Date.AddDays(service_days).Date <= today.Date)
                                        {
                                            decimal balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                            if (c_s.IsActive)
                                            {
                                                CardCharge _charge = new CardCharge { Tdate = today, CardId = _card.Card.Id, Status = CardChargeStatus.Service, Amount = c_s.Amount };
                                                balance -= _charge.Amount;
                                                _db.CardCharges.Add(_charge);

                                                c_s.IsActive = false;
                                                _db.Entry(c_s).State = EntityState.Modified;
                                            }

                                            if (balance < 0)
                                            {
                                                _card.Card.CardStatus = CardStatus.Blocked;
                                                _db.Entry(_card.Card).State = EntityState.Modified;

                                                _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.BlockToService, UserId = 1 });

                                                if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), false, DateTime.SpecifyKind(today, DateTimeKind.Utc)))
                                                {
                                                    _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                                    {
                                                        Date = today,
                                                        CardNum = _card.Card.CardNum,
                                                        ChargeCrushLogType = ChargeCrushLogType.Block,
                                                        Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                                    });
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        _db.SaveChanges();

                        //active cards
                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Active))
                        {
                            bool has_s = false;
                            foreach (var c_s in _card.CardServices.Where(c => c.IsActive))
                            {
                                if (c_s.Date.AddDays(service_days).Date <= today.Date)
                                {
                                    _db.CardCharges.Add(new CardCharge { Tdate = today, CardId = _card.Card.Id, Status = CardChargeStatus.Service, Amount = c_s.Amount });

                                    c_s.IsActive = false;
                                    _db.Entry(c_s).State = EntityState.Modified;
                                }
                                has_s = true;
                            }
                            _db.SaveChanges();

                            if (has_s)
                            {
                                Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                            }

                            if (_card.CustomerType != CustomerType.Juridical)
                            {
                                if (_card.Card.FinishDate <= today && _card.CustomerType != CustomerType.Technic) //&& _card.CustomerType != CustomerType.Technic
                                {
                                    RentAccrualsPresentation rentAccrualsPresentation = new RentAccrualsPresentation(_db);
                                    if (rentAccrualsPresentation.RentBalance(_card.Card.Id) >= rentAccrualsPresentation.RentDayAmount())
                                    {
                                        rentAccrualsPresentation.CardAccruals(_card);
                                        rentAccrualsPresentation.RentFinishDat( _card);
                                    }
                                    else
                                    {
                                        if (_card.SubscribAmount != 12)
                                        {
                                            _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = Math.Round((decimal)0.10, 2), Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });

                                        }
                                        _card.Card.CardStatus = CardStatus.Closed;
                                        _card.Card.CloseDate = today;
                                        _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;

                                        foreach (var c_s in _card.CardServices.Where(c => c.IsActive))
                                        {
                                            if (c_s.Date.AddDays(service_days).Date > today.Date)
                                            {
                                                _db.CardCharges.Add(new CardCharge { Tdate = today, CardId = _card.Card.Id, Status = CardChargeStatus.Service, Amount = c_s.Amount });
                                                _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.CloseToService, UserId = 1 });

                                                c_s.IsActive = false;
                                                _db.Entry(c_s).State = EntityState.Modified;
                                            }
                                        }

                                        _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Close, UserId = 1 });

                                        //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.FinishDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                        ////if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false))
                                        //{
                                        //    _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                        //    {
                                        //        Date = today,
                                        //        CardNum = _card.Card.CardNum,
                                        //        ChargeCrushLogType = ChargeCrushLogType.EntitlementClose,
                                        //        Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                        //    });
                                        //}
                                    }
                                }

                                if (_card.Card.CardStatus == CardStatus.Active && _card.CustomerType != CustomerType.Technic)  //&& _card.CustomerType != CustomerType.Technic
                                {
                                    //original code
                                    //decimal amount = (decimal)(_card.SubscribAmount / coeff);

                                    decimal amount = (decimal)(_card.SubscribAmount / coeff / Utils.Utils.divide_card_charge_interval);

                                    amount -= (amount * (decimal)_card.Card.Discount / 100);
                                    if (amount <= 0)
                                        continue;
                                    _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = today, Status = CardChargeStatus.Daily });
                                }

                            }
                            else
                            {
                                bool has_inough = false;
                                int pay_count = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count();
                                double min_price = pay_count == 1 ? _card.SubscribAmount : _card.MinPrice;
                                min_price -= (min_price * (double)_card.Card.Discount / 100);

                                decimal balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                DateTime _dt = DateTime.Now;

                                if ((double)balance >= min_price)
                                {
                                    has_inough = true;
                                }

                                if (_dt.Day == 1)
                                {
                                    balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                    if (balance >= 0)
                                        Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                }

                                int diff_days = (_card.Card.FinishDate - _dt).Days;

                                if (_dt.Day > 1 && _dt.Day < 10 /*6*/)
                                {
                                    if (diff_days < 25)
                                    {
                                        balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                        if(balance >=0)
                                        {
                                            Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                        }
                                    }
                                }

                                if (_dt.Day == 10 /*6*/)
                                {
                                    if (diff_days < 25)
                                    {
                                        DateTime f_datefrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                                        DateTime f_dateto = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10, 0, 0, 0);
                                        var CardCahrge_Balance = _db.CardCharges.Where(c => c.CardId == _card.Card.Id && c.Tdate >= f_datefrom && c.Tdate <= f_dateto).Select(s => s.Amount).Sum();
                                        balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                                        balance = balance + CardCahrge_Balance;
                                        if (balance >= 0)
                                        {
                                            Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                        }
                                        else
                                        {
                                            RentAccrualsPresentation rentAccrualsPresentation = new RentAccrualsPresentation(_db); // ijara daricxva qartuli arxebi
                                            if (rentAccrualsPresentation.RentBalance(_card.Card.Id) >= rentAccrualsPresentation.RentDayAmount())
                                            {
                                                rentAccrualsPresentation.CardAccruals(_card);
                                                rentAccrualsPresentation.RentFinishDat(_card);
                                            }
                                            else
                                            {
                                                if (_card.SubscribAmount != 12)
                                                {
                                                    _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = Math.Round((decimal)0.10, 2), Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });

                                                }
                                                _card.Card.CardStatus = CardStatus.Closed;
                                                _card.Card.CloseDate = today;
                                                _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                                                _db.CardLogs.Add(new CardLog() { CardId = _card.Card.Id, Date = today, Status = CardLogStatus.Close, UserId = 1 });
                                            }
                                        }
                                    }
                                }

                                if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.CloseDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false))
                                {
                                    _db.ChargeCrushLogs.Add(new ChargeCrushLog
                                    {
                                        Date = today,
                                        CardNum = _card.Card.CardNum,
                                        ChargeCrushLogType = ChargeCrushLogType.EntitlementClose,
                                        Text = "ბარათი: " + _card.Card.CardNum + ";" + _card.Card.AbonentNum
                                    });
                                }
                                //miniSMS
                                sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(_card.Card.CardNum), _card.Card.Id, _card.CasIds.ToArray(), _card.Card.CloseDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), (int)CardStatus.Active, true, (int)StatusMiniSMS.Charges);

                                if (_card.Card.CardStatus == CardStatus.Active && _card.CustomerType != CustomerType.Technic)  //&& _card.CustomerType != CustomerType.Technic
                                {
                                    //original code
                                    //decimal amount = (decimal)(_card.SubscribAmount / coeff);

                                    decimal amount = (decimal)(_card.SubscribAmount / coeff / Utils.Utils.divide_card_charge_interval);

                                    amount -= (amount * (decimal)_card.Card.Discount / 100);
                                    if (amount <= 0)
                                        continue;
                                    _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = today, Status = CardChargeStatus.Daily });
                                }
                            }

                            //if (has_s)
                            //{
                            //    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                            //}
                        }
                        _db.SaveChanges();

                        foreach (CardDetailData _card in _cards.Where(c => c.Card.CardStatus == CardStatus.Blocked))
                        {
                            Subscribtion sb = _db.Subscribtions.Where(s => s.CardId == _card.Card.Id && s.Status == true).First();
                            List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Include("Package").Where(s => s.SubscriptionId == sb.Id).ToList();
                            //Package package = _db.Packages.Where(p => p.Id == sbp.PackageId).First();

                            //if (_card.CustomerType != CustomerType.Juridical)
                            {
                                if (sbp.Any(p => p.Package.RentType == RentType.rent))
                                {
                                    _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = Math.Round((decimal)0.10, 2), Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });
                                }
                            }
                        }
                        _db.SaveChanges();

                        _socket.Disconnect();
                        tran.Commit();
                        Task.Run(async () => { await Utils.Utils.sendMessage(_db.Customers.Where(c => c.Code == "01025019391").FirstOrDefault().Phone1, "daricxva - " + DateTime.Now.ToString()); }).Wait();
                        Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "daricxva - " + DateTime.Now.ToString()); }).Wait();
                        Task.Run(async () => { await Utils.Utils.sendMessage("593668668", "daricxva - " + DateTime.Now.ToString()); }).Wait();
                        //Utils.Utils.OnSendAutorizeSMS("599610949", "shesrulda", _params.First(p => p.Name == "SMSUsername").Value, _params.First(p => p.Name == "SMSPassword").Value);
                        //Utils.Utils.OnSendAutorizeSMS("598894533", "shesrulda", _params.First(p => p.Name == "SMSUsername").Value, _params.First(p => p.Name == "SMSPassword").Value);
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Utils.Utils.ErrorLogging(ex, @"C:\DigitalTV\ChargeCardLog.txt");
                        File.AppendAllText(@"C:\DigitalTV\log.txt", "\r\n" + (ex.Message!= null?ex.Message:"null") + "\r\n");
                        File.AppendAllText(@"C:\DigitalTV\log.txt", (ex.InnerException != null ? ex.InnerException.Message : "null") + "\r\n");
                        File.AppendAllText(@"C:\DigitalTV\log.txt", (ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : "null") + ": Rollback at - " + DateTime.Now.ToString() + "\r\n");
                        File.AppendAllText(@"C:\DigitalTV\log.txt", "\r\n" + (ex.StackTrace != null ? ex.StackTrace : "null") + "\r\n");
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

                File.AppendAllText(@"C:\DigitalTV\log.txt", "\r\n end: " + DateTime.Now.ToString() + "\r\n");
            }
        }
    }
}