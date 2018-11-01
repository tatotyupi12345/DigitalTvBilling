using DigitalTVBilling.Filters;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Web.Mvc.Ajax;
using System.Threading.Tasks;
using DigitalTVBilling.ListModels;
using System.Data;
using System.Globalization;
using System.Data.SqlClient;
using DigitalTVBilling.Helpers;
using System.Threading;
using System.ComponentModel;
using DigitalTVBilling.Jobs;
using System.Text;
using System.Web.UI;
using RazorEngine;
using System.Web.Routing;
using SelectPdf;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Quartz;
using System.Configuration;
using Dapper;
using DigitalTVBilling.Rent.OSD;
using static DigitalTVBilling.Utils.SendMiniSMS;
using DigitalTVBilling.Docs.Contracts;
using System.Text.RegularExpressions;
using DigitalTVBilling.CallCenter;
using DigitalTVBilling.Infrastructure.OSD;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    [RequireHttpsAttribute]

    public class AbonentController : BaseController
    {
        private int pageSize = 20;
        public async Task<ActionResult> Index(int page = 1)
        {
            //ChargeCardJob auto = new ChargeCardJob();
            //auto.Execute(null);
            //SendOSD s = new SendOSD();
            //s.SendLock(0);
            //s.SendUnLock(0);
            //ChangeOrderDamageCancellation change = new ChangeOrderDamageCancellation();
            //change.Execute(null);
            if (!Utils.Utils.GetPermission("ABONENT_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                //List<Param> Params = _db.Params.ToList();
                //string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                //int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                //_socket.Connect();
                //MessageTemplate message_geo;

                //message_geo = _db.MessageTemplates.Where(m => m.Name == "ResetPauseAndActivate_GEO").FirstOrDefault();
                //string onPayMsg_geo = "თქვენმოგეხსნათბარათისპაუზათქვენისერვისიგააქტიურებულიაგააქტიურუუუუტ";
                //var countsa = onPayMsg_geo.Count();
                //if (_socket.SendOSDRequest(int.Parse("63702552"), onPayMsg_geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), 0))
                //{

                //}
                //var xx = 0;
                //using (IDbConnection db= new SqlConnection(
                //                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString))
                //{
                //    var query = (@"SELECT * FROM book.Cards AS c
                //                            inner join book.Customers as cu on cu.id=c.customer_id
                //                            ");

                //   var ccrd= db.Query<_Card,  _Customer,_Card>(query, (c, cu) =>
                //    {
                //        _Card cc;
                //        cc = c;
                //        c._Customers = new List<_Customer>() { cu };


                //        return c;
                //    }).Distinct().ToList();

                //    var ss =0;
                //}
                //ReportUserOnDisableShare8Job userOnDisableShare8Job = new ReportUserOnDisableShare8Job();
                //userOnDisableShare8Job.Execute(null);
                //ChangeOrderDamageCancellation changeOrderDamage = new ChangeOrderDamageCancellation();

                //PromoChangePack_8 changePack_8 = new PromoChangePack_8();
                //changePack_8.Execute(null);
                //AutoSubscribJob autoSubscribJob = new AutoSubscribJob();
                //autoSubscribJob.Execute(null);
                //ReportUserOnDisableJob userOnDisableJob = new ReportUserOnDisableJob();
                //userOnDisableJob.Execute(null);
                //var card_id = 374266;
                //_db.Database.ExecuteSqlCommand("DELETE FROM [doc].[SubscriptionPackages] where subscription_id=" + _db.Subscribtions.Where(c=>c.CardId==card_id).FirstOrDefault().Id + "");

                //_db.Database.ExecuteSqlCommand("DELETE FROM [doc].[AutoSubscribChangeCards] where card_id=" + card_id + "");

                //_db.Database.ExecuteSqlCommand("DELETE FROM [doc].[Subscribes] where card_id=" + card_id + "");

                //_db.Database.ExecuteSqlCommand("DELETE FROM [doc].[CardLogs] where card_id=" + card_id + "");

                //_db.Database.ExecuteSqlCommand("DELETE FROM [doc].[Payments] where card_id=" + card_id + "");

                //_db.Database.ExecuteSqlCommand("DELETE FROM [doc].[CardCharges] where card_id=" + card_id + "");

                //_db.Database.ExecuteSqlCommand("DELETE FROM [book].[Cards] where id=" + card_id + "");


                //ReportUserOnDisableJob reportUserOnDisable = new ReportUserOnDisableJob();
                //reportUserOnDisable.Execute(null);
                //DateTime datefrom = new DateTime(DateTime.Now.Year, 09, 01, 00, 00, 01);
                //DateTime dateto = new DateTime(DateTime.Now.Year, 09, 30, 23, 59, 01);
                //var counts = 0;
                //var Subscribtions = await _db.Cards.Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.Tdate >= datefrom && c.Tdate <= dateto).ToListAsync();
                //foreach (var subpack in Subscribtions.Select(s => s.Subscribtions).ToList())
                //{
                //    var xsx = subpack.Where(c => c.SubscriptionPackages.Any(s => s.Package.Id == 304086)).FirstOrDefault();
                //    DateTime time = new DateTime();

                //    if (subpack.Where(c => c.SubscriptionPackages.Any(s => s.Package.Id == 304086)).FirstOrDefault() != null)
                //    {
                //        time = xsx.Tdate;
                //        foreach (var item in subpack.ToList())
                //        {

                //            if ( time.AddDays(30) < item.Tdate)
                //            {
                //                time = item.Tdate;
                //                counts++;
                //            }
                //            else
                //            {
                //                time = item.Tdate;
                //            }
                //        }
                //    }
                //}
                //var xx = 0;
                //string output = Regex.Replace("", @"[^\u0009\u000A\u000D\u0020-\u007E]", "*");

                //foreach (var item in _db.ReturnedCards.ToList())
                //{
                //    double Cash = 0;
                //    double Cashless = 0;
                //    var arrray = item.commission.ToString();
                //    Commision _returne_cash = new Commision();

                //    JObject parsed = JObject.Parse(arrray);
                //    var commision_type = (JArray)parsed["commisionType"];
                //    var  _amount = (JArray)parsed["amount"];
                //    for (int j = 0; j < _amount.Count(); j++)
                //    {
                //        if (Convert.ToInt32(commision_type[j]) == 2)
                //        {
                //            Cash += Convert.ToDouble(_amount[j]);
                //        }
                //        if (Convert.ToInt32(commision_type[j]) == 18)
                //        {
                //            Cashless += Convert.ToDouble(_amount[j]);
                //        }

                //    }
                //    CardDetailData _card = _db.Cards.Where(c => c.Id == item.card_id).Select(c => new CardDetailData
                //    {
                //        PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                //        ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                //        Card = c,
                //        CustomerType = c.Customer.Type,
                //        IsBudget = c.Customer.IsBudget,
                //        SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                //        MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                //        CardLogs = c.CardLogs.ToList()
                //    }).FirstOrDefault();

                //    if (_card != null)
                //    {
                //        decimal amount = (decimal)_card.SubscribAmount;
                //        List<Param> _params = _db.Params.ToList();
                //        decimal jurid_limit_months = int.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);
                //        decimal balance = Convert.ToDecimal(Cash + Cashless);
                //        string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                //        int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                //        string charge_timess = _db.Params.First(p => p.Name == "CardCharge").Value;
                //        if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
                //        {
                //            balance += amount * jurid_limit_months;
                //        }

                //        //int coeff = service_days;
                //        //amount -= (amount * (decimal)_card.Card.Discount / 100);
                //        //decimal dayly_amount = amount / coeff;
                //        //day = (int)Math.Round((balance / dayly_amount), 0);
                //        int day = 0;
                //        //original code
                //        while (true)
                //        {
                //            int coeff = service_days;// DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                //            decimal dayly_amount = amount / coeff;
                //            dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                //            if (balance < dayly_amount)
                //                break;
                //            balance -= dayly_amount;
                //            day++;
                //        }
                //        _card.Card.FinishDate = _card.Card.FinishDate.AddDays(-day);

                //        _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                //        _db.SaveChanges();
                //        var tyday = GenerateFinishDate(charge_timess).AddDays(day);;
                //        //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                //        CashCashless _cash = new CashCashless();
                //    }
                //    // _cash = _returne_cash.Returned_commision(arrray);
                //}
                //var cus = _db.Customers.Include("Cards").ToList();
                //List<Card> idname = new List<Card>();
                //AutoMapper.Mapper.Map(cus, idname);
                //var cardss = _db.Database.SqlQuery<string>("SELECT ct.phone1 FROM book.Cards c inner join book.Customers ct on c.customer_id = ct.id where c.tower_id = 158").ToList();
                //foreach (var item in cardss)
                //{
                //    Task.Run(async () => { await Utils.Utils.sendMessage(item, "Samauwyeblo andzaze mexis dacemis gamo droebit shegiwydebat mauwyebloba. Mauwyeblobis agdgenis droa 22:40 wuti. bodishit diskomportistvis."); }).Wait();
                //}
                //using (var db = new SqlConnection("Data Source=localhost;Initial Catalog=DigitalTVBilling;User ID=sa;Password=tyupi123;MultipleActiveResultSets=true;"))
                //{
                //    // var orderDetails = db.Query<dapp>(sqlOrderDetails).ToList();
                //    string sqlrun = @"SELECT * FROM book.Cards as c inner join book.Customers as cu on c.customer_id=cu.id";
                //    db.Open();
                //  var  PackCardLogging = db.Query<Subscrib, SPackage, Subscrib>(sqlrun, (subscribtion, subscriptionPackage) =>
                //    {

                //        Subscrib sub;
                //        sub = subscribtion;
                //        sub.SubscriptionPackages = new List<SPackage>() { subscriptionPackage };


                //        return sub;

                //    }).Distinct().ToList();
                //    var x = 0;
                //}
                //    var cy = _db.Subscribtions.SqlQuery(@"SELECT s.id as Id, s.tdate as Tdate, s.card_id as CardId ,s.amount as Amount,s.status as Status,s.user_id as UserId
                // FROM doc.Subscribes s
                //inner join(
                //  SELECT DISTINCT[SubscriptionPackages].*
                //  from(
                //     select  subp.id, subp.package_id, subp.subscription_id

                //     from doc.Subscribes as su
                //     left join doc.SubscriptionPackages as subp on su.id = subp.subscription_id
                //     ) AS[SubscriptionPackages]
                //     ) as [t0] on[t0].subscription_id = s.id
                //      ").ToList();

                //    var cyx = _db.Database.SqlQuery<Subscrib>(@"SELECT s.id , s.tdate , s.card_id  ,s.amount ,s.status ,s.user_id 
                // FROM doc.Subscribes s
                //left join(
                //  SELECT DISTINCT SubscriptionPackages.*

                //     from doc.SubscriptionPackages  
                //    ) as SubscriptionPackages on s.id=SubscriptionPackages.subscription_id").ToList();
                // var cy = _db.Subscribtions.SqlQuery("SELECT s.id as Id, s.tdate as Tdate, s.card_id as CardId ,s.amount as Amount,s.status as Status,s.user_id as UserId FROM doc.Subscribes s left join  doc.SubscriptionPackages as sp on sp.subscription_id=s.id ORDER BY s.id").ToList();
                //var cust = _db.Database.SqlQuery<IdName>("select * from dbo.NameID(546583)").ToList();

                //var cys = _db.Database.SqlQuery<Subscrib>("SELECT *,SubscriptionPackages FROM doc.Subscribes s left join  doc.SubscriptionPackages as sp on sp.subscription_id=s.id GROUP BY SubscriptionPackages").ToList();
                //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString))
                //{
                //    //con.Open();
                //     //string str = "[dbo].[tyupi](@card_id)";
                //    using (SqlCommand cmd = new SqlCommand("select [dbo].[tyupi] (@card_id)", con))
                //    {
                //        DateTime original = DateTime.Now;
                //        //DateTime start = original.Add(new TimeSpan(-6, 0, 0));
                //        //DateTime end = original.Add(new TimeSpan(-3, 20, 0));

                //        cmd.Parameters.AddWithValue("@card_id", 359064);
                //        cmd.CommandType = CommandType.Text;
                //        //cmd.Parameters.Add(new SqlParameter("@endTime", end));
                //        con.Open();
                //        var card_id = cmd.ExecuteScalar();

                //    }
                //    con.Close();
                //}

                //var PackCardLogging = new List<sub_dap>();
                //string card_custumer = @"  SELECT * FROM [DigitalTVBilling].[book].[Cards] AS s
                //           LEFT JOIN [DigitalTVBilling].[book].[Customers] AS sub ON sub.id=s.customer_id 
                //           INNER JOIN  [DigitalTVBilling].[doc].[CardLogs] AS car ON car.card_id=s.id";
                //string sqlOrderDetails = "SELECT * FROM [DigitalTVBilling].[doc].[Subscribes] AS sub inner JOIN [DigitalTVBilling].[doc].[SubscriptionPackages] AS pac ON sub.id=pac.subscription_id";
                //using (var db = new SqlConnection("Data Source=localhost;Initial Catalog=DigitalTVBilling;User ID=sa;Password=tyupi123;MultipleActiveResultSets=true;"))
                //{
                //   // var orderDetails = db.Query<dapp>(sqlOrderDetails).ToList();
                //    var car = db.Query(card_custumer).ToList();
                //    db.Open();
                //    PackCardLogging = db.Query<sub_dap, pack_dap, sub_dap>(sqlOrderDetails, (subscribtion, subscriptionPackage) =>
                //    {

                //        sub_dap sub;
                //        sub = subscribtion;
                //        sub.pack_daps = new List<pack_dap>();
                //        sub.pack_daps.Add(subscriptionPackage);

                //        return subscribtion;

                //    }).Distinct().ToList();
                //    var x = 0;
                //}
                //var customerSellAttachs = from custAttachs in _db.CustomerSellAttachments.Include("Customer").ToList()
                //                          group custAttachs by custAttachs.CustomerID into cGroup
                //                          select new
                //                          {
                //                              Key = cGroup.Key,
                //                              customerAttachs = cGroup
                //                          };
                //foreach (var group in customerSellAttachs)
                //{
                //    var _grp = group;
                //    var subgroupbydiler = from dillerAttachs in _grp.customerAttachs
                //                          group dillerAttachs by dillerAttachs.Diler_Id into dGroup
                //                          select new
                //                          {
                //                              Key = dGroup.Key,
                //                              customerAttachs = dGroup
                //                          };

                //    foreach (var itembydiler in subgroupbydiler)
                //    {
                //        var list = itembydiler.customerAttachs.ToList();
                //        list.ForEach(a => a.VerifyStatus = a.Customer.AttachmentApproveStatus == 3 ? (short)1 : a.Customer.AttachmentApproveStatus);
                //        list.ForEach(a => _db.Entry(a).State = EntityState.Modified);
                //        _db.SaveChanges();
                //    }
                //}


                //var list = _db.CustomerSellAttachments.Include("Customer").ToList();

                //foreach (var item in list)
                //{
                //    item.Diler_Id = item.Customer.UserId;
                //    _db.Entry(item).State = EntityState.Modified;
                //    _db.SaveChanges();
                //}
                //List<Card> crds = _db.Cards.Include("Payments").Include("CardCharges").Where(c => c.Id >= 356854 && c.Id <= 357184 && c.CardStatus == CardStatus.Active && c.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Any(sub => sub.Package.Id == 304085)).ToList();
                //CASSocket _socket = new CASSocket() { IP = "192.168.4.143", Port = 8000 };
                //_socket.Connect();
                //foreach (var item in crds)
                //{
                //    DateTime __dt = item.Payments.LastOrDefault().Tdate;
                //    decimal balance = Utils.Utils.GetBalance(item.Payments.Where(p=>p.Tdate <= __dt).Sum(p => (decimal?)p.Amount) ?? 0, item.CardCharges.Where(ch=>ch.Tdate <= __dt).Select(s => (decimal?)s.Amount).Sum() ?? 0);

                //    //decimal balance = GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                //    decimal amount = (decimal)15;
                //    int day = 0, time_interval = 0;

                //    if (amount == 0)
                //        return null;

                //    string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                //    int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);

                //    //int coeff = service_days;
                //    //amount -= (amount * (decimal)_card.Card.Discount / 100);
                //    //decimal dayly_amount = amount / coeff;
                //    //day = (int)Math.Round((balance / dayly_amount), 0);

                //    //original code
                //    while (true)
                //    {
                //        int coeff = service_days;// DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                //        decimal dayly_amount = amount / coeff;
                //        dayly_amount -= (dayly_amount * (decimal)0 / 100);
                //        if (balance < dayly_amount)
                //            break;
                //        balance -= dayly_amount;
                //        day++;
                //    }

                //    //while (true)
                //    //{
                //    //    int coeff = service_days;//DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                //    //    decimal dayly_amount = amount / coeff / Utils.divide_card_charge_interval;
                //    //    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                //    //    if (balance < dayly_amount)
                //    //        break;
                //    //    balance -= dayly_amount;
                //    //    time_interval++;
                //    //}

                //    //_card.Card.FinishDate = GenerateFinishDate(charge_time).AddDays(time_interval);// AddHours(time_interval); //GenerateFinishDate(charge_time).AddHours(hours);

                //    DateTime findate = new DateTime(__dt.Year, __dt.Month, __dt.Day, 0,1,0).AddDays(day+1);

                //    if (!_socket.SendEntitlementRequest(Convert.ToInt32(item.CardNum), new short[] { 2,9 }, findate.AddHours(-4), findate.AddHours(-4), false))
                //    //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                //    {
                //        throw new Exception("ბარათის პაკეტები ვერ შაიშალა:" + item.CardNum);
                //    }

                //}
                //_socket.Disconnect();
                //List < Card > cards = _db.Cards.Include("Customer").Where(c => c.CardStatus != CardStatus.Canceled).ToList();
                //CASSocket _socket = new CASSocket() { IP = "192.168.4.143", Port = 8000 };
                //_socket.Connect();

                //foreach (var item in cards)
                //{
                //    Task.Run(async () => { await Utils.Utils.sendMessage(item.Customer.Phone1, "Sasiamovno siaxle digital TV-s abonentebistvis. Telearxebis standartul pakets daemateba kinoarxi - kinosemia. Cvlileba ar gamoiwvevs paketis gadzvirebas."); }).Wait();
                //    if (!_socket.SendOSDRequest(Convert.ToInt32(item.CardNum), "ტელეარხების სტანდარტულ პაკეტს დაემატება კინოარხი - КИНОСЕМЬЯ. ცვლილება არ გამოიწვევს პაკეტის გაძვირებას.", DateTime.Now.AddHours(-4), 0))
                //    //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                //    {
                //        //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                //    }
                //}


                //_socket.Disconnect();
                //List<Card> cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package")
                //    .Where(c => c.CardStatus != CardStatus.Canceled && c.CardStatus == CardStatus.Blocked &&
                //    c.Subscribtions.Where(s=>s.Status == true).FirstOrDefault().SubscriptionPackages.Where(p=>p.Package.RentType == RentType.rent).ToList().Count > 0).ToList();

                //foreach (var item in cards)
                //{
                //    _db.CardCharges.Add(new CardCharge() { CardId = item.Id, Amount = 6, Tdate = DateTime.Now, Status = CardChargeStatus.PenDaily });
                //}

                //_db.SaveChanges();


                //using (SqlConnection cn = new SqlConnection("Data Source=192.168.4.6;Initial Catalog=DigitalTVBilling;User ID=sa;Password=Sa1234;MultipleActiveResultSets=true;"))
                //{
                //    //List<string> abnums = new List<string>();
                //    //abnums.Add("3719535");
                //    //abnums.Add("5320589");
                //    //abnums.Add("4702210");
                //    //abnums.Add("1086681");

                //    //abnums.Add("9105560");
                //    //abnums.Add("5364995");
                //    //abnums.Add("5553824");
                //    //abnums.Add("5605709");

                //    //List<int> abids = _db.Cards.Where(i => !abnums.Any(e => e == i.AbonentNum) && i.Customer.Type != CustomerType.Technic).Select(c => c.CustomerId).ToList();
                //    //List<int> abids = _db.Cards.Where(i => i.AbonentNum == "7434193" || i.AbonentNum == "7854586").Select(c => c.CustomerId).ToList();
                //    List<int> abids = _db.Cards.Where(i => i.AbonentNum == "8193218").Select(c => c.CustomerId).ToList();


                //    cn.Open();
                //    using (SqlCommand cmd = new SqlCommand("DeleteCustomer", cn))
                //    {
                //        cmd.CommandType = CommandType.StoredProcedure;
                //        cmd.Parameters.Add(new SqlParameter("@customer_id", SqlDbType.Int));

                //        foreach (var item in abids)
                //        {
                //            cmd.Parameters[0].Value = item;
                //            int i = cmd.ExecuteNonQuery();

                //            cmd.Parameters.Clear();
                //        }

                //    }
                //}

                //string msg = "Gilotsavt damdeg Shoba-akhal wels! Gatsnobebt, rom TV MOBILE – m sheitsvala sakontaqto nomeri. Amieridan dagvikavshirdit, tskheli khazis nomerze  032 205 17 17";

                //List<Customer> ablist = _db.Customers.ToList();

                //foreach (Customer c in ablist)
                //{
                //    Task.Run(async () => { await Utils.Utils.sendMessage(c.Phone1, msg); }).Wait();
                //    Thread.Sleep(1000);
                //}
                //using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                //{
                //    List<Customer> customers = _db.Customers.ToList();

                //    foreach (var item in customers)
                //    {
                //        item.Cards = _db.Cards.Where(c => c.CustomerId == item.Id).ToList();

                //        foreach (var card in item.Cards)
                //        {
                //            if (item.Type == CustomerType.Physical)
                //            {
                //                if (card.CardStatus == CardStatus.Active && card.AbonentNum != "2054496" && card.AbonentNum != "63697781")
                //                {
                //                    card.FinishDate = card.FinishDate.AddDays(-1);
                //                    _db.Entry(card).State = EntityState.Modified;
                //                }
                //            }
                //        }
                //    }

                //    tran.Commit();
                //    _db.SaveChanges();
                //}
                //       var sqd = _db.Database.SqlQuery<Subscribtion>(@"select * from doc.Subscribes as sub
                //left OUTER  join doc.SubscriptionPackages as p on p.subscription_id=sub.id").ToList();

                string sql = @"SELECT TOP(" + pageSize + @") d.id AS Id,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.type AS Type,d.city AS City, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status, d.doc_num AS DocNum, d.pack AS ActivePacket 
                         FROM (SELECT row_number() over(ORDER BY cr.id DESC) AS row_num,cr.tdate,c.id,c.name,c.lastname,c.code,c.[type],c.city,c.phone1, cr.doc_num, cr.abonent_num,cr.card_num, cr.status,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE cr.status !=4) AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);

                int count = await _db.Database.SqlQuery<int>(@"SELECT COUNT(cr.id) FROM book.Cards AS cr 
                        INNER JOIN book.Customers AS c ON c.id=cr.customer_id 
                        LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE cr.status !=4").FirstOrDefaultAsync();
                return View(await _db.Database.SqlQuery<AbonentList>(sql).ToRawPagedListAsync(count, page, pageSize));
                //return View(new AsyncRawQueryPagedList<AbonentList>());
            }
        }
        [HttpPost]
        public JsonResult newReRegistering(string code)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    var _custumer = _db.Database.SqlQuery<ReRegistering>($"SELECT c.id AS id,cc.id as card_id, c.name,c.lastname,c.code,c.tdate,c.phone1 AS phone,c.user_id FROM book.Cards as cc inner join  book.Customers AS c ON cc.customer_id=c.id where c.code='{code}'").FirstOrDefault();
                    _db.ReRegisterings.Add(new ReRegistering
                    {
                        name = _custumer.name,
                        lastname = _custumer.lastname,
                        code = _custumer.code,
                        phone = _custumer.phone,
                        tdate = DateTime.Now,
                        user_id = _custumer.user_id,
                        card_id = _custumer.card_id
                    });
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(1);
                }
            }
            return Json(0);
        }
        //[HttpPost]
        //public PartialViewResult HistoryReRegistering(string code)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        var _Registering = _db.Database.SqlQuery<ReRegistering>($"SELECT * FROM dbo.ReRegistering AS c where c.code='{code}'").FirstOrDefault();
        //        if (_Registering == null)
        //        {
        //            return PartialView(null);
        //        }
        //        else
        //        {
        //            return PartialView("~/Views/Abonent/_CardReRegistering.cshtml", _Registering);
        //        }
        //    }
        //}
        [HttpGet]
        public ActionResult New()
        {
            if (!Utils.Utils.GetPermission("ABONENT_ADD"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
                ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList();
                List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType<DescriptionAttribute>(c).Description }).ToList();
                ViewBag.CardStatus = CardEnums;
                string max_num = _db.Cards.Select(c => c.AbonentNum).OrderByDescending(c => c).FirstOrDefault();

                Abonent abonent = new Abonent();

                if (Request["code"] != null)
                {
                    string code = Convert.ToString(Request["code"]);
                    int cust_id = _db.Customers.Where(c => c.Code == code).Select(c => c.Id).FirstOrDefault();
                    if (cust_id == 0)
                    {
                        Order order = _db.Orders.Where(c => c.Code == code).FirstOrDefault();
                        if (order != null)
                        {
                            Session["order"] = order.Id;
                            abonent = Newtonsoft.Json.JsonConvert.DeserializeObject<Abonent>(order.Data);
                            if (abonent.Cards != null)
                                abonent.Cards.ForEach(c => c.AbonentNum = new WebService1().getAbonentNum());
                            //abonent.Cards.ForEach(c => c.AbonentNum = Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1))));
                        }
                    }
                    else
                    {
                        return Redirect("/Abonent/Edit/" + cust_id + "/" + _db.Cards.Where(c => c.CustomerId == cust_id).Select(c => c.CardNum).FirstOrDefault());
                    }
                }
                else
                {
                    abonent = new Abonent() { Customer = new Customer(), Cards = new List<Card> { new Card { AbonentNum = ""/*Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1)))*/ } } };
                }

                if (abonent.Cards == null)
                {
                    abonent.Cards = new List<Card> { new Card { AbonentNum = Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1))) } };
                }

                abonent.attachments = _db.SellAttachments.ToList();
                Param param = _db.Params.Single(m => m.Name == "FreeDays");
                ViewBag.FreeDays = Convert.ToInt32(param.Value);
                return View(abonent);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult New(Abonent abonent, bool is_card_forward = false)
        {
            if (abonent.attachments != null)
                if (abonent.attachments.Count > 0)
                {
                    for (int i = 0; i < abonent.attachments.Count; i++)
                    {
                        if (ModelState.ContainsKey("attachments[" + i + "].Value"))
                            ModelState["attachments[" + i + "].Value"].Errors.Clear();
                        if (ModelState.ContainsKey("attachments[" + i + "].Name"))
                            ModelState["attachments[" + i + "].Name"].Errors.Clear();
                    }

                }

            if (abonent.Customer.Type == CustomerType.Juridical)
            {
                if (ModelState.ContainsKey("Customer.LastName"))
                    ModelState["Customer.LastName"].Errors.Clear();
                abonent.Customer.LastName = "";
            }
            else
            {
                if (ModelState.ContainsKey("Customer.JuridicalFinishDate"))
                    ModelState["Customer.JuridicalFinishDate"].Errors.Clear();
            }

            if (is_card_forward && Utils.Utils.GetPermission("ABONENT_ADD"))
            {
                if (ModelState.ContainsKey("Cards[0].Id"))
                    ModelState["Cards[0].Id"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].Tdate"))
                    ModelState["Cards[0].Tdate"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].CardNum"))
                    ModelState["Cards[0].CardNum"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].Address"))
                    ModelState["Cards[0].Address"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].Discount"))
                    ModelState["Cards[0].Discount"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].Group"))
                    ModelState["Cards[0].Group"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].CustomerId"))
                    ModelState["Cards[0].CustomerId"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].CardStatus"))
                    ModelState["Cards[0].CardStatus"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].City"))
                    ModelState["Cards[0].City"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].Village"))
                    ModelState["Cards[0].Village"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].Region"))
                    ModelState["Cards[0].Region"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].ReceiverId"))
                    ModelState["Cards[0].ReceiverId"].Errors.Clear();
                if (ModelState.ContainsKey("Cards[0].HasFreeDays"))
                    ModelState["Cards[0].HasFreeDays"].Errors.Clear();
                //ModelState.Clear();

                if (ModelState.IsValid && Utils.Utils.GetPermission("ABONENT_ADD"))
                    using (DataContext _db = new DataContext())
                    {
                        using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                        {
                            try
                            {

                                List<Param> _params = _db.Params.ToList();

                                int user_id = 0;
                                if (!abonent.isFromDiler)
                                    user_id = ((User)Session["CurrentUser"]).Id;
                                else
                                {
                                    user_id = abonent.dilerCards.diler_id;
                                }

                                List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(abonent.Logging);

                                if (_db.Customers.Any(c => c.Code == abonent.Customer.Code))
                                {
                                    throw new Exception("აბონენტი " + abonent.Customer.Code + " კოდით უკვე არსებობს");
                                }

                                abonent.Customer.SecurityCode = abonent.Customer.SecurityCode;// Utils.Utils.GetMd5(abonent.Customer.SecurityCode);

                                abonent.Customer.UserId = user_id;
                                //if (abonent.Customer.JuridicalFinishDate.HasValue)
                                //{
                                //    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                //    DateTime dt = abonent.Customer.JuridicalFinishDate.Value;
                                //    abonent.Customer.JuridicalFinishDate = new DateTime(dt.Year, dt.Month, dt.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);
                                //}
                                _db.Customers.Add(abonent.Customer);
                                _db.SaveChanges();

                                if (logs != null && logs.Count > 0)
                                {
                                    this.AddLoging(_db,
                                            LogType.Abonent,
                                            LogMode.Add,
                                            user_id,
                                            abonent.Customer.Id,
                                            abonent.Customer.Name + " " + abonent.Customer.LastName,
                                            logs.Where(c => c.type == "customer").ToList()
                                        );
                                }

                                bool is_valid_ammount = true, is_valid_status = true;
                                string abonent_nums = "";
                                foreach (var _card in abonent.Cards)
                                {
                                    CardDetailData _cardt = _db.Cards.Where(c => c.Id == _card.Id).Select(c => new CardDetailData
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

                                    if (balance < 0)
                                    {
                                        is_valid_ammount = false;
                                        break;
                                    }

                                    if (_cardt.Card.CardStatus == CardStatus.Active || _cardt.Card.CardStatus == CardStatus.Closed)
                                    {

                                    }
                                    else
                                    {
                                        is_valid_status = false;
                                        break;
                                    }
                                    //    Card crd = _db.Cards.Where(c => c.Id == _card.Id).FirstOrDefault();
                                    //crd.CustomerId = abonent.Customer.Id;


                                    //_db.SaveChanges();
                                    //int old_cust_id = _cardt.Card.CustomerId;
                                    string old_cust_name = _cardt.CustomerName + "(" + _cardt.Card.CustomerId + ")";
                                    string new_cust_name = abonent.Customer.Name + "(" + abonent.Customer.Code + ")" + "(" + abonent.Customer.Id + ")";
                                    _cardt.Card.CustomerId = abonent.Customer.Id;

                                    this.AddLoging(_db,
                                                LogType.Card,
                                                LogMode.Forward,
                                                user_id,
                                                _cardt.Card.Id,
                                                "From: " + old_cust_name + "  To:" + new_cust_name + " -> " + _cardt.Card.AbonentNum,
                                                logs.Where(cc => cc.type != "customer").ToList()
                                            );

                                    abonent_nums += " " + _cardt.Card.AbonentNum + " ";

                                    _db.Entry(_cardt.Card).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }

                                if (is_valid_ammount && is_valid_status)
                                {
                                    tran.Commit();


                                    string onRegMsg = "";
                                    MessageTemplate msg = _db.MessageTemplates.Where(m => m.Name == "AbonentRegistration").FirstOrDefault();
                                    if (msg != null)
                                    {
                                        {
                                            onRegMsg = String.Format(msg.Desc, abonent_nums);

                                            Task.Run(async () => { await Utils.Utils.sendMessage(abonent.Customer.Phone1, onRegMsg); }).Wait();
                                            //Utils.Utils.sendMessage(abonent.Customer.Phone1, message.Desc);
                                        }
                                    }


                                    return RedirectToAction("Index", new { page = 1 });
                                }
                                else
                                {
                                    ViewBag.Error = "გადაფორმება ვერ მოხერხდა არასწორი სტატუსის ან დავალიანების გამო.";
                                    tran.Rollback();
                                }

                            }
                            catch (Exception ex)
                            {
                                ViewBag.Error = ex.Message;
                                tran.Rollback();
                            }
                        }
                    }
            }
            else if (ModelState.IsValid && Utils.Utils.GetPermission("ABONENT_ADD"))
            {
                //for (int b = 0; b < 303233; b++)

                //foreach (st_Customers st_customer in st_customers)
                {
                    using (DataContext _db = new DataContext())
                    {
                        using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                        {
                            try
                            {
                                {
                                    string dilerlog = "";
                                    List<Param> _params = _db.Params.ToList();
                                    int user_id = 0;
                                    if (!abonent.isFromDiler)
                                        user_id = ((User)Session["CurrentUser"]).Id;
                                    else
                                    {
                                        user_id = abonent.dilerCards.diler_id;
                                        dilerlog = "Recorder: " + ((User)Session["CurrentUser"]).Name + "-> ";
                                    }

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(abonent.Logging);

                                    if (_db.Customers.Any(c => c.Code == abonent.Customer.Code))
                                    {
                                        throw new Exception("აბონენტი " + abonent.Customer.Code + " კოდით უკვე არსებობს");
                                    }

                                    abonent.Customer.SecurityCode = Utils.Utils.GetMd5(abonent.Customer.SecurityCode);

                                    abonent.Customer.UserId = user_id;
                                    if (abonent.Customer.JuridicalFinishDate.HasValue)
                                    {
                                        string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                        DateTime dt = abonent.Customer.JuridicalFinishDate.Value;
                                        abonent.Customer.JuridicalFinishDate = new DateTime(dt.Year, dt.Month, dt.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);
                                    }
                                    _db.Customers.Add(abonent.Customer);
                                    _db.SaveChanges();

                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                                LogType.Abonent,
                                                LogMode.Add,
                                                user_id,
                                                abonent.Customer.Id,
                                                dilerlog + abonent.Customer.Name + " " + abonent.Customer.LastName,
                                                logs.Where(c => c.type == "customer").ToList()
                                            );
                                    }

                                    double package_discount = Convert.ToDouble(_params.Where(p => p.Name == "PackageDiscount").Select(p => p.Value).First());
                                    int free_days = Convert.ToInt32(_params.Where(p => p.Name == "FreeDays").Select(p => p.Value).First());

                                    string docnum = new WebService1().getDocNum();
                                    //Action<Card> cardDocNumAct = (Card card) =>
                                    //{
                                    //    card.DocNum = docnum;
                                    //};
                                    string abonent_num = "";
                                    Action<Card> cardAct = (Card card) =>
                                       {
                                           string ab_num = Utils.Utils.IsAbonentNumExists(_db, card.AbonentNum);
                                           if (ab_num != string.Empty)
                                               card.AbonentNum = ab_num;

                                           if (_db.Cards.Any(c => c.DocNum == card.DocNum))
                                           {
                                               throw new Exception("ბარათი " + card.DocNum + " ხელშეკრულების ნომერი უკვე არსებობს");
                                           }

                                           card.CustomerId = abonent.Customer.Id;
                                           //card.CardStatus = CardStatus.Montage;
                                           card.CloseDate = DateTime.Now;
                                           card.UserId = user_id;
                                           card.PauseDate = DateTime.Now;
                                           card.CardLogs = new List<CardLog>() { new CardLog() { Date = card.Tdate, Status = CardLogStatus.Montage, UserId = user_id } };
                                           //card.TowerId = abonent.Cards.Select(s => s.TowerId).FirstOrDefault(); ;
                                           card.ClosedIsPen = false;
                                           card.AbonentNum = new WebService1().getAbonentNum();
                                           card.DocNum = docnum;// new WebService1().getDocNum();
                                           card.JuridVerifyStatus = CardJuridicalVerifyStatus.None;
                                           card.juridical_verify_status = "-1";
                                           card.mux1_level = abonent.Cards.Select(s => s.mux1_level).FirstOrDefault();
                                           card.mux2_level = abonent.Cards.Select(s => s.mux2_level).FirstOrDefault();
                                           card.mux3_level = abonent.Cards.Select(s => s.mux3_level).FirstOrDefault();
                                           card.mux1_quality = abonent.Cards.Select(s => s.mux1_quality).FirstOrDefault();
                                           card.mux2_quality = abonent.Cards.Select(s => s.mux2_quality).FirstOrDefault();
                                           card.mux3_quality = abonent.Cards.Select(s => s.mux3_quality).FirstOrDefault();
                                           card.RentFinishDate = DateTime.Now;
                                           if (card.CardServices != null)
                                           {
                                               List<int> serv_ids = new List<int>();
                                               foreach (CardService _serv in card.CardServices)
                                               {
                                                   _serv.Date = card.Tdate;
                                                   _serv.IsActive = _serv.PayType == CardServicePayType.NotCash;

                                                   serv_ids.Add(_serv.ServiceId);
                                               }

                                               this.AddLoging(_db,
                                                           LogType.CardService,
                                                           LogMode.Add,
                                                           user_id,
                                                           card.Id,
                                                           dilerlog + card.AbonentNum + " - ის მომსახურება ",
                                                           _db.Services.Where(c => serv_ids.Contains(c.Id)).Select(c => new LoggingData { field = "მომსახურება", new_val = c.Name }).ToList()
                                                       );
                                           }

                                           if (card.Subscribtions != null)
                                           {
                                               foreach (Subscribtion subscrib in card.Subscribtions)
                                               {
                                                   int[] arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                   var _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();

                                                   if (_packages.Any(p => p.RentType != RentType.block && p.RentType != RentType.technic))
                                                   {
                                                       subscrib.SubscriptionPackages.Add(new SubscriptionPackage() { PackageId = _db.Packages.Where(pack => pack.RentType == RentType.block).Select(p => p.Id).First() });
                                                       arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                       _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                                                   }

                                                   subscrib.Amount = _packages.Select(p => abonent.Customer.Type == CustomerType.Juridical ? p.JuridPrice : p.Price).Sum();
                                                   subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                                   subscrib.Status = true;
                                                   subscrib.Tdate = DateTime.Now;
                                                   subscrib.UserId = user_id;
                                               }
                                           }
                                           else
                                           {
                                               var defaultPackages = _db.Packages.Where(p => p.IsDefault || p.RentType == RentType.block).ToList();
                                               if (defaultPackages.Count > 0)
                                               {
                                                   card.Subscribtions = new List<Subscribtion>()
                                                           {
                                                            new Subscribtion {
                                                                Amount =  abonent.Customer.Type == CustomerType.Juridical ? defaultPackages.Select(p=>p.JuridPrice).Sum() : defaultPackages.Select(p=>p.Price).Sum(),
                                                                Status = true,
                                                                Tdate = DateTime.Now,
                                                                UserId = user_id,
                                                                SubscriptionPackages = defaultPackages.Select(s=>new SubscriptionPackage
                                                                {
                                                                    PackageId = s.Id
                                                                }).ToList()
                                                            }
                                                           };
                                                   //card.Subscribtions.First().SubscriptionPackages.Add(new SubscriptionPackage() { PackageId = _db.Packages.Where(pack => pack.RentType == RentType.block).Select(p => p.Id).First() });
                                                   var subscrib = card.Subscribtions.First();
                                                   subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                               }
                                           }

                                           string charge_time = _params.Where(p => p.Name == "CardCharge").First().Value;
                                           if (abonent.Customer.Type == CustomerType.Juridical && abonent.Customer.IsBudget)
                                           {

                                               //card.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(card.Tdate, charge_time, (decimal)card.Subscribtions.Where(s => s.Status).Sum(s => s.Amount), decimal.Parse(_params.Where(p => p.Name == "JuridLimitMonths").First().Value), card.Discount, free_days);
                                               card.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(charge_time);
                                               if (!card.HasFreeDays)
                                               {
                                                   card.CardStatus = CardStatus.Active;
                                                   free_days = 0;

                                                   //original code
                                                   //decimal amount = (decimal)(card.Subscribtions.Sum(s=>s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                   //int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                                                   //decimal amount = (decimal)(card.Subscribtions.Sum(s => s.Amount) / 1/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*// Utils.Utils.divide_card_charge_interval / 60);

                                                   //amount -= (amount * (decimal)card.Discount / 100);
                                                   //card.CardCharges = new List<CardCharge>() { new CardCharge() { Amount = amount, Tdate = card.CasDate, Status = CardChargeStatus.PreChange } };
                                               }
                                           }
                                           else
                                           {
                                               if (!card.HasFreeDays || free_days == 0)
                                               {
                                                   card.CardStatus = CardStatus.Closed;
                                                   free_days = 0;
                                               }
                                               else if (card.HasFreeDays && free_days > 0)
                                               {
                                                   card.CardStatus = CardStatus.FreeDays;
                                               }


                                               if (abonent.Customer.Type != CustomerType.Juridical)
                                               {
                                                   card.FinishDate = Utils.Utils.GenerateFinishDate(card.Tdate, charge_time).AddDays(free_days);

                                               }
                                               else
                                               {
                                                   card.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(charge_time);
                                                   card.CardStatus = CardStatus.Active;


                                               }


                                               if (abonent.Customer.Type == CustomerType.Technic)
                                               {
                                                   card.CardStatus = CardStatus.Active;
                                                   card.FinishDate = Utils.Utils.GenerateFinishDate(card.Tdate, charge_time).AddYears(20);
                                               }
                                           }



                                           //JobSheduler.resCheduleTrigger("trigger_" + card.Id.ToString(), card.Id, 0, 0);
                                           //card.FinishDate = DateTime.Now.AddMinutes(2);// DateTime.SpecifyKind(DateTime.Now.AddMinutes(2), DateTimeKind.Utc);
                                       };

                                    //abonent.Cards.ForEach(cardDocNumAct);

                                    abonent.Cards.ForEach(cardAct);


                                    _db.Cards.AddRange(abonent.Cards);
                                    _db.SaveChanges();
                                    //if (_db.Cards.Where(c => c.Id == card.Id).FirstOrDefault().CardStatus == CardStatus.Active)
                                    //{
                                    CardLog _log = new CardLog() { CardId = abonent.Cards.Select(s => s.Id).FirstOrDefault(), Date = DateTime.Now, Status = CardLogStatus.Open, UserId = user_id };
                                    _db.CardLogs.Add(_log);
                                    _db.SaveChanges();
                                    //}

                                    //diff docnums
                                    foreach (var item in abonent.Cards)
                                    {
                                        //List<Card> checkcards = abonent.Cards.Where(c=>c.Id != item.Id).ToList();
                                        Subscribtion curr_sb = _db.Subscribtions.Where(s => s.CardId == item.Id && s.Status == true).First();
                                        List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == curr_sb.Id).ToList();
                                        List<Package> curr_packs = new List<Package>();
                                        foreach (var sbs in sbp)
                                        {
                                            Package package = _db.Packages.Where(p => p.Id == sbs.PackageId).First();
                                            if (package.RentType != RentType.block)
                                            {
                                                curr_packs.Add(package);
                                            }
                                            if (package.Id == 304084)
                                            {
                                                //UtilsController ut = new UtilsController();
                                                //var cardsid = new List<int>();
                                                //var packagesid = new List<int>();
                                                //packagesid.Add(304085);
                                                //cardsid.Add(item.Id);
                                                //ut.NewAutoSubscrib(null, item, package, user_id);
                                                List<Package> packToChangeTo = _db.Packages.Where(p => p.Id == 304085 || p.RentType == RentType.block).ToList();
                                                AutoSubscribChangeCard _card = new AutoSubscribChangeCard()
                                                {
                                                    CardId = item.Id,
                                                    UserId = user_id,
                                                    CasIds = String.Join(",", packToChangeTo.Select(p => p.CasId)),
                                                    PackageNames = String.Join("+", packToChangeTo.Select(p => p.Name)),
                                                    PackageIds = String.Join(",", packToChangeTo.Select(p => p.Id)),
                                                    Amount = packToChangeTo.Select(c => c.Price).Sum(),
                                                    SendDate = DateTime.Now.AddDays(30)
                                                };
                                                _db.AutoSubscribChangeCards.Add(_card);
                                                _db.SaveChanges();
                                            }
                                            if (package.Id == 304086)
                                            {
                                                //UtilsController ut = new UtilsController();
                                                //var cardsid = new List<int>();
                                                //var packagesid = new List<int>();
                                                //packagesid.Add(304085);
                                                //cardsid.Add(item.Id);
                                                //ut.NewAutoSubscrib(null, item, package, user_id);
                                                List<Package> packToChangeTo = _db.Packages.Where(p => p.Id == 304071 || p.RentType == RentType.block).ToList();
                                                AutoSubscribChangeCard _card = new AutoSubscribChangeCard()
                                                {
                                                    CardId = item.Id,
                                                    UserId = user_id,
                                                    CasIds = String.Join(",", packToChangeTo.Select(p => p.CasId)),
                                                    PackageNames = String.Join("+", packToChangeTo.Select(p => p.Name)),
                                                    PackageIds = String.Join(",", packToChangeTo.Select(p => p.Id)),
                                                    Amount = packToChangeTo.Select(c => c.Price).Sum(),
                                                    SendDate = DateTime.Now.AddDays(30)
                                                };
                                                _db.AutoSubscribChangeCards.Add(_card);
                                                _db.SaveChanges();
                                            }
                                        }

                                        foreach (var checkcards in abonent.Cards.Where(c => c.Id != item.Id).ToList())
                                        {
                                            Subscribtion check_sb = _db.Subscribtions.Where(s => s.CardId == checkcards.Id && s.Status == true).First();
                                            List<SubscriptionPackage> check_sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == check_sb.Id).ToList();
                                            List<Package> check_packs = new List<Package>();

                                            foreach (var checks_sbps in check_sbp)
                                            {
                                                Package package = _db.Packages.Where(p => p.Id == checks_sbps.PackageId).First();
                                                if (package.RentType != RentType.block)
                                                {
                                                    check_packs.Add(package);
                                                }
                                            }

                                            if (check_packs.Any(p => p.RentType == curr_packs.First().RentType))
                                            {

                                            }
                                            else
                                            {
                                                checkcards.DocNum = new WebService1().getDocNum();
                                                _db.Entry(checkcards).State = EntityState.Modified;
                                                _db.SaveChanges();
                                            }

                                        }

                                    }

                                    //abonent.Cards.ForEach(x => JobSheduler.resCheduleTrigger("trigger_" + x.Id.ToString(), x.Id, 0, 0));

                                    string[] address = _params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                                    CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                    _socket.Connect();

                                    if (abonent.Customer.Type == CustomerType.Juridical)
                                    {
                                        foreach (Card c in abonent.Cards)
                                        {
                                            c.CloseDate = DateTime.Now;

                                            this.AddLoging(_db,
                                                LogType.Card,
                                                LogMode.Add,
                                                user_id,
                                                c.Id,
                                                dilerlog + c.AbonentNum,
                                                logs.Where(cc => cc.type != "customer").ToList()
                                            );
                                            _db.JuridicalStatus.Add(new JuridicalStatus()
                                            {
                                                tdate = DateTime.Now,
                                                card_id = c.Id,
                                                user_id = user_id,
                                                status = -1

                                            });
                                            _db.JuridicalLoggings.Add(new JuridicalLogging()
                                            {
                                                tdate = DateTime.Now,
                                                card_id = c.Id,
                                                user_id = 1,
                                                status = -1,
                                                name = _db.Database.SqlQuery<string>("SELECT u.name FROM book.Users u where u.id=" + 1).FirstOrDefault()
                                            });
                                            _db.SaveChanges();

                                            if (c.Subscribtions.First().SubscriptionPackages.Any(p => p.Package.RentType == RentType.block))
                                            {
                                                if (c.Subscribtions.First().SubscriptionPackages.Any(p => p.Package.RentType == RentType.buy))
                                                {
                                                    short[] cas_ids = c.Subscribtions.First().SubscriptionPackages.Where(p => p.Package.RentType == RentType.block).Select(p => (short)p.Package.CasId).ToArray();
                                                    if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), cas_ids, c.CloseDate.AddHours(-4), new DateTime(2038, 1, 1, 0, 0, 0, 0), true))
                                                    {
                                                        throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                                    }
                                                }
                                            }

                                            short[] _cas_ids = { };
                                            if (c.Subscribtions != null && c.Subscribtions.Count > 0)
                                                _cas_ids = c.Subscribtions.First().SubscriptionPackages.Select(p => (short)p.Package.CasId).ToArray();


                                            if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), _cas_ids, c.CloseDate.AddHours(-4), c.FinishDate.AddHours(-4), true))
                                            {
                                                throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                            }

                                            _db.Entry(c).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else
                                        foreach (Card c in abonent.Cards)
                                        {
                                            this.AddLoging(_db,
                                                    LogType.Card,
                                                    LogMode.Add,
                                                    user_id,
                                                    c.Id,
                                                    dilerlog + c.AbonentNum,
                                                    logs.Where(cc => cc.type != "customer").ToList()
                                                );
                                            _db.JuridicalStatus.Add(new JuridicalStatus()
                                            {
                                                tdate = DateTime.Now,
                                                card_id = c.Id,
                                                user_id = user_id,
                                                status = -1

                                            });
                                            _db.JuridicalLoggings.Add(new JuridicalLogging()
                                            {
                                                tdate = DateTime.Now,
                                                card_id = c.Id,
                                                user_id = 1,
                                                status = -1,
                                                name = _db.Database.SqlQuery<string>("SELECT u.name FROM book.Users u where u.id=" + 1).FirstOrDefault()
                                            });
                                            _db.SaveChanges();

                                            if (c.Subscribtions.First().SubscriptionPackages.Any(p => p.Package.RentType == RentType.block))
                                            {
                                                if (c.Subscribtions.First().SubscriptionPackages.Any(p => p.Package.RentType == RentType.buy))
                                                {
                                                    short[] cas_ids = c.Subscribtions.First().SubscriptionPackages.Where(p => p.Package.RentType == RentType.block).Select(p => (short)p.Package.CasId).ToArray();
                                                    if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), cas_ids, c.CloseDate.AddHours(-4), new DateTime(2038, 1, 1, 0, 0, 0, 0), true))
                                                    {
                                                        throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                                    }
                                                }
                                            }

                                            //original if statement
                                            //if (c.CardStatus != CardStatus.Closed)
                                            {
                                                //SEND CAS DATA
                                                //if (!_socket.SendCardStatus(Convert.ToInt32(c.CardNum), true, DateTime.SpecifyKind(DateTime.Now.AddDays(3), DateTimeKind.Utc)))
                                                //{
                                                //    throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                                //}

                                                short[] cas_ids = { };
                                                if (c.Subscribtions != null && c.Subscribtions.Count > 0)
                                                    cas_ids = c.Subscribtions.First().SubscriptionPackages.Select(p => (short)p.Package.CasId).ToArray();



                                                //original code
                                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), cas_ids, c.CloseDate.AddHours(-4), c.FinishDate.AddHours(-4), true))
                                                //{
                                                //    throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                                //}
                                                //if (!_socket.SendCardStatus(Convert.ToInt32(c.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                                //{
                                                //    throw new Exception();
                                                //}
                                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), cas_ids, DateTime.Now.AddHours(-8), DateTime.Now.AddHours(-4), true))
                                                //{
                                                //    throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                                //}

                                                //დროებითი
                                                //if (!_socket.SendEntitlementRequestTemp(Convert.ToInt32(c.CardNum), cas_ids, DateTime.SpecifyKind(new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), DateTimeKind.Utc), false))
                                                //{
                                                //}
                                                //SendTempCas(_db, _socket, c.CardNum);
                                            }

                                            //if (!c.HasFreeDays)
                                            //{
                                            //    short[] cas_ids = { };
                                            //    if (c.Subscribtions != null && c.Subscribtions.Count > 0)
                                            //        cas_ids = c.Subscribtions.First().SubscriptionPackages.Select(p => (short)p.Package.CasId).ToArray();
                                            //    //დროებითი
                                            //    if (!_socket.SendEntitlementRequestTemp(Convert.ToInt32(c.CardNum), cas_ids, DateTime.SpecifyKind(new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), DateTimeKind.Utc), false))
                                            //    {
                                            //    }

                                            //   SendTempCas(_db, _socket, c.CardNum);
                                            //}
                                        }

                                    _socket.Disconnect();

                                    if (Session["order"] != null)
                                    {
                                        int order_id = (int)Session["order"];
                                        Order order = _db.Orders.Where(o => o.Id == order_id).FirstOrDefault();
                                        if (order != null)
                                        {
                                            order.Status = OrderStatus.Closed;
                                            _db.Entry(order).State = EntityState.Modified;
                                            _db.SaveChanges();
                                            Session.Remove("order");
                                        }
                                    }

                                    if (abonent.attachments != null && abonent.attachments.Count > 0)
                                    {
                                        List<CustomerSellAttachments> cust_attachs = new List<CustomerSellAttachments>();

                                        foreach (var item in abonent.attachments)
                                        {
                                            if (item.Value != 0 && item.Value > 0)
                                            {
                                                if (abonent.Cards.Select(s => s.Subscribtions.First().SubscriptionPackages.Any(a => a.PackageId == 304086)).FirstOrDefault())
                                                    cust_attachs.Add(new CustomerSellAttachments() { AttachmentID = item.Id, VerifyStatus = 5, status = SellAttachmentStatus.temporary_use, CustomerID = abonent.Customer.Id, Count = item.Value, Diler_Id = user_id });
                                                else
                                                if (abonent.Customer.temporary_use == 1)
                                                {
                                                    cust_attachs.Add(new CustomerSellAttachments() { AttachmentID = item.Id, VerifyStatus = 6, status = SellAttachmentStatus.temporary_use, CustomerID = abonent.Customer.Id, Count = item.Value, Diler_Id = user_id });
                                                }
                                                else
                                                {
                                                    cust_attachs.Add(new CustomerSellAttachments() { AttachmentID = item.Id, CustomerID = abonent.Customer.Id, Count = item.Value, Diler_Id = user_id });
                                                }
                                            }
                                        }

                                        _db.CustomerSellAttachments.AddRange(cust_attachs);
                                        _db.SaveChanges();
                                    }

                                    tran.Commit();
                                    //MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "AbonentRegistration");

                                    string onRegMsg = "";
                                    MessageTemplate msg = _db.MessageTemplates.Where(m => m.Name == "AbonentRegistration").FirstOrDefault();
                                    if (msg != null)
                                    {
                                        {
                                            string abonent_nums = "";
                                            double ammount = 0;
                                            foreach (Card card in abonent.Cards)
                                            {
                                                abonent_nums += " " + card.AbonentNum + " ";
                                                foreach (Subscribtion subscrib in card.Subscribtions)
                                                {
                                                    ammount += subscrib.Amount;
                                                }
                                                //ammount += card.Subscribtions
                                            }

                                            onRegMsg = String.Format(msg.Desc, abonent_nums, ammount);

                                            Task.Run(async () => { await Utils.Utils.sendMessage(abonent.Customer.Phone1, onRegMsg); }).Wait();
                                            //Utils.Utils.sendMessage(abonent.Customer.Phone1, message.Desc);
                                        }
                                    }

                                }

                                return new RedirectResult("/Abonent");
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Error = ex.Message;
                                tran.Rollback();
                                //abonent.Cards.ForEach(x => JobSheduler.RemoveTrigger("trigger_" + x.Id.ToString()));
                                //JobSheduler.RemoveTrigger("trigger_" + card.Id.ToString(), card.Id, 0, 0);
                            }

                        }

                        Param param = _db.Params.Single(m => m.Name == "FreeDays");
                        ViewBag.FreeDays = Convert.ToInt32(param.Value);
                    }
                }
            }

            using (DataContext _db = new DataContext())
            {
                ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
                ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList();
                List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType<DescriptionAttribute>(c).Description }).ToList();
                ViewBag.CardStatus = CardEnums;
                for (int i = 0; i < abonent.Cards.Count; i++)
                {
                    string max_num = _db.Cards.Select(cc => cc.AbonentNum).OrderByDescending(cc => cc).FirstOrDefault();
                    abonent.Cards[i].AbonentNum = Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1)));
                    abonent.Cards[i].Id = 0;
                }
                abonent.attachments = _db.SellAttachments.ToList();
            }


            return View(abonent);
        }

        private void SendTempCas(DataContext _db, CASSocket _socket, string card_num)
        {
            List<TempCasCard> temp_cards = _db.TempCasCards.Where(t => t.CardNum == card_num).ToList();
            foreach (TempCasCard temp_card in temp_cards)
            {
                short[] cass = temp_card.CasIds.Split(',').Select(cc => Convert.ToInt16(cc.Trim())).ToArray();
                DateTime dt = new DateTime(temp_card.EndDate.Year, temp_card.EndDate.Month, temp_card.EndDate.Day, 0, 0, 0, DateTimeKind.Utc);

                _socket.SendEntitlementRequestTemp(Convert.ToInt32(temp_card.CardNum), cass, DateTime.SpecifyKind(dt, DateTimeKind.Utc), false);

                _db.TempCasCards.Remove(temp_card);
                _db.Entry(temp_card).State = EntityState.Deleted;
            }
            _db.SaveChanges();
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id, string cur_card)
        {
            Abonent ab = new Abonent() { Customer = new Customer(), Cards = new List<Card>() { new Card() } };
            if (id.HasValue)
            {
                using (DataContext _db = new DataContext())
                {
                    ab.Customer = await _db.Customers.Where(c => c.Id == id.Value).FirstOrDefaultAsync();
                    ab.Cards = await _db.Cards.Include("Subscribtions.SubscriptionPackages.Package")
                        .Include("CardServices").Include("CardDamages").Where(c => c.CustomerId == id.Value)
                        .ToListAsync();
                    var card_id = ab.Cards.FirstOrDefault().Id;
                    ab.autoSubscribChange = await _db.AutoSubscribChangeCards.Where(c => c.CardId == card_id).FirstOrDefaultAsync();

                    int[] card_ids = ab.Cards.Select(cr => cr.Id).ToArray();
                    int[] non_canceled = ab.Cards.Where(cr => cr.CardStatus != CardStatus.Canceled).Select(cr => cr.Id).ToArray();
                    int[] canceled = ab.Cards.Where(cr => cr.CardStatus == CardStatus.Canceled).Select(cr => cr.Id).ToArray();
                    List<Payment> _payments = _db.Payments.Where(c => card_ids.Contains(c.CardId)).ToList();
                    List<CardCharge> _charges = _db.CardCharges.Where(c => card_ids.Contains(c.CardId)).ToList();

                    ab.AbonentDetailInfo = new AbonentDetailInfo
                    {
                        Balanse = Math.Round(_payments.Where(c => non_canceled.Contains(c.CardId)).Select(c => c.Amount).Sum() - _charges.Where(c => non_canceled.Contains(c.CardId)).Select(c => c.Amount).Sum(), 3),
                        RentBalanse = Math.Round(_payments.Where(c => non_canceled.Contains(c.CardId)).Select(c => c.PayRent).Sum() - _charges.Where(c => non_canceled.Contains(c.CardId)).Select(c => c.RentAmount).Sum(), 3),
                        FinishDate = ab.Cards.Min(c => c.FinishDate),
                        CanceledCardAmount = Math.Round(_payments.Where(c => canceled.Contains(c.CardId)).Select(c => c.Amount).Sum() - _charges.Where(c => canceled.Contains(c.CardId)).Select(c => c.Amount).Sum(), 3),
                        Chats = _db.CustomersChat.Where(c => c.CustomerId == id.Value).Select(c => new Chat { Tdate = c.Tdate, Message = c.MessageText, UserName = c.User.Name, Id = c.Id }).ToList()
                    };

                    ViewBag.Receivers = await _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToListAsync();
                    ViewBag.Towers = await _db.Towers.OrderBy(r => r.Name).Select(r => new IdName { Id = r.Id, Name = r.Name }).ToListAsync();
                    ViewBag.Reasons = _db.Reasons.Where(c => c.ReasonType == ReasonType.Damage).Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();
                    List<Param> _params = await _db.Params.ToListAsync();

                    ViewBag.PacketChangeTime = int.Parse(_params.First(c => c.Name == "PacketChangeTime").Value);
                    ViewBag.CurrentCard = cur_card;
                    ViewBag.Balances = ab.Cards.Select(c => new IdName { Id = c.Id, Name = Math.Round(Utils.Utils.GetBalance(_payments.Where(p => p.CardId == c.Id).Select(p => p.Amount).Sum(), _charges.Where(p => p.CardId == c.Id).Select(p => p.Amount).Sum()), 2).ToString() }).ToList();
                    ViewBag.ServiceDays = int.Parse(_params.First(c => c.Name == "ServiceDays").Value);
                    ViewBag.HasReadonly = 0;//((User)Session["CurrentUser"]).Type != 1 && DateTime.Now > ab.Customer.Tdate.AddMinutes(int.Parse(_params.First(p => p.Name == "AbonentEditTime").Value)) ? 1 : 0;
                }
            }

            return View(ab);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Abonent abonent, int? id, string cur_card)
        {
            var pack_change = false;
            if (abonent.Customer.Type == CustomerType.Juridical)
            {
                if (ModelState.ContainsKey("Customer.LastName"))
                    ModelState["Customer.LastName"].Errors.Clear();
                abonent.Customer.LastName = "";
            }
            else
            {
                if (ModelState.ContainsKey("Customer.JuridicalFinishDate"))
                    ModelState["Customer.JuridicalFinishDate"].Errors.Clear();
            }

            if (ModelState.ContainsKey("Customer.SecurityCode"))
                ModelState["Customer.SecurityCode"].Errors.Clear();


            if (ModelState.IsValid)
            {


                using (DataContext _db = new DataContext())
                {

                    int user_id = _db.Customers.Where(c => c.Id == id).Select(cc => cc.UserId).FirstOrDefault();

                    abonent.Customer.UserId = user_id;

                    using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                    {
                        try
                        {

                            List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(abonent.Logging);

                            Customer cust = _db.Customers.Where(c => c.Id == abonent.Customer.Id).FirstOrDefault();
                            if (cust != null)
                            {
                                List<Param> _params = _db.Params.ToList();

                                if (Utils.Utils.GetPermission("ABONENT_EDIT"))
                                {
                                    if (_db.Customers.Any(c => c.Id != abonent.Customer.Id && c.Code == abonent.Customer.Code))
                                    {
                                        throw new Exception("აბონენტი " + abonent.Customer.Code + " კოდით უკვე არსებობს");
                                    }


                                    cust.Address = abonent.Customer.Address;
                                    cust.City = abonent.Customer.City.Trim();
                                    cust.Code = abonent.Customer.Code;
                                    cust.Desc = abonent.Customer.Desc;
                                    cust.IsBudget = abonent.Customer.IsBudget;
                                    cust.LastName = abonent.Customer.LastName;
                                    cust.JuridicalType = abonent.Customer.JuridicalType;
                                    cust.District = abonent.Customer.District;
                                    cust.Email = abonent.Customer.Email;
                                    cust.IsFacktura = abonent.Customer.IsFacktura;
                                    if (abonent.Customer.JuridicalFinishDate.HasValue)
                                    {
                                        string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                        DateTime dt = abonent.Customer.JuridicalFinishDate.Value;
                                        cust.JuridicalFinishDate = new DateTime(dt.Year, dt.Month, dt.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);
                                    }
                                    cust.Name = abonent.Customer.Name;
                                    cust.Village = abonent.Customer.Village;
                                    cust.Phone1 = abonent.Customer.Phone1;
                                    cust.Phone2 = abonent.Customer.Phone2;
                                    cust.Region = abonent.Customer.Region.Trim();
                                    cust.Type = abonent.Customer.Type;
                                    //cust.AttachmentApproveStatus = (short)(cust.AttachmentApproveStatus == 2 ? 3 : 1);
                                    if (abonent.isFromDiler == true && abonent.dilerCards != null && abonent.dilerCards.card_id != null && abonent.dilerCards.card_id.Contains(Convert.ToInt32(_db.Cards.Where(c => c.CustomerId == id).Select(s => s.CardNum).FirstOrDefault())))
                                    {
                                        cust.UserId = abonent.dilerCards.diler_id;
                                        user_id = abonent.dilerCards.diler_id;
                                    }
                                    else
                                    {
                                        cust.UserId = abonent.Customer.UserId;
                                    }
                                    if (!string.IsNullOrEmpty(abonent.Customer.SecurityCode))
                                    {
                                        // cust.SecurityCode = abonent.Customer.SecurityCode;
                                    }
                                    _db.Entry(cust).State = System.Data.Entity.EntityState.Modified;

                                    if (logs != null && logs.Where(t => t.type == "customer").Count() > 0)
                                    {
                                        this.AddLoging(_db,
                                            LogType.Abonent,
                                            LogMode.Change,
                                            user_id,
                                            abonent.Customer.Id,
                                            abonent.Customer.Name + " " + abonent.Customer.LastName,
                                            logs.Where(t => t.type == "customer").ToList()
                                         );
                                    }
                                    _db.SaveChanges();
                                }

                                double package_discount = Convert.ToDouble(_params.First(p => p.Name == "PackageDiscount").Value);
                                int free_days = Convert.ToInt32(_params.First(p => p.Name == "FreeDays").Value);
                                decimal jurid_limit_months = int.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);
                                List<CardEditCasData> _CAS_data = new List<CardEditCasData>();

                                Action<Card> cardDocNumAct = (Card card) =>
                                {
                                    Card c = _db.Cards.Where(s => s.Id == card.Id).FirstOrDefault();
                                    if (c != null)
                                    {
                                        card.AbonentNum = c.AbonentNum;
                                        card.DocNum = c.DocNum;
                                        //card.UserId = user_id;
                                    }
                                };

                                abonent.Cards.ForEach(cardDocNumAct);

                                bool isTechnic = false;
                                bool isjuridical = false;

                                var standard_promo = 0;
                                bool card_logg_juridical = false;
                                if (abonent.Customer.Type == CustomerType.Technic)
                                {
                                    isTechnic = true;
                                }

                                if (abonent.Customer.Type == CustomerType.Juridical)
                                    isjuridical = true;

                                if (abonent.Cards != null)
                                {
                                    foreach (Card crd in abonent.Cards)
                                    {
                                        if (crd.Id == 0) //ADD CARD
                                        {
                                            string ab_num = Utils.Utils.IsAbonentNumExists(_db, crd.AbonentNum);
                                            if (ab_num != string.Empty)
                                                crd.AbonentNum = ab_num;

                                            if (_db.Cards.Any(c => c.DocNum == crd.DocNum))
                                            {
                                                throw new Exception("ბარათი " + crd.AbonentNum + " ხელშეკრულების ნომერი უკვე არსებობს");
                                            }

                                            crd.CustomerId = abonent.Customer.Id;
                                            crd.CloseDate = DateTime.Now;
                                            crd.PauseDate = DateTime.Now;
                                            crd.CardStatus = CardStatus.FreeDays;
                                            crd.UserId = ((User)Session["CurrentUser"]).Id;
                                            crd.CardLogs = new List<CardLog>() { new CardLog() { Date = crd.Tdate, Status = CardLogStatus.Montage, UserId = user_id } };
                                            crd.AbonentNum = new WebService1().getAbonentNum();
                                            crd.DocNum = new WebService1().getDocNum();
                                            crd.CardStatus = CardStatus.Closed;
                                            crd.FinishDate = DateTime.Now;
                                            crd.juridical_verify_status = "-1";
                                            crd.JuridVerifyStatus = CardJuridicalVerifyStatus.None;
                                            crd.RentFinishDate = DateTime.Now;
                                            //crd.mux1_level = abonent.Cards.Select(s => s.mux1_level).FirstOrDefault();
                                            //crd.mux2_level = abonent.Cards.Select(s => s.mux2_level).FirstOrDefault();
                                            //crd.mux3_level = abonent.Cards.Select(s => s.mux3_level).FirstOrDefault();
                                            //crd.mux1_quality = abonent.Cards.Select(s => s.mux1_quality).FirstOrDefault();
                                            //crd.mux2_quality = abonent.Cards.Select(s => s.mux2_quality).FirstOrDefault();
                                            //crd.mux3_quality = abonent.Cards.Select(s => s.mux3_quality).FirstOrDefault();
                                            //if (abonent.isFromDiler == true && abonent.dilerCards != null && abonent.dilerCards.card_id != null && abonent.dilerCards.card_id.Contains(0))
                                            //{
                                            //    crd.UserId = abonent.dilerCards.diler_id;
                                            //}
                                            //else
                                            //{
                                            //crd.UserId = user_id;
                                            //}
                                            //for (int i = 0; i < abonent.dilerCards.card_id.Count(); i++)
                                            //{

                                            //    if (crd.CardNum == abonent.dilerCards.card_id[i].ToString())
                                            //    {
                                            //       crd.UserId = abonent.dilerCards.diler_id;
                                            //    }
                                            //}
                                            CardEditCasData _cas = new CardEditCasData();
                                            _cas.CardId = crd.Id;
                                            _cas.CardNum = int.Parse(crd.CardNum);
                                            _cas.CasIds = new short[] { };
                                            _cas.Status = crd.CardStatus;
                                            _cas.FinishDate = crd.FinishDate;
                                            _cas.Date = crd.CasDate;

                                            if (crd.Subscribtions != null)
                                            {
                                                string paket = "";
                                                short[] ids = { };
                                                foreach (Subscribtion subscrib in crd.Subscribtions)
                                                {
                                                    int[] arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                    var _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                                                    if (_packages.Any(p => p.RentType != RentType.block && p.RentType != RentType.technic))
                                                    {
                                                        if (!subscrib.SubscriptionPackages.Any(s => s.PackageId == 304073))
                                                        {
                                                            subscrib.SubscriptionPackages.Add(new SubscriptionPackage() { PackageId = _db.Packages.Where(pack => pack.RentType == RentType.block).Select(p => p.Id).First() });
                                                            arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                            _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                                                        }
                                                    }
                                                    subscrib.Amount = _packages.Select(p => abonent.Customer.Type == CustomerType.Juridical ? p.JuridPrice : p.Price).Sum();
                                                    subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                                    subscrib.Status = true;
                                                    subscrib.UserId = user_id;
                                                    subscrib.Tdate = DateTime.Now;

                                                    paket += "+" + String.Join("+", _packages.Select(p => p.Name));
                                                    ids = ids.Union(_packages.Select(p => (short)p.CasId)).ToArray();
                                                }
                                                _db.SaveChanges();

                                                paket = paket.Substring(1);
                                                paket = abonent.Customer.Name + " " + abonent.Customer.LastName + " ის ბარათზე " + crd.CardNum + ", პაკეტები:" + paket;
                                                this.AddLoging(_db,
                                                    LogType.CardPackage,
                                                    LogMode.Add,
                                                    user_id,
                                                    crd.Id,
                                                    paket,
                                                    new List<LoggingData>()
                                                );

                                                _cas.CasIds = ids;
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                var defaultPackages = _db.Packages.Where(p => p.IsDefault || p.RentType == RentType.block).ToList();
                                                if (defaultPackages.Count > 0)
                                                {
                                                    crd.Subscribtions = new List<Subscribtion>()
                                                        {
                                                            new Subscribtion {
                                                                Amount =  abonent.Customer.Type == CustomerType.Juridical ? defaultPackages.Select(p=>p.JuridPrice).Sum() : defaultPackages.Select(p=>p.Price).Sum(),
                                                                Status = true,
                                                                Tdate = DateTime.Now,
                                                                UserId = user_id,
                                                                SubscriptionPackages = defaultPackages.Select(s=>new SubscriptionPackage
                                                                {
                                                                    PackageId = s.Id
                                                                }).ToList()
                                                            }
                                                        };
                                                    var subscrib = crd.Subscribtions.First();
                                                    subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                                    _cas.CasIds = defaultPackages.Select(c => (short)c.CasId).ToArray();
                                                }
                                            }

                                            string charge_time = _params.Where(p => p.Name == "CardCharge").First().Value;
                                            if (abonent.Customer.Type == CustomerType.Juridical && abonent.Customer.IsBudget)
                                            {
                                                crd.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(crd.Tdate, charge_time, (decimal)crd.Subscribtions.Where(s => s.Status).Sum(s => s.Amount), decimal.Parse(_params.Where(p => p.Name == "JuridLimitMonths").First().Value), crd.Discount, free_days);
                                                if (!crd.HasFreeDays)
                                                {
                                                    crd.CardStatus = CardStatus.Active;
                                                    free_days = 0;

                                                    //original code
                                                    //decimal amount = (decimal)(crd.Subscribtions.Sum(s => s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                                                    decimal amount = (decimal)(crd.Subscribtions.Sum(s => s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) / Utils.Utils.divide_card_charge_interval);

                                                    amount -= (amount * (decimal)crd.Discount / 100);
                                                    crd.CardCharges = new List<CardCharge>() { new CardCharge() { Amount = amount, Tdate = crd.CasDate, Status = CardChargeStatus.PreChange } };
                                                }
                                            }
                                            else
                                            {

                                                if (!crd.HasFreeDays || free_days == 0)
                                                {
                                                    crd.CardStatus = CardStatus.Closed;
                                                    free_days = 0;
                                                    _cas.Status = crd.CardStatus;
                                                }
                                                else if (crd.HasFreeDays && free_days > 0)
                                                {
                                                    crd.CardStatus = CardStatus.FreeDays;
                                                }
                                                if (abonent.Customer.Type != CustomerType.Juridical)
                                                    crd.FinishDate = Utils.Utils.GenerateFinishDate(crd.Tdate, charge_time).AddDays(free_days);
                                                else
                                                {
                                                    crd.CardStatus = CardStatus.Active;
                                                    //if (_db.Cards.Where(c => c.Id == crd.Id).FirstOrDefault().CardStatus == CardStatus.Active)
                                                    {
                                                        crd.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(charge_time);
                                                        card_logg_juridical = true;
                                                        //CardLog _log = new CardLog() { CardId = crd.Id, Date = DateTime.Now, Status = CardLogStatus.Open, UserId = user_id };
                                                        //_db.CardLogs.Add(_log);

                                                    }
                                                }

                                                if (abonent.Customer.Type == CustomerType.Technic)
                                                {
                                                    crd.CardStatus = CardStatus.Active;
                                                    _cas.Status = crd.CardStatus;
                                                    crd.FinishDate = Utils.Utils.GenerateFinishDate(crd.Tdate, charge_time).AddYears(20);
                                                }

                                                //if (abonent.Customer.Type != CustomerType.Technic)
                                                //{
                                                //    if (!crd.HasFreeDays)
                                                //    {
                                                //        crd.CardStatus = CardStatus.Closed;
                                                //        free_days = 0;
                                                //        _cas.Status = crd.CardStatus;
                                                //    }
                                                //    crd.FinishDate = Utils.Utils.GenerateFinishDate(crd.Tdate, charge_time).AddDays(free_days);
                                                //}
                                                //else if(abonent.Customer.Type == CustomerType.Technic)
                                                //{
                                                //    crd.CardStatus = CardStatus.Active;
                                                //    free_days = 0;
                                                //    _cas.Status = crd.CardStatus;
                                                //    crd.FinishDate = Utils.Utils.GenerateFinishDate(crd.Tdate, charge_time).AddYears(20);
                                                //}
                                            }

                                            if (crd.CardServices != null)
                                            {
                                                List<int> serv_ids = new List<int>();
                                                foreach (CardService _serv in crd.CardServices)
                                                {
                                                    _serv.Date = crd.Tdate;
                                                    _serv.CardId = crd.Id;
                                                    _serv.IsActive = _serv.PayType == CardServicePayType.NotCash;

                                                    serv_ids.Add(_serv.ServiceId);
                                                }

                                                this.AddLoging(_db,
                                                            LogType.CardService,
                                                            LogMode.Add,
                                                            user_id,
                                                            crd.Id,
                                                            crd.AbonentNum + " - ის მომსახურება ",
                                                            _db.Services.Where(c => serv_ids.Contains(c.Id)).Select(c => new LoggingData { field = "მომსახურება", new_val = c.Name }).ToList()
                                                        );
                                            }

                                            crd.LogTower = "";// _db.Towers.FirstOrDefault(c => c.Id == crd.TowerId).Name;
                                            crd.LogReceiver = _db.Receivers.FirstOrDefault(c => c.Id == crd.ReceiverId).Name;


                                            _db.Cards.Add(crd);
                                            _CAS_data.Add(_cas);
                                            _db.SaveChanges();
                                            if (card_logg_juridical == true)
                                            {
                                                CardLog _log = new CardLog() { CardId = crd.Id, Date = DateTime.Now, Status = CardLogStatus.Open, UserId = user_id };
                                                _db.CardLogs.Add(_log);
                                                _db.SaveChanges();
                                            }
                                            if (crd.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Any(s => s.PackageId == 304084))
                                            {
                                                List<Package> packToChangeTo = _db.Packages.Where(p => p.Id == 304085 || p.RentType == RentType.block).ToList();
                                                AutoSubscribChangeCard _card = new AutoSubscribChangeCard()
                                                {
                                                    CardId = crd.Id,
                                                    UserId = user_id,
                                                    CasIds = String.Join(",", packToChangeTo.Select(p => p.CasId)),
                                                    PackageNames = String.Join("+", packToChangeTo.Select(p => p.Name)),
                                                    PackageIds = String.Join(",", packToChangeTo.Select(p => p.Id)),
                                                    Amount = packToChangeTo.Select(c => c.Price).Sum(),
                                                    SendDate = DateTime.Now.AddDays(30)
                                                };
                                                _db.AutoSubscribChangeCards.Add(_card);
                                                _db.SaveChanges();
                                            }
                                            if (crd.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Any(s => s.PackageId == 304086))
                                            {
                                                List<Package> packToChangeTo = _db.Packages.Where(p => p.Id == 304071 || p.RentType == RentType.block).ToList();
                                                AutoSubscribChangeCard _card = new AutoSubscribChangeCard()
                                                {
                                                    CardId = crd.Id,
                                                    UserId = user_id,
                                                    CasIds = String.Join(",", packToChangeTo.Select(p => p.CasId)),
                                                    PackageNames = String.Join("+", packToChangeTo.Select(p => p.Name)),
                                                    PackageIds = String.Join(",", packToChangeTo.Select(p => p.Id)),
                                                    Amount = packToChangeTo.Select(c => c.Price).Sum(),
                                                    SendDate = DateTime.Now.AddDays(30)
                                                };
                                                _db.AutoSubscribChangeCards.Add(_card);
                                                _db.SaveChanges();
                                            }
                                            this.AddLoging(_db,
                                                    LogType.Card,
                                                    LogMode.Add,
                                                    user_id,
                                                    crd.Id,
                                                    crd.AbonentNum,
                                                    new List<LoggingData>()//Utils.Utils.GetAddedData(typeof(Card), crd)
                                                );

                                            _db.JuridicalStatus.Add(new JuridicalStatus()
                                            {
                                                tdate = DateTime.Now,
                                                card_id = crd.Id,
                                                user_id = user_id,
                                                status = -1

                                            });
                                            _db.JuridicalLoggings.Add(new JuridicalLogging()
                                            {
                                                tdate = DateTime.Now,
                                                card_id = crd.Id,
                                                user_id = 1,
                                                status = -1,
                                                name = _db.Database.SqlQuery<string>("SELECT u.name FROM book.Users u where u.id=" + 1).FirstOrDefault()
                                            });

                                        }
                                        else
                                        {
                                            bool packet_cahnge = false;
                                            CardDetailData _card = _db.Cards.Where(c => c.Id == crd.Id).Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                                            {
                                                PaymentAmount = c.Payments.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                                Card = c,
                                                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                            }).FirstOrDefault();
                                            if (_card != null)
                                            {

                                                DateTime old_finish_date = _card.Card.FinishDate;
                                                string old_card = _card.Card.CardNum;
                                                if (Utils.Utils.GetPermission("CARD_EDIT"))
                                                {
                                                    int ApproveStatus = _db.Cards.Where(c => c.Id == crd.Id).Select(c => c.ApproveStatus).First();

                                                    _card.Card.AbonentNum = crd.AbonentNum;
                                                    _card.Card.CardNum = crd.CardNum;
                                                    _card.Card.Address = crd.Address;
                                                    _card.Card.Discount = crd.Discount;
                                                    _card.Card.Group = crd.Group;
                                                    _card.Card.DocNum = crd.DocNum;
                                                    _card.Card.ReceiverId = crd.ReceiverId;
                                                    _card.Card.TowerId = crd.TowerId;
                                                    _card.Card.mux1_level = crd.mux1_level;
                                                    _card.Card.mux2_level = crd.mux2_level;
                                                    _card.Card.mux3_level = crd.mux3_level;
                                                    _card.Card.mux1_quality = crd.mux1_quality;
                                                    _card.Card.mux2_quality = crd.mux2_quality;
                                                    _card.Card.mux3_quality = crd.mux3_quality;
                                                    _card.Card.AutoInvoice = crd.AutoInvoice;
                                                    //_card.Card.UserId = user_id;
                                                    _card.Card.City = crd.City;
                                                    _card.Card.Village = crd.Village;
                                                    _card.Card.Region = crd.Region;
                                                    _card.Card.ClosedIsPen = crd.ClosedIsPen;
                                                    //_card.Card.CardStatus = crd.CardStatus;
                                                    _card.Card.ApproveStatus = ApproveStatus == 2 || ApproveStatus == 3 ? 3 : 1;
                                                    _card.Card.juridical_verify_status = "-1";
                                                    //for (int i = 0; i < abonent.dilerCards.card_id.Count(); i++)
                                                    //{

                                                    if (abonent.isFromDiler == true && abonent.dilerCards != null && abonent.dilerCards.card_id != null && abonent.dilerCards.card_id.Contains(Convert.ToInt32(_card.Card.CardNum)))
                                                    {
                                                        _card.Card.UserId = abonent.dilerCards.diler_id;
                                                    }
                                                    //}
                                                    string charge_time = _params.Where(p => p.Name == "CardCharge").First().Value;
                                                    if (_card.Card.CardStatus == CardStatus.FreeDays)
                                                    {
                                                        _card.Card.Tdate = crd.Tdate;


                                                        if (abonent.Customer.Type == CustomerType.Juridical && abonent.Customer.IsBudget)
                                                            //crd.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(crd.Tdate, charge_time, (decimal)_card.SubscribAmount, decimal.Parse(_params.Where(p => p.Name == "JuridLimitMonths").First().Value), crd.Discount, free_days);
                                                            crd.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(charge_time);
                                                        else
                                                        {
                                                            if (!crd.HasFreeDays)
                                                            {
                                                                crd.CardStatus = CardStatus.Closed;
                                                                free_days = 0;
                                                            }
                                                            crd.FinishDate = Utils.Utils.GenerateFinishDate(crd.Tdate, charge_time).AddDays(free_days);


                                                        }
                                                    }

                                                    _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                                                }

                                                CardEditCasData _cas = new CardEditCasData();
                                                _cas.CardId = crd.Id;
                                                _cas.CardNum = int.Parse(crd.CardNum);
                                                _cas.CasIds = new short[] { };
                                                _cas.FinishDate = _card.Card.FinishDate;
                                                _cas.OldFinishDate = old_finish_date;
                                                _cas.Status = _card.Card.CardStatus;
                                                _cas.Date = _card.Card.CasDate;
                                                if (old_card != crd.CardNum)
                                                {
                                                    _cas.ResendCard = int.Parse(old_card);
                                                }

                                                if (crd.Subscribtions != null)
                                                {
                                                    Subscribtion updSubscrbs = _db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == crd.Id).OrderByDescending(s => s.Tdate).FirstOrDefault();
                                                    if (updSubscrbs != null)
                                                    {

                                                        updSubscrbs.Status = false;
                                                        _db.Entry(updSubscrbs).State = System.Data.Entity.EntityState.Modified;
                                                        _db.SaveChanges();

                                                        _cas.DeactiveCasIds = updSubscrbs.SubscriptionPackages.Select(c => (short)c.Package.CasId).ToArray();


                                                        string paket = "";
                                                        short[] ids = { };
                                                        double min_price = 0;
                                                        foreach (Subscribtion subscrib in crd.Subscribtions)
                                                        {
                                                            //if (updSubscrbs.SubscriptionPackages.Where(s => s.PackageId == 304085).FirstOrDefault() != null)
                                                            //{

                                                            //}

                                                            if (updSubscrbs.SubscriptionPackages.Where(s => s.PackageId == 304086).FirstOrDefault() != null)
                                                            {
                                                                pack_change = true;
                                                                List<Package> packToChangeTo = _db.Packages.Where(p => p.Id == 304085 || p.RentType == RentType.block).ToList();
                                                                var FinishDate = _db.Cards.Where(c => c.Id == crd.Id).FirstOrDefault().Tdate.AddDays(30);
                                                                AutoSubscribChangeCard autoSubscrib = _db.AutoSubscribChangeCards.Where(c => c.CardId == crd.Id).FirstOrDefault();
                                                                if (autoSubscrib != null)
                                                                {
                                                                    _db.AutoSubscribChangeCards.Remove(autoSubscrib);
                                                                    _db.SaveChanges();
                                                                }
                                                                AutoSubscribChangeCard autoSub = new AutoSubscribChangeCard()
                                                                {
                                                                    CardId = crd.Id,
                                                                    UserId = user_id,
                                                                    CasIds = String.Join(",", packToChangeTo.Select(p => p.CasId)),
                                                                    PackageNames = String.Join("+", packToChangeTo.Select(p => p.Name)),
                                                                    PackageIds = String.Join(",", packToChangeTo.Select(p => p.Id)),
                                                                    Amount = packToChangeTo.Select(c => c.Price).Sum(),
                                                                    SendDate = new DateTime(FinishDate.Year, FinishDate.Month, FinishDate.Day, FinishDate.Hour, FinishDate.Minute, FinishDate.Second)
                                                                };
                                                                _db.AutoSubscribChangeCards.Add(autoSub);
                                                                _db.SaveChanges();

                                                                PromoCahngePack promoSendSMS = new PromoCahngePack()
                                                                {
                                                                    card_id = crd.Id,
                                                                    user_id = user_id,
                                                                    tdate = new DateTime(FinishDate.Year, FinishDate.Month, FinishDate.Day, FinishDate.Hour, FinishDate.Minute, FinishDate.Second)
                                                                };
                                                                _db.PromoCahngePacks.Add(promoSendSMS);
                                                                _db.SaveChanges();

                                                                int[] arr = _db.Packages.Where(p => p.Id == 304086 || p.RentType == RentType.block).ToList().Select(s => s.Id).ToArray();
                                                                var _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();

                                                                if (_packages.Any(p => p.RentType != RentType.block && p.RentType != RentType.technic))
                                                                {
                                                                    subscrib.SubscriptionPackages.FirstOrDefault().PackageId = 304086;
                                                                    subscrib.SubscriptionPackages.Add(new SubscriptionPackage() { PackageId = _db.Packages.Where(pack => pack.RentType == RentType.block).Select(p => p.Id).First() });
                                                                    //arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                                    _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                                                                }

                                                                subscrib.Tdate =crd.Tdate;
                                                                subscrib.UserId = ((User)Session["CurrentUser"]).Id;
                                                                subscrib.Amount = _packages.Select(p => abonent.Customer.Type == CustomerType.Juridical ? p.JuridPrice : p.Price).Sum();
                                                                subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                                                subscrib.CardId = crd.Id;
                                                                subscrib.Status = true;

                                                                //subscribtion_promo = subscrib;

                                                                _db.Subscribtions.Add(subscrib);
                                                                _db.SaveChanges();

                                                                min_price += _packages.Sum(p => p.MinPrice);
                                                                paket += "+" + String.Join("+", _packages.Select(p => p.Name));
                                                                ids = ids.Union(_packages.Select(p => (short)p.CasId)).ToArray();

                                                                var message_text = _db.MessageTemplates.Where(m => m.Name == "Promo_PackCahnge").FirstOrDefault();
                                                                var message_text_geo= _db.MessageTemplates.Where(m => m.Name == "Promo_PackCahnge_Geo").FirstOrDefault();
                                                                Task.Run(async () => { await Utils.Utils.sendMessage(_db.Customers.Where(cs => cs.Id == crd.CustomerId).Select(cu => cu.Phone1).FirstOrDefault(), String.Format(message_text.Desc, FinishDate.ToString("dd/MM/yyyy"))); }).Wait();
                                                                SendOSDRequesSMS sendOSDReques = new SendOSDRequesSMS();
                                                                sendOSDReques.SendOSD(crd.CardNum, String.Format(message_text_geo.Desc, FinishDate.ToString("dd/MM/yyyy")), _db.Params.ToList());
                                                            }
                                                            else
                                                            {
                                                           
                                                                int[] arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                                var _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();

                                                                if (_packages.Any(p => p.RentType != RentType.block && p.RentType != RentType.technic))
                                                                {
                                                                    subscrib.SubscriptionPackages.Add(new SubscriptionPackage() { PackageId = _db.Packages.Where(pack => pack.RentType == RentType.block).Select(p => p.Id).First() });
                                                                    arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                                    _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                                                                }


                                                                subscrib.Tdate = DateTime.Now;
                                                                subscrib.UserId = ((User)Session["CurrentUser"]).Id;
                                                                subscrib.Amount = _packages.Select(p => abonent.Customer.Type == CustomerType.Juridical ? p.JuridPrice : p.Price).Sum();
                                                                subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                                                subscrib.CardId = crd.Id;
                                                                subscrib.Status = true;

                                                                _db.Subscribtions.Add(subscrib);
                                                                _db.SaveChanges();

                                                                min_price += _packages.Sum(p => p.MinPrice);
                                                                paket += "+" + String.Join("+", _packages.Select(p => p.Name));
                                                                ids = ids.Union(_packages.Select(p => (short)p.CasId)).ToArray();

                                                                var packpromo = _db.Subscribtions.Include("SubscriptionPackages.Package").Where(c => c.CardId == crd.Id && c.Status == false).OrderBy(c => c.Tdate).FirstOrDefault();
                                                                var packStandard = _db.Subscribtions.Include("SubscriptionPackages.Package").Where(c => c.CardId == crd.Id && c.Status == false).OrderByDescending(c => c.Tdate).FirstOrDefault();

                                                                if (packpromo != null && packStandard != null && packpromo.SubscriptionPackages.Where(s => s.PackageId == 304086).FirstOrDefault() != null && packStandard.SubscriptionPackages.Where(s => s.PackageId == 304071).FirstOrDefault() != null)
                                                                {
                                                                    standard_promo = 1;
                                                                }
                                                            }

                                                        }

                                                        _cas.CasIds = ids;

                                                        //if (packet_cahnge == false)
                                                        {

                                                            //------
                                                            if (crd.Subscribtions.First().SubscriptionPackages.Any(s => s.Package.RentType == RentType.buy))
                                                            {
                                                                if (crd.Subscribtions.First().SubscriptionPackages.Any(s => s.Package.RentType == RentType.block))
                                                                {
                                                                    _cas.unblock = true;
                                                                }
                                                            }


                                                            if (((User)Session["CurrentUser"]).Type != 1)
                                                            {
                                                                var promo = _db.Subscribtions.Include("SubscriptionPackages").Where(s => s.CardId == _card.Card.Id && s.SubscriptionPackages.Any(a => a.PackageId == 304086)).FirstOrDefault();
                                                                if (_card.SubscribAmount > crd.Subscribtions.Sum(s => s.Amount) && promo == null) //გადადის დაბალ პაკეტზე
                                                                {
                                                                    int package_days = int.Parse(_params.First(c => c.Name == "PacketChangeTime").Value);
                                                                    if ((DateTime.Now - updSubscrbs.Tdate).Days < package_days && _card.Card.CardStatus != CardStatus.Closed)
                                                                    {
                                                                        throw new Exception(_card.Card.CardNum + " - ზე არჩეული ბოლო პაკეტიდან არ არის გასული " + package_days + " დღე!");
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (!Utils.Utils.GetPermission("PACKAGES_CHARGES_Coercion"))
                                                                    {
                                                                        if (Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount) < (decimal)min_price && _card.Card.CardStatus != CardStatus.Closed)
                                                                        {
                                                                            throw new Exception(_card.Card.CardNum + " - ზე არჩეული მინიმალური ფასი აღემატება ბარათის ბალანსს!");
                                                                        }
                                                                    }

                                                                    if (_card.Card.CardStatus != CardStatus.Closed)
                                                                    {
                                                                        //original code
                                                                        //decimal old_amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                                                                        decimal old_amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) / Utils.Utils.divide_card_charge_interval);

                                                                        old_amount -= (old_amount * (decimal)_card.Card.Discount / 100);

                                                                        //original code
                                                                        //decimal new_amount = (decimal)(crd.Subscribtions.Sum(s => s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                                                                        decimal new_amount = (decimal)(crd.Subscribtions.Sum(s => s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) / Utils.Utils.divide_card_charge_interval);

                                                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = new_amount - old_amount, Tdate = DateTime.Now, Status = CardChargeStatus.PacketChange });
                                                                    }
                                                                }
                                                            }

                                                            paket = paket.Substring(1);
                                                            paket = abonent.Customer.Name + " " + abonent.Customer.LastName + " ის ბარათზე " + crd.CardNum + ", პაკეტები:" + paket;
                                                            this.AddLoging(_db,
                                                                LogType.CardPackage,
                                                                LogMode.Add,
                                                                ((User)Session["CurrentUser"]).Id,
                                                                crd.Id,
                                                                paket,
                                                                new List<LoggingData>()
                                                            );
                                                        }
                                                    }
                                                }


                                                if (crd.CardServices != null)
                                                {
                                                    List<int> serv_ids = new List<int>();
                                                    foreach (CardService _serv in crd.CardServices)
                                                    {
                                                        _serv.Date = DateTime.Now;
                                                        _serv.CardId = crd.Id;
                                                        _serv.IsActive = _serv.PayType == CardServicePayType.NotCash;

                                                        serv_ids.Add(_serv.ServiceId);
                                                        if (_card.Card.CardStatus == CardStatus.Closed && _serv.IsActive)
                                                        {
                                                            _db.CardCharges.Add(new CardCharge { Tdate = _serv.Date, CardId = _card.Card.Id, Status = CardChargeStatus.Service, Amount = _serv.Amount });
                                                            _serv.IsActive = false;
                                                        }
                                                        _db.Entry(_serv).State = System.Data.Entity.EntityState.Added;
                                                    }

                                                    this.AddLoging(_db,
                                                            LogType.CardService,
                                                            LogMode.Add,
                                                            user_id,
                                                            crd.Id,
                                                            crd.AbonentNum + " - ის მომსახურება ",
                                                            _db.Services.Where(c => serv_ids.Contains(c.Id)).Select(c => new LoggingData { field = "მომსახურება", new_val = c.Name }).ToList()
                                                        );

                                                }
                                                //if (packet_cahnge == false)
                                                {
                                                    if (!isjuridical)
                                                    {
                                                        if (_card.Card.CardStatus != CardStatus.FreeDays && !(_card.Card.CardStatus == CardStatus.Active && _card.Card.Mode == 1) && abonent.Customer.Type != CustomerType.Technic)
                                                        {
                                                            Utils.Utils.SetFinishDate(_db, jurid_limit_months, crd.Id);
                                                        }

                                                        if (abonent.Customer.Type == CustomerType.Technic)
                                                        {
                                                            _card.Card.CardStatus = crd.CardStatus = CardStatus.Active;
                                                            //_cas.Status = crd.CardStatus;
                                                            _card.Card.FinishDate = crd.FinishDate = Utils.Utils.GenerateFinishDate("00:01").AddYears(20).AddHours(-4);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (_card.Card.CardStatus == CardStatus.Active)
                                                            Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, crd.Id);

                                                    }

                                                    _cas.FinishDate = _card.Card.FinishDate;
                                                    _CAS_data.Add(_cas);

                                                    if (standard_promo == 1)
                                                    {
                                                        MessageTemplate message = new MessageTemplate();
                                                        string messageText = "";

                                                        CardDetailData _cardt = _db.Cards.Where(c => c.Id == crd.Id).Select(c => new CardDetailData
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

                                                        if (_cardt.Card.CardStatus == CardStatus.Closed)
                                                        {



                                                            double balance = Math.Round((double)Utils.Utils.GetBalance(_cardt.PaymentAmount, _cardt.ChargeAmount), 2);
                                                            if (balance <= 0)
                                                            {
                                                                balance = balance - (double)_cardt.ServiceAmount;
                                                            }
                                                            if (balance > 0 && balance < (double)_cardt.SubscribAmount)
                                                            {
                                                                balance = (double)_cardt.SubscribAmount- balance;
                                                            }
                                                            if (balance >= (double)_cardt.SubscribAmount)
                                                            {
                                                                message = _db.MessageTemplates.Where(m => m.Name == "Promo_Change_Active8").FirstOrDefault();
                                                                messageText = String.Format(message.Desc, _cardt.Card.FinishDate);
                                                            }
                                                            else
                                                            {
                                                                message = _db.MessageTemplates.Where(m => m.Name == "Promo_Change_Active8_false").FirstOrDefault();
                                                                messageText = String.Format(message.Desc,crd.AbonentNum ,balance);
                                                            }

                                                            string phoneto = _db.Customers.Where(cs => cs.Id == crd.CustomerId).Select(cu => cu.Phone1).FirstOrDefault();
                                                            Task.Run(async () => { await Utils.Utils.sendMessage(phoneto, messageText); }).Wait();
                                                            _db.MessageLoggings.Add(new MessageLogging()
                                                            {
                                                                card_id = crd.Id,
                                                                tdate = DateTime.Now,
                                                                status = MessageLoggingStatus.PromoCahngeActive8,
                                                                message_id = message.Id


                                                            });
                                                            _db.SaveChanges();
                                                        }
                                                        if (_cardt.Card.CardStatus == CardStatus.Active)
                                                        {
                                                            message = _db.MessageTemplates.Where(m => m.Name == "Promo_Change_Active8").FirstOrDefault();
                                                            messageText = String.Format(message.Desc, _cardt.Card.FinishDate);

                                                            string phonetos = _db.Customers.Where(cs => cs.Id == crd.CustomerId).Select(cu => cu.Phone1).FirstOrDefault();

                                                            Task.Run(async () => { await Utils.Utils.sendMessage(phonetos, messageText); }).Wait();

                                                            _db.MessageLoggings.Add(new MessageLogging()
                                                            {
                                                                card_id = crd.Id,
                                                                tdate = DateTime.Now,
                                                                status = MessageLoggingStatus.PromoCahngeClosed8,
                                                                message_id = message.Id


                                                            });
                                                            _db.SaveChanges();
                                                            //message_geo = _db.MessageTemplates.Where(m => m.Name == "OnShare8Active_GEO").FirstOrDefault();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    _db.SaveChanges();
                                }

                                string[] address = _params.First(c => c.Name == "CASAddress").Value.Split(':');
                                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                _socket.Connect();
                                foreach (CardEditCasData cas in _CAS_data)
                                {
                                    _socket.SessionId++;
                                    if (cas.CardId == 0)
                                    {
                                        if (cas.Status != CardStatus.Closed)
                                        {
                                            if (!_socket.SendCardStatus(cas.CardNum, true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                            {
                                                throw new Exception("ბარათი ვერ გააქტიურდა:" + cas.CardNum);
                                            }

                                            //დროებითი
                                            if (!_socket.SendEntitlementRequestTemp(Convert.ToInt32(cas.CardNum), cas.CasIds, new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), false))
                                            {
                                            }

                                            SendTempCas(_db, _socket, cas.CardNum.ToString());

                                            if (!_socket.SendEntitlementRequest(Convert.ToInt32(cas.CardNum), cas.CasIds, DateTime.SpecifyKind(cas.Date, DateTimeKind.Utc), true))
                                            {
                                                throw new Exception("ბარათის პაკეტები ვერ გააქტიურდა:" + cas.CardNum);
                                            }
                                        }
                                        else
                                        {
                                            //დროებითი
                                            if (!_socket.SendEntitlementRequestTemp(Convert.ToInt32(cas.CardNum), new short[] { 3, 4 }, new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), false))
                                            {
                                            }

                                            SendTempCas(_db, _socket, cas.CardNum.ToString());
                                        }
                                    }
                                    else
                                    {
                                        CardDetailData _card = _db.Cards.Where(c => c.Id == cas.CardId).Select(c => new CardDetailData
                                        {
                                            PaymentAmount = c.Payments.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                            Card = c,
                                            SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                                            MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                        }).FirstOrDefault();
                                        if (cas.CasIds.Length > 0)
                                        {
                                            if (_card != null)
                                            {
                                                if (_card.Card.CardStatus == CardStatus.Closed)
                                                {
                                                    double min_price = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count() == 1 ? _card.SubscribAmount : _card.MinPrice;
                                                    if (Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount) >= (decimal)min_price)
                                                    {
                                                        cas.Status = CardStatus.Active;
                                                        _card.Card.Mode = 0;
                                                        _card.Card.CardStatus = CardStatus.Active;
                                                        CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.Open, UserId = user_id };
                                                        _db.CardLogs.Add(_log);

                                                        //original code
                                                        //decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                                                        decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) / Utils.Utils.divide_card_charge_interval);

                                                        amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = DateTime.Now, Status = CardChargeStatus.PreChange });

                                                        _db.Entry(_card.Card).State = EntityState.Modified;
                                                        _db.SaveChanges();

                                                        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    }
                                                }
                                            }
                                        }


                                        if (cas.Status != CardStatus.Active && cas.Status != CardStatus.FreeDays)
                                            continue;

                                        if (cas.ResendCard != 0)
                                        {
                                            if (!_socket.SendCardStatus(cas.CardNum, true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                            {
                                                throw new Exception("ბარათის პაკეტები ვერ გააქტიურდა:" + cas.CardNum);
                                            }

                                            short[] casids = _db.Subscribtions.Include("SubscriptionPackages.Package")
                                                .Where(s => s.CardId == cas.CardId)
                                                .Where(s => s.Status).FirstOrDefault().SubscriptionPackages.Select(c => (short)c.Package.CasId).ToArray();
                                            if (casids.Length > 0)
                                            {
                                                if (!_socket.SendEntitlementRequest(cas.ResendCard, casids, DateTime.SpecifyKind(cas.Date, DateTimeKind.Utc), false))
                                                {
                                                    throw new Exception("ბარათის პაკეტები ვერ შაიშალა:" + cas.CardNum);
                                                }

                                                if (!_socket.SendEntitlementRequest(cas.CardNum, casids, DateTime.SpecifyKind(cas.Date, DateTimeKind.Utc), true))
                                                {
                                                    throw new Exception("ბარათის პაკეტები ვერ გააქტიურდა:" + cas.CardNum);
                                                }
                                            }
                                        }

                                        if (cas.DeactiveCasIds != null)
                                        {
                                            if (!_socket.SendEntitlementRequest(cas.CardNum, cas.DeactiveCasIds, cas.OldFinishDate.AddHours(-4), cas.OldFinishDate.AddHours(-4), false))
                                            //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                            {
                                                throw new Exception("ბარათის პაკეტები ვერ შაიშალა:" + cas.CardNum);
                                                //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                            }

                                            //original source
                                            //if (!_socket.SendEntitlementRequest(cas.CardNum, cas.DeactiveCasIds, DateTime.SpecifyKind(cas.Date, DateTimeKind.Utc), false))
                                            //{
                                            //    throw new Exception("ბარათის პაკეტები ვერ შაიშალა:" + cas.CardNum);
                                            //}
                                        }

                                        if (cas.CasIds.Length > 0)
                                        {
                                            if (_card != null)
                                            {
                                                _card.Card.CasDate = DateTime.Now;
                                                cas.Date = _card.Card.CasDate;

                                                _db.Entry(_card.Card).State = EntityState.Modified;
                                                _db.SaveChanges();
                                            }

                                            if (cas.unblock)
                                            {
                                                short[] cas_ids = _db.Packages.Where(p => p.RentType == RentType.block).Select(s => (short)s.CasId).ToArray();
                                                //if (!_socket.SendEntitlementRequest(cas.CardNum, cas_ids, new DateTime(2038, 1, 1, 0, 0, 0, 0), true))
                                                //{
                                                //    throw new Exception("ბარათის პაკეტები ვერ გააქტიურდა:" + cas.CardNum);
                                                //}

                                                if (!_socket.SendEntitlementRequest(cas.CardNum, cas_ids, cas.Date.AddHours(-4), new DateTime(2038, 1, 1, 0, 0, 0, 0), true))
                                                {
                                                    throw new Exception("ბარათის განბლოკვის პაკეტი ვერ გააქტიურდა:" + cas.CardNum);
                                                }
                                            }
                                            else
                                            {
                                                short[] cas_ids = _db.Packages.Where(p => p.RentType == RentType.block).Select(s => (short)s.CasId).ToArray();
                                                if (!_socket.SendEntitlementRequest(cas.CardNum, cas_ids, new DateTime(2038, 1, 1, 0, 0, 0, 0), new DateTime(2038, 1, 1, 0, 0, 0, 0), true))
                                                {
                                                    throw new Exception("ბარათის განბლოკვის პაკეტი ვერ წაიშალა:" + cas.CardNum);
                                                }
                                            }

                                            if (!_socket.SendEntitlementRequest(cas.CardNum, cas.CasIds, cas.Date.AddHours(-4), cas.FinishDate.AddHours(-4), true))
                                            //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                            {
                                                return Json(0);
                                                //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                            }

                                            //original source
                                            //if (!_socket.SendEntitlementRequest(cas.CardNum, cas.CasIds, DateTime.SpecifyKind(cas.Date, DateTimeKind.Utc), true))
                                            //{
                                            //    throw new Exception("ბარათის პაკეტები ვერ გააქტიურდა:" + cas.CardNum);
                                            //}
                                        }
                                    }
                                }
                                _socket.Disconnect();

                                if (Utils.Utils.GetPermission("CARD_EDIT"))
                                {
                                    if (logs != null && logs.Count > 0)
                                    {
                                        foreach (var key in logs.Where(t => t.type != "customer").GroupBy(c => c.type))
                                        {
                                            this.AddLoging(_db,
                                                LogType.Card,
                                                LogMode.Change,
                                                user_id,
                                                int.Parse(key.Key.Split('@')[0]),
                                                key.Key.Split('@')[1],
                                                key.ToList()
                                            );
                                        }
                                        _db.SaveChanges();
                                    }
                                }

                                tran.Commit();
                                if (pack_change == true)
                                {
                                    var cardID = abonent.Cards.Select(s => s.Id).FirstOrDefault();

                                    if (_db.Subscribtions.Where(c => c.CardId == cardID && c.Status == false).FirstOrDefault() != null)
                                    {
                                        _db.Database.ExecuteSqlCommand("DELETE FROM [doc].[SubscriptionPackages] where subscription_id=" + _db.Subscribtions.Where(c => c.CardId == cardID && c.Status==false).FirstOrDefault().Id + "");

                                        _db.Database.ExecuteSqlCommand("DELETE FROM [doc].[Subscribes] where card_id=" + cardID + " and status=0");
                                        //_db.Subscribtions.Remove(_db.Subscribtions.Where(c => c.CardId == cardID && c.Status == false).FirstOrDefault());
                                        //_db.SaveChanges();
                                    }
                                }
                                if (Request["save"] != null)
                                    return new RedirectResult("/Abonent");
                                else
                                    return new RedirectResult(Request.Url.AbsolutePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Error = ex.Message;
                            tran.Rollback();
                        }
                    }
                }
            }
            else
            {
                List<ModelState> states = ViewData.ModelState.Values.Where(x => x.Errors.Count > 0).ToList();
            }


            if (abonent.Cards != null)
            {
                using (DataContext _db = new DataContext())
                {
                    var _cards = _db.Cards.Include("Subscribtions.SubscriptionPackages.Package").Include("CardCharges").Include("CardDamages").Include("CardServices").Include("Payments").Where(c => c.CustomerId == id.Value).ToList();
                    int[] card_ids = abonent.Cards.Select(cr => cr.Id).ToArray();
                    int[] non_canceled = abonent.Cards.Where(cr => cr.CardStatus != CardStatus.Canceled).Select(cr => cr.Id).ToArray();
                    int[] canceled = abonent.Cards.Where(cr => cr.CardStatus == CardStatus.Canceled).Select(cr => cr.Id).ToArray();
                    List<Payment> _payments = _db.Payments.Where(c => card_ids.Contains(c.CardId)).ToList();
                    List<CardCharge> _charges = _db.CardCharges.Where(c => card_ids.Contains(c.CardId)).ToList();

                    abonent.AbonentDetailInfo = new AbonentDetailInfo
                    {
                        Balanse = Math.Round(_payments.Where(c => non_canceled.Contains(c.CardId)).Select(c => c.Amount).Sum() - _charges.Where(c => non_canceled.Contains(c.CardId)).Select(c => c.Amount).Sum(), 3),
                        FinishDate = abonent.Cards.Min(c => c.FinishDate),
                        CanceledCardAmount = Math.Round(_payments.Where(c => canceled.Contains(c.CardId)).Select(c => c.Amount).Sum() - _charges.Where(c => canceled.Contains(c.CardId)).Select(c => c.Amount).Sum(), 3),
                        Chats = _db.CustomersChat.Where(c => c.CustomerId == id.Value).Select(c => new Chat { Tdate = c.Tdate, Message = c.MessageText, UserName = c.User.Name }).ToList()
                    };
                    //for (int j = 0; j < abonent.dilerCards.card_id.Count(); j++)
                    //{

                    //}
                    for (int i = 0; i < abonent.Cards.Count; i++)
                    {
                        Card cur_c = abonent.Cards[i];
                        Card c = _cards.FirstOrDefault(cc => cc.Id == cur_c.Id);
                        if (c == null)
                        {
                            string max_num = _db.Cards.Select(cc => cc.AbonentNum).OrderByDescending(cc => cc).FirstOrDefault();
                            abonent.Cards[i].AbonentNum = new WebService1().getAbonentNum();// Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1)));
                            abonent.Cards[i].Id = 0;

                        }
                        else
                        {
                            abonent.Cards[i] = c;
                        }
                    }

                    ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
                    ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList();
                    ViewBag.Reasons = _db.Reasons.Where(c => c.ReasonType == ReasonType.Damage).Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();
                    List<Param> _params = _db.Params.ToList();

                    ViewBag.PacketChangeTime = int.Parse(_params.First(c => c.Name == "PacketChangeTime").Value);
                    ViewBag.CurrentCard = cur_card;
                    ViewBag.HasReadonly = 0;
                    ViewBag.Balances = abonent.Cards.Select(c => new IdName { Id = c.Id, Name = Utils.Utils.GetBalance(_payments.Where(p => p.CardId == c.Id).Select(p => p.Amount).Sum(), _charges.Where(p => p.CardId == c.Id).Select(p => p.Amount).Sum()).ToString() }).ToList();
                    ViewBag.ServiceDays = int.Parse(_params.First(c => c.Name == "ServiceDays").Value);
                    ViewBag.CardPauseDay = _params.First(p => p.Name == "CardPauseDays").Value;
                    ViewBag.ClosedCardDays = int.Parse(_params.First(p => p.Name == "ClosedCardDays").Value);
                    ViewBag.CloseCardAmount = double.Parse(_params.First(p => p.Name == "CloseCardAmount").Value);



                }
            }

            return View(abonent);
        }

        [HttpGet]
        public PartialViewResult CardPause(int card_id)
        {
            //bool privilegies_return = true;
            //if (!Utils.Utils.GetPermission("CARD_PAUSED_CASE_DEBT"))
            //{
            //    privilegies_return = false;

            //}
            //ViewBag.CardPause = privilegies_return;
            using (DataContext _db = new DataContext())
            {
                Card card = _db.Cards.Where(c => c.Id == card_id).FirstOrDefault();
                //if (DateTime.Now.Month >= card.Tdate.Month && DateTime.Now.Year > card.Tdate.Year && DateTime.Now.Day >= card.Tdate.Day)
                //{
                //    _db.Database.ExecuteSqlCommand("UPDATE book.Cards SET pause_days=0 , has_used_free_pause_days=0,last_pause_type=0,none_free_pause_count_per_month=0");
                //}
                //var number = ((now.Year - compareTo.Year) * 12) + now.Month - compareTo.Month;

                return PartialView("~/Views/Abonent/_PauseCard.cshtml", model: card);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Remove(int? card_id)
        {
            using (DataContext _db = new DataContext())
            {

            }

            return Json(0);
        }

        [HttpPost]
        public PartialViewResult AddCard(int count)
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
                ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList();
                List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType<DescriptionAttribute>(c).Description }).ToList();
                ViewBag.CardStatus = CardEnums;

                ViewBag.Count = count;
                return PartialView("~/Views/Abonent/_Card.cshtml", new Abonent() { Customer = new Customer(), Cards = new List<Card>() { new Card() } });
            }
        }

        public PartialViewResult AddSubscription(int id, CustomerType type)
        {
            ViewBag.CardId = id;
            ViewBag.ShowShare = Utils.Utils.GetPermission("SHARE_8_SHOW");
            ViewBag.ShowShare8 = Utils.Utils.GetPermission("SHARE_8_ACTIVATED_SHOW");
            using (DataContext _db = new DataContext())
            {
                ViewBag.Type = type;
                ViewBag.Discount = Convert.ToDouble(_db.Params.Where(p => p.Name == "PackageDiscount").Select(p => p.Value).First());
                return PartialView("~/Views/Abonent/_AddSubscription.cshtml", _db.Packages.ToList());
            }
        }

        //[HttpPost]
        //public JsonResult SaveReturnedCard(int card_id, string cash, float amount, string comment, int reason, int bort_id)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        try
        //        {
        //            //string json = "{ commisionType: ['ნაღდი', 'ბალანსი', 'ნაღდი/უნაღდო']" + "amount: [" + cash1 + "," + cash2 + "," + cash3+"] }";
        //            int user_id = ((User)Session["CurrentUser"]).Id;
        //            //var cash = (cash1 + "," + cash2 + "," + cash3);

        //            _db.ReturnedCards.Add(new ReturnedCard
        //            {
        //                card_id = card_id,
        //                commission = cash,
        //                user_id = user_id,
        //                bort_id = bort_id,
        //                commission_amount = 5,
        //                returned_amount = amount,
        //                info = comment,
        //                reason = reason

        //            });
        //            _db.SaveChanges();
        //            return Json(1);
        //        }
        //        catch(Exception)
        //        {

        //            return Json(0);
        //        }

        //    }
        //}

        [HttpPost]
        public PartialViewResult GetReturnedCard(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                double commision_ammount = 0;
                string comm = _db.Params.Where(p => p.Name == "ReturnedCardCommision").FirstOrDefault().Value;
                if (comm == null || comm == "")
                {
                    throw new Exception("საკომისიოს რაოდენობა ვერ მოიძებნა!");
                }

                double.TryParse(comm, out commision_ammount);
                ViewBag.CommisionAmount = commision_ammount;
                Card _card = _db.Cards.Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.Id == card_id).FirstOrDefault();
                if (_card != null)
                {
                    var __card = _db.Cards.Where(c => c.Id == _card.Id).Select(c => new CardDetailData
                    {
                        CustomerType = c.Customer.Type,
                        IsBudget = c.Customer.IsBudget,
                        SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                        CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                        PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                        ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        Card = c,
                        SubscriptionPackages = c.Subscribtions.FirstOrDefault().SubscriptionPackages.ToList(),
                        MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                        ServiceAmount = c.CardCharges.Where(s => s.Status == CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        WithoutServiceAmount = c.CardCharges.Where(s => s.Status != CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        CardServices = c.CardServices.ToList()
                    }).First();
                    decimal balance = 0;
                    if (__card.Card.Subscribtions.Count<=2 && __card.SubscriptionPackages.Where(c => c.Package.Id == 304086).FirstOrDefault() != null)
                    {
                        _db.Database.ExecuteSqlCommand($"UPDATE [doc].[Payments]  SET  [amount] = 0 WHERE card_id ={card_id }");
                        ViewBag.Balance = balance;
                    }
                    else
                    {
                        balance = Utils.Utils.GetBalance(__card.PaymentAmount, __card.ChargeAmount);
                        ViewBag.Balance = balance;
                    }
                    if (balance >= (decimal)commision_ammount)
                    {
                        ViewBag.Amount = balance - (decimal)commision_ammount;
                    }
                    else
                    {
                        ViewBag.Amount = 0;
                    }
                    if (__card.SubscriptionPackages.Where(c => c.Package.Id == 304086).FirstOrDefault() != null)
                    {
                        ViewBag.PackName = __card.SubscriptionPackages.Where(c => c.Package.Id == 304086).FirstOrDefault().Package.Name;
                    }
                    else
                    {
                        ViewBag.PackName = "";
                    }
                    ViewBag.card_id = card_id;
                }
                ViewBag.attachmenlist = _db.ReceiverAttachments.ToList();

                List<User> user_info = _db.Users.Include("UserType").Where(c => c.UserType.Name.Contains("დილერი") || c.UserType.Id == 1 || c.UserType.Id == 2 || c.UserType.Id == 4 || c.UserType.Id == 5).ToList();

                return PartialView("~/Views/Abonent/_ReturnedCard.cshtml", user_info);
            }
        }


        [HttpPost]
        public async Task<PartialViewResult> GetCardInfo(int card_id, int cust_id, bool detaled)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            {
                try
                {
                    CardInfo _info;
                    if (card_id != 0)
                    {
                        _info = new CardInfo()
                        {
                            Subscribtions = await _db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                            CardLogs = await _db.CardLogs.Include("User").Where(c => c.CardId == card_id).Where(c => c.Date >= dateFrom && c.Date <= dateTo).ToListAsync(),
                            Payments = await _db.Payments.Include("PayType").Where(c => c.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                            OtherCharges = await _db.CardCharges.Where(c => c.CardId == card_id).Where(c => c.Status != CardChargeStatus.Daily).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                            Balances = await _db.CardCharges.Where(c => c.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(c => new Balance()
                            {
                                Tdate = c.Tdate,
                                OutAmount = c.Amount,
                                RAmount = c.RentAmount,
                                InAmount = 0,
                                InRentAmount = 0,
                                OutAmountStatus = c.Status,
                                RentAmount = (c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.PayRent) ?? 0) - (c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.RentAmount).Sum() ?? 0),
                                CurrentBalance = (c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                            })
                            .Concat(_db.Payments.Where(p => p.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(p => new Balance()
                            {
                                Tdate = p.Tdate,
                                OutAmount = 0,
                                RAmount = 0,
                                InAmount = p.Amount,
                                InRentAmount = p.PayRent,
                                OutAmountStatus = CardChargeStatus.Daily,
                                RentAmount = (p.Card.Payments.Where(s => s.Tdate <= p.Tdate).Sum(s => (decimal?)s.PayRent) ?? 0) - (p.Card.CardCharges.Where(s => s.Tdate <= p.Tdate).Select(s => (decimal?)s.RentAmount).Sum() ?? 0),
                                CurrentBalance = (p.Card.Payments.Where(s => s.Tdate <= p.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (p.Card.CardCharges.Where(s => s.Tdate <= p.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                            })).OrderBy(c => c.Tdate).ToListAsync(),
                            CardServices = await _db.CardServices.Include("Service").Where(c => c.CardId == card_id).Where(c => c.Date >= dateFrom && c.Date <= dateTo).Select(c => new CardServicesList
                            {
                                Name = c.Service.Name,
                                PayType = c.PayType,
                                Price = c.Amount,
                                Date = c.Date
                            }).ToListAsync()
                        };
                    }
                    else
                    {
                        _info = new CardInfo()
                        {
                            Subscribtions = await _db.Subscribtions.Include("Card").Include("SubscriptionPackages.Package").Where(s => s.Card.CustomerId == cust_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                            CardLogs = await _db.CardLogs.Include("User").Include("Card").Where(c => c.Card.CustomerId == cust_id).Where(c => c.Date >= dateFrom && c.Date <= dateTo).ToListAsync(),
                            Payments = await _db.Payments.Include("Card").Include("PayType").Where(c => c.Card.CustomerId == cust_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                            OtherCharges = await _db.CardCharges.Include("Card").Where(c => c.Card.CustomerId == cust_id).Where(c => c.Status != CardChargeStatus.Daily).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                            Balances = await _db.CardCharges.Where(c => c.Card.CustomerId == cust_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(c => new Balance()
                            {
                                Tdate = c.Tdate,
                                OutAmount = c.Amount,
                                RAmount = c.RentAmount,
                                InAmount = 0,
                                InRentAmount = 0,
                                OutAmountStatus = c.Status,
                                CardName = c.Card.AbonentNum,
                                RentAmount = (c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.PayRent) ?? 0) - (c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.RentAmount).Sum() ?? 0),
                                CurrentBalance = (c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                            })
                            .Concat(_db.Payments.Where(c => c.Card.CustomerId == cust_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(p => new Balance()
                            {
                                Tdate = p.Tdate,
                                OutAmount = 0,
                                InAmount = p.Amount,
                                InRentAmount = p.PayRent,
                                RAmount = 0,
                                OutAmountStatus = CardChargeStatus.Daily,
                                CardName = p.Card.AbonentNum,
                                RentAmount = (p.Card.Payments.Where(s => s.Tdate <= p.Tdate).Sum(s => (decimal?)s.PayRent) ?? 0) - (p.Card.CardCharges.Where(s => s.Tdate <= p.Tdate).Select(s => (decimal?)s.RentAmount).Sum() ?? 0),
                                CurrentBalance = (p.Card.Payments.Where(s => s.Tdate <= p.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (p.Card.CardCharges.Where(s => s.Tdate <= p.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                            })).OrderBy(c => c.Tdate).ToListAsync(),
                            CardServices = await _db.CardServices.Include("Card").Include("Service").Where(c => c.Card.CustomerId == cust_id).Where(c => c.Date >= dateFrom && c.Date <= dateTo).Select(c => new CardServicesList
                            {
                                CardNum = c.Card.AbonentNum,
                                Name = c.Service.Name,
                                PayType = c.PayType,
                                Price = c.Amount,
                                Date = c.Date
                            }).ToListAsync()
                        };
                    }

                    ViewBag.Services = _db.Services.Select(s => new IdName { Id = s.Id, Name = s.Name }).ToList();
                    ViewBag.CardId = card_id;

                    if (!detaled)
                        return PartialView("~/Views/Abonent/_CardInfo.cshtml", _info);
                    else
                        return PartialView("~/Views/Abonent/_CardInfoData.cshtml", _info);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        [HttpPost]
        public ActionResult ExportCardInfo()
        {
            return Json("er");
        }
        //[HttpPost]
        static public GPSLocation _gpsview = new GPSLocation();
        public ActionResult GPS(string Latitude, string Longitude, string tower_id)
        {
            GPSLocation _gps = new GPSLocation();
            using (DataContext _db = new DataContext())
            {
                var towerID = Convert.ToInt32(tower_id);
                _gps.Latitude = Convert.ToDouble(Latitude);
                _gps.Longitude = Convert.ToDouble(Longitude);
                _gps.TowerLat = _db.Towers.Where(s => s.Id == towerID).Select(s => s.towerLat).FirstOrDefault();
                _gps.TowerLon = _db.Towers.Where(s => s.Id == towerID).Select(s => s.towerLon).FirstOrDefault();
                ViewBag.Tower = _db.Database.SqlQuery<Tower>("SELECT * FROM book.Towers").ToList();
            }
            if (Latitude != null)
            {
                _gpsview = _gps;
            }
            ViewBag.GPS = _gpsview;


            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> GetCardInfoFilter(int card_id, int cust_id, string type, string[] data)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            using (DataContext _db = new DataContext())
            {
                if (type == "balance")
                {
                    List<Balance> Balances = new List<Balance>();
                    if (card_id != 0)
                    {
                        var charges = _db.CardCharges.Where(c => c.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo)
                            .Select(c => new Balance() { Tdate = c.Tdate, OutAmount = c.Amount, InAmount = 0, OutAmountStatus = c.Status, CurrentBalance = c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.Amount) ?? 0 - c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0 });
                        var payments = _db.Payments.Where(p => p.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(p => new Balance() { Tdate = p.Tdate, OutAmount = 0, InAmount = p.Amount, OutAmountStatus = CardChargeStatus.Daily });

                        switch (int.Parse(data[0]))
                        {
                            case 0:
                                Balances = await charges.Concat(payments).OrderBy(c => c.Tdate).ToListAsync();
                                break;
                            case 1:
                                Balances = await charges.OrderBy(c => c.Tdate).ToListAsync();
                                break;
                            case 2:
                                Balances = await payments.OrderBy(c => c.Tdate).ToListAsync();
                                break;
                        }
                    }
                    else
                    {
                        var charges = _db.CardCharges.Where(c => c.Card.CustomerId == cust_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo)
                            .Select(c => new Balance() { Tdate = c.Tdate, OutAmount = c.Amount, InAmount = 0, OutAmountStatus = c.Status, CurrentBalance = c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.Amount) ?? 0 - c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0 });
                        var payments = _db.Payments.Where(c => c.Card.CustomerId == cust_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(p => new Balance() { Tdate = p.Tdate, OutAmount = 0, InAmount = p.Amount, OutAmountStatus = CardChargeStatus.Daily });

                        switch (int.Parse(data[0]))
                        {
                            case 0:
                                Balances = await charges.Concat(payments).OrderBy(c => c.Tdate).ToListAsync();
                                break;
                            case 1:
                                Balances = await charges.OrderBy(c => c.Tdate).ToListAsync();
                                break;
                            case 2:
                                Balances = await payments.OrderBy(c => c.Tdate).ToListAsync();
                                break;
                        }
                    }

                    ViewBag.CardId = card_id;
                    return PartialView("~/Views/Abonent/_BalanceList.cshtml", Balances);
                }
                else if (type == "card_logs")
                {
                    List<CardLog> CardLogs = new List<CardLog>();
                    CardLogStatus[] st = data[0].Split(',').Select(c => (CardLogStatus)Enum.Parse(typeof(CardLogStatus), c)).ToArray();
                    if (card_id != 0)
                    {
                        CardLogs = await _db.CardLogs.Include("User").Where(c => c.CardId == card_id).Where(c => st.Contains(c.Status)).Where(c => c.Date >= dateFrom && c.Date <= dateTo).ToListAsync();
                    }
                    else
                    {
                        CardLogs = await _db.CardLogs.Include("User").Include("Card").Where(c => c.Card.CustomerId == cust_id).Where(c => st.Contains(c.Status)).Where(c => c.Date >= dateFrom && c.Date <= dateTo).ToListAsync();
                    }

                    ViewBag.CardId = card_id;
                    return PartialView("~/Views/Abonent/_CardLogsList.cshtml", CardLogs);
                }
                else if (type == "services")
                {
                    List<CardServicesList> CardServices = new List<CardServicesList>();
                    int[] st1 = data[0].Split(',').Select(c => int.Parse(c)).ToArray();
                    string st2 = data[1];
                    CardServicePayType _card_service_pay_type = (CardServicePayType)Enum.Parse(typeof(CardServicePayType), st2);

                    if (card_id != 0)
                    {
                        CardServices = await _db.CardServices.Include("Service").Where(c => c.CardId == card_id)
                            .Where(c => c.Date >= dateFrom && c.Date <= dateTo)
                            .Where(c => st2 == "0" ? true : c.PayType == _card_service_pay_type)
                            .Where(c => st1.Contains(c.ServiceId))
                            .Select(c => new CardServicesList
                            {
                                Name = c.Service.Name,
                                PayType = c.PayType,
                                Price = c.Amount,
                                Date = c.Date
                            }).ToListAsync();
                    }
                    else
                    {
                        CardServices = await _db.CardServices.Include("Service").Include("Card").Where(c => c.Card.CustomerId == cust_id)
                            .Where(c => c.Date >= dateFrom && c.Date <= dateTo)
                            .Where(c => st2 == "0" ? true : c.PayType == _card_service_pay_type)
                            .Where(c => st1.Contains(c.ServiceId))
                            .Select(c => new CardServicesList
                            {
                                Name = c.Service.Name,
                                PayType = c.PayType,
                                Price = c.Amount,
                                Date = c.Date
                            }).ToListAsync();
                    }
                    ViewBag.CardId = card_id;
                    return PartialView("~/Views/Abonent/_CardServicesList.cshtml", CardServices);
                }
            }

            return PartialView("~/Views/Abonent/_CardLogsList.cshtml", null);
        }

        [HttpPost]
        public async Task<JsonResult> FilterAbonents(string letter, string column, int page)
        {
            string where = column + " LIKE N'" + letter + "%'";
            if (column == "cr.status" || column == "cr.tower_id" || column == "c.type")
                where = column + "=" + letter;
            else if (column == "c.lastname+c.name")
                where = column + " LIKE N'%" + letter + "%'";


            where = where.Replace("+", "+' '+");
            string sql = @"SELECT TOP(" + pageSize + @") d.id AS Id,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.type AS Type,d.city AS City, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status, d.doc_num AS DocNum, d.pack AS ActivePacket 
                         FROM (SELECT row_number() over(ORDER BY c.id) AS row_num,c.id,c.name,c.lastname,c.code,c.[type],c.city,c.phone1, cr.doc_num, cr.abonent_num,cr.card_num, cr.status,cr.tower_id,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1
                         WHERE " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);

            System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());
            using (DataContext _db = new DataContext())
            {
                int count = await _db.Database.SqlQuery<int>("SELECT COUNT(cr.id) FROM book.Cards AS cr INNER JOIN book.Customers AS c ON c.id=cr.customer_id LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE " + where).FirstOrDefaultAsync();
                var findList = await _db.Database.SqlQuery<AbonentList>(sql).ToRawPagedListAsync(count, page, pageSize);
                return Json(new
                {
                    Abonents = findList,
                    Paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, findList, p => p.ToString(), PagedListRenderOptions.PageNumbersOnly).ToHtmlString()
                });
            }
        }

        [HttpPost]
        public JsonResult SendSMSAbonentNum(string abonent_num, string phone)
        {
            try
            {
                Task.Run(async () => { await Utils.Utils.sendMessage(phone, "abonentis nomeri-" + abonent_num); }).Wait();
            }
            catch
            {
                return Json(1);
            }

            return Json(0);
        }

        [HttpPost]
        public JsonResult GetCustomerByCode(string code)
        {
            string name = new RsServiceFuncs(true).GetAbonentName(code);
            return Json(new { name = name });
        }

        [HttpPost]
        public JsonResult ValidateKey(string key, string original_key)
        {
            if (Utils.Utils.GetMd5(key) == original_key)
                return Json(true);
            else
                return Json(false);
        }

        [HttpPost]
        public JsonResult SaveReturnedCard(string return_attachment, int card_id, string cash, float amount, string comment, int reason, int bort_id, int pretentiu, bool force = false, bool commions_not = false, string date = "")
        {
            using (DataContext _db = new DataContext())
            {

                using (DbContextTransaction tran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        DateTime dateFrom = DateTime.Now;
                        try
                        {

                            if (date != "" && date != null)
                            {
                                var id = _db.Database.SqlQuery<int>("SELECT r.ID FROM dbo.ReturnedCards r where card_id=" + card_id).FirstOrDefault();
                                ReturnedCardDelete returnedCardDelete = new ReturnedCardDelete(_db, id);
                                //DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
                                //dateFrom = Convert.ToDateTime(date, usDtfi);

                                //var card_charges = _db.CardCharges.Where(c => c.CardId == card_id).Select(s => s).ToList();
                                //var card_payments = _db.Payments.Where(c => c.CardId == card_id).Select(s => s).ToList();
                                //var card_returneds = _db.ReturnedCards.Where(c => c.card_id == card_id).Select(s => s).ToList();
                                //var ids = card_returneds.Select(s => s.Id).FirstOrDefault();
                                //var card_returneds_attachments = _db.ReturnedCardAttachments.Where(c => c.ReturnedCardsID == id).Select(s => s).ToList();

                                //_db.ReturnedCardAttachments.RemoveRange(card_returneds_attachments);
                                //_db.ReturnedCards.RemoveRange(card_returneds);
                                //_db.SaveChanges();
                                //foreach (var item_charges in card_charges)
                                //{
                                //    if (item_charges.Tdate.ToString("yyyyMMddHHmm") == dateFrom.ToString("yyyyMMddHHmm"))
                                //    {
                                //        _db.CardCharges.Remove(item_charges);
                                //        _db.SaveChanges();
                                //    }
                                //}
                                //foreach (var item_payments in card_payments)
                                //{
                                //    if (item_payments.Tdate.ToString("yyyyMMddHHmm") == dateFrom.ToString("yyyyMMddHHmm"))
                                //    {
                                //        _db.Payments.Remove(item_payments);
                                //        _db.SaveChanges();
                                //    }
                                //}

                            }
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            return Json(new { error = 0, error_message = "შეცდომა! ბრათი ვერ გაუქმდა. " + ex.Message.ToString() + ". (" + (ex.InnerException != null ? ex.InnerException.InnerException.ToString() : (" ")) + ")" });
                        }
                        if (_db.Database.SqlQuery<int>($"SELECT c.status FROM [book].[Cards] as c where c.id={card_id}").FirstOrDefault() != (int)CardStatus.Canceled)
                        {
                            int user_id = 0;
                            if (date == null)
                            {
                                user_id = bort_id;
                            }
                            else
                            {
                                user_id = Convert.ToInt32(((User)Session["CurrentUser"]).Id);
                            }

                            List<int> cards = new List<int>();
                            cards.Add(card_id);
                            PaymentData payment = new PaymentData()
                            {
                                //Amount = (decimal)commis_amount,
                                Cards = cards,
                                //PayType = 4,
                                Logging = "[]",
                                Id = 0
                            };

                            PaymentController _pay = new PaymentController();

                            double commision_ammount = 0;
                            string comm = _db.Params.Where(p => p.Name == "ReturnedCardCommision").FirstOrDefault().Value;
                            if (comm == null || comm == "")
                            {
                                return Json(new { error = 0, error_message = "საკომისიო ვერ მოიძებნა!" });
                            }
                            if (!commions_not)
                            {
                                double.TryParse(comm, out commision_ammount);
                            }


                            JArray _amount, comm_type;
                            JObject parse = JObject.Parse(cash);
                            _amount = (JArray)parse["amount"];
                            comm_type = (JArray)parse["commisionType"];

                            //float commis_amount = 0;

                            for (int i = 0; i < _amount.ToList().Count(); i++)
                            {
                                int commision_type = Convert.ToInt32(comm_type.ToList()[i]);
                                if (!_db.PayTypes.Any(p => p.Id == commision_type))
                                {
                                    tran.Rollback();
                                    return Json(new { error = 0, error_message = "გადახდის ტიპი არასწორია." });
                                }

                                payment.Amount = (decimal)(Convert.ToSingle(_amount.ToList()[i]));
                                payment.PayType = Convert.ToInt32(comm_type.ToList()[i]);
                                int res = _pay.SavePaymentReturned(_db, payment, user_id /*((User)Session["CurrentUser"]).Id*/, false);
                            }


                            var __card = _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
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
                            }).FirstOrDefault();

                            if (__card != null && __card.Card != null && __card.Card.CardStatus != CardStatus.Canceled)
                            {
                                _db.ReturnedCards.Add(new ReturnedCard
                                {
                                    Tdate = dateFrom,
                                    card_id = card_id,
                                    commission = cash,
                                    user_id = user_id,
                                    bort_id = bort_id,
                                    commission_amount = commision_ammount,
                                    returned_amount = amount,
                                    info = comment,
                                    reason = reason,
                                    pretentious = pretentiu
                                });
                                _db.BortHistorys.Add(new BortHistory
                                {
                                    Tdate = dateFrom,
                                    ReturnedCardID = card_id,
                                    BortID = bort_id,
                                    Status = 1,
                                    Info = comment

                                });
                                __card.Card.CardStatus = CardStatus.Canceled;
                                _db.Entry(__card.Card).State = EntityState.Modified;
                                _db.SaveChanges();
                                //if (!commions_not)
                                //{


                                var retcard = _db.ReturnedCards.Where(r => r.card_id == card_id).ToList();
                                int retcard_id = -1;
                                if (retcard != null && retcard.Count > 0)
                                {
                                    retcard_id = retcard.Last().Id;
                                }
                                else
                                {
                                    return Json(new { error = 0, error_message = "გაუქმება ვერ მოხერხდა. არ მოიძებნა ჩანაწერი გაუქმებულ ბარათებში!" });
                                }

                                double receiverAttachsPrice = 0;

                                JObject parsed = JObject.Parse(return_attachment);
                                JArray array = (JArray)parsed["ReturnCardID"];
                                var return_item = array.ToList();
                                if (return_item.Count != 0)
                                {
                                    for (int i = 0; i < return_item.Count(); i++)
                                    {
                                        int __id = Convert.ToInt32(return_item[i]);
                                        receiverAttachsPrice += _db.ReceiverAttachments.Where(a => a.Id == __id).Select(a => a.Price).Sum();
                                        _db.ReturnedCardAttachments.Add(new ReturnedCardAttachment
                                        {
                                            ReceiverAttachmentsID = __id,
                                            ReturnedCardsID = retcard_id
                                        });
                                    }
                                    _db.SaveChanges();
                                }

                                //int CardTo = 0;
                                decimal balance = Math.Round(Utils.Utils.GetBalance(__card.PaymentAmount, __card.ChargeAmount), 2);

                                decimal returned_amount = decimal.Parse(amount.ToString());

                                if (commision_ammount > 0)
                                {
                                    if (balance >= (decimal)commision_ammount)
                                    {
                                        _db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = __card.Card.Id, Status = CardChargeStatus.ReturnComm, Amount = (decimal)commision_ammount });
                                    }
                                    else
                                    {
                                        if (force)
                                        {
                                            _db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = __card.Card.Id, Status = CardChargeStatus.ReturnComm, Amount = (decimal)commision_ammount });
                                        }
                                        else
                                        {
                                            tran.Rollback();
                                            return Json(new { error = 0, error_message = "ბალანსზე არ რჩება საკმრისი თანხა საკომისიოს გადასახდელად. შევსებული ბალანსი: " + balance + ", საკომისიო: " + commision_ammount });
                                        }
                                    }
                                    _db.SaveChanges();
                                }

                                if (receiverAttachsPrice > 0)
                                {
                                    decimal paysum = _db.Payments.Where(p => p.CardId == __card.Card.Id).ToList().Count > 0 ? _db.Payments.Where(p => p.CardId == __card.Card.Id).Select(p => p.Amount).Sum() : 0;
                                    decimal chargesum = _db.CardCharges.Where(c => c.CardId == __card.Card.Id).ToList().Count > 0 ? _db.CardCharges.Where(c => c.CardId == __card.Card.Id).Select(c => c.Amount).Sum() : 0;
                                    balance = Utils.Utils.GetBalance(paysum, chargesum);

                                    if (balance >= (decimal)receiverAttachsPrice)
                                    {
                                        _db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = __card.Card.Id, Status = CardChargeStatus.AccessoryCharge, Amount = (decimal)receiverAttachsPrice });
                                    }
                                    else
                                    {
                                        if (force)
                                        {
                                            _db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = __card.Card.Id, Status = CardChargeStatus.AccessoryCharge, Amount = (decimal)receiverAttachsPrice });
                                        }
                                        else
                                        {
                                            tran.Rollback();
                                            return Json(new { error = 0, error_message = "ბალანსზე არ რჩება საკმრისი თანხა აქსესუარების ჯარიმის გადასახდელად. შევსებული ბალანსი: " + balance + ", აქსესუარების ღირებულება: " + receiverAttachsPrice });
                                        }
                                    }
                                    _db.SaveChanges();
                                }

                                if (returned_amount > 0)
                                {
                                    balance = Utils.Utils.GetBalance(_db.Payments.Where(p => p.CardId == __card.Card.Id).Select(p => p.Amount).Sum(), _db.CardCharges.Where(c => c.CardId == __card.Card.Id).Select(c => c.Amount).Sum());

                                    if (balance >= returned_amount)
                                    {
                                        returned_amount = Math.Abs(returned_amount) * -1;
                                        //_db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = __card.Card.Id, Status = CardChargeStatus.ReturnMoney, Amount = returned_amount });
                                        //PaymentData payment = new PaymentData()
                                        //{
                                        //    Amount = returned_amount,
                                        //    Cards = cards,
                                        //    PayType = 4,
                                        //    Logging = "[]",
                                        //    Id = 0
                                        //};
                                        payment.Amount = returned_amount;
                                        payment.PayType = 4;
                                        _pay.SavePaymentReturned(_db, payment, user_id, false);
                                    }
                                    else
                                    {
                                        tran.Rollback();
                                        return Json(new { error = 0, error_message = "ბალანსზე აღარ დარჩა საკმრისი დასაბრუნებელი თანხა. ბალანსი: " + balance + ", დაასაბრუნებელი თანხა: " + returned_amount });
                                    }
                                }
                                //}
                                CardLog _log = new CardLog() { CardId = __card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.Cancel, UserId = user_id };
                                _db.CardLogs.Add(_log);

                                this.AddLoging(_db,
                                    LogType.Card,
                                    LogMode.CardDeal,
                                    user_id,
                                    __card.Card.Id,
                                    __card.Card.CardNum + " ის გაუქმება",
                                    new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათის გაუქმება", old_val = "" } }
                                 );

                                _db.SaveChanges();


                                string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                                var cas_ids = __card.CasIds;// __card.Card.Subscribtions.First(c => c.Status).SubscriptionPackages.Select(s => (short)s.Package.CasId).ToArray();
                                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                _socket.Connect();
                                if (!_socket.SendEntitlementRequest(Convert.ToInt32(__card.Card.CardNum), cas_ids.ToArray(), __card.Card.FinishDate.AddHours(-4), __card.Card.FinishDate.AddHours(-4), false))
                                {
                                    throw new Exception();
                                }
                                _socket.Disconnect();
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(new { error = 0, error_message = "შეცდომა! ბრათი ვერ გაუქმდა. " + ex.Message.ToString() + ". (" + (ex.InnerException != null ? ex.InnerException.InnerException.ToString() : (" ")) + ")" });
                    }
                }
            }
            return Json(new { error = 1, error_message = "ბარათი გაუქმდა" });
        }

        public JsonResult CardCredit(int id)
        {
            using (DataContext _db = new DataContext())
            {
                Card _card = _db.Cards.Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.Id == id).FirstOrDefault();
                if (_card != null)
                {
                    List<Param> _params = _db.Params.ToList();

                    if ((DateTime.Now - _card.CloseDate).Days > int.Parse(_params.Where(c => c.Name == "CreditValidDays").Select(c => c.Value).First()))
                    {
                        return Json("ბარათს არ ეკუთვნის კრედიტი!");
                    }

                    using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                    {
                        try
                        {
                            string[] charge_val = _params.Find(c => c.Name == "CardCharge").Value.Split(':');
                            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_val[0]), int.Parse(charge_val[1]), 0);

                            int user_id = ((User)Session["CurrentUser"]).Id;
                            _card.CardStatus = CardStatus.Active;
                            _card.Mode = 1;
                            _card.CasDate = DateTime.Now;
                            _card.FinishDate = now.AddDays(int.Parse(_params.First(c => c.Name == "CreditDays").Value));
                            _db.Entry(_card).State = EntityState.Modified;
                            _db.SaveChanges();

                            CardLog _log = new CardLog() { CardId = _card.Id, Date = DateTime.Now, Status = CardLogStatus.Credit, UserId = user_id };
                            _db.CardLogs.Add(_log);

                            //original code
                            //decimal amount = (decimal)(_card.Subscribtions.FirstOrDefault(s=>s.Status).Amount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                            decimal amount = (decimal)(_card.Subscribtions.FirstOrDefault(s => s.Status).Amount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) / Utils.Utils.divide_card_charge_interval);

                            amount -= (amount * (decimal)_card.Discount / 100);
                            _db.CardCharges.Add(new CardCharge() { CardId = _card.Id, Amount = amount, Tdate = DateTime.Now, Status = CardChargeStatus.PreChange });

                            this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Id,
                                 _card.AbonentNum + " - გაიცა კრედიტი",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "გაიცა კრედიტი", old_val = "" } }
                              );
                            _db.SaveChanges();

                            string[] address = _params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.CardNum), _card.Subscribtions.First(c => c.Status).SubscriptionPackages.Select(s => (short)s.Package.CasId).ToArray(), DateTime.SpecifyKind(_card.CasDate, DateTimeKind.Utc), true))
                            {
                                throw new Exception();
                            }

                            _socket.Disconnect();

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            return Json("კრედიტის აღება ვერ მოხერხდა!");
                        }
                    }
                }
            }

            return Json("");
        }

        [HttpPost]
        public JsonResult CardPause(int id, int? day, bool privilegies_pause)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        var _card = _db.Cards.Include("Subscribtions").Where(c => c.Id == id).Select(c => new CardDetailData
                        {
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                            Card = c,
                            Subscribtion = c.Subscribtions.Where(s => s.CardId == c.Id && s.Status == true).FirstOrDefault(),
                        }).FirstOrDefault();

                        if (_card != null)
                        {
                            //if (_card.Card.CardStatus != CardStatus.Active)
                            //{
                            //    return Json("დაპაუზება ვერ მოხერხდა! ბარათის სტატუსი არარის აქტიური.");
                            //}

                            var dateSpan = DateTimeSpan.CompareDates(DateTime.Now, _card.Card.Tdate);
                            dateSpan = DateTimeSpan.CompareDates(DateTime.Now, _card.Card.Tdate.AddYears(dateSpan.Years + 1));
                            bool lastDays = false;
                            if (dateSpan.Years == 0 && dateSpan.Months == 0)
                            {
                                if (dateSpan.Days < 30)
                                {
                                    lastDays = true;
                                }
                            }

                            //decimal pause_amount = decimal.Parse(_db.Params.First(p => p.Name == "CardPauseAmount").Value);
                            decimal pause_amount = 0;
                            if (day == null || day < 0 || day > 3)
                            {
                                return Json("დაპაუზება ვერ მოხერხდა!");
                            }
                            else if (day == 0)
                            {
                                pause_amount = 0;
                                _card.Card.LastPauseType = PauseType.OneMonthFree;
                                _card.Card.PauseFreeMonthUsed = true;
                                _card.Card.PauseDays = lastDays ? dateSpan.Days : 30;
                            }
                            else
                            {
                                pause_amount = (int)day * 3;
                                //_card.Card.LastPauseType = PauseType.UnUsed;
                                _card.Card.NonFreePausedCountPerMonth += (short)day;
                                _card.Card.PauseDays = 30 * (int)day;
                            }
                            //int _day = (int)Enum.Parse(typeof(PauseType), day);
                            //switch (_day)
                            //{
                            //    case (int)PauseType.OneMonthFree:
                            //        pause_amount = 0;
                            //        _card.Card.LastPauseType = PauseType.OneMonthFree;
                            //        _card.Card.PauseFreeMonthUsed = true;
                            //        _card.Card.PauseDays = lastDays? dateSpan.Days : 30;
                            //        break;
                            //    case (int)PauseType.OneMonth:
                            //        pause_amount = 3;
                            //        _card.Card.LastPauseType = PauseType.OneMonth;
                            //        _card.Card.PauseDays = 30;
                            //        break;
                            //    case (int)PauseType.ThreeMonth:
                            //        pause_amount = 9;
                            //        _card.Card.LastPauseType = PauseType.ThreeMonth;
                            //        _card.Card.PauseDays = 90;
                            //        break;
                            //    default:
                            //        break;
                            //}



                            decimal ammount = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                            //if (_card.Card.CardStatus == CardStatus.Active)
                            {
                                if (Utils.Utils.GetPermission("CARD_PAUSED_CASE_DEBT") && privilegies_pause == true)
                                {

                                }
                                else
                                {
                                    if (ammount - pause_amount < 0)
                                        return Json("დაპაუზება ვერ მოხერხდა! ბალანსზე არასაკმარისი თანხაა.");

                                }
                            }

                            //decimal jurid_limit_months = decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value);
                            //Utils.Utils.SetFinishDate(_db, _card, jurid_limit_months, day);

                            bool has_bought = false;
                            List<SubscriptionPackage> packages = _db.SubscriptionPackages.Where(s => s.SubscriptionId == _card.Subscribtion.Id).ToList();
                            if (packages.Where(p => p.PackageId == 304070).ToList().Count > 0)
                                has_bought = true;

                            string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            //if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), false, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            //{
                            //    throw new Exception();
                            //}
                            if (has_bought)
                            {
                                if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), new short[1] { 9 }, _card.Card.CasDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                {
                                    return Json(0);
                                    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                }
                                if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), new short[1] { 2 }, _card.Card.FinishDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), false))
                                {
                                    return Json(0);
                                    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                }
                            }
                            _socket.Disconnect();

                            _card.Card.CardStatus = CardStatus.Paused;
                            //_card.Card.PauseDays = day;
                            _card.Card.PauseDate = DateTime.Now;
                            _db.Entry(_card.Card).State = EntityState.Modified;

                            if (pause_amount != 0)
                            {
                                CardCharge _charge = new CardCharge()
                                {
                                    CardId = id,
                                    Tdate = DateTime.Now,
                                    Amount = pause_amount,
                                    Status = CardChargeStatus.Pause
                                };
                                _db.CardCharges.Add(_charge);
                            }

                            ammount -= pause_amount;

                            MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "OnPause");
                            string message_desc = String.Format(message.Desc, pause_amount, Math.Round((ammount), 2));
                            string phone = _db.Customers.Where(c => c.Id == _card.Card.CustomerId).First().Phone1;
                            //string message = "Tqveni TVMobile angarishidan chamogechrat pauzis sapasuri: " + pause_amount.ToString() + " - lari, tqveni mimdinare balansia: " + ammount.ToString();
                            Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();

                            CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.Pause, UserId = user_id };
                            _db.CardLogs.Add(_log);

                            this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Card.Id,
                                 _card.Card.AbonentNum + " - ბარათი დაპაუზდა " + "(" + day + ")",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაპაუზდა", old_val = "" } }
                              );
                        }
                        _db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json("დაპაუზება ვერ მოხერხდა!");
                    }
                }
            }
            return Json("");
        }

        [HttpPost]
        public JsonResult ResetCardPause(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        var _card = _db.Cards.Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.Id == card_id).Select(c => new CardDetailData
                        {
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            Card = c,
                            MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                            CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                            SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                            Subscribtion = c.Subscribtions.Where(s => s.CardId == c.Id && s.Status == true).FirstOrDefault(),
                        }).FirstOrDefault();
                        if (_card != null)
                        {
                            var _params = _db.Params.ToList();
                            string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                            decimal amount = (decimal)_card.SubscribAmount;
                            int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                            decimal dayly_amount = amount / service_days;

                            //original code
                            //bool status_sign = (_card.PaymentAmount - _card.ChargeAmount) >= (decimal)_card.MinPrice;
                            decimal balance = (_card.PaymentAmount - _card.ChargeAmount);
                            bool status_sign = balance >= dayly_amount;

                            _card.Card.PauseDays = 0;

                            string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();

                            if (status_sign)
                            {
                                bool has_bought = false;
                                List<SubscriptionPackage> packages = _db.SubscriptionPackages.Where(s => s.SubscriptionId == _card.Subscribtion.Id).ToList();
                                if (packages.Where(p => p.PackageId == 304070).ToList().Count > 0)
                                    has_bought = true;

                                DateTime fin_date = _card.Card.FinishDate;
                                Utils.Utils.SetFinishDate(_db, decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value), card_id);
                                //Utils.Utils.SetFinishDate(_db, _card, decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value));
                                _card.Card.CardStatus = CardStatus.Active;

                                if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                {
                                    throw new Exception();
                                }
                                if (!has_bought)
                                {
                                    if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), new short[1] { 9 }, _card.Card.CasDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                    {
                                        return Json(0);
                                        //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                    }
                                }

                                MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "ResetPauseAndActivate");
                                string message_desc = String.Format(message.Desc, Math.Round((balance), 2));
                                string phone = _db.Customers.Where(c => c.Id == _card.Card.CustomerId).First().Phone1;
                                Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();

                            }
                            else
                            {
                                if (_card.Card.FinishDate == today && _card.CustomerType != CustomerType.Technic)
                                {

                                    //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.FinishDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                    //{
                                    //    throw new Exception();
                                    //    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                    //}
                                }
                                _card.Card.CardStatus = CardStatus.Closed;

                                string phone = _db.Customers.Where(c => c.Id == _card.Card.CustomerId).First().Phone1;
                                MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "ResetPauseAndActivate");
                                string message_desc = String.Format(message.Desc, Math.Round((balance), 2));
                                Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                            }

                            //if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            //{
                            //    throw new Exception();
                            //}

                            _socket.Disconnect();

                            //_card.Card.CardStatus = status_sign ? CardStatus.Active : CardStatus.Closed;
                            _db.Entry(_card.Card).State = EntityState.Modified;

                            CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.ClosePause, UserId = user_id };
                            _db.CardLogs.Add(_log);

                            this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Card.Id,
                                 _card.Card.AbonentNum + " - ბარათის პაუზის მოხსნა",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათის პაუზის მოხსნა", old_val = "" } }
                              );

                            #region original code
                            //Utils.Utils.SetFinishDate(_db, _card, decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value));

                            //string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            //_socket.Connect();

                            //if (status_sign)
                            //    if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            //    {
                            //        throw new Exception();
                            //    }


                            ////original code
                            //if (status_sign)
                            //{
                            //    //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false))
                            //    //{
                            //    //    throw new Exception();
                            //    //}

                            //    //Thread.Sleep(2000);
                            //    //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                            //    //{
                            //    //    throw new Exception();
                            //    //}
                            //}

                            //_socket.Disconnect();
                            #endregion
                        }
                    }
                    catch
                    {
                        tran.Rollback();
                        return Json(false);
                    }

                    tran.Commit();
                }
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult CardBlock(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        CardDetailData _card = _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
                        {
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            Card = c,
                            MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                            CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                            SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                        }).FirstOrDefault();
                        if (_card != null)
                        {
                            CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.Blocked, UserId = user_id, CardStatus = _card.Card.CardStatus };
                            _db.CardLogs.Add(_log);

                            _card.Card.CardStatus = CardStatus.Blocked;
                            _db.Entry(_card.Card).State = EntityState.Modified;

                            this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Card.Id,
                                 _card.Card.AbonentNum + " - ბარათი დაიბლოკა",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაიბლოკა", old_val = "" } }
                              );

                            string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), false, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            {
                                throw new Exception();
                            }
                            _socket.Disconnect();

                            decimal balance = (_card.PaymentAmount - _card.ChargeAmount);
                            string phone = _db.Customers.Where(c => c.Id == _card.Card.CustomerId).First().Phone1;
                            MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "OnBlock");
                            string message_desc = String.Format(message.Desc, Math.Round((balance), 2));
                            Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json("დაპაუზება ვერ მოხერხდა!");
                    }
                }
            }
            return Json("");
        }

        [HttpPost]
        public JsonResult ResetCardBlock(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        //Card _card = _db.Cards.Where(c => c.Id == card_id).FirstOrDefault();
                        CardDetailData _card = _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
                        {
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            Card = c,
                            MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                            CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                            SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                        }).FirstOrDefault();

                        if (_card != null)
                        {
                            CardLog last_log = _db.CardLogs.Where(c => c.CardId == _card.Card.Id).OrderByDescending(c => c.Date).Skip(0).Take(1).FirstOrDefault();
                            if (last_log != null)
                            {
                                _card.Card.CardStatus = last_log.CardStatus;
                                _db.Entry(_card.Card).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                            if (_card.Card.CardStatus != CardStatus.FreeDays)
                            {
                                Utils.Utils.SetFinishDate(_db, decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value), _card.Card.Id);
                            }

                            CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.CloseBlock, UserId = user_id };
                            _db.CardLogs.Add(_log);

                            this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Card.Id,
                                 _card.Card.AbonentNum + " - ბარათის ბლოკის მოხსნა",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათის ბლოკის მოხსნა", old_val = "" } }
                              );

                            string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            {
                                throw new Exception();
                            }
                            _socket.Disconnect();

                            decimal balance = (_card.PaymentAmount - _card.ChargeAmount);
                            string phone = _db.Customers.Where(c => c.Id == _card.Card.CustomerId).First().Phone1;
                            MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "OnResetBlock");
                            string message_desc = String.Format(message.Desc, Math.Round((balance), 2));
                            Task.Run(async () => { await Utils.Utils.sendMessage(phone, message_desc); }).Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(false);
                    }

                    tran.Commit();
                }
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult AbonentCheckCode(string code)
        {
            using (DataContext _db = new DataContext())
            {
                if (_db.Customers.Any(c => c.Code == code))
                    return Json("აბონენტი უკვე არსებობს");
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult CardCheckCode(string code, string type, int id)
        {
            using (DataContext _db = new DataContext())
            {
                if (_db.Cards.Where(c => type == "CardNum" ? c.CardStatus != CardStatus.Canceled : true).Any(c => type == "CardNum" ? c.CardNum == code : c.AbonentNum == code))
                    return Json((type == "CardNum" ? "ბარათის" : "აბონენტის") + " № უკვე არსებობს");
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult GetTowers()
        {
            using (DataContext _db = new DataContext())
            {
                return Json(_db.Towers.Select(t => new { id = t.Id, name = t.Name }).ToList());
            }
        }

        [HttpPost]
        public PartialViewResult GetRestoreMoney(int abonent_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<Card> _cards = _db.Cards.Where(c => c.CustomerId == abonent_id).ToList();

                ViewBag.FromCards = new List<IdName>() { new IdName { Id = 0, Name = "" } }.Union(_cards.Select(c => new IdName { Id = c.Id, Name = c.AbonentNum + " / " + c.CardNum }).ToList()).ToList();
                ViewBag.ToCards = new List<IdName>() { new IdName { Id = 0, Name = "გატანა" } }.Union(_cards.Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new IdName { Id = c.Id, Name = c.AbonentNum + " / " + c.CardNum }).ToList()).ToList();
                return PartialView("~/Views/Abonent/_RestoreMoneyModal.cshtml");
            }
        }

        [HttpPost]
        public JsonResult RestoreMoney(decimal Balance, int CardFrom, int CardTo, CardStatus status)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;

                        PaymentController paymentController = new PaymentController();
                        PaymentData paymentData = new PaymentData()
                        {
                            Amount = -Balance,
                            PayType = (CardTo == 0 ? 4 : 1),
                            Cards = new List<int>()
                            {
                                CardFrom
                            }
                        };

                        if (paymentController.SavePayment(paymentData, user_id, false, null) == 1)
                        {
                            if (CardTo != 0)
                            {
                                PaymentData paymentDatum1 = new PaymentData()
                                {
                                    Amount = Balance,
                                    PayType = 1,
                                    Cards = new List<int>()
                                    {
                                        CardTo
                                    }
                                };
                                if (paymentController.SavePayment(paymentDatum1, user_id, false, null) != 1)
                                {
                                    tran.Rollback();
                                    return Json(false);
                                }

                                string str = string.Concat("ბარათიდან - ", _db.Cards.First<Card>((Card c) => c.Id == CardFrom).CardNum, " ", (CardTo == 0 ? "გატანა" : string.Concat("ბარათზე - ", _db.Cards.First<Card>((Card c) => c.Id == CardTo).CardNum)));
                                long cardFrom = (long)CardFrom;
                                List<LoggingData> loggingDatas = new List<LoggingData>();
                                LoggingData loggingDatum = new LoggingData()
                                {
                                    field = "გადანაწილება",
                                    new_val = Balance.ToString(),
                                    old_val = "",
                                    type = ""
                                };
                                loggingDatas.Add(loggingDatum);
                                base.AddLoging(_db, LogType.Card, LogMode.CardDeal, user_id, cardFrom, str, loggingDatas);
                                _db.SaveChanges();
                                tran.Commit();
                                return base.Json(true);
                            }
                            else
                            {
                                tran.Rollback();
                                return base.Json(true);
                            }
                        }

                        tran.Commit();
                        return Json(true);
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(false);
                    }
                }
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> GetServicesList()
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.PayTypes = (from CardServicePayType n in Enum.GetValues(typeof(CardServicePayType))
                                    select new SelectListItem { Value = n.ToString(), Text = Utils.Utils.GetEnumDescription(n) }).ToList();
                return PartialView("~/Views/Abonent/_ServicesList.cshtml", await _db.Services.ToListAsync());
            }
        }

        [HttpPost]
        public JsonResult CardAbonentNumGenerate(int index)
        {
            using (DataContext _db = new DataContext())
            {
                string max_num = _db.Cards.Select(c => c.AbonentNum).OrderByDescending(c => c).FirstOrDefault();
                return Json(Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1)) + index));
            }
        }

        [HttpPost]
        public JsonResult AddChat(string text, int customer_id)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    DateTime dt = DateTime.Now;
                    CustomerChat _chat = new CustomerChat
                    {
                        Tdate = dt,
                        MessageText = text,
                        CustomerId = customer_id,
                        UserId = ((User)Session["CurrentUser"]).Id,
                    };
                    _db.CustomersChat.Add(_chat);
                    _db.SaveChanges();
                    return Json(new { res = 1, UserName = ((User)Session["CurrentUser"]).Name, Tdate = dt.ToString("dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture), Message = text });
                }
                catch
                {
                    return Json(new { res = 0 });
                }
            }
        }

        [HttpPost]
        public JsonResult RemoveChat(int id)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    CustomerChat _chat = _db.CustomersChat.Where(c => c.Id == id).FirstOrDefault();
                    if (_chat != null)
                    {
                        _db.CustomersChat.Remove(_chat);
                        _db.Entry(_chat).State = EntityState.Deleted;
                        _db.SaveChanges();
                    }

                    return Json(1);
                }
                catch
                {
                    return Json(1);
                }
            }
        }

        [HttpPost]
        public JsonResult SendOSD(int card_num)
        {
            using (DataContext _db = new DataContext())
            {
                List<Param> Params = _db.Params.ToList();
                Card card = _db.Cards.Where(c => c.CardNum == card_num.ToString()).First();
                int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;


                AutoMessageTemplate message_geo = _db.AutoMessageTemplates.Where(m => m.Name == "ReportDisabling_GEO").FirstOrDefault();
                MessageTemplate msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                string onPayMsg_geo = "";
                if (msg_geo != null)
                    onPayMsg_geo = String.Format(msg_geo.Desc, card.AbonentNum, Math.Round((decimal)5, 2), card.FinishDate.ToString("dd/MM/yyyy"));
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();
                if (!_socket.SendOSDRequest(card_num, "შემოწმება...", DateTime.SpecifyKind(DateTime.Now.AddMinutes(1).AddSeconds(60), DateTimeKind.Utc), osd_duration))
                {
                    return Json(0);
                }
                _socket.Disconnect();
            }

            return Json(1);
        }

        [HttpPost]
        public JsonResult SendReset(int card_num)
        {
            using (DataContext _db = new DataContext())
            {
                List<Param> Params = _db.Params.ToList();

                int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();
                if (!_socket.SendOSDRequest(card_num, "D_RESET", DateTime.SpecifyKind(DateTime.Now.AddMinutes(1).AddSeconds(60), DateTimeKind.Utc), osd_duration))
                {
                    return Json(0);
                }
                _socket.Disconnect();
            }

            return Json(1);
        }

        [HttpPost]
        public JsonResult SendShowInfo(int card_num)
        {
            using (DataContext _db = new DataContext())
            {
                List<Param> Params = _db.Params.ToList();

                int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();
                if (!_socket.SendOSDRequest(card_num, "D_INFO", DateTime.SpecifyKind(DateTime.Now.AddMinutes(1).AddSeconds(60), DateTimeKind.Utc), osd_duration))
                {
                    return Json(0);
                }
                _socket.Disconnect();
            }

            return Json(1);
        }

        [HttpPost]
        public JsonResult SendPinReset(int card_num, string default_pin, string new_pin)
        {
            if (card_num == 0 || default_pin == null || new_pin == null)
                return Json(0);
            string osdsetr = "P_PINCHANGE{" + default_pin + "}{" + new_pin + "}";//String.Format("P_PINCHANGE{{0}}{{1}}", default_pin, new_pin);
            using (DataContext _db = new DataContext())
            {
                List<Param> Params = _db.Params.ToList();

                int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();
                if (!_socket.SendOSDRequest(card_num, osdsetr, DateTime.SpecifyKind(DateTime.Now.AddMinutes(1).AddSeconds(60), DateTimeKind.Utc), osd_duration))
                {
                    return Json(0);
                }
                _socket.Disconnect();
            }

            return Json(1);
        }

        [HttpPost]
        public JsonResult SendPinSetDefault(int card_num)
        {
            if (card_num == 0)
                return Json(0);

            using (DataContext _db = new DataContext())
            {
                List<Param> Params = _db.Params.ToList();

                int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();
                if (!_socket.SendPinResetRequest(card_num, DateTime.SpecifyKind(DateTime.Now.AddMinutes(1).AddSeconds(60), DateTimeKind.Utc)))
                {
                    return Json(0);
                }
                _socket.Disconnect();
            }

            return Json(1);
        }

        [HttpPost]
        public async Task<JsonResult> RefreshEntitlement(int card_num, int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                SendMiniSMS sendMiniSMS = new SendMiniSMS();
                List<Param> _params = _db.Params.ToList();
                decimal jurid_limit_months = int.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);
                string[] address = _db.Params.Where(p => p.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                var card = _db.Cards.Where(c => c.Id == card_id).Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
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
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();

                if (card != null)
                {
                    decimal balance = Utils.Utils.GetBalance(card.PaymentAmount, card.ChargeAmount);
                    decimal amount = (decimal)card.SubscribAmount;
                    //card.Card.CasDate = DateTime.Now;
                    //_db.Entry(card.Card).State = EntityState.Modified;
                    //_db.SaveChanges();
                    if (card.CustomerType != CustomerType.Juridical)
                    {
                        if (balance >= amount)
                        {
                            if (card.Card.CardStatus == CardStatus.Closed)
                                card.Card.CardStatus = CardStatus.Active;

                            _db.Entry(card.Card).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                        Utils.Utils.SetFinishDate(_db, jurid_limit_months, card.Card.Id);

                        if (card.CustomerType == CustomerType.Technic)
                        {
                            card.Card.FinishDate = DateTime.Now.AddYears(20);

                            _db.Entry(card.Card).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                        DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
                        DateTime dfrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                        DateTime dTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
                        DateTime date_closse = DateTime.Now;
                        CardInfo _info; CardDetailData _card;
                        _card = _db.Cards.Where(c => c.Id == card_id).Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Select(c => new CardDetailData
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
                        decimal _balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                        decimal CardCahrge_Balance = 0;
                        var error = _db.CardCharges.Where(c => c.CardId == card_id && c.Tdate >= dfrom && c.Tdate <= dTo).Select(s => s.Amount).ToList();
                        if (error.Count() != 0)
                        {
                            CardCahrge_Balance = _db.CardCharges.Where(c => c.CardId == card_id && c.Tdate >= dfrom && c.Tdate <= dTo).Select(s => s.Amount).Sum();

                        }

                        //_info = new CardInfo()
                        //{
                        //    Subscribtions = await _db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                        //    CardLogs = await _db.CardLogs.Include("User").Where(c => c.CardId == card_id).Where(c => c.Date >= dateFrom && c.Date <= dateTo).ToListAsync(),
                        //    Payments = await _db.Payments.Include("PayType").Where(c => c.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                        //    OtherCharges = await _db.CardCharges.Where(c => c.CardId == card_id).Where(c => c.Status != CardChargeStatus.Daily).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                        //    Balances = await _db.CardCharges.Where(c => c.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(c => new Balance()
                        //    {
                        //        Tdate = c.Tdate,
                        //        OutAmount = c.Amount,
                        //        InAmount = 0,
                        //        OutAmountStatus = c.Status,
                        //        CurrentBalance = (c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                        //    })
                        //   .Concat(_db.Payments.Where(p => p.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(p => new Balance()
                        //   {
                        //       Tdate = p.Tdate,
                        //       OutAmount = 0,
                        //       InAmount = p.Amount,
                        //       OutAmountStatus = CardChargeStatus.Daily,
                        //       CurrentBalance = (p.Card.Payments.Where(s => s.Tdate <= p.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (p.Card.CardCharges.Where(s => s.Tdate <= p.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                        //   })).OrderBy(c => c.Tdate).ToListAsync(),
                        //    CardServices = await _db.CardServices.Include("Service").Where(c => c.CardId == card_id).Where(c => c.Date >= dateFrom && c.Date <= dateTo).Select(c => new CardServicesList
                        //    {
                        //        Name = c.Service.Name,
                        //        PayType = c.PayType,
                        //        Price = c.Amount,
                        //        Date = c.Date
                        //    }).ToListAsync()
                        //};
                        //var CardCahrge_Balance = _db.CardCharges.Where(c => c.CardId == card_id && c.Tdate >= dfrom && c.Tdate <= dTo).Select(s => s.Amount).Sum();
                        var _blance = _balance + CardCahrge_Balance;
                        if (Math.Round(_blance, 2) >= 0 || (_blance < 0 && _blance > (decimal)-0.2))
                        {
                            //card.Card.CardStatus = CardStatus.Active;

                            //_db.Entry(card.Card).State = System.Data.Entity.EntityState.Modified;
                            //_db.SaveChanges();
                            Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, card.Card.Id);
                        }
                        else
                        {
                            if (date_closse.Day >= 10)
                            {
                                card.Card.CardStatus = CardStatus.Closed;
                                _db.Entry(card.Card).State = System.Data.Entity.EntityState.Modified;
                                _db.SaveChanges();
                            }
                            if (!_socket.SendEntitlementRequest(Convert.ToInt32(card.Card.CardNum), card.CasIds.ToArray(), card.Card.FinishDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), true))
                            //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                            {
                                return Json(0);
                                //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                            }
                            //miniSMS
                            sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(card.Card.CardNum), card.Card.Id, card.CasIds.ToArray(), card.Card.FinishDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), -2, true, (int)StatusMiniSMS.EntitleRefresh);
                        }
                        if (_db.Cards.Where(c => c.Id == card.Card.Id).FirstOrDefault().CardStatus == CardStatus.Active)
                        {
                            //CardLog _log = new CardLog() { CardId = card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.Open, UserId = ((User)Session["CurrentUser"]).Id };
                            //_db.CardLogs.Add(_log);
                            //_db.SaveChanges();
                        }
                    }


                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, card.Card.Id);
                    if (card.Card.CardStatus == CardStatus.Active)
                        if (!_socket.SendEntitlementRequest(Convert.ToInt32(card.Card.CardNum), card.CasIds.ToArray(), card.Card.CasDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), true))
                        //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                        {
                            return Json(0);
                            //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                        }
                    //miniSMS
                    sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(card.Card.CardNum), card.Card.Id, card.CasIds.ToArray(), card.Card.CasDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), (int)CardStatus.Active, true, (int)StatusMiniSMS.EntitleRefresh);

                    List<Package> packages = _db.Packages.Where(p => card.CasIds.Contains((short)p.CasId)).ToList();
                    if (card.SubscribAmount == 12)
                    {
                        if (!_socket.SendEntitlementRequest(Convert.ToInt32(card.Card.CardNum), new short[] { 9 }, card.Card.CloseDate.AddHours(-4), new DateTime(2038, 1, 1, 0, 0, 0, 0), true))
                        {
                            //throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                            return Json(1);
                        }
                        //miniSMS
                        sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(card.Card.CardNum), card.Card.Id, new short[] { 9 }, card.Card.CloseDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), (int)CardStatus.Active, true, (int)StatusMiniSMS.EntitleRefresh);
                    }
                    if (card.Card.CardStatus == CardStatus.Rent)
                    {
                        if (!_socket.SendEntitlementRequest(Convert.ToInt32(card.Card.CardNum), new short[] { 9 }, card.Card.CloseDate.AddHours(-4), card.Card.RentFinishDate.AddHours(-4), true))
                        {
                            //throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                            return Json(1);
                        }
                        //miniSMS
                        sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(card.Card.CardNum), card.Card.Id, card.CasIds.ToArray(), card.Card.CloseDate.AddHours(-4), card.Card.RentFinishDate.AddHours(-4), (int)CardStatus.Rent, true, (int)StatusMiniSMS.EntitleRefresh);
                    }
                    #region original code
                    //if (!_socket.SendEntitlementRequestTemp(card_num, new short[] { 3, 4 }, new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), false))
                    //{
                    //    return Json(0);
                    //}

                    //SendTempCas(_db, _socket, card.Card.CardNum);

                    //if (!_socket.SendEntitlementRequest(card_num, card.CasIds.ToArray(), DateTime.SpecifyKind(card.Card.CasDate, DateTimeKind.Utc), false))
                    //{
                    //    c
                    //}

                    //if (card.Card.CardStatus != CardStatus.Closed)
                    //{
                    //    Thread.Sleep(2000);
                    //    if (!_socket.SendEntitlementRequest(card_num, card.CasIds.ToArray(), DateTime.SpecifyKind(card.Card.CasDate, DateTimeKind.Utc), true))
                    //    {
                    //        return Json(0);
                    //    }
                    //}
                    #endregion

                    _socket.Disconnect();
                    return Json(1);
                }
            }
            return Json(0);
        }

        [HttpPost]
        public JsonResult cancelEntitlement(int card_num, int card_id)
        {
            SendMiniSMS sendMiniSMS = new SendMiniSMS();
            using (DataContext _db = new DataContext())
            {
                List<Param> _params = _db.Params.ToList();
                decimal jurid_limit_months = int.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);
                string[] address = _db.Params.Where(p => p.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                var card = _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
                {
                    Card = c,
                    CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId)
                }).FirstOrDefault();

                if (card != null)
                {
                    //card.Card.CasDate = DateTime.Now;
                    //_db.Entry(card.Card).State = EntityState.Modified;
                    //_db.SaveChanges();

                    CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                    _socket.Connect();

                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, card.Card.Id);
                    if (card.Card.CardStatus == CardStatus.Rent)
                    {
                        if (!_socket.SendEntitlementRequest(Convert.ToInt32(card.Card.CardNum), card.CasIds.ToArray(), card.Card.RentFinishDate.AddHours(-4), card.Card.RentFinishDate.AddHours(-4), false))
                        {
                            //throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                            return Json(1);
                        }
                        //miniSMS
                        sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(card.Card.CardNum), card.Card.Id, card.CasIds.ToArray(), card.Card.RentFinishDate.AddHours(-4), card.Card.RentFinishDate.AddHours(-4), (int)CardStatus.Rent, false, (int)StatusMiniSMS.EntitleDelete);
                    }
                    else
                    {
                        if (!_socket.SendEntitlementRequest(Convert.ToInt32(card.Card.CardNum), card.CasIds.ToArray(), card.Card.FinishDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), false))
                        //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                        {
                            return Json(0);
                            //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                        }
                        //miniSMS
                        sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(card.Card.CardNum), card.Card.Id, card.CasIds.ToArray(), card.Card.FinishDate.AddHours(-4), card.Card.FinishDate.AddHours(-4), (int)-2, false, (int)StatusMiniSMS.EntitleDelete);
                    }
                    #region original code
                    //if (!_socket.SendEntitlementRequestTemp(card_num, new short[] { 3, 4 }, new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), false))
                    //{
                    //    return Json(0);
                    //}

                    //SendTempCas(_db, _socket, card.Card.CardNum);

                    //if (!_socket.SendEntitlementRequest(card_num, card.CasIds.ToArray(), DateTime.SpecifyKind(card.Card.CasDate, DateTimeKind.Utc), false))
                    //{
                    //    c
                    //}

                    //if (card.Card.CardStatus != CardStatus.Closed)
                    //{
                    //    Thread.Sleep(2000);
                    //    if (!_socket.SendEntitlementRequest(card_num, card.CasIds.ToArray(), DateTime.SpecifyKind(card.Card.CasDate, DateTimeKind.Utc), true))
                    //    {
                    //        return Json(0);
                    //    }
                    //}
                    #endregion

                    _socket.Disconnect();
                    return Json(1);
                }
            }
            return Json(0);
        }
        [HttpPost]
        public JsonResult Custumer(string code, string phone)
        {
            using (DataContext db = new DataContext())
            {

                var custumer_data = db.Customers.Where(c => c.Code == code || c.Phone1 == phone).ToList().FirstOrDefault();

                if (custumer_data != null)
                    return Json(custumer_data);
            }
            return Json("");
        }
        [HttpPost]
        public JsonResult GetCities(string query)
        {
            XDocument doc = XDocument.Load(Server.MapPath("~/App_Data/city_xml.xml"));
            if (doc != null)
            {
                return Json(doc.Descendants("place").Where(c => c.Element("city").Value.StartsWith(query)).Select(c => c.Element("city").Value + " - " + c.Element("raion").Value).ToList());
            }
            return Json(new string[] { });
        }

        [HttpPost]
        public JsonResult GetRegion(string city, string raion)
        {

            XDocument doc = XDocument.Load(Server.MapPath("~/App_Data/city_xml.xml"));
            if (doc != null)
            {

                return Json(doc.Descendants("place").Where(c => c.Element("city").Value == city && c.Element("raion").Value == raion).Select(c => new { region = c.Element("region").Value, status = c.Element("status").Value }).FirstOrDefault());
            }
            return Json("");
        }

        [HttpPost]
        public PartialViewResult GetDetailFilterModal()
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Towers = _db.Towers.Select(c => new IdName { Id = c.Id, Name = c.Name }).OrderBy(r => r.Name).ToList();
                ViewBag.Receivers = _db.Receivers.Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();

                return PartialView("~/Views/Abonent/_AbonentDetailFilter.cshtml");
            }
        }

        public PartialViewResult GetExistingCard(int id)
        {
            PaymentData payment = new PaymentData() { Cards = new List<int>(), PayType = 2 };
            using (DataContext _db = new DataContext())
            {
                ViewBag.PayTypes = _db.PayTypes.Where(c => c.Id > 1).Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
                ViewBag.Customer = "";
                if (id > 0)
                {
                    Payment pay = _db.Payments
                        .Include(c => c.Card.Customer).Where(p => p.Id == id).FirstOrDefault();
                    if (pay != null)
                    {
                        payment.TransactionId = pay.PayTransaction.ToString();
                        payment.PayType = pay.PayTypeId;
                        payment.Id = pay.Id;
                        payment.Amount = pay.Amount;
                        payment.Cards = new List<int>() { pay.CardId };

                        ViewBag.CardNames = new List<string>() { pay.Card.AbonentNum + " - " + pay.Card.Address };
                        ViewBag.Customer = pay.Card.Customer.Name + " " + pay.Card.Customer.LastName;
                    }
                }
            }
            return PartialView("~/Views/Abonent/_ExistingCard.cshtml", payment);
        }

        [HttpPost]
        public JsonResult getCardsInfo(int id)
        {
            if (id == 0)
                return null;
            List<Card> _card = null;
            try
            {
                using (DataContext _db = new DataContext())
                {
                    _card = _db.Cards.Where(c => c.Id == id).ToList();
                    //return Card;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Json(_card);
        }

        [HttpPost]
        public async Task<JsonResult> DetailFilterAbonents(string type, string abonent, int status, int? tower, int? receiver, string abonent_num,
            MessageFilterBy finish_date, MessageFilterBy pause_date, MessageFilterBy credit_date, MessageFilterBy balance, MessageFilterBy discount, MessageFilterBy service, MessageFilterBy status2,
            int abonent_type)
        {
            using (DataContext _db = new DataContext())
            {
                string where = type == "" ? "" : " AND " + type + "=N'" + abonent + "'";
                where = where.Replace("+", "+' '+");
                where += status == -1 ? "" : (status == 6 ? " AND cr.mode=1 AND cr.status=0" : " AND cr.status=" + status);
                where += !tower.HasValue ? "" : " AND cr.tower_id=" + tower.Value;
                where += !receiver.HasValue ? "" : " AND cr.receiver_id=" + receiver.Value;
                where += abonent_type == -1 ? "" : " AND c.type=" + abonent_type;
                where += abonent_num == "" ? "" : " AND cr.abonent_num+cr.card_num LIKE '%" + abonent_num + "%'";

                string[] charge = _db.Params.Where(p => p.Name == "CardCharge").Select(c => c.Value).First().Split(':');
                string today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge[0]), int.Parse(charge[1]), 0).ToString("yyyy-MM-dd HH:mm") + ":0.000";
                if (!string.IsNullOrEmpty(finish_date.val))
                {
                    where += " AND DATEDIFF(day,'" + today + "',cr.finish_date)" + finish_date.where + finish_date.val;
                }
                if (!string.IsNullOrEmpty(pause_date.val))
                {
                    string p_date = "CAST(CAST(DATEPART(YEAR,cr.pause_date) as VARCHAR(4))+'-'+CAST(DATEPART(MONTH,cr.pause_date) as VARCHAR(2))+'-'+CAST(DATEPART(DAY,cr.pause_date) as VARCHAR(2))+' " + charge[0] + ":" + charge[1] + ":00.000' as DATETIME)";
                    where += " AND DATEDIFF(day,'" + today + "'," + p_date + ")" + pause_date.where + pause_date.val;
                }
                if (!string.IsNullOrEmpty(balance.val))
                {
                    where += " AND ((SELECT ISNULL(SUM(amount),0) FROM doc.Payments WHERE card_id=cr.id) - (SELECT ISNULL(SUM(amount),0) FROM doc.CardCharges WHERE card_id=cr.id)) " + balance.where + balance.val;
                }
                if (!string.IsNullOrEmpty(credit_date.val))
                {
                    where += " AND cr.mode=1 AND DATEDIFF(day,'" + today + "',cr.finish_date)" + credit_date.where + credit_date.val;
                }
                if (!string.IsNullOrEmpty(discount.val))
                {
                    where += " AND cr.discount" + discount.where + discount.val;
                }
                if (!string.IsNullOrEmpty(service.val))
                {
                    string s_date = "(SELECT TOP(1) CAST(CAST(DATEPART(YEAR,tdate) as VARCHAR(4))+'-'+CAST(DATEPART(MONTH,tdate) as VARCHAR(2))+'-'+CAST(DATEPART(DAY, tdate) as VARCHAR(2))+' " + charge[0] + ":" + charge[1] + ":00.000' as DATETIME) FROM doc.CardServices WHERE card_id=cr.id AND is_active=1)";
                    where += " AND (CASE WHEN DATEDIFF(day, cr.finish_date, '" + today + "') < DATEDIFF(day, " + s_date + ", '" + today + "') THEN DATEDIFF(day, cr.finish_date, '" + today + "') ELSE DATEDIFF(day, " + s_date + ", '" + today + "') END)" + service.where + service.val;
                }
                if (!string.IsNullOrEmpty(status2.val))
                {
                    string res = "1=1";
                    switch (status)
                    {
                        case 0:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=0 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 1:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=1 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 2:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=2 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 3:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=3 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 5:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=6 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                    }

                    where += " AND " + res;
                }

                if (where == "")
                    return Json(new List<AbonentList>());

                string sql = @"SELECT d.id AS Id,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.type AS Type,d.city AS City, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status, d.pack AS ActivePacket 
                         FROM (SELECT c.id,c.name,c.lastname,c.code,c.[type],c.city,c.phone1,cr.abonent_num,cr.card_num, cr.status,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1
                         WHERE 1=1 " + where + ") AS d";

                return Json(await _db.Database.SqlQuery<AbonentList>(sql).ToListAsync());
            }
        }

        [HttpGet]
        public async Task<FileResult> DetailFilterAbonentsExport(string type, string abonent, int status, int? tower, int? receiver, string abonent_num, int abonent_type,
            string finish_date_where, string finish_date_val, string pause_date_where, string pause_date_val, string credit_date_where, string credit_date_val,
            string balance_where, string balance_val, string discount_where, string discount_val, string service_where, string service_val, string status2_where, string status2_val)
        {
            using (DataContext _db = new DataContext())
            {
                string where = type == "" ? "" : " AND " + type + "=N'" + abonent + "'";
                where = where.Replace("+", "+' '+");
                where += status == -1 ? "" : (status == 6 ? " AND cr.mode=1 AND cr.status=0" : " AND cr.status=" + status);
                where += !tower.HasValue ? "" : " AND cr.tower_id=" + tower.Value;
                where += !receiver.HasValue ? "" : " AND cr.receiver_id=" + receiver.Value;
                where += abonent_type == -1 ? "" : " AND c.type=" + abonent_type;
                where += abonent_num == "" ? "" : " AND cr.abonent_num+cr.card_num LIKE '%" + abonent_num + "%'";

                string[] charge = _db.Params.Where(p => p.Name == "CardCharge").Select(c => c.Value).First().Split(':');
                string today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge[0]), int.Parse(charge[1]), 0).ToString("yyyy-MM-dd HH:mm") + ":0.000";
                if (!string.IsNullOrEmpty(finish_date_val))
                {
                    where += " AND DATEDIFF(day,'" + today + "',cr.finish_date)" + finish_date_where + finish_date_val;
                }
                if (!string.IsNullOrEmpty(pause_date_val))
                {
                    string p_date = "CAST(CAST(DATEPART(YEAR,cr.pause_date) as VARCHAR(4))+'-'+CAST(DATEPART(MONTH,cr.pause_date) as VARCHAR(2))+'-'+CAST(DATEPART(DAY,cr.pause_date) as VARCHAR(2))+' " + charge[0] + ":" + charge[1] + ":00.000' as DATETIME)";
                    where += " AND DATEDIFF(day,'" + today + "'," + p_date + ")" + pause_date_where + pause_date_val;
                }
                if (!string.IsNullOrEmpty(balance_val))
                {
                    where += " AND ((SELECT ISNULL(SUM(amount),0) FROM doc.Payments WHERE card_id=cr.id) - (SELECT ISNULL(SUM(amount),0) FROM doc.CardCharges WHERE card_id=cr.id)) " + balance_where + balance_val;
                }
                if (!string.IsNullOrEmpty(credit_date_val))
                {
                    where += " AND cr.mode=1 AND DATEDIFF(day,'" + today + "',cr.finish_date)" + credit_date_where + credit_date_val;
                }
                if (!string.IsNullOrEmpty(discount_val))
                {
                    where += " AND cr.discount" + discount_where + discount_val;
                }
                if (!string.IsNullOrEmpty(service_val))
                {
                    string s_date = "(SELECT TOP(1) CAST(CAST(DATEPART(YEAR,tdate) as VARCHAR(4))+'-'+CAST(DATEPART(MONTH,tdate) as VARCHAR(2))+'-'+CAST(DATEPART(DAY, tdate) as VARCHAR(2))+' " + charge[0] + ":" + charge[1] + ":00.000' as DATETIME) FROM doc.CardServices WHERE card_id=cr.id AND is_active=1)";
                    where += " AND (CASE WHEN DATEDIFF(day, cr.finish_date, '" + today + "') < DATEDIFF(day, " + s_date + ", '" + today + "') THEN DATEDIFF(day, cr.finish_date, '" + today + "') ELSE DATEDIFF(day, " + s_date + ", '" + today + "') END)" + service_where + service_val;
                }
                if (!string.IsNullOrEmpty(status2_val))
                {
                    string res = "1=1";
                    switch (status)
                    {
                        case 0:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=0 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2_where + status2_val;
                            break;
                        case 1:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=1 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2_where + status2_val;
                            break;
                        case 2:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=2 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2_where + status2_val;
                            break;
                        case 3:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=3 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2_where + status2_val;
                            break;
                        case 5:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=6 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2_where + status2_val;
                            break;
                    }

                    where += " AND " + res;
                }

                if (where == "")
                    return File(new Export().getExcelData("ExcelExport.xslt", new XElement("root")), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Abonents.xlsx");

                string sql = @"SELECT d.id AS Id,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.type AS Type,d.city AS City, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status, d.pack AS ActivePacket 
                         FROM (SELECT c.id,c.name,c.lastname,c.code,c.[type],c.city,c.phone1,cr.abonent_num,cr.card_num, cr.status,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1
                         WHERE 1=1 " + where + ") AS d";

                List<AbonentList> data = await _db.Database.SqlQuery<AbonentList>(sql).ToListAsync();

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());
                XElement element = new XElement("root",
                   new XElement("columns",
                       new XElement("name", "აბონენტი"),
                       new XElement("name", "ტიპი"),
                       new XElement("name", "ქალაქი"),
                       new XElement("name", "ტელეფონი"),
                       new XElement("name", "აბონენტის №"),
                       new XElement("name", "ბარათის №"),
                       new XElement("name", "სტატუსი"),
                       new XElement("name", "პაკეტი")),
                   data.Select(c => new XElement("data",
                       new XElement("name", c.Name),
                       new XElement("type", CoreHelper.GetCustomerTypeDesc(helper, c.Type)),
                       new XElement("city", c.City),
                       new XElement("phone", c.Phone),
                       new XElement("abonent_num", c.Num),
                       new XElement("card_num", c.CardNum),
                       new XElement("status", CoreHelper.GetCardStatus(helper, c.Status)),
                       new XElement("packets", c.ActivePacket)
                       )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Abonents.xlsx");
            }



        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult NewAbonent(Abonent abonent)
        {
            RetVal retval = new RetVal
            {
                code = 0,
                retvals = new List<Dictionary<string, string>>()
            };
            if (abonent.Customer.Type == CustomerType.Juridical)
            {
                if (ModelState.ContainsKey("Customer.LastName"))
                    ModelState["Customer.LastName"].Errors.Clear();
                abonent.Customer.LastName = "";
            }
            else
            {
                if (ModelState.ContainsKey("Customer.JuridicalFinishDate"))
                    ModelState["Customer.JuridicalFinishDate"].Errors.Clear();
            }

            //if (ModelState.IsValid /*&& Utils.Utils.GetPermission("ABONENT_ADD")*/)
            if (abonent.Customer.Name != null && abonent.Customer.LastName != null &&
                abonent.Customer.Code != null && abonent.Customer.Address != null &&
                abonent.Customer.City != null && abonent.Customer.Phone1 != null &&
                abonent.Customer.Region != null && abonent.Customer.Address != null &&
                abonent.Customer.SecurityCode != null)
            {
                //for (int b = 0; b < 303233; b++)
                JuridicalDocsInfo docsInfo = new JuridicalDocsInfo();

                //foreach (st_Customers st_customer in st_customers)
                {
                    using (DataContext _db = new DataContext())
                    {
                        using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                        {
                            try
                            {
                                {
                                    List<Param> _params = _db.Params.ToList();
                                    int user_id = abonent.UserID;// ((User)Session["CurrentUser"]).Id;
                                    if (_db.Users.Any(c => c.Id == user_id))
                                    {
                                        User user = _db.Users.Where(u => u.Id == user_id).FirstOrDefault();
                                        Dictionary<string, bool> privileges = Utils.Utils.GetPrivilegies(_db, user.Type);
                                        bool is_perrmited = Utils.Utils.GetPermissionForProto("ABONENT_ADD", privileges);

                                        if (!is_perrmited)
                                        {
                                            retval.code = 7;
                                            retval.errorstr = ("თქვენ არ გაქვთ აბონენტის დამატების უფლება!");
                                            return Json(retval);
                                        }
                                        //return 7;
                                    }
                                    else
                                    {
                                        retval.code = 1;
                                        retval.errorstr = ("არასოწრი მომხმარებელი!");
                                        return Json(retval);
                                        //return 1;
                                    }

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(abonent.Logging);

                                    if (_db.Customers.Any(c => c.Code == abonent.Customer.Code))
                                    {
                                        retval.code = 2;
                                        retval.errorstr = ("აბონენტი პირადი ნომრით უკვე არსებობს!");
                                        return Json(retval);
                                        //return 2;
                                        //throw new Exception("აბონენტი " + abonent.Customer.Code + " კოდით უკვე არსებობს");
                                    }

                                    abonent.Customer.SecurityCode = Utils.Utils.GetMd5(abonent.Customer.SecurityCode);

                                    abonent.Customer.UserId = user_id;
                                    if (abonent.Customer.JuridicalFinishDate.HasValue)
                                    {
                                        string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                        DateTime dt = abonent.Customer.JuridicalFinishDate.Value;
                                        abonent.Customer.JuridicalFinishDate = new DateTime(dt.Year, dt.Month, dt.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);
                                    }
                                    _db.Customers.Add(abonent.Customer);
                                    _db.SaveChanges();

                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                                LogType.Abonent,
                                                LogMode.Add,
                                                user_id,
                                                abonent.Customer.Id,
                                                abonent.Customer.Name + " " + abonent.Customer.LastName,
                                                logs.Where(c => c.type == "customer").ToList()
                                            );
                                    }

                                    double package_discount = Convert.ToDouble(_params.Where(p => p.Name == "PackageDiscount").Select(p => p.Value).First());
                                    int free_days = Convert.ToInt32(_params.Where(p => p.Name == "FreeDays").Select(p => p.Value).First());

                                    //string docnum = new WebService1().getDocNum();
                                    //Action<Card> cardDocNumAct = (Card card) =>
                                    //{
                                    //    card.DocNum = docnum;
                                    //};
                                    //string abonent_num = "";
                                    string docnum = new WebService1().getDocNum();
                                    Action<Card> cardAct = (Card card) =>
                                    {
                                        string ab_num = Utils.Utils.IsAbonentNumExists(_db, card.AbonentNum);
                                        if (ab_num != string.Empty)
                                            card.AbonentNum = ab_num;

                                        if (_db.Cards.Any(c => c.DocNum == card.DocNum))
                                        {
                                            //return 2;
                                            throw new Exception(3.ToString());
                                        }

                                        if (_db.Cards.Any(c => c.CardNum == card.CardNum && c.CardStatus != CardStatus.Canceled) && card.CardStatus != CardStatus.Canceled)
                                        {
                                            //return 2;
                                            throw new Exception(4.ToString());
                                        }

                                        card.Tdate = DateTime.Now;
                                        card.CustomerId = abonent.Customer.Id;
                                        //card.CardStatus = CardStatus.Montage;
                                        card.CloseDate = DateTime.Now;
                                        card.UserId = user_id;
                                        card.PauseDate = DateTime.Now;
                                        card.CardLogs = new List<CardLog>() { new CardLog() { Date = card.Tdate, Status = CardLogStatus.Montage, UserId = user_id } };
                                        // card.TowerId = abonent.Cards.Select(s=>s.TowerId).FirstOrDefault();
                                        card.ClosedIsPen = false;
                                        card.AbonentNum = new WebService1().getAbonentNum();
                                        card.DocNum = docnum;
                                        card.juridical_verify_status = "-1";
                                        card.Latitude = abonent.Cards.Select(s => s.Latitude).FirstOrDefault();
                                        card.Longitude = abonent.Cards.Select(s => s.Longitude).FirstOrDefault();
                                        card.mux1_level = abonent.Cards.Select(s => s.mux1_level).FirstOrDefault();
                                        card.mux2_level = abonent.Cards.Select(s => s.mux2_level).FirstOrDefault();
                                        card.mux3_level = abonent.Cards.Select(s => s.mux3_level).FirstOrDefault();
                                        card.mux1_quality = abonent.Cards.Select(s => s.mux1_quality).FirstOrDefault();
                                        card.mux2_quality = abonent.Cards.Select(s => s.mux2_quality).FirstOrDefault();
                                        card.mux3_quality = abonent.Cards.Select(s => s.mux3_quality).FirstOrDefault();
                                        card.RentFinishDate = DateTime.Now;
                                        if (card.CardServices != null)
                                        {
                                            List<int> serv_ids = new List<int>();
                                            foreach (CardService _serv in card.CardServices)
                                            {
                                                _serv.Date = card.Tdate;
                                                _serv.IsActive = _serv.PayType == CardServicePayType.NotCash;

                                                serv_ids.Add(_serv.ServiceId);
                                            }

                                            this.AddLoging(_db,
                                                        LogType.CardService,
                                                        LogMode.Add,
                                                        user_id,
                                                        card.Id,
                                                        card.AbonentNum + " - ის მომსახურება ",
                                                        _db.Services.Where(c => serv_ids.Contains(c.Id)).Select(c => new LoggingData { field = "მომსახურება", new_val = c.Name }).ToList()
                                                    );
                                        }

                                        if (card.Subscribtions != null)
                                        {
                                            foreach (Subscribtion subscrib in card.Subscribtions)
                                            {
                                                int[] arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                var _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();

                                                if (_packages.Any(p => p.RentType != RentType.block && p.RentType != RentType.technic))
                                                {
                                                    subscrib.SubscriptionPackages.Add(new SubscriptionPackage() { PackageId = _db.Packages.Where(pack => pack.RentType == RentType.block).Select(p => p.Id).First() });
                                                    arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                    _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                                                }

                                                subscrib.Amount = _packages.Select(p => abonent.Customer.Type == CustomerType.Juridical ? p.JuridPrice : p.Price).Sum();
                                                subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                                subscrib.Status = true;
                                                subscrib.Tdate = DateTime.Now;
                                                subscrib.UserId = user_id;
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(9.ToString());
                                            //var defaultPackages = _db.Packages.Where(p => p.IsDefault || p.RentType == RentType.block).ToList();
                                            //if (defaultPackages.Count > 0)
                                            //{
                                            //    card.Subscribtions = new List<Subscribtion>()
                                            //               {
                                            //                new Subscribtion {
                                            //                    Amount =  abonent.Customer.Type == CustomerType.Juridical ? defaultPackages.Select(p=>p.JuridPrice).Sum() : defaultPackages.Select(p=>p.Price).Sum(),
                                            //                    Status = true,
                                            //                    Tdate = DateTime.Now,
                                            //                    UserId = user_id,
                                            //                    SubscriptionPackages = defaultPackages.Select(s=>new SubscriptionPackage
                                            //                    {
                                            //                        PackageId = s.Id
                                            //                    }).ToList()
                                            //                }
                                            //               };
                                            //    //card.Subscribtions.First().SubscriptionPackages.Add(new SubscriptionPackage() { PackageId = _db.Packages.Where(pack => pack.RentType == RentType.block).Select(p => p.Id).First() });
                                            //    var subscrib = card.Subscribtions.First();
                                            //    subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                            //}
                                        }

                                        string charge_time = _params.Where(p => p.Name == "CardCharge").First().Value;
                                        if (abonent.Customer.Type == CustomerType.Juridical && abonent.Customer.IsBudget)
                                        {

                                            //card.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(card.Tdate, charge_time, (decimal)card.Subscribtions.Where(s => s.Status).Sum(s => s.Amount), decimal.Parse(_params.Where(p => p.Name == "JuridLimitMonths").First().Value), card.Discount, free_days);
                                            card.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(charge_time);
                                            if (!card.HasFreeDays)
                                            {
                                                card.CardStatus = CardStatus.Active;
                                                free_days = 0;

                                                //original code
                                                //decimal amount = (decimal)(card.Subscribtions.Sum(s=>s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                //int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                                                //decimal amount = (decimal)(card.Subscribtions.Sum(s => s.Amount) / 1/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*// Utils.Utils.divide_card_charge_interval / 60);

                                                //amount -= (amount * (decimal)card.Discount / 100);
                                                //card.CardCharges = new List<CardCharge>() { new CardCharge() { Amount = amount, Tdate = card.CasDate, Status = CardChargeStatus.PreChange } };
                                            }
                                        }
                                        else
                                        {
                                            if (!card.HasFreeDays || free_days == 0)
                                            {
                                                card.CardStatus = CardStatus.Closed;
                                                free_days = 0;
                                            }
                                            else if (card.HasFreeDays && free_days > 0)
                                            {
                                                card.CardStatus = CardStatus.FreeDays;
                                            }


                                            if (abonent.Customer.Type != CustomerType.Juridical)
                                                card.FinishDate = Utils.Utils.GenerateFinishDate(card.Tdate, charge_time).AddDays(free_days);
                                            else
                                            {
                                                card.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(charge_time);
                                                card.CardStatus = CardStatus.Active;
                                            }


                                            if (abonent.Customer.Type == CustomerType.Technic)
                                            {
                                                card.CardStatus = CardStatus.Active;
                                                card.FinishDate = Utils.Utils.GenerateFinishDate(card.Tdate, charge_time).AddYears(20);
                                            }
                                        }

                                        //JobSheduler.resCheduleTrigger("trigger_" + card.Id.ToString(), card.Id, 0, 0);
                                        //card.FinishDate = DateTime.Now.AddMinutes(2);// DateTime.SpecifyKind(DateTime.Now.AddMinutes(2), DateTimeKind.Utc);
                                    };

                                    //abonent.Cards.ForEach(cardDocNumAct);

                                    abonent.Cards.ForEach(cardAct);

                                    _db.Cards.AddRange(abonent.Cards);
                                    _db.SaveChanges();

                                    //abonent.Cards.ForEach(x => JobSheduler.resCheduleTrigger("trigger_" + x.Id.ToString(), x.Id, 0, 0));

                                    //diff docnums
                                    foreach (var item in abonent.Cards)
                                    {
                                        //List<Card> checkcards = abonent.Cards.Where(c=>c.Id != item.Id).ToList();
                                        Subscribtion curr_sb = _db.Subscribtions.Where(s => s.CardId == item.Id && s.Status == true).First();
                                        List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == curr_sb.Id).ToList();
                                        List<Package> curr_packs = new List<Package>();
                                        foreach (var sbs in sbp)
                                        {
                                            Package package = _db.Packages.Where(p => p.Id == sbs.PackageId).First();
                                            if (package.RentType != RentType.block)
                                            {
                                                curr_packs.Add(package);
                                            }
                                            if (package.Id == 304084)
                                            {
                                                //UtilsController ut = new UtilsController();
                                                //var cardsid = new List<int>();
                                                //var packagesid = new List<int>();
                                                //packagesid.Add(304085);
                                                //cardsid.Add(item.Id);
                                                //ut.NewAutoSubscrib(null, item, package, user_id);
                                                List<Package> packToChangeTo = _db.Packages.Where(p => p.Id == 304085 || p.RentType == RentType.block).ToList();
                                                AutoSubscribChangeCard _card = new AutoSubscribChangeCard()
                                                {
                                                    CardId = item.Id,
                                                    UserId = user_id,
                                                    CasIds = String.Join(",", packToChangeTo.Select(p => p.CasId)),
                                                    PackageNames = String.Join("+", packToChangeTo.Select(p => p.Name)),
                                                    PackageIds = String.Join(",", packToChangeTo.Select(p => p.Id)),
                                                    Amount = packToChangeTo.Select(c => c.Price).Sum(),
                                                    SendDate = DateTime.Now.AddDays(30)
                                                };
                                                _db.AutoSubscribChangeCards.Add(_card);
                                                _db.SaveChanges();
                                            }
                                            if (package.Id == 304086)
                                            {
                                                //UtilsController ut = new UtilsController();
                                                //var cardsid = new List<int>();
                                                //var packagesid = new List<int>();
                                                //packagesid.Add(304085);
                                                //cardsid.Add(item.Id);
                                                //ut.NewAutoSubscrib(null, item, package, user_id);
                                                List<Package> packToChangeTo = _db.Packages.Where(p => p.Id == 304071 || p.RentType == RentType.block).ToList();
                                                AutoSubscribChangeCard _card = new AutoSubscribChangeCard()
                                                {
                                                    CardId = item.Id,
                                                    UserId = user_id,
                                                    CasIds = String.Join(",", packToChangeTo.Select(p => p.CasId)),
                                                    PackageNames = String.Join("+", packToChangeTo.Select(p => p.Name)),
                                                    PackageIds = String.Join(",", packToChangeTo.Select(p => p.Id)),
                                                    Amount = packToChangeTo.Select(c => c.Price).Sum(),
                                                    SendDate = DateTime.Now.AddDays(30)
                                                };
                                                _db.AutoSubscribChangeCards.Add(_card);
                                                _db.SaveChanges();
                                            }
                                        }

                                        foreach (var checkcards in abonent.Cards.Where(c => c.Id != item.Id).ToList())
                                        {
                                            Subscribtion check_sb = _db.Subscribtions.Where(s => s.CardId == checkcards.Id && s.Status == true).First();
                                            List<SubscriptionPackage> check_sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == check_sb.Id).ToList();
                                            List<Package> check_packs = new List<Package>();

                                            foreach (var checks_sbps in check_sbp)
                                            {
                                                Package package = _db.Packages.Where(p => p.Id == checks_sbps.PackageId).First();
                                                if (package.RentType != RentType.block)
                                                {
                                                    check_packs.Add(package);
                                                }
                                            }

                                            if (check_packs.Any(p => p.RentType == curr_packs.First().RentType))
                                            {

                                            }
                                            else
                                            {
                                                checkcards.DocNum = new WebService1().getDocNum();
                                                _db.Entry(checkcards).State = EntityState.Modified;
                                                _db.SaveChanges();
                                            }

                                        }

                                    }

                                    string[] address = _params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                                    CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                    _socket.Connect();

                                    foreach (Card c in abonent.Cards)
                                    {
                                        this.AddLoging(_db,
                                                LogType.Card,
                                                LogMode.Add,
                                                user_id,
                                                c.Id,
                                                c.AbonentNum,
                                                logs.Where(cc => cc.type != "customer").ToList()
                                            );
                                        _db.JuridicalStatus.Add(new JuridicalStatus()
                                        {
                                            tdate = DateTime.Now,
                                            card_id = c.Id,
                                            user_id = user_id,
                                            status = -1

                                        });
                                        _db.JuridicalLoggings.Add(new JuridicalLogging()
                                        {
                                            tdate = DateTime.Now,
                                            card_id = c.Id,
                                            user_id = 1,
                                            status = -1,
                                            name = _db.Database.SqlQuery<string>("SELECT u.name FROM book.Users u where u.id=" + 1).FirstOrDefault()
                                        });
                                        _db.SaveChanges();

                                        if (c.Subscribtions.First().SubscriptionPackages.Any(p => p.Package.RentType == RentType.block))
                                        {
                                            if (c.Subscribtions.First().SubscriptionPackages.Any(p => p.Package.RentType == RentType.buy))
                                            {
                                                short[] cas_ids = c.Subscribtions.First().SubscriptionPackages.Where(p => p.Package.RentType == RentType.block).Select(p => (short)p.Package.CasId).ToArray();
                                                if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), cas_ids, c.CloseDate.AddHours(-4), new DateTime(3000, 1, 1, 0, 0, 0, 0), true))
                                                {
                                                    //throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                                }
                                            }
                                        }

                                        //if (c.CardStatus != CardStatus.Closed)
                                        {
                                            //SEND CAS DATA
                                            //if (!_socket.SendCardStatus(Convert.ToInt32(c.CardNum), true, DateTime.SpecifyKind(DateTime.Now.AddDays(3), DateTimeKind.Utc)))
                                            //{
                                            //    throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                            //}

                                            short[] cas_ids = { };
                                            if (c.Subscribtions != null && c.Subscribtions.Count > 0)
                                                cas_ids = c.Subscribtions.First().SubscriptionPackages.Select(p => (short)p.Package.CasId).ToArray();



                                            //original code
                                            //if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), cas_ids, c.CloseDate.AddHours(-4), c.FinishDate.AddHours(-4), true))
                                            //{
                                            //    //throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                            //    tran.Rollback();
                                            //    retval.code = 8;
                                            //    retval.errorstr = ("ბარათი ვერ გააქტიურდა!");
                                            //    return Json(retval);
                                            //    //return 8;
                                            //}

                                            //check card existance in cas base
                                            if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), cas_ids, DateTime.Now.AddHours(-8), DateTime.Now.AddHours(-4), true))
                                            {
                                                tran.Rollback();
                                                retval.code = 8;
                                                retval.errorstr = ("ბარათი ვერ გააქტიურდა!");
                                                return Json(retval);
                                            }

                                            //დროებითი
                                            //if (!_socket.SendEntitlementRequestTemp(Convert.ToInt32(c.CardNum), cas_ids, DateTime.SpecifyKind(new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), DateTimeKind.Utc), false))
                                            //{
                                            //}
                                            //SendTempCas(_db, _socket, c.CardNum);
                                        }

                                        //if (!c.HasFreeDays)
                                        //{
                                        //    short[] cas_ids = { };
                                        //    if (c.Subscribtions != null && c.Subscribtions.Count > 0)
                                        //        cas_ids = c.Subscribtions.First().SubscriptionPackages.Select(p => (short)p.Package.CasId).ToArray();
                                        //    //დროებითი
                                        //    if (!_socket.SendEntitlementRequestTemp(Convert.ToInt32(c.CardNum), cas_ids, DateTime.SpecifyKind(new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), DateTimeKind.Utc), false))
                                        //    {
                                        //    }

                                        //   SendTempCas(_db, _socket, c.CardNum);
                                        //}
                                        Dictionary<string, string> dic = new Dictionary<string, string>();
                                        dic.Add("abnum", c.AbonentNum);
                                        dic.Add("docnum", c.DocNum);
                                        dic.Add("CardID", c.Id.ToString());
                                        retval.retvals.Add(dic);
                                    }

                                    _socket.Disconnect();

                                    //if (Session["order"] != null)
                                    //{
                                    //    int order_id = (int)Session["order"];
                                    //    Order order = _db.Orders.Where(o => o.Id == order_id).FirstOrDefault();
                                    //    if (order != null)
                                    //    {
                                    //        order.Status = OrderStatus.Closed;
                                    //        _db.Entry(order).State = EntityState.Modified;
                                    //        _db.SaveChanges();
                                    //        Session.Remove("order");
                                    //    }
                                    //}

                                    if (abonent.attachments != null && abonent.attachments.Count > 0)
                                    {
                                        List<CustomerSellAttachments> cust_attachs = new List<CustomerSellAttachments>();

                                        foreach (var item in abonent.attachments)
                                        {
                                            if (item.Value != 0 && item.Value > 0)
                                            {
                                                docsInfo.attachment_docs = "დანართი";

                                                if (abonent.Cards.Select(s => s.Subscribtions.First().SubscriptionPackages.Any(a => a.PackageId == 304086)).FirstOrDefault())
                                                    cust_attachs.Add(new CustomerSellAttachments() { AttachmentID = item.Id, VerifyStatus = 5, status = SellAttachmentStatus.temporary_use, CustomerID = abonent.Customer.Id, Count = item.Value, Diler_Id = user_id });
                                                else
                                                {
                                                    if (abonent.Customer.temporary_use == 1)
                                                    {
                                                        cust_attachs.Add(new CustomerSellAttachments() { AttachmentID = item.Id, VerifyStatus = 6, status = SellAttachmentStatus.temporary_use, CustomerID = abonent.Customer.Id, Count = item.Value, Diler_Id = user_id });
                                                    }
                                                    else
                                                    {
                                                        cust_attachs.Add(new CustomerSellAttachments() { AttachmentID = item.Id, CustomerID = abonent.Customer.Id, Count = item.Value, Diler_Id = user_id });
                                                    }
                                                }
                                            }
                                        }

                                        _db.CustomerSellAttachments.AddRange(cust_attachs);
                                        _db.SaveChanges();
                                    }

                                    tran.Commit();
                                    //MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "AbonentRegistration");

                                    string onRegMsg = "";

                                    MessageTemplate msg = _db.MessageTemplates.Where(m => m.Name == "AbonentRegistration").FirstOrDefault();
                                    if (msg != null)
                                    {
                                        {
                                            string abonent_nums = "";
                                            double ammount = 0;
                                            foreach (Card card in abonent.Cards)
                                            {
                                                abonent_nums += " " + card.AbonentNum + " ";
                                                foreach (Subscribtion subscrib in card.Subscribtions)
                                                {
                                                    ammount += subscrib.Amount;
                                                }
                                                //ammount += card.Subscribtions
                                            }
                                            if (abonent.Cards.Select(s => s.Subscribtions.First().SubscriptionPackages.Any(a => a.PackageId == 304086)).FirstOrDefault())
                                            {
                                                msg = _db.MessageTemplates.Where(m => m.Name == "AbonentRegistrationFree").FirstOrDefault();
                                                onRegMsg = String.Format(msg.Desc, abonent_nums);
                                            }
                                            else
                                            {

                                                onRegMsg = String.Format(msg.Desc, abonent_nums, ammount);
                                            }
                                            Task.Run(async () => { await Utils.Utils.sendMessage(abonent.Customer.Phone1, onRegMsg); }).Wait();
                                            //Utils.Utils.sendMessage(abonent.Customer.Phone1, message.Desc);
                                        }
                                    }


                                    foreach (Card c in abonent.Cards)
                                    {
                                        PaymentData pay_data = new PaymentData();
                                        pay_data.Cards = new List<int>();
                                        PaymentController payment = new PaymentController();


                                        //c.Subscribtions.First().Amount +=
                                        pay_data.PayType = 17;
                                        pay_data.Amount = (decimal)c.Subscribtions.Select(s => s.Amount).Sum();

                                        pay_data.Cards.Add(c.Id);
                                        //payment.NewPayment(pay_data, null);
                                        int pay_ret = payment.SavePayment(pay_data, user_id);
                                    }

                                }

                                //return 0;
                                retval.code = 0;
                                retval.errorstr = ("");
                                //var pack_name = _db.Database.SqlQuery<string>(@"select p.name from doc.Subscribes AS s
                                //                                                inner join doc.SubscriptionPackages AS sp on s.id=sp.subscription_id
                                //                                                inner join book.Packages AS p on p.id=sp.package_id where s.card_id=" + abonent.Cards.Select(s => s.Id).FirstOrDefault() + "").FirstOrDefault();
                                //JuridicalController juridicalController = new JuridicalController();
                                //juridicalController.JuridicalDocs(abonent, pack_name);
                                //juridicalController.JuridicalDocsAttachment(abonent, pack_name);
                                //docsInfo.abonent_name = abonent.Customer.Name + " " + abonent.Customer.LastName;
                                //docsInfo.card_id = abonent.Cards.Select(s => s.Id).FirstOrDefault();
                                //docsInfo.subscription_docs = "სააბონენტო ხელშეკრულება";
                                //_db.JuridicalDocsInfo.Add(docsInfo);
                                //_db.SaveChanges();
                                //    new GeneratorDocs(
                                //    new RenderViewString(
                                //            "Contracts",
                                //            "~/Views/Contracts/ტესტ.cshtml",
                                //            new ConvertImageBase64(
                                //                new AbonentGenaratorDate(
                                //                    new SqlConnection(
                                //                         ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                //                   /* Convert.ToInt32*/(abonent.Customer.Code)
                                //                ).Result()
                                //            ).ResultImage()
                                //        ),
                                //    abonent.Customer.Name + abonent.Customer.LastName

                                //).Result();
                                return Json(retval);
                            }
                            catch (Exception ex)
                            {

                                ViewBag.Error = ex.Message;
                                tran.Rollback();
                                if (ex.Message != null)
                                    if (ex.Message.Length == 1)
                                    {

                                        int err;
                                        int.TryParse(ex.Message, out err);
                                        retval.code = err;
                                        if (retval.code == 9)
                                            retval.errorstr = ("აირჩიეთ სწორი პაკეტი!");
                                        else if (retval.code == 4)
                                        {
                                            retval.errorstr = ("ბარათი უკვე დარეგისტრირებულია!");
                                        }
                                        else
                                            retval.errorstr = ("from inner exeption");
                                        return Json(retval);
                                    }

                                retval.code = 5;
                                retval.errorstr = ("სხვა... მიმართეთ ადმინისტრატორს.");
                                return Json(retval);
                                //return 5;
                                //abonent.Cards.ForEach(x => JobSheduler.RemoveTrigger("trigger_" + x.Id.ToString()));
                                //JobSheduler.RemoveTrigger("trigger_" + card.Id.ToString(), card.Id, 0, 0);
                            }

                        }

                        Param param = _db.Params.Single(m => m.Name == "FreeDays");
                        ViewBag.FreeDays = Convert.ToInt32(param.Value);
                    }
                }
            }
            else
            {
                retval.code = 6;
                retval.errorstr = ("არასოწრი აპრამეტრები, შეავსეთ აუცილებელი ველები!");
                return Json(retval);
                //return 6;
            }
            //using (DataContext _db = new DataContext())
            //{
            //    ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
            //    ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList();
            //    List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType<DescriptionAttribute>(c).Description }).ToList();
            //    ViewBag.CardStatus = CardEnums;
            //    for (int i = 0; i < abonent.Cards.Count; i++)
            //    {
            //        string max_num = _db.Cards.Select(cc => cc.AbonentNum).OrderByDescending(cc => cc).FirstOrDefault();
            //        abonent.Cards[i].AbonentNum = Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1)));
            //        abonent.Cards[i].Id = 0;
            //    }
            //}

        }

        [HttpGet]
        public string __newAbonent()
        {
            using (DataContext _db = new DataContext())
            {
                //ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
                //ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList();
                List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType<DescriptionAttribute>(c).Description }).ToList();
                //ViewBag.CardStatus = CardEnums;
                string max_num = _db.Cards.Select(c => c.AbonentNum).OrderByDescending(c => c).FirstOrDefault();

                Abonent abonent = new Abonent();

                abonent = new Abonent() { Customer = new Customer(), Cards = new List<Card> { new Card { AbonentNum = ""/*Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1)))*/ } } };

                if (abonent.Cards == null)
                {
                    abonent.Cards = new List<Card> { new Card { AbonentNum = Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1))) } };
                }
                abonent.attachments = _db.SellAttachments.ToList();
                //Param param = _db.Params.Single(m => m.Name == "FreeDays");
                //ViewBag.FreeDays = Convert.ToInt32(param.Value);
                //return View(abonent);
                string body = RenderNewAbonentViewToString("Abonent", "~/Views/Abonent/__New.cshtml", abonent, _db, CardEnums);
                return body;
            }
        }

        [NonAction]
        public string __AddCard(int count)
        {
            using (DataContext _db = new DataContext())
            {
                List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType<DescriptionAttribute>(c).Description }).ToList();

                string body = RenderCardViewToString("Abonent", "~/Views/Abonent/__Card.cshtml", new Abonent() { Customer = new Customer(), Cards = new List<Card>() { new Card() } }, _db, CardEnums, count);

                return body;
            }
        }

        [NonAction]
        public List<SellAttachment> __getSellAttachments(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<UserDetails> details = new List<UserDetails>();
                UserDetails detail = new UserDetails();

                var users = _db.Users.Where(u => u.Id == user_id).ToList();

                if (users != null)
                    foreach (var item in users)
                    {
                        detail.user = item;
                        detail.userType = _db.UserTypes.Where(t => t.Id == detail.user.Type).FirstOrDefault();
                        detail.sellerObj = _db.Seller.Where(s => s.ID == detail.user.@object).FirstOrDefault();

                        details.Add(detail);
                    }

                List<SellAttachment> attachs = _db.SellAttachments.ToList();
                UserType userType = _db.UserTypes.Where(t => t.Id == _db.Users.Where(u => u.Id == user_id).FirstOrDefault().Type).First();//ViewBag.UserType;

                //string body = RenderUserDetailsToString("Books", "~/Views/Books/__UserDetails.cshtml", details, _db, attachs, userType);

                return attachs;
            }
        }

        [NonAction]
        public string __AddSubscribtion(int id, CustomerType type, int user_id)
        {
            ViewBag.CardId = id;
            using (DataContext _db = new DataContext())
            {
                ViewBag.Type = type;
                ViewBag.Discount = Convert.ToDouble(_db.Params.Where(p => p.Name == "PackageDiscount").Select(p => p.Value).First());
                ViewBag.User = _db.Users.Where(s => s.Id == user_id).Select(ss => ss.Type).FirstOrDefault();
                string body = RenderSubscribtionViewToString("Abonent", "~/Views/Abonent/__AddSubscription.cshtml", _db.Packages.ToList(), _db, id, type, user_id);
                return body;
            }
        }

        [NonAction]
        public string __getDocument(int card_id, int user_id, Dictionary<string, string> retval)
        {
            string DocName = "მიღება-ჩაბარება დილერისთვის ფიზიკური";
            int userid = user_id;// ((User)Session["CurrentUser"]).Id;
            Customer cust = new Customer();
            AbonentDoc model = new AbonentDoc();
            model.cards = new List<string>();

            using (DataContext _db = new DataContext())
            {
                model.user = _db.Users.Where(u => u.Id == userid).First();
                model.seller = _db.Seller.Where(s => s.ID == model.user.@object).FirstOrDefault();
                //cust.Cards = _db.Cards.Where(c => c.Id == card_id).ToList();
                cust = _db.Customers.Where(c => c.Id == _db.Cards.Where(c1 => c1.Id == card_id).Select(c1 => c1.CustomerId).FirstOrDefault()).First();
                cust.Cards = _db.Cards.Where(c => c.CustomerId == cust.Id).ToList();
                model.customer = cust;
                Card crd = _db.Cards.Where(c => c.Id == card_id).FirstOrDefault();

                model.cards.Add(crd.AbonentNum);
                List<string> abnums = _db.Cards.Where(c => c.DocNum == crd.DocNum && c.Id != crd.Id).Select(c => c.AbonentNum).ToList();
                if (abnums.Count > 0)
                {
                    foreach (var item in abnums)
                    {
                        model.cards.Add(item);
                    }
                }

                List<Subscribtion> sb = _db.Subscribtions.Where(s => s.CardId == card_id && s.Status == true).ToList();
                foreach (var item in sb)
                {

                    List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == item.Id).ToList();
                    foreach (var sbs in sbp)
                    {
                        Package package = _db.Packages.Where(p => p.Id == sbs.PackageId).First();

                        if (package.RentType == RentType.rent)
                        {
                            if (package.Name == "თანამშრომელი")
                            {
                                DocName = "სააბონენტო ხელშეკრულება - დროებითი სარგებლობისას - თანამშრომელზე";
                                break;
                            }
                            else
                            {
                                DocName = "სააბონენტო ხელშეკრულება - დროებითი სარგებლობისას - ფიზიკური პირი";
                                break;
                            }
                        }
                    }
                }
                retval.Add("DocName", DocName);
                retval.Add("CardNum", cust.Cards.Where(c => c.Id == card_id).Select(s => s.CardNum).FirstOrDefault());
                string body = RenderDocumentViewToString("Abonent", DocName, model);
                return body;
            }
        }

        public static string RenderCardViewToString(string controllerName, string viewName, object viewData, DataContext _db, List<IdName> CardEnums, int count)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                viewContext.ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
                viewContext.ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList();
                viewContext.ViewBag.CardStatus = CardEnums;
                viewContext.ViewBag.Count = count;
                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        public static string RenderUserDetailsToString(string controllerName, string viewName, object viewData, DataContext _db, List<SellAttachment> attachs, UserType userType)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                viewContext.ViewBag.attachs = attachs;
                viewContext.ViewBag.UserType = userType;

                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        public static string RenderSubscribtionViewToString(string controllerName, string viewName, object viewData, DataContext _db, int id, CustomerType type, int user_id)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                viewContext.ViewBag.Discount = Convert.ToDouble(_db.Params.Where(p => p.Name == "PackageDiscount").Select(p => p.Value).First());
                viewContext.ViewBag.Type = type;
                viewContext.ViewBag.CardId = id;

                viewContext.ViewBag.ShowShare = Utils.Utils.GetPermission(user_id, "SHARE_8_SHOW");

                User user = _db.Users.Where(u => u.Id == user_id).FirstOrDefault();
                viewContext.ViewBag.User = user.Type;
                Dictionary<string, bool> privileges = Utils.Utils.GetPrivilegies(_db, user.Type);
                bool is_perrmited = Utils.Utils.GetPermissionForProto("SHARE_8_SHOW", privileges);

                viewContext.ViewBag.ShowShare = is_perrmited;
                //viewContext.ViewBag.ShowShare = Utils.Utils.GetPermission("SHARE_8_SHOW");

                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        public static string RenderDocumentViewToString(string controllerName, string viewName, object viewData)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);

                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        public static string RenderNewAbonentViewToString(string controllerName, string viewName, object viewData, DataContext _db, List<IdName> CardEnums)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                viewContext.ViewBag.FreeDays = Convert.ToInt32(_db.Params.Single(m => m.Name == "FreeDays").Value);
                viewContext.ViewBag.CardStatus = CardEnums;
                viewContext.ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();

                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        //for pdf
        public string RenderViewAsString(string viewName, object model, bool isFromDiller = false)
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();

            // get the view to render
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    new ViewDataDictionary(model),
                    new TempDataDictionary(),
                    stringWriter
                    );
            viewContext.ViewBag.isFromDiller = isFromDiller;

            // render the view to a HTML code
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }

        //[HttpGet]
        //public ActionResult ConvertThisPageToPdf()
        //{
        //    // get the HTML code of this view
        //    string htmlToConvert = RenderViewAsString("Doc1", null);

        //    // the base URL to resolve relative images and css
        //    String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
        //    String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Abonent/ConvertThisPageToPdf".Length);

        //    // instantiate the HiQPdf HTML to PDF converter
        //    HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

        //    // hide the button in the created PDF
        //    //htmlToPdfConverter.HiddenHtmlElements = new string[] { "#convertThisPageButtonDiv" };

        //    // render the HTML code as PDF in memory
        //    byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

        //    // send the PDF file to browser
        //    FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
        //    fileResult.FileDownloadName = "ThisMvcViewToPdf.pdf";

        //    return fileResult;
        //}

        //[HttpPost]
        //public ActionResult ConvertAboutPageToPdf()
        //{
        //    // get the About view HTML code
        //    string htmlToConvert = RenderViewAsString("Abonent", null);

        //    // the base URL to resolve relative images and css
        //    String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
        //    String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);

        //    // instantiate the HiQPdf HTML to PDF converter
        //    HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

        //    // render the HTML code as PDF in memory
        //    byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

        //    // send the PDF file to browser
        //    FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
        //    fileResult.FileDownloadName = "AboutMvcViewToPdf.pdf";

        //    return fileResult;
        //}

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult GenerateDoc(int card_id)
        {
            string DocName = "მიღება-ჩაბარება ოფისიდან ფიზიკური პირისთვის";
            int user_id;// = ((User)Session["CurrentUser"]).Id;
            bool isFromDiller = false;
            Customer cust = new Customer();
            AbonentDoc model = new AbonentDoc();
            model.cards = new List<string>();

            using (DataContext _db = new DataContext())
            {

                //cust.Cards = _db.Cards.Where(c => c.Id == card_id).ToList();
                cust = _db.Customers.Where(c => c.Id == _db.Cards.Where(c1 => c1.Id == card_id).Select(c1 => c1.CustomerId).FirstOrDefault()).First();
                cust.Cards = _db.Cards.Where(c => c.CustomerId == cust.Id).ToList();
                model.customer = cust;
                Card crd = _db.Cards.Where(c => c.Id == card_id).FirstOrDefault();

                user_id = _db.Loggings.Where(l => l.Type == LogType.Card && l.Mode == LogMode.Add && l.TypeId == card_id).Select(l => l.UserId).First();

                model.user = _db.Users.Where(u => u.Id == user_id).First();
                model.user.UserType = _db.UserTypes.Where(u => u.Id == model.user.Type).First();
                model.seller = _db.Seller.Where(s => s.ID == model.user.@object).FirstOrDefault();

                if (model.user.UserType.Id == 18)
                {
                    DocName = "მიღება-ჩაბარება დილერისთვის ფიზიკური";
                    isFromDiller = true;
                }

                model.cards.Add(crd.AbonentNum);
                List<string> abnums = _db.Cards.Where(c => c.DocNum == crd.DocNum && c.Id != crd.Id).Select(c => c.AbonentNum).ToList();
                if (abnums.Count > 0)
                {
                    foreach (var item in abnums)
                    {
                        model.cards.Add(item);
                    }
                }

                List<Subscribtion> sb = _db.Subscribtions.Where(s => s.CardId == card_id && s.Status == true).ToList();
                foreach (var item in sb)
                {

                    List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == item.Id).ToList();
                    foreach (var sbs in sbp)
                    {
                        Package package = _db.Packages.Where(p => p.Id == sbs.PackageId).First();

                        if (package.RentType == RentType.rent)
                        {
                            if (package.Name == "თანამშრომელი")
                            {
                                DocName = "სააბონენტო ხელშეკრულება - დროებითი სარგებლობისას - თანამშრომელზე";
                                break;
                            }
                            else
                            {
                                DocName = "სააბონენტო ხელშეკრულება - დროებითი სარგებლობისას - ფიზიკური პირი";
                                break;
                            }
                        }
                    }
                }

            }
            // read parameters from the webpage
            //string htmlString = collection["TxtHtmlCode"];
            string htmlString = RenderViewAsString(DocName, model, isFromDiller);
            //string htmlString1 = RenderViewAsString("ნასყიდობა ფიზიკური პირი", null);
            string baseUrl = "";// collection["TxtBaseUrl"];

            string pdf_page_size = "A4";// collection["DdlPageSize"];
            SelectPdf.PdfPageSize pageSize = (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";// collection["DdlPageOrientation"];
            SelectPdf.PdfPageOrientation pdfOrientation =
                (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                pdf_orientation, true);

            string pdf_align = "Justify";// collection["DdlPageOrientation"];
            SelectPdf.PdfTextHorizontalAlign pdfTextAlign =
                (SelectPdf.PdfTextHorizontalAlign)Enum.Parse(typeof(SelectPdf.PdfTextHorizontalAlign),
                pdf_align, true);

            int webPageWidth = 1024;
            //try
            //{
            //    webPageWidth = Convert.ToInt32(collection["TxtWidth"]);
            //}
            //catch { }

            int webPageHeight = 0;
            //try
            //{
            //    webPageHeight = Convert.ToInt32(collection["TxtHeight"]);
            //}
            //catch { }

            // instantiate a html to pdf converter object
            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();


            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = DocName + "(" + cust.Cards.Where(c => c.Id == card_id).Select(s => s.CardNum).FirstOrDefault() + ").pdf";
            return fileResult;

            //Response.AppendHeader("Content-Disposition", "inline; filename=" + "test.pdf");
            //return File(pdf, "application/pdf");

        }
    }

    public class FakeController : ControllerBase { protected override void ExecuteCore() { } }
}
