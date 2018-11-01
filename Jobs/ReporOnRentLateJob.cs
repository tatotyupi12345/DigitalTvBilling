using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class ReporOnRentLateJob : IJob
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
                        int limit_days = 60;
                        int message_interval_days = 7;
                        //SELECT DATEDIFF(minute, GETDATE(), '2006-01-01 00:00:00.0000000');
                        //string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status=0 AND DATEDIFF(day,Convert(varchar(10), getdate(), 126)+' 23:59:0.000',cr.finish_date)=2";
                        string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id where DATEDIFF(second, GETDATE(), cr.finish_date)<0 AND DATEDIFF(second, GETDATE(), cr.finish_date)>-120";
                        //string sql = @"SELECT cr.id FROM book.Cards as cr WHERE cr.card_num = '38067026'";

                        List<Param> Params = _db.Params.ToList();
                        string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                        string username = Params.First(p => p.Name == "SMSPassword").Value;
                        string password = Params.First(p => p.Name == "SMSUsername").Value;
                        int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                        //List<Card> cards;
                        MessageTemplate message = _db.MessageTemplates.Where(m => m.Name == "RentLateMessage").FirstOrDefault();
                        MessageTemplate message_geo = _db.MessageTemplates.Where(m => m.Name == "RentLateMessage_GEO").FirstOrDefault();

                        int[] ids = _db.Database.SqlQuery<int>(sql).ToArray();

                        //cards = _db.Cards.Include("Customer")//.Include("Receiver").Include("Tower")
                        //                            .Where(c => c.CardStatus == CardStatus.Active && c.Customer.Type != CustomerType.Technic)
                        //                            .Where(c => ids.Contains(c.Id))
                        //                            .Select(c => c).ToList();

                        var cards = _db.Cards.Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
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

                        foreach (CardDetailData _card in cards.Where(c => c.Card.CardStatus == CardStatus.Closed))
                        {
                            Subscribtion sb = _db.Subscribtions.Where(s => s.CardId == _card.Card.Id).First();
                            SubscriptionPackage sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == sb.Id).First();
                            Package package = _db.Packages.Where(p => p.Id == sbp.PackageId).First();
                            string phone = _db.Customers.Where(c => c.Id == _card.Card.CustomerId).First().Phone1;
                            double balance = Math.Round((double)Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount), 2);

                            if (package.RentType == RentType.rent)
                            {
                                if ((DateTime.Now - _card.Card.CloseDate).Days == limit_days-2)
                                {
                                    MessageTemplate before_block_message = _db.MessageTemplates.Single(m => m.Name == "BeforeRentLateMessage");
                                    string before_block_message_desc = String.Format(before_block_message.Desc, Math.Round(balance * -1 + _card.SubscribAmount, 2));
                                    Task.Run(async () => { await Utils.Utils.sendMessage(phone, before_block_message_desc); }).Wait();
                                }
                            }
                        }

                        foreach (CardDetailData _card in cards.Where(c => c.Card.CardStatus == CardStatus.Blocked))
                        {
                            Subscribtion sb = _db.Subscribtions.Where(s => s.CardId == _card.Card.Id).First();
                            SubscriptionPackage sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == sb.Id).First();
                            Package package = _db.Packages.Where(p => p.Id == sbp.PackageId).First();

                            string phone = _db.Customers.Where(c => c.Id == _card.Card.CustomerId).First().Phone1;
                            string message_desc = String.Format(message.Desc, _card.Card.CardNum);
                            string message_desc_geo = String.Format(message_geo.Desc, _card.Card.CardNum);
                            

                            if (package.RentType == RentType.rent)
                            {
                                //if ((DateTime.Now - _card.Card.CloseDate).Days > 1)
                                {
                                    //if ((DateTime.Now - _card.Card.CloseDate).Days == limit_days)
                                    //{
                                    //    MessageTemplate block_message = _db.MessageTemplates.Single(m => m.Name == "OnRentLateLimit");
                                    //    string block_message_desc = message.Desc; // String.Format(message.Desc, Math.Round((balance), 2));
                                    //    Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                                    //}

                                    if ((DateTime.Now - _card.Card.CloseDate).Days == limit_days + 1)
                                    {
                                        //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                        //_socket.Connect();

                                        //if (!_socket.SendOSDRequest(int.Parse(_card.Card.CardNum), message_desc_geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                        //{
                                        //    _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                        //    {
                                        //        CardId = _card.Card.Id,
                                        //        MessageId = message_geo.Id,
                                        //        MessageType = MessageType.OSD,
                                        //    });
                                        //}

                                        //_socket.Disconnect();

                                        //_db.SaveChanges();

                                        //Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();

                                        MessageTemplate block_message = _db.MessageTemplates.Single(m => m.Name == "OnRentLateLimit");
                                        string block_message_desc = block_message.Desc; // String.Format(message.Desc, Math.Round((balance), 2));
                                        Task.Run(async () => { await Utils.Utils.sendMessage(phone, block_message_desc); }).Wait();

                                        Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();

                                    }
                                    if ((DateTime.Now - _card.Card.CloseDate).Days == limit_days + 1 + message_interval_days)
                                    {
                                        //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                        //_socket.Connect();

                                        //if (!_socket.SendOSDRequest(int.Parse(_card.Card.CardNum), message_desc_geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                        //{
                                        //    _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                        //    {
                                        //        CardId = _card.Card.Id,
                                        //        MessageId = message_geo.Id,
                                        //        MessageType = MessageType.OSD,
                                        //    });
                                        //}

                                        //_socket.Disconnect();

                                        //_db.SaveChanges();

                                        Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                                    }
                                    if ((DateTime.Now - _card.Card.CloseDate).Days == limit_days + 1 + message_interval_days * 2)
                                    {
                                        //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                        //_socket.Connect();

                                        //if (!_socket.SendOSDRequest(int.Parse(_card.Card.CardNum), message_desc_geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                        //{
                                        //    _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                        //    {
                                        //        CardId = _card.Card.Id,
                                        //        MessageId = message_geo.Id,
                                        //        MessageType = MessageType.OSD,
                                        //    });
                                        //}

                                        //_socket.Disconnect();

                                        //_db.SaveChanges();

                                        Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                                    }
                                    if ((DateTime.Now - _card.Card.CloseDate).Days == limit_days + 1 + message_interval_days * 3)
                                    {
                                        //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                        //_socket.Connect();

                                        //if (!_socket.SendOSDRequest(int.Parse(_card.Card.CardNum), message_desc_geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                        //{
                                        //    _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                        //    {
                                        //        CardId = _card.Card.Id,
                                        //        MessageId = message_geo.Id,
                                        //        MessageType = MessageType.OSD,
                                        //    });
                                        //}

                                        //_socket.Disconnect();

                                        //_db.SaveChanges();

                                        Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                                    }
                                    if ((DateTime.Now - _card.Card.CloseDate).Days == limit_days + 1 + message_interval_days*4)
                                    {
                                        //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                        //_socket.Connect();

                                        //if (!_socket.SendOSDRequest(int.Parse(_card.Card.CardNum), message_desc_geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                        //{
                                        //    _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                        //    {
                                        //        CardId = _card.Card.Id,
                                        //        MessageId = message_geo.Id,
                                        //        MessageType = MessageType.OSD,
                                        //    });
                                        //}

                                        //_socket.Disconnect();

                                        //_db.SaveChanges();

                                        Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                                    }
                                }
                            }
                        }

                        //foreach (CardDetailData card in cards)
                        //{
                        //    CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        //    _socket.Connect();

                        //    if (!_socket.SendOSDRequest(int.Parse(card.Card.CardNum), message_geo.MessageText, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                        //    {
                        //        _db.MessageNotSendLogs.Add(new MessageNotSendLog
                        //        {
                        //            CardId = card.Card.Id,
                        //            MessageId = message_geo.Id,
                        //            MessageType = MessageType.OSD,
                        //        });
                        //    }

                        //    _socket.Disconnect();

                        //    _db.SaveChanges();

                        //    //Task.Run(async () => { await Utils.Utils.sendMessage(card.Customer.Phone1, message.MessageText); }).Wait();
                        //}

                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }
    }
}