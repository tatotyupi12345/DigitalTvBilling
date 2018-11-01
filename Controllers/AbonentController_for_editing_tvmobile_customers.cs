using TVMobileBilling.Filters;
using TVMobileBilling.Models;
using TVMobileBilling.Utils;
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
using TVMobileBilling.ListModels;
using System.Data;
using System.Globalization;
using System.Data.SqlClient;
using TVMobileBilling.Helpers;
using System.Threading;
using System.ComponentModel;
using TVMobileBilling.Jobs;

namespace TVMobileBilling.Controllers
{
    [ValidateUserFilter]
    public class AbonentController : BaseController
    {
        private int pageSize = 20;
        public async Task<ActionResult> Index(int page = 1)
        {
            if (!Utils.Utils.GetPermission("ABONENT_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                string sql = @"SELECT TOP(" + pageSize + @") d.id AS Id,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.type AS Type,d.city AS City, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status, d.pack AS ActivePacket 
                         FROM (SELECT row_number() over(ORDER BY cr.id DESC) AS row_num,cr.tdate,c.id,c.name,c.lastname,c.code,c.[type],c.city,c.phone1,cr.abonent_num,cr.card_num, cr.status,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE cr.status !=5) AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);

                int count = await _db.Database.SqlQuery<int>(@"SELECT COUNT(cr.id) FROM book.Cards AS cr 
                        INNER JOIN book.Customers AS c ON c.id=cr.customer_id 
                        LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE cr.status !=4").FirstOrDefaultAsync();
                return View(await _db.Database.SqlQuery<AbonentList>(sql).ToRawPagedListAsync(count, page, pageSize));
                //return View(new AsyncRawQueryPagedList<AbonentList>());
            }
        }

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
                List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType< DescriptionAttribute>(c).Description }).ToList();
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

