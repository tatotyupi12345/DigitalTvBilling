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
using System.Globalization;

namespace DigitalTVBilling.Controllers
{
    public class CustomerAttachController : Controller
    {
        // GET: CustomerAttach
        public ActionResult Index(int? page, int? attach_filter_id, int? user_id)
        {
            if (!Utils.Utils.GetPermission("SHOW_SELL_ATTACHMENTS"))
            {
                return new RedirectResult("/CustomerAttachController");
            }

            List<CustomerSellAttachments> attachs;

            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            using (DataContext _db = new DataContext())
            {
                attachs = _db.CustomerSellAttachments.Include("Customer").Include("Attachment").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).OrderByDescending(c => c.Id).ToList();
                if (attach_filter_id != null && attach_filter_id != 0)
                {
                    attachs = attachs.Where(a => a.Attachment.Id == attach_filter_id).ToList();
                }
                if (user_id != null && user_id != 0)
                {
                    attachs = attachs.Where(a => a.Diler_Id == user_id && a.Tdate >= dateFrom && a.Tdate <= dateTo).ToList();
                }
                ViewBag.attachments = _db.SellAttachments.ToList();
                ViewBag.selectedFilter = attach_filter_id;
                ViewBag.selectedUserFilter = user_id;
                ViewBag.CountSum = attachs.Select(a => a.Count).Sum();
                ViewBag.Users = _db.Users.ToList();
                //var studentQuery1 =
                //     from student in attachs
                //     group student by student.AttachmentID;
            }

            return View(attachs.ToPagedList(page ?? 1, 60));
        }

