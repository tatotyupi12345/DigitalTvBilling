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
    public class DamageController : BaseController
    {
        public ActionResult Index(int? user_id, string name, int? order_status, int? operator_status, bool? checked_user, bool? checked_bort_end, int? user_answers, int? page = 1)
        {
            if (!Utils.Utils.GetPermission("DAMAGE_SHOW"))
            {
                return new RedirectResult("/Main");
            }

            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            using (DataContext _db = new DataContext())
            {
                //var _cust = _db.Customers.ToList();
                //var damage_d = _db.Damages.Select(s => s.Code).ToList();
                //foreach (var item in damage_d)
                //{
                //    Damage _damage = _db.Damages.Where(c => c.Code == item).Select(s => s).FirstOrDefault(); ;

                //    if (_damage != null)
                //    {
                //        _damage.montage_user_id = _cust.Where(c => c.Code == item).Select(s => s.UserId).FirstOrDefault();
                //        _db.Entry(_damage).State = EntityState.Modified;

                //    }
                //    _db.SaveChanges();
                //}
                //var damage_d = _db.Damages.Select(s => s).ToList();

                //Damage d = new Damage();
                //foreach (var item in damage_d)
                //{
                //    if (user_id != null)
                //    {
                //        var _damage = _db.Damages.Where(c => c.Code == item.Code && c.ExecutorID == user_id && c.montage_user_id == 0).Select(s => s).ToList();
                //        int count = _damage.Count();
                //        for (int i = 0; i < _damage.Count() - 1; i++)
                //        {

                //            d = _damage.Where(c => c.Code == item.Code && c.ExecutorID == user_id && c.montage_user_id == 0).Select(s => s).FirstOrDefault();
                //            if (d != null && user_id != null && d.montage_user_id == 0 && d.ExecutorID == user_id)
                //            {
                //                d.montage_user_id = d.ExecutorID;
                //                _db.Entry(d).State = EntityState.Modified;

                //                var x = 0;
                //            }


                //        }
                //        _db.SaveChanges();
                //    }

                //}
                ViewBag.selectedUserFilter = 0;
                //ViewBag.UserGroupsOperator= Newtonsoft.Json.JsonConvert.SerializeObject(_db.Users.Where(c => c.UserType.Id==16).Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
                ViewBag.UserGroups = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Users.Where(c => c.UserType.Name == "მემონტაჟეები").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
                ViewBag.Reasons = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Reasons.Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
                ViewBag.ExecutorUsers = _db.Users.Where(u => u.Type == 4 || u.Type==44).ToList();
                ViewBag.OperatorUsers = _db.Users.Where(u => u.Type == 39).ToList();
                ViewBag.selectedStatus = order_status == null ? -1 : order_status;
                ViewBag.Custumers = _db.Customers.ToList();
                ViewBag.GroupUser = _db.OperatorGroupUsers.ToList();
                // Task<IPagedList<Damage>> damages = null;
                List<Damage> damages = new List<Damage>();
                //var orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                if ((user_id != null && user_id != 0 && checked_bort_end == true))
                {
                    if (order_status != null && order_status != -1)
                    {
                        damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.montage_user_id == user_id && c.ExecutorID != 0 && c.Status == (DamageStatus)order_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        //damages = damages.Where(c => c.ExecutorID != user_id).ToList();
                        return View(damages.ToPagedList(page ?? 1, 20));
                    }
                }
                if (name != null && name != "")
                {
                    //var fullname = name.Split('/')[0];

                    damages = _db.Damages.Where(c => c.Name.Substring(0, name.Length) == name || c.Code == name).OrderByDescending(c => c.Id).ToList();//.ToPagedListAsync(page, 20);
                    return View(damages.ToPagedList(page ?? 1, 20));
                }
                if (order_status == -2)
                {
                    if (user_id != null && user_id != 0)
                    {
                        damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.MontageStatus == true && c.ExecutorID == user_id && c.Status != DamageStatus.Closed/*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        return View(damages.ToPagedList(page ?? 1, 20));
                    }
                    else
                    {
                        damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.MontageStatus == true && c.Status != DamageStatus.Closed/*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        return View(damages.ToPagedList(page ?? 1, 20));
                    }
                }
                if (user_id != null && user_id != 0)
                {
                    ViewBag.selectedUserFilter = user_id;
                    if (checked_user == true)
                    {
                        if (order_status != null && order_status != -1)
                        {
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.montage_user_id == user_id && c.Status == (DamageStatus)order_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        }
                        else
                        {
                            if (user_answers != null && user_answers != 0)
                            {
                                damages = _db.Damages.Include("DamageReserveAnswers").Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.DamageReserveAnswers.Any(o => o.tdate >= dateFrom && o.tdate <= dateTo && o.user_id == user_id && o.reserve_answer == (DamageCommitStatic)user_answers)).OrderByDescending(c => c.Num).ToList();//
                            }
                            else 
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.montage_user_id == user_id /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        }
                    }
                    else
                    {
                        //orders = orders.Where(o => o.ExecutorID == user_id).ToList();
                        if (order_status != null && order_status != -1)
                        {
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == user_id && c.Status == (DamageStatus)order_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        }
                        else
                        {
                            if (user_answers != null && user_answers != 0)
                            {
                                damages = _db.Damages.Include("DamageReserveAnswers").Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.DamageReserveAnswers.Any(o => o.tdate >= dateFrom && o.tdate <= dateTo && o.reserve_answer == (DamageCommitStatic)user_answers)).OrderByDescending(c => c.Num).ToList();//
                            }
                            else
                            {
                                damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == user_id /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                            }
                        }
                    }
                }
                else
                {
                    if (checked_user == true)
                    {
                        if (order_status != null && order_status != -1)
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.montage_user_id == c.ExecutorID && c.Status == (DamageStatus)order_status  /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        else
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.montage_user_id == c.ExecutorID /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Tdate).ToList();//.ToPagedListAsync(page, 20);
                    }
                    else
                    {


                        if (order_status != null && order_status != -1)
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == c.ExecutorID && c.Status == (DamageStatus)order_status  /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        else
                        {
                            if (user_answers != null && user_answers != 0)
                            {
                                damages = _db.Damages.Include("DamageReserveAnswers").Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.DamageReserveAnswers.Any(o => o.tdate >= dateFrom && o.tdate <= dateTo && o.reserve_answer == (DamageCommitStatic)user_answers)).OrderByDescending(c => c.Num).ToList();//
                            }
                            else
                            {
                                damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == c.ExecutorID /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Tdate).ToList();//.ToPagedListAsync(page, 20);
                            }
                        }
                    }
                }
                if (operator_status != null && operator_status != 0)
                {
                    damages = new List<Damage>();
                    var OperatorUsers = _db.Users.Where(u => u.Type == 39).ToList();
                    var operator_name = OperatorUsers.Where(cc => cc.Id == operator_status).Select(s => s.Name).FirstOrDefault();
                    var group_user = _db.OperatorGroupUsers.Where(c => c.name == operator_name).Select(s => s.d_id).ToList();
                    foreach (var item in group_user)
                    {
                        var damage_list = _db.Damages.Where(c => c.Id == item).FirstOrDefault();
                        damages.Add(damage_list);
                    }
                    // damages = _db.Damages.Include("UserUser").Include("UserGroup").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == user_id && c.Id==group_user /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();
                }
                return View(damages.ToPagedList(page ?? 1, 20));
            }
        }

        public PartialViewResult BortShowInfo(int? user_id, string name, int? order_status, int? operator_status, bool? checked_user, bool? checked_bort_end, bool? checked_bort, int? page = 1)
        {

            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            using (DataContext _db = new DataContext())
            {

                ViewBag.selectedUserFilter = user_id;

                ViewBag.UserGroups = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Users.Where(c => c.UserType.Name == "მემონტაჟეები").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
                ViewBag.Reasons = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Reasons.Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
                ViewBag.ExecutorUsers = _db.Users.ToList();
                ViewBag.OperatorUsers = _db.Users.Where(u => u.Type == 39).ToList();
                ViewBag.selectedStatus = order_status == null ? -1 : order_status;
                ViewBag.Custumers = _db.Customers.ToList();
                ViewBag.GroupUser = _db.OperatorGroupUsers.ToList();
                ViewBag.DamageUser = _db.Damages.ToList();
                List<Damage> damages = new List<Damage>();
                if ((user_id != null && user_id != 0 && checked_bort == true))
                {
                    {
                        damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.montage_user_id == user_id && c.ExecutorID != 0 /*|| (c.Status==DamageStatus.loading && c.montage_user_id == user_id)*/ ).OrderByDescending(c => c.Num).ToList();                                                                                                                                                                                                                                                                                                                                           //damages = damages.Where(c => c.ExecutorID != user_id).ToList();
                        return PartialView("~/Views/Damage/_bortInfo.cshtml", damages.ToPagedList(page ?? 1, 200));
                    }
                }
                if ((user_id != null && user_id != 0 && checked_bort_end == true))
                {
                    //if (order_status != null && order_status != -1)
                    {
                        damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.ChangeDate >= dateFrom && c.ChangeDate <= dateTo && c.montage_user_id == user_id && c.ExecutorID != 0 && c.Status == DamageStatus.Closed /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                                                                                                                                                                                                                                                                                                                                                                                    //damages = damages.Where(c => c.ExecutorID != user_id).ToList();
                        return PartialView("~/Views/Damage/_bortInfo.cshtml", damages.ToPagedList(page ?? 1, 200));
                    }
                }
                if (order_status == -2)
                {
                    if (user_id != null && user_id != 0)
                    {
                        damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.MontageStatus == true && c.ExecutorID == user_id && c.Status != DamageStatus.Closed /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        return PartialView("~/Views/Damage/_bortInfo.cshtml", damages.ToPagedList(page ?? 1, 200));
                    }
                    else
                    {
                        damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.MontageStatus == true && c.Status != DamageStatus.Closed /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        return PartialView("~/Views/Damage/_bortInfo.cshtml", damages.ToPagedList(page ?? 1, 200));
                    }
                }
                if (user_id != null && user_id != 0)
                {
                    ViewBag.selectedUserFilter = user_id;
                    if (checked_user == true)
                    {
                        if (order_status != null && order_status != -1)
                        {
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.montage_user_id == user_id && c.Status == (DamageStatus)order_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        }
                        else
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.ChangeDate >= dateFrom && c.ChangeDate <= dateTo && c.montage_user_id == user_id /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                    }
                    else
                    {
                        //orders = orders.Where(o => o.ExecutorID == user_id).ToList();
                        if (order_status != null && order_status != -1)
                        {
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.ChangeDate >= dateFrom && c.ChangeDate <= dateTo && c.ExecutorID == user_id && c.Status == (DamageStatus)order_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                        }
                        else
                            damages = _db.Damages.Include("UserUser").Include("UserGroup").Include("DamageReasons.Reason").Where(c => c.ChangeDate >= dateFrom && c.ChangeDate <= dateTo && c.ExecutorID == user_id /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                    }
                }

                return PartialView("~/Views/Damage/_bortInfo.cshtml", damages.ToPagedList(page ?? 1, 200));
            }
        }
        [HttpPost]
        public PartialViewResult MenueBort(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Bort_name = _db.Users.Where(u => (u.Type == 4 || u.Type == 44) && u.Id == user_id).Select(s => s.Name).FirstOrDefault();
            }
            return PartialView("~/Views/Damage/_ShowBortInfo.cshtml");
        }

        //public async Task<ActionResult> Index(int page = 1)
        //{
        //    if (!Utils.Utils.GetPermission("DAMAGE_SHOW"))
        //    {
        //        return new RedirectResult("/Main");
        //    }

        //    DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
        //    DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
        //    using (DataContext _db = new DataContext())
        //    {
        //        ViewBag.UserGroups = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Users.Where(c => c.UserType.Name == "დაზიანება").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
        //        ViewBag.Reasons = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Reasons.Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
        //        return View(await _db.CardDamages
        //            .Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo)
        //            .OrderByDescending(c => c.Tdate).Select(c => new DamageList
        //        {
        //            Id = c.Id,
        //            Tdate = c.Tdate,
        //            GetDate = c.GetDate,
        //            ChangeDate = c.ChangeDate,
        //            City = c.Card.Customer.City,
        //            Village = c.Card.Customer.Village,
        //            Region = c.Card.Customer.Region,
        //            AbonentName = c.Card.Customer.Name + " " + c.Card.Customer.LastName,
        //            AbonentCode = c.Card.Customer.Code,
        //            AbonentNum = c.Card.AbonentNum,
        //            Status = c.Status,
        //            DamageDesc = c.Description,
        //            IsApproved = c.IsApproved,
        //            ApproveUser = c.ApproveUser,
        //            Phone = c.Card.Customer.Phone1,
        //            ChangeUser = c.ChangerUser,
        //            GroupUser = c.UserGroup.Name,
        //            User = c.User.Name
        //        }).ToPagedListAsync(page, 30));

        //    }
        //}

        public PartialViewResult GetDetailFilterModal()
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Groups = _db.Users.Where(c => c.UserType.Name == "დაზიანება").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();

                return PartialView("~/Views/Damage/_DamageFilter.cshtml");
            }
        }

        [HttpPost]
        public async Task<JsonResult> FilterDamageByName(string letter,int user_id, int page)
        {
            string column = "o.name";
            string where = column + " LIKE N'%" + letter + "%'";
            if (column == "cr.status" || column == "cr.tower_id")
                where = column + "=" + letter;
            else if (column == "c.lastname+c.name")
                where = column + " LIKE N'%" + letter + "%'";
            if (user_id != 0)
                where += " and executor_id=" + user_id;
            string sql = @"SELECT TOP(" + 20 + @") * FROM (SELECT row_number() over(ORDER BY o.id) AS row_num, o.executor_id, o.data,o.comment,o.code,o.name,o.id,o.montage_status,o.num,o.tdate,o.get_date,o.change_date,
                                 o.receivers_count,o.status,o.changer_user,o.approve_user,u.name AS create_user,u_g.name AS group_name,o.is_approved, u_exec.name as exec_name,
                                 STUFF((SELECT '<br />' + r.[text] FROM doc.DamageReasons AS rr INNER JOIN book.Reasons AS r ON r.id=rr.reason_id WHERE rr.damage_id=o.id FOR XML PATH ('')),1,1,'') AS reasons  FROM dbo.Damage AS o 
                               INNER JOIN book.Users AS u_g ON o.user_group_id=u_g.id
                               INNER JOIN book.Users AS u ON u.id=o.user_id 
          LEFT JOIN book.Users as u_exec on u_exec.id = o.executor_id
                               WHERE " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * 20);

            string sql_count = @"SELECT COUNT(d.id) FROM (SELECT row_number() over(ORDER BY o.id) AS row_num, o.executor_id, o.data,o.code,o.name,o.id,o.montage_status,o.num,o.tdate,o.get_date,o.change_date,
                                 o.receivers_count,o.status,o.changer_user,o.approve_user,u.name AS create_user,u_g.name AS group_name,o.is_approved,o.to_go, u_exec.name as exec_name,
                                 STUFF((SELECT '<br />' + r.[text] FROM doc.DamageReasons AS rr INNER JOIN book.Reasons AS r ON r.id=rr.reason_id WHERE rr.damage_id=o.id FOR XML PATH ('')),1,1,'') AS reasons  FROM dbo.Damage AS o 
                               INNER JOIN book.Users AS u_g ON o.user_group_id=u_g.id
                               INNER JOIN book.Users AS u ON u.id=o.user_id 
                               LEFT JOIN book.Users as u_exec on u_exec.id = o.executor_id
                               WHERE " + where + ") AS d ";

            System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());
            using (DataContext _db = new DataContext())
            {
                int count = await _db.Database.SqlQuery<int>(sql_count).FirstOrDefaultAsync();
                var findList = await _db.Database.SqlQuery<OrderFilter>(sql).ToRawPagedListAsync(count, page, 100);
                var execs = _db.Users.Where(u => u.Type == 4).ToList();
                var damageReserve = _db.DamageReserveAnswers.ToList();
                return Json(new
                {
                    damageReserve = damageReserve,
                    execs = execs,
                    Abonents = findList,
                    Paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, findList, p => p.ToString(), PagedListRenderOptions.PageNumbersOnly).ToHtmlString()
                });
            }
        }
        [HttpPost]
        public async Task<PartialViewResult> FilterDamages(int? group, string abonent, string abonent_num, int? status, string city, string address)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            DateTime getDatedateFrom = Utils.Utils.GetRequestDate(Request["get_date_dt_from"], true);
            DateTime getDatedateTo = Utils.Utils.GetRequestDate(Request["get_date_dt_to"], false);

            string where = string.IsNullOrEmpty(abonent) ? "" : " AND c.name+' '+c.lastname LIKE N'%" + abonent + "'";
            where += string.IsNullOrEmpty(abonent) ? "" : " AND cr.abonent_num LIKE N'%" + abonent_num + "'";
            where += !group.HasValue ? "" : " AND cd.user_group_id=" + group.Value;
            where += !status.HasValue ? "" : " AND cd.status=" + status.Value;
            where += string.IsNullOrEmpty(city) ? "" : "c.city LIKE N'%" + city + "' ";
            where += string.IsNullOrEmpty(address) ? "" : " c.address LIKE N'%" + address + "' ";
            where += " AND cd.get_date BETWEEN @get_date_from AND @get_date_to ";

            string sql = @"SELECT cd.id AS Id,cd.tdate,cd.get_date AS [GetDate],cd.change_date AS ChangeDate,c.city AS City, cr.address AS Address, c.city AS City,c.village AS village, c.region AS Region,
