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
    public class MessageJob : IJob
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
                TypeValue = type_value
            };
            _db.Loggings.Add(_logging);
            _db.SaveChanges();

            _db.LoggingItems.AddRange(items.Where(c => c.field != null).Select(c => new LoggingItem
            {
                LoggingId = _logging.Id,
                ColumnDisplay = c.field.Replace(':', ' ').Trim(),
                NewValue = c.new_val,
                OldValue = c.old_val
            }));

            return _logging.Id;
        }

        public void Execute(IJobExecutionContext context)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            using (DataContext _db = new DataContext())
            {
                string sql = @"SELECT cr.id FROM book.Cards AS cr 
                                INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status!=4";

                List<Param> Params = _db.Params.ToList();
                string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;
                int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);        
                List<Card> cards;
                List<AutoMessageTemplate> queries = _db.AutoMessageTemplates.ToList();

                _db.Database.CommandTimeout = 6000;
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        foreach (var _query in queries)
                        {
                            int[] ids = _db.Database.SqlQuery<int>(sql + _query.Query).ToArray();

                            Message _mess = new Message
                            {
                                UserId = 1,
                                IsActive = true,
                                Date = DateTime.Now,
                                MessageText = _query.MessageText,
                                MessageType = Convert.ToString((int)_query.MessageType),
                                MessageAbonents = ids.Select(a => new MessageAbonent { CardId = a }).ToList()
                            };
                            _db.Messages.Add(_mess);

                            if (_query.IsDisposable)
                            {
                                _db.AutoMessageTemplates.Remove(_query);
                                _db.Entry(_query).State = EntityState.Deleted;
                            }

                            this.AddLoging(_db,
                                    LogType.Message,
                                    LogMode.Add,
                                    1,
                                    _mess.Id,
                                    "ავტომატური მესიჯი",
                                    new List<LoggingData>()
                              );

                            cards = _db.Cards.Include("Customer")//.Include("Receiver").Include("Tower")
                                            .Where(c => c.CardStatus != CardStatus.Canceled)
                                            .Where(c => ids.Contains(c.Id))
                                            .Select(c => c).ToList();

                            if (_query.MessageType == MessageType.Email || _query.MessageType == MessageType.OSD)
                            {
                                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                _socket.Connect();
                                foreach (Card card in cards)
                                {
                                    _query.MessageText = Utils.Utils.ReplaceMessageTags(_query.MessageText, card, _db);
                                    switch (_query.MessageType)
                                    {
                                        case MessageType.OSD: //osd
                                            {
                                                if (!_socket.SendOSDRequest(int.Parse(card.CardNum), _query.MessageText, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                                {
                                                    _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                                    {
                                                        CardId = card.Id,
                                                        MessageId = _mess.Id,
                                                        MessageType = MessageType.OSD,
                                                    });
                                                }
                                            }
                                            break;
                                        case MessageType.Email:
                                            {
                                                if (!_socket.SendEmailRequest(int.Parse(card.CardNum), _query.MessageText, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                                {
                                                    _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                                    {
                                                        CardId = card.Id,
                                                        MessageId = _mess.Id,
                                                        MessageType = MessageType.Email,
                                                    });
                                                }
                                            }
                                            break;
                                    }
                                }
                                _socket.Disconnect();
                            }
                            _db.SaveChanges();
                            if (_query.MessageType == MessageType.SMS) //sms
                            {
                                Utils.Utils.OnSendSMS(cards, _query.MessageText, username, password, _db, _mess.Id);
                            }
                        }
                        tran.Commit();
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