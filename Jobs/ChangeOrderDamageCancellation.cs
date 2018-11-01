using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class ChangeOrderDamageCancellation : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    _db.Database.ExecuteSqlCommand("UPDATE book.UserPermissions SET sign=0 , type=4 where type=44 and tag='FREE_INSTALLATION_ACTION'"); //privilegiebis washla
                    _db.Database.ExecuteSqlCommand("UPDATE book.Users SET type=4 where type=44"); //privilegiebis washla
                    var user = _db.Database.SqlQuery<int>("SELECT id FROM book.Users where type=4 or type=44").Select(s => new { x = functionalPrograming(s) }).ToList();
                    //Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "პრივილეგიების წაშლა - " + DateTime.Now.ToString()); }).Wait();
                    //MiniSMSDelete();
                    // Task.Run(async () => { await Utils.Utils.sendMessage(_db.Customers.Where(c => c.Code == "01025019391").FirstOrDefault().Phone1, "MiniSMS წაშლა - " + DateTime.Now.ToString()); }).Wait();
                    // Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "MiniSMS წაშლა - " + DateTime.Now.ToString()); }).Wait();
                }
                catch (Exception ex)
                {
                    var xx = ex;
                }
            }
        }
        public bool functionalPrograming(int item)
        {
            using (DataContext _db = new DataContext())
            {
                _db.Database.ExecuteSqlCommand($"UPDATE doc.Orders  SET change_date ='{DateTime.Now}' where executor_id = {item} and status!={ (int)OrderStatus.Closed } and status!={(int)OrderStatus.Canceled} and is_approved=0 and change_date<='{DateTime.Now}'");
                _db.Database.ExecuteSqlCommand($"UPDATE dbo.Damage  SET change_date ='{DateTime.Now}' where executor_id = {item} and status!={ (int)DamageStatus.Closed }  and is_approved=0 and get_date<='{DateTime.Now}'");
                _db.Database.ExecuteSqlCommand($"UPDATE dbo.Cancellation  SET change_date ='{DateTime.Now}' where executor_id = {item} and status!={ (int)CancleStatus.Closed } and status!={(int)CancleStatus.NotClosed} and is_approved=0 and change_date<='{DateTime.Now}'");

                _db.Database.ExecuteSqlCommand($"UPDATE doc.Orders  SET change_date ='{DateTime.Now}' where executor_id = {0} and status!={ (int)OrderStatus.Closed } and status!={(int)OrderStatus.Canceled} and is_approved=0 and change_date<='{DateTime.Now}'");
                _db.Database.ExecuteSqlCommand($"UPDATE dbo.Damage  SET change_date ='{DateTime.Now}' where executor_id = {0} and status!={ (int)DamageStatus.Closed }  and is_approved=0 and get_date<='{DateTime.Now}'");
                _db.Database.ExecuteSqlCommand($"UPDATE dbo.Cancellation  SET change_date ='{DateTime.Now}' where executor_id = {0} and status!={ (int)CancleStatus.Closed } and status!={(int)CancleStatus.NotClosed} and is_approved=0 and change_date<='{DateTime.Now}'");
            }
            return true;
        }
        public void MiniSMSDelete()
        {
            using (DataContext _db=new DataContext())
            {
                try
                {
                    Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "MiniSMS დაწყება - " + DateTime.Now.ToString()); }).Wait();
                    var __cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.CardStatus != CardStatus.Canceled && (c.CardStatus == CardStatus.Closed)).ToList();
                    var _Card = __cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Price == 12)).Select(c => c).ToList();
                    __cards = __cards.Except(_Card).ToList();
                    string[] address = _db.Params.Where(p => p.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                    int count = 0,error=0;
                    foreach (var item in __cards)
                    {
                        CardDetailData cardDetailData = new CardDetailData();
                        cardDetailData.CasIds = item.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId);
                        for (int i = 1; i <= 30; i++)
                        {
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendEntitlementRequest(Convert.ToInt32(item.CardNum), cardDetailData.CasIds.ToArray(), item.FinishDate.AddHours(-4).AddDays(i), item.FinishDate.AddHours(-4).AddDays(i), false))
                            {
                                error++;
                            }

                            _socket.Disconnect();
                        }
                        count++;

                    }
                    Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "MiniSMS ერორი "+error+" - " + DateTime.Now.ToString()); }).Wait();
                }
                catch(Exception ex) 
                {
                    var xx = ex;
                }
            }
        }

    }
}