        [HttpPost]
        public JsonResult RecordApprove(int id, int diler_id)
        {
            if (!Utils.Utils.GetPermission("APPROVE_SELL_ATTACHMENTS"))
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
                        var attach = _db.CustomerSellAttachments.Include("Attachment").Where(c => c.CustomerID == id && c.Diler_Id == diler_id).ToList();
                        if (attach != null && attach.Count > 0)
                        {
                            attach.ToList().ForEach(a => a.VerifyStatus = 2);
                            attach.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);
                            //_db.Entry(attach).State = EntityState.Modified;

                            _db.Loggings.Add(new Logging()
                            {
                                Tdate = DateTime.Now,
                                UserId = user_id,
                                Type = LogType.SellAttachments,
                                Mode = LogMode.VerifySellAttachments,
                                TypeValue = attach.FirstOrDefault().CustomerID.ToString(),
                                TypeId = attach.FirstOrDefault().Id
                            });

                            //this.AddLoging(_db,
                            //             LogType.Card,
                            //             LogMode.Approve,
                            //             user_id,
                            //             order.Id,
                            //            "№ " + order.Num + " შეკვეთის დადასტურება",
                            //             new List<LoggingData>()
                            //          );

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

        [HttpPost]
        public async Task<PartialViewResult> GetAttachsInfo(int customer_attach_id, int attach_diller_id)
        {
            //if (!Utils.Utils.GetPermission("SHOW_VERIFIED_CARDS"))
            //{
            //    return PartialView("~/Views/Verify/_VerifyInfo.cshtml");
            //}
            //DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            //DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            {
                ViewBag.attachmenlist = _db.SellAttachments.ToList();
                List<CustomerSellAttachments> attachs = _db.CustomerSellAttachments.Where(a => a.CustomerID == customer_attach_id && a.Diler_Id == attach_diller_id).Include("Customer").Include("Attachment").ToList();
                //Card card = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.CustomerId).Where(c => c.Id == card_id).FirstOrDefault();
                return PartialView("~/Views/CustomerAttach/_AttachInfo.cshtml", attachs);
            }
        }
        [HttpPost]
        public async Task<PartialViewResult> GetAttachsInfoEdit(string code)
        {
            using (DataContext _db = new DataContext())
            {
                var id = 0;
                List<int> attach_diler = new List<int>();
                List<Diler> attach_diler_ID = new List<Diler>();
                var custumer_id = _db.Customers.Where(c => c.Code == code).Select(s => s.Id).FirstOrDefault();
                List<CustomerSellAttachments> attach = new List<CustomerSellAttachments>();

                // _db.CustomerSellAttachments.Where(a => a.CustomerID == custumer_id && a.Diler_Id == data.diller_id).ToList();
                attach = _db.CustomerSellAttachments.Where(a => a.CustomerID == custumer_id).Include("Customer").Include("Attachment").ToList();
                var customerSellA = from custAttachs in attach
                                    let dt = custAttachs.Tdate
                                    let diler_id = custAttachs.Diler_Id
                                    group custAttachs by new { y = dt.Year, m = dt.Month, d = dt.Day, diler = diler_id }
                                    into cGroup
                                    select cGroup;

                foreach (var group in customerSellA)
                {
                    Diler diler_group = new Diler();
                    diler_group.diler = group.Select(s => s.Diler_Id).FirstOrDefault();
                    diler_group.tdate = group.Select(s => s.Tdate.ToString("dd-MM-yyy")).FirstOrDefault();
                    attach_diler_ID.Add(diler_group);
                    var itemgrop = group.Select(s => s.Diler_Id).FirstOrDefault();
                }
                //foreach (var item in attach)
                //{
                //    Diler _diler_item = new Diler();
                //    _diler_item.diler = item.Diler_Id;
                //    _diler_item.tdate = item.Tdate.ToString("dd-MM-yyy");
                //    if (attach_diler.Contains(item.Diler_Id))
                //    {

                //    }
                //    else
                //    {
                //        //attach_diler_ID.Add(_diler_item);
                //        attach_diler.Add(item.Diler_Id);
                //    }
                //}
                //Diler _diler_items = new Diler();

                List<User> user_info = _db.Users.Include("UserType")/*.Where(c => c.UserType.Name.Contains("დილერი") || c.UserType.Id == 1 || c.UserType.Id == 2 || c.UserType.Id == 4 || c.UserType.Id == 5)*/.ToList();
                ViewBag.Bort = user_info;
                ViewBag.attachmenlist = _db.SellAttachments.ToList();
                ViewBag.attachdilerID = attach_diler_ID.OrderBy(d => d.tdate).ToList(); ;
                ViewBag.Code = code;
                return PartialView("~/Views/CustomerAttach/_AttachInfoEdit.cshtml", attach);
            }
        }
        [HttpPost]
        public PartialViewResult GetCode()
        {
            using (DataContext _db = new DataContext())
            {
                return PartialView("~/Views/CustomerAttach/_AttachCode.cshtml");
            }
        }
        [HttpPost]
        public JsonResult UpdateEntry(AttachmentData data)
        {
            if (!Utils.Utils.GetPermission("SHOW_SELL_ATTACHMENTS"))
            {
                return Json(0);
            }

            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        //if (data.customer_id == 0)
                        //{
                        //    Customer cust = _db.Customers.Where(c => c.Code == data.code).FirstOrDefault();
                        //    data.customer_id = cust.Id;
                        //}
                        if (data.code != null && data.code != "")
                        {
                            //Customer cust = _db.Customers.Where(c => c.Code == data.code).FirstOrDefault();
                            List<CustomerSellAttachments> cur_attachs = _db.CustomerSellAttachments.Where(a => a.CustomerID == data.customer_id && a.Diler_Id == data.diller_id).ToList();
                            var customerSellAttachs = from custAttachs in cur_attachs
                                                      group custAttachs by custAttachs.AttachmentID into cGroup
                                                      select new
                                                      {
                                                          Key = cGroup.Key,
                                                          customerAttachs = cGroup
                                                      };

                            foreach (var item in data.attachmentVals)
                            {
                                if (cur_attachs.Any(a => a.AttachmentID == item.id))
                                {
                                    foreach (var group in customerSellAttachs)
                                    {
                                        if (group.customerAttachs.FirstOrDefault().AttachmentID == item.id && group.customerAttachs.Select(a => a.Count).Sum() == item.count)
                                        {
                                            continue;
                                        }

                                        if (group.customerAttachs.FirstOrDefault().AttachmentID == item.id)
                                        {
                                            short verify_status = cur_attachs.FirstOrDefault().VerifyStatus == 2 || cur_attachs.FirstOrDefault().VerifyStatus == (short)1 ? (short)1 : (short)0;
                                            if (item.count > 0)
                                            {
                                                _db.CustomerSellAttachments.Add(new CustomerSellAttachments()
                                                {
                                                    Tdate = DateTime.Now,
                                                    AttachmentID = group.customerAttachs.FirstOrDefault().AttachmentID,
                                                    Count = item.count,
                                                    CustomerID = group.customerAttachs.FirstOrDefault().CustomerID,
                                                    Diler_Id = group.customerAttachs.FirstOrDefault().Diler_Id,
                                                    VerifyStatus = verify_status
                                                });

                                                cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                                cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);

                                                _db.CustomerSellAttachments.RemoveRange(group.customerAttachs);
                                                _db.SaveChanges();

                                            }
                                            else if (item.count == 0)
                                            {
                                                cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                                cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);

                                                _db.CustomerSellAttachments.RemoveRange(group.customerAttachs);
                                                _db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.count > 0)
                                    {
                                        short verify_status = 0;
                                        //if (cur_attachs.Any(a => a.AttachmentID == item.id))
                                        {
                                            verify_status = cur_attachs.FirstOrDefault().VerifyStatus == 2 || cur_attachs.FirstOrDefault().VerifyStatus == (short)1 ? (short)1 : (short)0;
                                            cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                            cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);
                                        }
                                        _db.CustomerSellAttachments.Add(new CustomerSellAttachments()
                                        {
                                            Tdate = DateTime.Now,
                                            AttachmentID = item.id,
                                            Count = item.count,
                                            CustomerID = data.customer_id,
                                            Diler_Id = data.diller_id,
                                            VerifyStatus = verify_status
                                        });

                                        _db.SaveChanges();
                                    }
                                }
                            }

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

        [HttpPost]
        public async Task<PartialViewResult> NewAttach()
        {
            //if (!Utils.Utils.GetPermission("SHOW_VERIFIED_CARDS"))
            //{
            //    return PartialView("~/Views/Verify/_VerifyInfo.cshtml");
            //}
            //DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            //DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            {
                var attachments = _db.SellAttachments.ToList();
                //List<CustomerSellAttachments> attachs = _db.CustomerSellAttachments.Where(a => a.CustomerID == customer_attach_id).Include("Customer").Include("Attachment").ToList();
                //Card card = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.CustomerId).Where(c => c.Id == card_id).FirstOrDefault();
                return PartialView("~/Views/CustomerAttach/_NewAttachment.cshtml", attachments);
            }
        }

