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
    public class AccountantController : Controller
    {
        private int pageSize = 20;
        // GET: Accountant
        public async System.Threading.Tasks.Task<ActionResult> Index(string letter, string column, string dt_from, string dt_to, int page = 1)
        {
            if (!Utils.Utils.GetPermission("FINA_SHOW"))
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

                //        string sqlcmd = @"select TOP(" + pageSize + @") * FROM (select row_number() over(ORDER BY c.id DESC) AS row_num, c.id as ID, c.approve_status as Mode, c.tdate, a.name as CustomerName, a.lastname as CustomerLastName, a.code as CustomerCode, a.address as CustomerAddress, so.Name as ObjectName, so.City as ObjectAddress, so.IdentCode as UserCode, ut.name as [Group], so.Type as SType  from book.Cards as c 
                //left join config.Logging as l on c.id = l.type_id
                //left join book.Customers as a on a.id = c.customer_id
                //left join book.Users as u on l.user_id = u.id
                //left join dbo.SellerObject as so on u.object = so.ID
                //left join book.UserTypes as ut on u.type = ut.id where c.status != 4 and (ut.id = 18 or ut.id=23) AND c.tdate BETWEEN @date_from AND @date_to) as a where a.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);// + " ORDER BY a.id DESC";

                //        int count = await _db.Database.SqlQuery<int>(@"select COUNT(c.id) from book.Cards as c 
                //left join config.Logging as l on c.id = l.type_id
                //left join book.Customers as a on a.id = c.customer_id
                //left join book.Users as u on l.user_id = u.id
                //left join dbo.SellerObject as so on u.object = so.ID
                //left join book.UserTypes as ut on u.type = ut.id where c.status != 4 and (ut.id = 18 or ut.id=23) AND c.tdate BETWEEN @date_from AND @date_to", new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).FirstOrDefaultAsync();

                Dictionary<int, string> dic = new Dictionary<int, string>();
                Dictionary<int, double> pricedic = new Dictionary<int, double>();
                //List<AbonentRecord> lst = _db.Database.SqlQuery<AbonentRecord>(sqlcmd, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToList();
                double pack12price = 0, pack15price = 0;

                string packageName = "";
                double packageSumPrice = 0;

                //foreach (var AbonentRec in lst)
                //{
                //    packageName = "";
                //    packageSumPrice = 0;
                //    List<Subscribtion> sb = _db.Subscribtions.Where(s => s.CardId == AbonentRec.ID && s.Status == true).ToList();
                //    foreach (var item in sb)
                //    {
                //        List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == item.Id).ToList();
                //        foreach (var sbs in sbp)
                //        {
                //            Package package = _db.Packages.Where(p => p.Id == sbs.PackageId).First();
                //            packageName += package.Name + ", ";
                //            packageSumPrice += package.Price;

                //            if (package.Price == 15)
                //            {
                //                pack15price += package.Price;
                //            }

                //            if (package.Price == 12)
                //            {
                //                pack12price += package.Price;
                //            }
                //        }
                //    }
                //    AbonentRec.Package = packageName + packageSumPrice +" GEL";
                //    dic.Add(AbonentRec.ID, AbonentRec.Package);
                //    pricedic.Add(AbonentRec.ID, packageSumPrice);
                //}
                //ViewBag.PackageDictionary = dic;
                //ViewBag.PriceDictionary = pricedic;
                //ViewBag.pack12price = pack12price;
                //ViewBag.pack15price = pack15price;

                //List<SellerObject> obj = _db.Seller.ToList();
                //ViewBag.SellerObjects = obj;


                //List<Card> cards;

                ViewBag.selectedObject = 0;
                IPagedList<Card> cards = null;
                if (letter == null || letter == "")
                    cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.CardStatus != CardStatus.Canceled && (c.User.UserType.Name.Contains("დილერი")) && c.Tdate >= dateFrom && c.Tdate <= dateTo).ToPagedList(page, pageSize);
                else
                {
                    int sellerObjId = Convert.ToInt32(letter);
                    ViewBag.selectedObject = sellerObjId;
                    cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.CardStatus != CardStatus.Canceled && (c.User.UserType.Name.Contains("დილერი")) && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.User.SellerObj.ID == sellerObjId).ToPagedList(page, pageSize);
                }
                ViewBag.totalItemsCount = cards.TotalItemCount;
                ViewBag.page = page;
                ViewBag.pageSize = pageSize;
                foreach (var card in cards)
                {
                    var subs = card.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.ToList();
                    packageName = "";
                    packageSumPrice = 0;
                    foreach (var sub in subs)
                    {
                        packageName += sub.Package.Name + ", ";
                        packageSumPrice += sub.Package.Price;

                        if (sub.Package.Price == 15)
                        {
                            pack15price += sub.Package.Price;
                        }

                        if (sub.Package.Price == 12)
                        {
                            pack12price += sub.Package.Price;
                        }
                    }

                    dic.Add(card.Id, packageName + packageSumPrice + " GEL");
                    pricedic.Add(card.Id, packageSumPrice);
                }

                ViewBag.PackageDictionary = dic;
                ViewBag.PriceDictionary = pricedic;
                ViewBag.pack12price = pack12price;
                ViewBag.pack15price = pack15price;

                List<SellerObject> obj = _db.Seller.ToList();
                ViewBag.SellerObjects = obj;

                return View(cards);

                //return View(await _db.Database.SqlQuery<AbonentRecord>(sqlcmd, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToRawPagedListAsync(count, page, pageSize));

            }

            //return View();
        }

        [HttpGet]
        public async Task<ActionResult> FilterAbonents(string letter, string column, int page, string dt_from, string dt_to)
        {
            if (!Utils.Utils.GetPermission("FINA_SHOW"))
            {
                return Json(0);
            }
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
            //column = "so.Name";
            //string where = column + " LIKE N'" + letter + "%'";
            //string where = column + " LIKE N'" + letter + "%'";
            //if (column == "p.pay_type_id")
            //    where = column + "=" + letter;
            //where = where.Replace("+", "+' '+");

            string where = letter != ""? " AND " +column + "=" + letter:"";

            string sql = @"select TOP(" + pageSize + @") * FROM (select row_number() over(ORDER BY c.id DESC) AS row_num, c.id as ID, c.approve_status as Mode, c.tdate, a.name as CustomerName, a.lastname as CustomerLastName, a.code as CustomerCode, a.address as CustomerAddress, so.Name as ObjectName, so.City as ObjectAddress, so.IdentCode as UserCode, ut.name as [Group], so.Type as SType  from book.Cards as c 
								left join config.Logging as l on c.id = l.type_id
								left join book.Customers as a on a.id = c.customer_id
								left join book.Users as u on l.user_id = u.id
								left join dbo.SellerObject as so on u.object = so.ID
								left join book.UserTypes as ut on u.type = ut.id where c.status != 4 and (ut.id = 18 or ut.id=23) " + where + " AND c.tdate BETWEEN @date_from AND @date_to) as a where a.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);// + " ORDER BY a.id DESC";

            System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

            using (DataContext _db = new DataContext())
            {
                int count = await _db.Database.SqlQuery<int>(@"select COUNT(c.id) from book.Cards as c 
								left join config.Logging as l on c.id = l.type_id
								left join book.Customers as a on a.id = c.customer_id
								left join book.Users as u on l.user_id = u.id
								left join dbo.SellerObject as so on u.object = so.ID
								left join book.UserTypes as ut on u.type = ut.id where c.status != 4 and (ut.id = 18 or ut.id=23) " + where + " AND c.tdate BETWEEN @date_from AND @date_to", new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).FirstOrDefaultAsync();


                Dictionary<int, string> dic = new Dictionary<int, string>();
                Dictionary<int, double> pricedic = new Dictionary<int, double>();
                //List<AbonentRecord> lst = _db.Database.SqlQuery<AbonentRecord>(sql, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToList();
                //List<Package> packages = new List<Package>();
                double pack12price = 0, pack15price = 0;

                string packageName = "";
                double packageSumPrice = 0;
                //foreach (var AbonentRec in lst)
                //{
                //    packageName = "";
                //    packageSumPrice = 0;
                //    List<Subscribtion> sb = _db.Subscribtions.Where(s => s.CardId == AbonentRec.ID && s.Status == true).ToList();
                //    foreach (var item in sb)
                //    {
                //        List<SubscriptionPackage> sbp = _db.SubscriptionPackages.Where(s => s.SubscriptionId == item.Id).ToList();
                //        foreach (var sbs in sbp)
                //        {
                //            Package package = _db.Packages.Where(p => p.Id == sbs.PackageId).First();
                //            packageName += package.Name + ", ";
                //            packageSumPrice += package.Price;

                //            if (package.Price == 15)
                //            {
                //                pack15price += package.Price;
                //            }

                //            if (package.Price == 12)
                //            {
                //                pack12price += package.Price;
                //            }
                //        }
                //    }
                //    AbonentRec.Package = packageName + packageSumPrice + " GEL";
                //    if (dic.ContainsKey(AbonentRec.ID))
                //    {
                //        lst.Remove(new AbonentRecord() { ID = AbonentRec.ID });
                //        continue;
                //    }
                //    dic.Add(AbonentRec.ID, AbonentRec.Package);
                //    pricedic.Add(AbonentRec.ID, packageSumPrice);
                //}
                //ViewBag.PackageDictionary = dic;


                try
                {
                    ViewBag.selectedObject = 0;
                    IPagedList<Card> cards = null;
                    if (letter == null || letter == "")
                        cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.CardStatus != CardStatus.Canceled && (c.User.UserType.Id == 18 || c.User.UserType.Id == 23) && c.Tdate >= dateFrom && c.Tdate <= dateTo).ToPagedList(page, pageSize);
                    else
                    {
                        int sellerObjId = Convert.ToInt32(letter);
                        ViewBag.selectedObject = sellerObjId;
                        cards = _db.Cards.Include("Customer").Include("User").Include("User.SellerObj").Include("User.UserType").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c => c.CardStatus != CardStatus.Canceled && (c.User.UserType.Id == 18 || c.User.UserType.Id == 23) && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.User.SellerObj.ID == sellerObjId).ToPagedList(page, pageSize);
                    }

                    foreach (var card in cards)
                    {
                        var subs = card.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.ToList();
                        packageName = "";
                        packageSumPrice = 0;
                        foreach (var sub in subs)
                        {
                            packageName += sub.Package.Name + ", ";
                            packageSumPrice += sub.Package.Price;

                            if (sub.Package.Price == 15)
                            {
                                pack15price += sub.Package.Price;
                            }

                            if (sub.Package.Price == 12)
                            {
                                pack12price += sub.Package.Price;
                            }
                        }

                        dic.Add(card.Id, packageName + packageSumPrice + " GEL");
                        pricedic.Add(card.Id, packageSumPrice);
                    }

                    ViewBag.PackageDictionary = dic;
                    ViewBag.PriceDictionary = pricedic;
                    ViewBag.pack12price = pack12price;
                    ViewBag.pack15price = pack15price;

                    List<SellerObject> obj = _db.Seller.ToList();
                    ViewBag.SellerObjects = obj;

                    //var findList = await _db.Database.SqlQuery<AbonentRecord>(sql, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToRawPagedListAsync(count, page, pageSize);
                    //return Json(new
                    //{
                    //    Abonents = cards.ToList(),
                    //    Paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, cards.ToPagedList(page, pageSize), p => p.ToString(), PagedListRenderOptions.PageNumbersOnly).ToHtmlString(),
                    //    PackageDictionary = dic.ToList(),
                    //    PriceDictionary = pricedic.ToList(),
                    //    pack12price = pack12price,
                    //    pack15price = pack15price
                    //    //FilePath = _db.Params.Where(p => p.Name == "FTPHost").Select(p => p.Value).First()
                    //});
                    return View("~/Views/Accountant/Index.cshtml", cards);
                }
                catch(Exception ex)
                {
                    throw;
                    //return Json(new
                    //{
                    //    Abonents = 0,
                    //    Paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, null, p => p.ToString(), PagedListRenderOptions.PageNumbersOnly).ToHtmlString(),
                    //    PackageDictionary = dic.ToList(),
                    //    PriceDictionary = pricedic.ToList(),
                    //    pack12price = pack12price,
                    //    pack15price = pack15price
                    //    //FilePath = _db.Params.Where(p => p.Name == "FTPHost").Select(p => p.Value).First()
                    //});
                }
            }
        }

        [HttpPost]
        public JsonResult RecordApprove(int id)
        {
            if (!Utils.Utils.GetPermission("FINA_APPROVE"))
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
                        Card card = _db.Cards.Where(c => c.Id == id).FirstOrDefault();
                        if (card != null)
                        {
                            card.ApproveStatus = 2;
                            _db.Entry(card).State = EntityState.Modified;

                            _db.Loggings.Add(new Logging()
                            {
                                Tdate = DateTime.Now,
                                UserId = user_id,
                                Type = LogType.Card,
                                Mode = LogMode.Approve,
                                TypeValue = card.CardNum,
                                TypeId = card.Id
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
                    catch
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
            return Json(0);
        }
    }
}