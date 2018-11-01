using DigitalTVBilling.Filters;
using DigitalTVBilling.Jobs;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using PagedList.Mvc;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class UtilsController : BaseController
    {
        public async Task<ActionResult> OfficeCards(int page = 1)
        {
            if (!Utils.Utils.GetPermission("OFFICE_CARDS"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                var __cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.CardStatus != CardStatus.Canceled && c.CardStatus==CardStatus.Closed && c.Customer.Type == CustomerType.Physical).ToList();
                __cards = __cards.Where(c => c.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Select(s => s).ToList().Count > 0).ToList();
                List<Customer> customers = new List<Customer>();
                var count = 0;
                //CASSocket _socket = new CASSocket() { IP = "192.168.4.143", Port = 8000 };
                //_socket.Connect();

                foreach (var item in __cards)
                {

                    CardDetailData _card = _db.Cards.Where(c => c.Id == item.Id).Select(c => new CardDetailData
                    {
                        PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                        ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        Card = c,
                        CustomerType = c.Customer.Type,
                        IsBudget = c.Customer.IsBudget,
                        SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                        MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                        CardLogs = c.CardLogs.ToList()
                    }).FirstOrDefault();

                    if (_card != null)
                    {

                        decimal balance = Math.Round(_card.PaymentAmount - _card.ChargeAmount, 2);
                        decimal amount = (decimal)_card.SubscribAmount;
                        if (amount == 0)
                            return null;

                        if (balance >Convert.ToDecimal(0.50) && balance<amount)
                        {
                            count++;
                            //Task.Run(async () => { await Utils.Utils.sendMessage("598733767", "Tu gsurt paketis gaumjobeseba da yvela kodirebuli arxis chartva, dagvikavshirdit nomerze: 032 205 12 12"+item.AbonentNum); }).Wait();
                        }



                    }
                }
                return View(await _db.OfficeCards.OrderByDescending(c => c.Tdate).ToPagedListAsync(page, 50));
            }
        }

        public PartialViewResult NewOfficeCard(int id)
        {
            OfficeCard _card = new OfficeCard();
            if (id > 0)
            {
                using (DataContext _db = new DataContext())
                {
                    _card = _db.OfficeCards.Where(c => c.Id == id).FirstOrDefault();
                }
            }
            return PartialView("~/Views/Utils/_NewOfficeCard.cshtml", _card);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult NewOfficeCard(OfficeCard card)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int c_id = 0;
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        card.Tdate = DateTime.Now;
                        string send_card = "";
                        if (card.Id > 0)
                        {
                            OfficeCard _card = _db.OfficeCards.Where(c => c.Id == card.Id).FirstOrDefault();
                            if (_card != null)
                            {
                                c_id = card.Id;
                                _card.Name = card.Name;
                                _card.Address = card.Address;

                                _db.Entry(_card).State = EntityState.Modified;
                                List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(card.Logging);
                                if (logs != null)
                                {
                                    this.AddLoging(_db,
                                        LogType.OfficeCard,
                                        LogMode.Change,
                                        user_id,
                                        card.Id,
                                        card.Name,
                                        logs
                                    );
                                }
                                _db.SaveChanges();
                            }
                        }
                        else
                        {
                            _db.OfficeCards.Add(card);
                            _db.SaveChanges();

                            this.AddLoging(_db,
                                LogType.OfficeCard,
                                LogMode.Add,
                                user_id,
                                card.Id,
                                card.Name,
                                Utils.Utils.GetAddedData(typeof(OfficeCard), card)
                            );

                            send_card = card.CardNum;
                        }


                        string[] address = _db.Params.First(c => c.Name == "CASAddress").Value.Split(':');
                        CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        _socket.Connect();

                        if (c_id == 0)
                        {
                            short[] cas = { 3, 4 };
                            if (card.Packages != null)
                                cas = _db.Packages.Where(p => card.Packages.Contains(p.Id)).Select(c => (short)c.CasId).ToArray();

                            if (!_socket.SendEntitlementRequest(int.Parse(send_card), cas, DateTime.SpecifyKind(card.Tdate, DateTimeKind.Utc), true))
                            {
                                throw new Exception();
                            }
                        }

                        _socket.Disconnect();
                        _db.SaveChanges();
                        tran.Commit();
                        return Json(1);
                    }
                    catch
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteOfficeCard(int id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        OfficeCard _card = _db.OfficeCards.Where(c => c.Id == id).FirstOrDefault();
                        if (_card != null)
                        {
                            _db.Entry(_card).State = EntityState.Deleted;
                            this.AddLoging(_db,
                                LogType.OfficeCard,
                                LogMode.Delete,
                                user_id,
                                0,
                                _card.Name,
                                new List<LoggingData>()
                            );

                            string[] address = _db.Params.First(c => c.Name == "CASAddress").Value.Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendEntitlementRequest(int.Parse(_card.CardNum), new short[] { 3, 4 }, DateTime.SpecifyKind(_card.Tdate, DateTimeKind.Utc), false))
                            {
                                throw new Exception();
                            }
                            _socket.Disconnect();

                        }

                        tran.Commit();
                        return Json(1);
                    }
                    catch
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
        }

        public ActionResult AutoSubscribs()
        {
            if (!Utils.Utils.GetPermission("AUTO_CHANGE_SUBSCRIB"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                return View(_db.AutoSubscribChangeCards.Where(c=>c.Card.CardStatus!=CardStatus.Canceled).Select(c => new AutoChangeSubscribList
                {
                    Id = c.Id,
                    Abonent = c.Card.Customer.Name + " " + c.Card.Customer.LastName,
                    AbonentNum = c.Card.AbonentNum,
                    PackageNames = c.PackageNames,
                    SendDate = c.SendDate.HasValue ? c.SendDate.Value : c.Card.FinishDate
                }).OrderBy(c => c.SendDate).ToList());
            }
        }

        public PartialViewResult NewAutoSubscrib()
        {
            return PartialView("~/Views/Utils/_NewAutoSubscrib.cshtml");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult NewAutoSubscrib(DateTime? SendDate, List<int> Cards, List<int> Packages)
        {
            if (Cards == null || Cards.Count == 0)
                return Json("ბარათი არ არის არჩეული!");
            if (Packages == null || Packages.Count == 0)
                return Json("პაკეტი არ არის არჩეული!");

            Packages.Add(304073);
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int package_days = int.Parse(_db.Params.First(c => c.Name == "PacketChangeTime").Value);
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        foreach (int card_id in Cards)
                        {
                            CardDetailData cur_card = _db.Cards.Where(c => c.Id == card_id)
                                .Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                                {
                                    Card = c,
                                    CustomerType = c.Customer.Type,
                                }).FirstOrDefault();
                            if (cur_card != null)
                            {
                                var _packages = _db.Packages.Where(p => Packages.Contains(p.Id) || p.RentType == RentType.block).Select(c => new { Name = c.Name, CasId = c.CasId, Amount = cur_card.CustomerType == CustomerType.Juridical ? c.Price : c.JuridPrice }).ToList();
                                if (SendDate != null)
                                {
                                    Subscribtion updSubscrbs = _db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == card_id).OrderByDescending(s => s.Tdate).FirstOrDefault();
                                    if (updSubscrbs != null)
                                    {
                                        if (_packages.Select(s => s.Amount).Sum() < updSubscrbs.Amount)
                                        {
                                            //if ((SendDate.Value - updSubscrbs.Tdate).Days <= package_days)
                                            //{
                                            //    tran.Rollback();
                                            //    return Json(cur_card.Card.CardNum + " - ზე არჩეული ბოლო პაკეტიდან არ არის გასული " + package_days + " დღე!");
                                            //}
                                        }
                                    }
                                }

                                AutoSubscribChangeCard _card = new AutoSubscribChangeCard()
                                {
                                    CardId = card_id,
                                    UserId = user_id,
                                    CasIds = String.Join(",", _packages.Select(p => p.CasId)),
                                    PackageNames = String.Join("+", _packages.Select(p => p.Name)),
                                    PackageIds = String.Join(",", Packages),
                                    Amount = _packages.Select(c => c.Amount).Sum(),
                                    SendDate = SendDate
                                };
                                _db.AutoSubscribChangeCards.Add(_card);
                            }
                        }
                        _db.SaveChanges();
                        tran.Commit();
                        return Json(1);
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json("შენახვა ვერ შესრულდა!");
                    }
                }
            }
        }

        [NonAction]
        public JsonResult NewAutoSubscrib(DateTime? SendDate, Card cur_card, Package package, int userid)
        {
            //    if (Cards == null || Cards.Count == 0)
            //        return Json("ბარათი არ არის არჩეული!");
            //    if (Packages == null || Packages.Count == 0)
            //        return Json("პაკეტი არ არის არჩეული!");

            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int package_days = int.Parse(_db.Params.First(c => c.Name == "PacketChangeTime").Value);
                        int user_id = userid;// ((User)Session["CurrentUser"]).Id;
                        //foreach (int card_id in Cards)
                        {
                            //CardDetailData cur_card = _db.Cards.Where(c => c.Id == card_id)
                            //    .Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                            //    {
                            //        Card = c,
                            //        CustomerType = c.Customer.Type,
                            //    }).FirstOrDefault();
                            if (cur_card != null)
                            {
                                //var _packages = _db.Packages.Where(p => Packages.Contains(p.Id)).Select(c => new { Name = c.Name, CasId = c.CasId, Amount = cur_card.CustomerType == CustomerType.Juridical ? c.Price : c.JuridPrice }).ToList();
                                //if (SendDate != null)
                                //{
                                //    Subscribtion updSubscrbs = _db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == cur_card.Id).OrderByDescending(s => s.Tdate).FirstOrDefault();
                                //    if (updSubscrbs != null)
                                //    {
                                //        if (_packages.Select(s => s.Amount).Sum() < updSubscrbs.Amount)
                                //        {
                                //            //if ((SendDate.Value - updSubscrbs.Tdate).Days <= package_days)
                                //            //{
                                //            //    tran.Rollback();
                                //            //    return Json(cur_card.Card.CardNum + " - ზე არჩეული ბოლო პაკეტიდან არ არის გასული " + package_days + " დღე!");
                                //            //}
                                //        }
                                //    }
                                //}

                                AutoSubscribChangeCard _card = new AutoSubscribChangeCard()
                                {
                                    CardId = cur_card.Id,
                                    UserId = user_id,
                                    CasIds = String.Join(",", package.CasId),
                                    PackageNames = String.Join("+", package.Name),
                                    PackageIds = String.Join(",", package.Id),
                                    Amount = package.Price,
                                    SendDate = SendDate
                                };
                                _db.AutoSubscribChangeCards.Add(_card);
                            }
                        }
                        _db.SaveChanges();
                        tran.Commit();
                        return Json(1);
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json("შენახვა ვერ შესრულდა!");
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteAutoSubscrib(int id)
        {
            using (DataContext _db = new DataContext())
            {
                AutoSubscribChangeCard _card = _db.AutoSubscribChangeCards.Where(c => c.Id == id).FirstOrDefault();
                if (_card != null)
                {
                    _db.AutoSubscribChangeCards.Remove(_card);
                    _db.Entry(_card).State = EntityState.Deleted;
                    _db.SaveChanges();
                    return Json(1);
                }

                return Json(0);
            }
        }
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
            //viewContext.ViewBag.isFromDiller = isFromDiller;

            // render the view to a HTML code
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }
        CardDetailData _card;
        public static string ImageConvert()
        {
            var FileName =Path.GetFileName("Logowhite.png");
            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Static/Images"), FileName);
            string imagepath = path;// "~/Static/Images/Logowhite.png";
            //string imagepath = @"C:\Users\tyupi\Source\Repos\DigitalTVBilling\DigitalTVBilling\Static\Images\Logowhite.png";
            FileStream fs = new FileStream(imagepath, FileMode.Open);
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            var base64 = Convert.ToBase64String(byData);
            var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
            return imgSrc;
        }
        public async Task<ActionResult> Invoices(int page = 1)
        {
            if (!Utils.Utils.GetPermission("INVOICES"))
            {
                return new RedirectResult("/Main");
            }

            //List<int> OneTicketMessed = new List<int>();
            //RandomMessed(OneTicketMessed);
            using (DataContext _db = new DataContext())
            {
                var __cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.CardStatus != CardStatus.Canceled && c.CardStatus == CardStatus.Closed && c.Customer.Type == CustomerType.Physical).ToList();
                __cards = __cards.Where(c => c.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Where(s => s.Package.Id!= 304070 && s.Package.Id!= 303242).ToList().Count > 0).ToList();
                List<Customer> customers = new List<Customer>();
                var count = 0;
                //CASSocket _socket = new CASSocket() { IP = "192.168.4.143", Port = 8000 };
                //_socket.Connect();
                List<BalanceAmounList> blAmount = new List<BalanceAmounList>();
                foreach (var item in __cards)
                {

                    CardDetailData _card = _db.Cards.Where(c => c.Id == item.Id).Select(c => new CardDetailData
                    {
                        PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                        ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        Card = c,
                        CustomerType = c.Customer.Type,
                        IsBudget = c.Customer.IsBudget,
                        SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                        MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                        CardLogs = c.CardLogs.ToList()
                    }).FirstOrDefault();

                    if (_card != null)
                    {

                        decimal balance = Math.Round(_card.PaymentAmount - _card.ChargeAmount, 2);
                        decimal amount = (decimal)_card.SubscribAmount;
                        if (amount == 0)
                            return null;

                        if (balance > Convert.ToDecimal(0.5) && balance < amount && amount!=12)
                        {
                            BalanceAmounList b_l = new BalanceAmounList();
                            b_l.id = item.Id;
                            b_l.abonentNum = item.AbonentNum;
                            b_l.abonentName = item.Customer.Name +" " + item.Customer.LastName;
                            b_l.balance = balance;
                            b_l.phone = item.Customer.Phone1;
                            //b_l.packname = _card.SubscriptionPackages.Select(s => s.Package.Name).FirstOrDefault();
                            b_l.packamount = (decimal)_card.SubscribAmount;
                            count++;
                            blAmount.Add(b_l);
                        }



                    }
                }
                //using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString))
                //{
                //    var dbcard = db.Query<Card>("Select * From Cards; Select * From Custumers;").ToList();
                //}
                //var Invoce_Code = _db.InvoiceLoggings.Select(s => s.invoce_code).ToList();
                //if (Invoce_Code != null) // განმეორებადი კოდის გამორიცხვა
                //{
                //    OneTicketMessed = OneTicketMessed.Except(Invoce_Code).ToList();
                //}
                //string _Image = ImageConvert();
                DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
                //DateTime dateFrom = new DateTime(DateTime.Now.Year, 01, 1, 0, 0, 0);
                //DateTime dateTo = new DateTime(DateTime.Now.Year, 01, 31, 0, 0, 0);
                ViewBag.AbonentBalance = blAmount;
                ViewBag.FilePath = _db.Params.Where(p => p.Name == "FTPHost").Select(p => p.Value).First() + "invoce/";
                // var _CustumerI = _db.Customers.Include("Cards").Include("CustomerSellAttachments").Where(c => c.Type == CustomerType.Juridical).ToList();
                //// DateTime td = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now, 0, 0, 0);
                // InvoicesList _invoce = new InvoicesList();
                // CardInfo _info;

                // foreach (var item in _CustumerI)
                // {
                //     decimal balanceSum = 0; bool send_invoce = false;
                //     foreach (var card_item in item.Cards)
                //     {
                //         //if (_db.CardCharges.Where(c => c.CardId == card_item.Id && c.Tdate >= dateFrom && c.Tdate <= dateTo).FirstOrDefault() != null)
                //         //{
                //         //    balanceSum += _db.CardCharges.Where(c => c.CardId == card_item.Id && c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(s => s.Amount).Sum();
                //         //    send_invoce = true;
                //         //}
                //         _card = _db.Cards.Where(c => c.Id == card_item.Id).Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Select(c => new CardDetailData
                //         {
                //             PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                //             ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                //             Card = c,
                //             CustomerType = c.Customer.Type,
                //             IsBudget = c.Customer.IsBudget,
                //             SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                //             MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                //             CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId)
                //         }).FirstOrDefault();
                //         decimal balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                //         //_info = new CardInfo()
                //         //{
                //         //    //Subscribtions = await _db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                //         //    CardLogs = await _db.CardLogs.Include("User").Where(c => c.CardId == card_item.Id).Where(c => c.Date >= dateFrom && c.Date <= dateTo).ToListAsync(),
                //         //    Payments = await _db.Payments.Include("PayType").Where(c => c.CardId == card_item.Id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                //         //    OtherCharges = await _db.CardCharges.Where(c => c.CardId == card_item.Id).Where(c => c.Status != CardChargeStatus.Daily).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToListAsync(),
                //         //    Balances = await _db.CardCharges.Where(c => c.CardId == card_item.Id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(c => new Balance()
                //         //    {
                //         //        Tdate = c.Tdate,
                //         //        OutAmount = c.Amount,
                //         //        InAmount = 0,
                //         //        OutAmountStatus = c.Status,
                //         //        CurrentBalance = (c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                //         //    })
                //         //    .Concat(_db.Payments.Where(p => p.CardId == card_item.Id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(p => new Balance()
                //         //    {
                //         //        Tdate = p.Tdate,
                //         //        OutAmount = 0,
                //         //        InAmount = p.Amount,
                //         //        OutAmountStatus = CardChargeStatus.Daily,
                //         //        CurrentBalance = (p.Card.Payments.Where(s => s.Tdate <= p.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (p.Card.CardCharges.Where(s => s.Tdate <= p.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                //         //    })).OrderBy(c => c.Tdate).ToListAsync(),
                //         //    CardServices = await _db.CardServices.Include("Service").Where(c => c.CardId == card_item.Id).Where(c => c.Date >= dateFrom && c.Date <= dateTo).Select(c => new CardServicesList
                //         //    {
                //         //        Name = c.Service.Name,
                //         //        PayType = c.PayType,
                //         //        Price = c.Amount,
                //         //        Date = c.Date
                //         //    }).ToListAsync()
                //         //};
                //         if (/*_info.Balances.Select(s => s.CurrentBalance).LastOrDefault()*/ balance < 0)
                //             balanceSum += balance;//_info.Balances.Select(s => s.CurrentBalance).LastOrDefault();
                //     }
                //     if (balanceSum < 0)
                //     {
                //         var ID = item.Cards.Select(s => s.Id).FirstOrDefault();

                //         //var pack = _card.Card.Subscribtions.Select(s => s).ToList();//_db.Subscribtions.Include("SubscriptionPackages").Where(c => c.CardId == ID).Select(ss => ss).ToList();
                //         //var card_packages = pack.Where(c => c.CardId == ID).SelectMany(s => s.SubscriptionPackages).ToList();
                //         JuridicalInvoicesList juridical_invoice = new JuridicalInvoicesList()
                //         {
                //             Name = item.Name,
                //             dateFrom = dateFrom,//new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0),
                //             dateTo = dateTo,//,
                //             _attachment = item.CustomerSellAttachments.Select(s => s).ToList(),
                //             balance = Math.Abs(balanceSum),
                //             Count = item.Cards.Count(),
                //             Invoices_Code = item.Code,
                //             PackagesPrice = _card.SubscribAmount/*pack.Select(ss => ss.Amount).FirstOrDefault()*/,
                //             Ramdom_Generator = OneTicketMessed[0],
                //             Image = _Image,
                //             Phone = item.Phone1

                //         };
                //         ViewBag.SellAttachments = _db.SellAttachments.ToList();
                //         var result = AutoInvoice(juridical_invoice);
                //         bool send_email = SendMail(item.Email, juridical_invoice.Name, _db.Params.Where(c => c.Name == "SystemEmail").Select(s => s.Name).FirstOrDefault(), _db.Params.Where(c => c.Name == "SystemEmailPassword").Select(s => s.Name).FirstOrDefault());
                //         _db.Params.Where(c => c.Name == "SystemEmail").Select(s => s.Name).FirstOrDefault();
                //         var _invoice = new InvoceLogging
                //         {
                //             tdate = DateTime.Now,
                //             custumer_id = item.Id,
                //             name = item.Name,
                //             invoce_code = OneTicketMessed[0],
                //             send_email = true,
                //             send_sms = true
                //         };
                //         _db.InvoiceLoggings.Add(_invoice);
                //         _db.SaveChanges();

                //         //  JuridicalReportInvoices(_invoice);
                //         OneTicketMessed.Remove(OneTicketMessed[0]);
                //         //return new Rotativa.MVC.ViewAsPdf("JuridicalInvoce", juridical_invoice) { FileName = "Invoice.pdf" }; ;
                //         //var actionPDF = new Rotativa.MVC.ViewAsPdf("JuridicalInvoce",juridical_invoice) { FileName = "Invoice.pdf" };
                //         //return new Rotativa.MVC.ActionAsPdf("JuridicalInvoce");
                //     }
                // }

                // //JuridicalReportInvoices(ReportListInvoice);
                return View(await _db.Invoices.Where(p => p.Tdate >= dateFrom && p.Tdate <= dateTo).Select(c => new InvoicesList
                {
                    Id = c.Id,
                    AbonentName = c.Customer.Name + " " + c.Customer.LastName,
                    AbonentNum = c.AbonentNums,
                    Tdate = c.Tdate,
                    FileName = c.FileName,
                    Num = c.Num
                }).OrderByDescending(c => c.Tdate).ToPagedListAsync(page, 50));
                //return null;
                //ViewBag.Custumer = _CustumerI;

                //ViewBag.SellAttachments = _db.SellAttachments.ToList();


                //return View(_CustumerI);
            }
        }
        [HttpPost] 
        public JsonResult AbonentActive(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                string[] address = _db.Params.Where(p => p.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();
                List<Param> _params = _db.Params.ToList();
                decimal jurid_limit_months = int.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);

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
                //var card = _db.Cards.Where(c => c.Id == card_id).FirstOrDefault();
                
                _card.Card.CardStatus = CardStatus.Active;
                Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.CloseDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                {
                    return Json(0);
                    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                }
                _socket.Disconnect();
            }
            return Json("აბონენტი გაქტიურდა!");
        }
        public ActionResult GeneratePDF()
        {
            return new Rotativa.MVC.ActionAsPdf("JuridicalInvoce");
        }
        public class UploadFile
        {
            public HttpPostedFileWrapper typess { get; set; }
        }
        public bool SendMail(string user_email, string FileName,string SystemEmail, string SystemEmailPassword)
        {
            try
            {
                string email = "tarasamedzmariashvili@gmail.com";
                string password = "tyupi123";
                var FolderName = (@"C:\tato\" + DateTime.Now.ToString("dd -MM-yyyy"));
                var loginInfo = new NetworkCredential(email, password);
                var msg = new MailMessage();
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                msg.From = new MailAddress(email);
                msg.To.Add(new MailAddress("tarasamedzmariashvili@gmail.com")); //user_email
                msg.Subject = "DigitalTv";
                msg.Body = "DigitalTv";

                msg.Attachments.Add(new System.Net.Mail.Attachment(FolderName + '\\' + SplitAddresName(FileName)));
                msg.IsBodyHtml = true;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;
                smtpClient.Send(msg);
            }
            catch (Exception ex)
            {
                return false;
                string message = ex.Message;
            }
            return true;
        }

        Random Rad = new Random(); // shemtxveviti ricxvis generatori
        public void RandomMessed(List<int> List_Random)
        {
            List<int> sawyisi = new List<int>();
            sawyisi = Enumerable.Range(1000000, 3999999).OrderBy(x => Rad.Next()).ToList();
            List_Random.AddRange(sawyisi);

        }
        [ValidateInput(false)]
        public ActionResult AutoInvoice(JuridicalInvoicesList SendInvoice)
        {
            string DocName = "JuridicalInvoce";

            var path = System.IO.Path.GetTempPath();
            var fileName = DocName + ".pdf";
            var var = System.IO.Path.Combine(path, fileName);


            bool isFromDiller = false;

            string htmlString = RenderViewAsString(DocName, SendInvoice, isFromDiller);

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

            int webPageHeight = 0;

            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();


            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            var FolderName = (@"C:\tato\" + DateTime.Now.ToString("dd -MM-yyyy"));
            if (!Directory.Exists(FolderName))

            {

                Directory.CreateDirectory(FolderName);

            }
            doc.Save(FolderName + '\\' + SplitAddresName(SendInvoice.Name));
            return null;
        }
        public string SplitAddresName(string Name)
        {
            string FileNameArray = Name + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf";
            string[] SplitFileName = FileNameArray.Split('\"');
            string FileName = "";
            foreach (var item in SplitFileName)
            {
                FileName = FileName + item;
            }
            return FileName;
        }
        //[NonAction]
        //public ActionResult InvoiceJuridical()
        //{
        //    return
        //}

        [HttpPost]
        public async Task<JsonResult> FilterInvoices(string letter, string column, int page)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            string where = column + " LIKE N'" + letter + "%'";

            System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

            using (DataContext _db = new DataContext())
            {
                int count = await _db.Database.SqlQuery<int>(@"SELECT COUNT(i.id) FROM book.Invoices AS i
                      INNER JOIN book.Customers AS c ON c.id=i.customer_id WHERE i.tdate BETWEEN @date_from AND @date_to AND " + where,
                                                                                    new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).FirstOrDefaultAsync();

                string sql = @"SELECT i.id AS Id,i.num AS Num,i.tdate AS Tdate,c.name AS AbonentName,i.abonent_nums AS AbonentNum,i.file_name AS FileName FROM book.Invoices AS i
                      INNER JOIN book.Customers AS c ON c.id=i.customer_id WHERE i.tdate BETWEEN @date_from AND @date_to AND " + where;

                var findList = await _db.Database.SqlQuery<InvoicesList>(sql, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToRawPagedListAsync(count, page, 20);
                return Json(new
                {
                    Invoices = findList,
                    Paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, findList, p => p.ToString(), PagedListRenderOptions.PageNumbersOnly).ToHtmlString(),
                    FilePath = _db.Params.Where(p => p.Name == "FTPHost").Select(p => p.Value).First() + "invoce/"
                });
            }

        }

        public PartialViewResult NewInvoice()
        {
            return PartialView("~/Views/Utils/_NewInvoice.cshtml", new InvoicePost() { Months = 1 });
        }

        //[HttpPost]
        //public JsonResult NewInvoice()
        //{
        //    if (invoice.Cards != null)
        //    {
        //        using (DataContext _db = new DataContext())
        //        {
        //            int c_id = invoice.Cards[0];
        //            InvoicePrint _invoicePrint = _db.Cards.Where(c => c.Id == c_id).Select(c => new InvoicePrint
        //            {
        //                AbonentName = c.Customer.Name,
        //                AbonentCode = c.Customer.Code,
        //                AbonentAddress = c.Customer.City + ", " + c.Customer.Address,
        //                AbonentPhone = c.Customer.Phone1,
        //                AbonentEmail = c.Customer.Email,
        //                AbonentId = c.CustomerId
        //            }).FirstOrDefault();

        //            _invoicePrint.StartDate = DateTime.Now;
        //            _invoicePrint.EndDate = DateTime.Now.AddMonths(invoice.Months);
        //            var p_i = _db.Cards.Where(c => invoice.Cards.Contains(c.Id)).Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new
        //            {
        //                Amount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
        //                Name = c.AbonentNum,
        //            }).ToList();

        //            _invoicePrint.Items = new List<InvoicePrintItem>();
        //            _invoicePrint.Items.Add(new InvoicePrintItem { Amount = p_i.Sum(c => c.Amount), Name = "სააბონენტო გადასახადი - " + String.Join(",", p_i.Select(c => c.Name)) });
        //            _invoicePrint.Items.Add(new InvoicePrintItem { Amount = (double)(_db.CardServices.Where(c => invoice.Cards.Contains(c.CardId)).Where(c => c.IsActive).Sum(c => (decimal?)c.Amount) ?? 0), Name = "გაწეული მომსახურება" });

        //            Invoice last_inv = _db.Invoices.OrderByDescending(c => c.Id).FirstOrDefault();
        //            _invoicePrint.Num = last_inv == null ? Utils.Utils.GetInvoiceNum("I0", DateTime.Now.Year) : Utils.Utils.GetInvoiceNum(last_inv.Num, last_inv.Tdate.Year);

        //            var _params = _db.Params.ToList();
        //            string file_name = new InvoicePdf() { InvoiceData = _invoicePrint }.Generate();
        //            if (file_name == string.Empty)
        //                return Json(0);

        //            string system_email = _params.First(p => p.Name == "SystemEmail").Value;
        //            string system_email_password = _params.First(p => p.Name == "SystemEmailPassword").Value;
        //            string invoice_text = _params.First(p => p.Name == "InvoiceText").Value;

        //            bool res = Utils.Utils.SendEmail(new List<string>() { file_name }, "GlobalTV ინვოისი", "mail." + system_email.Split('@')[1], 25, false, system_email,
        //                        system_email.Split('@')[0], system_email_password, new List<string>() { _invoicePrint.AbonentEmail }, new List<string>(), invoice_text);
        //            if (!res)
        //            {
        //                System.IO.File.Delete(file_name);
        //                return Json(0);
        //            }

        //            string file_short_name = file_name.Substring(file_name.LastIndexOf('\\') + 1);
        //            try
        //            {
        //                FileInfo fi = new System.IO.FileInfo(file_name);
        //                Stream fs = fi.OpenRead();
        //                Utils.Utils.UploadFile(fs, _params.First(p => p.Name == "FTPHost").Value + "invoce/", _params.First(p => p.Name == "FTPLogin").Value, _params.First(p => p.Name == "FTPPassword").Value, file_short_name);
        //                fs.Close();
        //            }
        //            catch (Exception ex)
        //            {
        //                return Json(0);
        //            }
        //            finally
        //            {
        //                System.IO.File.Delete(file_name);
        //            }


        //            Invoice _inv = new Invoice
        //            {
        //                CustomerId = _invoicePrint.AbonentId,
        //                Num = _invoicePrint.Num,
        //                Tdate = DateTime.Now,
        //                FileName = file_short_name,
        //                AbonentNums = String.Join(",", p_i.Select(c => c.Name))
        //            };
        //            _db.Invoices.Add(_inv);
        //            _db.SaveChanges();

        //            return Json(1);
        //        }
        //    }

        //    return Json(0);
        //}

    }
}