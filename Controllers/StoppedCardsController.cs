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

namespace DigitalTVBilling.Controllers
{
    public class StoppedCardsController : BaseController
    {
        private int pageSize = 20;
        // GET: StoppedCards
        public async System.Threading.Tasks.Task<ActionResult> Index(int? page, string name, string dt_from, string dt_to, int? drp_filter, int? status)
        {
            if (!Utils.Utils.GetPermission("STOPPED_CARDS_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            DateTime dateFrom, dateTo;
            if (dt_from == "" || dt_to == "")
            {
                dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            }
            else
            {
                dateFrom = Utils.Utils.GetRequestDate(dt_from, true);
                dateTo = Utils.Utils.GetRequestDate(dt_to, false);
            }
            ViewBag.status = status == null ? -1 : status;

            List<CardStat> cardstats = new List<CardStat>();
            List<Card> cards;
            List<Logging> _loggings;
            List<User> _users;
            List<SellerObject> _sellers;

            using (DataContext _db = new DataContext())
            {
                cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.CustomerId).Where(c => c.CardStatus != CardStatus.Canceled && c.CardStatus == CardStatus.Closed).ToList();

                if (name != null && name != "")
                {
                    if (drp_filter != null)
                        switch (drp_filter)
                        {
                            case 1:
                                var fullname = name.Split(' ');

                                string fname = "", lname = "";
                                if (fullname.Length > 0)
                                {
                                    if (fullname.Length > 1)
                                    {
                                        fname = fullname[0];
                                        lname = fullname[1];
                                    }
                                    else
                                        fname = fullname[0];
                                }
                                if (fullname[0] != "undefined")
                                    cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.CustomerId).Where(c => c.CardStatus != CardStatus.Canceled && c.CardStatus == CardStatus.Closed && (c.Customer.Name.Contains(fname) && c.Customer.LastName.Contains(lname)) || (c.Customer.Name.Contains(lname) && c.Customer.LastName.Contains(fname))).ToList();
                                break;
                            case 2:
                                cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.CustomerId).Where(c => c.CardStatus != CardStatus.Canceled && c.CardStatus == CardStatus.Closed && c.CardNum.Contains(name)).ToList();
                                break;
                            case 3:
                                cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.CustomerId).Where(c => c.CardStatus != CardStatus.Canceled && c.CardStatus == CardStatus.Closed && c.AbonentNum.Contains(name)).ToList();
                                break;
                            default:
                                break;
                        }
                }
                if (status != null)
                {
                    if (status != -1 && StoppedCardStatus.Delay != (StoppedCardStatus)status && StoppedCardStatus.Promo != (StoppedCardStatus)status)
                    {
                        if (StoppedCardStatus.Unchecked == (StoppedCardStatus)status)
                        {
                            cards = cards.Where(c => c.StoppedCheckStatus == (StoppedCardStatus)status && c.FinishDate>=dateFrom && c.FinishDate<=dateTo).ToList();
                        }
                        else
                        {
                            cards = cards.Where(c => c.StoppedCheckStatus == (StoppedCardStatus)status && c.CallDate >= dateFrom && c.CallDate <= dateTo).ToList();

                        }
                       

                    }

                    else
                    {
                        if (StoppedCardStatus.Delay == (StoppedCardStatus)status)
                        {
                            cards = cards.Where(c => c.CallDate >= dateFrom && c.CallDate <= dateTo).ToList();
                        }
                    }
                }
                if (status != null)
                {

                    if (StoppedCardStatus.Promo == (StoppedCardStatus)status)
                    {
                        cards = cards.Where(c => c.Subscribtions.First().SubscriptionPackages.Any(s => s.Package.Id == 304086)).Select(s => s).ToList();
                    }

                }
                //cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).ToList();
                _loggings = _db.Loggings.Where(l => l.Mode == LogMode.Add && l.Type == LogType.Card).ToList();
                _users = _db.Users.Include("UserType").ToList();
                _sellers = _db.Seller.ToList();

