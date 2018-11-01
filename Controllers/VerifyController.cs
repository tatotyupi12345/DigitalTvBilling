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
    public class VerifyController : Controller
    {
        private int pageSize = 20;
        // GET: Verify
        public async System.Threading.Tasks.Task<ActionResult> Index(int page = 1)
        {
            if (!Utils.Utils.GetPermission("SHOW_VERIFIED_CARDS"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

                string filter_status = Request["filter_status"];

                if(filter_status == "" || filter_status== null)
                {
                    filter_status = "All";
                }
                var val = -1;
                string where_ = "", cwhere = "";
                if (filter_status != "All")
                {
                    val = (int)Enum.Parse(typeof(CardVerifyStatus), filter_status);

                    where_ += " AND c.VerifyStatus=" + val;
                    cwhere += " AND verify_status=" + val;
                }
                

                string sqlcmd = @"select TOP(" + pageSize + @") * FROM (select row_number() over(ORDER BY id DESC) AS row_num, id as Id, tdate as Tdate, name as Name, lastname as LastName, code as Code, address as Address, type as Type, juridical_type as JuridicalType, is_budget as IsBudget, jurid_finish_date as JuridicalFinishDate, city as City, village as Village, district as District, email as Email, is_facktura as IsFacktura, region as Region, phone1 as Phone1, phone2 as Phone2, [desc] as [Desc], security_code as SecurityCode, user_id as UserId, verify_status as VerifyStatus, info as Info, buy_reason as BuyReason, is_satisfied as IsSatisfied, attachment_approve_status as AttachmentApproveStatus from book.Customers where tdate BETWEEN @date_from AND @date_to) as c where c.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize) + where_;// + " ORDER BY c.id DESC";

                int count = await _db.Database.SqlQuery<int>(@"select count(id) from book.Customers where tdate BETWEEN @date_from AND @date_to" + cwhere, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).FirstOrDefaultAsync();

                List<Customer> custlist = _db.Database.SqlQuery<Customer>(sqlcmd, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToList();
                List<Verify> verifyList = new List<Verify>();

                foreach (var item in _db.Customers.ToList())
                {
                    verifyList.Add(new Verify()
                    {
                        customer = item,
                        cards = _db.Cards.Where(c => c.CustomerId == item.Id).ToList()
                    });
                }

                //foreach (var item in verifyList)
                //{
                //    if(item.cards.All(c=>c.VerifyStatus == CardVerifyStatus.Passed))
                //    {
                //        item.customer.VerifyStatus = AbonentVerifyStatus.Passed;
                //    }
                //    if (item.cards.All(c => c.VerifyStatus == CardVerifyStatus.PassedWithError))
                //    {
                //        item.customer.VerifyStatus = AbonentVerifyStatus.PassedWithError;
                //    }
                //    if (item.cards.Any(c => (c.VerifyStatus == CardVerifyStatus.PassedWithError || c.VerifyStatus == CardVerifyStatus.Passed)) && item.cards.Any(c=> c.VerifyStatus == CardVerifyStatus.ForPass))
                //    {
                //        item.customer.VerifyStatus = AbonentVerifyStatus.ForPass;
                //    }

                //    if (item.cards.All(c => c.VerifyStatus == CardVerifyStatus.ForPass))
                //    {
                //        item.customer.VerifyStatus = AbonentVerifyStatus.ForPass;
                //    }
                //}

                ViewBag.filter_status = filter_status;
                ViewBag.verifyList = verifyList;
                return View(await _db.Database.SqlQuery<Customer>(sqlcmd, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToRawPagedListAsync(count, page, pageSize));
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> GetVerifications(int cust_id)
        {
            if (!Utils.Utils.GetPermission("SHOW_VERIFIED_CARDS"))
            {
                return PartialView("~/Views/Verify/_VerifyInfo.cshtml");
            }
            //DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            //DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            {
                Verify verify = new Verify()
                {
                    customer = _db.Customers.Where(c => c.Id == cust_id).FirstOrDefault(),
                    cards = _db.Cards.Where(c=>c.CustomerId == cust_id).ToList()
                };
                return PartialView("~/Views/Verify/_VerifyInfo.cshtml", verify);
            }
        }

        [HttpPost]
        public JsonResult SaveVerify(int id, int verify_status, string info)
        {
            if (!Utils.Utils.GetPermission("SET_VERIFYSTATUS_CARDS"))
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
                            card.VerifyStatus = (CardVerifyStatus)verify_status;
                            card.Info = info;
                            _db.Entry(card).State = EntityState.Modified;

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
                                Mode = LogMode.Verify,
                                TypeValue = card.CardNum,
                                TypeId = card.Id
                            });

                            _db.SaveChanges();

                            tran.Commit();
                            setCustomerVerifyStatus(card.CustomerId);
                            return Json(1);
                        }
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
            return Json(0);
        }

        [HttpPost]
        public JsonResult UpdateAbonentStatus(int cust_id, string dataval, int buyreason_status, int satisfied_status, string info)
        {
            if (!Utils.Utils.GetPermission("SET_VERIFYSTATUS_CARDS"))
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
                        Customer cust = _db.Customers.Where(c => c.Id == cust_id).FirstOrDefault();
                        if (cust != null)
                        {
                            switch(dataval)
                            {
                                case "buy_reason":
                                    {
                                        cust.BuyReason = (AbonentBuyReason)buyreason_status;
                                        cust.Info = info;
                                        _db.Entry(cust).State = EntityState.Modified;
                                    }
                                    break;

                                case "is_satisfied":
                                    {
                                        cust.IsSatisfied = (AbonentSatisfiedStatus)satisfied_status;
                                        cust.Desc = info;
                                        _db.Entry(cust).State = EntityState.Modified;
                                    }
                                    break;
                            }
                            //card.VerifyStatus = (CardVerifyStatus)verify_status;
                            //card.Info = info;
                            //_db.Entry(card).State = EntityState.Modified;

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

        private bool setCustomerVerifyStatus(int customer_id)
        {
            bool status = false;
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        Verify verify = new Verify()
                        {
                            customer = _db.Customers.Where(c => c.Id == customer_id).FirstOrDefault(),
                            cards = _db.Cards.Where(c => c.CustomerId == customer_id).ToList()
                        };


                        if (verify.cards.All(c => c.VerifyStatus == CardVerifyStatus.Passed))
                        {
                            verify.customer.VerifyStatus = AbonentVerifyStatus.Passed;
                        }
                        if (verify.cards.Any(c => c.VerifyStatus == CardVerifyStatus.PassedWithError))
                        {
                            verify.customer.VerifyStatus = AbonentVerifyStatus.PassedWithError;
                        }
                        if (verify.cards.Any(c => (c.VerifyStatus == CardVerifyStatus.PassedWithError || c.VerifyStatus == CardVerifyStatus.Passed)) && verify.cards.Any(c => c.VerifyStatus == CardVerifyStatus.ForPass))
                        {
                            verify.customer.VerifyStatus = AbonentVerifyStatus.ForPass;
                        }

                        if (verify.cards.All(c => c.VerifyStatus == CardVerifyStatus.ForPass))
                        {
                            verify.customer.VerifyStatus = AbonentVerifyStatus.ForPass;
                        }

                        if (verify.cards.All(c => c.VerifyStatus == CardVerifyStatus.Called))
                        {
                            verify.customer.VerifyStatus = AbonentVerifyStatus.Called;
                        }

                        if (verify.cards.Any(c => c.VerifyStatus == CardVerifyStatus.Problem))
                        {
                            verify.customer.VerifyStatus = AbonentVerifyStatus.Problem;
                        }

                        _db.Entry(verify.customer).State = EntityState.Modified;
                        _db.SaveChanges();

                        tran.Commit();
                        status = true;
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        status = false;
                    }
                }
                //return PartialView("~/Views/Verify/_VerifyInfo.cshtml", verify);
            }

            return status;
        }
    }
}