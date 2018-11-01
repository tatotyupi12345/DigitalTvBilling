using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using DigitalTVBilling.ListModels;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class ReportUserOnDisableJob :IJob
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
                        //SELECT DATEDIFF(minute, GETDATE(), '2006-01-01 00:00:00.0000000');
                        //string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status=0 AND DATEDIFF(day,Convert(varchar(10), getdate(), 126)+' 23:59:0.000',cr.finish_date)=2";

                        //ორიგინალი
                        //string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id where DATEDIFF(second, GETDATE(), cr.finish_date)<0 AND DATEDIFF(second, GETDATE(), cr.finish_date)>-120";
                        string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id where DATEDIFF(day, '" + DateTime.Now + "', cr.close_date)=0";

                        //string sql = @"SELECT cr.id FROM book.Cards as cr WHERE cr.card_num = '38067026'
                        List<Param> Params = _db.Params.ToList();
                        string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                        string username = Params.First(p => p.Name == "SMSPassword").Value;
                        string password = Params.First(p => p.Name == "SMSUsername").Value;
                        int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                        List<Card> cards;
                        AutoMessageTemplate message = _db.AutoMessageTemplates.Where(m => m.Name == "ReportOnDisabled").FirstOrDefault();
                        AutoMessageTemplate message_geo = _db.AutoMessageTemplates.Where(m => m.Name == "ReportOnDisabled_GEO").FirstOrDefault();
                        AutoMessageTemplate messageJuridical = _db.AutoMessageTemplates.Where(m => m.Name == "ReportOnDisabledJuridical").FirstOrDefault();


                        int[] ids = _db.Database.SqlQuery<int>(sql).ToArray();



                        var cardjuridical = _db.Cards.Include("Customer")//.Include("Receiver").Include("Tower")
                                                     .Where(c => c.CardStatus == CardStatus.Closed && c.Customer.Type == CustomerType.Juridical)
                                                     .Where(c => ids.Contains(c.Id))
                                                     .Select(c => c).ToList();

                        var juridical = from custAttachs in cardjuridical
                                            let Code = custAttachs.Customer.Phone1
                                            group custAttachs by new { Code = Code }
                                            into cGroup
                                            select cGroup;
                       
                        foreach (var group in juridical)
                        {
                            var phone = group.Distinct().Select(s => s.Customer.Phone1).FirstOrDefault();
                            Task.Run(async () => { await Utils.Utils.sendMessage(phone, String.Format(messageJuridical.MessageText)); }).Wait();
                        }

                        cards = _db.Cards.Include("Customer")//.Include("Receiver").Include("Tower")
                                                    .Where(c => c.CardStatus == CardStatus.Closed && c.Customer.Type != CustomerType.Technic && c.Customer.Type!=CustomerType.Juridical)
                                                    .Where(c => ids.Contains(c.Id))
                                                    .Select(c => c).ToList();

                        foreach (Card card in cards)
                        {
                            CardDetailData _card = _db.Cards.Where(c => c.Id == card.Id).Select(c => new CardDetailData
                            {
                                PaymentAmount = c.Payments.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                Card = c,
                                IsBudget = c.Customer.IsBudget,
                                CustomerType = c.Customer.Type,
                                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                CardLogs = c.CardLogs.ToList()
                            }).FirstOrDefault();

                            double balance = Math.Round((double)Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount), 2);

                            string msg = String.Format(message.MessageText, _card.SubscribAmount - balance);
                            string msg_geo = String.Format(message_geo.MessageText, _card.SubscribAmount - balance);

                            //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            //_socket.Connect();

                            //if (!_socket.SendOSDRequest(int.Parse(card.CardNum), msg_geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                            //{
                            //    _db.MessageNotSendLogs.Add(new MessageNotSendLog
                            //    {
                            //        CardId = card.Id,
                            //        MessageId = message_geo.Id,
                            //        MessageType = MessageType.OSD,
                            //    });
                            //}

                            //_socket.Disconnect();

                            _db.SaveChanges();

                           Task.Run(async () => { await Utils.Utils.sendMessage(card.Customer.Phone1, msg); }).Wait();
                            _db.MessageLoggings.Add(new MessageLogging()
                            {
                                card_id = card.Id,
                                tdate = DateTime.Now,
                                status = MessageLoggingStatus.OnShare8Disabling,
                                message_id = message.Id


                            });
                            _db.SaveChanges();
                        }
                        Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "დამთავრდა სმს გაგზავნა გათიშვის დღეს. აბონენტებისთვის"); }).Wait();
                        Task.Run(async () => { await Utils.Utils.sendMessage("593668668", "დამთავრდა სმს გაგზავნა გათიშვის დღეს. აბონენტებისთვის"); }).Wait();
                        Task.Run(async () => { await Utils.Utils.sendMessage("571711305", "დამთავრდა სმს გაგზავნა გათიშვის დღეს. აბონენტებისთვის"); }).Wait();
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                    }
                }
            }
        }
    }
}