                foreach (var card in cards)
                {
                    //if (card.Customer.Type == CustomerType.Technic)
                    //    continue;
                    cardstats.Add(new CardStat() { card = card, });
                }
                int rownum = cardstats.Count;
                foreach (var cardstat in cardstats)
                {
                    cardstat.rowNumber = rownum--;
                    cardstat.customer = cardstat.card.Customer;
                    cardstat.subscribe = cardstat.card.Subscribtions.Where(s => s.Status == true).First();// _db.Subscribtions.Where(s => s.CardId == cardstat.card.Id && s.Status == true).First();
                    cardstat.subscribePackages = cardstat.subscribe.SubscriptionPackages.ToList();// _db.SubscriptionPackages.Where(s => s.SubscriptionId == cardstat.subscribe.Id).ToList();
                    cardstat.logging = _loggings.Where(l => (l.TypeId == cardstat.card.Id || l.TypeValue == cardstat.card.AbonentNum)).FirstOrDefault();
                    cardstat.user = _users.Where(u => u.Id == cardstat.logging.UserId).First();
                    cardstat.seller = _sellers.Where(s => s.ID == cardstat.user.@object).FirstOrDefault();
                    cardstat.userType = cardstat.user.UserType;// _db.UserTypes.Where(u => u.Id == cardstat.user.Type).FirstOrDefault();
                                                               //int[] arr = cardstat.subscribePackages.Select(s => s.PackageId).ToArray();
                    cardstat.packages = cardstat.subscribePackages.Select(s => s.Package).ToList();// _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                }
            }

            return View(cardstats.ToPagedList(page ?? 1, 20));

        }

        [HttpPost]
        public async Task<PartialViewResult> GetStatusInfo(int card_id)
        {
            //if (!Utils.Utils.GetPermission("SHOW_VERIFIED_CARDS"))
            //{
            //    return PartialView("~/Views/Verify/_VerifyInfo.cshtml");
            //}
            //DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            //DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            {
                Card card = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.CustomerId).Where(c => c.Id == card_id).FirstOrDefault();
                return PartialView("~/Views/StoppedCards/_StatusInfo.cshtml", card);
            }
        }

        [HttpPost]
        public JsonResult SaveStatus(int id, int call_status, string desc, string date)
        {
            if (!Utils.Utils.GetPermission("STOPPED_CARDS_CHANGE_STATUS"))
            {
                return Json(0);
            }

            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        //int user_id = ((User)Session["CurrentUser"]).Id;
                        Card card = _db.Cards.Where(c => c.Id == id).FirstOrDefault();
                        if (card != null)
                        {
                            card.StoppedCheckStatus = (StoppedCardStatus)call_status;
                            card.Desc = desc;
                            if (StoppedCardStatus.Delay == (StoppedCardStatus)call_status)
                            {
                                DateTime dateFrom = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(5, 2)), int.Parse(date.Substring(8, 2)), 0, 0, 0);
                                card.CallDate = dateFrom;
                            }
                            else
                            {
                                card.CallDate = DateTime.Now;
                            }
                            _db.Entry(card).State = EntityState.Modified;

                            //this.AddLoging(_db,
                            //             LogType.Order,
                            //             LogMode.Change,
                            //             user_id,
                            //             order.Id,
                            //            "№ " + order.Num + " შეკვეთის დადასტურება",
                            //             new List<LoggingData>()
                            //          );

                            //_db.Loggings.Add(new Logging()
                            //{
                            //    Tdate = DateTime.Now,
                            //    UserId = user_id,
                            //    Type = LogType.Card,
                            //    Mode = LogMode.Verify,
                            //    TypeValue = card.CardNum,
                            //    TypeId = card.Id
                            //});

                            _db.SaveChanges();

                            tran.Commit();
                            //setCustomerVerifyStatus(card.CustomerId);
                            return Json(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
            return Json(0);
        }
        //[HttpPost]
        //public JsonResult ChangeDate(int card_num, string date)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
        //        {
        //            try
        //            {
        //                //int[] order_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
        //                //foreach (int order_id in order_ids)
        //                //{
        //                    DateTime dateFrom = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(5, 2)), int.Parse(date.Substring(8, 2)), 0, 0, 0);
        //                    StopCardDate stop_card_date = new StopCardDate() {
        //                        card_num = card_num,
        //                        tdate= dateFrom,
        //                        status=StoppedCardStatus.Delay


        //                    };
        //                _db.StopCardDates.Add(stop_card_date);
        //                _db.SaveChanges();
        //                        int user_id = ((User)Session["CurrentUser"]).Id;
        //                        //this.AddLoging(_db,
        //                        //                 LogType.Order,
        //                        //                 LogMode.Change,
        //                        //                 user_id,
        //                        //                 stop_card_date.Id,
        //                        //                 "გათიშული ბარათის თარიღის შეცვლა - " + dateFrom.ToString("dd/MM/yyyy HH:mm"),
        //                        //                 new List<LoggingData>()
        //                        //              );

        //                        _db.SaveChanges();

        //                    //}
        //                //}

        //                tran.Commit();
        //            }
        //            catch
        //            {
        //                tran.Rollback();
        //                return Json(0);
        //            }
        //        }

        //        return Json(1);
        //    }
        //}
    }
}