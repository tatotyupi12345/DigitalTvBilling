using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Microsoft.Ajax.Utilities;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Data.SqlClient;
using DigitalTVBilling.Filters;
using DigitalTVBilling.ListModels;
using System.Data.Entity.SqlServer;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class ReportController : BaseController
    {
        public ActionResult Index()
        {
           // if (!Utils.Utils.GetPermission("REPORT_SHOW"))
           //     return new RedirectResult("/Main");
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetChannels(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                var rows = await _db.PackageChannels
                    .Select(p => new { package = p.Package.Name, price = p.Package.Price, jurid_price = p.Package.JuridPrice, min_price = p.Package.MinPrice, channel = p.Channel.Name })
                    .OrderBy(p=>p.package).ToPagedListAsync(pos, 50);
                var columns = new[] { 
                    new {name ="პაკეტი", width = 30, column = "package" }, 
                    new {name ="ფასი", width = 15, column = "price" } , 
                     new {name ="იურიდ. ფასი", width = 15, column = "jurid_price" } , 
                    new {name ="მინ. ფასი", width = 15, column = "min_price" } , 
                    new {name ="არხი", width = 40, column = "channel" } 
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetMaregChannels(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                var rows = await _db.PackageChannels
                    .Select(p => new { package = p.Package.Name, price = p.Package.Price, channel = p.Channel.Name })
                    .OrderBy(p=>p.package).ToPagedListAsync(pos, 500);
                var columns = new[] { 
                    new {name ="პაკეტის დასახელება", width = 30, column = "package" }, 
                    new {name ="პაკეტის ღირებულება", width = 15, column = "price" } , 
                    new {name ="არხების ჩამონათვალი", width = 40, column = "channel" } 
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        [HttpPost]
        public JsonResult GetForm1_1(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var initial_data = new List<string> { "სააბონენტო გადასახადი", "ტექნიკური მომსახურება", "სხვა" };
                var rows = initial_data.Select(c => new
                {
                    type = "საცალო",
                    service = "მაუწყებლობის ტრანზიტი - " + c,
                    money_in = c != "სააბონენტო გადასახადი" ? 0 : _db.CardCharges.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Where(p => p.Status != CardChargeStatus.ReturnMoney && p.Status != CardChargeStatus.Service).Select(p => p.Amount).DefaultIfEmpty().Sum() / (decimal)1.18,
                    dxg = c != "სააბონენტო გადასახადი" ? 0 : _db.CardCharges.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Where(p => p.Status != CardChargeStatus.ReturnMoney && p.Status != CardChargeStatus.Service).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Where(p => p.Status != CardChargeStatus.ReturnMoney && p.Status != CardChargeStatus.Service).Select(p => p.Amount).DefaultIfEmpty().Sum() / (decimal)1.18,
                    aqciz = 0
                }).ToList();

                var columns = new[] { 
                    new {name ="საცალო/საბითუმო", width = 10, column = "type" }, 
                    new {name ="მომსახურება და ქვეკატეგორია", width = 60, column = "service" } , 
                    new {name ="შემოსავალი", width = 10, column = "money_in" },
                    new {name ="დღგ", width = 10, column = "dxg" },
                    new {name ="აქციზი", width = 10, column = "aqciz" } 
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = "<div><ul class=\"pager\"></ul></div>" });
            }
        }


        public async Task<JsonResult> GetForm4_3(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                string sql = @"DECLARE @start_date DATETIME='" + fromDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
DECLARE @end_date DATETIME='" + toDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
;WITH Charges(amount, tdate, card_id)
AS
(
 SELECT amount, tdate, card_id FROM doc.CardCharges WHERE tdate BETWEEN @start_date AND @end_date AND [status] NOT IN (4,5)
)

SELECT
 q.city,
 N'მიტრისი (ციფრული)' AS [type],
 N'დიახ' AS coding,
 q.packet AS package,
 q.abonents_count,
 q.abonents_count AS decoders_count,
 q.n_abonents_count,
 q.amount / 1.18 AS money_in,
 q.amount - q.amount / 1.18 AS dxg
FROM (SELECT 
city, 
q.packet,
(SELECT COUNT(*) FROM book.Cards AS c 
 INNER JOIN book.Customers AS a ON a.id=c.customer_id 
 WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND EXISTS(SELECT * FROM Charges WHERE  card_id=c.id)) AS abonents_count,
 (SELECT COUNT(*) FROM book.Cards AS c 
 INNER JOIN book.Customers AS a ON a.id=c.customer_id 
 OUTER APPLY (SELECT TOP(1) tdate, card_id FROM doc.CardCharges WHERE card_id=c.id ORDER BY tdate) AS ch 
  WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND ch.tdate IS NOT NULL AND ch.tdate BETWEEN @start_date AND @end_date) AS n_abonents_count,
  (SELECT ISNULL(SUM(amount),0) FROM Charges AS p WHERE card_id IN (SELECT c.id FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city  AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1))) AS amount
FROM (SELECT
(a.village + ' - ' + a.city) AS city,
STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') AS packet 

FROM doc.Subscribes AS s
INNER JOIN book.Cards AS c ON c.id = s.card_id
INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE c.status!=5 AND s.status=1) AS q GROUP BY q.city, q.packet) AS q
WHERE q.abonents_count > 0";

                var rows = await _db.Database.SqlQuery<Form4_3_Report>(sql).ToListAsync();

                var columns = new[] { 
                    new {name ="დასახლებული პუნქტი", width = 15, column = "city" } , 
                    new {name ="მიწოდების ტიპი", width = 15, column = "type" }, 
                    new {name ="კოდირებული", width = 10, column = "coding" } , 
                    new {name ="პაკეტი", width = 10, column = "package" }, 
                    new {name ="აბონენტები", width = 10, column = "abonents_count" }, 
                    new {name ="დეკოდერების რაოდენობა", width = 10, column = "decoders_count" },
                    new {name ="ახალი აბონენტების რაოდენობა", width = 10, column = "n_abonents_count" }, 
                    new {name ="სააბონენტო შემოსავალი", width = 10, column = "money_in" },
                    new {name ="დღგ", width = 10, column = "dxg" }
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = "<div><ul class=\"pager\"></ul></div>" });
            }
        }

        public async Task<JsonResult> GetForm4_4(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                string sql = @"DECLARE @start_date DATETIME='" + fromDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
DECLARE @end_date DATETIME='" + toDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
;WITH Charges(tdate, card_id)
AS
(
 SELECT tdate, card_id FROM doc.CardCharges WHERE tdate BETWEEN @start_date AND @end_date AND [status] NOT IN (4,5)
)
SELECT 
city,
N'მიტრისი (ციფრული)' AS [type],
N'დიახ' AS coding, 
q.packet AS package,
(SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND EXISTS(SELECT * FROM Charges WHERE  card_id=c.id)) AS abonents_count,
(SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id OUTER APPLY (SELECT TOP(1) tdate, card_id FROM doc.CardCharges WHERE card_id=c.id ORDER BY tdate) AS ch  WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND ch.tdate IS NOT NULL AND ch.tdate BETWEEN @start_date AND @end_date) AS n_abonents_count
FROM (SELECT
(a.village + ' - ' + a.city) AS city,
STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') AS packet 

FROM doc.Subscribes AS s
INNER JOIN book.Cards AS c ON c.id = s.card_id
INNER JOIN book.Customers AS a ON a.id=c.customer_id


WHERE c.status!=5 AND s.status=1) AS q GROUP BY q.city, q.packet
 HAVING (SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND EXISTS(SELECT * FROM Charges WHERE  card_id=c.id)) > 0";

                var rows = await _db.Database.SqlQuery<Form4_4Report>(sql).ToListAsync();
                var columns = new[] { 
                    new {name ="დასახლებული პუნქტი", width = 15, column = "city" } , 
                    new {name ="მიწოდების ტიპი", width = 15, column = "type" }, 
                    new {name ="კოდირებული", width = 10, column = "coding" } , 
                    new {name ="პაკეტი", width = 10, column = "package" }, 
                    new {name ="აბონენტები", width = 10, column = "abonents_count" }, 
                    new {name ="ახალი აბონენტების რაოდენობა", width = 10, column = "n_abonents_count" }, 
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = "<div><ul class=\"pager\"></ul></div>" });

               
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetPackets(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                DateTime nDate = toDate.AddMonths(-1);
                bool isActive = Convert.ToBoolean(Request["sign"]);

                var rows = await _db.SubscriptionPackages.AsNoTracking()
                     .Where(c => c.Subscribtion.Status == isActive)
                     .Where(c => c.Subscribtion.Card.CardStatus != CardStatus.Canceled)
                     .Where(c => c.Subscribtion.Card.Tdate >= fromDate && c.Subscribtion.Card.Tdate <= toDate)
                     .GroupBy(c => c.PackageId)
                     .Select(s => new
                     {
                         package = s.FirstOrDefault().Package.Name,
                         price = s.FirstOrDefault().Package.Price,
                         jurid_price = s.FirstOrDefault().Package.JuridPrice,
                         channels_count = s.FirstOrDefault().Package.PackageChannels.Count(),
                         abonents_count = s.Select(ss => ss.Subscribtion.Card.CustomerId).Distinct().Count(),
                         decoders_count = s.Select(ss => ss.Subscribtion.Card.ReceiverId).Count(),
                         n_abonents_count = _db.Customers.Where(c => c.Tdate >= nDate && c.Tdate <= toDate).Where(c => c.Cards.Any(cc => s.Select(p => p.Subscribtion.CardId).Contains(cc.Id))).Distinct().Count(),
                         n_decoders_count = _db.Receivers.Count(c => c.Cards.Where(cc => cc.Tdate >= nDate && cc.Tdate <= toDate).Any(cc => s.Select(p => p.Subscribtion.CardId).Contains(cc.Id))),
                         amount_in = _db.Payments.Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Amount).Sum() ?? 0,
                         service_out_cash = _db.CardServices.Where(ss=>ss.PayType == CardServicePayType.Cash).Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Service.Amount).Sum() ?? 0,
                         service_out_not_cash = _db.CardServices.Where(ss => ss.PayType == CardServicePayType.NotCash).Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Service.Amount).Sum() ?? 0
                     }).OrderBy(p => p.package).ToPagedListAsync(pos, 50);

                var columns = new[] { 
                    new {name ="პაკეტი", width = 20, column = "package" }, 
                    new {name ="ფასი", width = 10, column = "price" } ,
                    new {name ="იურიდ. ფასი", width = 10, column = "jurid_price" } , 
                    new {name ="არხ. რაოდ.", width = 10, column = "channels_count" } , 
                    new {name ="აბ. რაოდ.", width = 10, column = "abonents_count" }, 
                    new {name ="დეკ. რაოდ.", width = 10, column = "decoders_count" }, 
                    new {name ="ახალი აბ. რაოდ.", width = 10, column = "n_abonents_count" },
                    new {name ="ახალი დეკ. რაოდ.", width = 10, column = "n_decoders_count" }, 
                    new {name ="შემოსული თანხა", width = 10, column = "amount_in" },
                    new {name ="გაწ.მომს. ნაღდი", width = 10, column = "service_out_cash" },
                    new {name ="გაწ.მომს. უნაღდო", width = 10, column = "service_out_not_cash" }
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });

            }
        }

        [HttpPost]
        public async Task<JsonResult> GetPacketsByAbonentType(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                DateTime nDate = toDate.AddMonths(-1);
                bool isActive = Convert.ToBoolean(Request["sign"]);

                var rows = await _db.SubscriptionPackages.AsNoTracking()
                     .Where(c => c.Subscribtion.Status == isActive)
                     .Where(c => c.Subscribtion.Card.CardStatus != CardStatus.Canceled)
                     .Where(c => c.Subscribtion.Card.Tdate >= fromDate && c.Subscribtion.Card.Tdate <= toDate)
                     .GroupBy(c => new { c.PackageId, c.Subscribtion.Card.Customer.Type })
                     .Select(s => new
                     {
                         type = s.FirstOrDefault().Subscribtion.Card.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (s.FirstOrDefault().Subscribtion.Card.Customer.JuridicalType == 1 ? ":კომუნალური" : (s.FirstOrDefault().Subscribtion.Card.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                         package = s.FirstOrDefault().Package.Name,
                         channels_count = s.FirstOrDefault().Package.PackageChannels.Count(),
                         abonents_count = s.Select(ss => ss.Subscribtion.Card.CustomerId).Distinct().Count(),
                         decoders_count = s.Select(ss => ss.Subscribtion.Card.ReceiverId).Count(),
                         n_abonents_count = _db.Customers.Where(c => c.Tdate >= nDate && c.Tdate <= toDate).Where(c => c.Cards.Any(cc => s.Select(p => p.Subscribtion.CardId).Contains(cc.Id))).Distinct().Count(),
                         n_decoders_count = _db.Receivers.Count(c => c.Cards.Where(cc => cc.Tdate >= nDate && cc.Tdate <= toDate).Any(cc => s.Select(p => p.Subscribtion.CardId).Contains(cc.Id))),
                         amount_in = _db.Payments.Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Amount).Sum() ?? 0,
                         service_out_cash = _db.CardServices.Where(ss => ss.PayType == CardServicePayType.Cash).Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Service.Amount).Sum() ?? 0,
                         service_out_not_cash = _db.CardServices.Where(ss => ss.PayType == CardServicePayType.NotCash).Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Service.Amount).Sum() ?? 0
                     }).OrderBy(p => p.package).ToPagedListAsync(pos, 50);

                var columns = new[] { 
                    new {name ="აბონენტის ტიპი", width = 15, column = "type" } , 
                    new {name ="პაკეტი", width = 15, column = "package" }, 
                    new {name ="არხ. რაოდ.", width = 10, column = "channels_count" } , 
                    new {name ="აბ. რაოდ.", width = 10, column = "abonents_count" }, 
                    new {name ="დეკ. რაოდ.", width = 10, column = "decoders_count" }, 
                    new {name ="ახალი აბ. რაოდ.", width = 10, column = "n_abonents_count" },
                    new {name ="ახალი დეკ. რაოდ.", width = 10, column = "n_decoders_count" }, 
                    new {name ="შემოსული თანხა", width = 10, column = "amount_in" },
                    new {name ="გაწ.მომს. ნაღდი", width = 10, column = "service_out_cash" },
                    new {name ="გაწ.მომს. უნაღდო", width = 10, column = "service_out_not_cash" }
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });

            }
        }

        [HttpPost]
        public async Task<JsonResult> GetCards(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                DateTime nDate = toDate.AddMonths(-1);
                int sign = Convert.ToInt32(Request["sign"]);

                var rows = await _db.Cards.AsNoTracking()
                          .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                          .Where(c => sign == 0 ? true : (sign == 1 ? c.CardServices.Count > 0 : c.CardServices.Count == 0))
                          .OrderByDescending(c => c.Tdate)
                          .Select(c => new
                          {
                              date = c.Tdate,
                              num = c.CustomerId,
                              abonent_num = c.AbonentNum,
                              doc_num = c.DocNum ?? "",
                              card_num = c.CardNum,
                              type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                              code = c.Customer.Code,
                              name = c.Customer.Name + " " + c.Customer.LastName,
                              region = c.Customer.Region,
                              city = c.Customer.City + (string.IsNullOrEmpty(c.Customer.Village) ? "" : "/" + c.Customer.Village),
                              village = c.Customer.Village,
                              district = c.Customer.District,
                              address = c.Customer.Address,
                              phone1 = c.Customer.Phone1,
                              phone2 = c.Customer.Phone2 ?? "",
                              status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : (c.CardStatus == CardStatus.Blocked ? "დაბლოკილი" : "დაპაუზებული")))),
                              days = SqlFunctions.DateDiff("dd", _db.CardLogs.Where(l => l.CardId == c.Id).OrderByDescending(l => l.Date).FirstOrDefault().Date, DateTime.Now),
                              packet = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                              discount = c.Discount,
                              summ_in = c.Payments.Select(p => (decimal?)p.Amount).Sum() ?? 0,
                              service_out = c.CardCharges.Select(cc => (decimal?)cc.Amount).Sum() ?? 0,
                              balance = (c.Payments.Select(p => (decimal?)p.Amount).Sum() ?? 0) - (c.CardCharges.Select(p => (decimal?)p.Amount).Sum()) ?? 0,
                              user = c.User.Name,
                              comment = c.Customer.Desc,
                              first_service = c.CardServices.Select(ss => ss.Service.Name).FirstOrDefault()
                          }).ToPagedListAsync(pos, 50);

                var columns = new[] { 
                    new {name ="თარიღი", width = 15, column = "date", format=true } ,
                    new {name ="№", width = 15, column = "num", format=false } ,
                    new {name ="აბონენტის №", width = 15, column = "abonent_num", format=false } ,
                    new {name ="ხელშ. №", width = 15, column = "doc_num", format=false } ,
                    new {name ="ბარათის №", width = 15, column = "card_num", format=false } ,
                    new {name ="აბონენტის ტიპი", width = 15, column = "type", format=false } , 
                    new {name ="პ/ნ", width = 15, column = "code", format=false }, 
                    new {name ="აბონენტი", width = 10, column = "name", format=false } , 
                    new {name ="რეგიონი", width = 10, column = "region", format=false }, 
                    new {name ="ქალაქი/სოფ.", width = 10, column = "city", format=false },
                    new {name ="რაიონი", width = 10, column = "village", format=false },
                    new {name ="უბანი", width = 10, column = "district", format=false },
                    new {name ="მისამართი", width = 10, column = "address", format=false },
                    new {name ="ტელ1", width = 10, column = "phone1", format=false }, 
                    new {name ="ტელ2", width = 10, column = "phone2", format=false },
                    new {name ="სტატუსი", width = 10, column = "status", format=false },
                    new {name ="დღეები", width = 10, column = "days", format=false },
                    new {name ="პაკეტი", width = 10, column = "packet", format=false }, 
                    new {name ="%", width = 10, column = "discount", format=false }, 
                    new {name ="შემ. თანხა", width = 10, column = "summ_in", format=false }, 
                    new {name ="გაწ. მომს", width = 10, column = "service_out", format=false }, 
                    new {name ="ბალანსი", width = 10, column = "balance", format=false },
                    new {name ="მომხ.", width = 10, column = "user", format=false },
                    new {name ="კომენტარი", width = 10, column = "comment", format=false },
                    new {name ="პირველი გაწ. მომს", width = 10, column = "first_service", format=false }
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetCardsByAbonents(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                var rows = await _db.Customers.Select(c => new
                {
                    name = c.Name + " " + c.LastName,
                    code = c.Code,
                    type = c.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.JuridicalType == 1 ? ":კომუნალური" : (c.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                    region = c.Region,
                    city = c.City + (string.IsNullOrEmpty(c.Village) ? "" : "/" + c.Village),
                    village = c.Village,
                    phone = c.Phone1,
                    cards_count = c.Cards.Where(cc=>cc.CardStatus != CardStatus.Canceled).Count(),
                    active_cards_count = c.Cards.Where(cc => cc.CardStatus == CardStatus.Active).Count(),
                    closed_cards_count = c.Cards.Where(cc=>cc.CardStatus == CardStatus.Closed).Count(),
                    paused_cards_count = c.Cards.Where(cc => cc.CardStatus == CardStatus.Paused).Count(),
                    montaged_cards_count = c.Cards.Where(cc => cc.CardStatus == CardStatus.FreeDays).Count(),
                }).OrderBy(c=>c.name).ToPagedListAsync(pos, 50);

                var columns = new[] { 
                    new {name ="სახელი", width = 15, column = "name", format = false } ,
                    new {name ="პ/ნ", width = 15, column = "code", format = false }, 
                    new {name ="ტიპი", width = 15, column = "type", format = false } ,  
                     new {name ="რეგიონი", width = 10, column = "region", format=false }, 
                    new {name ="ქალაქი/სოფ.", width = 10, column = "city", format=false },
                    new {name ="რაიონი", width = 10, column = "village", format=false },
                    new {name ="ტელ1", width = 10, column = "phone", format = false }, 
                    new {name ="ბარათები სულ", width = 10, column = "cards_count", format = false },
                    new {name ="აქტიური ბარათები", width = 10, column = "active_cards_count", format = false }, 
                    new {name ="გათიშული ბარათები", width = 10, column = "closed_cards_count", format = false }, 
                    new {name ="დაპაუზებული ბარათები", width = 10, column = "paused_cards_count", format = false }, 
                    new {name ="მონტაჟში ბარათები", width = 10, column = "montaged_cards_count", format = false }
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });

            }
        }

        [HttpPost]
        public async Task<JsonResult> GetLostCards(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                DateTime nDate = toDate.AddMonths(-1);
                int sign = Convert.ToInt32(Request["sign"]);

                var rows = await _db.Cards.AsNoTracking()
                          .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                          .Where(c => c.CardStatus == CardStatus.Canceled)
                          .OrderByDescending(c => c.Tdate)
                          .Select(c => new
                          {
                              abonent_num = c.AbonentNum,
                              card_num = c.CardNum,
                              type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                              code = c.Customer.Code,
                              name = c.Customer.Name + " " + c.Customer.LastName,
                              region = c.Customer.Region,
                              city = c.Customer.City + (string.IsNullOrEmpty(c.Customer.Village) ? "" : "/" + c.Customer.Village),
                              address = c.Customer.Address,
                              phone1 = c.Customer.Phone1,
                              phone2 = c.Customer.Phone2 ?? "",
                              status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                              packet = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                              summ_in = c.Payments.Select(p => (decimal?)p.Amount).Sum() ?? 0,
                              service_out = c.CardServices.Select(cc => (decimal?)cc.Service.Amount).Sum() ?? 0,
                              balance = c.Payments.Select(p => (decimal?)p.Amount).Sum() ?? 0 - c.CardCharges.Select(p => (decimal?)p.Amount).Sum() ?? 0
                          }).ToPagedListAsync(pos, 50);

                var columns = new[] { 
                    new {name ="აბონენტის №", width = 15, column = "abonent_num" } ,
                    new {name ="ბარათის №", width = 15, column = "card_num" } ,
                    new {name ="აბონენტის ტიპი", width = 15, column = "type" } , 
                    new {name ="პ/ნ", width = 15, column = "code" }, 
                    new {name ="აბონენტი", width = 10, column = "name" } , 
                    new {name ="რეგიონი", width = 10, column = "region" }, 
                    new {name ="ქალაქი/სოფ.", width = 10, column = "city" }, 
                    new {name ="მისამართი", width = 10, column = "address" },
                    new {name ="ტელ1", width = 10, column = "phone1" }, 
                    new {name ="ტელ2", width = 10, column = "phone2" },
                    new {name ="სტატუსი", width = 10, column = "status" },
                    new {name ="პაკეტი", width = 10, column = "packet" }, 
                    new {name ="შემ. თანხა", width = 10, column = "summ_in" }, 
                    new {name ="გაწ. მომს", width = 10, column = "service_out" }
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        //REAL REPORTS
        [HttpPost]
        public async Task<JsonResult> GetCardsByStatus(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                int sign = Convert.ToInt32(Request["sign"]);
                CardStatus status = (CardStatus)sign;

                var rows = await _db.Cards.AsNoTracking()
                          .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                          .Where(c => sign == -1 ? true : (sign == 5 ? c.Mode == 1 : c.CardStatus == status))
                          .OrderByDescending(c => c.Tdate)
                          .Select(c => new
                          {
                              date = c.Tdate,
                              name = c.Customer.Name + " " + c.Customer.LastName,
                              code = c.Customer.Code,
                              type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" : "ფიზიკური",
                              address = c.Address,
                              doc_num = c.DocNum,
                              abonent_num = c.AbonentNum,
                              card_num = c.CardNum,
                              phone1 = c.Customer.Phone1,
                              status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                              balance = (_db.Payments.Where(p => p.CardId == c.Id).Sum(p => (decimal?)p.Amount) ?? 0) - (_db.CardCharges.Where(p => p.CardId == c.Id).Sum(p => (decimal?)p.Amount) ?? 0)
                          }).ToPagedListAsync(pos, 50);

                var columns = new[] {
                    new {name ="თარიღი", width = 10, column = "date", format = true },
                    new {name ="აბონენტი", width = 10, column = "name", format=false }, 
                    new {name ="პ/ნ", width = 15, column = "code", format=false },
                    new {name ="აბონენტის ტიპი", width = 15, column = "type", format=false },
                    new {name ="მისამართი", width = 10, column = "address", format=false },
                    new {name ="ხელშ. №", width = 15, column = "doc_num", format=false } ,
                    new {name ="აბონენტის №", width = 15, column = "abonent_num", format=false } ,
                    new {name ="ბარათის №", width = 15, column = "card_num", format=false } ,
                    new {name ="ტელ1", width = 10, column = "phone1", format=false }, 
                    new {name ="სტატუსი", width = 10, column = "status", format=false },
                    new {name ="ბალანსი", width = 10, column = "balance", format=false }
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });

            }
        }

        [HttpPost]
        public async Task<JsonResult> GetCharges(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var rows = await _db.CardCharges.AsNoTracking()
                   .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                   .OrderByDescending(c => c.Tdate).Select(c => new
                   {
                       date = c.Tdate,
                       abonent = c.Card.Customer.Name + " " + c.Card.Customer.LastName,
                       type = c.Card.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Card.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Card.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                       phone = c.Card.Customer.Phone1,
                       region = c.Card.Customer.Village + "-" + c.Card.Customer.City,
                       status = c.Card.CardStatus == CardStatus.Active ? "აქტიური" : (c.Card.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.Card.CardStatus == CardStatus.Closed ? "დახურული" : (c.Card.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                       abonent_num = c.Card.AbonentNum,
                       card_num = c.Card.CardNum,
                       packets = c.Card.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                       packet_date = c.Card.Subscribtions.FirstOrDefault(s => s.Status).Tdate,
                       amount = c.Amount,
                       charge_type= c.Status == CardChargeStatus.Daily ? "დღიური დარიცხვა" : (c.Status == CardChargeStatus.Pause ? "პაუზის დარიხხვა" : (c.Status == CardChargeStatus.Pen ? "ჯარიმის დარიცხვა" : (c.Status == CardChargeStatus.PenDaily? "ჯარიმის დღიური დარიცხვა" : (c.Status == CardChargeStatus.Service ? "გაწეული მომსახურების დარიცხვა" : (c.Status == CardChargeStatus.ReturnMoney ? "თანხის დაბრუნება" : (c.Status == CardChargeStatus.PacketChange ? "პაკეტის შეცვლა" : "წინასწარი დარიცხვა")))))),
                   }).ToPagedListAsync(pos, 50);

                decimal amount = _db.CardCharges.Where(c => c.Tdate >= fromDate && c.Tdate <= toDate).Sum(s => (decimal?)s.Amount) ?? 0;
                var columns = new[] {
                    new {name ="თარიღი", width = 10, column = "date", format = true },
                    new {name ="აბონენტი", width = 15, column = "abonent", format=false } ,
                    new {name ="ტიპი", width = 10, column = "type", format=false },
                    new {name ="ტელ", width = 10, column = "phone", format=false },
                    new {name ="დას. პუნქტი", width = 10, column = "region", format=false },
                    new {name ="სტატუსი", width = 10, column = "status", format=false },
                    new {name ="აბონენტის №", width = 10, column = "abonent_num", format=false } ,
                    new {name ="ბარათის №", width = 10, column = "card_num", format=false } ,
                    new {name ="პაკეტი", width = 10, column = "packets", format=false }, 
                    new {name ="პაკეტის თარიღი", width = 10, column = "packet_date", format = true },
                    new {name ="თანხა", width = 10, column = "amount", format=false },
                    new {name ="სტატუსი", width = 10, column = "charge_type", format=false },
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { amount = amount, cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetChargesSummary(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var rows = await _db.Cards.Where(c => c.CardCharges.Any(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate))
                    .OrderBy(c => c.AbonentNum).Select(c => new
                    {
                        abonent = c.Customer.Name + " " + c.Customer.LastName,
                        type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                        phone = c.Customer.Phone1,
                        status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                        abonent_num = c.AbonentNum,
                        card_num = c.CardNum,
                        packets = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                        packet_date = c.Subscribtions.FirstOrDefault(s => s.Status).Tdate,
                        amount = c.CardCharges.Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum()
                    }).ToPagedListAsync(pos, 50);

                decimal amount = _db.CardCharges.Where(c => c.Tdate >= fromDate && c.Tdate <= toDate).Sum(s => (decimal?)s.Amount) ?? 0;
                var columns = new[] {
                    new {name ="აბონენტი", width = 15, column = "abonent", format=false } ,
                    new {name ="ტიპი", width = 10, column = "type", format=false },
                    new {name ="ტელ", width = 10, column = "phone", format=false },
                    new {name ="სტატუსი", width = 10, column = "status", format=false },
                    new {name ="აბონენტის №", width = 10, column = "abonent_num", format=false } ,
                    new {name ="ბარათის №", width = 10, column = "card_num", format=false } ,
                    new {name ="პაკეტი", width = 10, column = "packets", format=false }, 
                    new {name ="პაკეტის თარიღი", width = 10, column = "packet_date", format = true },
                    new {name ="თანხა", width = 10, column = "amount", format=false },
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { amount = amount, cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetBalanceByCardsSummary(int pos = 1)
        {
            string[] date_from = Request["date_from"].Split('/');
            string[] date_to = Request["date_to"].Split('/');
            DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

            using (DataContext _db = new DataContext())
            {
                var rows = await _db.Cards.OrderBy(c => c.AbonentNum).Select(c => new
                {
                    abonent = c.Customer.Name + " " + c.Customer.LastName,
                    code = c.Customer.Code,
                    type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                    abonent_num = c.AbonentNum,
                    start_amount = c.Payments.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.CardCharges.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    payments = c.Payments.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    charges = c.CardCharges.Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum(),
                    end_amount = c.Payments.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.CardCharges.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                }).ToPagedListAsync(pos, 50);
                var columns = new[] {
                    new {name ="აბონენტი", width = 15, column = "abonent", format=false },
                    new {name ="პ/ნ", width = 10, column = "code", format=false },
                    new {name ="ტიპი", width = 10, column = "type", format=false },
                    new {name ="აბონენტის №", width = 10, column = "abonent_num", format=false } ,
                    new {name ="საწყისი  ნაშთი", width = 10, column = "start_amount", format=false },
                    new {name ="გადახდები", width = 10, column = "payments", format=false },
                    new {name ="დარიცხვები", width = 10, column = "charges", format=false } ,
                    new {name ="საბოლოო ნაშთი", width = 10, column = "end_amount", format=false } ,
                };

                string amount = (_db.Payments.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum()) + ";" +
                    _db.Payments.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() + ";" + _db.CardCharges.Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum() + ";" +
                    (_db.Payments.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum());

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { amount = amount, cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetBalanceByCardsAccountingSummary(int pos = 1)
        {
            string[] date_from = Request["date_from"].Split('/');
            string[] date_to = Request["date_to"].Split('/');
            DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

            DateTime d_dt = fromDate.AddDays(-1);
            DateTime chargeFromDate = new DateTime(d_dt.Year, d_dt.Month, d_dt.Day, 23, 59, 0);
            DateTime chargeToDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 58, 0);

            using (DataContext _db = new DataContext())
            {
                var rows = await _db.Cards.OrderBy(c => c.AbonentNum).Select(c => new
                {
                    abonent = c.Customer.Name + " " + c.Customer.LastName,
                    code = c.Customer.Code,
                    type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                    abonent_num = c.AbonentNum,
                    start_amount = c.Payments.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.CardCharges.Where(p => p.Tdate <= chargeFromDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    payments = c.Payments.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    charges = c.CardCharges.Where(cc => cc.Tdate >= chargeFromDate && cc.Tdate <= chargeToDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum(),
                    end_amount = c.Payments.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.CardCharges.Where(p => p.Tdate <= chargeToDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                }).ToPagedListAsync(pos, 50);
                var columns = new[] {
                    new {name ="აბონენტი", width = 15, column = "abonent", format=false },
                    new {name ="პ/ნ", width = 10, column = "code", format=false },
                    new {name ="ტიპი", width = 10, column = "type", format=false },
                    new {name ="აბონენტის №", width = 10, column = "abonent_num", format=false } ,
                    new {name ="საწყისი  ნაშთი", width = 10, column = "start_amount", format=false },
                    new {name ="გადახდები", width = 10, column = "payments", format=false },
                    new {name ="დარიცხვები", width = 10, column = "charges", format=false } ,
                    new {name ="საბოლოო ნაშთი", width = 10, column = "end_amount", format=false } ,
                };

                string amount = (_db.Payments.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate <= chargeFromDate).Select(p => p.Amount).DefaultIfEmpty().Sum()) + ";" +
                    _db.Payments.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() + ";" + _db.CardCharges.Where(cc => cc.Tdate >= chargeFromDate && cc.Tdate <= chargeToDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum() + ";" +
                    (_db.Payments.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate <= chargeToDate).Select(p => p.Amount).DefaultIfEmpty().Sum());

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { amount = amount, cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        public async Task<JsonResult> GetBalanceByAbonentsSummary(int pos = 1)
        {
            string[] date_from = Request["date_from"].Split('/');
            string[] date_to = Request["date_to"].Split('/');
            DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

            using (DataContext _db = new DataContext())
            {
                var rows = await _db.Customers.OrderBy(c => c.Id).Select(c => new
                        {
                            abonent = c.Name + " " + c.LastName,
                            code = c.Code,
                            type = c.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.JuridicalType == 1 ? ":კომუნალური" : (c.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                            start_amount = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.Cards.SelectMany(p => p.CardCharges).Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                            payments = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                            charges = c.Cards.SelectMany(p => p.CardCharges).Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum(),
                            end_amount = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.Cards.SelectMany(p => p.CardCharges).Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                        }).ToPagedListAsync(pos, 50);
                var columns = new[] {
                    new {name ="აბონენტი", width = 15, column = "abonent", format=false },
                    new {name ="პ/ნ", width = 10, column = "code", format=false },
                    new {name ="ტიპი", width = 10, column = "type", format=false },
                    new {name ="საწყისი  ნაშთი", width = 10, column = "start_amount", format=false },
                    new {name ="გადახდები", width = 10, column = "payments", format=false },
                    new {name ="დარიცხვები", width = 10, column = "charges", format=false } ,
                    new {name ="საბოლოო ნაშთი", width = 10, column = "end_amount", format=false } ,
                };

                string amount = (_db.Payments.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum()) + ";" +
                    _db.Payments.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() + ";" + _db.CardCharges.Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum() + ";" +
                    (_db.Payments.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum());

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { amount = amount, cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        public async Task<JsonResult> GetBalanceByAbonentsAccountingSummary(int pos = 1)
        {
            string[] date_from = Request["date_from"].Split('/');
            string[] date_to = Request["date_to"].Split('/');
            DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

            DateTime d_dt = fromDate.AddDays(-1);
            DateTime chargeFromDate = new DateTime(d_dt.Year, d_dt.Month, d_dt.Day, 23, 59, 0);
            DateTime chargeToDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 58, 0);

            using (DataContext _db = new DataContext())
            {
                var rows = await _db.Customers.OrderBy(c => c.Id).Select(c => new
                {
                    abonent = c.Name + " " + c.LastName,
                    code = c.Code,
                    type = c.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.JuridicalType == 1 ? ":კომუნალური" : (c.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                    start_amount = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.Cards.SelectMany(p => p.CardCharges).Where(p => p.Tdate <= chargeFromDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    payments = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    charges = c.Cards.SelectMany(p => p.CardCharges).Where(cc => cc.Tdate >= chargeFromDate && cc.Tdate <= chargeFromDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum(),
                    end_amount = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.Cards.SelectMany(p => p.CardCharges).Where(p => p.Tdate <= chargeToDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                }).ToPagedListAsync(pos, 50);
                var columns = new[] {
                    new {name ="აბონენტი", width = 15, column = "abonent", format=false },
                    new {name ="პ/ნ", width = 10, column = "code", format=false },
                    new {name ="ტიპი", width = 10, column = "type", format=false },
                    new {name ="საწყისი  ნაშთი", width = 10, column = "start_amount", format=false },
                    new {name ="გადახდები", width = 10, column = "payments", format=false },
                    new {name ="დარიცხვები", width = 10, column = "charges", format=false } ,
                    new {name ="საბოლოო ნაშთი", width = 10, column = "end_amount", format=false } ,
                };

                string amount = (_db.Payments.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate <= chargeFromDate).Select(p => p.Amount).DefaultIfEmpty().Sum()) + ";" +
                    _db.Payments.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() + ";" + _db.CardCharges.Where(cc => cc.Tdate >= chargeFromDate && cc.Tdate <= chargeToDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum() + ";" +
                    (_db.Payments.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate <= chargeToDate).Select(p => p.Amount).DefaultIfEmpty().Sum());

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { amount = amount, cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetPayments(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var rows = await _db.Payments.AsNoTracking()
                    .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                    .OrderByDescending(c => c.Tdate).Select(c => new
                    {
                        date = c.Tdate,
                        abonent = c.Card.Customer.Name + " " + c.Card.Customer.LastName,
                        type = c.Card.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Card.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Card.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                        phone = c.Card.Customer.Phone1,
                        status = c.Card.CardStatus == CardStatus.Active ? "აქტიური" : (c.Card.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.Card.CardStatus == CardStatus.Closed ? "დახურული" : (c.Card.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                        abonent_num = c.Card.AbonentNum,
                        card_num = c.Card.CardNum,
                        packets = c.Card.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                        packet_date = c.Card.Subscribtions.FirstOrDefault(s => s.Status).Tdate,
                        pay_type = c.PayType.Name,
                        amount = c.Amount
                    }).ToPagedListAsync(pos, 50);

                decimal amount = _db.Payments.Where(c => c.Tdate >= fromDate && c.Tdate <= toDate).Sum(s => (decimal?)s.Amount) ?? 0;
                var columns = new[] {
                    new {name ="თარიღი", width = 10, column = "date", format = true },
                    new {name ="აბონენტი", width = 15, column = "abonent", format=false },
                    new {name ="ტიპი", width = 10, column = "type", format=false },
                    new {name ="ტელ", width = 10, column = "phone", format=false },
                    new {name ="სტატუსი", width = 10, column = "status", format=false },
                    new {name ="აბონენტის №", width = 10, column = "abonent_num", format=false },
                    new {name ="ბარათის №", width = 10, column = "card_num", format=false },
                    new {name ="პაკეტი", width = 10, column = "packets", format=false }, 
                    new {name ="პაკეტის თარიღი", width = 10, column = "packet_date", format = true },
                    new {name ="გად. სახეობა", width = 10, column = "pay_type", format = false },
                    new {name ="თანხა", width = 10, column = "amount", format=false },
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

                return Json(new { amount = amount, cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetCardsCount(int pos = 1)
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('/');
                string[] date_to = Request["date_to"].Split('/');
                string fromDate = date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00";
                string toDate = date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59";

                string sql = @"declare @fromdate datetime = '" + fromDate + @"'
                               declare @todate datetime = '" + toDate + @"'
                              select status,count(count) as count from (select dbo.GetCardStatus(c.status) as status,
                              (case when not exists(select * from doc.CardLogs where card_id=c.id and close_tdate between @fromdate and @todate) then 
                              (select count(*) from doc.CardLogs where card_id=c.id and (select top(1) status from doc.CardLogs where card_id=c.id and close_tdate <= @fromdate order by close_tdate desc) = dbo.GetLogStatusByCardStatus(c.status)) else
                              (select count(*) from doc.CardLogs where card_id=c.id and close_tdate between @fromdate and @todate and status=dbo.GetLogStatusByCardStatus(c.status)) end) AS [count] from book.Cards AS c 
                              inner join book.customers as ct on ct.id=c.customer_id and type=" + Request["sign"] + ") as cards group by status";

                var rows = await _db.Database.SqlQuery<CardReport>(sql).ToRawPagedListAsync(10, 1, 50);

                var columns = new[] {
                    new {name ="სტატუსი", width = 10, column = "status" },
                    new {name ="რაოდენობა", width = 15, column = "count"} ,
                };

                System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                            new ViewContext(), new ViewPage());

                return Json(new { cols = columns, rows = rows, paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, rows, p => p.ToString(), PagedListRenderOptions.TwitterBootstrapPager).ToHtmlString() });

            }
        }


        public async Task<FileResult> GetChannelsExport()
        {
            using (DataContext _db = new DataContext())
            {
                var data = await _db.PackageChannels
                    .Select(p => new { package = p.Package.Name, price = p.Package.Price, jurid_price = p.Package.JuridPrice, min_price = p.Package.MinPrice, channel = p.Channel.Name })
                    .OrderBy(p => p.package).ToListAsync();

                XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "პაკეტი"),
                        new XElement("name", "ფასი"),
                        new XElement("name", "იურიდ. ფასი"),
                        new XElement("name", "მინ. ფასი"),
                        new XElement("name", "არხი")),
                    data.Select(c => new XElement("data",
                            new XElement("package", c.package),
                            new XElement("price", c.price),
                            new XElement("jurid_price", c.jurid_price),
                            new XElement("min_price", c.min_price),
                            new XElement("channel", c.channel)
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ChannelsResult.xlsx");
            }
        }

        public async Task<FileResult> GetMaregChannelsExport()
        {
            using (DataContext _db = new DataContext())
            {
                var data = await _db.PackageChannels
                    .Select(p => new { package = p.Package.Name, price = p.Package.Price, channel = p.Channel.Name })
                    .OrderBy(p => p.package).ToListAsync();

                XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "პაკეტის დასახელება"),
                        new XElement("name", "პაკეტის ღირებულება"),
                        new XElement("name", "არხების ჩამონათვალი")),
                    data.Select(c => new XElement("data",
                            new XElement("package", c.package),
                            new XElement("price", c.price),
                            new XElement("channel", c.channel)
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ChannelsResult.xlsx");
            }
        }

        public FileResult GetForm1_1Export()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var initial_data = new List<string> { "სააბონენტო გადასახადი", "ტექნიკური მომსახურება", "სხვა" };
                var rows = initial_data.Select(c => new
                {
                    type = "საცალო",
                    service = "მაუწყებლობის ტრანზიტი - " + c,
                    money_in = c != "სააბონენტო გადასახადი" ? 0 : _db.CardCharges.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Where(p => p.Status != CardChargeStatus.ReturnMoney && p.Status != CardChargeStatus.Service).Select(p => p.Amount).DefaultIfEmpty().Sum() / (decimal)1.18,
                    dxg = c != "სააბონენტო გადასახადი" ? 0 : _db.CardCharges.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Where(p => p.Status != CardChargeStatus.ReturnMoney && p.Status != CardChargeStatus.Service).Select(p => p.Amount).DefaultIfEmpty().Sum() - _db.CardCharges.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Where(p => p.Status != CardChargeStatus.ReturnMoney && p.Status != CardChargeStatus.Service).Select(p => p.Amount).DefaultIfEmpty().Sum() / (decimal)1.18,
                    aqciz = 0
                }).ToList();

                XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "საცალო/საბითუმო"),
                        new XElement("name", "მომსახურება და ქვეკატეგორია"),
                        new XElement("name", "შემოსავალი"),
                        new XElement("name", "დღგ"),
                        new XElement("name", "აქციზი")),
                    rows.Select(c => new XElement("data",
                            new XElement("type", c.type),
                            new XElement("service", c.service),
                            new XElement("money_in", Math.Round(c.money_in, 2)),
                            new XElement("dxg", Math.Round(c.dxg, 2)),
                            new XElement("aqciz", c.aqciz)
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Form1_1Result.xlsx");
            }
        }

        public async Task<FileResult> GetForm4_3Export()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                string sql = @"DECLARE @start_date DATETIME='" + fromDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
DECLARE @end_date DATETIME='" + toDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
;WITH Charges(amount, tdate, card_id)
AS
(
 SELECT amount, tdate, card_id FROM doc.CardCharges WHERE tdate BETWEEN @start_date AND @end_date AND [status] NOT IN (4,5)
)
SELECT 
city,
N'მიტრისი (ციფრული)' AS [type],
N'დიახ' AS coding, 
q.packet AS package,
(SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND EXISTS(SELECT * FROM Charges WHERE  card_id=c.id)) AS abonents_count,
(SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND EXISTS(SELECT * FROM Charges WHERE  card_id=c.id)) AS decoders_count,
(SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id OUTER APPLY (SELECT TOP(1) tdate, card_id FROM doc.CardCharges WHERE card_id=c.id ORDER BY tdate) AS ch  WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND ch.tdate IS NOT NULL AND ch.tdate BETWEEN @start_date AND @end_date) AS n_abonents_count,
(SELECT ISNULL(SUM(amount),0) FROM Charges AS p WHERE card_id IN (SELECT c.id FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city  AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1))) / 1.18 AS money_in,
((SELECT ISNULL(SUM(amount),0) FROM Charges AS p WHERE card_id IN (SELECT c.id FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1))) - (SELECT ISNULL(SUM(amount),0) FROM Charges AS p WHERE p.card_id IN (SELECT c.id FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1)))/ 1.18) AS dxg
FROM (SELECT
(a.village + ' - ' + a.city) AS city,
STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') AS packet 

FROM doc.Subscribes AS s
INNER JOIN book.Cards AS c ON c.id = s.card_id
INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE c.status!=5 AND s.status=1) AS q GROUP BY q.city, q.packet 
HAVING (SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND EXISTS(SELECT * FROM Charges WHERE card_id=c.id)) > 0";

                var data = await _db.Database.SqlQuery<Form4_3_Report>(sql).ToListAsync();

                XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "დასახლებული პუნქტი"),
                        new XElement("name", "მიწოდების ტიპი"),
                        new XElement("name", "კოდირებული"),
                        new XElement("name", "პაკეტი"),
                        new XElement("name", "აბონენტები"),
                        new XElement("name", "დეკოდერების რაოდენობა"),
                        new XElement("name", "ახალი აბონენტების რაოდენობა"),
                        new XElement("name", "სააბონენტო შემოსავალი"),
                        new XElement("name", "დღგ")),
                    data.Select(c => new XElement("data",
                            new XElement("city", c.city),
                            new XElement("type", c.type),
                            new XElement("coding", c.coding),
                            new XElement("package", c.package),
                            new XElement("abonents_count", c.abonents_count),
                            new XElement("decoders_count", c.decoders_count),
                            new XElement("n_abonents_count", c.n_abonents_count),
                            new XElement("amount_in", Math.Round(c.money_in, 2)),
                            new XElement("dxg", Math.Round(c.dxg, 2))
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Form4_3Result.xlsx");
            }
        }

        public async Task<FileResult> GetForm4_4Export()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                string sql = @"DECLARE @start_date DATETIME='" + fromDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
DECLARE @end_date DATETIME='" + toDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'
;WITH Charges(tdate, card_id)
AS
(
 SELECT tdate, card_id FROM doc.CardCharges WHERE tdate BETWEEN @start_date AND @end_date AND [status] NOT IN (4,5)
)
SELECT 
city,
N'მიტრისი (ციფრული)' AS [type],
N'დიახ' AS coding, 
q.packet AS package,
(SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND EXISTS(SELECT * FROM Charges WHERE  card_id=c.id)) AS abonents_count,
(SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id OUTER APPLY (SELECT TOP(1) tdate, card_id FROM doc.CardCharges WHERE card_id=c.id ORDER BY tdate) AS ch  WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND ch.tdate IS NOT NULL AND ch.tdate BETWEEN @start_date AND @end_date) AS n_abonents_count
FROM (SELECT
(a.village + ' - ' + a.city) AS city,
STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') AS packet 

FROM doc.Subscribes AS s
INNER JOIN book.Cards AS c ON c.id = s.card_id
INNER JOIN book.Customers AS a ON a.id=c.customer_id


WHERE c.status!=5 AND s.status=1) AS q GROUP BY q.city, q.packet
 HAVING (SELECT COUNT(*) FROM book.Cards AS c INNER JOIN book.Customers AS a ON a.id=c.customer_id WHERE (a.village + ' - ' + a.city)= q.city AND q.packet IN (SELECT DISTINCT STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.card_id=c.id AND s.status=1) AND EXISTS(SELECT * FROM Charges WHERE  card_id=c.id)) > 0";

                var data = await _db.Database.SqlQuery<Form4_4Report>(sql).ToListAsync();

                var columns = new[] { 
                    new {name ="დასახლებული პუნქტი", width = 15, column = "city" } , 
                    new {name ="მიწოდების ტიპი", width = 15, column = "type" }, 
                    new {name ="კოდირებული", width = 10, column = "coding" } , 
                    new {name ="პაკეტი", width = 10, column = "package" }, 
                    new {name ="აბონენტები", width = 10, column = "abonents_count" }, 
                    new {name ="ახალი აბონენტების რაოდენობა", width = 10, column = "n_abonents_count" }
                };

                XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "დასახლებული პუნქტი"),
                        new XElement("name", "მიწოდების ტიპი"),
                        new XElement("name", "კოდირებული"),
                        new XElement("name", "პაკეტი"),
                        new XElement("name", "აბონენტები"),
                        new XElement("name", "ახალი აბონენტების რაოდენობა")),
                    data.Select(c => new XElement("data",
                            new XElement("city", c.city),
                            new XElement("type", c.type),
                            new XElement("coding", c.coding),
                            new XElement("package", c.package),
                            new XElement("abonents_count", c.abonents_count),
                            new XElement("n_abonents_count", c.n_abonents_count)
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Form4_4Result.xlsx");
            }
        }

        public async Task<FileResult> GetPacketsExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                DateTime nDate = toDate.AddMonths(-1);
                bool isActive = Convert.ToBoolean(Request["sign"]);

                var data = await _db.SubscriptionPackages.AsNoTracking()
                     .Where(c => c.Subscribtion.Status == isActive)
                     .Where(c => c.Subscribtion.Card.CardStatus != CardStatus.Canceled)
                     .Where(c => c.Subscribtion.Card.Tdate >= fromDate && c.Subscribtion.Card.Tdate <= toDate)
                     .GroupBy(c => c.PackageId)
                     .Select(s => new
                     {
                         package = s.FirstOrDefault().Package.Name,
                         price = s.FirstOrDefault().Package.Price,
                         jurid_price = s.FirstOrDefault().Package.JuridPrice,
                         channels_count = s.FirstOrDefault().Package.PackageChannels.Count(),
                         abonents_count = s.Select(ss => ss.Subscribtion.Card.CustomerId).Distinct().Count(),
                         decoders_count = s.Select(ss => ss.Subscribtion.Card.ReceiverId).Count(),
                         n_abonents_count = _db.Customers.Where(c => c.Tdate >= nDate && c.Tdate <= toDate).Where(c => c.Cards.Any(cc => s.Select(p => p.Subscribtion.CardId).Contains(cc.Id))).Distinct().Count(),
                         n_decoders_count = _db.Receivers.Count(c => c.Cards.Where(cc => cc.Tdate >= nDate && cc.Tdate <= toDate).Any(cc => s.Select(p => p.Subscribtion.CardId).Contains(cc.Id))),
                         amount_in = _db.Payments.Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Amount).Sum() ?? 0,
                         service_out_cash = _db.CardServices.Where(ss => ss.PayType == CardServicePayType.Cash).Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Service.Amount).Sum() ?? 0,
                         service_out_not_cash = _db.CardServices.Where(ss => ss.PayType == CardServicePayType.NotCash).Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Service.Amount).Sum() ?? 0
                     }).OrderBy(p => p.package).ToListAsync();

                XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "პაკეტი"),
                        new XElement("name", "ფასი"),
                        new XElement("name", "იურიდ. ფასი"),
                        new XElement("name", "არხების რაოდენობა"),
                        new XElement("name", "აბონენტების რაოდენობა"),
                        new XElement("name", "დეკოდერების რაოდენობა"),
                        new XElement("name", "ახალი აბონენტების რაოდენობა"),
                        new XElement("name", "ახალი დეკოდერების რაოდენობა"),
                        new XElement("name", "შემოსული თანხა"),
                        new XElement("name", "გაწეული მომსახურება ნაღდი"),
                        new XElement("name", "გაწეული მომსახურება უნაღდო")),
                    data.Select(c => new XElement("data",
                            new XElement("package", c.package),
                            new XElement("price", c.price),
                            new XElement("jurid_price", c.jurid_price),
                            new XElement("channels_count", c.channels_count),
                            new XElement("abonents_count", c.abonents_count),
                            new XElement("decoders_count", c.decoders_count),
                            new XElement("n_abonents_count", c.n_abonents_count),
                            new XElement("n_decoders_count", c.n_decoders_count),
                            new XElement("amount_in", c.amount_in),
                            new XElement("service_out_cash", c.service_out_cash),
                            new XElement("service_out_not_cash", c.service_out_not_cash)
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PacketsResult.xlsx");
            }
        }

        public async Task<FileResult> GetPacketsByAbonentTypeExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');

                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                DateTime nDate = toDate.AddMonths(-1);
                bool isActive = Convert.ToBoolean(Request["sign"]);

                var data = await _db.SubscriptionPackages.AsNoTracking()
                     .Where(c => c.Subscribtion.Status == isActive)
                     .Where(c => c.Subscribtion.Card.CardStatus != CardStatus.Canceled)
                     .Where(c => c.Subscribtion.Card.Tdate >= fromDate && c.Subscribtion.Card.Tdate <= toDate)
                     .GroupBy(c => new { c.PackageId, c.Subscribtion.Card.Customer.Type })
                     .Select(s => new
                     {
                         type = s.FirstOrDefault().Subscribtion.Card.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (s.FirstOrDefault().Subscribtion.Card.Customer.JuridicalType == 1 ? ":კომუნალური" : (s.FirstOrDefault().Subscribtion.Card.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                         package = s.FirstOrDefault().Package.Name,
                         channels_count = s.FirstOrDefault().Package.PackageChannels.Count(),
                         abonents_count = s.Select(ss => ss.Subscribtion.Card.CustomerId).Distinct().Count(),
                         decoders_count = s.Select(ss => ss.Subscribtion.Card.ReceiverId).Count(),
                         n_abonents_count = _db.Customers.Where(c => c.Tdate >= nDate && c.Tdate <= toDate).Where(c => c.Cards.Any(cc => s.Select(p => p.Subscribtion.CardId).Contains(cc.Id))).Distinct().Count(),
                         n_decoders_count = _db.Receivers.Count(c => c.Cards.Where(cc => cc.Tdate >= nDate && cc.Tdate <= toDate).Any(cc => s.Select(p => p.Subscribtion.CardId).Contains(cc.Id))),
                         amount_in = _db.Payments.Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Amount).Sum() ?? 0,
                         service_out_cash = _db.CardServices.Where(ss => ss.PayType == CardServicePayType.Cash).Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Service.Amount).Sum() ?? 0,
                         service_out_not_cash = _db.CardServices.Where(ss => ss.PayType == CardServicePayType.NotCash).Where(ss => s.Select(p => p.Subscribtion.CardId).Contains(ss.CardId)).Select(a => (decimal?)a.Service.Amount).Sum() ?? 0
                     }).OrderBy(p => p.package).ToListAsync();

                XElement element = new XElement("root",
                   new XElement("columns",
                       new XElement("name", "აბონენტის ტიპი"),
                       new XElement("name", "პაკეტი"),
                       new XElement("name", "არხების რაოდენობა"),
                       new XElement("name", "აბონენტების რაოდენობა"),
                       new XElement("name", "დეკოდერების რაოდენობა"),
                       new XElement("name", "ახალი აბონენტების რაოდენობა"),
                       new XElement("name", "ახალი დეკოდერების რაოდენობა"),
                       new XElement("name", "შემოსული თანხა"),
                       new XElement("name", "გაწეული მომსახურება")),
                   data.Select(c => new XElement("data",
                           new XElement("type", c.type),
                           new XElement("package", c.package),
                           new XElement("channels_count", c.channels_count),
                           new XElement("abonents_count", c.abonents_count),
                           new XElement("decoders_count", c.decoders_count),
                           new XElement("n_abonents_count", c.n_abonents_count),
                           new XElement("n_decoders_count", c.n_decoders_count),
                           new XElement("amount_in", c.amount_in),
                           new XElement("service_out_cash", c.service_out_cash),
                           new XElement("service_out_not_cash", c.service_out_not_cash)
                           )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PacketsByAbonentTypeResult.xlsx");

            }
        }

        public async Task<FileResult> GetCardsExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                DateTime nDate = toDate.AddMonths(-1);
                int sign = Convert.ToInt32(Request["sign"]);

                var data = await _db.Cards.AsNoTracking()
                          .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                          .Where(c => sign == 0 ? true : (sign == 1 ? c.CardServices.Count > 0 : c.CardServices.Count == 0))
                          .OrderByDescending(c => c.Tdate)
                          .Select(c => new
                          {
                              date = c.Tdate,
                              num = c.CustomerId,
                              abonent_num = c.AbonentNum,
                              doc_num = c.DocNum ?? "",
                              card_num = c.CardNum,
                              type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                              code = c.Customer.Code,
                              name = c.Customer.Name + " " + c.Customer.LastName,
                              region = c.Customer.Region,
                              city = c.Customer.City + (string.IsNullOrEmpty(c.Customer.Village) ? "" : "/" + c.Customer.Village),
                              village = c.Customer.Village,
                              district = c.Customer.District,
                              address = c.Customer.Address,
                              phone1 = c.Customer.Phone1,
                              phone2 = c.Customer.Phone2 ?? "",
                              status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : (c.CardStatus == CardStatus.Blocked ? "დაბლოკილი" : "დაპაუზებული")))),
                              days = SqlFunctions.DateDiff("dd", _db.CardLogs.Where(l => l.CardId == c.Id).OrderByDescending(l => l.Date).FirstOrDefault().Date, DateTime.Now),
                              packet = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                              discount = c.Discount,
                              summ_in = c.Payments.Select(p => (decimal?)p.Amount).Sum() ?? 0,
                              service_out = c.CardCharges.Select(cc => (decimal?)cc.Amount).Sum() ?? 0,
                              balance = (c.Payments.Select(p => (decimal?)p.Amount).Sum() ?? 0) - (c.CardCharges.Select(p => (decimal?)p.Amount).Sum()) ?? 0,
                              user = c.User.Name,
                              comment = c.Customer.Desc,
                              first_service = c.CardServices.Select(ss => ss.Service.Name).FirstOrDefault()
                          }).ToListAsync();

                XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "თარიღი"),
                        new XElement("name", "№"),
                        new XElement("name", "აბონენტის №"),
                        new XElement("name", "ხელშ. №"),
                        new XElement("name", "ბარათის №"),
                        new XElement("name", "აბონენტის ტიპი"),
                        new XElement("name", "პ/ნ"),
                        new XElement("name", "აბონენტი"),
                        new XElement("name", "რეგიონი"),
                        new XElement("name", "ქალაქი/სოფ."),
                        new XElement("name", "რაიონი"),
                        new XElement("name", "უბანი"),
                        new XElement("name", "მისამართი"),
                        new XElement("name", "ტელ1"),
                        new XElement("name", "ტელ2"),
                        new XElement("name", "სტატუსი"),
                        new XElement("name", "დღე"),
                        new XElement("name", "პაკეტი"),
                        new XElement("name", "%"),
                        new XElement("name", "შემოსული თანხა"),
                        new XElement("name", "გაწეული მომსახურება"),
                        new XElement("name", "ბალანსი"),
                        new XElement("name", "მომხარებელი"),
                        new XElement("name", "კომენტარი"),
                        new XElement("name", "პირველი გაწ. მომს")),
                    data.Select(c => new XElement("data",
                            new XElement("date", c.date),
                            new XElement("num", c.num),
                            new XElement("abonent_num", c.abonent_num),
                            new XElement("doc_num", c.doc_num),
                            new XElement("card_num", c.card_num),
                            new XElement("type", c.type),
                            new XElement("code", c.code),
                            new XElement("name", c.name),
                            new XElement("region", c.region),
                            new XElement("city", c.city),
                            new XElement("village", c.village),
                            new XElement("district", c.district),
                            new XElement("address", c.address),
                            new XElement("phone1", c.phone1),
                            new XElement("phone2", c.phone2),
                            new XElement("status", c.status),
                            new XElement("days", c.days),
                            new XElement("packet", c.packet),
                            new XElement("discount", c.discount),
                            new XElement("summ_in", c.summ_in),
                            new XElement("service_out", c.service_out),
                            new XElement("balance", c.balance),
                            new XElement("user", c.user),
                            new XElement("comment", c.comment),
                            new XElement("first_service", c.first_service)
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CardsResult.xlsx");
            }
        }

        public async Task<FileResult> GetCardsByAbonentsExport()
        {
             using (DataContext _db = new DataContext())
             {
                 var data = await _db.Customers.Select(c => new
                 {
                     name = c.Name + " " + c.LastName,
                     code = c.Code,
                     type = c.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.JuridicalType == 1 ? ":კომუნალური" : (c.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                     region = c.Region,
                     city = c.City + (string.IsNullOrEmpty(c.Village) ? "" : "/" + c.Village),
                     village = c.Village,
                     phone = c.Phone1,
                     cards_count = c.Cards.Where(cc => cc.CardStatus != CardStatus.Canceled).Count(),
                     active_cards_count = c.Cards.Where(cc => cc.CardStatus == CardStatus.Active).Count(),
                     closed_cards_count = c.Cards.Where(cc => cc.CardStatus == CardStatus.Closed).Count(),
                     paused_cards_count = c.Cards.Where(cc => cc.CardStatus == CardStatus.Paused).Count(),
                     montaged_cards_count = c.Cards.Where(cc => cc.CardStatus == CardStatus.FreeDays).Count(),
                 }).ToListAsync();

                 XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "აბონენტი"),
                        new XElement("name", "პ/ნ"),
                        new XElement("name", "ტიპი"),
                        new XElement("name", "რეგიონი"),
                        new XElement("name", "ქალაქი/სოფ."),
                        new XElement("name", "რაიონი"),
                        new XElement("name", "ტელ1"),
                        new XElement("name", "ბარათები სულ"),
                        new XElement("name", "აქტიური ბარათები"),
                        new XElement("name", "გათიშული ბარათები"),
                        new XElement("name", "დაპაუზებული ბარათები"),
                        new XElement("name", "მონტაჟში ბარათები")),
                    data.Select(c => new XElement("data",
                            new XElement("name", c.name),
                            new XElement("code", c.code),
                            new XElement("type", c.type),
                            new XElement("region", c.region),
                            new XElement("city", c.city),
                            new XElement("village", c.village),
                            new XElement("phone1", c.phone),
                            new XElement("cards_count", c.cards_count),
                            new XElement("active_cards_count", c.active_cards_count),
                            new XElement("closed_cards_count", c.closed_cards_count),
                            new XElement("paused_cards_count", c.paused_cards_count),
                            new XElement("montaged_cards_count", c.montaged_cards_count)
                            )));

                 return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GetCardsByAbonents.xlsx");

             }
        }

        public async Task<FileResult> GetLostCardsExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                DateTime nDate = toDate.AddMonths(-1);
                int sign = Convert.ToInt32(Request["sign"]);

                var data = await _db.Cards.AsNoTracking()
                         .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                         .Where(c => c.CardStatus == CardStatus.Canceled)
                         .OrderByDescending(c => c.Tdate)
                         .Select(c => new
                         {
                             abonent_num = c.AbonentNum,
                             card_num = c.CardNum,
                             type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                             code = c.Customer.Code,
                             name = c.Customer.Name + " " + c.Customer.LastName,
                             region = c.Customer.Region,
                             city = c.Customer.City + (string.IsNullOrEmpty(c.Customer.Village) ? "" : "/" + c.Customer.Village),
                             address = c.Customer.Address,
                             phone1 = c.Customer.Phone1,
                             phone2 = c.Customer.Phone2 ?? "",
                             status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                             packet = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                             summ_in = c.Payments.Select(p => (decimal?)p.Amount).Sum() ?? 0,
                             service_out = c.CardServices.Select(cc => (decimal?)cc.Service.Amount).Sum() ?? 0,
                             balance = c.Payments.Select(p => (decimal?)p.Amount).Sum() ?? 0 - c.CardCharges.Select(p => (decimal?)p.Amount).Sum() ?? 0
                         }).ToListAsync();

                XElement element = new XElement("root",
                   new XElement("columns",
                       new XElement("name", "აბონენტის №"),
                       new XElement("name", "ბარათის №"),
                       new XElement("name", "აბონენტის ტიპი"),
                       new XElement("name", "პ/ნ"),
                       new XElement("name", "აბონენტი"),
                       new XElement("name", "რეგიონი"),
                       new XElement("name", "ქალაქი/სოფ."),
                       new XElement("name", "მისამართი"),
                       new XElement("name", "ტელ1"),
                       new XElement("name", "ტელ2"),
                       new XElement("name", "სტატუსი"),
                       new XElement("name", "პაკეტი"),
                       new XElement("name", "შემოსული თანხა"),
                       new XElement("name", "გაწეული მომსახურება")),
                   data.Select(c => new XElement("data",
                           new XElement("abonent_num", c.abonent_num),
                           new XElement("card_num", c.card_num),
                           new XElement("type", c.type),
                           new XElement("code", c.code),
                           new XElement("name", c.name),
                           new XElement("region", c.region),
                           new XElement("city", c.city),
                           new XElement("address", c.address),
                           new XElement("phone1", c.phone1),
                           new XElement("phone2", c.phone2),
                           new XElement("status", c.status),
                           new XElement("packet", c.packet),
                           new XElement("summ_in", c.summ_in),
                           new XElement("service_out", c.service_out)
                           )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LostCardsResult.xlsx");
            }
        }

        //REAL REPORTS EXPORT
        public async Task<FileResult> GetCardsByStatusExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);
                int sign = Convert.ToInt32(Request["sign"]);
                CardStatus status = (CardStatus)sign;

                var data = await _db.Cards.AsNoTracking()
                          .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                          .Where(c => sign == -1 ? true : (sign == 5 ? c.Mode == 1 : c.CardStatus == status))
                          .OrderByDescending(c => c.Tdate)
                          .Select(c => new
                          {
                              date = c.Tdate,
                              name = c.Customer.Name + " " + c.Customer.LastName,
                              code = c.Customer.Code,
                              type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                              address = c.Address,
                              doc_num = c.DocNum,
                              abonent_num = c.AbonentNum,
                              card_num = c.CardNum,
                              phone1 = c.Customer.Phone1,
                              status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                              balance = (_db.Payments.Where(p => p.CardId == c.Id).Sum(p => (decimal?)p.Amount) ?? 0) - (_db.CardCharges.Where(p => p.CardId == c.Id).Sum(p => (decimal?)p.Amount) ?? 0)
                          }).ToListAsync();

                XElement element = new XElement("root",
                   new XElement("columns",
                       new XElement("name", "თარიღი"),
                       new XElement("name", "აბონენტი"),
                       new XElement("name", "პ/ნ"),
                       new XElement("name", "აბონენტის ტიპი"),
                       new XElement("name", "მისამართი"),
                       new XElement("name", "ხელშ. №"),
                       new XElement("name", "აბონენტის №"),
                       new XElement("name", "ბარათის №"),
                       new XElement("name", "ტელ1"),
                       new XElement("name", "სტატუსი"),
                       new XElement("name", "ბალანსი")),
                   data.Select(c => new XElement("data",
                       new XElement("name", c.date),
                       new XElement("name", c.name),
                       new XElement("code", c.code),
                       new XElement("type", c.type),
                       new XElement("address", c.address),
                       new XElement("doc_num", c.doc_num),
                       new XElement("abonent_num", c.abonent_num),
                       new XElement("card_num", c.card_num),
                       new XElement("phone1", c.phone1),
                       new XElement("status", c.status),
                       new XElement("balance", c.balance)
                       )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CardsByStatus.xlsx");

            }
        }

        public async Task<FileResult> GetChargesExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var data = await _db.CardCharges.AsNoTracking()
                    .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                    .OrderByDescending(c => c.Tdate).Select(c => new
                    {
                        date = c.Tdate,
                        abonent = c.Card.Customer.Name + " " + c.Card.Customer.LastName,
                        type = c.Card.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Card.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Card.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                        phone = c.Card.Customer.Phone1,
                        region = c.Card.Customer.Village + "-" + c.Card.Customer.City,
                        status = c.Card.CardStatus == CardStatus.Active ? "აქტიური" : (c.Card.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.Card.CardStatus == CardStatus.Closed ? "დახურული" : (c.Card.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                        abonent_num = c.Card.AbonentNum,
                        card_num = c.Card.CardNum,
                        packets = c.Card.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                        packet_date = c.Card.Subscribtions.FirstOrDefault(s => s.Status).Tdate,
                        amount = c.Amount,
                        charge_type = c.Status == CardChargeStatus.Daily ? "დღიური დარიცხვა" : (c.Status == CardChargeStatus.Pause ? "პაუზის დარიხხვა" : (c.Status == CardChargeStatus.Pen ? "ჯარიმის დარიცხვა" : (c.Status == CardChargeStatus.PenDaily ? "ჯარიმის დღიური დარიცხვა" : (c.Status == CardChargeStatus.Service ? "გაწეული მომსახურების დარიცხვა" : (c.Status == CardChargeStatus.ReturnMoney ? "თანხის დაბრუნება" : (c.Status == CardChargeStatus.PacketChange ? "პაკეტის შეცვლა" : "წინასწარი დარიცხვა")))))),
                    }).ToListAsync();

                XElement element = new XElement("root",
                   new XElement("columns",
                       new XElement("name", "თარიღი"),
                       new XElement("name", "აბონენტი"),
                       new XElement("name", "ტიპი"),
                       new XElement("name", "ტელ"),
                       new XElement("name", "დას. პუნქტი"),
                       new XElement("name", "სტატუსი"),
                       new XElement("name", "აბონენტის №"),
                       new XElement("name", "ბარათის №"),
                       new XElement("name", "პაკეტი"),
                       new XElement("name", "პაკეტის თარიღი"),
                       new XElement("name", "თანხა"),
                       new XElement("name", "სტატუსი")),
                   data.Select(c => new XElement("data",
                       new XElement("date", c.date),
                       new XElement("abonent", c.abonent),
                       new XElement("type", c.type),
                       new XElement("phone", c.phone),
                       new XElement("region", c.region),
                       new XElement("status", c.status),
                       new XElement("abonent_num", c.abonent_num),
                       new XElement("card_num", c.card_num),
                       new XElement("packets", c.packets),
                       new XElement("packet_date", c.packet_date),
                       new XElement("amount", c.amount),
                       new XElement("charge_type", c.charge_type)
                       )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CardCharges.xlsx");
            }
        }

        public async Task<FileResult> GetChargesSummaryExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

               
                var data = await _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Include("CardCharges").Where(c => c.CardCharges.Any(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate)).ToListAsync();
                //data.Select(c => new mod
                //{
                //    abonent = c.Customer.Name + " " + c.Customer.LastName,
                //    code = c.Customer.Code,
                //    type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                //    phone = c.Customer.Phone1,
                //    status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                //    abonent_num = c.AbonentNum,
                //    card_num = c.CardNum,
                //    packets = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                //    packet_date = c.Subscribtions.FirstOrDefault(s => s.Status).Tdate,
                //    amount = c.CardCharges.Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum()


                //});
                List<ReportCardChargesSummary> datasum = new List<ReportCardChargesSummary>();
                foreach(var c in data)
                {
                    ReportCardChargesSummary mods = new ReportCardChargesSummary();

                    mods.abonent = c.Customer.Name + " " + c.Customer.LastName;
                    mods.code = c.Customer.Code;
                    mods.type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური";
                    mods.phone = c.Customer.Phone1;
                    mods.status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული")));
                    mods.abonent_num = c.AbonentNum;
                    mods.card_num = c.CardNum;
                    mods.packets = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList();
                    mods.packet_date = c.Subscribtions.FirstOrDefault(s => s.Status).Tdate;
                    mods.amount = c.CardCharges.Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum();
                    datasum.Add(mods);
                }
                //.OrderBy(c => c.AbonentNum).Select(c => new
                //{
                //    abonent = c.Customer.Name + " " + c.Customer.LastName,
                //    code = c.Customer.Code,
                //    type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                //    phone = c.Customer.Phone1,
                //    status = c.CardStatus == CardStatus.Active ? "აქტიური" : (c.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.CardStatus == CardStatus.Closed ? "დახურული" : (c.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                //    abonent_num = c.AbonentNum,
                //    card_num = c.CardNum,
                //    packets = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                //    packet_date = c.Subscribtions.FirstOrDefault(s => s.Status).Tdate,
                //    amount = c.CardCharges.Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum()
                //}).ToListAsync();
                //                var data = _db.Database.SqlQuery<mod>(@"SELECT DISTINCT cu.name+' '+cu.lastname AS abonent,cu.code,cu.type,cu.phone1 AS phone,c.status,c.abonent_num,c.card_num,
                //STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS packets,
                // s.tdate AS packet_date,
                // (SELECT SUM(cc.amount) FROM doc.CardCharges AS cc WHERE cc.card_id=c.id and cc.tdate between '2018-09-01 00:00:01' and '2018-09-30 23:59:01'   ) AS amount  FROM book.Cards AS c 
                // inner join book.Customers AS cu ON c.customer_id=cu.id
                //  inner join doc.Subscribes AS s ON s.card_id=c.id 
                //  inner join doc.CardCharges AS cs On cs.card_id=c.id where c.tdate between '" + fromDate+"' and '"+toDate+"' and c.status!=4").ToList();

                XElement element = new XElement("root",
                   new XElement("columns",
                       new XElement("name", "აბონენტი"),
                       new XElement("name", "პ/ნ"),
                       new XElement("name", "ტიპი"),
                       new XElement("name", "ტელ"),
                       new XElement("name", "სტატუსი"),
                       new XElement("name", "აბონენტის №"),
                       new XElement("name", "ბარათის №"),
                       new XElement("name", "პაკეტი"),
                       new XElement("name", "პაკეტის თარიღი"),
                       new XElement("name", "თანხა")),
                   datasum.Select(c => new XElement("data",
                       new XElement("abonent", c.abonent),
                       new XElement("code", c.code),
                       new XElement("type", c.type),
                       new XElement("phone", c.phone),
                       new XElement("status", c.status),
                       new XElement("abonent_num", c.abonent_num),
                       new XElement("card_num", c.card_num),
                       new XElement("packets", c.packets),
                       new XElement("packet_date", c.packet_date),
                       new XElement("amount", c.amount)
                       )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CardChargesSummary.xlsx");
            }
        }

        public async Task<FileResult> GetBalanceByCardsSummaryExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var data = await _db.Cards.OrderBy(c => c.AbonentNum).Select(c => new
                        {
                            abonent = c.Customer.Name + " " + c.Customer.LastName,
                            type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                            abonent_num = c.AbonentNum,
                            start_amount = c.Payments.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.CardCharges.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                            payments = c.Payments.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                            charges = c.CardCharges.Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum(),
                            end_amount = c.Payments.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.CardCharges.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                        }).ToListAsync();

                XElement element = new XElement("root",
                  new XElement("columns",
                      new XElement("name", "აბონენტი"),
                      new XElement("name", "ტიპი"),
                      new XElement("name", "აბონენტის №"),
                      new XElement("name", "საწყისი ნაშთი"),
                      new XElement("name", "გადახდები"),
                      new XElement("name", "დარიცხვები"),
                      new XElement("name", "საბოლოო ნაშთი")),
                  data.Select(c => new XElement("data",
                      new XElement("abonent", c.abonent),
                      new XElement("type", c.type),
                      new XElement("abonent_num", c.abonent_num),
                      new XElement("start_amount", c.start_amount),
                      new XElement("payments", c.payments),
                      new XElement("charges", c.charges),
                      new XElement("end_amount", c.end_amount)
                      )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BalanceByCardsSummary.xlsx");
            }
        }

        public async Task<FileResult> GetBalanceByCardsAccountingSummaryExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                DateTime d_dt = fromDate.AddDays(-1);
                DateTime chargeFromDate = new DateTime(d_dt.Year, d_dt.Month, d_dt.Day, 23, 59, 0);
                DateTime chargeToDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 58, 0);


                var data = await _db.Cards.OrderBy(c => c.AbonentNum).Select(c => new
                {
                    abonent = c.Customer.Name + " " + c.Customer.LastName,
                    code = c.Customer.Code,
                    type = c.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                    abonent_num = c.AbonentNum,
                    start_amount = c.Payments.Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.CardCharges.Where(p => p.Tdate <= chargeFromDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    payments = c.Payments.Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    charges = c.CardCharges.Where(cc => cc.Tdate >= chargeFromDate && cc.Tdate <= chargeToDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum(),
                    end_amount = c.Payments.Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.CardCharges.Where(p => p.Tdate <= chargeToDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                }).ToListAsync();

                XElement element = new XElement("root",
                  new XElement("columns",
                      new XElement("name", "აბონენტი"),
                      new XElement("name", "ტიპი"),
                      new XElement("name", "აბონენტის №"),
                      new XElement("name", "საწყისი ნაშთი"),
                      new XElement("name", "გადახდები"),
                      new XElement("name", "დარიცხვები"),
                      new XElement("name", "საბოლოო ნაშთი")),
                  data.Select(c => new XElement("data",
                      new XElement("abonent", c.abonent),
                      new XElement("type", c.type),
                      new XElement("abonent_num", c.abonent_num),
                      new XElement("start_amount", c.start_amount),
                      new XElement("payments", c.payments),
                      new XElement("charges", c.charges),
                      new XElement("end_amount", c.end_amount)
                      )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BalanceByCardsSummary.xlsx");
            }
        }

        public async Task<FileResult> GetBalanceByAbonentsSummaryExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var data = await _db.Customers.Select(c => new
                        {
                            abonent = c.Name + " " + c.LastName,
                            code = c.Code,
                            type = c.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.JuridicalType == 1 ? ":კომუნალური" : (c.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                            start_amount = c.Cards.SelectMany(p => p.Payments.Where(pp => pp.Tdate <= fromDate)).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.Cards.SelectMany(p => p.CardCharges.Where(pp => pp.Tdate <= fromDate)).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                            payments = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                            charges = c.Cards.SelectMany(p => p.CardCharges).Where(cc => cc.Tdate >= fromDate && cc.Tdate <= toDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum(),
                            end_amount = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.Cards.SelectMany(p => p.CardCharges).Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),

                        }).ToListAsync();

                XElement element = new XElement("root",
                  new XElement("columns",
                      new XElement("name", "აბონენტი"),
                      new XElement("name", "პ/ნ"),
                      new XElement("name", "ტიპი"),
                      new XElement("name", "საწყისი ნაშთი"),
                      new XElement("name", "გადახდები"),
                      new XElement("name", "დარიცხვები"),
                      new XElement("name", "საბოლოო ნაშთი")),
                  data.Select(c => new XElement("data",
                      new XElement("abonent", c.abonent),
                      new XElement("code", c.code),
                      new XElement("type", c.type),
                      new XElement("start_amount", c.start_amount),
                      new XElement("payments", c.payments),
                      new XElement("charges", c.charges),
                      new XElement("end_amount", c.end_amount)
                      )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BalanceByAbonentsSummary.xlsx");
            }
        }

        public async Task<FileResult> GetBalanceByAbonentsAccountingSummaryExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                DateTime d_dt = fromDate.AddDays(-1);
                DateTime chargeFromDate = new DateTime(d_dt.Year, d_dt.Month, d_dt.Day, 23, 59, 0);
                DateTime chargeToDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 58, 0);

                var data = await _db.Customers.OrderBy(c => c.Id).Select(c => new
                {
                    abonent = c.Name + " " + c.LastName,
                    code = c.Code,
                    type = c.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.JuridicalType == 1 ? ":კომუნალური" : (c.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                    start_amount = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate <= fromDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.Cards.SelectMany(p => p.CardCharges).Where(p => p.Tdate <= chargeFromDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    payments = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate >= fromDate && p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                    charges = c.Cards.SelectMany(p => p.CardCharges).Where(cc => cc.Tdate >= chargeFromDate && cc.Tdate <= chargeFromDate).Select(cc => cc.Amount).DefaultIfEmpty().Sum(),
                    end_amount = c.Cards.SelectMany(p => p.Payments).Where(p => p.Tdate <= toDate).Select(p => p.Amount).DefaultIfEmpty().Sum() - c.Cards.SelectMany(p => p.CardCharges).Where(p => p.Tdate <= chargeToDate).Select(p => p.Amount).DefaultIfEmpty().Sum(),
                }).ToListAsync();

                XElement element = new XElement("root",
                  new XElement("columns",
                      new XElement("name", "აბონენტი"),
                      new XElement("name", "პ/ნ"),
                      new XElement("name", "ტიპი"),
                      new XElement("name", "საწყისი ნაშთი"),
                      new XElement("name", "გადახდები"),
                      new XElement("name", "დარიცხვები"),
                      new XElement("name", "საბოლოო ნაშთი")),
                  data.Select(c => new XElement("data",
                      new XElement("abonent", c.abonent),
                      new XElement("code", c.code),
                      new XElement("type", c.type),
                      new XElement("start_amount", c.start_amount),
                      new XElement("payments", c.payments),
                      new XElement("charges", c.charges),
                      new XElement("end_amount", c.end_amount)
                      )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BalanceByAbonentsSummary.xlsx");
            }
        }

        public async Task<FileResult> GetPaymentsExport()
        {
            using (DataContext _db = new DataContext())
            {
                string[] date_from = Request["date_from"].Split('_');
                string[] date_to = Request["date_to"].Split('_');
                DateTime fromDate = DateTime.Parse(date_from[2] + "-" + date_from[1] + "-" + date_from[0] + " 00:00:00", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.Parse(date_to[2] + "-" + date_to[1] + "-" + date_to[0] + " 23:59:59", CultureInfo.InvariantCulture);

                var data = await _db.Payments.AsNoTracking()
                    .Where(c => c.Tdate >= fromDate && c.Tdate <= toDate)
                    .OrderByDescending(c => c.Tdate).Select(c => new
                    {
                        date = c.Tdate,
                        abonent = c.Card.Customer.Name + " " + c.Card.Customer.LastName,
                        type = c.Card.Customer.Type == Models.CustomerType.Juridical ? "იურიდიული" + (c.Card.Customer.JuridicalType == 1 ? ":კომუნალური" : (c.Card.Customer.JuridicalType == 2 ? ":კომერციული" : "")) : "ფიზიკური",
                        phone = c.Card.Customer.Phone1,
                        status = c.Card.CardStatus == CardStatus.Active ? "აქტიური" : (c.Card.CardStatus == CardStatus.Canceled ? "გაუქმებული" : (c.Card.CardStatus == CardStatus.Closed ? "დახურული" : (c.Card.CardStatus == CardStatus.FreeDays ? "უფასო დღეები" : "დაპაუზებული"))),
                        abonent_num = c.Card.AbonentNum,
                        card_num = c.Card.CardNum,
                        packets = c.Card.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(s => s.Package.Name).ToList(),
                        packet_date = c.Card.Subscribtions.FirstOrDefault(s => s.Status).Tdate,
                        pay_type = c.PayType.Name,
                        amount = c.Amount
                    }).ToListAsync();

                XElement element = new XElement("root",
                   new XElement("columns",
                       new XElement("name", "თარიღი"),
                       new XElement("name", "აბონენტი"),
                       new XElement("name", "ტიპი"),
                       new XElement("name", "ტელ"),
                       new XElement("name", "სტატუსი"),
                       new XElement("name", "აბონენტის №"),
                       new XElement("name", "ბარათის №"),
                       new XElement("name", "პაკეტი"),
                       new XElement("name", "პაკეტის თარიღი"),
                       new XElement("name", "გადახდის სახეობა"),
                       new XElement("name", "თანხა")),
                   data.Select(c => new XElement("data",
                       new XElement("date", c.date),
                       new XElement("abonent", c.abonent),
                       new XElement("type", c.type),
                       new XElement("phone", c.phone),
                       new XElement("status", c.status),
                       new XElement("abonent_num", c.abonent_num),
                       new XElement("card_num", c.card_num),
                       new XElement("packets", c.packets),
                       new XElement("packet_date", c.packet_date),
                       new XElement("pay_type", c.pay_type),
                       new XElement("amount", c.amount)
                       )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payments.xlsx");

            }
        }

	}
}