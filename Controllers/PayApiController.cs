using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using DigitalTVBilling.ListModels;
using System.Globalization;
using DigitalTVBilling.Models;
using System.Web;
using System.IO;
using System.Threading;
using RazorEngine;
using System.Web.Routing;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Http.WebHost;
using System.Web.SessionState;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using Microsoft.AspNet.SignalR;
using DigitalTVBilling.CallCenter;
using System.Xml.Linq;
using DigitalTVBilling.CallCenter.Infrastructure;

namespace DigitalTVBilling.Controllers
{
    /// <summary>
    /// pay return 
    /// 1 - ok
    /// 2 - dublicate
    /// 3 - problem
    /// 
    /// check 
    /// 1- ok
    /// 2 - dublicate
    /// </summary>
    /// 

    [System.Web.Http.RoutePrefix("api/data")]
    public class PayApiController : ApiController
    {
        [System.Web.Http.Route("getcards")]
        [System.Web.Http.AcceptVerbs("POST")]
        [SkipMyGlobalActionFilter]
        public async Task<PayData> GetCards(PayCardFind request)
        {
            PayData _payData = new PayData() { PayDataAbonentInfo = new PayDataAbonentInfo(), Groups = new List<PayDataGroup>() };
            _payData.errorcode = 1;
            if (request.Secure == null || !Utils.Utils.IsRequestValid(request.Secure))
            {
                _payData.errorcode = 3;
                return _payData;
            }

            string value = request.Value.ToUpper();
            using (DataContext _db = new DataContext())
            {
                int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                int coeff = service_days;//DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                List<CardDetailData> _cards;
                double summinprice = 0;
                double sumbalance = 0;
                int count = 0;
                switch (request.Type)
                {
                    case CardFilterType.DocNum:
                        _cards = await _db.Cards
                            .Where(c => c.Customer.Cards.Any(cc => cc.DocNum == value))
                             .Include("Customer")
                           .Where(c => c.CardStatus != CardStatus.Canceled)
                            .Select(c => new CardDetailData
                            {
                                Card = c,
                                PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                                PackageNames = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(p => p.Package.Name).ToList(),
                                CustomerName = c.Customer.Name + " " + c.Customer.LastName
                            }).ToListAsync();
                        if (_cards != null && _cards.Count > 0)
                        {
                            _payData.PayDataAbonentInfo = new PayDataAbonentInfo { Name = _cards.First().CustomerName };
                            foreach (var data in _cards.GroupBy(c => c.Card.Group).Where(c=>c.Any(a=>a.Card.DocNum == value)))
                            {
                                _payData.Groups.Add(new PayDataGroup
                                {
                                    PayDataCards = data.Select(cc => new PayDataCard
                                    {
                                        Id = cc.Card.Id,
                                        AbonentNum = cc.Card.AbonentNum,
                                        Status = Utils.Utils.GetEnumDescription(cc.Card.CardStatus),
                                        Balance = Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                        StatusId = (int)cc.Card.CardStatus,
                                        FinishDate = cc.Card.FinishDate.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                                        Name = cc.Card.AbonentNum + " - " + cc.Card.Address,
                                        MinPrice = (cc.MinPrice - (cc.MinPrice * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                        Package = String.Join("+", cc.PackageNames.Select(c => c)),
                                        RecomendedPrice = (cc.SubscribAmount - (cc.SubscribAmount * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount)
                                    }).ToList()
                                });
                            }
                        }
                        break;

                    case CardFilterType.AbonentCode:

                        _cards = await _db.Cards.Where(c => c.Customer.Code == value)
                            .Where(c => c.CardStatus != CardStatus.Canceled)
                            .Select(c => new CardDetailData
                            {
                                Card = c,
                                PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                                PackageNames = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(p => p.Package.Name).ToList(),
                                CustomerName = c.Customer.Name + " " + c.Customer.LastName
                            }).ToListAsync();

                        if (_cards != null)
                        {
                            if (_cards.Count <= 0)
                            {

                            }
                            else
                            {
                                _payData.PayDataAbonentInfo = new PayDataAbonentInfo { Name = _cards.First().CustomerName };
                                foreach (var data in _cards.GroupBy(c => c.Card.Group))
                                {
                                    _payData.Groups.Add(new PayDataGroup
                                    {
                                        PayDataCards = data.Select(cc => new PayDataCard
                                        {
                                            Id = cc.Card.Id,
                                            AbonentNum = cc.Card.AbonentNum,
                                            Status = Utils.Utils.GetEnumDescription(cc.Card.CardStatus),
                                            Balance = Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                            StatusId = (int)cc.Card.CardStatus,
                                            FinishDate = cc.Card.FinishDate.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                                            Name = cc.Card.AbonentNum + " - " + cc.Card.Address,
                                            MinPrice = (cc.MinPrice - (cc.MinPrice * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                            Package = String.Join("+", cc.PackageNames.Select(c => c)),
                                            RecomendedPrice = (cc.SubscribAmount - (cc.SubscribAmount * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount)
                                        }).ToList()
                                    });

                                    //summinprice += data.Select(cc => (cc.MinPrice - (cc.MinPrice * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount)).FirstOrDefault();
                                    //sumbalance += data.Select(cc => (cc.MinPrice - (cc.MinPrice - (cc.MinPrice * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount))).FirstOrDefault();
                                }

                                foreach (var item in _payData.Groups)
                                {
                                    foreach (var cards in item.PayDataCards)
                                    {
                                        summinprice += cards.MinPrice;
                                        sumbalance += (double)cards.Balance;
                                    }
                                }

                                _payData.info = "dajgufebuli baratebis raodenoba: " + _cards.Count + ", minimaluri gadasaxdeli: " + summinprice + ", balansi jamshi: " + sumbalance;
                            }
                        }
                        break;

                    case CardFilterType.AbonentNum:

                        bool isgroup = false;
                        _cards = await _db.Cards
                             .Where(c => c.Customer.Cards.Any(cc => cc.AbonentNum == value))
                              .Include("Customer")
                            .Where(c => c.CardStatus != CardStatus.Canceled)
                             .Select(c => new CardDetailData
                             {
                                 Card = c,
                                 PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                                 ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                 MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                 SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                                 PackageNames = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(p => p.Package.Name).ToList(),
                                 CustomerName = c.Customer.Name + " " + c.Customer.LastName
                             }).ToListAsync();
                        int val = Convert.ToInt32(value);
                        if (_cards == null || _cards.Count <= 0)
                        {
                            isgroup = true;
                            _cards = await _db.Cards
                            .Where(c => c.Customer.Cards.Any(cc => cc.Group == val))
                             .Include("Customer")
                           .Where(c => c.CardStatus != CardStatus.Canceled)
                            .Select(c => new CardDetailData
                            {
                                Card = c,
                                PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                                PackageNames = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(p => p.Package.Name).ToList(),
                                CustomerName = c.Customer.Name + " " + c.Customer.LastName
                            }).ToListAsync();
                        }

                        if (_cards != null && _cards.Count > 0)
                        {
                            _payData.PayDataAbonentInfo = new PayDataAbonentInfo { Name = _cards.First().CustomerName };


                            if (!isgroup)
                            {
                                foreach (var data in _cards.GroupBy(c => c.Card.AbonentNum).Where(c => c.Any(a => a.Card.AbonentNum == value)))
                                {
                                    _payData.Groups.Add(new PayDataGroup
                                    {
                                        PayDataCards = data.Select(cc => new PayDataCard
                                        {
                                            Id = cc.Card.Id,
                                            AbonentNum = cc.Card.AbonentNum,
                                            Status = Utils.Utils.GetEnumDescription(cc.Card.CardStatus),
                                            Balance = Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                            RentMinPrice=decimal.Parse(_db.Params.First(p => p.Name == "Rent").Value),
                                            RentBalance = Utils.Utils.GetBalance(_db.Payments.Where(p => p.CardId == cc.Card.Id).Sum(p => (decimal?)p.PayRent) ?? 0, _db.CardCharges.Where(ca => ca.CardId == cc.Card.Id).Select(s => (decimal?)s.RentAmount).Sum() ?? 0),
                                            StatusId = (int)cc.Card.CardStatus,
                                            FinishDate = cc.Card.FinishDate.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                                            Name = cc.Card.AbonentNum + " <br> " + cc.Card.Address,
                                            MinPrice = (cc.MinPrice - (cc.MinPrice * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                            Package = String.Join("+", cc.PackageNames.Select(c => c)),
                                            RecomendedPrice = (cc.SubscribAmount - (cc.SubscribAmount * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount)
                                        }).ToList()
                                    });

                                }
                            }
                            else
                            {
                                foreach (var data in _cards.GroupBy(c => c.Card.Group).Where(c => c.Any(a => a.Card.Group == val)))
                                {
                                    _payData.Groups.Add(new PayDataGroup
                                    {
                                        PayDataCards = data.Select(cc => new PayDataCard
                                        {
                                            Id = cc.Card.Id,
                                            AbonentNum = cc.Card.AbonentNum,
                                            Status = Utils.Utils.GetEnumDescription(cc.Card.CardStatus),
                                            Balance = Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                            RentBalance = Utils.Utils.GetBalance(_db.Payments.Where(p => p.CardId == cc.Card.Id).Sum(p => (decimal?)p.PayRent) ?? 0, _db.CardCharges.Where(ca => ca.CardId == cc.Card.Id).Select(s => (decimal?)s.RentAmount).Sum() ?? 0),
                                            StatusId = (int)cc.Card.CardStatus,
                                            FinishDate = cc.Card.FinishDate.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                                            Name = cc.Card.AbonentNum + "<br>" + cc.Card.Address,
                                            MinPrice = (cc.MinPrice - (cc.MinPrice * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                            Package = String.Join("+", cc.PackageNames.Select(c => c)),
                                            RecomendedPrice = (cc.SubscribAmount - (cc.SubscribAmount * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount)
                                        }).ToList()
                                    });

                                }
                            }

                            foreach (var item in _payData.Groups)
                            {
                                foreach (var cards in item.PayDataCards)
                                {
                                    count++;
                                    summinprice += cards.MinPrice;
                                    sumbalance += (double)cards.Balance;
                                }
                            }

                            _payData.info = "დაჯგუფებული ბარათების რაოდენობა: " + count + ", მინიმალური გადასახდელი თანხა: " + summinprice + " ლარი, ბალანსი ჯამში: " + sumbalance + " ლარი.";
                        }
                        break;
                }
                return _payData;
            }
        }

        [System.Web.Http.Route("pay")]
        [System.Web.Http.AcceptVerbs("POST")]
        public int Pay(PaymentData request)
        {
            int userid = 1;
            if (request.Secure == null || !Utils.Utils.IsRequestValid(request.Secure))
                return 3;

            PaymentController _pay = new PaymentController();
            int res = _pay.SavePayment(request, userid, true);
            return res;
        }

        [System.Web.Http.Route("check")]
        [System.Web.Http.AcceptVerbs("POST")]
        public int Check(PaymentCheck request)
        {
            if (request.Secure == null || !Utils.Utils.IsRequestValid(request.Secure))
                return 3;

            using (DataContext _db = new DataContext())
            {
                if (_db.PayTransactions.Any(c => c.TransactionId == request.TransactionId))
                    return 2;
                else
                    return 1;
            }
        }

        [System.Web.Http.NonAction]
        public PayData getCardsInfo(string abonent_num)
        {
            List<CardDetailData> _cards;
            PayData _payData = new PayData() { PayDataAbonentInfo = new PayDataAbonentInfo(), Groups = new List<PayDataGroup>() };

            using (DataContext _db = new DataContext())
            {
                try
                {
                    _cards = _db.Cards
                                .Where(c => c.Customer.Cards.Any(cc => cc.AbonentNum == abonent_num))
                                    .Include("Customer")
                                .Where(c => c.CardStatus != CardStatus.Canceled)
                                .Select(c => new CardDetailData
                                {
                                    Card = c,
                                    PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                                    ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                    MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                    SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                                    PackageNames = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(p => p.Package.Name).ToList(),
                                    CustomerName = c.Customer.Name + " " + c.Customer.LastName
                                }).ToList();
                    if (_cards != null && _cards.Count > 0)
                    {
                        _payData.PayDataAbonentInfo = new PayDataAbonentInfo { Name = _cards.First().CustomerName };
                        foreach (var data in _cards.GroupBy(c => c.Card.Group).Where(c => c.Any(a => a.Card.AbonentNum == abonent_num)))
                        {
                            _payData.Groups.Add(new PayDataGroup
                            {
                                PayDataCards = data.Select(cc => new PayDataCard
                                {
                                    Id = cc.Card.Id,
                                    AbonentNum = cc.Card.AbonentNum,
                                    Status = Utils.Utils.GetEnumDescription(cc.Card.CardStatus),
                                    Balance = Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                    StatusId = (int)cc.Card.CardStatus,
                                    FinishDate = cc.Card.FinishDate.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                                    Name = cc.Card.AbonentNum + " - " + cc.Card.Address,
                                    MinPrice = (cc.MinPrice - (cc.MinPrice * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount),
                                    Package = String.Join("+", cc.PackageNames.Select(c => c)),
                                    RecomendedPrice = (cc.SubscribAmount - (cc.SubscribAmount * (double)cc.Card.Discount / 100)) - (double)Utils.Utils.GetBalance(cc.PaymentAmount, cc.ChargeAmount)
                                }).ToList()
                            });
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw;
                }
            }

            return _payData;
        }

        [System.Web.Http.Route("addCard")]
        [System.Web.Http.AcceptVerbs("GET")]
        public string addCard(int count)
        {
            //var req = Request.Content.ReadAsStringAsync();
            //var json = new StreamReader(req).ReadToEnd();
            //var result = JsonConvert.DeserializeObject<int>(json);

            AbonentController _ab = new AbonentController();
            var str = _ab.__AddCard(Convert.ToInt32(count));

            return str;
        }

        [System.Web.Http.Route("getSellAttachments")]
        [System.Web.Http.AcceptVerbs("GET")]
        public List<SellAttachment> getSellAttachments(int user_id)
        {
            //var req = Request.Content.ReadAsStringAsync();
            //var json = new StreamReader(req).ReadToEnd();
            //var result = JsonConvert.DeserializeObject<int>(json);

            AbonentController _ab = new AbonentController();
            var str = _ab.__getSellAttachments(user_id);

            return str;
        }

        [System.Web.Http.Route("getNewEntryEdit")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool getNewEntryEdit(DamageModel damage)
        {
            //var req = Request.Content.ReadAsStringAsync();
            //var json = new StreamReader(req).ReadToEnd();
            //var result = JsonConvert.DeserializeObject<int>(json);

            CustomerAttachController _Cu = new CustomerAttachController();
            JsonResult str = _Cu.getNewEntryEdit(damage);
            bool s = Convert.ToBoolean(str.Data);
            return s;
        }
        [System.Web.Http.Route("FinishReturnedCard")]
        [System.Web.Http.AcceptVerbs("POST")]
        public string FinishReturnedCard(SaveReturned returned)
        {
            //var req = Request.Content.ReadAsStringAsync();
            //var json = new StreamReader(req).ReadToEnd();
            //var result = JsonConvert.DeserializeObject<int>(json);

            AbonentController _abonent = new AbonentController();
            JsonResult str = _abonent.SaveReturnedCard(returned.return_attachment, returned.card_id, returned.cash, returned.amount, returned.comment, returned.reason, returned.bort_id, returned.pretentiu, returned.force, returned.commions_not, returned.date);
            string json_conver = str.Data.ToString().Substring(10,11);
            return str.Data.ToString();
        }
        public class StaticCount
        {
            public int order_count { get; set; }
            public int order_yielding { get; set; }
            public int order_remainder { get; set; }
            public int order_cancled { get; set; }

            public int damage_count { get; set; }
            public int damage_yielding { get; set; }
            public int damage_remainder { get; set; }

            public int cancel_count { get; set; }
            public int cancel_yielding { get; set; }
            public int cancel_remainder { get; set; }
            public int cancel_cancled { get; set; }

            public int start_end { get; set; }
            public string month { get; set; }
        }
        [System.Web.Http.Route("UserStatic")]
        [System.Web.Http.AcceptVerbs("GET")]
        public StaticCount UserStatic(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                string where = " AND u.id=" + user_id;
                StaticCount count = new StaticCount();
                DateTime datefrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);

                count.order_count = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "'").FirstOrDefault();
                count.order_yielding = _db.Database.SqlQuery<int>(@"select COUNT(c.id) from book.Cards as c 
								left join book.Customers as a on a.id = c.customer_id
								left join book.Users as u on c.user_id = u.id
								left join dbo.SellerObject as so on u.object = so.ID
								left join book.UserTypes as ut on u.type = ut.id where  ut.id = 4 " + where + " AND c.tdate BETWEEN @date_from AND @date_to", new SqlParameter("date_from", datefrom), new SqlParameter("date_to", DateTime.Now)).FirstOrDefault();
                count.order_remainder = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "' and status!=" + (int)OrderStatus.Closed + "and status!=" + (int)OrderStatus.Canceled + " and o.is_approved=0").FirstOrDefault();
                count.order_cancled = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "' and status=" + (int)OrderStatus.Canceled + "").FirstOrDefault();

                count.damage_count = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "'").FirstOrDefault();
                count.damage_yielding = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "' and status=" + (int)DamageStatus.Closed + "").FirstOrDefault();
                count.damage_remainder = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "' and status!=" + (int)DamageStatus.Closed + " and is_approved=1").FirstOrDefault();

                count.cancel_count = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "'").FirstOrDefault();
                count.cancel_yielding = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "' and status=" + (int)CancleStatus.Closed + "").FirstOrDefault();
                count.cancel_remainder = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "' and status!=" + (int)CancleStatus.Closed + "and status!=" + (int)CancleStatus.NotClosed + " and o.is_approved=0").FirstOrDefault();
                count.cancel_cancled = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy") + "' and status=" + (int)CancleStatus.NotClosed + "").FirstOrDefault();

                Month month = new Month();
                count.month =month.returnMonth( DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture));
                count.start_end = _db.Database.SqlQuery<int>($"SELECT s.start_end FROM book.Users s where s.id={user_id}").FirstOrDefault();
                return count;
            }

        }
        [System.Web.Http.Route("CountStaticPromo")]
        [System.Web.Http.AcceptVerbs("GET")]
        public StaticCount CountStaticPromo(int user_id, string dateFrom, string dateTo)
        {
            using (DataContext _db = new DataContext())
            {
                DateTime date_from, date_to;

                date_from = DateTime.Parse(dateFrom);
                date_to = DateTime.Parse(dateTo);

                string where = " AND u.id=" + user_id;
                StaticCount count = new StaticCount();
                count.order_count = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.user_id = " + user_id + " and tdate between '" + date_from.ToString("MM-dd-yyyy 00:01:ss") + "' and '" + date_to.ToString("MM-dd-yyyy 23:59:ss") + "'").FirstOrDefault();
                count.order_yielding= _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.user_id = " + user_id + " and change_date between '" + date_from.ToString("MM-dd-yyyy 00:01:ss") + "' and '" + date_to.ToString("MM-dd-yyyy 23:59:ss") + "' and status=" + (int)OrderStatus.Closed ).FirstOrDefault();
                count.order_remainder = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.user_id = " + user_id + " and tdate between '" + date_from.ToString("MM-dd-yyyy 00:01:ss") + "' and '" + date_to.ToString("MM-dd-yyyy 23:59:ss") + "' and status!=" + (int)OrderStatus.Closed + "and status!=" + (int)OrderStatus.Canceled + " and o.is_approved=0").FirstOrDefault();
                count.order_cancled = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.user_id = " + user_id + " and change_date between '" + date_from.ToString("MM-dd-yyyy 00:01:ss") + "' and '" + date_to.ToString("MM-dd-yyyy 23:59:ss") + "' and status=" + (int)OrderStatus.Canceled + "").FirstOrDefault();

                return count;
            }

        }
        [System.Web.Http.Route("returnImage")]
        [System.Web.Http.AcceptVerbs("GET")]
        public string returnImage(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                string image = _db.Database.SqlQuery<string>("SELECT u.image FROM book.Users u where id=" + user_id).FirstOrDefault();
                if (image == null)
                {
                    image = "tStyles/dist/img/user9-128x128.png";
                }

                return image;
            }

        }
        [System.Web.Http.Route("getOrders")]
        [System.Web.Http.AcceptVerbs("GET")]
        public List<Order> getOrders(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<Order> orders = new List<Order>();
                var user_id_type = _db.Database.SqlQuery<int>($"SELECT u.type FROM [book].[Users] u where u.id='{user_id}'").FirstOrDefault();
                if (user_id_type==43)
                {
                    orders = _db.Orders.OrderByDescending(o => o.Id).Where(c => c.UserId == user_id && c.Status!= OrderStatus.Canceled && c.Status!=OrderStatus.Closed).ToList(); //_db.Database.SqlQuery<Order>($"SELECT * FROM doc.Orders where user_id='{user_id}'").ToList();
                }
                else
                {
                    orders = _db.Orders.OrderByDescending(o => o.Id).Where(o => o.ExecutorID == user_id && o.IsApproved == false && o.Status != OrderStatus.Closed && o.Status != OrderStatus.Canceled && o.GetDate<=DateTime.Now).ToList();
                }
                return orders;
            }

        }
        [System.Web.Http.Route("OrderCancled")]
        [System.Web.Http.AcceptVerbs("GET")]
        public List<Order> OrderCancled(int user_id,string dateFrom,string dateTo)
        {
            dateFrom += " 00:01:00";
            dateTo += " 23:59:00";
            using (DataContext _db = new DataContext())
            {
                DateTime date_from, date_to;

                date_from= DateTime.Parse(dateFrom);
                date_to = DateTime.Parse(dateTo);
                return _db.Orders.OrderByDescending(o => o.Id).Where(c => c.ChangeDate >= date_from && c.ChangeDate <= date_to && c.UserId == user_id && c.Status == OrderStatus.Canceled).ToList(); ;
            }

        }
        [System.Web.Http.Route("getDamage")]
        [System.Web.Http.AcceptVerbs("GET")]
        public List<Damage> getDamage(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<Damage> damage = _db.Damages.OrderByDescending(o => o.Id).Where(o => o.ExecutorID == user_id && o.IsApproved == false && o.Status != DamageStatus.Closed).ToList();

                return damage;
            }

        }
        [System.Web.Http.Route("Cancellation")]
        [System.Web.Http.AcceptVerbs("GET")]
        public List<Cancellation> Cancellation(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<Cancellation> cancel = _db.Cancellations.OrderByDescending(o => o.Id).Where(o => o.ExecutorID == user_id && o.IsApproved == false && o.Status != CancleStatus.Closed && o.Status != CancleStatus.NotClosed).ToList();

                return cancel;
            }

        }
        [System.Web.Http.Route("_ReturnedCard")]
        [System.Web.Http.AcceptVerbs("GET")]
        public CancledCardReturned _ReturnedCard(string card_code)
        {
            using (DataContext _db = new DataContext())
            {
                using (var daper_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ToString()))
                {
                    double commision_ammount = 0;
                    string comm = _db.Params.Where(p => p.Name == "ReturnedCardCommision").FirstOrDefault().Value;
                    if (comm == null || comm == "")
                    {
                        throw new Exception("საკომისიოს რაოდენობა ვერ მოიძებნა!");
                    }
                    CancledCardReturned cancled = new CancledCardReturned();

                    //string str = "SELECT [id] FROM [DigitalTVBilling].[book].[Customers] where code=(" + card_code + @")";
                    //using (var multi = daper_db.QueryMultiple(str))
                    //{
                    //    var subscribes = multi.Read<IdName>().FirstOrDefault();

                    //}

                    //int custumer_id = _db.Customers.Where(c => c.Code == card_code).Select(s => s.Id).FirstOrDefault();
                    double.TryParse(comm, out commision_ammount);
                    cancled.commisionAmount = commision_ammount;
                    Card _card = _db.Cards.Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.CardNum == card_code).FirstOrDefault();
                    cancled.card_id = _card.Id;
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
                        cancled.cancel_date = __card.Card.Tdate;
                        decimal balance = 0;
                        if (__card.Card.Subscribtions.Count <= 2 && __card.SubscriptionPackages.Where(c => c.Package.Id == 304086).FirstOrDefault() != null)
                        {
                            cancled.balance = balance;
                            _db.Database.ExecuteSqlCommand($"UPDATE [doc].[Payments]  SET  [amount] = {__card.ChargeAmount} WHERE card_id ={_card.Id }");
                        }
                        else
                        {
                            balance = Utils.Utils.GetBalance(__card.PaymentAmount, __card.ChargeAmount);
                            cancled.balance = balance;
                        }
                        if (balance >= (decimal)commision_ammount)
                        {
                            cancled.amount = balance - (decimal)commision_ammount;
                        }
                        else
                        {
                            cancled.amount = 0;
                        }
                        if (__card.SubscriptionPackages.Where(c => c.Package.Id == 304086).FirstOrDefault() != null)
                        {
                            cancled.PackName = __card.SubscriptionPackages.Where(c => c.Package.Id == 304086).FirstOrDefault().Package.Name;
                        }
                        else
                        {
                            cancled.PackName = "";
                        }
                        cancled.temporary_use = _db.Database.SqlQuery<int>($"SELECT c.temporary_use FROM book.Customers AS c where c.id={_card.CustomerId}").FirstOrDefault();
                        //ViewBag.card_id = card_id;
                    }
                    //cancled.attachmenlist = _db.ReceiverAttachments.ToList();

                    // List<User> user_info = _db.Users.Include("UserType").Where(c => c.UserType.Name.Contains("დილერი") || c.UserType.Id == 1 || c.UserType.Id == 2 || c.UserType.Id == 4 || c.UserType.Id == 5).ToList();

                    return cancled;
                }
            }
        }
        [System.Web.Http.Route("ReceiverAttachment")]
        [System.Web.Http.AcceptVerbs("GET")]
        public List<ReceiverAttachment> ReceiverAttachment()
        {
            using (DataContext _db = new DataContext())
            {
                var xx = _db.ReceiverAttachments.ToList(); 

                return _db.ReceiverAttachments.ToList();
            }

        }
        [System.Web.Http.Route("getUser")]
        [System.Web.Http.AcceptVerbs("POST")]
        public int[] getUser(UserInfo userinfo)
        {
            int userid = 0, usertype = 0;
            userid = new PayController().CheckUsernameAndPassword(userinfo.username, Utils.Utils.GetMd5(userinfo.password));
            int[] retvals = new int[2];
            retvals[0] = userid;
            retvals[1] = 0;

            using (DataContext _db = new DataContext())
            {
                User user = _db.Users.Where(u => u.Id == userid).FirstOrDefault();
                if(user != null)
                    usertype = _db.Users.Where(u => u.Id == userid).FirstOrDefault().Type;
            }
            retvals[1] = usertype;
            //AbonentController _ab = new AbonentController();
            //_ab.NewAbonent(abonent);
            return retvals;
        }

        [System.Web.Http.Route("addAbonent")]
        [System.Web.Http.AcceptVerbs("POST")]
        public JsonResult addAbonent(Abonent abonent)
        {
            AbonentController _ab = new AbonentController();
            JsonResult status = _ab.NewAbonent(abonent);
            return status;
        }
        [System.Web.Http.Route("addOrder")]
        [System.Web.Http.AcceptVerbs("POST")]
        public JsonResult addOrder(Order order)
        {
            OrderController _ab = new OrderController();
            JsonResult status= _ab.NewOrder(order);
            return status;
        }
        [System.Web.Http.Route("OrderApprove")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool OrderApprove([FromBody]OrderApproveModel param)
        {
            OrderController order = new OrderController();
            bool status = order.__OrderApprove(param.order_id, param.user_id);
            return status;
        }
        [System.Web.Http.Route("OrderToGo")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool OrderToGo([FromBody]OrderDamageCancellation param)
        {
            using (DataContext _db=new DataContext())
            {
                switch (param.direction)
                {
                    case 0:

                        _db.Database.ExecuteSqlCommand($"UPDATE [doc].[Orders]  SET  [to_go] = 1 WHERE id ={param.order_id }");
                        _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[Damage]  SET  [to_go] =0 WHERE executor_id ={param.user_id }");
                        _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[Cancellation]  SET  [to_go] = 0 WHERE executor_id ={param.user_id }");
                        var context_order = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                        context_order.Clients.All.onHitRecorded("OrderToGo", param.user_id);
                        context_order.Clients.All.onHitRecorded("Region", 
                            new RegionGoName(
                                new SqlConnection(
                                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                  param.order_id ,
                                  "doc.Orders",
                                  param.user_id
                            ).Result(),
                             param.user_id
                            );
                        break;
                    case 1:
                        _db.Database.ExecuteSqlCommand($"UPDATE [doc].[Orders]  SET  [to_go] = 0 WHERE executor_id ={param.user_id }");
                        _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[Damage]  SET  [to_go] =1 WHERE id ={param.order_id }");
                        _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[Cancellation]  SET  [to_go] = 0 WHERE executor_id ={param.user_id }");
                        var context_damage = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                        context_damage.Clients.All.onHitRecorded("DamageToGo", param.user_id);
                        context_damage.Clients.All.onHitRecorded("Region",
                                new RegionGoName(
                                    new SqlConnection(
                                                ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                      param.order_id,
                                      "dbo.Damage",
                                      param.user_id
                                ).Result(),
                                 param.user_id
                                );
                        break;
                    case 2:
                        _db.Database.ExecuteSqlCommand($"UPDATE [doc].[Orders]  SET  [to_go] = 0 WHERE executor_id ={param.user_id }");
                        _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[Damage]  SET  [to_go] =0 WHERE executor_id ={param.user_id }");
                        _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[Cancellation]  SET  [to_go] = 1 WHERE id ={param.order_id }");
                        var context_cancellation = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                        context_cancellation.Clients.All.onHitRecorded("CancellationToGo", param.user_id);
                        context_cancellation.Clients.All.onHitRecorded("Region",
                                new RegionGoName(
                                    new SqlConnection(
                                                ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                      param.order_id,
                                      "dbo.Cancellation",
                                      param.user_id
                                ).Result(),
                                 param.user_id
                                );
                        break;
                }
            }
                return true;
        }
        [System.Web.Http.Route("DamageApprove")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool DamageApprove([FromBody]OrderApproveModel param)
        {
            DamageController order = new DamageController();
            bool status = order.__DamageApprove(param.order_id, param.user_id);
            return status;
        }

        [System.Web.Http.Route("CancledApprove")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool CancledApprove([FromBody]OrderApproveModel param)
        {
            CancellationController cancellation = new CancellationController();
            bool status = cancellation.__CancledApprove(param.order_id, param.user_id);
            return status;
        }
        [System.Web.Http.Route("NotApprove")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool NotApprove([FromBody]OrderApproveModel param)
        {
            CancellationController cancellation = new CancellationController();
            bool status = cancellation.__NotApprove(param.order_id, param.user_id);
            return status;
        }
        [System.Web.Http.Route("DamageComment")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool DamageComment([FromBody]DamageComments param)
        {
            DamageController damage = new DamageController();
            bool status = damage.DamageComment(param.damage_id, param.user_id, param.comment);
            return status;
        }

        [System.Web.Http.Route("DamageWriteComment")]
        [System.Web.Http.AcceptVerbs("POST")]
        public string DamageWriteComment([FromBody] writeComment param)
        {
            DamageController damage = new DamageController();
            string comment = damage.DamageWriteComment(param.id);
            return comment;
        }
        [System.Web.Http.Route("OrderComment")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool OrderComment([FromBody]DamageComments param)
        {
            OrderController order = new OrderController();
            bool status = order.OrderComment(param.damage_id, param.user_id, param.comment);
            return status;
        }
        [System.Web.Http.Route("OrderCancelStatic")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool OrderCancelStatic([FromBody]DamageComments param)
        {
            OrderController order = new OrderController();
            bool status = order.OrderCancelStatic(param.damage_id, param.user_id, Convert.ToInt32(param.comment));
            return status;
        }
        [System.Web.Http.Route("DamageCancelStatic")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool DamageCancelStatic([FromBody]DamageComments param)
        {
            DamageController damage = new DamageController();
            bool status = damage.DamageCancelStatic(param.damage_id, param.user_id, param.comment);
            return status;
        }
        [System.Web.Http.Route("OrderWriteComment")]
        [System.Web.Http.AcceptVerbs("POST")]
        public string OrderWriteComment([FromBody] writeComment param)
        {
            OrderController order = new OrderController();
            string comment = order.OrderWriteComment(param.id);
            return comment;
        }
        [System.Web.Http.Route("CancellationComment")]
        [System.Web.Http.AcceptVerbs("POST")]
        public bool CancellationComment([FromBody]DamageComments param)
        {
            CancellationController cancellations = new CancellationController();
            bool status = cancellations.CancellationComment(param.damage_id, param.user_id, param.comment);
            return status;
        }

        [System.Web.Http.Route("CancellationWriteComment")]
        [System.Web.Http.AcceptVerbs("POST")]
        public string CancellationWriteComment([FromBody] writeComment param)
        {
            CancellationController cancellations = new CancellationController();
            string comment = cancellations.CancellationWriteComment(param.id);
            return comment;
        }
        [System.Web.Http.Route("UserStartEnd")]
        [System.Web.Http.AcceptVerbs("GET")]
        public bool UserStartEnd(int start_end,int user_id)
        {
            using (DataContext _db=new DataContext())
            {
                _db.Database.ExecuteSqlCommand($"UPDATE [book].[Users]   SET  [start_end] = {start_end} WHERE id={user_id}");
                if (start_end == 0)
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>();
                    context.Clients.All.onHitRecorded("UserEnd", user_id);
                }
                else
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>();
                    context.Clients.All.onHitRecorded("UserStart", user_id);
                }
            }
            return true;
        }


        //[System.Web.Http.Route("newAbonent")]
        //[System.Web.Http.AcceptVerbs("POST")]
        //public int newAbonent(Abonent abonent)
        //{
        //    AbonentController _ab = new AbonentController();
        //    int status = _ab.NewAbonent(abonent);
        //    return status;
        //}

        [System.Web.Http.Route("addSubscribtion")]
        [System.Web.Http.AcceptVerbs("GET")]
        public string addSubscribtion(int id, CustomerType type, int user_id)
        {
            AbonentController _ab = new AbonentController();
            var str = _ab.__AddSubscribtion(id, type, user_id);

            return str;
        }

        [System.Web.Http.Route("getReceivers")]
        [System.Web.Http.AcceptVerbs("POST")]
        public List<IdName> getReceivers()
        {
            List<IdName> rec;
            Dictionary<int, string> dic = new Dictionary<int, string>();
            using (DataContext _db = new DataContext())
            {
                rec = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
            }
            return rec;
        }

        [System.Web.Http.Route("getTowers")]
        [System.Web.Http.AcceptVerbs("POST")]
        public List<IdName> towers()
        {
            List<IdName> rec;
            //Dictionary<int, string> dic = new Dictionary<int, string>();
            using (DataContext _db = new DataContext())
            {
                rec = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
            }
            return rec;
        }
        [System.Web.Http.Route("towersMux")]
        [System.Web.Http.AcceptVerbs("GET")]
        public Tower towersMux(int id)
        {
            //var req = Request.Content.ReadAsStringAsync();
            //var json = new StreamReader(req).ReadToEnd();
            //var result = JsonConvert.DeserializeObject<int>(json);
            //Dictionary<int, string> dic = new Dictionary<int, string>();
            Tower mux_tower = new Tower();
            using (DataContext _db = new DataContext())
            {
                string where = "where id=" + id;
                //mux_tower = _db.Towers.Where(c => c.Id == id).Select(s => s).FirstOrDefault();
                mux_tower = _db.Database.SqlQuery<Tower>(@"SELECT * FROM book.Towers " + where).FirstOrDefault();
            }
            return mux_tower;
        }

        [System.Web.Http.Route("getDocument")]
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage getDocument(int card_id, int user_id)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            AbonentController _ab = new AbonentController();
            var str = _ab.__getDocument(card_id, user_id, info);

            string htmlString = str;
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
            /*FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName ="test.pdf";
            return fileResult;*/

            string docname = "", cardnum = "";
            info.TryGetValue("DocName", out docname);
            info.TryGetValue("CardNum", out cardnum);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(pdf.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = Uri.EscapeDataString(docname + "(" + cardnum + ").pdf")
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }

    }


    public class MyHttpControllerHandler
  : HttpControllerHandler, IRequiresSessionState
    {
        public MyHttpControllerHandler(RouteData routeData) : base(routeData)
        { }
    }

    public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MyHttpControllerHandler(requestContext.RouteData);
        }
    }
    public class location
    {
        public int positionL { get; set; }
        public string name { get; set; }
    }
    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class SkipMyGlobalActionFilterAttribute : Attribute
    {
    }

    public class MyGlobalActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(RequireHttpsAttribute), false).Any())
            {
                return;
            }

            // here do whatever you were intending to do
        }
    }
    public class writeComment
    {
        public int id { get; set; }
    }
    public class OrderApproveModel
    {
        public int order_id { get; set; }
        public int user_id { get; set; }
    }
    public class OrderDamageCancellation
    {
        public int order_id { get; set; }
        public int user_id { get; set; }
        public int direction { get; set; }
    }
    public class DamageComments
    {
        public int damage_id { get; set; }
        public int user_id { get; set; }
        public string comment { get; set; }
    }
    public class DamageModel
    {
        public List<AttachmentVals> attachment { get; set; }
        public int user_id { get; set; }
        public string code { get; set; }
    }
    public class CancledCardModel
    {

        public int id { get; set; }
        public string tx1 { get; set; }
        public string tx2 { get; set; }
        public string tx3 { get; set; }
        public int user_id { get; set; }
    }
    public class CancledCardReturned
    {
        public decimal balance { get; set; }
        public decimal amount { get; set; }
        public double commisionAmount { get; set; }
        public int card_id { get; set; }
        public string PackName { get; set; }
        public int temporary_use { get; set; }
        public DateTime cancel_date { get; set; }
    }
    public class CardNum
    {
        public List<string> card_num { get; set; }
    }
    public class SaveReturned
    {
        public string return_attachment { get; set; }
        public int card_id { get; set; }
        public string cash { get; set; }
        public float amount { get; set; }
        public string comment { get; set; }
        public int reason { get; set; }
        public int bort_id { get; set; }
        public int pretentiu { get; set; }
        public bool force { get; set; }
        public bool commions_not { get; set; }
        public string date { get; set; }

    }
}