c.name+' '+c.lastname AS AbonentName,cr.abonent_num AS AbonentNum,c.code AS AbonentCode,cd.[desc] AS DamageDesc,is_approved AS IsApproved,cd.changer_user AS ChangeUser, cd.approve_user AS ApproveUser,
                            c.phone1 AS Phone, cd.status AS [Status], gr.name AS GroupUser, u.name AS [User] FROM dbo.Damages AS cd 
                          INNER JOIN book.Cards AS cr ON cr.id=cd.card_id 
                          INNER JOIN book.Users AS gr ON gr.id=cd.user_id
                          INNER JOIN book.Users AS u ON u.id=cd.user_id
                          INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cd.tdate BETWEEN @date_from AND @date_to " + where + " ORDER BY cd.tdate DESC";

            using (DataContext _db = new DataContext())
            {
                return PartialView("~/Views/Damage/_FilteredDamages.cshtml", await _db.Database.SqlQuery<DamageList>(sql,
                    new SqlParameter("date_from", dateFrom),
                    new SqlParameter("date_to", dateTo),
                    new SqlParameter("get_date_from", getDatedateFrom),
                    new SqlParameter("get_date_to", getDatedateTo)).ToListAsync());
            }
        }

        /*  [HttpPost]
          public async Task<PartialViewResult> FilterDamagesExport(int? group, string abonent, string abonent_num, int? status, string region, string city, string address, string dt_from, string dt_to, string get_date_dt_from, string get_date_dt_to)
          {
              string[] date_from = dt_from.Split('_');
              string[] date_to = dt_to.Split('_');
              DateTime dateFrom = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
              DateTime dateTo = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

              string[] get_date_from = get_date_dt_from.Split('_');
              string[] get_date_to = get_date_dt_to.Split('_');
              DateTime getDateFrom = DateTime.Parse(get_date_from[2] + "-" + get_date_from[1] + "-" + get_date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
              DateTime getDateTo = DateTime.Parse(get_date_to[2] + "-" + get_date_to[1] + "-" + get_date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

              string where = string.IsNullOrEmpty(abonent) ? "" : " AND c.name+' '+c.lastname LIKE N'%" + abonent + "'";
              where += string.IsNullOrEmpty(abonent) ? "" : " AND cr.abonent_num LIKE N'%" + abonent_num + "'";
              where += !group.HasValue ? "" : " AND cd.user_group_id=" + group.Value;
              where += !status.HasValue ? "" : " AND cd.status=" + status.Value;
              where += string.IsNullOrEmpty(city) ? "" : "c.city LIKE N'%" + city + "' ";
              where += string.IsNullOrEmpty(address) ? "" : " c.address LIKE N'%" + address + "' ";
              where += " AND cd.get_date BETWEEN @get_date_from AND @get_date_to ";

              string sql = @"SELECT cd.id AS Id,cd.tdate,cd.get_date AS [GetDate],cd.change_date AS ChangeDate,c.city AS City, cr.address AS Address, c.city AS City,c.village AS village, c.region AS Region,
  c.name+' '+c.lastname AS AbonentName,cr.abonent_num AS AbonentNum,c.code AS AbonentCode,cd.[desc] AS DamageDesc,is_approved AS IsApproved,cd.changer_user AS ChangeUser, cd.approve_user AS ApproveUser,
                              c.phone1 AS Phone, cd.status AS [Status], gr.name AS GroupUser, u.name AS [User] FROM doc.CardDamages AS cd 
                            INNER JOIN book.Cards AS cr ON cr.id=cd.card_id 
                            INNER JOIN book.Users AS gr ON gr.id=cd.user_id
                            INNER JOIN book.Users AS u ON u.id=cd.user_group_id
                            INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cd.tdate BETWEEN @date_from AND @date_to " + where + " ORDER BY cd.tdate DESC";

              using (DataContext _db = new DataContext())
              {
                  List<DamageList> data = await _db.Database.SqlQuery<DamageList>(sql,
                      new SqlParameter("date_from", dateFrom),
                      new SqlParameter("date_to", dateTo),
                      new SqlParameter("get_date_from", getDateFrom),
                      new SqlParameter("get_date_to", getDateTo)).ToListAsync();



              }

          }*/

        [HttpPost]
        public JsonResult ChangeDamageStatus(string ids, string status)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        int[] damages_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
                        foreach (int damage_id in damages_ids)
                        {
                            CardDamage _damage = _db.CardDamages.Where(c => c.Id == damage_id).FirstOrDefault();
                            if (_damage != null)
                            {
                                if (_damage.Status == (CardDamageStatus)Enum.Parse(typeof(CardDamageStatus), status))
                                    continue;

                                string old_val = Utils.Utils.GetEnumDescription(_damage.Status);
                                _damage.Status = (CardDamageStatus)Enum.Parse(typeof(CardDamageStatus), status);
                                _damage.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                _damage.ChangeDate = DateTime.Now;
                                _damage.IsApproved = false;
                                _damage.ApproveUser = "";
                                _db.Entry(_damage).State = EntityState.Modified;

                                this.AddLoging(_db,
                                        LogType.Card,
                                        LogMode.CardDeal,
                                        user_id,
                                        _damage.CardId,
                                        "დაზიანება - " + _db.Cards.First(c => c.Id == _damage.CardId).AbonentNum,
                                        new List<LoggingData>() { new LoggingData() { field = "სტატუსის შეცვლა", old_val = old_val, new_val = Utils.Utils.GetEnumDescription((CardDamageStatus)Enum.Parse(typeof(CardDamageStatus), status)) } }
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
            }
            return Json(1);
        }

        //[HttpPost]
        //public JsonResult DamageApprove(int id)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
        //        {
        //            try
        //            {
        //                int user_id = ((User)Session["CurrentUser"]).Id;
        //                CardDamage _damage = _db.CardDamages.Where(c => c.Id == id).FirstOrDefault();
        //                if (_damage != null)
        //                {
        //                    _damage.IsApproved = true;
        //                    _damage.ChangerUser = ((User)Session["CurrentUser"]).Name;
        //                    _db.Entry(_damage).State = EntityState.Modified;

        //                    CardLog _log = new CardLog() { CardId = _damage.CardId, Date = DateTime.Now, Status = CardLogStatus.DamageApproved, UserId = user_id };
        //                    _db.CardLogs.Add(_log);

        //                    this.AddLoging(_db,
        //                                 LogType.Card,
        //                                 LogMode.CardDeal,
        //                                 user_id,
        //                                 _damage.CardId,
        //                                 _db.Cards.First(c => c.Id == _damage.CardId).AbonentNum + " ზე დაზიანების დადასტურება",
        //                                 new List<LoggingData>()
        //                              );

        //                    _db.SaveChanges();
        //                    tran.Commit();
        //                    return Json(1);
        //                }
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
        [HttpPost]
        public JsonResult DamageApprove(int id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        Damage damage = _db.Damages.Where(c => c.Id == id).FirstOrDefault();
                        if (damage != null)
                        {
                            damage.IsApproved = true;
                            damage.ApproveUser = ((User)Session["CurrentUser"]).Name;
                            _db.Entry(damage).State = EntityState.Modified;

                            this.AddLoging(_db,
                                         LogType.Damage,
                                         LogMode.Change,
                                         user_id,
                                         damage.Id,
                                        "№ " + damage.Num + " შეკვეთის დადასტურება",
                                         new List<LoggingData>()
                                      );
                            _db.SaveChanges();

                            tran.Commit();
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
        public JsonResult CreateDamage(int card_id, int reason_id, string message)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;

                        CardDamage _damage = new CardDamage
                        {
                            CardId = card_id,
                            Description = message,
                            Tdate = DateTime.Now,
                            UserId = user_id,
                            UserGroupId = ((User)Session["CurrentUser"]).Type,
                            GetDate = DateTime.Now,
                            ChangeDate = new DateTime(2222, 12, 12),
                            Status = CardDamageStatus.Registered
                        };
                        _db.CardDamages.Add(_damage);

                        CardLog _log = new CardLog() { CardId = card_id, Date = DateTime.Now, Status = CardLogStatus.DamageFixed, UserId = user_id };
                        _db.CardLogs.Add(_log);

                        this.AddLoging(_db,
                                     LogType.Card,
                                     LogMode.CardDeal,
                                     user_id,
                                     card_id,
                                     _db.Cards.First(c => c.Id == card_id).AbonentNum + " ზე დაზიანების დაფიქსირება",
                                     new List<LoggingData>()
                                  );

                        DamageReason _reason = new DamageReason
                        {
                            DamageId = _damage.Id,
                            ReasonId = reason_id,
                            Text = message
                        };
                        _db.DamageReasons.Add(_reason);

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

        [HttpGet]
        public PartialViewResult GroupChange(int damage_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<User> Users = _db.Users.Where(o => o.Type == 4 || o.Type==44).ToList();
                ViewBag.selectedExecutor = _db.Damages.Where(o => o.Id == damage_id).FirstOrDefault().ExecutorID;
                return PartialView("~/Views/Damage/_GroupChange.cshtml", Users);

            }
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
                            Damage damage = _db.Damages.Where(c => c.Id == order_id).FirstOrDefault();
                            int old_executor_id = damage.ExecutorID;
                            if (damage != null)
                            {
                                //if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                                //    continue;

                                //if (damage.Status == DamageStatus.)
                                //    damage.Status = DamageStatus.Worked;
                                damage.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                //order.UserGroupId = group_id;
                                damage.MontageStatus = true;
                                damage.ExecutorID = group_id;
                                damage.ChangeDate = DateTime.Now;
                                damage.IsApproved = false;
                                _db.Entry(damage).State = EntityState.Modified;


                                this.AddLoging(_db,
                                                 LogType.Damage,
                                                 LogMode.Change,
                                                 user_id,
                                                 damage.Id,
                                                 "ჯგუფის შეცვლა - " + _db.Users.Where(c => c.Id == group_id).Select(c => c.Name).FirstOrDefault(),
                                                 new List<LoggingData>()
                                              );

                                _db.SaveChanges();

                                if (group_id != 0 && old_executor_id != group_id)
                                {
                                    string phoneto = _db.Users.Where(u => u.Id == group_id).FirstOrDefault().Phone;
                                    MessageTemplate message = _db.MessageTemplates.Where(m => m.Name == "OnDamageAttach_Geo").FirstOrDefault();
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

        //[HttpPost]
        //public JsonResult GroupChange(string ids, int group_id)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
        //        {
        //            try
        //            {
        //                int user_id = ((User)Session["CurrentUser"]).Id;
        //                int[] damage_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
        //                foreach (int damage_id in damage_ids)
        //                {
        //                    CardDamage damage = _db.CardDamages.Where(c => c.Id == damage_id).FirstOrDefault();
        //                    if (damage != null)
        //                    {

        //                        if (damage.Status == CardDamageStatus.Registered)
        //                            damage.Status = CardDamageStatus.Worked;
        //                        damage.ChangerUser = ((User)Session["CurrentUser"]).Name;
        //                        damage.UserGroupId = group_id;
        //                        damage.ChangeDate = DateTime.Now;
        //                        damage.IsApproved = false;
        //                        _db.Entry(damage).State = EntityState.Modified;

        //                        this.AddLoging(_db,
        //                                     LogType.Card,
        //                                     LogMode.Change,
        //                                     user_id,
        //                                     damage.Id,
        //                                     "ბარათის დაზიანება, ჯგუფის შეცვლა - " + _db.Users.Where(c => c.Id == group_id).Select(c => c.Name).FirstOrDefault(),
        //                                     new List<LoggingData>()
        //                                  );
        //                        _db.SaveChanges();
        //                    }
        //                }
        //                tran.Commit();
        //            }
        //            catch
        //            {
        //                tran.Rollback();
        //                return Json(0);
        //            }
        //        }
        //    }
        //    return Json(1);
        //}
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
                            Damage damage = _db.Damages.Where(c => c.Id == order_id).FirstOrDefault();
                            if (damage != null)
                            {
                                if (damage.Status == DamageStatus.Closed || damage.Status == DamageStatus.Processing)
                                    continue;

                                //if (damage.Status == OrderStatus.Registered)
                                //damage.Status = OrderStatus.Worked;
                                damage.GetDate = dateFrom;
                                damage.ChangeDate = DateTime.Now;
                                damage.IsApproved = false;
                                damage.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                _db.Entry(damage).State = EntityState.Modified;

                                int user_id = ((User)Session["CurrentUser"]).Id;
                                this.AddLoging(_db,
                                                 LogType.Order,
                                                 LogMode.Change,
                                                 user_id,
                                                 damage.Id,
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
        //[HttpPost]
        //public JsonResult ChangeDate(string ids, string date)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
        //        {
        //            try
        //            {
        //                int[] damages_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
        //                foreach (int damage_id in damages_ids)
        //                {
        //                    DateTime dateFrom = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(5, 2)), int.Parse(date.Substring(8, 2)), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        //                    CardDamage damage = _db.CardDamages.Where(c => c.Id == damage_id).FirstOrDefault();
        //                    if (damage != null)
        //                    {
        //                        if (damage.Status == CardDamageStatus.Registered)
        //                            damage.Status = CardDamageStatus.Worked;
        //                        string old_val = damage.GetDate.ToString("dd/MM/yyyy HH:mm");
        //                        damage.GetDate = dateFrom;
        //                        damage.ChangeDate = DateTime.Now;
        //                        damage.IsApproved = false;
        //                        damage.ChangerUser = ((User)Session["CurrentUser"]).Name;
        //                        _db.Entry(damage).State = EntityState.Modified;

        //                        int user_id = ((User)Session["CurrentUser"]).Id;
        //                        this.AddLoging(_db,
        //                                         LogType.Order,
        //                                         LogMode.Change,
        //                                         user_id,
        //                                         damage.Id,
        //                                         "დაზიანების მისვლის თარიღის შეცვლა - " + dateFrom.ToString("dd/MM/yyyy HH:mm"),
        //                                         new List<LoggingData>() { new LoggingData { field = "თარიღის შეცვლა", new_val = dateFrom.ToString("dd/MM/yyyy HH:mm"), old_val = old_val } }
        //                                      );

        //                        _db.SaveChanges();

        //                    }
        //                }

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
        [NonAction]
        public bool DamageCancelStatic(int id, int user_id, string comment)
        {
            using (IDbConnection _db = new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString))
            {
                var sql = "INSERT INTO[dbo].[DamageReserveAnswer] ([tdate] ,[damage_id],[reserve_answer] ,[user_id]) VALUES (@tdate ,@damage_id ,@reserve_answer,@user_id)";
                _db.Execute(sql, new
                {
                    tdate = DateTime.Now,
                    damage_id = id,
                    reserve_answer = comment,
                    user_id = user_id
                });
            }

            return true;
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
                        // var userBort = _db.Users.Where(c => c.Type == 4).ToList();
                        int[] order_ids = ids.Split(',').Select(c => int.Parse(c)).ToArray();
                        foreach (int order_id in order_ids)
                        {
                            Damage damage = _db.Damages.Where(c => c.Id == order_id).FirstOrDefault();
                            if (damage != null)
                            {
                                //order.ExecutorID = executerID != null ? Convert.ToInt32(executerID) : 0;
                                //_db.Entry(damage).State = EntityState.Modified;
                                //_db.SaveChanges();

                                if (damage.Status == (DamageStatus)Enum.Parse(typeof(DamageStatus), status))
                                    continue;
                                OperatorGroupUser _groupUser = new OperatorGroupUser()
                                {

                                    d_id = damage.Id,
                                    name = ((User)Session["CurrentUser"]).Name.ToString()
                                };
                                _db.OperatorGroupUsers.Add(_groupUser);
                                _db.SaveChanges();
                                string old_val = Utils.Utils.GetEnumDescription(damage.Status);
                                damage.Status = (DamageStatus)Enum.Parse(typeof(DamageStatus), status);
                                damage.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                damage.IsApproved = false;
                                damage.ApproveUser = "";
                                damage.ChangeDate = DateTime.Now;
                                damage.MontageStatus = false;
                                // var _user = userBort.Where(c => c.Id == user_id).Select(s => s).FirstOrDefault();
                                if (status == "Closed")
                                {
                                     damage.ExecutorID = 0;

                                    var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                                    context.Clients.All.onHitRecorded("DamageStatusClosed", user_id);
                                }
                                _db.Entry(damage).State = EntityState.Modified;
                                _db.SaveChanges();

                                this.AddLoging(_db,
                                                 LogType.Damage,
                                                 LogMode.Change,
                                                 user_id,
                                                 damage.Id,
                                                 damage.Num.ToString(),
                                                 new List<LoggingData>() { new LoggingData() { field = "სტატუსის შეცვლა", old_val = old_val, new_val = Utils.Utils.GetEnumDescription((DamageStatus)Enum.Parse(typeof(DamageStatus), status)) } }
                                              );

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

        [HttpPost]
        public JsonResult SaveReason(int id, int reason_id, string desc)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        CardDamage damage = _db.CardDamages.Where(c => c.Id == id).FirstOrDefault();
                        if (damage != null)
                        {
                            DamageReason _reason = new DamageReason()
                            {
                                DamageId = id,
                                ReasonId = reason_id,
                                Text = desc
                            };
                            _db.DamageReasons.Add(_reason);
                            _db.SaveChanges();
                            tran.Commit();
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
        public JsonResult SaveDamageServices(int id, List<CardService> services)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        string abonent_num = _db.Cards.Where(c => c.Id == id).Select(c => c.AbonentNum).FirstOrDefault();
                        List<int> serv_ids = new List<int>();
                        foreach (CardService _serv in services)
                        {
                            _serv.Date = DateTime.Now;
                            _serv.CardId = _db.CardDamages.Where(c => c.Id == id).Select(c => c.CardId).FirstOrDefault();
                            _serv.IsActive = _serv.PayType == CardServicePayType.NotCash;

                            serv_ids.Add(_serv.ServiceId);
                        }
                        _db.CardServices.AddRange(services);
                        this.AddLoging(_db,
                                    LogType.CardService,
                                    LogMode.Add,
                                    user_id,
                                    id,
                                    abonent_num + " - ის დაზიანების მომსახურება",
                                    _db.Services.Where(c => serv_ids.Contains(c.Id)).Select(c => new LoggingData { field = "მომსახურება", new_val = c.Name }).ToList()
                                );

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
        public JsonResult SendSMS(int[] abonents, string message)
        {
            using (DataContext _db = new DataContext())
            {
                List<Param> Params = _db.Params.ToList();

                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;

                var orders = _db.CardDamages.Where(o => abonents.Contains(o.Id)).Select(o => new { Phone = o.Card.Customer.Phone1, Name = o.Card.Customer.Name + " " + o.Card.Customer.LastName }).ToList();
                if (Utils.Utils.OnSendSMS(orders.Select(o => o.Phone).ToList(), message, username, password, _db))
                {
                    int user_id = ((User)Session["CurrentUser"]).Id;
                    this.AddLoging(_db,
                                    LogType.Message,
                                    LogMode.Add,
                                    user_id,
                                    0,
                                    "SMS ის გაგზავნა დაზიანებებიდან",
                                    orders.Select(c => new LoggingData { field = "აბონენტი", new_val = c.Name }).ToList()
                              );

                    return Json("შეტყობინება წარმატებით გაიგზავნა");
                }
                else
                {
                    return Json("შეტყობინება ვერ გაიგზავნა");
                }
            }
        }
        [HttpPost]
        public JsonResult CheckCustomer(string code)
        {
            using (DataContext _db = new DataContext())
            {
                int hasOrder = _db.Damages.Where(o => o.Code == code).Select(o => o.Num).FirstOrDefault();
                if (hasOrder != 0)
                    return Json(hasOrder);

                Customer _cust = _db.Customers.Where(c => c.Code == code).FirstOrDefault();
                if (_cust == null)
                {
                    _cust = new Customer
                    {
                        Name = new RsServiceFuncs(true).GetAbonentName(code)
                    };
                }
                return Json(_cust);
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
                    Damage _damage = _db.Damages.Where(c => c.Id == id.Value).FirstOrDefault();
                    if (_damage != null)
                    {
                        return View(new Damage()
                        {
                            Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Abonent>(_damage.Data).Customer,
                            GetDate = _damage.GetDate,
                            ReceiversCount = _damage.ReceiversCount,
                        });
                    }
                }
            }

            return View(new Damage() { Customer = new Customer(), GetDate = DateTime.Now, ReceiversCount = 1 });
        }
        [HttpPost]
        public ActionResult New(int? id, Damage damage)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                {
                    try
                    {
                        int motage_userID = _db.Damages.Where(c => c.Code == damage.Customer.Code && c.ExecutorID != 0).OrderByDescending(ss => ss.Id).Select(s => s.ExecutorID).FirstOrDefault();
                        if (motage_userID == 0)
                            motage_userID = _db.Customers.Where(c => c.Code == damage.Customer.Code).Select(s => s.UserId).FirstOrDefault();
                        //motage_userID = _db.Damages.Where(c => c.Code == damage.Customer.Code && c.ExecutorID != 0).OrderByDescending(ss=>ss.Id).Select(s => s.ExecutorID).FirstOrDefault();
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(new Abonent() { Customer = damage.Customer });

                        if (id.HasValue && id.Value > 0)
                        {
                            Damage _damage = _db.Damages.Where(c => c.Id == id.Value).FirstOrDefault();
                            if (_damage != null)
                            {
                                _damage.Name = damage.Customer.Name + " " + damage.Customer.LastName + "/" + damage.Customer.Phone1;
                                _damage.Code = damage.Customer.Code;
                                _damage.GetDate = damage.GetDate;
                                _damage.Data = data;
                                //_damage.montage_user_id = motage_userID;
                                _damage.ReceiversCount = damage.ReceiversCount;
                                _damage.Address = damage.Customer.Address;
                                _db.Entry(_damage).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            Models.Damage _damage = new Damage
                            {
                                Id = 0,
                                Status = 0,// damage.Status,
                                Name = damage.Customer.Name + " " + damage.Customer.LastName + "/" + damage.Customer.Phone1,
                                Code = damage.Customer.Code,
                                GetDate = damage.GetDate,
                                Data = data,
                                UserId = user_id,
                                ChangeDate = new DateTime(2222, 12, 12),
                                Tdate = DateTime.Now,
                                ReceiversCount = damage.ReceiversCount,
                                Address = damage.Customer.Address,
                                montage_user_id = motage_userID,
                                UserGroupId = 1,
                            };

                            _damage.Num = (_db.Orders.Max(x => (int?)x.Num) ?? 0) + 1;
                            _db.Damages.Add(_damage);

                            this.AddLoging(_db,
                                LogType.Damage,
                                LogMode.Add,
                                user_id,
                                _damage.Id,
                                _damage.Name,
                                new List<LoggingData>());
                        }

                        _db.SaveChanges();

                        var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>();
                        context.Clients.All.onHitRecorded("DamageNew", user_id);


                        tran.Commit();
                        return Redirect("/Damage");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                    }
                }
                ViewBag.UserGroups = _db.Users.Where(c => c.UserType.Name == "მემონტაჟეები").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();
            }

            ViewBag.Error = true;
            return View(damage);
        }
        [NonAction]
        public bool __DamageApprove(int id, int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        //int user_id = ((User)Session["CurrentUser"]).Id;
                        Damage order = _db.Damages.Where(c => c.Id == id).FirstOrDefault();
                        if (order != null)
                        {
                            order.ExecutorID = user_id != null ? Convert.ToInt32(user_id) : 0;
                            _db.Entry(order).State = EntityState.Modified;
                            _db.SaveChanges();



                            //if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                            //    continue;

                            string old_val = Utils.Utils.GetEnumDescription(order.Status);
                            order.Status = DamageStatus.Closed;
                            order.ChangerUser = _db.Users.Where(u => u.Id == user_id).FirstOrDefault().Name;// ((User)Session["CurrentUser"]).Name;
                            order.IsApproved = false;
                            order.MontageStatus = false;
                            //order.montage_user_id = user_id;
                            order.ApproveUser = "";
                            order.ChangeDate = DateTime.Now;
                            order.to_go = 0;
                            _db.Entry(order).State = EntityState.Modified;


                            this.AddLoging(_db,
                                             LogType.Order,
                                             LogMode.Change,
                                             user_id,
                                             order.Id,
                                             order.Num.ToString(),
                                             new List<LoggingData>() { new LoggingData() { field = "სტატუსის შეცვლა", old_val = old_val, new_val = Utils.Utils.GetEnumDescription(OrderStatus.Closed) } }
                                          );

                            _db.SaveChanges();
                            tran.Commit();

                            var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>(); //
                            context.Clients.All.onHitRecorded("Damage", user_id);
                            context.Clients.All.onHitRecorded("RegionClosed",
                             new RegionGoName(
                                 new SqlConnection(
                                             ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                   order.Id,
                                   "dbo.Damage",
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
        [NonAction]
        public bool DamageComment(int id, int user_id, string comment)
        {
            using (DataContext _db = new DataContext())
            {
                if (comment != "")
                {
                    var _damage = _db.Damages.Where(c => c.Id == id).FirstOrDefault();
                    _damage.comment = comment;
                    _db.Entry(_damage).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                return true;
            }
        }

        [NonAction]
        public string DamageWriteComment(int id)
        {
            using (DataContext _db = new DataContext())
            {
                var _damage = _db.Database.SqlQuery<string>("SELECT d.comment FROM dbo.Damage d where d.id=" + id).FirstOrDefault();
                if (_damage == null)
                {
                    _damage = "";
                }
                return _damage;
            }
        }

    }
}