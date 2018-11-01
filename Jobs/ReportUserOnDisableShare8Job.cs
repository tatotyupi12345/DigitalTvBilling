using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using DigitalTVBilling.Utils;
using DigitalTVBilling.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using DigitalTVBilling.ListModels;

namespace DigitalTVBilling.Jobs
{
    public class ReportUserOnDisableShare8Job : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //context.Trigger.JobDataMap.GetString("");
            using (DataContext _db = new DataContext())
            {
                _db.Database.CommandTimeout = 6000;
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //SELECT DATEDIFF(minute, GETDATE(), '2006-01-01 00:00:00.0000000');
                        //string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status=0 AND DATEDIFF(day,Convert(varchar(10), getdate(), 126)+' 23:59:0.000',cr.finish_date)=2";
                        //string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id where DATEDIFF(second, GETDATE(), cr.finish_date)<=60 AND DATEDIFF(second, GETDATE(), cr.finish_date)>=30";
                        //string sql = @"SELECT cr.id FROM book.Cards as cr WHERE cr.card_num = '38067026'";
                        string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id where DATEDIFF(day, '" + DateTime.Now + "', cr.finish_date)=0";
                        //string sql = @"SELECT cr.id FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id where abonent_num='9105560' or abonent_num='1086681'";
                        List<Param> Params = _db.Params.ToList();
                        string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                        string username = Params.First(p => p.Name == "SMSPassword").Value;
                        string password = Params.First(p => p.Name == "SMSUsername").Value;
                        int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                        List<Card> cards;
                        MessageTemplate message;// = _db.AutoMessageTemplates.Where(m => m.Name == "ReportDisabling").FirstOrDefault();
                        MessageTemplate message_geo;// = _db.AutoMessageTemplates.Where(m => m.Name == "ReportDisabling_GEO").FirstOrDefault();
                        
                        int[] ids = _db.Database.SqlQuery<int>(sql).ToArray();

                        //original source
                        //cards = _db.Cards.Include("Customer")//.Include("Receiver").Include("Tower")
                        //                            .Where(c => c.CardStatus == CardStatus.Active && c.Customer.Type != CustomerType.Technic)
                        //                            .Where(c => ids.Contains(c.Id))
                        //                            .Select(c => c).ToList();

                        cards = _db.Cards.Where(c => ids.Contains(c.Id)).Where(c => c.CardStatus == CardStatus.Active && c.Customer.Type != CustomerType.Technic).ToList();

                        foreach (Card card in cards)
                        {
                            message = _db.MessageTemplates.Where(m => m.Name == "OnShare8Active").FirstOrDefault();
                            message_geo = _db.MessageTemplates.Where(m => m.Name == "OnShare8Active_GEO").FirstOrDefault();

                            

                            Subscribtion curr_sb = _db.Subscribtions.Where(s => s.CardId == card.Id && s.Status == true).First();
                            List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == curr_sb.Id).ToList();
                            List<Package> curr_packs = new List<Package>();
                            foreach (var sbs in sbp)
                            {
                                List<Package> package = _db.Packages.Where(p => p.Id == sbs.PackageId).ToList();
                                //if (package.RentType != RentType.block)
                                //{
                                //    curr_packs.Add(package);
                                //}
                                if (package.Any(p=>p.Id == 304085))
                                {

                                    CardDetailData _cardt = _db.Cards.Where(c => c.Id == card.Id).Select(c => new CardDetailData
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

                                    double balance = Math.Round((double)Utils.Utils.GetBalance(_cardt.PaymentAmount, _cardt.ChargeAmount), 2);

                                    string messageText = String.Format(message.Desc, card.AbonentNum);
                                    string messageText_Geo = message_geo.Desc;// String.Format(message_geo.MessageText, card.FinishDate.ToString("dd/MM/yyyy"), card.AbonentNum);

                                    CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                    _socket.Connect();

                                    if (!_socket.SendOSDRequest(int.Parse(card.CardNum), messageText_Geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                    {
                                        //_db.MessageNotSendLogs.Add(new MessageNotSendLog
                                        //{
                                        //    CardId = card.Id,
                                        //    MessageId = message_geo.Id,
                                        //    MessageType = MessageType.OSD,
                                        //});
                                    }

                                    _socket.Disconnect();

                                    //_db.SaveChanges();

                                    string phoneto = _db.Customers.Where(cs => cs.Id == card.CustomerId).Select(cu => cu.Phone1).FirstOrDefault();
                                    Task.Run(async () => { await Utils.Utils.sendMessage(phoneto, messageText); }).Wait();
                                    _db.MessageLoggings.Add(new MessageLogging()
                                    {
                                        card_id = card.Id,
                                        tdate = DateTime.Now,
                                        status = MessageLoggingStatus.OnShare8Active,
                                        message_id = message.Id


                                    });
                                    _db.SaveChanges();
                                    break;
                                }
                            }



                            //message.MessageText = String.Format(message.MessageText, card.FinishDate.ToString("dd/MM/yyyy"), card.AbonentNum);
                            //message_geo.MessageText = String.Format(message_geo.MessageText, card.FinishDate.ToString("dd/MM/yyyy"), card.AbonentNum);


                        }
                        Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "დამთავრდა სმს გაგზავნა გათიშვის დღეს 8-₾ აბონენტებისთვის"); }).Wait();
                        Task.Run(async () => { await Utils.Utils.sendMessage("593668668", "დამთავრდა სმს გაგზავნა გათიშვის დღეს 8-₾ აბონენტებისთვის"); }).Wait();
                        Task.Run(async () => { await Utils.Utils.sendMessage("571711305", "დამთავრდა სმს გაგზავნა გათიშვის დღეს 8-₾ აბონენტებისთვის"); }).Wait();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                    }
                }
            }
        }
    }
}