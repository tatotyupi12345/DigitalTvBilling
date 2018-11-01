using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using PagedList;
using System.Web.Mvc;
using DigitalTVBilling.ListModels;
using System.Data;
using System.Data.SqlClient;
using DigitalTVBilling.Filters;
using System.Xml.Linq;
using System.Globalization;
using PagedList.Mvc;
using Microsoft.AspNet.SignalR;
using DigitalTVBilling.CallCenter;
using DigitalTVBilling.CallCenter.Infrastructure;
using System.Configuration;
using Dapper;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class OrderController : BaseController
    {
        public async Task<ActionResult> Index(int? user_id, int? promo_id, int? order_status, int? CallCentrPoll, int? user_answers, int page = 1)
        {
            if (!Utils.Utils.GetPermission("ORDER_SHOW"))
            {
                return new RedirectResult("/Main");
            }

            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            //string userid = null;//.ToString();
            //if(Request["user_id"] != null)
            //    userid = Request["user_id"].ToString();
            //int user_id = 0;
            //if (userid != null && userid != "")
            //user_id = Convert.ToInt32(userid);

            using (DataContext _db = new DataContext())
            {
                var m=_db.Orders.Include("OrderReserveAnswers").ToList();
                //var oo = _db.OrderReserveAnswers.ToList();
               
                ViewBag.selectedUserFilter = 0;
                ViewBag.selectedpromoFilter = 0;
                ViewBag.UserGroups = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Users.Where(c => c.UserType.Name == "მემონტაჟეები").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
                ViewBag.Reasons = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Reasons.Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList());
                ViewBag.ExecutorUsers = _db.Users.Where(u => u.Type == 4 || u.Type == 44).ToList();
                ViewBag.Promo = _db.Users.Where(u => u.Type == 43).ToList();
                ViewBag.selectedStatus = order_status == null ? -1 : order_status;
                Task<IPagedList<Order>> orders = null;

                //var orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).OrderByDescending(c => c.Num).ToList();//.ToPagedListAsync(page, 20);
                if (promo_id != null && promo_id != 0)
                {
                    ViewBag.selectedUserFilter = user_id;
                    //orders = orders.Where(o => o.ExecutorID == user_id).ToList();
                    if (order_status != null && order_status != -1)
                    {
                        orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.UserId == promo_id && c.Status == (OrderStatus)order_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                    }
                    else
                         if (CallCentrPoll != null && CallCentrPoll != 0)
                        orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.UserId == promo_id && c.Poll == (CallCentr)CallCentrPoll /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                    else

                        orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.UserId == promo_id  /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                    return View(await orders);
                }
                if (user_id != null && user_id != 0)
                {
                    ViewBag.selectedUserFilter = user_id;
                    //orders = orders.Where(o => o.ExecutorID == user_id).ToList();
                    if (order_status != null && order_status != -1)
                    {
                        orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == user_id && c.Status == (OrderStatus)order_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                    }
                    else
                    {
                        if (CallCentrPoll != null && CallCentrPoll != 0)
                            orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == user_id && c.Poll == (CallCentr)CallCentrPoll /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                        else

                            orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == user_id  /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                        if (user_answers != null && user_answers != 0)
                        {
                            orders = _db.Orders.Include("OrderReserveAnswers").Include("UserUser").Include("UserGroup").Where(c => c.OrderReserveAnswers.Any(cc => cc.tdate >= dateFrom && cc.tdate <= dateTo && cc.user_id==user_id && cc.reserve_answer == (OrderCommitStatic)user_answers)).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                        }
                    }
                }
                else
                {
                    if (order_status != null && order_status != -1)
                        orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == c.ExecutorID && c.Status == (OrderStatus)order_status /*((user_id != null || user_id != 0? user_id: c.ExecutorID))*/).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                    else
                    {

                        if (CallCentrPoll != null && CallCentrPoll != 0)

                            orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == c.ExecutorID && c.Poll == (CallCentr)CallCentrPoll).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                        else
                        {
                            if (user_answers != null && user_answers != 0)
                            {
                                orders = _db.Orders.Include("OrderReserveAnswers").Include("UserUser").Include("UserGroup").Where(c => c.OrderReserveAnswers.Any(cc=>cc.tdate>=dateFrom && cc.tdate<=dateTo && cc.reserve_answer==(OrderCommitStatic)user_answers)).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                            }
                            else
                                orders = _db.Orders.Include("UserUser").Include("UserGroup").Include("OrderReasons.Reason").Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo && c.ExecutorID == c.ExecutorID).OrderByDescending(c => c.Num).ToPagedListAsync(page, 20);
                        }
                    }
                }

                return View(await orders);
            }
        }

        [HttpPost]
        public async Task<JsonResult> FilterOrdersByName(string letter, int user_id, int page)
        {
            string column = "o.name";
            string where = column + " LIKE N'%" + letter + "%'";
            if (column == "cr.status" || column == "cr.tower_id")
                where = column + "=" + letter;
            else if (column == "c.lastname+c.name")
                where = column + " LIKE N'%" + letter + "%'";
            if (user_id != 0)
                where += " and executor_id=" + user_id;
            //where = where.Replace("+", "+' '+");
            //string sql = @"SELECT TOP(" + 20 + @") d.id AS Id,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.type AS Type,d.city AS City, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status, d.doc_num AS DocNum, d.pack AS ActivePacket 
            //             FROM (SELECT row_number() over(ORDER BY c.id) AS row_num,c.id,c.name,c.lastname,c.code,c.[type],c.city,c.phone1, cr.doc_num, cr.abonent_num,cr.card_num, cr.status,
            //             STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
            //             INNER JOIN book.Customers AS c ON c.id=cr.customer_id
            //             LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1
            //             WHERE " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * 20);

            string sql = @"SELECT TOP(" + 20 + @") * FROM (SELECT row_number() over(ORDER BY o.id) AS row_num, o.executor_id, o.data,o.code,o.name,o.comment,o.id,o.montage_status,o.to_go,o.num,o.tdate,o.get_date,o.change_date,
                                 o.receivers_count,o.status,o.changer_user,o.approve_user,u.name AS create_user,u_g.name AS group_name,o.is_approved, u_exec.name as exec_name,
                                 STUFF((SELECT '<br />' + r.[text] FROM doc.OrderReasons AS rr INNER JOIN book.Reasons AS r ON r.id=rr.reason_id WHERE rr.order_id=o.id FOR XML PATH ('')),1,1,'') AS reasons  FROM doc.Orders AS o 
                               INNER JOIN book.Users AS u_g ON o.user_group_id=u_g.id
                               INNER JOIN book.Users AS u ON u.id=o.user_id 
							   LEFT JOIN book.Users as u_exec on u_exec.id = o.executor_id
                               WHERE " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * 20);

            string sql_count = @"SELECT COUNT(d.id) FROM (SELECT row_number() over(ORDER BY o.id) AS row_num, o.executor_id, o.data,o.code,o.name,o.id,o.montage_status,o.to_go,o.num,o.tdate,o.get_date,o.change_date,
                                 o.receivers_count,o.status,o.changer_user,o.approve_user,u.name AS create_user,u_g.name AS group_name,o.is_approved, u_exec.name as exec_name,
                                 STUFF((SELECT '<br />' + r.[text] FROM doc.OrderReasons AS rr INNER JOIN book.Reasons AS r ON r.id=rr.reason_id WHERE rr.order_id=o.id FOR XML PATH ('')),1,1,'') AS reasons  FROM doc.Orders AS o 
                               INNER JOIN book.Users AS u_g ON o.user_group_id=u_g.id
                               INNER JOIN book.Users AS u ON u.id=o.user_id 
							   LEFT JOIN book.Users as u_exec on u_exec.id = o.executor_id
                               WHERE " + where + ") AS d ";

            System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());
            using (DataContext _db = new DataContext())
            {
                int count = await _db.Database.SqlQuery<int>(sql_count).FirstOrDefaultAsync();
                var findList = await _db.Database.SqlQuery<OrderFilter>(sql).ToRawPagedListAsync(count, page, 20);
                var execs = _db.Users.Where(u => u.Type == 4).ToList();
                var ordeReserve = _db.OrderReserveAnswers.ToList();
                return Json(new
                {
                    ordeReserve= ordeReserve,
                    execs = execs,
                    Abonents = findList,
                    Paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, findList, p => p.ToString(), PagedListRenderOptions.PageNumbersOnly).ToHtmlString()
                });
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> FilterOrders(int? group, string abonent, int? status, string region, string city, string district)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            DateTime getDatedateFrom = Utils.Utils.GetRequestDate(Request["get_date_dt_from"], true);
            DateTime getDatedateTo = Utils.Utils.GetRequestDate(Request["get_date_dt_to"], false);

            string where = string.IsNullOrEmpty(abonent) ? "" : " AND o.name LIKE N'%" + abonent + "'";
            where += !group.HasValue ? "" : " AND user_group_id=" + group.Value;
            where += !status.HasValue ? "" : " AND status=" + status.Value;
            where += string.IsNullOrEmpty(region) ? "" : " AND EXISTS (select TOP(1) * from parseJSON(o.data) WHERE NAME='REGION' AND StringValue LIKE N'%" + region + "')";
            where += string.IsNullOrEmpty(city) ? "" : " AND EXISTS (select TOP(1) * from parseJSON(o.data) WHERE NAME='CITY' AND StringValue LIKE N'%" + city + "')";
            where += string.IsNullOrEmpty(district) ? "" : " AND EXISTS (select TOP(1) * from parseJSON(o.data) WHERE NAME='DISTRICT' AND StringValue LIKE N'%" + district + "')";
            where += " AND o.get_date BETWEEN @get_date_from AND @get_date_to ";

            string sql = @"SELECT o.data,o.code,o.name,o.id,o.montage_status,o.num,o.tdate,o.get_date,o.change_date,
                                 o.receivers_count,o.status,o.changer_user,o.approve_user,u.name AS create_user,u_g.name AS group_name,o.is_approved,
                                 STUFF((SELECT '<br />' + r.[text] FROM doc.OrderReasons AS rr INNER JOIN book.Reasons AS r ON r.id=rr.reason_id WHERE rr.order_id=o.id FOR XML PATH ('')),1,1,'') AS reasons  FROM doc.Orders AS o 
                               INNER JOIN book.Users AS u_g ON o.user_group_id=u_g.id
                               INNER JOIN book.Users AS u ON u.id=o.user_id 
                               WHERE o.tdate BETWEEN @date_from AND @date_to " + where;

            using (DataContext _db = new DataContext())
            {
                return PartialView("~/Views/Order/_FilteredOrders.cshtml", await _db.Database.SqlQuery<OrderFilter>(sql,
                    new SqlParameter("date_from", dateFrom),
                    new SqlParameter("date_to", dateTo),
                    new SqlParameter("get_date_from", dateFrom),
                    new SqlParameter("get_date_to", dateTo)).ToListAsync());
            }
        }

        public async Task<FileResult> FilterOrdersExport(int? group, string abonent, int? status, string region, string city, string district, string dt_from, string dt_to, string get_date_dt_from, string get_date_dt_to)
        {
            string[] date_from = dt_from.Split('_');
            string[] date_to = dt_to.Split('_');
            DateTime dateFrom = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
            DateTime dateTo = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

            string[] get_date_from = get_date_dt_from.Split('_');
            string[] get_date_to = get_date_dt_to.Split('_');
            DateTime getDateFrom = DateTime.Parse(get_date_from[2] + "-" + get_date_from[1] + "-" + get_date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
            DateTime getDateTo = DateTime.Parse(get_date_to[2] + "-" + get_date_to[1] + "-" + get_date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

            string where = abonent == "" ? "" : " AND o.name LIKE N'%" + abonent + "'";
            where += !group.HasValue ? "" : " AND user_group_id=" + group.Value;
            where += !status.HasValue ? "" : " AND status=" + status.Value;
            where += string.IsNullOrEmpty(region) ? "" : " AND EXISTS (select TOP(1) * from parseJSON(o.data) WHERE NAME='REGION' AND StringValue LIKE N'%" + region + "')";
            where += string.IsNullOrEmpty(city) ? "" : " AND EXISTS (select TOP(1) * from parseJSON(o.data) WHERE NAME='CITY' AND StringValue LIKE N'%" + city + "')";
            where += string.IsNullOrEmpty(district) ? "" : " AND EXISTS (select TOP(1) * from parseJSON(o.data) WHERE NAME='DISTRICT' AND StringValue LIKE N'%" + district + "')";
            where += " AND o.get_date BETWEEN @get_date_from AND @get_date_to ";


            string sql = @"SELECT u.name AS create_user,
                        ((select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='NAME') + ' ' +
                        (select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='LASTNAME')) AS abonent_name,
                        (select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='CITY') AS city,
                        (select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='VILLAGE') AS raion,
                        (select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='REGION') AS region,
                        ISNULL((select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='DISTRICT'),'') AS district,
                        (select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='ADDRESS') AS address,
                        (select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='PHONE1') AS phone1,
                        ISNULL((select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='PHONE2'),'') AS phone2,
                        o.get_date,
                        o.receivers_count,
                        o.tdate,
                        ISNULL((select TOP(1) StringValue from parseJSON(o.data) WHERE NAME='COMMENT'),'') AS comment FROM doc.Orders AS o 
                                                        INNER JOIN book.Users AS u_g ON o.user_group_id=u_g.id
                                                        INNER JOIN book.Users AS u ON u.id=o.user_id 
                                                        WHERE o.tdate BETWEEN @date_from AND @date_to " + where;

            using (DataContext _db = new DataContext())
            {
                List<OrderExport> data = await _db.Database.SqlQuery<OrderExport>(sql,
                    new SqlParameter("date_from", dateFrom),
                    new SqlParameter("date_to", dateTo),
                    new SqlParameter("get_date_from", getDateFrom),
                    new SqlParameter("get_date_to", getDateTo)).ToListAsync();

                XElement element = new XElement("root",
                   new XElement("columns",
                       new XElement("name", "ოპერატორის გვარი/სახელი"),
                       new XElement("name", "აბონენტის გვარი/სახელი"),
                       new XElement("name", "ქალაქი/სოფელი"),
                       new XElement("name", "რაიონი"),
                       new XElement("name", "რეგიონი"),
                       new XElement("name", "უბანი"),
                       new XElement("name", "აბონენტის მისამართი"),
                       new XElement("name", "ტელეფონი 1"),
                       new XElement("name", "ტელეფონი 2"),
                       new XElement("name", "მომხმარებელთან მისვლის თარიღი"),
                       new XElement("name", "რესივერების რაოდენობა"),
                       new XElement("name", "შეტყობინების თარიღი"),
                       new XElement("name", "კომენტარი")),
                   data.Select(c => new XElement("data",
                       new XElement("create_user", c.create_user),
                       new XElement("abonent_name", c.abonent_name),
                       new XElement("city", c.city),
                       new XElement("raion", c.raion),
                       new XElement("region", c.region),
                       new XElement("district", c.district == "null" ? "" : c.district),
                       new XElement("address", c.address),
                       new XElement("phone1", c.phone1),
                       new XElement("phone2", c.phone2 == "null" ? "" : c.phone2),
                       new XElement("get_date", c.get_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)),
                       new XElement("receivers_count", c.receivers_count),
                       new XElement("tdate", c.tdate.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)),
                       new XElement("comment", c.comment)
                       )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
            }

        }

        public PartialViewResult GetDetailFilterModal()
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Groups = _db.Users.Where(c => c.UserType.Name == "მემონტაჟეები").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();

                return PartialView("~/Views/Order/_OrderFilter.cshtml");
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
                    Order _order = _db.Orders.Where(c => c.Id == id.Value).FirstOrDefault();
                    if (_order != null)
                    {
                        return View(new Order()
                        {
                            Customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Abonent>(_order.Data).Customer,
                            GetDate = _order.GetDate,
                            ReceiversCount = _order.ReceiversCount,
                        });
                    }
                }
            }

            return View(new Order() { Customer = new Customer(), GetDate = DateTime.Now, ReceiversCount = 1 });
        }

        [HttpPost]
        public ActionResult New(int? id, Order order)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(new Abonent() { Customer = order.Customer });

                        if (id.HasValue && id.Value > 0)
                        {
                            Order _order = _db.Orders.Where(c => c.Id == id.Value).FirstOrDefault();
                            if (_order != null)
                            {
                                _order.Name = order.Customer.Name + " " + order.Customer.LastName + "/" + order.Customer.Phone1;
                                _order.Code = order.Customer.Code;
                                _order.GetDate = order.GetDate;
                                _order.Data = data;
                                _order.ReceiversCount = order.ReceiversCount;
                                _order.Address = order.Customer.Address;
                                _db.Entry(_order).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            Models.Order _order = new Order
                            {
                                Status = OrderStatus.Registered,
                                Name = order.Customer.Name + " " + order.Customer.LastName + "/" + order.Customer.Phone1,
                                Code = order.Customer.Code,
                                GetDate = order.GetDate,
                                Data = data,
                                UserId = user_id,
                                ChangeDate = new DateTime(2222, 12, 12),
                                Tdate = DateTime.Now,
                                ReceiversCount = order.ReceiversCount,
                                Address = order.Customer.Address,
                                Poll = order.Poll,
                                UserGroupId = 1,
                            };

                            _order.Num = (_db.Orders.Max(x => (int?)x.Num) ?? 0) + 1;
                            _db.Orders.Add(_order);

                            this.AddLoging(_db,
                                LogType.Order,
                                LogMode.Add,
                                user_id,
                                _order.Id,
                                _order.Name,
                                new List<LoggingData>());
                        }


                        _db.SaveChanges();
                        var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>();
                        context.Clients.All.onHitRecorded("OrderNew", user_id);

                        tran.Commit();


                        return Redirect("/Order");
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
                ViewBag.UserGroups = _db.Users.Where(c => c.UserType.Name == "მემონტაჟეები").Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();
            }

            ViewBag.Error = true;
            return View(order);
        }
        [HttpPost]
        public JsonResult NewOrder(Order order)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                {
                    try
                    {
                        //int user_id = ((User)Session["CurrentUser"]).Id;
                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(new Abonent() { Customer = order.Customer });
                        Models.Order _order = new Order
                        {
                            Status = OrderStatus.Registered,
                            Name = order.Customer.Name + " " + order.Customer.LastName + "/" + order.Customer.Phone1,
                            Code = order.Customer.Code,
                            GetDate = DateTime.Now,
                            Data = data,
                            UserId = order.UserId,
                            ChangeDate = new DateTime(2222, 12, 12),
                            Tdate = DateTime.Now,
                            ReceiversCount = order.ReceiversCount,
                            Address = order.Customer.Address,
                            UserGroupId = 1,
                        };
                        _order.Num = (_db.Orders.Max(x => (int?)x.Num) ?? 0) + 1;
                        _db.Orders.Add(_order);

                        //this.AddLoging(_db,
                        //    LogType.Order,
                        //    LogMode.Add,
                        //    order.Customer.UserId,
                        //    _order.Id,
                        //    _order.Name,
                        //    new List<LoggingData>());

                        _db.SaveChanges();
                        tran.Commit();
                        var phone = _db.Users.Where(u => u.Id == order.UserId).Select(s => s).FirstOrDefault();
                        if (phone.Type == 43)
                            Task.Run(async () => { await Utils.Utils.sendMessage(phone.Phone, "Tqveni shekveta migebulia"); }).Wait();
                        return Json(0);
                    }
                    catch
                    {
                        tran.Rollback();

                    }
                }
            }
            return Json(1);
        }
        [HttpGet]
        public PartialViewResult GroupChange(int order_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<User> Users = _db.Users.Where(o => o.Type == 4 || o.Type == 44).ToList();
                ViewBag.selectedExecutor = _db.Orders.Where(o => o.Id == order_id).FirstOrDefault().ExecutorID;
                return PartialView("~/Views/Order/_GroupChange.cshtml", Users);

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
                            Order order = _db.Orders.Where(c => c.Id == order_id).FirstOrDefault();
                            int old_executor_id = order.ExecutorID; ;
                            if (order != null)
                            {
                                //if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                                //    continue;

                                if (order.Status == OrderStatus.Registered)
                                    order.Status = OrderStatus.Worked;
                                order.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                //order.UserGroupId = group_id;
                                order.ExecutorID = group_id;
                                order.ChangeDate = DateTime.Now;
                                order.IsApproved = false;
                                _db.Entry(order).State = EntityState.Modified;


                                this.AddLoging(_db,
                                                 LogType.Order,
                                                 LogMode.Change,
                                                 user_id,
                                                 order.Id,
                                                 "ჯგუფის შეცვლა - " + _db.Users.Where(c => c.Id == group_id).Select(c => c.Name).FirstOrDefault(),
                                                 new List<LoggingData>()
                                              );

                                _db.SaveChanges();

                                if (group_id != 0 && old_executor_id != group_id)
                                {
                                    string phoneto = _db.Users.Where(u => u.Id == group_id).FirstOrDefault().Phone;
                                    MessageTemplate message = _db.MessageTemplates.Where(m => m.Name == "OnOrderAttach_GEO").FirstOrDefault();
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
        public JsonResult SaveReason(int id, int reason_id, string desc)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        Order order = _db.Orders.Where(c => c.Id == id).FirstOrDefault();
                        if (order != null)
                        {
                            OrderReason _reason = new OrderReason()
                            {
                                OrderId = id,
                                ReasonId = reason_id,
                                Text = desc
                            };
                            _db.OrderReasons.Add(_reason);
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
        public JsonResult OrderApprove(int id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        Order order = _db.Orders.Where(c => c.Id == id).FirstOrDefault();
                        if (order != null)
                        {
                            order.IsApproved = true;
                            order.ApproveUser = ((User)Session["CurrentUser"]).Name;
                            _db.Entry(order).State = EntityState.Modified;

                            this.AddLoging(_db,
                                         LogType.Order,
                                         LogMode.Change,
                                         user_id,
                                         order.Id,
                                        "№ " + order.Num + " შეკვეთის დადასტურება",
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

        [NonAction]
        public bool __OrderApprove(int id, int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        //int user_id = ((User)Session["CurrentUser"]).Id;
                        Order order = _db.Orders.Where(c => c.Id == id).FirstOrDefault();
                        if (order != null)
                        {
                            order.ExecutorID = user_id != null ? Convert.ToInt32(user_id) : 0;
                            _db.Entry(order).State = EntityState.Modified;
                            _db.SaveChanges();



                            //if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                            //    continue;

                            string old_val = Utils.Utils.GetEnumDescription(order.Status);
                            order.Status = OrderStatus.Closed;
                            order.ChangerUser = _db.Users.Where(u => u.Id == user_id).FirstOrDefault().Name;// ((User)Session["CurrentUser"]).Name;
                            order.IsApproved = false;
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

                            var phone = _db.Users.Where(u => u.Id == order.UserId).Select(s => s).FirstOrDefault();
                            if (phone.Type == 43)
                                Task.Run(async () => { await Utils.Utils.sendMessage(phone.Phone, "Tqveni shekvetit abonenti warmatebit daregistrirda. Dagericxat bonusi"); }).Wait();

                            var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>();
                            context.Clients.All.onHitRecorded("OrderClosed", user_id);
                            context.Clients.All.onHitRecorded("RegionClosed",
                                 new RegionGoName(
                                     new SqlConnection(
                                                 ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                       order.Id,
                                       "doc.Orders",
                                       user_id
                                 ).Result(),
                                 user_id
                                 );
                            tran.Commit();
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
                            Order order = _db.Orders.Where(c => c.Id == order_id).FirstOrDefault();
                            if (order != null)
                            {
                                if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                                    continue;

                                if (order.Status == OrderStatus.Registered)
                                    order.Status = OrderStatus.Worked;
                                order.GetDate = dateFrom;
                                order.ChangeDate = DateTime.Now;
                                order.IsApproved = false;
                                order.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                _db.Entry(order).State = EntityState.Modified;

                                int user_id = ((User)Session["CurrentUser"]).Id;
                                this.AddLoging(_db,
                                                 LogType.Order,
                                                 LogMode.Change,
                                                 user_id,
                                                 order.Id,
                                                 "შეკვეთის მისვლის თარიღის შეცვლა - " + dateFrom.ToString("dd/MM/yyyy HH:mm"),
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
                            Order order = _db.Orders.Where(c => c.Id == order_id).FirstOrDefault();
                            if (order != null)
                            {
                                //order.ExecutorID = executerID != null ? Convert.ToInt32(executerID) : 0;
                                _db.Entry(order).State = EntityState.Modified;
                                _db.SaveChanges();

                                if (order.Status == (OrderStatus)Enum.Parse(typeof(OrderStatus), status))
                                    continue;

                                //if (order.Status == OrderStatus.Montage || order.Status == OrderStatus.Canceled || order.Status == OrderStatus.Closed)
                                //    continue;

                                string old_val = Utils.Utils.GetEnumDescription(order.Status);
                                order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), status);
                                order.ChangerUser = ((User)Session["CurrentUser"]).Name;
                                order.IsApproved = false;
                                order.ApproveUser = "";
                                order.ChangeDate = DateTime.Now;

                                _db.Entry(order).State = EntityState.Modified;


                                this.AddLoging(_db,
                                                 LogType.Order,
                                                 LogMode.Change,
                                                 user_id,
                                                 order.Id,
                                                 order.Num.ToString(),
                                                 new List<LoggingData>() { new LoggingData() { field = "სტატუსის შეცვლა", old_val = old_val, new_val = Utils.Utils.GetEnumDescription((OrderStatus)Enum.Parse(typeof(OrderStatus), status)) } }
                                              );

                                _db.SaveChanges();
                                if ((OrderStatus)Enum.Parse(typeof(OrderStatus), status) == OrderStatus.Closed)
                                {
                                    var phone = _db.Users.Where(u => u.Id == order.UserId).Select(s => s).FirstOrDefault();
                                    if (phone.Type == 43)
                                        Task.Run(async () => { await Utils.Utils.sendMessage(phone.Phone, "Tqveni shekvetit abonenti warmatebit daregistrirda. Dagericxat bonusi"); }).Wait();
                                }
                                // live qolcentri static
                                if ((OrderStatus)Enum.Parse(typeof(OrderStatus), status) == OrderStatus.Closed)
                                {
                                    var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>();
                                    context.Clients.All.onHitRecorded("OrderClosed", order.ExecutorID);
                                    _db.Database.ExecuteSqlCommand($"UPDATE [doc].[Orders]  SET  [to_go] = 0 WHERE executor_id ={order.ExecutorID }");
                                }
                                if ((OrderStatus)Enum.Parse(typeof(OrderStatus), status) == OrderStatus.Canceled)
                                {
                                    var context = GlobalHost.ConnectionManager.GetHubContext<HubMessage>();
                                    context.Clients.All.onHitRecorded("OrderCanceled", order.ExecutorID);
                                    _db.Database.ExecuteSqlCommand($"UPDATE [doc].[Orders]  SET  [to_go] = 0 WHERE executor_id ={order.ExecutorID }");
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
                return Json(1);
            }
        }

        [HttpPost]
        public JsonResult CheckCustomer(string code)
        {
            using (DataContext _db = new DataContext())
            {
                int hasOrder = _db.Orders.Where(o => o.Code == code).Select(o => o.Num).FirstOrDefault();
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

        [HttpPost]
        public JsonResult SendSMS(int[] abonents, string message)
        {
            using (DataContext _db = new DataContext())
            {
                List<Param> Params = _db.Params.ToList();

                string username = Params.First(p => p.Name == "SMSPassword").Value;
                string password = Params.First(p => p.Name == "SMSUsername").Value;

                var orders = _db.Orders.Where(o => abonents.Contains(o.Id)).Select(o => o.Name).ToList();
                if (Utils.Utils.OnSendSMS(orders.Select(s => s.Split('/')[1].Trim()).ToList(), message, username, password, _db))
                {
                    int user_id = ((User)Session["CurrentUser"]).Id;
                    this.AddLoging(_db,
                                    LogType.Message,
                                    LogMode.Add,
                                    user_id,
                                    0,
                                    "SMS ის გაგზავნა შეკვეთებიდან",
                                    orders.Select(c => new LoggingData { field = "აბონენტი", new_val = c.Split('/')[0].Trim() }).ToList()
                              );

                    return Json("შეტყობინება წარმატებით გაიგზავნა");
                }
                else
                {
                    return Json("შეტყობინება ვერ გაიგზავნა");
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetSmsDialog()
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Reasons = _db.Reasons.ToList();
            }
            return PartialView("~/Views/Order/_SmsDialog.cshtml");
        }

        [HttpPost]
        public PartialViewResult Detail(int id)
        {
            using (DataContext _db = new DataContext())
            {
                Order order = _db.Orders.Where(o => o.Id == id).FirstOrDefault();
                if (order != null)
                {
                    ViewBag.Name = order.Name.Split('/')[0];
                }

                return PartialView("~/Views/Order/_Detail.cshtml");
            }
        }

        [HttpGet]
        public PartialViewResult CardsAdd(int count, int id)
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
                ViewBag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();

                ViewBag.Count = count;
                ViewBag.OrderId = id;
                ViewBag.CustomerType = Newtonsoft.Json.JsonConvert.DeserializeObject<Abonent>(_db.Orders.Where(o => o.Id == id).Select(c => c.Data).FirstOrDefault()).Customer.Type;
                return PartialView("~/Views/Order/_CardsAdd.cshtml", new List<Card>() { new Card() });
            }
        }

        [HttpPost]
        public JsonResult CardsAdd(List<Card> Cards)
        {
            if (Cards != null)
                if (Cards.Count > 0)
                {
                    using (DataContext _db = new DataContext())
                    {
                        try
                        {
                            int order_id = Cards.First().CustomerId;
                            Order order = _db.Orders.Where(o => o.Id == order_id).FirstOrDefault();
                            if (order != null)
                            {
                                Abonent abonent = Newtonsoft.Json.JsonConvert.DeserializeObject<Abonent>(order.Data);

                                Cards.ForEach(c => c.CustomerId = 0);
                                abonent.Cards = Cards;
                                order.Data = Newtonsoft.Json.JsonConvert.SerializeObject(abonent);
                                _db.Entry(order).State = EntityState.Modified;
                                _db.SaveChanges();
                                return Json(1);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            return Json(0);
        }
        [NonAction]
        public bool OrderComment(int id, int user_id, string comment)
        {
            using (DataContext _db = new DataContext())
            {
                if (comment != "")
                {
                    var _order = _db.Orders.Where(c => c.Id == id).FirstOrDefault();
                    _order.comment = comment;
                    _db.Entry(_order).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            return true;
        }
        [NonAction]
        public bool OrderCancelStatic(int id, int user_id, int comment)
        {
            using (IDbConnection _db = new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString))
            {
                var sql = "INSERT INTO[dbo].[OrderReserveAnswer] ([tdate] ,[order_id],[reserve_answer] ,[user_id]) VALUES (@tdate ,@order_id ,@reserve_answer,@user_id)";
                _db.Execute(sql, new
                {
                   tdate= DateTime.Now,
                    order_id=id,
                    reserve_answer= comment,
                    user_id=user_id
                });
            }
            return true;
        }
        [NonAction]
        public string OrderWriteComment(int id)
        {
            using (DataContext _db = new DataContext())
            {
                var _damage = _db.Database.SqlQuery<string>("SELECT d.comment FROM doc.Orders d where d.id=" + id).FirstOrDefault();
                if (_damage == null)
                {
                    _damage = "";
                }
                return _damage;
            }
        }
    }
}