        [HttpPost]
        public JsonResult NewEntry(AttachmentData data)
        {
            if (!Utils.Utils.GetPermission("SHOW_SELL_ATTACHMENTS"))
            {
                return Json(0);
            }

            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        if (data.code != null && data.code != "")
                        {
                            Customer cust = _db.Customers.Where(c => c.Code == data.code).FirstOrDefault();
                            //if (user_id != 0)
                            //{
                            //    cust.UserId = user_id;
                            //    _db.Entry(cust).State = EntityState.Modified;
                            //}
                            if (cust != null)
                            {
                                if (!_db.CustomerSellAttachments.Any(a => a.CustomerID == cust.Id))
                                {
                                    List<CustomerSellAttachments> new_attachments = new List<CustomerSellAttachments>();
                                    foreach (var item in data.attachmentVals)
                                    {
                                        if (item.count > 0)
                                            new_attachments.Add(new CustomerSellAttachments { AttachmentID = item.id, Count = item.count, CustomerID = cust.Id });
                                    }

                                    if (new_attachments.Count > 0)
                                    {
                                        _db.CustomerSellAttachments.AddRange(new_attachments);
                                        _db.SaveChanges();

                                        tran.Commit();
                                        return Json(1);
                                    }
                                }
                            }
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
        //public JsonResult updateEntryEdit(AttachmentData data, int diler_id)
        //{
        //       if (!Utils.Utils.GetPermission("SHOW_SELL_ATTACHMENTS"))
        //        {
        //            return Json(0);
        //        }
        //    using (DataContext _db = new DataContext())
        //    {
        //        using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
        //        {
        //            try
        //            {
        //                if (data.code != null && data.code != "")
        //                {
        //                    Customer cust = _db.Customers.Where(c => c.Code == data.code).FirstOrDefault();
        //                    List<CustomerSellAttachments> cur_attachs = _db.CustomerSellAttachments.Where(a => a.CustomerID == cust.Id && a.Diler_Id==diler_id).ToList();
        //                    foreach (var item in data.attachmentVals)
        //                    {
        //                        if (item.count > 0)
        //                        {
        //                            if (cur_attachs.Any(a => a.AttachmentID == item.id && a.Count == item.count))
        //                            {
        //                                continue;
        //                            }
        //                            if (cur_attachs.Any(a => a.AttachmentID == item.id && a.Diler_Id==diler_id && a.Count!=item.count))
        //                            {
        //                                CustomerSellAttachments attach = cur_attachs.Where(a => a.AttachmentID == item.id && a.Diler_Id == diler_id).FirstOrDefault();
        //                                attach.Count = item.count;
        //                                _db.Entry(attach).State = EntityState.Modified;

        //                                //cust.AttachmentApproveStatus = (short)(cust.AttachmentApproveStatus == 2 ? 3 : 1);
        //                                _db.Entry(cust).State = EntityState.Modified;

        //                                _db.SaveChanges();

        //                            }
        //                            else
        //                            {
        //                                _db.CustomerSellAttachments.Add(new CustomerSellAttachments
        //                                {
        //                                    AttachmentID = item.id,
        //                                    Count = item.count,
        //                                    CustomerID = cust.Id,
        //                                    Diler_Id=diler_id

        //                                });
        //                                cust.AttachmentApproveStatus = (short)(cust.AttachmentApproveStatus == 2 ? 3 : 1);
        //                                _db.Entry(cust).State = EntityState.Modified;
        //                                _db.SaveChanges();
        //                            }
        //                        }
        //                        else if (item.count == 0)
        //                        {
        //                            if (cur_attachs.Any(a => a.AttachmentID == item.id && a.Diler_Id == diler_id))
        //                            {
        //                                CustomerSellAttachments attach = cur_attachs.Where(a => a.AttachmentID == item.id).FirstOrDefault();
        //                                _db.CustomerSellAttachments.Remove(attach);
        //                                _db.SaveChanges();
        //                            }
        //                        }
        //                    }
        //                }
        //                _db.SaveChanges();

        //                tran.Commit();
        //                return Json(1);
        //            }
        //            catch (Exception ex)
        //            {
        //                tran.Rollback();
        //                return Json(0);
        //            }
        //        }
        //    }
        // //   return Json(1);
        //}
        [HttpPost]
        public JsonResult UpdateEntryEdit(AttachmentData data, string date)
        {
            if (!Utils.Utils.GetPermission("SHOW_SELL_ATTACHMENTS"))
            {
                return Json(0);
            }

            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        if (data.customer_id == 0)
                        {
                            Customer cust = _db.Customers.Where(c => c.Code == data.code).FirstOrDefault();
                            data.customer_id = cust.Id;
                        }
                        if (data.temporarily == 1)
                        {
                            Customer cust_temp = _db.Customers.Where(c => c.Code == data.code).FirstOrDefault();
                            cust_temp.temporary_use = 1;

                        }
                        if (data.code != null && data.code != "")
                        {
                            //Customer cust = _db.Customers.Where(c => c.Code == data.code).FirstOrDefault();
                            List<CustomerSellAttachments> cur_attachs_date = _db.CustomerSellAttachments.Where(a => a.CustomerID == data.customer_id && a.Diler_Id == data.diller_id).ToList();
                            List<CustomerSellAttachments> cur_attachs = new List<CustomerSellAttachments>();
                            foreach (var item in cur_attachs_date)
                            {
                                if (item.Tdate.ToString("dd-MM-yyy") == date && item.CustomerID == data.customer_id && item.Diler_Id == data.diller_id)
                                {
                                    cur_attachs.Add(item);
                                }
                            }
                            var customerSellAttachs = from custAttachs in cur_attachs
                                                      group custAttachs by custAttachs.AttachmentID into cGroup
                                                      select new
                                                      {
                                                          Key = cGroup.Key,
                                                          customerAttachs = cGroup
                                                      };

                            foreach (var item in data.attachmentVals)
                            {
                                if (cur_attachs.Any(a => a.AttachmentID == item.id))
                                {
                                    foreach (var group in customerSellAttachs)
                                    {
                                        if (group.customerAttachs.FirstOrDefault().AttachmentID == item.id && group.customerAttachs.Select(a => a.Count).Sum() == item.count)
                                        {
                                            continue;
                                        }

                                        if (group.customerAttachs.FirstOrDefault().AttachmentID == item.id)
                                        {
                                            short verify_status = cur_attachs.FirstOrDefault().VerifyStatus == 2 || cur_attachs.FirstOrDefault().VerifyStatus == (short)1 ? (short)1 : (short)0;
                                            if (item.count > 0)
                                            {
                                                _db.CustomerSellAttachments.Add(new CustomerSellAttachments()
                                                {
                                                    Tdate = DateTime.Now,
                                                    AttachmentID = group.customerAttachs.FirstOrDefault().AttachmentID,
                                                    Count = item.count,
                                                    CustomerID = group.customerAttachs.FirstOrDefault().CustomerID,
                                                    Diler_Id = group.customerAttachs.FirstOrDefault().Diler_Id,
                                                    VerifyStatus = verify_status,
                                                    status = group.customerAttachs.FirstOrDefault().status
                                                });

                                                cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                                cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);
                                                cur_attachs.Remove(group.customerAttachs.Select(s => s).FirstOrDefault());
                                                _db.CustomerSellAttachments.RemoveRange(group.customerAttachs);
                                                _db.SaveChanges();

                                            }
                                            else if (item.count == 0)
                                            {

                                                cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                                cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);
                                                cur_attachs.Remove(group.customerAttachs.Select(s => s).FirstOrDefault());
                                                _db.CustomerSellAttachments.RemoveRange(group.customerAttachs);
                                                _db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.count > 0)
                                    {
                                        var status = customerSellAttachs.Select(s => s.customerAttachs.Where(a => a.CustomerID == data.customer_id).Select(ss => ss.status).FirstOrDefault()).FirstOrDefault();
                                        short verify_status = 0;
                                        //if (cur_attachs.Any(a => a.AttachmentID == item.id))
                                        {
                                            verify_status = cur_attachs.FirstOrDefault().VerifyStatus == 2 || cur_attachs.FirstOrDefault().VerifyStatus == (short)1 ? (short)1 : (short)0;
                                            cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                            cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);
                                        }
                                        _db.CustomerSellAttachments.Add(new CustomerSellAttachments()
                                        {
                                            Tdate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                                            AttachmentID = item.id,
                                            Count = item.count,
                                            CustomerID = data.customer_id,
                                            Diler_Id = data.diller_id,
                                            VerifyStatus = verify_status,
                                            status = status
                                        });

                                        _db.SaveChanges();
                                    }
                                }
                            }
                            if (data.temporarily == 1)
                            {
                                CustomerSellAttachments customerSell = _db.CustomerSellAttachments.Where(c => c.CustomerID == data.customer_id).FirstOrDefault();
                                customerSell.status = SellAttachmentStatus.temporary_use;
                                customerSell.VerifyStatus = 6;
                                _db.Entry(customerSell).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
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

        [HttpPost]
        public JsonResult NewEntryEdit(AttachmentData data, int diler_id)
        {
            if (!Utils.Utils.GetPermission("SHOW_SELL_ATTACHMENTS"))
            {
                return Json(0);
            }

            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        if (data.code != null && data.code != "")
                        {
                            Customer cust = _db.Customers.Where(c => c.Code == data.code).FirstOrDefault();
                            DateTime dateTo = new DateTime();
                            dateTo = dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
                            var datefrom = _db.CustomerSellAttachments.Where(a => a.CustomerID == cust.Id && a.Diler_Id == diler_id).Select(s => s.Tdate).FirstOrDefault();
                            if (datefrom.ToString("dd-MM-yyy") == dateTo.ToString("dd-MM-yyy"))
                            {
                                return Json(0);
                            }
                            if (cust != null)
                            {
                                List<CustomerSellAttachments> new_attachments = new List<CustomerSellAttachments>();
                                foreach (var item in data.attachmentVals)
                                {
                                    if (item.count > 0)
                                    {
                                        short verify_status = 0;
                                        var attachment = 0;
                                        if (_db.CustomerSellAttachments.Any(a => a.CustomerID == cust.Id && a.Diler_Id == diler_id))
                                        {
                                            verify_status = _db.CustomerSellAttachments.Where(a => a.CustomerID == cust.Id && a.Diler_Id == diler_id).FirstOrDefault().VerifyStatus;
                                            verify_status = verify_status == 2 || verify_status == 1 ? (short)1 : (short)0;
                                            var attachments = _db.CustomerSellAttachments.Where(a => a.CustomerID == cust.Id && a.Diler_Id == diler_id).ToList();
                                            attachments.ForEach(a => a.VerifyStatus = verify_status);
                                            attachments.ForEach(a => _db.Entry(a).State = EntityState.Modified);
                                        }
                                        new_attachments.Add(new CustomerSellAttachments { AttachmentID = item.id, Count = item.count, CustomerID = cust.Id, Diler_Id = diler_id, VerifyStatus = verify_status });
                                    }
                                }
                                var promo = _db.Database.SqlQuery<int>(@"Select sp.package_id from book.Cards as c
                                                                            inner join book.Customers as cu on cu.id=c.customer_id
                                                                            inner join doc.Subscribes as s on s.card_id=c.id
                                                                            inner join doc.SubscriptionPackages as sp on sp.subscription_id=s.id where cu.code='"+data.code+"' and sp.package_id=304086").FirstOrDefault();
                                if (data.temporarily == 1)
                                {
                                    new_attachments.ToList().ForEach(a => a.VerifyStatus = 6);
                                    new_attachments.ToList().ForEach(a => a.status = SellAttachmentStatus.temporary_use);
                                }
                                if (promo==304086)
                                {
                                    new_attachments.ToList().ForEach(a => a.VerifyStatus = 5);
                                    new_attachments.ToList().ForEach(a => a.status = SellAttachmentStatus.temporary_use);
                                }
                                if (new_attachments.Count > 0)
                                {
                                    _db.CustomerSellAttachments.AddRange(new_attachments);
                                    _db.SaveChanges();

                                    //if (data.temporarily == 1)
                                    //{
                                    //    CustomerSellAttachments customerSell = _db.CustomerSellAttachments.Where(c => c.CustomerID == _db.Customers.Where(cc => cc.Code == data.code).FirstOrDefault().Id).FirstOrDefault();
                                    //    customerSell.status = SellAttachmentStatus.temporary_use;
                                    //    customerSell.VerifyStatus = 5;
                                    //    _db.Entry(customerSell).State = EntityState.Modified;
                                    //    _db.SaveChanges();
                                    //}

                                    tran.Commit();
                                    return Json(1);
                                }
                                //}
                            }

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
        [NonAction]
        public JsonResult getNewEntryEdit(DamageModel damage)
        {

            //    if (!Utils.Utils.GetPermission("SHOW_SELL_ATTACHMENTS"))
            //{
            //    return Json(0);
            //}

            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        //if (data.customer_id == 0)
                        {
                            //Customer cust = _db.Customers.Where(c => c.Code == damage.code).FirstOrDefault();
                            //data.customer_id = cust.Id;
                        }
                        if (damage.code != null && damage.code != "")
                        {
                            Customer cust = _db.Customers.Where(c => c.Code == damage.code).FirstOrDefault();
                            List<CustomerSellAttachments> cur_attachs_date = _db.CustomerSellAttachments.Where(a => a.CustomerID == cust.Id && a.Diler_Id == damage.user_id).ToList();
                            List<CustomerSellAttachments> cur_attachs = new List<CustomerSellAttachments>();
                            foreach (var item in cur_attachs_date)
                            {
                                if (item.CustomerID == cust.Id && item.Diler_Id == damage.user_id)
                                {
                                    cur_attachs.Add(item);
                                }
                            }
                            var customerSellAttachs = from custAttachs in cur_attachs
                                                      group custAttachs by custAttachs.AttachmentID into cGroup
                                                      select new
                                                      {
                                                          Key = cGroup.Key,
                                                          customerAttachs = cGroup
                                                      };

                            foreach (var item in damage.attachment)
                            {
                                if (cur_attachs.Any(a => a.AttachmentID == item.id))
                                {
                                    foreach (var group in customerSellAttachs)
                                    {
                                        //if (group.customerAttachs.FirstOrDefault().AttachmentID == item.id && group.customerAttachs.Select(a => a.Count).Sum() == item.count)
                                        //{
                                        //    continue;
                                        //}

                                        if (group.customerAttachs.FirstOrDefault().AttachmentID == item.id)
                                        {
                                            short verify_status = cur_attachs.FirstOrDefault().VerifyStatus == 2 || cur_attachs.FirstOrDefault().VerifyStatus == (short)1 ? (short)1 : (short)0;
                                            if (item.count > 0)
                                            {
                                                var count_attach = cur_attachs.Where(c => c.AttachmentID == item.id).Select(s => s.Count).FirstOrDefault() + item.count;
                                                _db.CustomerSellAttachments.Add(new CustomerSellAttachments()
                                                {
                                                    Tdate = DateTime.Now,
                                                    AttachmentID = group.customerAttachs.FirstOrDefault().AttachmentID,
                                                    Count = count_attach,// item.count,
                                                    CustomerID = group.customerAttachs.FirstOrDefault().CustomerID,
                                                    Diler_Id = group.customerAttachs.FirstOrDefault().Diler_Id,
                                                    VerifyStatus = verify_status
                                                });

                                                cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                                cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);
                                                cur_attachs.Remove(group.customerAttachs.Select(s => s).FirstOrDefault());
                                                _db.CustomerSellAttachments.RemoveRange(group.customerAttachs);
                                                _db.SaveChanges();

                                            }
                                            //else if (item.count == 0)
                                            //{

                                            //    cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                            //    cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);
                                            //    cur_attachs.Remove(group.customerAttachs.Select(s => s).FirstOrDefault());
                                            //    _db.CustomerSellAttachments.RemoveRange(group.customerAttachs);
                                            //    _db.SaveChanges();
                                            //}
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.count > 0)
                                    {
                                        short verify_status = 0;
                                        if (cur_attachs.Any(a => a.AttachmentID == item.id))
                                        {
                                            verify_status = cur_attachs.FirstOrDefault().VerifyStatus == 2 || cur_attachs.FirstOrDefault().VerifyStatus == (short)1 ? (short)1 : (short)0;
                                            cur_attachs.ToList().ForEach(a => a.VerifyStatus = verify_status);
                                            cur_attachs.ToList().ForEach(a => _db.Entry(a).State = EntityState.Modified);
                                        }
                                        _db.CustomerSellAttachments.Add(new CustomerSellAttachments()
                                        {
                                            Tdate = DateTime.Now,
                                            AttachmentID = item.id,
                                            Count = item.count,
                                            CustomerID = cust.Id,
                                            Diler_Id = damage.user_id,
                                            VerifyStatus = verify_status
                                        });

                                        _db.SaveChanges();
                                    }
                                }
                            }
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
        //[NonAction]
        //public JsonResult getNewEntryEdit(DamageModel damage)
        //{

        //    using (DataContext _db = new DataContext())
        //    {
        //        using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
        //        {
        //            try
        //            {
        //                if (damage.code != null && damage.code != "")
        //                {
        //                    Customer cust = _db.Customers.Where(c => c.Code == damage.code).FirstOrDefault();
        //                    DateTime dateTo = new DateTime();
        //                    dateTo = DateTime.Now;// Utils.Utils.GetRequestDate(Request["dt_to"], false);
        //                    var datefrom = _db.CustomerSellAttachments.Where(a => a.CustomerID == cust.Id && a.Diler_Id == damage.user_id).Select(s => s.Tdate).FirstOrDefault();
        //                    //if (datefrom.ToString("dd-MM-yyy") == dateTo.ToString("dd-MM-yyy"))
        //                    //{
        //                    //    return Json(0);
        //                    //}
        //                    if (cust != null)
        //                    {
        //                        List<CustomerSellAttachments> new_attachments = _db.CustomerSellAttachments.Where(c => c.CustomerID == cust.Id && c.Diler_Id == damage.user_id).ToList();
        //                        foreach (var item in damage.attachment)
        //                        {
        //                            if (item.count > 0 && new_attachments.Any(c=>c.AttachmentID==item.id && c.Count==0))
        //                            {
        //                                short verify_status = 0;
        //                                var attachment = 0;
        //                                if (_db.CustomerSellAttachments.Any(a => a.CustomerID == cust.Id && a.Diler_Id == damage.user_id))
        //                                {
        //                                    verify_status = _db.CustomerSellAttachments.Where(a => a.CustomerID == cust.Id && a.Diler_Id == damage.user_id).FirstOrDefault().VerifyStatus;
        //                                    verify_status = verify_status == 2 || verify_status == 1 ? (short)1 : (short)0;
        //                                    var attachments = _db.CustomerSellAttachments.Where(a => a.CustomerID == cust.Id && a.Diler_Id == damage.user_id).ToList();
        //                                    attachments.ForEach(a => a.VerifyStatus = verify_status);
        //                                    attachments.ForEach(a => _db.Entry(a).State = EntityState.Modified);

        //                                }
        //                                    new_attachments.Add(new CustomerSellAttachments { AttachmentID = item.id, Count = item.count, CustomerID = cust.Id, Diler_Id = damage.user_id, VerifyStatus = verify_status });
        //                            }
        //                        }

        //                        if (new_attachments.Count > 0)
        //                        {
        //                            _db.CustomerSellAttachments.AddRange(new_attachments);
        //                            _db.SaveChanges();

        //                            tran.Commit();
        //                            return Json(1);
        //                        }
        //                        //}
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                tran.Rollback();
        //                return Json(0);
        //            }
        //        }
        //    }
        //    return Json(0);
        //}
        public JsonResult GetUsersFilter(string filter, int diler_id)
        {
            using (DataContext _db = new DataContext())
            {
                var user = _db.Users.Where(c => c.Id == diler_id && c.CodeWord == filter).FirstOrDefault();
                if (user != null)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }

        }
    }

    public class AttachmentData
    {
        public string code { get; set; }
        public List<AttachmentVals> attachmentVals { get; set; }
        public int diller_id { get; set; }
        public int customer_id { get; set; }
        public int temporarily { get; set; }
    }

    public class AttachmentVals
    {
        public int id { get; set; }
        public int count { get; set; }
    }
}