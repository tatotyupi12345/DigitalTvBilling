using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Controllers
{
    
    public class ReplacementController : Controller
    {
        private int pageSize = 20;
        private int page = 1;
        // GET: Replacement
        public ActionResult Index(string dt_from, string dt_to)
        {
            DateTime dateFrom, dateTo;
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
            ReRegistering registering = new ReRegistering();
            List<ReRegistering> reRegisterings = new List<ReRegistering>();
            List<ReRegistering> replacement = new List<ReRegistering>();
            using (DataContext _db = new DataContext())
            {

                 reRegisterings = _db.Database.SqlQuery<ReRegistering>($"SELECT * FROM dbo.ReRegistering where tdate between '{dateFrom}' and '{dateTo}' ").ToList();
                foreach (var item in reRegisterings)
                {

                    registering = _db.Database.SqlQuery<ReRegistering>($"SELECT c.id AS id,cc.id as card_id, c.name,c.lastname,c.code,c.tdate,c.phone1 AS phone,c.user_id FROM book.Cards as cc inner join  book.Customers AS c ON cc.customer_id=c.id where cc.id='{item.card_id}' ").FirstOrDefault();
                    replacement.Add(registering);
                }
                ViewBag.Replaments = replacement;
            }
            return View(reRegisterings);
        }
    }
}