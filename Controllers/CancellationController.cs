using Dapper;
using DigitalTVBilling.CallCenter;
using DigitalTVBilling.CallCenter.Infrastructure;
using DigitalTVBilling.Filters;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Microsoft.AspNet.SignalR;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class CancellationController : BaseController
    {
        public ActionResult Index(int? user_id, string name, int? cancled_status, int? page = 1)
        {
            if (!Utils.Utils.GetPermission("CANCELLATIONS_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            {
                using (var daper_db = new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ToString()))
                {
                    string str = "  SELECT c.[id],cu.[code] FROM [DigitalTVBilling].[book].[Cards] as c inner join [DigitalTVBilling].[book].[Customers] as cu on  c.customer_id=cu.id";
                    //using (var multi = daper_db.QueryMultiple(str))
                    //{
                    ViewBag.cards = daper_db.Query<CardCustumerID>(str);
                    ViewBag.Returned = daper_db.Query<ReturnedCard>("SELECT * FROM [DigitalTVBilling].[dbo].[ReturnedCards] ");
                    //}
                    ViewBag.Cancel = "active";
                    ViewBag.UserGroups = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Users.Where(c => c.UserType.Name == "მემონტაჟეები").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
                    ViewBag.ExecutorUsers = _db.Users.Where(u => u.Type == 4 || u.Type==44).ToList();
                    ViewBag.selectedStatus = cancled_status == null ? -1 : cancled_status;
                    //ViewBag.Returned = _db.ReturnedCards.ToList();
                    List<Cancellation> cancel = new List<Cancellation>();
                    if (name != null && name != "")
                    {
                        //var fullname = name.Split('/')[0];

                        cancel = _db.Cancellations.Where(c => c.Name.Substring(0, name.Length) == name || c.Code == name).OrderByDescending(c => c.Id).ToList();//.ToPagedListAsync(page, 20);
                        return View(cancel.ToPagedList(page ?? 1, 20));
                    }
                    if (user_id != null && user_id != 0)
                    {
                        ViewBag.selectedUserFilter = user_id;
                        //orders = orders.Where(o => o.ExecutorID == user_id).ToList();
                        if (cancled_status != null && cancled_status != -1)
                        {
                            cancel = _db.Cancellations.Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == user_id && c.Status == (CancleStatus)cancled_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Id).ToList();
                        }
                        else
                            cancel = _db.Cancellations.Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == user_id /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Id).ToList();
                    }
                    else
                    {
                        if (cancled_status != null && cancled_status != -1)
                            cancel = _db.Cancellations.Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == c.ExecutorID && c.Status == (CancleStatus)cancled_status  /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Id).ToList();
                        else
                            cancel = _db.Cancellations.Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == c.ExecutorID /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Id).ToList();
                    }
                    //cancel = _db.Cancellations.ToList();


                    return View(cancel.ToPagedList(page ?? 1, 20));
                }
            }
        }
        [HttpGet]
        public ActionResult New(int? id)
        {
            if (!Utils.Utils.GetPermission("ORDER_ADD"))
            {
                return new RedirectResult("/Main");
            }

            if (id.HasValue && id.Value > 0)
            {
                using (DataContext _db = new DataContext())
                {
                    Cancellation _cancellation = _db.Cancellations.Where(c => c.Id == id.Value).FirstOrDefault();
                    if (_cancellation != null)
                    {
                        return View(new Cancellation()
                        {
                            Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Abonent>(_cancellation.Data).Customer,
                            GetDate = _cancellation.GetDate,
                            ReceiversCount = _cancellation.ReceiversCount,
                        });
                    }
                }
            }

            return View(new Cancellation() { Customer = new Customer(), GetDate = DateTime.Now, ReceiversCount = 1 });
        }

        [HttpPost]
        public JsonResult New(CancellationCardNum cancellation)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(cancellation);

                        //if (id.HasValue && id.Value > 0)
                        //{
                        //    Cancellation _cancellation = _db.Cancellations.Where(c => c.Id == id.Value).FirstOrDefault();
                        //    if (_cancellation != null)
                        //    {
                        //        _cancellation.Name = cancellation.Customer.Name + " " + cancellation.Customer.LastName + "/" + cancellation.Customer.Phone1;
                        //        _cancellation.Code = cancellation.Customer.Code;
                        //        _cancellation.GetDate = cancellation.GetDate;
                        //        _cancellation.Data = data;
                        //        _cancellation.ReceiversCount = cancellation.ReceiversCount;
                        //        _cancellation.Address = cancellation.Customer.Address;
                        //        _db.Entry(_cancellation).State = EntityState.Modified;
                        //    }
                        //}
                        //else
                        //{
                        string str = cancellation.CardNum[0];
                        var fooArray = str.Split(',');  // now you have an array of 3 strings

                        for (int i = 0; i < fooArray.Length; i++)
                        {

                            Models.Cancellation _cancellation = new Cancellation
                            {
                                Id = 0,
                                Status =CancleStatus.loading,// cancellation.Status,
                                Name = cancellation.Customer_Name + " " + cancellation.Customer_LastName + "/" + cancellation.Customer_Phone1,
                                Code = cancellation.Customer_Code,
                                GetDate =DateTime.Now,
                                Data = data,
                                UserId = user_id,
                                ChangeDate = new DateTime(2222, 12, 12),
                                Tdate = DateTime.Now,
                                ReceiversCount = 1,
                                Address = cancellation.Customer_Address,
                                card_num = Convert.ToInt32(fooArray[i]),
                                UserGroupId = 1,
                            };

                            _cancellation.Num = (_db.Cancellations.Max(x => (int?)x.Num) ?? 0) + 1;
                            _db.Cancellations.Add(_cancellation);

                            this.AddLoging(_db,
                            LogType.Cancled,
                            LogMode.Add,
                            user_id,
                            _cancellation.Id,
                            _cancellation.Name,
                            new List<LoggingData>());
                        }

                        //}

                        _db.SaveChanges();

                        var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>();
                        context.Clients.All.onHitRecorded("CancellationNew", user_id);


                        tran.Commit();
                        return Json("");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                    }
                }
                ViewBag.UserGroups = _db.Users.Where(c => c.UserType.Name == "მემონტაჟეები").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();
            }
            return Json("");
            //ViewBag.Error = true;
            //return View(cancellation);
        }
        [HttpGet]
        public PartialViewResult GroupChange(int Cancellation_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<User> Users = _db.Users.Where(o => o.Type == 4 || o.Type==44).ToList();
                ViewBag.selectedExecutor = _db.Cancellations.Where(o => o.Id == Cancellation_id).FirstOrDefault().ExecutorID;
                return PartialView("~/Views/Cancellation/_GroupChange.cshtml", Users);

            }
        }

        [HttpPost]
        public JsonResult Custumer(string code, string phone)
        {
            using (DataContext db = new DataContext())
            {
                CustumerData custumer_data = new CustumerData();
                custumer_data.customer = db.Customers.Where(c => c.Code == code || c.Phone1 == phone).ToList().FirstOrDefault();


                if (custumer_data.customer != null)
                {
                    custumer_data.card = db.Cards.Where(c => c.CustomerId == custumer_data.customer.Id).Select(s => s.CardNum).ToList();
                    return Json(custumer_data);
                }
            }
            return Json("");
        }
        [HttpPost]
        public JsonResult GroupChange(int order_id, string ids, int group_id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        int[] order_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
                        //foreach (int order_id in order_ids)
                        {
                            Cancellation Cancellation = _db.Cancellations.Where(c => c.Id == order_id).FirstOrDefault();
                            int old_executor_id = Cancellation.ExecutorID;
                            if (Cancellation != null)
                            {
                                //if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                                //    continue;

                                //if (Cancellation.Status == CancellationStatus.)
                                //    Cancellation.Status = CancellationStatus.Worked;
                                Cancellation.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                //order.UserGroupId = group_id;
                                Cancellation.cancle_status = 0;
                                Cancellation.Status = CancleStatus.Cancel;
                                Cancellation.ExecutorID = group_id;
                                Cancellation.ChangeDate = DateTime.Now;
                                Cancellation.IsApproved = false;
                                _db.Entry(Cancellation).State = EntityState.Modified;


                                this.AddLoging(_db,
                                                 LogType.Cancled,
                                                 LogMode.Change,
                                                 user_id,
                                                 Cancellation.Id,
                                                 "ჯგუფის შეცვლა - " + _db.Users.Where(c => c.Id == group_id).Select(c => c.Name).FirstOrDefault(),
                                                 new List<LoggingData>()
                                              );

                                _db.SaveChanges();

                                if (group_id != 0 && old_executor_id != group_id)
                                {
                                    string phoneto = _db.Users.Where(u => u.Id == group_id).FirstOrDefault().Phone;
                                    MessageTemplate message = _db.MessageTemplates.Where(m => m.Name == "OnCancellationAttach_Geo").FirstOrDefault();
                                    if (phoneto != "558595900")
                                        Task.Run(async () => { await Utils.Utils.sendMessage(phoneto, message.Desc); }).Wait();
                                }
                            }
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
            return Json(1);
        }
        [HttpPost]
        public JsonResult ChangeStatus(string ids, string status)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        int[] order_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
                        foreach (int order_id in order_ids)
                        {
                            Cancellation Cancellation = _db.Cancellations.Where(c => c.Id == order_id).FirstOrDefault();
                            if (Cancellation != null)
                            {
                                //order.ExecutorID = executerID != null ? Convert.ToInt32(executerID) : 0;
                                _db.Entry(Cancellation).State = EntityState.Modified;
                                _db.SaveChanges();

                                if (Cancellation.Status == (CancleStatus)Enum.Parse(typeof(CancleStatus), status))
                                    continue;
                                string old_val = Utils.Utils.GetEnumDescription(Cancellation.Status);
                                Cancellation.Status = (CancleStatus)Enum.Parse(typeof(CancleStatus), status);
                                Cancellation.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                Cancellation.IsApproved = false;
                                Cancellation.ApproveUser = "";
                                Cancellation.ChangeDate = DateTime.Now;
                                // Cancellation.Group_User = ((User)Session["CurrentUser"]).Name;
                                _db.Entry(Cancellation).State = EntityState.Modified;

                                this.AddLoging(_db,
                                                 LogType.Cancled,
                                                 LogMode.Change,
                                                 user_id,
                                                 Cancellation.Id,
                                                 Cancellation.Num.ToString(),
                                                 new List<LoggingData>() { new LoggingData() { field = "სტატუსის შეცვლა", old_val = old_val, new_val = Utils.Utils.GetEnumDescription((CancleStatus)Enum.Parse(typeof(CancleStatus), status)) } }
                                              );

                                _db.SaveChanges();
                            }
                            if((CancleStatus)Enum.Parse(typeof(CancleStatus), status) == CancleStatus.Closed)
                            {
                                var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                                context.Clients.All.onHitRecorded("Cancellation", Cancellation.ExecutorID);
                                _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[Cancellation]  SET  [to_go] = 0 WHERE executor_id ={Cancellation.ExecutorID }");
                            }
                            if ((CancleStatus)Enum.Parse(typeof(CancleStatus), status) == CancleStatus.NotClosed)
                            {
                                var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                                context.Clients.All.onHitRecorded("Cancellation_Not", Cancellation.ExecutorID);
                                _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[Cancellation]  SET  [to_go] = 0 WHERE executor_id ={Cancellation.ExecutorID }");
                            }
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
                return Json(1);
            }

        }
        [HttpPost]
        public async Task<JsonResult> FilterCancellactionByName(string letter,int user_id, int page)
        {
            string column = "o.name";
            string where = column + " LIKE N'%" + letter + "%'";
            if (column == "cr.status" || column == "cr.tower_id")
                where = column + "=" + letter;
            else if (column == "c.lastname+c.name")
                where = column + " LIKE N'%" + letter + "%'";
            if (user_id != 0)
                where += " and executor_id=" + user_id;
            string sql = @"SELECT TOP(" + 20 + @") * FROM (SELECT row_number() over(ORDER BY o.id) AS row_num, o.executor_id, o.data,o.code,o.comment,o.to_go,o.name,o.id,o.num,o.tdate,o.get_date,o.change_date,
                                 o.receivers_count,o.status,o.changer_user,o.approve_user,u.name AS create_user,u_g.name AS group_name,o.is_approved, u_exec.name as exec_name,
                                 STUFF((SELECT '<br />' + r.[text] FROM doc.DamageReasons AS rr INNER JOIN book.Reasons AS r ON r.id=rr.reason_id WHERE rr.damage_id=o.id FOR XML PATH ('')),1,1,'') AS reasons  FROM dbo.Cancellation AS o 
                               INNER JOIN book.Users AS u_g ON o.user_group_id=u_g.id
                               INNER JOIN book.Users AS u ON u.id=o.user_id 
          LEFT JOIN book.Users as u_exec on u_exec.id = o.executor_id
                               WHERE " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * 20);

            string sql_count = @"SELECT COUNT(d.id) FROM (SELECT row_number() over(ORDER BY o.id) AS row_num, o.executor_id, o.data,o.code,o.to_go,o.name,o.id,o.num,o.tdate,o.get_date,o.change_date,
                                 o.receivers_count,o.status,o.changer_user,o.approve_user,u.name AS create_user,u_g.name AS group_name,o.is_approved, u_exec.name as exec_name,
                                 STUFF((SELECT '<br />' + r.[text] FROM doc.DamageReasons AS rr INNER JOIN book.Reasons AS r ON r.id=rr.reason_id WHERE rr.damage_id=o.id FOR XML PATH ('')),1,1,'') AS reasons  FROM dbo.Cancellation AS o 
                               INNER JOIN book.Users AS u_g ON o.user_group_id=u_g.id
                               INNER JOIN book.Users AS u ON u.id=o.user_id 
          LEFT JOIN book.Users as u_exec on u_exec.id = o.executor_id
                               WHERE " + where + ") AS d ";

            System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());
            using (DataContext _db = new DataContext())
            {
                int count = await _db.Database.SqlQuery<int>(sql_count).FirstOrDefaultAsync();
                var findList = await _db.Database.SqlQuery<CancellationFilter>(sql).ToRawPagedListAsync(count, page, 100);
                var execs = _db.Users.Where(u => u.Type == 4).ToList();
                return Json(new
                {
                    execs = execs,
                    Abonents = findList,
                    Paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, findList, p => p.ToString(), PagedListRenderOptions.PageNumbersOnly).ToHtmlString()
                });
            }
        }
        [HttpPost]
        public JsonResult ChangeComment(string ids, string comment)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        int[] order_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
                        foreach (int order_id in order_ids)
                        {
                            Cancellation Cancellation = _db.Cancellations.Where(c => c.Id == order_id).FirstOrDefault();
                            if (Cancellation != null)
                            {
                                var data= Newtonsoft.Json.JsonConvert.DeserializeObject<CancellationCardNum>(Cancellation.Data);
                                data.Customer_Desc = comment;
                                Cancellation.Data = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                                
                                //order.ExecutorID = executerID != null ? Convert.ToInt32(executerID) : 0;
                                _db.Entry(Cancellation).State = EntityState.Modified;
                                _db.SaveChanges();
                               
                            }
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
                return Json(1);
            }

        }
        [NonAction]
        public bool __CancledApprove(int id, int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        //int user_id = ((User)Session["CurrentUser"]).Id;
                        Cancellation cancel = _db.Cancellations.Where(c => c.Id == id).FirstOrDefault();
                        if (cancel != null)
                        {
                            cancel.ExecutorID = user_id != null ? Convert.ToInt32(user_id) : 0;
                            _db.Entry(cancel).State = EntityState.Modified;
                            _db.SaveChanges();



                            //if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                            //    continue;

                            string old_val = Utils.Utils.GetEnumDescription(cancel.Status);
                            cancel.Status = CancleStatus.Closed;
                            cancel.ChangerUser = _db.Users.Where(u => u.Id == user_id).FirstOrDefault().Name;// ((User)Session["CurrentUser"]).Name;
                            cancel.IsApproved = false;
                            cancel.ApproveUser = "";
                            cancel.ChangeDate = DateTime.Now;

                            _db.Entry(cancel).State = EntityState.Modified;


                            this.AddLoging(_db,
                                             LogType.Order,
                                             LogMode.Change,
                                             user_id,
                                             cancel.Id,
                                             cancel.Num.ToString(),
                                             new List<LoggingData>() { new LoggingData() { field = "სტატუსის შეცვლა", old_val = old_val, new_val = Utils.Utils.GetEnumDescription(CancleStatus.Closed) } }
                                          );

                            _db.SaveChanges();
                            tran.Commit();

                            var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                            context.Clients.All.onHitRecorded("Cancellation", user_id);
                            context.Clients.All.onHitRecorded("RegionClosed",
                             new RegionGoName(
                                 new SqlConnection(
                                             ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                   cancel.Id,
                                   "dbo.Cancellation",
                                   user_id
                             ).Result(),
                             user_id
                             );

                            return true;
                        }
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
            return false;
        }
        public JsonResult ChangeDate(string ids, string date)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int[] order_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
                        foreach (int order_id in order_ids)
                        {
                            DateTime dateFrom = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(5, 2)), int.Parse(date.Substring(8, 2)), 0, 0, 0);
                            Cancellation _cancellation = _db.Cancellations.Where(c => c.Id == order_id).FirstOrDefault();
                            if (_cancellation != null)
                            {
                                //if (_cancellation.Status == DamageStatus.Closed || _cancellation.Status == DamageStatus.Processing)
                                //    continue;

                                //if (damage.Status == OrderStatus.Registered)
                                //damage.Status = OrderStatus.Worked;
                                _cancellation.GetDate = dateFrom;
                                _cancellation.ChangeDate = DateTime.Now;
                                _cancellation.IsApproved = false;
                                _cancellation.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                _db.Entry(_cancellation).State = EntityState.Modified;

                                int user_id = ((User)Session["CurrentUser"]).Id;
                                this.AddLoging(_db,
                                                 LogType.Order,
                                                 LogMode.Change,
                                                 user_id,
                                                 _cancellation.Id,
                                                 "დაზიანების მისვლის თარიღის შეცვლა - " + dateFrom.ToString("dd/MM/yyyy HH:mm"),
                                                 new List<LoggingData>()
                                              );

                                _db.SaveChanges();

                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }

                return Json(1);
            }
        }
        [NonAction]
        public bool __NotApprove(int id, int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        //int user_id = ((User)Session["CurrentUser"]).Id;
                        Cancellation cancel = _db.Cancellations.Where(c => c.Id == id).FirstOrDefault();
                        if (cancel != null)
                        {
                            cancel.ExecutorID = user_id != null ? Convert.ToInt32(user_id) : 0;
                            _db.Entry(cancel).State = EntityState.Modified;
                            _db.SaveChanges();



                            //if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                            //    continue;

                            string old_val = Utils.Utils.GetEnumDescription(cancel.Status);
                            cancel.Status = CancleStatus.NotClosed;
                            cancel.ChangerUser = _db.Users.Where(u => u.Id == user_id).FirstOrDefault().Name;// ((User)Session["CurrentUser"]).Name;
                            cancel.IsApproved = false;
                            cancel.ApproveUser = "";
                            cancel.to_go = 0;
                            cancel.ChangeDate = DateTime.Now;

                            _db.Entry(cancel).State = EntityState.Modified;


                            this.AddLoging(_db,
                                             LogType.Order,
                                             LogMode.Change,
                                             user_id,
                                             cancel.Id,
                                             cancel.Num.ToString(),
                                             new List<LoggingData>() { new LoggingData() { field = "სტატუსის შეცვლა", old_val = old_val, new_val = Utils.Utils.GetEnumDescription(CancleStatus.Closed) } }
                                          );

                            _db.SaveChanges();
                            tran.Commit();

                            var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                            context.Clients.All.onHitRecorded("Cancellation_Not", user_id);

                            return true;
                        }
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
            return false;
        }
        [HttpPost]
        public JsonResult CancelApprove(int id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        Cancellation cancel = _db.Cancellations.Where(c => c.Id == id).FirstOrDefault();
                        if (cancel != null)
                        {
                            cancel.IsApproved = true;
                            cancel.ApproveUser = ((User)Session["CurrentUser"]).Name;
                            _db.Entry(cancel).State = EntityState.Modified;

                            //this.AddLoging(_db,
                            //             LogType.Order,
                            //             LogMode.Change,
                            //             user_id,
                            //             order.Id,
                            //            "№ " + order.Num + " შეკვეთის დადასტურება",
                            //             new List<LoggingData>()
                            //          );
                            _db.SaveChanges();

                            tran.Commit();


                            var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                            context.Clients.All.onHitRecorded("Cancellation", user_id);

                            return Json(1);
                        }
                    }
                    catch
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
            return Json(0);
        }
        [HttpPost]
        public PartialViewResult _ReturnedCardCancle(int returned_card_id)
        {
            using (DataContext _db = new DataContext())
            {
                var ReturnedCardCancel = _db.ReturnedCards.Include("ReturnedCardAttachments").Where(c => c.Id == returned_card_id).Select(s => s).ToList();
                int card_id = ReturnedCardCancel.Where(c => c.Id == returned_card_id).Select(s => s.card_id).FirstOrDefault();
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
                        MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                        ServiceAmount = c.CardCharges.Where(s => s.Status == CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        WithoutServiceAmount = c.CardCharges.Where(s => s.Status != CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        CardServices = c.CardServices.ToList()
                    }).First();

                    decimal balance = Utils.Utils.GetBalance(__card.PaymentAmount, __card.ChargeAmount);
                    ViewBag.Balance = balance;
                    if (balance >= (decimal)commision_ammount)
                    {
                        ViewBag.Amount = balance - (decimal)commision_ammount;
                    }
                    else
                    {
                        ViewBag.Amount = 0;
                    }
                    ViewBag.card_id = card_id;
                }
                ViewBag.attachmenlist = _db.ReceiverAttachments.ToList();

                List<User> user_info = _db.Users.Include("UserType").Where(c => c.UserType.Name.Contains("დილერი") || c.UserType.Id == 1 || c.UserType.Id == 2 || c.UserType.Id == 4 || c.UserType.Id == 5).ToList();
                // ნაღდი უნაღდოს ამოღება
                string[] arrraytechnical = _db.ReturnedCards.Where(c => c.Id == returned_card_id).Select(ss => ss.commission).ToArray();
                Commisions _returne_cash = new Commisions();
                CashCashles returncard_cash = new CashCashles();
                returncard_cash = _returne_cash.ReturnedCard_commision_arr(arrraytechnical);
                ViewBag.returncard_cash = returncard_cash;
                ViewBag.ReturnedCardCancel = ReturnedCardCancel.Where(c => c.Id == returned_card_id).FirstOrDefault();
                ViewBag.ReturnedCardAttachments = ReturnedCardCancel.Select(s => s.ReturnedCardAttachments.Select(ss => ss).ToList()).FirstOrDefault();
                ViewBag.Returned_Amount = ReturnedCardCancel.Select(s => s.returned_amount).FirstOrDefault();
                return PartialView("~/Views/Returned/_ReturnedCardCancl.cshtml", user_info);
            }
        }
        [NonAction]
        public bool CancellationComment(int id, int user_id, string comment)
        {
            using (DataContext _db = new DataContext())
            {
                if (comment != "")
                {
                    //string str = "UPDATE[dbo].[Cancellation] SET[comment] = '" + comment + "'  WHERE id = " + id;
                    var Cancellation = _db.Cancellations.Where(c => c.Id == id).FirstOrDefault();
                    Cancellation.comment = comment;
                    _db.Entry(Cancellation).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                return true;
            }
        }

        [NonAction]
        public string CancellationWriteComment(int id)
        {
            using (DataContext _db = new DataContext())
            {
                var _damage = _db.Database.SqlQuery<string>("SELECT d.comment FROM dbo.Cancellation d where d.id=" + id).FirstOrDefault();
                if (_damage == null)
                {
                    _damage = "";
                }
                return _damage;
            }
        }
    }
}
public class CustumerData
{
    public Customer customer { get; set; }
    public List<string> card { get; set; }
}