                Param param = _db.Params.Single(m => m.Name == "FreeDays");
                ViewBag.FreeDays = Convert.ToInt32(param.Value);
                return View(abonent);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult New(Abonent abonent)
        {

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

            if (ModelState.IsValid && Utils.Utils.GetPermission("ABONENT_ADD"))
            {

                //dreobiti regioni. es regioni wasashlelia
                #region droebiti   
                List<st_Customers> st_customers;
                using (TVMobileBillingEntities st_db = new TVMobileBillingEntities())
                {
                    st_customers = st_db.st_Customers.ToList();
                }
                #endregion
                //for (int b = 0; b < 303233; b++)
                //droebiti cikli
                foreach (st_Customers st_customer in st_customers)
                { 
                using (DataContext _db = new DataContext())
                {
                    using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                    {
                        try
                        {
                                //droebiti xazi. chasaxnelia
                            
                                //droebiti cikli. chasaxnelia
                            
                            { 
                                List<Param> _params = _db.Params.ToList();
                                int user_id = ((User)Session["CurrentUser"]).Id;

                                List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(abonent.Logging);

                                    //ar dagaviwdes komentaris moxsna
                                    //if (_db.Customers.Any(c => c.Code == abonent.Customer.Code))
                                    //{
                                    //    throw new Exception("აბონენტი " + abonent.Customer.Code + " კოდით უკვე არსებობს");
                                    //}


                                    //originali xazi. ar dagavfi9wydes komentaris chaxsna
                                    //abonent.Customer.SecurityCode = Utils.Utils.GetMd5(abonent.Customer.SecurityCode);

                                    //es regioni chasaxnelia
                                    #region droebiti regioni
                                    abonent.Customer.Name = st_customer.Firstname;
                                    abonent.Customer.LastName = st_customer.Lastname;
                                    abonent.Customer.Address = st_customer.Address;
                                    //abonent.Customer= st_customer.CardNo;
                                    if(st_customer.Phone.Length > 9)
                                        st_customer.Phone = st_customer.Phone.Remove(9);
                                    abonent.Customer.Phone1 = st_customer.Phone;
                                    //abonent.Customer.Name = st_customer.CarNo;
                                    //abonent.Customer.Name = st_customer.Balance;
                                    abonent.Customer.Code = st_customer.PersonalNo;
                                    abonent.Customer.SecurityCode = Utils.Utils.GetMd5(st_customer.Password);

                                    abonent.Customer.Cards = null;
                                    #endregion

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

                                string docnum = new WebService1().getDocNum();
                                Action<Card> cardDocNumAct = (Card card) =>
                                {
                                    card.DocNum = docnum;
                                };

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
                                       card.TowerId = 0;
                                       card.ClosedIsPen = false;
                                       card.AbonentNum = new WebService1().getAbonentNum();

                                       //droebiti kodi. chasxnelia
                                       card.Address = st_customer.CarNo;
                                       card.CardNum = st_customer.CardNo.ToString();

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
                                               subscrib.Amount = _packages.Select(p => abonent.Customer.Type == CustomerType.Juridical ? p.JuridPrice : p.Price).Sum();
                                               subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                               subscrib.Status = true;
                                               subscrib.Tdate = DateTime.Now;
                                               subscrib.UserId = user_id;
                                           }
                                       }
                                       else
                                       {
                                           var defaultPackages = _db.Packages.Where(p => p.IsDefault).ToList();
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
                                               var subscrib = card.Subscribtions.First();
                                               subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                           }
                                       }

                                       string charge_time = _params.Where(p => p.Name == "CardCharge").First().Value;
                                       if (abonent.Customer.Type == CustomerType.Juridical && abonent.Customer.IsBudget)
                                       {
                                           card.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(card.Tdate, charge_time, (decimal)card.Subscribtions.Where(s => s.Status).Sum(s => s.Amount), decimal.Parse(_params.Where(p => p.Name == "JuridLimitMonths").First().Value), card.Discount, free_days);
                                           if (!card.HasFreeDays)
                                           {
                                               card.CardStatus = CardStatus.Active;
                                               free_days = 0;

                                           //original code
                                           //decimal amount = (decimal)(card.Subscribtions.Sum(s=>s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                           int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                                               decimal amount = (decimal)(card.Subscribtions.Sum(s => s.Amount) / 1/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*// Utils.Utils.divide_card_charge_interval / 60);

                                               amount -= (amount * (decimal)card.Discount / 100);
                                               card.CardCharges = new List<CardCharge>() { new CardCharge() { Amount = amount, Tdate = card.CasDate, Status = CardChargeStatus.PreChange } };
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
                                           card.FinishDate = Utils.Utils.GenerateFinishDate(card.Tdate, charge_time).AddDays(free_days);
                                       }

                                   //JobSheduler.resCheduleTrigger("trigger_" + card.Id.ToString(), card.Id, 0, 0);
                                   //card.FinishDate = DateTime.Now.AddMinutes(2);// DateTime.SpecifyKind(DateTime.Now.AddMinutes(2), DateTimeKind.Utc);
                               };

                                abonent.Cards.ForEach(cardDocNumAct);

                                abonent.Cards.ForEach(cardAct);

                                _db.Cards.AddRange(abonent.Cards);
                                _db.SaveChanges();

                                //abonent.Cards.ForEach(x => JobSheduler.resCheduleTrigger("trigger_" + x.Id.ToString(), x.Id, 0, 0));

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
                                    _db.SaveChanges();

                                    if (c.CardStatus != CardStatus.Closed)
                                    {
                                        //SEND CAS DATA
                                        //if (!_socket.SendCardStatus(Convert.ToInt32(c.CardNum), true, DateTime.SpecifyKind(DateTime.Now.AddDays(3), DateTimeKind.Utc)))
                                        //{
                                        //    throw new Exception("ბარათი ვერ გააქტიურდა:" + c.CardNum);
                                        //}

                                        short[] cas_ids = { };
                                        if (c.Subscribtions != null && c.Subscribtions.Count > 0)
                                            cas_ids = c.Subscribtions.First().SubscriptionPackages.Select(p => (short)p.Package.CasId).ToArray();

                                        //droebiti komentari. chasaxnelia
                                        //if (!_socket.SendEntitlementRequest(Convert.ToInt32(c.CardNum), cas_ids, c.CloseDate.AddHours(-4), c.FinishDate.AddHours(-4), true))
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

                                tran.Commit();
                                MessageTemplate message = _db.MessageTemplates.Single(m => m.Name == "AbonentRegistration");

                                if (message != null)
                                {
                                    string abonentMessage = message.Desc;
                                    abonentMessage += System.Environment.NewLine + "xelshekrulebis N:";
                                    abonentMessage += " " + docnum + " ";

                                    abonentMessage += System.Environment.NewLine + "abonentis N:";
                                    foreach (Card card in abonent.Cards)
                                    {
                                        abonentMessage += " " + card.AbonentNum;
                                    }

                                    //droebiti komentari. chasxnelia
                                    //Task.Run(async () => { await Utils.Utils.sendMessage(abonent.Customer.Phone1, abonentMessage); }).Wait();
                                    //Utils.Utils.sendMessage(abonent.Customer.Phone1, message.Desc);
                                }

                            }
                            //ar dagaviwydes komentaris moxsna am xazze
                            //return new RedirectResult("/Abonent");
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
                ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r=>r.Name).ToList();
                List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType<DescriptionAttribute>(c).Description }).ToList();
                ViewBag.CardStatus = CardEnums;
                for (int i = 0; i < abonent.Cards.Count; i++)
                {
                    string max_num = _db.Cards.Select(cc => cc.AbonentNum).OrderByDescending(cc => cc).FirstOrDefault();
                    abonent.Cards[i].AbonentNum = Utils.Utils.GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1)));
                    abonent.Cards[i].Id = 0;
                }
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

                    int[] card_ids = ab.Cards.Select(cr => cr.Id).ToArray();
                    int[] non_canceled = ab.Cards.Where(cr => cr.CardStatus != CardStatus.Canceled).Select(cr => cr.Id).ToArray();
                    int[] canceled = ab.Cards.Where(cr => cr.CardStatus == CardStatus.Canceled).Select(cr => cr.Id).ToArray();
                    List<Payment> _payments = _db.Payments.Where(c => card_ids.Contains(c.CardId)).ToList();
                    List<CardCharge> _charges = _db.CardCharges.Where(c => card_ids.Contains(c.CardId)).ToList();

                    ab.AbonentDetailInfo = new AbonentDetailInfo
                    {
                        Balanse = Math.Round(_payments.Where(c => non_canceled.Contains(c.CardId)).Select(c => c.Amount).Sum() - _charges.Where(c => non_canceled.Contains(c.CardId)).Select(c => c.Amount).Sum(), 3),
                        FinishDate = ab.Cards.Min(c => c.FinishDate),
                        CanceledCardAmount = Math.Round(_payments.Where(c => canceled.Contains(c.CardId)).Select(c => c.Amount).Sum() - _charges.Where(c => canceled.Contains(c.CardId)).Select(c => c.Amount).Sum(), 3),
                        Chats = _db.CustomersChat.Where(c => c.CustomerId == id.Value).Select(c => new Chat { Tdate = c.Tdate, Message = c.MessageText, UserName = c.User.Name, Id = c.Id }).ToList()
                    };

                    ViewBag.Receivers = await _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToListAsync();
                    ViewBag.Towers = await _db.Towers.OrderBy(r=>r.Name).Select(r => new IdName { Id = r.Id, Name = r.Name }).ToListAsync();
                    ViewBag.Reasons = _db.Reasons.Where(c => c.ReasonType == ReasonType.Damage).Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();
                    List<Param> _params = await _db.Params.ToListAsync();

                    ViewBag.PacketChangeTime = int.Parse(_params.First(c=>c.Name == "PacketChangeTime").Value);
                    ViewBag.CurrentCard = cur_card;
                    ViewBag.Balances = ab.Cards.Select(c => new IdName { Id = c.Id, Name = Utils.Utils.GetBalance(_payments.Where(p => p.CardId == c.Id).Select(p => p.Amount).Sum(), _charges.Where(p => p.CardId == c.Id).Select(p => p.Amount).Sum()).ToString() }).ToList();
                    ViewBag.ServiceDays = int.Parse(_params.First(c => c.Name == "ServiceDays").Value);
                    ViewBag.HasReadonly = ((User)Session["CurrentUser"]).Type != 1 && DateTime.Now > ab.Customer.Tdate.AddMinutes(int.Parse(_params.First(p => p.Name == "AbonentEditTime").Value)) ? 1 : 0;
                }
            }

            return View(ab);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Abonent abonent, int? id, string cur_card)
        {
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
                if (id.HasValue)
                {
                    int user_id = ((User)Session["CurrentUser"]).Id;
                    using (DataContext _db = new DataContext())
                    {
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
                                        }
                                    };

                                    abonent.Cards.ForEach(cardDocNumAct);

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
                                                crd.UserId = user_id;
                                                crd.CardLogs = new List<CardLog>() { new CardLog() { Date = crd.Tdate, Status = CardLogStatus.Montage, UserId = user_id } };

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
                                                        subscrib.Amount = _packages.Select(p => abonent.Customer.Type == CustomerType.Juridical ? p.JuridPrice : p.Price).Sum();
                                                        subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                                        subscrib.Status = true;
                                                        subscrib.UserId = user_id;
                                                        subscrib.Tdate = DateTime.Now;

                                                        paket += "+" + String.Join("+", _packages.Select(p => p.Name));
                                                        ids = ids.Union(_packages.Select(p => (short)p.CasId)).ToArray();
                                                    }

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
                                                }
                                                else
                                                {
                                                    var defaultPackages = _db.Packages.Where(p => p.IsDefault).ToList();
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
                                                    if (!crd.HasFreeDays)
                                                    {
                                                        crd.CardStatus = CardStatus.Closed;
                                                        free_days = 0;
                                                        _cas.Status = crd.CardStatus;
                                                    }
                                                    crd.FinishDate = Utils.Utils.GenerateFinishDate(crd.Tdate, charge_time).AddDays(free_days);
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

                                                crd.LogTower = _db.Towers.FirstOrDefault(c => c.Id == crd.TowerId).Name;
                                                crd.LogReceiver = _db.Receivers.FirstOrDefault(c => c.Id == crd.ReceiverId).Name;
                                                this.AddLoging(_db,
                                                    LogType.Card,
                                                    LogMode.Add,
                                                    user_id,
                                                    crd.Id,
                                                    crd.AbonentNum,
                                                    Utils.Utils.GetAddedData(typeof(Card), crd)
                                                );

                                                _db.Cards.Add(crd);
                                                _CAS_data.Add(_cas);
                                            }
                                            else
                                            {
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
                                                    string old_card = _card.Card.CardNum;
                                                    if (Utils.Utils.GetPermission("CARD_EDIT"))
                                                    {
                                                        _card.Card.AbonentNum = crd.AbonentNum;
                                                        _card.Card.CardNum = crd.CardNum;
                                                        _card.Card.Address = crd.Address;
                                                        _card.Card.Discount = crd.Discount;
                                                        _card.Card.Group = crd.Group;
                                                        _card.Card.DocNum = crd.DocNum;
                                                        _card.Card.ReceiverId = crd.ReceiverId;
                                                        _card.Card.TowerId = crd.TowerId;
                                                        _card.Card.AutoInvoice = crd.AutoInvoice;
                                                        _card.Card.City = crd.City;
                                                        _card.Card.Village = crd.Village;
                                                        _card.Card.Region = crd.Region;
                                                        _card.Card.ClosedIsPen = crd.ClosedIsPen;
                                                        _card.Card.CardStatus = crd.CardStatus;
                                                        if (_card.Card.CardStatus == CardStatus.FreeDays)
                                                        {
                                                            _card.Card.Tdate = crd.Tdate;

                                                            string charge_time = _params.Where(p => p.Name == "CardCharge").First().Value;
                                                            if (abonent.Customer.Type == CustomerType.Juridical && abonent.Customer.IsBudget)
                                                                crd.FinishDate = Utils.Utils.GenerateJuridicalFinishDate(crd.Tdate, charge_time, (decimal)_card.SubscribAmount, decimal.Parse(_params.Where(p => p.Name == "JuridLimitMonths").First().Value), crd.Discount, free_days);
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
                                                                int[] arr = subscrib.SubscriptionPackages.Select(s => s.PackageId).ToArray();
                                                                var _packages = _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                                                                subscrib.Tdate = DateTime.Now;
                                                                subscrib.UserId = user_id;
                                                                subscrib.Amount = _packages.Select(p => abonent.Customer.Type == CustomerType.Juridical ? p.JuridPrice : p.Price).Sum();
                                                                subscrib.Amount -= subscrib.Amount * package_discount / 100;
                                                                subscrib.CardId = crd.Id;
                                                                subscrib.Status = true;

                                                                _db.Subscribtions.Add(subscrib);
                                                                _db.SaveChanges();

                                                                min_price += _packages.Sum(p => p.MinPrice);
                                                                paket += "+" + String.Join("+", _packages.Select(p => p.Name));
                                                                ids = ids.Union(_packages.Select(p => (short)p.CasId)).ToArray();
                                                            }

                                                            _cas.CasIds = ids;

                                                            if (((User)Session["CurrentUser"]).Type != 1)
                                                            {
                                                                if (_card.SubscribAmount > crd.Subscribtions.Sum(s => s.Amount)) //გადადის დაბალ პაკეტზე
                                                                {
                                                                    int package_days = int.Parse(_params.First(c => c.Name == "PacketChangeTime").Value);
                                                                    if ((DateTime.Now - updSubscrbs.Tdate).Days < package_days && _card.Card.CardStatus != CardStatus.Closed)
                                                                    {
                                                                        throw new Exception(_card.Card.CardNum + " - ზე არჩეული ბოლო პაკეტიდან არ არის გასული " + package_days + " დღე!");
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount) < (decimal)min_price && _card.Card.CardStatus != CardStatus.Closed)
                                                                    {
                                                                        throw new Exception(_card.Card.CardNum + " - ზე არჩეული მინიმალური ფასი აღემატება ბარათის ბალანსს!");
                                                                    }
                                                                    if(_card.Card.CardStatus != CardStatus.Closed)
                                                                    {
                                                                        //original code
                                                                        //decimal old_amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                                                                        decimal old_amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)/Utils.Utils.divide_card_charge_interval);

                                                                        old_amount -= (old_amount * (decimal)_card.Card.Discount / 100);

                                                                        //original code
                                                                        //decimal new_amount = (decimal)(crd.Subscribtions.Sum(s => s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                                                                        decimal new_amount = (decimal)(crd.Subscribtions.Sum(s => s.Amount) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)/Utils.Utils.divide_card_charge_interval);

                                                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = new_amount - old_amount, Tdate = DateTime.Now, Status = CardChargeStatus.PacketChange });
                                                                    }
                                                                }
                                                            }

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
                                                        }
                                                    }

                                                    _CAS_data.Add(_cas);
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

                                                    if (_card.Card.CardStatus != CardStatus.FreeDays && !(_card.Card.CardStatus == CardStatus.Active && _card.Card.Mode == 1))
                                                    {
                                                        Utils.Utils.SetFinishDate(_db, jurid_limit_months, crd.Id);
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
                                                if (!_socket.SendEntitlementRequestTemp(Convert.ToInt32(cas.CardNum), new short[] { 3, 4 }, new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), false))
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

                                                            decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)/Utils.Utils.divide_card_charge_interval);

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
                                                if (!_socket.SendEntitlementRequest(cas.CardNum, cas.DeactiveCasIds, DateTime.SpecifyKind(cas.Date, DateTimeKind.Utc), false))
                                                {
                                                    throw new Exception("ბარათის პაკეტები ვერ შაიშალა:" + cas.CardNum);
                                                }
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

                                                if (!_socket.SendEntitlementRequest(cas.CardNum, cas.CasIds, DateTime.SpecifyKind(cas.Date, DateTimeKind.Utc), true))
                                                {
                                                    throw new Exception("ბარათის პაკეტები ვერ გააქტიურდა:" + cas.CardNum);
                                                }
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
                return PartialView("~/Views/Abonent/_Card.cshtml", new Abonent() { Customer = new Customer(), Cards = new List<Card>() { new Card()} });
            }
        }

        public PartialViewResult AddSubscription(int id, CustomerType type)
        {
            ViewBag.CardId = id;
            using (DataContext _db = new DataContext())
            {
                ViewBag.Type = type;
                ViewBag.Discount = Convert.ToDouble(_db.Params.Where(p => p.Name == "PackageDiscount").Select(p => p.Value).First());
                return PartialView("~/Views/Abonent/_AddSubscription.cshtml", _db.Packages.ToList());
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> GetCardInfo(int card_id, int cust_id, bool detaled)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
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
                            InAmount = 0, 
                            OutAmountStatus = c.Status,
                            CurrentBalance = (c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                        })
                        .Concat(_db.Payments.Where(p => p.CardId == card_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(p => new Balance()
                        {
                            Tdate = p.Tdate,
                            OutAmount = 0,
                            InAmount = p.Amount,
                            OutAmountStatus = CardChargeStatus.Daily,
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
                            InAmount = 0, 
                            OutAmountStatus = c.Status, 
                            CardName = c.Card.AbonentNum,
                            CurrentBalance = (c.Card.Payments.Where(s => s.Tdate <= c.Tdate).Sum(s => (decimal?)s.Amount) ?? 0) - (c.Card.CardCharges.Where(s => s.Tdate <= c.Tdate).Select(s => (decimal?)s.Amount).Sum() ?? 0)
                        })
                        .Concat(_db.Payments.Where(c => c.Card.CustomerId == cust_id).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(p => new Balance() 
                        { 
                            Tdate = p.Tdate, 
                            OutAmount = 0, 
                            InAmount = p.Amount, 
                            OutAmountStatus = CardChargeStatus.Daily, 
                            CardName = p.Card.AbonentNum ,
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
        }

        [HttpPost]
        public ActionResult ExportCardInfo()
        {
            return Json("er");
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
                else if(type == "services")
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
            if (column == "cr.status" || column == "cr.tower_id")
                where = column + "=" + letter;
            else if (column == "c.lastname+c.name")
                where = column + " LIKE N'%" + letter + "%'";

            where = where.Replace("+", "+' '+");
            string sql = @"SELECT TOP(" + pageSize + @") d.id AS Id,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.type AS Type,d.city AS City, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status, d.pack AS ActivePacket 
                         FROM (SELECT row_number() over(ORDER BY c.id) AS row_num,c.id,c.name,c.lastname,c.code,c.[type],c.city,c.phone1,cr.abonent_num,cr.card_num, cr.status,
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
        public JsonResult CardCancel(int card_id, int mode)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        Card _card = _db.Cards.Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.Id == card_id).FirstOrDefault();
                         if (_card != null)
                         {
                             _card.CardStatus = CardStatus.Canceled;
                             _db.Entry(_card).State = EntityState.Modified;

                             CardLog _log = new CardLog() { CardId = _card.Id, Date = DateTime.Now, Status = CardLogStatus.Cancel, UserId = user_id };
                             _db.CardLogs.Add(_log);

                             this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Id,
                                 _card.CardNum + " ის გაუქმება",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათის გაუქმება", old_val = ""} }
                              );

                             if(mode == 2)
                             {
                                 CardDamage _damage = new CardDamage
                                 {
                                     CardId = card_id,
                                     Description = "",
                                     Tdate = DateTime.Now,
                                     UserId = user_id,
                                     UserGroupId = 1,
                                     Status = CardDamageStatus.Demontaged
                                 };
                                 _db.CardDamages.Add(_damage);
                             }

                             _db.SaveChanges();

                             string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                             CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                             _socket.Connect();
                             if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.CardNum), _card.Subscribtions.First(c => c.Status).SubscriptionPackages.Select(s => (short)s.Package.CasId).ToArray(), DateTime.SpecifyKind(_card.CasDate, DateTimeKind.Utc), false))
                             {
                                 throw new Exception();
                             }

                             _socket.Disconnect();
                         }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        return Json(false);
                    } 
                }
            }
            return Json(true);
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

                            decimal amount = (decimal)(_card.Subscribtions.FirstOrDefault(s => s.Status).Amount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)/Utils.Utils.divide_card_charge_interval);

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
        public JsonResult CardPause(int id, int day)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        var _card = _db.Cards.Where(c => c.Id == id).Select(c => new CardDetailData
                        {
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                            Card = c,
                        }).FirstOrDefault();

                        if (_card != null)
                        {
                            decimal pause_amount = decimal.Parse(_db.Params.First(p => p.Name == "CardPauseAmount").Value);

                            if (_card.Card.CardStatus == CardStatus.Active)
                            {
                                if (Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount) - pause_amount < 0)
                                    return Json("დაპაუზება ვერ მოხერხდა! ბალანსზე არასაკმარისი თანხაა.");
                            }

                            _card.Card.CardStatus = CardStatus.Paused;
                            _card.Card.PauseDays = day;
                            _card.Card.PauseDate = DateTime.Now;
                            _db.Entry(_card.Card).State = EntityState.Modified;

                            CardCharge _charge = new CardCharge()
                            {
                                CardId = id,
                                Tdate = DateTime.Now,
                                Amount = pause_amount,
                                Status = CardChargeStatus.Pause
                            };
                            _db.CardCharges.Add(_charge);

                            CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = DateTime.Now, Status = CardLogStatus.Pause, UserId = user_id };
                            _db.CardLogs.Add(_log);

                            this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Card.Id,
                                 _card.Card.AbonentNum + " - ბარათი დაპაუზდა",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაპაუზდა", old_val = "" } }
                              );

                            _card = _db.Cards.Where(c => c.Id == id).Select(c => new CardDetailData
                        {
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                            Card = c,
                        }).FirstOrDefault();

                            decimal jurid_limit_months = decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value);
                            Utils.Utils.SetFinishDate(_db, _card, jurid_limit_months, day);

                            string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), false, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            {
                                throw new Exception();
                            }
                            _socket.Disconnect();
                        }

                        tran.Commit();
                    }
                    catch
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
                        var _card = _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
                        {
                            PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                            ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                            Card = c,
                            MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                            CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId)
                        }).FirstOrDefault();
                        if (_card != null)
                        {
                            bool status_sign = (_card.PaymentAmount - _card.ChargeAmount) >= (decimal)_card.MinPrice;

                            _card.Card.PauseDays = 0;
                            _card.Card.CardStatus = status_sign ? CardStatus.Active : CardStatus.Closed;
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

                            Utils.Utils.SetFinishDate(_db, _card, decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value));

                            string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            {
                                throw new Exception();
                            }

                            if (status_sign)
                            {
                                if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false))
                                {
                                    throw new Exception();
                                }

                                Thread.Sleep(2000);
                                if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                {
                                    throw new Exception();
                                }
                            }

                            _socket.Disconnect();
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
                        Card _card = _db.Cards.Where(c => c.Id == card_id).FirstOrDefault();
                        if (_card != null)
                        {
                            CardLog _log = new CardLog() { CardId = _card.Id, Date = DateTime.Now, Status = CardLogStatus.Blocked, UserId = user_id, CardStatus = _card.CardStatus };
                            _db.CardLogs.Add(_log);

                            _card.CardStatus = CardStatus.Blocked;
                            _db.Entry(_card).State = EntityState.Modified;

                            this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Id,
                                 _card.AbonentNum + " - ბარათი დაიბლოკა",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათი დაიბლოკა", old_val = "" } }
                              );

                            string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.CardNum), false, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            {
                                throw new Exception();
                            }
                            _socket.Disconnect();
                        }

                        tran.Commit();
                    }
                    catch
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
                        Card _card = _db.Cards.Where(c => c.Id == card_id).FirstOrDefault();
                        if (_card != null)
                        {
                            CardLog last_log = _db.CardLogs.Where(c => c.CardId == _card.Id).OrderByDescending(c => c.Date).Skip(0).Take(1).FirstOrDefault();
                            if (last_log != null)
                            {
                                _card.CardStatus = last_log.CardStatus;
                                _db.Entry(_card).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                            if (_card.CardStatus != CardStatus.FreeDays)
                            {
                                Utils.Utils.SetFinishDate(_db, decimal.Parse(_db.Params.First(c => c.Name == "JuridLimitMonths").Value), _card.Id);
                            }

                            CardLog _log = new CardLog() { CardId = _card.Id, Date = DateTime.Now, Status = CardLogStatus.CloseBlock, UserId = user_id };
                            _db.CardLogs.Add(_log);

                            this.AddLoging(_db,
                                 LogType.Card,
                                 LogMode.CardDeal,
                                 user_id,
                                 _card.Id,
                                 _card.AbonentNum + " - ბარათის ბლოკის მოხსნა",
                                 new List<LoggingData>() { new LoggingData { field = "", new_val = "ბარათის ბლოკის მოხსნა", old_val = "" } }
                              );

                            string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                            _socket.Connect();
                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                            {
                                throw new Exception();
                            }
                            _socket.Disconnect();
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
                ViewBag.ToCards = new List<IdName>() { new IdName { Id = 0, Name = "გატანა" } }.Union(_cards.Where(c=>c.CardStatus != CardStatus.Canceled).Select(c => new IdName { Id = c.Id, Name = c.AbonentNum + " / " + c.CardNum }).ToList()).ToList();
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
                                return base.Json(false);
                            }
                        }

                        tran.Commit(); 
                        return Json(true);
                    }
                    catch
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

                int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;
                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                _socket.Connect();
                if (!_socket.SendOSDRequest(card_num, "Check", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                {
                    return Json(0);
                }
                _socket.Disconnect();
            }

            return Json(1);
        }

        [HttpPost]
        public JsonResult RefreshEntitlement(int card_num, int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                string[] address = _db.Params.Where(p => p.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                var card = _db.Cards.Where(c => c.Id == card_id).Where(c=>c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                {
                    Card = c,
                    CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId)
                }).FirstOrDefault();

                if (card != null)
                {
                    CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                    _socket.Connect();
                    if (!_socket.SendEntitlementRequestTemp(card_num, new short[] { 3, 4 }, new DateTime(2016, 01, 01, 0, 0, 0, DateTimeKind.Utc), false))
                    {
                        return Json(0);
                    }

                    SendTempCas(_db, _socket, card.Card.CardNum);

                    if (!_socket.SendEntitlementRequest(card_num, card.CasIds.ToArray(), DateTime.SpecifyKind(card.Card.CasDate, DateTimeKind.Utc), false))
                    {
                        return Json(0);
                    }

                    if (card.Card.CardStatus != CardStatus.Closed)
                    {
                        Thread.Sleep(2000);
                        if (!_socket.SendEntitlementRequest(card_num, card.CasIds.ToArray(), DateTime.SpecifyKind(card.Card.CasDate, DateTimeKind.Utc), true))
                        {
                            return Json(0);
                        }
                    }
                    _socket.Disconnect();
                    return Json(1);
                }
            }
            return Json(0);
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
                       new XElement("type", CoreHelper.GetCustomerTypeDesc(helper,c.Type)),
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

	}
}