using DigitalTVBilling.Infrastructure.OSD;
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
    public class PromoChangePack_8 :IJob
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

                        string sql = @"SELECT cr.card_id FROM [dbo].[PromoCahngePack] AS cr where DATEDIFF(day, '" + DateTime.Now + "', cr.tdate)=0";
                        List<Param> Params = _db.Params.ToList();
                        string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                        string username = Params.First(p => p.Name == "SMSPassword").Value;
                        string password = Params.First(p => p.Name == "SMSUsername").Value;
                        int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                        List<Card> cards;
                        string messageText = "";
                        MessageTemplate message = new MessageTemplate();// = _db.AutoMessageTemplates.Where(m => m.Name == "ReportDisabling").FirstOrDefault();
                        MessageTemplate message_geo;// = _db.AutoMessageTemplates.Where(m => m.Name == "ReportDisabling_GEO").FirstOrDefault();

                        int[] ids = _db.Database.SqlQuery<int>(sql).ToArray();

                        cards = _db.Cards.Where(c => ids.Contains(c.Id)).Where(c => c.Customer.Type != CustomerType.Technic).ToList();

                        foreach (Card card in cards)
                        {

                            Subscribtion curr_sb = _db.Subscribtions.Where(s => s.CardId == card.Id && s.Status == true).First();
                            List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == curr_sb.Id).ToList();
                            List<Package> curr_packs = new List<Package>();
                            foreach (var sbs in sbp)
                            {
                                List<Package> package = _db.Packages.Where(p => p.Id == sbs.PackageId).ToList();
                            
                                if (package.Any(p => p.Id == 304086))
                                {
                                    if (card.CardStatus == CardStatus.Closed)
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
                                        if (balance <= 0)
                                        {
                                            balance = balance - package.FirstOrDefault().MinPrice;
                                        }
                                        if (balance>0 && balance < package.FirstOrDefault().MinPrice)
                                        {
                                            balance = package.FirstOrDefault().MinPrice-balance;
                                        }
                                        message = _db.MessageTemplates.Where(m => m.Name == "Promo_Change_Active8_false").FirstOrDefault();
                                        messageText = String.Format(message.Desc, balance);
                                        string phoneto = _db.Customers.Where(cs => cs.Id == card.CustomerId).Select(cu => cu.Phone1).FirstOrDefault();
                                        Task.Run(async () => { await Utils.Utils.sendMessage(phoneto, messageText); }).Wait();
                                        _db.MessageLoggings.Add(new MessageLogging()
                                        {
                                            card_id = card.Id,
                                            tdate = DateTime.Now,
                                            status = MessageLoggingStatus.PromoCahngeActive8,
                                            message_id = message.Id


                                        });
                                        _db.SaveChanges();
                                    }
                                    if (card.CardStatus == CardStatus.Active)
                                    {
                                        message = _db.MessageTemplates.Where(m => m.Name == "Promo_Change_Active8").FirstOrDefault();
                                        messageText = String.Format(message.Desc, card.FinishDate);

                                        string phonetos = _db.Customers.Where(cs => cs.Id == card.CustomerId).Select(cu => cu.Phone1).FirstOrDefault();
                                        Task.Run(async () => { await Utils.Utils.sendMessage(phonetos, messageText); }).Wait();
                                        _db.MessageLoggings.Add(new MessageLogging()
                                        {
                                            card_id = card.Id,
                                            tdate = DateTime.Now,
                                            status = MessageLoggingStatus.PromoCahngeClosed8,
                                            message_id = message.Id


                                        });
                                        _db.SaveChanges();

                                        SendOSDRequesSMS sendOSDReques = new SendOSDRequesSMS();
                                        sendOSDReques.SendOSD(card.CardNum, String.Format(_db.MessageTemplates.Where(m => m.Name == "Promo_Change_Active8_Geo").FirstOrDefault().Desc, card.FinishDate.ToString("dd/MM/yyyy")), _db.Params.ToList());
                                        //message_geo = _db.MessageTemplates.Where(m => m.Name == "OnShare8Active_GEO").FirstOrDefault();
                                    }


                                }
                            }


                        }
                        Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "დამთავრდა სმს გაგზავნა პრომოსთვის" + ""); }).Wait();
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}