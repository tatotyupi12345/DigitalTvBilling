using DigitalTVBilling.CallCenter.Infrastructure;
using DigitalTVBilling.Infrastructure.Static_2;
using DigitalTVBilling.Infrastructure.Static_2.Model;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Controllers
{
    public class Main2Controller : Controller
    {
        // GET: JuridicalContracts
        public ActionResult Index()
        {
            ViewBag.OrderDistinguishedAnswers = JsonConvert.SerializeObject(
                    new OrderDistinguishedAnswers(
                            new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                new ReserveFilter(
                                    Utils.Utils.GetRequestDate(Request["dt_from"], true),
                                    Utils.Utils.GetRequestDate(Request["dt_to"], false),
                                    0
                                )
                        ).Result()
                            .Select(c =>
                                new object[] { c.Name, c.Id }
                        )
                        );
            ViewBag.DamageDistinguishedAnswers = JsonConvert.SerializeObject(
                     new DamageDistinguishedAnswers(
                                   new SqlConnection(
                                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                    new ReserveFilter(
                                            Utils.Utils.GetRequestDate(Request["dt_from"], true),
                                            Utils.Utils.GetRequestDate(Request["dt_to"], false),
                                            0
                                    )
                         ).Result()
                            .Select(c =>
                                new object[] { c.Name, c.Id }
                         )
                         );

            ViewBag.CardStatus = JsonConvert.SerializeObject(
                                new
                                {
                                    Active = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Active"
                                      )
                                 ).Result(),
                                    Closed = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Closed"
                                      )
                                 ).Result(),

                                    Pause = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Pause"
                                      )
                                 ).Result(),
                                    Blocked = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Blocked"
                                      )
                                 ).Result(),

                                    Discontinued = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Discontinued"
                                      )
                                 ).Result(),

                                    Cancel = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Cancel"
                                      )
                                 ).Result(),
                                });

            return View(
                        new Users(
                            new SqlConnection(
                                    ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString
                            )
                        ).Result()
                );
        }
        public JsonResult OrderFilterDateTime(string dt_from, string dt_to, int user_id)
        {
            return Json(ViewBag.OrderDistinguishedAnswers = JsonConvert.SerializeObject(
                new OrderDistinguishedAnswers(
                                new SqlConnection(
                                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                    new ReserveFilter(
                                            Utils.Utils.GetRequestDate(Request["dt_from"], true),
                                            Utils.Utils.GetRequestDate(Request["dt_to"], false),
                                            user_id
                                    )
                            ).Result()
                                .Select(c =>
                                    new object[] { c.Name, c.Id }
                   )
                  )
                 );
        }
        public JsonResult DamageFilterDateTime(string dt_from, string dt_to, int user_id)
        {
            return Json(ViewBag.DamageDistinguishedAnswers = JsonConvert.SerializeObject(
                    new DamageDistinguishedAnswers(
                                   new SqlConnection(
                                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                    new ReserveFilter(
                                            Utils.Utils.GetRequestDate(Request["dt_from"], true),
                                            Utils.Utils.GetRequestDate(Request["dt_to"], false),
                                            user_id
                                    )
                        ).Result()
                          .Select(c =>
                             new object[] { c.Name, c.Id }
                        )
                      )
                 );
        }
        public JsonResult UpdateChargesStatus(int type, int month)
        {
            using (DataContext _db = new DataContext())
            {
                if (type == 2)
                {
                    string days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "[" + c + "]"));
                    string select_days = String.Join(",", Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, month)).Select(c => "ISNULL([" + c + "],0) AS [" + c + "]"));
                    var ssd = @"SELECT aData FROM(SELECT " + select_days + @"
                                                             FROM(
                                                                   SELECT id,[day] = DATEPART(day, close_tdate) FROM  doc.CardLogs WHERE
                                                                    (status = 3) AND DATEPART(month, close_tdate) = " + month + " AND DATEPART(year, close_tdate) = " + DateTime.Now.Year + @") AS pr
                                                                   PIVOT(count(id) FOR[day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT(aData FOR fields in (" + days + @")) AS unpvt";
                    return Json(new
                    {

                        Active = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, close_tdate) FROM  doc.CardLogs WHERE 
                                                                    (status = 0) AND DATEPART(month, close_tdate) = " + month + " AND DATEPART(year, close_tdate) =" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        Closed = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, close_tdate) FROM  doc.CardLogs WHERE 
                                                                    (status = 1) AND DATEPART(month, close_tdate) = " + month + " AND DATEPART(year, close_tdate) =" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),

                        Pause = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, close_tdate) FROM  doc.CardLogs WHERE 
                                                                    (status = 3) AND DATEPART(month, close_tdate) = " + month + " AND DATEPART(year, close_tdate) =" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        Blocked = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, close_tdate) FROM  doc.CardLogs WHERE 
                                                                    (status = 6) AND DATEPART(month, close_tdate) = " + month + " AND DATEPART(year, close_tdate) =" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        Discontinued = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, close_tdate) FROM  doc.CardLogs WHERE 
                                                                    (status = 17) AND DATEPART(month, close_tdate) = " + month + " AND DATEPART(year, close_tdate) =" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                        Cancel = _db.Database.SqlQuery<int>(@"SELECT aData FROM( SELECT " + select_days + @"
                                                             FROM (
                                                                   SELECT id,[day] = DATEPART(day, close_tdate) FROM  doc.CardLogs WHERE 
                                                                    (status = 5) AND DATEPART(month, close_tdate) = " + month + " AND DATEPART(year, close_tdate) =" + DateTime.Now.Year + @") AS pr
                                                                   PIVOT (count(id) FOR [day] in (" + days + @")) AS pvt) AS p
                                                                   UNPIVOT (aData FOR fields in (" + days + @")) AS unpvt").ToList(),
                    });
                }
                else
                {

                    return Json(new
                    {

                        Active = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Active"
                                      )
                                 ).Result(),
                        Closed = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Closed"
                                      )
                                 ).Result(),

                        Pause = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Pause"
                                      )
                                 ).Result(),
                        Blocked = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Blocked"
                                      )
                                 ).Result(),

                        Discontinued = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Discontinued"
                                      )
                                 ).Result(),

                        Cancel = new StatusResult(
                                                 new StatusQuery(
                                                     new SqlConnection(
                                                          ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                                     "Cancel"
                                      )
                                 ).Result(),
                    });
                }
            }
        }
    }
}