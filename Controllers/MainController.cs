using DigitalTVBilling.Filters;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.Mvc;
using PagedList.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Newtonsoft.Json;
using DigitalTVBilling.ListModels;
using System.Diagnostics;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class MainController : BaseController
    {
        public static readonly IDictionary<int, string> _Tables = new Dictionary<int, string>()
        {
            {-1, ""},
            {0, "book.Customers"},
            {1, "book.Cards"},
            {2, "doc.Payments"},
            {3, "book.Packages"},
            {4, "book.Channels"},
            {5, "book.Messages"},
            {6, "book.Users"},
            {7, "book.UserTypes"},
            {8, "book.Receivers"},
            {9, "book.Towers"},
            {10, "doc.Subscribes"},
            {12, "config.Params"},
            {13, "book.Services"},
            {14, "doc.CardServices"},
            {15, "doc.Orders"},
            {16, "book.Reasons"},
            {17, "book.OfficeCards"}
        };

        //
        // GET: /Main/
        public ActionResult Index()
        {
            if (!Utils.Utils.GetPermission("SHOW_STAT"))
            {
                return new RedirectResult("/Blank");
            }
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            ViewBag.dtFrom = dateFrom;
            ViewBag.dtTo = dateTo;
            DateTime dateFromDay = DateTime.Now;
            dateFromDay = dateFrom.AddHours((double)1);
            using (DataContext _db = new DataContext())
            {

                //var Returned = _db.Cards.Include("ReturnedCards").ToList();
                //var countActiveReturned = Returned.Where(c => c.ReturnedCards.Any(cc => cc.Tdate >= dateFrom) && c.ReturnedCards.Any(cc => cc.Tdate <= dateTo) && c.ReturnedCards.Any(cc => cc.Tdate < c.FinishDate)).Count();
                //var count1mont = Returned.Where(c => c.ReturnedCards.Any(cc => cc.Tdate >= dateFrom) && c.ReturnedCards.Any(cc => cc.Tdate <= dateTo) && c.ReturnedCards.Any(cc => cc.Tdate > c.FinishDate) && c.ReturnedCards.Any(cc => cc.Tdate < c.FinishDate.AddMonths(1))).Count();
                //var count2mont = Returned.Where(c => c.ReturnedCards.Any(cc => cc.Tdate >= dateFrom) && c.ReturnedCards.Any(cc => cc.Tdate <= dateTo) && c.ReturnedCards.Any(cc => cc.Tdate > c.FinishDate.AddMonths(1)) && c.ReturnedCards.Any(cc => cc.Tdate < c.FinishDate.AddMonths(2))).Count();
                //var count3mont = Returned.Where(c => c.ReturnedCards.Any(cc => cc.Tdate >= dateFrom) && c.ReturnedCards.Any(cc => cc.Tdate <= dateTo) && c.ReturnedCards.Any(cc => cc.Tdate > c.FinishDate.AddMonths(2)) && c.ReturnedCards.Any(cc => cc.Tdate < c.FinishDate.AddMonths(24))).Count();

                //var cardlog = _db.Database.SqlQuery<CardLog>(@"select cc.id AS Id, cc.status as Status,cc.close_tdate as Date,cc.user_id AS UserId,cc.card_status as CardStatus,cc.card_id as CardId from dbo.ReturnedCards AS r
                //inner join doc.CardLogs AS cc on cc.card_id=r.card_id where (cc.status=5 or cc.status=1 or cc.status=0 or cc.status=6) and r.tdate between '"+dateFrom+"' and '"+dateTo+"' order by cc.card_id, cc.close_tdate").ToList();
                //int countActive = 0, count1monts = 0, count2monts = 0, count3monts = 0;
                //for (var i = 0; i < cardlog.Count(); i++)
                //{
                //    if (cardlog[i].Status == CardLogStatus.Cancel)
                //    {
                //        var x = cardlog[i - 1].Status;
                //        if (cardlog[i - 1].Status == CardLogStatus.Open)
                //        {
                //            countActive++;
                //        }
                //        if (cardlog[i - 1].Status == CardLogStatus.Close)
                //        {
                //            var date = (cardlog[i].Date - cardlog[i - 1].Date).Days;
                //            if (date <= 30)
                //                count1monts++;
                //            if (date <= 60 && date >30)
                //                count2monts++;
                //            if (date > 60)
                //                count3monts++;
                //        }
                //        if (cardlog[i - 1].Status == CardLogStatus.Blocked)
                //        {
                //            count3monts++;
                //        }
                //    }
                //}

                var static_8_back_active = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 0 where s.tdate < '{dateFrom}' and sp.package_id = 304085").ToList();
                var static_15_back_active = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 0 where s.tdate < '{dateFrom}' and sp.package_id = 304084").ToList();
                var static_8_false = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 0 where s.tdate between '{dateFrom}' and '{dateTo}' and sp.package_id = 304085").ToList();
                var static_15_false = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 0 where s.tdate between '{dateFrom}' and '{dateTo}' and sp.package_id = 304084").ToList();
                var static_8_active = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 1 where s.tdate between '{dateFrom}' and '{dateTo}' and sp.package_id = 304085").ToList();
                var static_15_active = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 1 where s.tdate between '{dateFrom}' and '{dateTo}' and sp.package_id = 304071").ToList();
                //static_15_back_active = static_15_back_active.Distinct().ToList();
                int count8 = 0, count15 = 0;
                foreach (var item in static_15_active)
                {
                    if (static_8_back_active.Contains(item) || static_8_false.Contains(item))
                    {
                        count15++;
                    }
                }
                foreach (var item in static_8_active)
                {
                    if (static_15_back_active.Contains(item) || static_15_false.Contains(item))
                    {
                        count8++;
                    }

                }

                ViewBag.CardsDataActive = "[[\"15-დან 8-ზე\"," + count8 + "],[\"8-დან 15-ზე\"," + count15 + "]]";

                var static_promo = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id  where  sp.package_id = 304086").ToList();
                var static_8_promo = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 1 where  sp.package_id = 304085").ToList();
                var static_15_promo = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 1 where sp.package_id = 304071").ToList();
                //static_15_back_active = static_15_back_active.Distinct().ToList();
                int count8promo = 0, count15promo = 0;
                foreach (var item in static_promo)
                {
                    if (static_15_promo.Contains(item))
                    {
                        count15promo++;
                    }
                }
                foreach (var item in static_promo)
                {
                    if (static_8_promo.Contains(item))
                    {
                        count8promo++;
                    }

                }

                ViewBag.Promo_Cahnge = "[[\"პრომო\"," + (static_promo.Count() - (count8promo + count15promo)) + "],[\"პრომო-დან 8-ზე\"," + count8promo + "],[\"პრომო-დან 15-ზე\"," + count15promo + "]]";
                //finish_date<= DATEADD(mm, -6, GETDATE())   // 6tvis ukan

                //var Returned = _db.Cards.Include("ReturnedCards").ToList();
                //var countActiveReturned = Returned.Where(c => c.ReturnedCards.Any(cc => cc.Tdate >= dateFrom) && c.ReturnedCards.Any(cc => cc.Tdate <= dateTo) && c.ReturnedCards.Any(cc => cc.Tdate < c.FinishDate)).Count();
                //var count1mont = Returned.Where(c => c.ReturnedCards.Any(cc => cc.Tdate >= dateFrom) && c.ReturnedCards.Any(cc => cc.Tdate <= dateTo) && c.ReturnedCards.Any(cc => cc.Tdate > c.FinishDate) && c.ReturnedCards.Any(cc => cc.Tdate < c.FinishDate.AddMonths(1))).Count();
                //var count2mont = Returned.Where(c => c.ReturnedCards.Any(cc => cc.Tdate >= dateFrom) && c.ReturnedCards.Any(cc => cc.Tdate <= dateTo) && c.ReturnedCards.Any(cc => cc.Tdate > c.FinishDate.AddMonths(1)) && c.ReturnedCards.Any(cc => cc.Tdate < c.FinishDate.AddMonths(2))).Count();
                //var count3mont = Returned.Where(c => c.ReturnedCards.Any(cc => cc.Tdate >= dateFrom) && c.ReturnedCards.Any(cc => cc.Tdate <= dateTo) && c.ReturnedCards.Any(cc => cc.Tdate > c.FinishDate.AddMonths(2)) && c.ReturnedCards.Any(cc => cc.Tdate < c.FinishDate.AddMonths(24))).Count();

                var _countActive = _db.Database.SqlQuery<int>(@"Select count(r.ID) from book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id where r.tdate >= '"+dateFrom+"' and r.tdate <= '"+dateTo+"' and r.tdate < c.finish_date").ToList();
                var _count1mont = _db.Database.SqlQuery<int>(@"Select count(r.ID) from book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id=c.id where r.tdate>='" + dateFrom + "' and r.tdate <= '" + dateTo + "' and r.tdate > c.finish_date and r.tdate < DATEADD(mm, 1, c.finish_date)").ToList();
                var _count2mont = _db.Database.SqlQuery<int>(@"Select count(r.ID) from book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id=c.id where r.tdate>='" + dateFrom + "' and r.tdate <= '" + dateTo + "' and r.tdate>DATEADD(mm, 1, c.finish_date) and r.tdate<DATEADD(mm, 2, c.finish_date)").ToList();

                var _count3mont = _db.Database.SqlQuery<int>(@"Select count(r.ID) from book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id=c.id where r.tdate>='" + dateFrom + "' and r.tdate <= '" + dateTo + "' and r.tdate>DATEADD(mm, 2, c.finish_date) and r.tdate<DATEADD(mm, 24, c.finish_date)").ToList();
                var promoReturnedCount = _db.Database.SqlQuery<int>(@"SELECt count(r.id) from book.Cards as c
                                                                                        inner join doc.Subscribes as s on s.card_id=c.id
                                                                                        inner join doc.SubscriptionPackages as sp on sp.subscription_id=s.id 
                                                                                        inner join dbo.ReturnedCards as r on r.card_id=c.id where r.tdate>='"+dateFrom+"' and r.tdate<='"+dateTo+"' and c.status=4 and sp.package_id=304086").ToList();

                ViewBag.CardsReturned = "[[\"აქტიურის-გაუქმება\"," + _countActive[0] + "],[\"1-თვე გათიშულის-გაუქმება\"," + _count1mont[0] + "],[\"2-თვე გათიშულის-გაუქმება\"," + _count2mont[0] + "],[\"2-თვეზე მეტი გათიშულის-გაუქმება\"," + _count3mont[0] + "],[\"პრომოს-გაუქმება\"," + promoReturnedCount[0] + "]]";

                ViewBag.CardsData = JsonConvert.SerializeObject(_db.Database.SqlQuery<IdName>("SELECT dbo.GetCardStatus([status]) AS Name, COUNT(*) AS Id FROM book.Cards WHERE [status] !=4 GROUP BY [status]").ToList().Select(c => new object[] { c.Name, c.Id }));
                //var block_count = (_db.Database.SqlQuery<int>("SELECT COUNT(*) AS Id FROM book.Cards WHERE status=5 and finish_date<= DATEADD(mm, -6, '"+DateTime.Now+"') GROUP BY [status]").FirstOrDefault());
                //var index = CardsData.IndexOf("დაბლოკილი");
                //var change_blockCount = CardsData.Substring(index + 11, 4);
                //CardsData = CardsData.Replace(change_blockCount, (Convert.ToInt32(change_blockCount) - block_count).ToString());
                //ViewBag.CardsData = CardsData.Remove(CardsData.Length - 1) + ",[\"შეწყვეტილი\"," + block_count + "]]";
                ViewBag.CanceledCardsCount = _db.Cards.Where(c => c.CardStatus == CardStatus.Canceled).Count();
                ViewBag.AbonentsByCityData = JsonConvert.SerializeObject(_db.Database.SqlQuery<IdName>("SELECT city AS Name, COUNT(*) AS Id FROM book.Customers GROUP BY city").ToList().Select(c => new object[] { c.Name, c.Id }));

                ViewBag.Poll = JsonConvert.SerializeObject(_db.Database.SqlQuery<IdName>("SELECT dbo.Poll([poll]) AS Name, COUNT(*) AS Id FROM doc.Orders where [poll]!=0 and tdate between '" + dateFrom.ToString("MM-dd-yyyy 00:01:ss") + "' and '" + dateTo.ToString("MM-dd-yyyy 23:59:ss") + "' GROUP BY [poll]").ToList().Select(c => new object[] { c.Name, c.Id }));
                string months = "[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12]";

                ViewBag.Promo = JsonConvert.SerializeObject(_db.Database.SqlQuery<IdName>(@"SELECT dbo.GetCardStatus(c.status) AS Name, COUNT(*) AS Id FROM book.Cards AS c
                                                                                                    inner join doc.Subscribes AS s on s.card_id=c.id
                                                                                                    inner join doc.SubscriptionPackages AS sp ON sp.subscription_id=s.id
                                                                                                    WHERE sp.package_id=304086  GROUP BY c.status").ToList().Select(c => new object[] { c.Name, c.Id }));
                ViewBag.ReturnedCards= JsonConvert.SerializeObject(new
                {
                    countactive = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate < c.finish_date and DATEPART(year, r.tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    count1mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE  r.tdate > c.finish_date and r.tdate < DATEADD(mm, 1, c.finish_date) and DATEPART(year, r.tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    count2mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate>DATEADD(mm, 1, c.finish_date) and r.tdate < DATEADD(mm, 2, c.finish_date) and DATEPART(year, r.tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    count3mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                            SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate>DATEADD(mm, 2, c.finish_date) and r.tdate < DATEADD(mm, 24, c.finish_date) and DATEPART(year, r.tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    countpromo = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                            SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards as c
                                                                                    inner join doc.Subscribes as s on s.card_id=c.id
                                                                                    inner join doc.SubscriptionPackages as sp on sp.subscription_id=s.id 
                                                                                    inner join dbo.ReturnedCards as r on r.card_id=c.id where sp.package_id=304086 ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList()
                });

                ViewBag.MonthlyDeals = JsonConvert.SerializeObject(new
                {
                    montages = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT id,[month] = DATEPART(month, tdate) FROM book.Cards WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    orders = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT id,[month] = DATEPART(month, tdate) FROM doc.Orders WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    damages = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT id,[month] = DATEPART(month, tdate) FROM doc.CardDamages WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                });

                ViewBag.ChargesAndPayments = JsonConvert.SerializeObject(new
                {
                    payments = _db.Database.SqlQuery<decimal>(@"SELECT aData FROM( SELECT 
                                                              ISNULL([1],0) AS [1],ISNULL([2],0) AS [2],ISNULL([3],0) AS [3],ISNULL([4],0) AS [4],ISNULL([5],0) AS [5],ISNULL([6],0) AS [6],
                                                              ISNULL([7],0) AS [7],ISNULL([8],0) AS [8],ISNULL([9],0) AS [9],ISNULL([10],0) AS [10],ISNULL([11],0) AS [11],ISNULL([12],0) AS [12]
                                                             FROM (
                                                                   SELECT amount,[month] = DATEPART(month, tdate) FROM doc.Payments WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (sum(amount) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    charges = _db.Database.SqlQuery<decimal>(@"SELECT aData FROM( SELECT 
                                                              ISNULL([1],0) AS [1],ISNULL([2],0) AS [2],ISNULL([3],0) AS [3],ISNULL([4],0) AS [4],ISNULL([5],0) AS [5],ISNULL([6],0) AS [6],
                                                              ISNULL([7],0) AS [7],ISNULL([8],0) AS [8],ISNULL([9],0) AS [9],ISNULL([10],0) AS [10],ISNULL([11],0) AS [11],ISNULL([12],0) AS [12]
                                                             FROM (
                                                                   SELECT amount,[month] = DATEPART(month, tdate) FROM doc.CardCharges WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (sum(amount) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList()
                });
                ViewBag.Cancled = JsonConvert.SerializeObject(new
                {
                    Cancled = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT 
                    ISNULL([1], 0) AS[1],ISNULL([2],0) AS[2],ISNULL([3],0) AS[3],ISNULL([4],0) AS[4],ISNULL([5],0) AS[5],ISNULL([6],0) AS[6],
                    ISNULL([7],0) AS[7],ISNULL([8],0) AS[8],ISNULL([9],0) AS[9],ISNULL([10],0) AS[10],ISNULL([11],0) AS[11],ISNULL([12],0) AS[12]
                    FROM(
                    SELECT id,[month] = DATEPART(month, tdate) FROM dbo.ReturnedCards WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList()
                });
                ViewBag.Damage = JsonConvert.SerializeObject(new
                {
                    damage = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT 
                    ISNULL([1], 0) AS[1],ISNULL([2],0) AS[2],ISNULL([3],0) AS[3],ISNULL([4],0) AS[4],ISNULL([5],0) AS[5],ISNULL([6],0) AS[6],
                    ISNULL([7],0) AS[7],ISNULL([8],0) AS[8],ISNULL([9],0) AS[9],ISNULL([10],0) AS[10],ISNULL([11],0) AS[11],ISNULL([12],0) AS[12]
                    FROM(
                    SELECT id,[month] = DATEPART(month, tdate) FROM dbo.Damage WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    damage_proces = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT 
                    ISNULL([1], 0) AS[1],ISNULL([2],0) AS[2],ISNULL([3],0) AS[3],ISNULL([4],0) AS[4],ISNULL([5],0) AS[5],ISNULL([6],0) AS[6],
                    ISNULL([7],0) AS[7],ISNULL([8],0) AS[8],ISNULL([9],0) AS[9],ISNULL([10],0) AS[10],ISNULL([11],0) AS[11],ISNULL([12],0) AS[12]
                    FROM(
                    SELECT id,[month] = DATEPART(month, tdate) FROM dbo.Damage WHERE (status=-2 or  executor_id!=0) and DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    damage_coll = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT 
                    ISNULL([1], 0) AS[1],ISNULL([2],0) AS[2],ISNULL([3],0) AS[3],ISNULL([4],0) AS[4],ISNULL([5],0) AS[5],ISNULL([6],0) AS[6],
                    ISNULL([7],0) AS[7],ISNULL([8],0) AS[8],ISNULL([9],0) AS[9],ISNULL([10],0) AS[10],ISNULL([11],0) AS[11],ISNULL([12],0) AS[12]
                    FROM(
                    SELECT id,[month] = DATEPART(month, tdate) FROM dbo.Damage WHERE (status=7 and  executor_id=0) and DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList()

                });
                string sql = @";WITH Packets (s_id, card_id, package_name)
                AS
                (
                    SELECT s.id, s.card_id, STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id ORDER BY p.name FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.status=1
                )

                SELECT r.package, COUNT(r.count) AS count FROM
                (SELECT 
                q.package_name AS package,
                (SELECT COUNT(*) FROM book.Cards AS c WHERE c.status != 4 AND EXISTS(SELECT * FROM Packets WHERE q.card_id=c.id AND EXISTS(SELECT * FROM doc.Subscribes WHERE id = q.s_id))) AS count

                FROM Packets AS q) AS r GROUP BY r.package";

                ViewBag.PacketsByCards = JsonConvert.SerializeObject(_db.Database.SqlQuery<CardReport>(sql).ToList().Select(c => new object[] { c.package, c.count }).ToArray());

                sql = @";WITH Packets (s_id, card_id, package_name)
                AS
                (
                    SELECT s.id, s.card_id, STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id ORDER BY p.name FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.status=1
                )

                SELECT r.card_status AS status, r.package, r.count FROM
                (SELECT
                dbo.GetCardStatus(cr.status) AS card_status,
                q.package_name AS package,
                (SELECT COUNT(*) FROM book.Cards AS c WHERE c.status != 4 AND EXISTS(SELECT * FROM Packets WHERE q.card_id=c.id AND EXISTS(SELECT * FROM doc.Subscribes WHERE id = q.s_id))) AS count

                FROM Packets AS q 
                INNER JOIN book.Cards AS cr ON cr.id=q.card_id
                INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status !=4 AND c.city=N'თბილისი') AS r";

                ViewBag.PacketsByCities = JsonConvert.SerializeObject(_db.Database.SqlQuery<CardReport>(sql).ToList().GroupBy(c => c.status).Select(c => new
                {
                    status = c.Key,
                    count = c.GroupBy(cc => cc.package).Select(cc => new
                    {
                        packet = cc.Key,
                        count = cc.Count()
                    })
                }).ToList());

                ViewBag.Cities = JsonConvert.SerializeObject(_db.Customers.GroupBy(r => r.City).Select(r => r.Key.Trim()).ToList());
            }

            return View();
        }
        public JsonResult ReturnedCard(string dt_from, string dt_to)
        {
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            { 
                var _countActive = _db.Database.SqlQuery<int>(@"Select count(r.ID) from book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id where r.tdate >= '" + dateFrom + "' and r.tdate <= '" + dateTo + "' and r.tdate < c.finish_date").ToList();
                var _count1mont = _db.Database.SqlQuery<int>(@"Select count(r.ID) from book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id=c.id where r.tdate>='" + dateFrom + "' and r.tdate <= '" + dateTo + "' and r.tdate > c.finish_date and r.tdate < DATEADD(mm, 1, c.finish_date)").ToList();
                var _count2mont = _db.Database.SqlQuery<int>(@"Select count(r.ID) from book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id=c.id where r.tdate>='" + dateFrom + "' and r.tdate <= '" + dateTo + "' and r.tdate>DATEADD(mm, 1, c.finish_date) and r.tdate<DATEADD(mm, 2, c.finish_date)").ToList();

                var _count3mont = _db.Database.SqlQuery<int>(@"Select count(r.ID) from book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id=c.id where r.tdate>='" + dateFrom + "' and r.tdate <= '" + dateTo + "' and r.tdate>DATEADD(mm, 2, c.finish_date) and r.tdate<DATEADD(mm, 24, c.finish_date)").ToList();
                var promoReturnedCount = _db.Database.SqlQuery<int>(@"SELECt count(r.id) from book.Cards as c
                                                                                        inner join doc.Subscribes as s on s.card_id=c.id
                                                                                        inner join doc.SubscriptionPackages as sp on sp.subscription_id=s.id 
                                                                                        inner join dbo.ReturnedCards as r on r.card_id=c.id where r.tdate>='" + dateFrom + "' and r.tdate<='" + dateTo + "' and sp.package_id=304086").ToList();
                var სსას = @"SELECt count(r.id) from book.Cards as c
                                                                                        inner join doc.Subscribes as s on s.card_id = c.id
                                                                                        inner join doc.SubscriptionPackages as sp on sp.subscription_id = s.id
                                                                                        inner join dbo.ReturnedCards as r on r.card_id = c.id where r.tdate >= '" + dateFrom + "' and r.tdate <= '" + dateTo + "' and sp.package_id = 304086";
                ViewBag.CardsReturned = "[[\"აქტიურის-გაუქმება\"," + _countActive[0] + "],[\"1-თვე გათიშულის-გაუქმება\"," + _count1mont[0] + "],[\"2-თვე გათიშულის-გაუქმება\"," + _count2mont[0] + "],[\"2-თვეზე მეტი გათიშულის-გაუქმება\"," + _count3mont[0] + "],[\"პრომოს-გაუქმება\"," + promoReturnedCount[0] + "]]";

            }
            return Json(ViewBag.CardsReturned);
        }
        //[HttpPost]
        public JsonResult CardsActive(string dt_from, string dt_to)
        {
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            {

                //var StaticCardCahrges = _db.Subscribtions.Include("SubscriptionPackages").ToList();
                //var static_8_back_active = StaticCardCahrges.Where(c => c.Status == false && c.Tdate < dateFrom  && c.SubscriptionPackages.Any(a=>a.PackageId== 304085)).Select(s => s.CardId).ToList();
                //var static_15_back_active = StaticCardCahrges.Where(c => c.Status == false && c.Tdate < dateFrom  && c.SubscriptionPackages.Any(a => a.PackageId == 304084)).Select(s => s.CardId).ToList();
                //var static_8_false = StaticCardCahrges.Where(c => c.Status == false && c.Tdate >= dateFrom && c.Tdate <= dateTo  && c.SubscriptionPackages.Any(a => a.PackageId == 304085)).Select(s => s.CardId).ToList();
                //var static_15_false = StaticCardCahrges.Where(c => c.Status == false && c.Tdate >= dateFrom && c.Tdate <= dateTo  && c.SubscriptionPackages.Any(a => a.PackageId == 304084)).Select(s => s.CardId).ToList();
                //var static_8_active = StaticCardCahrges.Where(c => c.Status == true && c.Tdate >= dateFrom && c.Tdate <= dateTo  && c.SubscriptionPackages.Any(a => a.PackageId == 304085)).ToList();
                //var static_15_active = StaticCardCahrges.Where(c => c.Status == true && c.Tdate >= dateFrom && c.Tdate <= dateTo  && c.SubscriptionPackages.Any(a => a.PackageId == 304071)).ToList();
                var static_8_back_active = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 0 where s.tdate < '{dateFrom}' and sp.package_id = 304085").ToList();
                var static_15_back_active = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 0 where s.tdate < '{dateFrom}' and sp.package_id = 304084").ToList();
                var static_8_false = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 0 where s.tdate between '{dateFrom}' and '{dateTo}' and sp.package_id = 304085").ToList();
                var static_15_false = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 0 where s.tdate between '{dateFrom}' and '{dateTo}' and sp.package_id = 304084").ToList();
                var static_8_active = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 1 where s.tdate between '{dateFrom}' and '{dateTo}' and sp.package_id = 304085").ToList();
                var static_15_active = _db.Database.SqlQuery<int>($"SELECT s.card_id FROM doc.Subscribes AS s inner join doc.SubscriptionPackages AS sp ON s.id = sp.subscription_id and status = 1 where s.tdate between '{dateFrom}' and '{dateTo}' and sp.package_id = 304071").ToList();
                int count8 = 0, count15 = 0;
                foreach (var item in static_15_active)
                {
                    if (static_8_back_active.Contains(item) || static_8_false.Contains(item))
                    {
                        count15++;
                    }
                }
                foreach (var item in static_8_active)
                {
                    if (static_15_back_active.Contains(item) || static_15_false.Contains(item))
                    {
                        count8++;
                    }

                }

                ViewBag.CardsDataActive = "[[\"15-დან 8-ზე\"," + count8 + "],[\"8-დან 15-ზე\"," + count15 + "]]";
            }
            return Json(ViewBag.CardsDataActive);
        }
        public JsonResult StaticPoll(string dt_from, string dt_to)
        {
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            var _dateFrom = dateFrom.ToString("yyyy-MM-dd 12:00:ss");
            var _dateTo = dateTo.ToString("yyyy-MM-dd 23:59:ss");
            using (DataContext _db = new DataContext())
            {
                ViewBag.Poll = JsonConvert.SerializeObject(_db.Database.SqlQuery<IdName>("SELECT dbo.Poll([poll]) AS Name, COUNT(*) AS Id FROM doc.Orders where [poll]!=0 and tdate between '" + dateFrom.ToString("MM-dd-yyyy 00:01:ss") + "' and '" + dateTo.ToString("MM-dd-yyyy 23:59:ss") + "' GROUP BY [poll]").ToList().Select(c => new object[] { c.Name, c.Id }));
            }
            return Json(ViewBag.Poll);
        }
        public class CardsActiveData
        {
            public Object Active { get; set; }
            public int Count { get; set; }

        }
        public async Task<FileResult> CardsCountExport(string year)
        {
            using (DataContext _db = new DataContext())
            {
                var data = await _db.Cards.Select(c => c).Where(c => c.CardStatus == CardStatus.Active || c.CardStatus == CardStatus.Closed || c.CardStatus == CardStatus.Paused || c.CardStatus == CardStatus.Blocked).GroupBy(c => c.CardStatus).Select(o => new
                {
                    count = _db.Cards.Select(q => q).Where(c => c.CardStatus == CardStatus.Active || c.CardStatus == CardStatus.Closed || c.CardStatus == CardStatus.Paused || c.CardStatus == CardStatus.Blocked).ToList().Count(),
                    cardStatus = o.FirstOrDefault().CardStatus,
                    active = o.Where(m => m.CardStatus == CardStatus.Active).ToList().Count(),
                    closed = o.Where(m => m.CardStatus == CardStatus.Closed).ToList().Count(),
                    paused = o.Where(m => m.CardStatus == CardStatus.Paused).ToList().Count(),
                    blocked = o.Where(m => m.CardStatus == CardStatus.Blocked).ToList().Count(),
                }).OrderBy(s => s.cardStatus).ToListAsync();
                XElement element = new XElement("root",
                        new XElement("columns",
                            new XElement("name", "სტატუსი"),
                            new XElement("name", "რაოდენობა"),
                            new XElement("name", "პროცენტი")),
                        data.Where(cc => cc.cardStatus == CardStatus.Active).Select(c => new XElement("data",
                            new XElement("Status", c.cardStatus),
                            new XElement("Active", c.active),
                            new XElement("Blocked", Math.Round(100m * (Convert.ToDecimal(c.active) / Convert.ToDecimal(c.count)), 2))
                            )),
                        data.Where(cc => cc.cardStatus == CardStatus.Closed).Select(c => new XElement("data",
                            new XElement("Status", c.cardStatus),
                            new XElement("Active", c.closed),
                            new XElement("Blocked", Math.Round(100m * (Convert.ToDecimal(c.closed) / Convert.ToDecimal(c.count)), 2))
                            )),
                        data.Where(cc => cc.cardStatus == CardStatus.Paused).Select(c => new XElement("data",
                            new XElement("Status", c.cardStatus),
                            new XElement("Active", c.paused),
                            new XElement("Blocked", Math.Round(100m * (Convert.ToDecimal(c.paused) / Convert.ToDecimal(c.count)), 2))
                            )),
                        data.Where(cc => cc.cardStatus == CardStatus.Blocked).Select(c => new XElement("data",
                            new XElement("Status", c.cardStatus),
                            new XElement("Active", c.blocked),
                            new XElement("Blocked", Math.Round(100m * (Convert.ToDecimal(c.blocked) / Convert.ToDecimal(c.count)), 2))
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PacketsResult.xlsx");
            }
        }

        public async Task<FileResult> CountByCitiesExport(string year)
        {
            using (DataContext _db = new DataContext())
            {
                var data = await _db.Customers.Select(c => c).Where(c => c.City != "").GroupBy(c => c.City).Select(o => new
                {
                    city = o.Select(f => f.City).Distinct().ToList()
                }).ToListAsync();
                XElement element = new XElement("root",
                        new XElement("columns",
                            new XElement("name", "ქალაქი"),
                            new XElement("name", "რაოდენობა")),
                        data.Select(c => new XElement("data",
                            new XElement("Status", c.city.FirstOrDefault().ToString()),
                            new XElement("Active", _db.Customers.Where(cc => cc.City == c.city.FirstOrDefault().ToString()).Select(cc => cc).ToList().Count())
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PacketsResult.xlsx");
            }
        }

        public async Task<FileResult> ChartDataExport(string year)
        {
            using (DataContext _db = new DataContext())
            {
                var data = await _db.Orders.Where(u => u.GetDate.Year.ToString() == year).GroupBy(u => u.GetDate.Year).Select(o => new
                {
                    january = o.Where(m => m.GetDate.Month == 1).ToList().Count(),
                    february = o.Where(m => m.GetDate.Month == 2).ToList().Count(),
                    march = o.Where(m => m.GetDate.Month == 3).ToList().Count(),
                    april = o.Where(m => m.GetDate.Month == 4).ToList().Count(),
                    may = o.Where(m => m.GetDate.Month == 5).ToList().Count(),
                    june = o.Where(m => m.GetDate.Month == 6).ToList().Count(),
                    july = o.Where(m => m.GetDate.Month == 7).ToList().Count(),
                    august = o.Where(m => m.GetDate.Month == 8).ToList().Count(),
                    september = o.Where(m => m.GetDate.Month == 9).ToList().Count(),
                    october = o.Where(m => m.GetDate.Month == 10).ToList().Count(),
                    november = o.Where(m => m.GetDate.Month == 11).ToList().Count(),
                    december = o.Where(m => m.GetDate.Month == 12).ToList().Count(),
                }).ToListAsync();
                XElement element = new XElement("root",
                        new XElement("columns",
                            new XElement("name", "წელი"),
                            new XElement("name", "იანვარი"),
                            new XElement("name", "თებერვალი"),
                            new XElement("name", "მარტი"),
                            new XElement("name", "აპრილი"),
                            new XElement("name", "მაისი"),
                            new XElement("name", "ივნისი"),
                            new XElement("name", "ივლისი"),
                            new XElement("name", "აგვისტო"),
                            new XElement("name", "სექტემბერი"),
                            new XElement("name", "ოქტომბერი"),
                            new XElement("name", "ნოემბერი"),
                            new XElement("name", "დეკემბერი")),
                        data.Select(c => new XElement("data",
                            new XElement("Year", year),
                            new XElement("January", c.january),
                            new XElement("February", c.february),
                            new XElement("March", c.march),
                            new XElement("April", c.april),
                            new XElement("May", c.may),
                            new XElement("June", c.june),
                            new XElement("July", c.july),
                            new XElement("August", c.august),
                            new XElement("September", c.september),
                            new XElement("October", c.october),
                            new XElement("November", c.november),
                            new XElement("December", c.december)
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PacketsResult.xlsx");
            }
        }

        public async Task<FileResult> PackagesByCardsExport(string year)
        {
            using (DataContext _db = new DataContext())
            {
                string sql = @";WITH Packets (s_id, card_id, package_name)
                AS
                (
                    SELECT s.id, s.card_id, STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id ORDER BY p.name FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.status=1
                )

                SELECT r.package, COUNT(r.count) AS count FROM
                (SELECT 
                q.package_name AS package,
                (SELECT COUNT(*) FROM book.Cards AS c WHERE c.status != 4 AND EXISTS(SELECT * FROM Packets WHERE q.card_id=c.id AND EXISTS(SELECT * FROM doc.Subscribes WHERE id = q.s_id))) AS count

                FROM Packets AS q) AS r GROUP BY r.package";

                var PacketsByCards = _db.Database.SqlQuery<CardReport>(sql).ToList().Select(c => new object[] { c.package, c.count }).ToList();

                var data = PacketsByCards;
                var total = 0;
                foreach (var item in data)
                {
                    total += Convert.ToInt32(item[1]);
                }
                XElement element = new XElement("root",
                        new XElement("columns",
                            new XElement("name", "პაკეტი"),
                            new XElement("name", "რაოდენობა"),
                            new XElement("name", "პროცენტი")),
                        data.Select(cc => new XElement("data",
                            new XElement("Package", cc[0]),
                            new XElement("Count", cc[1]),
                            new XElement("Percent", Math.Round(100m * (Convert.ToDecimal(cc[1]) / Convert.ToDecimal(total)), 2))
                            )));

                return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PacketsResult.xlsx");
            }
        }

        //public async Task<FileResult> PackagesByCitiesExport(string city)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        //string sql = @";WITH Packets (s_id, card_id, package_name)
        //        //AS
        //        //(
        //        //    SELECT s.id, s.card_id, STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id ORDER BY p.name FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.status=1
        //        //)

        //        //SELECT r.card_status AS status, r.package, r.count FROM
        //        //(SELECT
        //        //dbo.GetCardStatus(cr.status) AS card_status,
        //        //q.package_name AS package,
        //        //(SELECT COUNT(*) FROM book.Cards AS c WHERE c.status != 4 AND EXISTS(SELECT * FROM Packets WHERE q.card_id=c.id AND EXISTS(SELECT * FROM doc.Subscribes WHERE id = q.s_id))) AS count

        //        //FROM Packets AS q 
        //        //INNER JOIN book.Cards AS cr ON cr.id=q.card_id
        //        //INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status !=4 AND c.city=N'თბილისი') AS r";

        //        //var PackagesByCities = _db.Database.SqlQuery<CardReport>(sql).ToList().GroupBy(c => c.status).Select(c => new
        //        //{
        //        //    status = c.Key,
        //        //    count = c.GroupBy(cc => cc.package).Select(cc => new
        //        //    {
        //        //        packet = cc.Key,
        //        //        count = cc.Count()
        //        //    })
        //        //}).ToList();

        //        //string sql1 = @";WITH Packets (s_id, card_id, package_name)
        //        //AS
        //        //(
        //        //    SELECT s.id, s.card_id, STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id ORDER BY p.name FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.status=1
        //        //)

        //        //SELECT r.package, COUNT(r.count) AS count FROM
        //        //(SELECT 
        //        //q.package_name AS package,
        //        //(SELECT COUNT(*) FROM book.Cards AS c WHERE c.status != 4 AND c.City ='" + city + "' AND EXISTS(SELECT * FROM Packets WHERE q.card_id=c.id AND EXISTS(SELECT * FROM doc.Subscribes WHERE id = q.s_id))) AS count FROM Packets AS q) AS r GROUP BY r.package";

        //        //var PacketsByCities = _db.Database.SqlQuery<CardReport>(sql1).ToList().Select(c => new object[] { c.package, c.count, c.status }).ToArray();
        //        var data = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.CardStatus != CardStatus.Canceled).ToList();


        //        //var data = PackagesByCities;
        //        XElement element = new XElement("root",
        //                new XElement("columns",
        //                    new XElement("name", "პაკეტი"),
        //                    new XElement("name", "აქტიური"),
        //                    new XElement("name", "გათიშული"),
        //                    new XElement("name", "დაბლოკილი"),
        //                    new XElement("name", "დაპაუზებული")),
        //                data.Select(c => new XElement("data",
        //                    new XElement("Status", string.Join(",", c.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Select(s => s.Package.Name).ToArray()) ),
        //                    new XElement("Count", " ")
        //                    )));

        //        return File(new Export().getExcelData("ExcelExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PacketsResult.xlsx");
        //    }
        //}

        public JsonResult UpdateChargesPayments(int type, int month)
        {
            using (DataContext _db = new DataContext())
            {
                if (type == 2)
                {
                    string days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "[" + c + "]"));
                    string select_days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "ISNULL([" + c + "],0) AS [" + c + "]"));
                    return Json(new
                    {
                        payments = _db.Database.SqlQuery<decimal>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT amount,[day] = DATEPART(day, tdate) FROM doc.Payments WHERE DATEPART(month, tdate)=" + month + " AND DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (sum(amount) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        charges = _db.Database.SqlQuery<decimal>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT amount,[day] = DATEPART(day, tdate) FROM doc.CardCharges WHERE DATEPART(month, tdate)=" + month + " AND  DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (sum(amount) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList()
                    });
                }
                else
                {
                    string months = "[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12]";
                    return Json(new
                    {
                        payments = _db.Database.SqlQuery<decimal>(@"SELECT aData FROM( SELECT 
                                                              ISNULL([1],0) AS [1],ISNULL([2],0) AS [2],ISNULL([3],0) AS [3],ISNULL([4],0) AS [4],ISNULL([5],0) AS [5],ISNULL([6],0) AS [6],
                                                              ISNULL([7],0) AS [7],ISNULL([8],0) AS [8],ISNULL([9],0) AS [9],ISNULL([10],0) AS [10],ISNULL([11],0) AS [11],ISNULL([12],0) AS [12]
                                                             FROM (
                                                                   SELECT amount,[month] = DATEPART(month, tdate) FROM doc.Payments WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (sum(amount) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                        charges = _db.Database.SqlQuery<decimal>(@"SELECT aData FROM( SELECT 
                                                              ISNULL([1],0) AS [1],ISNULL([2],0) AS [2],ISNULL([3],0) AS [3],ISNULL([4],0) AS [4],ISNULL([5],0) AS [5],ISNULL([6],0) AS [6],
                                                              ISNULL([7],0) AS [7],ISNULL([8],0) AS [8],ISNULL([9],0) AS [9],ISNULL([10],0) AS [10],ISNULL([11],0) AS [11],ISNULL([12],0) AS [12]
                                                             FROM (
                                                                   SELECT amount,[month] = DATEPART(month, tdate) FROM doc.CardCharges WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (sum(amount) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList()
                    });
                }
            }
        }
        public JsonResult UpdateReturnedCard(int type, int month)
        {
            using (DataContext _db = new DataContext())
            {
                if (type == 2)
                {
                    string days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "[" + c + "]"));
                    string select_days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "ISNULL([" + c + "],0) AS [" + c + "]"));
                    return Json(new
                    {
                        Cancled = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, tdate) FROM dbo.ReturnedCards WHERE DATEPART(month, tdate)=" + month + " AND DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                    });
                }
                else
                {
                    string months = "[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12]";
                    return Json(new
                    {
                        Cancled = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT 
                                    ISNULL([1], 0) AS[1],ISNULL([2],0) AS[2],ISNULL([3],0) AS[3],ISNULL([4],0) AS[4],ISNULL([5],0) AS[5],ISNULL([6],0) AS[6],
                                    ISNULL([7],0) AS[7],ISNULL([8],0) AS[8],ISNULL([9],0) AS[9],ISNULL([10],0) AS[10],ISNULL([11],0) AS[11],ISNULL([12],0) AS[12]
                                    FROM(
                                    SELECT id,[month] = DATEPART(month, tdate) FROM dbo.ReturnedCards WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList()

                    });
                }
            }
        }

        public JsonResult ReturnedCardChart(int type, int month)
        {
            using (DataContext _db = new DataContext())
            {
                if (type == 2)
                {
                    string days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "[" + c + "]"));
                    string select_days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "ISNULL([" + c + "],0) AS [" + c + "]"));
                                            var rre = @"SELECT aData FROM(SELECT " + select_days + @"
                                                             FROM(
                                                                   SELECT r.ID,[day] = DATEPART(day, r.tdate) FROM book.Cards AS c
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate < c.finish_date and DATEPART(month, r.tdate) = " + month + " AND DATEPART(year, r.tdate) = " + DateTime.Now.Year + @") AS pr
                                                                   PIVOT(count(id) FOR[day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT(aData FOR fields in (" + days + @")) AS unpvt";
                    return Json(new
                    {

                        countactive = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT r.ID,[day] = DATEPART(day, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate < c.finish_date and DATEPART(month, r.tdate)=" + month + " AND DATEPART(year, r.tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        count1mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT r.ID,[day] = DATEPART(day, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE  r.tdate > c.finish_date and r.tdate < DATEADD(mm, 1, c.finish_date) and DATEPART(month, r.tdate)=" + month + " AND DATEPART(year, r.tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        count2mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT r.ID,[day] = DATEPART(day, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate>DATEADD(mm, 1, c.finish_date) and r.tdate < DATEADD(mm, 2, c.finish_date) and  DATEPART(month, r.tdate)=" + month + " AND DATEPART(year, r.tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        count3mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT r.ID,[day] = DATEPART(day, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate>DATEADD(mm,2, c.finish_date) and r.tdate < DATEADD(mm, 24, c.finish_date) and  DATEPART(month, r.tdate)=" + month + " AND DATEPART(year, r.tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        countpromo= _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT r.ID,[day] = DATEPART(day, r.tdate) FROM book.Cards as c
                                                                                    inner join doc.Subscribes as s on s.card_id=c.id
                                                                                    inner join doc.SubscriptionPackages as sp on sp.subscription_id=s.id 
                                                                                    inner join dbo.ReturnedCards as r on r.card_id=c.id where sp.package_id=304086) AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList()
                    });
                }
                else
                {
                    string months = "[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12]";
                        
                    return Json(new
                    {
                        countactive = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate < c.finish_date and DATEPART(year, r.tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                        count1mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE  r.tdate > c.finish_date and r.tdate < DATEADD(mm, 1, c.finish_date) and DATEPART(year, r.tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                        count2mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate>DATEADD(mm, 1, c.finish_date) and r.tdate < DATEADD(mm, 2, c.finish_date) and DATEPART(year, r.tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                        count3mont = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                            SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards AS c 
                                                                inner join dbo.ReturnedCards AS r on r.card_id = c.id WHERE r.tdate>DATEADD(mm, 2, c.finish_date) and r.tdate < DATEADD(mm, 24, c.finish_date) and DATEPART(year, r.tdate)=" + DateTime.Now.Year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                       countpromo= _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                            SELECT r.ID,[month] = DATEPART(month, r.tdate) FROM book.Cards as c
                                                                                    inner join doc.Subscribes as s on s.card_id=c.id
                                                                                    inner join doc.SubscriptionPackages as sp on sp.subscription_id=s.id 
                                                                                    inner join dbo.ReturnedCards as r on r.card_id=c.id where sp.package_id=304086 ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList()

                    });
                }
            }
        }
        public JsonResult UpdateDamageCard(int type, int month)
        {
            using (DataContext _db = new DataContext())
            {
                if (type == 2)
                {
                    string days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "[" + c + "]"));
                    string select_days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "ISNULL([" + c + "],0) AS [" + c + "]"));
                    return Json(new
                    {
                        damage = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, tdate) FROM dbo.Damage WHERE DATEPART(month, tdate)=" + month + " AND DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        damage_proces = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, tdate) FROM  dbo.Damage WHERE (status=-2 or  executor_id!=0) and DATEPART(month, tdate)=" + month + " AND DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        damage_coll = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, tdate) FROM  dbo.Damage WHERE (status=7 and executor_id=0) and DATEPART(month, tdate)=" + month + " AND DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList()
                    });
                }
                else
                {
                    string months = "[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12]";
                    return Json(new
                    {
                        damage = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT 
                    ISNULL([1], 0) AS[1],ISNULL([2],0) AS[2],ISNULL([3],0) AS[3],ISNULL([4],0) AS[4],ISNULL([5],0) AS[5],ISNULL([6],0) AS[6],
                    ISNULL([7],0) AS[7],ISNULL([8],0) AS[8],ISNULL([9],0) AS[9],ISNULL([10],0) AS[10],ISNULL([11],0) AS[11],ISNULL([12],0) AS[12]
                    FROM(
                    SELECT id,[month] = DATEPART(month, tdate) FROM dbo.Damage WHERE DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                        damage_proces = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT 
                    ISNULL([1], 0) AS[1],ISNULL([2],0) AS[2],ISNULL([3],0) AS[3],ISNULL([4],0) AS[4],ISNULL([5],0) AS[5],ISNULL([6],0) AS[6],
                    ISNULL([7],0) AS[7],ISNULL([8],0) AS[8],ISNULL([9],0) AS[9],ISNULL([10],0) AS[10],ISNULL([11],0) AS[11],ISNULL([12],0) AS[12]
                    FROM(
                    SELECT id,[month] = DATEPART(month, tdate) FROM dbo.Damage WHERE (status=-2 or  executor_id!=0) and DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                        damage_coll = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT 
                    ISNULL([1], 0) AS[1],ISNULL([2],0) AS[2],ISNULL([3],0) AS[3],ISNULL([4],0) AS[4],ISNULL([5],0) AS[5],ISNULL([6],0) AS[6],
                    ISNULL([7],0) AS[7],ISNULL([8],0) AS[8],ISNULL([9],0) AS[9],ISNULL([10],0) AS[10],ISNULL([11],0) AS[11],ISNULL([12],0) AS[12]
                    FROM(
                    SELECT id,[month] = DATEPART(month, tdate) FROM dbo.Damage WHERE (status=7 and executor_id=0) and DATEPART(year, tdate)=" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [month] in (" + months + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList()

                    });
                }
            }
        }
        public JsonResult UpdateMonthlyDeals(int year)
        {
            using (DataContext _db = new DataContext())
            {
                string months = "[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12]";
                return Json(new
                {
                    montages = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT id,[month] = DATEPART(month, tdate) FROM book.Cards WHERE DATEPART(year, tdate)=" + year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    orders = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT id,[month] = DATEPART(month, tdate) FROM doc.Orders WHERE DATEPART(year, tdate)=" + year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                    damages = _db.Database.SqlQuery<int>(@"SELECT aData FROM (
                                                             SELECT id,[month] = DATEPART(month, tdate) FROM doc.CardDamages WHERE DATEPART(year, tdate)=" + year + @"
                                                          ) AS pr
                                                          PIVOT (COUNT(id) FOR [month] in (" + months + @")) AS pvt
                                                          UNPIVOT (aData FOR fields in (" + months + @")) AS unpvt").ToList(),
                });
            }
        }

        public JsonResult PacketsByCities(string city)
        {
            using (DataContext _db = new DataContext())
            {
                string sql = @";WITH Packets (s_id, card_id, package_name)
AS
(
    SELECT s.id, s.card_id, STUFF((Select '+'+p.name from doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id ORDER BY p.name FOR XML PATH('')),1,1,'') FROM doc.Subscribes AS s WHERE s.status=1
)

SELECT r.card_status AS status, r.package, r.count FROM
(SELECT
dbo.GetCardStatus(cr.status) AS card_status,
q.package_name AS package,
(SELECT COUNT(*) FROM book.Cards AS c WHERE c.status != 4 AND EXISTS(SELECT * FROM Packets WHERE q.card_id=c.id AND EXISTS(SELECT * FROM doc.Subscribes WHERE id = q.s_id))) AS count

FROM Packets AS q 
INNER JOIN book.Cards AS cr ON cr.id=q.card_id
INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status !=4 AND c.city=N'" + city + "') AS r";

                return Json(_db.Database.SqlQuery<CardReport>(sql).ToList().GroupBy(c => c.status).Select(c => new
                {
                    status = c.Key,
                    count = c.GroupBy(cc => cc.package).Select(cc => new
                    {
                        packet = cc.Key,
                        count = cc.Count()
                    })
                }).ToList());
            }
        }

        public async Task<ActionResult> Logs(int page = 1)
        {
            if (!Utils.Utils.GetPermission("LOG_SHOW"))
                return new RedirectResult("/Main");

            using (DataContext _db = new DataContext())
            {
                DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
                int type = string.IsNullOrEmpty(Request["type"]) ? -1 : Utils.Utils.TryIntNegativeParse(Request["type"]);

                ViewBag.Types = (from LogType n in Enum.GetValues(typeof(LogType))
                                 select new { Id = (int)n, Text = Utils.Utils.GetEnumDescription(n) }).ToList();

                return View(await _db.Loggings
                    .AsNoTracking()
                    .Where(p => type == -1 ? true : p.Type == (LogType)type)
                    .Where(p => p.Tdate >= dateFrom && p.Tdate <= dateTo).Select(p => new LoggingList
                    {
                        UserName = p.User.Name,
                        Id = p.Id,
                        Mode = p.Mode,
                        Type = p.Type,
                        Value = p.TypeValue,
                        Tdate = p.Tdate,
                        UserGroupName = p.User.UserType.Name
                    }).OrderByDescending(c => c.Tdate).ToPagedListAsync(page, 30));
            }
        }

        public async Task<JsonResult> FilterLogs(string field, string letter)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            int type = Request["type"] == null ? -1 : Utils.Utils.TryIntParse(Request["type"]);
            string sql = getSqlString(type, dateFrom, dateTo, field, letter);
            using (DataContext _db = new DataContext())
            {
                return Json(await _db.Database.SqlQuery<LoggingList>(sql).ToListAsync());
            }
        }

        public async Task<PartialViewResult> GetLogDetails(int id)
        {
            using (DataContext _db = new DataContext())
            {
                return PartialView("~/Views/Main/_LogDetails.cshtml", await _db.LoggingItems.Where(l => l.LoggingId == id).ToListAsync());
            }
        }

        public JsonResult GetUserData(string sign)
        {
            using (DataContext _db = new DataContext())
            {
                if (sign == "user")
                {
                    return Json(_db.Users.Select(u => new { id = u.Id, name = u.Name }).ToList());
                }
                else
                {
                    return Json(_db.UserTypes.Select(u => new { id = u.Id, name = u.Name }).ToList());
                }
            }
        }

        public JsonResult GetPackages()
        {
            using (DataContext _db = new DataContext())
            {
                return Json(_db.Packages.Select(p => new { name = p.Id, value = p.Name }).ToList());
            }
        }

        public async Task<FileResult> ExportToExcel()
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            int type = Request["type"] == null ? -1 : Utils.Utils.TryIntParse(Request["type"]);

            string sql = getSqlString(type, dateFrom, dateTo, Request["field"], Request["letter"]);

            using (DataContext _db = new DataContext())
            {
                var data = await _db.Database.SqlQuery<LoggingList>(sql).ToListAsync();
                XElement element = new XElement("root",
                    new XElement("columns",
                        new XElement("name", "თარიღი"),
                        new XElement("name", "მომხმარებლის ჯგუფი"),
                        new XElement("name", "მომხმარებელი"),
                        new XElement("name", "ტიპი"),
                        new XElement("name", "მოქმედება"),
                        new XElement("name", "აღწერა")),
                    data.Select(c => new XElement("data",
                            new XElement("date", c.Tdate.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)),
                            new XElement("user_group", c.UserGroupName),
                            new XElement("user", c.UserName),
                            new XElement("type", Utils.Utils.GetEnumDescription(c.Type)),
                            new XElement("mode", Utils.Utils.GetEnumDescription(c.Mode)),
                            new XElement("value", c.Value)
                            )));

                return File(new Export().getExcelData("LogExport.xslt", element), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LogResult.xlsx");
            }
        }

        [HttpPost]
        public JsonResult DaylyCharge()
        {
            Jobs.ChargeCardJob job = new Jobs.ChargeCardJob();
            try
            {
                job.Execute(null);
                return Json("დარიცხვა შესრულდა");
            }
            catch
            {
                return Json("დარიცხვა ვერ შესრულდა");
            }
        }

        [HttpPost]
        public JsonResult DaylyMessage()
        {
            Jobs.InvoiceSendJob job = new Jobs.InvoiceSendJob();
            try
            {
                job.Execute(null);
                return Json("ინვოისი შესრულდა");
            }
            catch
            {
                return Json("ინვოისი ვერ შესრულდა");
            }
        }

        [NonAction]
        private string getSqlString(int type, DateTime dateFrom, DateTime dateTo, string field, string letter)
        {
            string inner_sql = string.Empty;
            string where = string.Empty;
            if (type == 2) //payment
            {
                inner_sql = "INNER JOIN " + _Tables[type] + " AS t ON t.id=l.[type_id] INNER JOIN book.Cards AS crd ON crd.id=t.card_id INNER JOIN book.Customers AS c ON c.id=crd.customer_id ";
            }
            else if (type == 5) //message
            {
                inner_sql = "INNER JOIN " + _Tables[type] + " AS t ON t.id=l.[type_id] INNER JOIN book.Customers AS c ON c.id=t.abonent_id ";
            }
            else if (type == 10) // subscribtions
            {
                inner_sql = "INNER JOIN " + _Tables[type] + " AS t ON t.id=l.[type_id] INNER JOIN book.Cards AS crd ON crd.id=t.card_id INNER JOIN doc.SubscriptionPackages AS c ON c.subscription_id=t.id ";
                where = "c.package_id=" + (field == "0" ? "c.package_id" : field) + " AND (crd.card_num+crd.abonent_num) LIKE N'" + letter + "%'";
            }
            else if (type == 11) // user actions
            {
                inner_sql = string.Empty;
                where = field == "user" ? "l.[user_id]=" + (letter == "" ? "l.[user_id]" : letter) : "u.[type]=" + (letter == "" ? "u.[type]" : letter);
            }
            else
            {
                inner_sql = type != -1 ? " INNER JOIN " + _Tables[type] + " AS c ON c.id=l.[type_id] " : " ";
                where = field + " LIKE N'%" + letter + "%'";
                where = where.Replace("+", "+' '+");
                if (field == "c.status" || field == "c.tower_id" || field == "cr.tower_id")
                    where = field + "=" + (letter == "" ? field : letter);
            }

            return @"SELECT l.id AS Id,u.name AS UserName, l.mode AS Mode, l.[type] AS [Type],l.type_value AS Value, l.tdate AS Tdate, ut.name AS UserGroupName  FROM config.Logging AS l " + inner_sql + @" 
                            INNER JOIN book.Users AS u ON u.id=l.[user_id] INNER JOIN book.UserTypes AS ut ON ut.id=u.[type] WHERE l.[type]=" + (type != -1 ? type.ToString() : "l.[type]") + @"
                             AND l.tdate BETWEEN '" + dateFrom.ToString("yyyy-MM-dd 00:00") + "' AND '" + dateTo.ToString("yyyy-MM-dd 23:59") + @"' 
                            AND " + where + " ORDER BY l.tdate DESC";
        }


    }
}