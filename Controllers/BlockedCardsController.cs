using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace DigitalTVBilling.Controllers
{
    public class BlockedCardsController : Controller
    {
        private int pageSize = 20;
        // GET: Accountant
        public async System.Threading.Tasks.Task<ActionResult> Index(string name, string dt_from, string dt_to, int? drp_filter, int status = -1,int Discontinued=-1 , int page = 1)
        {
            if (!Utils.Utils.GetPermission("BLOCKED_CARDS_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                //DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                //DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

                DateTime dateFrom, dateTo;
                //DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

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

                ViewBag.JuridicalActive = "active";
                ViewBag.selectedStatus = status;

                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 01, 01, 01);
                today = today.AddMonths(-6);
                //if (Discontinued == 1)
                //{
                //    List<CardBlock>  _card = _db.Database.SqlQuery<CardBlock>("SELECT * FROM book.Cards WHERE status=5 and finish_date<= DATEADD(mm, -6, GETDATE()) ").ToList();



                //IPagedList<CardBlock> card_ = _card.ToPagedList(page, pageSize);

                //    ViewBag.drpfiltet = drp_filter == null ? 1 : drp_filter;
                //    ViewBag.totalItemsCount = card_.TotalItemCount;
                //    ViewBag.page = page;
                //    ViewBag.pageSize = pageSize;

                //    return View(card_);
                //}
                List<Card> cards = new List<Card>();
                if (drp_filter == 8)
                {
                    cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.FinishDate <= today && c.CardStatus == CardStatus.Blocked).ToList();
                }
                else
                {
                    cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.Customer.Type != CustomerType.Technic && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.CardStatus == CardStatus.Blocked).ToList();
                }
                if (status != -1)
                    cards = cards.Where(c => c.BlockCardVerifyStatus == (CardBlockedCardsStatus)status).ToList();

                if (name != null && name != "")
                {
                    if (drp_filter != null)
                        switch (drp_filter)
                        {
                            case 1:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.DocNum.Contains(name)).ToList();
                                break;

                            case 2:
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


                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => (c.Customer.Name.Contains(fname) && c.Customer.LastName.Contains(lname)) || (c.Customer.Name.Contains(lname) && c.Customer.LastName.Contains(fname))).ToList();

                                break;

                            case 3:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.Customer.Code.Contains(name)).ToList();
                                break;

                            case 4:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.AbonentNum.Contains(name)).ToList();
                                break;

                            case 5:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.CardNum.Contains(name)).ToList();
                                break;

                            case 6:

                                break;

                            case 7:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.Customer.Phone1.Contains(name)).ToList();
                                break;
                            default:
                                break;


                        }
                    ViewBag.filterText = name;
                   
                }
                
                IPagedList<Card> cards_ = cards.ToPagedList(page, pageSize);

                ViewBag.drpfiltet = drp_filter == null ? 1 : drp_filter;
                ViewBag.totalItemsCount = cards_.TotalItemCount;
                ViewBag.page = page;
                ViewBag.pageSize = pageSize;

                return View(cards_);
            }
        }

        public async System.Threading.Tasks.Task<PartialViewResult> PartialIndex(string name, string dt_from, string dt_to, int? drp_filter, string search, int status = -1, int page = 1)
        {
            using (DataContext _db = new DataContext())
            {


                if (!Utils.Utils.GetPermission("BLOCKED_CARDS_SHOW"))
                {
                    return PartialView(new List<Card>().ToPagedList(page, pageSize));
                }
                //DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                //DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

                DateTime dateFrom, dateTo;
                //DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

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

                ViewBag.JuridicalActive = "active";
                ViewBag.selectedStatus = status;
                List<Card> cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.Customer.Type != CustomerType.Technic && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.CardStatus == CardStatus.Blocked && (c.Customer.Name.Contains(search) || c.Customer.LastName.Contains(search))).ToList(); // && c.User.Name.Contains(search)

                if (status != -1)
                    cards = cards.Where(c => c.BlockCardVerifyStatus == (CardBlockedCardsStatus)status).ToList();
                if (search != null && search != "")
                {
                    if (drp_filter != null)
                        switch (drp_filter)
                        {
                            case 1:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.DocNum.Contains(search) && c.CardStatus == CardStatus.Blocked).ToList();
                                break;

                            case 2:
                                var fullname = search.Split(' ');

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


                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => (c.Customer.Name.Contains(fname) && c.Customer.LastName.Contains(lname)) && c.CardStatus == CardStatus.Blocked || (c.Customer.Name.Contains(lname) && c.Customer.LastName.Contains(fname)) && c.CardStatus == CardStatus.Blocked).ToList();

                                break;

                            case 3:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.Customer.Code.Contains(search) && c.CardStatus == CardStatus.Blocked).ToList();
                                break;

                            case 4:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.AbonentNum.Contains(search) && c.CardStatus == CardStatus.Blocked).ToList();
                                break;

                            case 5:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.CardNum.Contains(search) && c.CardStatus == CardStatus.Blocked).ToList();
                                break;

                            case 6:

                                break;

                            case 7:
                                cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.Customer.Phone1.Contains(search) && c.CardStatus == CardStatus.Blocked).ToList();
                                break;

                            default:
                                break;
                        }
                    ViewBag.filterText = search;

                }

                IPagedList<Card> cards_ = cards.ToPagedList(page, pageSize);

                ViewBag.drpfiltet = drp_filter == null ? 1 : drp_filter;
                ViewBag.totalItemsCount = cards_.TotalItemCount;
                ViewBag.page = page;
                ViewBag.pageSize = pageSize;

                return PartialView(cards_);
            }
        }

        //[HttpPost]
        //public async Task<JsonResult> FilterListByName(string letter, int page)
        //{

        //}

        [HttpPost]
        public async Task<PartialViewResult> GetStatusInfo(int card_id)
        {
            //if (!Utils.Utils.GetPermission("BLOCKED_CARDS_SHOW"))
            //{
            //    return PartialView("~/Views/Verify/_VerifyInfo.cshtml", new Card());
            //}

            using (DataContext _db = new DataContext())
            {
                Card card = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.CustomerId).Where(c => c.Id == card_id).FirstOrDefault();
                return PartialView("~/Views/BlockedCards/Block_StatusInfo.cshtml", card);
            }
        }

        [HttpPost]
        public JsonResult SaveStatus(int id, int call_status, string desc)
        {
            if (!Utils.Utils.GetPermission("JURIDICAL_STATUS_CHANGE"))
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
                            card.BlockCardVerifyStatus = (CardBlockedCardsStatus)call_status;
                            card.Desc = desc;
                            card.CallDate = DateTime.Now;
                            _db.Entry(card).State = EntityState.Modified;

                            var ChangeStatus = Utils.Utils.GetEnumDescription(card.BlockCardVerifyStatus);

                            //this.AddLoging(_db,
                            //             LogType.Order,
                            //             LogMode.Change,
                            //             user_id,
                            //             order.Id,
                            //            "№ " + order.Num + " შეკვეთის დადასტურება",
                            //             new List<LoggingData>()
                            //          );

                            _db.Loggings.Add(new Logging()
                            {
                                Tdate = DateTime.Now,
                                UserId = user_id,
                                Type = LogType.Card,
                                Mode = LogMode.JuridVerify,
                                TypeValue = card.CardNum,
                                TypeId = card.Id
                            });

                            _db.SaveChanges();

                            tran.Commit();
                            //setCustomerVerifyStatus(card.CustomerId);
                            return Json(new { BlockStatus = ChangeStatus, ID=1 });
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
        //public JsonResult RecordApprove(int id)
        //{
        //    if (!Utils.Utils.GetPermission("FINA_APPROVE"))
        //    {
        //        return Json(0);
        //    }
        //    using (DataContext _db = new DataContext())
        //    {
        //        using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
        //        {
        //            try
        //            {
        //                //int user_id = ((User)Session["CurrentUser"]).Id;
        //                //Card card = _db.Cards.Where(c => c.Id == id).FirstOrDefault();
        //                //if (card != null)
        //                //{
        //                //    card.ApproveStatus = 2;
        //                //    _db.Entry(card).State = EntityState.Modified;

        //                //    _db.Loggings.Add(new Logging()
        //                //    {
        //                //        Tdate = DateTime.Now,
        //                //        UserId = user_id,
        //                //        Type = LogType.Card,
        //                //        Mode = LogMode.Approve,
        //                //        TypeValue = card.CardNum,
        //                //        TypeId = card.Id
        //                //    });

        //                //    //this.AddLoging(_db,
        //                //    //             LogType.Card,
        //                //    //             LogMode.Approve,
        //                //    //             user_id,
        //                //    //             order.Id,
        //                //    //            "№ " + order.Num + " შეკვეთის დადასტურება",
        //                //    //             new List<LoggingData>()
        //                //    //          );

        //                //    _db.SaveChanges();

        //                //    tran.Commit();
        //                //    return Json(1);
        //                //}
        //            }
        //            catch
        //            {
        //                tran.Rollback();
        //                return Json(0);
        //            }
        //        }
        //    }
        //    return Json(0);
        //}
    }
}